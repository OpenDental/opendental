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
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.TextChanged+=TextBox_TextChanged;
			canvasError.Visibility=Visibility.Hidden;
		}

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

		///<summary>You can use this instead of getting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		public int GetValue() {
			//This is a wrapper around Text.  Text is our single storage mechanism.
			if(!IsValid()){
				throw new Exception(_error);
			}
			if(!ShowZero && Text==""){
				return 0;
			}
			return Convert.ToInt32(Text);
		}

		///<summary>You can use this instead of setting Text.</summary>
		[Browsable(false)]
		public void SetValue(int value){
			if(value==0 && !ShowZero){
				Text="";//it's common to show empty string when value is zero
			}
			else{
				Text=value.ToString();
			}
			ParseValue();//to set any error
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
	}
}
