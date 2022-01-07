using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsFilingCodes{
		#region CachePattern

		private class InsFilingCodeCache : CacheListAbs<InsFilingCode> {
			protected override List<InsFilingCode> GetCacheFromDb() {
				string command="SELECT * FROM insfilingcode ORDER BY ItemOrder";
				return Crud.InsFilingCodeCrud.SelectMany(command);
			}
			protected override List<InsFilingCode> TableToList(DataTable table) {
				return Crud.InsFilingCodeCrud.TableToList(table);
			}
			protected override InsFilingCode Copy(InsFilingCode iInsFilingCode) {
				return iInsFilingCode.Clone();
			}
			protected override DataTable ListToTable(List<InsFilingCode> listInsFilingCodes) {
				return Crud.InsFilingCodeCrud.ListToTable(listInsFilingCodes,"InsFilingCode");
			}
			protected override void FillCacheIfNeeded() {
				InsFilingCodes.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static InsFilingCodeCache _insFilingCodeCache=new InsFilingCodeCache();

		public static List<InsFilingCode> GetDeepCopy(bool isShort=false) {
			return _insFilingCodeCache.GetDeepCopy(isShort);
		}

		public static InsFilingCode GetFirstOrDefault(Func<InsFilingCode,bool> match,bool isShort=false) {
			return _insFilingCodeCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_insFilingCodeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_insFilingCodeCache.FillCacheFromTable(table);
				return table;
			}
			return _insFilingCodeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		public static string GetEclaimCode(long insFilingCodeNum) {
			//No need to check RemotingRole; no call to db.
			InsFilingCode insFilingCode=GetFirstOrDefault(x => x.InsFilingCodeNum==insFilingCodeNum);
			return (insFilingCode==null ? "CI" : insFilingCode.EclaimCode);
		}

		///<summary>Gets the InsFilingCode for the specified eclaimCode, or creates one if the eclaimCodes does not exist.</summary>
		public static InsFilingCode GetOrInsertForEclaimCode(string descript,string eclaimCode) {
			//No need to check RemotingRole; no call to db.
			int itemOrderMax=0;
			List<InsFilingCode> listInsFilingCodes=GetDeepCopy();
			for(int i=0;i<listInsFilingCodes.Count;i++) {
				if(listInsFilingCodes[i].ItemOrder > itemOrderMax) {
					itemOrderMax=listInsFilingCodes[i].ItemOrder;
				}
				if(listInsFilingCodes[i].EclaimCode != eclaimCode) {
					continue;
				}
				return listInsFilingCodes[i];
			}
			InsFilingCode insFilingCode=new InsFilingCode();
			insFilingCode.Descript=descript;
			insFilingCode.EclaimCode=eclaimCode;
			insFilingCode.ItemOrder=(itemOrderMax+1);
			Insert(insFilingCode);
			return insFilingCode;
		}

		public static List<InsFilingCode> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsFilingCode>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM insfilingcode ORDER BY ItemOrder";
			return Crud.InsFilingCodeCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(InsFilingCode insFilingCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				insFilingCode.InsFilingCodeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),insFilingCode);
				return insFilingCode.InsFilingCodeNum;
			}
			return Crud.InsFilingCodeCrud.Insert(insFilingCode);
		}

		///<summary></summary>
		public static void Update(InsFilingCode insFilingCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insFilingCode);
				return;
			}
			Crud.InsFilingCodeCrud.Update(insFilingCode);
		}

		///<summary>Surround with try/catch</summary>
		public static void Delete(long insFilingCodeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insFilingCodeNum);
				return;
			}
			string command="SELECT COUNT(*) FROM insplan WHERE FilingCode="+POut.Long(insFilingCodeNum);
			if(Db.GetScalar(command) != "0") {
				throw new ApplicationException(Lans.g("InsFilingCode","Already in use by insplans."));
			}
			Crud.InsFilingCodeCrud.Delete(insFilingCodeNum);
		}
	}
}