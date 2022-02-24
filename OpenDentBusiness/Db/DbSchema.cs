/*/This is outdated, but will remain here for reference for a while.
 // Functionality was moved to the CrudGenerator project.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	public class DbSchema {
		
		/// <summary></summary>
		public static void AddColumnEnd7_7(string tableName,DbSchemaCol col) {
			string command = "";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command = "ALTER TABLE "+tableName+" ADD "+col.ColumnName+" "+GetMySqlType7_7(col);
				Db.NonQ(command);
				if(col.Indexed) {
					command = "ALTER TABLE "+tableName+" ADD INDEX IDX_"+tableName.ToUpper()+col.ColumnName.ToUpper()+" ("+col.ColumnName+")";
					Db.NonQ(command);
				}
				command = "UPDATE "+tableName+" SET "+col.ColumnName+" = "+getBlankMySQLData(col)+" WHERE "+col.ColumnName+" IS NULL";//fills column with 'blank' data
				Db.NonQ(command);
				if(getBlankMySQLData(col)!="''") {//only set column to NOT NULL if it is not a string type column.
					command = "ALTER TABLE "+tableName+" MODIFY "+col.ColumnName+" "+GetMySqlType7_7(col)+" NOT NULL";
					try {
						Db.NonQ(command);
					}
					catch(Exception e) {
						//fail silently. If this fails that means that the column is already set to NOT NULL Which should theoretically never be the case.
					}
				}
			}
			else {//oracle
				command = "ALTER TABLE "+tableName+" ADD "+col.ColumnName+" "+GetOracleType7_7(col);
				Db.NonQ(command);
				command = "UPDATE "+tableName+" SET "+col.ColumnName+" = "+getBlankOracleData(col)+" WHERE "+col.ColumnName+" IS NULL";//fills column with 'blank' data
				Db.NonQ(command);
				if(getBlankOracleData(col)!="''") {//only set column to NOT NULL if it is not a string type column.
					command = "ALTER TABLE "+tableName+" MODIFY "+col.ColumnName+" NOT NULL";
					try {
						Db.NonQ(command);
					}
					catch(Exception e) {
						//fail silently. If this fails that means that the column is already set to NOT NULL Which should theoretically never be the case.
					}
				}
				if(col.Indexed) {
					command=" CREATE INDEX IDX_"+tableName+"_"+col.ColumnName+" ON "+tableName+" ("+col.ColumnName+")";
					Db.NonQ(command);
				}
				OracleValidateDateTStampTriggerHelper7_7(tableName);
			}
		}

		/// <summary>Specify textSize if there's any chance of it being greater than 4000 char.</summary>
		public static void AddColumnAfter7_7(string tableName,DbSchemaCol col,string afterColumn) {
			string command = "";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command = "ALTER TABLE "+tableName+" ADD "+col.ColumnName+" "+GetMySqlType7_7(col)+" AFTER "+afterColumn;
				Db.NonQ(command);
				if(col.Indexed) {
					command = "ALTER TABLE "+tableName+" ADD INDEX IDX_"+tableName.ToUpper()+col.ColumnName.ToUpper()+" ("+col.ColumnName+")";
					Db.NonQ(command);
				}
			}
			else {//oracle
				int addAtIndex=0;
				command ="Select TABLE_NAME, COLUMN_NAME from user_tab_columns where table_name='"+tableName.ToUpper()+"'";
				DataTable tempTable = Db.GetTable(command);//get list of columns
				for(int i=0;i<tempTable.Rows.Count;i++) {//find column index of column that matches afterColumn
					if(tempTable.Rows[i][1].ToString()==afterColumn) {
						addAtIndex = i+1;
					}
				}
				if(addAtIndex!=0) {//only add after if the column was found
					OracleAddAtIndexHelper7_7(tableName,col,addAtIndex);
				}
				OracleValidateDateTStampTriggerHelper7_7(tableName);
			}
		}

		/*applied to just this method
		/// <summary>Specify textSize if there's any chance of it being greater than 4000 char.</summary>
		public static void AddColumnFirst#_#(string tableName,DbSchemaCol col) {
			string command = "";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command = "ALTER TABLE "+tableName+" DROP PRIMARY KEY, ADD "+col.ColumnName+" "+GetMySqlType(col)+" PRIMARY KEY FIRST;";
				Db.NonQ(command);
			}
			else {//oracle
				OracleAddAtIndexHelper(tableName,col,0);
				OracleValidateDateTStampTriggerHelper(tableName);
			}
		}  /

		/// <summary>TODO: trigger cleanup for oracle</summary>
		public static void DropColumn7_7(string tableName,string columnName) {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command= "ALTER TABLE "+tableName+" DROP COLUMN "+columnName+" ;";
				Db.NonQ(command);
			}
			else {//oracle
				command= "ALTER TABLE "+tableName+" DROP COLUMN "+columnName+" ;";
				Db.NonQ(command);
				OracleValidateDateTStampTriggerHelper7_7(tableName);
//todo: check for existing trigger or index other than DateTStamp
			}
		}

		/// <summary>First column is always a bigint, primary key, autoincrement.</summary>
		public static void AddTable7_7(string tableName,List<DbSchemaCol> cols) {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS "+tableName;
				Db.NonQ(command);
				command = "CREATE TABLE "+tableName+" (";
				for(int i=0;i<cols.Count;i++) {//create table
					command+=cols[i].ColumnName+" "+GetMySqlType7_7(cols[i])+(i==0?" PRIMARY KEY":"")+(i==cols.Count-1?") DEFAULT CHARSET=utf8 ;":", ");
				}
				Db.NonQ(command);
				for(int i=0;i<cols.Count;i++) {//create indexes
					if(cols[i].Indexed) {
						command = "ALTER TABLE "+tableName+" ADD INDEX IDX_"+tableName.ToUpper()+cols[i].ColumnName.ToUpper()+" ("+cols[i].ColumnName+")";
						Db.NonQ(command);
					}
				}
			}
			else {//oracle
				bool tableExists=false;
				DataTable tempUsernameTable = Db.GetTable("SELECT username FROM user_users;");//cannot exlicitly use tempUsernameTable.Rows["username"] because it cannot convert from string to int.
				command="SELECT * FROM user_tables WHERE user='"+tempUsernameTable.Rows[0][0].ToString()+"'";//check to see if table exists
				DataTable tempTableNames = Db.GetTable(command);
				for(int i=0;i<tempTableNames.Rows.Count;i++) {
					if(tempTableNames.Rows[i].ToString().Equals(tableName)) {
						tableExists=true;
					}
				}
				if(tableExists) {
					command="DROP TABLE "+tableName;
					Db.NonQ(command);
					//TODO: drop indexes and triggers?
				}
				//table doesn't exist and therefor needs to be created
				command = "CREATE TABLE "+tableName+" (";
				for(int i=0;i<cols.Count;i++) {
					command+=cols[i].ColumnName+" "+GetOracleType7_7(cols[i])+(i==0?" primary key ":"")+(i==cols.Count-1?")":", ");
				}
				Db.NonQ(command);
				OracleValidateDateTStampTriggerHelper7_7(tableName);
			}
		}

		/// <summary>TODO: trigger cleanup for oracle</summary>
		public static void DropTable7_7(string tableName) {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command= "DROP TABLE IF EXISTS "+tableName;
				Db.NonQ(command);
			}
			else {//oracle
				bool tableExists=false;
				DataTable tempUsernameTable = Db.GetTable("SELECT username FROM user_users;");//cannot exlicitly use tempUsernameTable.Rows["username"] because it cannot convert from string to int.
				command="SELECT * FROM user_tables WHERE user='"+tempUsernameTable.Rows[0][0].ToString()+"'";//check to see if table exists
				DataTable tempTableNames = Db.GetTable(command);
				for(int i=0;i<tempTableNames.Rows.Count;i++) {
					if(tempTableNames.Rows[i].ToString().Equals(tableName)) {
						tableExists=true;
					}
				}
				if(tableExists) {
					command="DROP TABLE "+tableName;
					Db.NonQ(command);
					//TODO: check for existing trigger or index other than DateTStamp
				}
			}
		}

		/// <summary>TODO.this.oracle</summary>
		public static void RenameColumn7_7(string tableName,string columnNameOld,DbSchemaCol colNew) {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command = "ALTER TABLE "+tableName+" CHANGE "+columnNameOld+" "+colNew.ColumnName+" "+GetMySqlType7_7(colNew);
			}
			else {//oracle

			}
		}

		/// <summary>TODO.this.Oracle</summary>
		public static void ChangeColumnType7_7(string tableName,DbSchemaCol colNew){//  string columnName,OdDbType newType) {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command = "ALTER TABLE "+tableName+" MODIFY "+colNew.ColumnName+" "+GetMySqlType7_7(colNew)+";";
			}
			else {//oracle

			}
		}

		/// <summary>TODO.this</summary>
		public static void AddKey7_7(string tableName,string columnName) {
			if(DataConnection.DBtype==DatabaseType.MySql) {
				//surround with try catch to fail silently.
			}
			else {//oracle

			}
		}

		/// <summary>TODO.this</summary>
		public static void RemoveKey7_7(string tableName,string columnName) {
			if(DataConnection.DBtype==DatabaseType.MySql) {

			}
			else {//oracle

			}
		}

		/// <summary>For example, might return "bigint NOT NULL".</summary>
		private static string GetMySqlType7_7(DbSchemaCol col) {
			switch(col.DataType) {
				case OdDbType.Blob:
					return "mediumblob";
				case OdDbType.Bool:
					return "tinyint";
				case OdDbType.Byte:
					return "tinyint unsigned";
				case OdDbType.Currency:
					return "double";
				case OdDbType.Date:
					return "date";
				case OdDbType.DateTime:
					return "datetime";
				case OdDbType.DateTimeStamp:
					return "timestamp";
				case OdDbType.Float:
					return "float";
				case OdDbType.Int:
					if(col.IntUseSmallInt){
						return "smallint";
					}
					else{
						return "int";
					}
				case OdDbType.Long:
					return "bigint";
				case OdDbType.Text:
					if(col.TextSize==TextSizeMySqlOracle.Small || col.TextSize==TextSizeMySqlOracle.Medium) {
						return "text";
					}
					else {//textSize==TextSizeMySqlOracle.large
						return "mediumtext";
					}
				case OdDbType.TimeOfDay:
					return "time";
				case OdDbType.TimeSpan:
					return "time";
				case OdDbType.VarChar255:
					return "varchar(255)";
				default:
					throw new ApplicationException("type not found");
			}
		}

		///<summary>For example, might return "NUMBER(11) NOT NULL".</summary>
		private static string GetOracleType7_7(DbSchemaCol col) {
			switch(col.DataType) {
				case OdDbType.Blob:
					return "blob";
				case OdDbType.Bool:
					return "number(3)";
				case OdDbType.Byte:
					return "number(3)";
				case OdDbType.Currency:
					return "number(38,8)";
				case OdDbType.Date:
					return "date";
				case OdDbType.DateTime:
					return "date";
				case OdDbType.DateTimeStamp:
					//also requires trigger, trigger code is automatically created above.
					return "date";
				case OdDbType.Float:
					return "number(38,8)";
				case OdDbType.Int:
					return "number(11)";
				case OdDbType.Long:
					return "number(20)";
				case OdDbType.Text:
					if(col.TextSize==TextSizeMySqlOracle.Small) {
						return "varchar2(4000)";
					}
					else {//textSize == medium or large
						return "clob";
					}
				case OdDbType.TimeOfDay:
					return "date";
				case OdDbType.TimeSpan:
					return "varchar2(255)";
				case OdDbType.VarChar255:
					return "varchar2(255)";
				default:
					throw new ApplicationException("type not found");
			}
		}

		/// <summary>Used to get the value that should be used instead of null. For example will return '' for a string column or 0 for an int column.</summary>
		private static string getBlankOracleData(DbSchemaCol col) {
			switch(col.DataType) {
				case OdDbType.Bool:
				case OdDbType.Byte:
				case OdDbType.Float:
				case OdDbType.Int:
				case OdDbType.Long:
				case OdDbType.Currency:
					return "0";
				case OdDbType.Date:
				case OdDbType.DateTime:
				case OdDbType.TimeOfDay:
				case OdDbType.DateTimeStamp:
					return "'01-JAN-0001'";
				case OdDbType.Text:
				case OdDbType.VarChar255:
					return "''";
				case OdDbType.TimeSpan:
					return "'00:00:00'";
			}
			return "";
		}

		/// <summary>Used to get the value that should be used instead of null. For example will return '' for a string column or 0 for an int column.</summary>
		private static string getBlankMySQLData(DbSchemaCol col) {
			switch(col.DataType) {
				case OdDbType.Bool:
				case OdDbType.Byte:
				case OdDbType.Float:
				case OdDbType.Int:
				case OdDbType.Long:
				case OdDbType.Currency:
					return "0";
				case OdDbType.Date:
				case OdDbType.DateTime:
				case OdDbType.TimeOfDay:
				case OdDbType.DateTimeStamp:
					return "'0001-01-01'";
				case OdDbType.Text:
				case OdDbType.VarChar255:
					return "''";
				case OdDbType.TimeSpan:
					return "'00:00:00'";
			}
			return "";
		}

		/// <summary>This creates a DbSchemaCol from an oracle datatype passed in as a string. Returns null if no matching type is found.
		/// DO NOT use to convert from oracleCol>>odDBCol>>MySQLCol, datatypes WILL be changed if used that way.</summary>
		private static DbSchemaCol getDbSchemaColFromOracleDataType(string colName,string oracleType) {
			DbSchemaCol newCol = new DbSchemaCol(colName,OdDbType.Bool);
			switch(oracleType.ToUpper()){
				case "NUMBER(3)":
					newCol.DataType=OdDbType.Byte;
					break;
				case "NUMBER(38,8)":
					newCol.DataType=OdDbType.Float;
					break;
				case "DATE":
					newCol.DataType=OdDbType.Date;
					break;
				case "NUMBER(11)":
					newCol.DataType=OdDbType.Int;
					break;
				case "NUMBER(20)":
					newCol.DataType=OdDbType.Long;
					break;
				case "VARCHAR2(4000)":
					newCol.DataType=OdDbType.Text;
					newCol.TextSize=TextSizeMySqlOracle.Small;
					break;
				case "CLOB":
					newCol.DataType=OdDbType.Text;
					newCol.TextSize=TextSizeMySqlOracle.Medium;
					break;
				case "VARCHAR2(255)":
					newCol.DataType=OdDbType.VarChar255;
					break;
				default://no matching datatype was found
					return null;
			}
			return newCol;

		}

		/// <summary>Validates any table's dateTStamp triggers for Oracle.</summary>
		private static void OracleValidateDateTStampTriggerHelper7_7(string tableName) {
			bool triggerNeeded = false;
			bool needDropTrigger = false;
			string command ="Select TABLE_NAME, COLUMN_NAME from user_tab_columns where table_name='"+tableName.ToUpper()+"'";
			DataTable tempTable = Db.GetTable(command);//get list of columns
			for(int i=0;i<tempTable.Rows.Count;i++){//check for a column named "DateTStamp"
				if(tempTable.Rows[i]["COLUMN_NAME"].ToString()=="DateTStamp") {
					triggerNeeded=true;
				}
			}
			if(triggerNeeded) {//table needs a timestamp trigger, regardless of existing triggers
				command = "CREATE OR REPLACE TRIGGER "+tableName+"_timestamp BEFORE UPDATE ON "+tableName+" FOR EACH ROW "
										+"BEGIN";
				for(int i=0;i<tempTable.Rows.Count;i++) {//iterate through each column to see if it was changed
					command+="	IF :OLD."+tempTable.Rows[i]["COLUMN_NAME"].ToString()+" <> :NEW."+tempTable.Rows[i]["COLUMN_NAME"].ToString()+" THEN"
										+"	:NEW.DateTStamp := SYSDATE;"
										+"	END IF;";
				}
				command+="END "+tableName+"_timestamp;";
			}
			else {//table needs to have zero DateTStamp triggers
				command = "Select TRIGGER_NAME FROM user_triggers WHERE table_name = '"+tableName.ToUpper()+"'";
				DataTable tempTriggerTable = Db.GetTable(command);
				for(int i=0;i<tempTriggerTable.Rows.Count;i++) {//check for timestamp triggers before trying to delete
					if(tempTriggerTable.Rows[i]["TRIGGER_NAME"].ToString().Contains("_timestamp")) {
						needDropTrigger = true;
					}
				}
				if(needDropTrigger) {//Delete timestamp triggers if they exist
					command = "DROP TRIGGER "+tableName.ToUpper()+"_timestamp WHERE TABLE_NAME = '"+tableName.ToUpper()+"'";
					Db.NonQ(command);
				}
			}
		}

		/// <summary>Fills new table by selecting each column before index from old table, creates new column at index, continues to select columns from old table, drops old table, renames new table.  Does not support adding as first column.  Does not support adding a primary key.  Assumes that primary key is always first column.  Will fail on "preference" table.</summary>
		private static void OracleAddAtIndexHelper7_7(string tableName,DbSchemaCol col, int indexOfNewColumn) {
			string command;
			string commandPart2;
			DbSchemaCol newCol;
			command = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM user_tab_columns WHERE table_name='"+tableName.ToUpper()+"'";
			DataTable tempTable = Db.GetTable(command);//get list of columns
			List<DbSchemaCol> newTableCols = new List<DbSchemaCol>();
			for(int i=0;i<tempTable.Rows.Count;i++) {
				if(i==indexOfNewColumn){//once we reach the index of the new column add it to the table
					newCol = new DbSchemaCol(col.ColumnName,col.DataType);//add new column here
					newTableCols.Add(newCol);
				}
				newCol = new DbSchemaCol(getDbSchemaColFromOracleDataType(tempTable.Rows[i]["COLUMN_NAME"].ToString(),tempTable.Rows[i]["DATA_TYPE"].ToString()));
				newTableCols.Add(newCol);
			}
			AddColumnEnd7_7(tableName,col);
			command="CREATE TABLE newtemptable ( ";
			commandPart2=" AS (SELECT ";
			for(int i=0;i<newTableCols.Count;i++) {
				//TODO:if someone try's to add a column to the beginning it will be set as the primary key, and requires information to be input. Also requires unique entries.
				command+=newTableCols[i].ColumnName+" "+GetOracleType7_7(newTableCols[i])+(i==0?" primary key ":"")+(i==newTableCols.Count-1?", ":" )");
				commandPart2+=tableName+"."+newTableCols[i].ColumnName+(i==newTableCols.Count-1?", ":" FROM "+tableName+") DEFAULT CHARSET=utf8;");
			}
			command+=commandPart2;
			Db.NonQ(command);
			command="DROP TABLE "+tableName;
			Db.NonQ(command);
			command = "ALTER TABLE newtemptable RENAME TO "+tableName;
			Db.NonQ(command);
			for(int i=0;i<newTableCols.Count;i++) {//creates triggers
				if(newTableCols[i].Indexed) {
					command=" CREATE INDEX IDX_"+tableName+"_"+col.ColumnName+" ON "+tableName+" ("+col.ColumnName+")";
					Db.NonQ(command);
				}
			}
		}



	}
}
*/