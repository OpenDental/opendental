using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class UserOdPrefs {
		#region Update
		///<summary></summary>
		public static void Sync(List<UserOdPref> listNew,List<UserOdPref> listDB){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,listDB);
				return;
			}
			Crud.UserOdPrefCrud.Sync(listNew,listDB);
		}
		#endregion

		/*
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		#region CachePattern

		private class UserOdPrefCache : CacheListAbs<UserOdPref> {
			protected override List<UserOdPref> GetCacheFromDb() {
				string command="SELECT * FROM UserOdPref ORDER BY ItemOrder";
				return Crud.UserOdPrefCrud.SelectMany(command);
			}
			protected override List<UserOdPref> TableToList(DataTable table) {
				return Crud.UserOdPrefCrud.TableToList(table);
			}
			protected override UserOdPref Copy(UserOdPref UserOdPref) {
				return UserOdPref.Clone();
			}
			protected override DataTable ListToTable(List<UserOdPref> listUserOdPrefs) {
				return Crud.UserOdPrefCrud.ListToTable(listUserOdPrefs,"UserOdPref");
			}
			protected override void FillCacheIfNeeded() {
				UserOdPrefs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(UserOdPref UserOdPref) {
				return !UserOdPref.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserOdPrefCache _UserOdPrefCache=new UserOdPrefCache();

		///<summary>A list of all UserOdPrefs. Returns a deep copy.</summary>
		public static List<UserOdPref> ListDeep {
			get {
				return _UserOdPrefCache.ListDeep;
			}
		}

		///<summary>A list of all visible UserOdPrefs. Returns a deep copy.</summary>
		public static List<UserOdPref> ListShortDeep {
			get {
				return _UserOdPrefCache.ListShortDeep;
			}
		}

		///<summary>A list of all UserOdPrefs. Returns a shallow copy.</summary>
		public static List<UserOdPref> ListShallow {
			get {
				return _UserOdPrefCache.ListShallow;
			}
		}

		///<summary>A list of all visible UserOdPrefs. Returns a shallow copy.</summary>
		public static List<UserOdPref> ListShort {
			get {
				return _UserOdPrefCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_UserOdPrefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_UserOdPrefCache.FillCacheFromTable(table);
				return table;
			}
			return _UserOdPrefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		///<summary></summary>
		public static List<UserOdPref> Refresh(long userNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT * FROM userodpref WHERE UserNum = "+POut.Long(userNum);
			return Crud.UserOdPrefCrud.SelectMany(command);
		}
		
		///<summary>Gets one UserOdPref from the db.</summary>
		public static UserOdPref GetOne(long userOdPrefNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<UserOdPref>(MethodBase.GetCurrentMethod(),userOdPrefNum);
			}
			return Crud.UserOdPrefCrud.SelectOne(userOdPrefNum);
		}
		*/

		///<summary></summary>
		public static void Update(UserOdPref userOdPref){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userOdPref);
				return;
			}
			Crud.UserOdPrefCrud.Update(userOdPref);
		}

		///<summary></summary>
		public static long Insert(UserOdPref userOdPref) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				userOdPref.UserOdPrefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userOdPref);
				return userOdPref.UserOdPrefNum;
			}
			return Crud.UserOdPrefCrud.Insert(userOdPref);
		}

		///<summary></summary>
		public static void Upsert(List<UserOdPref> listUserOdPrefs) {
			//This remoting role isn't necessary but helps significantly with speed due to looping.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listUserOdPrefs);
				return;
			}
			foreach(UserOdPref userOdPref in listUserOdPrefs) {
				Upsert(userOdPref);
			}
		}

		///<summary></summary>
		public static long Upsert(UserOdPref userOdPref) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				userOdPref.UserOdPrefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userOdPref);
				return userOdPref.UserOdPrefNum;
			}
			if(userOdPref.UserOdPrefNum==0) {
				return Crud.UserOdPrefCrud.Insert(userOdPref);
			}
			Crud.UserOdPrefCrud.Update(userOdPref);
			return userOdPref.UserOdPrefNum;
		}

		///<summary></summary>
		public static void Delete(long userOdPrefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userOdPrefNum);
				return;
			}
			Crud.UserOdPrefCrud.Delete(userOdPrefNum);
		}

		///<summary>This will ensure that when inserting this preference that there are no other preferences that are of the same fkey/fkeytype/user combination.
		///This will likely only be used in specific scenarios where there is only 1 userodpref for the fkey/fkeytype passed in.
		///To use this method with multiple userodprefs, you must make ValueString contain a JSON object or equivalent complex document.</summary>
		public static void DeleteMany(long userNum,long fkey,UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userNum,fkey,fkeyType);
				return;
			}
			//Delete any userodpref rows that match its usernum/fkey/fkeytype.
			string command="DELETE FROM userodpref WHERE UserNum="+POut.Long(userNum)
				+" AND FkeyType="+POut.Int((int)fkeyType)
				+" AND Fkey="+POut.Long(fkey);
			Db.NonQ(command);
		}

		public static List<UserOdPref> GetByUserAndFkeyType(long userNum,UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),userNum,fkeyType);
			}
			string command = "SELECT * FROM userodpref WHERE UserNum="+POut.Long(userNum)+" AND FkeyType="+POut.Int((int)fkeyType);
			return Crud.UserOdPrefCrud.SelectMany(command);
		}

		public static List<UserOdPref> GetByFkeyAndFkeyType(long fkey,UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),fkey,fkeyType);
			}
			string command = "SELECT * FROM userodpref WHERE Fkey="+POut.Long(fkey)+" AND FkeyType="+POut.Int((int)fkeyType);
			return Crud.UserOdPrefCrud.SelectMany(command);
		}

		public static List<UserOdPref> GetByUserFkeyAndFkeyType(long userNum,long fkey,UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),userNum,fkey,fkeyType);
			}
			string command="SELECT * FROM userodpref WHERE UserNum="+POut.Long(userNum)+" AND Fkey="+POut.Long(fkey)
				+" AND FkeyType="+POut.Int((int)fkeyType);
			return Crud.UserOdPrefCrud.SelectMany(command);
		}

		public static UserOdPref GetFirstOrNewByUserAndFkeyType(long userNum,UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<UserOdPref>(MethodBase.GetCurrentMethod(),userNum,fkeyType);
			}
			string command = "SELECT * FROM userodpref WHERE UserNum="+POut.Long(userNum)+" AND FkeyType="+POut.Int((int)fkeyType)+" LIMIT 1";
			return ODMethodsT.Coalesce(
				//Get the db value if it exists.
				Crud.UserOdPrefCrud.SelectOne(command),
				//Create a new instance if db value does not exist.
				new UserOdPref() {
					IsNew=true,
					UserOdPrefNum=0,
					Fkey=0,
					FkeyType=fkeyType,
					UserNum=userNum,
					ValueString="",
					ClinicNum=0,
				});
		}

		///<summary>Will return a list of UserOdPrefs corresponding to the usernum/fkey/fkeytype combination given.</summary>
		public static List<UserOdPref> GetByUserAndFkeyAndFkeyType(long userNum,long fkey,UserOdFkeyType fkeyType,List<long> listClinicNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),userNum,fkey,fkeyType,listClinicNums);
			}
			string command="SELECT * FROM userodpref WHERE UserNum="+POut.Long(userNum)
				+" AND Fkey="+POut.Long(fkey)+" AND FkeyType="+POut.Int((int)fkeyType);
			if(listClinicNums!=null && listClinicNums.Count>0) {
				command+=" AND ClinicNum IN("+String.Join(", ",listClinicNums)+") ";
			}
			return Crud.UserOdPrefCrud.SelectMany(command);
		}

		///<summary>Will return the UserOdPref corresponding to the usernum/fkey/fkeytype/ClinicNum composite key given.</summary>
		public static UserOdPref GetByCompositeKey(long userNum,long fkey,UserOdFkeyType fkeyType,long clinicNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<UserOdPref>(MethodBase.GetCurrentMethod(),userNum,fkey,fkeyType,clinicNum);
			}
			string command="SELECT * FROM userodpref WHERE UserNum="+POut.Long(userNum)
				+" AND Fkey="+POut.Long(fkey)+" AND FkeyType="+POut.Int((int)fkeyType)
				+" AND ClinicNum="+POut.Long(clinicNum);
			return ODMethodsT.Coalesce(
				//Get the db value if it exists.
				Crud.UserOdPrefCrud.SelectOne(command),
				//Create a new instance if db value does not exist.
				new UserOdPref() {
					IsNew=true,
					UserOdPrefNum=0,
					Fkey=fkey,
					FkeyType=fkeyType,
					UserNum=userNum,
					ValueString="",
					ClinicNum=clinicNum,
				});
		}
	
		///<summary>Returns a list of UserOdPrefs for the given UserOdFkeyType.</summary>
		public static List<UserOdPref> GetByFkeyType(UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),fkeyType);
			}
			string command = "SELECT * FROM userodpref WHERE FkeyType="+POut.Int((int)fkeyType);
			return Crud.UserOdPrefCrud.SelectMany(command);
		}

		public static List<UserOdPref> GetAllByFkeyAndFkeyType(long fkey,UserOdFkeyType fkeyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserOdPref>>(MethodBase.GetCurrentMethod(),fkey,fkeyType);
			}
			string command="SELECT * FROM userodpref WHERE Fkey="+POut.Long(fkey)+" AND FkeyType="+POut.Int((int)fkeyType);
			return Crud.UserOdPrefCrud.SelectMany(command);
		}

		///<summary>Deletes UserOdPref with provided parameters.  If "userNum" is 0 then will delete all UserOdPref's with corresponding fkeyType and fkey.</summary>
		public static void DeleteForFkey(long userNum,UserOdFkeyType fkeyType,long fkey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userNum,fkeyType,fkey);
				return;
			}
			string command = "DELETE FROM userodpref "
				+"WHERE Fkey="+POut.Long(fkey)+" AND FkeyType="+POut.Int((int)fkeyType);
			if(userNum!=0) {
				command+=" AND UserNum="+POut.Long(userNum);
			}
			Db.NonQ(command);
		}

		///<summary>Deletes UserOdPref with provided parameters.
		///If "userNum" is 0 then will delete all UserOdPref's with corresponding fkeyType and valueString.</summary>
		public static void DeleteForValueString(long userNum,UserOdFkeyType fkeyType,string valueString) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userNum,fkeyType,valueString);
				return;
			}
			string command = "DELETE FROM userodpref "
				+"WHERE ValueString='"+POut.String(valueString)+"' AND FkeyType="+POut.Int((int)fkeyType);
			if(userNum!=0) {
				command+=" AND UserNum="+POut.Long(userNum);
			}
			Db.NonQ(command);
		}
	}

}