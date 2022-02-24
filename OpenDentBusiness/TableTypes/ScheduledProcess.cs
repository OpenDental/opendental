using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	[Serializable]
	public class ScheduledProcess:TableBase {
		///<summary>Primary Key</summary>
		[CrudColumn (IsPriKey=true)]
		public long ScheduledProcessNum;
		///<summary>Enum:ScheduledActionEnum </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public ScheduledActionEnum ScheduledAction;
		///<summary>What time of the day it's supposed to run.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime TimeToRun;
		///<summary>Enum:FrequencyToRunEnum </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public FrequencyToRunEnum FrequencyToRun;
		///<summary>Date and time when process last ran.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastRanDateTime;

		///<summary></summary>
		public ScheduledProcess Copy() {
			return (ScheduledProcess)this.MemberwiseClone();
		}
	}

	/// <summary>Action to be selected by user when scheduling. When adding a new value to this Enum add a case for it in the ScheduledProcessThread in
	/// OpendentalService and a method to handle the action.</summary>
	public enum ScheduledActionEnum {
		///<summary>0</summary>
		[Description("Recall Sync")]
		RecallSync,
		///<summary>1</summary>
		[Description("Ins Verify Batch")]
		InsVerifyBatch,
	}

	/// <summary>Frequency with which an action will be run. When adding a new value to this Enum add a case for it in the ScheduledProcessThread in 
	/// OpendentalService with the logic to check if the action should be run.</summary>
	public enum FrequencyToRunEnum {
		///<summary>0</summary>
		Daily,
	}
}
