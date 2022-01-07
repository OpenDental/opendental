using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDrugUnitSetup:FormODBase {
		private List<DrugUnit> _listDrugUnits;

		public FormDrugUnitSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDrugUnitSetup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			DrugUnits.RefreshCache();
			_listDrugUnits=DrugUnits.GetDeepCopy();
			listMain.Items.Clear();
			for(int i=0;i<_listDrugUnits.Count;i++) {
				listMain.Items.Add(_listDrugUnits[i].UnitIdentifier + " - " + _listDrugUnits[i].UnitText);
			}
		}

		private void listMain_DoubleClick(object sender,EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			using FormDrugUnitEdit FormD=new FormDrugUnitEdit();
			FormD.DrugUnitCur=_listDrugUnits[listMain.SelectedIndex];
			FormD.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormDrugUnitEdit FormD=new FormDrugUnitEdit();
			FormD.DrugUnitCur=new DrugUnit();
			FormD.IsNew=true;
			FormD.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}