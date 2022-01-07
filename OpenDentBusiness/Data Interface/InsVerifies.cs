using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using CodeBase;
using OpenDentBusiness.Eclaims;

namespace OpenDentBusiness{
	///<summary></summary>
	public class InsVerifies {
		#region Misc Methods
		
		///<summary>Calculates the time available to verify based on a variety of patient and appointment factors.  
		///Looks at Appointment date, appointment creation date, last verification date, next scheduled verification, insurance renewal
		///</summary>
		public static InsVerify SetTimeAvailableForVerify(InsVerify insVer,PlanToVerify planType,int appointmentScheduledDays,int insBenefitEligibilityDays,
			int patientEnrollmentDays) 
		{
			//DateAppointmentScheduled-DateAppointmentCreated
			DateTime dateTimeLatestApptScheduling;
			//DateAppointmentScheduled-appointmentScheduledDays (default is 7)
			DateTime dateTimeUntilAppt;
			//DateTime the appointment takes place.
			DateTime dateTimeAppointment;
			//DateTime the patient was scheduled to be re-verified
			DateTime dateTimeScheduledReVerify;
			//DateTime verification last took place.  Will be 01/01/0001 if verification has never happened.
			DateTime dateTimeLastVerified;
			//DateTime the insurance renewal month rolls over.
			//The month the renewal takes place is stored in the database.  If the month is 0, then it is actually january. 
			//It is the first day of the given month at midnight, or (Month#)/01/(year) @ 00:00AM. 
			//Set to max val by default in case the PrefName.InsVerifyFutureDateBenefitYear=false.
			//Since max val is later in time than the appointment time, it will be ignored.
			DateTime dateBenifitRenewalNeeded=DateTime.MaxValue;
			#region Appointment Dates
			dateTimeAppointment=insVer.AppointmentDateTime;
			dateTimeUntilAppt=insVer.AppointmentDateTime.AddDays(-appointmentScheduledDays);
			//Calculate when the appointment was put into it's current time slot.
			//This will be the earliest datetime where the scheduled appointment time is what it is now
			List<HistAppointment> listHistAppt=HistAppointments.GetForApt(insVer.AptNum);
			listHistAppt.RemoveAll(x => x.AptDateTime.Date!=insVer.AppointmentDateTime.Date);
			listHistAppt=listHistAppt.Where(x => x.AptStatus==ApptStatus.Scheduled).OrderBy(x => x.AptDateTime).ToList();
			if(listHistAppt.Count>0) {
				//If the appointment was moved to the current date after the (Apt.DateTime-appointmentScheduledDays),
				//we only had (Apt.DateTime-listHistAppt.First().HistDateTstamp) days instead of (appointmentScheduledDays)
				dateTimeLatestApptScheduling=listHistAppt.First().HistDateTStamp;
			}
			else { 
				//Just in case there's no history for an appointment for some reason.
				//Shouldn't happen because a log entry is created when the appointment is created.
				//Use the date the appointment was created.  This is better than nothing and should never happen anyways.
				dateTimeLatestApptScheduling=Appointments.GetOneApt(insVer.AptNum).SecDateTEntry;
			}
			#endregion Appointment Dates
			#region Insurance Verification
			dateTimeLastVerified=insVer.DateLastVerified;
			//Add defined number of days to date last verified to calculate when the next verification date should have started.
			if(planType==PlanToVerify.InsuranceBenefits) {
				if(insVer.DateLastVerified==DateTime.MinValue) {//If it's the min value, the insurance has never been verified.
					dateTimeScheduledReVerify=insVer.DateTimeEntry;
				}
				else {
					dateTimeScheduledReVerify=insVer.DateLastVerified.AddDays(insBenefitEligibilityDays);
				}
			}
			else {//PlanToVerify.PatientEligibility
				if(insVer.DateLastVerified==DateTime.MinValue) {
					dateTimeScheduledReVerify=insVer.DateTimeEntry;
				}
				else {
					dateTimeScheduledReVerify=insVer.DateLastVerified.AddDays(patientEnrollmentDays);
				}
			}
			#endregion insurance verification
			#region Benifit Renewal
			if(PrefC.GetBool(PrefName.InsVerifyFutureDateBenefitYear)) {
				InsPlan insPlan=InsPlans.GetPlan(insVer.PlanNum,null);
				//Setup the month renew dates.  Need all 3 years in case the appointment verify window crosses over a year
				//e.g. Appt verify date: 12/30/2016 and Appt Date: 1/6/2017
				DateTime dateTimeOldestRenewal=new DateTime(DateTime.Now.Year-1,Math.Max((byte)1,insPlan.MonthRenew),1);
				DateTime dateTimeMiddleRenewal=new DateTime(DateTime.Now.Year,Math.Max((byte)1,insPlan.MonthRenew),1);
				DateTime dateTimeNewestRenewal=new DateTime(DateTime.Now.Year+1,Math.Max((byte)1,insPlan.MonthRenew),1);
				//We want to find the date closest to the appointment date without going past it.
				if(dateTimeMiddleRenewal>dateTimeAppointment) {
					dateBenifitRenewalNeeded=dateTimeOldestRenewal;
				}
				else {
					if(dateTimeNewestRenewal>dateTimeAppointment) {
						dateBenifitRenewalNeeded=dateTimeMiddleRenewal;
					}
					else {
						dateBenifitRenewalNeeded=dateTimeNewestRenewal;
					}
				}
			}
			#endregion Benifit Renewal
			DateTime dateTimeAbleToVerify=VerifyDateCalulation(dateTimeUntilAppt,dateTimeLatestApptScheduling,dateTimeScheduledReVerify,dateBenifitRenewalNeeded);
			insVer.HoursAvailableForVerification=insVer.AppointmentDateTime.Subtract(dateTimeAbleToVerify).TotalHours;
			return insVer;
		}
		
		///<summary>This calculates the datetime when a patient would appear on the "Needs verification" list.  </summary>
		///<param name="dateTimeDaysUntilAppt">The date and time to begin considering appointments for verification purposes.  
		///Calculated DateTime from AptDateTime - PrefName.InsVerifyDaysFromPastDueAppt</param>
		///<param name="dateTimeApptLastScheduled">The date and time of the most recent move of the appointment to the schedule.</param>
		///<param name="dateTimeVerificationExpired">The date and time that the verification has become invalid.
		///Calculated DateTime from DateTimeLastVerify + (PrefName.InsVerifyBenefitEligibilityDays or PrefName.InsVerifyPatientEnrollmentDays)</param>
		///<param name="dateBenefitRenewalNeeded">The date that the insurance benefit year expires and needs to be renewed regardless of the DateTimeLastVerify.</param>
		public static DateTime VerifyDateCalulation(DateTime dateTimeDaysUntilAppt,DateTime dateTimeApptLastScheduled,DateTime dateTimeVerificationExpired,DateTime dateBenefitRenewalNeeded) {
			//The date and time that the insurance verification has expired.  If it expired due to a benefit renewal, the time portion will assume midnight.
			DateTime dateTimeVerificationFirstNeeded=new DateTime(Math.Min(dateTimeVerificationExpired.Ticks, dateBenefitRenewalNeeded.Ticks));
			//The date and time that the patient associated to the patient was put on the verification list (this would happen for either plan or benefit insverify types)
			//To show on the verification list, an appointment must be made, and a verification must have expired.
			//Because of this, we get the most recent requirement.  This ensures the exact moment both requirements were present.
			DateTime dateTimeShowInVerificationList=new DateTime(Math.Max(dateTimeApptLastScheduled.Ticks, dateTimeVerificationFirstNeeded.Ticks));
			//The final requirement to show on the verification list is that the appointment needs to be X days away or sooner.
			//Because of this, we compare the X days and the exact date and time that the appointment requirements were met, and take the most recent one, since both need to be met.
			return new DateTime(Math.Max(dateTimeDaysUntilAppt.Ticks, dateTimeShowInVerificationList.Ticks));
		}
		#endregion
		
		///<summary>Gets one InsVerify from the db that has the given fkey and verify type.</summary>
		public static InsVerify GetOneByFKey(long fkey,VerifyTypes verifyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<InsVerify>(MethodBase.GetCurrentMethod(),fkey,verifyType);
			}
			string command="SELECT * FROM insverify WHERE FKey="+POut.Long(fkey)+" AND VerifyType="+POut.Int((int)verifyType)+"";
			return Crud.InsVerifyCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(InsVerify insVerify) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),insVerify);
				return;
			}
			Crud.InsVerifyCrud.Update(insVerify);
		}
		
		///<summary>Inserts a default InsVerify into the database based on the passed in patplan.  Used when inserting a new patplan.
		///Returns the primary key of the new InsVerify.</summary>
		public static long InsertForPatPlanNum(long patPlanNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),patPlanNum);
			}
			InsVerify insVerify=new InsVerify();
			insVerify.VerifyType=VerifyTypes.PatientEnrollment;
			insVerify.FKey=patPlanNum;
			return Crud.InsVerifyCrud.Insert(insVerify);
		}
		
		///<summary>Inserts a default InsVerify into the database based on the passed in insplan.  Used when inserting a new insplan.
		///Returns the primary key of the new InsVerify.</summary>
		public static long InsertForPlanNum(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),planNum);
			}
			InsVerify insVerify=new InsVerify();
			insVerify.VerifyType=VerifyTypes.InsuranceBenefit;
			insVerify.FKey=planNum;
			return Crud.InsVerifyCrud.Insert(insVerify);
		}
		
		///<summary>Deletes an InsVerify with the passed in FKey and VerifyType.</summary>
		public static void DeleteByFKey(long fkey,VerifyTypes verifyType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fkey,verifyType);
				return;
			}
			string command="DELETE FROM insverify WHERE FKey="+POut.Long(fkey)+" AND VerifyType="+POut.Int((int)verifyType);
			Db.NonQ(command);
		}

		public static List<InsVerify> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsVerify>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM insverify";
			return Crud.InsVerifyCrud.SelectMany(command);
		}

		public static List<long> GetAllInsVerifyUserNums() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT DISTINCT UserNum FROM insverify";
			return Db.GetListLong(command);
		}

		///<summary>UserNum=-1 is "All", UserNum=0 is "Unassigned". 
		///statusDefNum=-1 or statusDefNum=0 is "All".  
		///listClinicNums containing -1 is "All". listClinicNums containing 0 is "Unassigned". 
		///listRegionDefNums containing 0 or -1 is "All".</summary>
		public static List<InsVerifyGridObject> GetVerifyGridList(DateTime startDate, DateTime endDate,DateTime datePatEligibilityLastVerified
			,DateTime datePlanBenefitsLastVerified,List<long> listClinicNums,List<long> listRegionDefNums,long statusDefNum
			,long userNum,string carrierName,bool excludePatVerifyWhenNoIns,bool excludePatClones) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsVerifyGridObject>>(MethodBase.GetCurrentMethod(),startDate,endDate,datePatEligibilityLastVerified
					,datePlanBenefitsLastVerified,listClinicNums,listRegionDefNums,statusDefNum,userNum,carrierName,excludePatVerifyWhenNoIns,excludePatClones);
			}
			//clinicJoin should only be used if the passed in clinicNum is a value other than 0 (Unassigned).
			string whereClinic="";
			if(listClinicNums.Contains(-1)) {//All clinics
				whereClinic="AND (clinic.IsInsVerifyExcluded=0 OR clinic.ClinicNum IS NULL) ";
				if(!listRegionDefNums.Contains(0) && !listRegionDefNums.Contains(-1) && listRegionDefNums.Count>0) {//Specific region
					whereClinic+=" AND clinic.Region IN("+string.Join(",",listRegionDefNums.Select(x => POut.Long(x)))+") ";
				}
			}
			else if(listClinicNums.Contains(0)) {//Unassigned clinics
				whereClinic="AND clinic.ClinicNum IS NULL ";
				if(listClinicNums.Count(x => x!=0)>0) {//Also has specific clinics selected
					whereClinic="AND (clinic.ClinicNum IS NULL OR ";
					whereClinic+="(clinic.IsInsVerifyExcluded=0 AND clinic.ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
					if(!listRegionDefNums.Contains(0) && !listRegionDefNums.Contains(-1) && listRegionDefNums.Count>0) {//Specific region
						whereClinic+=" AND clinic.Region IN("+string.Join(",",listRegionDefNums.Select(x => POut.Long(x)))+") ";
					}
					whereClinic+=")) ";
				}
			}
			else if(listClinicNums.Count>0) {//Specific Clinic
				whereClinic="AND clinic.IsInsVerifyExcluded=0 AND clinic.ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
				if(!listRegionDefNums.Contains(0) && !listRegionDefNums.Contains(-1) && listRegionDefNums.Count>0) {//Specific region
					whereClinic+=" AND clinic.Region IN("+string.Join(",",listRegionDefNums.Select(x => POut.Long(x)))+") ";
				}
			}
			bool checkBenefitYear=PrefC.GetBool(PrefName.InsVerifyFutureDateBenefitYear);
			string mainQuery=@"
				SELECT insverify.*,
				patient.LName,patient.FName,patient.Preferred,appointment.PatNum,appointment.AptNum,appointment.AptDateTime,patplan.PatPlanNum,insplan.PlanNum,carrier.CarrierName,
				COALESCE(clinic.Abbr,'None') AS ClinicName,appointment.ClinicNum,inssub.InsSubNum,carrier.CarrierNum
				FROM appointment 
				LEFT JOIN clinic ON clinic.ClinicNum=appointment.ClinicNum 
				INNER JOIN patient ON patient.PatNum=appointment.PatNum 
				INNER JOIN patplan ON patplan.PatNum=appointment.PatNum 
				INNER JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum 
				INNER JOIN insplan ON insplan.PlanNum=inssub.PlanNum 
					"+(excludePatVerifyWhenNoIns ? "AND insplan.HideFromVerifyList=0" : "")+@"
				INNER JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum 
					"+(string.IsNullOrEmpty(carrierName) ? "" : "AND carrier.CarrierName LIKE '%"+POut.String(carrierName)+"%'")+@"
				"+(excludePatClones ? "LEFT JOIN patientlink ON patientlink.PatNumTo=patient.PatNum AND patientlink.LinkType="
					+POut.Int((int)PatientLinkType.Clone)+" " : "");
			string insVerifyJoin1=@"INNER JOIN insverify ON 
					(insverify.VerifyType="+POut.Int((int)VerifyTypes.InsuranceBenefit)+@" 
					AND insverify.FKey=insplan.PlanNum 
					AND (insverify.DateLastVerified<"+POut.Date(datePlanBenefitsLastVerified)+@"
						"+(checkBenefitYear?@"OR (insverify.DateLastVerified<DATE_FORMAT(appointment.AptDateTime,CONCAT('%Y-',LPAD(insplan.MonthRenew,2,'0'),'-01')) 
							AND DATE_FORMAT(appointment.AptDateTime,CONCAT('%Y-',LPAD(MonthRenew,2,'0'),'-01'))<="+DbHelper.DtimeToDate("appointment.AptDateTime")+")":"")+@") 
					"+(excludePatVerifyWhenNoIns ? "" : "AND insplan.HideFromVerifyList=0")+@") ";
			string insVerifyJoin2=@"INNER JOIN insverify ON 
					(insverify.VerifyType="+POut.Int((int)VerifyTypes.PatientEnrollment)+@"
					AND insverify.FKey=patplan.PatPlanNum
					AND (insverify.DateLastVerified<"+POut.Date(datePatEligibilityLastVerified)+@"
						"+(checkBenefitYear?@"OR (insverify.DateLastVerified<DATE_FORMAT(appointment.AptDateTime,CONCAT('%Y-',LPAD(MonthRenew,2,'0'),'-01')) 
							AND DATE_FORMAT(appointment.AptDateTime,CONCAT('%Y-',LPAD(MonthRenew,2,'0'),'-01'))<="+DbHelper.DtimeToDate("appointment.AptDateTime")+")":"")+@"))	";
			string whereClause=@"
				WHERE appointment.AptDateTime BETWEEN "+DbHelper.DtimeToDate(POut.Date(startDate))+" AND "+DbHelper.DtimeToDate(POut.Date(endDate.AddDays(1)))+@" 
				AND appointment.AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.Complete)+@")
				"+(userNum==-1 ? "" : "AND insverify.UserNum="+POut.Long(userNum))+@"
				"+(statusDefNum<1 ? "" : "AND insverify.DefNum="+POut.Long(statusDefNum))+@"
				"+(excludePatClones ? "AND patientlink.PatNumTo IS NULL" : "")+@"
				"+whereClinic;
			//Previously we joined the insverify table using a large OR clause. This caused MySQL to not be able to use any index on the insverify table.
			//Now we run two unioned queries, each with a different join clause for the insverify table, so that MySQL can use insverify.FKKey as an index.
			string command=
				mainQuery+
				insVerifyJoin1+
				whereClause+@"
				UNION ALL
				"+
				mainQuery+
				insVerifyJoin2+
				whereClause+@"
				ORDER BY AptDateTime";
			DataTable table=Db.GetTable(command);
			List<InsVerify> listInsVerifies=Crud.InsVerifyCrud.TableToList(table);
			List<InsVerifyGridObject> retVal=new List<InsVerifyGridObject>();
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow row=table.Rows[i];
				InsVerify insVerifyCur=listInsVerifies[i].Clone();
				insVerifyCur.PatNum=PIn.Long(row["PatNum"].ToString());
				insVerifyCur.PlanNum=PIn.Long(row["PlanNum"].ToString());
				insVerifyCur.PatPlanNum=PIn.Long(row["PatPlanNum"].ToString());
				insVerifyCur.ClinicName=PIn.String(row["ClinicName"].ToString());
				string patName=PIn.String(row["LName"].ToString())
					+", ";
				if(PIn.String(row["Preferred"].ToString())!="") {
					patName+="'"+PIn.String(row["Preferred"].ToString())+"' ";
				}
				patName+=PIn.String(row["FName"].ToString());
				insVerifyCur.PatientName=patName;
				insVerifyCur.CarrierName=PIn.String(row["CarrierName"].ToString());
				insVerifyCur.AppointmentDateTime=PIn.DateT(row["AptDateTime"].ToString());
				insVerifyCur.AptNum=PIn.Long(row["AptNum"].ToString());
				insVerifyCur.ClinicNum=PIn.Long(row["ClinicNum"].ToString());//Non DB column. Used in OpenDentalService.
				insVerifyCur.InsSubNum=PIn.Long(row["InsSubNum"].ToString());//Non DB column. Used in OpenDentalService.
				insVerifyCur.CarrierNum=PIn.Long(row["CarrierNum"].ToString());//Non DB column. Used in OpenDentalService.
				if(insVerifyCur.VerifyType==VerifyTypes.InsuranceBenefit) {
					InsVerifyGridObject gridObjPlanExists=retVal.FirstOrDefault(x => x.PlanInsVerify!=null && x.PlanInsVerify.PlanNum==insVerifyCur.PlanNum);
					if(gridObjPlanExists==null) {
						InsVerifyGridObject gridObjExists=retVal.FirstOrDefault(x => x.PatInsVerify!=null 
							&& x.PatInsVerify.PatPlanNum==insVerifyCur.PatPlanNum 
							&& x.PatInsVerify.PlanNum==insVerifyCur.PlanNum 
							&& x.PatInsVerify.Note==insVerifyCur.Note 
							&& x.PatInsVerify.DefNum==insVerifyCur.DefNum 
							&& x.PlanInsVerify==null);
						if(gridObjExists!=null) {
							gridObjExists.PlanInsVerify=insVerifyCur;
						}
						else {
							retVal.Add(new InsVerifyGridObject(plan:insVerifyCur));
						}
					}
				}
				else if(insVerifyCur.VerifyType==VerifyTypes.PatientEnrollment) {
					InsVerifyGridObject gridObjPatExists=retVal.FirstOrDefault(x => x.PatInsVerify!=null && x.PatInsVerify.PatPlanNum==insVerifyCur.PatPlanNum);
					if(gridObjPatExists==null) {
						InsVerifyGridObject gridObjExists=retVal.FirstOrDefault(x => x.PlanInsVerify!=null 
						&& x.PlanInsVerify.PlanNum==insVerifyCur.PlanNum 
						&& x.PlanInsVerify.Note==insVerifyCur.Note 
						&& x.PlanInsVerify.DefNum==insVerifyCur.DefNum 
						&& x.PatInsVerify==null);
						if(gridObjExists!=null) {
							gridObjExists.PatInsVerify=insVerifyCur;
						}
						else {
							retVal.Add(new InsVerifyGridObject(pat:insVerifyCur));
						}
					}
				}
			}
			return retVal;
		}

		public static void CleanupInsVerifyRows(DateTime startDate, DateTime endDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),startDate,endDate);
				return;
			}
			//Nathan OK'd the necessity for a complex update query like this to avoid looping through update statements.  This will be changed to a crud update method sometime in the future.
			string command="";
			List<long> listInsVerifyNums=Db.GetListLong(GetInsVerifyCleanupQuery(startDate,endDate));
			if(listInsVerifyNums.Count==0) {
				return;
			}
			command="UPDATE insverify "
				+"SET insverify.DateLastAssigned='0001-01-01', "
				+"insverify.DefNum=0, "
				+"insverify.Note='', "
				+"insverify.UserNum=0 "
				+"WHERE insverify.InsVerifyNum IN ("+string.Join(",",listInsVerifyNums)+")";
			Db.NonQ(command);
		}

		private static string GetInsVerifyCleanupQuery(DateTime startDate, DateTime endDate) {
			return @"SELECT InsVerifyNum
				FROM (
					SELECT InsVerifyNum,patplan.PatNum
					FROM patplan
					INNER JOIN inssub ON inssub.InsSubNum=patplan.InsSubNum
					INNER JOIN insplan ON insplan.PlanNum=inssub.PlanNum
						AND insplan.HideFromVerifyList=0
					INNER JOIN insverify ON VerifyType="+POut.Int((int)VerifyTypes.InsuranceBenefit)+@"
						AND insverify.FKey=insplan.PlanNum
					WHERE insverify.DateLastAssigned>'0001-01-01'
					AND insverify.DateLastAssigned<"+POut.Date(DateTime.Today.AddDays(-30))+@"
				
					UNION
					
					SELECT InsVerifyNum,patplan.PatNum
					FROM patplan
					INNER JOIN insverify ON VerifyType="+POut.Int((int)VerifyTypes.PatientEnrollment)+@"
						AND insverify.FKey=patplan.PatPlanNum
					WHERE insverify.DateLastAssigned>'0001-01-01'
					AND insverify.DateLastAssigned<"+POut.Date(DateTime.Today.AddDays(-30))+@"
				) insverifies
				LEFT JOIN appointment ON appointment.PatNum=insverifies.PatNum
					AND appointment.AptStatus IN ("+POut.Int((int)ApptStatus.Scheduled)+","+POut.Int((int)ApptStatus.Complete)+@")
					AND "+DbHelper.DtimeToDate("appointment.AptDateTime")+" BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate)+@"
				GROUP BY insverifies.InsVerifyNum
				HAVING MAX(appointment.AptNum) IS NULL";
		}

		///<summary>Called in OpenDentalService.  Attempts to verify patient benefits for same list in FormInsVerificaitonList.cs.
		///Only runs for carriers that are flagged with TrustedEtransTypes.RealTimeEligibility.</summary>
		public static List<InsVerify> TryBatchPatInsVerify(LogWriter logger=null) {
			//Mimics FormInsVerificaitonList.GetRowsForGrid(...)
			bool excludePatVerifyWhenNoIns=PrefC.GetBool(PrefName.InsVerifyExcludePatVerify);
			bool excludePatClones=(PrefC.GetBool(PrefName.ShowFeaturePatientClone) && PrefC.GetBool(PrefName.InsVerifyExcludePatientClones));
			DateTime dateTimeStart=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyDaysFromPastDueAppt));//Mimics past due ins verifies logic
			DateTime dateTimeEnd=DateTime.Today.AddDays(PrefC.GetInt(PrefName.InsVerifyAppointmentScheduledDays));//Non past due logic
			DateTime dateTimeLastPatEligibility=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays));
			DateTime dateTimeLastPlanBenefits=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays));
			logger?.WriteLine($"BatchPatInsVerify has started...\r\n" +
				$"dateTimeStart={dateTimeStart}\r\n" +
				$"dateTimeEnd={dateTimeEnd}\r\n" +
				$"dateTimeLastPatEligibility={dateTimeLastPatEligibility}\r\n" +
				$"dateTimeLastPlanBenefits={dateTimeLastPlanBenefits}",LogLevel.Verbose,"InsVerifyBatch");
			List<InsVerifyGridObject> listInsVerify=GetVerifyGridList(dateTimeStart,dateTimeEnd,dateTimeLastPatEligibility,dateTimeLastPlanBenefits
				,new List<long>(){ -1 }//All clinics
				,new List<long>(){ 0 }//All regions
				,0//All statuses
				,-1//All userNums
				,""//All carriers 
				,excludePatVerifyWhenNoIns
				,excludePatClones
			);
			logger?.WriteLine($"{listInsVerify.Count} insverify grid objects",LogLevel.Verbose,"InsVerifyBatch");
			Dictionary<long,Carrier> dictTrustedCarriers=null;//Key: CarrierNum, Value: Carrier
			Dictionary<long,InsSub> dictInsSubs=null;//Key: InsSubNum, Value: InsSub
			Dictionary<long,InsPlan> dictInsPlans=null;//Key: PlanNum, Value: InsPlan
			long errorStatusDefNum=-1;//FK to defNum associated to DefCat.InsuranceVerificationStatus 'ServiceError' def.
			if(listInsVerify.Count>0){//Avoid queries/logic if not necessary, but still update PrefName.InsVerifyServiceBatchLastRunDate below.
				dictTrustedCarriers=Carriers.GetWhere(x => x.TrustedEtransFlags.HasFlag(TrustedEtransTypes.RealTimeEligibility))
					.ToDictionary(x => x.CarrierNum,x => x);
				List<long> listInsSubNums=listInsVerify.Where(x => x.PatInsVerify!=null).Select(x => x.PatInsVerify.InsSubNum).ToList();
				//Eventually we might incoroprate ins plan verification
				//listInsSubNums.AddRange(listInsVerify.Where(x => x.PlanInsVerify!=null).Select(x => x.PlanInsVerify.InsSubNum).ToList());
				dictInsSubs=InsSubs.GetMany(listInsSubNums).ToDictionary(x => x.InsSubNum, x => x);
				dictInsPlans=InsPlans.GetByInsSubs(listInsSubNums).ToDictionary(x => x.PlanNum, x => x);
				errorStatusDefNum=Defs.GetByExactName(DefCat.InsuranceVerificationStatus,"ServiceError");//0 if not found
			}
			List<InsVerify> listInsVerifies=new List<InsVerify>();
			foreach(InsVerifyGridObject insVerifyObj in listInsVerify) {
				try {
					listInsVerifies.Add(insVerifyObj.PatInsVerify);
					if(insVerifyObj.IsOnlyInsRow() || !dictTrustedCarriers.ContainsKey(insVerifyObj.PatInsVerify.CarrierNum)) {
						if(insVerifyObj.PatInsVerify!=null) {//We've seen this be null before for a real customer. For now only effect is tied to unit test. Safe to skip if null.
							insVerifyObj.PatInsVerify.BatchVerifyState=BatchInsVerifyState.Skipped;
						}
						continue;
					}
					//Either only PatInsVerify or Both.
					//When Both we only request and verify the PatIns.
					string errorStatus="";
					Etrans etransRequest=null;
					etransRequest=x270Controller.TryInsVerifyRequest(insVerifyObj.PatInsVerify,dictInsPlans[insVerifyObj.PatInsVerify.PlanNum]
						,dictTrustedCarriers[insVerifyObj.PatInsVerify.CarrierNum],dictInsSubs[insVerifyObj.PatInsVerify.InsSubNum],out errorStatus
					);//Can be null
					if(errorStatus.IsNullOrEmpty()) {//No error yet.
						if(etransRequest==null) {//Can happen when an AAA segment is returned.
							errorStatus=Lans.g("InsVerifyService","Unexpected carrier response.");
						}
						else {//Success, no errors so far and etrans returned
							bool isCoinsuranceInverted=dictTrustedCarriers[insVerifyObj.PatInsVerify.CarrierNum].IsCoinsuranceInverted;
							//AckEtrans and MessageText are not DB columns but are always set in x270.RequestBenefits(...). This is done to avoid queries.
							X271 x271=new X271(etransRequest.AckEtrans.MessageText);
							//Per NADG we should be considering both in-network and out-of-network benefits
							List<EB271> listEb271=x271.GetListEB(true,isCoinsuranceInverted);
							listEb271.AddRange(x271.GetListEB(false,isCoinsuranceInverted));
							List<Benefit> listBensForPat=Benefits.RefreshForPlan(insVerifyObj.PatInsVerify.PlanNum,insVerifyObj.PatInsVerify.PatPlanNum);
							//If the benefits received from 271 are valid, continue with further validation
							if(x271.IsValidForBatchVerification(listEb271,isCoinsuranceInverted,out errorStatus)) {
								string strGroupNumInOd=dictInsPlans[insVerifyObj.PatInsVerify.PlanNum].GroupNum;
								string strGroupNumIn271=x271.GetGroupNum();
								errorStatus+=ValidateGroupNumber(strGroupNumInOd,strGroupNumIn271);
								#region Validate Plan dates
								DateTime datePlanStart=DateTime.MinValue;
								DateTime datePlanEnd=DateTime.MinValue;
								List<DTP271> listPlanDates=x271.GetListDtpSubscriber();
								List<DTP271> listPlanStartDates=listPlanDates.FindAll(x => x.Segment.Get(1)=="539");//539 => Policy Effective
								List<DTP271> listPlanEndDates=listPlanDates.FindAll(x => x.Segment.Get(1)=="540");//540 => Policy Expiration
								if(listPlanStartDates.Count==0 && listPlanEndDates.Count==0) {
									//Use plan dates if no policy dates were received
									listPlanStartDates=listPlanDates.FindAll(x => x.Segment.Get(1)=="346");//346 => Plan Start.
									listPlanEndDates=listPlanDates.FindAll(x => x.Segment.Get(1)=="347");//347 => Plan End.
								}
								//If the 271 specifies more than 1 date we will always use the last one for both plan start and plan end.
								if(listPlanStartDates.Count>0) {
									datePlanStart=X12Parse.ToDate(listPlanStartDates.Last().Segment.Get(3));//Mimics FormInsPlan.butGetElectronic_Click(...)
								}
								if(listPlanEndDates.Count>0) {
									datePlanEnd=X12Parse.ToDate(listPlanEndDates.Last().Segment.Get(3));//Mimics FormInsPlan.butGetElectronic_Click(...)
								}
								errorStatus+=ValidatePlanDates(datePlanStart,datePlanEnd,dictInsSubs[insVerifyObj.PatInsVerify.InsSubNum],insVerifyObj.PatInsVerify.AptNum);
								#endregion
								//The age old classic of short and sweet or long and descriptive.
								errorStatus+=ValidateAnnualMaxAndGeneralDeductible(listBensForPat,listEb271.Select(x=>x.Benefitt).ToList());
								if(string.IsNullOrWhiteSpace(errorStatus)) {
									CreateInsuranceAdjustmentIfNeeded(insVerifyObj.PatInsVerify.PatNum,insVerifyObj.PatInsVerify.PlanNum,
										insVerifyObj.PatInsVerify.InsSubNum,listBensForPat,listEb271);
									EB271.SetInsuranceHistoryDates(listEb271,insVerifyObj.GetPatNum(),InsSubs.GetOne(insVerifyObj.PatInsVerify.InsSubNum));
								}
							}
						}
					}
					if(string.IsNullOrWhiteSpace(errorStatus)) {
						InsVerifyOnVerify(insVerifyObj);
						insVerifyObj.PatInsVerify.BatchVerifyState=BatchInsVerifyState.Success;
					}
					else {//Error occurred
						InsVerifySetStatus(insVerifyObj,errorStatusDefNum,errorStatus);
						insVerifyObj.PatInsVerify.BatchVerifyState=BatchInsVerifyState.Error;
						logger?.WriteLine($"Validation errors for patnum {insVerifyObj.GetPatNum()}:",LogLevel.Verbose,"InsVerifyBatch");
						logger?.WriteLine($"{errorStatus}",LogLevel.Verbose,"InsVerifyBatch");
					}
				}
				catch(Exception ex) {
					logger?.WriteLine($"Exception was thrown on patnum: {insVerifyObj.GetPatNum()}",LogLevel.Verbose,"InsVerifyBatch");
					logger?.WriteLine($"Exception text: {ex.StackTrace}",LogLevel.Verbose,"InsVerifyBatch");
				}
			}
			logger?.WriteLine("BatchPatInsVerify has ended...",LogLevel.Verbose,"InsVerifyBatch");
			return listInsVerifies;
		}

		///<summary>Called when an error occurs and we want to update an insVerifyObj.Note.  Currently ony has pat ins verify logic.</summary>
		private static void InsVerifySetStatus(InsVerifyGridObject insVerifyObj,long errorPatInsDefNum,string errorNotePatIns) {
			//Mimics FormInsVerificationList.SetStatus(...)
			if(insVerifyObj.IsOnlyInsRow()) {
				//Eventually we might incoroprate ins plan verification
			}
			else {//Pat only or Both
				insVerifyObj.PatInsVerify.DefNum=errorPatInsDefNum;
				insVerifyObj.PatInsVerify.Note=errorNotePatIns;
				insVerifyObj.PatInsVerify.DateLastAssigned=DateTime.Today;
				Update(insVerifyObj.PatInsVerify);
			}
		}

		///<summary>Called after recieving a valid 271 response when verifying patient insurance benefits.
		///Currently only has pat ins verify logic (not insurnace plan logic).</summary>
		private static void InsVerifyOnVerify(InsVerifyGridObject insVerifyObj){
			int apptSchedDays=PrefC.GetInt(PrefName.InsVerifyAppointmentScheduledDays);
			int insBenefitEligibilityDays=PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays);
			int patEnrollmentDays=PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays);
			//Mimics the logic in FormInsVerificationList.OnVerify(...)
			insVerifyObj.PatInsVerify=SetTimeAvailableForVerify(insVerifyObj.PatInsVerify,
				PlanToVerify.PatientEligibility,apptSchedDays,patEnrollmentDays,insBenefitEligibilityDays);
			insVerifyObj.PatInsVerify.DateLastVerified=DateTime.Today;
			InsVerifyHists.InsertFromInsVerify(insVerifyObj.PatInsVerify);//This also updates the insVerifyObj.PatInsVerify InsVerify DB record.
			//Eventually we might incoroprate ins plan verification
		}

		///<summary>Checks to see if the group number in OD matches the group number in the 271 response. Returns "" for partial matches with the insplan group num
		///or if no group number was found in the 271.</summary>
		public static string ValidateGroupNumber(string insPlanGroupNum,string x271GroupNum) {
			if(String.IsNullOrWhiteSpace(insPlanGroupNum) || insPlanGroupNum.Length<3) {
				return Lans.g("InsVerifyService",$"Group number on insurance plan is invalid, current:{insPlanGroupNum}, received:{x271GroupNum}. ");
			}
			else if(String.IsNullOrWhiteSpace(x271GroupNum) || x271GroupNum.Length<3) {//If we receive an invalid or empty group number, assume what is in OD is correct.
				return "";
			}
			else if(x271GroupNum.StartsWith(insPlanGroupNum) || x271GroupNum.EndsWith(insPlanGroupNum)) {
				return "";
			}
			return Lans.g("InsVerifyService",$"Group number mismatch, current:{insPlanGroupNum}, received:{x271GroupNum}. ");
		}

		///<summary>Checks to see if a policy start and policy end date was specified in the given x271 object. 
		///Will always update InsSub.DateTerm if a valid policy start date was received, but only updates InsSub.DateEffective if a valid date was receieved.
		///Returns an error string if the patient's appointment date does not fall in the range of the received date(s).</summary>
		public static string ValidatePlanDates(DateTime datePolicyStart,DateTime datePolicyEnd,InsSub insSub,long aptNum) {
			//No policy date information was received from the 271, nothing to validate.
			if(datePolicyStart.Year<=1880 && datePolicyEnd.Year<=1880) {
				return "";
			}
			//Only update the policy start date if we get a valid value.
			if(datePolicyStart.Year>=1880) {
				insSub.DateEffective=datePolicyStart;
			}
			//If we are sent an invalid start date, but a valid end date, we will update just the end date. Verified with NADG.
			insSub.DateTerm=datePolicyEnd;
			InsSubs.Update(insSub);
			DateTime aptDate=Appointments.GetOneApt(aptNum).AptDateTime.Date;
			if(datePolicyEnd.Year<=1880 && datePolicyStart>aptDate) {//No end date, but we have a start date, and plan starts in the future
				return Lans.g("InsVerifyService",$"Policy does not start until {datePolicyStart.ToShortDateString()} ");
			}
			else if(datePolicyStart.Year<=1880 && datePolicyEnd<aptDate) {//No start date, but we have an end date, and plan ended in the past
				return Lans.g("InsVerifyService",$"Inactive coverage.  Policy ended {datePolicyEnd.ToShortDateString()} ");
			}
			//Carriers will sometimes send a valid start date, but a plan end date of Datetime.MinValue. This is considered a valid scenario and must be excluded from our validation.
			else if(datePolicyEnd.Year>1880 && !DateTime.Today.Between(datePolicyStart,datePolicyEnd)) {
				return Lans.g("InsVerifyService",$"Invalid policy dates: {datePolicyStart.ToShortDateString()} - {datePolicyEnd.ToShortDateString()} ");
			}
			return "";
		}
		
		///<summary>Given a list of benefits for the patient's plan and a list of benefits received in a 271 response. Determines if the general deductible and annual
		///max on the patient's insurance plan has a different amount than that received in the 271. Checks individual and family coverage level. Returns all errors as a string.</summary>
		public static string ValidateAnnualMaxAndGeneralDeductible(List<Benefit> listPlanBenefits,List<Benefit> list271Benefits) {
			Benefit annualMaxInd=null;
			Benefit annualMaxFam=null;
			Benefit generalDeductInd=null;
			Benefit generalDeductFam=null;
			//Find our current general deductible and annual max benefits.
			//If duplicate benefits the last one will be considered.  This mimics the behavior in FormInsBenefits.cs.
			foreach(Benefit ben in listPlanBenefits) {
				if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Individual)) {
					annualMaxInd=ben;
				}
				else if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Family)) {
					annualMaxFam=ben;
				}
				else if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Individual)) {
					generalDeductInd=ben;
				}
				else if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Family)) {
					generalDeductFam=ben;
				}
			}
			//Construct a list of annual max and general deductible benefits specified in the 271. 
			List<Benefit> listAnnualMaxInd271=new List<Benefit>();
			List<Benefit> listAnnualMaxFam271=new List<Benefit>();
			List<Benefit> listGeneralDeductInd271=new List<Benefit>();
			List<Benefit> listGeneralDeductFam271=new List<Benefit>();
			foreach(Benefit ben in list271Benefits) {
				if(ben==null) {//Most EB271.Benefitt objects will be null.
					continue;
				}
				if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Individual)) {
					listAnnualMaxInd271.Add(ben);
				}
				else if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Family)) {
					listAnnualMaxFam271.Add(ben);
				}
				else if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Individual)) {
					listGeneralDeductInd271.Add(ben);
				}
				else if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Family)) {
					listGeneralDeductFam271.Add(ben);
				}
			}
			//If the 271 does not specify a value, we will not do the comparison and consider the data in OD to be correct.
			//If ANY benefit segment matches the associated value in OD, then the annual max/general deductible benefit will be considered valid.
			//However, if the current benefit amount in OD is 0, and the 271 specifies an additional non-zero amount (for the same benefit),
			//we will flag this patient as needing manual correction.
			StringBuilder strBuildErrorStatus=new StringBuilder();
			if(listAnnualMaxInd271.Count>0) {
				if(annualMaxInd==null || ListTools.In(annualMaxInd.MonetaryAmt,0,-1) || !listAnnualMaxInd271.Any(x=>x.MonetaryAmt==annualMaxInd.MonetaryAmt)) {
					strBuildErrorStatus.AppendLine(Lans.g("InsVerifyService","Individual annual max mismatch. "));
				}
			}
			if(listAnnualMaxFam271.Count>0) {
				if(annualMaxFam==null || ListTools.In(annualMaxFam.MonetaryAmt,0,-1) || !listAnnualMaxFam271.Any(x=>x.MonetaryAmt==annualMaxFam.MonetaryAmt)) {
					strBuildErrorStatus.AppendLine(Lans.g("InsVerifyService","Family annual max mismatch. "));
				}
			}
			if(listGeneralDeductInd271.Count>0) {
				if(generalDeductInd==null || ListTools.In(generalDeductInd.MonetaryAmt,0,-1) || !listGeneralDeductInd271.Any(x=>x.MonetaryAmt==generalDeductInd.MonetaryAmt)) {
					strBuildErrorStatus.AppendLine(Lans.g("InsVerifyService","Individual general deductible mismatch. "));
				}
			}
			if(listGeneralDeductFam271.Count>0) {
				if(generalDeductFam==null || ListTools.In(generalDeductFam.MonetaryAmt,0,-1) || !listGeneralDeductFam271.Any(x=>x.MonetaryAmt==generalDeductFam.MonetaryAmt)) {
					strBuildErrorStatus.AppendLine(Lans.g("InsVerifyService","Family general deductible mismatch. "));
				}
			}
			return strBuildErrorStatus.ToString();
		}

		///<summary>This method determines whether an insurance adjustment needs to be made given the current benefits in Open Dental and the EB271 segments from the 271 response.
		///An insurance adjustment will be made if there is an individual general deductible or annual max specified in Open Dental and the 
		///271 response specifies a 'remaining' benefit amount for the associated benefit (deductible or annual max).  Only one insurance adjustment will be made.</summary>
		public static void CreateInsuranceAdjustmentIfNeeded(long patNum,long planNum,long insSubNum,List<Benefit> listDbBens,List<EB271> list271Bens) {
			Benefit generalDeductInd=null;
			Benefit annualMaxInd=null;
			foreach(Benefit ben in listDbBens) {
				if(Benefits.IsGeneralDeductible(ben,BenefitCoverageLevel.Individual)) {
					generalDeductInd=ben;
				}
				else if(Benefits.IsAnnualMax(ben,BenefitCoverageLevel.Individual)) {
					annualMaxInd=ben;
				}
			}
			ClaimProc insAdj=ClaimProcs.CreateInsPlanAdjustment(patNum,planNum,insSubNum);
			foreach(EB271 eb in list271Bens) {
				//Kick out early if the segment doesn't have the elements needed
				if(String.IsNullOrEmpty(eb.Segment.Get(6))) {//A time period qualifier must be specified in the 271 segment
					continue;
				}
				//Per Mark, we will not import general deductible from 271 if we do not already have one entered in the database.  See job.
				if(generalDeductInd!=null && IsIndGeneralDeductRemaining(eb)) {
					double rem271DeductAmt=PIn.Double(eb.Segment.Get(7));
					//Make sure the general deductible amount remaining is less than the 271 amount.  We don't want to make a negative insurance adjustment.
					if(rem271DeductAmt <= generalDeductInd.MonetaryAmt) {
						insAdj.DedApplied=generalDeductInd.MonetaryAmt-rem271DeductAmt;
					}
				}
				//Per Mark, we will not import annual max from 271 if we do not already have one entered in the database.  See job.
				else if(annualMaxInd!=null && IsIndAnnualMaxRemaining(eb)) {
					double rem271GenMaxAmt=PIn.Double(eb.Segment.Get(7));
					//Make sure the annual max amount remaining is less than the 271 amount.  We don't want to make a negative insurance adjustment.
					if(rem271GenMaxAmt <= annualMaxInd.MonetaryAmt) {
						insAdj.InsPayAmt=annualMaxInd.MonetaryAmt-rem271GenMaxAmt;
					}
				}
			}
			if(insAdj.DedApplied!=0 || insAdj.InsPayAmt!=0) {
				ClaimProcs.Insert(insAdj);
			}
		}

		///<summary>Checks the if the received EB271 segment is an individual general deductible 'remaining' benefit. 
		///Remaining benefit segments are flagged with a time period qualifier value of "29".</summary>
		public static bool IsIndGeneralDeductRemaining(EB271 eb) {
			//Check to see if the given eb271 segment represents an individual general deductible.
			//For example: EB*C*IND*35**DG PLUS, NON CONTRACTED*29*50.00*****U~ should result in a match with 50 being returned.
			if(eb.Segment.Get(1)=="C"//Deductible
				&& eb.Segment.Get(2)=="IND"//Individual
				&& ListTools.In(eb.Segment.Get(3),"","35")//Some carriers send "" and 23 for service type code. 
				&& eb.Segment.Get(6)=="29")//Remaining
			{
				return true;
			}
			return false;
		}

		///<summary>Checks the if the received EB271 segment is an individual annual max 'remaining' benefit. 
		///Remaining benefit segments are flagged with a time period qualifier value of "29".</summary>
		public static bool IsIndAnnualMaxRemaining(EB271 eb) {
			//We found a 'remaining' benefit segment. Check to see if it represents an individual general annual max.
			//For example: EB*F*IND*35**DG PLUS, NON CONTRACTED*29*2000.00*****U~ should result in a match with 2000 being returned.
			if(eb.Segment.Get(1)=="F"//Limitation
				&& eb.Segment.Get(2)=="IND"//Individual
				&& ListTools.In(eb.Segment.Get(3),"","35")//Some carriers send "" and 23 for service type code. 
				&& eb.Segment.Get(6)=="29")//Remaining
			{
				return true;
			}
			return false;
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern

		private class InsVerifyCache : CacheListAbs<InsVerify> {
			protected override List<InsVerify> GetCacheFromDb() {
				string command="SELECT * FROM InsVerify ORDER BY ItemOrder";
				return Crud.InsVerifyCrud.SelectMany(command);
			}
			protected override List<InsVerify> TableToList(DataTable table) {
				return Crud.InsVerifyCrud.TableToList(table);
			}
			protected override InsVerify Copy(InsVerify InsVerify) {
				return InsVerify.Clone();
			}
			protected override DataTable ListToTable(List<InsVerify> listInsVerifys) {
				return Crud.InsVerifyCrud.ListToTable(listInsVerifys,"InsVerify");
			}
			protected override void FillCacheIfNeeded() {
				InsVerifys.GetTableFromCache(false);
			}
			protected override bool IsInListShort(InsVerify InsVerify) {
				return !InsVerify.IsHidden;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static InsVerifyCache _InsVerifyCache=new InsVerifyCache();

		///<summary>A list of all InsVerifys. Returns a deep copy.</summary>
		public static List<InsVerify> ListDeep {
			get {
				return _InsVerifyCache.ListDeep;
			}
		}

		///<summary>A list of all visible InsVerifys. Returns a deep copy.</summary>
		public static List<InsVerify> ListShortDeep {
			get {
				return _InsVerifyCache.ListShortDeep;
			}
		}

		///<summary>A list of all InsVerifys. Returns a shallow copy.</summary>
		public static List<InsVerify> ListShallow {
			get {
				return _InsVerifyCache.ListShallow;
			}
		}

		///<summary>A list of all visible InsVerifys. Returns a shallow copy.</summary>
		public static List<InsVerify> ListShort {
			get {
				return _InsVerifyCache.ListShallowShort;
			}
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_InsVerifyCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_InsVerifyCache.FillCacheFromTable(table);
				return table;
			}
			return _InsVerifyCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		*/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<InsVerify> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<InsVerify>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM insverify WHERE PatNum = "+POut.Long(patNum);
			return Crud.InsVerifyCrud.SelectMany(command);
		}
		*/
	}

	public enum PlanToVerify {
		///<summary>Used when we neet to verify both PatientEligibility and InsuranceBenefits.</summary>
		Both,
		///<summary>Used when we need to verify that a specific patient is covered by a specific insurance.</summary>
		PatientEligibility,
		///<summary>Used when we need to verify an insurance plan and insurance plan benefits.</summary>
		InsuranceBenefits
	}

	///<summary>Enum used for UnitTest only.</summary>
	public enum BatchInsVerifyState{
		///<summary>1 - InsVerify was either a ins plan verification or was not associated to trusted carrier.</summary>
		Skipped,
		///<summary>2 - InsVerify attempted but failed.</summary>
		Error,
		///<summary>3 - InsVerify request set and valid response recieved.</summary>
		Success,
	}
}