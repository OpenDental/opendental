using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskAttachments{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		#region Cache Pattern
		/*//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class TaskAttachmentCache : CacheListAbs<TaskAttachment> {
			protected override List<TaskAttachment> GetCacheFromDb() {
				string command="SELECT * FROM taskattachment";
				return Crud.TaskAttachmentCrud.SelectMany(command);
			}
			protected override List<TaskAttachment> TableToList(DataTable table) {
				return Crud.TaskAttachmentCrud.TableToList(table);
			}
			protected override TaskAttachment Copy(TaskAttachment taskAttachment) {
				return taskAttachment.Copy();
			}
			protected override DataTable ListToTable(List<TaskAttachment> listTaskAttachments) {
				return Crud.TaskAttachmentCrud.ListToTable(listTaskAttachments,"TaskAttachment");
			}
			protected override void FillCacheIfNeeded() {
				TaskAttachments.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static TaskAttachmentCache _taskAttachmentCache=new TaskAttachmentCache();

		public static List<TaskAttachment> GetDeepCopy(bool isShort=false) {
			return _taskAttachmentCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _taskAttachmentCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<TaskAttachment> match,bool isShort=false) {
			return _taskAttachmentCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<TaskAttachment> match,bool isShort=false) {
			return _taskAttachmentCache.GetFindIndex(match,isShort);
		}

		public static TaskAttachment GetFirst(bool isShort=false) {
			return _taskAttachmentCache.GetFirst(isShort);
		}

		public static TaskAttachment GetFirst(Func<TaskAttachment,bool> match,bool isShort=false) {
			return _taskAttachmentCache.GetFirst(match,isShort);
		}

		public static TaskAttachment GetFirstOrDefault(Func<TaskAttachment,bool> match,bool isShort=false) {
			return _taskAttachmentCache.GetFirstOrDefault(match,isShort);
		}

		public static TaskAttachment GetLast(bool isShort=false) {
			return _taskAttachmentCache.GetLast(isShort);
		}

		public static TaskAttachment GetLastOrDefault(Func<TaskAttachment,bool> match,bool isShort=false) {
			return _taskAttachmentCache.GetLastOrDefault(match,isShort);
		}

		public static List<TaskAttachment> GetWhere(Predicate<TaskAttachment> match,bool isShort=false) {
			return _taskAttachmentCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_taskAttachmentCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_taskAttachmentCache.FillCacheFromTable(table);
				return table;
			}
			return _taskAttachmentCache.GetTableFromCache(doRefreshCache);
		}*/
		#endregion Cache Pattern
		
		#region Methods - Get		
		///<summary>Gets one TaskAttachment from the db.</summary>
		public static TaskAttachment GetOne(long taskAttachmentNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TaskAttachment>(MethodBase.GetCurrentMethod(),taskAttachmentNum);
			}
			return Crud.TaskAttachmentCrud.SelectOne(taskAttachmentNum);
		}

		///<summary>Retrieves attachment linked to the passed in docNum. If no attachment is linked to the docNum, then it returns null.</summary>
		public static TaskAttachment GetOneByDocNum(long docNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TaskAttachment>(MethodBase.GetCurrentMethod(),docNum);
			}
			string command="SELECT * FROM taskattachment WHERE DocNum = "+POut.Long(docNum);
			return Crud.TaskAttachmentCrud.SelectOne(command);
		}

		///<summary>Returns all attachments linked to the passed in docNum.</summary>
		public static List<TaskAttachment> GeyManyByDocNum(long docNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<TaskAttachment>>(MethodBase.GetCurrentMethod(),docNum);
			}
			string command="SELECT * FROM taskattachment WHERE DocNum = "+POut.Long(docNum);
			return Crud.TaskAttachmentCrud.SelectMany(command);
		}

		///<summary>Returns all attachments linked to the list of tasknums passed in.</summary>
		public static List<TaskAttachment> GetForTaskNums(List<long> listTaskNums) {
			if(listTaskNums==null || listTaskNums.Count==0) {
				return new List<TaskAttachment>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskAttachment>>(MethodBase.GetCurrentMethod(),listTaskNums);
			}
			string command="SELECT * FROM taskattachment WHERE TaskNum IN ("+string.Join(",",listTaskNums)+")";
			return Crud.TaskAttachmentCrud.SelectMany(command);
		}

		///<summary>Gets all attachments for a task directly from the db. Does not use cache.</summary>
		public static List<TaskAttachment> GetManyByTaskNum(long taskNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskAttachment>>(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command="SELECT * FROM taskattachment WHERE TaskNum = "+POut.Long(taskNum);
			return Crud.TaskAttachmentCrud.SelectMany(command);
		}

		///<summary>Gets the number of documents linked to a task.</summary>
		public static int GetCountDocumentForTaskNum(long taskNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<int>(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command="SELECT COUNT(*) FROM taskattachment WHERE TaskNum = "+POut.Long(taskNum)+" AND DocNum > 0";
			return PIn.Int(Db.GetCount(command));
		}

		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(TaskAttachment taskAttachment){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				taskAttachment.TaskAttachmentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),taskAttachment);
				return taskAttachment.TaskAttachmentNum;
			}
			return Crud.TaskAttachmentCrud.Insert(taskAttachment);
		}
		///<summary></summary>
		public static void Update(TaskAttachment taskAttachment){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskAttachment);
				return;
			}
			Crud.TaskAttachmentCrud.Update(taskAttachment);
		}

		public static bool Update(TaskAttachment taskAttachment,TaskAttachment taskAttachmentOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetBool(MethodBase.GetCurrentMethod(),taskAttachment,taskAttachmentOld);
			}
			return Crud.TaskAttachmentCrud.Update(taskAttachment,taskAttachmentOld);
		}

		///<summary></summary>
		public static void Delete(long taskAttachmentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskAttachmentNum);
				return;
			}
			Crud.TaskAttachmentCrud.Delete(taskAttachmentNum);
		}
		#endregion Methods - Modify
	}
}