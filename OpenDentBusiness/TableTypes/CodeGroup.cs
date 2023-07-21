using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	//
	//Job F42790
	//Also need to add benefit.CodeGroupNum
	//A very complex converion script will need to replace benefit CodeNums with CodeGroupNums for all the benefits that are "frequencies".
	//No conversion script needed for InsHist because we are not touching those prefs.
	//This is just for benefit Frequency limitations.
	//Proposed conversion script:
	//If any prefs are empty, fill them with codes. This covers the "default fallback" hard coded codes that are seen in many places.
	//Use a query to get a table of all benefits that meet the specific criteria for frequency limitation.
	//  Loop:
	//     Match up the procCode that's acting as a "group" and match it to an actual group.
	//     Convert the CodeNum to a CodeGroupNum
	//
	//
	/// <summary>These groups of procedure codes are used in Benefit Frequencies and Insurance History. We can't use CovCats because those spans are frequently far too broad.  We often need specific codes. Cached.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class CodeGroup:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CodeGroupNum;
		///<summary>.</summary>
		public string GroupName;
		///<summary>list of D codes. Comma delimited, no spaces.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string ProcCodes;//We could later add support for ranges with hyphens, intermingled with commas.
		///<summary>0-based.</summary>
		public int ItemOrder;
		///<summary>Enum:EnumCodeGroupFixed 0=None,BW,PanoFMX,Exam,Perio,Prophy,SRP,FMDebride,Fluoride,Sealant. This allows five groups to be shown in the Simplified View of Edit Benefits Window, six are used in sheet static text fields (example StaticTextField.dateLastBW), and seven are used in Ins History Window.</summary>
		public EnumCodeGroupFixed CodeGroupFixed;//this could be improved later, but it meets our current needs.
		///<summary>.</summary>
		public bool IsHidden;

		public CodeGroup Copy() {
			return (CodeGroup)this.MemberwiseClone();
		}
	}

	///<summary>This enum replaces the FrequencyType enum at bottom of Benefits.cs.  See description in CodeGroup.CodeGroupFixed.</summary>
	public enum EnumCodeGroupFixed {
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		[Description("Bitewing")]
		BW,
		///<summary>2</summary>
		[Description("Pano/FMX")]
		PanoFMX,
		///<summary>3</summary>
		[Description("Exam")]
		Exam,
		///<summary>4</summary>
		[Description("Perio Maintenance")]
		Perio,
		///<summary>5</summary>
		[Description("Prophylaxis")]
		Prophy,
		///<summary>6- When used in InsHist window, the quadrant is hard coded for each of the 4 rows.</summary>
		[Description("SRP")]
		SRP,
		///<summary>7</summary>
		[Description("Full Debridement")]
		FMDebride,
		///<summary>8</summary>
		[Description("Fluoride")]
		Fluoride,
		///<summary>9</summary>
		[Description("Sealant")]
		Sealant,
	}
}


/*
Child job:
The following column was originally proposed, but won't work anymore, given the new requirements.
I think it now belongs in the Benefit table.
public TreatmentArea TreatArea;
Enum:TreatmentArea. Entered here as defined by each insurance co. 
Example 1: Ins specifies one denture or partial per [arch] every 5 years.  Patient had a partial with teeth 7,8,9,10 three years ago.  Estimate for new upper denture should be $0. Est for new lower denture: $1000.
Example 2: Ins specifies one crown per [tooth] each 5 years. Patient had a crown on #14 three years ago.  Estimate for new crown on #14 should be $0. Estimate for new crown on #15 should be $800.
Example 3. Ins specifies one composite filling per [tooth] every 5 years. Pt had a comp filling on #29 three years ago. Estimate for comp filling on #29 should be $0. For #30, it should be $300.
Example 4. Ins specifies limit of three fillings per year [mouth].  Patient had two fillings three months ago, Estimate for two or more fillings should cover one but no more in the year.
Example 5. Both 3 and 4 are true for the same insurance.
Example 6. One plan groups exams with limited exams.  Another plan lists them separately.

So it looks like we do need this field, but it belongs in the Benefit table to allow example 5. 
The TreatArea would default can be automated by looking at the first procedurecode in the codegroup.
They could then change it, of course.
To allow example 6, we will allow the office to create an unlimited number of named CodeGroups,
and then they would add them to plans by picking from the list and only using the ones appropriate to that plan.
So you might have one code group for periodic exams, one code group for limited exams, and one code group with both exams in it.

*/





