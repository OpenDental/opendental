using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	///<summary>Used to send queries. The methods are internal since it is not acceptable for the UI to be sending queries.</summary>
	public class Db {

		///<summary>A thread safe and thread specific value containing the last SQL command attempted.</summary>
		[ThreadStatic]
		private static string _lastCommand;

		///<summary>The last SQL command attempted.</summary>
		public static string LastCommand {
			get	{
				return _lastCommand??"[COMMAND NOT SET]";
			}
			internal set {
				_lastCommand=value;
			}
		}

		///<summary>This is true if a connection to the database has been established.</summary>
		public static bool HasDatabaseConnection() {
			if(!string.IsNullOrEmpty(DataConnection.GetServerName())) {
				return true;
			}
			if(!string.IsNullOrEmpty(DataConnection.GetConnectionString())) {
				return true;
			}
			if(RemotingClient.ServerURI!=null) {//Connected middle tier
				return true;
			}
			return false;
		}

		///<summary>Returns false if mysql variable "AUTO_INCREMENT_INCREMENT" equals 1, otherwise true.</summary>
		public static bool IsAutoIncrementIncrementSetForReplication() {
			return (!ListTools.In(GetAutoIncrementIncrement(),1,-1));//auto_increment_increment is default value or not found (not found shouldn't happen, but for safety's sake).
		}

		/// <summary>Returns mysql variable "AUTO_INCREMENT_INCREMENT", or -1 if variable not found.</summary>
		public static int GetAutoIncrementIncrement() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SHOW VARIABLES LIKE 'auto_increment_increment'";
			DataTable table=GetTable(command);
			if(table.Rows.Count>0) {
				foreach(DataRow row in table.Rows) {
					if((string)row["Variable_name"]=="auto_increment_increment") {
						return PIn.Int((string)table.Rows[0]["Value"]);
					}
				}
			}
			return -1;//auto_increment_increment variable not found, just in case.
		}

		///<summary>Throws Exception.
		///Checks to see if the query is safe for replication and if the user has permission to run a command query if it is a command query.
		///Use isRunningOnReportServer to indicate if the connection is made directly to the Report Server.</summary>
		public static bool IsSqlAllowed(string command,bool suppressMessage=false,bool isRunningOnReportServer=false) {
			if(!IsSafeSqlForReplication(command,isRunningOnReportServer)) {
				return false;
			}
			bool isCommand;
			try {
				isCommand=IsCommandSql(command);
			}
			catch {
				throw new ApplicationException("Validation failed. Please remove mid-query comments and try again.");
			}
			if(isCommand && !Security.IsAuthorized(Permissions.CommandQuery,suppressMessage)) {
				return false;
			}
			if(isCommand) {
				SecurityLogs.MakeLogEntry(Permissions.CommandQuery,0,"Command query run.");
			}
			return true;
		}

		///<summary>Throws Exception.
		///Checks to see if the computer is allowed to use create table or drop table syntax queries.
		///Will return false if using replication and the computer OD is running on is not the ReplicationUserQueryServer set in replication setup.
		///Use isRunningOnReportServer to indicate if the connection is made directly to the Report Server(ReplicationUserQueryServer) such that the
		///query will be executed on the Report Server, even if the connection is initiated from a different computer.
		///Otherwise true.</summary>
		private static bool IsSafeSqlForReplication(string command,bool isRunningOnReportServer) {
			if(!PrefC.RandomKeys && !Db.IsAutoIncrementIncrementSetForReplication()) {//If replication is disabled, then any command is safe.
				//Previously users could set PrefName.RandomPrimaryKeys but there is no longer a UI for this.
				//PrefName.RandomPrimaryKeys use to be required for replication but this has since changed so we can not rely on this due to replication setup changes.
				return true;
			}
			bool isSafe=true;
			if(Regex.IsMatch(command,".*CREATE[\\s]+TABLE.*",RegexOptions.IgnoreCase)) {
				isSafe=false;
			}
			if(Regex.IsMatch(command,".*CREATE[\\s]+TEMPORARY[\\s]+TABLE.*",RegexOptions.IgnoreCase)) {
				isSafe=false;
			}
			if(Regex.IsMatch(command,".*DROP[\\s]+TABLE.*",RegexOptions.IgnoreCase)) {
				isSafe=false;
			}
			if(isSafe) {
				return true;
			}
			//At this point we know that replication is enabled and the command is potentially unsafe.
			if(PrefC.GetLong(PrefName.ReplicationUserQueryServer)==0) {//if no allowed ReplicationUserQueryServer set in replication setup
				throw new ApplicationException("This query contains unsafe syntax that can crash replication.  There is currently no computer set that is allowed to run these types of queries.  This can be set in the replication setup window.");
			}
			//If not known to be connected to the Report Server and if not running query from the ReplicationUserQueryServer set in replication setup 
			else if(!isRunningOnReportServer && !ReplicationServers.IsConnectedReportServer()) {
				throw new ApplicationException("This query contains unsafe syntax that can crash replication.  Only computers connected to the report server are allowed to run these queries.  The current report server can be found in the replication setup window.");
			}
			return true;
		}

		///<summary>Returns true if the given SQL script in strSql contains any commands (INSERT, UPDATE, DELETE, etc.). Surround with a try/catch.</summary>
		private static bool IsCommandSql(string strSql) {
			string trimmedSql=strSql.Trim();//If a line is completely a comment it may have only a trailing \n to make a subquery on. We need to keep it there.
			//splits the string while accounting for quotes and case/if/concat statements.
			string[] arraySqlExpressions = UserQueries.SplitQuery(UserQueries.RemoveSQLComments(trimmedSql).ToUpper(),false,";").ToArray(); 
			//Because of the complexities of parsing through MySQL and the fact that we don't want to take the time to create a fully functional parser
			//for our simple query runner we elected to err on the side of caution.  If there are comments in the middle of the query this section of
			//code will fire a UE.  This is due to the fact that without massive work we cannot intelligently discern if a comment is in the middle of
			//a string being used or if it is a legitimate comment.  Since we cannot know this we want to block more often than may be absolutely 
			//necessary to catch people doing anything that could potentially lead to SQL injection attacks.  We thus want to inform the user that simply
			//removing intra-query comments is the necessary fix for their problem.
			for(int i=0;i<arraySqlExpressions.Length;i++) {
				//Clean out any leading comments before we do anything else
				while(arraySqlExpressions[i].Trim().StartsWith("#") || arraySqlExpressions[i].Trim().StartsWith("--") || arraySqlExpressions[i].Trim().StartsWith("/*")) {
					if(arraySqlExpressions[i].Trim().StartsWith("/*")) {
						arraySqlExpressions[i]=arraySqlExpressions[i].Remove(0,arraySqlExpressions[i].IndexOf("*/")+3).Trim();
					}
					else {//Comment starting with # or starting with --
						int endIndex=arraySqlExpressions[i].IndexOf("\n");
						if(endIndex!=-1) {//This is so it doesn't break if the last line of a command is a comment
							arraySqlExpressions[i]=arraySqlExpressions[i].Remove(0,arraySqlExpressions[i].IndexOf("\n")).Trim();
						}
						else {
							arraySqlExpressions[i]=arraySqlExpressions[i].Remove(0,arraySqlExpressions[i].Length).Trim();
						}
					}
				}
				if(String.IsNullOrWhiteSpace(arraySqlExpressions[i])) {
					continue;//Ignore empty SQL statements.
				}
				if(arraySqlExpressions[i].Trim().StartsWith("SELECT")) {//We don't care about select queries
					continue;
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("SET")) {
					//We need to allow SET statements because we use them to set variables in our query examples.
					continue;
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("UPDATE")) {//These next we allow if they are on temp tables
					if(HasNonTempTable("UPDATE",arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("ALTER")) {
					if(HasNonTempTable("TABLE",arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("CREATE")) {//CREATE INDEX or CREATE TABLE or CREATE TEMPORARY TABLE
					int a=arraySqlExpressions[i].Trim().IndexOf("INDEX");
					int b=arraySqlExpressions[i].Trim().IndexOf("TABLE");
					string keyword="";
					if(a==-1 && b==-1) {
						//Invalid command.  Ignore.
					}
					else if(a!=-1 && b==-1) {
						keyword="INDEX";
					}
					else if(a==-1 && b!=-1) {
						keyword="TABLE";
					}
					else if(a!=-1 && b!=-1) {
						keyword=arraySqlExpressions[i].Trim().Substring(Math.Min(a,b),5);//Get the keyword that is closest to the front of the string.
					}
					if(keyword!="" && HasNonTempTable(keyword,arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("DROP")) { //DROP [TEMPORARY] TABLE [IF EXISTS]
					int a=arraySqlExpressions[i].Trim().IndexOf("TABLE");
					//We require exactly one space between these two keywords, because there are all sorts of technically valid ways to write the IF EXISTS which would create a lot of work for us.
					//Examples "DROP TABLE x IF    EXISTS ...", "DROP TABLE x IF /*comment IF EXISTS*/  EXISTS ...", "DROP TABLE ifexists IF EXISTS /*IF EXISTS*/"
					int b=arraySqlExpressions[i].Trim().IndexOf("IF EXISTS");
					string keyword="";
					if(a==-1 && b==-1) {
						//Invalid command.  Ignore.
					}
					else if(b==-1) {
						keyword="TABLE";//Must have TABLE if it's not invalid
					}
					else {
						keyword="IF EXISTS";//It has the IF EXISTS statement
					}
					if(keyword!="" && HasNonTempTable(keyword,arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("RENAME")) {
					if(HasNonTempTable("TABLE",arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("TRUNCATE")) {
					if(HasNonTempTable("TABLE",arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("DELETE")) {
					if(HasNonTempTable("DELETE",arraySqlExpressions[i])) {
						return true;
					}
				}
				else if(arraySqlExpressions[i].Trim().StartsWith("INSERT")) {
					if(HasNonTempTable("INTO",arraySqlExpressions[i])) {
						return true;
					}
				}
				else {//All the rest of the commands that we won't allow, even with temp tables, also includes if there are any additional comments embedded.
					return true;
				}
			}
			return false;
		}
		
		///<summary>The keywords must be listed in the order they are required to appear within the query.</summary>
		private static bool HasNonTempTable(string keyword,string command) {
			int keywordEndIndex=command.IndexOf(keyword)+keyword.Length;
			command=command.Remove(0,keywordEndIndex).Trim();//Everything left will be the table/s or nested queries.
			//Match one or more table names with optional alias for each table name, separated by commas.
			//A word in this contenxt is any string of non-space characters which also does not include ',' or '(' or ')'.
			Match m=Regex.Match(command,@"^([^\s,\(\)]+(\s+[^\s,\(\)]+)?(\s*,\s*[^\s,\(\)]+(\s+[^\s,\(\)]+)?)*)");
			string[] arrayTableNames=m.Result("$1").Split(',');
			for(int i=0;i<arrayTableNames.Length;i++) {//Adding matched strings to list
				string tableName=arrayTableNames[i].Trim().Split(' ')[0];
				if(!tableName.StartsWith("TEMP") && !tableName.StartsWith("TMP")) {//A table name that doesn't start with temp nor tmp (non temp table).
					return true;
				}
			}			
			return false;
		}

		///<summary></summary>
		internal static DataTable GetTable(string command) {
			DataTable retVal=GetTableWithRemotingRoleCheck(command);
			retVal.TableName="";//this is needed for FormQuery dataGrid
			return retVal;
		}

		internal static List<T> GetList<T>(string command,Func<IDataRecord,T> rowToObjMethod) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			return DataCore.GetList(command,rowToObjMethod);
		}

		///<summary>Performs PIn.Long on first column of table returned. Surround with try/catch. Returns empty list if nothing found.</summary>
		internal static List<long> GetListLong(string command,bool hasExceptions=true) {
			List<long> retVal=new List<long>();
			DataTable Table=GetTableWithRemotingRoleCheck(command);
			for(int i=0;i<Table.Rows.Count;i++) {
				retVal.Add(PIn.Long(Table.Rows[i][0].ToString(),hasExceptions));
			}
			return retVal;
		}

		///<summary>Performs PIn.String on first column of table returned. Returns empty list if nothing found.</summary>
		internal static List<string> GetListString(string command) {
			List<string> retVal=new List<string>();
			DataTable Table=GetTableWithRemotingRoleCheck(command);
			for(int i=0;i<Table.Rows.Count;i++) {
				retVal.Add(PIn.String(Table.Rows[i][0].ToString()));
			}
			return retVal;
		}

		///<summary>Gets the table after checking remoting role.</summary>
		private static DataTable GetTableWithRemotingRoleCheck(string command) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				if(RemotingClient.IsReportServer) {
					return Reports.GetTable(command);
				}
				throw new ApplicationException("No longer allowed to send sql directly.  For user sql, use GetTableLow.  Othewise, rewrite the calling "
					+"class to not use this query:\r\n"+command);
			}
			return DataCore.GetTable(command);
		}

		///<summary>This is used for queries written by the user.  If using direct connection, it gets a table in the ordinary way.  If ServerWeb, it uses the user with lower privileges to prevent injection attack.</summary>
		internal static DataTable GetTableLow(string command) {
			LastCommand=command;
			DataTable retVal;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Rewrite the calling class to pass this query off to the server:\r\n"+command);
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientDirect) {
				retVal=DataCore.GetTable(command);
			}
			else {//ServerWeb because ClientWeb was considered prior to this func.
				retVal=DataCore.GetTableLow(command);
			}
			retVal.TableName="";//this is needed for FormQuery dataGrid
			return retVal;
		}

		///<summary>This query is run with full privileges.  This is for commands generated by the main program, and the user will not have access for injection attacks.  Result is usually number of rows changed, or can be insert id if requested.  WILL NOT RETURN CORRECT PRIMARY KEY if the query specifies the primary key.</summary>
		internal static long NonQ(string command,bool getInsertID,string columnNamePK,string tableName,params OdSqlParameter[] parameters) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("No longer allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			return DataCore.NonQ(command,getInsertID,columnNamePK,tableName,parameters);
		}

		///<summary>This query is run with full privileges.  This is for commands generated by the main program, and the user will not have access for injection attacks.  Result is usually number of rows changed, or can be insert id if requested.  WILL NOT RETURN CORRECT PRIMARY KEY if the query specifies the primary key.</summary>
		internal static long NonQ(string command,bool getInsertID,params OdSqlParameter[] parameters) {
			if(getInsertID && DataConnection.DBtype==DatabaseType.Oracle) {
				//The engineer that called this method needs to call the one that accepts a columnNamePK and tableName in order to get the Insert ID back.
				throw new ApplicationException("This overload of Db.NonQ is not Oracle compatible for getting the last insert ID.");
			}
			//MySQL is smart enough to know what the last insert ID was so let it go through without passing in a columnNamePK and tableName.
			return NonQ(command,getInsertID,"","",parameters);
		}

		///<summary>This query is run with full privileges.  This is for commands generated by the main program, and the user will not have access for injection attacks.  Result is usually number of rows changed, or can be insert id if requested.</summary>
		internal static long NonQ(string command,params OdSqlParameter[] parameters) {
			return NonQ(command,false,parameters);
		}

		///<summary>We need to get away from this due to poor support from databases.  For now, each command will be sent entirely separately.  This never returns number of rows affected.</summary>
		internal static long NonQ(string[] commands) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("No longer allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+commands[0]);
			}
			foreach(string command in commands) {
				LastCommand=command;
				DataCore.NonQ(command,false);
			}
			return 0;
		}

		///<summary>This is used only for historical commands in ConvertDatabase.   WILL NOT RETURN CORRECT PRIMARY KEY if the query specifies the primary key.</summary>
		internal static int NonQ32(string command,bool getInsertID) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("No longer allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			return (int)DataCore.NonQ(command,getInsertID);
		}

		///<summary>This is used for historical commands in ConvertDatabase.  Seems to also be used in DBmaint when counting rows affected.</summary>
		internal static int NonQ32(string command) {
			return NonQ32(command,false);
		}

		///<summary>This is used only for historical commands in ConvertDatabase.</summary>
		internal static int NonQ32(string[] commands) {
			return (int)NonQ(commands);
		}

		///<summary>We use this for queries that return a single value that is an int.  If there are no results, it will return 0.</summary>
		internal static int GetInt(string command) {
			return PIn.Int((GetLong(command).ToString()),false);
		}

		///<summary>We use this for queries that return a single value that is a long.  If there are no results, it will return 0.</summary>
		internal static long GetLong(string command) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("No longer allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			DataTable table=DataCore.GetTable(command);
			if(table.Rows.Count==0) {
				return 0;
			}
			return PIn.Long(table.Rows[0][0].ToString());
		}

		///<summary>Use this for count(*) queries.  They are always guaranteed to return one and only one value.  Not any faster, just handier.  Can also be used when retrieving prefs manually, since they will also return exactly one value.</summary>
		internal static string GetCount(string command) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("No longer allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			return DataCore.GetTable(command).Rows[0][0].ToString();
		}

		///<summary>Use this only for queries that return one value.</summary>
		internal static string GetScalar(string command) {
			LastCommand=command;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("No longer allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			return DataCore.GetScalar(command);
		}

		#region old
		///<summary>Always throws exception.</summary>
		public static DataTable GetTableOld(string command) {
			throw new ApplicationException("No queries allowed in the UI layer.");
		}

		///<summary>Always throws exception.</summary>
		public static int NonQOld(string[] commands) {
			throw new ApplicationException("No queries allowed in the UI layer.");
		}

		///<summary>Always throws exception.</summary>
		public static int NonQOld(string command) {
			throw new ApplicationException("No queries allowed in the UI layer.");
		}
		#endregion old


	}
}
