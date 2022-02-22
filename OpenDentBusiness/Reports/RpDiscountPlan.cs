using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection;

namespace OpenDentBusiness {
	public class RpDiscountPlan {
		///<summary>Gets a table of data using low permissions.</summary>
		public static DataTable GetTable(string description) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),description);
			}
			string query="SELECT discountplan.Description, feesched.Description FeeSched, definition.ItemName AdjType,"+
				" discountplansub.DateEffective DateEffective, discountplansub.DateTerm DateTerm,"+
				" CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) PatientName"+
				" FROM discountplan"+
				" INNER JOIN feesched ON feesched.FeeSchedNum=discountplan.FeeSchedNum"+
				" INNER JOIN definition ON definition.DefNum=discountplan.DefNum"+
				" INNER JOIN discountplansub ON discountplansub.DiscountPlanNum=discountplan.DiscountPlanNum"+
				" INNER JOIN patient ON patient.PatNum=discountplansub.PatNum"+
				" WHERE discountplan.Description LIKE '%"+POut.String(description)+"%'"+
				" ORDER BY discountplan.Description,patient.LName,patient.FName,patient.MiddleI";
			return ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(query));
		}
	}
}
