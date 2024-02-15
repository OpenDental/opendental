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
How to use PanelAutoScroll:
Not allowed to use any alignment except Left,Top, or controls can end up outside the container.

*/
	///<summary>Warning, do not use this except at the bottom of the InsPlanEdit window.</summary>
	public class PanelAutoScroll : ItemsControl{	
		private Color _colorBack=Colors.White;
		private Color _colorBorder=Colors.Transparent;

		static PanelAutoScroll(){
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelAutoScroll), new FrameworkPropertyMetadata(typeof(PanelAutoScroll)));
		}

		public PanelAutoScroll(){
			//Panels should not have default sizes because they frequently need to stretch to fill a SplitContainer, TabControl, etc.
			//Width=200;
			//Height=200;
			PreviewMouseDown+=panel_PreviewMouseDown;
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
	}
}



