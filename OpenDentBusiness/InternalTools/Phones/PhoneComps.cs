using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PhoneComps {
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class PhoneCompCache : CacheListAbs<PhoneComp> {
			protected override List<PhoneComp> GetCacheFromDb() {
				string command="SELECT * FROM phonecomp";
				return Crud.PhoneCompCrud.SelectMany(command);
			}
			protected override List<PhoneComp> TableToList(DataTable table) {
				return Crud.PhoneCompCrud.TableToList(table);
			}
			protected override PhoneComp Copy(PhoneComp phoneComp) {
				return phoneComp.Copy();
			}
			protected override DataTable ListToTable(List<PhoneComp> listPhoneComps) {
				return Crud.PhoneCompCrud.ListToTable(listPhoneComps,"PhoneComp");
			}
			protected override void FillCacheIfNeeded() {
				PhoneComps.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PhoneCompCache _phoneCompCache=new PhoneCompCache();

		public static List<PhoneComp> GetDeepCopy(bool isShort=false) {
			return _phoneCompCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _phoneCompCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<PhoneComp> match,bool isShort=false) {
			return _phoneCompCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<PhoneComp> match,bool isShort=false) {
			return _phoneCompCache.GetFindIndex(match,isShort);
		}

		public static PhoneComp GetFirst(bool isShort=false) {
			return _phoneCompCache.GetFirst(isShort);
		}

		public static PhoneComp GetFirst(Func<PhoneComp,bool> match,bool isShort=false) {
			return _phoneCompCache.GetFirst(match,isShort);
		}

		public static PhoneComp GetFirstOrDefault(Func<PhoneComp,bool> match,bool isShort=false) {
			return _phoneCompCache.GetFirstOrDefault(match,isShort);
		}

		public static PhoneComp GetLast(bool isShort=false) {
			return _phoneCompCache.GetLast(isShort);
		}

		public static PhoneComp GetLastOrDefault(Func<PhoneComp,bool> match,bool isShort=false) {
			return _phoneCompCache.GetLastOrDefault(match,isShort);
		}

		public static List<PhoneComp> GetWhere(Predicate<PhoneComp> match,bool isShort=false) {
			return _phoneCompCache.GetWhere(match,isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_phoneCompCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_phoneCompCache.FillCacheFromTable(table);
				return table;
			}
			return _phoneCompCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Get Methods

		///<summary>Returns 0 if the extension could not be found.</summary>
		public static int GetExtForComputer(string computerName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),computerName);
			}
			string command="SELECT PhoneExt FROM phonecomp WHERE ComputerName='"+POut.String(computerName)+"'";
			return Db.GetInt(command);
		}

		#endregion
		#region Modification Methods

		#region Insert
		#endregion
		#region Update
		#endregion
		#region Delete
		#endregion

		public static bool Sync(List<PhoneComp> listNew,List<PhoneComp> listDB) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listDB);
			}
			return Crud.PhoneCompCrud.Sync(listNew,listDB);
		}

		#endregion
		#region Misc Methods
		#endregion



	}
}