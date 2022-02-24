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
		private EmailAddress _emailAddressCur;
		private bool _isNew;
		private int _groupAuthLocationXAuthorized=14;
		private int _groupAuthLocationXNotAuthorized=165;

		///<summary></summary>
		public FormEmailAddressEdit(EmailAddress emailAddress,bool isOpenedFromEmailSetup=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_emailAddressCur=emailAddress;
			List<long> listDefaultAddressNums=new List<long>() {
				PrefC.GetLong(PrefName.EmailNotifyAddressNum),
				PrefC.GetLong(PrefName.EmailDefaultAddressNum)
			};
			if(isOpenedFromEmailSetup && Security.IsAuthorized(Permissions.SecurityAdmin,true) 
				&& (_isNew || !ListTools.In(_emailAddressCur.EmailAddressNum,listDefaultAddressNums)))
			{
				butPickUserod.Visible=true;
			}
		}

		public FormEmailAddressEdit(long userNum,bool isOpenedFromEmailSetup=false) : this(new EmailAddress() { UserNum=userNum },isOpenedFromEmailSetup) {
			_isNew=true;
		}

		private void FormEmailAddress_Load(object sender, System.EventArgs e) {
			butAuthGoogleImageHelper(butAuthGoogle.Image);
			if(_emailAddressCur!=null) {
				textSMTPserver.Text=_emailAddressCur.SMTPserver;
				textUsername.Text=_emailAddressCur.EmailUsername;
				if(!String.IsNullOrEmpty(_emailAddressCur.EmailPassword)) { //can happen if creating a new user email.
					textPassword.Text=MiscUtils.Decrypt(_emailAddressCur.EmailPassword);
				}
				textPort.Text=_emailAddressCur.ServerPort.ToString();
				checkSSL.Checked=_emailAddressCur.UseSSL;
				textSender.Text=_emailAddressCur.SenderAddress;
				textSMTPserverIncoming.Text=_emailAddressCur.Pop3ServerIncoming;
				textPortIncoming.Text=_emailAddressCur.ServerPortIncoming.ToString();
				//Both EmailNotifyAddressNum and EmailDefaultAddressNum could be 0 (unset), in which case we still may want to display the user.
				List<long> listDefaultAddressNums=new List<long>() {
					PrefC.GetLong(PrefName.EmailNotifyAddressNum),
					PrefC.GetLong(PrefName.EmailDefaultAddressNum)
				};
				if(_isNew || !ListTools.In(_emailAddressCur.EmailAddressNum,listDefaultAddressNums)) {
					Userod user=Userods.GetUser(_emailAddressCur.UserNum);
					textUserod.Tag=user;
					textUserod.Text=user?.UserName;
				}
				else {
					groupUserod.Visible=false;
				}
				textAccessToken.Text=_emailAddressCur.AccessToken;
				textRefreshToken.Text=_emailAddressCur.RefreshToken;
				checkDownloadInbox.Checked=_emailAddressCur.DownloadInbox;
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
				Bitmap bitmapScaled=new Bitmap(image,size);
				butAuthGoogle.Image.Dispose();
				butAuthGoogle.Image=bitmapScaled;//panelEdgeExpress will dispose of this new Bitmap once the control itself is disposed of as normal
			}
			butAuthGoogle.ResumeLayout();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_isNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_emailAddressCur.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum)) {
				MsgBox.Show(this,"Cannot delete the default email address.");
				return;
			}
			if(_emailAddressCur.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum)) {
				MsgBox.Show(this,"Cannot delete the notify email address.");
				return;
			}
			Clinic clinic=Clinics.GetFirstOrDefault(x => x.EmailAddressNum==_emailAddressCur.EmailAddressNum);
			if(clinic!=null) {
				MessageBox.Show(Lan.g(this,"Cannot delete the email address because it is used by clinic")+" "+clinic.Description);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this email address?")) {
				return;
			}
			EmailAddresses.Delete(_emailAddressCur.EmailAddressNum);
			DialogResult=DialogResult.OK;//OK triggers a refresh for the grid.
		}

		private void butRegisterCertificate_Click(object sender,EventArgs e) {
			using FormEmailCertRegister form=new FormEmailCertRegister(textUsername.Text);
			form.ShowDialog();
		}

		private void butPickUserod_Click(object sender,EventArgs e) {
			using FormUserPick formUserPick=new FormUserPick();
			formUserPick.SuggestedUserNum=((Userod)textUserod.Tag)?.UserNum??0;//Preselect current selection.
			if(formUserPick.ShowDialog()==DialogResult.OK) {
				Userod user=Userods.GetUser(formUserPick.SelectedUserNum);
				if(user.UserNum==(((Userod)textUserod.Tag)?.UserNum??0)) {
					return;//No change.
				}
				EmailAddress emailUserExisting=EmailAddresses.GetForUser(user.UserNum);
				if(emailUserExisting!=null) {
					MsgBox.Show(this,"User email already exists for "+user.UserName);
					return;
				}
				textUserod.Tag=user;
				textUserod.Text=user.UserName;
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
				using InputBox inputbox=new InputBox("Please enter the authorization code from your browser");
				inputbox.setTitle("Google Account Authorization");
				inputbox.ShowDialog(this);
				if(inputbox.DialogResult!=DialogResult.OK) {
					return;
				}
				if(string.IsNullOrWhiteSpace(inputbox.textResult.Text)) {
					throw new ODException(Lan.g(this,"There was no authorization code entered."));
				}
				string authCode=inputbox.textResult.Text;
				GoogleToken tokens=Google.MakeAccessTokenRequest(authCode);
				if(tokens.ErrorMessage!="") {
					throw new Exception(tokens.ErrorMessage);
				}
				textAccessToken.Text=tokens.AccessToken;
				textRefreshToken.Text=tokens.RefreshToken;
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
			if(EmailAddresses.AddressExists(textUsername.Text,_emailAddressCur.EmailAddressNum)) {
				MsgBox.Show(this,"This email address already exists.");
				return;
			}
			_emailAddressCur.AccessToken=textAccessToken.Text;
			_emailAddressCur.RefreshToken=textRefreshToken.Text;
			_emailAddressCur.SMTPserver=PIn.String(textSMTPserver.Text);
			_emailAddressCur.EmailUsername=PIn.String(textUsername.Text);
			_emailAddressCur.EmailPassword=PIn.String(MiscUtils.Encrypt(textPassword.Text));
			_emailAddressCur.ServerPort=PIn.Int(textPort.Text);
			_emailAddressCur.UseSSL=checkSSL.Checked;
			_emailAddressCur.SenderAddress=PIn.String(textSender.Text);
			_emailAddressCur.Pop3ServerIncoming=PIn.String(textSMTPserverIncoming.Text);
			_emailAddressCur.ServerPortIncoming=PIn.Int(textPortIncoming.Text);
			_emailAddressCur.UserNum=((Userod)(textUserod.Tag))?.UserNum??0;
			_emailAddressCur.DownloadInbox=checkDownloadInbox.Checked;
			if(_isNew) {
				EmailAddresses.Insert(_emailAddressCur);
			}
			else {
				EmailAddresses.Update(_emailAddressCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
