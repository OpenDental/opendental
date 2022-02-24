using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormContactEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public Contact ContactCur;
		private List<Def> _listContactCategoryDefs;

		///<summary></summary>
		public FormContactEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormContactEdit_Load(object sender, System.EventArgs e) {
			_listContactCategoryDefs=Defs.GetDefsForCategory(DefCat.ContactCategories,true);
			for(int i=0;i<_listContactCategoryDefs.Count;i++){
				listCategory.Items.Add(_listContactCategoryDefs[i].ItemName);
				if(ContactCur.Category==_listContactCategoryDefs[i].DefNum){
					listCategory.SelectedIndex=i;
				}
			}
			textLName.Text=ContactCur.LName;
			textFName.Text=ContactCur.FName;
			textWkPhone.Text=ContactCur.WkPhone;
			textFax.Text=ContactCur.Fax;
			textNotes.Text=ContactCur.Notes;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lan.g(this,"Delete contact"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			else{
				Contacts.Delete(ContactCur);
				DialogResult=DialogResult.OK;
			}
		}

		private void textLName_TextChanged(object sender, System.EventArgs e) {
			if(textLName.Text.Length==1){
				textLName.Text=textLName.Text.ToUpper();
				textLName.SelectionStart=1;
			}
		}

		private void textFName_TextChanged(object sender, System.EventArgs e) {
			if(textFName.Text.Length==1){
				textFName.Text=textFName.Text.ToUpper();
				textFName.SelectionStart=1;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textLName.Text==""){
				MessageBox.Show(Lan.g(this,"Last Name cannot be blank."));
				return;
			}
			//a category will always be selected because of the manner in which Contact is accessed
			ContactCur.Category=_listContactCategoryDefs[listCategory.SelectedIndex].DefNum;
			ContactCur.LName=textLName.Text;
			ContactCur.FName=textFName.Text;
			ContactCur.WkPhone=textWkPhone.Text;
			ContactCur.Fax=textFax.Text;
			ContactCur.Notes=textNotes.Text;
			if(IsNew){
				Contacts.Insert(ContactCur);
			}
			else{
				Contacts.Update(ContactCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		

		


	}
}





















