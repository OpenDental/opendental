using System;
using System.Collections;
using System.Data;

namespace OpenDentBusiness{

	///<summary>A task is a single todo item.  Also see taskhist, which keeps a historical record.</summary>
	[Serializable]
	[CrudTable(AuditPerms=CrudAuditPerm.TaskNoteEdit)]
	public class Task:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TaskNum;
		///<summary>FK to tasklist.TaskListNum.  If 0, then it will show in the trunk of a section.  </summary>
		public long TaskListNum;
		///<summary>Only used if this task is assigned to a dated category.  Children are NOT dated.  Only dated if they should show in the trunk for a date category.  They can also have a parent if they are in the main list as well.</summary>
		public DateTime DateTask;
		///<summary>FK to patient.PatNum or appointment.AptNum. Only used when ObjectType is not 0.</summary>
		public long KeyNum;
		///<summary>The description of this task.  Might be very long.</summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Descript;
		///<summary>Enum:TaskStatusEnum New,Viewed,Done.  We may want to put an index on this column someday.</summary>
		public TaskStatusEnum TaskStatus;
		///<summary>True if it is to show in the repeating section.  There should be no date.  All children and parents should also be set to IsRepeating=true.</summary>
		public bool IsRepeating;
		///<summary>Enum:TaskDateType  None, Day, Week, Month.  If IsRepeating, then setting to None effectively disables the repeating feature.</summary>
		public TaskDateType DateType;
		///<summary>FK to task.TaskNum  If this is derived from a repeating task, then this will hold the TaskNum of that task.  It helps automate the adding and deleting of tasks.  It might be deleted automatically if not are marked complete.</summary>
		public long FromNum;
		///<summary>Enum:TaskObjectType  0=none,1=Patient,2=Appointment.  More will be added later. If a type is selected, then the KeyNum will contain the primary key of the corresponding Patient or Appointment.  Does not really have anything to do with the ObjectType of the parent tasklist, although they tend to match.</summary>
		public TaskObjectType ObjectType;
		///<summary>The date and time that this task was added.  User editable.
		///For reminder tasks, this field is used to indicate the date and time the reminder will take effect.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEntry;
		///<summary>FK to userod.UserNum.  The person who created the task.</summary>
		public long UserNum;
		///<summary>The date and time that this task was marked "done".</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeFinished;
		///<summary>Only used when tracking unread status by user instead of by task.  This gets set to true to indicate it has not yet been read.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsUnread;
		///<Summary>Not a database column.  A string description of the parent of this task.  It will only include the immediate parent.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ParentDesc;
		///<Summary>Not a database column.  Attached patient's name (NameLF) if there is an attached patient.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string PatientName;
		///<summary>FK to definition.DefNum.  The priority for this task which is used when filling task lists.  The placement of the task in the list is dependent on the item order of the definitions.</summary>
		public long PriorityDefNum;
		///<summary>Optional.  Set to null or empty if not a reminder task.
		///For repeating reminders, the ReminderGroupId will be the same for each task spawned from any task in the group.</summary>
		public string ReminderGroupId;
		///<summary>Bit field.</summary>
		public TaskReminderType ReminderType;
		///<summary></summary>
		public int ReminderFrequency;
		///<summary>The original datetime that the row was inserted.  Used to sort the list by the order entered.
		///Using taskhist.DateTimeOriginal will get the datetime that the task row was inserted, not the taskhist.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeOriginal;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;
		///<summary>Limited to 256 char. If present, shows only this text in task list grids instead of prepending date, aggregating notes, etc. Shows as Short Descript in the UI for space reasons.</summary>
		public string DescriptOverride;
		//NOTE: If adding any more columns, be sure to add them to TaskHist and to the constructor for TaskHist.

		///<summary></summary>
		public Task() {
			Descript="";
			ParentDesc="";//Might not be necessary.
			ReminderGroupId="";
		}

		///<summary></summary>
		public Task Copy() {
			return (Task)MemberwiseClone();
		}

		public override bool Equals(object obj) {
			if(TaskNum==((Task)obj).TaskNum
				&& TaskListNum==((Task)obj).TaskListNum
				&& DateTask==((Task)obj).DateTask
				&& KeyNum==((Task)obj).KeyNum
				&& Descript==((Task)obj).Descript
				&& TaskStatus==((Task)obj).TaskStatus
				&& IsRepeating==((Task)obj).IsRepeating
				&& DateType==((Task)obj).DateType
				&& FromNum==((Task)obj).FromNum
				&& ObjectType==((Task)obj).ObjectType
				&& DateTimeEntry==((Task)obj).DateTimeEntry
				&& UserNum==((Task)obj).UserNum
				&& DateTimeFinished==((Task)obj).DateTimeFinished
				&& PriorityDefNum==((Task)obj).PriorityDefNum
				&& ReminderGroupId==((Task)obj).ReminderGroupId
				&& ReminderType==((Task)obj).ReminderType
				&& ReminderFrequency==((Task)obj).ReminderFrequency
				&& DescriptOverride==((Task)obj).DescriptOverride)
			{
				return true;
			}
			return false;
			//return base.Equals(obj);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}

	///<summary>For use by the older Repeating Tasks feature.</summary>
	public enum TaskDateType{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		Day,
		///<summary>2</summary>
		Week,
		///<summary>3</summary>
		Month,
	}

	///<summary>Used when attaching objects to tasks.  These are the choices.</summary>
	public enum TaskObjectType{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		Patient,
		///<summary>2</summary>
		Appointment
	}

	///<summary></summary>
	[Flags]
	public enum TaskReminderType {
		///<summary>0</summary>
		NoReminder=0,
		///<summary>1</summary>
		Daily=1,
		///<summary>2</summary>
		Weekly=2,
		///<summary>4</summary>
		Monthly=4,
		///<summary>8</summary>
		Yearly=8,
		///<summary>16 - Use in combination with Weekly.</summary>
		Monday=16,
		///<summary>32 - Use in combination with Weekly.</summary>
		Tuesday=32,
		///<summary>64 - Use in combination with Weekly.</summary>
		Wednesday=64,
		///<summary>128 - Use in combination with Weekly.</summary>
		Thursday=128,
		///<summary>256 - Use in combination with Weekly.</summary>
		Friday=256,
		///<summary>512 - Use in combination with Weekly.</summary>
		Saturday=512,
		///<summary>1024 - Use in combination with Weekly.</summary>
		Sunday=1024,
		///<summary>2048 - Specific date for a reminder.</summary>
		Once=2048,
	}

	///<summary></summary>
	public enum TaskStatusEnum{
		///<summary>0</summary>
		New,
		///<summary>1</summary>
		Viewed,
		///<summary>2</summary>
		Done
	}

	public enum TaskType {
		///<summary>All task types.</summary>
		All=0,
		///<summary>Reminder tasks only.</summary>
		Reminder=1,
		///<summary>Regular tasks and repeating tasks.</summary>
		Normal=2,
	}

}




















