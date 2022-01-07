using System.Text;

namespace OpenDentBusiness.AutoComm {
	public class RecallTagReplacer : ApptTagReplacer {
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			ReplaceOneTag(sbTemplate,"[DueDate]",autoCommObj.DateTimeEvent.ToString(PrefC.PatientCommunicationDateFormat),isEmail);
		}
	}
}
