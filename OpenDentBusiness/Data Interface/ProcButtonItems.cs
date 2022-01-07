using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcButtonItems {
		#region CachePattern

		private class ProcButtonItemCache : CacheListAbs<ProcButtonItem> {
			protected override List<ProcButtonItem> GetCacheFromDb() {
				string command="SELECT * FROM procbuttonitem ORDER BY ItemOrder";
				return Crud.ProcButtonItemCrud.SelectMany(command);
			}
			protected override List<ProcButtonItem> TableToList(DataTable table) {
				return Crud.ProcButtonItemCrud.TableToList(table);
			}
			protected override ProcButtonItem Copy(ProcButtonItem procButtonItem) {
				return procButtonItem.Copy();
			}
			protected override DataTable ListToTable(List<ProcButtonItem> listProcButtonItems) {
				return Crud.ProcButtonItemCrud.ListToTable(listProcButtonItems,"ProcButtonItem");
			}
			protected override void FillCacheIfNeeded() {
				ProcButtonItems.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProcButtonItemCache _procButtonItemCache=new ProcButtonItemCache();

		public static List<ProcButtonItem> GetDeepCopy(bool isShort=false) {
			return _procButtonItemCache.GetDeepCopy(isShort);
		}

		private static List<ProcButtonItem> GetWhere(Predicate<ProcButtonItem> match,bool isShort=false) {
			return _procButtonItemCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_procButtonItemCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_procButtonItemCache.FillCacheFromTable(table);
				return table;
			}
			return _procButtonItemCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Must have already checked procCode for nonduplicate.</summary>
		public static long Insert(ProcButtonItem item) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				item.ProcButtonItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),item);
				return item.ProcButtonItemNum;
			}
			return Crud.ProcButtonItemCrud.Insert(item);
		}

		///<summary></summary>
		public static void Update(ProcButtonItem item) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),item);
				return;
			}
			Crud.ProcButtonItemCrud.Update(item);
		}

		///<summary></summary>
		public static void Delete(ProcButtonItem item) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),item);
				return;
			}
			string command="DELETE FROM procbuttonitem WHERE ProcButtonItemNum = '"+POut.Long(item.ProcButtonItemNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Sorted by Item Order.</summary>
		public static long[] GetCodeNumListForButton(long procButtonNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ProcButtonNum==procButtonNum && x.CodeNum > 0)
				.OrderBy(x => x.ItemOrder)
				.Select(x => x.CodeNum)
				.ToArray();
		}

		///<summary>Sorted by Item Order.</summary>
		public static long[] GetAutoListForButton(long procButtonNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ProcButtonNum==procButtonNum && x.AutoCodeNum > 0)
				.OrderBy(x => x.ItemOrder)
				.Select(x => x.AutoCodeNum).ToArray();
		}

		///<summary></summary>
		public static void DeleteAllForButton(long procButtonNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),procButtonNum);
				return;
			}
			string command= "DELETE from procbuttonitem WHERE procbuttonnum = '"+POut.Long(procButtonNum)+"'";
			Db.NonQ(command);
		}
	}

	




}










