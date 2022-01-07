using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {

	///<summary>XCharge transactions that have been imported into OD.  Used by reconcile tool.  Keeps a history, but no references to these rows from other tables.</summary>
	[Serializable()]
	public class XChargeTransaction:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long XChargeTransactionNum;
		///<summary>Usually "CCPurchase."</summary>
		public string TransType;
		///<summary>Amount.</summary>
		public double Amount;
		///<summary>Credit card entry method. Usually "Keyed".</summary>
		public string CCEntry;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Result: AP for approved, DECLINE for declined.</summary>
		public string Result;
		///<summary>ClerkID. Open Dental username with a possible " R" at the end to indicate a recurring charge.</summary>
		public string ClerkID;
		///<summary>ResultCode: 000 for approved, 005 for declined.</summary>
		public string ResultCode;
		///<summary>Expiration is shown as a four digit number (string since it may contain leading zeros).</summary>
		public string Expiration;
		///<summary>VISA, AMEX, MC, DISC etc.</summary>
		public string CCType;
		///<summary>Usually looks like 123456XXXXXX7890.</summary>
		public string CreditCardNum;
		///<summary>BatchNum.</summary>
		public string BatchNum;
		///<summary>ItemNum. Starts at 0001 for each batch.</summary>
		public string ItemNum;
		///<summary>Approval code. 6 characters. 72142Z for example.</summary>
		public string ApprCode;
		///<summary>TransactionDateTime. Is taken from the Date and Time columns in X-Charge.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime TransactionDateTime;


	}
}
