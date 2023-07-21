using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AutoCommExcludeDates{
        //If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
        /*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class AutoCommExcludeDateCache : CacheListAbs<AutoCommExcludeDate> {
			protected override List<AutoCommExcludeDate> GetCacheFromDb() {
				string command="SELECT * FROM autocommexcludedate";
				return Crud.AutoCommExcludeDateCrud.SelectMany(command);
			}
			protected override List<AutoCommExcludeDate> TableToList(DataTable table) {
				return Crud.AutoCommExcludeDateCrud.TableToList(table);
			}
			protected override AutoCommExcludeDate Copy(AutoCommExcludeDate autoCommExcludeDate) {
				return autoCommExcludeDate.Copy();
			}
			protected override DataTable ListToTable(List<AutoCommExcludeDate> listAutoCommExcludeDates) {
				return Crud.AutoCommExcludeDateCrud.ListToTable(listAutoCommExcludeDates,"AutoCommExcludeDate");
			}
			protected override void FillCacheIfNeeded() {
				AutoCommExcludeDates.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutoCommExcludeDateCache _autoCommExcludeDateCache=new AutoCommExcludeDateCache();

		public static List<AutoCommExcludeDate> GetDeepCopy(bool isShort=false) {
			return _autoCommExcludeDateCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _autoCommExcludeDateCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<AutoCommExcludeDate> match,bool isShort=false) {
			return _autoCommExcludeDateCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<AutoCommExcludeDate> match,bool isShort=false) {
			return _autoCommExcludeDateCache.GetFindIndex(match,isShort);
		}

		public static AutoCommExcludeDate GetFirst(bool isShort=false) {
			return _autoCommExcludeDateCache.GetFirst(isShort);
		}

		public static AutoCommExcludeDate GetFirst(Func<AutoCommExcludeDate,bool> match,bool isShort=false) {
			return _autoCommExcludeDateCache.GetFirst(match,isShort);
		}

		public static AutoCommExcludeDate GetFirstOrDefault(Func<AutoCommExcludeDate,bool> match,bool isShort=false) {
			return _autoCommExcludeDateCache.GetFirstOrDefault(match,isShort);
		}

		public static AutoCommExcludeDate GetLast(bool isShort=false) {
			return _autoCommExcludeDateCache.GetLast(isShort);
		}

		public static AutoCommExcludeDate GetLastOrDefault(Func<AutoCommExcludeDate,bool> match,bool isShort=false) {
			return _autoCommExcludeDateCache.GetLastOrDefault(match,isShort);
		}

		public static List<AutoCommExcludeDate> GetWhere(Predicate<AutoCommExcludeDate> match,bool isShort=false) {
			return _autoCommExcludeDateCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_autoCommExcludeDateCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_autoCommExcludeDateCache.FillCacheFromTable(table);
				return table;
			}
			return _autoCommExcludeDateCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/

		#region Methods - Get
		///<summary></summary>
		public static List<AutoCommExcludeDate> Refresh(long clinicNum) {
            if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
                return Meth.GetObject<List<AutoCommExcludeDate>>(MethodBase.GetCurrentMethod(),clinicNum);
            }
            string command = $"SELECT * FROM autocommexcludedate WHERE ClinicNum = {POut.Long(clinicNum)} ORDER BY autocommexcludedate.DateExclude ASC";
            return Crud.AutoCommExcludeDateCrud.SelectMany(command);
        }

        ///<summary>Gets one AutoCommExcludeDate from the db.</summary>
        public static AutoCommExcludeDate GetOne(long autoCommExcludeDateNum) {
            if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
                return Meth.GetObject<AutoCommExcludeDate>(MethodBase.GetCurrentMethod(),autoCommExcludeDateNum);
            }
            return Crud.AutoCommExcludeDateCrud.SelectOne(autoCommExcludeDateNum);
        }

		/// <summary>Clinic 0 is used for HQ Clinic or when clinics are turned off. </summary>
		public static List<AutoCommExcludeDate> GetFutureForClinic(long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AutoCommExcludeDate>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command = $"SELECT * FROM autocommexcludedate WHERE ClinicNum = {POut.Long(clinicNum)} AND DateExclude >= CURDATE() ORDER BY autocommexcludedate.DateExclude ASC";
			return Crud.AutoCommExcludeDateCrud.SelectMany(command);
		}

		public static List<AutoCommExcludeDate> GetAllFuture() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<AutoCommExcludeDate>>(MethodBase.GetCurrentMethod());
			}
			string command = $"SELECT * FROM autocommexcludedate WHERE DateExclude >= CURDATE() ORDER BY autocommexcludedate.DateExclude ASC";
			return Crud.AutoCommExcludeDateCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(AutoCommExcludeDate autoCommExcludeDate) {
            if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
                autoCommExcludeDate.AutoCommExcludeDateNum=Meth.GetLong(MethodBase.GetCurrentMethod(),autoCommExcludeDate);
                return autoCommExcludeDate.AutoCommExcludeDateNum;
            }
            return Crud.AutoCommExcludeDateCrud.Insert(autoCommExcludeDate);
        }

        ///<summary></summary>
        public static void Delete(long autoCommExcludeDateNum) {
            if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
                Meth.GetVoid(MethodBase.GetCurrentMethod(),autoCommExcludeDateNum);
                return;
            }
            Crud.AutoCommExcludeDateCrud.Delete(autoCommExcludeDateNum);
        }
        #endregion Methods - Modify

        #region Methods - Misc



        #endregion Methods - Misc




    }
}