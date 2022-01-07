using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	//NOTE: There was a bug before version 14.3 that would cause duplicate entries for patients if they were merged.  So it is possible that there is more than one entry for each patient.
	//Since this is only for internal use, we fixed the issue manually at HQ.  To fix correctly for resellers, a DBM will have to be written.

	///<summary>One to one relation with the patient table representing each customer as a reference.</summary>
	[Serializable]
	public class CustReference:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CustReferenceNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Most recent date the reference was used, loosely kept updated.</summary>
		public DateTime DateMostRecent;
		///<summary>Notes specific to this customer as a reference.</summary>
		public string Note;
		///<summary>Set to true if this customer was a bad reference.</summary>
		public bool IsBadRef;

		///<summary></summary>
		public CustReference Clone() {
			return (CustReference)this.MemberwiseClone();
		}

	}

}