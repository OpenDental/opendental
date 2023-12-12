using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class ApptSearch {
		/// <summary>Gets a list of openings for the selected providers, then gets a list of openings for the selected blockouts. Will 
		/// then compare the lists and return results where the DateTime exists is both the list for the providers and the blockouts.</summary>
		public static List<ScheduleOpening> GetSearchResultsForBlockoutAndProvider(List<long> listProvNums,long apptNum,DateTime startDate,DateTime endDate,
			List<long> listOpNums,List<long> listClinicNums,TimeSpan timeBefore,TimeSpan timeAfter,List<long> listBlockoutTypes,int resultCount=10) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ScheduleOpening> listOpenings=new List<ScheduleOpening>();
			//searching for intersection of provider and blockout type. Search needs to be handled differently. 
			List<long> listProvs=listProvNums.FindAll(x => x!=0);//list of providers (excluding the blockout provNum 0)
			List<ScheduleOpening> listOpeningsForProvs=new List<ScheduleOpening>();
			List<ScheduleOpening> listOpeningForBlockout=new List<ScheduleOpening>();
			//list of all openings for the given providers. 
			listOpeningsForProvs=GetSearchResults(apptNum,startDate,endDate,listProvs,listOpNums,listClinicNums,timeBefore,timeAfter,
				hasProvAndBlockout:true).OrderBy(x => x.DateTimeAvail).ToList();
			listOpeningsForProvs.RemoveAll(x => x.ProvNum==0);//blockouts should not be in the list when comparing. 
			//list of all openings for the given blockouts - list of providers is just provNum 0 for blockout purposes.
			listOpeningForBlockout=GetSearchResults(apptNum,startDate,endDate,new List<long>() {0},listOpNums,listClinicNums,timeBefore,
				timeAfter,listBlockoutTypes,true).OrderBy(x => x.DateTimeAvail).ToList();
			//Get the first DateTime,OpNum,Clinic combo that are present in both the provider and blockout openings.
			Dictionary<DateTime,List<ScheduleOpening>> dictOpeningsByDate=listOpeningsForProvs
				.Where(x => listOpeningForBlockout.Any(y => y.DateTimeAvail==x.DateTimeAvail && y.OpNum==x.OpNum && y.ClinicNum==x.ClinicNum))
				.GroupBy(x => x.DateTimeAvail.Date)
				.ToDictionary(x => x.Key,x => x.ToList());
			//Only return one opening per day and limit the results by resultCount that was passed in.
			foreach(DateTime dateKey in dictOpeningsByDate.Keys) {
				listOpenings.Add(dictOpeningsByDate[dateKey].First());
				if(listOpenings.Count>=resultCount) {
					break;
				}
			}
			return listOpenings;
		}

		///<summary>Gets time availability for appointment searching. Moved to business layer for unit tests.
		///Returns the first available time slots for each of the next 10 available days.  This means there will only be one time slot per day and
		///that time slot will always be the first available time slot within that day regardless of additional time slots being available.
		///resultCount defaults to preserve old functionalityThe UI only holds 10 but this could be changed.</summary>
		public static List<ScheduleOpening> GetSearchResults(long aptNum,DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listOpNums
			,List<long> listClinicNums,TimeSpan beforeTime,TimeSpan afterTime,List<long> listBlockoutTypes=null,bool hasProvAndBlockout=false,int resultCount=10,bool isForMakeRecall=false) 
		{ 
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {//No call to db but designed to save middle tier trips.
				return Meth.GetObject<List<ScheduleOpening>>(MethodBase.GetCurrentMethod(),aptNum,dateStart,dateEnd,listProvNums,listOpNums,listClinicNums
					,beforeTime,afterTime,listBlockoutTypes,hasProvAndBlockout,resultCount,isForMakeRecall);
			}
			if(listBlockoutTypes is null) {
				listBlockoutTypes=new List<long>(){0};
			}
			//if they didn't set a before time, set it to a large timespan so that we can use the same logic for checking appointment times.
			if(beforeTime==TimeSpan.FromSeconds(0)) {
				beforeTime=TimeSpan.FromHours(25);//bigger than any time of day.
			}
			ApptSearchData data=GetDataForSearch(aptNum,dateStart,dateEnd,listProvNums,listOpNums,listClinicNums,listBlockoutTypes,isForMakeRecall:isForMakeRecall);
			List<ScheduleOpening> retVal=new List<ScheduleOpening>();
			if(data.AppointmentToAdd==null) {//appointment was deleted after clicking search. 
				return retVal;
			}
			DateTime dateEvaluating=data.DateEvaluating;
			SearchBehaviorCriteria searchType=(SearchBehaviorCriteria)PrefC.GetInt(PrefName.AppointmentSearchBehavior);
			if(hasProvAndBlockout) {//searching for intersection of providers and blockouts get as many results as possible.
				while(dateEvaluating < dateEnd) {
					List<ScheduleOpening> listPotentialTimeAvailable=GetProvAndOpAvailabilityHelper(listProvNums,dateEvaluating,data,searchType,listOpNums
						,listBlockoutTypes);
					//At this point listPotentialTimeAvailable is already filtered and only contains appt times that match both provider time and operatory time. 
					List<ScheduleOpening> listOpeningsForEntireDay=AddTimesToSearchResultsHelper(listPotentialTimeAvailable,beforeTime,afterTime);
					retVal.AddRange(listOpeningsForEntireDay);
					dateEvaluating=dateEvaluating.AddDays(1);
				}
			}
			else {
				while(retVal.Count < resultCount && dateEvaluating < dateEnd) {
					List<ScheduleOpening> listPotentialTimeAvailable=GetProvAndOpAvailabilityHelper(listProvNums,dateEvaluating,data,searchType,listOpNums
						,listBlockoutTypes);
					//At this point listPotentialTimeAvailable is already filtered and only contains appt times that match both provider time and operatory time. 
					ScheduleOpening firstOpeningForDay=AddTimeToSearchResultsHelper(listPotentialTimeAvailable,beforeTime,afterTime);
					if(firstOpeningForDay!=null) {
						retVal.Add(firstOpeningForDay);
					}
					dateEvaluating=dateEvaluating.AddDays(1);
				}
			}
			return retVal;
		}

		private static List<ScheduleOpening> GetProvAndOpAvailabilityHelper(List<long> listProvNums,DateTime dateEvaluating,ApptSearchData data,
			SearchBehaviorCriteria searchType,List<long> listOpNums,List<long> listBlockoutTypes) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ScheduleOpening> listPotentialTimeAvailable=new List<ScheduleOpening>();//create or clear
			//Providers---------------------------------------------------------------------------------------------------------------
			List<ApptSearchProviderSchedule> listProvScheds=new List<ApptSearchProviderSchedule>();//Provider Bar, ProviderSched Bar, Date and Provider
			listProvScheds=Appointments.GetProviderScheduleForProvidersAndDate(listProvNums,dateEvaluating,data.ListSchedules,data.ListAppointments);
			if(searchType==SearchBehaviorCriteria.ProviderTime) {//Fill the time the provider is available
				listPotentialTimeAvailable=FillProviderTimeHelper(listProvScheds,data.AppointmentToAdd,dateEvaluating,listBlockoutTypes);
			}
			//Handle Operatories -----------------------------------------------------------------------------------------------------
			if(searchType==SearchBehaviorCriteria.ProviderTimeOperatory) {//Fill the time the prov and op are available
				List<ApptSearchOperatorySchedule> listOpScheds=new List<ApptSearchOperatorySchedule>();//filtered based on SearchType
				listOpScheds=GetAllForDate(dateEvaluating,data.ListSchedules,data.ListAppointments,data.ListSchedOps,listOpNums,listProvNums,listBlockoutTypes);
				listPotentialTimeAvailable=FillOperatoryTime(listOpScheds,listProvScheds,data.AppointmentToAdd,dateEvaluating,listProvNums,listBlockoutTypes
					,data.ListSchedOps,data.ListSchedules);
			}
			return listPotentialTimeAvailable;
		}

		///<summary>Returns the first ScheduleOpening that falls before or after the times passed in.  Returns null if none found.</summary>
		private static ScheduleOpening AddTimeToSearchResultsHelper(List<ScheduleOpening> listApptTimeForBehavior,TimeSpan beforeTime,TimeSpan afterTime)	{
			//No need to check MiddleTierRole; no call to db.
			ScheduleOpening firstAvailability=null;
			listApptTimeForBehavior=listApptTimeForBehavior.OrderBy(x => x.DateTimeAvail).ToList();
			for(int i=0;i<listApptTimeForBehavior.Count;i++) {
				if(listApptTimeForBehavior[i].DateTimeAvail.TimeOfDay>beforeTime || listApptTimeForBehavior[i].DateTimeAvail.TimeOfDay<afterTime) {
					continue;
				}
				firstAvailability=listApptTimeForBehavior[i];//add one for this day (only want one time per day).
				break;
			}
			return firstAvailability;
		}

		/// <summary>Used specifically when searching for provider schedules with blockouts. We need to get a list so we can check if the 
		/// provider shedule and blockout schedule have any matching times during the entire day since their availabilities may not line up as the first
		/// possible for the day.</summary>
		private static List<ScheduleOpening> AddTimesToSearchResultsHelper(List<ScheduleOpening> listApptTimeForBehavior,TimeSpan beforeTime
			,TimeSpan afterTime) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ScheduleOpening> listAvailability=new List<ScheduleOpening>();
			for(int i=0;i<listApptTimeForBehavior.Count;i++) {
				if(listApptTimeForBehavior[i].DateTimeAvail.TimeOfDay>beforeTime || listApptTimeForBehavior[i].DateTimeAvail.TimeOfDay<afterTime) {
					continue;
				}
				listAvailability.Add(listApptTimeForBehavior[i]);
			}
			return listAvailability;
		}

		///<summary>Gets the data necessary from the database to run the appointment search logic.</summary>
		public static ApptSearchData GetDataForSearch(long aptNum,DateTime dateAfter,DateTime dateBefore,List<long> listProvNums,List<long> listOpNums
			,List<long> listClinicNums,List<long> listBlockoutTypes, bool isForMakeRecall=false) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ApptSearchData>(MethodBase.GetCurrentMethod(),aptNum,dateAfter,dateBefore,listProvNums,listOpNums,listClinicNums
					,listBlockoutTypes,isForMakeRecall);
			}
			ApptSearchData data=new ApptSearchData();
			data.DateEvaluating=dateAfter.AddDays(1);
			data.AppointmentToAdd=Appointments.GetOneApt(aptNum);
			data.ListSchedules=Schedules.GetSchedulesForAppointmentSearch(data.DateEvaluating,dateBefore,listClinicNums,listOpNums
				,listProvNums,listBlockoutTypes,isForMakeRecall: isForMakeRecall);
			//Get all appointments that exist in the operaotries we will be searching to find an opening, not just for provider we're looking for
			//so we can get conflicts when multiple provs work in a single operaotry.  
			data.ListAppointments=Appointments.GetForPeriodList(data.DateEvaluating,dateBefore,listOpNums,listClinicNums);
			data.ListSchedOps=ScheduleOps.GetForSchedList(data.ListSchedules,listOpNums);//ops filter for case when a prov is scheduled in multiple ops
			return data;
		}

		///<summary>Searches through provider time relative to the appointment time to mark if provider is available.
		///Helper method for GetSearchResults.Fills prov availability for the given day.</summary>
		private static List<ScheduleOpening> FillProviderTimeHelper(List<ApptSearchProviderSchedule> listProvScheds,Appointment appointmentToAdd
			,DateTime dayEvaluating,List<long> listBlockoutTypes) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ScheduleOpening> listPotentialProvApptTime=new List<ScheduleOpening>();//clear or create
			if(listProvScheds==null || appointmentToAdd==null) {
				return listPotentialProvApptTime;
			}
			foreach(ApptSearchProviderSchedule providerSchedule in listProvScheds) {
				for(int j=0;j<288;j++) {//search every 5 minute increment per day
					//listBlockoutTypes should always have at least one value. 0 as the sole listBlockoutTypes value means we are not searching for blockouts, so we only want provider schedules
					bool isProviderSchedulesOnly=(providerSchedule.ProviderNum==0 && listBlockoutTypes[0]==0);
					if(j+appointmentToAdd.Pattern.Length>288) {//skip if appointment length spans over a 24 hour period.
						break;
					}
					if(listPotentialProvApptTime.Select(x => x.DateTimeAvail).Contains(dayEvaluating.AddMinutes(j*5))) {
						continue;//skip if provider isn't available in this 5 min increment
					}
					bool addDateTime=true;
					for(int k=0;k<appointmentToAdd.Pattern.Length;k++) {
						if((providerSchedule.IsProvAvailable[j+k]==false && appointmentToAdd.Pattern[k]=='X') || providerSchedule.IsProvScheduled[j+k]==false
							|| isProviderSchedulesOnly)
						{
							addDateTime=false;
							break;
						}
					}
					if(addDateTime) {
						listPotentialProvApptTime.Add(new ScheduleOpening 
						{
							DateTimeAvail=dayEvaluating.AddMinutes(j*5),
							ProvNum=providerSchedule.ProviderNum
						});
					}
				}
			}
			return listPotentialProvApptTime;
		}

		///<summary>Searches through operatory time relative to the appointment time to mark if operatory has availability. 
		///Helper method for GetSearchResults.</summary>
		private static List<ScheduleOpening> FillOperatoryTime(List<ApptSearchOperatorySchedule> listApptSearchOperatorySchedules,List<ApptSearchProviderSchedule> listApptSearchProviderSchedules
			,Appointment appointmentToAdd,DateTime dateEvaluating,List<long> listProvNums,List<long> listBlockoutTypes,List<ScheduleOp> listScheduleOps,List<Schedule> listSchedules) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ScheduleOpening> listScheduleOpenings=new List<ScheduleOpening>();//create or clear 
			for(int i=0;i<288;i++) {//search every 5 minute increment per day
				if(i+appointmentToAdd.Pattern.Length>288) {//skip if appointment would span across midnight
					break;
				}
				foreach(ApptSearchOperatorySchedule apptSearchOperatorySchedule in listApptSearchOperatorySchedules) {
					bool doAddDateTime=true;
					for(int k=0;k<appointmentToAdd.Pattern.Length;k++) {//check appointment against operatories
						if(apptSearchOperatorySchedule.IsOpAvailable[i+k]==false) {
							doAddDateTime=false;
							break;
						}
					}
					if(!doAddDateTime){
						continue;
					}
					//check appointment against providers available for the given operatory
					bool isProvAvailable=false;
					long provNumAvail=0;
					for(int k=0;k<listProvNums.Count;k++) {
						if(!apptSearchOperatorySchedule.ProviderNums.Contains(listProvNums[k])) {
							continue;
						}
						isProvAvailable=true;
						provNumAvail=listApptSearchProviderSchedules[k].ProviderNum;
						for(int m=0;m<appointmentToAdd.Pattern.Length;m++) {
							//If provider is not available, and the appointment has an "X" for provider time at this spot in the pattern
							//OR provider is not scheduled to work
							//OR current schedule is a blockout, while we're not considering blockouts
							if((listApptSearchProviderSchedules[k].IsProvAvailable[i+m]==false && appointmentToAdd.Pattern[m]=='X') 
								|| listApptSearchProviderSchedules[k].IsProvScheduled[i+m]==false
								|| listApptSearchProviderSchedules[k].ProviderNum==0 && listBlockoutTypes[0]==0)
							{
								isProvAvailable=false;
								break;
							}
							else if(provNumAvail==0) { //This is a blockout schedule, which we want to consider since we got to this point (provNumAvail is 0 and listBlockoutTypes contains non-zero elements). We only want to return times where blockout types are scheduled.
								//Get a list of any blockouts that are scheduled within this 5 minute timeframe. 
								//If none of those blockouts cover our current ApptSearchOperatorySchedule's operatory at this time, then we do not want to consider this time as available.
								List<Schedule> listSchedulesBlockouts=listSchedules.FindAll(x => x.ProvNum==0 
									&& x.SchedDate.Date==dateEvaluating.Date
									&& x.StartTime<=dateEvaluating.AddMinutes(i*5).TimeOfDay 
									&& x.StopTime>=dateEvaluating.AddMinutes(i*5).TimeOfDay).ToList();
								List<long> listOpNumsForSchedule=listScheduleOps.FindAll(x => listSchedulesBlockouts.Any(y => x.ScheduleNum==y.ScheduleNum))
									.Select(x => x.OperatoryNum).Distinct().ToList();
								if(!listOpNumsForSchedule.Contains(apptSearchOperatorySchedule.OperatoryNum)) {
									isProvAvailable=false;
								}
							}
							else { //This is an open provider schedule, but they may not be scheduled at this time for this operatory. Verify the provider is open at this time, and if applicable, for this operatory.
								List<Schedule> listSchedulesForProviderAndOp=listSchedules.FindAll(x => x.ProvNum==provNumAvail 
								&& (x.Ops.Count==0 || x.Ops.Contains(apptSearchOperatorySchedule.OperatoryNum))
								&& x.SchedDate.Date==dateEvaluating.Date
								&& x.StartTime<=dateEvaluating.AddMinutes(i*5).TimeOfDay 
								&& x.StopTime>=dateEvaluating.AddMinutes(i*5).TimeOfDay).ToList();
								if(listSchedulesForProviderAndOp.Count==0) {
									isProvAvailable=false;
								}
							}
						}
						if(isProvAvailable) {//found a provider with an available time and operatory
							break;
						}
					}
					if(isProvAvailable) {
						DateTime timeOpeningStart=dateEvaluating.AddMinutes(i*5);
						listScheduleOpenings.Add(new ScheduleOpening {DateTimeAvail=timeOpeningStart,ProvNum=provNumAvail
							,OpNum=apptSearchOperatorySchedule.OperatoryNum });
					}
				}
			}
			return listScheduleOpenings;
		}

		///<summary>Get all provider operatory availabilities for passed in data.</summary>
		private static List<ApptSearchOperatorySchedule> GetAllForDate(DateTime scheduleDate,List<Schedule> listSchedules
			,List<Appointment> listAppointments,List<ScheduleOp> listSchedOps,List<long> listOpNums,List<long> listProvNums,List<long> listBlockoutTypes) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<ApptSearchOperatorySchedule> listOpScheds=new List<ApptSearchOperatorySchedule>();
			List<Operatory> listOps=Operatories.GetWhere(x => listOpNums.Contains(x.OperatoryNum));
			//Remove any ScheduleOps that are not related to the operatories passed in.
			listSchedOps.RemoveAll(x => !listOpNums.Contains(x.OperatoryNum));
			//Create dictionaries that are comprised of every operatory in question and will keep track of all ProviderNums for specific scenarios.
			Dictionary<long,List<long>> dictProvNumsInOpsBySched=listOps.ToDictionary(x => x.OperatoryNum,x => new List<long>());
			Dictionary<long,List<long>> dictProvNumsInOpsByOp=listOps.ToDictionary(x => x.OperatoryNum,
				x => new List<long>() { x.ProvDentist,x.ProvHygienist });//Could be a list of two 0's if no providers are associated to this op.
			scheduleDate=scheduleDate.Date;//remove time component
			foreach(long opNum in listOpNums) {
				ApptSearchOperatorySchedule apptSearchOpSched=new ApptSearchOperatorySchedule();
				apptSearchOpSched.SchedDate=scheduleDate;
				apptSearchOpSched.ProviderNums=new List<long>();
				apptSearchOpSched.OperatoryNum=opNum;
				apptSearchOpSched.IsOpAvailable=new bool[288];
				for(int j=0;j<288;j++) {
					apptSearchOpSched.IsOpAvailable[j]=true;//Set entire operatory schedule to true. True=available.
				}
				listOpScheds.Add(apptSearchOpSched);
			}
			#region Fill OpScheds with Providers allowed to work in each operatory
			//Make explicit entries into dictProvNumsInOpsBySched if there are any SchedOps for each schedule OR add an entry to every operatory if none found.
			foreach(Schedule schedule in listSchedules.FindAll(x => x.SchedDate==scheduleDate)) {//use this loop to fill listProvsInOpBySched
				List<ScheduleOp> listSchedOpsForSchedule=listSchedOps.FindAll(x => x.ScheduleNum==schedule.ScheduleNum);
				if(listSchedOpsForSchedule.Count > 0) {
					AddProvNumToOps(dictProvNumsInOpsBySched,
							listSchedOpsForSchedule.Select(x => x.OperatoryNum).Distinct().ToList(),
							schedule.ProvNum);
				}
				else {//Provider scheduled to work, but not limited to specific operatory to add providerNum to all ops in opsProvPerSchedules
					AddProvNumToOps(dictProvNumsInOpsBySched,
						dictProvNumsInOpsBySched.Keys.ToList(),
						schedule.ProvNum);
				}
			}
			//Set each listOpScheds.ProviderNums to the corresponding providers via operatory OR schedules.
			SearchBehaviorCriteria searchBehaviorCriteria=(SearchBehaviorCriteria)PrefC.GetInt(PrefName.AppointmentSearchBehavior);
			foreach(Operatory op in listOps) {
				//If listBlockoutTypes has a non-zero entry, and 0 is only provNum in listProvNums, we are just looking for blockout schedules in ops.
				//Add zero to ProviderNums list for op if op has any blockout for the date we are searching. Unwanted blockouts are filtered out below.
				if(listBlockoutTypes.Exists(x => x>0) && listProvNums.Max()==0 && dictProvNumsInOpsBySched[op.OperatoryNum].Contains(0)) {
					listOpScheds.First(x => x.OperatoryNum==op.OperatoryNum).ProviderNums.Add(0);
				}
				//If the operatory does not have a primary and secondary provider, use all providers from the schedules.
				else if(dictProvNumsInOpsByOp[op.OperatoryNum][0]==0 && dictProvNumsInOpsByOp[op.OperatoryNum][1]==0) {
					if(searchBehaviorCriteria==SearchBehaviorCriteria.ProviderTimeOperatory && listSchedOps.Count==0) {
						//We are using ProviderTimeOp search logic and the providers are only assigned to their ops directly, NOT in their schedules.
						//In this case, an op with no primary or secondary provider means the op is unassigned, so we don't want to assign it.
						//This effectively excludes unassigned ops from ProviderTimeOp search logic for this specific case.
						continue;
					}
					listOpScheds.First(x => x.OperatoryNum==op.OperatoryNum).ProviderNums=dictProvNumsInOpsBySched[op.OperatoryNum];
				}
				else {//Otherwise; only add providers that intersect between schedules and being explicitly assigned to an operatory.
					List<long> listIntersectingProvNums=dictProvNumsInOpsBySched[op.OperatoryNum].Intersect(dictProvNumsInOpsByOp[op.OperatoryNum]).ToList();
					if(listIntersectingProvNums.Count() > 0) {
						listOpScheds.First(x => x.OperatoryNum==op.OperatoryNum).ProviderNums.AddRange(listIntersectingProvNums);
					}
				}
			}
			#endregion
			#region Remove provider availability for current appointments
			List<Appointment>listAppointmentsForDate=listAppointments.FindAll(x => x.Op!=0 && x.AptDateTime.Date==scheduleDate);
			foreach(Appointment appt in listAppointmentsForDate) {//Remove unavailable slots from schedule
				ApptSearchOperatorySchedule apptSearchOperatorySchedule=listOpScheds.FirstOrDefault(x => x.OperatoryNum==appt.Op);
				if(apptSearchOperatorySchedule==null) {
					continue;
				}
				int apptStartIndex=(int)appt.AptDateTime.TimeOfDay.TotalMinutes/5;
				for(int j=0;j<appt.Pattern.Length;j++) {//make unavailable all blocks of time during this appointment
					apptSearchOperatorySchedule.IsOpAvailable[apptStartIndex+j]=false;//set time block to false, meaning something is scheduled here
				}
			}
			#endregion
			#region Remove provider availiabilty for blockouts set to Do Not Schedule
			List<long> listBlockoutsDoNotSchedule=new List<long>();
			List<Def> listBlockoutsAll=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true);
			foreach(Def blockout in listBlockoutsAll) {
				if(blockout.ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
					listBlockoutsDoNotSchedule.Add(blockout.DefNum);//do not return results for blockouts set to 'Do Not Schedule'
					continue;
				}
				if(listBlockoutTypes.Exists(x => x>0) && !listBlockoutTypes.Contains(blockout.DefNum)) {
					listBlockoutsDoNotSchedule.Add(blockout.DefNum);//do not return results for blockout types that are not in listBlockoutTypes
				}
			}
			if(listBlockoutsDoNotSchedule.Count>0) {
				List<Schedule> listBlockouts=listSchedules.FindAll(x => x.ProvNum==0 && x.SchedType==ScheduleType.Blockout && x.SchedDate==scheduleDate 
					&& listBlockoutsDoNotSchedule.Contains(x.BlockoutType));
				foreach(Schedule blockout in listBlockouts) {
					//get length of blockout (how many 5 minute increments does it span)
					TimeSpan duration=blockout.StopTime.Subtract(blockout.StartTime);
					double fiveMinuteIncrements=Math.Ceiling(duration.TotalMinutes/5);
					int blockoutStartIndex=(int)blockout.StartTime.TotalMinutes/5;
					//Set each operatory as unavailable that has this blockout.
					List<ScheduleOp> listSchedOpsForBlockout=listSchedOps.FindAll(x => x.ScheduleNum==blockout.ScheduleNum);
					foreach(ScheduleOp schedOp in listSchedOpsForBlockout) {
						ApptSearchOperatorySchedule apptSearchOperatorySchedule=listOpScheds.FirstOrDefault(x => x.OperatoryNum==schedOp.OperatoryNum);
						if(apptSearchOperatorySchedule==null) {
							continue;
						}
						for(int i=0;i<fiveMinuteIncrements;i++) {
							apptSearchOperatorySchedule.IsOpAvailable[blockoutStartIndex+i]=false;
						}
					}
				}
			}
			#endregion
			//Return all ApptSearchOperatorySchedules for the providers passed in.
			return listOpScheds.FindAll(x => x.ProviderNums.Any(y => listProvNums.Contains(y)));
		}

		///<summary>Directly manipulates the dictionary passed in (key = OpNum, value = list of ProvNums) by adding the provNum to the dictionary's value
		///as needed.  Only adds the provNum for the OpNums passed in.</summary>
		private static void AddProvNumToOps(Dictionary<long,List<long>> dictProvNumsByOp,List<long> listOpNums,long provNum) {
			//No need to check MiddleTierRole; no call to db, private method, manipulates the dictionary passed in.
			if(listOpNums==null || listOpNums.Count < 1) {
				return;
			}
			foreach(long opNum in listOpNums) {
				List<long> listProvNums;
				if(dictProvNumsByOp.TryGetValue(opNum,out listProvNums)) {
					if(listProvNums==null) {
						listProvNums=new List<long>();
					}
					if(listProvNums.Contains(provNum)) {
						continue;
					}
					listProvNums.Add(provNum);
				}
			}
		}

		///<summary>Returns a list of ApiAppointmentSlots. Can return an empty list. Does not consider blockouts.</summary>
		public static List<ApiAppointmentSlot> GetApiSlots(long provNum,DateTime dateStart,DateTime dateEnd,long lengthMinutes,long operatoryNum){
			//Some comments before we get started: (If the user does not provide an operatoryNum.)
			//We don't care which operatories current appointments are in.
			//We don't care which operatories the schedules are assigned to.
			//The only reason we consider ops at all is because our return value has to include an op, but any op for that provider will do.
			List<ApiAppointmentSlot> listApiAppointmentSlots=new List<ApiAppointmentSlot>();
			//1. Get all appointments for these providers for the date range.
			List<Appointment> listAppointments=Appointments.GetForProv(dateStart,dateEnd,provNum);
			//2. Get all schedules for the provider and date range: SchedType=1(prov), ProvNum, SchedDate.
			List<Schedule> listSchedules=Schedules.GetForProv(dateStart,dateEnd,provNum);
			//3. Get associated scheduleops. Each scheduleop is a 1:1 link between ScheduleNum and OperatoryNum.
			List<ScheduleOp> listScheduleOps=ScheduleOps.GetForSchedList(listSchedules);
			//4. User provided an operatoryNum. Remove what we don't want from the lists.
			if(operatoryNum!=0) {
				//Revised behavior: Consider provider's appointments in all operatories, instead of only the operatory the provider is assigned to.
				//Example: Two operatories; OpNum 1, and OpNum 2. Provider is only assigned to OpNum 2.
				//Provider has an appointment in OpNum 1 and an appointment in OpNum 2.
				//Return three available slots (between two operatories) instead of two.
				//Previous behavior: Only list appointments for the operatory that the user asked about.
				//listAppointments.RemoveAll(x => x.Op!=operatoryNum);
				//Only list scheduleOps for the operatory that the user asked about. (Still consider appointments in other operatories.)
				listScheduleOps.RemoveAll(x => x.OperatoryNum!=operatoryNum);
				//Only list the schedules for the operatory that the user asked about.
				listSchedules=listSchedules.Where(x => listScheduleOps.Any(y => y.ScheduleNum==x.ScheduleNum)).ToList();
				//5. The user provided an operatoryNum that the provider is not assigned to. Return an empty list.
				if(listScheduleOps.Count==0) {
					return listApiAppointmentSlots;
				}
			}
			int dayCount=(dateEnd-dateStart).Days;
			for(int i=0;i<dayCount;i++) {//Loop through each day.
				DateTime date=dateStart.AddDays(i);
				bool[] bool5=new bool[60*24/5];//Represents the entire 24 hr day in 5 minute blocks.
				//All blocks start out false.
				for(int j=0;j<listSchedules.Count;j++) { 
					//Mark certain blocks true.
					if(listSchedules[j].SchedDate!=date) {
						continue;
					}
					int indexStart=(int)listSchedules[j].StartTime.TotalMinutes/5;
					int indexEnd=(int)listSchedules[j].StopTime.TotalMinutes/5;
					for(int k=indexStart;k < indexEnd;k++) {
						bool5[k]=true;
					}
				}
				for(int j=0;j<listAppointments.Count;j++) {
					//Mark certain blocks false.
					if(listAppointments[j].AptDateTime.Date!=date) {
						continue;
					}
					int indexStart=(int)listAppointments[j].AptDateTime.TimeOfDay.TotalMinutes/5;
					int indexEnd=(int)listAppointments[j].EndTime.TimeOfDay.TotalMinutes/5;
					for(int k=indexStart;k < indexEnd;k++) {
						bool5[k]=false;
					}
				}
				int blockStart=-1;
				int blockEnd=0;
				for(int j=0;j < bool5.Length;j++) {
					if(bool5[j] && blockStart==-1) {//Beginning of true blocks.
						blockStart=j;
					}
					if(bool5[j]) {//Running endpoint of true blocks.
						blockEnd=j;
						continue;//Keep counting.
					}
					if(blockEnd==0) {//False blocks.
						continue;
					}
					//Final endpoint of true blocks.
					int appointmentLength=(blockEnd+1-blockStart)*5;//Includes the 5 minutes in the last block.
					//Eliminate any slots that are too short.
					//Add remaining slots to final result list.
					if(appointmentLength>=lengthMinutes) { 
						ApiAppointmentSlot apiAppointmentSlot=new ApiAppointmentSlot();
						apiAppointmentSlot.DateTimeStart=date.AddMinutes(blockStart*5);
						apiAppointmentSlot.DateTimeEnd=date.AddMinutes((blockEnd+1)*5);//Includes the 5 minutes in the last block.
						apiAppointmentSlot.ProvNum=provNum;
						listApiAppointmentSlots.Add(apiAppointmentSlot);
					}
					blockStart=-1;//Reset endpoint markers for searching remainder of array.
					blockEnd=0;
				}
				//Pick an op to assign to each slot.
				//First, look for an operatory in which this provider has an appointment scheduled.
				if(listApiAppointmentSlots.Exists(x => x.DateTimeStart.Date==date)) {//Slots were added today.
					//User provided an operatoryNum, use that.
					if(operatoryNum!=0) {
						listApiAppointmentSlots.Where(x => x.DateTimeStart.Date==date).ForEach(x => x.OpNum=operatoryNum);
					}
					else {
						Operatory operatory=Operatories.GetWhere(x => x.ProvDentist==provNum || x.ProvHygienist==provNum).FirstOrDefault();
						if(operatory is null && listScheduleOps.Count==0) {
							//Provider is not assigned to an operatory and not scheduled to an operatory, but is scheduled for the day.
							//OpNum is not included in the API request. No need to set operatoryNum, will default 0, just continue for this rare use case.
						}
						else if(operatory is null) { //If that fails, then look through list of scheduleops to see if one of the schedules for today is linked to an op.
							//If slots were added for this day, then it guarantees a non-empty list of schedules (and therefore ScheduleOps, too).
							operatoryNum=listScheduleOps.FindAll(x => listSchedules.Exists(y => y.ScheduleNum==x.ScheduleNum && y.SchedDate==date)).FirstOrDefault().OperatoryNum;
						}
						else {
							operatoryNum=operatory.OperatoryNum;
						}
						//Assign all slots for today the above operatoryNum.
						listApiAppointmentSlots.Where(x => x.DateTimeStart.Date==date).ForEach(x => x.OpNum=operatoryNum);
					}
				}
			}//Day
			return listApiAppointmentSlots;
		}



	}

	public class ApiAppointmentSlot{
		public DateTime DateTimeStart;
		public DateTime DateTimeEnd;
		public long ProvNum;
		public long OpNum;
	}

	///<summary>Holds information about an operatory's Schedule. Not actual database table.</summary>
	[Serializable]
	public class ApptSearchOperatorySchedule {
		///<summary>FK to Operatory</summary>
		public long OperatoryNum;
		///<summary>Date of the OperatorySchedule.</summary>
		public DateTime SchedDate;
		///<summary>This contains a bool for each 5 minute block throughout the day. True means operatory is open, False means operatory is in use.</summary>
		public bool[] IsOpAvailable;
		///<summary>List of providers 'allowed' to work in this operatory.</summary>
		public List<long> ProviderNums;

	}

	///<summary>The data that we need to get in order to run the appointment search</summary>
	[Serializable]
	public class ApptSearchData {
		///<summary></summary>
		public DateTime DateEvaluating;
		///<summary></summary>
		public Appointment AppointmentToAdd;
		///<summary></summary>
		public List<Schedule> ListSchedules=new List<Schedule>();
		///<summary></summary>
		public List<Appointment> ListAppointments=new List<Appointment>();
		///<summary></summary>
		public List<ScheduleOp> ListSchedOps=new List<ScheduleOp>();
	}

	/// <summary>The information about a single 5 min opening time slot. A single opening will typically have a provider unless it is a blockout.</summary>
	[Serializable]
	public class ScheduleOpening {
		/// <summary> The start time of the 5 min opening for this availability.</summary>
		public DateTime DateTimeAvail=DateTime.MinValue;
		/// <summary> The provider for this opening.  Can be 0 when this opening is representing a blockout.</summary>
		public long ProvNum=0;
		/// <summary> The clinic this opening was found in (this will be the clinic associated to the operatory).</summary>
		public long ClinicNum=0;
		/// <summary> The operatory this opening is for.  Can be 0 if this opening is a dynamic prov schedule.</summary>
		public long OpNum=0;
	}
	
}
