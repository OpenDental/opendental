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

How to use this control:
-Height of a single line box should be 20
-V stands for valid
-The old name in WinForms was ValidNum.
-name it like textVInt...  The old naming convention was just text..., so the old names need to be fixed.
-Leave a bit of space to the right for the exclamation error circle.
-Typically use Value property instead of text property, but Text is allowed.

Example:
textVIntCount.Value=count;
//in butOK_Click:
			if(!textVIntCount.IsValid())
				|| (test additional textVs as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
		
Something.Count=textVIntCount.Value

*/
	///<summary></summary>
	public partial class TextVInt : UserControl{
		private string _error;
		private HorizontalAlignment _hAlign=HorizontalAlignment.Left;	
		
		public TextVInt(){
			InitializeComponent();
			//Width=75;
			//Height=20;
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

		///<summary></summary>
		[Category("OD")]
		[Description("The maximum value that user can enter.")]
		public int MaxVal {get;set;}=100000000;//no default always specify
		
		///<summary></summary>
		[Category("OD")]
		[Description("The minimum value that user can enter.")]
		[DefaultValue(0)]
		public int MinVal {get;set; }=0;

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

		///<summary></summary>
		[Category("OD")]
		[Description("When true, a zero value will show as zero instead of blank. Also when true, a blank entry will not be allowed. Default is true.")]
		[DefaultValue(true)]
		public bool ShowZero{get;set;}=true;

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

		#region Properties - not browsable
		///<summary>You can use this instead of getting and setting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Value{
			//This is a wrapper around Text.  Text is our single storage mechanism.
			get{
				if(!IsValid()){
					throw new Exception(_error);
				}
				if(!ShowZero && Text==""){
					return 0;
				}
				return Convert.ToInt32(Text);
			}
			set{
				if(value==0 && !ShowZero){
					Text="";//it's common to show empty string when value is zero
				}
				else{
					Text=value.ToString();
				}
				ParseValue();//to set any error
			}
		}
		#endregion Properties - not browsable

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
			textBox.Focus();
		}

		private void This_PreviewMouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Click?.Invoke(this,new EventArgs());
		}
		#endregion Methods - private Event Handlers

		#region Methods - private
		private void ParseValue(){
			if(DesignerProperties.GetIsInDesignMode(this)){
				return;
			}
			if(!ShowZero && Text==""){
				SetError("");//sets no error message. Empty is OK.
				return;
			}
			int intVal=0;
			try{
				intVal=Convert.ToInt32(Text);
			}
			catch{
				SetError("Must be a number. No letters or symbols allowed");
				return;
			}
			if(intVal>MaxVal){
				SetError("Number must be less than or equal to "+MaxVal.ToString());
				return;
			}
			if(intVal<MinVal){
				SetError("Number must be greater than or equal to "+MinVal.ToString());
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
