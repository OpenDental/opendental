using System;
using System.CodeDom;
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

How to use the PictureBox control:
-There are only 2 ways to add images:
	1. Add the bitmap to WpfControlsOD/Resources as follows:
		a. Do some research to find the file you need. It's probably in OpenDental/Resources, or Unversioned/Icons is also an option.
		b. Right click WpfControlsOD, Properties, Resources.
		c. On the Add Resource dropdown, Add Existing File. It will make a copy, which is what you want.
		d. Name it well.
		e. In Solution Explorer, Resources folder, find the new file. Right click, Properties, Build Action: Resource.
		f. Set the button.BitmapFileName. Example: EditPencil.gif
	2. Set it in code. This has not yet been implemented and we need a scenario in order to implement.
-Click event handlers usually look like this:
		private void pictureBox_Click(object sender,EventArgs e) { etc.

*/
	///<summary></summary>
	[DefaultEvent("Click")]
	public partial class PictureBox:UserControl{
		private string _bitmapFileName;
		private Color _colorBorder=Colors.Transparent;
		private EnumPictBoxStretch _stretch=EnumPictBoxStretch.None;

		public PictureBox(){
			InitializeComponent();
		}

		[Category("OD")]
		public event EventHandler Click;

		#region Properties
		///<summary>Example: EditPencil.gif</summary>
		[Category("OD")]
		[Description("Example: MyBitmap.jpg. This file needs to be present in WpfControlsOD/Resources/")]
		public string BitmapFileName{
			get{
				return _bitmapFileName;
			}
			set{
				_bitmapFileName=value;
				Uri uri=new Uri("pack://application:,,,/WPFControlsOD;component/Resources/"+_bitmapFileName);
				BitmapImage bitmapImage = new BitmapImage(uri);
				image.Source=bitmapImage;
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
				border.BorderBrush=new SolidColorBrush(value);
			}
		}

		[Category("OD")]
		[DefaultValue(EnumPictBoxStretch.None)]
		[Description("FillOverflow is rarely used. If aspect ratio doesn't match, FillOverflow will crop off part of the image.")]
		public EnumPictBoxStretch Stretch{
			get{
				return _stretch;
			}
			set{
				_stretch=value;
				if(_stretch==EnumPictBoxStretch.None){
					image.Stretch=System.Windows.Media.Stretch.None;
				}
				if(_stretch==EnumPictBoxStretch.Fit){
					image.Stretch=System.Windows.Media.Stretch.Uniform;
				}
				if(_stretch==EnumPictBoxStretch.FillOverflow){
					image.Stretch=System.Windows.Media.Stretch.UniformToFill;
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
		#endregion Properties

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			Click?.Invoke(this,new EventArgs());
		}
	}
	public enum EnumPictBoxStretch{
		///<summary>Default is no stretch/zoom.</summary>
		None,
		Fit,
		///<summary>If aspect ratio is different, then part of the image may be cropped off.</summary>
		FillOverflow
	}
}
