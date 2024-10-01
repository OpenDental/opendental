using System;
using System.Collections.Generic;
using System.IO;
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
	public partial class ToolBarButton : UserControl{
		//public object Tag;//already has one
		private EnumIcons _icon;
		//private string _bitmapFileName;
		///<summary>If the button is a dropdown, then this is the context menu that will show.</summary>
		public ContextMenu ContextMenuDropDown;
		///<summary>This adds a slightly darker wash and a gray border.</summary>
		private bool _isHover;
		///<summary>Only used if style is ToggleButton. Light blue wash with blue border. No hover effect when a toggle is pushed.</summary>
		public bool IsTogglePushed;
		private LinearGradientBrush linearGradientBrush=new LinearGradientBrush(Color.FromRgb(255,255,255),Color.FromRgb(191, 201, 229),angle:90);//#bfc9e5
			//Color.FromRgb(171, 181, 209),angle:90);//#ABB5D1
		private LinearGradientBrush linearGradientBrushToggled=new LinearGradientBrush(Color.FromRgb(255,255,255),Color.FromRgb(191,220,255),angle:90);
		private SolidColorBrush _solidColorBrushHoverBackground=new SolidColorBrush(Color.FromArgb(10,0,100,255));//20% blue wash
		private ToolBarButtonStyle _toolBarButtonStyle;
		private ToolTip _toolTip;
		private string _toolTipText;

		public ToolBarButton(){
			InitializeComponent();
			grid.ColumnDefinitions[2].Width=new GridLength(0);//hide the dropdown button
			polygonTriangle.Visibility=Visibility.Collapsed;
			IsEnabledChanged+=This_IsEnabledChanged;
		}

		public ToolBarButton(string text,EventHandler eventHandlerClick,EnumIcons icon=EnumIcons.None,ToolBarButtonStyle toolBarButtonStyle=ToolBarButtonStyle.NormalButton,
			string toolTipText=null,ContextMenu contextMenuDropDown=null,object tag=null)
			:this()
		{
			Text=text;
			Click+=eventHandlerClick;
			Icon=icon;
			ToolBarButtonStyle=toolBarButtonStyle;
			SetToolTipText(toolTipText);
			ContextMenuDropDown=contextMenuDropDown;
			Tag=tag;
		}

		public event EventHandler Click;

		#region Properties
		//<summary>There's no need for this. All icons should be actual icons.</summary>
		//public string BitmapFileName {
		//	get {
		//		return _bitmapFileName;
		//	}
		//	set {
		//		_bitmapFileName=value;
		//		gridImage.Children.Clear();
		//		if(string.IsNullOrEmpty(_bitmapFileName)) {
		//			SetMargins();
		//			return;
		//		}
		//		Uri uri = new Uri("pack://application:,,,/WPFControlsOD;component/Resources/"+_bitmapFileName);
		//		BitmapImage bitmapImage = new BitmapImage(uri);
		//		Image image = new Image();
		//		image.Source=bitmapImage;
		//		gridImage.Children.Add(image);
		//		gridImage.Width=22;
		//		gridImage.Height=22;
		//		SetMargins();
		//	}
		//}

		public EnumIcons Icon{
			get{
				return _icon;
			}
			set{
				_icon=value;
				gridImage.Children.Clear();
				if(_icon==EnumIcons.None){
					SetMargins();
					return;
				}
				gridImage.Width=22;
				gridImage.Height=22;
				IconLibrary.DrawWpf(_icon,gridImage);
				SetMargins();
			}
		}

		public string Text{
			get{
				return textBlock.Text;
			}
			set{
				textBlock.Text=value;
				SetMargins();
			}
		}

		public ToolBarButtonStyle ToolBarButtonStyle {
			get => _toolBarButtonStyle;
			set{
				_toolBarButtonStyle=value;
				if(_toolBarButtonStyle==ToolBarButtonStyle.DropDownButton){
					grid.ColumnDefinitions[2].Width=new GridLength(14);
					polygonTriangle.Visibility=Visibility.Visible;
				}
				else{
					grid.ColumnDefinitions[2].Width=new GridLength(0);
					polygonTriangle.Visibility=Visibility.Collapsed;
				}
			}
		}
		#endregion Properties

		public void SetBitmap(System.Drawing.Bitmap bitmap){
			using MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
			memoryStream.Position = 0;
			BitmapImage bitmapImage=new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;//makes it load into memory during EndInit
			bitmapImage.EndInit();
			bitmapImage.Freeze(); //for use in another thread
			Image image=new Image();
			image.Source=bitmapImage;
			gridImage.Children.Clear();
			gridImage.Width=22;
			gridImage.Height=22;
			image.Width=22;
			image.Height=22;
			gridImage.Children.Add(image);
			SetMargins();
		}

		public void SetToolTipText(string toolTipText){
			if(string.IsNullOrEmpty(toolTipText)){
				return;
			}
			_toolTipText=toolTipText;
			_toolTip=new ToolTip();
			_toolTip.SetControlAndAction(this,ToolTipSetString);
			_toolTip.TimeSpanDelay=TimeSpan.FromSeconds(0.3);
		}

		///<summary></summary>
		private void ToolTipSetString(FrameworkElement frameworkElement,Point point) {
			if(point.X>grid.ColumnDefinitions[0].ActualWidth+grid.ColumnDefinitions[1].ActualWidth){
				//in dropdown area
				_toolTip.SetString(this,"");
				return;
			}
			_toolTip.SetString(this,_toolTipText);
		}

		private void SetMargins(){
			if(gridImage.Children.Count==0){//no icon
				gridImage.Margin=new Thickness(0);
				if(string.IsNullOrEmpty(textBlock.Text)){//neither icon nor text is not a likely scenario
					textBlock.Margin=new Thickness(4,0,4,0);//I guess leave some space so it doesn't completely collapse
					//this is also the default in the designer
				}
				else{
					//just text
					textBlock.Margin=new Thickness(4,0,4,0);
				}
			}
			else{
				//icon present
				if(string.IsNullOrEmpty(textBlock.Text)){
					//just icon
					gridImage.Margin=new Thickness(1,0,1,0);
					textBlock.Margin=new Thickness(0);
				}
				else{
					//both icon and text
					gridImage.Margin=new Thickness(2,0,2,0);
					textBlock.Margin=new Thickness(0,0,4,0);
				}
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if(_isHover){
				_isHover=false;
				SetColors();
			}
			if(ToolBarButtonStyle==ToolBarButtonStyle.DropDownButton){
				if(e.GetPosition(this).X>=grid.ColumnDefinitions[0].ActualWidth+grid.ColumnDefinitions[1].ActualWidth){
					if(ContextMenuDropDown!=null){//there is a dropdown menu to display
						//default Placement is mouse point with no offset.
						ContextMenuDropDown.PlacementTarget=this;//relative to this button
						ContextMenuDropDown.Placement=System.Windows.Controls.Primitives.PlacementMode.Bottom;//Also aligns to left edge. No right align available. It looks good.
						ContextMenuDropDown.IsOpen=true;
					}
					return;
				}
			}
			if(ToolBarButtonStyle==ToolBarButtonStyle.ToggleButton){
				IsTogglePushed=!IsTogglePushed;
				SetColors();
				//Click will fire below
			}
			Click?.Invoke(this,new EventArgs());
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(isMouseDown){
				//regardless of whether a button is hot, nothing changes until the mouse is released.
				//a hot(pressed) button remains so, and no buttons are hot when hover,so do nothing
				return;
			}
			//mouse is not down
			if(Click is null){
				//if no click event, then we don't show any hover effect. It's just a label.
				_isHover=false;
			}
			else{
				_isHover=true;
			}
			SetColors();
		}

		///<summary>Resets button appearance. This will also deactivate the button if it has been pressed but not released. A pressed button will still be hot, however, so that if the mouse enters again, it will behave properly.  Repaints only if necessary.</summary>
		protected override void OnMouseLeave(MouseEventArgs e){
			base.OnMouseLeave(e);
			bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed;
			if(isMouseDown){
				//if a button is hot, it will remain so, even if leave.  As long as mouse is down,so do nothing.
				//Also, if a button is not hot, nothing will happen when leave,so do nothing.
				return;
			}
			//mouse is not down
			_isHover=false;
			SetColors();
		}

		private void This_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		private void SetColors(){
			grid.Background=linearGradientBrush;
			borderOverlay.BorderThickness=new Thickness(1);
			borderOverlay.BorderBrush=Brushes.Transparent;
			borderOverlay.Background=Brushes.Transparent;
			borderDropDown.BorderBrush=Brushes.Transparent;
			borderDropDown.Background=Brushes.Transparent;
			//image?
			textBlock.Foreground=Brushes.Black;
			polygonTriangle.Fill=Brushes.Black;
			if(!IsEnabled){
				textBlock.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(100));
				polygonTriangle.Fill=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(100));
				return;
			}
			if(ToolBarButtonStyle==ToolBarButtonStyle.ToggleButton && IsTogglePushed){
				//if toggled on, it will not respond to hover
				grid.Background=linearGradientBrushToggled;
				borderOverlay.BorderBrush=new SolidColorBrush(Color.FromRgb(50,120,200));
				borderOverlay.BorderThickness=new Thickness(1.5);
				return;
			}
			if(_isHover){
				borderOverlay.BorderBrush=Brushes.SlateGray;
				borderOverlay.Background=_solidColorBrushHoverBackground;
				borderDropDown.BorderBrush=Brushes.SlateGray;
				borderDropDown.Background=_solidColorBrushHoverBackground;
			}
		}
	}

	///<summary>Just like Forms.ToolBarButtonStyle, except includes some extras.</summary>
	public enum ToolBarButtonStyle{
		///<summary>A standard button</summary>
		NormalButton,
		///<summary>A button with a dropdown menu list on the right.</summary>
		DropDownButton,
		///<summary>Toggles between pushed and not pushed when clicked.</summary>
		ToggleButton,
		//<summary></summary>
		//Separator,//no need for this
		//<summary>Not clickable. Just text where a button would normally be. Can also include an image.</summary>
		//Label,//not yet implemented
		//<summary>Editable textbox that fires page nav events. Includes a label after the textbox to show total pages.</summary>
		//PageNav,//not yet implemented
	}
}
