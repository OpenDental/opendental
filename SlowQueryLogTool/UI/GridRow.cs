using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowQueryLogTool.UI {
	public class GridRow {

		#region Constructors
		///<summary>Creates a new ODGridRow.</summary>
		public GridRow() {

		}

		public GridRow(params string[] cellText) {
			cellText.ToList().ForEach(x => Cells.Add(x));
		}

		public GridRow(params GridCell[] cellList) {
			cellList.ToList().ForEach(x => Cells.Add(x));
		}
		#endregion Constructors

		#region Properties
		///<summary></summary>
		public ListGridCells Cells { get; } = new ListGridCells();

		///<summary>Color of Background of row.</summary>
		[DefaultValue(typeof(Color),"White")]
		public Color ColorBackG { get; set; } = Color.White;

		///<summary>Bold all cells in the row.  Each cell also has a bold override.</summary>
		[DefaultValue(typeof(Color),"White")]
		public bool Bold { get; set; } = false;

		///<summary>Color of all text in row.  Each gridCell also has a colorText property that will override this, if set.</summary>
		public Color ColorText { get; set; } = Color.Black;

		///<summary>Color of the lower border.</summary>
		public Color ColorLborder { get; set; } = Color.Empty;

		///<summary>Used to store any kind of object that is associated with the row.</summary>
		public object Tag { get; set; } = null;

		///<summary>This is a very special field.  Since most of the tables in OD require the ability to attach long notes to each row, this field makes it possible.  Any note set here will be drawn as a sort of subrow below this row.  The note can span multiple columns, as defined in grid.NoteSpanStart and grid.NoteSpanStop.</summary>
		public string Note { get; set; } = "";

		///<summary>If this is a dropdown row, set this reference to a parent row that drops this row down.  If not, null.</summary>
		public GridRow DropDownParent { get; set; } = null;

		///<summary>If this is a DropDown parent row, you can set this to true in order for it to show initially as dropped down.</summary>
		public bool DropDownInitiallyDown { get; set; } = false;

		///<summary>These fields can only be changed internally by ODGrid, never from outside ODGrid.  Includes size and position of this row, visiblity, and dropdown state.</summary>
		public GridRowState State { get; set; } = new GridRowState();
		#endregion Properties

		public override string ToString() {
			return "YPos "+State.YPos.ToString()+", HeightMain: "+State.HeightMain.ToString()+", Visible: "+State.Visible.ToString();
		}

		///<summary></summary>
		public class GridRowState {
			///<summary>The height of the main part of the row without the note section.</summary>
			public int HeightMain=0;
			///<summary>The height of the note section of this row.</summary>
			public int HeightNote=0;
			///<summary>The vertical location at which to start drawing this row.  Coordinates are from the top of the first row, as it would be without any scrolling.  To paint, add vertical scrolling, origin, etc.</summary>
			public int YPos=0;
			///<summary>ODGridDropDownState: 0: None (not a dropdown parent), 1:Up (not dropped), 2: Down.</summary>
			public ODGridDropDownState DropDownState=ODGridDropDownState.None;
			///<summary>All rows start out visible.  They can be set not visible if they have a dropdown parent that is up.</summary>
			public bool Visible=true;
			///<summary>Calculating height is expensive, so we assume a starting height and then do a lazy calc.  Set this to true once calculated.</summary>
			public bool IsHeightCalculated=false;

			///<summary>HeightMain + HeightNote</summary>
			public int HeightTotal => HeightMain+HeightNote;

			public override string ToString() {
				return "YPos "+YPos.ToString()+", HeightTotal: "+HeightTotal.ToString();
			}

			public GridRowState Copy() {
				return (GridRowState)this.MemberwiseClone();
			}

		}

	}

	///<summary>Determines the state of a dropdown row.</summary>
	public enum ODGridDropDownState {
		///<summary>0 - not a drop down parent.</summary>  
		None,
		///<summary>1 - not dropped down.</summary>  
		Up,
		///<summary>2 - dropped down.</summary>
		Down,
	}


	public class ListGridCells:List<GridCell> {
		///<summary></summary>
		public void Add(string value) {
			this.Add(new GridCell(value));
		}
	}





}






