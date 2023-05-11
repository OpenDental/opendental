using System;

namespace OpenDentBusiness {
	///<summary>A set of actions to take in sequence for each interaction with a specific patient.  Individual actions are in ERoutingAction. Templates are in ERoutingDef.  Only used in eClipboard for now.</summary>
	public class ERouting:TableBase {
		/// <summary>Primary Key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ERoutingNum;
		/// <summary>Copied from ERoutingDef. </summary>
		public string Description;
		/// <summary>FK to patient.PatNum. The patient this erouting is for.</summary>
		public long PatNum;
		/// <summary>FK to clinic.ClinicNum. The clinic this patient erouting is in. Set to 0 if in headquarters or clinics are disabled.</summary>
		public long ClinicNum;
		/// <summary>The DateTime this erouting was created. ERoutings are created when they are started. Not able to edited by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		/// <summary>ERouting is considered complete if this is true. Used on backend to get incomplete eroutings without checking ERoutingActions</summary>
		public bool IsComplete;
	}
}
