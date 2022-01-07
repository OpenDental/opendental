using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>Multiple subscribers can have the same insurance plan.  But the patplan table is still what determines coverage for individual patients.</summary>
	[Serializable]
	[CrudTable(IsSecurityStamped=true,AuditPerms=CrudAuditPerm.LogSubscriberEdit,IsLargeTable=true)]
	public class InsSub:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsSubNum;
		///<summary>FK to insplan.PlanNum.</summary>
		public long PlanNum;
		///<summary>FK to patient.PatNum.</summary>
		public long Subscriber;
		///<summary>Date plan became effective.</summary>
		public DateTime DateEffective;
		///<summary>Date plan was terminated</summary>
		public DateTime DateTerm;
		///<summary>Release of information signature is on file.</summary>
		public bool ReleaseInfo;
		///<summary>Assignment of benefits signature is on file.  For Canada, this handles Payee Code, F01.  Option to pay other third party is not included.</summary>
		public bool AssignBen;
		///<summary>Usually SSN, but can also be changed by user.  No dashes. Not allowed to be blank.</summary>
		public string SubscriberID;
		///<summary>User doesn't usually put these in.  Only used when automatically requesting benefits, such as with Trojan.  All the benefits get stored here in text form for later reference.  Not at plan level because might be specific to subscriber.  If blank, we try to display a benefitNote for another subscriber to the plan.</summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BenefitNotes;
		///<summary>Use to store any other info that affects coverage.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string SubscNote;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;


		///<summary>Returns a copy of this InsSub.</summary>
		public InsSub Copy(){
			return (InsSub)this.MemberwiseClone();
		}

		

		

		
		



	}

	

	

	


}













