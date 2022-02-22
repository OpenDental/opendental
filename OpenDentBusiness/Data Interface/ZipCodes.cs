using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
  ///<summary></summary>
	public class ZipCodes{
		#region CachePattern

		private class ZipCodeCache : CacheListAbs<ZipCode> {
			protected override List<ZipCode> GetCacheFromDb() {
				string command="SELECT * from zipcode ORDER BY ZipCodeDigits";
				return Crud.ZipCodeCrud.SelectMany(command);
			}
			protected override List<ZipCode> TableToList(DataTable table) {
				return Crud.ZipCodeCrud.TableToList(table);
			}
			protected override ZipCode Copy(ZipCode zipCode) {
				return zipCode.Copy();
			}
			protected override DataTable ListToTable(List<ZipCode> listZipCodes) {
				return Crud.ZipCodeCrud.ListToTable(listZipCodes,"ZipCode");
			}
			protected override void FillCacheIfNeeded() {
				ZipCodes.GetTableFromCache(false);
			}

			/// <summary>The zipcode "Short" list is for zipcodes marked frequent, not hidden.</summary>
			protected override bool IsInListShort(ZipCode zipCode) {
				return zipCode.IsFrequent;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ZipCodeCache _zipCodeCache=new ZipCodeCache();

		public static List<ZipCode> GetWhere(Predicate<ZipCode> match,bool isShort=false) {
			return _zipCodeCache.GetWhere(match,isShort);
		}

		public static List<ZipCode> GetDeepCopy(bool isShort=false) {
			return _zipCodeCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_zipCodeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_zipCodeCache.FillCacheFromTable(table);
				return table;
			}
			return _zipCodeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(ZipCode Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.ZipCodeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.ZipCodeNum;
			}
			return Crud.ZipCodeCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Update(ZipCode Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.ZipCodeCrud.Update(Cur);
		}

		///<summary></summary>
		public static void Delete(ZipCode Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command = "DELETE from zipcode WHERE zipcodenum = '"+POut.Long(Cur.ZipCodeNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<ZipCode> GetALMatches(string zipCodeDigits) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ZipCodeDigits==zipCodeDigits);
		}
	}

	

}













