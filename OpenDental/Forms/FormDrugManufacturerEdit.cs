using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDrugManufacturerEdit:FormODBase {
		public DrugManufacturer DrugManufacturerCur;
		public bool IsNew;

		public FormDrugManufacturerEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDrugManufacturerEdit_Load(object sender,EventArgs e) {
			textManufacturerName.Text=DrugManufacturerCur.ManufacturerName;
			textManufacturerCode.Text=DrugManufacturerCur.ManufacturerCode;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			try {
					DrugManufacturers.Delete(DrugManufacturerCur.DrugManufacturerNum);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textManufacturerName.Text=="" || textManufacturerCode.Text=="") {
				MsgBox.Show(this,"Bank fields are not allowed.");
				return;
			}
			DrugManufacturerCur.ManufacturerName=textManufacturerName.Text;
			DrugManufacturerCur.ManufacturerCode=textManufacturerCode.Text;
			if(IsNew) {
				if(DrugManufacturers.GetExists(x => x.ManufacturerCode==textManufacturerCode.Text)) {
					MsgBox.Show(this,"Manufacturer with this code already exists.");
					return;
				}
				DrugManufacturers.Insert(DrugManufacturerCur);
			}
			else {
				DrugManufacturers.Update(DrugManufacturerCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}