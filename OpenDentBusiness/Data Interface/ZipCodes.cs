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
		public static DataTable GetTableFromCache(bool refreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),refreshCache);
				_zipCodeCache.FillCacheFromTable(table);
				return table;
			}
			return _zipCodeCache.GetTableFromCache(refreshCache);
		}

		public static void ClearCache() {
			_zipCodeCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static long Insert(ZipCode zipCode){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				zipCode.ZipCodeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),zipCode);
				return zipCode.ZipCodeNum;
			}
			return Crud.ZipCodeCrud.Insert(zipCode);
		}

		///<summary></summary>
		public static void Update(ZipCode zipCode){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),zipCode);
				return;
			}
			Crud.ZipCodeCrud.Update(zipCode);
		}

		///<summary></summary>
		public static void Delete(ZipCode zipCode){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),zipCode);
				return;
			}
			string command = "DELETE from zipcode WHERE zipcodenum = '"+POut.Long(zipCode.ZipCodeNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<ZipCode> GetALMatches(string zipCodeDigits) {
			Meth.NoCheckMiddleTierRole();
			return GetWhere(x => x.ZipCodeDigits==zipCodeDigits);
		}
	}

	

}













