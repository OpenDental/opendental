using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class StateAbbrs{
		#region CachePattern

		private class StateAbbrCache : CacheListAbs<StateAbbr> {
			protected override List<StateAbbr> GetCacheFromDb() {
				string command="SELECT * FROM stateabbr ORDER BY Abbr";
				return Crud.StateAbbrCrud.SelectMany(command);
			}
			protected override List<StateAbbr> TableToList(DataTable table) {
				return Crud.StateAbbrCrud.TableToList(table);
			}
			protected override StateAbbr Copy(StateAbbr stateAbbr) {
				return stateAbbr.Clone();
			}
			protected override DataTable ListToTable(List<StateAbbr> listStateAbbrs) {
				return Crud.StateAbbrCrud.ListToTable(listStateAbbrs,"StateAbbr");
			}
			protected override void FillCacheIfNeeded() {
				StateAbbrs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static StateAbbrCache _stateAbbrCache=new StateAbbrCache();

		public static List<StateAbbr> GetDeepCopy(bool isShort=false) {
			return _stateAbbrCache.GetDeepCopy(isShort);
		}

		public static List<StateAbbr> GetWhere(Predicate<StateAbbr> match,bool isShort=false) {
			return _stateAbbrCache.GetWhere(match,isShort);
		}

		public static StateAbbr GetFirstOrDefault(Func<StateAbbr,bool> match,bool isShort=false) {
			return _stateAbbrCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_stateAbbrCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_stateAbbrCache.FillCacheFromTable(table);
				return table;
			}
			return _stateAbbrCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(StateAbbr stateAbbr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				stateAbbr.StateAbbrNum=Meth.GetLong(MethodBase.GetCurrentMethod(),stateAbbr);
				return stateAbbr.StateAbbrNum;
			}
			return Crud.StateAbbrCrud.Insert(stateAbbr);
		}

		///<summary></summary>
		public static void Update(StateAbbr stateAbbr) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stateAbbr);
				return;
			}
			Crud.StateAbbrCrud.Update(stateAbbr);
		}

		///<summary></summary>
		public static void Delete(long stateAbbrNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),stateAbbrNum);
				return;
			}
			Crud.StateAbbrCrud.Delete(stateAbbrNum);
		}

		///<summary>Returns a list of StatesAbbrs with abbreviations similar to the supplied string.
		///Used in dropdown list from state field for faster entry.</summary>
		public static List<StateAbbr> GetSimilarAbbrs(string abbr) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.Abbr.StartsWith(abbr,StringComparison.CurrentCultureIgnoreCase));
		}

		///<summary>Returns the Medicaid ID Length for a given abbreviation.</summary>
		public static int GetMedicaidIDLength(string abbr) {
			//No need to check RemotingRole; no call to db.
			StateAbbr stateAbbr=GetFirstOrDefault(x => x.Abbr.ToLower()==abbr.ToLower());
			return (stateAbbr==null ? 0 : stateAbbr.MedicaidIDLength);
		}

		///<summary>Returns true if the abbreviation exists in the stateabbr table.</summary>
		public static bool IsValidAbbr(string abbr) {
			//No need to check RemotingRole; no call to db.
			return (GetFirstOrDefault(x => x.Abbr.ToLower()==abbr.ToLower())!=null);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<StateAbbr> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<StateAbbr>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM stateabbr WHERE PatNum = "+POut.Long(patNum);
			return Crud.StateAbbrCrud.SelectMany(command);
		}

		///<summary>Gets one StateAbbr from the db.</summary>
		public static StateAbbr GetOne(long stateAbbrNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<StateAbbr>(MethodBase.GetCurrentMethod(),stateAbbrNum);
			}
			return Crud.StateAbbrCrud.SelectOne(stateAbbrNum);
		}
		*/
	}
}