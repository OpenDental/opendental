using OpenDentBusiness.Crud;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness.AutoComm {
	public abstract class RecallTagReplacer : ApptTagReplacer {
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			ReplaceOneTag(sbTemplate,"[DueDate]",autoCommObj.DateTimeEvent.ToString(PrefC.PatientCommunicationDateFormat),isEmail);
		}

		///<summary>Recall aggregate is treated specially. 
		///The only aggregate tag supported is FamilyList. The patient template is fixed as simply NameF per each patient.</summary>
		protected string ReplaceTagsAggregateRecall(string templateAll,Clinic clinicCur,List<AutoCommObj> listAutoCommObjs,AutoCommObj primaryAutoCommObj,bool isEmailBody) {
			if(string.IsNullOrEmpty(templateAll)) {
				return templateAll;
			}
			//Replace shared tags of agg template.
			StringBuilder ret=new StringBuilder(base.ReplaceTags(templateAll,primaryAutoCommObj,clinicCur,isEmailBody));
			//Check for agg replacement template.
			if(!ret.ToString().ToLower().Contains("[FamilyList]".ToLower())) {
				return ret.ToString();
			}
			//Replace each aco's tag with FName.
			string familyList="";
			List<AutoCommObj> listAutoCommObjsOrdered=GetAggregateGrouping(listAutoCommObjs);
			//If there are multiple appointments for the same patient in this group, we are going to use just the first appointment to fill out the template.
			foreach(AutoCommObj autoCommObj in listAutoCommObjsOrdered) {
				//Each patient uses fixed tag: NameF. If we want more tags per patient, put them here.
				if(!string.IsNullOrEmpty(familyList)) {
					familyList+="\r\n";
				}
				familyList=familyList+base.ReplaceTags("[NameF]",autoCommObj,clinicCur,isEmailBody);
				//12/14/23 Customer has requested that we also include Date RecallDue in each family member line.
				//After much delibration, Nathan/Mark/Sam decided that this datapoint is useless to most customers and won't be included.
				//This can be added as a feature request as a later time. Would probably need a pref associated to turn feature on/off.
				//Uncomment following line to make this work.
				//familyList=$"{familyList} {autoCommObj.DateTimeEvent.ToString(PrefC.PatientCommunicationDateFormat)}";
			}
			//All that is left if the FamilyList tag. Replace it with the familyList we just created.
			ReplaceOneTag(ret,"[FamilyList]",familyList,isEmailBody);
			return ret.ToString();
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
			return ReplaceTagsAggregateRecall(PrefC.GetString(PrefName.WebSchedAggregatedTextMessage),clinicCur,listAutoCommObjs,primaryAutoCommObj,false);
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
			return ReplaceTagsAggregateRecall(
				isEmailBody ? PrefC.GetString(PrefName.WebSchedAggregatedEmailBody) : PrefC.GetString(PrefName.WebSchedAggregatedEmailSubject),
				clinicCur,listAutoCommObjs,primaryAutoCommObj,isEmailBody);
		}
	}
}
