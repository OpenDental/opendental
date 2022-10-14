using System;
using System.Collections.Generic;
using System.Linq;
using OpenDentBusiness;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormGmailSettings:FormODBase {
		public EmailAddress GmailAddress;
		public bool IsNew;

		public FormGmailSettings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormGmailSettings_Load(object sender,EventArgs e) {
			if(!IsNew) {
				textParams.Text=GmailAddress.QueryString;
				checkDownloadInbox.Checked=GmailAddress.DownloadInbox;
			}
			if(!checkDownloadInbox.Checked) {
				textParams.Text="";
				textParams.Enabled=false;
				checkUnread.Checked=false;
				checkUnread.Enabled=false;
			}
		}

		private bool ContainsIsUnread(string queryString) {
			if(string.IsNullOrWhiteSpace(queryString)) {
				return false;
			}
			string queryStringToLower=queryString.ToLower();
			return queryStringToLower.Contains("is:unread");
		}

		private void checkDownloadInbox_CheckedChanged(object sender,EventArgs e) {
			if(checkDownloadInbox.Checked) {
				checkUnread.Enabled=true;
				textParams.Enabled=true;
				checkUnread.Checked=ContainsIsUnread(GmailAddress.QueryString);
				if(!IsNew) {
					textParams.Text=GmailAddress.QueryString;
				}
			}
			else {
				textParams.Text="";
				checkUnread.Checked=false;
				textParams.Enabled=false;
				checkUnread.Enabled=false;
			}
		}

		private void checkUnread_CheckedChanged(object sender,EventArgs e) {
			string strIsUnread="is:unread";
			string queryString=textParams.Text;
			if(checkUnread.Checked) {
				//Add is:unread if it is not present within the current query params.
				if(!ContainsIsUnread(queryString)) {
					queryString=queryString.Trim()+" "+strIsUnread;
				}
			}
			else {
				int index=queryString.ToLower().IndexOf(strIsUnread);
				//Remove is:unread if it is present within the current query params.
				if(index > -1) {
					queryString=textParams.Text.Remove(index,strIsUnread.Length);
				}
			}
			textParams.Text=queryString;
		}

		private void butOK_Click(object sender,EventArgs e) {
			GmailAddress.QueryString=textParams.Text.Trim();
			GmailAddress.DownloadInbox=checkDownloadInbox.Checked;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
