using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	///<summary>Every InsPlan has a Carrier.  The carrier stores the name and address.</summary>
	[Serializable()]
	[CrudTable(IsSecurityStamped=true)]
	public class Carrier:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CarrierNum;
		///<summary>Name of the carrier.</summary>
		public string CarrierName;
		///<summary>.</summary>
		public string Address;
		///<summary>Second line of address.</summary>
		public string Address2;
		///<summary>.</summary>
		public string City;
		///<summary>2 char in the US.</summary>
		public string State;
		///<summary>Postal code.</summary>
		public string Zip;
		///<summary>Includes any punctuation.</summary>
		public string Phone;
		///<summary>E-claims electronic payer id.  5 char in USA.  6 digits in Canada.  I've seen an ID this long before: "LA-DHH-MEDICAID".  The user interface currently limits length to 20, although db limits length to 255.  X12 requires length between 2 and 80.</summary>
		public string ElectID;
		///<summary>Enum:NoSendElectType 0 - send electronically, 1 - don't send electronically, 2 - don't send non-primary (secondary,tertiary, etc.) 
		///claims electronically.</summary>
		public NoSendElectType NoSendElect;
		///<summary>Canada: True if a CDAnet carrier.  This has significant implications:  1. It can be filtered for in the list of carriers.  2. An ElectID is required.  3. The ElectID can never be used by another carrier.  4. If the carrier is attached to any etrans, then the ElectID cannot be changed (and, of course, the carrier cannot be deleted or combined).</summary>
		public bool IsCDA;
		///<summary>The version of CDAnet supported.  Either 02 or 04.</summary>
		public string CDAnetVersion;
		///<summary>FK to canadiannetwork.CanadianNetworkNum.  Only used in Canada.  Right now, there is no UI to the canadiannetwork table in our db.</summary>
		public long CanadianNetworkNum;
		///<summary>.</summary>
		public bool IsHidden;
		///<summary>1=No Encryption, 2=CDAnet standard #1, 3=CDAnet standard #2.  Field A10.
		///Deprecated for all Canadian carriers.  Will always be 1 (No Encryption).</summary>
		public byte CanadianEncryptionMethod;
		///<summary>Bit flags.</summary>
		public CanSupTransTypes CanadianSupportedTypes;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>Tax ID Number.  Not user editable.  No UI for this field.
		///Used in when importing insurance plans from 834s to uniquely identify carriers.</summary>
		public string TIN;
		///<summary>FK to definition.DefNum. Links carriers into groups for queries.</summary>
		public long CarrierGroupName;
			///<summary>Color that the carrier is highlighted within the appointment module appointment and popup bubble.  Black indicates no color.</summary>
		[XmlIgnore]
		public Color ApptTextBackColor;
		///<summary>False by default.  Determines if the carrier supplied EB08 field of 271 transactions should be inverted for coinsurance percentages.
		///When true carriers sent us insurance percentage so we do not need to invert it, it is already inverted for us.</summary>
		public bool IsCoinsuranceInverted;
		///<summary>Bit flags.  None (0) by default.  Stores trusted user selected X12 transaction types related to this carrier.</summary>
		public TrustedEtransTypes TrustedEtransFlags;
		///<summary>Enum:EclaimCobInsPaidBehavior .  When sending X12 5010 eclaims, if not set to Default, then this setting overrides the ClaimCobInsPaidBehavior preference.</summary>
		public EclaimCobInsPaidBehavior CobInsPaidBehaviorOverride;
		///<summary>Enum:EraAutomationMode UseGlobal (0) by default. Determines the level of ERA processing automation for this carrier. This will override the EraAutomationBehavior preference when not set to UseGlobal.</summary>
		public EraAutomationMode EraAutomationOverride;
		///<summary>Enum:EnumOrthoInsPayConsolidate Global (0) by default. Determines how this carrier requires payments made to ortho claims made by the Auto Ortho Tool. This will override the OrthoInsPayConsolidated preference when not set to Global.</summary>
		public EnumOrthoInsPayConsolidate OrthoInsPayConsolidate;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ApptTextBackColor",typeof(int))]
		public int ApptTextBackColorXml {
			get {
				return ApptTextBackColor.ToArgb();
			}
			set {
				ApptTextBackColor=Color.FromArgb(value);
			}
		}

		public Carrier Copy(){
			return (Carrier)this.MemberwiseClone();
		}

		public Carrier() {

		}

		///<summary>Gets the EraAutomationBehavior preference if the Carrier.EraAutomationOverride is set to UseGlobal. Otherwise, returns the Carrier.EraAutomationOverride.</summary>
		public EraAutomationMode GetEraAutomationMode() {
			if(this.EraAutomationOverride==EraAutomationMode.UseGlobal) {
				return PrefC.GetEnum<EraAutomationMode>(PrefName.EraAutomationBehavior);
			}
			return this.EraAutomationOverride;
		}

	}

	

	///<summary>Type 23, Predetermination EOB (regular and embedded) are not included because they are not part of the testing scripts.  The three required types are not included: ClaimTransaction_01, ClaimAcknowledgement_11, and ClaimEOB_21.  Can't find specs for PredeterminationEobEmbedded.</summary>
	[Flags]
	public enum CanSupTransTypes {
		///<summary></summary>
		None=0,
		///<summary></summary>
		EligibilityTransaction_08=1,
		///<summary></summary>
		EligibilityResponse_18=2,
		///<summary></summary>
		CobClaimTransaction_07=4,
		///<summary>ClaimAck_11 is not here because it's required by all carriers.</summary>
		ClaimAckEmbedded_11e=8,
		///<summary>ClaimEob_21 is not here because it's required by all carriers.</summary>
		ClaimEobEmbedded_21e=16,
		///<summary></summary>
		ClaimReversal_02=32,
		///<summary></summary>
		ClaimReversalResponse_12=64,
		///<summary></summary>
		PredeterminationSinglePage_03=128,
		///<summary></summary>
		PredeterminationMultiPage_03=256,
		///<summary>Predetermination Acknowledgment.</summary>
		PredeterminationAck_13=512,
		///<summary>Predetermination Acknowledgement.  Secondary transaction nested inside primary transaction.</summary>
		PredeterminationAckEmbedded_13e=1024,
		///<summary></summary>
		RequestForOutstandingTrans_04=2048,
		///<summary></summary>
		OutstandingTransAck_14=4096,
		///<summary>Response</summary>
		EmailTransaction_24=8192,
		///<summary></summary>
		RequestForSummaryReconciliation_05=16384,
		///<summary></summary>
		SummaryReconciliation_15=32768,
		///<summary></summary>
		RequestForPaymentReconciliation_06=65536,
		///<summary></summary>
		PaymentReconciliation_16=131072
	}

	public enum NoSendElectType {
		///<summary>0 - Sending electronically is allowed for this carrier.</summary>
		[Description("Send Claims Electronically")]
		SendElect,
		///<summary>1 - Do not send electronically for this carrier.</summary>
		[Description("Don't Send Claims Electronically")]
		NoSendElect,
		///<summary>2 - Do not send electronically for this carrier if the carrier is not the primary insurance for the patient.</summary>
		[Description("Don't Send Secondary Claims Electronically")]
		NoSendSecondaryElect,
	}

	[Flags]
	public enum TrustedEtransTypes{
		///<summary>0 - Default, no trusted types.</summary>
		None=0,
		///<summary>1 - When used in bit-wise value enables the automated import of certain fields for 271s, otherwise disabled.</summary>
		RealTimeEligibility=1
	}

	public enum EclaimCobInsPaidBehavior {
		///<summary>Use the global preference value instead of the carrier override.</summary>
		[Description("Default")]
		Default=0,
		///<summary>Only send COB eclaim data claim totals.</summary>
		[Description("Claim Level")]
		ClaimLevel=1,
		///<summary>Only send COD eclaim data respective procedure amounts.</summary>
		[Description("Procedure Level")]
		ProcedureLevel=2,
		///<summary>Send COB eclaim data claim totals and respective procedure amounts.</summary>
		[Description("Both")]
		Both=3
	}

	///<summary></summary>
	public enum EnumOrthoInsPayConsolidate {
		///<summary>Uses the preference value of OrthoInsPayConsolidated.</summary>
		[Description("Use Global Preference")]
		Global,
		///<summary>Overrides the preference value of OrthoInsPayConsolidated and blocks users from entering payments on claims created by the Auto Ortho Tool.</summary>
		[Description("On")]
		ForceConsolidateOn,
		///<summary>Overrides the preference value of OrthoInsPayConsolidated and allows users to enter payments on claims created by the Auto Ortho Tool.</summary>
		[Description("Off")]
		ForceConsolidateOff,
	}

}













