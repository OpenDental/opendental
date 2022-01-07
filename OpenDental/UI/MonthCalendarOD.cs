using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	//Jordan is the only one allowed to edit this file.

	///<summary>A replacement for the MS MonthCalendar control.</summary>
	public partial class MonthCalendarOD : Control{
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields -Private
		///<summary>The dates that are showing in each cell.  This is a 7x6 array.</summary>
		private DateTime[,] _arrayDates;
		///<summary>Unfortunately, this has the same name as a MS event for selecting a date.  So this can't be used publicly unless I'm careful.</summary>
		private DateTime _dateSelected;
		private float _heightCell;
		///<summary>Needs to be scaled each time.</summary>
		private int _heightFooter96=21;
		///<summary>Needs to be scaled each time.</summary>
		private int _heightHeader96=34;
		///<summary>Because we can go to different months without changing the selected date. We always use the first day of the month. Can't be changed externally.</summary>
		private DateTime _monthShowing;
		///<summary>Identifies the hot hover cell.  Row 0 is never used because that's the days of week. 0,0 indicates no hot.</summary>
		private Point _pointHotHover;
		private Rectangle _rectangleLeftArrow;
		private Rectangle _rectangleRightArrow;
		private Rectangle _rectangleTodayHover;
		///<summary>x Position of left of each cell.  Always 7 columns</summary>
		private float[] _xPos;
		///<summary>y Position of top of each cell.  Top row is days of week, then always 6 rows of numbers</summary>
		private float[] _yPos;
		#endregion Fields - Private

		#region Constructor
		public MonthCalendarOD(){
			InitializeComponent();
			DoubleBuffered=true;
			_dateSelected=DateTime.Today;
			_monthShowing=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
			CalculateRectangles();
		}
		#endregion Constructor

		#region Events - Public Raise
		///<summary>Occurs when user clicks to change date.  Does not fire in response to programmatic data changes.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks to change date.  Does not fire in response to programmatic data changes.")]
		public event EventHandler DateChanged;
		#endregion Events - Public Raise

		#region Properties
		protected override Size DefaultSize => new Size(227, 162);
		#endregion Properties

		#region Methods Override
		protected override void OnMouseDown(MouseEventArgs e){
			/*
			if(e.Y<_yPos[1]){
				return;
			}
			if(e.Y>Height-LayoutManager.Scale(_heightFooter96)){
				return;
			}
			if(_pointHotHover==new Point(0,0)){
				return;
			}*/
			if(_pointHotHover!=new Point(0,0)){
				_dateSelected=_arrayDates[_pointHotHover.X,_pointHotHover.Y-1];
				DateChanged?.Invoke(this,new EventArgs());
				if(_dateSelected.Month!=_monthShowing.Month){
					_monthShowing=new DateTime(_dateSelected.Year,_dateSelected.Month,1);
				}
				Invalidate();
			}
			if(_rectangleLeftArrow.Contains(e.Location)){
				_monthShowing=_monthShowing.AddMonths(-1);
				Invalidate();
			}
			if(_rectangleRightArrow.Contains(e.Location)){
				_monthShowing=_monthShowing.AddMonths(1);
				Invalidate();
			}
			if(_rectangleTodayHover.Contains(e.Location)){
				_dateSelected=DateTime.Today;
				DateChanged?.Invoke(this,new EventArgs());
				if(DateTime.Today.Month!=_monthShowing.Month){
					_monthShowing=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
				}
				Invalidate();
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseLeave(EventArgs e){
			_pointHotHover=new Point();
			Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e){
			if(_yPos is null){
				return;
			}
			int hotRow=0;
			if(e.Y>_yPos[1] && e.Y<Height-LayoutManager.Scale(_heightFooter96)){
				for(int i=2;i<_yPos.Length;i++){
					if(e.Y<_yPos[i]){
						hotRow=i-1;
						break;
					}
					hotRow=i;//last row;
				}
			}
			int hotCol=0;
			for(int i=1;i<_xPos.Length;i++){
				if(e.X<_xPos[i]){
					hotCol=i-1;
					break;
				}
				hotCol=i;//last col
			}
			if(hotRow==0){
				_pointHotHover=new Point();
			}
			else{
				_pointHotHover=new Point(hotCol,hotRow);
			}
			Invalidate();
			base.OnMouseMove(e);
		}

		protected override void OnFontChanged(EventArgs e){
			base.OnFontChanged(e);
			CalculateRectangles();
			Invalidate();
		}

		protected override void OnSizeChanged(EventArgs e){
			base.OnSizeChanged(e);
			CalculateRectangles();
			Invalidate();
		}
		#endregion Methods Overide

		#region Method OnPaint
		protected override void OnPaint(PaintEventArgs pe){
			Graphics g=pe.Graphics;
			base.OnPaint(pe);
			g.FillRectangle(Brushes.White,ClientRectangle);
			//Calculate cell sizes========================================================================================
			float widthCell=Width/7f;
			_heightCell=(Height-LayoutManager.Scale(_heightHeader96)-LayoutManager.Scale(_heightFooter96))/7f;
			_yPos=new float[7];
			for(int i=0;i<7;i++){
				_yPos[i]=LayoutManager.Scale(_heightHeader96)+_heightCell*(float)i;
			}
			_xPos=new float[7];
			for(int i=0;i<7;i++){
				_xPos[i]=(float)Width/7f*(float)i;
			}
			//Fill arrayDates=============================================================================================
			_arrayDates=new DateTime[7,6];
			//First day of week is not Sunday in China, for example.  So:
			DayOfWeek firstDayOfWeek=DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek;
			//Which day does the first fall on?
			DayOfWeek dayOfFirst=(DayOfWeek)((int)_monthShowing.DayOfWeek-(int)firstDayOfWeek);
			DateTime dateFilling=_monthShowing.AddDays(-(int)dayOfFirst);
			int row=0;
			int col=0;
			while(true){
				_arrayDates[col,row]=dateFilling;
				dateFilling=dateFilling.AddDays(1);
				if(col==6){
					col=0;
					row++;
				}
				else{
					col++;
				}
				if(row==6){
					break;
				}
			}
			//Horizontal line below weekdays==============================================================================
			using Pen penFaint=new Pen(ColorOD.Gray(220));
			g.DrawLine(penFaint,0,_yPos[1]-1,Width,_yPos[1]-1);
			//Hover effect================================================================================================
			Rectangle rectangle;//We make these rectangles a bit narrower than the entire cell
			using SolidBrush brushHover=new SolidBrush(ColorOD.Hover);
			if(_pointHotHover!=new Point()){
				rectangle=new Rectangle((int)_xPos[_pointHotHover.X]+LayoutManager.Scale(4),(int)_yPos[_pointHotHover.Y],
					(int)widthCell-LayoutManager.Scale(8),(int)_heightCell);
				g.FillRectangle(brushHover,rectangle);
			}
			//Date selected===============================================================================================
			//if(_monthShowing.Month==_dateSelected.Month){//date is showing. Can't do this.  Could be showing dimmed.
			for(int c=0;c<7;c++){
				for(int r=0;r<6;r++){
					if(_arrayDates[c,r]!=_dateSelected){
						continue;
					}
					rectangle=new Rectangle((int)_xPos[c]+LayoutManager.Scale(4),(int)_yPos[r+1],
						(int)widthCell-LayoutManager.Scale(8),(int)_heightCell);
					using SolidBrush brushSelected=new SolidBrush(Color.FromArgb(70,120,180));//smokey blue to let the orange pop
						//70,120,240));Pretty good. Solid pleasant blue.
					g.FillRectangle(brushSelected,rectangle);
				}
			}
			//}
			//Today outline===============================================================================================
			Color colorToday=Color.FromArgb(255,130,0);
			using Pen penToday=new Pen(colorToday);
			//if(_monthShowing.Month==DateTime.Today.Month){//today is showing. Can't do this. Could be dimmed
				for(int c=0;c<7;c++){
					for(int r=0;r<6;r++){
						if(_arrayDates[c,r]!=DateTime.Today){
							continue;
						}
						rectangle=new Rectangle((int)_xPos[c]+LayoutManager.Scale(4),(int)_yPos[r+1],
							(int)widthCell-LayoutManager.Scale(8),(int)_heightCell);
						g.DrawRectangle(penToday,rectangle);
					}
				}
			//}
			//Hover header================================================================================================
			Point pointMouse=PointToClient(Control.MousePosition);//introducing variable for debugging
			if(_rectangleLeftArrow.Contains(pointMouse)){
				g.FillRectangle(brushHover,_rectangleLeftArrow);
			}
			if(_rectangleRightArrow.Contains(pointMouse)){
				g.FillRectangle(brushHover,_rectangleRightArrow);
			}
			//Header======================================================================================================
			//alignment is centered, left right, but we need to tightly control vertical because we only have a few pixels to work with.
			using StringFormat stringFormat=new StringFormat(){Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Near };
			int topCellBuffer=(int)((_heightCell-Font.Height)/2f);
			string strMonth=_monthShowing.ToString("MMMM yyyy");
			rectangle=new Rectangle(0,topCellBuffer+LayoutManager.Scale(11),Width,LayoutManager.Scale(_heightHeader96));
			g.DrawString(strMonth,Font,Brushes.Black,rectangle,stringFormat);
			//Days of week================================================================================================
			for(int i=0;i<7;i++){
				rectangle=new Rectangle((int)_xPos[i],(int)_yPos[0]+topCellBuffer,(int)widthCell,(int)_heightCell);
				string str=null;
				//First day of week is not Sunday in China, for example.  So:
				int intDay=i;
				intDay+=(int)firstDayOfWeek;
				if(intDay>6){
					intDay-=7;
				}
				str=DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName((DayOfWeek)intDay);
				g.DrawString(str,Font,Brushes.Black,rectangle,stringFormat);
			}
			//Month arrows================================================================================================
			//Left
			g.SmoothingMode=SmoothingMode.HighQuality;
			PointF[] points=new PointF[3];
			points[0]=new PointF(-3,0);//left, relative to center
			points[1]=new PointF(3f,-5f);//top
			points[2]=new PointF(3f,5f);//bottom
			GraphicsState graphicsState=g.Save();
			g.TranslateTransform(LayoutManager.ScaleF(11),LayoutManager.ScaleF(18));
			g.ScaleTransform(LayoutManager.ScaleF(1),LayoutManager.ScaleF(1));
			g.FillPolygon(Brushes.Black,points);
			g.Restore(graphicsState);
			//Right
			graphicsState=g.Save();
			g.TranslateTransform(Width-LayoutManager.ScaleF(11),LayoutManager.ScaleF(18));
			g.ScaleTransform(LayoutManager.ScaleF(1),LayoutManager.ScaleF(1));
			g.RotateTransform(180);
			g.FillPolygon(Brushes.Black,points);
			g.Restore(graphicsState);
			//Dates=======================================================================================================
			using SolidBrush brushDim=new SolidBrush(ColorOD.Gray(170));
			for(int c=0;c<7;c++){
				for(int r=0;r<6;r++){
					RectangleF rectangleF=new RectangleF(_xPos[c],(int)_yPos[r+1]+topCellBuffer,
						widthCell,_heightCell);//_heightCell is sort of ignored because we are controling y
					if(_arrayDates[c,r]==_dateSelected){
						g.DrawString(_arrayDates[c,r].ToString("%d"),Font,Brushes.White,rectangleF,stringFormat);
					}
					else if(_arrayDates[c,r].Month==_monthShowing.Month){
						g.DrawString(_arrayDates[c,r].ToString("%d"),Font,Brushes.Black,rectangleF,stringFormat);
					}
					else{
						g.DrawString(_arrayDates[c,r].ToString("%d"),Font,brushDim,rectangleF,stringFormat);
					}
				}
			}
			//Hover footer================================================================================================
			string s=Lan.g("Calendar","Today:")+" "+DateTime.Today.ToShortDateString();
			int widthBoxAndText=(int)widthCell-LayoutManager.Scale(15)+(int)g.MeasureString(s,Font).Width;
			if(_rectangleTodayHover.Contains(pointMouse)){
				g.FillRectangle(brushHover,_rectangleTodayHover);
			}
			//Footer======================================================================================================
			int x=Width/2-widthBoxAndText/2;
			int heightFooter=LayoutManager.Scale(_heightFooter96);
			rectangle=new Rectangle(x,
				y:Height-heightFooter+LayoutManager.Scale(4),
				width:(int)widthCell-LayoutManager.Scale(15),
				height:heightFooter-LayoutManager.Scale(11));
				//(int)_heightCell);
			g.DrawRectangle(penToday,rectangle);
			x+=(int)widthCell-LayoutManager.Scale(13);			
			g.DrawString(s,Font,Brushes.Black,x,Height-heightFooter+LayoutManager.Scale(2));
			//Lines=======================================================================================================
			//g.DrawLine(penFaint,0,Height-heightFooter,Width,Height-heightFooter);
			using Pen penOutline=new Pen(ColorOD.Gray(150)); //ColorOD.Outline is too dark for this control
			g.DrawRectangle(penOutline,0,0,Width-1,Height-1);
		}
		#endregion Methods OnPaint

		#region Methods - Public
		///<summary>Use this in place of MonthCalendar.SelectionStart.</summary>
		public DateTime GetDateSelected(){
			return _dateSelected;
		}

		public Size GetDefaultSize(){
			return DefaultSize;
		}

		///<summary>Use this in place of MonthCalendar.SetDate().</summary>
		public void SetDateSelected(DateTime date){
			_dateSelected=date;
			_monthShowing=new DateTime(_dateSelected.Year,_dateSelected.Month,1);
			Invalidate();
		}
		#endregion Methods - Public

		#region Methods - Private
		private void CalculateRectangles(){
			_rectangleLeftArrow=new Rectangle(0,0,LayoutManager.Scale(26),LayoutManager.Scale(33));
			_rectangleRightArrow=new Rectangle(Width-LayoutManager.Scale(26),0,LayoutManager.Scale(26),LayoutManager.Scale(33));
			float widthCell=Width/7f;
			string s=Lan.g("Calendar","Today:")+" "+DateTime.Today.ToShortDateString();
			int widthBoxAndText=(int)widthCell-LayoutManager.Scale(15)+TextRenderer.MeasureText(s,Font).Width;
			int widthTodayHover=widthBoxAndText+LayoutManager.Scale(10);
			int heightFooter=LayoutManager.Scale(_heightFooter96);
			_rectangleTodayHover=new Rectangle(Width/2-widthTodayHover/2,
				y:Height-heightFooter,
				width:widthTodayHover,
				height:heightFooter);
		}
		#endregion Methods - Private

	}
}
