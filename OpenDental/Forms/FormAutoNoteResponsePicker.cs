using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoNoteResponsePicker:FormODBase {
		///<summary>This will have the Response text and the chosen AutoNote in the format "Response text : {AutoNoteName}".</summary>
		public string AutoNoteResponseText;

		public FormAutoNoteResponsePicker() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNoteResponsePicker_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			AutoNotes.RefreshCache();
			List<AutoNote> listAutoNotes=AutoNotes.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("",100));
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(AutoNote autoNote in listAutoNotes) {
				row=new GridRow();
				row.Cells.Add(autoNote.AutoNoteName);
				row.Tag=autoNote;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Sets the AutoNoteResponseText with the selected AutoNote in the format "Auto Note Response Text : {AutoNoteName}".</summary>
		private void butOK_Click(object sender,EventArgs e) {
			if(string.IsNullOrEmpty(textResponseText.Text)) {
				MsgBox.Show(this,"Please enter a response text.");
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an AutoNote.");
				return;
			}
			AutoNote autoNoteSelected=gridMain.SelectedTag<AutoNote>();
			if(autoNoteSelected==null) {
				MsgBox.Show(this,"Invalid AutoNote selected. Please select a new one.");
				gridMain.SetAll(false);
				return;//This shouldn't happen.
			}
			//The AutoNoteResponseText should be in format "Auto Note Response Text : {AutoNoteName}"
			//This format is needed so the text processing logic can parse through it correctly.
			//If this format changes, we need to change the logic in FormAutoNoteCompose.PromptForAutoNotes()
			//If this format changes, you will also need to modify FormAutoNoteCompose.GetAutoNoteName() and FormAutoNoteCompose.GetAutoNoteResponseText
			AutoNoteResponseText=textResponseText.Text+" : {"+autoNoteSelected.AutoNoteName+"}";
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}