using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {

	public partial class FormEServicesPatientPortal:FormODBase {
		WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;
		private WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService _signupOutEServiceUrlsFromHQ;
		private string _webMailNotificationBody="";
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
			_signupOutEServiceUrlsFromHQ=WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut,eServiceCode.PatientPortal).FirstOrDefault()??new WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService() { HostedUrl="", HostedUrlPayment="" };
			textHostedUrlPortal.Text=_signupOutEServiceUrlsFromHQ.HostedUrl;
			if(textPatientFacingUrlPortal.Text=="") { //Customer has not set their own URL so use the URL provided by OD.
				textPatientFacingUrlPortal.Text=_signupOutEServiceUrlsFromHQ.HostedUrl;
			}
			textBoxNotificationSubject.Text=PrefC.GetString(PrefName.PatientPortalNotifySubject);
			_webMailNotificationBody=PrefC.GetString(PrefName.PatientPortalNotifyBody);
			RefreshEmail(browserWebMailNotificatonBody,_webMailNotificationBody);
			_isWebMailRawHtml=PrefC.GetEnum<EmailType>(PrefName.PortalWebEmailTemplateType)==EmailType.RawHtml;
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			groupBoxNotification.Enabled=allowEdit;
			butCustomUrl.Visible=allowEdit;
		}

		///<summary>When customize URL button is clicked, prompt for custom URL.</summary>
		private void butCustomUrl_Click (object sender,EventArgs e) {
			using InputBox inputBox=new InputBox(Lan.g(this,"Input Custom URL"));
			inputBox.ShowDialog();
			if(inputBox.DialogResult==DialogResult.OK && !inputBox.textResult.Text.IsNullOrEmpty()) {
				textPatientFacingUrlPortal.Text=inputBox.textResult.Text;
			}
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

		private void radioPatientPortal_CheckedChanged(object sender,EventArgs e) {
			ConstructURL();
		}

		private void ConstructURL() {
			textHostedUrlPortal.Clear();
			string url=_signupOutEServiceUrlsFromHQ.HostedUrlPayment;
			if(radioPatientPortalLogin.Checked) {
				url=_signupOutEServiceUrlsFromHQ.HostedUrl;
			}
			if(comboPPClinicUrl.SelectedClinicNum>0) {//'None' is not selected
				url+="&CID="+comboPPClinicUrl.SelectedClinicNum;
			}
			textHostedUrlPortal.Text=url;
		}
		#endregion

		private void butEditWebMailNotificationBody_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit {
				MarkupText=_webMailNotificationBody,
				DoCheckForDisclaimer=true,
				IsRawAllowed=true,
				IsRaw=_isWebMailRawHtml
			};
			formEmailEdit.ShowDialog();
			if(formEmailEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_webMailNotificationBody=formEmailEdit.MarkupText;
			_isWebMailRawHtml=formEmailEdit.IsRaw;
			RefreshEmail(browserWebMailNotificatonBody,_webMailNotificationBody);
		}

		private void RefreshEmail(WebBrowser emailBody,string emailText) {
			if(_isWebMailRawHtml) {
				emailBody.DocumentText=emailText;
				return;//text is already in HTML, it does not need to be translated. 
			}
			ODException.SwallowAnyException(() => {
				string text = MarkupEdit.TranslateToXhtml(emailText,isPreviewOnly: true,hasWikiPageTitles: false,isEmail: true);
				emailBody.DocumentText=text;
			});
		}

		private void SaveTabPatientPortal() {
			Prefs.UpdateString(PrefName.PatientPortalURL,textPatientFacingUrlPortal.Text);
			Prefs.UpdateString(PrefName.PatientPortalNotifySubject,textBoxNotificationSubject.Text);
			Prefs.UpdateString(PrefName.PatientPortalNotifyBody,_webMailNotificationBody);
			EmailType emailType=EmailType.Html;
			if(_isWebMailRawHtml) {
				emailType=EmailType.RawHtml;
			}
			Prefs.UpdateInt(PrefName.PortalWebEmailTemplateType,(int)emailType);
		}

		private void butOK_Click(object sender,EventArgs e) {
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
			if(string.IsNullOrEmpty(_webMailNotificationBody)) {
				MsgBox.Show(this,"Notification Body is empty");
				butEditWebMailNotificationBody.Focus();
				return;
			}
			if(!_webMailNotificationBody.Contains("[URL]")) { //prompt user that they omitted the URL field but don't prevent them from continuing
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"[URL] not included in notification body. Continue without setting the [URL] field?")) {
					butEditWebMailNotificationBody.Focus();
					return;
				}
			}
			SaveTabPatientPortal();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}