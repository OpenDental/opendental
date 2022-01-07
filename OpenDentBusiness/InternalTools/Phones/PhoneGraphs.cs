using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PhoneGraphs{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add RefreshCache and FillCache 
		//to the Cache.cs file with all the other Cache types.

		///<summary>A list of all PhoneGraphs.</summary>
		private static List<PhoneGraph> listt;

		///<summary>A list of all PhoneGraphs.</summary>
		public static List<PhoneGraph> Listt{
			get {
				if(listt==null) {
					RefreshCache();
				}
				return listt;
			}
			set {
				listt=value;
			}
		}

		///<summary></summary>
		public static DataTable RefreshCache(){
			//No need to check RemotingRole; Calls GetTableRemotelyIfNeeded().
			string command="SELECT * FROM phonegraph ORDER BY ItemOrder";//stub query probably needs to be changed
			DataTable table=Cache.GetTableRemotelyIfNeeded(MethodBase.GetCurrentMethod(),command);
			table.TableName="PhoneGraph";
			FillCache(table);
			return table;
		}

		///<summary></summary>
		public static void FillCache(DataTable table){
			//No need to check RemotingRole; no call to db.
			listt=Crud.PhoneGraphCrud.TableToList(table);
		}
		#endregion
		*/

		public static PhoneGraph GetOne(long employeeNum,DateTime dateEntry) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PhoneGraph>(MethodBase.GetCurrentMethod(),employeeNum,dateEntry);
			}
			string command="SELECT * FROM phonegraph WHERE EmployeeNum="+POut.Long(employeeNum)+" "
			+"AND DateEntry="+POut.Date(dateEntry);
			return Crud.PhoneGraphCrud.SelectOne(command);
		}

		///<summary></summary>
		public static List<PhoneGraph> GetAllForEmployeeNum(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneGraph>>(MethodBase.GetCurrentMethod(),employeeNum);
			}
			string command="SELECT * FROM phonegraph WHERE EmployeeNum="+POut.Long(employeeNum)+" "
			+"ORDER BY DateEntry";
			return Crud.PhoneGraphCrud.SelectMany(command);
		}

		public static List<PhoneGraph> GetAllForDate(DateTime dateGet) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneGraph>>(MethodBase.GetCurrentMethod(),dateGet);
			}
			string command="SELECT * FROM phonegraph WHERE DateEntry="+POut.Date(dateGet);
			return Crud.PhoneGraphCrud.SelectMany(command);
		}

		///<summary>Only used when we have just checked to make sure another one does not already exist.</summary>
		public static long Insert(PhoneGraph phoneGraph){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				phoneGraph.PhoneGraphNum=Meth.GetLong(MethodBase.GetCurrentMethod(),phoneGraph);
				return phoneGraph.PhoneGraphNum;
			}
			return Crud.PhoneGraphCrud.Insert(phoneGraph);
		}

		///<summary></summary>
		public static void Update(PhoneGraph phoneGraph){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneGraph);
				return;
			}
			Crud.PhoneGraphCrud.Update(phoneGraph);
		}
		
		///<summary>Concurrency is not really an issue with this table.  Last to save wins.  We just don't want to have two rows, so an insert deletes any existing, just to be sure.</summary>
		public static void InsertOrUpdate(PhoneGraph phoneGraph) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneGraph);
				return;
			}
			string command;
			if(phoneGraph.IsNew){
				command="DELETE FROM phonegraph "
					+"WHERE EmployeeNum="+POut.Long(phoneGraph.EmployeeNum)+" "
					+"AND DateEntry="+POut.Date(phoneGraph.DateEntry);
				Db.NonQ(command);
				Insert(phoneGraph); 
			}
			else{
				Update(phoneGraph);
			}
		}

		///<summary>Some may already be entered as exceptions to the default. We will fill in the gaps here. This will only be done for today's date.  Once Today has passed the opportunity to fill the gaps has passed. This results in an accurate history, even as employees come and go.</summary>
		public static void AddMissingEntriesForToday(List<PhoneEmpDefault> listPhoneEmpDefaults) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listPhoneEmpDefaults);
				return;
			}
			DateTime date=DateTime.Today;
			//if(CodeBase.ODBuild.IsDebug() && Environment.MachineName=="JORDANHOME"){//be very careful not to do this to our main db
			//	date=new DateTime(2020,03,18);
			//}
			List<PhoneGraph> listPhoneGraphs=GetAllForDate(date);//get existing, including overrides 
			List<Schedule> listSchedules=Schedules.GetDayList(date);
			//loop through all defaults and check if there are overrides added
			for(int iPed=0;iPed<listPhoneEmpDefaults.Count;iPed++) {
				PhoneEmpDefault ped=listPhoneEmpDefaults[iPed];
				bool hasPhoneGraphEntry=false;
				//we have a default, now loop through all known overrides and find a match
				for(int iPG=0;iPG<listPhoneGraphs.Count;iPG++) {
					PhoneGraph pg=listPhoneGraphs[iPG];
					if(ped.EmployeeNum==listPhoneGraphs[iPG].EmployeeNum) {//found a match so no op necessary for this employee
						hasPhoneGraphEntry=true;
						break;
					}
				}
				if(hasPhoneGraphEntry) {//no entry needed, it's already there
					continue;
				}
				//does employee have a schedule table entry for this date
				bool hasScheduleEntry=false;
				for(int iSch=0;iSch<listSchedules.Count;iSch++) {
					Schedule schedule=listSchedules[iSch];
					if(ped.EmployeeNum==listSchedules[iSch].EmployeeNum) {//found a match so no op necessary for this employee
						hasScheduleEntry=true;
						break;
					}
				}
				if(!hasScheduleEntry) { //no entry needed if not on the schedule
					continue;
				}
				//employee is on the schedule but does not have a phonegraph entry, so create one
				PhoneGraph pgNew=new PhoneGraph();
				pgNew.EmployeeNum=ped.EmployeeNum;
				pgNew.DateEntry=date;
				pgNew.IsGraphed=ped.IsGraphed;
				Insert(pgNew);
			}
		}

		///<summary></summary>
		public static void Delete(long phoneGraphNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneGraphNum);
				return;
			}
			Crud.PhoneGraphCrud.Delete(phoneGraphNum);
		}

		///<summary>Delete all entries for this date. Used by internal 'Shared Projects Subversion' project which back-fills PhoneGraph entries for past dates.</summary>
		public static long DeleteDate(DateTime dateEntry) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),dateEntry);
			}
			string command="DELETE FROM phonegraph WHERE DateEntry = "+POut.Date(dateEntry);
			return Db.NonQ(command);
		}

		///<summary></summary>
		public static string GetCommaDelimStringTimes(PhoneGraph phoneGraph) {
			string str="";
			if(phoneGraph.DateTimeStart1.Year>1880){
				str+=phoneGraph.DateTimeStart1.ToShortTimeString()+"-"+phoneGraph.DateTimeStop1.ToShortTimeString();
			}
			if(phoneGraph.DateTimeStart2.Year>1880){
				str+=","+phoneGraph.DateTimeStart2.ToShortTimeString()+"-"+phoneGraph.DateTimeStop2.ToShortTimeString();
			}
			return str;
		}


	}
}