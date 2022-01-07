using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Ebills{
		#region CachePattern

		private class EbillCache : CacheListAbs<Ebill> {
			protected override List<Ebill> GetCacheFromDb() {
				string command="SELECT * FROM ebill";
				return Crud.EbillCrud.SelectMany(command);
			}
			protected override List<Ebill> TableToList(DataTable table) {
				return Crud.EbillCrud.TableToList(table);
			}
			protected override Ebill Copy(Ebill Ebill) {
				return Ebill.Copy();
			}
			protected override DataTable ListToTable(List<Ebill> listEbills) {
				return Crud.EbillCrud.ListToTable(listEbills,"Ebill");
			}
			protected override void FillCacheIfNeeded() {
				Ebills.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EbillCache _EbillCache=new EbillCache();

		public static List<Ebill> GetDeepCopy(bool isShort=false) {
			return _EbillCache.GetDeepCopy(isShort);
		}

		public static Ebill GetFirstOrDefault(Func<Ebill,bool> match,bool isShort=false) {
			return _EbillCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EbillCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EbillCache.FillCacheFromTable(table);
				return table;
			}
			return _EbillCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		/*
		 
		///<summary></summary>
		public static List<Ebill> GetForPat(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Ebill>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ebill WHERE PatNum = "+POut.Long(patNum);
			return Crud.EbillCrud.SelectMany(command);
		}

		///<summary>Gets one Ebill from the db.</summary>
		public static Ebill GetOne(long ebillNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Ebill>(MethodBase.GetCurrentMethod(),ebillNum);
			}
			return Crud.EbillCrud.SelectOne(ebillNum);
		}

		///<summary></summary>
		public static long Insert(Ebill ebill){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				ebill.EbillNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ebill);
				return ebill.EbillNum;
			}
			return Crud.EbillCrud.Insert(ebill);
		}

		///<summary></summary>
		public static void Update(Ebill ebill){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ebill);
				return;
			}
			Crud.EbillCrud.Update(ebill);
		}
		
		 ///<summary></summary>
		public static void Delete(long ebillNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ebillNum);
				return;
			}
			Crud.EbillCrud.Delete(ebillNum);
		} 
		 
		*/

		///<summary>To get the defaults, use clinicNum=0.</summary>
		public static Ebill GetForClinic(long clinicNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ClinicNum==clinicNum);
		}

		public static bool Sync(List<Ebill> listNew,List<Ebill> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.EbillCrud.Sync(listNew,listOld);
		}
	}
}