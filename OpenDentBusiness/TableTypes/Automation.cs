using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{
	
	///<summary>A trigger event causes one or more actions.</summary>
	[Serializable()]
	public class Automation:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutomationNum;
		///<summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Description;
		///<summary>Enum:AutomationTrigger What triggers this automation</summary>
		public AutomationTrigger Autotrigger;
		///<summary>If this has a CompleteProcedure trigger, this is a comma-delimited list of codes that will trigger the action.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProcCodes;
		///<summary>Enum:AutomationAction The action taken as a result of the trigger.  To get more than one action, create multiple automation entries.</summary>
		public AutomationAction AutoAction;
		///<summary>FK to sheetdef.SheetDefNum.  If the action is to print a sheet, then this tells which sheet to print.  So it must be a custom sheet.  Also, not that this organization does not allow passing parameters to the sheet such as which procedures were completed, or which appt was broken.</summary>
		public long SheetDefNum;
		///<summary>FK to definition.DefNum. Only used if action is CreateCommlog.</summary>
		public long CommType;
		///<summary>If a commlog action, then this is the text that goes in the commlog.  If this is a ShowStatementNoteBold action, then this is the NoteBold. Might later be expanded to work with email or to use variables.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MessageContent;
		///<summary>Enum:ApptStatus . This column is not used anymore.</summary>
		public ApptStatus AptStatus;
		///<summary>FK to appointmenttype.AppointmentTypeNum.</summary>
		public long AppointmentTypeNum;
		///<summary>Enum:PatientStatus - used to determine which status to change to for ChangePatientStatus automation actions. Should never be 'Deleted'</summary>
		public PatientStatus PatStatus;
		

		public Automation Copy() {
			return (Automation)MemberwiseClone();
		}

	}



	///<summary></summary>
	public enum AutomationTrigger {
		///<summary></summary>
		CompleteProcedure,
		///<summary></summary>
		BreakAppointment,
		///<summary></summary>
		CreateApptNewPat,
		///<summary>Regardless of module.  Usually only used with conditions.</summary>
		OpenPatient,
		///<summary></summary>
		CreateAppt,
		///<summary>Attaching a procedure to a scheduled appointment.</summary>
		ScheduleProcedure,
		///<summary></summary>
		SetBillingType,
		//<summary>Either a single statement or as part of the billing process.  Either print or </summary>
		//CreateStatement
		///<summary>Creating a new Rx</summary>
		RxCreate,
	}

	///<summary></summary>
	public enum AutomationAction {
		///<summary></summary>
		PrintPatientLetter,
		///<summary></summary>
		CreateCommlog,
		///<summary>If a referral does not exist for this patient, then notify user instead.</summary>
		PrintReferralLetter,
		///<summary></summary>
		ShowExamSheet,
		///<summary></summary>
		PopUp,
		///<summary></summary>
		SetApptASAP,
		///<summary></summary>
		ShowConsentForm,
		///<summary></summary>
		SetApptType,
		///<summary>Similar to PopUp, but will only show once per WS per 10 minutes.</summary>
		PopUpThenDisable10Min,
		///<summary>When triggered, automatically restricts patient from being scheduled. See also PatRestriction.cs</summary>
		PatRestrictApptSchedTrue,
		///<summary>When triggered, automatically removes patient from scheduling restriction. See also PatRestriction.cs</summary>
		PatRestrictApptSchedFalse,
		///<summary>When triggered, it will automatically print a copy of the Patient Rx Instructions</summary>
		PrintRxInstruction,
		///<summary>When triggered, automatically set a patient's status to the status type in the PatStatus column. Delete should never be used.</summary>
		[Description("Change Pat Status")]
		ChangePatStatus,
	}
	


}









