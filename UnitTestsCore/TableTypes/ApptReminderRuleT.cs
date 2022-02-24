using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestsCore {
	public class ApptReminderRuleT {
		public static ApptReminderRule CreateApptReminderRule(long clinicNum,ApptReminderType type,TimeSpan tsPrior,
			bool isSendAll = true,CommType priority1 = CommType.Preferred,CommType priority2 = CommType.Text,CommType priority3 = CommType.Email,
			TimeSpan doNotSendWithin=default(TimeSpan),bool isAutoReplyEnabled=true,string language="") 
		{
			ApptReminderRule clinicRule=ApptReminderRules.CreateDefaultReminderRule(type,clinicNum);
			clinicRule.TSPrior=tsPrior;
			clinicRule.IsSendAll=isSendAll;
			clinicRule.SendOrder=string.Join(",",new List<CommType>() { priority1,priority2,priority3 }.Select(x => ((int)x).ToString()).ToArray());
			clinicRule.DoNotSendWithin=doNotSendWithin;
			clinicRule.Language=language;
			if(type==ApptReminderType.PatientPortalInvite && clinicNum > 0) {
				clinicRule.SendOrder="2";//Email only
				clinicRule.IsSendAll=false;
				if(ClinicPrefs.Upsert(PrefName.PatientPortalInviteEnabled,clinicNum,"1")
					| ClinicPrefs.Upsert(PrefName.PatientPortalInviteUseDefaults,clinicNum,"0")) {
					ClinicPrefs.RefreshCache();
				}
			}
			clinicRule.IsAutoReplyEnabled=isAutoReplyEnabled;
			ApptReminderRules.Insert(clinicRule);
			return clinicRule;
		}

		public static ApptReminderRule CreateBirthdayReminderRule(long clinicNum,TimeSpan tsPrior,string language="",bool isSendToMinor=false
			,int minorAge=14) 
		{
			ApptReminderRule clinicRule=ApptReminderRules.CreateDefaultReminderRule(ApptReminderType.Birthday,clinicNum);
			clinicRule.TSPrior=tsPrior;
			clinicRule.Language=language;
			EmailHostingTemplate template=EmailHostingTemplates.CreateDefaultTemplate(clinicNum,PromotionType.Birthday);
			IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
			CreateTemplateResponse response=api.CreateTemplate(new CreateTemplateRequest() {
				Template=new Template{
						TemplateName=template.TemplateName,
						TemplateBodyHtml=template.BodyHTML,
						TemplateBodyPlainText=template.BodyPlainText,
						TemplateSubject=template.Subject,
					},
			});
			template.TemplateId=response.TemplateNum;
			clinicRule.EmailHostingTemplateNum=EmailHostingTemplates.Insert(template);
			clinicRule.IsSendForMinorsBirthday=isSendToMinor;
			clinicRule.MinorAge=minorAge;
			ApptReminderRules.Insert(clinicRule);
			return clinicRule;
		}

		///<summary>Deletes everything from the apptreminderrule table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearApptReminderRuleTable() {
			string command="DELETE FROM apptreminderrule WHERE ApptReminderRuleNum > 0";
			DataCore.NonQ(command);
		}
	}
}
