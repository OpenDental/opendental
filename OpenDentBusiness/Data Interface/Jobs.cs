using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Jobs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class JobCache : CacheAbs<Job> {
			protected override List<Job> GetCacheFromDb() {
				string command="SELECT * FROM job";
				return Crud.JobCrud.SelectMany(command);
			}
			protected override List<Job> TableToList(DataTable table) {
				return Crud.JobCrud.TableToList(table);
			}
			protected override Job Copy(Job job) {
				return job.Copy();
			}
			protected override DataTable ListToTable(List<Job> listJobs) {
				return Crud.JobCrud.ListToTable(listJobs,"Job");
			}
			protected override void FillCacheIfNeeded() {
				Jobs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Job job) {
				return true;//Either change this method or delete it.
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static JobCache _jobCache=new JobCache();

		///<summary>A list of all Jobs. Returns a deep copy.</summary>
		public static List<Job> ListDeep {
			get {
				return _jobCache.ListDeep;
			}
		}

		///<summary>A list of all non-hidden Jobs. Returns a deep copy.</summary>
		public static List<Job> ListShortDeep {
			get {
				return _jobCache.ListShortDeep;
			}
		}

		///<summary>A list of all Jobs. Returns a shallow copy.</summary>
		public static List<Job> ListShallow {
			get {
				return _jobCache.ListShallow;
			}
		}

		///<summary>A list of all non-hidden Jobs. Returns a shallow copy.</summary>
		public static List<Job> ListShortShallow {
			get {
				return _jobCache.ListShortShallow;
			}
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_jobCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_jobCache.FillCacheFromTable(table);
				return table;
			}
			return _jobCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<Job> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM job WHERE PatNum = "+POut.Long(patNum);
			return Crud.JobCrud.SelectMany(command);
		}
		
		///<summary>Gets one Job from the db.</summary>
		public static Job GetOne(long jobNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Job>(MethodBase.GetCurrentMethod(),jobNum);
			}
			return Crud.JobCrud.SelectOne(jobNum);
		}
		#endregion
		#region Modification Methods
			#region Insert
		///<summary></summary>
		public static long Insert(Job job){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				job.JobNum=Meth.GetLong(MethodBase.GetCurrentMethod(),job);
				return job.JobNum;
			}
			return Crud.JobCrud.Insert(job);
		}
			#endregion
			#region Update
		///<summary></summary>
		public static void Update(Job job){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),job);
				return;
			}
			Crud.JobCrud.Update(job);
		}
			#endregion
			#region Delete
		///<summary></summary>
		public static void Delete(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			Crud.JobCrud.Delete(jobNum);
		}
			#endregion
		#endregion
		#region Misc Methods
		

		
		#endregion
		*/



	}
}