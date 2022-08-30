using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDentServer {
	[RunInstallerAttribute(true)]
	public class MyProjectInstaller:Installer {
		private ServiceInstaller serviceInstaller1;
		private ServiceProcessInstaller processInstaller;

		public MyProjectInstaller() {
			processInstaller = new ServiceProcessInstaller();
			serviceInstaller1 = new ServiceInstaller();
			processInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller1.StartType = ServiceStartMode.Automatic;
			serviceInstaller1.ServiceName="OpenDentHL7";
			string[] args=Environment.GetCommandLineArgs();
			for(int i=0;i<args.Length;i++) {
				if(args[i].StartsWith("/ServiceName")) {
					serviceInstaller1.ServiceName=args[i].Substring(13);
				}
			}
			Installers.Add(serviceInstaller1);
			Installers.Add(processInstaller);
		}
	}
}
