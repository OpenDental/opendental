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

How to use this control:
-Height of a single line box should be 20
-V stands for valid
-name it like textVDate...  The old naming convention was just text..., so the old names need to be fixed.
-Leave a bit of space to the right for the exclamation error circle.
-Typically use Value property instead of text property, but Text is allowed.

Example:
textVDateStart.Value=dateStart;
//in butOK_Click:
			if(!textVDateStart.IsValid())
				|| (test additional textVs as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
		
Something.DateStart=textVDateStart.Value

*/
	///<summary></summary>
	public partial class TextVDate : UserControl{
		private string _error;
		
		public TextVDate(){
			InitializeComponent();
			Focusable=true;//so that .Focus() will work, but then we manually change focus to textBox
			textBox.GotKeyboardFocus+=TextBox_GotKeyboardFocus;
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.LostKeyboardFocus+=TextBox_LostKeyboardFocus;
			textBox.PreviewKeyDown+=TextBox_PreviewKeyDown;
			textBox.PreviewTextInput+=TextBox_PreviewTextInput;
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

		#region Properties - not browsable
		///<summary>You can use this instead of getting and setting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime Value{
			//This is a wrapper around Text.  Text is our single storage mechanism.
			get{
				if(!IsValid()) {
					throw new Exception(_error);
				}
				if(Text=="") {
					return DateTime.MinValue;
				}
				return DateTime.Parse(this.Text);
			}
			set{
				if(value==DateTime.MinValue) {
					Text="";
				}
				else {
					Text=value.ToShortDateString();
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

		private void TextBox_PreviewKeyDown(object sender,KeyEventArgs e) {
			//if(IsReadOnly) {
			//	return;
			//}
			if(e.Key!=Key.Up && e.Key!=Key.Down){
				return;
			}
			DateTime dateDisplayed;
			try{
				dateDisplayed=DateTime.Parse(Text);
			}
			catch{
				return;
			}
			int caret=textBox.SelectionStart;
			//Only allow a user to add days to the date when the dateDisplay is less than DateTime.MaxValue to avoid UE
			if(e.Key==Key.Up && dateDisplayed.Date<DateTime.MaxValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			//Only allow a user to subtract days to the date when the dateDisplay is greater than DateTime.MinValue to avoid UE
			if(e.Key==Key.Down && dateDisplayed.Date>DateTime.MinValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			textBox.SelectionStart=caret;
			e.Handled=true;
		}

		private void TextBox_PreviewTextInput(object sender,TextCompositionEventArgs e) {
			//if(this.ReadOnly) {
			//	return;
			//} 
			if(e.Text!="+" && e.Text!="-"){
				return;
			}
			//The user might not be done typing in the date.  Make sure that there are at least two non-numeric characters before subtracting days.
			Regex regEx=new Regex("[^0-9]");
			if(regEx.Matches(Text).Count < 2) {
				return;//Not a complete date yet.
			}
			DateTime dateDisplayed;
			try{
				dateDisplayed=DateTime.Parse(Text);
			}
			catch{
				return;
			}
			int caret=textBox.SelectionStart;
			//Only allow a user to add days to the date when the dateDisplay is less than DateTime.MaxValue to avoid UE
			if(e.Text=="+" && dateDisplayed.Date<DateTime.MaxValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			//Only allow a user to subtract days to the date when the dateDisplay is greater than DateTime.MinValue to avoid UE
			if(e.Text=="-" && dateDisplayed.Date>DateTime.MinValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			textBox.SelectionStart=caret;
			e.Handled=true;
		}

		private void TextBox_TextChanged(object sender,TextChangedEventArgs e) {
			if(!string.IsNullOrEmpty(_error)) {
				ParseValue(doFixes:false);//It's nice to get rid of the error as soon as the user fixes it.
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
		private void ParseValue(bool doFixes=true){
			if(Text=="") {
				SetError("");
				return;
			}
			bool allNums = true;
			for(int i = 0;i<Text.Length;i++) {
				if(!Char.IsNumber(Text,i)) {
					allNums=false;
				}
			}
			DateTime dateTime = DateTime.MinValue;
			if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en") {
				if(allNums && doFixes) {
					if(Text.Length==4) {
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2);
					}
					else if(Text.Length==6) {
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,2);
					}
					else if(Text.Length==8) {
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,4);
					}
				}
			}
			try {
				dateTime=DateTime.Parse(this.Text);
			}
			catch {
				SetError("Invalid date.");
				return;
			}
			if(doFixes){
				Text=dateTime.ToString("d");// allows for year completion if data entered in format MM/DD 
			}
			if(dateTime.Year<1880) {
				SetError("Valid dates between 1880 and 2100");
				return;
			}
			if(dateTime.Year>2100) {
				SetError("Valid dates between 1880 and 2100");
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
