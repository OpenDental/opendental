using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ERoutingActions{
		#region commented cache pattern
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class PatientERoutingActionCache : CacheListAbs<PatientERoutingAction> {
			protected override List<PatientERoutingAction> GetCacheFromDb() {
				string command="SELECT * FROM patienteroutingaction";
				return Crud.PatientERoutingActionCrud.SelectMany(command);
			}
			protected override List<PatientERoutingAction> TableToList(DataTable table) {
				return Crud.PatientERoutingActionCrud.TableToList(table);
			}
			protected override PatientERoutingAction Copy(PatientERoutingAction eroutingAction) {
				return eroutingAction.Copy();
			}
			protected override DataTable ListToTable(List<PatientERoutingAction> listPatientERoutingActions) {
				return Crud.PatientERoutingActionCrud.ListToTable(listPatientERoutingActions,"PatientERoutingAction");
			}
			protected override void FillCacheIfNeeded() {
				PatientERoutingActions.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PatientERoutingActionCache _patientERoutingActionCache=new PatientERoutingActionCache();

		public static List<PatientERoutingAction> GetDeepCopy(bool isShort=false) {
			return _patientERoutingActionCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _patientERoutingActionCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<PatientERoutingAction> match,bool isShort=false) {
			return _patientERoutingActionCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<PatientERoutingAction> match,bool isShort=false) {
			return _patientERoutingActionCache.GetFindIndex(match,isShort);
		}

		public static PatientERoutingAction GetFirst(bool isShort=false) {
			return _patientERoutingActionCache.GetFirst(isShort);
		}

		public static PatientERoutingAction GetFirst(Func<PatientERoutingAction,bool> match,bool isShort=false) {
			return _patientERoutingActionCache.GetFirst(match,isShort);
		}

		public static PatientERoutingAction GetFirstOrDefault(Func<PatientERoutingAction,bool> match,bool isShort=false) {
			return _patientERoutingActionCache.GetFirstOrDefault(match,isShort);
		}

		public static PatientERoutingAction GetLast(bool isShort=false) {
			return _patientERoutingActionCache.GetLast(isShort);
		}

		public static PatientERoutingAction GetLastOrDefault(Func<PatientERoutingAction,bool> match,bool isShort=false) {
			return _patientERoutingActionCache.GetLastOrDefault(match,isShort);
		}

		public static List<PatientERoutingAction> GetWhere(Predicate<PatientERoutingAction> match,bool isShort=false) {
			return _patientERoutingActionCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_patientERoutingActionCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_patientERoutingActionCache.FillCacheFromTable(table);
				return table;
			}
			return _patientERoutingActionCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		#endregion
		#region Methods - Get
		///<summary></summary>
		public static List<ERoutingAction> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingAction>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM eroutingaction WHERE PatNum = "+POut.Long(patNum);
			return Crud.ERoutingActionCrud.SelectMany(command);
		}
		
		///<summary>Gets one ERoutingAction from the db.</summary>
		public static ERoutingAction GetOne(long eroutingActionNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ERoutingAction>(MethodBase.GetCurrentMethod(),eroutingActionNum);
			}
			return Crud.ERoutingActionCrud.SelectOne(eroutingActionNum);
		}

		///<summary>Gets one ERoutingAction from the db.</summary>
		public static List<ERoutingAction> GetListForERouting(long eroutingNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ERoutingAction>>(MethodBase.GetCurrentMethod(),eroutingNum);
			}
			string command = $"SELECT * FROM eroutingaction WHERE ERoutingNum = {eroutingNum}";
			return Crud.ERoutingActionCrud.SelectMany(command);
		}

		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ERoutingAction eroutingAction){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				eroutingAction.ERoutingActionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),eroutingAction);
				return eroutingAction.ERoutingActionNum;
			}
			return Crud.ERoutingActionCrud.Insert(eroutingAction);
		}
		///<summary></summary>
		public static void Update(ERoutingAction eroutingAction){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingAction);
				return;
			}
			Crud.ERoutingActionCrud.Update(eroutingAction);
		}
		///<summary></summary>
		public static void Delete(long eroutingActionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),eroutingActionNum);
				return;
			}
			Crud.ERoutingActionCrud.Delete(eroutingActionNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}