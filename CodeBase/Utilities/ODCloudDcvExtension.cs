using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Utilities.DcvExtension;
using Newtonsoft.Json;

namespace CodeBase.Utilities {
	public class ODCloudDcvExtension {
		private static DateTime LastRequest;
		private const int _bufferSize=4096;
		private const string _delimiter="<END>";
		private static readonly ODCloudDcvExtension _instance=new ODCloudDcvExtension();
    private NamedPipeClientStream _namedPipeClientStream;
		private ResponseListHandler _responseListHandler;

		private ODCloudDcvExtension() { }

		public static ODCloudDcvExtension Instance { 
			get {
				return _instance; 
			} 
		}

		public static async Task Start() {
			while (!Instance.IsConnected()) {
				LastRequest=DateTime.MinValue;
				await Task.Delay(500);
				try {
					// Create the client side stream to connect to the server namedpipe.
					Instance._namedPipeClientStream=new NamedPipeClientStream(".","ServerPipe",PipeDirection.InOut,PipeOptions.Asynchronous | PipeOptions.WriteThrough);
					await Instance._namedPipeClientStream.ConnectAsync();
					Instance._responseListHandler=new ResponseListHandler();
					await Instance.RunClient(Instance._responseListHandler);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
		}

		public bool IsConnected() {
			try {
				return (Instance._namedPipeClientStream!=null && Instance._namedPipeClientStream.IsConnected);
			}
			catch (Exception ex) {
				ex.DoNothing();
				return false;
			}
		}

		public DateTime GetLastRequest() {
			return LastRequest;
		}

		public async Task SendRequest(string request) {
			try {
				if (Instance._namedPipeClientStream!=null) {
					byte[] messageByteArray=Encoding.UTF8.GetBytes(request+_delimiter);
					int numBytes=messageByteArray.Length;
					int numBytesSent=0;
					while (numBytesSent<numBytes) {
						int numBytesSending=Math.Min(_bufferSize,numBytes-numBytesSent);
						await Instance._namedPipeClientStream.WriteAsync(messageByteArray,numBytesSent,numBytesSending);
						numBytesSent+=numBytesSending;
					}
				}
			}
			catch (Exception ex) {
				ex.DoNothing();
			}
		}

		public string GetResponse(string requestId) {
			if(Instance._responseListHandler==null) {
				return null;
			}
			string response=Instance._responseListHandler.GetResponse(requestId);
			return response;
		}

		private async Task RunClient(ResponseListHandler responseListHandler) {
			byte[] readBufferArray=new byte[_bufferSize];
			StringBuilder stringBuilder=new StringBuilder();
			while(Instance._namedPipeClientStream.IsConnected) {
				try {
					int numBytes=await Instance._namedPipeClientStream.ReadAsync(readBufferArray,0,readBufferArray.Length);
					if(numBytes==0) {
						continue;
					}
          if(!IsValidUtf8(readBufferArray,numBytes)) {
						continue;
					}
					stringBuilder.Append(Encoding.UTF8.GetString(readBufferArray,0,numBytes));
          string completeResponse=stringBuilder.ToString();
          if(completeResponse.Contains(_delimiter)) {
            int delimiterIndex=completeResponse.IndexOf(_delimiter);
            string response=completeResponse.Substring(0,delimiterIndex);

            ProcessResponse(response, responseListHandler);

            completeResponse=completeResponse.Substring(delimiterIndex+_delimiter.Length);
          }
          stringBuilder.Clear();
          stringBuilder.Append(completeResponse);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
		}

		private static bool IsValidUtf8(byte[] byteArray,int length) {
      try {
        Encoding.UTF8.GetString(byteArray,0,length);
        return true;
      }
      catch (DecoderFallbackException) {
        return false;
      }
    }

		private static void ProcessResponse(string response,ResponseListHandler responseListHandler) {
			if(response==null || response.Length==0) {
				return;
			}
			LastRequest=DateTime.Now;
			responseListHandler.AddResponse(response);
		}
	}
}