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
	public partial class ToolBarButton : UserControl{
		//public object Tag;//already has one
		private EnumIcons _icon;
		private string _bitmapFileName;

		private ToolBarButtonState _toolBarButtonState;
		//private LinearGradientBrush _linearGradientBrushMain;
		private SolidColorBrush _solidColorBrushHoverBackground=new SolidColorBrush(Color.FromArgb(10,0,100,255));//20% blue wash

		public ToolBarButton(){
			InitializeComponent();
			//Color colorStart=Color.FromRgb(255,255,255);
			//Color colorEnd=Color.FromRgb(225,232,235);
			//_linearGradientBrushMain=new LinearGradientBrush(colorStart,colorEnd,90);
			//_solidColorBrushHoverBackground
		}

		public event EventHandler Click;

		#region Properties
		public string BitmapFileName{
			get{
				return _bitmapFileName;
			}
			set{
				_bitmapFileName=value;
				Uri uri=new Uri("pack://application:,,,/WPFControlsOD;component/Resources/"+_bitmapFileName);
				BitmapImage bitmapImage;
				//This can fail for a few bitmaps for a very small number of computers for unknown reasons
				try {
					bitmapImage = new BitmapImage(uri);
				}
				catch {
					bitmapImage=new BitmapImage();
				}
				//BitmapImage bitmapImage = new BitmapImage();
				//bitmapImage.BeginInit();
				//bitmapImage.UriSource = new Uri("/Resources/"+_bitmapFileName,UriKind.Relative);
				//bitmapImage.EndInit();
				//ImageSource imageSource = new BitmapImage(new Uri("/Resources/editPencil.gif",UriKind.Relative));
				image.Source=bitmapImage;
			}
		}

		public EnumIcons Icon{
			get{
				return _icon;
			}
			set{
				_icon=value;
				BitmapImage bitmapImage=UI.IconLibrary.DrawWpf(_icon);
				image.Source=bitmapImage;
			}
		}

		public string Text{
			get{
				return textBlock.Text;
			}
			set{
				textBlock.Text=value;
			}
		}

		public string ToolTipText{
			get{
				if(this.ToolTip is ToolTip toolTip){
					return toolTip.Content.ToString();
				}
				return null;
			}
			set{
				ToolTip toolTip=new ToolTip();
				toolTip.Content=value;
				this.ToolTip=toolTip;
			}
		}
		#endregion Properties

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			_toolBarButtonState=ToolBarButtonState.Normal;
			SetColors();
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
			_toolBarButtonState=ToolBarButtonState.Hover;
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
			_toolBarButtonState=ToolBarButtonState.Normal;
			SetColors();
		}

		private void SetColors(){
			if(_toolBarButtonState==ToolBarButtonState.Hover){
				border.BorderBrush=Brushes.SlateGray;
				border.Background=_solidColorBrushHoverBackground;
			}
			else{
				border.BorderBrush=Brushes.Transparent;
				border.Background=Brushes.Transparent;
			}
		}

	}

	///<summary>IsTogglePushed, Enabled, and isRed are handled separately</summary>
	public enum ToolBarButtonState{
		///<summary>0.</summary>
		Normal,
		///<summary>Mouse is hovering over the button and the mouse button is not pressed.</summary>
		Hover,
		///<summary>Mouse was pressed over this button and is still down, even if it has moved off this button or off the toolbar.</summary>
		Pressed,
		///<summary>In a dropdown button, only the dropdown portion is pressed. For hover, the entire button acts as one, but for pressing, the dropdown can be pressed separately.</summary>
		DropPressed
	}

	///<summary>Just like Forms.ToolBarButtonStyle, except includes some extras.</summary>
	public enum ToolBarButtonStyle{
		///<summary>A standard button</summary>
		NormalButton,
		///<summary>A button with a dropdown menu list on the right.</summary>
		DropDownButton,
		///<summary></summary>
		Separator,
		///<summary>Toggles between pushed and not pushed when clicked on.</summary>
		ToggleButton,
		///<summary>Not clickable. Just text where a button would normally be. Can also include an image.</summary>
		Label,
		///<summary>Editable textbox that fires page nav events. Includes a label after the textbox to show total pages.</summary>
		PageNav,
	}
}
