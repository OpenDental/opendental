using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.UI{
	/// <summary>This class has replaced ApptDrawing, ApptSingleDrawing, ContrApptSheet, ContrApptSingle, and ApptOverlapOrdering.  Encapsulates both Data and Drawing for the main area of Appt module and the operatories header.  The Appt module gathers the data and stores it here.  This class does its drawing based mostly on the information passed in, although it does use the standard cache sometimes. It has defaults so that we will always be able to draw something reasonable, even if we're missing data. This class contains extra data that it doesn't actually need.  Appt module uses this data for other things besides drawing appointments.  This class intentionally supports a single thread only.</summary>
	public partial class ControlApptPanel:UserControl {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Public Printing
		///<summary>Only the hour component gets used.</summary>
		public DateTime	DateTimePrintStart;
		///<summary>Only the hour component gets used.</summary>
		public DateTime DateTimePrintStop;		
		public bool IsPrintPreview;		
		public int PagesPrinted;		
		///<summary>During printing, the number of columns per page.</summary>
		public int PrintingColsPerPage=0;
		///<summary>During printing, the horizontal page number that we are currently on.</summary>
		public int PrintingPageColumn;
		///<summary>During printing, the vertical page number that we are currently on.</summary>
		public int PrintingPageRow;
		public float PrintingSizeFont;
		#endregion Fields - Public Printing

		#region Fields - Private isValid flags
		///<summary>This will trigger redrawing of the provbars header and Ops header.  Not as frequent as Main.</summary>
		private bool _isValidHeaders=false;		
		///<summary>This will trigger redrawing everything in the main area, including appointments. It also triggers redraw of a second bitmap for ProvBars.</summary>
		private bool _isValidMain=false;//start false to trigger initial draw
		///<summary>This will trigger redrawing only L&R timebars.</summary>
		private bool _isValidTimebars=false;
		#endregion Fields - Private isValid flags

		#region Fields - Private Measurements
		///<summary>Height of single line at 96dpi.  Completely dependent on PrefName.ApptFontSize	or PrefName.ApptPrintFontSize.  We round this number instead of using float font height because horizontal lines need to snap to pixel to avoid looking distractingly different.</summary>
		private float _heightLine=12f;
		///<summary>Height of the main appt area, including the parts that are not visible due to scrolling.  Always calculated internally and does not depend on size of control. Does not include headers.  ProvBars and timeBars are also this high.  Changed during printing.</summary>
		private int _heightMain=500;
		///<summary>Height of the main appt area portion that is visible.  Derived from other dimensions when resizing.  Also same as height of timebar and provbar.  Ignored during printing; just use _heightMain.</summary>
		private int _heightMainVisible=500;
		///<summary>Height of prov header and ops header. Starts out at 16, but can be bumped up to 27 if any visible op requires double row title.</summary>
		private float _heightProvOpHeaders=16f;
		///<summary>Allows us to skip some redrawing until the control is actually loaded.</summary>
		private bool _isControlLoaded;
		///<summary>This is set during each PrintPage event.</summary>
		private bool _isPrinting=false;
		///<summary>Keeps track of where the appointments are located.</summary>
		private List<ApptLayoutInfo> _listApptLayoutInfos;
		///<summary>Typical values would be 10,15,5,or 7.5.  Derived from minPerIncr_10 / RowsPerIncr_2 =5. </summary>
		private float _minPerRow=10f;
		///<summary>Always 7, although we might have previously allowed 5. Only used with weekview.</summary>
		private int _numOfWeekDaysToDisplay=7;
		///<summary>Stores the shading info for the provider bars on the left of the appointments module.  First dim is number of visible providers.  Second dim is vertical cells for different times.  Ignores RowsPerIncr. For example 10 MinPerIncr and 2 RowsPerIncr will have 6 cells per hour here.</summary>
		private int[,] _provBar;
		///<summary>Radius of the corner of appts. Subtle.</summary>
		private static float _radiusCorn=2;
		///<summary>Rows per hour.  Derived from 60/MinPerIncr*RowsPerIncr</summary>
		private float _rowsPerHr=6;
		///<summary>This is used to show the provbars for day view, but not show them for week view. It's handy, but not very rigorous.</summary>
		private bool _showProvBars=true;
		///<summary>Starts out zero.  BeginUpdate increments it up, and EndUpdate increments it down. RedrawAsNeeded is blocked unless zero.</summary>
		private int _updatingLevel;
		///<summary>Width of main appt area, not including timebars, provbars, headers, or any other peripheral objects.  Derived from other dimensions when resizing.  Depends on number of providers showing in provbars.  Includes area hidden by horizontal scrolling. Partly determined by control size.</summary>
		private int _widthMain=800;
		///<summary>Width of the main appt area portion that is visible.  Derived from other dimensions when resizing.  Same as _widthMain when there is no horizontal scrolling.  Ignored during printing; just use _widthMain.</summary>
		private int _widthMainVisible=800;
		///<summary>Appointments are a little narrower on the right so that we can see blockouts underneath. Not used in week view.</summary>
		private float _widthNarrowedOnRight=3f;
		///<summary>Width of each operatory column.  Math is based solely on _widthMain and number of ops. Not used for week view.</summary>
		private float _widthOpCol=200f;
		///<summary>Width of each provider bar at the left of the screen, not the provbar on individual appts.  Currently constant at 8f.</summary>
		private float _widthProv=8f;
		///<summary>Width of timebars on the left and right of the main area.</summary>
		private float _widthTime=37f;
		///<summary>Only used with weekview. The width of individual appointments within each day.</summary>
		private float _widthWeekAppt=33f;
		///<summary>The width of an entire day in week view.</summary>
		private float _widthWeekDay=100f;
		#endregion Fields - Private Measurements

		#region Fields - Private Drawing
		///<summary>This bitmap for the info bubble includes text, patient pic, and outline.  There is only one.  It can be quickly placed on top of the background at various locations.  This is not combined with any background bitmap, but is instead painted separately in OnPaint.  It's not filled from RedrawAsNeeded() because its frequency is different, so it's not coupled.  It also has no _isValid flag.</summary>
		private Bitmap _bitmapBubble;
		///<summary>The main area background with appointments.</summary>
		private Bitmap _bitmapMain;
		///<summary>The header that shows operatory colors and info. Only redrawn if _isValidHeaders is false.</summary>
		private Bitmap _bitmapOpsHeader;
		///<summary>Bitmap for ProvBars at the left, plus the Appt info squares on it.</summary>
		private Bitmap _bitmapProvBars;
		///<summary>The prov bars header that shows provider colors and info. Only redrawn if _isValidHeaders is false.</summary>
		private Bitmap _bitmapProvBarsHeader;
		///<summary>The bitmap that shows on TempApptSingle.</summary>
		private Bitmap _bitmapTempApptSingle;
		///<summary>The bitmap for the left timebar. Only redrawn if _isValidProvTimebars is false.</summary>
		private Bitmap _bitmapTimebarLeft;
		///<summary>The bitmap for the right timebar. Only redrawn if _isValidProvTimebars is false.</summary>
		private Bitmap _bitmapTimebarRight;
		//The Brushes, fonts, and pens below will not change once set, and will exist as long as OD is open, so disposing is not needed. Some colors such as prov and blockout are handled locally, since they change frequently
		private Brush _brushBackground=SystemBrushes.Control;
		private Brush _brushBlack=Brushes.Black;
		private Brush _brushBlockText=Brushes.Black;
		private Brush _brushClosed=new SolidBrush(Color.FromArgb(219,219,219));
		private Brush _brushHoliday=new SolidBrush(Color.FromArgb(255,128,128));
		private Brush _brushLinesAndText=Brushes.Black;
		private Brush _brushOpen=Brushes.White;
		private Brush _brushTextBlack=Brushes.Black;
		private Brush _brushTimeBar=Brushes.LightGray;
		private Brush _brushWhite=Brushes.White;
		private Pen _penBlack=Pens.Black;		
		private Pen _penHorizBlack=Pens.Black;
		private Pen _penHorizDark=Pens.DarkSlateGray;
		private Pen _penHorizMinutes=Pens.LightGray;
		///<summary>On the left bar of each appt, the horizontal lines for time divisions</summary>
		private Pen _penTimeDiv=Pens.Silver;
		///<summary>Width gets set below, and color gets reset.</summary>
		private Pen _penTimeLine=new Pen(Color.Red);
		private Pen _penVertPrimary=Pens.DarkGray;
		private Pen _penVertWhite=Pens.White;
		///<summary>This is the main font.  Used for appts, not timebars. Will change, based on user setting.</summary>
		private Font _font=new Font(FontFamily.GenericSansSerif,8f);
		///<summary>This font is used for minutes in the timebars. Will change, based on user setting.</summary>
		private Font _fontMinutes=new Font(FontFamily.GenericSansSerif,8);
		///<summary>This font is used for bold minutes in the timebars. Will change, based on user setting.</summary>
		private Font _fontMinutesBold=new Font(FontFamily.GenericSansSerif,8,FontStyle.Bold);
		///<summary>This font is used in Ops headers, etc.</summary>
		private Font _fontOpsHeaders=new Font(FontFamily.GenericSansSerif,8);
		///<summary>This font is used for ProvBarText.</summary>
		private Font _fontCourier=new Font(FontFamily.GenericMonospace,8f);
		#endregion Fields - Private Drawing

		#region Fields - Private For Properties
		//complex:
		private List<ApptViewItem> _listApptViewItems=null;
		private List<ApptViewItem> _listApptViewItemRowElements=null;
		private static List<Operatory> _listOpsVisible=new List<Operatory>();
		private List<Provider> _listProvsVisible=new List<Provider>();
		private List<Schedule> _listSchedules=new List<Schedule>();
		private DataTable _tableAppointments=new DataTable();
		private DataTable _tableApptFields=new DataTable();
		private DataTable _tableEmpSched=new DataTable();
		private DataTable _tablePatFields=new DataTable();
		private DataTable _tableProvSched=new DataTable();
		private DataTable _tableSchedule=new DataTable();
		private DataTable _tableWaitingRoom=null;//must be null, or it will attempt to FillWaitingRoom on start
		//simple:
		private ApptView _apptViewCur=null;
		private DateTime _dateEnd;
		private DateTime _dateSelected;
		private DateTime _dateStart;
		private bool _isWeeklyView=false;
		private float _minPerIncr=10;
		private long _opNumClicked;
		private float _rowsPerIncr=1;
		private long _selectedAptNum=-1;
		///<summary>Default 8. Set from PrefName.ApptFontSize.  Font sizes are additionally scaled by zoom.</summary>
		private float _sizeFont=8;
		private float _widthProvOnAppt=8;
		#endregion Fields - Private For Properties

		#region Fields - Private Mouse
		///<summary></summary>
		private bool _boolApptMoved;
		///<summary>This is how we drag appts around in this control as well as over to pinboard.  In the beginning, it gets added to parent ContrAppt, where it stays and gets reused repeatedly.  We control it from here to hide the complexity and also to handle the drawing.</summary>
		private ControlDoubleBuffered contrTempAppt;
		///<summary>As we are dragging an appointment, this is the data used to draw it.</summary>
		private DataRow _dataRowTempAppt;
		///<summary>Day of the week clicked when in week view. 0-based index based on days currently showing. Set in mousedown, and used in double click.</summary>
		private int _dayClicked;
		///<summary>When resizing, we need to know the original height.</summary>
		private int _heightResizingOriginal;
		///<summary></summary>
		private bool _isResizingAppt;
		///<summary>Appt origin.  If moving an appointment, this is the location where the appointment was at the beginning of the drag, in coordinates of the parent control.</summary>
		private Point _pointApptOrigin;
		///<summary>The point where the mouse was originally down.  In coordinates of this control.</summary>
		private Point _pointMouseOrigin;
		///<summary>The exact time that user clicked on, not rounded in any way.  Gets set in mousedown, then used in double click as well as external.</summary>
		private TimeSpan _timeClicked;
		#endregion Fields - Private Mouse

		#region Fields - Private Bubble and Tooltip
		///<summary>This has to be tracked globally because mouse might move directly from one appt to another without any break.  This is the only way to know if we are still over the same appt.</summary>
		private int _bubbleApptIdx=-1;
		///<summary>There's a delay for hover effect of 0.28 sec. This is the start time for comparison.</summary>
		private DateTime _dateTimeBubble;
		private bool _isBubbleVisible;
		///<summary>There is a simulated 'tooltip' for the prov and op headers. This is the label on that tooltip.</summary>
		private Label labelHeaderTip;
		///<summary>For the op header tip, this tracks whether we are on the same appt.</summary>
		private long _opNumHeaderTipLast;
		///<summary>This panel is a simulated 'tooltip' for the prov and op headers.</summary>
		private Panel panelHeaderTip;
		///<summary>The position of the pointer the last time the bubble was drawn or the timer was started.  Used to show after timer is up.  In coordinates of this control.</summary>
		private Point _pointBubbleTimer;
		///<summary>This rectangle is in control coordinates and can paint over the top of any other bitmaps all the way up to the edge of this control.</summary>
		private Rectangle _rectangleBubble;
		private Timer timerBubble;
		#endregion Fields - Private Bubble and Tooltip

		#region Fields - Private Dictionaries
		///<summary>Prevents looping through VisOps in order to find the 0-based column index for a given OpNum.</summary>
		private Dictionary<long,int> _dictOpNumToColumnNum=new Dictionary<long, int>();
		
		///<summary>Prevents looping through VisProvs in order to find the 0-based column index for a given ProvNum.</summary>
		private Dictionary<long,int> _dictProvNumToColumnNum=new Dictionary<long, int>();		
		#endregion Fields - Private Dictionaries

		#region Constructor
		public ControlApptPanel() {
			InitializeComponent();
			this.MouseWheel+=ContrApptPanel_MouseWheel;
			//So dummy ops and provs will draw nicely, even if not initialized properly.
			ListOpsVisible.Add(new Operatory());
			ListOpsVisible.Add(new Operatory());
			ListOpsVisible.Add(new Operatory());
			ListProvsVisible.Add(new Provider());
			timerBubble=new Timer();
			timerBubble.Interval = 300;
			timerBubble.Tick += new System.EventHandler(this.timerInfoBubble_Tick);
			panelHeaderTip=new Panel();
			panelHeaderTip.Visible=false;
			panelHeaderTip.Size=new Size(200,17);
			panelHeaderTip.MouseMove+=new MouseEventHandler(panelOpHoverTip_MouseMove);
			panelHeaderTip.BackColor=Color.White;
			panelHeaderTip.BorderStyle=BorderStyle.FixedSingle;
			labelHeaderTip=new Label();
			labelHeaderTip.Location=new Point(0,0);
			labelHeaderTip.Width=300;//arbitrarily long
			panelHeaderTip.Controls.Add(labelHeaderTip);
			Controls.Add(panelHeaderTip);
		}
		#endregion Constructor

		#region Events - Raise
		///<summary>Passes the request up to the parent form, which shows the edit window.</summary>
		private void OnApptDoubleClicked(Appointment appt) {
			ApptDoubleClicked?.Invoke(this,new ApptEventArgs(){Appt=appt });
		}
		[Category("Appointment"),Description("Occurs when an appointment is double clicked.")]
		public event EventHandler<ApptEventArgs> ApptDoubleClicked;
		
		///<summary>Passes the request up to the parent form, which handles the rest, such as adding patient and showing appt edit window.</summary>
		private void OnApptMainAreaDoubleClicked(DateTime dateT,long opNum,Point location) {
			ApptMainAreaDoubleClicked?.Invoke(this,new ApptMainClickEventArgs(){DateT=dateT,OpNum=opNum,Location=location });
		}
		[Category("Appointment"),Description("Occurs when the main blank area is double clicked.")]
		public event EventHandler<ApptMainClickEventArgs> ApptMainAreaDoubleClicked;

		///<summary>Parent window will show context menu.</summary>
		private void OnApptMainAreaRightClicked(DateTime dateT,long opNum,Point location) {
			ApptMainAreaRightClicked?.Invoke(this,new ApptMainClickEventArgs(){DateT=dateT,OpNum=opNum,Location=location });
		}
		[Category("Appointment"),Description("Occurs when the main blank area is right clicked.")]
		public event EventHandler<ApptMainClickEventArgs> ApptMainAreaRightClicked;

		///<summary>Passes the request up to the parent form, which does the db interaction and a refresh.</summary>
		private void OnApptMoved(Appointment appt,Appointment apptOld) {
			ApptMoved?.Invoke(this,new ApptMovedEventArgs(){Appt=appt,ApptOld=apptOld });
		}
		[Category("Appointment"),Description("Occurs when an appointment is moved.")]
		public event EventHandler<ApptMovedEventArgs> ApptMoved;

		///<summary>Passes the datarow up to the parent form, which then passes it to the pinboard.</summary>
		private void OnApptMovedToPinboard(DataRow dataRow) {
			ApptMovedToPinboard?.Invoke(this,new ApptDataRowEventArgs(){DataRowAppt=dataRow });
		}
		[Category("Appointment"),Description("Occurs when an appointment is dragged to pinboard.")]
		public event EventHandler<ApptDataRowEventArgs> ApptMovedToPinboard;

		///<summary>If we find an appointment that is null, then raise this event so that the parent form will show message and refresh.</summary>
		private void OnApptNullFound() {
			ApptNullFound?.Invoke(this,new EventArgs());
		}
		[Category("Appointment"),Description("Occurs when a null appointment is found.")]
		public event EventHandler ApptNullFound;

		///<summary>Parent window will refresh.</summary>
		private void OnApptResized(Appointment appt) {
			ApptResized?.Invoke(this,new ApptEventArgs(){Appt=appt });
		}
		[Category("Appointment"),Description("Occurs when an appointment is resized.")]
		public event EventHandler<ApptEventArgs> ApptResized;

		///<summary>Parent window will show context menu.</summary>
		private void OnApptRightClicked(Point location) {
			ApptRightClicked?.Invoke(this,new ApptRightClickEventArgs(){Location=location });
		}
		[Category("Appointment"),Description("Occurs when an appointment is right clicked.")]
		public event EventHandler<ApptRightClickEventArgs> ApptRightClicked;

		///<summary>Parent window will handle a refresh.</summary>
		private void OnDateChanged() {
			DateChanged?.Invoke(this,new EventArgs());
		}
		[Category("Appointment"),Description("Occurs when date is changed by the user clicking on op header.")]
		public event EventHandler DateChanged;

		///<summary>Occurs when user clicks to change selected appt.  Changing SelectedAptNum externally does not cause this event to fire. Any of the 3 parameters can be -1 to indicate none. Also fires when clicking on the appt that's already selected.</summary>
		private void OnSelectedApptChanged(long patNumNew,long aptNumOld,long aptNumNew) {
			SelectedApptChanged?.Invoke(this,new ApptSelectedChangedEventArgs(){PatNumNew=patNumNew,AptNumOld=aptNumOld,AptNumNew=aptNumNew });
		}
		[Category("Appointment"),Description("Occurs when user clicks to change selected appt.  Changing SelectedAptNum externally does not cause this event to fire. Any of the 3 parameters can be -1 to indicate none.")]
		public event EventHandler<ApptSelectedChangedEventArgs> SelectedApptChanged;
		#endregion Events - Raise

		#region Properties - Complex Data (tables and lists)
		///<summary>A list of the ApptViewItems for the current view.  Ignore ApptViewItemL.ForCurView.  Only use this one.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<ApptViewItem> ListApptViewItems{
			get{
				return _listApptViewItems;
			}
			set{
				_listApptViewItems=value;
				RedrawAsNeeded();
			}
		}		
		
		///<summary>Subset of ListApptViewItems. Just items for rowElements, including apptfielddefs. If no view is selected, then the elements are filled with default info.  Ignore ApptViewItemL.ApptRows.  Only use this one.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<ApptViewItem> ListApptViewItemRowElements{
			get{
				return _listApptViewItemRowElements;
			}
			set{
				_listApptViewItemRowElements=value;
				RedrawAsNeeded();
			}
		}
		
		///<summary>Subset of all ops.  Can't include a hidden op in this list.  If user has set View.OnlyScheduledProvs, and not isWeekly, then the only VisOps will be for providers that have schedules for the day and ops with no provs assigned.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<Operatory> ListOpsVisible {
			get{ return _listOpsVisible; }
			set{
				_listOpsVisible=value;
				_dictOpNumToColumnNum=_listOpsVisible.ToDictionary(x => x.OperatoryNum,x => _listOpsVisible.FindIndex(y => y.OperatoryNum==x.OperatoryNum));
				_isValidMain=false;
				_isValidHeaders=false;
				RedrawAsNeeded();
			}
		}	
		
		///<summary>Was VisProvs.  Visible provider bars in appt module.  Subset of all provs.  Can't include a hidden prov in this list.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<Provider> ListProvsVisible {
			get{ return _listProvsVisible; }
			set{
				_listProvsVisible=value;
				_dictProvNumToColumnNum=_listProvsVisible.ToDictionary(x => x.ProvNum,x => _listProvsVisible.FindIndex(y => y.ProvNum==x.ProvNum));
				if(_showProvBars){//this is also present in IsWeeklyView.set and LayoutRecalcAfterResize()
					_widthMainVisible=this.Width-(int)_widthTime*2-(int)(_widthProv*_listProvsVisible.Count)-vScrollBar1.Width-1;
					hScrollBar1.Left=(int)(_widthTime+_widthProv*_listProvsVisible.Count)+1;
				}
				else{
					_widthMainVisible=this.Width-(int)_widthTime*2-vScrollBar1.Width-1;
					hScrollBar1.Left=(int)_widthTime+1;
				}
				hScrollBar1.Width=_widthMainVisible;
				_isValidMain=false;
				_isValidHeaders=false;
				RedrawAsNeeded();
			}
		}	
		
		///<summary>Was previously called SchedListPeriod.  It contains the background schedule for the entire period.  Includes all types.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
		public List<Schedule> ListSchedules{
			get{
				return _listSchedules;
			}
			//set from TableSchedule property
		}

		///<summary>The appointment table. Refreshed in RefreshAppointmentsIfNeeded.</summary>
		[Browsable(false)]
		public DataTable TableAppointments {
			get{ return _tableAppointments; }
			set{
				_tableAppointments=value;
				_isValidMain=false;
				_bubbleApptIdx=-1;
				RedrawAsNeeded();
			}
		}
		
		///<summary>The appointment fields table. Refreshed in RefreshAppointmentsIfNeeded.  Only includes entries for appointments that were present in TableAppointments.</summary>
		[Browsable(false)]
		public DataTable TableApptFields {
			get{ return _tableApptFields; }
			set{
				_tableApptFields=value;
				_isValidMain=false;
				RedrawAsNeeded();
			}
		}

		///<summary>The employee schedule table. Refreshed in RefreshSchedulesIfNeeded.</summary>
		[Browsable(false)]
		public DataTable TableEmpSched {
			get{ return _tableEmpSched; }
			set{
				_tableEmpSched=value;
				_isValidMain=false;
				RedrawAsNeeded();
			}
		}

		///<summary>The patient fields table. Refreshed in RefreshAppointmentsIfNeeded.</summary>
		[Browsable(false)]
		public DataTable TablePatFields {
			get{ return _tablePatFields; }
			set{
				_tablePatFields=value;
				_isValidMain=false;
				RedrawAsNeeded();
			}
		}

		///<summary>The provider schedule table. Refreshed in RefreshSchedulesIfNeeded.</summary>
		[Browsable(false)]
		public DataTable TableProvSched {
			get{ return _tableProvSched; }
			set{
				_tableProvSched=value;
				_isValidMain=false;
				RedrawAsNeeded();
			}
		}

		///<summary>The schedule table. Refreshed in RefreshSchedulesIfNeeded.</summary>
		[Browsable(false)]
		public DataTable TableSchedule {
			get{ return _tableSchedule; }
			set{
				_tableSchedule=value;
				_listSchedules=Schedules.ConvertTableToList(_tableSchedule);
				_isValidMain=false;
				RedrawAsNeeded();
			}
		}

		///<summary>The waiting room table. Refreshed in RefreshWaitingRoomTable.</summary>
		[Browsable(false)]
		public DataTable TableWaitingRoom {
			get{ return _tableWaitingRoom; }
			set{
				_tableWaitingRoom=value;
			}
		}
		#endregion Properties - Complex Data

		#region Properties - Simple Data
		///<summary>Frequently null to represent no view selected.</summary>
		[Browsable(false)]
		[DefaultValue(null)]
		public ApptView ApptViewCur{
			get{
				return _apptViewCur;
			}
			set{
				_apptViewCur=value;
				//no need to set valid flags or redraw because listOpsVisible etc will get set.
			}
		}

		///<summary>Computed from DateSelected and IsWeeklyView. If weekly view, this is the second date.  If daily view, this is the same as dateSelected.</summary>
		[Browsable(false)]
		public DateTime DateEnd{
			get{
				return _dateEnd;
			}
		}

		///<summary>The date currently selected in the appointment module.  If switching to week view, this date does not change.  That way, it is remembered when switching back to day view.</summary>
		[Browsable(false)]
		public DateTime DateSelected{
			get{
				return _dateSelected;
			}
			set{
				//only allow dates, no times
				DateTime date=value.Date;
				if(date==_dateSelected){
					return;
				}
				_dateSelected=date;
				if(IsWeeklyView){
					//user can click on a new date, which selects a new week.
					//There is a rare case where, in week view, you can select a different date within the existing week.
					//We don't technically need to set _isValidAll to false, but it's rare and harmless.
					if(_dateSelected.DayOfWeek==DayOfWeek.Sunday) {
						_dateStart=_dateSelected.AddDays(-6).Date;//go back to previous monday
					}
					else {
						_dateStart=_dateSelected.AddDays(1-(int)_dateSelected.DayOfWeek).Date;//go back to current monday
					}
					_dateEnd=_dateStart.AddDays(_numOfWeekDaysToDisplay-1).Date;
				}
				else {
					_dateStart=_dateSelected;
					_dateEnd=_dateSelected;
				}
				_isValidMain=false;
				//I don't think headers would change
				RedrawAsNeeded();
			}
		}

		///<summary>Computed from DateSelected and IsWeeklyView. If weekly view, this is the first date (old WeekStartDate).  If daily view, this is the same as dateSelected.</summary>
		[Browsable(false)]
		public DateTime DateStart{
			get{
				return _dateStart;
			}
		}

		[Browsable(false)]
		///<summary>The dateTime that the user clicked, set in mousedown, and used in double click.</summary>
		public DateTime DateTimeClicked{
			get{
				return _dateSelected+RoundTimeDown(_timeClicked,MinPerIncr);
			}
		}

		///<summary>WeekStartDate and WeekEndDate were also in ContrAppt, but aren't needed.</summary>
		[Browsable(false)]
		public bool IsWeeklyView{
			get{
				return _isWeeklyView;
			}
			set{
				if(value==_isWeeklyView){
					return;
				}
				_isWeeklyView=value;
				//the logic below was formerly in ContrAppt.SetWeekDates
				if(_isWeeklyView){//if changing to weekly view
					if(DateSelected.DayOfWeek==DayOfWeek.Sunday) {
						_dateStart=DateSelected.AddDays(-6).Date;//go back to previous monday
					}
					else {
						_dateStart=DateSelected.AddDays(1-(int)DateSelected.DayOfWeek).Date;//go back to current monday
					}
					_dateEnd=_dateStart.AddDays(_numOfWeekDaysToDisplay-1).Date;
					_showProvBars=false;
				}
				else {//changing to daily view
					_dateStart=DateSelected;
					_dateEnd=DateSelected;
					_showProvBars=true;
				}
				if(_showProvBars){//this is also present in ListProvsVisible.set and LayoutRecalcAfterResize()
					_widthMainVisible=this.Width-(int)_widthTime*2-(int)(_widthProv*_listProvsVisible.Count)-vScrollBar1.Width-1;
					hScrollBar1.Left=(int)(_widthTime+_widthProv*_listProvsVisible.Count)+1;
				}
				else{
					_widthMainVisible=this.Width-(int)_widthTime*2-vScrollBar1.Width-1;
					hScrollBar1.Left=(int)_widthTime+1;
				}
				hScrollBar1.Width=_widthMainVisible;
				_isValidMain=false;
				_isValidHeaders=false;
				RedrawAsNeeded();
			}
		}
		
		///<summary>Pulled from Prefs AppointmentTimeIncrement.  Either 5, 10, or 15. An increment can be one or more rows.</summary>
		[Browsable(false)]
		[DefaultValue(10f)]
		public float MinPerIncr{
			get{
				return _minPerIncr;
			}
			set{
				if(value==_minPerIncr){
					return;
				}
				TimeSpan timeScrollOriginal=TimeSpan.FromHours(vScrollBar1.Value/_heightLine/_rowsPerHr);//also in RowsPerIncr
				_minPerIncr=value;
				_minPerRow=_minPerIncr/RowsPerIncr;
				_rowsPerHr=60f/_minPerIncr*RowsPerIncr;
				ComputeHeight();//needed to calc new scroll val
				SetScrollByTime(timeScrollOriginal);
				_isValidMain=false;
				_isValidTimebars=false;
				RedrawAsNeeded();
			}
		}

		[Browsable(false)]
		[DefaultValue(0)]
		///<summary>The opNum of the clicked op, set in mousedown, and used in double click.</summary>
		public long OpNumClicked{
			get{
				return _opNumClicked;
			}
		}

		///<summary>Based on the view.  If no view, then it is set to 1. Different computers can be showing different views.</summary>
		[Browsable(false)]
		[DefaultValue(1f)]
		public float RowsPerIncr{
			get{
				return _rowsPerIncr;
			}
			set{
				if(value==_rowsPerIncr){
					return;
				}
				//When we change this, we want to keep the scrolltime the same instead of bouncing around randomly.  Also in MinPerIncr
				TimeSpan timeScrollOriginal=TimeSpan.FromHours(vScrollBar1.Value/_heightLine/_rowsPerHr);
				//int h=_heightMain-_heightMainVisible+vScrollBar1.LargeChange;		
				//int j=(int)(_lineH*24*_rowsPerHr)+1;
				_rowsPerIncr=value;
				_minPerRow=MinPerIncr/_rowsPerIncr;
				_rowsPerHr=60f/MinPerIncr*_rowsPerIncr;
				ComputeHeight();//needed to calc new scroll val
				SetScrollByTime(timeScrollOriginal);
				_isValidMain=false;//will harmlessley recompute height
				_isValidTimebars=false;
				RedrawAsNeeded();
			}
		}

		///<summary>-1 means none.  Selected appt will have thick outline.</summary>
		[Browsable(false)]
		[DefaultValue((long)-1)]
		public long SelectedAptNum{
			get{
				return _selectedAptNum;
			}
			set{
				if(value==_selectedAptNum){
					return;
				}
				_selectedAptNum=value;
				Invalidate();
			}
		}

		///<summary>Set externally from PrefName.ApptFontSize</summary>
		[Browsable(false)]
		[DefaultValue(8f)]
		public float SizeFont{
			set{
				//if(_sizeFont==value){//can't do this. Even at 8, we still need to apply zoom.
				//	return;
				//}
				//When we change this, we want to keep the scrolltime the same instead of bouncing around randomly.  Also in MinPerIncr and RowsPerIncr
				TimeSpan timeScrollOriginal=TimeSpan.FromHours(vScrollBar1.Value/_heightLine/_rowsPerHr);
				//int h=_heightMain-_heightMainVisible+vScrollBar1.LargeChange;		
				//int j=(int)(_lineH*24*_rowsPerHr)+1;
				_sizeFont=value;
				_font?.Dispose();
				_fontCourier?.Dispose();
				_fontMinutes?.Dispose();
				_fontMinutesBold?.Dispose();
				_fontOpsHeaders?.Dispose();
				_font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(_sizeFont));
				_fontCourier=new Font(FontFamily.GenericMonospace,LayoutManager.ScaleF(_sizeFont));
				_fontMinutes=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(_sizeFont));
				_fontMinutesBold=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(_sizeFont),FontStyle.Bold);
				_fontOpsHeaders=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(_sizeFont));
				//using(Graphics g=this.CreateGraphics()){
				//	_heightLine=g.MeasureString("X",_font,100,StringFormat.GenericTypographic).Height;//more accurate, but makes horizontal lines too distracting
				//}
				_heightLine=_font.Height;
				LayoutRecalcAfterResize();
				ComputeHeight();//needed to calc new scroll val
				SetScrollByTime(timeScrollOriginal);
				_isValidMain=false;//will harmlessley recompute height
				_isValidTimebars=false;
				RedrawAsNeeded();
			}
		}

		///<summary>Width of the provider bar on the left side of each appt.  Gets set from PrefName.ApptProvbarWidth.  Zoom is applied immediately.</summary>
		[Browsable(false)]
		[DefaultValue(8f)]
		public float WidthProvOnAppt{
			get{
				return _widthProvOnAppt;
			}
			set{
				if(LayoutManager.ScaleF(value)==_widthProvOnAppt){
					return;
				}
				_widthProvOnAppt=LayoutManager.ScaleF(value);
				_isValidMain=false;
				RedrawAsNeeded();
			}
		}
		#endregion Properties - Simple Data

		#region Methods - Event Handlers Standard
		private void ContrApptPanel_Load(object sender,EventArgs e) {
			_isControlLoaded=true;
			LayoutRecalcAfterResize();
			contrTempAppt=new ControlDoubleBuffered();
			contrTempAppt.Name="contrTempAppt";
			contrTempAppt.Visible=false;
			contrTempAppt.Size=new Size(100,100);
			contrTempAppt.Paint+=TempApptSingle_Paint;
			if(!DesignMode){
				LayoutManager.Add(contrTempAppt,Parent);
			}
		}

		private void ContrApptPanel_MouseWheel(object sender,MouseEventArgs e) {
			//this behavior changed in Win10 because the "Scroll inactive windows when I hover over them" option is turned by default. 
			//This means we don't have to have focus to scroll.  Nice.
			//e.delta value is pretty useless.  So we are only going to test whether it's scrolling up or down, 
			if(!vScrollBar1.Enabled){
				return;
			}
			bool isNeg=e.Delta<0;
			int valueNew=vScrollBar1.Value;
			if(isNeg){
				valueNew+=vScrollBar1.SmallChange;
			}
			else{
				valueNew-=vScrollBar1.SmallChange;
			}
			if(valueNew<vScrollBar1.Minimum){
				valueNew=vScrollBar1.Minimum;
			}
			if(valueNew>vScrollBar1.Maximum-vScrollBar1.LargeChange+1){
				valueNew=vScrollBar1.Maximum-vScrollBar1.LargeChange+1;
			}
			vScrollBar1.Value=valueNew;
			Invalidate();
		}

		private void ContrApptPanel_Resize(object sender,EventArgs e) {
			//seems to resize about 13 times before it's even loaded. Doing it this way causes just a single layout.
			if(!_isControlLoaded){
				return;
			}
			LayoutRecalcAfterResize();
		}

		private void LayoutRecalcAfterResize(){
			_widthTime=LayoutManager.ScaleF(37f)*_sizeFont/8f;
			_widthProv=LayoutManager.Scale(8);
			//SizeFont
			vScrollBar1.Width=LayoutManager.Scale(17);
			if(_showProvBars){//this is also present in ListProvsVisible.set and IsWeeklyView.set
				_widthMainVisible=this.Width-(int)_widthTime*2-(int)(_widthProv*_listProvsVisible.Count)-vScrollBar1.Width-1;
			}
			else{
				_widthMainVisible=this.Width-(int)_widthTime*2-vScrollBar1.Width-1;
			}
			hScrollBar1.Width=_widthMainVisible;
			hScrollBar1.Height=LayoutManager.Scale(17);
			//_widthMain is set in RedrawAsNeeded=>ComputeColWidth
			_heightMainVisible=this.Height-(int)_heightProvOpHeaders-hScrollBar1.Height-1;
			vScrollBar1.Location=new Point(this.Width-vScrollBar1.Width-1,(int)_heightProvOpHeaders);
			vScrollBar1.Height=_heightMainVisible;	
			if(_showProvBars){
				hScrollBar1.Location=new Point((int)(_widthTime+_widthProv*_listProvsVisible.Count)+1,this.Height-hScrollBar1.Height-1);
			}
			else{
				hScrollBar1.Location=new Point((int)_widthTime+1,this.Height-hScrollBar1.Height-1);
			}
			_isValidMain=false;
			_isValidHeaders=false;
			_bubbleApptIdx=-1;//trigger bubble to draw again in case we changed zoom
			_rectangleBubble=new Rectangle();//trigger recalc of rect
			//_isValidTimebars=;//timebars don't need to redraw on resize
			RedrawAsNeeded();
		}

		private void HScrollBar1_Scroll(object sender, ScrollEventArgs e){
			Invalidate();
		}

		private void vScrollBar1_Scroll(object sender,ScrollEventArgs e) {
			Invalidate();
		}
		#endregion Methods - Event Handlers Standard

		#region Methods - Event Handlers Mouse
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			HideDraggableTempApptSingle();
			//this logic is a little different than mouse down for now because on the first click of a 
			//double click, an appointment control is created under the mouse.
			if(SelectedAptNum>0) {//on appt
				//on mousedown, we didn't check this
				Appointment appt=Appointments.GetOneApt(SelectedAptNum);
				if(appt==null) {
					OnApptNullFound();
					return;
				}
				OnApptDoubleClicked(appt);//event raised to parent.
			}
			//not on apt, so trying to schedule an appointment---------------------------------------------------------------------
			else {
				if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
					return;
				}
				if(ListOpsVisible.Count==0) {//no ops visible.
					return;
				}
				//First we'll check to see if the location the user clicked on is a blockout with the type NoSchedule
				//DateTime dateSelected=_dateSelected;
				if(IsWeeklyView) {
					//we might even want to set this on single click, but double is a start
					_dateSelected=DateStart.AddDays(_dayClicked);
				}
				TimeSpan timeSpanNew=RoundTimeDown(_timeClicked,MinPerIncr);
				DateTime dateTimeStart=DateSelected+timeSpanNew;
				if(Appointments.CheckTimeForBlockoutOverlap(dateTimeStart,_opNumClicked)) {
					MsgBox.Show(this, "Appointment cannot be created on a blockout marked as 'Block appointment scheduling'");
					return;
				}
				OnApptMainAreaDoubleClicked(dateTimeStart,_opNumClicked,_pointMouseOrigin);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(Plugins.HookMethod(this,"ContrApptPanel_OnMouseDown_start",e)) {
				return;
			}
			if(ListOpsVisible.Count==0) {//no ops visible.
				return;
			}
			//if(_isMouseDown) {
			if(((Control.MouseButtons & MouseButtons.Left)>0 && (Control.MouseButtons & MouseButtons.Right)>0)) {
				return;
			}
			if(((Control.MouseButtons & MouseButtons.Left)>0 && (Control.MouseButtons & MouseButtons.Middle)>0)) {
				return;
			}
			if(e.Y<_heightProvOpHeaders
				&& e.X>_widthTime 
				&& e.X<_widthTime+_widthProv*_listProvsVisible.Count)
			{
				//clicked in prov header.  Do nothing.
			}
			else if(e.Y<_heightProvOpHeaders
				&& e.X>_widthTime 
				&& e.X<this.Width-vScrollBar1.Width-_widthTime)
			{//clicked in operatory or weekday header
				#region Click Op Header
				if(IsWeeklyView){
					int dayI=XPosToDay(e.X);
					DateSelected=DateStart.AddDays(dayI);
					IsWeeklyView=false;
					OnDateChanged();
					return;
				}
				string txt="";
				for(int i=0;i<ListOpsVisible.Count;i++){
					if(i==ListOpsVisible.Count-1
						|| e.X<_widthTime+_widthProv*_listProvsVisible.Count+_widthOpCol*(i+1))//compare to the righthand side of op
					{
						//found op where we clicked
						txt=ListOpsVisible[i].OpName;
						//_opNumHeaderTipLast=ListOpsVisible[i].OperatoryNum;//not needed here.  Handled in mouseMove
						Provider prov=Providers.GetProv(ListOpsVisible[i].ProvDentist);//We can enhance this later to show Hygienist.
						if(prov==null){
							txt+="\r\n"+Lan.g(this,"There is no provider associated with this operatory.");
						}
						else{
							Def defProvSpecialty=Defs.GetDef(DefCat.ProviderSpecialties,prov.Specialty);
							if(defProvSpecialty!=null) {
								txt+="\r\n"+Lan.g(this,"Default: ")+prov.FName+" "+prov.LName+", "+defProvSpecialty.ItemName;
							}
						}
						//Get the subset of schedules from ListSchedules that is linked to this op
						List<Schedule> listScheds=Schedules.GetProvSchedsForOp(ListSchedules,ListOpsVisible[i]);
						Provider providerScheduled;
						//Get the provider for each Schedule and show that provider's info.
						foreach(Schedule schedule in listScheds) {
							providerScheduled=Providers.GetProv(schedule.ProvNum);
							if(providerScheduled==null) {
								return;
							}
							Def defScheduledProv=Defs.GetDef(DefCat.ProviderSpecialties,providerScheduled.Specialty);
							string schedProcDef="";
							if(defScheduledProv!=null) {
								schedProcDef=", "+defScheduledProv.ItemName;
							}
							txt+="\r\n"+Lan.g(this,"Scheduled ")+schedule.StartTime.ToShortTimeString()+"-"+schedule.StopTime.ToShortTimeString()+": "
								+providerScheduled.GetFormalName()+schedProcDef;
							txt+=string.IsNullOrWhiteSpace(providerScheduled.SchedNote) ? "": "\r\n\t("+providerScheduled.SchedNote+")";
						}
						if(PrefC.HasClinicsEnabled && Clinics.ClinicNum!=0) {//HQ cannot have clinic specialties.
							Clinic clinic=Clinics.GetClinic(Clinics.ClinicNum);
							string clinicSchedNote=clinic.SchedNote;
							if(!string.IsNullOrEmpty(clinicSchedNote)) {
								if(txt!="") {
									txt+="\r\n------";
								}
								List<DefLink> listSpecialties=DefLinks.GetListByFKey(clinic.ClinicNum,DefLinkType.Clinic);
								txt+="\r\n"+string.Join(", ",listSpecialties.Select(x => Defs.GetName(DefCat.ClinicSpecialty,x.DefNum)))
									+"\r\n"+clinicSchedNote;
							}
						}
						break;
					}
				}
				panelHeaderTip.Width=TextRenderer.MeasureText(txt,_font).Width+2;
				panelHeaderTip.Height=TextRenderer.MeasureText(txt,_font).Height+3;
				labelHeaderTip.Width=panelHeaderTip.Width;
				labelHeaderTip.Height=panelHeaderTip.Height;//auto?
				labelHeaderTip.Text=txt;
				int xval=e.X;
				int yval=e.Y+20;
				if(xval+panelHeaderTip.Width>this.Width-vScrollBar1.Width-3){
					xval=this.Width-panelHeaderTip.Width-vScrollBar1.Width-3;
				}
				panelHeaderTip.Location=new Point(xval,yval);
				panelHeaderTip.Visible=true;
				#endregion Click Op Header
			}
			TimeSpan? timeClicked=YPosToTime(e.Y);
			if(timeClicked==null){//clicked outside the main bitmap
				return;//?
			}
			_pointMouseOrigin=e.Location;
			_timeClicked=timeClicked.Value;
			//MessageBox.Show(_timeClicked.ToShortTimeString());
			int idxAppt=HitTestAppt(e.Location);
			DataRow dataRow=null;
			if(idxAppt!=-1){
				dataRow=TableAppointments.Rows[idxAppt];
			}
			_opNumClicked=ListOpsVisible[XPosToOpIdx(e.X)].OperatoryNum;
			_dayClicked=XPosToDay(e.X);
			long clickedAptNum=-1;
			if(dataRow!=null){
				clickedAptNum=PIn.Long(dataRow["AptNum"].ToString());
			}
			if(IsWeeklyView) {
				DateSelected=DateStart.AddDays(_dayClicked);
			}
			DateTime dateTimeClicked=DateSelected+_timeClicked;
			long aptNumOld=SelectedAptNum;
			if(clickedAptNum>0) {//if clicked on an appt
				long patNumNew=PIn.Long(dataRow["PatNum"].ToString());
				if(e.Button==MouseButtons.Right) {
					SelectedAptNum=clickedAptNum;
					OnSelectedApptChanged(patNumNew,aptNumOld,SelectedAptNum);//this must be raised prior to OnApptRightClicked in order for patient to change.
					OnApptRightClicked(e.Location);
				}
				else{//left button
					if(HitTestApptBottom(e.Location,idxAppt)) {
						_isResizingAppt=true;
					}
					else {
						_isResizingAppt=false;
					}
					ShowDraggableApptSingle(idxAppt);//this sets _isMouseDown
					BubbleDraw(e.Location,idxAppt);//once mouse is down, this will hide the appt bubble. 
					_pointApptOrigin=contrTempAppt.Location;
					if(_isResizingAppt){
						_heightResizingOriginal=contrTempAppt.Height;
					}
					SelectedAptNum=clickedAptNum;
					OnSelectedApptChanged(patNumNew,aptNumOld,SelectedAptNum);
					//This must be raised after ShowDraggableContrApptSingle due to S_Contr_PatientSelected(...) indirectly stopping the drag if there are any popups.
				}
			}
			else {//clicked on empty space, not appt
				SelectedAptNum=clickedAptNum;//-1
				OnSelectedApptChanged(-1,aptNumOld,-1);
				if(e.Button==MouseButtons.Right){
					OnApptMainAreaRightClicked(dateTimeClicked,_opNumClicked,e.Location);
				}
			}
		}

		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			HeaderTipDraw(new Point(-1,-1));//to trigger hiding it
			_isBubbleVisible=false;
			timerBubble.Enabled=false;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			//if(!_isMouseDown) {
			if(Control.MouseButtons==MouseButtons.None) { 
				if(contrTempAppt.Visible) {//mouse was down when some other program stole focus, leaving orphaned appt.
					contrTempAppt.Visible=false;
					Invalidate();
				}
				int idxAppt=HitTestAppt(e.Location);
				BubbleDraw(e.Location,idxAppt);//could be -1 to not show
				HeaderTipDraw(e.Location);
				//decide what the pointer should look like.
				if(HitTestApptBottom(e.Location,idxAppt)) {
					Cursor=Cursors.SizeNS;
				}
				else {
					Cursor=Cursors.Default;
				}
				return;
			}
			if(Control.MouseButtons!=MouseButtons.Left) { 
				return;
			}
			//from here down, left mouse button is down
			if(SelectedAptNum<0) {
				return;
			}
			if(!this.Focused) {
				//This can happen if user clicks the view dropdown, then clicks in controlApptPanel.  A phantom appt shows up, then goes away.
				//MouseMove fires when mouse down because the combobox dropdown had focus.
				return;
			}
			//if resizing an appointment
			if(_isResizingAppt) {
				//already set in contrDraggableAppt in mousedown.  Just fiddle with height
				contrTempAppt.Visible=true;//show visible with even the slightest movement.
				LayoutManager.MoveHeight(contrTempAppt,_heightResizingOriginal+e.Y-_pointMouseOrigin.Y);
				if(contrTempAppt.Height<4) {//too small
					LayoutManager.MoveHeight(contrTempAppt,4);
				}
				//SetBitmapTempAppt();//no, only do this on mouse down
				contrTempAppt.Invalidate();
				return;
			}
			//Moving appt
			//int thisIndex=GetIndex(ContrApptSingle.SelectedAptNum);
			if((Math.Abs(e.X-_pointMouseOrigin.X)<3)//enhances double clicking
				&&(Math.Abs(e.Y-_pointMouseOrigin.Y)<3)) {
				_boolApptMoved=false;
				return;
			}
			_boolApptMoved=true;
			//since this usercontrol belongs to the parent, coordinates are in ContrAppt frame.
			LayoutManager.MoveLocation(contrTempAppt,new Point(
				_pointApptOrigin.X+e.X-_pointMouseOrigin.X,
				_pointApptOrigin.Y+e.Y-_pointMouseOrigin.Y));
			contrTempAppt.Visible=true;
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			//If both buttons are used in some combination, we only get a mouse up event on the first button released.
			//So we treat either of them as the mouse up event.
			//if(!_isMouseDown) {
			//	return;
			//}
			//try/finally only. No catch. We probably want a handy exception to popup if there is a real bug.
			//Any return from this point forward will cause HideDraggableContrApptSingle() and ResizingAppt=false.
			try {
				//int thisIndex=GetIndex(ContrApptSingle.SelectedAptNum);
				Appointment apptOld;
				if(_isResizingAppt) {
					#region Resizing, not moving.
					//if(!TempApptSingle.Visible) {//click with no drag?
					//	return;
					//}
					//convert Bottom to a time
					TimeSpan? timeSpanBottom=YPosToTime(contrTempAppt.Bottom-this.Location.Y);
					if(timeSpanBottom==null){
						return;
					}
					TimeSpan timeSpanBottomRounded=RoundTimeToNearestIncrement(timeSpanBottom.Value,MinPerIncr);					
					//subtract to get the new length of appt
					DateTime dateTimeTempAppt=PIn.DateT(_dataRowTempAppt["AptDateTime"].ToString());
					TimeSpan newspan=timeSpanBottomRounded-dateTimeTempAppt.TimeOfDay;
					//check if the appointment is being dragged to the next day
					if(dateTimeTempAppt.Day!=(dateTimeTempAppt+newspan).Day) {//I don't think this can get hit
						MsgBox.Show(this,"You cannot have an appointment that starts and ends on different days.");
						return;
					}
					int newpatternL=Math.Max((int)newspan.TotalMinutes/5,1);
					if(newpatternL<MinPerIncr/5) {//eg. if 1 < 10/5, would make appt too short. 
						newpatternL=(int)MinPerIncr/5;//sets new pattern length at one increment, typically 2 or 3 5min blocks
					}
					else if(newpatternL>78) {//max length of 390 minutes. Not sure why.  I want to increase this to 9 hours.
						newpatternL=78;
					}
					string pattern=PIn.String(_dataRowTempAppt["Pattern"].ToString());
					if(newpatternL<pattern.Length) {//shorten to match new pattern length
						pattern=pattern.Substring(0,newpatternL);
					}
					else if(newpatternL>pattern.Length) {//make pattern longer.
						pattern=pattern.PadRight(newpatternL,'/');
					}
					//Now, check for overlap with other appts.
					//Loop through all other appts in the op and make sure the new pattern will not overlap.
					long aptNumTemp=PIn.Long(_dataRowTempAppt["AptNum"].ToString());
					if(!PrefC.GetYN(PrefName.ApptsAllowOverlap)){
						long opNumTemp=PIn.Long(_dataRowTempAppt["Op"].ToString());
						for(int i=0;i<TableAppointments.Rows.Count;i++){
							if(PIn.Long(TableAppointments.Rows[i]["Op"].ToString())!=opNumTemp){
								continue;
							}
							if(PIn.Long(TableAppointments.Rows[i]["AptNum"].ToString())==aptNumTemp){
								continue;
							}
							DateTime dateTimeApptInSameOp=PIn.DateT(TableAppointments.Rows[i]["AptDateTime"].ToString());
							if(IsWeeklyView && dateTimeApptInSameOp.Date!=dateTimeTempAppt.Date) {
								continue;
							}
							if(dateTimeApptInSameOp < dateTimeTempAppt) {
								continue;//we don't care about appointments that are earlier than this one
							}
							if(dateTimeApptInSameOp.TimeOfDay < dateTimeTempAppt.TimeOfDay+TimeSpan.FromMinutes(5*pattern.Length)) {
								//New pattern overlaps so back it up to butt up against this appt. This will shorten the desired pattern to prevent overlap.
								newspan=dateTimeApptInSameOp.TimeOfDay-dateTimeTempAppt.TimeOfDay;
								newpatternL=Math.Max((int)newspan.TotalMinutes/5,1);
								pattern=pattern.Substring(0,newpatternL);						
							}
						}
					}
					//Check for any overlap with blockouts with the "No Schedule" flag and shorten the pattern
					Appointment curApt=Appointments.GetOneApt(aptNumTemp);
					if(curApt==null){
						OnApptNullFound();
						return;
					}
					curApt.Pattern=pattern;
					DateTime datePrevious=curApt.DateTStamp;
					Schedule overlappingBlockout=Appointments.GetOverlappingBlockouts(curApt).OrderBy(x => x.StartTime).FirstOrDefault();
					if(overlappingBlockout!=null) {
						//Figure out the amount of time between them and divide by 5 because of how time patterns are stored for appointments.  
						//This happens when resizing.
						//Appointments can be in the middle of a blocking blockout if the blockout type is changed after the appointment is placed.  
						//In this case we are going to leave the appointment because it has precedence.
						if(!(overlappingBlockout.SchedDate.Add(overlappingBlockout.StartTime)<=curApt.AptDateTime)) {
							newpatternL=(int)(overlappingBlockout.SchedDate.Add(overlappingBlockout.StartTime)-curApt.AptDateTime).TotalMinutes/5;
							pattern=pattern.Substring(0,newpatternL);	//Minimum newpatternL is 1, because an appointment can't be places on a valid blockout
						}
					}
					if(pattern=="") {
						pattern="///";
					}
					Appointments.SetPattern(curApt,pattern); //Appointments S-Class handles Signalods
					SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,curApt.PatNum,Lan.g(this,"Appointment resized from the appointment module."), curApt.AptNum,datePrevious);//Generate FKey to the appointment to show the audit entry in the ApptEdit window.
					OnApptResized(curApt);
					#endregion Resizing, not moving.
					return;					
				}//end if(_isResizingAppt)
				if((Math.Abs(e.X-_pointMouseOrigin.X)<7) && (Math.Abs(e.Y-_pointMouseOrigin.Y)<7)) { //Mouse has not moved enough to be considered an appt move.
					_boolApptMoved=false;
				}
				if(!_boolApptMoved) { //Click with no drag.
					return;
				}
				if(contrTempAppt.Location.X>this.Right) { //Dragging to pinboard, so place a copy there.
					#region Dragging to pinboard
					long patNum=PIn.Long(_dataRowTempAppt["PatNum"].ToString());
					if(!Security.IsAuthorized(Permissions.AppointmentMove) || PatRestrictionL.IsRestricted(patNum,PatRestrict.ApptSchedule)){
						return;
					}
					if((ApptStatus)PIn.Int(_dataRowTempAppt["AptStatus"].ToString())==ApptStatus.Complete) {//could cause completed procs to change date
						MsgBox.Show(this,"Not allowed to move completed appointments.");
						return;
					}
					//HideDraggableTempApptSingle() handled down below
					SelectedAptNum=-1;
					OnApptMovedToPinboard(_dataRowTempAppt);
					#endregion Dragging to pinboard
					return;
				}
				//We got this far so we are moving the appt to a new location.
				Appointment appt=Appointments.GetOneApt(SelectedAptNum);
				if(appt==null){
					OnApptNullFound();
					return;
				}
				Plugins.HookAddCode(this, "ContrAppt.ContrApptSheet2_MouseUp_validation_end",appt);//after we know we have a valid appointment.
				apptOld=appt.Copy();
				if(contrTempAppt.Location.Y-this.Location.Y<_heightProvOpHeaders
					&& contrTempAppt.Location.Y-this.Location.Y>6)
				{//10 or less pixels into the header
					LayoutManager.MoveLocation(contrTempAppt,new Point(contrTempAppt.Left,this.Location.Y+(int)_heightProvOpHeaders+1));
				}
				TimeSpan? timeSpanNew=YPosToTime(contrTempAppt.Location.Y-this.Location.Y);
				if(timeSpanNew==null){
					return;
				}
				TimeSpan timeSpanNewRounded=RoundTimeToNearestIncrement(timeSpanNew.Value,MinPerIncr);
				RoundToNearestDateAndOp(contrTempAppt.Location.X-this.Location.X,//passing in as coordinates of this control
					out DateTime dateNew,
					out int opIdx,contrTempAppt.Width);
				if(opIdx<0){
					MsgBox.Show(this,"Invalid operatory");
					return;
				}
				if(timeSpanNewRounded!=appt.AptDateTime.TimeOfDay
					|| ListOpsVisible[opIdx].OperatoryNum!=appt.Op
					|| dateNew!=appt.AptDateTime.Date)
				{
					#region Prompt or block
					if(appt.AptStatus==ApptStatus.PtNote || appt.AptStatus==ApptStatus.PtNoteCompleted) {
						//no question for notes
						if(!Security.IsAuthorized(Permissions.AppointmentMove) || PatRestrictionL.IsRestricted(appt.PatNum,PatRestrict.ApptSchedule)) {
							return;
						}
					}
					else {
						if(!Security.IsAuthorized(Permissions.AppointmentMove)
							|| (appt.AptStatus==ApptStatus.Complete && (!Security.IsAuthorized(Permissions.AppointmentCompleteEdit)))
							|| PatRestrictionL.IsRestricted(appt.PatNum,PatRestrict.ApptSchedule))
						{
							return;
						}
						if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Move Appointment?")){
							return;
						}
					}
					#endregion Prompt or block
				}
				appt.AptDateTime=dateNew+timeSpanNewRounded;
				//Compare beginning of new appointment against end to see if the appointment spans two days
				if(appt.AptDateTime.Day!=appt.AptDateTime.AddMinutes(appt.Pattern.Length*5).Day) {
					MsgBox.Show(this,"You cannot have an appointment that starts and ends on different days.");
					return;
				}
				//Prevent double-booking of certain procedures
				if(IsDoubleBooked(appt)) {
					return;
				}
				//Operatory operatoryCur=_visOps[ApptDrawing.ConvertToOp(TempApptSingle.Location.X-ContrApptSheet2.Location.X)];
				appt.Op=ListOpsVisible[opIdx].OperatoryNum;
				OnApptMoved(appt,apptOld);
				//MoveAppointment(new List<Appointment>() { appt },new List<Appointment>() { apptOld },curOp,timeWasMoved,isOpChanged);//Apt's time has already been changed at this point.  Internally calls Appointments S-class to insert invalid signal.				
			}
			finally { //Cleanup. We are done with mouse up so we can't possibly be resizing or moving an appt.
				_isResizingAppt=false;
				HideDraggableTempApptSingle();//sets mouseup
			}
		}
		#endregion Methods - Event Handlers Mouse

		#region Methods - Event Handlers OnPaint
		/// <summary></summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Graphics graphics=e.Graphics;
			graphics.SmoothingMode=SmoothingMode.HighQuality;
			Rectangle rectangleOnControl;//the rectangle of the entire area under consideration. We cycle through subsections of the control.
			Rectangle rectangleClip;//frequently smaller than e.ClipRectangle. 
			Rectangle rectangleSource;//source within the bitmap we are pulling from
			float widthProvs;
			if(_showProvBars){
				widthProvs=_widthProv*_listProvsVisible.Count;
			}
			else{
				widthProvs=0;
			}
			//main area
			if(_bitmapMain!=null){
				rectangleOnControl=new Rectangle(Round(_widthTime+widthProvs),Round(_heightProvOpHeaders),_widthMainVisible,_heightMainVisible);
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					//We don't want to redraw the entire image.  If we are dragging a small appt across this, we only want to redraw a small rect.
					rectangleClip=Rectangle.Intersect(rectangleOnControl,e.ClipRectangle);
					rectangleSource=new Rectangle(rectangleClip.X-Round(_widthTime+widthProvs)+hScrollBar1.Value,
						rectangleClip.Y-Round(_heightProvOpHeaders)+vScrollBar1.Value,
						rectangleClip.Width,rectangleClip.Height);
					graphics.DrawImage(_bitmapMain,rectangleClip,rectangleSource,GraphicsUnit.Pixel);
				}
			}
			//No need for a clipping mask here because the other bitmaps will be drawn on top of this outline
			DrawOutlineSelected(graphics);
			//the other areas are small enough that we don't bother with calculating clip rectangles and smaller bitmaps.
			//prov bars header------------------------------------------------------------------------------------------------
			if(_showProvBars && _bitmapProvBarsHeader!=null){
				rectangleOnControl=new Rectangle(Round(_widthTime),0,Round(widthProvs),Round(_heightProvOpHeaders));
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					graphics.DrawImage(_bitmapProvBarsHeader,_widthTime,0);
				}
			}
			//ops header-------------------------------------------------------------------------------------------------------
			if(_bitmapOpsHeader!=null){
				rectangleOnControl=new Rectangle(Round(_widthTime+widthProvs),0,_widthMainVisible,Round(_heightProvOpHeaders));
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					rectangleSource=new Rectangle(hScrollBar1.Value,0,_widthMainVisible,Round(_heightProvOpHeaders));
					graphics.DrawImage(_bitmapOpsHeader,_widthTime+widthProvs,0,rectangleSource,GraphicsUnit.Pixel);
				}
			}
			//Prov bars--------------------------------------------------------------------------------------------------------
			if(_showProvBars && _bitmapProvBars!=null){
				rectangleOnControl=new Rectangle(Round(_widthTime),Round(_heightProvOpHeaders),Round(widthProvs),_heightMainVisible);
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					rectangleSource=new Rectangle(0,vScrollBar1.Value,Round(widthProvs),_heightMainVisible);
					graphics.DrawImage(_bitmapProvBars,_widthTime,_heightProvOpHeaders,rectangleSource,GraphicsUnit.Pixel);
				}
			}
			//timebars---------------------------------------------------------------------------------------------------------
			rectangleSource=new Rectangle(0,vScrollBar1.Value,Round(_widthTime),_heightMainVisible);
			rectangleOnControl=new Rectangle(0,Round(_heightProvOpHeaders),Round(_widthTime),_heightMainVisible);
			if(_bitmapTimebarLeft!=null){ 
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					graphics.DrawImage(_bitmapTimebarLeft,0,_heightProvOpHeaders,rectangleSource,GraphicsUnit.Pixel);
				}
			}
			rectangleOnControl=new Rectangle(this.Width-vScrollBar1.Width-Round(_widthTime)-1,Round(_heightProvOpHeaders),Round(_widthTime),_heightMainVisible);
			if(_bitmapTimebarRight!=null){ 
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					graphics.DrawImage(_bitmapTimebarRight,this.Width-vScrollBar1.Width-_widthTime-1,_heightProvOpHeaders,rectangleSource,GraphicsUnit.Pixel);
				}
			}
			//Some extra lines to divide areas, especially when scrolling----------------------------------------------------
			graphics.DrawLine(_penVertPrimary,Width-1,_heightProvOpHeaders,Width-1,Height);//to the right of vScroll
			graphics.DrawLine(_penVertPrimary,_widthTime,0,_widthTime,_heightProvOpHeaders+_heightMainVisible);//to right of left timebar
			if(_showProvBars){
				graphics.DrawLine(_penVertPrimary,_widthTime+widthProvs,0,_widthTime+widthProvs,_heightProvOpHeaders+_heightMainVisible);//to right of provbars
			}
			graphics.DrawLine(_penVertPrimary,_widthTime+widthProvs+_widthMainVisible,0,_widthTime+widthProvs+_widthMainVisible,_heightProvOpHeaders);//to the right of Ops header
			graphics.DrawLine(_penVertPrimary,hScrollBar1.Left-1,hScrollBar1.Top,hScrollBar1.Left-1,hScrollBar1.Bottom);//to the left of hScroll
			graphics.DrawLine(_penVertPrimary,hScrollBar1.Right,hScrollBar1.Top,hScrollBar1.Right,hScrollBar1.Bottom);//to the right of hScroll
			graphics.DrawLine(_penVertPrimary,vScrollBar1.Left,vScrollBar1.Top-1,vScrollBar1.Right,vScrollBar1.Top-1);//to the top of vScroll
			graphics.DrawLine(_penVertPrimary,vScrollBar1.Left,vScrollBar1.Bottom,vScrollBar1.Right,vScrollBar1.Bottom);//to the top of vScroll
			//red line--------------------------------------------------------------------------------------------------------
			//DrawRedTimeLine(graphics);//uses math to stay within bounds 
			//Bubble----------------------------------------------------------------------------------------------------------
			if(_isBubbleVisible && _bitmapBubble!=null){
				rectangleOnControl=_rectangleBubble;
				if(rectangleOnControl.IntersectsWith(e.ClipRectangle)){
					graphics.DrawImage(_bitmapBubble,_rectangleBubble);
				}
			}
		}

		///<summary></summary>
		private void TempApptSingle_Paint(object sender,PaintEventArgs e) {
			if(_isResizingAppt){
				e.Graphics.DrawImage(_bitmapTempApptSingle,0,0);
				using(Pen pen=new Pen(Color.Black,1f)){
					//broken X
					if(_dataRowTempAppt["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()) {
						e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
						e.Graphics.DrawLine(pen,WidthProvOnAppt,1,contrTempAppt.Width-1,contrTempAppt.Height-1);
						e.Graphics.DrawLine(pen,WidthProvOnAppt,contrTempAppt.Height-1,contrTempAppt.Width-1,1);
					}
					//bottom line
					e.Graphics.DrawLine(pen,0,contrTempAppt.Height-1,contrTempAppt.Width,contrTempAppt.Height-1);
				}
			}
			else{
				e.Graphics.DrawImage(_bitmapTempApptSingle,0,0);
			}
		}
		#endregion Methods - Event Handlers OnPaint

		#region Methods - Event Handlers PrintPage
		///<summary></summary>
		private void DrawPrintingHeaderFooter(Graphics g,int totalPages,float pageWidth,float pageHeight) {
			//float xPos=0;//starting pos
			float yPos=25f;//starting pos
			//Print Title------------------------------------------------------------------------------
			string title;
			string date;
			if(IsWeeklyView) {
				title=Lan.g(this,"Weekly Appointments");
				date=DateStart.DayOfWeek.ToString()+" "+DateStart.ToShortDateString()
					+" - "+DateEnd.DayOfWeek.ToString()+" "+DateEnd.ToShortDateString();
			}
			else {
				title=Lan.g(this,"Daily Appointments");
				date=DateSelected.DayOfWeek.ToString()+"   "+DateSelected.ToShortDateString();
			}
			Font titleFont=new Font("Arial",12,FontStyle.Bold);
			float xTitle = pageWidth/2-g.MeasureString(title,titleFont).Width/2;
			g.DrawString(title,titleFont,Brushes.Black,xTitle,yPos);//centered
			//Print Date--------------------------------------------------------------------------------
			Font dateFont=new Font("Arial",8,FontStyle.Regular);
			float xDate = pageWidth/2-g.MeasureString(date,dateFont).Width/2;
			yPos+=20;
			g.DrawString(date,dateFont,Brushes.Black,xDate,yPos);//centered
			//Print Footer-----------------------------------------------------------------------------
			string page=(PagesPrinted+1)+" / "+totalPages;
			float xPage = pageWidth/2f-g.MeasureString(page,dateFont).Width/2f;
			yPos=pageHeight-35;
			g.DrawString(page,dateFont,Brushes.Black,xPage,yPos);
		}

		public void PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			_isPrinting=true;
			Rectangle rectanglePageBounds=e.PageBounds;
			//Header, footer, provbars, and timebars are present on all pages so that the appointment area is consistent in size and easier to calc for now.
			//The margins defined here are for the appointment drawing area, excluding the text of the simple headers and footers
			//Within these margins, a series of image portions needs to be drawn,
			//including _bitmapBackAppts, _bitmapProvBars,_bitmapOpsHeader,_bitmapProvBarsHeader, and_bitmapTimebarLeft/Right
			Rectangle rectangleMarginBounds=new Rectangle(30,65,rectanglePageBounds.Width-30-30,rectanglePageBounds.Height-65-40);
			//the next few lines are instead of LayoutRecalcAfterResize()
			float widthProvs;
			if(_showProvBars){
				widthProvs=_widthProv*_listProvsVisible.Count;
			}
			else{
				widthProvs=0;
			}
			float widthMainOnePage=rectangleMarginBounds.Width-_widthTime*2-(widthProvs);
			float widthCol=widthMainOnePage/PrintingColsPerPage;//widthCol should be ignored for WeekView
			if(IsWeeklyView){
				_widthMain=(int)widthMainOnePage; //Always just one page wide for week
			}
			else{
				_widthMain=(int)(widthCol*ListOpsVisible.Count);//This is for multiple pages. This entire width will be split across the pages later
			}
			_font.Dispose();
			_font=new Font(FontFamily.GenericSansSerif,PrintingSizeFont);
			Graphics g=e.Graphics;
			//_heightLine=g.MeasureString("X",_font,100,StringFormat.GenericTypographic).Height;
			_heightLine=_font.Height;
			//the math for calculating time start and stop is clumsy, but it's low priority to make it look better.
			int startHour=DateTimePrintStart.Hour;
			int stopHour=DateTimePrintStop.Hour;
			if(stopHour==0) {
				stopHour=24;
			}
			_heightMain=(int)(_heightLine*_rowsPerHr*24f);//This is for multiple pages, 24 hrs 
			//This entire height will be split among the pages later. Automatically rounded down.
			_isValidMain=false;
			_isValidHeaders=false;
			_isValidTimebars=false;
			RedrawAsNeeded();//All redrawn with these new widths and heights. Needed for the bitmap section, below, not for actual output.
			int heightMainOnePage=rectangleMarginBounds.Height-(int)_heightProvOpHeaders;
			int pagesAcross=(int)Math.Ceiling((decimal)_listOpsVisible.Count/PrintingColsPerPage);//Rounds up.  We frequently fall exactly on a boudary for rounding.
			int pagesTall=(int)Math.Ceiling(_heightLine*_rowsPerHr*(stopHour-startHour)/heightMainOnePage);//rounds up. Unlikely to be any close rounding issues here.
			int totalPages=pagesAcross*pagesTall;
			if(IsWeeklyView) {
				pagesAcross=1;
				totalPages=1*pagesTall;
			}
			#region Rows and Cols for this Page
			double dHoursPerPage=heightMainOnePage /_heightLine/_rowsPerHr;
			int hoursPerPage=(int)Math.Truncate(dHoursPerPage);//rounds down
			int hourBegin=startHour+(hoursPerPage*PrintingPageRow);//PrintingPageRow=0 for first page
			int hourEnd=hourBegin+hoursPerPage;
			if(hourEnd>stopHour) {//Don't show too many hours on the last page
				hourEnd=stopHour;
			}
			if(hourEnd>23) {//Midnight must be 0.
				hourEnd=0;
			}
			DateTime dateTimeBegin=new DateTime(1,1,1,hourBegin,0,0);
			DateTime dateTimeEnd=new DateTime(1,1,1,hourEnd,0,0);
			int colStart=PrintingPageColumn*PrintingColsPerPage;
			int colStop=colStart+PrintingColsPerPage;
			if(colStop>_listOpsVisible.Count){
				colStop=_listOpsVisible.Count;
			}
			#endregion Rows and Cols for this Page
			g.InterpolationMode=InterpolationMode.High;//reduces blur in print preview.  We're scaling an image smaller, so this is the best we can do.
			//Looks good in actual printing, regardless of this setting.
			#region CopyImageToClipboard
			//Some users 'paste' to their own editor for more control.
			if(PagesPrinted==0){//only on first page
				//this bitmap will not include headers
				float yTop=(float)Math.Ceiling(_heightLine*_rowsPerHr*startHour);
				float height=(float)Math.Ceiling(_heightLine*_rowsPerHr*(stopHour-startHour));
				Bitmap bitmapClipboard=new Bitmap((int)_widthTime*2+_widthMain+(int)widthProvs,(int)height);
				Graphics graphicsClipboard=Graphics.FromImage(bitmapClipboard);
				graphicsClipboard.InterpolationMode=InterpolationMode.High;
				RectangleF rectangleSource=new RectangleF(0,yTop,_widthTime,height);
				RectangleF rectangleDest=new RectangleF(0,0,_widthTime,height);	
				graphicsClipboard.DrawImage(_bitmapTimebarLeft,rectangleDest,rectangleSource,GraphicsUnit.Pixel);
				rectangleSource=new RectangleF(0,yTop,widthProvs,height);
				rectangleDest=new RectangleF(_widthTime,0,widthProvs,height);
				if(_bitmapProvBars!=null){
					graphicsClipboard.DrawImage(_bitmapProvBars,rectangleDest,rectangleSource,GraphicsUnit.Pixel);
				}
				rectangleSource=new RectangleF(0,yTop,_widthMain,height);
				rectangleDest=new RectangleF(_widthTime+widthProvs,0,_widthMain,height);	
				graphicsClipboard.DrawImage(_bitmapMain,rectangleDest,rectangleSource,GraphicsUnit.Pixel);
				rectangleSource=new RectangleF(0,yTop,_widthTime,height);
				rectangleDest=new RectangleF(_widthTime+widthProvs+_widthMain,0,_widthTime,height);	
				graphicsClipboard.DrawImage(_bitmapTimebarRight,rectangleDest,rectangleSource,GraphicsUnit.Pixel);
				try {
					Clipboard.SetImage(bitmapClipboard);
				}
				catch {
					//do nothing
				}
				graphicsClipboard.Dispose();
				//bitmapClipboard.Dispose();//this doesn't work.  I'll assume the clipboard will dispose of it
			}
			#endregion CopyImageToClipboard
			//Main-----------------------------------------------------------------------------------------------------------
			RectangleF rectangleSourceMain;//source within _bitmapBackAppts that we are pulling from
			if(IsWeeklyView){
				rectangleSourceMain=new RectangleF(
					colStart*widthCol,//colStart is 0 on first page
					hourBegin*_heightLine*_rowsPerHr,
					_widthMain,
					(hourEnd-hourBegin)*_heightLine*_rowsPerHr);
			}
			else{
				rectangleSourceMain=new RectangleF(
					colStart*widthCol,
					hourBegin*_heightLine*_rowsPerHr,
					(colStop-colStart)*widthCol,
					(hourEnd-hourBegin)*_heightLine*_rowsPerHr);
			}
			//destination rect width and height must be specified or it will be 4% too big because it's at 100dpi instead of 96dpi
			RectangleF rectangleDestMain=new RectangleF(
				rectangleMarginBounds.X+_widthTime+widthProvs,
				rectangleMarginBounds.Y+_heightProvOpHeaders,
				rectangleSourceMain.Width,
				rectangleSourceMain.Height);
			//g.DrawImage(_bitmapMain,rectangleDestMain,rectangleSourceMain,GraphicsUnit.Pixel);
			g.TranslateTransform(-rectangleSourceMain.X+rectangleDestMain.X,-rectangleSourceMain.Y+rectangleDestMain.Y);
			g.SetClip(rectangleSourceMain);
			SetBitmapMain(g);
			g.ResetClip();
			g.ResetTransform();
			//Prov Bars------------------------------------------------------------------------------------------------------
			if(_showProvBars && _bitmapProvBars!=null){
				RectangleF rectangleSourceProvBars=new RectangleF(
					0,
					hourBegin*_heightLine*_rowsPerHr,
					widthProvs,
					(hourEnd-hourBegin)*_heightLine*_rowsPerHr);
				RectangleF rectangleDestProvBars=new RectangleF(
					rectangleMarginBounds.X+_widthTime,
					rectangleMarginBounds.Y+_heightProvOpHeaders,
					rectangleSourceProvBars.Width,
					rectangleSourceProvBars.Height);
				//g.DrawImage(_bitmapProvBars,rectangleDestProvBars,rectangleSourceProvBars,GraphicsUnit.Pixel);
				g.TranslateTransform(-rectangleSourceProvBars.X+rectangleDestProvBars.X,-rectangleSourceProvBars.Y+rectangleDestProvBars.Y);
				g.SetClip(rectangleSourceProvBars);
				SetBitmapProvbars(g);
				g.ResetClip();
				g.ResetTransform();
			}
			//ProvBars Header---------------------------------------------------------------------------------------------------
			if(_showProvBars && _bitmapProvBarsHeader!=null){
				RectangleF rectangleDestProvBarHeader= new RectangleF(
					rectangleMarginBounds.X+_widthTime,
					rectangleMarginBounds.Y,
					_bitmapProvBarsHeader.Width,
					_bitmapProvBarsHeader.Height);
				//g.DrawImage(_bitmapProvBarsHeader,rectangleSourceProvBarHeader
				// no source was set because it will always be at 0,0
				g.TranslateTransform(rectangleDestProvBarHeader.X,rectangleDestProvBarHeader.Y);
				//g.SetClip();
				SetBitmapProvBarsHeaders(g);
				//g.ResetClip();
				g.ResetTransform();
			}
			//Ops Header-------------------------------------------------------------------------------------------------------
			RectangleF rectangleSourceOpsHeader;
			if(IsWeeklyView){
				rectangleSourceOpsHeader=new RectangleF(
					colStart*widthCol,
					0,
					_widthMain,
					_heightProvOpHeaders);
			}
			else{
				rectangleSourceOpsHeader=new RectangleF(
					colStart*widthCol,
					0,
					(colStop-colStart)*widthCol,
					_heightProvOpHeaders);
			}
			RectangleF retangleDestOpsHeader=new RectangleF(
				rectangleMarginBounds.X+_widthTime+widthProvs,
				rectangleMarginBounds.Y,
				rectangleSourceOpsHeader.Width,
				rectangleSourceOpsHeader.Height);
			//g.DrawImage(_bitmapOpsHeader,retangleDestOpsHeader,rectangleSourceOpsHeader,GraphicsUnit.Pixel);
			g.TranslateTransform(-rectangleSourceOpsHeader.X+retangleDestOpsHeader.X,-rectangleSourceOpsHeader.Y+retangleDestOpsHeader.Y);
			g.SetClip(rectangleSourceOpsHeader);
			SetBitmapOpsHeaders(g);
			g.ResetClip();
			g.ResetTransform();
			//Time Bar Left--------------------------------------------------------------------------------------------------------
			RectangleF rectangleSourceTime=new RectangleF(
				0,
				hourBegin*_heightLine*_rowsPerHr,
				_widthTime,
				(hourEnd-hourBegin)*_heightLine*_rowsPerHr);
			RectangleF rectangleDestTime=new RectangleF(
				rectangleMarginBounds.X,
				rectangleMarginBounds.Y+_heightProvOpHeaders,
				rectangleSourceTime.Width,
				rectangleSourceTime.Height);
			//g.DrawImage(_bitmapTimebarLeft,rectangleDestTime,rectangleSourceTime,GraphicsUnit.Pixel);
			g.TranslateTransform(-rectangleSourceTime.X+rectangleDestTime.X,-rectangleSourceTime.Y+rectangleDestTime.Y);
			g.SetClip(rectangleSourceTime);
			SetBitmapTimebarL(g);
			g.ResetClip();
			g.ResetTransform();
			//Time Bar Right---------------------------------------------------------------------------------------------------------
			rectangleDestTime=new RectangleF(
				rectangleDestMain.Right,
				rectangleMarginBounds.Y+_heightProvOpHeaders,
				rectangleSourceTime.Width,
				rectangleSourceTime.Height);
			//g.DrawImage(_bitmapTimebarRight,rectangleDestTime,rectangleSourceTime,GraphicsUnit.Pixel);
			g.TranslateTransform(-rectangleSourceTime.X+rectangleDestTime.X,-rectangleSourceTime.Y+rectangleDestTime.Y);
			g.SetClip(rectangleSourceTime);
			SetBitmapTimebarR(g);
			g.ResetClip();
			g.ResetTransform();
			//g.DrawRectangle(Pens.Red,rectangleMarginBounds);//just for testing
			DrawPrintingHeaderFooter(e.Graphics,totalPages,rectanglePageBounds.Width,rectanglePageBounds.Height);
			PagesPrinted++;
			PrintingPageColumn++;
			if(totalPages==PagesPrinted) {
				PagesPrinted=0;
				PrintingPageRow=0;
				PrintingPageColumn=0;
				e.HasMorePages=false;
			}
			else {
				e.HasMorePages=true;
				if(PagesPrinted==pagesAcross*(PrintingPageRow+1)) {
					PrintingPageRow++;
					PrintingPageColumn=0;
				}
			}
			_font.Dispose();
			_font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(_sizeFont));//Reset to default.
			//_heightLine=g.MeasureString("X",_font,100,StringFormat.GenericTypographic).Height;
			_heightLine=_font.Height;
			_isPrinting=false;//if there is another page to print, this gets set back to true again at the top of this event.
			if(!e.HasMorePages){
				ComputeHeight();//must come after set _isPrinting=false
				LayoutRecalcAfterResize();//for _widthMain
				_isValidMain=false;
				_isValidHeaders=false;
				_isValidTimebars=false;
				RedrawAsNeeded();
			}
		}
		#endregion Methods - Event Handlers PrintPage 

		#region Methods - Public
		public void GetBitmapForPinboard(Graphics g,DataRow dataRow,string patternShowing,float width,float height){
			g.SmoothingMode=SmoothingMode.HighQuality;
			DrawOneAppt(g,dataRow,patternShowing,width-1,height-1);
		}

		public DataRow GetDataRowForSelected(){
			if(SelectedAptNum==-1){
				return null;
			}
			for(int i=0;i<TableAppointments.Rows.Count;i++){
				if(TableAppointments.Rows[i]["AptNum"].ToString()==SelectedAptNum.ToString()){
					return TableAppointments.Rows[i];
				}
			}
			return null;
		}

		///<summary>Returns the index of the opNum within VisOps.  Returns -1 if not in VisOps.</summary>
		public int GetIndexOp(long opNum) {
			//No need to check RemotingRole; no call to db.
			int index;
			if(_dictOpNumToColumnNum.TryGetValue(opNum,out index)){
				return index;
			}
			else{
				return -1;
			}
		}

		///<summary>Returns the index of the provNum within VisProvs, or -1.</summary>
		public int GetIndexProv(long provNum) {
			//No need to check RemotingRole; no call to db.
			int index;
			return _dictProvNumToColumnNum.TryGetValue(provNum,out index) ? index : -1;
		}

		///<summary>This converts the dbPattern in 5 minute interval into the pattern that will be viewed based on RowsPerIncr and AppointmentTimeIncrement.  So it will always depend on the current view.Therefore, it should only be used for visual display purposes rather than within the FormAptEdit. If height of appointment allows a half increment, then this includes an row for that half increment.</summary>
		public string GetPatternShowing(string dbPattern) {
			StringBuilder strBTime=new StringBuilder();
			for(int i=0;i<dbPattern.Length;i++) {
				for(int j=0;j<RowsPerIncr;j++) {//Example: 10 MinPerIncr, 2 RowsPerIncr, so 12 per hour
					strBTime.Append(dbPattern.Substring(i,1));
				}
				if(MinPerIncr==10) {
					i++;//skip
				}
				if(MinPerIncr==15) {
					i++;
					i++;//skip two
				}
			}
			return strBTime.ToString();
		}

		///<summary>Returns a the production amount for the appointments as a string. Also includes net production. Uses either the listOpNums or listProvNums to calculate the net production.</summary>
		public string GetProduction(List<DataRow> listDataRows,List<long> listOpNums,List<long> listProvNums,DateTime start,DateTime end) {
			decimal grossproduction=0;
			decimal netproduction=0;
			int indexProv;
			foreach(DataRow dataRow in listDataRows){//TableAppointments.Rows){
				indexProv=-1;
				bool isHygiene=PIn.Bool(dataRow["IsHygiene"].ToString());
				long provNum=PIn.Long(dataRow["ProvNum"].ToString());
				long provHyg=PIn.Long(dataRow["ProvHyg"].ToString());
				long opNum=PIn.Long(dataRow["Op"].ToString());
				if(PrefC.GetBool(PrefName.ApptModuleProductionUsesOps)) {
					if(isHygiene){
						if(provHyg==0) {//if no hyg prov set.
							indexProv=GetIndexOp(opNum);
						}
						else {
							indexProv=GetIndexOp(opNum);
						}
					}
					else {//not hyg
						indexProv=GetIndexOp(opNum);
					}
				}
				else {//use provider bars in appointment view
					if(isHygiene) {
						if(provHyg==0) {//if no hyg prov set.
							indexProv=GetIndexProv(provNum);
						}
						else {
							indexProv=GetIndexProv(provHyg);
						}
					}
					else {//not hyg
						indexProv=GetIndexProv(provNum);
					}
				}
				if(indexProv==-1) {
					continue;
				}
				ApptStatus aptStatus=(ApptStatus)(PIn.Int(dataRow["AptStatus"].ToString()));
				long clinicNum=PIn.Long(dataRow["ClinicNum"].ToString());
				decimal productionVal=PIn.Decimal(dataRow["productionVal"].ToString());
				decimal netProductionVal=PIn.Decimal(dataRow["netProductionVal"].ToString());
				decimal writeoffPPO=PIn.Decimal(dataRow["writeoffPPO"].ToString());
				if(aptStatus!=ApptStatus.Broken
					&& aptStatus!=ApptStatus.UnschedList
					&& aptStatus!=ApptStatus.PtNote
					&& aptStatus!=ApptStatus.PtNoteCompleted) 
				{
					//When the program is restricted to a specific clinic, only count up production for the corresponding clinic.
					if(PrefC.HasClinicsEnabled 
						&& Clinics.ClinicNum!=0
						&& Clinics.ClinicNum!=clinicNum)
					{
						continue;//This appointment is for a different clinic.  Do not include this production in the daily prod.
					}
					//In order to get production numbers split by provider, it would require generating total production numbers
					//in another table from the business layer.  But that will only work if hyg procedures are appropriately assigned
					//when setting appointments.
					grossproduction+=productionVal;
					netproduction+=netProductionVal;
				}
			}
			string production=grossproduction.ToString("c0");
			if(grossproduction!=netproduction) {
				production+=Lan.g(this,", net:")+netproduction.ToString("c0");
			}
			return production;
		}

		///<summary>Used by parent form when a dialog needs to be displayed, but mouse might be down.  This forces a mouse up, and cleans up any mess so that dlg can show.</summary>
		public void MouseUpForced() {
			HideDraggableTempApptSingle();	
		}		
		
		///<summary>Call this whenever control should be redrawn because new data has been fetched or red time line needs to move.  Set an Invalid Flag first.  The Bubble is handled completely separately from this.</summary>
		public void RedrawAsNeeded(){
			if(_updatingLevel>0){//tests whether isUpdating
				return;
			}
			if(!_isValidMain){
				ComputeColWidth();
			}
			if(!_isValidHeaders){//need widths above in order to calc whether text will wrap
				ComputeHeightProvOpHeader();
			}
			if(!_isValidMain || !_isValidHeaders){
				ComputeHeight();
			}
			SetBitmapMain();
			SetBitmapProvbars();
			SetBitmapProvBarsHeaders();
			SetBitmapOpsHeaders();
			SetBitmapTimebarL();
			SetBitmapTimebarR();
			Invalidate();
		}

		///<summary>Called on startup and if color prefs change.</summary>
		public void SetColors(Color colorOpen,Color colorClosed,Color colorHoliday,Color colorBlockText,Color colorTimeLine){
			//Example pseudocode hints used before calling this method:
			//List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AppointmentColors);
			//colorOpen=(listDefs[0].ItemColor);
			//colorClosed=(listDefs[1].ItemColor);
			//colorHoliday=(listDefs[3].ItemColor);
			//colorBlockText=Defs.GetDefsForCategory(DefCat.AppointmentColors,true)[4].ItemColor;
			//colorTimeLine=PrefC.GetColor(PrefName.AppointmentTimeLineColor)
			DisposeObjects(_brushOpen,_brushClosed,_brushHoliday,_brushBlockText,_penTimeLine);
			_brushOpen=new SolidBrush(colorOpen);
			_brushClosed=new SolidBrush(colorClosed);
			_brushHoliday=new SolidBrush(colorHoliday);
			_brushBlockText=new SolidBrush(colorBlockText);
			_penTimeLine=new Pen(colorTimeLine,1.55f);//there's a big difference between 1.5 and 1.55.  ??
			_isValidMain=false;//if we change colors, redraw everything in main
			RedrawAsNeeded();
		}

		public void SetScrollByTime(TimeSpan timeOfDay){
			int valueNew=(int)(timeOfDay.TotalHours*_heightLine*(double)_rowsPerHr);
			if(valueNew<vScrollBar1.Minimum){
				valueNew=vScrollBar1.Minimum;
			}
			if(valueNew>vScrollBar1.Maximum-vScrollBar1.LargeChange){
				valueNew=vScrollBar1.Maximum-vScrollBar1.LargeChange;
			}
			vScrollBar1.Value=valueNew;
			//no bitmaps need to be redrawn
			Invalidate();//just a quick redraw
		}

		///<summary>Used for Planned apt and pinboard instead of SetLocation so that the location won't be altered.  Send in the actual timepattern in 5 min increments, not the PatternShowing.  Sets size in pixels.  If sizing an actual control, follow this by rounding.</summary>
		public SizeF SetSize(string pattern) {
			float widthSingleAppt;
			if(_widthOpCol<5){
				widthSingleAppt=0;
			}
			else{
				widthSingleAppt=_widthOpCol-3;
			}
			if(IsWeeklyView) {
				widthSingleAppt=_widthWeekAppt;
			}
			//height is based on original 5 minute pattern. Might result in half-rows
			float heightSingleAppt=pattern.Length*_heightLine*RowsPerIncr;
			if(MinPerIncr==10) {
				heightSingleAppt=heightSingleAppt/2;
			}
			if(MinPerIncr==15) {
				heightSingleAppt=heightSingleAppt/3;
			}
			return new SizeF(widthSingleAppt,heightSingleAppt);
		}
		#endregion Methods - Public

		#region Methods - Public Static
		public static List<Operatory> GetListOpsVisible(){
			return _listOpsVisible;
			//there is one place, in FormApptEdit, where it's incredibly hard to get a list of visible ops.
			//But we want to ensure no behavior change, so this is the unpleasant compromise for now
		}

		public static GraphicsPath GetRoundedPath(RectangleF rectF,float radiusCorner){
			//float width, float height){
			GraphicsPath graphicsPath=new GraphicsPath();
			graphicsPath.AddLine(rectF.Left+radiusCorner,rectF.Top,rectF.Right-radiusCorner,rectF.Top);//top
			graphicsPath.AddArc(rectF.Right-radiusCorner*2,rectF.Top,radiusCorner*2,radiusCorner*2,270,90);//UR
			graphicsPath.AddLine(rectF.Right,rectF.Top+radiusCorner,rectF.Right,rectF.Bottom-radiusCorner);//right
			graphicsPath.AddArc(rectF.Right-radiusCorner*2,rectF.Bottom-radiusCorner*2,radiusCorner*2,radiusCorner*2,0,90);//LR
			graphicsPath.AddLine(rectF.Right-radiusCorner,rectF.Bottom,rectF.Left+radiusCorner,rectF.Bottom);//bottom
			graphicsPath.AddArc(rectF.Left,rectF.Bottom-radiusCorner*2,radiusCorner*2,radiusCorner*2,90,90);//LL
			graphicsPath.AddLine(rectF.Left,rectF.Bottom-radiusCorner,rectF.Left,rectF.Top+radiusCorner);//left
			graphicsPath.AddArc(rectF.Left,rectF.Top,radiusCorner*2,radiusCorner*2,180,90);//UL
			return graphicsPath;
		}

		///<summary>Loops through all the appointments and decides where to place them.  Uses rectangles to define the positions.  If there is any overlap, then it makes appointments narrower so that they will fit side by side.  It does this by revising previous rectangles on the fly rather than any advance planning.  We do not store this layout in the db in any way.</summary>
		public static List<ApptLayoutInfo> PlaceAppointments(DataTable tableAppointments,List<Operatory> listOperatories, 
			float heightLine, bool isWeeklyView,float minPerIncr,int numOfWeekDaysToDisplay,float rowsPerIncr,
			int widthMain, float widthNarrowedOnRight,DateTime dateStart,DateTime dateEnd)
		{
			//This is public with parameters because we will need to build a variety of unit tests for it.
			if(tableAppointments==null || listOperatories==null){
				return null;
			}
			List<ApptLayoutInfo> listApptLayoutInfos=new List<ApptLayoutInfo>();
			//The following math is analogous to ComputeColWidth() to generate more of the useful variables without having to pass them in
			float widthCol;
			float widthWeekDay=0;
			float widthWeekAppt=0;
			//float dayofweek=0;//changes with each appt row
			//float idxOp=0;//changes with each appt row
			if(listOperatories.Count==0) {
				widthCol=0;
			}
			else {
				if(isWeeklyView) {
					widthWeekDay=(float)widthMain/numOfWeekDaysToDisplay;
					widthWeekAppt=widthWeekDay/listOperatories.Count;
				}
				widthCol=(float)widthMain/listOperatories.Count;
			}
			ApptLayoutInfo apptLayoutInfo;
			for(int i=0;i<tableAppointments.Rows.Count;i++){
				DataRow dataRow=tableAppointments.Rows[i];
				string strAptDateTime=dataRow["AptDateTime"].ToString();
				DateTime aptDateTime=PIn.Date(strAptDateTime);
				apptLayoutInfo=new ApptLayoutInfo();
				if(aptDateTime.Date < dateStart || aptDateTime.Date > dateEnd){
					//Appointment is outside of our date range
					apptLayoutInfo.idxInTableAppointments=i;
					apptLayoutInfo.RectangleBounds=new RectangleF(0,0,0,0);
					listApptLayoutInfos.Add(apptLayoutInfo);
					continue;
				}
				apptLayoutInfo.IdxOp=listOperatories.FindIndex(x=>x.OperatoryNum==PIn.Long(dataRow["Op"].ToString()));
				if(apptLayoutInfo.IdxOp==-1){//op not visible
					apptLayoutInfo.idxInTableAppointments=i;
					apptLayoutInfo.RectangleBounds=new RectangleF(0,0,0,0);
					listApptLayoutInfos.Add(apptLayoutInfo);
					continue;
				}
				apptLayoutInfo.DayOfWeek=(int)PIn.DateT(dataRow["AptDateTime"].ToString()).DayOfWeek-1;
				if(apptLayoutInfo.DayOfWeek==-1) {
					apptLayoutInfo.DayOfWeek=6;
				}
				PointF pointF=new PointF();
				if(isWeeklyView) {
					pointF.X=widthWeekDay*apptLayoutInfo.DayOfWeek+widthWeekAppt*apptLayoutInfo.IdxOp;
				}
				else {
					pointF.X=widthCol*apptLayoutInfo.IdxOp;
				}
				pointF.Y=(aptDateTime.Hour*60/minPerIncr+aptDateTime.Minute/minPerIncr)*heightLine*rowsPerIncr;
				string pattern=PIn.String(dataRow["Pattern"].ToString());
				//copied from SetSize(pattern):
				SizeF sizeF=new SizeF();
				if(widthCol<5){
					sizeF.Width=0;
				}
				else{
					sizeF.Width=widthCol-widthNarrowedOnRight;
				}
				if(isWeeklyView) {
					sizeF.Width=widthWeekAppt;
				}
				//height is based on original 5 minute pattern. Might result in half-rows
				sizeF.Height=pattern.Length*heightLine*rowsPerIncr;
				if(minPerIncr==10) {
					sizeF.Height/=2f;
				}
				if(minPerIncr==15) {
					sizeF.Height/=3f;
				}
				apptLayoutInfo.idxInTableAppointments=i;
				apptLayoutInfo.RectangleBounds=new RectangleF(Point.Round(pointF),sizeF);//fit the appointment to nearest pixel to help with fonts.
				//don't actually add it until it will fit
				//The newest appointment that we are adding is still full size, and won't be narrowed unless it's literally sitting on top of other appts.
				List<ApptLayoutInfo> listOverlaps=listApptLayoutInfos.FindAll(
					x => Rectangle.Truncate(x.RectangleBounds).IntersectsWith(Rectangle.Truncate(apptLayoutInfo.RectangleBounds)));
				if(listOverlaps.Count==0){
					listApptLayoutInfos.Add(apptLayoutInfo);
					continue;
				}
				//If we use RectangleF, the C# precision will be about 6-9 digits for testing intersectsWith.
				//This can cause false positives from rounding errors, where they look like they're just touching, but C# says they intersect.
				//If we use Rectangle, and they are touching, C# does not call this intersection. Example 0,0,100,100 does not intersect with 100,0,100,100.
				//One approach would be to build a custom RectangleF.IntersectsWith method with a precision parameter.
				//An easier approach is to trunc the rectangles down to nearest pixel before testing intersection to eliminate false positives.
				//Truncating x,y shifts the rectangle closer to other rectangles, but that's ok.
				//This will return true only if they overlap by at least one pixel. 
				//This might return false negatives if the layout is very narrow, where appointments are approaching one pixel width.
				//All the appointments in this list overlap the one we're trying to place, but not necessarily each other
				//It can get very messy very quickly.  The unit test project graphically tests every layout I could think of. 
				//Look for an open spot that won't require shrinking other appointments----------------------------------------------------------------------
				//Start by determining the narrowest of the overlaps
				float narrowest=listOverlaps.Max(x=>x.OverlapSections);//example 5 to indicate there might be a 1/5 slot open
				//The only rare problem is when overlapping appointments are of varying widths.  
				//If we just shrink any wide ones, then this might create an empty slot.
				for(int k=0;k<listOverlaps.Count;k++){
					if(listOverlaps[k].OverlapSections==(int)narrowest){
						continue;
					}
					listOverlaps[k].OverlapSections=(int)narrowest;
					CreateRectangle(listOverlaps[k]);
				}
				//Look for an open spot
				bool foundSpot=false;
				ApptLayoutInfo apptLayoutInfoNarrow;
				for(int j=0;j<narrowest;j++){//example loop 0 to 4
					apptLayoutInfoNarrow=apptLayoutInfo.Copy();
					apptLayoutInfoNarrow.OverlapSections=(int)narrowest;//example 5
					apptLayoutInfoNarrow.OverlapPosition=j;//example 1/5
					CreateRectangle(apptLayoutInfoNarrow);
					if(listOverlaps.Exists(x=> Rectangle.Truncate(x.RectangleBounds).IntersectsWith(Rectangle.Truncate(apptLayoutInfoNarrow.RectangleBounds)))){
						continue;
					}
					//we found an open spot. 
					apptLayoutInfo.OverlapSections=(int)narrowest;
					apptLayoutInfo.OverlapPosition=j;
					CreateRectangle(apptLayoutInfo);
					foundSpot=true;
					break;
				}
				if(foundSpot){
					listApptLayoutInfos.Add(apptLayoutInfo);
					continue;//done with this appt
				}
				//No open spots. Push our way in----------------------------------------------------------------------------------------------------------------
				//New appt goes on right-hand side, and shrink the appts to our left
				//This turns out to be very safe and reliable, as the unit tests show
				apptLayoutInfo.OverlapSections=(int)narrowest+1;//example 6
				apptLayoutInfo.OverlapPosition=(int)narrowest;//example 5/6
				CreateRectangle(apptLayoutInfo);
				listApptLayoutInfos.Add(apptLayoutInfo);
				for(int k=0;k<listOverlaps.Count;k++){
					listOverlaps[k].OverlapSections=(int)narrowest+1;//example 6
					//OverlapPosition remains unchanged.  Haven't yet found a scenario where this fails.
					CreateRectangle(listOverlaps[k]);
				}
				int count=0;//will keep track have how deep the ProcessOverlaps recursion has gone
				//Some secondary and tertiary overlaps can be created in rare situations.  Fix those.
				//This seems like a waste of resources, but it only gets triggered when we push our way in at the right.
				//And it goes fast because we only pass in the short list of appointments that we moved
				ProcessOverlaps(listApptLayoutInfos,listOverlaps,widthCol,apptLayoutInfo.IdxOp,widthNarrowedOnRight,isWeeklyView,widthWeekDay,widthWeekAppt,count);
			}
			return listApptLayoutInfos;
			//Local function to avoid passing lots of parameters.
			//X and width are calculated automatically, based on op, position, etc.  Y and height were set ahead of time and won't change here.
			void CreateRectangle(ApptLayoutInfo apptLayoutInfoLocal){
				//use floats for the math
				float y=apptLayoutInfoLocal.RectangleBounds.Y;
				float height=apptLayoutInfoLocal.RectangleBounds.Height;
				float overlapPosition=apptLayoutInfoLocal.OverlapPosition;
				float overlapSections=apptLayoutInfoLocal.OverlapSections;
				float idxOp=apptLayoutInfoLocal.IdxOp;
				float dayofweek=apptLayoutInfoLocal.DayOfWeek;
				if(isWeeklyView){
					apptLayoutInfoLocal.RectangleBounds=new RectangleF(
						widthWeekDay*dayofweek+widthWeekAppt*idxOp+widthWeekAppt*overlapPosition/overlapSections,
						y,
						widthWeekAppt/overlapSections,
						height);
				}
				else{
					apptLayoutInfoLocal.RectangleBounds=new RectangleF(
						widthCol*idxOp+(widthCol-widthNarrowedOnRight)*overlapPosition/overlapSections,//3/6
						y,
						(widthCol-widthNarrowedOnRight)/overlapSections,// 1/6
						height);
				}
			}
		}

		///<summary>A recursive function for rare secondary and tertiary overlaps created by adding a single appointment that shifted other tall appts.</summary>
		private static void ProcessOverlaps(List<ApptLayoutInfo> listApptLayoutInfos,List<ApptLayoutInfo> listOverlapsPrevious,float widthCol,float idxOp,
			float widthNarrowedOnRight,bool isWeeklyView,float widthWeekDay,float widthWeekAppt,int count)
		{
			if(count>=10) {
				return;//it should never process overlaps 10 times
			}
			count++;
			for(int k=0;k<listOverlapsPrevious.Count;k++){
				for(int i=0;i<listApptLayoutInfos.Count;i++){
					if(listApptLayoutInfos[i].idxInTableAppointments==listOverlapsPrevious[k].idxInTableAppointments){
						continue;//self
					}
					if(listApptLayoutInfos[i].DayOfWeek!=listOverlapsPrevious[k].DayOfWeek) {
						continue;//if they aren't on the same day, they aren't overlapping, so skip 
					}
					//if(listApptLayoutInfos[i].idxInTableAppointments==idxApptOriginal){
					//	continue;//We also don't want to compare it to the appointment that was originally placed, causing the moves.
					//}//harmless and not faster
					if(Rectangle.Truncate(listApptLayoutInfos[i].RectangleBounds).IntersectsWith(Rectangle.Truncate(listOverlapsPrevious[k].RectangleBounds))){
						//there is overlap, so fix it
						//we are leaving overlapPrevious alone, and changing the other one
						if(listApptLayoutInfos[i].OverlapSections<listOverlapsPrevious[k].OverlapSections){
							//increasing OverlapSections (e.g. from 5 to 6) shifts it left and makes it smaller.
							//This works unless number of appointments in column gets up to about 7, then it can start to fail.
							listApptLayoutInfos[i].OverlapSections=listOverlapsPrevious[k].OverlapSections;  //size it to match overlapPrevious
						}
						if(isWeeklyView){
							listApptLayoutInfos[i].RectangleBounds=new RectangleF(
								widthWeekDay*listApptLayoutInfos[i].DayOfWeek+widthWeekAppt*idxOp+widthWeekAppt*listApptLayoutInfos[i].OverlapPosition/listApptLayoutInfos[i].OverlapSections,
								listApptLayoutInfos[i].RectangleBounds.Y,
								(widthWeekAppt-2)/listApptLayoutInfos[i].OverlapSections,
								listApptLayoutInfos[i].RectangleBounds.Height);
						}
						else{
							//OverlapPosition remains unchanged.
							listApptLayoutInfos[i].RectangleBounds=new RectangleF(
								widthCol*idxOp+(widthCol-widthNarrowedOnRight)*listApptLayoutInfos[i].OverlapPosition/listApptLayoutInfos[i].OverlapSections,//3/6
								listApptLayoutInfos[i].RectangleBounds.Y,
								(widthCol-widthNarrowedOnRight)/listApptLayoutInfos[i].OverlapSections,// 1/6
								listApptLayoutInfos[i].RectangleBounds.Height);
						}
						//now, send this fixed appt in recursively to see if it affected anything else
						ProcessOverlaps(listApptLayoutInfos,new List<ApptLayoutInfo>(){listApptLayoutInfos[i]},//just pass in this single
							widthCol,idxOp,widthNarrowedOnRight,isWeeklyView,widthWeekDay,widthWeekAppt,count);
					}
				}
			}
		}

		///<summary>Used when clicking. Rounds time down to the nearest valid increment.  Static for unit test.</summary>
		public static TimeSpan RoundTimeDown(TimeSpan timeSpanIn,double minPerIncr){
			double base10Increment=minPerIncr/60d;//For example, 10 min becomes .167
			double totalIncrements=timeSpanIn.TotalHours/base10Increment;//not rounded, this is how many increments since midnight.
			double truncatedIncrements=Math.Truncate(totalIncrements);//rounded down
			double totalHours=truncatedIncrements*base10Increment;//for example, 8:10a would be 8.167
			TimeSpan timeSpanTruncated=TimeSpan.FromHours(totalHours);//but there might be a stray second
			return new TimeSpan(timeSpanTruncated.Hours,timeSpanTruncated.Minutes,0);
		}

		///<summary>Used when dropping an appointment to a new location or resizing. Rounds time up or down to the nearest valid increment.  Static for unit test.</summary>
		public static TimeSpan RoundTimeToNearestIncrement(TimeSpan timeSpanIn,double minPerIncr){
			double base10Increment=minPerIncr/60d;//For example, 10 min becomes .167
			double totalIncrements=timeSpanIn.TotalHours/base10Increment;//not rounded, this is how many increments since midnight.
			double roundedIncrements=Math.Round(totalIncrements);
			double totalHours=roundedIncrements*base10Increment;//for example, 8:10a would be 8.167
			TimeSpan timeSpanRounded=TimeSpan.FromHours(totalHours);//but there might be a stray second
			return new TimeSpan(timeSpanRounded.Hours,timeSpanRounded.Minutes,0);
		}
		#endregion Methods - Public Static

		#region Methods - Public BeginEndUpdate
		///<summary>Call this before making complex changes to properties.  Always pair with EndUpdate.</summary>
		public void BeginUpdate() {
			_updatingLevel++;
		}

		///<summary>Call after BeginUpdate, always paired, and when done making complex changes.</summary>
		public void EndUpdate() {
			_updatingLevel--;
			RedrawAsNeeded();
			Invalidate();
		}
		#endregion Methods - Public BeginEndUpdate

		#region Methods - Public Dropping
		public bool IsDoubleBooked(Appointment appt) {
			if(AppointmentRules.GetCount() == 0) { //If no rules exist then don't bother checking.
				return false;
			}
			List<long> listAptNums=new List<long>();
			for(int i=0;i<TableAppointments.Rows.Count;i++){
				listAptNums.Add(PIn.Long(TableAppointments.Rows[i]["AptNum"].ToString()));
			}
			List<Procedure> procsMultApts=Procedures.GetProcsMultApts(listAptNums);
			Procedure[] procsForOne=Procedures.GetProcsOneApt(appt.AptNum,procsMultApts);
			ArrayList doubleBookedCodes=Appointments.GetDoubleBookedCodes(appt,TableAppointments.Copy(),procsMultApts,procsForOne);
			if(doubleBookedCodes.Count==0) {
				return false;
			}
			//if some codes would be double booked
			if(AppointmentRules.IsBlocked(doubleBookedCodes)) {
				MessageBox.Show(Lan.g(this,"Not allowed to double book: ")
					+AppointmentRules.GetBlockedDescription(doubleBookedCodes));
				return true;
			}
			return false;
		}
		///<summary>Used when dropping an appointment to a new location.  Converts x-coordinate to date and opIdx.  Used in both daily and weekly views.  It is very different from XPosToOpIdx, which doesn't round the same way. Pass in X in coords of this entire control.  Date will always be reasonable, but opIdx might be -1 if unable to resolve.</summary>

		public void RoundToNearestDateAndOp(int xPos,out DateTime date,out int opIdx,float widthAppt) {
			//This cannot be two separate methods because the rounding increment is op, so date and op are interdependent.
			date=DateStart;//for daily view, this is the same as DateSelected
			opIdx=-1;
			if(ListOpsVisible.Count==0){
				return;
			}
			float xPosMain=TranslateXContrToMain(xPos);
			if(xPosMain<0){
				//user might have dragged it slightly into left timebar
				date=DateStart;//=DateSelected for daily
				opIdx=0;
				return;
			}
			int idxNearestOp;
			if(!IsWeeklyView){
				//daily appointments are not necessarily as wide as operatories because we might grab a narrow one stacked next to others.
				//The real condition to test is which op the majority of the appt is sitting in
				//So, take the midpoint of the appointment, and find exactly where it falls.
				float midpoint=xPos+(widthAppt/2);//coordinates of control, not main bitmap
				idxNearestOp=XPosToOpIdx((int)midpoint);
				//idxNearestOp=Round(xPosMain/_widthCol);
				if(idxNearestOp>ListOpsVisible.Count-1){
					idxNearestOp=ListOpsVisible.Count-1;
				}
				opIdx=idxNearestOp;
				return;
			}
			//weekly from here down--------------------------------------------------------------------------------------------
			idxNearestOp=Round(xPosMain/_widthWeekAppt);//895/100=9
			//in this case, it will be across multiple days
			//On the last day, in the last op, in the right half, it would round up to the first op of the following day.  This is not allowed.
			int maxOps=_listOpsVisible.Count*_numOfWeekDaysToDisplay;//e.g. 2*7=14
			if(idxNearestOp==maxOps){//14 not allowed
				idxNearestOp=maxOps-1;//13, which is last op on 7th day
			}
			int idxDay=(int)((float)idxNearestOp/ListOpsVisible.Count); //rounds down.  e.g. 9/2=4.5= day 4
			//impossible for idxDay to be out of range
			date=DateStart.AddDays(idxDay);
			opIdx=idxNearestOp-(idxDay*ListOpsVisible.Count);//9-(4*2)=1
			if(opIdx>ListOpsVisible.Count-1){
				opIdx=ListOpsVisible.Count-1;
			}
			if(opIdx<0){
				opIdx=0;
			}
		}

		///<summary>Pass in Y as coordinate of this entire control.</summary>
		public TimeSpan? YPosToTime(int yPos){
			if(yPos<_heightProvOpHeaders){
				return null;
			}
			float yMain=yPos-_heightProvOpHeaders+vScrollBar1.Value;//now it's in bitmapMain coordinates
			double hours=yMain/_rowsPerHr/_heightLine;//this is hours and decimal minutes
			TimeSpan timeSpan=TimeSpan.FromHours(hours);
			return timeSpan;
		}		
		#endregion Methods - Public Dropping

		#region Methods - Private
		///<summary>Returns true if the status passed in is a valid CareCredit approval status that the appointment has an icon for.</summary>
		private bool IsValidCareCreditStatus(string careCreditStatus) {
			if(string.IsNullOrWhiteSpace(careCreditStatus)) {
				return false;
			}
			string careCreditStatusLower=careCreditStatus.ToLower();
			return ListTools.In(careCreditStatusLower,new List<string>() {
				CareCreditWebStatus.AccountFound.GetDescription().ToLower(),
				CareCreditWebStatus.CallForAuth.GetDescription().ToLower(),
				CareCreditWebStatus.Declined.GetDescription().ToLower(),
				CareCreditWebStatus.PreApproved.GetDescription().ToLower(),
				CareCreditWebStatus.DupQS.GetDescription().ToLower()
			});
		}
		#endregion

		#region Methods - Private Set Bitmaps
		///<summary>Triggered much less frequently than OnPaint.</summary>
		private void SetBitmapMain(Graphics gPrinting=null){
			//if(_isValidMain){//no longer test so that red time line will draw
			//	return;
			//}
			//Main---------------------------------------------------------------------------------------------------
			if(_widthMain<1){
				_bitmapMain?.Dispose();
				_bitmapMain=null;
			}
			else if(gPrinting!=null){
				gPrinting.SmoothingMode=SmoothingMode.HighQuality;
				gPrinting.TextRenderingHint=TextRenderingHint.AntiAlias;
				DrawBackground(gPrinting);
				DrawRedTimeLineMain(gPrinting);
				gPrinting.TextRenderingHint=TextRenderingHint.AntiAlias;
				DrawAppts(gPrinting);
			}
			else{
				if(_bitmapMain==null){
					_bitmapMain=new Bitmap(_widthMain,_heightMain);
				}
				//Reuse the bitmap if it's the correct size.  Otherwise, dispose and reinitialize.
				else if(_bitmapMain.Size!=new Size(_widthMain,_heightMain)){
					_bitmapMain.Dispose();
					_bitmapMain=new Bitmap(_widthMain,_heightMain);
				}
				using(Graphics g=Graphics.FromImage(_bitmapMain)){
					g.SmoothingMode=SmoothingMode.HighQuality;
					g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
					DrawBackground(g);
					DrawRedTimeLineMain(g);
					g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;//We also round the location to preserve the font quality.
					DrawAppts(g);
				}
			}
		}

		private void SetBitmapOpsHeaders(Graphics gPrinting=null){
			if(_isValidHeaders && gPrinting==null){
				return;
			}
			if(_widthMain<1){
				_bitmapOpsHeader?.Dispose();
				_bitmapOpsHeader=null;
			}
			else if(gPrinting!=null){
				gPrinting.SmoothingMode=SmoothingMode.HighQuality;
				gPrinting.TextRenderingHint=TextRenderingHint.AntiAlias;
				DrawOpsHeader(gPrinting);
			}
			else{
				if(_bitmapOpsHeader==null){
					_bitmapOpsHeader=new Bitmap(_widthMain,(int)_heightProvOpHeaders);
				}
				else if(_bitmapOpsHeader.Size!=new Size(_widthMain,(int)_heightProvOpHeaders)){
					_bitmapOpsHeader.Dispose();
					_bitmapOpsHeader=new Bitmap(_widthMain,(int)_heightProvOpHeaders);
				}
				using(Graphics g=Graphics.FromImage(_bitmapOpsHeader)){
					g.SmoothingMode=SmoothingMode.HighQuality;
					g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
					DrawOpsHeader(g);
				}
			}
			_isValidHeaders=true;
		}
		
		private void SetBitmapProvbars(Graphics gPrinting=null){
			//Provbars------------------------------------------------------------------------------------------------
			if(_listProvsVisible.Count==0 || IsWeeklyView){
				_bitmapProvBars?.Dispose();
				_bitmapProvBars=null;
				_isValidMain=true;
				return;
			}
			else if(gPrinting!=null){
				gPrinting.SmoothingMode=SmoothingMode.HighQuality;
				gPrinting.FillRectangle(_brushClosed,0,0,_widthProv*_listProvsVisible.Count,_heightMain);
				DrawProvScheds(gPrinting);
				DrawProvBarsAppts(gPrinting);
				DrawGridLinesProv(gPrinting);
				DrawRedTimeLineProv(gPrinting);
			}
			else{
				if(_bitmapProvBars==null){
					_bitmapProvBars=new Bitmap((int)(_listProvsVisible.Count*_widthProv),_heightMain);
				}
				else if(_bitmapProvBars.Size!=new Size((int)(_listProvsVisible.Count*_widthProv),_heightMain)){
					_bitmapProvBars.Dispose();
					_bitmapProvBars=new Bitmap((int)(_listProvsVisible.Count*_widthProv),_heightMain);
				}
				using(Graphics g=Graphics.FromImage(_bitmapProvBars)){
					g.SmoothingMode=SmoothingMode.HighQuality;
					g.FillRectangle(_brushClosed,0,0,_widthProv*_listProvsVisible.Count,_heightMain);
					DrawProvScheds(g);
					DrawProvBarsAppts(g);
					DrawGridLinesProv(g);
					DrawRedTimeLineProv(g);
				}
			}
			_isValidMain=true;
		}

		///<summary>Does nothing unless _bitmapProvBarsHeader or _bitmapOpsHeader needs a redraw.</summary>
		private void SetBitmapProvBarsHeaders(Graphics gPrinting=null){
			if(_isValidHeaders && gPrinting==null){
				return;
			}
			if(_listProvsVisible.Count==0){
				_bitmapProvBarsHeader?.Dispose();
				_bitmapProvBarsHeader=null;
			}
			else if(gPrinting!=null){
				gPrinting.SmoothingMode=SmoothingMode.HighQuality;
				DrawProvBarsHeader(gPrinting);
			}
			else{
				if(_bitmapProvBarsHeader==null){
					_bitmapProvBarsHeader=new Bitmap((int)(_widthProv*_listProvsVisible.Count),(int)_heightProvOpHeaders);
				}
				else if(_bitmapProvBarsHeader.Size!=new Size((int)(_widthProv*_listProvsVisible.Count),(int)_heightProvOpHeaders)){
					_bitmapProvBarsHeader.Dispose();
					_bitmapProvBarsHeader=new Bitmap((int)(_widthProv*_listProvsVisible.Count),(int)_heightProvOpHeaders);
				}
				using(Graphics g=Graphics.FromImage(_bitmapProvBarsHeader)){
					g.SmoothingMode=SmoothingMode.HighQuality;
					DrawProvBarsHeader(g);
				}
			}
		}

		///<summary></summary>
		private void SetBitmapTempAppt(){
			//no isValid to track. Just call this to redraw the bitmap
			//See TempApptSingle_Paint, which simply paints the bitmap at 0,0, then draws a lower border.
			_bitmapTempApptSingle?.Dispose();
			int heightTemp=0;
			if(_isResizingAppt){
				heightTemp=1000;//8hrs x 12incr x 12pix = 1152
			}
			else{
				heightTemp=contrTempAppt.Height;
			}
			_bitmapTempApptSingle=new Bitmap(contrTempAppt.Width,heightTemp);//contrTempAppt.Height);
			string patternShowing=GetPatternShowing(_dataRowTempAppt["Pattern"].ToString());
			//make the pattern arbitrarily long so that appt won't ever get cut off
			if(patternShowing.Length<96){//8hrs x 12 increments
				patternShowing=patternShowing.PadRight(96,'/');
			}
			using(Graphics g = Graphics.FromImage(_bitmapTempApptSingle)){
				g.SmoothingMode=SmoothingMode.HighQuality;
				DrawOneAppt(g,_dataRowTempAppt,patternShowing,contrTempAppt.Width-1,heightTemp-1,false);
			}
		}

		///<summary></summary>
		private void SetBitmapTimebarL(Graphics gPrinting=null){
			//if(_isValidTimebars){//no longer test so that red time line will draw
			//	return;
			//}
			Size sizeTimebar=new Size((int)_widthTime,(int)(_heightLine*24*_rowsPerHr)+1);
			if(gPrinting!=null){
				gPrinting.SmoothingMode=SmoothingMode.HighQuality;
				gPrinting.TextRenderingHint=TextRenderingHint.AntiAlias;
				DrawTimebar(gPrinting,true);
				DrawRedTimeLineTimeBar(gPrinting);
			}
			else{
				if(_bitmapTimebarLeft==null){
					_bitmapTimebarLeft=new Bitmap((int)_widthTime,(int)(_heightLine*24*_rowsPerHr)+1);
				}
				else if(_bitmapTimebarLeft.Size!=sizeTimebar){
					_bitmapTimebarLeft.Dispose();
					_bitmapTimebarLeft=new Bitmap((int)_widthTime,(int)(_heightLine*24*_rowsPerHr)+1);
				}
				using(Graphics g=Graphics.FromImage(_bitmapTimebarLeft)){
					g.SmoothingMode=SmoothingMode.HighQuality;
					g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
					DrawTimebar(g,true);
					DrawRedTimeLineTimeBar(g);
				}
			}
		}

		///<summary></summary>
		private void SetBitmapTimebarR(Graphics gPrinting=null){
			//if(_isValidTimebars){//no longer test so that red time line will draw
			//	return;
			//}
			Size sizeTimebar=new Size((int)_widthTime,(int)(_heightLine*24*_rowsPerHr)+1);
			if(gPrinting!=null){
				gPrinting.SmoothingMode=SmoothingMode.HighQuality;
				gPrinting.TextRenderingHint=TextRenderingHint.AntiAlias;
				DrawTimebar(gPrinting,false);
				DrawRedTimeLineTimeBar(gPrinting);
			}
			else{
				if(_bitmapTimebarRight==null){
					_bitmapTimebarRight=new Bitmap((int)_widthTime,(int)(_heightLine*24*_rowsPerHr)+1);
				}
				else if(_bitmapTimebarRight.Size!=sizeTimebar){
					_bitmapTimebarRight.Dispose();
					_bitmapTimebarRight=new Bitmap((int)_widthTime,(int)(_heightLine*24*_rowsPerHr)+1);
				}
				using(Graphics g=Graphics.FromImage(_bitmapTimebarRight)){
					g.SmoothingMode=SmoothingMode.HighQuality;
					g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
					DrawTimebar(g,false);
					DrawRedTimeLineTimeBar(g);
				}
			}
			//_isValidTimebars=true;
		}
		#endregion Methods - Private Set Bitmaps

		#region Methods - Private DrawBackground
		///<summary></summary>
		private void DrawBackground(Graphics g) {
			g.FillRectangle(_brushBackground,0,0,_widthMain,_heightMain);
			DrawMainBackground(g);
			DrawBlockouts(g);
			DrawWebSchedASAPSlots(g);
			DrawGridLinesMain(g);
		}

		private void DrawBlockouts(Graphics g){
			//if(_listSchedules==null){
			Schedule[] schedForType=Schedules.GetForType(ListSchedules,ScheduleType.Blockout,0);
			SolidBrush brushBlockBackg;//gets disposed here after each loop through i blockoutType
			Pen penOutline;//gets disposed here after each loop through i blockoutType
			string blockText;
			RectangleF rect;
			for(int i=0;i<schedForType.Length;i++) {
				brushBlockBackg=new SolidBrush(Defs.GetColor(DefCat.BlockoutTypes,schedForType[i].BlockoutType));
				penOutline=new Pen(Defs.GetColor(DefCat.BlockoutTypes,schedForType[i].BlockoutType),2);
				blockText=Defs.GetName(DefCat.BlockoutTypes,schedForType[i].BlockoutType)+"\r\n"+schedForType[i].Note;
				for(int o=0;o<schedForType[i].Ops.Count;o++) {
					if(IsWeeklyView) {
						if(GetIndexOp(schedForType[i].Ops[o])==-1) {
							continue;//don't display if op not visible
						}
						//this is a workaround because we start on Monday:
						int dayofweek=(int)schedForType[i].SchedDate.DayOfWeek-1;
						if(dayofweek==-1) {
							dayofweek=6;
						}
						rect=new RectangleF(
							(dayofweek)*_widthWeekDay
							+_widthWeekAppt*(GetIndexOp(schedForType[i].Ops[o],ListOpsVisible))
							,schedForType[i].StartTime.Hours*_heightLine*_rowsPerHr
							+schedForType[i].StartTime.Minutes*_heightLine/_minPerRow
							,_widthWeekAppt-1
							,(schedForType[i].StopTime-schedForType[i].StartTime).Hours*_heightLine*_rowsPerHr
							+(schedForType[i].StopTime-schedForType[i].StartTime).Minutes*_heightLine/_minPerRow);
					}
					else {
						if(GetIndexOp(schedForType[i].Ops[o])==-1) {
							continue;//don't display if op not visible
						}
						rect=new RectangleF(
							_widthOpCol*(GetIndexOp(schedForType[i].Ops[o],ListOpsVisible))
							+WidthProvOnAppt*2//so they don't overlap prov bars
							,schedForType[i].StartTime.Hours*_heightLine*_rowsPerHr
							+schedForType[i].StartTime.Minutes*_heightLine/_minPerRow
							,_widthOpCol-1-WidthProvOnAppt*2
							,(schedForType[i].StopTime-schedForType[i].StartTime).Hours*_heightLine*_rowsPerHr
							+(schedForType[i].StopTime-schedForType[i].StartTime).Minutes*_heightLine/_minPerRow);
					}
					//paint either solid block or outline
					if(PrefC.GetBool(PrefName.SolidBlockouts)) {
						g.FillRectangle(brushBlockBackg,rect);
						g.DrawLine(Pens.Black,rect.X,rect.Y+1,rect.Right-1,rect.Y+1);
					}
					else {
						g.DrawRectangle(penOutline,rect.X+1,rect.Y+2,rect.Width-2,rect.Height-3);
					}
					DrawStringWithSpacing(g,blockText,_font,_brushBlockText,rect,_heightLine);
					//g.DrawString(blockText,_font,_brushBlockText,rect);
				}//for o ops
				brushBlockBackg.Dispose();
				penOutline.Dispose();
			}//for i blockoutType
		}

		private void DrawGridLinesMain(Graphics g){
			//Vert
			if(IsWeeklyView) {
				float widthOp=_widthWeekDay/_listOpsVisible.Count;
				for(int d=0;d<_numOfWeekDaysToDisplay;d++) {
					g.DrawLine(_penVertPrimary,_widthWeekDay*d,0,_widthWeekDay*d,_heightMain);
					for(float i=0;i<_listOpsVisible.Count;i++) {
						//draw the op lines for the week view to avoid phantom gaps between ops
						g.DrawLine(Pens.Silver,_widthWeekDay*d+widthOp*i,0,_widthWeekDay*d+widthOp*i,_heightMain);
					}
				}
			}
			else {
				for(float i=0;i<_listOpsVisible.Count;i++) {
					g.DrawLine(_penVertPrimary,_widthOpCol*i,0,_widthOpCol*i,_heightMain);
				}
			}
			//horiz gray
			for(float i=0;i<_heightMain;i+=_heightLine*RowsPerIncr) {
				g.DrawLine(_penHorizMinutes,0,i,_widthMain,i);
			}
			//horiz Hour lines
			for(float i=0;i<_heightMain;i+=_heightLine*_rowsPerHr) {
				g.DrawLine(_penHorizMinutes,0,i-1,_widthMain,i-1);
				//the line below is black in the main area and dark gray in the timebar areas
				g.DrawLine(_penHorizBlack,0,i,_widthMain,i);
			}
		}

		///<summary>Including the practice schedule, prov schedules in main area, and blockouts.</summary>
		private void DrawMainBackground(Graphics g) {
			List<Schedule> provSchedsForOp;
			//one giant rectangle for everything closed
			g.FillRectangle(_brushClosed,0,0,_widthMain,_heightMain);
			if(ListSchedules==null){
				return;
			}
			//then, loop through each day and operatory
			//int startHour=startTime.Hour;
			if(IsWeeklyView) {
				for(int d=0;d<_numOfWeekDaysToDisplay;d++) {//for each day of the week displayed
					//if any schedule for this day is type practice and status holiday and not assigned to specific ops (so either clinics are not enabled
					//or the holiday applies to all ops for the clinic), color all of the op columns in the view for this day with the holiday brush
					if(ListSchedules.FindAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday) //find all holidays
						.Any(x => (int)x.SchedDate.DayOfWeek==d+1 //for this day of the week
							&& (x.ClinicNum==0 || (PrefC.HasClinicsEnabled && x.ClinicNum==Clinics.ClinicNum)))) //and either practice or for this clinic
					{
						g.FillRectangle(_brushHoliday,d*_widthWeekDay,0,_widthWeekDay,_heightMain);
					}
					//DayOfWeek enum goes from Sunday to Saturday 0-6, OD goes Monday to Sunday 0-6
					DayOfWeek dayofweek=(DayOfWeek)((d+1)%7);//when d==0, dayofweek=monday (or (DayOfWeek)1).  when d==6 we want dayofweek=sunday (or (DayOfWeek)0)
					for(int i=0;i<ListOpsVisible.Count;i++) {//for each operatory visible for this view and day
						provSchedsForOp=Schedules.GetProvSchedsForOp(ListSchedules,dayofweek,ListOpsVisible[i]);
						//for each schedule assigned to this op
						foreach(Schedule schedCur in provSchedsForOp.Where(x => x.SchedType==ScheduleType.Provider)) {
							g.FillRectangle(_brushOpen
								,d*_widthWeekDay+i*_widthWeekAppt
								,schedCur.StartTime.Hours*_heightLine*_rowsPerHr+schedCur.StartTime.Minutes*_heightLine/_minPerRow//RowsPerHr=6, _minPerRow=10
								,_widthWeekAppt
								,(schedCur.StopTime-schedCur.StartTime).Hours*_heightLine*_rowsPerHr+(schedCur.StopTime-schedCur.StartTime).Minutes*_heightLine/_minPerRow);
						}
					}
				}
			}
			else {//only one day showing
				//if any schedule for the period is type practice and status holiday and either ClinicNum is 0 (HQ clinic or clinics not enabled) or clinics
				//are enabled and the schedule.ClinicNum is the currently selected clinic
				//SchedListPeriod contains scheds for only one day, not for a week
				if(ListSchedules.FindAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday) //find all holidays
					.Any(x => x.ClinicNum==0 || (PrefC.HasClinicsEnabled && x.ClinicNum==Clinics.ClinicNum)))//for the practice or clinic
				{
					g.FillRectangle(_brushHoliday,0,0,_widthOpCol*ListOpsVisible.Count,_heightMain);
				}
				for(int i=0;i<ListOpsVisible.Count;i++) {//ops per page in day view
					if(i==ListOpsVisible.Count) {//for printing, when _countCol is a smaller per-page loop
						break;
					}
					provSchedsForOp=Schedules.GetProvSchedsForOp(ListSchedules,ListOpsVisible[i]);
					foreach(Schedule schedCur in provSchedsForOp) {
						if(schedCur.StartTime.Hours>=24) {
							continue;
						}
						g.FillRectangle(_brushOpen
							,i*_widthOpCol
							,schedCur.StartTime.Hours*_heightLine*_rowsPerHr+schedCur.StartTime.Minutes*_heightLine/_minPerRow//6RowsPerHr 10_minPerRow
							,_widthOpCol
							,(schedCur.StopTime-schedCur.StartTime).Hours*_heightLine*_rowsPerHr//6
								+(schedCur.StopTime-schedCur.StartTime).Minutes*_heightLine/_minPerRow);//10
					}
					//now, fill up to 2 timebars along the left side of each rectangle.
					foreach(Schedule schedCur in provSchedsForOp) {
						if(schedCur.Ops.Count==0) {//if this schedule is not assigned to specific ops, skip
							continue;
						}
						float provWidthAdj=0f;//use to adjust for primary and secondary provider bars in op
						if(Providers.GetIsSec(schedCur.ProvNum)) {
							provWidthAdj=WidthProvOnAppt;//drawing secondary prov bar so shift right
						}
						using(SolidBrush solidBrushProvColor=new SolidBrush(Providers.GetColor(schedCur.ProvNum))) {
							g.FillRectangle(solidBrushProvColor
								,i*_widthOpCol+provWidthAdj
								,schedCur.StartTime.Hours*_heightLine*_rowsPerHr+schedCur.StartTime.Minutes*_heightLine/_minPerRow//6RowsPerHr 10MinPerRow
								,WidthProvOnAppt
								,(schedCur.StopTime-schedCur.StartTime).Hours*_heightLine*_rowsPerHr//6
									+(schedCur.StopTime-schedCur.StartTime).Minutes*_heightLine/_minPerRow);//10
						}
					}
				}
			}
		}

		///<summary>There's no native way to change line spacing for DrawString, so this draws a block of text with any desired line spacing.  We don't currently bother to use this with appointment notes, so they can harmlessly look a little scrunched.</summary>
		private void DrawStringWithSpacing(Graphics g,string str,Font font,Brush brush,RectangleF rectangleLayout,float heightLine){
			StringFormat stringFormatLeft=new StringFormat();
			stringFormatLeft.Alignment=StringAlignment.Near;
			stringFormatLeft.Trimming=StringTrimming.Word;
			RectangleF rectangleRemaining=rectangleLayout;//the top gets sequentially chopped off this rectangle
			string strRemaining=str;
			while(true){
				SizeF sizeOneRow=new SizeF(rectangleLayout.Width,heightLine);
				//we don't care about the size returned here, just the outs
				g.MeasureString(strRemaining,font,sizeOneRow,stringFormatLeft,out int charactersFitted,out int linesFilled);
				if(linesFilled==0 || charactersFitted==0){//can happen when half height rows can't draw			
					stringFormatLeft.Dispose();
					return;
				}
				//draw one row
				string strDraw=strRemaining.Substring(0,charactersFitted).Trim();//trailing CR would cause text to shift up by 1px, so trim.
				g.DrawString(strDraw,font,brush,rectangleRemaining,stringFormatLeft);
				if(strRemaining.Length==charactersFitted || rectangleRemaining.Y+heightLine>=rectangleLayout.Bottom){
					stringFormatLeft.Dispose();
					return;
				}
				rectangleRemaining=new RectangleF(rectangleRemaining.X,rectangleRemaining.Y+heightLine,rectangleRemaining.Width,rectangleRemaining.Height-heightLine);
				strRemaining=strRemaining.Substring(charactersFitted);				
			}
		}

		private void DrawWebSchedASAPSlots(Graphics g){
			Schedule[] schedulesForWebSchedASAP=Schedules.GetForType(ListSchedules,ScheduleType.WebSchedASAP,0);
			Pen penOutline=new Pen(Color.LightYellow,2);
			string txtBlock;
			RectangleF rectF;
			for(int i=0;i<schedulesForWebSchedASAP.Length;i++) {
				txtBlock=Lan.g(this,"Web Sched ASAP Slot")+"\r\n"+schedulesForWebSchedASAP[i].Note;
				for(int o=0;o<schedulesForWebSchedASAP[i].Ops.Count;o++) {
					TimeSpan timeSpan=schedulesForWebSchedASAP[i].StopTime-schedulesForWebSchedASAP[i].StartTime;
					if(IsWeeklyView) {
						if(GetIndexOp(schedulesForWebSchedASAP[i].Ops[o])==-1) {
							continue;//don't display if op not visible
						}
						//this is a workaround because we start on Monday:
						int dayofweek=(int)schedulesForWebSchedASAP[i].SchedDate.DayOfWeek-1;
						if(dayofweek==-1) {
							dayofweek=6;
						}
						rectF=new RectangleF(
							(dayofweek)*_widthWeekDay
							+_widthWeekAppt*(GetIndexOp(schedulesForWebSchedASAP[i].Ops[o],ListOpsVisible))//-(PrintingColsPerPage*PrintingPageColumn))
							,schedulesForWebSchedASAP[i].StartTime.Hours*_heightLine*_rowsPerHr
							+schedulesForWebSchedASAP[i].StartTime.Minutes*_heightLine/_minPerRow
							,_widthWeekAppt-5 //Narrowed so that blockouts will be visible underneath
							,timeSpan.Hours*_heightLine*_rowsPerHr
							+timeSpan.Minutes*_heightLine/_minPerRow);
					}
					else {//Daily view
						if(GetIndexOp(schedulesForWebSchedASAP[i].Ops[o])==-1) {
							continue;//don't display if op not visible
						}
						rectF=new RectangleF(
							_widthOpCol*(GetIndexOp(schedulesForWebSchedASAP[i].Ops[o],ListOpsVisible))//-(PrintingColsPerPage*PrintingPageColumn))
							+WidthProvOnAppt*2//so they don't overlap prov bars
							,schedulesForWebSchedASAP[i].StartTime.Hours*_heightLine*_rowsPerHr
							+schedulesForWebSchedASAP[i].StartTime.Minutes*_heightLine/_minPerRow
							,_widthOpCol-8-WidthProvOnAppt*2 //Shortened so that blockouts will be visible underneath
							,timeSpan.Hours*_heightLine*_rowsPerHr
							+timeSpan.Minutes*_heightLine/_minPerRow);
					}
					g.FillRectangle(Brushes.LightYellow,rectF);
					g.DrawLine(Pens.Black,rectF.X,rectF.Y+1,rectF.Right-1,rectF.Y+1);
					FillWithDiagonalLines(g,rectF,Pens.LightGray,12);
					DrawStringWithSpacing(g,txtBlock,_font,_brushBlockText,rectF,_heightLine);
					//g.DrawString(txtBlock,_font,_brushBlockText,rectF);
				}
				penOutline.Dispose();
			}
		}

		///<summary>Fills a rectangle with diagonal lines.</summary>
		private void FillWithDiagonalLines(Graphics g,RectangleF rectToFill,Pen linePen,float pixelsBetweenLines) {
			//Walk along the bottom of the rectangle and draw lines that go to the left side or the top.
			for(float x=rectToFill.X+pixelsBetweenLines;x<rectToFill.Right;x+=pixelsBetweenLines) {
				float y2=rectToFill.Bottom-(x-rectToFill.X);
				float x2=rectToFill.Left+Math.Max(rectToFill.Top-y2,0);
				if(y2 < rectToFill.Top) {
					y2=rectToFill.Top;
				}
				g.DrawLine(linePen,x,rectToFill.Bottom,x2,y2);
			}
			//Walk along the right side of the rectangle a draw lines that go to the top or left side.
			float offsetY=pixelsBetweenLines-rectToFill.Width%pixelsBetweenLines;
			for(float y=rectToFill.Bottom-offsetY;y>rectToFill.Top;y-=pixelsBetweenLines) {
				float x2=rectToFill.Right-(y-rectToFill.Top);
				float y2=rectToFill.Top+Math.Max(rectToFill.Left-x2,0);
				if(x2 < rectToFill.Left) {
					x2=rectToFill.Left;
				}
				g.DrawLine(linePen,rectToFill.Right,y,x2,y2);
			}
		}

		///<summary>Returns the index of the opNum within VisOps.  Returns -1 if not in VisOps.</summary>
		private int GetIndexOp(long opNum,List<Operatory> VisOps) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<VisOps.Count;i++) {
				if(VisOps[i].OperatoryNum==opNum)
					return i;
			}
			return -1;
		}
		#endregion Methods - Private DrawBackground

		#region Methods - Private DrawProvBars
		///<summary>Draws gridlines in the prov bars area.</summary>
		private void DrawGridLinesProv(Graphics g){
			if(IsWeeklyView) {
				for(int d=0;d<_numOfWeekDaysToDisplay;d++) {
					g.DrawLine(_penVertPrimary,_widthWeekDay*d,0,_widthWeekDay*d,_heightMain);
				}
			}
			else {//only one day showing
				for(int i=0;i<_listProvsVisible.Count;i++) {
					g.DrawLine(_penVertPrimary,_widthProv*i,0,_widthProv*i,_heightMain);
				}
			}
			//horiz gray
			for(float i=0;i<_heightMain;i+=_heightLine*RowsPerIncr) {
				g.DrawLine(_penHorizMinutes,0,i,_widthProv*_listProvsVisible.Count,i);
			}
			//horiz Hour lines
			for(float i=0;i<_heightMain;i+=_heightLine*_rowsPerHr) {
				g.DrawLine(_penHorizMinutes,0,i-1,_widthProv*_listProvsVisible.Count,i-1);
				//the line below is black in the main area and dark gray in the timebar areas
				g.DrawLine(_penHorizBlack,0,i,_widthProv*_listProvsVisible.Count,i);
			}
		}		
		
		///<summary>This draws the squares for actual appointments onto the prov bars area.</summary>
		private void DrawProvBarsAppts(Graphics g){
			if(!IsWeeklyView 
				&& !_isPrinting) //printing does a loop for each page, which is too much shading
			{
				_provBar=new int[ListProvsVisible.Count,24*(int)(60/_minPerIncr)];//_rowsPerHr
				foreach(DataRow dataRow in TableAppointments.Rows) {
					ProvBarShading(dataRow);
				}
			}
			if(_provBar.Length<ListProvsVisible.Count) {
				return;
			}
			for(int j=0;j<ListProvsVisible.Count;j++){//_provBar.Length;j++) {
				//loop through increments.  If two rowsPerIncr, then the loop will handle both at the same time
				for(int i=0;i<24*(int)(60/_minPerIncr);i++){//_rowsPerHr
					RectangleF rectFill=new RectangleF(_widthProv*j+1,_heightLine*RowsPerIncr*i+1,_widthProv-1,_heightLine*RowsPerIncr-1);
					switch(_provBar[j,i]) {
						case 0:
							break;
						case 1:
							if(DesignMode){
								g.FillRectangle(Brushes.White,rectFill);
							}
							else{
								using(SolidBrush solidBrushProvColor=new SolidBrush(ListProvsVisible[j].ProvColor)) {
									g.FillRectangle(solidBrushProvColor,rectFill);
								}
							}
							break;
						case 2:
							using(HatchBrush hatchBrushProvColor=new HatchBrush(HatchStyle.DarkUpwardDiagonal,Color.Black,ListProvsVisible[j].ProvColor)) {
								g.FillRectangle(hatchBrushProvColor,rectFill);
							}
							break;
						default://more than 2
							g.FillRectangle(_brushLinesAndText,rectFill);
							break;
					}
				}
			}
		}		
		
		///<summary>This is just for schedules in the prov bars area.</summary>
		private void DrawProvScheds(Graphics g){
			Schedule[] schedulesForProv;
			for(int j=0;j<ListProvsVisible.Count;j++) {
				schedulesForProv=Schedules.GetForType(ListSchedules,ScheduleType.Provider,ListProvsVisible[j].ProvNum);
				for(int i=0;i<schedulesForProv.Length;i++) {
					g.FillRectangle(_brushOpen
						,_widthProv*j
						,schedulesForProv[i].StartTime.Hours*_heightLine*_rowsPerHr//6
						+schedulesForProv[i].StartTime.Minutes*_heightLine/_minPerRow//10
						,_widthProv
						,(schedulesForProv[i].StopTime-schedulesForProv[i].StartTime).Hours*_heightLine*_rowsPerHr//6
						+(schedulesForProv[i].StopTime-schedulesForProv[i].StartTime).Minutes*_heightLine/_minPerRow);//10
				}
			}
		}
		#endregion Methods - Private DrawProvBars

		#region Methods - Private Draw ProvAndOp Headers
		private void DrawOpsHeader(Graphics g){
			g.FillRectangle(_brushBackground,0,0,_widthMain,_heightProvOpHeaders);
			if(IsWeeklyView) {
				for(int d=0;d<_numOfWeekDaysToDisplay;d++) {//for each day of the week displayed
					string strDay=DateStart.AddDays(d).ToString("dddd-d");//Wednesday-24
					float widthText=g.MeasureString(strDay,_fontOpsHeaders).Width;
					float xPos=d*_widthWeekDay+_widthWeekDay/2-widthText/2;//centered
					g.DrawString(strDay,_fontOpsHeaders,_brushTextBlack,new RectangleF(xPos,2,widthText,_heightProvOpHeaders-2));
					g.DrawLine(_penVertPrimary,d*_widthWeekDay,0,d*_widthWeekDay,_heightProvOpHeaders);//to left of op cell
				}
			}
			else{//only one day showing
				for(int i=0;i<_listOpsVisible.Count;i++) {//ops per page in day view
					if(i==ListOpsVisible.Count) {//for printing
						break;
					}
					Color colorOp=Color.Empty;
					//operatory color is based solely on provider colors and IsHygiene.  It has nothing to do with schedules
					if(ListOpsVisible[i].ProvDentist!=0 && !ListOpsVisible[i].IsHygiene) {
						colorOp=Providers.GetColor(ListOpsVisible[i].ProvDentist);
					}
					else if(ListOpsVisible[i].ProvHygienist!=0 && ListOpsVisible[i].IsHygiene) {
						colorOp=Providers.GetColor(ListOpsVisible[i].ProvHygienist);
					}
					if(colorOp!=Color.Empty){
						using(SolidBrush solidBrushProvColor=new SolidBrush(colorOp)) {
							g.FillRectangle(solidBrushProvColor,i*_widthOpCol,0,_widthOpCol,_heightProvOpHeaders);
						}
					}
					SizeF sizeTextMeasured=g.MeasureString(ListOpsVisible[i].OpName,_fontOpsHeaders);
					float widthText=sizeTextMeasured.Width;
					float xPos;
					if(widthText>_widthOpCol-2){
						xPos=i*_widthOpCol+1;//left side of column
						widthText=_widthOpCol-2;
					}
					else{
						xPos=i*_widthOpCol+(_widthOpCol/2f)-(widthText/2f);//centered
					}
					PointF locationText=new PointF(xPos,2);
					float heightText=_heightProvOpHeaders-2;
					g.DrawString(ListOpsVisible[i].OpName,_fontOpsHeaders,_brushTextBlack,new RectangleF(locationText.X,locationText.Y,widthText,heightText));
					g.DrawLine(_penVertPrimary,i*_widthOpCol,0,i*_widthOpCol,_heightProvOpHeaders);//to left of op cell
				}
			}
			//outline
			//g.DrawRectangle(_penVertPrimary,0,0,_width-1,_heightProvOpHeaders-1);
			//g.DrawLine(_penVertPrimary,_widthMain-1,0,_widthMain-1,_heightProvOpHeaders);//on right//now handled in OnPaint
		}

		private void DrawProvBarsHeader(Graphics g){
			g.FillRectangle(_brushBackground,0,0,_widthProv*_listProvsVisible.Count,_heightProvOpHeaders);
			for(int i=0;i<ListProvsVisible.Count;i++){
				using(SolidBrush solidBrushProvColor=new SolidBrush(ListProvsVisible[i].ProvColor)) {
					g.FillRectangle(solidBrushProvColor,_widthProv*i,0,_widthProv,_heightProvOpHeaders);
				}
				g.DrawLine(_penVertPrimary,_widthProv*i,0,_widthProv*i,_heightProvOpHeaders);//to left of prov cell
			}
			//outline
			//g.DrawRectangle(_penVertPrimary,0,0,_provWidth*_provCount-1,_heightProvOpHeaders-1);
		}
		#endregion Methods - Private Draw ProvAndOp Headers

		#region Methods - Private DrawTimebar	
		///<summary>Draws either left or right timebar minutes</summary>
		private void DrawMinutes(Graphics g,bool isLeft){
			DateTime hour;
			CultureInfo ci=(CultureInfo)CultureInfo.CurrentCulture.Clone();
			string hFormat=Lans.GetShortTimeFormat(ci);
			string strTime;
			int yHour=0;//This will cause drawing times to always start at the top.
			float xPos;
			for(int i=0;i<24;i++) {
				hour=new DateTime(2000,1,1,i,0,0);//hour is the only important part of this time.
				strTime=hour.ToString(hFormat,ci);
				strTime=strTime.Replace("a. m.","am");//So that the times are not cut off for foreign users
				strTime=strTime.Replace("p. m.","pm");
				SizeF sizef=g.MeasureString(strTime,_fontMinutesBold);
				xPos=1;//align left
				if(isLeft){
					xPos=_widthTime-sizef.Width-2;//align right
				}
				g.DrawString(strTime,_fontMinutesBold,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+2);
				if(isLeft){
					xPos=_widthTime-LayoutManager.ScaleF(19)*_sizeFont/8f;//align right
				}
				if(MinPerIncr==5) {
					g.DrawString(":15",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*3+1);
					g.DrawString(":30",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*6+1);
					g.DrawString(":45",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*9+1);
				}
				else if(MinPerIncr==10) {
					g.DrawString(":10",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr+1);
					g.DrawString(":20",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*2+1);
					g.DrawString(":30",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*3+1);
					g.DrawString(":40",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*4+1);
					g.DrawString(":50",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*5+1);
				}
				else {//15
					g.DrawString(":15",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr+1);
					g.DrawString(":30",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*2+1);
					g.DrawString(":45",_fontMinutes,_brushLinesAndText,xPos,yHour*_heightLine*_rowsPerHr+_heightLine*RowsPerIncr*3+1);
				}
				yHour++;
			}
		}		
		
		///<summary></summary>
		private void DrawTimebar(Graphics g,bool isLeft) {
			g.FillRectangle(_brushTimeBar,0,0,_widthTime,_heightMain);
			DrawTimebarLines(g,isLeft);
			DrawMinutes(g,isLeft);
		}

		///<summary></summary>
		private void DrawTimebarLines(Graphics g,bool isLeft){
			//horiz Hour lines
			for(float i=0;i<_heightMain;i+=_heightLine*_rowsPerHr*RowsPerIncr) {
				g.DrawLine(_penHorizMinutes,0,i-1,_widthTime,i-1);
				//the line below is black in the main area and dark gray in the timebar areas
				g.DrawLine(_penHorizDark,0,i,_widthTime,i);
			}
			if(isLeft){
				//g.DrawLine(_penVertPrimary,_widthTime-1,0,_widthTime-1,_heightMain);//right side
			}
			else{
				//g.DrawLine(_penVertPrimary,0,0,0,_heightMain);//left side
			}
		}
		#endregion Methods - Private DrawTimebar

		#region Methods - Private DrawAppts
		/// <summary></summary>
		private void DrawAppts(Graphics g){
			_listApptLayoutInfos=PlaceAppointments(TableAppointments,ListOpsVisible,_heightLine,IsWeeklyView,
				MinPerIncr,_numOfWeekDaysToDisplay,RowsPerIncr,_widthMain,_widthNarrowedOnRight,
				DateStart,DateEnd);
			for(int i=0;i<TableAppointments.Rows.Count;i++){
				DataRow dataRow=TableAppointments.Rows[i];
				if(_listApptLayoutInfos[i].RectangleBounds.Width==0){
					continue;//Appointment is outside of our date range.
				}
				string pattern=PIn.String(dataRow["Pattern"].ToString());
				string patternShowing=GetPatternShowing(pattern);
				RectangleF rect=_listApptLayoutInfos[i].RectangleBounds;
				//When printing, there's already a clip in place for the section of the background we are grabbing.
				//Translation also gets reset after each appointment.
				GraphicsState graphicsState=g.Save();
				g.TranslateTransform(rect.X,rect.Y);
				g.SetClip(new RectangleF(0,0,rect.Width+1,rect.Height+1),CombineMode.Intersect);
				DrawOneAppt(g,dataRow,patternShowing,rect.Width,rect.Height);
				g.Restore(graphicsState);//back to the way it was 4 lines up
			}
		}

		///<summary></summary>
		private void DrawOneAppt(Graphics g,DataRow dataRoww,string patternShowing,float widthAppt,float heightAppt,bool showBrokenX=true){
			SolidBrush brushProvBackground;
			SolidBrush brushProvBackground2;
			Color backColor;
			Color provColor;
			Color provColor2;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.AppointmentColors);
			if(dataRoww["ProvNum"].ToString()!="0" && dataRoww["IsHygiene"].ToString()=="0") {//dentist
				provColor=Providers.GetColor(PIn.Long(dataRoww["ProvNum"].ToString()));
				provColor2=Providers.GetColor(PIn.Long(dataRoww["ProvHyg"].ToString()));
			}
			else if(dataRoww["ProvHyg"].ToString()!="0" && dataRoww["IsHygiene"].ToString()=="1") {//hygienist
				provColor=Providers.GetColor(PIn.Long(dataRoww["ProvHyg"].ToString()));
				provColor2=Providers.GetColor(PIn.Long(dataRoww["ProvNum"].ToString()));
			}
			else {//unknown
				provColor=Color.White;
				provColor2=Color.White;
			}
			brushProvBackground=new SolidBrush(provColor);
			brushProvBackground2=new SolidBrush(provColor2);
			backColor=provColor;//Default the appointment to the primary provider's color.
			if(PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.Complete) {
				backColor=listDefs[2].ItemColor;
			}
			else if(PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.PtNote) {
				backColor=listDefs[5].ItemColor;
				if(PIn.Int(dataRoww["ColorOverride"].ToString()) != 0) {//Patient note has an override.
					backColor=Color.FromArgb(PIn.Int(dataRoww["ColorOverride"].ToString()));
				}
			}
			else if(PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.PtNoteCompleted) {
				backColor=listDefs[6].ItemColor;
			}
			else if(PIn.Int(dataRoww["ColorOverride"].ToString()) != 0) {
				backColor=Color.FromArgb(PIn.Int(dataRoww["ColorOverride"].ToString()));
			}
			//Check to see if the patient is late for their appointment. 
			DateTime aptDateTime=PIn.DateT(dataRoww["AptDateTime"].ToString());
			DateTime aptDateTimeArrived=PIn.DateT(dataRoww["AptDateTimeArrived"].ToString());
			//If the appointment is scheduled and the patient was late for the appointment.
			if((PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.Scheduled)
				&& ((aptDateTimeArrived.TimeOfDay==TimeSpan.FromHours(0) && DateTime.Now>aptDateTime) 
					|| (aptDateTimeArrived.TimeOfDay>TimeSpan.FromHours(0) && aptDateTimeArrived>aptDateTime))) 
			{
				//Loop through all the appt view items to see if appointments should display the late color.
				for(int i=0;i<ListApptViewItemRowElements.Count;i++) {
					if(ListApptViewItemRowElements[i].ElementDesc=="LateColor") {
						backColor=ListApptViewItemRowElements[i].ElementColor;
						break;
					}
				}
			}
			SolidBrush brushBack=new SolidBrush(backColor);
			if(widthAppt<14){//Narrow appointments will look very simple, with no rounded corners or text
				g.FillRectangle(brushBack,0,0,widthAppt,heightAppt);
				if(widthAppt<6){//below 6, the black side lines are distracting
					g.DrawLine(_penBlack,0,0,widthAppt,0);
					g.DrawLine(_penBlack,0,heightAppt,widthAppt,heightAppt);
				}
				else{
					g.DrawRectangle(_penBlack,0,0,widthAppt,heightAppt);
				}
				DisposeObjects(brushProvBackground,brushBack,brushProvBackground2);
				return;
			}
			GraphicsPath graphicsPathRightSide=new GraphicsPath();
			graphicsPathRightSide.AddLine(WidthProvOnAppt,0,widthAppt-_radiusCorn,0);//top
			graphicsPathRightSide.AddArc(widthAppt-_radiusCorn*2f,0,_radiusCorn*2,_radiusCorn*2,270,90);//UR
			graphicsPathRightSide.AddLine(widthAppt,_radiusCorn,widthAppt,heightAppt-_radiusCorn);//right
			graphicsPathRightSide.AddArc(widthAppt-_radiusCorn*2f,heightAppt-_radiusCorn*2,_radiusCorn*2,_radiusCorn*2,0,90);//LR
			graphicsPathRightSide.AddLine(widthAppt-_radiusCorn,heightAppt,WidthProvOnAppt,heightAppt);//bottom
			//graphicsPathRightSide.AddLine(8f,totalHeight,8f,0);//path doesn't include left
			GraphicsPath graphicsPathLeftSide=new GraphicsPath();
			graphicsPathLeftSide.AddLine(WidthProvOnAppt,heightAppt,_radiusCorn,heightAppt);//bottom
			graphicsPathLeftSide.AddArc(0,heightAppt-_radiusCorn*2,_radiusCorn*2,_radiusCorn*2,90,90);//LL
			graphicsPathLeftSide.AddLine(0,heightAppt-_radiusCorn,0,_radiusCorn);//left
			graphicsPathLeftSide.AddArc(0,0,_radiusCorn*2,_radiusCorn*2,180,90);//UL
			graphicsPathLeftSide.AddLine(_radiusCorn,0,WidthProvOnAppt,0);//top
			g.FillPath(brushBack,graphicsPathRightSide);
			g.FillPath(_brushWhite,graphicsPathLeftSide);
			//g.FillRectangle(_brushWhite,0,0,8f,totalHeight);
			for(int i=0;i<patternShowing.Length;i++) {//Info.MyApt.Pattern.Length;i++){
				float heightOverflow=0;//used to remove the overflowed line height in situations where an appt has 40 minutes scheduled on a 15 minute time increment
				if(patternShowing.Substring(i,1)=="X") {
					if(_heightLine*(i+1)>heightAppt) {
						heightOverflow=_heightLine*(i+1)-heightAppt;
					}
					g.FillRectangle(brushProvBackground,0.5f,i*_heightLine+0.5f,WidthProvOnAppt,_heightLine-heightOverflow);
				}
				else {
					//leave empty
				}
				if(i%RowsPerIncr==0){
					//Math.IEEERemainder((double)i,(double)RowsPerIncr)==0) {
					g.DrawLine(_penTimeDiv,1,i*_heightLine,WidthProvOnAppt,i*_heightLine);//horizontal line above cell
				}
			}
			//Get the second provider time pattern
			string pattern2=PIn.String(dataRoww["PatternSecondary"].ToString());
			string pattern2Showing=GetPatternShowing(pattern2);
			for(int i=0;i<patternShowing.Length;i++) {
				float heightOverflow=0;
				if(i>=pattern2Showing.Length) {
					break;
				}
				if(pattern2Showing.Substring(i,1)=="X" && patternShowing.Substring(i,1)=="X") {
					if(_heightLine*(i+1)>heightAppt) {
						heightOverflow=_heightLine*(i+1)-heightAppt;
					}
					g.FillRectangle(brushProvBackground2,(WidthProvOnAppt/2)-0.5f,i*_heightLine+0.5f,(WidthProvOnAppt/2),_heightLine-heightOverflow);
				}
				else if(pattern2Showing.Substring(i,1)=="X") {
					if(_heightLine*(i+1)>heightAppt) {
						heightOverflow=_heightLine*(i+1)-heightAppt;
					}
					g.FillRectangle(brushProvBackground2,0.5f,i*_heightLine+0.5f,WidthProvOnAppt,_heightLine-heightOverflow);
				}
				else {
					//leave empty
				}
				if(i%RowsPerIncr==0){
					g.DrawLine(_penTimeDiv,1,i*_heightLine,WidthProvOnAppt,i*_heightLine);//horizontal line above cell
				}
			}
			string provBarText=dataRoww["ProvBarText"].ToString();
			for(int i=0;i<provBarText.Length;i++) {
				if((i+1)*(_heightLine*RowsPerIncr)>heightAppt){
					break;
				}
				g.DrawString(provBarText.Substring(i,1),_fontCourier,Brushes.Black,1,i*_heightLine*RowsPerIncr);
			}
			g.DrawLine(_penBlack,WidthProvOnAppt,0,WidthProvOnAppt,heightAppt);
			#region Main rows
			PointF pointDraw=new PointF(WidthProvOnAppt+1,1);
			int elementI=0;
			while(pointDraw.Y<heightAppt && elementI<ListApptViewItemRowElements.Count) {
				if(ListApptViewItemRowElements[elementI].ElementAlignment!=ApptViewAlignment.Main){
					if(ListApptViewItemRowElements[elementI].ElementDesc!="ProcsColored") {//ProcsColored will behave differently than other patfields and will only be drawn in Main, even if in UR or LR
						elementI++;
						continue;
					}
				}
				pointDraw=DrawElement(g,elementI,pointDraw,ApptViewStackBehavior.Vertical,ApptViewAlignment.Main,dataRoww,ListApptViewItemRowElements,widthAppt,heightAppt);//set the drawLoc to a new point, based on space used by element
				elementI++;
			}
			#endregion
			#region UR
			pointDraw=new Point((int)widthAppt-1,0);//in the UR area, we refer to the upper right corner of each element.
			elementI=0;
			while(pointDraw.Y<heightAppt && pointDraw.X>0 && elementI<ListApptViewItemRowElements.Count) {
				if(ListApptViewItemRowElements[elementI].ElementAlignment!=ApptViewAlignment.UR || ListApptViewItemRowElements[elementI].ElementDesc=="ProcsColored") {
					elementI++;
					continue;
				}
				pointDraw=DrawElement(g,elementI,pointDraw,ApptViewCur.StackBehavUR,ApptViewAlignment.UR,dataRoww,ListApptViewItemRowElements,widthAppt,heightAppt);
				elementI++;
			}
			Plugins.HookAddCode(null,"OpenDentBusiness.UI.ApptSingleDrawing.DrawEntireAppt_UR",dataRoww,g,pointDraw);
			#endregion
			#region LR
			pointDraw=new Point((int)widthAppt-1,(int)heightAppt-1);//in the LR area, we refer to the lower right corner of each element.
			elementI=ListApptViewItemRowElements.Count-1;//For lower right, draw the list backwards.
			while(pointDraw.Y>0 && elementI>=0) {
				if(ListApptViewItemRowElements[elementI].ElementAlignment!=ApptViewAlignment.LR || ListApptViewItemRowElements[elementI].ElementDesc=="ProcsColored") {
					elementI--;
					continue;
				}
				pointDraw=DrawElement(g,elementI,pointDraw,ApptViewCur.StackBehavLR,ApptViewAlignment.LR,dataRoww,ListApptViewItemRowElements,widthAppt,heightAppt);
				elementI--;
			}
			#endregion
			g.DrawPath(_penBlack,graphicsPathRightSide);
			g.DrawPath(_penBlack,graphicsPathLeftSide);
			//broken X
			if(showBrokenX){//this gets skipped when dragging appt size
				if(dataRoww["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()) {
					g.DrawLine(_penBlack,WidthProvOnAppt,1,widthAppt-1,heightAppt-1);
					g.DrawLine(_penBlack,WidthProvOnAppt,heightAppt-1,widthAppt-1,1);
				}
			}
			DisposeObjects(brushProvBackground,brushBack,brushProvBackground2);
		}

		///<summary>Returns the point where the next element should draw.</summary>
		private PointF DrawElement(Graphics g,int idxItem,PointF pointDraw,ApptViewStackBehavior stackBehavior,ApptViewAlignment align,DataRow dataRoww,List<ApptViewItem> listApptViewItems,float widthAppt,float heightAppt) 
		{
			if(widthAppt<WidthProvOnAppt+5){//Appointment is too narrow to draw anything
				return new PointF(pointDraw.X,heightAppt);
			}
			string text="";
			bool isNote=false;
			bool isInsuranceColor=false;
			#region Fill Text
			if(PIn.Long(dataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNote
				|| PIn.Long(dataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNoteCompleted) {
				isNote=true;
			}
			bool isConfirmedCircle=false;
			bool isCareCreditStatus=false;
			if(listApptViewItems[idxItem].ElementDesc=="ConfirmedColor") {
				isConfirmedCircle=true;
			}
			if(listApptViewItems[idxItem].ApptFieldDefNum>0) {
				string fieldName=ApptFieldDefs.GetFieldName(listApptViewItems[idxItem].ApptFieldDefNum);
				for(int i=0;i<TableApptFields.Rows.Count;i++) {
					if(TableApptFields.Rows[i]["AptNum"].ToString()!=dataRoww["AptNum"].ToString()) {
						continue;
					}
					if(TableApptFields.Rows[i]["FieldName"].ToString()!=fieldName) {
						continue;
					}
					text=TableApptFields.Rows[i]["FieldValue"].ToString();
				}
			}
			else if(listApptViewItems[idxItem].PatFieldDefNum>0) {
				string fieldName=PatFieldDefs.GetFieldName(listApptViewItems[idxItem].PatFieldDefNum);
				string careCreditFieldName="";
				PatFieldDef patFieldDefCareCredit=PatFieldDefs.GetPatFieldCareCredit();
				if(patFieldDefCareCredit!=null) {
					careCreditFieldName=patFieldDefCareCredit.FieldName;
				}
				for(int i=0;i<TablePatFields.Rows.Count;i++) {
					if(TablePatFields.Rows[i]["PatNum"].ToString()!=dataRoww["PatNum"].ToString()) {
						continue;
					}
					if(TablePatFields.Rows[i]["FieldName"].ToString()!=fieldName) {
						continue;
					}
					if(TablePatFields.Rows[i]["FieldName"].ToString()==careCreditFieldName) {
						if(!IsValidCareCreditStatus(TablePatFields.Rows[i]["FieldValue"].ToString())) {
							continue;//Not a valid status. No need to draw anything. 
						}
						isCareCreditStatus=true;
					}
					text=TablePatFields.Rows[i]["FieldValue"].ToString();
				}
			}
			else switch(listApptViewItems[idxItem].GetElement()) {
					case EnumApptViewElement.Address:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["address"].ToString();
						}
						break;
					case EnumApptViewElement.AddrNote:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["addrNote"].ToString();
						}
						break;
					case EnumApptViewElement.Age:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["age"].ToString();
						}
						break;
					case EnumApptViewElement.ASAP:
						if(isNote) {
							text="";
						}
						else {
							if(PIn.Long(dataRoww["Priority"].ToString())==(int)ApptPriority.ASAP) {
								text=Lan.g("ContrAppt","ASAP");
							}
						}
						break;
					case EnumApptViewElement.ASAP_A:
						if(PIn.Long(dataRoww["Priority"].ToString())==(int)ApptPriority.ASAP) {
							text=Lan.g("ContrAppt","A");
						}
						break;
					case EnumApptViewElement.AssistantAbbr:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["assistantAbbr"].ToString();
						}
						break;
					case EnumApptViewElement.Birthdate:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["Birthdate"].ToString();
						}
						break;
					case EnumApptViewElement.ChartNumAndName:
						text=dataRoww["chartNumAndName"].ToString();
						break;
					case EnumApptViewElement.ChartNumber:
						text=dataRoww["chartNumber"].ToString();
						break;
					case EnumApptViewElement.CreditType:
						text=dataRoww["CreditType"].ToString();
						break;
					case EnumApptViewElement.DiscountPlan:
						text=dataRoww["discountPlan"].ToString();
						break;
					case EnumApptViewElement.EstPatientPortion:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["estPatientPortion"].ToString();
						}
						break;
					case EnumApptViewElement.Guardians:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["guardians"].ToString();
						}
						break;
					case EnumApptViewElement.HasDiscount_D:
						text=dataRoww["hasDiscount[D]"].ToString();
						break;
					case EnumApptViewElement.HasIns_I:
						text=dataRoww["hasIns[I]"].ToString();
						break;
					case EnumApptViewElement.HmPhone:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["hmPhone"].ToString();
						}
						break;
					case EnumApptViewElement.Insurance:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["insurance"].ToString();
						}
						break;
					case EnumApptViewElement.InsuranceColor:
						if(isNote) {
							text="";
						}
						else {
							isInsuranceColor=true;
							text=dataRoww["insurance"].ToString();
						}
						break;
					case EnumApptViewElement.InsToSend_excl:
						text=dataRoww["insToSend[!]"].ToString();
						break;
					case EnumApptViewElement.IsLate_L:
						DateTime aptDateTime=PIn.DateT(dataRoww["AptDateTime"].ToString());
						DateTime aptDateTimeArrived=PIn.DateT(dataRoww["AptDateTimeArrived"].ToString());
						//If the appointment is scheduled, complete, or ASAP and the patient was late for the appointment.
						if((PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.Scheduled
							|| PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.Complete)
							&& ((aptDateTimeArrived.TimeOfDay==TimeSpan.FromHours(0) && DateTime.Now>aptDateTime) 
								|| (aptDateTimeArrived.TimeOfDay>TimeSpan.FromHours(0) && aptDateTimeArrived>aptDateTime))) 
						{
							text="L";
						}
						break;
					case EnumApptViewElement.Lab:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["lab"].ToString();
						}
						break;
					case EnumApptViewElement.MedOrPremed_plus:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["medOrPremed[+]"].ToString();
						}
						break;
					case EnumApptViewElement.MedUrgNote:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["MedUrgNote"].ToString();
						}
						break;
					case EnumApptViewElement.NetProduction:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["netProduction"].ToString();
						}
						break;
					case EnumApptViewElement.Note:
						text=dataRoww["Note"].ToString();
						break;
					case EnumApptViewElement.PatientName:
						text=dataRoww["patientName"].ToString();
						break;
					case EnumApptViewElement.PatientNameF:
						text=dataRoww["patientNameF"].ToString();
						break;
					case EnumApptViewElement.PatientNamePref:
						text=dataRoww["PatientNamePref"].ToString();
						break;
					case EnumApptViewElement.PatNum:
						text=dataRoww["patNum"].ToString();
						break;
					case EnumApptViewElement.PatNumAndName:
						text=dataRoww["patNumAndName"].ToString();
						break;
					case EnumApptViewElement.PremedFlag:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["preMedFlag"].ToString();
						}
						break;
					case EnumApptViewElement.Procs:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["procs"].ToString();
						}
						break;
					case EnumApptViewElement.ProcsColored:
						string value=dataRoww["procsColored"].ToString();
						//The procsColored column is stored as XML in order to give additional information (color) for the procedure to display.
						//There was a bug with middle tier users where the column in the database would store an HTTP-encoded string;  "&lt;span&gt;..."
						//Convert the string that has potentially been encoded for HTTP transmission into a decoded string.
						value=System.Web.HttpUtility.HtmlDecode(value);
						string[] lines=value.Split(new string[] { "</span>" },StringSplitOptions.RemoveEmptyEntries);
						PointF tempPt=new PointF(pointDraw.X,pointDraw.Y);
						int lastH=0;
						for(int i=0;i<lines.Length;i++) {
							string rgbInt="";
							string proc=lines[i];
							Match m=Regex.Match(lines[i],"^<span color=\"(-?[0-9]*)\">(.*)$");
							if(m.Success) {
								rgbInt=m.Result("$1");
								proc=m.Result("$2");
							}
							//procedure abbreviation could be empty
							if(i!=lines.Length-1 && !proc.IsNullOrEmpty()) {
								proc+=",";
							}
							if(string.IsNullOrEmpty(rgbInt)) {
								rgbInt=listApptViewItems[idxItem].ElementColorXml.ToString();
							}
							Color c=Color.FromArgb(PIn.Int(rgbInt,false));
							SizeF procSize=new Size(1,lastH);//Use the last height measured, otherwise it will draw procs on top of each other if the appt is narrow
							if(!proc.IsNullOrEmpty()) {
								procSize=g.MeasureString(proc,_font,(int)widthAppt-(int)WidthProvOnAppt-1,new StringFormat(StringFormatFlags.MeasureTrailingSpaces));
							}
							procSize.Width=(float)Math.Ceiling(procSize.Width);
							if(tempPt.X+procSize.Width>widthAppt) {
								tempPt.X=pointDraw.X;
								tempPt.Y+=lastH;
							}
							RectangleF procRect=new RectangleF(tempPt,procSize);
							procRect.Height=Math.Min(procRect.Height,heightAppt-tempPt.Y);
							SolidBrush sb=new SolidBrush(c);
							g.DrawString(proc,_font,sb,procRect);
							DisposeObjects(sb);
							if(!proc.IsNullOrEmpty()) { 
								tempPt.X+=(int)procRect.Width+3;//+3 is room for spaces
							}
							lastH=(int)procSize.Height;
							if(tempPt.Y+lastH > heightAppt || i==lines.Length-1) {
								tempPt.Y+=lastH;
								//Faster to skip lines that would be below bottom of appt
								break;
							}
						}
						pointDraw.Y=tempPt.Y;
						return pointDraw;
					case EnumApptViewElement.Production:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["production"].ToString();
						}
						break;
					case EnumApptViewElement.ProphyPerioPastDue_P:
						text=dataRoww["prophy/PerioPastDue[P]"].ToString();
						break;
					case EnumApptViewElement.RecallPastDue_R:
						text=dataRoww["recallPastDue[R]"].ToString();
						break;
					case EnumApptViewElement.Provider:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["provider"].ToString();
						}
						break;
					case EnumApptViewElement.TimeAskedToArrive:
						if(isNote) {
							text="";
						}
						else {
							DateTime timeAskedToArrive;
							if(DateTime.TryParse(dataRoww["timeAskedToArrive"].ToString(),out timeAskedToArrive)) {//timeAskedToArrive value could be blank
								text=timeAskedToArrive.ToShortTimeString();
							}
							else {
								//probably just a blank string
								text=dataRoww["timeAskedToArrive"].ToString();
							}
						}
						break;
					case EnumApptViewElement.VerifyIns_V:
						text=dataRoww["verifyIns[V]"].ToString();
						break;
					case EnumApptViewElement.WirelessPhone:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["wirelessPhone"].ToString();
						}
						break;
					case EnumApptViewElement.WkPhone:
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["wkPhone"].ToString();
						}
						break;
				}
			#endregion Fill Text
			object[] parameters={ dataRoww["PatNum"].ToString(),listApptViewItems[idxItem].PatFieldDefNum,text };
			Plugins.HookAddCode(null,"ApptSingleDrawing.DrawElement_afterFillText",parameters);
			text=(string)parameters[2];
			if(text=="" && !isConfirmedCircle && !isCareCreditStatus) {
				return pointDraw;//next element will draw at the same position as this one would have.
			}
			SolidBrush brush=new SolidBrush(listApptViewItems[idxItem].ElementColor);
			//SolidBrush noteTitlebrush = new SolidBrush(Defs.Long[(int)DefCat.AppointmentColors][8].ItemColor);
			StringFormat stringFormatLeft=new StringFormat();
			stringFormatLeft.Alignment=StringAlignment.Near;
			int charactersFitted;//not used, but required as 'out' param for measureString.
			int linesFilled;
			SizeF sizeNote;
			SizeF sizeText;
			RectangleF rectangleLayout;
			RectangleF rectBack;
			#region Main
			if(align==ApptViewAlignment.Main) {//always stacks vertical
				if(isConfirmedCircle) {
					Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
					using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww)) {
						g.DrawImage(bitmapIcon,pointDraw.X+1,pointDraw.Y);
					}
					DisposeObjects(brush,stringFormatLeft);
					return new PointF(pointDraw.X,pointDraw.Y+sizeDesired.Height);
				}
				else if(isCareCreditStatus) {
					if(text=="") {//the text should have the status and if it doesn't, return the point we're currently at becuase there is nothing to draw
						return pointDraw;
					}
					Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
					using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww, text)) {
						g.DrawImage(bitmapIcon,pointDraw.X+1,pointDraw.Y);
					}
					DisposeObjects(brush,stringFormatLeft);
					return new PointF(pointDraw.X,pointDraw.Y+sizeDesired.Height);
				}
				else {//text
					SizeF sizeLayoutArea=new SizeF();
					sizeLayoutArea.Width=widthAppt-WidthProvOnAppt-1;
					sizeLayoutArea.Height=heightAppt-pointDraw.Y;
					sizeText=g.MeasureString(text,_font,sizeLayoutArea,stringFormatLeft,out charactersFitted,out linesFilled);
					if(isInsuranceColor) {
						string[] ins1ins2=text.Split("\r\n",StringSplitOptions.None);//2 insurances are connected together with \r\n
						string ins1=text;
						string ins2="";
						if(ins1ins2.Length>1){
							ins1=ins1ins2[0];
							ins2=ins1ins2[1];
						}
						SizeF size1=g.MeasureString(ins1,_font,sizeLayoutArea);
						if(dataRoww["insColor1"].ToString()!="") {
							Color color=Color.FromArgb(PIn.Int(dataRoww["insColor1"].ToString()));
							if(color!=Color.Black && color!=Color.FromArgb(255,Color.Black)) {
								PointF pt=new PointF(pointDraw.X,pointDraw.Y-1);
								size1.Height-=1;
								RectangleF rectF=new RectangleF(pt,size1);
								g.FillRectangle(new SolidBrush(color),rectF);
							}
						}
						if(dataRoww["insColor2"].ToString()!="" && ins2!="") {
							Color color=Color.FromArgb(PIn.Int(dataRoww["insColor2"].ToString()));
							if(color!=Color.Black && color!=Color.FromArgb(255,Color.Black)) {
								PointF pt=new PointF(pointDraw.X,pointDraw.Y-1+size1.Height);
								SizeF size=g.MeasureString(ins2,_font,sizeLayoutArea);
								size.Height-=1;
								RectangleF rectF=new RectangleF(pt,size);
								g.FillRectangle(new SolidBrush(color),rectF);
							}
						}
					}
					rectangleLayout=new RectangleF(pointDraw,sizeText);
					g.DrawString(text,_font,brush,rectangleLayout,stringFormatLeft);
					DisposeObjects(brush,stringFormatLeft);
					//to make each row line up, we need to use heightLine instead of rectangleLayout.Bottom
					return new PointF(pointDraw.X,pointDraw.Y+linesFilled*_heightLine);
				}
			}
			#endregion
			#region UR
			else if(align==ApptViewAlignment.UR) {
				if(stackBehavior==ApptViewStackBehavior.Vertical) {
					float w=widthAppt-9;
					if(isConfirmedCircle) {
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width,pointDraw.Y+1);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X,pointDraw.Y+(int)sizeDesired.Height);
					}
					else if(isCareCreditStatus) {
						if(text=="") {//the text should have the status and if it doesn't, return the point we're currently at becuase there is nothing to draw
							return pointDraw;
						}
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width,pointDraw.Y+1);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww, text)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X,pointDraw.Y+sizeDesired.Height);
					}
					else {
						sizeNote=g.MeasureString(text,_font,(int)w);
						sizeNote=new SizeF(sizeNote.Width,_heightLine+1);//only allowed to be one line high.
						if(sizeNote.Width<5) {
							sizeNote=new SizeF(5,sizeNote.Height);
						}
						if(sizeNote.Height+pointDraw.Y>heightAppt) {
							return new PointF(pointDraw.X,pointDraw.Y);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeNote.Width,pointDraw.Y);//upper left corner of this element
						rectangleLayout=new RectangleF(drawLocThis,sizeNote);
						rectangleLayout.Y+=2;
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,sizeNote.Width,_heightLine);
						if(listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()) 
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,_font,_brushBlack,rectangleLayout,stringFormatLeft);
						}
						else {
							//There are a few other boxed lettters (L,A,D,V,etc?) that are drawn here instead of with the other boxed letters.
							//We will leave behavior as is for now.
							g.FillRectangle(_brushWhite,rectBack);
							g.DrawString(text,_font,brush,rectangleLayout,stringFormatLeft);
						}
						g.DrawRectangle(_penBlack,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X,pointDraw.Y+_heightLine);//move down a certain number of lines for next element.
					}
				}
				else {//horizontal
					if(isConfirmedCircle) {
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						if(pointDraw.X-sizeDesired.Width<=0) {
							return new PointF(pointDraw.X-sizeDesired.Width,pointDraw.Y);
						}
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width,pointDraw.Y+1);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X-(int)sizeDesired.Width-3,pointDraw.Y);
					}
					else if(isCareCreditStatus) {
						if(text=="") {//the text should have the status and if it doesn't, return the point we're currently at becuase there is nothing to draw
							return pointDraw;
						}
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						if(pointDraw.X-sizeDesired.Width<=0) {
							return new PointF(pointDraw.X-sizeDesired.Width,pointDraw.Y);
						}
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width,pointDraw.Y+1);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww, text)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X-(int)sizeDesired.Width-3,pointDraw.Y);
					}
					else {
						float w=pointDraw.X-9;
						sizeNote=g.MeasureString(text,_font,(int)w);
						sizeNote=new SizeF(sizeNote.Width,_heightLine+1);//only allowed to be one line high.
						if(sizeNote.Width<5) {
							sizeNote=new SizeF(5,sizeNote.Height);
						}
						if(pointDraw.X-sizeNote.Width<=0) {
							return new PointF(pointDraw.X-sizeNote.Width,pointDraw.Y);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeNote.Width,pointDraw.Y);//upper left corner of this element
						rectangleLayout=new RectangleF(drawLocThis,sizeNote);
						rectangleLayout.Y+=2;
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,sizeNote.Width,_heightLine);
						if(listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()) 
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,_font,_brushBlack,rectangleLayout,stringFormatLeft);
						}
						else {
							//There are a few other boxed lettters (L,A,D,etc?) that are drawn here instead of with the other boxed letters.
							//We will leave behavior as is for now.
							g.FillRectangle(_brushWhite,rectBack);
							g.DrawString(text,_font,brush,rectangleLayout,stringFormatLeft);
						}
						g.DrawRectangle(_penBlack,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X-(int)rectangleLayout.Width-3,pointDraw.Y);//Move to left.  Might also have to subtract a little from x to space out elements.
					}
				}
			}
			#endregion
			#region LR
			else {//LR
				if(stackBehavior==ApptViewStackBehavior.Vertical) {
					float w=widthAppt-9;
					if(isConfirmedCircle) {
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width,pointDraw.Y+1-_heightLine);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X,pointDraw.Y-(int)sizeDesired.Height);
					}
					else if(isCareCreditStatus) {
						if(text=="") {//the text should have the status and if it doesn't, return the point we're currently at becuase there is nothing to draw
							return pointDraw;
						}
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width,pointDraw.Y+1-_heightLine);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww, text)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X,pointDraw.Y-(int)sizeDesired.Height);
					}
					else {
						sizeNote=g.MeasureString(text,_font,(int)w);
						sizeNote=new SizeF(sizeNote.Width,_heightLine+1);//only allowed to be one line high.  Needs an extra pixel.
						if(sizeNote.Width<5) {
							sizeNote=new SizeF(5,sizeNote.Height);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeNote.Width,pointDraw.Y-_heightLine);//upper left corner of this element
						rectangleLayout=new RectangleF(drawLocThis,sizeNote);
						rectangleLayout.Y+=2;
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,sizeNote.Width,_heightLine);
						if(listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()) 
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,_font,_brushBlack,rectangleLayout,stringFormatLeft);
						}
						else {
							//There are a few other boxed lettters (L,A,D,etc?) that are drawn here instead of with the other boxed letters.
							//We will leave behavior as is for now.
							g.FillRectangle(_brushWhite,rectBack);
							g.DrawString(text,_font,brush,rectangleLayout,stringFormatLeft);
						}
						g.DrawRectangle(_penBlack,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X,pointDraw.Y-_heightLine);//move up a certain number of lines for next element.
					}
				}
				else {//horizontal
					float w=pointDraw.X-9;//drawLoc is upper right of each element.  The first element draws at (totalWidth-1,0).
					if(isConfirmedCircle) {
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						if(pointDraw.X-sizeDesired.Width<=0) {
							return new PointF(pointDraw.X-sizeDesired.Width,pointDraw.Y);
						}
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width+1,pointDraw.Y+1-_heightLine);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X-(int)sizeDesired.Width-3,pointDraw.Y);
					}
					else if(isCareCreditStatus) {
						if(text=="") {//the text should have the status and if it doesn't, return the point we're currently at becuase there is nothing to draw
							return pointDraw;
						}
						Size sizeDesired=new Size((int)_heightLine,(int)_heightLine);
						if(pointDraw.X-sizeDesired.Width<=0) {
							return new PointF(pointDraw.X-sizeDesired.Width,pointDraw.Y);
						}
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeDesired.Width+1,pointDraw.Y+1-_heightLine);//upper left corner of this element
						using(Bitmap bitmapIcon=DrawElementGetIcon(sizeDesired, dataRoww, text)) {
							g.DrawImage(bitmapIcon,drawLocThis);
						}
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X-(int)sizeDesired.Width-3,pointDraw.Y);
					}
					else {
						sizeNote=g.MeasureString(text,_font,(int)w);
						sizeNote=new SizeF(sizeNote.Width,_heightLine+1);//only allowed to be one line high.  Needs an extra pixel.
						if(sizeNote.Width<5) {
							sizeNote=new SizeF(5,sizeNote.Height);
						}
						if(pointDraw.X-sizeNote.Width<=0) {
							return new PointF(pointDraw.X-sizeNote.Width,pointDraw.Y);
						}
						PointF drawLocThis=new PointF(pointDraw.X-(int)sizeNote.Width,(int)(pointDraw.Y-_heightLine));//upper left corner of this element
						rectangleLayout=new RectangleF(drawLocThis,sizeNote);
						rectangleLayout.Y+=2;
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,sizeNote.Width,_heightLine);
						if(listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
							|| listApptViewItems[idxItem].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()) 
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,_font,_brushBlack,rectangleLayout,stringFormatLeft);
						}
						else {
							//There are a few other boxed lettters (L,A,D,etc?) that are drawn here instead of with the other boxed letters.
							//We will leave behavior as is for now.
							g.FillRectangle(_brushWhite,rectBack);
							g.DrawString(text,_font,brush,rectangleLayout,stringFormatLeft);
						}
						g.DrawRectangle(_penBlack,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						DisposeObjects(brush,stringFormatLeft);
						return new PointF(pointDraw.X-(int)sizeNote.Width-3,pointDraw.Y);//Move to left.  Subtract a little from x to space out elements.
					}
				}
			}
			#endregion
		}

		///<summary>This is used to draw the ConfirmedColor icon or the CareCreditApprovalStatus icons</summary>
		private Bitmap DrawElementGetIcon(Size sizeDesired, DataRow dataRow, string careCreditStatus="") {
			//The default color is the color of the confirmed icon, because that will always be drawn if present
			Color color=Defs.GetColor(DefCat.ApptConfirmed,PIn.Long(dataRow["Confirmed"].ToString()));
			Pen penOutline=new Pen(Color.FromArgb(00,00,00));//Black
			string careCreditStatesLower=careCreditStatus.ToLower();
			if(ListTools.In(careCreditStatesLower.ToLower(),CareCreditWebStatus.PreApproved.GetDescription().ToLower(),
				CareCreditWebStatus.DupQS.GetDescription().ToLower())) 
			{ //example value: "Pre-Approved"
				color=Color.FromArgb(00,170,00);
				penOutline=new Pen(Color.FromArgb(00,24,00));//Dark Green;
			}
			else if(careCreditStatesLower==CareCreditWebStatus.Declined.GetDescription().ToLower()
				|| careCreditStatesLower==CareCreditWebStatus.CallForAuth.GetDescription().ToLower()) 
			{
				color=Color.Orange;
				penOutline=new Pen(Color.FromArgb(80,40,00));//Dark Brown
			}
			else if(careCreditStatesLower==CareCreditWebStatus.AccountFound.GetDescription().ToLower()) {
				color=Color.RoyalBlue;
				penOutline=new Pen(Color.FromArgb(255,27,43,92));//Dark Blue
			}
			SolidBrush brushIcon=new SolidBrush(color);
			Bitmap bitmapIcon=new Bitmap(sizeDesired.Width+1,sizeDesired.Height+1);//Make the bitmap 1 pixel larger so that the ellipse isn't being cut off
			using(Graphics g=Graphics.FromImage(bitmapIcon)) {
				g.SmoothingMode=SmoothingMode.AntiAlias;
				if(careCreditStatesLower==CareCreditWebStatus.Declined.GetDescription().ToLower()
					|| careCreditStatesLower==CareCreditWebStatus.CallForAuth.GetDescription().ToLower()) {
					PointF[] arrayPoints=new PointF[]{new PointF(sizeDesired.Width/2f,0), new PointF(0,sizeDesired.Height),
						new PointF(sizeDesired.Width,sizeDesired.Height), new PointF(sizeDesired.Width/2f,0)};
					g.FillPolygon(brushIcon,arrayPoints);//CareCreditWebStatus declined and call for auth will display a triangle icon
					g.DrawPolygon(penOutline,arrayPoints);
				}
				else if(careCreditStatesLower==CareCreditWebStatus.AccountFound.GetDescription().ToLower()) {
					Rectangle rect=new Rectangle(new Point(0,0),sizeDesired);
					g.FillRectangle(brushIcon,rect);
					g.DrawRectangle(penOutline,rect);
				}
				else {
					g.FillEllipse(brushIcon,0,0,sizeDesired.Width,sizeDesired.Height);
					g.DrawEllipse(penOutline,0,0,sizeDesired.Width,sizeDesired.Height);
				}
			}
			brushIcon.Dispose();
			penOutline.Dispose();
			return bitmapIcon;
		}

		///<summary>Disposes objects with typeof Brush, Pen, StringFormat, or Bitmap.</summary>
		private static void DisposeObjects(params object[] disposables) {
			for(int i=0;i<disposables.Length;i++) {
				if(disposables[i]==null) {
					continue;
				}
				if(disposables[i].GetType()==typeof(Brush)) {
					Brush b=(Brush)disposables[i];
					b.Dispose();
				}
				else if(disposables[i].GetType()==typeof(Pen)) {
					Pen p=(Pen)disposables[i];
					p.Dispose();
				}
				else if(disposables[i].GetType()==typeof(StringFormat)) {
					StringFormat sf=(StringFormat)disposables[i];
					sf.Dispose();
				}
				else if(disposables[i].GetType()==typeof(Bitmap)) {
					Bitmap bmp=(Bitmap)disposables[i];
					bmp.Dispose();
				}
			}
		}
		#endregion Methods - Private DrawAppts

		#region Methods - Private DrawApptOutlineSelected
		///<summary>Only one appt can be selected.  This draws the single thick rectangle indicating that selection.</summary>
		private void DrawOutlineSelected(Graphics g){
			if(SelectedAptNum<1){
				return;//don't draw anything
			}
			for(int i=0;i<TableAppointments.Rows.Count;i++) {
				DataRow dataRow=TableAppointments.Rows[i];
				long aptNum=PIn.Long(dataRow["AptNum"].ToString());
				if(aptNum!=SelectedAptNum){
					continue;
				}
				Pen penProvOutline;
				if(dataRow["ProvNum"].ToString()!="0" && dataRow["IsHygiene"].ToString()=="0") {//dentist
					penProvOutline=new Pen(Providers.GetOutlineColor(PIn.Long(dataRow["ProvNum"].ToString())),3f);
				}
				else if(dataRow["ProvHyg"].ToString()!="0" && dataRow["IsHygiene"].ToString()=="1") {//hygienist
					penProvOutline=new Pen(Providers.GetOutlineColor(PIn.Long(dataRow["ProvHyg"].ToString())),3f);
				}
				else {//unknown
					penProvOutline=new Pen(Color.Black,3f);
				}
				RectangleF rectangleF=new RectangleF();
				if(i>_listApptLayoutInfos.Count-1) { 
					continue;
				}
				rectangleF.Location=TranslateMainToContr(_listApptLayoutInfos[i].RectangleBounds.Location);
				rectangleF.Size=_listApptLayoutInfos[i].RectangleBounds.Size;
				if(rectangleF.Bottom<0 || rectangleF.Top>_heightMainVisible+_heightProvOpHeaders){//if outside visible area
					return;
				}
				GraphicsPath graphicsPathOutline=GetRoundedPath(rectangleF,_radiusCorn);
				g.DrawPath(penProvOutline,graphicsPathOutline);
				DisposeObjects(penProvOutline);
			}
		}
		#endregion Methods - Private DrawApptOutlineSelected

		#region Methods - Private DrawRedTimeLine
		///<summary>This was old way of drawing time line across all the bitmaps.</summary>
		private void DrawRedTimeLine(Graphics g){
			float curTimeY=(float)DateTime.Now.TimeOfDay.TotalHours*_heightLine*_rowsPerHr;
			float yVal=TranslateYMainToContr(curTimeY);
			if(yVal<_heightProvOpHeaders || yVal>_heightMainVisible+_heightProvOpHeaders){//if outside visible area
				return;
			}
			g.DrawLine(_penTimeLine,0,yVal,Width-vScrollBar1.Width,yVal);//draws under the scrollbar slightly
		}

		///<summary>Draws within main appt area only</summary>
		private void DrawRedTimeLineMain(Graphics g){
			float curTimeY=(float)DateTime.Now.TimeOfDay.TotalHours*_heightLine*_rowsPerHr;
			float yVal=curTimeY;
			g.DrawLine(_penTimeLine,0,yVal,_widthMain,yVal);
		}

		///<summary>Draws within left provbar area only</summary>
		private void DrawRedTimeLineProv(Graphics g){
			float curTimeY=(float)DateTime.Now.TimeOfDay.TotalHours*_heightLine*_rowsPerHr;
			float yVal=curTimeY;
			g.DrawLine(_penTimeLine,0,yVal,_widthProv*_listProvsVisible.Count,yVal);
		}

		///<summary>Draws for either of the time bars</summary>
		private void DrawRedTimeLineTimeBar(Graphics g){
			float curTimeY=(float)DateTime.Now.TimeOfDay.TotalHours*_heightLine*_rowsPerHr;
			float yVal=curTimeY;
			g.DrawLine(_penTimeLine,0,yVal,_widthTime,yVal);
		}
		#endregion Methods - Private DrawRedTimeLine

		#region Methods - Private DrawInfoBubble
		///<summary>Before calling this, do a hit test for appts.  Fills the bubble with data and then positions it.  In coordinates of this UserContrApptsPanel (mouse).</summary>
		private void BubbleDraw(Point point,int idxAppt) {
			if((ApptViewCur==null && PrefC.GetBool(PrefName.AppointmentBubblesDisabled))
					|| (ApptViewCur!=null && ApptViewCur.IsApptBubblesDisabled))
			{
				//don't show appt bubbles at all
				_isBubbleVisible=false;
				timerBubble.Enabled=false;
				return;
			}
			//if(_isMouseDown){
			if(Control.MouseButtons==MouseButtons.Left || Control.MouseButtons==MouseButtons.Right || Control.MouseButtons==MouseButtons.Middle){
				//bubble should not show while mouse is down. Includes dragging, context menus, etc.
				if(_isBubbleVisible){
					_isBubbleVisible=false;
					timerBubble.Enabled=false;
					this.Invalidate();
				}
				return;
			}
			_pointBubbleTimer=point;
			DataRow dataRow=null;
			if(idxAppt!=-1 && idxAppt < TableAppointments.Rows.Count){
				dataRow=TableAppointments.Rows[idxAppt];
			}
			if(dataRow==null || HitTestApptBottom(point,idxAppt)){
				//not hovering over an appointment at all, or getting ready to resize, so hide any existing bubble
				if(_isBubbleVisible) {
					_isBubbleVisible=false;
					timerBubble.Enabled=false;
					this.Invalidate();
				}
				return;
			}
			int yval=point.Y+10;
			int xval=point.X+10;
			if(idxAppt==_bubbleApptIdx) {//if the pointer is still on the same appt as last time
				if(DateTime.Now.AddMilliseconds(-280) > _dateTimeBubble //if it's been .28 seconds
					| !PrefC.GetBool(PrefName.ApptBubbleDelay)) //or if there is not supposed to be a delay
					{
					if(yval > hScrollBar1.Top-_rectangleBubble.Height) {
						yval=hScrollBar1.Top-_rectangleBubble.Height;
					}
					if(xval > this.Right-vScrollBar1.Width-1-_rectangleBubble.Width) {
						xval=this.Right-vScrollBar1.Width-1-_rectangleBubble.Width;
					}
					_rectangleBubble.Location=new Point(xval,yval);
					_isBubbleVisible=true;
				}
				this.Invalidate();//we should make this a smaller area instead of the entire screen
				return;
			}
			//From here down, aptNum!=bubbleAptNum, and we need to create a new bubble----------------------------------------------------------------------------
			//Hide while we draw everything to reduce jitter caused by resizing.  Will be set visible later if appropriate.  
			//Otherwise, after timerBubble ticks and ApptBubbleDelay preference has been satisfied.
			//long aptNum=PIn.Long(dataRow["AptNum"].ToString());
			//_isBubbleVisible=false;
			//reset timer for popup delay
			timerBubble.Enabled=false;
			timerBubble.Enabled=true;
			//delay for hover effect 0.28 sec
			_dateTimeBubble=DateTime.Now;
			_bubbleApptIdx=idxAppt;
			//Height is taller than needed. Later, only the portion we need will be copied to a smaller bitmap.
			if(_rectangleBubble.Width==0){
				_rectangleBubble.Size=new Size(LayoutManager.Scale(200),LayoutManager.Scale(300));
			}
			using Font fontBubble=new Font(FontFamily.GenericSansSerif,LayoutManager.Scale(9));
			using Font fontBubbleHeader=new Font(FontFamily.GenericSansSerif,LayoutManager.Scale(10),FontStyle.Bold);
			List<DisplayField> listDisplayFieldsBubble=DisplayFields.GetForCategory(DisplayFieldCategory.AppointmentBubble);
			//most data is already present in TableAppointment, but we do need to get the patient picture
			bool showingPatientPicture=listDisplayFieldsBubble.Any(x => x.InternalName=="Patient Picture");
			bool isNameWrapping=false;//This boolean is used to adjust the Y location of the patient picture.
			if(listDisplayFieldsBubble.Count>1) {
				DisplayField displayFieldFirst=listDisplayFieldsBubble[0];
				DisplayField displayFieldSecond=listDisplayFieldsBubble[1];
				bool isWrapNeeded=(_rectangleBubble.Width<TextRenderer.MeasureText(dataRow["PatientName"].ToString(),fontBubbleHeader).Width);
				//There is logic farther down that will force the patient name on top of the patient picture if the name is first or if the picture is first and the name is second.
				//Also, the picture will only need to move if the patient name is too wide for the current bubble and will need to wrap.
				if((displayFieldFirst.InternalName=="Patient Name" && isWrapNeeded)
					|| (displayFieldFirst.InternalName=="Patient Picture" && displayFieldSecond.InternalName=="Patient Name" && isWrapNeeded))
				{
					isNameWrapping=true;
				}
			}
			Bitmap bitmap=new Bitmap(_rectangleBubble.Width,LayoutManager.Scale(800));
			//We will draw on this tall temp bitmap until we figure out where the bottom is.
			//Then, we will copy the portion we want over to the class-wide bitmap.
			Graphics g=Graphics.FromImage(bitmap);
			g.SmoothingMode=SmoothingMode.HighQuality;
			using(SolidBrush brushBackColor = new SolidBrush(Color.FromArgb(255,250,190))) {
				g.FillRectangle(brushBackColor,0,0,bitmap.Width,bitmap.Height);
			}
			Bitmap bitmapPatPict=null;
			if(showingPatientPicture) {
				string imageFolder=dataRow["ImageFolder"].ToString();
				if(imageFolder!=""
					&& (PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage))//Do not use patient image when A to Z folders are disabled.
				{
					try {
						long patNum=PIn.Long(dataRow["PatNum"].ToString());
						bitmapPatPict=Documents.GetPatPict(patNum,
							ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),
								imageFolder.Substring(0,1).ToUpper(),
								imageFolder,""));
					}
					catch(ApplicationException) { }  //Folder access might be denied
				}
			}
			float imageOffset=LayoutManager.Scale(17);//use this to change the y location of the pat image if the pat name wraps
			Rectangle rectanglePict=new Rectangle(LayoutManager.Scale(6),(int)imageOffset,LayoutManager.Scale(100),LayoutManager.Scale(100));//Always draws in the same spot, unless the pat name wrapped
			//Draw the picture later, for the minor advantage of drawing on top of any horizontal line
			#region infoBubble Text
			Brush brush=Brushes.Black;//System brush, so we don't need to dispose.
			float x=0;
			float y=0;
			float h=0;
			string s;
			Font font=fontBubbleHeader;//for first row
			for(int i=0;i<listDisplayFieldsBubble.Count;i++) {
				if(listDisplayFieldsBubble[i].InternalName=="Patient Picture") {
					continue;//do nothing. We will draw later.
				}
				//we can't use idx to test where we are because pic could be anywhere
				//so we instead test x, which is a little clumsy, but no big deal
				//Note: the Patient Picture always shows in the same spot, regardless of the order within Display Fields.
				if(CompareFloat.IsEqual(x,LayoutManager.ScaleF(8))){//This is the second line
					x=LayoutManager.ScaleF(110);
					y=LayoutManager.ScaleF(14);
					font=fontBubble;
					if(!showingPatientPicture) { 
						x=LayoutManager.ScaleF(9); //the position of each display field can be closer to the edge of the appt bubble because there is no image
					}
					if(isNameWrapping) {
						y=imageOffset-LayoutManager.ScaleF(3);//the other text needs to start lower down if the name is wrapping
					}
				}
				if(x==0){//This is first line
					x=LayoutManager.ScaleF(8);//magic number. Ugh.
				}
				if(showingPatientPicture && y>=(imageOffset+rectanglePict.Height)) {
					x=LayoutManager.ScaleF(2);
				}
				int widthBubble=_rectangleBubble.Width;//more concise
				switch(listDisplayFieldsBubble[i].InternalName) {
					case "Patient Name":
						s=dataRow["PatientName"].ToString();
						h=(float)Math.Floor(g.MeasureString(s,font,widthBubble-(int)x).Height);
						if(isNameWrapping) {//Patient name is first and wraps.
							imageOffset=h;//Push the patient picture down below the name.
						}
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Patient Picture":
						//We have already dealt with this above.
						continue;
					case "Appt Day":
						s=dataRow["aptDay"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Appt Date":
						s=dataRow["aptDate"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Appt Time":
						s=dataRow["aptTime"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Appt Length":
						s=dataRow["aptLength"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Provider":
						s=dataRow["provider"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Production":
						s=dataRow["production"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Confirmed":
						s=Defs.GetName(DefCat.ApptConfirmed,PIn.Long(dataRow["Confirmed"].ToString()));
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "ASAP":
						ApptPriority priority=(ApptPriority)PIn.Int(dataRow["Priority"].ToString());
						if(priority==ApptPriority.ASAP) {
							h=g.MeasureString("ASAP",font,widthBubble-(int)x).Height;
							g.DrawString("ASAP",font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Med Flag":
						s=dataRow["preMedFlag"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Med Note":
						s=dataRow["MedUrgNote"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Lab":
						s=dataRow["lab"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Procedures":
						s=dataRow["procs"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Note":
						s=dataRow["Note"].ToString();
						int maxNoteLength=PrefC.GetInt(PrefName.AppointmentBubblesNoteLength);
						if(s.Trim()!="" && maxNoteLength>0 && s.Length>maxNoteLength) {//Trim text
							s=s.Substring(0,maxNoteLength)+"...";
						}
						if(s.Trim()!="") { //draw text
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Blue,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "PatNum":
						s=dataRow["PatNum"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "ChartNum":
						s=dataRow["chartNumber"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Billing Type":
						s=dataRow["billingType"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Horizontal Line":
						if(showingPatientPicture && y<LayoutManager.Scale(100)+imageOffset) { //height of pat picture is 100 plus the image offset
							y=LayoutManager.Scale(100)+imageOffset;
						}
						y+=3;
						Pen penGray=new Pen(Brushes.Gray,1.5f);
						g.DrawLine(penGray,3,y,widthBubble-3,y);
						penGray.Dispose();
						y+=2;
						continue;
					case "Age":
						s=dataRow["age"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Home Phone":
						s=dataRow["hmPhone"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Work Phone":
						s=dataRow["wkPhone"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Wireless Phone":
						s=dataRow["wirelessPhone"].ToString();
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Contact Methods":
						s=dataRow["contactMethods"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble,h));
							y+=h;
						}
						continue;
					case "Insurance":
						s=dataRow["insurance"].ToString();
						if(s!="") {//overkill since it's only one line
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Insurance Color":
						s=dataRow["insurance"].ToString();
						if(s!="") {
							string[] insuranceArray=s.Split(new string[] { "\r\n" },StringSplitOptions.None);
							foreach(string str in insuranceArray) {
								h=g.MeasureString(str,font,widthBubble-(int)x).Height;
								Carrier carrier=Carriers.GetCarrierByName(str.Replace("Ins1: ","").Replace("Ins2: ",""));
								if(carrier!=null && carrier.ApptTextBackColor!=Color.Black && carrier.ApptTextBackColor!=Color.FromArgb(255,Color.Black)) {
									g.FillRectangle(new SolidBrush(carrier.ApptTextBackColor),x-2,y+1,widthBubble-(int)x+2,h); 
								}
								g.DrawString(str,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
								y+=h;
							}
						}
						continue;
					case "Address Note":
						s=dataRow["addrNote"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Fam Note":
						s=dataRow["famFinUrgNote"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Appt Mod Note":
						s=dataRow["apptModNote"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,Brushes.Red,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "ReferralFrom":
						s=dataRow["referralFrom"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "ReferralTo":
						s=dataRow["referralTo"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Referral From With Phone":
						s=dataRow["referralFromWithPhone"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Referral To With Phone":
						s=dataRow["referralToWithPhone"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Language":
						s=dataRow["language"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Email":
						s=dataRow["Email"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Discount Plan":
						s=dataRow["discountPlan"].ToString();
						if(s!="") {
							h=g.MeasureString(s,font,widthBubble-(int)x).Height;
							g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
							y+=h;
						}
						continue;
					case "Estimated Patient Portion":
						decimal decimalPatPort=PIn.Decimal(dataRow["estPatientPortionRaw"].ToString());
						s=Lan.g(this,"Est Patient Portion: ")+decimalPatPort.ToString("c");
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "CareCredit Status":
						PatFieldDef patFieldDefCareCredit=PatFieldDefs.GetPatFieldCareCredit();
						if(patFieldDefCareCredit==null) {
							continue;
						}
						s="";
						for(int j=0;j<TablePatFields.Rows.Count;j++) {
							if(TablePatFields.Rows[j]["PatNum"].ToString()!=dataRow["PatNum"].ToString()) {
									continue;
								}
							if(TablePatFields.Rows[j]["FieldName"].ToString()!=patFieldDefCareCredit.FieldName) {
								continue;
							}
							s="CareCredit: "+TablePatFields.Rows[j]["FieldValue"].ToString();//example PRE-APPROVED
						}
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
					case "Verify Insurance":
						s=dataRow["verifyIns[V]"].ToString();
						if(s!="") {
							s="Verify Insurance";
						}
						h=g.MeasureString(s,font,widthBubble-(int)x).Height;
						g.DrawString(s,font,brush,new RectangleF(x,y,widthBubble-(int)x,h));
						y+=h;
						continue;
				}
			}
			#endregion infoBubble Text
			if(isNameWrapping) {
				rectanglePict.Y=(int)imageOffset;//change the y position if the name wrapped
			}
			if(showingPatientPicture) { //only draw the pat picture if the display field is present
				using(SolidBrush brushBackColor = new SolidBrush(Color.FromArgb(232,220,190))){
					g.FillRectangle(brushBackColor,rectanglePict);
				}
				if(bitmapPatPict==null){
					StringFormat stringFormat=new StringFormat();
					stringFormat.Alignment=StringAlignment.Center;
					RectangleF rectangleFUnavail=new RectangleF(rectanglePict.X,rectanglePict.Y+LayoutManager.Scale(40),rectanglePict.Width,LayoutManager.Scale(30));
					g.DrawString(Lan.g(this,"Patient Picture Unavailable"),_fontOpsHeaders,Brushes.Gray,rectangleFUnavail,stringFormat);//this wraps to 2 lines
				}
				else{
					g.DrawImage(bitmapPatPict,LayoutManager.Scale(6),imageOffset,LayoutManager.Scale(100),LayoutManager.Scale(100));
					bitmapPatPict.Dispose();
				}
				//in any case, draw the rectangle outline
				g.DrawRectangle(Pens.Gray,rectanglePict);
			}
			//other family members?
			if(showingPatientPicture && y<rectanglePict.Height+rectanglePict.Location.Y) {
				y=rectanglePict.Height+rectanglePict.Location.Y;
			}
			g.DrawRectangle(Pens.Gray,0,0,_rectangleBubble.Width-1,(int)y+2);
			g.Dispose();
			_rectangleBubble.Size=new Size(_rectangleBubble.Width,(int)y+3);
			_rectangleBubble.Location=new Point(xval,yval);
			//only show right away if option set for no delay, otherwise, it will not show until mouse had hovered for at least 0.28 seconds(arbitrary #)
			//the timer fires at 0.30 seconds, so the difference was introduced because of what seemed to be inconsistencies in the timer function 
			if(!PrefC.GetBool(PrefName.ApptBubbleDelay)) {
				_isBubbleVisible=true;
			}
			else {
				_isBubbleVisible=false;
			}
			//Draw the local bitmap onto the class _bitmapBubble---------------------------------------------------------------------------------------
			_bitmapBubble?.Dispose();
			_bitmapBubble=new Bitmap(_rectangleBubble.Width,_rectangleBubble.Height);
			using(Graphics gfx=Graphics.FromImage(_bitmapBubble)){
				gfx.SmoothingMode=SmoothingMode.HighQuality;
				gfx.DrawImage(bitmap,0,0);
			}
			bitmap.Dispose();
			this.Invalidate();//later, invalidate smaller area
		}
		#endregion Methods - Private DrawInfoBubble

		#region Methods - Private Computations
		///<summary>We do our computations in float, for accuracy.  But then, we sometimes need to convert to the nearest int. This mostly comes up when sizing and placing controls.  Drawing can continue to use floats.  This method makes the code look a little cleaner.</summary>
		private int Round(float inVal){
			return (int)Math.Round(inVal);
		}

		///<summary>Pass in the xPos as coordinate of this entire control, not of the main bitmap.</summary>
		private int XPosToOpIdx(int xPos) {
			int retVal;
			if(IsWeeklyView) {
				float day=XPosToDay(xPos);
				//provbars and hscroll not showing
				retVal=(int)Math.Floor((xPos-_widthTime-day*_widthWeekDay)/_widthWeekAppt);
			}
			else {
				retVal=(int)Math.Floor((xPos-_widthTime-_widthProv*_listProvsVisible.Count+hScrollBar1.Value)/_widthOpCol);
			}
			if(retVal>_listOpsVisible.Count-1){
				retVal=(int)_listOpsVisible.Count-1;
			}
			if(retVal<0){
				retVal=0;
			}
			return retVal;
		}

		///<summary>If not weekview, then it always returns 0.  If weekview, then it gives the dayofweek as int. Always based on current view, so 0 will be first day showing.  Pass in the xPos as coordinate of this entire control, not of the main bitmap.</summary>
		private int XPosToDay(int xPos) {
			if(!IsWeeklyView) {
				return 0;
			}
			//Weekly view from here down
			//there are no provbars or hScroll in week view.
			if(xPos<_widthTime){
				return 0;
			}
			if(xPos > _widthTime+_widthMainVisible){
				return 0;
			}
			int retVal=(int)Math.Floor((xPos-_widthTime)/_widthWeekDay);
			return retVal;
		}

		///<summary>Called once from RedrawAsNeeded.  Super fast.</summary>
		private void ComputeColWidth() {
			if(_listOpsVisible.Count==0) {
				_widthOpCol=0;
				return;
			}
			if(_isPrinting){
				_widthOpCol=(float)_widthMain/_listOpsVisible.Count;
				_widthWeekDay=(float)_widthMain/_numOfWeekDaysToDisplay;
				_widthWeekAppt=_widthWeekDay/_listOpsVisible.Count;
				return;
			}
			if(IsWeeklyView) {//no hscroll
				_widthWeekDay=(float)_widthMainVisible/_numOfWeekDaysToDisplay;
				_widthWeekAppt=_widthWeekDay/_listOpsVisible.Count;
				_widthMain=_widthMainVisible;
				hScrollBar1.Value=0;
				hScrollBar1.Enabled=false;
				return;
			}
			int widthOpMinimum=0;
			if(_apptViewCur!=null){
				widthOpMinimum=_apptViewCur.WidthOpMinimum;
			}
			_widthOpCol=(float)_widthMainVisible/_listOpsVisible.Count;
			if(_widthOpCol>=widthOpMinimum){
				_widthMain=_widthMainVisible;
				hScrollBar1.Value=0;
				hScrollBar1.Enabled=false;
				return;
			}
			_widthOpCol=widthOpMinimum;
			_widthMain=widthOpMinimum*_listOpsVisible.Count;
			hScrollBar1.Enabled=true;
			hScrollBar1.SmallChange=50;
			hScrollBar1.LargeChange=200;
			hScrollBar1.Maximum=_widthMain-_widthMainVisible+hScrollBar1.LargeChange-1;
			if(hScrollBar1.Value>hScrollBar1.Maximum-hScrollBar1.LargeChange){
				int hval=hScrollBar1.Maximum-hScrollBar1.LargeChange;
				if(hval<0){
					hval=0;
				}
				hScrollBar1.Value=hval;
			}
		}

		///<summary>Computes _heightProvOpHeaders.  </summary>
		private void ComputeHeightProvOpHeader(){
			_heightProvOpHeaders=LayoutManager.Scale(16)*_sizeFont/8f;
			for(int i=0;i<_listOpsVisible.Count;i++){
				if(TextRenderer.MeasureText(ListOpsVisible[i].OpName,_fontOpsHeaders).Width>_widthOpCol){
					_heightProvOpHeaders=LayoutManager.Scale(27);
					break;
				}
			}
			//In printing, _heightMain is already set, and _heightMainVisible is ignored
			_heightMainVisible=this.Height-(int)_heightProvOpHeaders-hScrollBar1.Height-1;
		}

		///<summary>Computes _heightMain and sets scrollbars</summary>
		private void ComputeHeight(){
			if(_isPrinting){
				//_heightMain= //already set, so nothing to do
				return;
			}
			_heightMain=(int)(_heightLine*24f*_rowsPerHr)+1;
			if(_heightMain>_heightMainVisible){//the normal situation, where part of the main area is hidden by scrolling
				vScrollBar1.Enabled=true;
				vScrollBar1.LargeChange=(int)(_rowsPerHr*_heightLine);//one hour
				vScrollBar1.SmallChange=(int)(_rowsPerHr*_heightLine);//also one hour because this controls the scrolling
				vScrollBar1.Maximum=_heightMain-_heightMainVisible+vScrollBar1.LargeChange;
			}
			else{//rare, when font is very small
				vScrollBar1.Enabled=false;
				vScrollBar1.Value=0;
			}
		}

		///<summary></summary>
		private void ProvBarShading(DataRow dataRow) {
			string patternShowing=GetPatternShowing(dataRow["Pattern"].ToString());//this includes multiple per incr. Example: 10 MinPerIncr, 2 RowsPerIncr, so 12 per hour
			string pattern2Showing=GetPatternShowing(dataRow["PatternSecondary"].ToString());
			int indexProv=-1;
			int indexProv2=-1;
			if(dataRow["IsHygiene"].ToString()=="1") {
				indexProv=GetIndexProv(PIn.Long(dataRow["ProvHyg"].ToString()));
				indexProv2=GetIndexProv(PIn.Long(dataRow["ProvNum"].ToString()));
			}
			else {
				indexProv=GetIndexProv(PIn.Long(dataRow["ProvNum"].ToString()));
				indexProv2=GetIndexProv(PIn.Long(dataRow["ProvHyg"].ToString()));
			}
			if(indexProv==-1){
				return;
			}
			if(dataRow["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()){
				return;
			}
			//was originally from ConvertToY:
			DateTime aptDateTime=PIn.DateT(dataRow["AptDateTime"].ToString());
			//this is not really a yPos because it's ignoring RowsPerIncr
			float yPos=(aptDateTime.Hour*60/MinPerIncr+aptDateTime.Minute/MinPerIncr)*_heightLine;//*RowsPerIncr;
			int startIndex=(int)(yPos/_heightLine);//rounds down
			for(int k=0;k<patternShowing.Length;k+=(int)RowsPerIncr) {
				if(patternShowing.Substring(k,1)=="X") {
					try {
						_provBar[indexProv,startIndex+k/(int)RowsPerIncr]++;
					}
					catch {
						//appointment must extend past midnight.  Very rare
					}
				}
			}
			if(indexProv2==-1) {
				return;
			}
			for(int k = 0;k<pattern2Showing.Length;k+=(int)RowsPerIncr) {
				if(k>=patternShowing.Length) {
					break;
				}
				if(pattern2Showing.Substring(k,1)=="X") {
					try {
						_provBar[indexProv2,startIndex+k/(int)RowsPerIncr]++;
					}
					catch {
						//leave empty
					}
				}
			}
		}

		///<summary>Translates a given point from Control frame of reference to the Main bitmap frame of reference.</summary>
		private PointF TranslateContrToMain(Point point){
			return TranslateContrToMain(new PointF(point.X,point.Y));
		}

		///<summary>Translates a given point from Control frame of reference to the Main bitmap frame of reference.</summary>
		private PointF TranslateContrToMain(PointF pointF){
			return new PointF(TranslateXContrToMain(pointF.X),TranslateYContrToMain(pointF.Y));
		}

		///<summary>Translates a given point from Main bitmap frame of reference to the Control frame of reference.</summary>
		private PointF TranslateMainToContr(PointF pointF){
			return new PointF(TranslateXMainToContr(pointF.X),TranslateYMainToContr(pointF.Y));
		}

		///<summary>Translates a given X val from Control frame of reference to Main bitmap frame of reference.</summary>
		private float TranslateXContrToMain(float xVal){
			if(_showProvBars){
				return xVal-_widthTime-_listProvsVisible.Count*_widthProv+hScrollBar1.Value;
			}
			else{
				return xVal-_widthTime+hScrollBar1.Value;
			}
		}

		///<summary>Translates a given X val from Control frame of reference to Main bitmap frame of reference.</summary>
		private float TranslateXMainToContr(float xVal){
			if(_showProvBars){
				return xVal+_widthTime+_listProvsVisible.Count*_widthProv-hScrollBar1.Value;
			}
			else{
				return xVal+_widthTime-hScrollBar1.Value;
			}
		}

		///<summary>Translates a given Y val from Control frame of reference to  Main bitmap frame of reference.</summary>
		private float TranslateYContrToMain(float yVal){
			return yVal-_heightProvOpHeaders+vScrollBar1.Value;
		}

		///<summary>Translates a given Y val from Main bitmap frame of reference to the Control frame of reference.</summary>
		private float TranslateYMainToContr(float yVal){
			return yVal+_heightProvOpHeaders-vScrollBar1.Value;
		}
		#endregion Methods - Private Computations

		#region Methods - Private HitTest
		///<summary>Returns the index of the appointment within TableAppointments at these coordinates, or -1 if none.  Pass in the point in coordinates of this control/mouse.</summary>
		private int HitTestAppt(Point point) {
			if(ListOpsVisible.Count==0) {//no ops visible.
				return -1;
			}
			if(_heightProvOpHeaders>point.Y){
				return -1;
			}
			if(hScrollBar1.Visible && point.Y>=hScrollBar1.Location.Y) {
				return -1;//if the scrollbar is present, you shouldn't be able to select appts through it
			}
			PointF pointF=TranslateContrToMain(point);
			for(int i=0;i<_listApptLayoutInfos.Count;i++){
				if(_listApptLayoutInfos[i].RectangleBounds.Contains(pointF)){
					return i;
				}
			}
			return -1;
		}
		
		///<summary>If the given point is in the bottom 8 (or 6 or 4) pixels of an appointment, then this returns true.  Always pass in appointment idx from previous HitTestAppt.</summary>
		private bool HitTestApptBottom(Point point,int idxAppt) {
			if(ListOpsVisible.Count==0) {//no ops visible.
				return false;
			}
			if(idxAppt==-1){//not inside any appt
				return false;
			}
			//location is in control coordinates, so convert to bitmap coords.
			float yPos=point.Y-_heightProvOpHeaders+vScrollBar1.Value;
			//if this is a very short appt, then the area we test for must be smaller.
			//at font 8, each row would be 12 high
			if(_listApptLayoutInfos[idxAppt].RectangleBounds.Height<15){
				if(yPos>_listApptLayoutInfos[idxAppt].RectangleBounds.Bottom-4){
					return true;
				}
				return false;
			}
			if(_listApptLayoutInfos[idxAppt].RectangleBounds.Height<25){
				if(yPos>_listApptLayoutInfos[idxAppt].RectangleBounds.Bottom-6){
					return true;
				}
				return false;
			}
			//normal height appt
			if(yPos>_listApptLayoutInfos[idxAppt].RectangleBounds.Bottom-8){
				return true;
			}
			return false;
		}
		#endregion Methods - Private HitTest

		#region Methods - Private Mouse Event Helpers
		///<summary>Returns the production for the operatory appointments as a string.</summary>
		private string GetOperatoryProduction(long operatoryNum) {
			if(!ListApptViewItemRowElements.Exists(x => ListTools.In(x.ElementDesc,"Production","NetProduction"))){
				return"";
			}
			if(DateStart.Year<1880 || DateEnd.Year<1880) {
				return "";
			}
			return "Production: "+GetProduction(
				TableAppointments.Rows.OfType<DataRow>().ToList().FindAll(x=>PIn.Long(x["Op"].ToString())==operatoryNum),
				new List<long> { operatoryNum },new List<long>(),DateStart,DateEnd);
		}

		///<summary>Does a hit test to determine if over operatory or prov header.  If so, creates appropriate "tooltip" panel and positions it.</summary>
		private void HeaderTipDraw(Point point) {
			//Mimic old behavior: 
			//Prov header, hover shows prov
			//Op header, hover shows some info
			//Op header, right or left click shows other info
			//Week: no prov headers, op header right click does nothing, left click changes to day view
			if(point.Y>_heightProvOpHeaders//down in main area
				|| point.X<_widthTime//to the left of prov headers
				|| point.X>this.Width-vScrollBar1.Width-_widthTime//to the right of op headers
				|| point.X<0 || point.Y<0)//pointer left the control completely
			{
				panelHeaderTip.Visible=false;
				_opNumHeaderTipLast=0;//clears out the tool tip additional schedule text
				return;
			}
			string txt="";
			if(IsWeeklyView){
				//do nothing, which is better than old behavior of showing tip that's same as header.
			}
			else if(point.X>_widthTime+_widthProv*_listProvsVisible.Count){//to the right of prov header, so in ops area
				if(ListOpsVisible.Count==0){
					panelHeaderTip.Visible=false;
					return;
				}
				for(int i=0;i<ListOpsVisible.Count;i++){
					if(i==ListOpsVisible.Count-1
						|| point.X+hScrollBar1.Value<_widthTime+_widthProv*_listProvsVisible.Count+_widthOpCol*(i+1))//compare to the righthand side of op
					{
						//found op where we are hovering
						if(_opNumHeaderTipLast==ListOpsVisible[i].OperatoryNum){
							//still same op as last time, so don't change txt, or it will cause the list of schedules that was added during click to disappear.
							txt=labelHeaderTip.Text;
						}
						else{//changed op
							txt=ListOpsVisible[i].OpName;
							string opProduction=GetOperatoryProduction(ListOpsVisible[i].OperatoryNum);
							if(opProduction!=""){
								txt+="\r\n"+opProduction;
							}
							_opNumHeaderTipLast=ListOpsVisible[i].OperatoryNum;
						}
						break;
					}
				}
			}
			else{//prov
				for(int i=0;i<ListProvsVisible.Count;i++){
					if(i==ListProvsVisible.Count-1
						|| point.X<_widthTime+_widthProv*(i+1))//compare to the righthand side of prov
					{
						txt=ListProvsVisible[i].Abbr;
						break;
					}
				}
			}
			if(txt==""){
				panelHeaderTip.Visible=false;
				return;
			}
			panelHeaderTip.Width=TextRenderer.MeasureText(txt,_font).Width+2;
			panelHeaderTip.Height=TextRenderer.MeasureText(txt,_font).Height+3;
			labelHeaderTip.Height=panelHeaderTip.Height;
			labelHeaderTip.Text=txt;
			int xval=point.X;
			int yval=point.Y+20;
			if(xval+panelHeaderTip.Width>this.Width-vScrollBar1.Width-3){
				xval=this.Width-panelHeaderTip.Width-vScrollBar1.Width-3;
			}
			panelHeaderTip.Location=new Point(xval,yval);
			panelHeaderTip.Visible=true;
		}

		///<summary>Hides it, turns off _isMouseDown and boolApptMoved.</summary>
		private void HideDraggableTempApptSingle() {
			contrTempAppt.Visible=false;
			_boolApptMoved=false;
		}

		private void panelOpHoverTip_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			//Calculate the real point in coordinates of this control
			Point point=new Point(e.X+panelHeaderTip.Left,e.Y+panelHeaderTip.Top);
			HeaderTipDraw(point);
		}

		///<summary>Creates bitmap for contr.  Does not make the contr visible yet.  Also sets mouseIsDown=true.  Pass in the index of the appointment within TableAppointments.</summary>
		private void ShowDraggableApptSingle(int idxAppt) {
			//Control was already created and added to Controls on load. Just reset the fields here and redraw accordingly.
			DataRow dataRow=TableAppointments.Rows[idxAppt];
			SizeF size=SetSize(dataRow["Pattern"].ToString());
			LayoutManager.MoveSize(contrTempAppt,_listApptLayoutInfos[idxAppt].RectangleBounds.Size.ToSize());
				//new Size(Round(size.Width),Round(size.Height));
			if(_isResizingAppt){
				//don't set a clip region. Clear any old clip region.
				contrTempAppt.Region?.Dispose();
				contrTempAppt.Region=null;
			}
			else{
				contrTempAppt.Region?.Dispose();
				contrTempAppt.Region=new Region(GetRoundedPath(new RectangleF(0,0,contrTempAppt.Width,contrTempAppt.Height),_radiusCorn));
			}
			//location is in bitmap coordinates, so convert to parent ControlAppt coords.
			if(_showProvBars){
				LayoutManager.MoveLocation(contrTempAppt,new Point(Round(_listApptLayoutInfos[idxAppt].RectangleBounds.X+_widthTime+_listProvsVisible.Count*_widthProv-hScrollBar1.Value),
					Round(this.Top+_listApptLayoutInfos[idxAppt].RectangleBounds.Y+_heightProvOpHeaders-vScrollBar1.Value)));
			}
			else{
				LayoutManager.MoveLocation(contrTempAppt,new Point(Round(_listApptLayoutInfos[idxAppt].RectangleBounds.X+_widthTime-hScrollBar1.Value),
					Round(this.Top+_listApptLayoutInfos[idxAppt].RectangleBounds.Y+_heightProvOpHeaders-vScrollBar1.Value)));
			}
			_dataRowTempAppt=dataRow;
			SetBitmapTempAppt();
			contrTempAppt.Invalidate();
			contrTempAppt.Visible=false;
			//contrTempAppt.Visible=true;//just for testing
			contrTempAppt.BringToFront();
			//Note: the control is now set to Visible=false. This flag gets flipped elsewhere when the mouse moves. We are just preparing it to be shown here.
		}
				
		private void timerInfoBubble_Tick(object sender,EventArgs e) {
			//not repeating.  Just a single timed event for delay purposes.
			//This is just to catch the rare situation where someone hovers on an appt, then doesn't move.
			//Without a mouseMove event, nothing would draw the bubble.
			int idxAppt=HitTestAppt(_pointBubbleTimer);
			BubbleDraw(_pointBubbleTimer,idxAppt);
			timerBubble.Enabled =false;
		}
		#endregion Methods - Private Mouse Event Helpers
	}

	#region Event Args
	public class ApptDataRowEventArgs:EventArgs {
		public DataRow DataRowAppt { get; set; }
	}

	public class ApptEventArgs:EventArgs {
		public Appointment Appt { get; set; }
	}

	public class ApptFromPinboardEventArgs:EventArgs {
		public DataRow DataRowAppt { get; set; }
		public Bitmap BitmapAppt { get; set; }
		public Point Location { get; set; }
	}

	public class ApptSelectedChangedEventArgs:EventArgs {
		public long PatNumNew { get; set; }
		public long AptNumOld { get; set; }
		public long AptNumNew { get; set; }
	}

	public class ApptMainClickEventArgs:EventArgs {
		public DateTime DateT { get; set; }
		public long OpNum { get; set; }
		public Point Location { get; set; }
	}

	public class ApptMovedEventArgs:EventArgs {
		public Appointment Appt { get; set; }
		public Appointment ApptOld { get; set; }
	}

	public class ApptRightClickEventArgs:EventArgs {
		public Point Location { get; set; }
	}
	#endregion Event Args

	#region Other Classes
	///<summary>Some extra info about each appointment to help with layout, including idxInTableAppointments and RectangleBounds. The two overlap fields allow quick decisions and simple drawing calculations.</summary>
	public class ApptLayoutInfo {
		///<summary>This is the index of the appointment within TableAppointments. We won't duplicate any of that info here, but we can quickly cross reference it.</summary>
		public int idxInTableAppointments;
		///<summary>0 is Monday</summary>
		public int DayOfWeek;
		///<summary></summary>
		public int IdxOp;
		///<summary>For example, if it's taking up 1/3 of the width of an operatory, this would be 3.</summary>
		public int OverlapSections=1;
		///<summary>0-based. For example, if it's 1/3 wide, and in the middle, this would be 1 (the second spot).</summary>
		public int OverlapPosition=0;
		///<summary>This allows efficient hit testing, especially with appointments that are double booked. In main bitmap coordinates.</summary>
		public RectangleF RectangleBounds;

		public override string ToString(){
			return "idx "+idxInTableAppointments.ToString()+", "+OverlapPosition.ToString()+"/"+OverlapSections.ToString();
		}

		public ApptLayoutInfo Copy(){
			return (ApptLayoutInfo)MemberwiseClone();
		}
	}
	#endregion Other Classes
}
