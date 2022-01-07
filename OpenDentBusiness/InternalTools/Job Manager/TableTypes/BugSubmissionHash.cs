using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	/// <summary></summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true,HasBatchWriteMethods=true)]
	public class BugSubmissionHash:TableBase {
		/// <summary>PK.</summary>
		[CrudColumn(IsPriKey=true)]
		public long BugSubmissionHashNum;
		/// <summary>Used as composite index with PartialHash. Limits to 50 characters.</summary>
		public string FullHash;
		/// <summary>Used as composite index with FullHash. Limits to 50 characters.</summary>
		public string PartialHash;
		/// <summary>FK to bug.bugId when corresponding bugsubmission is attached to a bug.</summary>
		public long BugId;
		/// <summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTimeModify;
		/// <summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
	}
}
