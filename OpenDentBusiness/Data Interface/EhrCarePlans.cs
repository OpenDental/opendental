using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrCarePlans{
		///<summary></summary>
		public static List<EhrCarePlan> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrCarePlan>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrcareplan WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DatePlanned";
			return Crud.EhrCarePlanCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EhrCarePlan ehrCarePlan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrCarePlan.EhrCarePlanNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrCarePlan);
				return ehrCarePlan.EhrCarePlanNum;
			}
			return Crud.EhrCarePlanCrud.Insert(ehrCarePlan);
		}

		///<summary></summary>
		public static void Update(EhrCarePlan ehrCarePlan) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrCarePlan);
				return;
			}
			Crud.EhrCarePlanCrud.Update(ehrCarePlan);
		}

		///<summary></summary>
		public static void Delete(long ehrCarePlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrCarePlanNum);
				return;
			}
			string command= "DELETE FROM ehrcareplan WHERE EhrCarePlanNum = "+POut.Long(ehrCarePlanNum);
			Db.NonQ(command);
		}
		
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one EhrCarePlan from the db.</summary>
		public static EhrCarePlan GetOne(long ehrCarePlanNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrCarePlan>(MethodBase.GetCurrentMethod(),ehrCarePlanNum);
			}
			return Crud.EhrCarePlanCrud.SelectOne(ehrCarePlanNum);
		}

		*/
	}
}