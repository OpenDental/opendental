using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bridges;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAdvertisingPostcardsAccountSetup:FormODBase {
		public PostcardManiaAccountData AccountDataEditing;
		public List<PostcardManiaAccountData> ListAccountData;
		private bool _isEditing;

		public FormAdvertisingPostcardsAccountSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassPostcardSetup_Load(object sender,EventArgs e) {
			_isEditing=AccountDataEditing!=null;
			butViewAccount.Visible=false;
			if(_isEditing) {
				textAccountTitle.Text=AccountDataEditing.AccountTitle;
				textEmail.Text=AccountDataEditing.Email;
				textEmail.Enabled=false;
				textFirstName.Visible=false;
				labelFName.Visible=false;
				textLastName.Visible=false;
				labelLName.Visible=false;
				textMobile.Visible=false;
				labelMobile.Visible=false;
				textPhone.Visible=false;
				labelPhone.Visible=false;
				butViewAccount.Visible=true;
				labelRequiredFields.Visible=false;
			}
		}

		private bool ValidateFields() {
			StringBuilder stringBuilder=new StringBuilder("");
			if(string.IsNullOrWhiteSpace(textAccountTitle.Text)) {
				stringBuilder.Append("- Account name is blank.\r\n");
			}
			if(!EmailAddresses.IsValidEmail(textEmail.Text,out System.Net.Mail.MailAddress mailAddress)) {
				stringBuilder.Append("- Email address is invalid\r\n");
			}
			if(!_isEditing && string.IsNullOrWhiteSpace(textFirstName.Text)) {
				stringBuilder.Append("- First name is blank\r\n");
			}
			if(!_isEditing && string.IsNullOrWhiteSpace(textLastName.Text)) {
				stringBuilder.Append("- Last name is blank\r\n");
			}
			if(!string.IsNullOrWhiteSpace(stringBuilder.ToString())) {
				MsgBox.Show($"Please fix the following errors and try again:\r\n{stringBuilder}");
				return false;
			}
			textEmail.Text=mailAddress.Address;
			if(!_isEditing && ListAccountData.Any(x => x.Email==textEmail.Text)) {
				MessageBox.Show("Cannot use the same email from another account.");
				return false;
			}		
			return true;
		}

		private bool SaveChanges() {
			string guid="";
			if(_isEditing) {
				guid=AccountDataEditing.Guid;
			}
			try {
				UI.ProgressWin progressOD=new UI.ProgressWin();
				progressOD.ActionMain=() => {
					AdvertisingPostcards.ManageAccount(textAccountTitle.Text,guid,textEmail.Text,textFirstName.Text,textLastName.Text,phone:textPhone.Text,mobile:textMobile.Text);
				};
				progressOD.StartingMessage=Lan.g(this,"Uploading Account Data")+"...";
				progressOD.ShowDialog();
				if(progressOD.IsCancelled){
					return false;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				return false;
			}
			return true;
		}

		private void butViewAccount_Click(object sender,EventArgs e) {
			string url="";
			SecurityLogs.MakeLogEntry(EnumPermType.Advertising,0,$"Navigated to Advertising - Postcards web framework for {AccountDataEditing.AccountTitle}");	
			try {
				UI.ProgressWin progressOD=new UI.ProgressWin();
				progressOD.ActionMain=() => {
					url=AdvertisingPostcards.GetSSO(AccountDataEditing.Guid,AccountDataEditing.Email);
				};
				progressOD.StartingMessage=Lan.g(this,"Getting Account Data")+"...";
				progressOD.ShowDialog();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				return;
			}
			using FormWebView fweb=new FormWebView();
			fweb.UrlBrowseTo=url;
			fweb.Title=Lan.g(this,"Advertising - Postcards");
			fweb.IsUrlSingleUse=true;
			fweb.ShowDialog();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ValidateFields()) {
				return;
			}
			if(!SaveChanges()) {
				return;
			}
			DialogResult=DialogResult.OK;
			Close();
		}

	}
}