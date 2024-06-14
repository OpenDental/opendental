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
		#endregion Fields - Private

		#region Constructor
		public ImageDockHeader(){
			InitializeComponent();
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
		#endregion Constructor

		#region Events - Raise
		public event EventHandler Close; 
		public event EventHandler Max; 
		public event EventHandler Min; 
		public event EventHandler Win; 
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
			if(_isEmpty){
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
			if(_isEmpty){
				rectangleWindows.Fill=Brushes.White;
				labelWin.Foreground=Brushes.White;
			}
			else if(_isButWinPressed){
				rectangleWindows.Fill=Brushes.White;
				labelWin.Foreground=Brushes.Black;
			}
			else{//not pressed
				labelWin.Foreground=Brushes.White;
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
			labelWin.ColorText=colorBorderText;
			lineMin.Stroke=new SolidColorBrush(colorBorderText);
			rectangleMaxSmall.Stroke=new SolidColorBrush(colorBorderText);
			lineX.Stroke=new SolidColorBrush(colorBorderText);
			lineX2.Stroke=new SolidColorBrush(colorBorderText);
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private void Grid_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Point point=e.GetPosition(grid);
			if(point.X>grid.ColumnDefinitions[0].ActualWidth+grid.ColumnDefinitions[1].ActualWidth){
				return;
			}
			IsSelected=true;//This gets done again in 
			SetColors();
		}

		private void RectangleClose_MouseLeave(object sender,MouseEventArgs e) {
			_isHoverX=false;
			SetColors();
		}

		private void RectangleClose_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			if(_isEmpty){
				return;
			}
			Close?.Invoke(this,new EventArgs());
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
			Max?.Invoke(this,new EventArgs());
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
			Min?.Invoke(this,new EventArgs());
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
			Win?.Invoke(this,new EventArgs());
		}

		private void RectangleWin_MouseMove(object sender,MouseEventArgs e) {
			_isHoverWin=true;
			SetColors();
		}
		#endregion Methods private EventHandlers

	}
}
