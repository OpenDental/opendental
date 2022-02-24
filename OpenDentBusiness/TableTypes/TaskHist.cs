using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>A historical copy of a task.  These are generated as a result of a task being edited, so there can be multiple entries here per task.  When creating for insertion it needs a passed-in Task object.</summary>
	[Serializable]
	public class TaskHist:TableBase {
		#region Not copied from Task
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TaskHistNum;
		///<summary>FK to userod.UserNum  Identifies the user that changed this task from this state, not the person who originally wrote it.</summary>
		public long UserNumHist;
		///<summary>The date and time that this task was edited and added to the Hist table. This value will not be updated by MySQL whenever the row changes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTStamp;
		///<summary>True if the note was changed when this historical copy was created.</summary>
		public bool IsNoteChange;
		#endregion Not copied from Task

		#region Copies of Task Fields
		///<summary>Copied from Task.</summary>
		public long TaskNum;
		///<summary>Copied from Task.</summary>
		public long TaskListNum;
		///<summary>Copied from Task.</summary>
		public DateTime DateTask;
		///<summary>Copied from Task.</summary>
		public long KeyNum;
		///<summary>Copied from Task.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Descript;
		///<summary>Copied from Task.</summary>
		public TaskStatusEnum TaskStatus;
		///<summary>Copied from Task.</summary>
		public bool IsRepeating;
		///<summary>Copied from Task.</summary>
		public TaskDateType DateType;
		///<summary>Copied from Task.</summary>
		public long FromNum;
		///<summary>Copied from Task.</summary>
		public TaskObjectType ObjectType;
		///<summary>Copied from Task.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEntry;
		///<summary>Copied from Task.</summary>
		public long UserNum;
		///<summary>Copied from Task.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeFinished;
		///<summary>Copied from Task.</summary>
		public long PriorityDefNum;
		///<summary>Copied from Task.</summary>
		public string ReminderGroupId;
		///<summary>Copied from Task.</summary>
		public TaskReminderType ReminderType;
		///<summary>Copied from Task.</summary>
		public int ReminderFrequency;
		///<summary>Copied from Task.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeOriginal;
		///<summary>Not copied from Task. Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>Copied from Task.</summary>
		public string DescriptOverride;
		#endregion Copies of Task Fields
		
		#region Not Db Columns
		///<summary>Only used when tracking unread status by user instead of by task.  This gets set to true to indicate it has not yet been read.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsUnread;
		///<Summary>Not a database column.  A string description of the parent of this task.  It will only include the immediate parent.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ParentDesc;
		///<Summary>Not a database column.  Attached patient's name (NameLF) if there is an attached patient.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string PatientName;
		#endregion Not Db Columns

		///<summary>Pass in the old task that needs to be recorded.</summary>
		public TaskHist(Task task) {
			TaskNum=task.TaskNum;
			TaskListNum=task.TaskListNum;
			DateTask=task.DateTask;
			KeyNum=task.KeyNum;
			Descript=task.Descript;
			TaskStatus=task.TaskStatus;
			IsRepeating=task.IsRepeating;
			DateType=task.DateType;
			FromNum=task.FromNum;
			ObjectType=task.ObjectType;
			DateTimeEntry=task.DateTimeEntry;
			UserNum=task.UserNum;
			DateTimeFinished=task.DateTimeFinished;
			PriorityDefNum=task.PriorityDefNum;
			ReminderGroupId=task.ReminderGroupId;
			ReminderType=task.ReminderType;
			ReminderFrequency=task.ReminderFrequency;
			DateTimeOriginal=task.DateTimeOriginal;
			//SecDateT can only be set by MySQL
			DescriptOverride=task.DescriptOverride;
			#region Not Db Columns
			IsUnread=task.IsUnread;
			ParentDesc=task.ParentDesc;
			PatientName=task.PatientName;
			#endregion Not Db Columns
		}

		public TaskHist() {
		}

		///<summary>Overrides Task.Copy() which is desired behavior because TaskHist extends Task.</summary>
		public new TaskHist Copy() {
			return (TaskHist)MemberwiseClone();
		}
	}
}
