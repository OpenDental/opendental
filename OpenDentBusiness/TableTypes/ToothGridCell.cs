using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Holds one recorded cell value for a tooth grid, which is a special kind of sheet field type that shows a grid with 32 rows and configurable columns.  The entire grid is a single large sheet field.</summary>
	[Serializable]
	public class ToothGridCell : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ToothGridCellNum;
		///<summary>FK to sheetfield.SheetFieldNum.  Required.</summary>
		public long SheetFieldNum;
		///<summary>FK to toothgridcol.ToothGridColNum.  This tells which column it belongs in.  Can't use the column name here because multiple columns could have the same name.</summary>
		public long ToothGridColNum;
		///<summary>Cannot be empty.  For a tooth-level cell, the only allowed value is X.  If the cell is unchecked, then it won't even have a row in this table.  For a surface level column, only valid surfaces can be entered:MOIDBFLV  Enforced.  FreeText columns can have any text up to 255 char.</summary>
		public string ValueEntered;
		///<summary>Corresponds exactly to procedurelog.ToothNum.  May be blank, otherwise 1-32, 51-82, A-T, or AS-TS, 1 or 2 char.  Gets internationalized as being displayed.</summary>
		public string ToothNum;

		public ToothGridCell Copy() {
			return (ToothGridCell)this.MemberwiseClone();
		}
	}

}

