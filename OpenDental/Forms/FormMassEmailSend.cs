using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailSend:FormODBase {
		private List<PatientInfo> _listPatientsSelected=new List<PatientInfo>();
		private EmailAddress _emailAddressReplyTo;
		private List<IdentityResource> _listVerifiedEmailAddresses;

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
			long clinicNum=comboClinic.SelectedClinicNum;
			LoadVerifiedEmails(clinicNum);
			UpdateClinicIsNotEnabled(clinicNum);
			FillComboEmailHostingTemplate(clinicNum);
			SetDefaultReplyToEmailAddress(clinicNum);
			textboxAlias.Text=GetEmailAlias(clinicNum);
			PreviewTemplate();
		}

		private void PreviewTemplate() {
			PatientInfo pat=null;//Perfectly fine to be null.
			if(textPatient.Tag is PatientInfo patSelected && checkDisplay.Checked) {
				pat=patSelected;
			}
			EmailHostingTemplate template=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			textEmailGroup.Text=template?.TemplateName;
			textSubject.Text=template?.Subject;
			userControlEmailTemplate1.RefreshView(template?.GetBodyPlainText(pat),template?.GetBodyHtmlText(pat),EmailType.RawHtml);
		}
		
		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.Add(new MenuItemOD("Templates",menuItemTemplates_Click));
			menuMain.EndUpdate();
		}

		private void FillComboEmailHostingTemplate(long clinicNum) {
			long templateNumOld=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>()?.EmailHostingTemplateNum??0;
			comboEmailHostingTemplate.Items.Clear();
			List<EmailHostingTemplate> listTemplates=EmailHostingTemplates.Refresh().FindAll(x => x.TemplateType==PromotionType.Manual && x.ClinicNum==clinicNum);
			int selectedIndex=0;
			for(int i=0;i<listTemplates.Count;i++) {
				EmailHostingTemplate template=listTemplates[i];
				if(templateNumOld==template.EmailHostingTemplateNum) {
					selectedIndex=i;
				}
				comboEmailHostingTemplate.Items.Add(template.TemplateName,template);
			}
			comboEmailHostingTemplate.SelectedIndex=selectedIndex;
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormMassEmailSetup formSetup=new FormMassEmailSetup();
			formSetup.ShowDialog();
			LoadUI();
		}

		private void menuItemTemplates_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			using FormMassEmailTemplates formMassEmailTemplates=new FormMassEmailTemplates();
			formMassEmailTemplates.ShowDialog();
			//In case a new template was added, refresh the template comboBox
			FillComboEmailHostingTemplate(comboClinic.SelectedClinicNum);
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
				IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
				GetIdentitiesResponse response=api.GetIdentities(new GetIdentitiesRequest());
				_listVerifiedEmailAddresses=response.ListIdentityResources;
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
				IdentityResource identity=GetIdentity(_emailAddressReplyTo);
				radioReturnAddress.Text=_emailAddressReplyTo?.EmailUsername??Lan.g(this,"Select a Return address");
				radioNoReply.Checked=true;
				if(identity is null) {
					radioReturnAddress.Text+=Lan.g(this,"(Unverified)");
					radioReturnAddress.Enabled=false;
				}
				else {
					radioReturnAddress.Text+=$"(Verification {identity.VerificationStatus.GetDescription()})";
					bool isVerified=identity.VerificationStatus==IdentityVerificationStatus.Success;
					radioReturnAddress.Enabled=isVerified;
					radioReturnAddress.Checked=isVerified && !ClinicPrefs.GetBool(PrefName.EmailHostingUseNoReply,clinicNum);
				}
			});
		}

		///<summary>Determines if the given EmailAddress has been verified with the EmailHosting service.</summary>
		private IdentityResource GetIdentity(EmailAddress address) {
			if(address is null) {
				return null;
			}
			return _listVerifiedEmailAddresses?.FirstOrDefault(x => x.Identity.ToLower()==address.EmailUsername.ToLower());
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
			EmailAddress address=EmailAddresses.GetOne(PIn.Long(PrefC.GetString(PrefName.EmailDefaultAddressNum)));
			if(address!=null && !string.IsNullOrWhiteSpace(address.GetFrom())) {
				emailAlias=address.GetFrom();
			}
			if(PrefC.HasClinicsEnabled) {
				Clinic clinic=Clinics.GetClinic(clinicNum);
				if(clinic!=null && clinic.EmailAddressNum!=0) {
					address=EmailAddresses.GetOne(clinic.EmailAddressNum);
					if(address!=null && !string.IsNullOrWhiteSpace(address.GetFrom())) {
						emailAlias=address.GetFrom();
					}
				}
			}
			return emailAlias;
		}

		private void SetDefaultReplyToEmailAddress(long clinicNum) {
			EmailAddress address=null;
			long emailAddressNum;
			Clinic clinic=Clinics.GetClinic(clinicNum);
			if(clinic!=null) {
				emailAddressNum=clinic.EmailAddressNum;
				address=EmailAddresses.GetOne(emailAddressNum);
			}
			if(address is null) {
				emailAddressNum=PrefC.GetLong(PrefName.EmailDefaultAddressNum);
				address=EmailAddresses.GetOne(emailAddressNum);
			}
			SetEmailAddressSender(address,clinicNum);
		}

		private void checkDisplay_Click(object sender,EventArgs e) {
			PreviewTemplate();
		}

		private void butPatientSelect_Click(object sender,EventArgs e) {
			//Build our grid to display for the form we are about to show the user. 
			List<UI.GridColumn> listColumns=new List<UI.GridColumn>();
			listColumns.Add(new UI.GridColumn(Lan.g(this,"Patient"),0));
			List<UI.GridRow> listRows=new List<UI.GridRow>();
			foreach(PatientInfo patient in _listPatientsSelected) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(patient.Name);
				row.Tag=patient;
				listRows.Add(row);
			}
			using FormGridSelection formGridSelection=new FormGridSelection(listColumns,listRows,Lan.g(this,"Select Patient"),Lan.g(this,"Select Patient"));
			if(formGridSelection.ShowDialog()!=DialogResult.OK) {
				return;
			}
			PatientInfo selectedPat=(PatientInfo)formGridSelection.ListSelectedTags.FirstOrDefault();
			SetSelectedPatient(selectedPat);
		}

		private void SetSelectedPatient(PatientInfo selectedPat) {
			textPatient.Tag=selectedPat;
			string name="";
			if(selectedPat!=null) {
				name=selectedPat.LName+", "+selectedPat.FName;
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
			DialogResult result=formEmailAddresses.ShowDialog();
			if(result!=DialogResult.OK || formEmailAddresses.EmailAddressNum==0) {
				return;
			}
			EmailAddress emailAddress=EmailAddresses.GetOne(formEmailAddresses.EmailAddressNum);
			SetEmailAddressSender(emailAddress,comboClinic.SelectedClinicNum);
		}

		private void butVerifications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			using FormEmailHostingAddressVerification formVerification=new FormEmailHostingAddressVerification();
			formVerification.ShowDialog();
			long clinicNum=comboClinic.SelectedClinicNum;
			//This form ran web calls to get the most up-to-date verified addresses.  The user has to click a link in the verification email to finish the
			//process for any new verifications to be "successful".
			_listVerifiedEmailAddresses=formVerification.GetVerifiedAddresses(clinicNum);
			UpdateSendFromMyAddress(clinicNum);
		}

		private void butSelectPatients_Click(object sender,EventArgs e) {
			MassEmailFilter massEmailFilter=new MassEmailFilter();
			using FormAdvertisingPatientList formPatientList=new FormAdvertisingPatientList(massEmailFilter.DoIncludePatient,massEmailFilter);
			if(formPatientList.ShowDialog()==DialogResult.OK) {
				_listPatientsSelected=formPatientList.ListPatientInfos;
			}
			textNumberOfRecipients.Text=_listPatientsSelected.Count.ToString();
			PatientInfo pat=null;
			if(textPatient.Tag is PatientInfo patSelected) {
				pat=_listPatientsSelected.FirstOrDefault(x => x.PatNum==patSelected.PatNum);
			}
			SetSelectedPatient(pat);
		}

		private void butSendEmails_Click(object sender,EventArgs e) {
			if(_listPatientsSelected.Count<1) {
				MsgBox.Show(this,"At least one patient must be selected.");
				return;
			}
			EmailHostingTemplate template=comboEmailHostingTemplate.GetSelected<EmailHostingTemplate>();
			if(template is null) {
				MsgBox.Show(this,"A template must be selected.");
				return;
			}
			string htmlSignature=template.GetHtmlSignature();
			string plainTextSignature=template.GetPlainTextSignature();
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
			MailAddress replyToAddress;
			try {
				//This object will parse out the sender name and email address.
				replyToAddress=new MailAddress(_emailAddressReplyTo.GetFrom());
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Practice or clinic email address is invalid. Please check the Email Addresses window and try again."),ex);
				return;
			}
			string message=Lan.g(this,"Are you sure you want to send this email to ")+_listPatientsSelected.Count+Lan.g(this," patients?");
			if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			//Send to self first to confirm that everything looks apropriate.
			string response=null;
			PatientInfo patientInfo=_listPatientsSelected[0];
			List<MassEmailDestination> listDestinations=new List<MassEmailDestination>() {
				new MassEmailDestination { 
					PatNum=patientInfo.PatNum,
					AptNum=patientInfo.NextAptNum,
					ToAddress=replyToAddress.Address
				}
			};
			string alias=textboxAlias.Text;
			string senderAddress="";
			if(radioReturnAddress.Checked) {
				//If the user wants to send these emails from their own address, and the address is verified with EmailHosting, otherwise, an empty string
				//will be sent which will translate to the EmailHosting noreply email address.
				senderAddress=replyToAddress.Address;
			}
			UI.ProgressOD progressOD=new UI.ProgressOD();
			Func<string,bool> isErrorMsg=new Func<string,bool>((errorMsg) => { 
				if(!string.IsNullOrWhiteSpace(errorMsg) && !long.TryParse(errorMsg,out long num)) {
					return true;
				}
				return false;
			});
			progressOD.ActionMain=() => {
				response=Promotions.SendEmails(template,
					listDestinations,//This is just one email, the sender.
					alias,replyToAddress.Address,promotionName,PromotionType.Manual,template.ClinicNum,senderAddress,isVerificationBatch:true);
			};
			progressOD.StartingMessage=Lan.g(this,"Sending verification email...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(isErrorMsg(response)) {
				MsgBox.Show(response);
				return;
			}
			message=$"A test email was sent to {replyToAddress.Address}.\r\n"
				+$"Choose Yes if the test email was received, looked appropriate, and you want to send {_listPatientsSelected.Count} patient email(s).\r\n"
				+"Choose No if the test email was not received or if the email did not look appropriate. No patient emails will be sent.";
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
				return;
			}
			//Now send out the mass email to all of the recipients.
			progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
				response=Promotions.SendEmails(template,
					_listPatientsSelected.Select(x => new MassEmailDestination { PatNum=x.PatNum,AptNum=x.NextAptNum,ToAddress=x.Email}).ToList(),
					alias,replyToAddress.Address,promotionName,PromotionType.Manual,template.ClinicNum,senderAddress);
			};
			progressOD.StartingMessage=Lan.g(this,"Sending emails...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(isErrorMsg(response)) {
				MsgBox.Show(response);
				return;
			}
			long numberSent=_listPatientsSelected.Count;
			string alteredCountMsg="";
			string templateName=template.TemplateName;
			if(!string.IsNullOrWhiteSpace(response) && long.TryParse(response,out long numRemoved)) {
				numberSent-=numRemoved;
				alteredCountMsg+=$"{Lan.g(this,"Prevented")} {numRemoved} {Lan.g(this,"duplicate email(s) from sending to the same email address")}.\n";
			}
			if(template.ClinicNum==0) {
				Prefs.UpdateString(PrefName.EmailHostingAlias,textboxAlias.Text);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailHostingAlias,template.ClinicNum,textboxAlias.Text);
			}
			message=$"{alteredCountMsg}{Lan.g(this,"Sent to")} {numberSent} {Lan.g(this,"patients with template:")} \"{templateName}\" {Lan.g(this,"for group:")} \"{promotionName}\".";
			MsgBox.Show(this,message);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}