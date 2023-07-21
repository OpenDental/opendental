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
	///<summary></summary>
	public class Panel : ItemsControl{	
		private bool _autoScroll=false;
		private Color _colorBack=Colors.White;
		private Color _colorBorder=Colors.Transparent;

		static Panel(){
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Panel), new FrameworkPropertyMetadata(typeof(Panel)));
		}

		public Panel(){
			PreviewMouseDown+=panel_PreviewMouseDown;
			//I tried lots of other things to make click work, but unsuccessful. 
			//Tried border.MouseDown, scrollViewer.MouseDown, grid.MouseDown, and preview versions of some.
			//I tried adding backgrounds to things so that click would register, but that didn't work either.
			//The way it's currently designed, it also fires when you click on any child in the panel.
			//This is probably ok because the only time we use click event is when we have a small panel with no controls.
		}

		public event EventHandler Click;

		#region Properties
		[Category("OD")]
		[Description("Used very rarely. So far, only at the bottom of InsPlanEdit window.")]
		[DefaultValue(false)]
		public bool AutoScroll {
			get{
				return _autoScroll;
			}
			set{
				_autoScroll=value;
				if(Template==null){
					return;
				}
				ScrollViewer scrollViewer = Template.FindName("scrollViewer",this) as ScrollViewer;
				if(scrollViewer!=null){
					if(value){
						scrollViewer.VerticalScrollBarVisibility=ScrollBarVisibility.Auto;
						scrollViewer.HorizontalScrollBarVisibility=ScrollBarVisibility.Auto;
					}
					else{
						scrollViewer.VerticalScrollBarVisibility=ScrollBarVisibility.Hidden;
						scrollViewer.HorizontalScrollBarVisibility=ScrollBarVisibility.Hidden;
					}
				}
			}
		}

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
			AutoScroll=_autoScroll;
			//ScrollViewer scrollViewer = Template.FindName("scrollViewer",this) as ScrollViewer;
			//if(scrollViewer!=null) {
			//	scrollViewer.MouseDown+=scrollViewer_MouseDown;
			//}
		}

		protected override Visual GetVisualChild(int index) {
			return base.GetVisualChild(index);
		}
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



