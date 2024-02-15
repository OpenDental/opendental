using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness.AutoComm {
	public class ConfirmationTagReplacer : ApptTagReplacer {
		///<summary>Replaces appointment and confirmation related tags.</summary>
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			base.ReplaceAddToCalendarTag(sbTemplate,autoCommObj,isEmail);
		}
	}
}
