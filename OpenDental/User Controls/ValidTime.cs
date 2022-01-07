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
	public class ValidTime:System.Windows.Forms.TextBox {
		private ErrorProvider _errorProv=new ErrorProvider();
		private System.ComponentModel.Container components = null;

		///<summary>Returns true if a valid time has been entered.</summary>
		public bool IsValid() {
			return _errorProv.GetError(this)=="";
		}

		/// <summary>Defines the IsShortTimeString property for ValidTime component. Defaults to false. During validation this will be called and a true
		/// value will result in a string formatted as '10:05 PM' for en-us. A false value will result in a string formatted as '10:05:30 PM' 
		/// for en-us</summary>
		[Category("Behavior"),Description("Formats validated Time in ValidTime textbox to ShortTimeString for true and LongTimeString for false.")]
		[DefaultValue(false)]
		public bool IsShortTimeString { get; set; }

		///<summary></summary>
		public ValidTime(){
			InitializeComponent();
			_errorProv.BlinkStyle=ErrorBlinkStyle.NeverBlink;
			Size=new Size(120,20);
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

		private void InitializeComponent(){
			this.SuspendLayout();
			// 
			// ValidDate
			// 
			this.Validating += new System.ComponentModel.CancelEventHandler(this.ValidTime_Validating);
			this.ResumeLayout(false);

		}
		#endregion

		private void ValidTime_Validating(object sender, CancelEventArgs e) {
			string myMessage="";
			try{
				if(Text==""){
					_errorProv.SetError(this,"");
					return;
				}
				if(IsShortTimeString) {
					Text=DateTime.Parse(Text).ToShortTimeString();//Formats string as '10:05 PM'. Will throw exception if invalid.
				}
				else {
					Text=DateTime.Parse(Text).ToLongTimeString();//Formats string as '10:05:30 PM'. Will throw exception if invalid.
				}
				_errorProv.SetError(this,"");
			}
			catch(Exception ex){
				//Cancel the event and select the text to be corrected by the user
				if(ex.Message=="String was not recognized as a valid time."){
					myMessage="Invalid time";
				}
				else{
					myMessage=ex.Message;
				}
				_errorProv.SetError(this,Lan.g("ValidTime",myMessage));
			}
		}

		///<summary>Gets rid of the orange exlamation circle.</summary>
		public void ClearError() {
			_errorProv.SetError(this,"");
		}


	}
}










