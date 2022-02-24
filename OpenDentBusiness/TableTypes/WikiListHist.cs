using System;

namespace OpenDentBusiness {
	///<summary>Rows never edited, just added.  Contains all historical versions of each list.</summary>
	[Serializable]
	public class WikiListHist:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WikiListHistNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Will not be unique because there are multiple revisions per page.</summary>
		public string ListName;
		///<summary>The contents of the corresponding WikiListHeaderWidths row converted to a string in format ColName1,ColWidth1;ColName2,ColWidth2;...  Database type text/varChar2(4000) (65K/4K)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ListHeaders;
		///<summary>The entire contents of the revision are stored as XML.  Database type mediumtext/clob (16M,4G)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ListContent;
		///<summary>The DateTime from the original WikiPage object.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSaved;

		///<summary></summary>
		public WikiListHist Copy() {
			return (WikiListHist)MemberwiseClone();
		}

	}
}
