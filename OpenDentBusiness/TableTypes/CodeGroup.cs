using System;

namespace OpenDentBusiness{
	//
	//Job F42790
	//Also need to add benefit.CodeGroupNum
	//A very complex converion script will need to replace benefit CodeNums with CodeGroupNums for all the benefits that are "frequencies".
	//Proposed conversion script:
	//No conversion script needed for InsHist. This is just for benefit Frequency limitations.
	//If any prefs are empty, fill them with codes. This covers the "default fallback" hard coded codes that are seen in many places.
	//Use a query to get a table of all benefits that meet the specific criteria for frequency limitation.
	//  Loop:
	//     Match up the procCode that's acting as a "group" and match it to an actual group.
	//     Convert the CodeNum to a CodeGroupNum
	//
	//
	/// <summary>These groups of procedure codes are used in Benefit Frequencies and Insurance History. We can't use CovCats because those spans are frequently far too broad.  We often need specific codes. Cached.</summary>
	[Serializable()]
	public class CodeGroup:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CodeGroupNum;
		///<summary>.</summary>
		public string GroupName;
		///<summary>list of D codes. Comma delimited, no spaces.</summary>
		public string ProcCodes;//We could later add support for ranges with hyphens, intermingled with commas.
		///<summary>Usually blank. Only used when there is a single procCode that we also want to restrict by quadrant, like SRP. Allowed values are "UL","LR", etc.</summary>
		public string Quadrant;
		///<summary>0-based allows ordering list. There's just one order, even though this becomes two lists in practice.</summary>
		public int ItemOrder;
		///<summary>Enum:EnumCodeGroupFixed 0=None,BW,PanoFMX,Exam,Perio,Prophy,SRP. This allows three groups to be shown in FormInsBenefits and also six are used in sheet static text fields.</summary>
		public EnumCodeGroupFixed CodeGroupFixed;//this could be improved later, but it meets our current needs.
		///<summary>If true, then this code group will be shown in FormBenefitFrequencies.</summary>
		public bool ShowInBenefitFreq;
		///<summary>If true, then this code group will be shown in FormInsHistSetup.</summary>
		public bool ShowInInsHist;
		//public bool IsHidden;//not needed because you can instead set both of the Show booleans to false.
	}

	///<summary>This enum replaces FrequencyType enum.  Used to show the first three in FormInsBenefits in the Frequencies section.  All six are used in sheets. For example StaticTextField.dateLastBW.</summary>
	public enum EnumCodeGroupFixed{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		BW,
		///<summary>2</summary>
		PanoFMX,
		///<summary>3</summary>
		Exam,
		///<summary>4</summary>
		Perio,
		///<summary>5</summary>
		Prophy,
		///<summary>6</summary>
		SRP
	}
}










