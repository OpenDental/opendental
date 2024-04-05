using CodeBase;
using Newtonsoft.Json;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebApps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEServicesPaymentPortal:FormODBase {
		private ApptReminderRule _apptReminderRule;
		private UserControlReminderMessage _userControlReminderMessage;
		private MsgToPayEmailTemplate _msgToPayEmailTemplate;

		public FormEServicesPaymentPortal() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textPatientFacingPaymentUrl.Text=WebAppUtil.GetWebAppUrl(eServiceCode.PaymentPortalUI,Clinics.ClinicNum);
			if(!PrefC.HasClinicsEnabled) {
				comboClinicPicker.Visible=false;
			}
		}

		private void FormEServicesPaymentPortal_Load(object sender,EventArgs e) {
			labelTags.Text=Lan.g(this,"Use the following replacement tags to customize messages : ")+string.Join(", ",ApptReminderRules.GetAvailableTags(ApptReminderType.PayPortalMsgToPay));
			_apptReminderRule=new ApptReminderRule();
			_apptReminderRule.TemplateSMS=PrefC.GetString(PrefName.PaymentPortalMsgToPayTextMessageTemplate);
			_apptReminderRule.TemplateEmailSubject=PrefC.GetString(PrefName.PaymentPortalMsgToPaySubjectTemplate);
			ODException.SwallowAnyException(() =>
				_msgToPayEmailTemplate=JsonConvert.DeserializeObject<MsgToPayEmailTemplate>(PrefC.GetString(PrefName.PaymentPortalMsgToPayEmailMessageTemplate))
			);
			if(_msgToPayEmailTemplate==null) {
				_msgToPayEmailTemplate=new MsgToPayEmailTemplate();
				_msgToPayEmailTemplate.Template="";
			}
			_apptReminderRule.TemplateEmail=_msgToPayEmailTemplate.Template;
			_apptReminderRule.EmailTemplateType=_msgToPayEmailTemplate.EmailType;
			_userControlReminderMessage=new UserControlReminderMessage(_apptReminderRule,LayoutManager,doCheckDisclaimer:false);
			_userControlReminderMessage.Anchor=System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Left|System.Windows.Forms.AnchorStyles.Right|System.Windows.Forms.AnchorStyles.Bottom;
			_userControlReminderMessage.Dock=DockStyle.Fill;
			_userControlReminderMessage.Location=new Point(LayoutManager.Scale(0),LayoutManager.Scale(0));
			_userControlReminderMessage.Size=new Size(498,315);
			LayoutManager.AddUnscaled(_userControlReminderMessage,groupBoxUserControlFill);
			LayoutManager.MoveSize(_userControlReminderMessage,LayoutManager.ScaleSize(_userControlReminderMessage.Size));
		}

		private void comboClinicPicker_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinicPicker.ClinicNumSelected==-1) {
				return;
			}
			textPatientFacingPaymentUrl.Text=WebAppUtil.GetWebAppUrl(eServiceCode.PaymentPortalUI,comboClinicPicker.ClinicNumSelected);
		}

		///<summary>Validates sms and email templates if they are set. Returns false if template contains any Redirect URLs or if Template is missing the msg to pay tag.</summary>
		private bool IsTemplateValid() {
			List<string> listErrors=new List<string>();
			//Email Template
			if(!string.IsNullOrWhiteSpace(_apptReminderRule.TemplateEmail)) {
				if(!_apptReminderRule.TemplateEmail.Contains(MsgToPaySents.MSG_TO_PAY_TAG)) {
					listErrors.Add(Lan.g(this,"Your Message-to-Pay Email Template must contain")+$" {MsgToPaySents.MSG_TO_PAY_TAG} ");
				}
				string emailErrorText=PrefC.GetFirstShortURL(_apptReminderRule.TemplateEmail);
				if(!string.IsNullOrWhiteSpace(emailErrorText)) {
					listErrors.Add(Lan.g(this,"Email Message Template cannot contain the URL")+" "+emailErrorText+" "+Lan.g(this,"as this is only allowed for eServices."));
				}
			}
			//SMS Template
			if(!string.IsNullOrWhiteSpace(_apptReminderRule.TemplateSMS)) {
				if(!_apptReminderRule.TemplateSMS.Contains(MsgToPaySents.MSG_TO_PAY_TAG)) {
					listErrors.Add(Lan.g(this,"Your Message-to-Pay Text Template must contain")+$" {MsgToPaySents.MSG_TO_PAY_TAG} ");
				}
				string smsErrorText=PrefC.GetFirstShortURL(_apptReminderRule.TemplateSMS);
				if(!string.IsNullOrWhiteSpace(smsErrorText)) {
					listErrors.Add(Lan.g(this,"Text Message Template cannot contain the URL")+" "+smsErrorText+" "+Lan.g(this,"as this is only allowed for eServices."));
				}
			}
			if(!listErrors.IsNullOrEmpty()) {
				MessageBox.Show(Lan.g(this,"Please fix the following errors before continuing:\r\n")+string.Join("\r\n",listErrors));
			}
			return listErrors.IsNullOrEmpty();
		}

		private void butSave_Click(object sender,EventArgs e) {
			//Saving form information to the rule object and then validating
			_userControlReminderMessage.SaveControlTemplates();
			if(!IsTemplateValid()) {
				return;
			}
			bool didUpdate=false;
			//Setting object for json serialization in pref
			_msgToPayEmailTemplate.Template=_apptReminderRule.TemplateEmail;
			_msgToPayEmailTemplate.EmailType=_apptReminderRule.EmailTemplateType;
			string emailMessageTemplate=JsonConvert.SerializeObject(_msgToPayEmailTemplate);
			didUpdate|=Prefs.UpdateString(PrefName.PaymentPortalMsgToPayEmailMessageTemplate,emailMessageTemplate);
			didUpdate|=Prefs.UpdateString(PrefName.PaymentPortalMsgToPaySubjectTemplate,_apptReminderRule.TemplateEmailSubject);
			didUpdate|=Prefs.UpdateString(PrefName.PaymentPortalMsgToPayTextMessageTemplate,_apptReminderRule.TemplateSMS);
			if(didUpdate) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}
	}
}