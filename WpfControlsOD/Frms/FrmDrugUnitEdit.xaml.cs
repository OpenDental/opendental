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
	public partial class FrmDrugUnitEdit:FrmODBase {
		public DrugUnit DrugUnitCur;
		public bool IsNew;

		public FrmDrugUnitEdit() {
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmDrugUnitEdit_Loaded(object sender,RoutedEventArgs e) {
			textUnitIdentifier.Text=DrugUnitCur.UnitIdentifier;
			textUnitText.Text=DrugUnitCur.UnitText;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				IsDialogOK=false;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try {
				DrugUnits.Delete(DrugUnitCur.DrugUnitNum);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(textUnitIdentifier.Text=="" || textUnitText.Text=="") {
				MsgBox.Show(this,"Bank fields are not allowed.");
				return;
			}
			DrugUnitCur.UnitIdentifier=textUnitIdentifier.Text;
			DrugUnitCur.UnitText=textUnitText.Text;
			if(IsNew) {
				if(DrugUnits.GetExists(x => x.UnitIdentifier==textUnitIdentifier.Text)) {
					MsgBox.Show(this,"Unit with this identifier already exists.");
					return;
				}
				DrugUnits.Insert(DrugUnitCur);
			}
			else {
				DrugUnits.Update(DrugUnitCur);
			}
			IsDialogOK=true;
		}
	}
}