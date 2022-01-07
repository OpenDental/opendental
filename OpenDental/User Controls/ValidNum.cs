using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental{
///<summary>See usage notes at bottom of this file.</summary>
	public class ValidNum : TextBox{
		private System.ComponentModel.Container components = null;
		private ErrorProvider errorProvider1=new ErrorProvider();

		///<summary></summary>
		public ValidNum(){
			InitializeComponent();
			errorProvider1.BlinkStyle=ErrorBlinkStyle.NeverBlink;
  	}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ValidNum
			// 
			this.Validated += new System.EventHandler(this.ValidNum_Validated);
			this.Validating += new System.ComponentModel.CancelEventHandler(this.ValidNum_Validating);
			this.ResumeLayout(false);

		}
		#endregion

		///<summary></summary>
		[Category("OD")]
		[Description("The minimum value that user can enter.")]
		[DefaultValue(0)]
		public int MinVal{get;set;}=0;

		///<summary></summary>
		[Category("OD")]
		[Description("The maximum value that user can enter.")]
		[DefaultValue(255)]
		public int MaxVal{get;set;}=255;

		///<summary></summary>
		[Category("OD")]
		[Description("When true, a zero value will show as zero instead of blank.")]
		[DefaultValue(true)]
		public bool ShowZero{get;set;}=true;

		///<summary>You can use this instead of getting and setting Text.  Will throw exception if you try to Get when IsValid=false.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Value{
			//This is a wrapper around Text.  Text is our single storage mechanism.
			get{
				if(!IsValid()){
					throw new Exception(errorProvider1.GetError(this));
				}
				if(!ShowZero && Text==""){
					return 0;
				}
				return Convert.ToInt32(Text);
			}
			set{
				if(value==0 && !ShowZero){
					Text="";
				}
				else{
					Text=value.ToString();
				}
				ParseValue();//to set any error
			}
		}

		///<summary>Returns true if a valid number has been entered. This replaces the older construct: if(textAbcd.errorProvider1.GetError(textAbcd)!="")</summary>
		public bool IsValid() {
			ParseValue();
			return string.IsNullOrEmpty(errorProvider1.GetError(this));
		}

		private void ValidNum_Validating(object sender, CancelEventArgs e) {
			//Warning.  This will not get hit if you never click into and then out of the box.
			//So we also parse when setting value and when checking IsValid.
			ParseValue();
		}

		private void ValidNum_Validated(object sender, System.EventArgs e) {			
			//not used
		}

		private void ParseValue(){
			if(DesignMode){
				return;
			}
			if(!ShowZero && Text==""){
				errorProvider1.SetError(this,"");//sets no error message. Empty is OK.
				return;
			}
			int intVal=0;
			try{
				intVal=Convert.ToInt32(this.Text);
			}
			catch{
				errorProvider1.SetError(this,"Must be a number. No letters or symbols allowed");
				return;
			}
			if(intVal>MaxVal){
				errorProvider1.SetError(this,"Number must be less than or equal to "+MaxVal.ToString());
				return;
			}
			if(intVal<MinVal){
				errorProvider1.SetError(this,"Number must be greater than or equal to "+MinVal.ToString());
				return;
			}
			errorProvider1.SetError(this,"");
		}

		


	}
}

//Jordan 2021-03-31 How to use: 
//Decide which Valid... to use.
//ValidNum: For integer fields.
//ValidDouble: for numbers with decimals, usually currency.  
//ValidDate: For dates.
//ValidTime, ValidPhone: I have not yet reviewed or overhauled.
//Paste the chosen control onto the Form.  Do not use the similar controls found in ODR.
//In designer, set MaxVal and MinVal. Be very generous with the range.
//Name it text...  Example: textLength.

//Example for ValidNum============================================================
//textWidth.Value=Something.Width;
//in butOK_Click:
/*
			if(!textWidth.IsValid())
				|| (test additional textboxes as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
*/
//Something.Width=textWidth.Value


//Don't use the old style validation for new situations============================
/*
			if(textLength.errorProvider1.GetError(textLength)!="")
				|| (test additional textboxes as needed) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
*/


