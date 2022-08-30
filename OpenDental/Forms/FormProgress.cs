using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	[Obsolete("Use ProgressOD instead")]
	public partial class FormProgress : FormODBase, IProgressHandler {
		//<summary></summary>
		//public string FileName;
		///<summary>The size in MB.</summary>
		public double MaxVal;
		///<summary>Starts as 0.  Progresses to MaxVal.</summary>
		public double CurrentVal;
		///<summary>eg: ?currentVal MB of ?maxVal MB copied.  The two parameters will be replaced by numbers using the format based on NumberFormat.  If there are no parameters, then it will just display the text as is.</summary>
		public string DisplayText;
		///<summary>F for fixed.2, N to include comma, etc.</summary>
		public string NumberFormat;
		///<summary>Since only int values are allowed for progress bar, this allows you to use a double for the current and max.  The true value of the progress bar will be obtained by multiplying the double by the number here.  For example, 100 if you want to show MB like this: 3.15 MB.  The current value might be 3.1496858596859609.  This will set the currentValue of the progress bar to 315.</summary>
		public int NumberMultiplication;
		public string ErrorMessage;
		///<summary>Sets the number of milliseconds between ticks.  Default is 0.  If 0, then a value of 200 will be used.</summary>
		public int TickMS;

		public FormProgress(bool showCancelButton=true,double maxVal=100,int numberMultiplication=100,string numFormat="F") 
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			MaxVal=maxVal;
			NumberMultiplication=numberMultiplication;
			NumberFormat=numFormat;
			Lan.F(this);
			butCancel.Visible=showCancelButton;
		}

		private void FormProgress_Load(object sender, System.EventArgs e) {
			progressBar1.Maximum=(int)(MaxVal*NumberMultiplication);
			labelError.Visible=false;
			if(TickMS>0) {
				timer1.Interval=TickMS;
			}
			string progress=DisplayText.Replace("?currentVal",CurrentVal.ToString(NumberFormat));
			progress=progress.Replace("?maxVal",MaxVal.ToString(NumberFormat));
			labelProgress.Text=progress;
		}
		
		///<summary>Happens every TickMS milliseconds.  Default is 200ms.</summary>
		private void timer1_Tick(object sender, System.EventArgs e) {
			Cursor=Cursors.Default;
			if(!string.IsNullOrEmpty(ErrorMessage)) {
				labelError.Visible=true;
				labelError.Text=ErrorMessage;
				//and this form will also not close because the currentVal will never reach the maxVal.
			}
			//progress bar shows 0 maxVal size
			progressBar1.Maximum=(int)(MaxVal*NumberMultiplication);
			string progress=DisplayText.Replace("?currentVal",CurrentVal.ToString(NumberFormat));
			progress=progress.Replace("?maxVal",MaxVal.ToString(NumberFormat));
			labelProgress.Text=progress;
				//=((double)CurrentVal/1024).ToString("F")+" MB of "
				//+((double)MaxVal/1024).ToString("F")+" MB copied"; 
			if(CurrentVal<MaxVal){
				progressBar1.Value=(int)(CurrentVal*(double)NumberMultiplication);
			}
			else{
				//must be done.
				//progressBar1.Value=progressBar1.Maximum;
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Cursor=Cursors.Default;//probably not needed
			DialogResult=DialogResult.Cancel;
		}

		///<summary>Can be used if the progress is going to be updated from the business layer (not from Open Dental).</summary>
		public void UpdateProgress(double newCurVal,string newDisplayText,double newMaxVal,string errorMessage) {
			CurrentVal=newCurVal;
			DisplayText=newDisplayText;
			MaxVal=newMaxVal;
			ErrorMessage=errorMessage;
		}
		
		public void UpdateBytesRead(long numBytes) {
			CurrentVal=numBytes / (1024 * 1024.0);
		}

		public void DisplayError(string error) {
			ErrorMessage=error;
		}

		public void CloseProgress() {
			CurrentVal=MaxVal;
		}
	}

	///<summary></summary>
	public delegate void PassProgressDelegate(double newCurVal,string newDisplayText,double newMaxVal,string errorMessage);
}





















