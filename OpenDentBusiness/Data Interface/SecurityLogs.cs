using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SecurityLogs {
		///<summary>The log source of the current application.</summary>
		public static LogSources LogSource=LogSources.None;

		#region Get Methods
		///<summary>Returns one SecurityLog from the db.  Called from SecurityLogHashs.CreateSecurityLogHash()</summary>
		public static SecurityLog GetOne(long securityLogNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SecurityLog>(MethodBase.GetCurrentMethod(),securityLogNum);
			}
			return Crud.SecurityLogCrud.SelectOne(securityLogNum);
		}

		///<summary>Gets many security logs matching the passed in parameters.///</summary>
		public static List<SecurityLog> GetMany(params SQLWhere[] sQLWhereArray) {
			Meth.NoCheckMiddleTierRole();
			return GetMany(sQLWhereArray.ToList());
		}

		///<summary>Gets a list of all securitylogs matching the passed in parameters.</summary>
		public static List<SecurityLog> GetMany(List<SQLWhere> listSQLWheres) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SecurityLog>>(MethodBase.GetCurrentMethod(),listSQLWheres);
			}
			string command="SELECT * FROM securitylog ";
			if(listSQLWheres!=null && listSQLWheres.Count > 0) {
				command+="WHERE "+string.Join(" AND ",listSQLWheres);
			}
			return Crud.SecurityLogCrud.SelectMany(command);
		}

		///<summary>Gets a list of all securitylogs for a specific API developer.</summary>
		public static List<SecurityLog> GetManyForApi(int limit,int offset,int permType,string apiDeveloperName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SecurityLog>>(MethodBase.GetCurrentMethod(),limit,offset,permType,apiDeveloperName);
			}
			string command="SELECT * FROM securitylog "
				+"WHERE LogText LIKE '%by "+POut.String(apiDeveloperName)+" through%' ";
			if(permType>-1) {//0 is 'None' and is valid.
				command+="AND PermType='"+POut.Long(permType)+"' ";
			}
			command+="AND LogSource='"+POut.Int((int)LogSources.API)+"' "//23 is LogSources.API
				+"ORDER BY SecurityLogNum DESC "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.SecurityLogCrud.SelectMany(command);
		}

		#endregion

		#region Delete
		public static void DeleteWithMaxPriKey(long securityLogMaxPriKey) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),securityLogMaxPriKey);
				return;
			}
			if(securityLogMaxPriKey==0) {
				return;
			}
			string command="DELETE FROM securitylog WHERE SecurityLogNum <= "+POut.Long(securityLogMaxPriKey);
			Db.NonQ(command);
		}

		public static long DeleteBeforeDateInclusive(DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),date);
			}
			int countDeleted=0;
			List<long> listSecurityLogNums;
			while(true) {
				//Delete the hashes
				ODEvent.Fire(CodeBase.ODEventType.ProgressBar,
					Lans.g("FormBackup","Removing old data from securityloghash table. Rows deleted so far:")+" "+countDeleted);
				//limiting to 100,000 to avoid out of memory exceptions
				string command=$"SELECT SecurityLogNum FROM securitylog WHERE DATE(LogDateTime) <= {POut.DateT(date.Date)} LIMIT 100000";
				listSecurityLogNums=Db.GetListLong(command);
				if(listSecurityLogNums.Count<1) {
					break;
				}
				SecurityLogHashes.DeleteForSecurityLogEntries(listSecurityLogNums);
				//Then delete the securitylog entries themselves
				ODEvent.Fire(CodeBase.ODEventType.ProgressBar,
					Lans.g("FormBackup","Removing old data from securitylog table. Rows deleted so far:")+" "+countDeleted);
				command=$"DELETE FROM securitylog WHERE SecurityLogNum IN ({string.Join(",",listSecurityLogNums)})";
				Db.NonQ(command);
				countDeleted+=listSecurityLogNums.Count;
				if(listSecurityLogNums.Count <= 0) {
					break;
				}
			}
			return countDeleted;
		}
		#endregion

		///<summary>Used when viewing securityLog from the security admin window.  PermTypes can be length 0 to get all types.
		///Throws exceptions.</summary>
		public static SecurityLog[] Refresh(DateTime dateFrom,DateTime dateTo,EnumPermType permType,long patNum,
			DateTime datePreviousFrom,DateTime datePreviousTo,int limit=0,long userNum=-1,int logSource=-1) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SecurityLog[]>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,permType,patNum,datePreviousFrom,datePreviousTo,limit,userNum,logSource);
			}
			string command="SELECT securitylog.*,LName,FName,Preferred,MiddleI,LogHash FROM securitylog "
				+"LEFT JOIN patient ON patient.PatNum=securitylog.PatNum "
				+"LEFT JOIN securityloghash ON securityloghash.SecurityLogNum=securitylog.SecurityLogNum "
				+"WHERE LogDateTime >= "+POut.Date(dateFrom)+" "
				+"AND LogDateTime <= "+POut.Date(dateTo.AddDays(1))+" "
				+"AND DateTPrevious >= "+POut.Date(datePreviousFrom)+" "
				+"AND DateTPrevious <= "+POut.Date(datePreviousTo.AddDays(1));
			if(patNum !=0) {
				command+=" AND securitylog.PatNum IN ("+string.Join(",",
					PatientLinks.GetPatNumsLinkedToRecursive(patNum,PatientLinkType.Merge).Select(x => POut.Long(x)))+")";
			}
			if(permType!=EnumPermType.None) {
				command+=" AND PermType="+POut.Long((int)permType);
			}
			if(userNum>=0) { //Greater than or equal to 0, since 0 is no/unknown user, and we want to be able to filter by that option in some cases.
				command+=" AND UserNum="+POut.Long(userNum);
			}
			if(logSource>=0) { //Greater than or equal to 0, since 0 is Automation/unknown, and we want to be able to filter by that option in some cases.
				command+=" AND LogSource="+POut.Long(logSource);
			}
			command+=" ORDER BY LogDateTime DESC";//Using DESC so that the most recent ones appear in the list
			if(limit>0) {
				command=DbHelper.LimitOrderBy(command,limit);
			}
			DataTable table=Db.GetTable(command);
			List<SecurityLog> listSecurityLogs=Crud.SecurityLogCrud.TableToList(table);
			for(int i=0;i<listSecurityLogs.Count;i++) {
				if(table.Rows[i]["PatNum"].ToString()=="0") {
					listSecurityLogs[i].PatientName="";
				}
				else {
					listSecurityLogs[i].PatientName=table.Rows[i]["PatNum"].ToString()+"-"
						+Patients.GetNameLF(table.Rows[i]["LName"].ToString()
						,table.Rows[i]["FName"].ToString()
						,table.Rows[i]["Preferred"].ToString()
						,table.Rows[i]["MiddleI"].ToString());
				}
				listSecurityLogs[i].LogHash=table.Rows[i]["LogHash"].ToString();
			}
			return listSecurityLogs.OrderBy(x => x.LogDateTime).ToArray();
		}

		///<summary></summary>
		public static long Insert(SecurityLog securityLog){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				securityLog.SecurityLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),securityLog);
				return securityLog.SecurityLogNum;
			}
			return Crud.SecurityLogCrud.Insert(securityLog);
		}

		//there are no methods for deleting or changing log entries because that will never be allowed.

		///<summary>Used when viewing various audit trails of specific types.  Only implemented Appointments,ProcFeeEdit,InsPlanChangeCarrierName so far. patNum only used for Appointments.  The other two are zero.</summary>
		public static SecurityLog[] Refresh(long patNum,List<EnumPermType> listPermissionsEnums,long fKey) {
			Meth.NoCheckMiddleTierRole();
			return Refresh(patNum,listPermissionsEnums,new List<long>(){ fKey });
		}

		///<summary>Used when viewing various audit trails of specific types.  This overload will return security logs for multiple objects (or fKeys).
		///Typically you will only need a specific type audit log for one type.
		///However, for things like ortho charts, each row (FK) in the database represents just one part of a larger ortho chart "object".
		///Thus, to get the full experience of a specific type audit trail window, we need to get security logs for multiple objects (FKs) that
		///comprise the larger object (what the user sees).  Only implemented with ortho chart so far.  FKeys can be null.
		///Throws exceptions.</summary>
		public static SecurityLog[] Refresh(long patNum,List<EnumPermType> listPermissionsEnums,List<long> listFKeys) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SecurityLog[]>(MethodBase.GetCurrentMethod(),patNum,listPermissionsEnums,listFKeys);
			}
			string types="";
			for(int i=0;i<listPermissionsEnums.Count;i++) {
				if(i>0) {
					types+=" OR";
				}
				types+=" PermType="+POut.Long((int)listPermissionsEnums[i]);
			}
			string command="SELECT * FROM securitylog "
				+"WHERE ("+types+") ";
			if(listFKeys!=null && listFKeys.Count > 0) {
				command+="AND FKey IN ("+String.Join(",",listFKeys)+") ";
			}
			if(patNum!=0) {//appointments
				command+=" AND PatNum IN ("+string.Join(",",
					PatientLinks.GetPatNumsLinkedToRecursive(patNum,PatientLinkType.Merge).Select(x => POut.Long(x)))+")";
			}
			command+="ORDER BY LogDateTime";
			List<SecurityLog> listSecurityLogs=Crud.SecurityLogCrud.SelectMany(command);
			return listSecurityLogs.OrderBy(x => x.LogDateTime).ToArray();
		}

		///<summary>Gets all security logs for the given foreign keys and permissions.</summary>
		public static List<SecurityLog> GetFromFKeysAndType(List<long> listFKeys,List<EnumPermType> listPermissionsEnums) {
			if(listFKeys==null || listFKeys.FindAll(x => x != 0).Count==0) {
				return new List<SecurityLog>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SecurityLog>>(MethodBase.GetCurrentMethod(),listFKeys,listPermissionsEnums);
			}
			string command="SELECT * FROM securitylog WHERE FKey IN("+string.Join(",",listFKeys.FindAll(x => x != 0))+") AND PermType IN"+
				"("+string.Join(",",listPermissionsEnums.Select(x => POut.Int((int)x)))+")";
			return Crud.SecurityLogCrud.SelectMany(command);
		}

		///<summary>Used to insert a list of security logs.</summary>
		///<param name="permType">The type of permission to be logged in the security log.</param>
		///<param name="patNum">The PatNum for the patient associated to the security log. Can be 0.</param>
		///<param name="listLogTexts">A list of the security log text that should be inserted.</param>
		public static void MakeLogEntries(EnumPermType permType,long patNum,List<string> listLogTexts) {
			if(listLogTexts==null || listLogTexts.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),permType,patNum,listLogTexts);
				return;
			}
			for(int i=0;i<listLogTexts.Count;i++) {
				MakeLogEntry(permType,patNum,listLogTexts[i]);
			}
		}

		///<summary>PatNum can be 0.</summary>
		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText) {
			Meth.NoCheckMiddleTierRole();
			MakeLogEntry(permType,patNum,logText,0,LogSource,DateTime.MinValue);
		}

		///<summary>Used when the security log needs to be identified by a particular source.  PatNum can be 0.</summary>
		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText,LogSources logSource) {
			Meth.NoCheckMiddleTierRole();
			MakeLogEntry(permType,patNum,logText,0,logSource,DateTime.MinValue);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.</summary>
		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText,long fKey,DateTime DateTPrevious) {
			Meth.NoCheckMiddleTierRole();
			MakeLogEntry(permType,patNum,logText,fKey,LogSource,DateTPrevious);
		}
		///<summary>Pass in device name, used in eClipboard</summary>
		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText,long fKey,DateTime DateTPrevious,string deviceName) {
			Meth.NoCheckMiddleTierRole();
			MakeLogEntry(permType,patNum,logText,fKey,LogSource,0,0,DateTPrevious,deviceName);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.</summary>
		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText,long fKey,LogSources logSource,DateTime DateTPrevious) {
			Meth.NoCheckMiddleTierRole();
			MakeLogEntry(permType,patNum,logText,fKey,logSource,0,0,DateTPrevious);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0. Allows user to be specified. </summary>
		public static void MakeLogEntry(EnumPermType permType, long patNum, string logText, long fKey, LogSources logSource, DateTime DateTPrevious, long userNum) {
			Meth.NoCheckMiddleTierRole();
			SecurityLog securityLog = MakeLogEntryNoInsert(permType, patNum, logText, fKey, logSource, 0, 0, DateTPrevious, userNum);
			MakeLogEntry(securityLog);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.</summary>
		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText,long fKey,LogSources logSource,long defNum,long defNumError,
			DateTime DateTPrevious) 
		{
			Meth.NoCheckMiddleTierRole();
			SecurityLog securityLog=MakeLogEntryNoInsert(permType,patNum,logText,fKey,logSource,defNum,defNumError,DateTPrevious);
			MakeLogEntry(securityLog);
		}

		public static void MakeLogEntry(EnumPermType permType,long patNum,string logText,long fKey,LogSources logSource,long defNum,long defNumError,
			DateTime DateTPrevious,string deviceName) {
			Meth.NoCheckMiddleTierRole();
			SecurityLog securityLog=MakeLogEntryNoInsert(permType,patNum,logText,fKey,logSource,deviceName,defNum,defNumError,DateTPrevious);
			MakeLogEntry(securityLog);
		}

		///<summary>Can pass in a device name, used with eClipboard</summary>
		public static SecurityLog MakeLogEntryNoInsert(EnumPermType permType,long patNum,string logText,long fKey,LogSources logSource,string deviceName,
			long defNum = 0,long defNumError = 0,DateTime DateTPrevious = default(DateTime)) {
			Meth.NoCheckMiddleTierRole();
			SecurityLog securityLog=MakeLogEntryNoInsert(permType,patNum,logText,fKey,logSource,defNum,defNumError,DateTPrevious);
			securityLog.CompName=deviceName;
			return securityLog;
		}

		///<summary>Take a SecurityLog object to save to the database. Creates a SecurityLogHash object as well.</summary>
		public static void MakeLogEntry(SecurityLog securityLog) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),securityLog);
				return;
			}
			securityLog.SecurityLogNum=SecurityLogs.Insert(securityLog);
			SecurityLogHashes.InsertSecurityLogHash(securityLog.SecurityLogNum);//uses db date/time
			if(securityLog.PermType==EnumPermType.AppointmentCreate) {
				EntryLog entryLog=new EntryLog();
				entryLog.UserNum=securityLog.UserNum;
				entryLog.FKeyType=EntryLogFKeyType.Appointment;
				entryLog.FKey=securityLog.FKey;
				entryLog.LogSource=securityLog.LogSource;
				EntryLogs.Insert(entryLog);
			}
		}
		
		///<summary>Creates security log entries for all that PatNums passed in.</summary>
		public static void MakeLogEntry(EnumPermType permType,List<long> listPatNums,string logText) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),permType,listPatNums,logText);
				return;
			}
			List<SecurityLog> listSecurityLogs=new List<SecurityLog>();
			for(int i=0;i<listPatNums.Count;i++) {
				SecurityLog securityLog=MakeLogEntryNoInsert(permType,listPatNums[i],logText,0,LogSource);
				SecurityLogs.Insert(securityLog);
				listSecurityLogs.Add(securityLog);
			}
			List<SecurityLogHash> listSecurityLogHashes=new List<SecurityLogHash>();
			List<EntryLog> listEntryLogs=new List<EntryLog>();
			List<long> listSecurityLogNums=listSecurityLogs.Select(x => x.SecurityLogNum).ToList();
			SQLWhere sQLWhere=SQLWhere.CreateIn(nameof(SecurityLog.SecurityLogNum),listSecurityLogNums);
			listSecurityLogs=SecurityLogs.GetMany(sQLWhere);
			for(int i=0;i<listSecurityLogs.Count;i++) {
				SecurityLogHash securityLogHash=new SecurityLogHash();
				securityLogHash.SecurityLogNum=listSecurityLogs[i].SecurityLogNum;
				securityLogHash.LogHash=SecurityLogHashes.GetHashString(listSecurityLogs[i]);
				listSecurityLogHashes.Add(securityLogHash);
				if(listSecurityLogs[i].PermType==EnumPermType.AppointmentCreate) {
					EntryLog entryLog=new EntryLog();
					entryLog.UserNum=listSecurityLogs[i].UserNum;
					entryLog.FKeyType=EntryLogFKeyType.Appointment;
					entryLog.FKey=listSecurityLogs[i].FKey;
					entryLog.LogSource=listSecurityLogs[i].LogSource;
					listEntryLogs.Add(entryLog);
				}
			}
			EntryLogs.InsertMany(listEntryLogs);
			SecurityLogHashes.InsertMany(listSecurityLogHashes);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.  Returns the created SecurityLog object.  Does not perform an insert.</summary>
		public static SecurityLog MakeLogEntryNoInsert(EnumPermType permType,long patNum,string logText,long fKey,LogSources logSource,long defNum=0,
			long defNumError=0,DateTime DateTPrevious=default(DateTime), long userNum=0) 
		{
			Meth.NoCheckMiddleTierRole();
			SecurityLog securityLog=new SecurityLog();
			securityLog.PermType=permType;
			securityLog.UserNum=userNum;//need to be able to pass in UserNum from mobile app user
			if(userNum==0) {
				securityLog.UserNum=Security.CurUser.UserNum;
			}
			securityLog.LogText=logText;
			securityLog.CompName=Security.GetComplexComputerName();
			securityLog.PatNum=patNum;
			securityLog.FKey=fKey;
			securityLog.LogSource=logSource;
			securityLog.DefNum=defNum;
			securityLog.DefNumError=defNumError;
			securityLog.DateTPrevious=DateTPrevious;
			return securityLog;
		}

		///<summary>Used when making a security log from a remote server, possibly with multithreaded connections.</summary>
		public static void MakeLogEntryNoCache(EnumPermType permType,long patnum,string logText, long userNum=0) {
			Meth.NoCheckMiddleTierRole();
			MakeLogEntryNoCache(permType,patnum,logText,userNum,LogSource);
		}

		///<summary>Used when making a security log from a remote server, possibly with multithreaded connections.</summary>
		public static void MakeLogEntryNoCache(EnumPermType permType,long patnum,string logText,long userNum,LogSources source) {
			Meth.NoCheckMiddleTierRole();
			SecurityLog securityLog=new SecurityLog();
			securityLog.PermType=permType;
			securityLog.UserNum=userNum;
			securityLog.LogText=logText;
			securityLog.CompName=Security.GetComplexComputerName();
			securityLog.PatNum=patnum;
			securityLog.FKey=0;
			securityLog.LogSource=source;
			securityLog.SecurityLogNum=SecurityLogs.InsertNoCache(securityLog);
			SecurityLogHashes.InsertSecurityLogHashNoCache(securityLog.SecurityLogNum);
		}

		///<summary>Insertion logic that doesn't use the cache. Has special cases for generating random PK's and handling Oracle insertions.</summary>
		public static long InsertNoCache(SecurityLog securityLog) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),securityLog);
			}
			return Crud.SecurityLogCrud.InsertNoCache(securityLog);
		}

		///<summary>Adds changes made to certain procedure fields to passed security logtext. These fields are CodeNum, ProcFee, ProcDate, Surf, ToothNum, and ToothRange. More fields can be added at a later time.</summary>
		public static string AppendProcCompleteEditSecurityLog(Procedure procNew, Procedure procOld) {
			Meth.NoCheckMiddleTierRole();
			string logText="";
			if(procNew==null || procOld==null) {
				return logText;
			}
			if(procNew.CodeNum!=procOld.CodeNum) {
				string oldProcCode=ProcedureCodes.GetStringProcCode(procOld.CodeNum,doThrowIfMissing:false);
				string newProcCode=ProcedureCodes.GetStringProcCode(procNew.CodeNum,doThrowIfMissing:false);
				logText+=Lans.g("Procedures","\nCode ")+oldProcCode+Lans.g("Procedures"," with fee of ")+procOld.ProcFee.ToString("F")
					+Lans.g("Procedures"," changed to code ")+newProcCode+Lans.g("Procedures"," with fee of ")+procNew.ProcFee.ToString("F");
			}
			if(procNew.ProcDate!=procOld.ProcDate) {
				logText+=Lans.g("Procedures","\nProcDate changed from ")+procOld.ProcDate.ToShortDateString()+Lans.g("Procedures"," to ")+procNew.ProcDate.ToShortDateString();
			}
			if(procNew.Surf!=procOld.Surf) {    //because Surf could be changed to or from blank, print "none" instead
				logText+=Lans.g("Procedures","\nSurf changed from ");
				if(procOld.Surf==null) {
					logText+=Lans.g("Procedures","none to ");
				}
				else {
					logText+=procOld.Surf+" to ";
				}
				if(procNew.Surf==null) {
					logText+=Lans.g("Procedures","none");
				}
				else {
					logText+=procNew.Surf;
				}
			}
			if(procNew.ToothNum!=procOld.ToothNum) { //because ToothNum could be changed to or from blank, print "none" instead
				logText+=Lans.g("Procedures","\nToothNum changed from ");
				if(procOld.ToothNum==null) {
					logText+=Lans.g("Procedures","none to");
				}
				else {
					logText+=procOld.ToothNum+" to ";
				}
				if(procNew.ToothNum==null) {
					logText+=Lans.g("Procedures","none");
				}
				else {
					logText+=procNew.ToothNum;
				}
			}
			if(procNew.ToothRange!=procOld.ToothRange) {
				logText+=Lans.g("Procedures","\nToothRange changed from ")+procOld.ToothRange+Lans.g("Procedures"," to ")+procNew.ToothRange;
			}
			return logText;
		}
	}
}