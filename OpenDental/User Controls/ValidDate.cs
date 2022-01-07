using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class ValidDate:System.Windows.Forms.TextBox {
		private ErrorProvider errorProvider1=new ErrorProvider();
		
		///<summary></summary>
		public ValidDate(){
			InitializeComponent();
			errorProvider1.BlinkStyle=ErrorBlinkStyle.NeverBlink;
		}		

		///<summary>You can use this instead of getting and setting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime Value{
			//This is a wrapper around Text.  Text is our single storage mechanism.
			get{
				if(!IsValid()){
					throw new Exception(errorProvider1.GetError(this));
				}
				if(Text==""){
					return DateTime.MinValue;
				}
				return DateTime.Parse(this.Text);
			}
			set{
				if(value==DateTime.MinValue){
					Text="";
				}
				else{
					Text=value.ToShortDateString();
				}
				ParseValue();//to set any error
			}
		}

		///<summary>Returns true if a valid date has been entered. This replaces the older construct: if(textAbcd.errorProvider1.GetError(textAbcd)!="")</summary>
		public bool IsValid() {
			ParseValue();
			return string.IsNullOrEmpty(errorProvider1.GetError(this));
		}

		///<summary>Triggers a validate when we change the text externally.  Rarely used.</summary>
		public void Validate() {
			//ValidDate_Validating(this,new CancelEventArgs());
			ParseValue();
		}

		private void ValidDate_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			ParseValue();
		}

		private void ParseValue(){
			//string myMessage="";
			if(Text==""){
				errorProvider1.SetError(this,"");
				return;
			}
			bool allNums=true;
			for(int i=0;i<Text.Length;i++){
				if(!Char.IsNumber(Text,i)){
					allNums=false;
				}
			}
			DateTime dateTime=DateTime.MinValue;
			if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en"){
				if(allNums){
					if(Text.Length==6){
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,2);
						try{
							dateTime=DateTime.Parse(Text);
							Text=dateTime.ToShortDateString();
						}
						catch{
							errorProvider1.SetError(this,Lan.g("ValidDate","Invalid date."));
							return;
						}
					}
					else if(Text.Length==8){
						Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,4);
					}
				}
			}
			try{
				dateTime=DateTime.Parse(this.Text);
			}
			catch{
				errorProvider1.SetError(this,Lan.g("ValidDate","Invalid date."));
				return;
			}
			if(dateTime.Year<1880){
				errorProvider1.SetError(this,Lan.g("ValidDate","Valid dates between 1880 and 2100"));
				return;
			}
			if(dateTime.Year>2100){
				errorProvider1.SetError(this,Lan.g("ValidDate","Valid dates between 1880 and 2100"));
				return;
			}
			errorProvider1.SetError(this,"");
			/*
			try{
				if(DateTime.Parse(Text).Year<1880 || DateTime.Parse(Text).Year>2100) {
					throw new Exception("Valid dates between 1880 and 2100");
				}
				else {
					Text=DateTime.Parse(this.Text).ToString("d");//will throw exception if invalid
				}
				errorProvider1.SetError(this,"");
			}
			catch(Exception ex){
				//Cancel the event and select the text to be corrected by the user
				if(ex.Message=="String was not recognized as a valid DateTime."){
					myMessage="Invalid date";
				}
				else{
					myMessage=ex.Message;
				}
				//this.Select(0,this.Text.Length);
				errorProvider1.SetError(this,Lan.g("ValidDate",myMessage));
			}*/
		}

		private void ValidDate_Validated(object sender, System.EventArgs e) {
			//not used
		}

		private void ValidDate_TextChanged(object sender, System.EventArgs e) {
			/*	if(Text.Length==2 && Char.IsNumber(Text,0) && Char.IsNumber(Text,1)){
					Text+="/";
					this.SelectionStart=Text.Length+1;
				}
				if(Text.Length==5 && Char.IsNumber(Text,3) && Char.IsNumber(Text,4)){
					Text+="/";
					this.SelectionStart=Text.Length+1;
				}*/
		}

		///<summary></summary>
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if(this.ReadOnly) {
				return;
			} 
			base.OnKeyPress(e);
			//if(CultureInfo.CurrentCulture.Name=="fr-CA" || CultureInfo.CurrentCulture.Name=="en-CA") {
			//	return;//because they use - in their regular dates which interferes with this feature.
			//}
			if(e.KeyChar!='+' && e.KeyChar!='-'){
				//base.OnKeyPress (e);
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
				//base.OnKeyPress (e);
				return;
			}
			int caret=SelectionStart;
			//Only allow a user to add days to the date when the dateDisplay is less than DateTime.MaxValue to avoid UE
			if(e.KeyChar=='+' && dateDisplayed.Date<DateTime.MaxValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			//Only allow a user to subtract days to the date when the dateDisplay is greater than DateTime.MinValue to avoid UE
			if(e.KeyChar=='-' && dateDisplayed.Date>DateTime.MinValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			SelectionStart=caret;
			e.Handled=true;
		}

		///<summary></summary>
		protected override void OnKeyDown(KeyEventArgs e) {
			if(this.ReadOnly) {
				return;
			}
			base.OnKeyDown (e);
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down){
				//base.OnKeyDown (e);
				return;
			}
			DateTime dateDisplayed;
			try{
				dateDisplayed=DateTime.Parse(Text);
			}
			catch{
				//base.OnKeyDown (e);
				return;
			}
			int caret=SelectionStart;
			//Only allow a user to add days to the date when the dateDisplay is less than DateTime.MaxValue to avoid UE
			if(e.KeyCode==Keys.Up && dateDisplayed.Date<DateTime.MaxValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			//Only allow a user to subtract days to the date when the dateDisplay is greater than DateTime.MinValue to avoid UE
			if(e.KeyCode==Keys.Down && dateDisplayed.Date>DateTime.MinValue.Date){//Must compare based on date, otherwise hrs can be off
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			SelectionStart=caret;
			e.Handled=true;
		}



	}
}

//Jordan 2021-04-02 How to use: 
//Paste ValidDate onto the Form.  Do not use the similar control found in ODR.
//Name it text...  Example: textDateVisit.

//Example for ValidNum============================================================
//textWidth.Value=Something.Width;
//in butOK_Click:
/*
			if(!textDateVisit.IsValid())
				|| (test additional textboxes as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
*/
//Something.DateVisit=textDateVisit.Value


//Don't use the old style validation for new situations============================
/*
			if(textDateVisit.errorProvider1.GetError(textDateVisit)!="")
				|| (test additional textboxes as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
*/








