using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	//[CrudTable(IsSynchable=true)]
	public class JobActiveLink:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobActiveLinkNum;
		///<summary>FK to job.JobNum.</summary>
		public long JobNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>The date/time that the jobactivelink was created.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>The date/time that the jobactivelink was ended.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEnd;

		///<summary></summary>
		public JobActiveLink Copy() {
			return (JobActiveLink)this.MemberwiseClone();
		}
	}

}


