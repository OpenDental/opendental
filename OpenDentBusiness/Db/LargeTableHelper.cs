using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	///<summary>Used in convert script and thus cannot change functionality without affecting conversion history.
	///If a function needs to be changed drastically, then create necessary polymorphisms of the function to handle the new scenario.
	///This class contains methods used to handle large tables - 
	///The public variables ARE NOT thread safe - The functions within run things in thread, do not call functions in this class in threads. 
	///USE SETTERS FOR THIS CLASS PRIOR TO RUNNING ANY LARGE TABLE COMMANDS!!!!!
	///TableName and TablePriKeyField are REQUIRED.  
	///ServerTo, DatabaseTo, UserTo, and PasswordTo are needed if you want to switch database connections prior to running commands. </summary>
	public class LargeTableHelper {

		#region Fields
		///<summary>The queue threads will wait until there are fewer than this many strings in the queue before adding more.</summary>
		private const int MAX_QUEUE_COUNT=4;
		///<summary>This is the number of threads that will be retrieving data and creating strings to be queued for the insert threads to process.</summary>
		private const int QUEUE_BATCHES_THREAD_COUNT=4;
		///<summary>The minimum of this and the max_allowed_packet is used to limit the number of rows per insert.  50,000,000 results in 15,000
		///histappointment rows or 59,000 securitylog rows (based on row length for the given table).</summary>
		private const int INSERT_MAX_BYTES=50000000;//50,000,000 bytes = 15000 rows for the histappointment table and 59,000 rows for securitylog
		///<summary>At least this many threads will be inserting the batches of data.  If there are more than this many cores on the current machine, it
		///will be the number of cores.</summary>
		private const int INSERT_THREAD_MIN_COUNT=8;
		///<summary>Queue to hold batches for FIFO processing.  A batch is two strings:
		///Item1: insert statement with comma delimited values per row in parentheses, i.e. INSERT INTO t (...,...) VALUES (...,...),(...,...)...
		///Item2: alternate insert stmt to be used if the Item1 stmt throws and error in order to prevent the convert from resulting in a corrupt db,
		///	i.e. INSERT INTO t ... SELECT ... FROM tempt WHERE pKey>x1 AND pKey&lt;=x2 AND pKey NOT IN (SELECT pKey FROM t WHERE pKey>x1 AND pKey&lt;=x2)
		///	The alternate statement will only insert rows that were not inserted with the first statement, but it may be slower than inserting the values
		///	explicitly and put other queries into a queue until it's finished, since it will lock the auto-increment variable. But it's only a precaution.
		///The queue is filled by QUEUE_BATCHES_THREAD_COUNT threads to a maximum capacity of MAX_QUEUE_COUNT.
		///The maximum of INSERT_THREAD_MIN_COUNT or the number of cores on the current computer will process the batches dequeued.  Make sure to use
		///_lockObjQueueBatchData when manipulating this queue for thread safety.</summary>
		private static Queue<BatchQueries> _queueBatchQueries;
		///<summary>Lock object to keep the queue thread safe.</summary>
		private static object _lockObjQueueBatchQueries=new object();
		///<summary>False until the filling threads have added the last batch of data to the queue.  Once true AND the queue is empty, the main thread is
		///finished as well.</summary>
		private static bool _areQueueBatchThreadsDone;
		///<summary>The number of rows per batch.  Used to get the primary keys for each batch and for debug console messages.</summary>
		private static int _rowsPerBatch;
		///<summary>The total number of rows to be inserted.  Only used for debug console messages.</summary>
		private static int _totalRowCount;
		///<summary>List of primary keys such that the number of rows from the previous value (or 0 if it's the first value) to the current value is
		///_numPerGroup rows.</summary>
		private static List<long> _listPriKeyMaxPerBatch;
		///<summary>Used to ensure all insert statements finished before dropping original table.</summary>
		private static int _insertBatchCount;
		///<summary>The table currently being altered/rows inserted into.</summary>
		private static string _tableName;
		///<summary>The original table's new temporary name.  This table will be dropped once all data is moved.</summary>
		private static string _tempTableName;
		///<summary>The primary key of the current table.</summary>
		private static string _tablePriKeyField;
		///<summary>The database we want to move things into.</summary>
		private static string _databaseTo;
		///<summary>The server we want to move things into.</summary>
		private static string _serverTo;
		///<summary>The user for the server we are moving things into.</summary>
		private static string _userTo;
		///<summary>The password for the server we are moving things into.</summary>
		private static string _passwordTo;
		#endregion Fields

		#region Properties
		///<summary>The name of the table you are using.</summary>
		public static string TableName {
			get {
				return _tableName;
			}
			set {
				_tableName=value;
			}
		}

		///<summary>The primary key for the table you are using.</summary>
		public static string TablePriKey {
			get {
				return _tablePriKeyField;
			}
			set {
				_tablePriKeyField=value;
			}
		}

		///<summary>The database for the server you are connecting to.</summary>
		public static string DatabaseTo {
			get {
				return _databaseTo;
			}
			set {
				_databaseTo=value;
			}
		}

		///<summary>The server you are connecting to for inserting/deleting.  If not specified, will use current server.</summary>
		public static string ServerTo {
			get {
				return _serverTo;
			}
			set {
				_serverTo=value;
			}
		}

		///<summary>The username for the server you are connecting to.</summary>
		public static string UserTo {
			get {
				return _userTo;
			}
			set {
				_userTo=value;
			}
		}

		///<summary>The password for the server you are connecting to.</summary>
		public static string PasswordTo {
			get {
				return _passwordTo;
			}
			set {
				_passwordTo=value;
			}
		}

		///<summary>Returns a list of primary keys if SetListPriKeyMaxPerBatch was run.  Typically sequential.</summary>
		public static List<long> ListPriKeyMaxPerBatch {
			get {
				return _listPriKeyMaxPerBatch;
			}
			set {
				_listPriKeyMaxPerBatch=value;
			}
		}
		#endregion Properties

		#region Database Methods

		///<summary>Renames the database that has the exact name of databaseNameCur to databaseNameNew.
		///The renaming process extremely fast (not moving any data) but is rather convoluted:
		///1. Check to see if databaseNameCur exists (throw if it doesn't)
		///2. Check to see if databaseNameNew exists (throw if it does)
		///3. Create a new database utilizing databaseNameNew.
		///4. Rename each individual table one at a time using database specificity (e.g. [databaseNameCur].appointment => [databaseNameNew].appointment)
		///5. Verify that no tables exist within databaseNameCur (throw if some linger)
		///6. Drop databaseNameCur.
		///Throws exceptions.</summary>
		public static void RenameDatabase(string databaseNameCur,string databaseNameNew) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),databaseNameCur,databaseNameNew);
				return;
			}
			#region Pre-validation
			string command="SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '"+POut.String(databaseNameCur)+"'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				throw new ApplicationException("Database does not exist so it cannot be renamed: "+POut.String(databaseNameCur));
			}
			command="SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '"+POut.String(databaseNameNew)+"'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				throw new ApplicationException("Database already exists so it cannot have another database renamed to it: "+POut.String(databaseNameNew));
			}
			#endregion
			#region Create databaseNameNew
			command="CREATE DATABASE `"+POut.String(databaseNameNew)+"` CHARACTER SET utf8";
			Db.NonQ(command);
			#endregion
			#region Rename
			//Get the names of all tables within databaseNameCur.
			command="SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='"+POut.String(databaseNameCur)+"'";
			table=Db.GetTable(command);
			//Loop through every table for databaseNameCur and rename it to databaseNameCur.
			foreach(DataRow row in table.Rows) {
				command="RENAME TABLE `"+POut.String(databaseNameCur)+"`.`"+row["TABLE_NAME"].ToString()+"` "
					+"TO `"+POut.String(databaseNameNew)+"`.`"+row["TABLE_NAME"].ToString()+"`";
				Db.NonQ(command);
			}
			#endregion
			#region Post-validation and Cleanup
			//Verify that there are no tables left within databaseNameCur.
			command="SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='"+POut.String(databaseNameCur)+"'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				throw new ApplicationException("Not all tables were successfully renamed from the database: "+POut.String(databaseNameCur));
			}
			//Drop databaseNameCur.
			command="DROP DATABASE `"+POut.String(databaseNameCur)+"`";
			Db.NonQ(command);
			#endregion
		}

		#endregion

		#region Large Table Methods
		public static string GetCurrentDatabase() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string command="SELECT database()";
			DataTable table=Db.GetTable(command);
			return PIn.String(table.Rows[0][0].ToString());
		}

		///<summary>Only used for MySql when altering a large table.  Oracle will use the normal alter table statements.  Uses the max_allowed_packet and
		///max row length to determine the number of rows that can be inserted per statement and retrieves the primary keys for each group to return that
		///number of rows and sets the class-wide list.  Used to insert the rows from a table into a copy of the table with an additional column.</summary>
		public static void SetListPriKeyMaxPerBatch(string whereClause="") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),whereClause);
				return;
			}
			_listPriKeyMaxPerBatch=new List<long>();
			SetRowsPerBatch();
			if(_rowsPerBatch==0) {
				return;
			}
			string command="SET @row=0,@maxPriKey=0; "
				+"SELECT priKey,@row totalCount "
				+"FROM ("
					+"SELECT @row:=@row+1 rowNum,@maxPriKey:=priKey priKey "
					+"FROM ("
						+"SELECT "+POut.String(_tablePriKeyField)+" priKey FROM "+_tableName+" "+whereClause+" ORDER BY "+POut.String(_tablePriKeyField)
					+") a"
				+") b "
				+"WHERE priKey=@maxPriKey OR rowNum%"+POut.Int(_rowsPerBatch)+"=0";
			DataTable t=Db.GetTable(command);
			if(t.Rows.Count<1) {
				return;
			}
			_listPriKeyMaxPerBatch=t.Select().Select(x => PIn.Long(x["priKey"].ToString())).ToList();
			_totalRowCount=PIn.Int(t.Rows[0]["totalCount"].ToString());
		}

		///<summary>Returns true if the colName is already a column in the table being modified (_tableName class-wide variable) in the database with
		///dbName.  dbName, _tableName, and colName are compared case insensitive.</summary>
		///<param name="tableName">The tableName parameter must be provided if called from outside of the LargeTableHelper class.</param>
		public static bool ColumnExists(string dbName,string colName,string tableName=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),dbName,colName,tableName);
			}
			string command="SELECT COLUMN_NAME "
				+"FROM information_schema.Columns "
				+"WHERE TABLE_SCHEMA='"+POut.String(dbName)+"' "
				+"AND TABLE_NAME='"+POut.String(string.IsNullOrEmpty(tableName)?_tableName:tableName)+"' "
				+"AND COLUMN_NAME='"+POut.String(colName)+"'";
			return !string.IsNullOrEmpty(Db.GetScalar(command));
		}

		///<summary>Using the smaller of max_allowed_packet or INSERT_MAX_BYTES and the max row length for the table with _tableName, returns the max
		///number of rows per insert statement.  Number returned is rounded down to the nearest thousand, i.e. 15,324.2341 rounds down to 15,000.
		///Maximum of 25000 is returned in order to prevent excessive memory usage.</summary>
		public static void SetRowsPerBatch() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//The SHOW command is used because it was able to run with a user that had no permissions whatsoever.
			string command="SHOW GLOBAL VARIABLES WHERE Variable_name='max_allowed_packet'";
			DataTable table=Db.GetTable(command);
			int maxAllowedPacket=0;
			if(table.Rows.Count>0) {
				maxAllowedPacket=PIn.Int(table.Rows[0]["Value"].ToString());
			}
			maxAllowedPacket=Math.Min(INSERT_MAX_BYTES,maxAllowedPacket);
			command="SELECT TRUNCATE("+POut.Int(maxAllowedPacket)+"/"//TRUNCATE(x,-3) eliminates anything after the thousands place, i.e. 25765.235412 => 25000
				+"SUM(CASE DATA_TYPE "
				+"WHEN 'tinyint' THEN 1 "
				+"WHEN 'smallint' THEN 2 "
				+"WHEN 'int' THEN 4 "
				+"WHEN 'bigint' THEN 8 "
				+"WHEN 'float' THEN IF(NUMERIC_PRECISION<=24,4,8) "
				+"WHEN 'double' THEN 8 "
				+"WHEN 'decimal' THEN FLOOR((NUMERIC_PRECISION-NUMERIC_SCALE)/9)*4+ROUND((NUMERIC_PRECISION-NUMERIC_SCALE)%9/2) "
					+"+FLOOR(NUMERIC_SCALE/9)*4+ROUND(NUMERIC_SCALE%9/2) "
				+"WHEN 'date' THEN 3 "
				+"WHEN 'time' THEN 3 "
				+"WHEN 'datetime' THEN 8 "
				+"WHEN 'timestamp' THEN 4 "
				+"WHEN 'longblob' THEN 12 "
				+"WHEN 'longtext' THEN 12 "
				+"WHEN 'mediumtext' THEN 11 "
				+"WHEN 'text' THEN 10 "
				+"WHEN 'varchar' THEN CHARACTER_OCTET_LENGTH+IF(CHARACTER_OCTET_LENGTH>255,2,1) "
				+"WHEN 'char' THEN CHARACTER_OCTET_LENGTH+IF(CHARACTER_OCTET_LENGTH>255,2,1) "
				+"ELSE 8 END),-3) AS maxNumRows "
				+"FROM information_schema.Columns c "
				+"WHERE c.TABLE_SCHEMA='"+POut.String(GetCurrentDatabase())+"' "
				+"AND c.TABLE_NAME='"+POut.String(_tableName)+"'";
			//Some extremely large tables get chopped up into huge batches (e.g. 645,000 items) and Open Dental starts to run out of memory when creating
			//each individual insert statement.  The easiest solution is to arbitrarily limit the batches to a maximum number of items.  This does not
			//affect the time it takes to execute all of these insert statements because we are going to run them all in parallel regardless.
			_rowsPerBatch=Math.Min(Db.GetInt(command),25000);
		}

		///<summary>Creates actions that load batches of data into the queue for inserting by the insert threads and runs them with
		///QUEUE_BATCHES_THREAD_COUNT number of parallel threads.  The threads will wait for the queue to drop below MAX_QUEUE_COUNT number of items
		///before queuing another item.</summary>
		public static void QueueBatches(ODThread odThread) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),odThread);
				return;
			}
			Stopwatch s=new Stopwatch();
			if(ODBuild.IsDebug()) {
				s.Start();
			}
			int queueCount=0;
			try {
				string dbName=GetCurrentDatabase();
				string cmd="SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS "
					+"WHERE TABLE_SCHEMA='"+POut.String(dbName)+"' AND TABLE_NAME='"+POut.String(_tempTableName)+"'";
				//Dictionary of key=column name, value=data type for the current table.  Used to determine whether a row value will need to have special
				//characters escaped and will need to be surrounded with quotes (using the MySql QUOTE() method).
				Dictionary<string,string> dictColNamesAndTypes=Db.GetTable(cmd).Select()
					.ToDictionary(x => PIn.String(x["COLUMN_NAME"].ToString()),x => PIn.String(x["DATA_TYPE"].ToString()));
				if(dictColNamesAndTypes.Count<1) {
					return;//table doesn't have any columns?  nothing to do?
				}
				#region Get Query Strings
				//data types that require special characters escaped and will be surrounded by quotes (using the MySql QUOTE() method).
				string[] dataTypeQuotesArr=new[] { "date","datetime","timestamp","time","char","varchar","text","mediumtext","longtext","blob","mediumblob","longblob" };
				StringBuilder sbGetSelectCommand=new StringBuilder(@"SELECT CONCAT('('");
				List<string> listWheres=new List<string>();
				int index=0;
				foreach(KeyValuePair<string,string> kvp in dictColNamesAndTypes) {
					sbGetSelectCommand.Append(@",");
					if(index>0) {
						sbGetSelectCommand.Append(@"',',");
					}
					if(dataTypeQuotesArr.Contains(kvp.Value)) {
						sbGetSelectCommand.Append(@"QUOTE("+kvp.Key+@")");
					}
					else {
						sbGetSelectCommand.Append(POut.String(kvp.Key));
					}
					index++;
				}
				sbGetSelectCommand.Append(@",')') vals FROM `"+_tempTableName+"` ");
				for(int i=0;i<_listPriKeyMaxPerBatch.Count;i++) {
					string where="WHERE "+POut.String(_tablePriKeyField)+"<="+POut.Long(_listPriKeyMaxPerBatch[i]);
					if(i>0) {
						where+=" AND "+POut.String(_tablePriKeyField)+">"+POut.Long(_listPriKeyMaxPerBatch[i-1]);
					}
					listWheres.Add(where);
				}
				#endregion Get Query Strings
				#region Run Commands and Queue Results
				#region Create List of Actions
				List<Action> listActions=new List<Action>();
				string colNames=string.Join(",",dictColNamesAndTypes.Keys.Select(x => POut.String(x)));
				foreach(string whereStr in listWheres) {
					listActions.Add(new Action(() => {
						List<string> listRowVals=Db.GetListString(sbGetSelectCommand.ToString()+whereStr);
						if(listRowVals==null || listRowVals.Count==0) {
							return;
						}
						string commandValuesInsert="REPLACE INTO `"+_tableName+"` ("+colNames+") VALUES "+string.Join(",",listRowVals);
						string commandBulkInsert="REPLACE INTO `"+_tableName+"` ("+colNames+") SELECT "+colNames+" FROM `"+_tempTableName+"` "+whereStr+" "
							+"AND "+POut.String(_tablePriKeyField)+" NOT IN (SELECT "+POut.String(_tablePriKeyField)+" FROM `"+_tableName+"` "+whereStr+")";
						bool isDataQueued=false;
						while(!isDataQueued) {
							lock(_lockObjQueueBatchQueries) {
								if(_queueBatchQueries.Count<MAX_QUEUE_COUNT) {//Wait until queue is a reasonable size before queueing more.
									_queueBatchQueries.Enqueue(new BatchQueries(commandValuesInsert,commandBulkInsert));
									isDataQueued=true;
									queueCount++;
								}
							}
							if(!isDataQueued) {
								Thread.Sleep(100);
							}
						}
					}));
				}//end of command loop
				#endregion Create List of Actions
				ODThread.RunParallel(listActions,TimeSpan.FromHours(12),QUEUE_BATCHES_THREAD_COUNT,new ODThread.ExceptionDelegate((ex) => {
					ODEvent.Fire(ODEventType.ConvertDatabases,new ProgressBarHelper("Error queuing batch: "+ex.Message,progressBarEventType:ProgBarEventType.TextMsg));
				}));
				#endregion Run Commands and Queue Results
			}
			catch(Exception ex) {
				//Don't pass along any exceptions because the main thread will validate that the table was successfully copied and will throw for us.
				ODEvent.Fire(ODEventType.ConvertDatabases,new ProgressBarHelper("Error queuing batch: "+ex.Message,progressBarEventType:ProgBarEventType.TextMsg));
			}
			finally {//always make sure to notify the main thread that the thread is done so the main thread doesn't wait for eternity
				_areQueueBatchThreadsDone=true;
				if(ODBuild.IsDebug()) {
					s.Stop();
					Console.WriteLine("QueueQueryBatches - Done, queued "+queueCount+" out of "+_listPriKeyMaxPerBatch.Count+" batches of "+_rowsPerBatch+" rows: "
						+(s.Elapsed.Hours>0?(s.Elapsed.Hours+" hours "):"")+(s.Elapsed.Minutes>0?(s.Elapsed.Minutes+" min "):"")
						+(s.Elapsed.TotalSeconds-(s.Elapsed.Hours*60*60)-(s.Elapsed.Minutes*60))+" sec");
				}
			}
		}
		
		///<summary>Creates actions and runs them in parallel threads to process the insert commands in the queue.</summary>
		public static void InsertBatches() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			Stopwatch s=new Stopwatch();
			if(ODBuild.IsDebug()) {
				s.Start();
			}
			_insertBatchCount=0;
			try {
				#region Create List of Actions
				List<Action> listActions=new List<Action>();
				int numThreads=Math.Max(INSERT_THREAD_MIN_COUNT,Environment.ProcessorCount);//use at least 8 threads, but use ProcessorCount if more than 8 cores
				for(int i=0;i<numThreads;i++) {//create numThreads number of actions, 1 per thread to run in parallel
					listActions.Add(new Action(() => {
						if(!string.IsNullOrEmpty(_serverTo)) {//SetDbT here if server is specified
							DataConnection dcon=new DataConnection();
							dcon.SetDbT(_serverTo,_databaseTo,_userTo,_passwordTo,"","",DatabaseType.MySql);
						}
						bool isBatchQueued=false;
						bool insertFailed=true;
						while(!_areQueueBatchThreadsDone || isBatchQueued) {//if queue batch thread is done and queue is empty, loop is finished
							BatchQueries batch=null;
							try {
								lock(_lockObjQueueBatchQueries) {
									if(_queueBatchQueries.Count==0) {
										//queueBatchThread must not be finished gathering batches but the queue is empty, give the batch thread time to catch up
										continue;
									}
									batch=_queueBatchQueries.Dequeue();
								}
								if(batch==null || (string.IsNullOrEmpty(batch.CommandValuesInsert) && string.IsNullOrEmpty(batch.CommandBulkInsert))) {
									continue;
								}
								Db.NonQ(batch.CommandValuesInsert);
								insertFailed=false;
							}
							catch(Exception ex) {//just loop again and wait if necessary
								ex.DoNothing();
								insertFailed=true;
								if(!string.IsNullOrEmpty(batch.CommandBulkInsert)) {
									try {
										//If multiple bulk insert commands get here at the same time they will fail 100% of the time for InnoDB an table due to 
										//a MySQL deadlock issue caused by the sub-select that makes sure it is not trying to insert duplicate rows.
										Db.NonQ(batch.CommandBulkInsert);
										insertFailed=false;
									}
									catch(Exception ex2) {
										ex2.DoNothing();
										insertFailed=true;
									}
								}
								continue;
							}
							finally {
								lock(_lockObjQueueBatchQueries) {
									if(!insertFailed) {
										insertFailed=true;
										_insertBatchCount++;
									}
									isBatchQueued=_queueBatchQueries.Count>0;
								}
							}
						}//end of while loop
					}));//end of listActions.Add
				}//end of for loop
				#endregion Create List of Actions
				ODThread.RunParallel(listActions,TimeSpan.FromHours(12),numThreads,new ODThread.ExceptionDelegate((ex) => {
					ODEvent.Fire(ODEventType.ConvertDatabases,new ProgressBarHelper("Error processing batch insert: "+ex.Message,
						progressBarEventType:ProgBarEventType.TextMsg));
				}));
			}
			catch(Exception ex) {
				ODEvent.Fire(ODEventType.ConvertDatabases,new ProgressBarHelper("Error inserting batch: "+ex.Message,progressBarEventType:ProgBarEventType.TextMsg));
			}
			if(ODBuild.IsDebug()) {
				s.Stop();
				Console.WriteLine("InsertDataThread - Done, inserted "+_insertBatchCount+" batches: "
					+(s.Elapsed.Hours>0?(s.Elapsed.Hours+" hours "):"")+(s.Elapsed.Minutes>0?(s.Elapsed.Minutes+" min "):"")
					+(s.Elapsed.TotalSeconds-(s.Elapsed.Hours*60*60)-(s.Elapsed.Minutes*60))+" sec");
			}
		}

		///<summary>NOT Oracle compatible.  This will take a set of rows from the specified table from the current database and insert them into a 
		///different database.  It does NOT create tables nor databases if they don't exist, such things need to already exist.  Enter text for the 
		///"whereClause" parameter to filter what rows to retrieve.  Needs to include the "WHERE" statement, if it's being used. (Ex. "WHERE PatNum != 2")
		///IMPORTANT NOTE --------------  MAKE SURE YOU SET CLASS-WIDE VARIABLES PRIOR TO RUNNING TABLE COMMANDS. - TableName, TablePriKey at the very least.</summary>
		public static void InsertIntoLargeTable(string whereClause) {
			Stopwatch s=new Stopwatch();
			if(ODBuild.IsDebug()) {
				s.Start();
			}
			SetListPriKeyMaxPerBatch(whereClause);
			_tempTableName=_tableName;//Set these to be equal for the Insert function - We aren't renaming tables, we are copying from one to another.
			ODThread odThreadQueueData=new ODThread(QueueBatches);
			try {
				lock(_lockObjQueueBatchQueries) {
					_queueBatchQueries=new Queue<BatchQueries>();
				}
				_areQueueBatchThreadsDone=false;
				odThreadQueueData.Name="QueueDataThread";
				odThreadQueueData.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { _areQueueBatchThreadsDone=true; }));
				odThreadQueueData.Start(true);
				InsertBatches();
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw;
			}
			finally {
				odThreadQueueData?.QuitAsync();
				_areQueueBatchThreadsDone=true;
				if(ODBuild.IsDebug()) {
					s.Stop();
					Console.WriteLine("Total time to insert into table "+_tableName+" with "+string.Format("{0:#,##0.##}",_totalRowCount)+" rows: "
						+(s.Elapsed.Hours>0?(s.Elapsed.Hours+" hours "):"")+(s.Elapsed.Minutes>0?(s.Elapsed.Minutes+" min "):"")
						+(s.Elapsed.TotalSeconds-(s.Elapsed.Hours*60*60)-(s.Elapsed.Minutes*60))+" sec");
				}
			}
		}

		///<summary>Do not use.  See AlterTable instead.</summary>
		public static void AlterLargeTable(string tableName,string priKeyField,List<Tuple<string,string>> colNamesAndDefs=null,
			List<Tuple<string,string>> indexColsAndNames=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ODException("AlterLargeTable should only be invoked from convert script classes.  Middle Tier not supported.");
			}
			Stopwatch s=new Stopwatch();
			if(ODBuild.IsDebug()) {
				s.Start();
			}
			_tableName=tableName;
			_tablePriKeyField=priKeyField;
			string dbName=GetCurrentDatabase();
			string command="";
			List<string> listChanges=new List<string>();
			colNamesAndDefs=colNamesAndDefs?.FindAll(x => !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2))??new List<Tuple<string,string>>();
			if(colNamesAndDefs.Count>0) {
				listChanges.AddRange(colNamesAndDefs.Select(x => (ColumnExists(dbName,x.Item1)?"MODIFY":"ADD")
					+" `"+x.Item1+"` "+x.Item2));//modify does nothing if data type isn't changed
			}
			indexColsAndNames=indexColsAndNames?.FindAll(x => !string.IsNullOrEmpty(x.Item1) && !IndexExists(_tableName,x.Item1))??new List<Tuple<string,string>>();
			if(indexColsAndNames.Count>0) {
				//drop any index with different columns but the same name as the index we're adding.  Useful if we are modifying the cols of a composite index
				listChanges.AddRange(indexColsAndNames.FindAll(x => !string.IsNullOrEmpty(x.Item2) && IndexNameExists(_tableName,x.Item2))
					.Select(x => "DROP INDEX `"+x.Item2+"`"));
				listChanges.AddRange(indexColsAndNames.Select(x => "ADD INDEX "+(!string.IsNullOrEmpty(x.Item2)?("`"+x.Item2+"` "):"")+"(`"+x.Item1.Replace(",","`,`")+"`)"));
			}
			if(listChanges.Count<1) {
				return;//nothing to do, just return
			}
			string alterTableStmt="ALTER TABLE `"+_tableName+"` "+string.Join(",",listChanges);
			command="SELECT ValueString FROM preference WHERE PrefName='UpdateAlterLargeTablesDirectly'";		
			if(Db.GetScalar(command)=="1") {
				Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
				return;
			}
			try {
				SetListPriKeyMaxPerBatch();
			}
			catch(Exception ex) {
				//MySQL 5.0 cannot run the command in the above method.
				Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
				ex.DoNothing();
				return;
			}
			if(_listPriKeyMaxPerBatch.Count<=1) {//if no rows, list will be empty.  One row means the table isn't very large.  Either way, small enough to run normal script
				Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
				return;
			}
			//The table is large enough to merit multiple batches.
			string rndStr=new Random().Next(1000000).ToString();
			_tempTableName="tempConv_"+_tableName+rndStr;
			command="DROP TABLE IF EXISTS `"+_tempTableName+"`";
			Db.NonQ(command);
			command="RENAME TABLE `"+_tableName+"` TO `"+_tempTableName+"`";
			Db.NonQ(command);
			command="CREATE TABLE `"+_tableName+"` LIKE `"+_tempTableName+"`";
			Db.NonQ(command);
			Db.NonQ(alterTableStmt);//execute the alter table statement on the new empty table
			command="SELECT ENGINE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='"+POut.String(dbName)+"' AND TABLE_NAME='"+POut.String(_tableName)+"'";
			string tableEngine=Db.GetScalar(command)?.ToLower();
			if(tableEngine=="myisam") {
				command="ALTER TABLE `"+_tableName+"` DISABLE KEYS";
				Db.NonQ(command);
			}
			ODThread odThreadQueueData=new ODThread(QueueBatches);
			try {
				lock(_lockObjQueueBatchQueries) {
					_queueBatchQueries=new Queue<BatchQueries>();
				}
				_areQueueBatchThreadsDone=false;
				odThreadQueueData.Name="QueueDataThread";
				odThreadQueueData.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { _areQueueBatchThreadsDone=true; }));
				odThreadQueueData.Start(true);
				InsertBatches();
				if(tableEngine=="myisam") {
					command="ALTER TABLE `"+_tableName+"` ENABLE KEYS";
					Db.NonQ(command);
				}
				if(_insertBatchCount==_listPriKeyMaxPerBatch.Count) {
					command="DROP TABLE IF EXISTS `"+_tempTableName+"`";
					Db.NonQ(command);
				}
				else {//if a batch(es) failed to run, keep the original table in case we want to drop the new and rename the original to undo this action
					try {
						command="DROP TABLE IF EXISTS `"+_tableName+"`";
						Db.NonQ(command);
						command="RENAME TABLE `"+_tempTableName+"` TO `"+_tableName+"`";
						Db.NonQ(command);
						Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
					}
					catch(Exception ex) {
						//Both the batch method of altering the table and the simple query failed. We will display an error message showing the number of rows that
						//were copied in the batch method, and we will also include the exception from the simple method as the inner exception.
						string colText="";
						if(colNamesAndDefs.Count>0) {
							colText+="column"+(colNamesAndDefs.Count>1?"s ":" ")
								+string.Join((colNamesAndDefs.Count==2?" and ":", "),colNamesAndDefs.Select(x => x.Item1))+" ";
							if(colNamesAndDefs.Count>2) {
								colText=colText.Insert(colText.LastIndexOf(", ")+2,"and ");
							}
						}
						string indexText="";
						if(indexColsAndNames.Count>0) {
							indexText="index"+(indexColsAndNames.Count>1?"es ":" ")
								+string.Join((indexColsAndNames.Count==2?" and ":", ")
									//use the index name if provided, otherwise use the column name
									,indexColsAndNames.Select(x => !string.IsNullOrEmpty(x.Item2)?x.Item2:("on column "+x.Item1)))+" ";
							if(indexColsAndNames.Count>2) {
								indexText=indexText.Insert(indexText.LastIndexOf(", ")+2,"and ");
							}
						}
						throw new ApplicationException("Adding the "+string.Join("and the ",(new[] { colText,indexText }).Where(x => !string.IsNullOrEmpty(x)))
							+" to the "+_tableName+" table failed due to incorrect number of rows being copied.",ex);
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw;
			}
			finally {
				odThreadQueueData?.QuitAsync();
				_areQueueBatchThreadsDone=true;
				if(ODBuild.IsDebug()) {
					s.Stop();
					Console.WriteLine("Total time to alter table "+_tableName+" with "+string.Format("{0:#,##0.##}",_totalRowCount)+" rows: "
						+(s.Elapsed.Hours>0?(s.Elapsed.Hours+" hours "):"")+(s.Elapsed.Minutes>0?(s.Elapsed.Minutes+" min "):"")
						+(s.Elapsed.TotalSeconds-(s.Elapsed.Hours*60*60)-(s.Elapsed.Minutes*60))+" sec");
				}
			}
		}

		///<summary>Examples are a few lines down from here.  For large tables instead of raw queries. Do NOT try-catch this method, even when only adding an index. This will rename the table specified to be preceded by "convTmp_"+tableName+"random number", creates a copy of the table structure and indexes, adds the column(s) and/or indexes. Once the empty table clone is made, keys will be disabled (for MyISAM only), all rows will be inserted from the renamed convTmp_ table, the keys (indexes) will be enabled (MyISAM only), and the convTmp_ table will be dropped.</summary>
		public static void AlterTable(string tableName,string priKeyField,
		ColNameAndDef colNameAndDef=null,IndexColsAndName indexColsAndName=null,
		List<ColNameAndDef> listColNamesAndDefs=null,List<IndexColsAndName> listIndexColsAndNames=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ODException("AlterTable should only be invoked from convert script classes.  Middle Tier not supported.");
			}
			//(Example Simple):
			//AlterTable("taskhist","TaskHistNum",new ColNameAndDef("DescriptOverride","varchar(255) NOT NULL"));
			//
			//(Example multiple columns):
			//List<ColNameAndDef> listColNameAndDef=new List<ColNameAndDef>();
			//listColNameAndDef.Add(new ColNameAndDef("DescriptOverride","varchar(255) NOT NULL"));
			//listColNameAndDef.Add(new ColNameAndDef("Test","varchar(255) NOT NULL"));
			//AlterTable("taskhist","TaskHistNum",listColNameAndDef:listColNameAndDef);
			//
			//(Example Index):
			//AlterTable("claimproc","ClaimProcNum",indexColsAndName:new IndexColsAndName("ClaimNum,ClaimPaymentNum","indexOutClaimCovering"));
			Stopwatch stopwatch=new Stopwatch();
			if(ODBuild.IsDebug()) {
				stopwatch.Start();
			}
			if(colNameAndDef!=null){
				listColNamesAndDefs=new List<ColNameAndDef>();
				listColNamesAndDefs.Add(colNameAndDef);
			}
			if(indexColsAndName!=null){
				listIndexColsAndNames=new List<IndexColsAndName>();
				listIndexColsAndNames.Add(indexColsAndName);
			}
			_tableName=tableName;
			_tablePriKeyField=priKeyField;
			string dbName=GetCurrentDatabase();
			string command="";
			List<string> listChanges=new List<string>();
			listColNamesAndDefs=listColNamesAndDefs?.FindAll(x => !string.IsNullOrEmpty(x.ColumnName) && !string.IsNullOrEmpty(x.Definition))??new List<ColNameAndDef>();;
			if(listColNamesAndDefs.Count>0) {
				for(int i=0;i<listColNamesAndDefs.Count;i++) {
					if(ColumnExists(dbName,listColNamesAndDefs[i].ColumnName)){
						listChanges.Add("MODIFY"+" `"+listColNamesAndDefs[i].ColumnName+"` "+listColNamesAndDefs[i].Definition);//modify does nothing if data type isn't changed
					}	
					else{
						listChanges.Add("ADD"+" `"+listColNamesAndDefs[i].ColumnName+"` "+listColNamesAndDefs[i].Definition);
					}
				}
			}
			listIndexColsAndNames=listIndexColsAndNames?.FindAll(x => !string.IsNullOrEmpty(x.IndexColumns) 
				&& !IndexExists(_tableName,x.IndexColumns))??new List<IndexColsAndName>();
			if(listIndexColsAndNames.Count>0) {
				//drop any index with different columns but the same name as the index we're adding.  Useful if we are modifying the cols of a composite index
				listChanges.AddRange(listIndexColsAndNames.FindAll(x => !string.IsNullOrEmpty(x.IndexName) && IndexNameExists(_tableName,x.IndexName))
					.Select(x => "DROP INDEX `"+x.IndexName+"`"));
				for(int i=0;i<listIndexColsAndNames.Count;i++) {
					if(!listIndexColsAndNames[i].IndexName.IsNullOrEmpty()) {
						listChanges.Add("ADD INDEX "+"`"+listIndexColsAndNames[i].IndexName+"` "+"(`"+listIndexColsAndNames[i].IndexColumns.Replace(",","`,`")+"`)");
					}
					else {
						listChanges.Add("ADD INDEX "+"(`"+listIndexColsAndNames[i].IndexColumns.Replace(",","`,`")+"`)");
					}
				}
			}
			if(listChanges.Count<1) {
				return;//nothing to do, just return
			}
			string alterTableStmt="ALTER TABLE `"+_tableName+"` "+string.Join(",",listChanges);
			command="SELECT ValueString FROM preference WHERE PrefName='UpdateAlterLargeTablesDirectly'";		
			if(Db.GetScalar(command)=="1") {
				Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
				return;
			}
			try {
				SetListPriKeyMaxPerBatch();
			}
			catch(Exception ex) {
				//MySQL 5.0 cannot run the command in the above method.
				Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
				ex.DoNothing();
				return;
			}
			if(_listPriKeyMaxPerBatch.Count<=1) {//if no rows, list will be empty.  One row means the table isn't very large.  Either way, small enough to run normal script
				Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
				return;
			}
			//The table is large enough to merit multiple batches.
			string rndStr=new Random().Next(1000000).ToString();
			_tempTableName="tempConv_"+_tableName+rndStr;
			command="DROP TABLE IF EXISTS `"+_tempTableName+"`";
			Db.NonQ(command);
			command="RENAME TABLE `"+_tableName+"` TO `"+_tempTableName+"`";
			Db.NonQ(command);
			command="CREATE TABLE `"+_tableName+"` LIKE `"+_tempTableName+"`";
			Db.NonQ(command);
			Db.NonQ(alterTableStmt);//execute the alter table statement on the new empty table
			command="SELECT ENGINE FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='"+POut.String(dbName)+"' AND TABLE_NAME='"+POut.String(_tableName)+"'";
			string tableEngine=Db.GetScalar(command)?.ToLower();
			if(tableEngine=="myisam") {
				command="ALTER TABLE `"+_tableName+"` DISABLE KEYS";
				Db.NonQ(command);
			}
			ODThread odThreadQueueData=new ODThread(QueueBatches);
			try {
				lock(_lockObjQueueBatchQueries) {
					_queueBatchQueries=new Queue<BatchQueries>();
				}
				_areQueueBatchThreadsDone=false;
				odThreadQueueData.Name="QueueDataThread";
				odThreadQueueData.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { _areQueueBatchThreadsDone=true; }));
				odThreadQueueData.Start(true);
				InsertBatches();
				if(tableEngine=="myisam") {
					command="ALTER TABLE `"+_tableName+"` ENABLE KEYS";
					Db.NonQ(command);
				}
				if(_insertBatchCount==_listPriKeyMaxPerBatch.Count) {
					command="DROP TABLE IF EXISTS `"+_tempTableName+"`";
					Db.NonQ(command);
				}
				else {//if a batch(es) failed to run, keep the original table in case we want to drop the new and rename the original to undo this action
					try {
						command="DROP TABLE IF EXISTS `"+_tableName+"`";
						Db.NonQ(command);
						command="RENAME TABLE `"+_tempTableName+"` TO `"+_tableName+"`";
						Db.NonQ(command);
						Db.NonQ(alterTableStmt);//execute the alter table statement on the original table
					}
					catch(Exception ex) {
						//Both the batch method of altering the table and the simple query failed. We will display an error message showing the number of rows that
						//were copied in the batch method, and we will also include the exception from the simple method as the inner exception.
						string colText="";
						if(listColNamesAndDefs.Count>0) {
							colText+="column"+(listColNamesAndDefs.Count>1?"s ":" ")
								+string.Join((listColNamesAndDefs.Count==2?" and ":", "),listColNamesAndDefs.Select(x => x.ColumnName))+" ";
							if(listColNamesAndDefs.Count>2) {
								colText=colText.Insert(colText.LastIndexOf(", ")+2,"and ");
							}
						}
						string indexText="";
						if(listIndexColsAndNames.Count>0) {
							indexText="index"+(listIndexColsAndNames.Count>1?"es ":" ")
								+string.Join((listIndexColsAndNames.Count==2?" and ":", ")
									//use the index name if provided, otherwise use the column name
									,listIndexColsAndNames.Select(x => !string.IsNullOrEmpty(x.IndexName)?x.IndexName:("on column "+x.IndexColumns)))+" ";
							if(listIndexColsAndNames.Count>2) {
								indexText=indexText.Insert(indexText.LastIndexOf(", ")+2,"and ");
							}
						}
						throw new ApplicationException("Adding the "+string.Join("and the ",(new[] { colText,indexText }).Where(x => !string.IsNullOrEmpty(x)))
							+" to the "+_tableName+" table failed due to incorrect number of rows being copied.",ex);
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw;
			}
			finally {
				odThreadQueueData?.QuitAsync();
				_areQueueBatchThreadsDone=true;
				if(ODBuild.IsDebug()) {
					stopwatch.Stop();
					Console.WriteLine("Total time to alter table "+_tableName+" with "+string.Format("{0:#,##0.##}",_totalRowCount)+" rows: "
						+(stopwatch.Elapsed.Hours>0?(stopwatch.Elapsed.Hours+" hours "):"")+(stopwatch.Elapsed.Minutes>0?(stopwatch.Elapsed.Minutes+" min "):"")
						+(stopwatch.Elapsed.TotalSeconds-(stopwatch.Elapsed.Hours*60*60)-(stopwatch.Elapsed.Minutes*60))+" sec");
				}
			}
		}

		public class ColNameAndDef {
			///<summary>Example: "PatName". Only one column name allowed.</summary>
			public string ColumnName;
			///<summary>Example: "varchar(255) NOT NULL".</summary>
			public string Definition;

			///<summary>Example: "PatName" and "varchar(255) NOT NULL".</summary>
			public ColNameAndDef(string columnName,string definition) {
				ColumnName=columnName;
				Definition=definition;
			}
		}

		public class IndexColsAndName {
			///<summary>One or multiple column names separated by commas. Example: "ClaimNum,ClaimPaymentNum".</summary>
			public string IndexColumns;
			///<summary>Example: "indexClaimPayNum". Leave empty to allow MySQL to name the index.</summary>
			public string IndexName;

			///<summary>Example: "ClaimNum,ClaimPaymentNum" and "indexClaimPayNum". IndexName can be empty for auto naming.</summary>
			public IndexColsAndName(string indexColumns,string indexName) {
				IndexColumns=indexColumns;
				IndexName=indexName;
			}
		}

		///<summary>Helper method to get the name(s) of indexes for dropping based on the table name and indexed column(s).
		///Returns a list of INDEX_NAMEs for indexes where colNames matches the concatenation of all COLUMN_NAME(s) for the column(s) referenced by an
		///index on the corresponding tableName.  If the index references multiple columns, colNames must have the column names in the exact order in
		///which the index was created separated by commas, without spaces.
		///Example: the claimproc table has the multi-column index on columns ClaimPaymentNum, Status, and InsPayAmt.
		///To get the name of any index for those columns, the parameters would be tableName="claimproc" and colNames="ClaimPaymentNum,Status,InsPayAmt".
		///Not case sensitive.  This will always return false for Oracle.</summary>
		public static List<string> GetIndexNames(string tableName,string colNames) {
			List<string> retval=new List<string>();
			if(DataConnection.DBtype==DatabaseType.Oracle) {//Oracle will not allow the same column to be indexed more than once
				return retval;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),tableName,colNames);
			}
			string command=$@"SELECT INDEX_NAME
				FROM INFORMATION_SCHEMA.STATISTICS
				WHERE TABLE_SCHEMA=SCHEMA()
				AND LOWER(TABLE_NAME)='{POut.String(tableName.ToLower())}'
				GROUP BY INDEX_NAME
				HAVING GROUP_CONCAT(LOWER(COLUMN_NAME) ORDER BY SEQ_IN_INDEX)='{POut.String(colNames.ToLower())}'";
			try {
				retval=Db.GetListString(command);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return retval;//might happen if user does not have permission to query information schema tables.
			}
			return retval;
		}

		///<summary>Helper method to determine if an index already exists.
		///Returns true if colNames matches the concatenation of all COLUMN_NAME(s) for the column(s) referenced by an index on the corresponding 
		///tableName.  If the index references multiple columns, colNames must have the column names in the exact order in which the index was created 
		///separated by commas, without spaces.
		///Example: the claimproc table has the multi-column index on columns ClaimPaymentNum, Status, and InsPayAmt.
		///To see if that index already exists, the parameters would be tableName="claimproc" and colNames="ClaimPaymentNum,Status,InsPayAmt".
		///Not case sensitive.  This will always return false for Oracle.</summary>
		public static bool IndexExists(string tableName,string colNames) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {//Oracle will not allow the same column to be indexed more than once
				return false;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),tableName,colNames);
			}
			string command="SELECT COUNT(*) FROM ("
				+"SELECT GROUP_CONCAT(LOWER(COLUMN_NAME) ORDER BY SEQ_IN_INDEX) ColNames "
				+"FROM INFORMATION_SCHEMA.STATISTICS "
				+"WHERE TABLE_SCHEMA=SCHEMA() "
				+"AND LOWER(TABLE_NAME)='"+POut.String(tableName.ToLower())+"' "
				+"GROUP BY INDEX_NAME) cols "
				+"WHERE cols.ColNames='"+POut.String(colNames.ToLower())+"'";
			try {
				if(Db.GetCount(command)=="0") {
					return false;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;//might happen if user does not have permission to query information schema tables.
			}
			return true;
		}

		///<summary>Helper method to determine if an index already exists with the given name.  Returns true if indexName matches the INDEX_NAME of any 
		///index in the table.  This will always return false for Oracle.</summary>
		public static bool IndexNameExists(string tableName,string indexName) {
			if(DataConnection.DBtype==DatabaseType.Oracle) {//Oracle will not allow the same column to be indexed more than once
				return false;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),tableName,indexName);
			}
			string command = "SELECT COUNT(DISTINCT INDEX_NAME) "
				+"FROM INFORMATION_SCHEMA.STATISTICS "
				+"WHERE TABLE_SCHEMA=SCHEMA() "
				+"AND LOWER(TABLE_NAME)='"+POut.String(tableName.ToLower())+"' "
				+"AND LOWER(INDEX_NAME)='"+POut.String(indexName.ToLower())+"'";
			try {
				if(Db.GetScalar(command)=="0") {
					return false;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;//might happen if user does not have permission to query information schema tables.
			}
			return true;
		}
		#endregion Large Table Methods

		#region Specific Table Methods
		///<summary>Uses bulk insertion techniques to find and insert all securitylog rows that are in the specified server/database and equal to or prior to the date.
		///NOTE ----- DOES NOT WORK WITH RANDOM PRIMARY KEYS</summary>
		public static string BulkInsertSecurityLogs(string serverName,string userName,string pass,DateTime dateArchive) {
			ServerTo=POut.String(serverName);
			UserTo=POut.String(userName);
			PasswordTo=POut.String(pass);
			DatabaseTo=MiscData.GetArchiveDatabaseName();
			TableName="securitylog";
			TablePriKey="SecurityLogNum";
			try { 
				//Insert security logs
				InsertIntoLargeTable("WHERE DATE(LogDateTime) <= "+POut.Date(dateArchive));
			}
			catch (Exception ex) {
				return ex.Message;
			}
			return "";
		}

		///<summary>Uses bulk insertion techniques to find and insert all securityloghash rows that are in the specified server/database and equal to or prior to the date.
		///NOTE ----- MUST BE RUN AFTER BulkInsertSecurityLogs - DOES NOT WORK WITH RANDOM PRIMARY KEYS</summary>
		public static string BulkInsertSecurityLogHashes() {
			DatabaseTo=MiscData.GetArchiveDatabaseName();
			TableName="securityloghash";
			TablePriKey="SecurityLogHashNum";
			try {
				//Insert security log hashes that correspond to maximum security log primary key obtained previously.
				if(LargeTableHelper.ListPriKeyMaxPerBatch!=null && LargeTableHelper.ListPriKeyMaxPerBatch.Count > 0) {
					InsertIntoLargeTable("WHERE SecurityLogNum <= "+POut.Long(LargeTableHelper.ListPriKeyMaxPerBatch.Max()));
				}
			}
			catch (Exception ex) {
				return ex.Message;
			}
			return "";
		}
		#endregion Specific Table Methods

		private class BatchQueries:Tuple<string,string> {
			///<summary>One insert statement with a plethora of individual value groupings that explicitly specify each value to be inserted.</summary>
			public string CommandValuesInsert { get { return Item1; } }
			///<summary>An insert statement that utilizes a single query to select from one table and insert that data into another table.</summary>
			public string CommandBulkInsert { get { return Item2; } }

			public BatchQueries(string commandValuesInsert,string commandBulkInsert) : base(commandValuesInsert,commandBulkInsert) {
			}
		}

	}
}
