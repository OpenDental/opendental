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
		///<summary>Enum:EnumToolBar The toolbar to show the button on.</summary>
		public EnumToolBar ToolBar;
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

	///<summary></summary>
	public enum EnumToolBar{
		///<summary>0</summary>
		AccountModule,
		///<summary>1</summary>
		ApptModule,
		///<summary>2</summary>
		ChartModule,
		///<summary>3</summary>
		ImagingModule,
		///<summary>4</summary>
		FamilyModule,
		///<summary>5</summary>
		TreatmentPlanModule,
		///<summary>6</summary>
		ClaimsSend,
		///<summary>7 Shows in the toolbar at the top that is common to all modules.</summary>
		MainToolbar,
		///<summary>8 Shows in the main menu Reports submenu.</summary>
		ReportsMenu,
	}

}













