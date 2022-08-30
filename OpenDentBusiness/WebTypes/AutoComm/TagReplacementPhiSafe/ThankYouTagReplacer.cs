using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.AutoComm {
    public class ThankYouTagReplacer : ApptTagReplacer {		
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			ReplaceAddToCalendarTag(sbTemplate,autoCommObj,isEmail);
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

