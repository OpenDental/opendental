using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ResellerServiceT {
		public static void ClearResellerServiceTable() {
			string command="DELETE FROM resellerservice";
			DataCore.NonQ(command);
		}

		public static void SetResellerServiceFeeForEService(long resellerNum,eServiceCode eServiceCode,double amount,double feeRetail=-1) {
			DataAction.RunCustomers(() => {
				ProcedureCodes.RefreshCache();
				string procCode=ProcedureCodes.GetProcCodeForEService(eServiceCode);
				ProcedureCode procedureCode=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode==procCode);
				ResellerService resellerService=ResellerServices.GetServicesForReseller(resellerNum).FirstOrDefault(x => x.CodeNum==procedureCode.CodeNum);
				if(resellerService!=null) {
					resellerService.Fee=amount;
					resellerService.FeeRetail=feeRetail;
					ResellerServices.Update(resellerService);
					ProcedureCodes.RefreshCache();
					return;
				}
				resellerService=new ResellerService() {
					ResellerNum=resellerNum,
					CodeNum=procedureCode.CodeNum,
					Fee=amount,
					FeeRetail=feeRetail,
				};
				ResellerServices.Insert(resellerService);
				ProcedureCodes.RefreshCache();
			});
		}
	}
}
