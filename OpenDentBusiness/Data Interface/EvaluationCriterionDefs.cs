using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EvaluationCriterionDefs{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class EvaluationCriterionDefCache : CacheListAbs<EvaluationCriterionDef> {
			protected override List<EvaluationCriterionDef> GetCacheFromDb() {
				string command="SELECT * FROM EvaluationCriterionDef ORDER BY ItemOrder";
				return Crud.EvaluationCriterionDefCrud.SelectMany(command);
			}
			protected override List<EvaluationCriterionDef> TableToList(DataTable table) {
				return Crud.EvaluationCriterionDefCrud.TableToList(table);
			}
			protected override EvaluationCriterionDef Copy(EvaluationCriterionDef EvaluationCriterionDef) {
				return EvaluationCriterionDef.Clone();
			}
			protected override DataTable ListToTable(List<EvaluationCriterionDef> listEvaluationCriterionDefs) {
				return Crud.EvaluationCriterionDefCrud.ListToTable(listEvaluationCriterionDefs,"EvaluationCriterionDef");
			}
			protected override void FillCacheIfNeeded() {
				EvaluationCriterionDefs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(EvaluationCriterionDef EvaluationCriterionDef) {
				return !EvaluationCriterionDef.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EvaluationCriterionDefCache _EvaluationCriterionDefCache=new EvaluationCriterionDefCache();

		///<summary>A list of all EvaluationCriterionDefs. Returns a deep copy.</summary>
		public static List<EvaluationCriterionDef> ListDeep {
			get {
				return _EvaluationCriterionDefCache.ListDeep;
			}
		}

		///<summary>A list of all visible EvaluationCriterionDefs. Returns a deep copy.</summary>
		public static List<EvaluationCriterionDef> ListShortDeep {
			get {
				return _EvaluationCriterionDefCache.ListShortDeep;
			}
		}

		///<summary>A list of all EvaluationCriterionDefs. Returns a shallow copy.</summary>
		public static List<EvaluationCriterionDef> ListShallow {
			get {
				return _EvaluationCriterionDefCache.ListShallow;
			}
		}

		///<summary>A list of all visible EvaluationCriterionDefs. Returns a shallow copy.</summary>
		public static List<EvaluationCriterionDef> ListShort {
			get {
				return _EvaluationCriterionDefCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_EvaluationCriterionDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_EvaluationCriterionDefCache.FillCacheFromTable(table);
				return table;
			}
			return _EvaluationCriterionDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/


		///<summary>Gets a list of all possible EvaluationCriterionDefs.  Defs attached to an EvaluationDef are copies and will not be shown.</summary>
		public static List<EvaluationCriterionDef> GetAvailableCriterionDefs() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EvaluationCriterionDef>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM evaluationcriteriondef where EvaluationDefNum=0";
			return Crud.EvaluationCriterionDefCrud.SelectMany(command);
		}

		///<summary>Gets a list of all EvaluationCriterion attached to an EvaluationDef.  Ordered by ItemOrder.</summary>
		public static List<EvaluationCriterionDef> GetAllForEvaluationDef(long evaluationDefNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EvaluationCriterionDef>>(MethodBase.GetCurrentMethod(),evaluationDefNum);
			}
			string command="SELECT * FROM evaluationcriteriondef WHERE EvaluationDefNum = "+POut.Long(evaluationDefNum)+" "
				+"ORDER BY ItemOrder";
			return Crud.EvaluationCriterionDefCrud.SelectMany(command);
		}

		///<summary>Gets one EvaluationCriterionDef from the db.</summary>
		public static EvaluationCriterionDef GetOne(long evaluationCriterionDefNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EvaluationCriterionDef>(MethodBase.GetCurrentMethod(),evaluationCriterionDefNum);
			}
			return Crud.EvaluationCriterionDefCrud.SelectOne(evaluationCriterionDefNum);
		}

		///<summary></summary>
		public static long Insert(EvaluationCriterionDef evaluationCriterionDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				evaluationCriterionDef.EvaluationCriterionDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),evaluationCriterionDef);
				return evaluationCriterionDef.EvaluationCriterionDefNum;
			}
			return Crud.EvaluationCriterionDefCrud.Insert(evaluationCriterionDef);
		}

		///<summary></summary>
		public static void Update(EvaluationCriterionDef evaluationCriterionDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),evaluationCriterionDef);
				return;
			}
			Crud.EvaluationCriterionDefCrud.Update(evaluationCriterionDef);
		}

		///<summary></summary>
		public static void Delete(long evaluationCriterionDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),evaluationCriterionDefNum);
				return;
			}
			string command= "DELETE FROM evaluationcriteriondef WHERE EvaluationCriterionDefNum = "+POut.Long(evaluationCriterionDefNum);
			Db.NonQ(command);
		}
	}
}