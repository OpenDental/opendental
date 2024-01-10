using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>A single action attached to a FlowDef. There are usually many.  Changing these does not alter any patient records. Only used in eClipboard for now.</summary>
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

		internal ERoutingActionDef Copy() {
			return (ERoutingActionDef)this.MemberwiseClone();
		}
	}

	public enum EnumERoutingActionType {
		[Description("No Action")]
		None,
		[Description("Perio Chart")]
		PerioChart,
		[Description("Treatment Plan")]
		TreatmentPlan,
		[Description("Payment Plan")]
		PaymentPlan,
		[Description("Chart Procedures")]
		ChartProcedures,
		[Description("Imaging")]
		Imaging,
		[Description("Complete Appointment")]
		CompleteAppointment,
		[Description("Take Payment")]
		TakePayment,
		[Description("Schedule Follow up")]
		ScheduleFollowup,
		[Description("ERX")]
		eRx,
		[Description("Exam Sheet")]
		ExamSheet,
		[Description("Consent Form")]
		ConsentForm,
		[Description("Medical")]
		Medical
	}
}
