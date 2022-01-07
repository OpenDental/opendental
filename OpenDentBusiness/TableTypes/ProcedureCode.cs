using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	
	///<summary>A list setup ahead of time with all the procedure codes used by the office.  Every procedurelog entry which is attached to a patient is also linked to this table.</summary>
	[Serializable]
	[CrudTable(AuditPerms=CrudAuditPerm.ProcFeeEdit,HasBatchWriteMethods=true)]
	public class ProcedureCode:TableBase{
		///<summary>Primary Key.  This happened in version 4.8.7.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CodeNum;
		///<summary>Was Primary key, but now CodeNum is primary key.  Can hold dental codes, medical codes, custom codes, etc.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public string ProcCode;
		///<summary>The main description.</summary>
		public string Descript;
		///<summary>Abbreviated description.</summary>
		public string AbbrDesc;
		///<summary>X's and /'s describe Dr's time and assistant's time in the same increments as the user has set.</summary>
		public string ProcTime;
		///<summary>FK to definition.DefNum.  The category that this code will be found under in the search window.  Has nothing to do with insurance categories.</summary>
		//[XmlIgnore]
		public long ProcCat;
		///<summary>Enum:TreatmentArea </summary>
		public TreatmentArea TreatArea;
		///<summary>If true, do not usually bill this procedure to insurance.</summary>
		public bool NoBillIns;
		///<summary>True if Crown,Bridge,Denture, or RPD. Forces user to enter Initial or Replacement and Date.</summary>
		public bool IsProsth;
		///<summary>The default procedure note to copy when marking complete.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DefaultNote;
		///<summary>Identifies hygiene procedures so that the correct provider can be selected.</summary>
		public bool IsHygiene;
		///<summary>No longer used.</summary>
		//[XmlIgnore]
		public int GTypeNum;
		///<summary>For Medicaid.  There may be more later.</summary>
		//[XmlIgnore]
		public string AlternateCode1;
		///<summary>FK to procedurecode.ProcCode.  The actual medical code that is being referenced must be setup first.  Anytime a procedure it added, this medical code will also be added to that procedure.  The user can change it in procedurelog.</summary>
		//[XmlIgnore]
		public string MedicalCode;
		///<summary>Used by some offices.  SalesTaxPercentage has been added to the preference table to store the amount of sales tax to apply as an adjustment attached to a procedurelog entry.</summary>
		//[XmlIgnore]
		public bool IsTaxed;
		///<summary>Enum:ToothPaintingType </summary>
		public ToothPaintingType PaintType;
		///<summary>If set to anything but 0, then this will override the graphic color for all procedures of this code, regardless of the status.</summary>
		[XmlIgnore]
		public Color GraphicColor;
		///<summary>When creating treatment plans, this description will be used instead of the technical description.</summary>
		//[XmlIgnore]
		public string LaymanTerm;
		///<summary>Only used in Canada.  Set to true if this procedure code is only used as an adjunct to track the lab fee.</summary>
		//[XmlIgnore]
		public bool IsCanadianLab;
		///<summary>This is true if this procedure code existed before ADA code distribution changed at version 4.8, false otherwise.</summary>
		//[XmlIgnore]
		public bool PreExisting;
		///<summary>Support for Base Units for a Code (like anesthesia).  Should normally be zero.</summary>
		//[XmlIgnore]
		public int BaseUnits;
		///<summary>FK to procedurecode.ProcCode.  Used for posterior composites because insurance substitutes the amalgam code when figuring the coverage.</summary>
		//[XmlIgnore]
		public string SubstitutionCode;
		///<summary>Enum:SubstitutionCondition Used so that posterior composites only substitute if tooth is molar.  Ins usually pays for premolar composites.</summary>
		//[XmlIgnore]
		public SubstitutionCondition SubstOnlyIf;
		///<summary>Last datetime that this row was inserted or updated.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Set to true if the procedure takes more than one appointment to complete.</summary>
		public bool IsMultiVisit;
		///<summary>11 digits or blank, enforced.  For 837I</summary>
		public string DrugNDC;
		///<summary>Gets copied to procedure.RevCode.  For 837I</summary>
		public string RevenueCodeDefault;
		///<summary>FK to provider.ProvNum.  0 for none. Otherwise, this provider will be used for this code instead of the normal provider.</summary>
		public long ProvNumDefault;
		///<summary>For Canadian customers, tracks scaling insurance and periodontal scaling units for patients depending on coverage.</summary>
		public double CanadaTimeUnits;
		///<summary>Not a database column.  Only used for xml import function.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private string procCatDescript;
		///<summary>Set to true for radiology procedures.  An EHR core measure uses this flag to help determine the denominator for rad orders.</summary>
		public bool IsRadiology;
		///<summary>Default note inserted to claim note when claim is created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DefaultClaimNote;
		///<summary>The default procedure note used when creating a new treatment planned procedure.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DefaultTPNote;
		///<summary>Enum:BypassLockStatus Specifies whether a proceduce with this code can be created before the global lock date. The only values that
		///should be used for this field are NeverBypass and BypassIfZero.</summary>
		public BypassLockStatus BypassGlobalLock;
		///<summary>Used for Sales Tax, this is the corresponding tax code we will send to Avalara API so they can determine how much sales tax to charge
		///for this procedure.</summary>
		public string TaxCode;
		///<summary>The text to draw on the tooth for paint type Text.</summary>
		public string PaintText;
		///<summary>This is an adjunct to TreatArea. If Quad or Arch, then this allows users to also specify a tooth or tooth range.  Required by some insurance.</summary>
		public bool AreaAlsoToothRange;

		public ProcedureCode(){
			ProcTime="/X/";
			//procCode.ProcCat=Defs.Short[(long)DefCat.ProcCodeCats][0].DefNum;
			GraphicColor=Color.FromArgb(0);
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("GraphicColor",typeof(int))]
		public int GraphicColorXml {
			get {
				return GraphicColor.ToArgb();
			}
			set {
				GraphicColor=Color.FromArgb(value);
			}
		}

		//[XmlElement(DataType="string",ElementName="ProcCatDescript")]
		[XmlIgnore]
		public string ProcCatDescript{
			get{
				if(ProcCat==0){//only used in xml import. We have an incomplete object.
					return procCatDescript;
				}
				return Defs.GetName(DefCat.ProcCodeCats,ProcCat);
			}
			set{
				procCatDescript=value;
			}
		}

		///<summary>Returns a copy of this Procedurecode.</summary>
		public ProcedureCode Copy(){
			return (ProcedureCode)this.MemberwiseClone();
		}


	}

	///<summary>The conditions when the global lock date can be bypassed.</summary>
	public enum BypassLockStatus {
		///<summary>0 - Never bypass the lock date.</summary>
		NeverBypass,
		///<summary>1 - Bypass the lock date if the fee is zero.</summary>
		BypassIfZero,
		///<summary>2 - Always bypass the global lock date.</summary>
		BypassAlways
	}

	
	
	


}










