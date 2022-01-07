using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class RequiredFieldConditions {
		#region CachePattern

		private class RequiredFieldConditionCache : CacheListAbs<RequiredFieldCondition> {
			protected override List<RequiredFieldCondition> GetCacheFromDb() {
				string command="SELECT * FROM requiredfieldcondition ORDER BY ConditionType,RequiredFieldConditionNum";
				return Crud.RequiredFieldConditionCrud.SelectMany(command);
			}
			protected override List<RequiredFieldCondition> TableToList(DataTable table) {
				return Crud.RequiredFieldConditionCrud.TableToList(table);
			}
			protected override RequiredFieldCondition Copy(RequiredFieldCondition requiredFieldCondition) {
				return requiredFieldCondition.Clone();
			}
			protected override DataTable ListToTable(List<RequiredFieldCondition> listRequiredFieldConditions) {
				return Crud.RequiredFieldConditionCrud.ListToTable(listRequiredFieldConditions,"RequiredFieldCondition");
			}
			protected override void FillCacheIfNeeded() {
				RequiredFieldConditions.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static RequiredFieldConditionCache _requiredFieldConditionCache=new RequiredFieldConditionCache();

		public static List<RequiredFieldCondition> GetWhere(Predicate<RequiredFieldCondition> match,bool isShort=false) {
			return _requiredFieldConditionCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_requiredFieldConditionCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_requiredFieldConditionCache.FillCacheFromTable(table);
				return table;
			}
			return _requiredFieldConditionCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Gets the requiredfieldconditions for one required field.</summary>
		public static List<RequiredFieldCondition> GetForRequiredField(long requiredFieldNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.RequiredFieldNum==requiredFieldNum);
		}

		///<summary></summary>
		public static long Insert(RequiredFieldCondition requiredFieldCondition){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				requiredFieldCondition.RequiredFieldConditionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),requiredFieldCondition);
				return requiredFieldCondition.RequiredFieldConditionNum;
			}
			return Crud.RequiredFieldConditionCrud.Insert(requiredFieldCondition);
		}

		///<summary></summary>
		public static void Update(RequiredFieldCondition requiredFieldCondition){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),requiredFieldCondition);
				return;
			}
			Crud.RequiredFieldConditionCrud.Update(requiredFieldCondition);
		}

		public static void DeleteAll(List<long> listRequiredFieldCondNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listRequiredFieldCondNums);
				return;
			}
			if(listRequiredFieldCondNums.Count<1) {
				return;
			}
			string command="DELETE FROM requiredfieldcondition WHERE RequiredFieldConditionNum IN("+string.Join(",",listRequiredFieldCondNums)+")";
			Db.NonQ(command);
		}

		/*
		///<summary></summary>
		public static void Delete(long requiredFieldConditionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),requiredFieldConditionNum);
				return;
			}
			Crud.RequiredFieldConditionCrud.Delete(requiredFieldConditionNum);
		}

		///<summary></summary>
		public static List<RequiredFieldCondition> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<RequiredFieldCondition>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM requiredfieldcondition WHERE PatNum = "+POut.Long(patNum);
			return Crud.RequiredFieldConditionCrud.SelectMany(command);
		}

		///<summary>Gets one RequiredFieldCondition from the db.</summary>
		public static RequiredFieldCondition GetOne(long requiredFieldConditionNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<RequiredFieldCondition>(MethodBase.GetCurrentMethod(),requiredFieldConditionNum);
			}
			return Crud.RequiredFieldConditionCrud.SelectOne(requiredFieldConditionNum);
		}
		*/
	}
}