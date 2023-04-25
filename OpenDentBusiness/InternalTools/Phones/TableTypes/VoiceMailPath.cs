using System;

namespace OpenDentBusiness {
	///<summary>This class is not an official table type. Instead, this class represents a structured object that is stored within a JSON string inside a preference.
	///Departments at HQ have their own voice mail inboxes (unique physical directory where department specific voice mail recordings are saved).
	///This class links a directory path with a specific task list so that any tasks created from voice mails in said path can be automatically linked to the department.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class VoiceMailPath {
		///<summary>The name of the computer that is running the Phone Tracking Server (PTS) application.
		///A singular computer can have multiple VoiceMailPaths that it monitors (typically one per department / voice mail directory).</summary>
		public string ComputerName;
		///<summary>The directory that should be monitored for new voice mail recordings.</summary>
		public string Directory;
		///<summary>FK to tasklist.TaskListNum  The task list (department) that new tasks made from voice mail recordings in this directory will be linked to.</summary>
		public long TaskListNum;

	}
}
