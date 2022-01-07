using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness{
	///<summary></summary>
	public class APIBillings {

		///<summary></summary>
		public static long Insert(APIBilling aPIBilling) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				aPIBilling.APIBillingNum=Meth.GetLong(MethodBase.GetCurrentMethod(),aPIBilling);
				return aPIBilling.APIBillingNum;
			}
			return Crud.APIBillingCrud.Insert(aPIBilling);
		}

		///<summary>Should only be called if ODHQ.</summary>
		public static List<Procedure> AddFHIRRepeatingChargesHelper(DateTime dateRun,List<RepeatCharge> listRepeatCharges) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod(),dateRun,listRepeatCharges);
			}
			//Get all bills that are due to be posted as of this date.
			string command="SELECT * FROM apibilling "+
				"WHERE DateOfBill <= "+POut.Date(dateRun.Date,true)+" AND DateTimeProceduresPosted = '0001-01-01 00:00:00'";
			List<APIBilling> listBillsDue=Crud.APIBillingCrud.SelectMany(command);
			//This table will all ProcedureCodes which are included in the EServiceCodeLink table.
			command="SELECT * FROM procedurecode "
				+"INNER JOIN eservicecodelink ON procedurecode.CodeNum=eservicecodelink.CodeNum";
			List<ProcedureCode> listProcCodes=Crud.ProcedureCodeCrud.TableToList(DataCore.GetTable(command));
			List<Procedure> retVal=new List<Procedure>();
			foreach(APIBilling apiBilling in listBillsDue) {
				//List of procedures for this billing cycle was serialized to APIBilling.ProceduresJson by AccountMaint thread. Deserialize and post them.
				List<Procedure> listProcs=JsonConvert.DeserializeObject<List<Procedure>>(apiBilling.ProceduresJson);
				foreach(Procedure proc in listProcs) {
					Procedures.Insert(proc);
					retVal.Add(proc);
					RepeatCharge repeatCharge=listRepeatCharges.Where(x => x.PatNum==proc.PatNum)
						.FirstOrDefault(x => x.ProcCode==ProcedureCodes.GetStringProcCode(proc.CodeNum,listProcCodes));
					if(repeatCharge!=null) {
						RepeatCharges.AllocateUnearned(repeatCharge,proc,dateRun);
					}
				}
				apiBilling.DateTimeProceduresPosted=DateTime_.Now;
				Crud.APIBillingCrud.Update(apiBilling);
			}
			return retVal;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<APIBilling> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<APIBilling>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM apibilling WHERE PatNum = "+POut.Long(patNum);
			return Crud.APIBillingCrud.SelectMany(command);
		}
		
		///<summary>Gets one APIBilling from the db.</summary>
		public static APIBilling GetOne(long aPIBillingNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<APIBilling>(MethodBase.GetCurrentMethod(),aPIBillingNum);
			}
			return Crud.APIBillingCrud.SelectOne(aPIBillingNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Update(APIBilling aPIBilling){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aPIBilling);
				return;
			}
			Crud.APIBillingCrud.Update(aPIBilling);
		}
		///<summary></summary>
		public static void Delete(long aPIBillingNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aPIBillingNum);
				return;
			}
			Crud.APIBillingCrud.Delete(aPIBillingNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/



	}
}