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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			string innerJoinWebSchedBoth = "";
			List<LogSources> listSources = new List<LogSources>();
			if(isShowNewPat) {
				listSources.Add(LogSources.WebSchedNewPatAppt);
			}
			if(isShowRecall) {
				listSources.Add(LogSources.WebSched);
			}
			if(isShowASAP) {
				listSources.Add(LogSources.WebSchedASAP);
			}
			if(isShowExistingPat) {
				listSources.Add(LogSources.WebSchedExistingPatient);
			}
			if(listSources.Count>0) {
				List<int> listPerms=new List<int>() { (int)Permissions.AppointmentCreate };
				if(sortBy==SortAndFilterBy.SecurityLogDate) {
					listPerms.Add((int)Permissions.AppointmentEdit);
					listPerms.Add((int)Permissions.AppointmentMove);
				}
				innerJoinWebSchedBoth=" INNER JOIN securitylog ON appointment.AptNum=securitylog.FKey"
					+" AND securitylog.PermType IN ("+string.Join(",",listPerms.Select(x => POut.Int(x)))+") "
					+" AND securitylog.LogSource IN ("+string.Join(",",listSources.Select(x=>(int)x))+") ";
			}
			//Query
			string command = @"SELECT ";
			if(sortBy==SortAndFilterBy.SecDateTEntry) {
				command+="appointment.SecDateTEntry,";
			}
			if(sortBy==SortAndFilterBy.SecurityLogDate) {
				command+="securitylog.LogDateTime,";
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
			else if(sortBy==SortAndFilterBy.SecurityLogDate && listSources.Count>0) {
				command+=" securitylog.LogDateTime BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd.AddDays(1))
					+" ORDER BY securitylog.LogDateTime,appointment.AptDateTime,PatName";
			}
			DataTable table=ReportsComplex.RunFuncOnReportServer(() => ReportsComplex.GetTable(command));
			return table;
		}

		///<summary>Set the date that RpAppointments.GetAppointmentTable will use to sort and filter results.</summary>
		public enum SortAndFilterBy {
			///<summary>Sort and filter on appointment.SecDateTEntry</summary>
			SecDateTEntry,
			///<summary>Sort and filter on appointment.AptDateTime</summary>
			AptDateTime,
			///<summary>Sort and filter on securitylog.LogDateTime</summary>
			SecurityLogDate
		}

	}

}
