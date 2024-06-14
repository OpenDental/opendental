using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlReminderMessage:UserControl {
		public LayoutManagerForms LayoutManager;
		private string _templateEmail;
		private bool _isLoading;
		///<summary>Set in constructor. If the boolean is not passed in this will default to true</summary>
		private bool _doCheckDisclaimer;

		public UserControlReminderMessage(ApptReminderRule apptReminder,LayoutManagerForms layoutManager,bool doCheckDisclaimer=true) {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			Rule=apptReminder;
			LayoutManager=layoutManager;
			LoadControl();
			_doCheckDisclaimer=doCheckDisclaimer;
		}

		public UserControlReminderMessage() {//just so the designer can load.
			InitializeComponent();
		}

		public string TemplateSms { get {return textTemplateSms.Text;} }

		public ApptReminderRule Rule { get; }

		private void butEditEmail_Click(object sender,EventArgs e) {
			using FormEmailEdit formEE=new FormEmailEdit();
			formEE.MarkupText=_templateEmail;
			formEE.IsRawAllowed=true;
			formEE.IsRaw=Rule.EmailTemplateType==EmailType.RawHtml;
			formEE.DoCheckForDisclaimer=_doCheckDisclaimer;
			formEE.ShowDialog();
			if(formEE.DialogResult!=DialogResult.OK) {
				return;
			}
			Rule.EmailTemplateType=formEE.IsRaw?EmailType.RawHtml:EmailType.Html;
			_templateEmail=formEE.MarkupText;
			RefreshEmail();
		}

		private void browserEmailBody_Navigating(object sender,WebBrowserNavigatingEventArgs e) {
			if(_isLoading) {
				return;
			}
			e.Cancel=true;//Cancel browser navigation (for links clicked within the email message).
			if(e.Url.AbsoluteUri=="about:blank") {
				return;
			}
			//if user did not specify a valid url beginning with http:// then the event args would make the url start with "about:"
			//ex: about:www.google.com and then would ask the user to get a separate app to open the link since it is unrecognized
			string url=e.Url.ToString();
			if(url.StartsWith("about")) {
				url=url.Replace("about:","http://");
			}
			Process.Start(url);//Instead launch the URL into a new default browser window.
		}

		private void browserEmailBody_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			_isLoading=false;
		}

		private void LoadControl() {
			textTemplateSms.Text=Rule.TemplateSMS;
			textTemplateSubject.Text=Rule.TemplateEmailSubject;
			_templateEmail=Rule.TemplateEmail;
			if(Rule.TypeCur==ApptReminderType.PatientPortalInvite) {
				textTemplateSms.Enabled=false;
			}
			if(Rule.TypeCur==ApptReminderType.Arrival) {
				butEditEmail.Visible=false;
				labelEmail.Visible=false;
				textTemplateSubject.Visible=false;
				browserEmailBody.Visible=false;
			}
			RefreshEmail();
		}

		private void RefreshEmail() {
			_isLoading=true;
			//There's no easy way to scale the text in the web browser preview to match zoom.  In other places, like wiki, we have altered font size within document.
			if(Rule.EmailTemplateType==EmailType.RawHtml) {
				//browserEmailBody.Document.Body.Style = "font-size:"+LayoutManager.ScaleF(12).ToString("f1")+";";
				browserEmailBody.DocumentText=_templateEmail;
				return;//text is already in HTML, it does not need to be translated.
			}
			try {
				string text=MarkupEdit.TranslateToXhtml(_templateEmail,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true,scale:LayoutManager.ScaleMyFont());
				//browserEmailBody.Document.Body.Style = "font-size:"+LayoutManager.ScaleF(12).ToString("f1")+";";
				browserEmailBody.DocumentText=text;
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		public List<string> ValidateTemplates() {
			List<string> listErrors=new List<string>();
			if(Rule.TypeCur==ApptReminderType.GeneralMessage) {
				string sms="";
				string email="";
				if(Rule.TemplateSMSAggShared.Contains("[Appts]") && string.IsNullOrWhiteSpace(Rule.TemplateSMSAggPerAppt)) {
					sms="SMS";
				}
				if(Rule.TemplateEmailAggShared.Contains("[Appts]") && string.IsNullOrWhiteSpace(Rule.TemplateEmailAggPerAppt)) {
					email="Email";
				}
				if (sms!="" || email!="") {
					string language=MiscUtils.GetCultureFromThreeLetter(Rule.Language)?.EnglishName??Rule.Language;
					if (language.IsNullOrEmpty()) {
						language="Default";
					}
					string and="";
					string tab=Lan.g(this,"tab");
					if(sms!="" && email!="") {
						and=" "+Lan.g(this,"and")+" ";
						tab=Lan.g(this,"tabs");
					}
					string errorMessage=Lan.g(this,"The [Appts] tag template has not been configured for ")+sms+and+email+Lan.g(this," messages for ")+language+". "+Lan.g(this,"Go to the ")+language+Lan.g(this," tab")+", "+Lan.g(this,"click on the Advanced button")+", "+Lan.g(this,"and set up Aggregated ")+sms+and+email+Lan.g(this," Template Per Appointment in the ")+sms+and+email+Lan.g(this," Templates ")+tab+".";
					listErrors.Add(errorMessage);
				}
			}
			if(Rule.TypeCur==ApptReminderType.Arrival) {
				if(!textTemplateSms.Text.ToLower().Contains(OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG.ToLower())) {
					listErrors.Add(Lan.g(this,$"Arrival texts must contain the \"{OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG}\" tag."));
				}
			}
			if(!(Rule.TypeCur.In(ApptReminderType.PatientPortalInvite,ApptReminderType.Birthday))) {
				if(string.IsNullOrWhiteSpace(textTemplateSms.Text)) {
					listErrors.Add(Lan.g(this,"Text message cannot be blank."));
				}
			}
			else {
				if(Rule.TypeCur!=ApptReminderType.PatientPortalInvite) {
					if(string.IsNullOrWhiteSpace(textTemplateSms.Text)) {
						listErrors.Add(Lan.g(this,"Text message cannot be blank."));
					}
				}
				if(string.IsNullOrWhiteSpace(textTemplateSubject.Text)) {
					listErrors.Add(Lan.g(this,"Email subject cannot be blank."));
				}
				if(string.IsNullOrWhiteSpace(_templateEmail)) {
					listErrors.Add(Lan.g(this,"Email message cannot be blank."));
				}
				if(PrefC.GetBool(PrefName.EmailDisclaimerIsOn) && !_templateEmail.ToLower().Contains("[emaildisclaimer]")) {
					listErrors.Add(Lan.g(this,"Email must contain the \"[EmailDisclaimer]\" tag."));
				}
			}
			if(Rule.TypeCur==ApptReminderType.ConfirmationFutureDay) {
				if(!textTemplateSms.Text.ToLower().Contains("[confirmcode]")) {
					listErrors.Add(Lan.g(this,"Confirmation texts must contain the \"[ConfirmCode]\" tag."));
				}
				if(_templateEmail.ToLower().Contains("[confirmcode]")) {
					listErrors.Add(Lan.g(this,"Confirmation emails should not contain the \"[ConfirmCode]\" tag."));
				}
				if(!_templateEmail.ToLower().Contains("[confirmurl]")) {
					listErrors.Add(Lan.g(this,"Confirmation emails must contain the \"[ConfirmURL]\" tag."));
				}
			}
			listErrors.AddRange(AddCalendarTagErrors());
			listErrors.AddRange(NewPatWebFormUrlErrors());
			//new (non-birthday) rule, user may have not remembered to setup the aggregates
			if(Rule.Language!="" && Rule.ApptReminderRuleNum==0 && !Rule.TypeCur.In(ApptReminderType.Birthday,ApptReminderType.GeneralMessage)) {
				string err=Lan.g(this,"Aggregated templates have not been set up for all additional languages. Click on the 'Advanced' button to set up aggregated templates.");
				if(Rule.TypeCur==ApptReminderType.Arrival) {
					if(Rule.TemplateSMSAggShared=="" || Rule.TemplateSMSAggPerAppt=="") { //Arrivals don't include email
						listErrors.Add(err);
					}
				}
				else if(Rule.TypeCur==ApptReminderType.PatientPortalInvite) {
					if(Rule.TemplateEmailAggShared=="" || Rule.TemplateEmailAggPerAppt=="") { //Patient Portal Invites don't include sms
						listErrors.Add(err);
					}
				}
				else {
					if(Rule.TemplateSMSAggShared=="" || Rule.TemplateSMSAggPerAppt=="" || Rule.TemplateEmailAggShared=="" || Rule.TemplateEmailAggPerAppt=="") {
						listErrors.Add(err);
					}
				}
			}
			return listErrors;
		}

		///<summary>Validates the AddToCalendar tag. Adds to the error list if the AddToCalendar tag is present but not signed up for eConfirmations</summary>
		public List<string> AddCalendarTagErrors() {
			List<string> listErrors=new List<string>();
			string addToCalTag=ApptThankYouSents.ADD_TO_CALENDAR.ToLower();
			if(!textTemplateSms.Text.ToLower().Contains(addToCalTag)
				&& !_templateEmail.ToLower().Contains(addToCalTag))
			{
				return listErrors;
			}
			//Only these rule types will have [AddToCalendar] tags.
			//See UserControlReminderAgg.AddCalendarTagErrors() for auto reply validation.
			//[AddToCalendar] tags are allowed tag when eConfirmations are enabled so don't bother validating.
			if(ApptReminderRules.IsAddToCalendarTagSupported(Rule.TypeCur) 
				&& PrefC.GetBool(PrefName.ApptConfirmAutoSignedUp)) 
			{
				return listErrors;
			}
			if(textTemplateSms.Text.ToLower().Contains(addToCalTag)) {
				listErrors.AddRange(ErrorText("texts",Rule.TypeCur));
			}
			if(_templateEmail.ToLower().Contains(addToCalTag)) {
				listErrors.AddRange(ErrorText("emails",Rule.TypeCur));
			}
			return listErrors;
		}

		private List<string> ErrorText(string mode,ApptReminderType reminderType) {
			List<string> listErrors=new List<string>();
			if(!ApptReminderRules.IsAddToCalendarTagSupported(reminderType)) {
				listErrors.Add(Lan.g(this,"AddToCalendar tag can only be used for Reminders, Thank-Yous, and eConfirmations."));
				return listErrors;
			}
			if(reminderType==ApptReminderType.Reminder) {
				if(mode=="texts") {
					listErrors.Add(Lan.g(this,"Automated Reminder texts cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR+Lan.g(this," when not signed up for eConfirmations."));
				}
				else {
					listErrors.Add(Lan.g(this,"Automated Reminder emails cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR+Lan.g(this," when not signed up for eConfirmations."));
				}
			}
			if(reminderType==ApptReminderType.ScheduleThankYou) {
				if(mode=="texts") {
					listErrors.Add(Lan.g(this,"Automated Thank-You texts cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR+Lan.g(this," when not signed up for eConfirmations."));
				}
				else {
					listErrors.Add(Lan.g(this,"Automated Thank-You emails cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR+Lan.g(this," when not signed up for eConfirmations."));
				}
			}
			if(reminderType==ApptReminderType.ConfirmationFutureDay) {
				if(mode=="texts") {
					listErrors.Add(Lan.g(this,"Automated eConfirmation texts cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR+Lan.g(this," when not signed up for eConfirmations."));
				}
				else {
					listErrors.Add(Lan.g(this,"Automated eConfirmation emails cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR+Lan.g(this," when not signed up for eConfirmations."));
				}
			}
			return listErrors;
		}

		public List<string> NewPatWebFormUrlErrors() {
			List<string> listErrors=new List<string>();
			//Only validate if we are NewPatientThankYou
			if(Rule.TypeCur!=ApptReminderType.NewPatientThankYou) {
				return listErrors;
			}
			if(!PrefC.GetBool(PrefName.ApptConfirmAutoSignedUp)) {
				listErrors.Add(Lan.g(this,$"Automated New Patient Thank-Yous cannot be used when not signed up for eConfirmations."));
			}
			if(!textTemplateSms.Text.Contains(ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG)) {
				listErrors.Add(Lan.g(this,"Text Message Template must contain ")+ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG);
			}
			if(!_templateEmail.Contains(ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG)) {
				listErrors.Add(Lan.g(this,"Email Body Template must contain ")+ApptNewPatThankYouSents.NEW_PAT_WEB_FORM_TAG);
			}
			return listErrors;
		}

		public void SaveControlTemplates() {
			if(Rule.TypeCur==ApptReminderType.Birthday) {
				Rule.TemplateSMS="";
			}
			else {
				//Clicking a link with a period will not get recognized. 
				Rule.TemplateSMS=textTemplateSms.Text.Replace("[ConfirmURL].","[ConfirmURL] .");
				Rule.TemplateSMS=textTemplateSms.Text.Replace($"{MsgToPaySents.MSG_TO_PAY_TAG}.",$"{MsgToPaySents.MSG_TO_PAY_TAG} .");
				Rule.TemplateSMS=textTemplateSms.Text.Replace($"{MsgToPaySents.STATEMENT_URL_TAG}.",$"{MsgToPaySents.STATEMENT_URL_TAG} .");
			}
			Rule.TemplateEmailSubject=textTemplateSubject.Text;
			Rule.TemplateEmail=_templateEmail;
		}

	}
}
