using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Used in accounting to represent a single credit or debit entry.  There will always be at least 2 journal enties attached to every transaction.  All transactions balance to 0.</summary>
	[Serializable]
	public class JournalEntry:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JournalEntryNum;
		///<summary>FK to transaction.TransactionNum</summary>
		public long TransactionNum;
		///<summary>FK to account.AccountNum</summary>
		public long AccountNum;
		///<summary>Always the same for all journal entries within one transaction.</summary>
		public DateTime DateDisplayed;
		///<summary>Negative numbers never allowed.</summary>
		public double DebitAmt;
		///<summary>Negative numbers never allowed.</summary>
		public double CreditAmt;
		///<summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Memo;
		///<summary>A human-readable description of the splits.  Used only for display purposes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Splits;
		///<summary>Any user-defined string.  Usually a check number, but can also be D for deposit, Adj, etc.</summary>
		public string CheckNumber;
		///<summary>FK to reconcile.ReconcileNum. 0 if not attached to a reconcile. Not allowed to alter amounts if attached.</summary>
		public long ReconcileNum;
		///<summary>FK to userod.UserNum. The user who created this journal entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>The date and time that this journal entry was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>FK to userod.UserNum. The user who last edited this journal entry.</summary>
		public long SecUserNumEdit;
		///<summary>The last time this journal entry was edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		///<summary></summary>
		public JournalEntry Copy() {
			return (JournalEntry)this.MemberwiseClone();
		}






	}

	
}




