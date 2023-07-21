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
	}
}
