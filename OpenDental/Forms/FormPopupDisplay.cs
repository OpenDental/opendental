using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>This control should not be resized, except by the user. Dentists can enter sensitive information in the area below what is normally shown.</summary>
	public partial class FormPopupDisplay:FormODBase {
		public Popup PopupCur;

		///<summary>Will be zero unless user successfully clicked a disable time interval.  Accepted range is 1 to 1440 (24hrs)</summary>
		public int MinutesDisabled;

		///<summary></summary>
		public FormPopupDisplay()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPopupDisplay_Load(object sender,EventArgs e) {
			//This homogenizes the display because sometimes popups are stored with "\n" and sometimes they are saved with "\r\n"
			textDescription.Text=PopupCur.Description.Replace("\r\n","\n").Replace("\n","\r\n");
			if(PopupCur.UserNum!=0) {
				//Display last user to edit PopupCur, or "Unknown(5)" if user not found.
				textUser.Text=Userods.GetUser(PopupCur.UserNum)?.UserName??(Lan.g(this,"Unknown")+$"({POut.Long(PopupCur.UserNum)})");
			}
			textCreateDate.Text="";
			if(PopupCur.DateTimeEntry.Year>1880) {
				textCreateDate.Text=PopupCur.DateTimeEntry.ToShortDateString()+" "+PopupCur.DateTimeEntry.ToShortTimeString();
			}
			DateTime dateT=Popups.GetLastEditDateTimeForPopup(PopupCur.PopupNum);
			textEditDate.Text="";
			if(dateT.Year>1880) {
				textEditDate.Text=dateT.ToShortDateString()+" "+dateT.ToShortTimeString();//Sets the Edit date to the entry date of the last popup change that was archived for this popup.
			}
			for(int i=1;i<=4;i++) {
				comboMinutes.Items.Add(i.ToString());
			}
			for(int i=1;i<=11;i++) {
				comboMinutes.Items.Add((i*5).ToString());
			}
			comboMinutes.Text="10";
			for(int i=1;i<=12;i++) {
				comboHours.Items.Add(i.ToString());
			}
			comboHours.Text="1";
			MinutesDisabled=0;
			if(PopupCur.UserNum != Security.CurUser.UserNum 
				&& !Security.IsAuthorized(Permissions.PopupEdit,true)) 
			{
				textDescription.ReadOnly=true;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(PopupCur.Description.Replace("\r","")!=textDescription.Text.Replace("\r","")) {//if user changed the note. remove "\r" to homogenize line returns because "\r\n" is the same as "\n"
				if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Save changes to note?")) {
					Popup popupArchive=PopupCur.Copy();
					popupArchive.IsArchived=true;
					popupArchive.PopupNumArchive=PopupCur.PopupNum;
					Popups.Insert(popupArchive);
					PopupCur.Description=textDescription.Text;
					PopupCur.DateTimeEntry=DateTime.Now;
					PopupCur.UserNum=Security.CurUser.UserNum;
					Popups.Update(PopupCur);
				}
			}
			else {
				MinutesDisabled=10;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















