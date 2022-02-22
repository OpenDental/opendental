using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>In the accounting section, this automates entries into the database when user enters a payment into a patient account.  This table presents the user with a picklist specific to that payment type.  For example, a cash payment would create a picklist of cashboxes for user to put the cash into.</summary>
	[Serializable()]
	public class AccountingAutoPay:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AccountingAutoPayNum;
		///<summary>FK to definition.DefNum.</summary>
		public long PayType;
		///<summary>FK to account.AccountNum.  AccountNums separated by commas.  No spaces.</summary>
		public string PickList;

		///<summary>Returns a copy of this AccountingAutoPay.</summary>
		public AccountingAutoPay Clone(){
			return (AccountingAutoPay)this.MemberwiseClone();
		}

	}
		
	
	


}













