using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace WpfControls.UI {
/*
Jordan is the only one allowed to edit this file.
*/
	///<summary>For a DatePicker, this is the month calendar that comes up when user clicks dropdown.</summary>
	public partial class WindowMonthCalendar:Window {
		#region Fields - Public
		///<summary>The initial point where the UL corner of this picker window should start, in Desktop coordinates.  This point is not in DIPs, but is actual pixels of the entire desktop.  So it could be a very big number if on the righthand monitor at high dpi.</summary>
		public Point PointInitial;
		//<summary>If this window spills off the bottom of the screen, then this is the position is should be </summary>
		//public Point PointInitialAlternateUpper;//probably do this in the parent form instead.
		#endregion Fields - Public

		#region Fields - Private
		private bool _isClosed;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _isMultiSelect=false;
		private List<int> _listIndicesSelected=new List<int>();
		#endregion Fields - Private for Properties

		#region Constructor
		public WindowMonthCalendar() {
			InitializeComponent();
			SetZoom();
			Closing+=WindowComboPicker_Closing;
			Loaded+=WindowComboPicker_Loaded;
			monthCalendar.DateChanged+=MonthCalendar_DateChanged;
			monthCalendar.LostKeyboardFocus+=MonthCalendar_LostKeyboardFocus;
			monthCalendar.AllowClickingTopText=false;
		}
		#endregion Constructor

		#region Events
		///<summary>Occurs when user clicks to change date in a number of different ways.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks to change date in a number of different ways.")]
		public event EventHandler DateChanged;
		#endregion Events

		#region Properties

		#endregion Properties

		#region Methods - public
		///<summary></summary>
		public DateTime GetDateSelected() {
			return monthCalendar.GetDateSelected();
		}

		///<summary></summary>
		public void SetDateSelected(DateTime date) {
			monthCalendar.SetDateSelected(date);
		}
		#endregion Methods - public

		#region Methods - Event Handlers
		private void MonthCalendar_DateChanged(object sender,EventArgs e) {
			DateChanged?.Invoke(this,new EventArgs());
		}

		private void MonthCalendar_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//IInputElement iInputElement=Keyboard.FocusedElement;
			if(_isClosed){
				return;
			}
			Close();
		}

		private void WindowComboPicker_Closing(object sender,CancelEventArgs e) {
			_isClosed=true;
		}

		private void WindowComboPicker_Loaded(object sender,RoutedEventArgs e) {
			//Point point=PointToScreen(PointInitial);//wrong
			System.Drawing.Point drawing_Point=new System.Drawing.Point((int)PointInitial.X,(int)PointInitial.Y);
			System.Drawing.Rectangle drawing_Rectangle=System.Windows.Forms.Screen.GetWorkingArea(drawing_Point);
			Rect rectScreenBounds=new Rect(drawing_Rectangle.X,drawing_Rectangle.Y,drawing_Rectangle.Width,drawing_Rectangle.Height);//Example: {4035,0,3645,2066}
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			double scaleWindows=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			//Another option seems to be:
			//VisualTreeHelper.GetDpi(this).DpiScaleX or PixelsPerDip
			//When specifying the location for this window, it must be in desktop coords, but adjusted to use DIPs.
			//I tried using PointFromScreen to convert back to DIPs, but it gave different results each time, even with the same input.
			Point pointNewDesktop=PointInitial;
			//if(pointNewDesktop.Y+Height>rectScreenBounds.Bottom) {
				//hitting the bottom, so bump it up
//todo: fine tune this:
			//	pointNewDesktop.Y=pointNewDesktop.Y-21-Height;
			//}
			Point pointDIP=new Point(pointNewDesktop.X/scaleWindows,pointNewDesktop.Y/scaleWindows);
			Left=pointDIP.X;
			Top=pointDIP.Y;
			bool isFocused=monthCalendar.Focus();
		}
		#endregion Methods - Event Handlers

		#region Methods - private
		private void SetZoom(){
			if(ComputerPrefs.IsLocalComputerNull()){
				return;
			}
			float zoom=1;
			try{
				zoom=ComputerPrefs.LocalComputer.Zoom/100f;//example 1.2
			}
			catch{
				return;
			}
			if(zoom==0){
				zoom=1;
			}
			LayoutTransform=new ScaleTransform(zoom,zoom);
		}
		#endregion Methods - private

		
	}
}
