using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{

	public class XChargeTransactions { 
		///<summary></summary>
		public static long Insert(XChargeTransaction xChargeTransaction) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				xChargeTransaction.XChargeTransactionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),xChargeTransaction);
				return xChargeTransaction.XChargeTransactionNum;
			}
			return Crud.XChargeTransactionCrud.Insert(xChargeTransaction);
		}

		///<summary>Gets one XChargeTransaction from the db that matches the given fields.</summary>
		public static XChargeTransaction GetOneMatch(string batchNum,string itemNum,long patNum,DateTime dateTTransaction,string transType){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<XChargeTransaction>(MethodBase.GetCurrentMethod(),batchNum,itemNum,patNum,dateTTransaction,transType);
			}
			string command="SELECT * FROM xchargetransaction WHERE BatchNum = '"+POut.String(batchNum)+"' AND ItemNum = '"+POut.String(itemNum)+"' "
				+"AND PatNum="+POut.Long(patNum)+" AND TransType='"+POut.String(transType)+"' "
				//We include transactions that are the same minute because we used to not store the seconds portion.
				+"AND TransactionDateTime BETWEEN "+POut.DateT(DateTools.ToBeginningOfMinute(dateTTransaction))+" AND "+POut.DateT(DateTools.ToEndOfMinute(dateTTransaction));
			return Crud.XChargeTransactionCrud.SelectOne(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<XChargeTransaction> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<XChargeTransaction>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM xchargetransaction WHERE PatNum = "+POut.Long(patNum);
			return Crud.XChargeTransactionCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(XChargeTransaction xChargeTransaction){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),xChargeTransaction);
				return;
			}
			Crud.XChargeTransactionCrud.Update(xChargeTransaction);
		}
	*/
		///<summary></summary>
		public static void Delete(long xChargeTransactionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),xChargeTransactionNum);
				return;
			}
			string command= "DELETE FROM xchargetransaction WHERE XChargeTransactionNum = "+POut.Long(xChargeTransactionNum);
			Db.NonQ(command);
		}

		public static DataTable GetMissingPaymentsTable(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command="SELECT xchargetransaction.* "
				+"FROM xchargetransaction "
				+"WHERE "+DbHelper.BetweenDates("TransactionDateTime",dateStart,dateEnd)+" "
				+"AND xchargetransaction.ResultCode=0";//Valid entries to count have result code 0
			List<XChargeTransaction> listXChargeTransactions=Crud.XChargeTransactionCrud.SelectMany(command);
			command="SELECT payment.* "
				+"FROM payment "
				//only payments with the same PaymentType as the X-Charge PaymentType for the clinic
				+"INNER JOIN ("
					+"SELECT ClinicNum,PropertyValue PaymentType FROM programproperty "
					+"WHERE ProgramNum="+POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+" AND PropertyDesc='PaymentType'"
				+") paytypes ON paytypes.ClinicNum=payment.ClinicNum AND paytypes.PaymentType=payment.PayType "
				+"WHERE DateEntry BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd);
			List<Payment> listPayments=Crud.PaymentCrud.SelectMany(command);
			for(int i=listXChargeTransactions.Count-1;i>=0;i--) {//Looping backwards in order to remove items
				XChargeTransaction xChargeTransaction=listXChargeTransactions[i];
				Payment payment=listPayments.Where(x => x.PatNum==xChargeTransaction.PatNum)
					.Where(x => x.DateEntry.Date==xChargeTransaction.TransactionDateTime.Date)
					.Where(x => x.PayAmt.Equals(xChargeTransaction.Amount))
					.FirstOrDefault();
				if(payment==null) {//The XCharge transaction does not have a corresponding payment.
					continue;
				}
				listXChargeTransactions.RemoveAt(i);
				listPayments.Remove(payment);//So that the same payment does not get counted for more than one XCharge transaction.
			}
			DataTable table=Crud.XChargeTransactionCrud.ListToTable(listXChargeTransactions);
			List<string> listColumnsToKeep=new List<string> {
				"TransactionDateTime","TransType","ClerkID","ItemNum","PatNum","CreditCardNum","Expiration","Result","Amount","BatchTotal"
			};
			//Remove columns we don't want.
			for(int i=table.Columns.Count-1;i>=0;i--) {
				if(listColumnsToKeep.Contains(table.Columns[i].ColumnName)) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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

		public static DataTable GetXChargeTransactionValidateBatchData(DateTime dateStart, DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command=
				$@"SELECT 
						BatchNum,
						ROUND(SUM(IF(ResultCode = '000' OR ResultCode = '010', Amount,0)),2) Summed,
						SUBSTRING_INDEX(GROUP_CONCAT(BatchTotal ORDER BY TransActionDateTime DESC),',',1) Total,
						ROUND(SUBSTRING_INDEX(GROUP_CONCAT(BatchTotal ORDER BY TransActionDateTime DESC),',',1) - ROUND(SUM(IF(ResultCode = '000' OR ResultCode = '010', Amount,0)),2),2) Difference
					FROM xchargetransaction
					WHERE BatchNum != 0
					AND TransactionDateTime BETWEEN {POut.Date(dateStart)} AND {POut.Date(dateEnd)}
					GROUP BY BatchNum";
			return Db.GetTable(command);
		}
	}
}