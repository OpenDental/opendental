using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	class AutoNoteL {

		public static void FillListTree(TreeView treeViewNotes,UserOdPref userOdPref) {
			List<long> listDefNumsExpanded=new List<long>();
			if(treeViewNotes.Nodes.Count==0 && userOdPref!=null) {//if this is the fill on load, the node count will be 0, expanded node list from pref
				listDefNumsExpanded=userOdPref.ValueString.Split(',').Where(x => x!="" && x!="0").Select(x => PIn.Long(x)).ToList();
			}
			else {//either not fill on load or no user pref, store the expanded node state to restore after filling tree
						//only defs (category folders) can be expanded or have children nodes
				listDefNumsExpanded=treeViewNotes.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))
					.Where(x => x.IsExpanded && x.Tag is Def).Select(x => ((Def)x.Tag).DefNum).ToList();
			}
			TreeNode treeNodeSelected=treeViewNotes.SelectedNode;
			TreeNode treeNodeTop=null;
			string topNodePath=treeViewNotes.TopNode?.FullPath;
			treeViewNotes.BeginUpdate();
			treeViewNotes.Nodes.Clear();//clear current tree contents
			List<Def> listDefs = Defs.GetDefsForCategory(DefCat.AutoNoteCats,true);
			List<AutoNote> listAutoNotes = AutoNotes.GetDeepCopy();
			for(int i = 0;i<listDefs.Count;i++) {
				if(listDefs[i].ItemValue!=""){
					continue;
				}
				//add categories and their autonotes recursively
				TreeNode treeNodeCat=CreateNodeAndChildren(listDefs[i].DefNum,listDefs,listAutoNotes,treeViewNotes);
				treeViewNotes.Nodes.Add(treeNodeCat);
			}
			//List<TreeNode> listTreeNodesOutsideCat = new List<TreeNode>();
			for(int i = 0;i<listAutoNotes.Count;i++) {
				if(listAutoNotes[i].Category!=0) {//because we only want the ones with no categories
					continue;
				}
				//add node to root
				TreeNode treeNodeAutoNote = new TreeNode(listAutoNotes[i].AutoNoteName,1,1);
				treeNodeAutoNote.Tag=listAutoNotes[i];
				treeViewNotes.Nodes.Add(treeNodeAutoNote);
			}
			//Create a list of all the tree nodes and add them
			List<TreeNode> listTreeNodes=new List<TreeNode>();
			for(int i=0;i<treeViewNotes.Nodes.Count;i++){
				listTreeNodes.Add(treeViewNotes.Nodes[i]);
			}
			List<TreeNode> listTreeNodesFlat = listTreeNodes.SelectMany(x => GetNodeAndChildren(x)).ToList();//get flat list of all nodes, copy entire tree
			for(int i=0;i<listTreeNodesFlat.Count;i++){
				if(!string.IsNullOrEmpty(topNodePath) && listTreeNodesFlat[i].FullPath==topNodePath) {
					treeNodeTop=listTreeNodesFlat[i];
				}
				if(listTreeNodesFlat[i].Tag is Def && listDefNumsExpanded.Contains(((Def)listTreeNodesFlat[i].Tag).DefNum)) {
					listTreeNodesFlat[i].Expand();
				}
				if(treeNodeSelected==null) {
					continue;
				}
				if(Equals(listTreeNodesFlat[i],treeNodeSelected)) {
					treeViewNotes.SelectedNode=listTreeNodesFlat[i];
				}
			}
			treeViewNotes.TopNode=treeNodeTop;//set scrolling pos
			if(treeNodeTop==null && treeViewNotes.Nodes.Count>0) {
				treeViewNotes.TopNode=treeViewNotes.SelectedNode??treeViewNotes.Nodes[0];
			}
			treeViewNotes.EndUpdate();
			treeViewNotes.Focus();
		}

		///<summary>Recursive. Adds current node and all children to the specified parent. Returns node to add.</summary>
		private static TreeNode CreateNodeAndChildren(long defNumCat,List<Def> listDefs,List<AutoNote> listAutoNotes,TreeView treeViewNotes){
			//Creates the current cat
			Def def=listDefs.Find(x=>x.DefNum==defNumCat);
			TreeNode treeNodeCat = new TreeNode(def.ItemName,0,0);
			treeNodeCat.Tag=def;
			//Make a list of the child cats
			List<Def> listDefsChildren=listDefs.FindAll(x=>x.ItemValue==defNumCat.ToString());
			//Add child categories recursively:
			for(int i=0;i<listDefsChildren.Count;i++){
				TreeNode treeNode=CreateNodeAndChildren(listDefsChildren[i].DefNum,listDefs,listAutoNotes,treeViewNotes);
				treeNodeCat.Nodes.Add(treeNode);
			}
			//Add AutoNotes for current cat
			List<AutoNote> listAutoNotesForCat=listAutoNotes.FindAll(x=>x.Category==defNumCat);
			for(int i=0;i<listAutoNotesForCat.Count;i++){
				TreeNode treeNodeAutoNote = new TreeNode(listAutoNotesForCat[i].AutoNoteName,1,1);
				treeNodeAutoNote.Tag=listAutoNotesForCat[i];
				treeNodeCat.Nodes.Add(treeNodeAutoNote);
			}
			return treeNodeCat;
		}

		///<summary>Returns true if both nodes are tagged with Defs and both Defs have the same DefNum OR both nodes are tagged with AutoNotes and both
		///AutoNotes have the same AutoNoteNum.  If either node is null, returns false.</summary>
		private static bool Equals(TreeNode treeNodeA,TreeNode treeNodeB) {
			if(treeNodeA==null || treeNodeB==null) {
				return false;
			}
			if((treeNodeA.Tag is AutoNote && treeNodeB.Tag is AutoNote && ((AutoNote)treeNodeA.Tag).AutoNoteNum==((AutoNote)treeNodeB.Tag).AutoNoteNum)
				|| (treeNodeA.Tag is Def && treeNodeB.Tag is Def && ((Def)treeNodeA.Tag).DefNum==((Def)treeNodeB.Tag).DefNum)) {
				return true;
			}
			return false;
		}

		///<summary>Returns a flat list containing this TreeNode and all of its descendant TreeNodes.</summary>
		public static List<TreeNode> GetNodeAndChildren(TreeNode treeNode,bool isCatsOnly=false) {
			List<TreeNode> listTreeNodesRetVal=new List<TreeNode>();
			if(isCatsOnly && treeNode.Tag is AutoNote) {
				//guaranteed leaf node.
				return listTreeNodesRetVal;
			}
			listTreeNodesRetVal.Add(treeNode);
			for(int i=0;i<listTreeNodesRetVal.Count;i++) {
				listTreeNodesRetVal.AddRange(listTreeNodesRetVal[i].Nodes.OfType<TreeNode>().Where(x => !isCatsOnly || x.Tag is Def));
			}
			return listTreeNodesRetVal;
		}

		public static void SetCollapsed(TreeView treeView,bool doSetChecked) {
			TreeNode treeNodeTop=treeView.TopNode;
			TreeNode treeNodeSelected=treeView.SelectedNode;
			treeView.BeginUpdate();
			if(doSetChecked) {
				while(treeNodeTop.Parent!=null) {//store the topNode's root to set the topNode after collapsing all nodes
					treeNodeTop=treeNodeTop.Parent;
				}
				while(treeNodeSelected!=null && treeNodeSelected.Parent!=null) {//store the selectedNode's root to select after collapsing
					treeNodeSelected=treeNodeSelected.Parent;
				}
				treeView.CollapseAll();
			}
			else {
				treeView.ExpandAll();
			}
			treeView.EndUpdate();
			if(treeNodeSelected==null) {
				treeView.TopNode=treeNodeTop;//set TopNode if there is no SelectedNode
			}
			else {//reselect the node and ensure that it is visible after expanding or collapsing
				treeView.SelectedNode=treeNodeSelected;
				treeView.SelectedNode.EnsureVisible();
				treeView.Focus();
			}
		}
	}
}
