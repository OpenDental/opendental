using System;

namespace OpenDentBusiness {
	///<summary>A single action attached to a Flow. Only used in eClipboard for now.</summary>
	public class FlowAction:TableBase {
		/// <summary>Primary Key</summary>
		[CrudColumn(IsPriKey=true)]
		public long FlowActionNum;
		/// <summary>FK to flow.FlowNum</summary>
		public long FlowNum;
		/// <summary>Copied from flowActionDef.ItemOrder.</summary>
		public int ItemOrder;
		/// <summary>Enum:EnumFlowActionType </summary>
		public EnumFlowActionType FlowActionType;
		/// <summary>FK to userod.UserNum. This is the user that completed the action. If not complete, this will be 0.</summary>
		public long UserNum;
		/// <summary>True if marked complete, otherwise set to false.</summary>
		public bool IsComplete;
		/// <summary>The date and time this action was set complete by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeComplete;
	}
}
