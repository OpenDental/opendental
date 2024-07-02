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
			double ratio=Math.Round(textVDoubleRatio.Value,1);//Round to 1 decimal
			bool ratioHasChanged=ChildRoomCur.Ratio!=ratio;
			ChildRoomCur.Ratio=ratio;
			ChildRoomCur.Notes=textNotes.Text;
			if(ChildRoomCur.IsNew) {
				ChildRoomCur.ChildRoomNum=ChildRooms.Insert(ChildRoomCur);//Hold on to PK for log entry
			}
			else {
				ChildRooms.Update(ChildRoomCur);
			}
			//If the ratio has changed make a ChildRoomLog entry
			if(ratioHasChanged) {
				ChildRoomLog childRoomLog=new ChildRoomLog();
				childRoomLog.RatioChange=ratio;
				childRoomLog.ChildRoomNum=ChildRoomCur.ChildRoomNum;
				childRoomLog.DateTEntered=DateTime.Now;
				childRoomLog.DateTDisplayed=DateTime.Now;
				//ChildNum and ChildTeacherNum remain 0 as this is specifically a RatioChange entry
				ChildRoomLogs.Insert(childRoomLog);
			}
			IsDialogOK=true;
		}


	}
}