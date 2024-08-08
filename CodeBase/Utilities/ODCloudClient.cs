using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CodeBase.Controls;
using Newtonsoft.Json;

namespace CodeBase {
	///<summary>Aka Thinfinity.</summary>
	public class ODCloudClient {
		private static DateTime _dateVersionLastChecked;
		private static string _latestCloudClientVersion;
		private static bool _hasReceivedResponse;
		private static string _response;
		public static bool IsApiEnabled=false;
		///<summary>Controlled by the CloudIsAppStream preference and is set when OpenDental is launched. Used to determine if communication with the CloudClient is done via HTTP or the FileWatcher.</summary>
		public static bool IsAppStream=false;
		///<summary>Used to completely shutdown attempts to use the CloudClient if we were unable to locate the FileWatcher Directory.</summary>
		public static bool DidLocateFileWatcherDirectory=true;
		///<summary>The directory the FileWatcher will monitor and raise events for </summary>
		public static string FileWatcherDirectory="";
		public static string FileWatcherDirectoryAPI="";
		private static string _responseFilePath="";
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
		/// Sends a request to ODCloudClient to launch the file. Throws exceptions.
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
				if(!ODCloudClient.IsAppStream){
					Process.Start(exePath);//If using Thinfinity, we can simply launch a browser tab.
					return;
				}
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
		
		///<summary>Sends a request to ODCloudClient to check if a directory exists or create a directory.</summary>
		/// <param name="directoryName"></param>
		/// <param name="action"></param>
		public static string CheckOrCreateWithODCloudClient(string directoryName,CloudClientAction action) {
			string response="";
			ODCloudClientData cloudClientData=new ODCloudClientData {
				OtherData=directoryName,
			};
			try {
				response=SendToODCloudClientSynchronously(cloudClientData,action);
			}
			catch(Exception ex) {
				ODMessageBox.Show(ex.Message);
			}
			if(response!="Success" && action==CloudClientAction.CreateDirectory) {
				throw new Exception("Failed to create directory with ODCloudClient.");
			}
			return response;
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

		public static bool ClearClipboard() {
			try {
				return Convert.ToBoolean(SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.ClearClipboard));
			}
			catch(Exception) {
				return false;
			}
		}

		public static string GetClipboardImageFromODCloudClient(bool doShowProgressBar=false) {
			string resultData=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetClipboardImage,doShowProgressBar:doShowProgressBar);
			if(resultData.IsNullOrEmpty()) {
				return null;
			}
			return resultData;
		}

		public static void SetClipboardText(string text) {
			if(ODBuild.IsThinfinity()) {
				SendDataToBrowser(text,(int)BrowserAction.SetClipboard);
			}
			else {
				SendToODCloudClient(new ODCloudClientData() { OtherData=text },CloudClientAction.SetClipboardText);
			}
		}

		public static string GetClipboardText() {
			if(ODBuild.IsThinfinity()) {
				return SendToBrowserSynchronously("",BrowserAction.GetClipboardText);
			}
			else {
				return SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetClipboardText,doShowProgressBar:false);
			}
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
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.SetDefaultScanner,300,false);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
			}
		}

		///<summary></summary>
		public static void CopyToClipboard(Bitmap bitmapCopy=null,string fileName=null,int nodeType=-1, long imageKey=-1, string dbNameOrUri=null){
			ODCloudClientData oDCloudClientData=new ODCloudClientData();
			if(bitmapCopy!=null){
				try{
					using (MemoryStream memoryStream=new MemoryStream()){
						bitmapCopy.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
						byte[] byteArray=memoryStream.ToArray();
						oDCloudClientData.BitmapCopy=Convert.ToBase64String(byteArray);
					}
				}
				catch(Exception){
					return;
				}
			}
			if(!string.IsNullOrEmpty(fileName)){
				oDCloudClientData.FilePath=fileName;
			}
			if(nodeType>-1 && imageKey>-1){
				oDCloudClientData.NodeType=nodeType;
				oDCloudClientData.ImageKey=imageKey;
			}
			if(!string.IsNullOrEmpty(dbNameOrUri)){
				oDCloudClientData.OtherData=dbNameOrUri;
			}
			try{
				SendToODCloudClientSynchronously(oDCloudClientData,CloudClientAction.CopyToClipboard);
			}
			catch(Exception ex){
				ex.DoNothing();
			}
		}

		///<summary></summary>
		public static CloudNodeTypeAndKey GetNodeTypeAndKey(){
			string strNodeTypeAndKey="";
			CloudNodeTypeAndKey nodeTypeAndKey=null;
			try{
				strNodeTypeAndKey=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetNodeTypeAndKey);
			}
			catch(Exception ex) {
				ODMessageBox.Show(ex.Message);
			}
			if(string.IsNullOrEmpty(strNodeTypeAndKey)){
				return null;
			}
			nodeTypeAndKey=JsonConvert.DeserializeObject<CloudNodeTypeAndKey>(strNodeTypeAndKey);
			return nodeTypeAndKey;
		}

		///<summary>Used for copy/paste in imaging module to prevent issues when copying across different databases</summary>
		public static string GetDbNameOrUriFromClipboard(){
			string dbNameOrUri="";
			try{
				dbNameOrUri=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetDbNameOrUriFromClipboard);
			}
			catch(Exception ex) {
				ex.DoNothing();//Don't need to handle here. Will return null and paste action will proceed.
			}
			if(string.IsNullOrEmpty(dbNameOrUri)){
				return null;
			}
			return dbNameOrUri;
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
		public static Bitmap TwainAcquireBitmap(string twainName, bool doThrowException=false,int timeoutSecs=300) {
			ODCloudClientData cloudClientData=new ODCloudClientData(){
				OtherData=twainName
			};
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(cloudClientData,CloudClientAction.TwainAcquireBitmap,timeoutSecs:timeoutSecs);
			}
			catch(Exception ex){
				if(!string.IsNullOrEmpty(ex.Message)){
					if(doThrowException) {
						throw;
					}
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

		///<summary>If the Cloud Client is not running, launch it. This will also reconnect the API WebSocket if it is closed and the API is enabled.</summary>
		public static void LaunchIfNotRunning() {
			SendToODCloudClient(new ODCloudClientData(),CloudClientAction.CheckIsRunning);
		}

		public static string CheckIsRunning() {
			return SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.CheckIsRunning,doShowProgressBar:false);
		}

		///<summary>Process the api request. Parses the request data ot of requestString and sends it to the API service.</summary>
		public static void ProcessApiRequest(string requestString) {
			ODApiRequestData odApiRequestData=JsonConvert.DeserializeObject<ODApiRequestData>(requestString);
			HttpWebRequest httpWebRequest=(HttpWebRequest)WebRequest.Create(odApiRequestData.Url);
			httpWebRequest.Method=odApiRequestData.HttpMethod;
			httpWebRequest.Headers.Add("Authorization",odApiRequestData.Authorization);
			HttpWebResponse httpWebResponse=null;
			string response="Failed to communicate with the API service: ";
			string contentType="application/json";
			int statusCode=(int)HttpStatusCode.ServiceUnavailable;
			try {
				httpWebResponse=(HttpWebResponse)httpWebRequest.GetResponse();
			}
			catch(WebException ex) {
				httpWebResponse=(HttpWebResponse)ex.Response;
			}
			catch(Exception ex) {
				response+=ex.Message;
			}
			if(httpWebResponse!=null) {
				Stream stream=httpWebResponse.GetResponseStream();
				using StreamReader streamReader=new StreamReader(stream);
				response=streamReader.ReadToEnd();
				contentType=httpWebResponse.ContentType;
				statusCode=(int)httpWebResponse.StatusCode;
				streamReader.Close();
			}
			ODApiResponseData odApiResponseData=new ODApiResponseData() {
				Id=odApiRequestData.Id,
				Data=response,
				ContentType=contentType,
				StatusCode=statusCode,
			};
			SendDataToBrowser(JsonConvert.SerializeObject(odApiResponseData),(int)BrowserAction.SendToODCloudClientSocket);
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

		///<summary>For Thinfinity only, sends the data to the browser.</summary>
		public static void SendToBrowser(ODBrowserData odBrowserData,BrowserAction browserAction) {
			string data=JsonConvert.SerializeObject(odBrowserData);
			SendDataToBrowser(data,(int)browserAction);
		}

		public static string GetComputerName() {
			if(ODBuild.IsThinfinity()) {
				return SendToBrowserSynchronously("",BrowserAction.GetComputerName,doShowProgressBar:false);
			}
			return SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetComputerName,doShowProgressBar:false);
		}

		///<summary>For Thinfinity only, sends the data to the browser and waits for a response.</summary>
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

		///<summary>For AppStream only, updates the data.txt file with the data string.</summary>
		public static string SendToAppStreamSynchronously(string data,int timeoutSecs=5,bool doShowProgressBar=true) {
			string retval="";
			_hasReceivedResponse=false;
			ODCloudClientArgs args=JsonConvert.DeserializeObject<ODCloudClientArgs>(data);
			string fileName=@"data_"+args.FileIdentifier+".txt";
			string watcherPath=ODFileUtils.CombinePaths(FileWatcherDirectory,@"response");
			_responseFilePath=ODFileUtils.CombinePaths(watcherPath,fileName);
			try {
				if(!Directory.Exists(watcherPath)) {
					Directory.CreateDirectory(watcherPath);
				}
				File.WriteAllText(ODFileUtils.CombinePaths(FileWatcherDirectory,fileName),data);
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw new ODException($"Unable to access the folder {FileWatcherDirectory} for communicating with the OpenDentalCloudClient.",ODException.ErrorCodes.ODCloudClientTimeout);
			}
			DateTime start=DateTime.Now;
			string tempResponseFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),"opendental",fileName);
			void waitForResponse() {
				while(!_hasReceivedResponse && (DateTime.Now-start).TotalSeconds<timeoutSecs) {
					try {
						if(!File.Exists(_responseFilePath)) {
							Thread.Sleep(100);
							continue;
						}
						File.Move(_responseFilePath,tempResponseFilePath);//move file to local temp folder before reading it in
						_response=File.ReadAllText(tempResponseFilePath);
						_hasReceivedResponse=true;
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
					if(!_hasReceivedResponse) {
						Thread.Sleep(100);
					}
				}
			}
			if(doShowProgressBar) {
				ODProgress.ShowAction(waitForResponse);
			}
			else {
				waitForResponse();
      }
      try {
          File.Delete(_responseFilePath);
      }
      catch(Exception ex) {
          ex.DoNothing();
      }
      try {
				File.Delete(tempResponseFilePath);
      }
      catch(Exception ex) {
				ex.DoNothing();
      }
			if(!_hasReceivedResponse) {
				throw new ODException("Unable to communicate with the OpenDentalCloudClient.",ODException.ErrorCodes.ODCloudClientTimeout);
			}
			retval=_response;
			//These are static so they need to be reset for next time.
			_hasReceivedResponse=false;
			_response="";
			return retval;
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

		public static bool IsProcessRunning(string processName) {
			CloudClientAction action = CloudClientAction.IsProcessRunning;
			ODCloudClientData cloudClientData = new ODCloudClientData {
				ProcessName=processName
			};
			try {
				return Convert.ToBoolean(SendToODCloudClientSynchronously(cloudClientData,action));
			}
			catch(Exception) {
				return false;
			}
		}

		///<summary>Sends a request to the OpenDentalCloudClient to run the CloudClientAction ODCloudAuthGoogleListener to start
		///an HttpListener that listens for response from google authentication</summary>
		public static bool ODCloudAuthGoogleListener(string loopBackAddress) {
			CloudClientAction action = CloudClientAction.ODCloudAuthGoogleListener;
			ODCloudClientData cloudClientData = new ODCloudClientData {
				OtherData = loopBackAddress,
			};
			bool isStarted;
			try {
				isStarted=Convert.ToBoolean(SendToODCloudClientSynchronously(cloudClientData,action,doShowProgressBar:false));
			}
			catch(Exception) {
				isStarted=false;
			}
			return isStarted;
		}

		///<summary>Sends the GoogleAuthCodeResponseHtml and state to the OpenDenalCloudClient SendListenerResponse method to
		///send a response to Google via the HttpListenerContext.</summary>
		public static string SendListenerResponse(string data, string state) {
			CloudClientAction action = CloudClientAction.SendListenerResponse;
			ODCloudClientData cloudClientData = new ODCloudClientData {
				OtherData = data,
				State=state
			};
			string authCode="";
			try {
				authCode=SendToODCloudClientSynchronously(cloudClientData,action,timeoutSecs:300,doShowProgressBar: false);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return authCode;
		}

		//private static void FileWatcherOnChanged(object sender, FileSystemEventArgs e)	{
  //          if (e.ChangeType!=WatcherChangeTypes.Changed)	{
  //              return;
  //          }
		//	//We've had issues where the FileSystemWatcher attempts to read a file before the writer has closed it. Retry one time after waiting for a short amount of time.
		//	for(int i = 0;i<2;i++) {
		//		try {
		//			_response=File.ReadAllText(e.FullPath);
		//			_hasReceivedResponse=true;
		//			break;
		//		}
		//		catch {
		//			Thread.Sleep(50);
		//		}
		//	}
  //      }

		///<summary>Sends a request to the OpenDentalCloudClient to run the CloudClientAction GetRedirectUri to get our redirect URI</summary>
		public static string GetRedirectUri() {
			CloudClientAction action = CloudClientAction.GetRedirectUri;
			string redirectUri="";
			try {
				redirectUri=SendToODCloudClientSynchronously(new ODCloudClientData(),action,doShowProgressBar:false);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return redirectUri;
		}

		///<summary>Sends a request to the OpenDentalCloudClient to run the CloudClientAction CloseListener, to close the httplistener</summary>
		public static void CloseListener() {
			CloudClientAction action = CloudClientAction.CloseListener;
			try {
				SendToODCloudClient(new ODCloudClientData(),action);
			}
			catch(Exception) {
				return;
			}
			return;
		}

		///<summary>Sends a request to the OpenDentalCloudClient to run the CloudClientAction CheckIsListening, to check if the httplistener is listening</summary>
		public static bool CheckIsListening() {
			CloudClientAction action = CloudClientAction.CheckIsListening;
			bool isListening;
			try {
				isListening=Convert.ToBoolean(SendToODCloudClientSynchronously(new ODCloudClientData(),action,doShowProgressBar:false));
			}
			catch(Exception) {
				isListening=false;
			}
			return isListening;
		}

		///<summary>Sends a requests to ODCloudClient and waits for a response. Throws any exception that ODCloudClient returns.</summary>
		private static string SendToODCloudClientSynchronously(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction,int timeoutSecs=30
			,bool doShowProgressBar=true) 
		{
			string odCloudClientResponse=null;
			cloudClientData.DoCheckVersion=DoCheckLatestCloudClientVersion();
			cloudClientData.LatestCloudClientVersion=_latestCloudClientVersion;
			try {
				string request=GetODCloudClientRequest(cloudClientData,cloudClientAction);
				if(ODBuild.IsThinfinity()) {
					odCloudClientResponse=SendToBrowserSynchronously(request,(int)BrowserAction.SendToODCloudClient,timeoutSecs,doShowProgressBar);
				}
				else if(IsAppStream) {
					odCloudClientResponse=SendToAppStreamSynchronously(request,timeoutSecs,doShowProgressBar);
				}
			}
			catch(ODException odEx) {
				if(odEx.ErrorCodeAsEnum==ODException.ErrorCodes.BrowserTimeout || odEx.ErrorCodeAsEnum==ODException.ErrorCodes.ODCloudClientTimeout) {
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

		private static string GetODCloudClientRequest(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction,bool hasResponse=true) {
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
				isApiEnabled=IsApiEnabled,
				ActionEnum=cloudClientAction,
				FileIdentifier="",
				hasResponse=hasResponse,
			};
			if(!ODBuild.IsThinfinity() && IsAppStream) {
				jsonData.FileIdentifier=Guid.NewGuid().ToString();
			}
			return JsonConvert.SerializeObject(jsonData);
		}

		///<summary>Sends a request to ODCloudClient.</summary>
		private static void SendToODCloudClient(ODCloudClientData cloudClientData,CloudClientAction cloudClientAction) {
			if(ODBuild.IsThinfinity()) {
				if(SendDataToBrowser==null) {
					throw new ODException(nameof(SendDataToBrowser)+" has not been initialized.");
				}
			}
			cloudClientData.DoCheckVersion=DoCheckLatestCloudClientVersion();
			cloudClientData.LatestCloudClientVersion=_latestCloudClientVersion;
			string request=GetODCloudClientRequest(cloudClientData,cloudClientAction,false);
			if(ODBuild.IsThinfinity()) {
				SendDataToBrowser(request,(int)BrowserAction.SendToODCloudClient);
			}
			else if(IsAppStream) {
				ODCloudClientArgs args=JsonConvert.DeserializeObject<ODCloudClientArgs>(request);
				string fileName=@"data_"+args.FileIdentifier+".txt";
				File.WriteAllText(ODFileUtils.CombinePaths(FileWatcherDirectory, fileName),request);
			}
		}

		public static string GetMicrosoftAccessToken(string textUsername,string textRefreshToken) {
			CloudClientAction action=CloudClientAction.GetMicrosoftAccessToken;
			ODCloudClientData oDCloudClientData=new ODCloudClientData() {
				OtherData=textUsername,
				MicrosoftRefreshToken=textRefreshToken
			};
			string microsoftTokenData="";
			try {
				microsoftTokenData=SendToODCloudClientSynchronously(oDCloudClientData,action,timeoutSecs:300,doShowProgressBar:false);
			}
			catch(Exception ex) {
				ODMessageBox.Show(ex.Message);
			}
			return microsoftTokenData;
		}

		public static string MicrosoftSignOutUser(string textUsername,string textRefreshToken) {
			CloudClientAction action=CloudClientAction.MicrosoftSignOutUser;
			ODCloudClientData oDCloudClientData=new ODCloudClientData() {
				OtherData=textUsername,
				MicrosoftRefreshToken=textRefreshToken
			};
			string microsoftTokenData="";
			try {
				microsoftTokenData=SendToODCloudClientSynchronously(oDCloudClientData,action,doShowProgressBar:false);
			}
			catch(Exception ex) {
				ODMessageBox.Show(ex.Message);
			}
			return microsoftTokenData;
		}

		public static void HttpListenerGetContext() {
			CloudClientAction action=CloudClientAction.HttpListenerGetContext;
			SendToODCloudClient(new ODCloudClientData(),action);
			return;
		}

		///<summary>Close duplicate Cloud Client processes</summary>
		public static void TerminateDuplicateCloudClientProcesses() {
			CloudClientAction action=CloudClientAction.TerminateDuplicateCloudClientProcesses;
			try {
				SendToODCloudClient(new ODCloudClientData(),action);
			}
			catch(Exception) {
				return;
			}
			return;
		}

		public static bool StartSnipAndSketchOrSnippingTool(string _snipSketchURI) {
			CloudClientAction action=CloudClientAction.StartSnipAndSketchOrSnippingTool;
			ODCloudClientData oDCloudClientData=new ODCloudClientData() {
				OtherData=_snipSketchURI
			};
			try {
				return Convert.ToBoolean(SendToODCloudClientSynchronously(oDCloudClientData,action,doShowProgressBar:false));
			}
			catch(Exception) {
				return false;
			}
		}

		public static void KillProcesses() {
			try {
				SendToODCloudClient(new ODCloudClientData(),CloudClientAction.KillProcesses);
			}
			catch(Exception ex) {
				ODMessageBox.Show(ex.Message);
			}
		}

		public static bool GetProcessesSnipTool() {
			try {
				return Convert.ToBoolean(SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.GetProcessesSnipTool,doShowProgressBar:false));
			}
			catch(Exception ex) {
				ODMessageBox.Show(ex.Message);
			}
			return false;
		}

		public static void TwainAcquireBitmapStart(string twainName,bool doThrowException,bool doShowProgressBar) {
			ODCloudClientData cloudClientData=new ODCloudClientData(){
				OtherData=twainName
			};
			SendToODCloudClient(cloudClientData,CloudClientAction.TwainAcquireBitmapStart);
		}

		public static string CheckBitmapIsAcquired(){
			string bitmapStatus;
			try{
				bitmapStatus=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.CheckBitmapIsAcquired,timeoutSecs:30,doShowProgressBar:false);
			}
			catch(Exception){
				bitmapStatus="";
			}
			return bitmapStatus;
		}

		public static Bitmap TwainGetAcquiredBitmap(bool doShowProgressBar=false){
			string resultData;
			try{
				resultData=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.TwainGetAcquiredBitmap,timeoutSecs:2000,doShowProgressBar:doShowProgressBar);
			}
			catch(Exception ex){
				if(!string.IsNullOrEmpty(ex.Message)){
					//Message is empty if the user cancelled
					ODMessageBox.Show(ex.Message);
				}
				return null;
			}
			if(resultData.IsNullOrEmpty()){
				return null;
			}
			byte[] byteArray = Convert.FromBase64String(resultData);
      using MemoryStream memoryStream = new MemoryStream();
      memoryStream.Write(byteArray, 0, byteArray.Length);
      Bitmap bitmap = (Bitmap)Bitmap.FromStream(memoryStream);
      memoryStream.Flush();
      byteArray = null;
      resultData = null;
			return bitmap;
		}

		public static void TwainCloseScanner(){
			try{
				SendToODCloudClient(new ODCloudClientData(),CloudClientAction.TwainCloseScanner);
			}
			catch(Exception ex){
				ODMessageBox.Show(ex.Message);
			}
		}

		public static void ExportForAppStream(string filePath,string fileName) {
			Byte[] bytes=File.ReadAllBytes(filePath);
			string fileDataString=Convert.ToBase64String(bytes);
			ODCloudClientData oDCloudClientData=new ODCloudClientData();
			oDCloudClientData.FileData=fileDataString;
			oDCloudClientData.OtherData=fileName;
			string response=SendToODCloudClientSynchronously(oDCloudClientData,CloudClientAction.ExportFile);
			MessageBox.Show(response);
		}

		///<summary>Calls the ImportFile method on the CloudClient. Splits the response string received into file name and data. Writes a new file to the FileTransferTempPath and returns the path string of the newly written file.</summary>
		public static string ImportFileForCloud() {
			string importFile=SendToODCloudClientSynchronously(new ODCloudClientData(),CloudClientAction.ImportFile,timeoutSecs:120);
			if(importFile.IsNullOrEmpty()){
				return "";
			}
			List<string> listImportFileStrings=JsonConvert.DeserializeObject<List<string>>(importFile);
			string fileName=listImportFileStrings[0];
			byte[] byteArray=Convert.FromBase64String(listImportFileStrings[1]);
			string importPath=ODFileUtils.CombinePaths(GetFileTransferTempPath(),fileName);
			try{
				File.WriteAllBytes(importPath,byteArray);
			}
			catch(Exception ex){
				MessageBox.Show("Error importing file: "+ex.Message);
				return "";
			}
			return importPath;
		}

		///<summary>Contains the data to be sent to the browser to perfrom a browser action. Will be serialized as JSON.</summary>
		public class ODBrowserData {
			public string ElementId;
			public string Url;
			public bool IsVisible;
		}

		public class ODApiRequestData {
			public string Id;
			public string Url;
			public string HttpMethod;
			public string Authorization;
		}

		public class ODApiResponseData {
			public string Id;
			public string Data;
			public string ContentType;
			public int StatusCode;
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
			///<summary>A random 32 byte string, base64 encoded. Sent with the auth request. Google returns it with their response
			///so we can confirm that their response is for our application's request.</summary>
			public string State;
			///<summary>Any error thrown while trying to perform acquisition will be held here.</summary>
			public string MicrosoftRefreshToken;
			///<summary>String of bitmap data to be attached to Clipboard</summary>
			public string BitmapCopy;
			///<summary>Node Type int used with ImageKey for copying documents in the image module. Corresponds to EnumImageNodeType</summary>
			public int NodeType;
			///<summary>Used with NodeType for copying documents in the image module.</summary>
			public long ImageKey;

		}

		///<summary>Contains the arguments to be sent to OD Cloud Client. Will be serialized as JSON.</summary>
		public class ODCloudClientArgs {
			///<summary>This will be a JSON serialized string of <see cref="ODCloudClientData"/>.</summary>
			public string Data;
			///<summary>A signature of Data to prove this is from on Open Dental server.</summary>
			public string Signature;
			///<summary>The public key that corresponds to the private key used to sign the Signature.</summary>
			public string PublicKey;
			///<summary>True if the ODCloudClient should listen for API Service requests and relay them to the VM, otherwise false.</summary>
			public bool isApiEnabled;
			///<summary>The action that should be performed for this request.</summary>
			public int Action;
            ///<summary>The GUID key that corresponds to the request and response.</summary>
            public string FileIdentifier;
            ///<summary>True if OD is expecting a response from the ODCloudClient, otherwise false.</summary>
            public bool hasResponse;
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

		///<summary></summary>
		public class CloudNodeTypeAndKey{
			public int nodeType;
			public long imagekey;
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
			///<summary>Check the workstation's running processes</summary>
			IsProcessRunning,
			///<summary>Starts an HttpListener to listen for response from google authentication</summary>
			ODCloudAuthGoogleListener ,
			///<summary>Sends a response to Google via the HttpListenerContext.</summary>
			SendListenerResponse,
			///<summary>Returns the first Prefix of the HttpListener which should be our redirect URI.</summary>
			GetRedirectUri,
			///<summary>Closes the HttpListener if it is not null. If you close the listener but intend to use this AuthorizationRequest again,
			///you must call StartListener() again.</summary>
			CloseListener,
			///<summary>Check if HttpListener is listening on the ODCloudClient</summary>
			CheckIsListening,
			///<summary>Call this enum action if you want to create a directory on the client machine.</summary>
			CreateDirectory,
			///<summary>Call this enum action if you want to check if a directory exists on the client machine.</summary>
			CheckForDirectory,
			///<summary>Get microsoft access token on the ODCloudClient</summary>
			GetMicrosoftAccessToken,
			///<summary>Signs out the passed in user from the account information. If there was an exception it will be held within the MicrosoftTokenHelper.</summary>
			MicrosoftSignOutUser,
			///<summary>Start HttpListener.GetContext on the cloudclient</summary>
			HttpListenerGetContext,
			///<summary>Close duplicate Cloud Client processes</summary>
			TerminateDuplicateCloudClientProcesses,
			///<summary>Close duplicate Cloud Client processes</summary>
			GetProcessesSnipTool,
			///<summary>Attempts to start Snip & Sketch, then Snipping Tool on the cloud users machine. If that failseturns true if either started, false if neither did.</summary>
			StartSnipAndSketchOrSnippingTool,
			///<summary> Kill all passed-in processes, ignoring any failures</summary>
			KillProcesses,
			///<summary> Get image from clipboard. Returns empty string if no image is found. </summary>
			GetClipboardImage,
			///<summary> Clear clipboard on cloud users machine</summary>
			ClearClipboard,
			///<summary> Sends a request to the OpenDentalCloudClient to copy the System.Windows.DataObject to the clients clipboard</summary>
			CopyToClipboard,
			///<summary> Used when copying to clipboard. Gets the NodeTypeAndKey for the image to be copied.</summary>
			GetNodeTypeAndKey,
			///<summary>Sends a request to the OpenDentalCloudClient to set the client's clipboard to a string value.  Used for AppStream, since there is no browser to use.</summary>
			SetClipboardText,
			///<summary>Gets the clipboard text from the OpenDentalCloudClient.  Used for AppStream, since there is no browser to get it from.</summary>
			GetClipboardText,
			///<summary>Open Scanner UI and start monitering for user input</summary>
			TwainAcquireBitmapStart,
			///<summary>Checked if bitmap is acquired or for error messages.</summary>
			CheckBitmapIsAcquired,
			///<summary>Bitmap is ready, retrieve the bitmap</summary>
			TwainGetAcquiredBitmap,
			///<summary>Close twain source</summary>
			TwainCloseScanner,
			///<summary>Receive file from Cloud session and place it in downloads folder.</summary>
			ExportFile,
			///<summary>Imports a file from the user's workstation</summary>
			ImportFile,
			///<summary>Used during copy/paste in Imaging Module to prevent issues when copying images across different databases</summary>
			GetDbNameOrUriFromClipboard,
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
			///<summary>Pass the data on to ODCloudClient via the websocket. Used for API traffic.</summary>
			SendToODCloudClientSocket,
			///<summary>Check if ODCloudClient is running via the browser and ask if they want to download or launch</summary>
			CheckODCloudClientViaBrowser,
			//Relaunch the cloudclient from the browser
			RelaunchODCloudClientViaBrowser
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
