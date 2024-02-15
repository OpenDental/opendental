using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>Communication item from workstation to OD HQ. Stores the statuses of the eConnector and other eServices. </summary>
	[Serializable]
	public class EServiceSignal:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EServiceSignalNum;
		///<summary>Enum eServiceCode. Service which this signal applies to.</summary>
		public int ServiceCode;
		///<summary>The enum is at HQ as OpenDentalWebCore.BroadcasterThreadDefs. Can be zero if no grouping is necessary per a given service.</summary>
		public int ReasonCategory;
		///<summary>The enum is at HQ as OpenDentalWebCore.BroadcasterErrorCodes. This code is used to determine what actions to take and how to process this message. 
		///It is a function of ReasonCategory. It will most likely be defined by an enum that lives on HQ-only closed source.</summary>
		public int ReasonCode;
		///<summary>Enum:eServiceSignalSeverity </summary>
		public eServiceSignalSeverity Severity;
		///<summary>Human readable description of what this signal means, or a message for the user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Description;
		///<summary>Time signal was sent.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime SigDateTime;
		///<summary>Used to store serialized data that can be used for processing this signal.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Tag;
		///<summary>After a message has been processed or acknowledged this is set true. Not currently used for heartbeat or service status signals.</summary>
		public bool IsProcessed;

		///<summary></summary>
		public EServiceSignal Copy() {
			return (EServiceSignal)this.MemberwiseClone();
		}
	}

	///<summary>Used by EServiceSignal.ServiceCode. Each service will have an entry here. Stored as an int for forward compatibility.</summary>
	public enum eServiceCode {
		///<summary>0 - Should not be used. If you are seeing this then an entry was made incorrectly.</summary>		
		Undefined = 0,
		///<summary>1 - Runs 1 instance per customer on a given client PC.</summary>		
		ListenerService = 1,
		///<summary>2 - Runs 1 instance total on HQ server.</summary>		
		[Description("Texting Access")]
		IntegratedTexting = 2,
		///<summary>3 - Runs 1 instance total on HQ server.</summary>		
		HQProxyService = 3,
		///<summary>4 - EService WebApp.</summary>		
		[Description("Mobile Web")]
		MobileWeb,
		///<summary>5 - EService WebApp.</summary>		
		[Description("Patient Portal")]
		PatientPortal,
		///<summary>6 - EService WebApp. The "Recall" version of Web Sched.</summary>		
		[Description("Web Sched Recalls")]
		WebSched,
		///<summary>7 - EService WebApp.</summary>		
		[Description("Web Forms")]
		WebForms,
		///<summary>8 - EService WebApp.</summary>		
		ResellerPortal,
		///<summary>9 - EService WebApp.</summary>		
		FeaturePortal,
		///<summary>10 - EService WebApp.</summary>		
		[Description("Auto E-Confirmation")]
		ConfirmationRequest,
		///<summary>11 - EService WebApp.</summary>		
		OAuth,
		///<summary>12 - RESTful API from HL7.</summary>		
		FHIR,
		///<summary>13 - EService WebApp. The "New Patient Appointment" version of Web Sched.</summary>
		[Description("Web Sched New Patient")]
		WebSchedNewPatAppt,
		///<summary>14 - HQ only WebApp. Allows HQ to remotely modify web services.</summary>
		HQManager,
		///<summary>15 - Entitles this practice/clinic to all eServices. Supercedes any other repeat charges for this practice/clinic.</summary>
		[Description("E-Services Bundle")]
		Bundle,
		///<summary>16 - IntegratedTexting is the actual enum value for texting access.  This value is for the usage portion.
		///Not used in billing, mainly used to keep technicians from manually adding the "TextUse" procedure code as a repeating charge.</summary>
		[Description("Texting Usage")]
		IntegratedTextingUsage,
		///<summary>17 - Resellers need to be able to give this service (not technically an eService) to their customers via sign up portal.</summary>		
		[Description("Software Only")]
		ResellerSoftwareOnly,
		///<summary>18 - Denotes the SignupPortal web app.  Only currently used to get a new URL path separate from FeaturePortal.</summary>		
		SignupPortal,
		///<summary>19 - Used by WebServiceCustomerUpdate to ask WebServiceHQ if this RegKey is eligible for OD proper version updates.</summary>		
		SoftwareUpdate,
		///<summary>20 - EService Web App. The "ASAP" version of Web Sched.</summary>		
		[Description("Web Sched ASAP")]
		WebSchedASAP,
		///<summary>21 - Request made to store information about unhandled exceptions</summary>		
		[Description("Bug Submission")]
		BugSubmission,
		/// <summary>22 - </summary>
		[Description("Make Payment")]
		PatientPortalMakePayment,
		/// <summary>23 - </summary>
		[Description("View Statement")]
		PatientPortalViewStatement,
		///<summary>24 - </summary>
		WebHostSynch,
		///<summary>25 - Monitoring app used by OD HQ.</summary>
		Headmaster,
		///<summary>26 - EClipboard mobile application.</summary>
		[Description("eClipboard")]
		EClipboard,
		///<summary>27 - Displays Help information.</summary>
		ODHelp,
		///<summary>28- Originally for paysimple ACH payments</summary>
		PaySimple,
		///<summary>29 - Used for storing customers OD software versions.</summary>
		CustomerVersion,
		///<summary>30 - eServiceCode that corresponds to ProcCode 045 in customers db at HQ. Not used for eService validation. 
		///Use ConfirmationRequest insted.</summary>
		ConfirmationOwn,
		///<summary>31 - eServiceCode that corresponds to ProcCode 046 in customers db at HQ. Not used for eService validation. 
		///Use IntegratedTexting insted.</summary>
		IntegratedTextingOwn,
		///<summary>32 - eServiceCode that corresponds to ProcCode 030 in customers db at HQ. Not used for eService validation.</summary>
		SoftwareOnly,
		///<summary>33</summary>
		SupplementalBackup,
		///<summary>34 - Will have a $0 RepeatCharge. Procedure will be generated each month as a function of number of masss email messages sent. Each email message has an incremental cost.</summary>
		[HostedUrl(CanResellerOverride = true)]
		[Description("Mass Email Usage")]
		EmailMassUsage,
		///<summary>35 - Will have a $0 RepeatCharge. Procedure will be generated each month as a function of number of secure email messages sent. Each email message has an incremental cost.</summary>
		[Description("Secure Email Usage")]
		EmailSecureUsage,
		///<summary>36 - Has a RepeatCharge. Clinics sign up for access to use secure email. Each email sent will be charged an additional fee, see EmailSecureUsage.</summary>
		[Description("Secure Email Access")]
		EmailSecureAccess,
		///<summary>37 - eService for Automated Appointment Thank-Yous and calendar events.</summary>
		[Description("Automated Appointment Thank-You")]
		ApptThankYou,
		///<summary>38.</summary>
		OregonCryo,
		///<summary>39 - eServices logging service.</summary>
		EserviceLog,
		///<summary>40 - Used for storing customers license agreement acceptance signature. </summary>
		LicenseAgreementSig,
		///<summary>41 - A Windows service running at HQ that is monitored by Headmaster.</summary>
		WebFormManager,
		///<summary>42 - Eclipboard Clinical Mobile application.</summary>
		ODTouch,
		///<summary>43 - Payment Portal UI.</summary>
		PaymentPortalUI,
		///<summary>44 - Payment Portal Api.</summary>
		PaymentPortalApi,
		///<summary>45 - EService Api.</summary>
		EServiceApi,
		///<summary>46 - Auth Api.</summary>
		AuthApi,
		/// <summary>47 - ODTouch Mobile application surcharge for additional devices that exceed the ODTouchDeviceLimitDefault pref.</summary>
		ODTSurplus,
	}
	
	///<summary>Used to determine that status of the entire service.  Order of enum is important, from lowest to highest importance.</summary>
	public enum eServiceSignalSeverity {
		///<summary>Service is not in use and is not supposed to be in use.</summary>
		None=-1,
		///<summary>0-Service is not in use and is not supposed to be in use.</summary>
		NotEnabled,
		///<summary>1-Used to convey information. Does not change the "working" status of the service. Will always be inserted with IsProcess=true.</summary>
		Info,
		///<summary>2-Service is operational and working as designed. Typcially used for heartbeat and initialization.</summary>
		Working,
		///<summary>3-Recoverable error has has occurred and no user intervention is required. Typically requires user acknowledgement only.</summary>
		Warning,
		///<summary>4-Recoverable error has has occurred and user intervention is probably required in addition to user acknowledgement only.</summary>
		Error,
		///<summary>5-Unrecoverable error and the service has shut itself off. Immediate user intervention is required.</summary>
		Critical
	}

}