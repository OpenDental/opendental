using System;

namespace OpenDentBusiness {
	///<summary>A phone number used to send and receive SMS.
	///When clinics is enabled all SmsPhones with clinic num 0 should be updated to have clinic num of the lowest numbered clinic.
	///When clinics are disabled, all SmsPhones with the lowest numbered clinic num should be re-associated to clinic number 0.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class SmsPhone:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SmsPhoneNum;
		///<summary>FK to clinic.ClinicNum. </summary>
		public long ClinicNum;
		///<summary>String representation of the phone number in international format. Ex: 15035551234 This field should not contain any formatting characters.</summary>
		public string PhoneNumber;
		///<summary>Date and time this phone number became active.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeActive;
		///<summary>Date and time this phone number became inactive. Once inactive, the phone is dead and cannot be reactivated. A new number will have to be purchased.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeInactive;
		///<summary>Used to indicate why this phone number was made inactive.</summary>
		public string InactiveCode;
		///<summary>Country linked to this phone's clinic at the instant that this phone is created. Based on ISO31661.</summary>
		public string CountryCode;
		///<summary>True if this is the primary phone for the clinic. Only valid from the eService Setup window.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsPrimary;

		///<summary></summary>
		public SmsPhone Copy() {
			return (SmsPhone)this.MemberwiseClone();
		}
	}
}