using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	///<summary>Stores information on mobile app devices. These are devices that utilize the Xamarin mobile application.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class MobileAppDevice : TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileAppDeviceNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>The name of the device.</summary>
		public string DeviceName;
		///<summary>The unique identifier of the device. Platform specific.</summary>
		public string UniqueID;
		///<summary>Indicates whether the device is allowed to operate the checkin app. 
		///For BYOD sessions will always be true because BYOD is authenticated by a unique URL link in a text message.</summary>
		public bool IsEclipboardEnabled;
		///<summary>FK to patient.PatNum. Indicates which patient is currently using the device. 0 indicates the device is not in use. -1 indicates
		///that the device is in use but we do not yet know which patient is using the device.</summary>
		public long PatNum;
		///<summary>Indicates whether a device is a BYOD device, defaults to false.</summary>
		public bool IsBYODDevice;
		///<summary>The date and time when we last updated the PatNum field for this device (indication the current use-state of the device).</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastCheckInActivity;
		///<summary>The date and time of the last attempted login for Eclipboard.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime EclipboardLastAttempt;
		///<summary>The date and time of the last successful login for Eclipboard.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime EclipboardLastLogin;
		///<summary>Current page of the device.</summary>
		public MADPage DevicePage;
		///<summary>FK to userod.UserNum. Indicates which user is currently logged into the device. 0 indicates this device is not logged into.</summary>
		public long UserNum;
		///<summary>The date and time of the last successful login for ODTouch.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime ODTouchLastLogin;
		///<summary>The date and time of the last attempted login for ODTouch.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime ODTouchLastAttempt;
		/// <summary>Indicates whether this device is being used for ODTouch or not.</summary>
		public bool IsODTouchEnabled;

		///<summary>Returns a copy of this MobileAppDevice.</summary>
		public MobileAppDevice Copy() {
			return (MobileAppDevice)this.MemberwiseClone();
		}
	}

	///<summary>Enum that stores all of the pages in MobileAppDevice. If a new entry is added, we also need to add it to MobileAppDevice.cs in ODXam_Prod.</summary>
	public enum MADPage {
		Undefined,
		[Description("Checkin Page")]
		CheckinPage,
		[Description("Settings Page")]
		SettingsListPage,
		[Description("Settings Page Split")]
		SettingsSecondaryPage,
		[Description("About Page")]
		AboutPage,
		[Description("Appointments Page")]
		AppointmentsPage,
		[Description("Appointment View Page")]
		AppointmentViewPage,
		[Description("Byod Validation Page")]
		ByodValidationPage,
		[Description("Checkin Checklist Page")]
		CheckinChecklistPage,
		[Description("Clinician Home Page")]
		ClinicianHomePage,
		[Description("Erx View Page")]
		ErxViewPage,
		[Description("Exam Sheets Page")]
		ExamSheetsPage,
		[Description("File Viewer Page")]
		FileViewerPage,
		[Description("Main Menu Page")]
		MainMenuPage,
		[Description("Master Page")]
		MasterPage,
		[Description("More Options Page")]
		MoreOptionsPage,
		[Description("Mobile Web Login Page")]
		MWLoginPage,
		[Description("Mobile Web Master Page")]
		MWMasterPage,
		[Description("Patient Edit Page")]
		PatientEditPage,
		[Description("Patient Payment Entry Page")]
		PatientPaymentEntryPage,
		[Description("Patient Payment Manage Page")]
		PatientPaymentManagePage,
		[Description("Patient Payment Page")]
		PatientPaymentPage,
		[Description("Payment Plan List Page")]
		PaymentPlanListPage,
		[Description("Treatment Plan Edit Module Page")]
		TreatPlanModuleEditPlanPage,
		[Description("Payment Plan Page")]
		PaymentPlanPage,
		[Description("Perio Exam Edit Page")]
		PerioExamEditPage,
		[Description("Perio Exam List Page")]
		PerioExamListPage,
		[Description("Perio Exam Overview Page")]
		PerioExamOverviewPage,
		[Description("Pin Input Page")]
		PinInputPage,
		[Description("Provider Select Page")]
		ProviderSelectPage,
		[Description("Sheet Page")]
		SheetPage,
		[Description("Sms Combined Page")]
		SmsCombinedPage,
		//SmsSignupPage,
		[Description("Sms Compose New Page")]
		SmsComposeNewPage,
		[Description("Sms Conversation Page")]
		SmsConversationPage,
		[Description("Sms Conversation List Page")]
		SmsConversationListPage,
		[Description("Treatment Plan List Page")]
		TreatmentPlanListPage,
		[Description("Treatment Plan Page")]
		TreatmentPlanPage,
		[Description("Troubleshoot Connections Page")]
		TroubleshootConnectionsPage,
		[Description("TwoFactor Auth Page")]
		TwoFactorAuthPage,
		[Description("Ui Viewer Page")]
		UiViewerPage,
		[Description("Validate User Page")]
		ValidateUserPage,
		[Description("Validation Failed Page")]
		ValidationFailedPage,
		[Description("Patient Actions Page")]
		PatientSelectedBubbleAction
	}

}

