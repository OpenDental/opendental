using System;

namespace OpenDentBusiness{

	///<summary>Each row represents one toolbar button to be placed on a toolbar and linked to a program.</summary>
	[Serializable]
	public class ToolButItem:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ToolButItemNum;
		///<summary>FK to program.ProgramNum.</summary>
		public long ProgramNum;
		///<summary>Enum:ToolBarsAvail The toolbar to show the button on.</summary>
		public ToolBarsAvail ToolBar;
		///<summary>The text to show on the toolbar button.</summary>
		public string ButtonText;
		//later include ComputerName.  If blank, then show on all computers.

		///<summary></summary>
		public static int Compare(ToolButItem item1,ToolButItem item2) {
			return item1.ButtonText.CompareTo(item2.ButtonText);
		}

		public ToolButItem Copy() {
			return (ToolButItem)this.MemberwiseClone();
		}
	}

	

}













