using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormAllergySetup:FormODBase {
		private List<AllergyDef> _listAllergyDefs;
		public bool IsSelectionMode;
		public long AllergyDefNumSelected;

		public FormAllergySetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAllergySetup_Load(object sender,EventArgs e) {
			if(!IsSelectionMode) {
				butOK.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid() {
			_listAllergyDefs=AllergyDefs.GetAll(checkShowHidden.Checked);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormAllergySetup","Description"),160);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormAllergySetup","Hidden"),60);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAllergyDefs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listAllergyDefs[i].Description);
				if(_listAllergyDefs[i].IsHidden) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				AllergyDefNumSelected=_listAllergyDefs[e.Row].AllergyDefNum;
				DialogResult=DialogResult.OK;
			}
			else {
				using FormAllergyDefEdit formAllergyDefEdit=new FormAllergyDefEdit();
				formAllergyDefEdit.AllergyDefCur=_listAllergyDefs[gridMain.GetSelectedIndex()];
				formAllergyDefEdit.ShowDialog();
				FillGrid();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AllergyDefEdit)) {
				return;
			}
			using FormAllergyDefEdit formAllergyDefEdit=new FormAllergyDefEdit();
			formAllergyDefEdit.AllergyDefCur=new AllergyDef();
			formAllergyDefEdit.AllergyDefCur.IsNew=true;
			formAllergyDefEdit.ShowDialog();
			FillGrid();
		}

		private void buttonMerge_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AllergyMerge)) {
				return;
			}
			if(gridMain.SelectedGridRows.Count!=2) {
				MsgBox.Show(this,"Select two allergies.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Combine the two selected allergies? This cannot be reversed.")) {
				return;
			}
			List<AllergyDef> listAllergyDefs=new List<AllergyDef>();
			//Only two will be selected
			listAllergyDefs.Add(_listAllergyDefs[gridMain.SelectedIndices[0]]);
			listAllergyDefs.Add(_listAllergyDefs[gridMain.SelectedIndices[1]]);
			try{
				AllergyDefs.Combine(allergyDefNumKeep:listAllergyDefs[0].AllergyDefNum,allergyDefNumCombine:listAllergyDefs[1].AllergyDefNum);
			}
			catch(ApplicationException ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.AllergyMerge,0,Lan.g(this,"Allergy with name")+" "+listAllergyDefs[1].Description+" " +Lan.g(this,"was merged with")+" "+listAllergyDefs[0].Description);
			FillGrid();
			//Reselect the allergy that was kept
			for(int i=0;i<_listAllergyDefs.Count;i++) {
				if(_listAllergyDefs[i].AllergyDefNum==listAllergyDefs[0].AllergyDefNum) {
					gridMain.SetSelected(i);
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Only visible in IsSelectionMode.
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Select at least one allergy.");
				return;
			}
			if(gridMain.SelectedGridRows.Count>1) {
				MsgBox.Show(this,"Only select one allergy.");
				return;
			}
			AllergyDefNumSelected=_listAllergyDefs[gridMain.GetSelectedIndex()].AllergyDefNum;
			DialogResult=DialogResult.OK;
		}

	}
}