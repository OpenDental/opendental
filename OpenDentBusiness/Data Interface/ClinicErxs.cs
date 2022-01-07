using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClinicErxs{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ClinicErxCache : CacheListAbs<ClinicErx> {
			protected override List<ClinicErx> GetCacheFromDb() {
				string command="SELECT * FROM clinicerx";
				return Crud.ClinicErxCrud.SelectMany(command);
			}
			protected override List<ClinicErx> TableToList(DataTable table) {
				return Crud.ClinicErxCrud.TableToList(table);
			}
			protected override ClinicErx Copy(ClinicErx clinicErx) {
				return clinicErx.Copy();
			}
			protected override DataTable ListToTable(List<ClinicErx> listClinicErxs) {
				return Crud.ClinicErxCrud.ListToTable(listClinicErxs,"ClinicErx");
			}
			protected override void FillCacheIfNeeded() {
				ClinicErxs.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ClinicErxCache _clinicErxCache=new ClinicErxCache();

		public static List<ClinicErx> GetDeepCopy(bool isShort=false) {
			return _clinicErxCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _clinicErxCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ClinicErx> match,bool isShort=false) {
			return _clinicErxCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ClinicErx> match,bool isShort=false) {
			return _clinicErxCache.GetFindIndex(match,isShort);
		}

		public static ClinicErx GetFirst(bool isShort=false) {
			return _clinicErxCache.GetFirst(isShort);
		}

		public static ClinicErx GetFirst(Func<ClinicErx,bool> match,bool isShort=false) {
			return _clinicErxCache.GetFirst(match,isShort);
		}

		public static ClinicErx GetFirstOrDefault(Func<ClinicErx,bool> match,bool isShort=false) {
			return _clinicErxCache.GetFirstOrDefault(match,isShort);
		}

		public static ClinicErx GetLast(bool isShort=false) {
			return _clinicErxCache.GetLast(isShort);
		}

		public static ClinicErx GetLastOrDefault(Func<ClinicErx,bool> match,bool isShort=false) {
			return _clinicErxCache.GetLastOrDefault(match,isShort);
		}

		public static List<ClinicErx> GetWhere(Predicate<ClinicErx> match,bool isShort=false) {
			return _clinicErxCache.GetWhere(match,isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_clinicErxCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_clinicErxCache.FillCacheFromTable(table);
				return table;
			}
			return _clinicErxCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Get Methods

		///<summary></summary>
		public static ClinicErx GetOne(long clinicErxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ClinicErx>(MethodBase.GetCurrentMethod(),clinicErxNum);
			}
			string command="SELECT * FROM clinicerx WHERE ClinicErxNum = "+POut.Long(clinicErxNum);
			return Crud.ClinicErxCrud.SelectOne(command);
		}

		///<summary></summary>
		public static List<ClinicErx> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClinicErx>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM clinicerx WHERE PatNum = "+POut.Long(patNum);
			return Crud.ClinicErxCrud.SelectMany(command);
		}

		///<summary>Gets one ClinicErx from the cache.</summary>
		public static ClinicErx GetByClinicNum(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ClinicNum==clinicNum);
		}

		///<summary>Gets one ClinicErx from the cache.</summary>
		public static ClinicErx GetByClinicIdAndKey(string clinicId,string clinicKey) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ClinicId==clinicId && x.ClinicKey==clinicKey);
		}

		///<summary>This should only be used for ODHQ.  Gets all account ids associated to the patient account.</summary>
		public static List<string> GetAccountIdsForPatNum(long patNum) {
			return GetDeepCopy().FindAll(x => x.PatNum==patNum && !string.IsNullOrWhiteSpace(x.AccountId))
				.Select(x => x.AccountId)
				.ToList();
		}

		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(ClinicErx clinicErx){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				clinicErx.ClinicErxNum=Meth.GetLong(MethodBase.GetCurrentMethod(),clinicErx);
				return clinicErx.ClinicErxNum;
			}
			return Crud.ClinicErxCrud.Insert(clinicErx);
		}
			#endregion

		#region Update
		///<summary></summary>
		public static void Update(ClinicErx clinicErx){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicErx);
				return;
			}
			Crud.ClinicErxCrud.Update(clinicErx);
		}

		///<summary></summary>
		public static bool Update(ClinicErx clinicErx,ClinicErx oldClinicErx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),clinicErx,oldClinicErx);
			}
			return Crud.ClinicErxCrud.Update(clinicErx,oldClinicErx);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long clinicErxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicErxNum);
				return;
			}
			Crud.ClinicErxCrud.Delete(clinicErxNum);
		}
			#endregion
	}
}