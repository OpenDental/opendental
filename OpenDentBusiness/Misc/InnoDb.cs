using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	public class InnoDb {

		/// <summary>Returns the default storage engine.</summary>
		public static string GetDefaultEngine() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string command="SELECT @@default_storage_engine";
			string defaultengine=Db.GetScalar(command).ToString();
			return defaultengine;
		}

		public static bool IsInnodbAvail() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			try { 
				string command="SELECT @@have_innodb";
				string innoDbOn=Db.GetScalar(command).ToString();
				return innoDbOn=="YES";
			}
			catch(Exception ex) {//MySQL 5.6 and higher
				ex.DoNothing();
				string command="SHOW ENGINES";
				DataTable table=Db.GetTable(command);
				foreach(DataRow row in table.Rows) {
					if(row["Engine"].ToString().ToLower()=="innodb" 
						&& ListTools.In(row["Support"].ToString().ToLower(),"yes","default")) 
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>Returns the number of MyISAM tables and the number of InnoDB tables in the current database.</summary>
		public static string GetEngineCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string command=@"SELECT SUM(CASE WHEN information_schema.tables.engine='MyISAM' THEN 1 ELSE 0 END) AS 'myisam',
				SUM(CASE WHEN information_schema.tables.engine='InnoDB' THEN 1 ELSE 0 END) AS 'innodb'
				FROM information_schema.tables
				WHERE table_schema=(SELECT DATABASE())";
			DataTable results=Db.GetTable(command);
			string retval=Lans.g("FormInnoDb","Number of MyISAM tables: ");
			retval+=Lans.g("FormInnoDb",results.Rows[0]["myisam"].ToString())+"\r\n";
			retval+=Lans.g("FormInnoDb","Number of InnoDB tables: ");
			retval+=Lans.g("FormInnoDb",results.Rows[0]["innodb"].ToString())+"\r\n";
			return retval;
		}

		///<summary>Gets the names of tables in InnoDB format, comma delimited (excluding the 'phone' table).  Returns empty string if none.</summary>
		public static string GetInnodbTableNames() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			//Using COUNT(*) with INFORMATION_SCHEMA is buggy.  It can return "1" even if no results.
			string command="SELECT TABLE_NAME FROM INFORMATION_SCHEMA.tables "
				+"WHERE TABLE_SCHEMA='"+POut.String(DataConnection.GetDatabaseName())+"' "
				+"AND TABLE_NAME!='phone' "//this table is used internally at OD HQ, and is always innodb.
				+"AND ENGINE NOT LIKE 'MyISAM'";
			DataTable table=Db.GetTable(command);
			string tableNames="";
			for(int i=0;i<table.Rows.Count;i++) {
				if(tableNames!="") {
					tableNames+=",";
				}
				tableNames+=PIn.String(table.Rows[i][0].ToString());
			}
			return tableNames;
		}

		///<summary>Returns true if the database has at least one table in InnoDB format.  Default db is DataConnection.GetDatabaseName().</summary>
		public static bool HasInnoDbTables(string dbName="") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),dbName);
			}
			//Using COUNT(*) with INFORMATION_SCHEMA is buggy.  It can return "1" even if no results.
			string command="SELECT TABLE_NAME FROM INFORMATION_SCHEMA.tables "
				+"WHERE TABLE_SCHEMA='"+POut.String(string.IsNullOrEmpty(dbName) ? DataConnection.GetDatabaseName() : dbName)+"' "
				+"AND ENGINE NOT LIKE 'MyISAM'";
			return Db.GetTable(command).Rows.Count>1;
		}

		///<summary>The only allowed parameters are "InnoDB" or "MyISAM".  Converts tables to toEngine type and returns the number of tables converted.</summary>
		public static int ConvertTables(string fromEngine,string toEngine) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),fromEngine,toEngine);
			}
			int numtables=0;
			string command="SELECT DATABASE()";
			string database=Db.GetScalar(command);
			command=@"SELECT table_name
				FROM information_schema.tables
				WHERE table_schema='"+POut.String(database)+"' AND information_schema.tables.engine='"+fromEngine+"'";
			DataTable results=Db.GetTable(command);
			command="";
			if(results.Rows.Count==0) {
				return numtables;
			}
			for(int i=0;i<results.Rows.Count;i++) {
				command+="ALTER TABLE `"+database+"`.`"+results.Rows[i]["table_name"].ToString()+"` ENGINE='"+toEngine+"'; ";
				numtables++;
			}
			Db.NonQ(command);
			return numtables;
		}
	}
}
