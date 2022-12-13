using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This is a menu-style window.  No title bar.  Hovers in place until the user clicks on an action or until it loses focus.</summary>
	public partial class FormImageFloatWindows:Form {
		///<summary>Because this form does not inherit from FormODBase, we need something besides the LayoutManager for scaling.  It also uses Font.</summary>
		public float ScaleMy=1;
		///<summary>These are the lower two points of the button that launched this window, in screen coordinates.  This window will roughly center its top edge on these anchor points and will also omit the outline between these two points so that it looks more like a menu.</summary>
		public Point PointAnchor1;
		public Point PointAnchor2;
		private Color _colorBack=Color.FromArgb(224,224,224);
		private Color _colorOutline=Color.FromArgb(140,140,140);
		private Color _colorHover=Color.FromArgb(180,200,220);//Bluer than the bland color below
		//private Color _colorHover=Color.FromArgb(198,208,220);//numeric value of color below
		//private Color _colorHover=Color.FromArgb(224-(255-229),224-(255-239),224-(255-251));//combination of 2 colors below
			//Color.FromArgb(0,103,192);//Default for snap is way too intense.
			//ColorOD.Hover=Color.FromArgb(229,239,251);//a very pale blue designed to work when background is white
		private Color _colorHoverOutline=Color.FromArgb(33,96,150);
		///<summary>Owner</summary>
		private FormImageFloat _formImageFloat;
		//Each of the screen layouts gets a name, and each area gets a name. Each is disposed at class level.
		private GraphicsPath _graphicsPathHalf_L;
		private GraphicsPath _graphicsPathHalf_R;
		private GraphicsPath _graphicsPathQuarter_UL;
		private GraphicsPath _graphicsPathQuarter_UR;
		private GraphicsPath _graphicsPathQuarter_LL;
		private GraphicsPath _graphicsPathQuarter_LR;
		private GraphicsPath _graphicsPathMax;
		private GraphicsPath _graphicsPathCenter;
		private GraphicsPath _graphicsPathHalf_L2;
		private GraphicsPath _graphicsPathHalf_R2;
		private GraphicsPath _graphicsPathQuarter_UL2;
		private GraphicsPath _graphicsPathQuarter_UR2;
		private GraphicsPath _graphicsPathQuarter_LL2;
		private GraphicsPath _graphicsPathQuarter_LR2;
		private GraphicsPath _graphicsPathMax2;
		private GraphicsPath _graphicsPathCenter2;
		//we'll need to do the drawing with parameters instead of hard numbers
		///<summary>10. Margin between screens and around sides.</summary>
		private int _marginOuter;
		///<summary>5. Margin within each screen between snap locations.</summary>
		private int _marginInner;
		///<summary>80x48. The size of each "screen" area.</summary>
		private Size _sizeScreen;
		///<summary>Will be null if no other screen.</summary>
		private System.Windows.Forms.Screen _screen2;

		public FormImageFloatWindows() {
			InitializeComponent();
			Lan.F(this);
		}

		#region Events
		///<summary>User wants to see a different window.  Bubbles up to FormImageFloat. Handled by its parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<int> EventWindowClicked=null;

		///<summary>Bubbles up to FormImageFloat. Handled by its parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventWindowCloseOthers=null;

		///<summary>Bubbles up to FormImageFloat. Handled by its parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventWindowDockThis=null;

		///<summary>Bubbles up to FormImageFloat. Handled by its parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventWindowShowAll=null;
		#endregion Events

		private void FormImageFloatWindows_Deactivate(object sender, EventArgs e){
			//lose focus
			Close();
		}

		private void FormImageFloatWindows_Load(object sender, EventArgs e){
			_formImageFloat=(FormImageFloat)Owner;
			System.Windows.Forms.Screen[] screenArray=System.Windows.Forms.Screen.AllScreens;
			if(screenArray.Length==1){
				_screen2=null;
			}
			else{
				_screen2=screenArray[0];
				System.Windows.Forms.Screen screenThis=System.Windows.Forms.Screen.FromControl(this);
				if(_screen2.Bounds==screenThis.Bounds){
					_screen2=screenArray[1];
				}
			}
			//screen graphics----------------------------------------------------------------------------------
			_marginOuter=(int)(10f*ScaleMy);
			_marginInner=(int)(5f*ScaleMy);
			_sizeScreen=new Size((int)(80f*ScaleMy),(int)(48f*ScaleMy));
			int x,y,w,h;
			RectangleF rectangleF;
			y=(int)(4f*ScaleMy)+Font.Height+(int)(4f*ScaleMy);//space for text
			//Half is at UL-------------------------------------------------------------------------------------
			x=_marginOuter;
			w=(_sizeScreen.Width-_marginInner)/2;
			h=_sizeScreen.Height;
			rectangleF=new RectangleF(x,y,w,h);
			_graphicsPathHalf_L=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true,roundLL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y,w,_sizeScreen.Height);
			_graphicsPathHalf_R=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true,roundLR:true);
			//Max and Center at U middle----------------------------------------------------------------------
			x=x+_sizeScreen.Width+_marginOuter;
			w=_sizeScreen.Width;
			h=_sizeScreen.Height;
			rectangleF=new RectangleF(x,y,w,h);
			_graphicsPathMax=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true,roundLR:true,roundUL:true,roundLL:true);
			w=_sizeScreen.Width/5*3;
			h=(int)(29f*ScaleMy);
			rectangleF=new RectangleF(x+_sizeScreen.Width/5,y+10f*ScaleMy,w,h);
			_graphicsPathCenter=GraphicsHelper.GetRoundedPathPartial(rectangleF,5);
			//Quarter is at UR-----------------------------------------------------------------------------------
			x=x+_sizeScreen.Width+_marginOuter;
			w=(_sizeScreen.Width-_marginInner)/2;
			h=(_sizeScreen.Height-_marginInner)/2;
			rectangleF=new RectangleF(x,y,w,h);
			_graphicsPathQuarter_UL=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y,w,h);//UR
			_graphicsPathQuarter_UR=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true);
			rectangleF=new RectangleF(x,y+h+_marginInner,w,h);//LL
			_graphicsPathQuarter_LL=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundLL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y+h+_marginInner,w,h);//LR
			_graphicsPathQuarter_LR=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundLR:true);
			//Second Screen======================================================================================
			if(_screen2 !=null){
				y+=_sizeScreen.Height+(int)(4f*ScaleMy)+Font.Height+(int)(4f*ScaleMy);//space for text
				//Half is at UL-------------------------------------------------------------------------------------
				x=_marginOuter;
				w=(_sizeScreen.Width-_marginInner)/2;
				h=_sizeScreen.Height;
				rectangleF=new RectangleF(x,y,w,h);
				_graphicsPathHalf_L2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true,roundLL:true);
				rectangleF=new RectangleF(x+w+_marginInner,y,w,_sizeScreen.Height);
				_graphicsPathHalf_R2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true,roundLR:true);
				//Max and Center at U middle----------------------------------------------------------------------
				x=x+_sizeScreen.Width+_marginOuter;
				w=_sizeScreen.Width;
				h=_sizeScreen.Height;
				rectangleF=new RectangleF(x,y,w,h);
				_graphicsPathMax2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true,roundLR:true,roundUL:true,roundLL:true);
				w=_sizeScreen.Width/5*3;
				h=(int)(29f*ScaleMy);
				rectangleF=new RectangleF(x+_sizeScreen.Width/5,y+10f*ScaleMy,w,h);
				_graphicsPathCenter2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5);
				//Quarter is at UR-----------------------------------------------------------------------------------
				x=x+_sizeScreen.Width+_marginOuter;
				w=(_sizeScreen.Width-_marginInner)/2;
				h=(_sizeScreen.Height-_marginInner)/2;
				rectangleF=new RectangleF(x,y,w,h);
				_graphicsPathQuarter_UL2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true);
				rectangleF=new RectangleF(x+w+_marginInner,y,w,h);//UR
				_graphicsPathQuarter_UR2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true);
				rectangleF=new RectangleF(x,y+h+_marginInner,w,h);//LL
				_graphicsPathQuarter_LL2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundLL:true);
				rectangleF=new RectangleF(x+w+_marginInner,y+h+_marginInner,w,h);//LR
				_graphicsPathQuarter_LR2=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundLR:true);
			}
			//Other controls=====================================================================================
			x=_marginOuter;
			y+=_sizeScreen.Height+_marginOuter;
			labelActions.Location=new Point(x,y);
			labelActions.Size=ScaleSize(100,18);
			y+=ScaleInt(20);
			listBoxActions.Location=new Point(x,y);
			listBoxActions.Size=ScaleSize(120,43);
			y+=ScaleInt(46);
			labelWindows.Location=new Point(x,y);
			labelWindows.Size=ScaleSize(100,18);
			y+=ScaleInt(20);
			listBoxWindows.Location=new Point(x,y);
			listBoxWindows.Width=_marginOuter+_sizeScreen.Width*2;
			List<FormImageFloat> listFormImageFloats=_formImageFloat.GetListWindows();
			for(int i=0;i<listFormImageFloats.Count;i++){
				listBoxWindows.Items.Add(listFormImageFloats[i].Text);
				if(listFormImageFloats[i]==_formImageFloat){
					listBoxWindows.SetSelected(i);
				}
			}
			listBoxWindows.Height=Font.Height*listFormImageFloats.Count+4;//pulled from ListBoxOD.IntegralHeight.
			//Size and Location of form==========================================================================
			x=_marginOuter*4+_sizeScreen.Width*3;
			y=listBoxWindows.Bottom+_marginOuter;
			Size=new Size(x,y);
			x=(PointAnchor1.X+PointAnchor2.X)/2-Width/2;
			Rectangle rectangleWorking=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;
			if(x+Width>rectangleWorking.Right-10){
				x=rectangleWorking.Right-Width-10;
			}
			y=PointAnchor1.Y;
			Location=new Point(x,y);//changing location here does not cause flicker
		}

		///<summary>Same as LayoutManager.Scale, which we don't have access to.</summary>
		private int ScaleInt(float val96){
			return (int)Math.Round(val96*ScaleMy);
		}

		///<summary>Same as LayoutManager.ScaleSize, which we don't have access to.</summary>
		private Size ScaleSize(Size size96){
			return new Size((int)Math.Round(size96.Width*ScaleMy),(int)Math.Round(size96.Height*ScaleMy));
		}

		///<summary>Same as LayoutManager.ScaleSize, which we don't have access to.</summary>
		private Size ScaleSize(int width,int height){
			return new Size((int)Math.Round(width*ScaleMy),(int)Math.Round(height*ScaleMy));
		}
		
		private void FormImageFloatWindows_MouseDown(object sender, MouseEventArgs e){
			if(_formImageFloat.IsImageFloatLocked){
				Close();
				MsgBox.Show(this,"PDFs cannot be undocked.  Double click to open in PDF viewer.");
				return;
			}
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromHandle(this.Handle);
			Rectangle rectangleWorking=screen.WorkingArea;//In screen coords
			if(ClickSecondScreen(e)){
				return;
			}
			//Half----------------------------------------------------------------------------------
			if(_graphicsPathHalf_L.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top,
					width:rectangleWorking.Width/2,
					height:rectangleWorking.Height
					);
			}
			else if(_graphicsPathHalf_R.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/2,
					y:rectangleWorking.Top,
					width:rectangleWorking.Width/2,
					height:rectangleWorking.Height
					);
			}
			//Max and Center-------------------------------------------------------------------------
			//Max
			//else if(_graphicsPathMax.IsVisible(e.Location) && !_graphicsPathCenter.IsVisible(e.Location)){
			//	_formImageFloat.WindowState=FormWindowState.Maximized;
			//	_formImageFloat.SetZoomSlider();
			//	_formImageFloat.IsImageFloatDocked=false;
			//	Close();
			//	return;
			//}
			//Center
			else if(_graphicsPathCenter.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/5,
					y:rectangleWorking.Top+rectangleWorking.Height/8,
					width:rectangleWorking.Width/5*3,
					height:rectangleWorking.Height/8*6
					);
			}
			//Quarter--------------------------------------------------------------------------------
			//UL
			else if(_graphicsPathQuarter_UL.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top,
					width:rectangleWorking.Width/2,
					height:rectangleWorking.Height/2
					);
			}
			//UR
			else if(_graphicsPathQuarter_UR.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/2,
					y:rectangleWorking.Top,
					width:rectangleWorking.Width/2,
					height:rectangleWorking.Height/2
					);
			}
			//LL
			else if(_graphicsPathQuarter_LL.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top+rectangleWorking.Height/2,
					width:rectangleWorking.Width/2,
					height:rectangleWorking.Height/2
					);
			}
			//LR
			else if(_graphicsPathQuarter_LR.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/2,
					y:rectangleWorking.Top+rectangleWorking.Height/2,
					width:rectangleWorking.Width/2,
					height:rectangleWorking.Height/2
					);
			}
			else{
				return;
			}
			_formImageFloat.SetZoomSlider();
			_formImageFloat.IsImageFloatDocked=false;
			Close();
			if(_formImageFloat.WindowState==FormWindowState.Maximized){
				_formImageFloat.WindowState=FormWindowState.Normal;
			}
		}

		///<summary>Returns true if the click was handled.</summary>
		private bool ClickSecondScreen(MouseEventArgs e){
			//This is in a separate method because the first method is too long and because this math is a little different.
			if(_screen2 is null){
				return false;
			}
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromHandle(this.Handle);
			//Once the window shows up on the other screen, windows will resize it due different dpi.
			//Position won't change, but size will.
			//Example: new size 100x100, will change to 200x200 on the 4k monitor.
			//So we should instead set it to 50x50, so that it will change to our desired 100x100.
			int dpi1=Dpi.GetScreenDpi(screen);//example 96
			int dpi2=Dpi.GetScreenDpi(_screen2);//example 192
			float ratioDpiThisOther=(float)dpi1/(float)dpi2;//example .5
			Rectangle rectangleWorking=_screen2.WorkingArea;
			//Half----------------------------------------------------------------------------------
			if(_graphicsPathHalf_L2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height*ratioDpiThisOther)
					);
			}
			else if(_graphicsPathHalf_R2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/2,
					y:rectangleWorking.Top,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height*ratioDpiThisOther)
					);
			}
			//Max and Center-------------------------------------------------------------------------
			//Max
			//else if(_graphicsPathMax2.IsVisible(e.Location) && !_graphicsPathCenter2.IsVisible(e.Location)){
				//I struggled with this for hours and couldn't make it work.  Oh well.
				//The problem is that it maximizes, but has the wrong dpi.
				//move it to other screen------------------
				/*
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height/2*ratioDpiThisOther)
					);*/
				//_formImageFloat.Invalidate();
				/*Didn't help
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/6,
					y:rectangleWorking.Top+rectangleWorking.Height/10,
					width:rectangleWorking.Width/6*4,
					height:rectangleWorking.Height/10*8
					);*/
				//Application.DoEvents();//didn't help
				//_formImageFloat.PerformLayout();//didn't help
				//then maximize------------------
				//_formImageFloat.WindowState=FormWindowState.Maximized;
				//_formImageFloat.Invalidate();
				//Application.DoEvents();
				//_formImageFloat.Invalidate();
				//System.Threading.Thread.Sleep(1000);
				//_formImageFloat.Invalidate();
				//Application.DoEvents();
				//_formImageFloat.Invalidate();
				//_formImageFloat.WindowState=FormWindowState.Normal;
				//Application.DoEvents();
				//_formImageFloat.WindowState=FormWindowState.Maximized;
				//_formImageFloat.SetZoomSlider();
				//_formImageFloat.IsImageFloatDocked=false;
				//Close();
				//return true;
			//}
			//Center
			else if(_graphicsPathCenter2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/5,
					y:rectangleWorking.Top+rectangleWorking.Height/8,
					width:(int)(rectangleWorking.Width/5*3*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height/8*6*ratioDpiThisOther)
					);
			}
			//Quarter--------------------------------------------------------------------------------
			//UL
			else if(_graphicsPathQuarter_UL2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height/2*ratioDpiThisOther)
					);
			}
			//UR
			else if(_graphicsPathQuarter_UR2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/2,
					y:rectangleWorking.Top,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height/2*ratioDpiThisOther)
					);
			}
			//LL
			else if(_graphicsPathQuarter_LL2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left,
					y:rectangleWorking.Top+rectangleWorking.Height/2,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height/2*ratioDpiThisOther)
					);
			}
			//LR
			else if(_graphicsPathQuarter_LR2.IsVisible(e.Location)){
				_formImageFloat.Bounds=new Rectangle(
					x:rectangleWorking.Left+rectangleWorking.Width/2,
					y:rectangleWorking.Top+rectangleWorking.Height/2,
					width:(int)(rectangleWorking.Width/2*ratioDpiThisOther),
					height:(int)(rectangleWorking.Height/2*ratioDpiThisOther)
					);
			}
			else{
				return false;
			}
			_formImageFloat.SetZoomSlider();
			_formImageFloat.IsImageFloatDocked=false;
			Close();
			if(_formImageFloat.WindowState==FormWindowState.Maximized){
				_formImageFloat.WindowState=FormWindowState.Normal;
			}
			return true;
		}

		private void FormImageFloatWindows_MouseLeave(object sender, EventArgs e){
			Invalidate();
		}

		private void FormImageFloatWindows_MouseMove(object sender, MouseEventArgs e){
			//GraphicsPath gp;
			//gp.IsVisible(e.Location);
			Invalidate();
		}

		private void FormImageFloatWindows_Paint(object sender, PaintEventArgs e){
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(ColorOD.Background);
			using SolidBrush solidBrushBack=new SolidBrush(_colorBack);
			using SolidBrush solidBrushHover=new SolidBrush(_colorHover);
			using Pen penOutline=new Pen(_colorOutline);
			using Pen penHoverOutline=new Pen(_colorOutline);
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;//horiz
			stringFormat.LineAlignment=StringAlignment.Center;//vert
			Point pointMouse=PointToClient(Control.MousePosition);
			if(_screen2 !=null){
				g.DrawString("This screen:",Font,Brushes.Black,_marginOuter-2f*ScaleMy,4f*ScaleMy);
			}
			//Half----------------------------------------------------------------------------------
			if(_graphicsPathHalf_L.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathHalf_L);
				g.DrawPath(penHoverOutline,_graphicsPathHalf_L);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathHalf_L);
				g.DrawPath(penOutline,_graphicsPathHalf_L);
			}
			g.DrawString("Left",Font,Brushes.Black,_graphicsPathHalf_L.GetBounds(),stringFormat);
			if(_graphicsPathHalf_R.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathHalf_R);
				g.DrawPath(penHoverOutline,_graphicsPathHalf_R);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathHalf_R);
				g.DrawPath(penOutline,_graphicsPathHalf_R);
			}
			g.DrawString("Right",Font,Brushes.Black,_graphicsPathHalf_R.GetBounds(),stringFormat);
			//Max and Center-------------------------------------------------------------------------
			//if(_graphicsPathMax.IsVisible(pointMouse) && !_graphicsPathCenter.IsVisible(pointMouse)){
			//	g.FillPath(brushHover,_graphicsPathMax);
			//	g.DrawPath(penHoverOutline,_graphicsPathMax);
			//}
			//else{
				g.FillPath(solidBrushBack,_graphicsPathMax);
				g.DrawPath(penOutline,_graphicsPathMax);
			//}
			//RectangleF rectangleFBoundsLargeText=_graphicsPathMax.GetBounds();
			//rectangleFBoundsLargeText.Height=Font.Height;
			//g.DrawString("Max",Font,Brushes.Black,rectangleFBoundsLargeText,stringFormat);
			if(_graphicsPathCenter.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathCenter);
				g.DrawPath(penHoverOutline,_graphicsPathCenter);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathCenter);
				g.DrawPath(penOutline,_graphicsPathCenter);
			}
			g.DrawString("Center",Font,Brushes.Black,_graphicsPathCenter.GetBounds(),stringFormat);
			//Quarter--------------------------------------------------------------------------------
			//UL
			if(_graphicsPathQuarter_UL.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathQuarter_UL);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_UL);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathQuarter_UL);
				g.DrawPath(penOutline,_graphicsPathQuarter_UL);
			}
			g.DrawString("UL",Font,Brushes.Black,_graphicsPathQuarter_UL.GetBounds(),stringFormat);
			//UR
			if(_graphicsPathQuarter_UR.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathQuarter_UR);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_UR);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathQuarter_UR);
				g.DrawPath(penOutline,_graphicsPathQuarter_UR);
			}
			g.DrawString("UR",Font,Brushes.Black,_graphicsPathQuarter_UR.GetBounds(),stringFormat);
			//LL
			if(_graphicsPathQuarter_LL.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathQuarter_LL);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_LL);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathQuarter_LL);
				g.DrawPath(penOutline,_graphicsPathQuarter_LL);
			}
			g.DrawString("LL",Font,Brushes.Black,_graphicsPathQuarter_LL.GetBounds(),stringFormat);
			//LR
			if(_graphicsPathQuarter_LR.IsVisible(pointMouse)){
				g.FillPath(solidBrushHover,_graphicsPathQuarter_LR);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_LR);
			}
			else{
				g.FillPath(solidBrushBack,_graphicsPathQuarter_LR);
				g.DrawPath(penOutline,_graphicsPathQuarter_LR);
			}
			g.DrawString("LR",Font,Brushes.Black,_graphicsPathQuarter_LR.GetBounds(),stringFormat);
			//Other screen==========================================================================
			if(_screen2 !=null){
				g.DrawString("Other screen:",Font,Brushes.Black,_marginOuter-2f*ScaleMy,_graphicsPathQuarter_LR.GetBounds().Bottom+6f*ScaleMy);
				if(_graphicsPathHalf_L2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathHalf_L2);
					g.DrawPath(penHoverOutline,_graphicsPathHalf_L2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathHalf_L2);
					g.DrawPath(penOutline,_graphicsPathHalf_L2);
				}
				g.DrawString("Left",Font,Brushes.Black,_graphicsPathHalf_L2.GetBounds(),stringFormat);
				if(_graphicsPathHalf_R2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathHalf_R2);
					g.DrawPath(penHoverOutline,_graphicsPathHalf_R2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathHalf_R2);
					g.DrawPath(penOutline,_graphicsPathHalf_R2);
				}
				g.DrawString("Right",Font,Brushes.Black,_graphicsPathHalf_R2.GetBounds(),stringFormat);
				//Max and Center-------------------------------------------------------------------------
				//if(_graphicsPathMax2.IsVisible(pointMouse) && !_graphicsPathCenter2.IsVisible(pointMouse)){
				//	g.FillPath(brushHover,_graphicsPathMax2);
				//	g.DrawPath(penHoverOutline,_graphicsPathMax2);
				//}
				//else{
					g.FillPath(solidBrushBack,_graphicsPathMax2);
					g.DrawPath(penOutline,_graphicsPathMax2);
				//}
				//rectangleFBoundsLargeText=_graphicsPathMax2.GetBounds();
				//rectangleFBoundsLargeText.Height=Font.Height;
				//g.DrawString("Max",Font,Brushes.Black,rectangleFBoundsLargeText,stringFormat);
				if(_graphicsPathCenter2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathCenter2);
					g.DrawPath(penHoverOutline,_graphicsPathCenter2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathCenter2);
					g.DrawPath(penOutline,_graphicsPathCenter2);
				}
				g.DrawString("Center",Font,Brushes.Black,_graphicsPathCenter2.GetBounds(),stringFormat);
				//Quarter--------------------------------------------------------------------------------
				//UL
				if(_graphicsPathQuarter_UL2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathQuarter_UL2);
					g.DrawPath(penHoverOutline,_graphicsPathQuarter_UL2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathQuarter_UL2);
					g.DrawPath(penOutline,_graphicsPathQuarter_UL2);
				}
				g.DrawString("UL",Font,Brushes.Black,_graphicsPathQuarter_UL2.GetBounds(),stringFormat);
				//UR
				if(_graphicsPathQuarter_UR2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathQuarter_UR2);
					g.DrawPath(penHoverOutline,_graphicsPathQuarter_UR2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathQuarter_UR2);
					g.DrawPath(penOutline,_graphicsPathQuarter_UR2);
				}
				g.DrawString("UR",Font,Brushes.Black,_graphicsPathQuarter_UR2.GetBounds(),stringFormat);
				//LL
				if(_graphicsPathQuarter_LL2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathQuarter_LL2);
					g.DrawPath(penHoverOutline,_graphicsPathQuarter_LL2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathQuarter_LL2);
					g.DrawPath(penOutline,_graphicsPathQuarter_LL2);
				}
				g.DrawString("LL",Font,Brushes.Black,_graphicsPathQuarter_LL2.GetBounds(),stringFormat);
				//LR
				if(_graphicsPathQuarter_LR2.IsVisible(pointMouse)){
					g.FillPath(solidBrushHover,_graphicsPathQuarter_LR2);
					g.DrawPath(penHoverOutline,_graphicsPathQuarter_LR2);
				}
				else{
					g.FillPath(solidBrushBack,_graphicsPathQuarter_LR2);
					g.DrawPath(penOutline,_graphicsPathQuarter_LR2);
				}
				g.DrawString("LR",Font,Brushes.Black,_graphicsPathQuarter_LR2.GetBounds(),stringFormat);
			}
			//outline
			g.DrawLine(Pens.Gray,0,0,PointAnchor1.X-Location.X,0);//UL
			g.DrawLine(Pens.Gray,PointAnchor2.X-Location.X,0,Width-1,0);//UR
			g.DrawLine(Pens.Gray,Width-1,0,Width-1,Height-1);//R
			g.DrawLine(Pens.Gray,0,Height-1,Width-1,Height-1);//bottom
			g.DrawLine(Pens.Gray,0,0,0,Height-1);//L
			//g.DrawRectangle(Pens.Gray,0,0,Width-1,Height-1);
		}

		private void listBoxActions_SelectionChangeCommitted(object sender, EventArgs e){
			switch(listBoxActions.SelectedIndex){
				case 0://Dock This
					//Warning. The moment a MsgBox is shown, this form will close, so the MessageBox must specify different owner.
					EventWindowDockThis?.Invoke(this,new EventArgs());
					Close();
					break;
				case 1://Close Others
					EventWindowCloseOthers?.Invoke(this,new EventArgs());
					Close();
					break;
				case 2://Show All
					EventWindowShowAll?.Invoke(this,new EventArgs());
					Close();
					break;
			}
		}

		private void listBoxWindows_SelectionChangeCommitted(object sender, EventArgs e){
			EventWindowClicked?.Invoke(this,listBoxWindows.SelectedIndex);
			Close();
		}


	}
}