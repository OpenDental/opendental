using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using OpenDentBusiness;

namespace OpenDentBusiness.WebTypes {

	///<summary>Keeps track of when different InvalidType signals are processed.  Allows eConnector to determine which caches are stale based on a
	///given timestamp.</summary>
	public class InvalidTypeHistory {
		///<summary>Dictionary of InvalidTypes and DateTime they were each last refreshed.</summary>
		private static Dictionary<InvalidType,DateTime> _dictInvalidTypes=new Dictionary<InvalidType,DateTime>();
		///<summary>A list of appointment and schedule signals used by GetAppointmentsSince() and populated by UpdateStatus() when called from Signalods.RefreshForWeb(). 
		///Otherwise null. Any signalods older than DateTime.Now-PrefName.ODMobileCacheDurationHours will be removed.
		///Public getter is for unit test.</summary>
		public static List<Signalod> ListApptSinceSignalCache { get; private set; } = new List<Signalod>();
		///<summary>Locks the dictionary of InvalidTypes for reading/writing.</summary>
		private static object _lockInvalidTypes=new object();
		private static bool _isInitialized=false;
		
		///<summary>Initializes local caches as having a start time of DateTime.Now. This is safe to call repeatedly. Blocks re-entrance after first time.</summary>
		public static void InitIfNecessary() {
			if(!_isInitialized) {
				UpdateStatus(DateTime.Now,Enum.GetValues(typeof(InvalidType)).Cast<InvalidType>().ToArray());
				_isInitialized=true;
			}
		}

		public static void ClearSignalCache() {
			lock(_lockInvalidTypes) {
				ListApptSinceSignalCache.Clear();
			}
		}

		///<summary>Overload for UpdateStatus that does not specify a list of signals.</summary>
		public static void UpdateStatus(DateTime timeRefreshed,params InvalidType[] arrInvalidTypes) {
			UpdateStatus(timeRefreshed,null,arrInvalidTypes);
		}
		
		///<summary>Updates the given InvalidTypes with the given DateTime.</summary>
		///<param name="timeRefreshed">The DateTime arrInvalidTypes was last refreshed.</param>
		///<param name="arrInvalidTypes">The InvalidTypes that were refreshed.</param>
		public static void UpdateStatus(DateTime timeRefreshed,List<Signalod> listSignals,params InvalidType[] arrInvalidTypes) {
			CleanApptSinceCache();
			if(arrInvalidTypes.IsNullOrEmpty()) {
				return;
			}
			lock(_lockInvalidTypes) {
				foreach(InvalidType invalidType in arrInvalidTypes) {
					//Truncate to seconds. This is to avoid race conditions which would report signals as overly stale.
					_dictInvalidTypes[invalidType]=DateTools.ToBeginningOfSecond(timeRefreshed);
				}
				if(listSignals!=null) {
					foreach(Signalod signal in listSignals) {
						//Any of these signal types can inidicate stale appointments for a given operatory in one case or another so save them all.
						if(ListTools.In(signal.IType,InvalidType.Appointment,InvalidType.Schedules,InvalidType.Operatories)) {
							ListApptSinceSignalCache.Add(signal);
						}
					}
				}
			}
		}

		///<summary>A small helper class that removes old signals from _listApptSinceSignalCache. Signals are flagged as old if
		///they have a SigDateTime older than DateTime.Now-PrefName.ODMobileCacheDurationHours. In separate method for unit testing.</summary>
		private static void CleanApptSinceCache() {
			DateTime cacheDuration=DateTime.Now.AddHours(-PrefC.GetLong(PrefName.ODMobileCacheDurationHours));//safe to call often, preference is cached.
			lock(_lockInvalidTypes) {
				ListApptSinceSignalCache.RemoveAll(x => x.SigDateTime<cacheDuration);
			}
		}

		///<summary>Uses ListApptSinceSignalCache to determine what Appointments and Schedules have changed for the given ClinicNum's ops. 
		///Only goes to the database if an appointment, schedule, or operatory signal is linked to the op's passed in and is in our daterange.
		///If an entry for a given operatory is included in the output, then all appointments and blockouts for that operatory will be included for all dates specified in the range.
		///timeRefreshed is the last time signals were processed. A value of DateTime.MinVal will return an empty list of appointments and schedules.
		///If includeApptItemsInOutput==true then lists of Appointments and Schedules will be full and valid.
		///When includeApptItemsInOutput==false then lists will be empty and caller is only interesting in whether or not the list of tuples itself would be > 0. This will be used to decide if signal processing should be performed.</summary>
		public static List<Tuple<long,DateTime,List<Appointment>,List<Schedule>>> GetApptsAndSchedsSince(DateTime timeRefreshed,DateTime dateViewingStart,DateTime dateViewingEnd,List<long> listClinicNums,bool includeApptItemsInOutput) {
			//Xam will pass DateTime.MinVal if first this is the first attempt for the session. 
			//Pass back empty list here so we can establish TimeRefreshed for next time.
			if(timeRefreshed==DateTime.MinValue){
				return new List<Tuple<long,DateTime,List<Appointment>,List<Schedule>>>();
			}
			List<Signalod> listSignalsFiltered=null;
			lock(_lockInvalidTypes) {
				listSignalsFiltered=ListApptSinceSignalCache.FindAll(x => 
					//Only care about signal since the last time we asked.
					x.SigDateTime>=timeRefreshed &&
					//Always include signals that have not specified DateViewing of MinVal. This is a special case means a full appt refresh is needed.
					//Otherwise only include signals which match that date range that we are actually viewing right now.
					(x.DateViewing==DateTime.MinValue.Date || x.DateViewing.Between(dateViewingStart,dateViewingEnd,true,true)));
			}
			List<Signalod> listApptSignals=listSignalsFiltered.FindAll(x => x.IType==InvalidType.Appointment);
			List<Signalod> listSchedSignals=listSignalsFiltered.FindAll(x => x.IType==InvalidType.Schedules);
			//Operatories signal insert is not accompanied by an Appointment signal in OD proper. 
			//So if we get any Op signal, assume that all appts are dirty for all clinics. Dirty Ops is rare so it's fine to be overly cautious.
			bool isOpsChanged=listSignalsFiltered.Any(x => x.IType==InvalidType.Operatories);
			//Get all operatories for each clinic in listClinics, generally we will only be passed one clinic.
			List<long> listOpNumsVisible=new List<long>();
			foreach(long clinicNum in listClinicNums) {
				//For now, all ops for this clinic. If we ever implement appt view for OD Mobile, we will need a filter here. 
				
				//todo: test with clinicNum 0 and clinics off. Should get all clinics but probably only gets clinicNum 0.

				listOpNumsVisible.AddRange(Operatories.GetOpsForClinic(clinicNum).Select(x => x.OperatoryNum).ToList());
			}
			//Remove duplicates.
			listOpNumsVisible=listOpNumsVisible.Distinct().ToList();
			//Init these to empty list, not null. This is important down below.
			List<Appointment> listAppts=new List<Appointment>();
			List<Schedule> listScheds=new List<Schedule>();
			List<Tuple<long,DateTime,List<Appointment>,List<Schedule>>> ret=new List<Tuple<long, DateTime, List<Appointment>, List<Schedule>>>();
			if(
				//An Operatory signal means we should refresh all operatories for all dates.
				isOpsChanged
				//An appt signal where we don't know exactly which appts have changed so refresh all operatories.
				|| listApptSignals.Exists(x => x.DateViewing==DateTime.MinValue.Date)
				//A schedule signal where we don't know exactly which schedules have changed so refresh all blockouts.
				|| listSchedSignals.Exists(x => x.DateViewing==DateTime.MinValue.Date))
			{
				//Only query the db when we absolutely have to.
				if(includeApptItemsInOutput && listOpNumsVisible.Count>0) {
					//Get all appts and schedules for the ops which have changes. 
					listAppts=Appointments.GetAppointmentsForOpsByPeriod(listOpNumsVisible,dateViewingStart,dateViewingEnd)
						.FindAll(x => ListTools.In(x.AptStatus,ApptStatus.Complete,
						ApptStatus.Scheduled,
						ApptStatus.Broken));
					listScheds=Schedules.GetAllForDateRangeAndType(dateViewingStart,dateViewingEnd,ScheduleType.Blockout,false,listOpNumsVisible);
				}
				//One entry in the list per every operatory and date in our range. 
				foreach(long opNum in listOpNumsVisible) {
					int days=dateViewingEnd.Subtract(dateViewingStart).Days;
					for(int i=0; i<=days; i++) {
						//The entry may be empty for a given operatory if there are currently no items for the given date range.
						DateTime date=dateViewingStart.AddDays(i).Date;
						ret.Add(new Tuple<long,DateTime,List<Appointment>,List<Schedule>>(
							opNum,
							date,
							//Will be empty in !includeApptItemsInOutput.
							listAppts.FindAll(y => y.Op==opNum && y.AptDateTime.Date==date),
							//Will be empty in !includeApptItemsInOutput.
							listScheds.FindAll(y => y.Ops.Contains(opNum) && y.SchedDate.Date==date)
						));
					}
				}
			}
			else {
				//Only add entries for operatory/date pairs which had a signal specificially added.
				var dateChanges=listApptSignals
					//We are not considering KeyType.Provider for now. 
					//This leaves a tiny edge case where a KeyType.Provider signal was made by SignalOds.SetInvalidAppt() but not a KeyType.Operatory signal.
					//Sam and Luke were not even able to produce this edge case and implementing it would cause us to have to redesign this linq statement to not be Ops-based.
					//If a bug is reported in the future regarding changing something about provider not updating the appt view in ODMobile then this is likely the culprit.
					.FindAll(x => x.FKeyType==KeyType.Operatory && ListTools.In(x.FKey,listOpNumsVisible))
					.GroupBy(x => new { x.FKey,x.DateViewing.Date })
					.Select(x => new { Op=x.Key.FKey,x.Key.Date})
					.Union(listSchedSignals
						.FindAll(x => x.FKeyType==KeyType.Operatory && ListTools.In(x.FKey,listOpNumsVisible))
						.GroupBy(x => new { x.FKey,x.DateViewing.Date })
						.Select(x => new { Op=x.Key.FKey,x.Key.Date })
					)
					.GroupBy(x => new { x.Op, x.Date })
					.Select(x => new { x.Key.Op,x.Key.Date })
					.GroupBy(x => x.Date)
					.Select(x => new {Date=x.Key,Ops=x.Select(y => y.Op).ToList()});
				foreach(var dateChange in dateChanges){
					//Only query the db when we absolutely have to.
					if(includeApptItemsInOutput) {
						//Get all appts and schedules for each unique op/date. 
						//Some dates in our range may have changes and some may not so we will need to query the db once for each date which has changes.
						//Appointments.GetAppointmentsForOpsByPeriod DOES add a day to DateEnd.
						listAppts=Appointments.GetAppointmentsForOpsByPeriod(dateChange.Ops,dateChange.Date,dateChange.Date)
							.FindAll(x => ListTools.In(x.AptStatus,ApptStatus.Complete,
							ApptStatus.Scheduled,
							ApptStatus.Broken));
						//Schedules.GetAllForDateRangeAndType does NOT add a day to DateEnd.
						listScheds=Schedules.GetAllForDateRangeAndType(dateChange.Date,dateChange.Date.AddDays(1),ScheduleType.Blockout,false,dateChange.Ops);
					}
					//One entry in the list per changed operatory/date. The entry may be empty for a given operatory if there are currently no items for the given date range.
					ret.AddRange(dateChange.Ops.Select(x => new Tuple<long,DateTime,List<Appointment>,List<Schedule>>(
						x,
						dateChange.Date,
						//Will be empty in !includeApptItemsInOutput.
						listAppts.FindAll(y => y.Op==x && y.AptDateTime.Date==dateChange.Date),
						//Will be empty in !includeApptItemsInOutput.
						listScheds.FindAll(y => y.Ops.Contains(x) && y.SchedDate.Date==dateChange.Date)
					)));
				}
			}
			return ret;
		}

		///<summary>Determines the DateTime the given signal was refreshed since the given DateTime.</summary>
		public static DateTime GetDateTimeRefreshed(InvalidType iType) {
			InitIfNecessary(); 
			lock(_lockInvalidTypes) {
				if(iType==InvalidType.Appointment){
					//Appointments signals are being stored locally so they can be played back for a given Xam session.
					//This will allow a determination to be made if there are any stale op views for the session within the given timeframe.
					//If there are no signals currently cached then assume there is nothing stale (DateTime.MinValue).
					//If there are signals then assume that the of the most recent signal is the latest stale time.
					//Any sessions that have not updated since the latest signal time will be assumed to be stale and should ask for appointment changes.
					return ListApptSinceSignalCache.Count==0 ? DateTime.MinValue : ListApptSinceSignalCache.Max(x => x.SigDateTime);
				}
				if(_dictInvalidTypes.TryGetValue(iType,out DateTime date)) {
					return date;
				}
			}
			return DateTime.MinValue;
		}
	}

}
