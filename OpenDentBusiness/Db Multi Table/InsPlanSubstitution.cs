using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class InsPlanSubstitution {
		public ProcedureCode ProcCode;
		public SubstitutionLink SubLink;
		public SubstitutionCondition SubCondition;

		public InsPlanSubstitution(ProcedureCode procCode,SubstitutionLink subLink=null) {
			ProcCode=procCode;
			SubLink=subLink;
			if(subLink is null) {
				SubCondition=procCode.SubstOnlyIf;
			}
			else {
				SubCondition=subLink.SubstOnlyIf;
			}
		}

		public static bool AreEqual(InsPlanSubstitution insPlanSub1,InsPlanSubstitution insPlanSub2) {
			return insPlanSub1.ProcCode?.CodeNum==insPlanSub2.ProcCode?.CodeNum
				&& insPlanSub1.SubLink?.SubstitutionCode==insPlanSub2.SubLink?.SubstitutionCode
				&& insPlanSub1.SubCondition==insPlanSub2.SubCondition;
		}

		public static bool HasDuplicates(List<InsPlanSubstitution> listInsPlanSubs) {
			for(int i=0;i<listInsPlanSubs.Count;i++) {
				for(int j=i+1;j<listInsPlanSubs.Count;j++) {
					if(AreEqual(listInsPlanSubs[i],listInsPlanSubs[j])) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
