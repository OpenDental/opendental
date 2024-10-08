using System;
using System.Collections.Generic;
using System.Diagnostics;
using CodeBase;
using OpenDentBusiness;
using System.IO;
using static CodeBase.ODCloudClient;

namespace OpenDental {
	public class CloudClientL
	{
		///<summary>Prompt the user to launch, or download and install, the OpenDentalCloudClient</summary>
		private static PromptSelections PromptODCloudClientInstall(bool isCheckingCloudClientResponseOnly=false) {
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
			if(response!="false") {
				return PromptSelections.ClientRunning;
			}
			string message=Lans.g("CloudClient","Please launch (download and install if missing) the OpenDentalCloudClient program and try again.");
			InputBox inputBox=new InputBox(message,new List<string>() { "Launch","Download" },0);
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return PromptSelections.Cancel;
			}
			int selectedIndex=inputBox.SelectedIndex;
			if(selectedIndex==1) {
				Process.Start("https://www.opendental.com/resources/ODCloudClientInstaller.msi");
				return PromptSelections.Download;
			}
			return PromptSelections.Launch;
		}

		///<summary>PromptODCloudClientInstall helper method for prompting the user to launch, or download and install, the OpenDentalCloudClient</summary>
		public static bool IsCloudClientRunning() {
			if(ODBuild.IsThinfinity()) {
				PromptSelections promptSelections=PromptODCloudClientInstall();
				if(promptSelections==PromptSelections.Cancel || promptSelections==PromptSelections.Download) {
					return false;
				}
				if(promptSelections==PromptSelections.Launch) {
					try {
						string response=SendToBrowserSynchronously("",BrowserAction.RelaunchODCloudClientViaBrowser,timeoutSecs:15);
					}
					catch(Exception) {
						MsgBox.Show("ODCloudClient","ODCloudClient did not respond, please ensure that the ODCloudClient is installed.");
						return false;
					}
					if(PromptODCloudClientInstall(true)==PromptSelections.NoResponse) {
						MsgBox.Show("ODCloudClient","ODCloudClient did not respond, please ensure that the ODCloudClient is installed.");
						return false;
					}
				}
				//close duplicate cloud client processes
				//TerminateDuplicateCloudClientProcesses();
				return true;
			}
			else if(IsAppStream) {
				try {
					//Verify OD has a connection to the extension
					if (!CodeBase.Utilities.ODCloudDcvExtension.Instance.IsConnected()){
						throw new ODException();
					}
					DateTime lastRequest=CodeBase.Utilities.ODCloudDcvExtension.Instance.GetLastRequest();
					if((DateTime.Now-lastRequest)>TimeSpan.FromMinutes(30)) {
						string response=CheckIsRunning();
						if (response.IsNullOrEmpty()) {
							throw new Exception();
						}
					}
				}
				catch(ODException ex) {
					ex.DoNothing();
					MsgBox.Show("ODCloudClient","Unable to access the dcv client for communicating with the OpenDentalCloudClient.");
					return false;
				}
				catch(Exception ex) {
					ex.DoNothing();
					MsgBox.Show("ODCloudClient","The ODCloudClient did not respond, please ensure it is installed and running or some features of Open Dental will not be available.");
					return false;
				}
				return true;
			}
			return false;
		}

		public static void ExportForCloud(string filePath,bool doPromptForName=true) {
			string fileName=Path.GetFileName(filePath);
			string origExt=Path.GetExtension(filePath);
			if(doPromptForName) {
				InputBox inputBox=new InputBox("Enter file name:\r\nExample: \"PaymentsReport.xls\", \"ProcedureCode.xml\"",fileName);
				inputBox.ShowDialog();
				if(!inputBox.IsDialogOK) {
					return;
				}
				string fileNameInput=inputBox.StringResult;
				if(!fileNameInput.IsNullOrEmpty()) {
					fileName=ODFileUtils.CleanFileName(fileNameInput);
					if(Path.GetExtension(fileNameInput).IsNullOrEmpty()) {
						fileName+=origExt;
					}
				}
			}
			ODCloudClient.ExportForAppStream(filePath,fileName);
		}

		private enum PromptSelections {
			Launch,
			Download,
			ClientRunning,
			NoResponse,
			Cancel
		}
	}
}
