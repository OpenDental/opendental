using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase {
	///<summary>This is a dummy class that exists solely so that UpdateFileCopier does not have to link to the real CodeBase.ODCloudClient so that
	///UpdateFileCopier does not need a reference to Newtonsoft.Json.</summary>
	public class ODCloudClient {
		public static SendDataToBrowserDelegate SendDataToBrowser;
		public delegate void SendDataToBrowserDelegate(string data,int browserAction,Action<string> onReceivedResponse = null);

		public static void LaunchFileWithODCloudClient(string exePath="",string extraArgs="",string extraFilePath="",string extraFileData="",
			string extraFileType="",bool doWaitForResponse=false,string createDirIfNeeded="") 
		{

		}

		public static string SendToBrowserSynchronously(string data,BrowserAction browserAction,int timeoutSecs=5) {
			return "";
		}

		public static string[] GetClipboardFilesFromODCloudClient() {
			return null;
		}

		///<summary>Tells the browser what action to take with the data passed to it.</summary>
		public enum BrowserAction {
			///<summary>Pass the data on to ODCloudClient</summary>
			SendToODCloudClient,
			///<summary>Write the data to the user's clipboard.</summary>
			SetClipboard,
			///<summary>Get the contents of the user's clipboard as text.</summary>
			GetClipboardText,
			///<summary>Get the contents of the user's clipboard as an image.</summary>
			GetClipboardImage,
		}
	}
}
