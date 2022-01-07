using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary></summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true,IsLargeTable=true)]
	public class InsEditLog:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsEditLogNum;
		///<summary>Key to the foreign table.</summary>
		public long FKey;
		///<summary>Enum:InsEditLogType 0 - InsPlan, 1 - Carrier, 2 - Benefit, 3 - Employer.</summary>
		public InsEditLogType LogType;
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
		///<summary>Stores the key to the parent table (insplan.PlanNum) when LogType = 2 (Benefit).</summary>
		public long ParentKey;
		///<summary>The string describing this entry. Displays different information depending on the LogType:
		///0 - InsPlan: GroupNum and GroupName
		///1 - Carrier: CarrierNum and CarrierName
		///2 - Benefit: CovCat Description
		///3 - Employer: Employer Name</summary>
		public string Description;
	}


	public enum InsEditLogType {
		///<summary>0</summary>
		InsPlan,
		///<summary>1</summary>
		Carrier,
		///<summary>2</summary>
		Benefit,
		///<summary>3</summary>
		Employer,
	}
}









