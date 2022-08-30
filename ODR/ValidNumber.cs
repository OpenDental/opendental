using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CodeBase;

namespace ODR{
///<summary>This differs slightly from ValidNum. Use this to allow a blank entry instead of defaulting to 0.
///</summary>
	public class ValidNumber : System.Windows.Forms.TextBox{
		private System.ComponentModel.Container components = null;
		public ErrorProvider errorProvider1=new ErrorProvider();
		///<summary></summary>
		private int maxVal=255;
		///<summary></summary>
		private int minVal=0;

		///<summary></summary>
		public ValidNumber(){
			InitializeComponent();
		}

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
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
			// ValidNumber
			// 
			this.Validating += new System.ComponentModel.CancelEventHandler(this.ValidNumber_Validating);
			this.ResumeLayout(false);

		}
		#endregion

		///<summary></summary>
		[Category("Data"),
			Description("The maximum value that user can enter.")
		]
		public int MaxVal{
			get{ 
				return maxVal; 
			}
			set{ 
				maxVal=value;
			}
		}

		///<summary></summary>
		[Category("Data"),
			Description("The minimum value that user can enter.")
		]
		public int MinVal{
			get{ 
				return minVal; 
			}
			set{ 
				minVal=value;
			}
		}
		
		private void ValidNumber_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			string myMessage="";
			if(Text==""){
				errorProvider1.SetError(this,myMessage);//sets no error message. (empty is OK)
				return;
			}
			try{
				if(System.Convert.ToInt32(this.Text)>MaxVal)
					throw new Exception("Number must be less than "+(MaxVal+1).ToString());
				if(System.Convert.ToInt32(this.Text)<MinVal)
					throw new Exception("Number must be greater than or equal to "+(MinVal).ToString());
				errorProvider1.SetError(this,"");
			}
			catch(Exception ex){
				if(ex.Message=="Input string was not in a correct format."){
					myMessage="Must be a number. No letters or symbols allowed";
				}
				else{
					myMessage=ex.Message;
				}
			}
			errorProvider1.SetError(this,myMessage);
		}

	}
}
