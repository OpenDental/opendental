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
	public partial class ZoomSlider : UserControl{
		#region Fields -Private
		///<summary>The time that the mouse button was pressed. Helps us distinguish a click from a drag.</summary>
		private DateTime _dateTimeMouseDown;
		///<summary>True if user hit esc while in the textbox. We don't want to save the value they entered.</summary>
		private bool _didTextHitEsc;
		///<summary>This is the zoom level that's calculated in SetValueInitialFit and then constantly used to derive other values like _maximum.</summary>
		private double _fit;
		///<summary>True if mouse down was inside the rectangleSlider.</summary>
		private bool _isMouseDownInRectangleSlider;
		///<summary>Mouse down was anywhere in the tickLineMargin. Either a drag or a click.</summary>
		private bool _isMouseDownTickLineMargin;
		private bool _isMouseDown;
		///<summary>This changes constantly and is not under external control.  It starts out as twice _fit, but then increases more each time user hits "plus".</summary>
		private double _maximum=100;
		///<summary>Current position of mouse on control.  -1,-1 indicates not over control.</summary>
		private Point _pointMouse=new Point(-1,-1);
		///<summary>Point where mouse was initially down.  If dragging, it will get the original position.</summary>
		private Point _pointMouseDown;
		///<summary>The rectangle defining the minus button</summary>
		private Rectangle rectangleMinus;
		///<summary>The rectangle defining the plus button</summary>
		private Rectangle rectanglePlus;
		///<summary>The rectangle defining the Fit button</summary>
		private Rectangle rectangleFit;
		///<summary>The rectangle defining the 100 button</summary>
		private Rectangle rectangle100;
		///<summary>The rectangle defining tick line itself. We never draw a rectangle around it, but it's useful for lots of math. It spans the height of the entire control. It extends left and right to the extent of the tick line.</summary>
		private Rect _rectTickLine;
		///<summary>The rectangle defining the tickline plus left and right padding. So the entire cell.</summary>
		private Rect _rectTickLineMargin;
		private TextBox textBoxEdit;
		private TranslateTransform _translateTransform=new TranslateTransform();
		///<summary>When mouse down to start dragging slider, we aren't necessarily in the center.  This keeps track of offset L/R.</summary>
		private double _xOffsetMouseDownSlider;
		///<summary>Property backer.</summary>
		private double _value=1;
		///<summary>This lets us avoid single digit changes from just clicking on the number. Only set when _isMouseDownInRectangleSlider.</summary>
		private double _valueWhenMouseDown;
		//private DispatcherTimer dispatcherTimer;
		#endregion Fields -Private

		#region Constructor
		public ZoomSlider(){
			InitializeComponent();
			FontSize=11.5;
			FontFamily=new FontFamily("Segoe UI");
			Loaded+=ZoomSlider_Loaded;
			MouseLeftButtonDown+=this_MouseLeftButtonDown;
			MouseLeftButtonUp+=this_MouseLeftButtonUp;
			MouseLeave+=this_MouseLeave;
			MouseMove+=this_MouseMove;
			textBoxEdit=new TextBox();
			System.Windows.Controls.Panel.SetZIndex(textBoxEdit,3);//to make is show on top of textBlock which has zindex 2
			textBoxEdit.LostFocus += TextBoxEdit_LostFocus;
			textBoxEdit.KeyDown += TextBoxEdit_KeyDown;
			textBoxEdit.KeyUp += TextBoxEdit_KeyUp;
			textBoxEdit.PreviewKeyDown+=TextBoxEdit_PreviewKeyDown;
		}

		private void ZoomSlider_Loaded(object sender,RoutedEventArgs e) {
			//None of these rectangles move.
			//colors are handled in Draw to make them show.
			//rectangleMinus--------------------------------------------------
			rectangleMinus=new Rectangle();
			Canvas.SetLeft(rectangleMinus,0);//actually required or it's NaN
			rectangleMinus.Width=18;
			rectangleMinus.Height=ActualHeight-1;
			rectangleMinus.Fill=Brushes.Transparent;
			rectangleMinus.Stroke=Brushes.Transparent;
			canvas.Children.Add(rectangleMinus);
			//rectanglePlus---------------------------------------------------
			rectanglePlus=new Rectangle();
			Canvas.SetLeft(rectanglePlus,158);
			rectanglePlus.Width=17;
			rectanglePlus.Height=ActualHeight-1;
			rectanglePlus.Fill=Brushes.Transparent;
			rectanglePlus.Stroke=Brushes.Transparent;
			canvas.Children.Add(rectanglePlus);
			//rectangleFit--------------------------------------------------
			rectangleFit=new Rectangle();
			Canvas.SetLeft(rectangleFit,175);
			rectangleFit.Width=27;
			rectangleFit.Height=ActualHeight-1;
			rectangleFit.Fill=Brushes.Transparent;
			rectangleFit.Stroke=Brushes.Transparent;
			canvas.Children.Add(rectangleFit);
			//rectangle100---------------------------------------------------
			rectangle100=new Rectangle();
			Canvas.SetLeft(rectangle100,202);
			rectangle100.Width=29;
			rectangle100.Height=ActualHeight-1;
			rectangle100.Fill=Brushes.Transparent;
			rectangle100.Stroke=Brushes.Transparent;
			canvas.Children.Add(rectangle100);
			canvas.Children.Add(textBoxEdit);
			textBoxEdit.Visible=false;
			textBoxEdit.PaddingOD=new Thickness(0,top:-1,0,0);
			Draw();
			//int tier=RenderCapability.Tier >> 16;//2
		}
		#endregion Constructor

		#region Events
		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when user presses Fit or 100 button because parent form needs to recenter image.")]
		public event EventHandler EventResetTranslation;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when user changes the zoom level.  It fires repeatedly while dragging.")]
		public event EventHandler EventZoomed;
		#endregion Events

		#region Properties - Not Browsable
		///<summary>The numeric zoom value, greater than zero.  To set, use SetValueInitialFit or SetValueAndMax method.</summary>
		[Browsable(false)]
		public int Value {
			get {
				if(Round(_value)<1){
					return 1;
				}
				return Round(_value);
			}
			//set { 
			//	_value=value;
			//	this.Invalidate();
			//}
		}
		#endregion Properties - Not Browsable

		#region Methods - Public
		///<summary>These parameters are required because initial center zoom is always set to "fit". This is also the only way to set Enabled to true.  If degreesRotated is is 90 or 270, it will swap width and height when doing the calc.</summary>
		public void SetValueInitialFit(Size sizeCanvas,Size sizeImage,float degreesRotated){
			System.Drawing.Size d_sizeCanvas=new System.Drawing.Size(Round(sizeCanvas.Width),Round(sizeCanvas.Height));
			System.Drawing.Size d_sizeImage=new System.Drawing.Size(Round(sizeImage.Width),Round(sizeImage.Height));
			_fit=ImageTools.CalcScaleFit(d_sizeCanvas,d_sizeImage,degreesRotated)*100;
			_value=_fit;
			_maximum=2*_fit;//starting point
			if(Round(_value)>_maximum){
				_maximum=Round(_value);
			}
			IsEnabled=true;
			Draw();
		}

		///<summary>The number passed in here is a zoom change, like -15.  It was calculated externally based on existing _value for proportionality.</summary>
		public void SetByWheel(double deltaZoom){
			SetValueAndMax(_value+deltaZoom);   
			Draw();
		}

		///<summary>Sets value.  If value is greater than Max, then Max will grow.</summary>
		public void SetValueAndMax(double newval){
			//Converting to a float causes the int.maxvalue to still wrap around to a negative value, removed 100 from the maximum to avoid this error.
			if(newval>=double.MaxValue){
				newval=double.MaxValue-100;//probably ok to remove
			}
			_value=newval;
			if(_value<1){
				_value=1;
			}
			if(_value>_maximum){
				_maximum=_value;
			}
			Draw();
		}
		#endregion Methods - Public

		#region Methods private
		private void Draw(){
			//This method sets position, visibilities, and colors of various existing objects.
			if(_maximum<5) {
				return;
			}
			if(rectanglePlus is null){
				return;//still loading
			}
			//Calculate a few rectangles
			Graphics graphics=Graphics.MeasureBegin();
			//We add some fluff to the measurements below so that the textbox can be the same size.
			double widthOne=graphics.MeasureString("1").Width+3;//because "1" is the minimum value
			//_maximum won't change while mouse is down because drag can't take _value to larger than _maximum
			double widthMax=graphics.MeasureString(Round(_maximum).ToString()).Width+3;
			double widthValue=graphics.MeasureString(Round(_value).ToString()).Width+3;
			double bufferLeft=widthOne/2;
			double bufferRight=widthMax/2;
			_rectTickLineMargin=new Rect(
				rectangleMinus.Width,
				y: 0,
				width: Canvas.GetLeft(rectanglePlus)-rectangleMinus.Width,
				height: ActualHeight-2);
			_rectTickLine=new Rect(_rectTickLineMargin.Left+bufferLeft,0,_rectTickLineMargin.Width-bufferLeft-bufferRight,ActualHeight);
			//xVal is the x position on the slider, in pixels, of _value.
			double xValuePixels=_rectTickLine.Left
				+((_rectTickLine.Right-_rectTickLine.Left)//how many pixels in tick line
				/(_maximum-1) *_value);
			double leftSlider=xValuePixels-(widthValue/2);
			Canvas.SetLeft(rectangleSlider,leftSlider);
			rectangleSlider.Width=widthValue;
			Canvas.SetLeft(polygonTriangle,xValuePixels);
			Canvas.SetLeft(textBlock,leftSlider+3);
			if(_isMouseDown){
				LinearGradientBrush linearGradientBrush=new LinearGradientBrush(Color.FromRgb(225,225,225),Color.FromRgb(141,151,179),angle:90);
				if(HitTest(rectangleMinus,_pointMouseDown)){
					rectangleMinus.Fill=linearGradientBrush;
					rectangleMinus.Stroke=Brushes.SlateGray;
				}
				else{
					rectangleMinus.Fill=Brushes.Transparent;
					rectangleMinus.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectanglePlus,_pointMouseDown)){
					rectanglePlus.Fill=linearGradientBrush;
					rectanglePlus.Stroke=Brushes.SlateGray;
				}
				else{
					rectanglePlus.Fill=Brushes.Transparent;
					rectanglePlus.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectangleFit,_pointMouseDown)){
					rectangleFit.Fill=linearGradientBrush;
					rectangleFit.Stroke=Brushes.SlateGray;
				}
				else{
					rectangleFit.Fill=Brushes.Transparent;
					rectangleFit.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectangle100,_pointMouseDown)){
					rectangle100.Fill=linearGradientBrush;
					rectangle100.Stroke=Brushes.SlateGray;
				}
				else{
					rectangle100.Fill=Brushes.Transparent;
					rectangle100.Stroke=Brushes.Transparent;
				}
			}
			else{//Mouse not down. Outline any hover button
				if(HitTest(rectangleMinus,_pointMouse)){
					rectangleMinus.Fill=Brushes.Transparent;
					rectangleMinus.Stroke=Brushes.SlateGray;
				}
				else{
					rectangleMinus.Fill=Brushes.Transparent;
					rectangleMinus.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectanglePlus,_pointMouse)){
					rectanglePlus.Fill=Brushes.Transparent;
					rectanglePlus.Stroke=Brushes.SlateGray;
				}
				else{
					rectanglePlus.Fill=Brushes.Transparent;
					rectanglePlus.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectangleFit,_pointMouse)){
					rectangleFit.Fill=Brushes.Transparent;
					rectangleFit.Stroke=Brushes.SlateGray;
				}
				else{
					rectangleFit.Fill=Brushes.Transparent;
					rectangleFit.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectangle100,_pointMouse)){
					rectangle100.Fill=Brushes.Transparent;
					rectangle100.Stroke=Brushes.SlateGray;
				}
				else{
					rectangle100.Fill=Brushes.Transparent;
					rectangle100.Stroke=Brushes.Transparent;
				}
				if(HitTest(rectangleSlider,_pointMouse)){
					rectangleSlider.Stroke=new SolidColorBrush(Color.FromRgb(50,50,130));//blue outline hover
				}
				else{
					rectangleSlider.Stroke=new SolidColorBrush(Color.FromRgb(180,180,180));//#b4b4b4
				}
			}
			Color colorText=Colors.Black;
			if(!IsEnabled){
				colorText=OpenDental.ColorOD.Gray_Wpf(160);
			}
			lineMinus.Stroke=new SolidColorBrush(colorText);
			linePlus1.Stroke=new SolidColorBrush(colorText);
			linePlus2.Stroke=new SolidColorBrush(colorText);
			textFit.Foreground=new SolidColorBrush(colorText);
			text100.Foreground=new SolidColorBrush(colorText);
			textBlock.Foreground=new SolidColorBrush(colorText);
			//Tick line
			//int yTickLine = 4;
			lineHoriz.X1=_rectTickLine.X;
			lineHoriz.X2=_rectTickLine.Right;
			lineVertLeft.X1=_rectTickLine.X;
			lineVertLeft.X2=_rectTickLine.X;
			double xFitPixels = (_rectTickLine.Right-_rectTickLine.Left)//how many pixels to the Fit tick line
				/(_maximum-1) *_fit;
			lineVertFit.X1=_rectTickLine.X+xFitPixels;
			lineVertFit.X2=_rectTickLine.X+xFitPixels;
			lineVertRight.X1=_rectTickLine.Right;
			lineVertRight.X2=_rectTickLine.Right;
			string sVal = Round(_value).ToString();
			textBlock.Text=sVal;
		}

		///<summary>Returns true if the point is within the rectangle. Doesn't bother with Y for this control.</summary>
		private bool HitTest(Rectangle rectangle,Point point){
			double x=Canvas.GetLeft(rectangle);
			if(point.X<x){
				return false;
			}
			if(point.X>x+rectangle.Width-1){
				return false;
			}
			return true;
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private int Round(double num){
			return (int)Math.Round(num);
		}

		private void TextBoxEdit_KeyDown(object sender, KeyEventArgs e){
			if(e.Key==Key.Enter){
				try{
					SetValueAndMax(int.Parse(textBoxEdit.Text));
				}
				catch{}
				textBoxEdit.Visible=false;
			}
			EventZoomed?.Invoke(this, new EventArgs());
		}

		private void TextBoxEdit_KeyUp(object sender, KeyEventArgs e){
			Graphics g=Graphics.MeasureBegin();
			double widthText=g.MeasureString(textBoxEdit.Text).Width;
			if(textBoxEdit.Width<widthText+3){
				textBoxEdit.Width=widthText+3;
				//textBoxEdit.ScrollToCaret();
			}
			Draw();
		}

		private void TextBoxEdit_LostFocus(object sender, EventArgs e){
			if(!_didTextHitEsc){
				try{
					SetValueAndMax(int.Parse(textBoxEdit.Text));
				}
				catch{
				}
			}
			textBoxEdit.Visible=false;
			Draw();
			EventZoomed?.Invoke(this, new EventArgs());
		}

		private void TextBoxEdit_PreviewKeyDown(object sender, KeyEventArgs e){
			if(e.Key==Key.Escape){
				_didTextHitEsc=true;
				textBoxEdit.Visible=false;
			}
			Draw();
		}

		private void this_MouseLeave(object sender,MouseEventArgs e) {
			_pointMouse=new Point(-1,-1);
			Draw();
		}

		private void this_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			CaptureMouse();
			_isMouseDown=true;
			_dateTimeMouseDown=DateTime.Now;
			_isMouseDownTickLineMargin=false;//unless set true down below
			_isMouseDownInRectangleSlider=false;
			_pointMouseDown=e.GetPosition(this);
			if(!HitTest(rectangleSlider,_pointMouseDown)) {//if not inside the slider box
				textBoxEdit.Visible=false;//then the edit box should go away. New value will depend on where clicked
			}
			if(HitTest(rectangleMinus,_pointMouseDown)) {
				_value=_value*.5f;
				if(_value<1) {
					_value=1;
				}
				EventZoomed?.Invoke(this,new EventArgs());
			}
			if(HitTest(rectanglePlus,_pointMouseDown)) {
				SetValueAndMax(_value+_maximum*.25f);
				EventZoomed?.Invoke(this,new EventArgs());
			}
			if(HitTest(rectangleFit,_pointMouseDown)) {
				SetValueAndMax(_fit);
				EventResetTranslation?.Invoke(this,new EventArgs());
				EventZoomed?.Invoke(this,new EventArgs());
			}
			if(HitTest(rectangle100,_pointMouseDown)) {
				SetValueAndMax(100);
				EventResetTranslation?.Invoke(this,new EventArgs());
				EventZoomed?.Invoke(this,new EventArgs());
			}
			if(_rectTickLineMargin.Contains(_pointMouseDown)) {
				_isMouseDownTickLineMargin=true;
				if(HitTest(rectangleSlider,_pointMouseDown)) {
					_xOffsetMouseDownSlider=Canvas.GetLeft(rectangleSlider)+rectangleSlider.Width/2-_pointMouseDown.X;
					_isMouseDownInRectangleSlider=true;//The Draw will move rectangleSlider, so we have to remember it here.
					_valueWhenMouseDown=_value;
				}
				else if(_rectTickLine.Contains(_pointMouseDown)) {
					_xOffsetMouseDownSlider=0;
					double xPixels = _pointMouseDown.X-_rectTickLine.Left;
					_value=(float)xPixels/_rectTickLine.Width*_maximum;
					EventZoomed?.Invoke(this,new EventArgs());
				}
			}
			Draw();
		}

		private void this_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			ReleaseMouseCapture();
			_isMouseDown=false;
			DateTime dateTimeNow=DateTime.Now;//for debugging
			Point point = e.GetPosition(this);
			if(!_isMouseDownTickLineMargin) {
				Draw();
				return;
			}
			if(dateTimeNow>_dateTimeMouseDown.Add(TimeSpan.FromSeconds(1))){//held button down for more than one second
				//so don't treat it as a click
				Draw();
				return;
			}
			if(point.X<_pointMouseDown.X-2 || point.X>_pointMouseDown.X+2){//moved more than 2px
				//don't treat it as a click
				Draw();
				return;
			}
			if(!_isMouseDownInRectangleSlider) {
				//We don't want textbox to pop up
				Draw();
				return;
			}
			//value might have changed slightly because mousemove can get triggered even when user doesn't move mouse.
			_value=_valueWhenMouseDown;
			double x = Canvas.GetLeft(rectangleSlider);
			Canvas.SetLeft(textBoxEdit,x);
			double y = Canvas.GetTop(rectangleSlider);
			Canvas.SetTop(textBoxEdit,y);
			textBoxEdit.Width=rectangleSlider.Width;
			textBoxEdit.Height=rectangleSlider.Height;
			textBoxEdit.Visible=true;
			_didTextHitEsc=false;
			textBoxEdit.Text=Round(_value).ToString();
			OpenDental.FrmODBase.DoEvents();
			//because the visible setting has not yet propagated to the nested textbox, so Focus would not work.
			textBoxEdit.SelectAll();
			Draw();
		}

		private void this_MouseMove(object sender,MouseEventArgs e) {
			//if(Mouse.LeftButton==MouseButtonState.Released) {
			//	return;
			//}
			_pointMouse=e.GetPosition(this);
			if(_isMouseDown) {
				if(_isMouseDownTickLineMargin) {
					//They are dragging
					//as they move, we change _value
					if(_pointMouse.X+_xOffsetMouseDownSlider<_rectTickLine.Left) {
						_value=1;
					}
					else if(_pointMouse.X+_xOffsetMouseDownSlider>_rectTickLine.Right) {
						_value=_maximum;
					}
					else {
						double xPixels = _pointMouse.X+_xOffsetMouseDownSlider-_rectTickLine.Left;
						_value=xPixels/_rectTickLine.Width*_maximum;
					}
					EventZoomed?.Invoke(this,new EventArgs());
				}
			}
			Draw();
			//Debug.WriteLine(_pointMouse.X.ToString());
		}
		#endregion Methods private EventHandlers

	}

	

}

/*I did extensive testing to try to speed up the drawing so that it wouldn't lag behind the mouse.
Here's a summary:
			//Canvas.SetLeft(rectangleSlider,_pointMouse.X);
			//Canvas.SetTop(rectangleSlider,_pointMouse.Y);
			//Above was laggy
			//rectangleSlider.Margin=new Thickness(_pointMouse.X,_pointMouse.Y,0,0);
			//Above was equally laggy
			//_pointMouse=Mouse.GetPosition(this);
			//canvas.Children.Clear();
			//Rectangle rectangleSlider = new Rectangle();
			//rectangleSlider.Stroke=new SolidColorBrush(Color.FromRgb(180,180,180));
			//rectangleSlider.Fill=Brushes.White;
			//rectangleSlider.Width=26;
			//rectangleSlider.Height=15.5;
			//Canvas.SetLeft(rectangleSlider,_pointMouse.X);
			//Canvas.SetTop(rectangleSlider,_pointMouse.Y);
			//canvas.Children.Add(rectangleSlider);
			//Above was equally laggy
			//using override OnMouseMove was equally laggy
			//tracking dateTimeLastMouse and disregarding frequent events was tried two different ways:
			//  1. 10 ms, no difference. Still equally laggy.
			//  2. 100 ms jumpy and laggy.
			//Running it in release instead of debug did not improve performance at all.
			//While using Performance Profiler in debug, lag improved, but could never figure out why.
			//visualHost.CreateRectangle(_pointMouse.X,_pointMouse.Y);
			//Using the DrawingVisual host above had the same lag.
			//getting rid of all other elements had the same lag.
			//Switching from 4k to HD had the same lag.
			//I then started a series of attempts using CompositionTarget
			//Mouse.GetPosition(this);//too slow
			//Next, dllImport GetCursorPos, combined with PointFromScreen: Too slow
			//https://stackoverflow.com/questions/69826441/draw-fast-graphics-in-wpf-directly-instead-of-using-compositiontarget-rendering
			//says that even compositiontarget is slow
			//Using DispatcherTimer at 10ms with Mouse.GetPosition and rectangle Margin was laggy and jittery
			//Using DispatcherTimer at 10ms with dllImport GetCursorPos and rectangle Margin was laggy and jittery
			//Using DispatcherTimer at 1ms with dllImport GetCursorPos and rectangle Margin was suprisingly laggy and jittery
			//Using DispatcherTimer at 1ms with dllImport GetCursorPos and new rectangle on Canvas was laggy and jittery
			//Turned off grid SnapToDevicePixels. Using DispatcherTimer at 1ms with dllImport GetCursorPos and new rectangle on Canvas was laggy and jittery
			//WriteableBitmap worked. Zero lag. But also no access to drawing text.
			//I then verified that the stock WPF scrollbar also has exactly the same lag.
			//So I've explored the limits of what WPF can do. There's just a small baseline lag compared to WinForms.
			//I know I can get better performance by switching to DirectX, but that's quite complex. That's what I'll do for the Imaging module.
			//I could also just use WinForms for this control.
			//Pros: Crisp response
			//Cons: bad organization, hard to move to future framework, 
			//But those are all weak pros and cons.
*/


/*Ignore this DrawingVisual example
//public class MyVisualHost:FrameworkElement {
	//	//from https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/using-drawingvisual-objects?view=netframeworkdesktop-4.8
	//I'm leaving this here in case I need it some day
	//	private VisualCollection _children;

	//	public MyVisualHost() {
	//		_children = new VisualCollection(this);
	//		//_children.Add(CreateDrawingVisualRectangle());
	//		//this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
	//	}

	//	public void CreateRectangle(double x,double y){
	//		_children.Clear();
	//		DrawingVisual drawingVisual = new DrawingVisual();
	//		DrawingContext drawingContext = drawingVisual.RenderOpen();
	//		Rect rect = new Rect(new Point(x,y),new Size(26,15.5));
	//		drawingContext.DrawRectangle(Brushes.White,new Pen(Brushes.Gray,1),rect);
	//		// Persist the drawing content.
	//		drawingContext.Close();
	//		//return drawingVisual;
	//		_children.Add(drawingVisual);
	//	}

	//	//private DrawingVisual CreateDrawingVisualRectangle() {
		
	//	//}

	//	protected override int VisualChildrenCount {
	//		get {
	//			return _children.Count;
	//		}
	//	}

	//	// Provide a required override for the GetVisualChild method.
	//	protected override Visual GetVisualChild(int index) {
	//		if(index < 0 || index >= _children.Count) {
	//			throw new ArgumentOutOfRangeException();
	//		}

	//		return _children[index];
	//	}

	//	void MyVisualHost_MouseLeftButtonUp(object sender,System.Windows.Input.MouseButtonEventArgs e) {
	//		//System.Windows.Point pt = e.GetPosition((UIElement)sender);
	//		//VisualTreeHelper.HitTest(this,null,new HitTestResultCallback(myCallback),new PointHitTestParameters(pt));
	//	}
	//}
*/