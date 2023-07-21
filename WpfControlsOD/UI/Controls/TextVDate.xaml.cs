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
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.TextChanged+=TextBox_TextChanged;
			canvasError.Visibility=Visibility.Hidden;
		}

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

		///<summary>You can use this instead of getting and setting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		public DateTime Value {
			//This is a wrapper around Text.  Text is our single storage mechanism.
			get {
				if(!IsValid()) {
					throw new Exception(_error);
				}
				if(Text=="") {
					return DateTime.MinValue;
				}
				return DateTime.Parse(this.Text);
			}
			set {
				if(value==DateTime.MinValue) {
					Text="";
				}
				else {
					Text=value.ToShortDateString();
				}
				ParseValue();//to set any error
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

		///<summary>Returns true if a valid date has been entered. This replaces the older construct: if(textAbcd.errorProvider1.GetError(textAbcd)!="")</summary>
		public bool IsValid() {
			ParseValue();
			return string.IsNullOrEmpty(_error);
		}

		///<summary>Triggers a validate when we change the text externally.  Rarely used.</summary>
		public void Validate() {
			ParseValue();
		}

		private void TextBox_LostFocus(object sender,RoutedEventArgs e) {
			ParseValue();
		}

		private void TextBox_TextChanged(object sender,TextChangedEventArgs e) {
			if(!string.IsNullOrEmpty(_error)) {
				ParseValue();//It's nice to get rid of the error as soon as the user fixes it.
			}
		}

		private void ParseValue(){
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
				if(allNums) {
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
				Text=DateTime.Parse(this.Text).ToString("d");// allows for year completion if data entered in format MM/DD 
				dateTime=DateTime.Parse(this.Text);
			}
			catch {
				SetError("Invalid date.");
				return;
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
	}
}
