using System;

namespace OpenDentBusiness {
	///<summary>Communication item from workstation to OD HQ. Stores the statuses of the eConnector and other eServices. </summary>
	[Serializable]
	public class EServiceSignal:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EServiceSignalNum;
		///<summary>Service which this signal applies to. Defined by eServiceCodes.</summary>
		public int ServiceCode;
		///<summary>Category defined by ReasonCodeCategories. Can be zero if no grouping is necessary per a given service. Stored as an int for forward compatibility.</summary>
		public int ReasonCategory;
		///<summary>The reason for the eServiceSignal. This code is used to determine what actions to take and how to process this message. 
		///It is a function of ReasonCategory. It will most likely be defined by an enum that lives on HQ-only closed source.</summary>
		public int ReasonCode;
		///<summary>Enum:eServiceSignalSeverity </summary>
		public eServiceSignalSeverity Severity;
		///<summary>Human readable description of what this signal means, or a message for the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Description;
		///<summary>Time signal was sent.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime SigDateTime;
		///<summary>Used to store serialized data that can be used for processing this signal.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Tag;
		///<summary>After a message has been processed or acknowledged this is set true. Not currently used for heartbeat or service status signals.</summary>
		public bool IsProcessed;

		///<summary></summary>
		public EServiceSignal Copy() {
			return (EServiceSignal)this.MemberwiseClone();
		}
	}
	
	///<summary>Used to determine that status of the entire service.  Order of enum is important, from lowest to highest importance.</summary>
	public enum eServiceSignalSeverity {
		///<summary>Service is not in use and is not supposed to be in use.</summary>
		None=-1,
		///<summary>Service is not in use and is not supposed to be in use.</summary>
		NotEnabled,
		///<summary>Used to convey information. Does not change the "working" status of the service. Will always be inserted with IsProcess=true.</summary>
		Info,
		///<summary>Service is operational and working as designed. Typcially used for heartbeat and initialization.</summary>
		Working,
		///<summary>Recoverable error has has occurred and no user intervention is required. Typically requires user acknowledgement only.</summary>
		Warning,
		///<summary>Recoverable error has has occurred and user intervention is probably required in addition to user acknowledgement only.</summary>
		Error,
		///<summary>Unrecoverable error and the service has shut itself off. Immediate user intervention is required.</summary>
		Critical
	}

}