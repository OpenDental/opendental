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
		//Each of the screen layouts gets a name, and each area gets a name.
		private GraphicsPath _graphicsPathHalf_L;
		private GraphicsPath _graphicsPathHalf_R;
		private GraphicsPath _graphicsPathQuarter_UL;
		private GraphicsPath _graphicsPathQuarter_UR;
		private GraphicsPath _graphicsPathQuarter_LL;
		private GraphicsPath _graphicsPathQuarter_LR;
		//we'll need to do the drawing with parameters instead of hard numbers
		///<summary>16. Margin between screens and around sides.</summary>
		private int _marginOuter;
		///<summary>7. Margin within each screen between snap locations.</summary>
		private int _marginInner;
		///<summary>120x80. The size of each "screen" area.</summary>
		private Size _sizeScreen;

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
			//screen graphics----------------------------------------------------------------------------------
			_marginOuter=(int)(10f*ScaleMy);
			_marginInner=(int)(5f*ScaleMy);
			_sizeScreen=new Size((int)(80f*ScaleMy),(int)(48f*ScaleMy));
			int x,y,w,h;
			RectangleF rectangleF;
			//Half is at UL-------------------------------------------------------------------------------------
			x=_marginOuter;
				//listBoxActions.Right+(int)(10f*ScaleMy)+_marginOuter;
			y=_marginOuter;
				//listBoxActions.Top;
			w=(_sizeScreen.Width-_marginInner)/2;
			h=_sizeScreen.Height;
			rectangleF=new RectangleF(x,y,w,h);
			_graphicsPathHalf_L=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true,roundLL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y,w,_sizeScreen.Height);
			_graphicsPathHalf_R=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true,roundLR:true);
			//Quarter is at UR-----------------------------------------------------------------------------------
			x=x+_sizeScreen.Width+_marginOuter;
			y=_marginOuter;
			//w no change
			h=(_sizeScreen.Height-_marginInner)/2;
			rectangleF=new RectangleF(x,y,w,h);
			_graphicsPathQuarter_UL=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y,w,h);//UR
			_graphicsPathQuarter_UR=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true);
			rectangleF=new RectangleF(x,y+h+_marginInner,w,h);//LL
			_graphicsPathQuarter_LL=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundLL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y+h+_marginInner,w,h);//LR
			_graphicsPathQuarter_LR=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundLR:true);
			//Other controls=====================================================================================
			x=_marginOuter;
			y=_marginOuter*2+_sizeScreen.Height;
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
			x=_marginOuter*3+_sizeScreen.Width*2;
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
			//int widthHalf=
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
			using SolidBrush brushBack=new SolidBrush(_colorBack);
			using SolidBrush brushHover=new SolidBrush(_colorHover);
			using Pen penOutline=new Pen(_colorOutline);
			using Pen penHoverOutline=new Pen(_colorOutline);
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;//horiz
			stringFormat.LineAlignment=StringAlignment.Center;//vert
			Point pointMouse=PointToClient(Control.MousePosition);
			//Half----------------------------------------------------------------------------------
			if(_graphicsPathHalf_L.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathHalf_L);
				g.DrawPath(penHoverOutline,_graphicsPathHalf_L);
			}
			else{
				g.FillPath(brushBack,_graphicsPathHalf_L);
				g.DrawPath(penOutline,_graphicsPathHalf_L);
			}
			g.DrawString("Left",Font,Brushes.Black,_graphicsPathHalf_L.GetBounds(),stringFormat);
			if(_graphicsPathHalf_R.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathHalf_R);
				g.DrawPath(penHoverOutline,_graphicsPathHalf_R);
			}
			else{
				g.FillPath(brushBack,_graphicsPathHalf_R);
				g.DrawPath(penOutline,_graphicsPathHalf_R);
			}
			g.DrawString("Right",Font,Brushes.Black,_graphicsPathHalf_R.GetBounds(),stringFormat);
			//Quarter--------------------------------------------------------------------------------
			//UL
			if(_graphicsPathQuarter_UL.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_UL);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_UL);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_UL);
				g.DrawPath(penOutline,_graphicsPathQuarter_UL);
			}
			g.DrawString("UL",Font,Brushes.Black,_graphicsPathQuarter_UL.GetBounds(),stringFormat);
			//UR
			if(_graphicsPathQuarter_UR.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_UR);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_UR);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_UR);
				g.DrawPath(penOutline,_graphicsPathQuarter_UR);
			}
			g.DrawString("UR",Font,Brushes.Black,_graphicsPathQuarter_UR.GetBounds(),stringFormat);
			//LL
			if(_graphicsPathQuarter_LL.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_LL);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_LL);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_LL);
				g.DrawPath(penOutline,_graphicsPathQuarter_LL);
			}
			g.DrawString("LL",Font,Brushes.Black,_graphicsPathQuarter_LL.GetBounds(),stringFormat);
			//LR
			if(_graphicsPathQuarter_LR.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_LR);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_LR);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_LR);
				g.DrawPath(penOutline,_graphicsPathQuarter_LR);
			}
			g.DrawString("LR",Font,Brushes.Black,_graphicsPathQuarter_LR.GetBounds(),stringFormat);
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