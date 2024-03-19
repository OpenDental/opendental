using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.IO;

namespace OpenDental {
	public partial class FormAutoNoteExport:FormODBase {
		private UserOdPref _userOdPrefDefNumsExpanded;

		public FormAutoNoteExport() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormAutoNoteExport_Load(object sender,System.EventArgs e) {
			_userOdPrefDefNumsExpanded=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			AutoNoteL.FillListTree(treeNotes,_userOdPrefDefNumsExpanded);
		}

		/// <summary>Helper method that sets up the SaveFileDialog form and then returns that </summary>
		private SaveFileDialog ExportDialogSetup() {
			string fileName="autonotes.json";
			string initialDirectory=PrefC.GetString(PrefName.ExportPath);
			SaveFileDialog saveFileDialog=new SaveFileDialog();
			saveFileDialog.AddExtension=true;
			saveFileDialog.FileName=fileName;
			saveFileDialog.Filter="JSON files(*.json)|*.json";
			if(Directory.Exists(initialDirectory)) {
				saveFileDialog.InitialDirectory=initialDirectory;
				return saveFileDialog;
			}
			try {
				Directory.CreateDirectory(initialDirectory);
				saveFileDialog.InitialDirectory=initialDirectory;
			}
			catch {
				saveFileDialog.InitialDirectory=Path.GetTempPath();
			}
			return saveFileDialog;
		}

		private void ToggleCheckboxes(TreeNodeCollection treeNodeCollection, bool value) {
			for(int i=0;i<treeNodeCollection.Count;i++) {
				if(treeNodeCollection[i].Nodes!=null) {
					ToggleCheckboxes(treeNodeCollection[i].Nodes,value);
				}
				treeNodeCollection[i].Checked=value;
			}
		}

		private void ToggleCheckboxes(TreeNode treeNode, bool value) {
			if(treeNode.Nodes!=null) {
				ToggleCheckboxes(treeNode.Nodes,value);
			}
			treeNode.Checked=value;
		}

		private List<AutoNote> GetCheckedAutoNotes(TreeNodeCollection treeNodeCollection) {
			List<AutoNote> listAutoNotesChecked=new List<AutoNote>();
			for(int i=0;i<treeNodeCollection.Count;i++) {
				if(treeNodeCollection[i].Nodes!=null) {//If it has children
					listAutoNotesChecked.AddRange(GetCheckedAutoNotes(treeNodeCollection[i].Nodes));
				}
				if(treeNodeCollection[i].Checked && treeNodeCollection[i].Tag is AutoNote) {
					listAutoNotesChecked.Add((AutoNote)treeNodeCollection[i].Tag);
				}
			}
			return listAutoNotesChecked;
		}

		private void butClear_Click(object sender,EventArgs e) {
			ToggleCheckboxes(treeNotes.Nodes,false);
		}

		private void butExport_Click(object sender,System.EventArgs e) {
			List<AutoNote> listAutoNotesChecked=GetCheckedAutoNotes(treeNotes.Nodes);
			if(listAutoNotesChecked.Count==0) {
				MsgBox.Show(this,"You must select at least one Auto Note to export.");
				return;
			}
			string fileName;
			if(ODEnvironment.IsCloudServer) {
				//file download dialog will come up later, after file is created.
				fileName="autonotes.json";
			}
			else {
				using(SaveFileDialog saveDialog=ExportDialogSetup()) {
					if(saveDialog.ShowDialog()!=DialogResult.OK) {
						return;//user canceled out of SaveFileDialog
					}
					fileName=saveDialog.FileName;
				}
			}
			List<SerializableAutoNote> listSerializableAutoNotes=AutoNotes.GetSerializableAutoNotes(listAutoNotesChecked);
			List<AutoNoteControl> listAutoNoteControls=AutoNoteControls.GetListByParsingAutoNoteText(listSerializableAutoNotes);
			List<SerializableAutoNoteControl> listSerializableAutoNoteControls=AutoNoteControls.GetSerializableAutoNoteControls(listAutoNoteControls);
			try {
				AutoNotes.WriteAutoNotesToJson(listSerializableAutoNotes,listSerializableAutoNoteControls,fileName);
			}
			catch(Exception err) {
				FriendlyException.Show("AutoNote(s) failed to export.",err);
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.AutoNoteQuickNoteEdit,0,"Auto Note Export");
			MsgBox.Show(this,"Auto Note(s) successfully exported.");
		}

		private void checkCollapse_CheckedChanged(object sender,EventArgs e) {
			AutoNoteL.SetCollapsed(treeNotes,checkCollapse.Checked);
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			ToggleCheckboxes(treeNotes.Nodes,true);
		}

		private void node_AfterCheck(object sender,TreeViewEventArgs e) {
			//Microsoft docs say to make this check if you're programatically checking boxes in BeforeCheck/AfterCheck handlers to prevent unexpected behavior
			if(e.Action!=TreeViewAction.Unknown) {
				ToggleCheckboxes(e.Node,e.Node.Checked);
			}
		}

	}
}