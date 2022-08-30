using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using CodeBase;

namespace OpenDentHL7 {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args) {
			if(ODBuild.IsDebug()) {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new FormDebug("OpenDentHL7"));
			}
			else {
				EventLog.WriteEntry("OpenDentHL7.Main", DateTime.Now.ToLongTimeString() +" - Service main method starting...");
				ServiceHL7 serviceHL7=new ServiceHL7();
				serviceHL7.ServiceName="OpenDentalHL7";//default
				//Get the executing assembly location directory (location of this OpenDentHL7.exe)
				string executingDir=Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				//Get all installed services
				List<ServiceController> serviceControllersOD=new List<ServiceController>();
				ServiceController[] serviceControllersAll=ServiceController.GetServices();
				//Get all installed services that have names that start with "OpenDent"
				for(int i=0;i<serviceControllersAll.Length;i++) {
					if(serviceControllersAll[i].ServiceName.StartsWith("OpenDent")) {
						serviceControllersOD.Add(serviceControllersAll[i]);
					}
				}
				string pathToODHL7exe;
				//Get the service that is installed from the same directory as the current directory
				for(int i=0;i<serviceControllersOD.Count;i++) {
					RegistryKey hklm=Registry.LocalMachine;
					hklm=hklm.OpenSubKey(@"System\CurrentControlSet\Services\"+serviceControllersOD[i].ServiceName);
					pathToODHL7exe=hklm.GetValue("ImagePath").ToString();
					pathToODHL7exe=pathToODHL7exe.Replace("\"","");
					pathToODHL7exe=Path.GetDirectoryName(pathToODHL7exe);
					if(pathToODHL7exe==executingDir) {
						//Set the name of the service to run as the name of the service installed from this directory
						serviceHL7.ServiceName=serviceControllersOD[i].ServiceName;
						break;
					}
				}
				ServiceBase.Run(serviceHL7);
				EventLog.WriteEntry("OpenDentHL7.Main",DateTime.Now.ToLongTimeString() +" - Service main method exiting...");
			}
		}
	}
}
