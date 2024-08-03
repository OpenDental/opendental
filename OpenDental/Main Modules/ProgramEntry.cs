using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using CodeBase;
using System.Threading;
using OpenDentBusiness;
using Microsoft.Win32;
using DataConnectionBase;
using OpenDental.Main_Modules;

namespace OpenDental {
	static class ProgramEntry {
		[STAThread]
		static void Main(string[] args) {
			//Application.EnableVisualStyles() uses version 6 of comctl32.dll instead of version 5.
			//See https://support.microsoft.com/en-us/topic/system-accessviolationexception-occurs-with-tooltips-in-windows-forms-application-71a775b2-8a03-6846-f810-76930766cda0?ui=en-us&rs=en-us&ad=us
			//See also http://stackoverflow.com/questions/8335983/accessviolationexception-on-tooltip-that-faults-comctl32-dll-net-4-0
			Application.EnableVisualStyles();//This line fixes rare AccessViolationExceptions for ToolTips on our ValidDate boxes, ValidDouble boxes, etc...
			Application.SetCompatibleTextRenderingDefault(false);//designer uses new text rendering.  This makes the exe use matching text rendering.  Before this was added, it was common for labels to be longer in the running program than they were in the designer.
			if(ODBuild.IsWeb()) {
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
				//Exits OD gracefully if the user closes the browser or navigates away
				Web.OnCloseHandler=FormOpenDental.S_ProcessKillCommand;
				//allows open dental to run in a web browser
				Web.Start(); 
				//allows open dental to send data to the browser
				ODCloudClient.SendDataToBrowser=OpenDental.Thinfinity.Browser.SendData;
				//allows open dental to get the latest cloud client version from HQ
				ODCloudClient.GetLatestCloudClientVersion=WebServiceMainHQProxy.GetLatestCloudClientVersion;
				//allows open dental to process API requests from the Cloud Client
				OpenDental.Thinfinity.Browser.ProcessApiRequest=ODCloudClient.ProcessApiRequest;
			}
			try {
				ODInitialize.Initialize();
				Security.CurComputerName=ODEnvironment.MachineName;
			}
			catch(Exception e) {
				FriendlyException.Show("Critical Error: "+e.Message,e,isUnhandledException:true);
				return;
			}
			//Register an EventHandler which handles unhandled exceptions.
			//AppDomain.CurrentDomain.UnhandledException+=new UnhandledExceptionEventHandler(OnUnhandeledExceptionPolicy);
			bool isSecondInstance=false ;//or more.
			Process[] processes=Process.GetProcesses();
			for(int i=0;i<processes.Length;i++) {
				if(processes[i].Id==Process.GetCurrentProcess().Id) {
					continue;
				}
				//we have to do it this way because during debugging, the name has vshost tacked onto the end.
				if(processes[i].ProcessName.StartsWith("OpenDental")) {
					isSecondInstance=true;
					break;
				}
			}
			/*
			if(args.Length>0) {//if any command line args, then we will attempt to reuse an existing OD window.
				if(isSecondInstance){
					FormCommandLinePassOff formCommandLine=new FormCommandLinePassOff();
					formCommandLine.CommandLineArgs=new string[args.Length];
					args.CopyTo(formCommandLine.CommandLineArgs,0);
					Application.Run(formCommandLine);
					return;
				}
			}*/
			string[] commandLineArgs=new string[args.Length];
			args.CopyTo(commandLineArgs,0);
			//Minimum version for dpi support is Windows 10, version 1607, which is the same as build 15063.
			//Windows recommends checking the specific windows version:
			//https://docs.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms?view=netframeworkdesktop-4.8
			//But checking versions violates many other MS guidelines that recommend only checking feature support instead of versions.
			//Also, this page cryptically mentions that no server version supports high dpi. Checking versions would not catch that nuance:
			//https://docs.microsoft.com/en-us/windows/win32/hidpi/dpi-awareness-context
			//So we will only test if the feature is supported.
			if(!Dpi.SupportsHighDpi()){
				Dpi.SetUnaware();
			}
			string appDir=Application.StartupPath;
			if(File.Exists(Path.Combine(appDir,"NoDpi.txt"))){
				Dpi.SetUnaware();
				FormODBase.IsDpiSystem=true;
			}
			if(File.Exists(Path.Combine(appDir,"NoCustomBorders.txt"))){
				FormODBase.AreBordersMS=true;
				FormFrame.AreBordersMS=true;
			}
			//In Win10, this registry entry is for 'Let Windows try to fix apps so they're not blurry'.
			//In Win11, behavior is true, but the registry entry is gone and there is no option to change it.
			//So starting with OD 23.1, we will turn this option on for Win 10 users for consistency.
			RegistryKey registryKey=Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop",RegistryKeyPermissionCheck.ReadWriteSubTree);
			object objectVal=registryKey.GetValue("EnablePerProcessSystemDPI");
			if(!ODBuild.IsDebug() && objectVal!=null) {
				int intVal=(int)objectVal;
				if(intVal==0){
					try{
						registryKey.SetValue("EnablePerProcessSystemDPI",1);
					}
					catch{
						MessageBox.Show("Unable to change registry entry.  You should go to Windows Settings, System, Display. Click on Advanced scaling settings about halfway down in blue.  Uncheck the option for 'Let Windows try to fix apps so they're not blurry'.");
						Application.Exit();
						return;
					}
					MessageBox.Show("Registry updated to EnablePerProcessSystemDPI. Please restart Open Dental.");
					Application.Exit();
					return;
				}
			}
			FormOpenDental formOpenDental=new FormOpenDental(commandLineArgs);
			Exception submittedException=null;
			Action<Exception,string> actionUnhandled=new Action<Exception,string>((e,threadName) => {
				//Try to automatically submit a bug report to HQ.
				string displayMsg="";
				try {
					//We want to submit a maximum of one exception per instance of OD.
					if(submittedException==null) {
						submittedException=e;
						BugSubmissions.SubmitException(e,out displayMsg,threadName,FormOpenDental.PatNumCur,formOpenDental.GetSelectedModuleName());
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				FriendlyException.Show((displayMsg.IsNullOrEmpty()) ? "Critical Error: "+e.Message : displayMsg,e,isUnhandledException:true);
				formOpenDental.ProcessKillCommand();
			});
			CodeBase.ODThread.RegisterForUnhandledExceptions(formOpenDental,actionUnhandled);
			formOpenDental.IsSecondInstance=isSecondInstance;
			Application.AddMessageFilter(new ODGlobalUserActiveHandler());
			Application.ThreadException+=new ThreadExceptionEventHandler((object s,ThreadExceptionEventArgs e) => {
				actionUnhandled(e.Exception,"ProgramEntry");
			});
            //OpenDentalCloud.dll references Dropbox.Api.dll which references Newtonsoft.Json.dll version 7.0.0.0. Sometimes it also says it can't find 
            //9.0.0.0.

            try  // Print a better message of ODSMS is unavailable.
            {
                if (ODSMS.USE_ODSMS)  // Check if the module is enabled
                {
                    // Empty block, do nothing.  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ODSMS module is not initialized or an error occurred while accessing it. Please check your VPN connection. The program is about to crash.");
				MsgBox.Show("Please check your VPN connection");
                throw new ApplicationException("ODSMS module is not initialized or an error occurred while accessing it. Please check your VPN connection.", ex);
            }

            if (ODSMS.USE_ODSMS)  // Check The module is enabled
			{
                OpenDental.Main_Modules.AsyncSMSHandling.InitializeAsyncSMSHandling();

                if (!ODSMS.DEBUG_NUMBER.IsNullOrEmpty()) // Debug number is set.  We're running in debug mode
				{
                    MsgBox.Show("DEBUG MODE!!");

                    // Process the simulated SMS
                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        await OpenDental.Main_Modules.AsyncSMSHandling.WaitForDatabaseAndUserInitialization();

                        // await OpenDental.Main_Modules.AsyncSMSHandling.processOneReceivedSMS(debugMsgText, debugMsgTime, debugMsgFrom, debugMsgGUID);
                    });

                    System.Threading.Tasks.Task.Run(() => OpenDental.Main_Modules.AsyncSMSHandling.receiveSMSforever());
                    System.Threading.Tasks.Task.Run(() => OpenDental.Main_Modules.AsyncSMSHandling.SMSDailyTasks());
                }
                else if (ODSMS.RUN_SCHEDULED_TASKS) // True if this is the computer that actually does the work
				{
                    MsgBox.Show("This computer will send/receive SMS");
                    System.Threading.Tasks.Task.Run(() => OpenDental.Main_Modules.AsyncSMSHandling.receiveSMSforever());
                    System.Threading.Tasks.Task.Run(() => OpenDental.Main_Modules.AsyncSMSHandling.SMSDailyTasks());
                }
            }

            ODInitialize.FixPackageAssembly("Newtonsoft.Json",ODFileUtils.CombinePaths(AppDomain.CurrentDomain.BaseDirectory,"Newtonsoft.Json.dll"));
			if(commandLineArgs.Any(x => x.ToLower()=="issilentupdate=true")) {
				formOpenDental.FormOpenDentalShown();//Form never shows. It returns out after not too long.
				//This prevents zombie process when drawing icons in Direct2D EnumContext.Graphics.
			}
			else {
				Application.Run(formOpenDental);
			}
		}
	}
}
