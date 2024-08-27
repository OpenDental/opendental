using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ChildRoomLogs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ChildRoomLogCache : CacheListAbs<ChildRoomLog> {
			protected override List<ChildRoomLog> GetCacheFromDb() {
				string command="SELECT * FROM childroomlog";
				return Crud.ChildRoomLogCrud.SelectMany(command);
			}
			protected override List<ChildRoomLog> TableToList(DataTable table) {
				return Crud.ChildRoomLogCrud.TableToList(table);
			}
			protected override ChildRoomLog Copy(ChildRoomLog childRoomLog) {
				return childRoomLog.Copy();
			}
			protected override DataTable ListToTable(List<ChildRoomLog> listChildRoomLogs) {
				return Crud.ChildRoomLogCrud.ListToTable(listChildRoomLogs,"ChildRoomLog");
			}
			protected override void FillCacheIfNeeded() {
				ChildRoomLogs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ChildRoomLogCache _childRoomLogCache=new ChildRoomLogCache();

		public static void ClearCache() {
			_childRoomLogCache.ClearCache();
		}

		public static List<ChildRoomLog> GetDeepCopy(bool isShort=false) {
			return _childRoomLogCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _childRoomLogCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ChildRoomLog> match,bool isShort=false) {
			return _childRoomLogCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ChildRoomLog> match,bool isShort=false) {
			return _childRoomLogCache.GetFindIndex(match,isShort);
		}

		public static ChildRoomLog GetFirst(bool isShort=false) {
			return _childRoomLogCache.GetFirst(isShort);
		}

		public static ChildRoomLog GetFirst(Func<ChildRoomLog,bool> match,bool isShort=false) {
			return _childRoomLogCache.GetFirst(match,isShort);
		}

		public static ChildRoomLog GetFirstOrDefault(Func<ChildRoomLog,bool> match,bool isShort=false) {
			return _childRoomLogCache.GetFirstOrDefault(match,isShort);
		}

		public static ChildRoomLog GetLast(bool isShort=false) {
			return _childRoomLogCache.GetLast(isShort);
		}

		public static ChildRoomLog GetLastOrDefault(Func<ChildRoomLog,bool> match,bool isShort=false) {
			return _childRoomLogCache.GetLastOrDefault(match,isShort);
		}

		public static List<ChildRoomLog> GetWhere(Predicate<ChildRoomLog> match,bool isShort=false) {
			return _childRoomLogCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_childRoomLogCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_childRoomLogCache.FillCacheFromTable(table);
				return table;
			}
			return _childRoomLogCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<ChildRoomLog> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM childroomlog WHERE PatNum = "+POut.Long(patNum);
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}
		
		///<summary>Gets one ChildRoomLog from the db.</summary>
		public static ChildRoomLog GetOne(long childRoomLogNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ChildRoomLog>(MethodBase.GetCurrentMethod(),childRoomLogNum);
			}
			return Crud.ChildRoomLogCrud.SelectOne(childRoomLogNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static void Update(ChildRoomLog childRoomLog){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childRoomLog);
				return;
			}
			Crud.ChildRoomLogCrud.Update(childRoomLog);
		}
		///<summary></summary>
		public static void Delete(long childRoomLogNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),childRoomLogNum);
				return;
			}
			Crud.ChildRoomLogCrud.Delete(childRoomLogNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/

		///<summary>Get all the logs for a specified ChildRoom and filtered by the given date.</summary>
		public static List<ChildRoomLog> GetChildRoomLogs(long childRoomNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),childRoomNum,date);
			}
			string command="SELECT * FROM childroomlog WHERE ChildRoomNum="+POut.Long(childRoomNum)
				+" AND DATE(DateTDisplayed)="+POut.Date(date)
				+" ORDER BY DateTDisplayed";
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}

		///<summary>Returns all child logs for a given date.</summary>
		public static List<ChildRoomLog> GetAllChildrenForDate(DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),date);
			}
			string command="SELECT * FROM childroomlog WHERE DATE(DateTDisplayed)="+POut.Date(date)//Specify date
				+" AND ChildNum!=0";//Only child logs
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}

		///<summary>Returns all the employee logs for a given date.</summary>
		public static List<ChildRoomLog> GetAllEmployeesForDate(DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),date);
			}
			string command="SELECT * FROM childroomlog WHERE DATE(DateTDisplayed)="+POut.Date(date)
				+" AND EmployeeNum!=0";//Only employee logs
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}

		///<summary>Returns all logs for a specific child for a given date.</summary>
		public static List<ChildRoomLog> GetAllLogsForChild(long childNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),childNum,date);
			}
			string command="SELECT * FROM childroomlog WHERE ChildNum="+POut.Long(childNum)
				+" AND DATE(DateTDisplayed)="+POut.Date(date);
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}

		///<summary>Returns all logs for a specific employee for a given date.</summary>
		public static List<ChildRoomLog> GetAllLogsForEmployee(long employeeNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ChildRoomLog>>(MethodBase.GetCurrentMethod(),employeeNum,date);
			}
			string command="SELECT * FROM childroomlog WHERE EmployeeNum="+POut.Long(employeeNum)
				+" AND DATE(DateTDisplayed)="+POut.Date(date);
			return Crud.ChildRoomLogCrud.SelectMany(command);
		}

		///<summary>Get the most recent previous ratio change for a specific child room and date. Returns 0 if no previous ratio change was found.</summary>
		public static double GetPreviousRatio(long childRoomNum,DateTime date) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),childRoomNum,date);
			}
			string command="SELECT * FROM childroomlog WHERE ChildRoomNum="+POut.Long(childRoomNum)//Specify room
				+" AND RatioChange!=0"//Specify RatioChange entry
				+" AND DateTDisplayed <"+POut.Date(date)//Find previous entries
				+" ORDER BY DateTDisplayed DESC"//Order so that the first entry is the most recent one
				+" LIMIT 1";
			ChildRoomLog childRoomLog=Crud.ChildRoomLogCrud.SelectOne(command);
			if(childRoomLog==null) {
				return 0;
			}
			return childRoomLog.RatioChange;
		}

		///<summary></summary>
		public static long Insert(ChildRoomLog childRoomLog){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				childRoomLog.ChildRoomLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),childRoomLog);
				return childRoomLog.ChildRoomLogNum;
			}
			return Crud.ChildRoomLogCrud.Insert(childRoomLog);
		}

		///<summary>Creates a ChildRoomLog leaving entry if the most recent log in the given list IsComing. The passed-in list should contain logs for one child or teacher.</summary>
		public static void CreateChildRoomLogLeaving(List<ChildRoomLog> listChildRoomLogs) {
			//No need to check MiddleTierRole; no call to db.
			if(listChildRoomLogs.Count==0) {//No need for leaving entry if the child has no logs for today
				return;
			}
			//OrderBy PK in case there are multiple entries with the same time
			ChildRoomLog childRoomLogOld=listChildRoomLogs.OrderByDescending(x => x.ChildRoomLogNum).First();
			if(!childRoomLogOld.IsComing) {//Most recent entry is already leaving
				return;
			}
			ChildRoomLog childRoomLog=new ChildRoomLog();
			childRoomLog.DateTEntered=DateTime.Now;
			childRoomLog.DateTDisplayed=DateTime.Now;
			childRoomLog.IsComing=false;
			childRoomLog.ChildRoomNum=childRoomLogOld.ChildRoomNum;
			if(childRoomLogOld.ChildNum!=0) {//Child entry
				childRoomLog.ChildNum=childRoomLogOld.ChildNum;
			}
			else {//Employee/teacher entry
				childRoomLog.EmployeeNum=childRoomLogOld.EmployeeNum;
			}
			Insert(childRoomLog);
			Signalods.SetInvalid(InvalidType.Children);
		}

		///<summary>For mixed age groups. Oregon law has specific numbers of teachers required for a classroom that has children over and under two years old. These requirements are outlined here https://www.oregon.gov/delc/providers/CCLD_Library/CCLD-0084-Rules-for-Certified-Child-Care-Centers-EN.pdf in the table on page 46. This method takes two paremeters, the total number of children and the number of children under the age of two. Returns the number of teachers required based on the table.</summary>
		public static int GetNumberTeachersMixed(int totalChildren,int childrenUnderTwo) {
			//No need to check MiddleTierRole; no call to db.
			if(totalChildren<1) {
				return 0;//No teachers required if there are no children present
			}
			if(totalChildren>16) {
				//If for some reason the max of 16 is exceeded, return -1 to indicate something is wrong
				return -1;
			}
			int teachersRequired=0;
			if(childrenUnderTwo==0) {
				if(totalChildren<11) {
					teachersRequired=1;
				}
				else {
					teachersRequired=2;
				}
			}
			else if(childrenUnderTwo==1) {
				if(totalChildren<9) {
					teachersRequired=1;
				}
				else {
					teachersRequired=2;
				}
			}
			else if(childrenUnderTwo==2) {
				if(totalChildren<8) {
					teachersRequired=1;
				}
				else {
					teachersRequired=2;
				}
			}
			else if(childrenUnderTwo==3) {
				if(totalChildren<7) {
					teachersRequired=1;
				}
				else {
					teachersRequired=2;
				}
			}
			else if(childrenUnderTwo==4) {
				if(totalChildren<15) {
					teachersRequired=2;
				}
				else {
					teachersRequired=3;
				}
			}
			else if(childrenUnderTwo==5) {
				if(totalChildren<13) {
					teachersRequired=2;
				}
				else {
					teachersRequired=3;
				}
			}
			else if(childrenUnderTwo==6) {
				if(totalChildren<12) {
					teachersRequired=2;
				}
				else {
					teachersRequired=3;
				}
			}
			else if(childrenUnderTwo==7) {
				if(totalChildren<11) {
					teachersRequired=2;
				}
				else {
					teachersRequired=3;
				}
			}
			else if(childrenUnderTwo==8) {
				if(totalChildren<9) {
					teachersRequired=2;
				}
				else {
					teachersRequired=3;
				}
			}
			else if(childrenUnderTwo==9) {
				teachersRequired=3;
			}
			else if(childrenUnderTwo==10) {
				if(totalChildren<16) {
					teachersRequired=3;
				}
				else {
					teachersRequired=4;
				}
			}
			else if(childrenUnderTwo==11) {
				if(totalChildren<15) {
					teachersRequired=3;
				}
				else {
					teachersRequired=4;
				}
			}
			else if(childrenUnderTwo==12) {
				if(totalChildren<13) {
					teachersRequired=3;
				}
				else {
					teachersRequired=4;
				}
			}
			else {
				teachersRequired=4;//13 or higher childrenUnderTwo requires 4 teachers
			}
			return teachersRequired;
		}

	}
}