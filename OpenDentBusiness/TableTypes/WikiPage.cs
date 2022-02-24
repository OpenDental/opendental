using System;

namespace OpenDentBusiness {
	///<summary>Rows never edited, just added, unless the wiki page is a draft.  Contains only newest versions of each page and all drafts.</summary>
	[Serializable]
	public class WikiPage:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WikiPageNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Must be unique.  Any character is allowed except: \r, \n, and ".  Needs to be tested, especially with apostrophes.</summary>
		public string PageTitle;
		///<summary>Automatically filled from the [[Keywords:]] tab in the PageContent field as page is being saved.</summary>
		public string KeyWords;
		///<summary>Content of page stored in "wiki markup language".  This should never be updated, unless it is a draft.  Medtext (16M)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PageContent;
		///<summary>The DateTime that the page was saved to the DB.  User can't directly edit.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime DateTimeSaved;
		///<summary>This flag will be set when the user archives the WikiPage.</summary>
		public bool IsDeleted;
		///<summary>Signifies that the wiki page is a draft, and will only show in the Wiki Drafts form.</summary>
		public bool IsDraft;
		///<summary>Records if a wiki page is locked.  If it is locked, only user swith the WikiAdmin permission are allowed to edit the page</summary>
		public bool IsLocked;
		///<summary>Content of page stored without any HTML markup or wiki page links.  This plain text allows for easier searching.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PageContentPlainText;

		///<summary></summary>
		public WikiPage Copy() {
			return (WikiPage)MemberwiseClone();
		}

		

	}
}
