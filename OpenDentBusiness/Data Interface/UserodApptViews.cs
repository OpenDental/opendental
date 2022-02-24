using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class UserodApptViews {
		///<summary>Gets the most recent UserodApptView from the db for the user and clinic.  clinicNum can be 0.  Returns null if no match found.</summary>
		public static UserodApptView GetOneForUserAndClinic(long userNum,long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<UserodApptView>(MethodBase.GetCurrentMethod(),userNum,clinicNum);
			}
			string command="SELECT * FROM userodapptview "
				+"WHERE UserNum = "+POut.Long(userNum)+" "
				+"AND ClinicNum = "+POut.Long(clinicNum)+" ";//If clinicNum of 0 passed in, we MUST filter by 0 because that is a valid entry in the db.
			return Crud.UserodApptViewCrud.SelectOne(command);
		}

		public static void InsertOrUpdate(long userNum,long clinicNum,long apptViewNum) {
			//No need to check RemotingRole; no call to db.
			UserodApptView userodApptView=new UserodApptView();
			userodApptView.UserNum=userNum;
			userodApptView.ClinicNum=clinicNum;
			userodApptView.ApptViewNum=apptViewNum;
			//Check if there is already a row in the database for this user, clinic, and apptview.
			UserodApptView userodApptViewDb=GetOneForUserAndClinic(userodApptView.UserNum,userodApptView.ClinicNum);
			if(userodApptViewDb==null) {
				Insert(userodApptView);
			}
			else if(userodApptViewDb.ApptViewNum!=userodApptView.ApptViewNum) {
				userodApptViewDb.ApptViewNum=userodApptView.ApptViewNum;
				Update(userodApptViewDb);
			}
		}

		///<summary></summary>
		public static long Insert(UserodApptView userodApptView) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				userodApptView.UserodApptViewNum=Meth.GetLong(MethodBase.GetCurrentMethod(),userodApptView);
				return userodApptView.UserodApptViewNum;
			}
			return Crud.UserodApptViewCrud.Insert(userodApptView);
		}

		///<summary></summary>
		public static void Update(UserodApptView userodApptView) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userodApptView);
				return;
			}
			Crud.UserodApptViewCrud.Update(userodApptView);
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class UserodApptViewCache : CacheListAbs<UserodApptView> {
			protected override List<UserodApptView> GetCacheFromDb() {
				string command="SELECT * FROM UserodApptView ORDER BY ItemOrder";
				return Crud.UserodApptViewCrud.SelectMany(command);
			}
			protected override List<UserodApptView> TableToList(DataTable table) {
				return Crud.UserodApptViewCrud.TableToList(table);
			}
			protected override UserodApptView Copy(UserodApptView UserodApptView) {
				return UserodApptView.Clone();
			}
			protected override DataTable ListToTable(List<UserodApptView> listUserodApptViews) {
				return Crud.UserodApptViewCrud.ListToTable(listUserodApptViews,"UserodApptView");
			}
			protected override void FillCacheIfNeeded() {
				UserodApptViews.GetTableFromCache(false);
			}
			protected override bool IsInListShort(UserodApptView UserodApptView) {
				return !UserodApptView.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static UserodApptViewCache _UserodApptViewCache=new UserodApptViewCache();

		///<summary>A list of all UserodApptViews. Returns a deep copy.</summary>
		public static List<UserodApptView> ListDeep {
			get {
				return _UserodApptViewCache.ListDeep;
			}
		}

		///<summary>A list of all visible UserodApptViews. Returns a deep copy.</summary>
		public static List<UserodApptView> ListShortDeep {
			get {
				return _UserodApptViewCache.ListShortDeep;
			}
		}

		///<summary>A list of all UserodApptViews. Returns a shallow copy.</summary>
		public static List<UserodApptView> ListShallow {
			get {
				return _UserodApptViewCache.ListShallow;
			}
		}

		///<summary>A list of all visible UserodApptViews. Returns a shallow copy.</summary>
		public static List<UserodApptView> ListShort {
			get {
				return _UserodApptViewCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_UserodApptViewCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_UserodApptViewCache.FillCacheFromTable(table);
				return table;
			}
			return _UserodApptViewCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one UserodApptView from the db.</summary>
		public static UserodApptView GetOne(long userodApptViewNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<UserodApptView>(MethodBase.GetCurrentMethod(),userodApptViewNum);
			}
			return Crud.UserodApptViewCrud.SelectOne(userodApptViewNum);
		}

		///<summary>Gets all recent userodapptviews for the user passed in.  Multiple userodapptviews can be returned when using clinics.</summary>
		public static List<UserodApptView> GetForUser(long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<UserodApptView>>(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT * FROM userodapptview WHERE UserNum = "+POut.Long(userNum);
			return Crud.UserodApptViewCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long userodApptViewNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userodApptViewNum);
				return;
			}
			string command= "DELETE FROM userodapptview WHERE UserodApptViewNum = "+POut.Long(userodApptViewNum);
			Db.NonQ(command);
		}
		*/
	}
}