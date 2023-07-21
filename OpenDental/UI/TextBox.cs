using CodeBase;
using Health.Direct.Common.Extensions;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using Tamir.SharpSsh.java;

namespace OpenDental.UI {
	//Jordan is the only one allowed to edit this file.
	//This TextBox is written from scratch, so we have more control. 
	//Some basic problems with the MS stock checkBox: highlight is stupid bold blue color, no hover effect, poor scaling for high dpi, font drawing layout is inconsistent with GDI+, Win11 removed outline from 3 sides, MS Vscrollbar has no setting for automatic, scrolling is jumpy instead of smooth, and Win11 scrollbar arrows hide unless hovered.

	///<summary>Incomplete!! Do not use. Designed to replace the stock MS checkBox throughout OD.</summary>
	public partial class TextBox : Control {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>There is no hScrollBar, but in single line, the user can scroll left and right with arrows or by dragging.</summary>
		private float _hScrollVal;
		private bool _isHovering;
		///<summary>When user clicks on text, this list of rectangles is created, on for each char, so that we can hit test where they clicked.  It's class level so that we can draw the rectangles during debugging. Scroll is already applied.</summary>
		private List<RectangleF> _listRectangleFsChars=new List<RectangleF>();
		///<summary>Text is drawn offset slightly from UL (1,2.5) at 96dpi, but this is scaled.</summary>
		private PointF _pointFOrigin;
		private System.Windows.Forms.VScrollBar vScrollBar;
		///<summary>This gets scaled when it's used.</summary>
		int widthScroll=17;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private bool _multiline=false;
		private HorizontalAlignment _textAlign=HorizontalAlignment.Left;
		#endregion Fields - Private for Properties

		#region Constructors
		public TextBox() {
			//vScrollBar.Visible=false;
			//vScrollBar.Enabled=false;
			InitializeComponent();
			vScrollBar=new VScrollBar();
			vScrollBar.Scroll+=VScroll_Scroll;
			vScrollBar.KeyDown+=VScroll_KeyDown;
			vScrollBar.Minimum=0;
			vScrollBar.Bounds=new Rectangle(Width-widthScroll-1,1,widthScroll,Height-2);
			vScrollBar.Anchor=AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;//for design mode
			this.Controls.Add(vScrollBar);
			DoubleBuffered=true;
			Size=new Size(120,20);//same as default
		}
		#endregion Constructors

		#region Events
		
		#endregion Events

		#region Properties - override
		protected override Size DefaultSize => new Size(120,20);
		#endregion Properties - override

		#region Properties
		///<summary></summary>
		[Category("OD")]
		[Description("Like the MS version, except that this also sets AcceptsReturn=true, AcceptsTab=false, WordWrap=true, ScrollBar=AutomaticVertical.")]
		[DefaultValue(false)]
		public bool Multiline{
			get{
				return _multiline;
			}
			set{
				if(value==_multiline) {
					return;
				}
				_multiline=value;
				Invalidate();
			}
		}

		//the attributes do not seem to be necessary.
		//[Category("Appearance")]
		//[Description("The text associated with the control.")]
		//[DefaultValue("")]
		public string Text{
			get{
				return base.Text;//base is just Control
			}
			set{
				base.Text=value;
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("Just like the MS version: left, middle, or right.")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign{
			get{
				return _textAlign;
			}
			set{
				_textAlign=value;
				Invalidate();
			}
		}
		#endregion Properties

		#region Methods - public
		
		#endregion Methods - public

		#region Methods - Event Handlers - OnPaint
		protected override void OnPaint(PaintEventArgs e){
			Graphics g=e.Graphics;
			//g.SmoothingMode=SmoothingMode.HighQuality;
			//LayoutScrollAndCalcRectangles(g);
			//g.TextRenderingHint//we're not going to do this. It will use system setting.  See notes down in LayoutScrollAndCalcRectangles.
			if(Enabled){
				g.Clear(BackColor);
			}
			else{
				g.Clear(ColorOD.Gray(240));
			}
			using Pen penOutline=new Pen(ColorOD.Gray(122)); //ColorOD.Outline//too dark compared to standard MS border
			g.DrawRectangle(penOutline,0,0,Width-1,Height-1);
			_pointFOrigin=new PointF(LayoutManager.ScaleF(1),LayoutManager.ScaleF(2.5f));
			//these numbers were chosen with trial and error. Pretty close to MS at 96dpi. Slightly too low at higher dpi, but acceptable.
			Color colorText =ForeColor;
			if(!Enabled){
				colorText=ColorOD.Gray(120);//160
			}
			using Brush brushText=new SolidBrush(colorText);
			//RectangleF rectangleFText=RectangleF.Empty;
			StringFormat stringFormat = new StringFormat();//StringFormat.GenericTypographic);
			//stringFormat.FormatFlags
			/*if(Multiline){			
				rectangleFText=new RectangleF(_pointFOrigin.X,_pointFOrigin.Y-vScrollBar.Value,Width-_pointFOrigin.X,100000);//huge arbitrary height
				if(vScrollBar.Visible){
					rectangleFText.Width=vScrollBar.Left-_pointFOrigin.X;
				}
				if(TextAlign==HorizontalAlignment.Center) {
					stringFormat.Alignment=StringAlignment.Center;
				}
				if(TextAlign==HorizontalAlignment.Right) {
					stringFormat.Alignment=StringAlignment.Far;
				}
				//Font is only zoomed. The MS scale gets applied by MS.
				g.DrawString(Text,Font,brushText,rectangleFText,stringFormat);
			}
			else {//single line*/
				//Single line text doesn't draw reliably with GDI+. It seems to switch to GDI or to some alternate tighter spacing.
				//I'm going to try drawing it at a point instead of inside a rectangle
				//float widthText=g.MeasureString(Text,Font,10000,stringFormat).Width;
				stringFormat.FormatFlags=StringFormatFlags.NoWrap;// | StringFormatFlags.NoClip;
				//stringFormat.
				//Leave alignment left/near for all cases
				float xPos=0;
				//if(TextAlign==HorizontalAlignment.Left
				//	|| widthText>Width) //MS completely ignores right or center alignment for these and treats them as left.
				//{
					xPos=_pointFOrigin.X-_hScrollVal;
					//rectangleFText=new RectangleF(_pointFOrigin.X-_hScrollVal,_pointFOrigin.Y,Width-_pointFOrigin.X,10000);
				/*}
				else if(TextAlign==HorizontalAlignment.Center) {//not sure how to handle this with h scrolling. Maybe just calc initial hScrollVal, and otherwise treat it like left?
					//stringFormat.Alignment=StringAlignment.Center;
					//rectangleFText=new RectangleF(_pointFOrigin.X,_pointFOrigin.Y,Width-_pointFOrigin.X,Height);
					xPos=_pointFOrigin.X-_hScrollVal;
				}
				else if(TextAlign==HorizontalAlignment.Right) {
					//stringFormat.Alignment=StringAlignment.Far;
					//rectangleFText=new RectangleF(Width-10000,_pointFOrigin.Y,10000,10000);
					xPos=Width-widthText-_hScrollVal;
				}*/
				Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(8.25f));
				//RectangleF rectangleFText=new RectangleF(_pointFOrigin.X-_hScrollVal,_pointFOrigin.Y,LayoutManager.ScaleF(200),LayoutManager.ScaleF(20));
				LayoutScrollAndCalcRectangles(g,stringFormat);
				g.DrawString(Text,font,brushText,xPos,_pointFOrigin.Y,stringFormat);
			//}
			//for testing
			//These are already adjusted for scroll
			if(_listRectangleFsChars.Count>0) {
				g.DrawRectangles(Pens.Red,_listRectangleFsChars.ToArray());
			}
		}
		#endregion Methods - Event Handlers - OnPaint

		#region Methods - Event Handlers - Mouse
		protected override void OnClick(EventArgs e){
			//base.OnClick(e);//no, we will fire the click from MouseDown so that we can control timing.
		}

		protected override void OnMouseDown(MouseEventArgs e){
			base.OnMouseDown(e);
		}

		protected override void OnMouseLeave(EventArgs e){
			base.OnMouseLeave(e);
			_isHovering=false;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			if(_isHovering){
				return;
			}
			_isHovering=true;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
		}
		#endregion Methods - Event Handlers - Mouse

		#region Methods - Event Handlers
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			Invalidate();
		}

		protected override void OnFontChanged(EventArgs e) {
			base.OnFontChanged(e);
			if(!Multiline){
				int heightFont=LayoutManager.Scale(Font.GetHeight());
				Height=heightFont+8;//to match MS behavior
			}
			Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			//e.Handled=true;
			base.OnKeyDown(e);
			if(e.KeyCode==Keys.Up) {//arrow key up
				
			}
			if(e.KeyCode==Keys.Down) {//arrow key down
				
			}
			char charKey=(char)e.KeyCode;
			if(e.KeyCode>=Keys.NumPad0 && e.KeyCode<=Keys.NumPad9) {
				charKey=e.KeyCode.ToString().Replace("NumPad","")[0];
			}
			//todo change text
			Invalidate();
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(!Multiline){
				int heightFont=LayoutManager.Scale(Font.GetHeight());
				Height=heightFont+8;//to match MS behavior
			}
			Invalidate();
		}
		
		private void VScroll_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down) {
				e.Handled=true;//I think we handle this is OnKeyDown instead
			}
		}

		private void VScroll_Scroll(object sender,ScrollEventArgs e) {
			Invalidate();
			//Debug.WriteLine(DateTime.Now.ToString()+" scroll val: "+vScrollBar.Value.ToString());
			//After dragging scrollbar, when mouse button is released, this fires again with a slightly different value.
			//So I just moved all the math into paint.
		}		
		#endregion Methods - Event Handlers

		#region Methods - private
		///<summary>This measures the text to determine fit, and lays out the scrollbar accordingly. It then calculates the text rectangles.</summary>
		private void LayoutScrollAndCalcRectangles(Graphics g,StringFormat stringFormat){
			_listRectangleFsChars=new List<RectangleF>();
			#region Scrollbar
			if(Text.Length==0 ){
				vScrollBar.Visible=false;
				vScrollBar.Enabled=false;
				vScrollBar.Value=0;
				return;
			}
			//Font is only zoomed, not MS scaled.
			//string format doesn't matter for this measurement
			/*
			float heightText=g.MeasureString(Text,Font,(int)(Width-_pointFOrigin.X)).Height;
			if(Multiline){
				if(heightText<Height-_pointFOrigin.Y){
					vScrollBar.Visible=false;
					vScrollBar.Enabled=false;
					vScrollBar.Value=0;
				}
				else{
					widthScroll=LayoutManager.Scale(17);
					LayoutManager.Move(vScrollBar,new Rectangle(Width-widthScroll-1,1,widthScroll,Height-2));
					vScrollBar.Visible=true;
					vScrollBar.Enabled=true;
					//recalc heightText using new width
					heightText=g.MeasureString(Text,Font,(int)(vScrollBar.Left-_pointFOrigin.X)).Height;
					//but we don't set its value other than to make sure it didn't go over max
					int scrollValOld=vScrollBar.Value;
					vScrollBar.Maximum=(int)heightText+(int)_pointFOrigin.Y;
					vScrollBar.SmallChange=Font.Height;
					vScrollBar.LargeChange=vScrollBar.Height;
					if(scrollValOld>vScrollBar.Maximum-vScrollBar.LargeChange){
						vScrollBar.Value=vScrollBar.Maximum-vScrollBar.LargeChange;
					}
				}
			}
			else{//single*/
				vScrollBar.Visible=false;
				vScrollBar.Enabled=false;
				vScrollBar.Value=0;
			//}
			#endregion Scrollbar
			//Graphics g=this.CreateGraphics();
			//g.TextRenderingHint=TextRenderingHint.AntiAlias;
			//==========================================================
			//GDI+ has some bad undocumented behavior in a very specific scenario:
			//This happens whether we pass in a Graphics from paint, or whether we CreateGraphics.
			//Condition 1: set TextRenderingHint=AntiAlias
			//Condition 2: g.MeasureCharacterRanges
			//When both of these conditions are met, the result is pixelation of other fonts on the form. They look terrible and skinny.
			//Possible fixes:
			//1. Set TextRenderingHint back to its original value before returning: This failed.
			//2. Use Graphics Save/Restore. This failed
			//3. Use a totally different Graphics object from a new bitmap: This failed.
			//4. I Googled for other people with same problem. Failed. There's not much out there about MeasureCharacterRanges.
			//5. I could instead make this a WPF control hosted in an ElementHost. Yikes.  Use https://jaylee.org/archive/2009/08/09/on-the-startup-performance-of-a-wpf-elementhost-in-winforms.html to improve performance.
			//6. I noticed that this did not happen in FormSheetFillEdit, so maybe it's a timing thing.
			//7. Maybe just don't set the TextRenderingHint. I don't like it, but it could work just fine. It will use system default, found in Performance Options, Smooth edges of screen fonts.
			//===========================================================
			//Another major issue that I had to deal with is that calling stringFormat.SetMeasurableCharacterRanges changes the way that subsequent drawing happens with that specific stringFormat.
			//If you use a different stringFormat for drawing than you used for measuring, then the string draws differently and won't match the measurements.
			//Put another way, once you measure with a stringFormat, MS seems to draw differently with that stringFormat and ensures that the drawing matches the measuring.
			//===========================================================


			RectangleF rectangleFText=RectangleF.Empty;
			//StringFormat stringFormat = new StringFormat();
			//if(Multiline){			
			//	rectangleFText=new RectangleF(_pointFOrigin.X,_pointFOrigin.Y-vScrollBar.Value,Width-_pointFOrigin.X,100000);//huge arbitrary height
			//	if(vScrollBar.Visible){
			//		rectangleFText.Width=vScrollBar.Left-_pointFOrigin.X;
			//	}
			//}
			//else{//single line
				//stringFormat.FormatFlags=StringFormatFlags.NoWrap;
				//float widthText=g.MeasureString(Text,Font,10000,stringFormat).Width;
				//if(TextAlign==HorizontalAlignment.Left
				//	|| widthText>Width)
				//{
				//	stringFormat.Alignment=StringAlignment.Near;
					rectangleFText=new RectangleF(_pointFOrigin.X-_hScrollVal,_pointFOrigin.Y,LayoutManager.ScaleF(200),LayoutManager.ScaleF(20));
				//}
				//else if(TextAlign==HorizontalAlignment.Center){//not sure how to handle this with h scrolling. Maybe just calc initial hScrollVal, and otherwise treat it like left?
				//	stringFormat.Alignment=StringAlignment.Center;
				//	rectangleFText=new RectangleF(_pointFOrigin.X,_pointFOrigin.Y,Width-_pointFOrigin.X,Height);
				//}
				//else if(TextAlign==HorizontalAlignment.Right){
				//	stringFormat.Alignment=StringAlignment.Far;
				//	rectangleFText=new RectangleF(Width-10000,_pointFOrigin.Y,10000,10000);
				//}
			//}
			//We're only allowed to measure 32 chars at a time
			int cStart=0;//the starting idx of the group of 32 or less chars that we're about to measure
			Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(8.25f));
			while(true){
				if(cStart>Text.Length-1){//example 5>4
					break;
				}
				int length=32;
				if(cStart+length>=Text.Length){
					length=Text.Length-cStart;
				}
				CharacterRange[] characterRangeArray=new CharacterRange[length];//each CharacterRange is just one char
				for(int i=0;i<characterRangeArray.Length;i++){
					characterRangeArray[i]=new CharacterRange(cStart+i,1);
				}
				stringFormat.SetMeasurableCharacterRanges(characterRangeArray);
				Region[] regionArray=g.MeasureCharacterRanges(Text,font,rectangleFText,stringFormat);
				for(int r=0;r<regionArray.Length;r++){
					RectangleF rectangleFBoundsChar=regionArray[r].GetBounds(g);
					_listRectangleFsChars.Add(rectangleFBoundsChar);
				}//end of r loop
				cStart+=length;
			}//end of while cStart loop
		}
		#endregion Methods - private
	}
}

//todo: read only



