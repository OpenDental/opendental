using System;
using System.Collections;
using System.Collections.Generic;
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
		///<summary>The time this log was uploaded.</summary>
		[System.Xml.Serialization.XmlIgnore]
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeUploaded;
		///<summary>Additional information for the log. This is intentionally limited to 255 characters to prevent bloat.
		///Add any new uses of this field to the list below. Provide the eServiceAction types and what the Note field represents for those types.
		///PPPaymentCreatedByXWeb:				The amount of the payment.
		///PPPaymentCreatedByPayconnect:	The amount of the payment.
		///PPOpenedHostedPaymentForm:			The name of the merchant service that the hosted payment form belongs to.</summary>
		public string Note;
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

	///<summary>Used by the API. Indicates how the appointment was originally made. Roughly corresponds to the eServiceType enumeration. </summary>
	public enum ApiEServiceLogType {
		///<summary>0. This means the appointment was not made through Web Sched.</summary>
		None,
		///<summary>1. This means the appointment was made via Web Sched Recall.</summary>
		Recall,
		///<summary>2. This means the appointment was made via Web Sched for a New Patient.</summary>
		NewPat,
		///<summary>3. This means the appointment was made via Web Sched for an Existing Patient.</summary>
		ExistingPat,
		///<summary>4. This means the appointment was made via Web Sched ASAP.</summary>
		ASAP,
	}

	///<summary>List of some possible eServices.</summary>
	public enum eServiceAction {
		///<summary>0 - Should not be in the database.</summary>
		[Description("Should Not Be Used")]
		[EServiceLogType(eServiceType.Unknown)]
		Undefined=0,
		///<summary>1 - Patient arrives at home view.</summary>
		[Description("Web Sched - Patient Arrived At Home View")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSHomeView,
		///<summary>2 - Patient chooses between new/existing/recall appointment on home view.</summary>
		[Description("Web Sched - Patient Selected Service")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSServiceSelect,
		///<summary>3 - Patient identifies themselves.</summary>
		[Description("Web Sched - Patient Identified Themselves")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSIdentify,
		///<summary>4 - Patient arrives at the scheduler page.</summary>
		[Description("Web Sched - Patient Arrived At Scheduler")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSScheduler,
		///<summary>5 - Patient switches months in the timeslot picker.</summary>
		[Description("Web Sched - Patient Switches Months")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSMonthSwitch,
		///<summary>6 - Patient selects an open timeslot.</summary>
		[Description("Web Sched - Patient Selected Timeslot")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSTimeSlotChoose,
		///<summary>7 - Patient recieves the confirmation popup.</summary>
		[Description("Web Sched - Patient Received Confirmation Popup")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSConfirmationPopup,
		///<summary>8 - Patient accepted the datetime.</summary>
		[Description("Web Sched - Patient Accepted Date/Time")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSDateTimeYes,
		///<summary>9 - Patient declines the datetime.</summary>
		[Description("Web Sched - Patient Declined Date/Time")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSDateTimeNo,
		///<summary>10 - Patient was sent a 2FA code.</summary>
		[Description("Web Sched - Patient Was Sent 2FA Code")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSTwoFactorSent,
		///<summary>11 - Patient successfully passed 2FA.</summary>
		[Description("Web Sched - Patient Passed 2FA")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSTwoFactorPassed,
		///<summary>12 - Patient schedules appointment.</summary>
		[Description("Web Sched - Patient Sent Schedule Request")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSAppointmentScheduleFromClient,
		///<summary>13 - Appointment scheduled.</summary>
		[Description("Web Sched - Appointment Scheduled")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSAppointmentScheduledFromServer,
		///<summary>14 - Appointment confirmation.</summary>
		[Description("Appt Confirmation - Appointment Confirmed")]
		[EServiceLogType(eServiceType.ApptConfirmations)]
		CONFConfirmedAppt,
		///<summary>15 - Appointment has been moved.</summary>
		[Description("Web Sched - Appointment Moved")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSMovedAppt,
		///<summary>16 - Patient logged into patient portal.</summary>
		[Description("Patient Portal - Patient Logged In")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPLoggedIn,
		///<summary>17 - Deprecated. Do not use.</summary>
		[Description("Patient Portal - Patient Made Payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPMadePayment,
		///<summary>18 - Form created.</summary>
		[Description("eClipboard - Form Created")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECAddedForm,
		///<summary>19 - Form was filled out.</summary>
		[Description("eClipboard - Form Completed")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCompletedForm,
		///<summary>20 - EClipboard Checked In.</summary>
		[Description("eClipboard - Checked In")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECLoggedIn,
		///<summary>21 - Web Forms Form Completed.</summary>
		[Description("Web Forms - Completed Web Form")]
		[EServiceLogType(eServiceType.WebForms)]
		WFCompletedForm,
		///<summary>22 - Web Sched Recall Not Found.</summary>
		[Description("Web Sched - Recall Not Found")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSRecallNotFound,
		///<summary>23 - Web Sched Already Scheduled.</summary>
		[Description("Web Sched - Recall Already Scheduled")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSRecallAlreadyScheduled,
		///<summary>24 - Arrivals, patient arrived.</summary>
		[Description("Arrival - Patient Arrived")]
		[EServiceLogType(eServiceType.Arrivals)]
		ArrivalReceived,
		///<summary>25 - Integrated Texting, Patient Opted Out.</summary>
		[Description("Integrated Texting - Patient Opted Out")]
		[EServiceLogType(eServiceType.IntegratedTexting)]
		IntegratedTextingOptOut,
		///<summary>26 - Patient Portal, Patient Arrived At Statement Portal.</summary>
		[Description("Patient Portal - Patient Arrived At Statement Portal")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPStatementPortalArrived,
		///<summary>27 - Patient Portal, Patient Logged In At Statement Portal.</summary>
		[Description("Patient Portal - Patient Logged In At Statement Portal")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPStatementPortalLoggedIn,
		///<summary>28 - Patient Portal, Patient Downloaded Statement.</summary>
		[Description("Patient Portal - Patient Downloaded Statement")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPStatementPortalDownloadStatement,
		///<summary>29 - Patient Portal, Patient Opened Payment Form.</summary>
		[Description("Patient Portal - Patient Opened Payment Form")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentFormOpened,
		///<summary>30 - Patient Portal, Patient Opened Payment Form From Login.</summary>
		[Description("Patient Portal - Patient Opened Payment Form From Login")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentFormOpenedFromLogin,
		///<summary>31 - Patient Portal, Patient Opened Hosted Payment Form.</summary>
		[Description("Patient Portal - Patient Opened Hosted Payment Form")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPOpenedHostedPaymentForm,
		///<summary>32 - Patient Portal, Patient Paid With Existing Card.</summary>
		[Description("Patient Portal - Patient Paid With Existing Card")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPayWithExistingFromClient,
		///<summary>33 - Patient Portal, Patient Submitted Payment With XWeb.</summary>
		[Description("Patient Portal - Patient Submitted Payment With XWeb")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentCreatedByXWeb,
		///<summary>34 - Patient Portal, Patient Submitted Payment With PayConnect.</summary>
		[Description("Patient Portal - Patient Submitted Payment With PayConnect")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentCreatedByPayconnect,
		///<summary>35 - Patient Portal, Patient Notified of Possible Duplicate Payment.</summary>
		[Description("Patient Portal - Patient Notified of Possible Duplicate Payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPDuplicatePaymentAlert,
		///<summary>36 - Patient Portal, Patient Allowed Submission of Duplicate Payment.</summary>
		[Description("Patient Portal - Patient Allowed Submission of Duplicate Payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPDuplicatePaymentAllowed,
		///<summary>37 - Patient Portal, Patient Rejected Submission of Duplicate Payment.</summary>
		[Description("Patient Portal - Patient Rejected Submission of Duplicate Payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPDuplicatePaymentDenied,
		/// <summary>38 - EClipboard - Check In for patients bringing their own device</summary>
		[Description("EClipboard - Check In Bring Your Own Device")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInBYOD,
		/// <summar>39 - EClipboard - Check In process started</summar>
		[Description("EClipboard - Check In process started")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInStarted,
		/// <summary>40 - EClipboard - Check In Arrived</summary>
		[Description("EClipboard - Check In Arrived")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInArrived,
		/// <summary>40 - EClipboard - Check In Submitted</summary>
		[Description("EClipboard - Check In Submitted")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInSubmitted,
		/// <summary>41 - EClipboard - Check In Error: Appt Not Found</summary>
		[Description("EClipboard - Error: Appt Not Found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorApptNotFound,
		/// <summary>42 - EClipboard - Check In Error: PatNum Not Linked To Appt</summary>
		[Description("EClipboard - Error: PatNum Not Linked To Appt")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorPatNumNotLinkedToAppt,
		/// <summary>43 - EClipboard - Error: Device Setup for Other Clinic</summary>
		[Description("EClipboard - Error: Device Setup for Other Clinic")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorDeviceSetupForOtherClinic,
		/// <summary>44 - EClipboard - Check In Error: Device Not Allowed for Checkin</summary>
		[Description("EClipboard - Error: Device Not Allowed for Checkin")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorDeviceNotAllowedForCheckin,
		/// <summary>45 - EClipboard - Check In Error: Office Device used as BYOD</summary>
		[Description("EClipboard - Error: Office Device used as BYOD")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorOfficeDeviceUsedAsBYOD,
		/// <summary>45 - EClipboard - Check In Error: No Appointment Found BYOD</summary>
		[Description("EClipboard - Error: No Appointment Found BYOD")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorNoApptFoundBYOD,
		/// <summary>48 - EClipboard - Check In Error: No Appt Found</summary>
		[Description("EClipboard - Error: No Appt Found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorNoApptFound,
		/// <summary>49 - EClipboard - Check In Error: Multiple Pats Found</summary>
		[Description("EClipboard - Error: Multiple Pats Found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorMultiplePatsFound,
		/// <summary>50 - EClipboard - Check In Error: Signature Error</summary>
		[Description("EClipboard - Error: Signature Error")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorSignatureError,
		/// <summary>51 - EClipboard - Check In Error: Deprecated Method</summary>
		[Description("EClipboard - Error: Deprecated Method")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorDeprecatedMethod,
		/// <summary>52 - EClipboard - Check In Confirmed Appt With Prov - Yes</summary>
		[Description("EClipboard - Check In Confirmed Appt With Prov - Yes")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInConfirmedApptWithProvYes,
		/// <summary>53 - EClipboard - Check In Confirmed Appt With Prov - No</summary>
		[Description("EClipboard - Check In Confirmed Appt With Prov - Yes")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInConfirmedApptWithProvNo,
		/// <summary>54 - EClipboard - Check In Took selfie before submitting</summary>
		[Description("EClipboard - Check In List Took selfie before submitting")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSubmittedWithPicture,
		/// <summary>55 - EClipboard - Check In List Did not take selfie before submitting</summary>
		[Description("EClipboard - Check In List did not take selfie before submitting")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSubmittedWithOutPicture,
		/// <summary>56 - EClipboard - Check In List Error, submitted without all items</summary>
		[Description("EClipboard - Check In List Error, submitted without all items")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListErrorSubmittedWithoutAllItems,
		/// <summary>57 - EClipboard - Check In List Submit Xamarin Error</summary>
		[Description("EClipboard - Check In List Submit Xamarin Error")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListXamSubmitError,
		/// <summary>59 - EClipboard - Check In Submit Success</summary>
		[Description("EClipboard - Check In List Submit Success")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSumbitSuccess,
		/// <summary>60 - EClipboard - Check In Submit Success BYOD</summary>
		[Description("EClipboard - Check In List Submit Success BYOD")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSumbitSuccessBYOD,
		/// <summary>60 - EClipboard - Check In List Selected Item</summary>
		[Description("EClipboard - Check In List Selected Item")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSelectedItem,
		/// <summary>61 - EClipboard - Check In List Sheet Next Tapped</summary>
		[Description("EClipboard - Check In List Sheet Next Tapped")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetNextTapped,
		/// <summary>61 - EClipboard - Check In List Sheet Prev Tapped</summary>
		[Description("EClipboard - Check In List Sheet Prev Tapped")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetPrevTapped,
		/// <summary>61 - EClipboard - Check In List Sheet Office signed Treatment Plan</summary>
		[Description("EClipboard - Check In List Office Signed Treatment Plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetOfficeSignedTreatPlan,
		/// <summary>61 - EClipboard - Check In List Sheet Patient signed treatment plan</summary>
		[Description("EClipboard - Check In List Patient Signed Treatment Plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetPatientSignedTreatPlan,
		/// <summary>61 - EClipboard - Check In List Sheet Patient signed Payment Plan</summary>
		[Description("EClipboard - Check In List Patient Signed Payment Plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetPatientSignedPaymentPlan,
		/// <summary>62 - EClipboard - Check In List Sheet Patient signed Payment Plan</summary>
		[Description("EClipboard - Check In List Patient Signed Payment Plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECPatientDirectedToMakePayment,
		/// <summary> - Used in Xam Exeptions. If this is sent back, error will not be logged. Default eServiceAction for XamException.</summary>
		[Description("Used in Xam Exeptions. If this is sent back, error will not be logged. Default eServiceAction for XamException.")]
		[EServiceLogType(eServiceType.Unknown)]
		DoNotLog

	}

	public class EServiceLogType:Attribute {
		public HashSet<eServiceType> eServiceTypes { get; set; }

		public EServiceLogType() { }

		public EServiceLogType(params eServiceType[] types) {
			eServiceTypes = new HashSet<eServiceType>();
			types.ForEach(x => eServiceTypes.Add(x));
		}
	}
}