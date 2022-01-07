using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Employees{
		#region Update
		///<summary>Will throw exception if the employee has no name.</summary>
		public static void Update(Employee Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			if(Cur.LName=="" && Cur.FName=="") {
				throw new ApplicationException(Lans.g("FormEmployeeEdit","Must include either first name or last name"));
			}
			Crud.EmployeeCrud.Update(Cur);
		}

		///<summary>Will throw exception if the employee has no name.</summary>
		public static void UpdateChanged(Employee employee,Employee employeeOld, bool doInvalidate=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employee,employeeOld, doInvalidate);
				return;
			}
			if(employee.LName=="" && employee.FName=="") {
				throw new ApplicationException(Lans.g("FormEmployeeEdit","Must include either first name or last name"));
			}
			if(Crud.EmployeeCrud.Update(employee,employeeOld) && doInvalidate) {
				Signalods.SetInvalid(InvalidType.Employees);
			}
		}

		///<summary>Updates the employee's ClockStatus if necessary based on their clock events. This method handles future clock events as having
		///already occurred. Ex: If I clock out for home at 6:00 but edit my time card to say 7:00, at 6:30 my status will say Home.</summary>
		public static void UpdateClockStatus(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum);
				return;
			}
			//Get the last clockevent for the employee. Will include clockevent with "in" before NOW, and "out" anytime before 23:59:59 of TODAY.
			string command = @"SELECT * FROM clockevent 
				WHERE TimeDisplayed2<="+DbHelper.DateAddSecond(DbHelper.DateAddDay(DbHelper.Curdate(),"1"),"-1")+" AND TimeDisplayed1<="+DbHelper.Now()+@"
				AND EmployeeNum="+POut.Long(employeeNum)+@"
				ORDER BY IF(YEAR(TimeDisplayed2) < 1880,TimeDisplayed1,TimeDisplayed2) DESC";
			command=DbHelper.LimitOrderBy(command,1);
			ClockEvent clockEvent=Crud.ClockEventCrud.SelectOne(command);
			Employee employee=GetEmp(employeeNum);
			Employee employeeOld=employee.Copy();
			if(clockEvent!=null && clockEvent.TimeDisplayed2>DateTime.Now) {//Future time manual clock out.
				employee.ClockStatus=Lans.g("ContrStaff","Manual Entry");
			}
			else if(clockEvent==null //Employee has never clocked in
				|| (clockEvent.TimeDisplayed2.Year > 1880 && clockEvent.ClockStatus==TimeClockStatus.Home))//Clocked out for home
			{
				employee.ClockStatus=Lans.g("enumTimeClockStatus",TimeClockStatus.Home.ToString());
			}
			else if(clockEvent.TimeDisplayed2.Year > 1880 && clockEvent.ClockStatus==TimeClockStatus.Lunch) {//Clocked out for lunch
				employee.ClockStatus=Lans.g("enumTimeClockStatus",TimeClockStatus.Lunch.ToString());
			}
			else if(clockEvent.TimeDisplayed1.Year > 1880 && clockEvent.TimeDisplayed2.Year < 1880 && clockEvent.ClockStatus==TimeClockStatus.Break) {
				employee.ClockStatus=Lans.g("enumTimeClockStatus",TimeClockStatus.Break.ToString());
			}
			else if(clockEvent.TimeDisplayed2.Year > 1880 && clockEvent.ClockStatus==TimeClockStatus.Break) {//Clocked back in from break
				employee.ClockStatus=Lans.g("ContrStaff","Working");
			}
			else {//The employee has not clocked out yet.
				employee.ClockStatus=Lans.g("ContrStaff","Working");
			}
			Crud.EmployeeCrud.Update(employee,employeeOld);
		}
		#endregion

		#region CachePattern
		private class EmployeeCache : CacheListAbs<Employee> {
			protected override List<Employee> GetCacheFromDb() {
				string command="SELECT * FROM employee ORDER BY IsHidden,FName,LName";
				return Crud.EmployeeCrud.SelectMany(command);
			}
			protected override List<Employee> TableToList(DataTable table) {
				return Crud.EmployeeCrud.TableToList(table);
			}
			protected override Employee Copy(Employee employee) {
				return employee.Copy();
			}
			protected override DataTable ListToTable(List<Employee> listEmployees) {
				return Crud.EmployeeCrud.ListToTable(listEmployees,"Employee");
			}
			protected override void FillCacheIfNeeded() {
				Employees.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Employee employee) {
				return !employee.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EmployeeCache _employeeCache=new EmployeeCache();

		public static Employee GetFirstOrDefault(Func<Employee,bool> match,bool isShort=false) {
			return _employeeCache.GetFirstOrDefault(match,isShort);
		}

		public static List<Employee> GetDeepCopy(bool isShort=false) {
			return _employeeCache.GetDeepCopy(isShort);
		}

		public static List<Employee> GetWhere(Predicate<Employee> match,bool isShort = false) {
			return _employeeCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_employeeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_employeeCache.FillCacheFromTable(table);
				return table;
			}
			return _employeeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Instead of using the cache, which sorts by FName, LName.</summary>
		public static List<Employee> GetForTimeCard() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Employee>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM employee WHERE IsHidden=0 ORDER BY LName,Fname";
			return Crud.EmployeeCrud.SelectMany(command);
		}

		/*public static Employee[] GetListByExtension(){
			if(ListShort==null){
				return new Employee[0];
			}
			Employee[] arrayCopy=new Employee[ListShort.Length];
			ListShort.CopyTo(arrayCopy,0);
			int[] arrayKeys=new int[ListShort.Length];
			for(int i=0;i<ListShort.Length;i++){
				arrayKeys[i]=ListShort[i].PhoneExt;
			}
			Array.Sort(arrayKeys,arrayCopy);
			//List<Employee> retVal=new List<Employee>(ListShort);
			//retVal.Sort(
			return arrayCopy;
		}*/

		///<summary></summary>
		public static long Insert(Employee Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.EmployeeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.EmployeeNum;
			}if(Cur.LName=="" && Cur.FName=="") {
				throw new ApplicationException(Lans.g("FormEmployeeEdit","Must include either first name or last name"));
			}
			return Crud.EmployeeCrud.Insert(Cur);
		}

		///<summary>Surround with try-catch</summary>
		public static void Delete(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum);
				return;
			}
			//appointment.Assistant will not block deletion
			//schedule.EmployeeNum will not block deletion
			string command="SELECT COUNT(*) FROM clockevent WHERE EmployeeNum="+POut.Long(employeeNum);
			if(Db.GetCount(command)!="0"){
				throw new ApplicationException(Lans.g("FormEmployeeSelect",
					"Not allowed to delete employee because of attached clock events."));
			}
			command="SELECT COUNT(*) FROM timeadjust WHERE EmployeeNum="+POut.Long(employeeNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("FormEmployeeSelect",
					"Not allowed to delete employee because of attached time adjustments."));
			}
			command="SELECT COUNT(*) FROM userod WHERE EmployeeNum="+POut.Long(employeeNum);
			if(Db.GetCount(command)!="0") {
				throw new ApplicationException(Lans.g("FormEmployeeSelect",
					"Not allowed to delete employee because of attached user."));
			}
			command="UPDATE appointment SET Assistant=0 WHERE Assistant="+POut.Long(employeeNum);
			Db.NonQ(command);
			command="SELECT ScheduleNum FROM schedule WHERE EmployeeNum="+POut.Long(employeeNum);
			DataTable table=Db.GetTable(command);
			List<string> listScheduleNums=new List<string>();//Used for deleting scheduleops below
			for(int i=0;i<table.Rows.Count;i++) {
				//Add entry to deletedobjects table if it is a provider schedule type
				DeletedObjects.SetDeleted(DeletedObjectType.ScheduleProv,PIn.Long(table.Rows[i]["ScheduleNum"].ToString()));
				listScheduleNums.Add(table.Rows[i]["ScheduleNum"].ToString());
			}
			if(listScheduleNums.Count>0) {
				command="DELETE FROM scheduleop WHERE ScheduleNum IN("+POut.String(String.Join(",",listScheduleNums))+")";
				Db.NonQ(command);
			}
			//command="DELETE FROM scheduleop WHERE ScheduleNum IN(SELECT ScheduleNum FROM schedule WHERE EmployeeNum="+POut.Long(employeeNum)+")";
			//Db.NonQ(command);
			command="DELETE FROM schedule WHERE EmployeeNum="+POut.Long(employeeNum);
			Db.NonQ(command);
			command= "DELETE FROM employee WHERE EmployeeNum ="+POut.Long(employeeNum);
			Db.NonQ(command);
			command="DELETE FROM timecardrule WHERE EmployeeNum="+POut.Long(employeeNum);
			Db.NonQ(command);
		}

		/*
		///<summary>Returns LName,FName MiddleI for the provided employee.</summary>
		public static string GetNameLF(Employee emp){
			return(emp.LName+", "+emp.FName+" "+emp.MiddleI);
		}

		///<summary>Loops through List to find matching employee, and returns LName,FName MiddleI.</summary>
		public static string GetNameLF(int employeeNum){
			for(int i=0;i<ListLong.Length;i++){
				if(ListLong[i].EmployeeNum==employeeNum){
					return GetNameLF(ListLong[i]);
				}
			}
			return "";
		}*/

		///<summary>Returns FName MiddleI LName for the provided employee.</summary>
		public static string GetNameFL(Employee emp) {
			//No need to check RemotingRole; no call to db.
			return (emp.FName+" "+emp.MiddleI+" "+emp.LName);
		}

		///<summary>Returns FNameL, where L is the first letter of the last name.</summary>
		public static string GetNameCondensed(Employee emp) {
			//No need to check RemotingRole; no call to db.
			string retVal=emp.FName;
			if(emp.LName.Length>0){
				retVal+=emp.LName.Substring(0,1);
			}
			return retVal;
		}

		///<summary>Loops through List to find matching employee, and returns FName MiddleI LName.</summary>
		public static string GetNameFL(long employeeNum) {
			//No need to check RemotingRole; no call to db.
			Employee employee=GetFirstOrDefault(x => x.EmployeeNum==employeeNum);
			//if(isCondensed){
			//	return (employee==null ? "" : GetNameCondensed(employee));
			//}
			return (employee==null ? "" : GetNameFL(employee));
		}

		///<summary>Loops through List to find matching employee, and returns first 2 letters of first name.  Will later be improved with abbr field.</summary>
		public static string GetAbbr(long employeeNum) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			Employee employee=GetFirstOrDefault(x => x.EmployeeNum==employeeNum);
			if(employee!=null) {
				retVal=employee.FName;
				if(retVal.Length > 2) {
					retVal=retVal.Substring(0,2);
				}
			}
			return retVal;
		}

		///<summary>From cache</summary>
		public static Employee GetEmp(long employeeNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.EmployeeNum==employeeNum);
		}

		///<summary>Find formatted name in list.  Takes in a name that was previously formatted by Employees.GetNameFL and finds a match in the list.  If no match is found then returns null.</summary>
		public static Employee GetEmp(string nameFL,List<Employee> employees) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<employees.Count;i++) {
				if(GetNameFL(employees[i])==nameFL) {
					return employees[i];
				}
			}
			return null;
		}   
		
		///<summary>From cache</summary>
		public static List<Employee> GetEmps(List<long> employeeNums) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => employeeNums.Contains(x.EmployeeNum));
		}

		///<summary>Gets all employees associated to users that have a clinic set to the clinic passed in.  Passing in 0 will get a list of employees not assigned to any clinic.  Gets employees from the cache which is sorted by FName, LastName.</summary>
		public static List<Employee> GetEmpsForClinic(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			return GetEmpsForClinic(clinicNum,false);
		}

		///<summary>Gets all the employees for a specific clinicNum, according to their associated user.  Pass in a clinicNum of 0 to get the list of unassigned or "all" employees (depending on isAll flag).  In addition to setting clinicNum to 0, set isAll true to get a list of all employees or false to get a list of employees that are not associated to any clinics.  Always gets the list of employees from the cache which is sorted by FName, LastName.</summary>
		public static List<Employee> GetEmpsForClinic(long clinicNum,bool isAll,bool getUnassigned=false) {
			//No need to check RemotingRole; no call to db.
			List<Employee> listEmpsShort=Employees.GetDeepCopy(true);
			if(!PrefC.HasClinicsEnabled || (clinicNum==0 && isAll)) {//Simply return all employees.
				return listEmpsShort;
			}
			List<Employee> listEmpsWithClinic=new List<Employee>();
			List<Employee> listEmpsUnassigned=new List<Employee>();
			Dictionary<long,List<UserClinic>> dictUserClinics=new Dictionary<long, List<UserClinic>>();
			foreach(Employee empCur in listEmpsShort) {
				List<Userod> listUsers=Userods.GetUsersByEmployeeNum(empCur.EmployeeNum);
				if(listUsers.Count==0) {
					listEmpsUnassigned.Add(empCur);
					continue;
				}
				foreach(Userod userCur in listUsers) {//At this point we know there is at least one Userod associated to this employee.
					if(userCur.ClinicNum==0) {//User's default clinic is HQ
						listEmpsUnassigned.Add(empCur);
						continue;
					}
					if(!dictUserClinics.ContainsKey(userCur.UserNum)) {//User is restricted to a clinic(s).  Compare to clinicNum
						dictUserClinics[userCur.UserNum]=UserClinics.GetForUser(userCur.UserNum);//run only once per user
					}
					if(dictUserClinics[userCur.UserNum].Count==0) {//unrestricted user, employee should show in all lists
						listEmpsUnassigned.Add(empCur);
						listEmpsWithClinic.Add(empCur);
					}
					else if(dictUserClinics[userCur.UserNum].Any(x => x.ClinicNum==clinicNum)) {//user restricted to this clinic
						listEmpsWithClinic.Add(empCur);
					}
				}
			}
			if(getUnassigned) {
				return listEmpsUnassigned.Union(listEmpsWithClinic).OrderBy(x=> Employees.GetNameFL(x)).ToList();
			}
			//Returning the isAll employee list was handled above (all non-hidden emps, ListShort).
			if(clinicNum==0) {//Return list of unassigned employees.  This is used for the 'Headquarters' clinic filter.
				return listEmpsUnassigned.GroupBy(x => x.EmployeeNum).Select(x => x.First()).ToList();//select distinct emps
			}
			//Return list of employees restricted to the specified clinic.
			return listEmpsWithClinic.GroupBy(x => x.EmployeeNum).Select(x => x.First()).ToList();//select distinct emps
		}

		/// <summary> Returns -1 if employeeNum is not found.  0 if not hidden and 1 if hidden.</summary>		
		public static int IsHidden(long employeeNum) {
			//No need to check RemotingRole; no call to db.
			Employee employee=GetFirstOrDefault(x => x.EmployeeNum==employeeNum);
			return (employee==null ? -1 : (employee.IsHidden ? 1 : 0));
		}

		///<summary>Loops through List to find the given extension and returns the employeeNum if found.  Otherwise, returns -1;</summary>
		public static long GetEmpNumAtExtension(int phoneExt) {
			//No need to check RemotingRole; no call to db.
			Employee employee=GetFirstOrDefault(x => x.PhoneExt==phoneExt);
			return (employee==null ? -1 : employee.EmployeeNum);
		}

		public static int SortByLastName(Employee x,Employee y) {
			return x.LName.CompareTo(y.LName);
		}

		public static int SortByFirstName(Employee x,Employee y) {
			return x.FName.CompareTo(y.FName);
		}
		
		/// <summary>sorting class used to sort Employee in various ways</summary>
		public class EmployeeComparer:IComparer<Employee> {
		
			private SortBy SortOn=SortBy.lastName;
			
			public EmployeeComparer(SortBy sortBy) {
				SortOn=sortBy;
			}
			
			public int Compare(Employee x,Employee y) {
				int ret=0;
				switch(SortOn) {
					case SortBy.empNum:
						ret=x.EmployeeNum.CompareTo(y.EmployeeNum); 
						break;
					case SortBy.ext:
						ret=x.PhoneExt.CompareTo(y.PhoneExt); 
						break;
					case SortBy.firstName:
						ret=x.FName.CompareTo(y.FName); 
						break;
					case SortBy.LFName:
						ret=x.LName.CompareTo(y.LName);
						if(ret==0) {
							ret=x.FName.CompareTo(y.FName);
						}
						break;
					case SortBy.lastName:
					default:
						ret=x.LName.CompareTo(y.LName); 
						break;
				}
				if(ret==0) {//last name is tie breaker
					return x.LName.CompareTo(y.LName);
				}
				//we got here so our sort was successful
				return ret;
			}

			public enum SortBy {
				///<summary>0 - By Extension.</summary>
				ext,
				///<summary>1 - By EmployeeNum.</summary>
				empNum,
				///<summary>2 - By FName.</summary>
				firstName,
				///<summary>3 - By LName.</summary>
				lastName,
				///<summary>4 - By LName, then FName.</summary>
				LFName
			};
		}
	}

	

	
	

}













