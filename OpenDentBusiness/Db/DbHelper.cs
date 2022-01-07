using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	///<summary>This class contains methods used to generate database independent SQL.</summary>
	public class DbHelper {

		///<summary>Helper method that is only useful for Oracle.  This method is really just here for exposure for the lack of Oracle functionality.
		///Oracle will cut up a section of the CLOB column using SUBSTR.  The portion is dictated by starting at startIndex for substringLength chars.
		///When using MySQL you simply order by the column name because it is smart enough to allow users to ORDER BY 'text' data type.</summary>
		public static string ClobOrderBy(string columnName,int startIndex=1,int substringLength=1000) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "DBMS_LOB.SUBSTR("+columnName+","+POut.Int(substringLength)+","+POut.Int(startIndex)+")";
			}
			return columnName;
		}

		///<summary>Returns a safe drop table string that does not need to be surrounded with a try catch.</summary>
		public static string DropTableIfExist(string tableName) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "BEGIN EXECUTE IMMEDIATE 'DROP TABLE "+tableName+"'; EXCEPTION WHEN OTHERS THEN NULL; END;";
			}
			else {
				return "DROP TABLE IF EXISTS "+tableName;
			}
		}

		///<summary>Use when you already have a WHERE clause in the query. Uses AND RowNum... for Oracle.</summary>
		public static string LimitAnd(int n) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "AND RowNum <= " + n;
			}
			else {
				return "LIMIT " + n;
			}
		}

		///<summary>Use when you do not otherwise have a WHERE clause in the query. Uses WHERE RowNum... for Oracle.</summary>
		public static string LimitWhere(int n) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "WHERE RowNum <= " + n;
			}
			else {
				return "LIMIT " + n;
			}
		}

		///<summary>Use when there is an ORDER BY clause in the query. Uses RowNum... for Oracle.</summary>
		public static string LimitOrderBy(string query,int n) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "SELECT * FROM (" + query + ") WHERE RowNum <= " + n;
			}
			else {
				return query + " LIMIT " + n;
			}
		}

		///<summary>Use when there is an ORDER BY clause in the query.  Uses RowNum... for Oracle.  Returns n rows after skipping offset number of rows.</summary>
		public static string LimitOrderByOffset(string query,int n,int offset) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "SELECT * FROM ("+query+") WHERE RowNum > "+offset+" AND RowNum <= "+(offset+n);
			}
			else {
				return query + " LIMIT "+offset+","+n;
			}
		}

		/// <summary>Concatenates the fields and/or literals passed as params for Oracle or MySQL. If passing in a literal, surround with single quotes.</summary>
		public static string Concat(params string[] values) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				string result="(";
				for(int i=0;i<values.Length;i++) {
					if(i!=0) {
						result+=" || ";
					}
					result+=values[i];
				}
				result+=")";
				return result;
			}
			else {
				string result="CONCAT(";
				for(int i=0;i<values.Length; i++) {
					if(i!=0) {
						result+=",";
					}
					result+=values[i];
				}
				result+=")";
				return result;
			}
		}

		///<summary>Specify column for equivalent of "GROUP_CONCAT(column)" in MySQL. Adds DISTINCT (MySQL only) and ORDERBY and SEPARATOR as specified.
		///SEPARATOR not used for Oracle.
		///Call using parameters by name, example: GroupConcat(column,distinct:true,separator:" | ").</summary>
		public static string GroupConcat(string column,bool distinct=false,bool orderby=false,string separator=",") {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				if(orderby) {
					return "RTRIM(REPLACE(REPLACE(XMLAgg(XMLElement(\"x\","+column+") ORDER BY "+column+"),'<x>'),'</x>',','),',')";
				}
				else {
					return "RTRIM(REPLACE(REPLACE(XMLAgg(XMLElement(\"x\","+column+")),'<x>'),'</x>',','),',')";
				}//Distinct ignored for Oracle case.
			}
			else {
				if(distinct && orderby) {
					return "GROUP_CONCAT(DISTINCT "+column+" ORDER BY "+column+" SEPARATOR '"+separator+"')";
				}
				if(distinct &&  !orderby) {
					return "GROUP_CONCAT(DISTINCT "+column+" SEPARATOR '"+separator+"')";
				}
				if(!distinct && orderby) {
					return "GROUP_CONCAT("+column+" ORDER BY "+column+" SEPARATOR '"+separator+"')";
				}
				else {
					return "GROUP_CONCAT("+column+" SEPARATOR '"+separator+"')";
				}
			}
		}

		///<summary>If the MySQL variable eq_range_index_dive_limit exists (MySQL 5.6 and above) and is set to a value>0, this will break a query with
		///eq_range_index_dive_limit or more items in an IN clause into multiple statements separated by either a UNION ALL for SELECT statements or a
		///semicolon for UPDATE/DELETE statements.  Prior to splitting up the list of items in the argsLists, a DISTINCT is applied to remove duplicates.
		///The query string should contain a "{n}" value in the "IN ()" clause, where n is the list order in the argsLists parameter that will replace the
		///"{n}" value in the string.  The isSelect parameter determines whether to separate by UNION ALL or a semicolon.
		///<para>CAUTION, this method could cause unexpected results due to the potential of breaking up the query into mulitple queries.
		///E.g. A query uses a DISTINCT or a GROUP BY on a separate column than the column having the IN applied to.  Or if multiple IN clauses are
		///present and a row is a match due to more than one clause, and the update is incremental in nature, the row could be updated more than once.</para>
		///<para>Example use: DbHelper.WhereIn("UPDATE procedurelog SET StatementNum=0 WHERE StatementNum IN ({0})",false,listStatementNums.Select(x => POut.Long(x)).ToList());</para></summary>
		public static string WhereIn(string query,bool isSelect=true,params List<string>[] argLists) {
			//The SHOW command is used because it was able to run with a user that had no permissions whatsoever.
			string command="SHOW GLOBAL VARIABLES WHERE Variable_name='eq_range_index_dive_limit'";
			DataTable table=Db.GetTable(command);
			int maxInValCount=0;
			if(table.Rows.Count>0) {
				maxInValCount=PIn.Int(table.Rows[0]["Value"].ToString())-1;
			}
			List<string> listQueries=new List<string>() { query };
			for(int i=0;i<argLists.Length;i++) {
				List<string> listColVals=argLists[i].Distinct().ToList();
				//For instances where maxInValCount could not be found (missing in MySQL 5.5) or was found and explicitly set to 0, always index dive.
				if(maxInValCount<=0 || maxInValCount>=listColVals.Count) {
					for(int k=0;k<listQueries.Count;k++) {
						listQueries[k]=listQueries[k].Replace("{"+i+"}",string.Join(",",listColVals));
					}
					continue;
				}
				List<string> listQs=new List<string>();
				for(int j=0;j<listColVals.Count;j+=maxInValCount) {
					List<string> listColValsCur=listColVals.GetRange(j,Math.Min(maxInValCount,listColVals.Count-j));
					for(int k=0;k<listQueries.Count;k++) {
						listQs.Add(listQueries[k].Replace("{"+i+"}",string.Join(",",listColValsCur)));
					}
				}
				listQueries=listQs;
			}
			string separator=(isSelect?" UNION ALL ":";");
			return string.Join(separator,listQueries);
		}

		///<summary>In Oracle, union order by combos can only use ordinals and not column names. Values for ordinal start at 1.</summary>
		public static string UnionOrderBy(string colName,int ordinal) {
			//Using POut doesn't name sense for column names or ordinal numbers because they are not values they are part of the query.
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return ordinal.ToString();
			}
			return colName;
		}

		///<summary>Helper for getting the correct "use index" syntax that will force a query to use the index passed in.
		///tableName is required for Oracle and it CANNOT reference the schema name E.g. "customers.patient" fails, just pass in "patient".</summary>
		public static string UseIndex(string indexName,string tableName) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				//For Oracle, they have this chaos calld "index hint" and the syntax looks crazy.
				//E.g. "  SELECT /*+ INDEX(table,index) */ col1,col2,col3 FROM table  "
				return "/*+ INDEX("+tableName+","+indexName+") */";
			}
			return "USE INDEX("+indexName+")";
		}

		public static string DateAddDay(string date,string days) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return date+" +"+days;//Can handle negatives even with '+' hardcoded.
			}
			return "ADDDATE("+date+","+days+")";
		}

		public static string DateAddMonth(string date,string months) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "ADD_MONTHS("+date+","+months+")";
			}
			return "ADDDATE("+date+",INTERVAL "+months+" MONTH)";
		}

		public static string DateAddYear(string date,string years) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "ADD_MONTHS("+date+","+years+"*12)";
			}
			return "ADDDATE("+date+",INTERVAL "+years+" YEAR)";
		}

		public static string DateAddMinute(string date,string minutes) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return date+" +"+minutes+"/1440";//1 minute is 1/1440 of a day
			}
			return "ADDDATE("+date+",INTERVAL "+minutes+" MINUTE)";
		}

		public static string DateAddSecond(string date,string seconds) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return date+" +"+seconds+"/86400";//1 second is 1/86400 of a day
			}
			return "ADDDATE("+date+",INTERVAL "+seconds+" SECOND)";
		}

		///<summary>Adds the time span to the date passed in.</summary>
		public static string DateAddTimeSpan(string date,TimeSpan timeSpan) {
			return DateAddSecond(date,((int)timeSpan.TotalSeconds).ToString());
		}

		///<summary>Use the overload taking three arguments in order to take advantage of indexes on the column.
		///TO_DATE() for datetime columns where we only want the date.</summary>
		public static string DtimeToDate(string colName) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "TO_DATE("+colName+")";
			}
			return "DATE("+colName+")";
		}

		///<summary>Returns a comparison clause that is capable of using the index on colName.</summary>
		public static string DateTConditionColumn(string colName,ConditionOperator compareType,DateTime dateTime) {
			//Oracle compatible.
			DateTime endDate=dateTime;//dateTime can be DateTime.Max. In those cases we do not want to add any additional time.
			switch(compareType) {
				case ConditionOperator.Equals:
					if(dateTime!=DateTime.MaxValue) {
						endDate=endDate.Date.AddDays(1).AddSeconds(-1);
					}
					return colName+" BETWEEN "+POut.DateT(dateTime.Date)+" AND "+POut.DateT(endDate);
				case ConditionOperator.NotEquals:
					if(dateTime!=DateTime.MaxValue) {
						endDate=endDate.Date.AddDays(1).AddSeconds(-1);
					}
					return colName+" NOT BETWEEN "+POut.DateT(dateTime.Date)+" AND "+POut.DateT(endDate);
				case ConditionOperator.GreaterThan:
					if(dateTime!=DateTime.MaxValue) {
						endDate=endDate.Date.AddDays(1);
					}
					return colName+">="+POut.DateT(endDate);
				case ConditionOperator.LessThan:
					return colName+" < "+POut.DateT(dateTime.Date);
				case ConditionOperator.GreaterThanOrEqual:
					return colName+">="+POut.DateT(dateTime.Date);
				case ConditionOperator.LessThanOrEqual:
					if(dateTime!=DateTime.MaxValue) {
						endDate=endDate.Date.AddDays(1).AddSeconds(-1);
					}
					return colName+"<="+POut.DateT(endDate);
				default:
					throw new NotImplementedException(compareType+" not implemented yet.");
			}
		}

		///<summary>Returns a few BETWEEN clauses that does a mathematical comparison instead of a string comparison. This is much faster than doing 
		///a string comparison such as PatNum LIKE '1234%'.</summary>
		public static string LongBetween(string colName,string val) {
			long valLong=0;
			StringBuilder retVal=new StringBuilder();
			long.TryParse(val,out valLong);
			if(valLong>0) {
				retVal.Append("AND ("+POut.String(colName)+"="+POut.Long(valLong)+" ");
				//Add all the potential ranges for this number between X0 and X9 where X is whatever the user typed in.
				//Never start i (padding) at the length of val, always add one even if that creates an invalid long (try parse should catch that).
				for(int i = val.Length+1;i<=long.MaxValue.ToString().Length;i++) {
					//Example, if user types 1234 this will add "OR patNum BETWEEN 12340000 AND 12349999"
					string startVal=val.PadRight(i,'0');
					string endVal=val.PadRight(i,'9');
					long endValParsed=0;
					if(!long.TryParse(endVal,out endValParsed)) {
						break;//This number falls outside the range of a long so break out.
					}
					retVal.Append(string.Format("OR "+POut.String(colName)+" BETWEEN {0} AND {1} ",startVal,endVal));
				}
				retVal.Append(")");
			}
			else if(val.Length>0) {
				retVal.Append("AND FALSE ");//impossible to match a val that did not parse into a long.
			}
			return retVal.ToString();
		}

		///<summary>Returns a BETWEEN clause that is capable of using the index on colName. 
		///Note that the time portions of the dates are ignored.</summary>
		public static string BetweenDates(string colName,DateTime dateTimeFrom,DateTime dateTimeTo) {
			return DateTConditionColumn(colName,ConditionOperator.GreaterThanOrEqual,dateTimeFrom)+" AND "
				+DateTConditionColumn(colName,ConditionOperator.LessThanOrEqual,dateTimeTo);
		}

		///<summary>The format must be the MySQL format. The following formats are currently acceptable as input: %c/%d/%Y , %m/%d/%Y</summary>
		public static string DateFormatColumn(string colName,string format) {
			//MySQL DATE_FORMAT() reference: http://dev.mysql.com/doc/refman/5.0/en/date-and-time-functions.html#function_date-format
			//Oracle TO_CHAR() reference: http://download.oracle.com/docs/cd/B19306_01/server.102/b14200/sql_elements004.htm#i34510
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				if(format=="%c/%d/%Y") {
					return "TO_CHAR("+colName+",'MM/DD/YYYY')";//Sadly, not exactly the same but closest option.
				}
				else if(format=="%m/%d/%Y") {
					return "TO_CHAR("+colName+",'MM/DD/YYYY')";//Sadly, not exactly the same but closest option.
				}
				throw new Exception("Unrecognized date format string.");
			}
			//MySQL-----------------------------------------------------------------------------
			if(System.Globalization.CultureInfo.CurrentCulture.Name.EndsWith("US")) {
				return "DATE_FORMAT("+colName+",'"+format+"')";
			}
			//foreign, assume d/m/y
			if(format=="%c/%d/%Y") {
				return "DATE_FORMAT("+colName+",'%d/%c/%Y')";
			}
			else if(format=="%m/%d/%Y") {
				return "DATE_FORMAT("+colName+",'%d/%m/%Y')";
			}
			throw new Exception("Unrecognized date format string.");
		}

		///<summary>The format must be the MySQL format.  The following formats are currently acceptable as input: %c/%d/%Y %H:%i:%s and %m/%d/%Y %H:%i:%s.</summary>
		public static string DateTFormatColumn(string colName,string format) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				if(format=="%c/%d/%Y %H:%i:%s") {
					return "TO_CHAR("+colName+",'MM/DD/YYYY %HH24:%MI:%SS')";//Sadly, not exactly the same but closest option.
				}
				else if(format=="%m/%d/%Y %H:%i:%s") {
					return "TO_CHAR("+colName+",'MM/DD/YYYY %HH24:%MI:%SS')";//Sadly, not exactly the same but closest option.
				}
				throw new Exception("Unrecognized datetime format string.");
			}
			//MySQL-----------------------------------------------------------------------------
			if(System.Globalization.CultureInfo.CurrentCulture.Name.EndsWith("US")) {
				return "DATE_FORMAT("+colName+",'"+format+"')";
			}
			//foreign, assume d/m/y
			if(format=="%c/%d/%Y %H:%i:%s") {
				return "DATE_FORMAT("+colName+",'%d/%c/%Y %H:%i:%s')";
			}
			else if(format=="%m/%d/%Y %H:%i:%s") {
				return "DATE_FORMAT("+colName+",'%d/%m/%Y %H:%i:%s')";
			}
			throw new Exception("Unrecognized datetime format string.");
		}

		/* Not used
		///<summary>Helper for Oracle that will return equivalent of MySql CURTIME().</summary>
		public static string Curtime() {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "SYSDATE";
			}
			return "CURTIME()";
		}*/

		///<summary>Helper for Oracle that will return equivalent of MySql CURDATE()</summary>
		public static string Curdate() {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				//return "(SELECT TO_CHAR(SYSDATE,'YYYY-MM-DD') FROM DUAL)";
				return "SYSDATE";
			}
			return "CURDATE()";
		}

		///<summary>NOW()</summary>
		public static string Now() {
			if(DateTime_.IsNowModified) {
				//This will only be used for unit testing.
				return POut.DateT(DateTime_.Now);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				//return "(SELECT TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') FROM DUAL)";
				return "SYSDATE";
			}
			return "NOW()";
		}

		///<summary>Helper for Oracle that will return equivalent of MySql YEAR()</summary>
		public static string Year(string date) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "CAST(TO_CHAR("+date+",'YYYY') AS NUMBER)";
			}
			return "YEAR("+date+")";
		}
		
		///<summary>Helper for Oracle that will return equivalent of MySql "input REGEXP 'pattern'". Also changes pattern:[0-9] to [:digit:] for Oracle.</summary>
		public static string Regexp(string input,string pattern) {
			return Regexp(input,pattern,true);
		}

		///<summary>Helper for Oracle that will return equivalent of MySql "input REGEXP 'pattern'". Also changes pattern:[0-9] to [:digit:] for Oracle. Takes matches param for "does [not] match this regexp."</summary>
		public static string Regexp(string input,string pattern,bool matches) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				pattern.Replace("[0-9]","[[:digit:]]");
				return (matches?"":"NOT ")+"REGEXP_LIKE("+input+",'"+pattern+"')";
			}
			return input+(matches?"":" NOT")+" REGEXP '"+pattern+"'";
		}

		///<summary>Gets the database specific character used for parameters.  For example, : or @.</summary>
		public static string ParamChar {
			get {
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					return ":";
				}
				return "@";
			}
		}

		///<summary>Gets the maximum value for the specified field within the specified table. This key will always be the MAX(field)+1 and will usually be the correct key to use for new inserts, but not always.</summary>
		public static long GetNextOracleKey(string tablename,string field) {
			//When inserting a new record with the key value returned by this function, these are some possible errors that can occur. 
			//The actual error text starts after the ... on each line. Note especially the duplicate key exception, as this exception 
			//must be considered by the insertion algorithm:
			//DUPLICATE PRIMARY KEY....ORA-00001: unique constraint (DEV77.PRIMARY_87) violated
			//MISSING WHOLE TABLE......ORA-00942: table or view does not exist
			//MISSING TABLE COLUMN.....ORA-00904: "ITEMORDER": invalid identifier
			//MISSING OPENING PAREND...ORA-00926: missing VALUES keyword
			//CONNECTION LOST..........ORA-03113: end-of-file on communication channel
			string command="SELECT MAX("+field+")+1 FROM "+tablename;
			long retval=PIn.Long(Db.GetCount(command));
			if(retval==0) {//Happens when the table has no records
				return 1;
			}
			return retval;
		}

		public static string CastToChar(string expr) {
			string result="CAST(";
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				result+=expr+" AS VARCHAR2(4000))";
			}
			else {
				result+=expr+" AS CHAR(4000))";
			}
			return result;
		}

		///<summary>Returns true if the input string is a reserved word in MySQL 5.6.25.</summary>
		public static bool isMySQLReservedWord(string input) {
			bool retval;
			//using a switch statement makes this method run in constant time (faster).
			switch(input.ToUpper()) {
				case "ACCESSIBLE":
				case "ADD":
				case "ALL":
				case "ALTER":
				case "ANALYZE":
				case "AND":
				case "AS":
				case "ASC":
				case "ASENSITIVE":
				case "BEFORE":
				case "BETWEEN":
				case "BIGINT":
				case "BINARY":
				case "BLOB":
				case "BOTH":
				case "BY":
				case "CALL":
				case "CASCADE":
				case "CASE":
				case "CHANGE":
				case "CHAR":
				case "CHARACTER":
				case "CHECK":
				case "COLLATE":
				case "COLUMN":
				case "CONDITION":
				case "CONSTRAINT":
				case "CONTINUE":
				case "CONVERT":
				case "CREATE":
				case "CROSS":
				case "CURRENT_DATE":
				case "CURRENT_TIME":
				case "CURRENT_TIMESTAMP":
				case "CURRENT_USER":
				case "CURSOR":
				case "DATABASE":
				case "DATABASES":
				case "DAY_HOUR":
				case "DAY_MICROSECOND":
				case "DAY_MINUTE":
				case "DAY_SECOND":
				case "DEC":
				case "DECIMAL":
				case "DECLARE":
				case "DEFAULT":
				case "DELAYED":
				case "DELETE":
				case "DESC":
				case "DESCRIBE":
				case "DETERMINISTIC":
				case "DISTINCT":
				case "DISTINCTROW":
				case "DIV":
				case "DOUBLE":
				case "DROP":
				case "DUAL":
				case "EACH":
				case "ELSE":
				case "ELSEIF":
				case "ENCLOSED":
				case "ESCAPED":
				case "EXISTS":
				case "EXIT":
				case "EXPLAIN":
				case "FALSE":
				case "FETCH":
				case "FLOAT":
				case "FLOAT4":
				case "FLOAT8":
				case "FOR":
				case "FORCE":
				case "FOREIGN":
				case "FROM":
				case "FULLTEXT":
				case "GENERAL":
				case "GET":
				case "GRANT":
				case "GROUP":
				case "HAVING":
				case "HIGH_PRIORITY":
				case "HOUR_MICROSECOND":
				case "HOUR_MINUTE":
				case "HOUR_SECOND":
				case "IF":
				case "IGNORE":
				case "IGNORE_SERVER_IDS":
				case "IN":
				case "INDEX":
				case "INFILE":
				case "INNER":
				case "INOUT":
				case "INSENSITIVE":
				case "INSERT":
				case "INT":
				case "INT1":
				case "INT2":
				case "INT3":
				case "INT4":
				case "INT8":
				case "INTEGER":
				case "INTERVAL":
				case "INTO":
				case "IO_AFTER_GTIDS":
				case "IO_BEFORE_GTIDS":
				case "IS":
				case "ITERATE":
				case "JOIN":
				case "KEY":
				case "KEYS":
				case "KILL":
				case "LEADING":
				case "LEAVE":
				case "LEFT":
				case "LIKE":
				case "LIMIT":
				case "LINEAR":
				case "LINES":
				case "LOAD":
				case "LOCALTIME":
				case "LOCALTIMESTAMP":
				case "LOCK":
				case "LONG":
				case "LONGBLOB":
				case "LONGTEXT":
				case "LOOP":
				case "LOW_PRIORITY":
				case "MASTER_BIND":
				case "MASTER_HEARTBEAT_PERIOD":
				case "MASTER_SSL_VERIFY_SERVER_CERT":
				case "MATCH":
				case "MAXVALUE":
				case "MEDIUMBLOB":
				case "MEDIUMINT":
				case "MEDIUMTEXT":
				case "MIDDLEINT":
				case "MINUTE_MICROSECOND":
				case "MINUTE_SECOND":
				case "MOD":
				case "MODIFIES":
				case "NATURAL":
				case "NOT":
				case "NO_WRITE_TO_BINLOG":
				case "NULL":
				case "NUMERIC":
				case "ON":
				case "ONE_SHOT":
				case "OPTIMIZE":
				case "OPTION":
				case "OPTIONALLY":
				case "OR":
				case "ORDER":
				case "OUT":
				case "OUTER":
				case "OUTFILE":
				case "PARTITION":
				case "PRECISION":
				case "PRIMARY":
				case "PROCEDURE":
				case "PURGE":
				case "RANGE":
				case "READ":
				case "READS":
				case "READ_WRITE":
				case "REAL":
				case "REFERENCES":
				case "REGEXP":
				case "RELEASE":
				case "RENAME":
				case "REPEAT":
				case "REPLACE":
				case "REQUIRE":
				case "RESIGNAL":
				case "RESTRICT":
				case "RETURN":
				case "REVOKE":
				case "RIGHT":
				case "RLIKE":
				case "SCHEMA":
				case "SCHEMAS":
				case "SECOND_MICROSECOND":
				case "SELECT":
				case "SENSITIVE":
				case "SEPARATOR":
				case "SET":
				case "SHOW":
				case "SIGNAL":
				case "SLOW":
				case "SMALLINT":
				case "SPATIAL":
				case "SPECIFIC":
				case "SQL":
				case "SQLEXCEPTION":
				case "SQLSTATE":
				case "SQLWARNING":
				case "SQL_AFTER_GTIDS":
				case "SQL_BEFORE_GTIDS":
				case "SQL_BIG_RESULT":
				case "SQL_CALC_FOUND_ROWS":
				case "SQL_SMALL_RESULT":
				case "SSL":
				case "STARTING":
				case "STRAIGHT_JOIN":
				case "TABLE":
				case "TERMINATED":
				case "THEN":
				case "TINYBLOB":
				case "TINYINT":
				case "TINYTEXT":
				case "TO":
				case "TRAILING":
				case "TRIGGER":
				case "TRUE":
				case "UNDO":
				case "UNION":
				case "UNIQUE":
				case "UNLOCK":
				case "UNSIGNED":
				case "UPDATE":
				case "USAGE":
				case "USE":
				case "USING":
				case "UTC_DATE":
				case "UTC_TIME":
				case "UTC_TIMESTAMP":
				case "VALUES":
				case "VARBINARY":
				case "VARCHAR":
				case "VARCHARACTER":
				case "VARYING":
				case "WHEN":
				case "WHERE":
				case "WHILE":
				case "WITH":
				case "WRITE":
				case "XOR":
				case "YEAR_MONTH":
				case "ZEROFILL":
					retval=true;
					break;
				default:
					retval=false;
					break;
			}
			if(Regex.IsMatch(input,WikiListHeaderWidths.DummyColName)) {
				retval=true;
			}
			return retval;
		}

		///<summary>Helper for Oracle that will return equivalent of MySQL IFNULL().</summary>
		public static string IfNull(string expr,int valWhenNull) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "CASE WHEN ("+expr+") IS NULL THEN "+valWhenNull+" ELSE "+expr+" END";
			}
			return "IFNULL("+expr+","+valWhenNull+")";
		}

		///<summary>Helper for Oracle that will return equivalent of MySQL IFNULL().  Automatically adds single quotes around valWhenNull so that it is treated as text in the query.</summary>
		public static string IfNull(string expr,string valWhenNull) {
			return IfNull(expr,valWhenNull,true);
		}

		///<summary>Helper for Oracle that will return equivalent of MySQL IFNULL(). Boolean to decide whether or not to encapsulate the value when null.</summary>
		public static string IfNull(string expr,string valWhenNull,bool isValEncapsulated) {
			if(isValEncapsulated) {
				valWhenNull="'"+valWhenNull+"'";
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "CASE WHEN ("+expr+") IS NULL THEN "+valWhenNull+" ELSE "+expr+" END";
			}
			return "IFNULL("+expr+","+valWhenNull+")";
		}

		///<summary>Queries information_schema.COLUMNS and returns all column names of given table.</summary>
		public static List<string> GetColumnNamesFromTableMySql(string tableName) {
			return Db.GetListString(@"
				SELECT COLUMN_NAME 
				FROM information_schema.COLUMNS
				WHERE TABLE_SCHEMA = '"+DataConnection.GetDatabaseName()+"' AND TABLE_NAME='"+tableName+"'");
		}

		///<summary>Build a select statement with omitted columns.</summary>
		public static string GetSelectCommandStringFromTableMySql(string tableName,params string[] omitColumns) {
			if(omitColumns==null||omitColumns.Length<=0||omitColumns.All(x => string.IsNullOrEmpty(x))) {
				return "SELECT * FROM "+tableName;
			}
			List<string> listAllColumns=DbHelper.GetColumnNamesFromTableMySql(tableName);
			string commandColumns="";
			foreach(string column in listAllColumns) {
				if(!string.IsNullOrEmpty(commandColumns)) {
					commandColumns+=",";
				}
				if(omitColumns.Any(x => string.Compare(x,column,true)==0)) {
					commandColumns+="'' AS "+column;
				}
				else {
					commandColumns+=column;
				}
			}
			return "SELECT "+commandColumns+" FROM "+tableName;
		}
	}
}
