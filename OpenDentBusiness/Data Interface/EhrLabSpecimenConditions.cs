using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrLabSpecimenConditions {
		///<summary></summary>
		public static List<EhrLabSpecimenCondition> GetForEhrLabSpecimen(long ehrLabSpecimenNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabSpecimenCondition>>(MethodBase.GetCurrentMethod(),ehrLabSpecimenNum);
			}
			string command="SELECT * FROM ehrlabspecimencondition WHERE EhrLabSpecimenNum="+POut.Long(ehrLabSpecimenNum);
			return Crud.EhrLabSpecimenConditionCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void DeleteForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNum);
				return;
			}
			string command="DELETE FROM ehrlabspecimencondition WHERE EhrLabSpecimenNum IN (SELECT EhrLabSpecimenNum FROM ehrlabspecimen WHERE EhrLabNum="+POut.Long(ehrLabNum)+")";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForLabSpecimen(long ehrLabSpecimenNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabSpecimenNum);
				return;
			}
			string command="DELETE FROM ehrlabspecimencondition WHERE EhrLabSpecimenNum="+POut.Long(ehrLabSpecimenNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(EhrLabSpecimenCondition ehrLabSpecimenCondition) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrLabSpecimenCondition.EhrLabSpecimenConditionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabSpecimenCondition);
				return ehrLabSpecimenCondition.EhrLabSpecimenConditionNum;
			}
			return Crud.EhrLabSpecimenConditionCrud.Insert(ehrLabSpecimenCondition);
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class EhrLabSpecimenConditionCache : CacheListAbs<EhrLabSpecimenCondition> {
			protected override List<EhrLabSpecimenCondition> GetCacheFromDb() {
				string command="SELECT * FROM EhrLabSpecimenCondition ORDER BY ItemOrder";
				return Crud.EhrLabSpecimenConditionCrud.SelectMany(command);
			}
			protected override List<EhrLabSpecimenCondition> TableToList(DataTable table) {
				return Crud.EhrLabSpecimenConditionCrud.TableToList(table);
			}
			protected override EhrLabSpecimenCondition Copy(EhrLabSpecimenCondition EhrLabSpecimenCondition) {
				return EhrLabSpecimenCondition.Clone();
			}
			protected override DataTable ListToTable(List<EhrLabSpecimenCondition> listEhrLabSpecimenConditions) {
				return Crud.EhrLabSpecimenConditionCrud.ListToTable(listEhrLabSpecimenConditions,"EhrLabSpecimenCondition");
			}
			protected override void FillCacheIfNeeded() {
				EhrLabSpecimenConditions.GetTableFromCache(false);
			}
			protected override bool IsInListShort(EhrLabSpecimenCondition EhrLabSpecimenCondition) {
				return !EhrLabSpecimenCondition.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EhrLabSpecimenConditionCache _EhrLabSpecimenConditionCache=new EhrLabSpecimenConditionCache();

		///<summary>A list of all EhrLabSpecimenConditions. Returns a deep copy.</summary>
		public static List<EhrLabSpecimenCondition> ListDeep {
			get {
				return _EhrLabSpecimenConditionCache.ListDeep;
			}
		}

		///<summary>A list of all visible EhrLabSpecimenConditions. Returns a deep copy.</summary>
		public static List<EhrLabSpecimenCondition> ListShortDeep {
			get {
				return _EhrLabSpecimenConditionCache.ListShortDeep;
			}
		}

		///<summary>A list of all EhrLabSpecimenConditions. Returns a shallow copy.</summary>
		public static List<EhrLabSpecimenCondition> ListShallow {
			get {
				return _EhrLabSpecimenConditionCache.ListShallow;
			}
		}

		///<summary>A list of all visible EhrLabSpecimenConditions. Returns a shallow copy.</summary>
		public static List<EhrLabSpecimenCondition> ListShort {
			get {
				return _EhrLabSpecimenConditionCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EhrLabSpecimenConditionCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EhrLabSpecimenConditionCache.FillCacheFromTable(table);
				return table;
			}
			return _EhrLabSpecimenConditionCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrLabSpecimenCondition> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabSpecimenCondition>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabspecimencondition WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrLabSpecimenConditionCrud.SelectMany(command);
		}

		///<summary>Gets one EhrLabSpecimenCondition from the db.</summary>
		public static EhrLabSpecimenCondition GetOne(long ehrLabSpecimenConditionNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrLabSpecimenCondition>(MethodBase.GetCurrentMethod(),ehrLabSpecimenConditionNum);
			}
			return Crud.EhrLabSpecimenConditionCrud.SelectOne(ehrLabSpecimenConditionNum);
		}

		///<summary></summary>
		public static long Insert(EhrLabSpecimenCondition ehrLabSpecimenCondition){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ehrLabSpecimenCondition.EhrLabSpecimenConditionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabSpecimenCondition);
				return ehrLabSpecimenCondition.EhrLabSpecimenConditionNum;
			}
			return Crud.EhrLabSpecimenConditionCrud.Insert(ehrLabSpecimenCondition);
		}

		///<summary></summary>
		public static void Update(EhrLabSpecimenCondition ehrLabSpecimenCondition){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabSpecimenCondition);
				return;
			}
			Crud.EhrLabSpecimenConditionCrud.Update(ehrLabSpecimenCondition);
		}

		///<summary></summary>
		public static void Delete(long ehrLabSpecimenConditionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabSpecimenConditionNum);
				return;
			}
			string command= "DELETE FROM ehrlabspecimencondition WHERE EhrLabSpecimenConditionNum = "+POut.Long(ehrLabSpecimenConditionNum);
			Db.NonQ(command);
		}
		*/
	}
}