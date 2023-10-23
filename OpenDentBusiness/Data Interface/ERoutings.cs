using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ERoutings{
	
		#region Methods - Get
		///<summary></summary>
		public static List<ERouting> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERouting>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command= "SELECT * FROM erouting WHERE PatNum = " + POut.Long(patNum);
			return Crud.ERoutingCrud.SelectMany(command);
		}

		public static ERouting GetCurrentIncompleteERoutingForPat(long patNum) {
			if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT) {
				return Meth.GetObject<ERouting>(MethodBase.GetCurrentMethod(), patNum);
			}
			string command = 
				$@"SELECT * FROM erouting 
				WHERE PatNum = {POut.Long(patNum)} 
				AND SecDateTEntry BETWEEN {POut.DateT(DateTime.Today)} AND {POut.DateT(DateTime.Now)}
				AND !IsComplete
				ORDER BY SecDateTEntry DESC
				LIMIT 1";
			return Crud.ERoutingCrud.SelectOne(command);
		}

		public static List<ERouting> GetAllForClinicInDateRange(long clinicNum, DateTime fromDate, DateTime toDate, bool includeAll) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERouting>>(MethodBase.GetCurrentMethod(),clinicNum, fromDate, toDate,includeAll);
			}
			string command = $"SELECT * FROM erouting WHERE SecDateTEntry BETWEEN {POut.DateT(fromDate)} AND {POut.DateT(toDate)} + INTERVAL 1 DAY {(includeAll ? $"AND ClinicNum = {POut.Long(clinicNum)} " :"")}";
			return Crud.ERoutingCrud.SelectMany(command);
		}
		
		///<summary>Gets one eRouting from the db.</summary>
		public static ERouting GetOne(long eRoutingNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ERouting>(MethodBase.GetCurrentMethod(),eRoutingNum);
			}
			return Crud.ERoutingCrud.SelectOne(eRoutingNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERouting eRouting){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eRouting.ERoutingNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eRouting);
				return eRouting.ERoutingNum;
			}
			return Crud.ERoutingCrud.Insert(eRouting);
		}
		///<summary></summary>
		public static void Update(ERouting eRouting){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRouting);
				return;
			}
			Crud.ERoutingCrud.Update(eRouting);
		}
		///<summary></summary>
		public static void Delete(long eRoutingNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eRoutingNum);
				return;
			}
			Crud.ERoutingCrud.Delete(eRoutingNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}