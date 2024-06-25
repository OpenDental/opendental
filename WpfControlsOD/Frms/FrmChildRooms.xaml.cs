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
			listBoxChildRooms.MouseDoubleClick+=listBoxChildRooms_MouseDoubleClick;
		}

		private void FrmFamilyMemberSelect_Load(object sender, System.EventArgs e) {
			FillListBox();
		}

		private void FillListBox() {
			listBoxChildRooms.Items.Clear();
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			listBoxChildRooms.Items.AddList(listChildRooms,x => x.RoomId);
		}

		private void listBoxChildRooms_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			ChildRoom childRoomSelected=listBoxChildRooms.GetSelected<ChildRoom>();
			FrmChildRoomEdit frmChildRoomEdit=new FrmChildRoomEdit();
			frmChildRoomEdit.ChildRoomCur=childRoomSelected;
			frmChildRoomEdit.ShowDialog();
			if(frmChildRoomEdit.IsDialogOK) {
				FillListBox();
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmChildRoomEdit frmChildRoomEdit=new FrmChildRoomEdit();
			ChildRoom childRoom=new ChildRoom();
			childRoom.IsNew=true;
			frmChildRoomEdit.ChildRoomCur=childRoom;
			frmChildRoomEdit.ShowDialog();
			if(frmChildRoomEdit.IsDialogOK) {
				FillListBox();
			}
		}


	}
}