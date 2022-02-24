using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDrugUnitEdit:FormODBase {
		public DrugUnit DrugUnitCur;
		public bool IsNew;

		public FormDrugUnitEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDrugUnitEdit_Load(object sender,EventArgs e) {
			textUnitIdentifier.Text=DrugUnitCur.UnitIdentifier;
			textUnitText.Text=DrugUnitCur.UnitText;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
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
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
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
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}