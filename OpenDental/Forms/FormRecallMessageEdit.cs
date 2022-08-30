using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRecallMessageEdit:FormODBase {
		public string MessageVal;
		private PrefName _prefName;

		public FormRecallMessageEdit(PrefName prefName) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_prefName=prefName;
		}

		private void FormRecallMessageEdit_Load(object sender,EventArgs e) {
			textMain.Text=MessageVal;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//We need to limit email subjects to 200 characters otherwise errors can happen in other places of the software and it's hard to track.
			//E.g. sending emails from the Recall List window and all recalls of type email will simply skip with no explanation.
			if(_prefName==PrefName.BillingEmailSubject
				|| _prefName==PrefName.ConfirmEmailSubject
				|| _prefName==PrefName.RecallEmailSubject
				|| _prefName==PrefName.RecallEmailSubject2
				|| _prefName==PrefName.RecallEmailSubject3
				|| _prefName==PrefName.ReactivationEmailSubject
				|| _prefName==PrefName.WebSchedSubject) 
			{
				if(textMain.Text.Length>200) {
					MsgBox.Show(this,"Email subjects cannot be longer than 200 characters.");
					return;
				}
			}
			string urlWarning="Web Sched message does not contain the \"[URL]\" variable. Omitting the \"[URL]\" variable will prevent the "+
				"patient from visiting the WebSched portal. Are you sure you want to continue?";
			if(_prefName==PrefName.WebSchedMessage
				|| _prefName==PrefName.WebSchedMessage2
				|| _prefName==PrefName.WebSchedMessage3) 
			{
				if(!textMain.Text.Contains("[URL]")
					&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,urlWarning)) 
				{
					return;
				}
			}
			if(_prefName==PrefName.WebSchedMessageText
				|| _prefName==PrefName.WebSchedMessageText2
				|| _prefName==PrefName.WebSchedMessageText3)
			{
				if(textMain.Text.Contains("[URL]")) {
					textMain.Text=textMain.Text.Replace("[URL].","[URL] .");//Clicking a link with a period will not get recognized. 
				}
				else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,urlWarning)) {
					return;
				}
			}
			MessageVal=textMain.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}