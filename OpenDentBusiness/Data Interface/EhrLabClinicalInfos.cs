using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrLabClinicalInfos {
		///<summary></summary>
		public static List<EhrLabClinicalInfo> GetForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabClinicalInfo>>(MethodBase.GetCurrentMethod(),ehrLabNum);
			}
			string command="SELECT * FROM ehrlabclinicalinfo WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			return Crud.EhrLabClinicalInfoCrud.SelectMany(command);
		}

		///<summary>Deletes notes for lab results too.</summary>
		public static void DeleteForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNum);
				return;
			}
			string command="DELETE FROM ehrlabclinicalinfo WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(EhrLabClinicalInfo ehrLabClinicalInfo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrLabClinicalInfo.EhrLabClinicalInfoNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabClinicalInfo);
				return ehrLabClinicalInfo.EhrLabClinicalInfoNum;
			}
			return Crud.EhrLabClinicalInfoCrud.Insert(ehrLabClinicalInfo);
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class EhrLabClinicalInfoCache : CacheListAbs<EhrLabClinicalInfo> {
			protected override List<EhrLabClinicalInfo> GetCacheFromDb() {
				string command="SELECT * FROM EhrLabClinicalInfo ORDER BY ItemOrder";
				return Crud.EhrLabClinicalInfoCrud.SelectMany(command);
			}
			protected override List<EhrLabClinicalInfo> TableToList(DataTable table) {
				return Crud.EhrLabClinicalInfoCrud.TableToList(table);
			}
			protected override EhrLabClinicalInfo Copy(EhrLabClinicalInfo EhrLabClinicalInfo) {
				return EhrLabClinicalInfo.Clone();
			}
			protected override DataTable ListToTable(List<EhrLabClinicalInfo> listEhrLabClinicalInfos) {
				return Crud.EhrLabClinicalInfoCrud.ListToTable(listEhrLabClinicalInfos,"EhrLabClinicalInfo");
			}
			protected override void FillCacheIfNeeded() {
				EhrLabClinicalInfos.GetTableFromCache(false);
			}
			protected override bool IsInListShort(EhrLabClinicalInfo EhrLabClinicalInfo) {
				return !EhrLabClinicalInfo.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EhrLabClinicalInfoCache _EhrLabClinicalInfoCache=new EhrLabClinicalInfoCache();

		///<summary>A list of all EhrLabClinicalInfos. Returns a deep copy.</summary>
		public static List<EhrLabClinicalInfo> ListDeep {
			get {
				return _EhrLabClinicalInfoCache.ListDeep;
			}
		}

		///<summary>A list of all visible EhrLabClinicalInfos. Returns a deep copy.</summary>
		public static List<EhrLabClinicalInfo> ListShortDeep {
			get {
				return _EhrLabClinicalInfoCache.ListShortDeep;
			}
		}

		///<summary>A list of all EhrLabClinicalInfos. Returns a shallow copy.</summary>
		public static List<EhrLabClinicalInfo> ListShallow {
			get {
				return _EhrLabClinicalInfoCache.ListShallow;
			}
		}

		///<summary>A list of all visible EhrLabClinicalInfos. Returns a shallow copy.</summary>
		public static List<EhrLabClinicalInfo> ListShort {
			get {
				return _EhrLabClinicalInfoCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EhrLabClinicalInfoCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EhrLabClinicalInfoCache.FillCacheFromTable(table);
				return table;
			}
			return _EhrLabClinicalInfoCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrLabClinicalInfo> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabClinicalInfo>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabclinicalinfo WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrLabClinicalInfoCrud.SelectMany(command);
		}

		///<summary>Gets one EhrLabClinicalInfo from the db.</summary>
		public static EhrLabClinicalInfo GetOne(long ehrLabClinicalInfoNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrLabClinicalInfo>(MethodBase.GetCurrentMethod(),ehrLabClinicalInfoNum);
			}
			return Crud.EhrLabClinicalInfoCrud.SelectOne(ehrLabClinicalInfoNum);
		}

		///<summary></summary>
		public static void Update(EhrLabClinicalInfo ehrLabClinicalInfo){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabClinicalInfo);
				return;
			}
			Crud.EhrLabClinicalInfoCrud.Update(ehrLabClinicalInfo);
		}

		///<summary></summary>
		public static void Delete(long ehrLabClinicalInfoNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabClinicalInfoNum);
				return;
			}
			string command= "DELETE FROM ehrlabclinicalinfo WHERE EhrLabClinicalInfoNum = "+POut.Long(ehrLabClinicalInfoNum);
			Db.NonQ(command);
		}
		*/
	}
}