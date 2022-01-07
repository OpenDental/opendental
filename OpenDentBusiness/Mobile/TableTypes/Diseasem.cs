using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {

		///<summary>Links medications to patients. Patient portal version</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class Diseasem:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long DiseaseNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to diseasedef.DiseaseDefNum.  The disease description is in that table.  Will be zero if ICD9Num has a value.</summary>
		public long DiseaseDefNum;
		///<summary>Any note about this disease that is specific to this patient.</summary>
		public string PatNote;
		///<summary>FK to icd9.ICD9Num.  Will be zero if DiseaseDefNum has a value.</summary>
		public long ICD9Num;
		///<summary>Enum: ProblemStatus: Active=0, Resolved=1, Inactive=2.</summary>
		public ProblemStatus ProbStatus;
		///<summary>Date that the disease was diagnosed.  Can be minval if unknown.</summary>
		public DateTime DateStart;
		///<summary>Date that the disease was set resolved or inactive.  Will be minval if still active.  ProbStatus should be used to determine if it is active or not.</summary>
		public DateTime DateStop;

		///<summary></summary>
		public Diseasem Copy() {
			return (Diseasem)this.MemberwiseClone();
		}

	}
}
