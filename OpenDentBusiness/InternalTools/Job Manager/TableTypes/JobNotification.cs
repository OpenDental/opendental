using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>A notification table for changes made to a job. Notifies subscribed users a change has been made.</summary>
	[Serializable()]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	public class JobNotification:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobNotificationNum;
		///<summary>FK to job.JobNum.</summary>
		public long JobNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Enum:JobNotificationChanges Identifies what changes were made to this job.</summary>
		public JobNotificationChanges Changes;

		public JobNotification() {
			
		}

		///<summary></summary>
		public JobNotification Copy() {
			return (JobNotification)this.MemberwiseClone();
		}

		public override bool Equals(object obj) {
			JobNotification notif=obj as JobNotification;
			if(notif==null) {
				return false;
			}
			return this.JobNotificationNum==notif.JobNotificationNum
				&& this.JobNum==notif.JobNum
				&& this.UserNum==notif.UserNum
				&& this.Changes==notif.Changes;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}

	///<summary>The possible flags shown for changes on a job.</summary>
	[Flags]
	public enum JobNotificationChanges {
		///<summary></summary>
		None=0,
		///<summary></summary>
		ConceptChange=1,
		///<summary></summary>
		WriteupChange=2,
		///<summary></summary>
		NoteAdded=4,
		///<summary></summary>
		PhaseChange=8,
		///<summary></summary>
		CategoryChange=16,
		///<summary></summary>
		ApprovalChange=32,
		///<summary></summary>
		ExpertChange=64,
		///<summary></summary>
		EngineerChange=128,
		///<summary></summary>
		PriorityChange=256,
	}

}
