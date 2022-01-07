using System;
using System.Collections;

namespace OpenDentBusiness {

	///<summary>Mostly used for EHR.  This rigorously records encounters using rich automation, so that reporting can be easy and meaningful.  Encounters can also be tracked separately using billable procedures.  In contrast, encounters in this table are not billable.  There can be multiple encounters at one appointment because there can be different types.</summary>
	[Serializable]
	public class Encounter:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EncounterNum;
		///<summary>FK to patient.PatNum. </summary>
		public long PatNum;
		///<summary>FK to provider.ProvNum. </summary>
		public long ProvNum;
		///<summary>FK to ehrcode.CodeValue.  This code may not exist in the ehrcode table, it may have been chosen from a bigger list of available codes.  In that case, this will be a FK to a specific code system table identified by the CodeSystem column.  The code for this item from one of the code systems supported.  Examples: 185349003 or 406547006.</summary>
		public string CodeValue;
		///<summary>FK to codesystem.CodeSystemName. This will determine which specific code system table the CodeValue is a FK to.  We only allow the following CodeSystems in this table: CDT, CPT, HCPCS, and SNOMEDCT. </summary>
		public string CodeSystem;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>Date the encounter occurred</summary>
		public DateTime DateEncounter;

		///<summary>Returns a copy of this Encounter.</summary>
		public Encounter Clone() {
			return (Encounter)this.MemberwiseClone();
		}

	}



}