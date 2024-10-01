using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class StmtLinks{		
		#region Get Methods
		///<summary>Gets all FKeys for the statement and StmtLinkType passed in.  Returns an empty list if statementNum is invalid or none found.</summary>
		public static List<long> GetForStatementAndType(long statementNum,StmtLinkTypes stmtLinkTypes) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),statementNum,stmtLinkTypes);
			}
			string command="SELECT FKey FROM stmtlink "
				+"WHERE StatementNum="+POut.Long(statementNum)+" "
				+"AND StmtLinkType="+POut.Int((int)stmtLinkTypes);
			return Db.GetListLong(command);
		}
		#endregion

		///<summary>Gets the a list of StmtLinks with an FKey in the provided list and a matching StmtLinkType</summary>
		public static List<StmtLink> GetForFKeyAndType(List<long> listFKeys,StmtLinkTypes stmtLinkType) {
			if(listFKeys.IsNullOrEmpty()) {
				return new List<StmtLink>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<StmtLink>>(MethodBase.GetCurrentMethod(),listFKeys,stmtLinkType);
			}
			string command="SELECT * FROM stmtlink WHERE StmtLinkType="+POut.Int((int)stmtLinkType)+" AND FKey IN (" + String.Join(",",listFKeys.Select(x=>POut.Long(x))) + ")" ;
			return Crud.StmtLinkCrud.SelectMany(command);
		}

		#region Insert
		///<summary></summary>
		public static long Insert(StmtLink stmtLink){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				stmtLink.StmtLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),stmtLink);
				return stmtLink.StmtLinkNum;
			}
			return Crud.StmtLinkCrud.Insert(stmtLink);
		}

		///<summary>Creates stmtlink entries for the statement and FKs passed in.</summary>
		public static void AttachFKeysToStatement(long stmtNum,List<long> listFKeys,StmtLinkTypes stmtLinkTypes) {
			//Remoting role check due to looping.  Without this there would be a potential for lots of network traffic.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stmtNum,listFKeys,stmtLinkTypes);
				return;
			}
			for(int i=0;i<listFKeys.Count;i++) {
				StmtLink stmtLink=new StmtLink();
				stmtLink.StatementNum=stmtNum;
				stmtLink.FKey=listFKeys[i];
				stmtLink.StmtLinkType=stmtLinkTypes;
				Insert(stmtLink);
			}
		}

		public static void AttachMsgToPayToStatement(long stmtNum,List<long> listMsgToPayNums) {
			Meth.NoCheckMiddleTierRole();
			AttachFKeysToStatement(stmtNum,listMsgToPayNums,StmtLinkTypes.MsgToPaySent);
		}

		public static void AttachPaySplitsToStatement(long stmtNum,List<long> listPaySplitNums) {
			Meth.NoCheckMiddleTierRole();
			AttachFKeysToStatement(stmtNum,listPaySplitNums,StmtLinkTypes.PaySplit);
		}

		public static void AttachAdjsToStatement(long stmtNum,List<long> listAdjNums) {
			Meth.NoCheckMiddleTierRole();
			AttachFKeysToStatement(stmtNum,listAdjNums,StmtLinkTypes.Adj);
		}

		public static void AttachProcsToStatement(long stmtNum,List<long> listProcNums) {
			Meth.NoCheckMiddleTierRole();
			AttachFKeysToStatement(stmtNum,listProcNums,StmtLinkTypes.Proc);
		}

		public static void AttachClaimsToStatement(long stmtNum, List<long> listClaimNums) {
			Meth.NoCheckMiddleTierRole();
			AttachFKeysToStatement(stmtNum,listClaimNums,StmtLinkTypes.ClaimPay);
		}

		public static void AttachPayPlanChargesToStatement(long stmtNum, List<long> listPayPlanChargeNums) {
			Meth.NoCheckMiddleTierRole();
			AttachFKeysToStatement(stmtNum,listPayPlanChargeNums,StmtLinkTypes.PayPlanCharge);
		}
		#endregion

		#region Update
		/*
		///<summary></summary>
		public static void Update(StmtLink stmtLink){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stmtLink);
				return;
			}
			Crud.StmtLinkCrud.Update(stmtLink);
		}
		*/
		#endregion

		#region Delete
		public static void Delete(long stmtLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stmtLinkNum);
				return;
			}
			Crud.StmtLinkCrud.Delete(stmtLinkNum);
		}

		public static void DetachAllFromStatement(long statementNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum);
				return;
			}
			string command="DELETE FROM stmtlink WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DetachAllFromStatements(List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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