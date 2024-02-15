using System;

namespace OpenDentBusiness {
	///<summary>A set of actions to take in sequence for each interaction with a specific patient.  Individual actions are in eRoutingAction. Templates are in eRoutingDef.  Only used in eClipboard for now.</summary>
	public class ERouting:TableBase {
		/// <summary>Primary Key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ERoutingNum;
		/// <summary>Copied from eRoutingDef. </summary>
		public string Description;
		/// <summary>FK to patient.PatNum. The patient this eRouting is for.</summary>
		public long PatNum;
		/// <summary>FK to clinic.ClinicNum. The clinic this patient eRouting is in. Set to 0 if in headquarters or clinics are disabled.</summary>
		public long ClinicNum;
		/// <summary>The DateTime this eRouting was created. eRoutings are created when they are started. Not able to edited by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		/// <summary>eRouting is considered complete if this is true. Used on backend to get incomplete eRouting without checking eRoutingActions</summary>
		public bool IsComplete;
	}
}
