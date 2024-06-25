using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.UI;
using System.Windows;
using OpenDentBusiness.WebTypes;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Signalods {
		#region Fields - Public
		///<summary>This is not the actual date/time last refreshed.  It is really the server based date/time of the last item in the database retrieved on previous refreshes.  That way, the local workstation time is irrelevant.</summary>
		///<summary>This is not the actual date/time last refreshed.  It is really the server based date/time of the last item in the database retrieved on previous refreshes.  That way, the local workstation time is irrelevant.
		///Middle tier also uses this field, however middle tier only processes cache refresh signals.</summary>
		public static DateTime DateTRegularPrioritySignalLastRefreshed;
		///<summary>This is the server based date/time of the last item in the database retrieved on a previous refresh. There is an attampt to update this value on every tick, with no conditions.
		///Middle tier does not uses this field, however middle tier only processes cache refresh signals, and does not need to handle high priority signals (like shutdown, printing, etc).</summary>
		public static DateTime DateTHighPrioritySignalLastRefreshed;
		///<summary>Mimics the behavior of DateSignalLastRefreshed, but is used exclusively in ContrAppt.TickRefresh(). The root issue was that when a client came back from being inactive
		///ContrAppt.TickRefresh() was using DateSignalLastRefreshed, which is only set after we process signals. Therefore, when a client went inactive, we could potentially query the 
		///SignalOD table for a much larger dataset than intended. E.g.- Client goes inactive for 3 hours, comes back, ContrAppt.TickRefresh() is called and calls RefreshTimed() with a 3 hour old datetime.</summary>
		public static DateTime DateTApptSignalLastRefreshed;
		///<summary>Track the last time that the web service refreshed it's cache. 
		///The cache is shared by all consumers of this web service for this app pool. 
		///Yes this goes against best practice and yes this could lead to occasional collisions. 
		///But the risk of these things happening is very low given the low frequency of traffic and the low frequency of cache-eligible changes being made.</summary>
		public static DateTime DateTSignalLastRefreshedWeb=DateTime.MinValue;
		#endregion

		#region Fields - Private
		///<summary>SignalNums that this ServerMT instance has processed.</summary>
		private static ConcurrentHashSet<long> _concurrentHashSetSignalNums=new ConcurrentHashSet<long>();
		#endregion

		///<summary> Get the latest signal's SigDateTime from the DB </summary>
		public static DateTime GetLatestSignalTime() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM signalod ";
			command+="ORDER BY SigDateTime DESC ";
			command+="LIMIT 1";
			try {
				return Crud.SignalodCrud.SelectOne(command).SigDateTime;
			}
			catch {
				return MiscData.GetNowDateTime();
			}
		}

		///<summary>Gets all Signals since a given DateTime.  If it can't connect to the database, then it returns a list of length 0.
		///Remeber that the supplied dateTime is server time.  This has to be accounted for.
		///ListITypes is an optional parameter for querying specific signal types.
		///ServerMT instances will always be given a chance to process signals being returned from this method.</summary>
		public static List<Signalod> RefreshTimed(DateTime dateTSince,List<InvalidType> listInvalidTypes=null,List<InvalidType> listInvalidTypesExclude=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Signalod>>(MethodBase.GetCurrentMethod(),dateTSince,listInvalidTypes,listInvalidTypesExclude);
			}
			//This command was written to take into account the fact that MySQL truncates seconds to the the whole second on DateTime columns. (newer versions support fractional seconds)
			//By selecting signals less than Now() we avoid missing signals the next time this function is called. Without the addition of Now() it was possible
			//to miss up to ((N-1)/N)% of the signals generated in the worst case scenario.
			string command="SELECT * FROM signalod "
				+"WHERE (SigDateTime>"+POut.DateT(dateTSince)+" AND SigDateTime< "+DbHelper.Now()+") ";
			if(!listInvalidTypes.IsNullOrEmpty()) {
				command+="AND IType IN("+String.Join(",",listInvalidTypes.Select(x => (int)x))+") ";
			}
			if(!listInvalidTypesExclude.IsNullOrEmpty()){
				command+="AND IType NOT IN("+String.Join(",",listInvalidTypesExclude.Select(x => (int)x))+") ";
			}
			command+="ORDER BY SigDateTime";
			//note: this might return an occasional row that has both times newer.
			List<Signalod> listSignalods=new List<Signalod>();
			try {
				listSignalods=Crud.SignalodCrud.SelectMany(command);
			} 
			catch {
				//we don't want an error message to show, because that can cause a cascade of a large number of error messages.
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
				ProcessSignalsForServerMT(listSignalods);
			}
			return listSignalods;
		}

		///<summary>ServerMT instances keep track of every SignalNum they have processed and will only process new signals that are passed in.</summary>
		private static void ProcessSignalsForServerMT(List<Signalod> listSignalods) {
			//No need to check MiddleTierRole; This is only run on ServerMT.
			//Find SignalNums that have not been processed.
			List<long> listSignalNumsNotProcessed=new List<long>();
			List<long> listSignalNums=listSignalods.Select(x => x.SignalNum).ToList();
			_concurrentHashSetSignalNums.ClearIfNeeded();
			for(int i=0;i<listSignalNums.Count;i++) {
				if(_concurrentHashSetSignalNums.Contains(listSignalNums[i])) {
					continue;//SignalNum has already been processed.
				}
				if(_concurrentHashSetSignalNums.Add(listSignalNums[i])) {
					//This thread is in charge of processing this SignalNum since it successfully added the SignalNum to the HashSet.
					listSignalNumsNotProcessed.Add(listSignalNums[i]);
				}
			}
			if(listSignalNumsNotProcessed.IsNullOrEmpty()) {
				return;//All signals have been processed before.
			}
			//Attempt to process InvalidTypes for new signals.
			InvalidType[] invalidTypeArray=listSignalods.Where(x => listSignalNumsNotProcessed.Contains(x.SignalNum))
				.Select(x => x.IType)
				.Distinct()
				.ToArray();
			ODException.SwallowAnyException(() => Cache.Refresh(invalidTypeArray));//even though the clients now use a strategy of just setting cache to null.
		}

		///<summary></summary>
		public static List<SignalodForApi> GetSignalOdsForApi(int limit,int offset,DateTime dateTSince,List<InvalidType> listInvalidTypes=null){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SignalodForApi>>(MethodBase.GetCurrentMethod(),limit,offset,dateTSince,listInvalidTypes);
			}
			//This command was written to take into account the fact that MySQL truncates seconds to the the whole second on DateTime columns. (newer versions support fractional seconds)
			//By selecting signals less than Now() we avoid missing signals the next time this function is called. Without the addition of Now() it was possible
			//to miss up to ((N-1)/N)% of the signals generated in the worst case scenario.
			string command="SELECT * FROM signalod "
				+"WHERE (SigDateTime>"+POut.DateT(dateTSince)+" AND SigDateTime< "+DbHelper.Now()+") ";
			if(!listInvalidTypes.IsNullOrEmpty()) {
				command+="AND IType IN("+String.Join(",",listInvalidTypes.Select(x => (int)x))+") ";
			}
			command+="ORDER BY SignalNum "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			//note: this might return an occasional row that has both times newer.
			List<Signalod> listSignalods=new List<Signalod>();
			string commandDatetime="SELECT "+DbHelper.Now();
			DateTime dateTimeServer=PIn.DateT(OpenDentBusiness.Db.GetScalar(commandDatetime));//run before signals for rigorous inclusion of signals
			try {
				listSignalods=Crud.SignalodCrud.SelectMany(command);
			} 
			catch {
				//we don't want an error message to show, because that can cause a cascade of a large number of error messages.
			}
			List<SignalodForApi> listSignalodForApis=new List<SignalodForApi>();
			for(int i=0;i<listSignalods.Count;i++) {//list can be empty
				SignalodForApi signalodForApi=new SignalodForApi();
				signalodForApi.Signalod=listSignalods[i];
				signalodForApi.DateTimeServer=dateTimeServer;
				listSignalodForApis.Add(signalodForApi);
			}
			return listSignalodForApis;
		}

		///<summary>Queries the database and returns true if we found a shutdown signal</summary>
		public static bool DoesNeedToShutDown(DateTime dateTimeSinceLastChecked) {
			//No need to check MiddleTierRole; no call to db.
			int numShutDownSignals=GetCountForTypes(dateTimeSinceLastChecked,InvalidType.ShutDownNow);
			return numShutDownSignals>0;
		}

		///<summary>Queries the database and returns true if we found a Sites signal</summary>
		public static bool DoesNeedToRefreshSitesCache(DateTime dateTSinceLastChecked) {
			//No need to check MiddleTierRole; no call to db.
			int numSitesSignals=GetCountForTypes(dateTSinceLastChecked,InvalidType.Sites);
			return numSitesSignals>0;
		}

		public static int GetCountForTypes(DateTime dateTimeSinceLastChecked,params InvalidType[] invalidTypeArray) {
			if(invalidTypeArray.IsNullOrEmpty()) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),dateTimeSinceLastChecked,invalidTypeArray);
			}
			//string[] array=;
			string command=$"SELECT COUNT(*) FROM signalod "
				+$"WHERE SigDateTime>{POut.DateT(dateTimeSinceLastChecked)} "
				+$"AND SigDateTime<{DbHelper.Now()} "
				+$"AND IType IN({string.Join(",",invalidTypeArray.Select(x => POut.Int((int)x)))})";
			int numSitesSignals=PIn.Int(Db.GetCount(command));
			return numSitesSignals;
		}

		///<summary>Process all Signals and Acks Since a given DateTime.  Only to be used by OpenDentalWebService.
		///Returns latest valid signal Date/Time and the list of InvalidTypes that were refreshed.
		///Can throw exception.</summary>
		public static List<InvalidType> RefreshForWeb() {
			InvalidType[] invalidTypeArray=new InvalidType[0];
			List<Signalod>listSignalods=new List<Signalod>();
			try {
				listSignalods=GetInvalidSignalsForWeb();
				invalidTypeArray=Signalods.GetInvalidTypesForWeb(listSignalods);
				//Get all invalid types since given time and refresh the cache for those given invalid types.
				Cache.Refresh(invalidTypeArray);
			}
			catch(Exception ex) {
				//Most likely cause for an exception here would be a thread collision between 2 consumers trying to refresh the cache at the exact same instant.
				//There is a chance that performing a subsequent refresh here would cause yet another collision but it's the best we can do without redesigning the entire cache pattern.
				Cache.Refresh(InvalidType.AllLocal);
				throw new Exception("Server cache may be invalid. Please try again. Error: "+ex.Message);
			}
			InvalidTypeHistory.UpdateStatus(DateTSignalLastRefreshedWeb,listSignalods,invalidTypeArray);
			return invalidTypeArray.ToList();
		}

		///<summary>Gets all Signals since SignalLastRefreshedWeb. Returns empty list if it is not yet time to process signals again. Can throw exception.</summary>
		public static List<Signalod> GetInvalidSignalsForWeb() {
			try {
				InvalidTypeHistory.InitIfNecessary();
				int defaultProcessSigsIntervalInSecs=7;
				ODException.SwallowAnyException(() => defaultProcessSigsIntervalInSecs=PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs));
				if(DateTime.Now.Subtract(DateTSignalLastRefreshedWeb)<=TimeSpan.FromSeconds(defaultProcessSigsIntervalInSecs)) {
					return new List<Signalod>();
				}
				//No need to check MiddleTierRole; no call to db.
				List<Signalod> listSignalods=new List<Signalod>();
				if(DateTSignalLastRefreshedWeb.Year<1880) { //First signals for this session so go back in time a bit.
					DateTSignalLastRefreshedWeb=MiscData.GetNowDateTime().AddSeconds(-1);
				}
				listSignalods=RefreshTimed(DateTSignalLastRefreshedWeb);
				if(listSignalods.Count > 0) { //Next lower bound is current upper bound.
					DateTSignalLastRefreshedWeb=listSignalods.Max(x => x.SigDateTime);
				}
				return listSignalods;
			}
			catch(Exception) {
				//Reset the last signal process time if there was an error.
				DateTime dateTimeLastRefreshed=DateTime.Now;
				ODException.SwallowAnyException(() => dateTimeLastRefreshed=OpenDentBusiness.MiscData.GetNowDateTime());
				DateTSignalLastRefreshedWeb=dateTimeLastRefreshed;
				throw;
			}
		}

		///<summary>Returns the PK of the signal inserted if only one signal was passed in; Otherwise, returns 0.</summary>
		public static long Insert(params Signalod[] signalodArray) {
			if(signalodArray==null || signalodArray.Length < 1) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				long signalNum=Meth.GetLong(MethodBase.GetCurrentMethod(),signalodArray);
				if(signalodArray.Length==1) {
					signalodArray[0].SignalNum=signalNum;
				}
				return signalNum;
			}
			for(int i=0;i<signalodArray.Length;i++) {
				signalodArray[i].RemoteRole=RemotingClient.MiddleTierRole;
			}
			if(signalodArray.Length==1) {
				return Crud.SignalodCrud.Insert(signalodArray[0]);
			}
			Crud.SignalodCrud.InsertMany(signalodArray.ToList());
			return 0;
		}

		///<summary>Simplest way to use the new fKey and FKeyType. Set isBroadcast=true to process signals immediately on workstation.</summary>
		public static long SetInvalid(InvalidType invalidType,KeyType fKeyType,long fKey) {
			//Remoting role check performed in the Insert.
			Signalod signalod=new Signalod();
			signalod.IType=invalidType;
			signalod.DateViewing=DateTime.MinValue;
			signalod.FKey=fKey;
			signalod.FKeyType=fKeyType;
			return Insert(signalod);
		}

		///<summary>Creates up to 3 signals for each supplied appt.  The signals are needed for many different kinds of changes to the appointment, but the signals only specify Provs and Ops because that's what's needed to tell workstations which views to refresh.  Always call a refresh of the appointment module before calling this method.  apptNew cannot be null.  apptOld is only used when making changes to an existing appt and Provs or Ops have changed. Generally should not be called outside of Appointments.cs</summary>
		public static void SetInvalidAppt(Appointment appointmentNew,Appointment appointmentOld = null) {
			if(appointmentNew==null) {
				if(appointmentOld==null) {
					return;//should never happen. Both apptNew and apptOld are null in this scenario
				}
				//If apptOld is not null then use it as the apptNew so we can send signals
				//Most likely occurred due to appointment delete.
				appointmentNew=appointmentOld;
				appointmentOld=null;
			}
			bool addSigForNewApt=IsApptInRefreshRange(appointmentNew);
			bool addSignForOldAppt=IsApptInRefreshRange(appointmentOld);
			//The eight possible signals are:
			//  1.New Provider
			//  2.New Hyg
			//  3.New Op
			//  4.Old Provider
			//  5.Old Hyg
			//  6.Old Op
			//  7.New Appt
			//  8.Old Appt
			//If there is no change between new and old, or if there is not an old appt provided, then fewer than 8 signals may be generated.
			List<Signalod> listSignalods=new List<Signalod>();
			if(addSigForNewApt) {
				//  1.New Provider
				Signalod signalodProv=new Signalod();
				signalodProv.DateViewing=appointmentNew.AptDateTime;
				signalodProv.IType=InvalidType.Appointment;
				signalodProv.FKey=appointmentNew.ProvNum;
				signalodProv.FKeyType=KeyType.Provider;
				listSignalods.Add(signalodProv);
				//  2.New Hyg
				if(appointmentNew.ProvHyg>0) {
					Signalod signalodHyg=new Signalod();
					signalodHyg.DateViewing=appointmentNew.AptDateTime;
					signalodHyg.IType=InvalidType.Appointment;
					signalodHyg.FKey=appointmentNew.ProvHyg;
					signalodHyg.FKeyType=KeyType.Provider;
					listSignalods.Add(signalodHyg);
				}
				//  3.New Op
				if(appointmentNew.Op>0) {
					Signalod signalodOp=new Signalod();
					signalodOp.DateViewing=appointmentNew.AptDateTime;
					signalodOp.IType=InvalidType.Appointment;
					signalodOp.FKey=appointmentNew.Op;
					signalodOp.FKeyType=KeyType.Operatory;
					listSignalods.Add(signalodOp);
				}
				//  7.New Appt
				if(appointmentNew!=null) {
					Signalod signalodAppt=new Signalod();
					signalodAppt.DateViewing=appointmentNew.AptDateTime;
					signalodAppt.IType=InvalidType.Appointment;
					signalodAppt.FKey=appointmentNew.PatNum;
					signalodAppt.FKeyType=KeyType.PatNum;
					listSignalods.Add(signalodAppt);
				}
			}
			if(addSignForOldAppt) {
				//  4.Old Provider
				if(appointmentOld!=null && appointmentOld.ProvNum>0 && (appointmentOld.AptDateTime.Date!=appointmentNew.AptDateTime.Date || appointmentOld.ProvNum!=appointmentNew.ProvNum)) {
					Signalod signalodProvOld=new Signalod();
					signalodProvOld.DateViewing=appointmentOld.AptDateTime;
					signalodProvOld.IType=InvalidType.Appointment;
					signalodProvOld.FKey=appointmentOld.ProvNum;
					signalodProvOld.FKeyType=KeyType.Provider;
					listSignalods.Add(signalodProvOld);
				}
				//  5.Old Hyg
				if(appointmentOld!=null && appointmentOld.ProvHyg>0 && (appointmentOld.AptDateTime.Date!=appointmentNew.AptDateTime.Date || appointmentOld.ProvHyg!=appointmentNew.ProvHyg)) {
					Signalod signalodHygOld=new Signalod();
					signalodHygOld.DateViewing=appointmentOld.AptDateTime;
					signalodHygOld.IType=InvalidType.Appointment;
					signalodHygOld.FKey=appointmentOld.ProvHyg;
					signalodHygOld.FKeyType=KeyType.Provider;
					listSignalods.Add(signalodHygOld);
				}
				//  6.Old Op
				if(appointmentOld!=null && appointmentOld.Op>0 && (appointmentOld.AptDateTime.Date!=appointmentNew.AptDateTime.Date || appointmentOld.Op!=appointmentNew.Op)) {
					Signalod signalodOpOld=new Signalod();
					signalodOpOld.DateViewing=appointmentOld.AptDateTime;
					signalodOpOld.IType=InvalidType.Appointment;
					signalodOpOld.FKey=appointmentOld.Op;
					signalodOpOld.FKeyType=KeyType.Operatory;
					listSignalods.Add(signalodOpOld);
				}
				//  8.Old Appt
				if(appointmentOld!=null && (appointmentOld.AptDateTime.Date!=appointmentNew.AptDateTime.Date)) {
					Signalod signalodApptOld=new Signalod();
					signalodApptOld.DateViewing=appointmentOld.AptDateTime;
					signalodApptOld.IType=InvalidType.Appointment;
					signalodApptOld.FKey=appointmentOld.PatNum;
					signalodApptOld.FKeyType=KeyType.PatNum;
					listSignalods.Add(signalodApptOld);
				}
			}
			for(int i=0;i<listSignalods.Count;i++) {
				Insert(listSignalods[i]);
			}
			//There was a delay when using this method to refresh the appointment module due to the time it takes to loop through the signals that iSignalProcessors need to loop through.
			//BroadcastSignals(listSignals);//for immediate update. Signals will be processed again at next tick interval.
		}

		///<summary>Returns true if the Apppointment.AptDateTime is between DateTime.Today and the number of ApptAutoRefreshRange preference days. </summary>
		public static bool IsApptInRefreshRange(Appointment appointment) {
			//No need to check MiddleTierRole; no call to db.
			if(appointment==null) {
				return false;
			}
			int days=PrefC.GetInt(PrefName.ApptAutoRefreshRange);
			if(days==-1) {
				//ApptAutoRefreshRange preference is -1, so all appointments are in range
				return true;
			}
			//Returns true if the appointment is between today and today + the auto refresh day range preference.
			return appointment.AptDateTime.Between(DateTime.Today,DateTime.Today.AddDays(days));
		}

		///<summary>The given dateStart must be less than or equal to dateEnd. Both dates must be valid dates (not min date, etc).</summary>
		public static void SetInvalidSchedForOps(List<Schedule> listSchedules) {
			//No need to check MiddleTierRole; no call to db.
			List<Signalod> listSignalods=new List<Signalod>();
			for(int i=0;i<listSchedules.Count;i++) {
				//All three places that call this just use a single op in their op list.
				//But this is a little more future proof.
				for(int j=0;j<listSchedules[i].Ops.Count;j++) {
					Signalod signalodForOp=new Signalod();
					signalodForOp.IType=InvalidType.Schedules;
					signalodForOp.DateViewing=listSchedules[i].SchedDate;
					signalodForOp.FKey=listSchedules[i].Ops[j];
					signalodForOp.FKeyType=KeyType.Operatory;
					listSignalods.Add(signalodForOp);
				}
			}
			Insert(listSignalods.ToArray());
		}

		///<summary>Inserts a signal for each operatory in the schedule that has been changed, and for the provider the schedule is for. This only
		///inserts a signal for today's schedules. Generally should not be called outside of Schedules.cs</summary>
		public static void SetInvalidSched(params Schedule[] scheduleArray) {
			//No need to check MiddleTierRole; no call to db.
			//Per Nathan, we are only going to insert signals for today's schedules. Most workstations will not be looking at other days for extended
			//lengths of time.
			//Make a list of signals for every operatory involved.
			DateTime dateTimeServer=MiscData.GetNowDateTime();
			List<Schedule> listSchedules=scheduleArray.ToList();
			List<Schedule> listSchedulesToday=listSchedules.Where(x => x.SchedDate.Date==DateTime.Today || x.SchedDate.Date==dateTimeServer.Date).ToList();
			List<Signalod> listSignalods= new List<Signalod>();
			for(int i=0;i<listSchedulesToday.Count;i++) {
				List<long> listOpNums=listSchedulesToday[i].Ops;
				for(int j=0;j<listOpNums.Count;j++) {
					Signalod signalodOp=new Signalod();
					signalodOp.IType=InvalidType.Schedules;
					signalodOp.DateViewing=listSchedulesToday[i].SchedDate;
					signalodOp.FKey=listOpNums[j];
					signalodOp.FKeyType=KeyType.Operatory;
					listSignalods.Add(signalodOp);
				}
			}
			//Make a list of signals for every provider involved.
			List<Schedule> listSchedulesProvider=scheduleArray
				.Where(x => x.ProvNum > 0 && (x.SchedDate.Date==DateTime.Today || x.SchedDate.Date==dateTimeServer.Date)).ToList();
			List<Signalod> listSignalodsProvider=new List<Signalod>();
			for(int i=0;i<listSchedulesProvider.Count;i++) {
				Signalod signalodProvider=new Signalod();
				signalodProvider.IType=InvalidType.Schedules;
				signalodProvider.DateViewing=listSchedulesProvider[i].SchedDate;
				signalodProvider.FKey=listSchedulesProvider[i].ProvNum;
				signalodProvider.FKeyType=KeyType.Provider;
				listSignalodsProvider.Add(signalodProvider);
			}
			List<Signalod> listSignalodsUnique=listSignalods.Union(listSignalodsProvider).ToList();
			if(listSignalodsUnique.Count <= 1000) {
				Insert(listSignalodsUnique.ToArray());
				return;
			}
			//We've had offices insert tens of thousands of signals at once which severely slowed down their database.
			Signalod signalod=new Signalod();
			signalod.IType=InvalidType.Schedules;
			signalod.DateViewing=DateTime.MinValue;//This will cause every workstation to refresh regardless of what they're viewing.
			Insert(signalod);
		}

		///<summary>Schedules, when we don't have a specific FKey and want to set an invalid for the entire type. 
		///Includes the dateViewing parameter for Refresh.
		///A dateViewing of 01-01-0001 will be ignored because it would otherwise cause a full refresh for all connected client workstations.</summary>
		public static void SetInvalidSched(DateTime dateViewing) {
			//No need to check MiddleTierRole; no call to db.
			if(dateViewing==DateTime.MinValue) {
				return;//A dateViewing of 01-01-0001 will be ignored because it would otherwise cause a full refresh for all connected client workstations.
			}
			Signalod signalod=new Signalod();
			signalod.IType=InvalidType.Schedules;
			signalod.DateViewing=dateViewing;
			Insert(signalod);
		}

		///<summary>Upserts the InvalidType.SmsTextMsgReceivedUnreadCount signal which tells all client machines to update the received unread SMS 
		///message count.  There should only be max one of this signal IType in the database.</summary>
		public static List<SmsFromMobiles.SmsNotification> UpsertSmsNotification() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SmsFromMobiles.SmsNotification>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT ClinicNum,COUNT(*) AS CountUnread FROM smsfrommobile WHERE SmsStatus=0 AND IsHidden=0 GROUP BY ClinicNum "
				+"ORDER BY ClinicNum";
			DataTable tableSmsFromMobile=Db.GetTable(command);
			List<SmsFromMobiles.SmsNotification> listSmsNotifications=new List<SmsFromMobiles.SmsNotification>();
			for(int i=0;i<tableSmsFromMobile.Rows.Count;i++) {
				SmsFromMobiles.SmsNotification smsNotification=new SmsFromMobiles.SmsNotification();
				smsNotification.ClinicNum=PIn.Long(tableSmsFromMobile.Rows[i]["ClinicNum"].ToString());
				smsNotification.Count=PIn.Int(tableSmsFromMobile.Rows[i]["CountUnread"].ToString());
				listSmsNotifications.Add(smsNotification);
			}
			//Insert as structured data signal so all workstations won't have to query the db to get the counts. They will get it directly from Signalod.MsgValue.
			string json=SmsFromMobiles.SmsNotification.GetJsonFromList(listSmsNotifications);
			//FKeyType SmsMsgUnreadCount is written to db as a string. 
			command="SELECT * FROM signalod WHERE IType="+POut.Int((int)InvalidType.SmsTextMsgReceivedUnreadCount)
				+" AND FKeyType='"+POut.String(KeyType.SmsMsgUnreadCount.ToString())+"' ORDER BY SigDateTime DESC LIMIT 1";
			DataTable table=Db.GetTable(command);
			Signalod signalod=Crud.SignalodCrud.TableToList(table).FirstOrDefault();
			if(signalod!=null && signalod.MsgValue==json) {//No changes, not need to insert a new signal.
				return listSmsNotifications;//Return the list of notifications, but do not update the existing signal.
			}
			Signalod signalodNew=new Signalod();
			signalodNew.IType=InvalidType.SmsTextMsgReceivedUnreadCount;
			signalodNew.FKeyType=KeyType.SmsMsgUnreadCount;
			signalodNew.MsgValue=json;
			signalodNew.RemoteRole=RemotingClient.MiddleTierRole;
			Signalods.Insert(signalodNew);
			return listSmsNotifications;
		}

		/// <summary>Check for appointment signals for a single date.</summary>
		public static bool IsApptRefreshNeeded(DateTime dateTimeShowing,List<Signalod> listSignalods,List<long> listOpNumsVisible,
			List<long> listProvNumsVisible) 
		{
			//No need to check MiddleTierRole; no call to db.
			return IsApptRefreshNeeded(dateTimeShowing,dateTimeShowing,listSignalods,listOpNumsVisible,listProvNumsVisible);
		}

		///<summary>After a refresh, this is used to determine whether the Appt Module needs to be refreshed. Returns true if there are any signals
		///with InvalidType=Appointment where the DateViewing time of the signal falls within the provided daterange, and the signal matches either
		///the list of visible operatories or visible providers in the current Appt Module View. Always returns true if any signals have
		///DateViewing=DateTime.MinVal.</summary>
		public static bool IsApptRefreshNeeded(DateTime dateStart,DateTime dateEnd,List<Signalod> listSignalods,List<long> listOpNumsVisible,
			List<long> listProvNumsVisible) 
		{
			//No need to check MiddleTierRole; no call to db.
			//A date range was refreshed.  Easier to refresh all without checking.
			if(listSignalods.Exists(x => (x.DateViewing.Date==DateTime.MinValue.Date || x.FKeyType==KeyType.PatNum) && x.IType==InvalidType.Appointment)) {
				return true;
			}
			List<Signalod> listSignalodsAppt=listSignalods.FindAll(x => x.IType==InvalidType.Appointment &&
				x.DateViewing.Date >= dateStart.Date && x.DateViewing.Date <= dateEnd.Date);
			if(listSignalodsAppt.Count==0) {
				return false;
			}
			//List<long> visibleOps = ApptDrawing.VisOps.Select(x => x.OperatoryNum).ToList();
			//List<long> visibleProvs = ApptDrawing.VisProvs.Select(x => x.ProvNum).ToList();
			if(listSignalodsAppt.Any(x=> x.FKeyType==KeyType.Operatory && listOpNumsVisible.Contains(x.FKey))
				|| listSignalodsAppt.Any(x=> x.FKeyType==KeyType.Provider && listProvNumsVisible.Contains(x.FKey))) 
			{
				return true;
			}
			return false;
		}

		/// <summary>Check for schedule signals for a single date.</summary>
		public static bool IsSchedRefreshNeeded(DateTime dateTimeShowing,List<Signalod> listSignalods,List<long> listOpNumsVisible,
			List<long> listProvNumsVisible) 
		{
			//No need to check MiddleTierRole; no call to db.
			return IsSchedRefreshNeeded(dateTimeShowing,dateTimeShowing,listSignalods,listOpNumsVisible,listProvNumsVisible);
		}

		///<summary>After a refresh, this is used to determine whether the Appt Module needs to be refreshed.  Returns true if there are any signals
		///with InvalidType=Appointment where the DateViewing time of the signal falls within the provided daterange, and the signal matches either
		///the list of visible operatories or visible providers in the current Appt Module View.  Always returns true if any signals have
		///DateViewing=DateTime.MinVal.</summary>
		public static bool IsSchedRefreshNeeded(DateTime dateStart,DateTime dateEnd,List<Signalod> listSignalods,List<long> listOpNumsVisible,
			List<long> listProvNumsVisible) 
		{
			//No need to check MiddleTierRole; no call to db.
			//A date range was refreshed.  Easier to refresh all without checking.
			if(listSignalods.Exists(x => x.DateViewing.Date==DateTime.MinValue.Date && x.IType==InvalidType.Schedules)) {
				return true;
			}
			List<Signalod> listSignalodsSched=listSignalods.FindAll(x => x.IType==InvalidType.Schedules && 
				x.DateViewing.Date >= dateStart.Date && x.DateViewing.Date <= dateEnd.Date);
			if(listSignalodsSched.Count==0) {
				return false;
			}
			if(listSignalodsSched.Any(x=> x.FKeyType==KeyType.Operatory && listOpNumsVisible.Contains(x.FKey))
				|| listSignalodsSched.Any(x=> x.FKeyType==KeyType.Provider && listProvNumsVisible.Contains(x.FKey))
				|| listSignalodsSched.Any(x => x.FKeyType==KeyType.Undefined))//For blockouts cleared on a single day.
			{
				return true;
			}
			return false;
		}
	
		///<summary>After a refresh, this is used to determine whether the buttons and listboxes need to be refreshed on the ContrApptPanel. 
		///Will return true with InvalidType==Defs.</summary>
		public static bool IsContrApptButtonRefreshNeeded(List<Signalod> listSignalods) {
			if(listSignalods.Exists(x => x.IType==InvalidType.Defs)) {
				return true;
			}
			return false;
		}

		///<summary>After a refresh, this is used to get a list containing all flags of types that need to be refreshed. The FKey must be 0 and the
		///FKeyType must Undefined. Types of Task and SmsTextMsgReceivedUnreadCount are not included.</summary>
		public static InvalidType[] GetInvalidTypes(List<Signalod> listSignalods) {
			//No need to check MiddleTierRole; no call to db.
			InvalidType[] invalidTypeArray=listSignalods.FindAll(x => x.IType!=InvalidType.Task
					&& x.IType!=InvalidType.TaskPopup
					&& x.IType!=InvalidType.SmsTextMsgReceivedUnreadCount
					&& x.FKey==0
					&& x.FKeyType==KeyType.Undefined)
				.Select(x => x.IType).ToArray();
			return invalidTypeArray;
		}


		///<summary>Our eServices have not been refactored yet to handle granular refreshes yet. This method does include signals that have a FKey. 
		///Ideally this method will be deprecated once eServices uses FKeys in cache refreshes.</summary>
		public static InvalidType[] GetInvalidTypesForWeb(List<Signalod> listSignalods) {
			//No need to check MiddleTierRole; no call to db.
			InvalidType[] invalidTypeArray=listSignalods.FindAll(x => x.IType!=InvalidType.Task
					&& x.IType!=InvalidType.TaskPopup
					&& x.IType!=InvalidType.SmsTextMsgReceivedUnreadCount)
					//TODO: Future enhancement is to rejoin this method with GetInvalidTypes. To do that we will need to have our eServices refresh parts of 
					//caches based on FKey.
				.Select(x => x.IType).ToArray();
			return invalidTypeArray;
		}

		/// <summary>2024-05-08-Jordan. I just noticed this. I think it's wrong, but it's heavily used. We're always supposed to use DataValid.SetInvalid, not this. Looks like I added it in 2009, but I don't remember. We need to at least test and compare this with DataValid.SetInvalid. We need to make sure it's doing the same thing, refreshing local machine, other machines, and MT. Seems to be used in UI layer 77 times vs 456 times for DataValid.SetInvalid. That's good. And another 68 times in ODB, which might be unavoidable, and is probably why it was created.</summary>
		//Won't work with InvalidType.Date, InvalidType.Task, or InvalidType.TaskPopup  yet.
		public static void SetInvalid(params InvalidType[] invalidTypeArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),invalidTypeArray);
				return;
			}
			for(int i=0;i<invalidTypeArray.Length;i++) {
				Signalod signalod=new Signalod();
				signalod.IType=invalidTypeArray[i];
				signalod.DateViewing=DateTime.MinValue;
				switch(invalidTypeArray[i]) {
					case InvalidType.UserOdPrefs:
						signalod.FKey=Security.CurUser?.UserNum??0;
						signalod.FKeyType=KeyType.UserOd;
						break;
				}
				Insert(signalod);
			}
		}

		///<summary>Insertion logic that doesn't use the cache. Has special cases for generating random PK's and handling Oracle insertions.</summary>
		public static void SetInvalidNoCache(params InvalidType[] invalidTypeArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),invalidTypeArray);
				return;
			}
			for(int i=0;i<invalidTypeArray.Length;i++) {
				Signalod signalod=new Signalod();
				signalod.IType=invalidTypeArray[i];
				signalod.DateViewing=DateTime.MinValue;
				signalod.RemoteRole=RemotingClient.MiddleTierRole;
				Crud.SignalodCrud.InsertNoCache(signalod);
			}
		}

		///<summary>Must be called after Preference cache has been filled.
		///Deletes all signals older than 2 days if this has not been run within the last week.  Will fail silently if anything goes wrong.</summary>
		public static void ClearOldSignals() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			DateTime dateTimeServer=MiscData.GetNowDateTime();
			if(Prefs.GetContainsKey(PrefName.SignalLastClearedDate.ToString())
				&& PrefC.GetDateT(PrefName.SignalLastClearedDate)>dateTimeServer.AddDays(-7) //Has already been run in the past week. This is all server based time.
				&& PrefC.GetDateT(PrefName.SignalLastClearedDate) < dateTimeServer) //SignalLastClearedDate isn't in the future job 46490
			{
				return;//Do not run this process again.
			}
			Prefs.UpdateDateT(PrefName.SignalLastClearedDate,dateTimeServer);//Set Last cleared to now.
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {//easier to read that using the DbHelper Functions and it also matches the ConvertDB3 script
				command="DELETE FROM signalod WHERE SigDateTime < DATE_ADD(NOW(),INTERVAL -2 DAY)";//Itypes only older than 2 days
				Db.NonQ(command);
			}
			else {//oracle
				command="DELETE FROM signalod WHERE SigDateTime < CURRENT_TIMESTAMP -2";//Itypes only older than 2 days
				Db.NonQ(command);
			}
			SigMessages.ClearOldSigMessages();//Clear messaging buttons which use to be stored in the signal table.
			//SigElements.DeleteOrphaned();
		}

		///<summary>A helper class that locks access to a HashSet for thread safety.</summary>
		private class ConcurrentHashSet<T> {
			private ReaderWriterLockSlim _readerWriterLockSlim=new ReaderWriterLockSlim();
			private HashSet<T> _hashSet=new HashSet<T>();

			///<summary>Adds the specified element to a set. Returns true if the element is added or false if the element is already present.</summary>
			public bool Add(T tItem) {
				_readerWriterLockSlim.EnterWriteLock();
				try {
					return _hashSet.Add(tItem);
				}
				finally {
					_readerWriterLockSlim.ExitWriteLock();
				}
			}

			///<summary>Returns true if the specified element is already present in the set; otherwise, false.</summary>
			public bool Contains(T tItem) {
				_readerWriterLockSlim.EnterReadLock();
				try {
					return _hashSet.Contains(tItem);
				}
				finally {
					_readerWriterLockSlim.ExitReadLock();
				}
			}

			///<summary>Clears the set if there are at least 5 million elements.</summary>
			public void ClearIfNeeded() {
				bool isClearNeeded=false;
				_readerWriterLockSlim.EnterReadLock();
				try {
					isClearNeeded=(_hashSet.Count > 5_000_000);
				}
				finally {
					_readerWriterLockSlim.ExitReadLock();
				}
				if(!isClearNeeded) {
					return;
				}
				_readerWriterLockSlim.EnterWriteLock();
				try {
					_hashSet.Clear();
				}
				finally {
					_readerWriterLockSlim.ExitWriteLock();
				}
			}
		}
	}

	public class SignalodForApi {
		public Signalod Signalod;
		public DateTime DateTimeServer;
	}

	


}
