using System.Text;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
	public class ApptTagReplacer : TagReplacer {
		public static string GetAddToCalendarTag(long aptNum,bool isRegExPattern = false) {
			string tag=ApptThankYouSents.ADD_TO_CALENDAR.Replace("]",$"({aptNum})]");
			if(isRegExPattern) {
				tag=tag.Replace("[","\\[")
					.Replace("(","\\(")
					.Replace(")","\\)");
			}
			return tag;
		}

		public virtual void ReplaceAddToCalendarTag(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			ApptLite appt=autoCommObj as ApptLite;
			if(appt!=null) {
				//Append AptNum to the [AddToCalendar] tag so we can identify which tag should be replaced at HQ with which appointment details.
				string replaceWith=GetAddToCalendarTag(appt.PrimaryKey);
				ReplaceOneTag(sbTemplate,ApptThankYouSents.ADD_TO_CALENDAR,replaceWith,isEmail);
			}
		}

		///<summary>Replaces appointment related tags.</summary>
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			ApptLite appt=autoCommObj as ApptLite;
			if(appt!=null) {
				ReplaceOneTag(sbTemplate,"[ApptTime]",appt.AptDateTime.ToShortTimeString(),isEmail);
				ReplaceOneTag(sbTemplate,"[ApptDate]",appt.AptDateTime.ToString(PrefC.PatientCommunicationDateFormat),isEmail);
				ReplaceOneTag(sbTemplate,"[ApptTimeAskedArrive]",appt.DateTimeAskedToArrive.ToShortTimeString(),isEmail);
			}
		}

		///<summary>Replaces appointment related tags.</summary>
		protected override void ReplaceTagsAggregateChild(StringBuilder sbTemplate,StringBuilder sbAutoCommObjsAggregate) {
			StringTools.RegReplace(sbTemplate,"\\[Appts]",sbAutoCommObjsAggregate.ToString());//We don't need to escape '<' here.
		}	
	}
}
