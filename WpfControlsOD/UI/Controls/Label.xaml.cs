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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the Label control:
-Height of a single row label should be 18.
-If your Text is long, it's easier to edit in XAML. You can drop the whole Text attibute down to the next line to help.
-Newlines in Text in code are handled just like normal: "\r\n"
-Newlines in Text in XAML require &#10;
-Do not use the WPF VerticalContentAlignment or HorizontalContentAlignment. They will be ignored completely.
-Instead, use our HAlign and VAlign.

*/
	///<summary></summary>
	public partial class Label : UserControl{
		private Color _colorBack=Colors.Transparent;
		private Color _colorText=Colors.Black;
		private HorizontalAlignment _hAlign=HorizontalAlignment.Left;	
		private bool _isBold=false;
		private bool _isWrap=false;
		private VerticalAlignment _vAlign=VerticalAlignment.Top;

		public Label(){
			InitializeComponent();
			//Width=75;
			//Height=18;
		}

		#region Properties
		//An empty ControlTemplate is included in the XAML
		//Its purpose is to prevent properties like HorizontalContentAlignment from affecting our layout.
		//So, many properties must be explicitly defined below.
		//This is actually working out pretty well.
		//Now we can use a color for background instead of a brush, which makes it easier.
		//And now, more properties are grouped at the bottom in the designer instead of being scattered.

		[Category("OD")]
		[DefaultValue(typeof(Color),"Transparent")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				grid.Background=new SolidColorBrush(value);
			}
		}

		[Category("OD")]
		[DefaultValue(typeof(Color),"Black")]
		public Color ColorText {
			get {
				return _colorText;
			}
			set {
				_colorText = value;
				textBlock.Foreground=new SolidColorBrush(value);
			}
		}


		[Category("OD")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment HAlign {
			get {
				return _hAlign;
			}
			set {
				_hAlign = value;
				textBlock.HorizontalAlignment=value;
				switch(value) {
					case HorizontalAlignment.Left:
						textBlock.TextAlignment=TextAlignment.Left;
						break;
					case HorizontalAlignment.Center:
						textBlock.TextAlignment=TextAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						textBlock.TextAlignment=TextAlignment.Right;
						break;
					default: //HorizontalAlignment.Stretch:
						textBlock.TextAlignment=TextAlignment.Justify;
						break;
				}
			}
		}

		[Browsable(false)]
		public InlineCollection Inlines {
			get{
				return textBlock.Inlines;
			}
			//no setter
		}

		///<summary></summary>
		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool IsBold{
			get{
				return _isBold;
			}
			set{
				_isBold=value;
				if(_isBold){
					textBlock.FontWeight=FontWeights.Bold;
				}
				else{
					textBlock.FontWeight=FontWeights.Normal;
				}
			}
		}

		///<summary></summary>
		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool IsWrap{
			get{
				return _isWrap;
			}
			set{
				_isWrap=value;
				if(_isWrap){
					textBlock.TextWrapping=TextWrapping.Wrap;
				}
				else{
					textBlock.TextWrapping=TextWrapping.NoWrap;
				}
			}
		}

		[Category("OD")]
		public string Text {
			get{
				return textBlock.Text;
			}
			set{
				textBlock.Text=value;
			}
		}

		[Category("OD")]
		[DefaultValue(VerticalAlignment.Top)]
		public VerticalAlignment VAlign {
			get {
				return _vAlign;
			}
			set {
				_vAlign = value;
				textBlock.VerticalAlignment=value;
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
	}

	/*
	public class HorizontalToTextAlignmentConverter:IValueConverter {
		public object Convert(object value,Type typeTarget,object parameter,CultureInfo culture) {
			HorizontalAlignment horizontalAlignment=(HorizontalAlignment)value;
			switch(horizontalAlignment) {
				case HorizontalAlignment.Left:
					return TextAlignment.Left;
				case HorizontalAlignment.Center:
					return TextAlignment.Center;
				case HorizontalAlignment.Right:
					return TextAlignment.Right;
				default: //HorizontalAlignment.Stretch:
					return TextAlignment.Justify;
			}
		}

		public object ConvertBack(object value,Type typeTarget,object parameter,CultureInfo culture) {
			TextAlignment textAlignment=(TextAlignment)value;
			switch(textAlignment) {
				case TextAlignment.Left:
					return HorizontalAlignment.Left;
				case TextAlignment.Right:
					return HorizontalAlignment.Right;
				case TextAlignment.Center:
					return HorizontalAlignment.Center;
				default: //Justify
					return HorizontalAlignment.Stretch;
			}
		}
	}*/
}



