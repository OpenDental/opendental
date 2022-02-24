using System;
using System.Text;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormTerminal.
	/// </summary>
	public class FormTerminalConnection : FormODBase,ITerminalConnector
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textTx;
		private System.Windows.Forms.TextBox textRx;
		private System.Windows.Forms.TextBox textProgress;
		private System.Windows.Forms.Label label3;

		private TerminalConnector termConnector;

		/// <summary></summary>
		public FormTerminalConnection()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			termConnector=new TerminalConnector();
			termConnector.TextReceivedEvent+=TermConnector_TextReceivedEvent;
		}

		private void TermConnector_TextReceivedEvent(object sender,TextReceivedEventArgs e) {
			textRx.Text+=e.TextReceived;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTerminalConnection));
			this.label1 = new System.Windows.Forms.Label();
			this.textTx = new System.Windows.Forms.TextBox();
			this.textRx = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textProgress = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25,0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100,20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Sent";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textTx
			// 
			this.textTx.Location = new System.Drawing.Point(25,22);
			this.textTx.Multiline = true;
			this.textTx.Name = "textTx";
			this.textTx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textTx.Size = new System.Drawing.Size(452,28);
			this.textTx.TabIndex = 1;
			// 
			// textRx
			// 
			this.textRx.Location = new System.Drawing.Point(25,83);
			this.textRx.Multiline = true;
			this.textRx.Name = "textRx";
			this.textRx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textRx.Size = new System.Drawing.Size(452,41);
			this.textRx.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(25,61);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100,20);
			this.label2.TabIndex = 2;
			this.label2.Text = "Received";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textProgress
			// 
			this.textProgress.Location = new System.Drawing.Point(25,157);
			this.textProgress.Multiline = true;
			this.textProgress.Name = "textProgress";
			this.textProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textProgress.Size = new System.Drawing.Size(452,418);
			this.textProgress.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(25,135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100,20);
			this.label3.TabIndex = 5;
			this.label3.Text = "Progress";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormTerminal
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(637,612);
			this.Controls.Add(this.textProgress);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textRx);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTx);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTerminal";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Modem Terminal";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormTerminal_Closing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public void ShowForm() {
			Show();
		}

		/// <summary></summary>
		public void OpenConnection(int port){
			termConnector.OpenConnection(port);
			textProgress.Text+="Modem Connection Opened\r\n";
		}

		/// <summary></summary>
		public void CloseConnection(){
			textProgress.Text+="Modem Connection Closed\r\n";
			ScrollProgress();
			termConnector.CloseConnection();
		}

		/// <summary></summary>
		public void Dial(string phone){
			termConnector.Dial(phone);
			string str="ATDT"+phone+"\r\n";
			textTx.Text+=str;
			textProgress.Text+="Dialed: "+phone+"\r\n";
		}

		/// <summary></summary>
		public void Send(string str){
			termConnector.Send(str);
			str+="\r\n";
			textTx.Text+=str;
			textProgress.Text+="Sent: "+str;
			ScrollProgress();
		}

		///<summary></summary>
		public void Send(byte[] block){
			termConnector.Send(block);
			textTx.Text+="**block**\r\n";
			textProgress.Text+="Sent block\r\n";
			ScrollProgress();
		}

		///<summary></summary>
		public void Send(char singleChar){
			termConnector.Send(singleChar);
			if(termConnector.IsSpecialCode(singleChar)){
				textTx.Text+=termConnector.DisplaySpecialCode(singleChar)+"\r\n";
				textProgress.Text+="Sent "+termConnector.DisplaySpecialCode(singleChar)+"\r\n";
			}
			else{
				textTx.Text+=singleChar.ToString()+"\r\n";
				textProgress.Text+="Sent char "+singleChar.ToString()+"\r\n";
			}
			ScrollProgress();
		}

		///<summary>Throws an exception if expected text not received within timeout period.</summary>
		public string WaitFor(string expectedText,double timeoutMS){
			textProgress.Text+="Waiting for '"+expectedText+"'\r\n";
			ScrollProgress();
			string receivedText=termConnector.WaitFor(expectedText,timeoutMS);
			textProgress.Text+="Receieved: "+receivedText+"'\r\n";
			ScrollProgress();
			return receivedText;
		}

		///<summary>Throws an exception if expected text not received within timeout period. Returns the response received out of the given expectedReplies.</summary>
		public string WaitFor(string expectedText1,string expectedText2,double timeoutMS){
			textProgress.Text+="Waiting for '"+expectedText1
				+"' or '"+expectedText2+"'\r\n";
			ScrollProgress();
			string receivedText=termConnector.WaitFor(expectedText1,expectedText2,timeoutMS);
			textProgress.Text+="Receieved: "+receivedText+"'\r\n";
			ScrollProgress();
			return receivedText;
		}

		///<summary>Throws an exception if expected byte not received within timeout period.</summary>
		public void WaitFor(byte expectedByte,double timeoutMS){
			textProgress.Text+="Waiting for byte "+expectedByte.ToString()
				+"\r\n";
			ScrollProgress();
			DateTime startTime=DateTime.Now;
			byte receivedByte=termConnector.WaitFor(expectedByte,timeoutMS);
			if(termConnector.IsSpecialCode((char)receivedByte)) {
				textProgress.Text+="Receieved expected byte: "
					+termConnector.DisplaySpecialCode((char)receivedByte)+"'\r\n";
			}
			else {
				textProgress.Text+="Receieved expected byte: "+receivedByte.ToString()+"'\r\n";
			}
		}

		///<summary>Gets a single byte from the RxBuff. Throws exception if no byte received within the given time. Does not clear this byte from the RxBuff in any way.  That's up to the calling code.</summary>
		public byte GetOneByte(double timeoutMS){
			byte receivedByte=termConnector.GetOneByte(timeoutMS);
			if(termConnector.IsSpecialCode((char)receivedByte)) {
				textProgress.Text+="Receieved: byte "
					+termConnector.DisplaySpecialCode((char)receivedByte)+"\r\n";
			}
			else {
				textProgress.Text+="Receieved: byte "+(char)receivedByte+"\r\n";
			}
			ScrollProgress();
			return receivedByte;
		}

		///<summary>Gets a precise number of bytes from RxBuff. Throws an error if not received in time.  Also, make sure to clear RxBuff before and after using this function.</summary>
		public byte[] GetBytes(int numberOfBytes, double timeoutMS){
			byte[] receivedBytes=termConnector.GetBytes(numberOfBytes,timeoutMS);
			textProgress.Text+="Receieved block\r\n";
			ScrollProgress();
			return receivedBytes;
		}

		///<summary></summary>
		public void ClearRxBuff(){
			termConnector.ClearRxBuff();
		}

		///<summary>Receives all text within the given timespan.</summary>
		public string Receive(double timeoutMS){
			textProgress.Text+="Receiving...\r\n";
			ScrollProgress();
			string textReceived=termConnector.Receive(timeoutMS);
			textProgress.Text+="Receieved: "+textReceived+"'\r\n";
			ScrollProgress();
			return textReceived;
		}

		private void ScrollProgress(){
			textProgress.SelectionStart=textProgress.Text.Length;
			textProgress.ScrollToCaret();
		}
		
		///<summary></summary>
		public void Pause(double ms){
			textProgress.Text+="Pausing for "+ms.ToString()+" ms\r\n";
			ScrollProgress();
			termConnector.Pause(ms);
			textProgress.Text+="Done pausing\r\n";
			ScrollProgress();
		}

		///<summary>Transmits a file using Xmodem protocol.</summary>
		public void UploadXmodem(string filePath){
			textProgress.Text+="Sending "+filePath+" by Xmodem\r\n";
			ScrollProgress();
			termConnector.UploadXmodem(filePath);
			textProgress.Text+="Sending "+filePath+" by Xmodem complete\r\n";
			ScrollProgress();
		}

		///<summary>Receives a file using Xmodem protocol.</summary>
		public void DownloadXmodem(string filePath){
			textProgress.Text+="Retrieving "+filePath+" by Xmodem\r\n";
			ScrollProgress();
			termConnector.DownloadXmodem(filePath);
			textProgress.Text+="Retrieving "+filePath+" by Xmodem complete\r\n";
			ScrollProgress();
		}
		
		///<summary>Converts any string to an acceptable format for modem ASCII. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		public string Sout(string intputStr,int maxL,int minL){
			return termConnector.Sout(intputStr,maxL,minL);			
		}

		///<summary>Converts any string to an acceptable format for modem ASCII. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		public string Sout(string str,int maxL){
			return Sout(str,maxL,-1);
		}

		///<summary>Converts any string to an acceptable format for modem ASCII. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		public string Sout(string str){
			return Sout(str,-1,-1);
		}

		private void FormTerminal_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			CloseConnection();
		}


	}
}

















