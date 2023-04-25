using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlowQueryLogTool.UI {
	public class GridColumn {
		///<summary>Property backer.</summary>
		private float _dynamicWeight=1;

		///<summary>Set this to an event method and it will be used when the column header is clicked.</summary>
		public EventHandler CustomClickEvent;

		#region Constructors
		///<summary>Creates a new ODGridcolumn.</summary>
		public GridColumn() {

		}

		///<summary>Creates a new ODGridcolumn with the given heading and width. Alignment left</summary>
		public GridColumn(string heading,int colWidth) {
			Heading=heading;
			ColWidth=colWidth;
		}

		///<summary>Creates a new ODGridcolumn with the given heading and width.</summary>
		public GridColumn(string heading,int colWidth,HorizontalAlignment textAlign) {
			Heading=heading;
			ColWidth=colWidth;
			TextAlign=textAlign;
		}

		///<summary>Deprecated. Creates a new ODGridcolumn with the given heading and width. Alignment left</summary>
		public GridColumn(string heading,int colWidth,HorizontalAlignment textAlign,bool isEditable) {
			Heading=heading;
			ColWidth=colWidth;
			TextAlign=textAlign;
			IsEditable=isEditable;
		}

		///<summary>Deprecated. Creates a new ODGridcolumn with the given heading and width. Alignment left</summary>
		public GridColumn(string heading,int colWidth,bool isEditable) {
			Heading=heading;
			ColWidth=colWidth;
			IsEditable=isEditable;
		}

		///<summary>Creates a new ODGridcolumn with the given heading, width, and sorting strategy.</summary>
		public GridColumn(string heading,int colWidth,GridSortingStrategy sortingStrategy) {
			Heading=heading;
			ColWidth=colWidth;
			SortingStrategy=sortingStrategy;
		}

		///<summary>Creates a new ODGridcolumn with the given heading, width, and sorting strategy.</summary>
		public GridColumn(string heading,int colWidth,HorizontalAlignment textAlign,GridSortingStrategy sortingStrategy) {
			Heading=heading;
			ColWidth=colWidth;
			TextAlign=textAlign;
			SortingStrategy=sortingStrategy;
		}
		#endregion Constructors

		#region Properties
		///<summary>Column width, default 80. This width is always 96 dpi and gets scaled internally to State.Width at current dpi.</summary>
		[DefaultValue(80)]
		public int ColWidth { get; set; } = 80;

		///<summary>When combo boxes are used in column cells, this can be set to force a width of dropdown instead of using the column width.</summary>
		[DefaultValue(0)]
		public int DropDownWidth { get; set; } = 0;

		///<summary>String that shows in the top heading cell.</summary>
		[DefaultValue("")]
		public string Heading { get; set; } = "";

		///<summary>List of images that can be picked from within each cell.</summary>
		[DefaultValue(null)]
		public ImageList ImageList { get; set; } = null;

		///<summary>Default false. Set a non-zero starting width for the column, then set this to true.  Column(s) will resize dynamically as long as not hScrollVisible.  If no columns are set to be dynamic, then the right column resizes automatically.</summary>
		[DefaultValue(false)]
		public bool IsWidthDynamic { get; set; } = false;

		///<summary>If IsWidthDynamic and if there are multiple dynamic columns, then this can also optionally be set.  By default, the weight of a dynamic column is 1.  Setting this to 2.5, for example, will make it have a dynamic width that is 2.5x bigger than a column with a weight of 1.  Must be 1 or greater.</summary>
		[DefaultValue(1f)]
		public float DynamicWeight {
			get {
				return _dynamicWeight;
			}
			set {
				if(value<1) {
					_dynamicWeight=1;
					return;
				}
				_dynamicWeight=value;
			}
		}

		///<summary>Default false. You also must set grid.SelectionMode to OneCell.</summary>
		[DefaultValue(false)]
		public bool IsEditable { get; set; } = false;

		///<summary>Can be used when grid.SelectionMode=OneCell. Setting this list of strings causes combo boxes to be used in column cells instead of textboxes.  This is the list of strings to show in the combo boxes.</summary>
		[DefaultValue(null)]
		public List<string> ListDisplayStrings { get; set; } = null;

		///<summary>Default StringCompare</summary>
		[DefaultValue(GridSortingStrategy.StringCompare)]
		public GridSortingStrategy SortingStrategy { get; set; } = GridSortingStrategy.StringCompare;

		///<summary>Attach any object to a column for a variety of reference purposes.</summary>
		[DefaultValue(null)]
		public object Tag { get; set; } = null;

		///<summary>Default Left</summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign { get; set; } = HorizontalAlignment.Left;

		///<summary>These fields can only be changed internally by ODGrid, never from outside ODGrid.  Includes Pos, Width, and Right of this column.</summary>
		public GridColState State { get; set; } = new GridColState();
		#endregion Properties

		public GridColumn Copy() {
			GridColumn gridColumn=(GridColumn)this.MemberwiseClone();
			if(this.ListDisplayStrings!=null) {
				gridColumn.ListDisplayStrings=this.ListDisplayStrings.Select(x => new string(x.ToArray())).ToList();
			}
			return gridColumn;
		}

		public override string ToString() {
			return Heading+":"+ColWidth.ToString();
		}

		///<summary></summary>
		public class GridColState {
			///<summary>The location of the left edge.</summary>
			public int XPos=0;
			///<summary>This is not 96 dpi.  It's width at current dpi.  Cannot set from outside.</summary>
			public int Width=0;
			///<summary>The right edge.  Same as the left edge of the next cell.</summary>
			public int Right=0;

			public override string ToString() {
				return "Pos "+XPos.ToString()+", Width: "+Width.ToString();
			}
		}





	}


	public enum GridSortingStrategy {
		///<summary>0- Default</summary>
		StringCompare,
		DateParse,
		ToothNumberParse,
		AmountParse,
		TimeParse,
		VersionNumber,
	}
}






