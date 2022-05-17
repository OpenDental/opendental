using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CodeBase;
using System.Text.RegularExpressions;

namespace DataConnectionBase {
	///<summary></summary>
	public class DataConnection:IDisposable {
		///<summary>This data adapter is used for all queries to the database.</summary>
		private MySqlDataAdapter _da;
		///<summary>This is the connection that is used by the data adapter for all queries.  8/30/2010 js Changed this to be not static so that we can use it with multiple threads.  Has potential to cause bugs.</summary>
		private MySqlConnection _con;
		///<summary>Used to get very small bits of data from the db when the data adapter would be overkill.  For instance retrieving the response after a command is sent.</summary>
		private MySqlDataReader _dr;
		///<summary>Stores the string of the command that will be sent to the database.</summary>
		private MySqlCommand _cmd;
		///<summary>After inserting a row, this variable will contain the primary key for the newly inserted row.  This can frequently save an additional query to the database.</summary>
		public long InsertID;
		private static string _database;
		private static string _serverName;
		private static string _mysqlUser;
		private static string _mysqlPass;
		//User with lower privileges:
		private static string _mysqlUserLow;
		private static string _mysqlPassLow;
		///<summary>If this is used, then none of the fields above will be set.</summary>
		private static string _connectionString="";
		///<summary>The value here is now reliable for public use.  FormChooseDatabase.DBtype, which used to be used for the client is now gone.</summary>
		private static DatabaseType _dBtype;
		//ThreadStatic Variables are thread specific and are thread safe thus do not require locking.
		[ThreadStatic]
		private static string _databaseT;
		[ThreadStatic]
		private static string _serverNameT;
		[ThreadStatic]
		private static string _mysqlUserT;
		[ThreadStatic]
		private static string _mysqlPassT;
		[ThreadStatic]
		private static string _mysqlUserLowT;
		[ThreadStatic]
		private static string _mysqlPassLowT;
		///<summary>If this is used, then none of the fields above will be set.</summary>
		[ThreadStatic]
		private static string _connectionStringT="";
		[ThreadStatic]
		private static DatabaseType _dBtypeT;
		private static int _commandTimeout=0;
		///<summary>Will be set to true if a DataConnectionEvent signifying the connection has been restored is received.</summary>
		private bool _isConnectionRestored;
		///<summary>The number of seconds to automatically retry connection to the database when the connection has been lost. Defaults
		///to 0 seconds. This value will be used if _connectionRetrySecondsT is set to 0.</summary>
		private static int _connectionRetryTimeoutSeconds=0;
		///<summary>The number of seconds that the thread will automatically retry connection to the database when the connection has been lost. Defaults
		///to 0 seconds. Must be set intentionally from every thread that wants to wait for a connection to be re-established. This value will override
		///_connectionRetrySeconds if not 0.</summary>
		[ThreadStatic]
		private static int _connectionRetryTimeoutSecondsT=0;
		///<summary>This flag indicates that the connection will try to reconnect if connection is lost.</summary>
		public static bool CanReconnect=true;
		///<summary>Dictates if the automatic retry threads should quit once the "retry timeout" has been reached
		///in the event of a database exception that is explicitly handled (e.g. lost connection).
		///When set to false, the automatic retry threads will keep the current thread "locked" here waiting on outside entites to "unlock" them once
		///the corresponding timeout has passed.  This is usually desireable for applications with a UI.
		///When set to true, the automatic retry threads will bubble up any UE that caused them to start once the timeout has been reached.
		///This is desireable for applications without a UI (e.g. the eConnector).</summary>
		public static bool DoThrowOnAutoRetryTimeout=false;
		///<summary>Represents a bool with a third state representing unset.  -1=unset, 0=false, 1=true.  Thread override for _canReconnect.</summary>
		[ThreadStatic]
		public static int CanReconnectT=-1;
		///<summary>Will be set to the table name of the table that has been reported as crashed.</summary>
		private string _crashedTableName;
		///<summary>Will be set to true if a CrashedTableEvent has detected a crashed table.</summary>
		private bool _isCrashedTableOK;
		///<summary>The number of seconds that should be spent monitoring crashed tables.  0 will not monitor crashed tables.</summary>
		private static double _crashedTableTimeoutSeconds=0;
		///<summary>The number of seconds that should be spent monitoring crashed tables.  0 will not monitor crashed tables.</summary>
		[ThreadStatic]
		private static double _crashedTableTimeoutSecondsT=0;
		///<summary>The number of seconds that should be spent monitoring when a null dataReader exception is encountered.  0 will not monitor when this
		///exception is encountered.</summary>
		public static double DataReaderNullTimeoutSeconds=0;

		///<summary>milliseconds. Debug only.</summary>
		private static int _delayForTesting=0;
		private static bool _logDebugQueries=false;

		#region Properties

		///<summary>Only call from the main thread.  The value here is now reliable for public use.  FormChooseDatabase.DBtype, which used to be used for the client is now gone.</summary>
		public static DatabaseType DBtype {
			get {
				//We need to know if thread variables are being used (SetDbT was called).  Because DBType is an enum we do not have a way to check if _dBtypeT is "null or empty".
				//Check if _databaseT and _connectionStringT are null or empty which will indicate that we need to return _dBtype (set by the main thread) instead of _dBtypeT (thread specific).
				if(String.IsNullOrEmpty(_databaseT) && String.IsNullOrEmpty(_connectionStringT)) {
					//This will often get hit by separate threads that were spawned from the main thread and did not use SetDbT.  E.g. FormOpenDental.ThreadEmailInbox
					//They need to follow old behavior and use the old static, non-thread safe variables that are assumed to "never change" except on startup.
					return _dBtype;
				}
				return _dBtypeT;
			}
			set {
				_dBtype=value;
				_dBtypeT=value;
			}
		}

		//=====================================================================================================================================================
		// The following properties MUST first check if the thread static variables are null or empty which will cause the non thread safe static variables
		// to be returned.  This is because the main thread of Open Dental is written in a way that SetDb is only called once (on startup) and then
		// the static connection settings are used afterwards for subsequent connections / database calls.
		// Individual threads that need to access different databases (CEMT) need to call SetDbT before making db calls.
		//=====================================================================================================================================================
		
		public static int CommandTimeout {
			get {
				if(_commandTimeout==0) {
					_commandTimeout=3600;//Default to 1 hour
				}
				return _commandTimeout;
			}
			set {
				_commandTimeout=value;
				if(!string.IsNullOrEmpty(ServerName)) {
					ConnectionString=BuildSimpleConnectionString(DBtype,ServerName,Database,MysqlUser,MysqlPass);
				}
			}
		}

		private static string Database {
			get {
				if(String.IsNullOrEmpty(_databaseT)) {
					return _database;
				}
				else {
					return _databaseT;
				}
			}
			set {
				_database=value;
				_databaseT=value;
			}
		}

		private static string ServerName {
			get {
				if(String.IsNullOrEmpty(_serverNameT)) {
					return _serverName;
				}
				else {
					return _serverNameT;
				}
			}
			set {
				_serverName=value;
				_serverNameT=value;
			}
		}

		private static string MysqlUser {
			get {
				if(String.IsNullOrEmpty(_mysqlUserT)) {
					return _mysqlUser;
				}
				else {
					return _mysqlUserT;
				}
			}
			set {
				_mysqlUser=value;
				_mysqlUserT=value;
			}
		}

		private static string MysqlPass {
			get {
				if(_mysqlPassT==null) {//If _mysqlPassT=="", then we still want to use _mysqlPassT.
					return _mysqlPass;
				}
				else {
					return _mysqlPassT;
				}
			}
			set {
				_mysqlPass=value;
				_mysqlPassT=value;
			}
		}

		private static string MysqlUserLow {
			get {
				if(String.IsNullOrEmpty(_mysqlUserLowT)) {
					return _mysqlUserLow;
				}
				else {
					return _mysqlUserLowT;
				}
			}
			set {
				_mysqlUserLow=value;
				_mysqlUserLowT=value;
			}
		}

		private static string MysqlPassLow {
			get {
				if(_mysqlPassLowT==null) {//If _mysqlPassLowT=="", then we still want to use _mysqlPassLowT.
					return _mysqlPassLow;
				}
				else {
					return _mysqlPassLowT;
				}
			}
			set {
				_mysqlPassLow=value;
				_mysqlPassLowT=value;
			}
		}

		private static string ConnectionString {
			get {
				if(String.IsNullOrEmpty(_connectionStringT)) {
					return _connectionString;
				}
				else {
					return _connectionStringT;
				}
			}
			set {
				_connectionString=value;
				_connectionStringT=value;
			}
		}

		///<summary>The name of the server for this instance of DataConnection. This may be different than the static or thread static field.</summary>
		public string ServerNameCur {
			get; private set;
		}

		///<summary>The name of the MySQL user for this instance of DataConnection. This may be different than the static or thread static field.
		///</summary>
		public string UserCur {
			get; private set;
		}

		///<summary>The number of seconds that the thread will automatically retry connection to the database when the connection has been lost. Defaults
		///to 0 seconds.</summary>
		public static int ConnectionRetryTimeoutSeconds {
			get {
				if(_connectionRetryTimeoutSecondsT!=0) {
					return _connectionRetryTimeoutSecondsT;
				}
				return _connectionRetryTimeoutSeconds;
			}
			set {
				_connectionRetryTimeoutSeconds=value;
				_connectionRetryTimeoutSecondsT=value;
			}
		}

		///<summary>Sets the number of seconds to retry establishing a connection. Thread specific.</summary>
		public static int ConnectionRetryTimeoutSecondsT {
			//get should use ConnectionRetryTimeoutSeconds
			set {
				_connectionRetryTimeoutSecondsT=value;
			}
		}

		///<summary>The number of seconds that should be spent monitoring crashed tables.  0 will not monitor crashed tables.</summary>
		public static double CrashedTableTimeoutSeconds {
			get {
				if(_crashedTableTimeoutSecondsT!=0) {
					return _crashedTableTimeoutSecondsT;
				}
				return _crashedTableTimeoutSeconds;
			}
			set {
				_crashedTableTimeoutSeconds=value;
				_crashedTableTimeoutSecondsT=value;
			}
		}

		///<summary>The number of seconds that should be spent monitoring crashed tables.  0 will not monitor crashed tables. Thread specific.</summary>
		public static double CrashedTableTimeoutSecondsT {
			//get should use CrashedTableTimeoutSeconds
			set {
				_crashedTableTimeoutSecondsT=value;
			}
		}

		///<summary>True if a connecion has been made to the database.</summary>
		public static bool HasDatabaseConnection {
			get {
				return !string.IsNullOrEmpty(GetServerName()) || !string.IsNullOrEmpty(GetConnectionString());
			}
		}
		#endregion

		#region Getters

		public static int DefaultPortNum() {
			switch(DBtype) {
				case DatabaseType.Oracle:
					return 1521;
				case DatabaseType.MySql:
					return 3306;
				default:
					return 3306;//Guess (same as MySQL, to target larger customer crowd).
			}
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetConnectionString() {
			return ConnectionString;
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetDatabaseName() {
			return Database;
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetServerName() {
			return ServerName;
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetMysqlUser() {
			return MysqlUser;
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetMysqlPass() {
			return MysqlPass;
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetMysqlUserLow() {
			return MysqlUserLow;
		}

		///<summary>From local variable.  Does not query the database.</summary>
		public static string GetMysqlPassLow() {
			return MysqlPassLow;
		}

		///<summary>This flag indicates that the connection will try to reconnect if connection is lost.  Thread specific.</summary>
		public static bool GetCanReconnect() {
			bool canReconnect=CanReconnect;
			if(CanReconnectT!=-1) {
				canReconnect=(CanReconnectT!=0);
			}
			return canReconnect;
		}

		#endregion

		#region Constructors
		///<summary>Instantiates a new MySQL database connection utilizing the current static connection variables.</summary>
		public DataConnection() : this(false) { }

		///<summary>Instantiates a new MySQL database connection utilizing the current static connection variables.
		///Uses MysqlUserLow and MysqlPassLow if isLow is set to true as to connect with the user with less permissions.</summary>
		public DataConnection(bool isLow) 
			: this(ConnectionString,ServerName,Database,MysqlUser,MysqlPass,MysqlUserLow,MysqlPassLow,DatabaseType.MySql,isLow) { }

		///<summary>Does nothing and assumes the calling method will invoke SetDb or other similar method that will create an actual connection.</summary>
		///<param name="dbtype">Not used.  This parameter simply causes the constructor signature to be different.</param>
		public DataConnection(DatabaseType dbtype) { }

		///<summary>Instantiates a new database connection utilizing the current static connection variables except for the database.
		///The database that is passed in will always be used.  Use a different constructor if you don't intend on overriding the database.
		///Used when making a backup and when updating multiple databases via the "Simultaneously update other databases" setting.
		///A safer and better paradigm for switching database context would be to spawn a new thread and utilize SetDbT().</summary>
		public DataConnection(string database) : this(ServerName,database,MysqlUser,MysqlPass,DBtype) { }

		///<summary>Instantiates a new database connection utilizing the connection variables passed in.</summary>
		public DataConnection(string serverName,string database,string mysqlUser,string mysqlPass,DatabaseType dtype)
			: this ("",serverName,database,mysqlUser,mysqlPass,dtype) { }

		///<summary>Instantiates a new database connection utilizing the connection variables passed in.
		///connectStr will be used unless it is null or empty AND serverName is not null.</summary>
		public DataConnection(string connectStr,string serverName,string database,string mysqlUser,string mysqlPass,DatabaseType dtype)
			: this(connectStr,serverName,database,mysqlUser,mysqlPass,"","",dtype,false) { }

		///<summary>Instantiates a new database connection utilizing the connection variables passed in.
		///isLow being true will forcefully use mysqlUserLow and mysqlPassLow when building the connection string for the database connection.
		///connectStr is never used when isLow is true; however, connectStr will be used unless it is null or empty AND serverName is not null.<summary>
		public DataConnection(string connectStr,string serverName,string database,string mysqlUser,string mysqlPass,
			string mysqlUserLow,string mysqlPassLow,DatabaseType dtype,bool isLow)
		{
			#region ODThread Database Context
			//All new threads that are created will need to know about the database context of their parent thread.
			//Set the global static database context func and action within ODThread if they haven't been set already (only need to be set once).
			if(ODThread.GetDatabaseContextParent==null) {
				//The database context getter will be executed on the parent thread and is designed to store this current database context within ODThread.
				//The magic behind this func is that ConnectionString can return a ThreadStatic variable which will be the correct database context.
				ODThread.GetDatabaseContextParent=() => { return ConnectionString; };
			}
			if(ODThread.SetDatabaseContextChild==null) {
				//The database context setter will be executed on the child thread and is designed to sync the database context of the child with the parent.
				//Since we were the ones that just set the func above we can unbox the object passed to us as a string.
				ODThread.SetDatabaseContextChild=(databaseContext) => {
					//For now the databaseContext will only be the ConnectionString of the parent thread.
					//It should be enhanced in the future to contain all necessary information to make intelligent decisions on how to invoke SetDbT().
					//E.g. we should probably be considering MysqlUserLow and MysqlPassLow or at the very least preserving it onto the child thread.
					if(databaseContext==null || !(databaseContext is string) || string.IsNullOrEmpty((string)databaseContext)) {
						return;
					}
					using(DataConnection dataConn=new DataConnection()) {
						dataConn.SetDbT((string)databaseContext,"",DBtype,true,true);
					}
				};
			}
			#endregion
			if(isLow) {
				//Forcefully use mysqlUserLow and mysqlPassLow regardless if a connectStr was passed in or not.
				//There is no such thing as a "connectiong string low" so it needs to fail if the lower user credentials were not passed in correctly.
				connectStr=BuildSimpleConnectionString(dtype,serverName,database,mysqlUserLow,mysqlPassLow);
				ServerNameCur=serverName;
				UserCur=mysqlUserLow;
			}
			//isLow is false so utilize the higher level mysqlUser but only if an invalid connectStr and a valid serverName was passed in.
			else if(string.IsNullOrEmpty(connectStr) && serverName!=null) {
				connectStr=BuildSimpleConnectionString(dtype,serverName,database,mysqlUser,mysqlPass);
				ServerNameCur=serverName;
				UserCur=mysqlUser;
			}
			_con=new MySqlConnection(connectStr);
			_cmd=new MySqlCommand();
			_cmd.Connection=_con;
		}
		#endregion Constructors

		#region ConnectionString

		public static string BuildSimpleConnectionString(DatabaseType pDbType,string pServer,string pDatabase,string pUserID,string pPassword) {
			string serverName=pServer;
			string port=DefaultPortNum().ToString();
			if(pServer.Contains(":")) {
				string[] serverNamePort=pServer.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);
				serverName=serverNamePort[0];
				port=serverNamePort[1];
			}
			string connectStr="";
			connectStr=
				"Server="+serverName
				+";Port="+port//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+";Database="+pDatabase
				//+";Connect Timeout=20"
				+";User ID="+pUserID
				+";Password="+pPassword
				+";SslMode=none"
				+";CharSet=utf8"
				+";Treat Tiny As Boolean=false"
				+";Allow User Variables=true"
				+";Convert Zero Datetime=true"//Convert all MySQL dates 0000-00-00 to 0001-01-01, since C# crashes for 0000-00-00 dates.
				+";Default Command Timeout="+CommandTimeout;//one hour timeout on commands.  Prevents crash during conversions, etc.
			//+";Pooling=false";
			return connectStr;
		}

		private string BuildSimpleConnectionString(string pServer,string pDatabase,string pUserID,string pPassword) {
			return BuildSimpleConnectionString(DBtype,pServer,pDatabase,pUserID,pPassword);
		}

		public static string GetCurrentConnectionString() {
			DataConnection dcon=new DataConnection();
			return dcon._con.ConnectionString;
		}

		///<summary>Returns a connection string constructed with the lower privileged MySQL user and password.</summary>
		public static string GetLowConnectionString() {
			return BuildSimpleConnectionString(DatabaseType.MySql,ServerName,Database,MysqlUserLow,MysqlPassLow);
		}

		public static string GetReportConnectionString(string pServer,string pDatabase,string pUserID,string pPassword) {
			DataConnection rdcon=new DataConnection(pServer
				,pDatabase
				,pUserID
				,pPassword
				,DatabaseType.MySql);
			return rdcon._con.ConnectionString;
		}

		#endregion

		#region SetDb and SetDbT

		///<summary>This needs to be run every time we switch databases, especially on startup.  Will throw an exception if fails.  Calling class should catch exception.</summary>
		public void SetDb(string server,string db,string user,string password,string userLow,string passLow,DatabaseType dbtype) {
			SetDb(server,db,user,password,userLow,passLow,dbtype,false);
		}

		///<summary>This needs to be run every time we switch databases, especially on startup.  Will throw an exception if fails.  Calling class should catch exception.</summary>
		public void SetDb(string server,string db,string user,string password,string userLow,string passLow,DatabaseType dbtype,bool skipValidation) {
			DBtype=dbtype;
			string connectStr=BuildSimpleConnectionString(server,db,user,password);
			string connectStrLow="";
			if(userLow!="") {
				connectStrLow=BuildSimpleConnectionString(server,db,userLow,passLow);
			}
			TestConnection(connectStr,connectStrLow,dbtype,skipValidation);
			//connection strings must be valid, so OK to set permanently
			ConnectionString=connectStr;
			Database=db;
			ServerName=server;//yes, it includes the port
			MysqlUser=user;
			MysqlPass=password;
			MysqlUserLow=userLow;
			MysqlPassLow=passLow;
			ServerNameCur=server;
			UserCur=user;
		}

		///<summary>This needs to be run every time we switch databases, especially on startup.  Will throw an exception if fails.  Calling class should catch exception.</summary>
		public void SetDb(string connectStr,string connectStrLow,DatabaseType dbtype,bool skipValidation) {
			TestConnection(connectStr,connectStrLow,dbtype,skipValidation);
			//connection string must be valid, so OK to set permanently
			ConnectionString=connectStr;
		}

		///<summary></summary>
		public void SetDb(string connectStr,string connectStrLow,DatabaseType dbtype) {
			SetDb(connectStr,connectStrLow,dbtype,false);
		}

		///<summary>This method sets all the thread specific variables for the DataConnection.  It will leave all normal static connection variables.  
		///Should be called before connecting to a database from a thread outside of the main thread. Will validate both high and low permission connection by running an arbitrary query against each.</summary>
		public void SetDbT(string server,string db,string user,string password,string userLow,string passLow,DatabaseType dbtype) {
			SetDbT(server,db,user,password,userLow,passLow,dbtype,false);
		}
		
		///<summary>This method sets all the thread specific variables for the DataConnection.  It will leave all normal static connection variables.  
		///Should be called before connecting to a database from a thread outside of the main thread. Can optionally validate both high and low permission connections by running an arbitrary query against each.</summary>
		public void SetDbT(string server,string db,string user,string password,string userLow,string passLow,DatabaseType dbtype,bool skipValidation) {
			_dBtypeT=dbtype;
			string connectStr=BuildSimpleConnectionString(server,db,user,password);
			string connectStrLow="";
			if(userLow!="") {
				connectStrLow=BuildSimpleConnectionString(server,db,userLow,passLow);
			}
			TestConnection(connectStr,connectStrLow,dbtype,skipValidation);
			//connection strings must be valid, so OK to set permanently
			_connectionStringT=connectStr;
			_databaseT=db;
			_serverNameT=server;//yes, it includes the port
			_mysqlUserT=user;
			_mysqlPassT=password;
			_mysqlUserLowT=userLow;
			_mysqlPassLowT=passLow;
			ServerNameCur=server;
			UserCur=user;
		}

		///<summary>This method sets all the thread specific variables for the DataConnection.  It will leave all normal static connection variables.
		///Should be called before connecting to a database from a thread outside of the main thread.
		///Optionally skip testing the connection strings passed in for scenarios where it doesn't matter if the database context is unavailable.
		///E.g. new threads need to preserve parent database context but shouldn't crash the program if the database is currently unavailable.</summary>
		public void SetDbT(string connectStr,string connectStrLow,DatabaseType dbtype,bool skipValidation,bool skipTestConnection=false) {
			if(!skipTestConnection) {
				TestConnection(connectStr,connectStrLow,dbtype,skipValidation);
			}
			//connection string must be valid, so OK to set permanently
			_connectionStringT=connectStr;
		}

		///<summary>This method sets all the thread specific variables for the DataConnection.  It will leave all normal static connection variables.  Should be called before connecting to a database from a thread outside of the main thread.</summary>
		public void SetDbT(string connectStr,string connectStrLow,DatabaseType dbtype) {
			SetDbT(connectStr,connectStrLow,dbtype,false);
		}

		#endregion

		private void TestConnection(string connectStr,string connectStrLow,DatabaseType dbtype,bool skipValidation) {
			DBtype=dbtype;
			_con=new MySqlConnection(connectStr);
			_cmd=new MySqlCommand();
			_cmd.Connection=_con;
			_con.Open();
			if(!skipValidation) {
				_cmd.CommandText="UPDATE preference SET ValueString = '0' WHERE ValueString = '0'";
				RunMySqlAction(new Action(() => _cmd.ExecuteNonQuery()));
			}
			_con.Close();
			if(connectStrLow!="") {
				_con=new MySqlConnection(connectStrLow);
				_cmd = new MySqlCommand();
				_cmd.Connection=_con;
				_con.Open();
				if(!skipValidation) {
					_cmd.CommandText="SELECT * FROM preference WHERE ValueString = 'DataBaseVersion'";
					DataTable table=new DataTable();
					_da=new MySqlDataAdapter(_cmd);
					RunMySqlAction(new Action(() => _da.Fill(table)));
				}
				_con.Close();
			}
		}

		public static bool IsTableCrashed(string tableName,bool doRetryConn=false) {
			try {
				using(DataConnection dconn=new DataConnection()) {
					DataTable table=dconn.GetTable($"CHECK TABLE `{tableName}`",doRetryConn);//No need to POut.String when tableName surrounded by back quotes.
					return (table.Rows[0]["Msg_text"].ToString().Trim().ToUpper()!="OK");//Any Msg_text other than 'OK' means the table is crashed.
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;
			}
		}

		///<summary>Fills table with data from the database.</summary>
		public DataTable GetTable(string command,bool doAutoRetry=true) {
			if(ODBuild.IsDebug()) {
				if(_logDebugQueries) {
					Debug.WriteLine(command);
				}
				Thread.Sleep(_delayForTesting);
			}
			DataTable table=new DataTable();
			_cmd.CommandText=command;
			try {
				_con.Open();
				_da=new MySqlDataAdapter(_cmd);
				RunMySqlAction(new Action(() => _da.Fill(table)));
			}
			catch(MySqlException ex) {//MySqlExceptions are the only ones we want to catch.
				if(doAutoRetry && IsErrorHandled(ex)) {
					_con.Close();
					return GetTable(command,doAutoRetry);
				}
				throw ex;//Throw the exception for any child threads to preserve functionality.
			}
			_con.Close();
			return table;
		}

		public List<T> GetList<T>(string command,Func<IDataRecord,T> rowToObj,bool doAutoRetry=true) {
			if(ODBuild.IsDebug()) {
				if(_logDebugQueries) {
					Debug.WriteLine(command);
				}
				Thread.Sleep(_delayForTesting);
			}
			List<T> retval=new List<T>();
			_cmd.CommandText=command;
			try {
				_con.Open();
				RunMySqlAction(new Action(() => {
					_dr=_cmd.ExecuteReader();
					while(_dr.Read()) {
						retval.Add(rowToObj(_dr));
					}
				}));
			}
			catch(MySqlException ex) {//MySqlExceptions are the only ones we want to catch.
				if(doAutoRetry && IsErrorHandled(ex)) {
					_con.Close();
					_dr.Close();
					return GetList(command,rowToObj,doAutoRetry);
				}
				throw ex;//Throw the exception for any child threads to preserve functionality.
			}
			finally {
				_dr.Close();
				_con.Close();
			}
			return retval;
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. 
		///If getInsertID is true, then InsertID will be set to the value of the primary key of the newly inserted row.   
		///WILL NOT RETURN CORRECT PRIMARY KEY for MySQL if the query specifies the primary key.
		///Pass in the PK column and table names so that Oracle can correctly lock the table and know which column to return for the Insert ID.</summary>
		public long NonQ(string commands,bool getInsertID,string columnNamePK,string tableName,bool doRetryConn,params MySqlParameter[] parameters) {
			if(ODBuild.IsDebug()) {
				if(_logDebugQueries) {
					Debug.WriteLine(commands);
				}
				Thread.Sleep(_delayForTesting);
			}
			long rowsChanged=0;
			_cmd.CommandText=commands;
			if(parameters.Length > 0) {
				_cmd.Parameters.Clear();//This collection can contain old items when retrying a command after connection loss.
			}
			for(int p=0;p<parameters.Length;p++) {
				_cmd.Parameters.Add(parameters[p]).Value=parameters[p].Value;
			}
			try {
				_con.Open();
				RunMySqlAction(new Action(() => rowsChanged=_cmd.ExecuteNonQuery()));
				InsertID=_cmd.LastInsertedId;//This field was not reliable prior to using version 6.9.9 of the MySQL connector
			}
			catch(MySqlException ex) {//MySqlExceptions are the only ones we want to catch.
				if(ex.Number==1153) {
					throw new ApplicationException("Please add the following to your my.ini file: max_allowed_packet=40000000");
				}
				if(doRetryConn && IsErrorHandled(ex)) {
					_con.Close();
					return NonQ(commands,getInsertID,"","",doRetryConn,parameters);//All other threads need to handle their own exceptions, if no connection.
				}
				throw ex;//Throw the exception for any child threads to preserve functionality.
			}
			_con.Close();
			return rowsChanged;
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. If true, then InsertID will be set to the value of the primary key of the newly inserted row.   WILL NOT RETURN CORRECT PRIMARY KEY if the query specifies the primary key.</summary>
		public long NonQ(string commands,bool getInsertID,params MySqlParameter[] parameters) {
			return NonQ(commands,getInsertID,"","",true,parameters);
		}

		///<summary>Sends a non query command to the database and returns the number of rows affected. If true, then InsertID will be set to the value of the primary key of the newly inserted row.</summary>
		public long NonQ(string command) {
			return NonQ(command,false);
		}

		///<summary>Use this for count(*) queries.  They are always guaranteed to return one and only one value.  Uses datareader instead of datatable, so faster.  Can also be used when retrieving prefs manually, since they will also return exactly one value</summary>
		public string GetCount(string command,bool doRetryConn=true) {
			if(ODBuild.IsDebug()) {
				if(_logDebugQueries) {
					Debug.WriteLine(command);
				}
				Thread.Sleep(_delayForTesting);
			}
			string retVal="";
			_cmd.CommandText=command;
			try {
				_con.Open();
				RunMySqlAction(new Action(() => _dr=(MySqlDataReader)_cmd.ExecuteReader()));
				_dr.Read();
				retVal=_dr[0].ToString();
			}
			catch(MySqlException ex) {//MySqlExceptions are the only ones we want to catch.
				if(doRetryConn && IsErrorHandled(ex)) {
					_con.Close();
					return GetCount(command,doRetryConn);//All other threads need to handle their own exceptions, if no connection.
				}
				throw ex;//Throw the exception for any child threads to preserve functionality.
			}
			_con.Close();
			return retVal;
		}

		///<summary>Get one value.</summary>
		public string GetScalar(string command,bool doRetryConn=true) {
			if(ODBuild.IsDebug()) {
				if(_logDebugQueries) {
					Debug.WriteLine(command);
				}
				Thread.Sleep(_delayForTesting);
			}
			object scalar=null;
			_cmd.CommandText=command;
			try {
				_con.Open();
				RunMySqlAction(new Action(() => scalar=_cmd.ExecuteScalar()));
			}
			catch(MySqlException ex) {
				//MySqlExceptions are the only ones we want to catch.
				if(doRetryConn && IsErrorHandled(ex)) {
					_con.Close();
					return GetScalar(command,doRetryConn);//All other threads need to handle their own exceptions, if no connection.
				}
				throw ex;//Throw the exception for any child threads to preserve functionality.
			}
			_con.Close();
			return (scalar==null ? "" : scalar.ToString());
		}

		///<summary>A method of bulk copying the data from the passed in datatable into the database table of the given tableName. This is primarily
		///used as a way of transferring data from one server to another, as we cannot backup data across servers as easily as for the same server.
		///Exports the datatable to a temporary ^ separated value file and then importing it to the database using MySqlBulkLoader.</summary>
		public void BulkCopy(DataTable dt, string tableName) {
			//export the table to a temp csv file
			string fileName=$"temp_{tableName+DateTime.Now.Ticks}.txt";
			StringBuilder fileContent=new StringBuilder();
			//Row data
			foreach(DataRow row in dt.Rows) {
				fileContent.AppendLine("\""+string.Join("\"^\"",row.ItemArray.Select(field =>
				//MySql DateTime format is different than the datetime that would normally be generated by ToString()
				field.GetType().Equals(typeof(DateTime)) ? ((DateTime)field).ToString("yyyy-MM-dd HH:mm:ss") : SOut.String(field.ToString())))+"\"" );
			}
			System.IO.File.WriteAllText(fileName,fileContent.ToString());
			//Load the file into the new database
			MySqlBulkLoader newTable=new MySqlBulkLoader(_con) {
				TableName=tableName,
				FileName=fileName,
				FieldTerminator="^",
				FieldQuotationCharacter='"',
				FieldQuotationOptional=false,
				LineTerminator=Environment.NewLine
			};
			newTable.Load();
            //Clean up the temp table we created
            System.IO.File.Delete(fileName);
		}

		///<summary>Oracle only. Used to split a command string into a list of individual commands so that each command can be run individually. It has proven difficult to run multiple commands at one time in Oracle without making drastic changes to existing queries.</summary>
		private string[] SplitCommands(string batchCmd) {
			if(DBtype!=DatabaseType.Oracle) {
				return new string[] { batchCmd };
			}
			//Some commands are surrounded by a BEGIN and END block, which does correctly execute in the Oracle connector and is hard for us to parse if there are nested BEGIN and END blocks.
			if(batchCmd.TrimStart().StartsWith("BEGIN",StringComparison.OrdinalIgnoreCase)) {
				return new string[] { batchCmd };
			}
			//Possibilities within a single statement:
			//Reference for Oracle Text Literals: http://download.oracle.com/docs/cd/B19306_01/server.102/b14200/sql_elements003.htm#SQLRF00218
			//Oracle allows one to use alternative quoting mechanisms instead of single-quotes, but we are not going to worry about that here.
			//In Oracle, one must escape ' with '' so we can just treat these like a string ending and a new one beginning immediately afterward.
			//Commands are terminated with semi-colons. We need to watch for the case when a semi-colon is within a single-quote string. As long as we correctly handle strings and ignore all data in a string, this shouldn't matter.
			//If batchCmd contains a command which has maulformed single or double quotes, this algorithm will not work so we will need to throw an exception in this case.
			List<string> result=new List<string>();
			StringBuilder strb=new StringBuilder();
			//bool beginningChar;
			//bool isInString;
			for(int i=0;i<batchCmd.Length;i++) {
				/*
				if(batchCmd[i]=='"') {
					if(isInString && beginningChar=='"') {
						isInString=false;
					}
					else {
						isInString=true;
					}
				}
				if(batchCmd[i]==";" && !isInString) {
					if(strb.Length>0){
						result.Add(strb.ToString();
					}
					strb=new StringBuilder();
					continue;
				}
				strb.Append(batchCmd[i]);*/
				if(batchCmd[i]=='\'') { //Single quotes are escaped with a single quote. So, for the string 'Hi I''m Bob' there are always an even number of single quotes.
					do {
						strb.Append(batchCmd[i++]);//Uses i then increments it.
						if(i>=batchCmd.Length) {
							throw new ApplicationException("Mismatched quotes found while splitting command.");
						}
					} while(batchCmd[i]!='\'');
					strb.Append(batchCmd[i]);
				}
				else if(batchCmd[i]==';') { //top-level ; so this is the end of a command.
					if(strb.Length>0) {
						result.Add(strb.ToString());
					}
					strb=new StringBuilder();
				}
				else { //All other characters
					strb.Append(batchCmd[i]);
				}
			}
			if(strb.Length>0) { //Make sure to add the last command if it did not have a ;
				result.Add(strb.ToString());
			}
			return result.ToArray();
		}

		///<summary>Run an action using the MySqlConnection and close the MySqlConnection if the action throws an exception. Re-throws the exception. This is strictly used to close the orphaned connection.</summary>
		private void RunMySqlAction(Action actionDb) {
			RunDbAction(actionDb,_con,_cmd);
		}

		///<summary>Run an action using the given connection and close the connection if the action throws an exception. Re-throws the exception.
		///This is strictly used to close the orphaned connection and to handle special exceptions.
		///Do not call this directly, instead use RunMySqlAction() or RunOracleAction().</summary>
		private static void RunDbAction(Action actionDb,System.Data.Common.DbConnection connection,System.Data.Common.DbCommand cmd) {
			try {
				QueryMonitor.Monitor.RunMonitoredQuery(actionDb,cmd);
			}
			catch(MySqlException mySqlEx) {
				#region Database Storage Too Full
				//This occurs when the servers storage is too full. Instead of saying, "got error 28 from storage engine", we will catch the error
				//and give them a better message to avoid them from calling us.
				if(mySqlEx.Number==1030 && mySqlEx.Message.ToLower()=="got error 28 from storage engine") {
					throw new Exception("The server's storage is full. Free space to avoid this error.",mySqlEx);
				}
				#endregion
				#region Retry Read Only Table Once
				//Recently we have been getting UEs from both HQ and customers that occurs when a table is marked as read-only by MySQL. Because we
				//generally only get one UE when this occurs, we can assume the table is marked as read-only for a very short time. Because of this, 
				//we decided to retry the query once instead of crashing the program.
				else if(mySqlEx.Number==1036) {
					RetryQuery(actionDb,connection,cmd,mySqlEx,"Query Read-only Table");
					return;
				}
				#endregion
				#region Retry Fatal Error & Host Failed to Respond Once
				//Users have been complaining about the "Query Execution Error" which is a customer error we throw when MySQL gives us the
				//"Fatal error encountered during command execution".  We have decided to simply "try again" in specific scenarios.
				//Related to the fatal error queries. Connection refuses or fails to respond for whatever reason. As the fatal errors have shown some 
				//success in working, we will try these once as well. Full exception text is:
				//Unable to read data from the transport connection: A connection attempt failed because the connected party did not properly respond 
				//after a period of time, or established connection failed because connected host has failed to respond.
				else if((mySqlEx.Message.ToLower().Contains("fatal error") || mySqlEx.Message.ToLower().Contains("transport connection"))
					&& CommandCanBeRetried(cmd)) 
				{
					string exceptionMessage="Query Execution Error - ";
					if(cmd.CommandText.ToLower().StartsWith("select")) {
						exceptionMessage+="'SELECT'";
					}
					else {//insert
						//The command text was checked for starting with "insert into securitylog" so there is a chance that the error is for securityloghash.
						//Get the first three words of the query so that we aren't mislead into thinking the failure was explicitly for securitylog if it wasn't.
						string[] commandWords=cmd.CommandText.Split(' ');
						if(commandWords.Length>=3) {
							exceptionMessage+="'"+commandWords[0]+" "+commandWords[1]+" "+commandWords[2]+"'";
						}
						else {
							exceptionMessage+="'INSERT INTO securitylog...'";//unknown table specificity.
						}
					}
					exceptionMessage+=" statement at "+DateTime.Now.ToString()+" local time - ";
					RetryQuery(actionDb,connection,cmd,mySqlEx,exceptionMessage);
					return;
				}
				#endregion
				//Close() will throw a different exception if it cannot close the connection. Swallow this and move on.
				ODException.SwallowAnyException(() => connection.Close());
				throw;//A different MySQL Exception, bubble it up.
			}
			catch(ArgumentNullException argNullEx) {
				if(argNullEx.ParamName=="dataReader"
					&& CommandCanBeRetried(cmd)) 
				{
					if(StartDataReaderNullMonitor(actionDb)) {
						return;
					}
				}
				throw;//A different ArgumentNullException, bubble it up.
			}
			catch(Exception ex) {
				ex.DoNothing();
				//Close() will throw a different exception if it cannot close the connection. Swallow this and move on.
				ODException.SwallowAnyException(() => connection.Close());
				throw;
			}
		}

		///<summary>Returns true if this MySQL command can be safely retried.</summary>
		private static bool CommandCanBeRetried(System.Data.Common.DbCommand cmd) {
			return cmd.CommandText.ToLower().StartsWith("select") || cmd.CommandText.ToLower().StartsWith("insert into securitylog");
		}

		#region MySQL Error Detection / Handling

		///<summary>Listens to see if the connection has been restored.</summary>
		private void DataConnectionEvent_Fired(DataConnectionEventArgs e) {
			if(e.IsConnectionRestored && e.ConnectionString==_con.ConnectionString) {
				_isConnectionRestored=true;
			}
		}

		///<summary>Listens to see if the table has reported the status OK.</summary>
		private void CrashedTableEvent_Fired(CrashedTableEventArgs e) {
			if(!e.IsTableCrashed && e.TableName==_crashedTableName) {
				_isCrashedTableOK=true;
				_crashedTableName="";
			}
		}

		///<summary>Handles certain types of MySQL errors. The application may pause here to wait for the error to be resolved.
		///Returns true if the calling method should retry the db action that just failed.  E.g. recursively invoke GetTable()
		///Returns false if the exception passed in is not specifically handled and the program should crash.</summary>
		private bool IsErrorHandled(MySqlException ex) {
			//Don't catch error 1153 (max_allowed_packet error), it will change program behavior if we do.
			if(IsConnectionLost(ex)) {
				//Pause the application here for a specified amount of time.
				return StartConnectionErrorRetry(DataConnectionEventType.ConnectionLost);
			}
			if(IsTooManyConnections(ex)) {
				//Pause the application here for a specified amount of time.
				return StartConnectionErrorRetry(DataConnectionEventType.TooManyConnections);
			}
			if(IsCrashedTable(ex,out _crashedTableName)) {
				//Pause the application here for a specified amount of time.
				return StartCrashedTableMonitor();
			}
			return false;
		}

		///<summary>Fires an event to launch the LostConnection window and freezes the calling thread until connection has been restored or the timeout
		///has been reached. This is only a blocking call when ConnectionRetrySeconds is greater than 0 or not the Middle Tier.
		///Immediately returns if ConnectionRetrySeconds is 0 or this is the Middle Tier that has lost connection to the database.
		///Returns true if the calling method should retry the db action that just failed.  E.g. recursively invoke GetTable()
		///Returns false if the calling method should instead bubble up the original exception.</summary>
		private bool StartConnectionErrorRetry(DataConnectionEventType errorType) {
			if(ConnectionRetryTimeoutSeconds==0 || !GetCanReconnect()) {
				return false;
			}
			//Important to use GetCurrentConnectionString() here instead of _con.ConnectionString.
			//_con.ConnectionString will be devoid of it's password after _con.Open() has been called.
			//This will yield authentication errors if we seed a new MySqlConnection (below) with _con.ConnectionString.
			//See https://stackoverflow.com/a/12467984.
			string connectionString=GetCurrentConnectionString();
			bool doRetry=true;
			//Register for the DataConnection event just in case another thread notices that the connection has come back before this method does.
			DataConnectionEvent.Fired+=DataConnectionEvent_Fired;
			//Notify anyone that cares that we are waiting here until the connection comes back online.
			//Typical consumers of this event will launch a connection lost window and turn off threads / timers until the connection has come back.
			DataConnectionEvent.Fire(new DataConnectionEventArgs(errorType,false,connectionString));
			//Keep the current thread stuck here while automatically retrying the db connection up until the timeout specified.
			DateTime beginning=DateTime.Now;
			ODThread threadRetry=new ODThread(500,(o) => {
				if(_isConnectionRestored) {
					o.QuitAsync();//Have this thread exit and join back with the main application.
					return;
				}
				if((DateTime.Now-beginning).TotalSeconds>=ConnectionRetryTimeoutSeconds) {//We have reached or exceeded the timeout.
					//Stop automatically retrying and bubble up the exception OR leave it up to a manual retry from the user.
					if(DoThrowOnAutoRetryTimeout) {
						doRetry=false;
						o.QuitAsync();
					}
					return;
				}
				try {
					using(DataConnection dconn=new DataConnection()) {
						dconn.TestConnection(connectionString,"",DBtype,skipValidation:false);
					}
					//An exception was not thrown by TestConnection() so the connection has been restored.
					_isConnectionRestored=true;
					//Also fire a DataConnectionEvent letting everyone who cares know that the connection has been restored.
					DataConnectionEvent.Fire(new DataConnectionEventArgs(DataConnectionEventType.ConnectionRestored,true,connectionString));
					o.QuitAsync();
				}
				catch(Exception ex) {
					ex.DoNothing();//Connection has not been restored yet.  Wait a little bit and then try again.
				}
			});
			threadRetry.Name="DataConnectionAutoRetryThread";
			threadRetry.AddExceptionHandler((e) => {
				e.DoNothing();
				//Unhandled exception so tell caller to throw. Letting caller continue would lead us right back here and potentially cause a stack overflow.
				doRetry=false;
			});
			threadRetry.Start();
			//Wait here until the automatic retry thread or the manual retry button has detected the connection as being restored.
			threadRetry.Join(Timeout.Infinite);
			DataConnectionEvent.Fired-=DataConnectionEvent_Fired;
			return doRetry;
		}

		///<summary></summary>
		private bool StartCrashedTableMonitor() {
			if(string.IsNullOrEmpty(_crashedTableName) || CrashedTableTimeoutSeconds==0) {
				return false;
			}
			bool doRetry=true;
			//Register for CrashedTableEvent events just in case another thread notices that the table is OK before this method does.
			CrashedTableEvent.Fired+=CrashedTableEvent_Fired;
			//Notify anyone that cares that we are waiting here until the table is OK.
			//Typical consumers of this event will launch a connection lost window and wait until the table is OK.
			CrashedTableEvent.Fire(new CrashedTableEventArgs(true,_crashedTableName));
			//Keep the current thread stuck here while automatically checking the status of the table up until the timeout specified.
			DateTime beginning=DateTime.Now;
			ODThread threadCrashedTableMonitor=new ODThread(500,(o) => {
				if(_isCrashedTableOK) {
					o.QuitAsync();//Have this thread exit and join back with the main application.
					return;
				}
				if((DateTime.Now-beginning).TotalSeconds>=CrashedTableTimeoutSeconds) {
					//Stop automatically retrying and bubble up the exception OR leave it up to a manual retry from the user.
					if(DoThrowOnAutoRetryTimeout) {
						doRetry=false;
						o.QuitAsync();
					}
					return;
				}
				if(!IsTableCrashed(_crashedTableName)) {
					_isCrashedTableOK=true;
					CrashedTableEvent.Fire(new CrashedTableEventArgs(false,_crashedTableName));
					o.QuitAsync();
				}
			});
			threadCrashedTableMonitor.Name="CrashedTableAutoRetryThread";
			threadCrashedTableMonitor.AddExceptionHandler((e) => {
				e.DoNothing();
				//Unhandled exception so tell caller to throw. Letting caller continue would lead us right back here and potentially cause a stack overflow.
				doRetry=false;
			});
			threadCrashedTableMonitor.Start();
			//Wait here until the retry thread has finished which is either due to connection being restored or the timeout was reached.
			threadCrashedTableMonitor.Join(Timeout.Infinite);//Wait forever because the retry thread has a timeout within itself.
			CrashedTableEvent.Fired-=CrashedTableEvent_Fired;
			return doRetry;
		}

		///<summary>Repeatedly retries the query that failed. Returns true if it is able to successfully complete the query.</summary>
		private static bool StartDataReaderNullMonitor(Action actionDb) {
			if(DataReaderNullTimeoutSeconds==0) {
				return false;
			}
			object lockRetry=new object();
			bool isQuerySuccessful=false;
			//This method will be called in the while loop below and also in FormConnectionLost when the user clicks 'Retry'. Because isQuerySuccessful is
			//captured in this method, a successful 'Retry' from FormConnectionLost will cause the while loop below to exit.
			#region RetryQuery
			bool RetryQuery() {
				try {
					//There will typically be multiple threads attempting the exact same action, which is designed to fill a DataTable, it should be locked.
					lock(lockRetry) {
						if(!isQuerySuccessful) {
							actionDb();
						}
						isQuerySuccessful=true;
					}
				}
				catch(ArgumentNullException argNullEx) {
					if(argNullEx.ParamName=="dataReader") {
						return false;
					}
					throw;//A different ArgumentNullException, bubble it up.
				}
				return isQuerySuccessful;
			}
			#endregion
			//Notify anyone that cares that we are waiting here until the query succeeds.
			//Typical consumers of this event will launch a connection lost window and wait until the query succeeds.
			DataReaderNullEvent.Fire(new DataReaderNullEventArgs(false,RetryQuery));
			DateTime startRetry=DateTime.Now;
			while(!isQuerySuccessful) {
				Thread.Sleep(500);
				if((DateTime.Now-startRetry).TotalSeconds>=DataReaderNullTimeoutSeconds) {
					//Stop automatically retrying and bubble up the exception OR leave it up to a manual retry from the user.
					if(DoThrowOnAutoRetryTimeout) {
						break;
					}
					continue;//Continue in this loop until the user retries successfully
				}
				isQuerySuccessful=RetryQuery();
			}
			if(isQuerySuccessful) {
				//Inform other consumers that the query has been successful.
				DataReaderNullEvent.Fire(new DataReaderNullEventArgs(true,() => true));
			}
			return isQuerySuccessful;
		}

		///<summary>Returns true if the MySQL connection has been lost.</summary>
		private bool IsConnectionLost(MySqlException ex) {
			if(((ex.Message.ToLower().Contains("stream") && ex.Message.ToLower().Contains("failed"))//Reading from the stream has failed 
					|| ex.Number==1042//Unable to connect to any of the specified MySQL hosts
					|| ex.Number==1045))//Access denied)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns true if the MySQL connection could not connect because there were too many connections to the server.</summary>
		private bool IsTooManyConnections(MySqlException ex) {
			//error from MySQL is "Too Many Connections" with a code of 1040
			if(ex.Message.ToLower().Contains("too many connections") && ex.Number==1040) { 
				return true;
			}
			return false;
		}

		///<summary>Returns true if the MySQL exception contains text that is common for a crashed table error and sets the tableName parameter.
		///Otherwise; false.</summary>
		private bool IsCrashedTable(MySqlException ex,out string tableName) {
			tableName="";
			//Error number: 1194; Symbol: ER_CRASHED_ON_USAGE; SQLSTATE: HY000
			//Message: Table '%s' is marked as crashed and should be repaired
			if(ex.ErrorCode!=1194 && !ex.Message.ToLower().Contains("is marked as crashed and should be repaired")) {
				return false;
			}
			//Try to extract the name of the table from the error messsage which will be between two single quotes.
			//If the name of the table could not be extracted then simply return false and allow the program to crash.
			Match match=Regex.Match(ex.Message,"\'(.*?)\'");
			if(!match.Success) {
				return false;
			}
			//Set the tableName parameter so that the calling method knows which table to monitor.
			//Sometimes the table name will be in an absolute format.  E.g. '.\databasename\tablename' 
			//Other times the table name will be alone.  E.g. 'tablename'
			tableName=match.Groups[1].Value.Split('\\').Last();
			return true;
		}

		///<summary>This method will retry a query once. This should only be used in special cases as generally if a query causes an exception,
		///there is a good reason for doing so.</summary>
		///<param name="actionDb">The given action that caused the exception. Will be ran again.</param>
		///<param name="connection">The connection used to run the query.</param>
		///<param name="cmd">The command object that will contain the query text being executed.</param>
		///<param name="mySqlEx">The original MySQL exception that was thrown.</param>
		///<param name="exceptionMessage">This is the message that will appear in HQ's bug submissions. The message will be followed by either 
		///Retry Successful or Retry Failure. E.g. Query Execution Error Retry Successful.</param>
		private static void RetryQuery(Action actionDb,System.Data.Common.DbConnection connection,System.Data.Common.DbCommand cmd,
			MySqlException mySqlEx,string exceptionMessage)
		{
			string commandTextRaw="";
			ODException.SwallowAnyException(() => {
				if(mySqlEx.Message.ToLower().Contains("fatal error")) {
					//The entire securityloghash query can be logged because there is never any PHI present.
					if(cmd.CommandText.ToLower().StartsWith("insert into securityloghash ")) {
						commandTextRaw=cmd.CommandText;
					}
					//Make sure that the LogText column is set to @paramLogText.  Otherwise, do not 
					else if(cmd.CommandText.ToLower().StartsWith("insert into securitylog ")) {
						//The LogText column is the only column that can contain PHI which is a query parameter when our CRUD is making the insert.
						//This will show up in the query string as "@paramLogText".  This is safe to upload the entire command to HQ in raw format.
						//Queries that are generated by our CRUD are formatted in a very predictable fashion;
						//^The 4th item will always be the column names definition.  E.g. "(SecurityLogNum,PermType,UserNum...)"
						//^The 5th item will always be the VALUES definition.  E.g. "VALUES(897489,83,1,NOW(),...)"
						//^^We REQUIRE the 5th item (values) to contain @paramLogText otherwise we will not log the query in raw format.
						//^^We know the 5th item will always contain the log text value because we split by space and our CRUD never has a space before LogText.
						string[] commandSplitBySpaces=cmd.CommandText.Split(' ');
						if(commandSplitBySpaces.Length>=5 && commandSplitBySpaces[4].Contains(",@paramLogText,")) {
							commandTextRaw=cmd.CommandText;
						}
						else {
							commandTextRaw="Non-CRUD insert statement trying to execute, not logging command on purpose.";
						}
					}
				}
			});
			//Surround the following in a try / catch so that we can tell HQ what happened regardless of any errors.
			try {
				//First, we will close the connection as depending on the situation, the connection may have already been forcefully closed.
				connection.Close();
				connection.Open();//Re-open the connection.
				actionDb();//Try the query one more time.
				return;//The retry attempt worked, so do not continue to bubble up the exception.  Let the user go back to work.
			}
			catch(Exception ex) {
				//Close() will throw a different exception if it cannot close the connection. Swallow this and move on.
				ODException.SwallowAnyException(() => connection.Close());
				//The retry attempt failed.  Pass along whatever UE occurred so that HQ can know about it as well (might be different, might be the same).
				//Throw a special type of ODException with a custom message in order to preserve the original StackTrace.
				//The first line of the custom UE's Message property will turn into the BugSubmission's ExceptionMessageText field.
				//All subsequent lines will become the BugSubmission's ExceptionStackTrace field.
				string message=exceptionMessage+" Retry Failure\r\n"//First Line, thus the ExceptionMessageText field...
					//Subsequent lines, thus the ExceptionStackTrace field...
						+(string.IsNullOrEmpty(commandTextRaw) ? "" : "Query Info: "+commandTextRaw+"\r\n")
					+"====================Retry Exception Information====================\r\n"+MiscUtils.GetExceptionText(ex)+"\r\n"
					+"====================MySQL Exception Information====================\r\n"+MiscUtils.GetExceptionText(mySqlEx);
				throw new ODException(message,ODException.ErrorCodes.BugSubmissionMessage);
			}
		}

		#endregion

		#region IDisposable Support
		///<summary>To detect redundant calls.</summary>
		private bool _disposedValue=false;

		///<summary>All disposable entites should be disposed here.</summary>
		protected virtual void Dispose(bool disposing) {
			if(_disposedValue) {
				return;
			}
			if(disposing) {
				ODException.SwallowAnyException(new Action(() => _con?.Close()));
				_con?.Dispose();//closes the connection if necessary
				ODException.SwallowAnyException(new Action(() => _dr?.Close()));
				_dr?.Dispose();
				_da?.Dispose();
				_cmd?.Dispose();
			}
			_disposedValue=true;
		}

		public void Dispose() {
			Dispose(true);
		}
		#endregion

	}

	///<summary></summary>
	public enum DatabaseType {
		MySql,
		Oracle
		//MS_Sql
	}

}
