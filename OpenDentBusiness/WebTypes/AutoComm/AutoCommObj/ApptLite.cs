using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenDentBusiness.AutoComm {
	///<summary>This class contains fields that are useful in appointment-type AutoComms.</summary>
	public class ApptLite : AutoCommObj {
		///<summary>From Appointment table.</summary>
		public ApptStatus AptStatus;
		///<summary>From Appointment table.</summary>
		public DateTime AptDateTime;
		///<summary>From Appointment table.</summary>
		public DateTime DateTimeAskedToArrive;
		///<summary>Indicates the name of the Office.</summary>
		public string OfficeName;
		///<summary>Indicates the phone number of the Office.</summary>
		public string OfficePhone;
		///<summary>Indicates the email of the Office.</summary>
		public string OfficeEmail;
		///<summary>From Appointment table, in minutes.</summary>
		public int Length;
		///<summary>Physical address of the office</summary>
		public string OfficeAddress;

		public ApptLite() {
			//Required for PatPortInviteLite
		}

		public ApptLite(Appointment appt,PatComm patComm,bool isForThankYou=false) {
			PrimaryKey=appt.AptNum;
			//For most AutoCommApptAbs, this will be AptDateTime, but for ApptThankYous, SecDateTEntry is used.
			DateTimeEvent=isForThankYou ? appt.SecDateTEntry : appt.AptDateTime;
			AptDateTime=appt.AptDateTime;
			DateTimeAskedToArrive=appt.DateTimeAskedToArrive;
			if(DateTimeAskedToArrive.Year<1880) {
				DateTimeAskedToArrive=AptDateTime;
			}
			AptStatus=appt.AptStatus;
			ClinicNum=appt.ClinicNum;
			PatNum=appt.PatNum;
			ProvNum=appt.ProvNum;
			Clinic clinic=(appt.ClinicNum==0) ? Clinics.GetPracticeAsClinicZero() : Clinics.GetClinic(appt.ClinicNum);
			OfficeName=Clinics.GetOfficeName(clinic);
			OfficePhone=Clinics.GetOfficePhone(clinic);
			OfficeEmail=EmailAddresses.GetByClinic(clinic?.ClinicNum??0).EmailUsername;
			OfficeAddress=Clinics.GetOfficeAddress(clinic);
			Length=appt.Length;
			SetPatientContact(patComm,new Dictionary<long,PatComm>());
		}

		public ApptLite(DataRow row,List<ClinicRule> listClinicRules) {
			PrimaryKey=PIn.Long(row["AptNum"].ToString());
			//For most AutoCommApptAbs, this will be AptDateTime, but for ApptThankYous, SecDateTEntry is used.
			DateTimeEvent=PIn.DateT(row["DateTimeEvent"].ToString());
			AptDateTime=PIn.DateT(row["AptDateTime"].ToString());
			DateTimeAskedToArrive=PIn.DateT(row["DateTimeAskedToArrive"].ToString());
			if(DateTimeAskedToArrive.Year<1880) {
				DateTimeAskedToArrive=AptDateTime;
			}
			AptStatus=(ApptStatus)PIn.Int(row["AptStatus"].ToString());
			ClinicNum=PIn.Long(row["ClinicNum"].ToString());
			PatNum=PIn.Long(row["PatNum"].ToString());
			ProvNum=PIn.Long(row["ProvNum"].ToString());
			Clinic clinic=listClinicRules.FirstOrDefault(x => x.RuleClinic.ClinicNum==ClinicNum)?.RuleClinic??new Clinic();
			OfficeName=OpenDentBusiness.Clinics.GetOfficeName(clinic);
			OfficePhone=OpenDentBusiness.Clinics.GetOfficePhone(clinic);
			OfficeEmail=EmailAddresses.GetByClinic(clinic.ClinicNum).EmailUsername;
			OfficeAddress=Clinics.GetOfficeAddress(clinic);
			Length=PIn.Int(row["AptLength"].ToString());
		}

		public ApptThankYouSent.CalendarIcsInfo ToCalendarIcs() {
			return new ApptThankYouSent.CalendarIcsInfo() {
				PatNum=PatNum,
				Title=ClinicPrefs.GetPrefValue(PrefName.ApptThankYouCalendarTitle,ClinicNum),
				Location=$"{OfficeName} {OfficeAddress}",
				AptNum=PrimaryKey,
				DateStart=AptDateTime,
				DateEnd=AptDateTime.AddMinutes(Length),
				OfficeEmail=OfficeEmail,
				Method=ApptThankYouSent.CalendarIcsInfo.CalMethod.Request,
			};
		}

		
	}
}
