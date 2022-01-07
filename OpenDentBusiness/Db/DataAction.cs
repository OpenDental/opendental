using CodeBase;
using DataConnectionBase;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Perform actions in different database contexts.</summary>
	public class DataAction {
		private static bool _hasConnections=false;
		///<summary>Filled via FillDictHqCentralConnections().</summary>
		private static ConcurrentDictionary<ConnectionNames,CentralConnection> _dictHqCentralConnections=
			new ConcurrentDictionary<ConnectionNames,CentralConnection>();

		#region HQ Central Connections
		///<summary>Fills the dictionary of connection setting defaults or user overrides for several fkeyTypes if needed.</summary>
		private static void FillDictHqCentralConnections() {
			if(_hasConnections) {
				return;
			}
			Dictionary<ConnectionNames,CentralConnection> dictHqCentralConnections=null;
			if(ODBuild.IsDebug() && !ODInitialize.IsRunningInUnitTest) {
				//The database will typically have connection settings to live databases that should not be used in debug mode.
				//Use the hard coded connection settings which developers can change as needed.
				dictHqCentralConnections=new Dictionary<ConnectionNames,CentralConnection>() {
					#region Default Debug Connections
					{
						ConnectionNames.BugsHQ,
						new CentralConnection() {
							ServerName="localhost",
							DatabaseName="bugs",
							MySqlUser="root",
							MySqlPassword="",
						}
					},
					{
						ConnectionNames.CustomersHQ,
						new CentralConnection() {
							ServerName="localhost",
							DatabaseName="customers",
							MySqlUser="root",
							MySqlPassword="",
						}
					},
					{
						ConnectionNames.ManualPublisher,
						new CentralConnection() {
							ServerName="localhost",
							DatabaseName="jordans_mp_test",
							MySqlUser="root",
							MySqlPassword="",
						}
					},
					{
						ConnectionNames.WebChat,
						new CentralConnection() {
							ServerName="localhost",
							DatabaseName="webchat",
							MySqlUser="root",
							MySqlPassword="",
						}
					},
					{
						ConnectionNames.RemoteSupport,
						new CentralConnection() {
							ServerName="localhost",
							DatabaseName="p2p_debug",
							MySqlUser="root",
							MySqlPassword="",
						}
					}
					#endregion
				};
			}
			else {//Release mode or unit testing
				//Get the default connection settings for all databases from the preference table.
				dictHqCentralConnections=GetHqConnections();
				#region Site Overrides
				SiteLink siteLink=SiteLinks.GetSiteLinkByGateway();
				//There should always be a default connection setting for every possible connection at HQ.
				if(dictHqCentralConnections!=null && siteLink!=null && !string.IsNullOrEmpty(siteLink.ConnectionSettingsHQOverrides)) {
					//Look for site specific overrides.
					Dictionary<ConnectionNames,CentralConnection> dictHqCentralConnectionOverrides=
					GetHqConnectionsFromString(siteLink.ConnectionSettingsHQOverrides);
					if(dictHqCentralConnectionOverrides!=null) {
						foreach(KeyValuePair<ConnectionNames,CentralConnection> keyValuePair in dictHqCentralConnectionOverrides) {
							dictHqCentralConnections[keyValuePair.Key]=keyValuePair.Value;
						}
					}
				}
				#endregion
			}
			foreach(KeyValuePair<ConnectionNames,CentralConnection> keyValuePair in dictHqCentralConnections) {
				_dictHqCentralConnections.AddOrUpdate(keyValuePair.Key,keyValuePair.Value,(conNames,conCentral) => { return keyValuePair.Value; });
			}
			_hasConnections=true;
		}

		///<summary>Flags the connection as stale so they are lazy refreshed next time they are needed.</summary>
		public static void ClearDictHqCentralConnections() {
			_hasConnections=false;
		}

		public static Dictionary<ConnectionNames,CentralConnection> GetHqConnections() {
			return GetHqConnectionsFromString(Prefs.GetOne(PrefName.ConnectionSettingsHQ).ValueString);
		}

		public static Dictionary<ConnectionNames,CentralConnection> GetHqConnectionsFromString(string connectionSettings) {
			Dictionary<ConnectionNames,CentralConnection> dictConns=JsonConvert.DeserializeObject
				<Dictionary<ConnectionNames,CentralConnection>>(connectionSettings);
			foreach(ConnectionNames name in dictConns.Keys) {
				CentralConnection conn=dictConns[name];
				if(conn.MySqlPassword.IsNullOrEmpty()) {
					continue;
				}
				CDT.Class1.Decrypt(conn.MySqlPassword,out conn.MySqlPassword);
			}
			return dictConns;
		}

		///<summary>Returns the connection settings (default or user override) for the type passed in.
		///Returns null if a corresponding connection could not be found.</summary>
		public static CentralConnection GetHqConnection(ConnectionNames connName) {
			FillDictHqCentralConnections();
			_dictHqCentralConnections.TryGetValue(connName,out CentralConnection centralConnection);
			return centralConnection;
		}
		#endregion

		#region Helpers to run directly on a given db.
		#region Actions
		///<summary>Perform the given action in the context of the bugs db.
		///Set useConnectionStore false to use the ConnectionSettingsHQ preference or the sitelink override for ConnectionNames.BugsHQ.</summary>
		public static void RunBugsHQ(Action a,bool useConnectionStore=true) {
			if(useConnectionStore) {
				Run(a,ConnectionNames.BugsHQ);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.BugsHQ);
				Run(a,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","");
			}
		}

		///<summary>HQ only. Perform the given action in the context of the customers db.
		///Set useConnectionStore false to use the ConnectionSettingsHQ preference or the sitelink override for ConnectionNames.CustomersHQ.</summary>
		public static void RunCustomers(Action a,bool useConnectionStore=true) {
			if(useConnectionStore) {
				Run(a,ConnectionNames.CustomersHQ);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.CustomersHQ);
				Run(a,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","",DatabaseType.MySql);
			}
		}

		///<summary>HQ only. Perform the given action in the context of the eServices db.</summary>
		public static void RunEServices(Action a) {
			Run(a,ConnectionNames.ServicesHQ);
		}

		///<summary>HQ only. Perform the given action in the context of the headmaster db.</summary>
		public static void RunHeadmaster(Action a) {
			Run(a,ConnectionNames.Headmaster);
		}

		///<summary>Perform the given action in the context of the manual publisher db (jordans_mp)
		///Leave useConnectionStore false to use the ConnectionSettingsHQ preference or the sitelink override for ConnectionNames.ManualPublisher.</summary>
		public static void RunManualPublisherHQ(Action a,bool useConnectionStore=false) {
			if(useConnectionStore) {
				Run(a,ConnectionNames.ManualPublisher);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.ManualPublisher);
				Run(a,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","");
			}
		}

		///<summary>Perform the given action as a Middle Tier client using OpenDentalServerMockIIS.</summary>
		public static void RunMiddleTierMock(Action a) {
			RemotingRole remotingRolePrevious=RemotingClient.RemotingRole;
			OpenDentBusiness.WebServices.OpenDentalServerMockIIS mockPrevious=OpenDentBusiness.WebServices.OpenDentalServerProxy.MockOpenDentalServerCur;
			if(mockPrevious==null) {
				OpenDentBusiness.WebServices.OpenDentalServerProxy.MockOpenDentalServerCur=new OpenDentBusiness.WebServices.OpenDentalServerMockIIS();
			}
			RemotingClient.RemotingRole=RemotingRole.ClientWeb;
			try {
				a();
			}
			finally {
				OpenDentBusiness.WebServices.OpenDentalServerProxy.MockOpenDentalServerCur=mockPrevious;
				RemotingClient.RemotingRole=remotingRolePrevious;
			}
		}		

		///<summary>Perform the given action in the context of the webforms db.</summary>
		public static void RunMobileWebForms(Action a) {
			Run(a,ConnectionNames.WebForms);
		}

		///<summary>Perform the given action in the context of the old mobile web db.</summary>
		public static void RunMobileWebOld(Action a) {
			Run(a,ConnectionNames.MobileWebOld);
		}

		///<summary>Perform the given action in the context of the dental office db.</summary>
		public static void RunPractice(Action a) {
			Run(a,ConnectionNames.DentalOffice);
		}

		///<summary>Perform the given action in the context of the remote support db.</summary>
		public static void RunRemoteSupport(Action a,bool useConnectionStore=false) {
			if(useConnectionStore) {
				Run(a,ConnectionNames.RemoteSupport);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.RemoteSupport);
				Run(a,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","");
			}
		}

		///<summary>HQ only.  Perform the given action in the context of the webchat db.
		///Set useConnectionStore false to use the ConnectionSettingsHQ preference or the sitelink override for ConnectionNames.WebChat.</summary>
		public static void RunWebChat(Action a,bool useConnectionStore=false) {
			if(useConnectionStore) {
				Run(a,ConnectionNames.WebChat);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.WebChat);
				Run(a,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","",DatabaseType.MySql);
			}
		}
		#endregion

		#region Funcs
		///<summary>Perform the given function in the context of the bugs db.</summary>
		public static T GetBugsHQ<T>(Func<T> fn,bool useConnectionStore=true) {
			if(useConnectionStore) {
				return GetT(fn,ConnectionNames.BugsHQ);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.BugsHQ);
				return GetT(fn,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","");
			}
		}

		///<summary>Perform the given function in the context of the customers db.</summary>
		public static T GetCustomers<T>(Func<T> fn) {
			return GetT(fn,ConnectionNames.CustomersHQ);
		}

		///<summary>Perform the given function in the context of the serviceshq db.</summary>
		public static T GetEServices<T>(Func<T> fn) {
			return GetT(fn,ConnectionNames.ServicesHQ);
		}

		///<summary>Perform the given function in the context of the hosting db.</summary>
		public static T GetHosting<T>(Func<T> fn) {
			return GetT(fn,ConnectionNames.Hosting);
		}

		///<summary>Perform the given function in the context of the dental office db.</summary>
		public static T GetPractice<T>(Func<T> fn) {
			return GetT(fn,ConnectionNames.DentalOffice);
		}

		///<summary>Perform the given function in the context of the remote support db.</summary>
		public static T GetRemoteSupport<T>(Func<T> fn,bool useConnectionStore=false) {
			if(useConnectionStore) {
				return GetT(fn,ConnectionNames.RemoteSupport);
			}
			else {
				CentralConnection con=GetHqConnection(ConnectionNames.RemoteSupport);
				return GetT(fn,con.ServerName,con.DatabaseName,con.MySqlUser,con.MySqlPassword,"","");
			}
		}
		#endregion
		#endregion

		#region Helpers to return typed data.
		///<summary>Perform the given function in the context of the given connectionName db and return a DataTable. Typed extension of GetT.</summary>
		public static DataTable GetDataTable(Func<DataTable> fn,ConnectionNames connectionName) {
			return GetT<DataTable>(fn,connectionName);
		}

		///<summary>Perform the given function in the context of the given connectionName db and return an int. Typed extension of GetT.</summary>
		public static int GetInt(Func<int> fn,ConnectionNames connectionName) {
			return GetT<int>(fn,connectionName);
		}

		///<summary>Perform the given function in the context of the given connectionName db and return a long. Typed extension of GetT.</summary>
		public static long GetLong(Func<long> fn,ConnectionNames connectionName) {
			return GetT<long>(fn,connectionName);
		}

		///<summary>Perform the given function in the context of the given connectionName db and return a string. Typed extension of GetT.</summary>
		public static string GetString(Func<string> fn,ConnectionNames connectionName) {
			//String is a reference type and will be set to null if the method happens to fail. Correct that to empty string since OD does not typically expect string to be null.
			return GetT<string>(fn,connectionName)??"";
		}
		#endregion

		#region Run and Get
		///<summary>Perform the given action in the context of the given connectionName db.</summary>
		public static void Run(Action a,ConnectionNames connectionName) {
			GetT(new Func<object>(() => { a(); return null; }),connectionName);
		}

		///<summary>Perform the given action in the context of the given connectionString db.</summary>
		public static void Run(Action a,string connectionString,DatabaseType dbType=DatabaseType.MySql) {
			GetT(new Func<object>(() => { a(); return null; }),connectionString,dbType);
		}
		
		///<summary>Perform the given action in the context of the given connectionString db.</summary>
		public static void Run(Action a,string server,string db,string user,string password,string userLow,string passLow,DatabaseType dbType=DatabaseType.MySql) {
			GetT(new Func<object>(() => { a(); return null; }),server,db,user,password,userLow,passLow,dbType);
		}

		///<summary>Perform the given function in the context of the given connectionString db and return a T.</summary>
		public static T GetT<T>(Func<T> fn,string connectionString,DatabaseType dbType=DatabaseType.MySql) {
			T ret=default(T);
			ExecuteThread(new ODThread((o) => {
				using(DataConnection dataConn=new DataConnection()) {
					dataConn.SetDbT(connectionString,"",dbType);
					ret=fn();
				}
			}));
			return ret;
		}

		///<summary>Perform the given function in the context of the given connectionString db and return a T.</summary>
		public static T GetT<T>(Func<T> fn,string server,string db,string user,string password,string userLow,string passLow,DatabaseType dbType=DatabaseType.MySql) {
			T ret=default(T);
			ExecuteThread(new ODThread((o) => {
				using(DataConnection dataConn=new DataConnection()) {
					dataConn.SetDbT(server,db,user,password,userLow,passLow,dbType,true);
					ret=fn();
				}
			}));
			return ret;
		}

		///<summary>Perform the given function in the context of the given connectionName db and return a T.</summary>
		public static T GetT<T>(Func<T> fn,ConnectionNames connectionName) {
			T ret=default(T);
			ExecuteThread(new ODThread((o) => {
				using(DataConnection dataConn=new DataConnection()) {
					ConnectionStore.SetDbT(connectionName,dataConn:dataConn);
					ret=fn();
				}
			}));
			return ret;
		}

		///<summary>Adds an exception handler to the thread passed in, starts the thread, and then waits until the thread has finished executing.
		///This is just a helper method that will throw any exception that occurs within the thread on the parent (usually main) thread.
		///Throws exceptions.</summary>
		private static void ExecuteThread(ODThread thread) {
			Exception eFinal=null;
			thread.AddExceptionHandler((e) => { eFinal=e; });
			thread.Start(true);
			//This is intended to be a blocking call so give the action as long as it needs to complete.
			thread.Join(Timeout.Infinite);
			if(eFinal!=null) { //We are back on the main thread so it is safe to throw.
				MiscUtils.PreserveExceptionInfoAndThrow(eFinal);
			}
		}
		#endregion
	}
}
