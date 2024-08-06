using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Converters;

namespace OpenDental.Drawing {
/*
How to use:
There are four supported drawing scenarios:
1. Drawing to a printer document. See WpfControls.PrinterL for example code.
2. Drawing to a bitmap. Example:
			Graphics g=Graphics.BitmapInit(100,100);
			g.Clear(Color.White);
			g.DrawRectangle... etc
			BitmapImage bitmapImage=g.BitmapCreate();
3. Drawing to screen. Example:
			Graphics g=Graphics.ScreenInit(panel);
			g.Clear(Color.White);
			g.DrawRectangle... etc
4. Measuring some text rather than drawing
			(using OpenDental.Drawing;)
			Graphics g=Graphics.MeasureBegin();
			double width=g.MeasureString("some text").Width;
			//or:
			double height=g.MeasureString("some text",width).Height;
			(if using this to measure screen elements, see the discussions further down and in the Font class)
These three different strategies are closely interrelated and you can use a single set of drawing code to draw to all three.
This class should allow you to use existing WinForms code with very little modification.
Just like we used "g" for an instance of System.Drawing.Graphics, we will use "g" for an instance of OpenDental.Drawing.Graphics.
Our old drawing code was all adapted to use the LayoutManager to scale everything.
You will need to remove all such scaling. It will be easy to see as red not compiling because LayoutManager doesn't exist here.
This will result in simpler code with everything simply drawn as if at 96 dpi.
Previously, the dpi for printing was 100 dpi, but now it's 96, exactly the same as the screen. Yes!!
So slight modifications to the printer math might be needed to change from 100 dpi to 96 dpi.
SmoothingMode can be removed. WPF draws everything smooth.
TextRenderingHint can be removed. WPF rendering is closest to ClearTypeGridFit, which is the highest quality.
FormSheetFillEdit uses the most intense drawing and it uses ClearTypeGridFit.
Far fewer overloads are provided than in WinForms. Part of this is because we can use optional parameters.
Doubles: All your variables will probably need to be changed from float, sizeF, etc to their double equivalents. Examples:
	Size and SizeF become (System.Windows.)Size, which is doubles.
	Rectangle and RectangleF become Rect
Add using statements at the top of any file where you need to use these new drawing and printing classes:
	using System.Windows.Media (for colors)
	using OpenDental.Drawing;
Font:
	WPF doesn't have a Font object, so we made our own to encapsulate fontFamily, size, bold, and underline into a single object.
	Create an OpenDental.Drawing.Font like this: Font font=new Font();//this will have normal defaults which you can then change.
Brushes and Pens:
	You can't use the System.Drawing brushes and pens.
	Instead, our methods are all designed to just take colors.
	Example: Pens.Black should be rewritten as Colors.Black
	Some Pen and Brush variables go away. Others must be converted to color variables prior to being used as arguments.
	If we need features like stroke width or gradient brush, we will add those features later as needed.
Colors:
	Use System.Windows.Media colors everywhere instead of System.Drawing colors.
	Don't forget to add the using statement descibed about 12 lines up from here.
	Colors coming from the database etc will need to be converted: Color color=ColorOD.ToWpf(dbObj.Color);
	The two classes have slightly different syntax: 
	Colors.Black instead of Color.Black
	Color.FromRgb instead of Color.FromArgb, etc
Margins are discussed more over in PrinterL.
	You should never need to know about margins to do your drawing, so we don't give you access.
	You can instead look at g.Width/Height etc, which is the area available to you within the margins.
Measuring strings
	g.MeasureString converts over very easily, including the optional width available parameter.
	TextRenderer.MeasureText should also be converted to g.MeasureString.
	g.MeasureCharactersFitted is a new method to replace a MeasureString overload.
StringFormat:
	These were required in WinForms for horiz and vert aligntment of strings.
	Just delete the existing ones and use the alignment parameters on DrawString.
	Don't get confused. The old "Alignment" meant horiz alignment, and the old "LineAlignment" meant vert alignement.
DrawSvg:
	This is an alternate class to this Graphics class
	It uses more powerful SVG style commands for drawing vectors.
	But it's a little harder to use.
WPF
	The discussion above all assumes you are drawing the old WinForms way.
	But what if you want to use this in a newer WPF window?
	This is especially important when measuring a string to lay it out on the screen.
	In that case, Font has a few helpers to help with size conversions.
	Also, MeasureString has an optional parameter for includePadding. This is typically false for WPF layout purposes.
*/
	///<summary>Designed to mimic System.Drawing.Graphics by drawing onto a canvas.</summary>
	public class Graphics {
		private Canvas _canvas;
		///<summary>In my testing, rectangles are drawing about 1.3 pixels shorter and narrower than GDI+. For now, I will cut the difference in half and adjust by 0.65 pixels. This is small enough that it won't be noticeable if wrong. I will continue to re-evaluate this adjustment.</summary>
		private double _adjRectSize=0.65;

		#region Ctor
		private Graphics(){

		}
		#endregion Ctor

		#region Properties
		public double Width{
			get=>_canvas.Width;
		}

		public double Height{
			get=>_canvas.Height;
		}
		#endregion Properties

		public BitmapImage BitmapCreate(){
			_canvas.Measure(new Size(_canvas.Width,_canvas.Height));
			_canvas.Arrange(new Rect(new Size(_canvas.Width, _canvas.Height)));
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)_canvas.Width, (int)_canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(_canvas);
			BitmapImage bitmapImage = new BitmapImage();
			using MemoryStream memoryStream = new MemoryStream();
			PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
			pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			pngBitmapEncoder.Save(memoryStream);
			memoryStream.Position = 0;
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.EndInit();
			return bitmapImage;
		}

		public static Graphics BitmapInit(double width,double height){
			Graphics g=new Graphics();
			g._canvas=new Canvas();
			g._canvas.Width=width;
			g._canvas.Height=height;
			return g;
		}

		public void Clear(Color color){
			FillRectangle(color,0,0,_canvas.Width-1,_canvas.Height-1);
		}

		public void DrawLine(Color color,double x1,double y1,double x2,double y2) {
			SnapIf96();
			Line line=new Line();
			line.X1=x1;
			line.Y1=y1;
			line.X2=x2;
			line.Y2=y2;
			line.StrokeThickness=1;
			line.Stroke=new SolidColorBrush(color);//ColorOD.ToWpf(pen.Color));
			_canvas.Children.Add(line);
		}

		public void DrawRectangle(Color color,double x,double y,double width,double height){
			SnapIf96();
			//CanvasCur.UseLayoutRounding=true;//these made no difference. 
			//CanvasCur.SnapsToDevicePixels=true;
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,x);
			Canvas.SetTop(rectangle,y);
			rectangle.Width=width+_adjRectSize;//Rectangles are about 1.3 pixels shorter and narrower than GDI+ in my testing environment. I will probably reduce or eliminate this if I can.
			rectangle.Height=height+_adjRectSize;
			rectangle.StrokeThickness=1;//unless we add support
			rectangle.Stroke=new SolidColorBrush(color);//ColorOD.ToWpf(pen.Color));
			_canvas.Children.Add(rectangle);
		}

		public void DrawRectangle(Color color,Rect rect){
			SnapIf96();
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,rect.X);
			Canvas.SetTop(rectangle,rect.Y);
			rectangle.Width=rect.Width+_adjRectSize;
			rectangle.Height=rect.Height+_adjRectSize;
			rectangle.StrokeThickness=1;
			rectangle.Stroke=new SolidColorBrush(color);//ColorOD.ToWpf(pen.Color));
			_canvas.Children.Add(rectangle);
		}

		///<summary></summary>
		public void DrawString(string s,Font font,Color color,Point point){
			DrawString(s,font,color,point.X,point.Y);
		}

		///<summary></summary>
		public void DrawString(string s,Font font,Color color,double x,double y){
			TextBlock textBlock=new TextBlock();
			Canvas.SetLeft(textBlock,x);
			Canvas.SetTop(textBlock,y);
			textBlock.Margin=new Thickness(left:2,0,0,0);
			//None of this seemed to be necessary and I didn't notice any change. It always draws nice.
			//textBlock.SetValue(TextOptions.TextRenderingModeProperty,TextRenderingMode.ClearType);
			//textBlock.UseLayoutRounding=true;
			//textBlock.SnapsToDevicePixels=true;
			textBlock.FontSize=font.Size*96f/72f;
			textBlock.FontFamily=new System.Windows.Media.FontFamily(font.Name);
			if(font.IsBold){
				textBlock.FontWeight=FontWeights.Bold;
			}
			if(font.IsUnderline){
				textBlock.TextDecorations=TextDecorations.Underline;
			}
			textBlock.Foreground=new SolidColorBrush(color);//ColorOD.ToWpf(((System.Drawing.SolidBrush)brush).Color));
			textBlock.Text=s;
			_canvas.Children.Add(textBlock);
		}

		///<summary></summary>
		public void DrawString(string s,Font font,Color color,Rect rect,
			HorizontalAlignment horizontalAlignment=HorizontalAlignment.Left,//stretch is not supported
			VerticalAlignment verticalAlignment=VerticalAlignment.Top)
		{
			TextBlock textBlock=new TextBlock();
			Canvas.SetLeft(textBlock,rect.X);
			Canvas.SetTop(textBlock,rect.Y);
			textBlock.Width=rect.Width;
			textBlock.Height=rect.Height;
			textBlock.Padding=new Thickness(left:2,0,right:2,bottom:2);
			textBlock.FontSize=font.Size*96f/72f;
			textBlock.FontFamily=new System.Windows.Media.FontFamily(font.Name);
			textBlock.Foreground=new SolidColorBrush(color);//ColorOD.ToWpf(((System.Drawing.SolidBrush)brush).Color));
			if(font.IsBold){
				textBlock.FontWeight=FontWeights.Bold;
			}
			if(font.IsUnderline){
				textBlock.TextDecorations=TextDecorations.Underline;
			}
			textBlock.Text=s;
			textBlock.TextWrapping=TextWrapping.Wrap;
			//default is already set to left
			if(horizontalAlignment==HorizontalAlignment.Center){
				textBlock.TextAlignment=TextAlignment.Center;
			}
			if(horizontalAlignment==HorizontalAlignment.Right){
				textBlock.TextAlignment=TextAlignment.Right;
			}
			if(verticalAlignment==VerticalAlignment.Top){
				_canvas.Children.Add(textBlock);
				return;
			}
			//The only way to do vertical alignment is to nest inside a grid.
			Grid grid=new Grid();
			textBlock.Height=double.NaN;//dynamic height
			//still same width as grid. Previous textBlock Canvas.Set... ignored.
			Canvas.SetLeft(grid,rect.X);
			Canvas.SetTop(grid,rect.Y);
			grid.Width=rect.Width;
			grid.Height=rect.Height;
			grid.Children.Add(textBlock);
			if(verticalAlignment==VerticalAlignment.Center){
				textBlock.VerticalAlignment=VerticalAlignment.Center;
			}
			if(verticalAlignment==VerticalAlignment.Bottom){
				textBlock.VerticalAlignment=VerticalAlignment.Bottom;
			}
			_canvas.Children.Add(grid);		
		}

		public void FillRectangle(Color color,double x,double y,double width,double height){
			SnapIf96();
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,x);
			Canvas.SetTop(rectangle,y);
			rectangle.Width=width;
			rectangle.Height=height;
			rectangle.Fill=new SolidColorBrush(color);//ColorOD.ToWpf(((System.Drawing.SolidBrush)brush).Color));
			_canvas.Children.Add(rectangle);
		}

		public void FillRectangle(Color color,Rect rect){
			SnapIf96();
			Rectangle rectangle=new Rectangle();
			Canvas.SetLeft(rectangle,rect.X);
			Canvas.SetTop(rectangle,rect.Y);
			rectangle.Width=rect.Width;
			rectangle.Height=rect.Height;
			rectangle.Fill=new SolidColorBrush(color);//ColorOD.ToWpf(((System.Drawing.SolidBrush)brush).Color));
			_canvas.Children.Add(rectangle);
		}

		///<summary></summary>
		public static Graphics MeasureBegin(){
			Canvas canvas=new Canvas();
			canvas.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
			canvas.Arrange(new Rect(new Size(100,100)));
			Graphics g=new Graphics();
			g._canvas=canvas;
			return g;
		}

		///<summary>Boilerplate example at the top of this file. includePadding is true by default to mimic WinForms. But if you truly want to just measure the text with no whitespace, like in WPF display, set it to false.</summary>
		public Size MeasureString(string text,Font font=null,double width=double.PositiveInfinity,bool includePadding=true){
			TextBlock textBlock=new TextBlock();
			if(width<double.PositiveInfinity){
				textBlock.Width=width;
			}
			//note: we could add an optional parameter to exclude padding to tightly measure the actual text
			//This padding was an attempt to duplicate the old WinForms code.
			if(includePadding){
				textBlock.Padding=new Thickness(left:2,0,right:2,bottom:2);
			}
			else{
				textBlock.Padding=new Thickness(0);
			}
			if(font is null){
				font=new Font();
			}
			textBlock.FontSize=font.Size*96f/72f;
			textBlock.FontFamily=new FontFamily(font.Name);
			if(font.IsBold){
				textBlock.FontWeight=FontWeights.Bold;
			}
			textBlock.Text=text;
			textBlock.TextWrapping=TextWrapping.Wrap;
			_canvas.Children.Add(textBlock);
			textBlock.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
			//textBlock.Arrange(new Rect(textBlock.DesiredSize));
			Size size= textBlock.DesiredSize;
			_canvas.Children.Remove(textBlock);
			return size;
		}

		///<summary>This replaces the WinForms MeasureString overload that returns charactersFitted as an "out".</summary>
		public int MeasureCharactersFitted(string text,Font font,Size sizeAvail){
			if(text.Length==0){
				return 0;
			}
			//WPF has no functionality for this.
			//The simplest approach is to repeatedly lay out a textBlock to find one that fits.
			TextBlock textBlock=new TextBlock();
			textBlock.Width=sizeAvail.Width;
			textBlock.Padding=new Thickness(left:2,0,right:2,bottom:2);
			textBlock.FontSize=font.Size*96/72;
			textBlock.FontFamily=new FontFamily(font.Name);
			if(font.IsBold){
				textBlock.FontWeight=FontWeights.Bold;
			}
			textBlock.TextWrapping=TextWrapping.Wrap;
			_canvas.Children.Add(textBlock);
			textBlock.Text=text;
			textBlock.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
			double hMeasured=textBlock.DesiredSize.Height;
			if(hMeasured<=sizeAvail.Height){//The measured height fits within the available space
				//this means we already know charactersFitted. We are done.
				_canvas.Children.Remove(textBlock);
				return text.Length;	
			}
			//From here down, we must find how many characters fit in the available space
			//algorithm:
			//if it doesn't fit 100, go backward by estimated percent
			//if it does fit 100, go forward by 50%
			//once the max and min are equal, we are done
			//So it's a modified binary search.
			int fits100=0;//lower bound that we know works. But we might be able to fit more.
			int tooBig=text.Length;//or could be just right
			bool dirFwd=false;//the first attempt will go back
			int idxTry;
			while(true) {
				if(tooBig-fits100<=1) {//They both fit, but differ by up to 1, so we will use smaller.
					_canvas.Children.Remove(textBlock);
					return fits100;
				}
				if(dirFwd) {					
					int incrementFwd = (tooBig-fits100)/2;
					if(incrementFwd==0) {//example (11-10)/2=0, so increment by 1 instead
						incrementFwd=1;
					}
					idxTry = fits100+incrementFwd;//Example =10+(15-10)/2=12, example2 =10+(11-10)/2=10
				}
				else{
					//Example: hMeasured=200, hAvail=160, so we want to try 80% on this pass.
					double percentBack=sizeAvail.Height/hMeasured;  //160/200=0.8 (pixels)
					int incrementBack=(int)(tooBig*(1-percentBack));//example incrementBack=400*(1-.8)=80 (idx) 
					if(tooBig-incrementBack<=fits100){//the estimate goes back too far
						incrementBack=(tooBig-fits100)/2;//there might be some tweaks we could use to get a better number.
					}
					if(incrementBack==0){
						incrementBack=1;
					}
					idxTry=tooBig-incrementBack;//example 400-80=320 (idx)
				}
				textBlock.Text=text.Substring(0,idxTry);
				textBlock.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
				hMeasured=textBlock.DesiredSize.Height;
				if(hMeasured<=sizeAvail.Height){//fits
					fits100=idxTry;//set lower bound
					//it doesn't matter which direction we were going to end up here.
					//In any case, go forward half the distance
					dirFwd=true;
				}
				else{//doesn't fit
					tooBig=idxTry;//set upper bounds
					//it doesn't matter which direction we were going to end up here.
					//In any case, go back a percentage
					dirFwd=false;
				}
			}
			/*
			while(true){
				//This loop worked fine, but took 3 seconds per page with very simple text.
				if(size.Height<=sizeLayout.Height){
					break;
				}
				if(charactersFitted==0){
					break;
				}
				charactersFitted--;
				textBlock.Text=text.Substring(0,charactersFitted);
				textBlock.Measure(new Size(double.PositiveInfinity,double.PositiveInfinity));
				size=textBlock.DesiredSize;
			}
			CanvasCur.Children.Remove(textBlock);
			return size;*/
			//Typeface typeface=new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
			//PresentationSource presentationSource = PresentationSource.FromVisual(CanvasCur);
			//double pixelsPerDip=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			//Tried using a TextFomatter here. It was no help.
		}

		///<summary></summary>
		public static Graphics PrinterInit(Canvas canvas){
			Graphics g=new Graphics();
			g._canvas=canvas;
			return g;
		}

		///<summary>The panel that you pass in should not have any children.</summary>
		public static Graphics ScreenInit(WpfControls.UI.Panel panel){
			Canvas canvas=new Canvas();
			canvas.Width=panel.Width;
			canvas.Height=panel.Height;
			panel.Items.Clear();
			panel.Items.Add(canvas);//this is the only control on the panel
			Graphics g=new Graphics();
			g._canvas=canvas;
			return g;
		}

		///<summary>This makes horiz and vert lines not blurry when on a normal 96dpi monitor.</summary>
		private void SnapIf96(){
			//Couldn't figure out how to just call it once, so we harmlessly call it repeatedly.
			PresentationSource presentationSource = PresentationSource.FromVisual(_canvas);
			if(presentationSource is null){
				return;//when printing?
			}
			double pixelsPerDip=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			if(pixelsPerDip==1){
				_canvas.SnapsToDevicePixels=true;
			}
		}
	}

	///<summary>WPF treats these fields separately, but we made our own Font object to group these fields together to make it slightly easier. Default font is Microsoft Sans Serif 8.25 because that's always what we used with WinForms. Use ForWpf and/or SizeDip when using with WPF screen.</summary>
	public class Font{
		///<summary>Default is 8.25</summary>
		public double Size=8.25;
		///<summary>Default is Microsoft Sans Serif. You can change it to whatever you want: Arial for example.</summary>
		public string Name="Microsoft Sans Serif";
		public bool IsBold=false;
		public bool IsUnderline=false;

		///<summary>Use this when you are creating a font for use in WPF DIPs instead of points.</summary>
		public double SizeDip{
			get=>Size*96/72;
			set=>Size=value*72/96;
		}

		public static Font ForWpf(){
			Font font=new Font();
			font.Name="Segoe UI";
			font.Size=11.5;
			return font;
		}
	}
}
