using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Links one planned appointment to one patient.  Allows multiple planned appointments per patient.  Also see the PlannedIsDone field. A planned appointment is an appointment that will show in the Chart module and in the Planned appointment tracker. It will never show in the Appointments module. In other words, it is the suggested next appoinment rather than an appointment that has already been scheduled.</summary>
	[Serializable()]
	public class PlannedAppt:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PlannedApptNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to appointment.AptNum.</summary>
		public long AptNum;
		///<summary>One-indexed order of item in group of planned appts.</summary>
		public int ItemOrder;
		
		public PlannedAppt Copy(){
			return (PlannedAppt)this.MemberwiseClone();
		}	
	}
}
