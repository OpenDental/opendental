using System;

namespace OpenDentBusiness{

	///<summary>Used in public health.  The database table only has 3 columns.  There are 5 additional columns in C# that are not in the databae.  These extra columns are used in the UI to organize input, and are transferred to the screen table as needed.</summary>
	[Serializable]
	public class ScreenGroup:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long ScreenGroupNum;
		///<summary>Up to the user.</summary>
		public string Description;
		///<summary>The date of the screening.</summary>
		public DateTime SGDate;
		///<summary>Required.  Could be the name of the screener and not a provider necessarily.</summary>
		public string ProvName;
		///<summary>FK to provider.ProvNum.  ProvNAME is always entered, but ProvNum supplements it by letting user select from list.
		///When entering a provNum, the name will be filled in automatically.
		///Can be 0 if the provider is not in the list, but provName is required.</summary>
		public long ProvNum;
		///<summary>Enum:PlaceOfService Describes where the screening will take place.</summary>
		public PlaceOfService PlaceService;
		///<summary>FK to county.CountyName, although it will not crash if key absent.</summary>
		public string County;
		///<summary>FK to site.Description, although it will not crash if key absent.</summary>
		public string GradeSchool;
		///<summary>FK to sheetdef.SheetDefNum</summary>
		public long SheetDefNum;

		///<summary>Returns a copy of this ScreenGroup.</summary>
		public ScreenGroup Copy() {
			return (ScreenGroup)this.MemberwiseClone();
		}
	}





}













