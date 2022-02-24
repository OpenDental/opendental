using Newtonsoft.Json;
using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	
	///<summary>Appointments can show in the Appointments module, or they can be on the unscheduled list.  An appointment object is also used to store the Planned appointment.  The planned appointment never gets scheduled, but instead gets copied.  Also see histappointment, which keeps a historical record.</summary>
	[Serializable()]
	[CrudTable(IsDeleteForbidden=true,IsSynchable=true,IsSecurityStamped=true,IsLargeTable=true,
		AuditPerms=CrudAuditPerm.AppointmentCompleteEdit|CrudAuditPerm.AppointmentCreate|CrudAuditPerm.AppointmentEdit|CrudAuditPerm.AppointmentMove)]
	public class Appointment:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AptNum;
		///<summary>FK to patient.PatNum.  The patient that the appointment is for.</summary>
		public long PatNum;
		///<summary>Enum:ApptStatus .</summary>
		public ApptStatus AptStatus;
		///<summary>Time pattern, X for Dr time, / for assist time. Stored in 5 minute increments.  Converted as needed to 10 or 15 minute representations for display.  There's not a hard limit on this.  When dragging, the max is 6.5 hours.  Within the AptEdit window, it can be set to 9 hours.</summary>
		public string Pattern;
		///<summary>FK to definition.DefNum.  This field can also be used to show patient arrived, in chair, etc.  The Category column in the definition table is DefCat.ApptConfirmed.</summary>
		public long Confirmed;
		///<summary>If true, then the program will not attempt to reset the user's time pattern and length when adding or removing procedures.</summary>
		public bool TimeLocked;
		///<summary>FK to operatory.OperatoryNum.</summary>
		public long Op;
		///<summary>Note.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string Note;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>FK to provider.ProvNum.  Optional.  Only used if a hygienist is assigned to this appt.</summary>
		public long ProvHyg;
		///<summary>Appointment Date and time.  If you need just the date or time for an SQL query, you can use DATE(AptDateTime) and TIME(AptDateTime) in your query.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime AptDateTime;
		///<summary>FK to appointment.AptNum.  A better description of this field would be PlannedAptNum.  Only used to show that this apt is derived from specified planned apt. Otherwise, 0.</summary>
		public long NextAptNum;
		///<summary>FK to definition.DefNum.  The definition.Category in the definition table is DefCat.RecallUnschedStatus.  Only used if this is an Unsched or Planned appt.</summary>
		public long UnschedStatus;
		///<summary>This is the first appoinment this patient has had at this office.  Somewhat automated.</summary>
		public bool IsNewPatient;
		///<summary>A one line summary of all procedures.  Can be used in various reports, Unscheduled list, and Planned appointment tracker.  Not user editable right now, so it doesn't show on the screen.</summary>
		public string ProcDescript;
		///<summary>FK to employee.EmployeeNum.  You can assign an assistant to the appointment.</summary>
		public long Assistant;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic.</summary>
		public long ClinicNum;
		///<summary>Set true if this is a hygiene appt. This flag is frequently not set even when it is a hygiene appointment because some offices want the dentist color on the appointments.</summary>
		public bool IsHygiene;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program updates.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>The date and time that the patient checked in.  Date is largely ignored since it should be the same as the appt.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeArrived;
		///<summary>The date and time that the patient was seated in the chair in the operatory.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSeated;
		///<summary>The date and time that the patient got up out of the chair.  Date is largely ignored since it should be the same as the appt.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeDismissed;
		///<summary>FK to insplan.PlanNum for the primary insurance plan at the time the appointment is set complete. May be 0. We can't tell later which subscriber is involved; only the plan.</summary>
		public long InsPlan1;
		///<summary>FK to insplan.PlanNum for the secoondary insurance plan at the time the appointment is set complete. May be 0. We can't tell later which subscriber is involved; only the plan.</summary>
		public long InsPlan2;
		///<summary>Date and time patient asked to arrive, or minval if patient not asked to arrive at a different time than appt.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeAskedToArrive;
		///<summary>Stores XML for the procs colors</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public String ProcsColored;
		///<summary>If set to anything but 0, then this will override the graphic color for the appointment.
		///Typically set to the color of the corresponding appointment type (if one is set) or a color manually picked by the user.</summary>
		[XmlIgnore]
		public Color ColorOverride;
		///<summary>FK to appointmenttype.AppointmentTypeNum.
		///Make sure to update ColorOverride to the corresponding color associated to this appointment type when changing the appointment type.</summary>
		public long AppointmentTypeNum;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>Enum:ApptPriority Indicates if the appointment has any special priority.</summary>
		public ApptPriority Priority;
		//No SecDateTEdit, DateTStamp already exists and is the timestamp updated by MySQL when a row is added or changed
		///<summary>Text that is superimposed on the provbar at the left of each appointment.  One character per 10 or 15 minute increment, not per 5 min.</summary>
		public string ProvBarText;
		///<summary>Time pattern, X for secondary provider time, / for spacing. Stored in 5 minute increments.  Converted as needed to 10 or 15 minute representations for display.  This could be Dr or Hyg, depending on if the IsHyg box is checked.  Does not have any effect on appointment length.  Probably same length as Pattern, but no guarantee.</summary>
		public string PatternSecondary;
		//NOTE: If adding any more columns, be sure to add them to HistAppointment and to the constructor for HistAppointment.

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorOverride",typeof(int))]
		public int ColorOverrideXml {
			get {
				return ColorOverride.ToArgb();
			}
			set {
				ColorOverride=Color.FromArgb(value);
			}
		}

		///<summary>Length of the appointment in minutes.</summary>
		[XmlIgnore,JsonIgnore]
		public int Length {
			get {
				return Pattern.Length*5;
			}
		}

		///<summary>Ending time for the appointment.</summary>
		[XmlIgnore,JsonIgnore]
		public DateTime EndTime {
			get {
				return AptDateTime.AddMinutes(Pattern.Length*5);
			}
		}

		///<summary>Returns a copy of the appointment.</summary>
		public Appointment Copy(){
			return (Appointment)this.MemberwiseClone();
		}

		
	}
	
	


}









