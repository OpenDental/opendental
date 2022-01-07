using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.UI {

	public partial class ODDatePicker:UserControl {

		public bool IsCalendarOpen;
		private bool _ReadOnly=false;
		private DateTime _defaultDateTime=new DateTime(DateTime.Today.Year,1,1);
		///<summary>Event is fired when the calendar is opened.</summary>
		public event CalendarOpenedHandler CalendarOpened=null;
		///<summary>Event is fired when the calendar is closed.</summary>
		public event CalendarClosedHandler CalendarClosed=null;
		///<summary>Event is fired when either calendar has made a selection.</summary>
		public event CalendarSelectionHandler CalendarSelectionChanged=null;
		private bool _hideCalendarOnLeave=true;
		//<summary>Always true now</summary>
		//private bool _isParentChange;
		private Point _locationOrigCalendar;
		private CalendarLocationOptions _calendarLocation=CalendarLocationOptions.Below;
		///<summary>Hiding Control.Leave because the Leave event is fired whenever the user clicks on the calendar. This control will fire this Leave
		///event when the user truly leaves this control.</summary>
		public new event EventHandler Leave;
		///<summary>Event is fired with the text in textDate changes.</summary>
		public event DateTextChangedHandler DateTextChanged=null;
		///<summary>Adjustments to the calendar location.  Starting location is based on the CalendarLocation property.</summary>
		private Point _adjustCalendarLocation;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		#region Properties
		///<summary>toggles whether the field is read only or not</summary>
		[Browsable(true)]
		[Category("OD")]
		[DefaultValue(false)]
		[Description("sets the control to read only.")]
		public bool ReadOnly {
			get {
				return _ReadOnly;
            }
			set {
				_ReadOnly=value;
				textDate.ReadOnly=_ReadOnly;
			}
        }

		///<summary>Set location of calendar.</summary>
		[Browsable(true)]
		[Category("OD")]
		[DefaultValue(CalendarLocationOptions.Below)]
		[Description("Location where calendar appears relative to the date box.")]
		public CalendarLocationOptions CalendarLocation {
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
		public Point AdjustCalendarLocation {
			get {
				return _adjustCalendarLocation;
			}
			set {
				_adjustCalendarLocation=value;
			}
		}

		///<summary>If true, will hide the calendar when focus leaves the control. If false, the calendar will stay open until the button is clicked.
		///</summary>
		[Category("OD")]
		[DefaultValue(true)]
		[Description("Set whether the calendar will be hidden when leaving focus from the control.")]
		public bool HideCalendarOnLeave {
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
		public DateTime DefaultDateTime {
			get {
				return _defaultDateTime;
			}
			set {
				_defaultDateTime=value;
			}
		}

		public bool IsValid() {
			return textDate.IsValid();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int DateBoxWidth {
			get {
				return textDate.Width;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point DateBoxLocation {
			get {
				return textDate.Location;
			}
		}
		#endregion Properties - Not Browsable

		public ODDatePicker() {
			InitializeComponent();
			SetDateTime(DefaultDateTime);
			base.Leave+=new System.EventHandler(this.ODDatePicker_Leave);
		}

		/*
		public void ChangeCalendarLocation() {
			switch(CalendarLocation) {
				case CalendarLocationOptions.Above:
					_locationOrigCalendar=new Point(1+_adjustCalendarLocation.X,textDate.Location.Y-1-calendar.Height+_adjustCalendarLocation.Y);
					break;
				case CalendarLocationOptions.ToTheRight:
					_locationOrigCalendar=new Point(textDate.Location.X+textDate.Width+1+_adjustCalendarLocation.X,textDate.Location.Y-1+_adjustCalendarLocation.Y);
					break;
				default://default to below
					_locationOrigCalendar=new Point(calendar.Location.X+_adjustCalendarLocation.X,calendar.Location.Y+_adjustCalendarLocation.Y);
					break;
			}
			calendar.Location=_locationOrigCalendar;//no need for LayoutManager
		}*/

		public DateTime GetDateTime() {
			return PIn.Date(textDate.Text);
		}

		// allows pulling the masked date, if date is masked.
		public string GetTextDate() {
			return textDate.Text;
        }

		//allow the date to show the masked value rather than actual date
		public void SetMaskedDate(string maskedDate) {
			textDate.Text=maskedDate;
        }

		public void SetDateTime(DateTime dateTime) {
			if(dateTime==DateTime.MinValue) {
				textDate.Text="";
			}
			else {
				textDate.Text=dateTime.ToShortDateString();
				calendar.SetDate(dateTime);
			}
		}
		
		///<summary>An empty string does not register as an error in ValidDate.</summary>
		public bool IsEmptyDateTime() {
			return (textDate.Text=="");
		}

		private void ODDatePicker_Load(object sender,EventArgs e) {
			//ChangeCalendarLocation();
			textDate.Text=_defaultDateTime.ToShortDateString();
			HideCalendar();
			this.textDate.TextChanged+=new System.EventHandler(this.textDate_TextChanged);
		}
		
		private void butToggleCalendar_Click(object sender,EventArgs e) {
            if(_ReadOnly) {// do not allow calendar to be toggled open if in read only mode.
				return;
            }
            else {
				ToggleCalendar();
			}
			
		}

		public void ToggleCalendar() {
			if(calendar.Visible) {
				HideCalendar();
				CalendarClosed?.Invoke(this,new EventArgs());
				//if(_isParentChange) {//Parent was not an ODForm, set back to original location and parent.
				calendar.Location=_locationOrigCalendar;
				calendar.Parent=this;
				//}
			}
			else {
				ShowCalendar();
				CalendarOpened?.Invoke(this,new EventArgs());
			}
		}

		public void HideCalendar() {
			calendar.Parent=this;
			calendar.Visible=false;
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
					calendar.SetDate(DateTime.Today);
				}
				else {
					calendar.SetDate(PIn.Date(textDate.Text));
				}
			}
			//if(!(this.Parent.Parent is FormODBase)) {//this date picker is on a sub control instead of main form
			//We always do this now
			//so move calendar to the main form for now, and back to this control when done.
			//_isParentChange=true;
			switch(CalendarLocation) {
				case CalendarLocationOptions.Above:
					_locationOrigCalendar=new Point(1+LayoutManager.Scale(_adjustCalendarLocation.X),
						textDate.Location.Y-1-LayoutManager.Scale(162)+LayoutManager.Scale(_adjustCalendarLocation.Y));
					break;
				case CalendarLocationOptions.ToTheRight:
					_locationOrigCalendar=new Point(textDate.Right+1+LayoutManager.Scale(_adjustCalendarLocation.X),
						textDate.Location.Y-1+LayoutManager.Scale(_adjustCalendarLocation.Y));
					break;
				default://default to below
					_locationOrigCalendar=new Point(0+LayoutManager.Scale(_adjustCalendarLocation.X),
						LayoutManager.Scale(21)+LayoutManager.Scale(_adjustCalendarLocation.Y));
					break;
			}
			//calendar.Location=_locationOrigCalendar;//no need for LayoutManager
			Point locNew=_locationOrigCalendar;//calendar.Location;//Start from current context location.
			locNew=GetParentFromPoint(locNew,calendar);//Recursively work out to main form context.
			calendar.Location=locNew;//Set new location.
			calendar.Parent=Parent.FindForm();
			calendar.BringToFront();
			//}
			//show the calendar
			calendar.Visible=true;
			IsCalendarOpen=true;
			if(LayoutManager.ScaleMy()>1){
				calendar.Size=LayoutManager.ScaleSize(new Size(227, 162));
			}
		}

		///<summary>Recursively calculates relative x-y coordinates up to this control's parent form.</summary>
		private Point GetParentFromPoint(Point locCur,Control contrCur) {
			Form form=this.Parent.FindForm();
			if(contrCur.Parent==form) {
				return locCur;//Base case
			}
			locCur.Y+=contrCur.Parent.Location.Y;
			locCur.X+=contrCur.Parent.Location.X;
			return GetParentFromPoint(locCur,contrCur.Parent);
		}

		///<summary>Used to set the calendar date when the calendar is open</summary>
		public void SetCalendarDate(DateTime dateTime) {
			if(dateTime.Year < 1880) {//If they passed in a new date time
				calendar.SetDate(DateTime.Today);
			}
			else {
				calendar.SetDate(dateTime);
			}
		}

		private void calendar_DateSelected(object sender,DateRangeEventArgs e) {
			SetDateTime(calendar.SelectionStart);
			Validate();//if there was an error icon showing and they selected a valid date from the calendar, this will make the error icon go away
			if(calendar.Visible) {
				CalendarSelectionChanged?.Invoke(this,new EventArgs());
			}
		}

		private void textDate_TextChanged(object sender,EventArgs e) {
			DateTextChanged?.Invoke(sender,e);
		}

		private void ODDatePicker_Leave(object sender,EventArgs e) {
			if(calendar.Focused) {//Still using the calendar.
				this.Focus();//Spoof that we never left
				return;
			}
			if(HideCalendarOnLeave) {
				if(calendar.Visible) {
					HideCalendar();
					//if(_isParentChange) {//Parent was not an ODForm, set back to original location and parent.
					calendar.Location=_locationOrigCalendar;
					calendar.Parent=this;
					//}
					CalendarClosed?.Invoke(this,new EventArgs());
				}
			}
			Leave?.Invoke(sender,e);
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

		public enum CalendarLocationOptions {
			Above,
			ToTheRight,
			Below,
		}

		
	}

	public delegate void CalendarClosedHandler(object sender,EventArgs e);

	public delegate void CalendarSelectionHandler(object sender,EventArgs e);

	public delegate void CalendarOpenedHandler(object sender,EventArgs e);

	public delegate void DateTextChangedHandler(object sender,EventArgs e);
}
