using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TransactionInvoices{

		///<summary></summary>
		public static TransactionInvoice GetOne(long transactionInvoiceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<TransactionInvoice>(MethodBase.GetCurrentMethod(),transactionInvoiceNum);
			}
			string command="SELECT * FROM transactioninvoice WHERE TransactionInvoiceNum = "+POut.Long(transactionInvoiceNum);
			return Crud.TransactionInvoiceCrud.SelectOne(command);
		}

		///<summary>Used only to get the name of the file, so we're not querying the entire document data (which could be multiple megabytes).</summary>
		public static string GetName(long transactionInvoiceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),transactionInvoiceNum);
			}
			string command="SELECT FileName FROM transactioninvoice WHERE TransactionInvoiceNum = "+POut.Long(transactionInvoiceNum);
			return Db.GetScalar(command);
		}

		///<summary></summary>
		public static long Insert(TransactionInvoice transactionInvoice) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				transactionInvoice.TransactionInvoiceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),transactionInvoice);
				return transactionInvoice.TransactionInvoiceNum;
			}
			return Crud.TransactionInvoiceCrud.Insert(transactionInvoice);
		}

		///<summary></summary>
		public static void Delete(long transactionInvoiceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),transactionInvoiceNum);
				return;
			}
			Crud.TransactionInvoiceCrud.Delete(transactionInvoiceNum);
		}
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get

		
		///<summary>Gets one TransactionInvoice from the db.</summary>
		public static TransactionInvoice GetOne(long transactionInvoiceNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TransactionInvoice>(MethodBase.GetCurrentMethod(),transactionInvoiceNum);
			}
			return Crud.TransactionInvoiceCrud.SelectOne(transactionInvoiceNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static void Update(TransactionInvoice transactionInvoice){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),transactionInvoice);
				return;
			}
			Crud.TransactionInvoiceCrud.Update(transactionInvoice);
		}

		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/



	}
}