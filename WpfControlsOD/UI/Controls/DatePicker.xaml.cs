using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

Height should be 21
This control is designed to behave just like TextVDate.
Both this and TextVDate were based on the older ValidDate control. 
One additional minor feature that was not present in ValidDate is that when user types, any error clears.
In very old versions of ValidDate, we added slashes as the user typed. We got rid of that and won't bring it back.
When it loses focus, it reformats and adds slashes if necessary.
+/- and up/down will change the date by one day at a time.
This differs a bit from the old ODDatePicker in that the popup month calendar works much better because it's now an actual window.
In the popup, there are buttons at the top for M/Y right/left. 
   -Yes, I know they change the month without changing the selected date.
   -This is intentional so that it's consistent with the same monthCalendar that's used in ContrAppts.
   -Users will be very familiar with that one.
   -Also, this behavior has already been present in ODDatePicker for a year or two.

	*/

	///<summary></summary>
	public partial class DatePicker : UserControl{
		#region Fields - Private
		private string _error;
		private bool _isExpanded;
		private bool _isHover;
		private SolidColorBrush _solidColorBrushHoverBackground=new SolidColorBrush(Color.FromArgb(10,0,100,255));//20% blue wash
		private SolidColorBrush _solidColorBrushHoverBorder=new SolidColorBrush(Color.FromRgb(126,180,234));//blue
		private WindowMonthCalendar _windowMonthCalendar;
		#endregion Fields - Private

		#region Fields - Private for Properties
		
		#endregion Fields - Private for Properties

		#region Constructor
		public DatePicker(){
			InitializeComponent();
			GotKeyboardFocus+=DatePicker_GotKeyboardFocus;
			MouseLeave+=datePicker_MouseLeave;
			MouseLeftButtonUp+=datePicker_MouseLeftButtonUp;
			MouseMove+=datePicker_MouseMove;
			textBox.GotKeyboardFocus+=TextBox_GotKeyboardFocus;
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.PreviewKeyDown+=TextBox_PreviewKeyDown;
			textBox.PreviewTextInput+=TextBox_PreviewTextInput;
			textBox.TextChanged+=TextBox_TextChanged;
			textBox.Text="";//clear out the design time text
			canvasError.Visibility=Visibility.Hidden;
		}
		#endregion Constructor

		#region Events
		//Probably not needed. Or maybe call it TextChanged? Description isn't quite right.
		/////<summary>Occurs when user clicks to change date.  Does not fire in response to programmatic data changes.</summary>
		//[Category("OD")]
		//[Description("Occurs when user clicks to change date.  Does not get raised in response to programmatic data changes.")]
		//public event EventHandler DateChanged;
		#endregion Events

		#region Properties - Public Browsable
		///<summary></summary>
		[Category("OD")]
		[Description("")]
		public string Text {
			get {
				return textBox.Text;
			}
			set {
				textBox.Text=value;
			}
		}
		#endregion Properties - Public Browsable

		#region Properties - Public not Browsable
		
		#endregion Properties - Public not Browsable

		#region Methods - Public
		public DateTime GetDateTime() {
			if(!IsValid()) {
				throw new Exception(_error);
			}
			if(Text=="") {
				return DateTime.MinValue;
			}
			return DateTime.Parse(this.Text);
		}

		///<summary>Returns true if a valid date has been entered. This replaces the older construct: if(textAbcd.errorProvider1.GetError(textAbcd)!="")</summary>
		public bool IsValid() {
			ParseValue();
			return string.IsNullOrEmpty(_error);
		}

		public void SetDateTime(DateTime dateTime) {
			if(dateTime==DateTime.MinValue) {
				Text="";
			}
			else {
				Text=dateTime.ToShortDateString();
			}
			ParseValue();//to set any error
		}
		#endregion Methods - Public

		#region Methods - event handlers
		private void DatePicker_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//in case we add DatePicker to tab order, this will be needed.
			if(e.KeyboardDevice.IsKeyDown(Key.Tab)){
				((System.Windows.Controls.TextBox)sender).SelectAll();
			}
		}

		private void datePicker_MouseLeave(object sender,MouseEventArgs e) {
			_isHover=false;
			SetColors();
		}

		private void datePicker_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			if(e.GetPosition(this).X<gridMain.ColumnDefinitions[0].ActualWidth){
				//clicked in the textbox. We only care about clicks on the dropdown arrow
				return;
			}
			//We're using mouse up to be consistent with our comboBox. See notes over there.
			if(_isExpanded){
				//_windowComboPicker.Close();//It closes automatically when it loses focus, so this isn't necessary
				_isExpanded=false;
				//e.Handled=true;//prevents mousedown from firing, which would expand it again
				return;
			}
			_isExpanded=true;
			_windowMonthCalendar=new WindowMonthCalendar();
			ElementHost.EnableModelessKeyboardInterop(_windowMonthCalendar);
			_windowMonthCalendar.DateChanged+=_windowMonthCalendar_DateChanged;
			if(IsValid()){
				DateTime dateTime=GetDateTime();
				if(dateTime>DateTime.MinValue){
					_windowMonthCalendar.SetDateSelected(dateTime);
				}
			}
			Point pointInitial=PointToScreen(new Point(0,ActualHeight));
			System.Drawing.Point drawing_Point=new System.Drawing.Point((int)pointInitial.X,(int)pointInitial.Y);
			System.Drawing.Rectangle drawing_Rectangle=System.Windows.Forms.Screen.GetWorkingArea(drawing_Point);
			Rect rectScreenBounds=new Rect(drawing_Rectangle.X,drawing_Rectangle.Y,drawing_Rectangle.Width,drawing_Rectangle.Height);//Example: {4035,0,3645,2066}
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			double scaleWindows=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			//Another option seems to be:
			//VisualTreeHelper.GetDpi(this).DpiScaleX or PixelsPerDip
			//When specifying the location for this window, it must be in desktop coords, but adjusted to use DIPs.
			//I tried using PointFromScreen to convert back to DIPs, but it gave different results each time, even with the same input.
			//Point pointNewDesktop=PointInitial;
			//this is in desktop pixels. Zoom was set for _windowMonthCalendar in ctor:
			//_windowMonthCalendar.ActualHeight is zero at this point
			double bottomOfCalendar=pointInitial.Y+_windowMonthCalendar.Height*scaleWindows;
			if(bottomOfCalendar>rectScreenBounds.Bottom) {
				Point pointTopDIP=new Point(0,-_windowMonthCalendar.Height);
				pointInitial=PointToScreen(pointTopDIP);
			}
			_windowMonthCalendar.PointInitial=pointInitial;
			_windowMonthCalendar.Show();
		}

		private void datePicker_MouseMove(object sender,MouseEventArgs e) {
			_isHover=true;
			SetColors();
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(isMouseDown){
				return;
			}
			if(_windowMonthCalendar != null && _windowMonthCalendar.IsVisible){
				_isExpanded=true;
			}
			else{
				_isExpanded=false;
			}
		}

		private void TextBox_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//even though we don't yet support DatePicker in TabOrder, we might eventually.
			if(e.KeyboardDevice.IsKeyDown(Key.Tab)){
				((System.Windows.Controls.TextBox)sender).SelectAll();
			}
		}

		private void TextBox_LostFocus(object sender,RoutedEventArgs e) {
			ParseValue();
		}

		private void TextBox_PreviewKeyDown(object sender,KeyEventArgs e) {
			//if(IsReadOnly) {
			//	return;
			//}
			if(e.Key!=Key.Up && e.Key!=Key.Down){
				return;
			}
			DateTime dateDisplayed;
			try{
				dateDisplayed=DateTime.Parse(Text);
			}
			catch{
				return;
			}
			int caret=textBox.SelectionStart;
			//Only allow a user to add days to the date when the dateDisplay is less than DateTime.MaxValue to avoid UE
			if(e.Key==Key.Up && dateDisplayed.Date<DateTime.MaxValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			//Only allow a user to subtract days to the date when the dateDisplay is greater than DateTime.MinValue to avoid UE
			if(e.Key==Key.Down && dateDisplayed.Date>DateTime.MinValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			textBox.SelectionStart=caret;
			e.Handled=true;
		}

		private void TextBox_PreviewTextInput(object sender,TextCompositionEventArgs e) {
			//if(this.ReadOnly) {
			//	return;
			//} 
			if(e.Text!="+" && e.Text!="-"){
				return;
			}
			//The user might not be done typing in the date.  Make sure that there are at least two non-numeric characters before subtracting days.
			Regex regEx=new Regex("[^0-9]");
			if(regEx.Matches(Text).Count < 2) {
				return;//Not a complete date yet.
			}
			DateTime dateDisplayed;
			try{
				dateDisplayed=DateTime.Parse(Text);
			}
			catch{
				return;
			}
			int caret=textBox.SelectionStart;
			//Only allow a user to add days to the date when the dateDisplay is less than DateTime.MaxValue to avoid UE
			if(e.Text=="+" && dateDisplayed.Date<DateTime.MaxValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			//Only allow a user to subtract days to the date when the dateDisplay is greater than DateTime.MinValue to avoid UE
			if(e.Text=="-" && dateDisplayed.Date>DateTime.MinValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			textBox.SelectionStart=caret;
			e.Handled=true;
		}

		private void _windowMonthCalendar_DateChanged(object sender,EventArgs e) {
			SetDateTime(_windowMonthCalendar.GetDateSelected());
		}

		private void TextBox_TextChanged(object sender,TextChangedEventArgs e) {
			if(!string.IsNullOrEmpty(_error)) {
				ParseValue(doFixes:false);//It's nice to get rid of the error as soon as the user fixes it.
			}
			//TextChanged?.Invoke(this,new EventArgs());//probably not needed. See notes up in event.
		}
		#endregion Methods - event handlers

		#region Methods - private
		///<summary>Set to false to only possibly clear an error without interfering with user typing.</summary>
		private void ParseValue(bool doFixes=true){
			if(Text=="") {
				SetError("");
				return;
			}
			bool allNums = true;
			for(int i = 0;i<Text.Length;i++) {
				if(!Char.IsNumber(Text,i)) {
					allNums=false;
				}
			}
			DateTime dateTime = DateTime.MinValue;
			if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en") {
				if(allNums && doFixes) {
					if(Text.Length==4) {
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2);
					}
					else if(Text.Length==6) {
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,2);
					}
					else if(Text.Length==8) {
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,4);
					}
				}
			}
			try {
				dateTime=DateTime.Parse(this.Text);
			}
			catch {
				SetError("Invalid date.");
				return;
			} 
			if(doFixes){
				Text=dateTime.ToString("d");// allows for year completion if data entered in format MM/DD 
			}
			if(dateTime.Year<1880) {
				SetError("Valid dates between 1880 and 2100");
				return;
			}
			if(dateTime.Year>2100) {
				SetError("Valid dates between 1880 and 2100");
				return;
			}
			SetError("");
		}

		private void SetColors(){
			if(_isHover){
				gridMain.Background=_solidColorBrushHoverBackground;
				//border.BorderBrush=_solidColorBrushHoverBorder;
			}
			else{
				gridMain.Background=Brushes.Transparent;
				//border.BorderBrush=Brushes.DarkGray;
			}
		}

		private void SetError(string error){
			_error=error;
			if(string.IsNullOrEmpty(error)) {
				canvasError.Visibility=Visibility.Hidden;
			}
			else {
				canvasError.Visibility=Visibility.Visible;
				canvasError.ToolTip=error;
			}
		}
		#endregion Methods - private
	
	}

	
}
