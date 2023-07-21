using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ResellerT	{
		private static Random _rand=new Random();
		///<summary>Create a reseller and it's corresponding Patient in customers db. Give new reseller permission to sell all available eServices.
		///Does not create any customers belonging to new reseller.</summary>
		public static Tuple<Reseller,Patient> CreateReseller() {
			return DataAction.GetCustomers(() => {
				Patient pat=PatientT.CreatePatient(billingCycleDay: _rand.Next(1,31));
				Patient patOld=pat.Copy();
				pat.SuperFamily=pat.PatNum;
				pat.FName="Reseller_F"+pat.PatNum;
				pat.LName="Reseller_L"+pat.PatNum;
				Patients.Update(pat,patOld);
				Reseller reseller=new Reseller {
					PatNum=pat.PatNum,
					UserName="Reseller_"+pat.PatNum.ToString(),
				};
				Resellers.Insert(reseller);
				Authentication.UpdatePasswordReseller(reseller,"Password1");
				ProcedureCodes.RefreshCache();
				List<ProcedureCode> procCodes=ProcedureCodes.GetWhere(x => ProcedureCodes.GetAllEServiceProcCodes().Contains(x.ProcCode));
				//Create all customers.ResellerService rows for this new reseller.
				foreach(ProcedureCode procCode in procCodes){
					OpenDentBusiness.Crud.ResellerServiceCrud.Insert(new ResellerService() {
						ResellerNum=reseller.ResellerNum,
						//Every eService should have a corresponding ProcCode. First() will fail here if not.
						CodeNum=procCode.CodeNum,
						Fee=5,
					});
				}
				return new Tuple<Reseller,Patient>(reseller,pat);
			});
		}

		public static void SetResellerServiceFeeForEService(long resellerNum,eServiceCode eServiceCode,double amount) {
			DataAction.RunCustomers(() => {
				ProcedureCodes.RefreshCache();
				string procCode=ProcedureCodes.GetProcCodeForEService(eServiceCode);
				ProcedureCode procedureCode=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode==procCode);
				ResellerService resellerService=ResellerServices.GetServicesForReseller(resellerNum).FirstOrDefault(x => x.CodeNum==procedureCode.CodeNum);
				if(resellerService!=null) {
					resellerService.Fee=amount;
					ResellerServices.Update(resellerService);
					return;
				}
				resellerService=new ResellerService() {
					ResellerNum=resellerNum,
					CodeNum=procedureCode.CodeNum,
					Fee=amount,
				};
				ResellerServices.Insert(resellerService);
			});
		}
	}
}
