using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace CodeBase {
	public class ODCloudClient {
		///<summary>Used for OD Cloud. This delegate will be invoked to send data to the browser.</summary>
		public static SendDataToBrowserDelegate SendDataToBrowser;
		///<summary>Used for OD Cloud. This delegate is used to send data to the browser.</summary>
		///<param name="data">Data to send to the browser.</param>
		///<param name="browserAction">Action the browser should perform. Based off <see cref="BrowserAction"></see>.</param>
		///<param name="onReceivedResponse">If the browser returns a response, this action can act upon it.</param>
		public delegate void SendDataToBrowserDelegate(string data,int browserAction,Action<string> onReceivedResponse=null);

		///<summary>Sends a request to ODCloudClient to launch the file.</summary>
		public static void LaunchFileWithODCloudClient(string exePath="",string extraArgs="",string extraFilePath="",string extraFileData="",
			string extraFileType="",bool doWaitForResponse=false,string createDirIfNeeded="") 
		{
			if(exePath.StartsWith("http://") || exePath.StartsWith("https://")) {
				Process.Start(exePath);//No need to go to ODCloudClient if we can simply launch a browser tab.
				return;
			}
			ODCloudClientData cloudClientData=new ODCloudClientData {
				ExePath=exePath,
				Args=extraArgs,
				FilePath=extraFilePath,
				FileData=extraFileData,
				FileType=extraFileType,
				CreateDirIfNeeded=createDirIfNeeded,
			};
			if(doWaitForResponse) {
				SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.LaunchFile);
			}
			else {
				SendToODCloudClient(cloudClientData,CloudClientAction.LaunchFile);
			}
		}

		/// <summary>Synchronously requests the clipboard files from the client's FileDropList and places them in '.../temp/opendental/ODCloudFileTransfer'.</summary>
		/// <returns>string[] of paths to the files in the temp directory</returns>
		public static string[] GetClipboardFilesFromODCloudClient() {
			string response=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetClipboardFiles);
			if(response.IsNullOrEmpty()) {
				return null;
			}
			CloudClientResult result=JsonConvert.DeserializeObject<CloudClientResult>(response);
			if(result.ResultCodeEnum==CloudClientResultCode.ODException) {//Throw ODException that was returned by ODCloudClient
				ODException odEx=JsonConvert.DeserializeObject<ODException>(result.ResultData);
				throw odEx;
			}
			if(result.ResultCodeEnum==CloudClientResultCode.Error) {//Throw generic Exception that was returned by ODCloudClient
				throw new Exception(result.ResultData);
			}
			string tempPathFT=GetFileTransferTempPath();//Path '.../temp/opendental/ODCloudFileTransfer'
			//Tuple: Item1=base64 filedata    Item2=filename
			//Write all files to the temp path using the given filename (Item2) and return an array of file paths
			Tuple<string,string>[] arrayFileData=JsonConvert.DeserializeObject<Tuple<string,string>[]>(result.ResultData);
			arrayFileData.ForEach(x => File.WriteAllBytes(ODFileUtils.CombinePaths(tempPathFT,x.Item2),Convert.FromBase64String(x.Item1)));
			return arrayFileData.Select(x => ODFileUtils.CombinePaths(tempPathFT,x.Item2)).ToArray();
		}

		///<summary>Returns '.../temp/opendental/ODCloudFileTransfer' temp path.</summary>
		private static string GetFileTransferTempPath() {
			string tempPath=ODFileUtils.CombinePaths(Path.GetTempPath(),"opendental","ODCloudFileTransfer");
			if(!Directory.Exists(tempPath)) {
				Directory.CreateDirectory(tempPath);
			}
			return tempPath;
		}

		///<summary>Sends a request to ODCloudClient to write the claim data to the specified file and archive any old claims.</summary>
		public static void ExportClaim(string filePath,string claimText,bool doOverwriteFile=true) {
			ODCloudClientData cloudClientData=new ODCloudClientData {
				FilePath=filePath,
				FileData=claimText,
				DoOverwriteFile=doOverwriteFile,
			};
			SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.ExportClaim);
		}

		///<summary>Sends a message to ODCloudClient to launch Sirona.</summary>
		public static void SendToSirona(string pathExe,List<string> listIniLines) {
			ODCloudClientData cloudClientData=new ODCloudClientData {
				ExePath=pathExe,
				OtherData=JsonConvert.SerializeObject(listIniLines),
			};
			SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.SendToSirona);
		}

		///<summary>Makes an HTTP GET request to the specified URL and returns the response.</summary>
		public static string DownloadString(string url,int timeoutSecs=30,bool doShowProgressBar=true) {
			var otherData=new {
				Url=url,
			};
			ODCloudClientData cloudClientData=new ODCloudClientData {
				OtherData=JsonConvert.SerializeObject(otherData),
			};
			return SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.DownloadString,timeoutSecs,doShowProgressBar);
		}

		///<summary>Sends the data to the browser and waits for a response.</summary>
		public static string SendToBrowserSynchronously(string data,BrowserAction browserAction,int timeoutSecs=5,bool doShowProgressBar=true) {
			bool hasReceivedResponse=false;
			string browserResponse="";
			Exception exFromThread=null;
			void onReceivedResponse(string response) {
				hasReceivedResponse=true;
				browserResponse=response;
			}
			ODThread thread=new ODThread(o => {
				SendDataToBrowser(data,(int)browserAction,onReceivedResponse);
			});
			thread.Name="SendDataToBrowser";
			thread.AddExceptionHandler(e => exFromThread=e);
			thread.Start();
			DateTime start=DateTime.Now;
			void waitForResponse() {
				while(!hasReceivedResponse && exFromThread==null && (DateTime.Now-start).TotalSeconds<timeoutSecs) {
					Thread.Sleep(100);
				}
			}
			if(doShowProgressBar) {
				ODProgress.ShowAction(waitForResponse);
			}
			else {
				waitForResponse();
			}
			if(exFromThread!=null) {
				throw exFromThread;
			}
			if(!hasReceivedResponse) {
				throw new ODException("Unable to communicate with the browser.",ODException.ErrorCodes.BrowserTimeout);
			}
			return browserResponse;
		}

		///<summary>Sends a requests to ODCloudClient and waits for a response. Throws any exception that ODCloudClient returns.</summary>
		private static string SendToODCloudClientSynchronously(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction,int timeoutSecs=30
			,bool doShowProgressBar=true) 
		{
			string odCloudClientResponse;
			try {
				string request=GetODCloudClientRequest(cloudClientData,cloudClientAction);
				odCloudClientResponse=SendToBrowserSynchronously(request,(int)BrowserAction.SendToODCloudClient,timeoutSecs,doShowProgressBar);
			}
			catch(ODException odEx) {
				if(odEx.ErrorCodeAsEnum==ODException.ErrorCodes.BrowserTimeout) {
					throw new ODException("Unable to communicate with OD Cloud Client.",ODException.ErrorCodes.ODCloudClientTimeout);
				}
				throw;
			}
			CloudClientResult result=JsonConvert.DeserializeObject<CloudClientResult>(odCloudClientResponse);
			if(result.ResultCodeEnum==CloudClientResultCode.ODException) {
				ODException odEx=JsonConvert.DeserializeObject<ODException>(result.ResultData);
				throw odEx;
			}
			if(result.ResultCodeEnum==CloudClientResultCode.Error) {
				throw new Exception(result.ResultData);
			}
			return result.ResultData;
		}

		private static string GetODCloudClientRequest(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction) {
			string dataStr=JsonConvert.SerializeObject(cloudClientData);
			//We will sign dataStr to prove this request came from an Open Dental server.
			byte[] byteArray=Encoding.Unicode.GetBytes(dataStr);
			CspParameters csp=new CspParameters {
				KeyContainerName="cloudkey",
				Flags=CspProviderFlags.UseMachineKeyStore,
			};
			using RSACryptoServiceProvider rsa=new RSACryptoServiceProvider(csp);
			byte[] signedByteArray=rsa.SignData(byteArray,CryptoConfig.MapNameToOID("SHA256"));
			string signature=Convert.ToBase64String(signedByteArray);
			string publicKey=rsa.ToXmlString(false);
			ODCloudClientArgs jsonData=new ODCloudClientArgs {
				Data=dataStr,
				Signature=signature,
				PublicKey=publicKey,
				ActionEnum=cloudClientAction,
			};
			return JsonConvert.SerializeObject(jsonData);
		}

		///<summary>Sends a request to ODCloudClient.</summary>
		private static void SendToODCloudClient(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction) {
			if(SendDataToBrowser==null) {
				throw new ODException(nameof(SendDataToBrowser)+" has not been initialized.");
			}
			string request=GetODCloudClientRequest(cloudClientData,cloudClientAction);
			SendDataToBrowser(request,(int)BrowserAction.SendToODCloudClient);
		}

		///<summary>Contains the data to be sent to OD Cloud Client. Will be serialized as JSON.</summary>
		public class ODCloudClientData {
			///<summary>The path of the executable to launch. Can be empty if FilePath is set.</summary>
			public string ExePath;
			///<summary>Arguments to pass to ExePath when launched.</summary>
			public string Args;
			///<summary>Path to write FileData to.</summary>
			public string FilePath;
			///<summary>File contents to write to FilePath.</summary>
			public string FileData;
			///<summary>Options are "binary" or "text".</summary>
			public string FileType;
			///<summary>Defaults to true. Whether to overwrite FilePath. If false and FilePath exists, throws an exception.</summary>
			public bool DoOverwriteFile=true;
			///<summary>If included, will create the directory if it doesn't exist.</summary>
			public string CreateDirIfNeeded;
			///<summary>Any additional data that is necessary for this action type.</summary>
			public string OtherData;
		}

		///<summary>Contains the arguments to be sent to OD Cloud Client. Will be serialized as JSON.</summary>
		public class ODCloudClientArgs {
			///<summary>This will be a JSON serialized string of <see cref="ODCloudClientData"/>.</summary>
			public string Data;
			///<summary>A signature of Data to prove this is from on Open Dental server.</summary>
			public string Signature;
			///<summary>The public key that corresponds to the private key used to sign the Signature.</summary>
			public string PublicKey;
			///<summary>The action that should be performed for this request.</summary>
			public int Action;
			///<summary>The enum version of the action that should be performed.</summary>
			[JsonIgnore]
			public CloudClientAction ActionEnum {
				get {
					return (CloudClientAction)Action;
				}
				set {
					Action=(int)value;
				}
			}
		}

		///<summary>Structure for the response that is sent back to JavaScript for any request coming to ODCloudClient.</summary>
		public class CloudClientResult {
			///<summary>The response from the request.</summary>
			public string ResultData;
			///<summary>String representation of the ResultCode.</summary>
			public string ResultCodeStr => ResultCodeEnum.ToString();
			///<summary>Code to identify the result of the request.</summary>
			public int ResultCode;
			///<summary>Enum representation of the ResultCode.</summary>
			[JsonIgnore]
			public CloudClientResultCode ResultCodeEnum {
				get {
					return (CloudClientResultCode)ResultCode;
				}
				set {
					ResultCode=(int)value;
				}
			}
		}

		///<summary>Different action types that can be sent to ODCloudClient.</summary>
		public enum CloudClientAction {
			///<summary>Asks ODCloudClient to launch a file. May also write data to a file.</summary>
			LaunchFile,
			///<summary>Checks if ODCloudClient is running.</summary>
			CheckIsRunning,
			///<summary>Writes the claim data to the specified file and archives any old claims.</summary>
			ExportClaim,
			///<summary>Launches the Sirona bridge.</summary>
			SendToSirona,
			///<summary>Makes an HTTP GET request to the specified URL and returns the response.</summary>
			DownloadString,
			///<summary>Gets the name of the computer the browser is running on.</summary>
			GetComputerName,
			///<summary>Gets the contents of the user's clipboard as a list of files.</summary>
			GetClipboardFiles,
		}

		///<summary>Tells the browser what action to take with the data passed to it.</summary>
		public enum BrowserAction {
			///<summary>Pass the data on to ODCloudClient.</summary>
			SendToODCloudClient,
			///<summary>Write the data to the user's clipboard.</summary>
			SetClipboard,
			///<summary>Gets the contents of the user's clipboard as text.</summary>
			GetClipboardText,
			///<summary>Get the contents of the user's clipboard as an image.</summary>
			GetClipboardImage,
			///<summary>Gets the name of the computer the browser is running on.</summary>
			GetComputerName,
			/// <summary>Plays the sound on the browser.</summary>
			PlaySound,
		}

		///<summary>Enum to identify the result of the request.</summary>
		public enum CloudClientResultCode {
			///<summary>The request completed successfully.</summary>
			Success,
			///<summary>The ODCloudClient that received the request was not the one the browser meant to send it to.</summary>
			IdentifierMismatch,
			///<summary>The request was unable to be completed. Unknown error.</summary>
			Error,
			///<summary>An ODException was thrown. ResultData will contain a serialized ODException.</summary>
			ODException,
		}

	}
}
