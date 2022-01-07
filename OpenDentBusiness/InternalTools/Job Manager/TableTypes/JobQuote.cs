using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	//[CrudTable(IsSynchable=true)]
	public class JobQuote:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobQuoteNum;
		///<summary>FK to job.JobNum.</summary>
		public long JobNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary></summary>
		public string Amount;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary></summary>
		public string Hours;
		///<summary></summary>
		public string ApprovedAmount;
		///<summary></summary>
		public bool IsCustomerApproved;

		///<summary></summary>
		public JobQuote Copy() {
			return (JobQuote)this.MemberwiseClone();
		}
	}
}
