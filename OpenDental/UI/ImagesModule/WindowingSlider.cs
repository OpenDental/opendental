using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.UI {
	[DefaultEvent("Scroll")]
	public partial class WindowingSlider:Control {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private int _minVal=0;
		private int _maxVal=255;
		private float _widthBut=7;//the width of the end sliders.
		///<summary>The width, in pixels, of one increment within the allowed 0-255 range.</summary>
		private float _tick;
		private Timer _timerDelay;
		private EnumSliderButState _enumSliderButStateM = EnumSliderButState.Normal;
		private EnumSliderButState _enumSliderButStateL = EnumSliderButState.Normal;
		private EnumSliderButState _enumSliderButStateR = EnumSliderButState.Normal;
		private bool _isMouseDown;
		private int _xMouseDown;
		///<summary>The original pixel position of the button in question. The pointy part.</summary>
		private float _pixLeftStart;
		private float _pixRightStart;

		[Category("OD"),Description("Occurs when the slider moves.  UI is typically updated here.  Also see ScrollComplete")]
		public event EventHandler Scroll=null;
		[Category("OD"),Description("Occurs when user releases slider after moving.  Database is typically updated here.  Also see Scroll event.")]
		public event EventHandler ScrollComplete=null;

		public WindowingSlider() {
			InitializeComponent();
			this.DoubleBuffered=true;
			_timerDelay=new Timer();
			_timerDelay.Interval=300;//this is optimized for 1700x1300 BW.  Feels very nice. 
			//For a 1.3MB pano, it's a little short, because the locking every .3 sec makes it feel slightly sluggish.  But certainly tolerable.
			//An improvement would be to use the file size to tweak larger images to have a longer delay.
			_timerDelay.Tick += timerDelay_Tick;
		}

		///<summary>The value of the left slider.</summary>
		[Browsable(false)]
		public int MinVal{
			get{
				if(!Enabled){
					return 0;
				}
				return _minVal;
			}
			set{
				_minVal=value;
				Invalidate();
			}
		}

		///<summary>The value of the right slider, max 255.</summary>
		[Browsable(false)]
		public int MaxVal {
			get {
				if(!Enabled){
					return 255;
				}
				return _maxVal;
			}
			set {
				_maxVal=value;
				Invalidate();
			}
		}

		protected override Size DefaultSize {
			get {
				return new Size(154,20);
			}
		}

		protected override void OnPaint(PaintEventArgs pe) {
			Graphics g=pe.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			SolidBrush brush=new SolidBrush(SystemColors.Control);//gray 240=#F0F0F0
			g.FillRectangle(brush,0,0,Width,Height);
			_tick=(float)(Width-_widthBut-1)/255f;//gets set in mousemove also
			Rectangle rectangleMiddle=GetRectMiddle();
			if(_enumSliderButStateM==EnumSliderButState.Hover) {
				g.FillRectangle(Brushes.White,rectangleMiddle);
			}
			g.DrawRectangle(new Pen(Color.FromArgb(28,81,128)),rectangleMiddle);
			//Black left
			Rectangle rect=new Rectangle((int)(_widthBut/2),2,rectangleMiddle.Left-(int)(_widthBut/2),Height-5);
			g.FillRectangle(Brushes.Black,rect);
			//Middle gradient
			if(Enabled){
				rect=new Rectangle(rectangleMiddle.Left,2,rectangleMiddle.Width,Height-5);
				if(rectangleMiddle.Width>_widthBut){
					LinearGradientBrush gradientBrush=new LinearGradientBrush(
						new Point(rectangleMiddle.X+(int)(_widthBut/2),0),
						new Point(rectangleMiddle.Right-(int)(_widthBut/2)+1,0),
						Color.Black,Color.White);
						//new Point(0,0),new Point(rect.Right+1,0),
					g.FillRectangle(gradientBrush,rect);
				}
			}
			//else it just all stays gray 240
			//this one is just to eliminate a rounding artifact
			rect=new Rectangle(rectangleMiddle.Right-(int)(_widthBut/2)+1,2,2,Height-5);
			g.FillRectangle(Brushes.White,rect);

			rect=new Rectangle(rectangleMiddle.Right,2,Width-(int)(_widthBut/2)-rectangleMiddle.Right,Height-5);
			g.FillRectangle(Brushes.White,rect);
			DrawButton(g,GetPathLeft(),_enumSliderButStateL);
			DrawButton(g,GetPathRight(),_enumSliderButStateR);
			base.OnPaint(pe);
		}

		///<summary>Gets the outline path of the middle button that connects the two ends. But it's partly tucked under the end buttons.</summary>
		private Rectangle GetRectMiddle(){
			//GraphicsPath path=new GraphicsPath();
			return new Rectangle((int)(_widthBut/2f+MinVal*_tick),0,(int)((MaxVal-MinVal)*_tick),Height-1);
			//path.AddRectangle(rect);
			//return path;
		}

		///<summary>Gets the outline path of the left end button.</summary>
		private GraphicsPath GetPathLeft() {
			GraphicsPath path=new GraphicsPath();
			path.AddLines(new PointF[] {
				new PointF(MinVal*_tick,Height-4),//start at lower left, work clockwise
				new PointF(MinVal*_tick,3),
				new PointF(MinVal*_tick+_widthBut/2f,0),
				new PointF(MinVal*_tick+_widthBut,3),
				new PointF(MinVal*_tick+_widthBut,Height-4),
				new PointF(MinVal*_tick+_widthBut/2f,Height-1),
				new PointF(MinVal*_tick,Height-4)
			});
			return path;
		}
		/*
		Assumes height of 20
		-3.5,16 -3.5,3 0,0 3.5,3 3.5,16 0,19 
		*/

		///<summary>Gets the outline path of the right end button.</summary>
		private GraphicsPath GetPathRight() {
			GraphicsPath path=new GraphicsPath();
			path.AddLines(new PointF[] {
				new PointF(MaxVal*_tick,Height-4),//start at lower left, work clockwise
				new PointF(MaxVal*_tick,3),
				new PointF(MaxVal*_tick+_widthBut/2f,0),
				new PointF(MaxVal*_tick+_widthBut,3),
				new PointF(MaxVal*_tick+_widthBut,Height-4),
				new PointF(MaxVal*_tick+_widthBut/2f,Height-1),
				new PointF(MaxVal*_tick,Height-4)
			});
			return path;
		}

		private void DrawButton(Graphics g,GraphicsPath pathmain,EnumSliderButState state){
			Color clrMain=Color.FromArgb(200,202,220);//#C8CADC
			if(state==EnumSliderButState.Hover){
				clrMain=Color.FromArgb(240,240,255);
			}
			else if(state==EnumSliderButState.Pressed) {
				clrMain=Color.FromArgb(150,150,160);
			}
			GraphicsPath pathsub=new GraphicsPath();
			RectangleF rect=pathmain.GetBounds();
			pathsub.AddEllipse(rect.Left-rect.Width/8f,rect.Top-rect.Height/2f,rect.Width,rect.Height*3f/2f);
			PathGradientBrush pathBrush=new PathGradientBrush(pathsub);
			pathBrush.CenterColor=Color.FromArgb(255,255,255,255);
			pathBrush.SurroundColors=new Color[] { Color.FromArgb(0,255,255,255) };
			g.FillPath(new SolidBrush(clrMain),pathmain);
			g.FillPath(pathBrush,pathmain);
			Color clrDarkOverlay=Color.FromArgb(50,125,125,125);
			LinearGradientBrush brush=new LinearGradientBrush(new PointF(rect.Left,rect.Bottom),
				new PointF(rect.Left,rect.Top+rect.Height/2),Color.FromArgb(0,0,0,0),
				Color.FromArgb(50,0,0,0));
			g.FillPath(brush,pathmain);
			Pen outline=new Pen(Color.FromArgb(28,81,128));//#1C5180
			g.DrawPath(outline,pathmain);
		}

		///<summary></summary>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			if(!Enabled){
				return;
			}
			base.OnMouseMove(e);
			_tick=(float)(Width-_widthBut-1)/255f;
			if(_isMouseDown) {
				int minAllowedL=0;
				int maxAllowedL;
				int minAllowedR;
				int maxAllowedR=255;
				float deltaPix=_xMouseDown-e.X;
				if(_enumSliderButStateL==EnumSliderButState.Pressed){
					MinVal=(int)((_pixLeftStart-deltaPix-_widthBut/2f)/_tick);
					maxAllowedL=MaxVal-(int)(_widthBut/_tick);
					if(MinVal<minAllowedL){
						MinVal=minAllowedL;
					}
					else if(MinVal>maxAllowedL){
						MinVal=maxAllowedL;
					}
					OnScroll();
				}
				else if(_enumSliderButStateR==EnumSliderButState.Pressed){
					MaxVal=(int)((_pixRightStart-deltaPix-_widthBut/2f)/_tick);
					minAllowedR=MinVal+(int)(_widthBut/_tick);
					if(MaxVal<minAllowedR){
						MaxVal=minAllowedR;
					}
					else if(MaxVal>maxAllowedR){
						MaxVal=maxAllowedR;
					}
					OnScroll();
				}
				else if(_enumSliderButStateM==EnumSliderButState.Pressed) {
					MinVal=(int)((_pixLeftStart-deltaPix-_widthBut/2f)/_tick);
					MaxVal=(int)((_pixRightStart-deltaPix-_widthBut/2f)/_tick);
					int originalValSpan=(int)((_pixRightStart-_pixLeftStart)/_tick);
					maxAllowedL=maxAllowedR-originalValSpan;
					minAllowedR=minAllowedL+originalValSpan;
					if(MinVal<minAllowedL) {
						MinVal=minAllowedL;
					}
					else if(MinVal>maxAllowedL) {
						MinVal=maxAllowedL;
					}
					if(MaxVal<minAllowedR) {
						MaxVal=minAllowedR;
					}
					else if(MaxVal>maxAllowedR) {
						MaxVal=maxAllowedR;
					}
					OnScroll();
				}
			}
			else {//mouse is not down
				_enumSliderButStateL=EnumSliderButState.Normal;
				_enumSliderButStateR=EnumSliderButState.Normal;
				_enumSliderButStateM=EnumSliderButState.Normal;
				if(GetPathLeft().IsVisible(e.Location)){
					_enumSliderButStateL=EnumSliderButState.Hover;
				}
				else if(GetPathRight().IsVisible(e.Location)) {
					_enumSliderButStateR=EnumSliderButState.Hover;
				}
				else if(GetRectMiddle().Contains(e.Location)){
					_enumSliderButStateM=EnumSliderButState.Hover;
				}
			}
			Invalidate();
		}

		///<summary>Resets button appearance.  Repaints only if necessary.</summary>
		protected override void OnMouseLeave(System.EventArgs e) {
			if(!Enabled) {
				return;
			}
			base.OnMouseLeave(e);
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
				Invalidate();
			}
		}

		///<summary>Change the button to a pressed state.</summary>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if(!Enabled) {
				return;
			}
			base.OnMouseDown(e);
			if((e.Button & MouseButtons.Left)!=MouseButtons.Left) {
				return;
			}
			_isMouseDown=true;
			_xMouseDown=e.X;
			_enumSliderButStateL=EnumSliderButState.Normal;
			_enumSliderButStateR=EnumSliderButState.Normal;
			_enumSliderButStateM=EnumSliderButState.Normal;
			if(GetPathLeft().IsVisible(e.Location)){//if mouse pressed within the left button
				_enumSliderButStateL=EnumSliderButState.Pressed;
				_pixLeftStart=(float)MinVal*_tick+_widthBut/2f;
			}
			else if(GetPathRight().IsVisible(e.Location)) {
				_enumSliderButStateR=EnumSliderButState.Pressed;
				_pixRightStart=(float)MaxVal*_tick+_widthBut/2f;
			}
			else if(GetRectMiddle().Contains(e.Location)) {
				_enumSliderButStateM=EnumSliderButState.Pressed;
				_pixLeftStart=(float)MinVal*_tick+_widthBut/2f;
				_pixRightStart=(float)MaxVal*_tick+_widthBut/2f;
			}
			Invalidate();
		}

		///<summary>Change button to hover state and repaint if needed.</summary>
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(!Enabled) {
				return;
			}
			base.OnMouseUp(e);
			if((e.Button & MouseButtons.Left)!=MouseButtons.Left) {
				return;
			}
			_isMouseDown=false;
			if(_enumSliderButStateL==EnumSliderButState.Pressed
				|| _enumSliderButStateR==EnumSliderButState.Pressed
				|| _enumSliderButStateM==EnumSliderButState.Pressed)
			{
				OnScrollComplete();
			}
			_enumSliderButStateL=EnumSliderButState.Normal;
			_enumSliderButStateR=EnumSliderButState.Normal;
			_enumSliderButStateM=EnumSliderButState.Normal;
			if(GetPathLeft().IsVisible(e.Location)) {
				_enumSliderButStateL=EnumSliderButState.Hover;
			}
			else if(GetPathRight().IsVisible(e.Location)) {
				_enumSliderButStateR=EnumSliderButState.Hover;
			}
			else if(GetRectMiddle().Contains(e.Location)) {
				_enumSliderButStateM=EnumSliderButState.Hover;
			}
			Invalidate();
		}

		protected override void OnResize(EventArgs e){
			base.OnResize(e);
			_widthBut=LayoutManager.Scale(7);
		}

		///<summary></summary>
		protected void OnScroll() {
			if(!Enabled) {
				return;
			}
			//This is designed to fire with a slight delay, and then to not fire more often than that delay.
			if(_timerDelay.Enabled){
				return;//already set to fire shortly
			}
			_timerDelay.Enabled=true;
			//the actual event fires in the timer
		}

		///<summary></summary>
		protected void OnScrollComplete() {
			if(!Enabled) {
				return;
			}
			_timerDelay.Enabled=false;//cancel any pending Scroll event
			EventArgs ea=new EventArgs();
			if(ScrollComplete!=null) {
				ScrollComplete(this,ea);
			}
		}

		private void timerDelay_Tick(object sender, EventArgs e){
			EventArgs ea=new EventArgs();
			if(Scroll!=null){
				Scroll(this,ea);
			}
			_timerDelay.Enabled=false;
		}

		///<summary></summary>
		private enum EnumSliderButState {
			///<summary></summary>
			Normal,
			///<summary></summary>
			Hover,
			///<summary>Mouse down. Not a permanent toggle state.</summary>
			Pressed
		}

	}
}
