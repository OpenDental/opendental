using System;
using System.Text;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormTerminal.
	/// </summary>
	public partial class FormTerminalConnection : FormODBase,ITerminalConnector {
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

















