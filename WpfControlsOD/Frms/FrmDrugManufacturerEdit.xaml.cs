using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmDrugManufacturerEdit:FrmODBase {
		public DrugManufacturer DrugManufacturerCur;
		public bool IsNew;

		public FrmDrugManufacturerEdit() {
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmDrugManufacturerEdit_Loaded(object sender,RoutedEventArgs e) {
			textManufacturerName.Text=DrugManufacturerCur.ManufacturerName;
			textManufacturerCode.Text=DrugManufacturerCur.ManufacturerCode;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				IsDialogOK=false;
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
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
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
			IsDialogOK=true;
		}
	}
}