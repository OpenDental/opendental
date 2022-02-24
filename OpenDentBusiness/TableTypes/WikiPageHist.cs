using System;

namespace OpenDentBusiness {
	///<summary>Rows never edited, just added.  Contains all historical versions of each page as well.</summary>
	[Serializable]
	public class WikiPageHist:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WikiPageNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Will not be unique because there are multiple revisions per page.</summary>
		public string PageTitle;
		///<summary>The entire contents of the revision are stored in "wiki markup language".  This should never be updated.  Medtext (16M)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PageContent;
		///<summary>The DateTime from the original WikiPage object.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSaved;
		///<summary>This flag will only be set for the revision where the user marked it deleted, not the ones prior.</summary>
		public bool IsDeleted;

		///<summary></summary>
		public WikiPageHist Copy() {
			return (WikiPageHist)MemberwiseClone();
		}

	}
}
