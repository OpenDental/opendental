using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Xml;

namespace CodeBase {
	///<summary>This is a helper class meant to be used to easily manage Windows services.</summary>
	public class ServicesHelper {
		///<summary>[CurrentDirectory]/InstallUtil/installutil.exe</summary>
		private static string _installUtilPath=Path.Combine(Directory.GetCurrentDirectory(),"InstallUtil","installutil.exe");

		///<summary>Executes a process and waits up to 10 seconds for the process to execute.</summary>
		private static void ExecuteProcess(string fileName,string arguments,out string standardOutput,out int exitCode,string workingDirectory="") {
			Process process=new Process();
			if(!string.IsNullOrEmpty(workingDirectory)) {
				process.StartInfo.WorkingDirectory=workingDirectory;
			}
			process.StartInfo.FileName=fileName;
			process.StartInfo.Arguments=arguments;
			process.StartInfo.UseShellExecute=false;
			process.StartInfo.RedirectStandardOutput=true;
			process.Start();
			standardOutput=process.StandardOutput.ReadToEnd();
			process.WaitForExit(10000);
			exitCode=process.ExitCode;
		}

		///<summary>Returns true if the service was installed successfully.</summary>
		public static bool Install(string serviceName,FileInfo serviceFileInfo) {
			try {
				string standardOutput;
				int exitCode;
				Install(serviceName,serviceFileInfo,out standardOutput,out exitCode);
				//Check to see if the service was successfully installed.
				ServiceController serviceController=GetServiceByServiceName(serviceName);
				if(serviceController!=null) {
					//Attempt to give Everyone permission to manage the newly installed service.  Call the method that takes a list so no exception is thrown.
					SetSecurityDescriptorToAllowEveryoneToManageServices(new List<ServiceController>() { serviceController });
					return true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return false;
		}

		///<summary>Utilizes "installutil.exe" to install the serviceFileInfo with the corresponding serviceName passed in.
		///The out parameters "standardOutput" and "exitCode" will contain the results of the "Process" execution.</summary>
		public static void Install(string serviceName,FileInfo serviceFileInfo,out string standardOutput,out int exitCode) {
			//Use the Windows Service Controller to show the "security descriptor" for the current service via a command prompt.
			ExecuteProcess(_installUtilPath
				,"/ServiceName="+serviceName+" \""+serviceFileInfo.FullName+"\""
				,out standardOutput
				,out exitCode
				,serviceFileInfo.DirectoryName);
		}

		///<summary>Returns true if the service was able to uninstall successfully.</summary>
		public static bool Uninstall(ServiceController service) {
			try {
				string standardOutput;
				int exitCode;
				Uninstall(service.ServiceName,out standardOutput,out exitCode);
				//Check to see if the service was successfully removed.
				return (GetServiceByServiceName(service.ServiceName)!=null);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return false;
		}

		///<summary>Utilizes "installutil.exe" to uninstall the serviceFileInfo with the corresponding serviceName passed in.
		///Uses the local machines registry in order to find the corresponding service that shares the serviceName passed in.
		///The out parameters "standardOutput" and "exitCode" will contain the results of the "Process" execution.</summary>
		public static void Uninstall(string serviceName,out string standardOutput,out int exitCode) {
			RegistryKey hklm=Registry.LocalMachine;
			hklm=hklm.OpenSubKey(@"System\CurrentControlSet\Services\"+serviceName);
			string imagePath=hklm.GetValue("ImagePath").ToString().Replace("\"","");
			FileInfo serviceFile=new FileInfo(imagePath);
			ExecuteProcess(_installUtilPath
				,"/u /ServiceName="+serviceName+" \""+serviceFile.FullName+"\""
				,out standardOutput
				,out exitCode
				,serviceFile.DirectoryName);
		}

		///<summary>Returns true if the service was able to start successfully.  Set hasExceptions to true if the exception is desired instead.</summary>
		public static bool Start(string serviceName,bool hasExceptions=false) {
			return Start(GetServiceByServiceName(serviceName),hasExceptions);
		}

		///<summary>Returns true if the service was able to start successfully.  Set hasExceptions to true if the exception is desired instead.
		///Set timeoutSeconds to the number of seconds that the service controller should wait for the service to report 'Running'.</summary>
		public static bool Start(ServiceController service,bool hasExceptions=false,int timeoutSeconds=7) {
			try {
				//Only attempt to start services that are of status stopped or stop pending.
				//If we do not do this, an InvalidOperationException will throw that says "An instance of the service is already running"
				if(service.Status!=ServiceControllerStatus.Stopped && service.Status!=ServiceControllerStatus.StopPending) {
					return true;
				}
				service.MachineName=Environment.MachineName;
				service.Start();
				service.WaitForStatus(ServiceControllerStatus.Running,TimeSpan.FromSeconds(timeoutSeconds));
			}
			catch(Exception ex) {
				if(hasExceptions) {
					throw ex;
				}
				return false;
			}
			return true;
		}

		///<summary>Starts all services passed in.
		///If hasExceptions is set to false then a string will be returned indicating which services did not start.  Empty string if all started.
		///If hasExceptions is set to true, there is a chance that not all services will start.</summary>
		public static string StartServices(List<ServiceController> listServices,bool hasExceptions=false) {
			StringBuilder stringBuilderErrors=new StringBuilder();
			foreach(ServiceController service in listServices) {
				try {
					if(!Start(service,hasExceptions)) {
						stringBuilderErrors.AppendLine(service.DisplayName);
					}
				}
				catch(Exception ex) {
					if(hasExceptions) {
						throw ex;
					}
					stringBuilderErrors.AppendLine(service.DisplayName);
				}
			}
			return stringBuilderErrors.ToString();
		}

		///<summary>Returns true if the service was able to stop successfully.  Set hasExceptions to true if the exception is desired instead.</summary>
		public static bool Stop(string serviceName,bool hasExceptions=false) {
			return Stop(GetServiceByServiceName(serviceName),hasExceptions);
		}

		///<summary>Returns true if the service was able to stop successfully.  Set hasExceptions to true if the exception is desired instead.</summary>
		public static bool Stop(ServiceController service,bool hasExceptions=false,int timeoutSeconds=7) {
			try {
				if(service.Status==ServiceControllerStatus.Stopped || service.Status==ServiceControllerStatus.StopPending) {
					return true;//Already stopped.  Calling Stop again will guarantee an exception is thrown.  Do not try calling Stop on a stopped service.
        }
        service.MachineName=Environment.MachineName;
        service.Stop();
				service.WaitForStatus(ServiceControllerStatus.Stopped,TimeSpan.FromSeconds(timeoutSeconds));
			}
			catch(Exception ex) {
				if(hasExceptions) {
					throw ex;
				}
				return false;
			}
			return true;
		}

		///<summary>Stops all services passed in.
		///If hasExceptions is set to false then a string will be returned indicating which services did not stop.  Empty string if all stopped.
		///If hasExceptions is set to true, there is a chance that not all services will be stopped.</summary>
		public static string StopServices(List<ServiceController> listServices,bool hasExceptions=false) {
			StringBuilder stringBuilderErrors=new StringBuilder();
			foreach(ServiceController service in listServices) {
				try {
					if(!Stop(service,hasExceptions)) {
						stringBuilderErrors.AppendLine(service.DisplayName);
					}
				}
				catch(Exception ex) {
					if(hasExceptions) {
						throw ex;
					}
					stringBuilderErrors.AppendLine(service.DisplayName);
				}
			}
			return stringBuilderErrors.ToString();
		}

		///<summary>Returns a list of all services.</summary>
		public static List<ServiceController> GetServices() {
			return ServiceController.GetServices().ToList();
		}

		///<summary>Returns a list of all services that have a registry entry where the ImagePath is pointing to the executable name passed in.
		///Computers can have mulitple services installed each named uniquely but all pointing to the same executable or different versions of the same
		///executable.  Sometimes an action needs to be taken on all services that share an exe.  E.g. shutting down all eConnectors.</summary>
		public static List<ServiceController> GetServicesByRegistryImagePath(string serviceExeName) {
			List<ServiceController> listServices=GetServices();
			List<ServiceController> listMatchingServices=new List<ServiceController>();
			foreach(ServiceController service in listServices) {
				RegistryKey hklm=Registry.LocalMachine;
				hklm=hklm.OpenSubKey(@"System\CurrentControlSet\Services\"+service.ServiceName);
				string[] arrayExePath=hklm.GetValue("ImagePath").ToString().Replace("\"","").Split('\\');
				//This will not work if in the future we allow command line args for the listener service that include paths.
				if(arrayExePath[arrayExePath.Length-1].StartsWith(serviceExeName)) {
					listMatchingServices.Add(service);
				}
			}
			return listMatchingServices;
		}

		///<summary>Returns the service with the specified name.  Returns null if not found.</summary>
		public static ServiceController GetServiceByServiceName(string serviceName,bool isCaseSensitive=true,bool canPartialMatch=false) {
			if(isCaseSensitive && canPartialMatch) {
				return GetServices().FirstOrDefault(x => x.ServiceName.Contains(serviceName));
			}
			else if(isCaseSensitive && !canPartialMatch) {
				return GetServices().FirstOrDefault(x => x.ServiceName==serviceName);
			}
			else if(!isCaseSensitive && canPartialMatch) {
				return GetServices().FirstOrDefault(x => x.ServiceName.ToLower().Contains(serviceName.ToLower()));
			}
			else if(!isCaseSensitive && !canPartialMatch) {
				return GetServices().FirstOrDefault(x => x.ServiceName.ToLower()==serviceName.ToLower());
			}
			return null;
		}

		///<summary>Returns all services that start with "OpenDent".</summary>
		public static List<ServiceController> GetAllOpenDentServices() {
			return GetServices().FindAll(x => x.ServiceName.StartsWith("OpenDent"));
		}

		public static ServiceController GetOpenDentServiceByName(string serviceName) {
			return GetAllOpenDentServices().FirstOrDefault(x => x.ServiceName==serviceName);
		}

		///<summary>Returns all services that their "Path to executeable" contains the passed in executable name.  Throws exceptions.</summary>
		///<param name="exeName">E.g. OpenDentalCustListener.exe</param>
		public static List<ServiceController> GetServicesByExe(string exeName) {
			RegistryKey hklm;
			List<ServiceController> retVal=new List<ServiceController>();
			List<ServiceController> listServices=GetServices();
			foreach(ServiceController serviceCur in listServices) {
				hklm=Registry.LocalMachine;
				hklm=hklm.OpenSubKey(Path.Combine(@"System\CurrentControlSet\Services\",serviceCur.ServiceName));
				if(hklm.GetValue("ImagePath")==null) {
					continue;
				}
				string installedServicePath=hklm.GetValue("ImagePath").ToString().Replace("\"","");
				if(installedServicePath.Contains(exeName)) {
					retVal.Add(serviceCur);
				}
			}
			return retVal;
		}

		///<summary>Returns one service that has "Path to executeable" set to the full path passed in.  Returns null if not found.</summary>
		///<param name="exeFullPath">E.g. C:\Program Files(x86)\Open Dental\OpenDentalCustListener\OpenDentalCustListener.exe</param>
		public static ServiceController GetServiceByExeFullPath(string exeFullPath) {
			return GetServicesByExe(exeFullPath).FirstOrDefault();
		}

		///<summary>Returns true if the service passed in allows "Everyone" to manage the service (start / stop).
		///Throws exceptions if anything went wrong accessing the security descriptor for the service controller passed in.</summary>
		public static bool GetIsEveryoneAllowedToManageService(ServiceController service) {
			//Use the Windows Service Controller to show the "security descriptor" for the current service.
			string standardOutputShow;
			int exitCodeShow;
			GetSecurityDescriptorForService(service.ServiceName,out standardOutputShow,out exitCodeShow);
			if(exitCodeShow!=0) {
				throw new ApplicationException("Error showing security descriptor.  Error code: "+exitCodeShow
					+"\r\n"+standardOutputShow);
			}
			//The security descriptor for all Open Dental services should ALWAYS contain the portion to grant all users permissions to stop and start.
			//https://msdn.microsoft.com/en-us/library/aa374928(v=vs.85)
			//The "ace type" section------------------------------------------------------------------------------------------------------------------------
			//A: ACCESS_ALLOWED_ACE_TYPE
			//The "rights" section--------------------------------------------------------------------------------------------------------------------------
			//RP: ADS_RIGHT_DS_READ_PROP - Read the properties of a DS object.
			//WP: ADS_RIGHT_DS_WRITE_PROP - Write properties for a DS object.
			//CR: ADS_RIGHT_DS_CONTROL_ACCESS - Access allowed only after extended rights checks supported by the object are performed. 
			//		This flag can be used alone to perform all extended rights checks on the object or it can be combined with an identifier of a specific 
			//		extended right to perform only that check.
			//The last two letters define the security principal assigned with these permissions a SID or well known aliases--------------------------------
			//WD - Everyone
			return standardOutputShow.Contains("(A;;RPWPCR;;;WD)");
		}

		///<summary>Adds the ability for Everyone to manage the passed in services by manipulating the security descriptor.
		///Tries to manipulate the security descriptor for every service passed in.  Silently fails if unable to apply the new permission.</summary>
		public static void SetSecurityDescriptorToAllowEveryoneToManageServices(List<ServiceController> listServices) {
			foreach(ServiceController service in listServices) {
				try {
					SetSecurityDescriptorToAllowEveryoneToManageService(service);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
		}

		///<summary>Adds the ability for Everyone to manage the passed in service by manipulating the security descriptor.
		///Throws exceptions if anything goes wrong manipulating the security descriptor for the service passed in.</summary>
		public static void SetSecurityDescriptorToAllowEveryoneToManageService(ServiceController service) {
			if(GetIsEveryoneAllowedToManageService(service)) {
				return;//Nothing to do, everyone can already manage the service passed in.
			}
			string standardOutputShow;
			int exitCodeShow;
			GetSecurityDescriptorForService(service.ServiceName,out standardOutputShow,out exitCodeShow);
			if(exitCodeShow!=0) {
				throw new ApplicationException("Error showing security descriptor.  Error code: "+exitCodeShow
					+"\r\n"+standardOutputShow);
			}
			//All users cannot correctly manage the service yet so we need to use the security descriptor setter to add the correct permission.
			//Always preserve whatever permissions were already set for the service.
			//However, we need to insert the new permission just before the SACL section ("S:" section).
			//https://msdn.microsoft.com/en-us/library/aa379570(v=vs.85)
			string securityDescriptor=standardOutputShow.Trim();
			int startIndex=securityDescriptor.IndexOf("S:");
			if(startIndex==-1) {
				//Unneeded components can be omitted from the security descriptor string and I'm not quite sure what to do in this case.
				//For now we will throw an error and have the customers call us so we can investigate further.  It has always been present in my testing.
				throw new ApplicationException("Error setting security descriptor; No SACL found.");
			}
			securityDescriptor=securityDescriptor.Insert(startIndex,"(A;;RPWPCR;;;WD)");
			//Update the current security descriptor with our new permission.
			string standardOutputSet;
			int exitCodeSet;
			SetSecurityDescriptorForService(service.ServiceName,securityDescriptor,out standardOutputSet,out exitCodeSet);
			if(exitCodeSet!=0) {
				throw new ApplicationException("Error setting security descriptor.  Error code: "+exitCodeSet
					+"\r\n"+standardOutputSet);
			}
		}

		///<summary>Uses the service controller sdshow command to get the security descriptor for the passed in serviceName.
		///The out parameters "standardOutput" and "exitCode" will contain the results of the "Process" execution.</summary>
		private static void GetSecurityDescriptorForService(string serviceName,out string standardOutput,out int exitCode) {
			//Use the Windows Service Controller to show the "security descriptor" for the current service via a command prompt.
			ExecuteProcess("cmd.exe"
				,"/C sc sdshow "+serviceName
				,out standardOutput
				,out exitCode);
		}

		///<summary>Uses the service controller sdset command to set the passed in serviceName to the passed in securityDescriptor.
		///The out parameters "standardOutput" and "exitCode" will contain the results of the "Process" execution.</summary>
		private static void SetSecurityDescriptorForService(string serviceName,string securityDescriptor,out string standardOutput,out int exitCode) {
			//Use the Windows Service Controller to set the "security descriptor" for the current service via a command prompt.
			ExecuteProcess("cmd.exe"
				,"/C sc sdset "+serviceName+" \""+securityDescriptor+"\""
				,out standardOutput
				,out exitCode);
		}
		
		///<summary>Creates a default ServiceConfig file at the full file path provided.
		///The config contains the current connection settings in DataConnection and defaults LogLevelOfApplication to 'Error'.</summary>
		public static bool CreateServiceConfigFile(string filePath,string serverName,string databaseName,string mySqlUser,string mySqlPass
			,string mySqlPassHash,string mySqlUserLow="",string mySqlUserPassLow="") 
		{
			XmlDocument document=new XmlDocument();
			//Creating Nodes
			XmlNode nodeConnSettings=document.CreateNode(XmlNodeType.Element,"ConnectionSettings","");
			XmlNode nodeDbeConn=document.CreateNode(XmlNodeType.Element,"DatabaseConnection","");
			XmlNode nodeCompName=document.CreateNode(XmlNodeType.Element,"ComputerName","");
			nodeCompName.InnerText=serverName;
			XmlNode nodeDatabase=document.CreateNode(XmlNodeType.Element,"Database","");
			nodeDatabase.InnerText=databaseName;
			XmlNode nodeUser=document.CreateNode(XmlNodeType.Element,"User","");
			nodeUser.InnerText=mySqlUser;
			XmlNode nodePassword=document.CreateNode(XmlNodeType.Element,"Password","");
			nodePassword.InnerText=string.IsNullOrEmpty(mySqlPassHash) ? mySqlPass : "";//write plain text pwd if encryption failed
			XmlNode nodePassHash=document.CreateNode(XmlNodeType.Element,"MySQLPassHash","");
			nodePassHash.InnerText=mySqlPassHash ?? "";//write encrypted password, if it's null then write an empty string
			XmlNode nodeUserLow=document.CreateNode(XmlNodeType.Element,"UserLow","");
			nodeUserLow.InnerText=mySqlUserLow;
			XmlNode nodePasswordLow=document.CreateNode(XmlNodeType.Element,"PasswordLow","");
			nodePasswordLow.InnerText=mySqlUserPassLow;
			XmlNode nodeDbType=document.CreateNode(XmlNodeType.Element,"DatabaseType","");
			nodeDbType.InnerText="MySql";//Not going to support Oracle until someone complains.
			XmlNode nodeLogLevelOfApp=document.CreateNode(XmlNodeType.Element,"LogLevelOfApplication","");
			nodeLogLevelOfApp.InnerText="Error";
			//Assigning Structure
			nodeDbeConn.AppendChild(nodeCompName);
			nodeDbeConn.AppendChild(nodeDatabase);
			nodeDbeConn.AppendChild(nodeUser);
			nodeDbeConn.AppendChild(nodePassword);
			nodeDbeConn.AppendChild(nodePassHash);
			nodeDbeConn.AppendChild(nodeUserLow);
			nodeDbeConn.AppendChild(nodePasswordLow);
			nodeDbeConn.AppendChild(nodeDbType);
			nodeConnSettings.AppendChild(nodeDbeConn);
			nodeConnSettings.AppendChild(nodeLogLevelOfApp);
			document.AppendChild(nodeConnSettings);
			//Outputting completed XML document
			StringBuilder strb=new StringBuilder();
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars="   ";
			settings.NewLineChars="\r\n";
			settings.OmitXmlDeclaration=true;
			try {
				using(XmlWriter xmlWriter=XmlWriter.Create(strb,settings)) {
					document.WriteTo(xmlWriter);
					xmlWriter.Flush();
					File.WriteAllText(filePath,strb.ToString());
				}
				return true;
			}
			catch {
				return false;
			}
		}

		///<summary>Returns true if a service is currently installed with the specified service name.
		///Optionally pass in the file info for the desired service and this method will also return true if there is a service utilizing the same 
		///executable.</summary>
		public static bool HasService(string serviceName,FileInfo serviceFileInfo = null) {
			//Old way is to search the registry. This can tend to throw exceptions based on Windows user permissions, especially if runnong on a Domain Controller.
			//Let's try it this way first so as not to break any back-compatibility.
			try {
				List<ServiceController> listServices=GetServices();
				foreach(ServiceController service in listServices) {
					if(serviceName==service.ServiceName) {
						return true;
					}
					if(serviceFileInfo==null) {
						continue;
					}
					RegistryKey hklm=Registry.LocalMachine;
					hklm=hklm.OpenSubKey(@"System\CurrentControlSet\Services\"+service.ServiceName);
					string installedServicePath=hklm.GetValue("ImagePath").ToString().Replace("\"","");
					if(installedServicePath.Contains(serviceFileInfo.FullName)) {
						return true;
					}
				}
				return false;
			}
			catch(Exception e) {
				e.DoNothing();
			}
			//Querying the registry failed so let's try querying Window Management Interface (WMI).
			//If this throws also then let it throw. It means we absolutely cannot determine that status of this service.
			var asWmi=ODWmiService.GetServices();
			if(asWmi.Any(x => string.Compare(x.Name,serviceName,true)==0)) { //Name match. Already exists.
				return true;
			}
			//Check the new path against all installed service paths. Typically installed service paths are encapsulated by \"  \" so use Contains().
			if(serviceFileInfo!=null && asWmi.Any(x => x.PathName.ToLower().Contains(serviceFileInfo.FullName.ToLower()))) { //Path match. Already exists.
				return true;
			}
			//Service does not exist.
			return false;
		}

		///<summary>Gets the service name for the currently running process. Throws if it cannot find a service.</summary>
		public static string GetCurrentProcessServiceName() {
			int processId=Process.GetCurrentProcess().Id;
			ODWmiService curService=ODWmiService.GetServices().FirstOrDefault(x => x.ProcessId==processId);
			if(curService==null) {
				throw new Exception("Unable to find service name.");
			}
			return curService.Name;
		}

		///<summary>Helper class that offers an alternative to querying windows to find out which services are installed.
		///Use this alternative when the old "search the registry" routine fails.</summary>
		private class ODWmiService {
			public string Description;
			public string DisplayName;
			public string Name;
			public string PathName;
			public bool Started;
			public string StartMode;
			public string StartName;
			public string State;
			public uint ProcessId;

			public static List<ODWmiService> GetServices() {
				ManagementObjectSearcher searcher=new ManagementObjectSearcher("SELECT * FROM Win32_Service");
				ManagementObjectCollection collection=searcher.Get();
				return collection
					.Cast<ManagementObject>()
					.Select(x => new ODWmiService() {
						Description=(string)x.Properties["Description"].Value??"",
						DisplayName=(string)x.Properties["DisplayName"].Value??"",
						Name=(string)x.Properties["Name"].Value??"",
						PathName=(string)x.Properties["PathName"].Value??"",
						Started=(bool)x.Properties["Started"].Value,
						StartMode=(string)x.Properties["StartMode"].Value??"",
						StartName=(string)x.Properties["StartName"].Value??"",
						State=(string)x.Properties["State"].Value??"",
						ProcessId=(uint)x.Properties["ProcessId"].Value,
					}).ToList();
			}
		}
	}
}
