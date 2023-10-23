using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {

	public partial class FormEServicesPatientPortal:FormODBase {
		private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		private WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService _signupOutEServiceHQUrls;
		private string _webMailNotifBody="";
		private bool _isWebMailRawHtml;

		public FormEServicesPatientPortal(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_signupOut=signupOut;
		}

		private void FormEServicesPatientPortal_Load(object sender,EventArgs e) {
			if(_signupOut==null){
				_signupOut=FormEServicesSetup.GetSignupOut();
			}
			//Office may have set a customer URL
			textPatientFacingUrlPortal.Text=PrefC.GetString(PrefName.PatientPortalURL);
			//HQ provides this URL for this customer.
			_signupOutEServiceHQUrls=WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut,eServiceCode.PatientPortal).FirstOrDefault();
			if(_signupOutEServiceHQUrls==null) {
				_signupOutEServiceHQUrls=new WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService();
				_signupOutEServiceHQUrls.HostedUrl="";
				_signupOutEServiceHQUrls.HostedUrlPayment="";
			}
			textHostedUrlPortal.Text=_signupOutEServiceHQUrls.HostedUrl;
			if(textPatientFacingUrlPortal.Text=="") { //Customer has not set their own URL so use the URL provided by OD.
				textPatientFacingUrlPortal.Text=_signupOutEServiceHQUrls.HostedUrl;
			}
			textBoxNotificationSubject.Text=PrefC.GetString(PrefName.PatientPortalNotifySubject);
			_webMailNotifBody=PrefC.GetString(PrefName.PatientPortalNotifyBody);
			RefreshEmail(browserWebMailNotificatonBody,_webMailNotifBody);
			_isWebMailRawHtml=PrefC.GetEnum<EmailType>(PrefName.PortalWebEmailTemplateType)==EmailType.RawHtml;
			bool allowEdit=Security.IsAuthorized(EnumPermType.EServicesSetup,suppressMessage:true);
			groupBoxNotification.Enabled=allowEdit;
			butCustomUrl.Visible=allowEdit;
			ConstructURL();
		}

		///<summary>When customize URL button is clicked, prompt for custom URL.</summary>
		private void butCustomUrl_Click (object sender,EventArgs e) {
			using InputBox inputBox=new InputBox(Lan.g(this,"Input Custom URL"));
			inputBox.ShowDialog();
			if(inputBox.DialogResult==DialogResult.OK && !inputBox.textResult.Text.IsNullOrEmpty()) {
				textPatientFacingUrlPortal.Text=inputBox.textResult.Text;
			}
		}

		private void butUpdateURL_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will update your Patient Facing URL to match your Hosted URL. " +
				"This will remove any customization to your URL. Do you want to proceed?"))
			{
				return;	
			}
			Prefs.UpdateString(PrefName.PatientPortalURL,_signupOutEServiceHQUrls.HostedUrl);
			textPatientFacingUrlPortal.Text=_signupOutEServiceHQUrls.HostedUrl;
		}

		#region ConstructURLS
		private void comboPPClinicUrl_SelectedIndexChanged(object sender,EventArgs e) {
			ConstructURL();
		}

		private void butCopyToClipboard_Click(object sender,EventArgs e) {
			ODClipboard.SetClipboard(textHostedUrlPortal.Text);
		}

		private void butNavigateTo_Click(object sender,EventArgs e) {
			if(!string.IsNullOrWhiteSpace(textHostedUrlPortal.Text)) {
				Process.Start(textHostedUrlPortal.Text);
			}
		}

		private void ConstructURL() {
			textHostedUrlPortal.Clear();
			string url=_signupOutEServiceHQUrls.HostedUrl;
			if(comboPPClinicUrl.ClinicNumSelected>0) {//'None' is not selected
				url+="&CID="+comboPPClinicUrl.ClinicNumSelected;
			}
			textHostedUrlPortal.Text=url;
		}
		#endregion

		private void butEditWebMailNotificationBody_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit {
				MarkupText=_webMailNotifBody,
				DoCheckForDisclaimer=true,
				IsRawAllowed=true,
				IsRaw=_isWebMailRawHtml
			};
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_webMailNotifBody=formEmailEdit.MarkupText;
			_isWebMailRawHtml=formEmailEdit.IsRaw;
			RefreshEmail(browserWebMailNotificatonBody,_webMailNotifBody);
		}

		private void RefreshEmail(WebBrowser webBrowser,string emailText) {
			if(_isWebMailRawHtml) {
				webBrowser.DocumentText=emailText;
				return;//text is already in HTML, it does not need to be translated. 
			}
			ODException.SwallowAnyException(() => {
				string text = MarkupEdit.TranslateToXhtml(emailText,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true);
				webBrowser.DocumentText=text;
			});
		}

		private void SaveTabPatientPortal() {
			Prefs.UpdateString(PrefName.PatientPortalURL,textPatientFacingUrlPortal.Text);
			Prefs.UpdateString(PrefName.PatientPortalNotifySubject,textBoxNotificationSubject.Text);
			Prefs.UpdateString(PrefName.PatientPortalNotifyBody,_webMailNotifBody);
			EmailType emailType=EmailType.Html;
			if(_isWebMailRawHtml) {
				emailType=EmailType.RawHtml;
			}
			Prefs.UpdateInt(PrefName.PortalWebEmailTemplateType,(int)emailType);
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ODBuild.IsDebug()) {
				if(!textPatientFacingUrlPortal.Text.ToUpper().StartsWith("HTTPS")) {
					MsgBox.Show(this,"Patient Facing URL must start with HTTPS.");
					return;
				}
			}
			if(textBoxNotificationSubject.Text=="") {
				MsgBox.Show(this,"Notification Subject is empty");
				textBoxNotificationSubject.Focus();
				return;
			}
			if(string.IsNullOrEmpty(_webMailNotifBody)) {
				MsgBox.Show(this,"Notification Body is empty");
				butEditWebMailNotificationBody.Focus();
				return;
			}
			if(!_webMailNotifBody.Contains("[URL]")) { //prompt user that they omitted the URL field but don't prevent them from continuing
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"[URL] not included in notification body. Continue without setting the [URL] field?")) {
					butEditWebMailNotificationBody.Focus();
					return;
				}
			}
			SaveTabPatientPortal();
			DialogResult=DialogResult.OK;
		}

	}
}