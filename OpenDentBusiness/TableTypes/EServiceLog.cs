using System;
using System.Collections;
using System.ComponentModel;
using CodeBase;

namespace OpenDentBusiness {

	///<summary>Stores an ongoing record of EServices activity. User not allowed to edit.</summary>
	[Serializable]
	[CrudTable(IsLargeTable = true)]
	public class EServiceLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EServiceLogNum;
		///<summary>Enum:FKeyType</summary>
		public FKeyType KeyType;
		///<summary>Enum:eServiceType</summary>
		public eServiceType EServiceType;
		///<summary>Enum:eServiceAction</summary>
		public eServiceAction EServiceAction;
		///<summary>The date and time of the entry. It's value is set when inserting and can never change. Even if a user changes the date on their computer, this remains accurate because it uses server time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime LogDateTime;
		///<summary>FK to patient.PatNum. Can be 0 if not applicable.</summary>
		public long PatNum;
		///<summary>Clinic Number.</summary>
		public long ClinicNum;
		///<summary>Guid for logging actions with no associated PatNum.</summary>
		public string LogGuid;
		///<summary>FKey for given type.</summary>
		public long FKey;
	}

	///<summary>Type associated with FKey value.</summary>
	public enum FKeyType {
		///<summary>0 Undefined.</summary>
		Undefined = 0,
		///<summary>1 Appointment Number.</summary>
		ApptNum,
		///<summary>2 Payment Number.</summary>
		PayNum,
		///<summary>3 Sheet Number.</summary>
		SheetNum,
	}
	///<summary>List of some possible eService Actions.</summary>
	public enum eServiceType {
		///<summary>0. This means the action done was Unknown.</summary>
		[Description("Should not be used"),ShortDescription("All")]
		Unknown,
		///<summary>1. This means the action done was a Web Sched Recall.</summary>
		[Description("Web Sched Recall")]
		WSRecall,
		///<summary>2. This means the action done was for a New Patient.</summary>
		[Description("Web Sched New Patient")]
		WSNewPat,
		///<summary>3. This means the action done was for an Existing Patient.</summary>
		[Description("Web Sched Existing Patient")]
		WSExistingPat,
		///<summary>4. This means the action done was for a Web Sched ASAP.</summary>
		[Description("Web Sched ASAP")]
		WSAsap,
		///<summary>5. This means the action done was for a Patient Portal.</summary>
		[Description("Patient Portal")]
		PatientPortal,
		///<summary>6. This means the action done was for a Mobile Checkin.</summary>
		[Description("eClipboard")]
		EClipboard,
		///<summary>7. This means the action done was for a Appointment Confirmations.</summary>
		[Description("Appointment Confirmations")]
		ApptConfirmations,
		///<summary>8. This means the action done was for a WebForm.</summary>
		[Description("WebForms")]
		WebForms,
		///<summary>9. This means the action done was for unspecified WebSched.</summary>
		[Description("Web Sched General")]
		WSGeneral,
		///<summary>10. This means the action done was for Arrivals.</summary>
		[Description("Arrivals")]
		Arrivals,
		///<summary>11. This means the action done was for Integrated Texting.</summary>
		[Description("Integrated Texting")]
		IntegratedTexting,
	}

	///<summary>List of some possible eServices.</summary>
	public enum eServiceAction {
		///<summary>0 - Should not be in the database.</summary>
		[Description("Should Not Be Used")]
		Undefined=0,
		///<summary>1 - Patient arrives at home view.</summary>
		[Description("Web Sched - Patient Arrived At Home View")]
		WSHomeView,
		///<summary>2 - Patient chooses between new/existing/recall appointment on home view.</summary>
		[Description("Web Sched - Patient Selected Service")]
		WSServiceSelect,
		///<summary>3 - Patient identifies themselves.</summary>
		[Description("Web Sched - Patient Identified Themselves")]
		WSIdentify,
		///<summary>4 - Patient arrives at the scheduler page.</summary>
		[Description("Web Sched - Patient Arrived At Scheduler")]
		WSScheduler,
		///<summary>5 - Patient switches months in the timeslot picker.</summary>
		[Description("Web Sched - Patient Switches Months")]
		WSMonthSwitch,
		///<summary>6 - Patient selects an open timeslot.</summary>
		[Description("Web Sched - Patient Selected Timeslot")]
		WSTimeSlotChoose,
		///<summary>7 - Patient recieves the confirmation popup.</summary>
		[Description("Web Sched - Patient Received Confirmation Popup")]
		WSConfirmationPopup,
		///<summary>8 - Patient accepted the datetime.</summary>
		[Description("Web Sched - Patient Accepted Date/Time")]
		WSDateTimeYes,
		///<summary>9 - Patient declines the datetime.</summary>
		[Description("Web Sched - Patient Declined Date/Time")]
		WSDateTimeNo,
		///<summary>10 - Patient was sent a 2FA code.</summary>
		[Description("Web Sched - Patient Was Sent 2FA Code")]
		WSTwoFactorSent,
		///<summary>11 - Patient successfully passed 2FA.</summary>
		[Description("Web Sched - Patient Passed 2FA")]
		WSTwoFactorPassed,
		///<summary>12 - Patient schedules appointment.</summary>
		[Description("Web Sched - Patient Sent Schedule Request")]
		WSAppointmentScheduleFromClient,
		///<summary>13 - Appointment scheduled.</summary>
		[Description("Web Sched - Appointment Scheduled")]
		WSAppointmentScheduledFromServer,
		///<summary>14 - Appointment confirmation.</summary>
		[Description("Appt Confirmation - Appointment Confirmed")]
		CONFConfirmedAppt,
		///<summary>15 - Appointment has been moved.</summary>
		[Description("Web Sched - Appointment Moved")]
		WSMovedAppt,
		///<summary>16 - Patient logged into patient portal.</summary>
		[Description("Patient Portal - Patient Logged In")]
		PPLoggedIn,
		///<summary>17 - Payment was made in payment portal.</summary>
		[Description("Patient Portal - Patient Made Payment")]
		PPMadePayment,
		///<summary>18 - Form created.</summary>
		[Description("eClipboard - Form Created")]
		ECAddedForm,
		///<summary>19 - Form was filled out.</summary>
		[Description("eClipboard - Form Completed")]
		ECCompletedForm,
		///<summary>20 - EClipboard Checked In.</summary>
		[Description("eClipboard - Checked In")]
		ECLoggedIn,
		///<summary>21 - Web Forms Form Completed.</summary>
		[Description("Web Forms - Completed Web Form")]
		WFCompletedForm,
		///<summary>22 - Web Sched Recall Not Found.</summary>
		[Description("Web Sched - Recall Not Found")]
		WSRecallNotFound,
		///<summary>23 - Web Sched Already Scheduled.</summary>
		[Description("Web Sched - Recall Already Scheduled")]
		WSRecallAlreadyScheduled,
		///<summary>24 - Arrivals, patient arrived.</summary>
		[Description("Arrival - Patient Arrived")]
		ArrivalReceived,
		///<summary>25 - Integrated Texting, Patient Opted Out.</summary>
		[Description("Integrated Texting - Patient Opted Out")]
		IntegratedTextingOptOut,
	}
}