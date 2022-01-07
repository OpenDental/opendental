using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{

	public class XChargeTransactions { 
		///<summary></summary>
		public static long Insert(XChargeTransaction xChargeTransaction) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				xChargeTransaction.XChargeTransactionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),xChargeTransaction);
				return xChargeTransaction.XChargeTransactionNum;
			}
			return Crud.XChargeTransactionCrud.Insert(xChargeTransaction);
		}

		///<summary>Gets one XChargeTransaction from the db that matches the given fields.</summary>
		public static XChargeTransaction GetOneMatch(string batchNum,string itemNum,long patNum,DateTime transactionDateT,string transType){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<XChargeTransaction>(MethodBase.GetCurrentMethod(),batchNum,itemNum,patNum,transactionDateT,transType);
			}
			string command="SELECT * FROM xchargetransaction WHERE BatchNum = '"+POut.String(batchNum)+"' AND ItemNum = '"+POut.String(itemNum)+"' "
				+"AND PatNum="+POut.Long(patNum)+" AND TransType='"+POut.String(transType)+"' "
				//We include transactions that are the same minute because we used to not store the seconds portion.
				+"AND TransactionDateTime BETWEEN "+POut.DateT(DateTools.ToBeginningOfMinute(transactionDateT))+" AND "+POut.DateT(DateTools.ToEndOfMinute(transactionDateT));
			return Crud.XChargeTransactionCrud.SelectOne(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<XChargeTransaction> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<XChargeTransaction>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM xchargetransaction WHERE PatNum = "+POut.Long(patNum);
			return Crud.XChargeTransactionCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(XChargeTransaction xChargeTransaction){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),xChargeTransaction);
				return;
			}
			Crud.XChargeTransactionCrud.Update(xChargeTransaction);
		}
	*/
		///<summary></summary>
		public static void Delete(long xChargeTransactionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),xChargeTransactionNum);
				return;
			}
			string command= "DELETE FROM xchargetransaction WHERE XChargeTransactionNum = "+POut.Long(xChargeTransactionNum);
			Db.NonQ(command);
		}

		public static DataTable GetMissingPaymentsTable(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command="SELECT xchargetransaction.* "
				+"FROM xchargetransaction "
				+"WHERE "+DbHelper.BetweenDates("TransactionDateTime",dateStart,dateEnd)+" "
				+"AND xchargetransaction.ResultCode=0";//Valid entries to count have result code 0
			List<XChargeTransaction> listTrans=Crud.XChargeTransactionCrud.SelectMany(command);
			command="SELECT payment.* "
				+"FROM payment "
				//only payments with the same PaymentType as the X-Charge PaymentType for the clinic
				+"INNER JOIN ("
					+"SELECT ClinicNum,PropertyValue PaymentType FROM programproperty "
					+"WHERE ProgramNum="+POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+" AND PropertyDesc='PaymentType'"
				+") paytypes ON paytypes.ClinicNum=payment.ClinicNum AND paytypes.PaymentType=payment.PayType "
				+"WHERE DateEntry BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
			List<Payment> listPays=Crud.PaymentCrud.SelectMany(command);
			for(int i=listTrans.Count-1;i>=0;i--) {//Looping backwards in order to remove items
				XChargeTransaction tran=listTrans[i];
				Payment pay=listPays.Where(x => x.PatNum==tran.PatNum)
					.Where(x => x.DateEntry.Date==tran.TransactionDateTime.Date)
					.Where(x => x.PayAmt.Equals(tran.Amount))
					.FirstOrDefault();
				if(pay==null) {//The XCharge transaction does not have a corresponding payment.
					continue;
				}
				listTrans.RemoveAt(i);
				listPays.Remove(pay);//So that the same payment does not get counted for more than one XCharge transaction.
			}
			DataTable table=Crud.XChargeTransactionCrud.ListToTable(listTrans);
			List<string> listColumnsToKeep=new List<string> {
				"TransactionDateTime","TransType","ClerkID","ItemNum","PatNum","CreditCardNum","Expiration","Result","Amount"
			};
			//Remove columns we don't want.
			for(int i=table.Columns.Count-1;i>=0;i--) {
				if(ListTools.In(table.Columns[i].ColumnName,listColumnsToKeep)) {
					continue;
				}
				table.Columns.RemoveAt(i);
			}
			//Reorder the column in the order we want them.
			for(int i=0;i<listColumnsToKeep.Count;i++) {
				table.Columns[listColumnsToKeep[i]].SetOrdinal(i);
			}
			return table;
		}

		public static DataTable GetMissingXTransTable(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command="SELECT payment.PatNum,LName,FName,payment.DateEntry,payment.PayDate,payment.PayNote,payment.PayAmt "
				+"FROM patient "
				+"INNER JOIN payment ON payment.PatNum=patient.PatNum "
				//only payments with the same PaymentType as the X-Charge PaymentType for the clinic
				+"INNER JOIN ("
					+"SELECT ClinicNum,PropertyValue AS PaymentType FROM programproperty "
					+"WHERE ProgramNum="+POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+" AND PropertyDesc='PaymentType'"
				+") paytypes ON paytypes.ClinicNum=payment.ClinicNum AND paytypes.PaymentType=payment.PayType "
				+"LEFT JOIN xchargetransaction ON xchargetransaction.PatNum=payment.PatNum "
					+"AND "+DbHelper.DtimeToDate("TransactionDateTime")+"=payment.DateEntry "
					+"AND (CASE WHEN xchargetransaction.ResultCode=5 THEN 0 ELSE xchargetransaction.Amount END)=payment.PayAmt "
					+"AND xchargetransaction.ResultCode IN(0,5,10) "
				+"WHERE payment.DateEntry BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)+" "
				+"AND TransactionDateTime IS NULL "
				+"ORDER BY payment.PayDate ASC,LName,FName";
			return Db.GetTable(command);
		}
	}
}