using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormContacts : FormODBase {
		private List<Contact> _listContacts;
		private List<Def> _listDefsContactCategories;

		///<summary></summary>
		public FormContacts()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormContacts_Load(object sender, System.EventArgs e) {
			_listDefsContactCategories=Defs.GetDefsForCategory(DefCat.ContactCategories,true);
			for(int i=0;i<_listDefsContactCategories.Count;i++){
				listCategory.Items.Add(_listDefsContactCategories[i].ItemName);
			}
			if(listCategory.Items.Count>0) {
				listCategory.SelectedIndex=0;
			}
		}

		private void listCategory_SelectedIndexChanged(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			if(listCategory.SelectedIndex==-1) {
				return;//This will happen when resizing due to LayoutManager
			}
			_listContacts=Contacts.Refresh(_listDefsContactCategories[listCategory.SelectedIndex].DefNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g("TableContacts","Last Name"),100));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableContacts","First Name"),100));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableContacts","Wk Phone"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableContacts","Fax"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableContacts","Note"),250));
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listContacts.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listContacts[i].LName);
				row.Cells.Add(_listContacts[i].FName);
				row.Cells.Add(_listContacts[i].WkPhone);
				row.Cells.Add(_listContacts[i].Fax);
				row.Cells.Add(_listContacts[i].Notes);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormContactEdit formContactEdit=new FormContactEdit();
			formContactEdit.ContactCur=_listContacts[e.Row];
			formContactEdit.ShowDialog();
			if(formContactEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Contact contact=new Contact();
			contact.Category=_listDefsContactCategories[listCategory.SelectedIndex].DefNum;
			using FormContactEdit formContactEdit=new FormContactEdit();
			formContactEdit.ContactCur=contact;
			formContactEdit.IsNew=true;
			formContactEdit.ShowDialog();
			if(formContactEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


		


	}
}






















