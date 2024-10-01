using OpenDental;
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
using System.Windows.Shapes;

namespace WpfControls.UI {
	/// <summary></summary>
	public partial class FrmColorDialog:FrmODBase {
		///<summary>Optionally set this before opening the form.  After closing, this will contain the new color.</summary>
		public Color Color;
		private Color _color;
		///<summary>This is set to true during loading and whenever else we are programmatically changing input boxes.  This lets us ignore textChanged events because we know they are not from the user typing.</summary>
		private bool _isInputLocked;
		///<summary>0 to 359</summary>
		private double _hue;
		///<summary>0 to 100</summary>
		private double _sat;
		///<summary>0 to 100</summary>
		private double _light;

		public FrmColorDialog():base() {
			InitializeComponent();
			Loaded+=Window_Loaded;
			gridSquareGradient.MouseDown+=gridSquareGradient_MouseDown;
			gridSquareGradient.MouseMove+=GridSquareGradient_MouseMove;
			gridSquareGradient.MouseUp+=GridSquareGradient_MouseUp;
		}

		#region Methods - private
		///<summary>Parses the RGB strings from the textboxes</summary>
		private void ParseColorFromRGB(){
			byte r;
			byte g;
			byte b;
			try{
				r=byte.Parse(textRed.textBox.Text);
				g=byte.Parse(textGreen.textBox.Text);
				b=byte.Parse(textBlue.textBox.Text);
			}
			catch{
				return;
			}
			_color=Color.FromRgb(r,g,b);
		}

		///<summary>Parses the hex string from the textbox</summary>
		private void ParseColorFromHex(){
			string hexStr=textHex.textBox.Text;
			if(hexStr==""){
				return;
			}
			if(hexStr.Substring(0,1)!="#"){
				hexStr="#"+hexStr;
			}
			object objectColor=null;
			try {
				objectColor=ColorConverter.ConvertFromString(hexStr);
			}
			catch {
				return;
			}
			_color=(Color)objectColor;
		}

		///<summary>Pass in hue 0 to 359, sat and light 0 to 100</summary>
		private Color CalcColorFromHSL(double hue,double sat,double light){
			//use 0 to 1:
			hue=hue/360;
			sat=sat/100;
			light=light/100;
			double temp1 =light+sat-(light*sat);
			if(light<0.5){
				temp1=light*(1+sat);
			}
			double temp2=2*light-temp1;
			double temp_R=hue+1d/3d;
			double temp_G=hue;
			double temp_B=hue-1d/3d;
			if(temp_R>1){
				temp_R-=1;
			}
			if(temp_R<0 && temp_R>-0.00001){
				temp_R=0;
			}
			if(temp_R<0){
				temp_R+=1;
			}
			if(temp_G>1){
				temp_G-=1;
			}
			if(temp_G<0 && temp_G>-0.00001){
				temp_G=0;
			}
			if(temp_G<0){
				temp_G+=1;
			}
			if(temp_B>1){
				temp_B-=1;
			}
			if(temp_B<0 && temp_B>-0.00001){
				temp_B=0;
			}
			if(temp_B<0){
				temp_B+=1;
			}
			#region RGB tests
			double Red=temp2+(temp1-temp2)*6*temp_R;
			//First test
			if((6*temp_R)>1){
				//Second test
				if((2*temp_R)>1){
					//Third test
					if((3*temp_R)>2){
						Red=temp2;
						goto exitRed;
					}
					Red=temp2+(temp1-temp2)*(2d/3d-temp_R)*6;
					goto exitRed;
				}
				Red=temp1;
			}
		exitRed:
			double Green=temp2+(temp1-temp2)*6*temp_G;
			//First test
			if((6*temp_G)>1){
				//Second test
				if((2*temp_G)>1){
					//Third test
					if((3*temp_G)>2){
						Green=temp2;
						goto exitGreen;
					}
					Green=temp2+(temp1-temp2)*(2d/3d-temp_G)*6;
					goto exitGreen;
				}
				Green=temp1;
			}
		exitGreen:
			double Blue=temp2+(temp1-temp2)*6*temp_B;
			//First test
			if((6*temp_B)>1){
				//Second test
				if((2*temp_B)>1){
					//Third test
					if((3*temp_B)>2){
						Blue=temp2;
						goto exitBlue;
					}
					Blue=temp2+(temp1-temp2)*(2d/3d-temp_B)*6;
					goto exitBlue;
				}
				Blue=temp1;
			}
		exitBlue:
			#endregion
			Red=Math.Round(Red*255);
			Green=Math.Round(Green*255);
			Blue=Math.Round(Blue*255);
			byte r=(byte)Red;
			byte g=(byte)Green;
			byte b=(byte)Blue;
			return Color.FromRgb(r,g,b);
		}

		///<summary>Sets _hue, _sat, and _light based off the current color</summary>
		private void SetHslFromColor(bool skipHue=false){
			float r=(float)_color.R/255;
			float g=(float)_color.G/255;
			float b=(float)_color.B/255;
			List<float> listFloats=new List<float>();
			listFloats.Add(r);
			listFloats.Add(g);
			listFloats.Add(b);
			float max=listFloats.Max();
			float min=listFloats.Min();
			//Light
			_light=((min+max)/2)*100;
			//Hue
			if(max-min==0){
				//input has no hue.  Like any shade of gray, black, or white.
				//So just leave the current hue alone.
			}
			else{
				float hue=0;
				if(r==max){
					hue=(g-b)/(max-min);
				}
				else if(g==max){
					hue=2+(b-r)/(max-min);
				}
				else if(b==max){
					hue=4+(r-g)/(max-min);
				}
				hue=hue*60;
				if(hue<0){
					hue=hue+360;
				}
				if(!skipHue){
					_hue=hue;
				}
			}
			//Sat
			if(min==max){
				_sat=0;
				return;
			}
			if(_light<=50){
				_sat=((max-min)/(max+min)*100);
			}
			if(_light>50){
				_sat=((max-min)/(2-max-min)*100);
			}
		}

		///<summary>Sets the box showing the final color.</summary>
		private void SetColorFinal(){
			if(panelColorFinal is null){
				//Jordan this was null in certain scenarios, although we can't duplicate now.
				return;
			}
			panelColorFinal.ColorBack=_color;
		}

		///<summary>//Sets hex box according to current color</summary>
		private void SetHex(){
			if(textHex is null){
				//Jordan this was null in certain scenarios, although we can't duplicate now.
				return;
			}
			_isInputLocked=true;
			textHex.Text=_color.ToString();//includes #
			_isInputLocked=false;
		}

		///<summary>Only done once on startup.</summary>
		private void SetHueBackground(){
			//order is Red-Yellow-Green-Blue-Purple-Red
			//There is no white or black or gray
			//In other words, it's always a mix of two colors, with the third color 0
			//There are 6 sections
			//Like this:
			//Red at 255
			//Green climbs to 255
			Rectangle rectangle=new Rectangle();
			rectangle.Width=60;
			rectangle.Height=15;
			LinearGradientBrush linearGradientBrush=new LinearGradientBrush(Color.FromRgb(255,0,0),Color.FromRgb(255,255,0),0);
			rectangle.Fill=linearGradientBrush;
			canvasHue.Children.Add(rectangle);
			//Red decreases to 0
			rectangle=new Rectangle();
			rectangle.Width=60;
			rectangle.Height=15;
			linearGradientBrush=new LinearGradientBrush(Color.FromRgb(255,255,0),Color.FromRgb(0,255,0),0);
			rectangle.Fill=linearGradientBrush;
			Canvas.SetLeft(rectangle,60);
			canvasHue.Children.Add(rectangle);
			//Blue climbs to 255
			rectangle=new Rectangle();
			rectangle.Width=60;
			rectangle.Height=15;
			linearGradientBrush=new LinearGradientBrush(Color.FromRgb(0,255,0),Color.FromRgb(0,255,255),0);
			rectangle.Fill=linearGradientBrush;
			Canvas.SetLeft(rectangle,120);
			canvasHue.Children.Add(rectangle);
			//Green decreases to 0
			rectangle=new Rectangle();
			rectangle.Width=60;
			rectangle.Height=15;
			linearGradientBrush=new LinearGradientBrush(Color.FromRgb(0,255,255),Color.FromRgb(0,0,255),0);
			rectangle.Fill=linearGradientBrush;
			Canvas.SetLeft(rectangle,180);
			canvasHue.Children.Add(rectangle);
			//Red climbs to 255
			rectangle=new Rectangle();
			rectangle.Width=60;
			rectangle.Height=15;
			linearGradientBrush=new LinearGradientBrush(Color.FromRgb(0,0,255),Color.FromRgb(255,0,255),0);
			rectangle.Fill=linearGradientBrush;
			Canvas.SetLeft(rectangle,240);
			canvasHue.Children.Add(rectangle);
			//Blue decreases to 0
			rectangle=new Rectangle();
			rectangle.Width=60;
			rectangle.Height=15;
			linearGradientBrush=new LinearGradientBrush(Color.FromRgb(255,0,255),Color.FromRgb(255,0,0),0);
			rectangle.Fill=linearGradientBrush;
			Canvas.SetLeft(rectangle,300);
			canvasHue.Children.Add(rectangle);
		}

		/// <summary>Sets the gradient canvas with a full saturation gradient of the hue color</summary>
		private void SetMainGradient(){
			LinearGradientBrush linearGradientBrush=new LinearGradientBrush(Colors.White, CalcColorFromHSL(_hue,100,50),0);
			canvas.Background=linearGradientBrush;
		}

		///<summary>Sets both the circle and the slider according to current color.</summary>
		private void SetPointersFromHSL(){
			//_hue, _sat, and _light already set
			Canvas.SetLeft(rectangleHueSlider,_hue-3);
			byte r=_color.R;
			byte g=_color.G;
			byte b=_color.B;
			Color colorH=CalcColorFromHSL(_hue,100,50);//Color of full saturation at upper right.
			//colorH will always have an rgb value that is set to 255 and a value that is set to 0. This is used to determine what values are passed to the method.
			Point point=new Point();
			List<byte> listBytesColorValues=new List<byte>();
			listBytesColorValues.Add(r);
			listBytesColorValues.Add(g);
			listBytesColorValues.Add(b);
			byte max=listBytesColorValues.Max();
			byte min=listBytesColorValues.Min();
			if(colorH.R==255 && colorH.B==0 && r==max && b==min){
				point=CalculatePoint(r,b);
			}
			else if(colorH.R==255 && colorH.G==0 && r==max && g==min){
				point=CalculatePoint(r,g);
			}
			else if(colorH.G==255 && colorH.R==0 && g==max && r==min){
				point=CalculatePoint(g,r);
			}
			else if(colorH.G==255 && colorH.B==0 && g==max && b==min){
				point=CalculatePoint(g,b);
			}
			else if(colorH.B==255 && colorH.R==0 && b==max && r==min){
				point=CalculatePoint(b,r);
			}
			else if(colorH.B==255 && colorH.G==0 && b==max && g==min){
				point=CalculatePoint(b,g);
			}
			Canvas.SetLeft(canvasEllipse,point.X);
			Canvas.SetTop(canvasEllipse,point.Y);
		}

		/// <summary>Helper method to calculate a point for the canvas based on two RGB values. The math below is the reverse of the math found in gridSquareGradient_MouseDown. Here we have the rgb values initially and instead need to find an x and y coordinate to go on the canvas.<summary>
		private Point CalculatePoint(byte maxHueRGB,byte minHueRgb){
			Point point=new Point();
			point.Y=maxHueRGB/255.0;
			if(point.Y==0) {//To avoid dividing by 0.
				return new Point(0,255);
			}
			double quotient=(double)minHueRgb/point.Y;
			point.X=(255.0-quotient)/255.0;
			point.X=point.X*255.0;//Revert decimal values to x,y coordinate on canvas.
			point.Y=255.0-(255.0*point.Y);
			return point;
		}

		///<summary>Sets all 3 RGB textboxes according to the current color.</summary>
		private void SetRgbText(){
			_isInputLocked=true;
			string s=_color.R.ToString();
			textRed.textBox.Text=_color.R.ToString();
			textGreen.textBox.Text=_color.G.ToString();
			textBlue.textBox.Text=_color.B.ToString();
			_isInputLocked=false;
		}
		#endregion Methods - private

		#region Methods - event handlers
		private void Window_Loaded(object sender,RoutedEventArgs e) {
			_color=Color;
			SetHueBackground();
			_isInputLocked=true;
			SetRgbText();
			SetHex();
			_isInputLocked=false;
			SetHslFromColor();
			SetPointersFromHSL();
			panelColorInitial.ColorBack=_color;
			SetMainGradient();
			SetColorFinal();
		}

		private void gridSquareGradient_MouseDown(object sender,MouseButtonEventArgs e) {
			Point point=e.GetPosition(gridSquareGradient);
			gridSquareGradient.CaptureMouse();
			MoveCircle(point);
		}

		private void GridSquareGradient_MouseMove(object sender,MouseEventArgs e) {
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(!isMouseDown){
				return;
			}
			Point point=e.GetPosition(gridSquareGradient);
			MoveCircle(point);
		}

		private void GridSquareGradient_MouseUp(object sender,MouseButtonEventArgs e) {
			gridSquareGradient.ReleaseMouseCapture();
		}

		private void MoveCircle(Point point){
			//Check to make sure the user stays within the canvas
			if(point.X>255){//Example grid is 256 pixels wide: 0-255.
				point.X=255;
			}
			if(point.Y>255){
				point.Y=255;
			}
			if(point.X<0){
				point.X=0;
			}
			if(point.Y<0){
				point.Y=0;
			}
			//move circle to that location
			Canvas.SetLeft(canvasEllipse,point.X);
			Canvas.SetTop(canvasEllipse,point.Y);
			double x=point.X/255.0;
			double y=(255.0-point.Y)/255.0; //from the bottom. 
			//x and y are now in 0 to 1 range.
			//Examples:
			//LL s,l =0,0    x,y=0,0
			//UL s,l =0,100  x,y=0,1
			//UR s,l =100,50 x,y=1,1
			//LR s,l =100,0  x,y=1,0
			//Mid s,l=33,38  x,y=.5,.5
			Color colorH=CalcColorFromHSL(_hue,100,50);//color of full saturation at upper right
			//Perform the calculations for RGB, taking the white/color gradient(x) * the black/white gradient(y)
			byte r=(byte)Math.Round(((x*colorH.R+(1-x)*255)*y));
			byte g=(byte)Math.Round(((x*colorH.G+(1-x)*255)*y));
			byte b=(byte)Math.Round(((x*colorH.B+(1-x)*255)*y));
			_color=Color.FromRgb(r,g,b);
			SetHslFromColor(skipHue:true);
			SetRgbText();
			SetHex();
			SetColorFinal();
		}

		private void canvasHue_MouseDown(object sender,MouseButtonEventArgs e) {
			double x=e.GetPosition(borderHue).X;
			_hue=(int)x;
			//set the slider
			Canvas.SetLeft(rectangleHueSlider,_hue-3);
			_color=CalcColorFromHSL(_hue,_sat,_light);
			SetMainGradient();
			SetRgbText();
			SetHex();
			SetColorFinal();
		}

		private void textRed_TextChanged(object sender,EventArgs e) {
			if(_isInputLocked){
				return;
			}
			ParseColorFromRGB();
			SetHslFromColor();
			SetHex();
			SetPointersFromHSL();
			SetMainGradient();
			SetColorFinal();
		}

		private void textGreen_TextChanged(object sender,EventArgs e) {
			if(_isInputLocked){
				return;
			}
			ParseColorFromRGB();
			SetHslFromColor();
			SetHex();
			SetPointersFromHSL();
			SetMainGradient();
			SetColorFinal();
		}

		private void textBlue_TextChanged(object sender,EventArgs e) {
			if(_isInputLocked){
				return;
			}
			ParseColorFromRGB();
			SetHslFromColor();
			SetHex();
			SetPointersFromHSL();
			SetMainGradient();
			SetColorFinal();
		}

		private void textHex_TextChanged(object sender,EventArgs e) {
			if(_isInputLocked){
				return;
			}
			ParseColorFromHex();
			SetHslFromColor();
			SetRgbText();
			SetPointersFromHSL();
			SetMainGradient();
			SetColorFinal();
		}

		private void butOK_Click(object sender,EventArgs e) {
			Color=_color;
			IsDialogOK=true;
		}
		#endregion Methods - event handlers

		//private class ColorD{
		//	public double R;
		//	public double G;
		//	public double B;

		//	public ColorD(){

		//	}

		//	public Color ToColor(){
		//		return Color.FromRgb(0,0,0);//round these
		//	}
		//}
	}
}

/*
Math:
https://www.niwa.nu/2013/05/math-behind-colorspace-conversions-rgb-hsl/


*/