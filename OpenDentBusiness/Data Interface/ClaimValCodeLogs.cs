using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class ClaimValCodeLogs {
		public static double GetValAmountTotal(long claimNum, string valCode){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),claimNum,valCode);
			}
			//double total = 0;
			string command="SELECT SUM(ValAmount) FROM claimvalcodelog WHERE ClaimNum="+POut.Long(claimNum)+" AND ValCode='"+POut.String(valCode)+"'";
			return PIn.Double(Db.GetScalar(command));
			//DataTable table=Db.GetTable(command);
			//for(int i=0;i<table.Rows.Count;i++){
			//	total+=PIn.Double(table.Rows[i][4].ToString());
			//}
			//return total;
		}

		public static List<ClaimValCodeLog> GetForClaim(long claimNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClaimValCodeLog>>(MethodBase.GetCurrentMethod(),claimNum);
			}
			string command="SELECT * FROM claimvalcodelog WHERE ClaimNum="+POut.Long(claimNum);
			return Crud.ClaimValCodeLogCrud.SelectMany(command);
		}

		public static void UpdateList(List<ClaimValCodeLog> listClaimValCodeLogs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClaimValCodeLogs);
				return;
			}
			for(int i=0;i<listClaimValCodeLogs.Count;i++){
				if(listClaimValCodeLogs[i].ClaimValCodeLogNum==0){
					Crud.ClaimValCodeLogCrud.Insert(listClaimValCodeLogs[i]);
				} 
				else {
					Crud.ClaimValCodeLogCrud.Update(listClaimValCodeLogs[i]);
				}
			}
		}
	}
} 