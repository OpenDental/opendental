using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Dunnings {
		///<summary>Gets a list of all dunnings.  Ordered by BillingType, then by AgeAccount-DaysInAdvance, then by InsIsPending, then by DunningNum.</summary>
		public static List<Dunning> Refresh(List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Dunning>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT * FROM dunning";
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+=" WHERE ClinicNum IN ("+string.Join(",",listClinicNums)+")";
			}
			return Crud.DunningCrud.SelectMany(command)
				.OrderBy(x => x.ClinicNum) //ensures that the highest precedence is Specific Clinics > Unassigned > All Clinics
				.ThenBy(x => x.BillingType)
				.ThenBy(x => x.AgeAccount-x.DaysInAdvance)
				.ThenBy(x => x.InsIsPending)
				.ThenBy(x => x.DunningNum).ToList();//PK allows the retval to be predictable.  Works for random PKs.
		}

		///<summary></summary>
		public static long Insert(Dunning dunning) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				dunning.DunningNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dunning);
				return dunning.DunningNum;
			}
			return Crud.DunningCrud.Insert(dunning);
		}

		///<summary></summary>
		public static void Update(Dunning dunning){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dunning);
				return;
			}
			Crud.DunningCrud.Update(dunning);
		}

		///<summary></summary>
		public static void Delete(Dunning dunning){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dunning);
				return;
			}
			string command="DELETE FROM dunning" 
				+" WHERE DunningNum = "+POut.Long(dunning.DunningNum);
 			Db.NonQ(command);
		}
	}
	


}













