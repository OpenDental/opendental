using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EFormDefs{
		#region Cache Pattern
		//Uses InvalidType.Sheets
		private class EFormDefCache : CacheListAbs<EFormDef> {
			protected override List<EFormDef> GetCacheFromDb() {
				string command="SELECT * FROM eformdef";
				return Crud.EFormDefCrud.SelectMany(command);
			}
			protected override List<EFormDef> TableToList(DataTable table) {
				return Crud.EFormDefCrud.TableToList(table);
			}
			protected override EFormDef Copy(EFormDef eFormDef) {
				return eFormDef.Copy();
			}
			protected override DataTable ListToTable(List<EFormDef> listEFormDefs) {
				return Crud.EFormDefCrud.ListToTable(listEFormDefs,"EFormDef");
			}
			protected override void FillCacheIfNeeded() {
				EFormDefs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EFormDefCache _eFormDefCache=new EFormDefCache();

		public static void ClearCache() {
			_eFormDefCache.ClearCache();
		}

		public static List<EFormDef> GetDeepCopy(bool isShort=false) {
			return _eFormDefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _eFormDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<EFormDef> match,bool isShort=false) {
			return _eFormDefCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<EFormDef> match,bool isShort=false) {
			return _eFormDefCache.GetFindIndex(match,isShort);
		}

		public static EFormDef GetFirst(bool isShort=false) {
			return _eFormDefCache.GetFirst(isShort);
		}

		public static EFormDef GetFirst(Func<EFormDef,bool> match,bool isShort=false) {
			return _eFormDefCache.GetFirst(match,isShort);
		}

		public static EFormDef GetFirstOrDefault(Func<EFormDef,bool> match,bool isShort=false) {
			return _eFormDefCache.GetFirstOrDefault(match,isShort);
		}

		public static EFormDef GetLast(bool isShort=false) {
			return _eFormDefCache.GetLast(isShort);
		}

		public static EFormDef GetLastOrDefault(Func<EFormDef,bool> match,bool isShort=false) {
			return _eFormDefCache.GetLastOrDefault(match,isShort);
		}

		public static List<EFormDef> GetWhere(Predicate<EFormDef> match,bool isShort=false) {
			return _eFormDefCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_eFormDefCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_eFormDefCache.FillCacheFromTable(table);
				return table;
			}
			return _eFormDefCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(EFormDef eFormDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eFormDef.EFormDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eFormDef);
				return eFormDef.EFormDefNum;
			}
			return Crud.EFormDefCrud.Insert(eFormDef);
		}

		///<summary></summary>
		public static void Update(EFormDef eFormDef){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormDef);
				return;
			}
			Crud.EFormDefCrud.Update(eFormDef);
		}

		///<summary></summary>
		public static void Delete(long eFormDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eFormDefNum);
				return;
			}
			Crud.EFormDefCrud.Delete(eFormDefNum);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EFormDef> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EFormDef>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eformdef WHERE PatNum = "+POut.Long(patNum);
			return Crud.EFormDefCrud.SelectMany(command);
		}
		
		///<summary>Gets one EFormDef from the db.</summary>
		public static EFormDef GetOne(long eFormDefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EFormDef>(MethodBase.GetCurrentMethod(),eFormDefNum);
			}
			return Crud.EFormDefCrud.SelectOne(eFormDefNum);
		}

		

		*/



	}
}