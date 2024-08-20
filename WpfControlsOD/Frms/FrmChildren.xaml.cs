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
	public partial class FrmChildren:FrmODBase {
		///<summary>This value will be filled when the frm closes with IsDialogOK=true and IsSelectionMode==true.</summary>
		public long ChildNumSelected;
		///<summary>True if the window is in selection mode.</summary>
		public bool IsSelectionMode;

		///<summary></summary>
		public FrmChildren() {
			InitializeComponent();
			Load+=FrmChildren_Load;
			gridMain.CellDoubleClick+=gridChildren_CellDoubleClick;
		}

		private void FrmChildren_Load(object sender, EventArgs e) {
			if(IsSelectionMode) {
				butAdd.Visible=false;
			}
			else {
				butOK.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid(){
			List<Child> listChildren=Children.GetAll();
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("First Name",80);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Last Name",80);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Birthdate",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Primary Room",75);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildren.Count;i++) {
				if(checkShowHidden.Checked==false && listChildren[i].IsHidden) {
					continue;
				}
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(listChildren[i].FName);
				gridRow.Cells.Add(listChildren[i].LName);
				gridRow.Cells.Add(listChildren[i].BirthDate.ToShortDateString());
				ChildRoom childRoom=listChildRooms.Find(x => x.ChildRoomNum==listChildren[i].ChildRoomNumPrimary);
				if(childRoom!=null) {
					gridRow.Cells.Add(childRoom.RoomId);
				}
				gridRow.Tag=listChildren[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void gridChildren_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(IsSelectionMode) {
				ChildNumSelected=gridMain.SelectedTag<Child>().ChildNum;
				IsDialogOK=true;
				return;
			}
			FrmChildEdit frmChildEdit=new FrmChildEdit();
			frmChildEdit.ChildCur=gridMain.SelectedTag<Child>();
			frmChildEdit.ShowDialog();
			if(!frmChildEdit.IsDialogOK) {
				return;//No child was edited
			}
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			FrmChildEdit frmChildEdit=new FrmChildEdit();
			Child child=new Child();
			//Insert here so that a ChildNum FK exists when users create a ChildParentLink
			child.ChildNum=Children.Insert(child);
			frmChildEdit.ChildCur=child;
			frmChildEdit.ShowDialog();
			if(frmChildEdit.IsDialogOK) {
				FillGrid();
				return;
			}
			//Cleanup the child that was initially created before opening FrmChildEdit
			//Delete ChildParentLink relationships
			List<ChildParentLink> listChildParentLinks=ChildParentLinks.GetChildParentLinksByChildNum(frmChildEdit.ChildCur.ChildNum);
			for(int i=0;i<listChildParentLinks.Count;i++) {
				ChildParentLinks.Delete(listChildParentLinks[i].ChildParentLinkNum);
			}
			//Delete child
			Children.Delete(frmChildEdit.ChildCur.ChildNum);
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please pick a child first.");
				return;
			}
			ChildNumSelected=gridMain.SelectedTag<Child>().ChildNum;
			IsDialogOK=true;
		}
	}
}