using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using OpenDentBusiness;
using OpenDental.Drawing;
using OpenDental.UI;//even though they are in this project

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.
*/
	///<summary></summary>
	public partial class WindowingSlider : UserControl{
		#region Fields - Private
		private DispatcherTimer _dispatcherTimerDelay;
		private EnumSliderButState _enumSliderButStateM = EnumSliderButState.Normal;
		private EnumSliderButState _enumSliderButStateL = EnumSliderButState.Normal;
		private EnumSliderButState _enumSliderButStateR = EnumSliderButState.Normal;
		private bool _isMouseDown;
		private int _maxVal=255;
		private int _minVal=0;
		///<summary>The original pixel position of the button in question. The pointy part.</summary>
		private double _pixRightStart;
		private double _pixLeftStart;
		///<summary>The width, in pixels, of one increment within the allowed 0-255 range.</summary>
		private double _tick;
		///<summary>The width of the end sliders.</summary>
		private double _widthBut=8;
		private double _xMouseDown;
		#endregion Fields - Private

		#region Constructor
		public WindowingSlider(){
			InitializeComponent();
			_dispatcherTimerDelay=new DispatcherTimer();
			_dispatcherTimerDelay.Interval=TimeSpan.FromMilliseconds(300);//this is optimized for 1700x1300 BW.  Feels very nice. 
			//For a 1.3MB pano, it's a little short, because the locking every .3 sec makes it feel slightly sluggish.  But certainly tolerable.
			//An improvement would be to use the file size to tweak larger images to have a longer delay.
			_dispatcherTimerDelay.Tick += timerDelay_Tick;
			MouseLeftButtonDown+=This_MouseLeftButtonDown;
			MouseLeave+=This_MouseLeave;
			MouseMove+=This_MouseMove;
			MouseLeftButtonUp+=This_MouseLeftButtonUp;
		}
		#endregion Constructor

		#region Events
		[Category("OD")]
		[Description("Occurs when the slider moves.  UI is typically updated here.  Also see ScrollComplete")]
		public event EventHandler Scroll;

		[Category("OD")]
		[Description("Occurs when user releases slider after moving.  Database is typically updated here.  Also see Scroll event.")]
		public event EventHandler ScrollComplete;
		#endregion Events

		#region Enums
		///<summary></summary>
		private enum EnumSliderButState {
			///<summary></summary>
			Normal,
			///<summary></summary>
			Hover,
			///<summary>Mouse down. Not a permanent toggle state.</summary>
			Pressed
		}
		#endregion Enums

		#region Properties - Not Browsable
		///<summary>The value of the left slider.</summary>
		[Browsable(false)]
		public int MinVal{
			get{
				if(!IsEnabled){
					return 0;
				}
				return _minVal;
			}
			set{
				_minVal=value;
				Draw();
			}
		}

		///<summary>The value of the right slider, max 255.</summary>
		[Browsable(false)]
		public int MaxVal {
			get {
				if(!IsEnabled){
					return 255;
				}
				return _maxVal;
			}
			set {
				if(value>255){
					_maxVal=255;
				}
				else{
					_maxVal=value;
				}
				Draw();
			}
		}
		#endregion Properties - Not Browsable

		#region Methods public
		public void Draw() {
			_tick=(Width-_widthBut)/255;//gets set in mousemove also
			if(!IsEnabled){
				_minVal=0;//don't use setter or infinite loop
				_maxVal=255;
				Canvas.SetLeft(polygonLeft,_widthBut/2+MinVal*_tick);
				Canvas.SetLeft(polygonRight,_widthBut/2+MaxVal*_tick);
				rectangleBlack.Width=MinVal*_tick;
				rectangleGradient.Visibility=Visibility.Hidden;
				//rectangleGradient.Width=(MaxVal-MinVal)*_tick;
				//Canvas.SetLeft(rectangleGradient,_widthBut/2+MinVal*_tick);
				rectangleWhite.Width=Width-_widthBut-MaxVal*_tick;
				Canvas.SetLeft(rectangleWhite,_widthBut/2+MaxVal*_tick);
				lineTop.X1=_widthBut/2+MinVal*_tick;
				lineTop.X2=_widthBut/2+MaxVal*_tick;
				lineBottom.X1=_widthBut/2+MinVal*_tick;
				lineBottom.X2=_widthBut/2+MaxVal*_tick;
				return;
			}
			Canvas.SetLeft(polygonLeft,_widthBut/2+MinVal*_tick);
			Canvas.SetLeft(polygonRight,_widthBut/2+MaxVal*_tick);
			double widthBlack=MinVal*_tick;
			if(widthBlack<0){
				widthBlack=0;
			}
			rectangleBlack.Width=widthBlack;
			rectangleGradient.Visibility=Visibility.Visible;
			rectangleGradient.Width=(MaxVal-MinVal)*_tick;
			Canvas.SetLeft(rectangleGradient,_widthBut/2+MinVal*_tick);
			double widthWhite=Width-_widthBut-MaxVal*_tick;
			if(widthWhite<0){
				widthWhite=0;
			}
			rectangleWhite.Width=widthWhite;
			Canvas.SetLeft(rectangleWhite,_widthBut/2+MaxVal*_tick);
			lineTop.X1=_widthBut/2+MinVal*_tick;
			lineTop.X2=_widthBut/2+MaxVal*_tick;
			lineBottom.X1=_widthBut/2+MinVal*_tick;
			lineBottom.X2=_widthBut/2+MaxVal*_tick;
			//left
			if(_enumSliderButStateL==EnumSliderButState.Hover){
				polygonLeft.Fill=new SolidColorBrush(Color.FromRgb(229,239,251));
			}
			else if(_enumSliderButStateL==EnumSliderButState.Pressed){
				polygonLeft.Fill=new SolidColorBrush(Color.FromRgb(150,150,160));
			}
			else{
				polygonLeft.Fill=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8CADC"));
			}
			//right
			if(_enumSliderButStateR==EnumSliderButState.Hover){
				polygonRight.Fill=new SolidColorBrush(Color.FromRgb(229,239,251));
			}
			else if(_enumSliderButStateR==EnumSliderButState.Pressed){
				polygonRight.Fill=new SolidColorBrush(Color.FromRgb(150,150,160));
			}
			else{
				polygonRight.Fill=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8CADC"));
			}
		}
		#endregion Methods public

		#region Methods private
		///<summary></summary>
		protected void OnScroll() {
			if(!IsEnabled) {
				return;
			}
			//This is designed to fire with a slight delay, and then to not fire more often than that delay.
			if(_dispatcherTimerDelay.IsEnabled){
				return;//already set to fire shortly
			}
			_dispatcherTimerDelay.IsEnabled=true;
			//the actual event fires in the timer
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private void This_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			if(!IsEnabled) {
				return;
			}
			CaptureMouse();
			_isMouseDown=true;
			_xMouseDown=e.GetPosition(this).X;
			_enumSliderButStateL=EnumSliderButState.Normal;
			_enumSliderButStateR=EnumSliderButState.Normal;
			_enumSliderButStateM=EnumSliderButState.Normal;
			HitTestResult hitTestResult=VisualTreeHelper.HitTest(this,Mouse.GetPosition(this));
			if(hitTestResult==null){
				return;
			}
			if(hitTestResult.VisualHit==grid){
				return;
			}
			if(hitTestResult.VisualHit==polygonLeft){
				_enumSliderButStateL=EnumSliderButState.Pressed;
				_pixLeftStart=(float)MinVal*_tick+_widthBut/2f;
			}
			if(hitTestResult.VisualHit==rectangleGradient){
				_enumSliderButStateM=EnumSliderButState.Pressed;
				_pixLeftStart=(float)MinVal*_tick+_widthBut/2f;
				_pixRightStart=(float)MaxVal*_tick+_widthBut/2f;
			}
			if(hitTestResult.VisualHit==polygonRight){
				_enumSliderButStateR=EnumSliderButState.Pressed;
				_pixRightStart=(float)MaxVal*_tick+_widthBut/2f;
			}
			Draw();
		}

		///<summary>Resets button appearance.  Repaints only if necessary.</summary>
		private void This_MouseLeave(object sender,MouseEventArgs e) {
			if(!IsEnabled) {
				return;
			}
			if(_isMouseDown) {//mouse is down
				//if a button is pressed, it will remain so, even if leave.  As long as mouse is down.
				//,so do nothing.
				//Also, if a button is not pressed, nothing will happen when leave
				//,so do nothing.
			}
			else {//mouse is not down
				_enumSliderButStateL=EnumSliderButState.Normal;
				_enumSliderButStateR=EnumSliderButState.Normal;
				_enumSliderButStateM=EnumSliderButState.Normal;
				Draw();
			}
		}

		private void This_MouseMove(object sender,MouseEventArgs e) { 
			if(!IsEnabled){
				return;
			}
			if(!_isMouseDown){
				_enumSliderButStateL=EnumSliderButState.Normal;
				_enumSliderButStateR=EnumSliderButState.Normal;
				_enumSliderButStateM=EnumSliderButState.Normal;
				HitTestResult hitTestResult=VisualTreeHelper.HitTest(this,Mouse.GetPosition(this));
				if(hitTestResult==null){
					return;	
				}
				if(hitTestResult.VisualHit==grid){
					return;
				}
				if(hitTestResult.VisualHit==polygonLeft){
					_enumSliderButStateL=EnumSliderButState.Hover;
				}
				if(hitTestResult.VisualHit==polygonRight){
					_enumSliderButStateR=EnumSliderButState.Hover;
				}
				Draw();
				return;
			}
			_tick=(float)(Width-_widthBut-1)/255f;
			int minAllowedL=0;
			int maxAllowedL;
			int minAllowedR;
			int maxAllowedR=255;
			double deltaPix=_xMouseDown-e.GetPosition(this).X;
			//use the private versions of _maxVal and _minVal to avoid triggering Draw() and to allow temporary out of bounds numbers.
			if(_enumSliderButStateL==EnumSliderButState.Pressed){
				_minVal=(int)((_pixLeftStart-deltaPix-_widthBut/2f)/_tick);
				maxAllowedL=_maxVal-(int)(_widthBut/_tick);
				if(_minVal<minAllowedL){
					_minVal=minAllowedL;
				}
				else if(_minVal>maxAllowedL){
					_minVal=maxAllowedL;
				}
				OnScroll();
			}
			else if(_enumSliderButStateR==EnumSliderButState.Pressed){
				_maxVal=(int)((_pixRightStart-deltaPix-_widthBut/2f)/_tick);
				minAllowedR=_minVal+(int)(_widthBut/_tick);
				if(_maxVal<minAllowedR){
					_maxVal=minAllowedR;
				}
				else if(_maxVal>maxAllowedR){
					_maxVal=maxAllowedR;
				}
				OnScroll();
			}
			else if(_enumSliderButStateM==EnumSliderButState.Pressed) {
				_minVal=(int)((_pixLeftStart-deltaPix-_widthBut/2f)/_tick);
				_maxVal=(int)((_pixRightStart-deltaPix-_widthBut/2f)/_tick);
				int originalValSpan=Math.Min((int)((_pixRightStart-_pixLeftStart)/_tick),255);//testing showed it was getting set to 256 sometimes, which would cause out of bounds below.
				maxAllowedL=maxAllowedR-originalValSpan;
				minAllowedR=minAllowedL+originalValSpan;
				if(_minVal<minAllowedL) {
					_minVal=minAllowedL;
				}
				else if(_minVal>maxAllowedL) {
					_minVal=maxAllowedL;
				}
				if(_maxVal<minAllowedR) {
					_maxVal=minAllowedR;
				}
				else if(_maxVal>maxAllowedR) {
					_maxVal=maxAllowedR;
				}
				OnScroll();
			}
			Draw();
		}

		///<summary>Change button to hover state and repaint if needed.</summary>
		private void This_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			if(!IsEnabled) {
				return;
			}
			ReleaseMouseCapture();
			_isMouseDown=false;
			if(_enumSliderButStateL==EnumSliderButState.Pressed
				|| _enumSliderButStateR==EnumSliderButState.Pressed
				|| _enumSliderButStateM==EnumSliderButState.Pressed)
			{
				_dispatcherTimerDelay.IsEnabled=false;//cancel any pending Scroll event
				ScrollComplete?.Invoke(this,new EventArgs());
			}
			_enumSliderButStateL=EnumSliderButState.Normal;
			_enumSliderButStateR=EnumSliderButState.Normal;
			_enumSliderButStateM=EnumSliderButState.Normal;
			HitTestResult hitTestResult=VisualTreeHelper.HitTest(this,Mouse.GetPosition(this));
			if(hitTestResult==null){
				return;
			}
			if(hitTestResult.VisualHit==grid){
				return;
			}
			if(hitTestResult.VisualHit==polygonLeft){
				_enumSliderButStateL=EnumSliderButState.Hover;
			}
			if(hitTestResult.VisualHit==polygonRight){
				_enumSliderButStateR=EnumSliderButState.Hover;
			}
			Draw();
		}

		private void timerDelay_Tick(object sender, EventArgs e){
			Scroll?.Invoke(this,new EventArgs());
			_dispatcherTimerDelay.IsEnabled=false;
		}
		#endregion Methods private EventHandlers

	}

	

}
