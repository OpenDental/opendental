using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDrugManufacturerSetup:FormODBase {
		private List<DrugManufacturer> _listDrugManufacturers;

		public FormDrugManufacturerSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDrugManufacturerSetup_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			DrugManufacturers.RefreshCache();
			_listDrugManufacturers=DrugManufacturers.GetDeepCopy();
			listMain.Items.Clear();
			for(int i=0;i<_listDrugManufacturers.Count;i++) {
				listMain.Items.Add(_listDrugManufacturers[i].ManufacturerCode + " - " + _listDrugManufacturers[i].ManufacturerName);
			}
		}

		private void listMain_DoubleClick(object sender,EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			FrmDrugManufacturerEdit frmDrugManufacturerEdit=new FrmDrugManufacturerEdit();
			frmDrugManufacturerEdit.DrugManufacturerCur=_listDrugManufacturers[listMain.SelectedIndex];
			frmDrugManufacturerEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmDrugManufacturerEdit frmDrugManufacturerEdit=new FrmDrugManufacturerEdit();
			frmDrugManufacturerEdit.DrugManufacturerCur=new DrugManufacturer();
			frmDrugManufacturerEdit.IsNew=true;
			frmDrugManufacturerEdit.ShowDialog();
			FillGrid();
		}

	}
}