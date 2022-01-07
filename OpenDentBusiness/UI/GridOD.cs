using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using PdfSharp.Drawing;
using OpenDental.Thinfinity;
using PdfSharp.Pdf;

namespace OpenDental.UI {
	///<summary>A custom grid/table control used extensively in OD.  Jordan is the only one allowed to edit this class.</summary>
	[DefaultEvent("CellDoubleClick")]
	public class GridOD:System.Windows.Forms.UserControl {
		#region Fields - Public
		///<summary>This grid was created solely to draw to a sheet.</summary>
		public bool IsForSheets;
		///<summary>When drawing to sheets, additional info about rows.</summary>
		public List<GridSheetRow> ListGridSheetRows;
		///<summary>Used when calculating row positions.  Set to 0 when using in FormSheetFillEdit.</summary>
		public int SheetBottomMargin;
		public int SheetPageHeight;
		///<summary>Height of drawn grid on this page.  Set using CalculateHeights() from EndUpdate()</summary>
		public int SheetPrintHeight;
		///<summary>Used when calculating row positions.  Set to 0 when using in FormSheetFillEdit.</summary>
		public int SheetTopMargin;
		///<summary>The position on the page that this grid will print. If this is halfway down the second page, 1100px tall, this value should be 1650, not 550.</summary>
		public int SheetYPos;
		#endregion Fields - Public

		#region Fields - Private Static Drawing
		//These static drawing objects never get disposed; just used repeatedly by all the various grids.
		//Colors-------------------------------------------------------------------------------------------------
		///<summary>The gray text when a button is disabled.</summary>
		private static Color _colorTextDisabled=Color.FromArgb(161,161,146);
		private static Color _colorTitleTop=Color.FromArgb(156,175,230);
		///<summary>This is a little bit misused.  Review.</summary>
		private static Color _colorTitleBottom=Color.FromArgb(60,90,150);
		//Brushes------------------------------------------------------------------------------------------------
		private static SolidBrush _brushBackground=new SolidBrush(Color.FromArgb(202,212,222));
		private static SolidBrush _brushHeaderBackground=new SolidBrush(Color.FromArgb(223,234,245));
		private static SolidBrush _brushHeaderText=(SolidBrush)Brushes.Black;
		private static SolidBrush _brushTitleText=(SolidBrush)Brushes.White;
		//Pens---------------------------------------------------------------------------------------------------
		///<summary>Review.  Also separates column headers.  Also used in a few other places where darker line needed.  Maybe change name _penGridlineDark?</summary>
		private static Pen _penColumnSeparator=new Pen(Color.FromArgb(120,120,120));
		///<summary>Seems to only be used once as the line between title and headers.</summary>
		private static Pen _penGridInnerLine=new Pen(Color.FromArgb(102,102,122));
		private static Pen _penGridline=new Pen(Color.FromArgb(180,180,180));
		private static Pen _penOutline=new Pen(Color.FromArgb(47,70,117));
		#endregion Fields - Private Static Drawing

		#region Fields - Private Drawing
		//These fields change when dpi changes, so they are not static.  All disposed
		private LinearGradientBrush _brushTitleBackground;//=new LinearGradientBrush(new Point(0,0),new Point(0,18),_colorTitleTop,_colorTitleBottom);
		//Fonts--------------------------------------------------------------------------------------------------
		private Font _fontCell;//=new Font(FontFamily.GenericSansSerif,8.5f);//this.Font is ignored
		private Font _fontCellBold;//=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);
		private Font _fontUnderline;//=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Underline);
		private Font _fontUnderlineBold;//=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Underline | FontStyle.Bold);
		private Font _fontHeader;//=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);
		private Font _fontTitle;//=new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold);
		#endregion Fields - Private Drawing

		#region Fields - Private for Properties
		private bool _addButtonEnabled=true;
		private Color _colorSelectedRow=Color.Silver;
		private bool _hasAddButton=false;
		private bool _hasDropDowns=false;
		private bool _headersVisible=true;
		private bool _hScrollVisible=false;
		private Font _fontForSheets;//disposed
		private Font _fontForSheetsBold;//disposed
		///<summary>HasMultiLineHeaders must be turned on for this to work.</summary>
		private bool _hasAutoWrappedHeaders=false;
		private bool _hasMultilineHeaders;
		private float _scaleMy = 1;
		private Point _selectedCell=new Point(-1,-1);
		private ArrayList _selectedIndices=new ArrayList();//todo: time to get away from old arraylist and make this more robust.
		private GridSelectionMode _selectionMode=GridSelectionMode.One;
		private bool _sortingAllowByColumn=false;
		private string _title="";
		private bool _titleVisible=true;
		private bool _vScrollVisible=true;
		#endregion Fields - Private for Properties

		#region Fields - Paging
		///<summary>A function that defines how to create a gridrow row given an object for the row when paging is enabled.</summary>
		public Func<object,GridRow> FuncConstructGridRow;
		///<summary>A function that defines how to filter a grid row given an object for the row when paging is enabled. There could be a subset of pages showing to the user.  Meaning, this func might not run against all DataRows provided to the grid. This is because the grid will only invoke this func against DataRows as needed by the navigation control (until pages are filled).</summary>
		public Func<object,bool> FuncDoesObjPassFilter;
		private GridPagingMode _pagingMode=GridPagingMode.Disabled;
		private int _maxPageRows=0;
		///<summary>Always a valid page number after NavigateToPage(...) has been called, otherwise 0.</summary>
		private int _pageCur=0;
		///<summary>When using paging, this contains the data instead of referencing an external DataTable.</summary>
		private List<DataRow> _listDataRows;
		///<summary>A lazy loaded dictionary of page numbers and a list of ODGridRows that will be displayed on the corresponding page.  Pages get added to this dictionary as the user visits them.</summary>
		private Dictionary<int,List<PagingGridRow>> _dictPages=new Dictionary<int, List<PagingGridRow>>();
		///<summary>True when the dictionary contains all possible pages.  This boolean is used as a short circuit of various methods in order to speed logic up.</summary>
		private bool _hasLoadedAllPages=false;
		#endregion Fields - Paging

		#region Fields - Private
		//<summary>Set at the very beginning of OnPaint.  Uses the column width of each column to set up this array with one element for each column.  Contains the columns Pos for that column.</summary>
		//private int[] ColPos;
		private ComboBoxGrid comboBox=new ComboBoxGrid();
		///<summary>At 96dpi. 19 instead of 14.  Used for textboxes, comboboxes, and buttons, so that they have room.</summary>
		private const int EDITABLE_ROW_HEIGHT=19;
		///<summary>The GridColumn that was clicked on during the MouseDown event.</summary>
		private GridColumn _gridColumnMouseDown;
		///<summary>The GridRow that was clicked on during the MouseDown event.</summary>
		private GridRow _gridRowMouseDown;
		///<summary>Is set when ComputeRows is called, then used . If any columns are editable, hasEditableColumn is true, and all row heights are slightly taller.</summary>
		private bool _hasEditableColumn;
		///<summary>Starts out 18.  If title is not visible, this will be set to 0. 96dpi.</summary>
		private int _heightTitle=18;
		///<summary>Starts out 15, but code can push it higher to fit multiline text. If header is not visible, this will be set to 0.</summary>
		private int _heightHeader=15;
		///<summary>The total height of the actual grid area, including the parts hidden by scroll.</summary>
		private int _heightTotal;
		private bool _isMouseDown;
		private bool _isMouseDownInHeader;
		///<summary>Used for BeginUpdate-EndUpdate.</summary>
		private bool _isUpdating;
		private List<MenuItem> _listMenuItemLinks;
		private MouseButtons _mouseButtonLastPressed;
		private Point _mouseClickLocation;
		private int _mouseDownCol;
		private int _mouseDownRow;
		///<summary>Truncates the note to this many characters. UEs can occur at lengths greater than 32,000</summary>
		public const int TEXT_LENGTH_LIMIT=30000;
		private int _printedRows;
		///<summary>If we are part way through drawing a note when we reach the end of a page, this will contain the remainder of the note still to be printed.  If it is empty string, then we are not in the middle of a note.</summary>
		private string _printNoteRemaining;
		private Point _selectedCellOld;
		///<summary>Helps with drag selections.</summary>
		private ArrayList _selectedIndicesWhenMouseDown;
		///<summary>Tab stop is set at 1/2" in ctor, top aligned, no trimming. Horizontal alignment and wrap are changed as needed.</summary>
		private StringFormat _stringFormat;
		private TextBoxBase textEdit;
		///<summary>The total width of the grid, including the parts hidden by scroll.</summary>
		private int _widthTotal;
		#endregion Fields - Private

		#region Contructors
		///<summary></summary>
		public GridOD() {
			InitializeComponent();// Required for Windows.Forms Class Composition Designer support
			vScroll=new VScrollBar();
			vScroll.Scroll+=new ScrollEventHandler(vScroll_Scroll);
			hScroll=new HScrollBar();
			hScroll.Scroll+=new ScrollEventHandler(hScroll_Scroll);
			this.Controls.Add(vScroll);
			this.Controls.Add(hScroll);
			float[] arrayTabStops={50.0f};
			_stringFormat=new StringFormat();
			_stringFormat.SetTabStops(0.0f,arrayTabStops);
			_stringFormat.LineAlignment=StringAlignment.Near;//top aligned, but we will consider a center someday
			//_stringFormat.Trimming=StringTrimming.None;//This causes PrintPage(...) to get stuck in an infinite loop
			DoubleBuffered=true;
			LayoutScrollsAndFonts();
		}
		#endregion Constructors

		#region ODGrid.Designer
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.Container components = null;

		///<summary>Clean up any resources being used.</summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				_stringFormat?.Dispose();
				_brushTitleBackground?.Dispose();
				_fontCell?.Dispose();
				_fontCellBold?.Dispose();
				_fontUnderline?.Dispose();
				_fontUnderlineBold?.Dispose();
				_fontHeader?.Dispose();
				_fontTitle?.Dispose();
				_fontForSheets?.Dispose();
				_fontForSheetsBold?.Dispose();
				comboBox?.Dispose();
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
		#endregion

		private System.Windows.Forms.VScrollBar vScroll;
		private System.Windows.Forms.HScrollBar hScroll;
		#endregion ODGrid.Designer

		#region Events - Raise
		///<summary></summary>
		protected void OnCellDoubleClick(int col,int row) {
			CellDoubleClick?.Invoke(this, new ODGridClickEventArgs(col,row,MouseButtons.Left));
		}
		[Category("OD")]
		[Description("Occurs when a cell is double clicked.")]
		public event ODGridClickEventHandler CellDoubleClick=null;

		///<summary></summary>
		protected void OnCellSelectionChangeCommitted(int col,int row) {
			CellSelectionCommitted?.Invoke(this, new ODGridClickEventArgs(col,row,MouseButtons.Left));
		}
		[Category("OD")]
		[Description("Occurs when a combo box item is selected.")]
		public event ODGridClickEventHandler CellSelectionCommitted=null;

		///<summary></summary>
		protected void OnCellClick(int col,int row,MouseButtons button) {
			CellClick?.Invoke(this, new ODGridClickEventArgs(col,row,button));
		}
		[Category("OD")]
		[Description("Occurs when a cell is single clicked.")]
		public event ODGridClickEventHandler CellClick=null;

		///<summary></summary>
		protected void OnCellTextChanged() {
			CellTextChanged?.Invoke(this, new EventArgs());
		}
		[Category("OD")]
		[Description("Event used when cells are editable.  The TextChanged event is passed up from the textbox where the editing is taking place.")]
		public event EventHandler CellTextChanged=null;

		///<summary></summary>
		protected void OnCellLeave(int col,int row) {
			CellLeave?.Invoke(this, new ODGridClickEventArgs(col, row, MouseButtons.None));
		}
		[Category("OD")]
		[Description("Event used when cells are editable.  LostFocus event is passed up from the textbox where the editing is taking place.")]
		public event ODGridClickEventHandler CellLeave=null;

		///<summary></summary>
		protected void OnCellEnter(int col,int row) {
			CellEnter?.Invoke(this, new ODGridClickEventArgs(col, row, MouseButtons.None));
		}
		[Category("OD")]
		[Description("Event used when cells are editable.  GotFocus event is passed up from the textbox where the editing is taking place.")]
		public event ODGridClickEventHandler CellEnter=null;

		///<summary></summary>
		protected void OnCellKeyDown(KeyEventArgs e) {
			CellKeyDown?.Invoke(this, new ODGridKeyEventArgs(e));
		}
		[Category("OD")]
		[Description("Event used when cells are editable.  KeyDown event is passed up from the textbox where the editing is taking place.")]
		public event ODGridKeyEventHandler CellKeyDown=null;

		///<summary></summary>
		private void OnSelectionCommitted(int index) {
			_selectedIndices.Add(index);
			SelectionCommitted?.Invoke(this,new EventArgs());
		}
		[Category("OD")]
		[Description("Occurs when rows are selected or unselected by the user for any reason, including mouse and keyboard clicks.  Only works for GridSelectionModes.One for now (enhance later).  Excludes programmatic selection.")]
		public event EventHandler SelectionCommitted=null;

		///<summary></summary>
		private void OnTitleAddClick(){
			TitleAddClick?.Invoke(this,new EventArgs());
		}
		[Category("OD")]
		[Description("If HasAddButton is true, this event will fire when the add button is clicked.")]
		public event EventHandler TitleAddClick=null;

		///<summary></summary>
		private void OnColumnSortClick(){
			ColumnSortClick?.Invoke(this,new EventArgs());
		}
		[Category("OD")]
		[Description("If AllowSortingByColumn is true, this event will fire when a column header is clicked and before the grid has sorted. Used to disable pagination so that the entire grid's data is sorted.")]
		public event EventHandler ColumnSortClick=null;

		///<summary></summary>
		private void OnColumnSorted(){
			ColumnSorted?.Invoke(this,new EventArgs());
		}
		[Category("OD")]
		[Description("If AllowSortingByColumn is true, this event will fire when a column header is clicked and the grid is sorted.  Used to reselect rows after sorting.")]
		public event EventHandler ColumnSorted=null;

		///<summary>This is used to dynamically line up "total" boxes below the grid while scrolling.</summary>
		private void OnHorizScrolled(ScrollEventArgs e){
			HorizScrolled?.Invoke(this,e);
		}
		[Category("OD")]
		[Description("If HScrollVisible is true, this event will fire when the horizontal scroll bar moves by mouse, keyboard, or programmatically.")]
		public event ScrollEventHandler HorizScrolled=null;
	
		///<summary>When paging is enabled, this is fired after the current page has been changed. Informs others of the current page and link data for the previous two and next two pages.</summary>
		///<summary></summary>
		private void OnPageChanged() {
			if(PageChanged==null) {
				return;
			}
			int firstLinkVal=(_pageCur>2 ? _pageCur-2 : -1);//When already viewing first or second page no link should ever show in first position.
			int secondLinkVal=(_pageCur>1 ? _pageCur-1 : -1);//When viewing anything other then first page there should be a be a link to previous page.
			int thirdLinkVal=(IsIndexValid(GetStartIndexForPage(_pageCur+1)) ? _pageCur+1 : -1);
			int fourthLinkVal=(IsIndexValid(GetStartIndexForPage(_pageCur+2)) ? _pageCur+2 : -1);
			PageChanged.Invoke(this,new ODGridPageEventArgs(_pageCur,new List<int>() { firstLinkVal,secondLinkVal,thirdLinkVal,fourthLinkVal }));
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event ODGridPageChangeEventHandler PageChanged=null;
		#endregion Events - Raise

		#region Enums
		///<summary>Dictates how ODGrids should handle paging. The following properties must be implemented when paging is enabled; MaxPageRows, ListPagingRowData, FuncConstructGridRow, and FuncFilterGridRow (optional).</summary>
		public enum GridPagingMode {
			///<summary>Default. Grid will not load pages.</summary>
			Disabled,
			///<summary>Grid will load a specific number of rows per page determined by MaxPageRows.</summary>
			Enabled,
			///<summary>Grid will display pertinent information starting at the bottom of the page instead of the top. Includes several niceities like automatically scrolling to the bottom of the page after navigation has occurred. E.g. See the Progress Notes grid in the Chart module.</summary>
			EnabledBottomUp
		}

		///<summary>Used to locally keep track of paging data and if the grid needs to invoke funcs defined by the calling member.</summary>
		private enum PagingIndexState {
			///<summary>Used when an index is beyond the possible index of items we have.</summary>
			NotValid,
			///<summary>Used when an index is less then filtered list size, indicates row filter and creation was already handled.</summary>
			ValidDataPreviouslyLoaded,
			///<summary>Used when an index is valid but needs to run filter logic and then row creation logic if filter logic return true.</summary>
			ValidDataNotLoaded,
		}
		#endregion Enums

		#region Properties - Public
		///<summary></summary>
		[Category("OD")]
		[Description("Set false to disable row selection when user clicks.  Row selection should then be handled by the form using the cellClick event.")]
		[DefaultValue(true)]
		public bool AllowSelection { get; set; }=true;

		///<summary>The background color that is used for selected rows.</summary>
		[Category("OD")]
		[Description("The background color that is used for selected rows.")]
		[DefaultValue(typeof(Color),"Silver")]
		public Color ColorSelectedRow {
			get => _colorSelectedRow;
			set {
				_colorSelectedRow=value;
				Invalidate();
			}
		}

		///<summary>Set to true to enable context menu to detect PatNum links. This should be set false for grids in modal windows because it can select a different patient, resulting in a potentially unsafe state.</summary>
		[Category("OD")]
		[Description("Set to true to enable context menu to detect PatNum links. This should be set to false for grids in modal windows.")]
		[DefaultValue(false)]
		public bool DoShowPatNumLinks { get; set; }=false;

		///<summary>Set to true to enable context menu to detect TaskNum links. This should be set false for grids in modal windows because it will allow the user to interact with the Task window and select a different patient via the "Go To" button, resulting in a potentially unsafe state.</summary>
		[Category("OD")]
		[Description("Set to true to enable context menu to detect TaskNum links. This should be set to false for grids in modal windows.")]
		[DefaultValue(false)]
		public bool DoShowTaskNumLinks { get; set; }=false;

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

		[Category("OD")]
		[Description("Set true to use RichTextBoxes for editable cells")]
		[DefaultValue(false)]
		public bool EditableUsesRTF { get; set; } =false;

		///<summary></summary>
		[Category("OD")]
		[Description("js Not allowed to be used in OD proper. Set to true to show an add button on the right side of the title bar.")]
		//[NotifyParentProperty(true),RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(false)]
		public bool HasAddButton {
			get {return _hasAddButton;}
			set {
				_hasAddButton=value;
				Invalidate();
			}
		}

		[Category("OD")]
		[Description("Set to true to wrap headers automatically if the length of the header text is longer than the column. HasMultiLineHeaders must be set to true for this to work.")]
		[DefaultValue(false)]
		public bool HasAutoWrappedHeaders {
			get {
				return _hasAutoWrappedHeaders;
			}
			set {
				_hasAutoWrappedHeaders=value;
				if(value) {
					HasMultilineHeaders=true;
				}
			}
		}

		///<summary>Allow rows to drop down other rows. Leave enough space in the row's first cell to display a drop down arrow. Rows that can drop down must have a parent row set.</summary>
		[Category("OD")]
		[Description("Allow rows to drop down other rows. Leave enough space in the row's first cell to display a drop down arrow. Rows that can drop down must have a parent row set.")]
		[DefaultValue(false)]
		public bool HasDropDowns {
			get {
				return _hasDropDowns;
			}
			set {
				_hasDropDowns=value;
				Invalidate();
			}
		}

		[Category("OD")]
		[Description("Set false to disallow links from being automatically added to right click menus.")]
		[DefaultValue(true)]
		public bool HasLinkDetect { get; set; } = true;

		///<summary>Allow Headers to be multiple lines tall.</summary>
		[Category("OD")]
		[Description("Set true to allow new line characters in column headers.")]
		[DefaultValue(false)]
		public bool HasMultilineHeaders {
			get {
				return _hasMultilineHeaders;
			}
			set {
				_hasMultilineHeaders=value;
				if(!_hasMultilineHeaders) {
					_hasAutoWrappedHeaders=false;
				}
				Invalidate();
			}
		}

		///<summary>Set false to hide the column header row below the main title.</summary>
		[Category("OD")]
		[Description("Set false to hide the column header row below the main title.")]
		[DefaultValue(true)]
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
				LayoutScrollsAndFonts();
				Invalidate();
			}
		}

		///<summary>Set true to show a horizontal scrollbar.  Vertical scrollbar always shows, but is disabled if not needed.  If hScroll is not visible, then grid will auto reset width to match width of columns.</summary>
		[Category("OD")]
		[Description("Set true to show a horizontal scrollbar.")]
		[DefaultValue(false)]
		public bool HScrollVisible {
			get {
				return _hScrollVisible;
			}
			set {
				_hScrollVisible=value;
				LayoutScrollsAndFonts();
				Invalidate();
			}
		}

		///<summary>Gets the List of GridColumns assigned to the ODGrid control.</summary>
		[Category("OD")]
		[Description("Gets the List of GridColumns assigned to the ODGrid control.")]
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		//[Editor(typeof(System.ComponentModel.Design.CollectionEditor),typeof(System.Drawing.Design.UITypeEditor))]
		//[Browsable(false)]//buggy.
		public ListGridColumns ListGridColumns { get; } = new ListGridColumns();

		[Category("OD")]
		[Description("The maximum number of rows to show in the grid before going to another page.")]
		[DefaultValue(0)]
		public int MaxPageRows {
			get {
				return _maxPageRows;
			}
			set {
				_maxPageRows=value;
				Invalidate();
			}
		}

		///<summary>The starting column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.</summary>
		[Category("OD")]
		[Description("The starting column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.")]
		[DefaultValue(0)]
		public int NoteSpanStart { get; set; } = 0;

		///<summary>The ending column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.  If this remains 0, then notes will be entirey skipped for this grid.  There is no grid line on the right side of a note.</summary>
		[Category("OD")]
		[Description("The ending column for notes on each row.  Notes are not part of the main row, but are displayed on subsequent lines.  If this remains 0, then notes will be entirely skipped for this grid.  There is no grid line on the right side of a note.")]
		[DefaultValue(0)]
		public int NoteSpanStop { get; set; } = 0;

		///<summary></summary>
		[Category("OD")]
		[Description("Disabled by default. When enabled, grid will attempt to load MaxPageRows per page.")]
		[DefaultValue(GridPagingMode.Disabled)]
		public GridPagingMode PagingMode {
			get {
				return _pagingMode;
			}
			set {
				_pagingMode=value;
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Just like the listBox.SelectionMode, except no MultiSimple, and added OneCell.")]
		[DefaultValue(typeof(GridSelectionMode),"One")]
		public GridSelectionMode SelectionMode {
			get {
				return _selectionMode;
			}
			set {
				//if((GridSelectionMode)value==SelectionMode.MultiSimple){
				//	MessageBox.Show("MultiSimple not supported.");
				//	return;
				//}
				if((GridSelectionMode)value==GridSelectionMode.OneCell) {
					_selectedCell=new Point(-1,-1);//?
					_selectedIndices=new ArrayList();
				}
				_selectionMode=value;
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Set to false to disable the context menu for the grid.")]
		[DefaultValue(true)]
		public bool ShowContextMenu { get; set; } = true;

		///<summary>Set true to allow user to click on column headers to sort rows, alternating between ascending and descending.</summary>
		[Category("OD")]
		[Description("Set true to allow user to click on column headers to sort rows, alternating between ascending and descending.")]
		[DefaultValue(false)]
//todo: change name to SortingAllowByColumn, but it will affect many files
		public bool AllowSortingByColumn {
			get => _sortingAllowByColumn;
			set {
				_sortingAllowByColumn=value;
				if(!_sortingAllowByColumn) {
					SortedByColumnIdx=-1;
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
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Set false to hide the main title. In rare cases, this could also be used to draw your own title bar.")]
		[DefaultValue(true)]
		public bool TitleVisible {
			get {
				return _titleVisible;
			}
			set {
				if(_titleVisible==value){
					return;
				}
				_titleVisible=value;
				if(_titleVisible){
					_heightTitle=18;
				}
				else{
					_heightTitle=0;
				}
				LayoutScrollsAndFonts();
				Invalidate();
			}
		}

		///<summary>Uniquely identifies the grid to translate title to another language.  Name it like 'Table...'  Grid contents must be manually translated.</summary>
		[Category("OD")]
		[Description("Uniquely identifies the grid to translate title to another language.  Name it like 'Table...'  Grid contents must be manually translated.")]
		[DefaultValue("")]
		public string TranslationName { get; set; } = "";

		///<summary></summary>
		[Category("OD")]
		[Description("Normally true to show vertical scroll. False will hide it IF the grid is short enough so that the vertical scroll is not needed.")]
		[DefaultValue(true)]
		public bool VScrollVisible {
			get {
				return _vScrollVisible;
			}
			set {
				_vScrollVisible=value;
				LayoutScrollsAndFonts();
				Invalidate();
			}
		}

		///<summary>Text within each cell will wrap, making some rows taller. Default true.</summary>
		[Category("OD")]
		[Description("Text within each cell will wrap, making some rows taller.")]
		[DefaultValue(true)]
		public bool WrapText { get; set; } = true;
		#endregion Properties - Public

		#region Properties - Not Browsable
		//todo: These all need to be reviewed and simplified

		///<summary>The width of the "+" button.</summary>
		protected int AddButtonWidth {
			get;
			private set;
		}

		///<summary>Used to "gray out" AddButton when functionality should be disabled.  Only affects UI; disabling functionality must be implemented in event handler.</summary>
		[Browsable(false)]
		[DefaultValue(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AddButtonEnabled{
			get{
				return _addButtonEnabled;
			}
			set{
				_addButtonEnabled=value;
				Refresh();
			}
		}

		///<summary>If not set, null. If set, this is used instead of _fontCell and _fontCellBold.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Font FontForSheets{
			set{
				if(_fontForSheets==value){
					return;
				}
				_fontForSheets?.Dispose();
				_fontForSheetsBold?.Dispose();
				_fontForSheets=value;
				_fontForSheetsBold=new Font(_fontForSheets,FontStyle.Bold);
			}
		}

		///<summary>Height of the horizontal scrollbar. Will give the same value whether it's visible or not.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HScrollHeight {
			get {
				return hScroll.Height;
			}
		}

		///<summary>Gets the position of the horizontal scrollbar.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HScrollValue {
			get {
				if(_hScrollVisible && hScroll.Enabled) {
					return hScroll.Value;
				}
				return 0;
			}
		}

		///<summary>Gets the List of GridRows assigned to the ODGrid control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<GridRow> ListGridRows { get; } = new List<GridRow>();

		///<summary>Default 1.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float ScaleMy{
			get {
				return _scaleMy; 
			}
			set{
				_scaleMy = value;
				LayoutScrollsAndFonts();
				BeginUpdate();
				EndUpdate(preserveSelected:true);//Force rows heights to be recalculated without touching indices
			}
		}

		///<summary>Gets the small change value for the vertical scrollbar.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollSmallChange {
			get {
				return vScroll.SmallChange;
			}
		}

		///<summary>Gets or sets the value of the vertical scrollbar.  Does all error checking and invalidates.</summary>
		[Browsable(false)]
		[DefaultValue(0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollValue {
			get {
				return vScroll.Value;
			}
			set {
				if(!vScroll.Enabled) {
					return;
				}
				//Can't do this because UserControlJobEdit:658 etc uses this to reset if > max
				//if(vScroll.Value==value){
				//	return;
				//}
				int newValue;
				if(value>vScroll.Maximum-vScroll.LargeChange){
					newValue=vScroll.Maximum-vScroll.LargeChange;
				}
				else if(value<vScroll.Minimum) {
					newValue=vScroll.Minimum;
				}
				else {
					newValue=value;
				}
				try {
					vScroll.Value=newValue;
				}
				catch(Exception e) {//This should never ever happen.
					//Showing a messagebox is NOT our normal way of handling errors on mouse events, but the user would get a popup for the unhandled exception, anyway.
					MessageBox.Show("Error: Invalid Scroll Value. \r\n"
						+"Scroll value from: "+vScroll.Value+"\r\n"
						+"Scroll value to: "+newValue+"\r\n"
						+"Min scroll value: "+vScroll.Minimum+"\r\n"
						+"Max scroll value: "+vScroll.Maximum+"\r\n"
						+"Large change value: "+vScroll.LargeChange+"\r\n\r\n"
						+e.ToString());
					vScroll.Value=vScroll.Minimum;
				}
				if(textEdit!=null) {
					textEdit.Dispose();
				}
				Invalidate();
			}
		}

		///<summary>Holds the x,y values of the selected cell if in OneCell mode.  -1,-1 represents no cell selected.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point SelectedCell {
			get {
				return _selectedCell;
			}
		}

		///<summary>Returns the list of rows selected instead of a list of indices.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<GridRow> SelectedGridRows {
			get {
				if(SelectionMode==GridSelectionMode.OneCell) {
					return new List<GridRow>() { ListGridRows[_selectedCell.Y] };
				}
				else {
					return _selectedIndices.ToArray().Where(x => (int)x>-1 && (int)x<ListGridRows.Count).Select(x => ListGridRows[(int)x]).ToList();
				}
			}
		}

		///<summary>Holds the int values of the indices of the selected rows.  To set selected indices, use SetSelected().</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int[] SelectedIndices {
			get {
				if(SelectionMode==GridSelectionMode.OneCell) {
					if(_selectedCell.Y==-1){
						return new int[0];
					}
					return new int[] { _selectedCell.Y };
				}
				int[] retVal=_selectedIndices.ToArray().Select(x => (int)x).Where(x => x>-1 && x<ListGridRows.Count).ToArray();
				Array.Sort(retVal);//they must be in numerical order
				return retVal;
			}
		}

		///<summary>Returns current sort column index.  Use SortForced to maintain current grid sorting when refreshing the grid.  Typically -1 to show no triangle.  Or, specify a column to show a triangle.  The actual sorting happens at mouse down.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SortedByColumnIdx { get; private set; } = -1;

		///<summary>Returns current sort order.  Use SortForced to maintain current grid sorting when refreshing the grid.  True to show a triangle pointing up.  False to show a triangle pointing down.  Only works if sortedByColumnIdx is not -1.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SortedIsAscending { get; private set; }

		///<summary>Width of all columns summed.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int WidthAllColumns{
			get{
				int retVal=0;
				for(int i=0;i<ListGridColumns.Count;i++){
					retVal+=ListGridColumns[i].State.Width;
				}
				return retVal;
			}
		}
		#endregion Properties - Not Browsable

		#region Methods - Event Handlers - Painting
		///<summary></summary>
		protected override void OnPaintBackground(PaintEventArgs pea) {
			//base.OnPaintBackground (pea);
			//don't paint background.  This reduces flickering.
		}

		///<summary>Runs any time the control is invalidated.</summary>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			if(_isUpdating) {
				return;
			}
			if(Width<1 || Height<1) {
				return;
			}
			ComputeColumns();//it's only here because I can't figure out how to do it when columns are added. It will be removed.
			Graphics g=e.Graphics;
			//g.SmoothingMode=SmoothingMode.HighQuality;//for the up/down triangles, but it messes up the normal lines, so it's done next to triangles
			g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
			DrawBackG(g);
			DrawRows(g);
			DrawTitle(g);
			DrawHeaders(g);//this will draw on top of any grid stuff
			DrawOutline(g);
			string stringException=null;
			if(TranslationName==null) {//I see many places where grids are created with "", so no error there
				stringException="TranslationName cannot be blank";
			}
			else if(this.ParentForm!=null && PagingMode!=GridPagingMode.Disabled && PageChanged==null) {
				stringException=new ArgumentNullException("PageChangeEventHandler").ToString();
			}
			if(!string.IsNullOrEmpty(stringException)) {
				Font exceptionFont=new Font(FontFamily.GenericSansSerif,16,FontStyle.Bold);
				RectangleF rectangle=new RectangleF(0,OriginY(),Width,Height-OriginY());
				SolidBrush exceptionBrush=new SolidBrush(Color.Red);
				g.DrawString(stringException,exceptionFont,exceptionBrush,rectangle,_stringFormat);	
			}
		}

		///<summary>Draws a solid gray background.</summary>
		private void DrawBackG(Graphics g) {
			g.FillRectangle(_brushBackground,0,OriginY(),Width,Height-OriginY());
		}

		///<summary>Draws the background, lines, and text for all rows that are visible.</summary>
		private void DrawRows(Graphics g) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("IN") && _fontCell.Name!="Arial") {
				//India. Not sure on history of this hack, but it seems harmless. Maybe MS not avail in India?
				_fontCell=new Font("Arial",_fontCell.Size);
				_fontCellBold=new Font("Arial",_fontCell.Size,FontStyle.Bold);
			}
			//Draw each row that should be displayed.
			for(int i=0;i<ListGridRows.Count;i++) {
				if(!ListGridRows[i].State.Visible){
					continue;
				}
				if(-vScroll.Value+ListGridRows[i].State.YPos+ListGridRows[i].State.HeightMain+ListGridRows[i].State.HeightNote < 0) {
					continue;//lower edge of row above top of grid area
				}
				if(-vScroll.Value+1+ScaleI(_heightTitle+_heightHeader)+ListGridRows[i].State.YPos > Height) {
					return;//row below lower edge of control
				}
				DrawRow(g,i);
			}
		}

		///<summary>Draws background, lines, image, and text for a single row.</summary>
		private void DrawRow(Graphics g, int rowI) {
			//The values for Ypos, XPos, etc. refer to actual gridline positions.
			//Cells are sequentially drawn top to bottom, left to right.
			//Cell background is drawn, followed by right and bottom gridlines.
			//So backgrounds must start 1 pix R and D to avoid drawing on top of gridlines to their L and top. 
			//This rule is consistent, including cell 0,0, where the left "gridline" is the outline of the entire grid.
			//For example, if a cell is 40x10, then the fill rect is size 40x10, drawn at 1,1.
			//Then, its own gridlines are drawn at 40 and 10 on right and bottom
			//These draw right on top of the filled rectangle, resulting in perfect alignment.	
			GridRow gridRow=ListGridRows[rowI];
			int top=-vScroll.Value+OriginY()+gridRow.State.YPos;//The gridline above this row. 
			int hMain=gridRow.State.HeightMain;
			int hNote=gridRow.State.HeightNote;
			int hTot=gridRow.State.HeightMain+gridRow.State.HeightNote;
			Font font;//do not dispose this ref.
			//selected row color
			if(_selectedIndices.Contains(rowI)) {
//todo: brushes aren't disposed
				g.FillRectangle(new SolidBrush(GetSelectedColor(gridRow.ColorBackG,ColorSelectedRow)),
					1,
					top+1,
					_widthTotal,
					hTot);
			}
			//colored row background
			else if(gridRow.ColorBackG!=Color.White) {
				g.FillRectangle(new SolidBrush(gridRow.ColorBackG),
					1,
					top+1,
					_widthTotal,
					hTot);
			}
			//normal row color
			else {//need to draw over the gray background
				g.FillRectangle(new SolidBrush(gridRow.ColorBackG),
					1,
					top+1,
					_widthTotal,
					hTot);
			}
			//Color Individual Cells.
			for(int i=0;i<gridRow.Cells.Count;i++) {
				if(i>ListGridColumns.Count) {
					break;
				}
				if(gridRow.Cells[i].ColorBackG==Color.Empty && !gridRow.Cells[i].IsButton) {
					continue;
				}
				//Blend with row background colors. Cell color= Avg(CellColor+BackGColor)
				Color colorCell;
				if(_selectedIndices.Contains(rowI)) {
					colorCell=Color.FromArgb(
						(ColorSelectedRow.R+gridRow.Cells[i].ColorBackG.R)/2,
						(ColorSelectedRow.G+gridRow.Cells[i].ColorBackG.G)/2,
						(ColorSelectedRow.B+gridRow.Cells[i].ColorBackG.B)/2);
				}
				//colored row background
				else if(gridRow.ColorBackG!=Color.White) {
					colorCell=Color.FromArgb(
						(gridRow.ColorBackG.R+gridRow.Cells[i].ColorBackG.R)/2,
						(gridRow.ColorBackG.G+gridRow.Cells[i].ColorBackG.G)/2,
						(gridRow.ColorBackG.B+gridRow.Cells[i].ColorBackG.B)/2);
				}
				//normal row color
				else {
					colorCell=gridRow.Cells[i].ColorBackG;
				}
				if(gridRow.Cells[i].IsButton) { 
					Rectangle rectangleButton=new Rectangle(-hScroll.Value+ListGridColumns[i].State.XPos+2,top+2,ListGridColumns[i].State.Width-4,hTot-4);
					Color colorTop=Color.White;
					if(gridRow.Cells[i].ButtonIsPressed){
						colorTop=Color.LightGray;
					}
					using(LinearGradientBrush brushButton = new LinearGradientBrush(rectangleButton,colorTop,Color.LightGray,90)) {
						g.FillRectangle(brushButton,rectangleButton); 
					}
					Color colorOutline=_colorTextDisabled;
					if(Enabled){
						colorOutline=_colorTitleBottom;
					}
					using(Pen pen=new Pen(colorOutline)) { 
						g.DrawRectangle(pen,rectangleButton);
					}
				}
				else {
					using(SolidBrush brushCell=new SolidBrush(colorCell)) {
						g.FillRectangle(brushCell,-hScroll.Value+ListGridColumns[i].State.XPos+1,top+1,ListGridColumns[i].State.Width,hTot);
					}
				}
			}
			if(_selectionMode==GridSelectionMode.OneCell && _selectedCell.X!=-1 && _selectedCell.Y!=-1
				&& _selectedCell.Y==rowI) {
				g.FillRectangle(new SolidBrush(ColorSelectedRow),
					-hScroll.Value+ListGridColumns[_selectedCell.X].State.XPos+1,
					top+1,
					ListGridColumns[_selectedCell.X].State.Width,
					hTot);
			}
			//lines for note section
			if(hNote>0) {
				//left vertical gridline for note
				if(NoteSpanStart!=0) {
					g.DrawLine(_penGridline,
						-hScroll.Value+ListGridColumns[NoteSpanStart].State.XPos,
						top+hMain,
						-hScroll.Value+ListGridColumns[NoteSpanStart].State.XPos,
						top+hTot);
				}
				//vertical line on very right of entire grid
				g.DrawLine(_penGridline,
					-hScroll.Value+_widthTotal,
					top+hMain+1,
					-hScroll.Value+_widthTotal,
					top+hTot);
				//Horizontal line which divides the main part of the row from the notes section of the row
				g.DrawLine(_penGridline,
					-hScroll.Value+ListGridColumns[0].State.XPos,
					top+hMain,
					-hScroll.Value+ListGridColumns[ListGridColumns.Count-1].State.Right,
					top+hMain);
			}
			Pen penLower=new Pen(_penGridline.Color);//disposed a few pages down
			if(rowI==ListGridRows.Count-1) {//last row
				penLower=new Pen(_penColumnSeparator.Color);
			}
			else {
				if(gridRow.ColorLborder!=Color.Empty) {
					penLower=new Pen(gridRow.ColorLborder);
				}
			}
			if(WrapText){
				_stringFormat.FormatFlags=0;
			}
			else{
				_stringFormat.FormatFlags=StringFormatFlags.NoWrap;
			}
			for(int i=0;i<ListGridColumns.Count;i++) {
				//vertical line on right of cell
				g.DrawLine(_penGridline,
					-hScroll.Value+ListGridColumns[i].State.Right,
					top+1,
					-hScroll.Value+ListGridColumns[i].State.Right,
					top+hMain);
				//lower horizontal gridline
				g.DrawLine(penLower,
					-hScroll.Value+ListGridColumns[i].State.XPos+1,
					top+hTot,
					-hScroll.Value+ListGridColumns[i].State.Right,
					top+hTot);
				//text
				if(gridRow.Cells.Count-1< i) {
					continue;
				}
				switch(ListGridColumns[i].TextAlign) {
					case HorizontalAlignment.Left:
						_stringFormat.Alignment = StringAlignment.Near;
						break;
					case HorizontalAlignment.Center:
						_stringFormat.Alignment = StringAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						_stringFormat.Alignment = StringAlignment.Far;
						break;
				}
				Rectangle rectangleText=new Rectangle(-hScroll.Value+ListGridColumns[i].State.XPos,top+1,ListGridColumns[i].State.Width,hMain);
				if(WrapText && ListGridColumns[i].TextAlign==HorizontalAlignment.Left){
					//The line below is a compromise that leaves a bit of extra white space on the right, 
					//but still works for sheets and printing.  See comments in ComputeRows.
					rectangleText=new Rectangle(-hScroll.Value+ListGridColumns[i].State.XPos,top+1,ListGridColumns[i].State.Width,hMain);
				}
				if(gridRow.Cells[i].IsButton){
					//handles both wrap and no wrap
					rectangleText=new Rectangle(-hScroll.Value+ListGridColumns[i].State.XPos+2,top+2,ListGridColumns[i].State.Width-1,hMain-2);
				}
				if(_hasDropDowns && i==0) {//only draw the dropdown arrow in the first column of the row.
					if(gridRow.State.DropDownState!=ODGridDropDownState.None || gridRow.DropDownParent!=null) {
						rectangleText=new Rectangle(-hScroll.Value+ListGridColumns[i].State.XPos+10,top+1,ListGridColumns[i].State.Width-10,hMain);
						//might be easier to just not support wrap in a dropdown cell. Oh well.
						if(WrapText && ListGridColumns[i].TextAlign==HorizontalAlignment.Left){
							rectangleText=new Rectangle(-hScroll.Value+ListGridColumns[i].State.XPos+10,top+1,ListGridColumns[i].State.Width-3,hMain);
						}
					}
				}
				Color colorText;
				if(gridRow.Cells[i].ColorText== Color.Empty) {
					if(gridRow.Cells[i].IsButton && !Enabled) { //set the "button" text to gray if this is grid is disabled
						colorText=_colorTextDisabled;
					}
					else { 
						colorText=gridRow.ColorText;
					}
				}
				else {
					colorText=gridRow.Cells[i].ColorText;
				}
				if(gridRow.Cells[i].Bold== YN.Yes) {
					font= _fontCellBold;
				}
				else if(gridRow.Cells[i].Bold== YN.No) {
					font= _fontCell;
				}
				else {//unknown.  Use row bold
					if(gridRow.Bold) {
						font= _fontCellBold;
					}
					else {
						font= _fontCell;
					}
				}
				if(gridRow.Cells[i].Underline== YN.Yes) {//Underline the current cell.  If it is already bold, make the cell bold and underlined.
					if(font == _fontCellBold){
						font= _fontUnderlineBold;
					}
					else{
						font= _fontUnderline;
					}
				}
				//If _hasDropDowns, we don't support images in the first column
				if(_hasDropDowns && i==0) { //only draw the dropdown arrow in the first column of the row.
					if(gridRow.State.DropDownState== ODGridDropDownState.Up) {//arrow pointing right
						PointF topPoint = new PointF(-hScroll.Value+ListGridColumns[i].State.XPos+3,top+(hMain/2)-5);
						PointF botPoint = new PointF(topPoint.X, topPoint.Y+10);
						PointF rightPoint = new PointF(topPoint.X+5, topPoint.Y+5);
						g.DrawPolygon(Pens.Black, new PointF[] { topPoint, botPoint, rightPoint });
					}
					else if(gridRow.State.DropDownState== ODGridDropDownState.Down) {//arrow pointing down
						PointF leftPoint = new PointF(-hScroll.Value+ListGridColumns[i].State.XPos+1,top+(hMain/2)-2);
						PointF rightPoint = new PointF(leftPoint.X+10, leftPoint.Y);
						PointF botPoint = new PointF(leftPoint.X+5, leftPoint.Y+5);
						g.DrawPolygon(Pens.Black, new PointF[] { leftPoint, botPoint, rightPoint });
					}
					//a small L indicator that shows that a row is a drop down child.
					else if(gridRow.DropDownParent!=null) {
						PointF topPoint = new PointF(-hScroll.Value+ListGridColumns[i].State.XPos+4,top+1);
						PointF midPoint = new PointF(-hScroll.Value+ListGridColumns[i].State.XPos+4,top+8);
						PointF rightPoint = new PointF(-hScroll.Value+ListGridColumns[i].State.XPos+9,top+8);
						g.DrawLines(Pens.Black, new PointF[] { topPoint, midPoint, rightPoint });
					}
				}
				if(ListGridColumns[i].ImageList==null){//text
					using(SolidBrush brush=new SolidBrush(colorText)){
						g.DrawString(gridRow.Cells[i].Text, font, brush, rectangleText, _stringFormat);
					}
				}
				else {//image
					int imageIndex=-1;
					if(gridRow.Cells[i].Text!="") {
						imageIndex= PIn.Int(gridRow.Cells[i].Text);
					}
					if(imageIndex != -1) {
						Image img =ListGridColumns[i].ImageList.Images[imageIndex];
						g.DrawImage(img,-hScroll.Value+ListGridColumns[i].State.XPos,top,
							ScaleI(img.Width),ScaleI(img.Height));
					}
				}
			}
			penLower.Dispose();
			//note text
			if(hNote>0 && NoteSpanStop>0 && NoteSpanStart<ListGridColumns.Count) {
				int noteW=0;
				for(int i=NoteSpanStart;i<=NoteSpanStop;i++) {
					noteW+=ListGridColumns[i].State.Width;
				}
				if(gridRow.Bold) {
					font= _fontCellBold; 
				}
				else {
					font= _fontCell;
				}
				Rectangle rectangleNote=new Rectangle(
					-hScroll.Value+ListGridColumns[NoteSpanStart].State.XPos,
					top+hMain+1,
					ListGridColumns[NoteSpanStop].State.Right-ListGridColumns[NoteSpanStart].State.XPos,
					hNote);
				_stringFormat.Alignment = StringAlignment.Near;
				using(SolidBrush brush=new SolidBrush(gridRow.ColorText)){
					if(gridRow.Note.Length<= TEXT_LENGTH_LIMIT) {
						g.DrawString(gridRow.Note,font,brush,rectangleNote,_stringFormat);
					}
					else {
						g.DrawString(gridRow.Note.Substring(0, TEXT_LENGTH_LIMIT)+"\r\n"+Lans.g(this,"...more..."),font,brush,rectangleNote,_stringFormat);
					}
				}
			}
		}

		private void DrawTitle(Graphics g) {
			if(_heightTitle==0) {
				return;
			}
			g.FillRectangle(_brushTitleBackground,1,1,Width,ScaleI(_heightTitle));
			if(!DesignMode){
				g.DrawString(_title,_fontTitle,Brushes.White,Width/2-g.MeasureString(_title,_fontTitle).Width/2,2);
			}
			if(!HasAddButton) {
				return;
			}
			//Everything from here down is AddButton
			int addW=ScaleI(_heightTitle);
			int dividerX=Width-addW-3;
			const int dividerLineWidth=1;
			const int plusSignWidth=4;
			Brush brushPlusSign=Brushes.White;//new SolidBrush(ODColorTheme.GridTextBrush.Color);//cannot dispose a brush from ODColorTheme
			if(!_addButtonEnabled) {
				//"gray out" darkest background color for plus sign
				const double fadeFactor=0.8;
				brushPlusSign=new LinearGradientBrush(new Rectangle(0,0,Width,ScaleI(_heightTitle)),
					Color.FromArgb((int)(_colorTitleTop.R*fadeFactor),(int)(_colorTitleTop.G*fadeFactor),(int)(_colorTitleTop.B*fadeFactor)),
					Color.FromArgb((int)(_colorTitleBottom.R*fadeFactor),(int)(_colorTitleBottom.G*fadeFactor),(int)(_colorTitleBottom.B*fadeFactor)),
					LinearGradientMode.Vertical);//"gray out" AddButton
			}
			using(Pen pDark=new Pen(Color.FromArgb(102,102,122))) {
				g.DrawLine(Pens.LightGray,new Point(dividerX,0),new Point(dividerX,ScaleI(_heightTitle)));//divider line(right side)
				g.DrawLine(pDark,new Point(dividerX-dividerLineWidth,0),new Point(dividerX-dividerLineWidth,ScaleI(_heightTitle)));//divider line(left side)
				g.FillRectangle(brushPlusSign,//vertical bar in "+" sign
					Width-addW/2-plusSignWidth,2,
					plusSignWidth,addW-plusSignWidth);
				//Width-addW/2+2,addW-2);
				g.FillRectangle(brushPlusSign,//horizontal bar in "+" sign
					Width-addW,(addW-plusSignWidth)/2,
					addW-plusSignWidth,plusSignWidth);
				//Width-2,addW/2+2);
				//g.DrawString("+",titleFont,brushTitleText,Width-addW+4,2);
			}
			AddButtonWidth=addW;
		}

		private void DrawHeaders(Graphics g) {
			if(_heightHeader==0) {
				return;
			}
			g.FillRectangle(_brushHeaderBackground,1,ScaleI(_heightTitle)+1,Width,ScaleI(_heightHeader));//background
			g.DrawLine(_penGridInnerLine,0,ScaleI(_heightTitle),Width,ScaleI(_heightTitle));//line between title and headers
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;
			stringFormat.LineAlignment=StringAlignment.Center;
			for(int i=0;i<ListGridColumns.Count;i++) {
				if(i!=0) {
					//vertical lines to right of column header
					g.DrawLine(_penColumnSeparator,-hScroll.Value+ListGridColumns[i].State.XPos,ScaleI(_heightTitle)+3,
						-hScroll.Value+ListGridColumns[i].State.XPos,OriginY()-2);
					g.DrawLine(new Pen(Color.White),-hScroll.Value+ListGridColumns[i].State.XPos+1,ScaleI(_heightTitle)+3,
						-hScroll.Value+ListGridColumns[i].State.XPos+1,OriginY()-2);
				}
				RectangleF rect=new RectangleF(ListGridColumns[i].State.XPos-hScroll.Value,ScaleI(_heightTitle)+1,
					ListGridColumns[i].State.Width,ScaleI(_heightHeader));
				g.DrawString(ListGridColumns[i].Heading,_fontHeader,_brushHeaderText,rect,stringFormat);
			}
			GraphicsState graphicsState=g.Save();
			g.SmoothingMode=SmoothingMode.HighQuality;
			for(int i=0;i<ListGridColumns.Count;i++) {
				if(SortedByColumnIdx == i) {
					PointF p=new PointF(-hScroll.Value+ListGridColumns[i].State.XPos+6,ScaleF(_heightTitle+_heightHeader/2f));
					if(SortedIsAscending) {//pointing up
						g.FillPolygon(Brushes.White,new PointF[] {
						new PointF(p.X-4.9f,p.Y+2f),//LLstub
						new PointF(p.X-4.9f,p.Y+2.5f),//LLbase
						new PointF(p.X+4.9f,p.Y+2.5f),//LRbase
						new PointF(p.X+4.9f,p.Y+2f),//LRstub
						new PointF(p.X,p.Y-2.8f)});//Top
						g.FillPolygon(Brushes.Black,new PointF[] {
						new PointF(p.X-4,p.Y+2),//LL
						new PointF(p.X+4,p.Y+2),//LR
						new PointF(p.X,p.Y-2)});//Top
					}
					else {//pointing down
						g.FillPolygon(Brushes.White,new PointF[] {//shaped like home plate
						new PointF(p.X-4.9f,p.Y-2f),//ULstub
						new PointF(p.X-4.9f,p.Y-2.7f),//ULtop
						new PointF(p.X+4.9f,p.Y-2.7f),//URtop
						new PointF(p.X+4.9f,p.Y-2f),//URstub
						new PointF(p.X,p.Y+2.8f)});//Bottom
						g.FillPolygon(Brushes.Black,new PointF[] {
						new PointF(p.X-4,p.Y-2),//UL
						new PointF(p.X+4,p.Y-2),//UR
						new PointF(p.X,p.Y+2)});//Bottom
					}
				}//if
			}//for
			g.Restore(graphicsState);//to possibly turn off HighQuality
			stringFormat.Dispose();
			//line below headers
			g.DrawLine(_penColumnSeparator,0,OriginY(),Width,OriginY());
		}

		///<summary>Draws outline around entire control.</summary>
		private void DrawOutline(Graphics g) {
			if(hScroll.Visible) {//for the little square at the lower right between the two scrollbars
				g.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.Control)),Width-vScroll.Width-1,
					Height-hScroll.Height-1,vScroll.Width,hScroll.Height);
			}
			g.DrawRectangle(_penOutline,0,0,Width-1,Height-1);
		}
		#endregion Methods - Event Handlers - Painting

		#region Methods - Event Handlers
		protected override void OnLayout(LayoutEventArgs e){
			base.OnLayout(e);
		}

		///<summary></summary>
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			//OnSizeChanged(new EventArgs());//this just causes some sort of infinite loop that crashes VS.
			//In fact, putting other layout code here also tends to crash VS, which I can find no cause for.
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(_isUpdating){
				return;
			}
			float scrollFraction=(float)vScroll.Value/vScroll.Maximum;
			LayoutScrollsAndFonts();
			if(this.IsHandleCreated) {//otherwise, EndUpdate creates Graphics object, causing premature OnLoad. This causes Parent.KeyDown to get skipped, so arrow keys don't work.
				BeginUpdate();
				EndUpdate(preserveSelected:true);//Force rows heights to be recalculated without touching indices
			}
			ScrollValue=(int)(scrollFraction*vScroll.Maximum);
		}

		///<summary></summary>
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(this.Parent!=null) {//parent control
				this.Parent.KeyDown+=new KeyEventHandler(Parent_KeyDown);
			}
			if(this.ParentForm!=null) {
				if(PagingMode!=GridPagingMode.Disabled) {
					SetPagingData(_listDataRows);
				}
			}
		}

		///<summary>The purpose of this is to allow dragging to select multiple rows.  Only makes sense if selectionMode==MultiExtended.  Doesn't matter whether ctrl is down, because that only affects the mouse down event.</summary>
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!_isMouseDown) {
				return;
			}
			if(_selectionMode!=GridSelectionMode.MultiExtended) {
				return;
			}
			if(!AllowSelection) {
				return;//dragging does not change selection of rows
			}
			if(_isMouseDownInHeader) {
				return;//started drag in header, so not allowed to select anything.
			}
			int curRow=PointToRow(e.Y);
			if(curRow==-1 || curRow==_mouseDownRow) {
				return;
			}
			//because mouse might have moved faster than computer could keep up, we have to loop through all rows between
			//todo: the comment on this method is inconsistent with the code below
			if(ControlIsDown()) {
				if(_selectedIndicesWhenMouseDown==null) {
					_selectedIndices=new ArrayList();
				}
				else {
					_selectedIndices=new ArrayList(_selectedIndicesWhenMouseDown);
				}
			}
			else {
				_selectedIndices=new ArrayList();
			}
			if(_mouseDownRow<curRow) {//dragging down
				for(int i=_mouseDownRow;i<=curRow;i++) {
					if(i==-1) {
						continue;
					}
					if(_selectedIndices.Contains(i)) {
						_selectedIndices.Remove(i);
					}
					else {
						_selectedIndices.Add(i);
					}
				}
			}
			else {//dragging up
				for(int i=curRow;i<=_mouseDownRow;i++) {
					if(_selectedIndices.Contains(i)) {
						_selectedIndices.Remove(i);
					}
					else {
						_selectedIndices.Add(i);
					}
				}
			}
			Invalidate();
		}

		///<summary></summary>
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			//_mouseDownRow could be .5s stale and ListGridRow might have changed. 
			int row=PointToRow(((MouseEventArgs)e).Y);//test based on current ListGridRows
			if(row==-1) {
				return;//double click was in the title or header section
			}
			if(_mouseDownCol==-1) {
				return;//click was to the right of the columns
			}
			_mouseDownRow=row;
			OnCellDoubleClick(_mouseDownCol,_mouseDownRow);
		}

		///<summary></summary>
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			//todo: consider mouse down.
			if(HasAddButton){ 
				if(((MouseEventArgs)e).X>=Width-ScaleI(_heightTitle)-5 && ((MouseEventArgs)e).Y<=ScaleI(_heightTitle)){
					OnTitleAddClick();
				}
			}
			if(_hasDropDowns
				&& _mouseDownRow>-1 && _mouseDownCol==0 && ListGridRows[_mouseDownRow].State.DropDownState!=ODGridDropDownState.None
				/*&& ((MouseEventArgs)(e)).X < 10*///uncomment if the user should have to click the DropDown Triangle to for the row to drop down.
				&& !ControlIsDown() //if the user is trying to select the row, don't drop it down.
				&& !ShiftIsDown())
			{
				DropDownRowClick(_mouseDownRow,((MouseEventArgs)(e)).Y);
				return;
			}
			GridRow rowClicked;
			if(_mouseDownRow.Between(0,ListGridRows.Count-1)) {
				rowClicked=ListGridRows[_mouseDownRow];
			}
			else {
				rowClicked=null;
			}
			GridColumn colClicked;
			if(_mouseDownCol.Between(0, ListGridColumns.Count-1)) {
				colClicked=ListGridColumns[_mouseDownCol];
			}
			else {
				colClicked=null;
			}
			if(_mouseDownRow>-1 && _mouseDownCol>-1 //on a row and not to the side of the columns.
				//Make sure that the row/column selected on mouse down is still the same as it is now. This may not be the case if columns
				//was changed after mouse down but before mouse up.
				&& rowClicked==_gridRowMouseDown && colClicked==_gridColumnMouseDown)
			{
				OnCellClick(_mouseDownCol,_mouseDownRow,_mouseButtonLastPressed);
			}
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified){
			base.ScaleControl(factor, specified);
			//we must leave grid scaling in place.  Otherwise, grids throughout the program won't resize when forms resize due to dpi changes.
		}
		#endregion Methods - Event Handlers

		#region Methods - Event Handlers - Mouse
		///<summary>Several location throughout the program the context menu changes. This subscribes each menu to use the popup helper below.</summary>
		protected override void OnContextMenuChanged(EventArgs e) {
			base.OnContextMenuChanged(e);
			if(this.ContextMenu==null) {
				this.ContextMenu=new ContextMenu();
			}
			this.ContextMenu.Popup+=CopyHelper;
			if(HasLinkDetect) {//Link detect should go after Copy Helper as the "Copy Text" menu item should be above any links.
				this.ContextMenu.Popup+=PopupHelper;
			}
		}

		///<summary>Just prior to displaying the context menu, add wiki links if neccesary.</summary>
		private void PopupHelper(object sender, EventArgs e) {
			//If multiple grids add the same instance of ConextMenu then all of them will raise this event any time any of them raise the event.
			//Only allow the event to operate if this is the grid that actually fired the event.
			try {
				if(((ContextMenu)sender).SourceControl.Name != this.Name) {
					return;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return;
			}
			removeContextMenuLinks();
			int rowClick = PointToRow(_mouseClickLocation.Y);
			int colClick = PointToCol(_mouseClickLocation.X);
			if(rowClick<0 || colClick<0) {//don't diplay links, not on grid row.
				return;
			}
			if(_mouseButtonLastPressed==MouseButtons.Right && rowClick<=this.ListGridRows.Count) {
				GridRow row = this.ListGridRows[rowClick];
				if(this.ContextMenu==null) {
					this.ContextMenu=new ContextMenu();
					return;
				}
				_listMenuItemLinks=new List<MenuItem>();
				if(this.ContextMenu.MenuItems.Count>0) {
					_listMenuItemLinks.Add(new MenuItem("-"));
				}
				StringBuilder sb = new StringBuilder();
				row.Cells.OfType<GridCell>().ToList().ForEach(x => sb.AppendLine(x.Text));
				sb.AppendLine(row.Note);
				_listMenuItemLinks.AddRange(OpenDentBusiness.UI.PopupHelper.GetContextMenuItemLinks(sb.ToString(),DoShowPatNumLinks,DoShowTaskNumLinks));
				if(_listMenuItemLinks.Any(x=>x.Text!="-")) {//at least one REAL menu item that is not the divider.
					_listMenuItemLinks.ForEach(x => this.ContextMenu.MenuItems.Add(x));
				}
			}
		}

		///<summary>Removes wiki and web links from context menu.</summary>
		private void removeContextMenuLinks() {
			if(this.ContextMenu==null || _listMenuItemLinks==null) {
				return;
			}
			foreach(MenuItem mi in _listMenuItemLinks) {
				this.ContextMenu.MenuItems.Remove(mi);
			}
		}

		///<summary>Just prior to displaying the context menu, add wiki links if neccesary.</summary>
		protected virtual void CopyHelper(object sender,EventArgs e) {
			//If multiple grids add the same instance of ContextMenu, then all of them will raise this event any time any of them raise the event.
			//Only allow the event to operate if this is the grid that actually fired the event.
			try {
				if(((ContextMenu)sender).SourceControl.Name != this.Name) {
					return;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return;
			}
			if(this.ContextMenu==null) {
				return;
			}
			//Todo: this is a bad selector:
			MenuItem menuItemCopy = this.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "Copy Cell Text");
			List<MenuItem> listMenuItems = new List<MenuItem>();
			if(menuItemCopy==null) {
				menuItemCopy = new MenuItem("Copy Cell Text",OnCopyCellClick);
				if(this.ContextMenu.MenuItems.Count > 0) {
					listMenuItems.Add(new MenuItem("-"));
				}
				listMenuItems.Add(menuItemCopy);
				listMenuItems.ForEach(x => this.ContextMenu.MenuItems.Add(x));
			}
			if(!(0<=_mouseDownRow && _mouseDownRow<=ListGridRows.Count-1)
				|| !(0<=_mouseDownCol && _mouseDownCol<=ListGridRows[_mouseDownRow].Cells.Count-1)
				|| string.IsNullOrEmpty(ListGridRows[_mouseDownRow].Cells[_mouseDownCol].Text)) 
			{
				menuItemCopy.Enabled = false;
			}
			else {
				menuItemCopy.Enabled = true;
			}
		}

		private void OnCopyCellClick(object sender,EventArgs e) {
			try {
				string copyText = ListGridRows[_mouseDownRow].Cells[_mouseDownCol].Text;
				ODClipboard.SetClipboard(copyText);
			}
			catch {
				//show a message box?
			}
		}

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			_mouseButtonLastPressed=e.Button;//used in the click event.
			_mouseClickLocation=e.Location;//stored for later use during context menu display
			_mouseDownRow=PointToRow(e.Y);
			_mouseDownCol=PointToCol(e.X);
			if(_mouseDownRow.Between(0,ListGridRows.Count-1)) {
				_gridRowMouseDown=ListGridRows[_mouseDownRow];
			}
			else {
				_gridRowMouseDown=null;
			}
			if(_mouseDownCol.Between(0, ListGridColumns.Count-1)) {
				_gridColumnMouseDown = ListGridColumns[_mouseDownCol];
			}
			else {
				_gridColumnMouseDown = null;
			}
			if(e.Button==MouseButtons.Right) {
				if(_selectedIndices.Contains(_mouseDownRow)) {//If a currently selected row is clicked, then ignore right click.
					return;
				}
				//otherwise, row will be selected. Useful when using context menu.
			}
			_isMouseDown=true;
			if(e.Y < 1+ScaleI(_heightTitle)) {//mouse down was in the title section
				return;
			}
			if(e.Y < 1+OriginY()) {//mouse down was on a column header
				_isMouseDownInHeader=true;
				if(_mouseDownCol!=-1 && ListGridColumns[_mouseDownCol].CustomClickEvent!=null) {
					ListGridColumns[_mouseDownCol].CustomClickEvent(null,null);
					return;
				}
				else if(AllowSortingByColumn) {
					if(_mouseDownCol==-1) {
						return;
					}
					SortByColumn(_mouseDownCol);
					Invalidate();
					return;
				}
				else {
					return;
				}
			}
			if(_mouseDownRow==-1) {//mouse down was below the grid rows
				return;
			}
			if(_mouseDownCol==-1) {//mouse down was to the right of columns
				return;
			}
			if(!AllowSelection) {
				return;//clicks do not trigger selection of rows, but cell click event still gets fired
			}
			switch(_selectionMode) {
				case GridSelectionMode.None:
					return;
				case GridSelectionMode.One:
					_selectedIndices.Clear();
					OnSelectionCommitted(_mouseDownRow);
					if(_mouseDownRow>ListGridRows.Count-1){
						return;
					}
					if(_mouseDownCol>ListGridRows[_mouseDownRow].Cells.Count-1){
						return;//this can happen if programmer forgot to add some cells to a row.
					}
					GridCell cell=ListGridRows[_mouseDownRow].Cells[_mouseDownCol];
					if(cell.IsButton) {
						cell.ButtonIsPressed=true;
						Refresh(); //Force the "button" styling to repaint in the "clicked" style
						cell.ClickEvent.Invoke(this,new EventArgs());
						cell.ButtonIsPressed=false;
					}
					break;
				case GridSelectionMode.OneCell:
					//The current grid could have another control floating on top of it (edit box, combo box, etc) which may require a LostFocus event to fire.
					//Setting the ActiveControl to null will cause these controls to lose focus which will trigger a CellLeave event.
					//If there is not a floating control, then the ActiveControl will get reset by the OnClick event after the MouseDown event finishes.
					this.ActiveControl=null;
					_selectedIndices.Clear();
					if(textEdit!=null) {
						textEdit.Dispose();//a lot happens right here, including a FillGrid() which sets selectedCell to -1,-1
					}
					comboBox.Visible=false;
					_selectedCell=new Point(_mouseDownCol,_mouseDownRow);
					if(ListGridColumns[_selectedCell.X].IsEditable || ListGridColumns[_selectedCell.X].ListDisplayStrings!=null) {
						if(ListGridColumns[_selectedCell.X].IsEditable) {
							CreateEditBox();
						}
						else if(ListGridColumns[_selectedCell.X].ListDisplayStrings!=null) {
							CreateComboBox();
						}
						//When the additional control is created, added to the controls, and given focus, the chain of events stops and the OnClick event never gets fired.
						//Manually fire the OnClick event because we can guarantee that the user did in fact click on a cell at this point in the mouse down event.
						OnClick(e);
					}
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
					}
					break;
				case GridSelectionMode.MultiExtended:
					if(ControlIsDown()) {
						//we need to remember exactly which rows were selected the moment the mouse down started.
						//Then, if the mouse gets dragged up or down, the rows between mouse start and mouse end
						//will be set to the opposite of these remembered values.
						_selectedIndicesWhenMouseDown=new ArrayList(_selectedIndices);
						if(_selectedIndices.Contains(_mouseDownRow)) {
							_selectedIndices.Remove(_mouseDownRow);
						}
						else {
							_selectedIndices.Add(_mouseDownRow);
						}
					}
					else if(ShiftIsDown()) {
						if(_selectedIndices.Count==0) {
							_selectedIndices.Add(_mouseDownRow);
						}
						else {
							int fromRow=(int)_selectedIndices[0];
							_selectedIndices.Clear();
							if(_mouseDownRow<fromRow) {//dragging down
								for(int i=_mouseDownRow;i<=fromRow;i++) {
									_selectedIndices.Add(i);
								}
							}
							else {
								for(int i=fromRow;i<=_mouseDownRow;i++) {
									_selectedIndices.Add(i);
								}
							}
						}
					}
					else {//ctrl or shift not down
						_selectedIndices.Clear();
						_selectedIndices.Add(_mouseDownRow);
					}
					break;
			}
			Invalidate();
		}

		///<summary></summary>
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			//if(e.Button==MouseButtons.Right){
			//	return;
			//}		
			if(this.ContextMenu==null && this.ContextMenuStrip==null && this.ShowContextMenu) {
				if(e.Button==MouseButtons.Right) {
					this.ContextMenu=new ContextMenu();
					try{
						this.ContextMenu.Show(this,_mouseClickLocation);//triggers autofill via the popup helper.
					}
					catch{ }//This can fail if user double clicks, causing form to close, and also right clicks on the second click.
				}
			}
			_isMouseDown=false;
			_isMouseDownInHeader=false;
		}
		
		void dropDownBox_GotFocus(object sender,EventArgs e) {
			OnCellEnter(_selectedCell.X,_selectedCell.Y);
		}

		void dropDownBox_LostFocus(object sender,EventArgs e) {
			OnCellLeave(_selectedCellOld.X,_selectedCellOld.Y);
			comboBox.Visible=false;
		}

		void dropDownBox_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Tab) {
				//supports moving to the right with tab, but not down
				//Used primarily in ortho chart
				for(int i=_selectedCellOld.X+1;i<ListGridColumns.Count;i++) {
					if(ListGridColumns[i].IsEditable){//textbox
						_selectedCell=new Point(i,_selectedCellOld.Y);
						Focus();
						CreateEditBox();
						return;
					}
					if(ListGridColumns[i].ListDisplayStrings!=null) {//dropdown
						_selectedCell=new Point(i,_selectedCellOld.Y);
						Focus();
						CreateComboBox();
						return;
					}
				}
			}
		}

		void dropDownBox_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!comboBox.SelectedIndex.Between(0,comboBox.Items.Count-1)) {
				//Combobox loaded with no selection, user opens combobox, does not highlight any option with mouse, and types "Enter".
				return;//Since no selection has been made, there hasn't actually been a SelectionChangeCommitted.
			}
			ListGridRows[SelectedCell.Y].Cells[_selectedCell.X].Text=comboBox.Items[comboBox.SelectedIndex].ToString();
			ListGridRows[SelectedCell.Y].Cells[_selectedCell.X].ComboSelectedIndex=comboBox.SelectedIndex;
			OnCellSelectionChangeCommitted(SelectedCell.X,_selectedCell.Y);
		}

		///<summary>When selection mode is OneCell, and user clicks in a column that isEditable, then this edit box will appear.</summary>
		private void CreateEditBox() {
			int hScrollBarHeight=0;
			if(HScrollVisible) {
				hScrollBarHeight=SystemInformation.HorizontalScrollBarHeight;
			}
			//Check if new edit box location is below the display screen
			int editBoxLocationTop=-vScroll.Value+1+OriginY()+ListGridRows[_selectedCell.Y].State.YPos+ListGridRows[_selectedCell.Y].State.HeightMain+hScrollBarHeight;
			if(editBoxLocationTop > this.DisplayRectangle.Bottom) {
				int onScreenPixels=vScroll.Value+DisplayRectangle.Height-OriginY()-(ListGridRows[_selectedCell.Y].State.YPos)-hScrollBarHeight;
				int offScreenPixels=ListGridRows[_selectedCell.Y].State.HeightMain-onScreenPixels;
				if(offScreenPixels>0) {
					ScrollValue+=offScreenPixels;//Scrolling down
				}
			}
			else if(-vScroll.Value+1+OriginY()+ListGridRows[_selectedCell.Y].State.YPos<this.DisplayRectangle.Top+OriginY()) {
				//If new edit box location is above the display screen
				ScrollToIndex(_selectedCell.Y);//Scrolling up
			}
			if(EditableUsesRTF) {
				RichTextBox editRichBox=new RichTextBox();
				editRichBox.Multiline=true;
				editRichBox.BorderStyle=BorderStyle.FixedSingle;
				editRichBox.ScrollBars=RichTextBoxScrollBars.None;
				
				if(ListGridColumns[_selectedCell.X].TextAlign==HorizontalAlignment.Right) {
					editRichBox.SelectionAlignment=HorizontalAlignment.Right;
				}

				//Rich text boxes have strange borders (3D looking) and so we have to manipulate the size and location differently.
				editRichBox.Size=new Size(ListGridColumns[_selectedCell.X].State.Width-1,ListGridRows[_selectedCell.Y].State.HeightMain-1);
				editRichBox.Location=new Point(-hScroll.Value+ListGridColumns[_selectedCell.X].State.XPos+1,
					-vScroll.Value+OriginY()+ListGridRows[_selectedCell.Y].State.YPos+1);
				textEdit=editRichBox;
			}
			else {
				TextBox editTextBox=new TextBox();
				editTextBox.Multiline=true;
				if(ListGridColumns[_selectedCell.X].TextAlign==HorizontalAlignment.Right) {
					editTextBox.TextAlign=HorizontalAlignment.Right;
				}
				editTextBox.Size=new Size(ListGridColumns[_selectedCell.X].State.Width+1,ListGridRows[_selectedCell.Y].State.HeightMain+1);
				editTextBox.Location=new Point(-hScroll.Value+ListGridColumns[_selectedCell.X].State.XPos,
					-vScroll.Value+OriginY()+ListGridRows[_selectedCell.Y].State.YPos);
				textEdit=editTextBox;
			}
			//If the cell's color is set manually, that color will also show up for this EditBox.
			textEdit.BackColor=ListGridRows[_selectedCell.Y].ColorBackG;
			textEdit.Font=_fontCell;
			//As far as I can tell, MS RichTextBox seems to not fully support dpi scaling. 
			//Specifically, if this textbox content is right aligned, the scale is off at high dpi.
			//Not a huge deal, as this is a rare situation.  Did not see this reported anywhere on web.
			//There's probably a workaround by setting rtf or something.
			textEdit.Text=ListGridRows[_selectedCell.Y].Cells[_selectedCell.X].Text;
			textEdit.TextChanged+=new EventHandler(textEdit_TextChanged);
			textEdit.GotFocus+=new EventHandler(textEdit_GotFocus);
			textEdit.LostFocus+=new EventHandler(textEdit_LostFocus);
			textEdit.KeyDown+=new KeyEventHandler(textEdit_KeyDown);
			textEdit.KeyUp+=new KeyEventHandler(textEdit_KeyUp);
			textEdit.AcceptsTab=true;
			this.Controls.Add(textEdit);
			if(EditableUsesRTF) {
				//RichTextBox always allows return
				if(!EditableAcceptsCR) {
					textEdit.SelectAll();//Only select all when not multiline (editableAcceptsCR) i.e. proc list for editing fees selects all for easy overwriting.
				}
			}
			else {
				if(EditableAcceptsCR) {//Allow the edit box to handle carriage returns/multiline text.
					((TextBox)textEdit).AcceptsReturn=true;
				}
				else {
					textEdit.SelectAll();//Only select all when not multiline (editableAcceptsCR) i.e. proc list for editing fees selects all for easy overwriting.
				}
			}
			//Set the cell of the current editBox so that the value of that cell is saved when it loses focus (used for mouse click).
			_selectedCellOld=new Point(_selectedCell.X,_selectedCell.Y);
			textEdit.Focus();
		}

		void textEdit_LostFocus(object sender,EventArgs e) {
			//editBox_Leave wouldn't catch all scenarios
			OnCellLeave(_selectedCellOld.X,_selectedCellOld.Y);
			if(textEdit!=null && (!textEdit.Disposing || !textEdit.IsDisposed)) {
				textEdit.Dispose();
				textEdit=null;
				Graphics g=this.CreateGraphics();
				ComputeRows(g);
				g.Dispose();
				LayoutScrollsAndFonts();
				Invalidate();
			}
		}

		void textEdit_GotFocus(object sender,EventArgs e) {
			OnCellEnter(_selectedCellOld.X,_selectedCellOld.Y);
		}

		void textEdit_KeyDown(object sender,KeyEventArgs e) {
			OnCellKeyDown(e);
			if(e.Handled) {
				return;
			}
			if(e.Shift && e.KeyCode == Keys.Enter) {
				ListGridRows[_selectedCell.Y].Cells[_selectedCell.X].Text+="\r\n";
				return;
			}
			if(e.KeyCode==Keys.Enter) {//usually move to the next cell
				if(EditableAcceptsCR) {//When multiline it inserts a carriage return instead of moving to the next cell.
					return;
				}
				if(EditableEnterMovesDown){
					textEdit.Dispose();
					textEdit=null;
					if(_selectedCellOld.Y==ListGridRows.Count-1) {
						return;//can't move down
					}
					_selectedCell=new Point(_selectedCellOld.X,_selectedCellOld.Y+1);
					CreateEditBox();
					return;
				}
				editBox_NextCellRight();
			}
			if(e.KeyCode==Keys.Down) {
				if(EditableAcceptsCR) {//When multiline it moves down inside the text instead of down to the next cell.
					return;
				}
				if(_selectedCellOld.Y<ListGridRows.Count-1) {
					textEdit.Dispose();
					textEdit=null;
					_selectedCell=new Point(_selectedCellOld.X,_selectedCellOld.Y+1);
					CreateEditBox();
				}
			}
			if(e.KeyCode==Keys.Up) {
				if(EditableAcceptsCR) {//When multiline it moves up inside the text instead of up to the next cell.
					return;
				}
				if(_selectedCellOld.Y>0) {
					textEdit.Dispose();
					textEdit=null;
					_selectedCell=new Point(_selectedCellOld.X,_selectedCellOld.Y-1);
					CreateEditBox();
				}
			}
			if(e.KeyCode==Keys.Tab) {
				editBox_NextCellRight();
			}
		}
		
		private void editBox_NextCellRight() {
			textEdit?.Dispose();//This fires editBox_LostFocus, which is where we call OnCellLeave.
			textEdit=null;
			//find the next editable cell to the right.
			for(int i= _selectedCellOld.X+1; i < ListGridColumns.Count; i++) {
				if(ListGridColumns[i].IsEditable){//textbox
					_selectedCell=new Point(i,_selectedCellOld.Y);
					CreateEditBox();
					return;
				}
				if(ListGridColumns[i].ListDisplayStrings!=null) {//dropdown
					_selectedCell=new Point(i,_selectedCellOld.Y);
					CreateComboBox();
					return;
				}
			}
			//can't move to the right, so attempt to move down.
			if(_selectedCellOld.Y==ListGridRows.Count-1) {
				return;//can't move down
			}
			int nextCellToRight=-1;
			for(int i=0; i < ListGridColumns.Count; i++) {
				if(ListGridColumns[i].IsEditable) {
					nextCellToRight= i;
					break;
				}
			}
			//guaranteed to have a value. Either the cell below, or possibly below and left.
			_selectedCell=new Point(nextCellToRight,_selectedCellOld.Y+1);
			CreateEditBox();
		}
		
		void textEdit_KeyUp(object sender,KeyEventArgs e) {
			if(textEdit==null) {
				return;
			}
			if(textEdit.Text=="") {
				return;
			}
			Graphics g=CreateGraphics();
			int heightText=(int)(1.03f*g.MeasureString(textEdit.Text+"\r\n",_fontCell,textEdit.Width).Height);
			g.Dispose();
			if(heightText < ScaleI(EDITABLE_ROW_HEIGHT)) {//if it's less than one line
			  heightText=ScaleI(EDITABLE_ROW_HEIGHT);//set it to one line
			}
			if(heightText<=textEdit.Height+1) {
				return;
			}
			//it needs to grow
			int bottomVisible=Height-1;
			if(hScroll.Visible){
				bottomVisible-=hScroll.Height;
			}
			if(textEdit.Top==0 && textEdit.Bottom==bottomVisible){
				return;//already as tall as it can get
			}
			textEdit.Height+=ScaleI(EDITABLE_ROW_HEIGHT);
			if(textEdit.Bottom<=bottomVisible){
				return;
			}
			textEdit.Top=bottomVisible-textEdit.Height;//this can hide upper portion of textbox
			if(textEdit.Top<0){
				textEdit.Top=0;
				textEdit.Height=bottomVisible;
				//if(textBoxEdit is TextBox textB){
				//	textB.ScrollBars=ScrollBars.Vertical;//this crashes
				//}
			}
		}

		void textEdit_TextChanged(object sender,EventArgs e) {
			if(textEdit!=null) {
				ListGridRows[_selectedCell.Y].Cells[_selectedCell.X].Text=textEdit.Text;
			}
			OnCellTextChanged();
		}

		///<summary></summary>
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			ScrollValue-=e.Delta/3;
		}

		#endregion Methods - Event Handlers - Mouse

		#region Methods - Event Handlers - Key

		/// <summary>If the Ctrl key down is not being captured by the grid because it doesn't have focus, then this automatically handles it.  The only thing you have to do to make it work is to turn on KeyPreview for the parent form. (This comment might be outdated. It doesn't seem to have anything to do with Ctrl)</summary>
		private void Parent_KeyDown(Object sender,KeyEventArgs e) {
			if(_selectionMode==GridSelectionMode.One) {
				if(e.KeyCode==Keys.Down) {
					if(_selectedIndices.Count>0 && (int)_selectedIndices[0] < ListGridRows.Count-1) {
						int prevRow=(int)_selectedIndices[0];
						_selectedIndices.Clear();
						OnSelectionCommitted(prevRow+1);
						hScroll.Value=hScroll.Minimum;
					}
				}
				else if(e.KeyCode==Keys.Up) {
					if(_selectedIndices.Count>0 && (int)_selectedIndices[0] > 0) {
						int prevRow=(int)_selectedIndices[0];
						_selectedIndices.Clear();
						OnSelectionCommitted(prevRow-1);
					}
				}
			}
		}

		#endregion Methods - Event Handlers - Key

		#region Methods - Sheets
		///<summary>Called from external Sheet   Presumably, the isPrinting distinguishes between printing sheets and displaying sheets.  Its only effect is to draw a slightly smaller font.</summary>
		public void SheetDrawRow(int rowI,Graphics g,int x=0,int y=0,bool isBottom=false,bool isSheetGrid=false,bool isPrintingSheet=false) {
			Font fontLocal=_fontCell;//no need to dispose. Just a ref.
			//Font fontLocalBold=_fontCellBold96;
			if(_fontForSheets!=null) {
				fontLocal=_fontForSheets;
				//fontLocalBold=new Font(_fontForSheets,FontStyle.Regular);
			}
			RectangleF textRect;
			SolidBrush textBrush;
			//selected row color
			if(_selectedIndices.Contains(rowI)) {
				g.FillRectangle(new SolidBrush(ColorSelectedRow),
					x+1,
					y-vScroll.Value+1,
					_widthTotal,
					ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1);
			}
			//colored row background
			else if(ListGridRows[rowI].ColorBackG!=Color.White) {
				g.FillRectangle(new SolidBrush(ListGridRows[rowI].ColorBackG),
					x+1,
					y-vScroll.Value+1,
					_widthTotal,
					ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1);
			}
			//normal row color
			else {//need to draw over the gray background
				g.FillRectangle(new SolidBrush(ListGridRows[rowI].ColorBackG),
					x+1,
					y-vScroll.Value+1,
					_widthTotal,//this is a really simple width value that always works well
					ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1);
			}
			if(_selectionMode==GridSelectionMode.OneCell && _selectedCell.X!=-1 && _selectedCell.Y!=-1
			&& _selectedCell.Y==rowI) {
				g.FillRectangle(new SolidBrush(ColorSelectedRow),
					x-hScroll.Value+1+ListGridColumns[_selectedCell.X].State.XPos,
					y-vScroll.Value+1,
					ListGridColumns[_selectedCell.X].State.Width,
					ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1);
			}
			//lines for note section
			if(ListGridRows[rowI].State.HeightNote>0) {
				//left vertical gridline
				if(NoteSpanStart!=0) {
					g.DrawLine(_penGridline,
						x-hScroll.Value+1+ListGridColumns[NoteSpanStart].State.XPos,
						y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain,
						x-hScroll.Value+1+ListGridColumns[NoteSpanStart].State.XPos,
						y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote);
				}
				//Horizontal line which divides the main part of the row from the notes section of the row
				g.DrawLine(_penGridline,
					x-hScroll.Value+1+ListGridColumns[0].State.XPos+1,
					y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain,
					x-hScroll.Value+1+ListGridColumns[ListGridColumns.Count-1].State.Right,
					y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain);

			}
			Pen penLower=new Pen(_penGridline.Color);//disposed a few pages down
			if(rowI==ListGridRows.Count-1) {//last row
				penLower=new Pen(_penColumnSeparator.Color);
			}
			else {
				if(ListGridRows[rowI].ColorLborder!=Color.Empty) {
					penLower=new Pen(ListGridRows[rowI].ColorLborder);
				}
			}
			for(int i=0;i<ListGridColumns.Count;i++) {
				//right vertical gridline
				if(rowI == 0) {
					g.DrawLine(_penGridline,
						x- hScroll.Value+ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width,
						y- vScroll.Value+1,
						x- hScroll.Value+ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width,
						y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain);
				}
				else {
					g.DrawLine(_penGridline,
						x- hScroll.Value + ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width,
						y- vScroll.Value+1,
						x- hScroll.Value + ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width,
						y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain);
				}
				//lower horizontal gridline
				if(i == 0) {
					g.DrawLine(penLower,
						x- hScroll.Value + ListGridColumns[i].State.XPos,
						y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote,
						x- hScroll.Value + ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width,
						y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote);
				}
				else {
					g.DrawLine(penLower,
						x- hScroll.Value + ListGridColumns[i].State.XPos +1,
						y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote,
						x- hScroll.Value + ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width,
						y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote);
				}
				//text
				if(ListGridRows[rowI].Cells.Count-1< i) {
					continue;
				}
				switch(ListGridColumns[i].TextAlign) {
					case HorizontalAlignment.Left:
						_stringFormat.Alignment = StringAlignment.Near;
						break;
					case HorizontalAlignment.Center:
						_stringFormat.Alignment = StringAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						_stringFormat.Alignment = StringAlignment.Far;
						break;
				}
				int vertical= y - vScroll.Value+1+1;
				int horizontal= x - hScroll.Value+1+ ListGridColumns[i].State.XPos;
				int cellW= ListGridColumns[i].State.Width;
				int cellH= ListGridRows[rowI].State.HeightMain;
				if(_hasEditableColumn) {//These cells are taller
					vertical+=2;//so this is to push text down to center it in the cell
					cellH-=3;//to keep it from spilling into the next cell
				}
				if(ListGridColumns[i].TextAlign== HorizontalAlignment.Right) {
					if(_hasEditableColumn) {
						horizontal-=4;
						cellW+=2;
					}
					else {
						horizontal-=2;
						cellW+=2;
					}
				}
				textRect=new RectangleF(horizontal, vertical, cellW, cellH);
				if(ListGridRows[rowI].Cells[i].ColorText== Color.Empty) {
					textBrush=new SolidBrush(ListGridRows[rowI].ColorText);
				}
				else {
					textBrush=new SolidBrush(ListGridRows[rowI].Cells[i].ColorText);
				}
				if(ListGridRows[rowI].Cells[i].Bold== YN.Yes) {
					fontLocal=new Font(fontLocal, FontStyle.Bold);
				}
				else if(ListGridRows[rowI].Cells[i].Bold== YN.No) {
					fontLocal=new Font(fontLocal, FontStyle.Regular);
				}
				else {//unknown.  Use row bold
					if(ListGridRows[rowI].Bold) {
						fontLocal=new Font(fontLocal, FontStyle.Bold);
					}
					else {
						fontLocal=new Font(fontLocal, FontStyle.Regular);
					}
				}
				if(ListGridRows[rowI].Cells[i].Underline== YN.Yes) {//Underline the current cell.  If it is already bold, make the cell bold and underlined.
					fontLocal=new Font(fontLocal, (fontLocal.Bold)?(FontStyle.Bold | FontStyle.Underline): FontStyle.Underline);
				}
				if(ListGridColumns[i].ImageList==null) {
					if(isPrintingSheet) {
						//Using a slightly smaller font because g.DrawString draws text slightly larger when using the printer's graphics
						Font smallerFont = new Font(fontLocal.FontFamily,(float)(fontLocal.Size*0.96), fontLocal.Style);
						g.DrawString(ListGridRows[rowI].Cells[i].Text, smallerFont, textBrush, textRect, _stringFormat);
						smallerFont.Dispose();
					}
					else {//Viewing the grid normally
						g.DrawString(ListGridRows[rowI].Cells[i].Text, fontLocal, textBrush, textRect, _stringFormat);
					}
				}
				else {
					int imageIndex=-1;
					if(ListGridRows[rowI].Cells[i].Text!="") {
						imageIndex= PIn.Int(ListGridRows[rowI].Cells[i].Text);
					}
					if(imageIndex != -1) {
						Image img =ListGridColumns[i].ImageList.Images[imageIndex];
						g.DrawImage(img, horizontal, vertical - 1);
					}
				}
			}
			penLower.Dispose();
			//note text
			if(ListGridRows[rowI].State.HeightNote>0 && NoteSpanStop > 0 && NoteSpanStart < ListGridColumns.Count) {
				int noteW=0;
				for(int i= NoteSpanStart; i <= NoteSpanStop; i++) {
					noteW+= ListGridColumns[i].State.Width;
				}
				if(ListGridRows[rowI].Bold) {
					fontLocal=new Font(fontLocal, FontStyle.Bold);
				}
				else {
					fontLocal=new Font(fontLocal, FontStyle.Regular);
				}
				textBrush=new SolidBrush(ListGridRows[rowI].ColorText);
				textRect=new RectangleF(
					x- hScroll.Value+1+ ListGridColumns[NoteSpanStart].State.XPos +1,
					y- vScroll.Value+1+ ListGridRows[rowI].State.HeightMain+1,
					ListGridColumns[NoteSpanStop].State.Right-ListGridColumns[NoteSpanStart].State.XPos,
					ListGridRows[rowI].State.HeightNote);
				_stringFormat.Alignment = StringAlignment.Near;
				g.DrawString(ListGridRows[rowI].Note, fontLocal, textBrush, textRect, _stringFormat);
			}
			//Left right and bottom lines of grid.  This creates the outline of the entire grid when not using outline control
			//Outline the Title
			Pen pen=_penOutline;//does not need to be disposed.
			if(isSheetGrid) {
				pen=Pens.Black;//System pen, does not need to be disposed.
			}
			//Draw line from LL to UL to UR to LR. top three sides of a rectangle.
			g.DrawLine(pen,x,y,x,y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1);//left side line
			g.DrawLine(pen,x+Width,y,x+Width,y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1);//right side line
			if(isBottom) {
				g.DrawLine(pen,x,y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1,x+Width,y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1);//bottom line.
			}
			fontLocal.Dispose();
		}

		///<summary></summary>
		public void SheetDrawRowX(int rowI,XGraphics g,int x=0,int y=0,bool isBottom=false,bool isSheetGrid=false) {
			XFont fontNormal=new XFont(_fontCell.Name,_fontCell.Size);//There is no Dispose for xfonts, or we would.
			XFont fontBold=new XFont(_fontCellBold.Name,_fontCellBold.Size,XFontStyle.Bold);
			if(_fontForSheets!=null) {
				fontNormal=new XFont(_fontForSheets.Name,_fontForSheets.Size);
				fontBold=new XFont(_fontForSheetsBold.Name,_fontForSheetsBold.Size,XFontStyle.Bold);;
			}
			XFont font;
			XRect textRect;
			XStringAlignment _xAlign=XStringAlignment.Near;
			XSolidBrush textBrush;
			//selected row color
			if(_selectedIndices.Contains(rowI)) {
				g.DrawRectangle(new XSolidBrush(ColorSelectedRow),
					ToPoints(x+1),
					ToPoints(y-vScroll.Value+1),
					ToPoints(_widthTotal),
					ToPoints(ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1));
			}
			//colored row background
			else if(ListGridRows[rowI].ColorBackG!=Color.White) {
				g.DrawRectangle(new XSolidBrush(ListGridRows[rowI].ColorBackG),
					ToPoints(x+1),
					ToPoints(y-vScroll.Value+1),
					ToPoints(_widthTotal),
					ToPoints(ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1));
			}
			//normal row color
			else {//need to draw over the gray background
				g.DrawRectangle(new XSolidBrush(ListGridRows[rowI].ColorBackG),
					ToPoints(x+1),
					ToPoints(y-vScroll.Value+1),
					ToPoints(_widthTotal),
					ToPoints(ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1));
			}
			if(_selectionMode==GridSelectionMode.OneCell && _selectedCell.X!=-1 && _selectedCell.Y!=-1
			&& _selectedCell.Y==rowI) {
				g.DrawRectangle(new XSolidBrush(ColorSelectedRow),
					ToPoints(x-hScroll.Value+1+ListGridColumns[_selectedCell.X].State.XPos),
					ToPoints(y-vScroll.Value+1),
					ToPoints(ListGridColumns[_selectedCell.X].State.Width),
					ToPoints(ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote-1));
			}
			XPen gridPen=new XPen(_penGridline);
			//lines for note section
			if(ListGridRows[rowI].State.HeightNote>0) {
				//left vertical gridline
				if(NoteSpanStart!=0) {
					g.DrawLine(gridPen,
						ToPoints(x-hScroll.Value+1+ListGridColumns[NoteSpanStart].State.XPos),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain),
						ToPoints(x-hScroll.Value+1+ListGridColumns[NoteSpanStart].State.XPos),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote));
				}
				//Horizontal line which divides the main part of the row from the notes section of the row
				g.DrawLine(gridPen,
					ToPoints(x-hScroll.Value+1+ListGridColumns[0].State.XPos+1),
					ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain),
					ToPoints(x-hScroll.Value+1+ListGridColumns[ListGridColumns.Count-1].State.Right),
					ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain));
			}
			XPen xPenLower=new XPen(_penGridline);
			if(rowI==ListGridRows.Count-1) {//last row
				xPenLower=new XPen(XColor.FromArgb(_penColumnSeparator.Color.ToArgb()));
			}
			else {
				if(ListGridRows[rowI].ColorLborder!=Color.Empty) {
					xPenLower=new XPen(ListGridRows[rowI].ColorLborder);
				}
			}
			for(int i=0;i<ListGridColumns.Count;i++) {
				//right vertical gridline
				if(rowI == 0) {
					g.DrawLine(gridPen,
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.Right),
						ToPoints(y-vScroll.Value+1),
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.Right),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain));
				}
				else {
					g.DrawLine(gridPen,
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.Right),
						ToPoints(y-vScroll.Value+1),
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.Right),
						ToPoints(y-vScroll.Value+1+ ListGridRows[rowI].State.HeightMain));
				}
				//lower horizontal gridline
				if(i == 0) {
					g.DrawLine(xPenLower,
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.XPos),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote),
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.Right),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote));
				}
				else {
					g.DrawLine(xPenLower,
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.XPos +1),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote),
						ToPoints(x-hScroll.Value+ListGridColumns[i].State.Right),
						ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+ ListGridRows[rowI].State.HeightNote));
				}
				//text
				if(ListGridRows[rowI].Cells.Count-1< i) {
					continue;
				}
				float adjH=0;
				switch(ListGridColumns[i].TextAlign) {
					case HorizontalAlignment.Left:
						_xAlign= XStringAlignment.Near;
						adjH=1;
						break;
					case HorizontalAlignment.Center:
						_xAlign= XStringAlignment.Center;
						adjH= ListGridColumns[i].State.Width/2f;
						break;
					case HorizontalAlignment.Right:
						_xAlign= XStringAlignment.Far;
						adjH= ListGridColumns[i].State.Width-2;
						break;
				}
				int vertical=y-vScroll.Value-2;
				int horizontal=x-hScroll.Value+1+ListGridColumns[i].State.XPos;
				int cellW=ListGridColumns[i].State.Width;
				int cellH=ListGridRows[rowI].State.HeightMain;
				if(_hasEditableColumn) {//These cells are taller
					vertical+=2;//so this is to push text down to center it in the cell
					cellH-=3;//to keep it from spilling into the next cell
				}
				if(ListGridColumns[i].TextAlign==HorizontalAlignment.Right){
					if(_hasEditableColumn) {
						horizontal-=4;
						cellW+=2;
					}
					else {
						horizontal-=2;
						cellW+=2;
					}
				}
				textRect=new XRect(p(horizontal + adjH),ToPoints(vertical),ToPoints(cellW),ToPoints(cellH));
				if(ListGridRows[rowI].Cells[i].ColorText==Color.Empty) {
					textBrush=new XSolidBrush(ListGridRows[rowI].ColorText);
				}
				else {
					textBrush=new XSolidBrush(ListGridRows[rowI].Cells[i].ColorText);
				}
				if(ListGridRows[rowI].Cells[i].Bold== YN.Yes) {
					font= fontBold;
				}
				else if(ListGridRows[rowI].Cells[i].Bold== YN.No) {
					font= fontNormal;
				}
				else {//unknown.  Use row bold
					if(ListGridRows[rowI].Bold) {
						font= fontBold;
					}
					else {
						font= fontNormal;
					}
				}
				//do not underline row if we are printing to PDF
				//if(rows[rowI].Cells[i].Underline==YN.Yes) {//Underline the current cell.  If it is already bold, make the cell bold and underlined.
				//	cellFont=new XFont(cellFont,(cellFont.Bold)?(XFontStyle.Bold | XFontStyle.Underline):XFontStyle.Underline);
				//}
				if(ListGridColumns[i].ImageList==null) {
					DrawStringX(g, ListGridRows[rowI].Cells[i].Text, font, textBrush, textRect, _xAlign);
				}
				else {
					int imageIndex=-1;
					if(ListGridRows[rowI].Cells[i].Text!="") {
						imageIndex= PIn.Int(ListGridRows[rowI].Cells[i].Text);
					}
					if(imageIndex != -1) {
						XImage img =ListGridColumns[i].ImageList.Images[imageIndex];
						g.DrawImage(img, horizontal, vertical - 1);
					}
				}
			}
			//note text
			if(ListGridRows[rowI].State.HeightNote>0 && NoteSpanStop>0 && NoteSpanStart < ListGridColumns.Count) {
				int noteW=0;
				for(int i=NoteSpanStart;i<=NoteSpanStop;i++) {
					noteW+=ListGridColumns[i].State.Width;
				}
				if(ListGridRows[rowI].Bold) {
					font=fontBold; 
				}
				else {
					font=fontNormal;
				}
				textBrush=new XSolidBrush(ListGridRows[rowI].ColorText);
				textRect=new XRect(
					ToPoints(x-hScroll.Value+1+ListGridColumns[NoteSpanStart].State.XPos+1),
					ToPoints(y-vScroll.Value+1+ListGridRows[rowI].State.HeightMain+1),
					ToPoints(ListGridColumns[NoteSpanStop].State.Right-ListGridColumns[NoteSpanStart].State.XPos),
					ToPoints(ListGridRows[rowI].State.HeightNote));
				_xAlign=XStringAlignment.Near;
				DrawStringX(g,ListGridRows[rowI].Note,font,textBrush,textRect,_xAlign);
			}
			//Left right and bottom lines of grid.  This creates the outline of the entire grid when not using outline control
			//Outline the Title
			XPen pen=new XPen(_penOutline.Color);
			if(isSheetGrid) {
				pen=new XPen(Color.Black);
			}
			//Draw line from LL to UL to UR to LR. top three sides of a rectangle.
			g.DrawLine(pen,ToPoints(x),ToPoints(y),ToPoints(x),ToPoints(y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1));//left side line
			g.DrawLine(pen,ToPoints(x+Width),ToPoints(y),ToPoints(x+Width),ToPoints(y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1));//right side line
			if(isBottom) {
				g.DrawLine(pen,ToPoints(x),ToPoints(y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote)+1,ToPoints(x+Width),ToPoints(y+ListGridRows[rowI].State.HeightMain+ListGridRows[rowI].State.HeightNote+1));//bottom line.
			}
		}

		public void SheetDrawTitle(Graphics g,int x,int y) {
			Color cTitleTop=_colorTitleTop;
			Color cTitleBottom=_colorTitleBottom;
			LinearGradientBrush brushTitleBackground=new LinearGradientBrush(new Rectangle(x,y,Width,_heightTitle),cTitleTop,cTitleBottom,LinearGradientMode.Vertical);
			g.FillRectangle(brushTitleBackground,x,y,Width,_heightTitle);
			g.DrawString(_title,_fontTitle,_brushTitleText,x+(Width/2-g.MeasureString(_title,_fontTitle).Width/2),y+2);
			//Outline the Title
			//Draw line from LL to UL to UR to LR. top three sides of a rectangle.
			g.DrawLines(_penOutline,new Point[] { 
				new Point(x,y+_heightTitle),
				new Point(x,y),
				new Point(x+Width,y),
				new Point(x+Width,y+_heightTitle) });
			//g.DrawRectangle(pen,0,0,Width-1,Height-1);
			if(brushTitleBackground!=null) {
				brushTitleBackground.Dispose();
				brushTitleBackground=null;
			}
		}

		public void SheetDrawTitleX(XGraphics g,int x,int y) {
			Color cTitleTop=_colorTitleTop;
			Color cTitleBottom=_colorTitleBottom;
			Color cTitleText=_brushTitleText.Color;
			LinearGradientBrush brushTitleBackground=new LinearGradientBrush(new Rectangle(x,y,Width,_heightTitle),cTitleTop,cTitleBottom,LinearGradientMode.Vertical);
			XSolidBrush brushTitleText=new XSolidBrush(cTitleText);
			g.DrawRectangle(brushTitleBackground,ToPoints(x),ToPoints(y),ToPoints(Width),ToPoints(_heightTitle));
			XFont xTitleFont=new XFont(_fontTitle.FontFamily.ToString(),_fontTitle.Size,XFontStyle.Bold);
			//g.DrawString(title,titleFont,brushTitleText,p((float)x+(float)(Width/2-g.MeasureString(title,titleFont).Width/2)),p(y+2));
			DrawStringX(g,_title,xTitleFont,brushTitleText,new XRect((float)x+(float)(Width/2),y,100,100),XStringAlignment.Center);
			//Outline the Title
			//Draw line from LL to UL to UR to LR. top three sides of a rectangle.
			g.DrawLines(_penOutline,new Point[] { 
				new Point(x,y+_heightTitle),
				new Point(x,y),
				new Point(x+Width,y),
				new Point(x+Width,y+_heightTitle) });
			//g.DrawRectangle(pen,0,0,Width-1,Height-1);
			if(brushTitleBackground!=null) {
				brushTitleBackground.Dispose();
				brushTitleBackground=null;
			}
		}

		public void SheetDrawHeader(Graphics g,int x,int y) {
			Color cOutline=cOutline=Color.Black;
			Color cTitleTop=Color.White;
			Color cTitleBottom=Color.FromArgb(213,213,223);
			Color cTitleText=Color.Black;
			Color cTitleBackG=Color.LightGray;
			g.FillRectangle(new SolidBrush(cTitleBackG),x,y,Width,_heightHeader);//background
			g.DrawLine(new Pen(Color.FromArgb(102,102,122)),x,y,x+Width,y);//line between title and headers
			for(int i=0;i<ListGridColumns.Count;i++) {
				if(i!=0) {
					//vertical lines separating column headers
					g.DrawLine(new Pen(cOutline), x + (-hScroll.Value + ListGridColumns[i].State.XPos), y,
						x+(-hScroll.Value + ListGridColumns[i].State.XPos), y + _heightHeader);
				}
				g.DrawString(ListGridColumns[i].Heading, _fontHeader, Brushes.Black,
					(float)x + (-hScroll.Value + ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width /2- g.MeasureString(ListGridColumns[i].Heading, _fontHeader).Width/2),
					(float)y + 1);
				if(SortedByColumnIdx == i) {
					PointF p = new PointF(x + (-hScroll.Value+1+ ListGridColumns[i].State.XPos +6), y + (float)_heightHeader / 2f);
					if(SortedIsAscending) { //pointing up
						g.FillPolygon(Brushes.White,new PointF[] {
							new PointF(p.X-4.9f,p.Y+2f), //LLstub
							new PointF(p.X-4.9f,p.Y+2.5f), //LLbase
							new PointF(p.X+4.9f,p.Y+2.5f), //LRbase
							new PointF(p.X+4.9f,p.Y+2f), //LRstub
							new PointF(p.X,p.Y-2.8f)
						}); //Top
						g.FillPolygon(Brushes.Black,new PointF[] {
							new PointF(p.X-4,p.Y+2), //LL
							new PointF(p.X+4,p.Y+2), //LR
							new PointF(p.X,p.Y-2)
						}); //Top
					}
					else { //pointing down
						g.FillPolygon(Brushes.White,new PointF[] { //shaped like home plate
							new PointF(p.X-4.9f,p.Y-2f), //ULstub
							new PointF(p.X-4.9f,p.Y-2.7f), //ULtop
							new PointF(p.X+4.9f,p.Y-2.7f), //URtop
							new PointF(p.X+4.9f,p.Y-2f), //URstub
							new PointF(p.X,p.Y+2.8f)
						}); //Bottom
						g.FillPolygon(Brushes.Black,new PointF[] {
							new PointF(p.X-4,p.Y-2), //UL
							new PointF(p.X+4,p.Y-2), //UR
							new PointF(p.X,p.Y+2)
						}); //Bottom
					}
				}
			} //end for columns.Count
			//Outline the Title
			using(Pen pen=new Pen(cOutline)) {
				g.DrawRectangle(pen,x,y,Width,_heightHeader);
			}
			g.DrawLine(new Pen(cOutline),x,y+_heightHeader,x+Width,y+_heightHeader);
		}

		public void SheetDrawHeaderX(XGraphics g,int x,int y) {
			Color cOutline=cOutline=Color.Black;
			Color cTitleTop=Color.White;
			Color cTitleBottom=Color.FromArgb(213,213,223);
			Color cTitleText=Color.Black;
			Color cTitleBackG=Color.LightGray;
			g.DrawRectangle(new XSolidBrush(cTitleBackG),ToPoints(x),ToPoints(y),ToPoints(Width),ToPoints(_heightHeader));//background
			g.DrawLine(new XPen(Color.FromArgb(102,102,122)),ToPoints(x),ToPoints(y),ToPoints(x+Width),ToPoints(y));//line between title and headers
			XFont xHeaderFont=new XFont(_fontHeader.FontFamily.Name.ToString(),_fontHeader.Size,XFontStyle.Bold);
			for(int i=0; i < ListGridColumns.Count; i++) {
				if(i != 0) {
					g.DrawLine(new XPen(cOutline), ToPoints(x + (-hScroll.Value + ListGridColumns[i].State.XPos)), ToPoints(y),
						ToPoints(x + (-hScroll.Value + ListGridColumns[i].State.XPos)), ToPoints(y + _heightHeader));
				}
				float xFloat=(float)x + (float)(-hScroll.Value + ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width /2);//for some reason visual studio would not allow this statement within the DrawString Below.
				DrawStringX(g, ListGridColumns[i].Heading, xHeaderFont, XBrushes.Black,new XRect(p(xFloat), ToPoints(y - 3),100,100), XStringAlignment.Center);
			}//end for columns.Count
			//Outline the Title
			XPen pen=new XPen(cOutline);
			g.DrawRectangle(pen,ToPoints(x),ToPoints(y),ToPoints(Width),ToPoints(_heightHeader));
			g.DrawLine(new XPen(cOutline),ToPoints(x),ToPoints(y+_heightHeader),ToPoints(x+Width),ToPoints(y+_heightHeader));
		}
		#endregion Methods - Sheets		

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
				if(_selectionMode==GridSelectionMode.One) {
					_selectedIndices.Clear();//clear existing selection before assigning the new one.
				}
				if(!_selectedIndices.Contains(index)) {
					_selectedIndices.Add(index);
				}
			}
			else {//unselect specified index
				if(_selectedIndices.Contains(index)) {
					_selectedIndices.Remove(index);
				}
			}
			Invalidate();
		}

		///<summary>Throws exceptions.  Allows setting multiple values all at once</summary>
		public void SetSelected(int[] iArray,bool setValue) {
			if(_selectionMode==GridSelectionMode.None) {
				throw new Exception("Selection mode is none.");
			}
			if(_selectionMode==GridSelectionMode.One) {
				throw new Exception("Selection mode is one.");
			}
			for(int i=0;i<iArray.Length;i++) {
				if(setValue) {//select specified index
					if(iArray[i]<0 || iArray[i]>ListGridRows.Count-1) {//check to see if index is within the valid range of values
						return;//if not, then ignore.
					}
					if(!_selectedIndices.Contains(iArray[i])) {
						_selectedIndices.Add(iArray[i]);
					}
				}
				else {//unselect specified index
					if(_selectedIndices.Contains(iArray[i])) {
						_selectedIndices.Remove(iArray[i]);
					}
				}
			}
			Invalidate();
		}

		///<summary>Selects or clears all rows.  Throws exceptions, especially when not MultiExtended.</summary>
		public void SetAll(bool setToValue){
			if(_selectionMode==GridSelectionMode.None) {
				throw new Exception("Selection mode is none.");
			}
			if(_selectionMode==GridSelectionMode.One && setToValue==true) {
				throw new Exception("Selection mode is one.");
			}
			if(_selectionMode==GridSelectionMode.OneCell) {
				throw new Exception("Selection mode is OneCell.");
			}
			_selectedIndices.Clear();
			if(setToValue) {//select all
				for(int i=0;i<ListGridRows.Count;i++) {
					_selectedIndices.Add(i);
				}
			}
			Invalidate();
		}

		///<summary>Throws exceptions.</summary>
		public void SetSelected(Point setCell) {
			if(_selectionMode!=GridSelectionMode.OneCell) {
				throw new Exception("Selection mode must be OneCell.");
			}
			_selectedCell=setCell;
			if(textEdit!=null) {
				textEdit.Dispose();
			}
			if(ListGridColumns[_selectedCell.X].IsEditable) {
				CreateEditBox();
			}
			Invalidate();
		}

		///<summary>If one row is selected, it returns the index to that row.  If more than one row are selected, it returns the first selected row.  Really only useful for SelectionMode.One.  If no rows selected, returns -1.</summary>
		public int GetSelectedIndex() {
			if(SelectionMode==GridSelectionMode.OneCell) {
				return _selectedCell.Y;
			}
			if(_selectedIndices.Count>0) {
				return (int)_selectedIndices[0];
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
		///<summary>Usually called after entering a new list to automatically scroll to the top.</summary>
		public void ScrollToTop() {
			ScrollValue=vScroll.Minimum;//this does all error checking and invalidates
		}

		///<summary>Usually called after entering a new list to automatically scroll to the end.</summary>
		public void ScrollToEnd() {
			ScrollValue=vScroll.Maximum;//this does all error checking and invalidates
		}

		///<summary>The index of the row that is the first row displayed on the ODGrid. Also sets ScrollValue.</summary>
		public void ScrollToIndex(int index) {
			if(index>ListGridRows.Count || index < 0) {
				return;
			}
			ScrollValue=ListGridRows[index].State.YPos;
		}

		///<summary>The index of the row that is the last row to be displayed on the ODGrid. Also sets ScrollValue.</summary>
		public void ScrollToIndexBottom(int index) {
			if(index>ListGridRows.Count) {
				return;
			}
			ScrollValue=((ListGridRows[index].State.YPos+ListGridRows[index].State.HeightMain+ListGridRows[index].State.HeightNote+OriginY())-Height)+3;//+3 accounts for the grid lines.
		}

		///<summary>Calcs header height, resizes and lays out scrollbars, and resizes fonts.  Gets called when resize, dpi changes, or item visibility changes.</summary>
		private void LayoutScrollsAndFonts() {
			if(_isUpdating){
				return;
			}
			//no way to test suspendLayout state.
			if(Width==0 || Height==0){
				return;
			}
			if(_hasMultilineHeaders) {
				TextFormatFlags textFormatFlags=TextFormatFlags.Default;
				if(_hasAutoWrappedHeaders){
					textFormatFlags=TextFormatFlags.WordBreak;
				}
				for(int i=0;i<ListGridColumns.Count;i++){
					Size sizeText=TextRenderer.MeasureText(ListGridColumns[i].Heading,_fontHeader,new Size(ListGridColumns[i].State.Width,int.MaxValue),textFormatFlags);
					int heightThisHeader=sizeText.Height-3;
					if(heightThisHeader>_heightHeader){//96dpi
						_heightHeader=heightThisHeader;
					}
				}
			}
			//Scrollbars could use 96dpi or scaled dpi.  We decided to use scaled, which means we can use scroll values directly for drawing, without scaling them
			vScroll.Width=ScaleI(17);//scroll width is 17 at 96dpi
			hScroll.Height=ScaleI(17);
			vScroll.Location=new Point(this.Width-vScroll.Width-1,OriginY()+1);
			if(this._hScrollVisible) {
				hScroll.Visible=true;
				vScroll.Height=this.Height-OriginY()-hScroll.Height-2;
				hScroll.Location=new Point(1,this.Height-hScroll.Height-1);
				hScroll.Width=this.Width-vScroll.Width-2;
				if(_widthTotal<hScroll.Width) {
					hScroll.Value=0;
					hScroll.Enabled=false;
				}
				else {
					hScroll.Enabled=true;
					hScroll.Minimum = 0;
					hScroll.Maximum=_widthTotal;
					if(hScroll.Width<0){//Don't see how this is possible, but leaving it.
						hScroll.LargeChange=0;
					}
					else{
						hScroll.LargeChange=hScroll.Width;
					}
					hScroll.SmallChange=50;
				}
			}
			else {
				hScroll.Visible=false;
				vScroll.Height=this.Height-OriginY()-2;
			}
			if(vScroll.Height<=0) {
				return;
			}
			if(_heightTotal<vScroll.Height) {
				vScroll.Value=0;
				vScroll.Enabled=false;
				vScroll.Visible=_vScrollVisible;
			}
			else {
				vScroll.Enabled=true;
				vScroll.Visible=true;
				vScroll.Minimum = 0;
				vScroll.Maximum=_heightTotal;
				vScroll.LargeChange=vScroll.Height;
				vScroll.SmallChange=(int)(14*3.4f);//it's not an even number so that it is obvious to user that rows moved
			}
			//Checks if old scroll position is less than new vScroll.Minimum or greater than vScroll.Maximum and adjusts if necessary.
			//Fixed an issue for a customer (Task 1562673) where viewing Family Module on patient with lengthy insurance grid and then changing to a 
			//patient with a shorter insurance grid did not properly update the vertical scroll position, resulting in a seemingly blank insurance grid 
			//until the user scrolls vertically.  This made it seem like the second patient did not have insurance.
			ScrollValue=vScroll.Value;
			_brushTitleBackground?.Dispose();
			_fontCell?.Dispose();
			_fontCellBold?.Dispose();
			_fontUnderline?.Dispose();
			_fontUnderlineBold?.Dispose();
			_fontHeader?.Dispose();
			_fontTitle?.Dispose();
			_brushTitleBackground=new LinearGradientBrush(new Point(0,0),new Point(0,ScaleI(18)),_colorTitleTop,_colorTitleBottom);
			_fontCell=new Font(FontFamily.GenericSansSerif,ScaleF(8.5f));//this.Font is ignored
			_fontCellBold=new Font(FontFamily.GenericSansSerif,ScaleF(8.5f),FontStyle.Bold);
			_fontUnderline=new Font(FontFamily.GenericSansSerif,ScaleF(8.5f),FontStyle.Underline);
			_fontUnderlineBold=new Font(FontFamily.GenericSansSerif,ScaleF(8.5f),FontStyle.Underline | FontStyle.Bold);
			_fontHeader=new Font(FontFamily.GenericSansSerif,ScaleF(8.5f),FontStyle.Bold);
			_fontTitle=new Font(FontFamily.GenericSansSerif,ScaleF(10),FontStyle.Bold);
		}

		private void vScroll_Scroll(object sender,System.Windows.Forms.ScrollEventArgs e) {
			if(textEdit!=null) {
				textEdit.Dispose();
			}
			Invalidate();
			this.Focus();
		}

		private void hScroll_Scroll(object sender,System.Windows.Forms.ScrollEventArgs e) {
			//if(UpDownKey) return;
			if(_hScrollVisible && e.OldValue!=e.NewValue) {
				OnHorizScrolled(e);
			}
			Invalidate();
			this.Focus();
		}
		#endregion Methods - Scrolling

		#region Methods - Sorting
		///<summary>Set sortedByColIdx to -1 to clear sorting. Copied from SortByColumn. No need to call fill grid after calling this.  Also used in PatientPortalManager.</summary>
		public void SortForced(int sortedByColumnIdx,bool sortedIsAscending) {
			SortedIsAscending=sortedIsAscending;
			SortedByColumnIdx=sortedByColumnIdx;
			if(sortedByColumnIdx==-1) {
				return;
			}
			List<GridRow> rowsSorted=new List<GridRow>();
			for(int i=0;i<ListGridRows.Count;i++) {
				rowsSorted.Add(ListGridRows[i]);
			}
			if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.StringCompare) {
				rowsSorted.Sort(SortStringCompare);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.DateParse) {
				rowsSorted.Sort(SortDateParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.ToothNumberParse) {
				rowsSorted.Sort(SortToothNumberParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.AmountParse) {
				rowsSorted.Sort(SortAmountParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.TimeParse) {
				rowsSorted.Sort(SortTimeParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.VersionNumber) {
				rowsSorted.Sort(SortVersionParse);
			}
			BeginUpdate();
			ListGridRows.Clear();
			for(int i=0;i<rowsSorted.Count;i++) {
				ListGridRows.Add(rowsSorted[i]);
			}
			EndUpdate();
			SortedByColumnIdx=sortedByColumnIdx;//Must be set again since set to -1 in EndUpdate();
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
			if(SortedByColumnIdx==mouseDownCol) {//already sorting by this column
				SortedIsAscending=!SortedIsAscending;//switch ascending/descending.
			}
			else {
				SortedIsAscending=true;//start out ascending
				SortedByColumnIdx=mouseDownCol;
			}
			if(AllowSortingByColumn) {
				OnColumnSortClick();
				SortedByColumnIdx=mouseDownCol;//Must be set again since set to -1 in EndUpdate();
			}
			List<GridRow> rowsSorted=new List<GridRow>();
			for(int i=0;i<ListGridRows.Count;i++) {
				rowsSorted.Add(ListGridRows[i]);
			}
			if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.StringCompare) {
				rowsSorted.Sort(SortStringCompare);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.DateParse) {
				rowsSorted.Sort(SortDateParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.ToothNumberParse) {
				rowsSorted.Sort(SortToothNumberParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.AmountParse) {
				rowsSorted.Sort(SortAmountParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.TimeParse) {
				rowsSorted.Sort(SortTimeParse);
			}
			else if(ListGridColumns[SortedByColumnIdx].SortingStrategy==GridSortingStrategy.VersionNumber) {
				rowsSorted.Sort(SortVersionParse);
			}
			BeginUpdate();
			ListGridRows.Clear();
			for(int i=0;i<rowsSorted.Count;i++) {
				ListGridRows.Add(rowsSorted[i]);
			}
			EndUpdate();
			SortedByColumnIdx=mouseDownCol;//Must be set again since set to -1 in EndUpdate();
			if(AllowSortingByColumn) { //only check this if sorting by column is enabled for the grid
				OnColumnSorted();
			}
		}

		private int SortStringCompare(GridRow row1,GridRow row2) {
			string textRow1=row1.Cells[SortedByColumnIdx].Text??"";
			return (SortedIsAscending?1:-1)*textRow1.CompareTo(row2.Cells[SortedByColumnIdx].Text);
		}

		private int SortDateParse(GridRow row1,GridRow row2) {
			string raw1=row1.Cells[SortedByColumnIdx].Text;
			string raw2=row2.Cells[SortedByColumnIdx].Text;
			DateTime date1=DateTime.MinValue;
			DateTime date2=DateTime.MinValue;
			//TryParse is a much faster operation than Parse in the event that the input won't parse to a date.
			if(DateTime.TryParse(raw1,out date1) &&
				DateTime.TryParse(raw2,out date2)) {
				return (SortedIsAscending?1:-1)*date1.CompareTo(date2);
			}
			else { //One of the inputs is not a date so default string compare.
				return SortStringCompare(row1,row2);
			}
		}

		private int SortTimeParse(GridRow row1,GridRow row2) {
			string raw1=row1.Cells[SortedByColumnIdx].Text;
			string raw2=row2.Cells[SortedByColumnIdx].Text;
			TimeSpan time1;
			TimeSpan time2;
			//TryParse is a much faster operation than Parse in the event that the input won't parse to a date.
			if(TimeSpan.TryParse(raw1,out time1) &&
				TimeSpan.TryParse(raw2,out time2)) {
				return (SortedIsAscending?1:-1)*time1.CompareTo(time2);
			}
			else { //One of the inputs is not a date so default string compare.
				return SortStringCompare(row1,row2);
			}
		}

		private int SortToothNumberParse(GridRow row1,GridRow row2) {
			//remember that teeth could be in international format.
			//fail gracefully
			string raw1=row1.Cells[SortedByColumnIdx].Text;
			string raw2=row2.Cells[SortedByColumnIdx].Text;
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
				string tooth1=Tooth.FromInternat(raw1);
				string tooth2=Tooth.FromInternat(raw2);
				int toothInt1=Tooth.ToInt(tooth1);
				int toothInt2=Tooth.ToInt(tooth2);
				retVal=toothInt1.CompareTo(toothInt2);
			}
			return (SortedIsAscending?1:-1)*retVal;
		}

		private int SortVersionParse(GridRow row1,GridRow row2) {
			Version v1, v2;
			if(!Version.TryParse(row1.Cells[SortedByColumnIdx].Text,out v1)) {
				v1=new Version();//0.0.0.0
			}
			if(!Version.TryParse(row2.Cells[SortedByColumnIdx].Text,out v2)) {
				v2=new Version();//0.0.0.0
			}
			return (SortedIsAscending?1:-1)*v1.CompareTo(v2);
		}

		private int SortAmountParse(GridRow row1,GridRow row2) {
			//This is here because AmountParse does not sort correctly when the amount contains non-numeric characters
			//We could improve this later with some kind of grid text cleaner that is called before running this sort.
			string raw1=row1.Cells[SortedByColumnIdx].Text;			
			raw1=raw1.Replace("$","");
			string raw2=row2.Cells[SortedByColumnIdx].Text;
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
			return (SortedIsAscending?1:-1)*amt1.CompareTo(amt2);
		}



		#endregion Methods - Sorting

		#region Methods - Paging
		///<summary>Must be called when initalizing or changing the full dataset that is used to populate a paged grid. Clears old filter data and navigates the grid to the first page.  Set listSelectedOriginalIndices if grid should attempt to reselect given rows original index, like on signal refresh.  When listSelectedOriginalIndices is empty, this grid will still attempt to maintain the current page.  When listSelectedOriginalIndices is null this grid will navigate to the first page.</summary>
		public void SetPagingData(List<DataRow> listDataRows,List<int> listSelectedOriginalIndices=null) {
			_dictPages.Clear();
			_hasLoadedAllPages=false;
			_listDataRows=listDataRows;
			if(listSelectedOriginalIndices==null) {
				NavigateToPage(1,true);
			}
			else {
				NavigateToFirstIndex(listSelectedOriginalIndices);
			}
		}

		///<summary>Trys to set control to given pageNew, if pageNew is not valid will automatically navigate to upper or lower bound of pages. Set isForcedRefresh true when it is possible that pageNew is the same as _pageCur, like on filter changes or data changes.</summary>
		public void NavigateToPage(int pageNew,bool isForcedRefresh=false) {
			pageNew=Math.Max(1,pageNew);//Lower bound, always valid
			if(!_hasLoadedAllPages) {
				TryFillDataToPage(pageNew);//Fill any data gaps, needs to run before GetCountMaxPageSeen().
			}
			pageNew=ValidateUpperPageBound(pageNew);//Upper bound, always valid.
			if(isForcedRefresh || _pageCur!=pageNew) {
				SetPageCur(pageNew);
			}
		}

		///<summary>Trys to set control to the page associated to the first value in listSelectedOriginalIndices. Will reselect all rows based on if their PagingODGridRow.OriginalIndex matches a value in listSelectedOriginalIndices.</summary>
		public void NavigateToFirstIndex(List<int> listSelectedOriginalIndices) {
			bool doBreakOnPageCur=false;
			if(listSelectedOriginalIndices.Count==0) {//Attempting to navigate to first index of empty list
				//The intent of this is to maintain selections on the current page.
				//Since there are no selections we need to break after we reach the _pageCur.
				doBreakOnPageCur=true;
			}
			int pageNew=1;
			while(true) {
				if(!TryInitDataForPage(pageNew)) {
					break;
				}
				if(doBreakOnPageCur) {
					if(pageNew==_pageCur) {
						break;
					}
				}
				else if(_dictPages[pageNew].Any(x => listSelectedOriginalIndices.Contains(x.OriginalIndex))) {
					break;//We've identified the target page after filling in the data, so we are done.
				}
				pageNew++;
			}
			pageNew=ValidateUpperPageBound(pageNew);
			SetPageCur(pageNew,listSelectedOriginalIndices);
		}

		///<summary>Returns the maximum valid page that can be viewed based on data </summary>
		private int ValidateUpperPageBound(int pageNew) {
			int pageMax=1;
			if(_dictPages.Count>0) {
				pageMax=_dictPages.Keys.Max();
			}
			return Math.Min(pageMax,pageNew);//Upper bound, always at least 1 and always valid.
		}

		private void SetPageCur(int page,List<int> listSelectedOriginalIndices=null) {
			_pageCur=page;
			RefreshGridPage(listSelectedOriginalIndices);
			OnPageChanged();
		}

		///<summary>Called when moving forward to a page we have not seen yet, fills in data gaps. Value of pageNew can be greater then count of total pages, will return once it gets to last page.</summary>
		private void TryFillDataToPage(int page) {
			Action actionCloseProgress=null;
			//It might take a while to navigate more than 3 pages away (depending on filtering, row construction, etc).
			if(Math.Abs(page-_pageCur)>3) {
				actionCloseProgress=ODProgress.Show(ODEventType.ODGrid,typeof(ODGridEvent));//can't replace this with ProgressOD because it's in business layer.
			}
			//Only need to run if moving forward, previous pages are always handled by pages prior.
			//Look ahead 3 pages so that we can determine if links are needed.
			//No harm starting at first page, TryInitDataForPage(...) will instantly return for pages we've already seen.
			for(int pageTemp=1; pageTemp<=(page+2); pageTemp++) {
				ODGridEvent.Fire(ODEventType.ODGrid,"Processing page: "+pageTemp.ToString());
				if(!TryInitDataForPage(pageTemp) || _hasLoadedAllPages) {//Returns false if index is past cout of data we have available or everything has been filtered already.
					break;
				}
			}
			actionCloseProgress?.Invoke();
		}

		///<summary>Initializes grid row data for the given page.  Returns true if pageNum is valid, otherwise false.</summary>
		private bool TryInitDataForPage(int page) {
			switch(GetPagingIndexState(GetStartIndexForPage(page))) {
				case PagingIndexState.NotValid:
					return false;
				case PagingIndexState.ValidDataPreviouslyLoaded:
					return true;
				case PagingIndexState.ValidDataNotLoaded:
					break;//Intentionally blank, valid index but data needs to be created for new rows. Done below.
			}
			//If the calling method is utilizing FuncFilterGridRow then we need to execute the paging logic below based off of item indexes.
			//There could be items in the full dataset that do not show (will not be present in the dictionary) so we cannot assume that the index of each item
			//correlates to what page they display on.  E.g. 10 items per page; the 51st item in the full dataset is not guaranteed to be on the 5th page when using a filter.
			//Therefore, we need to figure out where the 51st item fell based on its index within the current dictionary instead of using the full dataset.
			//The dictionary of pages contains a list of custom objects which keeps track of each items "original index" from the full dataset.
			int indexNextItem;
			if(PagingMode==GridPagingMode.EnabledBottomUp) {
				#region BottomsUp
				indexNextItem=(_listDataRows.Count-1);
				if(_dictPages.Count>0) {
					//Get the next item from the full dataset based on the OriginalIndex of the first item of the last page in our dictionary.
					indexNextItem=_dictPages[_dictPages.Keys.Max()].First().OriginalIndex-1;
				}
				if(!_dictPages.ContainsKey(page)) {//Create page, even if nothing passes FuncDoesObjPassFilter(...)
					_dictPages[page]=new List<PagingGridRow>();
				}
				int countAdded=0;
				while(countAdded<MaxPageRows && indexNextItem>=0) {
					object gridData=_listDataRows[indexNextItem];
					indexNextItem--;
					if(FuncDoesObjPassFilter!=null && !FuncDoesObjPassFilter(gridData)) {
						continue;
					}
					_dictPages[page].Insert(0,new PagingGridRow(gridData,indexNextItem+1));//Add one since we -- above.
					countAdded++;
				}
				//Once this is true we will never get past the above switch statement until data or a filter changes.
				_hasLoadedAllPages=(indexNextItem<0);
				#endregion
			}
			else {
				#region Normal sorting
				indexNextItem=0;
				if(_dictPages.Count>0) {
					//Get the next item from the full dataset based on the OriginalIndex of the last item in our dictionary.
					indexNextItem=_dictPages[_dictPages.Keys.Max()].Last().OriginalIndex+1;
				}
				if(!_dictPages.ContainsKey(page)) {//Create page, even if nothing passes FuncDoesObjPassFilter(...)
					_dictPages[page]=new List<PagingGridRow>();
				}
				int countAdded=0;
				while(countAdded<MaxPageRows && indexNextItem<_listDataRows.Count) {
					object gridData=_listDataRows[indexNextItem];
					indexNextItem++;
					if(FuncDoesObjPassFilter!=null && !FuncDoesObjPassFilter(gridData)) {
						continue;
					}
					_dictPages[page].Add(new PagingGridRow(gridData,indexNextItem-1));//Subtract one since we ++ above.
					countAdded++;
				}
				//Once this is true we will never get past the above switch statement until data or a filter changes.
				_hasLoadedAllPages=(indexNextItem>=_listDataRows.Count);
				#endregion
			}
			return true;
		}
		
		///<summary>Returns the index to start at for given page.  Can return an index that is out of bounds.</summary>
		private int GetStartIndexForPage(int page) {
			return ((page-1)*MaxPageRows);//Zero based index of last element shown on previous page.
		}

		///<summary>Returns state of given index based off of what we've see in grid so far.</summary>
		private PagingIndexState GetPagingIndexState(int index) {
			if(_listDataRows==null || !index.Between(0,_listDataRows.Count-1)) {
				//The index is greater or less than or the paging data was never set.
				return PagingIndexState.NotValid;
			}
			int countItemsSeen=_dictPages.Values.Sum(x => x.Count);
			if(index>=countItemsSeen && _hasLoadedAllPages) {
				//Attempting to look for an index that is past our fully filtered list.
				return PagingIndexState.NotValid;
			}
			else if(index==0 && countItemsSeen==0 && _listDataRows.Count>0) {
				//No data has been loaded but attempting to navigate to the first page.
				return PagingIndexState.ValidDataNotLoaded;
			}
			else if(index>=countItemsSeen) {
				//The index is valid but is outside of the bounds of "seen" items.  Need to load more data into the dictionary.
				return PagingIndexState.ValidDataNotLoaded;
			}
			return PagingIndexState.ValidDataPreviouslyLoaded;
		}

		///<summary>Refreshes the grid with the content for the current page then scrolls to the end if in EnabledBottomUp mode. Optionally pass in a list of indices for currently selected items that need to remain selected.</summary>
		private void RefreshGridPage(List<int> listSelectedOriginalIndices=null) {
			List<int> listSelectedIndicies=new List<int>();
			this.BeginUpdate();
			this.ListGridRows.Clear();//Always clear, even if no pages.
			if(_dictPages.ContainsKey(_pageCur)) {//Only fails when no data or filter validation fails for all data.
				_dictPages[_pageCur].ForEach(x => {
					if(listSelectedOriginalIndices!=null && listSelectedOriginalIndices.Contains(x.OriginalIndex)) {
						listSelectedIndicies.Add(this.ListGridRows.Count);
					}
					this.ListGridRows.Add(x.GetCacheGridRow(FuncConstructGridRow));
				});
			}
			this.EndUpdate();
			if(listSelectedIndicies.Count>0) {
				this.SetSelected(listSelectedIndicies.ToArray(),true);
				ScrollToIndex(listSelectedIndicies.First());
			}
			else if(PagingMode==GridPagingMode.EnabledBottomUp) {
				this.ScrollToEnd();
			}
			else {
				this.ScrollToTop();
			}
		}

		private bool IsIndexValid(int index) {
			return (ListTools.In(GetPagingIndexState(index),PagingIndexState.ValidDataPreviouslyLoaded,PagingIndexState.ValidDataNotLoaded));
		}
		#endregion Methods - Paging

		#region Methods - Public
		///<summary>Call this before adding any rows.  You would typically call Rows.Clear after this.</summary>
		public void BeginUpdate() {
			_isUpdating=true;
		}

		///<summary>Must be called after adding rows.  This computes the columns, computes the rows, lays out the scrollbars, clears SelectedIndices, and invalidates.  Does not zero out scrollVal.  Also used to recompute columns and rows when you want a different dpi. Examples are: 96dpi old monitors, 96dpi sheets, 100dpi printing, 144 dpi for 150% monitor, etc.</summary>
		public void EndUpdate(bool preserveSelected=false) {
			//Sometimes, it seems like scrollVal needs to be reset somehow because it's an inappropriate number, and when you first grab the scrollbar, it jumps.  No time to investigate.
			if(this.IsDisposed) {
				//todo:
				//Occassionally our customers report to us that they get a 'Cannot access a disposed object' from inside this method. We don't have
				//a good idea why this is happening since these errors usually occur on a form that doesn't have any threading. We're putting this here to
				//hopefully prevent some of these errors.
				return;
			}
			_isUpdating=false;
			ComputeColumns();
			using(Graphics g=this.CreateGraphics()) {
				ComputeRows(g);
			}
			LayoutScrollsAndFonts();//seems redundant, but it's not. Must be before ComputeColumns so that font is correct.  Must also be after ComputeColumns so that it has _heightTotal to work with.
			hScroll.Value=0;
			if(!preserveSelected) {
				_selectedIndices=new ArrayList();
				_selectedCell=new Point(-1,-1);
				if(textEdit!=null) {
					textEdit.Dispose();
				}
				SortedByColumnIdx=-1;
			}
			Invalidate();
		}

		///<summary>Exports the grid to a text or Excel file. The user will have the opportunity to choose the location of the export file.</summary>
		public void Export(string fileName) {
			string selectedFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
				//If extension is missing, add .txt extension. VirtualUI won't download if missing extension.
				if(string.IsNullOrEmpty(Path.GetExtension(selectedFilePath))) {
					selectedFilePath+=".txt";
				}
			}
			else {
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.AddExtension=true;
				saveFileDialog.FileName=fileName;
				if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					try {
						Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
						saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
					catch {
						//initialDirectory will be blank
					}
				}
				else {
					saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				saveFileDialog.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
				saveFileDialog.FilterIndex=0;
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				selectedFilePath=saveFileDialog.FileName;
			}
			try {
				using(StreamWriter sw=new StreamWriter(selectedFilePath,false)) {
					String line="";
					for(int i = 0; i < ListGridColumns.Count; i++) {
						line+="\""+ListGridColumns[i].Heading+"\"";
						if(i < ListGridColumns.Count-1) {
							line+="\t";
						}
					}
					sw.WriteLine(line);
					for(int i = 0;i<ListGridRows.Count;i++) {
						line="";
						for(int j = 0; j < ListGridColumns.Count; j++) {
							line+="\""+ListGridRows[i].Cells[j].Text.Replace("\r\n",", ")+"\"";
							if(j < ListGridColumns.Count-1) {
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
			}
			catch {
				MessageBox.Show(Lans.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(selectedFilePath);
			}
			else {
				MessageBox.Show(Lans.g(this,"File created successfully"));
			}
		}

		///<summary>Returns the text in the cell for the given row and column. Will throw if either index is invalid.</summary>
		public string GetText(int cellRow,int cellColumn) {
			return ListGridRows[cellRow].Cells[cellColumn].Text;
		}

		///<summary>This is designed for a very specific scenario with no horizontal scrollbar and all column widths defined.  Use this method to get the idea width of the grid, based on column widths.  This method does not take into account current grid width at all, preventing circular logic.</summary>
		public int GetIdealWidth(){
			int width=0;
			for(int i=0;i<ListGridColumns.Count;i++){
				width+=ListGridColumns[i].ColWidth;
			}
			width=ScaleI(width);
			width+=vScroll.Width+2;
			return width;
		}

		///<summary>Returns row. -1 if no valid row.  Supply the y position in pixels. Always returns the value in terms of the currently displaying rows.</summary>
		public int PointToRow(int y) {
			if(y<1+OriginY()) {
				return -1;
			}
			for(int i=0;i<ListGridRows.Count;i++) {
				if(!ListGridRows[i].State.Visible){
					continue;
				}
				if(y>-vScroll.Value+1+OriginY()+ListGridRows[i].State.YPos+ListGridRows[i].State.HeightMain+ListGridRows[i].State.HeightNote) {
					continue;//clicked below this row.
				}
				return i;
			}
			return -1;
		}

		///<summary>Returns col.  Supply the x position in pixels. -1 if no valid column.</summary>
		public int PointToCol(int x) {
			int colRight;//the right edge of each column
			for(int i=0; i < ListGridColumns.Count; i++) {
				colRight=0;
				for(int c=0; c < i + 1; c++) {
					colRight+= ListGridColumns[c].State.Width;
				}
				if(x > -hScroll.Value + colRight) {
					continue;//clicked to the right of this col
				}
				return i;
			}
			return -1;
		}

			///<summary>(Not used for sheets) If there are more pages to print, it returns -1.  If this is the last page, it returns the yPos of where the printing stopped.  Graphics will be paper, pageNumber resets some class level variables at page 0, bounds are used to contain the grid drawing, and marginTopFirstPage leaves room so as to not overwrite the title and subtitle.</summary>
		public int PrintPage(Graphics g,int pageNumber,Rectangle bounds,int marginTopFirstPage,bool HasHeaderSpaceOnEveryPage=false) {
			//Printers ignore TextRenderingHint.AntiAlias.  
			//And they ignore SmoothingMode.HighQuality.
			//They seem to do font themselves instead of letting us have control.
			//g.TextRenderingHint=TextRenderingHint.AntiAlias;//an attempt to fix the printing measurements.
			//g.SmoothingMode=SmoothingMode.HighQuality;
			//g.PageUnit=GraphicsUnit.Display;
			//float pagescale=g.PageScale;
			//g.PixelOffsetMode=PixelOffsetMode.HighQuality;
			//g.
			if(_printedRows==0) {
				//set row heights 4% larger when printing:
				ComputeRows(g);
			}
			int xPos=bounds.Left;
			//now, try to center in bounds
			if(_widthTotal<bounds.Width) {
				xPos=(int)(bounds.Left+bounds.Width/2-(float)_widthTotal/2);
			}
			SolidBrush textBrush;
			RectangleF textRect;
			Font font=_fontCell;//do not dispose of this font ref.
			//Initialize our pens for drawing.
			int yPos=bounds.Top;
			if(HasHeaderSpaceOnEveryPage) {
				yPos=marginTopFirstPage;//Margin is lower because title and subtitle are printed externally.
			}
			if(pageNumber==0) {
				yPos=marginTopFirstPage;//Margin is lower because title and subtitle are printed externally.
				_printedRows=0;
				_printNoteRemaining="";
			}
			bool isFirstRowOnPage=true;//helps with handling a very tall first row
			#region ColumnHeaders
			//Print column headers on every page.
			g.FillRectangle(Brushes.LightGray,xPos+ListGridColumns[0].State.XPos,yPos,_widthTotal,_heightHeader);
			g.DrawRectangle(Pens.Black,xPos+ListGridColumns[0].State.XPos,yPos,_widthTotal,_heightHeader);
			for(int i=1;i<ListGridColumns.Count;i++) {
				g.DrawLine(Pens.Black,xPos+ListGridColumns[i].State.XPos,yPos,xPos+ListGridColumns[i].State.XPos,yPos+_heightHeader);
			}
			for(int i=0;i<ListGridColumns.Count;i++){
				g.DrawString(ListGridColumns[i].Heading,_fontHeader,Brushes.Black,
					xPos+ListGridColumns[i].State.XPos + ListGridColumns[i].State.Width /2- g.MeasureString(ListGridColumns[i].Heading, _fontHeader).Width/2,
					yPos);
			}
			yPos+=_heightHeader;
			#endregion ColumnHeaders
			Pen gridPen=_penGridline;
			Pen lowerPen=new Pen(_penGridline.Color);
			if(_printedRows==ListGridRows.Count-1) {//last row
				lowerPen=new Pen(_penColumnSeparator.Color);
			}
			else {
				if(ListGridRows.Count>0 && ListGridRows[_printedRows].ColorLborder!=Color.Empty) {
					lowerPen=new Pen(ListGridRows[_printedRows].ColorLborder);
				}
			}
			#region Rows
			while(_printedRows<ListGridRows.Count) {
				#region RowMainPart
				if(_printNoteRemaining=="") {//We are not in the middle of a note from a previous page. If we are in the middle of a note that will get printed next, as it is the next region of code (RowNotePart).
					//Go to next page if it doesn't fit.
					if(yPos+(float)ListGridRows[_printedRows].State.HeightMain > bounds.Bottom) {//The row is too tall to fit
						if(isFirstRowOnPage) {
							//todo some day: handle very tall first rows.  For now, print what we can.
						}
						else {
							break;//Go to next page.
						}
					}
					//There is enough room to print this row.
					//Draw the left vertical gridline
					g.DrawLine(gridPen,
						xPos+ListGridColumns[0].State.XPos,
						yPos,
						xPos+ListGridColumns[0].State.XPos,
						yPos+ListGridRows[_printedRows].State.HeightMain);
					for(int i=0; i < ListGridColumns.Count; i++) {
						//Draw the other vertical gridlines
						g.DrawLine(gridPen,
							xPos+ListGridColumns[i].State.Right,
							yPos,
							xPos+ListGridColumns[i].State.Right,
							yPos+ListGridRows[_printedRows].State.HeightMain);
						if(ListGridRows[_printedRows].Note=="") {//End of row. Mark with a dark line (lowerPen).
							//Horizontal line which divides the main part of the row from the notes section of the row one column at a time.
							g.DrawLine(lowerPen,
								xPos+ListGridColumns[0].State.XPos,
								yPos+ListGridRows[_printedRows].State.HeightMain,
								xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
								yPos+ListGridRows[_printedRows].State.HeightMain);
						}
						else {//Middle of row. Still need to print the note part of the row. Mark with a medium line (gridPen).
							//Horizontal line which divides the main part of the row from the notes section of the row one column at a time.
							g.DrawLine(gridPen,
								xPos+ListGridColumns[0].State.XPos,
								yPos+ListGridRows[_printedRows].State.HeightMain,
								xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
								yPos+ListGridRows[_printedRows].State.HeightMain);
						}
						//text
						if(ListGridRows[_printedRows].Cells.Count-1< i) {
							continue;
						}
						switch(ListGridColumns[i].TextAlign) {
							case HorizontalAlignment.Left:
								_stringFormat.Alignment = StringAlignment.Near;
								break;
							case HorizontalAlignment.Center:
								_stringFormat.Alignment = StringAlignment.Center;
								break;
							case HorizontalAlignment.Right:
								_stringFormat.Alignment = StringAlignment.Far;
								break;
						}
						if(ListGridRows[_printedRows].Cells[i].ColorText== Color.Empty) {
							textBrush=new SolidBrush(ListGridRows[_printedRows].ColorText);
						}
						else {
							textBrush=new SolidBrush(ListGridRows[_printedRows].Cells[i].ColorText);
						}
						if(ListGridRows[_printedRows].Cells[i].Bold== YN.Yes) {
							font= _fontCellBold;
						}
						else if(ListGridRows[_printedRows].Cells[i].Bold== YN.No) {
							font= _fontCell;
						}
						else {//unknown.  Use row bold
							if(ListGridRows[_printedRows].Bold) {
								font= _fontCellBold;
							}
							else {
								font= _fontCell;
							}
						}
						//Do not underline if printing grid
						//if(rows[RowsPrinted].Cells[i].Underline==YN.Yes) {//Underline the current cell.  If it is already bold, make the cell bold and underlined.
						//	cellFont=new Font(cellFont,(cellFont.Bold)?(FontStyle.Bold | FontStyle.Underline):FontStyle.Underline);
						//}
						//Some printers will malfunction (BSOD) if print bold colored fonts.  This prevents the error.
						if(textBrush.Color != Color.Black && font.Bold) {
							font= _fontCell;
						}
						if(ListGridColumns[i].TextAlign== HorizontalAlignment.Right) {
							textRect=new RectangleF(
								xPos+ListGridColumns[i].State.XPos -2,
								yPos,
								ListGridColumns[i].State.Width +2,
								ListGridRows[_printedRows].State.HeightMain);
							//shift the rect to account for MS issue with strings of different lengths
							//js- 5/2/11,I don't understand this.  I would like to research it.
							textRect.Location=new PointF
								(textRect.X + g.MeasureString(ListGridRows[_printedRows].Cells[i].Text, font).Width/ textRect.Width,
								textRect.Y);
							//g.DrawString(rows[RowsPrinted].Cells[i].Text,cellFont,textBrush,textRect,_format);

						}
						else {
							textRect=new RectangleF(
								xPos+ListGridColumns[i].State.XPos,
								yPos,
								ListGridColumns[i].State.Width,
								ListGridRows[_printedRows].State.HeightMain);
							//g.DrawString(rows[RowsPrinted].Cells[i].Text,cellFont,textBrush,textRect,_format);
						}
						g.DrawString(ListGridRows[_printedRows].Cells[i].Text, font, textBrush, textRect, _stringFormat);
					}
					yPos+=(int)((float)ListGridRows[_printedRows].State.HeightMain);//Move yPos down the length of the row (not the note).
				}
				#endregion RowMainPart
				#region NotePart
				if(ListGridRows[_printedRows].Note=="") {
					_printedRows++;//There is no note. Go to next row.
					isFirstRowOnPage=false;
					continue; 
				}
				//Figure out how much vertical distance the rest of the note will take up.
				int noteHeight;
				int noteW=0;
				_stringFormat.Alignment=StringAlignment.Near;
				for(int i=NoteSpanStart;i<=NoteSpanStop;i++) {
					noteW+=ListGridColumns[i].State.Width;
				}
				if(_printNoteRemaining=="") {//We are not in the middle of a note.
					_printNoteRemaining=ListGridRows[_printedRows].Note;
				}
				noteHeight=(int)g.MeasureString(_printNoteRemaining,font,noteW,_stringFormat).Height; //This is how much height the rest of the note will take.
				bool roomForRestOfNote=false;
				//Test to see if there's enough room on the page for the rest of the note.
				if(yPos+noteHeight<bounds.Bottom) {
					roomForRestOfNote=true;
				}
				#region PrintRestOfNote
				if(roomForRestOfNote) { //There is enough room
					//print it
					//draw lines for the rest of the note
					if(noteHeight>0) {
						//left vertical gridline
						if(NoteSpanStart!=0) {
							g.DrawLine(gridPen,
								xPos+(float)ListGridColumns[NoteSpanStart].State.XPos,
								yPos,
								xPos+(float)ListGridColumns[NoteSpanStart].State.XPos,
								yPos+noteHeight);
						}
						//right vertical gridline
						g.DrawLine(gridPen,
							xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
							yPos,
							xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
							yPos+ noteHeight);
						//left vertical gridline
						g.DrawLine(gridPen,
							xPos+ListGridColumns[0].State.XPos,
							yPos,
							xPos+ListGridColumns[0].State.XPos,
							yPos+noteHeight);
					}
					//lower horizontal gridline gets marked with the dark lowerPen since this is the end of a row
					g.DrawLine(lowerPen,
						xPos+ ListGridColumns[0].State.XPos,
						yPos+ noteHeight,
						xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
						yPos+ noteHeight);
					//note text
					if(noteHeight > 0 && NoteSpanStop > 0 && NoteSpanStart < ListGridColumns.Count) {
						if(ListGridRows[_printedRows].Bold) {
							font= _fontCellBold;
						}
						else {
							font= _fontCell;
						}
						textBrush=new SolidBrush(ListGridRows[_printedRows].ColorText);
						textRect=new RectangleF(
							xPos+ListGridColumns[NoteSpanStart].State.XPos +1,
							yPos,
							ListGridColumns[NoteSpanStop].State.Right -ListGridColumns[NoteSpanStart].State.XPos,
							noteHeight);
						g.DrawString(_printNoteRemaining, font, textBrush, textRect, _stringFormat);
					}
					_printNoteRemaining="";
					_printedRows++;
					isFirstRowOnPage=false;
					yPos+=noteHeight;
				}
				#endregion PrintRestOfNote
				#region PrintPartOfNote
				else {//The rest of the note will not fit on this page.
					//Print as much as you can.
					noteHeight=bounds.Bottom-yPos;//This is the amount of space remaining.
					if(noteHeight<15) {
						return -1; //If noteHeight is less than this we will get a negative value for the rectangle of space remaining because we subtract 15 from this for the rectangle size when using measureString. This is because one line takes 15, and if there is 1 pixel of height available, measureString will fill it with text, which will then get partly cut off. So when we use measureString we will subtract 15 from the noteHeight.
					}							
					SizeF sizeF;
					int charactersFitted;
					int linesFilled;
					string noteFitted;//This is the part of the note we will print.
					//js- I'd like to incorporate ,StringFormat.GenericTypographic into the MeasureString, but can't find the overload.
					sizeF=g.MeasureString(_printNoteRemaining,font,new SizeF(noteW,noteHeight-15),_stringFormat,out charactersFitted,out linesFilled);//Text that fits will be NoteRemaining.Substring(0,charactersFitted).
					noteFitted=_printNoteRemaining.Substring(0,charactersFitted);
					//draw lines for the part of the note that fits on this page
					if(noteHeight>0) {
						//left vertical gridline
						if(NoteSpanStart!=0) {
							g.DrawLine(gridPen,
								xPos+(float)ListGridColumns[NoteSpanStart].State.XPos,
								yPos,
								xPos+(float)ListGridColumns[NoteSpanStart].State.XPos,
								yPos+noteHeight);
						}
						//right vertical gridline
						g.DrawLine(gridPen,
							xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
							yPos,
							xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
							yPos+ noteHeight);
						//left vertical gridline
						g.DrawLine(gridPen,
							xPos+ListGridColumns[0].State.XPos,
							yPos,
							xPos+ListGridColumns[0].State.XPos,
							yPos+noteHeight);
					}
					//lower horizontal gridline gets marked with gridPen since its still the middle of a row (still more note to print)
					g.DrawLine(gridPen,
						xPos+ListGridColumns[0].State.XPos,
						yPos+noteHeight,
						xPos+ListGridColumns[ListGridColumns.Count-1].State.Right,
						yPos+noteHeight);
					//note text
					if(noteHeight > 0 && NoteSpanStop > 0 && NoteSpanStart < ListGridColumns.Count) {
						if(ListGridRows[_printedRows].Bold) {
							font= _fontCellBold;
						}
						else {
							font= _fontCell;
						}
						textBrush=new SolidBrush(ListGridRows[_printedRows].ColorText);
						textRect=new RectangleF(
							xPos+(float)ListGridColumns[NoteSpanStart].State.XPos +1,
							yPos,
							ListGridColumns[NoteSpanStop].State.Right -ListGridColumns[NoteSpanStart].State.XPos,
							noteHeight);
						g.DrawString(noteFitted, font, textBrush, textRect, _stringFormat);
					}
					_printNoteRemaining=_printNoteRemaining.Substring(charactersFitted);
					break;
				}
				#endregion PrintPartOfNote
				#endregion Rows
			}
			#endregion Rows
			lowerPen.Dispose();
			if(_printedRows==ListGridRows.Count) {//done printing
				//set row heights back to screen heights.
				using(Graphics gfx=this.CreateGraphics()) {
					ComputeRows(gfx);
				}
				return yPos;
			}
			else{//more pages to print
				return -1;
			}
		}
		#endregion Methods - Public

		#region Methods - Private Static
		///<summary>For the given background and selected color, will average the two to return the color of the row or cells background if selected.  If the color is white, will simply return the selected color.</summary>
		private static Color GetSelectedColor(Color colorBackground,Color selectedColor) {
			Color newBackgroundColor;
			if(colorBackground.ToArgb()!=Color.White.ToArgb()) {
				newBackgroundColor=Color.FromArgb(
					(selectedColor.R+colorBackground.R)/2,
					(selectedColor.G+colorBackground.G)/2,
					(selectedColor.B+colorBackground.B)/2);
			}
			else {
				newBackgroundColor=selectedColor;
			}
			return newBackgroundColor;
		}

		///<summary>The pdfSharp version of drawstring.  g is used for measurement.  scaleToPix scales xObjects to pixels.</summary>
		private static void DrawStringX(XGraphics xg,string str,XFont xfont,XBrush xbrush,XRect xbounds,XStringAlignment sa) {
			Graphics g=Graphics.FromImage(new Bitmap(100,100));//only used for measurements.
			int topPad=0;// 2;
			int rightPad=5;//helps get measurements better.
			double scaleToPix=1d/ToPoints(1);
			//There are two coordinate systems here: pixels (used by us) and points (used by PdfSharp).
			//MeasureString and ALL related measurement functions must use pixels.
			//DrawString is the ONLY function that uses points.
			//pixels:
			Rectangle bounds=new Rectangle((int)(scaleToPix*xbounds.Left),
				(int)(scaleToPix*xbounds.Top),
				(int)(scaleToPix*xbounds.Width),
				(int)(scaleToPix*xbounds.Height));
			FontStyle fontstyle=FontStyle.Regular;
			if(xfont.Style==XFontStyle.Bold) {
				fontstyle=FontStyle.Bold;
			}
			//pixels: (except Size is em-size)
			Font font=new Font(xfont.Name,(float)xfont.Size,fontstyle);
			bool hasNonAscii=str.Any(x => x > 127);
			if(hasNonAscii) {
				XPdfFontOptions options=new XPdfFontOptions(PdfFontEncoding.Unicode,PdfFontEmbedding.Always);
				xfont=new XFont(xfont.Name,xfont.Size,xfont.Style,options);
			}
			else {
				xfont=new XFont(xfont.Name,xfont.Size,xfont.Style);
			}
			//pixels:
			SizeF fit=new SizeF((float)(bounds.Width-rightPad),(float)(font.Height));
			StringFormat format=StringFormat.GenericTypographic;
			//pixels:
			float pixelsPerLine=(float)font.Height-0.5f;//LineSpacingForFont(font.Name) * (float)font.Height;
			float lineIdx=0;
			int chars;
			int lines;
			//points:
			RectangleF layoutRectangle;
			for(int ix=0;ix<str.Length;ix+=chars) {
				if(bounds.Y+topPad+pixelsPerLine*lineIdx>bounds.Bottom) {
					break;
				}
				//pixels:
				g.MeasureString(str.Substring(ix),font,fit,format,out chars,out lines);
				//PdfSharp isn't smart enough to cut off the lower half of a line.
				//if(bounds.Y+topPad+pixelsPerLine*lineIdx+font.Height > bounds.Bottom) {
				//	layoutH=bounds.Bottom-(bounds.Y+topPad+pixelsPerLine*lineIdx);
				//}
				//else {
				//	layoutH=font.Height+2;
				//}
				//use points here:
				float adjustTextDown=10f;//this value was arrived at by trial and error.
				layoutRectangle=new RectangleF(
					(float)xbounds.X,
					//(float)(xbounds.Y+(float)topPad/scaleToPix+(pixelsPerLine/scaleToPix)*lineIdx),
					(float)(xbounds.Y+adjustTextDown+(pixelsPerLine/scaleToPix)*lineIdx),
					(float)xbounds.Width+50,//any amount of extra padding here will not cause malfunction
					0);//layoutH);
				XStringFormat sf=XStringFormats.Default;
				sf.Alignment=sa;
				xg.DrawString(str.Substring(ix,chars),xfont,xbrush,(double)layoutRectangle.Left,(double)layoutRectangle.Top,sf);
				lineIdx+=1;
			}
			g.Dispose();
		}

		///<summary>This line spacing is specifically picked to match the RichTextBox.</summary>
		private static float LineSpacingForFont(string fontName) {
			if(fontName.ToLower()=="arial") {
				return 1.055f;
			}
			else if(fontName.ToLower()=="courier new") {
				return 1.055f;
			}
			//else if(fontName.ToLower()=="microsoft sans serif"){
			//	return 1.00f;
			//}
			return 1.05f;
		}
		
		///<summary>Converts pixels used by us to points used by PdfSharp.</summary>
		private static double ToPoints(int pixels){
			XUnit xunit=XUnit.FromInch((double)pixels/100d);//100 ppi
			return xunit.Point;
				//XUnit.FromInch((double)pixels/100);
		}

		///<summary>Converts pixels used by us to points used by PdfSharp.</summary>
		private static double p(float pixels){
			XUnit xunit=XUnit.FromInch((double)pixels/100d);//100 ppi
			return xunit.Point;
		}
		#endregion Methods - Private Static

		#region Methods - Private
		///<summary>Computes the position of each column and the overall width.  Called from endUpdate and also from OnPaint.</summary>
		private void ComputeColumns() {
			if(!_hScrollVisible) {
				float sumDynamic=0;//sum of the weights of the dynamic columns, typically about 1 or 2.
				int widthFixedSum=0;
				for(int i=0;i<ListGridColumns.Count;i++){
					//if(ListGridColumns[i].ColWidth<1){
						//todo: handle this in the ColWidth property instead
						//throw new ApplicationException("Grid column width less than 1 not allowed.");
						//ListGridColumns[i].State.Width=20;//just temporary
					//}
					if(ListGridColumns[i].IsWidthDynamic){
						sumDynamic+=ListGridColumns[i].DynamicWeight;
					}
					else{
						ListGridColumns[i].State.Width=ScaleI(ListGridColumns[i].ColWidth);
						widthFixedSum+=ListGridColumns[i].State.Width;
					}
				}
				string name=this.Name;
				//Debug.WriteLine(name);
				if(sumDynamic>0){
					int widthExtra=Width-2-widthFixedSum;
					if(vScroll.Visible){// && vScroll.Enabled){
						widthExtra-=vScroll.Width;
					}
					for(int i=0;i<ListGridColumns.Count;i++){
						if(ListGridColumns[i].IsWidthDynamic){
							//example sum=1+2.5, width=350/3.5*2.5=250
							ListGridColumns[i].State.Width=(int)(widthExtra/sumDynamic*ListGridColumns[i].DynamicWeight);
						}
					}
				}
				else if(ListGridColumns.Count>0 && !IsForSheets) {//resize the last column automatically
					int widthExtra=Width-2-widthFixedSum+ScaleI(ListGridColumns[ListGridColumns.Count-1].ColWidth);
					if(vScroll.Visible){
						widthExtra-=vScroll.Width;
					}
					if(widthExtra>0){
						ListGridColumns[ListGridColumns.Count-1].State.Width=widthExtra;
					}
				}
			}
			else{
				for(int i=0;i<ListGridColumns.Count;i++){
					ListGridColumns[i].State.Width=ScaleI(ListGridColumns[i].ColWidth);
				}
			}
			//widths are all set
			for(int i=0;i<ListGridColumns.Count;i++){
				if(i==0){
					ListGridColumns[i].State.XPos=0;
				}
				else{
					ListGridColumns[i].State.XPos=ListGridColumns[i-1].State.XPos+ListGridColumns[i-1].State.Width;
				}
				ListGridColumns[i].State.Right=ListGridColumns[i].State.XPos+ListGridColumns[i].State.Width;
			}
			if(ListGridColumns.Count>0) {
				_widthTotal=ListGridColumns[ListGridColumns.Count-1].State.Right;
			}
		}

		///<summary>Called from PrintPage() and EndUpdate().  After adding rows to the grid, this calculates the height of each row because some rows may have text wrap and will take up more than one row.  Also, rows with notes must be made much larger because notes start on the second line.  If column images are used, rows will be enlarged to make space for the images.</summary>
		private void ComputeRows(Graphics g) {
			//We can create/dispose 500k fonts per second so we could do that instead. 
			Font fontNormal=_fontCell;//do not dispose any of these font refs.
			Font fontBold=_fontCellBold;
			if(_fontForSheets!=null) {
				fontNormal=_fontForSheets;
				fontBold=_fontForSheetsBold;
			}
			Font font;
			_heightTotal=0;
			int heightCell;
			int widthNote=0;
			if(0 <= NoteSpanStart && NoteSpanStart < ListGridColumns.Count
				&&  0 < NoteSpanStop && NoteSpanStop <= ListGridColumns.Count)
			{
				for(int i=NoteSpanStart;i<=NoteSpanStop;i++) {
					widthNote+=ListGridColumns[i].State.Width;
				}
			}
			int heightImage=0;
			_hasEditableColumn=false;
			for(int i=0;i<ListGridColumns.Count;i++) {
				if(ListGridColumns[i].IsEditable || ListGridColumns[i].ListDisplayStrings != null) {
					_hasEditableColumn=true;
				}
				if(ListGridColumns[i].ImageList!=null) {
					if(ScaleI(ListGridColumns[i].ImageList.ImageSize.Height)> heightImage) {
						heightImage=ScaleI(ListGridColumns[i].ImageList.ImageSize.Height)+1;
					}
				}
			}
			int yPos=0;
			for(int i=0;i<ListGridRows.Count;i++) {
				for(int j=0;j<ListGridRows[i].Cells.Count;j++) {
					if(ListGridRows[i].Cells[j].Text!=null && ListGridRows[i].Cells[j].Text.Length>=TEXT_LENGTH_LIMIT){
						ListGridRows[i].Cells[j].Text=ListGridRows[i].Cells[j].Text.Substring(0,TEXT_LENGTH_LIMIT);
					}
				}
				ListGridRows[i].State.HeightMain=0;
				font=fontNormal;
				if(ListGridRows[i].Bold==true) {
					font=fontBold;
				}
				else {
					//Determine if any cells in this row are bold.  If at least one cell is bold, then we need to calculate the row height using bold font.
					//Bold only affects width, but if there is wrap, then width can affect height.
					for(int j=0;j<ListGridRows[i].Cells.Count;j++) {
						if(ListGridRows[i].Cells[j].Bold==YN.Yes) {//We don't care if a cell is underlined because it does not affect the size of the row
							font=fontBold;
							break;
						}
					}
				}
				if(WrapText) {//make row taller after measuring
					//find the tallest col
					for(int j=0;j<ListGridRows[i].Cells.Count;j++) {
						if(_hasEditableColumn) {
							//todo: clean up this math
							//doesn't seem to calculate right when it ends in a "\r\n". It doesn't make room for the new line. Make it, by adding another one for calculations.
							heightCell=(int)Math.Ceiling(((1.03) 
								*(float)(g.MeasureString(ListGridRows[i].Cells[j].Text+"\r\n",font,ListGridColumns[j].State.Width,_stringFormat).Height))+4);//because textbox will be bigger
							if(heightCell < ScaleI(EDITABLE_ROW_HEIGHT)) {
								heightCell=ScaleI(EDITABLE_ROW_HEIGHT);//only used for single line text
							}
						}
						else {//no editable column
							float hTemp;
							//hTemp=g.MeasureString(ListGridRows[i].Cells[j].Text,font,ListGridColumns[j].State.Width+7,_stringFormat).Height;
							//The above line can help it look better on screen, with less wasted space at right.  
							//But both printing and sheets need to calculate slightly differently.
							//The line below is a compromise for now, until a more rigorous overhaul can be done
							//to combine all three into one set of drawing commands.
							hTemp=g.MeasureString(ListGridRows[i].Cells[j].Text,font,ListGridColumns[j].State.Width,_stringFormat).Height;
							if(ListGridRows[i].Cells[j].IsButton){
								hTemp=g.MeasureString(ListGridRows[i].Cells[j].Text,font,ListGridColumns[j].State.Width-1,_stringFormat).Height;
							}
							if(_hasDropDowns && j==0) {//only draw the dropdown arrow in the first column of the row.
								if(ListGridRows[i].State.DropDownState!=ODGridDropDownState.None || ListGridRows[i].DropDownParent!=null) {
									hTemp=g.MeasureString(ListGridRows[i].Cells[j].Text,font,ListGridColumns[j].State.Width-10,_stringFormat).Height;
									//might be easier to just not support wrap in a dropdown cell. Oh well.
									if(WrapText && ListGridColumns[j].TextAlign==HorizontalAlignment.Left){
										hTemp=g.MeasureString(ListGridRows[i].Cells[j].Text,font,ListGridColumns[j].State.Width-3,_stringFormat).Height;
									}
								}
							}
							if(IsForSheets) {
								hTemp=LineSpacingForFont(font.Name)*hTemp;
							}
							heightCell=(int)Math.Ceiling(hTemp)-1;
						}
						//if(rows[i].Height==0) {//not set
						//  cellH=(int)Math.Ceiling(g.MeasureString(rows[i].Cells[j].Text,cellFont,_listCurColumnWidths[j]).Height+1);
						//}
						//else {
						//  cellH=rows[i].Height;
						//}
						if(heightCell>ListGridRows[i].State.HeightMain) {
							ListGridRows[i].State.HeightMain=heightCell;
						}
					}
					if(ListGridRows[i].State.HeightMain<4) {
						ListGridRows[i].State.HeightMain=font.Height;
					}
				}
				else {//text not wrapping
					if(_hasEditableColumn) {
						ListGridRows[i].State.HeightMain=ScaleI(EDITABLE_ROW_HEIGHT);
					}
					else {
						ListGridRows[i].State.HeightMain=font.Height+1;
					}
				}
				if(heightImage>ListGridRows[i].State.HeightMain) {//heightImage is already scaled to current dpi
					ListGridRows[i].State.HeightMain=heightImage;
				}
				if(widthNote>0 && ListGridRows[i].Note!="") {
					string strToMeasure=ListGridRows[i].Note;
					if(strToMeasure.Length>TEXT_LENGTH_LIMIT){
						strToMeasure=strToMeasure.Substring(0,TEXT_LENGTH_LIMIT)+"\r\n"+Lans.g(this,"...more...");
					}
					ListGridRows[i].State.HeightNote=(int)g.MeasureString(strToMeasure,fontNormal,widthNote,_stringFormat).Height;//Notes cannot be bold.  Always normal font.
				}
				if(ListGridRows[i].Cells.Any(x => x.IsButton)) {
					if(ListGridRows[i].State.HeightMain<ScaleI(EDITABLE_ROW_HEIGHT)) {
						ListGridRows[i].State.HeightMain=ScaleI(EDITABLE_ROW_HEIGHT);
					}
				}
				if(_hasDropDowns) {
					if(ListGridRows[i].DropDownParent!=null){//if this is a child
						//we have direct access to the parent object, already
						if(ListGridRows[i].DropDownParent.State.DropDownState==ODGridDropDownState.None){
							//if this is an initial ComputeRows, set Up/Down
							//If it's a recompute after a click, then respect the existing Up/Down
							if(ListGridRows[i].DropDownParent.DropDownInitiallyDown) {
								ListGridRows[i].DropDownParent.State.DropDownState=ODGridDropDownState.Down;
							}
							else{
								ListGridRows[i].DropDownParent.State.DropDownState=ODGridDropDownState.Up;
							}
						}
						//we support multiple cascading parents
						GridRow rowCur=ListGridRows[i];
						//Keeps track of which rows have already been listed as a parent of this row. The same parent should not be added to this list more than one time.
						List<GridRow> listParentRows=new List<GridRow>();
						ListGridRows[i].State.Visible=true;
						while(rowCur.DropDownParent!=null) { //Keep going until we get to the topmost parent row in the tree. If DropDownParent==null, that means this row has no parent.
							if(listParentRows.Contains(rowCur)) {
								throw new Exception("You cannot have a parent row and a child row pointing to each other.");
							}
							listParentRows.Add(rowCur);
							if(rowCur.DropDownParent.State.DropDownState!=ODGridDropDownState.Down) { 
								//if any parent is not down
								ListGridRows[i].State.Visible=false;
							}
							//retain height in case we later make it visible again.
							//we choose not to break here for 2 reasons:
							//-a higher parent might be up, causing this row to be not visible
							//-we want go through all the parents to make sure there is not a circular relationship between parent & child rows.
							//go up one parent, then run the same loop.
							rowCur=rowCur.DropDownParent;
						}
						if(!ListGridRows[i].State.Visible){
							continue;//to the next row. Don't calc yPos
						}
					}//if this is a child
				}//if _hasDropdowns
				ListGridRows[i].State.YPos=yPos;
				yPos+=ListGridRows[i].State.HeightMain+ListGridRows[i].State.HeightNote;
			}//for rows
			_heightTotal=yPos;
			if(IsForSheets) {
				ComputeSheetSupplemental();
			}
		}

		///<summary>Fills ListGridSheetRows with supplemental row information.</summary>
		private void ComputeSheetSupplemental() {
			ListGridSheetRows=new List<GridSheetRow>();
			bool drawTitle=false;
			bool drawHeader=true;
			bool drawFooter=false;
			int yPosCur=SheetYPos;
			int bottomCurPage=SheetPageHeight-SheetBottomMargin;
			while(yPosCur>bottomCurPage) {//advance pages until we are using correct y values. Example: grid starts on page three, yPosCur would be something like 2500
				bottomCurPage+=SheetPageHeight-(SheetTopMargin+SheetBottomMargin);
			}
			for(int i=0;i<ListGridRows.Count;i++) {
				#region Split patient accounts on Statement grids.
				if(i==0
					&& (_title.StartsWith("TreatPlanBenefitsFamily") 
					|| _title.StartsWith("TreatPlanBenefitsIndividual")
					|| _title.StartsWith("StatementPayPlan")
					|| _title.StartsWith("StatementDynamicPayPlan")
					|| _title.StartsWith("StatementInvoicePayment")
					|| _title.StartsWith("StatementMain.NotIntermingled")))
				{
					drawTitle=true;
				}
				else if(_title.StartsWith("StatementMain.NotIntermingled") 
					&& i>0 
					&& ListGridRows[i].Tag.ToString()!=ListGridRows[i-1].Tag.ToString()) //Tag should be PatNum
				{
					yPosCur+=20; //space out grids.
					ListGridSheetRows[i-1].IsBottomRow=true;
					drawTitle=true;
					drawHeader=true;
				}
				#endregion
				#region Page break logic
				if(i==ListGridRows.Count-1 && (_title.StartsWith("StatementPayPlan") || _title.StartsWith("StatementDynamicPayPlan") || _title.StartsWith("StatementInvoicePayment"))) {
					drawFooter=true;
				}
				if(yPosCur //start position of row
					+ListGridRows[i].State.HeightMain //+row height
					+(drawTitle?_heightTitle:0) //+title height if needed
					+(drawHeader?_heightHeader:0) //+header height if needed
					+(drawFooter?_heightTitle:0) //+footer height if needed.
					>=bottomCurPage) 
				{
					if(i>0) {
						ListGridSheetRows[i-1].IsBottomRow=true;//this row causes a page break. Previous row should be a bottom row.
					}
					yPosCur=bottomCurPage+1;
					bottomCurPage+=SheetPageHeight-(SheetTopMargin+SheetBottomMargin);
					drawHeader=true;
				}
				#endregion
				ListGridSheetRows.Add(new GridSheetRow(yPosCur,drawTitle,drawHeader,false,drawFooter));
				yPosCur+=(drawTitle?_heightTitle:0);
				yPosCur+=(drawHeader?_heightHeader:0);
				yPosCur+=ListGridRows[i].State.HeightMain;
				yPosCur+=(drawFooter?_heightTitle:0);
				drawTitle=drawHeader=drawFooter=false;//reset all flags for next row.
				if(i==ListGridRows.Count-1){//set print height equal to the bottom of the last row.
					ListGridSheetRows[i].IsBottomRow=true;
					SheetPrintHeight=yPosCur-SheetYPos;
				}
			}
		}

		private bool ControlIsDown() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl);
		}

		///<summary>Creates combo boxes in the appropriate location of the grid so users can select and change them.</summary>
		private void CreateComboBox() {
			GridCell gridCell=ListGridRows[_selectedCell.Y].Cells[_selectedCell.X];
			GridColumn gridColumn=ListGridColumns[_selectedCell.X];
			comboBox.FlatStyle=FlatStyle.Popup;
			comboBox.DropDownStyle=ComboBoxStyle.DropDownList;
			int colWidth;
			if(gridColumn.DropDownWidth > 0){
				colWidth=gridColumn.DropDownWidth+1;
			}
			else{
				colWidth=ListGridColumns[_selectedCell.X].State.Width+1;
			}
			//int colWidth=(odGridColumn.DropDownWidth > 0) ? odGridColumn.DropDownWidth+1 : ListGridColumns[selectedCell.X].ColWidth+1;
			comboBox.Size=new Size(colWidth,ListGridRows[_selectedCell.Y].State.HeightMain+1);
			comboBox.Location=new Point(-hScroll.Value+1+ListGridColumns[_selectedCell.X].State.XPos,
				-vScroll.Value+1+OriginY()+ListGridRows[_selectedCell.Y].State.YPos+((ListGridRows[_selectedCell.Y].State.HeightMain-comboBox.Size.Height)/2));//Centers the combo box vertically.
			comboBox.Items.Clear();
			for(int i=0;i<gridColumn.ListDisplayStrings.Count;i++) {
				comboBox.Items.Add(gridColumn.ListDisplayStrings[i]);
			}
			comboBox.SelectedIndex=gridCell.ComboSelectedIndex;
			comboBox.Visible=true;
			if(!this.Controls.Contains(comboBox)) {
				comboBox.SelectionChangeCommitted+=new EventHandler(dropDownBox_SelectionChangeCommitted);
				comboBox.GotFocus+=new EventHandler(dropDownBox_GotFocus);
				comboBox.LostFocus+=new EventHandler(dropDownBox_LostFocus);
				comboBox.KeyDown+=new KeyEventHandler(dropDownBox_KeyDown);
				this.Controls.Add(comboBox);
			}
			comboBox.Focus();
			_selectedCellOld=new Point(_selectedCell.X,_selectedCell.Y);
		}

		private void DropDownRowClick(int mouseDownRow,int mouseDownY) {
			int curRow=PointToRow(mouseDownY);
			if(curRow!=_mouseDownRow) {
				//if they click + dragged, don't expand/collapse the row they initially clicked on.
				return;
			}
			if(ListGridRows[mouseDownRow].State.DropDownState==ODGridDropDownState.Up) {
				ListGridRows[mouseDownRow].State.DropDownState=ODGridDropDownState.Down;
			}
			else if(ListGridRows[mouseDownRow].State.DropDownState==ODGridDropDownState.Down) {
				ListGridRows[mouseDownRow].State.DropDownState=ODGridDropDownState.Up;
			}
			using(Graphics g=this.CreateGraphics()) {
				ComputeRows(g);
			}
			LayoutScrollsAndFonts();
			this.Invalidate(); //causes a repaint.
		}

		///<summary>Converts from 96dpi to current scale.</summary>
		private float ScaleF(float val96){
			return val96*ScaleMy;
		}

		///<summary>Converts from 96dpi to current scale.</summary>
		private int ScaleI(float val96){
			//This is here because Dpi.Scale is over in OD. Here, Scale is already used by Windows, so we call it ScaleI.  A refactor could use OpenDentBusiness.UI.DpiScale, but that's too much work, with no benefit.
			return (int)(Math.Round(val96*ScaleMy));
		}

		///<summary>The top left corner of the grid area, right below the title and header.  Converted to current screen dpi.</summary>
		private int OriginY(){
			return ScaleI(_heightTitle+_heightHeader);
		}

		private bool ShiftIsDown() {
			return System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightShift);
		}
		#endregion Methods - Private

		#region Class PagingGridRow
		///<summary>A wrapper class for an object that is displayed in the grid.  Keeps track of where this object is within the list of all objects for the grid.  Provides a helper method that will return a GridRow whenever this object needs to be displayed to the user.</summary>
		private class PagingGridRow {
			///<summary>The original index of this row entry in ListGridRowData.  Used to identify the index to start on for the next page.</summary>
			public int OriginalIndex;
			///<summary>The actual object that this ODGridRow represents.</summary>
			private object _tag;
			///<summary>An ODGridRow representation of _tag.  The construction of this ODGridRow is typically handled by ODGrid.FuncConstructGridRow(). Null unless this row has been shown on the grid at least once.</summary>
			private GridRow _gridRow;

			///<summary>Set tag to the actual object that is associated to a row in the grid and originalIndex to the index for this object in ListGridRowData.</summary>
			public PagingGridRow(object tag,int originalIndex) {
				_tag=tag;
				OriginalIndex=originalIndex;
			}

			///<summary></summary>
			public GridRow GetCacheGridRow(Func<object,GridRow> funcConstructGridRow) {
				if(_gridRow==null) {
					_gridRow=funcConstructGridRow(_tag);
				}
				return _gridRow;
			}
		}
		#endregion Class PagingGridRow

	}

	#region Other Classes
	///<summary>This is the comboBox used in the grid.  We don't have access to ComboBoxOD from here.</summary>
	public class ComboBoxGrid:ComboBox{
		protected override bool IsInputKey(Keys keyData) {
			if(keyData==Keys.Tab){
				return true;//this allows the Tab key to hit our KeyDown event.
			}
			return base.IsInputKey(keyData);
		}
	}

	///<summary>When drawing to sheets, this stores additional information about a row.</summary>
	public class GridSheetRow {
		///<summary>YPos relative to top of entire grid.  When printing this includes adjustments for page breaks.  If row has title/header the title/header should be drawn at this position.</summary>
		public int YPos;
		///<summary>Usually only true for some grids, and only for the first row.</summary>
		public bool IsTitleRow;
		///<summary>Usually true if row is at the top of a new page, or when changing patients in a statement grid.</summary>
		public bool IsHeaderRow;
		///<summary>True for rows that require a bold bottom line, at end of entire grid, at page breaks, or at a separation in the grid.</summary>
		public bool IsBottomRow;
		///<summary>Rarely true, usually only for last row in particular grids.</summary>
		public bool IsFooterRow;

		public GridSheetRow(int yPos,bool isTitleRow,bool isHeaderRow,bool isBottomRow,bool isFooterRow) {
			YPos=yPos;
			IsTitleRow=isTitleRow;
			IsHeaderRow=isHeaderRow;
			IsBottomRow=isBottomRow;
			IsFooterRow=isFooterRow;
		}
	}

	///<summary></summary>
	public class ODGridClickEventArgs {
		///<summary></summary>
		public ODGridClickEventArgs(int col,int row,MouseButtons button) {
			this.Col=col;
			this.Row=row;
			this.Button=button;
		}
		public int Row { get; }
		public int Col { get; }
		///<summary>Gets which mouse button was pressed.</summary>
		public MouseButtons Button { get; }
	}

	///<summary></summary>
	public class ODGridKeyEventArgs {
		///<summary></summary>
		public ODGridKeyEventArgs(KeyEventArgs keyEventArgs) {
			this.KeyEventArgs=keyEventArgs;
		}
		///<summary>Gets which key was pressed.</summary>
		public KeyEventArgs KeyEventArgs { get; }

	}

	public class ODGridPageEventArgs {
		public int PageCur;
		public List<int> ListLinkVals;

		public ODGridPageEventArgs(int pageCur,List<int> listLinkVals) {
			this.PageCur=pageCur;
			this.ListLinkVals=listLinkVals;
		}
	}
	#endregion Other Classes

	#region enum GridSelectionMode
	///<summary>Specifies the selection behavior of an ODGrid.</summary>
	public enum GridSelectionMode {
		///<summary>0-Nothing can be selected.</summary>
		None,
		///<summary>1-Only one row can be selected.</summary>
		One,
		///<summary>2-Only one cell can be selected.</summary>
		OneCell,
		///<summary>3-Multiple rows can be selected, and the user can use the SHIFT, CTRL, and arrow keys to make selections</summary>
		MultiExtended,
	}
	#endregion enum GridSelectionMode

	#region Delegates - Public
	//This is an obsolete pattern.  The new pattern is to use generic event handlers with custom event args.
	///<summary>Used for Cell specific events.</summary>
	public delegate void ODGridClickEventHandler(object sender,ODGridClickEventArgs e);
	///<summary>Used for Cell specific events.</summary>
	public delegate void ODGridKeyEventHandler(object sender,ODGridKeyEventArgs e);
	///<summary>Used when paging is enabled to update UI with current page and previous two / next two link information.</summary>
	public delegate void ODGridPageChangeEventHandler(object sender, ODGridPageEventArgs e);
	#endregion Delegates - Public
}

#region Grid Code Template
/*This is a template of typical grid code which can be quickly pasted into any form.
 
		using OpenDental.UI;

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("Table...","colname"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("Table...","colname"),100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("Table...","colname"),100){Tag=.., etc};
			col.Tag=...//an alternative to above line
			gridMain.ListGridColumns.Add(col);
			 
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_list.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(_list[i].);
				row.Cells.Add("");
				row.Cells.Add(new GridCell(""){ColorText=.., etc});
			  
				row.ColorText=;
				row.Tag=;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}
*/
#endregion Grid Code Template

//Some of the regular expressions used for find/replace:
//[\t ]+this.\w+.HasAddButton ?= ?false;\r\n
//[\t ]+this.\w+.TitleHeight ?= ?18;\r\n
//[\t ]+this.\w+.HeaderHeight ?= ?15;\r\n
//[\t ]+this.\w+.HasMultilineHeaders ?= ?false;\r\n
//[\t ]+[\w\.]*\.HeaderFont ?= ?.*8\.5.*;\r\n
//[\t ]+[\w\.]*\.TitleFont ?= ?.*10.*Bold.*;\r\n
//[\t ]+this.\w+.ScrollValue ?= ?0;\r\n
//[\t ]+[\w\.]*\.HasDropdowns ?= ?false;\r\n

	//this.gridMain.HasDropDowns = false;
