using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProviderErxs{
		#region CachePattern

		private class ProviderErxCache : CacheListAbs<ProviderErx> {
			protected override List<ProviderErx> GetCacheFromDb() {
				string command="SELECT * FROM providererx ORDER BY NationalProviderID";
				return Crud.ProviderErxCrud.SelectMany(command);
			}
			protected override List<ProviderErx> TableToList(DataTable table) {
				return Crud.ProviderErxCrud.TableToList(table);
			}
			protected override ProviderErx Copy(ProviderErx providerErx) {
				return providerErx.Clone();
			}
			protected override DataTable ListToTable(List<ProviderErx> listProviderErxs) {
				return Crud.ProviderErxCrud.ListToTable(listProviderErxs,"ProviderErx");
			}
			protected override void FillCacheIfNeeded() {
				ProviderErxs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProviderErxCache _providerErxCache=new ProviderErxCache();

		public static List<ProviderErx> GetDeepCopy(bool isShort=false) {
			return _providerErxCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _providerErxCache.GetCount(isShort);
		}

		public static int GetFindIndex(Predicate<ProviderErx> match,bool isShort=false) {
			return _providerErxCache.GetFindIndex(match,isShort);
		}

		public static ProviderErx GetFirstOrDefault(Func<ProviderErx,bool> match,bool isShort=false) {
			return _providerErxCache.GetFirstOrDefault(match,isShort);
		}

		public static List<ProviderErx> GetWhere(Predicate<ProviderErx> match,bool isShort=false) {
			return _providerErxCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_providerErxCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_providerErxCache.FillCacheFromTable(table);
				return table;
			}
			return _providerErxCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Delete(long providerErxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),providerErxNum);
				return;
			}
			Crud.ProviderErxCrud.Delete(providerErxNum);
		}

		///<summary>Gets from db.  Used from FormErxAccess at HQ only.</summary>
		public static List<ProviderErx> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ProviderErx>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM providererx WHERE PatNum = "+POut.Long(patNum)+" ORDER BY NationalProviderID";
			return Crud.ProviderErxCrud.SelectMany(command);
		}

		///<summary></summary>
		public static ProviderErx GetOne(long provErxNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ProviderErx>(MethodBase.GetCurrentMethod(),provErxNum);
			}
			string command="SELECT * FROM providererx WHERE ProviderErxNum = "+POut.Long(provErxNum);
			return Crud.ProviderErxCrud.SelectOne(command);
		}

		///<summary>Gets one ProviderErx from the cache.</summary>
		public static ProviderErx GetOneForNpiAndOption(string npi,ErxOption erxOption) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.NationalProviderID==npi && x.ErxType==erxOption);
		}

		///<summary>Gets all ProviderErx which have not yet been sent to HQ.</summary>
		public static List<ProviderErx> GetAllUnsent() {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => !x.IsSentToHq);
		}

		///<summary>This should only be used for ODHQ.  Gets all account ids associated to the patient account.</summary>
		public static List<string> GetAccountIdsForPatNum(long patNum) {
			return GetDeepCopy().FindAll(x => x.PatNum==patNum && !string.IsNullOrWhiteSpace(x.AccountId))
				.Select(x => x.AccountId)
				.ToList();
		}

		///<summary></summary>
		public static long Insert(ProviderErx providerErx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				providerErx.ProviderErxNum=Meth.GetLong(MethodBase.GetCurrentMethod(),providerErx);
				return providerErx.ProviderErxNum;
			}
			return Crud.ProviderErxCrud.Insert(providerErx);
		}

		///<summary></summary>
		public static void Update(ProviderErx providerErx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),providerErx);
				return;
			}
			Crud.ProviderErxCrud.Update(providerErx);
		}

		///<summary></summary>
		public static bool Update(ProviderErx providerErx,ProviderErx oldProviderErx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),providerErx,oldProviderErx);
			}
			return Crud.ProviderErxCrud.Update(providerErx,oldProviderErx);
		}

		///<summary>Inserts, updates, or deletes the passed in list verses the old list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<ProviderErx> listNew,List<ProviderErx> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
					return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.ProviderErxCrud.Sync(listNew,listOld);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		 
		///<summary></summary>
		public static void Update(ProviderErx providerErx) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),providerErx);
				return;
			}
			Crud.ProviderErxCrud.Update(providerErx);
		}

		///<summary>Gets one ProviderErx from the db.</summary>
		public static ProviderErx GetOne(long providerErxNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ProviderErx>(MethodBase.GetCurrentMethod(),providerErxNum);
			}
			return Crud.ProviderErxCrud.SelectOne(providerErxNum);
		}
		*/
	}
}