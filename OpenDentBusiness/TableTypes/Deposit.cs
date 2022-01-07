using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>A deposit slip.  Contains multiple insurance and patient checks.</summary>
	[Serializable]
	public class Deposit:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DepositNum;
		///<summary>The date of the deposit.</summary>
		public DateTime DateDeposit;
		///<summary>User editable.  Usually includes name on the account and account number.  Possibly the bank name as well.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BankAccountInfo;
		///<summary>Total amount of the deposit. User not allowed to directly edit.</summary>
		public double Amount;
		///<summary>Short description to help identify the deposit.</summary>
		public string Memo;
		///<summary>Holds the batch number for the deposit. Does not have a default value. 25 character limit.</summary>
		public string Batch;
		///<summary>FK to definition.DefNum. Links this deposit to a definition of type AutoDeposit.  When set to a valid value, it indicates that this deposit is an "auto deposit".</summary>
		public long DepositAccountNum;
		///<summary>Not in the database table.  Identifies the clinic(s) that the deposit is associated to.
		///'(None)', specific clinic abbr or '(Multiple)'.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ClinicAbbr;
		///<summary>Bool that indicates of a deposit has already been sent via QuickBooks Online. Defaults to false. Only true when successfully sent from FormDepositEdit.cs</summary>
		public bool IsSentToQuickBooksOnline;
		
		///<summary></summary>
		public Deposit Copy(){
			return (Deposit)this.MemberwiseClone();
		}
	}

	

	


}




















