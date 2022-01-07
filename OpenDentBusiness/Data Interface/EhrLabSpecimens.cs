using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrLabSpecimens {
		///<summary></summary>
		public static List<EhrLabSpecimen> GetForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabSpecimen>>(MethodBase.GetCurrentMethod(),ehrLabNum);
			}
			string command="SELECT * FROM ehrlabspecimen WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			return Crud.EhrLabSpecimenCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void DeleteForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNum);
				return;
			}
			EhrLabSpecimenConditions.DeleteForLab(ehrLabNum);
			EhrLabSpecimenRejectReasons.DeleteForLab(ehrLabNum);
			string command="DELETE FROM ehrlabspecimen WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static EhrLabSpecimen InsertItem(EhrLabSpecimen ehrLabSpecimen) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<EhrLabSpecimen>(MethodBase.GetCurrentMethod(),ehrLabSpecimen);
			}
			ehrLabSpecimen.EhrLabNum=Crud.EhrLabSpecimenCrud.Insert(ehrLabSpecimen);
			for(int i=0;i<ehrLabSpecimen.ListEhrLabSpecimenCondition.Count;i++) {
				ehrLabSpecimen.ListEhrLabSpecimenCondition[i].EhrLabSpecimenNum=ehrLabSpecimen.EhrLabSpecimenNum;
				EhrLabSpecimenConditions.Insert(ehrLabSpecimen.ListEhrLabSpecimenCondition[i]);
			}
			for(int i=0;i<ehrLabSpecimen.ListEhrLabSpecimenRejectReason.Count;i++) {
				ehrLabSpecimen.ListEhrLabSpecimenRejectReason[i].EhrLabSpecimenNum=ehrLabSpecimen.EhrLabSpecimenNum;
				EhrLabSpecimenRejectReasons.Insert(ehrLabSpecimen.ListEhrLabSpecimenRejectReason[i]);
			}
			return ehrLabSpecimen;
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class EhrLabSpecimenCache : CacheListAbs<EhrLabSpecimen> {
			protected override List<EhrLabSpecimen> GetCacheFromDb() {
				string command="SELECT * FROM EhrLabSpecimen ORDER BY ItemOrder";
				return Crud.EhrLabSpecimenCrud.SelectMany(command);
			}
			protected override List<EhrLabSpecimen> TableToList(DataTable table) {
				return Crud.EhrLabSpecimenCrud.TableToList(table);
			}
			protected override EhrLabSpecimen Copy(EhrLabSpecimen EhrLabSpecimen) {
				return EhrLabSpecimen.Clone();
			}
			protected override DataTable ListToTable(List<EhrLabSpecimen> listEhrLabSpecimens) {
				return Crud.EhrLabSpecimenCrud.ListToTable(listEhrLabSpecimens,"EhrLabSpecimen");
			}
			protected override void FillCacheIfNeeded() {
				EhrLabSpecimens.GetTableFromCache(false);
			}
			protected override bool IsInListShort(EhrLabSpecimen EhrLabSpecimen) {
				return !EhrLabSpecimen.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EhrLabSpecimenCache _EhrLabSpecimenCache=new EhrLabSpecimenCache();

		///<summary>A list of all EhrLabSpecimens. Returns a deep copy.</summary>
		public static List<EhrLabSpecimen> ListDeep {
			get {
				return _EhrLabSpecimenCache.ListDeep;
			}
		}

		///<summary>A list of all visible EhrLabSpecimens. Returns a deep copy.</summary>
		public static List<EhrLabSpecimen> ListShortDeep {
			get {
				return _EhrLabSpecimenCache.ListShortDeep;
			}
		}

		///<summary>A list of all EhrLabSpecimens. Returns a shallow copy.</summary>
		public static List<EhrLabSpecimen> ListShallow {
			get {
				return _EhrLabSpecimenCache.ListShallow;
			}
		}

		///<summary>A list of all visible EhrLabSpecimens. Returns a shallow copy.</summary>
		public static List<EhrLabSpecimen> ListShort {
			get {
				return _EhrLabSpecimenCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EhrLabSpecimenCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EhrLabSpecimenCache.FillCacheFromTable(table);
				return table;
			}
			return _EhrLabSpecimenCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrLabSpecimen> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabSpecimen>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabspecimen WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrLabSpecimenCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EhrLabSpecimen ehrLabSpecimen) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrLabSpecimen.EhrLabSpecimenNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabSpecimen);
				return ehrLabSpecimen.EhrLabSpecimenNum;
			}
			return Crud.EhrLabSpecimenCrud.Insert(ehrLabSpecimen);
		}

		///<summary>Gets one EhrLabSpecimen from the db.</summary>
		public static EhrLabSpecimen GetOne(long ehrLabSpecimenNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrLabSpecimen>(MethodBase.GetCurrentMethod(),ehrLabSpecimenNum);
			}
			return Crud.EhrLabSpecimenCrud.SelectOne(ehrLabSpecimenNum);
		}

		///<summary></summary>
		public static long Insert(EhrLabSpecimen ehrLabSpecimen){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ehrLabSpecimen.EhrLabSpecimenNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabSpecimen);
				return ehrLabSpecimen.EhrLabSpecimenNum;
			}
			return Crud.EhrLabSpecimenCrud.Insert(ehrLabSpecimen);
		}

		///<summary></summary>
		public static void Update(EhrLabSpecimen ehrLabSpecimen){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabSpecimen);
				return;
			}
			Crud.EhrLabSpecimenCrud.Update(ehrLabSpecimen);
		}

		///<summary></summary>
		public static void Delete(long ehrLabSpecimenNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabSpecimenNum);
				return;
			}
			string command= "DELETE FROM ehrlabspecimen WHERE EhrLabSpecimenNum = "+POut.Long(ehrLabSpecimenNum);
			Db.NonQ(command);
		}
		*/
	}
}