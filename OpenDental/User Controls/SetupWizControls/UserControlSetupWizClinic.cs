using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using OpenDental.UI;
using OpenDental;
using OpenDentBusiness;


namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizClinic:SetupWizControl {
		private int _blink;

		public UserControlSetupWizClinic() {
			InitializeComponent();
		}

		private void UserControlSetupWizClinic_Load(object sender,EventArgs e) {
			FillGrid();
			if(Clinics.GetCount(true)==0) {
				MsgBox.Show("FormSetupWizard","You have no valid clinics. Please click the Add button to add a clinic.");
				timer1.Start();
			}
		}

		private void FillGrid() {
			Color needsAttnCol = OpenDental.SetupWizard.GetColor(ODSetupStatus.NeedsAttention);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col = new GridColumn("Clinic",110);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("Abbrev",70);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("Phone",100);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("Address",120);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("City",90);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("State",50);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("ZIP",80);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("Default Prov",75);
			gridMain.ListGridColumns.Add(col);
			col = new GridColumn("IsHidden",55,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			bool IsAllComplete = true;
			List<Clinic> listClins = Clinics.GetDeepCopy();
			if(listClins.Count == 0) {
				IsAllComplete=false;
			}
			foreach(Clinic clinCur in listClins) {
				row = new GridRow();
				row.Cells.Add(clinCur.Description);
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.Description)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(clinCur.Abbr);
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.Abbr)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(TelephoneNumbers.FormatNumbersExactTen(clinCur.Phone));
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.Phone)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(clinCur.Address);
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.Address)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(clinCur.City);
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.City)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(clinCur.State);
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.State)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(clinCur.Zip);
				if(!clinCur.IsHidden && string.IsNullOrEmpty(clinCur.Zip)) {
					row.Cells[row.Cells.Count-1].ColorBackG=needsAttnCol;
					IsAllComplete=false;
				}
				row.Cells.Add(Providers.GetAbbr(clinCur.DefaultProv));
				row.Cells.Add(clinCur.IsHidden?"X":"");
				row.Tag=clinCur;
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
			using FormClinicEdit FormCE = new FormClinicEdit(new Clinic() { IsNew=true });
			FormCE.ShowDialog();
			if(FormCE.DialogResult==DialogResult.OK) {
				Clinics.Insert(FormCE.ClinicCur);
				DataValid.SetInvalid(InvalidType.Providers);
				FillGrid();
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Clinic clinCur = (Clinic)gridMain.ListGridRows[e.Row].Tag;
			Clinic clinOld = clinCur.Copy();
			using FormClinicEdit FormCE = new FormClinicEdit(clinCur);
			FormCE.ShowDialog();
			if(FormCE.DialogResult==DialogResult.OK) {
				Clinics.Update(FormCE.ClinicCur,clinOld);
				DataValid.SetInvalid(InvalidType.Providers);
				FillGrid();
			}
		}

		private void butAdvanced_Click(object sender,EventArgs e) {
			using FormClinics formClinics=new FormClinics();
			formClinics.ShowDialog();
			FillGrid();
		}
	}
}
