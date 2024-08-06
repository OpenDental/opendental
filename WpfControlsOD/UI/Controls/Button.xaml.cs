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
-Name these all like but... because we already used that naming convention historically.
-There are only 2 ways to add images:
	1. Set Icon if EnumIcon has the one you need.
	2. (will soon be deprecated) If there is no Icon, then add the bitmap (usually a png) to WpfControlsOD/Resources as follows:
		a. Look in Solution Explorer, Resources folder. If the file you need is there, set the button.BitmapFileName. Example: EditPencil.gif. You are done.
		b. If you need to add a file, do some research to find the file you need. It's probably in OpenDental/Resources, or Unversioned/Icons is also an option.
		c. Right click WpfControlsOD, Properties, Resources.
		d. On the Add Resource dropdown, Add Existing File. It will make a copy, which is what you want.
		e. In Solution Explorer, Resources folder, find the new file. Right click, Properties, Build Action: Resource.
		f. Set the button.BitmapFileName. Example: EditPencil.gif
	3. WPF doesn't use anything like the WF ImageList, so all of those are going away during the conversion to WPF. Image index is not an option.
	4. Someday, we could add option to set any in-memory image, although buttons probably don't need that feature.
-Keyboard shortcuts for buttons
	Create an event handler in your frm constructor:
	PreviewKeyDown+=Frm_PreviewKeyDown;
	Then:
	private void Frm_PreviewKeyDown(object sender,KeyEventArgs e) {
		if(butSave.IsAltKey(Key.S,e)){//automatically handles visible, enbled, etc.
			butSave_Click(this,new EventArgs());
		}
	}
	In addition to the code above, also include an underscore inside the button text so that the user knows about the keyboard shortcut.
	An underscore prefix is used in WPF instead of the & that's used in WinForms.
-Click event handlers usually look like this:
	private void butEdit_Click(object sender,EventArgs e) { etc.

	https://wpf-tutorial.com/basic-controls/the-textblock-control-inline-formatting/#google_vignette

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
		private Thickness _thicknessMarginImage=new Thickness(3,0,3,0);
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
		[Description("Example: editPencil.gif")]
		public string BitmapFileName{
			get{
				return _bitmapFileName;
			}
			set{
				_bitmapFileName=value;
				gridImage.Children.Clear();
				if(string.IsNullOrEmpty(_bitmapFileName)) {
					gridImage.Margin=new Thickness(0);
					return;
				}
				Uri uri=new Uri("pack://application:,,,/WPFControlsOD;component/Resources/"+_bitmapFileName);
				BitmapImage bitmapImage;
				//This can fail for a few bitmaps for a very small number of computers for unknown reasons
				try{
					bitmapImage = new BitmapImage(uri);
				}
				catch{
					bitmapImage = new BitmapImage();
				}
				Image image=new Image();
				image.Source=bitmapImage;
				gridImage.Children.Add(image);
				gridImage.Width=22;
				gridImage.Height=22;
				gridImage.Margin=_thicknessMarginImage;
			}
		}

		[Category("OD")]
		public EnumIcons Icon{
			get{
				return _icon;
			}
			set{
				_icon=value;
				gridImage.Children.Clear();
				if(_icon==EnumIcons.None){
					gridImage.Margin=new Thickness(0);
					return;
				}
				gridImage.Width=22;
				gridImage.Height=22;
				IconLibrary.DrawWpf(_icon,gridImage);
				gridImage.Margin=_thicknessMarginImage;
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
					System.Windows.Controls.Grid.SetColumn(gridImage,0);
					System.Windows.Controls.Grid.SetColumn(accessText,1);
					System.Windows.Controls.Grid.SetColumn(textBlock,1);
					gridImage.HorizontalAlignment=HorizontalAlignment.Left;
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
					System.Windows.Controls.Grid.SetColumn(gridImage,1);
					System.Windows.Controls.Grid.SetColumn(accessText,0);
					System.Windows.Controls.Grid.SetColumn(textBlock,0);
					gridImage.HorizontalAlignment=HorizontalAlignment.Left;
				}
				if(_imageAlign==EnumImageAlign.Center){
					grid.ColumnDefinitions.Clear();
					//Just one cell. Not designed for text, so text would just overlap image.
					System.Windows.Controls.Grid.SetColumn(gridImage,0);
					System.Windows.Controls.Grid.SetColumn(accessText,0);
					System.Windows.Controls.Grid.SetColumn(textBlock,0);
					gridImage.HorizontalAlignment=HorizontalAlignment.Center;
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

		///<summary>Default is 3,0,3,0</summary>
		[Category("OD")]
		[DefaultValue(typeof(Thickness), "3,0,3,0")]
		[Description("Default is 3,0,3,0")]
		public Thickness MarginImageOverride{
			get{
				return _thicknessMarginImage;
			}
			set{
				_thicknessMarginImage=value;
				gridImage.Margin=_thicknessMarginImage;
			}
		}

		[Category("OD")]
		public string Text{
			get{
				return accessText.Text;
			}
			set{
				accessText.Text=value;
				if(!DesignerProperties.GetIsInDesignMode(this)){
					return;
				}
				//design mode from here down.
				//Need to show the underscore.
				//accessText is still the sole structure where the text is stored, even when we set it not visible.
				accessText.Visibility=Visibility.Collapsed;
				if(!value.Contains("_")){
					textBlock.Text=value;
					return;
				}
				textBlock.Inlines.Clear();
				int idx=value.IndexOf("_");
				if(idx==value.Length-1){//example 0123_, idx=4 which =5-1
					textBlock.Text=value.Substring(0,value.Length-1);
					return;
				}
				//example 01_34
				if(idx>0){
					textBlock.Inlines.Add(value.Substring(0,idx));//example 01
				}
				Run run=new Run();
				run.Text=value.Substring(idx+1,1);//example 3
				run.TextDecorations=TextDecorations.Underline;
				textBlock.Inlines.Add(run);
				if(value.Length>idx+2){//example 5>4
					textBlock.Inlines.Add(value.Substring(idx+2,value.Length-idx-2));//example substring(4,5-2-2)=(4,1) =4
				}
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

		///<summary></summary>
		public bool IsAltKey(Key key,KeyEventArgs e){
			if(Keyboard.Modifiers!=ModifierKeys.Alt) {
				return false;
			}
			if(key!=e.SystemKey){//Use e.SystemKey instead of e.Key because the presence of the Alt modifier causes it to be interpreted as a system command.
				return false;
			}
			if(!Visible){
				return false;
			}
			if(!IsEnabled){
				return false;
			}
			DependencyObject dependencyObject=Parent;
			GroupBox groupBoxParent=new GroupBox();
			try {
				groupBoxParent=(GroupBox)dependencyObject;
			}
			catch {//The button is not in a GroupBox, so the last check is irrelevant
				return true;
			}
			if(!groupBoxParent.Visible) {
				return false;
			}
			return true;
		}

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
				textBlock.Foreground=Brushes.Black;
			}
			else{
				accessText.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
				textBlock.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
			}
		}

	}

	public enum EnumImageAlign{
		Left,
		Right,
		Center
	}

}
