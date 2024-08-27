using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;
using CodeBase;

namespace OpenDental {
	public partial class FrmChildRooms:FrmODBase {

		///<summary></summary>
		public FrmChildRooms() {
			InitializeComponent();
			Load+=FrmFamilyMemberSelect_Load;
			gridMain.CellDoubleClick+=GridMain_CellDoubleClick;
		}

		private void FrmFamilyMemberSelect_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Room ID",75);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Allowed Ratio",75,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildRooms.Count;i++) {
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(listChildRooms[i].RoomId);
				if(listChildRooms[i].Ratio==-1) {
					gridRow.Cells.Add("Mixed");
				}
				else {
					gridRow.Cells.Add(listChildRooms[i].Ratio.ToString());
				}
				gridRow.Tag=listChildRooms[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void GridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			ChildRoom childRoomSelected=gridMain.SelectedTag<ChildRoom>();
			FrmChildRoomEdit frmChildRoomEdit=new FrmChildRoomEdit();
			frmChildRoomEdit.ChildRoomCur=childRoomSelected;
			frmChildRoomEdit.ShowDialog();
			if(frmChildRoomEdit.IsDialogOK) {
				FillGrid();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmChildRoomEdit frmChildRoomEdit=new FrmChildRoomEdit();
			ChildRoom childRoom=new ChildRoom();
			childRoom.IsNew=true;
			frmChildRoomEdit.ChildRoomCur=childRoom;
			frmChildRoomEdit.ShowDialog();
			if(frmChildRoomEdit.IsDialogOK) {
				FillGrid();
			}
		}


	}
}