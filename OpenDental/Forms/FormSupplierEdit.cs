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
		public Supplier SupplierCur;

		public FormSupplierEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplierEdit_Load(object sender,EventArgs e) {
			textName.Text=SupplierCur.Name;
			textPhone.Text=SupplierCur.Phone;
			textCustomerId.Text=SupplierCur.CustomerId;
			textWebsite.Text=SupplierCur.Website;
			textUserName.Text=SupplierCur.UserName;
			textPassword.Text=SupplierCur.Password;
			textNote.Text=SupplierCur.Note;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SupplierCur.IsNew){
				DialogResult=DialogResult.Cancel;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try{
				Suppliers.DeleteObject(SupplierCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			//if(textDate.errorProvider1.GetError(textDate)!=""){
			//	MsgBox.Show(this,"Please fix data entry errors first.");
			//	return;
			//}
			if(textName.Text==""){
				MsgBox.Show(this,"Please enter a name.");
				return;
			}
			SupplierCur.Name=textName.Text;
			SupplierCur.Phone=textPhone.Text;
			SupplierCur.CustomerId=textCustomerId.Text;
			SupplierCur.Website=textWebsite.Text;
			SupplierCur.UserName=textUserName.Text;
			SupplierCur.Password=textPassword.Text;
			SupplierCur.Note=textNote.Text;
			if(SupplierCur.IsNew) {
				Suppliers.Insert(SupplierCur);
			}
			else {
				Suppliers.Update(SupplierCur);
			}
			DialogResult=DialogResult.OK;
		}

	}
}