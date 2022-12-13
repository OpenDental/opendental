using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskListBlocks:FormODBase {
		///<summary>A List of the task lists that the current user wants to block pop ups for.  Filled on load.</summary>
		private List<UserOdPref> _listUserOdPrefsBlocks;
		private List<UserOdPref> _listUserOdPrefsDB;
		///<summary>list to hold changed task list subscriptions.</summary>
		private List<BlockedTaskPref> _listBlockedTaskPrefs=new List<BlockedTaskPref>();
		private List<TaskList> _listTaskListsAll=new List<TaskList>();
		///<summary>Set to true when settings the checkmarks of parents so we don't roll back down through children for each parent recursivly.</summary>
		private bool _isCheckingParents=false;
		
		///<summary>Loads up a list of task lists that the currently logged in user is subscribed to.
		///Allows the user to selectivly block task lists that they are subscribed to.</summary>
		public FormTaskListBlocks() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary> map tasklist number to userodpref</summary>
		private class BlockedTaskPref {
			public UserOdPref UserOdPref;
			public long TaskListNum;
		}

		private void FormTaskListBlock_Load(object sender,EventArgs e) {
			_listTaskListsAll=TaskLists.GetAll();//Used so we don't need to acces the database multiple times
			_listUserOdPrefsBlocks=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.TaskListBlock);
			//We pull the list then save it so the sync moethod is able to run correctly.  
			//This correctly fixes users having duplicate task list preferences in the databse.
			_listUserOdPrefsDB=_listUserOdPrefsBlocks.Select(x => x.Clone()).ToList();
			_listUserOdPrefsBlocks=_listUserOdPrefsBlocks.GroupBy(x => x.Fkey).Select(x => x.First()).ToList();
			InitializeTree();
		}

		///<summary>Fetches the subscriptions that the user is currently subscribed too ad adds them to the treeview.</summary>
		private void InitializeTree() {
			treeSubscriptions.Nodes.Clear();
			List<TaskList> listTaskListsSubs=TaskLists.RefreshUserTrunk(Security.CurUser.UserNum);
			//Only want active task lists that have no archived ancestors or no ancestors at all.
			listTaskListsSubs.RemoveAll(x => x.TaskListStatus==TaskListStatusEnum.Archived);
			listTaskListsSubs.RemoveAll((x => TaskLists.IsAnchorTaskListArchived2(_listTaskListsAll,x)));
			BuildTaskListTree(listTaskListsSubs);
			treeSubscriptions.ExpandAll();
		}

		#region Building the Tree
		/// <summary>Creates a dictionary of all task lists then makes a tree based on user subscriptions.
		/// First we take the passed in listTaskListSubs and fill out all the parents missing from the list
		///	e.g: If a person is subscribed to [B] but not it's parent [A], we add [A] to the list so the
		///		task list hierarchy is correctly displayed for the user.
		/// </summary>
		private void BuildTaskListTree(List<TaskList> listTaskListsSubs) {
			//Add users inbox to the task list.
			long inboxNum=Security.CurUser.TaskListInBox;
			if(inboxNum!=0) {
				TaskList taskList=listTaskListsSubs.Find(x => x.TaskListNum==inboxNum);
				if(taskList!=null) {
					listTaskListsSubs.Add(taskList);
				}
			}
			//Create a list of all leaves downstream from user's subscriptions.
			List<TaskList> listTaskListsLeafSubs=new List<TaskList>();
			for(int i=0;i<listTaskListsSubs.Count;i++) {
				listTaskListsLeafSubs.AddRange(GetLeafSubsFromTask(listTaskListsSubs[i],listTaskListsSubs));//Adds down stream leaf nodes.
			}
			//Create tree of TaskList subscriptions
			treeSubscriptions.Nodes.AddRange(TreeBuilder(listTaskListsLeafSubs).ToArray());//Works its way up to root nodes from given leaf nodes.
			for(int i=0;i<treeSubscriptions.Nodes.Count;i++) {
				SetCheckBoxes(treeSubscriptions.Nodes[i]);
			}
			treeSubscriptions.Sort();//default sort is alphabetical by node text.
		}

		///<summary>Recursively returns a list of all leaf nodes down stream from the given parent taskListNode.</summary>
		private List<TaskList> GetLeafSubsFromTask(TaskList taskListNode,List<TaskList> listTaskListsSubs) {
			List<TaskList> listTaskListsChildren=TaskLists.RefreshChildren(taskListNode.TaskListNum,Security.CurUser.UserNum,Security.CurUser.TaskListInBox,TaskType.All);
			if(listTaskListsChildren.Count==0) {//base case: is a leaf
				return new List<TaskList>() { taskListNode };
			}
			List<TaskList> listTaskListsLeaves=new List<TaskList>();
			for(int i=0;i<listTaskListsChildren.Count;i++) {
				if(listTaskListsSubs.Contains(listTaskListsChildren[i])) {//This node is already in our list of tasklist subscriptions
					continue;//Avoid traversing the same route down the tree twice, reduces Db calls.
				}
				listTaskListsLeaves.AddRange(GetLeafSubsFromTask(listTaskListsChildren[i],listTaskListsSubs));
			}
			return listTaskListsLeaves;
		}

		///<summary>Builds a tree a nodes from a list of subscribed TaskLists.  Fleshes out the subscriptions list, then build the tree.</summary>
		private List<TreeNode> TreeBuilder(List<TaskList> _listTaskListsSubs) {
			//jordan 2022-11-01-Warning!
			//This entire method and associated methods do not comply with our current patterns.
			//There should be no dictionaries. But it was too complex to easily rewrite, so we left it in place.
			//If anyone ever needs to overhaul it, rewriting this method should come first, and I will be involved.
			//The basic strategy would be to chase the branches up from the tips to the roots.
			//Possibly add them in one pass, and then set their parent ref in another pass. It avoids all the complexity below.
			//----------------------------------------------------------------------------------------------------------------------
			//Setup dictionaries and list of Task Lists
			Dictionary<long, TreeNode> dictLookup=new Dictionary<long, TreeNode>();
			List<TreeNode> listTreeNodesRoot=new List<TreeNode>();
			//Remove duplicate lists.
			_listTaskListsSubs=_listTaskListsSubs.GroupBy(x => x.TaskListNum).Select(y => y.First()).ToList();
			Dictionary<long,TaskList> dictTaskListSubs=_listTaskListsSubs.ToDictionary(x => x.TaskListNum);
			//Completly fill out subscribed list with parent TaskLists
			for(int i=0;i<_listTaskListsSubs.Count;i++) {
				dictTaskListSubs=GetAllTaskListsRecursive(dictTaskListSubs, _listTaskListsSubs[i].TaskListNum);
			}
			//Iterate through complete list and build tree of connections
			List<TaskList> listTaskLists=dictTaskListSubs.Values.ToList();
			for(int i=0;i<listTaskLists.Count;i++) {
				TreeNode treeNode;
				if(dictLookup.TryGetValue(listTaskLists[i].TaskListNum, out treeNode)) {
					treeNode.Text=listTaskLists[i].Descript;
					treeNode.Tag=listTaskLists[i].TaskListNum;
				}
				else {
					treeNode=new TreeNode();
					treeNode.Tag=Tag=listTaskLists[i].TaskListNum;
					treeNode.Text=Text=listTaskLists[i].Descript;
					dictLookup.Add(listTaskLists[i].TaskListNum,treeNode);
				}
				if(listTaskLists[i].Parent==0) {
					listTreeNodesRoot.Add(treeNode);
					continue;
				}
				TreeNode treeNodeParent;
				if(!dictLookup.TryGetValue(listTaskLists[i].Parent, out treeNodeParent)) {
					treeNodeParent=new TreeNode();
					dictLookup.Add(listTaskLists[i].Parent, treeNodeParent);
				}
				treeNodeParent.Nodes.Add(treeNode);
			}
			return listTreeNodesRoot;
		}


		///<summary>Goes through the given dictionary and works to add all parents of given TaskLists.</summary>
		private Dictionary<long,TaskList> GetAllTaskListsRecursive(Dictionary<long, TaskList> dictCurTaskLists, long taskListNum) {
			//No such Task List exists, so we return what was passed to us.
			if(taskListNum==0) {
				return dictCurTaskLists;
			}
			////Grab the TaskList then add all its parents and ancestors.
			TaskList taskList = _listTaskListsAll.Find(x => x.TaskListNum==taskListNum);
			if(taskList==null) {
				return dictCurTaskLists;
			}
			dictCurTaskLists=GetAllTaskListsRecursive(dictCurTaskLists,taskList.Parent);  //Recursion
			if(!dictCurTaskLists.ContainsKey(taskListNum)) {
				dictCurTaskLists.Add(taskListNum,taskList);
			}
			return dictCurTaskLists;
		}
		#endregion


		#region Checking

		/// <summary>Recursivly goes through the tree and sets all nodes to checked.</summary>
		private void RecursiveSetAllChecked(TreeNode treeNode, bool isChecked) {
			for(int i=0;i<treeNode.Nodes.Count;i++) {
				RecursiveSetAllChecked(treeNode.Nodes[i], isChecked);//Recursion
			}
			treeNode.Checked=isChecked;
		}

		/// <summary> Start from the roots of the tree, and work toward leaves.  Sets the node to checked if needed.
		private void SetCheckBoxes(TreeNode treeNode) {
			treeNode.Checked=false;//Unchecked if no block exists yet.
			if(_listUserOdPrefsBlocks.Exists(x => x.Fkey==(long)treeNode.Tag && PIn.Bool(x.ValueString))) {
				treeNode.Checked=true;
			}
			//Deal with children
			for(int i=0;i<treeNode.Nodes.Count;i++) {
				SetCheckBoxes(treeNode.Nodes[i]);
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
			bool isNodeChecked=e.Node.Checked;
			for(int i=0;i<e.Node.Nodes.Count;i++) {
				if(e.Node.Nodes[i].Checked!=isNodeChecked) {
					e.Node.Nodes[i].Checked=isNodeChecked;//Activates their own AfterCheck event
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
		private void nodesCheckUp(TreeNode treeNode) {
			bool allChildrenChecked=true;
			for(int i=0;i<treeNode.Nodes.Count;i++) {
				if(treeNode.Nodes[i].Checked==false) {
					allChildrenChecked=false;
					break;
				}
			}
			//isCheckingParents should be true, so AfterCheck shouldn't be triggered for children
			treeNode.Checked=allChildrenChecked;
			if(treeNode.Parent!=null) {
				nodesCheckUp(treeNode.Parent);//Recursion
			}
		}
#endregion

		/// <summary>Handler to set all nodes as checked.</summary>
		private void butSetAll_Click(object sender,EventArgs e) {
			for(int i=0;i<treeSubscriptions.Nodes.Count;i++) {
				RecursiveSetAllChecked(treeSubscriptions.Nodes[i],true);//Recursion
			}
		}

		/// <summary>Handler to set all nodes as unchecked</summary>
		private void butSetNone_Click(object sender,EventArgs e) {
			for(int i=0;i<treeSubscriptions.Nodes.Count;i++) {
				RecursiveSetAllChecked(treeSubscriptions.Nodes[i],false);//Recursion
			}
		}

		///<summary>Goes through tree and sets up changes to the TaskList block preferences list.</summary>
		private void SetBlockedPrefsRecursive(TreeNode treeNode) {
			for(int i=0;i<treeNode.Nodes.Count;i++) {
				SetBlockedPrefsRecursive(treeNode.Nodes[i]);//Recursion
			}
			//Create preference
			UserOdPref userOdPref=new UserOdPref();
			userOdPref.Fkey=(long)treeNode.Tag;
			userOdPref.FkeyType=UserOdFkeyType.TaskListBlock;
			userOdPref.UserNum=Security.CurUser.UserNum;
			userOdPref.ValueString=POut.Bool(treeNode.Checked);
			// Add preference to list of preferences
			BlockedTaskPref blockedTaskPref = new BlockedTaskPref();
			blockedTaskPref.TaskListNum=(long)treeNode.Tag;
			blockedTaskPref.UserOdPref=userOdPref;
			_listBlockedTaskPrefs.Add(blockedTaskPref);
		}

		//~15ms with 8 TaskLists, about 1 frame @ 60fps
		///<summary>Gets the changed preferences for the tree, then updates the database with the changes.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			//Setup all the changed preferences
			for(int i=0;i<treeSubscriptions.Nodes.Count;i++) {
				SetBlockedPrefsRecursive(treeSubscriptions.Nodes[i]);
			}
			//Add new preferences and changes to database
			for(int i = 0;i<_listBlockedTaskPrefs.Count;i++) {
				UserOdPref userOdPref = _listUserOdPrefsBlocks.Find(x => x.Fkey==_listBlockedTaskPrefs[i].UserOdPref.Fkey);
				if(userOdPref==null) {
					continue;
				}
				_listBlockedTaskPrefs[i].UserOdPref.UserOdPrefNum=userOdPref.UserOdPrefNum;
			}
			if(UserOdPrefs.Sync(_listBlockedTaskPrefs.Select(x => x.UserOdPref).ToList(),_listUserOdPrefsDB)) {
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			DialogResult=DialogResult.OK;
			this.Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			this.Close();
		}
	}
}
