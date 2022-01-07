using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace xCrudGenerator {
	///<summary>This is the class that actually generates snippets of raw schema code.</summary>
	public class CrudSchemaRaw {
		private const string rn="\r\n";
		private const string t1="\t";
		private const string t2="\t\t";
		private const string t3="\t\t\t";
		private const string t4="\t\t\t\t";
		private const string t5="\t\t\t\t\t";
		private static string tb = "";

		///<summary>Generates C# code to add a table.</summary>
		public static string AddTable(string tableName,List<DbSchemaCol> cols,int tabInset,bool isMobile,bool doRunQueries) {
			StringBuilder strb = new StringBuilder();
			StringBuilder sbCommand=new StringBuilder();
			List<DbSchemaCol> indexes = new List<DbSchemaCol>();
			tb="";//must reset tabs each time method is called
			for(int i=0;i<tabInset;i++) {//defines the base tabs to be added to all lines
				tb+="\t";
			}
			#region MySQL
			//strb.Append(tb+"if(DataConnection.DBtype==DatabaseType.MySql) {");
			strb.Append(rn+tb+"command=\"DROP TABLE IF EXISTS "+tableName+"\";");
			strb.Append(rn+tb+"Db.NonQ(command);");
			strb.Append(rn+tb+"command=@\"");
			sbCommand.Append("CREATE TABLE "+tableName+" (");
			for(int i=0;i<cols.Count;i++) {
				sbCommand.Append(rn+tb+t1+cols[i].ColumnName+" "+GetMySqlType(cols[i])+" NOT NULL");
				if(!ListTools.In(GetMySqlBlankData(cols[i]),"\"\"","0")){
					sbCommand.Append(" DEFAULT "+GetMySqlBlankData(cols[i])+(GetMySqlType(cols[i])=="timestamp"?" ON UPDATE CURRENT_TIMESTAMP":""));
				}
				if(i==0 && !isMobile){
					sbCommand.Append(" auto_increment PRIMARY KEY");
				}
				else if(cols[i].DataType==OdDbType.Long) {//All bigints are assumed to be either keys or foreign keys.
					indexes.Add(cols[i]);
				}
				if(i<cols.Count-1) {
					sbCommand.Append(",");
				}
			}
			for(int i=0;i<indexes.Count;i++) {
				sbCommand.Append(",");//There will always be a column defined before this, so we need to add a comma before we add an index.
				sbCommand.Append(rn+tb+t1+"INDEX("+indexes[i].ColumnName+")");
			}
			sbCommand.Append(rn+tb+t1+") DEFAULT CHARSET=utf8");
			if(doRunQueries) {
				DataCore.NonQ(sbCommand.ToString());
			}
			strb.Append(sbCommand+"\";");
			strb.Append(rn+tb+"Db.NonQ(command);");
			#endregion
			return strb.ToString();
		}

		///<summary>Generates C# code to Add Column to table. Returns a string that contains the code to add to the convert script.</summary>
		///<param name="tableName">The name of the table being modified.</param>
		///<param name="col">The column that will be added. Includes information on type and name.</param>
		///<param name="tabInset">The number of tabs that should be added to each of the lines of code.</param>
		///<param name="doRunQuery">If set to true, the generated queries will be ran against the connected database.</param>
		///<param name="tableType">The type of the C# table object.</param>
		public static string AddColumnEnd(string tableName,DbSchemaCol col,int tabInset,bool doRunQuery,Type tableType) {
			StringBuilder strb=new StringBuilder();
			tb=new string('\t',tabInset);//Reset this variable everytime as it is static.
			//Generate the MySQL query and run it if necessary.
			string command="ALTER TABLE "+tableName+" ADD "+col.ColumnName+" ";
			string columnDef=GetMySqlType(col)+" NOT NULL";
			if(!ListTools.In(GetMySqlBlankData(col),"\"\"","0")) {
				columnDef+=" DEFAULT "+GetMySqlBlankData(col)+(GetMySqlType(col)=="timestamp"?" ON UPDATE CURRENT_TIMESTAMP":"");
			}
			command+=columnDef;
			if(doRunQuery) {
				DataCore.NonQ(command);
			}
			string commandAddIndex="";
			if(col.DataType==OdDbType.Long) {//Generate and run index regardless of large table status
				commandAddIndex="ALTER TABLE "+tableName+" ADD INDEX ("+col.ColumnName+")";
				if(doRunQuery) {
					DataCore.NonQ(commandAddIndex);
				}
			}
			//If there is a large table property set to true, we will use the the large table helper in the convert script.
			if(tableType.GetCustomAttributes<CrudTableAttribute>().Any(x => x.IsLargeTable)) {
				//With the AlterLargeTable method, we can add columns and new indexes at the same time.
				string primaryKeyColName=tableType.GetFields().First(x => x.GetCustomAttribute<CrudColumnAttribute>().IsPriKey).Name;
				strb.Append(rn+tb+"LargeTableHelper.AlterLargeTable(\""+tableName+"\",\""+primaryKeyColName+"\","
					+rn+tb+"\tnew List<Tuple<string,string>> { Tuple.Create(\""+col.ColumnName+"\",\""+columnDef+"\") }");
				//Add the parameter to LargeTableHelper to specify a new index. Index name will be the same as the column name.
				if(col.DataType==OdDbType.Long) {
					strb.Append(","+rn+tb+"\tnew List<Tuple<string,string>> { Tuple.Create(\""+col.ColumnName+"\",\"\") }");
				}
				strb.Append(");");
			}
			else {//Use the raw command in the convert script for adding the column and the index
				strb.Append(rn+tb+"command=\""+command+"\";");
				strb.Append(rn+tb+"Db.NonQ(command);");
				if(col.DataType==OdDbType.Long) {//key or foreign key
					strb.Append(rn+tb+"command=\""+commandAddIndex+"\";");
					strb.Append(rn+tb+"Db.NonQ(command);");
				}
			}
			return strb.ToString();
		}

		///<summary></summary>
		public static string AddIndex(string tableName,string colName,int tabInset) {
			StringBuilder strb = new StringBuilder();
			tb="";//must reset tabs each time method is called
			for(int i=0;i<tabInset;i++) {//defines the base tabs to be added to all lines
				tb+="\t";
			}
			//strb.Append(tb+"if(DataConnection.DBtype==DatabaseType.MySql) {");
			strb.Append(rn+tb+"command=\"ALTER TABLE "+tableName+" ADD INDEX("+colName+")\";");
			strb.Append(rn+tb+"Db.NonQ(command);");
			//strb.Append(rn+tb+"}");
			#region Oracle Removed
			//strb.Append(rn+tb+"else {//oracle");
			//string indexName=tableName+"_"+colName;
			//strb.Append(rn+tb+t1+"command=\"CREATE INDEX "+indexName.Substring(0,Math.Min(30,indexName.Length))+" ON "+tableName+" ("+colName+")\";");
			//strb.Append(rn+tb+t1+"Db.NonQ(command);");
			//strb.Append(rn+tb+"}");
			#endregion
			return strb.ToString();
		}

		///<summary>Does not work for Timestamp because of Oracle triggers.</summary>
		public static string DropColumn(string tableName,string colName,int tabInset) {
			StringBuilder strb = new StringBuilder();
			tb="";//must reset tabs each time method is called
			for(int i=0;i<tabInset;i++) {//defines the base tabs to be added to all lines
				tb+="\t";
			}
			//strb.Append(tb+"if(DataConnection.DBtype==DatabaseType.MySql) {");
			strb.Append(rn+tb+"command=\"ALTER TABLE "+tableName+" DROP COLUMN "+colName+"\";");
			strb.Append(rn+tb+"Db.NonQ(command);");
			//strb.Append(rn+tb+"}");
			#region Oracle Removed
			//strb.Append(rn+tb+"else {//oracle");
			//strb.Append(rn+tb+t1+"command=\"ALTER TABLE "+tableName+" DROP COLUMN "+colName+"\";");
			//strb.Append(rn+tb+t1+"Db.NonQ(command);");
			//strb.Append(rn+tb+"}");
			#endregion
			return strb.ToString();
		}

		/// <summary>For example, might return "bigint".</summary>
		private static string GetMySqlType(DbSchemaCol col) {
			switch(col.DataType) {
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
				case OdDbType.Enum:
					return "tinyint";
				case OdDbType.Int:
					if(col.IntUseSmallInt) {
						return "smallint";
					}
					else {
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

		///<summary>For example, might returns "0", "", or "01-01-0001" for cols with types OdDbType.Byte, OdDbType.Text, and OdDbType.DateTime respectively.</summary>
		private static string GetMySqlBlankData(DbSchemaCol col) {
			switch(col.DataType) {
				case OdDbType.Bool:
				case OdDbType.Byte:
				case OdDbType.Currency:
				case OdDbType.Enum:
				case OdDbType.Float:
				case OdDbType.Int:
				case OdDbType.Long:
					return "0";
				case OdDbType.Date:
					return "'0001-01-01'";//sets date to 01 JAN 2001, 00:00:00
				case OdDbType.DateTimeStamp:
					return "CURRENT_TIMESTAMP";
				case OdDbType.DateTime:
					return "'0001-01-01 00:00:00'";
				case OdDbType.TimeOfDay:
				case OdDbType.TimeSpan:
					return "'00:00:00'";
				case OdDbType.Text:
				case OdDbType.VarChar255:
					return "\"\"";//sets to empty string
				default:
					throw new ApplicationException("type not found");
			}
		}

		///<summary>For example, might return "NUMBER(11) NOT NULL".</summary>
		private static string GetOracleType(DbSchemaCol col) {
			switch(col.DataType) {
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
					return "timestamp";
				case OdDbType.Float:
					return "number(38,8)";
				case OdDbType.Enum:
					return "number(3)";
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

		///<summary>For example, might returns "0", "", or "01-JAN-0001" for cols with types OdDbType.Byte, OdDbType.Text, and OdDbType.DateTime respectively.</summary>
		private static string GetOracleBlankData(DbSchemaCol col) {
			switch(col.DataType) {
				case OdDbType.Bool:
				case OdDbType.Byte:
				case OdDbType.Currency:
				case OdDbType.Float:
				case OdDbType.Enum:
				case OdDbType.Int:
				case OdDbType.Long:
					return "0";
				case OdDbType.Date:
				case OdDbType.DateTime:
				case OdDbType.TimeOfDay:
					return "TO_DATE('0001-01-01','YYYY-MM-DD')";
				case OdDbType.DateTimeStamp://timestamp is stored as a date and trigger combination
					return null;
				case OdDbType.Text:
				case OdDbType.TimeSpan:
				case OdDbType.VarChar255:
					return null;//stored as NULL, 
				default:
					throw new ApplicationException("type not found");
			}
		}

		/////<summary>Rebuilds timestamp triggers for Oracle timestamps.</summary>
		//private static string TimeStampTriggerBuilderOracle(string tableName,List<DbSchemaCol> cols,int tabInset) {
		//  StringBuilder strb = new StringBuilder();
		//  tb="";//must reset tabs each time method is called
		//  for(int i=0;i<tabInset;i++) {//defines the base tabs to be added to all lines
		//    tb+="\t";
		//  }
		//  if(DataConnection.DBtype==DatabaseType.Oracle) {
		//    for(int i=0;i<cols.Count;i++) {//check for timestamp columns
		//      if(cols[i].DataType == OdDbType.DateTimeStamp) {
		//        strb.Append(rn+tb+t1+"command=@\"CREATE OR REPLACE TRIGGER "+tableName+"_timestamp");
		//        strb.Append(rn+tb+t1+"           BEFORE UPDATE ON "+tableName);
		//        strb.Append(rn+tb+t1+"           FOR EACH ROW");
		//        strb.Append(rn+tb+t1+"           BEGIN");
		//        for(int j=0;j<cols.Count;j++) {//Each column in the table must be set up to change timestamp when changed
		//          strb.Append(rn+tb+t2+"           IF :OLD."+cols[j].ColumnName+" <> :NEW."+cols[j].ColumnName+" THEN");
		//          strb.Append(rn+tb+t2+"           :NEW."+cols[i].ColumnName+" := SYSDATE;");
		//          strb.Append(rn+tb+t2+"           END IF");
		//        }
		//        strb.Append(rn+tb+t1+"           END "+tableName+"_timestamp;\";");
		//        strb.Append(rn+tb+t1+"Db.NonQ(command);");
		//      }
		//    }
		//  }
		//  return strb.ToString();
		//}


	}
}
