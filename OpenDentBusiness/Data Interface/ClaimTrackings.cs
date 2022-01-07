using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ClaimTrackings{
		#region Get Methods
		///<summary>Gets one ClaimTracking from the db.</summary>
		public static ClaimTracking GetOne(long claimTrackingNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ClaimTracking>(MethodBase.GetCurrentMethod(),claimTrackingNum);
			}
			return Crud.ClaimTrackingCrud.SelectOne(claimTrackingNum);
		}

		///<summary></summary>
		public static List<ClaimTracking> Refresh(long usernum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimTracking>>(MethodBase.GetCurrentMethod(),usernum);
			}
			string command="SELECT * FROM claimtracking WHERE UserNum = "+POut.Long(usernum);
			return Crud.ClaimTrackingCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<ClaimTracking> RefreshForUsers(ClaimTrackingType type,List<long> listUserNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimTracking>>(MethodBase.GetCurrentMethod(),type,listUserNums);
			}
			if(listUserNums==null || listUserNums.Count==0) {
				return new List<ClaimTracking>();
			}
			string command="SELECT * FROM claimtracking WHERE TrackingType='"+POut.String(type.ToString())+"' "
				+"AND UserNum IN ("+String.Join(",",listUserNums)+")";
			return Crud.ClaimTrackingCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<ClaimTracking> RefreshForClaim(ClaimTrackingType type,long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimTracking>>(MethodBase.GetCurrentMethod(),type,claimNum);
			}
			if(claimNum==0) {
				return new List<ClaimTracking>();
			}
			string command="SELECT * FROM claimtracking WHERE TrackingType='"+POut.String(type.ToString())+"' "
				+"AND ClaimNum="+POut.Long(claimNum);
			return Crud.ClaimTrackingCrud.SelectMany(command);
		}

		///<summary>Given a claimNum, this function returns all CustomTracking items for that claim regardless of type.</summary>
		public static List<ClaimTracking> GetForClaim(long claimNum) {
			if(claimNum==0) {
				return new List<ClaimTracking>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimTracking>>(MethodBase.GetCurrentMethod(),claimNum);
			}
			string command="SELECT * FROM claimtracking WHERE ClaimNum="+POut.Long(claimNum);
			return Crud.ClaimTrackingCrud.SelectMany(command);
		}

		#endregion
		
		#region Insert
		///<summary></summary>
		public static long Insert(ClaimTracking claimTracking){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				claimTracking.ClaimTrackingNum=Meth.GetLong(MethodBase.GetCurrentMethod(),claimTracking);
				return claimTracking.ClaimTrackingNum;
			}
			return Crud.ClaimTrackingCrud.Insert(claimTracking);
		}

		public static long InsertClaimProcReceived(long claimNum,long userNum,string note="") {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetLong(MethodBase.GetCurrentMethod(),claimNum,userNum,note);
			}
			string command="SELECT COUNT(*) FROM claimtracking WHERE TrackingType='"+POut.String(ClaimTrackingType.ClaimProcReceived.ToString())
				+"' AND ClaimNum="+POut.Long(claimNum)+" AND UserNum='"+userNum+"'";
			if(Db.GetCount(command)!="0") {
				return 0;//Do nothing.
			}
			ClaimTracking claimTracking=new ClaimTracking();
			claimTracking.TrackingType=ClaimTrackingType.ClaimProcReceived;
			claimTracking.ClaimNum=claimNum;
			claimTracking.UserNum=userNum;
			claimTracking.Note=note;
			return Crud.ClaimTrackingCrud.Insert(claimTracking);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(ClaimTracking claimTracking){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimTracking);
				return;
			}
			Crud.ClaimTrackingCrud.Update(claimTracking);
		}

		///<summary></summary>
		public static void Sync(List<ClaimTracking> listNew,List<ClaimTracking> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,listOld);
				return;
			}
			Crud.ClaimTrackingCrud.Sync(listNew,listOld);
		}
		#endregion

		#region Delete
		///<summary></summary>
		public static void Delete(long claimTrackingNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimTrackingNum);
				return;
			}
			Crud.ClaimTrackingCrud.Delete(claimTrackingNum);
		}
		#endregion

		#region Misc Methods
		///<summary>Attempts to create or update ClaimTrackings and calls sync to update the database at the end.
		///Will update ClaimTracking if one has been inserted for a given claim that did not have one prior to calling this method.
		///When called please ensure dictClaimTracking has entries.</summary>
		public static List<ClaimTracking> Assign(List<ODTuple<long,long>>listTrackingNumsAndClaimNums,long assignUserNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimTracking>>(MethodBase.GetCurrentMethod(),listTrackingNumsAndClaimNums,assignUserNum);
			}
			string command="SELECT * FROM claimtracking WHERE claimtracking.TrackingType='"+POut.String(ClaimTrackingType.ClaimUser.ToString())+"' "
				+"AND claimtracking.ClaimNum IN("+string.Join(",",listTrackingNumsAndClaimNums.Select(x => x.Item2).ToList())+")";
			List<ClaimTracking> listClaimTrackingDb=Crud.ClaimTrackingCrud.SelectMany(command);//up to date copy from the database
			List<ClaimTracking> listClaimTrackingNew=listClaimTrackingDb.Select(x => x.Copy()).ToList();
			foreach(Tuple<long,long> claimTrackingEntry in listTrackingNumsAndClaimNums) {//Item1=>claim tracking num & Item2=>claim num
				ClaimTracking claimTracking=new ClaimTracking();
				if(claimTrackingEntry.Item1==0 //Given claim did not have an existing ClaimTracking when dictClaimTracking was constructed.
					&& !listClaimTrackingDb.Exists(x => x.ClaimNum==claimTrackingEntry.Item2))//DB does not contain ClaimTracking row for this claimNum.
				{
					if(assignUserNum==0) {
						continue;
					}
					claimTracking.UserNum=assignUserNum;
					claimTracking.ClaimNum=claimTrackingEntry.Item2;//dict value is ClaimNum
					claimTracking.TrackingType=ClaimTrackingType.ClaimUser;
					listClaimTrackingNew.Add(claimTracking);
				}
				else {
					if(claimTrackingEntry.Item1==0) {//claim tracking did not originally exist but someone modified while we were here and it exists in the database now.
						claimTracking=listClaimTrackingNew.FirstOrDefault(x => x.ClaimNum==claimTrackingEntry.Item2);
						claimTracking.UserNum=assignUserNum;
					}
					else {//claim tracking already exsisted in the db for this claim
						claimTracking=listClaimTrackingNew.FirstOrDefault(x => x.ClaimTrackingNum==claimTrackingEntry.Item1);
						if(claimTracking==null) {//ClaimTracking existed when method called but has been removed since.
							if(assignUserNum==0) {
								continue;//ClaimTracking was already removed for us.
							}
							claimTracking=new ClaimTracking();
							claimTracking.UserNum=assignUserNum;
							claimTracking.ClaimNum=claimTrackingEntry.Item2;//dict value is ClaimNum
							claimTracking.TrackingType=ClaimTrackingType.ClaimUser;
							listClaimTrackingNew.Add(claimTracking);
						}
						if(assignUserNum==0) {
							listClaimTrackingNew.Remove(claimTracking);
						}
						else {
							claimTracking.UserNum=assignUserNum;
						}
					}
				}
			}
			ClaimTrackings.Sync(listClaimTrackingNew,listClaimTrackingDb);
			return listClaimTrackingNew;
		}

		///<summary>Supplied two claims, this function will make new copies of the custom trackings for claimOrig, attach them to claimDest, and insert them. </summary>
		public static void CopyToClaim(long claimOrigNum,long claimDestNum) {
			GetForClaim(claimOrigNum).ForEach(x => {
				x.ClaimNum=claimDestNum;
				x.Note=$"Split claim original entry timestamp: {x.DateTimeEntry.ToString()}\r\n{x.Note}";
				Insert(x);
			});
		}
		#endregion

		/*
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		#region CachePattern

		private class ClaimTrackingCache : CacheListAbs<ClaimTracking> {
			protected override List<ClaimTracking> GetCacheFromDb() {
				string command="SELECT * FROM ClaimTracking ORDER BY ItemOrder";
				return Crud.ClaimTrackingCrud.SelectMany(command);
			}
			protected override List<ClaimTracking> TableToList(DataTable table) {
				return Crud.ClaimTrackingCrud.TableToList(table);
			}
			protected override ClaimTracking Copy(ClaimTracking ClaimTracking) {
				return ClaimTracking.Clone();
			}
			protected override DataTable ListToTable(List<ClaimTracking> listClaimTrackings) {
				return Crud.ClaimTrackingCrud.ListToTable(listClaimTrackings,"ClaimTracking");
			}
			protected override void FillCacheIfNeeded() {
				ClaimTrackings.GetTableFromCache(false);
			}
			protected override bool IsInListShort(ClaimTracking ClaimTracking) {
				return !ClaimTracking.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ClaimTrackingCache _ClaimTrackingCache=new ClaimTrackingCache();

		///<summary>A list of all ClaimTrackings. Returns a deep copy.</summary>
		public static List<ClaimTracking> ListDeep {
			get {
				return _ClaimTrackingCache.ListDeep;
			}
		}

		///<summary>A list of all visible ClaimTrackings. Returns a deep copy.</summary>
		public static List<ClaimTracking> ListShortDeep {
			get {
				return _ClaimTrackingCache.ListShortDeep;
			}
		}

		///<summary>A list of all ClaimTrackings. Returns a shallow copy.</summary>
		public static List<ClaimTracking> ListShallow {
			get {
				return _ClaimTrackingCache.ListShallow;
			}
		}

		///<summary>A list of all visible ClaimTrackings. Returns a shallow copy.</summary>
		public static List<ClaimTracking> ListShort {
			get {
				return _ClaimTrackingCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ClaimTrackingCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ClaimTrackingCache.FillCacheFromTable(table);
				return table;
			}
			return _ClaimTrackingCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
	}
}