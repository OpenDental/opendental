using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Xml.Serialization;
using CodeBase;
using DataConnectionBase;
using System.Xml;

namespace OpenDentBusiness {
	///<summary>Thread-safe access to a list of connection store object which is retreived from a given file. If Init is not called then looks for ConnectionStore.xml in working directory.</summary>
	public class ConnectionStore {

		static ConnectionStore() {
			//Because ConnectionStoreBase does not have access to classes in OpenDentBusiness, we need to "override" a few methods to inject logic from 
			//OpenDentBusiness.
			ConnectionStoreBase.SetRemotingT=(string serverURI,bool isReportServer) => {
				RemotingClient.SetRemotingT(serverURI,RemotingRole.ClientWeb,isReportServer);
			};
			ConnectionStoreBase.SetServerURI=(string serverURI) => {
				RemotingClient.ServerURI=serverURI;
			};
			ConnectionStoreBase.GetDentalOfficeReportServerFromPrefC=() => {
				//Be aware that if PrefC cache is not already filled and/or DataConnection.SetDb() has not already been called, this will fail.
				CentralConnectionBase cn=null;
				if(ODBuild.IsWeb() && PrefC.ReportingServer.Server!="" && PrefC.ReportingServer.Database!=DataConnection.GetDatabaseName()) {
					//Security safeguard to prevent Web users from connecting to another office's database.
					throw new ODException("Report server database name must match current database.");
				}
				ODException.SwallowAnyException(() => {
					//give regular server credentials if the report server is not set up.
					cn=new CentralConnectionBase();
					cn.ServerName=PrefC.ReportingServer.Server=="" ? DataConnection.GetServerName() : PrefC.ReportingServer.Server;
					cn.DatabaseName=PrefC.ReportingServer.Server=="" ? DataConnection.GetDatabaseName() : PrefC.ReportingServer.Database;
					cn.MySqlUser=PrefC.ReportingServer.Server=="" ? DataConnection.GetMysqlUser() : PrefC.ReportingServer.MySqlUser;
					cn.MySqlPassword=PrefC.ReportingServer.Server=="" ? DataConnection.GetMysqlPass() : PrefC.ReportingServer.MySqlPass;
					//no ternary operator because URI will be blank if they're not using a middle tier reporting server.
					cn.ServiceURI=PrefC.ReportingServer.URI;
					//Connection string is not currently supported for report servers.
					//If ServerName is null or empty, then the current instance of Open Dental is utilizing a connection string.
					//The connection string should be preserved in order for reports to continue to work for non-report server queries.
					if(string.IsNullOrEmpty(cn.ServerName)) {
						cn.ConnectionString=PrefC.ReportingServer.ConnectionString=="" ? DataConnection.GetConnectionString() : PrefC.ReportingServer.ConnectionString;
					}
				});
				return cn;
			};
			ConnectionStoreBase.GetCustomersHQFromPrefC=() => {
				//If PrefC cache is not already filled and/or DataConnection.SetDb() has not already been called, this will fail.
				CentralConnectionBase cn=null;
				ODException.SwallowAnyException(() => {
					if(!PrefC.IsODHQ) {
						return;
					}
					cn=new CentralConnectionBase {
						ServerName=ODBuild.IsDebug() ? "localhost" : PrefC.GetString(PrefName.CustomersHQServer),
						DatabaseName=PrefC.GetString(PrefName.CustomersHQDatabase),
						MySqlUser=PrefC.GetString(PrefName.CustomersHQMySqlUser),
					};
					CDT.Class1.Decrypt(PrefC.GetString(PrefName.CustomersHQMySqlPassHash),out cn.MySqlPassword);
				});
				return cn;
			};
		}

		public new static ConnectionNames CurrentConnection {
			get {
				return ConnectionStoreBase.CurrentConnection;
			}
		}

		///<summary>Private ctor prevents this class from being instansiated. We will just use the class for static init.</summary>
		private ConnectionStore() {
		}

		///<summary>Get a central connection by name.</summary>
		public static OpenDentBusiness.CentralConnection GetConnection(ConnectionNames name) {
			return (CentralConnection)ConnectionStoreBase.GetConnection(name);
		}

		///<summary>Overrides any current connection with the connection passed in.  This should rarely be used.  E.g. used in Unit Testing.</summary>
		public static void OverrideConnection(ConnectionNames name,CentralConnection centralConnection) {
			ConnectionStoreBase.OverrideConnection(name,(CentralConnectionBase)centralConnection);
		}

		///<summary>Sets the connection of the current thread to the ConnectionName indicated. Connection details will be retrieved from ConnectionStore.xml.</summary>
		public static OpenDentBusiness.CentralConnection SetDb(ConnectionNames dbName,bool skipValidation=false) {
			return (CentralConnection)ConnectionStoreBase.SetDb(dbName,skipValidation);
		}

		///<summary>Sets the connection of the current thread to the ConnectionName indicated. Connection details will be retrieved from ConnectionStore.xml.</summary>
		public static OpenDentBusiness.CentralConnection SetDbT(ConnectionNames dbName,DataConnection dataConn=null) {
			return (CentralConnection)ConnectionStoreBase.SetDbT(dbName,dataConn);
		}

	}
}
