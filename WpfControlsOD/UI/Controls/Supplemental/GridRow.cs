using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfControls.UI {
	//Jordan is the only one allowed to edit this file.

	public class GridRow {
		///<summary>This canvas holds a background rectangle and rarely also cell rectangles. But since most backgrounds are empty, this will usually be null.</summary>
		public Canvas CanvasBackground;
		///<summary>This canvas holds all text and the lower horizontal line for this row.  It will be null until this row is drawn for the first time.</summary>
		public Canvas CanvasText;

		#region Properties
		///<summary>Bold all cells in the row.  Each cell also has a bold override.</summary>
		public bool Bold { get; set; } = false;

		public ListGridCells Cells { get; } = new ListGridCells();

		///<summary>Color of Background of row.  Default is White.  Each gridCell also has a colorBackG property that will override this if set.</summary>
		public Color ColorBackG {get;set; } = Colors.White;

		///<summary>Color of the lower border of a row. Default is Transparent, which will result in Gray220.</summary>
		public Color ColorLborder { get; set; } = Colors.Transparent;

			///<summary>Color of all text in row.  Default is Black. Each gridCell also has a colorText property that will override this if set.</summary>
		public Color ColorText { get; set; } = Colors.Black;

		///<summary>This is a very special field.  Since most of the tables in OD require the ability to attach long notes to each row, this field makes it possible.  Any note set here will be drawn as a sort of subrow below this row.  The note can span multiple columns, as defined in grid.NoteSpanStart and grid.NoteSpanStop.</summary>
		[Obsolete("Not Implemented yet")]
		public string Note { get; set; } = "";

		///<summary>These fields can only be changed internally by ODGrid, never from outside ODGrid.  Includes size and position of this row, visiblity, and dropdown state.</summary>
		public GridRowState State { get; set; } = new GridRowState();

		///<summary>Used to store any kind of object that is associated with the row.</summary>
		public object Tag {get;set;} = null;
		#endregion Properties

		public override string ToString(){
			return "YPos "+State.YPos.ToString()+", HeightMain: "+State.HeightMain.ToString()+", Visible: "+State.Visible.ToString();
		}

		///<summary></summary>
		public class GridRowState{
			///<summary>The height of the main part of the row without the note section.</summary>
			public int HeightMain=0;
			///<summary>The height of the note section of this row.</summary>
			public int HeightNote=0;
			///<summary>The vertical location at which to start drawing this row.  Coordinates are from the top of the first row, as it would be without any scrolling.  To paint, add vertical scrolling, origin, etc.</summary>
			public int YPos=0;
			///<summary>All rows start out visible.  They can be set not visible if they have a dropdown parent that is up.</summary>
			public bool Visible=true;
			///<summary>Calculating height is expensive, so we assume a starting height and then do a lazy calc.  Set this to true once calculated.</summary>
			public bool IsHeightCalculated=false;

			///<summary>HeightMain + HeightNote</summary>
			public int HeightTotal => HeightMain+HeightNote;

			public override string ToString(){
				return "YPos "+YPos.ToString()+", HeightTotal: "+HeightTotal.ToString();
			}

			public GridRowState Copy(){
				return (GridRowState)this.MemberwiseClone();
			}

		}
	}

	public class ListGridCells:List<GridCell>{
				///<summary></summary>
		public void Add(string value) {
			this.Add(new GridCell(value));
		}
	}
}
