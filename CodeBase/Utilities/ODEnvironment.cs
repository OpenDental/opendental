using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeBase{
  public class ODEnvironment{
		///<summary>Reflects the state of the laptop or slate mode, 0 for Slate Mode and non-zero otherwise.
		///When this system metric changes, the system sends a broadcast message via WM_SETTINGCHANGE with "ConvertibleSlateMode" in the LPARAM.
		///Note that this system metric doesn't apply to desktop PCs. In that case, use GetAutoRotationState.
		///See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics for details.
		///Not supported in Windows CE.</summary>
		private const int SM_CONVERTIBLESLATEMODE=0x2003;
		///<summary>Nonzero if the current operating system is the Windows XP Tablet PC edition or if the current operating system is Windows Vista or Windows 7
		///and the Tablet PC Input service is started; otherwise, 0. The SM_DIGITIZER setting indicates the type of digitizer input supported by a device 
		///running Windows 7 or Windows Server 2008 R2. For more information, see Remarks.
		///See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics for details.
		///Not supported in Windows CE.</summary>
		private const int SM_TABLETPC=86;
		///<summary>The name of the computer the client is running on.</summary>
		private static string _machineName;

		#region Tablet Mode
		[DllImport("user32.dll",SetLastError=true,CharSet=CharSet.Auto,EntryPoint="GetSystemMetrics")]
		///<summary>Retrieves the specified system metric or system configuration setting. Note that all dimensions retrieved by GetSystemMetrics are in 
		///pixels. The system metric or configuration setting to be retrieved. This parameter can be one of the following values. Note that all SM_CX* 
		///values are widths and all SM_CY* values are heights. Also note that all settings designed to return Boolean data represent TRUE as any nonzero 
		///value, and FALSE as a zero value. See https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics and
		///https://docs.microsoft.com/en-us/windows/desktop/tablet/determining-whether-a-pc-is-a-tablet-pc for more details.</summary>
		private static extern int GetSystemMetrics(int nIndex);

		///<summary>Indicates if the current device is running in Windows Tablet mode and does not have a physical keyboard attached.
		///Returns false if any errors occurred while trying to query system metrics for information so that calling entities default to Desktop mode.</summary>
		public static bool IsTabletMode {
			get {
				bool isTableMode=false;
				ODException.SwallowAnyException(() => {
					isTableMode=QueryTabletMode();
				});
				return isTableMode;
			}
		}

		///<summary>Indicates if the current device is a Windows Tablet PC. This means that all of the following are true:
		///There is an integrated digitizer, either pen or touch, on the system. The Tablet PC optional component is installed. This component contains 
		///features such as Tablet PC Input Panel and Windows Journal. The computer is licensed to use the optional component. Premium versions of 
		///Windows Vista—such as Windows Vista Home Premium, Windows Vista Small Business, Windows Vista Professional, Windows Vista Enterprise, and 
		///Windows Vista Ultimate—are licensed to use the optional component. Tablet PC Input Service is running. Tablet PC Input Service is a new 
		///service for Windows Vista that controls Tablet PC input.
		///See https://docs.microsoft.com/en-us/windows/desktop/tablet/determining-whether-a-pc-is-a-tablet-pc for details.</summary>
		private static bool IsTabletPC {
			get {
				return (GetSystemMetrics(SM_TABLETPC)!=0);
			}
		}

		///<summary>Indicates if we are running on an OD Cloud server.</summary>
		public static bool IsCloudServer {
			get {
				//Sometimes we connect to an OD Cloud db with a normal build to do things like updates.
				return ODBuild.IsWeb() || Directory.Exists(@"C:\Program Files\Thinfinity\VirtualUI");
			}
		}

		///<summary>True if the current device is both a Tablet PC and is also in slate mode, the combination of which we will consider to mean the
		///device is currently running in Tablet Mode.  SM_CONVERTIBLESLATEMODE reflects the state of the laptop or slate mode, 0 for Slate Mode and 
		///non-zero otherwise. Note that the SM_CONVERTIBLESLATEMODE system metric doesn't apply to desktop PCs, and as such is only useful for 
		///determining if a device is a Tablet PC and functioning as such in conjunction with SM_TABLETPC. Slate mode may be thought of as indicating 
		///that the Tablet PC is currently detached from a hardware keyboard.  An example is a Windows Surface with a detachable keyboard.  When the 
		///keyboard is attached, though the device is a still a Tablet PC, it is functioning like a laptop, therefore Slate mode is off.  When the 
		///keyboard is detached, the device is functioning as a tablet, i.e. in slate mode.</summary>
		private static bool QueryTabletMode() {
			return IsTabletPC && (GetSystemMetrics(SM_CONVERTIBLESLATEMODE)==0);
		}
		#endregion

		///<summary>If not in Web mode, returns Environment.MachineName. Otherwise sends a request to the browser to get the computer name.</summary>
		public static string MachineName {
			get {
				if(!ODBuild.IsWeb()) {
					return Environment.MachineName;
				}
				string unknownName="UNKNOWN";
				if(!_machineName.IsNullOrEmpty() && _machineName.ToUpper()!=unknownName.ToUpper()) {
					return _machineName;
				}
				try {
					_machineName=ODCloudClient.SendToBrowserSynchronously("",ODCloudClient.BrowserAction.GetComputerName,doShowProgressBar:false);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				if(_machineName.IsNullOrEmpty()) {
					_machineName=unknownName;
				}
				return _machineName;
			}
			set {
				_machineName=value;
			}
		}

		//public static bool Is64BitOperatingSystem(){
		//  string arch="";
		//  try{
		//      arch=Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
		//  }catch{
		//      //May fail if the environemnt variable is not present on the target machine (i.e. Unix).
		//  }
		//  bool retVal=Regex.IsMatch(arch,".*64.*");
		//  return retVal; 
		//}

		///<summary>Will return true if the provided id matches the local computer name or a local IPv4 or IPv6 address. Will return false if id is 'localhost' or '127.0.0.1'. Returns false in all other cases.</summary>
		public static bool IdIsThisComputer(string id){
			id=id.ToLower();
			//Compare ID against the local host name.
			if(Environment.MachineName.ToLower()==id){
			  return true;
			}
			IPHostEntry iphostentry;
			try {
				iphostentry=Dns.GetHostEntry(Environment.MachineName);
			}
			catch {
				return false;
			}
			//Check against the local computer's IP addresses (does not include 127.0.0.1). Includes IPv4 and IPv6.
			foreach(IPAddress ipaddress in iphostentry.AddressList){
			  if(ipaddress.ToString()==id){
			    return true;
			  }
			}
			return false;
		}

		///<summary>Will return true if the provided servername matches the local computer name or a local IPv4 or IPv6 address.  Will return true if servername is 'localhost' or '127.0.0.1'.  Returns false in all other cases.</summary>
		public static bool IsRunningOnDbServer(string servername) {
			servername=servername.ToLower();
			//Compare servername against the local host name.  Also check if the servername is "localhost".
			if(Environment.MachineName.ToLower()==servername || servername=="localhost") {
				return true;
			}
			//Check to see if the servername is an ipaddress that is a loopback (127.XXX.XXX.XXX).  Catches failure in parsing.
			try {
				if(IPAddress.IsLoopback(IPAddress.Parse(servername))) {
					return true;
				}
			}
			catch { }	//not a valid IP address
			IPHostEntry iphostentry;
			try {
				iphostentry=Dns.GetHostEntry(Environment.MachineName);
			}
			catch {
				return false;
			}
			//Check against the local computer's IP addresses (does not include 127.0.0.1). Includes IPv4 and IPv6.
			foreach(IPAddress ipaddress in iphostentry.AddressList) {
				if(ipaddress.ToString()==servername) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns true if the current application is running as an administrator.  Otherwise; false.  Throws exceptions.</summary>
		public static bool IsRunningAsAdministrator() {
			using(WindowsIdentity identity=WindowsIdentity.GetCurrent()) {
				WindowsPrincipal principal=new WindowsPrincipal(identity);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		///<summary>Returns an IPv4 address for the local machine. Returns an empty string if one cannot be found.</summary>
		public static string GetLocalIPAddress() {
			IPHostEntry iphostentry;
			try {
				iphostentry=Dns.GetHostEntry(Environment.MachineName);
			}
			catch {
				return "";
			}
			foreach(IPAddress ip in iphostentry.AddressList) {
				if(ip.AddressFamily==AddressFamily.InterNetwork) {
					return ip.ToString();
				}
			}
			return "";
		}

		///<summary>Returns the default gateway for the local machine. Returns an empty string if one cannot be found.</summary>
		public static string GetDefaultGateway() {
			IPAddress defaultGateway=NetworkInterface.GetAllNetworkInterfaces()
				.Where(x => x.OperationalStatus==OperationalStatus.Up)
				.Where(x => x.NetworkInterfaceType!=NetworkInterfaceType.Loopback)
				.SelectMany(x => x.GetIPProperties()?.GatewayAddresses)
				.Select(x => x?.Address)
				.Where(x => x!=null)
				.Where(x => x.AddressFamily==AddressFamily.InterNetwork)
				.FirstOrDefault();
			return defaultGateway?.ToString()??"";
		}

		///<summary>This method checks the product version of the kernel32.dll to determine if the current OS is running Windows 7 or not.
		///Throws exceptions by default.  Pass false to swallow all exceptions.</summary>
		public static bool IsWindows7(bool hasExceptions=true) {
			//For more information see https://www.geoffchappell.com/studies/windows/win32/kernel32/history/index.htm
			/*****************************************************************************************************************
			File Version		File Header		Date Stamp				File Size		Package
			3.51.1048.1			2FC3AE99		(25th May 1995)			336,224			Windows NT 3.51
			3.51.1057.6			3214F49D		(17th August 1996)		337,696			Windows NT 3.51 SP5
			4.0.0.950			2FF48837		(1st July 1995)			411,136			Windows 95
			4.0.0.1111			320C1CA0		(10th August 1996)		414,208			Windows 95 OSR2
			4.0.1380.1			31F7EBAB		(26th July 1996)		363,280			Windows NT 4.0
			4.0.1381.4			3361070B		(26th April 1997)		372,496			Windows NT 4.0 SP3
			4.0.1381.133		36232523		(13th October 1998)		375,056			Windows NT 4.0 SP4
			4.0.1381.178		36D9D5F3		(1st March 1999)		374,544			Windows NT 4.0 SP5
			4.0.1381.300		3794F60F		(21st July 1999)		375,056			Windows NT 4.0 SP6
			4.10.0.1998			3546ABB0		(29th April 1998)		471,040			Windows 98
			4.10.0.2222			371FC2B3		(23rd April 1999)		471,040			Windows 98 SE
			4.90.0.3000			393F3C0E		(8th June 2000)			536,576			Windows Me
			5.0.2191.1			3844D034		(1st December 1999)		732,432			Windows 2000
			5.0.2195.1600		394193D2		(10th June 2000)		730,384			Windows 2000 SP1
			5.0.2195.4272		3C1FE60F		(19th December 2001)	731,920			Windows 2000 SP2
			5.0.2195.5400		3D3D0209		(23rd July 2002)		733,968			Windows 2000 SP3
			5.0.2195.6688		3EF274DC		(20th June 2003)		743,184			Windows 2000 SP4
			5.1.2600.0			3B7DFE0E		(18th August 2001)		926,720			Windows XP
			5.1.2600.1106		3D6DFA28		(29th August 2002)		930,304			Windows XP SP1
			5.1.2600.2180		411096B4		(4th August 2004)		983,552			Windows XP SP2
			5.1.2600.5512		4802A12C		(14th April 2008)		989,696			Windows XP SP3
			5.2.3790.0			3E802494		(25th March 2003)		988,160			Windows Server 2003
			5.2.3790.1830		424377D2		(25th March 2005)		1,038,336		Windows Server 2003 SP1
			5.2.3790.3959		45D70AD8		(18th February 2007)	1,037,312		Windows Server 2003 SP2
			6.0.6000.16386		4549BD80		(2nd November 2006)		874,496			Windows Vista
			6.0.6001.18000		4791A76D		(19th January 2008)		888,320			Windows Vista SP1 & Windows Server 2008
			6.0.6002.18005		49E037DD		(11th April 2009)		891,392			Windows Vista SP2
			6.1.7600.16385		4A5BDAAD		(14th July 2009)		857,088			Windows 7
			6.1.7601.17514		4CE7B8EF		(20th November 2010)	857,600			Windows 7 SP1
			6.2.9200.16384		5010A99B		(25th July 2012)		1,011,712		Windows 8
			6.3.9600.16384		52158E47		(22nd August 2013)		1,037,504		Windows 8.1
			6.3.9600.17031		530886EB		(22nd February 2014)	1,037,504		Windows 8.1 Update
			10.0.10240.16384	559F3B86		(9th July 2015)			624,312			Windows 10
			******************************************************************************************************************/
			Version version;
			try {
				string pathToKernel32Dll=ODFileUtils.CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.System),"kernel32.dll");
				FileVersionInfo versionInfo=FileVersionInfo.GetVersionInfo(pathToKernel32Dll);
				version=new Version(versionInfo.ProductVersion);
			}
			catch(Exception ex) {
				if(hasExceptions) {
					throw ex;
				}
				return false;
			}
			//Windows 7 editions will always start with 6.1:
			//6.1.7600.16385 - Windows 7
			//6.1.7601.17514 - Windows 7 SP1
			return (version.Major==6 && version.Minor==1);//Build and Revision do not matter at this time.
		}
	}
}
