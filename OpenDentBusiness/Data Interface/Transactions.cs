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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Transaction>(MethodBase.GetCurrentMethod(),transactionNum);
			}
			return Crud.TransactionCrud.SelectOne(transactionNum);
		}

		public static List<Transaction> GetManyTrans(List<long> listTransactionNums) {
			if(listTransactionNums==null || listTransactionNums.Count<=0) {
				return new List<Transaction>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Transaction>>(MethodBase.GetCurrentMethod(),listTransactionNums);
			}
			string command="SELECT * FROM transaction WHERE transaction.TransactionNum IN("+string.Join(",",listTransactionNums)+")";
			return Crud.TransactionCrud.SelectMany(command);
		}

		///<summary>Gets one transaction directly from the database which has this deposit attached to it.  If none exist, then returns null.</summary>
		public static Transaction GetAttachedToDeposit(long depositNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Transaction>(MethodBase.GetCurrentMethod(),depositNum);
			}
			string command=
				"SELECT * FROM transaction "
				+"WHERE DepositNum="+POut.Long(depositNum);
			return Crud.TransactionCrud.SelectOne(command);
		}

		///<summary>Gets one transaction directly from the database which has this payment attached to it.  If none exist, then returns null.  There should never be more than one, so that's why it doesn't return more than one.</summary>
		public static Transaction GetAttachedToPayment(long payNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Transaction>(MethodBase.GetCurrentMethod(),payNum);
			}
			string command=
				"SELECT * FROM transaction "
				+"WHERE PayNum="+POut.Long(payNum);
			return Crud.TransactionCrud.SelectOne(command);
		}

		///<summary></summary>
		public static long Insert(Transaction transaction) {
			transaction.SecUserNumEdit=Security.CurUser.UserNum;//Before middle tier check to catch user at workstation
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				transaction.TransactionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),transaction);
				return transaction.TransactionNum;
			}
			return Crud.TransactionCrud.Insert(transaction);
		}

		///<summary></summary>
		public static void Update(Transaction transaction) {
			transaction.SecUserNumEdit=Security.CurUser.UserNum;//Before middle tier check to catch user at workstation
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),transaction);
				return;
			}
			Crud.TransactionCrud.Update(transaction);
		}

		///<summary></summary>
		public static void UpdateInvoiceNum(long transactionNum,long transactionInvoiceNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),transactionNum,transactionInvoiceNum);
				return;
			}
			string command="UPDATE transaction SET TransactionInvoiceNum="+POut.Long(transactionInvoiceNum)
				+" WHERE TransactionNum="+POut.Long(transactionNum);
			Db.NonQ(command);
		}

		///<summary>Also deletes all journal entries for the transaction.  Will later throw an error if journal entries attached to any reconciles.  Be sure to surround with try-catch.</summary>
		public static void Delete(Transaction transaction) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),transaction);
				return;
			}
			if(IsTransactionLocked(transaction.TransactionNum)) {
				throw new ApplicationException(Lans.g("Transactions","Not allowed to delete transactions because it is attached to a reconcile that is locked."));
			}
			string command="DELETE FROM journalentry WHERE TransactionNum="+POut.Long(transaction.TransactionNum);
			Db.NonQ(command);
			if(transaction.TransactionInvoiceNum!=0) {
				command="DELETE FROM transactioninvoice WHERE TransactionInvoiceNum="+POut.Long(transaction.TransactionInvoiceNum);
				Db.NonQ(command);
			}
			command= "DELETE FROM transaction WHERE TransactionNum="+POut.Long(transaction.TransactionNum);
			Db.NonQ(command);
		}

		private static bool IsTransactionLocked(long transactionNum) {
			//no need to check remoting role; private method
			string command="SELECT IsLocked FROM journalentry j, reconcile r WHERE j.TransactionNum="+POut.Long(transactionNum)
				+" AND j.ReconcileNum = r.ReconcileNum";
			DataTable table=Db.GetTable(command);	
			if(table.Rows.Count>0 && PIn.Int(table.Rows[0][0].ToString())==1) {
				return true;
			}
			return false;
		}

		public static bool IsAttachedToLockedReconcile(Transaction transaction) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),transaction);
			}
			return IsTransactionLocked(transaction.TransactionNum);
		}

		///<summary></summary>
		public static bool IsReconciled(Transaction transaction){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),transaction);
			}
			string command="SELECT COUNT(*) FROM journalentry WHERE ReconcileNum !=0"
				+" AND TransactionNum="+POut.Long(transaction.TransactionNum);
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}
	}

	
}




