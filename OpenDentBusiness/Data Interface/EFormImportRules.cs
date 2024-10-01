using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EFormImportRules{
		#region Cache Pattern
		private class EFormImportRuleCache : CacheListAbs<EFormImportRule> {
			protected override List<EFormImportRule> GetCacheFromDb() {
				string command="SELECT * FROM eformimportrule";
				return Crud.EFormImportRuleCrud.SelectMany(command);
			}
			protected override List<EFormImportRule> TableToList(DataTable table) {
				return Crud.EFormImportRuleCrud.TableToList(table);
			}
			protected override EFormImportRule Copy(EFormImportRule eFormImportRule) {
				return eFormImportRule.Copy();
			}
			protected override DataTable ListToTable(List<EFormImportRule> listEFormImportRules) {
				return Crud.EFormImportRuleCrud.ListToTable(listEFormImportRules,"EFormImportRule");
			}
			protected override void FillCacheIfNeeded() {
				EFormImportRules.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EFormImportRuleCache _eFormImportRuleCache=new EFormImportRuleCache();

		public static void ClearCache() {
			_eFormImportRuleCache.ClearCache();
		}

		public static List<EFormImportRule> GetDeepCopy(bool isShort=false) {
			return _eFormImportRuleCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _eFormImportRuleCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<EFormImportRule> match,bool isShort=false) {
			return _eFormImportRuleCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<EFormImportRule> match,bool isShort=false) {
			return _eFormImportRuleCache.GetFindIndex(match,isShort);
		}

		public static EFormImportRule GetFirst(bool isShort=false) {
			return _eFormImportRuleCache.GetFirst(isShort);
		}

		public static EFormImportRule GetFirst(Func<EFormImportRule,bool> match,bool isShort=false) {
			return _eFormImportRuleCache.GetFirst(match,isShort);
		}

		public static EFormImportRule GetFirstOrDefault(Func<EFormImportRule,bool> match,bool isShort=false) {
			return _eFormImportRuleCache.GetFirstOrDefault(match,isShort);
		}

		public static EFormImportRule GetLast(bool isShort=false) {
			return _eFormImportRuleCache.GetLast(isShort);
		}

		public static EFormImportRule GetLastOrDefault(Func<EFormImportRule,bool> match,bool isShort=false) {
			return _eFormImportRuleCache.GetLastOrDefault(match,isShort);
		}

		public static List<EFormImportRule> GetWhere(Predicate<EFormImportRule> match,bool isShort=false) {
			return _eFormImportRuleCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eFormImportRuleCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eFormImportRuleCache.FillCacheFromTable(table);
				return table;
			}
			return _eFormImportRuleCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		
		///<summary></summary>
		public static long Insert(EFormImportRule eFormImportRule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eFormImportRule.EFormImportRuleNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eFormImportRule);
				return eFormImportRule.EFormImportRuleNum;
			}
			return Crud.EFormImportRuleCrud.Insert(eFormImportRule);
		}
		///<summary></summary>
		public static void Update(EFormImportRule eFormImportRule){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormImportRule);
				return;
			}
			Crud.EFormImportRuleCrud.Update(eFormImportRule);
		}
		///<summary></summary>
		public static void Delete(long eFormImportRuleNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormImportRuleNum);
				return;
			}
			Crud.EFormImportRuleCrud.Delete(eFormImportRuleNum);
		}

		public static bool isAllowedSit(string fieldName,EnumEFormImportSituation enumEFormImportSituation){
			Meth.NoCheckMiddleTierRole();
			if(enumEFormImportSituation!=EnumEFormImportSituation.Invalid){
				return true;
			}
			if(fieldName.In("Address","Address2","City","State","Zip",
				"SSN","Birthdate","Email",
				"HmPhone","ICEPhone","ins1CarrierPhone","ins2CarrierPhone","WirelessPhone","WkPhone"))
			{
				return true;
			}
			return false;
		}

		public static bool isAllowedAction(string fieldName,EnumEFormImportAction enumEFormImportAction){
			Meth.NoCheckMiddleTierRole();
			if(enumEFormImportAction!=EnumEFormImportAction.Fix){
				return true;
			}
			if(fieldName.In("Address","Address2","City","State","Zip",
				"HmPhone","ICEPhone","ins1CarrierPhone","ins2CarrierPhone","WirelessPhone","WkPhone",
				"FName","MiddleI","LName"))
			{
				return true;
			}
			return false;
		}


		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<EFormImportRule> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EFormImportRule>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eformimportrule WHERE PatNum = "+POut.Long(patNum);
			return Crud.EFormImportRuleCrud.SelectMany(command);
		}
		
		///<summary>Gets one EFormImportRule from the db.</summary>
		public static EFormImportRule GetOne(long eFormImportRuleNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EFormImportRule>(MethodBase.GetCurrentMethod(),eFormImportRuleNum);
			}
			return Crud.EFormImportRuleCrud.SelectOne(eFormImportRuleNum);
		}
		#endregion Methods - Get
		
		*/



	}
}