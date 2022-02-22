using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	/////<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.
	///// Base object for use in the job tracking system.</summary>
	//[Serializable]
	//[CrudTable(IsMissingInGeneral=true)]
	//public class JobNotifyCust : TableBase {
	//	///<summary>Primary key.</summary>
	//	[CrudColumn(IsPriKey = true)]
	//	public long JobNotifyCustNum;
	//	///<summary>FK to job.JobNum. The Job that was completed and created this entry.</summary>
	//	public long JobNum;
	//	///<summary>FK to patient.PatNum. The patient to be contacted.</summary>
	//	public long PatNum;
	//	///<summary>FK to userod.UserNum. The user that contacted the customer.</summary>
	//	public long UserNumNotifier;
	//	///<summary>Human readable description of why the customer should be contacted. Also indicates if part of larger project.</summary>
	//	public string Description;
	//	///<summary>Should be the date the job was completed. This includes implementation and documentation.</summary>
	//	[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
	//	public DateTime SecDateTEntry;
	//	///<summary></summary>
	//	[CrudColumn(SpecialType = CrudSpecialColType.DateT)]
	//	public DateTime DateTimeContacted;

	//	public JobNotifyCust() {

	//	}

	//	///<summary></summary>
	//	public JobNotifyCust Copy() {
	//		return (JobNotifyCust)this.MemberwiseClone();
	//	}
	//}

}


