using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace OpenDentBusiness{

	///<summary>The claim table holds information about individual claims.  Each row represents one claim.</summary>
	[Serializable()]
	[CrudTable(AuditPerms=CrudAuditPerm.ClaimHistoryEdit,IsSecurityStamped=true)]
	public class Claim:TableBase{
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClaimNum;
		///<summary>FK to patient.PatNum.  Must always match claimProc.PatNum</summary>
		public long PatNum;//
		///<summary>Usually the same date as the procedures, but it can be changed if you wish.</summary>
		public DateTime DateService;
		///<summary>Usually the date it was created.  It might be sent a few days later if you don't send your e-claims every day.</summary>
		public DateTime DateSent;
		///<summary>Single char: U,H,W,P,S,or R.  U=Unsent, H=Hold until pri received, W=Waiting in queue, S=Sent, R=Received.  A(adj) is no longer used.  P(prob sent) is no longer used.</summary>
		public string ClaimStatus;
		///<summary>Date the claim was received.</summary>
		public DateTime DateReceived;
		///<summary>FK to insplan.PlanNum.  Every claim is attached to one plan.</summary>
		public long PlanNum;
		///<summary>FK to provider.ProvNum.  Treating provider for dental claims.  For institutional claims, this is called the attending provider.</summary>
		public long ProvTreat;
		///<summary>Total fee of claim.</summary>
		public double ClaimFee;
		///<summary>Amount insurance is estimated to pay on this claim.</summary>
		public double InsPayEst;
		///<summary>Amount insurance actually paid.</summary>
		public double InsPayAmt;
		///<summary>Deductible applied to this claim.</summary>
		public double DedApplied;
		///<summary>The predetermination of benefits number received from ins.  In X12, REF G3.</summary>
		public string PreAuthString;
		///<summary>Single char for No, Initial, or Replacement.</summary>
		public string IsProsthesis;
		///<summary>Date prior prosthesis was placed.  Note that this is only for paper claims.  E-claims have a date field on each individual procedure.</summary>
		public DateTime PriorDate;
		///<summary>Note for patient for why insurance didn't pay as expected.</summary>
		public string ReasonUnderPaid;
		///<summary>Note to be sent to insurance. Max 400 char.  E-claims also have notes on each procedure.</summary>
		public string ClaimNote;
		///<summary>"P"=primary, "S"=secondary, "PreAuth"=preauth, "Other"=other, "Cap"=capitation.  Not allowed to be blank. Might need to add "Med"=medical claim.</summary>
		public string ClaimType;
		///<summary>FK to provider.ProvNum.  Billing provider.  Assignment can be automated from the setup section.</summary>
		public long ProvBill;
		///<summary>FK to referral.ReferralNum.</summary>
		public long ReferringProv;
		///<summary>Referral number for this claim.</summary>
		public string RefNumString;
		///<summary>Enum:PlaceOfService .</summary>
		public PlaceOfService PlaceService;
		///<summary>blank or A=Auto, E=Employment, O=Other.</summary>
		public string AccidentRelated;
		///<summary>Date of accident, if applicable.  Canada only.</summary>
		public DateTime AccidentDate;
		///<summary>Accident state.</summary>
		public string AccidentST;
		///<summary>Enum:YN .</summary>
		public YN EmployRelated;
		///<summary>True if is ortho.</summary>
		public bool IsOrtho;
		///<summary>Remaining months of ortho.  Valid values are 1-36, although we allow greater than or equal to 0.</summary>
		public byte OrthoRemainM;
		///<summary>Date ortho appliance placed.</summary>
		public DateTime OrthoDate;
		///<summary>Enum:Relat  Relationship to subscriber.  The relationship is copied from InsPlan when the claim is created.  It might need to be changed in both places.</summary>
		public Relat PatRelat;
		///<summary>FK to insplan.PlanNum.  Other coverage plan number.  0 if none.  This provides the user with total control over what other coverage shows. This obviously limits the coverage on a single claim to two insurance companies.</summary>
		public long PlanNum2;
		///<summary>Enum:Relat  The relationship to the subscriber for other coverage on this claim.</summary>
		public Relat PatRelat2;
		///<summary>Sum of ClaimProc.Writeoff for this claim.</summary>
		public double WriteOff;
		///<summary>The number of x-rays enclosed.</summary>
		public byte Radiographs;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic.  Since one claim cannot have procs from multiple clinics, the clinicNum is set when creating the claim and then cannot be changed.  The claim would have to be deleted and recreated.  Otherwise, if changing at the claim level, a feature would have to be added that synched all procs, claimprocs, and probably some other tables.</summary>
		public long ClinicNum;
		///<summary>FK to claimform.ClaimFormNum.  0 if not assigned to use the claimform for the insplan.</summary>
		public long ClaimForm;
		///<summary>The number of intraoral images attached.  Not the number of files attached.  This is the value that goes on the 2006 claimform.</summary>
		public int AttachedImages;
		///<summary>The number of models attached.</summary>
		public int AttachedModels;
		///<summary>A comma-delimited set of flag keywords.  Can have one or more of the following: EoB,Note,Perio,Misc.  Must also contain one of these: Mail or Elect.</summary>
		public string AttachedFlags;
		///<summary>Example: NEA#1234567.  If present, and if the claim note does not already start with this Id, then it will be prepended to the claim note for both e-claims and mail.  If using e-claims, this same ID will be used for all PWK segements.</summary>
		public string AttachmentID;
		///<summary>A08.  Any combination of E(email), C(correspondence), M(models), X(x-rays), and I(images).  So up to 5 char.  Gets converted to a single char A-Z for e-claims.</summary>
		public string CanadianMaterialsForwarded;
		///<summary>B05.  Optional. The 9-digit CDA number of the referring provider, or identifier of referring party up to 10 characters in length.</summary>
		public string CanadianReferralProviderNum;
		///<summary>B06.  A number 0(none) through 13.</summary>
		public byte CanadianReferralReason;
		///<summary>F18.  Y, N, or X(not a lower denture, crown, or bridge).</summary>
		public string CanadianIsInitialLower;
		///<summary>F19.  Mandatory if F18 is N.</summary>
		public DateTime CanadianDateInitialLower;
		///<summary>F21.  If crown, not required.  If denture or bridge, required if F18 is N.  Single digit number code, 0-6.  We added type 7, which is crown.</summary>
		public byte CanadianMandProsthMaterial;
		///<summary>F15.  Y, N, or X(not an upper denture, crown, or bridge).</summary>
		public string CanadianIsInitialUpper;
		///<summary>F04.  Mandatory if F15 is N.</summary>
		public DateTime CanadianDateInitialUpper;
		///<summary>F20.  If crown, not required.  If denture or bridge, required if F15 is N.  0 indicates empty response.  Single digit number code, 1-6.  We added type 7, which is crown.</summary>
		public byte CanadianMaxProsthMaterial;
		///<summary>FK to inssub.InsSubNum.</summary>
		public long InsSubNum;
		///<summary>FK to inssub.InsSubNum.  The fk to the 'Other' insurance subscriber.  For a primary claim, this will be the secondary insurance
		///subscriber.  For a secondary claim, this will be primary insurance subscriber.</summary>
		public long InsSubNum2;
		///<summary>G01 assigned by carrier/network and returned in acks.  Used for claim reversal.  For Claim Acknowledgements, this can sometimes
		///be a series of spaces, which means the number is effectively empty.  This happens when the Claim Acknowledgement is forwarded to the carrier
		///as part of a batch.</summary>
		public string CanadaTransRefNum;
		///<summary>F37 Used for predeterminations.</summary>
		public DateTime CanadaEstTreatStartDate;
		///<summary>F28 Used for predeterminations.</summary>
		public double CanadaInitialPayment;
		///<summary>F29 Used for predeterminations.</summary>
		public byte CanadaPaymentMode;
		///<summary>F30 Used for predeterminations.</summary>
		public byte CanadaTreatDuration;
		///<summary>F31 Used for predeterminations.</summary>
		public byte CanadaNumAnticipatedPayments;
		///<summary>F32 Used for predeterminations.</summary>
		public double CanadaAnticipatedPayAmount;
		///<summary>This is NOT the predetermination of benefits number.  In X12, this is REF G1.</summary>
		public string PriorAuthorizationNumber;
		///<summary>Enum:EnumClaimSpecialProgram  This is used to track EPSDT.</summary>
		public EnumClaimSpecialProgram SpecialProgramCode;
		///<summary>A three digit number used on 837I.  Aka Bill Code.  UBO4 4.  Examples: 321,823,131,652.  The third digit is claim frequency code.  If this is used, then our CorrectionType should be 0=original.</summary>
		public string UniformBillType;
		///<summary>Enum:EnumClaimMedType 0=Dental, 1=Medical, 2=Institutional</summary>
		public EnumClaimMedType MedType;
		///<summary>Used for inst claims. Single digit.  X12 2300 CL101.  UB04 14.  Should only be required for IP, but X12 clearly states required for all.</summary>
		public string AdmissionTypeCode;
		///<summary>Used for inst claims. Single char.  X12 2300 CL102.  UB04 15.  Should only be required for IP, but X12 clearly states required for all.</summary>
		public string AdmissionSourceCode;
		///<summary>Used for inst claims. Two digit.  X12 2300 CL103.  UB04 17.  Should only be required for IP, but X12 clearly states required for all.</summary>
		public string PatientStatusCode;
		///<summary>FK to definition.DefNum. Most users will leave this blank.  Some offices may set up tracking statuses such as 'review', 'hold', 'riskmanage', etc.</summary>
		public long CustomTracking;
		///<summary>Used for historical purposes only, not sent electronically. Automatically set when CorrectionType is not original and the claim is resent.</summary>
		public DateTime DateResent;
		///<summary>Enum:ClaimCorrectionType X12 CLM05-3. Usually set to original, but can be used to resubmit claims.  Also used in 1500 Medical Claim Form field 22.</summary>
		public ClaimCorrectionType CorrectionType;
		///<summary>X12 CLM01. Semi-unique identifier for the claim within the current database. Defaults to PatNum/ClaimNum, but can be edited by user, and is often modified by the clearinghouse to ensure uniqueness on their end.
		///This also set for PreAuth claims.  The ClaimIdentifier for a PreAuth will probably not match the ClaimIdentifier for a regular claim, which makes ERA claim matching more straight forward for both PreAuths and regular claims.</summary>
		public string ClaimIdentifier;
		///<summary>X12 2300 REF (F8). Used when resending claims to refer to the original claim. The user must type this value in after reading it from the original claim response report.</summary>
		public string OrigRefNum;
		///<summary>FK to provider.ProvNum.  Ordering provider override.  Goes hand-in-hand with OrderingReferralNum.  Medical eclaims only.
		///Defaults to zero.</summary>
		public long ProvOrderOverride;
		///<summary>Total estimated months of ortho.  Valid values are 1-36, although we allow greater than or equal to 0.</summary>
		public byte OrthoTotalM;
		///<summary>Sum of all amounts paid specifically to this claim by the patient or family.
		///Goes out in X12 4010/5010 loop 2300 AMT segment if greater than zero.  Default value is 0, thus will not go out by default unless the user
		///enters a value.  This field was added for Denti-Cal certification, but can go out for any clearinghouse.</summary>
		public double ShareOfCost;
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
		///<summary>FK to referral.ReferralNum.  Goes hand-in-hand with ProvOrderOverride.  Medical eclaims only.  Defaults to zero.
		///If set, and the ProvOrderOverride is not set, then this referral will go out at the ordering provider on medical e-claims.</summary>
		public long OrderingReferralNum;
		///<summary>The original date the claim was sent.</summary>
		public DateTime DateSentOrig;
		///<summary>Date of Current Illness, Injury, or Pregnancy (LMP).  (LMP = Last Menstrual Period)  For use in 1500 Medical Claim Form box 14.
		///Identifies the first date of onset of illness, the actual date of injury, or the LMP for pregnancy.</summary>
		public DateTime DateIllnessInjuryPreg;
		///<summary>Enum:DateIllnessInjuryPregQualifier 3 digit code used in 1500 Medical Claim Form, 'Qual' box of field 14.  Valid values are 431 or 484.</summary>
		public DateIllnessInjuryPregQualifier DateIllnessInjuryPregQualifier;
		///<summary>Another date related to the patient's condition or treatment.  For use in 1500 Medical Claim Form box 15.</summary>
		public DateTime DateOther;
		///<summary>Enum:DateOtherQualifier 3 digit code used in 1500 Medical Claim Form, 'Qual' box of field 15.  Valid values are 090, 091, 304, 439, 444,
		///453,454, 455, and 471.</summary>
		public DateOtherQualifier DateOtherQualifier;
		///<summary>Used in 1500 Medical Claim Form field 20.  Place an 'X' the 'Yes' if true and the 'No' if false.</summary>
		public bool IsOutsideLab;

		///<summary>Not a data column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<ClaimAttach> Attachments;

		public Claim(){
			AttachedFlags="";
			CanadianMaterialsForwarded="";
			CanadianIsInitialLower="";
			CanadianIsInitialUpper="";
			Attachments=new List<ClaimAttach>();
		}

		///<summary>Returns a copy of the claim.</summary>
		public Claim Copy() {
			Claim c=(Claim)MemberwiseClone();
			c.Attachments=new List<ClaimAttach>();
			for(int i=0;i<Attachments.Count;i++){
				c.Attachments.Add(Attachments[i].Copy());
			}
			return c;
		}

		public override bool Equals(object obj){
			if(obj == null || GetType() != obj.GetType()){
				return false;
			}
			Claim c = (Claim)obj;
			return (ClaimNum == c.ClaimNum);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}

	///<summary></summary>
	public enum EnumClaimMedType {
		///<summary>0</summary>
		Dental,
		///<summary>1</summary>
		Medical,
		///<summary>2</summary>
		Institutional
	}

	///<summary>0=none, 1=EPSDT_1, 2=Handicapped_2, 3=SpecialFederal_3, (no 4), 5=Disability_5, 9=SecondOpinion_9</summary>
	public enum EnumClaimSpecialProgram {
		///<summary></summary>
		none=0,
		///<summary></summary>
		EPSDT_1=1,
		///<summary></summary>
		Handicapped_2=2,
		///<summary></summary>
		SpecialFederal_3=3,
		///<summary></summary>
		Disability_5=5,
		///<summary></summary>
		SecondOpinion_9=9
	}

	///<summary></summary>
	public enum ClaimCorrectionType {
		///<summary>0 - X12 1. Use for claims that are not ongoing.</summary>
		Original,
		///<summary>1 - X12 7. Use to entirely replace an original claim. A claim reference number will be required.</summary>
		Replacement,
		///<summary>2 - X12 8. Use to undo an original claim. A claim reference number will be required.</summary>
		Void
		/////<summary>X12 2. Use for first claim in a series of expected claims.</summary>
		//FirstClaim,
		/////<summary>X12 3. Use when one or more claims for the same ongoing care have already been submitted.</summary>
		//InterimContinuingClaim,
		/////<summary>X12 4. Use for a claim which is the last of a series of claims.</summary>
		//InterimLastClaim,
		/////<summary>X12 5.</summary>
		//LateCharge,
		/////<summary>X12 6. Nobody seems to use this.  What is it?  Use to make a correction to the original claim. A claim reference number will be required.</summary>
		//Corrected,
	}

	///<summary>Used for 1500 Medical Claim Form, 'Qual' box portion of fields 14 and 15.  Populate with 3 digit enum value. Non-standard enum numbering.</summary>
	public enum DateOtherQualifier {
		///<summary>0 - None</summary>
		[Description("None")]
		None=0,
		///<summary>090 - Report Start</summary>
		[Description("Report Start (Assumed Care Date)")]
		ReportStart=090,
		///<summary>091 - Report End</summary>
		[Description("Report End (Relinquished Care Date)")]
		ReportEnd=091,
		///<summary>304 - Latest Visit or Consultation</summary>
		[Description("Latest Visit or Consultation")]
		LatestVisitConsult=304,
		///<summary>439 - Accident</summary>
		[Description("Accident")]
		Accident=439,
		///<summary>444 - First Visit or Consultation</summary>
		[Description("First Visit or Consultation")]
		FirstVisitConsult=444,
		///<summary>453 - Acute Manifestation of a Chronic Condition</summary>
		[Description("Acute Manifestation of a Chronic Condition")]
		ChronicCondManifest=453,
		///<summary>454 - Initial Treatment</summary>
		[Description("Initial Treatment")]
		InitialTreatment=454,
		///<summary>455 - Last X-ray</summary>
		[Description("Last X-ray")]
		LastXray=455,
		///<summary>471 - Prescription</summary>
		[Description("Prescription")]
		Prescription=471
	}

	///<summary>Non-standard enum numbering.</summary>
	public enum DateIllnessInjuryPregQualifier {
		///<summary>0 - None</summary>
		[Description("None")]
		None=0,
		///<summary>431 - Onset of Current Symptoms or Illness</summary>
		[Description("Onset of Current Symptoms or Illness")]
		OnsetCurSymptoms=431,
		///<summary>484 - Last Menstrual Period</summary>
		[Description("Last Menstrual Period")]
		LastMenstrualPeriod=484
	}

}
