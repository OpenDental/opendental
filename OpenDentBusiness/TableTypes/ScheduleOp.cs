 using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Links one schedule block to one operatory.  A schedule block can be linked to one or more operatories.  A schedule can also not have any scheduleops.  For example the provider schedule.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true,IsLargeTable=true)]
	public class ScheduleOp:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ScheduleOpNum;
		///<summary>FK to schedule.ScheduleNum.</summary>
		public long ScheduleNum;
		///<summary>FK to operatory.OperatoryNum.</summary>
		public long OperatoryNum;

		public ScheduleOp Copy(){
			return (ScheduleOp)this.MemberwiseClone();
		}

	
		
	}

	

	

}













