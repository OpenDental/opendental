#region using
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion using

namespace OpenDental.UI{
	public partial class ZoomSlider : Control{
		#region Fields
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary></summary>
		private LinearGradientBrush _brushMainGradient;
		///<summary></summary>
		private LinearGradientBrush _brushPushedGradient;
		///<summary>This is the zoom level that's calculated in SetValueInitialFit and then constantly used to derive other values like _maximum.</summary>
		private int _fit;
		private Font _font=new Font("Segoe UI",9f);
		private bool _textHitEsc;
		///<summary>Mouse down was on slider. Either a drag or a click.</summary>
		private bool _isMouseDownOnSlider;
		private bool _isMouseDown;
		///<summary>This changes constantly and is not under external control.  It starts out as twice _fit, but then increases more each time user hits "plus".</summary>
		private int _maximum=100;
		///<summary></summary>
		private static Pen _penDivider=new Pen(Color.FromArgb(180,180,180));
		///<summary></summary>
		private static Pen _penSliderOutline=new Pen(Color.FromArgb(50,50,130));
		///<summary>Used for button outlines and control outline. Stays 1 pixel with scaling.</summary>
		private static Pen _penOutline=Pens.SlateGray;
		///<summary>Used for tickline. Varies in thickness with scaling.</summary>
		private static Pen _penTickLine=new Pen(Color.SlateGray);
		///<summary>Used plus and minus. Varies in thickness with scaling.</summary>
		private static Pen _penBlack=new Pen(Color.Black,1.5f);
		///<summary>Current position of mouse on control.  -1,-1 indicates not over control.</summary>
		private Point _pointMouse=new Point(-1,-1);
		///<summary>Point where mouse was initially down.  If dragging, it will get the original position.</summary>
		private Point _pointMouseDown;
		///<summary>The rectangle defining the Fit button</summary>
		private Rectangle _rectangleFit;
		///<summary>The rectangle defining the minus button</summary>
		private Rectangle _rectangleMinus;
		///<summary>The rectangle defining the plus button</summary>
		private Rectangle _rectanglePlus;
		///<summary>The rectangle defining the slider that contains the current zoom text.  Always falls within rectangleTickline.  Since user can only drag in whole pixel increments, we won't use a RectangleF.</summary>
		private Rectangle _rectangleSlider;
		///<summary>The rectangle defining tick line itself.  We never draw a rectangle around it, but it's useful for lots of math. It spans the height of the entire control. It extends left and right to the extent of the tick line.</summary>
		private Rectangle _rectangleTickLine;
		///<summary>The rectangle defining the tickline plus left and right padding. So the entire cell.</summary>
		private Rectangle _rectangleTickLineMargin;
		///<summary>The rectangle defining the 100 button</summary>
		private Rectangle _rectangle100;
		private TextBox textBoxEdit;
		private TimeSpan _timeMouseDown;
		///<summary>When mouse down to start dragging slider, we aren't necessarily in the center.  This keeps track of offset.</summary>
		private int _xOffsetMouseDownSlider;
		///<summary>Property backer.  Internally, this must be a float for better accuracy.  We round it for viewing and for the public property</summary>
		private float _value=1;
		#endregion Fields

		#region Constructor
		///<summary></summary>
		public ZoomSlider(){
			InitializeComponent();
			textBoxEdit=new TextBox();
			textBoxEdit.Font=new Font("Segoe UI",9f);//gets resized automatically by LayoutManager
			textBoxEdit.Visible=false;
			textBoxEdit.LostFocus += TextBoxEdit_LostFocus;
			textBoxEdit.KeyDown += TextBoxEdit_KeyDown;
			textBoxEdit.KeyUp += TextBoxEdit_KeyUp;
			textBoxEdit.PreviewKeyDown += TextBoxEdit_PreviewKeyDown;
			Controls.Add(textBoxEdit);
			DoubleBuffered=true;
		}
		#endregion Constructor

		#region Properties
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

		protected override Size DefaultSize {
			get {
				return new Size(200,25);
				//heavily optimized for about 25 to 26px high, or equivalently scaled
			}
		}
		#endregion Properties

		#region Events
		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when user presses Fit (or 100) button because parent form might need to recenter image.")]
		public event EventHandler FitPressed=null;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when user changes the zoom level.  It fires repeatedly while dragging.")]
		public event EventHandler Zoomed=null;
		#endregion Events

		#region Methods - Event Handlers
		private void TextBoxEdit_KeyDown(object sender, KeyEventArgs e){
			if(e.KeyCode==Keys.Enter){
				try{
					SetValueAndMax(int.Parse(textBoxEdit.Text));
				}
				catch{}
				textBoxEdit.Visible=false;
			}
			Zoomed?.Invoke(this, new EventArgs());
		}

		private void TextBoxEdit_KeyUp(object sender, KeyEventArgs e){
			int widthText=TextRenderer.MeasureText(textBoxEdit.Text,textBoxEdit.Font).Width;
			if(textBoxEdit.Width<widthText+6){
				textBoxEdit.Width=widthText+6;
				//textBoxEdit.ScrollToCaret();
			}
			Invalidate();
		}

		private void TextBoxEdit_LostFocus(object sender, EventArgs e){
			if(!_textHitEsc){
				try{
					SetValueAndMax(int.Parse(textBoxEdit.Text));
				}
				catch{
				}
			}
			textBoxEdit.Visible=false;
			Invalidate();
			Zoomed?.Invoke(this, new EventArgs());
		}

		private void TextBoxEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e){
			if(e.KeyCode==Keys.Escape){
				_textHitEsc=true;
				textBoxEdit.Visible=false;
			}
			Invalidate();
		}
		#endregion Methods - Event Handlers

		#region Methods - Override On...
		protected override void OnPaint(PaintEventArgs pe){
			base.OnPaint(pe);
			if(_maximum<5){
				return;
			}
			Graphics g=pe.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
			if(DesignMode){
				//because the first DrawLine keeps crashing otherwise
				g.DrawRectangle(_penOutline,0,0,Width-1,Height-1);
				StringFormat format=new StringFormat();
				format.Alignment=StringAlignment.Center;
				format.LineAlignment=StringAlignment.Center;
				g.DrawString(this.Name,Font,Brushes.Black,new Rectangle(0,0,Width,Height),format);
				return;
			}
			//Calculate a few rectangles
			using Font fontForMeasure=new Font(_font.FontFamily,LayoutManager.ScaleMS(_font.Size));
			int widthOne=(int)g.MeasureString("1",fontForMeasure).Width+2;//because "1" is the minimum value
			//_maximum won't change while mouse is down because drag can't take _value to larger than _maximum
			int widthMax=(int)g.MeasureString(_maximum.ToString(),fontForMeasure).Width+2;
			int widthValue=(int)g.MeasureString(Round(_value).ToString(),fontForMeasure).Width+2;
			int bufferLeft=widthOne/2;
			int bufferRight=widthMax/2;
			_rectangleTickLine=new Rectangle(_rectangleTickLineMargin.Left+bufferLeft,0,_rectangleTickLineMargin.Width-bufferLeft-bufferRight,Height);
			//xVal is the x position on the slider, in pixels, of _value.
			int xValuePixels=_rectangleTickLine.Left
				+(int)(((float)_rectangleTickLine.Right-_rectangleTickLine.Left)//how many pixels in tick line
				/(_maximum-1) *_value);
			_rectangleSlider=new Rectangle(xValuePixels-(widthValue/2),LayoutManager.Scale(9),widthValue,Height-LayoutManager.Scale(11));
			g.Clear(SystemColors.Control);
			g.FillRectangle(_brushMainGradient,0,0,Width,Height);//background does not change based on hover
			if(_isMouseDown){
				if(_rectangleMinus.Contains(_pointMouseDown)){
					g.FillRectangle(_brushPushedGradient,_rectangleMinus);
				}
				if(_rectanglePlus.Contains(_pointMouseDown)){
					g.FillRectangle(_brushPushedGradient,_rectanglePlus);
				}
				if(_rectangleFit.Contains(_pointMouseDown)){
					g.FillRectangle(_brushPushedGradient,_rectangleFit);
				}
				if(_rectangle100.Contains(_pointMouseDown)){
					g.FillRectangle(_brushPushedGradient,_rectangle100);
				}
			}
			g.DrawLine(_penDivider,0,0,0,Height);
			g.DrawLine(_penDivider,_rectangleMinus.Right,0,_rectangleMinus.Right,Height);
			g.DrawLine(_penDivider,_rectangleTickLineMargin.Right,0,_rectangleTickLineMargin.Right,Height);
			g.DrawLine(_penDivider,_rectanglePlus.Right,0,_rectanglePlus.Right,Height);
			g.DrawLine(_penDivider,_rectangleFit.Right,0,_rectangleFit.Right,Height);
			g.DrawLine(_penDivider,_rectangle100.Right,0,_rectangle100.Right,Height);
			Brush brushText=Brushes.Black;
			if(!Enabled){
				brushText=SystemBrushes.GrayText;
			}
			//these two rectangles are 17x25 unscaled
			//Minus
			GraphicsHelper.DrawLine(g,_penBlack,_rectangleMinus.Left+LayoutManager.Scale(4f),LayoutManager.Scale(12f),
				_rectangleMinus.Left+LayoutManager.Scale(13f),LayoutManager.Scale(12f));
			//Plus:
			GraphicsHelper.DrawLine(g,_penBlack,_rectanglePlus.Left+LayoutManager.ScaleF(4f),LayoutManager.ScaleF(12f),
				_rectanglePlus.Left+LayoutManager.ScaleF(14f),LayoutManager.ScaleF(12f));
			GraphicsHelper.DrawLine(g,_penBlack,_rectanglePlus.Left+LayoutManager.ScaleF(9f),LayoutManager.ScaleF(7f),
				_rectanglePlus.Left+LayoutManager.ScaleF(9f),LayoutManager.ScaleF(17f));
			g.DrawString("Fit",_font,brushText,_rectangleFit.Left+5,_rectangleFit.Top+4);
			g.DrawString("100",_font,brushText,_rectangle100.Left+3,_rectangle100.Top+4);
			//Tick line
			int yTickLine=LayoutManager.Scale(4);
			g.DrawLine(_penTickLine,_rectangleTickLine.X,yTickLine,_rectangleTickLine.Right,yTickLine);//horiz
			g.DrawLine(_penTickLine,_rectangleTickLine.X,0,_rectangleTickLine.X,yTickLine);//left vert. Const won't need to change once I measure it.
			int xFitPixels=(int)(((float)_rectangleTickLine.Right-_rectangleTickLine.Left)//how many pixels to the Fit tick line
				/(_maximum-1) *_fit);
			g.DrawLine(_penTickLine,_rectangleTickLine.Left+xFitPixels,0,_rectangleTickLine.Left+xFitPixels,yTickLine);//vertFit
			g.DrawLine(_penTickLine,_rectangleTickLine.Right,0,_rectangleTickLine.Right,yTickLine);//right vert
			//Slider
			g.FillRectangle(Brushes.White,_rectangleSlider);
			if(_isMouseDown){//outline any pressed button
				if(_isMouseDownOnSlider){
					g.DrawRectangle(_penSliderOutline,_rectangleSlider);
				}
				else{
					g.DrawRectangle(_penDivider,_rectangleSlider);
				}
			}
			else{//outline any hover button
				if(_rectangleSlider.Contains(_pointMouse)){
					g.DrawRectangle(_penSliderOutline,_rectangleSlider);
				}
				else{
					g.DrawRectangle(_penDivider,_rectangleSlider);
				}
			}
			string sVal=Round(_value).ToString();
			g.DrawString(sVal,_font,brushText,_rectangleSlider.X+LayoutManager.ScaleF(1),_rectangleSlider.Y-LayoutManager.ScaleF(1));
			PointF[] points=new PointF[3];
			points[0]=new PointF(xValuePixels,yTickLine);//top point
			points[1]=new PointF(xValuePixels-LayoutManager.ScaleF(2.5f),yTickLine+LayoutManager.ScaleF(5f));//left point
			points[2]=new PointF(xValuePixels+LayoutManager.ScaleF(2.5f),yTickLine+LayoutManager.ScaleF(5f));//right point
			g.FillPolygon(brushText,points);		
			if(_isMouseDown){//outline any pressed button
				if(_rectangleMinus.Contains(_pointMouseDown)){
					g.DrawRectangle(_penOutline,_rectangleMinus);
				}
				if(_rectanglePlus.Contains(_pointMouseDown)){
					g.DrawRectangle(_penOutline,_rectanglePlus);
				}
				if(_rectangleFit.Contains(_pointMouseDown)){
					g.DrawRectangle(_penOutline,_rectangleFit);
				}
				if(_rectangle100.Contains(_pointMouseDown)){
					g.DrawRectangle(_penOutline,_rectangle100);
				}
			}
			else{//outline any hover button
				if(_rectangleMinus.Contains(_pointMouse)){
					g.DrawRectangle(_penOutline,_rectangleMinus);
				}
				if(_rectanglePlus.Contains(_pointMouse)){
					g.DrawRectangle(_penOutline,_rectanglePlus);
				}
				if(_rectangleFit.Contains(_pointMouse)){
					g.DrawRectangle(_penOutline,_rectangleFit);
				}
				if(_rectangle100.Contains(_pointMouse)){
					g.DrawRectangle(_penOutline,_rectangle100);
				}
			}
			g.DrawLine(_penOutline,0,Height-1,Width,Height-1);
		}

		protected override void OnEnabledChanged(EventArgs e){

			base.OnEnabledChanged(e);
		}

		protected override void OnMouseLeave(EventArgs e){
			_pointMouse=new Point(-1,-1);
			Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e){
			//both mouse buttons are treated the same, so you could have two mouse downs and two mouse ups.
			if(_isMouseDown){
				return;//ignore the second one
			}
			if(textBoxEdit.Visible){
				TextBoxEdit_LostFocus(this,new EventArgs());
			}
			_isMouseDown=true;
			_timeMouseDown=DateTime.Now.TimeOfDay;
			_isMouseDownOnSlider=false;//unless set true down below
			_pointMouseDown=e.Location;
			if(_rectangleMinus.Contains(_pointMouseDown)){
				_value=_value*.5f;
				if(_value<1){
					_value=1;
				}
				Zoomed?.Invoke(this, new EventArgs());
			}
			if(_rectanglePlus.Contains(_pointMouseDown)){
				SetValueAndMax(_value+_maximum*.25f);
				Zoomed?.Invoke(this, new EventArgs());
			}
			if(_rectangleFit.Contains(_pointMouseDown)){
				SetValueAndMax(_fit);
				FitPressed?.Invoke(this, new EventArgs());
				Zoomed?.Invoke(this, new EventArgs());
			}
			if(_rectangle100.Contains(_pointMouseDown)){
				SetValueAndMax(100);
				FitPressed?.Invoke(this, new EventArgs());
				Zoomed?.Invoke(this, new EventArgs());
			}
			if(_rectangleTickLineMargin.Contains(_pointMouseDown)){
				if(_rectangleSlider.Contains(_pointMouseDown)){
					_isMouseDownOnSlider=true;
					_xOffsetMouseDownSlider=_rectangleSlider.X+_rectangleSlider.Width/2-e.X;
				}
				else if(_rectangleTickLine.Contains(_pointMouseDown)){
					//single click to change value
					int xPixels=e.X-_rectangleTickLine.Left;
					_value=(float)xPixels/_rectangleTickLine.Width*_maximum;
					Zoomed?.Invoke(this, new EventArgs());
				}
			}
			Invalidate();
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e){
			_pointMouse=e.Location;
			if(_isMouseDown){
				if(_isMouseDownOnSlider){
					//They are dragging
					//as they move, we change _value
					if(e.X+_xOffsetMouseDownSlider<_rectangleTickLine.Left){
						_value=1;
					}
					else if(e.X+_xOffsetMouseDownSlider>_rectangleTickLine.Right){
						_value=_maximum;
					}
					else{
						int xPixels=e.X+_xOffsetMouseDownSlider-_rectangleTickLine.Left;
						_value=(float)xPixels/_rectangleTickLine.Width*_maximum;
					}
					Zoomed?.Invoke(this, new EventArgs());
				}
			}
			Invalidate();
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e){
			if(!_isMouseDown){
				return;//ignore the second one
			}
			_isMouseDown=false;
			if(_isMouseDownOnSlider){
				//if they didn't hold the mouse down for very long and didn't drag very far, then this is a click to edit value
				if(DateTime.Now.TimeOfDay<_timeMouseDown.Add(TimeSpan.FromSeconds(1))//held button down for less than one second
					&& e.X>_pointMouseDown.X-3 && e.X<_pointMouseDown.X+3)//and moved less than 3px
				{
					textBoxEdit.Location=new Point(_rectangleSlider.X-1,_rectangleSlider.Y-2);
					textBoxEdit.Width=_rectangleSlider.Width+4;
					textBoxEdit.Visible=true;
					_textHitEsc=false;
					textBoxEdit.Text=Round(_value).ToString();
					textBoxEdit.Select();
				}
				//if they did drag, nothing to do
			}
			Invalidate();
			base.OnMouseUp(e);
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			CalculateScale();
		}
		#endregion Methods - Override On...

		#region Methods
		private void CalculateScale(){
			_brushMainGradient?.Dispose();
			_brushPushedGradient?.Dispose();
			_brushMainGradient=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(255,255,255),Color.FromArgb(171,181,209));
			_brushPushedGradient=new LinearGradientBrush(new Point(0,0),new Point(0,Height),Color.FromArgb(225,225,225),Color.FromArgb(141,151,179));
			_font?.Dispose();
			_font=new Font("Segoe UI",LayoutManager.ScaleFontODZoom(9f));
			_penDivider?.Dispose();
			_penSliderOutline?.Dispose();
			_penTickLine?.Dispose();
			_penBlack?.Dispose();
			_penDivider=new Pen(Color.FromArgb(180,180,180),LayoutManager.ScaleF(1));
			_penSliderOutline=new Pen(Color.FromArgb(50,50,130),LayoutManager.ScaleF(1));
			_penTickLine=new Pen(Color.SlateGray,LayoutManager.ScaleF(1));
			_penBlack=new Pen(Color.Black,LayoutManager.ScaleF(1.5f));
			//recalculate most rectangles
			_rectangleMinus=new Rectangle(0,0,LayoutManager.Scale(17),Height-2);
			_rectangle100=new Rectangle(Width-LayoutManager.Scale(30),0,LayoutManager.Scale(29),Height-2);
			_rectangleFit=new Rectangle(_rectangle100.Left-LayoutManager.Scale(27),0,LayoutManager.Scale(27),Height-2);
			_rectanglePlus=new Rectangle(_rectangleFit.Left-LayoutManager.Scale(17),0,LayoutManager.Scale(17),Height-2);
			_rectangleTickLineMargin=new Rectangle(_rectangleMinus.Right,0,_rectanglePlus.Left-_rectangleMinus.Right,Height-2);
			//rectangleTickLine and rectangleSlider are far too complex to calculate here.  We do those on each paint.			
		}

		private int Round(float num){
			return (int)Math.Round(num);
		}

		public void SetLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
			CalculateScale();//needed for initial load when main monitor is not 100%
		}

		///<summary>These parameters are required because initial center zoom is always to "fit". This is also the only way to set Enabled to true.  If degreesRotated is is 90 or 270, it will swap width and height when doing the calc.</summary>
		public void SetValueInitialFit(Size sizeCanvas,Size sizeImage,float degreesRotated){
			_fit=(int)(ImageTools.CalcScaleFit(sizeCanvas,sizeImage,degreesRotated)*100);
			_value=_fit;
			_maximum=2*_fit;//starting point
			if(Round(_value)>_maximum){
				_maximum=Round(_value);
			}
			Enabled=true;
			Invalidate();
		}

		///<summary>The number passed in here is a zoom change, like -15.  It was calculated externally based on existing _value for proportionality.</summary>
		public void SetByWheel(float deltaZoom){
			SetValueAndMax(_value+deltaZoom);   
			Invalidate();
		}

		///<summary>Sets value.  If value is greater than Max, then Max will grow.</summary>
		public void SetValueAndMax(float newval){
			//Converting to a float causes the int.maxvalue to still wrap around to a negative value, removed 100 from the maximum to avoid this error.
			if(newval>=int.MaxValue){
				newval=(float)int.MaxValue-100;
			}
			_value=newval;
			if(_value<1){
				_value=1;
			}
			if(Round(_value)>_maximum){
				_maximum=Round(_value);
			}
			Invalidate();
		}
		#endregion Methods



	

	}
}
