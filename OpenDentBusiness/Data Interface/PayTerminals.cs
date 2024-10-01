using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PayTerminals{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class PayTerminalCache : CacheListAbs<PayTerminal> {
			protected override List<PayTerminal> GetCacheFromDb() {
				string command="SELECT * FROM payterminal";
				return Crud.PayTerminalCrud.SelectMany(command);
			}
			protected override List<PayTerminal> TableToList(DataTable table) {
				return Crud.PayTerminalCrud.TableToList(table);
			}
			protected override PayTerminal Copy(PayTerminal payTerminal) {
				return payTerminal.Copy();
			}
			protected override DataTable ListToTable(List<PayTerminal> listPayTerminals) {
				return Crud.PayTerminalCrud.ListToTable(listPayTerminals,"PayTerminal");
			}
			protected override void FillCacheIfNeeded() {
				PayTerminals.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PayTerminalCache _payTerminalCache=new PayTerminalCache();

		public static void ClearCache() {
			_payTerminalCache.ClearCache();
		}

		public static List<PayTerminal> GetDeepCopy(bool isShort=false) {
			return _payTerminalCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _payTerminalCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<PayTerminal> match,bool isShort=false) {
			return _payTerminalCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<PayTerminal> match,bool isShort=false) {
			return _payTerminalCache.GetFindIndex(match,isShort);
		}

		public static PayTerminal GetFirst(bool isShort=false) {
			return _payTerminalCache.GetFirst(isShort);
		}

		public static PayTerminal GetFirst(Func<PayTerminal,bool> match,bool isShort=false) {
			return _payTerminalCache.GetFirst(match,isShort);
		}

		public static PayTerminal GetFirstOrDefault(Func<PayTerminal,bool> match,bool isShort=false) {
			return _payTerminalCache.GetFirstOrDefault(match,isShort);
		}

		public static PayTerminal GetLast(bool isShort=false) {
			return _payTerminalCache.GetLast(isShort);
		}

		public static PayTerminal GetLastOrDefault(Func<PayTerminal,bool> match,bool isShort=false) {
			return _payTerminalCache.GetLastOrDefault(match,isShort);
		}

		public static List<PayTerminal> GetWhere(Predicate<PayTerminal> match,bool isShort=false) {
			return _payTerminalCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_payTerminalCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_payTerminalCache.FillCacheFromTable(table);
				return table;
			}
			return _payTerminalCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		#region Methods - Get

		///<summary></summary>
		public static List<PayTerminal> Refresh(long clinicNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PayTerminal>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM payterminal WHERE ClinicNum = "+POut.Long(clinicNum);
			return Crud.PayTerminalCrud.SelectMany(command);
		}
		
		///<summary>Gets one PayTerminal from the db.</summary>
		public static PayTerminal GetOne(long payTerminalNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<PayTerminal>(MethodBase.GetCurrentMethod(),payTerminalNum);
			}
			return Crud.PayTerminalCrud.SelectOne(payTerminalNum);
		}
		#endregion Methods - Get
		#region Methods - Modify

		///<summary></summary>
		public static long Insert(PayTerminal payTerminal){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				payTerminal.PayTerminalNum=Meth.GetLong(MethodBase.GetCurrentMethod(),payTerminal);
				return payTerminal.PayTerminalNum;
			}
			return Crud.PayTerminalCrud.Insert(payTerminal);
		}

		///<summary></summary>
		public static void Update(PayTerminal payTerminal){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payTerminal);
				return;
			}
			Crud.PayTerminalCrud.Update(payTerminal);
		}

		///<summary></summary>
		public static void Delete(long payTerminalNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),payTerminalNum);
				return;
			}
			Crud.PayTerminalCrud.Delete(payTerminalNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		#endregion Methods - Misc
	}
}