using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DictCustoms{
		#region CachePattern

		private class DictCustomCache : CacheListAbs<DictCustom> {
			protected override List<DictCustom> GetCacheFromDb() {
				string command="SELECT * FROM dictcustom ORDER BY WordText";
				return Crud.DictCustomCrud.SelectMany(command);
			}
			protected override List<DictCustom> TableToList(DataTable table) {
				return Crud.DictCustomCrud.TableToList(table);
			}
			protected override DictCustom Copy(DictCustom dictCustom) {
				return dictCustom.Copy();
			}
			protected override DataTable ListToTable(List<DictCustom> listDictCustoms) {
				return Crud.DictCustomCrud.ListToTable(listDictCustoms,"DictCustom");
			}
			protected override void FillCacheIfNeeded() {
				DictCustoms.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DictCustomCache _dictCustomCache=new DictCustomCache();

		public static List<DictCustom> GetDeepCopy(bool isShort=false) {
			return _dictCustomCache.GetDeepCopy(isShort);
		}

		public static DictCustom GetFirstOrDefault(Func<DictCustom,bool> match,bool isShort=false) {
			return _dictCustomCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_dictCustomCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_dictCustomCache.FillCacheFromTable(table);
				return table;
			}
			return _dictCustomCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(DictCustom dictCustom){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				dictCustom.DictCustomNum=Meth.GetLong(MethodBase.GetCurrentMethod(),dictCustom);
				return dictCustom.DictCustomNum;
			}
			return Crud.DictCustomCrud.Insert(dictCustom);
		}

		///<summary></summary>
		public static void Update(DictCustom dictCustom){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dictCustom);
				return;
			}
			Crud.DictCustomCrud.Update(dictCustom);
		}

		///<summary></summary>
		public static void Delete(long dictCustomNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dictCustomNum);
				return;
			}
			Crud.DictCustomCrud.Delete(dictCustomNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<DictCustom> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<DictCustom>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM dictcustom WHERE PatNum = "+POut.Long(patNum);
			return Crud.DictCustomCrud.SelectMany(command);
		}
		
		///<summary>Gets one DictCustom from the db.</summary>
		public static DictCustom GetOne(long dictCustomNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<DictCustom>(MethodBase.GetCurrentMethod(),dictCustomNum);
			}
			return Crud.DictCustomCrud.SelectOne(dictCustomNum);
		}
		 

		
		*/
	}
}