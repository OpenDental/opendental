using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FieldDefLinks{
		#region CachePattern
		private class FieldDefLinkCache:CacheListAbs<FieldDefLink> {
			protected override List<FieldDefLink> GetCacheFromDb() {
				string command="SELECT * FROM fielddeflink";
				return Crud.FieldDefLinkCrud.SelectMany(command);
			}
			protected override List<FieldDefLink> TableToList(DataTable table) {
				return Crud.FieldDefLinkCrud.TableToList(table);
			}
			protected override FieldDefLink Copy(FieldDefLink fieldDefLink) {
				return fieldDefLink.Clone();
			}
			protected override DataTable ListToTable(List<FieldDefLink> listFieldDefLinks) {
				return Crud.FieldDefLinkCrud.ListToTable(listFieldDefLinks,"FieldDefLink");
			}
			protected override void FillCacheIfNeeded() {
				FieldDefLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static FieldDefLinkCache _fieldDefLinkCache=new FieldDefLinkCache();

		public static bool GetExists(Predicate<FieldDefLink> match,bool isShort = false) {
			return _fieldDefLinkCache.GetExists(match,isShort);
		}

		public static List<FieldDefLink> GetDeepCopy(bool isShort = false) {
			return _fieldDefLinkCache.GetDeepCopy(isShort);
		}

		public static FieldDefLink GetFirstOrDefault(Func<FieldDefLink,bool> match,bool isShort = false) {
			return _fieldDefLinkCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_fieldDefLinkCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_fieldDefLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _fieldDefLinkCache.GetTableFromCache(doRefreshCache);
		}
		#endregion

		public static bool Sync(List<FieldDefLink> listNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew);
			}
			string command="SELECT * FROM fielddeflink";
			List<FieldDefLink> listDB=Crud.FieldDefLinkCrud.SelectMany(command);
			return Crud.FieldDefLinkCrud.Sync(listNew,listDB);
		}

		///<summary>Deletes all fieldDefLink rows that are associated to the given fieldDefNum and fieldDefType.</summary>
		public static void DeleteForFieldDefNum(long fieldDefNum,FieldDefTypes fieldDefType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fieldDefNum,fieldDefType);
				return;
			}
			if(fieldDefNum==0) {
					return;
			}
			//Only delete records of the correct fieldDefType (Pat vs Appt)
			Db.NonQ("DELETE FROM fielddeflink WHERE FieldDefNum="+POut.Long(fieldDefNum)+" AND FieldDefType="+POut.Int((int)fieldDefType));
		}
	}
}