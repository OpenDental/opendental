using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenDental{
///<summary></summary>
	public partial class FormEmailAddressEdit:FormODBase {
		private EmailAddress _emailAddress;
		private bool _isNew;
		private int _groupAuthLocationXAuthorized=14;
		private int _groupAuthLocationXNotAuthorized=165;

		///<summary></summary>
		public FormEmailAddressEdit(EmailAddress emailAddress,bool isOpenedFromEmailSetup=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_emailAddress=emailAddress;
			List<long> listDefaultAddressNums=new List<long>() {
				PrefC.GetLong(PrefName.EmailNotifyAddressNum),
				PrefC.GetLong(PrefName.EmailDefaultAddressNum)
			};
			if(isOpenedFromEmailSetup && Security.IsAuthorized(Permissions.SecurityAdmin,true) 
				&& (_isNew || !ListTools.In(_emailAddress.EmailAddressNum,listDefaultAddressNums)))
			{
				butPickUserod.Visible=true;
			}
		}

		public FormEmailAddressEdit(long userNum,bool isOpenedFromEmailSetup=false) : this(new EmailAddress() { UserNum=userNum },isOpenedFromEmailSetup) {
			_isNew=true;
		}

		private void FormEmailAddress_Load(object sender, System.EventArgs e) {
			butAuthGoogleImageHelper(butAuthGoogle.Image);
			if(_emailAddress!=null) {
				textSMTPserver.Text=_emailAddress.SMTPserver;
				textUsername.Text=_emailAddress.EmailUsername;
				if(!String.IsNullOrEmpty(_emailAddress.EmailPassword)) { //can happen if creating a new user email.
					textPassword.Text=MiscUtils.Decrypt(_emailAddress.EmailPassword);
				}
				textPort.Text=_emailAddress.ServerPort.ToString();
				checkSSL.Checked=_emailAddress.UseSSL;
				textSender.Text=_emailAddress.SenderAddress;
				textSMTPserverIncoming.Text=_emailAddress.Pop3ServerIncoming;
				textPortIncoming.Text=_emailAddress.ServerPortIncoming.ToString();
				//Both EmailNotifyAddressNum and EmailDefaultAddressNum could be 0 (unset), in which case we still may want to display the user.
				List<long> listDefaultAddressNums=new List<long>() {
					PrefC.GetLong(PrefName.EmailNotifyAddressNum),
					PrefC.GetLong(PrefName.EmailDefaultAddressNum)
				};
				if(_isNew || !ListTools.In(_emailAddress.EmailAddressNum,listDefaultAddressNums)) {
					Userod userod=Userods.GetUser(_emailAddress.UserNum);
					textUserod.Tag=userod;
					textUserod.Text=userod?.UserName;
				}
				else {
					groupUserod.Visible=false;
				}
				textAccessToken.Text=_emailAddress.AccessToken;
				textRefreshToken.Text=_emailAddress.RefreshToken;
				checkDownloadInbox.Checked=_emailAddress.DownloadInbox;
				if(!textAccessToken.Text.IsNullOrEmpty()) {//If using Google OAuth, disable unnecessary fields
					textPassword.Enabled=false;
					textSMTPserver.ReadOnly=true;
					textPort.Enabled=false;
					textSMTPserverIncoming.ReadOnly=true;
					textPortIncoming.Enabled=false;
				}
			}
			groupGoogleAuth.Visible=!textAccessToken.Text.IsNullOrEmpty();
			if(groupGoogleAuth.Visible) {
				checkDownloadInbox.Checked=!textSMTPserverIncoming.Text.IsNullOrEmpty();
				AdjustEmailTextFields();
				LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXAuthorized,groupAuthentication.Location.Y));
			}
			else {
				LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXNotAuthorized,groupAuthentication.Location.Y));
			}
		}

		private void AdjustEmailTextFields(bool isEnablingEmailFields=false) {
			textSMTPserver.Text="smtp.gmail.com";
			textPort.Text="0";
			if(checkDownloadInbox.Checked) {
				textPassword.Text="";
				textSMTPserverIncoming.Text="pop.gmail.com";
				textPortIncoming.Text="0";
			}
			else {
				textSMTPserverIncoming.Text="";
				textPortIncoming.Text="";
			}
			//Google OAuth uses the API instead of ports so we can disable and autofill fields
			textPassword.Enabled=isEnablingEmailFields;
			textSMTPserver.ReadOnly=!isEnablingEmailFields;
			textPort.Enabled=isEnablingEmailFields;
			textSMTPserverIncoming.ReadOnly=!isEnablingEmailFields;
			textPortIncoming.Enabled=isEnablingEmailFields;
		}

		private void butAuthGoogleImageHelper(Image image) {
			butAuthGoogle.SuspendLayout();
			if(LayoutManager.ScaleMy()!=1) {
				Size size=new Size(LayoutManager.Scale(image.Width),LayoutManager.Scale(image.Height));
				Bitmap bitmap=new Bitmap(image,size);
				butAuthGoogle.Image?.Dispose();
				butAuthGoogle.Image=bitmap;//panelEdgeExpress will dispose of this new Bitmap once the control itself is disposed of as normal
			}
			butAuthGoogle.ResumeLayout();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_isNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_emailAddress.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum)) {
				MsgBox.Show(this,"Cannot delete the default email address.");
				return;
			}
			if(_emailAddress.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum)) {
				MsgBox.Show(this,"Cannot delete the notify email address.");
				return;
			}
			Clinic clinic=Clinics.GetFirstOrDefault(x => x.EmailAddressNum==_emailAddress.EmailAddressNum);
			if(clinic!=null) {
				MessageBox.Show(Lan.g(this,"Cannot delete the email address because it is used by clinic")+" "+clinic.Description);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this email address?")) {
				return;
			}
			EmailAddresses.Delete(_emailAddress.EmailAddressNum);
			DialogResult=DialogResult.OK;//OK triggers a refresh for the grid.
		}

		private void butRegisterCertificate_Click(object sender,EventArgs e) {
			using FormEmailCertRegister formEmailCertRegister=new FormEmailCertRegister(textUsername.Text);
			formEmailCertRegister.ShowDialog();
		}

		private void butPickUserod_Click(object sender,EventArgs e) {
			using FormUserPick formUserPick=new FormUserPick();
			formUserPick.SuggestedUserNum=((Userod)textUserod.Tag)?.UserNum??0;//Preselect current selection.
			if(formUserPick.ShowDialog()==DialogResult.OK) {
				Userod userod=Userods.GetUser(formUserPick.SelectedUserNum);
				if(userod.UserNum==(((Userod)textUserod.Tag)?.UserNum??0)) {
					return;//No change.
				}
				EmailAddress emailAddress=EmailAddresses.GetForUser(userod.UserNum);
				if(emailAddress!=null) {
					MsgBox.Show(this,"User email already exists for "+userod.UserName);
					return;
				}
				textUserod.Tag=userod;
				textUserod.Text=userod.UserName;
			}
		}

		private void butClearTokens_Click(object sender,EventArgs e) {
			textAccessToken.Text="";
			textRefreshToken.Text="";
			butClearTokens.Enabled=false;
			checkDownloadInbox.Checked=false;
			checkDownloadInbox.Enabled=false;
			AdjustEmailTextFields(true);
		}
		
		private void checkDownloadInbox_Click(object sender,EventArgs e) {
			AdjustEmailTextFields();
		}

		private void butAuthGoogle_Click(object sender,EventArgs e) {
			try {
				string url=Google.GetGoogleAuthorizationUrl(textUsername.Text);
				Process.Start(url);
				using InputBox inputBox=new InputBox("Please enter the authorization code from your browser");
				inputBox.setTitle("Google Account Authorization");
				inputBox.ShowDialog(this);
				if(inputBox.DialogResult!=DialogResult.OK) {
					return;
				}
				if(string.IsNullOrWhiteSpace(inputBox.textResult.Text)) {
					throw new ODException(Lan.g(this,"There was no authorization code entered."));
				}
				string authCode=inputBox.textResult.Text;
				GoogleToken googleToken=Google.MakeAccessTokenRequest(authCode);
				if(googleToken.ErrorMessage!="") {
					throw new Exception(googleToken.ErrorMessage);
				}
				textAccessToken.Text=googleToken.AccessToken;
				textRefreshToken.Text=googleToken.RefreshToken;
				AdjustEmailTextFields();
				LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXAuthorized,groupAuthentication.Location.Y));
				groupGoogleAuth.Visible=true;
				butClearTokens.Enabled=true;
				checkDownloadInbox.Enabled=true;
				checkDownloadInbox.Checked=false;
			}
			catch(ODException ae) {
				MsgBox.Show(ae.Message);
			}
			catch(Exception ex) {
				MsgBox.Show("Error: "+ex.Message);
			}
		}

		private void butAuthGoogle_MouseEnter(object sender,EventArgs e) {
			butAuthGoogle.Image=Properties.Resources.google_signin_focus;
			butAuthGoogleImageHelper(butAuthGoogle.Image);
		}

		private void butAuthGoogle_MouseLeave(object sender,EventArgs e) {
			butAuthGoogle.Image=Properties.Resources.google_signin_normal;
			butAuthGoogleImageHelper(butAuthGoogle.Image);
		}

		private void butAuthGoogle_MouseDown(object sender,MouseEventArgs e) {
			butAuthGoogle.Image=Properties.Resources.google_signin_pressed;
			butAuthGoogleImageHelper(butAuthGoogle.Image);
		}

		private void butAuthGoogle_MouseUp(object sender,MouseEventArgs e) {
			butAuthGoogle.Image=Properties.Resources.google_signin_normal;
			butAuthGoogleImageHelper(butAuthGoogle.Image);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			try {
				PIn.Int(textPort.Text);
			}
			catch {
				MsgBox.Show(this,"Invalid outgoing port number.");
				return;
			}
			try {
				PIn.Int(textPortIncoming.Text);
			}
			catch {
				MsgBox.Show(this,"Invalid incoming port number.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textUsername.Text)) {
				MsgBox.Show(this,"Please enter a valid email address.");
				return;
			}
			//Only checks against non-user email addresses.
			if(EmailAddresses.AddressExists(textUsername.Text,_emailAddress.EmailAddressNum)) {
				MsgBox.Show(this,"This email address already exists.");
				return;
			}
			_emailAddress.AccessToken=textAccessToken.Text;
			_emailAddress.RefreshToken=textRefreshToken.Text;
			_emailAddress.SMTPserver=PIn.String(textSMTPserver.Text);
			_emailAddress.EmailUsername=PIn.String(textUsername.Text);
			_emailAddress.EmailPassword=PIn.String(MiscUtils.Encrypt(textPassword.Text));
			_emailAddress.ServerPort=PIn.Int(textPort.Text);
			_emailAddress.UseSSL=checkSSL.Checked;
			_emailAddress.SenderAddress=PIn.String(textSender.Text);
			_emailAddress.Pop3ServerIncoming=PIn.String(textSMTPserverIncoming.Text);
			_emailAddress.ServerPortIncoming=PIn.Int(textPortIncoming.Text);
			_emailAddress.UserNum=((Userod)(textUserod.Tag))?.UserNum??0;
			_emailAddress.DownloadInbox=checkDownloadInbox.Checked;
			if(_isNew) {
				EmailAddresses.Insert(_emailAddress);
			}
			else {
				EmailAddresses.Update(_emailAddress);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
