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
	}
}

