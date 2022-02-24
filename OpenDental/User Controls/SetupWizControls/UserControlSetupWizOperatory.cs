using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;


namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizOperatory:SetupWizControl {
		private List<Operatory> _listOps = Operatories.GetDeepCopy();
		private int _blink;
		public UserControlSetupWizOperatory() {
			InitializeComponent();
		}

		private void UserControlSetupWizOperatory_Load(object sender,EventArgs e) {
			FillGrid();
			if(Operatories.GetCount(true)==0) {
				MsgBox.Show("FormSetupWizard","You have no valid operatories. Please click the Add button to add an operatory.");
				timer1.Start();
			}
		}

		private void FillGrid() {
			Color needsAttnCol = OpenDental.SetupWizard.GetColor(ODSetupStatus.NeedsAttention);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			if(PrefC.HasClinicsEnabled) {
				col = new GridColumn(Lan.g("FormSetupWizard","OpName"),110);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","Abbrev"),110);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","Clinic"),110);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","ProvDentist"),110);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","ProvHygienist"),110);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","IsHygiene"),60,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","IsHidden"),60,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			else {
				col = new GridColumn(Lan.g("FormSetupWizard","OpName"),135);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","Abbrev"),120);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","ProvDentist"),130);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","ProvHygienist"),130);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","IsHygiene"),80,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col = new GridColumn(Lan.g("FormSetupWizard","IsHidden"),80,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			//col = new ODGridColumn("Clinic",120);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			bool IsAllComplete = true;
			if(_listOps.Count == 0) {
				IsAllComplete=false;
			}
			foreach(Operatory opCur in _listOps) {
				row = new GridRow();
				row.Cells.Add(opCur.OpName);
				if(string.IsNullOrEmpty(opCur.OpName)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(opCur.Abbrev);
				if(string.IsNullOrEmpty(opCur.Abbrev)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(opCur.ClinicNum));
				}
				//not a required field
				row.Cells.Add(Providers.GetAbbr(opCur.ProvDentist));
				//not a required field
				row.Cells.Add(Providers.GetAbbr(opCur.ProvHygienist));
				//not a required field
				row.Cells.Add(opCur.IsHygiene ? "X" : "");
				//not a required field
				row.Cells.Add(opCur.IsHidden ? "X" : "");
				//not a required field
				//row = new ODGridRow();
				//row.Cells.Add(opCur.OpName);
				//if(string.IsNullOrEmpty(opCur.OpName)) {
				//	row.Cells[row.Cells.Count-1].CellColor=needsAttnCol;
				//	IsAllComplete=false;
				//}
				row.Tag=opCur;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(IsAllComplete) {
				IsDone=true;
			}
			else {
				IsDone=false;
			}
		}

		private void timer1_Tick(object sender,EventArgs e) {
			if(_blink > 5) {
				pictureAdd.Visible=true;
				timer1.Stop();
				return;
			}
			pictureAdd.Visible=!pictureAdd.Visible;
			_blink++;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormOperatoryEdit FormOE = new FormOperatoryEdit(new Operatory());
			FormOE.IsNew=true;
			List<Operatory> listOld = new List<Operatory>();
			foreach(Operatory op in _listOps) {
				listOld.Add(op.Copy());
			}
			FormOE.ListOps=_listOps;
			FormOE.ShowDialog();
			if(FormOE.DialogResult==DialogResult.OK) {
				Operatories.Sync(_listOps,listOld);
				FillGrid();
				DataValid.SetInvalid(InvalidType.Operatories);
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Operatory opCur = (Operatory)gridMain.ListGridRows[e.Row].Tag;
			using FormOperatoryEdit FormOE = new FormOperatoryEdit(opCur);
			List<Operatory> listOld = new List<Operatory>();
			foreach(Operatory op in _listOps) {
				listOld.Add(op.Copy());
			}
			FormOE.ListOps=_listOps;
			FormOE.ShowDialog();
			if(FormOE.DialogResult==DialogResult.OK) {
				Operatories.Sync(_listOps,listOld);
				FillGrid();
				DataValid.SetInvalid(InvalidType.Operatories);
			}
		}

		private void butAdvanced_Click(object sender,EventArgs e) {
			using FormOperatories FormP=new FormOperatories();
			FormP.ShowDialog();
			FillGrid();
		}
	}
}
