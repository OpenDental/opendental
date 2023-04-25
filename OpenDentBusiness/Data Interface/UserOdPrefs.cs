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
		public static bool Sync(List<UserOdPref> listNew,List<UserOdPref> listOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.UserOdPrefCrud.Sync(listNew,listOld);
		}
		#endregion

		#region CachePattern
		private class UserOdPrefCache : CacheListAbs<UserOdPref> {
			protected override List<UserOdPref> GetCacheFromDb() {
				string command="SELECT * FROM userodpref";
				return Crud.UserOdPrefCrud.SelectMany(command);
			}
			protected override List<UserOdPref> TableToList(DataTable table) {
				return Crud.UserOdPrefCrud.TableToList(table);
			}
			protected override UserOdPref Copy(UserOdPref userOdPref) {
				return userOdPref.Clone();
			}
			protected override DataTable ListToTable(List<UserOdPref> listUserOdPrefs) {
				return Crud.UserOdPrefCrud.ListToTable(listUserOdPrefs,"UserOdPref");
			}
			protected override void FillCacheIfNeeded() {
				UserOdPrefs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserOdPrefCache _userOdPrefCache=new UserOdPrefCache();

		///<summary>A list of all UserOdPrefs. Returns a deep copy.</summary>
		public static List<UserOdPref> GetDeepCopy(bool isShort=false) {
			return _userOdPrefCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _userOdPrefCache.GetCount(isShort);
		}

		public static UserOdPref GetFirst(bool isShort=false) {
			return _userOdPrefCache.GetFirst(isShort);
		}

		public static UserOdPref GetFirstOrDefault(Func<UserOdPref,bool> match,bool isShort=false) {
			return _userOdPrefCache.GetFirstOrDefault(match,isShort);
		}

		public static List<UserOdPref> GetWhere(Predicate<UserOdPref> match,bool isShort=false) {
			return _userOdPrefCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_userOdPrefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_userOdPrefCache.FillCacheFromTable(table);
				return table;
			}
			return _userOdPrefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_userOdPrefCache.ClearCache();
		}
		#endregion
		///<summary></summary>
		public static bool Update(UserOdPref userOdPref,UserOdPref userOdPrefOld=null){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetBool(MethodBase.GetCurrentMethod(),userOdPref,userOdPrefOld);
			}
			if(userOdPrefOld is null) {
				Crud.UserOdPrefCrud.Update(userOdPref);
				return true;
			}
			else {
				return Crud.UserOdPrefCrud.Update(userOdPref,userOdPrefOld);
			}
		}

		///<summary></summary>
		public static long Insert(UserOdPref userOdPref) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				userOdPref.UserOdPrefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userOdPref);
				return userOdPref.UserOdPrefNum;
			}
			return Crud.UserOdPrefCrud.Insert(userOdPref);
		}

		public static void InsertMany(List<UserOdPref> listUserOdPref) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listUserOdPref);
				return;
			}
			Crud.UserOdPrefCrud.InsertMany(listUserOdPref);
		}

		///<summary></summary>
		public static void Upsert(List<UserOdPref> listUserOdPrefs) {
			//This remoting role isn't necessary but helps significantly with speed due to looping.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listUserOdPrefs);
				return;
			}
			for(int i=0;i<listUserOdPrefs.Count;i++) {
				Upsert(listUserOdPrefs[i]);
			}
		}

		///<summary></summary>
		public static long Upsert(UserOdPref userOdPref) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userOdPrefNum);
				return;
			}
			Crud.UserOdPrefCrud.Delete(userOdPrefNum);
		}

		///<summary>This will ensure that when inserting this preference that there are no other preferences that are of the same fkey/fkeytype/user combination.
		///This will likely only be used in specific scenarios where there is only 1 userodpref for the fkey/fkeytype passed in.
		///To use this method with multiple userodprefs, you must make ValueString contain a JSON object or equivalent complex document.</summary>
		public static void DeleteMany(long userNum,long fkey,UserOdFkeyType fkeyType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userNum,fkey,fkeyType);
				return;
			}
			//Delete any userodpref rows that match its usernum/fkey/fkeytype.
			string command="DELETE FROM userodpref WHERE UserNum="+POut.Long(userNum)
				+" AND FkeyType="+POut.Int((int)fkeyType)
				+" AND Fkey="+POut.Long(fkey);
			Db.NonQ(command);
		}

		public static void DeleteManyForUserAndFkeyType(long userNum,UserOdFkeyType fkeyType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userNum,fkeyType);
				return;
			}
			//Delete any userodpref rows that match its usernum/fkeytype.
			string command="DELETE FROM userodpref WHERE UserNum="+POut.Long(userNum)
				+" AND FkeyType="+POut.Int((int)fkeyType);
			Db.NonQ(command);
		}

		public static List<UserOdPref> GetByUserAndFkeyType(long userNum,UserOdFkeyType fkeyType) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.UserNum==userNum && x.FkeyType==fkeyType);
		}

		public static List<UserOdPref> GetByFkeyAndFkeyType(long fkey,UserOdFkeyType fkeyType) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.Fkey==fkey && x.FkeyType==fkeyType);
		}

		public static List<UserOdPref> GetByUserFkeyAndFkeyType(long userNum,long fkey,UserOdFkeyType fkeyType) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.UserNum==userNum && x.Fkey==fkey && x.FkeyType==fkeyType);
		}

		///<summary>Creates a new UserOdPref if the fkeyType does not exist for the userNum.</summary>
		public static UserOdPref GetFirstOrNewByUserAndFkeyType(long userNum,UserOdFkeyType fkeyType) {
			//No need to check MiddleTierRole; no call to db.
			UserOdPref userOdPref=GetFirstOrDefault(x => x.UserNum==userNum && x.FkeyType==fkeyType);
			//Create a new instance if db value does not exist.
			if(userOdPref==null) {
				userOdPref=new UserOdPref() {
					IsNew=true,
					UserOdPrefNum=0,
					Fkey=0,
					FkeyType=fkeyType,
					UserNum=userNum,
					ValueString="",
					ClinicNum=0,
				};
			}
			return userOdPref;
		}

		///<summary>Will return a list of UserOdPrefs corresponding to the usernum/fkey/fkeytype combination given.</summary>
		public static List<UserOdPref> GetByUserAndFkeyAndFkeyType(long userNum,long fkey,UserOdFkeyType fkeyType,List<long> listClinicNums=null) {
			//No need to check MiddleTierRole; no call to db.
			List<UserOdPref> listUserOdPrefs=GetWhere(x => x.UserNum==userNum && x.Fkey==fkey && x.FkeyType==fkeyType);
			if(!listClinicNums.IsNullOrEmpty()) {
				listUserOdPrefs=listUserOdPrefs.Where(x => listClinicNums.Contains(x.ClinicNum)).ToList();
			}
			return listUserOdPrefs;
		}

		///<summary>Will return the UserOdPref corresponding to the usernum/fkey/fkeytype/ClinicNum composite key given.</summary>
		public static UserOdPref GetByCompositeKey(long userNum,long fkey,UserOdFkeyType fkeyType,long clinicNum=0) {
			//No need to check MiddleTierRole; no call to db.
			UserOdPref userOdPref=GetFirstOrDefault(x => 
				x.UserNum==userNum
				&& x.Fkey==fkey 
				&& x.FkeyType==fkeyType
				&& x.ClinicNum==clinicNum);
			//Create a new instance if db value does not exist.
			if(userOdPref==null) {
				userOdPref=new UserOdPref() {
					IsNew=true,
					UserOdPrefNum=0,
					Fkey=fkey,
					FkeyType=fkeyType,
					UserNum=userNum,
					ValueString="",
					ClinicNum=clinicNum,
				};
			}
			return userOdPref;
		}
	
		///<summary>Returns a list of UserOdPrefs for the given UserOdFkeyType.</summary>
		public static List<UserOdPref> GetByFkeyType(UserOdFkeyType fkeyType) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.FkeyType==fkeyType);
		}

		public static List<UserOdPref> GetAllByFkeyAndFkeyType(long fkey,UserOdFkeyType fkeyType) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.Fkey==fkey && x.FkeyType==fkeyType);
		}

		///<summary>Deletes UserOdPref with provided parameters.  If "userNum" is 0 then will delete all UserOdPref's with corresponding fkeyType and fkey.</summary>
		public static void DeleteForFkey(long userNum,UserOdFkeyType fkeyType,long fkey) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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