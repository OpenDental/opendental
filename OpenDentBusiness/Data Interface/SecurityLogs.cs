using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SecurityLogs {
		///<summary>The log source of the current application.</summary>
		public static LogSources LogSource=LogSources.None;

		#region Get Methods
		///<summary>Returns one SecurityLog from the db.  Called from SecurityLogHashs.CreateSecurityLogHash()</summary>
		public static SecurityLog GetOne(long securityLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SecurityLog>(MethodBase.GetCurrentMethod(),securityLogNum);
			}
			return Crud.SecurityLogCrud.SelectOne(securityLogNum);
		}

		///<summary>Gets many security logs matching the passed in parameters.///</summary>
		public static List<SecurityLog> GetMany(params SQLWhere[] whereClause) {
			List<SQLWhere> listWheres=new List<SQLWhere>();
			foreach(SQLWhere where in whereClause) {
				listWheres.Add(where);
			}
			return GetMany(listWheres);
		}

		///<summary>Gets a list of all securitylogs matching the passed in parameters.</summary>
		public static List<SecurityLog> GetMany(List<SQLWhere> listWheres) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SecurityLog>>(MethodBase.GetCurrentMethod(),listWheres);
			}
			string command="SELECT * FROM securitylog ";
			if(listWheres!=null && listWheres.Count > 0) {
				command+="WHERE "+string.Join(" AND ",listWheres);
			}
			return Crud.SecurityLogCrud.SelectMany(command);
		}

		#endregion

		#region Delete
		public static void DeleteWithMaxPriKey(long securityLogMaxPriKey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),securityLogMaxPriKey);
				return;
			}
			if(securityLogMaxPriKey==0) {
				return;
			}
			string command="DELETE FROM securitylog WHERE SecurityLogNum <= "+POut.Long(securityLogMaxPriKey);
			Db.NonQ(command);
		}

		public static void DeleteBeforeDateInclusive(DateTime date) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),date);
				return;
			}
			int countDeleted=0;
			List<long> listSecurityLogNums;
			do {
				//Delete the hashes
				ProgressBarEvent.Fire(CodeBase.ODEventType.ProgressBar,
					Lans.g("FormBackup","Removing old data from securityloghash table. Rows deleted so far:")+" "+countDeleted);
				//limiting to 100,000 to avoid out of memory exceptions
				string command=$"SELECT SecurityLogNum FROM securitylog WHERE DATE(LogDateTime) <= {POut.DateT(date.Date)} LIMIT 100000";
				listSecurityLogNums=Db.GetListLong(command);
				if(listSecurityLogNums.Count<1) {
					break;
				}
				SecurityLogHashes.DeleteForSecurityLogEntries(listSecurityLogNums);
				//Then delete the securitylog entries themselves
				ProgressBarEvent.Fire(CodeBase.ODEventType.ProgressBar,
					Lans.g("FormBackup","Removing old data from securitylog table. Rows deleted so far:")+" "+countDeleted);
				command=$"DELETE FROM securitylog WHERE SecurityLogNum IN ({string.Join(",",listSecurityLogNums)})";
				Db.NonQ(command);
				countDeleted+=listSecurityLogNums.Count;
			}
			while(listSecurityLogNums.Count > 0);
			MessageBox.Show(Lans.g("FormBackup", "Successfully removed ")+countDeleted+Lans.g("FormBackup", " securitylog entries."));
		}
		#endregion

		///<summary>Used when viewing securityLog from the security admin window.  PermTypes can be length 0 to get all types.
		///Throws exceptions.</summary>
		public static SecurityLog[] Refresh(DateTime dateFrom,DateTime dateTo,Permissions permType,long patNum,long userNum,
			DateTime datePreviousFrom,DateTime datePreviousTo,int limit=0) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SecurityLog[]>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,permType,patNum,userNum,datePreviousFrom,datePreviousTo,limit);
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
			if(permType!=Permissions.None) {
				command+=" AND PermType="+POut.Long((int)permType);
			}
			if(userNum!=0) {
				command+=" AND UserNum="+POut.Long(userNum);
			}
			command+=" ORDER BY LogDateTime DESC";//Using DESC so that the most recent ones appear in the list
			if(limit>0) {
				command=DbHelper.LimitOrderBy(command,limit);
			}
			DataTable table=Db.GetTable(command);
			List<SecurityLog> listLogs=Crud.SecurityLogCrud.TableToList(table);
			for(int i=0;i<listLogs.Count;i++) {
				if(table.Rows[i]["PatNum"].ToString()=="0") {
					listLogs[i].PatientName="";
				}
				else {
					listLogs[i].PatientName=table.Rows[i]["PatNum"].ToString()+"-"
						+Patients.GetNameLF(table.Rows[i]["LName"].ToString()
						,table.Rows[i]["FName"].ToString()
						,table.Rows[i]["Preferred"].ToString()
						,table.Rows[i]["MiddleI"].ToString());
				}
				listLogs[i].LogHash=table.Rows[i]["LogHash"].ToString();
			}
			return listLogs.OrderBy(x => x.LogDateTime).ToArray();
		}

		///<summary></summary>
		public static long Insert(SecurityLog log){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				log.SecurityLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),log);
				return log.SecurityLogNum;
			}
			return Crud.SecurityLogCrud.Insert(log);
		}

		//there are no methods for deleting or changing log entries because that will never be allowed.

		///<summary>Used when viewing various audit trails of specific types.  Only implemented Appointments,ProcFeeEdit,InsPlanChangeCarrierName so far. patNum only used for Appointments.  The other two are zero.</summary>
		public static SecurityLog[] Refresh(long patNum,List<Permissions> permTypes,long fKey) {
			//No need to check RemotingRole; no call to db.
			return Refresh(patNum,permTypes,new List<long>(){ fKey });
		}

		///<summary>Used when viewing various audit trails of specific types.  This overload will return security logs for multiple objects (or fKeys).
		///Typically you will only need a specific type audit log for one type.
		///However, for things like ortho charts, each row (FK) in the database represents just one part of a larger ortho chart "object".
		///Thus, to get the full experience of a specific type audit trail window, we need to get security logs for multiple objects (FKs) that
		///comprise the larger object (what the user sees).  Only implemented with ortho chart so far.  FKeys can be null.
		///Throws exceptions.</summary>
		public static SecurityLog[] Refresh(long patNum,List<Permissions> permTypes,List<long> fKeys) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SecurityLog[]>(MethodBase.GetCurrentMethod(),patNum,permTypes,fKeys);
			}
			string types="";
			for(int i=0;i<permTypes.Count;i++) {
				if(i>0) {
					types+=" OR";
				}
				types+=" PermType="+POut.Long((int)permTypes[i]);
			}
			string command="SELECT * FROM securitylog "
				+"WHERE ("+types+") ";
			if(fKeys!=null && fKeys.Count > 0) {
				command+="AND FKey IN ("+String.Join(",",fKeys)+") ";
			}
			if(patNum!=0) {//appointments
				command+=" AND PatNum IN ("+string.Join(",",
					PatientLinks.GetPatNumsLinkedToRecursive(patNum,PatientLinkType.Merge).Select(x => POut.Long(x)))+")";
			}
			command+="ORDER BY LogDateTime";
			List<SecurityLog> listLogs=Crud.SecurityLogCrud.SelectMany(command);
			return listLogs.OrderBy(x => x.LogDateTime).ToArray();
		}

		///<summary>Gets all security logs for the given foreign keys and permissions.</summary>
		public static List<SecurityLog> GetFromFKeysAndType(List<long> listFKeys,List<Permissions> permTypes) {
			if(listFKeys==null || listFKeys.FindAll(x => x != 0).Count==0) {
				return new List<SecurityLog>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SecurityLog>>(MethodBase.GetCurrentMethod(),listFKeys,permTypes);
			}
			string command="SELECT * FROM securitylog WHERE FKey IN("+string.Join(",",listFKeys.FindAll(x => x != 0))+") AND PermType IN"+
				"("+string.Join(",",permTypes.Select(x => POut.Int((int)x)))+")";
			return Crud.SecurityLogCrud.SelectMany(command);
		}

		///<summary>Used to insert a list of security logs.</summary>
		///<param name="permType">The type of permission to be logged in the security log.</param>
		///<param name="patNum">The PatNum for the patient associated to the security log. Can be 0.</param>
		///<param name="listSecurityLogs">A list of the security log text that should be inserted.</param>
		public static void MakeLogEntries(Permissions permType,long patNum,List<string> listSecurityLogs) {
			if(listSecurityLogs==null || listSecurityLogs.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),permType,patNum,listSecurityLogs);
				return;
			}
			foreach(string securityLogEntry in listSecurityLogs) {
				MakeLogEntry(permType,patNum,securityLogEntry);
			}
		}

		///<summary>PatNum can be 0.</summary>
		public static void MakeLogEntry(Permissions permType,long patNum,string logText) {
			//No need to check RemotingRole; no call to db.
			MakeLogEntry(permType,patNum,logText,0,LogSource,DateTime.MinValue);
		}

		///<summary>Used when the security log needs to be identified by a particular source.  PatNum can be 0.</summary>
		public static void MakeLogEntry(Permissions permType,long patNum,string logText,LogSources logSource) {
			//No need to check RemotingRole; no call to db.
			MakeLogEntry(permType,patNum,logText,0,logSource,DateTime.MinValue);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.</summary>
		public static void MakeLogEntry(Permissions permType,long patNum,string logText,long fKey,DateTime DateTPrevious) {
			//No need to check RemotingRole; no call to db.
			MakeLogEntry(permType,patNum,logText,fKey,LogSource,DateTPrevious);
		}
		///<summary>Pass in device name, used in eClipboard</summary>
		public static void MakeLogEntry(Permissions permType,long patNum,string logText,long fKey,DateTime DateTPrevious,string deviceName) {
			//No need to check RemotingRole; no call to db.
			MakeLogEntry(permType,patNum,logText,fKey,LogSource,0,0,DateTPrevious,deviceName);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.</summary>
		public static void MakeLogEntry(Permissions permType,long patNum,string logText,long fKey,LogSources logSource,DateTime DateTPrevious) {
			MakeLogEntry(permType,patNum,logText,fKey,logSource,0,0,DateTPrevious);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.</summary>
		public static void MakeLogEntry(Permissions permType,long patNum,string logText,long fKey,LogSources logSource,long defNum,long defNumError,
			DateTime DateTPrevious) 
		{
			//No need to check RemotingRole; no call to db.
			SecurityLog securityLog=MakeLogEntryNoInsert(permType,patNum,logText,fKey,logSource,defNum,defNumError,DateTPrevious);
			MakeLogEntry(securityLog);
		}

		public static void MakeLogEntry(Permissions permType,long patNum,string logText,long fKey,LogSources logSource,long defNum,long defNumError,
			DateTime DateTPrevious,string deviceName) {
			SecurityLog securityLog=MakeLogEntryNoInsert(permType,patNum,logText,fKey,logSource,deviceName,defNum,defNumError,DateTPrevious);
			MakeLogEntry(securityLog);
		}

		///<summary>Can pass in a device name, used with eClipboard</summary>
		public static SecurityLog MakeLogEntryNoInsert(Permissions permType,long patNum,string logText,long fKey,LogSources logSource,string deviceName,
			long defNum = 0,long defNumError = 0,DateTime DateTPrevious = default(DateTime)) {
			SecurityLog securityLog=MakeLogEntryNoInsert(permType,patNum,logText,fKey,logSource,defNum,defNumError,DateTPrevious);
			securityLog.CompName=deviceName;
			return securityLog;
		}

		///<summary>Take a SecurityLog object to save to the database. Creates a SecurityLogHash object as well.</summary>
		public static void MakeLogEntry(SecurityLog secLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),secLog);
				return;
			}
			secLog.SecurityLogNum=SecurityLogs.Insert(secLog);
			SecurityLogHashes.InsertSecurityLogHash(secLog.SecurityLogNum);//uses db date/time
			if(secLog.PermType==Permissions.AppointmentCreate) {
				EntryLogs.Insert(new EntryLog(secLog.UserNum,EntryLogFKeyType.Appointment,secLog.FKey,secLog.LogSource));
			}
		}
		
		///<summary>Creates security log entries for all that PatNums passed in.</summary>
		public static void MakeLogEntry(Permissions permType,List<long> listPatNums,string logText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),permType,listPatNums,logText);
				return;
			}
			List<SecurityLog> listSecLogs=new List<SecurityLog>();
			foreach(long patNum in listPatNums) {
				SecurityLog secLog=MakeLogEntryNoInsert(permType,patNum,logText,0,LogSource);
				SecurityLogs.Insert(secLog);
				listSecLogs.Add(secLog);
			}
			List<SecurityLogHash> listHash=new List<SecurityLogHash>();
			List<EntryLog> listEntries=new List<EntryLog>();
			listSecLogs=SecurityLogs.GetMany(SQLWhere.CreateIn(nameof(SecurityLog.SecurityLogNum),
				listSecLogs.Select(x => x.SecurityLogNum).ToList()));
			foreach(SecurityLog log in listSecLogs) {
				SecurityLogHash secLogHash=new SecurityLogHash();
				secLogHash.SecurityLogNum=log.SecurityLogNum;
				secLogHash.LogHash=SecurityLogHashes.GetHashString(log);
				listHash.Add(secLogHash);
				if(log.PermType==Permissions.AppointmentCreate) {
					listEntries.Add(new EntryLog(log.UserNum,EntryLogFKeyType.Appointment,log.FKey,log.LogSource));
				}
			}
			EntryLogs.InsertMany(listEntries);
			SecurityLogHashes.InsertMany(listHash);
		}

		///<summary>Takes a foreign key to a table associated with that PermType.  PatNum can be 0.  Returns the created SecurityLog object.  Does not perform an insert.</summary>
		public static SecurityLog MakeLogEntryNoInsert(Permissions permType,long patNum,string logText,long fKey,LogSources logSource,long defNum=0,
			long defNumError=0,DateTime DateTPrevious=default(DateTime)) 
		{
			//No need to check RemotingRole; no call to db.
			SecurityLog securityLog=new SecurityLog();
			securityLog.PermType=permType;
			securityLog.UserNum=Security.CurUser.UserNum;
			securityLog.LogText=logText;
			securityLog.CompName=Security.CurComputerName;
			securityLog.PatNum=patNum;
			securityLog.FKey=fKey;
			securityLog.LogSource=logSource;
			securityLog.DefNum=defNum;
			securityLog.DefNumError=defNumError;
			securityLog.DateTPrevious=DateTPrevious;
			return securityLog;
		}

		///<summary>Used when making a security log from a remote server, possibly with multithreaded connections.</summary>
		public static void MakeLogEntryNoCache(Permissions permType,long patnum,string logText) {
			MakeLogEntryNoCache(permType,patnum,logText,0,LogSource);
		}

		///<summary>Used when making a security log from a remote server, possibly with multithreaded connections.</summary>
		public static void MakeLogEntryNoCache(Permissions permType,long patnum,string logText,long userNum,LogSources source) {
			SecurityLog securityLog=new SecurityLog();
			securityLog.PermType=permType;
			securityLog.UserNum=userNum;
			securityLog.LogText=logText;
			securityLog.CompName=Security.CurComputerName;
			securityLog.PatNum=patnum;
			securityLog.FKey=0;
			securityLog.LogSource=source;
			securityLog.SecurityLogNum=SecurityLogs.InsertNoCache(securityLog);
			SecurityLogHashes.InsertSecurityLogHashNoCache(securityLog.SecurityLogNum);
		}

		///<summary>Insertion logic that doesn't use the cache. Has special cases for generating random PK's and handling Oracle insertions.</summary>
		public static long InsertNoCache(SecurityLog securityLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),securityLog);
			}
			return Crud.SecurityLogCrud.InsertNoCache(securityLog);
		}

		///<summary>Adds changes made to certain procedure fields to passed security logtext. These fields are ProcDate, Surf, ToothNum, and ToothRange. More fields can be added at a later time.</summary>
		public static string AppendProcCompleteEditSecurityLog(Procedure procNew, Procedure procOld) {
			string logText="";
			if(procNew==null || procOld==null) {
				return logText;
			}
			if(procNew.ProcDate!=procOld.ProcDate) {
				logText+=Lans.g("Procedures","\nProcDate changed from ")+procOld.ProcDate.ToShortDateString()+Lans.g("Procedures"," to ")+procNew.ProcDate.ToShortDateString();
			}
			if(procNew.Surf!=procOld.Surf) {    //because Surf could be changed to or from blank, print "none" instead
				logText+=Lans.g("Procedures","\nSurf changed from ");
				if(procNew.Surf!=null) {
					logText+=procOld.Surf+" to ";
				}
				else {
					logText+=Lans.g("Procedures","none to ");
				}
				if(procNew.Surf!=null) {
					logText+=procNew.Surf;
				}
				else {
					logText+=Lans.g("Procedures","none");
				}
			}
			if(procNew.ToothNum!=procOld.ToothNum) { //because ToothNum could be changed to or from blank, print "none" instead
				logText+=Lans.g("Procedures","\nToothNum changed from ");
				if(procNew.ToothNum!=null) {
					logText+=procOld.ToothNum+" to ";
				}
				else {
					logText+=Lans.g("Procedures","none to");
				}
				if(procNew.ToothNum!=null) {
					logText+=procNew.ToothNum;
				}
				else {
					logText+=Lans.g("Procedures","none");
				}
			}
			if(procNew.ToothRange!=procOld.ToothRange) {
				logText+=Lans.g("Procedures","\nToothRange changed from ")+procOld.ToothRange+Lans.g("Procedures"," to ")+procNew.ToothRange;
			}
			return logText;
		}
	}
}