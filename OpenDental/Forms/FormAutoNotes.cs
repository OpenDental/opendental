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
		private UserOdPref _userOdPref;

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
			_userOdPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			AutoNoteL.FillListTree(treeNotes,_userOdPref);
		}

		/// <summary>Helper method that sets up the SaveFileDialog form and then returns that </summary>
		private OpenFileDialog ImportDialogSetup() {
			OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=false;
			openFileDialog.Filter="JSON files(*.json)|*.json";
			openFileDialog.InitialDirectory=Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			return openFileDialog;
		}

		///<summary>Returns whether or not a node can be moved.
		///isSourceDef dictates whether the nodeCur is a definition or an autonote.
		///This is important because definitions are psuedo-directories and we need to guard against circular loops</summary>
		private bool IsValidDestination(TreeNode treeNode,TreeNode treeNodeDestination,bool isSourceDef) {
			//Null check just in case, destination node is already the parent of the selected node
			if(treeNode==null || treeNode.Parent==treeNodeDestination) {
				return false;
			}
			//If the selected node is an auto note, it can move anywhere.
			//It was determined that the parent is different, so the node will actually move somewhere.
			if(!isSourceDef) {
				return true;
			}
			//The nodeCur is a definition, so we need to make sure it isn't trying to be moved to a decendant of itself.
			if(treeNodeDestination!=null && treeNodeDestination.FullPath.StartsWith(treeNode.FullPath)) {
				return false;
			}
			return true;
		}

		private void treeNotes_MouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(e.Node==null || !(e.Node.Tag is AutoNote)) {
				return;
			}
			AutoNote autoNote=((AutoNote)e.Node.Tag).Copy();
			if(IsSelectionMode) {
				AutoNoteCur=autoNote;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormAutoNoteEdit formAutoNoteEdit=new FormAutoNoteEdit();
			formAutoNoteEdit.IsNew=false;
			formAutoNoteEdit.AutoNoteCur=autoNote;
			formAutoNoteEdit.ShowDialog();
			if(formAutoNoteEdit.DialogResult==DialogResult.OK) {
				AutoNoteL.FillListTree(treeNotes,_userOdPref);
			}
		}

		private TreeNode _treeNodeGray=null;//only used in treeNotes_DragOver to reduce flickering.

		private void treeNotes_DragOver(object sender,DragEventArgs e) {
			Point point=treeNotes.PointToClient(new Point(e.X,e.Y));
			TreeNode treeNodeSelected=treeNotes.GetNodeAt(point);
			if(_treeNodeGray!=null && _treeNodeGray!=treeNodeSelected) {
				_treeNodeGray.BackColor=Color.White;
				_treeNodeGray=null;
			}
			if(treeNodeSelected!=null && treeNodeSelected.BackColor!=Color.LightGray) {
				treeNodeSelected.BackColor=Color.LightGray;
				_treeNodeGray=treeNodeSelected;
			}
			if(point.Y<25) {
				MiscUtils.SendMessage(treeNotes.Handle,277,0,0);//Scroll Up
			}
			else if(point.Y>treeNotes.Height-25) {
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
			if(_treeNodeGray!=null) {
				_treeNodeGray.BackColor=Color.White;
			}
			if(!e.Data.GetDataPresent("System.Windows.Forms.TreeNode",false)) { 
				return; 
			}
			TreeNode treeNodeSource=(TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
			if(treeNodeSource==null || !(treeNodeSource.Tag is Def || treeNodeSource.Tag is AutoNote)) {
				return;
			}
			//User is trying to drag and drop a definition entity. Make sure they have permission to edit definitions before continuing.
			if(treeNodeSource.Tag is Def && !Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			TreeNode treeNodeTop=treeNotes.TopNode;
			if(treeNotes.TopNode==treeNodeSource && treeNodeSource.PrevVisibleNode!=null) {
				//if moving the topNode to another category, make the previous visible node the topNode once the move is successful
				treeNodeTop=treeNodeSource.PrevVisibleNode;
			}
			Point point=((TreeView)sender).PointToClient(new Point(e.X,e.Y));
			TreeNode treeNodeDestination=((TreeView)sender).GetNodeAt(point);
			if(treeNodeDestination==null || !(treeNodeDestination.Tag is Def || treeNodeDestination.Tag is AutoNote)) {//moving to root node (category 0)
				if(treeNodeSource.Parent==null) {//already at the root node, nothing to do
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Move the selected "+(treeNodeSource.Tag is AutoNote?"Auto Note":"category")+" to the root level?")) {
					return;
				}
				if(treeNodeSource.Tag is Def) {
					((Def)treeNodeSource.Tag).ItemValue="";
				}
				else {//must be an AutoNote
					((AutoNote)treeNodeSource.Tag).Category=0;
				}
			}
			else {//moving to another folder (category)
				if(treeNodeDestination.Tag is AutoNote) {
					treeNodeDestination=treeNodeDestination.Parent;//if destination is AutoNote, set destination to the parent, which is the category def node for the note
				}
				if(!IsValidDestination(treeNodeSource,treeNodeDestination,treeNodeSource.Tag is Def)) {
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,
					"Move the selected "+(treeNodeSource.Tag is AutoNote?"Auto Note":"category")+(treeNodeDestination==null?" to the root level":"")+"?"))
				{
					return;
				}
				//destNode will be null if a root AutoNote was selected as the destination
				long defNumDest=(treeNodeDestination==null?0:((Def)treeNodeDestination.Tag).DefNum);
				if(treeNodeSource.Tag is Def) {
					((Def)treeNodeSource.Tag).ItemValue=(defNumDest==0?"":defNumDest.ToString());//make a DefNum of 0 be a blank string in the db, not a "0" string
				}
				else {//must be an AutoNote
					((AutoNote)treeNodeSource.Tag).Category=defNumDest;
				}
			}
			if(treeNodeSource.Tag is Def) {
				DefL.Update((Def)treeNodeSource.Tag);
				DataValid.SetInvalid(InvalidType.Defs);
			}
			else {//must be an AutoNote
				AutoNotes.Update((AutoNote)treeNodeSource.Tag);
				DataValid.SetInvalid(InvalidType.AutoNotes);
			}
			treeNotes.TopNode=treeNodeTop;//if sourceNode was the TopNode and was moved, make the TopNode the previous visible node
			AutoNoteL.FillListTree(treeNotes,_userOdPref);
		}

		private void checkCollapse_CheckedChanged(object sender,System.EventArgs e) {
			AutoNoteL.SetCollapsed(treeNotes,checkCollapse.Checked);
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AutoNoteQuickNoteEdit)) {
				return;
			}
			long defNum=0;
			if(treeNotes.SelectedNode?.Tag is Def) {
				defNum=((Def)treeNotes.SelectedNode.Tag).DefNum;
			}
			else if(treeNotes.SelectedNode?.Tag is AutoNote) {
				defNum=((AutoNote)treeNotes.SelectedNode.Tag).Category;
			}
			using FormAutoNoteEdit formAutoNoteEdit=new FormAutoNoteEdit();
			formAutoNoteEdit.IsNew=true;
			formAutoNoteEdit.AutoNoteCur=new AutoNote() { Category=defNum };
			formAutoNoteEdit.ShowDialog();
			if(formAutoNoteEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			treeNotes.SelectedNode?.Expand();//expanding an AutoNote has no effect, and if nothing selected nothing to expand
			AutoNoteL.FillListTree(treeNotes,_userOdPref);
			if(formAutoNoteEdit.AutoNoteCur!=null && formAutoNoteEdit.AutoNoteCur.AutoNoteNum>0) {//select the newly added note in the tree
				treeNotes.SelectedNode=treeNotes.Nodes.OfType<TreeNode>().SelectMany(x => AutoNoteL.GetNodeAndChildren(x)).Where(x => x.Tag is AutoNote)
					.FirstOrDefault(x => ((AutoNote)x.Tag).AutoNoteNum==formAutoNoteEdit.AutoNoteCur.AutoNoteNum);
				treeNotes.SelectedNode?.EnsureVisible();
				treeNotes.Focus();
			}
		}

		private void butExport_Click(object sender,System.EventArgs e) {
			using FormAutoNoteExport formAutoNoteExport=new FormAutoNoteExport();
			formAutoNoteExport.ShowDialog();
		}

		private void butImport_Click(object sender,EventArgs e) {
			string importFilePath;
			if(ODCloudClient.IsAppStream) {
				importFilePath=ODCloudClient.ImportFileForCloud();
				if(importFilePath.IsNullOrEmpty()) {
					return; //User cancelled out of OpenFileDialog
				}
			}
			else {
				using OpenFileDialog openFileDialog=ImportDialogSetup();
				if(openFileDialog.ShowDialog()!=DialogResult.OK) {
					return; //User cancelled out of OpenFileDialog
				}
				importFilePath=openFileDialog.FileName;
			}
			string fileContents;
			try {
				fileContents=File.ReadAllText(importFilePath);
			}
			catch(Exception err) {
				FriendlyException.Show(Lans.g(this,"Auto Note(s) failed to import."),err);
				return;
			}
			TransferableAutoNotes transferableAutoNotesImport=JsonConvert.DeserializeObject<TransferableAutoNotes>(fileContents);
			AutoNoteControls.RemoveDuplicatesFromList(transferableAutoNotesImport.AutoNoteControls,transferableAutoNotesImport.AutoNotes);
			AutoNoteControls.InsertBatch(transferableAutoNotesImport.AutoNoteControls);
			AutoNotes.InsertBatch(transferableAutoNotesImport.AutoNotes);
			DataValid.SetInvalid(InvalidType.AutoNotes);
			AutoNoteL.FillListTree(treeNotes,_userOdPref);
			SecurityLogs.MakeLogEntry(EnumPermType.AutoNoteQuickNoteEdit,0,
				$"Auto Note Import. {transferableAutoNotesImport.AutoNotes.Count} new Auto Notes, {transferableAutoNotesImport.AutoNoteControls.Count} new Prompts");
			MsgBox.Show(Lans.g(this,"Auto Notes successfully imported!")+"\r\n"+transferableAutoNotesImport.AutoNotes.Count+" "+Lans.g(this,"new Auto Notes")
				+"\r\n"+transferableAutoNotesImport.AutoNoteControls.Count+" "+Lans.g(this,"new Prompts"));
		}

		private void FormAutoNotes_FormClosing(object sender,FormClosingEventArgs e) {
			//store the current node expanded state for this user
			List<long> listDefNumsExpanded=treeNotes.Nodes.OfType<TreeNode>()
				.SelectMany(x => AutoNoteL.GetNodeAndChildren(x,true))
				.Where(x => x.IsExpanded)
				.Select(x => ((Def)x.Tag).DefNum)
				.Where(x => x>0).ToList();
			if(_userOdPref==null) {
				UserOdPref userOdPref=new UserOdPref();
				userOdPref.UserNum=Security.CurUser.UserNum;
				userOdPref.FkeyType=UserOdFkeyType.AutoNoteExpandedCats;
				userOdPref.ValueString=string.Join(",",listDefNumsExpanded);
				UserOdPrefs.Insert(userOdPref);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
				return;
			}
			UserOdPref userOdPrefOld=_userOdPref.Clone();
			_userOdPref.ValueString=string.Join(",",listDefNumsExpanded);
			if(UserOdPrefs.Update(_userOdPref,userOdPrefOld)) {
				//Only need to signal cache refresh on change.
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
		}
	}

	///<summary>Sorting class used to sort a MethodInfo list by Name.</summary>
	public class NodeSorter:IComparer<TreeNode> {

		public int Compare(TreeNode treeNode1,TreeNode treeNode2) {
			if(treeNode1.Tag is Def && treeNode2.Tag is AutoNote) {
				return -1;
			}
			if(treeNode1.Tag is AutoNote && treeNode2.Tag is Def) {
				return 1;
			}
			if(treeNode1.Tag is Def && treeNode2.Tag is Def) {
				Def defX=(Def)treeNode1.Tag;
				Def defY=(Def)treeNode2.Tag;
				if(defX.ItemOrder!=defY.ItemOrder) {
					return defX.ItemOrder.CompareTo(defY.ItemOrder);
				}
			}
			//either both nodes are AutoNote nodes or both are Def nodes and both have the same ItemOrder (shouldn't happen), sort alphabetically
			return treeNode1.Text.CompareTo(treeNode2.Text);
		}
	}

}