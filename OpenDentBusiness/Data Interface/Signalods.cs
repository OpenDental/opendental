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
		public static DateTime SignalLastRefreshed;
		///<summary>Mimics the behavior of SignalLastRefreshed, but is used exclusively in ContrAppt.TickRefresh(). The root issue was that when a client came back from being inactive
		///ContrAppt.TickRefresh() was using SignalLastRefreshed, which is only set after we process signals. Therefore, when a client went inactive, we could potentially query the 
		///SignalOD table for a much larger dataset than intended. E.g.- Client goes inactive for 3 hours, comes back, ContrAppt.TickRefresh() is called and calls RefreshTimed() with a 3 hour old datetime.</summary>
		public static DateTime ApptSignalLastRefreshed;
		///<summary>Track the last time that the web service refreshed it's cache. 
		///The cache is shared by all consumers of this web service for this app pool. 
		///Yes this goes against best practice and yes this could lead to occasional collisions. 
		///But the risk of these things happening is very low given the low frequency of traffic and the low frequency of cache-eligible changes being made.</summary>
		public static DateTime SignalLastRefreshedWeb {
			get;
			private set;
		} = DateTime.MinValue;
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
		public static List<Signalod> RefreshTimed(DateTime sinceDateT,List<InvalidType> listITypes=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Signalod>>(MethodBase.GetCurrentMethod(),sinceDateT,listITypes);
			}
			//This command was written to take into account the fact that MySQL truncates seconds to the the whole second on DateTime columns. (newer versions support fractional seconds)
			//By selecting signals less than Now() we avoid missing signals the next time this function is called. Without the addition of Now() it was possible
			//to miss up to ((N-1)/N)% of the signals generated in the worst case scenario.
			string command="SELECT * FROM signalod "
				+"WHERE (SigDateTime>"+POut.DateT(sinceDateT)+" AND SigDateTime< "+DbHelper.Now()+") ";
			if(!listITypes.IsNullOrEmpty()) {
				command+="AND IType IN("+String.Join(",",listITypes.Select(x => (int)x))+") ";
			}
			command+="ORDER BY SigDateTime";
			//note: this might return an occasional row that has both times newer.
			List<Signalod> listSignals=new List<Signalod>();
			try {
				listSignals=Crud.SignalodCrud.SelectMany(command);
			} 
			catch {
				//we don't want an error message to show, because that can cause a cascade of a large number of error messages.
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
				ProcessSignalsForServerMT(listSignals);
			}
			return listSignals;
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
		public static List<SignalodForApi> GetSignalOdsForApi(int limit,int offset,DateTime sinceDateT,List<InvalidType> listITypes=null){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SignalodForApi>>(MethodBase.GetCurrentMethod(),limit,offset,sinceDateT,listITypes);
			}
			//This command was written to take into account the fact that MySQL truncates seconds to the the whole second on DateTime columns. (newer versions support fractional seconds)
			//By selecting signals less than Now() we avoid missing signals the next time this function is called. Without the addition of Now() it was possible
			//to miss up to ((N-1)/N)% of the signals generated in the worst case scenario.
			string command="SELECT * FROM signalod "
				+"WHERE (SigDateTime>"+POut.DateT(sinceDateT)+" AND SigDateTime< "+DbHelper.Now()+") ";
			if(!listITypes.IsNullOrEmpty()) {
				command+="AND IType IN("+String.Join(",",listITypes.Select(x => (int)x))+") ";
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
				signalodForApi.SignalodCur=listSignalods[i];
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
		public static bool DoesNeedToRefreshSitesCache(DateTime dateTimeSinceLastChecked) {
			//No need to check MiddleTierRole; no call to db.
			int numSitesSignals=GetCountForTypes(dateTimeSinceLastChecked,InvalidType.Sites);
			return numSitesSignals>0;
		}

		public static int GetCountForTypes(DateTime dateTimeSinceLastChecked,params InvalidType[] arrayITypes) {
			if(arrayITypes.IsNullOrEmpty()) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),dateTimeSinceLastChecked,arrayITypes);
			}
			//string[] array=;
			string command=$"SELECT COUNT(*) FROM signalod "
				+$"WHERE SigDateTime>{POut.DateT(dateTimeSinceLastChecked)} "
				+$"AND SigDateTime<{DbHelper.Now()} "
				+$"AND IType IN({string.Join(",",arrayITypes.Select(x => POut.Int((int)x)))})";
			int numSitesSignals=PIn.Int(Db.GetCount(command));
			return numSitesSignals;
		}

		///<summary>Process all Signals and Acks Since a given DateTime.  Only to be used by OpenDentalWebService.
		///Returns latest valid signal Date/Time and the list of InvalidTypes that were refreshed.
		///Can throw exception.</summary>
		public static List<InvalidType> RefreshForWeb() {
			InvalidType[] arrayInvalidTypes=new InvalidType[0];
			List<Signalod>listSignals=new List<Signalod>();
			try {
				listSignals=GetInvalidSignalsForWeb();
				arrayInvalidTypes=Signalods.GetInvalidTypesForWeb(listSignals);
				//Get all invalid types since given time and refresh the cache for those given invalid types.
				Cache.Refresh(arrayInvalidTypes);
			}
			catch(Exception ex) {
				//Most likely cause for an exception here would be a thread collision between 2 consumers trying to refresh the cache at the exact same instant.
				//There is a chance that performing as subsequent refresh here would cause yet another collision but it's the best we can do without redesigning the entire cache pattern.
				Cache.Refresh(InvalidType.AllLocal);
				throw new Exception("Server cache may be invalid. Please try again. Error: "+ex.Message);
			}
			InvalidTypeHistory.UpdateStatus(SignalLastRefreshedWeb,listSignals,arrayInvalidTypes);
			return arrayInvalidTypes.ToList();
		}

		///<summary>Gets all Signals since SignalLastRefreshedWeb. Returns empty list if it is not yet time to process signals again. Can throw exception.</summary>
		public static List<Signalod> GetInvalidSignalsForWeb() {
			try {
				InvalidTypeHistory.InitIfNecessary();
				int defaultProcessSigsIntervalInSecs=7;
				ODException.SwallowAnyException(() => defaultProcessSigsIntervalInSecs=PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs));
				if(DateTime.Now.Subtract(SignalLastRefreshedWeb)<=TimeSpan.FromSeconds(defaultProcessSigsIntervalInSecs)) {
					return new List<Signalod>();
				}
				//No need to check MiddleTierRole; no call to db.
				List<Signalod> listSignals=new List<Signalod>();
				if(SignalLastRefreshedWeb.Year<1880) { //First signals for this session so go back in time a bit.
					SignalLastRefreshedWeb=MiscData.GetNowDateTime().AddSeconds(-1);
				}
				listSignals=RefreshTimed(SignalLastRefreshedWeb);
				if(listSignals.Count > 0) { //Next lower bound is current upper bound.
					SignalLastRefreshedWeb=listSignals.Max(x => x.SigDateTime);
				}
				return listSignals;
			}
			catch(Exception) {
				//Reset the last signal process time if there was an error.
				DateTime dateTimeLastRefreshed=DateTime.Now;
				ODException.SwallowAnyException(() => dateTimeLastRefreshed=OpenDentBusiness.MiscData.GetNowDateTime());
				SignalLastRefreshedWeb=dateTimeLastRefreshed;
				throw;
			}
		}

		///<summary>Returns the PK of the signal inserted if only one signal was passed in; Otherwise, returns 0.</summary>
		public static long Insert(params Signalod[] arraySignals) {
			if(arraySignals==null || arraySignals.Length < 1) {
				return 0;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				long signalNum=Meth.GetLong(MethodBase.GetCurrentMethod(),arraySignals);
				if(arraySignals.Length==1) {
					arraySignals[0].SignalNum=signalNum;
				}
				return signalNum;
			}
			foreach(Signalod signal in arraySignals) {
				signal.RemoteRole=RemotingClient.MiddleTierRole;
			}
			if(arraySignals.Length==1) {
				return Crud.SignalodCrud.Insert(arraySignals[0]);
			}
			Crud.SignalodCrud.InsertMany(arraySignals.ToList());
			return 0;
		}

		///<summary>Simplest way to use the new fKey and FKeyType. Set isBroadcast=true to process signals immediately on workstation.</summary>
		public static long SetInvalid(InvalidType iType,KeyType fKeyType,long fKey) {
			//Remoting role check performed in the Insert.
			Signalod sig=new Signalod();
			sig.IType=iType;
			sig.DateViewing=DateTime.MinValue;
			sig.FKey=fKey;
			sig.FKeyType=fKeyType;
			return Insert(sig);
		}

		///<summary>Creates up to 3 signals for each supplied appt.  The signals are needed for many different kinds of changes to the appointment, but the signals only specify Provs and Ops because that's what's needed to tell workstations which views to refresh.  Always call a refresh of the appointment module before calling this method.  apptNew cannot be null.  apptOld is only used when making changes to an existing appt and Provs or Ops have changed. Generally should not be called outside of Appointments.cs</summary>
		public static void SetInvalidAppt(Appointment apptNew,Appointment apptOld = null) {
			if(apptNew==null) {
				//If apptOld is not null then use it as the apptNew so we can send signals
				//Most likely occurred due to appointment delete.
				if(apptOld!=null) {
					apptNew=apptOld;
					apptOld=null;
				}
				else {
					return;//should never happen. Both apptNew and apptOld are null in this scenario
				}
			}
			bool addSigForNewApt=IsApptInRefreshRange(apptNew);
			bool addSignForOldAppt=IsApptInRefreshRange(apptOld);
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
			List<Signalod> listSignals=new List<Signalod>();
			if(addSigForNewApt) {
				//  1.New Provider
				listSignals.Add(
					new Signalod() {
						DateViewing=apptNew.AptDateTime,
						IType=InvalidType.Appointment,
						FKey=apptNew.ProvNum,
						FKeyType=KeyType.Provider,
					});
				//  2.New Hyg
				if(apptNew.ProvHyg>0) {
					listSignals.Add(
						new Signalod() {
							DateViewing=apptNew.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptNew.ProvHyg,
							FKeyType=KeyType.Provider,
						});
				}
				//  3.New Op
				if(apptNew.Op>0) {
					listSignals.Add(
						new Signalod() {
							DateViewing=apptNew.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptNew.Op,
							FKeyType=KeyType.Operatory,
						});
				}
				//  7.New Appt
				if(apptNew!=null) {
					listSignals.Add(
						new Signalod {
							DateViewing=apptNew.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptNew.PatNum,
							FKeyType=KeyType.PatNum,
						});
				}
			}
			if(addSignForOldAppt) {
				//  4.Old Provider
				if(apptOld!=null && apptOld.ProvNum>0 && (apptOld.AptDateTime.Date!=apptNew.AptDateTime.Date || apptOld.ProvNum!=apptNew.ProvNum)) {
					listSignals.Add(
						new Signalod() {
							DateViewing=apptOld.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptOld.ProvNum,
							FKeyType=KeyType.Provider,
						});
				}
				//  5.Old Hyg
				if(apptOld!=null && apptOld.ProvHyg>0 && (apptOld.AptDateTime.Date!=apptNew.AptDateTime.Date || apptOld.ProvHyg!=apptNew.ProvHyg)) {
					listSignals.Add(
						new Signalod() {
							DateViewing=apptOld.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptOld.ProvHyg,
							FKeyType=KeyType.Provider,
						});
				}
				//  6.Old Op
				if(apptOld!=null && apptOld.Op>0 && (apptOld.AptDateTime.Date!=apptNew.AptDateTime.Date || apptOld.Op!=apptNew.Op)) {
					listSignals.Add(
						new Signalod() {
							DateViewing=apptOld.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptOld.Op,
							FKeyType=KeyType.Operatory,
						});
				}
				//  8.Old Appt
				if(apptOld!=null && (apptOld.AptDateTime.Date!=apptNew.AptDateTime.Date)) {
					listSignals.Add(
						new Signalod {
							DateViewing=apptOld.AptDateTime,
							IType=InvalidType.Appointment,
							FKey=apptOld.PatNum,
							FKeyType=KeyType.PatNum,
						});
				}
			}
			listSignals.ForEach(x=>Insert(x));
			//There was a delay when using this method to refresh the appointment module due to the time it takes to loop through the signals that iSignalProcessors need to loop through.
			//BroadcastSignals(listSignals);//for immediate update. Signals will be processed again at next tick interval.
		}

		///<summary>Returns true if the Apppointment.AptDateTime is between DateTime.Today and the number of ApptAutoRefreshRange preference days. </summary>
		public static bool IsApptInRefreshRange(Appointment appt) {
			//No need to check MiddleTierRole; no call to db.
			if(appt==null) {
				return false;
			}
			int days=PrefC.GetInt(PrefName.ApptAutoRefreshRange);
			if(days==-1) {
				//ApptAutoRefreshRange preference is -1, so all appointments are in range
				return true;
			}
			//Returns true if the appointment is between today and today + the auto refresh day range preference.
			return appt.AptDateTime.Between(DateTime.Today,DateTime.Today.AddDays(days));
		}

		///<summary>The given dateStart must be less than or equal to dateEnd. Both dates must be valid dates (not min date, etc).</summary>
		public static void SetInvalidSchedForOps(Dictionary<DateTime,List<long>> dictOpNumsForDates) {
			//No need to check MiddleTierRole; no call to db.
			List <Signalod> listOpSignals=new List<Signalod>();
			foreach(DateTime date in dictOpNumsForDates.Keys) {
				long[] arrayUniqueOpNums=dictOpNumsForDates[date].Distinct().ToArray();
				foreach(long opNum in arrayUniqueOpNums) {
					Signalod signalForOp=new Signalod();
					signalForOp.IType=InvalidType.Schedules;
					signalForOp.DateViewing=date;
					signalForOp.FKey=opNum;
					signalForOp.FKeyType=KeyType.Operatory;
					listOpSignals.Add(signalForOp);
				}
			}
			Insert(listOpSignals.ToArray());
		}

		///<summary>Inserts a signal for each operatory in the schedule that has been changed, and for the provider the schedule is for. This only
		///inserts a signal for today's schedules. Generally should not be called outside of Schedules.cs</summary>
		public static void SetInvalidSched(params Schedule[] arraySchedules) {
			//No need to check MiddleTierRole; no call to db.
			//Per Nathan, we are only going to insert signals for today's schedules. Most workstations will not be looking at other days for extended
			//lengths of time.
			//Make an array of signals for every operatory involved.
			DateTime serverTime=MiscData.GetNowDateTime();
			Signalod[] arrayOpSignals=arraySchedules
				.Where(x => x.SchedDate.Date==DateTime.Today || x.SchedDate.Date==serverTime.Date)
				.SelectMany(x => x.Ops.Select(y => new Signalod() {
					IType=InvalidType.Schedules,
					DateViewing=x.SchedDate,
					FKey=y,
					FKeyType=KeyType.Operatory,
				}))
				.ToArray();
			//Make a array of signals for every provider involved.
			Schedule[] arrayProviderSchedules=arraySchedules.Where(x => x.ProvNum > 0).ToArray();
			Signalod[] arrayProviderSignals=arrayProviderSchedules
				.Where(x => x.SchedDate.Date==DateTime.Today || x.SchedDate.Date==serverTime.Date)
				.Select(x => new Signalod() {
					IType=InvalidType.Schedules,
					DateViewing=x.SchedDate,
					FKey=x.ProvNum,
					FKeyType=KeyType.Provider,
				})
				.ToArray();
			Signalod[] arrayUniqueSignals=arrayOpSignals.Union(arrayProviderSignals).ToArray();
			if(arrayUniqueSignals.Length > 1000) {
				//We've had offices insert tens of thousands of signals at once which severely slowed down their database.
				Signalod signal=new Signalod {
					IType=InvalidType.Schedules,
					DateViewing=DateTime.MinValue,//This will cause every workstation to refresh regardless of what they're viewing.
				};
				Insert(signal);
				return;
			}
			Insert(arrayUniqueSignals);
		}

		///<summary>Schedules, when we don't have a specific FKey and want to set an invalid for the entire type. 
		///Includes the dateViewing parameter for Refresh.
		///A dateViewing of 01-01-0001 will be ignored because it would otherwise cause a full refresh for all connected client workstations.</summary>
		public static void SetInvalidSched(DateTime dateViewing) {
			//No need to check MiddleTierRole; no call to db.
			if(dateViewing==DateTime.MinValue) {
				return;//A dateViewing of 01-01-0001 will be ignored because it would otherwise cause a full refresh for all connected client workstations.
			}
			Signalod sig=new Signalod() {
				IType=InvalidType.Schedules,
				DateViewing=dateViewing
			};
			Insert(sig);
		}

		///<summary>Upserts the InvalidType.SmsTextMsgReceivedUnreadCount signal which tells all client machines to update the received unread SMS 
		///message count.  There should only be max one of this signal IType in the database.</summary>
		public static List<SmsFromMobiles.SmsNotification> UpsertSmsNotification() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SmsFromMobiles.SmsNotification>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT ClinicNum,COUNT(*) AS CountUnread FROM smsfrommobile WHERE SmsStatus=0 AND IsHidden=0 GROUP BY ClinicNum "
				+"ORDER BY ClinicNum";
			List<SmsFromMobiles.SmsNotification> ret=Db.GetTable(command).AsEnumerable()
				.Select(x => new SmsFromMobiles.SmsNotification() {
					ClinicNum=PIn.Long(x["ClinicNum"].ToString()),
					Count=PIn.Int(x["CountUnread"].ToString()),
				}).ToList();
			//Insert as structured data signal so all workstations won't have to query the db to get the counts. They will get it directly from Signalod.MsgValue.
			string json=SmsFromMobiles.SmsNotification.GetJsonFromList(ret);
			//FKeyType SmsMsgUnreadCount is written to db as a string. 
			command="SELECT * FROM signalod WHERE IType="+POut.Int((int)InvalidType.SmsTextMsgReceivedUnreadCount)
				+" AND FKeyType='"+POut.String(KeyType.SmsMsgUnreadCount.ToString())+"' ORDER BY SigDateTime DESC LIMIT 1";
			DataTable table=Db.GetTable(command);
			Signalod sig=Crud.SignalodCrud.TableToList(table).FirstOrDefault();
			if(sig!=null && sig.MsgValue==json) {//No changes, not need to insert a new signal.
				return ret;//Return the list of notifications, but do not update the existing signal.
			}
			Signalods.Insert(new Signalod() {
				IType=InvalidType.SmsTextMsgReceivedUnreadCount,
				FKeyType=KeyType.SmsMsgUnreadCount,
				MsgValue=json,
				RemoteRole=RemotingClient.MiddleTierRole
			});
			return ret;
		}

		/// <summary>Check for appointment signals for a single date.</summary>
		public static bool IsApptRefreshNeeded(DateTime dateTimeShowing,List<Signalod> signalList,List<long> listOpNumsVisible,List<long> listProvNumsVisible) {
			//No need to check MiddleTierRole; no call to db.
			return IsApptRefreshNeeded(dateTimeShowing,dateTimeShowing,signalList,listOpNumsVisible,listProvNumsVisible);
		}

		///<summary>After a refresh, this is used to determine whether the Appt Module needs to be refreshed. Returns true if there are any signals
		///with InvalidType=Appointment where the DateViewing time of the signal falls within the provided daterange, and the signal matches either
		///the list of visible operatories or visible providers in the current Appt Module View. Always returns true if any signals have
		///DateViewing=DateTime.MinVal.</summary>
		public static bool IsApptRefreshNeeded(DateTime startDate,DateTime endDate,List<Signalod> signalList,List<long> listOpNumsVisible,List<long> listProvNumsVisible) {
			//No need to check MiddleTierRole; no call to db.
			//A date range was refreshed.  Easier to refresh all without checking.
			if(signalList.Exists(x => (x.DateViewing.Date==DateTime.MinValue.Date || x.FKeyType==KeyType.PatNum) && x.IType==InvalidType.Appointment)) {
				return true;
			}
			List<Signalod> listApptSignals=signalList.FindAll(x => x.IType==InvalidType.Appointment &&
				x.DateViewing.Date >= startDate.Date && x.DateViewing.Date <= endDate.Date);
			if(listApptSignals.Count==0) {
				return false;
			}
			//List<long> visibleOps = ApptDrawing.VisOps.Select(x => x.OperatoryNum).ToList();
			//List<long> visibleProvs = ApptDrawing.VisProvs.Select(x => x.ProvNum).ToList();
			if(listApptSignals.Any(x=> x.FKeyType==KeyType.Operatory && listOpNumsVisible.Contains(x.FKey))
				|| listApptSignals.Any(x=> x.FKeyType==KeyType.Provider && listProvNumsVisible.Contains(x.FKey))) 
			{
				return true;
			}
			return false;
		}

		/// <summary>Check for schedule signals for a single date.</summary>
		public static bool IsSchedRefreshNeeded(DateTime dateTimeShowing,List<Signalod> signalList,List<long> listOpNumsVisible,
			List<long> listProvNumsVisible) 
		{
			//No need to check MiddleTierRole; no call to db.
			return IsSchedRefreshNeeded(dateTimeShowing,dateTimeShowing,signalList,listOpNumsVisible,listProvNumsVisible);
		}

		///<summary>After a refresh, this is used to determine whether the Appt Module needs to be refreshed.  Returns true if there are any signals
		///with InvalidType=Appointment where the DateViewing time of the signal falls within the provided daterange, and the signal matches either
		///the list of visible operatories or visible providers in the current Appt Module View.  Always returns true if any signals have
		///DateViewing=DateTime.MinVal.</summary>
		public static bool IsSchedRefreshNeeded(DateTime startDate,DateTime endDate,List<Signalod> signalList,List<long> listOpNumsVisible,
			List<long> listProvNumsVisible) 
		{
			//No need to check MiddleTierRole; no call to db.
			//A date range was refreshed.  Easier to refresh all without checking.
			if(signalList.Exists(x => x.DateViewing.Date==DateTime.MinValue.Date && x.IType==InvalidType.Schedules)) {
				return true;
			}
			List<Signalod> listSchedSignals=signalList.FindAll(x => x.IType==InvalidType.Schedules && 
				x.DateViewing.Date >= startDate.Date && x.DateViewing.Date <= endDate.Date);
			if(listSchedSignals.Count==0) {
				return false;
			}
			if(listSchedSignals.Any(x=> x.FKeyType==KeyType.Operatory && listOpNumsVisible.Contains(x.FKey))
				|| listSchedSignals.Any(x=> x.FKeyType==KeyType.Provider && listProvNumsVisible.Contains(x.FKey))
				|| listSchedSignals.Any(x => x.FKeyType==KeyType.Undefined))//For blockouts cleared on a single day.
			{
				return true;
			}
			return false;
		}
	
		///<summary>After a refresh, this is used to determine whether the buttons and listboxes need to be refreshed on the ContrApptPanel. 
		///Will return true with InvalidType==Defs.</summary>
		public static bool IsContrApptButtonRefreshNeeded(List<Signalod> signalList) {
			if(signalList.Exists(x => x.IType==InvalidType.Defs)) {
				return true;
			}
			return false;
		}

		///<summary>After a refresh, this is used to get a list containing all flags of types that need to be refreshed. The FKey must be 0 and the
		///FKeyType must Undefined. Types of Task and SmsTextMsgReceivedUnreadCount are not included.</summary>
		public static InvalidType[] GetInvalidTypes(List<Signalod> signalodList) {
			//No need to check MiddleTierRole; no call to db.
			return signalodList.FindAll(x => x.IType!=InvalidType.Task
					&& x.IType!=InvalidType.TaskPopup
					&& x.IType!=InvalidType.SmsTextMsgReceivedUnreadCount
					&& x.FKey==0
					&& x.FKeyType==KeyType.Undefined)
				.Select(x => x.IType).ToArray();
		}


		///<summary>Our eServices have not been refactored yet to handle granular refreshes yet. This method does include signals that have a FKey. 
		///Ideally this method will be deprecated once eServices uses FKeys in cache refreshes.</summary>
		public static InvalidType[] GetInvalidTypesForWeb(List<Signalod> signalodList) {
			//No need to check MiddleTierRole; no call to db.
			return signalodList.FindAll(x => x.IType!=InvalidType.Task
					&& x.IType!=InvalidType.TaskPopup
					&& x.IType!=InvalidType.SmsTextMsgReceivedUnreadCount)
					//TODO: Future enhancement is to rejoin this method with GetInvalidTypes. To do that we will need to have our eServices refresh parts of 
					//caches based on FKey.
				.Select(x => x.IType).ToArray();
		}

		/// <summary>Won't work with InvalidType.Date, InvalidType.Task, or InvalidType.TaskPopup  yet.</summary>
		public static void SetInvalid(params InvalidType[] arrayITypes) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),arrayITypes);
				return;
			}
			foreach(InvalidType iType in arrayITypes) {
				Signalod sig=new Signalod();
				sig.IType=iType;
				sig.DateViewing=DateTime.MinValue;
				switch(iType) {
					case InvalidType.UserOdPrefs:
						sig.FKey=Security.CurUser?.UserNum??0;
						sig.FKeyType=KeyType.UserOd;
						break;
				}
				Insert(sig);
			}
		}

		///<summary>Insertion logic that doesn't use the cache. Has special cases for generating random PK's and handling Oracle insertions.</summary>
		public static void SetInvalidNoCache(params InvalidType[] itypes) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),itypes);
				return;
			}
			foreach(InvalidType iType in itypes) {
				Signalod sig=new Signalod();
				sig.IType=iType;
				sig.DateViewing=DateTime.MinValue;
				sig.RemoteRole=RemotingClient.MiddleTierRole;
				Crud.SignalodCrud.InsertNoCache(sig);
			}
		}

		///<summary>Must be called after Preference cache has been filled.
		///Deletes all signals older than 2 days if this has not been run within the last week.  Will fail silently if anything goes wrong.</summary>
		public static void ClearOldSignals() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			try {
				DateTime serverDateTime=MiscData.GetNowDateTime();
				if(Prefs.GetContainsKey(PrefName.SignalLastClearedDate.ToString())
					&& PrefC.GetDateT(PrefName.SignalLastClearedDate)>serverDateTime.AddDays(-7) //Has already been run in the past week. This is all server based time.
					&& PrefC.GetDateT(PrefName.SignalLastClearedDate) < serverDateTime) //SignalLastClearedDate isn't in the future job 46490
				{
					return;//Do not run this process again.
				}
				Prefs.UpdateDateT(PrefName.SignalLastClearedDate,serverDateTime);//Set Last cleared to now.
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
			catch(Exception) {
				//fail silently
			}
		}

		///<summary>A helper class that locks access to a HashSet for thread safety.</summary>
		private class ConcurrentHashSet<T> {
			private ReaderWriterLockSlim _readerWriterLockSlim=new ReaderWriterLockSlim();
			private HashSet<T> _hashSet=new HashSet<T>();

			///<summary>Adds the specified element to a set. Returns true if the element is added or false if the element is already present.</summary>
			public bool Add(T item) {
				_readerWriterLockSlim.EnterWriteLock();
				try {
					return _hashSet.Add(item);
				}
				finally {
					_readerWriterLockSlim.ExitWriteLock();
				}
			}

			///<summary>Returns true if the specified element is already present in the set; otherwise, false.</summary>
			public bool Contains(T item) {
				_readerWriterLockSlim.EnterReadLock();
				try {
					return _hashSet.Contains(item);
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
		public Signalod SignalodCur;
		public DateTime DateTimeServer;
	}

	


}




















