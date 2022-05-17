using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace CodeBase {
	public class ODCloudClient {
		private static DateTime _dateVersionLastChecked;
		private static string _latestCloudClientVersion;

		///<summary>Used for OD Cloud. This delegate will be invoked to send data to the browser.</summary>
		public static SendDataToBrowserDelegate SendDataToBrowser;
		///<summary>Used to get the latest cloud client version number from HQ.</summary>
		public static GetLatestCloudClientVersionDelegate GetLatestCloudClientVersion;
		///<summary>Used for OD Cloud. This delegate is used to send data to the browser.</summary>
		///<param name="data">Data to send to the browser.</param>
		///<param name="browserAction">Action the browser should perform. Based off <see cref="BrowserAction"></see>.</param>
		///<param name="onReceivedResponse">If the browser returns a response, this action can act upon it.</param>
		public delegate void SendDataToBrowserDelegate(string data,int browserAction,Action<string> onReceivedResponse=null);
		///<summary>Used to get the latest cloud client version number from HQ.</summary>
		public delegate string GetLatestCloudClientVersionDelegate();

		/// <summary>
		/// Sends a request to ODCloudClient to launch the file.
		/// </summary>
		/// <param name="exePath">Path for program to be launched</param>
		/// <param name="extraArgs"></param>
		/// <param name="extraFilePath"></param>
		/// <param name="extraFileData"></param>
		/// <param name="extraFileType"></param>
		/// <param name="doWaitForResponse"></param>
		/// <param name="createDirIfNeeded"></param>
		/// <param name="tryLaunch">Setting this to true will have the client check and see if the process is already running before attempting to launch it</param>
		public static void LaunchFileWithODCloudClient(string exePath="",string extraArgs="",string extraFilePath="",string extraFileData="",
			string extraFileType="",bool doWaitForResponse=false,string createDirIfNeeded="",bool tryLaunch=false,bool doStartWithoutExtraFile=false) 
		{
			if(exePath.StartsWith("http://") || exePath.StartsWith("https://")) {
				Process.Start(exePath);//No need to go to ODCloudClient if we can simply launch a browser tab.
				return;
			}
			CloudClientAction action=CloudClientAction.LaunchFile;
			string processName="";
			if(tryLaunch) {
				action=CloudClientAction.TryLaunch;
				processName=GetProcessNameFromPath(exePath);
			}
			ODCloudClientData cloudClientData=new ODCloudClientData {
				ExePath=exePath,
				Args=extraArgs,
				FilePath=extraFilePath,
				FileData=extraFileData,
				FileType=extraFileType,
				CreateDirIfNeeded=createDirIfNeeded,
				ProcessName=processName,
				DoStartWithoutExtraFile=doStartWithoutExtraFile,
			};
			if(doWaitForResponse) {
				SendToODCloudClientSynchronously(cloudClientData,action);
			}
			else {
				SendToODCloudClient(cloudClientData,action);
			}
		}

		///<summary>Grabs the process name from the exe file path to see if it's currently running on the client's computer</summary>
		private static string GetProcessNameFromPath(string exePath) {
			string retVal="";
			if(string.IsNullOrWhiteSpace(exePath)) {
				return retVal;
			}
			exePath=exePath.TrimEnd('\\');
			int startIndex=Math.Max(0,exePath.LastIndexOf(@"\"));
			int endIndex=exePath.LastIndexOf('.');
			if(endIndex<0 || endIndex < startIndex) {
				retVal=exePath.Substring(startIndex);//there was no .exe ending, so we just need the last chunk of this filepath
			}
			else {
				retVal=exePath.Substring(startIndex,endIndex-startIndex);
			}
			return retVal.Trim(new char[] {'\\'});
		}

		/// <summary>Synchronously requests the clipboard files from the client's FileDropList and places them in '.../temp/opendental/ODCloudFileTransfer'.</summary>
		/// <returns>string[] of paths to the files in the temp directory</returns>
		public static string[] GetClipboardFilesFromODCloudClient() {
			string resultData=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetClipboardFiles);
			if(resultData.IsNullOrEmpty()) {
				return null;
			}
			string tempPathFT=GetFileTransferTempPath();//Path '.../temp/opendental/ODCloudFileTransfer'
			//Tuple: Item1=base64 filedata    Item2=filename
			//Write all files to the temp path using the given filename (Item2) and return an array of file paths
			Tuple<string,string>[] arrayFileData=JsonConvert.DeserializeObject<Tuple<string,string>[]>(resultData);
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

		/// <summary>Synchronously scans an image using the client computer's scanners.</summary>
		/// <returns>Bitmap of the image scanned.</returns>
		public static Bitmap GetImageFromScanner(bool selectSource, bool showOptions, bool isDuplex, bool isGrayscale, int resolution, int quality) {
			ODCloudClientData cloudClientData=new ODCloudClientData {
				ScanDocSelectSource=selectSource,
				ScanDocShowOptions=showOptions,
				ScanDocDuplex=isDuplex,
				ScanDocGrayscale=isGrayscale,
				ScanDocResolution=resolution,
				ScanDocQuality=quality,
			};
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.GetImageFromScanner, 300);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
				return null;
			}
			if(resultData.IsNullOrEmpty()) {//The scan was probably cancelled
				return null;
			}
			byte[] byteArray=Convert.FromBase64String(resultData);
			Bitmap bitmap;
			using MemoryStream memoryStream=new MemoryStream();
			memoryStream.Write(byteArray, 0, byteArray.Length);
			bitmap=(Bitmap)Bitmap.FromStream(memoryStream);
			return bitmap;
		}

		/// <summary>Synchronously scans multiple image using the client computer's scanners.</summary>
		/// <returns>Path to the temporary file (located at the temp path) containing the image.</returns>
		public static string GetImageMultiFromScanner(bool selectSource, bool showOptions, bool isDuplex, bool isGrayscale, int resolution, int quality) {
			ODCloudClientData cloudClientData=new ODCloudClientData {
				ScanDocSelectSource=selectSource,
				ScanDocShowOptions=showOptions,
				ScanDocDuplex=isDuplex,
				ScanDocGrayscale=isGrayscale,
				ScanDocResolution=resolution,
				ScanDocQuality=quality,
			};
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.GetImageMultiFromScanner, 300);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
				return null;
			}
			if(resultData.IsNullOrEmpty()) {
				return null;
			}
			byte[] bytes=Convert.FromBase64String(resultData);
			string tempFile=ODFileUtils.CreateRandomFile(ODFileUtils.CombinePaths(Path.GetTempPath(),"opendental"),".pdf");
			File.WriteAllBytes(tempFile,bytes);
			return tempFile;
		}

		///<summary>Uses devices on the client side. Asks ODCloudClient to prompt the user to select a scanner and sets that scanner as the default.
		///Returns true if the default scanner was successfully updated. Returns false if the request timed out or the user clicked 'cancel'.</summary>
		public static void SetDefaultScanner() {
			ODCloudClientData cloudClientData=new ODCloudClientData();
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.SetDefaultScanner, 300);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
			}
		}

		///<summary>Asks ODCloudClient to process a PayConnect terminal payment. If successful, returns the contents of the PosResponse object. Otherwise, returns null.
		///Type can be 'SALE', 'AUTH', 'VOID', 'RETURN'</summary>
		public static PayConnectResponse ProcessPaymentTerminal(string type,decimal amount,bool forceDuplicate,string refnum="") {
			ODCloudClientData cloudClientData=new ODCloudClientData {
				PayConnectType=type,
				PayConnectAmount=amount,
				PayConnectRefNum=refnum,
				PayConnectForceDuplicate=forceDuplicate
			};
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.PayConnectProcessPaymentTerminal,300);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
				return null;
			}
			return JsonConvert.DeserializeObject<PayConnectResponse>(resultData);
		}

		///<summary>Sends a request to ODCloudClient to write the claim data to the specified file and archive any old claims.</summary>
		public static void WriteFile(string filePath,string fileText,bool doOverwriteFile=true) {
			ODCloudClientData cloudClientData=new ODCloudClientData {
				FilePath=filePath,
				FileData=fileText,
				DoOverwriteFile=doOverwriteFile,
			};
			SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.WriteFile);
		}

		///<summary>Uses devices on the client side. Asks ODCloudClient to get the list of sources as a list of Twain Names. Returns an empty list on error.</summary>
		public static List<string> GetTwainSourceList() {
			ODCloudClientData cloudClientData=new ODCloudClientData();
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.GetTwainSourceList, 300);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
				return new List<string>();
			}
			return JsonConvert.DeserializeObject<List<string>>(resultData);
		}

		///<summary>Initializes the Cloud Client's TWAIN driver. Throws ODException or Exception on error.</summary>
		public static void TwainInitializeDevice(bool showTwainUI) {
			ODCloudClientData cloudClientData=new ODCloudClientData();
			cloudClientData.ScanDocShowOptions=showTwainUI;
			SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.TwainInitializeDevice, 300);
		}

		///<summary>Acquires a single bitmap from the TWAIN device given by the Twain Name</summary>
		public static Bitmap TwainAcquireBitmap(string twainName) {
			ODCloudClientData cloudClientData=new ODCloudClientData(){
				OtherData=twainName
			};
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.TwainAcquireBitmap, 300);
			}
			catch(Exception ex){
				if(!string.IsNullOrEmpty(ex.Message)){
					//Message is empty if the user cancelled
					ODMessageBox.Show(ex.Message);
				}
				return null;
			}
			if(resultData.IsNullOrEmpty()) {
				return null;
			}
			byte[] byteArray=Convert.FromBase64String(resultData);
			using MemoryStream memoryStream=new MemoryStream();
			memoryStream.Write(byteArray,0,byteArray.Length);
			return (Bitmap)Bitmap.FromStream(memoryStream);
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

		public static void SendToDrCeph(string data) {
			ODCloudClientData ccData=new ODCloudClientData() {
				Args=data
			};
			SendToODCloudClientSynchronously(ccData,CloudClientAction.SendToDrCeph);
		}

		///<summary>Sends the data to the browser.</summary>
		public static void SendToBrowser(ODBrowserData odBrowserData,BrowserAction browserAction) {
			string data=JsonConvert.SerializeObject(odBrowserData);
			SendDataToBrowser(data,(int)browserAction);
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

		///<summary>
		///Gets the latest cloud client version from HQ and updates the in-memory version and date last checked if it is time to do so. 
		///Returns true if the version should be checked by the cloud client, otherwise false.
		///</summary>
		private static bool DoCheckLatestCloudClientVersion() {
			if(_dateVersionLastChecked==DateTime.MinValue || string.IsNullOrWhiteSpace(_latestCloudClientVersion) || _dateVersionLastChecked.AddDays(1).CompareTo(DateTime.Now)<0) {
				try {
					_latestCloudClientVersion=GetLatestCloudClientVersion?.Invoke();
					return true;
				}
				catch(Exception ex) {
					//This exception is probably because the latest client version couldn't be retrieved. This is not critical. Continue without checking the cloud client version.
					ex.DoNothing();
				}
				finally {
					_dateVersionLastChecked=DateTime.Now;
				}
			}
			return false;
		}

		///<summary>Sends a requests to ODCloudClient and waits for a response. Throws any exception that ODCloudClient returns.</summary>
		private static string SendToODCloudClientSynchronously(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction,int timeoutSecs=30
			,bool doShowProgressBar=true) 
		{
			string odCloudClientResponse;
			cloudClientData.DoCheckVersion=DoCheckLatestCloudClientVersion();
			cloudClientData.LatestCloudClientVersion=_latestCloudClientVersion;
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
			cloudClientData.DoCheckVersion=DoCheckLatestCloudClientVersion();
			cloudClientData.LatestCloudClientVersion=_latestCloudClientVersion;
			string request=GetODCloudClientRequest(cloudClientData,cloudClientAction);
			SendDataToBrowser(request,(int)BrowserAction.SendToODCloudClient);
		}

		///<summary>Contains the data to be sent to the browser to perfrom a browser action. Will be serialized as JSON.</summary>
		public class ODBrowserData {
			public string ElementId;
			public string Url;
			public bool IsVisible;
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
			///<summary>Defaults to false. Whether to start start the process if the extra file can't be created. If false and the extra file can't be created, throws an exception.</summary>
			public bool DoStartWithoutExtraFile=false;
			///<summary>Defaults to true. Whether to overwrite FilePath. If false and FilePath exists, throws an exception.</summary>
			public bool DoOverwriteFile=true;
			///<summary>If included, will create the directory if it doesn't exist.</summary>
			public string CreateDirIfNeeded;
			///<summary>Defaults to true. Whether the scanner selection window should be displayed.</summary>
			public bool ScanDocSelectSource=true;
			///<summary>Defaults to false. Whether the scaner options window should be displayed.</summary>
			public bool ScanDocShowOptions=false;
			///<summary>Defaults to false. Whether the scaner should use duplex mode.</summary>
			public bool ScanDocDuplex=false;
			///<summary>Defaults to false. Whether the scaner should scan in grayscale if scanDocShowOptions is false.</summary>
			public bool ScanDocGrayscale=false;
			///<summary>The resolution the scanner should use if scanDocShowOptions is false.</summary>
			public int ScanDocResolution;
			///<summary>The JPEG compression quality the scanner should use if scanDocShowOptions is false.</summary>
			public int ScanDocQuality;
			///<summary>The PayConnect transaction type used for processing a payment from a terminal. Values are: SALE, AUTH, VOID, RETURN</summary>
			public string PayConnectType;
			///<summary>The PayConnect transaction amount.</summary>
			public decimal PayConnectAmount;
			///<summary>The PayConnect transaction reference number. Used for some RETURN transactions.</summary>
			public string PayConnectRefNum;
			///<summary>Whether the PayConnect PosRequest should 'Force Duplicate'.</summary>
			public bool PayConnectForceDuplicate;
			///<summary>Any additional data that is necessary for this action type.</summary>
			public string OtherData;
			///<summary>Process name that needs to be checked for when the action is a TryLaunch</summary>
			public string ProcessName;
			///<summary>The latest version number of the ODCloudClient.</summary>
			public string LatestCloudClientVersion;
			///<summary>Indicates that the browser should prompt the user if their ODCloudClient is not the latest version.</summary>
			public bool DoCheckVersion=false;
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
			///<summary>Current cloud client version number.</summary>
			public string ClientVersion;
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
			///<summary>Gets the image from a scanner device as a bitmap.</summary>
			GetImageFromScanner,
			///<summary>Gets multiple images from a scanner device as a .pdf file containing the image.</summary>
			GetImageMultiFromScanner,
			///<summary>Asks ODCloudClient to prompt the user to select a default scanner.</summary>
			SetDefaultScanner,
			///<summary>Asks ODCloudClient to process a PayConnect terminal payment and return the contents of PosResponse.</summary>
			PayConnectProcessPaymentTerminal,
			///<summary>Asks ODCloudClient to write a file with the given text at the given path.</summary>
			WriteFile,
			///<summary>Call this enum action if you just want to make sure that a process is running before trying to call it</summary>
			TryLaunch,
			///<summary>Launches the DrCeph bridge</summary>
			SendToDrCeph,
			///<summary>Gets the list of the client's available TWAIN sources by their Twain Name.</summary>
			GetTwainSourceList,
			///<summary>Initialize a TWAIN device. This is its own action in case there are multiple TwainAcquireBitmap calls in a row.</summary>
			TwainInitializeDevice,
			///<summary>Asks ODCloudClient to acquire a single image from the TWAIN device. Assumes the device has already been initialized using TwainInitializeDevice.</summary>
			TwainAcquireBitmap,
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
			///<summary>Plays the sound on the browser.</summary>
			PlaySound,
			///<summary>Plays the sound on the browser.</summary>
			NavigateIframe,
			///<summary>Plays the sound on the browser.</summary>
			SetIframeVisible,
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
