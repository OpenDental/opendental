using System;

namespace OpenDentBusiness {
	///<summary>A single action attached to a eRouting. Only used in eClipboard for now.</summary>
	public class ERoutingAction:TableBase {
		/// <summary>Primary Key</summary>
		[CrudColumn(IsPriKey=true)]
		public long ERoutingActionNum;
		/// <summary>FK to eRouting.ERoutingNum</summary>
		public long ERoutingNum;
		/// <summary>Copied from eRoutingActionDef.ItemOrder.</summary>
		public int ItemOrder;
		/// <summary>Enum:EnumERoutingActionType </summary>
		public EnumERoutingActionType ERoutingActionType;
		/// <summary>FK to userod.UserNum. This is the user that completed the action. If not complete, this will be 0.</summary>
		public long UserNum;
		/// <summary>True if marked complete, otherwise set to false.</summary>
		public bool IsComplete;
		/// <summary>The date and time this action was set complete by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeComplete;
		/// <summary> FK to attached object. Type is indicated by ForeignKeyType. Sheet for Consent forms. </summary>
		public long ForeignKey;
		/// <summary> Enum:EnumERoutingFKType Indicates the type of object that ForeignKey references. None=0, Sheet=1 </summary>
		public EnumERoutingFKType ForeignKeyType;
	}

	/// <summary>Indicates the type of object that ForeignKey references. None=0, Sheet=1</summary>
	public enum EnumERoutingFKType {
		///<summary> 0 - Nothing is attached. </summary>
		None=0,
		///<summary> 1 - Sheet is attached. This blank sheet was created when the ERouting and its actions were copied from the ERoutingDef. This is done so that use can bring up that sheet again if connection is force closed.</summary>
		Sheet=1
	}
}
