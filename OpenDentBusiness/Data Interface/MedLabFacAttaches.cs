using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MedLabFacAttaches{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class MedLabFacAttachCache : CacheListAbs<MedLabFacAttach> {
			protected override List<MedLabFacAttach> GetCacheFromDb() {
				string command="SELECT * FROM MedLabFacAttach ORDER BY ItemOrder";
				return Crud.MedLabFacAttachCrud.SelectMany(command);
			}
			protected override List<MedLabFacAttach> TableToList(DataTable table) {
				return Crud.MedLabFacAttachCrud.TableToList(table);
			}
			protected override MedLabFacAttach Copy(MedLabFacAttach MedLabFacAttach) {
				return MedLabFacAttach.Clone();
			}
			protected override DataTable ListToTable(List<MedLabFacAttach> listMedLabFacAttachs) {
				return Crud.MedLabFacAttachCrud.ListToTable(listMedLabFacAttachs,"MedLabFacAttach");
			}
			protected override void FillCacheIfNeeded() {
				MedLabFacAttachs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(MedLabFacAttach MedLabFacAttach) {
				return !MedLabFacAttach.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static MedLabFacAttachCache _MedLabFacAttachCache=new MedLabFacAttachCache();

		///<summary>A list of all MedLabFacAttachs. Returns a deep copy.</summary>
		public static List<MedLabFacAttach> ListDeep {
			get {
				return _MedLabFacAttachCache.ListDeep;
			}
		}

		///<summary>A list of all visible MedLabFacAttachs. Returns a deep copy.</summary>
		public static List<MedLabFacAttach> ListShortDeep {
			get {
				return _MedLabFacAttachCache.ListShortDeep;
			}
		}

		///<summary>A list of all MedLabFacAttachs. Returns a shallow copy.</summary>
		public static List<MedLabFacAttach> ListShallow {
			get {
				return _MedLabFacAttachCache.ListShallow;
			}
		}

		///<summary>A list of all visible MedLabFacAttachs. Returns a shallow copy.</summary>
		public static List<MedLabFacAttach> ListShort {
			get {
				return _MedLabFacAttachCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_MedLabFacAttachCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_MedLabFacAttachCache.FillCacheFromTable(table);
				return table;
			}
			return _MedLabFacAttachCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary></summary>
		public static long Insert(MedLabFacAttach medLabFacAttach) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				medLabFacAttach.MedLabFacAttachNum=Meth.GetLong(MethodBase.GetCurrentMethod(),medLabFacAttach);
				return medLabFacAttach.MedLabFacAttachNum;
			}
			return Crud.MedLabFacAttachCrud.Insert(medLabFacAttach);
		}

		///<summary>Gets all MedLabFacAttach objects from the db for a MedLab or a MedLabResult.  Only one parameter is required,
		///EITHER a MedLabNum OR a MedLabResultNum.  The other parameter should be 0.  If both parameters are >0, then list
		///returned will be all MedLabFacAttaches with EITHER the MedLabNum OR the MedLabResultNum provided.</summary>
		public static List<MedLabFacAttach> GetAllForLabOrResult(long medLabNum,long medLabResultNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedLabFacAttach>>(MethodBase.GetCurrentMethod(),medLabNum,medLabResultNum);
			}
			string command="SELECT * FROM medlabfacattach WHERE ";
			if(medLabNum!=0) {
				command+="MedLabNum="+POut.Long(medLabNum);
			}
			if(medLabResultNum!=0) {
				if(medLabNum!=0) {
					command+=" OR ";
				}
				command+="MedLabResultNum="+POut.Long(medLabResultNum)+" ";
			}
			command+="ORDER BY MedLabFacAttachNum DESC";
			return Crud.MedLabFacAttachCrud.SelectMany(command);
		}

		public static List<MedLabFacAttach> GetAllForResults(List<long> listResultNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedLabFacAttach>>(MethodBase.GetCurrentMethod(),listResultNums);
			}
			string command="SELECT * FROM medlabfacattach WHERE MedLabResultNum IN("+String.Join(",",listResultNums)+")";
			return Crud.MedLabFacAttachCrud.SelectMany(command);
		}

		///<summary>Delete all MedLabFacAttach objects for the list of MedLabNums and/or list of MedLabResultNums.  Supply either list or both lists and
		///the MedLabFacAttach entries for either list will be deleted.  This could leave MedLabFacility entries not attached
		///to any lab or result, but we won't worry about cleaning those up since the MedLabFacility table will likely always remain very small.</summary>
		public static void DeleteAllForLabsOrResults(List<long> listLabNums,List<long> listResultNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listLabNums,listResultNums);
				return;
			}
			string command="DELETE FROM medlabfacattach "
				+"WHERE MedLabNum IN("+String.Join(",",listLabNums)+") "
				+"OR MedLabResultNum IN("+String.Join(",",listResultNums)+")";
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<MedLabFacAttach> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MedLabFacAttach>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM medlabfacattach WHERE PatNum = "+POut.Long(patNum);
			return Crud.MedLabFacAttachCrud.SelectMany(command);
		}

		///<summary>Gets one MedLabFacAttach from the db.</summary>
		public static MedLabFacAttach GetOne(long medLabFacAttachNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<MedLabFacAttach>(MethodBase.GetCurrentMethod(),medLabFacAttachNum);
			}
			return Crud.MedLabFacAttachCrud.SelectOne(medLabFacAttachNum);
		}

		///<summary></summary>
		public static void Update(MedLabFacAttach medLabFacAttach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medLabFacAttach);
				return;
			}
			Crud.MedLabFacAttachCrud.Update(medLabFacAttach);
		}

		///<summary></summary>
		public static void Delete(long medLabFacAttachNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),medLabFacAttachNum);
				return;
			}
			string command= "DELETE FROM medlabfacattach WHERE MedLabFacAttachNum = "+POut.Long(medLabFacAttachNum);
			Db.NonQ(command);
		}
		*/
	}
}