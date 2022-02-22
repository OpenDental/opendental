using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental{
///<summary>See usage notes in ValidNum.</summary>
	public class ValidDouble:System.Windows.Forms.TextBox {
		///<summary></summary>
		[Category("OD")]
		[Description("The maximum value that user can enter.")]
		public Double MaxVal {get;set;}=100000000;
		
		///<summary></summary>
		[Category("OD")]
		[Description("The minimum value that user can enter.")]
		public Double MinVal {get;set; }=-100000000;
			
		private ErrorProvider errorProvider1=new ErrorProvider();

		///<summary>You can use this instead of getting and setting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double Value{
			//This is a wrapper around Text.  Text is our single storage mechanism.
			get{
				if(!IsValid()){
					throw new Exception(errorProvider1.GetError(this));
				}
				if(Text=="" && MinVal==0.01){
					//In the 5 places where minVal is set to 1 cent, the Value property is not used.
					//But we need to return something valid.
					return 0.01;
				}
				if(Text==""){
					return 0;
				}
				return Convert.ToDouble(Text);
			}
			set{
				Text=value.ToString();
				ParseValue();//to set any error
			}
		}

		///<summary>True if the text entered is a valid double. This replaces the older construct: if(textAbcd.errorProvider1.GetError(textAbcd)!="")</summary>
		public bool IsValid() {
			ParseValue();
			return string.IsNullOrEmpty(errorProvider1.GetError(this));
		}

		///<summary></summary>
		public ValidDouble(){
			InitializeComponent();
			errorProvider1.BlinkStyle=ErrorBlinkStyle.NeverBlink;
		}

		#region Component Designer generated code

		private void InitializeComponent(){
			this.SuspendLayout();
			// 
			// ValidDouble
			// 
			this.Validating += new System.ComponentModel.CancelEventHandler(this.ValidNum_Validating);
			this.ResumeLayout(false);

		}
		#endregion

		private void ValidNum_Validating(object sender, CancelEventArgs e) {
			//Warning.  This will not get hit if you never click into and then out of the box.
			//So we also parse when setting value and when checking IsValid.
			ParseValue();
		}

		private void ParseValue() {
			string myMessage="";
			if(Text=="" && MinVal==0.01) {
				//We do use 1 cent in about 5 windows and we allow it to be blank
				errorProvider1.SetError(this,"");
				return;
			}
			if(Text=="") {
				//Implied zero, which is usually allowed unless that's outside the range.
				if(0<MinVal || 0>MaxVal)	{
					errorProvider1.SetError(this,"Zero or blank is not allowed.");
					return;
				}
				errorProvider1.SetError(this,"");
				return;
			}
			try {
				if(System.Convert.ToDouble(this.Text)>MaxVal){
					throw new Exception("Number must be less than or equal to "+MaxVal.ToString());
				}
				if(System.Convert.ToDouble(this.Text)<MinVal){
					throw new Exception("Number must be greater than or equal to "+(MinVal).ToString());
				}
				errorProvider1.SetError(this,"");
			}
			catch(Exception ex) {
				if(ex.Message=="Input string was not in a correct format.") {
					myMessage="Must be a number. No letters or symbols allowed";
				}
				else {
					myMessage=ex.Message;
				}
				errorProvider1.SetError(this,myMessage);
			}			
		}

	

	}
}

//Example for ValidDouble=========================================================
//textAmount.Text=something.Amount.ToString("f2");
//In butOK_Click:
/*
			if(!textAmount.IsValid())
				|| (test additional textboxes as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
*/
//something.Amount=PIn.Double(textAmount.Text);
