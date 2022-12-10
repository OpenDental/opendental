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
				string message=@"It appears OpenDentalCloudClient is not running. Please launch the OpenDentalCloudClient program and try again, download and install first if missing.";
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
		public enum PromptSelections {
			Launch,
			Download,
			ClientRunning,
			NoResponse,
			Cancel
		}
	}
}
