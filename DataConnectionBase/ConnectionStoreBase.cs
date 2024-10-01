using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;

namespace DataConnectionBase {
	///<summary>Used to retrieve connections from a configuration file. If you have access to OpenDentBusiness, you should call methods in OpenDentBusiness.ConeectionStore because that class contains additional logic relevant to OpenDentBusiness.</summary>
	public class ConnectionStoreBase {
		public delegate void SetServerURIDelegate(string serverURI);
		public delegate void SetRemotingTDelegate(string serverURI,bool isReportServer,bool isReadOnlyServer);
		///<summary>Called when the server URI for middle tier is set in a thread-specific context.</summary>
		public static SetRemotingTDelegate SetRemotingT=(string serverURI,bool isReportServer,bool isReadOnlyServer) => { };
		///<summary>Called when the server URI for middle tier is set NOT in a thread-specific context.</summary>
		public static SetServerURIDelegate SetServerURI=(string serverURI) => { };
		///<summary>Called when getting the DentalOfficeReportServer settings from PrefC.</summary>
		public static Func<CentralConnectionBase> GetDentalOfficeReportServerFromPrefC=() => { return null; };
		///<summary>Called when getting the DentalOfficeReadOnlyServer settings from PrefC.</summary>
		public static Func<CentralConnectionBase> GetDentalOfficeReadOnlyServerFromPrefC=() => { return null; };
		///<summary>Called when getting the TriageHQ settings. This is intentionally set to null and does not return a valid null function like the existing report and read-only patterns. This is so the Triage connection can be explicitly defined later in the initialization process of the parent project. E.g. Open Dental instantiates this within the Shown event handler.</summary>
		public static Func<CentralConnectionBase> GetTriageHQ=null;
		///<summary>The current database connection.</summary>
		private static ConnectionNames _currentConnection=ConnectionNames.DentalOffice;
		///<summary>The current database connection. Specific to this thread.</summary>
		[ThreadStatic]
		private static ConnectionNames _currentConnectionT;
		private static object _lock=new object();
		///<summary>Only used by GetDictCentConnSafe. Do not use elsewhere in this class.</summary>
		private static Dictionary<ConnectionNames,CentralConnectionBase> _dictCentConnUnsafe;

		///<summary>The current database connection.</summary>
		public static ConnectionNames CurrentConnection {
			get {
				if(_currentConnectionT==ConnectionNames.None) {
					return _currentConnection;
				}
				return _currentConnectionT;
			}
		}

		///<summary>Uses for thread-safe internal access.</summary>
		private static Dictionary<ConnectionNames,CentralConnectionBase> GetDictCentConnSafe() {
			//This action is just a glorified private method. It does a bunch of dirty work that would need to be copy/pasted multiple times otherwise.
			//Any callers of this action should guard against re-entry by only returning non-null one time (or whenever absolutely necessary).
			Action<Func<Dictionary<ConnectionNames,CentralConnectionBase>>> actionTryInit=new Action<Func<Dictionary<ConnectionNames, CentralConnectionBase>>>((f) => {
				//Try to load the file.
				Dictionary<ConnectionNames,CentralConnectionBase> dictionaryNew=null;
				ODException.SwallowAnyException(new Action(() => { dictionaryNew=f(); }));
				if(dictionaryNew==null) { //Load failed or we already found valid connection previously so don't continue.
					return;
				}
				lock(_lock) { //We got this far so set/merge the global dict.
					if(_dictCentConnUnsafe==null) { //First time, set the global dict.
						_dictCentConnUnsafe=dictionaryNew;
					}
					//Merge the new dict with the global dict.
					foreach(KeyValuePair<ConnectionNames,CentralConnectionBase> kvp in dictionaryNew) {
						if(!_dictCentConnUnsafe.ContainsKey(kvp.Key)) { //First in wins, so only add if we don't already have this entry.
							_dictCentConnUnsafe[kvp.Key]=kvp.Value;
						}
					}
				}
			});
			//Try to init each connection file. The first one that loads successfully will be given priority. All other attempts will fail silently and the first file's contents will persist.							
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //Windows forms and service applications.
				string path=CodeBase.ODFileUtils.CombinePaths(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),"ConnectionStore.xml");
				return InitConnectionStoreXml(path);
			}));
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //Web applications.
				string path=CodeBase.ODFileUtils.CombinePaths(GetWebAppPath(),"ConnectionStore.xml");
				return InitConnectionStoreXml(path);
			}));
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //Windows forms and service applications.
				string path=CodeBase.ODFileUtils.CombinePaths(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),"OpenDentalWebConfig.xml");
				return InitOpenDentalWebConfigXml(path);
			}));
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //Web applications.
				string path=CodeBase.ODFileUtils.CombinePaths(GetWebAppPath(),"OpenDentalWebConfig.xml");
				return InitOpenDentalWebConfigXml(path);
			}));
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //Dental Office Report Server (via prefs).
				if(HasSingleEntry(ConnectionNames.DentalOfficeReportServer)) { //Only try to add this value if it doesn't already exist.
					return null;
				}
				CentralConnectionBase cn=GetDentalOfficeReportServerFromPrefC();
				//Not already there so add it once.
				return new Dictionary<ConnectionNames,CentralConnectionBase>() { { ConnectionNames.DentalOfficeReportServer,cn??new CentralConnectionBase() } };
			}));
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //Dental Office Read-Only Server (via prefs).
				if(HasSingleEntry(ConnectionNames.DentalOfficeReadOnlyServer)) { //Only try to add this value if it doesn't already exist.
					return null;
				}
				CentralConnectionBase cn=GetDentalOfficeReadOnlyServerFromPrefC();
				//Not already there so add it once.
				return new Dictionary<ConnectionNames,CentralConnectionBase>() { { ConnectionNames.DentalOfficeReadOnlyServer,cn??new CentralConnectionBase() } };
			}));
			actionTryInit(new Func<Dictionary<ConnectionNames,CentralConnectionBase>>(() => { //TriageHQ (if implemented).
				//GetTriageHQ defaults to null instead of a valid func that returns null so that the parent project can dictate when a TriageHQ connection can be added to the dictionary.
				//E.g. Open Dental instantiates GetTriageHQ within the Shown event handler instead of a static constructor like the report and read-only funcs.
				if(GetTriageHQ==null || HasSingleEntry(ConnectionNames.TriageHQ)) {//TriageHQ is not implemented or has already been added.
					return null;
				}
				CentralConnectionBase cn=GetTriageHQ();
				//Not already there so add it once.
				return new Dictionary<ConnectionNames,CentralConnectionBase>() { { ConnectionNames.TriageHQ,cn??new CentralConnectionBase() } };
			}));
			Dictionary<ConnectionNames,CentralConnectionBase> dictRet=null;
			lock(_lock) {
				dictRet=_dictCentConnUnsafe;
			}
			return dictRet;
		}

		///<summary>Sets the current dictionary of connections to null so that it reinitializes all connections the next time it is accessed.
		///This is mainly used for connections that utilize preferences so that the dictionary can be up to date.</summary>
		public static void ClearConnectionDictionary() {
			lock(_lock) {
				_dictCentConnUnsafe=null;
			}
		}

		///<summary>Returns true if any ConnectionName entries have been loaded; otherwise returns false.</summary>
		public static bool HasAnyEntries {
			get {
				int entryCount=0;
				lock(_lock) {
					entryCount=_dictCentConnUnsafe==null ? 0 : _dictCentConnUnsafe.Count;
				}
				return entryCount>0;
			}
		}

		///<summary>Returns true if the connectionName entry has been loaded; otherwise returns false.</summary>
		public static bool HasSingleEntry(ConnectionNames connectionName) {
			bool ret=false;
			lock(_lock) {
				ret=_dictCentConnUnsafe!=null&&_dictCentConnUnsafe.ContainsKey(connectionName);
			}
			return ret;
		}

		///<summary>Gets the path to the root web path for the application.</summary>
		public static string GetWebAppPath()  {
#if DOT_NET_STANDARD
			//If we ever have .NET Core web apps that use connection store, we will have to implement this.
			throw new NotImplementedException(); 
#else
			return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
#endif
		}

		///<summary>Initializes central connection store from a given ConnectionStore formatted file. Throws exceptions if file not found or init fails for any other reason.</summary>
		private static Dictionary<ConnectionNames,CentralConnectionBase> InitConnectionStoreXml(string fullPath) {
			return InitConnectionsFromXmlFile<ListCentralConnections,CentralConnectionBase>(fullPath,new Func<CentralConnectionBase,CentralConnectionBase>((conn) => {
				if(!string.IsNullOrEmpty(conn.MySqlPassword)) {
					if(!CDT.Class1.Decrypt(conn.MySqlPassword,out conn.MySqlPassword)) {
						throw new Exception("Unable to decrypt MySQL password: "+fullPath);
					}
				}
				return conn;
			}));
		}

		///<summary>Initializes central connection store from a given OpenDentalWebConfig file. Throws exceptions if file not found or init fails for any other reason.</summary>
		private static Dictionary<ConnectionNames,CentralConnectionBase> InitOpenDentalWebConfigXml(string fullPath) {
			return InitConnectionsFromXmlFile<ConnectionSettings,DatabaseConnection>(fullPath,new Func<DatabaseConnection,CentralConnectionBase>((conn) => {
				if(!string.IsNullOrEmpty(conn.Password)) {
					if(!CDT.Class1.Decrypt(conn.Password,out conn.Password)) {
						throw new Exception("Unable to decrypt MySQL password: "+fullPath);
					}
				}
				return new CentralConnectionBase() {
					CentralConnectionNum=0,
					ConnectionStatus="",
					DatabaseName=conn.Database,
					ItemOrder=0,
					MySqlPassword=conn.Password,
					MySqlUser=conn.User,
					Note=conn.Note,
					OdPassword="",
					OdUser="",
					ServerName=conn.ComputerName,
					SslCa=conn.SslCa,
					ServiceURI="",
					WebServiceIsEcw=false
				};
			}));
		}

		///<summary>Initializes central connection store from a given xml file. Throws exceptions if file not found or init fails for any other reason.</summary>
		/// <typeparam name="FILETYPE">Must extend IConnectionFile. Instance will be created by deserializing the given xml file.</typeparam>
		/// <typeparam name="ITEMTYPE">The type of item defined by the given IConnectionFile type.</typeparam>
		/// <param name="fullPath">Full file path to the xml file.</param>
		/// <param name="funcGetConnectionFromItem">Function that will take in an instance of ITEMTYPE and return an instance of CentralConnectionBase.</param>
		private static Dictionary<ConnectionNames,CentralConnectionBase> InitConnectionsFromXmlFile<FILETYPE, ITEMTYPE>(string fullPath,
			Func<ITEMTYPE,CentralConnectionBase> funcGetConnectionFromItem) where FILETYPE : IConnectionFile<ITEMTYPE> 
		{
			if(HasAnyEntries) { //Prevent re-entry. Only want to run this once.
				return null;
			}
			if(!File.Exists(fullPath)) {
				throw new Exception("ConnectionStore file not found: "+fullPath);
			}
			//Deserialize the file.
			FILETYPE fromFile;
			using(XmlReader reader=XmlReader.Create(fullPath)) {
				fromFile=(FILETYPE)new XmlSerializer(typeof(FILETYPE)).Deserialize(reader);
			}
			Dictionary<ConnectionNames,CentralConnectionBase> dictionaryReturn=new Dictionary<ConnectionNames,CentralConnectionBase>();
			//Loop through each item in the file and convert it to a CentralConnectionBase.
			foreach(ITEMTYPE item in fromFile.Items) {
				CentralConnectionBase centralConnectionBase=funcGetConnectionFromItem(item);
				ConnectionNames connName;
				//Note field must deserialize to a ConnectionNames enum value.
				if(Enum.TryParse<ConnectionNames>(centralConnectionBase.Note,out connName)) {
					dictionaryReturn[connName]=centralConnectionBase;
				}
			}
			return dictionaryReturn;
		}

		///<summary>Get a central connection by name.</summary>
		public static CentralConnectionBase GetConnection(ConnectionNames connectionName) {
			CentralConnectionBase centralConnectionBase=null;
			lock(_lock) {
				if(_dictCentConnUnsafe!=null && _dictCentConnUnsafe.ContainsKey(connectionName)) {
					centralConnectionBase=_dictCentConnUnsafe[connectionName];
				}
			}
			if(centralConnectionBase!=null) {
				//This connection name has been loaded into the dictionary of connections before.
				//No need to read in any config files searching for it, just return it.
				return centralConnectionBase;
			}
			//Search through all known config files and preferences for this connection name.
			Dictionary<ConnectionNames,CentralConnectionBase> dictionary=GetDictCentConnSafe();
			if(dictionary==null) {
				throw new Exception("Critical error occurred when looking for connection name: "+connectionName);
			}
			if(!dictionary.TryGetValue(connectionName,out centralConnectionBase)) {
				throw new Exception("Connection name not found: "+connectionName);
			}
			return centralConnectionBase;
		}

		///<summary>Overrides any current connection with the connection passed in.  This should rarely be used.  E.g. used in Unit Testing.</summary>
		public static void OverrideConnection(ConnectionNames name,CentralConnectionBase CentralConnectionBase) {
			lock(_lock) {
				if(_dictCentConnUnsafe==null) {
					_dictCentConnUnsafe=new Dictionary<ConnectionNames,CentralConnectionBase>();
				}
				_dictCentConnUnsafe[name]=CentralConnectionBase;
			}
		}

		///<summary>Sets the connection of the current thread to the ConnectionName indicated. Connection details will be retrieved from ConnectionStore.xml.</summary>
		public static CentralConnectionBase SetDb(ConnectionNames dbName,bool skipValidation=false) {
			CentralConnectionBase conn=GetConnection(dbName);
			_currentConnection=dbName;
			if(!string.IsNullOrEmpty(conn.ServiceURI)) {
				SetServerURI(conn.ServiceURI);
			}
			else {
				new DataConnection().SetDb(conn.ServerName,conn.DatabaseName,conn.MySqlUser,conn.MySqlPassword,"","",DatabaseType.MySql,skipValidation);
			}
			return conn;
		}

		///<summary>Sets the connection of the current thread to the ConnectionName indicated. Connection details will be retrieved from ConnectionStore.xml.</summary>
		public static CentralConnectionBase SetDbT(ConnectionNames dbName,DataConnection dataConn=null) {
			dataConn=dataConn??new DataConnection();
			CentralConnectionBase conn=GetConnection(dbName);
			_currentConnectionT=dbName;
			if(!string.IsNullOrEmpty(conn.ServiceURI)) {
				SetRemotingT(conn.ServiceURI,dbName==ConnectionNames.DentalOfficeReportServer,dbName==ConnectionNames.DentalOfficeReadOnlyServer); 
			}
			else if(!string.IsNullOrEmpty(conn.ConnectionString)) {
				dataConn.SetDbT(conn.ConnectionString,"",DatabaseType.MySql);
			}
			else if(conn.ServerName==null){
				return null;
			}
			else {
				dataConn.SetDbT(conn.ServerName,conn.DatabaseName,conn.MySqlUser,conn.MySqlPassword,"","",DatabaseType.MySql,true,sslCa:conn.SslCa);
			}
			return conn;
		}

		///<summary>Instance will be created from a deserialized xml string.</summary>
		/// <typeparam name="ITEMTYPE">Type type of list items included in this file.</typeparam>
		public interface IConnectionFile<ITEMTYPE> {
			List<ITEMTYPE> Items {
				get; set;
			}
		}

		[XmlRoot("ListCentralConnections")]
		public class ListCentralConnections:IConnectionFile<CentralConnectionBase> {
			//Example File Format.
			/*
			<ListCentralConnectionBases>
				<CentralConnectionBase>
					<CentralConnectionBaseNum>0</CentralConnectionBaseNum>
					<ServerName>localhost</ServerName>
					<DatabaseName>serviceshq</DatabaseName>
					<MySqlUser>root</MySqlUser>
					<MySqlPassword></MySqlPassword>
					<ServiceURI></ServiceURI>
					<OdUser></OdUser>
					<OdPassword></OdPassword>
					<Note>ServicesHQ</Note>
					<ItemOrder>0</ItemOrder>
					<WebServiceIsEcw>0</WebServiceIsEcw>
				</CentralConnectionBase>
			</ListCentralConnectionBases>
			*/

			///<summary>Interface property. Overriding here allows us to define the XML element name which to look for in the file.</summary>
			[XmlElement("CentralConnection")]
			public List<CentralConnectionBase> Items {
				get; set;
			}
			public ListCentralConnections() {
				Items=new List<CentralConnectionBase>();
			}
		}

		[XmlRoot("ConnectionSettings")]
		public class ConnectionSettings:IConnectionFile<DatabaseConnection> {
			//Example File Format.
			/*
			<?xml version="1.0"?>
			<ConnectionSettings>
				<DatabaseConnection>
					<ComputerName>localhost</ComputerName>
					<Database>serviceshq</Database>
					<User>root</User>
					<Password></Password>
					<UserLow>root</UserLow>
					<PasswordLow></PasswordLow>
					<DatabaseType>MySql</DatabaseType>
					<Note>ServicesHQ</Note>
				</DatabaseConnection>
			</ConnectionSettings>
			*/

			///<summary>Interface property. Overriding here allows us to define the XML element name which to look for in the file.</summary>
			[XmlElement("DatabaseConnection")]
			public List<DatabaseConnection> Items {
				get; set;
			}
			public ConnectionSettings() {
				Items=new List<DatabaseConnection>();
			}
		}

		public class DatabaseConnection {
			///<summary>If direct db connection.  Can be ip address.</summary>
			public string ComputerName;
			///<summary>If direct db connection.</summary>
			public string Database;
			///<summary>High permissions user name.</summary>
			public string User;
			///<summary>High permissions password.</summary>
			public string Password;
			///<summary>Low permissions user name.</summary>
			public string UserLow;
			///<summary>Low permissions password.</summary>
			public string PasswordLow;
			///<summary>Used for connecting to MariaDB SkySQl for SSL secured connections</summary>
			public string SslCa;
			///<summary>String representation of database type.</summary>
			public string DatabaseType;
			///<summary>Must deserialize to a ConnectionNames enum value.</summary>
			public string Note;

			[XmlIgnore]
			public DatabaseType DbType {
				get {
					DatabaseType dbType;
					if(!Enum.TryParse<DatabaseType>(this.DatabaseType,out dbType)) {
						dbType=DataConnectionBase.DatabaseType.MySql;
					}
					return dbType;
				}
			}
		}
	}
}
