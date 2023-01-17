using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Flows{
	
		#region Methods - Get
		///<summary></summary>
		public static List<Flow> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Flow>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM flow WHERE PatNum = "+POut.Long(patNum);
			return Crud.FlowCrud.SelectMany(command);
		}

		public static Flow GetCurrentIncompleteFlowForPat(long patNum) {
			if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT) {
				return Meth.GetObject<Flow>(MethodBase.GetCurrentMethod(), patNum);
			}
			string command = 
				$@"SELECT * FROM flow 
				WHERE PatNum = {POut.Long(patNum)} 
				AND SecDateTEntry BETWEEN {POut.DateT(DateTime.Today)} AND {POut.DateT(DateTime.Now)}
				AND !IsComplete
				ORDER BY SecDateTEntry DESC
				LIMIT 1";
			return Crud.FlowCrud.SelectOne(command);
		}

		public static List<Flow> GetAllForClinicInDateRange(long clinicNum, DateTime fromDate, DateTime toDate, bool includeAll) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Flow>>(MethodBase.GetCurrentMethod(),clinicNum, fromDate, toDate);
			}
			string command = $"SELECT * FROM flow WHERE SecDateTEntry BETWEEN {POut.DateT(fromDate)} AND {POut.DateT(toDate)} + INTERVAL 1 DAY {(includeAll ? $"AND ClinicNum = {POut.Long(clinicNum)} " :"")}";
			return Crud.FlowCrud.SelectMany(command);
		}
		
		///<summary>Gets one Flow from the db.</summary>
		public static Flow GetOne(long flowNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<Flow>(MethodBase.GetCurrentMethod(),flowNum);
			}
			return Crud.FlowCrud.SelectOne(flowNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(Flow flow){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				flow.FlowNum=Meth.GetLong(MethodBase.GetCurrentMethod(),flow);
				return flow.FlowNum;
			}
			return Crud.FlowCrud.Insert(flow);
		}
		///<summary></summary>
		public static void Update(Flow flow){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flow);
				return;
			}
			Crud.FlowCrud.Update(flow);
		}
		///<summary></summary>
		public static void Delete(long flowNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowNum);
				return;
			}
			Crud.FlowCrud.Delete(flowNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}