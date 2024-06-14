using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.UI {

	public partial class ODDatePicker:UserControl {
		#region Fields
		public bool IsCalendarOpen;
		private bool _isReadOnly=false;
		private DateTime _dateTimeDefault=new DateTime(DateTime.Today.Year,1,1);
		private bool _hideCalendarOnLeave=true;
		private Point _pointLocOrigCalendar;
		private EnumCalendarLocation _calendarLocation=EnumCalendarLocation.Below;
		///<summary>Adjustments to the calendar location.  Starting location is based on the CalendarLocation property.</summary>
		private Point _pointAdjustCalendarLoc;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields

		#region Properties
		///<summary>toggles whether the field is read only or not</summary>
		[Browsable(true)]
		[Category("OD")]
		[DefaultValue(false)]
		[Description("sets the control to read only.")]
		public bool ReadOnly {
			get {
				return _isReadOnly;
			}
			set {
				_isReadOnly=value;
				textDate.ReadOnly=_isReadOnly;
			}
		}

		///<summary>Set location of calendar.</summary>
		[Browsable(true)]
		[Category("OD")]
		[DefaultValue(EnumCalendarLocation.Below)]
		[Description("Location where calendar appears relative to the date box.")]
		public EnumCalendarLocation CalendarLocation {//deprecate this
			get {
				return _calendarLocation;
			}
			set {
				_calendarLocation=value;
			}
		}
		
		///<summary>Adjust the position of the calendar.  Starting location is based on the CalendarLocation property.</summary>
		[Browsable(true)]
		[Category("OD")]
		[DefaultValue(typeof(Point),"0,0")]
		[Description("Allows adustment of the calendar location.  Starting location is based on the CalendarLocation property.")]
		public Point AdjustCalendarLocation {//deprecate this
			get {
				return _pointAdjustCalendarLoc;
			}
			set {
				_pointAdjustCalendarLoc=value;
			}
		}

		///<summary>If true, will hide the calendar when focus leaves the control, focus leaves calendar, or user selects a date from calendar. If false, the calendar will stay open until the button is clicked.
		///</summary>
		[Category("OD")]
		[DefaultValue(true)]
		[Description("Set whether the calendar will be hidden when leaving focus from the control.")]
		public bool HideCalendarOnLeave {//deprecate this
			get {
				return _hideCalendarOnLeave;
			}
			set {
				_hideCalendarOnLeave=value;
			}
		}
		#endregion Properties

		#region Properties - Not Browsable
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime DefaultDateTime {//deprecate this
			get {
				return _dateTimeDefault;
			}
			set {
				_dateTimeDefault=value;
			}
		}

		public bool IsValid() {
			return textDate.IsValid();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int DateBoxWidth {//deprecate this
			get {
				return textDate.Width;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point DateBoxLocation {//deprecate this
			get {
				return textDate.Location;
			}
		}
		#endregion Properties - Not Browsable

		#region Constructor
		public ODDatePicker() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			SetDateTime(DefaultDateTime);
			base.Leave+=new System.EventHandler(this.ODDatePicker_Leave);
		}
		#endregion Constructor

		#region Events - Public Raise
		///<summary>Event is fired when the calendar is closed.</summary>
		public event EventHandler CalendarClosed=null;//deprecate
		///<summary>Event is fired when the calendar is opened.</summary>
		public event EventHandler CalendarOpened=null;//deprecate
		///<summary>Event is fired when either calendar has made a selection.</summary>
		public event EventHandler CalendarSelectionChanged=null;
		///<summary>Event is fired when the text in textDate changes.</summary>
		public event EventHandler DateTextChanged=null;//deprecate either this or the event above.
		///<summary>Hiding Control.Leave because the Leave event is fired whenever the user clicks on the calendar. This control will fire this Leave event when the user truly leaves this control.</summary>
		public new event EventHandler Leave;//deprecate?
		#endregion Events - Public Raise

		#region Enumerations
		public enum EnumCalendarLocation {
			Above,
			ToTheRight,
			Below,
		}
		#endregion Enumerations

		#region Methods - Event Handlers
		private void butToggleCalendar_Click(object sender,EventArgs e) {
			if(_isReadOnly) {// do not allow calendar to be toggled open if in read only mode.
				return;
			}
			else {
				ToggleCalendar();
			}
		}

		private void calendar_DateChanged(object sender,EventArgs e) {
			SetDateTime(monthCalendarOD.GetDateSelected());
			Validate();//if there was an error icon showing and they selected a valid date from the calendar, this will make the error icon go away
			if(monthCalendarOD.Visible) {
				CalendarSelectionChanged?.Invoke(this,new EventArgs());
			}
			if(HideCalendarOnLeave) {
				if(monthCalendarOD.Visible) {
					HideCalendar();
					monthCalendarOD.Location=_pointLocOrigCalendar;
					monthCalendarOD.Parent=this;
					CalendarClosed?.Invoke(this,new EventArgs());
				}
			}
		}

		private void monthCalendarOD_Leave(object sender,EventArgs e) {
			//Unfortunately, this doesn't fire when clicking outside the calendar on a blank spot.
			//The solution is to change the calendar from a control into a window.
			//This would also let it spill outside the parent form.
			//That's all fairly straightforward and was done with the combobox popup,
			//but it would take some time, and there are other higher priorities.
			if(HideCalendarOnLeave) {
				if(monthCalendarOD.Visible) {
					HideCalendar();
					monthCalendarOD.Location=_pointLocOrigCalendar;
					monthCalendarOD.Parent=this;
					CalendarClosed?.Invoke(this,new EventArgs());
				}
			}
			if(!Focused){
				Leave?.Invoke(sender,e);
			}
		}

		private void ODDatePicker_Leave(object sender,EventArgs e) {
			if(monthCalendarOD.Focused) {//Still using the calendar.
				this.Focus();//Spoof that we never left
				return;
			}
			if(HideCalendarOnLeave) {
				if(monthCalendarOD.Visible) {
					HideCalendar();
					monthCalendarOD.Location=_pointLocOrigCalendar;
					monthCalendarOD.Parent=this;
					CalendarClosed?.Invoke(this,new EventArgs());
				}
			}
			Leave?.Invoke(sender,e);
		}

		private void ODDatePicker_Load(object sender,EventArgs e) {
			//ChangeCalendarLocation();
			textDate.Text=_dateTimeDefault.ToShortDateString();
			HideCalendar();
			this.textDate.TextChanged+=new System.EventHandler(this.textDate_TextChanged);
		}

		private void textDate_TextChanged(object sender,EventArgs e) {
			DateTextChanged?.Invoke(sender,e);
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);//16x18
			//float scale=LayoutManager.ScaleF(1);
			Rectangle bounds=new Rectangle(
				LayoutManager.Scale(63),
				1,
				LayoutManager.Scale(102),
				LayoutManager.Scale(20));//height gets ignored
			LayoutManager.Move(textDate,bounds);
			bounds=new Rectangle(
				textDate.Right-1-LayoutManager.Scale(16),
				textDate.Top+1,
				LayoutManager.Scale(16),
				textDate.Height-2);//18
			LayoutManager.Move(butDrop,bounds);
		}
		
		private void textDate_SizeChanged(object sender,EventArgs e) {
			Rectangle bounds=new Rectangle(
				textDate.Right-1-LayoutManager.Scale(16),
				textDate.Top+1,
				LayoutManager.Scale(16),
				textDate.Height-2);//18
			LayoutManager.Move(butDrop,bounds);
		}
		#endregion Methods - Event Handlers

		#region Methods
		public DateTime GetDateTime() {//good
			return PIn.Date(textDate.Text);
		}

		public void SetDateTime(DateTime dateTime) {//good
			if(dateTime==DateTime.MinValue) {
				textDate.Text="";
			}
			else {
				textDate.Text=dateTime.ToShortDateString();
				monthCalendarOD.SetDateSelected(dateTime);
			}
		}
		
		///<summary>An empty string does not register as an error in ValidDate.</summary>
		public bool IsEmptyDateTime() {
			return (textDate.Text=="");
		}

		public void HideCalendar() {
			monthCalendarOD.Parent=this;
			monthCalendarOD.Visible=false;
			//this.Height=this.MinimumSize.Height;//We don't use min or max sizes for anything.
			//this.Height=LayoutManager.Scale(22);//this doesn't make any sense, since the height was never increased in the first place.
			IsCalendarOpen=false;
		}

		public void ShowCalendar() {
			//set the date on the calendar to match what's showing in the box
			if(IsValid()) {
				if(textDate.Text=="") {
					//MonthCalendars have to have a selection, and Today's date seems to make the most sense,
					//even if implementors of this control interpret empty dates differently
					monthCalendarOD.SetDateSelected(DateTime.Today);
				}
				else {
					monthCalendarOD.SetDateSelected(PIn.Date(textDate.Text));
				}
			}
			//if(!(this.Parent.Parent is FormODBase)) {//this date picker is on a sub control instead of main form
			//We always do this now
			//so move calendar to the main form for now, and back to this control when done.
			//_isParentChange=true;
			switch(CalendarLocation) {
				case EnumCalendarLocation.Above:
					_pointLocOrigCalendar=new Point(1+LayoutManager.Scale(_pointAdjustCalendarLoc.X),
						textDate.Location.Y-1-LayoutManager.Scale(162)+LayoutManager.Scale(_pointAdjustCalendarLoc.Y));
					break;
				case EnumCalendarLocation.ToTheRight:
					_pointLocOrigCalendar=new Point(textDate.Right+1+LayoutManager.Scale(_pointAdjustCalendarLoc.X),
						textDate.Location.Y-1+LayoutManager.Scale(_pointAdjustCalendarLoc.Y));
					break;
				default://default to below
					_pointLocOrigCalendar=new Point(0+LayoutManager.Scale(_pointAdjustCalendarLoc.X),
						LayoutManager.Scale(21)+LayoutManager.Scale(_pointAdjustCalendarLoc.Y));
					break;
			}
			//calendar.Location=_locationOrigCalendar;//no need for LayoutManager
			Point pointNew=_pointLocOrigCalendar;//calendar.Location;//Start from current context location.
			pointNew=GetPointParentFromPoint(pointNew,monthCalendarOD);//Recursively work out to main form context.
			monthCalendarOD.Location=pointNew;//Set new location.
			monthCalendarOD.Parent=FindForm();
			monthCalendarOD.BringToFront();
			//}
			//show the calendar
			monthCalendarOD.Visible=true;
			IsCalendarOpen=true;
			if(LayoutManager.ScaleMy()>1){
				monthCalendarOD.Size=LayoutManager.ScaleSize(new Size(227, 162));
			}
		}

		///<summary>Recursively calculates relative x-y coordinates up to this control's parent form.</summary>
		private Point GetPointParentFromPoint(Point point,Control control) {
			Form formParent=this.Parent.FindForm();
			if(control.Parent==formParent) {
				return point;//Base case
			}
			point.Y+=control.Parent.Location.Y;
			point.X+=control.Parent.Location.X;
			return GetPointParentFromPoint(point,control.Parent);
		}

		///<summary>Used to set the calendar date when the calendar is open</summary>
		public void SetCalendarDate(DateTime dateTime) {
			if(dateTime.Year < 1880) {//If they passed in a new date time
				monthCalendarOD.SetDateSelected(DateTime.Today);
			}
			else {
				monthCalendarOD.SetDateSelected(dateTime);
			}
		}

		public void ToggleCalendar() {
			if(monthCalendarOD.Visible) {
				HideCalendar();
				CalendarClosed?.Invoke(this,new EventArgs());
				//if(_isParentChange) {//Parent was not an ODForm, set back to original location and parent.
				monthCalendarOD.Location=_pointLocOrigCalendar;
				monthCalendarOD.Parent=this;
				//}
			}
			else {
				ShowCalendar();
				CalendarOpened?.Invoke(this,new EventArgs());
			}
		}

		#endregion Methods
	}
}
