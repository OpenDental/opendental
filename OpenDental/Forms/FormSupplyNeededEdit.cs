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
		public SupplyNeeded Supp;

		public FormSupplyNeededEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplyNeededEdit_Load(object sender,EventArgs e) {
			textDate.Text=Supp.DateAdded.ToShortDateString();
			textDescription.Text=Supp.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(Supp.IsNew){
				DialogResult=DialogResult.Cancel;
			}
			//if(!MsgBox.){
			
			//}
			SupplyNeededs.DeleteObject(Supp);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			Supp.DateAdded=PIn.Date(textDate.Text);
			Supp.Description=textDescription.Text;
			if(Supp.IsNew) {
				SupplyNeededs.Insert(Supp);
			}
			else {
				SupplyNeededs.Update(Supp);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}