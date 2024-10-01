using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Drawing;
using OpenDental.UI;
using OpenDental;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.
*/
	///<summary>This header panel shows at the top of ControlImageDock. It is designed to mimic the look and function of a normal window title bar, but it's really totally different.</summary>
	public partial class ImageDockHeader : UserControl{
		#region Fields - Private
		///<summary>Property backer.</summary>
		private bool _isButWinPressed;
		private bool _isEmpty=true;
		private bool _isHoverMax;
		private bool _isHoverMin;
		private bool _isHoverWin;
		private bool _isHoverX;
		///<summary>Property backer.</summary>
		private bool _isSelected;
		private bool _isMouseDown;
		private Point _pointMouseDownScreen;
		#endregion Fields - Private

		#region Constructor
		public ImageDockHeader(){
			InitializeComponent();
			Focusable=true;
			//GotFocus+=ImageDockHeader_GotFocus;
			//LostFocus+=ImageDockHeader_LostFocus;
			borderDrag.MouseLeftButtonDown+=BorderDrag_MouseLeftButtonDown;
			borderDrag.MouseLeftButtonUp+=BorderDrag_MouseLeftButtonUp;
			borderDrag.MouseMove+=BorderDrag_MouseMove;
			grid.MouseLeftButtonDown+=Grid_MouseLeftButtonDown;
			rectangleClose.MouseLeftButtonDown+=RectangleClose_MouseLeftButtonDown;
			rectangleClose.MouseLeave+=RectangleClose_MouseLeave;
			rectangleClose.MouseMove+=RectangleClose_MouseMove;
			rectangleMax.MouseLeftButtonDown+=RectangleMax_MouseLeftButtonDown;
			rectangleMax.MouseLeave+=RectangleMax_MouseLeave;
			rectangleMax.MouseMove+=RectangleMax_MouseMove;
			rectangleMin.MouseLeftButtonDown+=RectangleMin_MouseLeftButtonDown;
			rectangleMin.MouseLeave+=RectangleMin_MouseLeave;
			rectangleMin.MouseMove+=RectangleMin_MouseMove;
			rectangleWindows.MouseLeftButtonDown+=RectangleWin_MouseLeftButtonDown;
			rectangleWindows.MouseLeave+=RectangleWin_MouseLeave;
			rectangleWindows.MouseMove+=RectangleWin_MouseMove;
			SetColors();
		}

		/*
		private void ImageDockHeader_LostFocus(object sender,RoutedEventArgs e) {
			bool hasKBFocus=IsFocused;
			bool hasLFocus=IsKeyboardFocused;
			return;
		}

		private void ImageDockHeader_GotFocus(object sender,RoutedEventArgs e) {
			bool hasKBFocus=IsFocused;
			bool hasLFocus=IsKeyboardFocused;
			return;
		}*/
		#endregion Constructor

		#region Events - Raise
		public event EventHandler EventClose; 
		///<summary>With the various nested WPF and Winforms controls, I couldn't get this to focus reliably, so this is an alternative built from scratch.</summary>
		public event EventHandler EventGotODFocus;
		public event EventHandler EventMax; 
		public event EventHandler EventMin; 
		public event EventHandler EventWin; 
		public event EventHandler EventPopFloater;
		#endregion Events - Raise

		#region Properties

		///<summary>When empty, the title bar will be white/invisible.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEmpty {
			get{
				return _isEmpty;
			}
			set{
				_isEmpty=value;
				SetColors();
			}
		}

		///<summary>When selected, the title bar will be dark blue. When not, it will be pale gray.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSelected {
			get{
				return _isSelected;
			}
			set{
				_isSelected=value;
				SetColors();
			}
		}

		public string Text{
			get{
				return labelTitle.Text;
			}
			set{
				labelTitle.Text=value;
			}
		}

		public bool IsButWinPressed {
			get => _isButWinPressed;
			set {
				_isButWinPressed=value;
				SetColors();
			}
		}
		#endregion Properties

		#region Methods public

		#endregion Methods public

		#region Methods private
		private void SetColors(){
			//this is copied from FormODBase PanelBorders_Paint
			Color colorBorder;
			Color colorBorderText;
			Color colorHoverMinMax;
			Color colorFloatBase=Color.FromRgb(65, 94, 154);//This is the default dark blue-gray, same as grid titles
			Color colorHoverX=Color.FromRgb(232,17,35);;
			if(IsEmpty){
				colorBorder=Colors.White;
				colorBorderText=Colors.White;
				colorHoverMinMax=Colors.White;
				colorHoverX=Colors.White;
			}
			else if(IsSelected){
				colorBorder=ColorOD.Mix_Wpf(colorFloatBase,Colors.White,3,1);
				colorBorderText=Colors.White;
				colorHoverMinMax=ColorOD.Mix_Wpf(colorBorder,colorBorderText,4,1);
			}
			else{
				colorBorder=ColorOD.Mix_Wpf(colorFloatBase,Colors.White,1,3);
				colorBorderText=Colors.Black;
				colorHoverMinMax=ColorOD.Mix_Wpf(colorBorder,colorBorderText,10,1);
			}
			grid.Background=new SolidColorBrush(colorBorder);
			borderDrag.Background=new SolidColorBrush(colorBorder);//so it can be clickable
			if(_isHoverMax) {
				rectangleMax.Fill=new SolidColorBrush(colorHoverMinMax);
			}
			else{
				rectangleMax.Fill=new SolidColorBrush(colorBorder);
			}
			if(_isHoverMin) {
				rectangleMin.Fill=new SolidColorBrush(colorHoverMinMax);
			}
			else{
				rectangleMin.Fill=new SolidColorBrush(colorBorder);
			}
			if(IsEmpty){
				rectangleWindows.Fill=Brushes.White;
				labelWin.Foreground=Brushes.White;
			}
			else if(_isButWinPressed){
				rectangleWindows.Fill=Brushes.White;
				labelWin.Foreground=Brushes.Black;
			}
			else{//not pressed
				labelWin.Foreground=new SolidColorBrush(colorBorderText);
					//Brushes.White;
				if(_isHoverWin) {
					rectangleWindows.Fill=new SolidColorBrush(colorHoverMinMax);
				}
				else{
					rectangleWindows.Fill=new SolidColorBrush(colorBorder);
				}
			}
			if(_isHoverX) {
				rectangleClose.Fill=new SolidColorBrush(colorHoverX);
			}
			else{
				rectangleClose.Fill=new SolidColorBrush(colorBorder);
			}
			labelTitle.ColorText=colorBorderText;
			lineMin.Stroke=new SolidColorBrush(colorBorderText);
			rectangleMaxSmall.Stroke=new SolidColorBrush(colorBorderText);
			lineX.Stroke=new SolidColorBrush(colorBorderText);
			lineX2.Stroke=new SolidColorBrush(colorBorderText);
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private void BorderDrag_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			_isMouseDown=true;
			_pointMouseDownScreen=PointToScreen(e.GetPosition(this));
		}

		private void BorderDrag_MouseLeftButtonUp(object sender,MouseButtonEventArgs e) {
			_isMouseDown=false;
		}

		private void BorderDrag_MouseMove(object sender,MouseEventArgs e) {
			if(!_isMouseDown){
				return;
			}
			if(Mouse.LeftButton!=MouseButtonState.Pressed){
				return;
			}
			//we are definitely dragging
			Point pointScreen=PointToScreen(e.GetPosition(this));
			Point pointDelta=new Point(pointScreen.X-_pointMouseDownScreen.X,pointScreen.Y-_pointMouseDownScreen.Y);
			if(Math.Abs(pointDelta.X)<3 && Math.Abs(pointDelta.Y)<3){//ignore small drags.
				return;
			}
			EventPopFloater?.Invoke(this,new EventArgs());
		}

		private void Grid_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			//Yes, this also gets hit in addition to any of the other buttons that you click inside the grid.
			//This is here so that any of those clicks will grab focus.
			Point point=e.GetPosition(grid);
			if(point.X>grid.ColumnDefinitions[0].ActualWidth+grid.ColumnDefinitions[1].ActualWidth){
				return;
			}
			IsSelected=true;
			//bool success=this.Focus();
			EventGotODFocus?.Invoke(this,new EventArgs());
			SetColors();
		}

		private void RectangleClose_MouseLeave(object sender,MouseEventArgs e) {
			_isHoverX=false;
			SetColors();
		}

		private void RectangleClose_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			if(IsEmpty){
				return;
			}
			EventClose?.Invoke(this,new EventArgs());
		}

		private void RectangleClose_MouseMove(object sender,MouseEventArgs e) {
			_isHoverX=true;
			SetColors();
		}
		
		private void RectangleMax_MouseLeave(object sender,MouseEventArgs e) {
			_isHoverMax=false;
			SetColors();
		}

		private void RectangleMax_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			EventMax?.Invoke(this,new EventArgs());
		}

		private void RectangleMax_MouseMove(object sender,MouseEventArgs e) {
			_isHoverMax=true;
			SetColors();
		}
		
		private void RectangleMin_MouseLeave(object sender,MouseEventArgs e) {
			_isHoverMin=false;
			SetColors();
		}

		private void RectangleMin_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			EventMin?.Invoke(this,new EventArgs());
		}

		private void RectangleMin_MouseMove(object sender,MouseEventArgs e) {
			_isHoverMin=true;
			SetColors();
		}
		
		private void RectangleWin_MouseLeave(object sender,MouseEventArgs e) {
			_isHoverWin=false;
			SetColors();
		}

		private void RectangleWin_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			EventWin?.Invoke(this,new EventArgs());
		}

		private void RectangleWin_MouseMove(object sender,MouseEventArgs e) {
			_isHoverWin=true;
			SetColors();
		}
		#endregion Methods private EventHandlers

	}
}
