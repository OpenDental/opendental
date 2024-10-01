using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	/// <summary>These groups of procedure codes are used in Benefit Frequencies (and Insurance History?). We can't use CovCats because those spans are frequently far too broad.  We often need specific codes. Cached.</summary>
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
		///<summary>Zero-based. CodeGroups that don't show in either list (IsHidden=true and ShowInAgeLimit=false) get higher ItemOrders so that then are at the bottom of the setup list.</summary>
		public int ItemOrder;
		///<summary>Enum:EnumCodeGroupFixed 0=None,BW,PanoFMX,Exam,Perio,Prophy,SRP,FMDebride,Fluoride,Sealant. Six are used in sheet static text fields (example StaticTextField.dateLastBW), and seven are used in Ins History Window.</summary>
		public EnumCodeGroupFixed CodeGroupFixed;//this could be improved later, but it meets our current needs.
		///<summary>If true, this codegroup will be hidden from the frequency limitations grid. Control of showing in age limitations grid is done separately using ShowInAgeLimit.</summary>
		public bool IsHidden;
		///<summary> If true, this codegroup will show in Age Limitations grid. Control of showing in Freq Lim is done separately using IsHidden.</summary>
		public bool ShowInAgeLimit;

		public CodeGroup Copy() {
			return (CodeGroup)this.MemberwiseClone();
		}

		/// <summary> Will return true if it shows in either list.</summary>
		public bool IsVisible() {
			if(ShowInAgeLimit){
				return true;
			}
			if(!IsHidden){
				return true;
			}
			return false;
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







