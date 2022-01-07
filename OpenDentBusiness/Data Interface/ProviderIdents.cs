using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary>Refreshed with local data.</summary>
	public class ProviderIdents {
		#region CachePattern

		private class ProviderIdentCache : CacheListAbs<ProviderIdent> {
			protected override List<ProviderIdent> GetCacheFromDb() {
				string command="SELECT * FROM providerident";
				return Crud.ProviderIdentCrud.SelectMany(command);
			}
			protected override List<ProviderIdent> TableToList(DataTable table) {
				return Crud.ProviderIdentCrud.TableToList(table);
			}
			protected override ProviderIdent Copy(ProviderIdent ProviderIdent) {
				return ProviderIdent.Copy();
			}
			protected override DataTable ListToTable(List<ProviderIdent> listProviderIdents) {
				return Crud.ProviderIdentCrud.ListToTable(listProviderIdents,"ProviderIdent");
			}
			protected override void FillCacheIfNeeded() {
				ProviderIdents.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProviderIdentCache _ProviderIdentCache=new ProviderIdentCache();

		public static ProviderIdent GetFirstOrDefault(Func<ProviderIdent,bool> match,bool isShort=false) {
			return _ProviderIdentCache.GetFirstOrDefault(match,isShort);
		}

		public static List<ProviderIdent> GetWhere(Predicate<ProviderIdent> match,bool isShort=false) {
			return _ProviderIdentCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ProviderIdentCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ProviderIdentCache.FillCacheFromTable(table);
				return table;
			}
			return _ProviderIdentCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(ProviderIdent pi){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pi);
				return;
			}
			Crud.ProviderIdentCrud.Update(pi);
		}

		///<summary></summary>
		public static long Insert(ProviderIdent pi){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				pi.ProviderIdentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),pi);
				return pi.ProviderIdentNum;
			}
			return Crud.ProviderIdentCrud.Insert(pi);
		}

		///<summary></summary>
		public static void Delete(ProviderIdent pi){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pi);
				return;
			}
			string command= "DELETE FROM providerident "
				+"WHERE ProviderIdentNum = "+POut.Long(pi.ProviderIdentNum);
 			Db.NonQ(command);
		}

		///<summary>Gets all supplemental identifiers that have been attached to this provider. Used in the provider edit window.</summary>
		public static ProviderIdent[] GetForProv(long provNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ProvNum==provNum).ToArray();
		}

		///<summary>Gets all supplemental identifiers that have been attached to this provider and for this particular payorID.  Called from X12 when creating a claim file.  Also used now on printed claims.</summary>
		public static ProviderIdent[] GetForPayor(long provNum,string payorID) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ProvNum==provNum && x.PayorID==payorID).ToArray();
		}

		///<summary>Called from FormProvEdit if cancel on a new provider.</summary>
		public static void DeleteAllForProv(long provNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),provNum);
				return;
			}
			string command= "DELETE from providerident WHERE provnum = '"+POut.Long(provNum)+"'";
 			Db.NonQ(command);
		}

		/// <summary></summary>
		public static bool IdentExists(ProviderSupplementalID type,long provNum,string payorID) {
			//No need to check RemotingRole; no call to db.
			ProviderIdent providerIdent=GetFirstOrDefault(x => x.ProvNum==provNum && x.SuppIDType==type && x.PayorID==payorID);
			return (providerIdent!=null);
		}
	}
	
}










