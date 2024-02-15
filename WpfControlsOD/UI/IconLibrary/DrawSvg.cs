using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfControls.UI {
	///<summary>This class allows concise chaining of SVG-style commands in order to draw vectors.</summary>
	public class DrawSvg {
/*
How to use this class.

Example
DrawSvg drawSvg=new DrawSvg();
drawSvg.CanvasCur=canvas;
...
drawSvg.BeginPath();
drawSvg.BeginFigure(14.42f,8f,isFilled:true);
drawSvg.AddArc(7.58f,8f,5.15f,5.15f,0f,false,true);
drawSvg.AddBezier(5.51f,9.78f,5.5f,13.07f,5.5f,13.07f);
drawSvg.AddLine(5.5f,19.38f);
drawSvg.AddBezier(7.33f,22.54f,14.67f,22.54f,16.5f,19.38f);
drawSvg.AddLine(16.5f,13.07f);
drawSvg.AddBezier(16.5f,13.07f,16.49f,9.78f,14.42f,8f);
drawSvg.EndFigure(isClosed:true);
drawSvg.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromRgb(119,35,27),colorOutline:Color.FromRgb(0,0,0),strokeWidth:1f);
drawSvg.FillEllipse(Color.FromRgb(119,35,27),11f,4f,3.75f,3.75f);
*/

		public Canvas CanvasCur;
		private PathFigure _pathFigure;
		private PathGeometry _pathGeometry;

		//All commands from here down are alphabetical
		///<summary>Adds an arc segment to a figure.</summary>
		public void AddArc(double xEnd,double yEnd,double xRadius,double yRadius,double rotation,bool isLargeArc,bool isCW){
			if(_pathFigure is null){
				throw new Exception("Must first call BeginFigure.");
			}
			ArcSegment arcSegment=new ArcSegment();
			arcSegment.Point=new Point(xEnd,yEnd);
			arcSegment.Size=new Size(xRadius,yRadius);
			arcSegment.RotationAngle=rotation;
			arcSegment.IsLargeArc=isLargeArc;
			if(isCW){
				arcSegment.SweepDirection=SweepDirection.Clockwise;
			}
			else{
				arcSegment.SweepDirection=SweepDirection.Counterclockwise;
			}
			_pathFigure.Segments.Add(arcSegment);
		}

		///<summary>Adds an bezier segment to a figure.</summary>
		public void AddBezier(double x1control,double y1control,double x2control,double y2control,double xEnd,double yEnd){
			if(_pathFigure is null){
				throw new Exception("Must first call BeginFigure.");
			}
			BezierSegment bezierSegment=new BezierSegment();
			bezierSegment.Point1=new Point(x1control,y1control);
			bezierSegment.Point2=new Point(x2control,y2control);
			bezierSegment.Point3=new Point(xEnd,yEnd);
			_pathFigure.Segments.Add(bezierSegment);
		}

		//<summary>Cardinal spline. This is not supported in SVG or WPF, but is supported in C# WinForms. This would be accomplished in SVG with a series of bezier segments where we would need to also specify manual control points. So this would easier if it was supported because we define the basic path of points to follow and then we don't worry about control point at all. The first and last points do not need to be drawn, but act as control points to help shape the ends of the curve.</summary>
		//public void AddCurve(){
		//	//tension 1 is the most relaxed, but that just makes it look like a jagged line
		//		graphicsPath.AddCurve(pointFArray,offset:1,numberOfSegments:2,tension:0.2f);
		//}

		///<summary>Adds a line segment to a figure.</summary>
		public void AddLine(double x,double y){
			if(_pathFigure is null){
				throw new Exception("Must first call BeginFigure.");
			}
			LineSegment lineSegment=new LineSegment();
			lineSegment.Point=new Point(x,y);
			_pathFigure.Segments.Add(lineSegment);
		}

		///<summary>Adds quadratic bezier segment to a figure.</summary>
		public void AddQuadraticBezier(double xControl,double yControl,double xEnd,double yEnd){
			if(_pathFigure is null){
				throw new Exception("Must first call BeginFigure.");
			}
			QuadraticBezierSegment quadraticBezierSegment=new QuadraticBezierSegment();
			quadraticBezierSegment.Point1=new Point(xControl,yControl);
			quadraticBezierSegment.Point2=new Point(xEnd,yEnd);
			_pathFigure.Segments.Add(quadraticBezierSegment);
		}

		///<summary>If this figure will have a fill applied, use isFilled=true.</summary>
		public void BeginFigure(double xStart,double yStart,bool isFilled){
			if(_pathGeometry is null){
				throw new Exception("Must first call BeginPath.");
			}
			if(_pathFigure!=null){
				throw new Exception("Already drawing a figure. EndFigure was not called.");
			}
			_pathFigure=new PathFigure();
			_pathFigure.StartPoint=new Point(xStart,yStart);
			_pathFigure.IsFilled=isFilled;
			//isClosed gets set during EndFigure
			//PathFigure is composed of one or more PathSegments which will be added using one of the Add methods above.
		}

		///<summary>Call BeginFigure after this.</summary>
		public void BeginPath(){
			if(_pathGeometry!=null){
				throw new Exception("Already drawing a path. EndPath was not called.");
			}
			if(_pathFigure!=null){
				throw new Exception("Already drawing a figure. EndFigure was not called.");
			}
			//PathGeometry is a collection of PathFigures.
			//Usually, there is only one PathFigure per PathGeometry.
			//Fill is applied at the PathGeometry level using FillRule.
			//Once a PathGeometry is assembled and complete, it's attached to a WPF control called Path, and Path.Data is set.
			//It's the Path control that's added to the Canvas.
			_pathGeometry=new PathGeometry();
		}

		///<summary>Default is white.</summary>
		public void Clear(Color? color = null) {
			if(color is null) {
				color=Colors.White;
			}
			FillRectangle(color.Value,0,0,CanvasCur.Width,CanvasCur.Height,1);
		}

		//public void DrawAtlas(int bitmapNum,int xSource,int ySource,int sizeSource,int x, int y,int size){
		//if(_context==EnumContext.WindowHandle){
		//	D2DWrapperHwnd.WrapperHwnd_DrawAtlas(_pD2DWrapperHwnd,bitmapNum,xSource,ySource,sizeSource,x,y,size);
		//}
		//not supported yet
		//}

		//public void DrawBitmap(int bitmapNum,int x,int y,int width,int height){
		//if(_context==EnumContext.WindowHandle){
		//	D2DWrapperHwnd.WrapperHwnd_DrawBitmap(_pD2DWrapperHwnd,bitmapNum,x,y,width,height);
		//}
		//not supported yet
		//}

		//public void DrawBitmapImmediate(){//Bitmap bitmap,Rectangle rectangle){

		//}

		public void DrawEllipse(Color color,double centerX,double centerY,double radiusX,double radiusY,double strokeWidth=1,double scale=1){
			Ellipse ellipse=new Ellipse();
			Canvas.SetLeft(ellipse,scale*(centerX-radiusX));
			Canvas.SetTop(ellipse,scale*(centerY-radiusY));
			ellipse.Width=radiusX*2;
			ellipse.Height=radiusY*2;
			ellipse.Stroke=new SolidColorBrush(color);
			ellipse.StrokeThickness=strokeWidth;
			CanvasCur.Children.Add(ellipse);
		}

		public void DrawLine(Color color,double x1,double y1,double x2,double y2,double strokeWidth=1){
			Line line=new Line();
			line.X1=x1;
			line.Y1=y1;
			line.X2=x2;
			line.Y2=y2;
			line.Stroke=new SolidColorBrush(color);
			line.StrokeThickness=strokeWidth;
			CanvasCur.Children.Add(line);
		}

		//<summary>Not implemented</summary>
		//public void DrawText(Color color,double x,double y,double width,double height,double fontSize,string text,double scale){
		//	TextBlock textBlock=new TextBlock();
		//	Canvas.SetLeft(textBlock,x);
		//	Canvas.SetTop(textBlock,y);
		//	textBlock.Width=width;
		//	textBlock.Height=height;
		//	textBlock.FontSize=fontSize;//but this is in DIPs instead of point. Might need to do a conversion? Not sure.
		//	//conversion would be DIPs=points*96/72
		//	textBlock.Foreground=new SolidColorBrush(color);
		//	textBlock.Text=text;
		//	CanvasCur.Children.Add(textBlock);
		//}

		public void DrawRectangle(Color color,double x,double y,double width,double height,double strokeWidth=1,double scale=1){
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,x*scale);
			Canvas.SetTop(rectangle,y*scale);
			rectangle.Width=width;
			rectangle.Height=height;
			rectangle.Stroke=new SolidColorBrush(color);
			rectangle.StrokeThickness=strokeWidth;
			CanvasCur.Children.Add(rectangle);
		}

		public void DrawRoundedRectangle(Color color,double x,double y,double width,double height,double radiusCorners,double strokeWidth=1,double scale=1){
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,x*scale);
			Canvas.SetTop(rectangle,y*scale);
			rectangle.Width=width;
			rectangle.Height=height;
			rectangle.Stroke=new SolidColorBrush(color);
			rectangle.StrokeThickness=strokeWidth;
			rectangle.RadiusX=radiusCorners;
			rectangle.RadiusY=radiusCorners;
			CanvasCur.Children.Add(rectangle);
		}

		///<summary></summary>
		public void EndFigure(bool isClosed=true){
			if(_pathGeometry is null){
				throw new Exception("Must first call BeginPath, add a figure, and segments to the figure.");
			}
			if(_pathFigure is null){
				throw new Exception("Must first call BeginFigure.");
			}
			if(_pathFigure.Segments.Count==0){
				throw new Exception("Must first add segments to the figure.");
			}
			//already did start point and isFilled
			_pathFigure.IsClosed=isClosed;
			_pathGeometry.Figures.Add(_pathFigure);
			_pathFigure=null;
		}

		///<summary>Make sure to call EndFigure first.  Frequently only a few of these parameters are needed.  Name the ones you need and ignore the rest. Default colors are black.  You can do fill and/or outline.</summary>
		public void EndPath(bool isFilled=true,bool isOutline=true,Color? colorFill=null,Color? colorOutline=null,double strokeWidth=1){
			if(_pathGeometry==null){
				throw new Exception("Must first call BeginPath, add a figure, and segments to the figure.");
			}
			if(_pathGeometry.Figures.Count==0){
				throw new Exception("Must first add figures to the path.");
			}
			if(_pathFigure!=null){
				throw new Exception("Must first call EndFigure.");
			}
			if(!isFilled && !isOutline){
				throw new Exception("Must specify either a fill or an outline.");
			}
			if(colorFill is null){
				colorFill=Colors.Black;
			}
			if(colorOutline is null){
				colorOutline=Colors.Black;
			}
			Path path=new Path();
			if(isFilled){
				path.Fill=new SolidColorBrush(colorFill.Value);
			}
			if(isOutline){
				path.Stroke=new SolidColorBrush(colorOutline.Value);
				path.StrokeThickness=strokeWidth;
			}
			path.Data=_pathGeometry;
			CanvasCur.Children.Add(path);
			_pathGeometry=null;
		}

		public void FillEllipse(Color color,double centerX,double centerY,double radiusX,double radiusY,double scale){
			Ellipse ellipse=new Ellipse();
			Canvas.SetLeft(ellipse,scale*(centerX-radiusX));
			Canvas.SetTop(ellipse,scale*(centerY-radiusY));
			ellipse.Width=radiusX*2;
			ellipse.Height=radiusY*2;
			ellipse.Fill=new SolidColorBrush(color);
			CanvasCur.Children.Add(ellipse);
		}

		public void FillRectangle(Color color,double x,double y,double width,double height,double scale){
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,x*scale);
			Canvas.SetTop(rectangle,y*scale);
			rectangle.Width=width;
			rectangle.Height=height;
			rectangle.Fill=new SolidColorBrush(color);
			CanvasCur.Children.Add(rectangle);
		}

		//<summary>Specify gradientNum=0 until we need and support multiple gradients</summary>
		//public void FillRectangleGradient(int gradientNum,float x,float y,float width,float height){
			
		//}

		public void FillRoundedRectangle(Color color,double x,double y,double width,double height,double radiusCorners,double scale){
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,x*scale);
			Canvas.SetTop(rectangle,y*scale);
			rectangle.Width=width;
			rectangle.Height=height;
			rectangle.Fill=new SolidColorBrush(color);
			rectangle.RadiusX=radiusCorners;
			rectangle.RadiusY=radiusCorners;
			CanvasCur.Children.Add(rectangle);
		}

		//<summary>Use after SaveDrawingState.</summary>
		//public void RestoreDrawingState(int levelNum=0){
		//	if(levelNum>1){
		//		throw new Exception("Only zero and one are supported for now.");
		//	}
			
		//}

		//<summary></summary>
		//public void Rotate(float angle){
			//if we need this, then we will need to create a class level transform that includes both this and scale.
		//}

		//<summary>Caller must keep track of which level they want to call.  Also see RestoreDrawingState.</summary>
		//public void SaveDrawingState(int levelNum=0){
		//	if(levelNum>1){
		//		throw new Exception("Only zero and one are supported for now.");
		//	}
			
		//}

		//<summary></summary>
		//public void Scale(double scale){
		//	ScaleTransform scaleTransform=new ScaleTransform(scale,scale);
		//	CanvasCur.LayoutTransform=scaleTransform;
		//	//only supports one transform as written, which should be fine.
		//}

		//<summary></summary>
		//public void Translate(float x,float y){
			
		//}

	}
}
