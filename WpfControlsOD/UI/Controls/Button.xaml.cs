using System;
using System.Collections.Generic;
using System.ComponentModel;
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
/*
Jordan is the only one allowed to edit this file.

How to use the Button control:
-All buttons should be 24 DIPs tall, just like WinForms. Bitmap images are usually 22x22.
-Generally leave about 10-20% whitespace on left and right of text for foreign language translation.
-There are only 2 ways to add images:
	1. Set Icon if EnumIcon has the one you need.
	2. If there is no Icon, then add the bitmap (usually a png) to WpfControlsOD/Resources as follows:
		a. Do some research to find the file you need. It's probably in OpenDental/Resources, or Unversioned/Icons is also an option.
		b. Right click WpfControlsOD, Properties, Resources.
		c. On the Add Resource dropdown, Add Existing File. It will make a copy, which is what you want.
		d. In Solution Explorer, Resources folder, find the new file. Right click, Properties, Build Action: Resource.
		e. Set the button.BitmapFileName. Example: EditPencil.gif
	3. WPF doesn't use anything like the WF ImageList, so all of those are going away during the conversion to WPF. Image index is not an option.
-Keyboard shortcuts for buttons are discussed in FrmODBase. For example Enter for OK or Alt-P for print.
-Click event handlers usually look like this:
		private void butEdit_Click(object sender,EventArgs e) { etc.

*/
	///<summary></summary>
	[DefaultEvent("Click")]
	public partial class Button : UserControl{
		//public object Tag;//already has one
		private EnumIcons _icon;
		private string _bitmapFileName;
		private EnumImageAlign _imageAlign=EnumImageAlign.Left;
		private bool _isEnabled=true;
		private bool _isHover;
		private SolidColorBrush _solidColorBrushHoverBackground=new SolidColorBrush(Color.FromArgb(10,0,100,255));//20% blue wash
		//public static RoutedCommand MyRoutedCommand=new RoutedCommand();
		//this didn't work because AccessText is not focusable, although we could make it focusable.
		//But even then, this button would also have to have focus before keystrokes would register.  That's completely unworkable.
		//So, I had to move the event handler up to the window level.

		//static Button(){
			//It works to set width and height either here or in ctor. Ctor is easier for usercontrols.
			//WidthProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(75.0));
			//HeightProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(24.0));
		//}

		public Button(){
			InitializeComponent();
			//Width=75;
			//Height=24;
			Focusable=false;
			//CommandBinding commandBinding = new CommandBinding(MyRoutedCommand,MyRoutedCommandExecuted,MyRoutedCommandCanExecute);
			//CommandBindings.Add(commandBinding);
			IsEnabledChanged+=Button_IsEnabledChanged;
		}

		[Category("OD")]
		public event EventHandler Click;

		#region Properties
		///<summary>Example: EditPencil.gif</summary>
		[Category("OD")]
		[Description("Example: EditPencil.gif")]
		public string BitmapFileName{
			get{
				return _bitmapFileName;
			}
			set{
				_bitmapFileName=value;
				//if(_icon==EnumIcons.None){
				//	image.Margin=new Thickness(0);
				//	return;
				//}
				Uri uri=new Uri("pack://application:,,,/WPFControlsOD;component/Resources/"+_bitmapFileName);
				BitmapImage bitmapImage = new BitmapImage(uri);
				image.Source=bitmapImage;
				image.Width=bitmapImage.Width;
				image.Height=bitmapImage.Height;
				image.Margin=new Thickness(3,0,3,0);
			}
		}

		[Category("OD")]
		public EnumIcons Icon{
			get{
				return _icon;
			}
			set{
				_icon=value;
				if(_icon==EnumIcons.None){
					image.Margin=new Thickness(0);
					return;
				}
				BitmapImage bitmapImage=UI.IconLibrary.DrawWpf(_icon);
				image.Source=bitmapImage;
				int width=IconLibrary.Width(_icon);
				image.Width=width;
				image.Height=width;
				image.Margin=new Thickness(3,0,3,0);
			}
		}

		[Category("OD")]
		[DefaultValue(EnumImageAlign.Left)]
		public EnumImageAlign ImageAlign{
			get{
				return _imageAlign;
			}
			set{
				_imageAlign=value;
				if(_imageAlign==EnumImageAlign.Left){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//image
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(image,0);
					System.Windows.Controls.Grid.SetColumn(accessText,1);
					image.VerticalAlignment=VerticalAlignment.Center;
					accessText.VerticalAlignment=VerticalAlignment.Center;
				}
				if(_imageAlign==EnumImageAlign.Right){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//image
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(image,1);
					System.Windows.Controls.Grid.SetColumn(accessText,0);
					image.VerticalAlignment=VerticalAlignment.Center;
					accessText.VerticalAlignment=VerticalAlignment.Center;
				}
			}
		}

		[Category("OD")]
		[DefaultValue(true)]
		public new bool IsEnabled{
			//This doesn't actually ever get hit. 
			//It's just here to move IsEnabled down into the OD category.
			get{
				return base.IsEnabled;
			}
			set{
				base.IsEnabled=value;
			}
		}

		//[Category("OD")]
		//[DefaultValue(true)]
		//public bool Enabled{
		//	get{
		//		return _isEnabled;
		//	}
		//	set{
		//		_isEnabled=value;
		//		SetColors();
		//	}
		//}

		[Category("OD")]
		public string Text{
			get{
				return accessText.Text;
			}
			set{
				accessText.Text=value;
				//InputBindings.Clear();
				//if(string.IsNullOrEmpty(value)) {
				//	return;
				//}
				//int idxUnderscore = value.IndexOf('_');
				//if(idxUnderscore==-1 || idxUnderscore==value.Length-1) {//if it's the last char, then we can use it
				//	return;
				//}
				//string strChar = value.Substring(idxUnderscore+1,1);
				//KeyConverter keyConverter = new KeyConverter();
				//Key key;
				//try {
				//	key=(Key)keyConverter.ConvertFromString(strChar);
				//}
				//catch {
				//	return;
				//}
				//InputBinding inputBinding = new InputBinding(MyRoutedCommand,new KeyGesture(key,ModifierKeys.Alt));
				//InputBindings.Add(inputBinding);
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
		#endregion Properties

		//private void MyRoutedCommandCanExecute(object sender,CanExecuteRoutedEventArgs e) {
		//	e.CanExecute = true;
		//}

		//private void MyRoutedCommandExecuted(object sender,ExecutedRoutedEventArgs e) {
		//	Click?.Invoke(this,new EventArgs());
		//	//RaiseEvent(new RoutedEventArgs(ClickEvent));
		//}

		private void Button_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if(!IsEnabled){
				return;
			}
			Click?.Invoke(this,new EventArgs());
			//RaiseEvent(new RoutedEventArgs(ClickEvent));
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!IsEnabled){
				return;
			}
			_isHover=true;
			SetColors();
		}

		///<summary>Resets button appearance. This will also deactivate the button if it has been pressed but not released. A pressed button will still be hot, however, so that if the mouse enters again, it will behave properly.  Repaints only if necessary.</summary>
		protected override void OnMouseLeave(MouseEventArgs e){
			base.OnMouseLeave(e);
			_isHover=false;
			SetColors();
		}

		private void SetColors(){
			if(_isHover){
				borderHover.Background=_solidColorBrushHoverBackground;
			}
			else{
				borderHover.Background=Brushes.Transparent;
			}
			if(IsEnabled){
				accessText.Foreground=Brushes.Black;//not hit very often. Usually black because of default.
			}
			else{
				accessText.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
			}
		}

	}

	public enum EnumImageAlign{
		Left,
		Right
	}

}
