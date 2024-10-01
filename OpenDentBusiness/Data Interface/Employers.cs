using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary>Employers are refreshed as needed. A full refresh is frequently triggered if an employerNum cannot be found in the HList.  Important retrieval is done directly from the db.</summary>
	public class Employers{
		#region Cache Pattern

		private class EmployerCache : CacheDictAbs<Employer,long,Employer> {
			protected override List<Employer> GetCacheFromDb() {
				string command="SELECT EmployerNum,EmpName,'' Address,'' Address2,'' City,'' State,'' Zip,'' Phone FROM employer";
				return Crud.EmployerCrud.SelectMany(command);
			}
			protected override List<Employer> TableToList(DataTable table) {
				return Crud.EmployerCrud.TableToList(table);
			}
			protected override Employer Copy(Employer employer) {
				return employer.Copy();
			}
			protected override DataTable DictToTable(Dictionary<long,Employer> dictEmployers) {
				return Crud.EmployerCrud.ListToTable(dictEmployers.Values.Cast<Employer>().ToList(),"Employer");
			}
			protected override void FillCacheIfNeeded() {
				Employers.GetTableFromCache(false);
			}
			protected override long GetDictKey(Employer employer) {
				return employer.EmployerNum;
			}
			protected override Employer GetDictValue(Employer employer) {
				return employer;
			}
			protected override Employer CopyDictValue(Employer employer) {
				return employer.Copy();
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EmployerCache _employerCache=new EmployerCache();

		public static Employer GetOne(long employerNum) {
			return _employerCache.GetOne(employerNum);
		}

		public static List<Employer> GetListDeep(bool isShort=false) {
			return _employerCache.GetDeepCopy(isShort).Values.ToList();
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_employerCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_employerCache.FillCacheFromTable(table);
				return table;
			}
			return _employerCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_employerCache.ClearCache();
		}
		#endregion Cache Pattern

		/*
		 * Not using this because it turned out to be more efficient to refresh the whole
		 * list if an empnum could not be found.
		///<summary>Just refreshes Cur from the db with info for one employer.</summary>
		public static void Refresh(int employerNum){
			Cur=new Employer();//just in case no rows are returned
			if(employerNum==0) return;
			string command="SELECT * FROM employer WHERE EmployerNum = '"+employerNum+"'";
			DataTable table=Db.GetTable(command);;
			for(int i=0;i<table.Rows.Count;i++){//almost always just 1 row, but sometimes 0
				Cur.EmployerNum   =PIn.PInt   (table.Rows[i][0].ToString());
				Cur.EmpName       =PIn.PString(table.Rows[i][1].ToString());
			}
		}*/
		
		///<summary>Gets employers from database. Returns an empty list if none found.</summary>
		public static List<Employer> GetEmployersForApi(int limit,int offset) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Employer>>(MethodBase.GetCurrentMethod(),limit,offset);
			}
			string command="SELECT * FROM employer ";
			command+="ORDER BY employernum "//Ensure order for limit and offset
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.EmployerCrud.SelectMany(command);
		}

		public static void Update(Employer employerNew, Employer employerOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employerNew,employerOld);
				return;
			}
			Crud.EmployerCrud.Update(employerNew,employerOld);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(employerNew,employerOld,InsEditLogType.Employer,Security.CurUser.UserNum);
		}
		
		public static long Insert(Employer employer) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				employer.EmployerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),employer);
				return employer.EmployerNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(employer,null,InsEditLogType.Employer,Security.CurUser.UserNum);
			return Crud.EmployerCrud.Insert(employer);
		}

		///<summary>There MUST not be any dependencies before calling this or there will be invalid foreign keys.  
		///This is only called from FormEmployers after proper validation.</summary>
		public static void Delete(Employer employer) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),employer);
				return;
			}
			string command="DELETE from employer WHERE EmployerNum = '"+employer.EmployerNum.ToString()+"'";
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(null,employer,InsEditLogType.Employer,Security.CurUser.UserNum);
		}

		///<summary>Returns a list of patients that are dependent on the Cur employer. The list includes carriage returns for easy display.  Used before deleting an employer to make sure employer is not in use.</summary>
		public static string DependentPatients(Employer employer){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),employer);
			}
			string command="SELECT CONCAT(CONCAT(LName,', '),FName) FROM patient" 
				+" WHERE EmployerNum = '"+POut.Long(employer.EmployerNum)+"'";
			DataTable table=Db.GetTable(command);
			string retStr="";
			for(int i=0;i<table.Rows.Count;i++){
				if(i>0){
					retStr+="\r\n";//return, newline for multiple names.
				}
				retStr+=PIn.String(table.Rows[i][0].ToString());
			}
			return retStr;
		}

		///<summary>Returns a list of insplans that are dependent on the Cur employer. The list includes carriage returns for easy display.  Used before deleting an employer to make sure employer is not in use.</summary>
		public static string DependentInsPlans(Employer employer){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),employer);
			}
			string command="SELECT carrier.CarrierName,CONCAT(CONCAT(patient.LName,', '),patient.FName) "
				+"FROM insplan "
				+"LEFT JOIN inssub ON insplan.PlanNum=inssub.PlanNum "
				+"LEFT JOIN patient ON inssub.Subscriber=patient.PatNum "
				+"LEFT JOIN carrier ON insplan.CarrierNum=carrier.CarrierNum "
				+"WHERE insplan.EmployerNum = "+POut.Long(employer.EmployerNum);
			DataTable table=Db.GetTable(command);
			string retStr="";
			for(int i=0;i<table.Rows.Count;i++){
				if(i>0){
					retStr+="\r\n";//return, newline for multiple names.
				}
				retStr+=PIn.String(table.Rows[i][1].ToString())+": "+PIn.String(table.Rows[i][0].ToString());
			}
			return retStr;
		}

		///<summary>Gets the name of an employer based on the employerNum.  This also refreshes the list if necessary, so it will work even if the list has not been refreshed recently.</summary>
		public static string GetName(long employerNum) {
			Meth.NoCheckMiddleTierRole();
			Employer employer=GetEmployer(employerNum);
			if(employer.EmpName==null) {
				return "";
			} 
			return employer.EmpName;
		}

		///<summary>Gets an employer based on the employerNum. This will work even if the list has not been refreshed recently, but if you are going to need a lot of names all at once, then it is faster to refresh first.</summary>
		public static Employer GetEmployer(long employerNum) {
			Meth.NoCheckMiddleTierRole();
			if(employerNum==0) {
				return new Employer();
			}
			Employer employer=null;
			ODException.SwallowAnyException(() => {
				employer=GetOne(employerNum);
			});
			if(employer==null) {
				RefreshCache();
				ODException.SwallowAnyException(() => {
					employer=GetOne(employerNum);
				});
			}
			if(employer==null) {
				return new Employer();//Could only happen if corrupted or we're looking up an employer that no longer exists.
			}
			return employer;
		}

		public static Employer GetEmployerNoCache(long employerNum) {
			if(employerNum==0) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Employer>(MethodBase.GetCurrentMethod(),employerNum);
			}
			return Crud.EmployerCrud.SelectOne(employerNum);
		}

		///<summary>Gets an employerNum from the database based on the supplied name.  If that empName does not exist, then a new employer is created, and the employerNum for the new employer is returned.</summary>
		public static long GetEmployerNum(string empName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),empName);
			}
			if(empName==""){
				return 0;
			}
			string command="SELECT EmployerNum FROM employer" 
				+" WHERE EmpName = '"+POut.String(empName)+"'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0){
				return PIn.Long(table.Rows[0][0].ToString());
			}
			Employer employer=new Employer();
			employer.EmpName=empName;
			Insert(employer);
			Signalods.Insert(new Signalod { IType=InvalidType.Employers });//Signal to other workstations to refresh their caches as a new employer was inserted into the DB.
			//MessageBox.Show(Cur.EmployerNum.ToString());
			return employer.EmployerNum;
		}

		///<summary>Returns an employer if an exact match is found for the text supplied in the database.  Returns null if nothing found.</summary>
		public static Employer GetByName(string empName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Employer>(MethodBase.GetCurrentMethod(),empName);
			}
			string command="SELECT * FROM employer WHERE EmpName = '"+POut.String(empName)+"'";
			return Crud.EmployerCrud.SelectOne(command);
		}

		///<summary>Returns all employers with matching name, case-insensitive.</summary>
		public static List<Employer> GetAllByName(string empName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Employer>>(MethodBase.GetCurrentMethod(),empName);
			}
			string command="SELECT * FROM employer WHERE EmpName = '"+POut.String(empName)+"'";
			return Crud.EmployerCrud.SelectMany(command);
		}

		///<summary>Returns an arraylist of Employers with names similar to the supplied string.  Used in dropdown list from employer field for faster entry.  There is a small chance that the list will not be completely refreshed when this is run, but it won't really matter if one employer doesn't show in dropdown.</summary>
		public static List<Employer> GetSimilarNames(string empName) {
			Meth.NoCheckMiddleTierRole();
			return _employerCache.GetWhere(x => x.EmpName.StartsWith(empName,StringComparison.CurrentCultureIgnoreCase));
		}
		
		public static void MakeLog(Employer employer,LogSources logSources=LogSources.None) {
			string retVal="";
			retVal="Creating 'EmployerNum #"+employer.EmployerNum+":\r\n";
			retVal+="   Employer Name: "+employer.EmpName+"\r\n";
			if(!employer.Phone.IsNullOrEmpty()) {
				retVal+="   Phone: "+employer.Phone+"\r\n";
			}
			if(!employer.Address.IsNullOrEmpty()) {
				retVal+="   Address: "+employer.Address+"'\r\n";
			}
			if(logSources==LogSources.EmployerImport834) {
				retVal+="from Import 834.";
			}
			SecurityLogs.MakeLogEntry(EnumPermType.EmployerCreate,0,retVal,logSources);
		} 

		///<summary>Combines all the given employers into one. Updates patient and insplan. Then deletes all the others.</summary>
		public static void Combine(List<long> listEmployerNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEmployerNums);
				return;
			}
			long newNum=listEmployerNums[0];
			for(int i=1;i<listEmployerNums.Count;i++) {
				string command="SELECT PatNum FROM patient WHERE EmployerNum = "+POut.Long(listEmployerNums[i])+"";
				List<long> listPatNums=Db.GetListLong(command);
				for(int j=0;j<listPatNums.Count;j++) {
					command="UPDATE patient SET EmployerNum = "+POut.Long(newNum)+" WHERE PatNum = "+POut.Long(listPatNums[j])+"";
					Db.NonQ(command);
				}
				command="SELECT * FROM insplan WHERE EmployerNum = "+POut.Long(listEmployerNums[i]);
				List<InsPlan> listInsPlans=Crud.InsPlanCrud.SelectMany(command);
				//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
				for(int j=0;j<listInsPlans.Count;j++) {
					command="UPDATE insplan SET EmployerNum = "+POut.Long(newNum)+" WHERE PlanNum = "+POut.Long(listInsPlans[j].PlanNum);
					Db.NonQ(command);
					InsEditLogs.MakeLogEntry("EmployerNum",Security.CurUser.UserNum,listEmployerNums[i].ToString(),newNum.ToString(),
						InsEditLogType.InsPlan,listInsPlans[j].PlanNum,0,listInsPlans[j].GroupNum+" - "+listInsPlans[j].GroupName);
				}
				Employer employer=Employers.GetEmployer(listEmployerNums[i]);//from the cache
				Employers.Delete(employer); //logging taken care of in Delete method.
			}
		}
	}
}