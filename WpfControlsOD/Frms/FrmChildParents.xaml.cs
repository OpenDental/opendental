using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildParents:FrmODBase {
		///<summary>This value will be filled when the frm closes with IsDialogOK=true and IsSelectionMode==true.</summary>
		public long ChildParentNumSelected;
		///<summary>True if the window is in selection mode.</summary>
		public bool IsSelectionMode;

		///<summary></summary>
		public FrmChildParents() {
			InitializeComponent();
			Load+=FrmChildParents_Load;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
		}

		private void FrmChildParents_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butAdd.Visible=false;
			}
			else {
				butOK.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid() {
			List<ChildParent> listChildParents=ChildParents.GetAll();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("First Name",150);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Last Name",150);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildParents.Count;i++) {
				if(checkShowHidden.Checked==false && listChildParents[i].IsHidden) {
					continue;
				}
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(listChildParents[i].FName);
				gridRow.Cells.Add(listChildParents[i].LName);
				gridRow.Tag=listChildParents[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(IsSelectionMode) {//Set select ChildParent and kick out
				ChildParentNumSelected=gridMain.SelectedTag<ChildParent>().ChildParentNum;
				IsDialogOK=true;
				return;
			}
			FrmChildParentEdit frmChildParentEdit=new FrmChildParentEdit();
			frmChildParentEdit.ChildParentCur=gridMain.SelectedTag<ChildParent>();
			frmChildParentEdit.ShowDialog();
			if(frmChildParentEdit.IsDialogOK) {
				FillGrid();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//not visible in selection mode
			FrmChildParentEdit frmChildParentEdit=new FrmChildParentEdit();
			ChildParent childParent=new ChildParent();
			childParent.IsNew=true;
			frmChildParentEdit.ChildParentCur=childParent;
			frmChildParentEdit.ShowDialog();
			if(frmChildParentEdit.IsDialogOK) {
				FillGrid();
			}
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please pick a parent first.");
				return;
			}
			ChildParentNumSelected=gridMain.SelectedTag<ChildParent>().ChildParentNum;
			IsDialogOK=true;
		}


	}
}