using OpenDentBusiness.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsBlueBookLogs{
		#region Get Methods
		///<summary>Gets all InsBlueBookLogs for a given ClaimProcNum</summary>
		public static List<InsBlueBookLog> GetAllByClaimProcNum(long claimProcNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsBlueBookLog>>(MethodBase.GetCurrentMethod(),claimProcNum);
			}
			string command="SELECT * FROM insbluebooklog WHERE ClaimProcNum = "+POut.Long(claimProcNum);
			return InsBlueBookLogCrud.SelectMany(command);
		}

		///<summary>Gets the most recent InsBlueBookLog for a ClaimProc. Returns null if ClaimProc has none.</summary>
		public static InsBlueBookLog GetMostRecentForClaimProc(long claimProcNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsBlueBookLog>(MethodBase.GetCurrentMethod(),claimProcNum);
			}
			string command=$@"
				SELECT * FROM insbluebooklog
				WHERE ClaimProcNum={POut.Long(claimProcNum)}
				ORDER BY insbluebooklog.DateTEntry DESC
				LIMIT 1";
			return InsBlueBookLogCrud.SelectOne(command);
		}
		#endregion Get Methods

		#region Modification Methods
		///<summary>Inserts an InsBlueBookLog into the DB and returns the InsBlueBookLogNum.</summary>
		public static long Insert(InsBlueBookLog insBlueBookLog){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				insBlueBookLog.InsBlueBookLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),insBlueBookLog);
				return insBlueBookLog.InsBlueBookLogNum;
			}
			return Crud.InsBlueBookLogCrud.Insert(insBlueBookLog);
		}
		#endregion Modification Methods

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<InsBlueBookLog> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsBlueBookLog>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM insbluebooklog WHERE PatNum = "+POut.Long(patNum);
			return Crud.InsBlueBookLogCrud.SelectMany(command);
		}
		
		///<summary>Gets one InsBlueBookLog from the db.</summary>
		public static InsBlueBookLog GetOne(long insBlueBookLogNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<InsBlueBookLog>(MethodBase.GetCurrentMethod(),insBlueBookLogNum);
			}
			return Crud.InsBlueBookLogCrud.SelectOne(insBlueBookLogNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Update(InsBlueBookLog insBlueBookLog){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insBlueBookLog);
				return;
			}
			Crud.InsBlueBookLogCrud.Update(insBlueBookLog);
		}
		///<summary></summary>
		public static void Delete(long insBlueBookLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insBlueBookLogNum);
				return;
			}
			Crud.InsBlueBookLogCrud.Delete(insBlueBookLogNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/
	}
}