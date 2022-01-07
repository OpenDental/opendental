using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrNotPerformeds{
		//If this table type will exist as cached data, uncomment the CachePattern region below.
		/*
		#region CachePattern

		private class EhrLabNotPerformedCache : CacheListAbs<EhrLabNotPerformed> {
			protected override List<EhrLabNotPerformed> GetCacheFromDb() {
				string command="SELECT * FROM EhrLabNotPerformed ORDER BY ItemOrder";
				return Crud.EhrLabNotPerformedCrud.SelectMany(command);
			}
			protected override List<EhrLabNotPerformed> TableToList(DataTable table) {
				return Crud.EhrLabNotPerformedCrud.TableToList(table);
			}
			protected override EhrLabNotPerformed Copy(EhrLabNotPerformed EhrLabNotPerformed) {
				return EhrLabNotPerformed.Clone();
			}
			protected override DataTable ListToTable(List<EhrLabNotPerformed> listEhrLabNotPerformeds) {
				return Crud.EhrLabNotPerformedCrud.ListToTable(listEhrLabNotPerformeds,"EhrLabNotPerformed");
			}
			protected override void FillCacheIfNeeded() {
				EhrLabNotPerformeds.GetTableFromCache(false);
			}
			protected override bool IsInListShort(EhrLabNotPerformed EhrLabNotPerformed) {
				return !EhrLabNotPerformed.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EhrLabNotPerformedCache _EhrLabNotPerformedCache=new EhrLabNotPerformedCache();

		///<summary>A list of all EhrLabNotPerformeds. Returns a deep copy.</summary>
		public static List<EhrLabNotPerformed> ListDeep {
			get {
				return _EhrLabNotPerformedCache.ListDeep;
			}
		}

		///<summary>A list of all visible EhrLabNotPerformeds. Returns a deep copy.</summary>
		public static List<EhrLabNotPerformed> ListShortDeep {
			get {
				return _EhrLabNotPerformedCache.ListShortDeep;
			}
		}

		///<summary>A list of all EhrLabNotPerformeds. Returns a shallow copy.</summary>
		public static List<EhrLabNotPerformed> ListShallow {
			get {
				return _EhrLabNotPerformedCache.ListShallow;
			}
		}

		///<summary>A list of all visible EhrLabNotPerformeds. Returns a shallow copy.</summary>
		public static List<EhrLabNotPerformed> ListShort {
			get {
				return _EhrLabNotPerformedCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EhrLabNotPerformedCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EhrLabNotPerformedCache.FillCacheFromTable(table);
				return table;
			}
			return _EhrLabNotPerformedCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static List<EhrNotPerformed> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrNotPerformed>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrnotperformed WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DateEntry";
			return Crud.EhrNotPerformedCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EhrNotPerformed ehrNotPerformed){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ehrNotPerformed.EhrNotPerformedNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrNotPerformed);
				return ehrNotPerformed.EhrNotPerformedNum;
			}
			return Crud.EhrNotPerformedCrud.Insert(ehrNotPerformed);
		}

		///<summary></summary>
		public static void Update(EhrNotPerformed ehrNotPerformed){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrNotPerformed);
				return;
			}
			Crud.EhrNotPerformedCrud.Update(ehrNotPerformed);
		}

		///<summary></summary>
		public static void Delete(long ehrNotPerformedNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrNotPerformedNum);
				return;
			}
			string command= "DELETE FROM ehrnotperformed WHERE EhrNotPerformedNum = "+POut.Long(ehrNotPerformedNum);
			Db.NonQ(command);
		}

		///<summary>Gets one EhrNotPerformed from the db.</summary>
		public static EhrNotPerformed GetOne(long ehrNotPerformedNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EhrNotPerformed>(MethodBase.GetCurrentMethod(),ehrNotPerformedNum);
			}
			return Crud.EhrNotPerformedCrud.SelectOne(ehrNotPerformedNum);
		}
	}
}