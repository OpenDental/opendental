using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Corresponds to the electid table in the database. Helps with entering elecronic/payor id's as well as keeping track of the specific carrier requirements. Only used by the X12 format.</summary>
	[Serializable]
	public class ElectID:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ElectIDNum;
		///<summary>aka Electronic ID.  A simple string. This is not necessarily unique between different CarrierNames.  Also, different clearinghouses use different systems of PayorIDs.</summary>
		public string PayorID;
		///<summary>Used when doing a search.</summary>
		public string CarrierName;
		///<summary>True if medicaid. Then, the billing and treating providers will have their Medicaid ID's attached.</summary>
		public bool IsMedicaid;
		///<summary>Integers separated by commas. Each long represents a ProviderSupplementalID type that is required by this insurance. Usually only used for BCBS or other carriers that require supplemental provider id's.  Even if we don't put the supplemental types in here, the user can still add them.  This just helps by doing an additional check for known required types.</summary>
		public string ProviderTypes;
		///<summary>Any comments. Usually includes enrollment requirements and descriptions of how to use the provider id's supplied by the carrier because they might call them by different names.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Comments;
		///<summary>Enum:EclaimsCommBridge Where this Electronic ID came from. Will be 0 if created by the user. Currently, only ClaimConnect and EDS are supported.</summary>
		public EclaimsCommBridge CommBridge;
		///<summary>Comma delimited list of which PayerAttributes of a CommBridge are supported by this Electronic ID. Example: "0,2,8". Enum values for either EnumClaimConnectPayerAttributes or EnumEDSPayerAttributes.</summary>
		public string Attributes;

		public ElectID Copy() {
			return (ElectID)this.MemberwiseClone();
		}

	
	}
	
	///<summary>Attributes for a payer returned by DentalXChange when retrieving their payer list.</summary>
	public enum EnumClaimConnectPayerAttributes {
		///<summary>0 - Determines payer support for Claim submissions.</summary>
		ClaimIsSupported,
		///<summary>1 - Determines payer support for Eligibility submissions.</summary>
		EligibilityIsSupported,
		///<summary>2 - A possible requirement for EligibilityIsSupported.
		///Must send name and DOB information for subscriber to the payer, even when the eligibility request is for a dependent.</summary>
		PatientAndSubscriberReqdForElig,
		///<summary>3 - A possible requirement for EligibilityIsSupported.
		///Determines if patient information is required for an eligibility request.</summary>
		PatientReqdForElig,
		///<summary>4 - Determines payer support for Eligibility submissions with expectation that a robust benefit response will be returned.</summary>
		BenefitsIsSupported,
		///<summary>5 - Determines payer support for Claim Status Inquiries in Real-Time.</summary>
		ClaimStatusIsSupported,
		///<summary>6 - Determines payer support for ERA receipts.</summary>
		ERAIsSupported,
		///<summary>7 - Determines payer support for Real-Time Claim submission.</summary>
		RTClaimIsSupported,
		///<summary>8 - Determines payer support for DentalXChange Attachment submissions.</summary>
		DXCAttachmentIsSupported
	}

	///<summary>Attributes for a payer returned by EDS when retrieving their payer list.</summary>
	public enum EnumEDSPayerAttributes {
		///<summary>0 - Determines whether this is an electronic or paper payer. If this flag is set, it is electronic. Otherwise, paper.</summary>
		DefaultClaimTP,
		///<summary>1 - Determines payer support for accepting realtime claim submissions.</summary>
		RealtimeClaimTP,
		///<summary>2 - Determines payer support for accepting Eligibility requests.</summary>
		EligibilityTP,
		///<summary>3 - Determines if the payer sends back ERA's.</summary>
		ERATP,
		///<summary>4 - Determines if this payer requires enrollment for claim submissions.</summary>
		ClaimEnrollment,
		///<summary>5 - Determines if this payer requires enrollment for ERA responses.</summary>
		ERAEnrollment,
		///<summary>6 - Determines whether this is a dental or medical payer. If this flag is set, it is dental. Otherwise, medical.</summary>
		PayerType
	}
	

}










