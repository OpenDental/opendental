using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FlowActions{
		#region commented cache pattern
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class PatientFlowActionCache : CacheListAbs<PatientFlowAction> {
			protected override List<PatientFlowAction> GetCacheFromDb() {
				string command="SELECT * FROM patientflowaction";
				return Crud.PatientFlowActionCrud.SelectMany(command);
			}
			protected override List<PatientFlowAction> TableToList(DataTable table) {
				return Crud.PatientFlowActionCrud.TableToList(table);
			}
			protected override PatientFlowAction Copy(PatientFlowAction flowAction) {
				return flowAction.Copy();
			}
			protected override DataTable ListToTable(List<PatientFlowAction> listPatientFlowActions) {
				return Crud.PatientFlowActionCrud.ListToTable(listPatientFlowActions,"PatientFlowAction");
			}
			protected override void FillCacheIfNeeded() {
				PatientFlowActions.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PatientFlowActionCache _patientFlowActionCache=new PatientFlowActionCache();

		public static List<PatientFlowAction> GetDeepCopy(bool isShort=false) {
			return _patientFlowActionCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _patientFlowActionCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<PatientFlowAction> match,bool isShort=false) {
			return _patientFlowActionCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<PatientFlowAction> match,bool isShort=false) {
			return _patientFlowActionCache.GetFindIndex(match,isShort);
		}

		public static PatientFlowAction GetFirst(bool isShort=false) {
			return _patientFlowActionCache.GetFirst(isShort);
		}

		public static PatientFlowAction GetFirst(Func<PatientFlowAction,bool> match,bool isShort=false) {
			return _patientFlowActionCache.GetFirst(match,isShort);
		}

		public static PatientFlowAction GetFirstOrDefault(Func<PatientFlowAction,bool> match,bool isShort=false) {
			return _patientFlowActionCache.GetFirstOrDefault(match,isShort);
		}

		public static PatientFlowAction GetLast(bool isShort=false) {
			return _patientFlowActionCache.GetLast(isShort);
		}

		public static PatientFlowAction GetLastOrDefault(Func<PatientFlowAction,bool> match,bool isShort=false) {
			return _patientFlowActionCache.GetLastOrDefault(match,isShort);
		}

		public static List<PatientFlowAction> GetWhere(Predicate<PatientFlowAction> match,bool isShort=false) {
			return _patientFlowActionCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_patientFlowActionCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_patientFlowActionCache.FillCacheFromTable(table);
				return table;
			}
			return _patientFlowActionCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		#endregion
		#region Methods - Get
		///<summary></summary>
		public static List<FlowAction> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FlowAction>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM flowaction WHERE PatNum = "+POut.Long(patNum);
			return Crud.FlowActionCrud.SelectMany(command);
		}
		
		///<summary>Gets one FlowAction from the db.</summary>
		public static FlowAction GetOne(long flowActionNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<FlowAction>(MethodBase.GetCurrentMethod(),flowActionNum);
			}
			return Crud.FlowActionCrud.SelectOne(flowActionNum);
		}

		///<summary>Gets one FlowAction from the db.</summary>
		public static List<FlowAction> GetListForFlow(long flowNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FlowAction>>(MethodBase.GetCurrentMethod(),flowNum);
			}
			string command = $"SELECT * FROM flowaction WHERE FlowNum = {flowNum}";
			return Crud.FlowActionCrud.SelectMany(command);
		}

		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(FlowAction flowAction){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				flowAction.FlowActionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),flowAction);
				return flowAction.FlowActionNum;
			}
			return Crud.FlowActionCrud.Insert(flowAction);
		}
		///<summary></summary>
		public static void Update(FlowAction flowAction){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowAction);
				return;
			}
			Crud.FlowActionCrud.Update(flowAction);
		}
		///<summary></summary>
		public static void Delete(long flowActionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowActionNum);
				return;
			}
			Crud.FlowActionCrud.Delete(flowActionNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc



	}
}