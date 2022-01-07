using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Used to store phone numbers for patients.</summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true)]
	public class PhoneNumber:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PhoneNumberNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>The actual phone number for the patient.  Includes any punctuation.  No leading 1 or plus, so almost always 10 digits.</summary>
		public string PhoneNumberVal;
		///<summary>The phone number for the patient with all non-digit chars and any leading 1's or 0's removed.</summary>
		public string PhoneNumberDigits;
		///<summary>Enum:PhoneType .  Used to determine which column in the patient table, if any, this row should be synced with.  Rows with 0 - Other
		///are not synced with patient table columns.  The other values sync with their corresponding column in the patient table 1 - HmPhone,
		///2 - WkPhone, and 3 - WirelessPhone.</summary>
		public PhoneType PhoneType;

	
	}

	///<summary>Used to identify which field, if any, in the patient table to </summary>
	public enum PhoneType {
		///<summary>0 - Other</summary>
		Other,
		///<summary>1 - HmPhone.  Row is synced with the patient.HmPhone column.</summary>
		HmPhone,
		///<summary>2 - WkPhone.  Row is synced with the patient.WkPhone column.</summary>
		WkPhone,
		///<summary>3 - WirelessPhone.  Row is synced with the patient.WirelessPhone column.</summary>
		WirelessPhone
	}
}















