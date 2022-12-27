using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using static CodeBase.ODCloudClient;

namespace OpenDental
{
	public class CloudClientL
	{
		///<summary>Prompt the user to launch, or download and install, the OpenDentalCloudClient</summary>
		public static PromptSelections PromptODCloudClientInstall(bool isCheckingCloudClientResponseOnly=false) {
			string response="false";
			try {
				response=SendToBrowserSynchronously("",BrowserAction.CheckODCloudClientViaBrowser,doShowProgressBar: false);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			if(isCheckingCloudClientResponseOnly) {
				if(response=="false") {
					return PromptSelections.NoResponse;
				}
				return PromptSelections.ClientRunning;
			}
			if(response=="false") {
				string message=Lan.g("CloudClient","Please launch (download and install if missing) the OpenDentalCloudClient program and try again.");
				InputBox inputBox=new InputBox(message,new List<string>() { "Launch","Download" },0);
				if(inputBox.ShowDialog()==DialogResult.Cancel) {
					return PromptSelections.Cancel;
				}
				int selectedIndex=inputBox.SelectedIndex;
				if(selectedIndex==1) {
					Process.Start("https://www.opendental.com/resources/ODCloudClientInstaller.msi");
					return PromptSelections.Download;
				}
				else {
					return PromptSelections.Launch;
				}
			}
			return PromptSelections.ClientRunning;
		}

		///<summary>PromptODCloudClientInstall helper method for prompting the user to launch, or download and install, the OpenDentalCloudClient</summary>
		public static bool IsCloudClientRunning() {
			CloudClientL.PromptSelections promptSelections=CloudClientL.PromptODCloudClientInstall();
			if(promptSelections==CloudClientL.PromptSelections.Cancel || promptSelections==CloudClientL.PromptSelections.Download) {
				return false;
			}
			else if(promptSelections==CloudClientL.PromptSelections.Launch) {
				try {
					string response=ODCloudClient.SendToBrowserSynchronously("",ODCloudClient.BrowserAction.RelaunchODCloudClientViaBrowser,timeoutSecs:6);
				}
				catch(Exception) {
					MsgBox.Show("ODCloudClient","ODCloudClient did not respond, please ensure that the ODCloudClient is installed.");
					return false;
				}
				if(CloudClientL.PromptODCloudClientInstall(true)==CloudClientL.PromptSelections.NoResponse) {
					MsgBox.Show("ODCloudClient","ODCloudClient did not respond, please ensure that the ODCloudClient is installed.");
					return false;
				}
			}
			return true;
		}

		public enum PromptSelections {
			Launch,
			Download,
			ClientRunning,
			NoResponse,
			Cancel
		}
	}
}
