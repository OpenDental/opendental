using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSupplyNeededEdit:FormODBase {
		public SupplyNeeded SupplyNeededCur;

		public FormSupplyNeededEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplyNeededEdit_Load(object sender,EventArgs e) {
			textDate.Text=SupplyNeededCur.DateAdded.ToShortDateString();
			textDescription.Text=SupplyNeededCur.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SupplyNeededCur.IsNew){
				DialogResult=DialogResult.Cancel;
			}
			//if(!MsgBox.){
			
			//}
			SupplyNeededs.DeleteObject(SupplyNeededCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			SupplyNeededCur.DateAdded=PIn.Date(textDate.Text);
			SupplyNeededCur.Description=textDescription.Text;
			if(SupplyNeededCur.IsNew) {
				SupplyNeededs.Insert(SupplyNeededCur);
			}
			else {
				SupplyNeededs.Update(SupplyNeededCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}