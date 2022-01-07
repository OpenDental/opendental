using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormIcd9s:FormODBase {
		public bool IsSelectionMode;
		public ICD9 SelectedIcd9;
		private List<ICD9> icd9List;

		public FormIcd9s() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormIcd9s_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butClose.Text=Lan.g(this,"Cancel");
			}
			else {
				butOK.Visible=false;
			}
		}
		
		private void butSearch_Click(object sender,EventArgs e) {
			//if(textCode.Text.Length<3) {
			//	MsgBox.Show(this,"Please enter at least 3 characters before searching.");
			//	return;
			//}
			//forget about the above.  Allow general browsing by entering no search parameters.
			FillGrid();
		}

		private void FillGrid() {
			Cursor=Cursors.WaitCursor;
			icd9List=ICD9s.GetByCodeOrDescription(textCode.Text);
			icd9List.RemoveAll(x => string.IsNullOrEmpty(x.ICD9Code));//We have seen some old ICD9 codes with invalid codes.
			listMain.Items.Clear();
			for(int i=0;i<icd9List.Count;i++) {
				listMain.Items.Add(icd9List[i].ICD9Code+" - "+icd9List[i].Description);
			}
			Cursor=Cursors.Default;
		}

		private void listMain_DoubleClick(object sender,System.EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			if(IsSelectionMode) {
				SelectedIcd9=icd9List[listMain.SelectedIndex];
				DialogResult=DialogResult.OK;
				return;
			}
			/* Commented to prevent Icd9 edit window from being shown
			changed=true;
			using FormIcd9Edit FormI=new FormIcd9Edit(icd9List[listMain.SelectedIndex]);
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK) {
				return;
			}
			FillGrid();
			*/
		}

		/* Deprecated. This is populated from a list and follows a standard so we do not allow custom ICD-9 Codes.
		private void butAdd_Click(object sender,EventArgs e) {
			changed=true;
			ICD9 icd9=new ICD9();
			using FormIcd9Edit FormI=new FormIcd9Edit(icd9);
			FormI.IsNew=true;
			FormI.ShowDialog();
			FillGrid();
		}
		*/

		private void butCodeImport_Click(object sender,EventArgs e) {
			using FormCodeSystemsImport FormCSI=new FormCodeSystemsImport();
			FormCSI.ShowDialog();
		}
		
		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless IsSelectionMode
			if(listMain.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SelectedIcd9=icd9List[listMain.SelectedIndex];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

	}
}