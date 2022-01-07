using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;

namespace OpenDentBusiness {
	public class RemotingClient {
		///<summary>This dll will be in one of these three roles.  There can be a dll on the client and a dll on the server, both involved in the logic.  This keeps track of which one is which.</summary>
		private static RemotingRole _remotingRole;
		///<summary>If ClientWeb, then this is the URL to the server.</summary>
		private static string _serverUri;
		///<summary>If ClientWeb (middle tier user), proxy settings can be picked up from MiddleTierProxyConfig.xml.</summary>
		public static string MidTierProxyAddress;
		///<summary>If ClientWeb (middle tier user), proxy settings can be picked up from MiddleTierProxyConfig.xml.</summary>
		public static string MidTierProxyUserName;
		///<summary>If ClientWeb (middle tier user), proxy settings can be picked up from MiddleTierProxyConfig.xml.</summary>
		public static string MidTierProxyPassword;
		///<summary>Thread static version of RemotingRole</summary>
		[ThreadStatic]
		public static RemotingRole _remotingRoleT;
		///<summary>Thread static string version of RemotingRole because enums cannot be null thus we never know what value to trust.</summary>
		[ThreadStatic]
		public static string _remotingRoleTStr;
		///<summary>Thread static version of ServerURI</summary>
		[ThreadStatic]
		public static string _serverUriT;
		private static bool _hasLoginFailed;
		private static bool _hasRemoteConnectionFailed;
		private static ReaderWriterLockSlim _lockLoginFailed=new ReaderWriterLockSlim();
		private static ReaderWriterLockSlim _lockRemoteConnectionFailed=new ReaderWriterLockSlim();
		
		///<summary>Set to true when a middle tier client has failed to validate credentials AFTER having logged in successfully.</summary>
		public static bool HasLoginFailed {
			get {
				if(RemotingRole!=RemotingRole.ClientWeb) {
					return false;//There is no such thing as the middle tier failing to "log in".  It must return a failed to log in payload to the client.
				}
				_lockLoginFailed.EnterReadLock();
				try {
					return _hasLoginFailed;
				}
				finally {
					_lockLoginFailed.ExitReadLock();
				}
			}
			set {
				_lockLoginFailed.EnterWriteLock();
				try {
					_hasLoginFailed=value;
				}
				finally {
					_lockLoginFailed.ExitWriteLock();
				}
			}
		}

		///<summary>Set to true when a middle tier client has failed to connect AFTER having logged in successfully.</summary>
		public static bool HasMiddleTierConnectionFailed {
			get {
				if(RemotingRole!=RemotingRole.ClientWeb) {
					return false;//ClientDirect and ServerWeb are both directly connected and will handle a loss of connection differently.
				}
				_lockRemoteConnectionFailed.EnterReadLock();
				try {
					return _hasRemoteConnectionFailed;
				}
				finally {
					_lockRemoteConnectionFailed.ExitReadLock();
				}
			}
			set {
				_lockRemoteConnectionFailed.EnterWriteLock();
				try {
					_hasRemoteConnectionFailed=value;
				}
				finally {
					_lockRemoteConnectionFailed.ExitWriteLock();
				}
			}
		}

		[ThreadStatic]
		private static bool _isReportServer;
		///<summary>True if the RemotingClient connection is connecting to a report middle tier server.</summary>
		public static bool IsReportServer {
			get {
				return _isReportServer;
			}
			set {
				if(RemotingRole!=RemotingRole.ServerWeb) {
					_isReportServer=false;//We only allow the passing of queries over middle tier when the client is itself a middle tier server.
				}
				else {
					_isReportServer=value;
				}
			}
		}

		///<summary>Returns either the thread specific RemotingRole or the globally set RemotingRole.</summary>
		public static RemotingRole RemotingRole {
			get {
				if(String.IsNullOrEmpty(_remotingRoleTStr)) {
					return _remotingRole;
				}
				return _remotingRoleT;
			}
			set {
				_remotingRole=value;
				_remotingRoleT=value;
				_remotingRoleTStr=value.ToString();//Simply used as an indicator that _remotingRoleT has been set.
				DataConnection.CanReconnect=(value!=RemotingRole.ServerWeb);
				DataConnection.CanReconnectT=(DataConnection.CanReconnect?1:0);
			}
		}

		///<summary>Returns either the thread specific Server URI or the globally set Server URI.</summary>
		public static string ServerURI {
			get {
				if(String.IsNullOrEmpty(_serverUriT)) {
					return _serverUri;
				}
				return _serverUriT;
			}
			set {
				_serverUri=value;
				_serverUriT=value;
			}
		}

		public static void SetRemotingT(string serverURI,RemotingRole remotingRole,bool isReportServer) {
			IsReportServer=isReportServer;
			_remotingRoleT=remotingRole;
			_remotingRoleTStr=_remotingRoleT.ToString();//Simply used as an indicator that _remotingRoleT has been set.
			_serverUriT=serverURI;
			DataConnection.CanReconnectT=(remotingRole!=RemotingRole.ServerWeb)?1:0;
		}

		public static void SetRemotingRoleT(RemotingRole remotingRole) {
			_remotingRoleT=remotingRole;
			_remotingRoleTStr=_remotingRoleT.ToString();//Simply used as an indicator that _remotingRoleT has been set.
			DataConnection.CanReconnectT=(remotingRole!=RemotingRole.ServerWeb)?1:0;
		}

		public static void SetServerURIT(string serverURI) {
			_serverUriT=serverURI;
		}

		#region Process DTO Methods

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static DataTable ProcessGetTable(DtoGetTable dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return XmlConverter.XmlToTable(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static DataTable ProcessGetTableLow(DtoGetTableLow dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return XmlConverter.XmlToTable(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static DataSet ProcessGetDS(DtoGetDS dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			if(Regex.IsMatch(result,"<DtoException xmlns:xsi=")) {
				DtoException exception=(DtoException)DataTransferObject.Deserialize(result);
				throw new Exception(exception.Message);
			}
			try {
				return XmlConverter.XmlToDs(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static SerializableDictionary<K,V> ProcessGetSerializableDictionary<K,V>(DtoGetSerializableDictionary dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return XmlConverterSerializer.Deserialize<SerializableDictionary<K,V>>(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static long ProcessGetLong(DtoGetLong dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return PIn.Long(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static int ProcessGetInt(DtoGetInt dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return PIn.Int(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static double ProcessGetDouble(DtoGetDouble dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return PIn.Double(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static void ProcessGetVoid(DtoGetVoid dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			if(result!="0"){
				DtoException exception=(DtoException)DataTransferObject.Deserialize(result);
				throw ThrowExceptionForDto(exception);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static T ProcessGetObject<T>(DtoGetObject dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			try {
				return XmlConverterSerializer.Deserialize<T>(result);
			}
			catch(Exception ex) {
				throw ProcessExceptionDeserialize(result,ex);
			}
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static string ProcessGetString(DtoGetString dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			DtoException exception;
			try {
				exception=(DtoException)DataTransferObject.Deserialize(result);
			}
			catch {
				return XmlConverter.XmlUnescape(result);
			}
			throw ThrowExceptionForDto(exception);
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		public static bool ProcessGetBool(DtoGetBool dto,bool hasConnectionLost=true) {
			string result=SendAndReceive(dto,hasConnectionLost);
			if(result=="True") {
				return true;
			}
			if(result=="False") {
				return false;
			}
			DtoException exception=(DtoException)DataTransferObject.Deserialize(result);
			throw ThrowExceptionForDto(exception);
		}

		#endregion

		///<summary>Returns an exception to be thrown if an exception is being caught while attempting to deserialize a serialized string returned
		///from the server.</summary>
		///<param name="result">The serialized string sent by the server.</param>
		///<param name="ex">The exception that was caught while trying to deserialize the result.</param>
		private static Exception ProcessExceptionDeserialize(string result,Exception ex) {
			if(ex is ThreadAbortException) {
				//If we abort a thread while it is trying to deserialize the result, it will attempt to deserialize it below and throw a different
				//exception. We want the abort exception to bumble up to ODThread to be caught and handled. Normally, ThreadAbortExceptions bubble up
				//even if caught. However, it does not when a different exception is thrown within a catch.
				return ex;
			}
			DtoException exception;
			try {
				exception=(DtoException)DataTransferObject.Deserialize(result);
			}
			catch(Exception e) {
				throw new AggregateException("Error deserializing result from server.",new Exception("Result: "+result),e,ex);
			}
			return ThrowExceptionForDto(exception);
		}

		///<summary>Optionally set hasConnectionLost true to keep the calling thread here until a connection to the Middle Tier connection can be established
		///in the event of a web connection failure. Set hasConnectionLost to false if a throw is desired when a connection cannot be made.</summary>
		internal static string SendAndReceive(DataTransferObject dto,bool hasConnectionLost=true) {
			//Anyone trying to invoke a method other than CheckUserAndPassword must first check the current HasLoginFailed status as to not call the middle tier too often.
			bool isCheckUserAndPassword=(dto.MethodName==nameof(OpenDentBusiness)+"."+nameof(Userods)+"."+nameof(Userods.CheckUserAndPassword));
			if(!isCheckUserAndPassword && HasLoginFailed) {
				throw new ODException("Invalid username or password.",ODException.ErrorCodes.CheckUserAndPasswordFailed);
			}
			string dtoString=dto.Serialize();
			IOpenDentalServer service=OpenDentBusiness.WebServices.OpenDentalServerProxy.GetOpenDentalServerInstance();
			return SendAndReceiveRecursive(service,dtoString,hasConnectionLost);
		}

		///<summary>Tries to process the dto passed in.  If there was a web connection failure then this method will keep the calling thread here 
		///until a connection to the Middle Tier can be established.  Set hasConnectionLost to false if a throw is desired when a connection cannot be made.
		///E.g. Set hasConnectionLost to false when a user is trying to log in for the first time (don't want to try logging in forever).</summary>
		private static string SendAndReceiveRecursive(IOpenDentalServer service,string dtoString,bool hasConnectionLost=true) {
			try {
				return QueryMonitor.Monitor.ProcessMonitoredPayload(service.ProcessRequest,dtoString);
			}
			catch(WebException wex) {
				//If no connection monitoring desired or this is a WebException that we aren't explicitly looking for then bubble up the exception.
				//WebException class: https://docs.microsoft.com/en-us/dotnet/api/system.net.webexception?view=netframework-4.7.2
				//WebException.Status property: https://docs.microsoft.com/en-us/dotnet/api/system.net.webexception.status?view=netframework-4.7.2
				//Handling WebExceptions: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/handling-errors?view=netframework-4.7.2
				if(!hasConnectionLost || wex.Status!=WebExceptionStatus.ConnectFailure) {
					throw;
				}
				//The calling method wants to automatically retry connecting to the Middle Tier until it comes back.
				RemoteConnectionFailed();
				return SendAndReceiveRecursive(service,dtoString);
			}
		}

		///<summary>This method will get invoked by clients that are having trouble connecting to the middle tier.
		///It does not query the database on purpose because each instance of Open Dental can have many threads asking this question.
		///Also, some payloads have an impressive amount of data that had to be serialized and this method will have a small signature,
		///thereby reducing network traffic during the timeframe that Clients are attempting to reconnect to the Middle Tier server.</summary>
		public static bool IsMiddleTierAvailable() {
			//Clients will be invoking this method so there needs to be a remoting role check here even though no db call.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetIsMiddleTierAvailable(MethodBase.GetCurrentMethod());
			}
			return (RemotingClient.RemotingRole==RemotingRole.ServerWeb);
		}

		///<summary>Fires a MiddleTierConnectionEvent of type MiddleTierConnectionLost to notify the main thread that the Middle Tier connection has been lost.
		///Wait here until the connection is restored.</summary>
		private static void RemoteConnectionFailed() {
			RemotingClient.HasMiddleTierConnectionFailed=true;
			//Inform all threads that we've lost the MiddleTier connection, so they can handle this appropriately.
			MiddleTierConnectionEvent.Fire(new MiddleTierConnectionEventArgs(false));
			//Keep the current thread stuck here while automatically retrying the connection up until the timeout specified.
			DateTime beginning=DateTime.Now;
			ODThread threadRetry=new ODThread(500,(o) => {
				if(!HasMiddleTierConnectionFailed) {
					o.QuitAsync();
					return;
				}
				if((DateTime.Now-beginning).TotalSeconds>=TimeSpan.FromHours(4).TotalSeconds) {
					return;//We have reached or exceeded the timeout.  Stop automatically retrying and leave it up to a manual retry from the user.
				}
				try {
					if(IsMiddleTierAvailable()) {
						//Unset HasMiddleTierConnectionFailed flag allowing any blocked Middle Tier communication to continue.
						RemotingClient.HasMiddleTierConnectionFailed=false;
						//Also fire a DataConnectionEvent letting everyone who cares know that the connection has been restored.
						MiddleTierConnectionEvent.Fire(new MiddleTierConnectionEventArgs(true));
						o.QuitAsync();
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Connection has not been restored yet.  Wait a little bit and then try again.
				}
			});
			threadRetry.Name="MiddleTierConnectionLostAutoRetryThread";
			threadRetry.AddExceptionHandler((e) => e.DoNothing());
			threadRetry.Start();
			//Wait here until the retry thread has finished which is either due to connection being restored or the timeout was reached.
			threadRetry.Join(Timeout.Infinite);//Wait forever because the retry thread has a timeout within itself.
		}

		///<summary>Open Dental can require specific exceptions to be thrown.  This is a helper method that throws the correct exception type.
		///Add this function directly into a throw statement, so that the calling code knows that the code path will not need to return a value.</summary>
		private static Exception ThrowExceptionForDto(DtoException exception) {
			switch(exception.ExceptionType) {
				case "ApplicationException":
					throw new ApplicationException(exception.Message);
				case "InvalidProgramException":
					throw new InvalidProgramException(exception.Message);
				case "NotSupportedException":
					throw new NotSupportedException(exception.Message);
				case "ODException":
					throw new ODException(exception.Message,exception.ErrorCode);
				default:
					//Throw a generic exception which follows the old functionality for any other Exception type that we weren't explicitly expecting.
					throw new Exception(exception.Message);
			}
		}

		
	}

}
