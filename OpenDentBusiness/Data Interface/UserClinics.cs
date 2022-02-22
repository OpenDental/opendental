using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class UserClinics{
		#region CachePattern

		private class UserClinicCache : CacheListAbs<UserClinic> {
			protected override List<UserClinic> GetCacheFromDb() {
				string command="SELECT * FROM userclinic";
				return Crud.UserClinicCrud.SelectMany(command);
			}
			protected override List<UserClinic> TableToList(DataTable table) {
				return Crud.UserClinicCrud.TableToList(table);
			}
			protected override UserClinic Copy(UserClinic userClinic) {
				return userClinic.Copy();
			}
			protected override DataTable ListToTable(List<UserClinic> listUserClinics) {
				return Crud.UserClinicCrud.ListToTable(listUserClinics,"UserClinic");
			}
			protected override void FillCacheIfNeeded() {
				UserClinics.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserClinicCache _userClinicCache=new UserClinicCache();

		///<summary>Gets a deep copy of all matching items from the cache via ListLong.  Set isShort true to search through ListShort instead.</summary>
		public static List<UserClinic> GetWhere(Predicate<UserClinic> match,bool isShort=false) {
			return _userClinicCache.GetWhere(match,isShort);
		}

		public static UserClinic GetFirstOrDefault(Func<UserClinic,bool> match,bool isShort=false) {
			return _userClinicCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_userClinicCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_userClinicCache.FillCacheFromTable(table);
				return table;
			}
			return _userClinicCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Gets all User to Clinic associations for the user.  Can return an empty list if there are none.</summary>
		public static List<UserClinic> GetForUser(long userNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.UserNum==userNum);
		}

		///<summary>Gets all User to Clinic associations for a clinic.  Can return an empty list if there are none.</summary>
		public static List<UserClinic> GetForClinic(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ClinicNum==clinicNum);
		}

		///<summary>Gets one UserClinic from cache.  Can return null if none are found.</summary>
		public static UserClinic GetOne(long userClinicNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.UserClinicNum==userClinicNum);
		}

		///<summary></summary>
		public static long Insert(UserClinic userClinic){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				userClinic.UserClinicNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userClinic);
				return userClinic.UserClinicNum;
			}
			return Crud.UserClinicCrud.Insert(userClinic);
		}

		///<summary></summary>
		public static void Update(UserClinic userClinic){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userClinic);
				return;
			}
			Crud.UserClinicCrud.Update(userClinic);
		}

		///<summary></summary>
		public static void Delete(long userClinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userClinicNum);
				return;
			}
			Crud.UserClinicCrud.Delete(userClinicNum);
		}

		public static bool Sync(List<UserClinic> listNew,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,userNum);
			}
			List<UserClinic> listOld=UserClinics.GetForUser(userNum);
			return Crud.UserClinicCrud.Sync(listNew,listOld);
		}

		///<summary>Deletes all User to Clinic associations for a specific user.</summary>
		public static void DeleteForUser(long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userNum);
				return;
			}
			string command="DELETE FROM userclinic WHERE UserNum="+POut.Long(userNum);
			Db.NonQ(command);
		}

		///<summary>Deletes all User to Clinic associations for a specific clinic.</summary>
		public static void DeleteForClinic(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicNum);
				return;
			}
			string command="DELETE FROM userclinic WHERE ClinicNum="+POut.Long(clinicNum);
			Db.NonQ(command);
		}
	}
}