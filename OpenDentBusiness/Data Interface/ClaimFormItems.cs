using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimFormItems {
		#region Cache Pattern

		private class ClaimFormItemCache : CacheListAbs<ClaimFormItem> {
			protected override List<ClaimFormItem> GetCacheFromDb() {
				string command="SELECT * FROM claimformitem ORDER BY ImageFileName DESC";
				return Crud.ClaimFormItemCrud.SelectMany(command);
			}
			protected override List<ClaimFormItem> TableToList(DataTable table) {
				return Crud.ClaimFormItemCrud.TableToList(table);
			}
			protected override ClaimFormItem Copy(ClaimFormItem claimFormItem) {
				return claimFormItem.Copy();
			}
			protected override DataTable ListToTable(List<ClaimFormItem> listClaimFormItems) {
				return Crud.ClaimFormItemCrud.ListToTable(listClaimFormItems,"ClaimFormItem");
			}
			protected override void FillCacheIfNeeded() {
				ClaimFormItems.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ClaimFormItemCache _claimFormItemCache=new ClaimFormItemCache();

		public static List<ClaimFormItem> GetWhere(Predicate<ClaimFormItem> match,bool isShort=false) {
			return _claimFormItemCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_claimFormItemCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_claimFormItemCache.FillCacheFromTable(table);
				return table;
			}
			return _claimFormItemCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(ClaimFormItem item) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				item.ClaimFormItemNum=Meth.GetLong(MethodBase.GetCurrentMethod(),item);
				return item.ClaimFormItemNum;
			}
			return Crud.ClaimFormItemCrud.Insert(item);
		}

		///<summary></summary>
		public static void Update(ClaimFormItem item){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),item);
				return;
			}
			Crud.ClaimFormItemCrud.Update(item);
		}

		///<summary></summary>
		public static void Delete(ClaimFormItem item){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),item);
				return;
			}
			string command = "DELETE FROM claimformitem "
				+"WHERE ClaimFormItemNum = '"+POut.Long(item.ClaimFormItemNum)+"'";
 			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteAllForClaimForm(long claimFormNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimFormNum);
				return;
			}
			string command="DELETE FROM claimformitem WHERE ClaimFormNum = '"+POut.Long(claimFormNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Gets all claimformitems for the specified claimform from the preloaded List.</summary>
		public static List<ClaimFormItem> GetListForForm(long claimFormNum) {
			//No need to check RemotingRole; no call to db.
			return ClaimFormItems.GetWhere(x => x.ClaimFormNum==claimFormNum);
		}
	}

	

	

}









