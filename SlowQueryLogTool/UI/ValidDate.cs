using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;

namespace SlowQueryLog {
	///<summary></summary>
	public class ValidDate:System.Windows.Forms.TextBox {
		public ErrorProvider errorProvider1=new ErrorProvider();
		private System.ComponentModel.Container components = null;

		///<summary>Returns true if a valid number has been entered.</summary>
		public bool IsValid {
			get {
				return string.IsNullOrEmpty(errorProvider1.GetError(this));
			}
		}

		///<summary></summary>
		public ValidDate() {
			InitializeComponent();
			errorProvider1.BlinkStyle=ErrorBlinkStyle.NeverBlink;
		}

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components!=null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// ValidDate
			// 
			this.Validated+=new System.EventHandler(this.ValidDate_Validated);
			this.Validating+=new System.ComponentModel.CancelEventHandler(this.ValidDate_Validating);
			this.TextChanged+=new System.EventHandler(this.ValidDate_TextChanged);
			this.ResumeLayout(false);

		}
		#endregion

		private void ValidDate_Validating(object sender,System.ComponentModel.CancelEventArgs e) {
			string myMessage = "";
			try {
				if(Text=="") {
					errorProvider1.SetError(this,"");
					return;
				}
				bool allNums = true;
				for(int i = 0;i<Text.Length;i++) {
					if(!Char.IsNumber(Text,i)) {
						allNums=false;
					}
				}
				if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en") {
					if(allNums) {
						if(Text.Length==6) {
							Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,2);
						}
						else if(Text.Length==8) {
							Text=Text.Substring(0,2)+"/"+Text.Substring(2,2)+"/"+Text.Substring(4,4);
						}
					}
				}
				if(DateTime.Parse(Text).Year<1880||DateTime.Parse(Text).Year>2100) {
					throw new Exception("Valid dates between 1880 and 2100");
				}
				else {
					Text=DateTime.Parse(this.Text).ToString("d");//will throw exception if invalid
				}
				errorProvider1.SetError(this,"");
			}
			catch(Exception ex) {
				//Cancel the event and select the text to be corrected by the user
				if(ex.Message=="String was not recognized as a valid DateTime.") {
					myMessage="Invalid date";
				}
				else {
					myMessage=ex.Message;
				}
				//this.Select(0,this.Text.Length);
				errorProvider1.SetError(this,myMessage);
			}
		}

		private void ValidDate_Validated(object sender,System.EventArgs e) {

		}

		private void ValidDate_TextChanged(object sender,System.EventArgs e) {
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
			if(e.KeyChar!='+'&&e.KeyChar!='-') {
				//base.OnKeyPress (e);
				return;
			}
			//The user might not be done typing in the date.  Make sure that there are at least two non-numeric characters before subtracting days.
			Regex regEx = new Regex("[^0-9]");
			if(regEx.Matches(Text).Count<2) {
				return;//Not a complete date yet.
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(Text);
			}
			catch {
				//base.OnKeyPress (e);
				return;
			}
			int caret = SelectionStart;
			if(e.KeyChar=='+') {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyChar=='-') {
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
			base.OnKeyDown(e);
			if(e.KeyCode!=Keys.Up&&e.KeyCode!=Keys.Down) {
				//base.OnKeyDown (e);
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(Text);
			}
			catch {
				//base.OnKeyDown (e);
				return;
			}
			int caret = SelectionStart;
			if(e.KeyCode==Keys.Up) {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyCode==Keys.Down) {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			Text=dateDisplayed.ToShortDateString();
			SelectionStart=caret;
			e.Handled=true;
		}

		public void SetError(string error) {
			errorProvider1.SetError(this,error);
		}



	}
}










