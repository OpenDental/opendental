using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

-This is a rarely used control.
-See the other TextV... controls for documentation.

*/
	///<summary></summary>
	public partial class TextVTime : UserControl{
		private string _error;
		
		public TextVTime(){
			InitializeComponent();
			Focusable=true;//so that .Focus() will work, but then we manually change focus to textBox
			textBox.GotKeyboardFocus+=TextBox_GotKeyboardFocus;
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.LostKeyboardFocus+=TextBox_LostKeyboardFocus;
			textBox.TextChanged+=TextBox_TextChanged;
			GotKeyboardFocus+=This_GotKeyboardFocus;
			PreviewMouseLeftButtonDown+=This_PreviewMouseLeftButtonDown;
			canvasError.Visibility=Visibility.Hidden;
		}

		#region Events
		public event EventHandler Click;
		[Category("OD")]
		[Description("Try not to use this because it will also fire when changing the value programmatically, like on load. This can cause infinite loops.")]
		public event EventHandler TextChanged;
		#endregion Events

		#region Properties
		///<summary>This hides the one that would otherwise be attached to canvasMain.</summary>
		public new HorizontalAlignment HorizontalContentAlignment {
			get {
				return textBox.HorizontalContentAlignment;
			}
			set {
				textBox.HorizontalContentAlignment=value;
			}
		}

		[Category("OD")]
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			get{
				return textBox.TabIndex;
			}
			set{
				textBox.TabIndex=value;
			}
		}

		//ReadOnly. No. Input validators should never need this.

		///<summary></summary>
		[Category("OD")]
		[Description("")]
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
		///<summary>Returns true if a valid date has been entered. This replaces the older construct: if(textAbcd.errorProvider1.GetError(textAbcd)!="")</summary>
		public bool IsValid() {
			ParseValue();
			return string.IsNullOrEmpty(_error);
		}
		
		public void SelectAll(){
			bool gotFocus=textBox.Focus();
			textBox.SelectAll();
		}

		///<summary>Triggers a validate when we change the text externally.  Rarely used.</summary>
		public void Validate() {
			ParseValue();
		}
		#endregion Methods - public

		#region Methods - private Event Handlers
		private void TextBox_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			if(e.KeyboardDevice.IsKeyDown(Key.Tab)){
				((System.Windows.Controls.TextBox)sender).SelectAll();
			}
		}

		private void TextBox_LostFocus(object sender,RoutedEventArgs e) {
			ParseValue();
			//in the rare case that we need to externally use this event, typically where Validating even was used in WinForms.
			RaiseEvent(new RoutedEventArgs(e.RoutedEvent));
		}

		private void TextBox_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//((System.Windows.Controls.TextBox)sender).Select(0,0);
		}

		private void TextBox_TextChanged(object sender,TextChangedEventArgs e) {
			if(!string.IsNullOrEmpty(_error)) {
				ParseValue();//It's nice to get rid of the error as soon as the user fixes it.
			}
			TextChanged?.Invoke(this,new EventArgs());
		}

		private void This_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			if(textBox.IsFocused){
				return;
			}
			bool isFocused=textBox.Focus();
		}

		private void This_PreviewMouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Click?.Invoke(this,new EventArgs());
		}
		#endregion Methods - private Event Handlers

		#region Methods - private
		///<summary>Set to false to only possibly clear an error without interfering with user typing.</summary>
		private void ParseValue(){
			if(Text=="") {
				SetError("");
				return;
			}
			DateTime dateTime = DateTime.MinValue;
			try {
				dateTime=DateTime.Parse(this.Text);
			}
			catch {
				SetError("Invalid time.");
				return;
			}
			SetError("");
		}

		private void SetError(string error){
			_error=error;
			if(string.IsNullOrEmpty(error)) {
				canvasError.Visibility=Visibility.Hidden;
			}
			else {
				canvasError.Visibility=Visibility.Visible;
				canvasError.ToolTip=error;
			}
		}
		#endregion Methods - private
	}
}
