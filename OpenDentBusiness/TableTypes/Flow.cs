using System;

namespace OpenDentBusiness {
	///<summary>A set of actions to take in sequence for each interaction with a specific patient.  Individual actions are in FlowAction. Templates are in FlowDef.  Only used in eClipboard for now.</summary>
	public class Flow:TableBase {
		/// <summary>Primary Key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FlowNum;
		/// <summary>Copied from FlowDef. </summary>
		public string Description;
		/// <summary>FK to patient.PatNum. The patient this flow is for.</summary>
		public long PatNum;
		/// <summary>FK to clinic.ClinicNum. The clinic this patient flow is in. Set to 0 if in headquarters or clinics are disabled.</summary>
		public long ClinicNum;
		/// <summary>The DateTime this flow was created. Flows are created when they are started. Not able to edited by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		/// <summary>Flow is considered complete if this is true. Used on backend to get incomplete flows without checking FlowActions</summary>
		public bool IsComplete;
	}
}
