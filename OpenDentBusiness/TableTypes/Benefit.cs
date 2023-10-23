using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	/// <summary>Corresponds to the benefit table in the database which replaces the old covpat table.  A benefit is usually a percentage, deductible, 
	/// limitation, max, or similar. Each row represents a single benefit.  A benefit can have a value in EITHER PlanNum OR PatPlanNum.  If it is for a 
	/// PlanNum, the most common, then the benefit is attached to an insurance plan.  If it is for a PatPlanNum, then it overrides the plan benefit, 
	/// usually a percentage, for a single patient.  Benefits we can't handle yet include posterior composites, COB duplication, amounts used, in/out 
	/// of plan network, authorization required, missing tooth exclusion, and any date related limitations like waiting periods.<br/>
	/// <para>Here are examples of typical usage which parallel X12 usage.</para>
	/// <para>Example fields shown in this order:</para>
	/// <para>CovCat, ProcCode(- indicates blank), BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CoverageLevel</para>
	/// <para>Annual Max Indiv $1000: None/General,-,Limitations,-1,1000,CalendarYear,None,0,Individual</para>
	/// <para>Restorative 80%: Restorative,-,CoInsurance,80,-1,CalendarYear,None,0,None</para>
	/// <para>$50 deductible: None/General,-,Deductible,-1,50,CalendarYear,None,0,Individual</para>
	/// <para>Deductible waived on preventive: Preventive,-,Deductible,-1,0,CalendarYear,None,0,Individual</para>
	/// <para>1 pano every 5 years: None,D0330,Limitations,-1,-1,Years?,Years,5,None</para>
	/// <para>2 exams per year: Preventive(or Diagnostic),-,Limitations,-1,-1,BenefitYear,NumberOfServices,2,None</para>
	/// <para>Fluoride limit 18yo: None, D1204, Limitations, -1, -1, CalendarYear/None, AgeLimit, 18,None (might require a second identical entry for D1205)</para>
	/// <para>4BW every 6 months: None, D0274, Limitations, -1, -1, None, Months, 6,None.</para>
	/// <para>The text above might be difficult to read.  We are trying to improve the white spacing.</para></summary>
	[Serializable()]
	public class Benefit:TableBase, IComparable {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long BenefitNum;
		///<summary>FK to insplan.PlanNum.  Most benefits should be attached using PlanNum.  The exception would be if each patient has a different percentage.  If PlanNum is used, then PatPlanNum should be 0.</summary>
		public long PlanNum;
		///<summary>FK to patplan.PatPlanNum.  It is rare to attach benefits this way.  Usually only used to override percentages for patients.   In this case, PlanNum should be 0.</summary>
		public long PatPlanNum;
		///<summary>FK to covcat.CovCatNum.  Corresponds to X12 EB03- Service Type code.  Situational, so it can be 0.  Will probably be 0 for general deductible and annual max.  There are very specific categories covered by X12. Users should set their InsCovCats to the defaults we provide.</summary>
		public long CovCatNum;
		///<summary>Enum:InsBenefitType Corresponds to X12 EB01. Examples: 0=ActiveCoverage, 1=CoInsurance, 2=Deductible, 3=CoPayment, 4=Exclusions, 5=Limitations. ActiveCoverage doesn't really provide meaningful information.</summary>
		public InsBenefitType BenefitType;
		///<summary>Only used if BenefitType=CoInsurance.  Valid values are 0 to 100.  -1 indicates empty, which is almost always true if not CoInsurance.  The percentage that insurance will pay on the procedure.  Note that benefits coming from carriers are usually backwards, indicating the percentage that the patient is responsible for.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TinyIntSigned)]
		public int Percent;
		///<summary>Used for CoPayment, Limitations, and Deductible.  -1 indicates empty</summary>
		public double MonetaryAmt;
		///<summary>Enum:BenefitTimePeriod Corresponds to X12 EB06, Time Period Qualifier.  Examples: 0=None,1=ServiceYear,2=CalendarYear,3=Lifetime,4=Years. Might add Visit and Remaining.</summary>
		public BenefitTimePeriod TimePeriod;
		///<summary>Enum:BenefitQuantity Corresponds to X12 EB09. Not used very much. Examples: 0=None,1=NumberOfServices,2=AgeLimit,3=Visits,4=Years,5=Months</summary>
		public BenefitQuantity QuantityQualifier;
		///<summary>Corresponds to X12 EB10. Qualify the quantity using QuantityQualifier.</summary>
		public byte Quantity;
		///<summary>FK to procedurecode.CodeNum.  Typical uses include fluoride, sealants, etc.  If a specific code is used here, then the CovCat should be None.</summary>
		public long CodeNum;
		///<summary>Enum:BenefitCoverageLevel Corresponds to X12 EB02.  None, Individual, or Family.  Individual and Family are commonly used for deductibles and maximums.  None is commonly used for percentages and copays.</summary>
		public BenefitCoverageLevel CoverageLevel;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>FK to codegroup.CodeGroupNum  The group of procedure codes that apply to this frequency limitation benefit.</summary>
		public long CodeGroupNum;
		///<summary>Enum:TreatmentArea . Only for frequency limitations, ignored for all other benefits. Enforced by the UI. Example 3 fillings per year [mouth].</summary>
		public TreatmentArea TreatArea;

		public Benefit() {
			Percent=-1;
			MonetaryAmt=-1;
		}
		
		public string ToString(bool isLeadingIncluded=false) {
			return ToStringHelper(isLeadingIncluded);
		}

		public override string ToString() {
			return ToStringHelper();
		}

		private string ToStringHelper(bool isLeadingIncluded=false) {
			//return base.ToString();
			string retVal="";
			retVal+=BenefitType.ToString();//EB01: Eligibility or benefit information. Required
			if(CoverageLevel !=BenefitCoverageLevel.None) {
				retVal+=", "+CoverageLevel.ToString();//EB02: Coverage level code. Situational
			}
			EbenefitCategory ebenCat=CovCats.GetEbenCat(CovCatNum);
			if(ebenCat != EbenefitCategory.None) {
				retVal+=", "+ebenCat.ToString();//EB03: Service type code. Situational
			}
			//EB04: Insurance type code. Situational.  Not a Benefit field.  We treat it as plan level.
			//EB05: Plan coverage description. Situational.  Not a Benefit field.  We treat it as plan level.
			if(TimePeriod != BenefitTimePeriod.None) {
				retVal+=", "+TimePeriod.ToString();//EB06: Time period qualifier. Situational
			}
			if(MonetaryAmt != -1) {
				retVal+=", "+MonetaryAmt.ToString("c");//EB07: Monetary amount. Situational
			}
			if(Percent != -1) {
				string leadingStr="";
				if(isLeadingIncluded) {
					leadingStr="Insurance Pays ";
				}
				retVal+=", "+leadingStr+Percent.ToString()+"%";//EB08: Percent. Situational
			}
			if(QuantityQualifier != BenefitQuantity.None) {
				retVal+=", "+QuantityQualifier.ToString();//EB09: Quantity qualifier. Situational
			}
			if(Quantity != 0) {
				retVal+=", "+Quantity.ToString();//EB10: Quantity. Situational
			}
			//EB11: Authorization Required. Situational.  Not a Benefit field.
			//EB12: In plan network.  Situational.  Not a Benefit field.
			//EB13:  Procedure identifier. Situational.  We don't import this from EB, but we can show it anyway.
			if(CodeNum != 0) {
				ProcedureCode proccode=ProcedureCodes.GetProcCode(CodeNum);
				retVal+=", "+proccode.ProcCode+" - "+proccode.AbbrDesc;
			}
			return retVal;
		}

		/// <summary>
		///Tests MonteraryAmt for how many decimal values it has and then returns the amount
		///formatted with the ToString() "n" operators to ensure it is percise to the nearest decimal value.
		///This does not work with commas, which are used for other culture types.
		///Returns blank string if MonetaryAmt is -1.
		/// </summary>
		public string GetDisplayMonetaryAmt() {
			if(MonetaryAmt==-1) {
				 return"";
			}
			double amount=Math.Round(MonetaryAmt,2);//Handles partial cents.
			//E.X. 4.25 => {"4","25"} OR 4.00 => {"4"}
			List<string> listVals=amount.ToString().Split('.').ToList();
			int count=listVals.Count()>1 ? listVals.ElementAt(1).Length : 0;
			if(count==0) {//No decimals
				return amount.ToString("n0");//Do not show decimal, but round up.
			}//decimals present
			return amount.ToString("n2");//Show two decimals, round up.
		}

		///<summary>IComparable.CompareTo implementation.  This is used to order benefit lists as well as to group benefits if the type is essentially equal.  It doesn't compare values such as percentages or amounts.  It only compares types.</summary>
		public int CompareTo(object obj) {
			if(!(obj is Benefit)) {
				throw new ArgumentException("object is not a Benefit");
			}
			Benefit ben=(Benefit)obj;
			//Family level benefits should show at the top of the sorting order.
			if(CoverageLevel!=ben.CoverageLevel) {
				return CoverageLevel.CompareTo(ben.CoverageLevel);
			}
			//Benefits of the same type should be grouped up.
			if(BenefitType!=ben.BenefitType) {
				return BenefitType.CompareTo(ben.BenefitType);
			}
			//Compare CovCats if both benefits have a category set.
			//Sort the benefits based off of the CovOrder on their corresponding CovCat when the CovCats are different.
			if(CovCatNum!=0 && ben.CovCatNum!=0 && CovCatNum!=ben.CovCatNum) {
				return CovCats.GetFindIndex(x => x.CovCatNum==CovCatNum).CompareTo(CovCats.GetFindIndex(x => x.CovCatNum==ben.CovCatNum));
			}
			//Compare the ItemOrder of the corresponding code groups if either have one set.
			if(CodeGroupNum!=0 || ben.CodeGroupNum!=0) {
				CodeGroup codeGroup=CodeGroups.GetOne(CodeGroupNum);
				CodeGroup codeGroupBen=CodeGroups.GetOne(ben.CodeGroupNum);
				int itemOrder=codeGroup?.ItemOrder??-1;
				int itemOrderBen=codeGroupBen?.ItemOrder??-1;
				if(itemOrder!=itemOrderBen) {
					return itemOrder.CompareTo(itemOrderBen);
				}
			}
			//ProcCode
			if(CodeNum!=ben.CodeNum) {
				return ProcedureCodes.GetStringProcCode(CodeNum).CompareTo(ProcedureCodes.GetStringProcCode(ben.CodeNum));
			}
			//TimePeriod ServiceYear and CalendarYear are treated as the same.
			//If either are not serviceYear or CalendarYear
			List<BenefitTimePeriod> listTimePeriods=new List<BenefitTimePeriod>(){ BenefitTimePeriod.CalendarYear,BenefitTimePeriod.ServiceYear };
			if((!listTimePeriods.Contains(TimePeriod) && !listTimePeriods.Contains(ben.TimePeriod) && TimePeriod!=ben.TimePeriod)
				|| (listTimePeriods.Contains(TimePeriod)!= listTimePeriods.Contains(ben.TimePeriod))) 
			{
				return TimePeriod.CompareTo(ben.TimePeriod);
			}
			//QuantityQualifier
			if(QuantityQualifier!=ben.QuantityQualifier) {//if different
				return QuantityQualifier.CompareTo(ben.QuantityQualifier);
			}
			//Pat plan overrides should show higher in the sort order.
			if(PatPlanNum==0 && ben.PatPlanNum!=0) {
				return 1;
			}
			//Followed by plan level benefits.
			if(PlanNum==0 && ben.PlanNum!=0) {
				return -1;
			}
			//Last resort.  Can't find any significant differences in the type, so:
			return 0;//then values are the same.
		}

		///<summary></summary>
		public Benefit Copy() {
			return (Benefit)MemberwiseClone();
		}

		///<summary>Returns true if most of the fields match except BenefitNum</summary>
		public bool IsSimilar(Benefit ben){
			if(//PlanNum             != oldBenefitList[i].PlanNum
				//|| PatPlanNum        != oldBenefitList[i].PatPlanNum
					 CovCatNum         != ben.CovCatNum
				|| BenefitType       != ben.BenefitType
				|| Percent           != ben.Percent
				|| MonetaryAmt       != ben.MonetaryAmt
				|| TimePeriod        != ben.TimePeriod
				|| QuantityQualifier != ben.QuantityQualifier
				|| Quantity          != ben.Quantity
				|| CodeNum           != ben.CodeNum 
				|| CoverageLevel     != ben.CoverageLevel
				|| CodeGroupNum      != ben.CodeGroupNum
				|| TreatArea         != ben.TreatArea) 
			{
				return false;
			}
			return true;
		}

		///<summary>Warning, this does not check all fields on the Benefit objects.</summary>
		public override bool Equals(object obj) {
			if(!(obj is Benefit)) {
				return false;
			}
			return (this.CompareTo(obj)==0);
		}

		public override int GetHashCode() {
			int retval=486187739;//Arbitrary prime number
			int prime=104743;
			unchecked {//Overflow is fine, just wrap around
				retval=CovCatNum.GetHashCode()+prime*retval;
				retval=BenefitType.GetHashCode()+prime*retval;
				retval=CoverageLevel.GetHashCode()+prime*retval;
				retval=(TimePeriod==BenefitTimePeriod.CalendarYear ? BenefitTimePeriod.ServiceYear : TimePeriod).GetHashCode()+prime*retval;
				retval=(PatPlanNum==0).GetHashCode()+prime*retval;
				retval=(PlanNum==0).GetHashCode()+prime*retval;
				retval=CodeNum.GetHashCode()+prime*retval;
				return retval;
			}
		}
	}

	/*================================================================================================================
	=========================================== class BenefitArraySorter =============================================*/
	///<summary></summary>
	public class BenefitArraySorter:IComparer {
		///<summary></summary>
		int IComparer.Compare(Object x,Object y) {
			Benefit[] array1=(Benefit[])x;
			Benefit ben1=null;
			for(long i=0;i<array1.Length;i++){
				if(array1[i]==null){
					continue;
				}
				ben1=array1[i].Copy();
				break;
			}
			Benefit[] array2=(Benefit[])y;
			Benefit ben2=null;
			for(int i=0;i<array2.Length;i++) {
				if(array2[i]==null) {
					continue;
				}
				ben2=array2[i].Copy();
				break;
			}
			return(ben1.CompareTo(ben2));
		}

	}

	///<summary>Used in Benefit frequency limitation because using QuantityQualifier and TimePeriod is too hard for user to understand. so this just combines those two fields.</summary>
	public enum FrequencyOptions {
		///<summary>0 - Every # Years</summary>
		[Description("Every # Years")]
		Every_Years,
		///<summary>1 - # Per Benefit Year</summary>
		[Description("# Per Benefit Year")]
		_PerBenefitYear,
		///<summary>2 - Every # Months</summary>
		[Description("Every # Months")]
		Every_Months,
		///<summary>3 - # in Last 12 Months</summary>
		[Description("# in Last 12 Months")]
		_InLast12Months,
	}



		
	

	

	


}










