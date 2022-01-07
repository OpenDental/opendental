using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Interventions{
		//If this table type will exist as cached data, uncomment the CachePattern region below.
		/*
		#region CachePattern
		private class InterventionCache : CacheListAbs<Intervention> {
			protected override List<Intervention> GetCacheFromDb() {
				string command="SELECT * FROM Intervention ORDER BY ItemOrder";
				return Crud.InterventionCrud.SelectMany(command);
			}
			protected override List<Intervention> TableToList(DataTable table) {
				return Crud.InterventionCrud.TableToList(table);
			}
			protected override Intervention Copy(Intervention Intervention) {
				return Intervention.Clone();
			}
			protected override DataTable ListToTable(List<Intervention> listInterventions) {
				return Crud.InterventionCrud.ListToTable(listInterventions,"Intervention");
			}
			protected override void FillCacheIfNeeded() {
				Interventions.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Intervention Intervention) {
				return !Intervention.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static InterventionCache _InterventionCache=new InterventionCache();

		///<summary>A list of all Interventions. Returns a deep copy.</summary>
		public static List<Intervention> ListDeep {
			get {
				return _InterventionCache.ListDeep;
			}
		}

		///<summary>A list of all visible Interventions. Returns a deep copy.</summary>
		public static List<Intervention> ListShortDeep {
			get {
				return _InterventionCache.ListShortDeep;
			}
		}

		///<summary>A list of all Interventions. Returns a shallow copy.</summary>
		public static List<Intervention> ListShallow {
			get {
				return _InterventionCache.ListShallow;
			}
		}

		///<summary>A list of all visible Interventions. Returns a shallow copy.</summary>
		public static List<Intervention> ListShort {
			get {
				return _InterventionCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_InterventionCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_InterventionCache.FillCacheFromTable(table);
				return table;
			}
			return _InterventionCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		
		///<summary></summary>
		public static long Insert(Intervention intervention) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				intervention.InterventionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),intervention);
				return intervention.InterventionNum;
			}
			return Crud.InterventionCrud.Insert(intervention);
		}

		///<summary></summary>
		public static void Update(Intervention intervention) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),intervention);
				return;
			}
			Crud.InterventionCrud.Update(intervention);
		}

		///<summary></summary>
		public static void Delete(long interventionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),interventionNum);
				return;
			}
			string command= "DELETE FROM intervention WHERE InterventionNum = "+POut.Long(interventionNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<Intervention> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Intervention>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM intervention WHERE PatNum = "+POut.Long(patNum);
			return Crud.InterventionCrud.SelectMany(command);
		}

		public static List<Intervention> Refresh(long patNum,InterventionCodeSet codeSet) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Intervention>>(MethodBase.GetCurrentMethod(),patNum,codeSet);
			}
			string command="SELECT * FROM intervention WHERE PatNum = "+POut.Long(patNum)+" AND CodeSet = "+POut.Int((int)codeSet);
			return Crud.InterventionCrud.SelectMany(command);
		}

		///<summary>Gets list of CodeValue strings from interventions with DateEntry in the last year and CodeSet equal to the supplied codeSet.
		///Result list is grouped by CodeValue, CodeSystem even though we only return the list of CodeValues.  However, there are no codes in the
		///EHR intervention code list that conflict between code systems, so we should never have a duplicate code in the returned list.</summary>
		public static List<string> GetAllForCodeSet(InterventionCodeSet codeSet) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),codeSet);
			}
			string command="SELECT CodeValue FROM intervention WHERE CodeSet="+POut.Int((int)codeSet)+" "
				+"AND "+DbHelper.DtimeToDate("DateEntry")+">="+POut.Date(MiscData.GetNowDateTime().AddYears(-1))+" "
				+"GROUP BY CodeValue,CodeSystem";
			return Db.GetListString(command);
		}

		///<summary>Gets one Intervention from the db.</summary>
		public static Intervention GetOne(long interventionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Intervention>(MethodBase.GetCurrentMethod(),interventionNum);
			}
			return Crud.InterventionCrud.SelectOne(interventionNum);
		}
	}
}