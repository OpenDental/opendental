using System.Text;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
	public class ApptTagReplacer : TagReplacer {
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
