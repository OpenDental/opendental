using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Net.Mail;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormMassEmailSend:FormODBase {
		private const string LOAD_VERIFIED_EMAIL_ADDRESSES="LoadVerifiedEmailAddresses";
		private readonly EmailHostingTemplate _templateCur;
		private List<FormMassEmailList.PatientInfo> _listPatientsSelected;
		///<summary>Patient users wants to view replaced data with. Can be null.</summary>
		private Patient _patSelected;
		private EmailAddress _emailAddressReplyTo;
		private List<IdentityResource> _listVerifiedEmailAddresses;

		public FormMassEmailSend(EmailHostingTemplate template,List<FormMassEmailList.PatientInfo> listPatientSelected) 
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_templateCur=template;
			_listPatientsSelected=listPatientSelected;
		}

		private void FormMassEmailSend_Load(object sender,EventArgs e) {
			SetDefaultReplyToEmailAddress();
			LoadVerifiedEmails(_templateCur.ClinicNum);
			textboxAlias.Text=GetEmailAlias();
			textEmailGroup.Text=_templateCur.TemplateName;
			labelSendingPatients.Text=labelSendingPatients.Text.Replace("###",_listPatientsSelected.Count.ToString());
			textSubject.Text=_templateCur.Subject;
			userControlEmailTemplate1.RefreshView(GetBodyPlainText(),GetBodyHtmlText(),EmailType.RawHtml);
			labelReplacedData.ForeColor=Color.Firebrick;
			labelReplacedData.Text=Lan.g(this,"Without replaced data");
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
			thread.AddExitHandler(o => UpdateSendFromMyAddress());
			thread.Start();
		}

		///<summary>Sets the sender EmailAddress, updates the UI, and shows/hides the checkbox to use the SenderAddress.</summary>
		private void SetEmailAddressSender(EmailAddress emailAddress) {
			_emailAddressReplyTo=emailAddress;
			textSenderAddress.Text=_emailAddressReplyTo?.EmailUsername??"";
			UpdateSendFromMyAddress();
		}

		///<summary>Sets the checkbox visible if the ReplyToAddress is a verified EmailAddress</summary>
		private void UpdateSendFromMyAddress() {
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
					radioReturnAddress.Checked=isVerified && !ClinicPrefs.GetBool(PrefName.EmailHostingUseNoReply,_templateCur.ClinicNum);
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
		private string GetEmailAlias() {
			string emailAlias;
			if(!PrefC.HasClinicsEnabled || _templateCur.ClinicNum==0) {
				emailAlias=PrefC.GetString(PrefName.EmailHostingAlias);
			}
			else {
				emailAlias=ClinicPrefs.GetPrefValue(PrefName.EmailHostingAlias,_templateCur.ClinicNum);
			}
			if(!string.IsNullOrWhiteSpace(emailAlias)) {
				return emailAlias;
			}
			EmailAddress address=EmailAddresses.GetOne(PIn.Long(PrefC.GetString(PrefName.EmailDefaultAddressNum)));
			if(address!=null && !string.IsNullOrWhiteSpace(address.GetFrom())) {
				emailAlias=address.GetFrom();
			}
			if(PrefC.HasClinicsEnabled) {
				Clinic clinic=Clinics.GetClinic(_templateCur.ClinicNum);
				if(clinic!=null && clinic.EmailAddressNum!=0) {
					address=EmailAddresses.GetOne(clinic.EmailAddressNum);
					if(address!=null && !string.IsNullOrWhiteSpace(address.GetFrom())) {
						emailAlias=address.GetFrom();
					}
				}
			}
			return emailAlias;
		}

		private void SetDefaultReplyToEmailAddress() {
			EmailAddress address=null;
			long emailAddressNum;
			Clinic clinic=Clinics.GetClinic(_templateCur.ClinicNum);
			if(clinic!=null) {
				emailAddressNum=clinic.EmailAddressNum;
				address=EmailAddresses.GetOne(emailAddressNum);
			}
			if(address is null) {
				emailAddressNum=PrefC.GetLong(PrefName.EmailDefaultAddressNum);
				address=EmailAddresses.GetOne(emailAddressNum);
			}
			SetEmailAddressSender(address);
		}

		///<summary>Returns the templates BodyPlainText with the signature.</summary>
		private string GetBodyPlainText() {
			string plainTextSignature;
			if(!PrefC.HasClinicsEnabled || _templateCur.ClinicNum==0) {
				plainTextSignature=PrefC.GetString(PrefName.EmailHostingSignaturePlainText);
			}
			else {
				plainTextSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignaturePlainText,_templateCur.ClinicNum);
			}
				return _templateCur.BodyPlainText+"\r\n\r\n"+plainTextSignature;
		}

		///<summary>Returns the templates BodyHtml with the signature.</summary> 
		private string GetBodyHtmlText() {
			string bodyHtmlSignature;
			if(!PrefC.HasClinicsEnabled || _templateCur.ClinicNum==0) {
				bodyHtmlSignature=PrefC.GetString(PrefName.EmailHostingSignatureHtml);
			}
			else {
				bodyHtmlSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignatureHtml,_templateCur.ClinicNum);
			}
			string xhtml=_templateCur.BodyHTML;
			if(_templateCur.EmailTemplateType==EmailType.Html) {
				//This might not work for images, we should consider blocking them or warning them about sending if we detect images
				try {
					xhtml=MarkupEdit.TranslateToXhtml(_templateCur.BodyHTML,true,false,true);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			int bodyTagIndex=xhtml.IndexOf("</body>");
			//Couldn't find it.
			if(bodyTagIndex < 0) {
				xhtml=xhtml+"<br/><br/>"+bodyHtmlSignature;
			}
			else {
				xhtml=xhtml.Insert(bodyTagIndex,"<br/><br/>"+bodyHtmlSignature);
			}
			return xhtml;
		}

		private void checkDisplay_Click(object sender,EventArgs e) {
			if(_patSelected==null) {
				MsgBox.Show("Please select a patient to use as an example for replacement data.");
				checkDisplay.Checked=false;
				return;
			}
			if(checkDisplay.Checked) {
				labelReplacedData.ForeColor=Color.LimeGreen;
				labelReplacedData.Text=Lan.g(this,"With replaced data");
				FormMassEmailList.PatientInfo pat=_listPatientsSelected.First(x => x.PatNum==_patSelected.PatNum);
				Patient guarantor=Patients.GetPat(_patSelected.Guarantor);
				Appointment apt=Appointments.GetOneApt(pat.NextAptNum);
				Clinic clinicPat=Clinics.GetClinic(pat.ClinicNum);
				//Refresh view with the newly replaced data
				string replacedSubject=_templateCur.Subject;
				string replacedPlainText=GetBodyPlainText();
				string replacedHtmlText=GetBodyHtmlText();
				List<string> listSubjectReplacements=EmailHostingTemplates.GetListReplacements(_templateCur.Subject).Distinct().ToList();
				List<string> listBodyHtmlReplacements=EmailHostingTemplates.GetListReplacements(GetBodyHtmlText()).Distinct().ToList();
				List<string> listBodyPlainReplacements=listBodyHtmlReplacements;
				SerializableDictionary<string,string> subjectReplacements=new SerializableDictionary<string,string>();
				string replaceTag(string text,string tag,string value) {
					return Regex.Replace(text,$@"\[{{\[{{\s?{tag}\s?}}\]}}\]",value);
				}
				foreach(string replacement in listSubjectReplacements) {
					subjectReplacements[replacement]=GetReplacementValue(replacement,guarantor,apt,clinicPat);
					replacedSubject=replaceTag(replacedSubject,replacement,subjectReplacements[replacement]);
				}
				SerializableDictionary<string,string> bodyHtmlReplacements=new SerializableDictionary<string,string>();
				foreach(string replacement in listBodyHtmlReplacements) {
					bodyHtmlReplacements[replacement]=GetReplacementValue(replacement,guarantor,apt,clinicPat);
					replacedHtmlText=replaceTag(replacedHtmlText,replacement,bodyHtmlReplacements[replacement]);
				}
				SerializableDictionary<string,string> bodyPlainReplacements=new SerializableDictionary<string,string>();
				foreach(string replacement in listBodyPlainReplacements) {
					bodyPlainReplacements[replacement]=GetReplacementValue(replacement,guarantor,apt,clinicPat);
					replacedPlainText=replaceTag(replacedPlainText,replacement,bodyPlainReplacements[replacement]);
				}
				userControlEmailTemplate1.RefreshView(replacedPlainText,replacedHtmlText,EmailType.RawHtml);
				textSubject.Text=replacedSubject;
			}
			else {
				labelReplacedData.ForeColor=Color.Firebrick;
				labelReplacedData.Text=Lan.g(this,"Without replaced data");
				//Refresh view with the original un-replaced values. 
				userControlEmailTemplate1.RefreshView(GetBodyPlainText(),GetBodyHtmlText(),EmailType.RawHtml);
				textSubject.Text=_templateCur.Subject;
			}
		}

		private string GetReplacementValue(string replacementKey,Patient guarantor,Appointment apt,Clinic clinicPat) {
			string bracketReplacement="["+replacementKey+"]";
			string result=Patients.ReplacePatient(bracketReplacement,_patSelected);
			if(result==bracketReplacement) {
				result=Patients.ReplaceGuarantor(bracketReplacement,guarantor);
			}
			if(result==bracketReplacement) {
				result=ReplaceTags.ReplaceMisc(bracketReplacement);
			}
			if(result==bracketReplacement) {
				result=ReplaceTags.ReplaceUser(bracketReplacement,Security.CurUser);
			}
			if(result==bracketReplacement) {
				result=Appointments.ReplaceAppointment(bracketReplacement,apt);
			}
			if(result==bracketReplacement) {
				result=Clinics.ReplaceOffice(bracketReplacement,clinicPat);
			}
			return result;
		}

		private void butPatientSelect_Click(object sender,EventArgs e) {
			//Build our grid to display for the form we are about to show the user. 
			List<UI.GridColumn> listColumns=new List<UI.GridColumn>();
			listColumns.Add(new UI.GridColumn(Lan.g(this,"Patient"),0));
			List<UI.GridRow> listRows=new List<UI.GridRow>();
			foreach(FormMassEmailList.PatientInfo patient in _listPatientsSelected) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(patient.Name);
				row.Tag=patient;
				listRows.Add(row);
			}
			using FormGridSelection formGridSelection=new FormGridSelection(listColumns,listRows,Lan.g(this,"Select Patient"),Lan.g(this,"Select Patient"));
			if(formGridSelection.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FormMassEmailList.PatientInfo selectedPat=(FormMassEmailList.PatientInfo)formGridSelection.ListSelectedTags.FirstOrDefault();
			_patSelected=Patients.GetPat(selectedPat.PatNum);
			if(_patSelected!=null) {
				textPatient.Text=_patSelected.LName+", "+_patSelected.FName;
			}
		}

		private void butSelectSender_Click(object sender,EventArgs e) {
			using FormEmailAddresses formEmailAddresses=new FormEmailAddresses();
			formEmailAddresses.IsSelectionMode=true;
			DialogResult result=formEmailAddresses.ShowDialog();
			if(result!=DialogResult.OK || formEmailAddresses.EmailAddressNum==0) {
				return;
			}
			EmailAddress emailAddress=EmailAddresses.GetOne(formEmailAddresses.EmailAddressNum);
			SetEmailAddressSender(emailAddress);
		}

		private void butVerifications_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			using FormEmailHostingAddressVerification formVerification=new FormEmailHostingAddressVerification();
			formVerification.ShowDialog();
			//This form ran web calls to get the most up-to-date verified addresses.  The user has to click a link in the verification email to finish the
			//process for any new verifications to be "successful".
			_listVerifiedEmailAddresses=formVerification.GetVerifiedAddresses(_templateCur.ClinicNum);
			UpdateSendFromMyAddress();
		}

		private void butSendEmails_Click(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(textboxAlias.Text)) {
				MsgBox.Show(this,labelEmailAlias.Text+" cannot be blank.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textEmailGroup.Text)) {
				MsgBox.Show(this,"Mass email group name cannot be blank.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textSenderAddress.Text)) {
				MsgBox.Show(this,"Return email address cannot be blank.");
				return;
			}
			string promotionName=textEmailGroup.Text;
			string htmlSignature;
			string plainTextSignature;
			if(!PrefC.HasClinicsEnabled || _templateCur.ClinicNum==0) {
				htmlSignature=PrefC.GetString(PrefName.EmailHostingSignatureHtml);
				plainTextSignature=PrefC.GetString(PrefName.EmailHostingSignaturePlainText);
			}
			else {
				htmlSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignatureHtml,_templateCur.ClinicNum);
				plainTextSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignaturePlainText,_templateCur.ClinicNum);
			}
			if(string.IsNullOrWhiteSpace(htmlSignature) || string.IsNullOrWhiteSpace(plainTextSignature)) {
				MsgBox.Show(this,"Email signatures cannot be blank. Please set in the eServices setup window.");
				return;
			}
			string message=Lan.g(this,"Are you sure you want to send this email to ")+_listPatientsSelected.Count+Lan.g(this," patients?");
			if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.No) {
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
			if(string.IsNullOrWhiteSpace(promotionName)) {
				return;
			}
			//Send to self first to confirm that everything looks apropriate.
			string error=null;
			FormMassEmailList.PatientInfo patientInfo=_listPatientsSelected[0];
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
			progressOD.ActionMain=() => {
				error=Promotions.SendEmails(_templateCur,
					listDestinations,//This is just one email, the sender.
					alias,replyToAddress.Address,promotionName,PromotionType.Manual,_templateCur.ClinicNum,senderAddress,isVerificationBatch:true);
			};
			progressOD.StartingMessage=Lan.g(this,"Sending verification email...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(!string.IsNullOrWhiteSpace(error)) {
				MsgBox.Show(error);
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
				error=Promotions.SendEmails(_templateCur,
					_listPatientsSelected.Select(x => new MassEmailDestination { PatNum=x.PatNum,AptNum=x.NextAptNum,ToAddress=x.Email}).ToList(),
					alias,replyToAddress.Address,promotionName,PromotionType.Manual,_templateCur.ClinicNum,senderAddress);
			};
			progressOD.StartingMessage=Lan.g(this,"Sending emails...");
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(!string.IsNullOrWhiteSpace(error)) {
				MsgBox.Show(error);
				return;
			}
			long numberSent=_listPatientsSelected.Count;
			string templateName=_templateCur.TemplateName;
			if(!PrefC.HasClinicsEnabled || _templateCur.ClinicNum==0) {
				Prefs.UpdateString(PrefName.EmailHostingAlias,textboxAlias.Text);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailHostingAlias,_templateCur.ClinicNum,textboxAlias.Text);
			}
			message=$"{Lan.g(this,"Sent to")} {numberSent} {Lan.g(this,"patients with template:")} \"{templateName}\" {Lan.g(this,"for group:")} \"{promotionName}\".";
			MsgBox.Show(this,message);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}