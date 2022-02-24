using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;

namespace OpenDentalWpf {
	/// <summary></summary>
	public partial class ContrDashGenericGraph:UserControl {
		///<summary>One path for each provider.  We currently combine all provs, so we just use the first path.</summary>
		private List<Path> ListPaths;
		/// <summary>Each item contains a list of dots corresponding to path points.</summary>
		private List<List<Ellipse>> ListDots;
		/// <summary>Each item contains a list of dots that are used for hover effects, not visible.</summary>
		private List<List<Ellipse>> ListDotsBig;
		private bool IsHovering;
		/// <summary>Each item contains a list of patient counts. This is the data that will be plotted.</summary>
		private List<List<int>> ListData;
		/// <summary>Each path gets a different color.</summary>
		private List<Color> ListColors;
		/// <summary>Can handle 1 through about 500.  May need to enhance for bigger range.</summary>
		private int YIncrement;
		///<summary>Either 1 or 1000.  A factor of 1000 will also trigger $ to show in certain places.</summary>
		private double YMultFactor;
		private List<int> visibleIndicesOld;
		private List<int> visibleIndices;

		public ContrDashGenericGraph() {
			InitializeComponent();
		}

		///<summary>Also fills the graph.</summary>
		public void FillData(string title,double yMultFactor,List<Color> listColors,List<List<int>> listData){
			labelTitle.Content=title;
			if(yMultFactor!=1 && yMultFactor!=1000){
				throw new ArgumentOutOfRangeException("yMultFactor can only be 1 or 1000.");
			}
			YMultFactor=yMultFactor;
			ListColors=listColors;
			ListData=listData;
			FillGraph();
		}

		/// <summary>This lets us set some paths not visible.  An empty collection means that all data is visible.</summary>
		public List<int> VisibleIndices{
			set{
				visibleIndices=value;
				FadeInAndOut();
			}
		}

		public DateTime DateStart{
			get{
				DateTime dateFirstThisMonth=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
				return dateFirstThisMonth.AddMonths(-12);
			}
		}

		public DateTime DateEnd{
			get{
				DateTime dateFirstThisMonth=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
				return dateFirstThisMonth.AddDays(-1);
			}
		}

		private void FillGraph() {
			double wCol=rectMain.Width/11d;
			//vertical lines----------------------------------------------------------------------
			for(double i=1;i<11;i++) {
				Line line=new Line();
				line.X1=rectMain.Left()+(i*wCol);
				line.Y1=rectMain.Top();
				line.X2=rectMain.Left()+(i*wCol);
				line.Y2=rectMain.Bottom();
				line.Stroke=Brushes.LightGray;
				line.StrokeThickness=1;
				canvasMain.Children.Add(line);
				//year marker
				if(DateStart.AddMonths((int)i).Month==1) {
					line=new Line();
					line.X1=rectMain.Left()+(i*wCol)-wCol/2d;
					line.Y1=rectMain.Top();
					line.X2=rectMain.Left()+(i*wCol)-wCol/2d;
					line.Y2=rectMain.Bottom();
					line.Stroke=Brushes.Black;
					line.StrokeThickness=1.5;
					Canvas.SetZIndex(line,5);//same as grid outline, causing horizontal lines to go under
					canvasMain.Children.Add(line);
				}
			}
			//x axis numbers-----------------------------------------------------------------------
			for(double i=0;i<12;i++) {
				Label label=new Label();
				string content=DateStart.AddMonths((int)i).ToString("%M");
				label.Content=content;
				label.MaxWidth=100;
				Canvas.SetTop(label,rectMain.Bottom()-4);
				Typeface typeface=new Typeface(FontFamily,FontStyle,FontWeight,FontStretch);
				FormattedText ft=new FormattedText(content,CultureInfo.CurrentCulture,FlowDirection.LeftToRight,typeface,FontSize,Foreground);
				double wText=ft.Width;
				//Debug.WriteLine(content+": "+wText.ToString("F0"));
				Canvas.SetLeft(label,rectMain.Left()+(i*wCol)-wText/2d-5);
				canvasMain.Children.Add(label);
			}
			//calculate max y and increments---------------------------------------------------------
			double maxValD=0;
			for(int p=0;p<ListData.Count;p++) {
				for(int i=0;i<ListData[p].Count;i++) {
					if((ListData[p][i]/YMultFactor)>maxValD) {
						maxValD=(double)ListData[p][i]/YMultFactor;
					}
				}
			}
			//add 5% for white space
			maxValD=1.05*maxValD;
			//round up to nearest int. This is important if the max is just over 1k.
			int maxVal=(int)maxValD+1;
			//calculate amount for each tick.  No more than 10 ticks
			//replace the code below later with a more elegant algorithm to handle unlimited scaling
			YIncrement=1;
			if(maxVal>10) {
				YIncrement=2;
			}
			if(maxVal>20) {
				YIncrement=5;
			}
			if(maxVal>50) {
				YIncrement=10;
			}
			if(maxVal>100) {
				YIncrement=20;
			}
			if(maxVal>200) {
				YIncrement=50;
			}
			//etc
			//int yCount=(int)(maxVal/10000);
			//double hRow=rectMain.Height/(yCount-1);
			double hRow=rectMain.Height/(double)maxVal*(double)YIncrement;//in pixels
			int tickCount=(int)(rectMain.Height/hRow);//trunc to int. Does not include the tick at zero.
			//horizontal lines----------------------------------------------------------------------
			for(double i=0;i<tickCount;i++) {
				//the first tick is not at zero
				Line line=new Line();
				line.X1=rectMain.Left();
				line.Y1=rectMain.Bottom()-((i+1)*hRow);
				line.X2=rectMain.Right();
				line.Y2=rectMain.Bottom()-((i+1)*hRow);
				line.Stroke=Brushes.LightGray;
				line.StrokeThickness=1;
				canvasMain.Children.Add(line);
			}
			//y axis numbers-----------------------------------------------------------------------
			for(double i=0;i<tickCount;i++) {
				Label label=new Label();
				string content=((i+1)*YIncrement).ToString();
				if(YMultFactor==1000){
					content=content+"k";
				}
				label.Content=content;
				label.MaxWidth=200;
				Canvas.SetTop(label,rectMain.Bottom()-((i+1)*hRow)-14);
				Canvas.SetLeft(label,-3);
				label.Width=rectMain.Left()+5;
				label.HorizontalContentAlignment=HorizontalAlignment.Right;
				canvasMain.Children.Add(label);
			}
			//Initialize-----------------------------------------------------------------------------
			ListPaths=new List<Path>();
			ListDots=new List<List<Ellipse>>();
			ListDotsBig=new List<List<Ellipse>>();
			//Paths and dots-------------------------------------------------------------------------
			for(int p=0;p<ListData.Count;p++) {
				PathFigure pathFig=new PathFigure();
				List<Ellipse> listDotsOneType=new List<Ellipse>();
				List<Ellipse> listDotsBigOneType=new List<Ellipse>();
				for(int i=0;i<ListData[p].Count;i++) {
					Point pt=new Point(rectMain.Left()+(i*wCol),rectMain.Bottom()-(double)((double)ListData[p][i]/YMultFactor*hRow/(double)YIncrement));
					if(i==0) {
						pt.X+=1;
					}
					if(i==ListData[p].Count-1) {
						pt.X-=1;
					}
					if(i==0) {
						pathFig.StartPoint=pt;
					}
					else {
						LineSegment lineSeg=new LineSegment();
						lineSeg.Point=pt;
						pathFig.Segments.Add(lineSeg);
					}
					//dots
					Ellipse ellipse=new Ellipse();
					ellipse.Height=4;
					ellipse.Width=4;
					Canvas.SetLeft(ellipse,pt.X-2);
					Canvas.SetTop(ellipse,pt.Y-2);
					ellipse.Fill=new SolidColorBrush(ListColors[p]);
					Panel.SetZIndex(ellipse,6);
					canvasMain.Children.Add(ellipse);
					listDotsOneType.Add(ellipse);
					//dotsBig
					ellipse=new Ellipse();
					ellipse.Height=14;
					ellipse.Width=14;
					Canvas.SetLeft(ellipse,pt.X-7);
					Canvas.SetTop(ellipse,pt.Y-7);
					ellipse.Opacity=0;
					ellipse.Fill=Brushes.Red;
					Panel.SetZIndex(ellipse,7);//in front of everything
					ellipse.MouseEnter+=new MouseEventHandler(dotBig_MouseEnter);
					ellipse.MouseLeave+=new MouseEventHandler(dotBig_MouseLeave);
					canvasMain.Children.Add(ellipse);
					listDotsBigOneType.Add(ellipse);
				}
				PathGeometry pathGeo=new PathGeometry();
				pathGeo.Figures.Add(pathFig);
				Path path=new Path();
				path.Data=pathGeo;
				path.Stroke=new SolidColorBrush(ListColors[p]);
				path.StrokeThickness=1.5;
				Panel.SetZIndex(path,6);//in front of grid
				canvasMain.Children.Add(path);
				ListPaths.Add(path);
				ListDots.Add(listDotsOneType);
				ListDotsBig.Add(listDotsBigOneType);
			}
		}

		void dotBig_MouseEnter(object sender,MouseEventArgs e) {
			if(IsHovering) {
				return;//don't jump when two big dots overlap
			}
			int idxPath=-1;
			int idxDot=-1;
			for(int i=0;i<ListDotsBig.Count;i++) {
				if(visibleIndices!=null && visibleIndices.Count>0 && !visibleIndices.Contains(i)){
					continue;
				}
				if(ListDotsBig[i].IndexOf((Ellipse)sender)==-1) {
					continue;
				}
				idxPath=i;
				idxDot=ListDotsBig[i].IndexOf((Ellipse)sender);
			}
			if(idxDot==-1) {
				return;
			}
			labelHover.Opacity=1;
			string numFormat="n0";
			if(YMultFactor==1000){
				numFormat="c0";
			}
			string content=DateStart.AddMonths(idxDot).ToString("MMM")+": "+ListData[idxPath][idxDot].ToString(numFormat);
			labelHover.Content=content;
			double xCenter=Canvas.GetLeft((Ellipse)sender)+7;
			double yCenter=Canvas.GetTop((Ellipse)sender)+7;
			Typeface typeface=new Typeface(FontFamily,FontStyle,FontWeight,FontStretch);
			FormattedText ft=new FormattedText(labelHover.Content.ToString(), 
				CultureInfo.CurrentCulture,FlowDirection.LeftToRight,typeface,FontSize,Foreground);
			double wText=ft.Width;
			Canvas.SetLeft(labelHover,xCenter-wText/2d-1);
			Canvas.SetTop(labelHover,yCenter-23);
			Panel.SetZIndex(labelHover,8);//bring to front of other element
			IsHovering=true;
		}

		void dotBig_MouseLeave(object sender,MouseEventArgs e) {
			if(!IsHovering) {
				return;
			}
			IsHovering=false;
			labelHover.Opacity=0;
			Panel.SetZIndex(labelHover,0);//send it to the back so it won't interfere with mouse hover
		}

		private void FadeInAndOut() {
			if(ListPaths==null) {
				return;
			}
			//Fade out--------------------------------------------------------------------------
			DoubleAnimation animation=new DoubleAnimation();
			animation.From=1;
			animation.To=0;
			animation.Duration=TimeSpan.FromMilliseconds(200);
			animation.FillBehavior=FillBehavior.HoldEnd;
			for(int p=0;p<ListPaths.Count;p++){
				if(visibleIndices.Count==0 || visibleIndices.Contains(p)){//don't fade out if it's supposed to be currently visible
					continue;
				}
				if(visibleIndicesOld!=null && visibleIndicesOld.Count>0 && !visibleIndicesOld.Contains(p)){//don't fade out if it was already not visible
					continue;
				}
				ListPaths[p].BeginAnimation(OpacityProperty,animation);
				for(int i=0;i<ListDots[p].Count;i++) {
					ListDots[p][i].BeginAnimation(OpacityProperty,animation);
					ListDotsBig[p][i].IsHitTestVisible=false;
					//the other elements won't interfere with hit test because of z-order.
				}
			}
			//Fade in---------------------------------------------------------------------------
			animation=new DoubleAnimation();
			animation.From=0;
			animation.To=1;
			animation.Duration=TimeSpan.FromMilliseconds(200);
			animation.FillBehavior=FillBehavior.HoldEnd;
			for(int p=0;p<ListPaths.Count;p++){
				if(visibleIndices.Count>0 && !visibleIndices.Contains(p)){//don't fade in if it's supposed to be currently not visible
					continue;
				}
				if(visibleIndicesOld==null || visibleIndicesOld.Count==0 || visibleIndicesOld.Contains(p)){//don't fade in if it was already visible
					continue;
				}
				ListPaths[p].BeginAnimation(OpacityProperty,animation);
				for(int i=0;i<ListDots[p].Count;i++) {
					ListDots[p][i].BeginAnimation(OpacityProperty,animation);
					ListDotsBig[p][i].IsHitTestVisible=true;
				}
			}
			visibleIndicesOld=new List<int>(visibleIndices);
		}



	}
}
