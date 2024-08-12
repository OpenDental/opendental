using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.Crud;
using OpenDentBusiness.FileIO;

namespace OpenDentBusiness {
	public class DatabaseMaintenances {
		private const string _lanThis="FormDatabaseMaintenance";

		#region Get Methods
		public static List<DatabaseMaintenance> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DatabaseMaintenance>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM databasemaintenance";
			return Crud.DatabaseMaintenanceCrud.SelectMany(command);
		}
		#endregion

		#region Insert
		///<summary>Compares all DBM methods in the database to the entire list of methods passed in.</summary>
		public static void InsertMissingDBMs(List<string> listDbmMethods) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDbmMethods);
				return;
			}
			List<string> listDbmMethodNames=GetAll().Select(x => x.MethodName).ToList();
			for(int i=0;i<listDbmMethods.Count;i++){
				if(listDbmMethodNames.Contains(listDbmMethods[i])) {
					continue;
				}
				DatabaseMaintenance databaseMaintenance=new DatabaseMaintenance();
				databaseMaintenance.MethodName=listDbmMethods[i];
				Crud.DatabaseMaintenanceCrud.Insert(databaseMaintenance);
			}
		}
		#endregion

		#region Update
		public static void Update(DatabaseMaintenance databaseMaintenance) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),databaseMaintenance);
				return;
			}
			Crud.DatabaseMaintenanceCrud.Update(databaseMaintenance);
		}

		///<summary>Moves a DBM from the 'Checks' to the 'Old' tab by updating DatabaseMaintenance.IsOld=true</summary>
		public static void MoveToOld(string methodName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),methodName);
				return;
			}
			string command="UPDATE databasemaintenance SET IsOld=1 "
				+"WHERE MethodName='"+POut.String(methodName)+"'";
			Db.NonQ(command);
		}

		///<summary>Updates the DateLastRun column to NOW for any DBM method that matches the method name passed in.</summary>
		public static void UpdateDateLastRun(string methodName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),methodName);
				return;
			}
			string command="UPDATE databasemaintenance SET DateLastRun="+DbHelper.Now()+" "
				+"WHERE MethodName='"+POut.String(methodName)+"'";
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		///<summary>Adds the logText to a centralized log file for the current day if the current data storage type is LocalAtoZ.
		///Throws exceptions to be displayed to the user.</summary>
		public static void SaveLogToFile(string logText) {
			//No need to check MiddleTierRole; no call to db.
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				//If docs are stored in DB, we don't want to create a file because the user has no way to access it.
				//We also skip any cloud storage at this time, could enhance later to include.
				return; //Don't make a log.
			}
			string machineName="~INVALID~";
			ODException.SwallowAnyException(() => { machineName=ODEnvironment.MachineName; });
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.Append(DateTime.Now.ToString());
			stringBuilder.Append(" - Computer Name: "+machineName);
			stringBuilder.Append('-',45);
			stringBuilder.AppendLine();//New line.
			stringBuilder.AppendLine(logText.Trim());
			stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Done"));
			string path=CodeBase.ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"DBMLogs");
			if(!Directory.Exists(path)) {
					Directory.CreateDirectory(path);//Create DBM Logs folder if it does not exist.
			}
			string logFile=CodeBase.ODFileUtils.CombinePaths(path,DateTime.Now.ToShortDateString().Replace("/","_")+".txt");
			try {
				File.AppendAllText(logFile,stringBuilder.ToString());//One file per date
			}
			catch(SecurityException ex) {
				throw new ODException(Lans.g("FormDatabaseMaintenance","Log not saved to DBM Logs folder because user does not have permission to access that file."),ex);
			}
			catch(UnauthorizedAccessException unauthorizedAccessEx) {
				throw new ODException(Lans.g("FormDatabaseMaintenance","Log not saved to DBM Logs folder because user does not have permission to access that file."),unauthorizedAccessEx);
			}
			//Throw all other types of exceptions like usual.
		}
		#endregion

		#region List of Tables and Columns for null check---------------------------------------------------------------------------------------------------
		///<summary>List of tables and columns to remove null characters from.
		///Loop through this list two items at a time because it is designed to have a table first which is then followed by a relative column.</summary>
		public static List<string> ListTableAndColumns=new List<string>() {
				//Table					//Column
				"adjustment",   "AdjNote",
				"appointment",  "Note",
				"commlog",      "Note",
				"definition",   "ItemName",
				"diseasedef",   "DiseaseName",
				"patient",      "Address",
				"patient",      "Address2",
				"patient",      "AddrNote",
				"patient",      "MedUrgNote",
				"patient",      "WirelessPhone",
				"patientnote",  "FamFinancial",
				"patientnote",  "Medical",
				"patientnote",  "MedicalComp",
				"patientnote",  "Service",
				"patientnote",  "Treatment",
				"payment",      "PayNote",
				"popup",        "Description",
				"procnote",     "Note",
				"securitylog",  "LogText",
			};
		#endregion List of Tables and Columns for null check------------------------------------------------------------------------------------------------

		#region Methods That Affect All or Many Tables------------------------------------------------------------------------------------------------------

		///<summary></summary>
		[DbmMethodAttr(HasBreakDown=true,HasWarningMessage=true)]
		public static string MySQLServerOptionsValidate(bool verbose,DbmMode dbmMode) {
			if(PrefC.GetBool(PrefName.DatabaseGlobalVariablesDontSet)) {
				return "";//Hosted databases don't have permission to call SET GLOBAL. Also the MySQL variables can be assumed to be correct.
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			List<GlobalVariableH> listGlobalVariableHs=new List<GlobalVariableH>();
			listGlobalVariableHs.Add(new GlobalVariableH("sql_mode","no_auto_create_user"));
			listGlobalVariableHs.Add(new GlobalVariableH("myisam_recover_options","off"));
			listGlobalVariableHs.Add(new GlobalVariableH("slave_skip_errors","off"));
			listGlobalVariableHs.Add(new GlobalVariableH("optimizer_switch", "split_materialized=off", requiresVersionCheck:true, "version", "10.5.9"));
			// Check to see if user may not have permission to access global variables.
			if(listGlobalVariableHs.Exists(x=>x.MayLackPermissions)) {
				return Lans.g("FormDatabaseMaintenance","Unable to access MySQL server global variables to validate server options, probably due to permissions")+".\r\n";
			}
			if(listGlobalVariableHs.TrueForAll(x=>x.IsValid)) { // Check to see if we have issues or if we can simply return here.
				string successLog="";
				if(verbose) {
					successLog+="Done.  No maintenance needed.\r\n\r\n";
					for(int i = 0;i<listGlobalVariableHs.Count;i++) {
						successLog+=listGlobalVariableHs[i].GetMessage();
					}
				}
				return successLog;
			}
			// If we have any issues, we reset the log (which would only have messages relating to non-error states at this point), and add the approriate errors/issues to the log. We set hasCritcalWarning=true to indicate that we have possible data-loss due to the incorrect settings.
			string warningMessage=Lans.g("FormDatabaseMaintenance","There are some global MySQL variables that are not set correctly. A manual fix is required to prevent data loss. For more information please check the web manual at")+" https://opendental.com/manual/myini.html ";
			string issueLog="";
			switch(dbmMode) {
				case DbmMode.Breakdown:
					for(int i = 0;i<listGlobalVariableHs.Count;i++) {
						// Grab only the invalid/error messages.
						if(listGlobalVariableHs[i].IsValid) {
							continue;
						}
						issueLog+=listGlobalVariableHs[i].GetMessage();
					}
					break;
				case DbmMode.Check:
					if(listGlobalVariableHs.TrueForAll(x=>x.IsValid)) {
						break;
					}
					issueLog+=warningMessage+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					break;
				case DbmMode.Fix:
					/*try {
						command="SET GLOBAL sql_mode=''";
						Db.NonQ(command);
						command="SET SESSION sql_mode=''";
						Db.NonQ(command);
						log+=Lans.g("FormDatabaseMaintenance","The MySQL server variable 'sql_mode' has been changed from")+" "+sqlmodeDisplay+" "
							+Lans.g("FormDatabaseMaintenance","to blank")+".\r\n";
					}
					catch(Exception ex) {
						ex.DoNothing();//prevent vs warning
						log+=Lans.g("FormDatabaseMaintenance","Unable to set the MySQL server variable 'sql_mode', probably due to permissions")+".  "
							+Lans.g("FormDatabaseMaintenance","The sql_mode must be blank or NO_AUTO_CREATE_USER and is currently set to")
							+" "+sqlmodeDisplay+".\r\n";
					}*/
					//E46954 disabled the "Fix" logic above in favor of showing the bad config to the user and asking them to fix it with suggested options
					//However, we must still return an error message from this case in order to trigger the correct feedback logic
					if(listGlobalVariableHs.TrueForAll(x => x.IsValid)) {
						break;
					}
					issueLog+=warningMessage+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					break;
			}//end switch
			return issueLog;
		}

		///<summary>Returns a Result with Msg=log string and IsSuccess=whether the table checks were successful.</summary>
		public static Result MySQLTables(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Result>(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				Result resultSkipped=new Result();
				resultSkipped.IsSuccess=true;
				return resultSkipped;
			}
			if(PrefC.GetBool(PrefName.DatabaseMaintenanceSkipCheckTable)) {
				Result resultSkipped=new Result();
				resultSkipped.IsSuccess=true;
				return resultSkipped;
			}
			string command="DROP TABLE IF EXISTS `signal`";//Signal is keyword for MySQL 5.5.  Was renamed to signalod so drop if exists.
			Db.NonQ(command);
			command="SHOW FULL TABLES WHERE Table_type='BASE TABLE'";//Tables, not views.  Does not work in MySQL 4.1, however we test for MySQL version >= 5.0 in PrefL.
			DataTable table=Db.GetTable(command);
			List<string> listTableNames=new List<string>();
			int lastRow;
			bool existsCorruptFiles=false;
			bool existsUnvalidatedTables=false;
			for(int i = 0;i<table.Rows.Count;i++) {
				listTableNames.Add(table.Rows[i][0].ToString());
			}
			string checkingTable=Lans.g(nameof(MiscData),"Checking table");
			bool isSuccess=true;
			string log="";
			for(int i=0;i<listTableNames.Count;i++) {
				//Alert the thread we are running this on that we are checking this table.
				ODEvent.Fire(ODEventType.ProgressBar,checkingTable+": "+listTableNames[i]);
				command="CHECK TABLE `"+listTableNames[i]+"`";
				try {
					table=Db.GetTable(command);		
				}
				catch(Exception ex) {
					log+=Lans.g("FormDatabaseMaintenance","Unable to validate table")+" "+listTableNames[i]+"\r\n"+ex.Message+"\r\n";
					existsUnvalidatedTables=true;
				}
				lastRow=table.Rows.Count-1;
				string status=PIn.ByteArray(table.Rows[lastRow][3]);
				if(status!="OK") {
					log+=Lans.g("FormDatabaseMaintenance","Corrupt file found for table")+" "+listTableNames[i]+"\r\n";
					existsCorruptFiles=true;
				}
			}
			if(existsUnvalidatedTables) {
				isSuccess=false;//no other checks should be done until we can successfully get past this.
				log+=Lans.g("FormDatabaseMaintenance","Tables found that could not be validated.")+"\r\n"
					+Lans.g("FormDatabaseMaintenance","Done.");
			}
			if(existsCorruptFiles) {
				isSuccess=false;//no other checks should be done until we can successfully get past this.
				log+=Lans.g("FormDatabaseMaintenance","Corrupted database files found, please call support immediately or see manual for more details.")+"\r\n"
					+Lans.g("FormDatabaseMaintenance","Done.");
			}
			if(isSuccess) {
				if(verbose) {
					log+=Lans.g("FormDatabaseMaintenance","Tables validated successfully.  No corrupted tables.")+"\r\n";
				}
			}
			Result result=new Result();
			result.IsSuccess=isSuccess;
			result.Msg=log;
			return result;
		}

		///<summary>If using MySQL, tries to repair and then optimize each table.
		///Developers must make a backup prior to calling this method because repairs have a tendency to delete data.
		///Currently called whenever MySQL is upgraded and when users click Optimize in database maintenance.</summary>
		public static void RepairAndOptimize() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				return;
			}
			string command="SHOW FULL TABLES WHERE Table_type='BASE TABLE';";//Tables, not views.  Does not work in MySQL 4.1, however we test for MySQL version >= 5.0 in PrefL.
			DataTable table=Db.GetTable(command);
			List<string> listTableNames=new List<string>();
			for(int i=0;i<table.Rows.Count;i++) {
				listTableNames.Add(table.Rows[i][0].ToString());
			}
			List<string> listInnoDbTableNames=InnoDb.GetInnodbTableNames().Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
			bool forceOptimize=true;
			if(new Version(MiscData.GetMySqlVersion())<=new Version(5,5)) {
				//MySQL version 5.5 or earlier, optimize is not supported in versions 5.5 or lower. 
				forceOptimize=false;
			}
			for(int i=0;i<listTableNames.Count;i++) {
				//Alert anyone that cares that we are optimizing this table.
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("MiscData","Optimizing table")+": "+listTableNames[i]);
				OptimizeTable(listTableNames[i],forceOptimize,listInnoDbTableNames);
			}
			for(int i=0;i<listTableNames.Count;i++) {
				if(listInnoDbTableNames.Contains(listTableNames[i])) {//Skip innodb tables
					continue;
				}
				//Alert anyone that cares that we are repairing this table.
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("MiscData","Repairing table")+": "+listTableNames[i]);
				command="REPAIR TABLE `"+listTableNames[i]+"`";
				Db.NonQ(command);
			}
		}

		///<summary>Optimizes the table passed in. Returns an error if the table was not able to be optimized. Returns empty string if not using MySQL.
		///Does not attempt the optimize if random PKs is turned on, or if the table is of storage engine InnoDB (unless canOptimizeInnodb is true).
		///See wiki page [[Database Storage Engine Comparison: InnoDB vs MyISAM]] for reasons why.
		///As documented online, https://dev.mysql.com/doc/refman/5.6/en/optimize-table.html, MySQL 5.6 supports the optimize
		///command for MyISAM, InnoDB and Archive tables only.</summary>
		public static string OptimizeTable(string tableName,bool forceOptimize=false,List<string> listInnoDbTableNames=null) {
			if(PrefC.GetBool(PrefName.RandomPrimaryKeys)) {
				return tableName+" "+Lans.g("MiscData","skipped due to using random primary keys.");
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),tableName,forceOptimize,listInnoDbTableNames);
			}
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				return "";//Non-MySQL DBMS might not even have an optimize option.
			}
			if(forceOptimize) {
				//go straight to optimization
			}
			else {
				//Check to see if the table has its storage engine set to InnoDB.
				if(listInnoDbTableNames==null) {
					listInnoDbTableNames=new List<string>();
					string command="SELECT ENGINE FROM information_schema.TABLES "
						+"WHERE TABLE_SCHEMA='"+POut.String(DataConnection.GetDatabaseName())+"' "
						+"AND TABLE_NAME='"+POut.String(tableName)+"' ";
					string storageEngine=Db.GetScalar(command);
					if(storageEngine.ToLower()=="innodb") {
						listInnoDbTableNames.Add(tableName);
					}
				}
				if(listInnoDbTableNames.Any(x => x.ToLower()==tableName.ToLower())) {
					return tableName+" "+Lans.g("MiscData","skipped due to using the InnoDB storage engine.");
				}
			}
			//Only run OPTIMIZE if random PKs are not used and the table is not using the InnoDB storage engine.
			DataConnection.CommandTimeout=43200;//12 hours, because conversion commands may take longer to run.
			try {
				Db.NonQ("OPTIMIZE TABLE `"+tableName+"`");//Ticks used in case user has added custom tables with unusual characters.
			}
			finally {
				DataConnection.CommandTimeout=3600;//Set back to default of 1 hour.
			}
			return "";
		}

		[DbmMethodAttr(HasBreakDown=true)]
		public static string DatesNoZeros(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				//This check is not valid for Oracle, because each of the following fields are defined as non-null, and 0000-00-00 is not a valid Oracle date.
				return "";
			}
			//dynamically get every single date, datetime, and timestamp column from all tables in the db.
			string command = @"
				SELECT cols.TABLE_NAME, cols.COLUMN_NAME, cols.COLUMN_DEFAULT
				FROM information_schema.COLUMNS cols
				WHERE cols.DATA_TYPE IN ('datetime','date','timestamp')
				AND TABLE_SCHEMA = '"+DataConnection.GetDatabaseName()+"'";
			DataTable table = Db.GetTable(command);
			int countTotal=0;
			List<string> listInvalidColNames = new List<string>();
			List<string> listErrors=new List<string>();
			//for each of those columns
			List<string> listDbCommands = new List<string>();
			for (int i=0;i<table.Rows.Count;i++){
				string tableName = PIn.String(table.Rows[i]["TABLE_NAME"].ToString());
				string columnName = PIn.String(table.Rows[i]["COLUMN_NAME"].ToString());
				try {
					//get the primary key of that column's table
					command = "SHOW KEYS FROM "+tableName+" WHERE Key_name='PRIMARY'";
					DataTable tablePKs=Db.GetTable(command);
					if(tablePKs.Rows.Count==0) {
						continue;//Should never happen but there might be tables without a primary key.
					}
					string priKeyCol = tablePKs.Rows[0]["Column_name"].ToString();
					//check to see if there are any invalid dates
					command = "SELECT "+priKeyCol+" FROM "+tableName+" WHERE "+columnName+" = '0000-00-00'";//works for invalid dates, datetimes, and timestamps.
					DataTable tableInvalid = Db.GetTable(command);
					//and count them up.
					countTotal+=tableInvalid.Rows.Count;
					//if there are some that are invalid, then fix them by setting them to the default value.
					//default value is usually 0001-01-01 for most dates, but can be the current timestamp for DateTStamp columns.
					if(tableInvalid.Rows.Count > 0) {
						string priKeys = String.Join(",",tableInvalid.Rows.Cast<DataRow>().Select(x => x[0]).ToList());
						listInvalidColNames.Add(tableName+"."+columnName+": Keys ("+priKeys+")");
						listDbCommands.Add("UPDATE "+tableName+" SET "+columnName+" = DEFAULT WHERE "+priKeyCol+" IN ("+priKeys+")");
					}
				}
				catch(Exception ex) {
					//This could happen if we're trying to inspect a temp table that has since been deleted.
					listErrors.Add(tableName+"."+columnName+": "+ex.Message);
				}
			}
			string log="";
			if(listErrors.Count > 0) {
				log+=Lans.g("FormDatabaseMaintenance","Unable to check the following columns:")+"\r\n";
				log+=String.Join("\r\n",listErrors)+"\r\n";
			}
			switch(dbmMode) {
				case DbmMode.Check:
					if(countTotal > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Number of rows with invalid dates found:")+" "+countTotal;
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(countTotal > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Number of rows with invalid dates found:")+" "+countTotal+"\r\n";
						log+=Lans.g("FormDatabaseMaintenance","The following table.Column(s) have rows with invalid dates:")+"\r\n";
						log+=String.Join("\r\n",listInvalidColNames);
					}
					break;
				case DbmMode.Fix:
					if(countTotal > 0 || verbose) {
						for(int i=0;i<listDbCommands.Count;i++){
							Db.NonQ(listDbCommands[i]);
						}
						log+=Lans.g("FormDatabaseMaintenance","Number of rows with invalid dates fixed:")+" "+countTotal+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Deprecated.</summary>
		public static string DecimalValues(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			//This specific fix is no longer needed, since we are using ROUND(EstBalance,2) in the aging calculation now.
			//However, it is still a problem in many columns that we will eventually need to deal with.
			//Maybe add this back when users can control which fixes they make.
			//One problem is the foreign users do not necessarily use 2 decimal places (Kuwait uses 3).
			////Holds columns to be checked. Strings are in pairs in the following order: table-name,column-name
			//string[] decimalCols=new string[] {
			//  "patient","EstBalance"
			//};
			//int decimalPlacessToRoundTo=8;
			//long numberFixed=0;
			//for(int i=0;i<decimalCols.Length;i+=2) {
			//  string tablename=decimalCols[i];
			//  string colname=decimalCols[i+1];
			//  string command="UPDATE "+tablename+" SET "+colname+"=ROUND("+colname+","+decimalPlacessToRoundTo
			//    +") WHERE "+colname+"!=ROUND("+colname+","+decimalPlacessToRoundTo+")";
			//  numberFixed+=Db.NonQ(command);
			//}
			//if(numberFixed>0 || verbose) {
			//  log+=Lans.g("FormDatabaseMaintenance","Decimal values fixed: ")+numberFixed.ToString()+"\r\n";
			//}
			return log;
		}

		///<summary>also checks patient.AddrNote</summary>
		[DbmMethodAttr(HasExplain=true)]
		public static string SpecialCharactersInNotes(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			//this will run for fix or check, but will only fix if the special char button is used 
			//Fix code is in a dedicated button "Spec Char"
			string command="SELECT * FROM appointment WHERE (ProcDescript REGEXP '[^[:alnum:]^[:space:]^[:punct:]]+') OR (Note REGEXP '[^[:alnum:]^[:space:]^[:punct:]]+')";
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			List<char> listCharsSpecial=new List<char>();
			int countSpecialChar=0;
			int intC=0;
			for(int i=0;i<listAppointments.Count;i++){
				for(int j=0;j<listAppointments[i].Note.Length;j++){
					intC=(int)listAppointments[i].Note[j];
					if((intC<126 && intC>31)//31 - 126 are all safe.
						|| intC==9     //"Horizontal Tabulation"
						|| intC==10    //Line Feed
						|| intC==13) { //carriage return
						continue;
					}
					countSpecialChar++;
					if(listCharsSpecial.Contains(listAppointments[i].Note[j])) {
						continue;
					}
					listCharsSpecial.Add(listAppointments[i].Note[j]);
				}
				for(int j=0;j<listAppointments[i].ProcDescript.Length;j++){//search every character in ProcDescript
					intC=(int)listAppointments[i].ProcDescript[j];
					if((intC<126 && intC>31)//31 - 126 are all safe.
						|| intC==9     //"Horizontal Tabulation"
						|| intC==10    //Line Feed
						|| intC==13) { //carriage return
						continue;
					}
					countSpecialChar++;
					if(listCharsSpecial.Contains(listAppointments[i].ProcDescript[j])) {
						continue;
					}
					listCharsSpecial.Add(listAppointments[i].ProcDescript[j]);
				}
			}
			command="SELECT * FROM patient WHERE AddrNote REGEXP '[^[:alnum:]^[:space:]]+'";
			List<Patient> listPatients=OpenDentBusiness.Crud.PatientCrud.SelectMany(command);
			intC=0;
			for(int i=0;i<listPatients.Count;i++){
				for(int j=0;j< listPatients[i].AddrNote.Length;j++){
					intC=(int)listPatients[i].AddrNote[j];
					if((intC<126 && intC>31)//31 - 126 are all safe.
						|| intC==9      //"Horizontal Tabulation"
						|| intC==10     //Line Feed
						|| intC==13) {  //carriage return
						continue;
					}
					countSpecialChar++;
					if(listCharsSpecial.Contains(listPatients[i].AddrNote[j])) {
						continue;
					}
					listCharsSpecial.Add(listPatients[i].AddrNote[j]);
				}
			}
			for(int i=0;i<listCharsSpecial.Count;i++){
				log+=listCharsSpecial[i].ToString()+" doesn't work.\r\n";
			}
			for(int i = 0;i<ListTableAndColumns.Count;i+=2) {
				string tableName=ListTableAndColumns[i];
				string columnName=ListTableAndColumns[i+1];
				command="SELECT COUNT(*) FROM "+tableName+" WHERE "+columnName+" LIKE '%"+POut.String("\0")+"%'";
				countSpecialChar+=PIn.Int(Db.GetCount(command));
			}
			if(countSpecialChar!=0 || verbose) {
				log+=countSpecialChar.ToString()+" "+Lans.g("FormDatabaseMaintenance","total special characters found.  The Spec Char tool will remove these characters.")+"\r\n";
			}
			return log;
		}

		private class TableColKey{
			public string Table;
			public string Col;
			public string Key;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string NotesWithTooMuchWhiteSpace(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command;
			string logBreakdown="";
			long countTotal=0;
			string tooManyT=string.Join("",Enumerable.Repeat("\t",30));//Can't think of any good reason to have more than 30 tabs in a row
			string tooManySP=string.Join("",Enumerable.Repeat(@" ",300));//Spaces are very easy to draw so only remove ridiculous amounts of them.
			string tooManyRN=string.Join("",Enumerable.Repeat("\r\n",30));// \r\n, \r\n, \r\n... as fast as you can!
			string tooManyN=string.Join("",Enumerable.Repeat("\n",30));// Sometimes we have had newlines encoded as \n
			string tooMuchMixedSp="\\s{150,}";//Copying and pasting notes into database could lead to multiple tabs/spaces followed by a carriage return
			//Create a list of type TableColKeys to easily see which tables, columns, and primary keys will be considered in this method.
			List<TableColKey> listTableColKeys=new List<TableColKey>();
			TableColKey tableColKey;
			tableColKey=new TableColKey();
			tableColKey.Table="appointment";
			tableColKey.Col="Note";
			tableColKey.Key="AptNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="commlog";
			tableColKey.Col="Note";
			tableColKey.Key="CommlogNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="procnote";
			tableColKey.Col="Note";
			tableColKey.Key="ProcNoteNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="patient";
			tableColKey.Col="FamFinUrgNote";
			tableColKey.Key="PatNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="patient";
			tableColKey.Col="AddrNote";
			tableColKey.Key="PatNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="patfield";
			tableColKey.Col="FieldValue";
			tableColKey.Key="PatFieldNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="insplan";
			tableColKey.Col="PlanNote";
			tableColKey.Key="PlanNum";
			listTableColKeys.Add(tableColKey);
			tableColKey=new TableColKey();
			tableColKey.Table="inssub";
			tableColKey.Col="SubscNote";
			tableColKey.Key="InsSubNum";
			listTableColKeys.Add(tableColKey);
			for(int i=0;i<listTableColKeys.Count;i++) {
				string tableName=listTableColKeys[i].Table;
				string colName=listTableColKeys[i].Col;
				string priKey=listTableColKeys[i].Key;
				#region Tabs
				switch(dbmMode) {
					case DbmMode.Breakdown:
					case DbmMode.Check:
						command="SELECT COUNT(*) FROM "+POut.String(tableName)+" WHERE "+POut.String(colName)+" LIKE '%"+POut.String(tooManyT)+"%'";
						int countLocal=PIn.Int(Db.GetCount(command));
						countTotal+=countLocal;
						if(dbmMode==DbmMode.Breakdown) {
							logBreakdown+=POut.String(tableName)+"."+POut.String(colName)+" "+Lans.g("FormDatabaseMaintenance","rows with too many tabs found:")
								+" "+countLocal+"\r\n";
						}
						break;
					case DbmMode.Fix:
						command="SELECT "+priKey+","+colName+" FROM "+POut.String(tableName)+" WHERE "+POut.String(colName)+" LIKE '%"+POut.String(tooManyT)+"%'";
						DataTable table=Db.GetTable(command);
						if(table.Rows.Count>0 || verbose) {
							for(int j=0;j<table.Rows.Count;j++) {
								long id=PIn.Long(table.Rows[j][priKey].ToString());
								string oldNote=PIn.String(table.Rows[j][colName].ToString());
								string newNote=Regex.Replace(oldNote,POut.String(tooManyT)+"[\t]*","");
								command="UPDATE "+POut.String(tableName)+" SET "+POut.String(colName)+"='"+POut.String(newNote)+"' WHERE "+POut.String(priKey)+"="+POut.Long(id);
								Db.NonQ(command);
								countTotal++;
							}
						}
						break;
				}
				#endregion
				#region Newlines
				switch(dbmMode) {
					case DbmMode.Breakdown:
					case DbmMode.Check:
						command="SELECT COUNT(*) FROM "+POut.String(tableName)+" "
							+"WHERE "+POut.String(colName)+" LIKE '%"+POut.String(tooManyRN)+"%' "
							+"OR "+POut.String(colName)+" LIKE '%"+POut.String(tooManyN)+"%'";
						int countLocal=PIn.Int(Db.GetCount(command));
						countTotal+=countLocal;
						if(dbmMode==DbmMode.Breakdown) {
							logBreakdown+=POut.String(tableName)+"."+POut.String(colName)+" "+Lans.g("FormDatabaseMaintenance","rows with too many newlines found:")
								+" "+countLocal+"\r\n";
						}
						break;
					case DbmMode.Fix:
						command="SELECT "+priKey+","+colName+" FROM "+POut.String(tableName)+" "
							+"WHERE "+POut.String(colName)+" LIKE '%"+POut.String(tooManyRN)+"%' "
							+"OR "+POut.String(colName)+" LIKE '%"+POut.String(tooManyN)+"%'";
						DataTable table=Db.GetTable(command);
						if(table.Rows.Count>0 || verbose) {
							for(int j=0;j<table.Rows.Count;j++) {
								long id=PIn.Long(table.Rows[j][priKey].ToString());
								string oldNote=PIn.String(table.Rows[j][colName].ToString());
								string newNote=Regex.Replace(oldNote,POut.String(tooManyRN)+"[\r\n]*","\r\n");
								newNote=Regex.Replace(newNote,POut.String(tooManyN)+"[\n]*","\r\n");
								command="UPDATE "+POut.String(tableName)+" SET "+POut.String(colName)+"='"+POut.String(newNote)+"' WHERE "+POut.String(priKey)+"="+POut.Long(id);
								Db.NonQ(command);
								countTotal++;
							}
						}
						break;
				}
				#endregion
				#region Trailing Spaces
				switch(dbmMode) {
					case DbmMode.Breakdown:
					case DbmMode.Check:
						command=@"SELECT COUNT(*) FROM "+POut.String(tableName)+" "
							+@"WHERE "+POut.String(colName)+" LIKE '%"+POut.String(tooManySP)+"%' ";//This is Sparta!
						int countLocal=PIn.Int(Db.GetCount(command));
						countTotal+=countLocal;
						if(dbmMode==DbmMode.Breakdown) {
							logBreakdown+=POut.String(tableName)+"."+POut.String(colName)+" "+Lans.g("FormDatabaseMaintenance","rows with too many trailing white spaces found:")
								+" "+countLocal+"\r\n";
						}
						break;
					case DbmMode.Fix:
						command="SELECT "+priKey+","+colName+" FROM "+POut.String(tableName)+" "
							+"WHERE "+POut.String(colName)+" LIKE '%"+POut.String(tooManySP)+"%' ";
						DataTable table=Db.GetTable(command);
						if(table.Rows.Count>0 || verbose) {
							for(int j=0;j<table.Rows.Count;j++) {
								long id=PIn.Long(table.Rows[j][priKey].ToString());
								string oldNote=PIn.String(table.Rows[j][colName].ToString());
								string newNote=Regex.Replace(oldNote,POut.String(tooManySP)+"[ ]*","");
								command="UPDATE "+POut.String(tableName)+" SET "+POut.String(colName)+"='"+POut.String(newNote)+"' WHERE "+POut.String(priKey)+"="+POut.Long(id);
								Db.NonQ(command);
								countTotal++;
							}
						}
						break;
				}
				#endregion
				#region Mixed Spaces
				//If a string satisfies the above requirements, chances are the check will show that it found a row that satisfies this region as well.
				//No need to worry about overcorrecting though as the above regions will address those problem areas first. 
				//This last check catches all the rest. Once the DBM is done, all problem areas should be fixed.
				switch(dbmMode) {
					case DbmMode.Breakdown:
					case DbmMode.Check:
						command=$"SELECT COUNT(*) FROM {POut.String(tableName)} WHERE {POut.String(colName)} REGEXP '[[:space:]]{{150,}}'";//finds any 150 consecutive spaces in a row
						int countLocal=PIn.Int(Db.GetCount(command));
						countTotal+=countLocal;
						if(dbmMode==DbmMode.Breakdown) {
							logBreakdown+=POut.String(tableName)+"."+POut.String(colName)+" "+Lans.g("FormDatabaseMaintenance","rows with too many consecutive white spaces found:")
								+" "+countLocal+"\r\n";
						}
						break;
					case DbmMode.Fix:
						command=$"SELECT {POut.String(priKey)},{POut.String(colName)} FROM {POut.String(tableName)} WHERE {POut.String(colName)} REGEXP '[[:space:]]{{150,}}'";
						DataTable table=Db.GetTable(command);
						if(table.Rows.Count>0 || verbose) {
							for(int j=0;j<table.Rows.Count;j++) {
								long priKeyValue=PIn.Long(table.Rows[j][priKey].ToString());
								string oldNote=PIn.String(table.Rows[j][colName].ToString());
								string newNote=Regex.Replace(oldNote,tooMuchMixedSp," ");
								command=$"UPDATE {POut.String(tableName)} SET {POut.String(colName)}='{POut.String(newNote)}' WHERE {POut.String(priKey)}={POut.Long(priKeyValue)}";
								Db.NonQ(command);
								countTotal++;
							}
						}
						break;
				}
				#endregion
			}
			string log="";
			if(countTotal>0 || verbose) {
				switch(dbmMode) {
					case DbmMode.Breakdown:
						log=logBreakdown;
						break;
					case DbmMode.Check:
						log=Lans.g("FormDatabaseMaintenance","Notes with too much white space found:")+" "+countTotal.ToString()+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
						break;
					case DbmMode.Fix:
						log=Lans.g("FormDatabaseMaintenance","Notes with too much white space fixed:")+" "+countTotal.ToString()+"\r\n";
						break;
				}
			}
			return log;
		}

		[DbmMethodAttr]
		public static string TablesWithClinicNumNegative(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			List<string> listTablesToCheck=new List<string>();
			//"alertitem",//Not including this because we intentionally set ClinicNum to -1 to indicate the alert's for all clinics.
			listTablesToCheck.Add("appointment");
			listTablesToCheck.Add("claim");
			listTablesToCheck.Add("claimpayment");
			listTablesToCheck.Add("claimproc");
			listTablesToCheck.Add("histappointment");
			listTablesToCheck.Add("patient");
			listTablesToCheck.Add("procedurelog");
			listTablesToCheck.Add("smstomobile");
			string command="SELECT CountInvalid,TableName FROM (\r\n"+string.Join("\r\nUNION ALL\r\n",listTablesToCheck.Select(x => 
					$"SELECT COUNT(*) CountInvalid,'{x}' TableName FROM {x} WHERE ClinicNum < 0"))+"\r\n"+
				") invalid WHERE CountInvalid > 0";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Total rows found with invalid ClinicNums: ")+table.Select().Sum(x => PIn.Long(x["CountInvalid"].ToString()))
							+"\r\n  "+string.Join("\r\n  ",table.Select().Select(x => PIn.String(x["TableName"].ToString())+": "+PIn.String(x["CountInvalid"].ToString())));
					}
					break;
				case DbmMode.Fix:
					if(table.Rows.Count > 0) {
						command=string.Join(";\r\n",listTablesToCheck.Select(x => $"UPDATE {x} SET ClinicNum=0 WHERE ClinicNum < 0"));
						Db.NonQ(command);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Total rows fixed with invalid ClinicNums: ")+table.Select().Sum(x => PIn.Long(x["CountInvalid"].ToString()))
							+"\r\n  "+string.Join("\r\n  ",table.Select().Select(x => PIn.String(x["TableName"].ToString())+": "+PIn.String(x["CountInvalid"].ToString())));
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true)]
		public static string TransactionsWithFutureDates(bool verbose,DbmMode dbmMode) { 
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			bool isFutureTransAllowed=PrefC.GetBool(PrefName.FutureTransDatesAllowed);
			bool isFuturePaymentsAllowed=PrefC.GetBool(PrefName.AccountAllowFutureDebits);
			if(isFutureTransAllowed) {//future dates are allowed so this DBM doesn't apply.
				return log;
			}
			string command=Ledgers.GetTransQueryString(DateTime.Today,"");
			DataTable table=Db.GetTable(command);
			DataTable tableFlaggedTranss=table.Clone();
			for(int i=0;i<table.Rows.Count;i++){
				if(PIn.String(table.Rows[i]["TranType"].ToString())=="PPCharge"){
					continue;
				}
				if(PIn.String(table.Rows[i]["TranType"].ToString())=="PPCComplete"){
					continue;
				}
				if(PIn.Date(table.Rows[i]["TranDate"].ToString()) <= DateTime.Today.Date){//transaction is date for the future
					continue;
				}
				//if either future dated payments or future transactions are allowed, don't count transactions dealing with payments.
				if(PIn.String(table.Rows[i]["TranType"].ToString())=="PatPay") {
					if(isFuturePaymentsAllowed){
						continue; //they are allowing future payments so skip this row. 
					}
				}
				tableFlaggedTranss.Rows.Add(table.Rows[i].ItemArray);
			}
			log+=Lans.g("FormDatabaseMaintenance","Future dated transactions found:")+" "+tableFlaggedTranss.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(tableFlaggedTranss.Rows.Count > 0 || verbose) {
						log+="\r\n"+Lans.g("FormDatabaseMaintenance","Manual fix needed. Double click to see a break down.");
					}
					break;
				case DbmMode.Breakdown:
					if(tableFlaggedTranss.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i=0;i<tableFlaggedTranss.Rows.Count;i++){
							string tranType="";
							switch(PIn.String(tableFlaggedTranss.Rows[i]["TranType"].ToString())) {
								case "Adj":
									tranType=Lans.g("FormDatabaseMaintenance","Adjustment");
									break;
								case "Claimproc":
									tranType=Lans.g("FormDatabaseMaintenance","Claim procedure");
									break;
								case "PatPay":
									tranType=Lans.g("FormDatabaseMaintenance","Patient payment");//paysplit?
									break;
								case "Proc":
									tranType=Lans.g("FormDatabaseMaintenance","Procedure");
									break;
							}
							log+="\r\n   "+tranType+" "+Lans.g("FormDatabaseMaintenance","found for patient #")+PIn.Long(tableFlaggedTranss.Rows[i]["PatNum"].ToString())+" "
								+Lans.g("FormDatabaseMaintenance", "dated")+" "+PIn.Date(tableFlaggedTranss.Rows[i]["TranDate"].ToString()).ToShortDateString()+" "
								+Lans.g("FormDatabaseMaintenance","amounting to")+" "+PIn.Double(tableFlaggedTranss.Rows[i]["TranAmount"].ToString()).ToString("c");
						}
						log+="\r\n"+Lans.g("FormDatabaseMaintenance","Go to patient accounts to find and manually correct future dates.");
					}
					break;
			}
			return log;
		}

		#endregion Methods That Affect All or Many Tables---------------------------------------------------------------------------------------------------
		#region Methods That Apply to Specific Tables-------------------------------------------------------------------------------------------------------

		#region Appointment-----------------------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string AppointmentCompleteWithTpAttached(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT DISTINCT appointment.PatNum, "+DbHelper.Concat("LName","\", \"","FName")+" AS PatName, AptDateTime "
				+"FROM appointment "
				+"INNER JOIN patient ON patient.PatNum=appointment.PatNum "
				+"INNER JOIN procedurelog ON procedurelog.AptNum=appointment.AptNum "
				+"WHERE AptStatus="+POut.Int((int)ApptStatus.Complete)+" "
				+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.TP)+" "
				+"ORDER BY PatName";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			//There is something to report OR the user has verbose mode on.
			string log=Lans.g("FormDatabaseMaintenance","Completed appointments with treatment planned procedures attached")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   "+table.Rows[i]["PatNum"].ToString()
								+"-"+table.Rows[i]["PatName"].ToString()
								+"  Appt Date:"+PIn.DateT(table.Rows[i]["AptDateTime"].ToString()).ToShortDateString();
							log+="\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string AppointmentsEndingOnDifferentDay(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//This pulls appointments that extend past the end of the day. CHAR_LENGTH(Pattern)-1 as if the appointment goes to midnight, it does
			//not need to be fixed.
			string command="SELECT AptNum FROM appointment WHERE DATE(AptDateTime) != DATE(AptDateTime + INTERVAL (CHAR_LENGTH(Pattern)-1)*5 MINUTE)";
			List<long> listAptNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listAptNums.Count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments found that span over multiple days: ")+listAptNums.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listAptNums.Count > 0) {
						//We're going to truncate the appointment to end at midnight. We will grab the substring of the pattern by calculating the
						//number of 5 minute increments between the aptDateTime and midnight of the next day
						command="UPDATE appointment SET Pattern = SUBSTRING(Pattern,1,TIMESTAMPDIFF(MINUTE,AptDateTime,DATE(AptDateTime) "
							+"+ INTERVAL 1 DAY)/5) WHERE AptNum IN("+string.Join(",",listAptNums)+")";
						Db.NonQ(command);
					}
					int numberFixed=listAptNums.Count;
					if(numberFixed!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments shortened")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AppointmentsNoPattern(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT AptNum FROM appointment WHERE Pattern=''";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments found with zero length: ")+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(table.Rows.Count>0) {
						//detach all procedures
						List<Procedure> listProceduresModified;
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						string methodName=MethodBase.GetCurrentMethod().Name;
						string where="WHERE P.AptNum=A.AptNum AND A.Pattern=''";
						command="SELECT P.* FROM procedurelog P,appointment A "+where;
						listProceduresModified=Crud.ProcedureCrud.SelectMany(command);
						command="UPDATE procedurelog P, appointment A SET P.AptNum=0 "+where;
						Db.NonQ(command);
						//Add changes to listDbmLogs
						for(int i=0;i<listProceduresModified.Count;i++){
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=listProceduresModified[i].ProcNum;
							dbmLog.FKeyType=DbmLogFKeyType.Procedure;
							dbmLog.ActionType=DbmLogActionType.Update;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Updated AptNum from "+listProceduresModified[i].AptNum+" to 0 from AppointmentsNoPattern";
							listDbmLogs.Add(dbmLog);
						}
						where="WHERE P.PlannedAptNum=A.AptNum AND A.Pattern=''";
						command="SELECT P.* FROM procedurelog P,appointment A "+where;
						listProceduresModified=Crud.ProcedureCrud.SelectMany(command);
						command="UPDATE procedurelog P, appointment A SET P.PlannedAptNum=0 "+where;
						Db.NonQ(command);
						//Add changes to listDbmLogs
						for(int i=0;i<listProceduresModified.Count;i++){
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=listProceduresModified[i].ProcNum;
							dbmLog.FKeyType=DbmLogFKeyType.Procedure;
							dbmLog.ActionType=DbmLogActionType.Update;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Updated PlannedAptNum from "+listProceduresModified[i].PlannedAptNum+" to 0 from AppointmentsNoPattern";
							listDbmLogs.Add(dbmLog);
						}
						command="SELECT appointment.AptNum FROM appointment WHERE Pattern=''";
						DataTable tableAptNums=Db.GetTable(command);
						List<long> listAptNums=new List<long>();
						for(int i = 0;i<tableAptNums.Rows.Count;i++) {
							listAptNums.Add(PIn.Long(tableAptNums.Rows[i]["AptNum"].ToString()));
						}
						if(listAptNums.Count>0) {
							CrudAuditPerm crudAuditPerm=CrudTableAttribute.GetCrudAuditPermForClass(typeof(Appointment));
							List<EnumPermType> listEnumPermTypes=GroupPermissions.GetPermsFromCrudAuditPerm(crudAuditPerm);
							List<SecurityLog> listSecurityLogs=SecurityLogs.GetFromFKeysAndType(listAptNums,listEnumPermTypes);
							Appointments.ClearFkey(listAptNums);//Zero securitylog FKey column for rows to be deleted.
							//Add securitylog changes to listDbmLogs
							for(int i=0;i<listSecurityLogs.Count;i++){
								DbmLog dbmLog=new DbmLog();
								dbmLog.UserNum=Security.CurUser.UserNum;
								dbmLog.FKey=listSecurityLogs[i].SecurityLogNum;
								dbmLog.FKeyType=DbmLogFKeyType.Securitylog;
								dbmLog.ActionType=DbmLogActionType.Delete;
								dbmLog.MethodName=methodName;
								dbmLog.LogText="Updated FKey from "+listSecurityLogs[i].FKey+" to 0.";
								listDbmLogs.Add(dbmLog);
							}
							for(int i=0;i<listAptNums.Count;i++){
								DbmLog dbmLog=new DbmLog();
								dbmLog.UserNum=Security.CurUser.UserNum;
								dbmLog.FKey=listAptNums[i];
								dbmLog.FKeyType=DbmLogFKeyType.Appointment;
								dbmLog.ActionType=DbmLogActionType.Delete;
								dbmLog.MethodName=methodName;
								dbmLog.LogText="Deleted appointment with no pattern";
								listDbmLogs.Add(dbmLog);
							}
							command="SELECT * FROM appointment WHERE AptNum IN("+String.Join(",",listAptNums)+")";
							List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
							for(int i=0;i<listAppointments.Count;i++){
								HistAppointment histAppointment=HistAppointments.CreateHistoryEntry(listAppointments[i],HistAppointmentAction.Deleted);
								if(histAppointment==null) {
									continue;
								}
								DbmLog dbmLog=new DbmLog();
								dbmLog.UserNum=Security.CurUser.UserNum;
								dbmLog.FKey=histAppointment.HistApptNum;
								dbmLog.FKeyType=DbmLogFKeyType.HistAppointment;
								dbmLog.ActionType=DbmLogActionType.Insert;
								dbmLog.MethodName=methodName;
								dbmLog.LogText="Inserted hist appointment.";
								listDbmLogs.Add(dbmLog);
							}
						}
						command="DELETE FROM appointment WHERE Pattern=''";
						Db.NonQ(command);
						DbmLogs.InsertMany(listDbmLogs);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments deleted with zero length")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AppointmentsNoDateOrProcs(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM appointment "
						+"WHERE AptStatus=1 "//scheduled 
						+"AND "+DbHelper.Year("AptDateTime")+"<1880 "//scheduled but no date 
						+"AND NOT EXISTS(SELECT * FROM procedurelog WHERE procedurelog.AptNum=appointment.AptNum)";//and no procs
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments found with no date and no procs")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT appointment.AptNum FROM appointment "
						+"WHERE AptStatus="+POut.Int((int)ApptStatus.Scheduled)+" "
						+"AND "+DbHelper.Year("AptDateTime")+"<1880 "//scheduled but no date 
						+"AND NOT EXISTS(SELECT * FROM procedurelog WHERE procedurelog.AptNum=appointment.AptNum)";//and no procs
					DataTable tableAptNums=Db.GetTable(command);
					List<long> listAptNums=new List<long>();
					for(int i = 0;i<tableAptNums.Rows.Count;i++) {
						listAptNums.Add(PIn.Long(tableAptNums.Rows[i]["AptNum"].ToString()));
					}
					if(listAptNums.Count>0) {
						List<EnumPermType> listEnumPermTypes=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeof(Appointment)));
						List<SecurityLog> listSecurityLogs=SecurityLogs.GetFromFKeysAndType(listAptNums,listEnumPermTypes);
						Appointments.ClearFkey(listAptNums);//Zero securitylog FKey column for rows to be deleted.
						//Add changes to listDbmLogs
						for(int i=0;i<listAptNums.Count;i++){
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=listAptNums[i];
							dbmLog.FKeyType=DbmLogFKeyType.Appointment;
							dbmLog.ActionType=DbmLogActionType.Delete;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Deleted appointment from AppointmentsNoDateOrProcs.";
							listDbmLogs.Add(dbmLog);
						}
						for(int i=0;i<listSecurityLogs.Count;i++){
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=listSecurityLogs[i].SecurityLogNum;
							dbmLog.FKeyType=DbmLogFKeyType.Securitylog;
							dbmLog.ActionType=DbmLogActionType.Update;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Cleared securitylog FKey column.";
							listDbmLogs.Add(dbmLog);
						}
						command="SELECT * FROM appointment WHERE AptNum IN("+String.Join(",",listAptNums)+")";
						List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
						for(int i=0;i<listAppointments.Count;i++){
							HistAppointment histAppointment=HistAppointments.CreateHistoryEntry(listAppointments[i],HistAppointmentAction.Deleted);
							if(histAppointment==null) {
								continue;
							}
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=histAppointment.HistApptNum;
							dbmLog.FKeyType=DbmLogFKeyType.HistAppointment;
							dbmLog.ActionType=DbmLogActionType.Insert;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Inserted hist appointment.";
							listDbmLogs.Add(dbmLog);
						}
					}
					command="DELETE FROM appointment "
						+"WHERE AptStatus="+POut.Int((int)ApptStatus.Scheduled)+" "
						+"AND "+DbHelper.Year("AptDateTime")+"<1880 "//scheduled but no date 
						+"AND NOT EXISTS(SELECT * FROM procedurelog WHERE procedurelog.AptNum=appointment.AptNum)";//and no procs
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed!=0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Appointments deleted due to no date and no procs")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AppointmentPlannedDuplicateItemOrderPlanned(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(DISTINCT a.PatNum) AS COUNT FROM (SELECT PatNum, COUNT(*) FROM appointment WHERE AptStatus="+POut.Int((int)ApptStatus.Planned)+" GROUP BY PatNum, ItemOrderPlanned HAVING COUNT(*)>1) a";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments with status set to planned without Planned Appointment: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT PatNum FROM appointment WHERE AptStatus="+POut.Int((int)ApptStatus.Planned)+" GROUP BY PatNum, ItemOrderPlanned HAVING COUNT(*)>1";
					List<long> listPatNum=Db.GetListLong(command);
					if(listPatNum.Count>0 || verbose) {
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						string methodName=MethodBase.GetCurrentMethod().Name;
						for(int i=0;i<listPatNum.Count;i++) {
							List<Appointment> listPlannedAppointments=Appointments.GetRefreshedPlannedAppts(listPatNum[i]);
							for(int j = 0;j<listPlannedAppointments.Count;j++) {
								if(j+1!=listPlannedAppointments[j].ItemOrderPlanned) {
									command="UPDATE appointment SET ItemOrderPlanned="+POut.Long(j+1)
										+" WHERE AptNum="+POut.Long(listPlannedAppointments[j].AptNum);
									Db.NonQ(command);
									DbmLog dbmLog=new DbmLog();
									dbmLog.UserNum=Security.CurUser.UserNum;
									dbmLog.FKey=listPlannedAppointments[j].AptNum;
									dbmLog.FKeyType=DbmLogFKeyType.Appointment;
									dbmLog.ActionType=DbmLogActionType.Insert;
									dbmLog.MethodName=methodName;
									dbmLog.LogText="Updated Appointment.ItemOrderPlanned from"+listPlannedAppointments[j].ItemOrderPlanned+" to "+POut.Long(j+1);
									listDbmLogs.Add(dbmLog);
								}
							}
						}
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Appointments updated with duplicate ItemOrderPlanned values: ")+
							listPatNum.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string AppointmentScheduledWithCompletedProcs(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT DISTINCT appointment.PatNum, "+DbHelper.Concat("LName","\', \'","FName")+" AS PatName, appointment.AptDateTime "
				+"FROM appointment "
				+"INNER JOIN patient ON patient.PatNum=appointment.PatNum "
				+"INNER JOIN procedurelog ON procedurelog.AptNum=appointment.AptNum "
				+"WHERE AptStatus = "+POut.Int((int)ApptStatus.Scheduled)+" "
				+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"ORDER BY PatName";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			//There is something to report OR the user has verbose mode on.
			string log=Lans.g("FormDatabaseMaintenance","Scheduled appointments with completed procedures attached")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   "+table.Rows[i]["PatNum"].ToString()
								+"-"+table.Rows[i]["PatName"].ToString()
								+"  Appt Date:"+PIn.DateT(table.Rows[i]["AptDateTime"].ToString()).ToShortDateString();
							log+="\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion Appointment--------------------------------------------------------------------------------------------------------------------------
		#region AuditTrail, AutoCode, Automation--------------------------------------------------------------------------------------------------------

		///<summary>For appointments that have more than one AppointmentCreate audit entry, deletes all but the newest.</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AuditTrailDeleteDuplicateApptCreate(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT securitylog.* "
				+"FROM securitylog "
				+"INNER JOIN ("
					+"SELECT PatNum,FKey,MAX(LogDateTime) LogDateTime "
					+"FROM securitylog "
					+"WHERE PermType="+POut.Int((int)EnumPermType.AppointmentCreate)+" "
					+"AND FKey>0 "
					+"GROUP BY PatNum,FKey "
					+"HAVING COUNT(*)>1"
				+") sl ON sl.PatNum=securitylog.PatNum "
				+"AND sl.FKey=securitylog.FKey "
				+"AND sl.LogDateTime!=securitylog.LogDateTime "
				+"AND securitylog.PermType="+POut.Int((int)EnumPermType.AppointmentCreate)+" "
				+"GROUP BY securitylog.PatNum,securitylog.FKey";
			List<SecurityLog> listSecurityLogs=Crud.SecurityLogCrud.SelectMany(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listSecurityLogs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Appointments found with duplicate Appt Create audit trail entries:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listSecurityLogs.Count>0) {
						string methodName=MethodBase.GetCurrentMethod().Name;
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						command="DELETE FROM securitylog WHERE SecurityLogNum IN("+string.Join(",",listSecurityLogs.Select(x => x.SecurityLogNum))+")";
						int numberFixed=(int)Db.NonQ(command);
						for(int i=0;i<listSecurityLogs.Count;i++){
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=listSecurityLogs[i].SecurityLogNum;
							dbmLog.FKeyType=DbmLogFKeyType.Securitylog;
							dbmLog.ActionType=DbmLogActionType.Delete;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Audit trail entry deleted from AuditTrailDeleteDuplicateApptCreate.";
							listDbmLogs.Add(dbmLog);
						}
						if(numberFixed>0 || verbose) {
							Crud.DbmLogCrud.InsertMany(listDbmLogs);
							log+=Lans.g("FormDatabaseMaintenance","Audit trail entries deleted due to duplicate Appt Create entries:")+" "
								+numberFixed.ToString()+"\r\n";
						}
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AutoCodeItemsWithNoAutoCode(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			DataTable table;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT DISTINCT AutoCodeNum FROM autocodeitem WHERE NOT EXISTS(
						SELECT * FROM autocode WHERE autocodeitem.AutoCodeNum=autocode.AutoCodeNum)";
					table=Db.GetTable(command);
					int numFound=table.Rows.Count;
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Auto codes missing due to invalid auto code items")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command=@"SELECT DISTINCT AutoCodeNum FROM autocodeitem WHERE NOT EXISTS(
						SELECT * FROM autocode WHERE autocodeitem.AutoCodeNum=autocode.AutoCodeNum)";
					table=Db.GetTable(command);
					int numFixed=table.Rows.Count;
					for(int i = 0;i<table.Rows.Count;i++) {
						AutoCode autoCode=new AutoCode();
						autoCode.AutoCodeNum=PIn.Long(table.Rows[i]["AutoCodeNum"].ToString());
						autoCode.Description="UNKNOWN";
						Crud.AutoCodeCrud.Insert(autoCode,true);
						DbmLog dbmLog=new DbmLog();
						dbmLog.UserNum=Security.CurUser.UserNum;
						dbmLog.FKey=autoCode.AutoCodeNum;
						dbmLog.FKeyType=DbmLogFKeyType.AutoCode;
						dbmLog.ActionType=DbmLogActionType.Insert;
						dbmLog.MethodName=methodName;
						dbmLog.LogText="Added a new AutoCode from AutoCodeItemsWithNoAutoCode";
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0) {
						Signalods.SetInvalid(InvalidType.AutoCodes);
					}
					if(numFixed!=0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Auto codes created due to invalid auto code items")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AutoCodesDeleteWithNoItems(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM autocode WHERE NOT EXISTS(
						SELECT * FROM autocodeitem WHERE autocodeitem.AutoCodeNum=autocode.AutoCodeNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Autocodes found with no items: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"SELECT * FROM autocode WHERE NOT EXISTS(SELECT * FROM autocodeitem WHERE autocodeitem.AutoCodeNum=autocode.AutoCodeNum)";
					List<AutoCode> listAutoCodes=Crud.AutoCodeCrud.SelectMany(command);
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command=@"DELETE FROM autocode WHERE NOT EXISTS(
						SELECT * FROM autocodeitem WHERE autocodeitem.AutoCodeNum=autocode.AutoCodeNum)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listAutoCodes.Count;i++){
						DbmLog dbmLog=new DbmLog();
						dbmLog.UserNum=Security.CurUser.UserNum;
						dbmLog.FKey=listAutoCodes[i].AutoCodeNum;
						dbmLog.FKeyType=DbmLogFKeyType.AutoCode;
						dbmLog.ActionType=DbmLogActionType.Delete;
						dbmLog.MethodName=methodName;
						dbmLog.LogText="Deleted AutoCode:"+listAutoCodes[i].Description+" from AutoCodesDeleteWithNoItems";
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0) {
						Signalods.SetInvalid(InvalidType.AutoCodes);
					}
					if(numberFixed!=0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Autocodes deleted due to no items: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string AutomationTriggersWithNoSheetDefs(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM automation WHERE automation.SheetDefNum!=0 AND NOT EXISTS(
					SELECT SheetDefNum FROM sheetdef WHERE automation.SheetDefNum=sheetdef.SheetDefNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Automation triggers found with no sheet defs: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command=@"SELECT * FROM automation WHERE automation.SheetDefNum!=0 AND NOT EXISTS(
					SELECT SheetDefNum FROM sheetdef WHERE automation.SheetDefNum=sheetdef.SheetDefNum)";
					List<Automation> listAutomations=Crud.AutomationCrud.SelectMany(command);
					command=@"DELETE FROM automation WHERE automation.SheetDefNum!=0 AND NOT EXISTS(
					SELECT SheetDefNum FROM sheetdef WHERE automation.SheetDefNum=sheetdef.SheetDefNum)";
					int numberFixed=(int)Db.NonQ(command);
					//Add to listDbmlogs
					for(int i=0;i<listAutomations.Count;i++){
						DbmLog dbmLog=new DbmLog();
						dbmLog.UserNum=Security.CurUser.UserNum;
						dbmLog.FKey=listAutomations[i].AutomationNum;
						dbmLog.FKeyType=DbmLogFKeyType.Automation;
						dbmLog.ActionType=DbmLogActionType.Delete;
						dbmLog.MethodName=methodName;
						dbmLog.LogText="Deleted automation from AutomationTriggersWithNoSheetDefs.";
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed!=0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Automation triggers deleted due to no sheet defs: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion AuditTrail, AutoCode, Automation-----------------------------------------------------------------------------------------------------
		#region Benefit, BillingType--------------------------------------------------------------------------------------------------------------------

		///<summary>Remove duplicates where all benefit columns match except for BenefitNum.</summary>
		[DbmMethodAttr]
		public static string BenefitsWithExactDuplicatesForInsPlan(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			DataTable table;
			//Non-limitation benefits cannot be created with a TreatmentArea other than None, so the TreatArea column is irrelevant for non-frequency benefits and should not affect the results
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) DuplicateCount FROM "  // Do a sub-select to get the count - for some reason it's much faster
						+"(SELECT DISTINCT ben2.BenefitNum FROM "
						+"(SELECT BenefitNum, PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea, COUNT(*) FROM "
						+"(SELECT BenefitNum, PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea "
						+"FROM benefit ORDER BY BenefitNum ASC) ben3 "
						+"GROUP BY PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea "
						+"HAVING COUNT(*) > 1) ben "
						+"INNER JOIN (SELECT BenefitNum, PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea FROM benefit) ben2 "  // select the specific columns we want - selecting the whole table takes 4x longer for some reason
						+"ON ben.PlanNum=ben2.PlanNum "
						+"AND ben.PatPlanNum=ben2.PatPlanNum "
						+"AND ben.CovCatNum=ben2.CovCatNum "
						+"AND ben.BenefitType=ben2.BenefitType "
						+"AND ben.Percent=ben2.Percent "
						+"AND ben.MonetaryAmt=ben2.MonetaryAmt "
						+"AND ben.TimePeriod=ben2.TimePeriod "
						+"AND ben.QuantityQualifier=ben2.QuantityQualifier "
						+"AND ben.Quantity=ben2.Quantity "
						+"AND ben.CodeNum=ben2.CodeNum "
						+"AND ben.CoverageLevel=ben2.CoverageLevel "
						+"AND ben.CodeGroupNum=ben2.CodeGroupNum "
						+"AND ben.TreatArea=ben2.TreatArea "
						+"AND ben.BenefitNum!=ben2.BenefitNum) ben4";  //This ensures that the benefit with the lowest primary key in the match will not be counted.
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Duplicate benefits found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT DISTINCT ben2.BenefitNum FROM "
						+"(SELECT BenefitNum, PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea, COUNT(*) "
						+"FROM (SELECT BenefitNum, PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea FROM benefit "
						+"ORDER BY BenefitNum ASC) ben3 "
						+"GROUP BY PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea "
						+"HAVING COUNT(*) > 1) ben "
						+"INNER JOIN (SELECT BenefitNum, PlanNum, PatPlanNum, CovCatNum, BenefitType, Percent, MonetaryAmt, TimePeriod, QuantityQualifier, Quantity, CodeNum, CoverageLevel, CodeGroupNum, TreatArea FROM benefit) ben2 "
						+"ON ben.PlanNum=ben2.PlanNum "
						+"AND ben.PatPlanNum=ben2.PatPlanNum "
						+"AND ben.CovCatNum=ben2.CovCatNum "
						+"AND ben.BenefitType=ben2.BenefitType "
						+"AND ben.Percent=ben2.Percent "
						+"AND ben.MonetaryAmt=ben2.MonetaryAmt "
						+"AND ben.TimePeriod=ben2.TimePeriod "
						+"AND ben.QuantityQualifier=ben2.QuantityQualifier "
						+"AND ben.Quantity=ben2.Quantity "
						+"AND ben.CodeNum=ben2.CodeNum "
						+"AND ben.CoverageLevel=ben2.CoverageLevel "
						+"AND ben.CodeGroupNum=ben2.CodeGroupNum "
						+"AND ben.TreatArea=ben2.TreatArea "
						+"AND ben.BenefitNum!=ben2.BenefitNum";  //This ensures that the benefit with the lowest primary key in the match will not be deleted.
					table=Db.GetTable(command);
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<DbmLog> listDbmlogs=new List<DbmLog>();
					List<long> listBenefitNums=new List<long>();
					if(table.Rows.Count>0 || verbose) {
						for(int i = 0;i<table.Rows.Count;i++) {
							long benefitNum=PIn.Long(table.Rows[i]["BenefitNum"].ToString());
							listBenefitNums.Add(benefitNum);
							DbmLog dbmLog=new DbmLog();
							dbmLog.UserNum=Security.CurUser.UserNum;
							dbmLog.FKey=benefitNum;
							dbmLog.FKeyType=DbmLogFKeyType.Benefit;
							dbmLog.ActionType=DbmLogActionType.Delete;
							dbmLog.MethodName=methodName;
							dbmLog.LogText="Deleted duplicate benefit.";
							listDbmlogs.Add(dbmLog);
						}
						int numFixed=0;
						if(listBenefitNums.Count>0) {
							command="DELETE FROM benefit WHERE BenefitNum IN ("+string.Join(",",listBenefitNums)+")";
							numFixed=(int)Db.NonQ(command);
							Crud.DbmLogCrud.InsertMany(listDbmlogs);
						}
						log+=Lans.g("FormDatabaseMaintenance","Duplicate benefits deleted")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Identify duplicates where all benefit columns match except for BenefitNum, Percent, and MonetaryAmt.</summary>
		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string BenefitsWithPartialDuplicatesForInsPlan(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT DISTINCT employer.EmpName,carrier.CarrierName,carrier.Phone,carrier.Address,carrier.City,carrier.State,carrier.Zip, "
				+"insplan.GroupNum,insplan.GroupName, carrier.NoSendElect,carrier.ElectID, "
				+"(SELECT COUNT(DISTINCT Subscriber) FROM inssub WHERE insplan.PlanNum=inssub.PlanNum) subscribers, insplan.PlanNum "
				+"FROM benefit ben "
				+"INNER JOIN benefit ben2 ON ben.PlanNum=ben2.PlanNum "
					+"AND ben.PatPlanNum=ben2.PatPlanNum "
					+"AND ben.CovCatNum=ben2.CovCatNum "
					+"AND ben.BenefitType=ben2.BenefitType "
					+"AND (ben.Percent!=ben2.Percent OR ben.MonetaryAmt!=ben2.MonetaryAmt) "  //Only benefits with Percent or MonetaryAmts that don't match.
					+"AND ben.TimePeriod=ben2.TimePeriod "
					+"AND ben.QuantityQualifier=ben2.QuantityQualifier "
					+"AND ben.Quantity=ben2.Quantity "
					+"AND ben.CodeNum=ben2.CodeNum "
					+"AND ben.CodeGroupNum=ben2.CodeGroupNum "
					+"AND ben.CoverageLevel=ben2.CoverageLevel "
					+"AND ben.BenefitNum<ben2.BenefitNum "
				+"INNER JOIN insplan ON insplan.PlanNum=ben.PlanNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum "
				+"LEFT JOIN employer ON employer.EmployerNum=insplan.EmployerNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			//There is something to report OR the user has verbose mode on.
			string log=Lans.g("FormDatabaseMaintenance","Insurance plans with partial duplicate benefits found")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							//Show the same columns as the Insurance Plans list.  We don't have an easy identifier for insurance plans, and we do not want to 
							//  give a patient example since there is a good chance that in fixing the benefits the user will just split that plan off and will 
							//  not solve the issue.
							log+="   Employer: "+table.Rows[i]["EmpName"].ToString();
							log+=",  Carrier: "+table.Rows[i]["CarrierName"].ToString();
							log+=",  Phone: "+table.Rows[i]["Phone"].ToString();
							log+=",  Address: "+table.Rows[i]["Address"].ToString();
							log+=",  City: "+table.Rows[i]["City"].ToString();
							log+=",  ST: "+table.Rows[i]["State"].ToString();
							log+=",  Zip: "+table.Rows[i]["Zip"].ToString();
							log+=",  Group#: "+table.Rows[i]["GroupNum"].ToString();
							log+=",  GroupName: "+table.Rows[i]["GroupName"].ToString();
							log+=",  NoE: ";
							if(table.Rows[i]["NoSendElect"].ToString()=="1") {
								log+="X";
							}
							else {
								log+=" ";
							}
							log+=",  ElectID: "+table.Rows[i]["ElectID"].ToString();
							log+=",  Subs: "+table.Rows[i]["subscribers"].ToString();
							log+="\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string BillingTypesInvalid(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT ValueString FROM preference WHERE PrefName='PracticeDefaultBillType'";
			int billingType=PIn.Int(Db.GetScalar(command));
			command="SELECT COUNT(*) FROM definition WHERE Category=4 AND definition.DefNum="+billingType;
			int prefExists=PIn.Int(Db.GetCount(command));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(prefExists!=1) {
						log+=Lans.g("FormDatabaseMaintenance","No default billing type set.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Default practice billing type verified.")+"\r\n";
					}
					//Check for any patients with invalid billingtype.
					command="SELECT COUNT(*) FROM patient WHERE NOT EXISTS(SELECT * FROM definition WHERE Category=4 AND patient.BillingType=definition.DefNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with invalid billing type: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					//Fix for default billingtype not being set.
					if(prefExists!=1) {//invalid billing type
						command="SELECT DefNum FROM definition WHERE Category=4 AND IsHidden=0 ORDER BY ItemOrder";
						DataTable table=Db.GetTable(command);
						if(table.Rows.Count==0) {//if all billing types are hidden
							command="SELECT DefNum FROM definition WHERE Category=4 ORDER BY ItemOrder";
							table=Db.GetTable(command);
						}
						command="UPDATE preference SET ValueString='"+table.Rows[0][0].ToString()+"' WHERE PrefName='PracticeDefaultBillType'";
						Db.NonQ(command);
						log+=Lans.g("FormDatabaseMaintenance","Default billing type preference was set due to being invalid.")+"\r\n";
						Prefs.RefreshCache();//for the next line.
					}
					//Fix for patients with invalid billingtype.
					command="UPDATE patient SET patient.BillingType="+POut.Long(PrefC.GetLong(PrefName.PracticeDefaultBillType));
					command+=" WHERE NOT EXISTS(SELECT * FROM definition WHERE Category=4 AND patient.BillingType=definition.DefNum)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients billing type set to default due to being invalid: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion Benefit, BillingType-----------------------------------------------------------------------------------------------------------------
		#region Carrier, Claim--------------------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsCanada=true,IsReplicationUnsafe=true)]
		public static string CanadaCarriersCdaMissingInfo(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				return Lans.g("FormDatabaseMaintenance","Skipped. Local computer region must be set to Canada to run.");
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='TELUS B' LIMIT 1";
			long canadianNetworkNumTelusB=PIn.Long(Db.GetScalar(command));
			command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='CSI' LIMIT 1";
			//CSI is now known as "instream"
			long canadianNetworkNumCSI=PIn.Long(Db.GetScalar(command));
			command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='CDCS' LIMIT 1";
			long canadianNetworkNumCDCS=PIn.Long(Db.GetScalar(command));
			command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='TELUS A' LIMIT 1";
			long canadianNetworkNumTelusA=PIn.Long(Db.GetScalar(command));
			command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='MBC' LIMIT 1";
			long canadianNetworkNumMBC=PIn.Long(Db.GetScalar(command));
			command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='ABC' LIMIT 1";
			long canadianNetworkNumABC=PIn.Long(Db.GetScalar(command));
			CanSupTransTypes canSupTransTypesClaim=CanSupTransTypes.ClaimAckEmbedded_11e|CanSupTransTypes.ClaimEobEmbedded_21e;//Claim 01, claim ack 11, and claim eob 21 are implied.
			CanSupTransTypes canSupTransTypesReversal=CanSupTransTypes.ClaimReversal_02|CanSupTransTypes.ClaimReversalResponse_12;
			CanSupTransTypes canSupTransTypesPredetermination=CanSupTransTypes.PredeterminationAck_13|CanSupTransTypes.PredeterminationAckEmbedded_13e|CanSupTransTypes.PredeterminationMultiPage_03|CanSupTransTypes.PredeterminationSinglePage_03;
			CanSupTransTypes canSupTransTypesRot=CanSupTransTypes.RequestForOutstandingTrans_04;
			CanSupTransTypes canSupTransTypesCob=CanSupTransTypes.CobClaimTransaction_07;
			CanSupTransTypes canSupTransTypesEligibility=CanSupTransTypes.EligibilityTransaction_08|CanSupTransTypes.EligibilityResponse_18;
			CanSupTransTypes CanSupTransTypesRpr=CanSupTransTypes.RequestForPaymentReconciliation_06;
			//Column order: ElectID,CanadianEncryptionMethod,CDAnetVersion,CanadianSupportedTypes,CanadianNetworkNum
			object[] arrayCarrierInfo=new object[] {
				//accerta
				"311140",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//adsc
				"000105",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob|canSupTransTypesEligibility,canadianNetworkNumCSI,
				//aga
				"610226",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//appq
				"628112",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//alberta blue cross. Usually sent through ClaimStream instead of ITRANS.
				"000090",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob,canadianNetworkNumABC,
				//assumption life
				"610191",1,"04",canSupTransTypesClaim,canadianNetworkNumTelusB,
				//autoben
				"628151",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//benecaid health benefit solutions
				"610708",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//benefits trust
				"610146",1,"02",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//beneplan - Old carrier that is no longer listed one iTrans supported list.
				"400008",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//boilermakers' national benefit plan - Old carrier that is no longer listed one iTrans supported list.
				"000116",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//canadian benefit providers
				"610202",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//capitale
				"600502",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob,canadianNetworkNumTelusB,
				//carpenters and allied workers local
				"000117",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//cdcs
				"610129",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumCDCS,
				//claimsecure
				"610099",1,"04",canSupTransTypesClaim|canSupTransTypesEligibility,canadianNetworkNumTelusB,
				//co-operators
				"606258",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//Commision de la construction du Quebec
				"000036",1,"02",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//coughlin & associates
				"610105",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot,canadianNetworkNumTelusB,
				//cowan wright beauchamps
				"610153",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//desjardins financial security
				"000051",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesRot,canadianNetworkNumTelusB,
				//empire life insurance company
				"000033",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//equitable life
				"000029",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//esorse corporation
				"610650",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|CanSupTransTypesRpr|canSupTransTypesCob,canadianNetworkNumTelusB,
				//fas administrators
				"610614",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//GMS Insurance Inc. (GMS) (ESC)
				"610218",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//great west life assurance company
				"000011",1,"04",canSupTransTypesClaim|canSupTransTypesCob|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//green sheild canada
				"000102",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//group medical services
				"610217",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//groupe premier medical
				"610266",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//grouphealth benefit solutions
				"000125",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//groupsource - Old carrier that is no longer listed one iTrans supported list.
				"605064",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesEligibility,canadianNetworkNumCSI,
				//Humania Assurance Inc (formerly La Survivance) (ESC)
				"000080",1,"04",canSupTransTypesClaim,canadianNetworkNumTelusB,
				//industrial alliance
				"000060",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusA,
				//industrial alliance pacific insurance and financial
				"000024",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusA,
				//johnson inc.
				"627265",1,"04",canSupTransTypesClaim,canadianNetworkNumTelusB,
				//johnston group
				"627223",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//lee-power & associates
				"627585",1,"02",canSupTransTypesClaim,canadianNetworkNumTelusA,
				//local 1030 health benefity plan
				"000118",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//manion wilkins
				"610158",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//manitoba blue cross
				"000094",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot,canadianNetworkNumMBC,
				//manitoba cleft palate program
				"000114",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination|canSupTransTypesRot,canadianNetworkNumCSI,
				//manitoba health
				"000113",1,"04",canSupTransTypesClaim|canSupTransTypesRot,canadianNetworkNumCSI,
				//telus adjudicare
				"000034",1,"04",canSupTransTypesClaim|canSupTransTypesCob|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesEligibility,canadianNetworkNumTelusB,
				//manulife financial
				"610059",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//maritime life assurance company
				"311113",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//maritime pro
				"610070",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//mdm
				"601052",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesEligibility,canadianNetworkNumTelusB,
				//medavie blue cross
				"610047",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//nexgenrx
				"610634",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//Non-Insured Health Benefits (NIHB) Program (ESC)
				"610124",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//nova scotia community services
				"000109",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob|canSupTransTypesEligibility,canadianNetworkNumCSI,
				//nova scotia medical services insurance
				"000108",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob|canSupTransTypesEligibility,canadianNetworkNumCSI,
				//nunatsiavut government department of health
				"610172",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumCSI,
				//ontario ironworkers
				"000123",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumCSI,
				//pacific blue cross
				"000064",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumCSI,
				//pbas
				"610256",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination|canSupTransTypesRot,canadianNetworkNumCSI,
				//quickcard
				"000103",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob|canSupTransTypesEligibility,canadianNetworkNumCSI,
				//rbc insurance
				"000124",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot,canadianNetworkNumTelusB,
				//rwam insurance
				"610616",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//saskatchewan blue cross
				"000096",1,"04",canSupTransTypesClaim,canadianNetworkNumTelusB,
				//Segic (BATCH) benefits
				"610360",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesCob,canadianNetworkNumTelusB,
				//ses benefits
				"610196",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//sheet metal workers local 30 benefit plan - Old carrier that is no longer listed one iTrans supported list.
				"000119",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//ssq societe d'assurance-vie inc.
				"000079",1,"04",canSupTransTypesClaim,canadianNetworkNumTelusB,
				//standard life assurance company
				"000020",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//sun life of canada
				"000016",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination|canSupTransTypesRot|canSupTransTypesCob,canadianNetworkNumTelusB,
				//syndicat des fonctionnaires municipaux mtl
				"610677",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//TELUS HS Assure test carrier (V4)
				"000010",1,"04",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
				//the building union of canada health beneift plan
				"000120",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//U-L Mutual (ESC)
				"610643",1,"04",canSupTransTypesClaim|canSupTransTypesReversal,canadianNetworkNumTelusB,
				//u.a. local 46 dental plan
				"000115",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//u.a. local 787 health trust fund dental plan - Old carrier that is no longer listed one iTrans supported list.
				"000110",1,"04",canSupTransTypesClaim|canSupTransTypesPredetermination,canadianNetworkNumCSI,
				//wawanesa
				"311109",1,"02",canSupTransTypesClaim|canSupTransTypesReversal|canSupTransTypesPredetermination,canadianNetworkNumTelusB,
			};
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=0;
					for(int i = 0;i<arrayCarrierInfo.Length;i+=5) {
						command="SELECT COUNT(*) "+
							"FROM carrier "+
							"WHERE IsCDA<>0 AND ElectID='"+POut.String((string)arrayCarrierInfo[i])+"' AND "+
							"(CanadianEncryptionMethod<>"+POut.Int((int)arrayCarrierInfo[i+1])+" OR "+
							"CDAnetVersion<>'"+POut.String((string)arrayCarrierInfo[i+2])+"' OR "+
							"CanadianSupportedTypes<>"+POut.Int((int)arrayCarrierInfo[i+3])+" OR "+
							"CanadianNetworkNum<>"+POut.Long((long)arrayCarrierInfo[i+4])+")";
						numFound+=PIn.Int(Db.GetCount(command));
					}
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","CDANet carriers with incorrect network, encryption method or version, based on carrier identification number: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i = 0;i<arrayCarrierInfo.Length;i+=5) {
						command="SELECT * "+
							"FROM carrier "+
							"WHERE IsCDA<>0 AND ElectID='"+POut.String((string)arrayCarrierInfo[i])+"' AND "+
							"(CanadianEncryptionMethod<>"+POut.Int((int)arrayCarrierInfo[i+1])+" OR "+
							"CDAnetVersion<>'"+POut.String((string)arrayCarrierInfo[i+2])+"' OR "+
							"CanadianSupportedTypes<>"+POut.Int((int)arrayCarrierInfo[i+3])+" OR "+
							"CanadianNetworkNum<>"+POut.Long((long)arrayCarrierInfo[i+4])+")";
						List<Carrier> listCarriers=Crud.CarrierCrud.SelectMany(command);
						command="UPDATE carrier SET "+
							"CanadianEncryptionMethod="+POut.Int((int)arrayCarrierInfo[i+1])+","+
							"CDAnetVersion='"+POut.String((string)arrayCarrierInfo[i+2])+"',"+
							"CanadianSupportedTypes="+POut.Int((int)arrayCarrierInfo[i+3])+","+
							"CanadianNetworkNum="+POut.Long((long)arrayCarrierInfo[i+4])+" "+
							"WHERE IsCDA<>0 AND ElectID='"+POut.String((string)arrayCarrierInfo[i])+"' AND "+
							"(CanadianEncryptionMethod<>"+POut.Int((int)arrayCarrierInfo[i+1])+" OR "+
							"CDAnetVersion<>'"+POut.String((string)arrayCarrierInfo[i+2])+"' OR "+
							"CanadianSupportedTypes<>"+POut.Int((int)arrayCarrierInfo[i+3])+" OR "+
							"CanadianNetworkNum<>"+POut.Long((long)arrayCarrierInfo[i+4])+")";
						int numUpdated=(int)Db.NonQ(command);
						numberFixed+=numUpdated;
						//add changes to dbmLogs
						if(numUpdated > 0) {
							for(int j=0;j<listCarriers.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listCarriers[j].CarrierNum,DbmLogFKeyType.Carrier,
									DbmLogActionType.Update,methodName,"Updated Carrier info for carrier: "+listCarriers[j].CarrierName);
								listDbmLogs.Add(dbmLog);
							}
						}
					}
					if(numberFixed!=0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","CDANet carriers fixed based on carrier identification number: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true,HasPatNum=true)]
		public static string ClaimDeleteWithNoClaimProcs(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			string command;
			string patWhere="";
			if(patNum>0) {
				patWhere=$"PatNum={POut.Long(patNum)} AND ";
			}
			switch(dbmMode) {
				case DbmMode.Check:
					command=$"SELECT COUNT(*) FROM claim " +
						$"WHERE {patWhere}NOT EXISTS(SELECT * FROM claimproc WHERE claim.ClaimNum=claimproc.ClaimNum AND IsOverPay=0)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims found with no claimprocs")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT claim.ClaimNum FROM claim "
						+$"WHERE {patWhere}NOT EXISTS(SELECT * FROM claimproc WHERE claim.ClaimNum=claimproc.ClaimNum AND IsOverPay=0)";
					DataTable tableClaimNums=Db.GetTable(command);
					List<long> listClaimNums=new List<long>();
					for(int i = 0;i<tableClaimNums.Rows.Count;i++) {
						listClaimNums.Add(PIn.Long(tableClaimNums.Rows[i]["ClaimNum"].ToString()));
					}
					if(listClaimNums.Count>0) {
						List<EnumPermType> listEnumPermTypes=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeof(Claim)));
						List<SecurityLog> listSecurityLogs=SecurityLogs.GetFromFKeysAndType(listClaimNums,listEnumPermTypes);
						Claims.ClearFkey(listClaimNums);//Zero securitylog FKey column for rows to be deleted.
						//Insert changes to DbmLogs
						for(int i=0;i<listSecurityLogs.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSecurityLogs[i].SecurityLogNum,DbmLogFKeyType.Securitylog,DbmLogActionType.Update,
								methodName,"Updated FKey from "+listSecurityLogs[i].FKey+" to 0 from ClaimDeleteWithNoClaimProcs.");
							listDbmLogs.Add(dbmLog);
						}
					}
					//Orphaned claims do not show in the account module (tested) so we need to delete them because no other way.
					command=@"DELETE FROM claim "
						+$"WHERE {patWhere}NOT EXISTS(SELECT * FROM claimproc WHERE claim.ClaimNum=claimproc.ClaimNum AND IsOverPay=0)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimNums[i],DbmLogFKeyType.Claim,DbmLogActionType.Delete,
							methodName,"Deleted claim from ClaimDeleteWithNoClaimProcs");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed!=0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claims deleted due to no claimprocs")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,IsReplicationUnsafe=true)]
		public static string ClaimWithInvalidInsSub(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				#region Check
				case DbmMode.Check:
					//Claim.PlanNum is 0 and inssub.plannum is 0 or does not exist.
					command=@"SELECT COUNT(*) FROM claim 
						LEFT JOIN inssub ON claim.InsSubNum=inssub.InsSubNum
						WHERE claim.PlanNum=0 
						AND ( inssub.PlanNum=0 OR inssub.PlanNum IS NULL )";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims with inssub with invalid PlanNum")+": "+numFound+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					//situation where PlanNum and InsSubNum are both invalid and not zero is handled in InsSubNumMismatchPlanNum
					break;
				#endregion
				#region Breakdown
				case DbmMode.Breakdown:
					//We only need 3 fields, but we must get them all in order for Crud SelectMany to work.
					command=@"SELECT claim.* FROM claim 
						LEFT JOIN inssub ON claim.InsSubNum=inssub.InsSubNum 
						WHERE claim.PlanNum=0 
						AND ( inssub.PlanNum=0 OR inssub.PlanNum IS NULL )";
					List<Claim> listClaims=Crud.ClaimCrud.SelectMany(command);
					if(listClaims.Count > 0 || verbose) {
						//Get minimal patient information in order to display the name of the patient to the user.
						List<long> listPatNums=listClaims.Select(x => x.PatNum).Distinct().ToList();
						List<Patient> listPatientsLim=Patients.GetLimForPats(listPatNums);
						log+=Lans.g("FormDatabaseMaintenance","Claims with inssub with invalid PlanNum")+": "+listClaims.Count.ToString()+"\r\n";
						log+=Lans.g("FormDatabaseMaintenance","Patients Affected")+": "+listPatNums.Count.ToString()+"\r\n\r\n";
						StringBuilder stringBuilder=new StringBuilder();
						for(int i=0;i<listPatientsLim.Count;i++){//Only show information for patients found in the database.
							//No translations needed, all "words" will exactly match schema column names.
							stringBuilder.AppendLine(listPatientsLim[i].GetNameLF()+" (PatNum:"+listPatientsLim[i].PatNum+")");
							for(int j=0;j<listClaims.Count;j++){
								if(listClaims[j].PatNum!=listPatientsLim[i].PatNum){
									continue;
								}
								stringBuilder.AppendLine("    ClaimNum:"+listClaims[j].ClaimNum.ToString()+" DateService:"+listClaims[j].DateService.ToShortDateString());
							}
							stringBuilder.AppendLine("");//Add a newline between each patient for a nice visible separation.
						}
						log+=stringBuilder.ToString();//Does nothing if strBuilder is empty.
					}
					break;
				#endregion
				#region Fix
				case DbmMode.Fix:
					command=@"SELECT claim.ClaimNum, claim.PatNum FROM claim 
						LEFT JOIN inssub ON claim.InsSubNum=inssub.InsSubNum 
						WHERE claim.PlanNum=0 
						AND ( inssub.PlanNum=0 OR inssub.PlanNum IS NULL )";
					DataTable table=Db.GetTable(command);
					int numberFixed=table.Rows.Count;
					InsPlan insPlan=null;
					InsSub insSub=null;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(numberFixed>0) {
						log+=Lans.g("FormDatabaseMaintenance","Reenter insurance information for patients associated to UNKNOWN CARRIER.")+"\r\n";
					}
					long unknownCarrierNum=Carriers.GetByNameAndPhone("UNKNOWN CARRIER","",true).CarrierNum;
					for(int i=0;i<numberFixed;i++) {
						insPlan=new InsPlan();//Create a dummy plan and carrier to attach claims and claim procs to.
						insPlan.IsHidden=true;
						insPlan.CarrierNum=unknownCarrierNum;
						//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
						insPlan.SecUserNumEntry=Security.CurUser.UserNum;
						InsPlans.Insert(insPlan);
						long claimNum=PIn.Long(table.Rows[i]["ClaimNum"].ToString());
						insSub=new InsSub();//Create inssubs and attach claim and procs to both plan and inssub.
						insSub.PlanNum=insPlan.PlanNum;
						insSub.Subscriber=PIn.Long(table.Rows[i]["PatNum"].ToString());
						insSub.SubscriberID="unknown";
						//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
						insSub.SecUserNumEntry=Security.CurUser.UserNum;
						InsSubs.Insert(insSub);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,insSub.InsSubNum,DbmLogFKeyType.InsSub,DbmLogActionType.Insert,
							methodName,"Added new InsSub from ClaimWithInvalidInsSub.");
						listDbmLogs.Add(dbmLog);
						List<Claim> listClaimsInvalid=Crud.ClaimCrud.SelectMany("SELECT * FROM claim WHERE ClaimNum="+claimNum);
						command="UPDATE claim SET PlanNum="+insPlan.PlanNum+",InsSubNum="+insSub.InsSubNum+" WHERE ClaimNum="+claimNum;
						Db.NonQ(command);
						for(int j=0;j<listClaimsInvalid.Count;j++){
							dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimsInvalid[j].ClaimNum,DbmLogFKeyType.Claim,DbmLogActionType.Update,
								methodName,"Updated PlanNum from "+listClaimsInvalid[j].PlanNum+" to "+insPlan.PlanNum
								+" and InsSubNum from "+listClaimsInvalid[j].InsSubNum+" to "+insSub.InsSubNum+" from ClaimWithInvalidInsSub");
							listDbmLogs.Add(dbmLog);
						}
						List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany("SELECT * FROM claimproc WHERE ClaimNum="+claimNum);
						command="UPDATE claimproc SET PlanNum="+insPlan.PlanNum+",InsSubNum="+insSub.InsSubNum+" WHERE ClaimNum="+claimNum;
						Db.NonQ(command);
						for(int j=0;j<listClaimProcs.Count;j++){
							dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[j].ClaimProcNum,DbmLogFKeyType.ClaimProc,
								DbmLogActionType.Update,methodName,"Updated PlanNum from "+listClaimProcs[j].PlanNum+" to "+insPlan.PlanNum
								+" and InsSubNum from "+listClaimProcs[j].InsSubNum+" to "+insSub.InsSubNum+" from ClaimWithInvalidInsSub");
							listDbmLogs.Add(dbmLog);
						}
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claims with invalid InsSubNum fixed")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
					#endregion
			}
			return log;
		}

		///<summary>Also fixes situations where PatNum=0</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimWithInvalidPatNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT claim.ClaimNum, procedurelog.PatNum patNumCorrect
				FROM claim, claimproc, procedurelog
				WHERE claim.PatNum NOT IN (SELECT PatNum FROM patient)
				AND claim.ClaimNum=claimproc.ClaimNum
				AND claimproc.ProcNum=procedurelog.ProcNum
				AND procedurelog.PatNum!=0
				GROUP BY claim.ClaimNum, procedurelog.PatNum";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims found with invalid patients attached: ")+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i = 0;i<table.Rows.Count;i++) {
						command="SELECT * FROM claim WHERE ClaimNum="+POut.Long(PIn.Long(table.Rows[i]["ClaimNum"].ToString()));
						List<Claim> listClaims=Crud.ClaimCrud.SelectMany(command);
						command="UPDATE claim SET PatNum='"+POut.Long(PIn.Long(table.Rows[i]["patNumCorrect"].ToString()))+"' "
							+"WHERE ClaimNum="+POut.Long(PIn.Long(table.Rows[i]["ClaimNum"].ToString()));
						Db.NonQ(command);
						for(int j=0;j<listClaims.Count;j++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaims[j].ClaimNum,DbmLogFKeyType.Claim,DbmLogActionType.Update,
								methodName,"Updated PatNum from "+listClaims[j].PatNum+" to "+table.Rows[i]["patNumCorrect"].ToString());
							listDbmLogs.Add(dbmLog);
						}
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claim with invalid PatNums fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimWithInvalidProvTreat(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			string command="SELECT * FROM claim WHERE ProvTreat > 0 AND ProvTreat NOT IN (SELECT ProvNum FROM provider);";
			List<Claim> listClaims=Crud.ClaimCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaims.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims with invalid ProvTreat found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="UPDATE claim SET ProvTreat="+POut.Long(PrefC.GetLong(PrefName.PracticeDefaultProv))+
							" WHERE ProvTreat > 0 AND ProvTreat NOT IN (SELECT ProvNum FROM provider);";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaims.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaims[i].ClaimNum,DbmLogFKeyType.Claim,DbmLogActionType.Update,
							methodName,"Updated ProvTreat from "+listClaims[i].ProvTreat+" to "+POut.Long(PrefC.GetLong(PrefName.PracticeDefaultProv)));
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claims with invalid ProvTreat fixed")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimWriteoffSum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			//Sums for each claim---------------------------------------------------------------------
			string command=@"SELECT claim.ClaimNum,SUM(claimproc.WriteOff) sumwo,claim.WriteOff
				FROM claim,claimproc
				WHERE claim.ClaimNum=claimproc.ClaimNum
				GROUP BY claim.ClaimNum,claim.WriteOff
				HAVING sumwo-claim.WriteOff > .01
				OR sumwo-claim.WriteOff < -.01";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claim writeoff sums found incorrect: ")+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i = 0;i<table.Rows.Count;i++) {
						command="SELECT * FROM claim WHERE ClaimNum="+table.Rows[i]["ClaimNum"].ToString();
						List<Claim> listClaims=Crud.ClaimCrud.SelectMany(command);
						command="UPDATE claim SET WriteOff='"+POut.Double(PIn.Double(table.Rows[i]["sumwo"].ToString()))+"' "
							+"WHERE ClaimNum="+table.Rows[i]["ClaimNum"].ToString();
						Db.NonQ(command);
						for(int j=0;j<listClaims.Count;j++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaims[j].ClaimNum,DbmLogFKeyType.Claim,DbmLogActionType.Update,
								methodName,"Updated the WriteOff from "+table.Rows[i]["WriteOff"].ToString()
								+" to "+table.Rows[i]["sumwo"].ToString()+" from ClaimWriteoffSum");
							listDbmLogs.Add(dbmLog);
						}
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claim writeoff sums fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string ClaimMissingProcedures(bool verbose, DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT patient.Lname 'Last Name',patient.FName 'First Name',DATE(claim.DateService) 'Claim Date',claim.ClaimType 'Claim Type',"
					+"claim.ClaimFee 'Claim Fee',GROUP_CONCAT(claimproc.CodeSent) AS 'Codes Sent' "
					+"FROM claimproc "
					+"INNER JOIN claim ON claimproc.ClaimNum=claim.ClaimNum "
					+"INNER JOIN patient ON patient.PatNum=claim.PatNum "
					+"WHERE claimproc.ProcNum=0 AND claimproc.CodeSent!='' "
					+"GROUP BY claim.ClaimNum "
					+"ORDER BY patient.PatNum,claim.DateService,claim.ClaimNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return log;
			}
			log+=Lans.g("FormDatabaseMaintenance","Claims with missing procedures")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count>0) {
						log+="\r\n"+Lans.g("FormDatabaseMaintenance","Manual fix needed. Double click to see a break down.");
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count==0) {
						return log;
					}
					log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
					List<int> listMaxColumnWidths=new List<int>();
					List<string> listStringColValues=new List<string>();
					int widthColumn;
					string strColumnValueMax;
					string strColumnValue;
					for(int i=0;i< table.Columns.Count;i++) {
						if(table.Columns[i].ColumnName=="Claim Date") {
							strColumnValueMax=table.AsEnumerable()
								.Select(x => ((DateTime)x[table.Columns[i]]).ToShortDateString())
								.OrderByDescending(x => x.Length)
								.FirstOrDefault();
						}
						else {
							strColumnValueMax=table.AsEnumerable()
								.Select(x => x[table.Columns[i]].ToString())
								.OrderByDescending(x => x.Length)
								.FirstOrDefault();
						}
						if(strColumnValueMax.IsNullOrEmpty()) {
							widthColumn=100;//Default width if parse fails. Shouldn't happen.
						}
						else {
							widthColumn=strColumnValueMax.Length;
						}
						listMaxColumnWidths.Add(widthColumn);
						strColumnValue=table.Columns[i].ColumnName;
						if(strColumnValue.Length < widthColumn) {//Pad with spaces if needed
							strColumnValue+=new string(' ',widthColumn-strColumnValue.Length);
						}
						listStringColValues.Add(strColumnValue);
					}
					log+=string.Join(" | ",listStringColValues)+"\r\n"+new string('-',93)+"\r\n";
					listStringColValues.Clear();
					for(int i = 0;i < table.Rows.Count;i++) {
						for(int j=0;j<table.Columns.Count;j++) {
							if(table.Columns[j].ColumnName=="Claim Type") {
								try {
									EnumClaimType enumClaimType=(EnumClaimType)Enum.Parse(typeof(EnumClaimType),table.Rows[i][j].ToString());
									strColumnValue=Lans.g("FormDatabaseMaintenance",enumClaimType.GetDescription());
								}
								catch {
									strColumnValue=Lans.g("FormDatabaseMaintenance",table.Rows[i][j].ToString());
								}
							} 
							else if (table.Columns[j].ColumnName=="Claim Fee") {
								try {
									decimal amtClaimFee=decimal.Parse(table.Rows[i][j].ToString());
									strColumnValue=amtClaimFee.ToString("C",CultureInfo.CurrentCulture);
								}
								catch {
									strColumnValue=table.Rows[i][j].ToString();
								}
							}
							else if (table.Columns[j].ColumnName=="Claim Date") {
								strColumnValue=((DateTime)table.Rows[i][j]).ToShortDateString();
							}
							else {
								strColumnValue=table.Rows[i][j].ToString();
							}
							if(strColumnValue.Length<table.Columns[j].ColumnName.Length) {//Pad with spaces if needed
								strColumnValue+=new string(' ',table.Columns[j].ColumnName.Length-strColumnValue.Length);
							}
							listStringColValues.Add(strColumnValue);
						}
						log+=string.Join(" | ",listStringColValues)+"\r\n";
						listStringColValues.Clear();
					}
					log+="\r\n"+Lans.g("FormDatabaseMaintenance","They need to be fixed manually.");
					break;
			}
			return log;
		}
		#endregion Carrier, Claim-----------------------------------------------------------------------------------------------------------------------
		#region ClaimPayment----------------------------------------------------------------------------------------------------------------------------

		///<summary>Finds claimpayments where the CheckAmt!=SUM(InsPayAmt) for the claimprocs attached to the claimpayment.  Manual fix.</summary>
		[DbmMethodAttr(HasBreakDown=true,IsReplicationUnsafe=true,HasExplain=true)]
		public static string ClaimPaymentCheckAmt(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			//because of the way this is grouped, it will just get one of many patients for each
			string command=@"SELECT claimproc.ClaimPaymentNum,ROUND(SUM(InsPayAmt),2) _sumpay,ROUND(CheckAmt,2) _checkamt,claimproc.PatNum
					FROM claimpayment,claimproc
					WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum
					GROUP BY claimproc.ClaimPaymentNum,CheckAmt
					HAVING _sumpay!=_checkamt";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			//There is something to report OR the user has verbose mode on.
			string log=Lans.g("FormDatabaseMaintenance","Claim payment sums found incorrect: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {//Does not make any DB changes, see breakdown for manual fix.
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						string methodName=MethodBase.GetCurrentMethod().Name;
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						//Changing the claim payment sums automatically is dangerous so give the user enough information to investigate themselves.
						for(int i = 0;i<table.Rows.Count;i++) {
							Patient patient=Patients.GetPat(PIn.Long(table.Rows[i]["PatNum"].ToString()));
							command="SELECT CheckDate,CheckAmt,IsPartial FROM claimpayment WHERE ClaimPaymentNum="+table.Rows[i]["ClaimPaymentNum"].ToString();
							DataTable tableClaimPay=Db.GetTable(command);
							if(patient==null) {
								//insert pat
								Patient patientDummy=new Patient();
								patientDummy.PatNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
								patientDummy.Guarantor=patientDummy.PatNum;
								patientDummy.FName="MISSING";
								patientDummy.LName="PATIENT";
								patientDummy.AddrNote="This patient was inserted due to claimprocs with invalid PatNum on "+DateTime.Now.ToShortDateString()+" while doing database maintenance.";
								patientDummy.BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType);
								patientDummy.PatStatus=PatientStatus.Archived;
								patientDummy.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
								//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
								patientDummy.SecUserNumEntry=Security.CurUser.UserNum;
								long patNumDummy=Patients.Insert(patientDummy,true);
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patNumDummy,DbmLogFKeyType.Patient,DbmLogActionType.Insert,
									methodName,"Inserted new patient from ClaimPaymentCheckAmt.");
								listDbmLogs.Add(dbmLog);
								SecurityLogs.MakeLogEntry(EnumPermType.PatientCreate,patNumDummy,"Recreated from DBM fix for ClaimPaymentCheckAmt.",LogSources.DBM);
								patient=Patients.GetPat(patientDummy.PatNum);
							}
							log+="   Patient: #"+table.Rows[i]["PatNum"].ToString()+":"+patient.GetNameFirstOrPrefL()
									+" Date: "+PIn.Date(tableClaimPay.Rows[0]["CheckDate"].ToString()).ToShortDateString()
									+" Amount: "+PIn.Double(tableClaimPay.Rows[0]["CheckAmt"].ToString()).ToString("F");
							if(!PIn.Bool(tableClaimPay.Rows[0]["IsPartial"].ToString())) {
								command="SELECT * from claimpayment WHERE ClaimPaymentNum="+PIn.Long(table.Rows[i]["ClaimPaymentNum"].ToString()).ToString();
								List<ClaimPayment> listClaimPayments=Crud.ClaimPaymentCrud.SelectMany(command);
								command="UPDATE claimpayment SET IsPartial=1 WHERE ClaimPaymentNum="+PIn.Long(table.Rows[i]["ClaimPaymentNum"].ToString()).ToString();
								Db.NonQ(command);
								for(int j=0;j<listClaimPayments.Count;j++){
									DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimPayments[j].ClaimPaymentNum,DbmLogFKeyType.ClaimPayment,
										DbmLogActionType.Update,methodName,"Updated ClaimPaymentNum from "+listClaimPayments[j].IsPartial+" to True from ClaimPaymentCheckAmt.");
									listDbmLogs.Add(dbmLog);
								}
								log+=" (row has been unlocked and marked as partial)";
							}
							log+="\r\n";
						}
						if(listDbmLogs.Count>0) {
							Crud.DbmLogCrud.InsertMany(listDbmLogs);
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
						#region Potential Fix
						/*
						for(int i=0;i<table.Rows.Count;i++) {
							command="UPDATE claimpayment SET CheckAmt='"+POut.Double(PIn.Double(table.Rows[i]["_sumpay"].ToString()))+"' "
								+"WHERE ClaimPaymentNum="+table.Rows[i]["ClaimPaymentNum"].ToString();
							Db.NonQ(command);
						}
						int numberFixed=table.Rows.Count;
						if(numberFixed>0 || verbose) {
							log+=Lans.g("FormDatabaseMaintenance","Claim payment sums fixed: ")+numberFixed.ToString()+"\r\n";
						}*/
						#endregion Potential Fix
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimPaymentDetachMissingDeposit(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM claimpayment "
				+"WHERE DepositNum != 0 "
				+"AND NOT EXISTS(SELECT * FROM deposit WHERE deposit.DepositNum=claimpayment.DepositNum)";
			List<ClaimPayment> listClaimPayments=Crud.ClaimPaymentCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimPayments.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claim payments attached to deposits that no longer exist: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="UPDATE claimpayment SET DepositNum=0 "
						+"WHERE DepositNum != 0 "
						+"AND NOT EXISTS(SELECT * FROM deposit WHERE deposit.DepositNum=claimpayment.DepositNum)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimPayments.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimPayments[i].ClaimPaymentNum,DbmLogFKeyType.ClaimPayment,
							DbmLogActionType.Update,methodName,"Updated the DepositNum from "+listClaimPayments[i].DepositNum+" to 0 .");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claim payments detached from deposits that no longer exist: ")
						+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string ClaimPaymentsNotPartialWithNoClaimProcs(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT claimpayment.CheckDate, definition.ItemName, claimpayment.CheckAmt, "+
				"claimpayment.CarrierName, clinic.Description, claimpayment.Note "+
				"FROM claimpayment "+
				"INNER JOIN definition ON definition.DefNum=claimpayment.PayType "+
				"LEFT JOIN clinic ON clinic.ClinicNum=claimpayment.ClinicNum "+
				"WHERE claimpayment.IsPartial=0 "+
				"AND NOT EXISTS( "+
					"SELECT ClaimProcNum "+
					"FROM claimproc "+
					"WHERE claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum "+
				") ";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			//There is something to report OR the user has verbose mode on.   
			string log=Lans.g("FormDatabaseMaintenance","Insurance payments with no claims attached that are not marked as partial")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   "+Lans.g("FormDatabaseMaintenance","Date")+": "+PIn.Date(table.Rows[i]["CheckDate"].ToString()).ToShortDateString();
							log+=", "+Lans.g("FormDatabaseMaintenance","Type")+": "+PIn.String(table.Rows[i]["ItemName"].ToString());
							log+=", "+Lans.g("FormDatabaseMaintenance","Amount")+": "+PIn.Double(table.Rows[i]["CheckAmt"].ToString()).ToString("c");
							//Partial will always be blank
							log+=", "+Lans.g("FormDatabaseMaintenance","Carrier")+": "+PIn.String(table.Rows[i]["CarrierName"].ToString());
							log+=", "+Lans.g("FormDatabaseMaintenance","Clinic")+": "+PIn.String(table.Rows[i]["Description"].ToString());
							log+=", "+Lans.g("FormDatabaseMaintenance","Note")+": ";
							if(PIn.String(table.Rows[i]["Note"].ToString()).Length>15) {
								log+=PIn.String(table.Rows[i]["Note"].ToString()).Substring(0,15)+"...";
							}
							else {
								log+=PIn.String(table.Rows[i]["Note"].ToString());
							}
							log+="\r\n";
						}
						log+="   "+Lans.g("FormDatabaseMaintenance","They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion ClaimPayment-------------------------------------------------------------------------------------------------------------------------
		#region ClaimProc-------------------------------------------------------------------------------------------------------------------------------
		/// <summary>Shows patients that have claim payments attached to patient payment plans.</summary>
		[DbmMethodAttr(HasBreakDown=true)]
		public static string ClaimProcAttachedToPatientPaymentPlans(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			DataTable table=GetClaimProcsAttachedToPatientPaymentPlans();
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","ClaimProcs attached to insurance payment plans")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+=Lans.g("FormDatabaseMaintenance","Manual fix needed. Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i=0;i<table.Rows.Count;i++) {
							log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Patient #")+table.Rows[i]["PatNum"].ToString()+" "
								+Lans.g("FormDatabaseMaintenance","has a payment amount for")+" "+PIn.Double(table.Rows[i]["InsPayAmt"].ToString()).ToString("c")+" "
								+Lans.g("FormDatabaseMaintenance","on date")+" "+PIn.Date(table.Rows[i]["DateCP"].ToString()).ToShortDateString()+" "
								+Lans.g("FormDatabaseMaintenance","attached to patient payment plan #")+table.Rows[i]["PayPlanNum"];
						}
						log+="\r\n"+Lans.g("FormDatabaseMaintenance","Run 'Pay Plan Payments' in the Tools tab to fix these payments.");
					}
					break;
			}
			return log;
		}

		///<summary>Deletes claimprocs that are attached to group notes.</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true,HasExplain=true)]
		public static string ClaimProcEstimateAttachedToGroupNote(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//It is impossible to attach a group note to a claim, because group notes always have status EC, but status C is required to attach to a claim, or status TP for a preauth.
			//Since the group note cannot be attached to a claim, it also cannot be attached to a claim payment.
			//Claimproc estimates attached to group notes cannot be viewed anywhere in the UI.
			string command="SELECT claimproc.ClaimProcNum "
				+"FROM claimproc "
				+"INNER JOIN procedurelog ON claimproc.ProcNum=procedurelog.ProcNum "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum AND procedurecode.ProcCode='~GRP~' "
				+"WHERE claimproc.Status="+POut.Int((int)ClaimProcStatus.Estimate)+" AND claimproc.ClaimNum=0 AND claimproc.ClaimPaymentNum=0";//Ensures that the claimproc has no relevant information attached to it.
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Estimates attached to group notes")+": "+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(table.Rows.Count>0) {
						string inCommand=string.Join(",",table.Select().Select(x => PIn.Long(x["ClaimProcNum"].ToString())));
						command="DELETE FROM claimproc WHERE claimproc.ClaimProcNum IN ("+inCommand+")";
						Db.NonQ(command);
						List<long> listClaimProcNums=table.Select().Select(x => PIn.Long(x["ClaimProcNum"].ToString())).ToList();
						for(int i=0;i<listClaimProcNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
								methodName,"Deleted claimproc.");
							listDbmLogs.Add(dbmLog);
						}
					}
					if(table.Rows.Count>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Estimates attached to group notes deleted")+": "+table.Rows.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDateNotMatchCapComplete(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM claimproc WHERE Status=7 AND DateCP != ProcDate";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Capitation procs with mismatched dates found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//js ok
					command="UPDATE claimproc SET DateCP=ProcDate WHERE Status=7 AND DateCP != ProcDate";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,
							methodName,"Updated the DateCp from "+listClaimProcs[i].DateCP+" to "+listClaimProcs[i].ProcDate+".");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Capitation procs with mismatched dates fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,IsReplicationUnsafe=true)]
		public static string ClaimProcDateNotMatchPayment(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			DataTable table;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT claimproc.ClaimProcNum,claimpayment.CheckDate FROM claimproc,claimpayment "
						+"WHERE claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum "
						+"AND claimproc.DateCP!=claimpayment.CheckDate";
					table=Db.GetTable(command);
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claim payments with mismatched dates found:")+" "+table.Rows.Count.ToString()+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown: //Splits off below, no DB changes.
				case DbmMode.Fix:
					//This is a very strict relationship that has been enforced rigorously for many years.
					//If there is an error, it is a fairly new error.  All errors must be repaired.
					//It won't change amounts of history, just dates.  The changes will typically be only a few days or weeks.
					//Various reports assume this enforcement and the reports will malfunction if this is not fixed.
					//Let's list out each change.  Patient name, procedure desc, date of service, old dateCP, new dateCP (check date).
					command="SELECT patient.LName,patient.FName,patient.MiddleI,claimproc.CodeSent,claim.DateService,claimproc.DateCP,claimpayment.CheckDate,claimproc.ClaimProcNum "
						+"FROM claimproc,patient,claim,claimpayment "
						+"WHERE claimproc.PatNum=patient.PatNum "
						+"AND claimproc.ClaimNum=claim.ClaimNum "
						+"AND claimproc.ClaimPaymentNum=claimpayment.ClaimPaymentNum "
						+"AND claimproc.DateCP!=claimpayment.CheckDate";
					table=Db.GetTable(command);
					string patientName;
					string codeSent;
					DateTime dateService;
					DateTime dateCPOld;
					DateTime dateCPNew;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listClaimProcNums=null;
					long claimProcNum;
					StringBuilder stringBuilder=new StringBuilder();
					if(table.Rows.Count>0 || verbose) {
						stringBuilder.AppendLine("Claim payments with mismatched dates (Patient Name, Code Sent, Date of Service, Old Date, New Date):");
					}
					for(int i=0;i<table.Rows.Count;i++) {
						patientName=table.Rows[i]["LName"].ToString() + ", " + table.Rows[i]["FName"].ToString() + " " + table.Rows[i]["MiddleI"].ToString();
						patientName=patientName.Trim();//Looks better when middle initial is not present.//Doesn't work though
						codeSent=table.Rows[i]["CodeSent"].ToString();
						dateService=PIn.Date(table.Rows[i]["DateService"].ToString());
						dateCPOld=PIn.Date(table.Rows[i]["DateCP"].ToString());
						dateCPNew=PIn.Date(table.Rows[i]["CheckDate"].ToString());
						claimProcNum=PIn.Long(table.Rows[i]["ClaimProcNum"].ToString());
						command="SELECT ClaimProcNum FROM claimproc WHERE ClaimProcNum="+claimProcNum.ToString();
						listClaimProcNums=Db.GetListLong(command);
						if(dbmMode==DbmMode.Fix) {
							command="UPDATE claimproc SET DateCP="+POut.Date(dateCPNew)
								+" WHERE ClaimProcNum="+claimProcNum.ToString();
							Db.NonQ(command);
							for(int j=0;j<listClaimProcNums.Count;j++){
								DbmLog dbmlog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[j],DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,
									methodName,"Updated DateCP from "+dateCPOld+" to "+dateCPNew+".");
								listDbmLogs.Add(dbmlog);
							}
						}
						else {//Breakdown
							stringBuilder.AppendLine(patientName+", "+codeSent+", "+dateService.ToShortDateString()+", "+dateCPOld.ToShortDateString()
								+", "+dateCPNew.ToShortDateString());
						}
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						if(dbmMode==DbmMode.Fix) {
							log=Lans.g("FormDatabaseMaintenance","Claim payments with mismatched dates fixed:")+" "+numberFixed.ToString()+"\r\n";
						}
						else {//Breakdown
							stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Claim payments with mismatched dates found:")+" "+numberFixed.ToString());
							log=stringBuilder.ToString();
						}
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteDroppedInsPlanEstimates(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Get all of the ClaimProcNums for insurance estimates that are not associated to a claim and are associated to an insurance plan that has been dropped.
			string command=$@"SELECT claimproc.ClaimProcNum 
				FROM claimproc
				LEFT JOIN (
					SELECT patplan.PatNum,insplan.PlanNum
					FROM patplan
					INNER JOIN inssub ON patplan.InsSubNum=inssub.InsSubNum
					INNER JOIN insplan ON inssub.PlanNum=insplan.PlanNum
				) patinsplan ON claimproc.PatNum=patinsplan.PatNum AND claimproc.PlanNum=patinsplan.PlanNum
				WHERE claimproc.ClaimNum=0
				AND claimproc.Status IN ({POut.Enum(ClaimProcStatus.Estimate)},{POut.Enum(ClaimProcStatus.CapEstimate)})
				AND patinsplan.PlanNum IS NULL";//The patient no longer has this insurance plan.
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProc estimates for a dropped InsPlan found:")+" "+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(table.Rows.Count > 0) {
						List<long> listClaimProcNums=table.Select().Select(x => PIn.Long(x["ClaimProcNum"].ToString())).ToList();
						Db.NonQ("DELETE FROM claimproc WHERE ClaimProcNum IN ("+string.Join(",",listClaimProcNums)+")");
						for(int i=0;i<listClaimProcNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
								methodName,"ClaimProc estimate deleted because it was attached to dropped insurance plan.");
							listDbmLogs.Add(dbmLog);
						}
					}
					if(table.Rows.Count > 0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);//Does nothing for empty list.
						log+=Lans.g("FormDatabaseMaintenance","ClaimProc estimates for a dropped InsPlan deleted:")+" "+table.Rows.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteDuplicateEstimateForSameInsPlan(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT cp.ClaimProcNum FROM claimproc cp USE KEY(PRIMARY)"
				+" INNER JOIN claimproc cp2 ON cp2.PatNum=cp.PatNum"
				+" AND cp2.PlanNum=cp.PlanNum"    //The same insurance plan
				+" AND cp2.InsSubNum=cp.InsSubNum"//for the same subscriber
				+" AND cp2.ProcNum=cp.ProcNum"    //for the same procedure.
				+" AND cp2.Status IN ("+POut.Int((int)ClaimProcStatus.Received)+","+POut.Int((int)ClaimProcStatus.NotReceived)+")"
				+" WHERE cp.Status="+POut.Int((int)ClaimProcStatus.Estimate)
				+" AND cp.ClaimNum=0"//Make sure the estimate is not already attached to a claim somehow.
				+" GROUP BY cp.ClaimProcNum";//Group by the PK of the claimprocs that will be deleted so that the counts are accurate.
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Duplicate ClaimProc estimates for the same InsPlan found: ")+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(table.Rows.Count>0) {
						string inCommand=string.Join(",",table.Select().Select(x => PIn.Long(x["ClaimProcNum"].ToString())));
						command="SELECT ClaimProcNum FROM claimproc WHERE ClaimProcNum IN ("+inCommand+")";
						List<long> listClaimProcNums=Db.GetListLong(command);
						command="DELETE FROM claimproc WHERE ClaimProcNum IN ("+inCommand+")";
						Db.NonQ(command);
						for(int i=0;i<listClaimProcNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
								methodName,"Deleted this ClaimProc Estimate due to having a duplicate Received or NotReceived ClaimProc.");
							listDbmLogs.Add(dbmLog);
						}
					}
					if(table.Rows.Count>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Duplicate ClaimProc estimates for the same InsPlan deleted: ")+table.Rows.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteInvalidAdjustments(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ClaimProcNum FROM claimproc WHERE claimproc.ClaimNum=0 "
				+"AND NOT EXISTS(SELECT PlanNum FROM insplan WHERE insplan.PlanNum=claimproc.PlanNum) "
				+"AND claimproc.Status="+POut.Int((int)ClaimProcStatus.Adjustment);
			List<long> listClaimProcNums=Db.GetListLong(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimproc adjustments found with invalid PlanNum:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="DELETE FROM claimproc WHERE claimproc.ClaimNum=0 "
						+"AND NOT EXISTS(SELECT PlanNum FROM insplan WHERE insplan.PlanNum=claimproc.PlanNum) "
						+"AND claimproc.Status="+POut.Int((int)ClaimProcStatus.Adjustment);
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
							methodName,"Deleted ClaimProc from ClaimProcDeleteInvalidAdjustments.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claimproc adjustments deleted due to invalid PlanNum:")+" "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasPatNum = true,IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteWithInvalidClaimNum(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			string patWhere=(patNum>0 ? "AND claimproc.PatNum="+POut.Long(patNum)+" " : "");
			string command="SELECT ClaimProcNum FROM claimproc WHERE claimproc.ClaimNum!=0 "
						+patWhere
						+"AND NOT EXISTS(SELECT * FROM claim WHERE claim.ClaimNum=claimproc.ClaimNum) "
						+"AND claimproc.InsPayAmt=0 AND claimproc.WriteOff=0";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs found with invalid ClaimNum: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listClaimProcNums=Db.GetListLong(command);
					command="DELETE FROM claimproc WHERE claimproc.ClaimNum!=0 "
						+patWhere
						+"AND NOT EXISTS(SELECT * FROM claim WHERE claim.ClaimNum=claimproc.ClaimNum) "
						+"AND claimproc.InsPayAmt=0 AND claimproc.WriteOff=0";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
							methodName,"Deleted ClaimProc from ClaimProcDeleteWithInvalidClaimNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs deleted due to invalid ClaimNum: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteMismatchPatNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT ClaimProcNum FROM claimproc "
				+"INNER JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum "
				+"WHERE claimproc.ProcNum>0 "
				+"AND claimproc.PatNum!=procedurelog.PatNum "
				+"AND claimproc.InsPayAmt=0 "
				+"AND(claimproc.WriteOff=0 "
				+"OR(claimproc.Status="+(int)ClaimProcStatus.CapEstimate+" "
				+"AND claimproc.WriteOff=procedurelog.ProcFee AND procedurelog.ProcStatus IN("+(int)ProcStat.TP+","+(int)ProcStat.TPi+")))";
			List<long> listClaimProcNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs found with PatNum that doesn't match the procedure PatNum:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listClaimProcNums.Count>0) {
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						string methodName=MethodBase.GetCurrentMethod().Name;
						command="DELETE FROM claimproc WHERE ClaimProcNum IN("+string.Join(",",listClaimProcNums)+")";
						int numberFixed=(int)Db.NonQ(command);
						for(int i=0;i<listClaimProcNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
								methodName,"Deleted ClaimProc from ClaimProcDeleteMismatchPatNum.");
							listDbmLogs.Add(dbmLog);
						}
						if(numberFixed>0 || verbose) {
							log+=Lans.g("FormDatabaseMaintenance","Claimprocs deleted due to PatNum not matching the procedure PatNum:")+" "
								+numberFixed.ToString()+"\r\n";
							Crud.DbmLogCrud.InsertMany(listDbmLogs);
						}
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteEstimateWithInvalidProcNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ClaimProcNum FROM claimproc WHERE ProcNum>0 "
				+"AND Status="+POut.Int((int)ClaimProcStatus.Estimate)+" "
				+"AND NOT EXISTS(SELECT * FROM procedurelog "
				+"WHERE claimproc.ProcNum=procedurelog.ProcNum)";
			List<long> listClaimProcNums=Db.GetListLong(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Estimates found for procedures that no longer exist: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//These seem to pop up quite regularly due to the program forgetting to delete them
					command="DELETE FROM claimproc WHERE ProcNum>0 "
						+"AND Status="+POut.Int((int)ClaimProcStatus.Estimate)+" "
						+"AND NOT EXISTS(SELECT * FROM procedurelog "
						+"WHERE claimproc.ProcNum=procedurelog.ProcNum)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
							methodName,"Deleted ClaimProc from ClaimProcDeleteEstimateWithInvalidProcNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Estimates deleted for procedures that no longer exist: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcDeleteCapEstimateWithProcComplete(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ClaimProcNum FROM claimproc WHERE ProcNum>0 "
				+"AND Status="+POut.Int((int)ClaimProcStatus.CapEstimate)+" "
				+"AND EXISTS("
					+"SELECT * FROM procedurelog "
					+"WHERE claimproc.ProcNum=procedurelog.ProcNum "
					+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)
				+")";
			List<long> listClaimProcNums=Db.GetListLong(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Capitation estimates found for completed procedures: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="DELETE FROM claimproc WHERE ProcNum>0 "
						+"AND Status="+POut.Int((int)ClaimProcStatus.CapEstimate)+" "
						+"AND EXISTS("
							+"SELECT * FROM procedurelog "
							+"WHERE claimproc.ProcNum=procedurelog.ProcNum "
							+"AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)
						+")";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
							methodName,"Deleted ClaimProc from ClaimProcDeleteCapEstimateWithProcComplete.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Capitation estimates deleted for completed procedures: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcEstNoBillIns(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM claimproc WHERE NoBillIns=1 AND IsOverpay=0 AND InsPayEst !=0";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs found with non-zero estimates marked NoBillIns: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//This is just estimate info, regardless of the claimproc status, so totally safe.
					command="UPDATE claimproc SET InsPayEst=0 WHERE NoBillIns=1 AND IsOverpay=0 AND InsPayEst !=0";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++) {
						ClaimProc claimProcOld=listClaimProcs[i];
						if(ClaimProcs.IsClaimProcHashValid(claimProcOld)) { //Only rehash claimprocs that were already valid.
							//Since the query updated each of the claimprocs to have an InsPayEst of 0, we're going to make a copy of the current one and set it's InsPayEst to 0 before hashing.
							ClaimProc claimProc=claimProcOld.Copy();
							claimProc.InsPayEst=0;
							claimProc.SecurityHash=ClaimProcs.HashFields(claimProc);
							Crud.ClaimProcCrud.Update(claimProc);
						}
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,
							methodName,"Updated InsPayEst from "+listClaimProcs[i].InsPayEst+" to 0 from ClaimProcEstNoBillIns.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claimproc estimates set to zero because marked NoBillIns: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcEstWithInsPaidAmt(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command=@"SELECT * FROM claimproc WHERE InsPayAmt > 0 AND ClaimNum=0 AND Status=6";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProc estimates with InsPaidAmt > 0 found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//The InsPayAmt is already being ignored due to the status of the claimproc.  So changing its value is harmless.
					command=@"UPDATE claimproc SET InsPayAmt=0 WHERE InsPayAmt > 0 AND ClaimNum=0 AND Status=6";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++) {
						ClaimProc claimProcOld=listClaimProcs[i];
						if(ClaimProcs.IsClaimProcHashValid(claimProcOld)) { //Only rehash claimprocs that were already valid.
							//Since the query updated each of the claimprocs to have an InsPayAmt of 0, we're going to make a copy of the current one and set it's InsPayAmt to 0 before hashing.
							ClaimProc claimProc=claimProcOld.Copy();
							claimProc.InsPayAmt=0;
							claimProc.SecurityHash=ClaimProcs.HashFields(claimProc);
							Crud.ClaimProcCrud.Update(claimProc);
						}
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,
							methodName,"Updated InsPayAmt from "+listClaimProcs[i].InsPayEst+" to 0 from ClaimProcEstWithInsPaidAmt.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProc estimates with InsPaidAmt > 0 fixed: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcPatNumMissing(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ClaimProcNum FROM claimproc WHERE PatNum=0 AND InsPayAmt=0 AND WriteOff=0";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with missing patnums found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listClaimProcNums=Db.GetListLong(command);
					command="DELETE FROM claimproc WHERE PatNum=0 AND InsPayAmt=0 AND WriteOff=0";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
							methodName,"Deleted ClaimProc from ClaimProcPatNumMissing.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with missing patnums fixed: ")+numberFixed+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcProvNumMissing(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ClaimProcNum FROM claimproc WHERE ProvNum=0 AND Status="+POut.Int((int)ClaimProcStatus.Estimate);
			List<long> listClaimProcNums=Db.GetListLong(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with missing provnums found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//If estimate, set to default prov (doesn't affect finances)
					command="UPDATE claimproc SET ProvNum="+PrefC.GetString(PrefName.PracticeDefaultProv)+" WHERE ProvNum=0 AND Status="+POut.Int((int)ClaimProcStatus.Estimate);
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,
							methodName,"Updated ProvNum from 0 to "+PrefC.GetString(PrefName.PracticeDefaultProv)+".");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with missing provnums fixed: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					//create a dummy provider (using helper function in Providers.cs)
					//change provnum to the dummy prov (something like Providers.GetDummy())
					//Provider dummy=new Provider();
					//dummy.Abbr="Dummy";
					//dummy.FName="Dummy";
					//dummy.LName="Provider";
					//Will get to this soon.
					//01-17-2011 No fix yet. This has not caused issues except for notifying users.
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string ClaimProcPreauthNotMatchClaim(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT claim.PatNum,claim.DateService,claimproc.ProcDate,claimproc.CodeSent,claimproc.FeeBilled 
				FROM claimproc,claim 
				WHERE claimproc.ClaimNum=claim.ClaimNum
				AND claim.ClaimType='PreAuth'
				AND claimproc.Status!=2";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","ClaimProcs for preauths with status not preauth")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}                    
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							Patient patient=Patients.GetPat(PIn.Long(table.Rows[i]["PatNum"].ToString()));
							log+="   Patient: #"+patient.PatNum.ToString()+":"+patient.GetNameFirstOrPrefL()
								+" ClaimDate: "+PIn.Date(table.Rows[i]["DateService"].ToString()).ToShortDateString()
								+" ProcDate: "+PIn.Date(table.Rows[i]["ProcDate"].ToString()).ToShortDateString()
								+" Code: "+table.Rows[i]["CodeSent"].ToString()+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>We are only checking mismatched statuses if claim is marked as received.</summary>
		[DbmMethodAttr(HasBreakDown=true,HasPatNum=true,HasExplain=true)]
		public static string ClaimProcStatusNotMatchClaim(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string patWhere="";
			if(patNum>0){
				patWhere="AND claimproc.PatNum="+POut.Long(patNum)+" ";
			}
			string command=@"SELECT claim.PatNum,claim.DateService,claimproc.ProcDate,claimproc.CodeSent,claimproc.FeeBilled
					FROM claimproc,claim
					WHERE claimproc.ClaimNum=claim.ClaimNum
					AND claim.ClaimStatus='R'
					AND IsOverpay=0
					AND claimproc.Status="+POut.Int((int)ClaimProcStatus.NotReceived)+" "
					+patWhere;
			//If a claim is re-sent after being received, the claimprocs Status will be Received but the claim will be Sent, which is to be expected, so we
			//no longer want to flag them as being a DBM issue.  They will show on the unreceived claims report and the user can go manually change the
			//claim status to received if it really is a mistake caused by a user manually changing the claim or claimproc statuses.
			//+"OR (claim.ClaimStatus!='R' AND claimproc.Status="+POut.Int((int)ClaimProcStatus.Received)+"))";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","ClaimProcs with status not matching claim found: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							Patient patient=Patients.GetPat(PIn.Long(table.Rows[i]["PatNum"].ToString()));
							log+="   Patient: #"+patient.PatNum.ToString()+":"+patient.GetNameFirstOrPrefL()
								+" ClaimDate: "+PIn.Date(table.Rows[i]["DateService"].ToString()).ToShortDateString()
								+" ProcDate: "+PIn.Date(table.Rows[i]["ProcDate"].ToString()).ToShortDateString()
								+" Code: "+table.Rows[i]["CodeSent"].ToString()
								+" FeeBilled: "+PIn.Double(table.Rows[i]["FeeBilled"].ToString()).ToString("F")+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcTotalPaymentWithInvalidDate(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT claimproc.ClaimProcNum,claimproc.ProcDate,claim.DateService FROM claimproc,claim"
				+" WHERE claimproc.ProcNum=0"//Total payments
				+" AND claimproc.ProcDate < "+POut.Date(new DateTime(1880,1,1))//which have invalid dates
				+" AND claimproc.ClaimNum=claim.ClaimNum"
				+" AND claim.DateService > "+POut.Date(new DateTime(1880,1,1));//but have valid date of service on the claim
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Total claim payments with invalid date found")+": "+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(table.Rows.Count>0) {
						command="UPDATE claimproc,claim SET claimproc.ProcDate=claim.DateService"//Resets date for total payments to DateService
							+" WHERE claimproc.ProcNum=0"//Total payments
							+" AND claimproc.ProcDate < "+POut.Date(new DateTime(1880,1,1))//which have invalid dates
							+" AND claimproc.ClaimNum=claim.ClaimNum"
							+" AND claim.DateService > "+POut.Date(new DateTime(1880,1,1));//but have valid date of service on the claim
						Db.NonQ(command);
						for(int i=0;i<table.Rows.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,PIn.Long(table.Rows[i]["ClaimProcNum"].ToString()),
								DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,methodName,
								"Updated ProcDate from "+table.Rows[i]["ProcDate"].ToString()
								+" to "+table.Rows[i]["DateService"].ToString()+" from ClaimProcTotalPaymentWithInvalidDate.");
							listDbmLogs.Add(dbmLog);
						}
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Total claim payments with invalid date fixed")+": "+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcUpdateStatusWithInvalidClaim(bool verbose,DbmMode dbmMode)  {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@$"SELECT * 
				FROM claimproc 
				WHERE Status={POut.Int((int)ClaimProcStatus.NotReceived)}
				AND ClaimNum=0
				AND InsPayAmt=0 
				AND WriteOff=0";
			List<ClaimProc> listClaimProcs=ClaimProcCrud.SelectMany(command);
			string log="";
			if(listClaimProcs.Count==0 && !verbose) {
				return log;
			}
			switch(dbmMode) {
				case DbmMode.Check:
					log+=Lans.g("FormDatabaseMaintenance",$"ClaimProcs with status")+
						" '"+ClaimProcStatus.NotReceived.GetDescription()+"' "+
						Lans.g("FormDatabaseMaintenance","found where no claim is attached:")+" "+POut.Long(listClaimProcs.Count)+"\r\n";
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(listClaimProcs.Count>0) {
						command=$"UPDATE claimproc SET Status={POut.Enum(ClaimProcStatus.Estimate)} "+
							$"WHERE ClaimProcNum IN("+string.Join(",",listClaimProcs.Select(x=>x.ClaimProcNum))+")";
						Db.NonQ(command);
						for(int i=0;i<listClaimProcs.Count;i++) {
							ClaimProc claimProcOld=listClaimProcs[i];
							if(ClaimProcs.IsClaimProcHashValid(claimProcOld)) { //Only rehash claimprocs that were already valid.
								//Since the query updated each of the claimprocs to have a Status of Estimate, we're going to make a copy of the current one and set it's Status to Estimate before hashing.
								ClaimProc claimProc=claimProcOld.Copy();
								claimProc.Status=ClaimProcStatus.Estimate;
								claimProc.SecurityHash=ClaimProcs.HashFields(claimProc);
								Crud.ClaimProcCrud.Update(claimProc);
							}
						}
						for(int i=0;i<listClaimProcs.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,
								DbmLogActionType.Update,methodName,"Updated status of ClaimProc from '"+ClaimProcStatus.NotReceived.GetDescription()+"' to " +
								"'"+ClaimProcStatus.Estimate.GetDescription()+"' because no claim was attached.");
							listDbmLogs.Add(dbmLog);
						}
					}
					if(listClaimProcs.Count>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with invalid claims set to")+" "+ClaimProcStatus.Estimate.GetDescription()+
							": "+POut.Long(listClaimProcs.Count)+"\r\n";
					}
					break;
			}
			return log;
		}
		
		[DbmMethodAttr(HasBreakDown=true,HasPatNum=true,IsReplicationUnsafe=true)]
		public static string ClaimProcWithInvalidClaimNum(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			string command;
			string patWhere="";
			if(patNum>0){
				patWhere="AND claimproc.PatNum="+POut.Long(patNum)+" ";
			}
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM claimproc "
						+"WHERE claimproc.ClaimNum!=0 "
						+patWhere
						+"AND NOT EXISTS(SELECT * FROM claim WHERE claim.ClaimNum=claimproc.ClaimNum) "
						+"AND (claimproc.InsPayAmt!=0 OR claimproc.WriteOff!=0)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs found with invalid ClaimNum:")+" "+numFound+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:  //No DB changes made, fix splits off below.
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//We can't touch those claimprocs because it would mess up the accounting.
					//Create dummy claims for all claimprocs with invalid ClaimNums if those claimprocs have amounts entered in the InsPayAmt or Writeoff 
					//columns, otherwise you could not delete the procedure or create a new claim.
					command="SELECT * FROM claimproc "
						+"WHERE claimproc.ClaimNum!=0 "
						+patWhere
						+"AND NOT EXISTS(SELECT * FROM claim WHERE claim.ClaimNum=claimproc.ClaimNum) "
						+"AND (claimproc.InsPayAmt!=0 OR claimproc.WriteOff!=0) "
						+"GROUP BY claimproc.ClaimNum";
					DataTable table=Db.GetTable(command);
					List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.TableToList(table);
					Claim claim;
					//Get limited data for all patients that will have a new claim created.
					List<Patient> listPatients=Patients.GetLimForPats(listClaimProcs.Select(x => x.PatNum).ToList());
					if(dbmMode==DbmMode.Fix) {
						for(int i=0;i<listClaimProcs.Count;i++) {
							claim=new Claim();
							claim.ClaimNum=listClaimProcs[i].ClaimNum;
							claim.PatNum=listClaimProcs[i].PatNum;
							claim.ClinicNum=listClaimProcs[i].ClinicNum;
							if(listClaimProcs[i].Status==ClaimProcStatus.Received) {
								claim.ClaimStatus="R";//Status received because we know it's been paid on and the claimproc status is received
							}
							else {
								claim.ClaimStatus="W";
							}
							claim.PlanNum=listClaimProcs[i].PlanNum;
							claim.InsSubNum=listClaimProcs[i].InsSubNum;
							claim.ProvTreat=listClaimProcs[i].ProvNum;
							//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
							claim.SecUserNumEntry=Security.CurUser.UserNum;
							Crud.ClaimCrud.Insert(claim,true);//Allows us to use a primary key that was "used".
							Patient patient=listPatients.Find(x => x.PatNum==claim.PatNum);
							if(patient is null){
								patient=new Patient();
								patient.PatNum=claim.PatNum;
								patient.FName="";
								patient.LName="";
							}
							listDbmLogs.Add(new DbmLog(Security.CurUser.UserNum,claim.ClaimNum,DbmLogFKeyType.Claim,DbmLogActionType.Insert,
								methodName,$"Added new claim from ClaimProcWithInvalidClaimNum. {patient.PatNum} - {patient.GetNameFL()}"));
						}
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						if(listClaimProcs.Count>0 || verbose) {
							log+=Lans.g("FormDatabaseMaintenance","Claimprocs with invalid ClaimNums fixed:")+" "+listClaimProcs.Count+"\r\n";
						}
					}
					if(dbmMode==DbmMode.Breakdown) {
						StringBuilder stringBuilder=new StringBuilder();
						List<DbmLog> listDbmLogsRecent=DbmLogs.GetByMethodName(methodName,DateTime.Now);
						if(listClaimProcs.Count > 0) {
							stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Claims will be created due to claimprocs with invalid ClaimNums for patients:"));
							for(int i=0;i<listPatients.Count;i++){
								stringBuilder.AppendLine(listPatients[i].PatNum.ToString()+" - "+listPatients[i].GetNameFL());
							}
						stringBuilder.AppendLine();
						}
						//Append any DbmLogs for claims that were created today by this method.  This gets around the problem that a Fix will put the database in
						//a state where the Breakdown yields no results, even though the Fix summary indicated action was required.
						foreach(DbmLog logRecent in listDbmLogsRecent) {
							stringBuilder.AppendLine(logRecent.LogText);
						}
						log+=stringBuilder.ToString();
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasPatNum = true,IsReplicationUnsafe=true)]
		public static string ClaimProcWithInvalidClaimPaymentNum(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			string patWhere="";
			if(patNum>0){
				patWhere="AND claimproc.PatNum="+POut.Long(patNum)+" ";
			}
			string command=@"SELECT * FROM claimproc WHERE claimpaymentnum !=0 "
				+patWhere
				+"AND NOT EXISTS(SELECT * FROM claimpayment WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum)";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with with invalid ClaimPaymentNumber found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//slightly dangerous.  User will have to create ins check again.  But does not alter financials.
					command=@"UPDATE claimproc SET ClaimPaymentNum=0 WHERE claimpaymentnum !=0 "
						+patWhere
						+"AND NOT EXISTS(SELECT * FROM claimpayment WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,
							DbmLogActionType.Update,methodName,"Updated ClaimPaymentNum from "+listClaimProcs[i].ClaimPaymentNum+" to 0.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with with invalid ClaimPaymentNumber fixed: ")+numberFixed.ToString()+"\r\n";
						//Tell user what items to create ins checks for?
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcWithInvalidPayPlanNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command=@"SELECT * FROM claimproc WHERE PayPlanNum>0 AND PayPlanNum NOT IN(SELECT PayPlanNum FROM payplan)";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with with invalid PayPlanNum found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					//safe, if the user wants to attach the claimprocs to a payplan for tracking the ins payments they would just need to attach to a valid payplan
					command=@"UPDATE claimproc SET PayPlanNum=0 WHERE PayPlanNum>0 AND PayPlanNum NOT IN(SELECT PayPlanNum FROM payplan)";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,
						 DbmLogActionType.Update,methodName,"Updated PayPlanNum from "+listClaimProcs[i].PayPlanNum+" to 0.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ClaimProcs with with invalid PayPlanNum fixed")+": "+numFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcWithInvalidProvNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM claimproc WHERE ProvNum > 0 AND ProvNum NOT IN (SELECT ProvNum FROM provider)";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listClaimProcs.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs with invalid ProvNum found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="UPDATE claimproc SET ProvNum="+POut.Long(PrefC.GetLong(PrefName.PracticeDefaultProv))+
							" WHERE ProvNum > 0 AND ProvNum NOT IN (SELECT ProvNum FROM provider)";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,DbmLogActionType.Update,methodName,
							"Updated ProvNum from "+listClaimProcs[i].ProvNum+" to "+POut.String(PrefC.GetLong(PrefName.PracticeDefaultProv).ToString()));
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs with invalid ProvNum fixed")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ClaimProcWithInvalidSubNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ClaimProcNum FROM claimproc WHERE claimproc.InsSubNum > 0 AND claimproc.Status="+POut.Int((int)ClaimProcStatus.Estimate)
				+" AND claimproc.InsSubNum NOT IN (SELECT inssub.InsSubNum FROM inssub)";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs with invalid InsSubNum found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listClaimProcNums=Db.GetListLong(command);
					command="DELETE FROM claimproc WHERE claimproc.InsSubNum > 0 AND claimproc.Status="+POut.Int((int)ClaimProcStatus.Estimate)
						+" AND claimproc.InsSubNum NOT IN (SELECT inssub.InsSubNum FROM inssub)";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcNums[i],DbmLogFKeyType.ClaimProc,DbmLogActionType.Delete,
							methodName,"Deleted ClaimProc from ClaimProcWithInvalidSubNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs with invalid InsSubNum fixed")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>This DBM is meant to help Canada customers identify duplicate claimprocs. The behavior causing this bug was fixed in 18.3 and 18.4 by bug jobs 12553 and 13395</summary>
		[DbmMethodAttr(IsCanada=true,HasBreakDown=true,HasExplain=true)]
		public static string ClaimProcsWithPartialDuplicates(bool verbose, DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA") && !ODBuild.IsUnitTest) {//Canadian. en-CA or fr-CA
				return Lans.g("FormDatabaseMaintenance","Skipped. Local computer region must be set to Canada to run.");
			}
			StringBuilder stringBuilderLog=new StringBuilder();
			string command=@"SELECT CONCAT(patient.LName,', ',patient.FName) as PatName,claimproc.CodeSent,claim.DateSent,claim.ClaimType 
				FROM claimproc 
				INNER JOIN patient ON patient.PatNum=claimproc.PatNum
				INNER JOIN claim ON claim.ClaimNum=claimproc.ClaimNum
				WHERE claimproc.ClaimNum!=0 AND claimproc.ProcNum!=0 AND claimproc.Status!=4 AND claimproc.IsOverpay=0
				GROUP BY claimproc.ClaimNum,claimproc.ProcNum,claimproc.Status
				HAVING COUNT(*) > 1 
				ORDER BY claim.DateSent";
			DataTable tableDupeClaimProcs=Db.GetTable(command);
			if(tableDupeClaimProcs.Rows.Count==0 && !verbose) {
				return stringBuilderLog.ToString();
			}
			//There is something to report OR the user has verbose mode on.
			stringBuilderLog.Append(Lans.g("FormDatabaseMaintenance","ClaimProcs with partial duplicates found:")+" "+tableDupeClaimProcs.Rows.Count);
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(tableDupeClaimProcs.Rows.Count>0) {
						stringBuilderLog.AppendLine("\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down."));
					}
					break;
				case DbmMode.Breakdown:
					if(tableDupeClaimProcs.Rows.Count>0) {
						stringBuilderLog.AppendLine(", "+Lans.g("FormDatabaseMaintenance","including")+":");
						for(int i=0;i<tableDupeClaimProcs.Rows.Count;i++) {
							DataRow dataRow=tableDupeClaimProcs.Rows[i];
							stringBuilderLog.AppendLine(Lans.g("FormDatabaseMaintenance","  Patient:")+" "+PIn.String(dataRow["PatName"].ToString())+"  "
								+Lans.g("FormDatabaseMaintenance","Code Sent:")+" "+PIn.String(dataRow["CodeSent"].ToString())+"  "
								+Lans.g("FormDatabaseMaintenance","Claim Date:")+" "+PIn.DateT(dataRow["DateSent"].ToString()).ToShortDateString()+"  "
								+Lans.g("FormDatabaseMaintenance","Claim Type:")+" "+PIn.String(dataRow["ClaimType"].ToString()));
						}
						stringBuilderLog.AppendLine(Lans.g("FormDatabaseMaintenance","The above patients have duplicate claim procedures attached to claims. "
							+"You will need to manually navigate to the claims and delete the claim procedures you don't need."));
					}
					break;
			}
			return stringBuilderLog.ToString();
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string ClaimProcWriteOffNegative(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT patient.LName,patient.FName,patient.MiddleI,claimproc.CodeSent,procedurelog.ProcFee,procedurelog.ProcDate,claimproc.WriteOff
					FROM claimproc 
					LEFT JOIN patient ON claimproc.PatNum=patient.PatNum
					LEFT JOIN procedurelog ON claimproc.ProcNum=procedurelog.ProcNum 
					WHERE claimproc.WriteOff<0
					AND claimproc.IsTransfer=0 "+//The income transfer tool creates claimprocs with negative writeoffs. These are valid.
					"AND claimproc.Status!="+POut.Enum(ClaimProcStatus.Supplemental);//Ignore supplementals since these are allowed to be negative
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Negative writeoffs found: ")+table.Rows.Count+"\r\n";
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						
						log+=Lans.g("FormDatabaseMaintenance","List of patients with procedures that have negative writeoffs:\r\n");
						for(int i = 0;i<table.Rows.Count;i++) {
							string patientName=table.Rows[i]["LName"].ToString() + ", " + table.Rows[i]["FName"].ToString() + " " + table.Rows[i]["MiddleI"].ToString();
							string codeSent=table.Rows[i]["CodeSent"].ToString();
							DateTime procDate=PIn.Date(table.Rows[i]["ProcDate"].ToString());
							decimal writeOff=PIn.Decimal(table.Rows[i]["WriteOff"].ToString());
							decimal procFee=PIn.Decimal(table.Rows[i]["ProcFee"].ToString());
							log+=patientName+" "+codeSent+" fee:"+procFee.ToString("c")+" date:"+procDate.ToShortDateString()+" writeoff:"+writeOff.ToString("c")+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","Go to the patients listed above and manually correct the writeoffs.\r\n");
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsCanada=true,IsReplicationUnsafe=true)]
		public static string CanadaClaimProcForWrongPatient(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return Lans.g("FormDatabaseMaintenance","Skipped. Local computer region must be set to Canada to run.");
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			//Look at the comments for claimProc.DateEntry, if not set then the claimProc has no financial meaning yet.
			string command=@"SELECT claimproc.*
				FROM claimproc 
				INNER JOIN claim ON claim.ClaimNum=claimproc.ClaimNum
				WHERE (claimproc.PatNum!=claim.PatNum)";
			List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
			if(listClaimProcs.Count==0 && !verbose) {
				return "";
			}
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listClaimProcs.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs associated to wrong patient found")+": "+listClaimProcs.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i=0;i<listClaimProcs.Count;i++){
						ClaimProcs.Delete(listClaimProcs[i]);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,
						DbmLogActionType.Delete,methodName,"Deleted ClaimProc from CanadaClaimProcForWrongPatient.");
						listDbmLogs.Add(dbmLog);
					}
					Crud.DbmLogCrud.InsertMany(listDbmLogs);
					log+=Lans.g("FormDatabaseMaintenance","Claimprocs associated to wrong patient fixed")+":"+listClaimProcs.Count;
					break;
			}
			return log;
		}

		#endregion ClaimProc----------------------------------------------------------------------------------------------------------------------------
		#region Clearinghouse---------------------------------------------------------------------------------------------------------------------------
		[DbmMethodAttr(HasBreakDown = true)]
		public static string ClearinghouseInvalidFormat(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			int countFormatEnum=Enum.GetNames(typeof(ElectronicClaimFormat)).Length-1;
			string command="SELECT clearinghouse.Description,COALESCE(clinic.Abbr,'"+POut.String(Lans.g("FormDatabaseMaintenance","Unassigned"))+"') Abbr "
				+"FROM clearinghouse "
				+"LEFT JOIN clinic on clinic.ClinicNum=clearinghouse.ClinicNum "
				+"WHERE EFormat>"+countFormatEnum;
			DataTable table=Db.GetTable(command);
			int numFound=table.Rows.Count;
			if(numFound==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Clearinghouses with invalid Format found:")+" "+numFound;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(numFound>0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.");
					}
					break;
				case DbmMode.Breakdown:
					if(numFound>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="	"+table.Rows[i]["Description"].ToString();
							if(PrefC.HasClinicsEnabled) {
								log+=" "+Lans.g("FormDatabaseMaintenance","for Clinic")+": "+table.Rows[i]["Abbr"].ToString();
							}
							log+="\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","They need to be fixed manually.");
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true)]
		public static string ClearinghouseInvalidCommBridge(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			int countCommBridgeEnum=Enum.GetNames(typeof(EclaimsCommBridge)).Length - 1;
			string command="SELECT clearinghouse.Description,COALESCE(clinic.Abbr,'"+POut.String(Lans.g("FormDatabaseMaintenance","Unassigned"))+"') Abbr "
				+"FROM clearinghouse "
				+"LEFT JOIN clinic on clinic.ClinicNum=clearinghouse.ClinicNum "
				+"WHERE CommBridge>"+countCommBridgeEnum;
			DataTable table=Db.GetTable(command);
			int numFound=table.Rows.Count;
			if(numFound==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Clearinghouses with invalid CommBridge found:")+" "+numFound;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(numFound>0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.");
					}
					break;
				case DbmMode.Breakdown:
					if(numFound>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="	"+table.Rows[i]["Description"].ToString();
							if(PrefC.HasClinicsEnabled) {
								log+=" "+Lans.g("FormDatabaseMaintenance","for Clinic")+": "+table.Rows[i]["Abbr"].ToString();
							}
							log+="\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","They need to be fixed manually.");
					}
					break;
			}
			return log;
		}
		#endregion
		#region Clinic

		///<summary>Inserts missing/invalid clinics.</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true,HasExplain=true)]
		public static string ClinicNumMissingInvalid(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//look at the procedurelog and patient table because they will most likely have all possible clinics.
			string command=@"
				SELECT procedurelog.ClinicNum 
				FROM procedurelog 
				WHERE procedurelog.ClinicNum > 0 
				AND procedurelog.ClinicNum NOT IN (SELECT ClinicNum FROM clinic) 
				UNION 
				SELECT patient.ClinicNum 
				FROM patient
				WHERE patient.ClinicNum > 0 
				AND patient.ClinicNum NOT IN (SELECT ClinicNum FROM clinic) ";
			List<long> listInvalidClinicNums = Db.GetListLong(command);
			string log = "";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listInvalidClinicNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Clinics missing")+": "+listInvalidClinicNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listInvalidClinicNums.Count>0) {
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						string methodName=MethodBase.GetCurrentMethod().Name;
						for(int i=0;i<listInvalidClinicNums.Count;i++){
							command ="SELECT MAX(ItemOrder) FROM clinic";
							int itemOrder = Db.GetInt(command) + 1;
							Clinic clinicMissing = new Clinic();
							clinicMissing.ClinicNum = listInvalidClinicNums[i];
							clinicMissing.Description = "INVALID CLINIC #"+listInvalidClinicNums[i];
							clinicMissing.Abbr = "INVALID #"+listInvalidClinicNums[i];
							clinicMissing.ItemOrder = itemOrder;
							Clinics.Insert(clinicMissing,true);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,clinicMissing.ClinicNum,DbmLogFKeyType.Clinic,
								DbmLogActionType.Insert,methodName,"Inserted clinic from ClinicNumMissingInvalid.");
							listDbmLogs.Add(dbmLog);
						}
						if(listInvalidClinicNums.Count>0 || verbose) {
							log+=Lans.g("FormDatabaseMaintenance","Missing clinics added")+": "+listInvalidClinicNums.Count+"\r\n";
							Crud.DbmLogCrud.InsertMany(listDbmLogs);
						}
					}
					break;
			}
			return log;
		}

		#endregion
		#region ClockEvent, ComputerPref, Deposit, DiscountPlanSub, Disease, Document--------------------------------------------------------------------------------------------------

		[DbmMethodAttr]
		public static string ClockEventInFuture(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM clockevent WHERE TimeDisplayed1 > "+DbHelper.Now()+"+INTERVAL 15 MINUTE";
					int numFound=PIn.Int(Db.GetCount(command));
					command=@"SELECT COUNT(*) FROM clockevent WHERE TimeDisplayed2 > "+DbHelper.Now()+"+INTERVAL 15 MINUTE";
					numFound+=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Time card entries invalid")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"UPDATE clockevent SET TimeDisplayed1="+DbHelper.Now()+" WHERE TimeDisplayed1 > "+DbHelper.Now()+"+INTERVAL 15 MINUTE";
					int numberFixed=(int)Db.NonQ(command);
					command=@"UPDATE clockevent SET TimeDisplayed2="+DbHelper.Now()+" WHERE TimeDisplayed2 > "+DbHelper.Now()+"+INTERVAL 15 MINUTE";
					numberFixed+=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Future timecard entry times fixed")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string ComputerPrefDuplicates(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Min may not be the oldest when using random primary keys, but we have to pick one.
			string command="SELECT MIN(ComputerPrefNum) ComputerPrefNum, ComputerName "
				+"FROM computerpref "
				+"GROUP BY ComputerName "
				+"HAVING COUNT(*)>1 ";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ComputerPref duplicate computer name entries found:")+" "+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					if(table.Rows.Count>0) {
						command="DELETE FROM computerpref WHERE ComputerPrefNum NOT IN ("
							+string.Join(",",table.Select().Select(x => POut.Long(PIn.Long(x["ComputerPrefNum"].ToString()))))+") "
							+"AND ComputerName IN ("
							+string.Join(",",table.Select().Select(x => $"'{POut.String(PIn.String(x["ComputerName"].ToString()))}'"))+")";
						numberFixed=(int)Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ComputerPref duplicate computer name entries deleted:")+" "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		/*Deprecated per Nathan 4/18/2017.
		 * These do not show in the UI, so there is no way to manually fix them (aside from running a query to manually delete them).  Since they have no
		 * payments attached, they should not affect any reports or any other area of the program.  And since there is no fix Nathan decided we just
		 * wouldn't report them.  The only possible downside is if the table filled up with thousands or hundreds of thousands of these "bad" deposits,
		 * but we don't believe this will happen.  We believe we have fixed the bug that was creating this as of 4/18/2017.  FormDepositEdit was allowing
		 * empty deposits to be created as well as an issue with the Print/CreatePDF/Email buttons and the Refresh button causing transactions to be saved
		 * attached to the deposit, then when refreshed they were removed from the list and when OK was pressed the transactions were removed from the
		 * deposit but the deposit was left in the db with the amount but nothing attached.
		[DbmMethodAttr(HasBreakDown=true)]
		public static string DepositsWithNoPayments(bool verbose,DbmMode modeCur) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,modeCur);
			}
			//Gets all deposits that have a positive amount with no payments attached. This was caused by a bug in 16.2.1-16.2.8.
			command=@"SELECT deposit.DepositNum,deposit.DateDeposit,deposit.Amount
				FROM deposit 
				LEFT JOIN payment ON payment.DepositNum=deposit.DepositNum
				LEFT JOIN claimpayment ON claimpayment.DepositNum=deposit.DepositNum			
				WHERE payment.PayNum IS NULL AND claimpayment.ClaimPaymentNum IS NULL
				AND deposit.Amount>0.005
				ORDER BY deposit.DateDeposit,deposit.DepositNum";
			table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			int numFound=table.Rows.Count;
			string log=Lans.g("FormDatabaseMaintenance","Deposits found with no payments attached: ")+numFound;
			switch(modeCur) {
				case DbmMode.Check:
					if(numFound>0) {//Only the fix should show the entire list of items.
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
				case DbmMode.Fix:
					if(numFound>0) {//Running the fix and there are items to show.
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						foreach(DataRow row in table.Rows) {
							log+="   "+PIn.Date(row["DateDeposit"].ToString()).ToShortDateString()
								+"    "+PIn.Double(row["Amount"].ToString()).ToString("f")+"\r\n";
						}
						log+="   "+Lans.g("FormDatabaseMaintenance","They need to be deleted manually.")+"\r\n";
					}
					break;
			}
			return log;
		}*/

		///<summary>Finds deposits where there are attached payments and the deposit amount does not equal the sum of the attached payment amounts.</summary>
		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string DepositsWithIncorrectAmount(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			//only deposits with payments attached (INNER JOIN) where sum of payment amounts don't match the deposit amount
			//deposits with positive amount but no payments attached (LEFT JOIN...) is handled in a separate DBM above
			string command=@"SELECT deposit.DepositNum,deposit.Amount,deposit.DateDeposit,SUM(payments.amt) _sum
				FROM deposit
				INNER JOIN (
					SELECT payment.DepositNum,payment.PayAmt amt
					FROM payment
					UNION ALL
					SELECT claimpayment.DepositNum,claimpayment.CheckAmt amt
					FROM claimpayment
				) payments ON payments.DepositNum=deposit.DepositNum
				GROUP BY deposit.DepositNum
				HAVING ROUND(_sum,2) != ROUND(deposit.Amount,2)
				ORDER BY deposit.DateDeposit,deposit.DepositNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			//There is something to report OR the user has verbose mode on.
			string log=Lans.g("FormDatabaseMaintenance","Deposit sums found incorrect: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							DateTime date=PIn.Date(table.Rows[i]["DateDeposit"].ToString());
							Double valOld=PIn.Double(table.Rows[i]["Amount"].ToString());
							Double valNew=PIn.Double(table.Rows[i]["_sum"].ToString());
							log+="   "+Lans.g("FormDatabaseMaintenance","Deposit Date: ")+date.ToShortDateString()
								+", "+Lans.g("FormDatabaseMaintenance","Current Sum: ")+valOld.ToString("c")
								+", "+Lans.g("FormDatabaseMaintenance","Expected Sum:")+valNew.ToString("c")+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Finds DiscountPlanSubs where the DiscountPlanNum is not linked to an existing DiscountPlan.</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string DiscountPlanSubWithInvalidDiscountPlanNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			//Get the number of discountplansubs with invalid discount plans.
			string command=@"SELECT DiscountSubNum
				FROM discountplansub 
				WHERE DiscountPlanNum 
				NOT IN (SELECT DiscountPlanNum FROM discountplan)";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","DiscountPlanSubs with invalid discount plan found")+$": {numFound}";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					List<long> listDiscountSubNums=Db.GetListLong(command);
					if(listDiscountSubNums.Count==0) {
						break;
					}
					//Delete and log discountplansubs with invalid discount plan.
					command=@$"DELETE FROM discountplansub 
						WHERE DiscountSubNum IN ({string.Join(",",listDiscountSubNums.Select(x => x.ToString()))})";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listDiscountSubNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listDiscountSubNums[i],DbmLogFKeyType.DiscountPlanSub,DbmLogActionType.Delete,
							MethodBase.GetCurrentMethod().Name,"Deleted DiscountPlanSub from DiscountPlanSubWithInvalidDiscountPlanNum");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed > 0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","DiscountPlanSubs with invalid discount plan deleted")+$": {numberFixed}";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string DiseaseWithInvalidDiseaseDef(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			string command=@"SELECT DiseaseNum,DiseaseDefNum FROM disease WHERE DiseaseDefNum NOT IN(SELECT DiseaseDefNum FROM diseasedef)";
			DataTable table=Db.GetTable(command);
			int numFound=table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Problems with invalid references found")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Problems with invalid references found")+": "+numFound+"\r\n";
					}
					if(numFound > 0) {
						//Check to see if there is already a diseasedef called UNKNOWN PROBLEM.
						command="SELECT DiseaseDefNum FROM diseasedef WHERE DiseaseName='UNKNOWN PROBLEM'";
						long diseaseDefNum=PIn.Long(Db.GetScalar(command));
						if(diseaseDefNum==0) {
							//Create a new DiseaseDef called UNKNOWN PROBLEM.
							DiseaseDef diseaseDef=new DiseaseDef();
							diseaseDef.DiseaseName="UNKNOWN PROBLEM";
							diseaseDef.IsHidden=false;
							diseaseDefNum=DiseaseDefs.Insert(diseaseDef);
						}
						//Update the disease table.
						command="UPDATE disease SET DiseaseDefNum="+POut.Long(diseaseDefNum)+" WHERE DiseaseNum IN("
							+string.Join(",",table.Select().Select(x => PIn.Long(x["DiseaseNum"].ToString())))+")";
						Db.NonQ(command);
						log+=Lans.g("FormDatabaseMaintenance","All invalid references have been attached to the problem called")+" UNKNOWN PROBLEM.\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string DocumentWithInvalidDate(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Gets a list of documents with dates that are invalid (0001-01-01). The list should be blank. If not, then the document's date will be set to 001-01-02 which will allow deletion.
			string command="SELECT COUNT(*) FROM document WHERE DateCreated="+POut.Date(new DateTime(1,1,1));
			int numFound=PIn.Int(Db.GetCount(command));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Documents with invalid dates found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(numFound>0) {
						command="UPDATE document SET DateCreated="+POut.Date(new DateTime(1,1,2))+" WHERE DateCreated="+POut.Date(new DateTime(1,1,1));
						Db.NonQ(command);
					}
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Documents with invalid dates fixed")+": "+numFound.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string DocumentWithNoCategory(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT DocNum FROM document WHERE DocCategory=0";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Images with no category found: ")+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
					for(int i = 0;i<table.Rows.Count;i++) {
						command="UPDATE document SET DocCategory="+POut.Long(listDefs[0].DefNum)
							+" WHERE DocNum="+table.Rows[i][0].ToString();
						Db.NonQ(command);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Images with no category fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion ClockEvent, Deposit, Disease, Document-----------------------------------------------------------------------------------------------
		#region Ebill, EClipboard, EduResource, EmailAttach, Etrans-------------------------------------------------------------------------------------------------

		[DbmMethodAttr]
		public static string EbillDuplicates(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Min may not be the oldest when using random primary keys, but we have to pick one.  In most cases they're identical anyway.
			string command="SELECT MIN(EbillNum) EbillNum,COUNT(*) Count "
				+"FROM ebill "
				+"GROUP BY ClinicNum ";
			DataTable tableEbills=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=tableEbills.Select().Select(x => PIn.Int(x["Count"].ToString())-1).Sum();//count duplicates=Sum(# per group-1)
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ebill duplicate clinic entries found: ")
							+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					if(tableEbills.Rows.Count>0) {
						command="DELETE FROM ebill WHERE EbillNum NOT IN ("
							+string.Join(",",tableEbills.Select().Select(x => PIn.Long(x["EbillNum"].ToString())))+")";
						numberFixed=(int)Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ebill duplicate clinic entries deleted: ")
							+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Inserts an ebill for ClinicNum 0 if it does not exist.</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string EbillMissingDefaultEntry(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT COUNT(*) FROM ebill WHERE ClinicNum=0";
			int numFound=PIn.Int(Db.GetCount(command));
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound==0) {
						log+=Lans.g("FormDatabaseMaintenance","Missing default ebill entry.");
					}
					break;
				case DbmMode.Fix:
					if(numFound==0) {
						Ebill ebill=new Ebill();
						ebill.ClinicNum=0;
						ebill.ClientAcctNumber="";
						ebill.ElectUserName="";
						ebill.ElectPassword="";
						ebill.PracticeAddress=EbillAddress.PracticePhysical;
						ebill.RemitAddress=EbillAddress.PracticeBilling;
						long ebillNum=OpenDentBusiness.Crud.EbillCrud.Insert(ebill);
						if(ebillNum>0) {
							log+=Lans.g("FormDatabaseMaintenance","Default ebill entry inserted.");
						}
					}
					break;
			}
			return log;
		}

		///<summary>Deletes orphaned sheetdefs from eClipboard.</summary>
		[DbmMethodAttr]
		public static string EClipboardOrphanedSheetDefRow(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//MySQL error without nested select
			string command=$@"
				SELECT EClipboardSheetDefNum 
					FROM eclipboardsheetdef 
					LEFT JOIN sheetdef 
						ON sheetdef.SheetDefNum=eclipboardsheetdef.SheetDefNum 
					WHERE sheetdef.SheetDefNum IS NULL
					OR sheetdef.HasMobileLayout=0;";
			List<long> listSheetNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listSheetNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid eClipboard sheets found")+": "+listSheetNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listSheetNums.Count>0) {
						command=$@"
							DELETE 
							FROM eclipboardsheetdef
							WHERE EClipboardSheetDefNum IN({string.Join(",",listSheetNums.Select(POut.Long))})";
						Db.NonQ(command);
					}
					if(listSheetNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid eClipbaord sheets removed")+": "+listSheetNums.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>This could be enhanced to validate all foreign keys on the eduresource.</summary>
		[DbmMethodAttr]
		public static string EduResourceInvalidDiseaseDefNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT EduResourceNum FROM eduresource WHERE DiseaseDefNum != 0 AND DiseaseDefNum NOT IN (SELECT DiseaseDefNum FROM diseasedef)";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","EHR Educational Resources with invalid problem found: ")+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT DiseaseDefNum FROM diseasedef WHERE ItemOrder=(SELECT MIN(ItemOrder) FROM diseasedef WHERE IsHidden=0)";
					long diseaseDefNum=PIn.Long(Db.GetScalar(command));
					for(int i = 0;i<table.Rows.Count;i++) {
						command="UPDATE eduresource SET DiseaseDefNum='"+diseaseDefNum+"' WHERE EduResourceNum='"+table.Rows[i][0].ToString()+"'";
						Db.NonQ(command);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","EHR Educational Resources with invalid problem fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string EmailAttachWithTemplateNumAndMessageNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM emailattach WHERE emailattach.EmailTemplateNum!=0 AND emailattach.EmailMessageNum!=0";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Email attachments attached to both an email and a template found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE emailattach SET EmailTemplateNum=0 WHERE emailattach.EmailTemplateNum!=0 AND emailattach.EmailMessageNum!=0";
					int numFixed=(int)Db.NonQ(command);
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Email attachments attached to both an email and a template fixed")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string Etrans835MultipleForEtrans(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM etrans WHERE etrans.EtransNum IN ("
				+"SELECT etrans835.EtransNum FROM etrans835 GROUP BY etrans835.EtransNum HAVING Count(*) > 1)";
			List<Etrans> listEtranses=Crud.EtransCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					if(listEtranses.Count>0 || verbose) {
						log=Lans.g("FormDatabaseMaintenance","Etrans with multiple Etrans835s")+": "+listEtranses.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listEtranses.Count>0 || verbose) {
						int countDeleted=0;
						for(int i=0;i<listEtranses.Count;i++) {
							string messageText835=EtransMessageTexts.GetMessageText(listEtranses[i].EtransMessageTextNum);
							X835 x835=new X835(listEtranses[i],messageText835,listEtranses[i].TranSetId835,null);
							List<string> listPatNames=x835.ListClaimsPaid.Select(x => x.PatientName.ToString()).Distinct().ToList();
							//Mimics how the patient name is constructed in Etrans835s.Upsert()
							string patientName="";
							if(listPatNames.Count>0){
								patientName=listPatNames[0];
							}
							if(listPatNames.Count>1) {
								patientName="("+POut.Long(listPatNames.Count)+")";
							}
							List<Etrans835> listEtrans835s=Etrans835s.GetByEtransNums(listEtranses[i].EtransNum);
							for(int j=listEtrans835s.Count-1;j>=0;j--) {
								if(listEtrans835s[j].PayerName==x835.PayerName
									&& listEtrans835s[j].TransRefNum==x835.TransRefNum
									&& listEtrans835s[j].InsPaid==(double)x835.InsPaid
									&& listEtrans835s[j].ControlId==x835.ControlId
									&& listEtrans835s[j].PaymentMethodCode==x835.PaymentMethodCode
									&& listEtrans835s[j].PatientName==patientName
									//We don't want to include status because claims may have changed, and the Etrans835 status may not be up to date.
									/*&& listEtrans835s[j].Status==x835.GetStatus()*/)
								{
									//Remove the first match that we find so we don't delete it.
									listEtrans835s.RemoveAt(j);
									break;
								}
							}
							List<long> listEtrans835Nums=listEtrans835s.Select(x => x.Etrans835Num).ToList();
							Crud.Etrans835Crud.DeleteMany(listEtrans835Nums);
							countDeleted+=listEtrans835Nums.Count;
						}
						log=Lans.g("FormDatabaseMaintenance","Invalid Etrans835s deleted")+": "+countDeleted.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string Etrans835AttachWithInvalidClaimNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			List<long> listEtrans835AttachNums=Db.GetListLong(
				@"SELECT etrans835attach.Etrans835AttachNum
				FROM etrans835attach
				LEFT JOIN claim ON claim.ClaimNum=etrans835attach.ClaimNum
				WHERE etrans835attach.ClaimNum!=0
				AND claim.ClaimNum IS NULL"
			);
			switch(dbmMode) {
				case DbmMode.Check:
					if(listEtrans835AttachNums.Count>0 || verbose) {
						log=Lans.g("FormDatabaseMaintenance","Etrans835Attaches with an invalid claimNum")+": "+listEtrans835AttachNums.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					Etrans835Attaches.DeleteMany(listEtrans835AttachNums);
					if(listEtrans835AttachNums.Count>0 || verbose) {
						log=Lans.g("FormDatabaseMaintenance","Etrans835Attaches with an invalid claimNum deleted")+": "+listEtrans835AttachNums.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Deletes both the etrans requests and responses when the response transaction type is a request type (obviously invalid). Also deletes ROT requests and responses when the response message text is a blank string which are internal errors of some kind which also caused the same problem as a request message type being returned in the response message text. This DBM method is geared towards only preserving valid ROT error responses that follow message standards.</summary>
		[DbmMethodAttr(IsCanada=true)]
		public static string EtransInvalidRotResponses(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			//Identify the invalid ROT responses that gave back a copy of our request payload (or at least gave back a response transaction type message).
			//Outstanding Transaction Request messages are of type 04 - EtransType.RequestOutstand_CA (enum value 10).
			//Any responses to said request messages that are of type 04 are obviously invalid since a request message type cannot be used as a response.
			//Therefore, we need to find all etrans records of type 'RequestOutstand_CA' that have an etrans response type of 'AckError'.
			//The etrans requests and responses are safe to delete if the response has an etransmessagetext.MessageText with a type 04 request in it.
			//The 21st and 22nd characters in the response message will be exactly '04' if they responded with a request transaction message type.
			//Also, etrans requests and responses can be deleted when the etrans response has a blank etransmessagetext.MessageText value.
			//Blank AckError responses mean that there was an internal error of some kind (e.g. could not read / write to clearinghouse path, etc).
			string command=$@"SELECT eRequest.EtransNum RequestEtransNum,eResponse.EtransNum ResponseEtransNum,
					eRequest.EtransMessageTextNum RequestMsgTextNum,eResponse.EtransMessageTextNum ResponseMsgTextNum
				FROM etrans eRequest
				INNER JOIN etrans eResponse ON eRequest.AckEtransNum=eResponse.EtransNum 
					AND eRequest.Etype={POut.Enum(EtransType.RequestOutstand_CA)}
				INNER JOIN etransmessagetext eResponseMsgText ON eResponse.EtransMessageTextNum=eResponseMsgText.EtransMessageTextNum
					AND (eResponseMsgText.MessageText='' OR SUBSTRING(eResponseMsgText.MessageText,21,2)='04')
				WHERE eResponse.Etype={POut.Enum(EtransType.AckError)}";
			DataTable table=Db.GetTable(command);
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log=Lans.g("FormDatabaseMaintenance","Etrans Outstanding Transaction Requests with invalid responses")+$": {table.Rows.Count}";
					}
					break;
				case DbmMode.Fix:
					List<long> listEtransNums=new List<long>();
					List<long> listEtransMsgTxtNums=new List<long>();
					for(int i=0;i<table.Rows.Count;i++) {
						listEtransNums.Add(PIn.Long(table.Rows[i]["RequestEtransNum"].ToString()));
						listEtransNums.Add(PIn.Long(table.Rows[i]["ResponseEtransNum"].ToString()));
						listEtransMsgTxtNums.Add(PIn.Long(table.Rows[i]["RequestMsgTextNum"].ToString()));
						listEtransMsgTxtNums.Add(PIn.Long(table.Rows[i]["ResponseMsgTextNum"].ToString()));
					}
					if(!listEtransNums.IsNullOrEmpty()) {
						command=$"DELETE FROM etrans WHERE EtransNum IN ({string.Join(",",listEtransNums.Select(x => POut.Long(x)))})";
						Db.NonQ(command);
					}
					if(!listEtransMsgTxtNums.IsNullOrEmpty()) {
						command="DELETE FROM etransmessagetext "
							+$"WHERE EtransMessageTextNum IN ({string.Join(",",listEtransMsgTxtNums.Select(x => POut.Long(x)))})";
						Db.NonQ(command);
					}
					if(table.Rows.Count>0 || verbose) {
						log=Lans.g("FormDatabaseMaintenance","Etrans Outstanding Transaction Requests with invalid responses deleted")+$": {table.Rows.Count}";
					}
					break;
			}
			return log;
		}

		#endregion Ebill, EduResource, EmailAttach, Etrans------------------------------------------------------------------------------------------------
		#region Fee, FeeSchedule, GroupNote, GroupPermission----------------------------------------------------------------------------------------------

		[DbmMethodAttr(HasBreakDown=true,IsReplicationUnsafe=true)]
		public static string FeeDeleteDuplicates(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT FeeNum,FeeSched,CodeNum,Amount,ClinicNum,ProvNum FROM fee "
				+"GROUP BY FeeSched,CodeNum,ClinicNum,ProvNum HAVING COUNT(CodeNum)>1";
			DataTable table=Db.GetTable(command);
			int count=table.Rows.Count;
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with duplicate fee entries:")+" "+count+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					StringBuilder stringBuilder=new StringBuilder();
					stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","The following procedure codes have duplicate fee entries."
						+"  Verify that the following amounts are correct:"));
					for(int i=0;i<table.Rows.Count;i++){
						stringBuilder.AppendLine("Fee Schedule: "+FeeScheds.GetDescription(PIn.Long(table.Rows[i]["FeeSched"].ToString()))//No call to db.
							+" - Code: "+ProcedureCodes.GetStringProcCode(PIn.Long(table.Rows[i]["CodeNum"].ToString()))//No call to db.
							+" - Amount: "+PIn.Double(table.Rows[i]["Amount"].ToString()).ToString("n"));
					}
					if(count>0 || verbose) {
						log+=stringBuilder.ToString();
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i=0;i<count;i++) {
						//At least one fee needs to stay.  Each row in table is a random fee, so we'll just keep that one and delete the rest.
						command="SELECT FeeNum FROM fee WHERE FeeSched="+table.Rows[i]["FeeSched"].ToString()
							+" AND CodeNum="+table.Rows[i]["CodeNum"].ToString()
							+" AND ClinicNum="+table.Rows[i]["ClinicNum"].ToString()
							+" AND ProvNum="+table.Rows[i]["ProvNum"].ToString()
							+" AND FeeNum!="+table.Rows[i]["FeeNum"].ToString();//This is the random fee we will keep.
						List<long> listFeeNums=Db.GetListLong(command);
						Fees.DeleteMany(listFeeNums);
						for(int j=0;j<listFeeNums.Count;j++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listFeeNums[j],DbmLogFKeyType.Fee,DbmLogActionType.Delete,
								methodName,"Deleted fee from FeeDeleteDuplicates.");
							listDbmLogs.Add(dbmLog);
						}
						numberFixed+=listFeeNums.Count;
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Duplicate fees deleted")+": "+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string FeeDeleteForInvalidProc(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command=@"SELECT FeeNum,FeeSched,fee.CodeNum AS 'CodeNum' FROM fee 
									LEFT JOIN procedurecode ON fee.CodeNum=procedurecode.CodeNum 
									WHERE procedurecode.CodeNum IS NULL";
			DataTable table=Db.GetTable(command);
			int count=table.Rows.Count;
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with invalid procedure codes: ")+count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i=0;i<count;i++) {
						command="SELECT FeeNum FROM fee WHERE FeeNum="+table.Rows[i]["FeeNum"].ToString();
						List<long> listFeeNums=Db.GetListLong(command);
						Fees.DeleteMany(listFeeNums);
						for(int j=0;j<listFeeNums.Count;j++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listFeeNums[j],DbmLogFKeyType.Fee,DbmLogActionType.Delete,
								methodName,"Deleted fee from FeeDeleteForInvalidProc.");
							listDbmLogs.Add(dbmLog);
						}
						numberFixed+=listFeeNums.Count;
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Fees with invalid procedure codes deleted")+": "+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string GroupNoteWithInvalidAptNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"WHERE procedurelog.AptNum!=0 AND procedurecode.ProcCode='~GRP~'";
			List<Procedure> listProcedures=Crud.ProcedureCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listProcedures.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Group notes attached to appointments: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="UPDATE procedurelog SET AptNum=0 "
						+"WHERE AptNum!=0 AND CodeNum IN(SELECT CodeNum FROM procedurecode WHERE procedurecode.ProcCode='~GRP~')";
					int numfixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcedures.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcedures[i].ProcNum,DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Updated AptNum from "+listProcedures[i].AptNum+" to 0 from GroupNoteWithInvalidAptNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numfixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Group notes attached to appointments fixed: ")+numfixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string GroupNoteWithInvalidProcStatus(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT * FROM procedurelog "
				+"INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum "
				+"WHERE procedurelog.ProcStatus NOT IN("+POut.Int((int)ProcStat.D)+","+POut.Int((int)ProcStat.EC)+") "
				+"AND procedurecode.ProcCode='~GRP~'";
			List<Procedure> listProcedures=Crud.ProcedureCrud.SelectMany(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listProcedures.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Group notes with invalid status: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="UPDATE procedurelog SET ProcStatus="+POut.Long((int)ProcStat.EC)+" "
						+"WHERE ProcStatus NOT IN("+POut.Int((int)ProcStat.D)+","+POut.Int((int)ProcStat.EC)+") "
						+"AND CodeNum IN(SELECT CodeNum FROM procedurecode WHERE procedurecode.ProcCode='~GRP~')";
					int numfixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcedures.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcedures[i].ProcNum,DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Updated ProcStatus from "+listProcedures[i].ProcStatus+" to "+POut.String(ProcStat.EC.ToString())
							+" from GroupNoteWithInvalidProcStatus.");
						listDbmLogs.Add(dbmLog);
					}
					if(numfixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Group notes statuses fixed: ")+numfixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string FeeScheduleHiddenWithPatient(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT DISTINCT(FeeSched) FROM patient WHERE PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			List<long> listPatFeeSchedNums=Db.GetListLong(command);
			List<long> listFeeSchedNums=new List<long>();
			if(listPatFeeSchedNums.Count>0) {
				command="SELECT DISTINCT(FeeSchedNum) FROM feesched WHERE IsHidden=1 AND FeeSchedNum IN ("+string.Join(",",listPatFeeSchedNums)+");";
				listFeeSchedNums=Db.GetListLong(command);
			}
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listFeeSchedNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Hidden Fee Schedules associated to patients: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					int numFixed=0;
					if(listFeeSchedNums.Count>0) {
						command="UPDATE feesched SET IsHidden=0 WHERE FeeSchedNum IN ("+string.Join(",",listFeeSchedNums)+");";
						numFixed=(int)Db.NonQ(command);
						for(int i=0;i<listFeeSchedNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listFeeSchedNums[i],DbmLogFKeyType.FeeSched,DbmLogActionType.Update,
								methodName,"Updated IsHidden from 1 to 0 from FeeScheduleHiddenWithPatient.");
							listDbmLogs.Add(dbmLog);
						}
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Hidden Fee Schedules associated to patients unhidden: ")+numFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string GroupPermissionInvalidNewerDays(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=$"SELECT COUNT(GroupPermNum) FROM grouppermission WHERE NewerDays<0 OR NewerDays>{GroupPermissions.NewerDaysMax}";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Group permissions with invalid NewerDays found: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					string methodName=MethodBase.GetCurrentMethod().Name;
					command=$@"UPDATE grouppermission 
						SET NewerDays=(CASE WHEN NewerDays<0 THEN 0 ELSE {GroupPermissions.NewerDaysMax} END) 
						WHERE NewerDays<0 OR NewerDays>{GroupPermissions.NewerDaysMax}";
					int numfixed=(int)Db.NonQ(command);
					if(numfixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Group permissions with invalid NewerDays fixed: ")+numfixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion Fee, FeeSchedule, GroupNote----------------------------------------------------------------------------------------------------------
		#region Icd9
		[DbmMethodAttr]
		public static string Icd9InvalidCodes(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			List<long> listIcd9Nums=Db.GetListLong("SELECT ICD9Num FROM icd9 WHERE ICD9Code=''");
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listIcd9Nums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid ICD9 codes found")+": "+listIcd9Nums.Count+"\r\n";
					}
				break;
				case DbmMode.Fix:
					if(listIcd9Nums.Count>0) {
						Db.NonQ("DELETE FROM icd9 WHERE ICD9Num IN("+String.Join(",",listIcd9Nums)+")");
					}
					int numberFixed=listIcd9Nums.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid ICD9 codes removed")+": "+numberFixed.ToString()+"\r\n";
					}
				break;
			}
			return log;
		}
		#endregion

		#region InsPayPlan, InsPlan, InsSub-------------------------------------------------------------------------------------------------------------
		[DbmMethodAttr(IsOneOff=true)]
		public static string InsPayPlanWithNonZeroGuarantor(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Gets a list of payplans of type insurance that have a guarantor that is greater than 0 
			//Insurance Payment Plans gathered based on PlanNum and InsSubNum, which should only be populated for non-Insurance Payment Plans
			//as per our Database Documentation.
			string command= @"SELECT PayPlanNum FROM payplan WHERE PlanNum!=0";
			List<long> listPayPlanNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listPayPlanNums.Count>0||verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ins payment plans with guarantors set")+": "+listPayPlanNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(listPayPlanNums.Count>0) {
						//Set the guarantors in the listPayPlanNums to 0
						command="UPDATE payplan SET Guarantor=0 WHERE PayPlanNum IN("+String.Join(",",listPayPlanNums)+")";
						Db.NonQ(command);
						for(int i=0;i<listPayPlanNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPayPlanNums[i],DbmLogFKeyType.PayPlan,DbmLogActionType.Update,
								methodName,"Updated Guarantor to 0 from InsPayPlanWithNonZeroGuarantor.");
							listDbmLogs.Add(dbmLog);
						}
					}
					int numberFixed=listPayPlanNums.Count;
					if(numberFixed>0||verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ins payment plans with guarantors set are now 0")+": "+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string InsPayPlanWithPatientPayments(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Gets a list of payplans of type insurance that have patient payments attached to them and no insurance payments attached
			string command=@"SELECT payplan.PayPlanNum 
								FROM payplan
								INNER JOIN paysplit ON paysplit.PayPlanNum=payplan.PayPlanNum
								LEFT JOIN claimproc ON claimproc.PayPlanNum=payplan.PayPlanNum
									AND claimproc.Status IN("
										+POut.Int((int)ClaimProcStatus.Received)+","
										+POut.Int((int)ClaimProcStatus.Supplemental)+","
										+POut.Int((int)ClaimProcStatus.CapClaim)+") "+
							@"WHERE payplan.PlanNum!=0
								AND claimproc.ClaimProcNum IS NULL
								GROUP BY payplan.PayPlanNum";
			List<long> listPayPlanNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listPayPlanNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ins payment plans with patient payments attached")+": "+listPayPlanNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(listPayPlanNums.Count>0) {
						//Change the insurance payment plan to a patient payment plan so that the payments will be visible within the payment plan
						command="UPDATE payplan SET PlanNum=0 WHERE PayPlanNum IN("+String.Join(",",listPayPlanNums)+")";
						Db.NonQ(command);
						for(int i=0;i<listPayPlanNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPayPlanNums[i],DbmLogFKeyType.PayPlan,DbmLogActionType.Update,
								methodName,"Updated PlanNum to 0 from InsPayPlanWithPatientPayments.");
							listDbmLogs.Add(dbmLog);
						}
					}
					int numberFixed=listPayPlanNums.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ins payment plans with patient payments attached fixed")+": "+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true,IsReplicationUnsafe=true)]
		public static string InsPlanInvalidCarrier(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Gets a list of insurance plans that do not have a carrier attached. The list should be blank. If not, then you need to go to the plan listed and add a carrier. Missing carriers will cause the send claims function to give an error.
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT PlanNum FROM insplan WHERE CarrierNum NOT IN (SELECT CarrierNum FROM carrier)";
					DataTable table=Db.GetTable(command);
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ins plans with carrier missing found: ")+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					command="SELECT PlanNum, CarrierNum FROM insplan WHERE CarrierNum NOT IN (SELECT CarrierNum FROM carrier)";
					DataTable tableInsPlan=Db.GetTable(command);
					List<InsPlan> listInsPlans=new List<InsPlan>();
					for(int i=0;i<tableInsPlan.Rows.Count;i++) {
						InsPlan insPlan=new InsPlan();
						insPlan.CarrierNum=PIn.Long(tableInsPlan.Rows[i]["CarrierNum"].ToString());
						insPlan.PlanNum=PIn.Long(tableInsPlan.Rows[i]["PlanNum"].ToString());
						listInsPlans.Add(insPlan);
					}
					List<long> listCarrierNumsDist=listInsPlans.Select(x => x.CarrierNum).Distinct().ToList();
					if(tableInsPlan.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid Carriers Referenced: ")+listCarrierNumsDist.Count.ToString()+"\r\n";
						log+=Lans.g("FormDatabaseMaintenance","Ins Plans Affected: ")+listInsPlans.Count.ToString()+"\r\n\r\n";
						for(int i=0;i<listCarrierNumsDist.Count;i++){
							log+="   "+Lans.g("FormDatabaseMaintenance","CarrierNum")+": "+listCarrierNumsDist[i]+"\r\n";
							for(int j=0;j<listInsPlans.Count;j++){
								if(listCarrierNumsDist[i]!=listInsPlans[j].CarrierNum){
									continue;
								}
								log+="    PlanNum:"+listInsPlans[j].PlanNum.ToString()+"\r\n";
							}
							log+="\r\n";
						}
					}
					break;
				case DbmMode.Fix:
					command="SELECT PlanNum,CarrierNum FROM insplan WHERE CarrierNum NOT IN (SELECT CarrierNum FROM carrier)";
					table=Db.GetTable(command);
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					if(table.Rows.Count>0) {
						long carrierNum0=0;//The new CarrierNum for any plans that have 0 for their CarrierNum.
						List<long> listCarrierNums=table.Select().Select(x => PIn.Long(x["CarrierNum"].ToString())).Distinct().ToList();
						foreach(long carrierNum in listCarrierNums) {
							if(carrierNum<=0 && carrierNum0!=0) {
								continue;//We'll only insert one carrier for all carrier nums equal or less than 0.
							}
							Carrier carrier=new Carrier {
								CarrierName="UNKNOWN CARRIER "+Math.Max(carrierNum,0),
								CarrierNum=carrierNum,
							};
							Carriers.Insert(carrier,useExistingPriKey:carrierNum > 0);
							if(carrierNum<=0) {
								carrierNum0=carrier.CarrierNum;
							}
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,carrierNum,DbmLogFKeyType.Carrier,DbmLogActionType.Insert,methodName,
								"Created new carrier '"+carrier.CarrierName+"' from InsPlanInvalidCarrier.");
							listDbmLogs.Add(dbmLog);
						}
						if(carrierNum0!=0) {//If we had any plans with CarrierNum of 0
							command="UPDATE insplan SET CarrierNum="+POut.Long(carrierNum0)//set this new carrier for the insplans
								+" WHERE CarrierNum<=0";
							Db.NonQ(command);
							List<DataRow> listDataRow=table.Select().Where(x => PIn.Long(x["CarrierNum"].ToString())<=0).ToList();
							for(int i = 0;i<listDataRow.Count;i++) {
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,PIn.Long(listDataRow[i]["PlanNum"].ToString()),DbmLogFKeyType.InsPlan,
									DbmLogActionType.Update,methodName,"Updated CarrierNum from "+listDataRow[i]["CarrierNum"].ToString()+" to "+carrierNum0
									+" from InsPlanInvalidCarrier.");
								listDbmLogs.Add(dbmLog);
							}
						}
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Ins plans with carrier missing fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}
		
		[DbmMethodAttr(HasBreakDown = true,HasPatNum = true,IsReplicationUnsafe=true)]
		public static string InsPlanInvalidNum(bool verbose,DbmMode dbmMode,long patNum = 0) {
			//Many sections removed because they are now fixed in InsSubNumMismatchPlanNum.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			bool isPatSpecific=false;
			List<long> listPatPlanNums=new List<long>();
			List<long> listInsSubNums=new List<long>();
			List<long> listPlanNums=new List<long>();
			if(patNum > 0) {
				isPatSpecific=true;
				//Benefits need to check against PlanNums and PatPlanNums
				List<PatPlan> listPatPlans=PatPlans.Refresh(patNum);
				List<InsSub> listInsSubs=InsSubs.GetListForSubscriber(patNum);
				if(listPatPlans.Count < 1) {
					if(listInsSubs.Count < 1) {
						return Lans.g("FormDatabaseMaintenance","No insurance plans on file for patient.  Run the full DBM in order to fix any problems.");
					}
					listInsSubNums=listInsSubs.Select(x => x.InsSubNum).ToList();
					listPlanNums=listInsSubs.Select(x => x.PlanNum).ToList();
				}
				else {//PatPlans in the database
					listPatPlanNums=listPatPlans.Select(x => x.PatPlanNum).ToList();
					//Patients could have orphaned ins subs in the database, make sure to include those as well.
					listInsSubNums=listPatPlans.Select(x => x.InsSubNum)
						.Union(listInsSubs.Select(x => x.InsSubNum))
						.Distinct().ToList();
					listPlanNums=InsSubs.GetMany(listInsSubNums).Select(x => x.PlanNum).ToList();
				}
				if(listInsSubNums.Count < 1) {
					return Lans.g("FormDatabaseMaintenance","This patient has insurance plans that cannot be fixed on a patient specific level.\r\n"
						+"PatPlans: "+listPatPlanNums.Count+"  InsSubs: "+listInsSubNums.Count+"  PlanNums: "+listPlanNums.Count
						+"Run the full DBM in order to fix these problems.");
				}
			}
			switch(dbmMode) {
				case DbmMode.Check:
					#region CHECK
					string command="SELECT COUNT(*) FROM appointment "
						+"WHERE appointment.InsPlan1 != 0 ";
					if(isPatSpecific){
						command+="AND appointment.PatNum="+POut.Long(patNum)+" ";
					}
					command+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=appointment.InsPlan1)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid appointment InsPlan1 values: ")+numFound+"\r\n";
					}
					command="SELECT COUNT(*) FROM appointment "
						+"WHERE appointment.InsPlan2 != 0 ";
					if(isPatSpecific){
						command+="AND appointment.PatNum="+POut.Long(patNum)+" ";
					}
					command+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=appointment.InsPlan2)";
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid appointment InsPlan2 values: ")+numFound+"\r\n";
					}
					command="SELECT COUNT(*) FROM benefit "
						+"WHERE PlanNum !=0 ";
					if(isPatSpecific) {
						if(listPlanNums.Count > 0) {
							command+="AND PlanNum IN ("+string.Join(",",listPlanNums)+") ";
						}
						else {
							command+="AND FALSE ";
						}
					}
					command+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=benefit.PlanNum)";
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid benefit PlanNums: ")+numFound+"\r\n";
					}
					command="SELECT COUNT(*) FROM inssub WHERE ";
					if(isPatSpecific){
						command+="InsSubNum IN("+string.Join(",",listInsSubNums)+") AND ";
					}
					command+="NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=inssub.PlanNum)";
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid inssub PlanNums: ")+numFound+"\r\n";
					}
					#endregion CHECK
					break;
				case DbmMode.Breakdown:
					#region BREAKDOWN
					command="SELECT PatNum, AptNum, InsPlan1 FROM appointment "
						+"WHERE appointment.InsPlan1 != 0 ";
					if(isPatSpecific){
						command+="AND appointment.PatNum="+POut.Long(patNum)+" ";
					}
					command+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=appointment.InsPlan1)";
					DataTable tableAppt=Db.GetTable(command);
					List<Appointment> listAppointmentsInvalidInsPlan1=new List<Appointment>();
					for(int i = 0;i<tableAppt.Rows.Count;i++) {
						Appointment appointment=new Appointment();
						appointment.PatNum=PIn.Long(tableAppt.Rows[i]["PatNum"].ToString());
						appointment.AptNum=PIn.Long(tableAppt.Rows[i]["AptNum"].ToString());
						appointment.InsPlan1=PIn.Long(tableAppt.Rows[i]["InsPlan1"].ToString());
						listAppointmentsInvalidInsPlan1.Add(appointment);
					}
					command="SELECT PatNum, AptNum, InsPlan2 FROM appointment "
						+"WHERE appointment.InsPlan2 != 0 ";
					if(isPatSpecific){
						command+="AND appointment.PatNum="+POut.Long(patNum)+" ";
					}
					command+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=appointment.InsPlan2)";
					DataTable tableAppt2=Db.GetTable(command);
					List<Appointment> listAppointmentsInvalidInsPlan2=new List<Appointment>();
					for(int i = 0;i<tableAppt2.Rows.Count;i++) {
						Appointment appointment=new Appointment();
						appointment.PatNum=PIn.Long(tableAppt2.Rows[i]["PatNum"].ToString());
						appointment.AptNum=PIn.Long(tableAppt2.Rows[i]["AptNum"].ToString());
						appointment.InsPlan2=PIn.Long(tableAppt2.Rows[i]["InsPlan2"].ToString());
						listAppointmentsInvalidInsPlan2.Add(appointment);
					}
					command="SELECT BenefitNum, PlanNum FROM benefit "
						+"WHERE PlanNum !=0 ";
					if(isPatSpecific) {
						if(listPlanNums.Count > 0) {
							command+="AND PlanNum IN ("+string.Join(",",listPlanNums)+") ";
						}
						else {
							command+="AND FALSE ";
						}
					}
					command+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=benefit.PlanNum)";
					DataTable tableBenefits=Db.GetTable(command);
					List<Benefit> listBenefitsInvalidPlanNum=new List<Benefit>();
					for(int i = 0;i<tableBenefits.Rows.Count;i++) {
						Benefit benefit=new Benefit();
						benefit.PlanNum=PIn.Long(tableBenefits.Rows[i]["PlanNum"].ToString());
						benefit.BenefitNum=PIn.Long(tableBenefits.Rows[i]["BenefitNum"].ToString());
						listBenefitsInvalidPlanNum.Add(benefit);
					}
					command="SELECT Subscriber, InsSubNum, PlanNum FROM inssub "
						+"WHERE "+(isPatSpecific ? "InsSubNum IN("+string.Join(",",listInsSubNums)+") AND " : "")
						+"NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=inssub.PlanNum)";
					DataTable tableInsSub=Db.GetTable(command);
					List<InsSub> listInsSub=new List<InsSub>();
					for(int i = 0;i<tableInsSub.Rows.Count;i++) {
						InsSub insSub=new InsSub();
						insSub.Subscriber=PIn.Long(tableInsSub.Rows[i]["Subscriber"].ToString());
						insSub.InsSubNum=PIn.Long(tableInsSub.Rows[i]["InsSubNum"].ToString());
						insSub.PlanNum=PIn.Long(tableInsSub.Rows[i]["PlanNum"].ToString());
						listInsSub.Add(insSub);
					}
					List<long> listPatNumsDistinct=listAppointmentsInvalidInsPlan1.Select(x => x.PatNum).Distinct()
						.Union(listAppointmentsInvalidInsPlan2.Select(x => x.PatNum).Distinct())
						.Union(listInsSub.Select(x => x.Subscriber).Distinct())
						.ToList();
					List<Patient> listPatientsLims=Patients.GetLimForPats(listPatNumsDistinct);
					numFound=listAppointmentsInvalidInsPlan1.Count+
						listAppointmentsInvalidInsPlan2.Count+
						listInsSub.Count+
						listBenefitsInvalidPlanNum.Count;
					if(numFound>0||verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid PlanNum references found : ")+numFound+"\r\n";
						log+=Lans.g("FormDatabaseMaintenance","Patients Affected : ")+listPatNumsDistinct.Count.ToString()
							+(listBenefitsInvalidPlanNum.Count>0 ? "+" : "")+"\r\n";
						for(int i=0;i<listPatientsLims.Count;i++){
							string lineItemDBM="   "+Lans.g("FormDatabaseMaintenance","Patient with invalid PlanNums")+":"+listPatientsLims[i].GetNameLF()+" (PatNum:"+listPatientsLims[i].PatNum+")"+"\r\n";
							//No additional translation needed. All "words" exactly match Schema column names.
							for(int j = 0;j<listAppointmentsInvalidInsPlan1.Count;j++) {
								if(listAppointmentsInvalidInsPlan1[j].PatNum!=listPatientsLims[i].PatNum) {
									continue;
								}
								lineItemDBM+="    AptNum:"+listAppointmentsInvalidInsPlan1[j].AptNum+" InsPlan1:"+listAppointmentsInvalidInsPlan1[j].InsPlan1+"\r\n";
							}
							for(int j = 0;j<listAppointmentsInvalidInsPlan2.Count;j++) {
								if(listAppointmentsInvalidInsPlan2[j].PatNum!=listPatientsLims[i].PatNum) {
									continue;
								}
								lineItemDBM+="    AptNum:"+listAppointmentsInvalidInsPlan2[j].AptNum+" InsPlan2:"+listAppointmentsInvalidInsPlan2[j].InsPlan2+"\r\n";
							}
							for(int j = 0;j<listInsSub.Count;j++) {
								if(listInsSub[j].Subscriber!=listPatientsLims[i].PatNum) {
									continue;
								}
								lineItemDBM+="    InsSubNum:"+listInsSub[j].InsSubNum+" PlanNum:"+listInsSub[j].PlanNum+"\r\n";
							}
							lineItemDBM+="\r\n";
							log+=lineItemDBM;
						}
						List<long> listPlanNumsDist=listBenefitsInvalidPlanNum.Select(x => x.PlanNum).Distinct().ToList();
						for(int i = 0;i<listPlanNumsDist.Count;i++) {
							List<Benefit> listBenefitsForPlan=listBenefitsInvalidPlanNum.FindAll(x => x.PlanNum==listPlanNumsDist[i]);
							string lineItemDBM="   "+Lans.g("FormDatabaseMaintenance","Invalid plan with attached benefits")+": PlanNum:"+listPlanNumsDist[i]+"\r\n";
							//No additional translation needed. All "words" exactly match Schema column names.
							lineItemDBM+="    BenefitNums:"+string.Join(", ",listBenefitsForPlan.Select(x => x.BenefitNum))+"\r\n\r\n";
							log+=lineItemDBM;
						}
					}
					#endregion BREAKDOWN
					break;
				case DbmMode.Fix:
					#region FIX
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					string where="";
					//One option will sometimes be to create a dummy plan to attach these things to, be we have not had to implement that yet.  
					//We need databases with actual problems to test these fixes against.
					//appointment.InsPlan1-----------------------------------------------------------------------------------------------
					where="WHERE InsPlan1 != 0 ";
					if(isPatSpecific){
						where+="AND appointment.PatNum="+POut.Long(patNum)+" ";
					}
					where+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=appointment.InsPlan1)";
					command="SELECT * FROM appointment "+where;
					List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
					command="UPDATE appointment SET InsPlan1=0 "+where;
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listAppointments.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listAppointments[i].AptNum,DbmLogFKeyType.Appointment,DbmLogActionType.Update,
						methodName,"Updated InsPlan1 from "+listAppointments[i].InsPlan1+" to 0 from InsPlanInvalidNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid appointment InsPlan1 values fixed: ")+numFixed+"\r\n";
					}
					//appointment.InsPlan2-----------------------------------------------------------------------------------------------
					where="WHERE InsPlan2 != 0 ";
					if(isPatSpecific){
						where+="AND appointment.PatNum="+POut.Long(patNum)+" ";
					}
					where+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=appointment.InsPlan2)";
					command="SELECT * FROM appointment "+where;
					listAppointments=Crud.AppointmentCrud.SelectMany(command);
					command="UPDATE appointment SET InsPlan2=0 "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listAppointments.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listAppointments[i].AptNum,DbmLogFKeyType.Appointment,DbmLogActionType.Update,
						methodName,"Updated InsPlan2 from "+listAppointments[i].InsPlan2+" to 0 from InsPlanInvalidNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid appointment InsPlan2 values fixed: ")+numFixed+"\r\n";
					}
					//benefit.PlanNum----------------------------------------------------------------------------------------------------
					where="WHERE PlanNum !=0 ";
					if(isPatSpecific) {
						if(listPlanNums.Count > 0) {
							where+="AND PlanNum IN ("+string.Join(",",listPlanNums)+") ";
						}
						else {
							where+="AND FALSE ";
						}
					}
					where+="AND NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=benefit.PlanNum)";
					command="SELECT * FROM benefit "+where;
					List<Benefit> listBenefits=Crud.BenefitCrud.SelectMany(command);
					command="DELETE FROM benefit "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listBenefits.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listBenefits[i].BenefitNum,DbmLogFKeyType.Benefit,
							DbmLogActionType.Delete,methodName,"Deleted benefit from InsPlanInvalidNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid benefit PlanNums fixed: ")+numFixed+"\r\n";
					}
					//inssub.PlanNum------------------------------------------------------------------------------------------------------
					numFixed=0;
					//1: PlanNum=0
					List<SecurityLog> listSecurityLogs= new List<SecurityLog>();
					List<InsSub> listInsSubs=new List<InsSub>();
					command="SELECT InsSubNum FROM inssub WHERE PlanNum=0 "+(isPatSpecific ? "AND InsSubNum IN("+string.Join(",",listInsSubNums)+")" : "");
					DataTable table=Db.GetTable(command);
					for(int i = 0;i<table.Rows.Count;i++) {
						long insSubNum=PIn.Long(table.Rows[i]["InsSubNum"].ToString());
						command="SELECT COUNT(*) FROM claim WHERE InsSubNum="+POut.Long(insSubNum);
						int countUsed=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM claimproc WHERE InsSubNum="+POut.Long(insSubNum)+" "
							+"AND (ClaimNum<>0 OR (Status<>6 AND Status<>3))";//attached to a claim or (not an estimate or adjustment)
						countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM etrans WHERE InsSubNum="+POut.Long(insSubNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						//command="SELECT COUNT(*) FROM patplan WHERE InsSubNum="+POut.Long(insSubNum);
						//countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM payplan WHERE InsSubNum="+POut.Long(insSubNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						if(countUsed==0) {
							where="WHERE InsSubNum="+POut.Long(insSubNum)+" AND ClaimNum=0 AND (Status=6 OR Status=3)";
							command="SELECT * FROM claimproc "+where;
							List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
							command="DELETE FROM claimproc "+where;//ok to delete because no claim and just an estimate or adjustment
							Db.NonQ(command);
							for(int j=0;j<listClaimProcs.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[j].ClaimProcNum,DbmLogFKeyType.ClaimProc,
								DbmLogActionType.Delete,methodName,"Deleted claimproc from InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLog);
							}
							List<EnumPermType> listEnumPermTypes=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeof(InsSub)));
							listSecurityLogs=SecurityLogs.GetFromFKeysAndType(new List<long> { insSubNum },listEnumPermTypes);
							InsSubs.ClearFkey(insSubNum);
							for(int j=0;j<listSecurityLogs.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSecurityLogs[j].SecurityLogNum,DbmLogFKeyType.Securitylog,
									DbmLogActionType.Update,methodName,"Updated securitylog by setting FKey to 0 from InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLog);
							}
							command="SELECT * FROM inssub WHERE InsSubNum="+POut.Long(insSubNum);
							listInsSubs=Crud.InsSubCrud.SelectMany(command);
							command="DELETE FROM inssub WHERE InsSubNum="+POut.Long(insSubNum);
							Db.NonQ(command);
							for(int j=0;j<listInsSubs.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listInsSubs[j].InsSubNum,DbmLogFKeyType.InsSub,
								DbmLogActionType.Delete,methodName,"Deleted inssub from InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLog);
							}
							command="SELECT * FROM patplan WHERE InsSubNum="+POut.Long(insSubNum);
							List<PatPlan> listPatPlans=Crud.PatPlanCrud.SelectMany(command);
							command="DELETE FROM patplan WHERE InsSubNum="+POut.Long(insSubNum);//It's very safe to "drop coverage" for a patient.
							Db.NonQ(command);
							for(int j=0;j<listPatPlans.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatPlans[j].PatPlanNum,DbmLogFKeyType.PatPlan,
								DbmLogActionType.Delete,methodName,"Deleted patplan from InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLog);
							}
							numFixed++;
							continue;
						}
					}
					//2: PlanNum invalid
					command="SELECT InsSubNum,PlanNum FROM inssub "
						+"WHERE ";
					if(isPatSpecific){
						command+="InsSubNum IN("+string.Join(",",listInsSubNums)+") AND ";
					}
					command+="NOT EXISTS(SELECT * FROM insplan WHERE insplan.PlanNum=inssub.PlanNum)";
					table=Db.GetTable(command);
					for(int i = 0;i<table.Rows.Count;i++) {
						long planNum=PIn.Long(table.Rows[i]["PlanNum"].ToString());
						long insSubNum=PIn.Long(table.Rows[i]["InsSubNum"].ToString());
						command="SELECT COUNT(*) FROM claim WHERE InsSubNum="+POut.Long(insSubNum);
						int countUsed=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM claimproc WHERE InsSubNum="+POut.Long(insSubNum)+" AND Status NOT IN (6,3)";//Estimate,Adjustment
						countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM etrans WHERE InsSubNum="+POut.Long(insSubNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM patplan WHERE InsSubNum="+POut.Long(insSubNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM payplan WHERE InsSubNum="+POut.Long(insSubNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						//planNum
						command="SELECT COUNT(*) FROM benefit WHERE PlanNum="+POut.Long(planNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM claim WHERE PlanNum="+POut.Long(planNum);
						countUsed+=PIn.Int(Db.GetCount(command));
						command="SELECT COUNT(*) FROM claimproc WHERE PlanNum="+POut.Long(planNum)+" AND Status NOT IN (6,3)";//Estimate,Adjustment
						countUsed+=PIn.Int(Db.GetCount(command));
						if(countUsed==0) {//There are no other pointers to this invalid plannum or this inssub, delete this inssub
							List<EnumPermType> listEnumPermTypes=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeof(InsSub)));
							listSecurityLogs=SecurityLogs.GetFromFKeysAndType(new List<long> { insSubNum },listEnumPermTypes);
							InsSubs.ClearFkey(insSubNum);
							for(int j=0;j<listSecurityLogs.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSecurityLogs[j].SecurityLogNum,DbmLogFKeyType.Securitylog,
									DbmLogActionType.Update,methodName,"Updated securitylog by setting FKey to 0 from InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLog);
							}
							command="SELECT * FROM inssub WHERE InsSubNum="+POut.Long(insSubNum);
							listInsSubs=Crud.InsSubCrud.SelectMany(command);
							command="DELETE FROM inssub WHERE InsSubNum="+POut.Long(insSubNum);
							Db.NonQ(command);
							command="DELETE FROM claimproc WHERE PlanNum="+POut.Long(planNum)+" AND Status IN (6,3)";//Estimate,Adjustment
							Db.NonQ(command);
							command="DELETE FROM claimproc WHERE InsSubNum="+POut.Long(insSubNum)+" AND Status IN (6,3)";//Estimate,Adjustment
							Db.NonQ(command);
							for(int j=0;j<listInsSubs.Count;j++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listInsSubs[j].InsSubNum,DbmLogFKeyType.InsSub,
									DbmLogActionType.Delete,methodName,"Deleted inssub from InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLog);
							}
							numFixed++;
							continue;
						}
						else {//There are objects referencing this inssub or this insplan.  Insert a dummy plan linked to a dummy carrier with CarrierName=Unknown
							InsPlan insPlan=new InsPlan();
							insPlan.IsHidden=true;
							insPlan.CarrierNum=Carriers.GetByNameAndPhone("UNKNOWN CARRIER","",true).CarrierNum;
							//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
							insPlan.SecUserNumEntry=Security.CurUser.UserNum;
							long insPlanNum=InsPlans.Insert(insPlan);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,insPlanNum,DbmLogFKeyType.InsPlan,DbmLogActionType.Insert,
								methodName,"Inserted a new insplan from InsPlanInvalidNum");
							listDbmLogs.Add(dbmLog);
							command="SELECT * FROM inssub WHERE InsSubNum="+POut.Long(insSubNum);
							listInsSubs=Crud.InsSubCrud.SelectMany(command);
							command="UPDATE inssub SET PlanNum="+POut.Long(insPlanNum)+" WHERE InsSubNum="+POut.Long(insSubNum);
							Db.NonQ(command);
							for(int j=0;j<listInsSubs.Count;j++){
								DbmLog dbmLogInsSub=new DbmLog(Security.CurUser.UserNum,listInsSubs[j].InsSubNum,DbmLogFKeyType.InsSub,DbmLogActionType.Update,
									methodName,"Updated PlanNum from "+listInsSubs[j].PlanNum+" to "+insPlanNum+" InsPlanInvalidNum.");
								listDbmLogs.Add(dbmLogInsSub);
							}
							numFixed++;
							continue;
						}
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Invalid inssub PlanNums fixed: ")+numFixed+"\r\n";
					}
					#endregion FIX
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string InsPlanNoClaimForm(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM insplan WHERE ClaimFormNum=0";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Insplan claimforms missing: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT * FROM insplan WHERE ClaimFormNum=0";
					List<InsPlan> listInsPlans=Crud.InsPlanCrud.SelectMany(command);
					command="UPDATE insplan SET ClaimFormNum="+POut.Long(PrefC.GetLong(PrefName.DefaultClaimForm))
						+" WHERE ClaimFormNum=0";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listInsPlans.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listInsPlans[i].PlanNum,DbmLogFKeyType.InsPlan,DbmLogActionType.Update,
							methodName,"Updated ClaimFormNum from 0 to "+PrefC.GetLong(PrefName.DefaultClaimForm));
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Insplan claimforms set if missing: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}
		
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string InsSubInvalidSubscriber(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT inssub.Subscriber
				FROM inssub 
				INNER JOIN patient ON patient.PatNum=inssub.Subscriber
				WHERE patient.PatStatus="+POut.Int((int)PatientStatus.Deleted)+@"
				AND inssub.Subscriber != 0 
				GROUP BY inssub.Subscriber";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=table.Rows.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","InsSub subscribers that are deleted:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i = 0;i<table.Rows.Count;i++) {
						//Change the patient to Archived.
						Patient patient=Patients.GetPat(PIn.Long(table.Rows[i]["Subscriber"].ToString()));
						Patient patientOld=patient.Copy();
						patient.PatStatus=PatientStatus.Archived;
						Patients.Update(patient,patientOld);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patient.PatNum,DbmLogFKeyType.Patient,DbmLogActionType.Update,
							methodName,"Updated PatStatus from "+patientOld.PatStatus+" to "+PatientStatus.Archived+".");
						listDbmLogs.Add(dbmLog);
						SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patient.PatNum,
							"Patient status changed from 'Deleted' to 'Archived' from DBM fix for InsSubInvalidSubscriber.",LogSources.DBM);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","InsSub subscribers status changed:")+" "+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		///<summary>Checks for situations where there are valid InsSubNums, but mismatched PlanNums.</summary>
		[DbmMethodAttr(HasBreakDown=true,HasPatNum=true,IsReplicationUnsafe=true)]
		public static string InsSubNumMismatchPlanNum(bool verbose,DbmMode dbmMode,long patNumSpecific=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNumSpecific);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			//Not going to validate the following tables because they do not have an InsSubNum column: appointmentx2, benefit.
			//This DBM assumes that the inssub table is correct because that's what we're comparing against.
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					#region CHECK
					int numFound=0;
					bool hasBreakDown=false;
					//Can't do the following because no inssubnum: appointmentx2, benefit.
					//Can't do inssub because that's what we're comparing against.  That's the one that's assumed to be correct.
					//claim.PlanNum -----------------------------------------------------------------------------------------------------
					command="SELECT COUNT(*) FROM claim "
						+"WHERE PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum) "
						+PatientAndClauseHelper(patNumSpecific,"claim");
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched claim InsSubNum/PlanNum values")+": "+numFound+"\r\n";
						if(numFound>0) {
							hasBreakDown=true;
						}
					}
					//claim.PlanNum2---------------------------------------------------------------------------------------------------
					command="SELECT COUNT(*) FROM claim "
						+"WHERE PlanNum2 != 0 "
						+"AND PlanNum2 NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum2)"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched claim InsSubNum2/PlanNum2 values")+": "+numFound+"\r\n";
						if(numFound>0) {
							hasBreakDown=true;
						}
					}
					//claimproc---------------------------------------------------------------------------------------------------
					command="SELECT COUNT(*) FROM claimproc "
						+"WHERE PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claimproc.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"claimproc");
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched claimproc InsSubNum/PlanNum values")+": "+numFound+"\r\n";
						if(numFound>0) {
							hasBreakDown=true;
						}
					}
					//etrans---------------------------------------------------------------------------------------------------
					command="SELECT COUNT(*) FROM etrans "
						+"WHERE PlanNum!=0 AND InsSubNum!=0 AND PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=etrans.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"etrans");
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched etrans InsSubNum/PlanNum values")+": "+numFound+"\r\n";
						if(numFound>0) {
							hasBreakDown=true;
						}
					}
					//payplan---------------------------------------------------------------------------------------------------
					command="SELECT COUNT(*) FROM payplan "
						+"WHERE EXISTS (SELECT PlanNum FROM inssub WHERE inssub.InsSubNum=payplan.InsSubNum AND inssub.PlanNum!=payplan.PlanNum)"
						+PatientAndClauseHelper(patNumSpecific,"payplan");
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched payplan InsSubNum/PlanNum values")+": "+numFound+"\r\n";
						if(numFound>0) {
							hasBreakDown=true;
						}
					}
					if(hasBreakDown) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Run Fix or double click to see a break down.");
					}
					break;
				#endregion CHECK
				case DbmMode.Breakdown:
					#region BREAKDOWN
					//In this BREAKDOWN, when user double clicks on this DBM query and show what needs to be fixed/will attempted to be fixed when running Fix.
					//claim.PlanNum -----------------------------------------------------------------------------------------------------
					command="SELECT claim.PatNum,claim.PlanNum,claim.ClaimNum,claim.DateService FROM claim "
						+"WHERE PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum) "
						+PatientAndClauseHelper(patNumSpecific,"claim");
					DataTable tableClaims=Db.GetTable(command);
					List<Claim> listClaimsBad1=new List<Claim>();
					for(int i = 0;i<tableClaims.Rows.Count;i++) {
						Claim claim=new Claim();
						claim.PatNum=PIn.Long(tableClaims.Rows[i]["PatNum"].ToString());
						claim.PlanNum=PIn.Long(tableClaims.Rows[i]["PlanNum"].ToString());
						claim.ClaimNum=PIn.Long(tableClaims.Rows[i]["ClaimNum"].ToString());
						claim.DateService=PIn.DateT(tableClaims.Rows[i]["DateService"].ToString());
						listClaimsBad1.Add(claim);
					}
					//claim.PlanNum2---------------------------------------------------------------------------------------------------
					command="SELECT claim.PatNum,claim.PlanNum2,claim.ClaimNum,claim.DateService FROM claim "
						+"WHERE PlanNum2 != 0 "
						+"AND PlanNum2 NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum2)"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					DataTable tableClaims2=Db.GetTable(command);
					List<Claim> listClaimsBad2=new List<Claim>();
					for(int i = 0;i<tableClaims2.Rows.Count;i++) {
						Claim claim=new Claim();
						claim.PatNum=PIn.Long(tableClaims2.Rows[i]["PatNum"].ToString());
						claim.PlanNum2=PIn.Long(tableClaims2.Rows[i]["PlanNum2"].ToString());
						claim.ClaimNum=PIn.Long(tableClaims2.Rows[i]["ClaimNum"].ToString());
						claim.DateService=PIn.DateT(tableClaims2.Rows[i]["DateService"].ToString());
						listClaimsBad2.Add(claim);
					}
					//claimproc---------------------------------------------------------------------------------------------------
					command="SELECT claimproc.PatNum,claimproc.ClaimProcNum,claimproc.InsSubNum,claimproc.ProcNum,claimproc.ClaimNum FROM claimproc "
						+"WHERE PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claimproc.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"claimproc");
					DataTable tableClaimProcs=Db.GetTable(command);
					List<ClaimProc> listClaimProcsBad=new List<ClaimProc>();
					for(int i = 0;i<tableClaimProcs.Rows.Count;i++) {
						ClaimProc claimProc=new ClaimProc();
						claimProc.PatNum=PIn.Long(tableClaimProcs.Rows[i]["PatNum"].ToString());
						claimProc.ClaimProcNum=PIn.Long(tableClaimProcs.Rows[i]["ClaimProcNum"].ToString());
						claimProc.InsSubNum=PIn.Long(tableClaimProcs.Rows[i]["InsSubNum"].ToString());
						claimProc.ProcNum=PIn.Long(tableClaimProcs.Rows[i]["ProcNum"].ToString());
						claimProc.ClaimNum=PIn.Long(tableClaimProcs.Rows[i]["ClaimNum"].ToString());
						listClaimProcsBad.Add(claimProc);
					}
					//etrans---------------------------------------------------------------------------------------------------
					command="SELECT etrans.PatNum,etrans.PlanNum,etrans.EtransNum,etrans.DateTimeTrans FROM etrans "
						+"WHERE PlanNum!=0 AND InsSubNum!=0 AND PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=etrans.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"etrans");
					DataTable tableEtrans=Db.GetTable(command);
					List<Etrans> listEtransesBad=new List<Etrans>();
					for(int i = 0;i<tableEtrans.Rows.Count;i++) {
						Etrans etrans=new Etrans();
						etrans.PatNum=PIn.Long(tableEtrans.Rows[i]["PatNum"].ToString());
						etrans.PlanNum=PIn.Long(tableEtrans.Rows[i]["PlanNum"].ToString());
						etrans.EtransNum=PIn.Long(tableEtrans.Rows[i]["EtransNum"].ToString());
						etrans.DateTimeTrans=PIn.DateT(tableEtrans.Rows[i]["DateTimeTrans"].ToString());
						listEtransesBad.Add(etrans);
					}
					//payplan---------------------------------------------------------------------------------------------------
					command="SELECT payplan.PatNum,payplan.PlanNum,payplan.PayPlanNum FROM payplan "
						+"WHERE EXISTS (SELECT PlanNum FROM inssub WHERE inssub.InsSubNum=payplan.InsSubNum AND inssub.PlanNum!=payplan.PlanNum)"
						+PatientAndClauseHelper(patNumSpecific,"payplan");
					DataTable tablePayPlans=Db.GetTable(command);
					List<PayPlan> listPayPlansBad=new List<PayPlan>();
					for(int i = 0;i<tablePayPlans.Rows.Count;i++) {
						PayPlan payPlan=new PayPlan();
						payPlan.PatNum=PIn.Long(tablePayPlans.Rows[i]["PatNum"].ToString());
						payPlan.PlanNum=PIn.Long(tablePayPlans.Rows[i]["PlanNum"].ToString());
						payPlan.PayPlanNum=PIn.Long(tablePayPlans.Rows[i]["PayPlanNum"].ToString());
						listPayPlansBad.Add(payPlan);
					}
					List<long> listPatNumsDistinct=listClaimsBad1.Select(x => x.PatNum).Distinct()
						.Union(listClaimsBad2.Select(x => x.PatNum).Distinct())
						.Union(listClaimProcsBad.Select(x => x.PatNum).Distinct())
						.Union(listEtransesBad.Select(x => x.PatNum).Distinct())
						.Union(listPayPlansBad.Select(x => x.PatNum).Distinct())
						.ToList();
					List<Patient> listPatientsLim=Patients.GetLimForPats(listPatNumsDistinct);
					numFound=listClaimsBad1.Count+
						listClaimsBad2.Count+
						listClaimProcsBad.Count+
						listEtransesBad.Count+
						listPayPlansBad.Count;
					if(numFound>0||verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched InsSubNum/PlanNum values")+": "+numFound+"\r\n";
						log+=Lans.g("FormDatabaseMaintenance","Patients affected")+": "+listPatNumsDistinct.Count+"\r\n";
						for(int i=0;i<listPatientsLim.Count;i++){
							string lineItemDBM="   "+Lans.g("FormDatabaseMaintenance","Patient with associated invalid PlanNums")+":"+listPatientsLim[i].GetNameLF()+" (PatNum:"+listPatientsLim[i].PatNum+")"+"\r\n";
							for(int j=0;j<listClaimsBad1.Count;j++) {
								if(listClaimsBad1[j].PatNum!=listPatientsLim[i].PatNum) {
									continue;
								}
								lineItemDBM+="    ClaimNum:"+listClaimsBad1[j].ClaimNum+" PlanNum:"+listClaimsBad1[j].PlanNum+" DateService:"
									+listClaimsBad1[j].DateService.ToShortDateString()+"\r\n";
							}
							for(int j=0;j<listClaimsBad2.Count;j++) {
								if(listClaimsBad2[j].PatNum!=listPatientsLim[i].PatNum) {
									continue;
								}
								lineItemDBM+="    ClaimNum:"+listClaimsBad2[j].ClaimNum+" PlanNum2:"+listClaimsBad2[j].PlanNum2+" DateService:"
									+listClaimsBad2[j].DateService.ToShortDateString()+"\r\n";
							}
							for(int j=0;j<listClaimProcsBad.Count;j++) {
								if(listClaimProcsBad[j].PatNum!=listPatientsLim[i].PatNum) {
									continue;
								}
								lineItemDBM+="    ClaimProcNum:"+listClaimProcsBad[j].ClaimProcNum+" InsSubNum:"+listClaimProcsBad[j].InsSubNum+" ClaimNum:"
									+listClaimProcsBad[j].ClaimNum+" ProcNum:"+listClaimProcsBad[j].ProcNum+"\r\n";
							}
							for(int j=0;j<listEtransesBad.Count;j++) {
								if(listEtransesBad[j].PatNum!=listPatientsLim[i].PatNum) {
									continue;
								}
								lineItemDBM+="    EtransNum:"+listEtransesBad[j].EtransNum+" PlanNum:"+listEtransesBad[j].PlanNum+" DateTimeTrans:"
									+listEtransesBad[j].DateTimeTrans.ToShortDateString()+"\r\n";
							}
							for(int j=0;j<listPayPlansBad.Count;j++) {
								if(listPayPlansBad[j].PatNum!=listPatientsLim[i].PatNum) {
									continue;
								}
								lineItemDBM+="    PayPlanNum:"+listPayPlansBad[j].PayPlanNum+" PlanNum:"+listPayPlansBad[j].PlanNum+"\r\n";
							}
							lineItemDBM+="\r\n";
							log+=lineItemDBM;
						}
					}
					break;
				#endregion BREAKDOWN
				case DbmMode.Fix:
					#region FIX
					int numFixed=0;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					string where="";
					#region Claim PlanNum
					#region claim.PlanNum (1/4) Mismatch
					where="WHERE PlanNum != (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					command="SELECT * FROM claim "+where;
					List<Claim> listClaims=Crud.ClaimCrud.SelectMany(command);
					command="UPDATE claim SET PlanNum=(SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum) "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaims.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaims[i].ClaimNum,DbmLogFKeyType.Claim,DbmLogActionType.Update,
							methodName,"Updated PlanNum from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched claim InsSubNum/PlanNum fixed")+": "+numFixed.ToString()+"\r\n";
					}
					#endregion
					numFixed=0;
					#region claim.PlanNum (2/4) PlanNum zero, invalid InsSubNum
					//Will leave orphaned claimprocs. No finanicals to check.
					command="SELECT claim.ClaimNum FROM claim WHERE PlanNum=0 AND ClaimStatus IN ('PreAuth','W','U','H','I') "
						+"AND NOT EXISTS(SELECT * FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					DataTable tableClaimNums=Db.GetTable(command);
					List<long> listClaimNums=new List<long>();
					for(int i = 0;i<tableClaimNums.Rows.Count;i++) {
						listClaimNums.Add(PIn.Long(tableClaimNums.Rows[i]["ClaimNum"].ToString()));
					}
					if(listClaimNums.Count>0) {
						List<EnumPermType> listEnumPermTypes=GroupPermissions.GetPermsFromCrudAuditPerm(CrudTableAttribute.GetCrudAuditPermForClass(typeof(Claim)));
						List<SecurityLog> listSecurityLogs=SecurityLogs.GetFromFKeysAndType(listClaimNums,listEnumPermTypes);
						Claims.ClearFkey(listClaimNums);//Zero securitylog FKey column for rows to be deleted.
						for(int i=0;i<listSecurityLogs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSecurityLogs[i].SecurityLogNum,DbmLogFKeyType.Securitylog,
							DbmLogActionType.Update,methodName,"Set FKey to 0 from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					}
					where="WHERE PlanNum=0 AND ClaimStatus IN('PreAuth','W','U','H','I') AND NOT EXISTS(SELECT * FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					command="SELECT ClaimNum FROM claim "+where;
					listClaimNums=Db.GetListLong(command);
					command="DELETE FROM claim "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimNums[i],DbmLogFKeyType.Claim,DbmLogActionType.Delete,
							methodName,"Claim deleted with invalid InsSubNum and PlanNum=0 .");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims deleted with invalid InsSubNum and PlanNum=0")+": "+numFixed.ToString()+"\r\n";
					}
					#endregion
					numFixed=0;
					#region claim.PlanNum (3/4) PlanNum invalid, and claim.InsSubNum invalid
					command="SELECT claim.PatNum,claim.PlanNum,claim.InsSubNum FROM claim "
						+"WHERE PlanNum NOT IN (SELECT insplan.PlanNum FROM insplan) "
						+"AND InsSubNum NOT IN (SELECT inssub.InsSubNum FROM inssub) "
						+PatientAndClauseHelper(patNumSpecific,"claim");
					DataTable table=Db.GetTable(command);
					if(table.Rows.Count>0) {
						log+=Lans.g("FormDatabaseMaintenance","List of patients who will need insurance information reentered:")+"\r\n";
					}
					for(int i = 0;i<table.Rows.Count;i++) {//Create simple InsPlans and InsSubs for each claim to replace the missing ones.
						//make sure a plan does not exist from a previous insert in this loop
						command="SELECT COUNT(*) FROM insplan WHERE PlanNum=" + table.Rows[i]["PlanNum"].ToString();
						if(Db.GetCount(command)=="0") {
							InsPlan insPlan=new InsPlan();
							insPlan.PlanNum=PIn.Long(table.Rows[i]["PlanNum"].ToString());//reuse the existing FK
							insPlan.IsHidden=true;
							insPlan.CarrierNum=Carriers.GetByNameAndPhone("UNKNOWN CARRIER","",true).CarrierNum;
							//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
							insPlan.SecUserNumEntry=Security.CurUser.UserNum;
							InsPlans.Insert(insPlan,true);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,insPlan.PlanNum,DbmLogFKeyType.InsPlan,DbmLogActionType.Insert,
								methodName,"Inserted new insplan from InsSubNumMismatchPlanNum.");
							listDbmLogs.Add(dbmLog);
						}
						long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
						//make sure an inssub does not exist from a previous insert in this loop
						command="SELECT COUNT(*) FROM inssub WHERE InsSubNum=" + table.Rows[i]["InsSubNum"].ToString();
						if(Db.GetCount(command)=="0") {
							InsSub insSub=new InsSub();
							insSub.InsSubNum=PIn.Long(table.Rows[i]["InsSubNum"].ToString());//reuse the existing FK
							insSub.PlanNum=PIn.Long(table.Rows[i]["PlanNum"].ToString());
							insSub.Subscriber=patNum;//if this sub was created on a previous loop, this may be some other patient.
							insSub.SubscriberID="unknown";
							//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
							insSub.SecUserNumEntry=Security.CurUser.UserNum;
							InsSubs.Insert(insSub,true);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,insSub.InsSubNum,DbmLogFKeyType.InsSub,DbmLogActionType.Insert,
								methodName,"Inserted new inssub from InsSubNumMismatchPlanNum.");
							listDbmLogs.Add(dbmLog);
						}
						Patient patient=Patients.GetLim(patNum);
						log+="PatNum: "+patient.PatNum+" - "+Patients.GetNameFL(patient.LName,patient.FName,patient.Preferred,patient.MiddleI)+"\r\n";
					}
					numFixed=table.Rows.Count;
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims with invalid PlanNums and invalid InsSubNums fixed: ")+numFixed.ToString()+"\r\n";
					}
					#endregion
					numFixed=0;
					#region claim.PlanNum (4/4) PlanNum valid, but claim.InsSubNum invalid
					command="SELECT PatNum,PlanNum,InsSubNum FROM claim "
						+"WHERE PlanNum IN (SELECT insplan.PlanNum FROM insplan) "
						+"AND InsSubNum NOT IN (SELECT inssub.InsSubNum FROM inssub) GROUP BY InsSubNum"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					table=Db.GetTable(command);
					//Create a dummy inssub and link it to the valid plan.
					for(int i = 0;i<table.Rows.Count;i++) {
						InsSub insSub=new InsSub();
						insSub.InsSubNum=PIn.Long(table.Rows[i]["InsSubNum"].ToString());
						insSub.PlanNum=PIn.Long(table.Rows[i]["PlanNum"].ToString());
						insSub.Subscriber=PIn.Long(table.Rows[i]["PatNum"].ToString());
						insSub.SubscriberID="unknown";
						//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
						insSub.SecUserNumEntry=Security.CurUser.UserNum;
						InsSubs.Insert(insSub,true);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,insSub.InsSubNum,DbmLogFKeyType.InsSub,DbmLogActionType.Insert,
							methodName,"Inserted new inssub from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					numFixed=table.Rows.Count;
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims with invalid InsSubNums and invalid PlanNums fixed: ")+numFixed.ToString()+"\r\n";
					}
					#endregion
					#endregion
					numFixed=0;
					#region Claim PlanNum2
					//claim.PlanNum2---------------------------------------------------------------------------------------------------
					where="WHERE PlanNum2 != 0 AND PlanNum2 NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum2)"
						+PatientAndClauseHelper(patNumSpecific,"claim");
					command="SELECT * FROM claim "+where;
					listClaims=Crud.ClaimCrud.SelectMany(command);
					command="UPDATE claim SET PlanNum2=(SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claim.InsSubNum2) "+where;
					//if InsSubNum2 was completely invalid, then PlanNum2 gets set to zero here.
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaims.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaims[i].ClaimNum,DbmLogFKeyType.Claim,
							DbmLogActionType.Update,methodName,"Updated PlanNum2 from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched claim InsSubNum2/PlanNum2 fixed: ")+numFixed.ToString()+"\r\n";
					}
					#endregion
					numFixed=0;
					#region ClaimProc
					//claimproc (1/2) If planNum is valid but InsSubNum does not exist, then add a dummy inssub----------------------------------------
					command="SELECT PatNum,PlanNum,InsSubNum FROM claimproc "
						+"WHERE PlanNum IN (SELECT insplan.PlanNum FROM insplan) "
						+PatientAndClauseHelper(patNumSpecific,"claimproc")
						+"AND InsSubNum NOT IN (SELECT inssub.InsSubNum FROM inssub) GROUP BY InsSubNum";
					table=Db.GetTable(command);
					//Create a dummy inssub and link it to the valid plan.
					for(int i = 0;i<table.Rows.Count;i++) {
						InsSub insSub=new InsSub();
						insSub.InsSubNum=PIn.Long(table.Rows[i]["InsSubNum"].ToString());
						insSub.PlanNum=PIn.Long(table.Rows[i]["PlanNum"].ToString());
						insSub.Subscriber=PIn.Long(table.Rows[i]["PatNum"].ToString());
						insSub.SubscriberID="unknown";
						//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
						insSub.SecUserNumEntry=Security.CurUser.UserNum;
						InsSubs.Insert(insSub,true);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,insSub.InsSubNum,DbmLogFKeyType.InsSub,DbmLogActionType.Insert,
							methodName,"Inserted new inssub from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					numFixed=table.Rows.Count;
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claims with valid PlanNums and invalid InsSubNums fixed: ")+numFixed.ToString()+"\r\n";
					}
					numFixed=0;
					//claimproc (2/2) Mismatch, but InsSubNum is valid
					where="WHERE PlanNum != (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claimproc.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"claimproc");
					command="SELECT * FROM claimproc "+where;
					List<ClaimProc> listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
					command="UPDATE claimproc SET PlanNum=(SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=claimproc.InsSubNum) "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,
							DbmLogActionType.Update,methodName,"Updated PlanNum from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched claimproc InsSubNum/PlanNum fixed: ")+numFixed.ToString()+"\r\n";
					}
					numFixed=0;
					//claimproc.PlanNum zero, invalid InsSubNum--------------------------------------------------------------------------------
					where="WHERE PlanNum=0 AND NOT EXISTS(SELECT * FROM inssub WHERE inssub.InsSubNum=claimproc.InsSubNum)"
						+" AND InsPayAmt=0 AND WriteOff=0"//Make sure this deletion will not affect financials.
						+" AND Status IN (6,2)"//OK to delete because no claim and just an estimate (6) or preauth (2) claimproc
						+PatientAndClauseHelper(patNumSpecific,"claimproc");
					command="SELECT * FROM claimproc "+where;
					listClaimProcs=Crud.ClaimProcCrud.SelectMany(command);
					command="DELETE FROM claimproc "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listClaimProcs.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listClaimProcs[i].ClaimProcNum,DbmLogFKeyType.ClaimProc,
							DbmLogActionType.Delete,methodName,"Deleted claimproc from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Claimprocs deleted with invalid InsSubNum and PlanNum=0: ")+numFixed.ToString()+"\r\n";
					}
					#endregion
					numFixed=0;
					#region Etrans
					//etrans---------------------------------------------------------------------------------------------------
					where="WHERE PlanNum!=0 AND InsSubNum!=0 AND PlanNum NOT IN (SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=etrans.InsSubNum)"
						+PatientAndClauseHelper(patNumSpecific,"etrans");
					command="SELECT * FROM etrans "+where;
					List<Etrans> listEtranses=Crud.EtransCrud.SelectMany(command);
					command="UPDATE etrans SET PlanNum=(SELECT inssub.PlanNum FROM inssub WHERE inssub.InsSubNum=etrans.InsSubNum) "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listEtranses.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listEtranses[i].EtransNum,DbmLogFKeyType.Etrans,
							DbmLogActionType.Update,methodName,"Updated PlanNum from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched etrans InsSubNum/PlanNum fixed: ")+numFixed.ToString()+"\r\n";
					}
					#endregion
					numFixed=0;
					#region PayPlan
					//payplan--------------------------------------------------------------------------------------------------
					where="WHERE EXISTS (SELECT PlanNum FROM inssub WHERE inssub.InsSubNum=payplan.InsSubNum AND inssub.PlanNum!=payplan.PlanNum)"
						+PatientAndClauseHelper(patNumSpecific,"payplan");
					command="SELECT * FROM payplan "+where;
					List<PayPlan> listPayPlans=Crud.PayPlanCrud.SelectMany(command);
					command="UPDATE payplan SET PlanNum=(SELECT PlanNum FROM inssub WHERE inssub.InsSubNum=payplan.InsSubNum) "+where;
					numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPayPlans.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPayPlans[i].PayPlanNum,DbmLogFKeyType.PayPlan,
							DbmLogActionType.Update,methodName,"Updated PlanNum from InsSubNumMismatchPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Mismatched payplan InsSubNum/PlanNum fixed: ")+numFixed.ToString()+"\r\n";
					}
					#endregion
					Crud.DbmLogCrud.InsertMany(listDbmLogs);
					#endregion FIX
					break;
			}
			return log;
		}

		#endregion InsPayPlan, InsPlan, InsSub----------------------------------------------------------------------------------------------------------
		#region JournalEntry, LabCase, Laboratory-------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string JournalEntryInvalidAccountNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT COUNT(*) FROM journalentry WHERE AccountNum NOT IN(SELECT AccountNum FROM account)";
			int numFound=PIn.Int(Db.GetCount(command));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Transactions found attached to an invalid account")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Transactions found attached to an invalid account")+": "+numFound+"\r\n";
					}
					if(numFound > 0) {
						//Check to see if there is already an active account called UNKNOWN.
						command="SELECT AccountNum FROM account WHERE Description='UNKNOWN' AND Inactive=0";
						long accountNum=PIn.Long(Db.GetScalar(command));
						if(accountNum==0) {
							//Create a new Account called UNKNOWN.
							Account account=new Account();
							account.Description="UNKNOWN";
							account.Inactive=false;//Just in case.
							account.AcctType=AccountType.Asset;//Default account type.  This DBM check was added to fix orphaned automatic payment journal entries, which should have been associated to an income account.
							accountNum=Accounts.Insert(account);
						}
						//Update the journalentry table.
						command="UPDATE journalentry SET AccountNum="+POut.Long(accountNum)+" WHERE AccountNum NOT IN(SELECT AccountNum FROM account)";
						Db.NonQ(command);
						log+=Lans.g("FormDatabaseMaintenance","   All invalid transactions have been attached to the account called UNKNOWN.")+"\r\n";
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string JournalEntryInvalidTransactionNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT COUNT(*) FROM journalentry WHERE TransactionNum NOT IN(SELECT TransactionNum FROM transaction)";
			int numFound=PIn.Int(Db.GetCount(command));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Transactions found with an invalid transaction number")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Transactions found with an invalid transaction number")+": "+numFound+"\r\n";
					}
					if(numFound > 0) {
						//Delete the journalentry row.
						command="DELETE FROM journalentry WHERE TransactionNum NOT IN(SELECT TransactionNum FROM transaction)";
						Db.NonQ(command);
						log+=Lans.g("FormDatabaseMaintenance","All invalid transactions have been deleted from transaction history.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string LabCaseWithInvalidLaboratory(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM labcase WHERE laboratoryNum NOT IN(SELECT laboratoryNum FROM laboratory)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Lab cases found with invalid laboratories")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT COUNT(*) FROM labcase WHERE laboratoryNum NOT IN(SELECT laboratoryNum FROM laboratory)";
					int numberFixed=PIn.Int(Db.GetCount(command));
					command="SELECT * FROM labcase WHERE laboratoryNum NOT IN(SELECT laboratoryNum FROM laboratory) GROUP BY LaboratoryNum";
					DataTable table=Db.GetTable(command);
					long labNum;
					for(int i = 0;i<table.Rows.Count;i++) {
						Laboratory laboratory=new Laboratory();
						laboratory.LaboratoryNum=PIn.Long(table.Rows[i]["LaboratoryNum"].ToString());
						laboratory.Description="Laboratory "+table.Rows[i]["LaboratoryNum"].ToString();
						//laboratoryNum is not allowed to be zero
						labNum=Crud.LaboratoryCrud.Insert(laboratory);
						command="UPDATE labcase SET LaboratoryNum="+POut.Long(labNum)+" WHERE LaboratoryNum="+table.Rows[i]["LaboratoryNum"].ToString();
						Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Lab cases fixed with invalid laboratories")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string LaboratoryWithInvalidSlip(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM laboratory WHERE Slip NOT IN(SELECT SheetDefNum FROM sheetdef) AND Slip != 0";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Laboratories found with invalid lab slips")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE laboratory SET Slip=0 WHERE Slip NOT IN(SELECT SheetDefNum FROM sheetdef) AND Slip != 0";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Laboratories fixed with invalid lab slips")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion JournalEntry, LabCase, Laboratory----------------------------------------------------------------------------------------------------
		#region MedicationPat, Medication, MessageButton------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string MedicationPatWithInvalidMedNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(DISTINCT MedicationNum) FROM medicationpat WHERE (medicationpat.MedicationNum<>0 AND NOT EXISTS(SELECT * FROM medication WHERE medication.MedicationNum=medicationpat.MedicationNum))";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Medications found where no definition exists for them: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT * FROM medicationpat WHERE (medicationpat.MedicationNum<>0 AND NOT EXISTS(SELECT * FROM medication WHERE medication.MedicationNum=medicationpat.MedicationNum)) GROUP BY MedicationNum";
					List<MedicationPat> listMedicationPats = MedicationPatCrud.SelectMany(command);
					List<Medication> listMedications = GetMedicationsFromMedPats(listMedicationPats);
					Medications.InsertMany(listMedications,true);
					if(listMedications.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Medications recreated because no definition existed for them: ")+listMedications.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		private static List<Medication> GetMedicationsFromMedPats(List<MedicationPat> listMedicationPats) {
			List<Medication> listMedications = new List<Medication>();
			string notes=Lans.g("FormDatabaseMaintenance","This medication was automatically created via Database Maintenance.");
			for(int i = 0;i < listMedicationPats.Count;i++) {
				Medication medication = new Medication();
				medication.MedicationNum=listMedicationPats[i].MedicationNum;
				medication.GenericNum=listMedicationPats[i].MedicationNum;
				medication.DateTStamp=DateTime.Now;
				medication.MedName=listMedicationPats[i].MedDescript;
				medication.RxCui=listMedicationPats[i].RxCui;
				medication.Notes=notes;
				listMedications.Add(medication);
			}
			return listMedications;
		}

		[DbmMethodAttr]
		public static string MedicationWithInvalidGenericNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM medication WHERE GenericNum NOT IN (SELECT MedicationNum FROM medication)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Medications with missing generic brand found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<Medication> listMedications;
					//Select into list because the following query is not valid in MySQL
					//UPDATE medication SET GenericNum=MedicationNum WHERE GenericNum NOT IN (SELECT MedicationNum FROM medication)
					command="SELECT * FROM medication WHERE GenericNum NOT IN (SELECT MedicationNum FROM medication)";
					listMedications=Crud.MedicationCrud.SelectMany(command);
					for(int i = 0;i<listMedications.Count;i++) {
						listMedications[i].GenericNum=listMedications[i].MedicationNum;
						Medications.Update(listMedications[i]);
					}
					if(listMedications.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Medications with missing generic brand fixed: ")+listMedications.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string MessageButtonDuplicateButtonIndex(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string queryStr="SELECT COUNT(*) NumFound,SigButDefNum,ButtonIndex,ComputerName FROM sigbutdef GROUP BY ComputerName,ButtonIndex HAVING COUNT(*) > 1";
			DataTable table=Db.GetTable(queryStr);
			int numFound=table.Select().Sum(x => PIn.Int(x["NumFound"].ToString())-1);//sum the duplicates; one in each group is valid.
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Messaging buttons found with invalid button orders")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					while(true){
						//Loop through the messaging buttons and increment the duplicate button index by the max plus one.
						for(int i = 0;i<table.Rows.Count;i++) {
							command="SELECT MAX(ButtonIndex) FROM sigbutdef WHERE ComputerName='"+table.Rows[i]["ComputerName"].ToString()+"'";
							int indexNew=PIn.Int(Db.GetScalar(command))+1;
							command="UPDATE sigbutdef SET ButtonIndex="+indexNew.ToString()+" WHERE SigButDefNum="+table.Rows[i]["SigButDefNum"].ToString();
							Db.NonQ(command);
						}
						//It's possible we need to loop through several more times depending on how many items shared the same button index. 
						table=Db.GetTable(queryStr);
						if(table.Rows.Count <= 0){
							break;
						}
					}
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Messaging buttons with invalid button orders fixed")+": "+numFound.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion MedicationPat, Medication, MessageButton---------------------------------------------------------------------------------------------
		#region Operatory, OrthoChart, PatField---------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string OperatoryInvalidReference(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Get distinct operatory nums that have been orphaned from appointment, scheduleop, and apptviewitems.  
			//We use a UNION instead of UNION ALL because we want MySQL or Oracle to group duplicate OpNums together.
			string command=@"SELECT appointment.Op AS OpNum FROM appointment WHERE appointment.Op!=0 AND NOT EXISTS(SELECT * FROM operatory WHERE operatory.OperatoryNum=appointment.Op)
									UNION 
									SELECT scheduleop.OperatoryNum AS OpNum FROM scheduleop WHERE scheduleop.OperatoryNum!=0 AND NOT EXISTS(SELECT * FROM operatory WHERE operatory.OperatoryNum=scheduleop.OperatoryNum) 
									UNION 
									SELECT apptviewitem.OpNum AS OpNum FROM apptviewitem WHERE apptviewitem.OpNum!=0 AND NOT EXISTS(SELECT * FROM operatory WHERE operatory.OperatoryNum=apptviewitem.OpNum)";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("OperatoryInvalidReference","Operatory references that are invalid")+": "+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					for(int i = 0;i<table.Rows.Count;i++) {
						long opNum=PIn.Long(table.Rows[i]["OpNum"].ToString());
						if(opNum!=0) {
							Operatory operatory=new Operatory();
							operatory.OperatoryNum=opNum;
							operatory.OpName="UNKNOWN-"+opNum;
							operatory.Abbrev="UNKN";
							Crud.OperatoryCrud.Insert(operatory,true);
						}
					}
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("OperatoryInvalidReference","Operatories created from an invalid operatory reference")+": "+table.Rows.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string OrthoChartFieldsWithoutValues(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string queryStr="SELECT COUNT(*) FROM orthochart WHERE FieldValue=''";
			int numFound=PIn.Int(Db.GetCount(queryStr));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ortho chart fields without values found")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					string command="DELETE FROM orthochart WHERE FieldValue=''";
					Db.NonQ(command);
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ortho chart fields without values fixed")+": "+numFound.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,IsReplicationUnsafe=true)]
		public static string PatFieldsDeleteDuplicates(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			//This code is only needed for older db's. New DB's created after 12.2.30 and 12.3.2 shouldn't need this.
			string command=@"DROP TABLE IF EXISTS tempduplicatepatfields";
			Db.NonQ(command);
			command="SELECT PatFieldNum,PatNum,FieldName " +
							"FROM patfield " +
							"GROUP BY PatNum,FieldName " +
							"HAVING COUNT(*)>1";
			DataTable tableDupPatfields=Db.GetTable(command);
			List<PatField> listPatFieldsLim=new List<PatField>();
			for(int i=0;i<tableDupPatfields.Rows.Count;i++){
				PatField patField=new PatField();
				patField.PatFieldNum=PIn.Long(tableDupPatfields.Rows[i]["PatFieldNum"].ToString());
				patField.PatNum=PIn.Long(tableDupPatfields.Rows[i]["PatNum"].ToString());
				patField.FieldName=PIn.String(tableDupPatfields.Rows[i]["FieldName"].ToString());
				listPatFieldsLim.Add(patField);
			}
			DataTable tablePatientsWithDups=new DataTable();
			if(listPatFieldsLim.Count>0) {
				command="SELECT PatNum,LName,FName "+
								"FROM patient WHERE PatNum IN ("+string.Join(",",listPatFieldsLim.Select(x=>POut.Long(x.PatNum)))+") "+
								"GROUP BY PatNum";
				tablePatientsWithDups=Db.GetTable(command);
			}
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(tablePatientsWithDups.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with duplicate field entries found:")+" "+tablePatientsWithDups.Rows.Count+".\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(tablePatientsWithDups.Rows.Count>0 || verbose) {
						StringBuilder stringBuilder=new StringBuilder();
						stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","The following patients had corrupt Patient Fields.  "
							+"Please verify the Patient Fields of these patients:"));
						for(int i=0;i<tablePatientsWithDups.Rows.Count;i++){
							stringBuilder.AppendLine("#"+tablePatientsWithDups.Rows[i]["PatNum"].ToString()+" "+tablePatientsWithDups.Rows[i]["LName"]+
								", "+tablePatientsWithDups.Rows[i]["FName"]);
						}
						stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Patients with duplicate field entries found:")+" "+tablePatientsWithDups.Rows.Count);
						log+=stringBuilder.ToString();
					}
					break;
				case DbmMode.Fix:
					if(tablePatientsWithDups.Rows.Count>0) {
						for(int i=0;i<listPatFieldsLim.Count;i++) {
							command="DELETE FROM patfield WHERE PatNum="+POut.Long(listPatFieldsLim[i].PatNum)+" " +
							"AND FieldName='"+POut.String(listPatFieldsLim[i].FieldName)+"' " +
							"AND PatFieldNum!="+POut.Long(listPatFieldsLim[i].PatFieldNum);
							Db.NonQ(command);
						}
					}
					if(tablePatientsWithDups.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with duplicate field entries removed:")+" "+tablePatientsWithDups.Rows.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion Operatory, OrthoChart, PatField------------------------------------------------------------------------------------------------------
		#region Patient---------------------------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatientBadGuarantor(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=$"SELECT p.PatNum,p.Guarantor FROM patient p LEFT JOIN patient p2 ON p.Guarantor=p2.PatNum WHERE (p2.PatNum IS NULL) OR (p.Guarantor != p.PatNum AND p.patstatus={POut.Enum(PatientStatus.Deleted)})";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with invalid Guarantors found: ")+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i=0;i<table.Rows.Count;i++) {
						long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
						Patient patientOld=Patients.GetPat(patNum);
						Patient patient=patientOld.Copy();
						patient.Guarantor=patient.PatNum;
						Patients.Update(patient,patientOld);
						long guarantor=PIn.Long(table.Rows[i]["Guarantor"].ToString());
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patNum,DbmLogFKeyType.Patient,DbmLogActionType.Update,methodName,
							"Updated Guarantor from "+guarantor+" to "+patNum+" from PatientBadGuarantor.");
						listDbmLogs.Add(dbmLog);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with invalid Guarantors fixed: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatientBadGuarantorWithAnotherGuarantor(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT p.PatNum,p2.Guarantor FROM patient p LEFT JOIN patient p2 ON p.Guarantor=p2.PatNum WHERE p2.PatNum!=p2.Guarantor";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with a guarantor who has another guarantor found: ")+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i=0;i<table.Rows.Count;i++) {
						long patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
						long guarantor=PIn.Long(table.Rows[i]["Guarantor"].ToString());
						Patient patientOld=Patients.GetPat(patNum);
						Patient patient=patientOld.Copy();
						patient.Guarantor=guarantor;
						Patients.Update(patient,patientOld);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patNum,DbmLogFKeyType.Patient,DbmLogActionType.Update,methodName,
							"Updated Guarantor to "+guarantor+" from PatientBadGuarantorWithAnotherGuarantor.");
						listDbmLogs.Add(dbmLog);
					}
					int numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with a guarantor who has another guarantor fixed: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatientDeletedWithClinicNumSet(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM patient WHERE ClinicNum!=0 AND PatStatus="+POut.Int((int)PatientStatus.Deleted);
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deleted patients with a clinic still set: ")+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					string where="WHERE ClinicNum!=0 AND PatStatus="+POut.Int((int)PatientStatus.Deleted);
					command="SELECT * FROM patient "+where;
					List<Patient> listPatients=Crud.PatientCrud.SelectMany(command);
					command="UPDATE patient SET ClinicNum=0 "+where;
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPatients.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatients[i].PatNum,DbmLogFKeyType.Patient,DbmLogActionType.Update,
							methodName,"Updated ClinicNum from "+listPatients[i].ClinicNum+" to 0.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deleted patients with clinics cleared: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatientInvalidGradeLevel(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM patient WHERE GradeLevel < 0";//Any negative number is considered invalid.
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with invalid GradeLevel set")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT * FROM patient WHERE GradeLevel < 0";
					List<Patient> listPatients=Crud.PatientCrud.SelectMany(command);
					//Set all invalid Grade Levels to Unknown.
					command="UPDATE patient SET GradeLevel="+POut.Int((int)PatientGrade.Unknown)+" WHERE GradeLevel < 0";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPatients.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatients[i].PatNum,DbmLogFKeyType.Patient,DbmLogActionType.Update,
							methodName,"Updated GradeLevel from "+listPatients[i].GradeLevel+" to "+POut.Int((int)PatientGrade.Unknown)+".");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with invalid GradeLevel fixed")+": "+numFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		///<summary>Finds Patients in a SuperFamily where the SuperHead is no longer in the SuperFamily.  This occurs when the SuperHead is moved out of 
		///the SuperFamily but the remaining SuperFamily members do not have their SuperFamily field updated.  Since we cannot reliably choose a member 
		///of the remaining SuperFamily as the new SuperHead, we use the new Guarantor of the previous SuperHead as the new SuperHead, or in the event 
		///the old SuperHead has been moved to a new SuperFamily we use the SuperHead of that SuperFamily, effectively merging the SuperFamily into this 
		///new Family/SuperFamily where the previous SuperHead now resides.</summary>
		[DbmMethodAttr(IsOneOff=true, IsReplicationUnsafe=true)]
		public static string PatientInvalidSuperFamilyHead(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT p1.PatNum, p2.PatNum AS OldHead, IF(p2.SuperFamily=0,p2.Guarantor,p2.SuperFamily) AS NewHead "
				+"FROM patient p1 "
				+"INNER JOIN patient p2 on p1.SuperFamily=p2.PatNum "//Bring on patient SuperFamily head
				+"AND p2.PatNum!=p2.SuperFamily";//Limit down to patients associated to invalid super family heads.
			DataTable table=Db.GetTable(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=table.Rows.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients in a SuperFamily with invalid super head")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					List<string> listInvSupFamHeadPatNums=new List<string>();
					int countFixed=0;
					int countValidPatsStartingSupFam=0;
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i=0;i<table.Rows.Count;i++){
						string patNum=POut.String(table.Rows[i]["PatNum"].ToString());
						string SupFamHeadNumOld=POut.String(table.Rows[i]["OldHead"].ToString());
						string SupFamHeadNumNew=POut.String(table.Rows[i]["NewHead"].ToString());
						if(!listInvSupFamHeadPatNums.Contains(SupFamHeadNumOld)) {//Only run UPDATE once per distinct invalid super head.
							listInvSupFamHeadPatNums.Add(SupFamHeadNumOld);
							//Get all patients who are currently in a valid Family (not SuperFamily), but will have their SuperFamily changed in order for the 
							//patients with an invalid SuperFamily head to be moved to a new valid SuperFamily. Only applies when the oldSuperFamilyHead is in a
							//Family which is not already part of a SuperFamily.
							command="SELECT patient.PatNum, patient.SuperFamily FROM patient "
								+"WHERE (patient.PatNum="+SupFamHeadNumNew+" AND "+"patient.SuperFamily!="+SupFamHeadNumNew+") "//New super head.
								+"OR (patient.Guarantor="+SupFamHeadNumNew+" AND "+"patient.SuperFamily!="+SupFamHeadNumNew+")";//Dependents.
							DataTable tableValidPatsMovingToNewSupFam=Db.GetTable(command);
							//Three groups of patients need to be updated.
							//1. Update all the patients who had an invalid SuperFamily head because they were invalid.
							//2. Update the new superfamily head to be in the SuperFamily because otherwise we would be reproducing the invalid SuperFamily head 
							//scenario with a different patient.
							//3. Update the dependents of the new SuperFamily head to be in the SuperFamily as well, because in the UI adding any family member to a 
							//SuperFamily will also bring along all other family members into the SuperFamily.
							command="UPDATE patient SET patient.SuperFamily="+SupFamHeadNumNew+" "
								+"WHERE patient.SuperFamily="+SupFamHeadNumOld+" "
								+"OR (patient.PatNum="+SupFamHeadNumNew+" AND "+"patient.SuperFamily!="+SupFamHeadNumNew+") "
								+"OR (patient.Guarantor="+SupFamHeadNumNew+" AND "+"patient.SuperFamily!="+SupFamHeadNumNew+")";
							//(All patients updated)-(prev valid patients)=patients fixed
							countFixed+=(int)Db.NonQ(command)-tableValidPatsMovingToNewSupFam.Rows.Count;
							countValidPatsStartingSupFam+=tableValidPatsMovingToNewSupFam.Rows.Count;
							//Since we are changing the SuperFamily on previously valid patients, we need to log them as well.

							for(int j=0;j<tableValidPatsMovingToNewSupFam.Rows.Count;j++){
								long validPatNum=PIn.Long(tableValidPatsMovingToNewSupFam.Rows[j]["PatNum"].ToString());
								long validSuperFamilyNum=PIn.Long(tableValidPatsMovingToNewSupFam.Rows[j]["SuperFamily"].ToString());
								DbmLog dbmLogUpdateSupFam=new DbmLog(Security.CurUser.UserNum,validPatNum,DbmLogFKeyType.Patient,//Log for each patient.
									DbmLogActionType.Update,methodName,"Updated SuperFamily from "+validSuperFamilyNum+" to "+SupFamHeadNumNew+".");
								listDbmLogs.Add(dbmLogUpdateSupFam);
							}
						}
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,PIn.Long(patNum),DbmLogFKeyType.Patient,//Log for each patient.
							DbmLogActionType.Update,methodName,"Updated SuperFamily from "+SupFamHeadNumOld+" to "+SupFamHeadNumNew+".");
						listDbmLogs.Add(dbmLog);
					}
					if(countFixed > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients in a SuperFamily with invalid super head fixed")+": "+countFixed.ToString()+"\r\n"
							+Lans.g("FormDatabaseMaintenance","Previously valid Patients incorporated into a new SuperFamily")+": "
							+countValidPatsStartingSupFam.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		//NOTE: 03/24/2016 Issue with DateTimeDeceased for a patient where the the DB value was "·,'*-0--,) 14:25:37" for customer 3202. 
		//Per Nathan if this occurs again we need to make a DBM to fix this.
		//[DbmMethod]
		//public static string PatientInvalidDateTimeDeceased(bool verbose,DBMMode modeCur) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,modeCur);
		//	}
		//	string log="";
		//	if(isCheck) {
		//		command="";
		//		int numFound=PIn.Int(Db.GetCount(command));
		//		if(numFound>0 || verbose) {
		//			log+=Lans.g("FormDatabaseMaintenance",": ")+numFound.ToString()+"\r\n";
		//		}
		//	}
		//	else {//fix
		//		command="";
		//		long numberFixed=Db.NonQ(command);
		//		if(numberFixed>0 || verbose) {
		//			log+=Lans.g("FormDatabaseMaintenance",": ")+numberFixed.ToString()+"\r\n";
		//		}
		//	}
		//	return log;
		//}

		[DbmMethodAttr(HasBreakDown = true,HasExplain=true)]
		public static string PatientNoClinicSet(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(!PrefC.HasClinicsEnabled) {
				return "";
			}
			//Get patients not assigned to a clinic:
			string command=@"SELECT PatNum,LName,FName FROM patient WHERE ClinicNum=0 AND PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Patients with no Clinic assigned: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count!=0) {
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							//Start a new line and indent every three patients for printing purposes.
							if(i%3==0) {
								log+="\r\n   ";
							}
							log+=table.Rows[i]["PatNum"].ToString()+"-"
							+table.Rows[i]["LName"].ToString()+", "
							+table.Rows[i]["FName"].ToString()+"; ";
						}
						log+="\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string PatientPriProvHidden(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command=@"
				SELECT provider.ProvNum,provider.Abbr
				FROM provider
				INNER JOIN patient ON patient.PriProv=provider.ProvNum
				WHERE provider.IsHidden=1
				GROUP BY provider.ProvNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Hidden providers with patients: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count<=0){
						break;
					}
					log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
					DataTable tablePat;
					for(int i = 0;i<table.Rows.Count;i++) {
						log+="     "+table.Rows[i]["Abbr"].ToString()+": ";
						command=@"SELECT PatNum,LName,FName FROM patient WHERE PriProv=(SELECT ProvNum FROM provider WHERE ProvNum="
							+table.Rows[i]["ProvNum"].ToString()+" AND IsHidden=1) LIMIT 10";
						tablePat=Db.GetTable(command);
						for(int j = 0;j<tablePat.Rows.Count;j++) {
							if(j>0) {
								log+=", ";
							}
							log+=tablePat.Rows[j]["PatNum"].ToString()+"-"+tablePat.Rows[j]["FName"].ToString()+" "+tablePat.Rows[j]["LName"].ToString();
						}
						log+="\r\n";
					}
					log+=Lans.g("FormDatabaseMaintenance","   Go to Lists | Providers to move all patients from the hidden provider to another provider.")+"\r\n";
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatientPriProvMissing(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=$@"SELECT COUNT(*) FROM patient 
						WHERE PriProv=0
						AND PatStatus!={POut.Int((int)PatientStatus.Deleted)}";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patient pri provs not set: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command=$@"SELECT PatNum FROM patient 
						WHERE PriProv=0 
						AND PatStatus!={POut.Int((int)PatientStatus.Deleted)}";
					List<long> listPatNums=Db.GetListLong(command);
					//previous versions of the program just dealt gracefully with missing provnum.
					//From now on, we can assum priprov is not missing, making coding easier.
					command=$@"UPDATE patient 
						SET PriProv={PrefC.GetString(PrefName.PracticeDefaultProv)} 
						WHERE PriProv=0 
						AND PatStatus!={POut.Int((int)PatientStatus.Deleted)}";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPatNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatNums[i],DbmLogFKeyType.Patient,DbmLogActionType.Update,
						methodName,"Updated PriProv from 0 to "+PrefC.GetString(PrefName.PracticeDefaultProv));
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patient pri provs fixed: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,IsReplicationUnsafe=true)]
		public static string PatientUnDeleteWithBalance(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT PatNum FROM patient "
				+"WHERE PatStatus=4 "
				+"AND (Bal_0_30 !=0 "
					+"OR Bal_31_60 !=0 "
					+"OR Bal_61_90 !=0 "
					+"OR BalOver90 !=0 "
					+"OR InsEst !=0 "
					+"OR BalTotal !=0)";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients found who are marked deleted with non-zero balances:")+" "+table.Rows.Count+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:  //No DB changes made, see breakdown if below.
				case DbmMode.Fix:
					Patient patientOld;
					List<Patient> listPatients=Patients.GetMultPats(table.Select().Select(x => PIn.Long(x["PatNum"].ToString())).ToList()).ToList();
					if(table.Rows.Count>0 || verbose) {
						if(dbmMode==DbmMode.Fix) {
							List<DbmLog> listDbmLogs=new List<DbmLog>();
							string methodName=MethodBase.GetCurrentMethod().Name;
							for(int i=0;i<listPatients.Count;i++){
								patientOld=listPatients[i].Copy();
								listPatients[i].LName=listPatients[i].LName+Lans.g("FormDatabaseMaintenance","DELETED");
								listPatients[i].PatStatus=PatientStatus.Archived;
								Patients.Update(listPatients[i],patientOld);
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatients[i].PatNum,DbmLogFKeyType.Patient,DbmLogActionType.Update,
									methodName,"Updated the PatStatus from Deleted to Archived from PatientUnDeleteWithBalance");
								listDbmLogs.Add(dbmLog);
							}
							Crud.DbmLogCrud.InsertMany(listDbmLogs);
							log+=Lans.g("FormDatabaseMaintenance","Patients with non-zero balances that have been undeleted:")+" "+listPatients.Count+"\r\n";
						}
						else {//Breakdown
							log+=Lans.g("FormDatabaseMaintenance","The following patients are marked as Deleted but have a balance.  "
								+"They will have 'DELETED' appended to their last name and their status will be changed to Archive.  "
								+"This will allow the account to be accessed so that it can be manually cleared and then deleted again.")+"\r\n\r\n";
							for(int i=0;i<listPatients.Count;i++){
								log+="#"+listPatients[i].PatNum+" - "+listPatients[i].GetNameFL();
								log+="\r\n";
							}
						}
					}
					break;
			}
			return log;
		}

		#endregion Patient------------------------------------------------------------------------------------------------------------------------------
		#region PatPlan, Payment------------------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatPlanDeleteWithInvalidInsSubNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM patplan WHERE InsSubNum NOT IN (SELECT InsSubNum FROM inssub)";
					string strCount=Db.GetCount(command);
					if(strCount!="0" || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Pat plans found with invalid InsSubNums: ")+strCount+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT PatPlanNum FROM patplan WHERE InsSubNum NOT IN (SELECT InsSubNum FROM inssub)";
					List<long> listPatPlanNums=Db.GetListLong(command);
					command="DELETE FROM patplan WHERE InsSubNum NOT IN (SELECT InsSubNum FROM inssub)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPatPlanNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatPlanNums[i],DbmLogFKeyType.PatPlan,DbmLogActionType.Delete,
							methodName,"Deleted patplan from PatPlanDeleteWithInvalidInsSubNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Pat plans with invalid InsSubNums deleted: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatPlanDeleteWithInvalidPatNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM patplan WHERE PatNum NOT IN (SELECT PatNum FROM patient)";
					string strCount=Db.GetCount(command);
					if(strCount!="0" || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Pat plans found with invalid PatNums: ")+strCount+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT PatPlanNum FROM patplan WHERE PatNum NOT IN (SELECT PatNum FROM patient)";
					List<long> listPatPlanNums=Db.GetListLong(command);
					command="DELETE FROM patplan WHERE PatNum NOT IN (SELECT PatNum FROM patient)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPatPlanNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPatPlanNums[i],DbmLogFKeyType.PatPlan,DbmLogActionType.Delete,
							methodName,"Deleted patplan from PatPlanDeleteWithInvalidPatNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Pat plans with invalid PatNums deleted: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string PatPlanOrdinalDuplicates(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT patient.PatNum,patient.LName,patient.FName,COUNT(*) "
				+"FROM patplan "
				+"INNER JOIN patient ON patient.PatNum=patplan.PatNum "
				+"GROUP BY patplan.PatNum,patplan.Ordinal "
				+"HAVING COUNT(*)>1";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","PatPlan duplicate ordinals: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   #"+table.Rows[i]["PatNum"].ToString()+" - "+PIn.String(table.Rows[i]["FName"].ToString())+" "
								+PIn.String(table.Rows[i]["LName"].ToString())+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatPlanOrdinalZeroToOne(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT PatPlanNum,PatNum FROM patplan WHERE Ordinal=0";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PatPlan ordinals currently zero: ")+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i = 0;i<table.Rows.Count;i++) {
						PatPlan patPlan=PatPlans.GetPatPlan(PIn.Long(table.Rows[i][1].ToString()),0);
						if(patPlan==null) {//Unlikely but possible if plan gets deleted by a user during this check.
							continue;
						}
						PatPlans.SetOrdinal(patPlan.PatPlanNum,1);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patPlan.PatPlanNum,DbmLogFKeyType.PatPlan,DbmLogActionType.Update,
							methodName,"PatPlan ordinal changed from 0 to 1 from PatPlanOrdinalZeroToOne.");
						listDbmLogs.Add(dbmLog);
						numberFixed++;
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PatPlan ordinals changed from 0 to 1: ")+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PatPlanOrdinalTwoToOne(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT PatPlanNum,PatNum FROM patplan patplan1 WHERE Ordinal=2 AND NOT EXISTS("
				+"SELECT * FROM patplan patplan2 WHERE patplan1.PatNum=patplan2.PatNum AND patplan2.Ordinal=1)";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PatPlans for secondary found where no primary ins: ")+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					for(int i = 0;i<table.Rows.Count;i++) {
						PatPlan patPlan=PatPlans.GetPatPlan(PIn.Long(table.Rows[i][1].ToString()),2);
						if(patPlan==null) {//Unlikely but possible if plan gets deleted by a user during this check.
							continue;
						}
						PatPlans.SetOrdinal(patPlan.PatPlanNum,1);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patPlan.PatPlanNum,DbmLogFKeyType.PatPlan,DbmLogActionType.Update,
							methodName,"PatPlan ordinal changed from 2 to 1 from PatPlanOrdinalTwoToOne.");
						listDbmLogs.Add(dbmLog);
						numberFixed++;
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","PatPlan ordinals changed from 2 to 1 if no primary ins: ")+numberFixed+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Shows payments that have a PaymentAmt that doesn't match the sum of all associated PaySplits.  
		///Payments with no PaySplits are dealt with in PaymentMissingPaySplit()</summary>
		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string PaymentAmtNotMatchPaySplitTotal(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Note that this just returns info for a (seemingly) random patient that has a paysplit for the payment.
			//This is because the payment only shows in the ledger for the patient with the paysplit, not the patient on the payment.
			string command="SELECT patient.PatNum, patient.LName, patient.FName, payment.PayDate "
				+"FROM payment "
				+"INNER JOIN ( "
					+"SELECT paysplit.PayNum, SUM(paysplit.SplitAmt) totSplitAmt, MIN(paysplit.PatNum) PatNum "
					+"FROM paysplit "
					+"GROUP BY paysplit.PayNum "
				+") pstotals ON pstotals.PayNum=payment.PayNum "
				+"INNER JOIN patient ON patient.PatNum=pstotals.PatNum "
				+"WHERE payment.PayAmt!=0 "
				+"AND ROUND(payment.PayAmt,2)!=ROUND(pstotals.totSplitAmt,2) "
				+"ORDER BY patient.LName, patient.FName, payment.PayDate";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Payments with amounts that do not match the total split(s) amounts")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   "+table.Rows[i]["PatNum"].ToString();
							log+="  "+Patients.GetNameLF(table.Rows[i]["LName"].ToString(),table.Rows[i]["FName"].ToString(),"","");
							log+="  "+PIn.DateT(table.Rows[i]["PayDate"].ToString()).ToShortDateString();
							log+="\r\n";
						}
						log+="   "+Lans.g("FormDatabaseMaintenance","They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PaymentDetachMissingDeposit(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM payment "
						+"WHERE DepositNum != 0 "
						+"AND NOT EXISTS(SELECT * FROM deposit WHERE deposit.DepositNum=payment.DepositNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Payments attached to deposits that no longer exist: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					string where="WHERE DepositNum != 0 AND NOT EXISTS(SELECT * FROM deposit WHERE deposit.DepositNum=payment.DepositNum)";
					command="SELECT * FROM payment "+where;
					List<Payment> listPayments=Crud.PaymentCrud.SelectMany(command);
					command="UPDATE payment SET DepositNum=0 "+where;
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPayments.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPayments[i].PayNum,DbmLogFKeyType.Payment,DbmLogActionType.Update,
							methodName,"Updated DepositNum from "+listPayments[i].DepositNum+" to 0.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Payments detached from deposits that no longer exist: ")
						+numberFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PaymentMissingPaySplit(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM payment "
						+"WHERE PayNum NOT IN (SELECT PayNum FROM paysplit) "//Payments with no split that are
						+"AND ((DepositNum=0) "                              //not attached to a deposit
						+"OR (DepositNum!=0 AND PayAmt=0))";                 //or attached to a deposit with no amount.
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Payments with no attached paysplit: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					string where="WHERE PayNum NOT IN (SELECT PayNum FROM paysplit) "//Payments with no split that are
						+"AND ((DepositNum=0) "                              //not attached to a deposit
						+"OR (DepositNum!=0 AND PayAmt=0))";                 //or attached to a deposit with no amount.
					command="SELECT PayNum FROM payment "+where;
					List<long> listPayNums=Db.GetListLong(command);
					command="DELETE FROM payment "+where;
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listPayNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPayNums[i],DbmLogFKeyType.Payment,
							DbmLogActionType.Delete,methodName,"Deleted payment from PaymentMissingPaySplit.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Payments with no attached paysplit fixed: ")+numberFixed+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		#endregion PatPlan, Payment---------------------------------------------------------------------------------------------------------------------
		#region PayPlanCharge, PayPlan, PaySplit--------------------------------------------------------------------------------------------------------

		//DEPRECATED. This no longer holds true with Payment Plans as of Version 16.2.
		//[DbmMethod]
		//public static string PayPlanChargeGuarantorMatch(bool verbose,DBMMode modeCur) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,modeCur);
		//	}
		//	if(DataConnection.DBtype==DatabaseType.Oracle) {
		//		return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
		//	}
		//	string log="";
		//	if(isCheck) {
		//		int numFound=0;
		//		command="SELECT COUNT(*) FROM payplancharge,payplan "
		//			+"WHERE payplan.PayPlanNum=payplancharge.PayPlanNum "
		//			+"AND payplancharge.Guarantor != payplan.Guarantor";
		//		numFound+=PIn.Int(Db.GetCount(command));
		//		command="SELECT COUNT(*) FROM payplancharge,payplan "
		//			+"WHERE payplan.PayPlanNum=payplancharge.PayPlanNum "
		//			+"AND payplancharge.PatNum != payplan.PatNum";
		//		numFound+=PIn.Int(Db.GetCount(command));
		//		if(numFound>0 || verbose) {
		//			log+=Lans.g("FormDatabaseMaintenance","PayPlanCharge guarantors and pats not matching payplan guarantors and pats: ")+numFound+"\r\n";
		//		}
		//	}
		//	else {
		//		//Fix the cases where payplan.Guarantor and payplan.PatNum are not zero. 
		//		command="UPDATE payplan,payplancharge "
		//			+"SET payplancharge.Guarantor=payplan.Guarantor "
		//			+"WHERE payplan.PayPlanNum=payplancharge.PayPlanNum "
		//			+"AND payplancharge.Guarantor != payplan.Guarantor "
		//		+"AND payplan.Guarantor != 0";
		//		long numFixed=Db.NonQ(command);
		//		command="UPDATE payplan,payplancharge "
		//			+"SET payplancharge.PatNum=payplan.PatNum "
		//			+"WHERE payplan.PayPlanNum=payplancharge.PayPlanNum "
		//			+"AND payplancharge.PatNum != payplan.PatNum "
		//		+"AND payplan.PatNum != 0";
		//		numFixed+=Db.NonQ(command);
		//		if(numFixed>0 || verbose) {
		//			log+=Lans.g("FormDatabaseMaintenance","PayPlanCharge guarantors and pats fixed to match payplan: ")+numFixed+"\r\n";
		//		}
		//		//No fix yet if payplan.Guarantor or payplan.PatNum are zero but there are good values in PayPlanCharge.
		//	}
		//	return log;
		//}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string PayPlanChargeProvNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT pat.PatNum AS 'PatNum',pat.LName AS 'PatLName',pat.FName AS 'PatFName',guar.PatNum AS 'GuarNum',guar.LName AS 'GuarLName',guar.FName AS 'GuarFName',payplan.PayPlanDate "
				+"FROM payplancharge "
				+"LEFT JOIN payplan ON payplancharge.PayPlanNum=payplan.PayPlanNum "
				+"LEFT JOIN patient pat ON payplan.PatNum=pat.PatNum "
				+"LEFT JOIN patient guar ON payplan.Guarantor=guar.PatNum "
				+"WHERE payplancharge.ProvNum=0 "
				+"GROUP BY payplancharge.PayPlanNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Pay plans with charges that have providers missing: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   "+Lans.g("FormDatabaseMaintenance","Pay Plan Date")+": "+PIn.DateT(table.Rows[i]["PayPlanDate"].ToString()).ToShortDateString()+"\r\n"
							+"      "+Lans.g("FormDatabaseMaintenance","Guarantor")+": #"+table.Rows[i]["PatNum"]+" - "+table.Rows[i]["PatFName"]+" "+table.Rows[i]["PatLName"]+"\r\n"
							+"      "+Lans.g("FormDatabaseMaintenance","For Patient")+": #"+table.Rows[i]["GuarNum"]+" - "+table.Rows[i]["GuarFName"]+" "+table.Rows[i]["GuarLName"]+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PayPlanChargeWithInvalidPayPlanNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT COUNT(DISTINCT PayPlanNum) FROM (SELECT PayPlanNum FROM payplancharge WHERE PayPlanNum NOT IN(SELECT PayPlanNum FROM payplan) "
				+"UNION SELECT PayPlanNum FROM creditcard WHERE PayPlanNum>0 AND PayPlanNum NOT IN(SELECT PayPlanNum FROM payplan)) A";
			int count=PIn.Int(Db.GetCount(command));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(count!=0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PayPlan charges or credit cards with an invalid PayPlanNum found")+": "+count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(count!=0 || verbose) {
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						string methodName=MethodBase.GetCurrentMethod().Name;
						string where="WHERE PayPlanNum NOT IN(SELECT PayPlanNum FROM payplan)";
						//Delete the payment plan charges and update credit cards that point to an invalid payment plan. Claimprocs and paysplits with an invalid
						//PayPlanNum are taken care of in other DBM methods.
						command="SELECT PayPlanChargeNum FROM payplancharge "+where;
						List<long> listPrikeys=Db.GetListLong(command);
						command="DELETE FROM payplancharge "+where;
						Db.NonQ(command);
						for(int i=0;i<listPrikeys.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPrikeys[i],DbmLogFKeyType.PayPlanCharge,DbmLogActionType.Delete,
								methodName,"Deleted paysplancharge from PayPlanChargeWithInvalidPayPlanNum.");
							listDbmLogs.Add(dbmLog);
						}
						command="SELECT CreditCardNum FROM creditcard "+where;
						listPrikeys=Db.GetListLong(command);
						command="UPDATE creditcard SET PayPlanNum=0 "+where;
						Db.NonQ(command);
						for(int i=0;i<listPrikeys.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listPrikeys[i],DbmLogFKeyType.CreditCard,DbmLogActionType.Update,
								methodName,"Set creditcard.PayPlanNum to 0 from PayPlanChargeWithInvalidPayPlanNum.");
							listDbmLogs.Add(dbmLog);
						}
						log+=Lans.g("FormDatabaseMaintenance","PayPlan charges or credit cards with an invalid PayPlanNum fixed")+": "+count.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string PaySplitAttachedToDeletedProc(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT paysplit.SplitNum "
				+"FROM paysplit "
				+"INNER JOIN procedurelog ON paysplit.ProcNum=procedurelog.ProcNum AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.D)+" ";
			List<long> listSplitNums=Db.GetListLong(command);
			if(listSplitNums.Count==0 && !verbose) {
				return "";
			}
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					log+=Lans.g("FormDatabaseMaintenance","Payment splits attached to deleted procedures")+": "+listSplitNums.Count;
					break;
				case DbmMode.Fix:
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					if(!listSplitNums.IsNullOrEmpty()) {
						command=$"UPDATE paysplit SET paysplit.ProcNum=0 WHERE paysplit.SplitNum IN({string.Join(",",listSplitNums)})";
						Db.NonQ(command);
					}
					for(int i=0;i<listSplitNums.Count;i++) {
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSplitNums[i],DbmLogFKeyType.PaySplit,
							DbmLogActionType.Update,methodName,"Updated ProcNum to 0 from "+methodName+".");
						listDbmLogs.Add(dbmLog);
					}
					log+=Lans.g("FormDatabaseMaintenance","Payment splits with deleted procedures detached")+": "+listSplitNums.Count;
					if(!listDbmLogs.IsNullOrEmpty()) {
						DbmLogs.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		/// <summary>Shows patients that have paysplits attached to insurance payment plans.</summary>
		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string PaySplitAttachedToInsurancePaymentPlan(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			DataTable table=GetPaySplitsAttachedToInsurancePaymentPlan();
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Paysplits attached to insurance payment plans")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+=Lans.g("FormDatabaseMaintenance","Manual fix needed. Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count<=0) {
						break;
					}
					log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
					for(int i=0;i<table.Rows.Count;i++) {
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Patient #")+" "+table.Rows[i]["PatNum"].ToString()+" "
							+Lans.g("FormDatabaseMaintenance","Amount")+": "+PIn.Double(table.Rows[i]["SplitAmt"].ToString()).ToString("c")+" "
							+Lans.g("FormDatabaseMaintenance","Date")+": "+PIn.Date(table.Rows[i]["DatePay"].ToString()).ToShortDateString()+" "
							+Lans.g("FormDatabaseMaintenance","Insurance payment plan #")+table.Rows[i]["PayPlanNum"];
					}
					log+="\r\n"+Lans.g("FormDatabaseMaintenance","Run 'Pay Plan Payments' in the Tools tab to fix these payments.");
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = false,IsReplicationUnsafe=true)]
		public static string PaySplitAttachedToItself(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT * FROM paysplit WHERE FSplitNum=SplitNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					log+=Lans.g("FormDatabaseMaintenance","Paysplits attached to themselves")+": "+table.Rows.Count+"\r\n";
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listSplitNums=table.Select().Select(x => PIn.Long(x["SplitNum"].ToString())).ToList();
					command="UPDATE paysplit SET paysplit.FSplitNum=0 WHERE paysplit.FSplitNum=paysplit.SplitNum";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listSplitNums.Count;i++) {
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSplitNums[i],DbmLogFKeyType.PaySplit,DbmLogActionType.Update,
							methodName,"Updated FSplitNum to 0 from PaySplitAttachedToItself.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplits with invalid FSplitNums fixed: ")+numFixed+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsOneOff=true, IsReplicationUnsafe=true)]
		public static string PaySplitTransfersWithNoUnearnedType(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string methodName=MethodBase.GetCurrentMethod().Name;
			//If the parent is an unearned split (has a prepayment type) then update the unearned type on the current split to match the unearned type of the parent.
			string command=@"SELECT pschild.SplitNum,psparent.UnearnedType FROM paysplit pschild
				INNER JOIN paysplit psparent ON pschild.FSplitNum=psparent.SplitNum
				WHERE pschild.SplitAmt < 0 
				AND pschild.FSplitNum!=0 
				AND pschild.UnearnedType=0
				AND pschild.AdjNum=0
				AND pschild.ProcNum=0
				AND pschild.PayPlanNum=0
				AND psparent.UnearnedType!=0
				AND psparent.UnearnedType!=pschild.UnearnedType";
			DataTable tableSplitsWithoutUnearnedType=Db.GetTable(command);
			int count=tableSplitsWithoutUnearnedType.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
					if(count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplit transfers with no UnearnedType")+": "+count;
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					for(int i=0;i<tableSplitsWithoutUnearnedType.Rows.Count;i++){
						long splitNum=PIn.Long(tableSplitsWithoutUnearnedType.Rows[i]["SplitNum"].ToString());
						long unearnedType=PIn.Long(tableSplitsWithoutUnearnedType.Rows[i]["UnearnedType"].ToString());
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,splitNum,DbmLogFKeyType.PaySplit,DbmLogActionType.Update,
							methodName,"Updated unearned type from 0 to "+unearnedType.ToString());
						listDbmLogs.Add(dbmLog);
						command=$@"UPDATE paysplit 
							SET UnearnedType = {POut.Long(unearnedType)} 
							WHERE SplitNum = {POut.Long(splitNum)}";
						Db.NonQ(command);
					}
					DbmLogs.InsertMany(listDbmLogs);
					if(count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplit transfers with no UnearnedType fixed")+": "+count;
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasPatNum=true,IsReplicationUnsafe=true)]
		public static string PaySplitUnearnedAttachedToCompletedProcedures(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			//Find every completed procedure that has a unearned payment splits associated that do not balance out.
			//There are scenarios where it is acceptable to have unearned splits linked to a completed procedure but those splits should offset each other.
			//E.g. TP prepayment of X will be offset by a transfer split of -X once the procedure has been set complete.
			//Any unearned money that is 'explicitly' linked to a procedure will be money that is 'locked' into unearned and offices do not like seeing Unearned label with a value.
			//Also, there is no such thing as unearned money being explicitly linked to a completed procedure because that money is technically 'earned' once the proc is complete.
			string command=$@"SELECT paysplit.ProcNum,SUM(paysplit.SplitAmt) SumUnearned
				FROM paysplit
				INNER JOIN procedurelog ON paysplit.ProcNum=procedurelog.ProcNum
				WHERE paysplit.UnearnedType > 0
				AND paysplit.ProcNum > 0
				{((patNum > 0) ? $"AND paysplit.PatNum={POut.Long(patNum)}" : "")}
				AND procedurelog.ProcStatus={POut.Enum(ProcStat.C)}
				GROUP BY paysplit.ProcNum
				HAVING SumUnearned > 0";
			List<long> listProcNums=Db.GetListLong(command);
			//Get the list of specific PaySplitNums that will be impacted by this DBM method for logging purposes.
			List<long> listSplitNums=new List<long>();
			if(listProcNums.Count > 0) {
				command=$@"SELECT SplitNum
					FROM paysplit
					WHERE paysplit.UnearnedType > 0
					AND paysplit.ProcNum IN({string.Join(",",listProcNums)})";
				listSplitNums=Db.GetListLong(command);
			}
			switch(dbmMode) {
				case DbmMode.Check:
					if(listSplitNums.Count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Unearned payment splits attached to a completed procedure found")+": "+listSplitNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					//It is safe to remove the Unearned Type on every payment split for completed procedures since all of that money is technically 'earned' at this point.
					//Payment splits become available for income transfers once the unearned type has been removed.
					//That and the Unearned label in the Account module will no longer inclue these values.
					if(listSplitNums.Count > 0) {
						command=$@"UPDATE paysplit 
							SET UnearnedType=0
							WHERE SplitNum IN ({string.Join(",",listSplitNums)})";
						Db.NonQ(command);
						string dbmLogMsg=Lans.g("FormDatabaseMaintenance","Removed the UnearnedType from this payment split since it is attached to a completed procedure.");
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						for(int i=0;i<listSplitNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSplitNums[i],DbmLogFKeyType.PaySplit,
								DbmLogActionType.Update,MethodBase.GetCurrentMethod().Name,dbmLogMsg);
							listDbmLogs.Add(dbmLog);
						}
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					if(listSplitNums.Count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Unearned payment splits attached to a completed procedure fixed")+": "+listSplitNums.Count+"\r\n";
					}
					break;
			}
			return log;
		}
		
		[DbmMethodAttr(HasPatNum = true,IsReplicationUnsafe=true)]
		public static string PaySplitWithInvalidPayNum(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			//Get the unique PayNums for orphaned paysplits.
			string command="SELECT paysplit.PayNum FROM paysplit WHERE NOT EXISTS(SELECT * FROM payment WHERE paysplit.PayNum=payment.PayNum) ";
			if(patNum > 0) {
				command+=$"AND paysplit.PatNum={POut.Long(patNum)} ";
			}
			command+="GROUP BY paysplit.PayNum";
			List<long> listPayNums=Db.GetListLong(command);
			int countFound=0;
			List<DataRow> listDataRows=new List<DataRow>();
			//Get the all of the data necessary to create dummy payments for ALL payment splits associated to the orphaned PayNums.
			if(!listPayNums.IsNullOrEmpty()) {
				command=$@"SELECT PayNum,PatNum,DatePay,DateEntry,SUM(SplitAmt) SplitAmt_,COUNT(*) Count_
					FROM paysplit
					WHERE PayNum IN({string.Join(",",listPayNums.Select(x => POut.Long(x)))})
					GROUP BY PayNum";
				DataTable table=Db.GetTable(command);
				listDataRows=table.Select().ToList();
				//The query results were grouped by PayNum so sum the value in the Count_ column for all of the rows returned.
				countFound=(int)listDataRows.Sum(x => PIn.Long(x["Count_"].ToString()));
			}
			switch(dbmMode) {
				case DbmMode.Check:
					if(countFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplits found with invalid PayNum:")+" "+countFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					int countFixed=0;
					bool hasPayNumZero=false;
					List<Def> listDefsPaymentTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
					#region PayNum Not Zero
					if(listDataRows.Count > 0) {
						for(int i=0;i<listDataRows.Count;i++) {
							long payNum=PIn.Long(listDataRows[i]["PayNum"].ToString());
							if(payNum==0) {
								hasPayNumZero=true;
								continue;
							}
							//There's only one place in the program where this is called from.  Date is today, so no need to validate the date.
							Payment payment=new Payment();
							payment.PayType=listDefsPaymentTypes[0].DefNum;
							payment.DateEntry=PIn.Date(listDataRows[i]["DateEntry"].ToString());
							payment.PatNum=PIn.Long(listDataRows[i]["PatNum"].ToString());
							payment.PayDate=PIn.Date(listDataRows[i]["DatePay"].ToString());
							payment.PayAmt=PIn.Double(listDataRows[i]["SplitAmt_"].ToString());
							payment.PayNote="Dummy payment. Original payment entry missing from the database.";
							payment.PayNum=payNum;
							//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
							payment.SecUserNumEntry=Security.CurUser.UserNum;
							payment.PaymentSource=CreditCardSource.None;
							payment.ProcessStatus=ProcessStat.OfficeProcessed;
							payment.IsCcCompleted=true;
							Payments.Insert(payment,true);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,payment.PayNum,DbmLogFKeyType.Payment,
								DbmLogActionType.Insert,methodName,"Inserted payment from PaySplitWithInvalidPayNum.");
							listDbmLogs.Add(dbmLog);
							countFixed+=PIn.Int(listDataRows[i]["Count_"].ToString());
						}
					}
					#endregion
					#region PayNum Zero
					if(hasPayNumZero) {
						//Handling paysplits that have a pay num of 0 separately because we want to create one payment per patient per day
						command="SELECT PatNum,DatePay,DateEntry,SUM(SplitAmt) SplitAmt_,COUNT(*) Count_ FROM paysplit WHERE PayNum=0 ";
						if(patNum > 0) {
							command+=$"AND PatNum={POut.Long(patNum)} ";
						}
						command+="GROUP BY PatNum,DatePay";
						DataTable table=Db.GetTable(command);
						for(int i=0;i<table.Rows.Count;i++) {
							Payment payment=new Payment();
							payment.PayType=listDefsPaymentTypes[0].DefNum;
							payment.DateEntry=PIn.Date(table.Rows[i]["DateEntry"].ToString());
							payment.PatNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
							payment.PayDate=PIn.Date(table.Rows[i]["DatePay"].ToString());
							payment.PayAmt=PIn.Double(table.Rows[i]["SplitAmt_"].ToString());
							payment.PayNote="Dummy payment. Original payment entry number was 0.";
							//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
							payment.SecUserNumEntry=Security.CurUser.UserNum;
							payment.PaymentSource=CreditCardSource.None;
							payment.ProcessStatus=ProcessStat.OfficeProcessed;
							payment.IsCcCompleted=true;
							Payments.Insert(payment);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,payment.PayNum,DbmLogFKeyType.Payment,
								DbmLogActionType.Insert,methodName,"Inserted payment from PaySplitWithInvalidPayNum.");
							listDbmLogs.Add(dbmLog);
							command="SELECT SplitNum FROM paysplit WHERE PayNum=0 AND PatNum="+POut.Long(payment.PatNum)+" AND DatePay="+POut.Date(payment.PayDate);
							List<long> listSplitNums=Db.GetListLong(command);
							command=$@"UPDATE paysplit SET PayNum={POut.Long(payment.PayNum)}
								WHERE SplitNum IN({string.Join(",",listSplitNums.Select(x => POut.Long(x)))})";
							Db.NonQ(command);
							for(int j=0;j<listSplitNums.Count;j++){
								DbmLog dbmLog2=new DbmLog(Security.CurUser.UserNum,listSplitNums[j],DbmLogFKeyType.PaySplit,
									DbmLogActionType.Update,methodName,"Updated PayNum from 0 to "+payment.PayNum+".");
								listDbmLogs.Add(dbmLog2);
							}
							countFixed+=listSplitNums.Count;
						}
					}
					#endregion
					if(countFixed > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplits found with invalid PayNum fixed:")+" "+countFixed.ToString()+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PaySplitWithInvalidPayPlanChargeNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT paysplit.SplitNum FROM paysplit WHERE paysplit.PayPlanNum=0 AND paysplit.PayPlanChargeNum!=0";
			List<long> listSplitNums=Db.GetListLong(command);
			switch(dbmMode) {
				case DbmMode.Check:
					if(listSplitNums.Count > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplits found with an invalid PayPlanChargeNum:")+" "+listSplitNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listSplitNums.Count > 0) {
						command=$@"UPDATE paysplit 
							SET paysplit.PayPlanChargeNum=0
							WHERE paysplit.SplitNum IN ({string.Join(",",listSplitNums.Select(x => POut.Long(x)))})";
						Db.NonQ(command);
					}
					if(listSplitNums.Count > 0 || verbose) {
						string methodName=MethodBase.GetCurrentMethod().Name;
						log+=Lans.g("FormDatabaseMaintenance","Paysplits with an invalid PayPlanChargeNum fixed:")+" "+listSplitNums.Count+"\r\n";
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						for(int i=0;i<listSplitNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSplitNums[i],DbmLogFKeyType.PaySplit,
								DbmLogActionType.Update,methodName,"Updated PayPlanChargeNum to 0.");
							listDbmLogs.Add(dbmLog);
						}
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PaySplitWithInvalidPayPlanNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM paysplit WHERE paysplit.PayPlanNum!=0 AND paysplit.PayPlanNum NOT IN(SELECT payplan.PayPlanNum FROM payplan)";
					int numFound=PIn.Int(Db.GetScalar(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplits found with invalid PayPlanNum: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					string where="WHERE paysplit.PayPlanNum!=0 AND paysplit.PayPlanNum NOT IN(SELECT payplan.PayPlanNum FROM payplan)";
					command="SELECT SplitNum FROM paysplit "+where;
					List<long> listSplitNums=Db.GetListLong(command);
					command="UPDATE paysplit SET paysplit.PayPlanNum=0 "+where;
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listSplitNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listSplitNums[i],DbmLogFKeyType.PaySplit,DbmLogActionType.Update,
							methodName,"Updated PayPlanNum to 0 from PaySplitWithInvalidPayPlanNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Paysplits with invalid PayPlanNums fixed: ")+numFixed+"\r\n";
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string PaySplitWithInvalidPrePayNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT ps1.* FROM paysplit ps1 LEFT JOIN paysplit ps2 ON ps1.FSplitNum=ps2.SplitNum WHERE ps1.FSplitNum!=0 AND ps2.SplitNum IS NULL";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Paysplits attached to deleted prepayments")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count<=0) {
						break;
					}
					log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
					for(int i = 0;i<table.Rows.Count;i++) {
						log+="   "+Lans.g("FormDatabaseMaintenance","PatNum")+": #"+table.Rows[i]["PatNum"].ToString();
						log+=" "+PIn.DateT(table.Rows[i]["DatePay"].ToString()).ToShortDateString();
						log+=" "+PIn.Double(table.Rows[i]["SplitAmt"].ToString()).ToString("c");
						log+="\r\n";
					}
					log+="   "+Lans.g("FormDatabaseMaintenance","They need to be fixed manually.")+"\r\n";
					break;
			}
			return log;
		}

		#endregion PayPlanCharge, PayPlan, PaySplit-----------------------------------------------------------------------------------------------------
		#region PerioMeasure, Preference---------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsOneOff=true)]
		public static string PerioMeasureDuplicateMissingTeeth(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			command=@$"SELECT PerioMeasureNum FROM periomeasure WHERE SequenceType={POut.Enum<PerioSequenceType>(PerioSequenceType.SkipTooth)} GROUP BY PerioExamNum,IntTooth";
			List<long> listPerioMeasureNumsAllowed=Db.GetListLong(command);
			if(listPerioMeasureNumsAllowed.Count==0) {
				if(verbose) {
					log+=Lans.g("FormDatabaseMaintenance","PerioMeasures that are duplicates of missing teeth:")+" "+0+"\r\n";
				}
				return log;
			}
			command=$"SELECT COUNT(*) FROM periomeasure WHERE SequenceType={POut.Enum<PerioSequenceType>(PerioSequenceType.SkipTooth)} AND PerioMeasureNum NOT IN ({String.Join(",",listPerioMeasureNumsAllowed)})";
			int countDuplicate=Db.GetInt(command);
			switch(dbmMode) {
				case DbmMode.Breakdown:
				case DbmMode.Check:
				if(countDuplicate>0 || verbose) {
					log+=Lans.g("FormDatabaseMaintenance","PerioMeasures found that are duplicates of missing teeth:")+" "+countDuplicate+"\r\n";
				}
				break;
				case DbmMode.Fix:
					if(countDuplicate > 0) {
						command=@$"DELETE FROM periomeasure WHERE SequenceType={POut.Enum<PerioSequenceType>(PerioSequenceType.SkipTooth)} AND PerioMeasureNum NOT IN ({String.Join(",",listPerioMeasureNumsAllowed)})";
						Db.NonQ(command);
					}
					if(countDuplicate>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PerioMeasures that were duplicates of missing teeth deleted:")+" "+countDuplicate+"\r\n";
					}
				break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string PerioMeasureWithInvalidIntTooth(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM periomeasure WHERE IntTooth > 32 OR IntTooth < 1";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PerioMeasures found with invalid tooth number: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"DELETE FROM periomeasure WHERE IntTooth > 32 OR IntTooth < 1";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","PerioMeasures deleted due to invalid tooth number: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string PhoneNumbersWithEmptyDigits(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM phonenumber WHERE phonenumber.PhoneNumberDigits='' AND phonenumber.PhoneType="+POut.Enum(PhoneType.Other);
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Unsearchable Other Numbers found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT * FROM phonenumber WHERE phonenumber.PhoneNumberDigits='' AND phonenumber.PhoneType="+POut.Enum(PhoneType.Other);
					DataTable table=Db.GetTable(command);
					if(table.Rows.Count<=0 && !verbose){
						break;
					}
					int numberFixed=0;
					List<PhoneNumber> listPhoneNumbers=Crud.PhoneNumberCrud.TableToList(table);
					for(int i=0;i<listPhoneNumbers.Count;i++) {
						string phoneNumberDigits=PhoneNumbers.RemoveNonDigitsAndTrimStart(listPhoneNumbers[i].PhoneNumberVal);
						if(phoneNumberDigits==listPhoneNumbers[i].PhoneNumberDigits) {
							continue;
						}
						listPhoneNumbers[i].PhoneNumberDigits=phoneNumberDigits;
						PhoneNumbers.Update(listPhoneNumbers[i]);
						numberFixed++;
					}
					log+=Lans.g("FormDatabaseMaintenance","Unsearchable Other Numbers fixed")+": "+numberFixed.ToString()+"\r\n";
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PreferenceAllergiesIndicateNone(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM allergydef where AllergyDefNum="+POut.Long(PrefC.GetLong(PrefName.AllergiesIndicateNone));
					if(PIn.Int(Db.GetCount(command))==0 && PrefC.GetString(PrefName.AllergiesIndicateNone)!="") {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"AllergyIndicatesNone\" is an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"AllergyIndicatesNone\" checked.")+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT COUNT(*) FROM allergydef where AllergyDefNum="+POut.Long(PrefC.GetLong(PrefName.AllergiesIndicateNone));
					if(PIn.Int(Db.GetCount(command))==0 && PrefC.GetString(PrefName.AllergiesIndicateNone)!="") {
						Prefs.UpdateString(PrefName.AllergiesIndicateNone,"");
						Signalods.SetInvalid(InvalidType.Prefs);
						log+=Lans.g("FormDatabaseMaintenance","Preference \"AllergyIndicatesNone\" set to blank due to an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"AllergyIndicatesNone\" checked.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true,HasExplain=true)]
		public static string PreferenceDateDepositsStarted(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			DateTime date=PrefC.GetDate(PrefName.DateDepositsStarted);
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					if(date<DateTime.Now.AddMonths(-1)) {
						log+=Lans.g("FormDatabaseMaintenance","Deposit start date needs to be reset.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deposit start date checked.")+"\r\n";
					}
					break;
				case DbmMode.Fix:
					//If the program locks up when trying to create a deposit slip, it's because someone removed the start date from the deposit edit window. Run this query to get back in.
					if(date<DateTime.Now.AddMonths(-1)) {
						command="UPDATE preference SET ValueString="+POut.Date(DateTime.Today.AddDays(-21))
						+" WHERE PrefName='DateDepositsStarted'";
						Db.NonQ(command);
						Signalods.SetInvalid(InvalidType.Prefs);
						log+=Lans.g("FormDatabaseMaintenance","Deposit start date reset.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deposit start date checked.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string PreferenceInsBillingProv(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(dbmMode==DbmMode.Breakdown) {
				return "";
			}
			string log="";
			long insBillingProv=PrefC.GetLong(PrefName.InsBillingProv);
			Provider provider=Providers.GetProv(insBillingProv);
			// 0 means the program will use the default practice provider. -1 means Treating Provider.
			if(insBillingProv==0 || insBillingProv==-1 || provider!=null) {
				if(verbose) {
					log+=Lans.g("FormDatabaseMaintenance","Default insurance billing provider verified.")+"\r\n";
				}
				return log;
			}
			log+=Lans.g("FormDatabaseMaintenance","Invalid default insurance billing provider set.")+"\r\n";
			if(dbmMode!=DbmMode.Check) {
				Prefs.UpdateLong(PrefName.InsBillingProv,0);//Set it to zero so it can default to the practice provider.
				log+="  "+Lans.g("FormDatabaseMaintenance","Fixed.")+"\r\n";
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PreferenceMedicationsIndicateNone(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM medication where MedicationNum="+POut.Long(PrefC.GetLong(PrefName.MedicationsIndicateNone));
					if(PIn.Int(Db.GetCount(command))==0 && PrefC.GetString(PrefName.MedicationsIndicateNone)!="") {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"MedicationsIndicateNone\" is an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"MedicationsIndicateNone\" checked.")+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT COUNT(*) FROM medication where MedicationNum="+POut.Long(PrefC.GetLong(PrefName.MedicationsIndicateNone));
					if(PIn.Int(Db.GetCount(command))==0 && PrefC.GetString(PrefName.MedicationsIndicateNone)!="") {
						Prefs.UpdateString(PrefName.MedicationsIndicateNone,"");
						Signalods.SetInvalid(InvalidType.Prefs);
						log+=Lans.g("FormDatabaseMaintenance","Preference \"MedicationsIndicateNone\" set to blank due to an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"MedicationsIndicateNone\" checked.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PreferenceProblemsIndicateNone(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM diseasedef where DiseaseDefNum="+POut.Long(PrefC.GetLong(PrefName.ProblemsIndicateNone));
					if(PIn.Int(Db.GetCount(command))==0 && PrefC.GetString(PrefName.ProblemsIndicateNone)!="") {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"ProblemsIndicateNone\" is an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"ProblemsIndicateNone\" checked.")+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT COUNT(*) FROM diseasedef where DiseaseDefNum="+POut.Long(PrefC.GetLong(PrefName.ProblemsIndicateNone));
					if(PIn.Int(Db.GetCount(command))==0 && PrefC.GetString(PrefName.ProblemsIndicateNone)!="") {
						Prefs.UpdateString(PrefName.ProblemsIndicateNone,"");
						Signalods.SetInvalid(InvalidType.Prefs);
						log+=Lans.g("FormDatabaseMaintenance","Preference \"ProblemsIndicateNone\" set to blank due to an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"ProblemsIndicateNone\" checked.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string PreferenceTimeCardOvertimeFirstDayOfWeek(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)<0 || PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)>6) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"TimeCardOvertimeFirstDayOfWeek\" is an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"TimeCardOvertimeFirstDayOfWeek\" checked.")+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)<0 || PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)>6) {
						Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,0);//0==Sunday
						Signalods.SetInvalid(InvalidType.Prefs);
						log+=Lans.g("FormDatabaseMaintenance","Preference \"TimeCardOvertimeFirstDayOfWeek\" set to Sunday due to an invalid value.")+"\r\n";
					}
					else if(verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Preference \"TimeCardOvertimeFirstDayOfWeek\" checked.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string PreferencePracticeProv(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(dbmMode==DbmMode.Breakdown) {
				return "";
			}
			string log="";
			string command="SELECT valuestring FROM preference WHERE prefname='PracticeDefaultProv'";
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="") {
				if(verbose) {
					log+=Lans.g("FormDatabaseMaintenance","Default practice provider verified.")+"\r\n";
				}
				return log;
			}
			log+=Lans.g("FormDatabaseMaintenance","No default provider set.")+"\r\n";
			if(dbmMode==DbmMode.Check) {
				return log;
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command="SELECT ProvNum FROM provider WHERE IsHidden=0 ORDER BY itemorder";
			}
			else {//MySQL
				command="SELECT provnum FROM provider WHERE IsHidden=0 ORDER BY itemorder LIMIT 1";
			}
			table=Db.GetTable(command);
			command="UPDATE preference SET valuestring='"+table.Rows[0][0].ToString()+"' WHERE prefname='PracticeDefaultProv'";
			Db.NonQ(command);
			log+="  "+Lans.g("FormDatabaseMaintenance","Fixed.")+"\r\n";
			return log;
		}

		#endregion PerioMeasure, Preference------------------------------------------------------------------------------------------------
		#region ProcButton, ProcedureCode---------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcButtonItemsDeleteWithInvalidAutoCode(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM procbuttonitem WHERE CodeNum=0 AND NOT EXISTS(
						SELECT * FROM autocode WHERE autocode.AutoCodeNum=procbuttonitem.AutoCodeNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ProcButtonItems found with invalid autocode: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"DELETE FROM procbuttonitem WHERE CodeNum=0 AND NOT EXISTS(
						SELECT * FROM autocode WHERE autocode.AutoCodeNum=procbuttonitem.AutoCodeNum)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0) {
						Signalods.SetInvalid(InvalidType.ProcButtons);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","ProcButtonItems deleted due to invalid autocode: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurecodeCategoryNotSet(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			List<Def> listDefsProcCodeCats=Defs.GetDefsForCategory(DefCat.ProcCodeCats,true);
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM procedurecode WHERE procedurecode.ProcCat=0";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						if(listDefsProcCodeCats.Count==0) {
							log+=Lans.g("FormDatabaseMaintenance","Procedure codes with no categories found but cannot be fixed because there are no visible proc code categories.")+"\r\n";
							return log;
						}
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with no category found")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listDefsProcCodeCats.Count==0) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with no categories cannot be fixed because there are no visible proc code categories.")+"\r\n";
						return log;
					}
					command="UPDATE procedurecode SET procedurecode.ProcCat="+POut.Long(listDefsProcCodeCats[0].DefNum)+" WHERE procedurecode.ProcCat=0";
					int numberfixed=(int)Db.NonQ(command);
					if(numberfixed>0) {
						Signalods.SetInvalid(InvalidType.ProcCodes);
					}
					if(numberfixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with no category fixed")+": "+numberfixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Some customers have duplicate procedure codes existing within their database due to past conversions that allowed these 
		/// duplicate codes to exist.  Each proc code does have a different primary key, however in some cases the same procedure code is used and listed
		/// multiple times causing errors/UE issues.  This method will correct those duplicates by identifying them and if found, will add
		/// a hiphen to the duplicate proc code with a numerical count for each duplicate. Example U0001, U0001-1, U0001-2, etc.///</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurecodeFixDuplicateProcedureCodes(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			List<ProcedureCode> listProcedureCodes=ProcedureCodes.GetAllCodes();
			//Group all procedure codes by case sensitive ProcCode while ignoring leading and trailing white-space.
			List<ProcedureCodeGroup> listProcedureCodeGroups=listProcedureCodes.GroupBy(x => x.ProcCode.Trim())
				.Where(x => x.Count() > 1)
				.Select(x => new ProcedureCodeGroup(x.Key,x.ToList()))
				.ToList();
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listProcedureCodeGroups.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures found using the same Proc Code")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					int numFixed=0;
					//Go through each ProcCode group and make the ProcCode string unique for each entry (except the first one).
					for(int i=0;i<listProcedureCodeGroups.Count;i++) {
						for(int j=0;j<listProcedureCodeGroups[i].ListProcedureCodes.Count;j++) {
							if(j==0) {
								continue;//Arbitrarilly leave the first proc code alone.
							}
							long codeNum=listProcedureCodeGroups[i].ListProcedureCodes[j].CodeNum;
							string procCodeOriginal=listProcedureCodeGroups[i].ProcCode;
							string procCodeFix=listProcedureCodeGroups[i].ProcCode+"-"+j;
							string command=@"UPDATE procedurecode SET ProcCode='"+POut.String(procCodeFix)+"' WHERE CodeNum="+POut.Long(codeNum);
								Db.NonQ(command);
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,codeNum,DbmLogFKeyType.ProcedureCode,DbmLogActionType.Update,methodName,
								"Duplicate procedure code found, ProcCode Changed from '"+POut.String(procCodeOriginal)+"' to '"+POut.String(procCodeFix)+"'");
							listDbmLogs.Add(dbmLog);
						}
						numFixed++;
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures fixed that had a duplicate Proc Code")+": "+numFixed.ToString()+"\r\n";
					}
				break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurecodeInvalidPaintType(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string methodName=MethodBase.GetCurrentMethod().Name;
			List<ToothPaintingType> listToothPaintingTypes=Enum.GetValues(typeof(ToothPaintingType)).Cast<ToothPaintingType>().ToList();
			string command=@$"SELECT CodeNum FROM procedurecode WHERE PaintType NOT IN ({String.Join(",",listToothPaintingTypes.Select(x => POut.Enum(x)))})";
			List<long> listCodeNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listCodeNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with invalid Paint Type found")+": "+listCodeNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listCodeNums.Count>0) {
						command="UPDATE procedurecode SET PaintType = "+POut.Enum<ToothPaintingType>(ToothPaintingType.None)
							+" WHERE CodeNum IN ("+String.Join(",",listCodeNums)+")"
							+" AND PaintType NOT IN ("+String.Join(",",listToothPaintingTypes.Select(x => POut.Enum(x)))+")";
						Db.NonQ(command);
						List<DbmLog> listDbmLogs=new List<DbmLog>();
						for(int i=0;i<listCodeNums.Count;i++){
							DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listCodeNums[i],DbmLogFKeyType.ProcedureCode,DbmLogActionType.Update,methodName,
								"Invalid Paint Type detected and updated to "+ToothPaintingType.None.ToString());
							listDbmLogs.Add(dbmLog);
						}
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
					}
					if(listCodeNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with invalid Paint Type fixed")+": "+listCodeNums.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string ProcedurecodeInvalidProvNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT procedurecode.CodeNum FROM procedurecode 
				LEFT JOIN provider ON procedurecode.ProvNumDefault=provider.ProvNum 
				WHERE provider.ProvNum IS NULL 
				AND procedurecode.ProvNumDefault!=0";
			List<long> listCodeNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listCodeNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with invalid Default Provider found")+": "+listCodeNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listCodeNums.Count>0) {
						command="UPDATE procedurecode SET procedurecode.ProvNumDefault=0 WHERE procedurecode.CodeNum IN ("+String.Join(",",listCodeNums)+")";
						Db.NonQ(command);
					}
					if(listCodeNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure codes with invalid Default Provider fixed")+": "+listCodeNums.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion ProcButton, ProcedureCode------------------------------------------------------------------------------------------------------------
		#region ProcedureLog----------------------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogAttachedToApptWithProcStatusDeleted(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ProcNum FROM procedurelog "
						+"WHERE ProcStatus=6 AND (AptNum!=0 OR PlannedAptNum!=0)";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deleted procedures still attached to appointments: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command="UPDATE procedurelog SET AptNum=0,PlannedAptNum=0 "
						+"WHERE ProcStatus=6 "
						+"AND (AptNum!=0 OR PlannedAptNum!=0)";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,
							DbmLogActionType.Update,methodName,"Appointment detached.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Deleted procedures detached from appointments: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogAttachedToWrongAppts(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT procedurelog.ProcNum FROM appointment,procedurelog "
							+"WHERE procedurelog.AptNum=appointment.AptNum AND procedurelog.PatNum != appointment.PatNum";
			switch(dbmMode) {
				case DbmMode.Check:
					if(DataConnection.DBtype==DatabaseType.Oracle) {
						command="SELECT ProcNum FROM procedurelog p "
							+"WHERE (SELECT COUNT(*) FROM appointment a WHERE p.AptNum=a.AptNum AND p.PatNum!=a.PatNum AND ROWNUM<=1)>0";
					}
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures attached to appointments with incorrect patient: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command="UPDATE appointment,procedurelog SET procedurelog.AptNum=0 "
						+"WHERE procedurelog.AptNum=appointment.AptNum AND procedurelog.PatNum != appointment.PatNum";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Appointment detached from ProcedurelogAttachedToWrongAppts.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures detached from appointments: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogAttachedToWrongApptDate(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command=@"SELECT procedurelog.ProcNum FROM procedurelog,appointment
							WHERE procedurelog.AptNum=appointment.AptNum
							AND DATE(procedurelog.ProcDate) != DATE(appointment.AptDateTime)
							AND procedurelog.ProcStatus=2";//only detach completed procs 
			switch(dbmMode) {
				case DbmMode.Check:
					if(DataConnection.DBtype==DatabaseType.Oracle) {
						command=@"SELECT ProcNum FROM procedurelog p
							WHERE p.ProcStatus=2 AND 
							(SELECT COUNT(*) FROM appointment a WHERE a.AptNum=p.AptNum AND TO_DATE(p.ProcDate)!=TO_DATE(a.AptDateTime) AND ROWNUM<=1)>0";
					}
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures which are attached to appointments with mismatched dates: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command=@"UPDATE procedurelog,appointment
						SET procedurelog.AptNum=0
						WHERE procedurelog.AptNum=appointment.AptNum
						AND DATE(procedurelog.ProcDate) != DATE(appointment.AptDateTime)
						AND procedurelog.ProcStatus=2";//only detach completed procs 
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Appointment detached from ProcedurelogAttachedToWrongApptDate.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures detached from appointments due to mismatched dates: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogBaseUnitsZero(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					//zero--------------------------------------------------------------------------------------
					command=@"SELECT COUNT(*) FROM procedurelog 
						WHERE baseunits != (SELECT procedurecode.BaseUnits FROM procedurecode WHERE procedurecode.CodeNum=procedurelog.CodeNum)
						AND baseunits=0";
					//we do not want to change this automatically.  Do not fix these!
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure BaseUnits are zero and are not matching procedurecode BaseUnits: ")+numFound+"\r\n";
					}
					//not zero----------------------------------------------------------------------------------
					command=@"SELECT COUNT(*)
						FROM procedurelog
						WHERE BaseUnits!=0
						AND (SELECT procedurecode.BaseUnits FROM procedurecode WHERE procedurecode.CodeNum=procedurelog.CodeNum)=0";
					//very safe to change them back to zero.
					numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedure BaseUnits not zero, but procedurecode BaseUnits are zero: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command=@"SELECT ProcNum FROM procedurelog
						WHERE BaseUnits!=0 
						AND (SELECT procedurecode.BaseUnits FROM procedurecode WHERE procedurecode.CodeNum=procedurelog.CodeNum)=0";
					List<long> listProcNums=Db.GetListLong(command);
					//first situation: don't fix.
					//second situation:
					//Writing the query this way allows it to work with Oracle.
					command=@"UPDATE procedurelog
						SET BaseUnits=0
						WHERE BaseUnits!=0 
						AND (SELECT procedurecode.BaseUnits FROM procedurecode WHERE procedurecode.CodeNum=procedurelog.CodeNum)=0";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Procedure BaseUnit set to zero from ProcedurelogBaseUnitsZero.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedure BaseUnits set to zero because procedurecode BaseUnits are zero: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasPatNum=true,IsReplicationUnsafe=true)]
		public static string ProcedurelogCodeNumInvalid(bool verbose,DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			string command=@"SELECT ProcNum FROM procedurelog WHERE NOT EXISTS(SELECT * FROM procedurecode WHERE procedurecode.CodeNum=procedurelog.CodeNum)";
			if(patNum>0) {
				command+=" AND patNum="+POut.String(patNum.ToString())+" ";
			}
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures found with invalid CodeNum")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					long codeNumBad=0;
					if(!ProcedureCodes.IsValidCode("~BAD~")) {
						ProcedureCode procedureCode=new ProcedureCode();
						procedureCode.ProcCode="~BAD~";
						procedureCode.Descript="Invalid procedure";
						procedureCode.AbbrDesc="Invalid procedure";
						procedureCode.ProcCat=Defs.GetByExactNameNeverZero(DefCat.ProcCodeCats,"Never Used");
						ProcedureCodes.Insert(procedureCode);
						codeNumBad=procedureCode.CodeNum;
					}
					else {
						codeNumBad=ProcedureCodes.GetCodeNum("~BAD~");
					}
					command="UPDATE procedurelog SET CodeNum=" + POut.Long(codeNumBad) + " WHERE NOT EXISTS (SELECT * FROM procedurecode WHERE procedurecode.CodeNum=procedurelog.CodeNum)";
					if(patNum>0) {
						command+=" AND patNum="+POut.String(patNum.ToString())+" ";
					}
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Procedure fixed with invalid CodeNum from ProcedurelogCodeNumInvalid.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures fixed with invalid CodeNum")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>We don't want this method to show in the DBM, only the Pat specific DBM.</summary>
		[DbmMethodAttr(HasPatNum=true,IsPatDependent=true,HasBreakDown=true)]
		public static string ProcedurelogDeletedWithAttachedIncome(bool verbose, DbmMode dbmMode, long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			Family family=Patients.GetFamily(patNum);
			List<ClaimProcStatus> listClaimProcStatuses=ClaimProcs.GetInsPaidStatuses();
			List<Procedure> listProceduresDeleted=Procedures.GetAllForPatsAndStatuses(family.GetPatNums(),ProcStat.D );//Get all deleted procedures.
			List<long> listProcNumsDeleted=listProceduresDeleted.Select(x => x.ProcNum).ToList();
			//Get all entities that are still attached to the deleted procedures.
			List<Adjustment> listAdjustmentsDeleted=Adjustments.GetForProcs(listProcNumsDeleted);
			List<PaySplit> listPaySplitsDeleted=PaySplits.GetForProcs(listProcNumsDeleted);
			List<ClaimProc> listClaimProcsDeleted=ClaimProcs.GetForProcs(listProcNumsDeleted).FindAll(x=> listClaimProcStatuses.Contains(x.Status));
			List<PayPlanCharge> listPayPlanChargesDeleted=PayPlanCharges.GetForProcs(listProcNumsDeleted);
			List<PayPlanLink> listPayPlanLinksDeleted=PayPlanLinks.GetForFKeysAndLinkType(listProcNumsDeleted,PayPlanLinkType.Procedure);
			//Get unique proc nums.
			List<long> listProcNums=listAdjustmentsDeleted.Select(x => x.ProcNum).Distinct().ToList();
			listProcNums.AddRange(listPaySplitsDeleted.Select(x => x.ProcNum).Distinct().ToList());
			listProcNums.AddRange(listClaimProcsDeleted.Select(x => x.ProcNum).Distinct().ToList());
			listProcNums.AddRange(listPayPlanChargesDeleted.Select(x => x.ProcNum).Distinct().ToList());
			listProcNums.AddRange(listPayPlanLinksDeleted.Select(x => x.FKey).Distinct().ToList());//We can use the FKey here since we only pulled procs.
			List<Procedure> listProcedures=listProceduresDeleted.FindAll(x=> listProcNums.Contains(x.ProcNum));//List of procedures with income.
			int numFound=listProcedures.Count();
			List<ProcedureCode> listProcCodes=ProcedureCodes.GetCodesForCodeNums(listProcedures.Select(x=>x.CodeNum).ToList());
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures of status 'Deleted' with attached income:")+" "+numFound.ToString()+"\r\n";
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(numFound > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance",$"Deleted procedures with attached income that need to be manually investigated:")+"\r\n";
					}
					//Instead of grabbing these inside the loops, to speed things along do a bulk get for these before entering the nested loops.
					List<Claim> listClaimsDeletedProcs=Claims.GetClaimsFromClaimNums(listClaimProcsDeleted.Select(x=>x.ClaimNum).Distinct().ToList());
					List<long> listPayPlanNums=listPayPlanChargesDeleted.Select(x=>x.PayPlanNum).Distinct().ToList();
					listPayPlanNums.AddRange(listPayPlanLinksDeleted.Select(x=>x.PayPlanNum).Distinct().ToList());
					List<PayPlan> listPayPlansDeleted=PayPlans.GetMany(listPayPlanNums.ToArray());
					List<Payment> listPaymentsDeleted=Payments.GetPayments(listPaySplitsDeleted.Select(x=>x.PayNum).ToList());
					List<InsPlan> listInsPlansDeleted=InsPlans.GetByInsSubs(listClaimProcsDeleted.Select(x=>x.InsSubNum).ToList());
					List<InsSub> listInsSubsDeleted=InsSubs.GetMany(listClaimProcsDeleted.Select(x=>x.InsSubNum).ToList());
					StringBuilder stringBuilder=new StringBuilder();
					List<long> listPatNumsDist = listProcedures.Select(x => x.PatNum).Distinct().ToList();
					for(int i=0;i<listPatNumsDist.Count;i++){
						Patient patient=Patients.GetPat(listPatNumsDist[i]);
						List<Procedure> listProceduresByPatNum=listProcedures.FindAll(x => x.PatNum==listPatNumsDist[i]);
						stringBuilder.Append("PatNum #"+listPatNumsDist[i]+" - "+family.GetNameInFamFL(listPatNumsDist[i])+" has "
							+listProceduresByPatNum.Count().ToString()+" deleted procedures:\r\n");
						for(int j = 0;j<listProceduresByPatNum.Count();j++) {
							//These are all the different attached income for the given procedure.
							List<Adjustment> listAdjustments=listAdjustmentsDeleted.FindAll(x=>x.ProcNum==listProceduresByPatNum[j].ProcNum);
							List<PaySplit> listPaySplits=listPaySplitsDeleted.FindAll(x=>x.ProcNum==listProceduresByPatNum[j].ProcNum);
							List<ClaimProc> listClaimProcs=listClaimProcsDeleted.FindAll(x=>x.ProcNum==listProceduresByPatNum[j].ProcNum);
							List<PayPlanCharge> listPayPlanCharges=listPayPlanChargesDeleted.FindAll(x=>x.ProcNum==listProceduresByPatNum[j].ProcNum);
							List<PayPlanLink> listPayPlanLinks=listPayPlanLinksDeleted.FindAll(x=>x.FKey==listProceduresByPatNum[j].ProcNum);
							stringBuilder.Append($" ProcNum #{listProceduresByPatNum[j].ProcNum} {listProceduresByPatNum[j].ProcDate.ToShortDateString()} - {Procedures.GetDescription(listProceduresByPatNum[j])}\r\n");
							for(int k = 0;k<listAdjustments.Count;k++) {//Adjustments
								Def def=Defs.GetDef(DefCat.AdjTypes,listAdjustments[k].AdjType);
								stringBuilder.Append($"		Adjustment #{listAdjustments[k].AdjNum} - Date: {listAdjustments[k].DateEntry.ToShortDateString()}\r\n");
								stringBuilder.Append($"			Adjustment Date: {listAdjustments[k].AdjDate.ToShortDateString()}\r\n");
								stringBuilder.Append($"			Procedure date: {listProceduresByPatNum[j].ProcDate.ToShortDateString()}\r\n");
								stringBuilder.Append($"			Amount: {listAdjustments[k].AdjAmt:C}\r\n");
								if(def!=null) {
									stringBuilder.Append($"			Adj Type: {def.ItemName}\r\n");
								}
							}
							for(int k = 0;k<listPaySplits.Count;k++) {//PaySplits
								Payment payment=listPaymentsDeleted.FirstOrDefault(x=>x.PayNum==listPaySplits[k].PayNum); //Maybe use this? Since paysplits are hidden behind 2 extra forms
								Def def=null;
								stringBuilder.Append($"		PaySplit #{listPaySplits[k].SplitNum} - Date: {listPaySplits[k].DateEntry.ToShortDateString()}\r\n");
								//Payment Info
								if(payment!=null) {
									stringBuilder.Append($"			Payment Date: {payment.PayDate.ToShortDateString()}\r\n");
									stringBuilder.Append($"			Amount: {payment.PayAmt:C}\r\n");
									def=Defs.GetDef(DefCat.PaymentTypes,payment.PayType);
									if(def!=null) {
										stringBuilder.Append($"			Payment Type: {def.ItemName}\r\n");
									}
								}
								//PaySplit Info
								def=Defs.GetDef(DefCat.PaySplitUnearnedType,listPaySplits[k].UnearnedType);
								stringBuilder.Append($"			Amount: {listPaySplits[k].SplitAmt:C}\r\n");
								if(def!=null) {
									stringBuilder.Append($"			Unearned Type: {def.ItemName}\r\n");
								}
							}
							for(int k = 0;k<listClaimProcs.Count;k++) {//ClaimProcs
								Claim claim=listClaimsDeletedProcs.FirstOrDefault(x=>x.ClaimNum==listClaimProcs[k].ClaimNum);
								stringBuilder.Append($"		ClaimProc #{listClaimProcs[k].ClaimProcNum} - Date: {listClaimProcs[k].DateCP.ToShortDateString()}\r\n");
								//Claim Info
								if(claim!=null) {
									stringBuilder.Append($"			Claim Status: {claim.ClaimStatus}\r\n");
									stringBuilder.Append($"			Insurance Plan: {InsPlans.GetDescript(claim.PlanNum,family,listInsPlansDeleted,claim.InsSubNum,listInsSubsDeleted)}\r\n");
									stringBuilder.Append($"			Date Orig Sent: {claim.DateSentOrig.ToShortDateString()}\r\n");
								}
								//ClaimProc Info
								stringBuilder.Append($"			Status: {listClaimProcs[k].Status.GetDescription()}\r\n");
								stringBuilder.Append($"			Pay Entry Date: {listClaimProcs[k].DateEntry.ToShortDateString()}\r\n");
								stringBuilder.Append($"			Payment Date: {listClaimProcs[k].DateCP.ToShortDateString()}\r\n");
								stringBuilder.Append($"			Procedure Date: {listClaimProcs[k].ProcDate.ToShortDateString()}\r\n");
								stringBuilder.Append($"			Description: {Procedures.GetDescription(listProceduresByPatNum[j])}\r\n");
								stringBuilder.Append($"			Billed to Ins: {listClaimProcs[k].FeeBilled:C}\r\n");
								stringBuilder.Append($"			Ins Est Amount: {listClaimProcs[k].InsEstTotal:C}\r\n");
								stringBuilder.Append($"			Ins Pay Amount: {listClaimProcs[k].InsPayAmt:C}\r\n");
							}
							for(int k = 0;k<listPayPlanCharges.Count;k++) {//PayPlanCharges
								PayPlan payPlan=listPayPlansDeleted.FirstOrDefault(x=>x.PayPlanNum==listPayPlanCharges[k].PayPlanNum);
								//PayPlanCharge
								stringBuilder.Append($"		PayPlanCharge #{listPayPlanCharges[k].PayPlanNum} - Date: {listPayPlanCharges[k].ChargeDate}\r\n");
								if(payPlan!=null) { 
									stringBuilder.Append($"			Total Amount: {payPlan.CompletedAmt:C}\r\n");
									stringBuilder.Append($"			Date of First payment: {payPlan.DatePayPlanStart.ToShortDateString()}\r\n");
									stringBuilder.Append($"			APR: {payPlan.APR}\r\n");
									stringBuilder.Append($"			Charge Frequency: {payPlan.ChargeFrequency.GetDescription()}\r\n");
								}
							}
							for(int k = 0;k<listPayPlanLinks.Count;k++) {//PayPlanLinks
								PayPlan payPlan=listPayPlansDeleted.FirstOrDefault(x=>x.PayPlanNum==listPayPlanLinks[k].PayPlanNum);
								stringBuilder.Append($"		PayPlanLink #{listPayPlanLinks[k].PayPlanLinkNum} - Date: {listPayPlanLinks[k].SecDateTEntry.ToShortDateString()}\r\n");
								//PayPlan Info
								if(payPlan!=null) { 
									stringBuilder.Append($"			Total Amount: {payPlan.CompletedAmt:C}\r\n");
									stringBuilder.Append($"			Date of First payment: {payPlan.DatePayPlanStart.ToShortDateString()}\r\n");
									stringBuilder.Append($"			APR: {payPlan.APR}\r\n");
									stringBuilder.Append($"			Charge Frequency: {payPlan.ChargeFrequency.GetDescription()}\r\n");
									stringBuilder.Append($"			Treatment Planned Mode: {payPlan.DynamicPayPlanTPOption.GetDescription()}\r\n");
								}
								//PayPlanLink Info
								stringBuilder.Append($"			Amount Override: {listPayPlanLinks[k].AmountOverride:C}\r\n");
								stringBuilder.Append($"			Type: {listPayPlanLinks[k].LinkType.GetDescription(true)}\r\n");
							}
							stringBuilder.Append("\r\n");
						}
					}
					log+=stringBuilder.ToString();
					break;
			}
			return log;
		}

		///<summary>There was a bug introduced that created an invalid tooth range when the user selected teeth from both mandibular and maxillary teeth.
		///The bug was fixed in v19.2.23 with job #16655 but databases were left in an invalid state.  This method will correct almost all situations.
		///In addition, we identified an issue where old procedures would store toothrange information as abbrieviations such as LA (Lower Arch),
		///UA (Upper Arch), and FM (Full Mouth) these will also be addressed by this fix.  We can correct this issue because we always store tooth ranges 
		///in US nomenclature (predictable numbers).</summary>
		[DbmMethodAttr(HasPatNum = true,IsReplicationUnsafe=true)]
		public static string ProcedurelogFixInvalidToothranges(bool verbose, DbmMode dbmMode, long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode, patNum);
			}
			string patWhere="";
			if(patNum>0){
				patWhere=" AND PatNum="+POut.Long(patNum);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM procedurelog WHERE ToothRange REGEXP '[A-Z]{2}|[0-9]{3}'"+patWhere;//2 letters or 3 numbers
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures found with invalid ToothRange")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="SELECT PatNum,ProcNum,ToothRange FROM procedurelog WHERE ToothRange REGEXP '[A-Z]{2}|[0-9]{3}'"+patWhere;
					DataTable table=Db.GetTable(command);
					int numFixed=0;
					if(table.Rows.Count>0) {
						for(int i=0;i<table.Rows.Count;i++){
							long procNum=PIn.Long(table.Rows[i]["ProcNum"].ToString());
							string toothRange=PIn.String(table.Rows[i]["ToothRange"].ToString());
							List<string> listTeeth=toothRange.Split(',').ToList();
							string toothRangeUpdate="";
							#region Separate mandibular and maxillary
							for(int j=0;j<listTeeth.Count;j++) {
								if(listTeeth[j].Length==4) { 
									toothRangeUpdate+=listTeeth[j].Substring(0,2)+","+listTeeth[j].Substring(2,2)+",";
								}
								else if(listTeeth[j].Length==3) {
									toothRangeUpdate+=listTeeth[j].Substring(0,1)+","+listTeeth[j].Substring(1,2)+",";//because numbers are going lower to higher
								}
								else if(listTeeth[j].Length==2 && Char.IsLetter(listTeeth[j],0) && Char.IsLetter(listTeeth[j],1)) {
									toothRangeUpdate+=listTeeth[j].Substring(0,1)+","+listTeeth[j].Substring(1,1)+",";
								}
								else {
									toothRangeUpdate+=listTeeth[j]+",";
								}
							}
							toothRangeUpdate=toothRangeUpdate.TrimEnd(',');
							#endregion
							#region Known Invalid ToothRange Values
							//The following values were found in some live databases.  These are the correct way to translate the values into valid ToothRanges.
							switch(toothRange.ToUpper()) {
								case "FM": //Full Mouth stored as FM and should be updated to the correct toothrange
									toothRangeUpdate="1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32";
									break;
								case "LA"://Lower Arch
									toothRangeUpdate="17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32";
									break;
								case "UA"://Upper Arch
									toothRangeUpdate="1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16";
									break;
								case "UR"://Upper Right
									toothRangeUpdate="1,2,3,4,5,6,7,8";
									break;
								case "UL"://Upper Left
									toothRangeUpdate="9,10,11,12,13,14,15,16";
									break;
								case "LL"://Lower Left
									toothRangeUpdate="17,18,19,20,21,22,23,24";
									break;
								case "LR"://Lower Left
									toothRangeUpdate="25,26,27,28,29,30,31,32";
									break;
								default:
									//Do nothing.
									break;
							}
							#endregion
							if(toothRangeUpdate!=toothRange) {
								command=$"UPDATE procedurelog SET ToothRange='{POut.String(toothRangeUpdate)}' "
									+"WHERE ProcNum="+POut.Long(procNum)+patWhere;
								Db.NonQ(command);
								numFixed++;
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,procNum,DbmLogFKeyType.Procedure,DbmLogActionType.Update,methodName,
									"Invalid ToothRange of '"+POut.String(toothRange)+"' changed to '"+POut.String(toothRangeUpdate)+"'");
								listDbmLogs.Add(dbmLog);
							}
						}
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures fixed with invalid ToothRange")+": "+numFixed.ToString()+"\r\n";
					}
					break;
				}
			return log;
		}

		[DbmMethodAttr(HasBreakDown=true, IsCanada=true,IsReplicationUnsafe=true)]
		public static string ProcedurelogLabAttachedToDeletedProc(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				return Lans.g("FormDatabaseMaintenance","Skipped. Local computer region must be set to Canada to run.");
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM procedurelog "
						+"WHERE ProcStatus=2 AND ProcNumLab IN(SELECT ProcNum FROM procedurelog WHERE ProcStatus=6)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Completed procedure labs attached to deleted procedures: ")+numFound;
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:  //No db changes made, see if statement below.
				case DbmMode.Fix:
					command="SELECT patient.PatNum,patient.FName,patient.LName,procedurelog.ProcNum FROM procedurelog "
						+"LEFT JOIN patient ON procedurelog.PatNum=patient.PatNum "
						+"WHERE ProcStatus="+POut.Int((int)ProcStat.C)+" "
						+"AND ProcNumLab IN(SELECT ProcNum FROM procedurelog WHERE ProcStatus="+POut.Int((int)ProcStat.D)+") "
						+"GROUP BY patient.PatNum ";
					DataTable table=Db.GetTable(command);
					if(table.Rows.Count>0 || verbose) {
						if(dbmMode==DbmMode.Fix) {
							command="UPDATE procedurelog plab,procedurelog p "
								+"SET plab.ProcNumLab=0 "
								+"WHERE plab.ProcStatus="+POut.Int((int)ProcStat.C)+" AND plab.ProcNumLab=p.ProcNum AND p.ProcStatus="+POut.Int((int)ProcStat.D);
							Db.NonQ(command);
							List<DbmLog> listDbmLogs=new List<DbmLog>();
							string methodName=MethodBase.GetCurrentMethod().Name;
							for(int i=0;i<table.Rows.Count;i++){
								DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,PIn.Long(table.Rows[i]["ProcNum"].ToString()),DbmLogFKeyType.Procedure,
									DbmLogActionType.Update,methodName,"Lab procedure detached from ProcedurelogLabAttachedToDeletedProc.");
								listDbmLogs.Add(dbmLog);
							}
							Crud.DbmLogCrud.InsertMany(listDbmLogs);
							log+=Lans.g("FormDatabaseMaintenance","Patients with completed lab procedures detached from deleted procedures: ")+table.Rows.Count;
						}
						if(dbmMode==DbmMode.Breakdown) {
							log+=Lans.g("FormDatabaseMaintenance","Patients with completed lab procedures that will be detached from deleted procedures: ")
								+table.Rows.Count+", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
							log+=string.Join("\r\n",table.Select().Select(x => "#"+x["PatNum"].ToString()+":"+x["FName"].ToString()+" "+x["LName"].ToString()));
						}
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true)]
		public static string ProcedurelogMultipleClaimProcForInsSub(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT patient.PatNum,patient.LName,patient.FName,procedurelog.ProcDate,procedurecode.ProcCode "
				+"FROM claimproc "
				+"INNER JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"INNER JOIN patient ON patient.PatNum=claimproc.PatNum "
				+"WHERE (claimproc.Status="+POut.Int((int)ClaimProcStatus.NotReceived)+" "
				+"OR claimproc.Status="+POut.Int((int)ClaimProcStatus.Received)+" "
				+"OR claimproc.Status="+POut.Int((int)ClaimProcStatus.Estimate)+") "
				+"AND procedurelog.ProcStatus!="+POut.Int((int)ProcStat.D)+" " //exclude deleted procedures
				+"AND claimproc.IsOverpay=0 "
				+"GROUP BY claimproc.ProcNum, claimproc.InsSubNum, claimproc.PlanNum "
					+", patient.PatNum, patient.LName, patient.FName, procedurelog.ProcDate, procedurecode.ProcCode "//For Oracle.
				+"HAVING COUNT(*)>1 "
				+"ORDER BY patient.LName, patient.FName, procedurelog.ProcDate, procedurecode.ProcCode";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Procedures with multiple claimprocs for the same insurance plan")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count!=0) {
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+="   "+table.Rows[i]["PatNum"].ToString()+"-"+table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString()
								+"  Procedure Date: "+PIn.Date(table.Rows[i]["ProcDate"].ToString()).ToShortDateString()+"  "+table.Rows[i]["ProcCode"];
							log+="\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true)]
		public static string ProcedurelogProvNumMissing(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Get procedures not assigned to a provider and not deleted
			string command=@"SELECT patient.PatNum,patient.LName,patient.FName,procedurelog.ProcDate,procedurecode.ProcCode,procedurelog.ToothNum "
				+"FROM procedurelog "
				+"INNER JOIN patient ON patient.PatNum=procedurelog.PatNum "
				+"INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"WHERE ProvNum=0 AND ProcStatus!="+POut.Int((int)ProcStat.D)+" "
				+"ORDER BY patient.LName,patient.FName,procedurelog.ProcDate,procedurecode.ProcCode";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Procedures with missing provnums found: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count>0 || verbose) {
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count==0) {
						break;
					}
					log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
					for(int i = 0;i<table.Rows.Count;i++) {
						log+="\r\n   ";
						log+=table.Rows[i]["PatNum"].ToString()+"-"+table.Rows[i]["LName"].ToString()+", "+table.Rows[i]["FName"].ToString()+", "
							+PIn.Date(table.Rows[i]["ProcDate"].ToString()).ToShortDateString()+", "+table.Rows[i]["ProcCode"].ToString();
						if(table.Rows[i]["ToothNum"].ToString()!="") {	//displays ToothNum if present
							log+=", ToothNum: "+table.Rows[i]["ToothNum"].ToString();
						}
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogToothNums(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			if(dbmMode==DbmMode.Breakdown) {
				return "";
			}
			string log="";
			//The logic for checking whether a tooth is invalid was obtained from Tooth.IsValidDB().
			string command="SELECT ProcNum,ToothNum,PatNum FROM procedurelog "
				+"WHERE ToothNum!='' "
				+"AND ToothNum NOT REGEXP '^[A-T]S?$' "//supernumerary
				+"AND (ToothNum NOT REGEXP '^[1-9][0-9]?$' "//matches 1 or 2 digits, leading 0 not allowed
				+"OR (CAST(ToothNum AS UNSIGNED)>32 AND CAST(ToothNum AS UNSIGNED)<51) "
				+"OR CAST(ToothNum AS UNSIGNED)>82) ";
			DataTable table=Db.GetTable(command);
			Patient patient=null;
			string toothNum;
			int numberFixed=0;
			List<DbmLog> listDbmLogs=new List<DbmLog>();
			string methodName=MethodBase.GetCurrentMethod().Name;
			for(int i = 0;i<table.Rows.Count;i++) {
				toothNum=table.Rows[i][1].ToString();
				if(verbose) {
					patient=Patients.GetLim(Convert.ToInt32(table.Rows[i][2].ToString()));
				}
				if(string.CompareOrdinal(toothNum,"a")>=0 && string.CompareOrdinal(toothNum,"t")<=0) {
					if(dbmMode!=DbmMode.Check) {
						command="UPDATE procedurelog SET ToothNum='"+toothNum.ToUpper()+"' WHERE ProcNum="+table.Rows[i][0].ToString();
						Db.NonQ(command);
					}
					if(verbose) {
						log+=patient.GetNameLF()+" "+toothNum+" - "+toothNum.ToUpper()+"\r\n";
					}
					numberFixed++;
					continue;
				}
				if(dbmMode!=DbmMode.Check) {
					command="UPDATE procedurelog SET ToothNum='1' WHERE ProcNum="+table.Rows[i][0].ToString();
					Db.NonQ(command);
				}
				if(verbose) {
					log+=patient.GetNameLF()+" "+toothNum+" - 1\r\n";
				}
				numberFixed++;
			}
			for(int i=0;i<table.Rows.Count;i++){
				DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,PIn.Long(table.Rows[i]["ProcNum"].ToString()),DbmLogFKeyType.Procedure,
					DbmLogActionType.Update,methodName,"Fixed invalid tooth number from ProcedurelogToothNums.");
				listDbmLogs.Add(dbmLog);
			}
			if(numberFixed!=0 || verbose) {
				Crud.DbmLogCrud.InsertMany(listDbmLogs);
				log+=Lans.g("FormDatabaseMaintenance","Check for invalid tooth numbers complete.  Records checked: ")
					+Db.GetCount("SELECT COUNT(*) FROM procedurelog")+". "+Lans.g("FormDatabaseMaintenance","Records invalid: ")+numberFixed.ToString()+"\r\n";
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true)]
		public static string ProcedurelogTpAttachedToClaim(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=$@"SELECT patient.PatNum,patient.LName,patient.FName,claim.ClaimNum,claim.DateService,
					procedurelog.ProcNum,procedurecode.ProcCode,procedurelog.ProcStatus,procedurelog.ProcDate,
					procedurelog.ProcNumLab,procedurecodelab.ProcCode ProcCodeLab,procedureloglab.ProcStatus ProcStatusLab,procedureloglab.ProcDate ProcDateLab
				FROM procedurelog
				INNER JOIN claimproc ON procedurelog.ProcNum=claimproc.ProcNum
				INNER JOIN claim ON claimproc.ClaimNum=claim.ClaimNum
				INNER JOIN patient ON claim.PatNum=patient.PatNum
				INNER JOIN procedurecode ON procedurelog.CodeNum=procedurecode.CodeNum
				LEFT JOIN procedurelog procedureloglab ON procedurelog.ProcNumLab=procedureloglab.ProcNum
				LEFT JOIN procedurecode procedurecodelab ON procedureloglab.CodeNum=procedurecodelab.CodeNum
				WHERE procedurelog.ProcStatus!={POut.Long((int)ProcStat.C)}
				AND claim.ClaimStatus IN ('W','S','R')
				AND claim.ClaimType IN ('P','S','Other')";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			StringBuilder stringBuilder=new StringBuilder(Lans.g("FormDatabaseMaintenance","Procedures attached to claims that are not complete:")+" "+table.Rows.Count);
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count!=0) {
						stringBuilder.AppendLine("\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down."));
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						stringBuilder.AppendLine(", "+Lans.g("FormDatabaseMaintenance","including")+":");
						List<PatNumProceduresGroup> listPatNumClaimsGroups=table.Select()
							.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
							.Select(x => new PatNumProceduresGroup(x.Key,x.ToList()))
							.ToList();
						for(int i=0;i<listPatNumClaimsGroups.Count;i++) {
							/**********************************************************************************************************************
							 * Display the claims for each patient in groupings that will yield something like this for Americans:
							 * 
							 * Jason Salmon (PatNum:9)
							 *   Claim (ClaimNum:10) with service date of 06/30/2020
							 *     Procedure T6357 on 06/30/2020 (ProcNum:51) has a status of 'TPi'
							 *     Procedure T6357 on 06/30/2020 (ProcNum:52) has a status of 'TPi'
							 *   Claim (ClaimNum:14) with service date of 04/24/2020
							 *     Procedure T9999 on 04/24/2020 (ProcNum:92) has a status of 'TPi'
							 * Eric Salmon (PatNum:25)
							 *   Claim (ClaimNum:16) with service date of 09/21/2022
							 *     Procedure T1356 on 09/21/2022 (ProcNum:122) has a status of 'TPi'
							 *     Procedure T3541 on 09/21/2022 (ProcNum:123) has a status of 'TPi'
							 *     Procedure T1254 on 09/21/2022 (ProcNum:124) has a status of 'TPi'
							 * 
							 * And something like this for Canadians (if Lab Fees are involved):
							 * 
							 * Isla Salmon (PatNum:1277)
							 *   Claim (ClaimNum:15210) with service date of 2020-03-04
							 *     Procedure 67201 on 2020-03-04 (ProcNum:97282) has a status of 'C'
							 *       ^^Lab Fee 99111 on 2019-10-04 (ProcNum:97294) has a status of 'TPi'
							 *     Procedure 62502 on 2020-03-04 (ProcNum:97283) has a status of 'C'
							 *       ^^Lab Fee 99111 on 2019-10-04 (ProcNum:97295) has a status of 'TPi'
							 *       ...
							 **********************************************************************************************************************/
							stringBuilder.AppendLine(listPatNumClaimsGroups[i].ToString());
						}
						stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","They need to be fixed manually."));
					}
					break;
			}
			return stringBuilder.ToString();
		}

		[DbmMethodAttr(HasBreakDown = true, IsCanada = true)]
		public static string ProcedurelogNotComplAttachedToComplLabCanada(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				return Lans.g("FormDatabaseMaintenance","Skipped. Local computer region must be set to Canada to run.");
			}
			string command="SELECT pc.ProcCode ProcCode,pclab.ProcCode ProcCodeLab,proc.PatNum,proc.ProcDate "
				+"FROM procedurelog proc "
				+"INNER JOIN procedurecode pc ON pc.CodeNum=proc.CodeNum "
				+"INNER JOIN procedurelog lab ON proc.ProcNum=lab.ProcNumLab AND lab.ProcStatus="+POut.Long((int)ProcStat.C)+" "
				+"INNER JOIN procedurecode pclab ON pclab.CodeNum=lab.CodeNum "
				+"WHERE proc.ProcStatus!="+POut.Long((int)ProcStat.C);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Completed lab fees with treatment planned procedures attached")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count!=0) {
						log+="\r\n   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+=Lans.g("FormDatabaseMaintenance","Completed lab fee")+" "+table.Rows[i]["ProcCodeLab"].ToString()+" "
									+Lans.g("FormDatabaseMaintenance","is attached to non-complete procedure")+" "+table.Rows[i]["ProcCode"].ToString()+" "
									+Lans.g("FormDatabaseMaintenance","on date")+" "+PIn.Date(table.Rows[i]["ProcDate"].ToString()).ToShortDateString()+". "
									+Lans.g("FormDatabaseMaintenance","PatNum: ")+table.Rows[i]["PatNum"].ToString()+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   Fix manually from within the Chart module.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true, HasPatNum = true)]
		public static string ProcedurelogNotCompletedAttachedToReceivedClaimprocOrPaySplitOrAdj(bool verbose,DbmMode dbmMode,long patNumSpecific=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNumSpecific);
			}
			string command=$@"SELECT patient.PatNum,patient.LName,patient.FName,
					COALESCE(claimproc.ClaimProcNum,0) ClaimProcNum,COALESCE(paysplit.SplitNum,0) SplitNum,
					COALESCE(claimproc.ClaimNum,0) ClaimNum,COALESCE(claimproc.ProcDate,'0001-01-01') dateService,
					COALESCE(adjustment.AdjNum,0) AdjNum, COALESCE(adjustment.AdjDate,'0001-01-01') AdjDate,COALESCE(adjustment.AdjAmt,0) AdjAmt,
					COALESCE(paysplit.PayNum,0) PayNum, COALESCE(paysplit.DatePay,'0001-01-01') DatePay,COALESCE(paysplit.SplitAmt,0) SplitAmt,
					procedurelog.ProcNum,procedurecode.ProcCode,procedurelog.ProcStatus,procedurelog.ProcDate,
					procedurelog.ProcNumLab,procedurecodelab.ProcCode ProcCodeLab,procedureloglab.ProcStatus ProcStatusLab,
					procedureloglab.ProcDate ProcDateLab
				FROM procedurelog 
				INNER JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum
				INNER JOIN patient ON patient.PatNum=procedurelog.PatNum
				LEFT JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum AND claimproc.Status IN ({POut.Enum(ClaimProcStatus.NotReceived)},
					{POut.Enum(ClaimProcStatus.Received)},
					{POut.Enum(ClaimProcStatus.Supplemental)},
					{POut.Enum(ClaimProcStatus.CapComplete)})
				LEFT JOIN adjustment ON adjustment.ProcNum=procedurelog.ProcNum
				LEFT JOIN paysplit ON paysplit.ProcNum=procedurelog.ProcNum and paysplit.UnearnedType=0
				LEFT JOIN procedurelog procedureloglab ON procedurelog.ProcNumLab=procedureloglab.ProcNum
				LEFT JOIN procedurecode procedurecodelab ON procedureloglab.CodeNum=procedurecodelab.CodeNum
				WHERE procedurelog.ProcStatus NOT IN({POut.Enum(ProcStat.C)}) 
				AND (claimproc.ProcNum IS NOT NULL 
					OR adjustment.ProcNum IS NOT NULL 
					OR paysplit.ProcNum IS NOT NULL 
					OR procedureloglab.ProcNum IS NOT NULL) "
				+PatientAndClauseHelper(patNumSpecific,"patient");//Allow this DBM to run just for a specific patient.
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			int countClaims=table.Select().Where(x => PIn.Long(x["ClaimNum"].ToString()) > 0).DistinctBy(x => PIn.Long(x["ClaimProcNum"].ToString())).Count();
			int countAdjustments=table.Select().Where(x => PIn.Long(x["AdjNum"].ToString()) > 0).DistinctBy(x => PIn.Long(x["AdjNum"].ToString())).Count();
			int countPayments=table.Select().Where(x => PIn.Long(x["PayNum"].ToString()) > 0).DistinctBy(x => PIn.Long(x["SplitNum"].ToString())).Count();
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Procedures attached to claims that are not complete")+$": {countClaims}");
			stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Adjustments attached to procedures that are not complete")+$": {countAdjustments}");
			stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Payments attached to procedures that are not complete")+$": {countPayments}");
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					if(table.Rows.Count!=0) {
						stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down."));
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
					/**********************************************************************************************************************
					* Display the claims for each patient in groupings that will yield something like this for Americans:
					* 
					* Test Patient (PatNum:9)
					*   Claim (ClaimNum:10) with service date of 06/30/2020
					*     Procedure T6357 on 06/30/2020 (ProcNum:51) has a status of 'TP'
					*     Procedure T6357 on 06/30/2020 (ProcNum:52) has a status of 'TP'
					*   Adjustment (AdjustmentNum:14) with adjustment date of 04/24/2020
					*     Procedure T9999 on 04/24/2020 (ProcNum:92) has a status of 'TP'
					* Test Patient2 (PatNum:25)
					*   Patient Payment (PayNum:16) with payment date of 09/21/2022
					*     Procedure T1356 on 09/21/2022 (ProcNum:122) has a status of 'TP'
					*     Procedure T3541 on 09/21/2022 (ProcNum:123) has a status of 'TP'
					*     Procedure T1254 on 09/21/2022 (ProcNum:124) has a status of 'TP'
					* 
					* And something like this for Canadians (if Lab Fees are involved):
					* 
					* Test Patient3 (PatNum:1277)
					*   Claim (ClaimNum:15210) with service date of 2020-03-04
					*     Procedure 67201 on 2020-03-04 (ProcNum:97282) has a status of 'C'
					*       ^^Lab Fee 99111 on 2019-10-04 (ProcNum:97294) has a status of 'TPi'
					*     Procedure 62502 on 2020-03-04 (ProcNum:97283) has a status of 'C'
					*       ^^Lab Fee 99111 on 2019-10-04 (ProcNum:97295) has a status of 'TPi'
					*       ...
					**********************************************************************************************************************/
						List<PatNumProceduresGroup> listPatNumClaimsGroups=table.Select()
							.GroupBy(x => PIn.Long(x["PatNum"].ToString()))
							.Select(x => new PatNumProceduresGroup(x.Key,x.ToList()))
							.ToList();
						for(int i=0;i<listPatNumClaimsGroups.Count;i++) {
							stringBuilder.AppendLine(listPatNumClaimsGroups[i].ToString());
						}
						stringBuilder.AppendLine(Lans.g("FormDatabaseMaintenance","They need to be fixed manually."));
					}
					break;
			}
			return stringBuilder.ToString();
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogUnitQtyZero(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ProcNum FROM procedurelog WHERE UnitQty=0";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures with UnitQty=0 found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command=@"UPDATE procedurelog        
						SET UnitQty=1
						WHERE UnitQty=0";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,
							DbmLogActionType.Update,methodName,"Procedure changed from UnitQty=0 to UnitQty=1.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures changed from UnitQty=0 to UnitQty=1: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogWithInvalidProvNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT ProcNum FROM procedurelog WHERE ProvNum > 0 AND ProvNum NOT IN (SELECT ProvNum FROM provider)";
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures with invalid ProvNum found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command="UPDATE procedurelog SET ProvNum="+POut.Long(PrefC.GetLong(PrefName.PracticeDefaultProv))+
							" WHERE ProvNum > 0 AND ProvNum NOT IN (SELECT ProvNum FROM provider)";
					int numFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Updated invalid provider from ProcedurelogWithInvalidProvNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures with invalid ProvNum fixed")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogWithInvalidAptNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command=@"SELECT ProcNum FROM procedurelog
				WHERE (
					AptNum !=0
					AND AptNum NOT IN (SELECT AptNum FROM appointment) "//Procedures attached to appointments that don't exist
				+@")
				OR (
					PlannedAptNum !=0
					AND (
						PlannedAptNum NOT IN(SELECT AptNum FROM appointment) "//Procedures attached to planned appointments that don't exist
						+@"OR PlannedAptNum IN(SELECT AptNum FROM appointment WHERE AptStatus!=6) "//procedures attached to planned appointments that are not considered planned appointments
					+@")
				)";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures attached to invalid appointments")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command="UPDATE procedurelog SET AptNum=0 "
						+"WHERE AptNum NOT IN(SELECT AptNum FROM appointment) AND AptNum!=0";
					int numberFixed=(int)Db.NonQ(command);
					command="UPDATE procedurelog SET PlannedAptNum=0 "
						+"WHERE PlannedAptNum NOT IN(SELECT AptNum FROM appointment) AND PlannedAptNum!=0";
					numberFixed+=(int)Db.NonQ(command);
					command="SELECT ProcNum,PlannedAptNum,AptNum FROM procedurelog WHERE PlannedAptNum!=0 AND (PlannedAptNum IN(SELECT AptNum FROM appointment WHERE AptStatus!=6)) ORDER BY ProcNum";
					DataTable table=Db.GetTable(command);
					for(int i = 0;i<table.Rows.Count;i++) {
						command="UPDATE procedurelog SET ";
						if(PIn.Long(table.Rows[i]["AptNum"].ToString())==0){
							command+="AptNum="+table.Rows[i]["PlannedAptNum"].ToString()+",";
						}
						command+="PlannedAptNum=0 WHERE ProcNum="+table.Rows[i]["ProcNum"].ToString();
						numberFixed+=(int)Db.NonQ(command);
					}
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,
							DbmLogActionType.Update,methodName,"Set AptNum from ProcedurelogWithInvalidAptNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures with invalid appointments fixed")+": "+numberFixed.ToString()+"\r\n";//Do we care enough that this number could be inflated if a procedure had both an invalid AptNum AND PlannedNum?
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProcedurelogWithInvalidClinicNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT ProcNum "
						+"FROM procedurelog "
						+"WHERE ClinicNum NOT IN(SELECT ClinicNum FROM clinic) AND ClinicNum!=0 ";
			switch(dbmMode) {
				case DbmMode.Check:					
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Procedures attached to invalid clinics")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<long> listProcNums=Db.GetListLong(command);
					command="UPDATE procedurelog SET ClinicNum=0 "
						+"WHERE ClinicNum NOT IN(SELECT ClinicNum FROM clinic) and ClinicNum!=0 ";
					int numberFixed=(int)Db.NonQ(command);
					for(int i=0;i<listProcNums.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listProcNums[i],DbmLogFKeyType.Procedure,DbmLogActionType.Update,
							methodName,"Fixed invalid clinicnum from ProcedurelogWithInvalidClinicNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(numberFixed>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Procedures with invalid clinics fixed")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}
		#endregion ProcedureLog-------------------------------------------------------------------------------------------------------------------------
		#region ProgramProperty, Provider, QuickPasteNote-----------------------------------------------------------------------------------------------
		[DbmMethodAttr]
		public static string ProgramPropertiesDuplicateLocalPath(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//Min may not be the oldest when using random primary keys, but we have to pick one.  In most all cases theyre identical anyway.
			string command="SELECT MIN(ProgramPropertyNum) ProgramPropertyNum,COUNT(*) CountDup "
					+"FROM programproperty "
					+"WHERE PropertyDesc='' "//Blank for workstation overrides of program path.
					+"GROUP BY ProgramNum,ComputerName,ClinicNum";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=table.Select().Select(x => PIn.Int(x["CountDup"].ToString())-1).Sum();
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Duplicate local path program properties entries found: ")
							+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					string validProgPropNums=string.Join(",",table.Select().Select(x => PIn.Long(x["ProgramPropertyNum"].ToString())));
					int numberFixed=0;
					if(!validProgPropNums.IsNullOrEmpty()) {
						command="DELETE FROM programproperty WHERE PropertyDesc='' "
							+"AND ProgramPropertyNum NOT IN "
							+"("+validProgPropNums+")";
						numberFixed=(int)Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Duplicate local path program properties entries found: ")
							+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string ProgramPropertiesDuplicatesForHQ(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string strProgNum=POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+","+POut.Long(Programs.GetProgramNum(ProgramName.PayConnect));
			//Min may not be the oldest when using random primary keys, but we have to pick one.  In most all cases theyre identical anyway.
			string command="SELECT MIN(ProgramPropertyNum) ProgramPropertyNum,COUNT(*) Count "
					+"FROM programproperty "
					+"WHERE ClinicNum=0 "
					+"AND ProgramNum IN ("+strProgNum+") "
					+"GROUP BY ProgramNum,PropertyDesc";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=table.Select().Select(x => PIn.Int(x["Count"].ToString())-1).Sum();
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","X-Charge and/or PayConnect duplicate program property entries found: ")
							+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM programproperty WHERE ClinicNum=0 AND ProgramNum IN ("+strProgNum+") "
						+"AND ProgramPropertyNum NOT IN ("+string.Join(",",table.Select().Select(x => PIn.Long(x["ProgramPropertyNum"].ToString())))+")";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","X-Charge and/or PayConnect duplicate program property entries deleted: ")
							+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProgramPropertiesMissingForClinic(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			//X-Charge and PayConnect are currently the only program links that use ClinicNum.
			string strProgNum=POut.Long(Programs.GetProgramNum(ProgramName.Xcharge))+","+POut.Long(Programs.GetProgramNum(ProgramName.PayConnect));
			string command="SELECT DISTINCT pphq.*,clinic.ClinicNum missingClinicNum "//Distinct in case there are duplicate prog props with a ClinicNum 0.
				+"FROM programproperty pphq "
				+"INNER JOIN clinic ON TRUE "
				+"LEFT JOIN programproperty ppcl ON ppcl.ProgramNum=pphq.ProgramNum "
				+"AND ppcl.PropertyDesc=pphq.PropertyDesc "
					+"AND ppcl.ClinicNum=clinic.ClinicNum "
				+"WHERE pphq.ProgramNum IN ("+strProgNum+") "
				+"AND pphq.ClinicNum=0 "
				+"AND pphq.PropertyDesc!='' "
				+"AND ppcl.ClinicNum IS NULL ";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=table.Rows.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","X-Charge and/or PayConnect missing program property entries found: ")
							+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<ProgramProperty> listProgramProperties=Crud.ProgramPropertyCrud.TableToList(table);
					for(int i = 0;i<listProgramProperties.Count;i++) {
						listProgramProperties[i].ClinicNum=PIn.Long(table.Rows[i]["missingClinicNum"].ToString());
						ProgramProperties.Insert(listProgramProperties[i]);
					}
					long numberFixed=table.Rows.Count;
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","X-Charge and/or PayConnect missing program property entries inserted: ")
							+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string ProviderHiddenOnAppointmentView(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(DISTINCT provider.ProvNum) FROM apptview
						INNER JOIN apptviewitem ON apptview.ApptViewNum=apptviewitem.ApptViewNum
						INNER JOIN provider ON provider.ProvNum=apptviewitem.ProvNum
						WHERE provider.IsHidden=1";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g(_lanThis,"Hidden Providers found on Appointment Views")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					//Collect a DataTable of the ApptViews with hidden Providers on them
					command=@"SELECT apptview.ApptViewNum, apptviewitem.ApptViewItemNum, apptviewitem.ProvNum FROM apptview
						INNER JOIN apptviewitem ON apptview.ApptViewNum=apptviewitem.ApptViewNum
						INNER JOIN provider ON provider.ProvNum=apptviewitem.ProvNum
						WHERE provider.IsHidden=1";
					DataTable table=Db.GetTable(command);
					//Use the DataTable to get the ApptViewItemNums afflicted
					List<long> listApptViewItemNums=table.Select()
						.Select(x => PIn.Long(x["ApptViewItemNum"].ToString())).Distinct().ToList();
					//If there are afflicted tables, then do the below, but most of the time it should only run the above logic.
					if(listApptViewItemNums.Count==0) {
						break;
					}
					//Delete the affected apptviewitems
					command=$"DELETE FROM apptviewitem WHERE apptviewitem.ApptViewItemNum IN (" +
						$"{string.Join(", ",listApptViewItemNums.Select(x => POut.Long(x)))})";
					Db.NonQ(command);
					//Create a new ApptViewItem per Provider to hold affected views for logging
					List<ApptViewItem> listApptViewItems=new List<ApptViewItem>();
					for(int i = 0;i<table.Rows.Count;i++) {
						ApptViewItem apptViewItem=new ApptViewItem();
						apptViewItem.ApptViewNum=PIn.Long(table.Rows[i]["ApptViewNum"].ToString());
						apptViewItem.ApptViewItemNum=PIn.Long(table.Rows[i]["ApptViewItemNum"].ToString());
						apptViewItem.ProvNum=PIn.Long(table.Rows[i]["ProvNum"].ToString());
						listApptViewItems.Add(apptViewItem);
					}
					for(int i = 0;i<listApptViewItems.Count;i++) {
						log+=Lans.g(_lanThis,"Removed ")+"'"+Providers.GetFormalName(listApptViewItems[i].ProvNum)+"'"+Lans.g(_lanThis," from Appointment Views")+": "
							+string.Join(", ",listApptViewItems.FindAll(x => x.ProvNum==listApptViewItems[i].ProvNum)
								.Select(x => ApptViews.GetApptView(x.ApptViewNum).Description).Distinct())+"\r\n";
					}
					//Check if there are any other ApptViewItems on the ApptView we removed the provider from
					//It should be noted that we know there are valid ApptViews we've removed at this point 
					//and have ensured listApptViewItemNumsToDelete is not empty
					List<long> listApptViewNums=listApptViewItems.Select(x => x.ApptViewNum).Distinct().ToList();
					command=@"SELECT apptview.ApptViewNum FROM apptview 
						INNER JOIN apptviewitem ON apptview.ApptViewNum=apptviewitem.ApptViewNum
						WHERE apptview.ApptViewNum IN("+string.Join(",",listApptViewNums.Select(x => POut.Long(x)))+@")
						GROUP BY apptview.ApptViewNum";
					List<long> listApptViewNumsExists=Db.GetListLong(command);
					//Get a list of any orphaned ApptViews that have become orphanged due to prior deletion of hidden provider associated rows
					List<long> listApptViewNumsOrphaned=listApptViewNums.Except(listApptViewNumsExists).ToList();
					//Extremely rare case because we currently require an assigned Operatory and Provider to create an ApptView, meaning something
					//will be associated with the ApptView and thus it is not truly orphaned
					if(listApptViewNumsOrphaned.Count>0) {//If any orphaned rows exist, then we delete the ApptView as well
						command+="DELETE FROM apptview WHERE ApptViewNum IN("+string.Join(", ",listApptViewNumsOrphaned.Select(x => POut.Long(x)))+")";
						Db.NonQ(command);
						//Show a list of deleted views, if a list is necessary
						log+=Lans.g(_lanThis,"Appointment Views deleted for having no remaining items")+": "
							+string.Join(", ",listApptViewNumsOrphaned.Select(x =>	ApptViews.GetApptView(x).Description))+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasBreakDown = true)]
		public static string ProviderHiddenWithClaimPayments(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT MAX(claimproc.ProcDate) ProcDate,provider.ProvNum
				FROM claimproc,provider
				WHERE claimproc.ProvNum=provider.ProvNum
				AND provider.IsHidden=1
				AND claimproc.InsPayAmt>0
				GROUP BY provider.ProvNum";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Hidden providers with claim payments")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count > 0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count > 0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							Provider provider=Providers.GetProv(PIn.Long(table.Rows[i]["ProvNum"].ToString()));
							log+=provider.Abbr+" "+Lans.g("FormDatabaseMaintenance","has claim payments entered as recently as")+" "
								+PIn.Date(table.Rows[i]["ProcDate"].ToString()).ToShortDateString()+"\r\n";
						}
						log+="   "+Lans.g("FormDatabaseMaintenance","This data will be missing on income reports.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string ProviderMissingFromDB(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="";
			List<long> listProvNumsFound=new List<long>();
			command="SELECT DISTINCT PriProv FROM patient";
			listProvNumsFound.AddRange(Db.GetListLong(command).FindAll(x=>x!=0 && !listProvNumsFound.Contains(x)));
			command="SELECT DISTINCT SecProv FROM patient";
			listProvNumsFound.AddRange(Db.GetListLong(command).FindAll(x=>x!=0 && !listProvNumsFound.Contains(x)));
			command="SELECT DISTINCT ProvNum FROM procedurelog";
			listProvNumsFound.AddRange(Db.GetListLong(command).FindAll(x=>x!=0 && !listProvNumsFound.Contains(x)));
			command="SELECT ProvNum FROM provider";
			List<long> listProvNums=Db.GetListLong(command);
			List<long> listProvNumsMissing=listProvNumsFound.Except(listProvNums).ToList();
			switch(dbmMode) { 
				case DbmMode.Check:
					if(listProvNumsMissing.Count>0 || verbose) { 
						log=Lans.g("FormDatabaseMaintenance","Providers missing from the provider table:")+$" {listProvNumsMissing.Count}\r\n";
					}
					break;
				case DbmMode.Fix:
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					if(listProvNumsMissing.Count==0 && !verbose) {
						break;
					}
					command="SELECT MAX(ItemOrder) FROM provider";
					int itemOrder=Db.GetInt(command); 
					for(int i=0;i<listProvNumsMissing.Count;i++) {
						long provNumMissing=listProvNumsMissing[i];
						Provider provider=new Provider();
						provider.ProvNum=provNumMissing;
						provider.Abbr="MissingProvAbbr_"+provNumMissing;
						provider.ItemOrder=++itemOrder;
						provider.LName="MissingProvLName_"+provNumMissing;
						provider.FName="MissingProvFName_"+provNumMissing;
						Crud.ProviderCrud.Insert(provider,useExistingPK:true);
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,provNumMissing,DbmLogFKeyType.Provider,
							DbmLogActionType.Insert,methodName,"Inserted missing provider due to orphaned entries.");
						listDbmLogs.Add(dbmLog);
					}
					log=Lans.g("FormDatabaseMaintenance","Missing providers added into provider table:")+" "+listProvNumsMissing.Count+"\r\n";
					DbmLogs.InsertMany(listDbmLogs);
					break;
			}
			return log;
		}


		[DbmMethodAttr]
		public static string ProviderWithInvalidFeeSched(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM provider WHERE FeeSched NOT IN (SELECT FeeSchedNum FROM feesched)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Providers found with invalid FeeSched: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"UPDATE provider SET FeeSched="+POut.Long(FeeScheds.GetFirst(true).FeeSchedNum)+" "
						+"WHERE FeeSched NOT IN (SELECT FeeSchedNum FROM feesched)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Providers whose FeeSched has been changed: ")
						+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string QuickPasteNoteWithInvalidCatNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT COUNT(*) FROM quickpastenote WHERE QuickPasteCatNum=0";
			int numFound=PIn.Int(Db.GetCount(command));
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Quick Paste Notes with an invalid category num")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					if(numFound>0) {
						QuickPasteCat quickPasteCat=new QuickPasteCat();
						quickPasteCat.Description="DBM GENERATED";
						QuickPasteCats.Insert(quickPasteCat);
						command=@"UPDATE quickpastenote SET QuickPasteCatNum="+POut.Long(quickPasteCat.QuickPasteCatNum)+" WHERE QuickPasteCatNum=0";
						numberFixed=(int)Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Quick Paste Notes with an invalid category num fixed")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion ProgramProperty, Provider, QuickPasteNote--------------------------------------------------------------------------------------------
		#region Recall, RecallTrigger, RefAttach, RxAlert---------------------------------------------------------------------------------------

		[DbmMethodAttr(HasBreakDown = true)]
		public static string RecallDuplicatesWarn(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			if(RecallTypes.PerioType<1 || RecallTypes.ProphyType<1) {
				return Lans.g("FormDatabaseMaintenance","Warning!  Recall types not set up properly.  There must be at least one of each type: perio and prophy.")+"\r\n";
			}
			string command="SELECT FName,LName,COUNT(*) countDups FROM patient LEFT JOIN recall ON recall.PatNum=patient.PatNum "
				+"AND (recall.RecallTypeNum="+POut.Long(RecallTypes.PerioType)+" "
				+"OR recall.RecallTypeNum="+POut.Long(RecallTypes.ProphyType)+") "
				+"GROUP BY FName,LName,patient.PatNum HAVING countDups>1";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Number of patients with duplicate recalls: ")+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count==0) {
						break;
					}
					log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
					for(int i = 0;i<table.Rows.Count;i++) {
						if(i%3==0) {
							log+="\r\n   ";
						}
						log+=table.Rows[i]["FName"].ToString()+" "+table.Rows[i]["LName"].ToString()+"; ";
					}
					log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.")+"\r\n";
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string RecallsWithInvalidRecallType(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT recall.RecallTypeNum 
				FROM recall 
				LEFT JOIN recalltype ON recalltype.RecallTypeNum=recall.RecallTypeNum 
				WHERE recalltype.RecallTypeNum IS NULL";
			List<long> listRecallTypeNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listRecallTypeNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Recalls found with invalid recall types:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					//Inserting temporary recall types so that the recalls are no longer orphaned
					int numberFixed=listRecallTypeNums.Count;
					listRecallTypeNums=listRecallTypeNums.Distinct().ToList();
					for(int i = 0;i<listRecallTypeNums.Count;i++) {
						command="INSERT INTO recalltype (RecallTypeNum,Description,DefaultInterval,TimePattern,Procedures) VALUES ("
							+POut.Long(listRecallTypeNums[i])+",'Temporary Recall "+POut.Int(i+1)+"',0,'','')";
						Db.NonQ(command);
					}
					int numberFixedTypes=listRecallTypeNums.Count;
					if(numberFixedTypes > 0) {
						Signalods.SetInvalid(InvalidType.RecallTypes);
					}
					if(numberFixed > 0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Recalls fixed with invalid recall types:")+" "+numberFixed+". "
							+Lans.g("FormDatabaseMaintenance","Temporary recall types added:")+" "+numberFixedTypes+". "
							+Lans.g("FormDatabaseMaintenance","Go to Setup | Appointments | Recall Types "
								+"to either rename them or remove all recalls from these recall types.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string RecallTriggerDeleteBadCodeNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM recalltrigger WHERE NOT EXISTS (SELECT * FROM procedurecode WHERE procedurecode.CodeNum=recalltrigger.CodeNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Recall triggers found with bad codenum: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"DELETE FROM recalltrigger
						WHERE NOT EXISTS (SELECT * FROM procedurecode WHERE procedurecode.CodeNum=recalltrigger.CodeNum)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0) {
						Signalods.SetInvalid(InvalidType.RecallTypes);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Recall triggers deleted due to bad codenum: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string RefAttachDeleteWithInvalidReferral(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM refattach WHERE ReferralNum NOT IN (SELECT ReferralNum FROM referral)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ref attachments found with invalid referrals: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM refattach WHERE ReferralNum NOT IN (SELECT ReferralNum FROM referral)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Ref attachments with invalid referrals deleted: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Finds patients that have a more than 1 from referral with the same order.</summary>
		[DbmMethodAttr]
		public static string RefAttachesWithDuplicateOrder(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT DISTINCT PatNum 
						FROM refattach 
						GROUP BY PatNum,ItemOrder
						HAVING COUNT(*) > 1";
					int numFound=Db.GetListLong(command).Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients found with multiple referral attachments of the same order:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"SELECT refattach.*
						FROM (
							SELECT DISTINCT PatNum
							FROM refattach
							GROUP BY PatNum,ItemOrder
							HAVING COUNT(*) > 1
						) multattach
						INNER JOIN refattach ON refattach.PatNum=multattach.PatNum";
					List<RefAttach> listRefAttaches=Crud.RefAttachCrud.SelectMany(command);
					List<long> listPatNumsDist=listRefAttaches.Select(x => x.PatNum).Distinct().ToList();
					for(int i=0;i<listPatNumsDist.Count;i++){
						List<RefAttach> listRefAttachesForPat=listRefAttaches.FindAll(x => x.PatNum==listPatNumsDist[i]);
						//Change the order of all ref attaches on the patient so that none have the same ItemOrder.
						List<RefAttach> listRefAttachesForPatOrdered=listRefAttachesForPat.OrderBy(x => x.ItemOrder).ThenBy(x => x.RefDate).ThenBy(x => x.RefAttachNum).ToList();
						for(int j=0;j<listRefAttachesForPatOrdered.Count;j++){
							RefAttach refAttachOld=listRefAttachesForPatOrdered[j].Copy();
							listRefAttachesForPatOrdered[j].ItemOrder=j+1;
							RefAttaches.Update(listRefAttachesForPatOrdered[j],refAttachOld);
						}
					}
					int numberFixed=listRefAttaches.Select(x => x.PatNum).Distinct().Count();
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients fixed with multiple referral attachments of the same order:")+" "+numberFixed+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string RxAlertBadAllergyDefNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM rxalert WHERE rxalert.AllergyDefNum!=0 AND NOT EXISTS (SELECT * FROM allergydef WHERE allergydef.AllergyDefNum=rxalert.AllergyDefNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Rx alerts with bad allergy definitions: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					//command=@"SELECT * FROM rxalert WHERE NOT EXISTS (SELECT * FROM allergydef WHERE allergydef.AllergyDefNum=rxalert.AllergyDefNum)";
					//table=Db.GetTable(command);
					command="UPDATE rxalert SET AllergyDefNum=0 WHERE rxalert.AllergyDefNum!=0 AND NOT EXISTS (SELECT * FROM allergydef WHERE allergydef.AllergyDefNum=rxalert.AllergyDefNum)";
					int numRowsChanged=(int)Db.NonQ(command);
					if(numRowsChanged>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Rx alerts with bad allergy definitions cleared: ")+numRowsChanged.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion Recall, RecallTrigger, RefAttach, RxAlert------------------------------------------------------------------------------------
		#region ScheduleOp, Schedule, SecurityLog, Sheet------------------------------------------------------------------------------------------------

		[DbmMethodAttr]
		public static string ScheduleOpsInvalidScheduleNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM scheduleop WHERE NOT EXISTS(SELECT * FROM schedule WHERE scheduleop.ScheduleNum=schedule.ScheduleNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Scheduleops with invalid ScheduleNums found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM scheduleop WHERE NOT EXISTS(SELECT * FROM schedule WHERE scheduleop.ScheduleNum=schedule.ScheduleNum)";
					int numFixed=(int)Db.NonQ(command);
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Scheduleops with invalid ScheduleNums deleted")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SchedulesBlockoutStopBeforeStart(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM schedule WHERE StopTime<StartTime";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Schedules and blockouts having stop time before start time: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM schedule WHERE StopTime<StartTime";
					int numFixed=(int)Db.NonQ(command);
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Schedules and blockouts having stop time before start time fixed: ")+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SchedulesDeleteHiddenProviders(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM provider WHERE IsHidden=1 AND ProvNum IN (SELECT ProvNum FROM schedule WHERE SchedDate > "+DbHelper.Now()+" GROUP BY ProvNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Hidden providers found on future schedules: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="SELECT ProvNum FROM provider WHERE IsHidden=1 AND ProvNum IN (SELECT ProvNum FROM schedule WHERE SchedDate > "+DbHelper.Now()+" GROUP BY ProvNum)";
					DataTable table=Db.GetTable(command);
					List<long> provNums=new List<long>();
					for(int i = 0;i<table.Rows.Count;i++) {
						provNums.Add(PIn.Long(table.Rows[i]["ProvNum"].ToString()));
					}
					Providers.RemoveProvsFromFutureSchedule(provNums);//Deletes future schedules for providers.
					if(provNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Hidden providers found on future schedules fixed: ")+provNums.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SchedulesDeleteShort(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command=@"SELECT schedule.ScheduleNum
				FROM schedule
				WHERE schedule.Status="+POut.Int((int)SchedStatus.Open)/*closed and holiday statuses do not use starttime and stoptime*/+@"
				AND TIMEDIFF(schedule.StopTime,schedule.StartTime)<'00:05:00'
				AND (schedule.Note=''"/*we don't want to remove provider notes, employee notes, or pratice notes.*/+@"
				OR schedule.SchedType="+POut.Int((int)ScheduleType.WebSchedASAP)+")";
			List<long> listScheduleNums=Db.GetListLong(command);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=listScheduleNums.Count;
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Schedule blocks invalid:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=listScheduleNums.Count;
					if(listScheduleNums.Count > 0) {
						command="DELETE FROM schedule WHERE ScheduleNum IN("+string.Join(",",listScheduleNums)+")";
						Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Schedule blocks fixed:")+" "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SchedulesDeleteProvClosed(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM schedule WHERE SchedType=1 AND Status=1";//type=prov,status=closed
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Schedules found which are causing printing issues: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM schedule WHERE SchedType=1 AND Status=1";//type=prov,status=closed
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Schedules deleted that were causing printing issues: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>This function will fix any FKey entries in securitylog that point to entries in other tables that have been deleted (orphaned FKeys).
		///It uses reflection to find all tables using audit trail Fkey columns and their respective permissions.
		///This method does not need to change even if more permissions are added.</summary>
		//No longer needed, removed per Nathan.
		//public static string SecurityLogInvalidFKey(bool verbose,DbmMode modeCur) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,modeCur);
		//	}
		//	if(modeCur==DbmMode.Breakdown) {
		//		return "";
		//	}
		//	string log="";
		//	long numFoundOrFixed=0;
		//	Assembly assembly=Assembly.GetExecutingAssembly();
		//	Type[] arrTypes;
		//	try {
		//		arrTypes=assembly.GetTypes();
		//	}
		//	catch(ReflectionTypeLoadException ex) {//Some dependencies were probably not able to be loaded
		//		log+="Error: Unable to load types from OpenDentBusiness. Try running the method on a different computer.\r\n";
		//		if(verbose) {
		//			log+="Types successfully loaded:\r\n"+string.Join("\r\n",ex.Types.Where(x => x!=null))+"\r\n";
		//		}
		//		return log;
		//	}
		//	foreach(Type type in arrTypes) {
		//		if(type.Namespace!="OpenDentBusiness" || !type.IsClass) {
		//			continue;
		//		}
		//		string tableName=type.Name.ToLower();
		//		List<CrudTableAttribute> listCrudTableAttributes=type.GetCustomAttributes(typeof(CrudTableAttribute),true)
		//			.Select(x => (CrudTableAttribute)x).ToList();
		//		//Get audit trail permissions
		//		if(listCrudTableAttributes.Count!=1 || listCrudTableAttributes[0].AuditPerms==CrudAuditPerm.None) {
		//			continue;
		//		}
		//		List<Permissions> listPermissions=GroupPermissions.GetPermsFromCrudAuditPerm(listCrudTableAttributes[0].AuditPerms);
		//		if(listPermissions.Count==0) {
		//			//This error log is explicitly for Open Dental engineers and is purposefully not translated.
		//			log+="Error: Permission not found in GetPermsFromCrudAuditPerm() for "+tableName+" \r\n";
		//			continue;
		//		}
		//		//Make a comma delimited string of the int values of each permission for this class.
		//		string permsCommaDelimStr=String.Join(",",listPermissions.Select(x => (int)x).ToList());
		//		//Get the table type name from the type.
		//		if(listCrudTableAttributes[0].TableName!="") {
		//			tableName=listCrudTableAttributes[0].TableName;
		//		}
		//		//Get primary key column name
		//		string priKeyColumnName="";
		//		foreach(FieldInfo field in type.GetFields()) {
		//			List<CrudColumnAttribute> listCrudColumnAttributes=field.GetCustomAttributes(typeof(CrudColumnAttribute),true)
		//				.Select(x => (CrudColumnAttribute)x).ToList();
		//			if(listCrudColumnAttributes.Count!=1 || !listCrudColumnAttributes[0].IsPriKey) {
		//				continue;
		//			}
		//			priKeyColumnName=field.Name;
		//			break;
		//		}
		//		if(priKeyColumnName=="") {
		//			//This error log is explicitly for Open Dental engineers and is purposefully not translated.
		//			log+="Error: Primary key attribute not found for table "+tableName+"\r\n";
		//			continue;
		//		}
		//		if(modeCur==DbmMode.Check) {
		//			numFoundOrFixed+=GetCountForSecuritylogInvalidFKeys(permsCommaDelimStr,tableName,priKeyColumnName);
		//		}
		//		else {
		//			numFoundOrFixed+=UpdateOrphanedSecuritylogInvalidKeys(permsCommaDelimStr,tableName,priKeyColumnName);
		//		}
		//	}
		//	if(modeCur==DbmMode.Check) {
		//		if(numFoundOrFixed>0 || verbose) {
		//			log+=Lans.g("FormDatabaseMaintenance","Audit trail entries with invalid FKeys found")+": "+numFoundOrFixed.ToString()+"\r\n";
		//		}
		//	}
		//	else {
		//		if(numFoundOrFixed>0 || verbose) {
		//			log+=Lans.g("FormDatabaseMaintenance","Audit trail entries with invalid FKeys fixed")+": "+numFoundOrFixed.ToString()+"\r\n";
		//		}
		//	}
		//	return log;
		//}

		[DbmMethodAttr]
		public static string SheetDepositSlips(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT SheetNum FROM sheet WHERE SheetType="+POut.Int((int)SheetTypeEnum.DepositSlip);
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deposit slip sheets")+": "+table.Rows.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(table.Rows.Count>0) {
						for(int i = 0;i<table.Rows.Count;i++) {
							long sheetNum=PIn.Long(table.Rows[i]["SheetNum"].ToString());
							command="DELETE FROM sheetfield WHERE SheetNum="+POut.Long(sheetNum);
							Db.NonQ(command);
							command="DELETE FROM sheet WHERE SheetNum="+POut.Long(sheetNum);
							Db.NonQ(command);
						}
					}
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Deposit slip sheets deleted")+": "+table.Rows.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SheetFieldsWithEmptyItemColor(bool verbose,DbmMode dbmMode){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
			return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
		 	string command=$@"SELECT SUM(c) FROM (SELECT COUNT(*) c FROM sheetfield WHERE ItemColor={POut.Int(Color.Empty.ToArgb())} 
				UNION ALL
				SELECT COUNT(*) FROM sheetfielddef  WHERE ItemColor={POut.Int(Color.Empty.ToArgb())}) a";
			string count=Db.GetCount(command);
			string log="";
			//Place any other variables which are used for the check and fix here.
			switch(dbmMode){
				case DbmMode.Check:
					if(count!="0" || verbose){
						log+=Lans.g("FormDatabaseMaintenance","SheetFields found with invalid color")+": "+count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(count!="0"){
						command=$"UPDATE sheetfield SET ItemColor={POut.Int(Color.Black.ToArgb())} WHERE ItemColor={POut.Int(Color.Empty.ToArgb())}";
						Db.NonQ(command);
						command=$"UPDATE sheetfielddef SET ItemColor={POut.Int(Color.Black.ToArgb())} WHERE ItemColor={POut.Int(Color.Empty.ToArgb())}";
						Db.NonQ(command); 
					}
					if(count!="0" || verbose){
						log+=Lans.g("FormDatabaseMaintenance","SheetFields fixed with invalid color")+": "+count+"\r\n";
					}
					break;
				}
			return log;
		}

		///<summary>At one point, we had an issue on our Web Forms server that possibly caused offices to import 1000's of blank Web Forms.</summary>
		[DbmMethodAttr]
		public static string SheetsWithNoSheetFields(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT sheet.SheetNum 
				FROM sheet 
				LEFT JOIN sheetfield ON sheetfield.SheetNum=sheet.SheetNum
				WHERE sheet.IsWebForm=1
				AND sheetfield.SheetNum IS NULL";
			List<long> listSheetNums=Db.GetListLong(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listSheetNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Blank Web Forms sheets found")+": "+listSheetNums.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					if(listSheetNums.Count>0) {
						command="DELETE FROM sheet WHERE SheetNum IN("+string.Join(",",listSheetNums.Select(POut.Long))+")";
						Db.NonQ(command);
					}
					if(listSheetNums.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Blank Web Forms sheets deleted")+": "+listSheetNums.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion ScheduleOp, Schedule, SecurityLog, Sheet---------------------------------------------------------------------------------------------
		#region Signal, SigMessage, SmsFromMobile, Statement, SummaryOfCare--------------------------------------------------------------------------------------------

		[DbmMethodAttr]
		public static string SignalInFuture(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM signalod WHERE SigDateTime > "+DbHelper.Now();
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Signalod entries with future time:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"DELETE FROM signalod WHERE SigDateTime > "+DbHelper.Now();
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Signalod entries with future times deleted:")+" "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SigMessageInFuture(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command=@"SELECT COUNT(*) FROM sigmessage WHERE MessageDateTime > "+DbHelper.Now()+" OR AckDateTime > "+DbHelper.Now();
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Sigmessage entries with future time:")+" "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command=@"DELETE FROM sigmessage WHERE MessageDateTime > "+DbHelper.Now()+" OR AckDateTime > "+DbHelper.Now();
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Sigmessage entries with future times deleted:")+" "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SmsFromMobilesInvalidClinicNum(bool verbose, DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="";
			switch(dbmMode){
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM smsfrommobile WHERE ClinicNum=-1";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Messages found with invalid ClinicNum(s)")+": "+numFound+"\r\n";
					}
					break;
			case DbmMode.Fix:
					//Grab all the smsfrommobiles that have -1 clinicnums and try to match them up with smsphones that have valid clinicnums.
					command="SELECT s.SmsFromMobileNum,p.ClinicNum " +
					"FROM smsfrommobile s " +
					"INNER JOIN smsphone p ON s.SmsPhoneNumber=p.PhoneNumber " +
					"WHERE s.ClinicNum=-1 AND p.ClinicNum<>-1";
					DataTable table=Db.GetTable(command);
					int numFixed=0;
					for(int i = 0;i < table.Rows.Count;i++) {
						command="UPDATE smsfrommobile SET ClinicNum="+PIn.Long(table.Rows[i]["ClinicNum"].ToString())
							+" WHERE SmsFromMobileNum="+PIn.Long(table.Rows[i]["SmsFromMobileNum"].ToString());
						Db.NonQ(command);
						numFixed++;
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Messages with invalid ClinicNum(s) fixed")+": "+numFixed+"\r\n";
					}
					break;
				}
		 return log;
		}

		[DbmMethodAttr]
		public static string StatementDateRangeMax(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM statement WHERE DateRangeTo='9999-12-31'";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Statement DateRangeTo max found: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE statement SET DateRangeTo='2200-01-01' WHERE DateRangeTo='9999-12-31'";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Statement DateRangeTo max fixed: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string StatementsWithInvalidDocNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM statement WHERE DocNum>0 AND DocNum NOT IN (SELECT DocNum FROM document)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Statements with invalid DocNum found")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE statement SET DocNum=0 WHERE DocNum>0 AND DocNum NOT IN (SELECT DocNum FROM document)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Statements with invalid DocNum fixed")+": "+numberFixed+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string SummaryOfCaresWithoutReferralsAttached(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(dbmMode==DbmMode.Breakdown) {
				return "";
			}
			string log="";
			string command="SELECT * FROM refattach WHERE RefAttachNum NOT IN ("
					+"SELECT FKey FROM ehrmeasureevent WHERE EventType="+POut.Int((int)EhrMeasureEventType.SummaryOfCareProvidedToDr)+" "
					+"OR EventType="+POut.Int((int)EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic)+") "
				+"AND RefType="+POut.Int((int)ReferralType.RefTo)+" "
				+"AND IsTransitionOfCare=1 ";
			//We want to fix as many measure events as we can even if they aren't good enough to count towards the actual measure. 
			//+"AND ProvNum!=0";//E.g. we will link measure events to refattaches even if the ref attach has no provider.  This way, they only have to fix the ref attach in order for their measures to show.
			List<RefAttach> refAttaches=Crud.RefAttachCrud.SelectMany(command);
			command="SELECT * FROM ehrmeasureevent "
				+"WHERE FKey NOT IN (SELECT RefAttachNum FROM refattach WHERE RefType="+POut.Int((int)ReferralType.RefTo)+" AND IsTransitionOfCare=1) "
				+"AND EventType="+POut.Int((int)EhrMeasureEventType.SummaryOfCareProvidedToDr)+" "
				+"OR EventType="+POut.Int((int)EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic)+" "
				+"ORDER BY DateTEvent";
			List<EhrMeasureEvent> listEhrMeasureEvents=Crud.EhrMeasureEventCrud.SelectMany(command);
			int numberFixed=0;
			for(int i = 0;i<refAttaches.Count;i++) {
				for(int j = 0;j<listEhrMeasureEvents.Count;j++) {
					if(refAttaches[i].PatNum!=listEhrMeasureEvents[j].PatNum
							|| listEhrMeasureEvents[j].FKey!=0
							|| listEhrMeasureEvents[j].DateTEvent<refAttaches[i].RefDate.AddDays(-3)
							|| listEhrMeasureEvents[j].DateTEvent>refAttaches[i].RefDate.AddDays(1))
					{
						continue;
					}
					if(dbmMode!=DbmMode.Check) {
						listEhrMeasureEvents[j].FKey=refAttaches[i].RefAttachNum;
						EhrMeasureEvents.Update(listEhrMeasureEvents[j]);
					}
					listEhrMeasureEvents.RemoveAt(j);
					numberFixed++;
					break;
				}
			}
			if(numberFixed==0 && !verbose){
				return log;
			}
			if(dbmMode==DbmMode.Check) {
				log+=Lans.g("FormDatabaseMaintenance","Summary of cares with no referrals attached")+": "+numberFixed.ToString()+"\r\n";
				return log;
			}
			log+=Lans.g("FormDatabaseMaintenance","Summary of cares that had a referral attached")+": "+numberFixed.ToString()+"\r\n";
			return log;
		}

		#endregion Signal, SigMessage, Statement, SummaryOfCare-----------------------------------------------------------------------------------------
		#region Task, TaskList, TimeCardRule, ToothInitial, TreatPlan-------------------------------------------------------------------------------------------------
		[DbmMethodAttr]
		public static string TaskListsAbandonedRepeating(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command="SELECT TaskListNum FROM tasklist";
			List<string> listTaskListNumsAll=Db.GetListLong(command).Select(x=>POut.Long(x)).ToList();
			string taskListNumsAll=string.Join(",",listTaskListNumsAll);
			switch(dbmMode) {
				case DbmMode.Check:
					int numFound=0;
					if(!string.IsNullOrWhiteSpace(taskListNumsAll)) {
						command="SELECT COUNT(*) FROM tasklist "+
							"WHERE Parent=0 AND FromNum!=0 AND "+ //Parent repeating tasklist whose FromNum is no longer valid
							"FromNum NOT IN ("+taskListNumsAll+")";
						numFound=PIn.Int(Db.GetCount(command));
					}
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Abandoned repeating tasklist(s) found")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numFixed=0;
					if(!string.IsNullOrWhiteSpace(taskListNumsAll)) {
						command="UPDATE tasklist SET Parent=0,FromNum=0,DateTL=DATE('0001-01-01') "+ //Move to main list so user can delete manually
							"WHERE Parent=0 AND FromNum!=0 AND "+ //Parent repeating tasklist whose FromNum is no longer valid
							"FromNum NOT IN ("+taskListNumsAll+")";
						numFixed=(int)Db.NonQ(command);
					}
					if(numFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Abandoned repeating tasklist(s) moved to the 'Main' task tab")+": "+numFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string TaskListsWithCircularParentChild(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			//In order to figure out a cyclical chain of task lists we need to:
			//1. Get all TaskLists
			//2. For each TaskList whose Parent is 0...
			//a. Find all TaskLists in that "family".  We know that in order to have a cyclical relationship NONE of the TaskLists in the cycle can have Parent of 0
			//b. Remove those TaskLists from the "bad list"
			//c. When we run out of TaskLists with Parent of 0, the TaskLists left in the list are those that are part of a TaskList cycle
			//Get a list of all TaskLists
			string command="SELECT * FROM tasklist";
			List<TaskList> listTaskListsAll=Crud.TaskListCrud.SelectMany(command);
			List<TaskList> listTaskListsTrunk=listTaskListsAll.FindAll(x => x.Parent==0);//Find first TaskList with Parent of 0
			listTaskListsAll.RemoveAll(x => x.Parent==0);
			Action<long> RemoveAncestors=null;
			//Delegate method to recursively traverse the tree of a TaskList and remove all child TaskLists.
			RemoveAncestors = new Action<long>(taskListNum => {
				List<TaskList> listTaskListsChildren=listTaskListsAll.FindAll(x => x.Parent==taskListNum);
				for(int i=0;i<listTaskListsChildren.Count;i++) {
					RemoveAncestors.Invoke(listTaskListsChildren[i].TaskListNum);
					listTaskListsAll.Remove(listTaskListsChildren[i]);
				}
			});
			for(int i=0;i<listTaskListsTrunk.Count;i++){
				RemoveAncestors.Invoke(listTaskListsTrunk[i].TaskListNum);
			}
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listTaskListsAll.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Task Lists with circular parent-child relationship")+": "+listTaskListsAll.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int taskListNum=0;
					if(listTaskListsAll.Count>0) {
						command="INSERT INTO tasklist (Descript,Parent,DateTL,IsRepeating,DateType,FromNum,ObjectType,DateTimeEntry) VALUES('FIX TASKLISTS',0,'0001-01-01',0,0,0,0,CURDATE())";
						taskListNum=(int)Db.NonQ(command,true);
					}
					for(int i=0;i<listTaskListsAll.Count;i++){
						//We will set each TaskList's parent to be 0 so the user can again access them via the Main tab and put them wherever they want.
						command="UPDATE tasklist SET Parent="+POut.Long(taskListNum)+" WHERE TaskListNum="+listTaskListsAll[i].TaskListNum.ToString();
						Db.NonQ(command);
					}
					if(listTaskListsAll.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Task Lists with circular parent-child relationship corrected")+": "+listTaskListsAll.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string TasksCompletedWithInvalidFinishDateTime(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT task.TaskNum,IFNULL(MAX(tasknote.DateTimeNote),task.DateTimeEntry) AS DateTimeNoteMax "
				+"FROM task "
				+"LEFT JOIN tasknote ON task.TaskNum=tasknote.TaskNum "
				+"WHERE task.TaskStatus="+POut.Int((int)TaskStatusEnum.Done)+" "
				+"AND task.DateTimeFinished="+POut.DateT(new DateTime(1,1,1))+" "
				+"GROUP BY task.TaskNum";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Tasks completed with invalid Finished Date/Time")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					for(int i=0;i<table.Rows.Count;i++){
						//Update the DateTimeFinished with either the max note DateTime or the time of the tasks DateTimeEntry.
						//We cannot use the raw string in the DataTable because C# has auto-formatted the row into a DateTime row.
						//Therefore we have to convert the string into a DateTime object and then send it back out in the format that MySQL expects.
						DateTime dateTimeNoteMax=PIn.DateT(table.Rows[i]["DateTimeNoteMax"].ToString());
						command="UPDATE task SET DateTimeFinished="+POut.DateT(dateTimeNoteMax)+" "
							+"WHERE TaskNum="+table.Rows[i]["TaskNum"].ToString();
						Db.NonQ(command);
					}
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Tasks completed with invalid Finished Date/Times corrected")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string TaskSubscriptionsInvalid(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM tasksubscription "
						+"WHERE NOT EXISTS(SELECT * FROM tasklist WHERE tasksubscription.TaskListNum=tasklist.TaskListNum)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Task subscriptions invalid: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM tasksubscription "
						+"WHERE NOT EXISTS(SELECT * FROM tasklist WHERE tasksubscription.TaskListNum=tasklist.TaskListNum)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Task subscriptions deleted: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string TaskUnreadsWithoutTasksAttached(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM taskunread "
						+"WHERE taskunread.TaskNum NOT IN(SELECT TaskNum FROM task)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Unread task notifications for deleted tasks")+": "+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="DELETE FROM taskunread "
						+"WHERE taskunread.TaskNum NOT IN(SELECT TaskNum FROM task)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Unread task notifications for deleted tasks removed")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string TimeCardRuleEmployeeNumInvalid(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM timecardrule "
						+"WHERE timecardrule.EmployeeNum!=0 " //0 is all employees, so it is a 'valid' employee number
						+"AND timecardrule.EmployeeNum NOT IN(SELECT employee.EmployeeNum FROM employee)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Time card rules found with invalid employee number: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE timecardrule "
						+"SET timecardrule.EmployeeNum=0 "
						+"WHERE timecardrule.EmployeeNum!=0 " //don't set to 0 if already 0
						+"AND timecardrule.EmployeeNum NOT IN(SELECT employee.EmployeeNum FROM employee)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Time card rules applied to All Employees due to invalid employee number: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr(HasPatNum=true,IsReplicationUnsafe=true)]
		public static string ToothChartInvalidDrawingSegments(bool verbose, DbmMode dbmMode,long patNum=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode,patNum);
			}
			string log="";
			string command="SELECT ToothInitialNum FROM toothinitial " +
				"WHERE (DrawingSegment LIKE '%,;%' OR DrawingSegment LIKE '%;,%' " +//any middle coordinate is invalid
				"OR DrawingSegment LIKE ',%' " +//first coordinate is invalid
				"OR DrawingSegment LIKE '%,') ";//last coordinate is invalid
			if(patNum!=0) {
				command+=$"AND PatNum={POut.Long(patNum)} ";
			}
			List<long> listToothInitialNumsInvalid=Db.GetListLong(command);
			int numRows=listToothInitialNumsInvalid.Count;
			switch(dbmMode) {
				case DbmMode.Check:
					if(numRows>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid tooth chart drawing segments found")+": "+numRows.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numberFixed=0;
					if(numRows>0) {
						command="DELETE FROM toothinitial WHERE ToothInitialNum IN ("+string.Join(",",listToothInitialNumsInvalid.Select(x => POut.Long(x)))+")";
						numberFixed=(int)Db.NonQ(command);
					}
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Number of invalid tooth chart drawing segments removed")+": "+numberFixed.ToString()+"\r\n";
					}
					string methodName=MethodBase.GetCurrentMethod().Name;
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					for(int i=0;i<listToothInitialNumsInvalid.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,listToothInitialNumsInvalid[i],DbmLogFKeyType.ToothInitial,
							DbmLogActionType.Delete,methodName,"Removed row with invalid drawing segment from toothintial.");
						listDbmLogs.Add(dbmLog);
					}
					DbmLogs.InsertMany(listDbmLogs);
					break;
			}
			return log;
		}
		
		[DbmMethodAttr(IsOneOff=true)]
		public static string ToothChartInvalidInitialTypes(bool verbose, DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			ToothInitialType toothInitialTypeMax=Enum.GetValues(typeof(ToothInitialType)).Cast<ToothInitialType>().Max();
			string command=$"SELECT ToothInitialNum FROM toothinitial WHERE InitialType > {POut.Int((int)toothInitialTypeMax)}";
			List<long> listToothInitialNumsInvalid=Db.GetListLong(command);
			switch(dbmMode){
				case DbmMode.Check:
					if(listToothInitialNumsInvalid.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Invalid tooth chart annotations found")+": "+listToothInitialNumsInvalid.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					int numFixed=0;
					if(listToothInitialNumsInvalid.Count > 0) {
						command=$"DELETE FROM toothinitial WHERE ToothInitialNum IN ({string.Join(",",listToothInitialNumsInvalid)})";
						numFixed=(int)Db.NonQ(command);
					}
					log+=Lans.g("FormDatabaseMaintenance","Number of invalid tooth chart annotations removed")+": "+numFixed.ToString()+"\r\n";
					break;
				}
			return log;
		}

		[DbmMethodAttr]
		public static string TreatPlansInvalid(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT treatplan.PatNum FROM procedurelog	"//procs for 1 pat attached to a treatplan for another
					+"INNER JOIN treatplanattach ON treatplanattach.ProcNum=procedurelog.ProcNum "
					+"INNER JOIN treatplan ON treatplan.TreatPlanNum=treatplanattach.TreatPlanNum AND procedurelog.PatNum!=treatplan.PatNum "
					+"UNION "//more than 1 active treatment plan
					+"SELECT PatNum FROM treatplan WHERE TPStatus=1 GROUP BY PatNum HAVING COUNT(DISTINCT TreatPlanNum)>1";
			List<long> listPatNumsForAudit=Db.GetListLong(command).Distinct().ToList();
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(listPatNumsForAudit.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients found with one or more invalid treatment plans")+": "+listPatNumsForAudit.Count+"\r\n";
					}
					break;
				case DbmMode.Fix:
					for(int i=0;i<listPatNumsForAudit.Count;i++){
						if(DiscountPlanSubs.HasDiscountPlan(listPatNumsForAudit[i])){
							TreatPlans.AuditPlans(listPatNumsForAudit[i],TreatPlanType.Discount);
							continue;
						}
						TreatPlans.AuditPlans(listPatNumsForAudit[i],TreatPlanType.Insurance);
					}
					TreatPlanAttaches.DeleteOrphaned();
					if(listPatNumsForAudit.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Patients with one or more invalid treatment plans fixed")+": "+listPatNumsForAudit.Count+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Finds treatplanattaches with the same treatplannum and procnum.</summary>
		[DbmMethodAttr]
		public static string TreatPlanAttachDuplicateProc(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT treatplanattach.TreatPlanNum, MIN(treatplanattach.TreatPlanAttachNum) AS OriginalTPANum, "
			+ " ProcNum, "
			+ " COUNT(ProcNum) NumDupes "
			+ " FROM treatplanattach "
			+ " GROUP BY treatplanattach.treatplannum, treatplanattach.ProcNum "
			+ " HAVING NumDupes > 1 ";
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","TreatPlanAttaches with duplicate ProcNums and TreatPlanNums found")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					for(int i = 0;i < table.Rows.Count;i++) {
						command="DELETE FROM treatplanattach WHERE treatplanattach.TreatPlanNum="+table.Rows[i]["TreatPlanNum"]
							+" AND treatplanattach.ProcNum="+table.Rows[i]["ProcNum"]
							+" AND treatplanattach.TreatPlanAttachNum != "+table.Rows[i]["OriginalTPANum"];
						Db.NonQ(command);
					}
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","TreatPlanAttaches with duplicate ProcNums and TreatPlanNums deleted")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///<summary>Finds proctps that have been orphaned and creates dummy treatment plans for DateTime.MinValue so that the orphaned proctps can be viewed.</summary>
		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string TreatPlanOrphanedProcTps(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command=@"SELECT proctp.PatNum,proctp.TreatPlanNum 
				FROM proctp
				LEFT JOIN treatplan ON treatplan.TreatPlanNum = proctp.TreatPlanNum 
				WHERE treatplan.TreatPlanNum IS NULL 
				GROUP BY proctp.TreatPlanNum";
			DataTable table=Db.GetTable(command);
			string log = "";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Treatment Plans with orphaned proctps")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					for(int i = 0;i < table.Rows.Count;i++) {
						TreatPlan treatPlan = new TreatPlan();
						treatPlan.DateTP = DateTime.MinValue;
						treatPlan.Heading = "MISSING TREATMENT PLAN";
						treatPlan.Note = "This treatment plan was created by Database Maintenence because of orphaned proctps.";
						treatPlan.PatNum = PIn.Long(table.Rows[i]["PatNum"].ToString());
						treatPlan.SecUserNumEntry = Security.CurUser.UserNum;
						treatPlan.TreatPlanNum = PIn.Long(table.Rows[i]["TreatPlanNum"].ToString());
						treatPlan.TPStatus = TreatPlanStatus.Saved;
						Crud.TreatPlanCrud.Insert(treatPlan,true);
					}
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Treatment Plans with orphaned proctps fixed")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion Task, TaskList, TimeCardRule, TreatPlan----------------------------------------------------------------------------------------------
		#region UnscheduledAppt, Userod-----------------------------------------------------------------------------------------------------------------

		[DbmMethodAttr(IsReplicationUnsafe=true)]
		public static string UnscheduledApptsWithInvalidOpNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string command="SELECT AptNum FROM appointment WHERE Op != 0 AND AptStatus=3";//UnschedList
			DataTable table=Db.GetTable(command);
			string log="";
			switch(dbmMode) {
				case DbmMode.Check:
					if(table.Rows.Count>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Unscheduled appointments with invalid Op nums")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					List<DbmLog> listDbmLogs=new List<DbmLog>();
					string methodName=MethodBase.GetCurrentMethod().Name;
					command="UPDATE appointment SET Op=0 WHERE AptStatus=3";//UnschedList
					Db.NonQ(command);
					for(int i=0;i<table.Rows.Count;i++){
						DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,PIn.Long(table.Rows[i]["AptNum"].ToString()),DbmLogFKeyType.Appointment,
							DbmLogActionType.Update,methodName,"Fixed invalid OpNum from UnscheduledApptsWithInvalidOpNum.");
						listDbmLogs.Add(dbmLog);
					}
					if(table.Rows.Count>0 || verbose) {
						Crud.DbmLogCrud.InsertMany(listDbmLogs);
						log+=Lans.g("FormDatabaseMaintenance","Unscheduled appointments with invalid Op nums corrected")+": "+table.Rows.Count.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		/// <summary>Only one user of a given UserName may be unhidden at a time. Warn the user and instruct them to hide extras.</summary>
		[DbmMethodAttr(HasBreakDown=true,HasExplain=true)]
		public static string UserodDuplicateUser(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return Lans.g("FormDatabaseMaintenance","Currently not Oracle compatible.  Please call support.");
			}
			string command="SELECT UserName FROM userod WHERE IsHidden=0 GROUP BY UserName HAVING Count(*)>1;";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0 && !verbose) {
				return "";
			}
			string log=Lans.g("FormDatabaseMaintenance","Users with duplicates")+": "+table.Rows.Count;
			switch(dbmMode) {
				case DbmMode.Check:
				case DbmMode.Fix:
					log+="\r\n";
					if(table.Rows.Count!=0) {
						log+="   "+Lans.g("FormDatabaseMaintenance","Manual fix needed.  Double click to see a break down.")+"\r\n";
					}
					break;
				case DbmMode.Breakdown:
					if(table.Rows.Count>0) {
						log+=", "+Lans.g("FormDatabaseMaintenance","including")+":\r\n";
						for(int i = 0;i<table.Rows.Count;i++) {
							log+=Lans.g("FormDatabaseMaintenance","User")+" - "+table.Rows[i]["UserName"].ToString()+"\r\n";
						}
						log+=Lans.g("FormDatabaseMaintenance","   They need to be fixed manually.  Please go to Setup | Security and hide all but one of each unique user.")+"\r\n";
					}
					break;
			}
			return log;
		}

		[DbmMethodAttr]
		public static string UserodInvalidClinicNum(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT Count(*) FROM userod WHERE ClinicNum<>0 AND ClinicNum NOT IN (SELECT ClinicNum FROM clinic)";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Users found with invalid ClinicNum: ")+numFound+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE userod SET ClinicNum=0 WHERE ClinicNum<>0 AND ClinicNum NOT IN (SELECT ClinicNum FROM clinic)";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Users fixed with invalid ClinicNum: ")+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		///// <summary>Deprecated as of 17.4 since we began using the usergroupattach table. userod has an invalid FK to usergroup</summary>
		//[DbmMethodAttr]
		//public static string UserodInvalidUserGroupNum(bool verbose,DbmMode modeCur) {
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//		return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,modeCur);
		//	}
		//	string log="";
		//	switch(modeCur) {
		//		case DbmMode.Check:
		//			command="SELECT Count(*) FROM userod WHERE UserGroupNum NOT IN (SELECT UserGroupNum FROM usergroup) ";
		//			long numFound=PIn.Long(Db.GetCount(command));
		//			if(numFound>0 || verbose) {
		//				log+=Lans.g("FormDatabaseMaintenance","Users found with invalid UserGroupNum: ")+numFound+"\r\n";
		//			}
		//			break;
		//		case DbmMode.Fix:
		//			command="SELECT * FROM userod WHERE UserGroupNum NOT IN (SELECT UserGroupNum FROM usergroup) ";
		//			table=Db.GetTable(command);
		//			long userNum;
		//			string userName;
		//			long userGroupNum;
		//			long numberFixed=0;
		//			for(int i=0;i<table.Rows.Count;i++) {//Create a usergroup with the same name as the userod+"Group"
		//				userNum=PIn.Long(table.Rows[i]["UserNum"].ToString());
		//				userName=PIn.String(table.Rows[i]["UserName"].ToString());
		//				command="INSERT INTO usergroup (Description) VALUES('"+POut.String(userName+" Group")+"')";
		//				userGroupNum=Db.NonQ(command,true);
		//				command="UPDATE userod SET UserGroupNum="+POut.Long(userGroupNum)+" WHERE UserNum="+POut.Long(userNum);
		//				Db.NonQ(command);
		//				numberFixed++;
		//			}
		//			if(numberFixed>0 || verbose) {
		//				log+=Lans.g("FormDatabaseMaintenance","Users fixed with invalid UserGroupNum: ")+numberFixed.ToString()+"\r\n";
		//			}
		//			break;
		//	}
		//	return log;
		//}

		/// <summary>userod is restricted to ClinicNum 0 - All.  Restricted to All clinics doesn't make sense.  This will set the ClinicIsRestricted bool to false if ClinicNum=0.</summary>
		[DbmMethodAttr]
		public static string UserodInvalidRestrictedClinic(bool verbose,DbmMode dbmMode) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),verbose,dbmMode);
			}
			string log="";
			string command;
			switch(dbmMode) {
				case DbmMode.Check:
					command="SELECT COUNT(*) FROM userod WHERE ClinicNum=0 AND ClinicIsRestricted=1";
					int numFound=PIn.Int(Db.GetCount(command));
					if(numFound>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Users found restricted to an invalid clinic")+": "+numFound.ToString()+"\r\n";
					}
					break;
				case DbmMode.Fix:
					command="UPDATE userod SET ClinicIsRestricted=0 WHERE ClinicNum=0 AND ClinicIsRestricted=1";
					int numberFixed=(int)Db.NonQ(command);
					if(numberFixed>0 || verbose) {
						log+=Lans.g("FormDatabaseMaintenance","Users fixed with restriction to an invalid clinic")+": "+numberFixed.ToString()+"\r\n";
					}
					break;
			}
			return log;
		}

		#endregion UnscheduledAppt, Userod--------------------------------------------------------------------------------------------------------------

		#endregion Methods That Apply to Specific Tables----------------------------------------------------------------------------------------------------
		#region Tool Tab and Helper Methods--------------------------------------------------------------------------------------------------------------

		public static List<string> GetDatabaseNames() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> listRetVals=new List<string>();
			string command="SHOW DATABASES";
			//if this next step fails, table will simply have 0 rows
			DataTable table=Db.GetTable(command);
			for(int i = 0;i<table.Rows.Count;i++) {
				listRetVals.Add(table.Rows[i][0].ToString());
			}
			return listRetVals;
		}

		///<summary>Will return empty string if no problems.</summary>
		public static string GetDuplicateClaimProcs() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string retVal="";
			string command=@"SELECT LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber, COUNT(*) cnt
FROM claimproc
LEFT JOIN patient ON patient.PatNum=claimproc.PatNum
WHERE ClaimNum > 0
AND ProcNum>0
AND Status!=4/*exclude supplemental*/
GROUP BY LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber 
HAVING cnt>1";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return "";
			}
			retVal+="Duplicate claim payments found:\r\n";
			DateTime date;
			for(int i = 0;i<table.Rows.Count;i++) {
				if(i>0) {//check for duplicate rows.  We only want to report each claim once.
					if(table.Rows[i]["ClaimNum"].ToString()==table.Rows[i-1]["ClaimNum"].ToString()) {
						continue;
					}
				}
				date=PIn.Date(table.Rows[i]["ProcDate"].ToString());
				retVal+=table.Rows[i]["LName"].ToString()+", "
					+table.Rows[i]["FName"].ToString()+" "
					+"("+table.Rows[i]["PatNum"].ToString()+"), "
					+date.ToShortDateString()+"\r\n";
			}
			retVal+="\r\n";
			return retVal;
		}

		///<summary>Will return empty string if no problems.</summary>
		public static string GetDuplicateSupplementalPayments() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string retVal="";
			string command=@"SELECT LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber, COUNT(*) cnt
FROM claimproc
LEFT JOIN patient ON patient.PatNum=claimproc.PatNum
WHERE ClaimNum > 0
AND ProcNum>0
AND Status=4/*only supplemental*/
GROUP BY LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber
HAVING cnt>1";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return "";
			}
			retVal+="Duplicate supplemental payments found (may be false positives):\r\n";
			DateTime date;
			for(int i = 0;i<table.Rows.Count;i++) {
				if(i>0) {
					if(table.Rows[i]["ClaimNum"].ToString()==table.Rows[i-1]["ClaimNum"].ToString()) {
						continue;
					}
				}
				date=PIn.Date(table.Rows[i]["ProcDate"].ToString());
				retVal+=table.Rows[i]["LName"].ToString()+", "
					+table.Rows[i]["FName"].ToString()+" "
					+"("+table.Rows[i]["PatNum"].ToString()+"), "
					+date.ToShortDateString()+"\r\n";
			}
			retVal+="\r\n";
			return retVal;
		}

		///<summary></summary>
		public static string GetMissingClaimProcs(string dbOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),dbOld);
			}
			string retVal="";
			string command="SELECT LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber "
				+"FROM "+dbOld+".claimproc "
				+"LEFT JOIN "+dbOld+".patient ON "+dbOld+".patient.PatNum="+dbOld+".claimproc.PatNum "
				+"WHERE NOT EXISTS(SELECT * FROM claimproc WHERE claimproc.ClaimProcNum="+dbOld+".claimproc.ClaimProcNum) "
				+"AND ClaimNum > 0 AND ProcNum>0";
			DataTable table=Db.GetTable(command);
			double insPayAmt;
			double feeBilled;
			int count=0;
			for(int i = 0;i<table.Rows.Count;i++) {
				insPayAmt=PIn.Double(table.Rows[i]["InsPayAmt"].ToString());
				feeBilled=PIn.Double(table.Rows[i]["FeeBilled"].ToString());
				command="SELECT COUNT(*) FROM "+dbOld+".claimproc "
					+"WHERE ClaimNum= "+table.Rows[i]["ClaimNum"].ToString()+" "
					+"AND ProcNum= "+table.Rows[i]["ProcNum"].ToString()+" "
					+"AND Status= "+table.Rows[i]["Status"].ToString()+" "
					+"AND InsPayAmt= '"+POut.Double(insPayAmt)+"' "
					+"AND FeeBilled= '"+POut.Double(feeBilled)+"' "
					+"AND LineNumber= "+table.Rows[i]["LineNumber"].ToString();
				string result=Db.GetCount(command);
				if(result!="1") {//only include in result if there are duplicates in old db.
					count++;
				}
			}
			command="SELECT ClaimPaymentNum "
				+"FROM "+dbOld+".claimpayment "
				+"WHERE NOT EXISTS(SELECT * FROM claimpayment WHERE claimpayment.ClaimPaymentNum="+dbOld+".claimpayment.ClaimPaymentNum) ";
			DataTable table2=Db.GetTable(command);
			if(count==0 && table2.Rows.Count==0) {
				return "";
			}
			retVal+="Missing claim payments found: "+count.ToString()+"\r\n";
			retVal+="Missing claim checks found (probably false positives): "+table2.Rows.Count.ToString()+"\r\n";
			return retVal;
		}

		///<summary>Checks various tables for PatNum==0 and creates missing patients.</summary>
		public static string PatientMissing() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			List<PatientMissingTableHelper> listPatientMissingTableHelpers=new List<PatientMissingTableHelper>();
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("appointment","PatNum"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("claim","PatNum"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("claimproc","PatNum"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("commlog","PatNum",
				queryGetPatNums:"SELECT DISTINCT(PatNum) FROM commlog WHERE PatNum NOT IN (SELECT PatNum FROM patient) AND commlog.ReferralNum=0",
				queryGetCount:$"SELECT COUNT(PatNum) FROM commlog WHERE PatNum=0 AND commlog.ReferralNum=0",
				funcGetUpdateQuery:(patNum) => {
					return $"UPDATE commlog SET PatNum={POut.Long(patNum)} WHERE PatNum=0 AND commlog.ReferralNum=0";
				}));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("document","PatNum"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("procedurelog","PatNum"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("inssub","Subscriber"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("payment","PatNum"));
			listPatientMissingTableHelpers.Add(new PatientMissingTableHelper("paysplit","PatNum"));
			//Use UNION instead of UNION ALL so that duplicates are removed.
			string command="SELECT PatNum FROM (\r\n"
				+string.Join("\r\nUNION\r\n",listPatientMissingTableHelpers.Select(x => x.QueryGetPatNums))
				+"\r\n) missing ";
			List<long> listPatNumsMissing=Db.GetListLong(command);
			int countModified=0;
			command="SELECT MAX(PatNum) FROM patient";
			long patNumMax=Db.GetLong(command);
			//Fix is safe because we are not deleting data, we are just attaching abandoned objects to a dummy patient.
			List<DbmLog> listDbmLogs=new List<DbmLog>();
			string methodName=MethodBase.GetCurrentMethod().Name;
			for(int i=0;i<listPatNumsMissing.Count;i++) {
				if(!PrefC.GetBool(PrefName.RandomPrimaryKeys)) {
					if(listPatNumsMissing[i]>patNumMax+100) {
						continue;
					}
				}
				countModified++;
				Patient patientTemp=Patients.CreateNewPatient("Patient",
					"Missing-"+listPatNumsMissing[i].ToString(),
					DateTime.MinValue,
					PrefC.GetLong(PrefName.PracticeDefaultProv),
					0,
					"Database Maintenance Tool method "+methodName+" created this patient due to orphaned entities.",
					logSource:LogSources.DBM,
					patStatus:PatientStatus.Inactive,
					patNum:listPatNumsMissing[i]
				);
				if(listPatNumsMissing[i]==0) {
					for(int j=0;j<listPatientMissingTableHelpers.Count;j++) {
						if(Db.GetCount(listPatientMissingTableHelpers[j].QueryGetCount)=="0") {
							continue;
						}
						Db.NonQ(listPatientMissingTableHelpers[j].FuncGetUpdateQuery(patientTemp.PatNum));
					}
				}
				//Add new patient to listDbmLogs
				DbmLog dbmLog=new DbmLog(Security.CurUser.UserNum,patientTemp.PatNum,DbmLogFKeyType.Patient,DbmLogActionType.Insert,
					methodName,"Database Maintenance Tool method "+methodName+" created this patient due to orphaned entities.");
				listDbmLogs.Add(dbmLog);
			}
			if(countModified>0) {
				Crud.DbmLogCrud.InsertMany(listDbmLogs);
			}
			return "Missing patients added: "+countModified;
		}

		public static string OrthoChartDeleteDuplicates() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string log="";
			//Check for duplicates while considering the orthochartrow table. This check will need to also consider the ProvNum since a user can insert more than 
			//one orthochart per day per provider.
			//The group by clause must use the keyword BINARY because the ortho chart within Open Dental is case sensitive.
			string command=@"
				SELECT GROUP_CONCAT(orthochart.OrthoChartNum) groupOrthoChartNums
				FROM orthochart
				INNER JOIN orthochartrow ON orthochartrow.OrthoChartRowNum=orthochart.OrthoChartRowNum
				GROUP BY orthochartrow.PatNum,orthochartrow.ProvNum,DATE(orthochartrow.DateTimeService),orthochart.OrthoChartRowNum,BINARY orthochart.FieldName,BINARY orthochart.FieldValue
				HAVING COUNT(*)>1";
			//This table shows the duplicate orthochartrows that should be considered.
			//The table will contain a group of all of the OrthoChartRowNums that each duplicate
			//orthochart contains. We will keep the Max(OrthoChartNum) and remove the rest. Signatures are still probably going to get invalidated.
			DataTable table=Db.GetTable(command);
			List<long> listOrthoChartNums=GetDuplicateOrthoChartNumsToDelete(table);
			if(listOrthoChartNums.Count==0) {
				log+=Lans.g("FormDatabaseMaintenance","Zero exact duplicate entries were found. No changes were made ");
				return log;
			}
			command=$@"DELETE FROM orthochart 
				WHERE OrthoChartNum IN({string.Join(",",listOrthoChartNums.Distinct())})";
			Db.NonQ(command);
			log+=Lans.g("FormDatabaseMaintenance","All exact duplicate entries have been removed ");
			return log;
		}

		///<summary>Loops through the DataTable and returns a list of OrthChartNums to be deleted. DataTable passed in must contain the column 'groupOrthoChartNums' which contain duplicate orthochartnums. </summary>
		private static List<long> GetDuplicateOrthoChartNumsToDelete(DataTable tableOrthoChartsDuplicates) {
			//No remoting role check; This is a private static method and those cannot be called from the middle tier.
			List<long> listOrthoChartNums=new List<long>();
			if(tableOrthoChartsDuplicates==null || tableOrthoChartsDuplicates.Rows.Count==0 || !tableOrthoChartsDuplicates.Columns.Contains("groupOrthoChartNums")) {
				return listOrthoChartNums;
			}
			//Loop through list and keep one. Select the MAX(OrthoChartNum). The rest will get added to a list of orthochartnums to delete
			for(int i=0;i<tableOrthoChartsDuplicates.Rows.Count;i++) {
				//list will contain a comma delimited string with all of the duplicate orthochartnums. Split them out and create a list of longs.
				List<long> listOrthoChartNumsDups=tableOrthoChartsDuplicates.Rows[i]["groupOrthoChartNums"].ToString()
					.Split(",",StringSplitOptions.RemoveEmptyEntries)
					.Select(x => PIn.Long(x)).ToList();
				//Select the max Orthochartnum. This is the one we will keep.
				long orthoChartNumToKeep=listOrthoChartNumsDups.Max();
				//Get the rest of the duplicate orthochartrows that will get deleted.
				listOrthoChartNums
					.AddRange(listOrthoChartNumsDups
						.FindAll(x => x!=orthoChartNumToKeep)
					);
			}
			return listOrthoChartNums;
		}

		//public static bool DatabaseIsOlderThanMarchSeventeenth(string olddb) {
		//  if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		//    return Meth.GetBool(MethodBase.GetCurrentMethod(),olddb);
		//  }
		//  command="SELECT COUNT(*) FROM "+olddb+".claimproc WHERE DateEntry > '2010-03-16'";
		//  if(Db.GetCount(command)=="0") {
		//    return true;
		//  }
		//  return false;
		//}

		/// <summary></summary>
		public static string FixClaimProcDeleteDuplicates() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string log="";
			//command=@"SELECT LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber, COUNT(*) cnt
			//	FROM claimproc
			//	LEFT JOIN patient ON patient.PatNum=claimproc.PatNum
			//	WHERE ClaimNum > 0
			//	AND ProcNum>0
			//	AND Status!=4/*exclude supplemental*/
			//	GROUP BY ClaimNum,ProcNum,Status,InsPayAmt,FeeBilled,LineNumber
			//	HAVING cnt>1";
			//table=Db.GetTable(command);
			//long numberFixed=0;
			//double insPayAmt;
			//double feeBilled;
			//for(int i=0;i<table.Rows.Count;i++) {
			//  insPayAmt=PIn.Double(table.Rows[i]["InsPayAmt"].ToString());
			//  feeBilled=PIn.Double(table.Rows[i]["FeeBilled"].ToString());
			//  command="DELETE FROM claimproc "
			//    +"WHERE ClaimNum= "+table.Rows[i]["ClaimNum"].ToString()+" "
			//    +"AND ProcNum= "+table.Rows[i]["ProcNum"].ToString()+" "
			//    +"AND Status= "+table.Rows[i]["Status"].ToString()+" "
			//    +"AND InsPayAmt= '"+POut.Double(insPayAmt)+"' "
			//    +"AND FeeBilled= '"+POut.Double(feeBilled)+"' "
			//    +"AND LineNumber= "+table.Rows[i]["LineNumber"].ToString()+" "
			//    +"AND ClaimProcNum != "+table.Rows[i]["ClaimProcNum"].ToString();
			//  numberFixed+=Db.NonQ(command);
			//}
			//log+="Claimprocs deleted due duplicate entries: "+numberFixed.ToString()+".\r\n";
			return log;
		}

		/// <summary></summary>
		public static string FixMissingClaimProcs(string olddb) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),olddb);
			}
			string log="";
			//command="SELECT LName,FName,patient.PatNum,ClaimNum,FeeBilled,Status,ProcNum,ProcDate,ClaimProcNum,InsPayAmt,LineNumber "
			//  +"FROM "+olddb+".claimproc "
			//  +"LEFT JOIN "+olddb+".patient ON "+olddb+".patient.PatNum="+olddb+".claimproc.PatNum "
			//  +"WHERE NOT EXISTS(SELECT * FROM claimproc WHERE claimproc.ClaimProcNum="+olddb+".claimproc.ClaimProcNum) "
			//  +"AND ClaimNum > 0 AND ProcNum>0";
			//table=Db.GetTable(command);
			//long numberFixed=0;
			//command="SELECT ValueString FROM "+olddb+".preference WHERE PrefName='DataBaseVersion'";
			//string oldVersString=Db.GetScalar(command);
			//Version oldVersion=new Version(oldVersString);
			//if(oldVersion < new Version("6.7.1.0")) {
			//  return "Version of old database is too old to use with the automated tool: "+oldVersString;
			//}
			//double insPayAmt;
			//double feeBilled;
			//for(int i=0;i<table.Rows.Count;i++) {
			//  insPayAmt=PIn.Double(table.Rows[i]["InsPayAmt"].ToString());
			//  feeBilled=PIn.Double(table.Rows[i]["FeeBilled"].ToString());
			//  command="SELECT COUNT(*) FROM "+olddb+".claimproc "
			//    +"WHERE ClaimNum= "+table.Rows[i]["ClaimNum"].ToString()+" "
			//    +"AND ProcNum= "+table.Rows[i]["ProcNum"].ToString()+" "
			//    +"AND Status= "+table.Rows[i]["Status"].ToString()+" "
			//    +"AND InsPayAmt= '"+POut.Double(insPayAmt)+"' "
			//    +"AND FeeBilled= '"+POut.Double(feeBilled)+"' "
			//    +"AND LineNumber= "+table.Rows[i]["LineNumber"].ToString();
			//  string result=Db.GetCount(command);
			//  if(result=="1") {//only include in result if there are duplicates in old db.
			//    continue;
			//  }
			//  command="INSERT INTO claimproc SELECT *";
			//  if(oldVersion < new Version("6.8.1.0")) {
			//    command+=",-1,-1,0";
			//  }
			//  else if(oldVersion < new Version("6.9.1.0")) {
			//    command+=",0";
			//  }
			//  command+=" FROM "+olddb+".claimproc "
			//    +"WHERE "+olddb+".claimproc.ClaimProcNum="+table.Rows[i]["ClaimProcNum"].ToString();
			//  numberFixed+=Db.NonQ(command);
			//}
			//command="SELECT ClaimPaymentNum "
			//  +"FROM "+olddb+".claimpayment "
			//  +"WHERE NOT EXISTS(SELECT * FROM claimpayment WHERE claimpayment.ClaimPaymentNum="+olddb+".claimpayment.ClaimPaymentNum) ";
			//table=Db.GetTable(command);
			//long numberFixed2=0;
			//for(int i=0;i<table.Rows.Count;i++) {
			//  command="INSERT INTO claimpayment SELECT * FROM "+olddb+".claimpayment "
			//    +"WHERE "+olddb+".claimpayment.ClaimPaymentNum="+table.Rows[i]["ClaimPaymentNum"].ToString();
			//  numberFixed2+=Db.NonQ(command);
			//}
			//log+="Missing claimprocs added back: "+numberFixed.ToString()+".\r\n";
			//log+="Missing claimpayments added back: "+numberFixed2.ToString()+".\r\n";
			return log;
		}

		///<summary>Removes unsupported unicode characters from appointment.ProcDescript, appointment.Note, and patient.AddrNote.
		///Also removes mysql null character ("\0" or CHAR(0)) from several columns from several tables.
		///These null characters were causing the middle tier deserialization to fail as they are not UTF-16 supported characters.
		///They are, however, allowed in UTF-8.</summary>
		public static void FixSpecialCharacters() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//helper method to find rows with invalid chars, and remove the invalid chars
			void findAndReplace<T>(string regexPattern,params string[] stringArrayColumnNames) {
				string table=CrudTableAttribute.GetTableName(typeof(T));
				string command="SELECT CONCAT("+string.Join(",",stringArrayColumnNames)+") " +
					"FROM "+table+" "+
					"WHERE ("+string.Join(") OR (",stringArrayColumnNames.Select(x => x+" REGEXP '"+regexPattern+"'"))+")";
				List<string> listStrings=Db.GetListString(command);
				List<char> listChars=getSpecialChars(listStrings);
				for(int i=0;i<listChars.Count;i++){
					command="UPDATE "+table+" SET "+string.Join(", ",stringArrayColumnNames.Select(x => x+"=REPLACE("+x+",'"+POut.String(listChars[i].ToString())+"','')"));
					Db.NonQ(command);
				}
			}
			//helper method to find "special characters"
			List<char> getSpecialChars(List<string> list) {
				List<char> listCharsSpecialFound=new List<char>();
				List<char> listChars=list.SelectMany(x=> x.ToCharArray()).ToList();
				for(int i=0;i<listChars.Count;i++){
					if((listChars[i]<126 && listChars[i]>31)//31 - 126 are all safe.
						|| listChars[i]==9      //"Horizontal Tabulation"
						|| listChars[i]==10     //Line Feed
						|| listChars[i]==13) {  //carriage return
						continue;
					}
					listCharsSpecialFound.Add(listChars[i]);
				}
				return listCharsSpecialFound.Distinct().ToList();
			}
			findAndReplace<Appointment>("[^[:alnum:]^[:space:]^[:punct:]]+",nameof(Appointment.ProcDescript),nameof(Appointment.Note));
			findAndReplace<Patient>  ("[^[:alnum:]^[:space:]]+",nameof(Patient.AddrNote));
			findAndReplace<Procedure>("[^[:alnum:]^[:space:]]+",nameof(Procedure.Surf));
			#region \0 Char
			for(int i = 0;i<ListTableAndColumns.Count;i+=2) {
				string tableName=ListTableAndColumns[i];
				string columnName=ListTableAndColumns[i+1];
				string command="UPDATE "+tableName+" "
					+"SET "+columnName+"=REPLACE("+columnName+",CHAR(0),'') "
					+"WHERE "+columnName+" LIKE '%"+POut.String("\0")+"%'";
				Db.NonQ(command);
			}
			#endregion
		}

		///<summary>Replaces null strings with empty strings and returns the number of rows changed.</summary>
		public static long MySqlRemoveNullStrings() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			//information_schema.COLUMNS does not have TABLE_TYPE so we must join information_schema.TABLES
			//and filter with DISTINCT and 'BASE TABLE' to exclude views.
			string tableSchema=POut.String(DataConnection.GetDatabaseName());
			string command=$@"SELECT DISTINCT information_schema.COLUMNS.TABLE_NAME,information_schema.COLUMNS.COLUMN_NAME 
				FROM information_schema.COLUMNS 
				INNER JOIN information_schema.TABLES
					ON information_schema.COLUMNS.TABLE_NAME=information_schema.TABLES.TABLE_NAME 
					AND information_schema.TABLES.TABLE_SCHEMA='{tableSchema}'
					AND information_schema.TABLES.TABLE_TYPE='BASE TABLE'
				WHERE information_schema.COLUMNS.TABLE_SCHEMA='{tableSchema}'
				AND DATA_TYPE IN ('char','longtext','mediumtext','text','varchar') 
				AND IS_NULLABLE='YES'";
			DataTable table=Db.GetTable(command);
			int countChanged=0;
			for(int i = 0;i<table.Rows.Count;i++) {
				command="UPDATE `"+table.Rows[i]["table_name"].ToString()+"` "
					+"SET `"+table.Rows[i]["column_name"].ToString()
					+"`='' WHERE `"+table.Rows[i]["column_name"].ToString()+"` IS NULL";
				countChanged+=(int)Db.NonQ(command);
			}
			return countChanged;
		}

		///<summary>Makes a backup of the database, clears out etransmessagetext entries over a year old, and then runs optimize on just the etransmessagetext table.  Customers were calling in with the complaint that their etransmessagetext table is too big so we added this tool.</summary>
		public static void ClearOldEtransMessageText() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//Make a backup of DB before we change anything, especially because we will be running optimize at the end.
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return; //Several issues need to be addressed before supporting Oracle.  E.g. backing up, creating temporary tables with globally unique identifiers, etc.
			}
			//Unlink etrans records from their etransmessagetext records if older than 1 year.
			//We want to keep the 835's around, because they are financial documents which the user may want to reference from the claim edit window later.
			string command="UPDATE etrans "
				+"SET EtransMessageTextNum=0 "
				+"WHERE DATE(DateTimeTrans)<ADDDATE(CURDATE(),INTERVAL -1 YEAR) AND Etype!="+POut.Long((int)EtransType.ERA_835);
			Db.NonQ(command);
			//Create a temporary table to hold all of the EtransMessageTextNum foreign keys which are sill in use within etrans.  The temporary table speeds up the next query.
			string tableName="tempetransnomessage"+MiscUtils.CreateRandomAlphaNumericString(8);//max size for a table name in oracle is 30 chars.
			command="DROP TABLE IF EXISTS "+tableName+"; "
				+"CREATE TABLE "+tableName+" "
				+"SELECT DISTINCT EtransMessageTextNum FROM etrans WHERE EtransMessageTextNum!=0; "
				+"ALTER TABLE "+tableName+" ADD INDEX (EtransMessageTextNum);";
			Db.NonQ(command);
			//Delete unlinked etransmessagetext entries.  Remember, multiple etrans records might point to a single etransmessagetext record.  Therefore, we must keep a particular etransmessagetext record if at least one etrans record needs it.
			command="DELETE FROM etransmessagetext "
				+"WHERE EtransMessageTextNum NOT IN (SELECT EtransMessageTextNum FROM "+tableName+");";
			Db.NonQ(command);
			//Remove the temporary table which is no longer needed.
			command="DROP TABLE "+tableName+";";
			Db.NonQ(command);
			//To reclaim that space on the disk you have to do an Optimize.
			//The reasons listed at [[Database Storage Engine Comparison: InnoDB vs MyISAM]] do not apply to these two tables.
			//We just did a massive delete and therefore optimzing a table which is not an "insert only table".
			//Optimizing etrans and etransmessagetext helped a large customer improve speeds by 100x.  They are using innodb tables.
			OptimizeTable("etransmessagetext",forceOptimize:true);
			OptimizeTable("etrans",forceOptimize:true);
		}

		///<summary>Return values look like 'MyISAM' or 'InnoDB'. Will return empty string on error.</summary>
		public static string GetStorageEngineDefaultName() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string retVal="";
			try {
				retVal=Db.GetScalar("SELECT @@default_storage_engine");//Mysql 5.5.3+
			}
			catch {
				//using SHOW GLOBAL VARIABLES will return an empty string if not supported.
				DataTable tableEngine=Db.GetTable("SHOW GLOBAL VARIABLES LIKE 'storage_engine'");//MySQL 5.5.2-
				if(tableEngine.Rows.Count>0) {
					retVal=PIn.String(tableEngine.Rows[0]["Value"].ToString());
				}
			}
			return retVal;
		}

		///<summary>Gets the number of tables in MyISAM format.</summary>
		public static int GetMyisamTableCount() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod());
			}
			string command="SELECT TABLE_NAME FROM INFORMATION_SCHEMA.tables "
				+"WHERE TABLE_SCHEMA='"+POut.String(DataConnection.GetDatabaseName())+"' "
				+"AND ENGINE LIKE 'MyISAM'";
			return Db.GetTable(command).Rows.Count;
		}

		public static DataTable GetTableEngineTableNames() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command=$@"SELECT TABLE_NAME,ENGINE FROM information_schema.TABLES
				WHERE TABLE_SCHEMA='{SOut.String(DataConnection.GetDatabaseName())}'
				AND TABLE_TYPE='BASE TABLE'";//only 'BASE TABLE' types to exclude views
			return Db.GetTable(command);
		}

		public static bool ConvertToDefaultEngine() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {//Does not apply to Oracle
				return true;
			}
			string defaultStorageEngine=GetStorageEngineDefaultName();
			//only 'BASE TABLE' types to exclude views, which have NULL for the ENGINE
			string commandTableNames=$@"SELECT TABLE_NAME FROM information_schema.TABLES
				WHERE TABLE_SCHEMA='{POut.String(DataConnection.GetDatabaseName())}'
				AND TABLE_TYPE='BASE TABLE'
				AND TABLE_NAME!='phone'
				AND ENGINE!='{defaultStorageEngine}'";
			List<string> listTableNames=Db.GetListString(commandTableNames);
			if(listTableNames.Count==0) {
				return true;
			}
			DataConnection.CommandTimeout=43200;//12 hours, because altering a large table may take longer to run.
			string alterCommand;
			for(int i=0;i<listTableNames.Count;i++) {
				alterCommand="ALTER TABLE `"+listTableNames[i]+"` ENGINE='"+defaultStorageEngine+"'";
				try {
					Db.NonQ(alterCommand);
				}
				catch(Exception ex) {
					ex.DoNothing();
					DataConnection.CommandTimeout=3600;//Set back to default of 1 hour.
					return false;
				}
			}
			DataConnection.CommandTimeout=3600;//Set back to default of 1 hour.
			listTableNames=Db.GetListString(commandTableNames);
			if(listTableNames.Count>0) {
				return false;
			}
			return true;
		}

		///<summary>Returns true if the conversion was successfull or no conversion was necessary. The goal is to convert InnoDB tables (excluding the 'phone' table) to MyISAM format when there are a mixture of InnoDB and MyISAM tables but no conversion will be performed when all of the tables are already in the same format.</summary>
		public static bool ConvertTablesToMyisam() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) { //Does not apply to Oracle.
				return true;
			}
			string command="SELECT TABLE_NAME,ENGINE FROM information_schema.TABLES "
				+"WHERE TABLE_SCHEMA='"+POut.String(DataConnection.GetDatabaseName())+"' "
				+"AND TABLE_TYPE='BASE TABLE' "//only 'BASE TABLE' types to exclude views
				+"AND TABLE_NAME!='phone'";//this table is used internally at OD HQ, and is always innodb.
			DataTable table=Db.GetTable(command);
			int numInnodb=0;//Or possibly some other format.
			int numMyisam=0;
			for(int i = 0;i<table.Rows.Count;i++) {
				if(PIn.String(table.Rows[i]["ENGINE"].ToString()).ToUpper()=="MYISAM") {
					numMyisam++;
					continue;
				}
				numInnodb++;
			}
			if(numInnodb>0 && numMyisam>0) {//Fix tables by converting them to MyISAM when there is a mixture of different table types.
				for(int i = 0;i<table.Rows.Count;i++) {
					if(PIn.String(table.Rows[i]["ENGINE"].ToString()).ToUpper()=="MYISAM") {
						continue;
					}
					string tableName=PIn.String(table.Rows[i]["TABLE_NAME"].ToString());
					command="ALTER TABLE `"+tableName+"` ENGINE='MyISAM'";
					try {
						Db.NonQ(command);
					}
					catch {
						return false;
					}
				}
				command="SELECT TABLE_NAME FROM information_schema.TABLES "
					+"WHERE TABLE_SCHEMA='"+POut.String(DataConnection.GetDatabaseName())+"' "
					+"AND TABLE_TYPE='BASE TABLE' "//only 'BASE TABLE' types to exclude views
					+"AND TABLE_NAME!='phone' "
					+"AND ENGINE NOT LIKE 'MyISAM'";
				if(Db.GetTable(command).Rows.Count!=0) { //If any tables are still InnoDB.
					return false;
				}
			}
			return true;
		}

		///<summary>Returns the number of invalid FKey entries for specified tableName, permissions, and primary key column.
		///You MUST check remoting role before calling this method.  It is purposefully private and must remain so.</summary>
		private static long GetCountForSecuritylogInvalidFKeys(string permsCommaDelimStr,string tableName,string priKeyColumnName) {
			//No remoting role check; This is a private static method and those cannot be called from the middle tier.
			string command="SELECT COUNT(securitylog.SecurityLogNum) "
					+"FROM securitylog "
					+"WHERE securitylog.PermType IN ("+POut.String(permsCommaDelimStr)+") "
					+"AND securitylog.FKey!=0 "
					+"AND NOT EXISTS ( "
						+"SELECT "+tableName+"."+priKeyColumnName+" "
						+"FROM "+tableName+" "
						+"WHERE "+tableName+"."+priKeyColumnName+"=securitylog.FKey "
					+")";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Fixes orphaned FKey entries for specific tableName, permissions, and primary key column.
		///Returns number of rows fixed.
		///You MUST check remoting role before calling this method.  It is purposefully private and must remain so.</summary>
		private static long UpdateOrphanedSecuritylogInvalidKeys(string permsCommaDelimStr,string tableName,string priKeyColumnName) {
			string command="UPDATE securitylog SET FKey=0 "
					+"WHERE securitylog.PermType IN ("+POut.String(permsCommaDelimStr)+") "
					+"AND securitylog.FKey!=0 "
					+"AND NOT EXISTS ( "
						+"SELECT "+tableName+"."+priKeyColumnName+" "
						+"FROM "+tableName+" "
						+"WHERE "+tableName+"."+priKeyColumnName+"=securitylog.FKey "
					+")";
			return Db.NonQ(command);
		}

		///<summary>Used to estimate the time that CreateMissingActiveTPs will take to run.</summary>
		public static List<Procedure> GetProcsNoActiveTp() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Procedure>>(MethodBase.GetCurrentMethod());
			}
			//pats with TP'd procs and no active treatplan OR pats with TPi'd procs that are attached to a sched or planned appt and no active treatplan
			string command="SELECT * FROM procedurelog WHERE (ProcStatus="+(int)ProcStat.TP+" "//TP proc exists
				+"OR (ProcStatus="+(int)ProcStat.TPi+" AND (AptNum>0 OR PlannedAptNum>0))) "//TPi proc exists that is attached to a sched or planned appt
				+"AND PatNum NOT IN(SELECT PatNum FROM treatplan WHERE TPStatus="+(int)TreatPlanStatus.Active+")";//no active treatplan
			return Crud.ProcedureCrud.SelectMany(command);
		}
		
		public static string CreateMissingActiveTPs(List<Procedure> listProceduresTp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),listProceduresTp);
			}
			if(listProceduresTp.Count==0) {//should never happen, won't get called if the list is empty, but just in case
				return "";
			}
			listProceduresTp=listProceduresTp.OrderBy(x => x.PatNum).ToList();//code below relies of patients being grouped.
																																		//listTpTpiProcs.Sort((x,y) => { return x.PatNum.CompareTo(y.PatNum); });//possibly more efficient
			TreatPlan treatPlanActive=null;
			long patNumCur=0;
			//listProcsNoTp is ordered by PatNum, so each time we find a new PatNum we will create a new active plan and attach procs to it
			//until we find the next PatNum
			for(int i=0;i<listProceduresTp.Count;i++) {
				if(listProceduresTp[i].PatNum!=patNumCur) {//new patient, create active plan
					treatPlanActive=new TreatPlan();//create active plan, all patients in listPatNumsNoTp do not have an active plan
					treatPlanActive.Heading=Lans.g("TreatPlans","Active Treatment Plan");
					treatPlanActive.Note=PrefC.GetString(PrefName.TreatmentPlanNote);
					treatPlanActive.TPStatus=TreatPlanStatus.Active;
					treatPlanActive.PatNum=listProceduresTp[i].PatNum;
					//UserNumPresenter=userNum,
					//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
					treatPlanActive.SecUserNumEntry=Security.CurUser.UserNum;
					treatPlanActive.TPType=(DiscountPlanSubs.HasDiscountPlan(listProceduresTp[i].PatNum) ? TreatPlanType.Discount : TreatPlanType.Insurance);
					treatPlanActive.TreatPlanNum=TreatPlans.Insert(treatPlanActive);
					patNumCur=listProceduresTp[i].PatNum;
				}
				TreatPlanAttach treatPlanAttach=new TreatPlanAttach();
				treatPlanAttach.ProcNum=listProceduresTp[i].ProcNum;
				treatPlanAttach.TreatPlanNum=treatPlanActive.TreatPlanNum;
				treatPlanAttach.Priority=listProceduresTp[i].Priority;
				TreatPlanAttaches.Insert(treatPlanAttach);
			}
			return "Patients with active treatment plans created: "+listProceduresTp.Select(x => x.PatNum).Distinct().ToList().Count;
		}

		///<summary>This method is designed to help save hard drive space due to the RawEmailIn column containing Base64 attachments. This method
		///should be run on a separate thread and this thread should be passed in to update the progress window.</summary>
		public static string CleanUpRawEmails() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			//Get all clear text emailmessages that can have their RawEmailIn columns safely manipulated from the inbox.
			//These emails are safe to remove attachments from the RawEmailIn because they have already been digested and attchments extracted.
			string command="SELECT EmailMessageNum FROM emailmessage "
				+"WHERE RawEmailIn!='' "
				+"AND SentOrReceived IN ("+POut.Int((int)EmailSentOrReceived.Received)+","+POut.Int((int)EmailSentOrReceived.Read)+")";
			//POut.Int((int)EmailSentOrReceived.ReceivedDirect)+","+POut.Int((int)EmailSentOrReceived.ReadDirect)
			//We might need to include encrypted emails in the future if the email table is large due to encrypted emails.
			//Currently not including encrypted emails because the computer running this tool would need the private key to decrypt the message and
			//we would need to take an extra step at the end (after cleaning up attachments) to re-encrypt the modified email message. 
			//The current customers complaining only have bloat with clear text emails so that is where we are going to start with the clean up tool.
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Getting email messages from the database..."));
			DataTable tableEmailMessageNums=Db.GetTable(command);
			if(tableEmailMessageNums.Rows.Count==0) {
				return Lans.g("DatabaseMaintenance","There are no email messages that need to be cleaned up.");
			}
			List<EmailAddress> listEmailAddresses=EmailAddresses.GetAll();//Do not use the cache because the cache doesn't contain all email addresses.
			int countNoChange=0;
			int countError=0;
			int countCleaned=0;
			//Call the processing email logic for each email which will clear out the RawEmailIn column if the email is successfully digested.
			for(int i=0;i<tableEmailMessageNums.Rows.Count;i++) {
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Processing email message")
					+"  "+(i+1).ToString()+" / "+tableEmailMessageNums.Rows.Count.ToString());
				EmailMessage emailMessage=EmailMessages.GetOne(PIn.Long(tableEmailMessageNums.Rows[i]["EmailMessageNum"].ToString()));
				EmailMessage emailMessageOld=emailMessage.Copy();
				//Try and find the corresponding email address for this email.
				EmailAddress emailAddress=listEmailAddresses.FirstOrDefault(x => x.EmailUsername.ToLower()==emailMessage.RecipientAddress.ToLower());
				if(emailAddress==null) {
					countError++;
					continue;
				}
				try {
					EmailMessage emailMessageNew=EmailMessages.ProcessRawEmailMessageIn(emailMessage.RawEmailIn,emailMessage.EmailMessageNum
						,emailAddress,false,emailMessageOld.SentOrReceived);
					emailMessageNew.FailReason=emailMessage.FailReason;// not copied over in ProcessRawEmailMessageIn()
					if(emailMessageNew.RawEmailIn!=emailMessage.RawEmailIn && emailMessageNew.RawEmailIn.EndsWith("\r\n")) {
						//In Health.Direct.Agent.IncomingMessage.SerializeMessage(), a trailing newline is sometimes added.
						emailMessageNew.RawEmailIn=emailMessageNew.RawEmailIn.Substring(0,emailMessageNew.RawEmailIn.Length-2);
					}
					if(Crud.EmailMessageCrud.UpdateComparison(emailMessageNew,emailMessageOld)) {
						countCleaned++;
					}
					else {//No changes.
						countNoChange++;
					}
				}
				catch(Exception) {
					//Nothing to do, don't worry about it.
					countError++;
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql && tableEmailMessageNums.Rows.Count!=countNoChange) {//Using MySQL and something actually changed.
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Optimizing the email message table..."));
				OptimizeTable("emailmessage");
			}
			string strResults=Lans.g("DatabaseMaintenance","Done.  No clean up required.");
			if(countCleaned > 0 || countError > 0) {
				strResults=Lans.g("DatabaseMaintenance","Total email messages considered")+": "+tableEmailMessageNums.Rows.Count.ToString()+"\r\n"
					+Lans.g("DatabaseMaintenance","Email messages successfully cleaned up")+": "+countCleaned.ToString()+"\r\n"
					+Lans.g("DatabaseMaintenance","Email messages that did not need to be cleaned up")+": "+countNoChange.ToString()+"\r\n"
					+Lans.g("DatabaseMaintenance","Email messages that failed to be cleaned up")+": "+countError.ToString();
			}
			return strResults;
		}

		///<summary>This method will move any email attachment files from the root EmailAttachments directory that start with 'In_' or 'Out_'
		///and will move them into their corresponding In or Out sub directories.</summary>
		public static string CleanUpAttachmentsRootDirectory() {
			StringBuilder stringBuilder=new StringBuilder();
			char charSeparator=CloudStorage.DirectorySeparatorChar;
			string attachPath=EmailAttaches.GetAttachPath();
			List<string> listFileNames=FileAtoZ.GetFilesInDirectory(attachPath);
			stringBuilder.AppendLine($"Total files in folder '{attachPath}': {listFileNames.Count}");
			List<string> listFileNamesIn=listFileNames.FindAll(x => x.Split(charSeparator).Last().StartsWith("In_"));
			stringBuilder.AppendLine($"Total files starting with 'In_': {listFileNamesIn.Count}");
			List<string> listFileNamesOut=listFileNames.FindAll(x => x.Split(charSeparator).Last().StartsWith("Out_"));
			stringBuilder.AppendLine($"Total files starting with 'Out_': {listFileNamesOut.Count}");
			try {
				if(!FileAtoZ.DirectoryExists(FileAtoZ.CombinePaths(attachPath,"In"))) {
					FileAtoZ.CreateDirectory(FileAtoZ.CombinePaths(attachPath,"In"));
				}
				if(!FileAtoZ.DirectoryExists(FileAtoZ.CombinePaths(attachPath,"Out"))) {
					FileAtoZ.CreateDirectory(FileAtoZ.CombinePaths(attachPath,"Out"));
				}
			}
			catch(Exception ex) {
				stringBuilder.Append("There was an error cleaning up email attachments:\r\n"+ex.Message);
				return stringBuilder.ToString().Trim();
			}
			int countMoved=0;
			int countErrors=0;
			for(int i=0;i<listFileNamesIn.Count;i++){
				string fileNameNew=listFileNamesIn[i].Replace(charSeparator+"In_",charSeparator+"In"+charSeparator);
				try {
					FileAtoZ.Move(listFileNamesIn[i],fileNameNew);
				}
				catch(Exception ex) {
					stringBuilder.AppendLine("  Error moving "+listFileNamesIn[i]+": "+ex.Message);
					countErrors++;
					continue;
				}
				countMoved++;
			}
			for(int i=0;i<listFileNamesOut.Count;i++){
				string fileNameNew=listFileNamesOut[i].Replace(charSeparator+"Out_",charSeparator+"Out"+charSeparator);
				try {
					FileAtoZ.Move(listFileNamesOut[i],fileNameNew);
				}
				catch(Exception ex) {
					stringBuilder.AppendLine("  Error moving "+listFileNamesOut[i]+": "+ex.Message);
					countErrors++;
					continue;
				}
				countMoved++;
			}
			stringBuilder.AppendLine("Total files successfully moved: "+countMoved);
			if(countErrors > 0) {
				stringBuilder.AppendLine("Total errors: "+countErrors);
				stringBuilder.AppendLine("    Please fix the above errors and try again or call support.");
			}
			return stringBuilder.ToString().Trim();
		}

		///<summary>Similar to InsPlans.ComputeEstimatesForPatNums(...)</summary>
		public static void RecalcEstimates(List<Procedure> listProcedures) {
			List<long> listPatNums=listProcedures.Select(x => x.PatNum).Distinct().ToList();
			//No need to check MiddleTierRole; no call to db.
			for(int i = 0;i<listPatNums.Count;i++) {
				long patNum=listPatNums[i];
				Family family=Patients.GetFamily(patNum);
				Patient patient=family.GetPatient(patNum);
				//Only grab the procedures that have not been completed yet.
				List<Procedure> listProceduresNonCompleted=listProcedures.FindAll(x => x.PatNum==patNum);
				List<ClaimProc> listClaimProcs=ClaimProcs.GetForProcs(listProceduresNonCompleted.Select(x => x.ProcNum).ToList());
				//Only use the claim procs associated to the non-completed procedures.
				List<ClaimProc> listClaimProcsNonCompleted=listClaimProcs.FindAll(x => listProceduresNonCompleted.Exists(y => y.ProcNum==x.ProcNum));
				List<InsSub> listInsSubs=InsSubs.RefreshForFam(family);
				List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
				List<PatPlan> listPatPlans=PatPlans.Refresh(patNum);
				List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
				Procedures.ComputeEstimatesForAll(patNum,listClaimProcsNonCompleted,listProceduresNonCompleted,listInsPlans,listPatPlans,listBenefits,patient.Age,listInsSubs,null,true);
				Patients.SetHasIns(patNum);
			}
		}

		///<summary>Detaches all patient payments attached to insurance payment plans and all insurance payments attached to patient payment plans.
		///Returns a description of the changes that were made so that the user can go make manual changes if necessary.</summary>
		public static string DetachInvalidPaymentPlanPayments() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			string resultsMsg="";
			DataTable table=GetPaySplitsAttachedToInsurancePaymentPlan();
			if(table.Rows.Count > 0) {
				string command="UPDATE paysplit SET PayPlanNum=0 WHERE SplitNum IN("
					+string.Join(",",table.Select().Select(x => x["SplitNum"].ToString()))+")";
				Db.NonQ(command);
				resultsMsg+=Lans.g(_lanThis,"The following patient payments were detached from insurance payment plans.  It is recommended you verify "
					+"these accounts are correct.");
				for(int i=0;i<table.Rows.Count;i++) {
					resultsMsg+="\r\n   "+Lans.g(_lanThis,"Patient #")+" "+table.Rows[i]["PatNum"].ToString()+" "
						+Lans.g(_lanThis,"had a payment amount for")+" "+PIn.Double(table.Rows[i]["SplitAmt"].ToString()).ToString("c")+" "
						+Lans.g(_lanThis,"on date")+" "+PIn.Date(table.Rows[i]["DatePay"].ToString()).ToShortDateString()+" "
						+Lans.g(_lanThis,"attached to insurance payment plan #")+table.Rows[i]["PayPlanNum"];
				}
			}
			table=GetClaimProcsAttachedToPatientPaymentPlans();
			if(table.Rows.Count > 0) {
				string command="UPDATE claimproc SET PayPlanNum=0 WHERE ClaimProcNum IN("
					+string.Join(",",table.Select().Select(x => x["ClaimProcNum"].ToString()))+")";
				Db.NonQ(command);
				if(resultsMsg!="") {
					resultsMsg+="\r\n\r\n";
				}
				resultsMsg+=Lans.g(_lanThis,"The following insurance payments were detached from patient payment plans.  It is recommended you verify "
					+"these accounts are correct.");
				for(int i = 0;i<table.Rows.Count;i++) {
					resultsMsg+="\r\n   "+Lans.g(_lanThis,"Patient #")+table.Rows[i]["PatNum"].ToString()+" "
						+Lans.g(_lanThis,"had a payment amount for")+" "+PIn.Double(table.Rows[i]["InsPayAmt"].ToString()).ToString("c")+" "
						+Lans.g(_lanThis,"on date")+" "+PIn.Date(table.Rows[i]["DateCP"].ToString()).ToShortDateString()+" "
						+Lans.g(_lanThis,"attached to patient payment plan #")+table.Rows[i]["PayPlanNum"];
				}
			}
			if(resultsMsg=="") {
				resultsMsg+=Lans.g(_lanThis,"No payments found that needed to be detached from payment plans.");
			}
			return resultsMsg;
		}

		///<summary>Gets the DataTable that contains paysplits attached to insurance payment plans.
		///Table will contain the following columns; SplitNum, PatNum, SplitAmt, DatePay, PayPlanNum</summary>
		private static DataTable GetPaySplitsAttachedToInsurancePaymentPlan() {
			//Need to check remoting role before calling; private method
			string command="SELECT paysplit.SplitNum,paysplit.PatNum,paysplit.SplitAmt,paysplit.DatePay,paysplit.PayPlanNum FROM paysplit "
				+"INNER JOIN payplan ON payplan.PayPlanNum=paysplit.PayPlanNum "
				+"WHERE paysplit.PayPlanNum!=0 "
				+"AND payplan.PlanNum!=0 ";//insurance payment plan
			return Db.GetTable(command);
		}

		///<summary>Gets claim procs that are attached to patient payment plans.
		///Table will contain the following columns; ClaimProcNum, PatNum, InsPayAmt, DateCP, PayPlanNum</summary>
		private static DataTable GetClaimProcsAttachedToPatientPaymentPlans() {
			//Need to check remoting role before calling; private method
			string command="SELECT claimproc.ClaimProcNum,claimproc.PatNum,claimproc.InsPayAmt,claimproc.DateCP,claimproc.PayPlanNum FROM claimproc "
				+"INNER JOIN payplan ON payplan.PayPlanNum=claimproc.PayPlanNum "
				+"WHERE claimproc.PayPlanNum!=0 "
				+"AND payplan.PlanNum=0 ";//standard payment plan
			return Db.GetTable(command);
		}

		///<summary>Given a patnum and the name of a table this helper builds the MySQL AND clause string used for our patient specific DBMs. 
		///Currently only works if the column name on the table is "PatNum" and will return an empty string if the PatNum is less than 1.</summary>
		private static string PatientAndClauseHelper(long patNum,string tableName) {
			//Not running patient specific DBM or a table wasn't specified.
			if(patNum<1 || string.IsNullOrWhiteSpace(tableName)) {
				return "";
			}
			return " AND "+tableName+".PatNum="+POut.Long(patNum)+" ";
		}

		public static DataTable GetRedundantIndexesTable() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string dbName=MiscData.GetCurrentDatabase();
			string command=$@"SELECT table1.TABLE_NAME,
				REPLACE(
					CASE WHEN table1.COLS=table2.COLS AND table1.NON_UNIQUE=table2.NON_UNIQUE
						THEN
							CASE WHEN INSTR(REPLACE(table2.INDEX_NAME,'`',''),REPLACE(table1.INDEX_NAME,'`',''))=1
								THEN table2.INDEX_NAME
							WHEN INSTR(REPLACE(table1.INDEX_NAME,'`',''),REPLACE(table2.INDEX_NAME,'`',''))=1
								THEN table1.INDEX_NAME
							ELSE GREATEST(table1.INDEX_NAME,table2.INDEX_NAME)
							END
					WHEN LENGTH(table1.COLS)-LENGTH(REPLACE(table1.COLS,',',''))>LENGTH(table2.COLS)-LENGTH(REPLACE(table2.COLS,',',''))
						THEN table2.INDEX_NAME
					ELSE table1.INDEX_NAME
					END
				,'`','') INDEX_NAME,
				REPLACE(
					CASE WHEN table1.COLS=table2.COLS AND table1.NON_UNIQUE=table2.NON_UNIQUE
						THEN
							CASE WHEN INSTR(REPLACE(table2.INDEX_NAME,'`',''),REPLACE(table1.INDEX_NAME,'`',''))=1
								THEN CONCAT(table2.COLS,IFNULL(CONCAT('(',table2.SUB_PART,')'),''))
							WHEN INSTR(REPLACE(table1.INDEX_NAME,'`',''),REPLACE(table2.INDEX_NAME,'`',''))=1
								THEN CONCAT(table1.COLS,IFNULL(CONCAT('(',table1.SUB_PART,')'),''))
							ELSE
								CASE WHEN table1.INDEX_NAME>table2.INDEX_NAME
									THEN CONCAT(table1.COLS,IFNULL(CONCAT('(',table1.SUB_PART,')'),''))
								ELSE CONCAT(table2.COLS,IFNULL(CONCAT('(',table2.SUB_PART,')'),''))
								END
							END
					WHEN LENGTH(table1.COLS)-LENGTH(REPLACE(table1.COLS,',',''))>LENGTH(table2.COLS)-LENGTH(REPLACE(table2.COLS,',',''))
						THEN CONCAT(table2.COLS,IFNULL(CONCAT('(',table2.SUB_PART,')'),''))
					ELSE CONCAT(table1.COLS,IFNULL(CONCAT('(',table1.SUB_PART,')'),''))
					END
				,'`','') INDEX_COLS,
				REPLACE(
					GROUP_CONCAT(
						DISTINCT CASE WHEN table1.COLS=table2.COLS AND table1.NON_UNIQUE=table2.NON_UNIQUE
							THEN
								CASE WHEN INSTR(REPLACE(table2.INDEX_NAME,'`',''),REPLACE(table1.INDEX_NAME,'`',''))=1
									THEN CONCAT(table1.INDEX_NAME,' (',table1.COLS,')')
								WHEN INSTR(REPLACE(table1.INDEX_NAME,'`',''),REPLACE(table2.INDEX_NAME,'`',''))=1
									THEN CONCAT(table2.INDEX_NAME,' (',table2.COLS,')')
								ELSE
									CASE WHEN table1.INDEX_NAME<table2.INDEX_NAME
										THEN CONCAT(table1.INDEX_NAME,' (',table1.COLS,')')
									ELSE CONCAT(table2.INDEX_NAME,' (',table2.COLS,')')
									END
								END
						WHEN LENGTH(table1.COLS)-LENGTH(REPLACE(table1.COLS,',',''))>LENGTH(table2.COLS)-LENGTH(REPLACE(table2.COLS,',',''))
							THEN CONCAT(table1.INDEX_NAME,' (',table1.COLS,')')
						ELSE CONCAT(table2.INDEX_NAME,' (',table2.COLS,')')
						END
						SEPARATOR '\r\n'
					)
				,'`','') REDUNDANT_OF,
				CASE WHEN table1.COLS=table2.COLS AND table1.NON_UNIQUE=table2.NON_UNIQUE
					THEN
						CASE WHEN INSTR(REPLACE(table2.INDEX_NAME,'`',''),REPLACE(table1.INDEX_NAME,'`',''))=1
							THEN table2.ENGINE
						WHEN INSTR(REPLACE(table1.INDEX_NAME,'`',''),REPLACE(table2.INDEX_NAME,'`',''))=1
							THEN table1.ENGINE
						ELSE
							CASE WHEN table1.INDEX_NAME>table2.INDEX_NAME
								THEN table1.ENGINE
							ELSE table2.ENGINE
							END
						END
				WHEN LENGTH(table1.COLS)-LENGTH(REPLACE(table1.COLS,',',''))>LENGTH(table2.COLS)-LENGTH(REPLACE(table2.COLS,',',''))
					THEN table2.ENGINE
				ELSE table1.ENGINE
				END `ENGINE`
				FROM (
					SELECT s.TABLE_NAME,CONCAT('`',s.INDEX_NAME,'`') AS INDEX_NAME,s.INDEX_TYPE,s.NON_UNIQUE,s.SUB_PART,t.ENGINE,
					GROUP_CONCAT(CONCAT('`',s.COLUMN_NAME,'`') ORDER BY IF(s.INDEX_TYPE='BTREE',s.SEQ_IN_INDEX,0),s.COLUMN_NAME) COLS
					FROM information_schema.STATISTICS s
					INNER JOIN information_schema.TABLES t ON t.TABLE_SCHEMA=s.TABLE_SCHEMA
						AND t.TABLE_NAME=s.TABLE_NAME
					WHERE s.TABLE_SCHEMA='{POut.String(dbName)}'
					GROUP BY s.TABLE_NAME,s.INDEX_NAME,s.INDEX_TYPE,s.NON_UNIQUE
				) table1
				INNER JOIN (
					SELECT s.TABLE_NAME,CONCAT('`',s.INDEX_NAME,'`') AS INDEX_NAME,s.INDEX_TYPE,s.NON_UNIQUE,s.SUB_PART,t.ENGINE,
					GROUP_CONCAT(CONCAT('`',s.COLUMN_NAME,'`') ORDER BY IF(s.INDEX_TYPE='BTREE',s.SEQ_IN_INDEX,0),s.COLUMN_NAME) COLS
					FROM information_schema.STATISTICS s
					INNER JOIN information_schema.TABLES t ON t.TABLE_SCHEMA=s.TABLE_SCHEMA
						AND t.TABLE_NAME=s.TABLE_NAME
					WHERE s.TABLE_SCHEMA='{POut.String(dbName)}'
					GROUP BY s.TABLE_NAME,s.INDEX_NAME,s.INDEX_TYPE,s.NON_UNIQUE
				) AS table2
				WHERE table2.TABLE_NAME=table1.TABLE_NAME
				AND table2.INDEX_NAME!=table1.INDEX_NAME
				AND table2.INDEX_TYPE=table1.INDEX_TYPE
				AND (
					(
						table2.COLS=table1.COLS
						AND (
							table1.NON_UNIQUE
							OR table1.NON_UNIQUE=table2.NON_UNIQUE
						)
					)
					OR (
						table1.INDEX_TYPE='BTREE'
						AND INSTR(table2.COLS,table1.COLS)=1
						AND table1.NON_UNIQUE
					)
				)
				GROUP BY table1.TABLE_NAME,INDEX_NAME";
			return Db.GetTable(command);
		}

		public static string DropRedundantIndexes(List<DataRow> listDataRows) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),listDataRows);
			}
			//Incoming DataRows have these columns: "TABLE_NAME","INDEX_NAME","INDEX_COLS","REDUNDANT_OF"
			bool hasInnoDbFilePerTable=false;
			using(DataTable table=Db.GetTable("SHOW GLOBAL VARIABLES LIKE 'INNODB_FILE_PER_TABLE'")) {
				if(table.Rows.Count>0 && table.Columns.Count>1) {
					hasInnoDbFilePerTable=PIn.Bool(table.Rows[0][1].ToString());
				}
			}
			StringBuilder stringBuilderLog=new StringBuilder();
			string dbName=MiscData.GetCurrentDatabase();
			DataConnection.CommandTimeout=43200;//12 hours, just in case
			List<string> listTableNamesDist=listDataRows.Select(x => x["TABLE_NAME"].ToString()).Distinct().ToList();
			for(int i=0;i<listTableNamesDist.Count;i++){
				string fullTableName="`"+POut.String(dbName)+"`.`"+POut.String(listTableNamesDist[i])+"`";
				List<DataRow> listDataRowsMatchTableName=listDataRows.FindAll(x => x["TABLE_NAME"].ToString()==listTableNamesDist[i]);
				if(listDataRowsMatchTableName.Count==0){
					continue;
				}
				stringBuilderLog.AppendLine("ALTER TABLE "+fullTableName+" "+string.Join(", ",
					listDataRowsMatchTableName.Select(x => "ADD INDEX "+POut.String(x["INDEX_NAME"].ToString())+" ("+POut.String(x["INDEX_COLS"].ToString())+")"))+";");
				string command="ALTER TABLE "+fullTableName+" "+string.Join(", ",listDataRowsMatchTableName.Select(x => "DROP INDEX "+POut.String(x["INDEX_NAME"].ToString())))+";";
				//The ENGINE column should be the same for all rows in the list, since it's the table's storage engine. Using .Exists just in case.
				bool doOptimize=listDataRowsMatchTableName.Exists(x => x["ENGINE"].ToString().ToLower()=="innodb") && hasInnoDbFilePerTable;
				string optimize="";
				if(doOptimize) {
					//For InnoDb tables with innodb_file_per_table set, optimize table to reclaim hard drive space and reduce .ibd file size
					command+=@"
						OPTIMIZE TABLE "+fullTableName+";";
					optimize=Lans.g("DatabaseMaintenance","and optimizing")+" ";
				}
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Dropping redundant indexes")+" "
					+(optimize)+Lans.g("DatabaseMaintenance","table")+" "
					+fullTableName.Replace("`","")+".");
				Db.NonQ(command);
			}
			DataConnection.CommandTimeout=3600;//set back to 1 hour default
			return stringBuilderLog.ToString();
		}

		///<summary>This method will remove all duplicate email messages and duplicate email message uids.</summary>
		public static string CleanUpDuplicateEmails() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			//Find and delete all duplicate MsgIds within emailmessageuid.
			string command=$@"SET group_concat_max_len=4294967295;
				SELECT GROUP_CONCAT(EmailMessageUidNum) msgUidNums
				FROM emailmessageuid
				GROUP BY BINARY MsgId
				HAVING COUNT(*)>1";
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Getting duplicate email message uids from the database..."));
			List<string> listMsgUidNumsDup=Db.GetListString(command);
			if(listMsgUidNumsDup.Count>0) {
				for(int i=0;i<listMsgUidNumsDup.Count;i++) {
					try {
						List<string> listMsgUidNums=listMsgUidNumsDup[i].Split(",",StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();//Keep one of the EmailMessageUidNums
						if(listMsgUidNums.Count==0) {
							continue;//shouldn't happen
						}
						ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Deleting duplicate email message uids from the database..."));
						Db.NonQ($"DELETE FROM emailmessageuid WHERE EmailMessageUidNum IN({string.Join(",",listMsgUidNums)})");
					}
					catch(Exception ex) {
						ex.DoNothing();
						continue; //Skip any failures and continue
					}
				}
			}
			//Find and remove all duplicate email messages.
			command=$@"SET group_concat_max_len=4294967295;
				SELECT GROUP_CONCAT(EmailMessageNum ORDER BY SentOrReceived DESC) msgNums
				FROM emailmessage
				GROUP BY BINARY SUBJECT,RecipientAddress,ToAddress,FromAddress,CcAddress,BccAddress,BINARY BodyText,MsgDateTime
				HAVING COUNT(*)>1";
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Getting duplicate email messages from the database..."));
			List<string> listMsgNumsDup=Db.GetListString(command);
			if(listMsgNumsDup.Count==0) {
				return Lans.g("DatabaseMaintenance","There are no duplicate emails that need to be cleaned up.");
			}
			for(int i=0;i<listMsgNumsDup.Count;i++) {
				try {
					List<string> listMsgNums=listMsgNumsDup[i].Split(",",StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();//Keep one of the EmailMessageNums
					if(listMsgNums.Count==0) {
						continue;
					}
					ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Deleting duplicate email messages from the database..."));
					Db.NonQ($"DELETE FROM emailmessage WHERE EmailMessageNum IN({string.Join(",",listMsgNums)})");
				}
				catch(Exception ex) {
					ex.DoNothing();
					continue; //Skip any failures and continue
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {//Using MySQL.
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Optimizing the email message and email message uid tables..."));
				OptimizeTable("emailmessageuid");
				OptimizeTable("emailmessage");
			}
			string strResults=Lans.g("DatabaseMaintenance","Done.  No clean up required.");
			if(listMsgUidNumsDup.Count>0 || listMsgNumsDup.Count>0) {
				strResults=Lans.g("DatabaseMaintenance","Total duplicate email message uids deleted")+": "+listMsgUidNumsDup.Count.ToString()+"\r\n"
					+Lans.g("DatabaseMaintenance","Total duplicate email messages deleted")+": "+listMsgNumsDup.Count.ToString();
			}
			return strResults;
		}

		///<summary>This method will go through the emailattach table to find and delete all the attachments not linked to an EmailMessage. 
		///This also includes going into the OpenDentImages folder where the attachments are stored and delete the files there as well.</summary>
		public static string CleanUpUnattachedAttachments() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod());
			}
			//We will not effect cloud storage. Only if the files are on the computer.
			if(CloudStorage.IsCloudStorage || PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				return Lans.g("DatabaseMaintenance","This tool will not remove attachments using cloud storage or AtoZ folders."); 
			}
			string command="SELECT ActualFileName FROM emailattach WHERE EmailMessageNum NOT IN (SELECT EmailMessageNum FROM emailmessage)";
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Getting attachments not linked to an email message from the database..."));
			List<string> listAttachmentFilesToDelete=Db.GetListString(command);
			if(listAttachmentFilesToDelete.Count==0) {
				return Lans.g("DatabaseMaintenance","There are no attachments that need to be deleted.");
			}
			//Safe to use this method since already checked if using cloud storage.
			string attachPath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"EmailAttachments");
			int countErrors=0;
			int countDeleted=0;
			//Delete all the attachment files within the images folder that aren't linked to a email message.
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Deleting unlinked attachments in the EmailAttachments folder..."));
			for(int i=0;i<listAttachmentFilesToDelete.Count;i++) {
				try {
					string attachFilePath=FileAtoZ.CombinePaths(attachPath,listAttachmentFilesToDelete[i]);
					if(FileAtoZ.Exists(attachFilePath)) {
						FileAtoZ.Delete(attachFilePath);
						countDeleted++;
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
					countErrors++;
				}
			}
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Deleting unlinked email attaches from the database..."));
			command="DELETE FROM emailattach WHERE EmailMessageNum NOT IN (SELECT EmailMessageNum FROM emailmessage)";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {//Using MySQL.
				ODEvent.Fire(ODEventType.ProgressBar,Lans.g("DatabaseMaintenance","Optimizing the email attach table..."));
				OptimizeTable("emailattach");
			}
			string strResults=Lans.g("DatabaseMaintenance","Done.  No clean up required.");
			if(countDeleted>0 || countErrors>0) {
				strResults=Lans.g("DatabaseMaintenance","Total attachments considered")+": "+listAttachmentFilesToDelete.Count.ToString()+"\r\n"
					+Lans.g("DatabaseMaintenance","Email Attaches successfully deleted")+": "+countDeleted.ToString()+"\r\n"
					+Lans.g("DatabaseMaintenance","Email Attaches that failed to be deleted")+": "+countErrors.ToString();
			}
			return strResults;
		}

		///<summary>Update all to new insurance plan type.</summary>
		public static void UpdateInsurancePlanType(string planType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),planType);
				return;
			}
			string command="";
			if(planType=="Percentage") {
				command="UPDATE insplan SET PlanType='p' WHERE PlanType=''";
			}
			else{//"Category"
				command="UPDATE insplan SET PlanType='' WHERE PlanType='p'";
			}
			Db.NonQ(command);
		}

		#endregion Tool Button and Helper Methods-----------------------------------------------------------------------------------------------------------

		///<summary>Uses reflection to get all database maintenance methods that are specifically flagged for DBM.
		///When clinicNum is set to a medical clinic, all methods that match "tooth" will not be returned.</summary>
		public static List<MethodInfo> GetMethodsForDisplay(long clinicNum=0,bool hasOnlyPatNumMethods=false) {
			//No need to check MiddleTierRole; no call to db.
			List<MethodInfo> listMethodInfosGrid=new List<MethodInfo>();
			//Workstations region settings.
			string country=CultureInfo.CurrentCulture.Name;
			//Grab all methods from the DatabaseMaintenance class to dynamically fill the grid.
			MethodInfo[] methodInfoArrayAll=(typeof(DatabaseMaintenances)).GetMethods();
			//Sort the methods by name so that they are easier for users to find desired methods to run.
			Array.Sort(methodInfoArrayAll,new MethodInfoComparer());
			bool isMedicalClinic=Clinics.IsMedicalPracticeOrClinic(clinicNum);
			for(int i=0;i<methodInfoArrayAll.Length;i++) {
				DbmMethodAttr dbmMethodAttr=(DbmMethodAttr)Attribute.GetCustomAttribute(methodInfoArrayAll[i],typeof(DbmMethodAttr));
				if(dbmMethodAttr==null) {
					continue;//This is not a valid DBM method.
				}
				if(!country.EndsWith("CA") && dbmMethodAttr.IsCanada) {//Skip over Canada dbm's if not in Canada.
					continue;
				}
				if(isMedicalClinic && Regex.IsMatch(methodInfoArrayAll[i].Name,"tooth",RegexOptions.IgnoreCase)) {
					continue;//This is not a DBM for medical users.
				}
				if(hasOnlyPatNumMethods && !dbmMethodAttr.HasPatNum) {
					continue;//This is not a patient specific DBM method.
				}
				if(!hasOnlyPatNumMethods && dbmMethodAttr.IsPatDependent) {
					continue;//This is a patient specific DBM method that depends on a patient being passed in.
				}
				//This is a valid DBM method and should be added to the list of methods to display to the user.
				listMethodInfosGrid.Add(methodInfoArrayAll[i]);
			}
			return listMethodInfosGrid;
		}

		///<summary>Returns true if the method passed in supports break down.</summary>
		public static bool MethodHasBreakDown(MethodInfo methodInfo) {
			//No need to check MiddleTierRole; no call to db.
			return methodInfo.GetCustomAttributes(typeof(DbmMethodAttr),true).OfType<DbmMethodAttr>().All(x => x.HasBreakDown);
		}

		///<summary>Returns true if the method passed in has an explaination.</summary>
		public static bool MethodHasExplain(MethodInfo methodInfo) {
			//No need to check MiddleTierRole; no call to db.
			return methodInfo.GetCustomAttributes(typeof(DbmMethodAttr),true).OfType<DbmMethodAttr>().All(x => x.HasExplain);
		}

		#region Helper Classes

		private class PatientMissingTableHelper {
			public string TableName;
			public string ColumnName;
			public string QueryGetPatNums;
			public string QueryGetCount;
			public Func<long,string> FuncGetUpdateQuery;

			public PatientMissingTableHelper(string tableName,string columnName,string queryGetPatNums=null,string queryGetCount=null,Func<long,string> funcGetUpdateQuery=null) {
				TableName=tableName;
				ColumnName=columnName;
				if(queryGetPatNums==null) {
					queryGetPatNums=$"SELECT DISTINCT({POut.String(ColumnName)}) FROM {POut.String(TableName)} WHERE {POut.String(ColumnName)} NOT IN (SELECT PatNum FROM patient)";
				}
				QueryGetPatNums=queryGetPatNums;
				if(queryGetCount==null) {
					queryGetCount=$"SELECT COUNT({POut.String(ColumnName)}) FROM {POut.String(TableName)} WHERE {POut.String(ColumnName)}=0";
				}
				QueryGetCount=queryGetCount;
				if(funcGetUpdateQuery==null) {
					funcGetUpdateQuery=(patNum) => {
						return $"UPDATE {POut.String(TableName)} SET {POut.String(ColumnName)}={POut.Long(patNum)} WHERE {POut.String(ColumnName)}=0";
					};
				}
				FuncGetUpdateQuery=funcGetUpdateQuery;
			}
		}

		private class PatNumProceduresGroup {
			public long PatNum;
			public string PatientName;
			public List<ProceduresGroup> ListClaimNumProceduresGroups;
			public List<ProceduresGroup> ListAdjNumProceduresGroups=new List<ProceduresGroup>();//new List<AdjNumProceduresGroup>();
			public List<ProceduresGroup> ListPayNumProceduresGroups=new List<ProceduresGroup>();//new List<PayNumProceduresGroup>();

			///<summary>Each DataRow passed in must contain the following column names:
			///LName, FName, PatNum, ClaimNum, DateService, ProcNum, ProcCode, ProcDate, ProcStatus, ProcNumLab, ProcCodeLab, ProcDateLab, ProcStatusLab.
			///Optionally: AdjNum, AdjDate, PayNum, PayDate </summary>
			public PatNumProceduresGroup(long patNum,List<DataRow> listDataRows) {
				PatNum=patNum;
				PatientName=Patients.GetNameFLnoPref(PIn.String(listDataRows.First()["LName"].ToString()),PIn.String(listDataRows.First()["FName"].ToString()),"");
				ListClaimNumProceduresGroups=listDataRows.Where(x => PIn.Long(x["ClaimNum"].ToString()) > 0)
					.GroupBy(x => PIn.Long(x["ClaimNum"].ToString()))
					.Select(x => new ProceduresGroup(x.Key,"Claim","ClaimNum","dateService",x.ToList(),strDateName:"service"))
					.ToList();
				if(!listDataRows.IsNullOrEmpty() && listDataRows[0].Table.Columns.Contains("AdjNum")) {
					ListAdjNumProceduresGroups=listDataRows.Where(x => PIn.Long(x["AdjNum"].ToString()) > 0)
						.GroupBy(x => PIn.Long(x["AdjNum"].ToString()))
						.Select(x => new ProceduresGroup(x.Key,"Adjustment","AdjNum","AdjDate",x.ToList(),amountColumn:"AdjAmt"))
						.ToList();
				}
				if(!listDataRows.IsNullOrEmpty() && listDataRows[0].Table.Columns.Contains("PayNum")) {
					ListPayNumProceduresGroups=listDataRows.Where(x => PIn.Long(x["PayNum"].ToString()) > 0)
						.GroupBy(x => PIn.Long(x["PayNum"].ToString()))
						.Select(x => new ProceduresGroup(x.Key,"Payment","PayNum","DatePay",x.ToList(),amountColumn:"SplitAmt"))
						.ToList();
				}
			}

			public override string ToString() {
				string retVal=$"{PatientName} (PatNum:{PatNum})\r\n{string.Join("\r\n",ListClaimNumProceduresGroups)}\r\n";
				if(!ListAdjNumProceduresGroups.IsNullOrEmpty()) {
					retVal+=$"{string.Join("\r\n",ListAdjNumProceduresGroups)}\r\n";
				}
				if(!ListPayNumProceduresGroups.IsNullOrEmpty()) {
					retVal+=$"{string.Join("\r\n",ListPayNumProceduresGroups)}\r\n";
				}
				return retVal;
			}
		}

		private class ProcedureLiteLabsGroup {
			public ProcedureLite ProcedureLite;
			///<summary>A singular procedure can have multiple labs associated to it.</summary>
			public List<ProcedureLite> ListProcedureLitesLabs=new List<ProcedureLite>();

			public ProcedureLiteLabsGroup(ProcedureLite procedureLite) {
				ProcedureLite=procedureLite;
			}

			public override string ToString() {
				//The parent procedure description followed by any lab fee descriptions.
				string retVal=ProcedureLite.ToString();
				if(ListProcedureLitesLabs.Count > 0) {
					retVal+="\r\n"+string.Join("\r\n",ListProcedureLitesLabs);
				}
				return retVal;
			}
		}

		private class ProcedureLite {
			public long ProcNum;
			public string ProcCode;
			public ProcStat ProcStatus;
			public DateTime ProcDate;
			public bool IsLabFee;

			public override string ToString() {
				string retVal;
				if(IsLabFee) {
					retVal=$"      ^^Lab Fee";//E.g. ^^Lab Fee 99111 on 2019-10-04 (ProcNum:97294) has a status of 'TPi'
				}
				else {
					retVal=$"    Procedure";//E.g. Procedure 67201 on 2020-03-04 (ProcNum:97282) has a status of 'C'
				}
				return retVal+$" {ProcCode} on {ProcDate.ToShortDateString()} (ProcNum:{ProcNum}) has a status of '{ProcStatus}'";
			}
		}

		private class ProcedureCodeGroup {
			public string ProcCode;
			public List<ProcedureCode> ListProcedureCodes;

			public ProcedureCodeGroup(string procCode,List<ProcedureCode> listProcedureCodes) {
				ProcCode=procCode;
				ListProcedureCodes=listProcedureCodes;
			}
		}

		private class ProceduresGroup {
			public long PrimaryKey;
			public double Amount;
			public DateTime DateService;
			public List<ProcedureLiteLabsGroup> ListProcedureLiteLabsGroups=new List<ProcedureLiteLabsGroup>();
			public string TableName;
			public string PrimaryKeyColumn;
			public string StrDateName;

			public ProceduresGroup(long primaryKey,string tableName, string primaryKeyColumn, string dateColumn, List<DataRow> listDataRows,string amountColumn="",string strDateName="") {
				PrimaryKey=primaryKey;
				TableName=tableName;
				PrimaryKeyColumn=primaryKeyColumn;
				StrDateName=strDateName;
				//Since we are looking at groupings of procedures for a given claim, adjstment, or payment we can simply select
				//the first row to get the date and amount since all rows refer to for the claim, adjustment,or payment we're looking at.
				DataRow dataRow=listDataRows.FirstOrDefault();
				if(dataRow==null) {
					return;
				}
				Amount=0;
				if(!amountColumn.IsNullOrEmpty()) {
					Amount=PIn.Double(dataRow[amountColumn].ToString());
				}
				DateService=PIn.Date(dataRow[dateColumn].ToString());
				for(int i=0;i<listDataRows.Count;i++) {
					long procNum=PIn.Long(listDataRows[i]["ProcNum"].ToString());
					long procNumLab=PIn.Long(listDataRows[i]["ProcNumLab"].ToString());
					long procNumParent=procNum;
					if(procNumLab!=0) {
						procNumParent=procNumLab;//Prefer the 'parent' num for lab fees.
					}
					//Check to see if parent procedure has already been added to the list (procedures can have multiple lab fees attached).
					if(ListProcedureLiteLabsGroups.Any(x => x.ProcedureLite.ProcNum==procNumParent)) {
						continue;
					}
					ProcedureLite procedureLite=new ProcedureLite();
					if(procNumLab==0) {//parent proc
						procedureLite.ProcNum=procNum;
						procedureLite.ProcCode=PIn.String(listDataRows[i]["ProcCode"].ToString());
						procedureLite.ProcStatus=PIn.Enum<ProcStat>(listDataRows[i]["ProcStatus"].ToString());
						procedureLite.ProcDate=PIn.Date(listDataRows[i]["ProcDate"].ToString());
						ListProcedureLiteLabsGroups.Add(new ProcedureLiteLabsGroup(procedureLite));
						continue;
					}
					//else lab
					procedureLite.ProcNum=procNumLab;
					procedureLite.ProcCode=PIn.String(listDataRows[i]["ProcCodeLab"].ToString());
					procedureLite.ProcStatus=PIn.Enum<ProcStat>(listDataRows[i]["ProcStatusLab"].ToString());
					procedureLite.ProcDate=PIn.Date(listDataRows[i]["ProcDateLab"].ToString());
					ListProcedureLiteLabsGroups.Add(new ProcedureLiteLabsGroup(procedureLite));
				}
				List<DataRow> listDataRowsLabs=listDataRows.FindAll(x => x["ProcNumLab"].ToString()!="0");
				//Add any Canadian Lab Fees to their parent ProcNumProcLabsGroup.
				for(int i=0;i<listDataRowsLabs.Count;i++) {
					long procNumLab=PIn.Long(listDataRows[i]["ProcNumLab"].ToString());
					ProcedureLite procedureLite=new ProcedureLite();
					procedureLite.ProcNum=PIn.Long(listDataRows[i]["ProcNum"].ToString());
					procedureLite.ProcCode=PIn.String(listDataRows[i]["ProcCode"].ToString());
					procedureLite.ProcStatus=PIn.Enum<ProcStat>(listDataRows[i]["ProcStatus"].ToString());
					procedureLite.ProcDate=PIn.Date(listDataRows[i]["ProcDate"].ToString());
					procedureLite.IsLabFee=true;
					//Look for the parent procedure (which should have been added already).
					ProcedureLiteLabsGroup procedureLiteLabsGroup=ListProcedureLiteLabsGroups.Find(x => x.ProcedureLite.ProcNum==procNumLab);
					if(procedureLiteLabsGroup==null) {
						//The parent procedure wasn't found in the database... probably database corruption.
						//Regardless, add this lab fee as a parent procedure so that it shows up in the break down.
						ListProcedureLiteLabsGroups.Add(new ProcedureLiteLabsGroup(procedureLite));
						continue;
					}
					//Add this lab fee to the parent procedure list of labs; there can be multiple labs.
					procedureLiteLabsGroup.ListProcedureLitesLabs.Add(procedureLite);
				}
			}

			public override string ToString() {
				string retVal=$"  {TableName} ({PrimaryKeyColumn}: {PrimaryKey}) ";
				if(Amount != 0) {//Allow for positive and negative amounts
					retVal+=$"for {Amount.ToString("c")} ";
				}
				retVal+=$"with ";
				if(StrDateName.IsNullOrEmpty()) {
					retVal+=TableName.ToLower();
				}
				else {
					retVal+=StrDateName.ToLower();
				}
				retVal+=$" date of {DateService.ToShortDateString()}\r\n{string.Join("\r\n",ListProcedureLiteLabsGroups)}";
				return retVal;
			}
		}

		/// <summary>SQL Global Variable Helper Class for DBM method 'MySQLServerOptionsValidate' to check if that variable is set to that value. You can also pass in a list of requirements and it will check to see if the variable value matches any of the supplied list of strings. You may also pass in a version by specifying 'requiresVersionCheck=true' and supplying a sql global version variable name and a minimum required version number. This should be in a standard dot-based format such as "10.5.21.0". When supplying a version minimum, IsValid will return true if the main variable is valid - the version is only checked if the main variable is not valid.<br></br><b>Example:</b> If the dbms needs to have the sql variable "myisam_recover_options" set to "off" if the dbms variable 'version' is greater than or equal to "10.5.9" => "new GlobalVariableH("myisam_recover_options", "off", requiresVersionCheck:true, "version", "10.5.9"));"</summary>
		private class GlobalVariableH {
			/// <summary>Literal string of the db variable name. Like: "sql_mode"</summary>
			public readonly string Name;
			/// <summary>Value retrieved from the DB when instantiated.</summary>
			public readonly string Value;
			/// <summary>Returns true if the value is blank or if it contains one of the passed-in desired values. If a version is supplied and it fails the primary validity check, it will perform a secondary validity check. In that case IsValid will return true only if the current version is less than the supplied version.</summary>
			public readonly bool IsValid;
			public readonly bool MayLackPermissions;
			public readonly bool RequiresVersionCheck;
			private readonly string _version;
			private readonly List<string> _listDesiredValues;

			public GlobalVariableH(string varName, string desiredValue, bool requiresVersionCheck=false, string versionVarName="", string versionMin="") : this(varName, new List<string>(){ desiredValue }, requiresVersionCheck, versionVarName, versionMin) { }

			public GlobalVariableH(string varName, List<string> listDesiredValues, bool requiresVersionCheck=false, string versionVarName=null, string versionMin=null) {
				Name=varName;
				_listDesiredValues=listDesiredValues;
				DataTable table=Db.GetTable($"SHOW GLOBAL VARIABLES LIKE '{Name}'");
				if(table.Rows.Count<1 || table.Columns.Count<2) { // User may not have permission to access global variables?
					MayLackPermissions=true;
					return;
				}
				Value=table.Rows[0][1].ToString().ToLower().Trim();
				IsValid=_listDesiredValues.Exists(x=> Value.Contains(x)) || string.IsNullOrWhiteSpace(Value);
				if(!requiresVersionCheck || versionVarName==null || versionMin==null) {
					return;
				}
				table=Db.GetTable($"SHOW GLOBAL VARIABLES LIKE '{versionVarName}'");
				if(table.Rows.Count<1 || table.Columns.Count<2) { // User may not have permission to access global variables?
					MayLackPermissions=true;
					return;
				}
				_version=table.Rows[0][1].ToString().Trim();
				IsValid=IsValid || !CurVerMeetsMinReq(_version,versionMin);
				RequiresVersionCheck=true;
			}

			private bool CurVerMeetsMinReq(string version, string versionMin) {
				List<string> listValues=version.Split('.').ToList();
				List<string> listValuesMin=versionMin.Split('.').ToList();
				int versionSegments=Math.Min(listValues.Count,listValuesMin.Count);
				for(int i=0;i<versionSegments;i++) {
					string digitsOnly=new string(listValues[i].Where(x=>char.IsDigit(x)).ToArray());
					if (PIn.Int(digitsOnly)>PIn.Int(listValuesMin[i])) {
						return true;
					}
					else if(PIn.Int(digitsOnly)<PIn.Int(listValuesMin[i])) {
						return false;
					}
				}
				return true;
			}

			///<summary>Returns a string in the following format: "The MySQL server variable [variableName] is currently set to [variableValue].\r\n". If the object passed in isn't Valid it will automatically add the clause "should be blank or [ReqVal], but " to the middle of the string as appropriate. All (static parts) of the string are translated using Lan.g().</summary>
			public string GetMessage() {
				string message="";
				if(RequiresVersionCheck && !IsValid) {
					message+=Lans.g("FormDatabaseMaintenance","For your DBMS version")+$" ({_version}), ";
				}
				message+=Lans.g("FormDatabaseMaintenance","The MySQL server variable")+$" '{Name}' ";
				if(!IsValid) {
					if(_listDesiredValues.Count==1) {
						message+=Lans.g("FormDatabaseMaintenance","should be blank or")+$" '{_listDesiredValues[0]}', ";
					}
					else if(_listDesiredValues.Count>1) {
						string tempS=string.Join("', '",_listDesiredValues);
						tempS=tempS.Insert(tempS.LastIndexOf(',')+1," or ");
						message+=Lans.g("FormDatabaseMaintenance","should be blank")+$", '{tempS}', ";
					}
					message+=Lans.g("FormDatabaseMaintenance","but")+" ";
				}
				string myVal=Value;
				if(string.IsNullOrWhiteSpace(Value)){
					myVal=Lans.g("FormDatabaseMaintenance","blank");
				}
				message+=Lans.g("FormDatabaseMaintenance","is currently set to")+" '"+myVal+"'.\r\n";
				return message;
			}
		}

		#endregion Helper Classes
	}
}
