using System.Collections.Generic;

namespace OpenDentBusiness.AutoComm {
	public class BirthdayTagReplacer : TagReplacer {
		public override string ReplaceTags(string template,AutoCommObj autoCommObj,Clinic clinic,bool isEmailBody) {
			return template;//Promotions.SendEmail() takes care of templating.
		}

		public override string ReplaceTagsAggregate(string templateAll,string templateSingle,Clinic clinicCur,List<AutoCommObj> listAutoCommObjs,AutoCommObj primaryAutoCommObj,bool isEmailBody) {
			return templateAll;//Promotions.SendEmail() takes care of templating.
		}
	}
}
