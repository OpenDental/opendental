using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.AutoComm {
    public class ThankYouTagReplacer : ApptTagReplacer {		
		
		public static string GetAddToCalendarTag(long aptNum,bool isRegExPattern=false) {
			string tag=ApptThankYouSents.ADD_TO_CALENDAR.Replace("]",$"({aptNum})]");
			if(isRegExPattern) {
				tag=tag.Replace("[","\\[")
					.Replace("(","\\(")
					.Replace(")","\\)");
			}
			return tag;
		}

		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			ApptLite appt=autoCommObj as ApptLite;
			if(appt!=null) {
				//Append AptNum to the [AddToCalendar] tag so we can identify which tag should be replaced at HQ with which appointment details.
				string replaceWith=GetAddToCalendarTag(appt.PrimaryKey);
				ReplaceOneTag(sbTemplate,ApptThankYouSents.ADD_TO_CALENDAR,replaceWith,isEmail);
			}
		}

		///<summary>Thank yous use the appointment scheduled time as the DateTimeEvent, need to explicitly use the AptDateTime.</summary>
		protected override List<AutoCommObj> GetAggregateGrouping(List<AutoCommObj> listAutoCommObjs) {
			List<ApptLite> listApptLites=listAutoCommObjs.OfType<ApptLite>().ToList();
			List<AutoCommObj> listAutoCommObjReturn=new List<AutoCommObj>();
			listAutoCommObjReturn.AddRange(listApptLites.GroupBy(x => new { x.PatNum,x.AptDateTime.Date }).Select(x => x.OrderBy(y => y.AptDateTime).First()));
			return listAutoCommObjReturn;
		}
	}
}

