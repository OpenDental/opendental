using System;
using System.Text;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormTerminalConnection : FormODBase,ITerminalConnector {
		private TerminalConnector _terminalConnector;

		/// <summary></summary>
		public FormTerminalConnection()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			_terminalConnector=new TerminalConnector();
			_terminalConnector.TextReceivedEvent+=TermConnector_TextReceivedEvent;
		}

		private void TermConnector_TextReceivedEvent(object sender,TextReceivedEventArgs e) {
			textRx.Text+=e.TextReceived;
		}

		public void ShowForm() {
			Show();
		}

		/// <summary></summary>
		public void OpenConnection(int port){
			_terminalConnector.OpenConnection(port);
			textProgress.Text+="Modem Connection Opened\r\n";
		}

		/// <summary></summary>
		public void CloseConnection(){
			textProgress.Text+="Modem Connection Closed\r\n";
			ScrollProgress();
			_terminalConnector.CloseConnection();
		}

		/// <summary></summary>
		public void Dial(string phone){
			_terminalConnector.Dial(phone);
			string str="ATDT"+phone+"\r\n";
			textTx.Text+=str;
			textProgress.Text+="Dialed: "+phone+"\r\n";
		}

		/// <summary></summary>
		public void Send(string str){
			_terminalConnector.Send(str);
			str+="\r\n";
			textTx.Text+=str;
			textProgress.Text+="Sent: "+str;
			ScrollProgress();
		}

		///<summary></summary>
		public void Send(byte[] byteArrayBlock){
			_terminalConnector.Send(byteArrayBlock);
			textTx.Text+="**block**\r\n";
			textProgress.Text+="Sent block\r\n";
			ScrollProgress();
		}

		///<summary></summary>
		public void Send(char charSingle){
			_terminalConnector.Send(charSingle);
			if(_terminalConnector.IsSpecialCode(charSingle)){
				textTx.Text+=_terminalConnector.DisplaySpecialCode(charSingle)+"\r\n";
				textProgress.Text+="Sent "+_terminalConnector.DisplaySpecialCode(charSingle)+"\r\n";
			}
			else{
				textTx.Text+=charSingle.ToString()+"\r\n";
				textProgress.Text+="Sent char "+charSingle.ToString()+"\r\n";
			}
			ScrollProgress();
		}

		///<summary>Throws an exception if expected text not received within timeout period.</summary>
		public string WaitFor(string expectedText,double timeoutMS){
			textProgress.Text+="Waiting for '"+expectedText+"'\r\n";
			ScrollProgress();
			string textReceived=_terminalConnector.WaitFor(expectedText,timeoutMS);
			textProgress.Text+="Receieved: "+textReceived+"'\r\n";
			ScrollProgress();
			return textReceived;
		}

		///<summary>Throws an exception if expected text not received within timeout period. Returns the response received out of the given expectedReplies.</summary>
		public string WaitFor(string expectedText1,string expectedText2,double timeoutMS){
			textProgress.Text+="Waiting for '"+expectedText1+"' or '"+expectedText2+"'\r\n";
			ScrollProgress();
			string textReceived=_terminalConnector.WaitFor(expectedText1,expectedText2,timeoutMS);
			textProgress.Text+="Receieved: "+textReceived+"'\r\n";
			ScrollProgress();
			return textReceived;
		}

		///<summary>Throws an exception if expected byte not received within timeout period.</summary>
		public void WaitFor(byte byteExpected,double timeoutMS){
			textProgress.Text+="Waiting for byte "+byteExpected.ToString()+"\r\n";
			ScrollProgress();
			byte byteReceived=_terminalConnector.WaitFor(byteExpected,timeoutMS);
			if(_terminalConnector.IsSpecialCode((char)byteReceived)) {
				textProgress.Text+="Receieved expected byte: "
					+_terminalConnector.DisplaySpecialCode((char)byteReceived)+"'\r\n";
			}
			else {
				textProgress.Text+="Receieved expected byte: "+byteReceived.ToString()+"'\r\n";
			}
		}

		///<summary>Gets a single byte from the RxBuff. Throws exception if no byte received within the given time. Does not clear this byte from the RxBuff in any way.  That's up to the calling code.</summary>
		public byte GetOneByte(double timeoutMS){
			byte byteReceived=_terminalConnector.GetOneByte(timeoutMS);
			if(_terminalConnector.IsSpecialCode((char)byteReceived)) {
				textProgress.Text+="Receieved: byte "
					+_terminalConnector.DisplaySpecialCode((char)byteReceived)+"\r\n";
			}
			else {
				textProgress.Text+="Receieved: byte "+(char)byteReceived+"\r\n";
			}
			ScrollProgress();
			return byteReceived;
		}

		///<summary>Gets a precise number of bytes from RxBuff. Throws an error if not received in time.  Also, make sure to clear RxBuff before and after using this function.</summary>
		public byte[] GetBytes(int numberOfBytes, double timeoutMS){
			byte[] byteArrayReceived=_terminalConnector.GetBytes(numberOfBytes,timeoutMS);
			textProgress.Text+="Receieved block\r\n";
			ScrollProgress();
			return byteArrayReceived;
		}

		///<summary></summary>
		public void ClearRxBuff(){
			_terminalConnector.ClearRxBuff();
		}

		///<summary>Receives all text within the given timespan.</summary>
		public string Receive(double timeoutMS){
			textProgress.Text+="Receiving...\r\n";
			ScrollProgress();
			string textReceived=_terminalConnector.Receive(timeoutMS);
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
			_terminalConnector.Pause(ms);
			textProgress.Text+="Done pausing\r\n";
			ScrollProgress();
		}

		///<summary>Transmits a file using Xmodem protocol.</summary>
		public void UploadXmodem(string filePath){
			textProgress.Text+="Sending "+filePath+" by Xmodem\r\n";
			ScrollProgress();
			_terminalConnector.UploadXmodem(filePath);
			textProgress.Text+="Sending "+filePath+" by Xmodem complete\r\n";
			ScrollProgress();
		}

		///<summary>Receives a file using Xmodem protocol.</summary>
		public void DownloadXmodem(string filePath){
			textProgress.Text+="Retrieving "+filePath+" by Xmodem\r\n";
			ScrollProgress();
			_terminalConnector.DownloadXmodem(filePath);
			textProgress.Text+="Retrieving "+filePath+" by Xmodem complete\r\n";
			ScrollProgress();
		}
		
		///<summary>Converts any string to an acceptable format for modem ASCII. Converts to all caps and strips off all invalid characters. Optionally shortens the string to the specified length and/or makes sure the string is long enough by padding with spaces.</summary>
		public string Sout(string inputStr,int maxL,int minL){
			return _terminalConnector.Sout(inputStr,maxL,minL);
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

















