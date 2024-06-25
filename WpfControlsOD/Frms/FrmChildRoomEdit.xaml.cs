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
	public partial class FrmChildRoomEdit:FrmODBase {
		///<summary></summary>
		public ChildRoom ChildRoomCur;

		///<summary></summary>
		public FrmChildRoomEdit() {
			InitializeComponent();
			Load+=FrmChildRoomEdit_Load;
		}

		private void FrmChildRoomEdit_Load(object sender,EventArgs e) {
			textRoomId.Text=ChildRoomCur.RoomId;
			textVDoubleRatio.Value=ChildRoomCur.Ratio;
			textNotes.Text=ChildRoomCur.Notes;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textRoomId.Text=="") {
				MsgBox.Show("This child room must have an ID.");
				return;
			}
			if(!textVDoubleRatio.IsValid()) {
				return;
			}
			ChildRoomCur.RoomId=textRoomId.Text;
			ChildRoomCur.Ratio=Math.Round(textVDoubleRatio.Value,1);//Round to 1 decimal
			ChildRoomCur.Notes=textNotes.Text;
			if(ChildRoomCur.IsNew) {
				ChildRooms.Insert(ChildRoomCur);
			}
			else {
				ChildRooms.Update(ChildRoomCur);
			}
			IsDialogOK=true;
			DataValid.SetInvalid(InvalidType.Children);
		}


	}
}