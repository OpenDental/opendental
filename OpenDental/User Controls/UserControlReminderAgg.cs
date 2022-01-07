using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlReminderAgg:UserControl {
		private string _templateEmailAggShared;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		public UserControlReminderAgg(ApptReminderRule apptReminderDefault) {
			InitializeComponent();
			Rule=apptReminderDefault;
			LoadControls();
		}

		public ApptReminderRule Rule { get; }

		///<summary>These tags are not allowed in confirmation auto reply.</summary>
		private List<string> ListTagsExludedFromAutoReply {
			get {
				return new List<string>() { "[Appts]","[ConfirmCode]","[ConfirmURL]" };
			}
		}

		///<summary>These tags are not allowed in Arrival Response or Come In sms messages.</summary>
		private List<string> ListTagsExludedFromArrivalResponseComeIn => new List<string> { OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG };

		private void LoadControls() {
			textSMSAggShared.Text=PIn.String(Rule.TemplateSMSAggShared);
			textSMSAggPerAppt.Text=PIn.String(Rule.TemplateSMSAggPerAppt);
			textEmailSubjAggShared.Text=PIn.String(Rule.TemplateEmailSubjAggShared);
			_templateEmailAggShared=PIn.String(Rule.TemplateEmailAggShared);
			RefreshEmail();
			textEmailAggPerAppt.Text=PIn.String(Rule.TemplateEmailAggPerAppt);
			labelTags.Text=GetTagsAvailable();
			textSingleAutoReply.Text=PIn.String(Rule.TemplateAutoReply);
			textAggregateAutoReply.Text=PIn.String(Rule.TemplateAutoReplyAgg);
			textArrivalResponse.Text=PIn.String(Rule.TemplateAutoReply);
			textComeIn.Text=PIn.String(Rule.TemplateComeInMessage);
			if(Rule.TypeCur==ApptReminderType.PatientPortalInvite) {
				textSMSAggShared.Enabled=false;	
				textSMSAggPerAppt.Enabled=false;
				labelEmailAggPerAppt.Text+="  "+Lans.g(this,"Replaces the [Credentials] tag.");
			}
			if(Rule.TypeCur!=ApptReminderType.ConfirmationFutureDay) {
				LayoutManager.Remove(tabAutoReplyTemplate);
			}
			if(Rule.TypeCur==ApptReminderType.Arrival) {
				for(int i = tabTemplates.TabPages.Count-1;i>=0;i--) {
					if(!ListTools.In(tabTemplates.TabPages[i],tabSMSTemplate,tabArrivalTemplate)) {
						LayoutManager.Remove(tabTemplates.TabPages[i]);
					}
				}
			}
			else {
				LayoutManager.Remove(tabArrivalTemplate);
			}
		}

		private string GetTagsAvailable() {
			List<string> listTagsAvailable=ApptReminderRules.GetAvailableAggTags(Rule.TypeCur);
			if(tabTemplates.SelectedTab==tabAutoReplyTemplate) {
				listTagsAvailable.RemoveAll(x => ListTools.In(x,ListTagsExludedFromAutoReply));
			}
			else if(tabTemplates.SelectedTab==tabArrivalTemplate) {
				listTagsAvailable.RemoveAll(x => ListTools.In(x,ListTagsExludedFromArrivalResponseComeIn));
			}
			return Lan.g(this,"Use the following replacement tags to customize messages: ")
				 +string.Join(", ",listTagsAvailable);
		}

		private void RefreshEmail() {
			if(Rule.EmailTemplateType==EmailType.RawHtml) {
				browserEmailBody.DocumentText=_templateEmailAggShared;
				return;//text is already in HTML, it does not need to be translated. 
			}
			try {
				string text=MarkupEdit.TranslateToXhtml(_templateEmailAggShared,isPreviewOnly:true,hasWikiPageTitles:false,isEmail:true);
				browserEmailBody.DocumentText=text;
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		private void butEditEmail_Click(object sender,EventArgs e) {
			using FormEmailEdit formEE=new FormEmailEdit();
			formEE.IsRawAllowed=true;
			formEE.MarkupText=_templateEmailAggShared;
			formEE.IsRaw=Rule.AggEmailTemplateType==EmailType.RawHtml;
			formEE.DoCheckForDisclaimer=true;
			formEE.ShowDialog();
			if(formEE.DialogResult!=DialogResult.OK) {
				return;
			}
			Rule.AggEmailTemplateType=formEE.IsRaw?EmailType.RawHtml:EmailType.Html;
			_templateEmailAggShared=formEE.MarkupText;
			RefreshEmail();
		}

		private void tabTemplates_SelectedIndexChanged(object sender,EventArgs e) {
			labelTags.Text=GetTagsAvailable();
			LayoutManager.LayoutControlBoundsAndFonts(tabTemplates);
		}

		public List<string> ValidateTemplates() {
			List<string> errors=new List<string>();
			if(Rule.TypeCur==ApptReminderType.Arrival) {
				if(string.IsNullOrWhiteSpace(textSMSAggShared.Text)) {
					errors.Add(groupBoxSMSAggShared.Text+Lan.g(this," cannot be blank."));
				}
				if(ListTagsExludedFromArrivalResponseComeIn.Any(x => ListTools.In(x.ToLower().Trim(),textArrivalResponse.Text.ToLower()))
					|| ListTagsExludedFromArrivalResponseComeIn.Any(x => ListTools.In(x.ToLower().Trim(),textComeIn.Text.ToLower()))) 
				{
					//Not allowed to use [Arrived] in Arrival Response or ComeIn messages.
					errors.Add(groupArrivedReply.Text+Lan.g(this," and ")+groupComeIn.Text
						+Lan.g(this," cannot contain ")+string.Join(",",ListTagsExludedFromArrivalResponseComeIn));
				}
				if(!textSMSAggShared.Text.ToLower().Contains(OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG.ToLower())) {
					errors.Add(groupBoxSMSAggShared.Text+Lan.g(this,$" must contain the \"{OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG}\" tag."));
				}
				return errors;//Arrival Response and ComeIn templates are allowed to be blank, so we can just return here.
			}
			if(Rule.TypeCur!=ApptReminderType.PatientPortalInvite) {
				if(string.IsNullOrWhiteSpace(textSMSAggShared.Text)) {
					errors.Add(Lan.g(this,"Text message cannot be blank."));
				}
			}
			//Patient portal invites and general messages do not require the [appts] tag.
			if(!ListTools.In(Rule.TypeCur,ApptReminderType.PatientPortalInvite,ApptReminderType.GeneralMessage)) {
				if(!textSMSAggShared.Text.ToLower().Contains("[appts]")) {
					errors.Add(Lan.g(this,"Text message must contain the \"[Appts]\" tag."));
				}
				if(!_templateEmailAggShared.ToLower().Contains("[appts]")) {
					errors.Add(Lan.g(this,"Email message must contain the \"[Appts]\" tag."));
				}
			}
			if(string.IsNullOrWhiteSpace(textEmailSubjAggShared.Text)) {
				errors.Add(Lan.g(this,"Email subject cannot be blank."));
			}
			if(string.IsNullOrWhiteSpace(_templateEmailAggShared)) {
				errors.Add(Lan.g(this,"Email message cannot be blank."));
			}	
			if(Rule.TypeCur==ApptReminderType.ConfirmationFutureDay) {
				if(_templateEmailAggShared.ToLower().Contains("[confirmcode]")) {
					errors.Add(Lan.g(this,"Confirmation emails should not contain the \"[ConfirmCode]\" tag."));
				}
				if(!_templateEmailAggShared.ToLower().Contains("[confirmurl]")) {
					errors.Add(Lan.g(this,"Confirmation emails must contain the \"[ConfirmURL]\" tag."));
				}
				if(string.IsNullOrWhiteSpace(textSingleAutoReply.Text)) {
					errors.Add(Lan.g(this,"Single auto reply text cannot be blank."));
				}
				if(string.IsNullOrWhiteSpace(textAggregateAutoReply.Text)) {
					errors.Add(Lan.g(this,"Aggregate auto reply text cannot be blank."));
				}
				List<string> listInvalidTags=ListTagsExludedFromAutoReply.FindAll(x => textSingleAutoReply.Text.ToLower().Contains(x.ToLower()));
				if(listInvalidTags.Count>0) {
					errors.Add(Lan.g(this,"Single auto reply text contains invalid tags:\r\n"+$"  {string.Join(", ",listInvalidTags)}"));
				}
				listInvalidTags=ListTagsExludedFromAutoReply.FindAll(x => textAggregateAutoReply.Text.ToLower().Contains(x.ToLower()));
				if(listInvalidTags.Count>0) {
					errors.Add(Lan.g(this,"Aggregate auto reply text contains invalid tags:\r\n"+$"  {string.Join(", ",listInvalidTags)}"));
				}
			}
			if(Rule.TypeCur==ApptReminderType.ScheduleThankYou) {
				//ThankYou templates can only use the [AddToCalendar] tag when Confirmations are enabled.
				string addToCalTag=ApptThankYouSents.ADD_TO_CALENDAR.ToLower();
				if(!PrefC.GetBool(PrefName.ApptConfirmAutoSignedUp)) {
					if(textSMSAggPerAppt.Text.ToLower().Contains(addToCalTag)) {
						errors.Add(Lan.g(this,"Automated Thank-You texts cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR
							+Lan.g(this," when not signed up for eConfirmations."));
					}
					if(textEmailAggPerAppt.Text.ToLower().Contains(addToCalTag)) {
						errors.Add(Lan.g(this,"Automated Thank-You emails cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR
							+Lan.g(this," when not signed up for eConfirmations."));
					}
				}
				//Shared templates cannot use [AddToCalendar] tag.
				if(textSMSAggShared.Text.ToLower().Contains(addToCalTag)) {
					errors.Add(Lan.g(this,"Automated Thank-You Aggregated SMS Template cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR
						+Lan.g(this,". Use Per Appointment instead."));
				}
				if(_templateEmailAggShared.ToLower().Contains(addToCalTag)) {
					errors.Add(Lan.g(this,"Automated Thank-You Aggregated E-mail Template cannot contain ")+ApptThankYouSents.ADD_TO_CALENDAR
						+Lan.g(this,". Use Per Appointment instead."));
				}
			}
			if(PrefC.GetBool(PrefName.EmailDisclaimerIsOn) && !_templateEmailAggShared.ToLower().Contains("[emaildisclaimer]")) {
				errors.Add(Lan.g(this,"Email must contain the \"[EmailDisclaimer]\" tag."));
			}
			return errors;

		}

		public void SaveControlTemplates() {
			Rule.TemplateSMSAggShared=textSMSAggShared.Text.Replace("[ConfirmURL].","[ConfirmURL] .");
			Rule.TemplateSMSAggPerAppt=textSMSAggPerAppt.Text;
			Rule.TemplateEmailSubjAggShared=textEmailSubjAggShared.Text;
			Rule.TemplateEmailAggShared=_templateEmailAggShared;
			Rule.TemplateEmailAggPerAppt=textEmailAggPerAppt.Text;
			Rule.TemplateAutoReply=(Rule.TypeCur==ApptReminderType.Arrival) ? textArrivalResponse.Text : textSingleAutoReply.Text;
			Rule.TemplateAutoReplyAgg=textAggregateAutoReply.Text;
			Rule.TemplateComeInMessage=textComeIn.Text;
		}

	}
}
