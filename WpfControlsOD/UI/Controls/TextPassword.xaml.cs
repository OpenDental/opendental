using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
	See instructions in TextBox for nearly everything.
	This control will always show stars. If you need to not show stars, just swap it programmatically for an ordinary TextBox.
	Because it's not used much, there may be places where it's not considered properly. This will become evident during testing.
	*/
	///<summary></summary>
	public partial class TextPassword : UserControl{
		private Color _colorBack=Colors.White;
		private Color _colorText=Colors.Black;
		private bool _isChangingFocusToTextBox;
		private bool _isMultiline=false;
		private HorizontalAlignment _hAlign=HorizontalAlignment.Left;	
		private Thickness _paddingOD;
		private bool _readOnly;
		
		public TextPassword(){
			InitializeComponent();
			//Width=75;
			//Height=20;
			Focusable=true;//so that .Focus() will work, but then we manually change focus to textBox
			passwordBox.GotKeyboardFocus+=TextBox_GotKeyboardFocus;
			passwordBox.LostFocus+=TextBox_LostFocus;
			passwordBox.LostKeyboardFocus+=TextBox_LostKeyboardFocus;
			passwordBox.PasswordChanged+=TextBox_PasswordChanged;
			IsEnabledChanged+=This_IsEnabledChanged;
			GotKeyboardFocus+=This_GotKeyboardFocus;
			LostFocus+=This_LostFocus;
			LostKeyboardFocus+=This_LostKeyboardFocus;
			PreviewMouseLeftButtonDown+=This_PreviewMouseLeftButtonDown;
		}

		#region Events
		public event EventHandler Click;
		[Category("OD")]
		[Description("Try not to use this because it will also fire when changing the value programmatically, like on load. This can cause infinite loops. One good pattern to avoid this is a class level boolean to disable the code inside this event handler during certain situations like load.")]
		public event EventHandler TextChanged;
		#endregion Events

		#region Properties
		[Category("OD")]
		[DefaultValue(typeof(Color),"White")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				passwordBox.Background=new SolidColorBrush(value);
			}
		}

		/*
		[Category("OD")]
		[DefaultValue(typeof(Color),"Black")]
		public Color ColorText {
			get {
				return _colorText;
			}
			set {
				_colorText = value;
				passwordBox.Foreground=new SolidColorBrush(value);
			}
		}*/

		/*
		[Category("OD")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment HAlign {
			get {
				return _hAlign;
			}
			set {
				_hAlign = value;
				passwordBox.HorizontalContentAlignment=_hAlign;
			}
		}*/

		/*
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
		}*/

		//public bool IsMultiline {//no, not possible

		/*
		[Category("OD")]
		[Description("Default of 0 indicates no limit.")]
		[DefaultValue(0)]
		public int MaxLength { 
			get{
				return passwordBox.MaxLength;
			}
			set{
				passwordBox.MaxLength=value;
			}
		}*/

		/*
		///<summary>Sets padding on the text for odd situations. Default is 0. Can be negative.</summary>
		[Browsable(false)]
		public Thickness PaddingOD{
			get{
				return _paddingOD;
			}
			set{
				_paddingOD=value;
				passwordBox.Padding=_paddingOD;
			}
		}*/

		//public bool ReadOnly {//not possible

		//SelectedText, SelectionLength, SelectionStart not possible

		/*
		//We can't do this unless we implement it everywhere. Don't think it will be needed.
		[Category("OD")]
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			get{
				return textBox.TabIndex;
			}
			set{
				textBox.TabIndex=value;
				//InvalidateVisual();//if we want the new TabIndex value to show immediately. But there's a performance hit, so no. Also no longer relevant.
			}
		}*/

		[Category("OD")]
		public string Text {
			get {
				return passwordBox.Password;
			}
			set {
				passwordBox.Password=value;
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
		//public void Select(int start,int length){//not possible

		///<summary>Also sets focus</summary>
		public void SelectAll(){
			bool gotFocus=passwordBox.Focus();
			passwordBox.SelectAll();
		}
		#endregion Methods - public

		#region Methods - private event handlers
		private void TextBox_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//string name=Name;
			//Debug.WriteLine("GOT KBFocus: nested textBox within "+name);
			if(e.KeyboardDevice.IsKeyDown(Key.Tab)){
				SelectAll();
			}
		}

		private void TextBox_LostFocus(object sender,RoutedEventArgs e) {
			//rarely, a parent might need to subscribe to the LostFocus event, typically where Validating event was used in WinForms.
			//But we don't need to manually raise the LostFocus event for our usercontrol. 
			//When textbox loses focus, that event bubbles up through the usercontrol automatically.
		}

		private void TextBox_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//string name=Name;
			//Debug.WriteLine("LOST KBFocus: nested textBox within "+name);
			//((System.Windows.Controls.TextBox)sender).Select(0,0);
		}

		private void TextBox_PasswordChanged(object sender,RoutedEventArgs e) {
			TextChanged?.Invoke(this,new EventArgs());
		}

		private void This_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//This does fire when user clicks on textBox
			//Somehow, this is also currently firing on startup, even though I don't think I set it.
			//string name=Name;//for debugging
			if(passwordBox.IsFocused){
				//Debug.WriteLine("GOT KBFocus: "+name+". Nested textBox already has focus.");
				return;
			}
			//else{
			//	Debug.WriteLine("GOT KBFocus: "+name+". Setting focus on nested textBox.");
			//}
			_isChangingFocusToTextBox=true;
			bool isFocused=passwordBox.Focus();
			//unfortunately, the above line causes a LostFocus to fire. We try to intercept and cancel that down in This_LostFocus.
			//if(isFocused){
			//	Debug.WriteLine("Setting focus was successful.");
			////}
			//else{
			//	Debug.WriteLine("Setting focus failed.");
			//}
		}
		
		private void This_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		private void This_LostFocus(object sender,RoutedEventArgs e) {
			if(_isChangingFocusToTextBox) {
				e.Handled=true;
				_isChangingFocusToTextBox=false;
			}
		}

		private void This_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//string name=Name;
			//Debug.WriteLine("LOST KBFocus: "+name);
		}

		private void This_PreviewMouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Click?.Invoke(this,new EventArgs());
		}
		#endregion Methods - private event handlers

		#region Methods - private
		///<summary></summary>
		private void SetColors(){
			//Nothing to do here.  The textbox that this control wraps already turns gray on is own.
		}
		#endregion Methods - private
	}
}
