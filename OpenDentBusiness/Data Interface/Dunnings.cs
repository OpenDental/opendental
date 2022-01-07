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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Dunning>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT * FROM dunning";
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+=" WHERE ClinicNum IN ("+string.Join(",",listClinicNums)+")";
			}
			return Crud.DunningCrud.SelectMany(command)
				.OrderBy(x => x.BillingType)
				.ThenBy(x => x.AgeAccount-x.DaysInAdvance)
				.ThenBy(x => x.InsIsPending)
				.ThenBy(x => x.DunningNum).ToList();//PK allows the retval to be predictable.  Works for random PKs.
		}

		///<summary></summary>
		public static long Insert(Dunning dun) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				dun.DunningNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dun);
				return dun.DunningNum;
			}
			return Crud.DunningCrud.Insert(dun);
		}

		///<summary></summary>
		public static void Update(Dunning dun){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dun);
				return;
			}
			Crud.DunningCrud.Update(dun);
		}

		///<summary></summary>
		public static void Delete(Dunning dun){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dun);
				return;
			}
			string command="DELETE FROM dunning" 
				+" WHERE DunningNum = "+POut.Long(dun.DunningNum);
 			Db.NonQ(command);
		}
	}
	


}













