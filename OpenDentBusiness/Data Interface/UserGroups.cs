using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class UserGroups {
		#region CachePattern

		private class UserGroupCache : CacheListAbs<UserGroup> {
			protected override List<UserGroup> GetCacheFromDb() {
				string command="SELECT * from usergroup ORDER BY Description";
				return Crud.UserGroupCrud.SelectMany(command);
			}
			protected override List<UserGroup> TableToList(DataTable table) {
				return Crud.UserGroupCrud.TableToList(table);
			}
			protected override UserGroup Copy(UserGroup userGroup) {
				return userGroup.Copy();
			}
			protected override DataTable ListToTable(List<UserGroup> listUserGroups) {
				return Crud.UserGroupCrud.ListToTable(listUserGroups,"UserGroup");
			}
			protected override void FillCacheIfNeeded() {
				UserGroups.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserGroupCache _userGroupCache=new UserGroupCache();

		public static List<UserGroup> GetDeepCopy(bool isShort=false) {
			return _userGroupCache.GetDeepCopy(isShort);
		}

		public static UserGroup GetFirstOrDefault(Func<UserGroup,bool> match,bool isShort=false) {
			return _userGroupCache.GetFirstOrDefault(match,isShort);
		}

		public static List<UserGroup> GetWhere(Predicate<UserGroup> match,bool isShort=false) {
			return _userGroupCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_userGroupCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool refreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),refreshCache);
				_userGroupCache.FillCacheFromTable(table);
				return table;
			}
			return _userGroupCache.GetTableFromCache(refreshCache);
		}

		public static void ClearCache() {
			_userGroupCache.ClearCache();
		}
		#endregion

		///<summary>A list of all user groups, ordered by description.  Does not include CEMT user groups.</summary>
		public static List<UserGroup> GetList() {
			return GetList(false);
		}

		///<summary>A list of all user groups, ordered by description.  Set includeCEMT to true if you want CEMT user groups included.</summary>
		public static List<UserGroup> GetList(bool includeCEMT) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => includeCEMT || x.UserGroupNumCEMT==0);
		}

		///<summary>Gets a list of usergroups for the API. Set doIncludeCEMT true to include CEMT usergroups.</summary>
		public static List<UserGroup> GetListForApi(bool doIncludeCEMT) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<UserGroup>>(MethodBase.GetCurrentMethod(),doIncludeCEMT);
			}
			string command="SELECT * FROM usergroup ";
			if(!doIncludeCEMT) {
				command+=" WHERE UserGroupNumCEMT=0";
			}
			return Crud.UserGroupCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(UserGroup userGroup){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroup);
				return;
			}
			Crud.UserGroupCrud.Update(userGroup);
		}

		///<summary>Only called from the CEMT in order to update a remote database with changes.  
		///This method will update rows based on the UserGroupNumCEMT instead of the typical UserGroupNum column.</summary>
		public static void UpdateCEMTNoCache(UserGroup userGroupCEMT) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroupCEMT);
				return;
			}
			if(userGroupCEMT.UserGroupNum == 0) {
				throw new Exception(userGroupCEMT.Description+" has a UserGroupNum of 0 and cannot be synced.");
			}
			string command="UPDATE usergroup SET "
				+"Description = '"+POut.String(userGroupCEMT.Description)+"' "
				+"WHERE UserGroupNumCEMT = "+POut.Long(userGroupCEMT.UserGroupNum);
			Db.NonQ(command);
		}

		public static List<UserGroup> GetCEMTGroups() {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => x.UserGroupNumCEMT!=0);
		}

		///<summary>Gets a list of CEMT usergroups without using the cache.  Useful for multithreaded connections.</summary>
		public static List<UserGroup> GetCEMTGroupsNoCache() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<UserGroup>>(MethodBase.GetCurrentMethod());
			}
			List<UserGroup> listUserGroups=new List<UserGroup>();
			string command="SELECT * FROM usergroup WHERE UserGroupNumCEMT!=0";
			DataTable tableUserGroups=Db.GetTable(command);
			listUserGroups=Crud.UserGroupCrud.TableToList(tableUserGroups);
			return listUserGroups;
		}

		///<summary></summary>
		public static long Insert(UserGroup userGroup) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				userGroup.UserGroupNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userGroup);
				return userGroup.UserGroupNum;
			}
			return Crud.UserGroupCrud.Insert(userGroup);
		}

		///<summary>Insertion logic that doesn't use the cache. Has special cases for generating random PK's and handling Oracle insertions.</summary>
		public static long InsertNoCache(UserGroup userGroup) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),userGroup);
			}
			return Crud.UserGroupCrud.InsertNoCache(userGroup);
		}

		///<summary>Checks for dependencies first</summary>
		public static void Delete(UserGroup userGroup){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroup);
				return;
			}
			string command="SELECT COUNT(*) FROM usergroupattach WHERE UserGroupNum='"
				+POut.Long(userGroup.UserGroupNum)+"'";
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="0"){
				throw new Exception(Lans.g("UserGroups","Must move users to another group first."));
			}
			if(PrefC.GetLong(PrefName.SecurityGroupForStudents)==userGroup.UserGroupNum) {
				throw new Exception(Lans.g("UserGroups","Group is the default group for students and cannot be deleted.  Change the default student group before deleting."));
			}
			if(PrefC.GetLong(PrefName.SecurityGroupForInstructors)==userGroup.UserGroupNum) {
				throw new Exception(Lans.g("UserGroups","Group is the default group for instructors and cannot be deleted.  Change the default instructors group before deleting."));
			}
			command= "DELETE FROM usergroup WHERE UserGroupNum='"
				+POut.Long(userGroup.UserGroupNum)+"'";
			Db.NonQ(command);
			command="DELETE FROM grouppermission WHERE UserGroupNum='"
				+POut.Long(userGroup.UserGroupNum)+"'";
			Db.NonQ(command);
		}
		
		///<summary>Deletes without using the cache.  Doesn't check dependencies.  Useful for multithreaded connections.</summary>
		public static void DeleteNoCache(UserGroup userGroup) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroup);
				return;
			}
			string command="DELETE FROM usergroup WHERE UserGroupNum="+POut.Long(userGroup.UserGroupNum);
			Db.NonQ(command);
			command="DELETE FROM grouppermission WHERE UserGroupNum="+POut.Long(userGroup.UserGroupNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static UserGroup GetGroup(long userGroupNum) {
			Meth.NoCheckMiddleTierRole();
			return GetFirstOrDefault(x => x.UserGroupNum==userGroupNum);
		}

		///<summary>Returns a list of usergroups given a list of usergroupnums.</summary>
		public static List<UserGroup> GetList(List<long> listUserGroupNums, bool includeCEMT) {
			Meth.NoCheckMiddleTierRole();
			List<UserGroup> listUserGroupsRet = new List<UserGroup>();
			List<UserGroup> listUserGroups;
			if(includeCEMT) {
				listUserGroups=GetDeepCopy(false);
			}
			else {
				listUserGroups=GetList();
			}
			for(int i=0;i<listUserGroupNums.Count;i++) {
				UserGroup userGroup = listUserGroups.FirstOrDefault(x => x.UserGroupNum == listUserGroupNums[i]);
				if(userGroup != null) { //should never be null.
					listUserGroupsRet.Add(listUserGroups.FirstOrDefault(x => x.UserGroupNum == listUserGroupNums[i]));
				}
			}
			return listUserGroupsRet;
		}

		///<summary>Returns a list of UserGroups that are associated to the permission passed in.</summary>
		public static List<UserGroup> GetForPermission(EnumPermType permissions) {
			Meth.NoCheckMiddleTierRole();
			List<long> listUserGroupNums=GroupPermissions.GetWhere(x => x.PermType==permissions)
				.Select(x => x.UserGroupNum)
				.Distinct()
				.ToList();
			return GetWhere(x => listUserGroupNums.Contains(x.UserGroupNum));
		}

		///<summary>Returns a list of usergroups for a given user. 
		///Returns an empty list if the user is not associated to any user groups. (should never happen)</summary>
		public static List<UserGroup> GetForUser(long userNum, bool includeCEMT) {
			Meth.NoCheckMiddleTierRole();
			//get the user group attaches.
			return GetList(UserGroupAttaches.GetForUser(userNum).Select(x => x.UserGroupNum).ToList(),includeCEMT);
		}

		///<summary>Returns true if at least one of the usergroups passed in has the SecurityAdmin permission.</summary>
		public static bool IsAdminGroup(List<long> listUserGroupNums) {
			Meth.NoCheckMiddleTierRole();
			List<GroupPermission> listGroupPermissionsAdmin=GroupPermissions.GetWhere(x => x.PermType==EnumPermType.SecurityAdmin);
			if(listUserGroupNums.Any(x => listGroupPermissionsAdmin.Select(y => y.UserGroupNum).Contains(x))) {
				return true;
			}
			return false;
		}
	}
 
	

	
}













