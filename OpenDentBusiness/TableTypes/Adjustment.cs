using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>An adjustment in the patient account.  Usually, adjustments are very simple, just being assigned to one patient and provider.  But they can also be attached to a procedure to represent a discount on that procedure.  Attaching adjustments to procedures is not automated, so it is not very common.</summary>
	[Serializable()]
	[CrudTable(IsSecurityStamped=true,HasBatchWriteMethods=true)]
	public class Adjustment:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AdjNum;
		///<summary>The date that the adjustment shows in the patient account.</summary>
		public DateTime AdjDate;
		///<summary>Amount of adjustment.  Can be pos or neg.</summary>
		public double AdjAmt;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to definition.DefNum.</summary>
		public long AdjType;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>Note for this adjustment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string AdjNote;
		///<summary>Procedure date.  Not when the adjustment was entered.</summary>
		public DateTime ProcDate;
		///<summary>FK to procedurelog.ProcNum.  Only used if attached to a procedure.  Otherwise, 0.</summary>
		public long ProcNum;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime DateEntry;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>FK to statement.StatementNum.  Only used when the statement in an invoice.</summary>
		public long StatementNum;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		//No SecDateEntry, DateEntry already exists and is set by MySQL when the row is inserted and never updated
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>(Deprecated) Holds the Avalara transaction ID associated with this adjustment so that we can track reported adjustments.
		///Not editable in the UI.</summary>
		[Obsolete("This column has been deprecated. Do not use.",false)]
		public long TaxTransID;

		///<summary></summary>
		public Adjustment Clone() {
			return (Adjustment)this.MemberwiseClone();
		}

		
	}

	


	


}









