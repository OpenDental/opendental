using System;

namespace OpenDentBusiness{
	
	///<summary>Template for quick pasted note feature.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class QuickPasteNote:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long QuickPasteNoteNum;
		///<summary>FK to quickpastecat.QuickPasteCatNum.  Keeps track of which category this note is in.</summary>
		public long QuickPasteCatNum;
		///<summary>The order of this note within it's category. 0-based.</summary>
		public int ItemOrder;
		///<summary>The actual note. Can be multiple lines and possibly very long.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The abbreviation which will automatically substitute when preceded by a ?.</summary>
		public string Abbreviation;

		public QuickPasteNote Copy() {
			return (QuickPasteNote)this.MemberwiseClone();
		}

	}

	


}









