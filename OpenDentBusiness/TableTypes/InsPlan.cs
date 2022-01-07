using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{
	//Any changes made to this tabletype needs to be documented on the Online Manual
	///<summary>Subscribers can share insplans by using the InsSub table.  The patplan table determines coverage for individual patients.  InsPlans can also exist without any subscriber. </summary>
	[Serializable]
	[CrudTable(AuditPerms=CrudAuditPerm.InsPlanChangeCarrierName,IsSecurityStamped=true)]
	public class InsPlan:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PlanNum;
		///<summary>Optional</summary>
		public string GroupName;
		///<summary>Optional.  In Canada, this is called the Plan Number.</summary>
		public string GroupNum;
		///<summary>Note for this plan.  Same for all subscribers.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PlanNote;
		///<summary>FK to feesched.FeeSchedNum.</summary>
		public long FeeSched;
		///<summary>""=percentage(the default),"p"=ppo_percentage,"f"=flatCopay,"c"=capitation.</summary>
		public string PlanType;
		///<summary>FK to claimform.ClaimFormNum. eg. "1" for ADA2002.  For ADA2006, it varies by office.</summary>
		public long ClaimFormNum;
		///<summary>0=no,1=yes.  could later be extended if more alternates required</summary>
		public bool UseAltCode;
		///<summary>Fee billed on claim should be the UCR fee for the patient's provider.</summary>
		public bool ClaimsUseUCR;
		///<summary>FK to feesched.FeeSchedNum. Not usually used. This fee schedule holds only co-pays(patient portions).  Only used for Capitation or for fixed copay plans.</summary>
		public long CopayFeeSched;
		///<summary>FK to employer.EmployerNum.</summary>
		public long EmployerNum;
		///<summary>FK to carrier.CarrierNum.</summary>
		public long CarrierNum;
		///<summary>FK to feesched.FeeSchedNum.  Not usually used.  This fee schedule holds amounts allowed by carriers.  Always represents a feesched of type OutOfNetwork.</summary>
		public long AllowedFeeSched;
		///<summary>.</summary>
		public string TrojanID;
		///<summary>Only used in Canada. It's a suffix to the plan number (group number).</summary>
		public string DivisionNo;
		///<summary>True if this is medical insurance rather than dental insurance.  When creating a claim, this, along with pref.</summary>
		public bool IsMedical;
		///<summary>FK to insfilingcode.InsFilingCodeNum.  Used for e-claims.  Also used for some complex reports in public health.  The e-claim usage might become obsolete when PlanID implemented by HIPAA.  Can be 0 to indicate none.  Then 'CI' will go out on claims.</summary>
		public long FilingCode;
		///<summary>Canadian e-claim field. D11 and E07.  Zero indicates empty.  Mandatory value for Dentaide.  Not used for all others.  2 digit.</summary>
		public byte DentaideCardSequence;
		///<summary>If checked, the units Qty will show the base units assigned to a procedure on the claim form.</summary>
		public bool ShowBaseUnits;
		///<summary>Set to true to not allow procedure code downgrade substitution on this insurance plan.</summary>
		public bool CodeSubstNone;
		///<summary>Set to true to hide it from the pick list and from the main list.</summary>
		public bool IsHidden;
		///<summary>The month, 1 through 12 when the insurance plan renews.  It will renew on the first of the month.  To indicate calendar year, set renew month to 0.</summary>
		public byte MonthRenew;
		///<summary>FK to insfilingcodesubtype.InsFilingCodeSubtypeNum</summary>
		public long FilingCodeSubtype;
		///<summary>Canadian C12.  Single char, usually blank.  If non-blank, then it's one of three kinds of Provincial Medical Plans.  A=Newfoundland MCP Plan.  V=Veteran's Affairs Plan.  N=NIHB.  N and V are not yet in use, so they will result in blank being sent instead.  See Elig5.</summary>
		public string CanadianPlanFlag;
		///<summary>Canadian C39. Required when CanadianPlanFlag is 'A'.</summary>
		public string CanadianDiagnosticCode;
		///<summary>Canadian C40. Required when CanadianPlanFlag is 'A'.</summary>
		public string CanadianInstitutionCode;
		///<summary>BIN location number.  Only used with EHR.</summary>
		public string RxBIN;
		///<summary>Enum:EnumCobRule 0=Basic, 1=Standard, 2=CarveOut. </summary>
		public EnumCobRule CobRule;
		///<summary>FK to sop.SopCode. Examples: 121, 3115, etc.  Acts as default for all patients using this insurance.  When code is changed for an insplan, it should change automatically for patients having that primary insurance. </summary>
		public string SopCode;
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
		///<summary>Is false if this plan needs to be verified.</summary>
		public bool HideFromVerifyList;
		///<summary>Enum:OrthoClaimType 0=InitialClaimOnly, 1=InitialPlusVisit, 2=InitialPlusPeriodic. 
		///If this is an ortho claim, dictates what type of Ortho claim it is.</summary>
		public OrthoClaimType OrthoType;
		///<summary>Enum:OrthoAutoProcFrequency The frequency that the automatic procedures and claims are created for insplans with an InitialPlusPeriodic OrthoType</summary>
		public OrthoAutoProcFrequency OrthoAutoProcFreq;
		///<summary>If 0, this insplan uses the OrthoAutoProc preference.  Otherwise, this overrides that value.</summary>
		public long OrthoAutoProcCodeNumOverride;
		///<summary>The amount that the ortho auto procedure will bill to insurance by default. Overridden by patplan.OrthoAutoFeeBilledOverride.</summary>
		public double OrthoAutoFeeBilled;
		///<summary>Usually 0 or 30. Number of days that should pass after the initial banding that an automatic Ortho claim/procedure are generated.</summary>
		public int OrthoAutoClaimDaysWait;
		///<summary>FK to definition.DefNum.</summary>
		public long BillingType;
		///<summary>True by default.  When a plan allows downgrading procedures and this field is false, the writeoff will be $0 and the difference
		///between the proc fee and the insurance estimate will be the patient portion.</summary>
		public bool HasPpoSubstWriteoffs=true;
		///<summary>Enum:ExclusionRule The exclusion rule for this insurance plan.  PracticeDefault by default.  It determines how insurance plans should behave with Exclusions.</summary>
		public ExclusionRule ExclusionFeeRule;
		///<summary>FK to feesched that has a FeeSchedType of 4-ManualBlueBook. Optional, can be 0.</summary>
		public long ManualFeeSchedNum;
		///<summary>determines if the plan is going to have BlueBook Enabled or not</summary>
		public bool IsBlueBookEnabled=true;

		///<summary>This is not a database column.  It is just used to display the number of plans with the same info.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public int NumberSubscribers;

		/*
		///<summary>IComparable.CompareTo implementation.  This is used to determine if plans are identical.  The criteria is that they have 6 fields in common: Employer, Carrier, GroupName, GroupNum, DivisionNo, and IsMedical.  There is no less than or greater than; just not equal.</summary>
		public long CompareTo(object obj) {
			if(!(obj is InsPlan)) {
				throw new ArgumentException("object is not an InsPlan");
			}
			InsPlan plan=(InsPlan)obj;
			if(plan.EmployerNum==EmployerNum
				&& plan.CarrierNum==CarrierNum
				&& plan.GroupName==GroupName
				&& plan.GroupNum==GroupNum
				&& plan.DivisionNo==DivisionNo
				&& plan.IsMedical==IsMedical)
			{
				return 0;//they are the same
			}
			return -1;
		}*/

		///<summary>Returns a copy of this InsPlan.</summary>
		public InsPlan Copy(){
			return (InsPlan)this.MemberwiseClone();
		}

		

	}

	///<summary></summary>
	public enum EnumCobRule {
		///<summary>0=Basic</summary>
		Basic,
		///<summary>1=Standard</summary>
		Standard,
		///<summary>2=CarveOut</summary>
		CarveOut,
		///<summary>3=SecondaryMedicaid. The secondary insurance will reduce what it pays by what primary pays (like Basic). Then anything that would be the
		///patient portion is a writeoff for the secondary insurance. Sometimes Medicaid is required to be the primary, so only use this if you are sure
		///you are allowed to.</summary>
		SecondaryMedicaid,
	}

	///<summary></summary>
	public enum ExclusionRule {
		///<summary>0=Practice Default</summary>
		[Description("Practice Default")]
		PracticeDefault,
		///<summary>1=Do Nothing</summary>
		[Description("Do Nothing")]
		DoNothing,
		///<summary>2=Use UCR Fee</summary>
		[Description("Use UCR Fee")]
		UseUcrFee,
	}

	///<summary></summary>
	public enum OrthoClaimType {
		///<summary>Payment schedule to be determined after EOB is received.</summary>
		[Description("Initial Claim Only")]
		InitialClaimOnly,
		///<summary>D8080 submitted on initial banding with D8030 or D8670 submitted per visit.</summary>
		[Description("Initial Plus Visit")]
		InitialPlusVisit,
		///<summary>D8080 submitted on initial banding and OrthoAutoProc (usually D8670.auto) submitted at a set frequency regardless of visits.
		///Actual visits should use D8670 and be marked 'DoNotBillIns.'</summary>
		[Description("Initial Plus Periodic")]
		InitialPlusPeriodic
	}

	///<summary></summary>
	public enum OrthoAutoProcFrequency {
		///<summary></summary>
		Monthly,
		///<summary>Every three months.</summary>
		Quarterly,
		///<summary>Every six months.</summary>
		SemiAnnual,
		///<summary></summary>
		Annual
	}


}













