using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ApiSubscriptions{
		#region Methods - Get
		
		///<summary>Gets one ApiSubscription from the db.</summary>
		public static ApiSubscription GetOne(long apiSubscriptionNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ApiSubscription>(MethodBase.GetCurrentMethod(),apiSubscriptionNum);
			}
			return Crud.ApiSubscriptionCrud.SelectOne(apiSubscriptionNum);
		}

		///<summary>Gets one ApiSubscription from the db.</summary>
		public static ApiSubscription GetOne(string customerKey,string endPoint,string workstation,EnumWatchTable watchTable){//,string watchColumn){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<ApiSubscription>(MethodBase.GetCurrentMethod(),customerKey,endPoint,workstation,watchTable);
			}
			string command="SELECT * FROM apisubscription "+
				"WHERE CustomerKey='"+POut.String(customerKey)+"' "+
				"AND EndPointUrl='"+POut.String(endPoint)+"' "+
				"AND Workstation='"+POut.String(workstation)+"' "+
				"AND WatchTable="+POut.Enum(watchTable)+" "+
				//"AND WatchColumn='"+POut.String(watchColumn)+"' "+
				"AND (DateTimeStop='0001-01-01' OR "+POut.Date(DateTime.Now)+"<=DateTimeStop)";
			return Crud.ApiSubscriptionCrud.SelectOne(command);
		}

		///<summary>Gets all subscriptions for a given Customer API Key. </summary>
		public static List<ApiSubscription> GetAllForCustomerKey(string customerKey){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<ApiSubscription>>(MethodBase.GetCurrentMethod(),customerKey);
			}
			string command="SELECT * FROM apisubscription WHERE CustomerKey='"+POut.String(customerKey)+"'";
			return Crud.ApiSubscriptionCrud.SelectMany(command);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(ApiSubscription apiSubscription){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				apiSubscription.ApiSubscriptionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apiSubscription);
				return apiSubscription.ApiSubscriptionNum;
			}
			return Crud.ApiSubscriptionCrud.Insert(apiSubscription);
		}
		///<summary></summary>
		public static void Update(ApiSubscription apiSubscription){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apiSubscription);
				return;
			}
			Crud.ApiSubscriptionCrud.Update(apiSubscription);
		}
		///<summary></summary>
		public static void Delete(long apiSubscriptionNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apiSubscriptionNum);
				return;
			}
			Crud.ApiSubscriptionCrud.Delete(apiSubscriptionNum);
		}

		#endregion Methods - Modify

		#region Methods - Misc

		///<summary>Returns true if there is already a subscription with the same parameters for a Customer's API Key, otherwise false. </summary>
		public static bool IsExistingSubscription(ApiSubscription apiSubscription){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetBool(MethodBase.GetCurrentMethod(),apiSubscription);
			}
			string command="SELECT COUNT(*) FROM apisubscription "+
				"WHERE CustomerKey='"+POut.String(apiSubscription.CustomerKey)+"' "+
				"AND EndPointUrl='"+POut.String(apiSubscription.EndPointUrl)+"' "+
				"AND Workstation='"+POut.String(apiSubscription.Workstation)+"' "+
				"AND WatchTable='"+POut.String(apiSubscription.WatchTable.ToString())+"' "+
				"AND UiEventType='"+POut.String(apiSubscription.UiEventType.ToString())+"' "+
				"AND (DateTimeStop='0001-01-01' OR "+POut.Date(DateTime.Now)+"<=DateTimeStop)";
			return Db.GetScalar(command)!="0";  //Anything other than zero means a subscription already exists
		}

		#endregion Methods - Misc

		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ApiSubscriptionCache : CacheListAbs<ApiSubscription> {
			protected override List<ApiSubscription> GetCacheFromDb() {
				string command="SELECT * FROM apisubscription";
				return Crud.ApiSubscriptionCrud.SelectMany(command);
			}
			protected override List<ApiSubscription> TableToList(DataTable table) {
				return Crud.ApiSubscriptionCrud.TableToList(table);
			}
			protected override ApiSubscription Copy(ApiSubscription apiSubscription) {
				return apiSubscription.Copy();
			}
			protected override DataTable ListToTable(List<ApiSubscription> listApiSubscriptions) {
				return Crud.ApiSubscriptionCrud.ListToTable(listApiSubscriptions,"ApiSubscription");
			}
			protected override void FillCacheIfNeeded() {
				ApiSubscriptions.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ApiSubscriptionCache _apiSubscriptionCache=new ApiSubscriptionCache();

		public static List<ApiSubscription> GetDeepCopy(bool isShort=false) {
			return _apiSubscriptionCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _apiSubscriptionCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ApiSubscription> match,bool isShort=false) {
			return _apiSubscriptionCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ApiSubscription> match,bool isShort=false) {
			return _apiSubscriptionCache.GetFindIndex(match,isShort);
		}

		public static ApiSubscription GetFirst(bool isShort=false) {
			return _apiSubscriptionCache.GetFirst(isShort);
		}

		public static ApiSubscription GetFirst(Func<ApiSubscription,bool> match,bool isShort=false) {
			return _apiSubscriptionCache.GetFirst(match,isShort);
		}

		public static ApiSubscription GetFirstOrDefault(Func<ApiSubscription,bool> match,bool isShort=false) {
			return _apiSubscriptionCache.GetFirstOrDefault(match,isShort);
		}

		public static ApiSubscription GetLast(bool isShort=false) {
			return _apiSubscriptionCache.GetLast(isShort);
		}

		public static ApiSubscription GetLastOrDefault(Func<ApiSubscription,bool> match,bool isShort=false) {
			return _apiSubscriptionCache.GetLastOrDefault(match,isShort);
		}

		public static List<ApiSubscription> GetWhere(Predicate<ApiSubscription> match,bool isShort=false) {
			return _apiSubscriptionCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_apiSubscriptionCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_apiSubscriptionCache.FillCacheFromTable(table);
				return table;
			}
			return _apiSubscriptionCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern		

		///<summary></summary>
		public static List<ApiSubscription> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ApiSubscription>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM apisubscription";
			return Crud.ApiSubscriptionCrud.SelectMany(command);
		}

		public static void UpdateSubscriptionTime(long apiSubscriptionNum,DateTime dateTimeStart){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apiSubscriptionNum,dateTimeStart);
				return;
			}
			string command="UPDATE apisubscription "
				+"SET DateTimeStart="+POut.DateT(dateTimeStart)+" "
				+"WHERE ApiSubscriptionNum="+POut.Long(apiSubscriptionNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<Appointment> GetApptsForSubscription(DateTime dateTimeStart,DateTime dateTimeStop){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Appointment>>(MethodBase.GetCurrentMethod(),dateTimeStart,dateTimeStop);
			}
			string command="SELECT * FROM appointment "
				+"WHERE DateTStamp >= "+POut.DateT(dateTimeStart)+" "
				+"AND DateTStamp < "+POut.DateT(dateTimeStop);
			//We use a rigorous stop time so that we don't get any duplicates or misses at the edges of the time range.
			List<Appointment> listAppointments=Crud.AppointmentCrud.SelectMany(command);
			return listAppointments;
		}
		
		///<summary></summary>
		public static List<Patient> GetPatsForSubscription(DateTime dateTimeStart,DateTime dateTimeStop){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Patient>>(MethodBase.GetCurrentMethod(),dateTimeStart,dateTimeStop);
			}
			string command="SELECT * FROM patient "
				+"WHERE DateTStamp >= "+POut.DateT(dateTimeStart)+" "
				+"AND DateTStamp < "+POut.DateT(dateTimeStop);
			List<Patient> listPatients=Crud.PatientCrud.SelectMany(command);
			return listPatients;
		}

		

		

	}
}