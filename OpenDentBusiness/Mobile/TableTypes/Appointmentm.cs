using System;
using System.Collections;

namespace OpenDentBusiness.Mobile{
	///<summary>Appointments can show in the Appointments module, or they can be on the unscheduled list.  An appointment object is also used to store the Planned appointment.  The planned appointment never gets scheduled, but instead gets copied.</summary>
	[Serializable()]
	[CrudTable(IsMobile=true)]
	public class Appointmentm:TableBase{
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long AptNum;
		///<summary>FK to patient.PatNum.  The patient that the appointment is for.</summary>
		public long PatNum;
		///<summary>Enum:ApptStatus .</summary>
		public ApptStatus AptStatus;
		///<summary>Time pattern, X for Dr time, / for assist time. Stored in 5 minute increments.  Converted as needed to 10 or 15 minute representations for display.</summary>
		public string Pattern;
		///<summary>FK to definition.DefNum.  This field can also be used to show patient arrived, in chair, etc.  The Category column in the definition table is DefCat.ApptConfirmed.</summary>
		public long Confirmed;
		///<summary>FK to operatory.OperatoryNum.</summary>
		public long Op;
		///<summary>Note.</summary>
		public string Note;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>FK to provider.ProvNum.  Optional.  Only used if a hygienist is assigned to this appt.</summary>
		public long ProvHyg;
		///<summary>Appointment Date and time.  If you need just the date or time for an SQL query, you can use DATE(AptDateTime) and TIME(AptDateTime) in your query.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime AptDateTime;
		///<summary>This is the first appoinment this patient has had at this office.  Somewhat automated.</summary>
		public bool IsNewPatient;
		///<summary>A one line summary of all procedures.  Can be used in various reports, Unscheduled list, and Planned appointment tracker.  Not user editable right now, so it doesn't show on the screen.</summary>
		public string ProcDescript;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic.</summary>
		public long ClinicNum;
		///<summary>Set true if this is a hygiene appt.  The hygiene provider's color will show.</summary>
		public bool IsHygiene;

		///<summary>Returns a copy of the appointment.</summary>
    public Appointmentm Clone(){
			return (Appointmentm)this.MemberwiseClone();
		}

		
	}
	
	


}









