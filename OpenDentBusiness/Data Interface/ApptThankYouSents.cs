using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ApptThankYouSents{
		public const string ADD_TO_CALENDAR="[AddToCalendar]";

		public static List<ApptThankYouSent> GetForApt(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptThankYouSent>>(MethodBase.GetCurrentMethod(),aptNum);
			}
			string command="SELECT * FROM apptthankyousent WHERE ApptNum="+POut.Long(aptNum);
			return Crud.ApptThankYouSentCrud.SelectMany(command);
		}

		public static void InsertMany(List<ApptThankYouSent> listApptThankYouSents) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listApptThankYouSents);
				return;
			}
			Crud.ApptThankYouSentCrud.InsertMany(listApptThankYouSents);
		}

		#region Update
		///<summary></summary>
		public static void Update(ApptThankYouSent apptThankYouSent){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),apptThankYouSent);
				return;
			}
			Crud.ApptThankYouSentCrud.Update(apptThankYouSent);
		}
		#endregion Update

		#region Delete
		///<summary></summary>
		public static void Delete(params long[] arrApptThankYouSentNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),arrApptThankYouSentNums);
				return;
			}
			if(arrApptThankYouSentNums.IsNullOrEmpty()) {
				return;
			}
			string command="DELETE FROM apptthankyousent WHERE ApptThankYouSentNum IN ("
				+string.Join(",",arrApptThankYouSentNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}
		#endregion Delete

		public static void HandleApptChanged(Logger.IWriteLine log,ODThread thread) {
			bool HasQuit() {
				if(thread.HasQuit) {
					log.WriteLine("Leaving HandleApptChanged early.",LogLevel.Information);
				}
				return thread.HasQuit;
			}
			if(HasQuit()) {
				return;
			}
			//Delete the ApptThankYouSent entries for rescheduled/cancelled appointments.  AutoComm will resend these automatically where the appointment 
			//still and HQ will increment the Sequence number of the .ics file such that the calendar entry on the patient's device is updated.
			//Remove those that the user specifically said to not resend
			List<ApptThankYouSent> listThanksChanged=GetForApptChanged();
			log.WriteLine($"Deleting {listThanksChanged.Count} ApptThankYouSent entries.",LogLevel.Information);
			string verboseLog=string.Join("\r\n\t\t",listThanksChanged
				.Select(x => $"ApptThankYouSentNum: {x.ApptThankYouSentNum}, PatNum: {x.PatNum}, ApptDateTime: {x.ApptDateTime}"));
			log.WriteLine($"Deleting \r\n\t\t{verboseLog}",LogLevel.Verbose);
			Delete(listThanksChanged.Select(x => x.ApptThankYouSentNum).ToArray());
		}

		//<summary>Get the list of ApptThankYouSents where the appointment was rescheduled or cancelled after sending the thank you.</summary>
		public static List<ApptThankYouSent> GetForApptChanged() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptThankYouSent>>(MethodBase.GetCurrentMethod());
			}
			//Do not include UnscheduledList or Broken appointments
			List<ApptStatus> listStatus=new List<ApptStatus>() { ApptStatus.UnschedList, ApptStatus.Broken };
			string command=@"SELECT apptthankyousent.* 
				FROM apptthankyousent
				LEFT JOIN appointment ON apptthankyousent.ApptNum=appointment.AptNum
				WHERE apptthankyousent.DoNotResend=0 AND (appointment.AptNum IS NULL OR appointment.AptDateTime!=apptthankyousent.ApptDateTime
				OR appointment.AptStatus IN ("+string.Join(",",listStatus.Select(x => POut.Int((int)x)))+"))";
			return Crud.ApptThankYouSentCrud.SelectMany(command);
		}

		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ApptThankYouSentCache : CacheListAbs<ApptThankYouSent> {
			protected override List<ApptThankYouSent> GetCacheFromDb() {
				string command="SELECT * FROM apptthankyousent";
				return Crud.ApptThankYouSentCrud.SelectMany(command);
			}
			protected override List<ApptThankYouSent> TableToList(DataTable table) {
				return Crud.ApptThankYouSentCrud.TableToList(table);
			}
			protected override ApptThankYouSent Copy(ApptThankYouSent apptThankYouSent) {
				return apptThankYouSent.Copy();
			}
			protected override DataTable ListToTable(List<ApptThankYouSent> listApptThankYouSents) {
				return Crud.ApptThankYouSentCrud.ListToTable(listApptThankYouSents,"ApptThankYouSent");
			}
			protected override void FillCacheIfNeeded() {
				ApptThankYouSents.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ApptThankYouSentCache _apptThankYouSentCache=new ApptThankYouSentCache();

		public static List<ApptThankYouSent> GetDeepCopy(bool isShort=false) {
			return _apptThankYouSentCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _apptThankYouSentCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<ApptThankYouSent> match,bool isShort=false) {
			return _apptThankYouSentCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<ApptThankYouSent> match,bool isShort=false) {
			return _apptThankYouSentCache.GetFindIndex(match,isShort);
		}

		public static ApptThankYouSent GetFirst(bool isShort=false) {
			return _apptThankYouSentCache.GetFirst(isShort);
		}

		public static ApptThankYouSent GetFirst(Func<ApptThankYouSent,bool> match,bool isShort=false) {
			return _apptThankYouSentCache.GetFirst(match,isShort);
		}

		public static ApptThankYouSent GetFirstOrDefault(Func<ApptThankYouSent,bool> match,bool isShort=false) {
			return _apptThankYouSentCache.GetFirstOrDefault(match,isShort);
		}

		public static ApptThankYouSent GetLast(bool isShort=false) {
			return _apptThankYouSentCache.GetLast(isShort);
		}

		public static ApptThankYouSent GetLastOrDefault(Func<ApptThankYouSent,bool> match,bool isShort=false) {
			return _apptThankYouSentCache.GetLastOrDefault(match,isShort);
		}

		public static List<ApptThankYouSent> GetWhere(Predicate<ApptThankYouSent> match,bool isShort=false) {
			return _apptThankYouSentCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_apptThankYouSentCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_apptThankYouSentCache.FillCacheFromTable(table);
				return table;
			}
			return _apptThankYouSentCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<ApptThankYouSent> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ApptThankYouSent>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM apptthankyousent WHERE PatNum = "+POut.Long(patNum);
			return Crud.ApptThankYouSentCrud.SelectMany(command);
		}
		
		///<summary>Gets one ApptThankYouSent from the db.</summary>
		public static ApptThankYouSent GetOne(long apptThankYouSentNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<ApptThankYouSent>(MethodBase.GetCurrentMethod(),apptThankYouSentNum);
			}
			return Crud.ApptThankYouSentCrud.SelectOne(apptThankYouSentNum);
		}
		#endregion Get Methods
		#region Modification Methods
		#region Insert
		///<summary></summary>
		public static long Insert(ApptThankYouSent apptThankYouSent){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				apptThankYouSent.ApptThankYouSentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),apptThankYouSent);
				return apptThankYouSent.ApptThankYouSentNum;
			}
			return Crud.ApptThankYouSentCrud.Insert(apptThankYouSent);
		}
		#endregion Insert
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/
	}
}