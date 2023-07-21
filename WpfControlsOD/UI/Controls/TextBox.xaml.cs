using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

How to use the TextBox control:
-Height of a single line textbox should be 20
-Newlines in Text in code are handled just like normal: "\r\n"
-Newlines in Text in XAML require &#10;
-Click event handler usually look like this:
		private void butEdit_Click(object sender,EventArgs e) { etc.
-TextChanged event handler:
		private void butEdit_TextChanged(object sender,EventArgs e) { etc.

*/
	///<summary></summary>
	public partial class TextBox : UserControl{
		private Color _colorBack=Colors.White;
		private Color _colorText=Colors.Black;
		private bool _isMultiline=false;
		private HorizontalAlignment _hAlign=HorizontalAlignment.Left;	
		private bool _readOnly;
		
		public TextBox(){
			InitializeComponent();
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.TextChanged+=TextBox_TextChanged;
			Click+=TextBox_Click;
			IsEnabledChanged+=TextBox_IsEnabledChanged;
		}

		public event EventHandler Click;
		[Category("OD")]
		[Description("Try not to use this because it will also fire when changing the value programmatically, like on load. This can cause infinite loops. One good pattern to avoid this is a class level boolean to disable the code inside this event handler during certain situations like load.")]
		public event EventHandler TextChanged;

		#region Properties
		[Category("OD")]
		[DefaultValue(typeof(Color),"White")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				textBox.Background=new SolidColorBrush(value);
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
				textBox.Foreground=new SolidColorBrush(value);
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
				textBox.HorizontalContentAlignment=_hAlign;
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

		[Category("OD")]
		[Description("This one property handles wrap and acceptsReturn.")]
		public bool IsMultiline {
			get{
				return _isMultiline;
			}
			set{
				_isMultiline=value;
				if(_isMultiline){
					textBox.TextWrapping=TextWrapping.Wrap;
					textBox.AcceptsReturn=true;
					return;
				}
				textBox.TextWrapping=TextWrapping.NoWrap;
				textBox.AcceptsReturn=false;
			}
		}

		[Category("OD")]
		[Description("Set true to prevent user from editing. Enabled false also does this, but that also grays out the text.")]
		public bool ReadOnly {
			//No need to do this for TextV... because the whole point of those is to validate input, so they would never be ReadOnly.
			get{
				return _readOnly;
			}
			set{
				//The MSWF textbox also changes backColor when changing this.  In WPF, they are independent.
				_readOnly=value;
				if(_readOnly){
					textBox.IsReadOnly=true;
					return;
				}
				textBox.IsReadOnly=false;
			}
		}

		[Browsable(false)]
		public int SelectionStart { 
			get{
				return textBox.SelectionStart;
			}
			set{
				textBox.SelectionStart=value;
			}
		}

		[Category("OD")]
		public string Text {
			get {
				return textBox.Text;
			}
			set {
				textBox.Text=value;
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

		#region Methods - public
		public void SelectAll(){
//todo: this line is failing
			bool gotFocus=textBox.Focus();
			textBox.SelectAll();
		}
		#endregion Methods - public

		#region Methods - event handlers
		private void TextBox_Click(object sender,EventArgs e) {
			Click?.Invoke(this,new EventArgs());
		}

		private void TextBox_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		private void TextBox_LostFocus(object sender,RoutedEventArgs e) {
			
		}

		private void TextBox_TextChanged(object sender,TextChangedEventArgs e) {
			TextChanged?.Invoke(this,new EventArgs());
		}
		#endregion Methods - event handlers

		#region Methods - private
		///<summary></summary>
		private void SetColors(){
			//Nothing to do here.  The textbox that this control wraps already turns gray on is own.
		}
		#endregion Methods - private
	}
}
