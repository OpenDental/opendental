using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobTeamUsers{
		#region Cache Pattern
		private class JobTeamUserCache : CacheListAbs<JobTeamUser> {
			protected override List<JobTeamUser> GetCacheFromDb() {
				string command="SELECT * FROM jobteamuser";
				return Crud.JobTeamUserCrud.SelectMany(command);
			}
			protected override List<JobTeamUser> TableToList(DataTable table) {
				return Crud.JobTeamUserCrud.TableToList(table);
			}
			protected override JobTeamUser Copy(JobTeamUser jobTeamUser) {
				return jobTeamUser.Copy();
			}
			protected override DataTable ListToTable(List<JobTeamUser> listJobTeamUsers) {
				return Crud.JobTeamUserCrud.ListToTable(listJobTeamUsers,"JobTeamUser");
			}
			protected override void FillCacheIfNeeded() {
				JobTeamUsers.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static JobTeamUserCache _jobTeamUserCache=new JobTeamUserCache();

		public static List<JobTeamUser> GetDeepCopy(bool isShort=false) {
			return _jobTeamUserCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _jobTeamUserCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<JobTeamUser> match,bool isShort=false) {
			return _jobTeamUserCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<JobTeamUser> match,bool isShort=false) {
			return _jobTeamUserCache.GetFindIndex(match,isShort);
		}

		public static JobTeamUser GetFirst(bool isShort=false) {
			return _jobTeamUserCache.GetFirst(isShort);
		}

		public static JobTeamUser GetFirst(Func<JobTeamUser,bool> match,bool isShort=false) {
			return _jobTeamUserCache.GetFirst(match,isShort);
		}

		public static JobTeamUser GetFirstOrDefault(Func<JobTeamUser,bool> match,bool isShort=false) {
			return _jobTeamUserCache.GetFirstOrDefault(match,isShort);
		}

		public static JobTeamUser GetLast(bool isShort=false) {
			return _jobTeamUserCache.GetLast(isShort);
		}

		public static JobTeamUser GetLastOrDefault(Func<JobTeamUser,bool> match,bool isShort=false) {
			return _jobTeamUserCache.GetLastOrDefault(match,isShort);
		}

		public static List<JobTeamUser> GetWhere(Predicate<JobTeamUser> match,bool isShort=false) {
			return _jobTeamUserCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_jobTeamUserCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_jobTeamUserCache.FillCacheFromTable(table);
				return table;
			}
			return _jobTeamUserCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_jobTeamUserCache.ClearCache();
		}
		#endregion Cache Pattern
		#region Methods - Get
		///<summary></summary>
		public static List<JobTeamUser> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<JobTeamUser>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM jobteamuser WHERE PatNum = "+POut.Long(patNum);
			return Crud.JobTeamUserCrud.SelectMany(command);
		}
		
		///<summary>Gets one JobTeamUser from the db.</summary>
		public static JobTeamUser GetOne(long jobTeamUserNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<JobTeamUser>(MethodBase.GetCurrentMethod(),jobTeamUserNum);
			}
			return Crud.JobTeamUserCrud.SelectOne(jobTeamUserNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(JobTeamUser jobTeamUser){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				jobTeamUser.JobTeamUserNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobTeamUser);
				return jobTeamUser.JobTeamUserNum;
			}
			return Crud.JobTeamUserCrud.Insert(jobTeamUser);
		}

		///<summary></summary>
		public static void Update(JobTeamUser jobTeamUser){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobTeamUser);
				return;
			}
			Crud.JobTeamUserCrud.Update(jobTeamUser);
		}

		///<summary></summary>
		public static bool Sync(List<JobTeamUser> listJobTeamUserNew,List<JobTeamUser> listJobTeamUserOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listJobTeamUserNew,listJobTeamUserOld);
			}
			return Crud.JobTeamUserCrud.Sync(listJobTeamUserNew,listJobTeamUserOld);
		}

		///<summary></summary>
		public static void Delete(long jobTeamUserNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobTeamUserNum);
				return;
			}
			Crud.JobTeamUserCrud.Delete(jobTeamUserNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}