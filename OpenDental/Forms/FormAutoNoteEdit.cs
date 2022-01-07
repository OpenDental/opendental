using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {

	public partial class FormAutoNoteEdit :FormODBase {
		public bool IsNew;
		public AutoNote AutoNoteCur;
		private int textSelectionStart;
		private List<AutoNoteControl> _listAutoNoteControls;

		public FormAutoNoteEdit() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoNoteEdit_Load(object sender, EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AutoNoteQuickNoteEdit,true)) {
				butAdd.Enabled=false;
				butDelete.Enabled=false;
				butOK.Enabled=false;
				textMain.ReadOnly=true;
				textMain.BackColor=SystemColors.Window;
				textBoxAutoNoteName.ReadOnly=true;
				textBoxAutoNoteName.BackColor=SystemColors.Window;
			}
			else {//user has permission to edit auto notes
				gridMain.CellDoubleClick+=new ODGridClickEventHandler(gridMain_CellDoubleClick);
			}
			textBoxAutoNoteName.Text=AutoNoteCur.AutoNoteName;
			textMain.Text=AutoNoteCur.MainText;
			FillGrid();
		}

		///<summary></summary>
		private void FillGrid() {
			AutoNoteControls.RefreshCache();
			_listAutoNoteControls=AutoNoteControls.GetDeepCopy(false);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAutoNoteControls.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listAutoNoteControls[i].Descript);  
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAutoNoteControlEdit FormA=new FormAutoNoteControlEdit();
			FormA.ControlCur=_listAutoNoteControls[e.Row];
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAutoNoteControlEdit FormA=new FormAutoNoteControlEdit();
			AutoNoteControl control=new AutoNoteControl();
			control.ControlType="Text";
			FormA.ControlCur=control;
			FormA.IsNew=true;
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butInsert_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a prompt first.");
				return;
			}
			string fieldStr=_listAutoNoteControls[gridMain.GetSelectedIndex()].Descript;
			if(textSelectionStart < textMain.Text.Length-1) {
				textMain.Text=textMain.Text.Substring(0,textSelectionStart)
					+"[Prompt:\""+fieldStr+"\"]"
					+textMain.Text.Substring(textSelectionStart);
			}
			else{//otherwise, just tack it on the end
				textMain.Text+="[Prompt:\""+fieldStr+"\"]";
			}
			textMain.Select(textSelectionStart+fieldStr.Length+11,0);
			textMain.Focus();
		}

		private void textMain_Leave(object sender,EventArgs e) {
			textSelectionStart=textMain.SelectionStart;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this autonote?")){
				return;
			}
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			AutoNotes.Delete(AutoNoteCur.AutoNoteNum);
			DataValid.SetInvalid(InvalidType.AutoNotes);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			AutoNoteCur.AutoNoteName=textBoxAutoNoteName.Text;
			AutoNoteCur.MainText=textMain.Text;
			if(IsNew) {
				AutoNotes.Insert(AutoNoteCur);
			}
			else {
				AutoNotes.Update(AutoNoteCur);
			}
			DataValid.SetInvalid(InvalidType.AutoNotes);
			DialogResult=DialogResult.OK;
		}


		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		

		
	}
}