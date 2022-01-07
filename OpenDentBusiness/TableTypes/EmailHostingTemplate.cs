using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	[Serializable]
	public class EmailHostingTemplate:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailHostingTemplateNum;
		///<summary>Name of the template.</summary>
		public string TemplateName;
		///<summary>Default subject line.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Subject;
		///<summary>Body of the email</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string BodyPlainText;
		///<summary>Body of the email. When email is regular html this will only contain the body text. Will contain full html when email type is RawHtml</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string BodyHTML;
		///<summary>The email hosting template's identifier </summary>
		public long TemplateId;
		///<summary>FK to clinic.ClinicNum </summary>
		public long ClinicNum;
		///<summary>Enum:EmailType The type of email template this is (Regular HTML or Full HTML)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public EmailType EmailTemplateType;
		///<summary>Enum:PromotionType the type of mass email this template is for</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public PromotionType TemplateType;

		///<summary>Returns a copy of this EmailTemplate.</summary>
		public EmailHostingTemplate Copy(){
			return (EmailHostingTemplate)this.MemberwiseClone();
		}

		public string GetPlainTextSignature() {
			return GetSignature(PrefName.EmailHostingSignaturePlainText);
		}

		public string GetHtmlSignature() {
			return GetSignature(PrefName.EmailHostingSignatureHtml);
		}

		private string GetSignature(PrefName pref) {
			string signature;
			if(ClinicNum==0) {
				signature=PrefC.GetString(pref);
			}
			else {
				signature=ClinicPrefs.GetPrefValue(pref,ClinicNum);
			}
			return signature;
		}

		///<summary>Returns the templates BodyPlainText with the signature.</summary>
		public string GetBodyPlainText(PatientInfo patientInfo=null) {
			string plainText=BodyPlainText+"\r\n\r\n"+GetPlainTextSignature();
			long patNum=patientInfo?.PatNum??0;
			long nextAptNum=patientInfo?.NextAptNum??0;
			plainText=PerformReplacements(patNum,nextAptNum,ClinicNum,plainText);
			return plainText;
		}
		

		///<summary>Returns the templates BodyHtml with the signature.</summary> 
		public string GetBodyHtmlText(PatientInfo patientInfo=null) {
			string bodyHtmlSignature=GetHtmlSignature();
			string xhtml=BodyHTML;
			if(EmailTemplateType==EmailType.Html) {
				//This might not work for images, we should consider blocking them or warning them about sending if we detect images
				ODException.SwallowAnyException(() => xhtml=MarkupEdit.TranslateToXhtml(BodyHTML,true,false,true));
			}
			int bodyTagIndex=xhtml.IndexOf("</body>");
			//Couldn't find it.
			if(bodyTagIndex < 0) {
				xhtml=xhtml+"<br/><br/>"+bodyHtmlSignature;
			}
			else {
				xhtml=xhtml.Insert(bodyTagIndex,"<br/><br/>"+bodyHtmlSignature);
			}
			long patNum=patientInfo?.PatNum??0;
			long nextAptNum=patientInfo?.NextAptNum??0;
			xhtml=PerformReplacements(patNum,nextAptNum,ClinicNum,xhtml);
			return xhtml;
		}

		private string PerformReplacements(long patNum,long aptNum,long clinicNum,string text) {
			Family fam=Patients.GetFamily(patNum);
			Patient pat=fam?.GetPatient(patNum);
			Patient guarantor=fam?.Guarantor;
			Appointment apt=Appointments.GetOneApt(aptNum);
			Clinic clinic=Clinics.GetClinic(clinicNum);
			//Refresh view with the newly replaced data
			string replacedSubject=Subject;
			string replacedText=text;
			List<string> listSubjectReplacements=EmailHostingTemplates.GetListReplacements(Subject).Distinct().ToList();
			List<string> listReplacements=EmailHostingTemplates.GetListReplacements(text).Distinct().ToList();
			Dictionary<string,string> subjectReplacements=new Dictionary<string,string>();
			string replaceTag(string text,string tag,string value) {
				return Regex.Replace(text,$@"\[{{\[{{\s?{tag}\s?}}\]}}\]",value);
			}
			foreach(string replacement in listSubjectReplacements) {
				subjectReplacements[replacement]=GetReplacementValue(replacement,pat,guarantor,apt,clinic);
				replacedSubject=replaceTag(replacedSubject,replacement,subjectReplacements[replacement]);
			}
			Dictionary<string,string> dictBodyReplacements=new Dictionary<string,string>();
			foreach(string replacement in listReplacements) {
				dictBodyReplacements[replacement]=GetReplacementValue(replacement,pat,guarantor,apt,clinic);
				replacedText=replaceTag(replacedText,replacement,dictBodyReplacements[replacement]);
			}
			return replacedText;
		}

		private string GetReplacementValue(string replacementKey,Patient pat,Patient guarantor,Appointment apt,Clinic clinic) {
			string bracketReplacement="["+replacementKey+"]";
			string result=Patients.ReplacePatient(bracketReplacement,pat);
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
				result=Clinics.ReplaceOffice(bracketReplacement,clinic);
			}
			return result;
		}
	}
}
