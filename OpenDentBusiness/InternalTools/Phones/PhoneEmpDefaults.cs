using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PhoneEmpDefaults{
		#region Cache
		private class PhoneEmpDefaultCache : CacheListAbs<PhoneEmpDefault> {
			protected override List<PhoneEmpDefault> GetCacheFromDb() {
				string command="SELECT * FROM phoneempdefault ORDER BY PhoneExt";
				return Crud.PhoneEmpDefaultCrud.SelectMany(command);
			}
			protected override List<PhoneEmpDefault> TableToList(DataTable table) {
				return Crud.PhoneEmpDefaultCrud.TableToList(table);
			}
			protected override PhoneEmpDefault Copy(PhoneEmpDefault phoneEmpDefault) {
				return phoneEmpDefault.Copy();
			}
			protected override DataTable ListToTable(List<PhoneEmpDefault> listPhoneEmpDefault) {
				return Crud.PhoneEmpDefaultCrud.ListToTable(listPhoneEmpDefault,"PhoneEmpDefault");
			}
			protected override void FillCacheIfNeeded() {
				PhoneEmpDefaults.GetTableFromCache(false);
			}
			protected override bool IsInListShort(PhoneEmpDefault phoneEmpDefault) {
				return true;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PhoneEmpDefaultCache _phoneEmpDefaultCache=new PhoneEmpDefaultCache();

		public static List<PhoneEmpDefault> GetDeepCopy(bool isShort=false) {
			return _phoneEmpDefaultCache.GetDeepCopy(isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_phoneEmpDefaultCache.FillCacheFromTable(table);
		}

		///<summary></summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_phoneEmpDefaultCache.FillCacheFromTable(table);
				return table;
			}
			return _phoneEmpDefaultCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache
		
		/*
		///<summary>Gets all 275 rows, but only 3 columns.</summary>
		public static List<PhoneEmpDefaultLim> RefreshLim(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneEmpDefaultLim>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT EmployeeNum,EmpName,IsGraphed FROM phoneempdefault";
			DataTable table=Db.GetTable(command);
			List<PhoneEmpDefaultLim> listPhoneEmpDefaultLims=new List<PhoneEmpDefaultLim>();
			foreach(DataRow row in table.Rows) {
				PhoneEmpDefaultLim phoneEmpDefaultLim=new PhoneEmpDefaultLim();
				phoneEmpDefaultLim.EmployeeNum=PIn.Long  (row["EmployeeNum"].ToString());
				phoneEmpDefaultLim.EmpName    =PIn.String(row["EmpName"].ToString());
				phoneEmpDefaultLim.IsGraphed  =PIn.Bool  (row["IsGraphed"].ToString());
				listPhoneEmpDefaultLims.Add(phoneEmpDefaultLim);
			}
			return listPhoneEmpDefaultLims;
		}*/

		///<summary>Gets one PhoneEmpDefault from the db.  Can return null.</summary>
		public static PhoneEmpDefault GetOne(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<PhoneEmpDefault>(MethodBase.GetCurrentMethod(),employeeNum);
			}
			return Crud.PhoneEmpDefaultCrud.SelectOne(employeeNum);
		}

		///<summary>From local list. Can return null.</summary>
		public static PhoneEmpDefault GetEmpDefaultFromList(long employeeNum,List<PhoneEmpDefault> listPED) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<listPED.Count;i++) {
				if(listPED[i].EmployeeNum==employeeNum) {
					return listPED[i];
				}
			}
			return null;
		}

		///<summary>No call to DB.  Can return null.</summary>
		public static PhoneEmpDefault GetByExtAndEmp(int extension,long employeeNum) {
			//No need to check RemotingRole; no call to db.
			List<PhoneEmpDefault> listPhoneEmpDefaults=GetDeepCopy();
			PhoneEmpDefault phoneEmpDefault=listPhoneEmpDefaults.FirstOrDefault(x=>x.PhoneExt==extension && x.EmployeeNum==employeeNum);
			return phoneEmpDefault;
		}

		///<summary>Find first employee with this extension and return their IsTriageOperator flag.</summary>
		public static bool IsTriageOperatorForExtension(int extension,List<PhoneEmpDefault> listPED) {
			//No need to check RemotingRole; no call to db.
			if(extension==0) {
				return false;
			}
			for(int i=0;i<listPED.Count;i++) {
				if(listPED[i].PhoneExt==extension) {
					return listPED[i].IsTriageOperator;
				}
			}
			return false; //couldn't find extension
		}

		///<summary>The employee passed in will take over the extension passed in.  
		///Moves any other employee who currently has this extension set (in phoneempdefault) to extension zero.  
		///This prevents duplicate extensions in phoneempdefault.</summary>
		public static void SetAvailable(int extension,long empNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),extension,empNum);
				return;
			}
			Employee emp=Employees.GetEmp(empNum);
			if(emp==null) {//Should never happen. This means the employee that's changing their status doesn't exist in the employee table.
				return;
			}
			string command="UPDATE phoneempdefault "
				+"SET StatusOverride="+POut.Int((int)PhoneEmpStatusOverride.None)
					+",PhoneExt="+POut.Int(extension)
					+",EmpName='"+POut.String(emp.FName)+"' "
				+"WHERE EmployeeNum="+POut.Long(empNum);
			Db.NonQ(command);
			//Set the extension to 0 for any other employee that is using this extension to prevent duplicate rows using the same extentions.
			//This would cause confusion for the ring groups.  This is possible if a user logged off and another employee logs into their computer.
			command="UPDATE phoneempdefault SET PhoneExt=0 "
				+"WHERE PhoneExt="+POut.Int(extension)+" "
				+"AND EmployeeNum!="+POut.Long(empNum);
			Db.NonQ(command);
		}
	
		///<summary></summary>
		public static long Insert(PhoneEmpDefault phoneEmpDefault){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				phoneEmpDefault.EmployeeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),phoneEmpDefault);
				return phoneEmpDefault.EmployeeNum;
			}
			return Crud.PhoneEmpDefaultCrud.Insert(phoneEmpDefault,true);//user specifies the PK
		}

		///<summary></summary>
		public static void Update(PhoneEmpDefault phoneEmpDefault){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneEmpDefault);
				return;
			}
			Crud.PhoneEmpDefaultCrud.Update(phoneEmpDefault);
		}

		///<summary></summary>
		public static void Delete(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employeeNum);
				return;
			}
			string command= "DELETE FROM phoneempdefault WHERE EmployeeNum = "+POut.Long(employeeNum);
			Db.NonQ(command);
		}

		/// <summary>Obsolete.  Use Linq OrderBy.</summary>
		public class PhoneEmpDefaultComparer:IComparer<PhoneEmpDefault> {
			
			private SortBy SortOn = SortBy.name;
			
			public PhoneEmpDefaultComparer(SortBy sortBy) {
				SortOn=sortBy;
			}
			
			public int Compare(PhoneEmpDefault x,PhoneEmpDefault y) {
				int retVal=0;
				switch(SortOn) {
					case SortBy.empNum:
						retVal=x.EmployeeNum.CompareTo(y.EmployeeNum); 
						break;
					case SortBy.ext:
						retVal=x.PhoneExt.CompareTo(y.PhoneExt); 
						break;
					case SortBy.escalation:
						retVal=x.EscalationOrder.CompareTo(y.EscalationOrder);
						break;
					case SortBy.name:
					default:
						retVal=x.EmpName.CompareTo(y.EmpName);
						break;
				}
				if(retVal==0) {//last name is tie breaker
					return x.EmpName.CompareTo(y.EmpName);
				}
				//we got here so our sort was successful
				return retVal;
			}
			
			public enum SortBy {
				///<summary>0 - By Extension.</summary>
				ext,
				///<summary>1 - By EmployeeNum.</summary>
				empNum,
				///<summary>2 - By Name.</summary>
				name,
				///<summary>3 - By Escalation Order.</summary>
				escalation
			};
		}
	}
}