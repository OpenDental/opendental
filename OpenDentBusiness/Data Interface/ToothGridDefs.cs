using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ToothGridDefs{
		/*
		#region CachePattern

		private class ToothGridDefCache : CacheListAbs<ToothGridDef> {
			protected override List<ToothGridDef> GetCacheFromDb() {
				string command="SELECT * FROM ToothGridDef ORDER BY ItemOrder";
				return Crud.ToothGridDefCrud.SelectMany(command);
			}
			protected override List<ToothGridDef> TableToList(DataTable table) {
				return Crud.ToothGridDefCrud.TableToList(table);
			}
			protected override ToothGridDef Copy(ToothGridDef ToothGridDef) {
				return ToothGridDef.Clone();
			}
			protected override DataTable ListToTable(List<ToothGridDef> listToothGridDefs) {
				return Crud.ToothGridDefCrud.ListToTable(listToothGridDefs,"ToothGridDef");
			}
			protected override void FillCacheIfNeeded() {
				ToothGridDefs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(ToothGridDef ToothGridDef) {
				return !ToothGridDef.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ToothGridDefCache _ToothGridDefCache=new ToothGridDefCache();

		///<summary>A list of all ToothGridDefs. Returns a deep copy.</summary>
		public static List<ToothGridDef> ListDeep {
			get {
				return _ToothGridDefCache.ListDeep;
			}
		}

		///<summary>A list of all visible ToothGridDefs. Returns a deep copy.</summary>
		public static List<ToothGridDef> ListShortDeep {
			get {
				return _ToothGridDefCache.ListShortDeep;
			}
		}

		///<summary>A list of all ToothGridDefs. Returns a shallow copy.</summary>
		public static List<ToothGridDef> ListShallow {
			get {
				return _ToothGridDefCache.ListShallow;
			}
		}

		///<summary>A list of all visible ToothGridDefs. Returns a shallow copy.</summary>
		public static List<ToothGridDef> ListShort {
			get {
				return _ToothGridDefCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ToothGridDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ToothGridDefCache.FillCacheFromTable(table);
				return table;
			}
			return _ToothGridDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion*/

		public static List<ToothGridDef> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ToothGridDef>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM toothgriddef WHERE toothgriddefnum = "+POut.Long(patNum);
			return Crud.ToothGridDefCrud.SelectMany(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<ToothGridDef> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ToothGridDef>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM toothgriddef WHERE PatNum = "+POut.Long(patNum);
			return Crud.ToothGridDefCrud.SelectMany(command);
		}

		///<summary>Gets one ToothGridDef from the db.</summary>
		public static ToothGridDef GetOne(long toothGridDefNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ToothGridDef>(MethodBase.GetCurrentMethod(),toothGridDefNum);
			}
			return Crud.ToothGridDefCrud.SelectOne(toothGridDefNum);
		}

		///<summary></summary>
		public static long Insert(ToothGridDef toothGridDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				toothGridDef.ToothGridDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),toothGridDef);
				return toothGridDef.ToothGridDefNum;
			}
			return Crud.ToothGridDefCrud.Insert(toothGridDef);
		}

		///<summary></summary>
		public static void Update(ToothGridDef toothGridDef){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),toothGridDef);
				return;
			}
			Crud.ToothGridDefCrud.Update(toothGridDef);
		}

		///<summary></summary>
		public static void Delete(long toothGridDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),toothGridDefNum);
				return;
			}
			string command= "DELETE FROM toothgriddef WHERE ToothGridDefNum = "+POut.Long(toothGridDefNum);
			Db.NonQ(command);
		}
		*/
	}
}