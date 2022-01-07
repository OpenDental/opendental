using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>All info about a referral is stored with that referral even if a patient.  That way, it's available for easy queries.</summary>
	[Serializable]
	public class Referral:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ReferralNum;
		///<summary>Last name.</summary>
		public string LName;
		///<summary>First name.</summary>
		public string FName;
		///<summary>Middle name or initial.</summary>
		public string MName;
		///<summary>SSN or TIN, no punctuation.  For Canada, this holds the referring provider CDA num for claims.</summary>
		public string SSN;
		///<summary>Specificies if SSN is real SSN.</summary>
		public bool UsingTIN;
		///<summary>FK to definition.DefNum.</summary>
		public long Specialty;
		///<summary>State</summary>
		public string ST;
		///<summary>Primary phone, restrictive, must only be 10 digits and only numbers.</summary>
		public string Telephone;
		///<summary>.</summary>
		public string Address;
		///<summary>.</summary>
		public string Address2;
		///<summary>.</summary>
		public string City;
		///<summary>.</summary>
		public string Zip;
		///<summary>Holds important info about the referral.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>Additional phone no restrictions</summary>
		public string Phone2;
		///<summary>Can't delete a referral, but can hide if not needed any more.</summary>
		public bool IsHidden;
		///<summary>Set to true for referralls such as Yellow Pages.</summary>
		public bool NotPerson;
		///<summary>i.e. DMD or DDS</summary>
		public string Title;
		///<summary>.</summary>
		public string EMail;
		///<summary>FK to patient.PatNum for referrals that are patients.</summary>
		public long PatNum;
		///<summary>NPI for the referral</summary>
		public string NationalProvID;
		///<summary>FK to sheetdef.SheetDefNum.  Referral slips can be set for individual referral sources.  If zero, then the default internal referral slip will be used instead of a custom referral slip.</summary>
		public long Slip;
		///<summary>True if another dentist or physician.  Cannot be a patient.</summary>
		public bool IsDoctor;
		///<summary>True if checkbox E-mail Trust for Direct is checked.</summary>
		public bool IsTrustedDirect;
		///<summary>The datetime this referral was last edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>True if the referral is a preferred referral.</summary>
		public bool IsPreferred;
		///<summary>Represents the name of the business that the referral works for.</summary>
		public string BusinessName;
		///<summary>This is a global field used for Scheduling Notes that will show in the family module patient info grid.</summary>
		public string DisplayNote;

		///<summary>Returns a copy of this Referral.</summary>
		public Referral Copy(){
			return (Referral)this.MemberwiseClone();
		}
		
		///<summary>Includes title, such as DMD.</summary>
		public string GetNameFL() {
			string retVal="";
			if(FName!="") {
				retVal+=FName+" ";
			}
			if(MName!="") {
				retVal+=MName+" ";
			}
			retVal+=LName;
			if(Title!="") {
				retVal+=", "+Title;
			}
			return retVal;
		}

	}


}













