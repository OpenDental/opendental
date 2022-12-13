using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobTeams{
		#region Cache Pattern
		private class JobTeamCache : CacheListAbs<JobTeam> {
			protected override List<JobTeam> GetCacheFromDb() {
				string command="SELECT * FROM jobteam";
				return Crud.JobTeamCrud.SelectMany(command);
			}
			protected override List<JobTeam> TableToList(DataTable table) {
				return Crud.JobTeamCrud.TableToList(table);
			}
			protected override JobTeam Copy(JobTeam jobTeam) {
				return jobTeam.Copy();
			}
			protected override DataTable ListToTable(List<JobTeam> listJobTeams) {
				return Crud.JobTeamCrud.ListToTable(listJobTeams,"JobTeam");
			}
			protected override void FillCacheIfNeeded() {
				JobTeams.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static JobTeamCache _jobTeamCache=new JobTeamCache();

		public static List<JobTeam> GetDeepCopy(bool isShort=false) {
			return _jobTeamCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _jobTeamCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<JobTeam> match,bool isShort=false) {
			return _jobTeamCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<JobTeam> match,bool isShort=false) {
			return _jobTeamCache.GetFindIndex(match,isShort);
		}

		public static JobTeam GetFirst(bool isShort=false) {
			return _jobTeamCache.GetFirst(isShort);
		}

		public static JobTeam GetFirst(Func<JobTeam,bool> match,bool isShort=false) {
			return _jobTeamCache.GetFirst(match,isShort);
		}

		public static JobTeam GetFirstOrDefault(Func<JobTeam,bool> match,bool isShort=false) {
			return _jobTeamCache.GetFirstOrDefault(match,isShort);
		}

		public static JobTeam GetLast(bool isShort=false) {
			return _jobTeamCache.GetLast(isShort);
		}

		public static JobTeam GetLastOrDefault(Func<JobTeam,bool> match,bool isShort=false) {
			return _jobTeamCache.GetLastOrDefault(match,isShort);
		}

		public static List<JobTeam> GetWhere(Predicate<JobTeam> match,bool isShort=false) {
			return _jobTeamCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_jobTeamCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_jobTeamCache.FillCacheFromTable(table);
				return table;
			}
			return _jobTeamCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		#region Methods - Get
		///<summary></summary>
		public static List<JobTeam> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<JobTeam>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM jobteam WHERE PatNum = "+POut.Long(patNum);
			return Crud.JobTeamCrud.SelectMany(command);
		}

		///<summary>Gets one JobTeam from the db.</summary>
		public static JobTeam GetOne(long jobTeamNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<JobTeam>(MethodBase.GetCurrentMethod(),jobTeamNum);
			}
			return Crud.JobTeamCrud.SelectOne(jobTeamNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(JobTeam jobTeam){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				jobTeam.JobTeamNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobTeam);
				return jobTeam.JobTeamNum;
			}
			return Crud.JobTeamCrud.Insert(jobTeam);
		}

		///<summary></summary>
		public static void Update(JobTeam jobTeam){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobTeam);
				return;
			}
			Crud.JobTeamCrud.Update(jobTeam);
		}

		///<summary></summary>
		public static bool Sync(List<JobTeam> listJobTeamNew,List<JobTeam> listJobTeamOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listJobTeamNew,listJobTeamOld);
			}
			return Crud.JobTeamCrud.Sync(listJobTeamNew,listJobTeamOld);
		}

		///<summary></summary>
		public static void Delete(long jobTeamNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobTeamNum);
				return;
			}
			Crud.JobTeamCrud.Delete(jobTeamNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}