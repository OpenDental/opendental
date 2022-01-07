using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class HouseCallsQueries {

		public static DataTable GetHouseCalls(DateTime FromDate,DateTime ToDate){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),FromDate,ToDate);
			}
			//now, the query--------------------------------------------------------------------------
			//Appointment Reminder Fields- numbers are as they come back from db-----------------------
			//0-LastName
			//1-FirstName (or we substitute 2-Preferred Name if exists)
			// PatientNumber (Can be 3-PatNum or 4-ChartNumber, depending on what user selected)
			//5-HomePhone
			//6-WorkNumber
			//7-EmailAddress
			// SendEmail (this will be true if email address exists. Might change later)
			//8-Address
			//9-Address2 (although they did not offer this as an option)
			//10-City
			//11-State
			//12-Zip
			//13-ApptDate
			//13-ApptTime
			//14-ApptReason (procedures descriptions-user can't edit)
			//15-DoctorNumber (for the Doctor, we currently use the patient primary provider. Otherwise, we would run into trouble with appointments assigned to a specific hygienist.)
			//15-DoctorName
			//16-IsNewPatient
			//17-WirelessPhone
			string command=@"SELECT patient.LName,patient.FName,patient.Preferred
				,patient.PatNum,patient.ChartNumber,patient.HmPhone,patient.WkPhone
				,patient.Email,patient.Address,patient.Address2,patient.City,patient.State
				,patient.Zip
				,appointment.AptDateTime,appointment.ProcDescript
				,patient.PriProv
				,appointment.IsNewPatient,
				patient.WirelessPhone
				FROM patient,appointment 
				WHERE patient.PatNum=appointment.PatNum "
				+"AND (appointment.AptStatus=1 OR appointment.AptStatus=4) "//sched or ASAP
				+"AND appointment.AptDateTime > "+POut.Date(FromDate)//> midnight
				+" AND appointment.AptDateTime < "+POut.Date(ToDate.AddDays(1));//< midnight
			return Db.GetTable(command);
		}

	}
}
