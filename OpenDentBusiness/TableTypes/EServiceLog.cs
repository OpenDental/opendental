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
		///<summary>4 Utm Number.</summary>
		UtmNum,
		///<summary>5 Web Form Sheet Number.</summary>
		WebFormSheetID,
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
		///<summary>12. This means the action done was for the mobile app ODTouch.</summary>
		[Description("ODTouch")]
		ODTouch,
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
		[Description("Should not be used")]
		[EServiceLogType(eServiceType.Unknown)]
		Undefined=0,
		///<summary>1 - Patient arrives at home view.</summary>
		[Description("Web Sched - Patient arrived at home view")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSHomeView,
		///<summary>2 - Patient chooses between new/existing/recall appointment on home view.</summary>
		[Description("Web Sched - Patient selected service")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSServiceSelect,
		///<summary>3 - Patient identifies themselves.</summary>
		[Description("Web Sched - Patient identified themselves")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSIdentify,
		///<summary>4 - Patient arrives at the scheduler page.</summary>
		[Description("Web Sched - Patient arrived at scheduler")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSScheduler,
		///<summary>5 - Patient switches months in the timeslot picker.</summary>
		[Description("Web Sched - Patient switches months")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSMonthSwitch,
		///<summary>6 - Patient selects an open timeslot.</summary>
		[Description("Web Sched - Patient selected timeslot")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSTimeSlotChoose,
		///<summary>7 - Patient recieves the confirmation popup.</summary>
		[Description("Web Sched - Patient received confirmation popup")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSConfirmationPopup,
		///<summary>8 - Patient accepted the datetime.</summary>
		[Description("Web Sched - Patient accepted date/time")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSDateTimeYes,
		///<summary>9 - Patient declines the datetime.</summary>
		[Description("Web Sched - Patient declined date/time")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSDateTimeNo,
		///<summary>10 - Patient was sent a 2FA code.</summary>
		[Description("Web Sched - Patient was sent 2FA code")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSTwoFactorSent,
		///<summary>11 - Patient successfully passed 2FA.</summary>
		[Description("Web Sched - Patient passed 2FA")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSTwoFactorPassed,
		///<summary>12 - Patient schedules appointment.</summary>
		[Description("Web Sched - Patient sent schedule request")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSAppointmentScheduleFromClient,
		///<summary>13 - Appointment scheduled.</summary>
		[Description("Web Sched - Appointment scheduled")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSAppointmentScheduledFromServer,
		///<summary>14 - Appointment confirmation.</summary>
		[Description("Appt Confirmation - Appointment confirmed")]
		[EServiceLogType(eServiceType.ApptConfirmations)]
		CONFConfirmedAppt,
		///<summary>15 - Appointment has been moved.</summary>
		[Description("Web Sched - Appointment moved")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSMovedAppt,
		///<summary>16 - Patient logged into patient portal.</summary>
		[Description("Patient Portal - Patient logged in")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPLoggedIn,
		///<summary>17 - Deprecated. Do not use.</summary>
		[Description("Patient Portal - Patient made payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPMadePayment,
		///<summary>18 - Form created.</summary>
		[Description("eClipboard - Form created")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECAddedForm,
		///<summary>19 - Form was filled out.</summary>
		[Description("eClipboard - Form completed")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCompletedForm,
		///<summary>20 - eClipboard Checked In.</summary>
		[Description("eClipboard - Checked in")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECLoggedIn,
		///<summary>21 - Web Forms Form Completed.</summary>
		[Description("Web Forms - Completed web form")]
		[EServiceLogType(eServiceType.WebForms)]
		WFCompletedForm,
		///<summary>22 - Web Sched Recall Not Found.</summary>
		[Description("Web Sched - Recall not found")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSRecallNotFound,
		///<summary>23 - Web Sched Already Scheduled.</summary>
		[Description("Web Sched - Recall already scheduled")]
		[EServiceLogType(eServiceType.WSGeneral,eServiceType.WSNewPat,eServiceType.WSExistingPat,eServiceType.WSRecall,eServiceType.WSAsap)]
		WSRecallAlreadyScheduled,
		///<summary>24 - Arrivals, patient arrived.</summary>
		[Description("Arrival - Patient arrived")]
		[EServiceLogType(eServiceType.Arrivals)]
		ArrivalReceived,
		///<summary>25 - Integrated Texting, Patient Opted Out.</summary>
		[Description("Integrated Texting - Patient opted out")]
		[EServiceLogType(eServiceType.IntegratedTexting)]
		IntegratedTextingOptOut,
		///<summary>26 - Patient Portal, Patient Arrived At Statement Portal.</summary>
		[Description("Patient Portal - Patient arrived at statement portal")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPStatementPortalArrived,
		///<summary>27 - Patient Portal, Patient Logged In At Statement Portal.</summary>
		[Description("Patient Portal - Patient logged in at statement portal")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPStatementPortalLoggedIn,
		///<summary>28 - Patient Portal, Patient Downloaded Statement.</summary>
		[Description("Patient Portal - Patient downloaded statement")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPStatementPortalDownloadStatement,
		///<summary>29 - Patient Portal, Patient Opened Payment Form.</summary>
		[Description("Patient Portal - Patient opened payment form")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentFormOpened,
		///<summary>30 - Patient Portal, Patient Opened Payment Form From Login.</summary>
		[Description("Patient Portal - Patient opened payment form from login")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentFormOpenedFromLogin,
		///<summary>31 - Patient Portal, Patient Opened Hosted Payment Form.</summary>
		[Description("Patient Portal - Patient opened hosted payment form")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPOpenedHostedPaymentForm,
		///<summary>32 - Patient Portal, Patient Paid With Existing Card.</summary>
		[Description("Patient Portal - Patient paid with existing card")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPayWithExistingFromClient,
		///<summary>33 - Patient Portal, Patient Submitted Payment With XWeb.</summary>
		[Description("Patient Portal - Patient submitted payment with XWeb")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentCreatedByXWeb,
		///<summary>34 - Patient Portal, Patient Submitted Payment With PayConnect.</summary>
		[Description("Patient Portal - Patient submitted payment with PayConnect")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPPaymentCreatedByPayconnect,
		///<summary>35 - Patient Portal, Patient Notified of Possible Duplicate Payment.</summary>
		[Description("Patient Portal - Patient notified of possible duplicate payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPDuplicatePaymentAlert,
		///<summary>36 - Patient Portal, Patient Allowed Submission of Duplicate Payment.</summary>
		[Description("Patient Portal - Patient allowed submission of duplicate payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPDuplicatePaymentAllowed,
		///<summary>37 - Patient Portal, Patient Rejected Submission of Duplicate Payment.</summary>
		[Description("Patient Portal - Patient rejected submission of duplicate payment")]
		[EServiceLogType(eServiceType.PatientPortal)]
		PPDuplicatePaymentDenied,
		/// <summary>38 - eClipboard - Check In for patients bringing their own device</summary>
		[Description("eClipboard - Check in bring your own device")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInBYOD,
		/// <summary>39 - eClipboard - Check In process started</summary>
		[Description("eClipboard - Check in process started")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInStarted,
		/// <summary>40 - eClipboard - Check In Arrived</summary>
		[Description("eClipboard - Check in arrived")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInArrived,
		/// <summary>41 - eClipboard - Check In Submitted</summary>
		[Description("eClipboard - Check in submitted")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInSubmitted,
		/// <summary>42 - eClipboard - Check In Error: Appt Not Found</summary>
		[Description("eClipboard - Error: Appt not found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorApptNotFound,
		/// <summary>43 - eClipboard - Check In Error: PatNum Not Linked To Appt</summary>
		[Description("eClipboard - Error: PatNum not linked to appt")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorPatNumNotLinkedToAppt,
		/// <summary>44 - eClipboard - Error: Device Setup for Other Clinic</summary>
		[Description("eClipboard - Error: Device setup for other clinic")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorDeviceSetupForOtherClinic,
		/// <summary>45 - eClipboard - Check In Error: Device Not Allowed for Checkin</summary>
		[Description("eClipboard - Error: Device not allowed for checkin")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorDeviceNotAllowedForCheckin,
		/// <summary>46 - eClipboard - Check In Error: Office Device used as BYOD</summary>
		[Description("eClipboard - Error: Office device used as BYOD")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorOfficeDeviceUsedAsBYOD,
		/// <summary>47 - eClipboard - Check In Error: No Appointment Found BYOD</summary>
		[Description("eClipboard - Error: No appointment found BYOD")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorNoApptFoundBYOD,
		/// <summary>48 - eClipboard - Check In Error: No Appt Found</summary>
		[Description("eClipboard - Error: No appt found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorNoApptFound,
		/// <summary>49 - eClipboard - Check In Error: Multiple Pats Found</summary>
		[Description("eClipboard - Error: Multiple pats found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorMultiplePatsFound,
		/// <summary>50 - eClipboard - Check In Error: Signature Error</summary>
		[Description("eClipboard - Error: Signature error")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorSignatureError,
		/// <summary>51 - eClipboard - Check In Error: Deprecated Method</summary>
		[Description("eClipboard - Error: Deprecated method")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInErrorDeprecatedMethod,
		/// <summary>52 - eClipboard - Check In Confirmed Appt With Prov - Yes</summary>
		[Description("eClipboard - Check in confirmed appt with prov - Yes")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInConfirmedApptWithProvYes,
		/// <summary>53 - eClipboard - Check In Confirmed Appt With Prov - No</summary>
		[Description("eClipboard - Check in confirmed appt with prov - No")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInConfirmedApptWithProvNo,
		/// <summary>54 - eClipboard - Check In Took selfie before submitting</summary>
		[Description("eClipboard - Check in list took selfie before submitting")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSubmittedWithPicture,
		/// <summary>55 - eClipboard - Check In List Did not take selfie before submitting</summary>
		[Description("eClipboard - Check in list did not take selfie before submitting")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSubmittedWithOutPicture,
		/// <summary>56 - eClipboard - Check In List Error, submitted without all items</summary>
		[Description("eClipboard - Error: Check in list submitted without all items")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListErrorSubmittedWithoutAllItems,
		/// <summary>57 - eClipboard - Check In List Submit Xamarin Error</summary>
		[Description("eClipboard - Error: Check in list submit xamarin")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListXamSubmitError,
		/// <summary>58 - eClipboard - Check In Submit Success</summary>
		[Description("eClipboard - Check in list submit success")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSumbitSuccess,
		/// <summary>59 - eClipboard - Check In Submit Success BYOD</summary>
		[Description("eClipboard - Check in list submit success BYOD")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSumbitSuccessBYOD,
		/// <summary>60 - eClipboard - Check In List Selected Item</summary>
		[Description("eClipboard - Check in list selected item")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSelectedItem,
		/// <summary>61 - eClipboard - Check In List Sheet Next Tapped</summary>
		[Description("eClipboard - Check in list sheet next tapped")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetNextTapped,
		/// <summary>62 - eClipboard - Check In List Sheet Prev Tapped</summary>
		[Description("eClipboard - Check in list sheet prev tapped")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetPrevTapped,
		/// <summary>63 - eClipboard - Check In List Sheet Office signed Treatment Plan</summary>
		[Description("eClipboard - Check in list office signed treatment plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetOfficeSignedTreatPlan,
		/// <summary>64 - eClipboard - Check In List Sheet Patient signed treatment plan</summary>
		[Description("eClipboard - Check in list patient signed treatment plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetPatientSignedTreatPlan,
		/// <summary>65 - eClipboard - Check In List Sheet Patient signed Payment Plan</summary>
		[Description("eClipboard - Check in list patient signed payment plan")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCheckInListSheetPatientSignedPaymentPlan,
		/// <summary>66 - eClipboard - Check In List Sheet Patient signed Payment Plan</summary>
		[Description("eClipboard - Patient directed to make payment")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECPatientDirectedToMakePayment,
		/// <summary>67 - eClipboard - BYOD 6 Digit validation page reached</summary>
		[Description("eClipboard - BYOD 6 digit validation page reached")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECBYODValidationReached,
		/// <summary>68 - eClipboard - BYOD 6 Digit validation failed</summary>
		[Description("eClipboard - BYOD 6 digit validation failed")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECBYODValidationFailed,
		/// <summary>69 - eClipboard - BYOD 6 Digit validation success</summary>
		[Description("eClipboard - BYOD 6 digit validation success")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECBYODValidationSuccess,
		/// <summary>70 - eClipboard - 2 Factor Auth Screen Shown</summary>
		[Description("eClipboard - 2 Factor auth screen shown")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthShown,
		/// <summary>71 - eClipboard - 2 Factor Auth Close Clicked</summary>
		[Description("eClipboard - 2 Factor auth close clicked")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthClosed,
		/// <summary>72 - eClipboard - 2 Factor Auth Email Selected</summary>
		[Description("eClipboard - 2 Factor auth email selected")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthEmailSelected,
		/// <summary>73 - eClipboard - 2 Factor Auth Text Selected</summary>
		[Description("eClipboard - 2 Factor auth text selected")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthTextSelected,
		/// <summary>74 - eClipboard - 2 Factor Auth Code Submitted</summary>
		[Description("eClipboard - 2 Factor auth code submitted")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthCodeSubmitted,
		/// <summary>75 - eClipboard - 2 Factor Auth Code Success</summary>
		[Description("eClipboard - 2 Factor auth code success")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthCodeSuccess,
		/// <summary>76 - eClipboard - 2 Factor Auth Code Fail</summary>
		[Description("eClipboard - 2 Factor auth code fail")]
		[EServiceLogType(eServiceType.EClipboard)]
		EC2FactorAuthCodeFail,
		/// <summary>77 - Used in Xam Exceptions. If this is sent back, error will not be logged. Default eServiceAction for XamException.</summary>
		[Description("eClipboard - Error: Xam exceptions")]
		[EServiceLogType(eServiceType.Unknown)]
		DoNotLog,
		/// <summary>78 - eClipboard - patient opens payment page. Note should indicate where it is opened from</summary>
		[Description("eClipboard - Patient opened payment page")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECOpenPaymentPage,
		/// <summary>79 - eClipboard - User tapped "Add Card"</summary>
		[Description("eClipboard - User tapped 'Add Card'")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECAddCreditCardTapped,
		/// <summary>80 - eClipboard - User tapped 'Done' on credit card manage page </summary>
		[Description("eClipboard - User tapped 'Done' on credit card manage page")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardManageDoneTapped,
		/// <summary>81 - eClipboard - User removed a credit card </summary>
		[Description("eClipboard - User removed a credit card")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardRemoved,
		/// <summary>82 - eClipboard - User made payment with new credit card</summary>
		[Description("eClipboard - User made payment with new credit card")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardPaymentWithNewCard,
		/// <summary>83 - eClipboard - User made payment with existing credit card</summary>
		[Description("eClipboard - User made payment with existing credit card")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardPaymentWIthExistingCard,
		/// <summary>84 - eClipboard - User tapped cancel when making a payment </summary>
		[Description("eClipboard - User tapped cancel when making a payment")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardPaymentCancelled,
		/// <summary>85 - eClipboard - Error: Delete card not found</summary>
		[Description("eClipboard - Error: Delete card not found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorDeleteCardNotFound,
		/// <summary>86 - eClipboard - Error: Delete credit card patnum does not match current patnum</summary>
		[Description("eClipboard - Error: Delete credit card patnum does not match current patnum")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorDeleteCardPatNumDoesNotMatch,
		/// <summary>87 - eClipboard - Error: Delete credit card invalid alias</summary>
		[Description("eClipboard - Error: Delete credit card invalid alias")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorDeleteCardInvalidAlias,
		/// <summary>88 - eClipboard - Error: Delete credit card patient not found</summary>
		[Description("eClipboard - Error: Delete credit card patient not found")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorDeleteCardPatientNotFound,
		/// <summary>89 - eClipboard - Error: error making payment</summary>
		[Description("eClipboard - Error: Making payment with alias")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorMakingPaymentWithAlias,
		/// <summary>90 - eClipboard - Error: Making Payment, patient not found</summary>
		[Description("eClipboard - Error: Patient not found for payment")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorMakingPaymentPatientNotFound,
		/// <summary>91 - eClipboard - Error: Making Payment, invalid amount </summary>
		[Description("eClipboard - Error: Invalid amount to make payment")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECCreditCardErrorMakingPaymentInvalidAmount,
		/// <summary>92 - EClipboard - QR scan window activated</summary>
		[Description("QR scan window activated")]
		[EServiceLogType(eServiceType.EClipboard,eServiceType.ODTouch)]
		ECQRScanAttempt,
		/// <summary>93 - EClipboard - QR scan window cancelled</summary>
		[Description("QR scan cancelled")]
		[EServiceLogType(eServiceType.EClipboard,eServiceType.ODTouch)]
		ECQRScanCancel,
		/// <summary>94 - EClipboard - QR scan window success</summary>
		[Description("QR scan window success")]
		[EServiceLogType(eServiceType.EClipboard,eServiceType.ODTouch)]
		ECQRScanOk,
		/// <summary>95 - EClipboard - Error: Failed to submit sheet</summary>
		[Description("Failed to submit sheet")]
		[EServiceLogType(eServiceType.EClipboard)]
		ECSubmitSheetFailed,
		///<summary>96 - Web Forms Form Downloaded.</summary>
		[Description("Web Forms - Downloaded web form")]
		[EServiceLogType(eServiceType.WebForms)]
		WFDownloadedForm,
		///<summary>97 - Web Forms Form Discarded.</summary>
		[Description("Web Forms - Discarded web form")]
		[EServiceLogType(eServiceType.WebForms)]
		WFDiscardedForm,
		///<summary>98 - Web Forms Form Skipped.</summary>
		[Description("Web Forms - Skipped web form")]
		[EServiceLogType(eServiceType.WebForms)]
		WFSkippedForm,
		///<summary>99 - Web Forms Form Deleted.</summary>
		[Description("Web Forms - Deleted web form")]
		[EServiceLogType(eServiceType.WebForms)]
		WFDeletedForm,
		/// <summary>100 - Web Forms Error.</summary>
		[Description("Web Forms - Error")]
		[EServiceLogType(eServiceType.WebForms)]
		WFError,
		/// <summary>101 - Web Forms Cancelled Import.</summary>
		[Description("Web Forms - Import Cancelled")]
		[EServiceLogType(eServiceType.WebForms)]
		WFCancelled,
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