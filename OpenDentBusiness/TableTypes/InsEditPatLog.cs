using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {

	///<summary></summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true,IsLargeTable=true)]
	public class InsEditPatLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsEditPatLogNum;
		///<summary>Foreign key to the field flagged with the PriKey attributed for the corresponding table type which is specified by LogType. Note, some logs do not use table type objects that are directly related to the LogType. E.g. Adjustment LogType uses a claimproc entity. 0 - PatPlan: patplan.PatPlanNum. 1 - Subscriber: inssub.InsSubNum. 2 - Adjustment: claimproc.ClaimProcNum.</summary>
		public long FKey;
		///<summary>Enum:InsEditPatLogType 0 - PatPlan, 1 - Subscriber, 2 - Adjustment.</summary>
		public InsEditPatLogType LogType;
		///<summary>The name of the column that was altered.</summary>
		public string FieldName;
		///<summary>The old value of this field.</summary>
		public string OldValue;
		///<summary>The new value of this field.</summary>
		public string NewValue;
		///<summary>FK to userod.UserNum. The user that made this change.</summary>
		public long UserNum;
		///<summary>Time that the row was inserted into the DB.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Used to store another foreign key link to another entity based off of the current LogType. 0 - PatPlan: Not used. 1 - Subscriber: Not used. 2 - Adjustment: claimproc.InsSubNum</summary>
		public long ParentKey;
		///<summary>The string describing this entry. Displays different information depending on the LogType: 1 - Subscriber: Subscriber's Name, 2 - Adjustment: Insurance Benefit</summary>
		public string Description;
	}

	public enum InsEditPatLogType {
		///<summary>0</summary>
		PatPlan,
		///<summary>1</summary>
		Subscriber,
		///<summary>2 - Adjustments to insurance benefits.</summary>
		Adjustment,
	}
}
