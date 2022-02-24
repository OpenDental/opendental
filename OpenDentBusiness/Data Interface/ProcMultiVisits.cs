using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcMultiVisits{
		#region Cache Pattern
		///<summary>The procmultivisit table could be quite large after a few years of use by a big organization.
		///However, the number of In Process procedures should be a few thousand or less, even for a large organization.</summary>
		private class ProcMultiVisitCache : CacheListAbs<ProcMultiVisit> {
			protected override List<ProcMultiVisit> GetCacheFromDb() {
				string command="SELECT * FROM procmultivisit WHERE IsInProcess=1";
				return Crud.ProcMultiVisitCrud.SelectMany(command);
			}
			protected override List<ProcMultiVisit> TableToList(DataTable table) {
				return Crud.ProcMultiVisitCrud.TableToList(table);
			}
			protected override ProcMultiVisit Copy(ProcMultiVisit procMultiVisit) {
				return procMultiVisit.Copy();
			}
			protected override DataTable ListToTable(List<ProcMultiVisit> listProcMultiVisits) {
				return Crud.ProcMultiVisitCrud.ListToTable(listProcMultiVisits,"ProcMultiVisit");
			}
			protected override void FillCacheIfNeeded() {
				ProcMultiVisits.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProcMultiVisitCache _procMultiVisitCache=new ProcMultiVisitCache();

		public static List<ProcMultiVisit> GetDeepCopy(bool isShort=false) {
			return _procMultiVisitCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _procMultiVisitCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ProcMultiVisit> match,bool isShort=false) {
			return _procMultiVisitCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ProcMultiVisit> match,bool isShort=false) {
			return _procMultiVisitCache.GetFindIndex(match,isShort);
		}

		public static ProcMultiVisit GetFirst(bool isShort=false) {
			return _procMultiVisitCache.GetFirst(isShort);
		}

		public static ProcMultiVisit GetFirst(Func<ProcMultiVisit,bool> match,bool isShort=false) {
			return _procMultiVisitCache.GetFirst(match,isShort);
		}

		public static ProcMultiVisit GetFirstOrDefault(Func<ProcMultiVisit,bool> match,bool isShort=false) {
			return _procMultiVisitCache.GetFirstOrDefault(match,isShort);
		}

		public static ProcMultiVisit GetLast(bool isShort=false) {
			return _procMultiVisitCache.GetLast(isShort);
		}

		public static ProcMultiVisit GetLastOrDefault(Func<ProcMultiVisit,bool> match,bool isShort=false) {
			return _procMultiVisitCache.GetLastOrDefault(match,isShort);
		}

		public static List<ProcMultiVisit> GetWhere(Predicate<ProcMultiVisit> match,bool isShort=false) {
			return _procMultiVisitCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_procMultiVisitCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_procMultiVisitCache.FillCacheFromTable(table);
				return table;
			}
			return _procMultiVisitCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Insert
		///<summary>Does not send cache refresh signal.  Send the signal from calling code.</summary>
		public static long Insert(ProcMultiVisit procMultiVisit){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				procMultiVisit.ProcMultiVisitNum=Meth.GetLong(MethodBase.GetCurrentMethod(),procMultiVisit);
				return procMultiVisit.ProcMultiVisitNum;
			}
			return Crud.ProcMultiVisitCrud.Insert(procMultiVisit);
		}

		///<summary>Will not create a group if there are less than 2 items in listProcs.  Also sends signal and refreshes cache.</summary>
		public static void CreateGroup(List<Procedure> listProcs) {
			//No need to check RemotingRole; no call to db.
			if(listProcs.Count<2) {//No reason to make a "group" with 0 or 1 items.
				return;
			}
			List<ProcMultiVisit> listPmvs=new List<ProcMultiVisit>();
			for(int i=0;i<listProcs.Count;i++) {
				ProcMultiVisit pmv=new ProcMultiVisit();
				pmv.ProcNum=listProcs[i].ProcNum;
				pmv.ProcStatus=listProcs[i].ProcStatus;
				listPmvs.Add(pmv);
			}
			bool isGroupInProcess=ProcMultiVisits.IsGroupInProcess(listPmvs);//Could be in process if grouped procs which are different statuses via menu.
			long groupProcMultiVisitNum=0;
			for(int i=0;i<listPmvs.Count;i++) {
				ProcMultiVisit pmv=listPmvs[i];
				pmv.IsInProcess=isGroupInProcess;
				if(i==0) {
					groupProcMultiVisitNum=ProcMultiVisits.Insert(pmv);
					ProcMultiVisit oldPmv=pmv.Copy();
					pmv.GroupProcMultiVisitNum=groupProcMultiVisitNum;
					ProcMultiVisits.Update(pmv,oldPmv);//Have to update after insert, or else we cannot know what the primary key is.
				}
				else {
					pmv.GroupProcMultiVisitNum=groupProcMultiVisitNum;
					ProcMultiVisits.Insert(pmv);
				}
			}
			Signalods.SetInvalid(InvalidType.ProcMultiVisits);
			RefreshCache();
		}

		#endregion Insert

		#region Update
		///<summary>Does not send cache refresh signal.  Send the signal from calling code.</summary>
		public static void Update(ProcMultiVisit procMultiVisit,ProcMultiVisit oldProcMultiVisit){
			if(!Crud.ProcMultiVisitCrud.UpdateComparison(procMultiVisit,oldProcMultiVisit)) {
				return;//No changes.  Save middle tier call.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procMultiVisit,oldProcMultiVisit);
				return;
			}
			Crud.ProcMultiVisitCrud.Update(procMultiVisit,oldProcMultiVisit);
		}

		///<summary>Responsible for updating procedures in the group to "In Process" or "Not In Process", depending on the stat passed in.
		///Also sends signal to cause cache refresh.  Refreshes local cache for clients directly connected.</summary>
		public static void UpdateGroupForProc(long procNum,ProcStat stat) {
			//No need to check RemotingRole; no call to db.
			List<ProcMultiVisit> listPmvs=GetGroupsForProcsFromDb(procNum);
			ProcMultiVisit pmv=listPmvs.FirstOrDefault(x => x.ProcNum==procNum);
			if(pmv==null) {
				return;//Rare edge case.  Might happen is someone deletes the procedure at the same time another person is updating it.
			}
			bool isGroupInProcessOld=IsGroupInProcess(listPmvs);
			if(stat==ProcStat.D) {
				//If the procedure is deleted, also delete the procvisitmulti to reduce clutter.
				listPmvs.Remove(pmv);//Remove pmv from listpmvs.
				if(pmv.ProcMultiVisitNum==pmv.GroupProcMultiVisitNum && !listPmvs.IsNullOrEmpty()) {//If the group points to the pmv to be removed and the group still exists.
					long replacementGPMVNum=listPmvs.First().ProcMultiVisitNum;
					UpdateGroupProcMultiVisitNumForGroup(pmv.ProcMultiVisitNum,replacementGPMVNum);
					foreach(ProcMultiVisit procMulti in listPmvs) {//Replace all group numbers.
						procMulti.GroupProcMultiVisitNum=replacementGPMVNum;
					}
				}
				Delete(pmv.ProcMultiVisitNum);
			}
			else {
				ProcMultiVisit oldPmv=pmv.Copy();
				pmv.ProcStatus=stat;
				Update(pmv,oldPmv);
			}
			bool isGroupInProcess=IsGroupInProcess(listPmvs);
			if(isGroupInProcess!=isGroupInProcessOld) {
				UpdateInProcessForGroup(pmv.GroupProcMultiVisitNum,isGroupInProcess);
			}
			//Always send a signal and refresh the cache in case someone else is going to edit the group soon.
			Signalods.SetInvalid(InvalidType.ProcMultiVisits);
			if(RemotingClient.RemotingRole==RemotingRole.ClientDirect) {
				RefreshCache();
			}
		}

		///<summary>Updates the group IsInProcess values for all procedures to the specified bool value.
		///Does not send cache refresh signal.  Send the signal from calling code.</summary>
		public static void UpdateInProcessForGroup(long groupProcMultiVisitNum,bool isGroupInProcess) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupProcMultiVisitNum,isGroupInProcess);
				return;
			}
			string command="UPDATE procmultivisit "
				+"SET IsInProcess="+POut.Bool(isGroupInProcess)+" "
				+"WHERE GroupProcMultiVisitNum="+POut.Long(groupProcMultiVisitNum);
			Db.NonQ(command);
		}

		///<summary>Update the parameter GroupProcMultiVisitNum to a new value.
		///Does not send cache refresh signal.  Send the signal from calling code.</summary>
		public static void UpdateGroupProcMultiVisitNumForGroup(long groupProcMultiVisitNumOld,long groupProcMultiVisitNumNew) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupProcMultiVisitNumOld,groupProcMultiVisitNumNew);
				return;
			}
			string command="UPDATE procmultivisit "
				+"SET GroupProcMultiVisitNum="+POut.Long(groupProcMultiVisitNumNew)+" "
				+"WHERE GroupProcMultiVisitNum="+POut.Long(groupProcMultiVisitNumOld);
			Db.NonQ(command);
		}

		#endregion Update

		#region Delete
		///<summary>Does not send cache refresh signal.  Send the signal from calling code.</summary>
		public static void Delete(long procMultiVisitNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procMultiVisitNum);
				return;
			}
			Crud.ProcMultiVisitCrud.Delete(procMultiVisitNum);
		}
		#endregion Delete

		#region Get Methods

		///<summary>Calls db to get most up to date information.</summary>
		public static List<ProcMultiVisit> GetGroupsForProcsFromDb(params long[] arrayProcNums){
			if(arrayProcNums.IsNullOrEmpty()) {
				return new List<ProcMultiVisit>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<ProcMultiVisit>>(MethodBase.GetCurrentMethod(),arrayProcNums);
			}
			string command="SELECT * FROM procmultivisit "
				+"WHERE GroupProcMultiVisitNum IN (SELECT GroupProcMultiVisitNum FROM procmultivisit p2 WHERE p2.ProcNum IN ("+String.Join(",",arrayProcNums)+"))";
			return Crud.ProcMultiVisitCrud.SelectMany(command);
		}

		#endregion Get Methods

		#region Misc Methods

		///<summary>Returns true if the group of multi visits passed in is "In Process".
		///This method assumes that all ProcMultiVisits passed in are all of the entities for the group.
		///No db call.  Only uses the passed in list.</summary>
		public static bool IsGroupInProcess(List<ProcMultiVisit> listPmvs) {
			//No need to check RemotingRole; no call to db.
			//A group is considered "In Process" if at least one procedure is treatment planned and at least one is complete.
			return (listPmvs.Exists(x => ListTools.In(x.ProcStatus,ProcStat.TP,ProcStat.TPi)) && listPmvs.Exists(x => ListTools.In(x.ProcStatus,ProcStat.C)));
		}

		///<summary>Returns true if the group the procedure belongs to is in process and the procedure status is set to "complete".
		///If isAssumedComplete, then will pretend the procedure for procNum is complete regardless of current status.</summary>
		public static bool IsProcInProcess(long procNum,bool isAssumedComplete=false) {
			//No need to check RemotingRole; no call to db.
			ProcMultiVisit pmv=GetFirstOrDefault(x => x.ProcNum==procNum);
			//The existience of the procmultivisit in the cache means the procedure's group is In Process.
			if(pmv==null) {
				return false;
			}
			//Now all we need to check is ProcStatus.
			if(isAssumedComplete) {
				List <ProcMultiVisit> listPmvs=GetWhere(x => x.GroupProcMultiVisitNum==pmv.GroupProcMultiVisitNum);//Makes a deep copy.
				for(int i=0;i<listPmvs.Count;i++) {
					if(listPmvs[i].ProcNum==procNum) {
						listPmvs[i].ProcStatus=ProcStat.C;//We can safely change status directly, since GetWhere() returned a deep copy above.
						break;
					}
				}
				return IsGroupInProcess(listPmvs);//Since are are assuming proc is complete, all we need to know is if the group would be in process or not.
			}
			return (pmv.IsInProcess && pmv.ProcStatus==ProcStat.C);//Part of an in process group and is complete.
		}

		#endregion
	}
}