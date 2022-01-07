using CodeBase;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ServiceManager {
	public partial class FormServiceManage:Form {
		///<summary>Indicates if a service was successfully installed while the service manager was showing.</summary>
		public bool HadServiceInstalled=false;
		private bool _isInstallOnly=false;
		private bool _isNew;

		private FileInfo _serviceFileInfo {
			get {
				if(File.Exists(textPathToExe.Text)) {
					return new FileInfo(textPathToExe.Text);
				}
				return null;
			}
		}

		///<summary>Pass in empty string to create a new service. Pass in OpenDent string to manage an existing service.</summary>
		public FormServiceManage(string serviceName,bool isInstallOnly,bool isNew) {
			InitializeComponent();
			textName.Text=serviceName;
			textPathToExe.Text=Directory.GetCurrentDirectory();
			_isInstallOnly=isInstallOnly;
			_isNew=isNew;
		}

		private void FormServiceManager_Load(object sender,EventArgs e) {
			if(_isNew) {
				return;//Don't do validation if we're adding a new service via the "Add" button.
			}
			RefreshFormData();
		}

		private void RefreshFormData() {
			ServiceController service=ServicesHelper.GetOpenDentServiceByName(textName.Text);
			if(service!=null) {//installed
				RegistryKey hklm=Registry.LocalMachine;
				hklm=hklm.OpenSubKey(@"System\CurrentControlSet\Services\"+service.ServiceName);
				textPathToExe.Text=hklm.GetValue("ImagePath").ToString().Replace("\"","");
				textStatus.Text="Installed";
				butInstall.Enabled=false;
				butUninstall.Enabled=true;
				butBrowse.Enabled=false;
				textPathToExe.ReadOnly=true;
				textName.ReadOnly=true;
				if(service.Status==ServiceControllerStatus.Running) {
					textStatus.Text+=", Running";
					butStart.Enabled=false;
					butStop.Enabled=true;
				}
				else {
					textStatus.Text+=", Stopped";
					butStart.Enabled=true;
					butStop.Enabled=false;
				}
			}
			else {
				textStatus.Text="Not installed";
				textName.ReadOnly=false;
				textPathToExe.ReadOnly=false;
				butInstall.Enabled=true;
				butUninstall.Enabled=false;
				butStart.Enabled=false;
				butStop.Enabled=false;
			}
			if(_isInstallOnly) {
				butUninstall.Enabled=false;
			}
		}

		private void butInstall_Click(object sender,EventArgs e) {
			if(_serviceFileInfo==null) {
				MessageBox.Show("Select a valid service path");
				return;
			}
			string serviceName=textName.Text;
			if(serviceName.Length<8 || serviceName.Substring(0,8)!="OpenDent") {
				MessageBox.Show("Error.  Service name must begin with \"OpenDent\".");
				return;
			}
			if(ServicesHelper.HasService(serviceName,_serviceFileInfo)) {
				MessageBox.Show("Error.  Either a service with this name is already installed or there is another service installed from this directory.");
				return;
			}
			if(_serviceFileInfo.Name=="OpenDentalEConnector.exe"
				|| _serviceFileInfo.Name=="OpenDentalService.exe" 
				|| _serviceFileInfo.Name=="OpenDentalReplicationService.exe")
			{
				using FormWebConfigSettings FormWCS=new FormWebConfigSettings(_serviceFileInfo);
				FormWCS.ShowDialog();
				if(FormWCS.DialogResult!=DialogResult.OK) {
					return;
				}
			}
			try {
				string standardOutput;
				int exitCode;
				ServicesHelper.Install(serviceName,_serviceFileInfo,out standardOutput,out exitCode);
				if(exitCode!=0) {
					MessageBox.Show("Error. Exit code: "+exitCode+"\r\n"+standardOutput.Trim());
				}
			}
			catch(Exception ex) {
				MessageBox.Show("Unexpected error installing the service:\r\n"+ex.Message);
			}
			ServiceController service=null;
			try {
				service=ServicesHelper.GetServiceByServiceName(serviceName);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			if(service!=null) {
				HadServiceInstalled=true;//We verified that the service was successfully installed
				//Try to grant access to "Everyone" so that the service can be stopped and started by all users.
				try {
					ServicesHelper.SetSecurityDescriptorToAllowEveryoneToManageService(service);
				}
				catch(Exception ex) {
					MessageBox.Show("The service was successfully installed but there was a problem updating the permissions for managing the service."
						+"\r\nThe service may have to be manually stopped and started via an administrative user."
						+"\r\nThis can be cumbersome when updating to newer versions of the software."
						+"\r\n\r\n"+ex.Message);
				}
			}
			RefreshFormData();
		}

		private void butUninstall_Click(object sender,EventArgs e) {
			if(_serviceFileInfo==null) {
				MessageBox.Show("Selected service has an invalid path");
				return;
			}
			try {
				string standardOutput;
				int exitCode;
				ServicesHelper.Uninstall(textName.Text,out standardOutput,out exitCode);
				if(exitCode!=0) {
					MessageBox.Show("Error. Exit code: "+exitCode+"\r\n"+standardOutput.Trim());
					return;
				}
			}
			catch(Exception ex) {
				MessageBox.Show("Unexpected error uninstalling the service:\r\n"+ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butStart_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			try {
				ServicesHelper.Start(textName.Text,true);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
			}
			Cursor=Cursors.Default;
			RefreshFormData();
		}

		private void butStop_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			try {
				ServicesHelper.Stop(textName.Text,true);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
			}
			Cursor=Cursors.Default;
			RefreshFormData();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshFormData();
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			OpenFileDialog fdlg=new OpenFileDialog();
			fdlg.Title="Select a Service";
			fdlg.InitialDirectory=Directory.GetCurrentDirectory();
			fdlg.Filter="Executable files(*.exe)|*.exe";
			fdlg.RestoreDirectory=true;
			if(fdlg.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textPathToExe.Text=fdlg.FileName;
		}
	}
}