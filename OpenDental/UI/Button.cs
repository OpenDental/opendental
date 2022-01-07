using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OpenDental.UI {
	//Jordan is the only one allowed to edit this file.

	///<summary>Custom button used extensively throughout OD. Typically shows an image and text.  Image is usually 22x22.  Height is usually 24.</summary>
	public partial class Button : System.Windows.Forms.Button {
		#region Fields - Public
		///<summary>Just holds the scaling factor.</summary>
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private Static Colors
		private static Color _colorBorder=Color.FromArgb(28,81,128);
		///<summary>This brighter border color is used for hover and pressed.</summary>
		private static Color _colorBorderHover=Color.FromArgb(0,110,190);
		///<summary>The top gradient color. Also used for the Pressed color.</summary>
		private static Color _colorMainTop=Color.FromArgb(255,255,255);
		///<summary>The bottom gradient color. Also used for the Pressed color.</summary>
		private static Color _colorMainBottom=Color.FromArgb(205,212,215);
		///<summary>The top gradient color when pressed.</summary>
		private static Color _colorPressedTop=Color.FromArgb(255,255,255);
		///<summary>The bottom gradient color when pressed.</summary>
		private static Color _colorPressedBottom=Color.FromArgb(175,185,190);
		private static Color _colorText=Color.FromArgb(0,0,0);
		///<summary>The white glow that shows .5 pixels to the lower right behind the text.</summary>
		private static Color _colorTextGlow=Color.FromArgb(255,255,255);
		///<summary>The gray text when this button is disabled.</summary>
		private static Color _colorTextDisabled=Color.FromArgb(161,161,146);
		//Since these are static, one shared copy of each will persist for the life of the program
		private static Pen _penBorder=new Pen(_colorBorder);
		private static Pen _penBorderHover=new Pen(_colorBorderHover,1.7f);//remember, known bug in GDI+ does not scale lines <= 1.5, which affected certain decisions.
		private static LinearGradientBrush _brushMain=new LinearGradientBrush(new Point(0,0),new Point(0,24),_colorMainTop,_colorMainBottom);
		private static LinearGradientBrush _brushPressed=new LinearGradientBrush(new Point(0,0),new Point(0,24),_colorPressedTop,_colorPressedBottom);
		private static Brush _brushText=new SolidBrush(_colorText);
		private static Brush _brushTextGlow=new SolidBrush(_colorTextGlow);
		private static Brush _brushTextDisabled=new SolidBrush(_colorTextDisabled);
		#endregion Fields - Private Static Colors	
		
		#region Fields - Private
		private ODButtonState _buttonState=ODButtonState.Normal;
		private bool _bCanClick=false;
		private Point _adjustImageLocation=new Point(0,0);
		///<summary>This brush is only used when button is taller than 24.  Otherwise, it uses the static brush.  Disposed.</summary>
		private LinearGradientBrush _brushMainNonStatic;
		private LinearGradientBrush _brushPressedNonStatic;
		private EnumIcons _icon=EnumIcons.None;
		///<summary>When changing the text, this keeps track of the previous value. This lets us avoid overhead for initializing.</summary>
		private string TextOld;
		#endregion Fields - Private	

		#region Constructor
		///<summary></summary>
		public Button() {
			//This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			DoubleBuffered=true;
			if(Height>24){
				//gradient brush will fail, so adapt it to the size of this bigger button
				_brushMainNonStatic=new LinearGradientBrush(new Point(0,0),new Point(0,Height),_colorMainTop,_colorMainBottom);
				_brushPressedNonStatic=new LinearGradientBrush(new Point(0,0),new Point(0,Height),_colorPressedTop,_colorPressedBottom);
			}
		}
		#endregion Constructor

		#region Enums
		///<summary></summary>
		private enum ODButtonState {
			///<summary></summary>
			Normal,
			///<summary></summary>
			Hover,
			///<summary>Mouse down. Not a permanent toggle state.</summary>
			Pressed
		}
		#endregion Enums

		#region Properties
		///<summary></summary>
		[Category("OD"),Description("Allows slight adjustments of image location, usually just one or two pixels.")]
		[DefaultValue(typeof(Point),"0,0")]
		public Point AdjustImageLocation {
			get {
				return _adjustImageLocation;
			}
			set { 
				_adjustImageLocation=value;
				this.Invalidate();
			}
		}

		///<summary></summary>
		[Category("OD"),Description("This is replacing Image.")]
		[DefaultValue(0)]
		public EnumIcons Icon {
			get {
				return _icon;
			}
			set { 
				_icon=value;
				this.Invalidate();
			}
		}

		protected override Size DefaultSize {
			get {
				return new Size(75,24);
			}
		}

		#endregion Properties

		#region Methods - Private
		private static StringFormat GetStringFormat(ContentAlignment contentAlignment) {
			if(!Enum.IsDefined(typeof(ContentAlignment),(int)contentAlignment)) {
				throw new InvalidEnumArgumentException("contentAlignment",(int)contentAlignment,typeof(ContentAlignment));
			}
			StringFormat stringFormat=new StringFormat();
			switch(contentAlignment) {
				case ContentAlignment.MiddleCenter:
					stringFormat.LineAlignment=StringAlignment.Center;
					stringFormat.Alignment=StringAlignment.Center;
					break;
				case ContentAlignment.MiddleLeft:
					stringFormat.LineAlignment=StringAlignment.Center;
					stringFormat.Alignment=StringAlignment.Near;
					break;
				case ContentAlignment.MiddleRight:
					stringFormat.LineAlignment=StringAlignment.Center;
					stringFormat.Alignment=StringAlignment.Far;
					break;
				case ContentAlignment.TopCenter:
					stringFormat.LineAlignment=StringAlignment.Near;
					stringFormat.Alignment=StringAlignment.Center;
					break;
				case ContentAlignment.TopLeft:
					stringFormat.LineAlignment=StringAlignment.Near;
					stringFormat.Alignment=StringAlignment.Near;
					break;
				case ContentAlignment.TopRight:
					stringFormat.LineAlignment=StringAlignment.Near;
					stringFormat.Alignment=StringAlignment.Far;
					break;
				case ContentAlignment.BottomCenter:
					stringFormat.LineAlignment=StringAlignment.Far;
					stringFormat.Alignment=StringAlignment.Center;
					break;
				case ContentAlignment.BottomLeft:
					stringFormat.LineAlignment=StringAlignment.Far;
					stringFormat.Alignment=StringAlignment.Near;
					break;
				case ContentAlignment.BottomRight:
					stringFormat.LineAlignment=StringAlignment.Far;
					stringFormat.Alignment=StringAlignment.Far;
					break;
			}
			return stringFormat;
		}
		#endregion Methods - Private

		#region Methods - Override On...
		///<summary></summary>
		protected override void OnClick(EventArgs ea) {
			this.Capture=false;
			_bCanClick=false;
			if(this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition))) {
				_buttonState=ODButtonState.Hover;
			}
			else {
				_buttonState=ODButtonState.Normal;
			}
			this.Invalidate();
			base.OnClick(ea);
		}

		///<summary></summary>
		protected override void OnMouseEnter(EventArgs ea) {
			base.OnMouseEnter(ea);
			_buttonState=ODButtonState.Hover;
			this.Invalidate();
		}

		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs mea) {
			base.OnMouseDown(mea);
			if(mea.Button==MouseButtons.Left) {
				_bCanClick=true;
				_buttonState=ODButtonState.Pressed;
				this.Invalidate();
			}
		}

		///<summary></summary>
		protected override void OnMouseMove(MouseEventArgs mea) {
			base.OnMouseMove(mea);
			if(ClientRectangle.Contains(mea.X, mea.Y)) {
				if(_buttonState==ODButtonState.Hover && this.Capture && !_bCanClick) {
					_bCanClick=true;
					_buttonState=ODButtonState.Pressed;
					this.Invalidate();
				}
			}
			else {
				if(_buttonState==ODButtonState.Pressed) {
					_bCanClick=false;
					_buttonState=ODButtonState.Hover;
					this.Invalidate();
				}
			}
		}

		///<summary></summary>
		protected override void OnMouseLeave(EventArgs ea) {
			base.OnMouseLeave(ea);
			_buttonState=ODButtonState.Normal;
			this.Invalidate();
		}

		///<summary></summary>
		protected override void OnEnabledChanged(EventArgs ea) {
			base.OnEnabledChanged(ea);
			_buttonState=ODButtonState.Normal;
			this.Invalidate();
		}

		protected override void OnSizeChanged(EventArgs e){
			base.OnSizeChanged(e);
			if(Height>24){
				//gradient brush will fail, so adapt it to the size of this bigger button
				_brushMainNonStatic?.Dispose();
				_brushPressedNonStatic?.Dispose();
				_brushMainNonStatic=new LinearGradientBrush(new Point(0,0),new Point(0,Height),_colorMainTop,_colorMainBottom);
				_brushPressedNonStatic=new LinearGradientBrush(new Point(0,0),new Point(0,Height),_colorPressedTop,_colorPressedBottom);
			}
		}

		///<summary></summary>
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			//The only point of this override is to automatically widen a button if the foreign language translation would take more space than available.
			if(TextOld==null || TextOld==""){//There's no way to test the internal "IsLayoutSuspended", which is what I would rather have.
				//probably initially setting text value, so just skip this.
				TextOld=this.Text;
				return;
			}
			TextOld=this.Text;
			if(Text=="" || IsDisposed) {
				return;
			}
			try {
				int buffer=6;
				int textWidth=0;
				using(Graphics g = this.CreateGraphics()) {
					textWidth=(int)g.MeasureString(Text,Font).Width;
				}
				int oldWidth=Width;
				if(this.Image==null) {
					if(Width<textWidth+buffer) {
						LayoutManager.MoveWidth(this,textWidth+buffer);
					}
				}
				else {
					if(Width<textWidth+Image.Size.Width+buffer) {
						LayoutManager.MoveWidth(this,textWidth+Image.Size.Width+buffer);
					}
				}
//todo: this is broken for LayoutManager, but only noticed for foreign language and is rare
				if((Anchor&AnchorStyles.Right)==AnchorStyles.Right) {
					//to be perfect, it would exclude if anchored left also
					//this works even if no change in width
					Left+=oldWidth-Width;
				}
			}
			catch{
				//This method may fail if window handle has not been created yet. 
				//Can happen if invoked from a separate thread due to a race condition.
				//Nothing we can do about it and not worth crashing the program so ignore it.
			}
			Invalidate();
		}
		#endregion Methods - Override On...

		#region Method - OnPaint
		///<summary></summary>
		protected override void OnPaint(PaintEventArgs p) {
			this.OnPaintBackground(p);//draws background color at corners.  There are better approaches.
			Graphics g=p.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			RectangleF recOutline=new RectangleF(0,0,Width-1.2f,Height-1.2f);//lower edge looks better than at -1px
			float radius=3;
			using GraphicsPath graphicsPath=OpenDentBusiness.GraphicsHelper.GetRoundedPath(recOutline,radius);
			//_penBorderHover.Alignment=PenAlignment.Inset;
			switch(_buttonState) {
				case ODButtonState.Normal:
					//Focused and IsDefault are both old fashioned behaviors that we just don't care about, so no handing for them here
					//Disabled is handled when drawing the text and image
					if(Height>24){
						g.FillPath(_brushMainNonStatic,graphicsPath);
					}
					else{
						g.FillPath(_brushMain,graphicsPath);
					}
					break;
				case ODButtonState.Hover:
					if(Height>24){
						g.FillPath(_brushMainNonStatic,graphicsPath);
					}
					else{
						g.FillPath(_brushMain,graphicsPath);
					}
					break;
				case ODButtonState.Pressed:
					if(Height>24){
						g.FillPath(_brushPressedNonStatic,graphicsPath);
					}
					else{
						g.FillPath(_brushPressed,graphicsPath);
					}
					break;
			}
			DrawTextAndImage(g);
			switch(_buttonState) {
				case ODButtonState.Normal:
					g.DrawPath(_penBorder,graphicsPath);
					break;
				case ODButtonState.Hover:
					g.DrawPath(_penBorderHover,graphicsPath);
					g.DrawPath(_penBorder,graphicsPath);
					break;
				case ODButtonState.Pressed:
					g.DrawPath(_penBorderHover,graphicsPath);
					g.DrawPath(_penBorder,graphicsPath);
					break;
			}
		}

		///<summary></summary>
		private void DrawTextAndImage(Graphics g) {
			StringFormat stringFormat=GetStringFormat(this.TextAlign);
			if(ShowKeyboardCues) {
				stringFormat.HotkeyPrefix=System.Drawing.Text.HotkeyPrefix.Show;
			}
			else {
				stringFormat.HotkeyPrefix=System.Drawing.Text.HotkeyPrefix.Hide;
			}
			RectangleF rectangleTextGlow;
			if(Image==null && Icon==EnumIcons.None){
				rectangleTextGlow=new RectangleF(ClientRectangle.X+.5f,ClientRectangle.Y+.5f,ClientRectangle.Width,ClientRectangle.Height);
				if(this.Enabled) {
					g.DrawString(this.Text,this.Font,_brushTextGlow,rectangleTextGlow,stringFormat);
					g.DrawString(this.Text,this.Font,_brushText,this.ClientRectangle,stringFormat);
				}
				else{
					g.DrawString(this.Text,this.Font,_brushTextDisabled,this.ClientRectangle,stringFormat);
				}
				stringFormat?.Dispose();
				return;
			}
			//Image:
			Rectangle rectangleText=new Rectangle();
			Rectangle rectangleImage;
			Point pointImage= new Point(6, 4);
			Size sizeScaled;
			if(Icon!=EnumIcons.None){
				sizeScaled=new Size(LayoutManager.Scale(22),LayoutManager.Scale(22));
			}
			else{
				sizeScaled=new Size(LayoutManager.Scale(Image.Width),LayoutManager.Scale(Image.Height));
			}
			switch(this.ImageAlign) {
				case ContentAlignment.MiddleLeft:
					pointImage.X=6;
					pointImage.Y=this.ClientRectangle.Height/2-sizeScaled.Height/2;
					rectangleText.Width=this.ClientRectangle.Width-sizeScaled.Width;
					rectangleText.Height=this.ClientRectangle.Height;
					rectangleText.X=sizeScaled.Width;
					rectangleText.Y=0;
					break;
				case ContentAlignment.MiddleRight:
					rectangleText.Width=this.ClientRectangle.Width-sizeScaled.Width-8;
					rectangleText.Height=this.ClientRectangle.Height;
					rectangleText.X=0;
					rectangleText.Y=0;
					pointImage.X=rectangleText.Width;
					rectangleText.Width+=this._adjustImageLocation.X;
					pointImage.Y=this.ClientRectangle.Height/2-sizeScaled.Height/2;
					break;
				case ContentAlignment.MiddleCenter:// no text in this alignment
					pointImage.X=(this.ClientRectangle.Width-sizeScaled.Width)/2;
					pointImage.Y=(this.ClientRectangle.Height-sizeScaled.Height)/2;
					rectangleText.Width=0;
					rectangleText.Height=0;
					rectangleText.X=this.ClientRectangle.Width;
					rectangleText.Y=this.ClientRectangle.Height;
					break;
			}
			pointImage.X+=_adjustImageLocation.X;
			pointImage.Y+=_adjustImageLocation.Y;
			//these two gradient colors are a little different than the other colors.
			//They are just a hack for Direct2d to match up that gradient with the DirectX one.
			//They were obtained by color matching from a screenshot.
			Color colorGradientTop=Color.FromArgb(252,252,253);
			Color colorGradientBottom=Color.FromArgb(209,216,219);
			if(_buttonState==ODButtonState.Pressed){//might be missing a few edge cases
				colorGradientTop=Color.FromArgb(250,251,251);
				colorGradientBottom=Color.FromArgb(182,191,196);
			}
			if(this.Enabled) {
				rectangleImage=new Rectangle(pointImage,sizeScaled);
				if(Icon!=EnumIcons.None){
					IconLibrary.Draw(g,Icon,rectangleImage,colorGradientTop,colorGradientBottom,DesignMode);
				}
				else{
					g.DrawImage((Bitmap)Image,rectangleImage);
				}
			}
			else {
				rectangleImage=new Rectangle(pointImage,sizeScaled);
				if(Icon!=EnumIcons.None){
					IconLibrary.DrawDisabled(g,Icon,rectangleImage);
				}
				else{
					Bitmap bitmapDisabled=new Bitmap(Image.Width,Image.Height);//disabled bitmap is old small size
					Graphics gfx=Graphics.FromImage(bitmapDisabled);
					//System.Windows.Forms.ControlPaint.DrawImageDisabled(g,this.Image,pointImage.X,pointImage.Y,BackColor);
					ControlPaint.DrawImageDisabled(gfx,Image,0,0,SystemColors.Control);
					g.DrawImage(bitmapDisabled,rectangleImage);
					gfx.Dispose();
					bitmapDisabled.Dispose();
				}
			}
			rectangleTextGlow=new RectangleF(rectangleText.X+.5f,rectangleText.Y+.5f,rectangleText.Width,rectangleText.Height);
			if(this.ImageAlign!=ContentAlignment.MiddleCenter) {
				if(Enabled) {
					//first draw white text slightly off center
					g.DrawString(this.Text,this.Font,_brushTextGlow,rectangleTextGlow,stringFormat);
					//then, the black text
					g.DrawString(this.Text,this.Font,_brushText,rectangleText,stringFormat);
				}
				else{//disabled
					g.DrawString(this.Text,this.Font,_brushTextDisabled,rectangleText,stringFormat);
				}
			}
			stringFormat?.Dispose();
		}

		///<summary>Used by ODButtonPanel.  Let Button.cs do this drawing because this is where the colors are stored.  No image. Text in middle center.  No hover effect.</summary>
		public static void DrawSimpleButton(Graphics g,Rectangle rectangle,string text,Font font,Brush brush){
			RectangleF recOutline=new RectangleF(rectangle.X,rectangle.Y,rectangle.Width-1.2f,rectangle.Height-1.2f);
			float radius=3;
			GraphicsPath graphicsPath=OpenDentBusiness.GraphicsHelper.GetRoundedPath(recOutline,radius);
			g.FillPath(brush,graphicsPath);
			g.DrawPath(_penBorder,graphicsPath);
			graphicsPath.Dispose();
			StringFormat stringFormat=GetStringFormat(ContentAlignment.MiddleCenter);
			RectangleF rectangleTextGlow;
			rectangleTextGlow=new RectangleF(rectangle.X+.5f,rectangle.Y+.5f,rectangle.Width,rectangle.Height);
			g.DrawString(text,font,_brushTextGlow,rectangleTextGlow,stringFormat);
			g.DrawString(text,font,_brushText,rectangle,stringFormat);
			stringFormat.Dispose();
		}
		#endregion Method - OnPaint

	}

}

//[\t ]+this\.\w+\.AdjustImageLocation ?= ?new System\.Drawing\.Point\(0,0\);\r\n
//this.butOK.AdjustImageLocation = new System.Drawing.Point(0, 0);