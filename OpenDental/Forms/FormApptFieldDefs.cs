using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// </summary>
	public partial class FormApptFieldDefs:FormODBase {
		private List<ApptFieldDef> _listApptFieldDefs;

		///<summary></summary>
		public FormApptFieldDefs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptFieldDefs_Load(object sender, System.EventArgs e) {
			LayoutMenu();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void FillGrid(){
			ApptFieldDefs.RefreshCache();
			_listApptFieldDefs=ApptFieldDefs.GetDeepCopy();
			listMain.Items.Clear();
			for(int i=0;i<_listApptFieldDefs.Count;i++) {
				listMain.Items.Add(_listApptFieldDefs[i].FieldName);
			}
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormFieldDefLink formFieldDefLink=new FormFieldDefLink(FieldLocations.AppointmentEdit);
			formFieldDefLink.ShowDialog();
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listMain.SelectedIndex==-1){
				return;
			}
			using FormApptFieldDefEdit formApptFieldDefEdit=new FormApptFieldDefEdit(_listApptFieldDefs[listMain.SelectedIndex]);
			formApptFieldDefEdit.ShowDialog();
			if(formApptFieldDefEdit.DialogResult!=DialogResult.OK)
				return;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			ApptFieldDef apptFieldDef=new ApptFieldDef();
			using FormApptFieldDefEdit formApptFieldDefEdit=new FormApptFieldDefEdit(apptFieldDef);
			formApptFieldDefEdit.IsNew=true;
			formApptFieldDefEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormApptFieldDefs_FormClosing_1(object sender,FormClosingEventArgs e) {
			DataValid.SetInvalid(InvalidType.PatFields);
		}
	}
}



























