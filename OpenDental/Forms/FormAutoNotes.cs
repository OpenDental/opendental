using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.IO;
using Newtonsoft.Json;

namespace OpenDental {
	/// <summary>
	/// </summary>
	public partial class FormAutoNotes:FormODBase {
		public AutoNote AutoNoteCur;
		public bool IsSelectionMode;
		///<summary>On load, the UserOdPref that contains the comma delimited list of expanded category DefNums is retrieved from the database.  On close
		///the UserOdPref is updated with the current expanded DefNums.</summary>
		private UserOdPref _userOdCurPref;

		///<summary></summary>
		public FormAutoNotes(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNotes_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode) {
				butAdd.Visible=false;
				labelSelection.Visible=true;
			}
			_userOdCurPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			AutoNoteL.FillListTree(treeNotes,_userOdCurPref);
		}

		/// <summary>Helper method that sets up the SaveFileDialog form and then returns that </summary>
		private OpenFileDialog ImportDialogSetup() {
			OpenFileDialog openDialog=new OpenFileDialog();
			openDialog.Multiselect=false;
			openDialog.Filter="JSON files(*.json)|*.json";
			openDialog.InitialDirectory=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			return openDialog;
		}

		///<summary>Returns whether or not a node can be moved.
		///isSourceDef dictates whether the nodeCur is a definition or an autonote.
		///This is important because definitions are psuedo-directories and we need to guard against circular loops</summary>
		private bool IsValidDestination(TreeNode nodeCur,TreeNode nodeDest,bool isSourceDef) {
			//Null check just in case, destination node is already the parent of the selected node
			if(nodeCur==null || nodeCur.Parent==nodeDest) {
				return false;
			}
			//If the selected node is an auto note, it can move anywhere.
			//It was determined that the parent is different, so the node will actually move somewhere.
			if(!isSourceDef) {
				return true;
			}
			//The nodeCur is a definition, so we need to make sure it isn't trying to be moved to a decendant of itself.
			if(nodeDest!=null && nodeDest.FullPath.StartsWith(nodeCur.FullPath)) {
				return false;
			}
			return true;
		}

		private void treeNotes_MouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(e.Node==null || !(e.Node.Tag is AutoNote)) {
				return;
			}
			AutoNote noteCur=((AutoNote)e.Node.Tag).Copy();
			if(IsSelectionMode) {
				AutoNoteCur=noteCur;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormAutoNoteEdit FormA=new FormAutoNoteEdit();
			FormA.IsNew=false;
			FormA.AutoNoteCur=noteCur;
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				AutoNoteL.FillListTree(treeNotes,_userOdCurPref);
			}
		}

		private TreeNode _grayNode=null;//only used in treeNotes_DragOver to reduce flickering.

		private void treeNotes_DragOver(object sender,DragEventArgs e) {
			Point pt=treeNotes.PointToClient(new Point(e.X,e.Y));
			TreeNode nodeSelected=treeNotes.GetNodeAt(pt);
			if(_grayNode!=null && _grayNode!=nodeSelected) {
				_grayNode.BackColor=Color.White;
				_grayNode=null;
			}
			if(nodeSelected!=null && nodeSelected.BackColor!=Color.LightGray) {
				nodeSelected.BackColor=Color.LightGray;
				_grayNode=nodeSelected;
			}
			if(pt.Y<25) {
				MiscUtils.SendMessage(treeNotes.Handle,277,0,0);//Scroll Up
			}
			else if(pt.Y>treeNotes.Height-25) {
				MiscUtils.SendMessage(treeNotes.Handle,277,1,0);//Scroll down.
			}
		}

		private void treeNotes_ItemDrag(object sender,ItemDragEventArgs e) {
			treeNotes.SelectedNode=(TreeNode)e.Item;
			DoDragDrop(e.Item,DragDropEffects.Move);
		}

		private void treeNotes_DragEnter(object sender,DragEventArgs e) {
			e.Effect=DragDropEffects.Move;
		}

		private void treeNotes_DragDrop(object sender,DragEventArgs e) {
			if(_grayNode!=null) {
				_grayNode.BackColor=Color.White;
			}
			if(!e.Data.GetDataPresent("System.Windows.Forms.TreeNode",false)) { 
				return; 
			}
			TreeNode sourceNode=(TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
			if(sourceNode==null || !(sourceNode.Tag is Def || sourceNode.Tag is AutoNote)) {
				return;
			}
			TreeNode topNodeCur=treeNotes.TopNode;
			if(treeNotes.TopNode==sourceNode && sourceNode.PrevVisibleNode!=null) {
				//if moving the topNode to another category, make the previous visible node the topNode once the move is successful
				topNodeCur=sourceNode.PrevVisibleNode;
			}
			Point pt=((TreeView)sender).PointToClient(new Point(e.X,e.Y));
			TreeNode destNode=((TreeView)sender).GetNodeAt(pt);
			if(destNode==null || !(destNode.Tag is Def || destNode.Tag is AutoNote)) {//moving to root node (category 0)
				if(sourceNode.Parent==null) {//already at the root node, nothing to do
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Move the selected "+(sourceNode.Tag is AutoNote?"Auto Note":"category")+" to the root level?")) {
					return;
				}
				if(sourceNode.Tag is Def) {
					((Def)sourceNode.Tag).ItemValue="";
				}
				else {//must be an AutoNote
					((AutoNote)sourceNode.Tag).Category=0;
				}
			}
			else {//moving to another folder (category)
				if(destNode.Tag is AutoNote) {
					destNode=destNode.Parent;//if destination is AutoNote, set destination to the parent, which is the category def node for the note
				}
				if(!IsValidDestination(sourceNode,destNode,sourceNode.Tag is Def)) {
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,
					"Move the selected "+(sourceNode.Tag is AutoNote?"Auto Note":"category")+(destNode==null?" to the root level":"")+"?"))
				{
					return;
				}
				//destNode will be null if a root AutoNote was selected as the destination
				long destDefNum=(destNode==null?0:((Def)destNode.Tag).DefNum);
				if(sourceNode.Tag is Def) {
					((Def)sourceNode.Tag).ItemValue=(destDefNum==0?"":destDefNum.ToString());//make a DefNum of 0 be a blank string in the db, not a "0" string
				}
				else {//must be an AutoNote
					((AutoNote)sourceNode.Tag).Category=destDefNum;
				}
			}
			if(sourceNode.Tag is Def) {
				Defs.Update((Def)sourceNode.Tag);
				DataValid.SetInvalid(InvalidType.Defs);
			}
			else {//must be an AutoNote
				AutoNotes.Update((AutoNote)sourceNode.Tag);
				DataValid.SetInvalid(InvalidType.AutoNotes);
			}
			treeNotes.TopNode=topNodeCur;//if sourceNode was the TopNode and was moved, make the TopNode the previous visible node
			AutoNoteL.FillListTree(treeNotes,_userOdCurPref);
		}

		private void checkCollapse_CheckedChanged(object sender,System.EventArgs e) {
			AutoNoteL.SetCollapsed(treeNotes,checkCollapse.Checked);
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AutoNoteQuickNoteEdit)) {
				return;
			}
			long selectedDefNum=0;
			if(treeNotes.SelectedNode?.Tag is Def) {
				selectedDefNum=((Def)treeNotes.SelectedNode.Tag).DefNum;
			}
			else if(treeNotes.SelectedNode?.Tag is AutoNote) {
				selectedDefNum=((AutoNote)treeNotes.SelectedNode.Tag).Category;
			}
			using FormAutoNoteEdit FormA=new FormAutoNoteEdit();
			FormA.IsNew=true;
			FormA.AutoNoteCur=new AutoNote() { Category=selectedDefNum };
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			treeNotes.SelectedNode?.Expand();//expanding an AutoNote has no effect, and if nothing selected nothing to expand
			AutoNoteL.FillListTree(treeNotes,_userOdCurPref);
			if((FormA.AutoNoteCur?.AutoNoteNum??0)>0) {//select the newly added note in the tree
				treeNotes.SelectedNode=treeNotes.Nodes.OfType<TreeNode>().SelectMany(x => AutoNoteL.GetNodeAndChildren(x)).Where(x => x.Tag is AutoNote)
					.FirstOrDefault(x => ((AutoNote)x.Tag).AutoNoteNum==FormA.AutoNoteCur.AutoNoteNum);
				treeNotes.SelectedNode?.EnsureVisible();
				treeNotes.Focus();
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void butExport_Click(object sender,System.EventArgs e) {
			using FormAutoNoteExport export=new FormAutoNoteExport();
			export.ShowDialog();
		}

		private void butImport_Click(object sender,EventArgs e) {
			try {
				using(OpenFileDialog openDialog=ImportDialogSetup()) {
					if(openDialog.ShowDialog()!=DialogResult.OK) {
						return; //User cancelled out of OpenFileDialog
					}
					string fileContents=File.ReadAllText(openDialog.FileName);
					TransferableAutoNotes import=JsonConvert.DeserializeObject<TransferableAutoNotes>(fileContents);
					AutoNoteControls.RemoveDuplicatesFromList(import.AutoNoteControls,import.AutoNotes);
					AutoNoteControls.InsertBatch(import.AutoNoteControls);
					AutoNotes.InsertBatch(import.AutoNotes);
					DataValid.SetInvalid(InvalidType.AutoNotes);
					AutoNoteL.FillListTree(treeNotes,_userOdCurPref);
					SecurityLogs.MakeLogEntry(Permissions.AutoNoteQuickNoteEdit,0,
						$"Auto Note Import. {import.AutoNotes.Count} new Auto Notes, {import.AutoNoteControls.Count} new Prompts");
					MsgBox.Show(Lans.g(this,"Auto Notes successfully imported!")+"\r\n"+import.AutoNotes.Count+" "+Lans.g(this,"new Auto Notes")
						+"\r\n"+import.AutoNoteControls.Count+" "+Lans.g(this,"new Prompts"));
				}
			}
			catch(Exception err) {
				FriendlyException.Show(Lans.g(this,"Auto Note(s) failed to import."),err);
			}
		}

		private void FormAutoNotes_FormClosing(object sender,FormClosingEventArgs e) {
			//store the current node expanded state for this user
			List<long> listExpandedDefNums=treeNotes.Nodes.OfType<TreeNode>()
				.SelectMany(x => AutoNoteL.GetNodeAndChildren(x,true))
				.Where(x => x.IsExpanded)
				.Select(x => ((Def)x.Tag).DefNum)
				.Where(x => x>0).ToList();
			if(_userOdCurPref==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.AutoNoteExpandedCats,
					ValueString=string.Join(",",listExpandedDefNums)
				});
			}
			else {
				_userOdCurPref.ValueString=string.Join(",",listExpandedDefNums);
				UserOdPrefs.Update(_userOdCurPref);
			}
		}
	}

	///<summary>Sorting class used to sort a MethodInfo list by Name.</summary>
	public class NodeSorter:IComparer<TreeNode> {

		public int Compare(TreeNode x,TreeNode y) {
			if(x.Tag is Def && y.Tag is AutoNote) {
				return -1;
			}
			if(x.Tag is AutoNote && y.Tag is Def) {
				return 1;
			}
			if(x.Tag is Def && y.Tag is Def) {
				Def defX=(Def)x.Tag;
				Def defY=(Def)y.Tag;
				if(defX.ItemOrder!=defY.ItemOrder) {
					return defX.ItemOrder.CompareTo(defY.ItemOrder);
				}
			}
			//either both nodes are AutoNote nodes or both are Def nodes and both have the same ItemOrder (shouldn't happen), sort alphabetically
			return x.Text.CompareTo(y.Text);
		}
	}

}
