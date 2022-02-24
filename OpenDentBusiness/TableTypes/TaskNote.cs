using System;
using System.Collections;

namespace OpenDentBusiness {

	///<summary>A tasknote is a note that may be added to a task. Many notes may be attached to a task. A user may only edit their own tasknotes within a task.</summary>
	[Serializable]
	public class TaskNote:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TaskNoteNum;
		///<summary>FK to task.TaskNum. The task this tasknote is attached to.</summary>
		public long TaskNum;
		///<summary>FK to userod.UserNum. The user who created this tasknote.</summary>
		public long UserNum;
		///<summary>Date and time the note was created or last modified (editable).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime DateTimeNote;
		///<summary>Note. Text that the user wishes to show on the task.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;

		///<summary></summary>
		public TaskNote Copy() {
			return (TaskNote)MemberwiseClone();
		}



	}






}













