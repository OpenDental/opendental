using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.IO;

namespace OpenDental {
	public partial class FormAutoNoteExport:FormODBase {
		private UserOdPref _userPrefExpandedDefNums;

		public FormAutoNoteExport() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormAutoNoteExport_Load(object sender,System.EventArgs e) {
			_userPrefExpandedDefNums=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AutoNoteExpandedCats).FirstOrDefault();
			AutoNoteL.FillListTree(treeNotes,_userPrefExpandedDefNums);
		}

		/// <summary>Helper method that sets up the SaveFileDialog form and then returns that </summary>
		private SaveFileDialog ExportDialogSetup() {
			string fileName="autonotes.json";
			string initialDirectory=PrefC.GetString(PrefName.ExportPath);
			SaveFileDialog saveDialog=new SaveFileDialog();
			saveDialog.AddExtension=true;
			saveDialog.FileName=fileName;
			saveDialog.Filter="JSON files(*.json)|*.json";
			if(!Directory.Exists(initialDirectory)) {
				try {
					Directory.CreateDirectory(initialDirectory);
					saveDialog.InitialDirectory=initialDirectory;
				}
				catch {
					saveDialog.InitialDirectory=Path.GetTempPath();
				}
			}
			else {
				saveDialog.InitialDirectory=initialDirectory;
			}
			return saveDialog;
		}

		private void ToggleCheckboxes(TreeNodeCollection nodeList, bool value) {
			foreach(TreeNode node in nodeList) {
				if(node.Nodes!=null) {
					ToggleCheckboxes(node.Nodes,value);
				}
				node.Checked=value;
			}
		}

		private void ToggleCheckboxes(TreeNode node, bool value) {
			if(node.Nodes!=null) {
				ToggleCheckboxes(node.Nodes,value);
			}
			node.Checked=value;
		}

		private List<AutoNote> GetCheckedAutoNotes(TreeNodeCollection nodes) {
			List<AutoNote> listCheckedAutoNotes=new List<AutoNote>();
			foreach(TreeNode node in nodes) {
				if(node.Nodes!=null) {//If it has children
					listCheckedAutoNotes.AddRange(GetCheckedAutoNotes(node.Nodes));
				}
				if(node.Checked && node.Tag is AutoNote) {
					listCheckedAutoNotes.Add((AutoNote)node.Tag);
				}
			}
			return listCheckedAutoNotes;
		}

		private void butClear_Click(object sender,EventArgs e) {
			ToggleCheckboxes(treeNotes.Nodes,false);
		}

		private void butExport_Click(object sender,System.EventArgs e) {
			List<AutoNote> listCheckedAutoNotes=GetCheckedAutoNotes(treeNotes.Nodes);
			if(listCheckedAutoNotes.Count==0) {
				MsgBox.Show(this,"You must select at least one Auto Note to export.");
				return;
			}
			try {
				string fileName;
				if(ODBuild.IsWeb()) {
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
				List<SerializableAutoNote> serializableAutoNotes=AutoNotes.GetSerializableAutoNotes(listCheckedAutoNotes);
				List<AutoNoteControl> listAutoNoteControls=AutoNoteControls.GetListByParsingAutoNoteText(serializableAutoNotes);
				List<SerializableAutoNoteControl> serializableAutoNoteControls=AutoNoteControls.GetSerializableAutoNoteControls(listAutoNoteControls);
				AutoNotes.WriteAutoNotesToJson(serializableAutoNotes,serializableAutoNoteControls,fileName);
				SecurityLogs.MakeLogEntry(Permissions.AutoNoteQuickNoteEdit,0,"Auto Note Export");
				MsgBox.Show(this,"Auto Note(s) successfully exported.");
			}
			catch(Exception err) {
				FriendlyException.Show("AutoNote(s) failed to export.",err);
			}
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

		private void butCancel_Click(object sender,System.EventArgs e) {
			Close();
		}

	}
}
