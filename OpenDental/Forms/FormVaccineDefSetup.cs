using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormVaccineDefSetup:FormODBase {

		public FormVaccineDefSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormVaccineDefSetup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			VaccineDefs.RefreshCache();
			listMain.Items.Clear();
			listMain.Items.AddList(VaccineDefs.GetDeepCopy(),x => x.CVXCode+" - " +x.VaccineName);
		}

		private void listMain_DoubleClick(object sender,EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			using FormVaccineDefEdit formVaccineDefEdit=new FormVaccineDefEdit();
			formVaccineDefEdit.VaccineDefCur=listMain.GetSelected<VaccineDef>();
			formVaccineDefEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormVaccineDefEdit formVaccineDefEdit=new FormVaccineDefEdit();
			formVaccineDefEdit.VaccineDefCur=new VaccineDef();
			formVaccineDefEdit.IsNew=true;
			formVaccineDefEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}