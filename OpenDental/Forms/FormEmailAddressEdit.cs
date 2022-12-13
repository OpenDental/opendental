using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;
using System.Diagnostics;
using OpenDental.UI;
using Bridges;

namespace OpenDental{
///<summary></summary>
	public partial class FormEmailAddressEdit:FormODBase {
		private EmailAddress _emailAddress;
		private bool _isNew;
		private int _groupAuthLocationXAuthorized=14;
		private int _groupAuthLocationXNotAuthorized=165;
		private OAuthType _authenticationType=OAuthType.None;

		///<summary></summary>
		public FormEmailAddressEdit(EmailAddress emailAddress,bool isOpenedFromEmailSetup=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_emailAddress=emailAddress;
			_authenticationType=emailAddress.AuthenticationType;
			List<long> listDefaultAddressNums=new List<long>();
			listDefaultAddressNums.Add(PrefC.GetLong(PrefName.EmailNotifyAddressNum));
			listDefaultAddressNums.Add(PrefC.GetLong(PrefName.EmailDefaultAddressNum));
			if(isOpenedFromEmailSetup && Security.IsAuthorized(Permissions.SecurityAdmin,suppressMessage:true) 
				&& (_isNew || !listDefaultAddressNums.Contains(_emailAddress.EmailAddressNum)))
			{
				butPickUserod.Visible=true;
			}
		}

		public FormEmailAddressEdit(long userNum,bool isOpenedFromEmailSetup=false) : this(new EmailAddress() { UserNum=userNum },isOpenedFromEmailSetup) {
			_isNew=true;
		}

		private void FormEmailAddress_Load(object sender, System.EventArgs e) {
			butAuthGoogleImageHelper(butAuthGoogle.Image);
			butAuthMicrosoftImageHelper(butAuthMicrosoft.Image);
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
				List<long> listDefaultAddressNums=new List<long>();
				listDefaultAddressNums.Add(PrefC.GetLong(PrefName.EmailNotifyAddressNum));
				listDefaultAddressNums.Add(PrefC.GetLong(PrefName.EmailDefaultAddressNum));
				if(_isNew || !listDefaultAddressNums.Contains(_emailAddress.EmailAddressNum)) {
					Userod userod=Userods.GetUser(_emailAddress.UserNum);
					textUserod.Tag=userod;
					textUserod.Text=userod?.UserName;
				}
				else {
					groupUserod.Visible=false;
				}
				textAccessToken.Text=_emailAddress.AccessToken;
				textRefreshToken.Text=_emailAddress.RefreshToken;
			}
			groupAuth.Visible=!textAccessToken.Text.IsNullOrEmpty();
			if(groupAuth.Visible) {
				AdjustEmailTextFields();
				LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXAuthorized,groupAuthentication.Location.Y));
			}
			else {
				LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXNotAuthorized,groupAuthentication.Location.Y));
			}
		}

		private void AdjustEmailTextFields(bool isEnablingEmailFields=false) {
			//OAuth uses the API instead of ports so we can disable and autofill fields
			textPassword.Enabled=isEnablingEmailFields;
			textSMTPserver.ReadOnly=!isEnablingEmailFields;
			textPort.Enabled=isEnablingEmailFields;
			textSMTPserverIncoming.ReadOnly=!isEnablingEmailFields;
			textPortIncoming.Enabled=isEnablingEmailFields;
			if(_authenticationType==OAuthType.Google) {
				textSMTPserver.Text="smtp.gmail.com";
				textPort.Text="0";
				if(_emailAddress.DownloadInbox) {
					textPassword.Text="";
					textSMTPserverIncoming.Text="pop.gmail.com";
					textPortIncoming.Text="0";
				}
				else {
					textSMTPserverIncoming.Text="";
					textPortIncoming.Text="";
				}
				groupAuth.Visible=true;
				groupAuth.Text="Google Authorization";
				butClearTokens.Enabled=true;
				butClearTokens.Text="Clear Tokens";
				butGmailSettings.Visible=true;
				textRefreshToken.Visible=true;
				labelRefresh.Visible=true;
			}
			else if(_authenticationType==OAuthType.Microsoft) {
				//Microsoft OAuth doesnt need to use these fields since it uses it's own Api and redirect URI to access Microsoft Graph.
				textPort.Text="0";
				textPassword.Text="";
				//These values are populated to allow sending/receiving emails. Microsoft Graph doesn't need these values. These are random and not in Microsoft Documentation.
				textSMTPserver.Text="smtp.outlook.com";
				textSMTPserverIncoming.Text="pop.outlook.com";
				textPortIncoming.Text="";
				groupAuth.Visible=true;
				groupAuth.Text="Microsoft Authorization";
				butClearTokens.Enabled=true;
				butClearTokens.Text="Sign out";
				butGmailSettings.Visible=false;
				textRefreshToken.Visible=false;
				labelRefresh.Visible=false;
			}
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

		private void butAuthMicrosoftImageHelper(Image image) {
			butAuthMicrosoft.SuspendLayout();
			if(LayoutManager.ScaleMy()!=1) {
				Size size=new Size(LayoutManager.Scale(image.Width),LayoutManager.Scale(image.Height));
				Bitmap bitmap=new Bitmap(image,size);
				butAuthMicrosoft.Image?.Dispose();
				butAuthMicrosoft.Image=bitmap;//panelEdgeExpress will dispose of this new Bitmap once the control itself is disposed of as normal
			}
			butAuthMicrosoft.ResumeLayout();
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
			if(formUserPick.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Userod userod=Userods.GetUser(formUserPick.SelectedUserNum);
			if(userod.UserNum==(((Userod)textUserod.Tag)?.UserNum??0)) {
				return;//No change.
			}
			//grabs from the database in case the cache hasn't refreshed
			EmailAddress emailAddress=EmailAddresses.GetForUserDb(userod.UserNum);
			if(emailAddress!=null) {
				MsgBox.Show(this,"User email already exists for "+userod.UserName);
				return;
			}
			textUserod.Tag=userod;
			textUserod.Text=userod.UserName;
		}

		private void butClearTokens_Click(object sender,EventArgs e) {
			if(_authenticationType==OAuthType.Google) {
				_emailAddress.DownloadInbox=false;
			}
			else if(_authenticationType==OAuthType.Microsoft && textRefreshToken.Text!="") {
				//Microsft requires it's own way of signing out users in this application rather than just clearing the token.
				MicrosoftTokenHelper microsoftToken=MicrosoftApiConnector.SignOutUser(textUsername.Text,textRefreshToken.Text).Result;
				if(microsoftToken.ErrorMessage!="") {
					MsgBox.Show("Error: "+microsoftToken.ErrorMessage);
					return;
				}
			}
			textAccessToken.Text="";
			textRefreshToken.Text="";
			butClearTokens.Enabled=false;
			_authenticationType=OAuthType.None;
			AdjustEmailTextFields(isEnablingEmailFields:true);
		}

		///<summary>Requests authorization for Open Dental to send emails and access the inbox for a gmail address.
		///Google sends us access and refresh tokens that we store in the database.</summary>
		private void butAuthGoogle_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				CloudClientL.PromptSelections promptSelections=CloudClientL.PromptODCloudClientInstall();
				if(promptSelections==CloudClientL.PromptSelections.ClientRunning) {
					//do nothing
				}
				else if(promptSelections==CloudClientL.PromptSelections.Cancel || promptSelections==CloudClientL.PromptSelections.Download) {
					return;
				}
				else if(promptSelections==CloudClientL.PromptSelections.Launch) {
					try {
						string response=ODCloudClient.SendToBrowserSynchronously("",ODCloudClient.BrowserAction.RelaunchODCloudClientViaBrowser,6);
					}
					catch(Exception) {
						MsgBox.Show("ODCloudClient did not respond, please ensure that the ODCloudClient is installed.");
						return;
					}
					if(CloudClientL.PromptODCloudClientInstall(true)==CloudClientL.PromptSelections.NoResponse) {
						MsgBox.Show("ODCloudClient did not respond, please ensure that the ODCloudClient is installed.");
						return;
					}
					//do nothing
				}
			}
			if(!textAccessToken.Text.IsNullOrEmpty() && _authenticationType==OAuthType.Microsoft) {
				MsgBox.Show("Already signed into Microsoft.");
				return;
			}
			Google.AuthorizationRequest authorizationRequest=new Google.AuthorizationRequest();
			GoogleToken googleToken=null;
			string emailAddress=textUsername.Text;
			ProgressOD progressOD=new ProgressOD();
			progressOD.StartingMessage=Lan.g(this,"Searching for an available port")+"...";
			progressOD.ActionMain=() => {
				authorizationRequest.StartListener();
				ProgressBarEvent.Fire(ODEventType.ProgressBar,Lan.g(this,"Requesting tokens and waiting for a response from Google")+"...");
				googleToken=authorizationRequest.MakeAccessTokenRequest(emailAddress);
			};
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				FriendlyException.Show("Failed to get tokens from Google.",ex);
				return;
			}
			finally {
				authorizationRequest.CloseListener();
			}
			if(progressOD.IsCancelled) {
				return;
			}
			if(googleToken==null) {//This should never happen. An error should be thrown or be in the googleToken.
				MsgBox.Show(this,"Failed to get tokens from Google.");
				return;
			}
			//Errors that occur in WebServiceMainHQ are put in the GoogleToken that is returned.
			if(googleToken.ErrorMessage!="") {
				MsgBox.Show("Error: "+googleToken.ErrorMessage);
				return;
			}
			textAccessToken.Text=googleToken.AccessToken;
			textRefreshToken.Text=googleToken.RefreshToken;
			_authenticationType=OAuthType.Google;
			AdjustEmailTextFields();
			LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXAuthorized,groupAuthentication.Location.Y));
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

		private void butGmailSettings_Click(object sender,EventArgs e) {
			using FormGmailSettings formGmail=new FormGmailSettings();
			//Information gets updated on the okClick method for this form so no need to manage it out here
			formGmail.GmailAddress=_emailAddress;
			formGmail.IsNew=_isNew;
			formGmail.ShowDialog();
			AdjustEmailTextFields();
		}

		private void butAuthMicrosoft_Click(object sender,EventArgs e) {
			if(!textAccessToken.Text.IsNullOrEmpty() && _authenticationType==OAuthType.Google) {
				MsgBox.Show("Already signed into Google.");
				return;
			}
			MicrosoftTokenHelper microsoftToken=System.Threading.Tasks.Task.Run(async () =>
				await MicrosoftApiConnector.GetAccessToken(textUsername.Text,textRefreshToken.Text)
			).GetAwaiter().GetResult();
			if(microsoftToken.ErrorMessage!="") {
				MsgBox.Show("Error: "+microsoftToken.ErrorMessage);
				return;
			}
			if(microsoftToken.AccessToken=="") {
				return; //Authentication was cancelled so do nothing.
			}
			textUsername.Text=microsoftToken.EmailAddress;
			textAccessToken.Text=microsoftToken.AccessToken;
			textRefreshToken.Text=microsoftToken.AccountInfo;
			_authenticationType=OAuthType.Microsoft;
			//These two settings are for Gmail so make sure they're cleared.
			_emailAddress.QueryString="";
			_emailAddress.DownloadInbox=false;
			AdjustEmailTextFields();
			LayoutManager.MoveLocation(groupAuthentication,new Point(_groupAuthLocationXAuthorized,groupAuthentication.Location.Y));
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
			_emailAddress.AuthenticationType=_authenticationType;
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
