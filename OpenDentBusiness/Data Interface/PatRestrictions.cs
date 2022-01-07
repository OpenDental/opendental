using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PatRestrictions{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class PatRestrictionCache : CacheListAbs<PatRestriction> {
			protected override List<PatRestriction> GetCacheFromDb() {
				string command="SELECT * FROM PatRestriction ORDER BY ItemOrder";
				return Crud.PatRestrictionCrud.SelectMany(command);
			}
			protected override List<PatRestriction> TableToList(DataTable table) {
				return Crud.PatRestrictionCrud.TableToList(table);
			}
			protected override PatRestriction Copy(PatRestriction PatRestriction) {
				return PatRestriction.Clone();
			}
			protected override DataTable ListToTable(List<PatRestriction> listPatRestrictions) {
				return Crud.PatRestrictionCrud.ListToTable(listPatRestrictions,"PatRestriction");
			}
			protected override void FillCacheIfNeeded() {
				PatRestrictions.GetTableFromCache(false);
			}
			protected override bool IsInListShort(PatRestriction PatRestriction) {
				return !PatRestriction.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PatRestrictionCache _PatRestrictionCache=new PatRestrictionCache();

		///<summary>A list of all PatRestrictions. Returns a deep copy.</summary>
		public static List<PatRestriction> ListDeep {
			get {
				return _PatRestrictionCache.ListDeep;
			}
		}

		///<summary>A list of all visible PatRestrictions. Returns a deep copy.</summary>
		public static List<PatRestriction> ListShortDeep {
			get {
				return _PatRestrictionCache.ListShortDeep;
			}
		}

		///<summary>A list of all PatRestrictions. Returns a shallow copy.</summary>
		public static List<PatRestriction> ListShallow {
			get {
				return _PatRestrictionCache.ListShallow;
			}
		}

		///<summary>A list of all visible PatRestrictions. Returns a shallow copy.</summary>
		public static List<PatRestriction> ListShort {
			get {
				return _PatRestrictionCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_PatRestrictionCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_PatRestrictionCache.FillCacheFromTable(table);
				return table;
			}
			return _PatRestrictionCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/

		///<summary>Gets all patrestrictions for the specified patient.</summary>
		public static List<PatRestriction> GetAllForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PatRestriction>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM patrestriction WHERE PatNum="+POut.Long(patNum);
			return Crud.PatRestrictionCrud.SelectMany(command);
		}

		///<summary>This will only insert a new PatRestriction if there is not already an existing PatRestriction in the db for this patient and type.
		///If exists, returns the PatRestrictionNum of the first one found.  Otherwise returns the PatRestrictionNum of the newly inserted one.</summary>
		public static long Upsert(long patNum,PatRestrict patRestrictType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),patNum,patRestrictType);
			}
			List<PatRestriction> listPatRestricts=GetAllForPat(patNum).FindAll(x => x.PatRestrictType==patRestrictType);
			if(listPatRestricts.Count>0) {
				return listPatRestricts[0].PatRestrictionNum;
			}
			return Crud.PatRestrictionCrud.Insert(new PatRestriction() { PatNum=patNum,PatRestrictType=patRestrictType });
		}

		///<summary>This will only insert a new PatRestriction if there is not already an existing PatRestriction in the db for the family member and type.</summary>
		public static void InsertForFam(Family fam,PatRestrict patRestrictType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fam,patRestrictType);
				return;
			}
			List<long> listFamPatNums=fam.ListPats.Select(x => x.PatNum).ToList();
			string command=$@"SELECT PatNum FROM patrestriction
				WHERE PatNum IN ({string.Join(",",listFamPatNums.Select(x => POut.Long(x)))})
				AND PatRestrictType={POut.Enum<PatRestrict>(patRestrictType)}";
			List<long> listPatsToSkip=Db.GetListLong(command);
			foreach(long patNumCur in listFamPatNums.Where(x => !listPatsToSkip.Contains(x))) {
				Crud.PatRestrictionCrud.Insert(new PatRestriction() { PatNum=patNumCur,PatRestrictType=patRestrictType });
			}
		}

		///<summary>Checks for an existing patrestriction for the specified patient and PatRestrictType.
		///If one exists, returns true (IsRestricted).  If none exist, returns false (!IsRestricted).</summary>
		public static bool IsRestricted(long patNum,PatRestrict patRestrictType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetBool(MethodBase.GetCurrentMethod(),patNum,patRestrictType);
			}
			string command="SELECT COUNT(*) FROM patrestriction WHERE PatNum="+POut.Long(patNum)+" AND PatRestrictType="+POut.Int((int)patRestrictType);
			if(PIn.Int(Db.GetCount(command))>0) {
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>Given a list of PatNums, returns a filtered list of PatNums that are not part of the given RestrictType.</summary>
		public static List<long> GetUnrestrictedPatNumsFromList(List<long> listPatNums,PatRestrict patRestrictType) {
			if(listPatNums.IsNullOrEmpty()) {
				return new List<long>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),listPatNums,patRestrictType);
			}
			string command="SELECT * FROM patrestriction WHERE PatNum IN("+string.Join<long>(",",listPatNums)+") AND PatRestrictType="+POut.Int((int)patRestrictType);
			List<long> listRestrictedPatNums=Crud.PatRestrictionCrud.SelectMany(command).Select(x => x.PatNum).ToList();
			return listPatNums.Except(listRestrictedPatNums).ToList();
		}

		/// <summary>Get a list of all PatNums that are associated with a given restriction type</summary>
		public static List<long> GetAllRestrictedForType(PatRestrict patRestrictType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patRestrictType);
			}
			string command="SELECT * FROM patrestriction WHERE PatRestrictType="+POut.Int((int)patRestrictType);
			return Crud.PatRestrictionCrud.SelectMany(command).Select(x => x.PatNum).ToList();
		}

		///<summary>Gets the human readable description of the patrestriction, passed through Lans.g.
		///Returns empty string if the enum was not found in the switch statement.</summary>
		public static string GetPatRestrictDesc(PatRestrict patRestrictType) {
			switch(patRestrictType) {
				case PatRestrict.ApptSchedule:
					return Lans.g("patRestrictEnum","Appointment Scheduling");
				case PatRestrict.None:
				default:
					return "";
			}
		}

		///<summary>Deletes any patrestrictions for the specified patient and type.</summary>
		public static void RemovePatRestriction(long patNum,PatRestrict patRestrictType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,patRestrictType);
				return;
			}
			string command="DELETE FROM patrestriction WHERE PatNum="+POut.Long(patNum)+" AND PatRestrictType="+POut.Int((int)patRestrictType);
			Db.NonQ(command);
			return;
		}

		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		/*
		///<summary>Gets one PatRestriction from the db.</summary>
		public static PatRestriction GetOne(long patRestrictionNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<PatRestriction>(MethodBase.GetCurrentMethod(),patRestrictionNum);
			}
			return Crud.PatRestrictionCrud.SelectOne(patRestrictionNum);
		}

		///<summary></summary>
		public static void Update(PatRestrict patRestrict){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patRestrict);
				return;
			}
			Crud.PatRestrictionCrud.Update(patRestrict);
		}

		///<summary></summary>
		public static void Delete(long patRestrictionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patRestrictionNum);
				return;
			}
			Crud.PatRestrictionCrud.Delete(patRestrictionNum);
		}		
		*/
	}
}