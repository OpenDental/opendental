using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Defines the columns present in a single completed tooth grid, which is a special kind of sheet field that shows a grid with 32 rows and configurable columns.  The entire grid is a single large sheet field.  This table defines how the grid is layed out on an actual sheet, pulled initially from a ToothGridDef.  The data itself is recorded in ToothGridCell.</summary>
	[Serializable]
	public class ToothGridCol : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ToothGridColNum;
		///<summary>FK to sheet.SheetFieldNum.  Required.</summary>
		public long SheetFieldNum;
		///<summary>Pulled from the ToothGridDef.  This can be a NameInternal , or it can be a NameShowing if it's a user-defined column.</summary>
		public string NameItem;
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

		public ToothGridCol Copy() {
			return (ToothGridCol)this.MemberwiseClone();
		}
	}

}

