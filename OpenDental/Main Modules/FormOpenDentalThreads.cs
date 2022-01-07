using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using DataConnectionBase;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OpenDentBusiness;

namespace OpenDental {
	//Partial class file which houses all thread related business for FormOpenDental.
	public partial class FormOpenDental:FormODBase {
		#region Thread instances. Only keep an instance when necessary for guarding against re-entrace.
		///<summary>Add your thread instance to this list if you only want this thread to only be started once.</summary>
		private List<ODThread> _listOdThreadsRunOnce=new List<ODThread>();
		///<summary>Used internally to await for all threads to exit before attempting to restart.</summary>
		private List<NamedResetEvent> _listThreadExitWaitHandles=new List<NamedResetEvent>();
		private ODThread _odThreadDataConnectionLost;
		private ODThread _odThreadCrashedTableMonitor;
		private ODThread _odThreadMiddleTierConnectionLost;
		private ODThread _odThreadDataReaderNullMonitor;
		#endregion

		///<summary>Starts or stops all local timers and threads that should be started and stopped.
		///Only starts signal timer if interval preference is set to non-zero value.
		///The Windows Forms timer is designed for use in a single-threaded environment which requires this method called from the main UI thread 
		///or marshal / invoke the call onto another thread.</summary>
		private void SetTimersAndThreads(bool doStart) {
			SetTimers(doStart);//The calling method is in charge of invoking or not invoking based on being inside main thread or not.
			SetThreads(doStart);//The calling method is in charge of invoking or not invoking based on being inside main thread or not.
		}

		///<summary>Starts or stops the local timers owned by FormOpenDental.
		///Only starts signal timer if interval preference is set to non-zero value.
		///The Windows Forms timer is designed for use in a single-threaded environment which requires this method called from the main UI thread 
		///or marshal / invoke the call onto another thread.</summary>
		private void SetTimers(bool doStart) {
			if(doStart) {
				if(PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)==0) {
					_hasSignalProcessingPaused=true;
				}
				else {
					timerSignals.Interval=PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)*1000;
					timerSignals.Start();
				}
				timerTimeIndic.Start();
			}
			else {
				timerSignals.Stop();
				timerTimeIndic.Stop();
				_hasSignalProcessingPaused=true;
			}
		}

		///<summary>Either starts all possible threads owned by FormOpenDental or stops a select few threads which are safe to stop.
		///Some threads are not designed to be stopped once they've started.  E.g. heartbeat, data connection lost, etc.</summary>
		private void SetThreads(bool doStart) {
			//We need to lock this entire method because we can't have threads being added while threads are being removed at the same time.
			lock(_listThreadExitWaitHandles) {
				//Make sure our threads from last time are dead and gone.
				if(!_listThreadExitWaitHandles.IsNullOrEmpty()) {
					DateTime dateTimeStart=DateTime.Now;
					TimeSpan timeSpanTotalWait=TimeSpan.FromSeconds(30);
					//WaitAll will throw an exception when called from main thread. Instead we have to loop through each thread individually and WaitOne.
					for(int i=0; i<_listThreadExitWaitHandles.Count; i++) {
						//Accumulate total wait time over all threads and only allow a total of 30 seconds.
						int msRemaining=(int)timeSpanTotalWait.Subtract(DateTime.Now.Subtract(dateTimeStart)).TotalMilliseconds;
						//Always wait at least 1 ms.	
						Logger.LogActionIfOverTimeLimit(
							$"Waiting to stop: {_listThreadExitWaitHandles[i].Name}",
							LogPath.Threads,
							() => { _listThreadExitWaitHandles[i].WaitOne(Math.Max(msRemaining,1)); }
						);					
					}
					_listThreadExitWaitHandles.Clear();
					Logger.LogToPath("Stopped threads",LogPath.Threads,LogPhase.Unspecified);
				}
				if(doStart) {
					Logger.LogToPath("Starting threads",LogPath.Threads,LogPhase.Unspecified);
					Logger.LogActionIfOverTimeLimit("BeginClaimReportThread",LogPath.Threads,() => BeginClaimReportThread());
					Logger.LogActionIfOverTimeLimit("BeginCanadianItransCarrierThread",LogPath.Threads,() => BeginCanadianItransCarrierThread());
					Logger.LogActionIfOverTimeLimit("BeginEServiceMonitorThread",LogPath.Threads,() => BeginEServiceMonitorThread());
					Logger.LogActionIfOverTimeLimit("BeginLogOffThread",LogPath.Threads,() => BeginLogOffThread());
					Logger.LogActionIfOverTimeLimit("BeginODServiceMonitorThread",LogPath.Threads,() => BeginODServiceMonitorThread());
					Logger.LogActionIfOverTimeLimit("BeginUpdateFormTextThread",LogPath.Threads,() => BeginUpdateFormTextThread());
					Logger.LogActionIfOverTimeLimit("BeginWebSyncThread",LogPath.Threads,() => BeginWebSyncThread());
					Logger.LogActionIfOverTimeLimit("BeginComputerHeartbeatThread",LogPath.Threads,() => BeginComputerHeartbeatThread());
					Logger.LogActionIfOverTimeLimit("BeginPodiumThread",LogPath.Threads,() => BeginPodiumThread());
					Logger.LogActionIfOverTimeLimit("BeginEhrCodeListThread",LogPath.Threads,() => BeginEhrCodeListThread());
					Logger.LogActionIfOverTimeLimit("BeginTimeSyncThread",LogPath.Threads,() => BeginTimeSyncThread());
					Logger.LogActionIfOverTimeLimit("BeginVoicemailThread",LogPath.Threads,() => BeginVoicemailThread());
					Logger.LogActionIfOverTimeLimit("BeginHqMetricsThread",LogPath.Threads,() => BeginHqMetricsThread());
					Logger.LogActionIfOverTimeLimit("BeginRegKeyThread",LogPath.Threads,() => BeginRegKeyThread());
					Logger.LogActionIfOverTimeLimit("BeginRegistrationKeyIsDisabledThread",LogPath.Threads,() => BeginRegistrationKeyIsDisabledThread());
					Logger.LogActionIfOverTimeLimit("CheckAlerts",LogPath.Threads,() => CheckAlerts(doRunOnThread: true));
					Logger.LogToPath("Started threads",LogPath.Threads,LogPhase.Unspecified);
					return;
				}
				Logger.LogToPath("Stopping threads",LogPath.Threads,LogPhase.Unspecified);
				Enum.GetValues(typeof(FormODThreadNames)).Cast<FormODThreadNames>().ForEach(threadName => {
					switch(threadName) {
						//Do not kill these.
						case FormODThreadNames.EhrCodeList:
						case FormODThreadNames.RegKeyIsForTesting:
						case FormODThreadNames.ODServiceStarter:
						case FormODThreadNames.ComputerHeartbeat:
						case FormODThreadNames.RegistrationKeyIsDisabled:
						case FormODThreadNames.CrashedTableMonitor:
						case FormODThreadNames.DataConnectionLost:
						case FormODThreadNames.MiddleTierConnectionLost:
							break;
						//Kill these.
						case FormODThreadNames.CanadianItransCarrier:
						case FormODThreadNames.ClaimReport:
						case FormODThreadNames.EServiceMonitoring:
						case FormODThreadNames.HqMetrics:
						case FormODThreadNames.LogOff:
						case FormODThreadNames.ODServiceMonitor:
						case FormODThreadNames.Podium:
						case FormODThreadNames.ReplicationMonitor:
						case FormODThreadNames.UpdateFormText:
						case FormODThreadNames.WebSync:
						case FormODThreadNames.TimeSync:
						case FormODThreadNames.VoicemailHQ:
						case FormODThreadNames.CheckAlerts:
						default:
							//Add these to our list so we can wait on them to exit before restarting the threads next time.
							_listThreadExitWaitHandles.AddRange(ODThread.QuitAsyncThreadsByGroupName(threadName.GetDescription()));
							break;
					}
				});
			}
		}

		///<summary>Checks to see if there is a thread running with the passed in group name. Will return true if there is or if a thread that is only set to run once has already ran. 
		///Will return false if no thread is running.</summary>
		private bool IsThreadAlreadyRunning(FormODThreadNames threadName) {
			if(_listOdThreadsRunOnce.Any(x => x.GroupName==threadName.GetDescription())) {
				return true;
			}
			List<ODThread> listThreads=ODThread.GetThreadsByGroupName(threadName.GetDescription());
			return !listThreads.IsNullOrEmpty();
		}		

		#region CanadianItransCarrierThread

		private void BeginCanadianItransCarrierThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.CanadianItransCarrier)) {
				return;
			}
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada
				return;
			}
			ODThread odThread=new ODThread((int)TimeSpan.FromHours(1).TotalMilliseconds,(o) => {
				ItransNCpl.TryCarrierUpdate();
			});
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.GroupName=FormODThreadNames.CanadianItransCarrier.GetDescription();
			odThread.Name=FormODThreadNames.CanadianItransCarrier.GetDescription();
			odThread.Start();
		}

		#endregion
		#region CheckAlertsThread

		///<summary>May begin a thread that checks for alerts and update the main alerts tool bar menu.
		///Pass false to doRunOnThread if you want to run alerts on the main thread.</summary>
		private void CheckAlerts(bool doRunOnThread=false) {
			if(doRunOnThread && IsThreadAlreadyRunning(FormODThreadNames.CheckAlerts)) {
				return;
			}
			ODThread.WorkerDelegate getAlerts=new ODThread.WorkerDelegate((o) => {
				DateTime dtInactive=Security.DateTimeLastActivity.AddMinutes(PrefC.GetInt(PrefName.AlertInactiveMinutes));
				if(PrefC.GetInt(PrefName.AlertInactiveMinutes)!=0 && DateTime.Now>dtInactive) {
					return;//user has been inactive for a while, so stop checking alerts.
				}
				long clinicNumCur=Clinics.ClinicNum;
				long userNumCur=Security.CurUser.UserNum;
				Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
				List<List<AlertItem>> listUniqueAlerts=AlertItems.GetUniqueAlerts(userNumCur,clinicNumCur);
				//We will set the alert's tag to all the items in its list so that all can be marked read/deleted later.
				listUniqueAlerts.ForEach(x => x.First().TagOD=x.Select(y => y.AlertItemNum).ToList());
				List<AlertItem> listAlertItems=listUniqueAlerts.Select(x => x.First())
					.Where(x => x.Type!=AlertType.ClinicsChangedInternal).ToList();//These alerts are not supposed to be displayed to the end user.
				//Update listUserAlertTypes to only those with active AlertItems.
				List<AlertType> listUserAlertLinks=listAlertItems.Select(x => x.Type).ToList();
				List<AlertRead> listAlertItemReads=AlertReads.RefreshForAlertNums(userNumCur,listAlertItems.Select(x => x.AlertItemNum).ToList());
				this.InvokeIfRequired(() => {
					//Assigning this inside Invoke so that we don't have to lock _listAlertItems and _listAlertReads.
					_listAlertItems=listAlertItems;
					_listAlertReads=listAlertItemReads;
					AddAlertsToMenu();
				});
			});
			if(!doRunOnThread) {
				getAlerts(null);
				return;
			}
			int checkAlertsIntervalMS=(int)TimeSpan.FromSeconds(PrefC.GetInt(PrefName.AlertCheckFrequencySeconds)).TotalMilliseconds;
			if(checkAlertsIntervalMS==0) {
				//Office has disabled alert checking. We won't periodically check alerts, but we will do it when the user does something alert related.
				return;
			}
			ODThread odThread=new ODThread(checkAlertsIntervalMS,getAlerts);
			odThread.AddExceptionHandler((ex) => ex.DoNothing());
			odThread.GroupName=FormODThreadNames.CheckAlerts.GetDescription();
			odThread.Name=FormODThreadNames.CheckAlerts.GetDescription();
			odThread.Start(true);
		}

		#endregion
		#region ClaimReportThread

		///<summary>If the local computer is the computer where claim reports are retrieved then this thread runs in the background and will retrieve
		///and import reports for the default clearinghouse or for clearinghouses where both the Payors field is not empty plus the Eformat matches the
		///region the user is in.  If an error is returned from the importation, this thread will silently fail.</summary>
		private void BeginClaimReportThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.ClaimReport)) {
				return;
			}
			if(PrefC.GetBool(PrefName.ClaimReportReceivedByService)) {
				return;
			}
			int claimReportRetrieveIntervalMS=(int)TimeSpan.FromMinutes(PrefC.GetInt(PrefName.ClaimReportReceiveInterval)).TotalMilliseconds;
			ODThread odThread=new ODThread(claimReportRetrieveIntervalMS,(o) => {
				string claimReportComputer=PrefC.GetString(PrefName.ClaimReportComputerName);
				if(claimReportComputer=="" || claimReportComputer!=Dns.GetHostName()) {
					return;
				}
				Clearinghouses.RetrieveReportsAutomatic(false);//only run for the selected clinic, if clinics are enabled
			});
			odThread.AddExceptionHandler(ex => ex.DoNothing());
			odThread.GroupName=FormODThreadNames.ClaimReport.GetDescription();
			odThread.Name=FormODThreadNames.ClaimReport.GetDescription();
			odThread.Start();
		}

		#endregion
		#region ComputerHeartbeatThread

		private void BeginComputerHeartbeatThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.ComputerHeartbeat)) {
				return;
			}
			ODThread threadCompHeartbeat=new ODThread(180000,o => {//Every three minutes
				ODException.SwallowAnyException(() => {
					Computers.UpdateHeartBeat(ODEnvironment.MachineName,false);
					ActiveInstances.Upsert(Security.CurUser.UserNum,Computers.GetCur().ComputerNum,Process.GetCurrentProcess().Id);
				});
			});
			threadCompHeartbeat.AddExceptionHandler((e) => e.DoNothing());
			threadCompHeartbeat.GroupName=FormODThreadNames.ComputerHeartbeat.GetDescription();
			threadCompHeartbeat.Name=FormODThreadNames.ComputerHeartbeat.GetDescription();
			threadCompHeartbeat.Start();
		}

		#endregion
		#region CrashedTableMonitorThread

		private void BeginCrashedTableMonitorThread(CrashedTableEventArgs e) {
			if(_odThreadCrashedTableMonitor!=null) {
				return;
			}
			_odThreadCrashedTableMonitor=new ODThread((o) => {
				string errorMessage=(string)e.Tag;
				Func<bool> funcShouldWindowClose=() => {
					if(DataConnection.IsTableCrashed(e.TableName)) {
						return false;//The table is still marked as crashed so do not close the Connection Lost window.
					}
					else {
						//We have detected that the table is no longer marked as crashed so let everyone know and then close the Connection Lost window.
						CrashedTableEvent.Fire(new CrashedTableEventArgs(false,e.TableName));
						return true;
					}
				};
				using FormConnectionLost FormCL=new FormConnectionLost(funcShouldWindowClose,ODEventType.CrashedTable,errorMessage,typeof(CrashedTableEvent));
				if(FormCL.ShowDialog()==DialogResult.Cancel) {
					ExitCode=108;//User decided to exit the program due to a crashed table UE.
					Environment.Exit(ExitCode);
					return;
				}
			});
			_odThreadCrashedTableMonitor.AddExceptionHandler((ex) => ex.DoNothing());
			_odThreadCrashedTableMonitor.AddExitHandler((ex) => _odThreadCrashedTableMonitor=null);
			_odThreadCrashedTableMonitor.GroupName=FormODThreadNames.CrashedTableMonitor.GetDescription();
			_odThreadCrashedTableMonitor.Name=FormODThreadNames.CrashedTableMonitor.GetDescription();
			_odThreadCrashedTableMonitor.Start();
		}

		#endregion
		#region DataConnectionLostThread

		private void BeginDataConnectionLostThread(DataConnectionEventArgs e) {
			if(_odThreadDataConnectionLost!=null) {
				return;
			}
			_odThreadDataConnectionLost=new ODThread((o) => {
				//Stop all appropriate threads and open the Connection Lost window.
				//It is not safe to stop timers at this point because we would need to invoke back over to the main thread which is waiting in a Join().
				Logger.LogToPath("Database connection lost",LogPath.Threads,LogPhase.Unspecified); 
				SetThreads(false);//Only stop threads because the main thread is locked waiting for this thread to finish, which means ticks cannot fire.
				string errorMessage=(string)e.Tag;
				Func<bool> funcTestConnection=() => {
					using(DataConnection dconn=new DataConnection()) {
						try {
							dconn.SetDb(e.ConnectionString,"",DataConnection.DBtype);
							//Tell everyone that the data connection has been found.
							Logger.LogToPath("Database connection restored",LogPath.Threads,LogPhase.Unspecified);
							DataConnectionEvent.Fire(new DataConnectionEventArgs(DataConnectionEventType.ConnectionRestored,true,e.ConnectionString));
						}
						catch(Exception ex) {
							ex.DoNothing();
							return false;//Data connection is still lost so do not close the Connection Lost window.
						}
					}
					return true;//Data connection has been found so close the Connection Lost window.
				};
				using FormConnectionLost FormCL=new FormConnectionLost(funcTestConnection,ODEventType.DataConnection,errorMessage);
				if(FormCL.ShowDialog()==DialogResult.Cancel) {
					//This is problematic because it causes DirectX to cause a UE but there doesn't seem to be a better way to close without using the database.
					ExitCode=106;//Connection to specified database has failed
					Environment.Exit(ExitCode);
					return;
				}
			});
			//Add exception handling just in case MySQL is unreachable at any point in the lifetime of this session.
			_odThreadDataConnectionLost.AddExceptionHandler((ex) => ex.DoNothing());
			_odThreadDataConnectionLost.AddExitHandler((ex) => {
				//Restart our threads no matter what happened.  If we're killing the program this won't matter anyway.
				SetThreads(true);//Start the threads because they were the only ones stopped, the timers were locked up via a Join() on the main thread.
				//Important that this only get set to null AFTER we have restarted the threads. This our re-entrance blocker at the top of this method.
				_odThreadDataConnectionLost=null;
			});
			_odThreadDataConnectionLost.GroupName=FormODThreadNames.DataConnectionLost.GetDescription();
			_odThreadDataConnectionLost.Name=FormODThreadNames.DataConnectionLost.GetDescription();
			_odThreadDataConnectionLost.Start();
		}

		#endregion
		#region DataReaderNullMonitorThread

		private void BeginDataReaderNullMonitorThread(DataReaderNullEventArgs e) {
			if(_odThreadDataReaderNullMonitor!=null) {
				return;
			}
			_odThreadDataReaderNullMonitor=new ODThread(o => {
				string errorMessage=(string)e.Tag;
				using FormConnectionLost FormCL=new FormConnectionLost(e.FuncRetryQuery,e.EventType,errorMessage,typeof(DataReaderNullEvent));
				if(FormCL.ShowDialog()==DialogResult.Cancel) {
					Environment.Exit(ExitCode);
					return;
				}
			});
			_odThreadDataReaderNullMonitor.AddExceptionHandler((ex) => ex.DoNothing());
			_odThreadDataReaderNullMonitor.AddExitHandler((ex) => _odThreadDataReaderNullMonitor=null);
			_odThreadDataReaderNullMonitor.GroupName=FormODThreadNames.DataReaderNullMonitor.GetDescription();
			_odThreadDataReaderNullMonitor.Name=FormODThreadNames.DataReaderNullMonitor.GetDescription();
			_odThreadDataReaderNullMonitor.Start();
		}

		#endregion DataReaderNullMonitorThread
		#region EhrCodeListThread

		///<summary>This begins a thread that loads the EHR.dll in the background.</summary>
		private void BeginEhrCodeListThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.EhrCodeList)) {
				return;
			}
			//For EHR users we want to load up the EHR code list from the obfuscated dll in a background thread because it takes roughly 11 seconds to load up.
			if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				return;
			}
			ODThread odThread=new ODThread(o => {
				//In regards to throwing, this should never happen.  It would most likely be due to a corrupt dll issue but I don't want to stop the 
				//start up sequence. Users could theoretically use Open Dental for an entire day and never hit the code that utilizes the EhrCodes class.
				//Therefore, we do not want to cause any issues and the worst case scenario is the users has to put up with the 11 second delay (old behavior).
				ODException.SwallowAnyException(() => {
					EhrCodes.UpdateList();
				});
			});
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.GroupName=FormODThreadNames.EhrCodeList.GetDescription();
			odThread.Name=FormODThreadNames.EhrCodeList.GetDescription();
			odThread.Start();
			_listOdThreadsRunOnce.Add(odThread);
		}

		#endregion
		#region EServiceMonitorThread

		///<summary>Starts the eService monitoring thread that will run once a minute.  Only runs if the user currently logged in has the eServices permission.</summary>
		private void BeginEServiceMonitorThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.EServiceMonitoring)) {
				return;
			}
			//If the user currently logged in has permission to view eService settings, turn on the listener monitor.
			if(Security.CurUser==null || !Security.IsAuthorized(Permissions.EServicesSetup,true)) {
				return;//Do not start the listener service monitor for users without permission.
			}
			//Process any Error signals that happened due to an update:
			EServiceSignals.ProcessErrorSignalsAroundTime(PrefC.GetDateT(PrefName.ProgramVersionLastUpdated));
			//Create a separate thread that will run every 60 seconds to monitor eService signals.
			ODThread odThread=new ODThread(60000,EServiceMonitorWorker);
			//Currently we don't want to do anything if the eService signal processing fails.  Simply try again in a minute.  
			//Most likely cause for exceptions will be database IO when computers are just sitting around not doing anything.
			//Implementing this delegate allows us to NOT litter ProcessEServiceSignals() with try catches.  
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.GroupName=FormODThreadNames.EServiceMonitoring.GetDescription();
			odThread.Name=FormODThreadNames.EServiceMonitoring.GetDescription();
			odThread.Start();
		}

		///<summary>Worker method for eServiceMonitorThread.  Call BeginEServiceMonitorThread() to start monitoring eService signals instead of calling 
		///this method directly. This thread's only job is to check to see if the eConnector's current status is critical and if it is critical, 
		///create a High severity alert.</summary>
		private void EServiceMonitorWorker(ODThread odThread) {
			//The listener service will have a local heartbeat every 5 minutes so it's overkill to check every time timerSignals_Tick fires.
			//Only check the Listener Service status once a minute.
			//The downside to doing this is that the menu item will stay red up to one minute when a user wants to stop monitoring the service.
			eServiceSignalSeverity listenerStatus = EServiceSignals.GetListenerServiceStatus();
			if(listenerStatus==eServiceSignalSeverity.None) {
				//This office has never had a valid listener service running and does not have more than 5 patients set up to use the listener service.
				//Quit the thread so that this computer does not waste its time sending queries to the server every minute.
				odThread.QuitAsync();
				return;
			}
			if(listenerStatus!=eServiceSignalSeverity.Critical) { //Not a critical event so no need to continue.
				return;
			}
			if(AlertItems.RefreshForType(AlertType.EConnectorDown).Count>0) { //Alert already exists to no need to continue.
				return;
			}
			//Create an alert.
			AlertItems.Insert(new AlertItem {
				//Do not allow delete. The only way for this alert to be deleted is for the eConnector to insert a heartbeat, which will in-turn delete this alert.
				Actions=ActionType.MarkAsRead|ActionType.OpenForm,
				Description=Lans.g("EConnector","eConnector needs to be restarted"),
				Severity=SeverityType.High,
				Type=AlertType.EConnectorDown,
				//Show for all clinics.
				ClinicNum=-1,
				FormToOpen=FormType.FormEServicesEConnector,
			});
			//We just inserted an alert so update the alert menu.
			CheckAlerts();
		}
		
		#endregion
		#region HqMetricsThread

		private void BeginHqMetricsThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.HqMetrics)) {
				return;
			}
			if(!PrefC.IsODHQ) {
				return;
			}
			//Only run this thread every 1.6 seconds.
			ODThread odThread=new ODThread(1600,(o) => {
				ProcessHqMetricsPhones();
				ProcessHqMetricsEServices();
			});
			odThread.AddExceptionHandler(ex => ex.DoNothing());
			odThread.GroupName=FormODThreadNames.HqMetrics.GetDescription();
			odThread.Name=FormODThreadNames.HqMetrics.GetDescription();
			odThread.Start();
		}

		///<summary>HQ only. Called from HqMetricsThread(). Deals with HQ phone panel. This method runs in a thread so any access to form controls must be invoked. Gets hit every 1.6 seconds from each workstation.</summary>
		private void ProcessHqMetricsPhones() {
			if(Security.CurUser==null) {
				return;//Don't waste time processing phone metrics when no one is logged in and sitting at the log on screen.
			}
			if(_listFormMapHQs.Count>0 
				&& DateTime.Now.Subtract(_dateHqOfficeDownLastRefreshed).TotalSeconds>PrefC.GetInt(PrefName.ProcessSigsIntervalInSecs)) 
			{
				List<OpenDentBusiness.Task> listOfficesDowns=Tasks.GetOfficeDowns();
				if(!IsDisposed) {
					Invoke(new ProcessOfficeDownArgs(ProcessOfficeDowns),new object[] { listOfficesDowns });
				}
				_dateHqOfficeDownLastRefreshed=DateTime.Now;
			}
			List<WebChatSession> listWebChatSessions=null;
			List<PeerInfo> listRemoteSupportSessions=null;
			TriageMetric triageMetrics=Phones.GetTriageMetrics();
			ODException.SwallowAnyException(() => Invoke(new FillTriageLabelsResultsArgs(OnFillTriageLabelsResults),triageMetrics));
			//Attempt to deserialize the active web chat sessions from the triage metric row.
			ODException.SwallowAnyException(() => listWebChatSessions=JsonConvert.DeserializeObject<List<WebChatSession>>(triageMetrics.WebChatSessions));
			//Attempt to deserialize the active web chat sessions from the triage metric row.
			ODException.SwallowAnyException(() => {
				listRemoteSupportSessions=JsonConvert.DeserializeObject<List<PeerInfo>>(triageMetrics.RemoteSupportSessions);
				PeerInfos.SetEmployeeNum(ref listRemoteSupportSessions);
			});
			if(listWebChatSessions==null) {//An empty string value within triageMetrics.WebChatSessions will deserialize to null.
				listWebChatSessions=new List<WebChatSession>();
			}
			if(listRemoteSupportSessions==null) {//An empty string value within triageMetrics.RemoteSupportSessions will deserialize to null.
				listRemoteSupportSessions=new List<PeerInfo>();
			}
			List<PhoneEmpDefault> listPED=PhoneEmpDefaults.GetDeepCopy();
			List<PhoneEmpSubGroup> listSubGroups=PhoneEmpSubGroups.GetAll();
			//Get the extension linked to this machine.
			PhoneComp phoneComp=PhoneComps.GetFirstOrDefault(x => x.ComputerName.ToUpper()==Environment.MachineName.ToUpper());
			int extension=phoneComp?.PhoneExt??0;
			//Get the phoneempdefault row that is currently associated to the corresponding extension.
			PhoneEmpDefault pedCur=listPED.FirstOrDefault(x => x.PhoneExt==extension);
			bool isTriageOperator=pedCur?.IsTriageOperator??false;
			//Now get the Phone object for this extension. Phone table matches PhoneEmpDefault table more or less 1:1. 
			//Phone fields represent current state of the PhoneEmpDefault table and will be modified by the phone tracking server anytime a phone state changes for a given extension 
			//(EG... incoming call, outgoing call, hangup, etc).
			List<Phone> listPhones=Phones.GetPhoneList();
			Phone phone=listPhones.FirstOrDefault(x => x.Extension==extension);
			List<ChatUser> listChatUsers=ChatUsers.GetAll();
			//send the results back to the UI layer for action.
			if(!this.IsDisposed) {
				this.Invoke(() => ProcessHqMetricsResults(listPED,listPhones,listSubGroups,listChatUsers,phone,isTriageOperator,listWebChatSessions,
					listRemoteSupportSessions));
			}
		}

		///<summary>HQ only. Called from ProcessHqMetrics(). Deals with HQ EServices. This method runs in a thread so any access to form controls must be invoked.</summary>
		private void ProcessHqMetricsEServices() {
			if(DateTime.Now.Subtract(_dateTimeHqEServiceMetricsLastRefreshed).TotalSeconds<10) {
				return;
			}
			if(_listFormMapHQs.Count==0) { //Do not run if the HQ map is not open.
				return;
			}
			_dateTimeHqEServiceMetricsLastRefreshed=DateTime.Now;
			//Get important metrics from serviceshq db.
			EServiceMetrics metricsToday=EServiceMetrics.GetEServiceMetricsFromSignalHQ();
			if(metricsToday==null) {
				return;
			}
			foreach(FormMapHQ formMapHQ in _listFormMapHQs) {
				formMapHQ.Invoke(new MethodInvoker(delegate { formMapHQ.SetEServiceMetrics(metricsToday); }));
			}
		}

		#endregion
		#region LogOffThread

		///<summary>Begins the thread that checks for a forced log off.</summary>
		private void BeginLogOffThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.LogOff)) {
				return;
			}
			ODThread odThread=new ODThread((int)TimeSpan.FromSeconds(15).TotalMilliseconds,(o) => { LogOffWorker(); });
			//Do not add an exception handler for the log off thread.  If it fails for any unhandled reason then the program should crash.
			odThread.GroupName=FormODThreadNames.LogOff.GetDescription();
			odThread.Name=FormODThreadNames.LogOff.GetDescription();
			odThread.Start();
		}

		///<summary>Thread set to run every 15 seconds. This interval must be longer than the interval of the timer in FormLogoffWarning (10s), or it will go into a loop.</summary>
		private void LogOffWorker() {
			int logOffTimerMins=0;//Default the automatic log off feature to off until the preference cache / MySQL can be reached.
			try {
				logOffTimerMins=PrefC.LogOffTimer;
			}
			catch(MySqlException ex) {
				//The thread in charge of this worker does not have an exception handler registered on purpose.
				//There are certain exceptions that can be thrown when accessing PrefC.LogOffTimer that should crash the program.
				//However, failing to communicate with MySQL should not cause the program to crash.
				ex.DoNothing();
			}
			if(logOffTimerMins==0) {
				return;
			}
			if(this.InvokeRequired) {
				//Invoke here as the following uses Application wide variables and accesses UI elements when logging off.
				this.Invoke(() => LogOffWorker());
				return;
			}
			for(int f=Application.OpenForms.Count-1;f>=0;f--) {//This checks if any forms are open that make us not want to automatically log off. Currently only FormTerminal is checked for.
				Form openForm;
				try {
					openForm=Application.OpenForms[f];
				}
				catch(Exception ex) {
					ex.DoNothing();//We have received a bug submission for an index out of range exception from the above line.
					continue;
				}
				//If anything is in progress we should halt the autologoff. After the window finishes, this will get hit after a maximum of 15 seconds and perform the auto-logoff.
				if(ListTools.In(openForm.Name,new List<string>{ "FormTerminal","FormProgress","ProgressOD","FormProgressAuto" })){
					return;
				}
			}
			//Warning.  When debugging this, the ActiveForm will be impossible to determine by setting breakpoints.
			//string activeFormText=Form.ActiveForm.Text;
			//If a breakpoint is set below here, ActiveForm will erroneously show as null.
			Form formActive=Form.ActiveForm;
			if(formActive==null) {//some other program has focus
				_formRecentlyOpenForLogoff=null;
				//Do not alter IsFormLogOnLastActive because it could still be active in background.
			}
			else if(formActive==this) {//main form active
				_formRecentlyOpenForLogoff=null;
				//User must have logged back in so IsFormLogOnLastActive should be false.
				_isFormLogOnLastActive=false;
			}
			else {//Some Open Dental dialog is active.
				if(formActive==_formRecentlyOpenForLogoff) {
					//The same form is active as last time, so don't add events again.
					//The active form will now be constantly resetting the dateTimeLastActivity.
				}
				else {//this is the first time this form has been encountered, so attach events and don't do anything else
					_formRecentlyOpenForLogoff=formActive;
					Security.DateTimeLastActivity=DateTime.Now;
					//Flag FormLogOn as the active form so that OD doesn't continue trying to log the user off when using the web service.
					if(formActive.GetType()==typeof(FormLogOn)) {
						_isFormLogOnLastActive=true;
					}
					else {
						_isFormLogOnLastActive=false;
					}
					return;
				}
			}
			DateTime dtDeadline=Security.DateTimeLastActivity+TimeSpan.FromMinutes(logOffTimerMins);
			//Debug.WriteLine("Now:"+DateTime.Now.ToLongTimeString()+", Deadline:"+dtDeadline.ToLongTimeString());
			if(DateTime.Now<dtDeadline) {
				return;
			}
			if(Security.CurUser==null) {//nobody logged on
				return;
			}
			//The above check works unless using web service.  With web service, CurUser is not set to null when FormLogOn is shown.
			if(_isFormLogOnLastActive) {//Don't try to log off a user that is already logged off.
				return;
			}
			using FormLogoffWarning formW=new FormLogoffWarning();
			formW.ShowDialog();
			if(formW.DialogResult!=DialogResult.OK) {
				Security.DateTimeLastActivity=DateTime.Now;
				return;//user hit cancel, so don't log off
			}
			//User could be working outside of OD and the Log On window will never become "active" so we set it here for a fail safe.
			_isFormLogOnLastActive=true;
			//WE are inside of an Invoke call here and we are on the main UI thread.
			//Launch the LogOffNow() inside a new thread so that we can leave the UI thread to allow other invoke calls to execute inside LogOffNow().
			//Invoke calls which are nested are delayed until the parent invoke finishes.
			//We need form Close() to cause ShowDialog() to return immediately instead of after leaving LogOffNow().
			ODThread thread=new ODThread((o) => {
				ODException.SwallowAnyException(() => {
					LogOffNow(true);
				});
			});
			thread.Start();
		}

		#endregion
		#region MiddleTierConnectionMonitorThread

		private void BeginMiddleTierConnectionMonitorThread(MiddleTierConnectionEventArgs e) {
			if(_odThreadMiddleTierConnectionLost!=null) {
				return;
			}
			RemotingClient.HasMiddleTierConnectionFailed=true;
			_odThreadMiddleTierConnectionLost=new ODThread((o) => {
				//Stop all appropriate threads and open the Connection Lost window.
				//It is not safe to stop timers at this point because we would need to invoke back over to the main thread which is waiting in a Join().
				Logger.LogToPath("Middle-Tier connection lost",LogPath.Threads,LogPhase.Unspecified);
				SetThreads(false);//Only stop threads because the main thread is locked waiting for this thread to finish, which means ticks cannot fire.
				string errorMessage=(string)e.Tag;
				Func<bool> funcShouldWindowClose=() => {
					//Very simple MiddleTier communication, small payload.
					if(RemotingClient.IsMiddleTierAvailable()) {
						//Tell everyone that the Middle Tier is back online.
						RemotingClient.HasMiddleTierConnectionFailed=false;
						Logger.LogToPath("Middle-Tier connection restored",LogPath.Threads,LogPhase.Unspecified);
						MiddleTierConnectionEvent.Fire(new MiddleTierConnectionEventArgs(true));
						return true;//Middle Tier is back online so close the Connection Lost window.
					}
					else {
						return false;//Middle Tier is not back online yet so don't close the Connection Lost window.
					}
				};
				using FormConnectionLost FormCL=new FormConnectionLost(funcShouldWindowClose,ODEventType.MiddleTierConnection,errorMessage
					,typeof(MiddleTierConnectionEvent));
				//Halt the thread here until clicking 'Retry' is successful, clicking 'Exit Program', or an event fires indicating connection is back.
				if(FormCL.ShowDialog()==DialogResult.Cancel) {
					//This is problematic because it causes DirectX to cause a UE but there doesn't seem to be a better way to close without using the database.
					ExitCode=107;//Connection to the Middle Tier has failed
					Environment.Exit(ExitCode);
					return;
				}
			});
			_odThreadMiddleTierConnectionLost.AddExceptionHandler((ex) => ex.DoNothing());
			_odThreadMiddleTierConnectionLost.AddExitHandler((ex) => {
				_odThreadMiddleTierConnectionLost=null;
				//Restart our threads no matter what happened.  If we're killing the program this won't matter anyway.
				SetThreads(true);//Start the threads because they were the only ones stopped, the timers were locked up via a Join() on the main thread.
			});
			_odThreadMiddleTierConnectionLost.GroupName=FormODThreadNames.MiddleTierConnectionLost.GetDescription();
			_odThreadMiddleTierConnectionLost.Name=FormODThreadNames.MiddleTierConnectionLost.GetDescription();
			_odThreadMiddleTierConnectionLost.Start();
		}

		#endregion
		#region ODServiceMonitorThread

		///<summary>Begins a thread that monitor's the Open Dental Service heartbeat and alerts the user if the service is not running.</summary>
		private void BeginODServiceMonitorThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.ODServiceMonitor)) {
				return;
			}
			ODThread threadOpenDentalServiceCheck=new ODThread((int)TimeSpan.FromMinutes(10).TotalMilliseconds,
				(o) => { AlertItems.CheckODServiceHeartbeat(); });
			threadOpenDentalServiceCheck.AddExceptionHandler(ex => ex.DoNothing());
			threadOpenDentalServiceCheck.GroupName=FormODThreadNames.ODServiceMonitor.GetDescription();
			threadOpenDentalServiceCheck.Name=FormODThreadNames.ODServiceMonitor.GetDescription();
			threadOpenDentalServiceCheck.Start();
		}

		#endregion
		#region ODServiceStarterThread

		///<summary>Spawns a thread that attempts to start all Open Dental services.</summary>
		private void BeginODServiceStarterThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.ODServiceStarter)) {
				return;
			}
			ODThread odThread=new ODThread((o) => {
				if(PrefC.GetString(PrefName.WebServiceServerName)!="" && ODEnvironment.IdIsThisComputer(PrefC.GetString(PrefName.WebServiceServerName))) {
					//An InvalidOperationException can get thrown if services could not start.  E.g. current user is not running Open Dental as an 
					//administrator.	We do not want to halt the startup sequence here.  If we want to notify customers of a downed service, there needs to 
					//be an additional monitoring service installed.
					ServicesHelper.StartServices(ServicesHelper.GetAllOpenDentServices());
				}
			});
			//If the thread that attempts to start all Open Dental services fails for any reason, silently fail.
			odThread.AddExceptionHandler(ex => ex.DoNothing());
			odThread.GroupName=FormODThreadNames.ODServiceStarter.GetDescription();
			odThread.Name=FormODThreadNames.ODServiceStarter.GetDescription();
			odThread.Start(true);
		}

		///<summary>Spawns a thread that attempts to start the Patient Dashboard.</summary>
		private void BeginODDashboardStarterThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.Dashboard)) {
				return;
			}
			ODThread odThread=new ODThread((o) => {
				RefreshMenuDashboards();
				if(Security.CurUser!=null) {
					InitDashboards(Security.CurUser.UserNum);
				}
			});
			//If the thread that attempts to start Open Dental dashboard fails for any reason, silently fail.
			odThread.AddExceptionHandler(ex => {
				ex.DoNothing();
				if(Security.CurUser!=null && Security.CurUser.UserNum!=0) {//Defensive to ensure all Patient Dashboard userprefs are not deleted.
					UserOdPrefs.DeleteForValueString(Security.CurUser.UserNum,UserOdFkeyType.Dashboard,string.Empty);//All Dashboard userodprefs for this user.
				}
			});
			odThread.GroupName=FormODThreadNames.Dashboard.GetDescription();
			odThread.Name=FormODThreadNames.Dashboard.GetDescription();
			odThread.Start(true);
		}

		#endregion
		#region PhoneConferenceThread

		///<summary>Begins a thread that updates the phone conference rooms. This is for HQ only.</summary>
		void BeginPhoneConferenceThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.PhoneConference)) {
				return;
			}
			ODThread odThread=new ODThread((o) => {
				Logger.LogToPath("PhoneConf",LogPath.Signals,LogPhase.Start);
				List<PhoneConf> listPhoneConfs=PhoneConfs.GetAll();
				this.Invoke((() => lightSignalGrid1.SetConfs(listPhoneConfs)));
				Logger.LogToPath("PhoneConf",LogPath.Signals,LogPhase.End);
			});
			odThread.AddExceptionHandler(ex => SignalsTickExceptionHandler(ex));
			odThread.GroupName=FormODThreadNames.PhoneConference.GetDescription();
			odThread.Name=FormODThreadNames.PhoneConference.GetDescription();
			odThread.Start();
		}

		#endregion
		#region PlaySoundsThread

		///<summary>Begins a thread that will play sounds based on the given signals.</summary>
		private void BeginPlaySoundsThread(List<SigMessage> listSigMessages) {
			//Do not check if the thread is already running. If there are more sounds to play, play them. 
			ODThread odThread=new ODThread((o) => PlaySoundsWorker(listSigMessages));
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.GroupName=FormODThreadNames.PlaySounds.GetDescription();
			odThread.Name=FormODThreadNames.PlaySounds.GetDescription();
			odThread.Start();
		}

		private void PlaySoundsWorker(List<SigMessage> listSigMessages) {
			//loop through each signal
			foreach(SigMessage sigMessage in listSigMessages) { 
				if(sigMessage.AckDateTime.Year > 1880) {
					continue;//don't play any sounds for acks.
				}
				//play all the sounds.
				List<SigElementDef> listSigElementDefs=SigElementDefs.GetDefsForSigMessage(sigMessage);
				foreach(SigElementDef sigElement in listSigElementDefs) {
					if(sigElement.Sound=="") {
						continue;
					}
					ODException.SwallowAnyException(() => {
						byte[] rawData=Convert.FromBase64String(sigElement.Sound);
						SoundHelper.PlaySoundSync(rawData);//sound will finish playing before thread continues
					});
				}
				Thread.Sleep(1000);//pause 1 second between signals.
			}
		}

		#endregion
		#region PodiumThread

		///<summary>If the local computer is the computer where Podium invitations are sent, then this thread runs in the background and checks for 
		///appointments that started 10-40 minutes ago (depending on in the patient is a new patient) at 10 minute intervals.  No preferences.
		///In the future, some sort of identification should be made to tell if this thread is running on any computer.</summary>
		private void BeginPodiumThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.Podium)) {
				return;
			}
			ODThread odThread=new ODThread(Podium.PodiumThreadIntervalMS,((ODThread o) => { Podium.ThreadPodiumSendInvitations(false); }));
			odThread.AddExceptionHandler((ex) => {
				Logger.WriteException(ex,Podium.LOG_DIRECTORY_PODIUM);
			});
			odThread.GroupName=FormODThreadNames.Podium.GetDescription();
			odThread.Name=FormODThreadNames.Podium.GetDescription();
			odThread.Start();
		}

		#endregion
		#region RegKeyThread

		///<summary>Begins a thread that checks if the thread key is for testing. Also makes a call to check and update customer version history if 
		///recently updated. This thread is ran only once.</summary>
		private void BeginRegKeyThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.RegKeyIsForTesting)) {
				return;
			}
			ODThread odThread=new ODThread(o => {
				IsRegKeyForTesting=PrefL.IsRegKeyForTesting();
				WebServiceMainHQProxy.UpdateCustomerVersionHistory();
			});
			odThread.AddExceptionHandler(ex => ex.DoNothing());//silently fail.
			odThread.GroupName=FormODThreadNames.RegKeyIsForTesting.GetDescription();
			odThread.Name=FormODThreadNames.RegKeyIsForTesting.GetDescription();
			odThread.Start(true);
			_listOdThreadsRunOnce.Add(odThread);
		}

		#endregion
		#region ShutdownThread

		///<summary>Begins a thread that shutsdown Open Dental.</summary>
		private void BeginShutdownThread() {
			//If this thread is already running, do not start another one.
			if(IsThreadAlreadyRunning(FormODThreadNames.Shutdown)) {
				return;
			}
			ODThread odThread=new ODThread((o) => {
				if(PrefC.IsODHQ) {
					ODThread webCamKillThread = new ODThread(((o2) => { Process.GetProcessesByName("WebCamOD").ToList().ForEach(x => x.Kill()); }));
					webCamKillThread.AddExceptionHandler((ex) => ex.DoNothing());
					webCamKillThread.Start();
					ODThread proximityKillThread = new ODThread(((o2) => { Process.GetProcessesByName("ProximityOD").ToList().ForEach(x => x.Kill()); }));
					proximityKillThread.AddExceptionHandler((ex) => ex.DoNothing());
					proximityKillThread.Start();
				}
				Thread.Sleep(15000);//15 seconds
				CloseOpenForms(true);
				this.Invoke(Application.Exit);
			});
			//Do not add an exception handler for the shutdown thread.  If it fails for any unhandled reason then the program should crash.
			odThread.GroupName=FormODThreadNames.Shutdown.GetDescription();
			odThread.Name=FormODThreadNames.Shutdown.GetDescription();
			odThread.Start();
		}

		#endregion
		#region TasksThread

		///<summary>Begins a thread that handles all tasks from signals. Only call within signal processing.</summary>
		private void BeginTasksThread(List<Signalod> listSignalTasks,List<long> listEditedTaskNums) {
			//Do not call IsThreadAlreadyRunning(FormODThreadNames.Tasks) here.
			//Allow this thread to re-enter in the rare case that a subsequent run is required while the first run is still busy.
			//We don't want to miss the specific input that belongs to the second run.
			//SamO and Luke made this decision as it retains previous behavior.			
			ODThread threadTasks=new ODThread(new ODThread.WorkerDelegate((o) => {
				List<TaskNote> listRefreshedTaskNotes=null;
				List<UserOdPref> listBlockedTaskLists=null;
				//JM: Bug fix, but we do not know what would cause Security.CurUser to be null. Worst case task wont show till next signal tick.
				long userNumCur=Security.CurUser?.UserNum??0;
				List<OpenDentBusiness.Task> listRefreshedTasks=Tasks.GetNewTasksThisUser(userNumCur,Clinics.ClinicNum,listEditedTaskNums);
				if(listRefreshedTasks.Count > 0) {
					listRefreshedTaskNotes=TaskNotes.GetForTasks(listRefreshedTasks.Select(x => x.TaskNum).ToList());
					listBlockedTaskLists=UserOdPrefs.GetByUserAndFkeyType(userNumCur,UserOdFkeyType.TaskListBlock);
				}
				this.Invoke((() => HandleRefreshedTasks(listSignalTasks,listEditedTaskNums,listRefreshedTasks,listRefreshedTaskNotes,
					listBlockedTaskLists)));
			}));
			threadTasks.AddExceptionHandler((e) => e.DoNothing());
			threadTasks.GroupName=FormODThreadNames.Tasks.GetDescription();
			threadTasks.Name=FormODThreadNames.Tasks.GetDescription();
			threadTasks.Start();
		}

		#endregion
		#region TimeSyncThread

		///<summary>This begins the time sync thread. If OpenDental is running on the same machine as the mysql server, then a thread is runs in the 
		///background to update the local machine's time using NTPv4 from the NIST time server set in the NistTimeServerUrl pref.</summary>
		private void BeginTimeSyncThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.TimeSync)) {
				return;
			}
			if(!(ODEnvironment.IsRunningOnDbServer(MiscData.GetODServer()) && PrefC.GetBool(PrefName.ShowFeatureEhr))) {
				return;
			}
			//OpenDental has EHR enabled and is running on the same machine as the mysql server it is connected to.
			ODThread odThread=new ODThread((int)TimeSpan.FromHours(4).TotalMilliseconds,TimeSyncWorker);
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.GroupName=FormODThreadNames.TimeSync.GetDescription();
			odThread.Name=FormODThreadNames.TimeSync.GetDescription();
			odThread.Start();
		}

		///<summary>Worker thread for the time sync thread. Every 6 hours gets the time from an NTPv4 server and sets the local time to that.</summary>
		private void TimeSyncWorker(ODThread o) {
			NTPv4 ntp=new NTPv4();
			double nistOffset=double.MaxValue;
			ODException.SwallowAnyException(() => {//Invalid NIST Server URL if fails
				nistOffset=ntp.getTime(PrefC.GetString(PrefName.NistTimeServerUrl));
			});
			if(nistOffset!=double.MaxValue) {
				//Did not timeout, or have invalid NIST server URL
				//Sets local machine time. May error if unable to set machine.
				ODException.SwallowAnyException(() => {
					WindowsTime.SetTime(DateTime.Now.AddMilliseconds(nistOffset)); 
				});
			}
		}

		#endregion
		#region UpdateFormTextThread

		///<summary>Begins a thread that updates the title text if necessary in the case of an update. Always rounds down so as not to give users 
		///the impression that there is more time than there really is until the update.</summary>
		private void BeginUpdateFormTextThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.UpdateFormText)) {
				return;
			}
			ODThread odThreadUpdateFormText=new ODThread((int)TimeSpan.FromSeconds(1).TotalMilliseconds,(o) => {
				string mainTitleText=PatientL.GetMainTitleSamePat();
				//The Form.Text property is thead safe. Will invoke as a safety precaution still as the assignment will be nearly instant.
				this.Invoke(() => {
					this.Text=mainTitleText;
				});
			});
			odThreadUpdateFormText.AddExceptionHandler((e) => e.DoNothing());
			odThreadUpdateFormText.GroupName=FormODThreadNames.UpdateFormText.GetDescription();
			odThreadUpdateFormText.Name=FormODThreadNames.UpdateFormText.GetDescription();
			odThreadUpdateFormText.Start();
		}

		#endregion
		#region VoicemailThread

		private void BeginVoicemailThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.VoicemailHQ)) {
				return;
			}
			if(!PrefC.IsODHQ) {
				return;
			}
			ODThread odThread=new ODThread((int)TimeSpan.FromSeconds(3).TotalMilliseconds,VoicemailWorker);
			odThread.AddExceptionHandler(ex => ex.DoNothing());
			odThread.GroupName=FormODThreadNames.VoicemailHQ.GetDescription();
			odThread.Name=FormODThreadNames.VoicemailHQ.GetDescription();
			odThread.Start(true);
		}

		///<summary>Called on the voicemail thread for HQ only.</summary>
		private void VoicemailWorker(ODThread odThread) {
			try {
				List<VoiceMail> listVoiceMails=VoiceMails.GetAll(false,false).FindAll(x => x.UserNum==0);//Only include unclaimed VMs in the count and timer
				DateTime oldestVoicemail=DateTime.MaxValue;
				foreach(VoiceMail voiceMail in listVoiceMails) {
					if(voiceMail.DateCreated.AddSeconds(voiceMail.Duration)<oldestVoicemail) {
						oldestVoicemail=voiceMail.DateCreated.AddSeconds(voiceMail.Duration);//Adding Duration so the timer starts at the end of the VM
					}
				}
				TimeSpan ageOfOldestVoicemail=new TimeSpan(0);
				if(oldestVoicemail!=DateTime.MaxValue) {
					ageOfOldestVoicemail=DateTime.Now-oldestVoicemail;
				}
				this.Invoke(() => { SetVoicemailMetrics(false,listVoiceMails.Count,ageOfOldestVoicemail); });
			}
			catch(Exception ex) {
				//Something went wrong with determining how many voicemails there are.  Sleep for 4 minutes then try again.
				ODException.SwallowAnyException(() => this.Invoke(() => { SetVoicemailMetrics(true,0,new TimeSpan(0)); }));
				odThread.Wait((int)TimeSpan.FromMinutes(4).TotalMilliseconds);
				ex.DoNothing();
			}
		}

		///<summary>Called from worker thread for HQ voicemails. Sets all UI changes for the voicemail counter.</summary>
		private void SetVoicemailMetrics(bool hasError,int voiceMailCount,TimeSpan ageOfOldestVoicemail) {
			if(hasError) {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,8.25f,FontStyle.Bold);
				labelMsg.Text="error";
				labelMsg.ForeColor=Color.Firebrick;
				return;
			}
			labelMsg.Text=voiceMailCount.ToString();
			if(voiceMailCount==0) {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,7.75f,FontStyle.Regular);
				labelMsg.ForeColor=Color.Black;
			}
			else {
				labelMsg.Font=new Font(FontFamily.GenericSansSerif,7.75f,FontStyle.Bold);
				labelMsg.ForeColor=Color.Firebrick;
			}
			foreach(FormMapHQ formMapHQ in _listFormMapHQs) {
				formMapHQ.SetVoicemailRed(voiceMailCount,ageOfOldestVoicemail);
			}
			if(_formPhoneTiles!=null && !_formPhoneTiles.IsDisposed) {
				_formPhoneTiles.SetVoicemailCount(voiceMailCount);
			}
		}

		#endregion
		#region WebSyncThread

		///<summary>Begins the thread that checks for mobile sync. This will sync parts of a users database to HQ if certain preferences are set.</summary>
		private void BeginWebSyncThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.WebSync)) {
				return;
			}
			string interval=PrefC.GetStringSilent(PrefName.MobileSyncIntervalMinutes);
			if(interval=="" || interval=="0") {//not a paid customer or chooses not to synch
				return;
			}
			if(System.Environment.MachineName.ToUpper()!=PrefC.GetStringSilent(PrefName.MobileSyncWorkstationName).ToUpper()) {
				//Since GetStringSilent returns "" before OD is connected to db, this gracefully loops out
				return;
			}
			if(PrefC.GetDate(PrefName.MobileExcludeApptsBeforeDate).Year<1880) {
				//full synch never run
				return;
			}
			ODThread odThread=new ODThread((int)TimeSpan.FromSeconds(30).TotalMilliseconds,(o) => {
				ODException.SwallowAnyException(() => {
					FormEServicesMobileSynch.SynchFromMain(false);
				});
			});
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.GroupName=FormODThreadNames.WebSync.GetDescription();
			odThread.Name=FormODThreadNames.WebSync.GetDescription();
			odThread.Start();
		}

		#endregion
		#region RegistrationKeyIsDisabledThread

		///<summary>Begins a thread that monitor's the RegistrationKeyIsDisabled pref. If pref exists and value is true then shows an annoying popup.</summary>
		private void BeginRegistrationKeyIsDisabledThread() {
			if(IsThreadAlreadyRunning(FormODThreadNames.RegistrationKeyIsDisabled)) {
				return;
			}
			ODThread threadRegistrationKeyIsDisabled=new ODThread(
				(int)TimeSpan.FromMinutes(10).TotalMilliseconds,
				(o) => {
					if(PrefC.GetBoolSilent(PrefName.RegistrationKeyIsDisabled,false)) {
						this.Invoke(() => {
							MessageBox.Show(
								"Registration key has been disabled.  You are using an unauthorized version of this program.",
								"Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning);
						});						
					}
				});
			threadRegistrationKeyIsDisabled.AddExceptionHandler(ex => ex.DoNothing());
			threadRegistrationKeyIsDisabled.GroupName=FormODThreadNames.RegistrationKeyIsDisabled.GetDescription();
			threadRegistrationKeyIsDisabled.Name=FormODThreadNames.RegistrationKeyIsDisabled.GetDescription();
			threadRegistrationKeyIsDisabled.Start();
		}

		#endregion

		///<summary>A list of the names and group names for all threads spawned from FormOpenDental.</summary>
		private enum FormODThreadNames {
			[Obsolete]
			CacheFillForFees,
			CanadianItransCarrier,
			CheckAlerts,
			ClaimReport,
			ComputerHeartbeat,
			CrashedTableMonitor,
			DataConnectionLost,
			EhrCodeList,
			///<summary>DEPRECATED. This thread now runs from Open Dental Service solution</summary>
			EnableAdditionalFeatures,
			EServiceMonitoring,
			HqMetrics,
			LogOff,
			MiddleTierConnectionLost,
			ODServiceMonitor,
			ODServiceStarter,
			PhoneConference,
			PlaySounds,
			Podium,
			RegKeyIsForTesting,
			ReplicationMonitor,
			Shutdown,
			Tasks,
			TimeSync,
			UpdateFormText,
			VoicemailHQ,
			WebSync,
			Dashboard,
			RegistrationKeyIsDisabled,
			DataReaderNullMonitor,
		}
	}
}
