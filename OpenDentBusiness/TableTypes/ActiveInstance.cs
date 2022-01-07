using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>ActiveInstances are used to track OD sessions.</summary>
	[Serializable]
	public class ActiveInstance:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long ActiveInstanceNum;
		///<summary>FK to Computers.ComputerNum</summary>
		public long ComputerNum;
		///<summary>FK to Userod.UserNum</summary>
		public long UserNum;
		///<summary>Windows Process ID of the Open Dental instance</summary>
		public long ProcessId;
		///<summary>Last datetime that was activity was recorded</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeLastActive;
		///<summary>The time at which we recorded DateTimeLastActive. This is not a TimeStamp column because we need to update it even if nothing else in the row changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTRecorded;
		///<summary>Enum:ConnectionTypes Used to distinguish the connection type.</summary>
		public ConnectionTypes ConnectionType;
	}

	public enum ConnectionTypes {
		///<summary>0 - Direct</summary>
		Direct,
		///<summary>1 - MiddleTier</summary>
		MiddleTier,
		///<summary>2 - ODCloud</summary>
		ODCloud,
	}
}