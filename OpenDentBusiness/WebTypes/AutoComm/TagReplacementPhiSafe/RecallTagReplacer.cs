using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness.AutoComm {
	public abstract class RecallTagReplacer : ApptTagReplacer {
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			ReplaceOneTag(sbTemplate,"[DueDate]",autoCommObj.DateTimeEvent.ToString(PrefC.PatientCommunicationDateFormat),isEmail);
		}
	}

	public class RecallSmsTagReplacer : RecallTagReplacer {
		public override string ReplaceTags(string template,AutoCommObj autoCommObj,Clinic clinic,bool isEmailBody) {
			//Recall templates are not pulled from ApptReminderRules->AutoCommRules.
			WebSchedAgg aco=(WebSchedAgg)autoCommObj;
			template=aco.NumReminders switch {
				0 => PrefC.GetString(PrefName.WebSchedMessageText),
				1 => PrefC.GetString(PrefName.WebSchedMessageText2),
				_ => PrefC.GetString(PrefName.WebSchedMessageText3),
			};
			return base.ReplaceTags(template,autoCommObj,clinic,isEmailBody);
		}

		public override string ReplaceTagsAggregate(string templateAll,string templateSingle,Clinic clinicCur,List<AutoCommObj> listAutoCommObjs,
			AutoCommObj primaryAutoCommObj,bool isEmailBody)
		{
			//Recall templates are not pulled from ApptReminderRules->AutoCommRules.
			templateAll=PrefC.GetString(PrefName.WebSchedAggregatedTextMessage);
			return base.ReplaceTags(templateAll,primaryAutoCommObj,clinicCur,isEmailBody);
		}
	}

	public class RecallEmailTagReplacer : RecallTagReplacer {
		public override string ReplaceTags(string template,AutoCommObj autoCommObj,Clinic clinic,bool isEmailBody) {
			//Recall templates are not pulled from ApptReminderRules->AutoCommRules.
			WebSchedAgg aco=(WebSchedAgg)autoCommObj;
			if(isEmailBody) {
				template=aco.NumReminders switch {
					0 => PrefC.GetString(PrefName.WebSchedMessage),
					1 => PrefC.GetString(PrefName.WebSchedMessage2),
					_ => PrefC.GetString(PrefName.WebSchedMessage3),
				};
			}
			else {//Email Subject
				template=aco.NumReminders switch {
					0 => PrefC.GetString(PrefName.WebSchedSubject),
					1 => PrefC.GetString(PrefName.WebSchedSubject2),
					_ => PrefC.GetString(PrefName.WebSchedSubject3),
				};
			}
			return base.ReplaceTags(template,autoCommObj,clinic,isEmailBody);
		}

		public override string ReplaceTagsAggregate(string templateAll,string templateSingle,Clinic clinicCur,List<AutoCommObj> listAutoCommObjs,
			AutoCommObj primaryAutoCommObj,bool isEmailBody)
		{
			//Recall templates are not pulled from ApptReminderRules->AutoCommRules.
			if(isEmailBody) {
				templateAll=PrefC.GetString(PrefName.WebSchedAggregatedEmailBody);
			}
			else {//Subject
				templateAll=PrefC.GetString(PrefName.WebSchedAggregatedEmailSubject);
			}
			return base.ReplaceTags(templateAll,primaryAutoCommObj,clinicCur,isEmailBody);
		}
	}
}
