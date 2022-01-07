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
		///<summary>Stores the subfolders and autonotes under this category (definition).</summary>
		private static Dictionary<long,NodeChildren> _dictChildNodesForDefNum;
		///<summary>Allows distinction of child node types as both categories and as single autonotes.</summary>
		private class NodeChildren {
			public List<TreeNode> ListChildDefNodes=new List<TreeNode>();
			public List<TreeNode> ListAutoNoteNodes=new List<TreeNode>();
		}

		public static void FillListTree(TreeView treeNotes,UserOdPref _userOdCurPref) {
			List<long> listExpandedDefNums=new List<long>();
			if(treeNotes.Nodes.Count==0 && _userOdCurPref!=null) {//if this is the fill on load, the node count will be 0, expanded node list from pref
				listExpandedDefNums=_userOdCurPref.ValueString.Split(',').Where(x => x!="" && x!="0").Select(x => PIn.Long(x)).ToList();
			}
			else {//either not fill on load or no user pref, store the expanded node state to restore after filling tree
						//only defs (category folders) can be expanded or have children nodes
				listExpandedDefNums=treeNotes.Nodes.OfType<TreeNode>().SelectMany(x => GetNodeAndChildren(x))
					.Where(x => x.IsExpanded && x.Tag is Def).Select(x => ((Def)x.Tag).DefNum).ToList();
			}
			TreeNode selectedNode=treeNotes.SelectedNode;
			TreeNode topNode=null;
			string topNodePath=treeNotes.TopNode?.FullPath;
			treeNotes.BeginUpdate();
			treeNotes.Nodes.Clear();//clear current tree contents
			_dictChildNodesForDefNum=Defs.GetDefsForCategory(DefCat.AutoNoteCats,true).GroupBy(x => x.ItemValue??"0")
				.ToDictionary(x => PIn.Long(x.Key),x => new NodeChildren() { ListChildDefNodes=x.Select(y => new TreeNode(y.ItemName,0,0) { Tag=y }).ToList() });
			Dictionary<long,List<TreeNode>> dictDefNumAutoNotes=AutoNotes.GetDeepCopy().GroupBy(x => x.Category)
				.ToDictionary(x => x.Key,x => x.Select(y => new TreeNode(y.AutoNoteName,1,1) { Tag=y }).ToList());
			foreach(KeyValuePair<long,List<TreeNode>> kvp in dictDefNumAutoNotes) {
				if(_dictChildNodesForDefNum.ContainsKey(kvp.Key)) {
					_dictChildNodesForDefNum[kvp.Key].ListAutoNoteNodes=kvp.Value;
				}
				else {
					_dictChildNodesForDefNum[kvp.Key]=new NodeChildren() { ListAutoNoteNodes=kvp.Value };
				}
			}
			List<TreeNode> listNodes=new List<TreeNode>();//all nodes to add to tree, categories and autonotes
			NodeChildren nodeChildren;
			if(_dictChildNodesForDefNum.TryGetValue(0,out nodeChildren)) {
				nodeChildren.ListChildDefNodes.ForEach(SetAllDescendantsForNode);
				listNodes.AddRange(nodeChildren.ListChildDefNodes);
				listNodes.AddRange(nodeChildren.ListAutoNoteNodes);
			}
			treeNotes.Nodes.AddRange(listNodes.OrderBy(x => x,new NodeSorter()).ToArray());//add node list to tree, after sorting
			List<TreeNode> listNodesCur=listNodes.SelectMany(x => GetNodeAndChildren(x)).ToList();//get flat list of all nodes, copy entire tree
			foreach(TreeNode nodeCur in listNodesCur) {
				if(!string.IsNullOrEmpty(topNodePath) && nodeCur.FullPath==topNodePath) {
					topNode=nodeCur;
				}
				if(nodeCur.Tag is Def && listExpandedDefNums.Contains(((Def)nodeCur.Tag).DefNum)) {
					nodeCur.Expand();
				}
				if(selectedNode==null) {
					continue;
				}
				if(Equals(nodeCur,selectedNode)) {
					treeNotes.SelectedNode=nodeCur;
				}
			}
			treeNotes.TopNode=topNode;
			if(topNode==null && treeNotes.Nodes.Count>0) {
				treeNotes.TopNode=treeNotes.SelectedNode??treeNotes.Nodes[0];
			}
			treeNotes.EndUpdate();
			treeNotes.Focus();
		}

		///<summary>Returns true if both nodes are tagged with Defs and both Defs have the same DefNum OR both nodes are tagged with AutoNotes and both
		///AutoNotes have the same AutoNoteNum.  If either node is null, returns false.</summary>
		private static bool Equals(TreeNode nodeA,TreeNode nodeB) {
			if(nodeA==null || nodeB==null) {
				return false;
			}
			if((nodeA.Tag is AutoNote && nodeB.Tag is AutoNote && ((AutoNote)nodeA.Tag).AutoNoteNum==((AutoNote)nodeB.Tag).AutoNoteNum)
				|| (nodeA.Tag is Def && nodeB.Tag is Def && ((Def)nodeA.Tag).DefNum==((Def)nodeB.Tag).DefNum)) {
				return true;
			}
			return false;
		}

		///<summary>Recursive function, returns a tree node with all descendants, including all auto note children for this def cat and all children for
		///any cat within this this cat.  Auto Notes that are at the 'root' level (considered uncategorized) have to be added separately after filling the
		///rest of the tree with this function and will be at the bottom of the root node list.</summary>
		private static void SetAllDescendantsForNode(TreeNode defNodeCur) {
			if(defNodeCur==null || defNodeCur.Tag is AutoNote) {
				return;
			}
			List<TreeNode> listChildNodes=new List<TreeNode>();
			NodeChildren nodeChildrenCur;
			if(_dictChildNodesForDefNum.TryGetValue(((Def)defNodeCur.Tag).DefNum,out nodeChildrenCur)) {
				nodeChildrenCur.ListChildDefNodes.ForEach(x => SetAllDescendantsForNode(x));
				listChildNodes.AddRange(nodeChildrenCur.ListChildDefNodes);
				listChildNodes.AddRange(nodeChildrenCur.ListAutoNoteNodes);
			}
			defNodeCur.Nodes.AddRange(listChildNodes.OrderBy(x => x,new NodeSorter()).ToArray());
		}

		///<summary>Returns a flat list containing this TreeNode and all of its descendant TreeNodes.</summary>
		public static List<TreeNode> GetNodeAndChildren(TreeNode treeNode,bool isCatsOnly=false) {
			List<TreeNode> listRetval=new List<TreeNode>();
			if(isCatsOnly && treeNode.Tag is AutoNote) {
				//guaranteed leaf node.
				return listRetval;
			}
			listRetval.Add(treeNode);
			for(int i=0;i<listRetval.Count;i++) {
				listRetval.AddRange(listRetval[i].Nodes.OfType<TreeNode>().Where(x => !isCatsOnly || x.Tag is Def));
			}
			return listRetval;
		}

		public static void SetCollapsed(TreeView treeNotes,bool doSetChecked) {
			TreeNode topNode=treeNotes.TopNode;
			TreeNode selectedNode=treeNotes.SelectedNode;
			treeNotes.BeginUpdate();
			if(doSetChecked) {
				while(topNode.Parent!=null) {//store the topNode's root to set the topNode after collapsing all nodes
					topNode=topNode.Parent;
				}
				while(selectedNode!=null && selectedNode.Parent!=null) {//store the selectedNode's root to select after collapsing
					selectedNode=selectedNode.Parent;
				}
				treeNotes.CollapseAll();
			}
			else {
				treeNotes.ExpandAll();
			}
			treeNotes.EndUpdate();
			if(selectedNode==null) {
				treeNotes.TopNode=topNode;//set TopNode if there is no SelectedNode
			}
			else {//reselect the node and ensure that it is visible after expanding or collapsing
				treeNotes.SelectedNode=selectedNode;
				treeNotes.SelectedNode.EnsureVisible();
				treeNotes.Focus();
			}
		}
	}
}
