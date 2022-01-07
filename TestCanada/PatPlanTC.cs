/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace TestCanada {
	public class PatPlanTC {
		///<summary>Ordinal should be 1 for primary, etc.</summary>
		public static InsPlan GetInsPlan(long patNum,int ordinal) {
			PatPlan patplan=PatPlans.GetPatPlan(patNum,ordinal);
			return InsPlans.GetPlan(patplan.PlanNum,new List<InsPlan>());
		}

	}
}*/
