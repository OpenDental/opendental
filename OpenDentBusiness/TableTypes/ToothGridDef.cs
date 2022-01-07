using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary> Defines the columns present in a tooth grid, which is a special kind of sheet field def that shows a grid with 32 rows and configurable columns.  Can be edited without damaging any completed sheets.</summary>
	[Serializable]
	public class ToothGridDef : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ToothGridDefNum;
		///<summary>FK to sheetfielddef.SheetFieldDefNum</summary>
		public long SheetFieldDefNum;
		///<summary>This is the internal name that OD uses to identify the column.  Blank if this is a user-defined column.  We will keep a hard-coded list of available NameInternals in the code to pick from.</summary>
		public string NameInternal;
		///<summary>The user may override the internal name for display purposes.  If this is a user-defined column, this is the only name, since there is no NameInternal.</summary>
		public string NameShowing;
		///<summary>Enum:ToothGridCellType  0=HardCoded, 1=Tooth, 2=Surface, 3=FreeText.</summary>
		public ToothGridCellType CellType;
		///<summary>Order of the column to display.  Every entry must have a unique itemorder.</summary>
		public int ItemOrder;
		///<summary>.</summary>
		public int ColumnWidth;
		///<summary>FK to procedurecode.CodeNum.  This allows data entered to flow into main program as actual completed or tp procedures.</summary>
		public long CodeNum;
		///<summary>Enum:ProcStat  If these flow into main program, then this is the status that the new procs will have.</summary>
		public ProcStat ProcStatus;
		

		public ToothGridDef Copy() {
			return (ToothGridDef)this.MemberwiseClone();
		}
	}

	///<summary>0=HardCoded, 1=Tooth, 2=Surface, 3=FreeText.</summary>
	public enum ToothGridCellType {
		///<summary>0</summary>
		HardCoded,
		///<summary>1</summary>
		Tooth,
		///<summary>2</summary>
		Surface,
		///<summary>3</summary>
		FreeText
	}
}

