using OpenDental;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CodeBase;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

This is a template of typical grid code which can be quickly pasted into any form.
 
using WpfControls.UI;

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("Table...","colname"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("Table...","colname"),100,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("Table...","colname"),100);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_list.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_list[i].);
				row.Cells.Add("");
				row.Cells.Add(new GridCell(""){ColorBackG=.., etc});//this is not allowed
			  GridCell cell=new GridCell("");//instead of above line
				cell.ColorBackG=...;
				row.Cells.Add(cell);
				row.Cells.Add(new GridCell(_list[i].IsHidden?"X":""));
				row.Tag=;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}
*/
	///<summary></summary>
	public partial class Grid:UserControl {
		#region Fields - Public
			//public List<GridColumn> ListGridColumns=new List<GridColumn>();
		#endregion Fields - Public

		#region Fields - Private for Properties
		private Color _colorSelectedRow=Color.FromRgb(205,220,235);
		private bool _headersVisible=true;
		private bool _hScrollVisible=false;
		///<summary>This is the only internal storage for tracking selected row indices.  All properties refer to this same list.</summary>
		private List<int> _listSelectedIndices=new List<int>();
		private ColRow _selectedCell=new ColRow(-1,-1);
		private GridSelectionMode _selectionMode=GridSelectionMode.OneRow;
		private bool _sortingAllowByColumn=false;
		private string _title="";
		private bool _titleVisible=true;
		#endregion Fields - Private for Properties

		#region Fields - Private
		/// <summary>This canvas contains all the vertical gridlines.  That way, we can clear the vertical gridlines as needed.</summary>
		private Canvas _canvasVertLines;
		///<summary>The ordinary horizontal and vertical gridlines. Gray 220</summary>
		private Color _colorGridline=OpenDental.ColorOD.Gray_Wpf(220);//Color.FromRgb(220,220,220);
		///<summary>Used when resizing this grid in order to </summary>
		private DispatcherTimer _dispatcherTimerSizing;
		///<summary>19 instead of 14.  Used for textboxes, comboboxes, and buttons, so that they have room.</summary>
		private const int EDITABLE_ROW_HEIGHT=19;
		///<summary>Is set when ComputeRows is run, then used . If any columns are editable, hasEditableColumn is true, and all row heights are slightly taller.</summary>
		private bool _hasEditableColumn;
				///<summary>Starts out 15, but code can push it higher to fit multiline text. If header is not visible, this will be set to 0. 96dpi.</summary>
		private int _heightHeader=15;
				///<summary>The total height of the actual grid area, including the parts hidden by scroll. </summary>
		private int _heightTotal;
		///<summary>-1 if none. A possible future enhancement would be to hover single cell instead of entire row for selectionMode=OneCell.</summary>
		private int _hoverRow=-1;
				///<summary>True to show a triangle pointing up.  False to show a triangle pointing down.  Only works if _sortedByColumnIdx is not -1.</summary>
		private bool _isSortedAscending;
		///<summary>True between BeginUpdate-EndUpdate.</summary>
		private bool _isUpdating;
		///<summary></summary>
		private List<Rectangle> _listRectanglesSelected=new List<Rectangle>();
		///<summary>Helps with drag selections. This is a snapshot of the selected indices when the mouse goes down and starts a drag.</summary>
		private List<int> _listSelectedIndicesWhenMouseDown;
		private int _mouseDownCol;
		private int _mouseDownRow;
		///<summary></summary>
		private Rectangle _rectangleHover;
		private ColRow _selectedCellOld;
		///<summary>Typically -1 to show no triangle.  Or, specify a column to show a triangle.  The actual sorting happens at mouse down.</summary>
		private int _sortedByColumnIdx = -1;
		///<summary>This single textBox is always present as a child of canvasMain.  We set it to not visible when we don't need it. I had to use the raw textBox instead of UI.TextBox because ours had trouble getting focus immediately.</summary>
		private System.Windows.Controls.TextBox textEdit;
		///<summary>The total width of the grid, including the parts hidden by scroll.</summary>
		private int _widthTotal;
		private bool _wrapText = true;
		#endregion Fields - Private

		#region Constructor
		public Grid() {
			InitializeComponent();
			//Width=300;
			//Height=300;
			Columns=new GridColumnCollection(this);
			FontSize=11.5;//equivalent to WF 8.6. This is same as declared on each frm, but it's here in case we need to print without a frm.
			_heightTotal=0;//instead of	ComputeRows(g);
			textEdit=new System.Windows.Controls.TextBox();
			textEdit.Visibility=Visibility.Collapsed;
			Canvas.SetZIndex(textEdit,5);
			textEdit.TextChanged+=textEdit_TextChanged;
			textEdit.GotFocus+=textEdit_GotFocus;
			textEdit.LostFocus+=textEdit_LostFocus;
			textEdit.KeyDown+=textEdit_KeyDown;
			textEdit.KeyUp+=textEdit_KeyUp;
			canvasMain.Children.Add(textEdit);
			Focusable=true;//so that clicking causes lost focus on textbox
			Loaded+=Grid_Loaded;
			MouseWheel+=Grid_MouseWheel;
			PreviewKeyDown+=Grid_PreviewKeyDown;
		}
		#endregion Constructor

		#region Events - raise
		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when a cell is double clicked.")]
		public event EventHandler<GridClickEventArgs> CellDoubleClick=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when a combo box item is selected.")]
		//Not implemented
		public event EventHandler<GridClickEventArgs> CellSelectionCommitted=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when a cell is single clicked.")]
		public event EventHandler<GridClickEventArgs> CellClick=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Event used when cells are editable.  The TextChanged event is passed up from the textbox where the editing is taking place.")]
		public event EventHandler CellTextChanged=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Event used when cells are editable.  LostFocus event is passed up from the textbox where the editing is taking place.")]
		//I'm not convinced that we need col, row, or mouseButton. For now, we include none of those, but we can add them if needed.
		public event EventHandler CellLeave=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Event used when cells are editable.  GotFocus event is passed up from the textbox where the editing is taking place.")]
		//I'm not convinced that we need col, row, or mouseButton. For now, we include none of those, but we can add them if needed.
		public event EventHandler CellEnter=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Event used when cells are editable.  KeyDown event is passed up from the textbox where the editing is taking place.")]
		//In GridOD, this used ODGridKeyEventHandler/ODGridKeyEventArgs, which was just a wrapper for KeyEventArgs.
		public event EventHandler<KeyEventArgs> CellKeyDown=null;

		[Category("OD")]
		[Description("Occurs when rows are selected or unselected by the user for any reason, including mouse and keyboard clicks.  Only works for GridSelectionModes.One for now (enhance later).  Excludes programmatic selection.")]
		public event EventHandler SelectionCommitted=null;

		///<summary></summary>
		[Category("OD")]
		[Description("If HasAddButton is true, this event will fire when the add button is clicked.")]
		//Not implemented
		public event EventHandler TitleAddClick=null;

		//<summary></summary>
		//[Category("OD")]
		//[Description("If SortingAllowByColumn is true, this event will fire when a column header is clicked and before the grid has sorted. Used to disable pagination so that the entire grid's data is sorted.")]
		//public event EventHandler ColumnSortClick=null;
		//Was never even used in GridOD and we don't ever plan to do pagination anyway

		///<summary></summary>
		[Category("OD")]
		[Description("If SortingAllowByColumn is true, this event will fire when a column header is clicked and the grid is sorted.  Used to reselect rows after sorting.")]
		//Not implemented
		public event EventHandler ColumnSorted=null;

		///<summary>This is used to dynamically line up "total" boxes below the grid while scrolling.</summary>
		[Category("OD")]
		[Description("If HScrollVisible is true, this event will fire when the horizontal scroll bar moves by mouse, keyboard, or programmatically.")]
		//Not implemented
		public event EventHandler<System.Windows.Controls.Primitives.ScrollEventArgs> HorizScrolled=null;
		#endregion Events - raise

		#region Properties - Public
		///<summary></summary>
		[Category("OD")]
		[Description("Set false to disable row selection when user clicks.  Row selection should then be handled by the form in response to the cellClick event.")]
		[DefaultValue(true)]
		public bool AllowSelection { get; set; }=true;

		///<summary></summary>
		[Category("OD")]
		[Description("Set to true to have alternate rows colored a very pale green.")]
		[DefaultValue(false)]
		public bool AlternateRowsColored {get; set; } =false;

		///<summary></summary>
		[Category("OD")]
		[Description("Set true if you want the up and down arrows to work even when the grid does not have focus. Default false.")]
		[DefaultValue(false)]
		public bool ArrowsWhenNoFocus { get; set; }=false;

		///<summary>The background color that is used for selected rows. Default is light blue/gray color.</summary>
		[Category("OD")]
		[Description("The background color that is used for selected rows. Default of is light blue/gray color.")]
		[DefaultValue(typeof(Color),"205,220,235")]
		public Color ColorSelectedRow {
			get => _colorSelectedRow;
			set {
				_colorSelectedRow=value;
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Set to false to disable showing any context menu that you may have set for the grid.")]
		[DefaultValue(true)]
		//Not implemented
		public bool ContextMenuShows { get; set; } = true;

		///<summary>Allow rows to drop down other rows. Leave enough space in the row's first cell to display a drop down arrow. Rows that can drop down must have a parent row set.</summary>
		[Category("OD")]
		[Description("Allow rows to drop down other rows. Leave enough space in the row's first cell to display a drop down arrow. Rows that can drop down must have a parent row set.")]
		[DefaultValue(false)]
		//Not implemented. Was called HasDropdowns.
		public bool DropDownNesting {get; set; } =false;

		///<summary>Only affects grids with editable columns. True allows carriage returns within cells. False causes carriage returns to go to the next editable cell.</summary>
		[Category("OD")]
		[Description("Only affects grids with editable columns. True allows carriage returns within cells. False causes carriage returns to go to the next editable cell.")]
		[DefaultValue(false)]
		public bool EditableAcceptsCR { get; set; } =false;

		///<summary>If column is editable and user presses Enter, default behavior is to move right.  For some grids, default behavior needs to move down.</summary>
		[Category("OD")]
		[Description("If column is editable and user presses Enter, default behavior is to move right.  For some grids, default behavior needs to move down.")]
		[DefaultValue(false)]
		public bool EditableEnterMovesDown { get; set; }

		//EditableUsesRTF will not be implemented. It was only used in one place and didn't seem necessary.

		//HasAddButton will not be implemented.  Very easy to just add your own button on top of the grid.

		//HasAutoWrappedHeaders  is redundant.  When HasMultilineHeaders is turned on, then wrapping will happen automatically

		//HasLinkDetect seems useless.  It was only set to false in 3 random places where it probably doesn't matter.  It should just always be treated as true.

		///<summary>Allow Headers to be multiple lines tall, including wrap and new line characters.</summary>
		[Category("OD")]
		[Description("Allow Headers to be multiple lines tall, including wrap and new line characters.")]
		[DefaultValue(false)]
		//Not implemented
		public bool HeadersMultiline {get; set; } =false;

		///<summary>Set false to hide the column header row below the main title.</summary>
		[Category("OD")]
		[Description("Set false to hide the column header row below the main title.")]
		[DefaultValue(true)]
		//Not implemented
		public bool HeadersVisible {
			get {
				return _headersVisible;
			}
			set {
				if(_headersVisible==value){
					return;
				}
				_headersVisible=value;
				if(_headersVisible){
					_heightHeader=15;
				}
				else{
					_heightHeader=0;
				}
				//LayoutScrolls();
			}
		}

		///<summary>Set true to show a horizontal scrollbar.  Vertical scrollbar always shows if needed.</summary>
		[Category("OD")]
		[Description("Set true to show a horizontal scrollbar.  Vertical scrollbar always shows if needed.")]
		[DefaultValue(false)]
		public bool HScrollVisible {
			get {
				return _hScrollVisible;
			}
			set {
				_hScrollVisible=value;
				//LayoutScrolls();
				if(_hScrollVisible){
					scrollH.Visibility=Visibility.Visible;
				}
				else{
					scrollH.Visibility=Visibility.Collapsed;
				}
			}
		}

		///<summary>The starting column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.</summary>
		[Category("OD")]
		[Description("The starting column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.")]
		[DefaultValue(0)]
		//Not implemented.
		public int NoteSpanStart { get; set; } = 0;

		///<summary>The ending column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.  If this remains 0, then notes will be entirey skipped for this grid.  There is no grid line on the right side of a note.</summary>
		[Category("OD")]
		[Description("The ending column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.  If this remains 0, then notes will be entirely skipped for this grid.  There is no grid line on the right side of a note.")]
		[DefaultValue(0)]
		//Not implemented.
		public int NoteSpanStop { get; set; } = 0;

		///<summary>If right click, and there is 'PatNum:##', 'TaskNum:##', or 'JobNum:##', then the context menu will show a clickable link. Only used task edit, task list, and account commlog.</summary>
		[Category("OD")]
		[Description("If right click, and there is 'PatNum:##', 'TaskNum:##', or 'JobNum:##', then the context menu will show a clickable link. Only used task edit, task list, and account commlog.")]
		[DefaultValue(false)]
		public bool RightClickLinks { get; set; }=false;

		///<summary></summary>
		[Category("OD")]
		[Description("None, OneRow, OneCell, MultiExtended. Default is OneRow.")]
		[DefaultValue(typeof(GridSelectionMode),"OneRow")]
		public GridSelectionMode SelectionMode {
			get {
				return _selectionMode;
			}
			set {
				if(value==GridSelectionMode.OneCell) {
					_selectedCell=new ColRow(-1,-1);
					_listSelectedIndices=new List<int>();
				}
				_selectionMode=value;
			}
		}

		///<summary>Set true to allow user to click on column headers to sort rows, alternating between ascending and descending.</summary>
		[Category("OD")]
		[Description("Set true to allow user to click on column headers to sort rows, alternating between ascending and descending.")]
		[DefaultValue(false)]
		public bool SortingAllowByColumn {
			get => _sortingAllowByColumn;
			set {
				_sortingAllowByColumn=value;
				if(!_sortingAllowByColumn) {
					_sortedByColumnIdx=-1;
				}
			}
		}

		///<summary>The title of the grid which shows across the top.</summary>
		[Category("OD")]
		[Description("The title of the grid which shows across the top.")]
		[DefaultValue("")]
		public string Title {
			get {
				return _title;
			}
			set {
				_title=value;
				textTitle.Text=value;
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Set false to hide the main title. In rare cases, this could also be used to draw your own title bar.")]
		[DefaultValue(true)]
		//Not implemented
		public bool TitleVisible {
			get {
				return _titleVisible;
			}
			set {
				if(_titleVisible==value){
					return;
				}
				_titleVisible=value;
				//if(_titleVisible){
				//	_heightTitle=18;
				//}
				//else{
				//	_heightTitle=0;
				//}
				//LayoutScrolls();
			}
		}

		///<summary>Uniquely identifies the grid to translate title to another language.  Name it like 'Table...'  Grid contents must be manually translated.</summary>
		[Category("OD")]
		[Description("Uniquely identifies the grid to translate title to another language.  Name it like 'Table...'  Grid contents must be manually translated.")]
		[DefaultValue("")]
		public string TranslationName { get; set; } = "";

		//public bool VScrollVisible //no, scrollV visibility is always automatic

		///<summary>Text within each cell will wrap, making some rows taller. Default true.</summary>
		[Category("OD")]
		[Description("Text within each cell will wrap, making some rows taller. Default true.")]
		[DefaultValue(true)]
		public bool WrapText {
			get {
				return _wrapText;
			}
			set {
				_wrapText=value;
				EndUpdate();
			}
		}
		#endregion Properties - Public

		#region Properties - Not Browsable
		///<summary>Gets the List of GridColumns.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridColumnCollection Columns { get;	} //had to be initialized in constructor

		///<summary>Gets the position of the horizontal scrollbar.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HScrollValue {
			get {
				if(_hScrollVisible && scrollH.IsEnabled) {
					return (int)scrollH.Value;
				}
				return 0;
			}
		}

		///<summary>Gets the List of GridRows assigned to the ODGrid control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<GridRow> ListGridRows { get; } = new List<GridRow>();

		///<summary>Gets or sets the value of the vertical scrollbar.  Does all error checking to ensure valid value.</summary>
		[Browsable(false)]
		[DefaultValue(0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollValue {
			get {
				return (int)scrollV.Value;
			}
			set {
				if(!scrollV.IsEnabled) {
					return;
				}
				//Can't do this because we use this to reset if > max
				//if(vScroll.Value==value){
				//	return;
				//}
				double newValue;
				if(value>scrollV.Maximum){
					newValue=scrollV.Maximum;
				}
				else if(value<scrollV.Minimum) {
					newValue=scrollV.Minimum;
				}
				else {
					newValue=value;
				}
				scrollV.Value=newValue;
				Canvas.SetTop(canvasMain,-scrollV.Value);
				//textEdit?.Dispose();//I think it's harmless to leave any textbox in place. It will just scroll with the canvas.
				//comboBox.Visible=false;
			}
		}

		///<summary>Holds the x,y values of the selected cell if in OneCell mode.  -1,-1 represents no cell selected.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColRow SelectedCell {
			get {
				return _selectedCell;
			}
		}

		///<summary>Returns a list of rows selected instead of a list of indices.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<GridRow> SelectedGridRows {
			get {
				if(SelectionMode==GridSelectionMode.OneCell) {
					return new List<GridRow>() { ListGridRows[_selectedCell.Row] };
				}
				return _listSelectedIndices.Select(x => ListGridRows[x]).ToList();
			}
		}

		///<summary>Holds the int values of the indices of the selected rows.  To set selected indices, use SetSelected().</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int[] SelectedIndices {
			get {
				if(SelectionMode==GridSelectionMode.OneCell) {
					if(_selectedCell.Row==-1) {
						return new int[0];
					}
					return new int[] { _selectedCell.Row };
				}
				_listSelectedIndices.Sort();
				int[] intArray=_listSelectedIndices.ToArray();
				return intArray;
			}
		}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}
		#endregion Properties - Not Browsable

		#region Methods - public
		///<summary></summary>
		public void BeginUpdate() {
			_isUpdating=true;
		}

		///<summary>Must be called after adding rows.  This computes the columns, computes the rows, lays out the scrollbars, clears SelectedIndices, draws headers, and draws rows.  Does not zero out scrollVal.</summary>
		public void EndUpdate(){
			_isUpdating=false;
			//scrollH.Visibility=Visibility.Collapsed;//this must come before ComputeRows, but it's not even working here
			ComputeColumns();
			//if(_hasEditableColumn) {
				//Always truly calculate row height when a column is editable to prevent exceptions with the edit box.
			//	ComputeRows(doActualCalc:true);
			//}
			//else if(ListGridRows.Count<200){
				//See notes at top of this file. This prevents scroll jumping, but doesn't scale to massive numbers of rows.
				ComputeRows();
			//}
			//else{
			//	ComputeRows(doActualCalc:false);//massively scalable
			//}
			FillLastPage();
			_listSelectedIndices=new List<int>();
			_selectedCell=new ColRow(-1,-1);
			textEdit.Visibility=Visibility.Collapsed;
			_sortedByColumnIdx=-1;
			DrawHeaders();
			ClearAll();
			DateTime dateTStart=DateTime.Now;
			DrawRows();
			TimeSpan timeSpanDelta=DateTime.Now-dateTStart;
			DrawGridlinesVert();
			LayoutScrolls();
		}

		///<summary>Returns row. -1 if no valid row.  Supply the y position in DiPs of canvasMain.</summary>
		public int PointToRow(double y) {
			if(y<borderTitle.ActualHeight+borderHeaders.ActualHeight){
				return -1;
			}
			//this can handle clicking below the rows.
			return CanvasPointToRow(y-ScrollValue-borderTitle.ActualHeight-borderHeaders.ActualHeight);
		}

		///<summary>Returns col.  Supply the x position in DiPs of canvasMain. -1 if no valid column.</summary>
		public int PointToCol(double x) {
			return CanvasPointToCol(x+HScrollValue);
		}
		#endregion Methods - public

		#region Methods - Drawing
		private void DrawHeaders(){
			if(_heightHeader==0) {
				return;
			}
			canvasHeaders.Children.Clear();
			for(int i=0;i<Columns.Count;i++) {
				if(i!=0) {
					//vertical lines to left of column header
					Line lineGray=new Line();
					lineGray.X1=Columns[i].State.XPos;
					lineGray.Y1=2;
					lineGray.X2=Columns[i].State.XPos;
					lineGray.Y2=_heightHeader-2;
					lineGray.Stroke=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(120));
					canvasHeaders.Children.Add(lineGray);
					//g.DrawLine(_penColumnSeparator,-hScroll.Value+Columns[i].State.XPos,ScaleI(_heightTitle)+3,
					//	-hScroll.Value+Columns[i].State.XPos,OriginY()-2);
					Line lineWhite=new Line();
					lineWhite.X1=Columns[i].State.XPos+1;
					lineWhite.Y1=2;
					lineWhite.X2=Columns[i].State.XPos+1;
					lineWhite.Y2=_heightHeader-2;
					lineWhite.Stroke=Brushes.White;
					canvasHeaders.Children.Add(lineWhite);
					//g.DrawLine(new Pen(Color.White),-hScroll.Value+Columns[i].State.XPos+1,ScaleI(_heightTitle)+3,
					//	-hScroll.Value+Columns[i].State.XPos+1,OriginY()-2);
				}
				Label label=new Label();
				label.Width=Columns[i].State.Width;
				label.Height=_heightHeader;
				Canvas.SetLeft(label,Columns[i].State.XPos);
				Canvas.SetTop(label,-1);
				label.HAlign=HorizontalAlignment.Center;
				label.VAlign=VerticalAlignment.Center;
				label.Text=Columns[i].Heading;
				label.IsBold=true;
				label.IsWrap=false;
				//label.ColorText=
				canvasHeaders.Children.Add(label);
			}
			for(int i=0;i<Columns.Count;i++) {
				if(_sortedByColumnIdx!=i) {
					continue;
				}
				Polygon polygon=new Polygon();
				polygon.Fill=Brushes.Black;
				Point point=new Point(Columns[i].State.XPos+6,_heightHeader/2f);
				if(_isSortedAscending) {//pointing up
					polygon.Points.Add(new Point(point.X-4,point.Y+2));//LL
					polygon.Points.Add(new Point(point.X+4,point.Y+2));//LR
					polygon.Points.Add(new Point(point.X,point.Y-2));//Top
				}
				else{//pointing down
					polygon.Points.Add(new Point(point.X-4,point.Y-2));//UL
					polygon.Points.Add(new Point(point.X+4,point.Y-2));//UR
					polygon.Points.Add(new Point(point.X,point.Y+2));//Bottom
				}
				canvasHeaders.Children.Add(polygon);
			}//for
		}

		///<summary>Draws the background, horizontal lines, and text for all rows that are visible (for now, it just draws all rows) For less than 500 rows, it draws all rows. Also recalulates each row height that draws.</summary>
		private void DrawRows() {
			//DateTime dateTStart=DateTime.Now;
			//canvasMain.Children.Clear();//This step takes the same amount of time as adding children.
			//TimeSpan timeSpanDelta=DateTime.Now-dateTStart;
			int rowEnd=-1;
			bool didAnyHeightChange=false;
			for(int i=0;i<ListGridRows.Count;i++) {
				if(!ListGridRows[i].State.Visible){
					continue;
				}
				//no need for any of this fancy stuff until we start scaling to hundreds of rows:
				/*
				if(-scrollV.Value+ListGridRows[i].State.YPos+ListGridRows[i].State.HeightMain+ListGridRows[i].State.HeightNote < 0) {
					continue;//lower edge of row above top of grid area
				}
				if(-scrollV.Value+ListGridRows[i].State.YPos > canvasMain.ActualHeight) {//row below lower edge of area
					rowEnd=i;
					break;
				}*/
				if(ListGridRows[i].Cells.Count==0){
					FillRow(i);	
				}
				//if(!ListGridRows[i].State.IsHeightCalculated){
					//int heightBefore=ListGridRows[i].State.HeightTotal;
					//ComputeRowHeightOne(i);
					//if(heightBefore!=ListGridRows[i].State.HeightTotal){
					//	didAnyHeightChange=true;
					//}
				//}
				//always compute the yPos of the row below this one in case it's stale
				if(i<ListGridRows.Count-1){//unless this is the last row
					ListGridRows[i+1].State.YPos=ListGridRows[i].State.YPos+ListGridRows[i].State.HeightTotal;
				}
				DrawRow(i);
				DrawRowBackground(i);
			}
			if(didAnyHeightChange && rowEnd!=-1){
				ComputeRowYposStartingAt(rowEnd);
				LayoutScrolls();
			}
		}

		///<summary>Draws image and text for a single row. As it draws, it measures the resulting height and sets the row height. DrawRowBackground is then called separately.</summary>
		private void DrawRow(int rowI) {
			GridRow gridRow=ListGridRows[rowI];
			if(gridRow.CanvasText==null){
				gridRow.CanvasText=new Canvas();
				canvasMain.Children.Add(gridRow.CanvasText);
				Canvas.SetZIndex(gridRow.CanvasText,3);
			}
			else{
				gridRow.CanvasText.Children.Clear();
			}
			Canvas.SetTop(gridRow.CanvasText,gridRow.State.YPos);
			//width and height later, but might not even matter for a canvas
			FontFamily fontFamily=new FontFamily("Segoe UI");
			int heightFont=(int)Math.Ceiling(fontFamily.LineSpacing * FontSize);//for one normal row
			for(int i=0;i<Columns.Count;i++) {
				//text
				if(i > gridRow.Cells.Count-1) {
					continue;
				}
				TextBlock textBlock=new TextBlock();
				textBlock.Width=Math.Max(0,Columns[i].State.Width-3);
				if(WrapText){
					textBlock.TextWrapping=TextWrapping.Wrap;
					//textBlock.Height=double.NaN
				}
				else{
					//textBlock.TextTrimming=TextTrimming.//this didn't work because I don't want ellipsis
					textBlock.Height=heightFont+1;
					//textBlock.Clip=new RectangleGeometry(new Rect(0,0,textBlock.Width,textBlock.Height));
				}
				Canvas.SetLeft(textBlock,Columns[i].State.XPos+2);
				//Canvas.SetTop(textBlock,0);
				//Canvas.SetZIndex(textBlock,0);
				if(gridRow.Cells[i].Underline){
					//underline is very simple per cell. No row override involved.
					//There's also no partial underlining of a cell. It's all or nothing.
					textBlock.Inlines.Add(new Underline(new Run(gridRow.Cells[i].Text)));
				}
				else{
					textBlock.Text=gridRow.Cells[i].Text;
				}
				switch(Columns[i].TextAlign) {
					case HorizontalAlignment.Left:
						//already left by default
						break;
					case HorizontalAlignment.Center:
						textBlock.TextAlignment=TextAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						textBlock.TextAlignment=TextAlignment.Right;
						break;
				}
				if(gridRow.Cells[i].ColorText.ToString()==Colors.Transparent.ToString()) {
					textBlock.Foreground=new SolidColorBrush(gridRow.ColorText);
				}
				else{
					//if(gridRow.Cells[i].IsButton && !Enabled) { //set the "button" text to gray if this is grid is disabled
					//	colorText=_colorTextDisabled;
					//}
					//else {
					textBlock.Foreground=new SolidColorBrush(gridRow.Cells[i].ColorText);
					//}
				}
				//bool isCellBold=false;
				if(gridRow.Cells[i].Bold==null) {//no override.  Use row bold
					if(gridRow.Bold){
						textBlock.FontWeight=FontWeights.Bold;
					}
				}
				else {
					if(gridRow.Cells[i].Bold==true){
						textBlock.FontWeight=FontWeights.Bold;
					}
				}
				//if(Columns[i].ImageList==null){//text
				//using(SolidBrush brush=new SolidBrush(colorText)){
				//	g.DrawString(gridRow.Cells[i].Text, font, brush, rectangleText, _stringFormat);
				//}
				//}
				gridRow.CanvasText.Children.Add(textBlock);
				double heightCell=heightFont+1;//for single line.
				if(WrapText){
					textBlock.Measure(new Size(textBlock.Width,double.PositiveInfinity));//Measure only fills in the DesiredSize property.
					if(textBlock.DesiredSize.Height>heightCell){
						textBlock.Arrange(new Rect(textBlock.DesiredSize));
						heightCell=textBlock.ActualHeight;
					}
				}
				if(heightCell>gridRow.State.HeightMain){
					gridRow.State.HeightMain=(int)heightCell;
					ComputeRowYposStartingAt(rowI);
				}
			}
			Line lineLower=new Line();
			Canvas.SetZIndex(lineLower,1);//forward from text
			lineLower.X1=0;
			lineLower.Y1=gridRow.State.HeightTotal;
			lineLower.X2=_widthTotal;
			lineLower.Y2=gridRow.State.HeightTotal;
			lineLower.Stroke=new SolidColorBrush(_colorGridline);
			if(rowI==ListGridRows.Count-1) {//last row
				lineLower.Stroke=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(120));
			}
			else if(gridRow.ColorLborder.ToString()!=Colors.Transparent.ToString()) {
				lineLower.Stroke=new SolidColorBrush(gridRow.ColorLborder);
			}
			gridRow.CanvasText.Children.Add(lineLower);
		}

		private void DrawRowBackground(int rowI) {
			GridRow gridRow=ListGridRows[rowI];
			bool needsBackground=false;
			if(gridRow.ColorBackG.ToString()!=Colors.White.ToString()) {//required for value comparison
				needsBackground=true;
			}
			else if(AlternateRowsColored && rowI%2==0){//color even rows
				needsBackground=true;
			}
			for(int i=0;i<gridRow.Cells.Count;i++) {
				if(i>Columns.Count) {
					break;
				}
				if(gridRow.Cells[i].ColorBackG.ToString()==Colors.Transparent.ToString()){// && !gridRow.Cells[i].IsButton) {
					continue;
				}
				needsBackground=true;
			}
			if(needsBackground) {
				if(gridRow.CanvasBackground==null) {
					gridRow.CanvasBackground=new Canvas();
					canvasMain.Children.Add(gridRow.CanvasBackground);
					//Canvas.SetZIndex(gridRow.CanvasBackground,0);
				}
				else{
					gridRow.CanvasBackground.Children.Clear();
				}
			}
			else {
				if(gridRow.CanvasBackground==null) {
					return;
				}
				else{
					gridRow.CanvasBackground.Children.Clear();
					gridRow.CanvasBackground=null;
					return;
				}
			}
			//from here down, we have a CanvasBackground object and we are going to put something on it.
			Canvas.SetTop(gridRow.CanvasBackground,gridRow.State.YPos);
			Color colorRow=Colors.White;//canvasMain.Background is already white, so most rows will require no background.
			//colored row background
			if(gridRow.ColorBackG.ToString()!=Colors.White.ToString()) {//required for value comparison
				colorRow=gridRow.ColorBackG;
			}
			else if(AlternateRowsColored && rowI%2==0){//color even rows
				colorRow=Color.FromRgb(245,255,248);
			}
			if(colorRow!=Colors.White){
				Rectangle rectangle=new Rectangle();
				rectangle.Fill=new SolidColorBrush(colorRow);
				rectangle.Width=_widthTotal;
				rectangle.Height=gridRow.State.HeightTotal;
				//Canvas.SetLeft(rectangle,0);
				//Canvas.SetTop(rectangle,0);
				gridRow.CanvasBackground.Children.Add(rectangle);
			}
			//Color Individual Cells.
			for(int i=0;i<gridRow.Cells.Count;i++) {
				if(i>Columns.Count) {
					break;
				}
				if(gridRow.Cells[i].ColorBackG.ToString()==Colors.Transparent.ToString()){// && !gridRow.Cells[i].IsButton) {
					continue;
				}
				Rectangle rectangle=new Rectangle();
				rectangle.Fill=new SolidColorBrush(gridRow.Cells[i].ColorBackG);
				rectangle.Width=Columns[i].State.Width;
				rectangle.Height=gridRow.State.HeightTotal;
				Canvas.SetLeft(rectangle,Columns[i].State.XPos);
				//Canvas.SetTop(rectangle,0);
				Canvas.SetZIndex(rectangle,1);//local, so it will show in front of row rect.
				gridRow.CanvasBackground.Children.Add(rectangle);
				//}
				//if(gridRow.Cells[i].IsButton) { 
				//	Rectangle rectangleButton=new Rectangle(-hScroll.Value+Columns[i].State.XPos+2,top+2,Columns[i].State.Width-4,hTot-4);
				//	Color colorTop=Color.White;
				//	if(gridRow.Cells[i].ButtonIsPressed){
				//		colorTop=Color.LightGray;
				//	}
				//	using(LinearGradientBrush brushButton = new LinearGradientBrush(rectangleButton,colorTop,Color.LightGray,90)) {
				//		g.FillRectangle(brushButton,rectangleButton); 
				//	}
				//	Color colorOutline=_colorTextDisabled;
				//	if(Enabled){
				//		colorOutline=_colorTitleBottom;
				//	}
				//	using(Pen pen=new Pen(colorOutline)) { 
				//		g.DrawRectangle(pen,rectangleButton);
				//	}
				//}
			}
		}

		///<summary>Covers one row or cell with semitransparent rectangles over the normal rows.</summary>
		private void DrawRowHover(){
			if(_hoverRow==-1){
				if(_rectangleHover is null){
					return;
				}
				canvasMain.Children.Remove(_rectangleHover);
				_rectangleHover=null;
				return;
			}
			//from here down, we are showing a row
			if(_rectangleHover is null){
				_rectangleHover=new Rectangle();
				Canvas.SetZIndex(_rectangleHover,2);
				Color color=Color.FromArgb(70,215,237,243);//50,215,237,245 matches the previous GridOD, but felt too subtle and too blue
				_rectangleHover.Fill=new SolidColorBrush(color);
				canvasMain.Children.Add(_rectangleHover);
			}
			_rectangleHover.Height=ListGridRows[_hoverRow].State.HeightTotal;
			_rectangleHover.Width=_widthTotal;
			Canvas.SetTop(_rectangleHover,ListGridRows[_hoverRow].State.YPos);
			//Canvas.SetLeft(0
		}

		///<summary>Covers selected rows with semitransparent rectangle over the normal rows.</summary>
		private void DrawRowSelections(){
			//User will have specified a selection color.
			//Convert the white portion of that color into transparent
			//So it looks like the color they chose, but it's as transparent as possible over a white background.
			int r=_colorSelectedRow.R;
			int g=_colorSelectedRow.G;
			int b=_colorSelectedRow.B;
			double alpha=Math.Min(r,Math.Min(g,b))/255.0;
			int r2=(int)((r-(1-alpha)*255)/alpha);
			int g2=(int)((g-(1-alpha)*255)/alpha);
			int b2=(int)((b-(1-alpha)*255)/alpha);
			r2=Math.Max(0,Math.Min(255,r2));
			g2=Math.Max(0,Math.Min(255,g2));
			b2=Math.Max(0,Math.Min(255,b2));
			Color color=Color.FromArgb((byte)(alpha * 255),(byte)r2,(byte)g2,(byte)b2);
			if(_selectionMode==GridSelectionMode.OneCell){
				//&& _selectedCell.Col!=-1 && _selectedCell.Row!=-1 && _selectedCell.Row==rowI) {
				//g.FillRectangle(new SolidBrush(ColorSelectedRow),
				//	-hScroll.Value+Columns[_selectedCell.X].State.XPos+1,
				//	top+1,
				//	Columns[_selectedCell.X].State.Width,
				//	hTot);

				if(_selectedCell.Col==-1 || _selectedCell.Row==-1){
					if(_listRectanglesSelected.Count==0){
						return;
					}
					for(int i = _listRectanglesSelected.Count-1;i>=0;i--) {//go backward
						canvasMain.Children.Remove(_listRectanglesSelected[i]);
						_listRectanglesSelected.RemoveAt(i);
					}
					return;
				}
				//cell is selected
				//GridCell gridCell=ListGridRows[_selectedCell.Row].Cells[_selectedCell.Col];//doesn't really help us
				if(_listRectanglesSelected.Count>1){
					for(int i = _listRectanglesSelected.Count-1;i>0;i--) {//go backward, and leave one remaining
						canvasMain.Children.Remove(_listRectanglesSelected[i]);
						_listRectanglesSelected.RemoveAt(i);
					}
				}
				if(_listRectanglesSelected.Count==0){
					Rectangle rectangle=new Rectangle();
					Canvas.SetZIndex(rectangle,2);
					rectangle.Fill=new SolidColorBrush(color);
					canvasMain.Children.Add(rectangle);
					_listRectanglesSelected.Add(rectangle);
				}
				//we now have exactly one rectangle
				Canvas.SetTop(_listRectanglesSelected[0],ListGridRows[_selectedCell.Row].State.YPos);
				Canvas.SetLeft(_listRectanglesSelected[0],Columns[_selectedCell.Col].State.XPos);
				_listRectanglesSelected[0].Width=Columns[_selectedCell.Col].State.Width;
				_listRectanglesSelected[0].Height=ListGridRows[_selectedCell.Row].State.HeightMain;//don't consider note row
				return;
			}
			//from here down, single or multi row.
			//first delete any that shouldn't show
			for(int i = _listRectanglesSelected.Count-1;i>=0;i--) {//go backward
				bool shouldShow = false;
				for(int s = 0;s<_listSelectedIndices.Count;s++) {
					if(ListGridRows[_listSelectedIndices[s]].State.YPos==Canvas.GetTop(_listRectanglesSelected[i])) {
						shouldShow=true;
					}
				}
				if(!shouldShow) {
					canvasMain.Children.Remove(_listRectanglesSelected[i]);
					_listRectanglesSelected.RemoveAt(i);
				}
			}

			//then add any missing
			for(int i=0;i<_listSelectedIndices.Count;i++){
				bool exists=_listRectanglesSelected.Exists(x=>Canvas.GetTop(x)==ListGridRows[_listSelectedIndices[i]].State.YPos);
				if(exists){
					continue;
				}
				Rectangle rectangle=new Rectangle();
				Canvas.SetZIndex(rectangle,2);
				Canvas.SetTop(rectangle,ListGridRows[_listSelectedIndices[i]].State.YPos);
				//Canvas.SetLeft(0
				rectangle.Height=ListGridRows[_listSelectedIndices[i]].State.HeightTotal;
				rectangle.Width=_widthTotal;
				rectangle.Fill=new SolidColorBrush(color);
				canvasMain.Children.Add(rectangle);
				_listRectanglesSelected.Add(rectangle);
			}
			
		}

		///<summary>Draws just the vertical gridlines. ZIndex forward from background.  Only drawn once.</summary>
		private void DrawGridlinesVert(){
			if(_canvasVertLines==null){
				_canvasVertLines=new Canvas();
				canvasMain.Children.Add(_canvasVertLines);
				Canvas.SetZIndex(_canvasVertLines,4);
			}
			else{
				_canvasVertLines.Children.Clear();
			}
			for(int i=0;i<Columns.Count;i++) {
				Line lineRight=new Line();
				lineRight.X1=Columns[i].State.Right;
				lineRight.Y1=0;
				lineRight.X2=Columns[i].State.Right;
				lineRight.Y2=_heightTotal;
				lineRight.Stroke=new SolidColorBrush(_colorGridline);
				_canvasVertLines.Children.Add(lineRight);
			}
		}
		#endregion Methods - Drawing

		#region Methods - Event Handlers
		private void Grid_Loaded(object sender,RoutedEventArgs e) {
			if(DesignerProperties.GetIsInDesignMode(this)){
				return;
			}
			FrameworkElement frameworkElement=this;
			while(true){
				if(frameworkElement.Parent is null){//can't imagine how this could ever happen
					return;
				}
				if(frameworkElement.Parent.GetType().IsSubclassOf(typeof(FrmODBase))){
					FrmODBase frmODBase=(FrmODBase)frameworkElement.Parent;
					frmODBase.PreviewKeyDown+=FrmODBase_PreviewKeyDown;
					return;
				}
				if(!frameworkElement.Parent.GetType().IsSubclassOf(typeof(FrameworkElement))){
					return;
				}
				frameworkElement=(FrameworkElement)frameworkElement.Parent;
			}
		}

		private void textEdit_LostFocus(object sender,EventArgs e) {
			//editBox_Leave wouldn't catch all scenarios
			//This method is also directly invoked programmatically in some cases.
			CellLeave?.Invoke(this,new EventArgs());//_selectedCellOld);
			//if(textEdit.Visibility!=Visibility.Visible){
			//	return;
			//}
			//so this will now all happen regardless of whether we programatically lost focus
			//or whether user just clicked somewhere else.
			textEdit.Visibility=Visibility.Collapsed;
			//if(ListGridRows.Count>_selectedCellOld.Row) {
				//In FormQueryParser, SetFilterControlsAndAction uses an action that calls FillGrid, but number of rows can be different than before.
				//This is a problem when use is editing a cell and clicks Tab.  That should really be fixed, but this makes the grid more resilient.
				//DrawRow(_selectedCellOld.Row);//recomputes that row height
				//because rows are computed as they're drawn, the above bug doesn't seem to be an issue anymore.
			//}
			DrawRow(_selectedCellOld.Row);//so text will show
			DrawRowBackground(_selectedCellOld.Row);
			ComputeRowYposStartingAt(_selectedCellOld.Row);
			LayoutScrolls();
		}

		void textEdit_GotFocus(object sender,RoutedEventArgs e) {
			CellEnter?.Invoke(this, new EventArgs());//_selectedCellOld
		}

		void textEdit_TextChanged(object sender,EventArgs e) {
			ListGridRows[_selectedCell.Row].Cells[_selectedCell.Col].Text=textEdit.Text;
			CellTextChanged?.Invoke(this, new EventArgs());
		}

		private void timerSizing_Tick(object sender, EventArgs e){
			//This fires half a second after resizing movement stops.
			//That way, it typically only fires once when resizing.
			_dispatcherTimerSizing.Stop();
			if(ListGridRows.Count==0){
				return;//if it hasn't filled with rows yet, then don't redraw.
			}
			ClearAll();
			ComputeRows();//wrapping can affect row heights
			ComputeColumns();//For dynamic width columns
			DrawHeaders();
			DrawRows();
			DrawRowSelections();
			DrawGridlinesVert();
			LayoutScrolls();
		}

		private void UserControl_SizeChanged(object sender,System.Windows.SizeChangedEventArgs e) {
			if(_isUpdating){
				return;
			}
			double scrollFraction=1;
			if(scrollV.Maximum-scrollV.LargeChange!=0) {
				scrollFraction=scrollV.Value/(scrollV.Maximum-scrollV.LargeChange);
			}
			if(_dispatcherTimerSizing is null){
				_dispatcherTimerSizing=new DispatcherTimer();
				_dispatcherTimerSizing.Tick += new EventHandler(timerSizing_Tick);
				//Tested 500,400,300,200,100,175,150 milliseconds. 150 was the highest without a noticeable column size adjustment upon opening a frm. The adjustment occurs when a vertical scrollbar is not needed.
				_dispatcherTimerSizing.Interval = TimeSpan.FromMilliseconds(150);
			}
			_dispatcherTimerSizing.Start();
			//todo:
			//ScrollValue=(int)(scrollFraction*(scrollV.Maximum-scrollV.LargeChange));
		}
		#endregion Methods - Event Handlers

		#region Method - Event Handlers - Key
		private void Grid_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(_selectionMode!=GridSelectionMode.OneRow) {
				return;
			}
			if(_listSelectedIndices.Count==0){
				return;
			}
			//from here down, we are moving selected row up or down
			//If you want this to work when grid does not have focus, then the parent form needs to have KeyPreview turned on.
			//This is already the case for FormODBase
			if(e.Key==Key.Down) {
				if(_listSelectedIndices[0]==ListGridRows.Count-1) {//example 4 items, last item is idx 3 which should do nothing
					return;
				}
				int selectedNew=_listSelectedIndices[0]+1;
				_listSelectedIndices.Clear();
				_listSelectedIndices.Add(selectedNew);
				DrawRowSelections();
				ScrollToMakeVisible(selectedNew);
				SelectionCommitted?.Invoke(this,new EventArgs());
				//scrollH.Value=scrollH.Minimum;
			}
			if(e.Key==Key.Up) {
				if(_listSelectedIndices[0]==0) {
					return;
				}
				int selectedNew=_listSelectedIndices[0]-1;
				_listSelectedIndices.Clear();
				_listSelectedIndices.Add(selectedNew);
				DrawRowSelections();
				ScrollToMakeVisible(selectedNew);				
				SelectionCommitted?.Invoke(this,new EventArgs());
				//scrollH.Value=
			}
		}

		
		/// <summary>The grid might not have focus, so the up and down arrows can come through the parent frm.</summary>
		private void FrmODBase_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(this.IsFocused){
				return;//avoid double call
			}
			Point point=Mouse.GetPosition(this);
			if(point.X>=0 && point.X<ActualWidth && point.Y>=0 && point.Y<=ActualHeight){
				//the cursor is hovering over the grid, so it should respond to the up and down arrows
				Grid_PreviewKeyDown(sender,e);
				//OnKeyDown(e);
				return;
			}
			if(ArrowsWhenNoFocus){
				//Example, in FormPatientSelect, arrows will still work when focus is on the input boxes.
				Grid_PreviewKeyDown(sender,e);
				//OnKeyDown(e);
				return;
			}
		}

		void textEdit_KeyDown(object sender,KeyEventArgs e) {
			CellKeyDown?.Invoke(this,e);
			if(e.Handled) {
				return;
			}
			bool isShift=Keyboard.Modifiers==ModifierKeys.Shift;
			if(isShift && e.Key==Key.Enter) {
				int idxSelectionStart=textEdit.SelectionStart;
				textEdit.Text+="\r\n";
				textEdit.SelectionStart=idxSelectionStart+2;
				return;
			}
			if(e.Key==Key.Enter) {//usually move to the next cell
				if(EditableAcceptsCR) {//When multiline it inserts a carriage return instead of moving to the next cell.
					return;
				}
				if(EditableEnterMovesDown){
					//code here is copied from editBox_NextCellRight().
					textEdit.Visibility=Visibility.Collapsed;
					textEdit_LostFocus(textEdit,new EventArgs());
					if(_selectedCellOld.Row==ListGridRows.Count-1) {
						return;//can't move down
					}
					_selectedCell=new ColRow(_selectedCellOld.Col,_selectedCellOld.Row+1);
					CreateEditBox();
					return;
				}
				editBox_NextCellRight();
			}
			if(e.Key==Key.Down) {
				if(EditableAcceptsCR) {//When multiline it moves down inside the text instead of down to the next cell.
					return;
				}
				if(_selectedCellOld.Row<ListGridRows.Count-1) {
					textEdit.Visibility=Visibility.Collapsed;
					_selectedCell=new ColRow(_selectedCellOld.Col,_selectedCellOld.Row+1);
					CreateEditBox();
				}
			}
			if(e.Key==Key.Up) {
				if(EditableAcceptsCR) {//When multiline it moves up inside the text instead of up to the next cell.
					return;
				}
				if(_selectedCellOld.Row>0) {
					textEdit.Visibility=Visibility.Collapsed;
					_selectedCell=new ColRow(_selectedCellOld.Col,_selectedCellOld.Row-1);
					CreateEditBox();
				}
			}
			if(e.Key==Key.Tab) {
				editBox_NextCellRight();
			}
		}
		
		void textEdit_KeyUp(object sender,KeyEventArgs e) {
			//The only thing this method does is grow the textBox as needed to fit the new text
			if(textEdit==null) {
				return;
			}
			if(textEdit.Text=="") {
				return;
			}
			if(textEdit.Height==canvasMain.ActualHeight){
				return;//can't grow because already full height
			}
			FontFamily fontFamily=new FontFamily("Segoe UI");
			int heightFont=(int)Math.Ceiling(fontFamily.LineSpacing * FontSize);
			//not sure if I should really be adding a row of height here.
			int heightText=(int)textEdit.ExtentHeight;//+heightFont;
			if(heightText < EDITABLE_ROW_HEIGHT) {//if it's less than one line
			  heightText=EDITABLE_ROW_HEIGHT;//set it to one line
			}
			if(heightText<=textEdit.ActualHeight+1) {
				return;
			}
			//it needs to grow
			if(textEdit.Height==canvasView.ActualHeight){
				return;//already as tall as it can get
			}
			//add a row of height
			textEdit.Height+=EDITABLE_ROW_HEIGHT;
			//decide if it's spilling off the bottom
			int bottomVisible//in canvasMain coords
				=(int)canvasMain.ActualHeight+ScrollValue;
			if(Canvas.GetTop(textEdit)+textEdit.Height <= bottomVisible){
				return;
			}
			Canvas.SetTop(textEdit,bottomVisible-textEdit.Height);//this can hide upper portion of textbox
			if(Canvas.GetTop(textEdit)<ScrollValue){
				Canvas.SetTop(textEdit,ScrollValue);
				textEdit.Height=canvasView.ActualHeight;
			}
			
			//int bottomVisible=(int)canvasView.ActualHeight-1;
			//if(scrollH.Visibility=Visibility.Visible){
			//if(hScroll.Visible){
			//	bottomVisible-=hScroll.Height;
			//}
			//if(textEdit.Top==0 && textEdit.Bottom==bottomVisible){
			//	return;//already as tall as it can get
			//}
			//textEdit.Height+=ScaleI(EDITABLE_ROW_HEIGHT);
			//if(textEdit.Bottom<=bottomVisible){
			//	return;
			//}
			//textEdit.Top=bottomVisible-textEdit.Height;//this can hide upper portion of textbox
			//if(textEdit.Top<0){
			//	textEdit.Top=0;
			//	textEdit.Height=bottomVisible;
				//if(textBoxEdit is TextBox textB){
				//	textB.ScrollBars=ScrollBars.Vertical;//this crashes
				//}
			//}
		}
		#endregion Method - Event Handlers - Key

		#region Methods - Event Handlers - Mouse
		private void canvasHeaders_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Point point=e.GetPosition(canvasHeaders);
			_mouseDownCol=CanvasPointToCol(point.X);
			if(_mouseDownCol!=-1 && Columns[_mouseDownCol].HeaderClick!=null) {
				Columns[_mouseDownCol].HeaderClick(this,new EventArgs());
				return;
			}
			if(!_sortingAllowByColumn) {
				return;
			}
			if(_mouseDownCol==-1) {
				return;
			}
			SortByColumn(_mouseDownCol);
		}

		private void canvasMain_MouseLeave(object sender,MouseEventArgs e) {
			_hoverRow=-1;
			DrawRowHover();
		}

		private void canvasMain_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			bool gotFocus=Focus();//this is important. See notes in OneCell section
			Point point=e.GetPosition(canvasMain);
			_mouseDownCol=CanvasPointToCol(point.X);
			_mouseDownRow=CanvasPointToRow(point.Y);
			GridClickEventArgs gridClickEventArgs = new GridClickEventArgs(_mouseDownCol,_mouseDownRow,MouseButton.Left);
			if(e.ClickCount==2) {//double click
				CellDoubleClick?.Invoke(this,gridClickEventArgs);
				return;
			}
			//single click from here down
			CellClick?.Invoke(this,gridClickEventArgs);
			if(!AllowSelection) {
				return;//clicks do not trigger selection of rows, but cell click event still gets fired
			}
			switch(_selectionMode) {
				case GridSelectionMode.None:
					return;
				case GridSelectionMode.OneRow:
					List<int> listSelectedIndicesOld=new List<int>(_listSelectedIndices);
					_listSelectedIndices.Clear();
					if(_mouseDownRow==-1){
						break;
					}
					_listSelectedIndices.Add(_mouseDownRow);
					SelectionCommitted?.Invoke(this,new EventArgs());
					DrawRowSelections();
					/*
					if(row>ListGridRows.Count-1){
						return;
					}
					if(col>ListGridRows[row].Cells.Count-1){
						return;//this can happen if programmer forgot to add some cells to a row.
					}
					GridCell gridCell=ListGridRows[row].Cells[col];
					if(gridCell.IsButton) {
						gridCell.ButtonIsPressed=true;
						Refresh(); //Force the "button" styling to repaint in the "clicked" style
						gridCell.ClickEvent.Invoke(this,new EventArgs());
						gridCell.ButtonIsPressed=false;
					}*/
					break;
				case GridSelectionMode.OneCell:
					//The current grid could have another control floating on top of it (edit box, combo box, etc) which may require a LostFocus event to fire.
					//Removing focus will trigger a CellLeave event.
					//Grid is not a containerControl, so setting focus to itself should remove focus from others.
					//this.ActiveControl=null;
					//Focus();//Moved to top of this method
					_listSelectedIndices.Clear();
					//textEdit.Visible=false;//a lot happens right here, including a FillGrid() which sets selectedCell to -1,-1
					//The above comment is no longer true. Does not get set to -1,-1. Not in GridOD, either.
					//This was already done in LostFocus, which happens before this point because of the Focus() at the top of this method.
					//comboBox.Visible=false;
					_selectedCell=new ColRow(_mouseDownCol,_mouseDownRow);
					if(Columns[_selectedCell.Col].IsEditable){// || Columns[_selectedCell.Col].ListDisplayStrings!=null) {
						if(Columns[_selectedCell.Col].IsEditable) {
							CreateEditBox();
						}
						//else if(Columns[_selectedCell.X].ListDisplayStrings!=null) {
						//	CreateComboBox();
						//}
						//When the additional control is created, added to the controls, and given focus, the chain of events stops and the OnClick event never gets fired.
						//Manually fire the OnClick event because we can guarantee that the user did in fact click on a cell at this point in the mouse down event.
//todo: what's this?:
						//OnClick(e);
					}
					/*
					if(_mouseDownRow>ListGridRows.Count-1){
						return;
					}
					if(_mouseDownCol>ListGridRows[_mouseDownRow].Cells.Count-1){
						return;//this can happen if programmer forgot to add some cells to a row.
					}
					cell=ListGridRows[_mouseDownRow].Cells[_mouseDownCol];
					if(cell.IsButton) {
						cell.ButtonIsPressed=true;
						Refresh(); //Force the "button" styling to repaint in the "clicked" style
						cell.ClickEvent.Invoke(this,new EventArgs());
						cell.ButtonIsPressed=false;
					}*/
					DrawRowSelections();
					break;
				case GridSelectionMode.MultiExtended:
					if(_mouseDownRow==-1){
						break;
					}
					if(ControlIsDown()) {
						//we need to remember exactly which rows were selected the moment the mouse down started.
						//Then, if the mouse gets dragged up or down, the rows between mouse start and mouse end
						//will be set to the opposite of these remembered values.
						_listSelectedIndicesWhenMouseDown=new List<int>(_listSelectedIndices);
						if(_listSelectedIndices.Contains(_mouseDownRow)) {
							_listSelectedIndices.Remove(_mouseDownRow);
						}
						else {
							_listSelectedIndices.Add(_mouseDownRow);
						}
					}
					else if(ShiftIsDown()) {
						if(_listSelectedIndices.Count==0) {
							_listSelectedIndices.Add(_mouseDownRow);
						}
						else {
							int fromRow=_listSelectedIndices[0];
							_listSelectedIndices.Clear();
							if(_mouseDownRow<fromRow) {//dragging down
								for(int i=_mouseDownRow;i<=fromRow;i++) {
									_listSelectedIndices.Add(i);
								}
							}
							else {
								for(int i=fromRow;i<=_mouseDownRow;i++) {
									_listSelectedIndices.Add(i);
								}
							}
						}
					}
					else {//ctrl or shift not down
						_listSelectedIndices.Clear();
						_listSelectedIndices.Add(_mouseDownRow);
					}
					DrawRowSelections();
					break;
			}//end switch
			//e.Handled = true;
		}

		private void canvasMain_MouseMove(object sender,MouseEventArgs e) {
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			Point point=e.GetPosition(canvasMain);
			if(!isMouseDown) {
				//if(AllowSelection){
				_hoverRow=CanvasPointToRow(point.Y);
				//else{	_hoverRow=-1;
				//decided not to suppress hover effect for now to be conistent with GridOD.
				DrawRowHover();
				return;
			}
			//From here down, mouse is down
			if(_selectionMode!=GridSelectionMode.MultiExtended) {
				return;
			}
			if(!AllowSelection) {
				return;//dragging does not change selection of rows
			}
			int row=CanvasPointToRow(point.Y);
			if(row==-1 || row==_mouseDownRow) {
				return;
			}
			//because mouse might have moved faster than computer could keep up, we have to loop through all rows between
			if(ControlIsDown()) {
				if(_listSelectedIndicesWhenMouseDown==null) {
					_listSelectedIndices=new List<int>();
				}
				else {
					_listSelectedIndices=new List<int>(_listSelectedIndicesWhenMouseDown);
				}
			}
			else {
				_listSelectedIndices=new List<int>();
			}
			if(_mouseDownRow<row) {//dragging down
				for(int i=_mouseDownRow;i<=row;i++) {
					if(i==-1) {
						continue;
					}
					if(_listSelectedIndices.Contains(i)) {
						_listSelectedIndices.Remove(i);
					}
					else {
						_listSelectedIndices.Add(i);
					}
				}
			}
			else {//dragging up
				for(int i=row;i<=_mouseDownRow;i++) {
					if(_listSelectedIndices.Contains(i)) {
						_listSelectedIndices.Remove(i);
					}
					else {
						_listSelectedIndices.Add(i);
					}
				}
			}
			DrawRowSelections();
		}

		private void canvasMain_MouseRightButtonUp(object sender,MouseButtonEventArgs e) {
			//Context menus are added to grids in various places.
			//We want to add a few extra items to any existing contextMenu or to a null contextMenu:
			//Separator
			//Copy Cell Text
			//Copy Rows
			//Separator
			//Wiki links
			//We can't do this earlier because there is no ContextMenuChanged event in WPF
			if(!ContextMenuShows) {
				return;
			}
			Point point=e.GetPosition(canvasMain);
			_mouseDownRow=CanvasPointToRow(point.Y);
			_mouseDownCol=CanvasPointToCol(point.X);
			if(_mouseDownRow<0){
				return;
			}
			if(_mouseDownCol<0){
				return;
			}
			//select the row or cell, just like if they left clicked.
			GridClickEventArgs gridClickEventArgs=new GridClickEventArgs(_mouseDownCol,_mouseDownRow,MouseButton.Right);
			CellClick?.Invoke(this,gridClickEventArgs);
			if(AllowSelection) {
				switch(_selectionMode) {
					case GridSelectionMode.None:
						break;
					case GridSelectionMode.OneRow:
					case GridSelectionMode.MultiExtended:
						_listSelectedIndices.Clear();
						_listSelectedIndices.Add(_mouseDownRow);
						SelectionCommitted?.Invoke(this,new EventArgs());
						DrawRowSelections();
						break;
					case GridSelectionMode.OneCell:
						_listSelectedIndices.Clear();
						_selectedCell=new ColRow(_mouseDownCol,_mouseDownRow);
						//Do not create a floating textbox when right click
						DrawRowSelections();
						break;
				}
			}
			else{
				//clicks do not trigger selection of rows, but cell click event still gets fired
				//and right click menu still works.
			}
			ContextMenu contextMenu=(ContextMenu)this.ContextMenu;
			if(contextMenu is null){
				contextMenu=new ContextMenu();
				ContextMenu=contextMenu;
			}
			List<MenuItem> listMenuItems=contextMenu.GetMenuItems();
			//Copy Cell-----------------------------------------------------
			MenuItem menuItemCopy = listMenuItems.Find(x => x.Tag?.ToString() == "copy");
			if(menuItemCopy==null) {//Add it the first time
				if(listMenuItems.Count>0){
					contextMenu.AddSeparator();
				}
				//Debug.WriteLine("Copy Cell Text");
				menuItemCopy =new MenuItem("Copy Cell Text",CopyCellClick);
				menuItemCopy.Tag="copy";
				contextMenu.Add(menuItemCopy);
			}
			if(string.IsNullOrEmpty(ListGridRows[_mouseDownRow].Cells[_mouseDownCol].Text)) {
				menuItemCopy.IsEnabled = false;
			}
			else {
				menuItemCopy.IsEnabled = true;
			}
			//Copy Rows----------------------------------------------------
			MenuItem menuItemCopyRows = listMenuItems.Find(x => x.Tag?.ToString() == "copyrows");
			if(menuItemCopyRows==null) {//Add it the first time
				menuItemCopyRows=new MenuItem("Copy Rows",CopyRowsClick);
				menuItemCopyRows.Tag="copyrows";
				contextMenu.Add(menuItemCopyRows);
			}
			if(_listSelectedIndices.Contains(_mouseDownRow)){//If clicked row is selected
				menuItemCopyRows.IsEnabled = true;
			}
			else {
				menuItemCopyRows.IsEnabled = false;
			}
			//Link items----------------------------------------------------
			for(int i=listMenuItems.Count-1;i>=0;i--) {//backward because removing
				if(listMenuItems[i].Tag?.ToString()!="autolink"){
					continue;
				}
				contextMenu.RemoveAt(i);
			}
			StringBuilder stringBuilder = new StringBuilder();
			ListGridRows[_mouseDownRow].Cells.OfType<GridCell>().ToList().ForEach(x => stringBuilder.AppendLine(x.Text));
			stringBuilder.AppendLine(ListGridRows[_mouseDownRow].Note);
			List<MenuItem> listMenuItemsLinks=PopupHelper2.GetContextMenuItemLinks(stringBuilder.ToString(),RightClickLinks);
			for(int i=0;i<listMenuItemsLinks.Count;i++){
				if(i==0){
					contextMenu.AddSeparator();
				}
				contextMenu.Add(listMenuItemsLinks[i]);
			}
			//_mouseClickLocation
			ContextMenu.Visibility=Visibility.Visible;
		}

		private void CopyCellClick(object sender,EventArgs e) {
			try {
				string copyText = ListGridRows[_mouseDownRow].Cells[_mouseDownCol].Text;
				ODClipboard.SetClipboard(copyText);
			}
			catch {
				//show a message box?
			}
		}

		/// <summary>Copies selected rows to clipboard in tab-delimited format</summary>
		private void CopyRowsClick(object sender,EventArgs e) {
			List<GridRow> listGridRowsSelected=new List<GridRow>();
			_listSelectedIndices.ForEach(x => listGridRowsSelected.Add(ListGridRows[x]));
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<listGridRowsSelected.Count;i++) {
				string parsedRow="";
				for(int j=0;j<listGridRowsSelected[i].Cells.Count;j++) {
					if(!string.IsNullOrEmpty(listGridRowsSelected[i].Cells[j].Text)) {
						parsedRow+=listGridRowsSelected[i].Cells[j].Text.Replace("\t","    "); //remove tabs in string so we keep each column separated by tabs
					}
					if(j!=listGridRowsSelected[i].Cells.Count-1) {
						parsedRow+="\t";
					}
				}
				stringBuilder.AppendLine(parsedRow);
			}
			try {
				ODClipboard.SetClipboard(stringBuilder.ToString());
			}
			catch { 
				//show a message box?
			}
		}
		#endregion Methods - Event Handlers - Mouse

		#region Methods - Selections
		///<summary>Throws exceptions.  Use to set a row selected or not.  Can handle values outside the acceptable range.</summary>
		public void SetSelected(int index,bool setValue=true) {
			if(setValue) {//select specified index
				if(_selectionMode==GridSelectionMode.None) {
					throw new Exception("Selection mode is none.");
				}
				if(index<0 || index>ListGridRows.Count-1) {//check to see if index is within the valid range of values
					return;//if not, then ignore.
				}
				if(_selectionMode==GridSelectionMode.OneRow) {
					_listSelectedIndices.Clear();//clear existing selection before assigning the new one.
				}
				if(!_listSelectedIndices.Contains(index)) {
					_listSelectedIndices.Add(index);
				}
			}
			else {//unselect specified index
				if(_listSelectedIndices.Contains(index)) {
					_listSelectedIndices.Remove(index);
				}
			}
			DrawRowSelections();
		}

		///<summary>Throws exceptions.  Allows setting multiple values all at once</summary>
		public void SetSelected(int[] iArray,bool setValue) {
			if(_selectionMode==GridSelectionMode.None) {
				throw new Exception("Selection mode is none.");
			}
			if(_selectionMode==GridSelectionMode.OneRow) {
				throw new Exception("Selection mode is one.");
			}
			for(int i=0;i<iArray.Length;i++) {
				if(setValue) {//select specified index
					if(iArray[i]<0 || iArray[i]>ListGridRows.Count-1) {//check to see if index is within the valid range of values
						return;//if not, then ignore.
					}
					if(!_listSelectedIndices.Contains(iArray[i])) {
						_listSelectedIndices.Add(iArray[i]);
					}
				}
				else {//unselect specified index
					if(_listSelectedIndices.Contains(iArray[i])) {
						_listSelectedIndices.Remove(iArray[i]);
					}
				}
			}
			DrawRowSelections();
		}

		///<summary>Selects or clears all rows.  Throws exceptions, especially when not MultiExtended.</summary>
		public void SetAll(bool setToValue){
			if(_selectionMode==GridSelectionMode.None) {
				throw new Exception("Selection mode is none.");
			}
			if(_selectionMode==GridSelectionMode.OneRow && setToValue==true) {
				throw new Exception("Selection mode is one.");
			}
			if(_selectionMode==GridSelectionMode.OneCell) {
				throw new Exception("Selection mode is OneCell.");
			}
			_listSelectedIndices.Clear();
			if(setToValue) {//select all
				for(int i=0;i<ListGridRows.Count;i++) {
					_listSelectedIndices.Add(i);
				}
			}
			DrawRowSelections();
		}

		///<summary>Throws exceptions.</summary>
		public void SetSelected(ColRow colRow) {
			if(_selectionMode!=GridSelectionMode.OneCell) {
				throw new Exception("Selection mode must be OneCell.");
			}
			_selectedCell=colRow;
//todo: test
			//textEdit?.Dispose();
			if(Columns[_selectedCell.Col].IsEditable) {
				CreateEditBox();
			}
		}

		///<summary>If one row is selected, it returns the index to that row.  If more than one row are selected, it returns the first selected row.  Really only useful for SelectionMode.One.  If no rows selected, returns -1.</summary>
		public int GetSelectedIndex() {
			if(SelectionMode==GridSelectionMode.OneCell) {
				return _selectedCell.Row;
			}
			if(_listSelectedIndices.Count>0) {
				return _listSelectedIndices[0];
			}
			return -1;
		}

		///<summary>Returns the tag of the selected item.  If T does not match the type of tag, it will return the default of T (usually null).</summary>
		public T SelectedTag<T>() {
			if(SelectedIndices.Length > 0) {
				if(ListGridRows[GetSelectedIndex()].Tag is T) {
					return (T)ListGridRows[GetSelectedIndex()].Tag;
				}
			}
			return default(T);
		}

		///<summary>Returns the tags of the selected items.  If T does not match the type of tag, it will not be included in the returned list.</summary>
		public List<T> SelectedTags<T>() {
			List<T> listTags=new List<T>();
			foreach(int selectedIndex in SelectedIndices) {
				if(ListGridRows[selectedIndex].Tag is T) {
					listTags.Add((T)ListGridRows[selectedIndex].Tag);
				}
			}
			return listTags;
		}

		///<summary>Returns a list of tags for all items.  To get only the tags for selected items use SelectedTags().  If T does not match the type of tag, it will not be included in the returned list.</summary>
		public List<T> GetTags<T>() {
			List<T> listTags=new List<T>();
			foreach(GridRow row in ListGridRows) {
				if(row.Tag is T) {
					listTags.Add((T)row.Tag);
				}
			}
			return listTags;
		}
		#endregion Methods - Selections

		#region Methods - Scrolling
		private void Grid_MouseWheel(object sender,MouseWheelEventArgs e) {
			ScrollValue-=e.Delta/3;
		}

		///<summary>Usually called after entering a new list to automatically scroll to the top.</summary>
		public void ScrollToTop() {
			ScrollValue=(int)scrollV.Minimum;//this does all error checking
		}

		///<summary>Usually called after entering a new list to automatically scroll to the end.</summary>
		public void ScrollToEnd() {
			ScrollValue=(int)scrollV.Maximum;//this does all error checking
		}

		///<summary>The index of the row that is the top row displayed on the ODGrid. Also sets ScrollValue.</summary>
		private void ScrollToIndex(int index) {
			if(index>ListGridRows.Count || index < 0) {
				return;
			}
			ScrollValue=ListGridRows[index].State.YPos;
		}

		///<summary>The index of the row that is the last row to be displayed on the ODGrid. Also sets ScrollValue.</summary>
		private void ScrollToIndexBottom(int index) {
			if(index>ListGridRows.Count || index < 0) {
				return;
			}
			ScrollValue=ListGridRows[index].State.YPos+ListGridRows[index].State.HeightTotal-(int)canvasView.ActualHeight;
		}

		///<summary>If the index is currently scrolled out of view, then it scrolls to make that row fully visible.  It only scrolls minimally instead of centering.</summary>
		private void ScrollToMakeVisible(int index) {
			if(index>ListGridRows.Count || index < 0) {
				return;
			}
			if(-scrollV.Value+ListGridRows[index].State.YPos+ListGridRows[index].State.HeightTotal > canvasView.ActualHeight) {
				//bottom of row below lower edge of control
				ScrollToIndexBottom(index);
			}
			if(-scrollV.Value+ListGridRows[index].State.YPos < 0) {//top edge of row above top of grid area
				ScrollToIndex(index);
			}	
			//because of the order above, a cell that is taller than grid will be aligned to the top of the cell.
		}

		private void scrollH_Scroll(object sender,System.Windows.Controls.Primitives.ScrollEventArgs e) {
			Canvas.SetLeft(canvasMain,-scrollH.Value);
			Canvas.SetLeft(canvasHeaders,-scrollH.Value);
			//if(_hScrollVisible && e.OldValue!=e.NewValue) {
			//	HorizScrolled?.Invoke(this,e);
			//}
		}

		private void scrollV_Scroll(object sender,System.Windows.Controls.Primitives.ScrollEventArgs e) {
			//textEdit?.Dispose();//ok to leave.  It will scroll with canvasMain
			Canvas.SetTop(canvasMain,-scrollV.Value);
			this.Focus();
		}

		private void LayoutScrolls() {
			//scrollbars seem to have a natural width in WPF of 17, but we'll try not to use that and depend instead on space available.
			if(_isUpdating){
				return;
			}
			//no way to test suspendLayout state.
			if(Width==0 || Height==0){
				return;
			}
			/*
			if(_hasMultilineHeaders) {//added in R12381, v15?
				TextFormatFlags textFormatFlags=TextFormatFlags.Default;
				if(_hasAutoWrappedHeaders){//added in R16725, v17.1
					textFormatFlags=TextFormatFlags.WordBreak;
				}
				//since the header size is at 96dpi, we need to unscale the font
				using Font font=new Font(_fontHeader.FontFamily,UnscaleFontODZoom(_fontHeader.Size));
				for(int i=0;i<Columns.Count;i++){
					Size sizeText=TextRenderer.MeasureText(Columns[i].Heading,font,new Size(Columns[i].State.Width,int.MaxValue),textFormatFlags);
					int heightThisHeader=sizeText.Height-3;
					if(heightThisHeader>_heightHeader){//96dpi
						_heightHeader=heightThisHeader;
					}
				}
			}*/
			canvasMain.UpdateLayout();//For updated height measurements
			if(_hScrollVisible) {
				//We can't be resetting the scrollH without a reason.
				//scrollH.Value=0;
				//Canvas.SetLeft(canvasMain,0);
				//Canvas.SetLeft(canvasHeaders,0);
				scrollH.Visibility=Visibility.Visible;
				//vScroll.Height=this.Height-OriginY()-scrollH.Height-2;
				//if(vScroll.Height<0){
				//	vScroll.Height=1;
				//}
				//scrollH.Location=new Point(1,this.Height-scrollH.Height-1);
				//scrollH.Width=this.Width-vScroll.Width-2;
				if(_widthTotal<canvasView.ActualWidth) {
					scrollH.IsEnabled=false;
					scrollH.Maximum=_widthTotal-canvasView.ActualWidth;;
					scrollH.Minimum=1;
					scrollH.ViewportSize=canvasView.ActualWidth;
				}
				else {
					scrollH.IsEnabled=true;
					scrollH.Minimum = 0;
					//if we just changed Visibility to true, it will not have laid out yet, so ActualWidth will still be zero.
					//One way to solve this is to use canvasView.ActualWidth instead.
					scrollH.Maximum=_widthTotal-canvasView.ActualWidth;
					scrollH.ViewportSize=canvasView.ActualWidth;
					scrollH.LargeChange=canvasView.ActualWidth;
					scrollH.SmallChange=50;
				}
			}
			else {
				scrollH.Visibility=Visibility.Collapsed;
				//vScroll.Height=this.Height-OriginY()-2;
			}
			//We measure height of canvasView because the height of scrollV is zero if it's not visible, and that messes up the math
			if(canvasView.ActualHeight<=0) {
				return;
			}
			if(_heightTotal<canvasView.ActualHeight) {
				scrollV.Visibility=Visibility.Collapsed;
				scrollV.Value=0;
				Canvas.SetTop(canvasMain,0);
			}
			else {
				scrollV.Visibility=Visibility.Visible;
				scrollV.Minimum = 0;
				scrollV.Maximum=_heightTotal-scrollV.ActualHeight;
				scrollV.ViewportSize=scrollV.ActualHeight;
				scrollV.LargeChange=scrollV.ActualHeight;
				scrollV.SmallChange=(int)(14*3.4f);//it's not an even number so that it is obvious to user that rows moved
			}
			//Adjust scrollbar if it's outside normal range
			ScrollValue=(int)scrollV.Value;
		}
		#endregion Methods - Scrolling

		#region Methods - Sorting
		///<summary>Returns current sort column index.  Use SortForced to maintain current grid sorting after refreshing a grid.  </summary>
		public int GetSortedByColumnIdx(){
			return _sortedByColumnIdx;
		}

		///<summary>Returns current sort order.  Use SortForced to maintain current grid sorting after refreshing a grid.</summary>
		public bool IsSortedAscending(){
			return _isSortedAscending;
		}

		///<summary>Set sortedByColIdx to -1 to clear sorting. Copied from SortByColumn. No need to call fill grid after calling this.  Also used in PatientPortalManager.</summary>
		public void SortForced(int sortedByColumnIdx,bool sortedIsAscending) {
			_isSortedAscending=sortedIsAscending;
			_sortedByColumnIdx=sortedByColumnIdx;
			if(sortedByColumnIdx==-1) {
				return;
			}
			List<GridRow> rowsSorted=new List<GridRow>();
			for(int i=0;i<ListGridRows.Count;i++) {
				rowsSorted.Add(ListGridRows[i]);
			}
			if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.StringCompare) {
				rowsSorted.Sort(SortStringCompare);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.DateParse) {
				rowsSorted.Sort(SortDateParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.ToothNumberParse) {
				rowsSorted.Sort(SortToothNumberParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.AmountParse) {
				rowsSorted.Sort(SortAmountParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.TimeParse) {
				rowsSorted.Sort(SortTimeParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.VersionNumber) {
				rowsSorted.Sort(SortVersionParse);
			}
			BeginUpdate();
			ListGridRows.Clear();
			for(int i=0;i<rowsSorted.Count;i++) {
				ListGridRows.Add(rowsSorted[i]);
			}
			EndUpdate();
			_sortedByColumnIdx=sortedByColumnIdx;//Must be set again since set to -1 in EndUpdate();
		}

		///<summary>Swaps two rows in the grid. Returns false if either of the indexes is greater than the number of rows in the grid.</summary>
		public bool SwapRows(int indxMoveFrom,int indxMoveTo) {
			if(ListGridRows.Count<=Math.Max(indxMoveFrom,indxMoveTo)
				|| Math.Min(indxMoveFrom,indxMoveTo)<0) 
			{
				return false;
			}
			BeginUpdate();
			GridRow dataRowFrom=ListGridRows[indxMoveFrom];
			ListGridRows[indxMoveFrom]=ListGridRows[indxMoveTo];
			ListGridRows[indxMoveTo]=dataRowFrom;
			EndUpdate();
			return true;
		}

		///<summary>Gets run on mouse down on a column header.</summary>
		private void SortByColumn(int mouseDownCol) {
			if(mouseDownCol==-1) {
				return;
			}
			if(_sortedByColumnIdx==mouseDownCol) {//already sorting by this column
				_isSortedAscending=!_isSortedAscending;//switch ascending/descending.
			}
			else {
				_isSortedAscending=true;//start out ascending
				_sortedByColumnIdx=mouseDownCol;
			}
			List<GridRow> listGridRowsSorted=new List<GridRow>();
			for(int i=0;i<ListGridRows.Count;i++) {
				listGridRowsSorted.Add(ListGridRows[i]);
			}
			if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.StringCompare) {
				listGridRowsSorted.Sort(SortStringCompare);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.DateParse) {
				listGridRowsSorted.Sort(SortDateParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.ToothNumberParse) {
				listGridRowsSorted.Sort(SortToothNumberParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.AmountParse) {
				listGridRowsSorted.Sort(SortAmountParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.TimeParse) {
				listGridRowsSorted.Sort(SortTimeParse);
			}
			else if(Columns[_sortedByColumnIdx].SortingStrategy==GridSortingStrategy.VersionNumber) {
				listGridRowsSorted.Sort(SortVersionParse);
			}
			BeginUpdate();
			ListGridRows.Clear();
			for(int i=0;i<listGridRowsSorted.Count;i++) {
				ListGridRows.Add(listGridRowsSorted[i]);
			}
			EndUpdate();
			_sortedByColumnIdx=mouseDownCol;//Must be set again since set to -1 in EndUpdate();
			if(_sortingAllowByColumn) { //only check this if sorting by column is enabled for the grid
				ColumnSorted?.Invoke(this,new EventArgs());
			}
			DrawHeaders();
		}

		private int SortStringCompare(GridRow row1,GridRow row2) {
			string textRow1=row1?.Cells[_sortedByColumnIdx].Text??"";
			return (_isSortedAscending?1:-1)*textRow1.CompareTo(row2?.Cells[_sortedByColumnIdx].Text??"");
		}

		private int SortDateParse(GridRow row1,GridRow row2) {
			string raw1=row1.Cells[_sortedByColumnIdx].Text;
			string raw2=row2.Cells[_sortedByColumnIdx].Text;
			DateTime date1=DateTime.MinValue;
			DateTime date2=DateTime.MinValue;
			//TryParse is a much faster operation than Parse in the event that the input won't parse to a date.
			if(DateTime.TryParse(raw1,out date1) &&
				DateTime.TryParse(raw2,out date2)) {
				return (_isSortedAscending?1:-1)*date1.CompareTo(date2);
			}
			else { //One of the inputs is not a date so default string compare.
				return SortStringCompare(row1,row2);
			}
		}

		private int SortTimeParse(GridRow row1,GridRow row2) {
			string raw1=row1.Cells[_sortedByColumnIdx].Text;
			string raw2=row2.Cells[_sortedByColumnIdx].Text;
			TimeSpan time1;
			TimeSpan time2;
			//TryParse is a much faster operation than Parse in the event that the input won't parse to a date.
			if(TimeSpan.TryParse(raw1,out time1) &&
				TimeSpan.TryParse(raw2,out time2)) {
				return (_isSortedAscending?1:-1)*time1.CompareTo(time2);
			}
			else { //One of the inputs is not a date so default string compare.
				return SortStringCompare(row1,row2);
			}
		}

		private int SortToothNumberParse(GridRow row1,GridRow row2) {
			/*
			//remember that teeth could be in international format.
			//fail gracefully
			string raw1=row1.Cells[_sortedByColumnIdx].Text;
			string raw2=row2.Cells[_sortedByColumnIdx].Text;
			if(!Tooth.IsValidEntry(raw1) && !Tooth.IsValidEntry(raw2)) {//both invalid
				return 0;
			}
			int retVal=0;
			if(!Tooth.IsValidEntry(raw1)) {//only first invalid
				retVal=-1; ;
			}
			else if(!Tooth.IsValidEntry(raw2)) {//only second invalid
				retVal=1; ;
			}
			else {//both valid
				string tooth1=Tooth.Parse(raw1);
				string tooth2=Tooth.Parse(raw2);
				int toothInt1=Tooth.ToInt(tooth1);
				int toothInt2=Tooth.ToInt(tooth2);
				retVal=toothInt1.CompareTo(toothInt2);
			}
			return (_isSortedAscending?1:-1)*retVal;*/
			return -1;
		}

		private int SortVersionParse(GridRow row1,GridRow row2) {
			Version v1, v2;
			if(!Version.TryParse(row1.Cells[_sortedByColumnIdx].Text,out v1)) {
				v1=new Version();//0.0.0.0
			}
			if(!Version.TryParse(row2.Cells[_sortedByColumnIdx].Text,out v2)) {
				v2=new Version();//0.0.0.0
			}
			return (_isSortedAscending?1:-1)*v1.CompareTo(v2);
		}

		private int SortAmountParse(GridRow row1,GridRow row2) {
			//This is here because AmountParse does not sort correctly when the amount contains non-numeric characters
			//We could improve this later with some kind of grid text cleaner that is called before running this sort.
			string raw1=row1.Cells[_sortedByColumnIdx].Text;			
			raw1=raw1.Replace("$","");
			string raw2=row2.Cells[_sortedByColumnIdx].Text;
			raw2=raw2.Replace("$","");
			Decimal amt1=0;
			Decimal amt2=0;
			if(raw1!="") {
				try {
					amt1=Decimal.Parse(raw1);
				}
				catch {
					return 0;//shouldn't happen
				}
			}
			if(raw2!="") {
				try {
					amt2=Decimal.Parse(raw2);
				}
				catch {
					return 0;//shouldn't happen
				}
			}
			return (_isSortedAscending?1:-1)*amt1.CompareTo(amt2);
		}
		#endregion Methods - Sorting		

		#region Methods - private
		///<summary>Returns row. -1 if no valid row.  Supply the y position in DiPs of canvasMain.</summary>
		private int CanvasPointToRow(double y) {
			for(int i=0;i<ListGridRows.Count;i++) {
				if(!ListGridRows[i].State.Visible){
					continue;
				}
				if(y>ListGridRows[i].State.YPos+ListGridRows[i].State.HeightTotal) {
					continue;//clicked below this row.
				}
				return i;
			}
			return -1;
		}

		///<summary>Returns col.  Supply the x position in DiPs of canvasMain. -1 if no valid column.</summary>
		private int CanvasPointToCol(double x) {
			int colRight;//the right edge of each column
			for(int i=0; i < Columns.Count; i++) {
				colRight=0;
				for(int c=0; c < i + 1; c++) {
					colRight+= Columns[c].State.Width;
				}
				if(x > colRight) {
					continue;//clicked to the right of this col
				}
				return i;
			}
			return -1;
		}

		///<summary>This clears all the children from all the canvases and clears out the various selection and hover rectangles.  Called from EndUpdate and from resize.</summary>
		private void ClearAll(){
			canvasMain.Children.Clear();//this does not set the children to null, however
			canvasMain.Children.Add(textEdit);//this one shouldn't get removed from the canvas
			for(int i=0;i<ListGridRows.Count;i++){
				ListGridRows[i].CanvasText=null;
				ListGridRows[i].CanvasBackground=null;
			}
			_listRectanglesSelected=new List<Rectangle>();
			_rectangleHover=null;
			_canvasVertLines=null;
		}

		///<summary>Computes the position of each column and the overall width.  Called from EndUpdate, when adding columns, when Size changes, or scale changes.</summary>
		private void ComputeColumns() {
			if(_hScrollVisible) {
				for(int i = 0;i<Columns.Count;i++) {
					Columns[i].State.Width=Columns[i].ColWidth;
				}
			}
			else {
				float sumDynamic=0;//sum of the weights of the dynamic columns, typically about 1 or 2.
				int widthFixedSum=0;
				for(int i=0;i<Columns.Count;i++){
					//if(ListGridColumns[i].ColWidth<1){
						//todo: handle this in the ColWidth property instead
						//throw new ApplicationException("Grid column width less than 1 not allowed.");
						//ListGridColumns[i].State.Width=20;//just temporary
					//}
					if(Columns[i].IsWidthDynamic){
						sumDynamic+=Columns[i].DynamicWeight;
					}
					else{
						Columns[i].State.Width=Columns[i].ColWidth;
						widthFixedSum+=Columns[i].State.Width;
					}
				}
				
				string name=this.Name;
				//Debug.WriteLine(name);
				if(sumDynamic>0){
					int widthExtra=(int)ActualWidth-2-widthFixedSum;
					//if(_printWidth>0){
					//	widthExtra=_printWidth-2-widthFixedSum;
					//}
					if(scrollV.Visibility==Visibility.Visible){
						widthExtra-=(int)scrollV.Width;
					}
					for(int i=0;i<Columns.Count;i++){
						if(Columns[i].IsWidthDynamic){
							//example sum=1+2.5, width=350/3.5*2.5=250
							Columns[i].State.Width=Math.Max((int)(widthExtra/sumDynamic*Columns[i].DynamicWeight),20);
							//The min width width of 20 is arbitrary, but no dynamic column should be narrower than that.
						}
					}
				}
				else if(Columns.Count>0 ) {//resize the last column automatically
					int widthExtra=(int)ActualWidth-2-widthFixedSum+Columns[Columns.Count-1].ColWidth;
					//if(_printWidth>0){
					//	widthExtra=_printWidth-2-widthFixedSum+ScaleI(Columns[Columns.Count-1].ColWidth);
					//}
					if(scrollV.Visibility==Visibility.Visible){
						widthExtra-=(int)scrollV.Width;
					}
					if(widthExtra>0){
						Columns[Columns.Count-1].State.Width=widthExtra;
					}
				}
			}
			//widths are all set
			for(int i=0;i<Columns.Count;i++){
				if(i==0){
					Columns[i].State.XPos=0;
				}
				else{
					Columns[i].State.XPos=Columns[i-1].State.XPos+Columns[i-1].State.Width;
				}
				Columns[i].State.Right=Columns[i].State.XPos+Columns[i].State.Width;
			}
			if(Columns.Count>0) {
				_widthTotal=Columns[Columns.Count-1].State.Right;
			}
			canvasMain.Width=_widthTotal;
		}

		///<summary>Only called once from EndUpdate(). Sets each row to a default single row height. Once a row is drawn, the height will be recalculated to handle text wrapping, notes, etc.</summary>
		private void ComputeRows() {
			_heightTotal=0;
			if(ListGridRows.Count==0){
				return;
			}
			//_widthNote=0;
			//if(0 <= NoteSpanStart && NoteSpanStart < Columns.Count
			//	&&  0 < NoteSpanStop && NoteSpanStop <= Columns.Count)
			//{
			//	for(int i=NoteSpanStart;i<=NoteSpanStop;i++) {
			//		_widthNote+=Columns[i].State.Width;
			//	}
			//}
			//_heightImage=0;
			_hasEditableColumn=false;
			//for(int i=0;i<Columns.Count;i++) {
				//if(Columns[i].IsEditable){// || Columns[i].ListDisplayStrings != null) {
				//	_hasEditableColumn=true;
				//}
				//if(Columns[i].ImageList!=null) {
				//	if(ScaleI(Columns[i].ImageList.ImageSize.Height)> _heightImage) {
				//		_heightImage=ScaleI(Columns[i].ImageList.ImageSize.Height)+1;
				//	}
				//}
			//}
			DateTime dt1=DateTime.Now;
			FontFamily fontFamily=new FontFamily("Segoe UI");
			int heightFont=(int)Math.Ceiling(fontFamily.LineSpacing * FontSize);
			int heightEach=heightFont+1;//assume one row
			if(_hasEditableColumn) {
				heightEach=EDITABLE_ROW_HEIGHT;
			}
			//if(_heightImage>heightEach){//_heightImage is already scaled to current dpi
			//	heightEach=_heightImage;
			//}
			for(int i=0;i<ListGridRows.Count;i++) {
				//if(doActualCalc){
					//Slow on big sets of rows (over 200), so this should only be done on smaller numbers of rows.
					//ComputeRowHeightOne(i);
				//}
				//else{
					//This is just a starting point. As rows are shown, they we be truly calculated.
					ListGridRows[i].State.HeightMain=heightEach;
					ListGridRows[i].State.IsHeightCalculated=false;
				//}
			}
			DateTime dt2=DateTime.Now;
			TimeSpan timeSpanHeights=dt2-dt1;//put a breakpoint below this line to look at the timespan
			ComputeRowYposStartingAt(0);
			//Always follow this up with LayoutScrolls because of _heightTotal
		}

		private void ComputeRowYposStartingAt(int startingAtIdx){
			DateTime dt1=DateTime.Now;
			int yPos=0;
			if(startingAtIdx>0){
				yPos=ListGridRows[startingAtIdx-1].State.YPos+ListGridRows[startingAtIdx-1].State.HeightTotal;
			}
			for(int i=startingAtIdx;i<ListGridRows.Count;i++) {
				if(ListGridRows[i].State.YPos!=yPos){
					ListGridRows[i].State.YPos=yPos;
					if(ListGridRows[i].CanvasBackground!=null){
						Canvas.SetTop(ListGridRows[i].CanvasBackground,yPos);
					}
					if(ListGridRows[i].CanvasText!=null){
						Canvas.SetTop(ListGridRows[i].CanvasText,yPos);
					}
				}
				if(!ListGridRows[i].State.Visible){//child not dropped down
					continue;//to the next row. Don't increment yPos
				}
				yPos+=ListGridRows[i].State.HeightTotal;
			}
			DateTime dt2=DateTime.Now;
			TimeSpan timeSpanYpos=dt2-dt1;//put a breakpoint below this line to look at the timespans
			_heightTotal=yPos;
			canvasMain.Height=_heightTotal;
		}

		private bool ControlIsDown() {
			return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
		}

		private void CreateEditBox() {
			//int hScrollBarHeight=0;
			//if(HScrollVisible) {
			//	hScrollBarHeight=SystemInformation.HorizontalScrollBarHeight;
			//}
			//Check if new edit box location is below the display screen
			int editBoxLocationBottom=ListGridRows[_selectedCell.Row].State.YPos+ListGridRows[_selectedCell.Row].State.HeightMain;
			if(editBoxLocationBottom > ScrollValue+canvasView.ActualHeight){
				//scroll down
				ScrollValue=editBoxLocationBottom-(int)canvasView.ActualHeight;
			}
			//If new edit box location is above the display screen
			else if(ListGridRows[_selectedCell.Row].State.YPos<ScrollValue){
				//scroll up
				ScrollValue=ListGridRows[_selectedCell.Row].State.YPos;
			}
			//textEdit.IsMultiline=true;//this is slightly different than original because only one property
			if(Columns[_selectedCell.Col].TextAlign==HorizontalAlignment.Right) {
				textEdit.HorizontalContentAlignment=HorizontalAlignment.Right;
			}
			Canvas.SetLeft(textEdit,Columns[_selectedCell.Col].State.XPos-1);
			textEdit.Width=Columns[_selectedCell.Col].State.Width+1;
			if(ListGridRows[_selectedCell.Row].State.HeightMain > canvasView.ActualHeight){
				//Single row is taller than the entire grid
				textEdit.Height=canvasView.ActualHeight;
				Canvas.SetTop(textEdit,ScrollValue-1);
			}
			else{//normal
				textEdit.Height=ListGridRows[_selectedCell.Row].State.HeightMain+1;
				Canvas.SetTop(textEdit,ListGridRows[_selectedCell.Row].State.YPos-1);
			}		
			//If a row color is set manually, that color will also show up for this edit box. No support for single cell colors.
			textEdit.Background=new SolidColorBrush(ListGridRows[_selectedCell.Row].ColorBackG);
			//textEdit.Font=new Font(_fontCell.FontFamily,ScaleMS(_fontCell.Size),_fontCell.Style);
			textEdit.Text=ListGridRows[_selectedCell.Row].Cells[_selectedCell.Col].Text;
//todo: why do we need this?: I think we always intercept it to handle cell advancement.
			//textEdit.AcceptsTab=true;
			textEdit.Visibility=Visibility.Visible;
			if(EditableAcceptsCR) {//Allow the edit box to handle carriage returns/multiline text.
				//we might need multiline that does not accept CR?  Why?
				textEdit.TextWrapping=TextWrapping.Wrap;
				textEdit.AcceptsReturn=true;
			}
			else {
//todo:
				textEdit.SelectAll();//Only select all when not multiline (editableAcceptsCR) i.e. proc list for editing fees selects all for easy overwriting.
			}
			//Set the cell of the current editBox so that the value of that cell is saved when it loses focus (used for mouse click).
			_selectedCellOld=new ColRow(_selectedCell.Col,_selectedCell.Row);
			textEdit.Focus();
		}

		private void editBox_NextCellRight() {
			textEdit.Visibility=Visibility.Collapsed;//This fires editBox_LostFocus, which is where we call CellLeave.
			//The line above is not causing LostFocus to fire, so I'm going to call it manually
			textEdit_LostFocus(textEdit,new EventArgs());
			//find the next editable cell to the right.
			for(int i= _selectedCellOld.Col+1; i < Columns.Count; i++) {
				if(Columns[i].IsEditable){//textbox
					_selectedCell=new ColRow(i,_selectedCellOld.Row);
					CreateEditBox();
					return;
				}
				//if(Columns[i].ListDisplayStrings!=null) {//dropdown
				//	_selectedCell=new Point(i,_selectedCellOld.Y);
				//	CreateComboBox();
				//	return;
				//}
			}
			//can't move to the right, so attempt to move down.
			if(_selectedCellOld.Row==ListGridRows.Count-1) {
				return;//can't move down
			}
			int nextCellToRight=-1;
			for(int i=0; i < Columns.Count; i++) {
				if(Columns[i].IsEditable) {
					nextCellToRight= i;
					break;
				}
			}
			//guaranteed to have a value. Either the cell below, or possibly below and left.
			_selectedCell=new ColRow(nextCellToRight,_selectedCellOld.Row+1);
			CreateEditBox();
		}

		///<summary>So that scroll to end always works properly.</summary>
		private void FillLastPage(){
			/*
			if(ListGridRows.Count==0){
				return;
			}
			int rowStart=0;//ListGridRows.Count-1;
			for(int i=ListGridRows.Count-1;i>=0;i--) {//start at the end and work backward
				if(ListGridRows[i].State.YPos+ListGridRows[i].State.HeightTotal > _heightTotal-vScroll.Height){//lower edge of row below top of grid area
					continue;
				}
				//we have found the row that would be at the top of the last page
				rowStart=i;
				break;
			}
			for(int i=rowStart;i<ListGridRows.Count;i++) {
				if(ListGridRows[i].Cells.Count==0){
					FillRow(i);	
				}
				ComputeRowHeightOne(g,i);
			}
			ComputeRowYposStartingAt(rowStart);*/
		}

		///<summary>Fills the cells.  Preserves State.YPos and HeightMain</summary>
		private void FillRow(int rowI){
			GridRow.GridRowState gridRowState=ListGridRows[rowI].State.Copy();
			//if(_listDataRows!=null){
			//	ListGridRows[rowI]=FuncConstructGridRow(_listDataRows[rowI]);
			//}
			ListGridRows[rowI].State.YPos=gridRowState.YPos;
			ListGridRows[rowI].State.HeightMain=gridRowState.HeightMain;
			//None of the other state fields have info in them yet
			//row height should be recalculated separately after this
		}

		private bool ShiftIsDown() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
		}
		#endregion Methods - private

		#region Class - GridColumnCollection
		///<summary>Nested class for the collection of GridColumns.</summary>
		public class GridColumnCollection{
			///<summary>The internal list of columns, exposed through methods.</summary>
			private List<GridColumn> _listGridColumns;
			///<summary>The gridOD that this collection is attached to.</summary>
			private Grid _gridParent;

			public GridColumnCollection(Grid grid){
				_listGridColumns=new List<GridColumn>();
				_gridParent=grid;
			}

			public int Count{
				get{
					return _listGridColumns.Count;
				}
			}

			///<summary></summary>
			public GridColumn this[int index]{
				get { 
					return _listGridColumns[index];
				}
			}

			public void Add(GridColumn gridColumn){
				_listGridColumns.Add(gridColumn);
				if(_gridParent._isUpdating){
					return;
				}
//todo: test. This seems pointless since they should always call EndUpdate to do this.
				_gridParent.ComputeColumns();

			}

			public void AddRange(IEnumerable<GridColumn> collection){
				_listGridColumns.AddRange(collection);
				if(_gridParent._isUpdating){
					return;
				}
//todo: test. This seems pointless since they should always call EndUpdate to do this.
				_gridParent.ComputeColumns();
			}

			public void Clear(){
				_listGridColumns.Clear();
				if(_gridParent._isUpdating){
					return;
				}
				_gridParent.ComputeColumns();
			}

			public List<GridColumn>.Enumerator GetEnumerator(){
				return _listGridColumns.GetEnumerator();
			}

			///<summary>Gets the index of the column with the specified heading.</summary>
			public int GetIndex(string heading){
				for(int i=0;i<_listGridColumns.Count;i++){
					if(_listGridColumns[i].Heading==heading){
						return i;
					}
				}
				return -1;//not found
			}

			public IEnumerable<T> Select<T>(Func<GridColumn,T> selector){
				return _listGridColumns.Select(selector);
			}

			public int Sum(Func<GridColumn,int> selector){
				return _listGridColumns.Sum(selector);
			}

			///<summary>Only use this only for read, not write.</summary>
			public List<GridColumn> ToList(){
				List<GridColumn> listGridColumns=new List<GridColumn>();
				for(int i=0;i<_listGridColumns.Count;i++){
					listGridColumns.Add(_listGridColumns[i].Copy());
				}
				return listGridColumns;
			}

			///<summary>Width of all columns summed.</summary>
		
			public int WidthAll(){
				int retVal=0;
				for(int i=0;i<_listGridColumns.Count;i++){
					retVal+=_listGridColumns[i].State.Width;
				}
				return retVal;
			}

		}

		#endregion Class - GridColumnCollection
	}

	#region outside class
	///<summary>Specifies the selection behavior of a DataGrid.</summary>
	public enum GridSelectionMode {
		///<summary>Nothing can be selected.</summary>
		None,
		///<summary>Only one row can be selected.</summary>
		OneRow,
		///<summary>Only one cell can be selected.</summary>
		OneCell,
		///<summary>Multiple rows can be selected, and the user can use the SHIFT, CTRL, and arrow keys to make selections</summary>
		MultiExtended,
	}

	///<summary></summary>
	public class GridClickEventArgs {
		///<summary></summary>
		public GridClickEventArgs(int col,int row,MouseButton mouseButton) {
			this.Col=col;
			this.Row=row;
			this.Button=mouseButton;
		}
		public int Row { get; }
		public int Col { get; }
		public MouseButton Button { get; }
	}

	///<summary>We used to use Point for this, but WPF point is double instead of int, which doesn't really work. And this can also be null, so we don't have to deal with -1,-1.</summary>
	public class ColRow{
		public int Col;
		public int Row;

		public ColRow(int col,int row){
			Col=col;
			Row=row;
		}

		///<summary>True if Row and Col are equal.</summary>
		public bool IsEquivalentTo(ColRow colRow){
			if(colRow.Col==Col && colRow.Row==Row){
				return true;
			}
			return false;
		}
	}
	#endregion outside class
}

#region Ignore Notes
/*
With WPF, we now have a graphics card to consider, so the strategy can be very different from WinForms, while still making use of lessons learned there.
We must have massive scalability that can handle millions of rows with ease.
Scrolling must be very smooth and fast in all cases.
Testing results with 8 columns:
-By putting each row in its own canvas, I was able to double the speed. and also make clearing the canvas instantaneous.
-It now takes 0.27s to draw 500 rows, which is not annoying.

Strategy:
1. Use a canvasMain that is the full size of data.  Max height is 1.8 x 10^308, so this will always work.
2. For reasonably small grids, like less than 500 rows, draw the entire grid right at the beginning. It will end up on the graphics card, and then scrolling will be smooth. The advantage is that all row heights will be accurate, so scrollbar won't jump.
3. For over 500 rows, must use a different strategy. Downside is that scrollbar might jump a little.
4. A full page is about 36 rows tall, so if we just draw what we see, it will be blazing fast.

Strategy for grids with over 500 rows.
1. Only add gridlines, cell text, etc for rows that are visible, plus a bit(5 rows?) up and a bit down down.
2. As user scrolls, add more controls on the incoming edge, and remove controls on the leaving edge.  Keeping the number of controls small might help. 
3. Set all row heights to a default of 1 row height instead of wasting time calculating.
4. Don't calculate row heights in advance, but only as they are visible.  Unlike WF grid, heights won't change due to dropdowns.
5. After calculating all row heights that are showing, update yPos for all rows showing and all below.(blazing fast)
   Also, update total height, and if it changed, then layout scrollbar
6. If there might be over 50k rows, then programmer might want to use Func to fill rows on the fly.  This is a separate issue from the above.
7. In massive grids, the scrollbar will jump a little as it recalculates row heights.
The next remaining slow step, noticeable at about 100k rows, causes a pause of a few seconds when loading:
List<DataRow> listDataRows=_table.Select().ToList();//We could work on eliminating it, but not urgent

zIndex:
0=Row backgrounds
  0a. Each colored row background is a separate canvas, attached to the GridRow.
  0b. There typically aren't many of these, so most canvases will be null.
  0c. Cell backgrounds are also on these canvases, with local zIndex of 1.
1=Selections
2=Hover
3=Text and horizontal lines
  4a. Each row is a separate canvas, attached to the GridRow. The reason is because rows must shift down as rows above are recalculated.
  4b. Text is textblock. FormattedText wouldn't let us move rows as heights change
  4c. Line under each row. LineGeometry: see above.
4=Vertical Gridlines have their own canvas
5=TextBox and comboBox
The values for Ypos, XPos, etc. refer to the upper left of each cell.
At different dpi, gridlines might not be one pixel wide.
We have to draw text first in order to measure the row height before we can draw backgrounds and gridlines.
Cell contents are drawn 
cell height is set
Row and cell backgrounds should butt up against each other nicely. 
Gridlines are drawn right and bottom for each cell.
For example, if a cell is 40x10, then the fill rect is size 40x10, drawn at 0,0.
Then, separately, its own gridlines are drawn at 39 and 9 on right and bottom
These draw right on top of the filled rectangle. In this way, all cells are the same size.

Other basics of how the grid works internally (the notes below are very outdated, but patterns aren't quite in place yet to know how to reword):
ComputeColumns as they are added and in EndUpdate.
SetFonts as needed, but this should always be followed by the next two.
ComputeRows (depends on fonts.  Follow with LayoutScrolls)
LayoutScrolls (depends on _heightTotal from ComputeRows. It can be called without anything after it)
Invalidate
Paint only paints the rows that are showing

*/
#endregion Ignore Notes
