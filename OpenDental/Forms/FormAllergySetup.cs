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
			if(IsSelectionMode) {
				butOK.Visible=true;
				butClose.Text="Cancel";
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
			if(!Security.IsAuthorized(Permissions.AllergyDefEdit)) {
				return;
			}
			using FormAllergyDefEdit formAllergyDefEdit=new FormAllergyDefEdit();
			formAllergyDefEdit.AllergyDefCur=new AllergyDef();
			formAllergyDefEdit.AllergyDefCur.IsNew=true;
			formAllergyDefEdit.ShowDialog();
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Only visible in IsSelectionMode.
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Select at least one allergy.");
				return;
			}
			AllergyDefNumSelected=_listAllergyDefs[gridMain.GetSelectedIndex()].AllergyDefNum;
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}