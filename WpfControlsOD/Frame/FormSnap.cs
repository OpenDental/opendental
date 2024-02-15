using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This is a small window that will come up when hovering over the maximize button to duplicate the funcationality found in Win11 for snapping a window to some screen position.</summary>
	public partial class FormSnap:Form {
		public float ScaleMy=1;
		///<summary>This is the lower middle of the button that launched this window, in screen coordinates.  This window will center itself on the anchor point.</summary>
		public Point PointAnchor;

		private Color _colorBack=Color.FromArgb(224,224,224);
		private Color _colorOutline=Color.FromArgb(140,140,140);
		private Color _colorHover=Color.FromArgb(0,103,192);
		private Color _colorHoverOutline=Color.FromArgb(33,96,150);
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

		public FormSnap() {
			InitializeComponent();
			//Lans.F(this);
		}

		private void FormSnap_Load(object sender, EventArgs e){
			_marginOuter=(int)(16f*ScaleMy);
			_marginInner=(int)(7f*ScaleMy);
			_sizeScreen=new Size((int)(120f*ScaleMy),(int)(80f*ScaleMy));
			int x,y,w,h;
			RectangleF rectangleF;
			//Half is at UL-------------------------------------------------------------------------------------
			x=_marginOuter;
			y=_marginOuter;
			w=(_sizeScreen.Width-_marginInner)/2;
			h=_sizeScreen.Height;
			rectangleF=new RectangleF(x,y,w,h);
			_graphicsPathHalf_L=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUL:true,roundLL:true);
			rectangleF=new RectangleF(x+w+_marginInner,y,w,_sizeScreen.Height);
			_graphicsPathHalf_R=GraphicsHelper.GetRoundedPathPartial(rectangleF,5,roundUR:true,roundLR:true);
			//Quarter is at UR-----------------------------------------------------------------------------------
			x=_marginOuter*2+_sizeScreen.Width;
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
			//Size and Location of form
			Size=new Size(_sizeScreen.Width*2+_marginOuter*3,_sizeScreen.Height+_marginOuter*2);
			x=PointAnchor.X-Width/2;
			Rectangle rectangleWorking=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;
			if(x+Width>rectangleWorking.Right){
				x=rectangleWorking.Right-Width;
			}
			y=PointAnchor.Y;
			Location=new Point(x,y);//changing location here does not cause flicker
		}
		
		private void FormSnap_Paint(object sender, PaintEventArgs e){
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			using SolidBrush brushBack=new SolidBrush(_colorBack);
			using SolidBrush brushHover=new SolidBrush(_colorHover);
			using Pen penOutline=new Pen(_colorOutline);
			using Pen penHoverOutline=new Pen(_colorOutline);
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
			if(_graphicsPathHalf_R.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathHalf_R);
				g.DrawPath(penHoverOutline,_graphicsPathHalf_R);
			}
			else{
				g.FillPath(brushBack,_graphicsPathHalf_R);
				g.DrawPath(penOutline,_graphicsPathHalf_R);
			}
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
			//UR
			if(_graphicsPathQuarter_UR.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_UR);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_UR);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_UR);
				g.DrawPath(penOutline,_graphicsPathQuarter_UR);
			}
			//LL
			if(_graphicsPathQuarter_LL.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_LL);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_LL);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_LL);
				g.DrawPath(penOutline,_graphicsPathQuarter_LL);
			}
			//LR
			if(_graphicsPathQuarter_LR.IsVisible(pointMouse)){
				g.FillPath(brushHover,_graphicsPathQuarter_LR);
				g.DrawPath(penHoverOutline,_graphicsPathQuarter_LR);
			}
			else{
				g.FillPath(brushBack,_graphicsPathQuarter_LR);
				g.DrawPath(penOutline,_graphicsPathQuarter_LR);
			}
		}

		private void FormSnap_MouseMove(object sender, MouseEventArgs e){
			//GraphicsPath gp;
			//gp.IsVisible(e.Location);
			Invalidate();
		}

		private void timerClose_Tick(object sender, EventArgs e){
			//maybe make sure we are outside the form first?
			Close();
		}

		private void FormSnap_MouseLeave(object sender, EventArgs e){
			timerClose.Enabled=true;
		}

		private void FormSnap_MouseEnter(object sender, EventArgs e){
			timerClose.Enabled=false;
		}


	}
}