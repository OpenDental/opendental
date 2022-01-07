using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Represents one ancestor of one task.  Each task will have at least one ancestor unless it is directly on a main trunk.  
	///An ancestor is defined as a tasklist that is higher in the heirarchy for the task, regardless of how many levels up it is.  
	///This allows us to mark task lists as having "new" tasks, and it allows us to quickly check for new tasks for a user on startup.</summary>
	[Serializable()]
	[CrudTableAttribute(HasBatchWriteMethods=true)]
	public class TaskAncestor : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TaskAncestorNum;
		/// <summary>FK to task.TaskNum</summary>
		public long TaskNum;
		/// <summary>FK to tasklist.TaskListNum</summary>
		public long TaskListNum;

		

		
		

			
	}

	

}









