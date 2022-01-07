using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Transactions {
		///<summary>Since transactions are always viewed individually, this function returns one transaction</summary>
		public static Transaction GetTrans(long transactionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Transaction>(MethodBase.GetCurrentMethod(),transactionNum);
			}
			return Crud.TransactionCrud.SelectOne(transactionNum);
		}

		public static List<Transaction> GetManyTrans(List<long> listTransactionNums) {
			if(listTransactionNums==null || listTransactionNums.Count<=0) {
				return new List<Transaction>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Transaction>>(MethodBase.GetCurrentMethod(),listTransactionNums);
			}
			string command="SELECT * FROM transaction WHERE transaction.TransactionNum IN("+string.Join(",",listTransactionNums)+")";
			return Crud.TransactionCrud.SelectMany(command);
		}

		///<summary>Gets one transaction directly from the database which has this deposit attached to it.  If none exist, then returns null.</summary>
		public static Transaction GetAttachedToDeposit(long depositNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Transaction>(MethodBase.GetCurrentMethod(),depositNum);
			}
			string command=
				"SELECT * FROM transaction "
				+"WHERE DepositNum="+POut.Long(depositNum);
			return Crud.TransactionCrud.SelectOne(command);
		}

		///<summary>Gets one transaction directly from the database which has this payment attached to it.  If none exist, then returns null.  There should never be more than one, so that's why it doesn't return more than one.</summary>
		public static Transaction GetAttachedToPayment(long payNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Transaction>(MethodBase.GetCurrentMethod(),payNum);
			}
			string command=
				"SELECT * FROM transaction "
				+"WHERE PayNum="+POut.Long(payNum);
			return Crud.TransactionCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(Transaction trans) {
			trans.SecUserNumEdit=Security.CurUser.UserNum;//Before middle tier check to catch user at workstation
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				trans.TransactionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),trans);
				return trans.TransactionNum;
			}
			return Crud.TransactionCrud.Insert(trans);
		}

		///<summary></summary>
		public static void Update(Transaction trans) {
			trans.SecUserNumEdit=Security.CurUser.UserNum;//Before middle tier check to catch user at workstation
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),trans);
				return;
			}
			Crud.TransactionCrud.Update(trans);
		}

		///<summary></summary>
		public static void UpdateInvoiceNum(long transactionNum,long transactionInvoiceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),transactionNum,transactionInvoiceNum);
				return;
			}
			string command="UPDATE transaction SET TransactionInvoiceNum="+POut.Long(transactionInvoiceNum)
				+" WHERE TransactionNum="+POut.Long(transactionNum);
			Db.NonQ(command);
		}

		///<summary>Also deletes all journal entries for the transaction.  Will later throw an error if journal entries attached to any reconciles.  Be sure to surround with try-catch.</summary>
		public static void Delete(Transaction trans) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),trans);
				return;
			}
			string command="SELECT IsLocked FROM journalentry j, reconcile r WHERE j.TransactionNum="+POut.Long(trans.TransactionNum)
				+" AND j.ReconcileNum = r.ReconcileNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				if(PIn.Int(table.Rows[0][0].ToString())==1) {
					throw new ApplicationException(Lans.g("Transactions","Not allowed to delete transactions because it is attached to a reconcile that is locked."));
				}
			}
			command="DELETE FROM journalentry WHERE TransactionNum="+POut.Long(trans.TransactionNum);
			Db.NonQ(command);
			if(trans.TransactionInvoiceNum!=0) {
				command="DELETE FROM transactioninvoice WHERE TransactionInvoiceNum="+POut.Long(trans.TransactionInvoiceNum);
				Db.NonQ(command);
			}
			command= "DELETE FROM transaction WHERE TransactionNum="+POut.Long(trans.TransactionNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static bool IsReconciled(Transaction trans){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),trans);
			}
			string command="SELECT COUNT(*) FROM journalentry WHERE ReconcileNum !=0"
				+" AND TransactionNum="+POut.Long(trans.TransactionNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}
	}

	
}




