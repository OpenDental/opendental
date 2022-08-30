using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	public partial class WarningIntegrity : UserControl{
		//We will scale based on size rather than LayoutManager
		private List<Tip> _listTips;
		///<summary></summary>
		private bool _didShowPopup;
		private Region _regionTriangle;
		///<summary>Usually 1, unless bigger than 18 wide.</summary>
		private float _scale;
		private Timer _timer;
		private ToolTipOD _toolTipOD;
		private DatabaseIntegrity _databaseIntegrity;

		public WarningIntegrity(){
			InitializeComponent();
			DoubleBuffered=true;
			_scale=Width/18f;
			_toolTipOD=new ToolTipOD();
			_toolTipOD.SetControlAndAction(this,ToolTipSetString);
		}

		protected override Size DefaultSize {
			get {
				return new Size(18,18);
			}
		}	

		public void SetTypeAndVisibility(EnumWarningIntegrityType warningIntegrityType,bool isHashValid){
			if(isHashValid){
				Visible=false;
				return;
			}
			//We have an invalid hash and we are going to show a triangle. Let's see how aggressive we will get.
			_databaseIntegrity=DatabaseIntegrities.GetOneClass(warningIntegrityType);
			if(_databaseIntegrity is null){
				//connection to HQ failed, so just don't show warnings.
				Visible=false;
				return;
			}
			_databaseIntegrity.WarningIntegrityType=warningIntegrityType;//if this was default, we want the correct class type
			//for both popup and triangle, we will show this triangle
			Visible=true;
		}

		///<summary></summary>
		private void ToolTipSetString(Point point) {
			if(_regionTriangle is null) {
				_toolTipOD.SetString("");
				return;
			}
			if(_regionTriangle.IsVisible(point)){
				_toolTipOD.SetString(Lan.g(this,"Click to learn about Database Integrity"),Font);
				return;
			}
			_toolTipOD.SetString("");
		}

		private void ShowPopup(){
			//This was called from OnPaint because I simply couldn't find anything else that would show it at the right time.
			if(_didShowPopup){
				return;//so that we just show it once
			}
			_didShowPopup=true;
			if(!Visible){
				return;
			}
			//We use a timer so that the draw will finish before the popup
			_timer=new Timer();
			_timer.Interval=100;
			_timer.Tick+=Timer_Tick;
			_timer.Enabled=true;
		}

		private void Timer_Tick(object sender,EventArgs e) {
			_timer.Enabled=false;//only happens once
			if(_databaseIntegrity is null){
				return;//probably an HQ connection issue, but this probably wouldn't fire anyway.
			}
			if(_databaseIntegrity.Behavior!=EnumIntegrityBehavior.Popup){
				return;
			}
			using FormDatabaseIntegrity formDatabaseIntegrity=new FormDatabaseIntegrity();
			formDatabaseIntegrity.MessageToShow=_databaseIntegrity.Message.Replace("[Class]",_databaseIntegrity.WarningIntegrityType.ToString());
			formDatabaseIntegrity.ShowDialog();
		}

		protected override void OnPaint(PaintEventArgs pe){
			//base.OnPaint(pe);
			Graphics g=pe.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			//using SolidBrush solidBrushBack=new SolidBrush(ColorOD.Background);
			g.Clear(ColorOD.Background);
			PointF[] pointFArray=new PointF[3];
			pointFArray[0]=new PointF(Width/2f,1*_scale);//top
			pointFArray[1]=new PointF(Width-1,Height-1-1*_scale);//BR
			pointFArray[2]=new PointF(0,Height-1-1*_scale);//BL
			//g.DrawPolygon(Pens.Red,pointArray);
			FillListTips(pointFArray,rounding:3*_scale);
			GraphicsPath graphicsPath = new GraphicsPath();
			for(int i=0;i<_listTips.Count;i++){
				//draw curve at this
				pointFArray=new PointF[5];
				pointFArray[0]=_listTips[i].PointFPrevious0;
				pointFArray[1]=_listTips[i].PointFTipPrevious1;
				pointFArray[2]=_listTips[i].PointFTipCurve2;
				pointFArray[3]=_listTips[i].PointFTipNext3;
				pointFArray[4]=_listTips[i].PointFNext4;
				//tension 1 is the most relaxed, but that just makes it look like an M
				graphicsPath.AddCurve(pointFArray,offset:1,numberOfSegments:2,tension:0.2f);
				//draw line between this tip and next
				graphicsPath.AddLine(_listTips[i].PointFTipNext3,pointFArray[4]=_listTips[i].PointFNext4);
			}
			_regionTriangle=new Region(graphicsPath);
			//255, 128, 0 is pure orange
			Color colorOrange=Color.FromArgb(255, 192, 128);
			using SolidBrush solidBrushOrange=new SolidBrush(colorOrange);
			g.FillPath(solidBrushOrange,graphicsPath);
			using Pen penOrange=new Pen(colorOrange);
			g.DrawPath(Pens.Orange,graphicsPath);
			//g.DrawLine(Pens.Blue,_listTips[0].PointFPrevious0,_listTips[0].PointFTipPrevious1);
			//g.DrawLine(Pens.Blue,pointFArray[3],pointFArray[4]);
			//Exclamation mark stalk:
			pointFArray=new PointF[4];
			pointFArray[0]=new PointF(Width/2f-1*_scale,Height/2f-4*_scale);//UL
			pointFArray[1]=new PointF(Width/2f+1*_scale,Height/2f-4*_scale);//UR
			pointFArray[2]=new PointF(Width/2f+0.8f*_scale,Height/2f+3*_scale);//BR
			pointFArray[3]=new PointF(Width/2f-0.8f*_scale,Height/2f+3*_scale);//BL
			g.FillPolygon(Brushes.White,pointFArray);
			RectangleF rectangleFCircle=new RectangleF(Width/2f-1.15f*_scale,13*_scale,2.3f*_scale,2.3f*_scale);
			g.FillEllipse(Brushes.White,rectangleFCircle);
			//using Pen penString=new Pen(Color.FromArgb(100,50,50));
			//g.DrawString("!",Font,Brushes.Black,LayoutManager.ScalePoint(new Point(5,2)));
			ShowPopup();
		}

		protected override void OnMouseDown(MouseEventArgs e){
			base.OnMouseDown(e);
			if(_databaseIntegrity is null){
				return;//probably an HQ connection issue, but this probably wouldn't fire anyway.
			}
			using FormDatabaseIntegrity formDatabaseIntegrity=new FormDatabaseIntegrity();
			//the next line applies to both Popup and Triangle when user clicks.
			formDatabaseIntegrity.MessageToShow=_databaseIntegrity.Message.Replace("[Class]",_databaseIntegrity.WarningIntegrityType.ToString());
			formDatabaseIntegrity.ShowDialog();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
		}

		protected override void OnPaintBackground(PaintEventArgs paintEventArgs){
			if(DesignMode){
				base.OnPaintBackground(paintEventArgs);
			}
			else{
				//don't normally paint it. Reduces flicker when we do our own painting and we don't have a "background".
			}
		}

		protected override void OnSizeChanged(EventArgs e){
			base.OnSizeChanged(e);
			_scale=Width/18f;
			Invalidate();
		}

		private void FillListTips(PointF[] pointFArray, float rounding){
			_listTips=new List<Tip>();
			for(int p=0;p<pointFArray.Length;p++){
				Line linePrevious;
				if(p==0){
					linePrevious=new Line(pointFArray[pointFArray.Length-1], pointFArray[p]);
				}
				else{
					linePrevious=new Line(pointFArray[p-1], pointFArray[p]);
				}
				linePrevious=ShortenLine(linePrevious,rounding);
				Line lineNext;
				if(p==pointFArray.Length-1){
					lineNext=new Line(pointFArray[p], pointFArray[0]);
				}
				else{
					lineNext=new Line(pointFArray[p], pointFArray[p+1]);
				}
				lineNext=ShortenLine(lineNext,rounding);
				Tip tip=new Tip();
				tip.PointFPrevious0=linePrevious.PointFPrevious;
				tip.PointFTipPrevious1=linePrevious.PointFNext;
				//tip.PointFTipCurve2 will be calculated soon
				tip.PointFTipNext3=lineNext.PointFPrevious;
				PointF pointFBetween = GetPointFBetween(tip.PointFTipPrevious1, tip.PointFTipNext3, 0.5f);
				tip.PointFTipCurve2=GetPointFBetween(pointFBetween, pointFArray[p], 0.3f);//change this proportion to change rounding shape				
				tip.PointFNext4=lineNext.PointFNext;
				_listTips.Add(tip);
			}
		}

		///<summary>A tip holds the info to draw a curved line using 3 points.  Going clockwise, it draws from PointPrevious to PointCurveTip to PointNext</summary>
		private class Tip{
			///<summary>0. This point is not drawn. It's to give the spline the right curve</summary>
			public PointF PointFPrevious0;
			///<summary>1</summary>
			public PointF PointFTipPrevious1;
			///<summary>2</summary>
			public PointF PointFTipCurve2;
			///<summary>3</summary>
			public PointF PointFTipNext3;
			///<summary>4. This point is not drawn. It's to give the spline the right curve</summary>
			public PointF PointFNext4;
		}

		struct Line{
			public PointF PointFPrevious; 
			public PointF PointFNext;

			public Line(PointF pointFPrevious, PointF pointFNext) { 
				PointFPrevious = pointFPrevious; 
				PointFNext = pointFNext; 
			}
		}

		///<summary>Shortens on both ends.</summary>
		private Line ShortenLine(Line line, float byPixels){
			float lengthOriginal = (float)Math.Sqrt(Math.Pow(line.PointFPrevious.X - line.PointFNext.X, 2)+ Math.Pow(line.PointFPrevious.Y - line.PointFNext.Y, 2));
			float proportion = (lengthOriginal - byPixels) / lengthOriginal;
			PointF pointFPrevious = GetPointFBetween(line.PointFPrevious, line.PointFNext, proportion);
			PointF pointFNext = GetPointFBetween(line.PointFPrevious, line.PointFNext, 1 - proportion);
			return new Line(pointFNext, pointFPrevious);
		}

		///<summary>With a proportion of 0.5 the point sits in the middle</summary>
		private PointF GetPointFBetween(PointF pointF1, PointF pointF2, float proportion){
			float x=pointF1.X + (pointF2.X - pointF1.X) * proportion;
			float y=pointF1.Y + (pointF2.Y - pointF1.Y) * proportion;
			return new PointF(x,y);
		}



	}
}


/*
Use it like this:
Copy/paste the control onto the form of interest at 0,0 or at 0,0 relative to some subsection of the form.
Size should be exactly 18x18.
Add the following single line in the Load event:
warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.Patient,Patients.IsPatientHashValid(_patientCur));

Edit from patient to the type in question.
This will frequently include adding a new EnumWarningIntegrityType, which must involve Jordan.
*/
