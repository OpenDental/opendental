using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>When a task is created or a comment made, a series of these taskunread objects are created, one for each user who is subscribed to the tasklist.  Duplicates are intelligently avoided.  Rows are deleted once user reads the task.</summary>
	[Serializable()]
	[CrudTableAttribute(HasBatchWriteMethods=true)]
	public class TaskUnread:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TaskUnreadNum;
		///<summary>FK to task.TaskNum.</summary>
		public long TaskNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;

		///<summary></summary>
		public Account Clone() {
			return (Account)this.MemberwiseClone();
		}

	}
}




