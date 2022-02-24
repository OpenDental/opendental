using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ErxLogs{
		///<summary></summary>
		public static long Insert(ErxLog erxLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				erxLog.ErxLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),erxLog);
				return erxLog.ErxLogNum;
			}
			return Crud.ErxLogCrud.Insert(erxLog);
		}

		///<summary>Returns the latest ErxLog entry for the specified patient and before the specified dateTimeMax. Can return null.
		///Called from Chart when fetching prescriptions from NewCrop to determine the provider on incoming prescriptions.</summary>
		public static ErxLog GetLatestForPat(long patNum,DateTime dateTimeMax) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ErxLog>(MethodBase.GetCurrentMethod(),patNum,dateTimeMax);
			}
			string command=DbHelper.LimitOrderBy("SELECT * FROM erxlog WHERE PatNum="+POut.Long(patNum)+" AND DateTStamp<"+POut.DateT(dateTimeMax)+" ORDER BY DateTStamp DESC",1);
			List <ErxLog> listErxLog=Crud.ErxLogCrud.SelectMany(command);
			if(listErxLog.Count==0) {
				return null;
			}
			return listErxLog[0];
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ErxLog> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ErxLog>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM erxlog WHERE PatNum = "+POut.Long(patNum);
			return Crud.ErxLogCrud.SelectMany(command);
		}

		///<summary>Gets one ErxLog from the db.</summary>
		public static ErxLog GetOne(long erxLogNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ErxLog>(MethodBase.GetCurrentMethod(),erxLogNum);
			}
			return Crud.ErxLogCrud.SelectOne(erxLogNum);
		}

		///<summary></summary>
		public static void Update(ErxLog erxLog){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),erxLog);
				return;
			}
			Crud.ErxLogCrud.Update(erxLog);
		}

		///<summary></summary>
		public static void Delete(long erxLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),erxLogNum);
				return;
			}
			string command= "DELETE FROM erxlog WHERE ErxLogNum = "+POut.Long(erxLogNum);
			Db.NonQ(command);
		}
		*/
	}
}