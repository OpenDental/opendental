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
		private Contact[] ContactList;
		private List<Def> _listContactCategoryDefs;

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
			_listContactCategoryDefs=Defs.GetDefsForCategory(DefCat.ContactCategories,true);
			for(int i=0;i<_listContactCategoryDefs.Count;i++){
				listCategory.Items.Add(_listContactCategoryDefs[i].ItemName);
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
			ContactList=Contacts.Refresh(_listContactCategoryDefs[listCategory.SelectedIndex].DefNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableContacts","Last Name"),100));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableContacts","First Name"),100));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableContacts","Wk Phone"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableContacts","Fax"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableContacts","Note"),250));
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(Contact contactCur in ContactList) {
				row=new GridRow();
				row.Cells.Add(contactCur.LName);
				row.Cells.Add(contactCur.FName);
				row.Cells.Add(contactCur.WkPhone);
				row.Cells.Add(contactCur.Fax);
				row.Cells.Add(contactCur.Notes);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormContactEdit FormCE=new FormContactEdit();
			FormCE.ContactCur=ContactList[e.Row];
			FormCE.ShowDialog();
			if(FormCE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Contact ContactCur=new Contact();
			ContactCur.Category=_listContactCategoryDefs[listCategory.SelectedIndex].DefNum;
			using FormContactEdit FormCE=new FormContactEdit();
			FormCE.ContactCur=ContactCur;
			FormCE.IsNew=true;
			FormCE.ShowDialog();
			if(FormCE.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


		


	}
}






















