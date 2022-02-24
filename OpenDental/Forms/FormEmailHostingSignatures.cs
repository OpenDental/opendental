using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailHostingSignatures:FormODBase {
		///<summary>Used to switch back to clinics if user selected a different clinic and validation did not pass.</summary>
		private Clinic _clinicPreviouslySelected;
		private bool _isAuthorized;

		//==================== Promotion Variables ====================
		private Clinic _clinicSelected {	
			get {
				if(comboClinicMassEmail.IsUnassignedSelected) {
					return Clinics.GetPracticeAsClinicZero();//Get the default/HQ clinic
				}
				return comboClinicMassEmail.GetSelectedClinic();	
			}	
		}

		public FormEmailHostingSignatures() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailHostingSignatures_Load(object sender,EventArgs e) {
			_isAuthorized=Security.IsAuthorized(Permissions.EServicesSetup);
			_clinicPreviouslySelected=_clinicSelected;
			FillSignature();
		}
		

		private void FillSignature() {
			Clinic clinic=_clinicSelected;
			//We only need EmailHosting (either Mass or Secure) to be able to make the web calls to update signatures.
			bool isClinicActivated=Clinics.IsMassEmailSignedUp(clinic.ClinicNum) || Clinics.IsSecureEmailSignedUp(clinic.ClinicNum);
			labelNotActivated.Visible=!isClinicActivated;
			textboxPlainText.Enabled=_isAuthorized && isClinicActivated;
			butEditSignature.Enabled=_isAuthorized && isClinicActivated;
			string plainTextSignature;
			string htmlSignature;
			//Gets the practice preference if not found for this clinic.
			htmlSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignatureHtml,clinic.ClinicNum);
			plainTextSignature=ClinicPrefs.GetPrefValue(PrefName.EmailHostingSignaturePlainText,clinic.ClinicNum);
			string description=GetSignatureField(clinic.Description);
			string city=GetSignatureField(clinic.City,",");
			string state=GetSignatureField(clinic.State,"");
			string zip=GetSignatureField(clinic.Zip,"");
			string cityStateZip=GetSignatureField($"{city} {state} {zip}");
			string phoneWithFormat=TelephoneNumbers.ReFormat(clinic.Phone);
			string phone="";
			if(!string.IsNullOrWhiteSpace(phoneWithFormat)) {
				phone="PH: "+GetSignatureField(phoneWithFormat,"");
			}
			string signatureGenericPlainText=description+GetSignatureField(clinic.Address)+GetSignatureField(clinic.Address2)+cityStateZip+phone;
			if(!string.IsNullOrWhiteSpace(description)) {
				description=$"<b>{description}</b>";
			}
			if(!string.IsNullOrWhiteSpace(phoneWithFormat)) {
				phone="PH: <i>"+GetSignatureField(phoneWithFormat,"")+"</i>";
			}
			string signatureGenericHtmlText=description+GetSignatureField(clinic.Address)+GetSignatureField(clinic.Address2)+cityStateZip+phone;
			if(string.IsNullOrWhiteSpace(htmlSignature)) {
				htmlSignature=signatureGenericHtmlText.Replace("\r\n","<br/>");
			}
			if(string.IsNullOrWhiteSpace(plainTextSignature)) {
				plainTextSignature=signatureGenericPlainText;
			}
			textboxPlainText.Text=plainTextSignature;
			if(webBrowserSignature.Document==null) {
				webBrowserSignature.DocumentText=htmlSignature;
			}
			else {
				webBrowserSignature.Document.Body.InnerHtml=htmlSignature;
			}
		}

		///<summary>Returns blank string if field is blank. Otherwise, returns field with separator.</summary> 
		private string GetSignatureField(string field,string separator="\r\n") {
			if(string.IsNullOrWhiteSpace(field)) {
				return "";
			}
			return field+separator;
		}

		///<summary>Returns true if saving email signatures was successful. Otherwise, returns false. makes api call.</summary> 
		private bool TrySaveEmailSignatures(long clinicNum) {
			if(!Clinics.IsMassEmailSignedUp(clinicNum) && !Clinics.IsSecureEmailSignedUp(clinicNum)) {
				return true;
			}
			string signaturePlainText=textboxPlainText.Text;
			string htmlSignature=webBrowserSignature.Document.Body.InnerHtml;
			if(string.IsNullOrWhiteSpace(htmlSignature) || string.IsNullOrWhiteSpace(signaturePlainText)) {
				MsgBox.Show(this,"Email signatures cannot be blank.");
				return false;
			}
			IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
			UpdateSignatureResponse updateSignatureResponse=null;
			UpdateSignatureRequest updateSignatureRequest=new UpdateSignatureRequest() { SignatureHtml=htmlSignature,SignaturePlainText=signaturePlainText };
			//Call API to update signature
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => updateSignatureResponse=api.UpdateSignature(updateSignatureRequest);
			progressOD.StartingMessage="Saving email signatures...";
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				FriendlyException.Show("An error occurred while saving the email signatures.",ex);
				return false;
			}
			if(progressOD.IsCancelled) {
				return false;
			}
			//Update preference/clinicpref
			if(clinicNum==0) {
				Prefs.UpdateString(PrefName.EmailHostingSignatureHtml,htmlSignature);
				Prefs.UpdateString(PrefName.EmailHostingSignaturePlainText,signaturePlainText);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			else {
				ClinicPrefs.Upsert(PrefName.EmailHostingSignatureHtml,clinicNum,htmlSignature);
				ClinicPrefs.Upsert(PrefName.EmailHostingSignaturePlainText,clinicNum,signaturePlainText);
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			return true;
		}		

		private void butEditSignature_Click(object sender,EventArgs e) {
			using FormEmailEdit formEmailEdit=new FormEmailEdit();
			formEmailEdit.MarkupText=webBrowserSignature.Document.Body.InnerHtml;
			formEmailEdit.IsEmailSignature=true;
			if(formEmailEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//Email signatures only supports Raw Html. No need to translate the signature through our translator.
			//We need to update the InnerHtml text instead of the DocumentText because we set AllowNavigation to false. 
			//Setting AllowNavigation to false does not allow you to change the DocumentText. The work around is to set the InnerHtml to the updated value.
			webBrowserSignature.Document.Body.InnerHtml=formEmailEdit.HtmlText;
		}

		private void comboClinicMassEmail_SelectionChangeCommitted(object sender,EventArgs e) {if(_clinicSelected.ClinicNum==_clinicPreviouslySelected.ClinicNum) {//didn't change the selected clinic
				return;
			}
			//Don't let them continue if enabled and signatures are not set correctly.
			if(!TrySaveEmailSignatures(_clinicPreviouslySelected.ClinicNum)) {
				//Switch back to previous clinic.
				comboClinicMassEmail.SelectedClinicNum=_clinicPreviouslySelected.ClinicNum;
				return;
			}
			_clinicPreviouslySelected=_clinicSelected;
			FillSignature();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Don't let them continue if enabled and signatures are not set correctly.
			if(!TrySaveEmailSignatures(_clinicPreviouslySelected.ClinicNum)) {
				//Switch back to previous clinic.
				comboClinicMassEmail.SelectedClinicNum=_clinicPreviouslySelected.ClinicNum;
				if(!MsgBox.Show(MsgBoxButtons.YesNo,"Exit without saving?")) {
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}
	}
}