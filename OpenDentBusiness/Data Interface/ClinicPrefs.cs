using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClinicPrefs{
		#region Cache Pattern

		private class ClinicPrefCache : CacheListAbs<ClinicPref> {
			protected override List<ClinicPref> GetCacheFromDb() {
				string command="SELECT * FROM clinicpref";
				return Crud.ClinicPrefCrud.SelectMany(command);
			}
			protected override List<ClinicPref> TableToList(DataTable table) {
				return Crud.ClinicPrefCrud.TableToList(table);
			}
			protected override ClinicPref Copy(ClinicPref clinicPref) {
				return clinicPref.Clone();
			}
			protected override DataTable ListToTable(List<ClinicPref> listClinicPrefs) {
				return Crud.ClinicPrefCrud.ListToTable(listClinicPrefs,"ClinicPref");
			}
			protected override void FillCacheIfNeeded() {
				ClinicPrefs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ClinicPrefCache _clinicPrefCache=new ClinicPrefCache();

		public static List<ClinicPref> GetWhere(Predicate<ClinicPref> match,bool isShort=false) {
			return _clinicPrefCache.GetWhere(match,isShort);
		}

		private static ClinicPref GetFirstOrDefault(Func<ClinicPref,bool> match,bool isShort=false) {
			return _clinicPrefCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_clinicPrefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_clinicPrefCache.FillCacheFromTable(table);
				return table;
			}
			return _clinicPrefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_clinicPrefCache.ClearCache();
		}
		#endregion Cache Pattern

		///<summary>Gets one ClinicPref from the db.</summary>
		public static ClinicPref GetOne(long clinicPrefNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ClinicPref>(MethodBase.GetCurrentMethod(),clinicPrefNum);
			}
			return Crud.ClinicPrefCrud.SelectOne(clinicPrefNum);
		}

		///<summary></summary>
		public static long Insert(ClinicPref clinicPref){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				clinicPref.ClinicPrefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),clinicPref);
				return clinicPref.ClinicPrefNum;
			}
			return Crud.ClinicPrefCrud.Insert(clinicPref);
		}

		///<summary></summary>
		public static void Update(ClinicPref clinicPref){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicPref);
				return;
			}
			Crud.ClinicPrefCrud.Update(clinicPref);
		}

		public static void Update(ClinicPref clinicPrefNew, ClinicPref clinicPrefOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicPrefNew,clinicPrefOld);
				return;
			}
			Crud.ClinicPrefCrud.Update(clinicPrefNew,clinicPrefOld);
		}

		///<summary></summary>
		public static void Delete(long clinicPrefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clinicPrefNum);
				return;
			}
			Crud.ClinicPrefCrud.Delete(clinicPrefNum);
		}

		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.</summary>
		public static bool Sync(List<ClinicPref> listClinicPrefsNew,List<ClinicPref> listClinicPrefOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listClinicPrefsNew,listClinicPrefOld);
			}
			return Crud.ClinicPrefCrud.Sync(listClinicPrefsNew,listClinicPrefOld);
		}

		///<summary></summary>
		public static List<ClinicPref> GetAllPrefs(long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ClinicPref>>(MethodBase.GetCurrentMethod(),clinicNum);
			}
			string command="SELECT * FROM clinicpref WHERE ClinicNum="+POut.Long(clinicNum);
			return Crud.ClinicPrefCrud.SelectMany(command);
		}

		///<summary>If including default, it will create an extra "clinicpref" with ClinicNum=0, based on the pref.</summary>
		public static List<ClinicPref> GetPrefAllClinics(PrefName prefName,bool includeDefault=false) {
			Meth.NoCheckMiddleTierRole();
			List<ClinicPref> listClinicPrefs=new List<ClinicPref>();
			if(includeDefault){ 
				listClinicPrefs.Add(new ClinicPref() { 
					ClinicNum=0,
					PrefName=prefName,
					ValueString=prefName.GetValueAsText()
				});
			};
			listClinicPrefs.AddRange(GetWhere(x => x.PrefName==prefName));
			return listClinicPrefs;
		}

		///<summary></summary>
		public static ClinicPref GetPref(PrefName prefName,long clinicNum,bool isDefaultIncluded=false) {
			Meth.NoCheckMiddleTierRole();
			return GetPrefAllClinics(prefName,isDefaultIncluded).Find(x => x.ClinicNum==clinicNum);
		}

		///<summary>Gets the ValueString for this clinic's pref or gets the actual preference if it does not exist.</summary>
		public static string GetPrefValue(PrefName prefName,long clinicNum) {
			Meth.NoCheckMiddleTierRole();
			ClinicPref clinicPref=GetPrefAllClinics(prefName).Find(x => x.ClinicNum==clinicNum);
			if(clinicPref==null) {
				return PrefC.GetString(prefName);
			}
			return clinicPref.ValueString;
		}

		///<summary>Update ClinicPrefs that contains a comma-delimited list of DefNums if there are changes.</summary>
		public static void UpdateDefNumsForClinicPref(PrefName prefName,string strDefNumFrom,string strDefNumTo) {
			Meth.NoCheckMiddleTierRole();
			List<ClinicPref> listClinicPrefs=GetPrefAllClinics(prefName);
			for(int i=0;i<listClinicPrefs.Count;i++) {
				List<string> listStrDefNums=GetPrefValue(prefName,listClinicPrefs[i].ClinicNum)
					.Split(",",StringSplitOptions.RemoveEmptyEntries)
					.ToList();
				listStrDefNums=Defs.RemoveOrReplaceDefNum(listStrDefNums,strDefNumFrom,strDefNumTo);
				if(listStrDefNums==null) {
					continue;//Nothing to update.
				}
				string strDefNums=string.Join(",",listStrDefNums.Select(x => POut.String(x)));
				Upsert(prefName,listClinicPrefs[i].ClinicNum,strDefNums);
			}
		}

		///<summary>Returns 0 if there is no clinicpref entry for the specified pref.</summary>
		public static long GetLong(PrefName prefName,long clinicNum) {
			Meth.NoCheckMiddleTierRole();
			ClinicPref clinicPref=GetPref(prefName,clinicNum);
			if(clinicPref==null) {
				return 0;
			}
			long prefNum= PIn.Long(clinicPref.ValueString);
			return prefNum;
		}

		public static int GetInt(PrefName prefName,long clinicNum) {
			Meth.NoCheckMiddleTierRole();
			ClinicPref clinicPref=GetPref(prefName,clinicNum);
			if(clinicPref==null) {
				return 0;
			}
			int prefNum=PIn.Int(clinicPref.ValueString);
			return prefNum;
		}

		///<summary>Gets the ValueString as a boolean for this clinic's pref or gets the actual preference if it does not exist.</summary>
		public static bool GetBool(PrefName prefName,long clinicNum) {
			Meth.NoCheckMiddleTierRole();
			ClinicPref clinicPref=GetPref(prefName,clinicNum);
			if(clinicPref==null) {
				return PrefC.GetBool(prefName);
			}
			return PIn.Bool(clinicPref.ValueString);
		}

		/// <summary>Returns the bool for the specified pref. Explicitly checks if clinics are enabled. If they are, returns the corresponding clinicpref. Else, returns the value from the preference cache.</summary>
		public static bool GetBoolHandleHasClinics(PrefName prefName, long clinicNum) {
			Meth.NoCheckMiddleTierRole();
			if(PrefC.HasClinicsEnabled) {
				return ClinicPrefs.GetBool(prefName,clinicNum);
			}
			bool retVal=PrefC.GetBool(prefName);
			return retVal;
		}

		///<summary>Returns false if no clinic entry found for this pref. Otherwise returns true and value of isSet can be trusted.</summary>
		public static bool TryGetBool(PrefName prefName,long clinicNum,out bool isSet) {
			Meth.NoCheckMiddleTierRole();
			ClinicPref clinicPref=GetPref(prefName,clinicNum);
			if(clinicPref==null) {
				isSet=false;
				return false;
			}
			isSet=PIn.Bool(clinicPref.ValueString);
			return true;
		}

		///<summary>Inserts a pref of type long for the specified clinic.  Throws an exception if the preference already exists.</summary>
		public static void InsertPref(PrefName prefName,long clinicNum,string valueString) {
			Meth.NoCheckMiddleTierRole();
			if(GetFirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName)!=null) {
				throw new ApplicationException("The PrefName "+prefName+" already exists for ClinicNum: "+clinicNum);
			}
			ClinicPref clinicPrefToInsert=new ClinicPref();
			clinicPrefToInsert.PrefName=prefName;
			clinicPrefToInsert.ValueString=valueString;
			clinicPrefToInsert.ClinicNum=clinicNum;
			Insert(clinicPrefToInsert);
		}

		///<summary>Inserts a new clinic pref or updates the existing one.</summary>
		///<returns>True if an insert or update was made, false otherwise.</returns>
		public static bool Upsert(PrefName prefName,long clinicNum,string newValue) {
			Meth.NoCheckMiddleTierRole();
			ClinicPref clinicPref=GetPref(prefName,clinicNum);
			if(clinicPref==null) {
				InsertPref(prefName,clinicNum,newValue);
				return true;
			}
			if(clinicPref.ValueString==newValue) {
				return false;
			}
			clinicPref.ValueString=newValue;
			Update(clinicPref);
			return true;
		}

		///<summary>Deletes the prefs for this clinic. If any pref does not exist, then nothing will be done with that pref.</summary>
		public static long DeletePrefs(long clinicNum,List<PrefName> listPrefNames) {
			if(listPrefNames.IsNullOrEmpty()) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),clinicNum,listPrefNames);
			}
			List<ClinicPref> listClinicPrefs=new List<ClinicPref>();
			for(int i=0;i<listPrefNames.Count;i++){ 
				ClinicPref clinicPref=GetPref(listPrefNames[i],clinicNum);
				if(clinicPref!=null){ 
					listClinicPrefs.Add(clinicPref);
				}
			}
			if(listClinicPrefs.Count==0) {
				return 0;
			}
			string command="DELETE FROM clinicpref WHERE ClinicPrefNum IN("+string.Join(",",listClinicPrefs.Select(x => x.ClinicPrefNum))+")";
			return Db.NonQ(command);
		}

		///<summary>Returns true if ODTouch is allowed for this clinic, false otherwise. 
		///Takes clinics feature on/off into account. Ok to call this when PrefC.HasClinicsEnabled==false.</summary>
		public static bool IsODTouchAllowed(long clinicNum) {
			Meth.NoCheckMiddleTierRole();
			//The office is on limited beta.
			bool isAllowed=ClinicPrefs.GetBoolHandleHasClinics(PrefName.IsODTouchEnabled,clinicNum);
			return isAllowed;
		}
	}
}