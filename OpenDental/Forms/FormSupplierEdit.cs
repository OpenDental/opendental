using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSupplierEdit:FormODBase {
		public Supplier Supp;

		public FormSupplierEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplierEdit_Load(object sender,EventArgs e) {
			textName.Text=Supp.Name;
			textPhone.Text=Supp.Phone;
			textCustomerId.Text=Supp.CustomerId;
			textWebsite.Text=Supp.Website;
			textUserName.Text=Supp.UserName;
			textPassword.Text=Supp.Password;
			textNote.Text=Supp.Note;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(Supp.IsNew){
				DialogResult=DialogResult.Cancel;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try{
				Suppliers.DeleteObject(Supp);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//if(textDate.errorProvider1.GetError(textDate)!=""){
			//	MsgBox.Show(this,"Please fix data entry errors first.");
			//	return;
			//}
			if(textName.Text==""){
				MsgBox.Show(this,"Please enter a name.");
				return;
			}
			Supp.Name=textName.Text;
			Supp.Phone=textPhone.Text;
			Supp.CustomerId=textCustomerId.Text;
			Supp.Website=textWebsite.Text;
			Supp.UserName=textUserName.Text;
			Supp.Password=textPassword.Text;
			Supp.Note=textNote.Text;
			if(Supp.IsNew) {
				Suppliers.Insert(Supp);
			}
			else {
				Suppliers.Update(Supp);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}