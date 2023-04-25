using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace OpenDentBusiness {
	public class RpAppointments {
		///<summary>If not using clinics then supply an empty list of clinicNums.</summary>
		public static DataTable GetAppointmentTable(DateTime dateStart,DateTime dateEnd,List<long> listProvNums,List<long> listClinicNums,
			bool hasClinicsEnabled,bool isShowRecall,bool isShowNewPat,bool isShowASAP,bool isShowExistingPat,SortAndFilterBy sortBy,List<ApptStatus> listApptStatusesToExclude,
			List<long> listConfirmationStatuses,string formSender) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateStart,dateEnd,listProvNums,listClinicNums,hasClinicsEnabled,
					isShowRecall,isShowNewPat,isShowASAP,isShowExistingPat,sortBy,listApptStatusesToExclude,listConfirmationStatuses,formSender);
			}
			//Appointment status conditions
			string whereApptStatus="";
			if(listApptStatusesToExclude.Count > 0) {
				whereApptStatus+=" appointment.AptStatus NOT IN ("+string.Join(",",listApptStatusesToExclude.Select(x => POut.Int((int)x)))+") AND ";
			}
			//Provider Conditions
			string whereProv="";
			if(listProvNums.Count > 0) {
				whereProv+=" (appointment.ProvNum IN("+string.Join(",",listProvNums)+") "
					+" OR appointment.ProvHyg IN("+string.Join(",",listProvNums)+")) AND ";
			}
			//Clinic Conditions
			string whereClinics="";
			if(hasClinicsEnabled && listClinicNums.Count > 0) {
				whereClinics+=" appointment.ClinicNum IN("+string.Join(",",listClinicNums)+") AND ";
			}
			//Appointment confirmation conditions
			string whereConfStatus="";
			if(listConfirmationStatuses.Count > 0) {
				whereConfStatus+=" appointment.Confirmed IN ("+string.Join(",",listConfirmationStatuses)+") AND ";
			}
			//WebSched Appointments
			string innerJoinWebSchedBoth=BuildWebSchedInnerJoin(dateStart,isShowNewPat,isShowRecall,isShowASAP,isShowExistingPat);
			//Query
			string command = @"SELECT ";
			if(sortBy==SortAndFilterBy.SecDateTEntry) {
				command+="appointment.SecDateTEntry,";
			}
			command += 
				@"appointment.AptDateTime,
				patient.PatNum,
				TRIM(CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),CASE WHEN LENGTH(patient.Preferred) > 0 THEN CONCAT(CONCAT('(',patient.Preferred),') ') ELSE '' END),patient.FName), ' '),patient.MiddleI)) PatName,
				patient.Birthdate,
				appointment.AptDateTime,
				LENGTH(appointment.Pattern)*5 AptLength,
				appointment.ProcDescript,
				patient.HmPhone,
				patient.WkPhone,
				patient.WirelessPhone,
				COALESCE(clinic.Description,'"+POut.String(Lans.g("formSender","Unassigned"))+@"') ClinicDesc,
				appointment.SecDateTEntry AS 'DateTimeCreated',
				appointment.Confirmed,
				appointment.Note,
				appointment.AptNum
				FROM appointment
				INNER JOIN patient ON appointment.PatNum=patient.PatNum "
				+innerJoinWebSchedBoth+
				@" LEFT JOIN clinic ON appointment.ClinicNum=clinic.ClinicNum 
				WHERE "
				+whereApptStatus
				+whereProv
				+whereClinics
				+whereConfStatus;
			if(sortBy==SortAndFilterBy.SecDateTEntry) {
				command+=" "+DbHelper.BetweenDates("appointment.SecDateTEntry",dateStart,dateEnd)
					+" ORDER BY appointment.ClinicNum,appointment.SecDateTEntry,PatName";
			}
			else if(sortBy==SortAndFilterBy.AptDateTime) {
				command+=" appointment.AptDateTime BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd.AddDays(1))
					+" ORDER BY appointment.ClinicNum,appointment.AptDateTime,PatName";
			}
			DataTable table=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(command));
			return table;
		}

		///<summary>If the dateStart for the report is equal to or earlier than the date that they updated to or beyond version 21.1,
		///we use the securitylog table to filter appointments in the query, otherwise, we use the eservicelog table. We want to
		///get away from using the securitylog table, but didn't start collecting eservicelog data until version 21.1.</summary>
		private static string BuildWebSchedInnerJoin(DateTime dateStart,bool isShowNewPat,bool isShowRecall,bool isShowASAP,bool isShowExistingPat) {
			List<LogSources> listSources=new List<LogSources>();
			List<eServiceType> listEserviceTypes=new List<eServiceType>();
			if(isShowNewPat) {
				listSources.Add(LogSources.WebSchedNewPatAppt);
				listEserviceTypes.Add(eServiceType.WSNewPat);
			}
			if(isShowRecall) {
				listSources.Add(LogSources.WebSched);
				listEserviceTypes.Add(eServiceType.WSRecall);
			}
			if(isShowASAP) {
				listSources.Add(LogSources.WebSchedASAP);
				listEserviceTypes.Add(eServiceType.WSAsap);
			}
			if(isShowExistingPat) {
				listSources.Add(LogSources.WebSchedExistingPatient);
				listEserviceTypes.Add(eServiceType.WSExistingPat);
			}
			DateTime dateUpdateToVersion21_1=UpdateHistories.GetDateForVersion(new Version(21,1,0,0));
			string innerJoinWebSchedBoth="";
			if(dateStart.Date <= dateUpdateToVersion21_1.Date) {
				if(listSources.Count>0) {
					innerJoinWebSchedBoth=" INNER JOIN securitylog ON appointment.AptNum=securitylog.FKey"
						+" AND securitylog.PermType="+POut.Int((int)Permissions.AppointmentCreate)
						+" AND securitylog.LogSource IN ("+string.Join(",",listSources.Select(x => (int)x))+") ";
				}
			}
			else {
				if(listEserviceTypes.Count>0) {
					innerJoinWebSchedBoth=" INNER JOIN eservicelog ON appointment.AptNum=eservicelog.FKey"
						+" AND eservicelog.EserviceAction="+POut.Int((int)eServiceAction.WSAppointmentScheduledFromServer)
						+" AND eservicelog.EServiceType IN ("+string.Join(",",listEserviceTypes.Select(x => (int)x))+") ";
				}
			}
			return innerJoinWebSchedBoth;
		}

		///<summary>Set the date that RpAppointments.GetAppointmentTable will use to sort and filter results.</summary>
		public enum SortAndFilterBy {
			///<summary>Sort and filter on appointment.SecDateTEntry</summary>
			SecDateTEntry,
			///<summary>Sort and filter on appointment.AptDateTime</summary>
			AptDateTime,
		}

	}

}
