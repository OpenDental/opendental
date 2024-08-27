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
			checkMixedRatio.Click+=CheckMixedRatio_Click;
		}

		private void FrmChildRoomEdit_Load(object sender,EventArgs e) {
			textRoomId.Text=ChildRoomCur.RoomId;
			if(ChildRoomCur.Ratio==-1) {//-1 indicates a mixed ratio
				checkMixedRatio.Checked=true;
				textRatio.IsEnabled=false;
			}
			else{
				textRatio.Text=ChildRoomCur.Ratio.ToString();
			}
			textNotes.Text=ChildRoomCur.Notes;
		}

		private void CheckMixedRatio_Click(object sender,EventArgs e) {
			if(checkMixedRatio.Checked==true) {
				textRatio.Text="";
				textRatio.IsEnabled=false;
				return;
			}
			textRatio.IsEnabled=true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textRoomId.Text=="") {
				MsgBox.Show("This child room must have an ID.");
				return;
			}
			int ratio=0;
			if(checkMixedRatio.Checked==true) {
				ratio=-1;
			}
			else {
				try {
					ratio=PIn.Int(textRatio.Text);
				}
				catch {
					MsgBox.Show("Ratio must be value like 4 or 10.");
					return;
				}
				if(ratio<0 || ratio>10) {
					MsgBox.Show("Ratio should be a value like 4 or 10.");
					return;
				}
			}
			ChildRoomCur.RoomId=textRoomId.Text;
			bool ratioHasChanged=(int)Math.Round(ChildRoomCur.Ratio,0)!=ratio;
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
				Signalods.SetInvalid(InvalidType.Children);
			}
			IsDialogOK=true;
		}


	}
}