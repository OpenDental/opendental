using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Database table is procedurelog.  A procedure for a patient.  Can be treatment planned or completed.  Once it's completed, it gets tracked more closely by the security portion of the program.  A procedure can NEVER be deleted.  Status can just be changed to "deleted".</summary>
	[Serializable()]
	[CrudTable(TableName="procedurelog",IsDeleteForbidden=true,IsSynchable=true,IsSecurityStamped=true,IsLargeTable=true,UsesDataReader=true,
		HasBatchWriteMethods=true)]
	public class Procedure:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProcNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>FK to appointment.AptNum.  Only allowed to attach proc to one appt(not counting planned appt)</summary>
		public long AptNum;
		///<summary>No longer used.</summary>
		public string OldCode;
		///<summary>Procedure date that will show in the account as the date performed.  If just treatment planned, the date can be the date it was tp'd, or the date can be min val if we don't care.  Also see ProcTime column.</summary>
		public DateTime ProcDate;
		///<summary>Procedure fee.</summary>
		public double ProcFee;
		///<summary>Surfaces, or use "UL" etc for quadrant, "2" etc for sextant, "U","L" for arches.  Sextants in the United States are: 1 (Upper Right), 2 (Upper Anterior), 3 (Upper Left), 4 (Lower Left), 5 (Lower Anterior), 6 (Lower Right).  In Canada, Sextants are 03 through 08 (add 2 to the US sextant and prepend a zero).</summary>
		public string Surf;
		///<summary>May be blank, otherwise 1-32, 51-82, A-T, or AS-TS, 1 or 2 char.  For Canadian users, using FDI nomenclature, we use 51 as a placeholder for supernumerary teeth, which is tooth number 99 according to CDHA standards (2/17/2014).  Logic for this is handled in the tooth logic class.</summary>
		public string ToothNum;
		///<summary>May be blank, otherwise is series of toothnumbers separated by commas.  Tooth numbers 1-32 or A-T.  Supernumeraries not supported here yet.</summary>
		public string ToothRange;
		///<summary>FK to definition.DefNum, which contains the text of the priority.</summary>
		public long Priority;
		///<summary>Enum:ProcStat TP=1,Complete=2,Existing Cur Prov=3,Existing Other Prov=4,Referred=5,Deleted=6,Condition=7.</summary>
		public ProcStat ProcStatus;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>FK to definition.DefNum, which contains text of the Diagnosis.</summary>
		public long Dx;
		///<summary>FK to appointment.AptNum.  Was called NextAptNum in older versions.  Allows this procedure to be attached to a Planned appointment as well as a standard appointment.</summary>
		public long PlannedAptNum;
		///<summary>Enum:PlaceOfService  Only used in Public Health. Zero(Office) until procedure set complete. Then it's set to the value of the DefaultProcedurePlaceService preference.</summary>
		public PlaceOfService PlaceService;
		///<summary>Single char. Blank=no, I=Initial, R=Replacement.</summary>
		public string Prosthesis;
		///<summary>For a prosthesis Replacement, this is the original date.</summary>
		public DateTime DateOriginalProsth;
		///<summary>This note goes out on e-claims.  Not visible in Canada.</summary>
		public string ClaimNote;
		///<summary>This is the date this procedure was entered or set complete.  If not status C, then the value is ignored.  This date is set automatically when Insert, but older data or converted data might not have this value set.  It gets updated when set complete.  User never allowed to edit.  This will be enhanced later.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntryEditable)]
		public DateTime DateEntryC;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic.</summary>
		public long ClinicNum;
		///<summary>FK to procedurecode.ProcCode. Optional.</summary>
		public string MedicalCode;
		///<summary>Simple text for ICD-9 code. Gets sent with medical claims.</summary>
		public string DiagnosticCode;
		///<summary>Set true if this medical diagnostic code is the principal diagnosis for the visit.  If no principal diagnosis is marked for any procedures on a medical e-claim, then it won't be allowed to be sent.  If more than one is marked, then it will just use one at random.</summary>
		public bool IsPrincDiag;
		///<summary>FK to procedurelog.ProcNum. Only used in Canada. If not zero, then this proc is a lab fee and this indicates to which actual procedure the lab fee is attached.  For ordinary use, they are treated like two separate procedures.  It's only for insurance claims that we need to know which lab fee belongs to which procedure.  Two lab fees may be attached to one procedure.</summary>
		public long ProcNumLab;
		///<summary>FK to definition.DefNum. Lets some users track charges for certain types of reports.  For example, a Medicaid billing type could be assigned to a procedure, flagging it for inclusion in a report mandated by goverment.  Would be more useful if it was automated to flow down based on insurance plan type, but that can be added later.  Not visible if prefs.EasyHideMedicaid is true.</summary>
		public long BillingTypeOne;
		///<summary>FK to definition.DefNum.  Same as BillingTypeOne, but used when there is a secondary billing type to account for.</summary>
		public long BillingTypeTwo;
		///<summary>FK to procedurecode.CodeNum</summary>
		public long CodeNum;
		///<summary>Modifier for certain CPT codes.</summary>
		public string CodeMod1;
		///<summary>Modifier for certain CPT codes.</summary>
		public string CodeMod2;
		///<summary>Modifier for certain CPT codes.</summary>
		public string CodeMod3;
		///<summary>Modifier for certain CPT codes.</summary>
		public string CodeMod4;
		///<summary>NUBC Revenue Code for medical/inst billing. Used on UB04 and 837I.</summary>
		public string RevCode;
		///<summary>Default is 1.  Becomes Service Unit Count on institutional UB claimforms SV205.  Becomes Service Unit Count on medical 1500 claimforms SV104.  Becomes procedure count on dental claims SV306.  Gets multiplied by fee in all accounting calculations.</summary>
		public int UnitQty;
		///<summary>Base units used for some billing codes.  Default is 0.  No UI for this field.  It is only edited in the ProcedureCode window.  The database maint tool changes BaseUnits of all procedures to match that of the procCode.  Not sure yet what it's for.</summary>
		public int BaseUnits;
		///<summary>Start time in military.  No longer used, but not deleting just in case someone has critical information stored here.</summary>
		public int StartTime;
		///<summary>Stop time in military.  No longer used, but not deleting just in case someone has critical information stored here.</summary>
		public int StopTime;
		///<summary>The date that the procedure was originally treatment planned.  Does not change when marked complete.</summary>
		public DateTime DateTP;
		///<summary>FK to site.SiteNum.</summary>
		public long SiteNum;
		///<summary>Set to true to hide the chart graphics for this procedure.  For example, a crown was done, but then tooth extracted.</summary>
		public bool HideGraphics;
		///<summary>F16, up to 5 char. One or more of the following: A=Repair of a prior service, B=Temporary placement, C=TMJ, E=Implant, L=Appliance lost, S=Appliance stolen, X=none of the above.  Blank is equivalent to X for claim output, but one value will not be automatically converted to the other in this table.  That will allow us to track user entry for procedurecode.IsProsth.</summary>
		public string CanadianTypeCodes;
		///<summary>Used to be part of the ProcDate, but that was causing reporting issues.</summary>
		[XmlIgnore]
		public TimeSpan ProcTime;
		///<summary>Marks the time a procedure was finished.</summary>
		[XmlIgnore]
		public TimeSpan ProcTimeEnd;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>FK to definition.DefNum, which contains text of the Prognosis.</summary>
		public long Prognosis;
		///<summary>Enum:EnumProcDrugUnit For 837I and UB04</summary>
		public EnumProcDrugUnit DrugUnit;
		///<summary>Includes fractions. For 837I</summary>
		public float DrugQty;
		///<summary>Enum:ProcUnitQtyType For dental, the type is always sent electronically as MultiProcs. For institutional SV204, Days will be sent electronically if chosen, otherwise ServiceUnits will be sent. For medical SV103, MinutesAnesth will be sent electronically if chosen, otherwise ServiceUnits will be sent.</summary>
		public ProcUnitQtyType UnitQtyType;
		///<summary>FK to statement.StatementNum.  Only used when the statement in an invoice.</summary>
		public long StatementNum;
		///<summary>If this flag is set, then the proc is locked down tight.  No changes at all can be made except to append, sign, or invalidate. Invalidate really just sets the proc to status 'deleted'.  An invalidated proc retains its IsLocked status.  All locked procs will be status of C or D.  Locked group notes will be status of EC or D.</summary>
		public bool IsLocked;
		///<summary>A note that will show directly in the Account module.  Also used for repeating charges. Helps distinguish between charges for the same proccode in the same month.</summary>
		public string BillingNote;
		///<summary>FK to repeatcharge.RepeatChargeNum.  Used in repeating charges to determine which procedures belong to each repeating charge. If the
		///repeat charge that this RepeatChargeNum points to is deleted, this column will not be set to 0 so that a record will still exist that this
		///procedure came from a repeat charge.</summary>
		public long RepeatChargeNum;
		///<summary>Simple text for ICD-9 code. Gets sent with medical claims.</summary>
		public string DiagnosticCode2;
		///<summary>Simple text for ICD-9 code. Gets sent with medical claims.</summary>
		public string DiagnosticCode3;
		///<summary>Simple text for ICD-9 code. Gets sent with medical claims.</summary>
		public string DiagnosticCode4;
		///<summary>Stores the dollar amount of the discount, not full price.  E.g.  for a 10% discount, Fee = $160 Discount = $16.  This column is used by treatment planned procedures to create an adjustment when set complete.  It should not be used as an accurate monetary discount value for completed procedures.</summary>
		public double Discount;
		///<summary>Some procedures require a SNOMED code which indicates that site on the body at which this procedure was performed.</summary>
		public string SnomedBodySite;
		///<summary>FK to provider.ProvNum.  Ordering provider override.  Goes hand-in-hand with OrderingReferralNum.  Medical eclaims only.
		///Defaults to zero.</summary>
		public long ProvOrderOverride;
		///<summary>For prosthesis replacement procedures on 5010 eclaims only.  If true, indicates that the DateOriginalProsth is an estimated date.  Estimated dates are often used when the original prosthesis was performed by another doctor.</summary>
		public bool IsDateProsthEst;
		///<summary>The ICD code version for all diagnosis codes on this procedure, including DiagnosisCode, DiagnosisCode2, DiagnosisCode3,
		///and DiagnosisCode4.  Value of 9 for ICD-9, 10 for ICD-10, etc.  Default value is 9.  This value is copied from the DxIcdVersion preference
		///when a procedure is created.  The user can also manually change the IcdVersion on individual procedures.</summary>
		public byte IcdVersion;
		///<summary>Procedures will be flagged as CPOE (Computerized Provider Order Entry) if this procedure was created by a provider.
		///If a provider views, edits, or has any interaction with this procedure after its creation, it will be flagged as IsCPOE.
		///Also, there will be a helpful window where providers can go to to "approve" non-CPOE procedures and mark them as CPOE to help meet EHR measures.
		///If a staff person is logged in and enters this procedure then this is non-CPOE, so false.</summary>
		public bool IsCpoe;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		//No SecDateTEdit, DateTStamp already exists and is the timestamp updated by MySQL when a row is added or changed
		///<summary>The date the procedure was originally set complete. If status is set complete and then set to something other than complete, this field will be set to DateTime.MinValue if DateComplete is today.  If DateComplete is set to a day in the past and the status is changed from complete to something else, the field will not be cleared or updated.  Db only field used by one customer and this is how they requested it. PatNum #19191</summary>
		public DateTime DateComplete;
		///<summary>FK to referral.ReferralNum.  Goes hand-in-hand with ProvOrderOverride.  Medical eclaims only.  Defaults to zero.
		///If set, and the ProvOrderOverride is not set, then this referral will go out at the ordering provider on medical e-claims.</summary>
		public long OrderingReferralNum;

		///<summary>Not a database column.  Saved in database in the procnote table.  This note is only the most recent note from that table.  If user changes it, then the business layer handles it by adding another procnote to that table.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string Note="";
		///<summary>Not a database column.  Just used for now to set the user so that it can be saved with the ProcNote.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public long UserNum;
		///<summary>Not a database column.  If viewing an individual procedure, then this will contain the encrypted signature.  If viewing a procedure list, this will typically just contain an "X" if a signature is present.  If user signs note, the signature will be encrypted before placing into this field.  Then it will be passed down and saved directly as is.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string Signature="";
		///<summary>Not a database column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool SigIsTopaz;
		/// <summary>Not a database column.  This variable is set to true if the procedure is part of a set several procedures. It is used when 
		/// calculating auto codes to correctly determine the right procedure code for the "first" and "eachAdditional" auto code rules.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsAdditional;
		///<summary>Holds the Sales Tax estimate for this procedure.  Becomes a finalized amount when the procedure is marked complete.</summary>
		public double TaxAmt;
		///<summary>Enum:ProcUrgency Used in 1500 Medical Claim Form box 24c. Normal=blank 24c,Emergency='Y' in 24c.</summary>
		public ProcUrgency Urgency;
		///<summary>The difference between the UCR fee and discount plan fee. Frequently recalculated when procedure is TP.</summary>
		public double DiscountPlanAmt;

		private int _priorityOrder=int.MinValue;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ProcTime",typeof(long))]
		public long ProcTimeXml {
			get {
				return ProcTime.Ticks;
			}
			set {
				ProcTime = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ProcTimeEnd",typeof(long))]
		public long ProcTimeEndXml {
			get {
				return ProcTimeEnd.Ticks;
			}
			set {
				ProcTimeEnd = TimeSpan.FromTicks(value);
			}
		}

		///<summary>Gets priority order from Definition cache.</summary>
		public int PriorityOrder {
			get {
				if(_priorityOrder==int.MinValue) {
					_priorityOrder=Defs.GetOrder(DefCat.TxPriorities,this.Priority);
				}
				return _priorityOrder;
			}
		}

		///<summary>Returns the proc fee with quantity taken into account; Procedure Fee * (Base Units + Unit Quantity)</summary>
		public double ProcFeeTotal {
			get {
				return ProcFee * Quantity;
			}
		}

		///<summary>Returns the BaseUnits plus the UnitQty.  Never returns a value less than 1.</summary>
		public double Quantity {
			get {
				return Math.Max(1,BaseUnits + UnitQty);
			}
		}

		public Procedure() {
			UnitQty=1;
			IcdVersion=PrefC.GetByte(PrefName.DxIcdVersion);
		}

		///<summary>Returns a copy of the procedure.</summary>
		public Procedure Copy() {
			return (Procedure)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum EnumProcDrugUnit {
		///<summary>0</summary>
		None,
		///<summary>1 - F2 on UB04.</summary>
		InternationalUnit,
		///<summary>2 - GR on UB04.</summary>
		Gram,
		///<summary>3 - GR on UB04.</summary>
		Milligram,
		///<summary>4 - ML on UB04.</summary>
		Milliliter,
		///<summary>5 - UN on UB04.</summary>
		Unit
	}

	///<summary></summary>
	public enum ProcUnitQtyType {
		///<summary>0-Only allowed on dental, and only option allowed on dental.  This is also the default for all procs in our UI.  For example, 4 PAs all on one line on the e-claim.</summary>
		MultProcs,
		///<summary>1-Only allowed on medical SV103.</summary>
		MinutesAnesth,
		///<summary>2-Allowed on medical SV103 and institutional SV204.  This is the default for both medical and inst when creating X12 claims, regardless of what is set on the proc.</summary>
		ServiceUnits,
		///<summary>3-Only allowed on institutional SV204.</summary>
		Days
	}

	///<summary>Used for marking procedures on medical claims as 'Emergency' on the 1500 claim form.</summary>
	public enum ProcUrgency {
		///<summary>0 - Standard procedure urgency.  Most procedures will have this ProcUrgency.  This will result in the 1500 Medical Claim Form box 24c
		///being blank.  (Normal=blank,Emergency='Y')</summary>
		Normal,
		///<summary>1 - Emergency ProcUrgency is used to populate the 1500 Medical Claim Form box 24c with a 'Y'. (Emergency='Y',Normal=blank)</summary>
		Emergency
	}

}
