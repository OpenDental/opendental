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
		private int _textSelectionStartNum;
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
			if(Security.IsAuthorized(EnumPermType.AutoNoteQuickNoteEdit,true)) {//user has permission to edit auto notes
				gridMain.CellDoubleClick+=new ODGridClickEventHandler(gridMain_CellDoubleClick);
			}
			else {
				butAdd.Enabled=false;
				butDelete.Enabled=false;
				butSave.Enabled=false;
				textMain.ReadOnly=true;
				textMain.BackColor=SystemColors.Window;
				textBoxAutoNoteName.ReadOnly=true;
				textBoxAutoNoteName.BackColor=SystemColors.Window;
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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			gridMain.Columns.Add(col);
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
			using FormAutoNoteControlEdit formAutoNoteControlEdit=new FormAutoNoteControlEdit();
			formAutoNoteControlEdit.AutoNoteControlCur=_listAutoNoteControls[e.Row];
			formAutoNoteControlEdit.ShowDialog();
			if(formAutoNoteControlEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAutoNoteControlEdit formAutoNoteControlEdit=new FormAutoNoteControlEdit();
			AutoNoteControl autoNoteControl=new AutoNoteControl();
			autoNoteControl.ControlType="Text";
			formAutoNoteControlEdit.AutoNoteControlCur=autoNoteControl;
			formAutoNoteControlEdit.IsNew=true;
			formAutoNoteControlEdit.ShowDialog();
			if(formAutoNoteControlEdit.DialogResult!=DialogResult.OK) {
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
			if(_textSelectionStartNum < textMain.Text.Length-1) {
				textMain.Text=textMain.Text.Substring(0,_textSelectionStartNum)
					+"[Prompt:\""+fieldStr+"\"]"
					+textMain.Text.Substring(_textSelectionStartNum);
			}
			else{//otherwise, just tack it on the end
				textMain.Text+="[Prompt:\""+fieldStr+"\"]";
			}
			textMain.Select(_textSelectionStartNum+fieldStr.Length+11,0);
			textMain.Focus();
		}

		private void textMain_Leave(object sender,EventArgs e) {
			_textSelectionStartNum=textMain.SelectionStart;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			List<SheetFieldDef> listSheetFieldDefs=SheetFieldDefs.GetWhere(x => x.FieldType==SheetFieldType.InputField && x.FieldValue.Contains("AutoNoteNum:"+AutoNoteCur.AutoNoteNum.ToString()));
			if(listSheetFieldDefs.Count>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"There are sheet field definitions associated with this autonote. Delete this autonote and the associated fields?")) {
					return;
				}
				for(int i=0;i<listSheetFieldDefs.Count;i++) {
					SheetFieldDefs.Delete(listSheetFieldDefs[i].SheetFieldDefNum);
				}
			}
			else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this autonote?")){
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

		private void butSave_Click(object sender,EventArgs e) {
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

	}
}