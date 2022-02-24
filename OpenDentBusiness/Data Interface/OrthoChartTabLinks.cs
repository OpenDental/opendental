using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoChartTabLinks{
		#region CachePattern

		private class OrthoChartTabLinkCache : CacheListAbs<OrthoChartTabLink> {
			protected override List<OrthoChartTabLink> GetCacheFromDb() {
				string command="SELECT * FROM orthocharttablink ORDER BY ItemOrder";
				return Crud.OrthoChartTabLinkCrud.SelectMany(command);
			}
			protected override List<OrthoChartTabLink> TableToList(DataTable table) {
				return Crud.OrthoChartTabLinkCrud.TableToList(table);
			}
			protected override OrthoChartTabLink Copy(OrthoChartTabLink orthoChartTabLink) {
				return orthoChartTabLink.Copy();
			}
			protected override DataTable ListToTable(List<OrthoChartTabLink> listOrthoChartTabLinks) {
				return Crud.OrthoChartTabLinkCrud.ListToTable(listOrthoChartTabLinks,"OrthoChartTabLink");
			}
			protected override void FillCacheIfNeeded() {
				OrthoChartTabLinks.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static OrthoChartTabLinkCache _orthoChartTabLinkCache=new OrthoChartTabLinkCache();

		public static bool GetExists(Predicate<OrthoChartTabLink> match,bool isShort=false) {
			return _orthoChartTabLinkCache.GetExists(match,isShort);
		}

		public static List<OrthoChartTabLink> GetDeepCopy(bool isShort=false) {
			return _orthoChartTabLinkCache.GetDeepCopy(isShort);
		}

		public static List<OrthoChartTabLink> GetWhere(Predicate<OrthoChartTabLink> match,bool isShort=false) {
			return _orthoChartTabLinkCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_orthoChartTabLinkCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_orthoChartTabLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _orthoChartTabLinkCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Inserts, updates, or deletes the passed in list against the stale list listOld.  Returns true if db changes were made.</summary>
		public static bool Sync(List<OrthoChartTabLink> listNew,List<OrthoChartTabLink> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.OrthoChartTabLinkCrud.Sync(listNew,listOld);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<OrthoChartTabLink> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChartTabLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthocharttablink WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrthoChartTabLinkCrud.SelectMany(command);
		}

		///<summary>Gets one OrthoChartTabLink from the db.</summary>
		public static OrthoChartTabLink GetOne(long orthoChartTabLinkNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OrthoChartTabLink>(MethodBase.GetCurrentMethod(),orthoChartTabLinkNum);
			}
			return Crud.OrthoChartTabLinkCrud.SelectOne(orthoChartTabLinkNum);
		}

		///<summary></summary>
		public static long Insert(OrthoChartTabLink orthoChartTabLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				orthoChartTabLink.OrthoChartTabLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoChartTabLink);
				return orthoChartTabLink.OrthoChartTabLinkNum;
			}
			return Crud.OrthoChartTabLinkCrud.Insert(orthoChartTabLink);
		}

		///<summary></summary>
		public static void Update(OrthoChartTabLink orthoChartTabLink){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartTabLink);
				return;
			}
			Crud.OrthoChartTabLinkCrud.Update(orthoChartTabLink);
		}

		///<summary></summary>
		public static void Delete(long orthoChartTabLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartTabLinkNum);
				return;
			}
			Crud.OrthoChartTabLinkCrud.Delete(orthoChartTabLinkNum);
		}

		

		
		*/
	}
}