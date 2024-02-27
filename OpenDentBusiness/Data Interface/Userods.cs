using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Services;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.DirectoryServices;
using CodeBase;
using DataConnectionBase;
using ODCrypt;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness {
	///<summary>(Users OD)</summary>
	public class Userods {
		#region Get Methods

		///<summary>Returns the UserNum of the first non-hidden admin user if they have no password set.
		///It is very important to order by UserName in order to preserve old behavior of only considering the first Admin user we come across.
		///This method does not simply return the first admin user with no password.  It is explicit in only considering the FIRST admin user.
		///Returns 0 if there are no admin users or the first admin user found has a password set.</summary>
		public static long GetFirstSecurityAdminUserNumNoPasswordNoCache() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			//The query will order by UserName in order to preserve old behavior (mimics the cache).
			string command=@"SELECT userod.UserNum,CASE WHEN COALESCE(userod.Password,'')='' THEN 0 ELSE 1 END HasPassword 
				FROM userod
				INNER JOIN usergroupattach ON userod.UserNum=usergroupattach.UserNum
				INNER JOIN grouppermission ON usergroupattach.UserGroupNum=grouppermission.UserGroupNum 
				WHERE userod.IsHidden=0
				AND grouppermission.PermType="+POut.Int((int)EnumPermType.SecurityAdmin)+@"
				GROUP BY userod.UserNum
				ORDER BY userod.UserName
				LIMIT 1";
			DataTable table=Db.GetTable(command);
			long userNumAdminNoPass=0;
			if(table!=null && table.Rows.Count > 0 && table.Rows[0]["HasPassword"].ToString()=="0") {
				//The first admin user in the database does NOT have a password set.  Return their UserNum.
				userNumAdminNoPass=PIn.Long(table.Rows[0]["UserNum"].ToString());
			}
			return userNumAdminNoPass;
		}

		///<summary>Gets the corresponding user for the userNum passed in without using the cache.</summary>
		public static Userod GetUserNoCache(long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Userod>(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT * FROM userod WHERE userod.UserNum="+POut.Long(userNum);
			return Crud.UserodCrud.SelectOne(command);
		}

		///<summary>Gets the user name for the userNum passed in.  Returns empty string if not found in the database.</summary>
		public static string GetUserNameNoCache(long userNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT userod.UserName FROM userod WHERE userod.UserNum="+POut.Long(userNum);
			return Db.GetScalar(command);
		}

		///<summary>Returns a list of non-hidden, non-CEMT user names.  Set hasOnlyCEMT to true if you only want non-hidden CEMT users.
		///Always returns all non-hidden users if PrefName.UserNameManualEntry is true.</summary>
		public static List<string> GetUserNamesNoCache(bool hasOnlyCEMT) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),hasOnlyCEMT);
			}
			string command=$@"SELECT userod.UserName FROM userod 
				WHERE userod.IsHidden=0 
				{ (PrefC.GetBool(PrefName.UserNameManualEntry) ? " " : " AND userod.UserNumCEMT"+(hasOnlyCEMT ? "!=" : "=")+@"0 ") }
				ORDER BY userod.UserName";
			return Db.GetListString(command);
		}

		///<summary>Returns a list all user names from the database.</summary>
		public static List<string> GetUserNamesAllNoCache(bool isHidden=false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),isHidden);
			}
			string command=$@"SELECT userod.UserName FROM userod 
				WHERE userod.IsHidden={POut.Bool(isHidden)} ORDER BY userod.UserName";
			return Db.GetListString(command);
		}

		///<summary>Returns all non-hidden UserNums (key) and UserNames (value) associated with the domain user name passed in.
		///Returns an empty dictionary if no matches were found.</summary>
		public static SerializableDictionary<long,string> GetUsersByDomainUserNameNoCache(string domainUser) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,string>(MethodBase.GetCurrentMethod(),domainUser);
			}
			string command=@"SELECT userod.UserNum, userod.UserName, userod.DomainUser 
				FROM userod 
				WHERE IsHidden=0";
			//Not sure how to do an InvariantCultureIgnoreCase via a query so doing it over in C# in order to preserve old behavior.
			SerializableDictionary<long,string> dictNonHiddenUsers=Db.GetTable(command).Select()
				.Where(x => PIn.String(x["DomainUser"].ToString()).Equals(domainUser,StringComparison.InvariantCultureIgnoreCase))
				.ToSerializableDictionary(x => PIn.Long(x["UserNum"].ToString()),x => PIn.String(x["UserName"].ToString()));
			return dictNonHiddenUsers;
		}

		/// <summary>Gets Domain Users</summary>
		public static SerializableDictionary<long,string> GetDomainUsers() { //return dict of matched users
			//No remoting role check; no call to db
			string loginPath=PrefC.GetString(PrefName.DomainLoginPath);
			try {
				DirectoryEntry directoryEntry=new DirectoryEntry(loginPath);
				string distinguishedName=directoryEntry.Properties["distinguishedName"].Value.ToString();
				string domainGuid=directoryEntry.Guid.ToString();
				string domainGuidPref=PrefC.GetString(PrefName.DomainObjectGuid);
				string domainUser=(domainGuidPref+'\\'+Environment.UserName);
				if(domainGuidPref.IsNullOrEmpty()) {
					//Domain login was setup before we started recording the domain's ObjectGuid. We will save it now for future use.
					Prefs.UpdateString(PrefName.DomainObjectGuid,domainGuid);
					domainGuidPref=domainGuid;
				}
				return Userods.GetUsersByDomainUserNameNoCache(domainUser);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return new SerializableDictionary<long,string>();
			}
		}

		public static List<Userod> GetListByJobTeams(List<JobTeam> listJobTeams) {
			if(listJobTeams.IsNullOrEmpty()) {
				return new List<Userod>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Userod>>(MethodBase.GetCurrentMethod(),listJobTeams);
			}
			string command=@$"
				SELECT userod.*
				FROM userod
				INNER JOIN jobteamuser ON userod.UserNum=jobteamuser.UserNumEngineer
				WHERE userod.IsHidden = 0
				AND jobteamuser.JobTeamNum IN({POut.String(string.Join(",",listJobTeams.Select(x => x.JobTeamNum).ToList()))})";
			return UserodCrud.SelectMany(command);
		}
		#endregion

		#region Misc Methods

		///<summary>Returns true if at least one admin user is present within the database.  Otherwise; false.</summary>
		public static bool HasSecurityAdminUserNoCache() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command=@"SELECT COUNT(*) FROM userod
				INNER JOIN usergroupattach ON userod.UserNum=usergroupattach.UserNum
				INNER JOIN grouppermission ON usergroupattach.UserGroupNum=grouppermission.UserGroupNum 
				WHERE userod.IsHidden=0
				AND grouppermission.PermType="+POut.Int((int)EnumPermType.SecurityAdmin);
			return (Db.GetCount(command)!="0");
		}

		///<summary>Returns true if there are any users (including hidden) with a UserNumCEMT set.  Otherwise; false.</summary>
		public static bool HasUsersForCEMTNoCache() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command=@"SELECT COUNT(*) FROM userod
				WHERE userod.UserNumCEMT > 0";
			return (Db.GetCount(command)!="0");
		}

		///<summary>Returns true if the user can sign notes. Uses the NotesProviderSignatureOnly preference to validate.</summary>
		public static bool CanUserSignNote(Userod userod=null) {
			//No need to check MiddleTierRole; no call to db.
			Userod userodSig=userod;
			if(userod==null) {
				userodSig=Security.CurUser;
			}
			if(PrefC.GetBool(PrefName.NotesProviderSignatureOnly) && userodSig.ProvNum==0) {
				return false;//Prefernce is on and our user is not a provider.
			}
			return true;//Either pref is off or it is on and user is a provider.
		}

		#endregion

		#region CachePattern

		private class UserodCache : CacheListAbs<Userod> {
			protected override List<Userod> GetCacheFromDb() {
				string command="SELECT * FROM userod ORDER BY UserName";
				return Crud.UserodCrud.SelectMany(command);
			}
			protected override List<Userod> TableToList(DataTable table) {
				return Crud.UserodCrud.TableToList(table);
			}
			protected override Userod Copy(Userod userod) {
				return userod.Copy();
			}
			protected override DataTable ListToTable(List<Userod> listUserods) {
				return Crud.UserodCrud.ListToTable(listUserods,"Userod");
			}
			protected override void FillCacheIfNeeded() {
				Userods.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Userod userod) {
				return !userod.IsHidden;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserodCache _userodCache=new UserodCache();

		public static Userod GetFirstOrDefault(Func<Userod,bool> match,bool isShort=false) {
			return _userodCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Gets a deep copy of all matching items from the cache via ListLong.  Set isShort true to search through ListShort instead.</summary>
		public static List<Userod> GetWhere(Predicate<Userod> match,bool isShort=false) {
			return _userodCache.GetWhere(match,isShort);
		}

		public static List<Userod> GetDeepCopy(bool isShort=false) {
			return _userodCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_userodCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool refreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable tableOnClient=Meth.GetTable(MethodBase.GetCurrentMethod(),refreshCache);
				_userodCache.FillCacheFromTable(tableOnClient);
				Security.SyncCurUser();//Cache can have a stale reference to the Security.CurUser to ensure it has a current one.
				return tableOnClient;
			}
			DataTable table=_userodCache.GetTableFromCache(refreshCache);
			Security.SyncCurUser();//Cache can have a stale reference to the Security.CurUser to ensure it has a current one.
			return table;
		}

		public static void ClearCache() {
			_userodCache.ClearCache();
		}

		///<summary>Returns the boolean indicating if the user cache has been turned off or not.</summary>
		public static bool GetIsCacheAllowed() {
			return _userodCache.IsCacheAllowed;
		}

		///<summary>Set isCacheAllowed false to immediately clear out the userod cache and then set the cache into a state where it will throw an
		///exception if any method attempts to have the cache fill itself.  This is designed to keep sensitive data from being cached until a
		///verified user has logged in to the program.  Once a user has logged in then it is acceptable to fill the userod cache.</summary>
		public static void SetIsCacheAllowed(bool isCacheAllowed) {
			_userodCache.IsCacheAllowed=isCacheAllowed;
		}

		#endregion

		///<summary></summary>
		public static List<Userod> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Userod>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM userod ORDER BY UserName";
			return Crud.UserodCrud.TableToList(Db.GetTable(command));
		}

		///<summary></summary>
		public static Userod GetUser(long userNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.UserNum==userNum);
		}

		///<summary>Returns a list of users from cache from the list of usernums.</summary>
		public static List<Userod> GetUsers(List<long> listUserNums) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => listUserNums.Contains(x.UserNum));
		}

		///<summary>Returns a list of all non-hidden users.  Set includeCEMT to true if you want CEMT users included.</summary>
		public static List<Userod> GetUsers(bool includeCEMT=false) {
			//No need to check MiddleTierRole; no call to db.
			List<Userod> listUserodsNonHidden=new List<Userod>();
			List<Userod> listUserodsLong=Userods.GetDeepCopy();
			for(int i=0;i<listUserodsLong.Count;i++) {
				if(listUserodsLong[i].IsHidden) {
					continue;
				}
				if(!includeCEMT && listUserodsLong[i].UserNumCEMT!=0) {
					continue;
				}
				listUserodsNonHidden.Add(listUserodsLong[i]);
			}
			return listUserodsNonHidden;
		}

		///<summary>Returns a list of all non-hidden users.  Does not include CEMT users.</summary>
		public static List<Userod> GetUsersByClinic(long clinicNum) {
			//No need to check MiddleTierRole; no call to db.
			List<Userod> listUserRods=Userods.GetWhere(x => !x.IsHidden)//all non-hidden users
				.FindAll(x => !x.ClinicIsRestricted || x.ClinicNum==clinicNum); //for the given clinic or unassigned to clinic
				//CEMT user filter not required. CEMT users SHOULD be unrestricted to a clinic.
			return listUserRods;
		}
		
		///<summary>Returns a list of all users without using the local cache.  Useful for multithreaded connections.</summary>
		public static List<Userod> GetUsersNoCache() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Userod>>(MethodBase.GetCurrentMethod());
			}
			List<Userod> listUserods=new List<Userod>();
			string command="SELECT * FROM userod";
			DataTable tableUsers=Db.GetTable(command);
			listUserods=Crud.UserodCrud.TableToList(tableUsers);
			return listUserods;
		}

		///<summary>Returns a list of all CEMT users.</summary>
		public static List<Userod> GetUsersForCEMT() {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.UserNumCEMT!=0);
		}

		///<summary>Returns null if not found.  Is not case sensitive.  isEcwTight isn't even used.</summary>
		public static Userod GetUserByName(string userName,bool isEcwTight) {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => !x.IsHidden && x.UserName.ToLower()==userName.ToLower());
		}

		///<summary>Gets the first user with the matching userName passed in.  Not case sensitive.  Returns null if not found.
		///Does not use the cache to find a corresponding user with the passed in userName.  Every middle tier call passes through here.</summary>
		public static Userod GetUserByNameNoCache(string userName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Userod>(MethodBase.GetCurrentMethod(),userName);
			}
			string command="SELECT * FROM userod WHERE UserName='"+POut.String(userName)+"'";
			List<Userod> listUserods=Crud.UserodCrud.TableToList(Db.GetTable(command));
			return listUserods.FirstOrDefault(x => !x.IsHidden && x.UserName.ToLower()==userName.ToLower());
		}

		///<summary>Returns null if not found.</summary>
		public static Userod GetUserByEmployeeNum(long employeeNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.EmployeeNum==employeeNum);
		}

		///<summary>Returns all users that are associated to the employee passed in.  Returns empty list if no matches found.</summary>
		public static List<Userod> GetUsersByEmployeeNum(long employeeNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.EmployeeNum==employeeNum);
		}

		///<summary>Returns all users that are associated to the permission passed in. Returns empty list if no matches found.</summary>
		public static List<Userod> GetUsersByPermission(EnumPermType permissions,bool showHidden) {
			//No need to check MiddleTierRole; no call to db.
			List<UserGroup> listUserGroups=UserGroups.GetForPermission(permissions);
			List<long> listUserNums=UserGroupAttaches.GetUserNumsForUserGroups(listUserGroups);
			return Userods.GetWhere(x => listUserNums.Contains(x.UserNum),!showHidden);
		}

		///<summary>Returns all users that are associated to the permission passed in.  Returns empty list if no matches found.</summary>
		public static List<Userod> GetUsersByJobRole(JobPerm jobPerm,bool showHidden) {
			//No need to check MiddleTierRole; no call to db.
			List<JobPermission> listJobPermissionsRoles=JobPermissions.GetList().FindAll(x=>x.JobPermType==jobPerm);
			return Userods.GetWhere(x=>listJobPermissionsRoles.Any(y=>x.UserNum==y.UserNum),!showHidden);
		}

		///<summary>Gets all non-hidden users that have an associated provider.</summary>
		public static List<Userod> GetUsersWithProviders() {
			//No need to check MiddleTierRole; no call to db.
			return Userods.GetWhere(x => x.ProvNum!=0,true);
		}

		///<summary>Returns all users associated to the provider passed in.  Returns empty list if no matches found.</summary>
		public static List<Userod> GetUsersByProvNum(long provNum) {
			//No need to check MiddleTierRole; no call to db.
			return Userods.GetWhere(x => x.ProvNum==provNum,true);
		}
		
		public static List<Userod> GetUsersByInbox(long taskListNum) {
			//No need to check MiddleTierRole; no call to db.
			return Userods.GetWhere(x => x.TaskListInBox==taskListNum,true);
		}

		///<summary>Returns all users selectable for the insurance verification list.  
		///Pass in an empty list to not filter by clinic.  
		///Set isAssigning to false to return only users who have an insurance already assigned.</summary>
		public static List<Userod> GetUsersForVerifyList(List<long> listClinicNums,bool isAssigning,bool includeHiddenUsers=false) {
			//No need to check RemotingRole; no explicit call to db.
			List<long> listUserNumsInInsVerify=InsVerifies.GetAllInsVerifyUserNums();
			List<long> listUserNumsInClinic=new List<long>();
			if(listClinicNums.Count>0) {
				List<UserClinic> listUserClinics=new List<UserClinic>();
				for(int i=0;i<listClinicNums.Count;i++) {
					listUserNumsInClinic.AddRange(UserClinics.GetForClinic(listClinicNums[i]).Select(y => y.UserNum).Distinct().ToList());
				}
				listUserNumsInClinic.AddRange(GetUsers().FindAll(x => !x.ClinicIsRestricted).Select(x => x.UserNum).Distinct().ToList());//Always add unrestricted users into the list.
				listUserNumsInClinic=listUserNumsInClinic.Distinct().ToList();//Remove duplicates that could possibly be in the list.
				if(listUserNumsInClinic.Count>0) {
					listUserNumsInInsVerify=listUserNumsInInsVerify.FindAll(x => listUserNumsInClinic.Contains(x));
				}
				listUserNumsInInsVerify.AddRange(GetUsers(listUserNumsInInsVerify).FindAll(x => !x.ClinicIsRestricted).Select(x => x.UserNum).Distinct().ToList());//Always add unrestricted users into the list.
				listUserNumsInInsVerify=listUserNumsInInsVerify.Distinct().ToList();
			}
			List<Userod> listUserodsWithPerm=GetUsersByPermission(EnumPermType.InsPlanVerifyList,includeHiddenUsers);
			if(isAssigning) {
				if(listClinicNums.Count==0) {
					return listUserodsWithPerm;//Return unfiltered list of users with permission
				}
				//Don't limit user list to already assigned insurance verifications.
				return listUserodsWithPerm.FindAll(x => listUserNumsInClinic.Contains(x.UserNum));//Return users with permission, limited by their clinics
			}
			return listUserodsWithPerm.FindAll(x => listUserNumsInInsVerify.Contains(x.UserNum));//Return users limited by permission, clinic, and having an insurance already assigned.
		}

		///<summary>Returns all non-hidden users associated with the domain user name passed in. Returns an empty list if no matches found.</summary>
		public static List<Userod> GetUsersByDomainUserName(string domainUser) {
			return Userods.GetWhere(x => x.DomainUser.Equals(domainUser,StringComparison.InvariantCultureIgnoreCase),true);
		}

		///<summary>This handles situations where we have a usernum, but not a user.  And it handles usernum of zero.</summary>
		public static string GetName(long userNum) {
			//No need to check MiddleTierRole; no call to db.
			Userod userod=GetFirstOrDefault(x => x.UserNum==userNum);
			if (userod==null) {
				return "";
			}
			return userod.UserName;
		}

		///<summary>Returns true if the user passed in is associated with a provider that has (or had) an EHR prov key.</summary>
		public static bool IsUserCpoe(Userod userod) {
			//No need to check MiddleTierRole; no call to db.
			if(userod==null) {
				return false;
			}
			Provider provider=Providers.GetProv(userod.ProvNum);
			if(provider==null) {
				return false;
			}
			//Check to see if this provider has had a valid key at any point in history.
			return EhrProvKeys.HasProvHadKey(provider.LName,provider.FName);
		}

		///<summary>Searches the database for a corresponding user by username (not case sensitive).  Returns null is no match found.
		///Once a user has been found, if the number of failed log in attempts exceeds the limit an exception is thrown with a message to display to the 
		///user.  Then the hash of the plaintext password (if usingEcw is true, password needs to be hashed before passing into this method) is checked 
		///against the password hash that is currently in the database.  Once the plaintext password passed in is validated, this method will upgrade the 
		///hashing algorithm for the password (if necessary) and then returns the entire user object for the corresponding user found.  Throws exceptions 
		///with error message to display to the user if anything goes wrong.  Manipulates the appropriate log in failure columns in the db as 
		///needed.</summary>
		public static Userod CheckUserAndPassword(string userName,string plaintext,bool isEcw) {
			return CheckUserAndPassword(userName,plaintext,isEcw,true);
		}

		///<summary>Searches the database for a corresponding user by username (not case sensitive).  Returns null is no match found.
		///Once a user has been found, if the number of failed log in attempts exceeds the limit an exception is thrown with a message to display to the 
		///user.  Then the hash of the plaintext password (if usingEcw is true, password needs to be hashed before passing into this method) is checked 
		///against the password hash that is currently in the database.  Once the plaintext password passed in is validated, this method will upgrade the 
		///hashing algorithm for the password (if necessary) and then returns the entire user object for the corresponding user found.  Throws exceptions 
		///with error message to display to the user if anything goes wrong.  Manipulates the appropriate log in failure columns in the db as 
		///needed.  Null will be returned when hasExceptions is false and no matching user found, credentials are invalid, or account is locked.</summary>
		public static Userod CheckUserAndPassword(string userName,string plaintext,bool isEcw,bool hasExceptions) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Userod>(MethodBase.GetCurrentMethod(),userName,plaintext,isEcw,hasExceptions);
			}
			//Do not use the cache here because an administrator could have cleared the log in failure attempt columns for this user.
			//Also, middle tier calls this method every single time a process request comes to it.
			Userod userodDb=GetUserByNameNoCache(userName);
			if(userodDb==null) {
				if(hasExceptions) {
					throw new ODException(Lans.g("Userods","Invalid username or password."),ODException.ErrorCodes.CheckUserAndPasswordFailed);
				}
				return null;
			}
			DateTime dateTimeNowDb=MiscData.GetNowDateTime();
			//We found a user via matching just the username passed in.  Now we need to check to see if they have exceeded the log in failure attempts.
			//For now we are hardcoding a 5 minute delay when the user has failed to log in 5 times in a row.  
			//An admin user can reset the password or the failure attempt count for the user failing to log in via the Security window.
			if(userodDb.DateTFail.Year > 1880 //The user has failed to log in recently
				&& dateTimeNowDb.Subtract(userodDb.DateTFail) < TimeSpan.FromMinutes(5) //The last failure has been within the last 5 minutes.
				&& userodDb.FailedAttempts >= 5) //The user failed 5 or more times.
			{
				if(hasExceptions) {
					throw new ApplicationException(Lans.g("Userods","Account has been locked due to failed log in attempts."
						+"\r\nCall your security admin to unlock your account or wait at least 5 minutes."));
				}
				return null;
			}
			bool isPasswordValid=Authentication.CheckPassword(userodDb,plaintext,isEcw);
			Userod userodNew=userodDb.Copy();
			//If the last failed log in attempt was more than 5 minutes ago, reset the columns in the database so the user can try 5 more times.
			if(userodDb.DateTFail.Year > 1880 && dateTimeNowDb.Subtract(userodDb.DateTFail) > TimeSpan.FromMinutes(5)) {
				userodNew.FailedAttempts=0;
				userodNew.DateTFail=DateTime.MinValue;
			}
			if(!isPasswordValid) {
				userodNew.DateTFail=dateTimeNowDb;
				userodNew.FailedAttempts+=1;
			}
			//Synchronize the database with the results of the log in attempt above
			Crud.UserodCrud.Update(userodNew,userodDb);
			if(isPasswordValid) {
				//Upgrade the encryption for the password if this is not an eCW user (eCW uses md5) and the password is using an outdated hashing algorithm.
				if(!isEcw && !string.IsNullOrEmpty(plaintext) && userodNew.LoginDetails.HashType!=HashTypes.SHA3_512) {
					//Update the password to the default hash type which should be the most secure hashing algorithm possible.
					Authentication.UpdatePasswordUserod(userodNew,plaintext,HashTypes.SHA3_512);
					//The above method is almost guaranteed to have changed the password for userNew so go back out the db and get the changes that were made.
					userodNew=GetUserNoCache(userodNew.UserNum);
				}
				return userodNew;
			}
			//Password was not valid.
			if(hasExceptions) {
				throw new ODException(Lans.g("Userods","Invalid username or password."),ODException.ErrorCodes.CheckUserAndPasswordFailed);
			}
			return null;
		}

		///<summary>Only used from Middle Tier. Will throw an exception if it fails for any reason.  This will directly access the config file on the disk, read the values, and set 
		///the DataConnection to the new database.  If the web service attmepts to access the config file, and the config file xml node 
		///'ApplicationName' is missing or blank, it will be appended to the xml file.  If the 'ApplicationName' node
		///is set and the Application Virtual Path for the web service is not the same as the node value, throws an exception, which keeps the IIS service
		///from accessing the wrong database.</summary>
		public static void LoadDatabaseInfoFromFile(string configFilePath){
			//No need to check MiddleTierRole; no call to db.
			if(!File.Exists(configFilePath)){
				throw new Exception("Could not find "+configFilePath+" on the web server.");
			}
			XmlDocument xmlDocument=new XmlDocument();
			try {
				xmlDocument.Load(configFilePath);
			}
			catch{
				throw new Exception("Web server "+configFilePath+" could not be opened or is in an invalid format.");
			}
			XPathNavigator xPathNavigator=xmlDocument.CreateNavigator();
			//always picks the first database entry in the file:
			XPathNavigator xPathNavigatorConn=xPathNavigator.SelectSingleNode("//DatabaseConnection");//[Database='"+database+"']");
			if(xPathNavigatorConn==null) {
				throw new Exception(configFilePath+" does not contain a valid database entry.");//database+" is not an allowed database.");
			}
			XPathNavigator xPathNavigatorLogging=xPathNavigatorConn.SelectSingleNode("VerboseLogging");
			bool doLogVerbosely=false;
			try {
				doLogVerbosely = xPathNavigatorLogging.ValueAsBoolean;
			}
			catch { } // Do nothing, Verbose Logging just won't activate.
			Logger.DoVerboseLogging = () => doLogVerbosely;
			if( doLogVerbosely ) {
				XPathNavigator xPathNavigatorLogDirPath=xPathNavigatorConn.SelectSingleNode("LogDirectory");
				string pathLogDir="";
				if(xPathNavigatorLogDirPath!=null) {
					pathLogDir=xPathNavigatorLogDirPath.Value.Trim();
				}
				bool isValidPath=false;
				try {
					isValidPath=Path.IsPathRooted(pathLogDir); // Check for drive letter and for invalid path characters
				}
				catch { } // Do nothing, will default to server's virtual directory.
				Logger.LoggerDirOverride=ODFileUtils.CombinePaths(HostingEnvironment.ApplicationPhysicalPath, "Logging");
				if( isValidPath ) {
					Logger.LoggerDirOverride=pathLogDir;
				}
			}
			#region Verify ApplicationName Config File Value
			XPathNavigator xPathNavigatorConfigFileNode=xPathNavigatorConn.SelectSingleNode("ApplicationName");//usually /OpenDentalServer
			if(xPathNavigatorConfigFileNode==null) {//when first updating, this node will not exist in the xml file, so just add it.
				try {
					//AppendChild does not affect the position of the XPathNavigator; adds <ApplicationName>/OpenDentalServer<ApplicationName/> to the xml
					using(XmlWriter xmlWriter=xPathNavigatorConn.AppendChild()) {
						xmlWriter.WriteElementString("ApplicationName",HostingEnvironment.ApplicationVirtualPath);
					}
					xmlDocument.Save(configFilePath);
				}
				catch { }//do nothing, unable to write to the XML file, move on anyway
			}
			else if(string.IsNullOrWhiteSpace(xPathNavigatorConfigFileNode.Value)) {//empty node, add the Application Virtual Path
				try {
					xPathNavigatorConfigFileNode.SetValue(HostingEnvironment.ApplicationVirtualPath);//sets value to /OpenDentalServer or whatever they named their app
					xmlDocument.Save(configFilePath);
				}
				catch { }//do nothing, unable to write to the XML file, move on anyway
			}
			else if(xPathNavigatorConfigFileNode.Value.ToLower()!=HostingEnvironment.ApplicationVirtualPath.ToLower()) {
				//the xml node exists and this file already has an Application Virtual Path in it that does not match the name of the IIS attempting to access it
				string filePath=ODFileUtils.CombinePaths(Path.GetDirectoryName(configFilePath),HostingEnvironment.ApplicationVirtualPath.Trim('/')+"Config.xml");
				throw new Exception("Multiple middle tier servers are potentially trying to connect to the same database.\r\n"
					+"This middle tier server cannot connect to the database within the config file found.\r\n"
					+"This middle tier server should be using the following config file:\r\n\t"+filePath+"\r\n"
					+"The config file is expecting an ApplicationName of:\r\n\t"+HostingEnvironment.ApplicationVirtualPath);
			}
			#endregion Verify ApplicationName Config File Value
			string connString="",server="",database="",mysqlUser="",mysqlPassword="",mysqlUserLow="",mysqlPasswordLow="";
			XPathNavigator xPathNavigatorConString=xPathNavigatorConn.SelectSingleNode("ConnectionString");
			if(xPathNavigatorConString!=null) {//If there is a connection string then use it.
				connString=xPathNavigatorConString.Value;
			}
			else {
				//return navOne.SelectSingleNode("summary").Value;
				//now, get the values for this connection
				server=xPathNavigatorConn.SelectSingleNode("ComputerName").Value;
				database=xPathNavigatorConn.SelectSingleNode("Database").Value;
				mysqlUser=xPathNavigatorConn.SelectSingleNode("User").Value;
				mysqlPassword=xPathNavigatorConn.SelectSingleNode("Password").Value;
				XPathNavigator xPathNavigatorEncryptPwdNode=xPathNavigatorConn.SelectSingleNode("MySQLPassHash");
				string decryptedPwd;
				if(mysqlPassword=="" && xPathNavigatorEncryptPwdNode!=null && xPathNavigatorEncryptPwdNode.Value!="" && CDT.Class1.Decrypt(xPathNavigatorEncryptPwdNode.Value,out decryptedPwd)) {
					mysqlPassword=decryptedPwd;
				}
				mysqlUserLow=xPathNavigatorConn.SelectSingleNode("UserLow").Value;
				mysqlPasswordLow=xPathNavigatorConn.SelectSingleNode("PasswordLow").Value;
			}
			XPathNavigator xPathNavigatorDbType=xPathNavigatorConn.SelectSingleNode("DatabaseType");
			DatabaseType databaseType=DatabaseType.MySql;
			if(xPathNavigatorDbType!=null){
				if(xPathNavigatorDbType.Value=="Oracle"){
					databaseType=DatabaseType.Oracle;
				}
			}
			DataConnection dataConnection=new DataConnection();
			if(connString!="") {
				try {
					dataConnection.SetDb(connString,"",databaseType);
				}
				catch(Exception e) {
					throw new Exception(e.Message+"\r\n"+"Connection to database failed.  Check the values in the config file on the web server "+configFilePath);
				}
			}
			else {
				try {
					dataConnection.SetDb(server,database,mysqlUser,mysqlPassword,mysqlUserLow,mysqlPasswordLow,databaseType);
				}
				catch(Exception e) {
					throw new Exception(e.Message+"\r\n"+"Connection to database failed.  Check the values in the config file on the web server "+configFilePath);
				}
			}
			//todo?: make sure no users have blank passwords.
		}

		///<summary>DEPRICATED DO NOT USE.  Use OpenDentBusiness.Authentication class instead.  For middle tier backward-compatability only.</summary>
		public static string HashPassword(string inputPass) {
			//No need to check MiddleTierRole; no call to db.
			bool useEcwAlgorithm=Programs.IsEnabled(ProgramName.eClinicalWorks);
			return HashPassword(inputPass,useEcwAlgorithm);
		}

		///<summary>DEPRICATED DO NOT USE.  Use OpenDentBusiness.Authentication class instead.  For middle tier backward-compatability only.</summary>
		public static string HashPassword(string inputPass,bool useEcwAlgorithm) {
			//No need to check MiddleTierRole; no call to db.
			if(inputPass=="") {
				return "";
			}
			return Authentication.HashPasswordMD5(inputPass,useEcwAlgorithm);
		}

		///<summary>Updates all students/instructors to the specified user group.  Surround with try/catch because it can throw exceptions.</summary>
		public static void UpdateUserGroupsForDentalSchools(UserGroup userGroup,bool isInstructor) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroup,isInstructor);
				return;
			}
			string command;
			//Check if the user group that the students or instructors are trying to go to has the SecurityAdmin permission.
			if(!GroupPermissions.HasPermission(userGroup.UserGroupNum,EnumPermType.SecurityAdmin,0)) {
				//We need to make sure that moving these users to the new user group does not eliminate all SecurityAdmin users in db.
				command="SELECT COUNT(*) FROM usergroupattach "
					+"INNER JOIN usergroup ON usergroupattach.UserGroupNum=usergroup.UserGroupNum "
					+"INNER JOIN grouppermission ON grouppermission.UserGroupNum=usergroup.UserGroupNum "
					+"WHERE usergroupattach.UserNum NOT IN "
					+"(SELECT userod.UserNum FROM userod,provider "
						+"WHERE userod.ProvNum=provider.ProvNum ";
				if(isInstructor) {
					command+="AND provider.IsInstructor="+POut.Bool(isInstructor)+") ";
				}
				else {
					command+="AND provider.IsInstructor="+POut.Bool(isInstructor)+" ";
					command+="AND provider.SchoolClassNum!=0) ";
				}
				command+="AND grouppermission.PermType="+POut.Int((int)EnumPermType.SecurityAdmin)+" ";
				int lastAdmin=PIn.Int(Db.GetCount(command));
				if(lastAdmin==0) {
					throw new Exception("Cannot move students or instructors to the new user group because it would leave no users with the SecurityAdmin permission.");
				}
			}
			command="UPDATE userod INNER JOIN provider ON userod.ProvNum=provider.ProvNum "
					+"SET UserGroupNum="+POut.Long(userGroup.UserGroupNum)+" "
					+"WHERE provider.IsInstructor="+POut.Bool(isInstructor);
			if(!isInstructor) {
				command+=" AND provider.SchoolClassNum!=0";
			}
			Db.NonQ(command);
		}

		///<summary>Surround with try/catch because it can throw exceptions.</summary>
		public static void Update(Userod userod, List<long> listUserGroupNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userod,listUserGroupNums);
				return;
			}
			Validate(false,userod,false,listUserGroupNums);
			Crud.UserodCrud.Update(userod);
			if(listUserGroupNums == null) {
				return;
			}
			UserGroupAttaches.SyncForUser(userod,listUserGroupNums);
		}

		///<summary>Update for CEMT only.  Used when updating Remote databases with information from the CEMT.  Because of potentially different primary keys we have to update based on UserNumCEMT.</summary>
		public static void UpdateCEMT(Userod userod) {
			//This should never happen, but is a failsafe to prevent the overwriting of all non-CEMT users in the remote database.
			if(userod.UserNumCEMT == 0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userod);
				return;
			}
			UserodCrud.UpdateCemt(userod);
		}

		///<summary>DEPRICATED DO NOT USE. Use OpenDentBusiness.Authentication class instead.  For middle tier backward-compatability only.</summary>
		public static void UpdatePassword(Userod userod,string newPassHashed,bool isPasswordStrong) {
			//Before 18.3, we only used MD5
			UpdatePassword(userod,new PasswordContainer(HashTypes.MD5,"",newPassHashed),isPasswordStrong);
		}

		///<summary>Surround with try/catch because it can throw exceptions.
		///Same as Update(), only the Validate call skips checking duplicate names for hidden users.</summary>
		public static void UpdatePassword(Userod userod,PasswordContainer passwordContainer,bool isPasswordStrong, bool includeCEMT = false) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userod,passwordContainer,isPasswordStrong,includeCEMT);
				return;
			}
			Userod userodToUpdate=userod.Copy();
			userodToUpdate.LoginDetails=passwordContainer;
			userodToUpdate.PasswordIsStrong=isPasswordStrong;
			List<UserGroup> listUserGroups=userodToUpdate.GetGroups(includeCEMT);
			if(listUserGroups.Count < 1) {
				throw new Exception(Lans.g("Userods","The current user must be in at least one user group."));
			}
			Validate(false,userodToUpdate,true,listUserGroups.Select(x => x.UserGroupNum).ToList());
			Crud.UserodCrud.Update(userodToUpdate);
		}

		///<summary>Sets the TaskListInBox to 0 for any users that have this as their inbox.</summary>
		public static void DisassociateTaskListInBox(long taskListNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskListNum);
				return;
			}
			string command="UPDATE userod SET TaskListInBox=0 WHERE TaskListInBox="+POut.Long(taskListNum);
			Db.NonQ(command);
		}

		///<summary>A user must always have at least one associated userGroupAttach. Pass in the usergroup(s) that should be attached.
		///Surround with try/catch because it can throw exceptions.</summary>
		public static long Insert(Userod userod,List<long> listUserGroupNums,bool isForCEMT = false){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				userod.UserNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userod,listUserGroupNums,isForCEMT);
				return userod.UserNum;
			}
			if(userod.IsHidden && UserGroups.IsAdminGroup(listUserGroupNums)) {
				throw new Exception(Lans.g("Userods","Admins cannot be hidden."));
			}
			Validate(true,userod,false,listUserGroupNums);
			long userNum = Crud.UserodCrud.Insert(userod);
			UserGroupAttaches.SyncForUser(userod,listUserGroupNums);
			if(isForCEMT) {
				userod.UserNumCEMT=userNum;
				Crud.UserodCrud.Update(userod);
			}
			return userNum;
		}

		public static long InsertNoCache(Userod userod){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),userod);
			}
			return Crud.UserodCrud.InsertNoCache(userod);
		}

		///<summary>Surround with try/catch because it can throw exceptions.  
		///We don't really need to make this public, but it's required in order to follow the RemotingRole pattern.
		///listUserGroupNum can only be null when validating for an Update.</summary>
		public static void Validate(bool isNew,Userod userod,bool excludeHiddenUsers,List<long> listUserGroupNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),isNew,userod,excludeHiddenUsers,listUserGroupNums);
				return;
			}
			//should add a check that employeenum and provnum are not both set.
			//make sure username is not already taken
			string command;
			long excludeUserNum;
			if(isNew){
				excludeUserNum=0;
			}
			else{
				excludeUserNum=userod.UserNum;//it's ok if the name matches the current username
			}
			//It doesn't matter if the UserName is already in use if the user being updated is going to be hidden.  This check will block them from unhiding duplicate users.
			if(!userod.IsHidden) {//if the user is now not hidden
				//CEMT users will not be visible from within Open Dental.  Therefore, make a different check so that we can know if the name
				//the user typed in is a duplicate of a CEMT user.  In doing this, we are able to give a better message.
				if(!IsUserNameUnique(userod.UserName,excludeUserNum,excludeHiddenUsers,true)) {
					throw new ApplicationException(Lans.g("Userods","UserName already in use by CEMT member."));
				}
				if(!IsUserNameUnique(userod.UserName,excludeUserNum,excludeHiddenUsers)) {
					//IsUserNameUnique doesn't care if it's a CEMT user or not.. It just gets a count based on username.
					throw new ApplicationException(Lans.g("Userods","UserName already in use."));
				}
			}
			if(listUserGroupNums==null) {//Not validating UserGroup selections.
				return;
			}
			if(listUserGroupNums.Count<1) {
				throw new ApplicationException(Lans.g("Userods","The current user must be in at least one user group."));
			}
			//an admin user can never be hidden
			command="SELECT COUNT(*) FROM grouppermission "
				+"WHERE PermType='"+POut.Long((int)EnumPermType.SecurityAdmin)+"' "
				+"AND UserGroupNum IN ("+string.Join(",",listUserGroupNums)+") ";
			if(!isNew//Updating.
				&& Db.GetCount(command)=="0"//if this user would not have admin
				&& !IsSomeoneElseSecurityAdmin(userod))//make sure someone else has admin
			{
				throw new ApplicationException(Lans.g("Users","At least one user must have Security Admin permission."));
			}
			if(userod.IsHidden//hidden 
				&& userod.UserNumCEMT==0//and non-CEMT
				&& Db.GetCount(command)!="0")//if this user is admin
			{
				throw new ApplicationException(Lans.g("Userods","Admins cannot be hidden."));
			}
		}

		/// <summary>Returns true if there is at least one user part of the SecurityAdmin permission excluding the user passed in.</summary>
		public static bool IsSomeoneElseSecurityAdmin(Userod userod) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),userod);
			}
			string command="SELECT COUNT(*) FROM userod "
				+"INNER JOIN usergroupattach ON usergroupattach.UserNum=userod.UserNum "
				+"INNER JOIN grouppermission ON usergroupattach.UserGroupNum=grouppermission.UserGroupNum "
				+"WHERE grouppermission.PermType='"+POut.Long((int)EnumPermType.SecurityAdmin)+"'"
				+" AND userod.IsHidden =0"
				+" AND userod.UserNum != "+POut.Long(userod.UserNum);
			if(Db.GetCount(command)=="0") {//there are no other users with this permission
				return false;
			}
			return true;
		}

		public static bool IsUserNameUnique(string userName,long excludeUserNum,bool excludeHiddenUsers) {
			return IsUserNameUnique(userName,excludeUserNum,excludeHiddenUsers,false);
		}

		///<summary>Supply 0 or -1 for the excludeUserNum to not exclude any.</summary>
		public static bool IsUserNameUnique(string userName,long excludeUserNum,bool excludeHiddenUsers,bool searchCEMTUsers) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),userName,excludeUserNum,excludeHiddenUsers,searchCEMTUsers);
			}
			if(userName==""){
				return false;
			}
			string command="SELECT COUNT(*) FROM userod WHERE ";
			//if(Programs.UsingEcwTight()){
			//	command+="BINARY ";//allows different usernames based on capitalization.//we no longer allow this
				//Does not need to be tested under Oracle because eCW users do not use Oracle.
			//}
			command+="UserName='"+POut.String(userName)+"' "
				+"AND UserNum !="+POut.Long(excludeUserNum)+" ";
			if(excludeHiddenUsers) {
				command+="AND IsHidden=0 ";//not hidden
			}
			if(searchCEMTUsers) {
				command+="AND UserNumCEMT!=0";
			}
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()=="0") {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Generates a unique username based on what is passed into it.
		/// Returns null if given userName can not be easily identified as unique.
		/// </summary>
		/// <param name="userName">The username you are copying</param>
		/// <param name="excludeUserNum">The UserNum that is excluded when checking if a username is in use.</param>
		/// <param name="excludeHiddenUsers">Set to true to exclude hidden patients when checking if a username is in use, otherwise false</param>
		/// <param name="searchCEMTUsers">Set to true to include checking usernames that are associated to CEMT users.</param>
		/// <param name="uniqueUserName">When returning true this is set to a unique username, otherwise null.</parm>
		/// <returns></returns>
		public static bool TryGetUniqueUsername(string userName,long excludeUserNum,bool excludeHiddenUsers,bool searchCEMTUsers,out string uniqueUserName) {
			int attempt=1;
			uniqueUserName=userName;//Default to given username, will change if not unique.
			while(!IsUserNameUnique(uniqueUserName,excludeUserNum,excludeHiddenUsers,searchCEMTUsers)) {
				if(attempt>100) {
					uniqueUserName=null;
					return false;
				}
				uniqueUserName=userName+$"({++attempt})";
			}
			return true;
		}

		/// <summary>
		/// Inserts a new user into table and returns that new user. Not all fields are copied from original user.
		/// </summary>
		/// <param name="userod">The user that we will be copying from, not all fields are copied.</param>
		/// <param name="passwordContainer"></param>
		/// <param name="isPasswordStrong"></param>
		/// <param name="userName"></param>
		/// <param name="isForCemt">When true newly inserted user.UserNumCEMT will be set to the user.UserNum</param>
		/// <returns></returns>
		public static Userod CopyUser(Userod userod,PasswordContainer passwordContainer,bool isPasswordStrong,string userName=null,bool isForCemt=false) {
			if(!TryGetUniqueUsername(userName??(userod.UserName+"(Copy)"),0,false,isForCemt,out string uniqueUserName)) {
					return null;
			}
			Userod userodCopy=new Userod();		
			//if function is ever called outside of the security form this ensures that we will know if a user is a copy of another user
			userodCopy.UserName=uniqueUserName;
			userodCopy.LoginDetails=passwordContainer;
			userodCopy.PasswordIsStrong=isPasswordStrong;
			userodCopy.ClinicIsRestricted=userod.ClinicIsRestricted;
			userodCopy.ClinicNum=userod.ClinicNum;
			//Insert also validates the user.
			userodCopy.UserNum=Insert(userodCopy,UserGroups.GetForUser(userod.UserNum,isForCemt).Select(x => x.UserGroupNum).ToList(),isForCemt);
			#region UserClinics
			List<UserClinic> listUserClinics=new List<UserClinic>(UserClinics.GetForUser(userod.UserNum));
			listUserClinics.ForEach(x => x.UserNum=userodCopy.UserNum);
			UserClinics.Sync(listUserClinics,userodCopy.UserNum);
			#endregion
			#region Alerts
			List<AlertSub> listAlertSubsUsers=AlertSubs.GetAllForUser(userod.UserNum);
			listAlertSubsUsers.ForEach(x => x.UserNum=userodCopy.UserNum);
			AlertSubs.Sync(listAlertSubsUsers,new List<AlertSub>());
			#endregion
			return userodCopy;
		}

		public static List<Userod> GetForGroup(long userGroupNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.IsInUserGroup(userGroupNum));
		}

		///<summary>Gets a list of users for which the passed-in clinicNum is the only one they have access to.</summary>
		public static List<Userod> GetUsersOnlyThisClinic(long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Userod>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT userod.* "
			+ "FROM( "
				+ "SELECT userclinic.UserNum,COUNT(userclinic.ClinicNum) Clinics FROM userclinic "
				+ "GROUP BY userNum "
				+ "HAVING Clinics = 1 "
			+ ") users "
			+ "INNER JOIN userclinic ON userclinic.UserNum = users.UserNum "
				+ "AND userclinic.ClinicNum = " + POut.Long(clinicNum) + " "
			+ "INNER JOIN userod ON userod.UserNum = userclinic.UserNum ";
			return Crud.UserodCrud.SelectMany(command);
		}

		/// <summary>Will return 0 if no inbox found for user.</summary>
		public static long GetInbox(long userNum) {
			//No need to check MiddleTierRole; no call to db.
			Userod userod=GetFirstOrDefault(x => x.UserNum==userNum);
			if (userod==null) {
				return 0;
			}
			return userod.TaskListInBox;
		}

		///<summary>Returns 3, which is non-admin provider type, if no match found.</summary>
		public static long GetAnesthProvType(long anesthProvType) {
			//No need to check MiddleTierRole; no call to db.
			Userod userod=GetFirstOrDefault(x => x.AnesthProvType==anesthProvType);
			if (userod==null) {
				return 3;
			}
			return userod.AnesthProvType;
		}

		public static List<Userod> GetUsersForJobs() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Userod>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM userod "
				+"INNER JOIN jobpermission ON userod.UserNum=jobpermission.UserNum "
				+"WHERE IsHidden=0 GROUP BY userod.UserNum ORDER BY UserName";
			return Crud.UserodCrud.SelectMany(command);
		}

		///<summary>Returns empty string if password is strong enough.  Otherwise, returns explanation of why it's not strong enough.</summary>
		public static string IsPasswordStrong(string password,bool requireStrong=false) {
			//No need to check MiddleTierRole; no call to db.
			string strongPasswordMsg=" when the strong password feature is turned on";
			if(requireStrong) {//Just used by the API, which always requires strong pw
				strongPasswordMsg="";
			}
			if(password=="") {
				return Lans.g("FormUserPassword","Password may not be blank"+strongPasswordMsg+".");
			}
			if(password.Length<8) {
				return Lans.g("FormUserPassword","Password must be at least eight characters long"+strongPasswordMsg+".");
			}
			bool containsCap=false;
			for(int i=0;i<password.Length;i++) {
				if(Char.IsUpper(password[i])) {
					containsCap=true;
				}
			}
			if(!containsCap) {
				return Lans.g("FormUserPassword","Password must contain at least one capital letter"+strongPasswordMsg+".");
			}
			bool containsLower=false;
			for(int i=0;i<password.Length;i++) {
				if(Char.IsLower(password[i])) {
					containsLower=true;
				}
			}
			if(!containsLower) {
				return Lans.g("FormUserPassword","Password must contain at least one lower case letter"+strongPasswordMsg+".");
			}
			if(PrefC.GetBool(PrefName.PasswordsStrongIncludeSpecial)) {
				bool hasSpecial=false;
				for(int i=0;i<password.Length;i++) {
					if(!Char.IsLetterOrDigit(password[i])) {
						hasSpecial=true;
						break;
					}
				}
				if(!hasSpecial) {
					return Lans.g("FormUserPassword","Password must contain at least one special character when the 'strong passwords require a special character' feature is turned on.");
				}
			}
			bool containsNum=false;
			for(int i=0;i<password.Length;i++) {
				if(Char.IsNumber(password[i])) {
					containsNum=true;
				}
			}
			if(!containsNum) {
				return Lans.g("FormUserPassword","Password must contain at least one number"+strongPasswordMsg+".");
			}
			return "";
		}

		///<summary>This resets the strong password flag on all users after an admin turns off pref PasswordsMustBeStrong.  If strong passwords are again turned on later, then each user will have to edit their password in order set the strong password flag again.</summary>
		public static void ResetStrongPasswordFlags() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="UPDATE userod SET PasswordIsStrong=0";
			Db.NonQ(command);
		}

		///<summary>Returns true if the passed-in user is apart of the passed-in usergroup.</summary>
		public static bool IsInUserGroup(long userNum,long userGroupNum) {
			//No need to check MiddleTierRole; no call to db.
			List<UserGroupAttach> listUserGroupAttaches = UserGroupAttaches.GetForUser(userNum);
			return listUserGroupAttaches.Select(x => x.UserGroupNum).Contains(userGroupNum);
		}
	}
}