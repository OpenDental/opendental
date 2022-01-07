using System;
using System.Collections;

namespace OpenDentBusiness {

	///<summary>A jobnote is a note that may be added to a job. Many notes may be attached to a job.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	//[CrudTable(IsSynchable=true)]
	public class JobNote:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobNoteNum;
		///<summary>FK to job.JobNum. The job this jobnote is attached to.</summary>
		public long JobNum;
		///<summary>FK to userod.UserNum. The user who created this jobnote.</summary>
		public long UserNum;
		///<summary>Date and time the note was created (editable).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime DateTimeNote;
		///<summary>Note. Text that the user wishes to show on the task.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>The type of note.</summary>
		public JobNoteTypes NoteType;

		///<summary></summary>
		public JobNote Copy() {
			return (JobNote)MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum JobNoteTypes {
		///<summary>0 - These notes show up in the Discussion grid in the Discussion tab.</summary>
		Discussion,
		///<summary>1 - These notes show up in the Notes grid in the Testing tab.</summary>
		Testing,
	}







}













