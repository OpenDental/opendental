using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class StmtLinks{		
		#region Get Methods
		///<summary>Gets all FKeys for the statement and StmtLinkType passed in.  Returns an empty list if statementNum is invalid or none found.</summary>
		public static List<long> GetForStatementAndType(long statementNum,StmtLinkTypes statementType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),statementNum,statementType);
			}
			string command="SELECT FKey FROM stmtlink "
				+"WHERE StatementNum="+POut.Long(statementNum)+" "
				+"AND StmtLinkType="+POut.Int((int)statementType);
			return Db.GetListLong(command);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(StmtLink stmtLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				stmtLink.StmtLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),stmtLink);
				return stmtLink.StmtLinkNum;
			}
			return Crud.StmtLinkCrud.Insert(stmtLink);
		}

		///<summary>Creates stmtlink entries for the statement and FKs passed in.</summary>
		public static void AttachFKeysToStatement(long stmtNum,List<long> listFKeys,StmtLinkTypes stmtLinkType) {
			//Remoting role check due to looping.  Without this there would be a potential for lots of network traffic.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stmtNum,listFKeys,stmtLinkType);
				return;
			}
			listFKeys.ForEach(x => Insert(new StmtLink() { StatementNum=stmtNum,FKey=x,StmtLinkType=stmtLinkType }));
		}

		public static void AttachPaySplitsToStatement(long stmtNum,List<long> listPaySplitNums) {
			//No need to check RemotingRole; no call to db.
			AttachFKeysToStatement(stmtNum,listPaySplitNums,StmtLinkTypes.PaySplit);
		}

		public static void AttachAdjsToStatement(long stmtNum,List<long> listAdjNums) {
			//No need to check RemotingRole; no call to db.
			AttachFKeysToStatement(stmtNum,listAdjNums,StmtLinkTypes.Adj);
		}

		public static void AttachProcsToStatement(long stmtNum,List<long> listProcNums) {
			//No need to check RemotingRole; no call to db.
			AttachFKeysToStatement(stmtNum,listProcNums,StmtLinkTypes.Proc);
		}

		public static void AttachClaimsToStatement(long stmtNum, List<long> listClaimNums) {
			//No need to check RemotingRole; no call to db.
			AttachFKeysToStatement(stmtNum,listClaimNums,StmtLinkTypes.ClaimPay);
		}
		#endregion

		#region Update
		/*
		///<summary></summary>
		public static void Update(StmtLink stmtLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stmtLink);
				return;
			}
			Crud.StmtLinkCrud.Update(stmtLink);
		}
		*/
		#endregion

		#region Delete
		public static void Delete(long stmtLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stmtLinkNum);
				return;
			}
			Crud.StmtLinkCrud.Delete(stmtLinkNum);
		}

		public static void DetachAllFromStatement(long statementNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum);
				return;
			}
			string command="DELETE FROM stmtlink WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DetachAllFromStatements(List<long> listStatementNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatementNums);
				return;
			}
			if(listStatementNums==null || listStatementNums.Count==0) {
				return;
			}
			string command=DbHelper.WhereIn("DELETE FROM stmtlink WHERE StatementNum IN ({0})",false,listStatementNums.Select(x => POut.Long(x)).ToList());
			Db.NonQ(command);
		}
		#endregion
	}
}