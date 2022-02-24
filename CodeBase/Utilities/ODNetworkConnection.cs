using System;
using System.Net;
using System.Runtime.InteropServices;

namespace CodeBase {
	public class ODNetworkConnection:IDisposable {
		///<summary>Keep the network path around so that it can be used within Dispose() as to cancel the connection automatically.</summary>
		private string _remoteName;

		///<summary>Creates a connection to a network resource via the remoteName and credentials passed in.
		///Throws a Win32Exception with specific error code if WNetAddConnection2() fails.
		///See https://msdn.microsoft.com/en-us/library/windows/desktop/aa385413(v=vs.85).aspx for more details.</summary>
		///<param name="remoteName">The full network path to the desired network share.  Typically a UNC path.</param>
		///<param name="remoteCredentials">The credentials that will be used when connecting to the share.  Only UserName and Password are used.</param>
		public ODNetworkConnection(string remoteName,NetworkCredential remoteCredentials) {
			//Keep track of the remoteName passed in so that we can always close the connection when we dispose.
			_remoteName=remoteName;
			//This particular constructor will always create a global, disk, share NetResource connection.
			//Additional methods / constructors can be added later if we find the need for the other types of net resources.
			NetResource netResource=new NetResource() {
				dwScope=NetResourceScope.RESOURCE_GLOBALNET,
				dwType=NetResourceType.RESOURCETYPE_DISK,
				dwDisplayType=NetResourceDisplayType.RESOURCEDISPLAYTYPE_SHARE,
				lpRemoteName=remoteName
			};
			//Try and add the new network connection to the remoteName with the credentials passed in.
			int errorCode=WNetAddConnection2(netResource,remoteCredentials.Password,remoteCredentials.UserName,0);
			//Bubble up any errors that occurred when trying to add the network connection.
			if(errorCode!=0) {
				throw new ODException("Fatal error making network connection",errorCode);
			}
		}

		~ODNetworkConnection() {
			Dispose(false);//Cancel the network connection.
		}

		public void Dispose() {
			Dispose(true);//Cancel the network connection.
			//Objects that implement the IDisposable interface can call this method from the object's IDisposable.Dispose implementation to prevent the 
			//garbage collector from calling Object.Finalize on an object that does not require it. Typically, this is done to prevent the finalizer from 
			//releasing unmanaged resources that have already been freed by the IDisposable.Dispose implementation.
			//See https://msdn.microsoft.com/en-us/library/system.gc.suppressfinalize(v=vs.110).aspx for more details.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			//The WNetCancelConnection2 function cancels an existing network connection. 
			//You can also call the function to remove remembered network connections that are not currently connected.
			//See https://msdn.microsoft.com/en-us/library/windows/desktop/aa385427(v=vs.85).aspx for more details.
			WNetCancelConnection2(_remoteName,0,true);//Can return a system error code but we don't care at this point if this fails, we tried our best.
		}

		///<summary>The WNetAddConnection2 function makes a connection to a network resource and can redirect a local device to the network resource.
		///The WNetAddConnection2 function supersedes the WNetAddConnection function. If you can pass a handle to a window that the provider of network 
		///resources can use as an owner window for dialog boxes, call the WNetAddConnection3 function instead.
		///See https://msdn.microsoft.com/en-us/library/windows/desktop/aa385413(v=vs.85).aspx for more details.</summary>
		///<param name="lpNetResource">A pointer to a NETRESOURCE structure that specifies details of the proposed connection, 
		///such as information about the network resource, the local device, and the network resource provider.</param>
		///<param name="lpPassword">A pointer to a constant null-terminated string that specifies a password to be used in making the network connection.
		///If lpPassword is NULL, the function uses the current default password associated with the user specified by the lpUserName parameter.
		///If lpPassword points to an empty string,the function does not use a password.
		///If the connection fails because of an invalid password and the CONNECT_INTERACTIVE value is set in the dwFlags parameter,the function displays 
		///a dialog box asking the user to type the password.
		///Windows Me/98/95:  This parameter must be NULL or an empty string.</param>
		///<param name="lpUsername">A pointer to a constant null-terminated string that specifies a user name for making the connection.
		///If lpUserName is NULL, the function uses the default user name. (The user context for the process provides the default user name.)
		///The lpUserName parameter is specified when users want to connect to a network resource for which they have been assigned a user name or account
		///other than the default user name or account.
		///The user-name string represents a security context.It may be specific to a network provider.
		///Windows Me/98/95:  This parameter must be NULL or an empty string.</param>
		///<param name="dwFlags">A set of connection options. The possible values for the connection options are defined in the Winnetwk.h header file.</param>
		///<returns>If the function succeeds, the return value is NO_ERROR.
		///If the function fails, the return value can be an error code or one of the system error codes.</returns>
		[DllImport("mpr.dll")]
		private static extern int WNetAddConnection2(NetResource lpNetResource,string lpPassword,string lpUsername,int dwFlags);

		///<summary>The WNetCancelConnection2 function cancels an existing network connection. 
		///You can also call the function to remove remembered network connections that are not currently connected.
		///The WNetCancelConnection2 function supersedes the WNetCancelConnection function.
		///See https://msdn.microsoft.com/en-us/library/windows/desktop/aa385427(v=vs.85).aspx for more details.</summary>
		///<param name="lpName">Pointer to a constant null-terminated string that specifies the name of either the redirected local device or the remote
		///network resource to disconnect from.
		///If this parameter specifies a redirected local device,the function cancels only the specified device redirection.
		///If the parameter specifies a remote network resource, all connections without devices are canceled.</param>
		///<param name="dwFlags">Connection type. The following values are defined.
		///0 - The system does not update information about the connection.
		///CONNECT_UPDATE_PROFILE - The system updates the user profile with the information that the connection is no longer a persistent one.
		///The system will not restore this connection during subsequent logon operations. 
		///(Disconnecting resources using remote names has no effect on persistent connections.)</param>
		///<param name="fForce">Specifies whether the disconnection should occur if there are open files or jobs on the connection.
		///If this parameter is FALSE, the function fails if there are open files or jobs.</param>
		///<returns>If the function succeeds, the return value is NO_ERROR.
		///If the function fails, the return value is a system error code.</returns>
		[DllImport("mpr.dll")]
		private static extern int WNetCancelConnection2(string lpName,int dwFlags,bool fForce);
	}

	///<summary>The NETRESOURCE structure contains information about a network resource.
	///See https://msdn.microsoft.com/en-us/library/windows/desktop/aa385353(v=vs.85).aspx for more details.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public class NetResource {
		///<summary>The scope of the enumeration. This member can be one of the following values defined in the Winnetwk.h header file.</summary>
		public NetResourceScope dwScope;
		///<summary>The type of resource. This member can be one of the following values defined in the Winnetwk.h header file.</summary>
		public NetResourceType dwType;
		///<summary>The display options for the network object in a network browsing user interface.
		///This member can be one of the following values defined in the Winnetwk.h header file.</summary>
		public NetResourceDisplayType dwDisplayType;
		///<summary>A set of bit flags describing how the resource can be used.
		///Note that this member can be specified only if the dwScope member is equal to RESOURCE_GLOBALNET.
		///This member can be one of the following values defined in the Winnetwk.h header file.</summary>
		public NetResourceUsage dwUsage;
		///<summary>If the dwScope member is equal to RESOURCE_CONNECTED or RESOURCE_REMEMBERED, this member is a pointer to a null-terminated character 
		///string that specifies the name of a local device. This member is NULL if the connection does not use a device.</summary>
		public string lpLocalName;
		///<summary>If the entry is a network resource, this member is a pointer to a null-terminated character string 
		///that specifies the remote network name.
		///If the entry is a current or persistent connection, 
		///lpRemoteName member points to the network name associated with the name pointed to by the lpLocalName member.
		///The string can be MAX_PATH characters in length, and it must follow the network provider's naming conventions.</summary>
		public string lpRemoteName;
		///<summary>A pointer to a NULL-terminated string that contains a comment supplied by the network provider.</summary>
		public string lpComment;
		///<summary>A pointer to a NULL-terminated string that contains the name of the provider that owns the resource. 
		///This member can be NULL if the provider name is unknown. 
		///To retrieve the provider name, you can call the WNetGetProviderName function.</summary>
		public string lpProvider;
	}

	public enum NetResourceScope {
		///<summary>Enumerate currently connected resources. The dwUsage member cannot be specified.</summary>
		RESOURCE_CONNECTED=1,
		///<summary>Enumerate all resources on the network. The dwUsage member is specified.</summary>
		RESOURCE_GLOBALNET,
		///<summary>Enumerate remembered (persistent) connections. The dwUsage member cannot be specified.</summary>
		RESOURCE_REMEMBERED,
	}

	public enum NetResourceType {
		///<summary>All resources.</summary>
		RESOURCETYPE_ANY=0,
		///<summary>Disk resources.</summary>
		RESOURCETYPE_DISK=1,
		///<summary>Print resources.</summary>
		RESOURCETYPE_PRINT=2,
	}

	public enum NetResourceDisplayType {
		///<summary>The method used to display the object does not matter.</summary>
		RESOURCEDISPLAYTYPE_GENERIC=0x00000000,
		///<summary>The object should be displayed as a domain.</summary>
		RESOURCEDISPLAYTYPE_DOMAIN=0x00000001,
		///<summary>The object should be displayed as a server.</summary>
		RESOURCEDISPLAYTYPE_SERVER=0x00000002,
		///<summary>The object should be displayed as a share.</summary>
		RESOURCEDISPLAYTYPE_SHARE=0x00000003,
		///<summary>The object should be displayed as a file.</summary>
		RESOURCEDISPLAYTYPE_FILE=0x00000004,
		///<summary>The object should be displayed as a group.</summary>
		RESOURCEDISPLAYTYPE_GROUP=0x00000005,
		///<summary>The object should be displayed as a network.</summary>
		RESOURCEDISPLAYTYPE_NETWORK=0x00000006,
		///<summary>The object should be displayed as a logical root for the entire network.</summary>
		RESOURCEDISPLAYTYPE_ROOT=0x00000007,
		///<summary>The object should be displayed as a administrative share.</summary>
		RESOURCEDISPLAYTYPE_SHAREADMIN=0x00000008,
		///<summary>The object should be displayed as a directory.</summary>
		RESOURCEDISPLAYTYPE_DIRECTORY=0x00000009,
		///<summary>The object should be displayed as a tree.  This display type was used for a NetWare Directory Service (NDS) tree 
		///by the NetWare Workstation service supported on Windows XP and earlier.</summary>
		RESOURCEDISPLAYTYPE_TREE=0x0000000A,
		///<summary>The object should be displayed as a Netware Directory Service container.
		///This display type was used by the NetWare Workstation service supported on Windows XP and earlier.</summary>
		RESOURCEDISPLAYTYPE_NDSCONTAINER=0x0000000A
	}

	public enum NetResourceUsage {
		///<summary>The resource is a connectable resource; the name pointed to by the lpRemoteName member can be passed to the WNetAddConnection function
		///to make a network connection.</summary>
		RESOURCEUSAGE_CONNECTABLE=0x00000001,
		///<summary>The resource is a container resource; the name pointed to by the lpRemoteName member can be passed to the WNetOpenEnum function to 
		///enumerate the resources in the container.</summary>
		RESOURCEUSAGE_CONTAINER=0x00000002,
		///<summary>The resource is not a local device.</summary>
		RESOURCEUSAGE_NOLOCALDEVICE=0x00000004,
		///<summary>The resource is a sibling. This value is not used by Windows.</summary>
		RESOURCEUSAGE_SIBLING=0x00000008,
		///<summary>The resource must be attached. This value specifies that a function to enumerate resource this should fail if the caller is not 
		///authenticated, even if the network permits enumeration without authentication.</summary>
		RESOURCEUSAGE_ATTACHED=0x00000010,
	}
}
