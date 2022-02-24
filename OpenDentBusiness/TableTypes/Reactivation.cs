using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Track patient contact via a commlog type ("Reactivation").  
	///Any commlogs of this type that occur after the last completed procedure will be considered a contact attempt.
	///Patients should show in this list if they have previously completed procedures (excluding broken/canceled), 
	///and the most recent was completed before the time span specified by the "Days Past" preference.  
	///Include Patients with the following PatStatus: Patient, Inactive, Prospective
	///Patients should not show in this list if they have been marked "Do not contact".
	///Patients should not show in this list if a future appointment is scheduled.
	///Once contacted, Patient should not show in this list.  Patient will later reappear in this list if the "Reactivation contact interval" time 
	///period passes since the last contact and an appointment has not yet been scheduled.  
	///If the patient is contacted the maximum number of times specified by the "Count Contact Max" preference, mark the patient as "Do Not Contact".  
	///Example:
	///Johnny Patient had his last procedure completed on 1/1/2018.  There is a "Reactivation" type commlog on his chart from 6/1/2018.  
	///He does not have any future scheduled appointments.  Johnny would be included in the list of "Reactivation" patients, with a single contact 
	///attempt having been made already.</summary>
	[Serializable]
	public class Reactivation:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ReactivationNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to definition.DefNum. Uses the existing RecallUnschedStatus DefCat.</summary>
		public long ReactivationStatus;
		///<summary>An administrative note for staff use.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ReactivationNote;
		///<summary>The patient can set this property if they don't want to be contacted so that it won't interfere with the max attempts to contact option.</summary>
		public bool DoNotContact;

		///<summary>Returns a copy of this Reactivation.</summary>
		public Reactivation Copy(){
			return (Reactivation)this.MemberwiseClone();
		}
	}
}