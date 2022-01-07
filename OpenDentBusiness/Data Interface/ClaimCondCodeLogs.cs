using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class ClaimCondCodeLogs {
		/// <summary>Will be null if this claim has no condition codes.</summary>
		public static ClaimCondCodeLog GetByClaimNum(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ClaimCondCodeLog>(MethodBase.GetCurrentMethod(),claimNum);
			}
			string command="SELECT * FROM claimcondcodelog WHERE ClaimNum="+POut.Long(claimNum);
			return Crud.ClaimCondCodeLogCrud.SelectOne(command);
			//ClaimCondCodeLog claimCondCodeLog = 
			//if(claimCondCodeLog==null){
			//	return new ClaimCondCodeLog();
			//}
			//return claimCondCodeLog;
		}

		public static void Update(ClaimCondCodeLog claimCondCodeLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimCondCodeLog);
				return;
			}
			Crud.ClaimCondCodeLogCrud.Update(claimCondCodeLog);
		}

		public static long Insert(ClaimCondCodeLog claimCondCodeLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				claimCondCodeLog.ClaimCondCodeLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),claimCondCodeLog);
				return claimCondCodeLog.ClaimCondCodeLogNum;
			}
			return Crud.ClaimCondCodeLogCrud.Insert(claimCondCodeLog);
		}
	}
}
