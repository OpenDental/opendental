using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSecurityCentralUserEdit:FormODBase {
		public FormSecurityCentralUserEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSecurityCentralUserEdit_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<Userod> listCEMTUsers=Userods.GetUsersForCEMT();
			gridCentralUsers.BeginUpdate();
			gridCentralUsers.ListGridColumns.Clear();
			gridCentralUsers.ListGridColumns.Add(new GridColumn());//Only one column, so title is used as the header.
			gridCentralUsers.ListGridRows.Clear();
			for(int i=0;i<listCEMTUsers.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listCEMTUsers[i].UserName);
				row.Tag=listCEMTUsers[i];
				gridCentralUsers.ListGridRows.Add(row);
			}
			gridCentralUsers.EndUpdate();
    }

		private void ShowCemtUserEdit() {
      if(gridCentralUsers.SelectedTag<Userod>()==null) {
				return;
      }
			using FormUserEdit formUserEdit=new FormUserEdit(gridCentralUsers.SelectedTag<Userod>(),false,true);
			if(formUserEdit.ShowDialog()==DialogResult.OK) {//Refresh the list and grid to get the most recent user updates.
				FillGrid();
			}
		}

		private void gridCentralUsers_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ShowCemtUserEdit();
		}

		private void butEdit_Click(object sender,EventArgs e) {
			ShowCemtUserEdit();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

  }
}