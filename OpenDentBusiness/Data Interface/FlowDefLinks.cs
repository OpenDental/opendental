using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FlowDefLinks{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class FlowDefLinkCache : CacheListAbs<FlowDefLink> {
			protected override List<FlowDefLink> GetCacheFromDb() {
				string command="SELECT * FROM flowdeflink";
				return Crud.FlowDefLinkCrud.SelectMany(command);
			}
			protected override List<FlowDefLink> TableToList(DataTable table) {
				return Crud.FlowDefLinkCrud.TableToList(table);
			}
			protected override FlowDefLink Copy(FlowDefLink patientFlowDefLink) {
				return patientFlowDefLink.Copy();
			}
			protected override DataTable ListToTable(List<FlowDefLink> listFlowDefLinks) {
				return Crud.FlowDefLinkCrud.ListToTable(listFlowDefLinks,"FlowDefLink");
			}
			protected override void FillCacheIfNeeded() {
				FlowDefLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static FlowDefLinkCache _flowDefLinkCache=new FlowDefLinkCache();

		public static List<FlowDefLink> GetDeepCopy(bool isShort=false) {
			return _flowDefLinkCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _flowDefLinkCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<FlowDefLink> match,bool isShort=false) {
			return _flowDefLinkCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<FlowDefLink> match,bool isShort=false) {
			return _flowDefLinkCache.GetFindIndex(match,isShort);
		}

		public static FlowDefLink GetFirst(bool isShort=false) {
			return _flowDefLinkCache.GetFirst(isShort);
		}

		public static FlowDefLink GetFirst(Func<FlowDefLink,bool> match,bool isShort=false) {
			return _flowDefLinkCache.GetFirst(match,isShort);
		}

		public static FlowDefLink GetFirstOrDefault(Func<FlowDefLink,bool> match,bool isShort=false) {
			return _flowDefLinkCache.GetFirstOrDefault(match,isShort);
		}

		public static FlowDefLink GetLast(bool isShort=false) {
			return _flowDefLinkCache.GetLast(isShort);
		}

		public static FlowDefLink GetLastOrDefault(Func<FlowDefLink,bool> match,bool isShort=false) {
			return _flowDefLinkCache.GetLastOrDefault(match,isShort);
		}

		public static List<FlowDefLink> GetWhere(Predicate<FlowDefLink> match,bool isShort=false) {
			return _flowDefLinkCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_flowDefLinkCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientMT's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if MiddleTierRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_flowDefLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _flowDefLinkCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Methods - Get
		///<summary></summary>
		public static List<FlowDefLink> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FlowDefLink>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command = "SELECT * FROM flowdeflink WHERE PatNum = "+POut.Long(patNum);
			return Crud.FlowDefLinkCrud.SelectMany(command);
		}

		///<summary>Gets a list of each type of def link a give patientflow has. Used in FormFlowEdit to know what kinds of links are selected.</summary>
		public static List<FlowDefLink> GetListFlowTypesForFlowDefNum(long flowDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<FlowDefLink>>(MethodBase.GetCurrentMethod(),flowDefNum);
			}
			string command = $"SELECT * FROM flowdeflink WHERE FlowDefNum = {POut.Long(flowDefNum)} GROUP BY FlowType";
			return Crud.FlowDefLinkCrud.SelectMany(command).ToList();
		}

		///<summary>Gets one PatientFlowDefLink from the db.</summary>
		public static FlowDefLink GetOne(long flowDefLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<FlowDefLink>(MethodBase.GetCurrentMethod(),flowDefLinkNum);
			}
			return Crud.FlowDefLinkCrud.SelectOne(flowDefLinkNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(FlowDefLink flowDefLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				flowDefLink.FlowDefLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),flowDefLink);
				return flowDefLink.FlowDefLinkNum;
			}
			return Crud.FlowDefLinkCrud.Insert(flowDefLink);
		}

		///<summary></summary>
		public static void Update(FlowDefLink flowDefLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowDefLink);
				return;
			}
			Crud.FlowDefLinkCrud.Update(flowDefLink);
		}
		///<summary></summary>
		public static void Delete(long flowDefLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowDefLinkNum);
				return;
			}
			Crud.FlowDefLinkCrud.Delete(flowDefLinkNum);
		}
		
		/// <summary>Deletes everything for a given FlowDefNum. Used during setup to create a clean slate and only save what is selected in form.</summary>
		public static void DeleteAll(long flowDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),flowDefNum);
				return;
			}
			string command = $"DELETE FROM flowdeflink WHERE FlowDefNum = {POut.Long(flowDefNum)}";
			Db.NonQ(command);
		}

		#endregion Methods - Modify
		#region Methods - Misc



		#endregion Methods - Misc




	}
}