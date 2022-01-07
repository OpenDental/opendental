using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class InsFilingCodeSubtypes {
		#region CachePattern

		private class InsFilingCodeSubtypeCache : CacheListAbs<InsFilingCodeSubtype> {
			protected override List<InsFilingCodeSubtype> GetCacheFromDb() {
				string command="SELECT * FROM insfilingcodesubtype ORDER BY Descript";
				return Crud.InsFilingCodeSubtypeCrud.SelectMany(command);
			}
			protected override List<InsFilingCodeSubtype> TableToList(DataTable table) {
				return Crud.InsFilingCodeSubtypeCrud.TableToList(table);
			}
			protected override InsFilingCodeSubtype Copy(InsFilingCodeSubtype InsFilingCodeSubtype) {
				return InsFilingCodeSubtype.Clone();
			}
			protected override DataTable ListToTable(List<InsFilingCodeSubtype> listInsFilingCodeSubtypes) {
				return Crud.InsFilingCodeSubtypeCrud.ListToTable(listInsFilingCodeSubtypes,"InsFilingCodeSubtype");
			}
			protected override void FillCacheIfNeeded() {
				InsFilingCodeSubtypes.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static InsFilingCodeSubtypeCache _InsFilingCodeSubtypeCache=new InsFilingCodeSubtypeCache();

		public static List<InsFilingCodeSubtype> GetWhere(Predicate<InsFilingCodeSubtype> match,bool isShort=false) {
			return _InsFilingCodeSubtypeCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_InsFilingCodeSubtypeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_InsFilingCodeSubtypeCache.FillCacheFromTable(table);
				return table;
			}
			return _InsFilingCodeSubtypeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<Summary>Gets one InsFilingCodeSubtype from the database.</Summary>
		public static InsFilingCodeSubtype GetOne(long insFilingCodeSubtypeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsFilingCodeSubtype>(MethodBase.GetCurrentMethod(),insFilingCodeSubtypeNum);
			}
			return Crud.InsFilingCodeSubtypeCrud.SelectOne(insFilingCodeSubtypeNum);
		}

		///<summary></summary>
		public static long Insert(InsFilingCodeSubtype insFilingCodeSubtype) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				insFilingCodeSubtype.InsFilingCodeSubtypeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),insFilingCodeSubtype);
				return insFilingCodeSubtype.InsFilingCodeSubtypeNum;
			}
			return Crud.InsFilingCodeSubtypeCrud.Insert(insFilingCodeSubtype);
		}

		///<summary></summary>
		public static void Update(InsFilingCodeSubtype insFilingCodeSubtype) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insFilingCodeSubtype);
				return;
			}
			Crud.InsFilingCodeSubtypeCrud.Update(insFilingCodeSubtype);
		}

		///<summary>Surround with try/catch</summary>
		public static void Delete(long insFilingCodeSubtypeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insFilingCodeSubtypeNum);
				return;
			}
			string command="SELECT COUNT(*) FROM insplan WHERE FilingCodeSubtype="+POut.Long(insFilingCodeSubtypeNum);
			if(Db.GetScalar(command) != "0") {
				throw new ApplicationException(Lans.g("InsFilingCodeSubtype","Already in use by insplans."));
			}
			Crud.InsFilingCodeSubtypeCrud.Delete(insFilingCodeSubtypeNum);
		}

		public static List<InsFilingCodeSubtype> GetForInsFilingCode(long insFilingCodeNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.InsFilingCodeNum==insFilingCodeNum);
		}

		public static void DeleteForInsFilingCode(long insFilingCodeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insFilingCodeNum);
				return;
			}
			string command="DELETE FROM insfilingcodesubtype "+
				"WHERE InsFilingCodeNum="+POut.Long(insFilingCodeNum);
			Db.NonQ(command);
		}
	}
}