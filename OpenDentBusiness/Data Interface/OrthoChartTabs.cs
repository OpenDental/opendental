using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoChartTabs{
		#region CachePattern

		private class OrthoChartTabCache : CacheListAbs<OrthoChartTab> {
			protected override List<OrthoChartTab> GetCacheFromDb() {
				string command="SELECT * FROM orthocharttab ORDER BY ItemOrder";
				return Crud.OrthoChartTabCrud.SelectMany(command);
			}
			protected override List<OrthoChartTab> TableToList(DataTable table) {
				return Crud.OrthoChartTabCrud.TableToList(table);
			}
			protected override OrthoChartTab Copy(OrthoChartTab orthoChartTab) {
				return orthoChartTab.Copy();
			}
			protected override DataTable ListToTable(List<OrthoChartTab> listOrthoChartTabs) {
				return Crud.OrthoChartTabCrud.ListToTable(listOrthoChartTabs,"OrthoChartTab");
			}
			protected override void FillCacheIfNeeded() {
				OrthoChartTabs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(OrthoChartTab orthoChartTab) {
				return !orthoChartTab.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static OrthoChartTabCache _orthoChartTabCache=new OrthoChartTabCache();

		public static List<OrthoChartTab> GetDeepCopy(bool isShort=false) {
			return _orthoChartTabCache.GetDeepCopy(isShort);
		}

		public static OrthoChartTab GetFirst(bool isShort=false) {
			return _orthoChartTabCache.GetFirst(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _orthoChartTabCache.GetCount(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_orthoChartTabCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_orthoChartTabCache.FillCacheFromTable(table);
				return table;
			}
			return _orthoChartTabCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Inserts, updates, or deletes the passed in list against the stale list listOld.  Returns true if db changes were made.</summary>
		public static bool Sync(List<OrthoChartTab> listNew,List<OrthoChartTab> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.OrthoChartTabCrud.Sync(listNew,listOld);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<OrthoChartTab> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChartTab>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthocharttab WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrthoChartTabCrud.SelectMany(command);
		}

		///<summary>Gets one OrthoChartTab from the db.</summary>
		public static OrthoChartTab GetOne(long orthoChartTabNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OrthoChartTab>(MethodBase.GetCurrentMethod(),orthoChartTabNum);
			}
			return Crud.OrthoChartTabCrud.SelectOne(orthoChartTabNum);
		}

		///<summary></summary>
		public static long Insert(OrthoChartTab orthoChartTab){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				orthoChartTab.OrthoChartTabNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoChartTab);
				return orthoChartTab.OrthoChartTabNum;
			}
			return Crud.OrthoChartTabCrud.Insert(orthoChartTab);
		}

		///<summary></summary>
		public static void Update(OrthoChartTab orthoChartTab){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartTab);
				return;
			}
			Crud.OrthoChartTabCrud.Update(orthoChartTab);
		}

		///<summary></summary>
		public static void Delete(long orthoChartTabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartTabNum);
				return;
			}
			Crud.OrthoChartTabCrud.Delete(orthoChartTabNum);
		}

		

		
		*/
	}
}