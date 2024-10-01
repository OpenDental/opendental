using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.
How to use panel control:


*/
	///<summary></summary>
	public class Panel : ItemsControl{	
		private Color _colorBack=Colors.White;
		private Color _colorBorder=Colors.Transparent;

		static Panel(){
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Panel), new FrameworkPropertyMetadata(typeof(Panel)));
			//WidthProperty.OverrideMetadata(typeof(Panel), new FrameworkPropertyMetadata(200.0));
			//HeightProperty.OverrideMetadata(typeof(Panel), new FrameworkPropertyMetadata(200.0));
		}

		public Panel(){
			//Panels should not have default sizes because they frequently need to stretch to fill a SplitContainer, TabControl, etc.
			//Width=200;
			//Height=200;
			PreviewMouseDown+=panel_PreviewMouseDown;
			//I tried lots of other things to make click work, but unsuccessful. 
			//Tried border.MouseDown, scrollViewer.MouseDown, grid.MouseDown, and preview versions of some.
			//I tried adding backgrounds to things so that click would register, but that didn't work either.
			//The way it's currently designed, it also fires when you click on any child in the panel.
			//This is probably ok because the only time we use click event is when we have a small panel with no controls.
			KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);
			Focusable=false;
		}

		public event EventHandler Click;

		#region Properties
		[Category("OD")]
		[DefaultValue(typeof(Color),"#FFFFFFFF")]//white
		[Description("Default is White.")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				if(Template==null){
					return;
				}
				Border border = Template.FindName("border",this) as Border;
				if(border!=null){
					border.Background=new SolidColorBrush(value);
				}
			}
		}

		[Category("OD")]
		[DefaultValue(typeof(Color),"#00FFFFFF")]//transparent
		[Description("Recommend DarkGray as a good natural border color.")]
		public Color ColorBorder {
			get {
				return _colorBorder;
			}
			set {
				_colorBorder = value;
				if(Template==null){
					return;
				}
				Border border = Template.FindName("border",this) as Border;
				if(border!=null){
					border.BorderBrush=new SolidColorBrush(value);
				}
			}
		}

		[Category("OD")]
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			//For now, this is just to move it down into the OD category,
			//but later, there are plans to enhance it.
			//Because TabIndex is an Advanced Property, we had to give it a new name to keep it out of Advanced Property area.
			get{
				return TabIndex;
			}
			set{
				TabIndex=value;
				//InvalidateVisual();//if we want the new TabIndex value to show immediately. But there's a performance hit, so no.
			}
		}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}

		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount;
			}
		}
		#endregion Properties

		private void panel_PreviewMouseDown(object sender,MouseButtonEventArgs e) {
			Click?.Invoke(this,new EventArgs());
		}

		//private void scrollViewer_MouseDown(object sender,MouseButtonEventArgs e) {
		//	Click?.Invoke(this,new EventArgs());
		//}
		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			//Border border = Template.FindName("border",this) as Border;
			//border.Background=new SolidColorBrush(_colorBack);
			//border.BorderBrush=new SolidColorBrush(_colorBorder);
			ColorBack=_colorBack;
			ColorBorder=_colorBorder;
			//ScrollViewer scrollViewer = Template.FindName("scrollViewer",this) as ScrollViewer;
			//if(scrollViewer!=null) {
			//	scrollViewer.MouseDown+=scrollViewer_MouseDown;
			//}
		}

		protected override Visual GetVisualChild(int index) {
			return base.GetVisualChild(index);
		}

		/*
		protected override Size MeasureOverride(Size sizeAvailable) {
			Size desiredSize = new Size();
			for(int i=0;i<base.VisualChildrenCount;i++){
				Visual visualChild=base.GetVisualChild(i);
				UIElement uIElement=(UIElement)visualChild;
				uIElement.Measure(sizeAvailable);
			}
			desiredSize=new Size(ActualWidth,ActualHeight);
			return desiredSize;
		}

		protected override Size ArrangeOverride(Size sizeFinal) {
			Border border=VisualTreeHelper.GetChild(this, 0) as Border;
			if(border is null){
				return sizeFinal;
			}
			ScrollViewer scrollViewer=VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
			if(scrollViewer is null){
				return sizeFinal;
			}
			ItemsPresenter itemsPresenter = VisualTreeHelper.GetChild(scrollViewer, 0) as ItemsPresenter;
			if(itemsPresenter is null){
				return sizeFinal;
			}
			//ItemsPanelTemplate:
			System.Windows.Controls.Panel itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0) as System.Windows.Controls.Panel;
			if(itemsPanel is null){
				return sizeFinal;
			}
			System.Windows.Controls.Panel itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0) as System.Windows.Controls.Panel;
			if(itemsPanel is null){
				return sizeFinal;
			}

			foreach (UIElement child in itemsPanel.Children)
			{



			for(int i=0;i<base.VisualChildrenCount;i++){
				Visual visualChild=base.GetVisualChild(i);
				FrameworkElement frameworkElement=(FrameworkElement)visualChild;
				Thickness thicknessMargin=frameworkElement.Margin;
				double x=20;//thicknessMargin.Left;
				double y=thicknessMargin.Top;
				double width=frameworkElement.Width;
				if(double.IsNaN(width)){
					width=frameworkElement.DesiredSize.Width;
				}
				double height=frameworkElement.Height;
				if(double.IsNaN(height)){
					height=frameworkElement.DesiredSize.Height;
				}
				if(frameworkElement.HorizontalAlignment==HorizontalAlignment.Right){
					x=//sizeFinal.Width
						20;
						//-width-thicknessMargin.Right;
				}
				if(frameworkElement.HorizontalAlignment==HorizontalAlignment.Stretch){
					width=sizeFinal.Width-thicknessMargin.Left-thicknessMargin.Right;
				}
				if(frameworkElement.VerticalAlignment==VerticalAlignment.Bottom){
					y=sizeFinal.Height-height-thicknessMargin.Bottom;
				}
				if(frameworkElement.VerticalAlignment==VerticalAlignment.Stretch){
					height=sizeFinal.Height-thicknessMargin.Top-thicknessMargin.Bottom;
				}
				width = Math.Max(0, width);//ensure not negative
				height = Math.Max(0, height);
				Rect rectFinal=new Rect(x,y,width,height);
				frameworkElement.Arrange(rectFinal);
			}
			return sizeFinal;
		}*/
	}

/*	
	public class AutoScrollConverter:IValueConverter {
		public object Convert(object value,Type typeTarget,object parameter,CultureInfo culture) {
			bool isAutoScroll=(bool)value;
			if(isAutoScroll){
				return ScrollBarVisibility.Auto;
			}
			return ScrollBarVisibility.Hidden;
		}

		public object ConvertBack(object value,Type typeTarget,object parameter,CultureInfo culture) {
			ScrollBarVisibility scrollBarVisibility=(ScrollBarVisibility)value;
			if(scrollBarVisibility==ScrollBarVisibility.Auto){
				return true;
			}
			return false;
		}
	}*/

	/*
	public class ColorBrushConverter:IValueConverter {
		public object Convert(object value,Type typeTarget,object parameter,CultureInfo culture) {
			Color color=(Color)value;
			if(color==Colors.Transparent){
				return null;
			}
			return new SolidColorBrush(color);
		}

		public object ConvertBack(object value,Type typeTarget,object parameter,CultureInfo culture) {
			if(value is null){
				return Colors.Transparent;
			}
			SolidColorBrush solidColorBrush=(SolidColorBrush)value;
			return solidColorBrush.Color;
		}
	}*/
}



