using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls.UI{
	///<summary></summary>
	public partial class TestContr : UserControl{
		public TestContr(){
			InitializeComponent();
			Loaded+=TestContr_Loaded;
		}

		private void TestContr_Loaded(object sender,RoutedEventArgs e) {
			// Create a drawing of two ellipses.
			GeometryDrawing geometryDrawing = new GeometryDrawing();
			EllipseGeometry ellipseGeometry1 = new EllipseGeometry();
			ellipseGeometry1.RadiusX = 20;
			ellipseGeometry1.RadiusY = 45;
			ellipseGeometry1.Center = new Point(50,50);
			EllipseGeometry ellipseGeometry2 = new EllipseGeometry();
			ellipseGeometry2.RadiusX = 45;
			ellipseGeometry2.RadiusY = 20;
			ellipseGeometry2.Center = new Point(50,50);
			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(ellipseGeometry1);
			geometryGroup.Children.Add(ellipseGeometry2);
			geometryDrawing.Geometry = geometryGroup;
			geometryDrawing.Brush = Brushes.Blue;
			Pen pen = new Pen();
			pen.Thickness = 10.0;
			pen.Brush = new LinearGradientBrush(Colors.Black,Colors.Gray,new Point(0,0),new Point(1,1));
			geometryDrawing.Pen = pen;
			DrawingBrush drawingBrush = new DrawingBrush();
			drawingBrush.Drawing = geometryDrawing;
			drawingBrush.Viewport = new Rect(0,0,0.25,0.25);
			drawingBrush.TileMode = TileMode.Tile;
			rectangle1.Fill = drawingBrush;

			BezierSegment bezierSegment=new BezierSegment(new Point(1,0),new Point(2,2),new Point(3,1),isStroked:true);
			PathFigure pathFigure=new PathFigure(start:new Point(0,1),new PathSegment[]{bezierSegment},closed:false);
			PathGeometry pathGeometry=new PathGeometry(new PathFigure[]{pathFigure });
			GeometryDrawing geometryDrawing2=new GeometryDrawing();
			geometryDrawing2.Geometry=pathGeometry;
			Pen pen2=new Pen(new SolidColorBrush(Color.FromRgb(235,0,0)),.4);//0.4 feels too thick, but anything less quickly fades out and looks terrible.
			pen2.EndLineCap=PenLineCap.Square;//add caps so that we have more to work with for splicing.
			pen2.StartLineCap=PenLineCap.Square;
			geometryDrawing2.Pen=pen2;
			DrawingBrush drawingBrush2 = new DrawingBrush();
			drawingBrush2.Drawing = geometryDrawing2;
			drawingBrush2.Viewport = new Rect(0,0,0.25,0.25);
			drawingBrush2.Viewbox=new Rect(0.082,0,.836,1);
			drawingBrush2.TileMode = TileMode.Tile;
			rectangle2.Fill = drawingBrush2;

			DrawingBrush drawingBrush3 = new DrawingBrush();
			drawingBrush3.Drawing = geometryDrawing2;
			//viewport: 
			//y is 10% negative to move it up slightly. Not exactly sure why it was down more than I expected.
			drawingBrush3.Viewport = new Rect(0,0,20,20);
			drawingBrush3.ViewportUnits=BrushMappingMode.Absolute;
			//viewbox:
			//height is slightly taller than the drawing to avoid vertical wrapping
			//left and right sides are slightly cut off so that they exactly line up and create a smooth joint. Arrived at by trial and error.
			drawingBrush3.Viewbox=new Rect(0.082,0,.836,1.1);
			drawingBrush3.TileMode = TileMode.Tile;
			line1.Stroke=drawingBrush3;

			DrawingBrush drawingBrush4 = new DrawingBrush();
			drawingBrush4.Drawing = geometryDrawing2;
			//Viewport: 
			//Y might need to be adjusted by some arbitrary number because the y origin of the fill pattern can depend on the y pos of the line.
			//We sometimes don't really know what the origin is, so the y adjustment compensates for that.
			//Width is a bit wider than height to arrive at an attractive proportion.
			drawingBrush4.Viewport = new Rect(0,0,5,4);//height of less than 4 fades out and looks terrible.
			drawingBrush4.ViewportUnits=BrushMappingMode.Absolute;
			//Viewbox:
			//Height is slightly taller than the drawing to avoid vertical wrapping.
			//Left and right sides are slightly cut off so that they exactly line up and create a smooth joint.
			//Arrived at by trial and error, and same regardless of the line thickness it's applied to.
			drawingBrush4.Viewbox=new Rect(0.082,0,.836,1.1);
			//drawingBrush2.Viewbox=new Rect(0.063,0,.874,1.1);//If we use thickness of 0.3, instead of 0.4
			drawingBrush4.TileMode = TileMode.Tile;
			line2.Stroke=drawingBrush4;
		}
	}
}
