using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>A historical copy of an appointment.  These are generated as a result of an appointment being edited.  When creating for insertion it needs a passed-in Appointment object.</summary>
	[Serializable]
	[CrudTable(IsLargeTable=true)]
	public class HistAppointment:TableBase {
		#region Not copied from Appointment
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long HistApptNum;
		///<summary>FK to userod.UserNum  Identifies the user that changed this appointment from previous state, not the person who originally wrote it.</summary>
		public long HistUserNum;
		///<summary>The date and time that this appointment was edited and added to the Hist table.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime HistDateTStamp;
		///<summary>Enum:HistAppointmentAction .</summary>
		public HistAppointmentAction HistApptAction;
		///<summary>Enum:EServiceTypes .</summary>
		public EServiceTypes ApptSource;
		//Note that the columns ProcsColored and Note are VARCHAR(255) in this table while they are TEXT in the appointment table. This is intentional
		//because it is less important to store the entire note and color when the appointment is not current.
		#endregion Not copied from Appointment

		#region Copies of Appointment Fields
		///<summary>Copied from Appointment.</summary>
		public long AptNum;
		///<summary>Copied from Appointment.</summary>
		public long PatNum;
		///<summary>Copied from Appointment.</summary>
		public ApptStatus AptStatus;
		///<summary>Copied from Appointment.</summary>
		public string Pattern;
		///<summary>Copied from Appointment.</summary>
		public long Confirmed;
		///<summary>Copied from Appointment.</summary>
		public bool TimeLocked;
		///<summary>Copied from Appointment.</summary>
		public long Op;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string Note;
		///<summary>Copied from Appointment.</summary>
		public long ProvNum;
		///<summary>Copied from Appointment.</summary>
		public long ProvHyg;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime AptDateTime;
		///<summary>Copied from Appointment.</summary>
		public long NextAptNum;
		///<summary>Copied from Appointment.</summary>
		public long UnschedStatus;
		///<summary>Copied from Appointment.</summary>
		public bool IsNewPatient;
		///<summary>Copied from Appointment.</summary>
		public string ProcDescript;
		///<summary>Copied from Appointment.</summary>
		public long Assistant;
		///<summary>Copied from Appointment.</summary>
		public long ClinicNum;
		///<summary>Copied from Appointment.</summary>
		public bool IsHygiene;
		///<summary>Not copied from Appointment. Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeArrived;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSeated;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeDismissed;
		///<summary>Copied from Appointment.</summary>
		public long InsPlan1;
		///<summary>Copied from Appointment.</summary>
		public long InsPlan2;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeAskedToArrive;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public String ProcsColored;
		///<summary>Copied from Appointment.</summary>
		[XmlIgnore]
		public Color ColorOverride;
		///<summary>Copied from Appointment.</summary>
		public long AppointmentTypeNum;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Copied from Appointment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime SecDateTEntry;
		///<summary>Copied from Appointment.</summary>
		public ApptPriority Priority;
		//No SecDateTEdit, DateTStamp already exists and is the timestamp updated by MySQL when a row is added or changed
		///<summary>Copied from Appointment.</summary>
		public string ProvBarText;
		///<summary>Copied from Appointment.</summary>
		public string PatternSecondary;
		#endregion Copies of Task Fields

		///<summary>Pass in the old appointment that needs to be recorded.</summary>
		public HistAppointment(Appointment appt) {
			SetAppt(appt);
		}

		///<summary>Updates the base appointment object but maintains HistAppointment filed values.</summary>
		public void SetAppt(Appointment appt) {
			FieldInfo[] arrayFieldInfos=typeof(Appointment).GetFields();
			for(int i=0;i<arrayFieldInfos.Length;i++)	{
				FieldInfo fieldInfoHist=typeof(HistAppointment).GetField(arrayFieldInfos[i].Name);
				fieldInfoHist.SetValue(this,arrayFieldInfos[i].GetValue(appt));
			}
		}

		///<summary>Converts a histAppointment object to a regular appointment object.</summary>
		public Appointment ToAppt() {
			Appointment appt=new Appointment();
			FieldInfo[] arrayFieldInfos=typeof(Appointment).GetFields();
			for(int i=0;i<arrayFieldInfos.Length;i++)	{
				FieldInfo fieldInfoHist=typeof(HistAppointment).GetField(arrayFieldInfos[i].Name);
				arrayFieldInfos[i].SetValue(appt,fieldInfoHist.GetValue(this));
			}
			return appt;
		}

		public HistAppointment() {
			
		}

		///<summary>Overrides Appointment.Copy() which is desired behavior because HistAppointment extends Appointment.</summary>
		public new HistAppointment Copy() {
			return (HistAppointment)MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum HistAppointmentAction {
		///<summary>0</summary>
		Created,
		///<summary>1</summary>
		Changed,
		///<summary>2</summary>
		Missed,
		///<summary>3</summary>
		Cancelled,
		///<summary>4</summary>
		Deleted,
	}
}
