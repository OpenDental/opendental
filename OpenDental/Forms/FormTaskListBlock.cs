using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskListBlocks:FormODBase {
		///<summary>A List of the task lists that the current user wants to block pop ups for.  Filled on load.</summary>
		private List<UserOdPref> _listUserOdPrefTaskListBlocks;
		private List<UserOdPref> _listUserDBPrefs;
		///<summary>Dictionary to hold changed task list subscriptions.</summary>
		private Dictionary<long,UserOdPref> _dictBlockedTaskPrefs=new Dictionary<long, UserOdPref>();
		private Dictionary<long,TaskList> _dictAllTaskLists=new Dictionary<long, TaskList>();
		///<summary>Set to true when settings the checkmarks of parents so we don't roll back down through children for each parent recursivly.</summary>
		private bool _isCheckingParents=false;
		
		///<summary>Loads up a list of task lists that the currently logged in user is subscribed to.
		///Allows the user to selectivly block task lists that they are subscribed to.</summary>
		public FormTaskListBlocks() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTaskListBlock_Load(object sender,EventArgs e) {
			_dictAllTaskLists=TaskLists.GetAll().ToDictionary(x => x.TaskListNum);//Used so we don't need to acces the database multiple times
			_listUserOdPrefTaskListBlocks=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskListBlock);
			//We pull the list then save it so the sync moethod is able to run correctly.  
			//This correctly fixes users having duplicate task list preferences in the databse.
			_listUserDBPrefs=_listUserOdPrefTaskListBlocks.Select(x => x.Clone()).ToList();
			_listUserOdPrefTaskListBlocks=_listUserOdPrefTaskListBlocks.GroupBy(x => x.Fkey).Select(x => x.First()).ToList();
			InitializeTree();
		}

		///<summary>Fetches the subscriptions that the user is currently subscribed too ad adds them to the treeview.</summary>
		private void InitializeTree() {
			treeSubscriptions.Nodes.Clear();
			List<TaskList> listTaskListSubs=TaskLists.RefreshUserTrunk(Security.CurUser.UserNum);
			//Only want active task lists that have no archived ancestors or no ancestors at all.
			listTaskListSubs.RemoveAll(x => x.TaskListStatus==TaskListStatusEnum.Archived 
				|| TaskLists.IsAncestorTaskListArchived(ref _dictAllTaskLists,x));
			BuildTaskListTree(listTaskListSubs);
			treeSubscriptions.ExpandAll();
		}

		#region Building the Tree
		/// <summary>Creates a dictionary of all task lists then makes a tree based on user subscriptions.
		/// First we take the passed in listTaskListSubs and fill out all the parents missing from the list
		///	e.g: If a person is subscribed to [B] but not it's parent [A], we add [A] to the list so the
		///		task list hierarchy is correctly displayed for the user.
		/// </summary>
		private void BuildTaskListTree(List<TaskList> listTaskListSubs) {
			//Add users inbox to the task list.
			long inboxNum=Security.CurUser.TaskListInBox;
			if(inboxNum!=0) {
				listTaskListSubs.Add(_dictAllTaskLists[inboxNum]);
			}
			//Create a list of all leaves downstream from user's subscriptions.
			List<TaskList> listLeafSubs=new List<TaskList>();
			foreach(TaskList taskList in listTaskListSubs) {
				listLeafSubs.AddRange(GetLeafSubsFromTask(taskList,listTaskListSubs));//Adds down stream leaf nodes.
			}
			//Create tree of TaskList subscriptions
			treeSubscriptions.Nodes.AddRange(TreeBuilder(listLeafSubs).ToArray());//Works its way up to root nodes from given leaf nodes.
			foreach(TreeNode roots in treeSubscriptions.Nodes) {
				SetCheckBoxes(roots);
			}
			treeSubscriptions.Sort();//default sort is alphabetical by node text.
		}

		///<summary>Recursively returns a list of all leaf nodes down stream from the given parent taskListNode.</summary>
		private List<TaskList> GetLeafSubsFromTask(TaskList taskListNode,List<TaskList> listTaskListSubs) {
			List<TaskList> children=TaskLists.RefreshChildren(taskListNode.TaskListNum,Security.CurUser.UserNum,Security.CurUser.TaskListInBox,TaskType.All);
			if(children.Count==0) {//base case: is a leaf
				return new List<TaskList>() { taskListNode };
			}
			List<TaskList> listLeaves=new List<TaskList>();
			foreach(TaskList child in children) {
				if(listTaskListSubs.Contains(child)) {//This node is already in our list of tasklist subscriptions
					continue;//Avoid traversing the same route down the tree twice, reduces Db calls.
				}
				listLeaves.AddRange(GetLeafSubsFromTask(child,listTaskListSubs));
			}
			return listLeaves;
		}

		///<summary>Builds a tree a nodes from a list of subscribed TaskLists.  Fleshes out the subscriptions list, then build the tree.</summary>
		private List<TreeNode> TreeBuilder(List<TaskList> _listTaskSubs) {
			//Setup dictionaries and list of Task Lists
			Dictionary<long, TreeNode> dictLookup=new Dictionary<long, TreeNode>();
			List<TreeNode> listRootNodes=new List<TreeNode>();
			//Remove duplicate lists.
			_listTaskSubs=_listTaskSubs.GroupBy(x => x.TaskListNum).Select(y => y.First()).ToList();
			Dictionary<long,TaskList> dictTaskListSubs=_listTaskSubs.ToDictionary(x => x.TaskListNum);
			//Completly fill out subscribed list with parent TaskLists
			foreach(TaskList task in _listTaskSubs) {
				dictTaskListSubs=GetAllTaskListsRecursive(dictTaskListSubs, task.TaskListNum);
			}
			//Iterate through complete list and build tree of connections
			foreach(TaskList taskList in dictTaskListSubs.Values) {
				TreeNode ourNode;
				if(dictLookup.TryGetValue(taskList.TaskListNum, out ourNode)) {
					ourNode.Text=taskList.Descript;
					ourNode.Tag=taskList.TaskListNum;
				}
				else {
					ourNode=new TreeNode() { Tag=taskList.TaskListNum, Text=taskList.Descript };
					dictLookup.Add(taskList.TaskListNum,ourNode);
				}
				if(taskList.Parent==0) {
					listRootNodes.Add(ourNode);
				}
				else {
					TreeNode parentNode;
					if(!dictLookup.TryGetValue(taskList.Parent, out parentNode)) {
						parentNode=new TreeNode();
						dictLookup.Add(taskList.Parent, parentNode);
					}
					parentNode.Nodes.Add(ourNode);
				}
			}
			return listRootNodes;
		}

		///<summary>Goes through the given dictionary and works to add all parents of given TaskLists.</summary>
		private Dictionary<long,TaskList> GetAllTaskListsRecursive(Dictionary<long, TaskList> dictCurTaskLists, long taskListNum) {
			//No such Task List exists, so we return what was passed to us.
			if(taskListNum==0) {
				return dictCurTaskLists;
			}
			//Grab the TaskList then add all it's parents and ancestors.
			TaskList taskList=_dictAllTaskLists[taskListNum];
			if(taskList==null) {
				return dictCurTaskLists;
			}
			dictCurTaskLists=GetAllTaskListsRecursive(dictCurTaskLists, taskList.Parent);	//Recursion
			if(!dictCurTaskLists.ContainsKey(taskListNum)) {
					dictCurTaskLists.Add(taskListNum,taskList);
			}
			return dictCurTaskLists;
		}
		#endregion

		#region Checking

		/// <summary>Recursivly goes through the tree and sets all nodes to checked.</summary>
		private void RecursiveSetAllChecked(TreeNode node, bool Checked) {
			foreach(TreeNode child in node.Nodes) {
				RecursiveSetAllChecked(child, Checked);	//Recursion
			}
			node.Checked=Checked;
		}

		/// <summary> Start from the roots of the tree, and work toward leaves.  Sets the node to checked if needed.
		private void SetCheckBoxes(TreeNode node) {
			node.Checked=false;//Unchecked if no block exists yet.
			if(_listUserOdPrefTaskListBlocks.Exists(x => x.Fkey==(long)node.Tag && PIn.Bool(x.ValueString))) {
				node.Checked=true;
			}
			//Deal with children
			foreach(TreeNode child in node.Nodes) {
				SetCheckBoxes(child);//Recursion
			}
		}
		
		///<summary>Handles the user clicking on the node text to activate the checkbox</summary>
		private void treeSubscriptions_NodeMouseClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(e.X-e.Node.Bounds.X>0 && e.X<e.Node.Bounds.Width+e.Node.Bounds.X) {
				e.Node.Checked=!e.Node.Checked;
			}
		}
		/// <summary>Is activated when a node is checked or unchecked.</summary>
		private void treeSubscriptions_AfterCheck(object sender,TreeViewEventArgs e) {
			if(_isCheckingParents) {
				return;
			}
			bool nodeChecked=e.Node.Checked;
			foreach(TreeNode branch in e.Node.Nodes) {
				if(branch.Checked!=nodeChecked) {
					branch.Checked=nodeChecked;	//Activates their own AfterCheck event
				}
			}
			//If all children are checked, the parent should be checked too.
			//We'll work our way up the list.
			if(e.Node.Parent==null) {
				return;
			}
			//Lock children from being iterated through because of parent changes
			_isCheckingParents=true;
			nodesCheckUp(e.Node.Parent);
			_isCheckingParents=false;
		}

		///<summary>Works it's way up from a given node, checking the parent if all it's children are checked.  
		///	Be sure to set isCheckingParents to true so the treeSubscriptions_AfterCheck isn't triggered
		/// </summary>
		private void nodesCheckUp(TreeNode curNode) {
			bool allChildrenChecked=true;
			foreach(TreeNode child in curNode.Nodes) {
				if(child.Checked==false) {
					allChildrenChecked=false;
					break;
				}
			}
			//isCheckingParents should be true, so AfterCheck shouldn't be triggered for children
			curNode.Checked=allChildrenChecked;
			if(curNode.Parent!=null) {
				nodesCheckUp(curNode.Parent);	//Recursion
			}
		}
#endregion

		/// <summary>Handler to set all nodes as checked.</summary>
		private void butSetAll_Click(object sender,EventArgs e) {
			foreach(TreeNode node in treeSubscriptions.Nodes) {
				RecursiveSetAllChecked(node,true);	//Recursion
			}
		}

		/// <summary>Handler to set all nodes as unchecked</summary>
		private void butSetNone_Click(object sender,EventArgs e) {
			foreach(TreeNode node in treeSubscriptions.Nodes) {
				RecursiveSetAllChecked(node,false);	//Recursion
			}
		}

		///<summary>Goes through tree and sets up changes to the TaskList block preferences dictionary.</summary>
		private void SetDictPrefsRecursive(TreeNode node) {
			foreach(TreeNode child in node.Nodes) {
				SetDictPrefsRecursive(child);	//Recursion
			}
			//Create preference
			UserOdPref pref=new UserOdPref();
			pref.Fkey=(long)node.Tag;
			pref.FkeyType=UserOdFkeyType.TaskListBlock;
			pref.UserNum=Security.CurUser.UserNum;
			pref.ValueString=POut.Bool(node.Checked);
			//Add preference to dictionary of preferences
			_dictBlockedTaskPrefs[(long)node.Tag]=pref;
		}

		//~15ms with 8 TaskLists, about 1 frame @ 60fps
		///<summary>Gets the changed preferences for the tree, then updates the database with the changes.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			//Setup all the changed preferences
			foreach(TreeNode node in treeSubscriptions.Nodes) {
				SetDictPrefsRecursive(node);
			}
			//Add new preferences and changes to database
			foreach(UserOdPref editPref in _dictBlockedTaskPrefs.Values) {
				if(_listUserOdPrefTaskListBlocks.Exists(x => x.Fkey==editPref.Fkey)) {
					editPref.UserOdPrefNum=_listUserOdPrefTaskListBlocks.Find(x => x.Fkey==editPref.Fkey).UserOdPrefNum;
				}
			}
			UserOdPrefs.Sync(_dictBlockedTaskPrefs.Select(x => x.Value).ToList(),_listUserDBPrefs);
			DialogResult=DialogResult.OK;
			this.Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			this.Close();
		}
	}
}
