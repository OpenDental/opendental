using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RecallTriggers{
		#region CachePattern

		private class RecallTriggerCache : CacheListAbs<RecallTrigger> {
			protected override List<RecallTrigger> GetCacheFromDb() {
				string command="SELECT * FROM recalltrigger";
				return Crud.RecallTriggerCrud.SelectMany(command);
			}
			protected override List<RecallTrigger> TableToList(DataTable table) {
				return Crud.RecallTriggerCrud.TableToList(table);
			}
			protected override RecallTrigger Copy(RecallTrigger recallTrigger) {
				return recallTrigger.Copy();
			}
			protected override DataTable ListToTable(List<RecallTrigger> listRecallTriggers) {
				return Crud.RecallTriggerCrud.ListToTable(listRecallTriggers,"RecallTrigger");
			}
			protected override void FillCacheIfNeeded() {
				RecallTriggers.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static RecallTriggerCache _recallTriggerCache=new RecallTriggerCache();

		public static List<RecallTrigger> GetWhere(Predicate<RecallTrigger> match,bool isShort=false) {
			return _recallTriggerCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_recallTriggerCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_recallTriggerCache.FillCacheFromTable(table);
				return table;
			}
			return _recallTriggerCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(RecallTrigger trigger) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				trigger.RecallTriggerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),trigger);
				return trigger.RecallTriggerNum;
			}
			return Crud.RecallTriggerCrud.Insert(trigger);
		}

		/*
		///<summary></summary>
		public static void Update(RecallTrigger trigger) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),trigger);
				return;
			}
			Crud.RecallTriggerCrud.Update(trigger);
		}*/

		public static List<RecallTrigger> GetForType(long recallTypeNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.RecallTypeNum==recallTypeNum);
		}

		public static void SetForType(long recallTypeNum,List<RecallTrigger> triggerList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),recallTypeNum,triggerList);
				return;
			}
			string command="DELETE FROM recalltrigger WHERE RecallTypeNum="+POut.Long(recallTypeNum);
			Db.NonQ(command);
			for(int i=0;i<triggerList.Count;i++){
				triggerList[i].RecallTypeNum=recallTypeNum;
				Insert(triggerList[i]);
			}
		}
	}
}