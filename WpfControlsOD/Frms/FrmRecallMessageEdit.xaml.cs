using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmRecallMessageEdit:FrmODBase {
		public string MessageVal;
		private PrefName _prefName;

		public FrmRecallMessageEdit(PrefName prefName) {
			InitializeComponent();
			//Lan.F(this);
			_prefName=prefName;
		}

		private void FrmRecallMessageEdit_Loaded(object sender,RoutedEventArgs e) {
			textMain.Text=MessageVal;
		}

		private void butSave_Click(object sender,EventArgs e) {
			//We need to limit email subjects to 200 characters otherwise errors can happen in other places of the software and it's hard to track.
			//E.g. sending emails from the Recall List window and all recalls of type email will simply skip with no explanation.
			bool isEqual=false;
			isEqual |= _prefName==PrefName.BillingEmailSubject;
			isEqual |= _prefName==PrefName.ConfirmEmailSubject;
			isEqual |= _prefName==PrefName.RecallEmailSubject;
			isEqual |= _prefName==PrefName.RecallEmailSubject2;
			isEqual |= _prefName==PrefName.RecallEmailSubject3;
			isEqual |= _prefName==PrefName.ReactivationEmailSubject;
			isEqual |= _prefName==PrefName.WebSchedSubject;
			if(isEqual){
				if(textMain.Text.Length>200) {
					MsgBox.Show(this,"Email subjects cannot be longer than 200 characters.");
					return;
				}
			}
			string urlWarning="Web Sched message does not contain the \"[URL]\" variable. Omitting the \"[URL]\" variable will prevent the "+
				"patient from visiting the WebSched portal. Are you sure you want to continue?";
			isEqual=false;
			isEqual |= _prefName==PrefName.WebSchedMessage;
			isEqual |= _prefName==PrefName.WebSchedMessage2;
			isEqual |= _prefName==PrefName.WebSchedMessage3;
			if(isEqual) {
				if(!textMain.Text.Contains("[URL]")
					&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,urlWarning)) 
				{
					return;
				}
			}
			isEqual=false;
			isEqual |= _prefName==PrefName.WebSchedMessageText;
			isEqual |= _prefName==PrefName.WebSchedMessageText2;
			isEqual |= _prefName==PrefName.WebSchedMessageText3;
			if(isEqual) {
				if(textMain.Text.Contains("[URL]")) {
					textMain.Text=textMain.Text.Replace("[URL].","[URL] .");//Clicking a link with a period will not get recognized. 
				}
				else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,urlWarning)) {
					return;
				}
			}
			MessageVal=textMain.Text;
			IsDialogOK=true;
		}
		
	}
}