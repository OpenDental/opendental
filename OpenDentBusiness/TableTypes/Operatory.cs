using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary>Each row is a single operatory or column in the appts module.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class Operatory:TableBase{
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long OperatoryNum;
		///<summary>The full name to show in the column.</summary>
		public string OpName;
		///<summary>5 char or less. Not used much.</summary>
		public string Abbrev;
		///<summary>The order that this op column will show.  Changing views only hides some ops; it does not change their order.  Zero based.</summary>
		public int ItemOrder;
		///<summary>Used instead of deleting to hide an op that is no longer used.</summary>
		public bool IsHidden;
		///<summary>FK to provider.ProvNum.  The dentist assigned to this op.  If more than one dentist might be assigned to an op, then create a second op and use one for each dentist. If 0, then no dentist is assigned.</summary>
		public long ProvDentist;
		///<summary>FK to provider.ProvNum.  The hygienist assigned to this op.  If 0, then no hygienist is assigned.</summary>
		public long ProvHygienist;
		///<summary>Set true if this is a hygiene operatory.  The hygienist will then be considered the main provider for this op.</summary>
		public bool IsHygiene;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic.</summary>
		public long ClinicNum;
		///<summary>If true patients put into this operatory will have status set to prospective.</summary>
		public bool SetProspective;
		///<summary>Not user editable. The last time this row was edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Operatories with IsWebSched set to true will be the ONLY operatories considered when searching for available time slots.</summary>
		public bool IsWebSched;
		///<summary>Deprecated as of 18.1.  Entries within the deflink table indicate if this operatory is in fact available for WebSched New Pat Appt.
		///Old summary: Operatories with IsNewPatAppt set to true will be the ONLY operatories considered when searching for available time slots.
		///This is in regards to the New Patient Appointment portion of the Web Sched web application.</summary>
		public bool IsNewPatAppt;

		///<summary>True if the current op is in an HQ view.  Defaults to true for safety.  Not stored in the db.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsInHQView=true;
		///<summary>DefNums of category WebSchedNewPatApptTypes that this operatory is associated to.  Filled within the OperatoryCache.
		///Necessary for the sync method so that DefLink enteries can be made for newly created operatories.
		///Also, used as an indicator that this operatory is ready for WSNPA (replaces IsNewPatAppt bool column above).</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<long> ListWSNPAOperatoryDefNums;
		/// <summary>DefNums of category WebSchedExistingPatApptTypes that this operatory is associated to. Filled within the OperatoryCache.
		/// Used as an indicator that this operatory is ready for WSEP.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<long> ListWSEPOperatoryDefNums;

		///<summary>Returns a copy of this Operatory.</summary>
		public Operatory Copy(){
			return (Operatory)this.MemberwiseClone();
		}

	
	}
	


}













