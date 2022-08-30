using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Used in the accounting section of the program.  Each row is one transaction in the ledger, and must always have at least two splits.  All splits must always add up to zero.</summary>
	[Serializable]
	public class Transaction:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TransactionNum;
		///<summary>Not user editable.  Server time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>FK to userod.UserNum. The user that entered this transaction.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long UserNum;
		///<summary>FK to deposit.DepositNum.  Will eventually be replaced by a source document table, and deposits will just be one of many types.</summary>
		public long DepositNum;
		///<summary>FK to payment.PayNum.  Like DepositNum, it will eventually be replaced by a source document table, and payments will just be one of many types.</summary>
		public long PayNum;
		///<summary>FK to userod.UserNum. The user who last edited this transaction.</summary>
		public long SecUserNumEdit;
		///<summary>The last time this transaction was edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>FK to transactioninvoice.TransactionInvoiceNum.  A document that can be attached to the transaction.</summary>
		public long TransactionInvoiceNum;

		///<summary></summary>
		public Transaction Copy() {
			return (Transaction)MemberwiseClone();
		}


	}
}