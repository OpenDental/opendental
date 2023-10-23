using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailSend:FormODBase {
		private List<PatientInfo> _listPatientInfos=new List<PatientInfo>();
		private EmailAddress _emailAddressReplyTo;
		private List<IdentityResource> _listIdentityResources;

		public FormMassEmailSend() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailSend_Load(object sender,EventArgs e) {
			LayoutMenu();
			LoadUI();
		}

		private void LoadUI() {
			long clinicNum=comboClinic.ClinicNumSelected;
			LoadVerifiedEmails(clinicNum);
			UpdateClinicIsNotEnabled(clinicNum);
			FillComboEmailHostingTemplate(clinicNum);
			SetDefaultReplyToEmailAddress(clinicNum);
			textboxAlias.Text=GetEmailAlias(clinicNum);
			PreviewTemplate();
		}

		private void PreviewTemplate() {
			PatientInfo patientInfo=null;//Perfectly fine to be null.
			if(textPatient.Tag is PatientInfo patientInfoSelected && checkDisplay.Checked) {
				patientInfo=patientInfoSelected;
			}
			EmailHostingTemplate emailHostingTemplate=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			textEmailGroup.Text=emailHostingTemplate?.TemplateName;
			textSubject.Text=emailHostingTemplate?.Subject;
			userControlEmailTemplate1.RefreshView(emailHostingTemplate?.GetBodyPlainText(patientInfo),emailHostingTemplate?.GetBodyHtmlText(patientInfo),EmailType.RawHtml);
		}
		
		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.Add(new MenuItemOD("Templates",menuItemTemplates_Click));
			menuMain.EndUpdate();
		}

		private void FillComboEmailHostingTemplate(long clinicNum) {
			long emailHostingTemplateNumOld=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>()?.EmailHostingTemplateNum??0;
			comboEmailHostingTemplate.Items.Clear();
			List<EmailHostingTemplate> listEmailHostingTemplates=EmailHostingTemplates.Refresh().FindAll(x => x.TemplateType==PromotionType.Manual && x.ClinicNum==clinicNum);
			int selectedIndex=0;
			for(int i=0;i<listEmailHostingTemplates.Count;i++) {
				EmailHostingTemplate emailHostingTemplate=listEmailHostingTemplates[i];
				if(emailHostingTemplateNumOld==emailHostingTemplate.EmailHostingTemplateNum) {
					selectedIndex=i;
				}
				comboEmailHostingTemplate.Items.Add(emailHostingTemplate.TemplateName,emailHostingTemplate);
			}
			comboEmailHostingTemplate.SelectedIndex=selectedIndex;
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormMassEmailSetup formMassEmailSetup=new FormMassEmailSetup();
			formMassEmailSetup.ShowDialog();
			LoadUI();
		}

		private void menuItemTemplates_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup)) {
				return;
			}
			using FormMassEmailTemplates formMassEmailTemplates=new FormMassEmailTemplates();
			formMassEmailTemplates.ShowDialog();
			//In case a new template was added, refresh the template comboBox
			FillComboEmailHostingTemplate(comboClinic.ClinicNumSelected);
			PreviewTemplate();
		}
		
		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			LoadUI();
		}

		private void UpdateClinicIsNotEnabled(long clinicNum) {
			labelNotEnabled.Text=Lan.g(this,"* Mass Email not enabled for office");
			if(PrefC.HasClinicsEnabled) {
				labelNotEnabled.Text=Lan.g(this,"* Mass Email not enabled for clinic");
			}
			labelNotEnabled.Visible=!Clinics.IsMassEmailEnabled(clinicNum);
			butSendEmails.Enabled=Clinics.IsMassEmailEnabled(clinicNum);
		}

		private void comboEmailHostingTemplate_SelectionChangeCommitted(object sender,EventArgs e) {
			PreviewTemplate();
		}

		///<summary>Asynchronously loads verified email addresses from AccountApi.</summary>
		private void LoadVerifiedEmails(long clinicNum) {
			ODThread thread=new ODThread(o => {
				if(!Clinics.HasEmailHostingCredentials(clinicNum)) {
					return;
				}
				IAccountApi iAccountApi=EmailHostingTemplates.GetAccountApi(clinicNum);
				GetIdentitiesResponse getIndentitiesResponse=iAccountApi.GetIdentities(new GetIdentitiesRequest());
				_listIdentityResources=getIndentitiesResponse.ListIdentityResources;
			});
			thread.AddExceptionHandler(ex => ex.DoNothing());
			thread.AddExitHandler(o => UpdateSendFromMyAddress(clinicNum));
			thread.Start();
		}

		///<summary>Sets the sender EmailAddress, updates the UI, and shows/hides the checkbox to use the SenderAddress.</summary>
		private void SetEmailAddressSender(EmailAddress emailAddress,long clinicNum) {
			_emailAddressReplyTo=emailAddress;
			textSenderAddress.Text=_emailAddressReplyTo?.EmailUsername??"";
			UpdateSendFromMyAddress(clinicNum);
		}

		///<summary>Sets the checkbox visible if the ReplyToAddress is a verified EmailAddress</summary>
		private void UpdateSendFromMyAddress(long clinicNum) {
			this.InvokeIfRequired(() => {
				IdentityResource identityResource=GetIdentity(_emailAddressReplyTo);
				radioReturnAddress.Text=_emailAddressReplyTo?.EmailUsername??Lan.g(this,"Select a Return address");
				radioNoReply.Checked=true;
				if(identityResource is null) {
					radioReturnAddress.Text+=Lan.g(this,"(Unverified)");
					radioReturnAddress.Enabled=false;
					return;
				}
				radioReturnAddress.Text+=$"(Verification {identityResource.VerificationStatus.GetDescription()})";
				bool isVerified=identityResource.VerificationStatus==IdentityVerificationStatus.Success;
				radioReturnAddress.Enabled=isVerified;
				radioReturnAddress.Checked=isVerified && !ClinicPrefs.GetBool(PrefName.EmailHostingUseNoReply,clinicNum);
			});
		}

		///<summary>Determines if the given EmailAddress has been verified with the EmailHosting service.</summary>
		private IdentityResource GetIdentity(EmailAddress emailAddress) {
			if(emailAddress is null) {
				return null;
			}
			return _listIdentityResources?.FirstOrDefault(x => x.Identity.ToLower()==emailAddress.EmailUsername.ToLower());
		}

		///<summary>Returns the email alias.</summary> 
		private string GetEmailAlias(long clinicNum) {
			string emailAlias;
			if(clinicNum==0) {
				emailAlias=PrefC.GetString(PrefName.EmailHostingAlias);
			}
			else {
				emailAlias=ClinicPrefs.GetPrefValue(PrefName.EmailHostingAlias,clinicNum);
			}
			if(!string.IsNullOrWhiteSpace(emailAlias)) {
				return emailAlias;
			}
			EmailAddress emailAddress=EmailAddresses.GetOne(PIn.Long(PrefC.GetString(PrefName.EmailDefaultAddressNum)));
			if(emailAddress!=null && !string.IsNullOrWhiteSpace(emailAddress.GetFrom())) {
				emailAlias=emailAddress.GetFrom();
			}
			if(!PrefC.HasClinicsEnabled){
				return emailAlias;
			}
			Clinic clinic=Clinics.GetClinic(clinicNum);
			if(clinic==null || clinic.EmailAddressNum==0) {
				return emailAlias;
			}
			emailAddress=EmailAddresses.GetOne(clinic.EmailAddressNum);
			if(emailAddress!=null && !string.IsNullOrWhiteSpace(emailAddress.GetFrom())) {
				emailAlias=emailAddress.GetFrom();
			}
			return emailAlias;
		}

		private void SetDefaultReplyToEmailAddress(long clinicNum) {
			EmailAddress emailAddress=null;
			long emailAddressNum;
			Clinic clinic=Clinics.GetClinic(clinicNum);
			if(clinic!=null) {
				emailAddressNum=clinic.EmailAddressNum;
				emailAddress=EmailAddresses.GetOne(emailAddressNum);
			}
			if(emailAddress is null) {
				emailAddressNum=PrefC.GetLong(PrefName.EmailDefaultAddressNum);
				emailAddress=EmailAddresses.GetOne(emailAddressNum);
			}
			SetEmailAddressSender(emailAddress,clinicNum);
		}

		private void checkDisplay_Click(object sender,EventArgs e) {
			PreviewTemplate();
		}

		private void butPatientSelect_Click(object sender,EventArgs e) {
			//Build our grid to display for the form we are about to show the user. 
			List<UI.GridColumn> listColumns=new List<UI.GridColumn>();
			listColumns.Add(new UI.GridColumn(Lan.g(this,"Patient"),0));
			List<UI.GridRow> listRows=new List<UI.GridRow>();
			for(int i=0;i<_listPatientInfos.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(_listPatientInfos[i].Name);
				row.Tag=_listPatientInfos[i];
				listRows.Add(row);
			}
			using FormGridSelection formGridSelection=new FormGridSelection(listColumns,listRows,Lan.g(this,"Select Patient"),Lan.g(this,"Select Patient"));
			if(formGridSelection.ShowDialog()!=DialogResult.OK) {
				return;
			}
			PatientInfo patientInfoSelected=(PatientInfo)formGridSelection.ListSelectedTags.FirstOrDefault();
			SetSelectedPatient(patientInfoSelected);
		}

		private void SetSelectedPatient(PatientInfo patientInfoSelected) {
			textPatient.Tag=patientInfoSelected;
			string name="";
			if(patientInfoSelected!=null) {
				name=patientInfoSelected.LName+", "+patientInfoSelected.FName;
			}
			textPatient.Text=name;//fires TextChanged, which will call PreviewTempalte().
		}

		private void textPatient_TextChanged(object sender,EventArgs e) {
			checkDisplay.Enabled=textPatient.Tag is PatientInfo;
			checkDisplay.Checked&=checkDisplay.Enabled;
			PreviewTemplate();
		}

		private void butSelectSender_Click(object sender,EventArgs e) {
			using FormEmailAddresses formEmailAddresses=new FormEmailAddresses();
			formEmailAddresses.IsSelectionMode=true;
			DialogResult dialogResult=formEmailAddresses.ShowDialog();
			if(dialogResult!=DialogResult.OK || formEmailAddresses.EmailAddressNum==0) {
				return;
			}
			EmailAddress emailAddress=EmailAddresses.GetOne(formEmailAddresses.EmailAddressNum);
			SetEmailAddressSender(emailAddress,comboClinic.ClinicNumSelected);
		}

		private void butVerifications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup)) {
				return;
			}
			using FormEmailHostingAddressVerification formEmailHostingAddressVerification=new FormEmailHostingAddressVerification();
			formEmailHostingAddressVerification.ShowDialog();
			long clinicNum=comboClinic.ClinicNumSelected;
			//This form ran web calls to get the most up-to-date verified addresses.  The user has to click a link in the verification email to finish the
			//process for any new verifications to be "successful".
			_listIdentityResources=formEmailHostingAddressVerification.GetVerifiedAddresses(clinicNum);
			UpdateSendFromMyAddress(clinicNum);
		}

		private void butSelectPatients_Click(object sender,EventArgs e) {
			using FormAdvertisingPatientList formAdvertisingPatientList=new FormAdvertisingPatientList(AdvertisingType.MassEmail);
			if(formAdvertisingPatientList.ShowDialog()==DialogResult.OK) {
				_listPatientInfos=formAdvertisingPatientList.ListPatientInfos;
			}
			textNumberOfRecipients.Text=_listPatientInfos.Count.ToString();
			PatientInfo patientInfo=null;
			if(textPatient.Tag is PatientInfo patientInfoSelected) {
				patientInfo=_listPatientInfos.FirstOrDefault(x => x.PatNum==patientInfoSelected.PatNum);
			}
			SetSelectedPatient(patientInfo);
		}

		private void butSendEmails_Click(object sender,EventArgs e) {
			if(_listPatientInfos.Count<1) {
				MsgBox.Show(this,"At least one patient must be selected.");
				return;
			}
			EmailHostingTemplate emailHostingTemplate=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			if(emailHostingTemplate is null) {
				MsgBox.Show(this,"A template must be selected.");
				return;
			}
			string htmlSignature=emailHostingTemplate.GetHtmlSignature();
			string plainTextSignature=emailHostingTemplate.GetPlainTextSignature();
			if(string.IsNullOrWhiteSpace(htmlSignature) || string.IsNullOrWhiteSpace(plainTextSignature)) {
				MsgBox.Show(this,"Email signatures cannot be blank. Go to 'Setup'.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textboxAlias.Text)) {
				MsgBox.Show(this,labelEmailAlias.Text+" cannot be blank.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textEmailGroup.Text)) {
				MsgBox.Show(this,"Mass email group name cannot be blank.");
				return;
			}
			string promotionName=textEmailGroup.Text;
			if(string.IsNullOrWhiteSpace(textSenderAddress.Text)) {
				MsgBox.Show(this,"Return email address cannot be blank.");
				return;
			}
			//Where emailAddress used to be set.
			if(_emailAddressReplyTo==null) {
				MsgBox.Show("Please select a valid return Email Address");
				return;
			}
			//This object will parse out the sender name and email address.
			MailAddress mailAddressReplyTo;
			try {
				//This object will parse out the sender name and email address.
				mailAddressReplyTo=new MailAddress(_emailAddressReplyTo.GetFrom());
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Practice or clinic email address is invalid. Please check the Email Addresses window and try again."),ex);
				return;
			}
			string message=Lan.g(this,"Are you sure you want to send this email to ")+_listPatientInfos.Count+Lan.g(this," patients?");
			if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			PatientInfo patientInfo=_listPatientInfos[0];
			List<MassEmailDestination> listMassEmailDestinations=new List<MassEmailDestination>() {
				new MassEmailDestination { 
					PatNum=patientInfo.PatNum,
					AptNum=patientInfo.NextAptNum,
					ToAddress=mailAddressReplyTo.Address
				}
			};
			string alias=textboxAlias.Text;
			string senderAddress="";
			if(radioReturnAddress.Checked) {
				//If the user wants to send these emails from their own address, and the address is verified with EmailHosting, otherwise, an empty string
				//will be sent which will translate to the EmailHosting noreply email address.
				senderAddress=mailAddressReplyTo.Address;
			}
			MassEmailSendResult massEmailSendResult=null;
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
				massEmailSendResult=Promotions.SendEmails(emailHostingTemplate,
					listMassEmailDestinations,//This is just one email, the sender.
					alias,mailAddressReplyTo.Address,promotionName,PromotionType.Manual,emailHostingTemplate.ClinicNum,senderAddress,isVerificationBatch:true);
			};
			progressOD.StartingMessage=Lan.g(this,"Sending verification email...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(!(massEmailSendResult?.IsSuccess??false)) {
				MsgBox.Show(massEmailSendResult?.ResultDescription??"An unexpected error occurred.");
				return;
			}
			message=$"A test email was sent to {mailAddressReplyTo.Address}.\r\n"
				+$"Choose Yes if the test email was received, looked appropriate, and you want to send {_listPatientInfos.Count} patient email(s).\r\n"
				+"Choose No if the test email was not received or if the email did not look appropriate. No patient emails will be sent.";
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
				return;
			}
			//Now send out the mass email to all of the recipients.
			progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
				massEmailSendResult=Promotions.SendEmails(emailHostingTemplate,
					_listPatientInfos.Select(x => new MassEmailDestination { PatNum=x.PatNum,AptNum=x.NextAptNum,ToAddress=x.Email}).ToList(),
					alias,mailAddressReplyTo.Address,promotionName,PromotionType.Manual,emailHostingTemplate.ClinicNum,senderAddress);
			};
			progressOD.StartingMessage=Lan.g(this,"Sending emails...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(!(massEmailSendResult?.IsSuccess??false)) {
				MsgBox.Show(massEmailSendResult?.ResultDescription??"An unexpected error occurred.");
				return;
			}
			if(emailHostingTemplate.ClinicNum==0) {
				Prefs.UpdateString(PrefName.EmailHostingAlias,textboxAlias.Text);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailHostingAlias,emailHostingTemplate.ClinicNum,textboxAlias.Text);
			}
			using FormMassEmailSendResults formMassEmailSendResults=new FormMassEmailSendResults(_listPatientInfos,massEmailSendResult);
			formMassEmailSendResults.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}