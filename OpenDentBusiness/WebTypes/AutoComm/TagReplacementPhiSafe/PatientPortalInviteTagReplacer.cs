using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
    public class PatientPortalInviteTagReplacer : ApptTagReplacer {		
			
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);//To replace [ApptDate], etc.
			PatPortalInviteLite invite=(PatPortalInviteLite)autoCommObj;
			ReplaceOneTag(sbTemplate,"[UserName]",invite.UserWeb.Item1.UserName,isEmail);
			ReplaceOneTag(sbTemplate,"[Password]",invite.UserWeb.Item2,isEmail);
			ReplaceOneTag(sbTemplate,"[PatientPortalURL]",PrefC.GetString(PrefName.PatientPortalURL),isEmail);
		}
			
		protected override void ReplaceTagsAggregateChild(StringBuilder sbTemplate,StringBuilder sbAutoCommObjsAggregate) {
			StringTools.RegReplace(sbTemplate,"\\[Credentials]",sbAutoCommObjsAggregate.ToString());//We don't need to escape '<' here.
		}

		///<summary>In PatientPortalInvites, we only want to display one set of credentials per person.</summary>
		protected override List<AutoCommObj> GetAggregateGrouping(List<AutoCommObj> listAutoCommObjs) {
			return listAutoCommObjs.GroupBy(x => x.PatNum).Select(x => x.OrderBy(y => y.DateTimeEvent).First()).ToList();
		}
	}
}
