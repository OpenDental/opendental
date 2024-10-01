using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>A single action attached to an ERoutingDef. Changing these does not alter any patient records. Only used in ODTouch for now.</summary>
	public class ERoutingActionDef:TableBase {
		///<summary>PK</summary>
		[CrudColumn(IsPriKey=true)]
		public long ERoutingActionDefNum;
		/// <summary>FK to eRouting.eRoutingDefNum. Defines what eRouting this action is tied to</summary>
		public long ERoutingDefNum;
		/// <summary>Enum:EnumeRoutingActionType </summary>
		public EnumERoutingActionType ERoutingActionType;
		/// <summary>Determines the order the items show in the eRoutingactiondef and what order they are to be completed in.</summary>
		public int ItemOrder;
		///<summary>The date this action definition was created. Not able to edited by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>The date time this action was last changed. Not able to be edited by the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTLastModified;
		/// <summary> FK to attached object. Type is indicated by ForeignKeyType. SheetDef for Consent forms. </summary>
		public long ForeignKey;
		/// <summary> Enum:EnumERoutingDefFKType Indicates the type of object that ForeignKey references. None=0, SheetDef=1 </summary>
		public EnumERoutingDefFKType ForeignKeyType;
		/// <summary> Override for the title of the eRouting Action. This will be shown in the eClipboard UI instead of EnumERoutingActionType description if it is present. </summary>
		public string LabelOverride;

		internal ERoutingActionDef Copy() {
			return (ERoutingActionDef)this.MemberwiseClone();
		}
	}


	public enum EnumERoutingActionType {
		///<summary>0-Shouldn't be present in db. Used in UI when user has not yet picked an action type.</summary>
		None,
		///<summary>1-Perio Chart</summary>
		[Description("Perio Chart")]
		PerioChart,
		///<summary>2-Treatment Plan</summary>
		[Description("Treatment Plan")]
		TreatmentPlan,
		///<summary>3-Payment Plan</summary>
		[Description("Payment Plan")]
		PaymentPlan,
		///<summary>4-Chart Procedures</summary>
		[Description("Chart Procedures")]
		ChartProcedures,
		///<summary>5-Imaging</summary>
		Imaging,
		///<summary>6-Complete Appointment</summary>
		[Description("Complete Appointment")]
		CompleteAppointment,
		///<summary>7-Take Payment</summary>
		[Description("Take Payment")]
		TakePayment,
		///<summary>8-Schedule Follow up</summary>
		[Description("Schedule Follow up")]
		ScheduleFollowup,
		///<summary>9-ERX</summary>
		[Description("ERX")]
		eRx,
		///<summary>10-Exam Sheet</summary>
		[Description("Exam Sheet")]
		ExamSheet,
		///<summary>11-Consent Form</summary>
		[Description("Consent Form")]
		ConsentForm,
		///<summary>12-Medical</summary>
		Medical,
		///<summary>13-Checklist Item</summary>
		[Description("Checklist Item")]
		ChecklistItem
	}

	/// <summary> Indicates the type of object that ForeignKey references. None=0, SheetDef=1 </summary>
	public enum EnumERoutingDefFKType {
		///<summary> 0 - Nothing is attached. </summary>
		None=0,
		///<summary> 1 - SheetDef is attached. </summary>
		SheetDef=1
	}

}
