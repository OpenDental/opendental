using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace OpenDental.InternalTools.Phones {
	public class PersistentTcpClient {
		public string UserName;
		public string Password;
		private int _port=18022;
		private TcpClient _tcpClient = new TcpClient();
		private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);
		private SslStream sslStream;
		public bool IsPWValid;

		public async Task ConnectAsyncIfNeeded(){
			try{
				if(_tcpClient!=null && _tcpClient.Connected){
					//Unfortunately, we cannot properly test all scenarios.
					//The above test can return true even if _tcpClient is disposed and not ready for use.
					//This is because Connected does not perform a live check, but instead only checks whether it was connected the last time an operation was performed.
					//There's also no way to check _tcpClient for disposed.
					//For these reasons, we must use big try/catches around all of our _tcpClient activity.
					//We also minimize hitting the try/catch by always setting _tcpClient to null when we dispose of it.
					return;//this is what normally happens if we don't need to re-establish connection.
				}
			}
			catch{
			}
			string server="hqvideo.opendental.com";
			#if DEBUG && LOCAL
				server="localhost";
			#endif
			//We are now guaranteed to either have a null _tcpClient or Connected=false.
			if(_tcpClient is null){
				_tcpClient = new TcpClient();
			}
			try{
				await _tcpClient.ConnectAsync(server, _port);
				NetworkStream networkStream=_tcpClient.GetStream();//network stream is not buffered, so no flush
				sslStream = new SslStream(networkStream,leaveInnerStreamOpen:false,userCertificateValidationCallback:new RemoteCertificateValidationCallback(ValidateServerCertificate));
				await sslStream.AuthenticateAsClientAsync(server);
			}
			catch{
				_tcpClient=null;//so that the next request will create a new client. Might be a better way?
				return;
			}
			//No need for a keep-alive mechanism because the client is already frequently asking the server for clocked-in status.
			//We have now successfully connected.
			IsPWValid=false;
			try{
				IsPWValid=await CheckIsPWValid();
			}
			catch{
				_tcpClient=null;
				return;
			}
		}

		public bool IsConnected(){
			if(_tcpClient is null){
				return false;
			}
			return true;
		}
		
		public static bool ValidateServerCertificate(object sender,X509Certificate x509Certificate,X509Chain x509Chain,SslPolicyErrors sslPolicyErrors){
			if(sslPolicyErrors==SslPolicyErrors.None){
				//.NET framework has already done all the normal validation and has not found any errors
				return true;
			}
			//If there are errors, we might check them and let some situations through.
			//But safest for now to just no allow with any errors.
			return false;
		}

		public void Close(){
			try{
				_tcpClient?.Close();
			}
			catch{ }
		}

		///<summary>Throws exception if cannot connect.</summary>
		private async Task<bool> CheckIsPWValid(){
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings); 
			xmlWriter.WriteStartElement("IsPWValid");
			xmlWriter.WriteStartElement("UserName");
			xmlWriter.WriteString(UserName);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Password");
			xmlWriter.WriteString(Password);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Dispose();
			string request=stringBuilder.ToString();
			//Bypass the semaphore
			byte[] byteArrayResponse=await SendReceiveActual("IsPWValid",Encoding.ASCII.GetBytes(request));
			if(byteArrayResponse is null){
				throw new Exception();//can happen during debugging if execution paused on server because it simply times out.
			}
			string response=Encoding.ASCII.GetString(byteArrayResponse, 0, byteArrayResponse.Length);
			if(response=="true"){
				return true;
			}
			return false;
		}

		///<summary>Can return null</summary>
		public async Task<Bitmap> RequestBitmap(string userName){
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings); 
			xmlWriter.WriteStartElement("RequestBitmap");
			xmlWriter.WriteStartElement("UserName");
			xmlWriter.WriteString(userName);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Dispose();
			string request=stringBuilder.ToString();
			byte[] byteArrayResponse=await SendReceive("RequestBitmap",Encoding.ASCII.GetBytes(request));
			if(byteArrayResponse is null){
				return null;
			}
			if(byteArrayResponse.Length==1 && byteArrayResponse[0]==0){
				return null;
			}
			using MemoryStream memoryStream = new MemoryStream(byteArrayResponse);
			Bitmap bitmap = (Bitmap)Image.FromStream(memoryStream);//don't need to specify jpg
			memoryStream.Close();
			return bitmap;
		}

		///<summary>Can return null if anything goes wrong.</summary>
		private async Task<byte[]> SendReceive(string name,byte[] byteArrayRequest){
			//I had a bug here where each timer tick was starting this method.
			//But they all got stuck at WriteAsync when server was unavailable.
			//The solution is to make sure only one instance of this method runs at a time.
			//This is currently done by turning off the timer each time we come in here.
			//But there might be a variety of actions that all call this,
			//so the semaphore enforces one at a time.
			//Subsequent requests stack up, although there shouldn't be very many.
			bool isSuccess=await _semaphoreSlim.WaitAsync(5000);//times out after 5 seconds
			//I didn't actually test this semaphore mechanism yet because that's hard.
			if(!isSuccess){
				return null;
			}
			byte[] byteArrayResponse;
			try{
				byteArrayResponse=await SendReceiveActual(name,byteArrayRequest);
			}
			finally{
				_semaphoreSlim.Release();
			}
			return byteArrayResponse;
		}
		
		///<summary>Can return null if anything goes wrong.</summary>
		private async Task<byte[]> SendReceiveActual(string name,byte[] byteArrayRequest){
			//Header format for outgoing requests: [MessageName][MessageLength]
			//MessageName is always exactly 30 bytes in ASCII. Padded on right with spaces.
			//MessageLength does not include header length. It is 4 bytes, big endian.
			//So header is always 34.
			if(name!="IsPWValid"){//because IsPWValid is only called from within ConnectAsyncIfNeeded. That would be an infinite loop.
				await ConnectAsyncIfNeeded();
			}
			int lengthRequest = byteArrayRequest.Length;
			byte[] byteArrayLength = BitConverter.GetBytes(lengthRequest);
			if(BitConverter.IsLittleEndian){
				Array.Reverse(byteArrayLength);//ensure big endian
			}
			byte[] byteArrayFinalOut=new byte[34+byteArrayRequest.Length];
			byte[] byteArrayName=Encoding.ASCII.GetBytes(name.PadRight(30,' '));
			Array.Copy(byteArrayName,0,byteArrayFinalOut,0,30);//[MessageName]
			Array.Copy(byteArrayLength,0,byteArrayFinalOut,30,4);//[MessageLength]
			Array.Copy(byteArrayRequest,0,byteArrayFinalOut,34,lengthRequest);
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));//3 seconds wasn't long enough even on my localhost test environment
			try{
				await sslStream.WriteAsync(byteArrayFinalOut, 0, byteArrayFinalOut.Length,cancellationTokenSource.Token);
			}
			catch{
				_tcpClient=null;//so that the next request will create a new client. Might be a better way?
				return null;
			}
			//Process response=====================================================================================
			//Incoming message headers are different.
			//We already know the type since we are the ones who sent the message.
			//So incoming message header is just [MessageLength], 4 bytes, not including header length.
			int lengthHeader=4;
			int countRead=0;
			byte[] byteArrayHeader=new byte[lengthHeader];
			while(true){
				//Yes, this is almost certainly overkill for just 4 bytes, but it's correct.
				if(countRead>=lengthHeader){
					break;
				}
				int countReadThisLoop=0;
				//Logger.Log("header Time started ReadAsync: "+DateTime.Now.Ticks.ToString());
				try{
					countReadThisLoop=await sslStream.ReadAsync(byteArrayHeader,countRead,lengthHeader-countRead,cancellationTokenSource.Token);
					//count can be 0 here if connection closed, even without exception.
				}
				catch{ }
				//Logger.Log("header Time finished ReadAsync: "+DateTime.Now.Ticks.ToString());
				//Logger.Log("header countReadThisLoop: "+countReadThisLoop.ToString());
				//Logger.Log("header countRead: "+countRead.ToString());
				//Logger.Log(byteArrayHeader);
				if(countReadThisLoop==0){
					_tcpClient=null;
					return null;
				}
				countRead+=countReadThisLoop;
			}
			if(BitConverter.IsLittleEndian){
				Array.Reverse(byteArrayHeader);
			}
			int lengthResponse = BitConverter.ToInt32(byteArrayHeader,0);
			//End of header processing------------------------------------------------------------------------
			byte[] byteArrayResponse=new byte[lengthResponse];
			countRead=0;
			while(true){
				if(countRead>=lengthResponse){
					break;
				}
				int countReadThisLoop=0;
				try{
					countReadThisLoop=await sslStream.ReadAsync(byteArrayResponse,countRead,lengthResponse-countRead,cancellationTokenSource.Token);
				}
				catch{ }
				//Logger.Log("response countReadThisLoop: "+countReadThisLoop.ToString());
				//Logger.Log("response countRead: "+countRead.ToString());
				//Logger.Log(byteArrayResponse);
				if(countReadThisLoop==0){
					_tcpClient=null;
					return null;
				}
				countRead+=countReadThisLoop;
			}
			//sslStream.Dispose();//No, this is owned by tcpClient and it shouldn't be disposed here
			return byteArrayResponse;
		}
	}
}
