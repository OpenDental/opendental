using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness.AutoComm {
	public class ReminderTagReplacer:ApptTagReplacer {
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			string premedTemplate="";
			if(autoCommObj.Premed) {
				premedTemplate=PrefC.GetString(PrefName.ApptReminderPremedTemplate);
			}
			ReplaceOneTag(sbTemplate,"[Premed]",premedTemplate,isEmail);
			ReplaceAddToCalendarTag(sbTemplate,autoCommObj,isEmail);
		}

		///<summary>Groups the AutoCommObjs by patient when replacing tags.</summary>
		public override string ReplaceTagsAggregate(string templateAll,string templateSingle,Clinic clinicCur,List<AutoCommObj> listAutoCommObjs,
			AutoCommObj primaryAutoCommObj,bool isEmailBody) {
			if(listAutoCommObjs.Any(x => x.Premed)) {
				//If any one patient has premed, show the [Premed] tag.
				primaryAutoCommObj.Premed=true;
			}
			return base.ReplaceTagsAggregate(templateAll,templateSingle,clinicCur,listAutoCommObjs,primaryAutoCommObj,isEmailBody);
		}
	}
}

