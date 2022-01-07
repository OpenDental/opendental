using System;
using System.ComponentModel;

namespace OpenDentBusiness{
	///<summary>This is an enumeration of all the enumeration types that are used in the database.  This is used in the reporting classes to make the data human-readable.  May need to be updated with recent additions.</summary>
	public enum EnumType{
		///<summary></summary>
		YN,
		///<summary></summary>
		Relat,
		///<summary></summary>
		Month,
		///<summary></summary>
		ProcStat,
		///<summary></summary>
		DefCat,
		///<summary></summary>
		TreatmentArea,
		///<summary></summary>
		DentalSpecialty,
		///<summary></summary>
		ApptStatus,
		///<summary></summary>
		PatientStatus,
		///<summary></summary>
		PatientGender,
		///<summary></summary>
		PatientPosition,
		///<summary></summary>
		ScheduleType,
		///<summary></summary>
		LabCase,
		///<summary></summary>
		PlaceOfService,
		///<summary></summary>
		PaintType,
		///<summary></summary>
		SchedStatus,
		///<summary></summary>
		AutoCondition,
		///<summary></summary>
		ClaimProcStatus,
		///<summary></summary>
		CommItemType,
		///<summary></summary>
		ToolBarsAvail,
		///<summary></summary>
		ProblemStatus,
		///<summary></summary>
		EZTwainErrorCode,
		///<summary></summary>
		ScaleType,
		///<summary></summary>
		SortStrategy,
		///<summary></summary>
		ProcCodeListSort
	}
	///<summary>0=Unknown,1=Yes, or 2=No. UI can be tricky. Some success with a 3x1 listbox, multicolumn, see FormPatientEdit. Radiobuttons are also an option.  You can also use another YN variable to store unknown, and then use a click event on the checkbox to change to Y or N.  You can also use a three state checkbox if you translate properly between the enums.</summary>
	public enum YN{
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		Yes,
		///<summary>2</summary>
		No}
	///<summary>Relationship to subscriber for insurance.</summary>
	public enum Relat{
		///<summary>0</summary>
		Self,
		///<summary>1</summary>
		Spouse,
		///<summary>2</summary>
		Child,
		///<summary>3</summary>
		Employee,
		///<summary>4</summary>
		HandicapDep,
		///<summary>5</summary>
		SignifOther,
		///<summary>6</summary>
		InjuredPlaintiff,
		///<summary>7</summary>
		LifePartner,
		///<summary>8</summary>
		Dependent
	}
	///<summary></summary>
	public enum Month{
		///<summary>1</summary>
		Jan=1,
		///<summary>2</summary>
		Feb,
		///<summary>3</summary>
		Mar,
		///<summary>4</summary>
		Apr,
		///<summary>5</summary>
		May,
		///<summary>6</summary>
		Jun,
		///<summary>7</summary>
		Jul,
		///<summary>8</summary>
		Aug,
		///<summary>9</summary>
		Sep,
		///<summary>10</summary>
		Oct,
		///<summary>11</summary>
		Nov,
		///<summary>12</summary>
		Dec}
	///<summary>Progress notes line type. Used when displaying lines in the Chart module.</summary>
	public enum ProgType{
		///<summary>1</summary>
		Proc=1,
		///<summary>2</summary>
		Rx}
	///<summary>Primary, secondary, or total. Used in some insurance estimates to specify which kind of estimate is needed.</summary>
	public enum PriSecTot{
		///<summary>0</summary>
		Pri,
		///<summary>1</summary>
		Sec,
		///<summary>2</summary>
		Tot}
		//<summary>3</summary>
		//Other}
	///<summary>Procedure Status.  These statuses are transalted via class type "enumProcStat" (ex Lan.g("enumProcStat","..."))</summary>
	public enum ProcStat{
		///<summary>1- Treatment Plan.</summary>
		TP=1,
		///<summary>2- Complete.</summary>
		C,
		///<summary>3- Existing Current Provider.</summary>
		EC,
		///<summary>4- Existing Other Provider.</summary>
		EO,
		///<summary>5- Referred Out.</summary>
		R,
		///<summary>6- Deleted.</summary>
		D,
		///<summary>7- Condition.</summary>
		Cn,
		///<summary>8- Treatment Plan inactive.</summary>
		TPi,
		//See ProcStatExt for pseudo statuses.
	}

	///<summary>The pseudo statuses inside this extended enum must always be mutually exclusive of the values inside the ProcStat enum.
	///These statuses are transalted via class type "enumProcStat" (ex Lan.g("enumProcStat",ProcStatExt.InProcess))</summary>
	public class ProcStatExt {
		///<summary>I - Stands for "Invalid".</summary>
		public const string Invalid="I";
		///<summary>C/P - Stands for "Complete (In Process)".</summary>
		public const string InProcess="C/P";
	}		
	
	//public enum StudentStat{None,Full,Part};
	///<summary>Used in procedurecode setup to specify the treatment area for a procedure.  This determines what fields are available when editing an appointment.</summary>
	public enum TreatmentArea{
		///<summary>0-goes on claims as blank.</summary>
		None,
		///<summary>1</summary>
		Surf,
		///<summary>2</summary>
		Tooth,
		///<summary>3-goes on claims as 00.</summary>
		Mouth,
		///<summary>4</summary>
		Quad,
		///<summary>5</summary>
		Sextant,
		///<summary>6</summary>
		Arch,
		///<summary>7</summary>
		ToothRange
	}

	///<summary>When the autorefresh message is sent to the other computers, this is the type.</summary>
	public enum InvalidType{
		///<summary>0</summary>
		None,
		///<summary>1 Deprecated. Not used with any other flags</summary>
		Date,
		///<summary>2 Deprecated.  Inefficient.  All flags combined except Date and Tasks.</summary>
		AllLocal,
		///<summary>3 Not used with any other flags.  Used to just indicate added tasks, but now it indicates any change at all except those where a popup is needed.  If we also want a popup, then use TaskPopup.</summary>
		Task,
		///<summary>4</summary>
		ProcCodes,
		///<summary>5</summary>
		Prefs,
		///<summary>6 ApptViews, ApptViewItems, AppointmentRules, ProcApptColors.</summary>
		Views,
		///<summary>7</summary>
		AutoCodes,
		///<summary>8</summary>
		Carriers,
		///<summary>9</summary>
		ClearHouses,
		///<summary>10</summary>
		Computers,
		///<summary>11</summary>
		InsCats,
		///<summary>12- Also includes payperiods.</summary>
		Employees,
		///<summary>13- Deprecated.</summary>
		StartupOld,
		///<summary>14</summary>
		Defs,
    ///<summary>15. Templates and addresses, but not messages.</summary>
    Email,
		///<summary>16. Obsolete</summary>
		Fees,
		///<summary>17</summary>
		Letters,
		///<summary>18- Invalidates quick paste notes and cats.</summary>
		QuickPaste,
		///<summary>19- Userods, UserGroups, UserGroupAttaches, and GroupPermissions</summary>
		Security,
		///<summary>20 - Also includes program properties.</summary>
		Programs,
		///<summary>21- Also includes MountDefs and ImagingDevices</summary>
		ToolButsAndMounts,
		///<summary>22- Also includes clinics.</summary>
		Providers,
		///<summary>23- Also includes ClaimFormItems.</summary>
		ClaimForms,
		///<summary>24</summary>
		ZipCodes,
		///<summary>25</summary>
		LetterMerge,
		///<summary>26</summary>
		DentalSchools,
		///<summary>27</summary>
		Operatories,
		///<summary>28</summary>
		TaskPopup,
		///<summary>29</summary>
		Sites,
		///<summary>30</summary>
		Pharmacies,
		///<summary>31</summary>
		Sheets,
		///<summary>32</summary>
		RecallTypes,
		///<summary>33</summary>
		FeeScheds,
		///<summary>34. This is used internally by OD, Inc with the phonenumber table and the phone server.</summary>
		PhoneNumbers,
		///<summary>35. Deprecated, use SigMessages instead.  Old summary: Signal/message defs</summary>
		Signals,
		///<summary>36</summary>
		DisplayFields,
		///<summary>37. And ApptFields.</summary>
		PatFields,
		///<summary>38</summary>
		AccountingAutoPays,
		///<summary>39</summary>
		ProcButtons,
		///<summary>40.  Includes ICD9s.</summary>
		Diseases,
		///<summary>41</summary>
		Languages,
		///<summary>42</summary>
		AutoNotes,
		///<summary>43</summary>
		ElectIDs,
		///<summary>44</summary>
		Employers,
		///<summary>45</summary>
		ProviderIdents,
		///<summary>46</summary>
		ShutDownNow,
		///<summary>47</summary>
		InsFilingCodes,
		///<summary>48</summary>
		ReplicationServers,
		///<summary>49</summary>
		Automation,
		///<summary>50. This is used internally by OD, Inc with the phone server to trigger the phone system to reload after changing which call groups users are in.
		///Also used when sending a signal to the phone tracking server to kick users in conference rooms.</summary>
		PhoneAsteriskReload,
		///<summary>51</summary>
		TimeCardRules,
		///<summary>52. Includes DrugManufacturers and DrugUnits.</summary>
		Vaccines,
		///<summary>53. Includes all 4 HL7Def tables.</summary>
		HL7Defs,
		///<summary>54</summary>
		DictCustoms,
		///<summary>55. Caches the wiki master page and the wikiListHeaderWidths</summary>
		Wiki,
		///<summary>56. SourceOfPayment</summary>
		Sops,
		///<summary>57. In-Memory table used for hard-coded codes and CQMs</summary>
		EhrCodes,
		///<summary>58. Used to override appointment color.  Might be used for other appointment attributes in the future.</summary>
		AppointmentTypes,
		///<summary>59. Caches the medication list to stop from over-refreshing and causing slowness.</summary>
		Medications,
		///<summary>60. This is a special InvalidType which indicates a refresh, but also includes the data to be refreshed inside of the signalod.FKey field.</summary>
		SmsTextMsgReceivedUnreadCount,
		///<summary>61</summary>
		ProviderErxs,
		///<summary>62. This is used internally by OD, refreshes the jobs windows in the Job Manager.</summary>
		Jobs,
		///<summary>63. This is used internally by OD, refreshes the jobRoles</summary>
		JobPermission,
		///<summary>64. Caches the StateAbbrs used for helping prefill state fields and for state validations.</summary>
		StateAbbrs,
		///<summary>65</summary>
		RequiredFields,
		///<summary>66</summary>
		Ebills,
		///<summary>67</summary>
		UserClinics,
		///<summary>68. Replaces the deprecated "Date" invalid type for more granularity on invalid signals.</summary>
		Appointment,
		///<summary>69</summary>
		OrthoChartTabs,
		///<summary>70. A user either acknowledged or added to the messaging buttons system.</summary>
		SigMessages,
		///<summary>71. Deprecated.</summary>
		AlertSubs,
		///<summary>72. THIS IS NOT CACHED. But is used to make server run the alert logic in OpenDentalService.</summary>
		AlertItems,
		///<summary>73. This is used internally by OD, refreshes the voice mails.</summary>
		VoiceMails,
		///<summary>74. Used to refresh the active kiosk grid in FormTerminalManager and loaded patient with list of forms in FormTerminal.</summary>
		Kiosk,
    ///<summary>75</summary>
    ClinicPrefs,
    ///<summary>76. Not addresses or templates, but inbox and sent messages.</summary>
    EmailMessages,
		///<summary>77. The eConnector has finished sending web sched recall reminders.</summary>
		WebSchedRecallReminders,
		///<summary>78.</summary>
		SmsBlockPhones,
		///<summary>79.</summary>
		AlertCategories,
		///<summary>80.</summary>
		AlertCategoryLinks,
		///<summary>81. Used in updating menu item in report menu.</summary>
		UnfinalizedPayMenuUpdate,
		///<summary>82. Used for validating clinics for eRx.</summary>
		ClinicErxs,
		///<summary>83.</summary>
		DisplayReports,
		///<summary>84.</summary>
		UserQueries,
		///<summary>85. Schedules are not cached, but alerts other workstations if the schedules were changed</summary>
		Schedules,
		///<summary>86. This is used internally by OD, refreshes the computer / extension linker table.</summary>
		PhoneComps,
		///<summary>87. This is used internally by OD, refreshes the call center map.</summary>
		PhoneMap,
		///<summary>88.</summary>
		SmsPhones,
		///<summary>89.  Chat support through our website at http://opendental.com/contact.html.
		///Used to indicate a new session has been created, an existing session has been destroyed, or messages inside the session have changed.</summary>
		WebChatSessions,
		///<summary>90. Used for tracking refreshes on tabs 'for [User]', 'New for [User]', 'Main', 'Reminders'.</summary>
		TaskList,
		///<summary>91. Used for tracking refreshes on tab 'Open Tasks'.</summary>
		TaskAuthor,
		///<summary>92. Used for tracking refreshes on tab 'Patient Tasks'.</summary>
		TaskPatient,
		///<summary>93. Used for refreshing the Referral cache.</summary>
		Referral,
		///<summary>94. Used for refreshing "In Process" pseudo procedure statuses.</summary>
		ProcMultiVisits,
		///<summary>95. Used for refreshing the ProviderClinicLink cache.</summary>
		ProviderClinicLink,
		///<summary>96. Used for refreshing the KioskManager with eClipboard information.</summary>
		EClipboard,
		///<summary>97. Used for refreshing the TP module for a specific patient.</summary>
		TPModule,
		///<summary>98. Used for closing Cloud sessions. ActiveInstanceNum is the Fkey.</summary>
		ActiveInstance,
		///<summary>99. Used internally by OD HQ.</summary>
		PhoneEmpDefaults
	}
	//<summary></summary>
	/*public enum ButtonType{
		///<summary></summary>
		ButPush,
		///<summary></summary>
		Text}*/
	//DentalSpecialties are now a definition, user editable.
	/////<summary></summary>
	//public enum DentalSpecialty{
	//	///<summary>0</summary>
	//	General,
	//	///<summary>1</summary>
	//	Hygienist,
	//	///<summary>2</summary>
	//	Endodontics,
	//	///<summary>3</summary>
	//	Pediatric,
	//	///<summary>4</summary>
	//	Perio,
	//	///<summary>5</summary>
	//	Prosth,
	//	///<summary>6</summary>
	//	Ortho,
	//	///<summary>7</summary>
	//	Denturist,
	//	///<summary>8</summary>
	//	Surgery,
	//	///<summary>9</summary>
	//	Assistant,
	//	///<summary>10</summary>
	//	LabTech,
	//	///<summary>11</summary>
	//	Pathology,
	//	///<summary>12</summary>
	//	PublicHealth,
	//	///<summary>13</summary>
	//	Radiology
	//}
	///<summary>Appointment status.</summary>
	public enum ApptStatus{
		///<summary>0- No appointment should ever have this status.</summary>
		None,
		///<summary>1- Shows as a regularly scheduled appointment.</summary>
		Scheduled,
		///<summary>2- Shows greyed out.</summary>
		Complete,
		///<summary>3- Only shows on unscheduled list.</summary>
		UnschedList,
		///<summary>4- Deprecated in 17.4.1. Use Appointment.Priority instead. </summary>
		ASAP,
		///<summary>5- Shows with a big X on it.</summary>
		Broken,
		///<summary>6- Planned appointment.  Only shows in Chart module. User not allowed to change this status, and it does not display as one of the options.</summary>
		Planned,
		///<summary>7- Patient "post-it" note on the schedule. Shows light yellow. Shows on day scheduled just like appt, as well as in prog notes, etc.</summary>
		PtNote,
		///<summary>8- Patient "post-it" note completed</summary>
		PtNoteCompleted}

	///<summary></summary>
	public enum PatientStatus{
		///<summary>0</summary>
		Patient,
		///<summary>1</summary>
		[Description("Non-patient")]
		NonPatient,
		///<summary>2</summary>
		Inactive,
		///<summary>3</summary>
		Archived,
		///<summary>4</summary>
		Deleted,
		///<summary>5</summary>
		Deceased,
		///<summary>6- Not an actual patient yet.</summary>
		Prospective
	}
	///<summary>Known as administrativeGender (HL7 OID of 2.16.840.1.113883.5.1) Male=M, Female=F, Unknown=Undifferentiated=UN.</summary>
	public enum PatientGender{//known as administrativeGender HL7 OID of 2.16.840.1.113883.5.1
		///<summary>0</summary>
		Male,
		///<summary>1</summary>
		Female,
		///<summary>2- Required by HIPAA for privacy.  Required by ehr to track missing entries. EHR/HL7 known as undifferentiated (UN).</summary>
		Unknown}
	///<summary></summary>
	public enum PatientPosition{
		///<summary>0</summary>
		Single,
		///<summary>1</summary>
		Married,
		///<summary>2</summary>
		Child,
		///<summary>3</summary>
		Widowed,
		///<summary>4</summary>
		Divorced}
	///<summary>For schedule timeblocks.</summary>
	public enum ScheduleType{
		///<summary>0</summary>
		Practice,
		///<summary>1</summary>
		Provider,
		///<summary>2</summary>
		Blockout,
		///<summary>3</summary>
		Employee,
		///<summary>4 - A slot of time that an ASAP appointment can be moved up to.</summary>
		WebSchedASAP,
	}
	///<summary>For actions taken on blockouts (cut,copy,paste, etc.)</summary>
	public enum BlockoutAction {
		///<summary>0</summary>
		Cut,
		///<summary>1</summary>
		Copy,
		///<summary>2</summary>
		Paste,
		///<summary>3</summary>
		Delete,
		///<summary>4</summary>
		Create,
		///<summary>5</summary>
		Edit,
		///<summary>6</summary>
		Clear
	}
	//<summary>Used in the appointment edit window.</summary>
	/*public enum LabCaseOld{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		Sent,
		///<summary>2</summary>
		Received,
		///<summary>3</summary>
		QualityChecked};*/
	///<summary>Default sort method of the Procedure Code list.  0=Category, 1=ProcCode</summary>
	public enum ProcCodeListSort{
		///<summary>0</summary>
		Category,
		///<summary>1</summary>
		ProcCode
	}
	///<summary>Used in the other appointments window to keep track of the result when closing.</summary>
	public enum OtherResult{
		///<summary></summary>
		Cancel,
		///<summary></summary>
		CreateNew,
		///<summary></summary>
		GoTo,
		///<summary></summary>
		CopyToPinBoard,
		///<summary></summary>
		NewToPinBoard,
		///<summary>Currently only used when scheduling a recall.  Puts it on the pinboard, and then launches a search, jumping to a new date in the process.</summary>
		PinboardAndSearch
	}
	
	///<summary>Schedule status.  Open=0,Closed=1,Holiday=2.</summary>
  public enum SchedStatus{
		///<summary>0</summary>
		Open,
		///<summary>1</summary>
		Closed,
		///<summary>2</summary>
		Holiday}
	
	///<summary></summary>
  public enum AutoCondition{
		///<summary>0</summary>
		Anterior,
		///<summary>1</summary>
		Posterior,
		///<summary>2</summary>
		Premolar,
		///<summary>3</summary>
		Molar,
		///<summary>4</summary>
		One_Surf,
		///<summary>5</summary>
		Two_Surf,
		///<summary>6</summary>
		Three_Surf,
		///<summary>7</summary>
		Four_Surf,
		///<summary>8</summary>
		Five_Surf,
		///<summary>9</summary>
		First,
		///<summary>10</summary>
		EachAdditional,
		///<summary>11</summary>
		Maxillary,
		///<summary>12</summary>
		Mandibular,
		///<summary>13</summary>
		Primary,
		///<summary>14</summary>
		Permanent,
		///<summary>15</summary>
		Pontic,
		///<summary>16</summary>
		Retainer,
		///<summary>17</summary>
		AgeOver18}
	///<Summary>Used for insurance substitutions conditions of procedurecodes.  Mostly for posterior composites.</Summary>
	public enum SubstitutionCondition{
		///<Summary>0</Summary>
		Always,
		///<Summary>1</Summary>
		Molar,
		///<Summary>2</Summary>
		SecondMolar,
		///<Summary>3</Summary>
		Never,
		///<Summary>4</Summary>
		Posterior
	}
	///<summary>Claimproc Status.  The status must generally be the same as the claim, although it is sometimes not strictly enforced.</summary>
	public enum ClaimProcStatus{
		///<summary>0: For claims that have been created or sent, but have not been received.</summary>
		NotReceived,
		///<summary>1: For claims that have been received.</summary>
		Received,
		///<summary>2: For preauthorizations.</summary>
		Preauth,
		///<summary>3: The only place that this status is used is to make adjustments to benefits from the coverage window.  It is never attached to a claim.</summary>
		Adjustment,
		///<summary>4:This differs from Received only slightly.  It's for additional payments on procedures already received.  Most fields are blank.</summary>
		Supplemental,
		///<summary>5: CapClaim is used when you want to send a claim to a capitation insurance company.  These are similar to Supplemental in that there will always be a duplicate claimproc for a procedure. The first claimproc tracks the copay and writeoff, has a status of CapComplete, and is never attached to a claim. The second claimproc has status of CapClaim.</summary>
		CapClaim,
		///<summary>6: Estimates have replaced the fields that were in the procedure table.  Once a procedure is complete, the claimprocstatus will still be Estimate.  An Estimate can be attached to a claim and status gets changed to NotReceived.</summary>
		Estimate,
		///<summary>7: For capitation procedures that are complete.  This replaces the old procedurelog.CapCoPay field. This stores the copay and writeoff amounts.  The copay is only there for reference, while it is the writeoff that actually affects the balance. Never attached to a claim. If procedure is TP, then status will be CapEstimate.  Only set to CapComplete if procedure is Complete.</summary>
		CapComplete,
		///<summary>8: For capitation procedures that are still estimates rather than complete.  When procedure is completed, this can be changed to CapComplete, but never to anything else.</summary>
		CapEstimate,
		///<summary>9: For InsHist procedures.</summary>
		InsHist
	}

	///<summary></summary>
	public enum ToolBarsAvail{
		///<summary>0</summary>
		AccountModule,
		///<summary>1</summary>
		ApptModule,
		///<summary>2</summary>
		ChartModule,
		///<summary>3</summary>
		ImagesModule,
		///<summary>4</summary>
		FamilyModule,
		///<summary>5</summary>
		TreatmentPlanModule,
		///<summary>6</summary>
		ClaimsSend,
		///<summary>7 Shows in the toolbar at the top that is common to all modules.</summary>
		MainToolbar,
		///<summary>8 Shows in the main menu Reports submenu.</summary>
		ReportsMenu,
	}

	///<summary></summary>
	public enum TimeClockStatus{
		///<summary>0</summary>
		[Description("Home")]
		Home,
		///<summary>1</summary>
		[Description("Lunch")]
		Lunch,
		///<summary>2</summary>
		[Description("Break")]
		Break
	}

	///<summary>In perio, the type of measurements for a given row.</summary>
	public enum PerioSequenceType{
		///<summary>0</summary>
		Mobility,
		///<summary>1</summary>
		Furcation,
		///<summary>2-AKA recession.</summary>
		GingMargin,
		///<summary>3-MucoGingivalJunction- the division between attached and unattached mucosa.</summary>
		MGJ,
		///<summary>4</summary>
		Probing,
		///<summary>5-For the skiptooth type, set surf to none, and ToothValue to 1.</summary>
		SkipTooth,
		///<summary>6. Sum of flags for bleeding(1), suppuration(2), plaque(4), and calculus(8).</summary>
		Bleeding,
		///<summary>7. But this type is never saved to the db. It is always calculated on the fly.</summary>
		CAL
	}

	///<summary>Deprecated, use patientrace table instead.  Temporarily used for converting old patient races to patientrace entries and screening.  Race and ethnicity for patient. Used by public health.  The problem is that everyone seems to want different choices.  If we give these choices their own table, then we also need to include mapping functions.  These are currently used in ArizonaReports, HL7 w ECW, and EHR.  Foreign users would like their own mappings.</summary>
	public enum PatientRaceOld {
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		Multiracial,
		///<summary>2</summary>
		HispanicLatino,
		///<summary>3</summary>
		AfricanAmerican,
		///<summary>4</summary>
		White,
		///<summary>5</summary>
		HawaiiOrPacIsland,
		///<summary>6</summary>
		AmericanIndian,
		///<summary>7</summary>
		Asian,
		///<summary>8</summary>
		Other,
		///<summary>9</summary>
		Aboriginal,
		///<summary>10 - Required by EHR.</summary>
		BlackHispanic
	}

	///<summary>Grade level used in public health.</summary>
	public enum PatientGrade{
		///<summary>0</summary>
		Unknown,
		///<summary>1</summary>
		First,
		///<summary>2</summary>
		Second,
		///<summary>3</summary>
		Third,
		///<summary>4</summary>
		Fourth,
		///<summary>5</summary>
		Fifth,
		///<summary>6</summary>
		Sixth,
		///<summary>7</summary>
		Seventh,
		///<summary>8</summary>
		Eighth,
		///<summary>9</summary>
		Ninth,
		///<summary>10</summary>
		Tenth,
		///<summary>11</summary>
		Eleventh,
		///<summary>12</summary>
		Twelfth,
		///<summary>13</summary>
		PrenatalWIC,
		///<summary>14</summary>
		PreK,
		///<summary>15</summary>
		Kindergarten,
		///<summary>16</summary>
		Other
	}

	///<summary>For public health.  Unknown, NoProblems, NeedsCarE, or Urgent.</summary>
	public enum TreatmentUrgency{
		///<summary></summary>
		Unknown,
		///<summary></summary>
		NoProblems,
		///<summary></summary>
		NeedsCare,
		///<summary></summary>
		Urgent
	}

	///<summary>The type of image for images module.</summary>
	public enum ImageType{
		///<summary>0- Includes scanned documents and screenshots.</summary>
		Document,
		///<summary>1</summary>
		Radiograph,
		///<summary>2</summary>
		Photo,
		///<summary>3- For instance a Word document or a spreadsheet. Not an image.</summary>
		File,
		///<summary>4- Used for Claim Attachments. Preserves original resolution.</summary>
		Attachment
	}

	///<summary>Used by QuickPasteCat to determine which category to default to when opening.</summary>
	public enum QuickPasteType {
		///<summary>0 - None should never be used.  It is simply used as a "default" when adding a new control.  Searching for usage of "None" is an easy way to find spots where our pattern was not followed correctly.</summary>
		None,
		///<summary>1</summary>
		Procedure,
		///<summary>2</summary>
		Appointment,
		///<summary>3</summary>
		CommLog,
		///<summary>4</summary>
		Adjustment,
		///<summary>5</summary>
		Claim,
		///<summary>6</summary>
		Email,
		///<summary>7</summary>
		InsPlan,
		///<summary>8</summary>
		Letter,
		///<summary>9</summary>
		MedicalSummary,
		///<summary>10</summary>
		ServiceNotes,
		///<summary>11</summary>
		MedicalHistory,
		///<summary>12</summary>
		MedicationEdit,
		///<summary>13</summary>
		MedicationPat,
		///<summary>14</summary>
		PatAddressNote,
		///<summary>15</summary>
		Payment,
		///<summary>16</summary>
		PayPlan,
		///<summary>17</summary>
		Query,
		///<summary>18</summary>
		Referral,
		///<summary>19</summary>
		Rx,
		///<summary>20</summary>
		FinancialNotes,
		///<summary>21</summary>
		ChartTreatment,
		///<summary>22</summary>
		MedicalUrgent,
		///<summary>23</summary>
		Statement,
		///<summary>24</summary>
		Recall,
		///<summary>25</summary>
		Popup,
		///<summary>26</summary>
		TxtMsg,
		///<summary>27</summary>
		Task,
		///<summary>28</summary>
		Schedule,
		///<summary>29</summary>
		TreatPlan,
		///<summary>30</summary>
		ClaimCustomTrack,
		///<summary>31</summary>
		AutoNote,
		///<summary>32</summary>
		JobManager,
		///<summary>33 - Only to be used if the ReadOnly property is set to true.</summary>
		ReadOnly,
		///<summary>34</summary>
		Lab,
		///<summary>35</summary>
		Equipment,
		///<summary>36</summary>
		FilePath,
		///<summary>37</summary>
		ContactInfo,
		///<summary>38</summary>
		Office,
		///<summary>39</summary>
		ProgramLink,
		///<summary>40</summary>
		EmployeeStatus,
		///<summary>41</summary>
		WebChat,
		///<summary>42</summary>
		FAQ,
	}

	///<summary>For every type of electronic claim format that Open Dental can create, there will be an item in this enumeration.  All e-claim formats are hard coded due to complexity.</summary>
	public enum ElectronicClaimFormat{
		///<summary>0-Not in database, but used in various places in program.</summary>
		None,
		///<summary>1-The American standard through 12/31/11.</summary>
		x837D_4010,
		///<summary>2-Proprietary format for Renaissance.</summary>
		Renaissance,
		///<summary>3-CDAnet format version 4.</summary>
		Canadian,
		///<summary>4-CSV file adaptable for use in Netherlands.</summary>
		Dutch,
		///<summary>5-The American standard starting on 1/1/12.</summary>
		x837D_5010_dental,
		///<summary>6-Either professional or medical.  The distiction is stored at the claim level.</summary>
		x837_5010_med_inst,
		///<summary>7-A specific Canadian carrier located in Quebec which has their own format.</summary>
		Ramq,
	}

	///<summary>Used when submitting e-claims to some carriers who require extra provider identifiers.  Usage varies by company.  Only used as needed.  SiteNumber is the only one that is still used on 5010s.  The other 3 have been deprecated and replaced by NPI.</summary>
	public enum ProviderSupplementalID{
		///<summary>0</summary>
		BlueCross,
		///<summary>1</summary>
		BlueShield,
		///<summary>2</summary>
		SiteNumber,
		///<summary>3</summary>
		CommercialNumber
	}

	///<summary>Each clearinghouse can have a hard-coded comm bridge which handles all the communications of transfering the claim files to the clearinghouse/carrier.  Does not just include X12, but can include any format at all.</summary>
	public enum EclaimsCommBridge{
		///<summary>0-No comm bridge will be activated. The claim files will be created to the specified path, but they will not be uploaded.</summary>
		None,
		///<summary>1</summary>
		WebMD,
		///<summary>2</summary>
		BCBSGA,
		///<summary>3</summary>
		Renaissance,
		///<summary>4</summary>
		ClaimConnect,
		///<summary>5</summary>
		RECS,
		///<summary>6</summary>
		Inmediata,
		///<summary>7</summary>
		AOS,
		///<summary>8</summary>
		PostnTrack,
		///<summary>9 Canadian clearinghouse.</summary>
		ITRANS,
		///<summary>10</summary>
		Tesia,
		///<summary>11</summary>
		MercuryDE,
		///<summary>12</summary>
		ClaimX,
		///<summary>13</summary>
		DentiCal,
		///<summary>14</summary>
		EmdeonMedical,
		///<summary>15 Canadian clearinghouse.</summary>
		Claimstream,
		///<summary>16 UK clearinghouse.</summary>
		NHS,
		///<summary>17</summary>
		EDS,
		///<summary>18</summary>
		Ramq,
		///<summary>19</summary>
		EdsMedical,
	}

	///<summary>Used as the enumeration of FieldValueType.ForeignKey.  Basically, this allows lists to be included in the parameter list.  The lists are those common short lists that are used so frequently.  The user can only select one from the list, and the primary key of that item will be used as the parameter.</summary>
	public enum ReportFKType{
		///<summary>0</summary>
		None,
		///<summary>The schoolclass table in the database. Used for dental schools.</summary>
		SchoolClass,
		///<summary>The schoolcourse table in the database. Used for dental schools.</summary>
		SchoolCourse
	}

	///<summary>The type of signal being sent.</summary>
	public enum SignalType{
		///<summary>0- Includes text messages.</summary>
		Button,
		///<summary>1</summary>
		Invalid
	}

	///<summary>Used in the benefit table.  Corresponds to X12 EB01.</summary>
	public enum InsBenefitType{
		///<summary>0- Not usually used.  Would only be used if you are just indicating that the patient is covered, but without any specifics.</summary>
		ActiveCoverage,
		///<summary>1- Used for percentages to indicate portion that insurance will cover.  When interpreting electronic benefit information, this is the opposite percentage, the percentage that the patient will pay after deductible.</summary>
		CoInsurance,
		///<summary>2- The deductible amount.  Might be two entries if, for instance, deductible is waived on preventive.</summary>
		Deductible,
		///<summary>3- A dollar amount.</summary>
		CoPayment,
		///<summary>4- Services that are simply not covered at all.</summary>
		Exclusions,
		///<summary>5- Covers a variety of limitations, including Max, frequency, fee reductions, etc.</summary>
		Limitations,
		///<summary>6- Sets a period of time after the effective date where a benefit will not be used.</summary>
		WaitingPeriod
	}

	///<summary>Used in the benefit table.  Corresponds to X12 EB06.</summary>
	public enum BenefitTimePeriod{
		///<summary>0- A timeperiod is frequenly not needed.  For example, percentages.</summary>
		None,
		///<summary>1- The renewal month is not Jan.  In this case, we need to know the effective date so that we know which month the benefits start over in.</summary>
		ServiceYear,
		///<summary>2- Renewal month is Jan.</summary>
		CalendarYear,
		///<summary>3- Usually used for ortho max.</summary>
		Lifetime,
		///<summary>4- Wouldn't be used alone.  Years would again be specified in the quantity field along with a number.</summary>
		Years,
		///<summary>5- # in last 12 months.  Does not care about when benefit year begins. Looks at previous 12 months.</summary>
		NumberInLast12Months,
	}

	///<summary>Used in the benefit table in conjunction with an integer quantity.</summary>
	public enum BenefitQuantity{
		///<summary>0- This is used a lot. Most benefits do not need any sort of quantity.</summary>
		None,
		///<summary>1- For example, two exams per year</summary>
		NumberOfServices,
		///<summary>2- For example, 18 when flouride only covered to 18 y.o.</summary>
		AgeLimit,
		///<summary>3- For example, copay per 1 visit.</summary>
		Visits,
		///<summary>4- For example, pano every 5 years.</summary>
		Years,
		///<summary>5- For example, BWs every 6 months.</summary>
		Months
	}

	///<summary>Used in the benefit table.</summary>
	public enum BenefitCoverageLevel{
		///<summary>0- Since this is a situational X12 field, we can also have none.  Typical for percentages and copayments.</summary>
		None,
		///<summary>1- The default for deductibles and maximums.</summary>
		Individual,
		///<summary>2- For example, family deductible or family maximum.</summary>
		Family
		
	}
	

	///<summary>The X12 benefit categories.  Used to link the user-defined CovCats to the corresponding X12 category.</summary>
	public enum EbenefitCategory{
		///<summary>0- Default.  Applies to all codes.</summary>
		None,
		///<summary>1- X12: 30 and 35. All ADA codes except ortho.  D0000-D7999 and D9000-D9999</summary>
		General,
		///<summary>2- X12: 23. ADA D0000-D0999.  This includes DiagnosticXray.</summary>
		Diagnostic,
		///<summary>3- X12: 24. ADA D4000</summary>
		Periodontics,
		///<summary>4- X12: 25. ADA D2000-D2699, and D2800-D2999.</summary>
		Restorative,
		///<summary>5- X12: 26. ADA D3000</summary>
		Endodontics,
		///<summary>6- X12: 27. ADA D5900-D5999</summary>
		MaxillofacialProsth,
		///<summary>7- X12: 36. Exclusive subcategory of restorative.  D2700-D2799</summary>
		Crowns,
		///<summary>8- X12: 37. ADA range?</summary>
		Accident,
		///<summary>9- X12: 38. ADA D8000-D8999</summary>
		Orthodontics,
		///<summary>10- X12: 39. ADA D5000-D5899 (removable), and D6200-D6899 (fixed)</summary>
		Prosthodontics,
		///<summary>11- X12: 40. ADA D7000</summary>
		OralSurgery,
		///<summary>12- X12: 41. ADA D1000</summary>
		RoutinePreventive,
		///<summary>13- X12: 4. ADA D0200-D0399.  So this is like an optional category which is otherwise considered to be diagnosic.</summary>
		DiagnosticXRay,
		///<summary>14- X12: 28. ADA D9000-D9999</summary>
		Adjunctive
	}

	///<summary>Used in accounting for chart of accounts.</summary>
	public enum AccountType{
		///<summary>0</summary>
		Asset,
		///<summary>1</summary>
		Liability,
		///<summary>2</summary>
		Equity,
		///<summary>3</summary>
		Income,
		///<summary>4</summary>
		Expense
	}

	///<summary></summary>
	public enum ToothPaintingType{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		Extraction,
		///<summary>2</summary>
		Implant,
		///<summary>3</summary>
		RCT,
		///<summary>4</summary>
		PostBU,
		///<summary>5</summary>
		FillingDark,
		///<summary>6</summary>
		FillingLight,
		///<summary>7</summary>
		CrownDark,
		///<summary>8</summary>
		CrownLight,
		///<summary>9</summary>
		BridgeDark,
		///<summary>10</summary>
		BridgeLight,
		///<summary>11</summary>
		DentureDark,
		///<summary>12</summary>
		DentureLight,
		///<summary>13</summary>
		Sealant,
		///<summary>14</summary>
		Veneer,
		///<summary>15-Text was previously called Watch</summary>
		Text,
		///<summary>16</summary>
		RetainedRoot,
		///<summary>17</summary>
		SpaceMaintainer
	}
	
	///<summary>Indicates at what point the patient is in the sequence. 0=standby, 1=PatientInfo, 2=Medical, 3=UpdateOnly.</summary>
	public enum TerminalStatusEnum{
		///<summary>0</summary>
		Standby,
		///<summary>1</summary>
		PatientInfo,
		///<summary>2</summary>
		Medical,
		///<summary>3. Only the patient info tab will be visible.  This is just to let patient up date their address and phone number.</summary>
		UpdateOnly
	}

	///<summary>0=FreeformText, 1=YesNoUnknown. Allows for later adding other types, 3=picklist, 4, etc</summary>
	public enum QuestionType{
		///<summary>0</summary>
		FreeformText,
		///<summary>1</summary>
		YesNoUnknown
	}

	///<summary>0=User,1=Extra,2=Message.</summary>
	public enum SignalElementType{
		///<summary>0-To and From lists.  Not tied in any way to the users that are part of security.</summary>
		User,
		///<summary>Typically used to insert "family" before "phone" signals.</summary>
		Extra,
		///<summary>Elements of this type show in the last column and trigger the message to be sent.</summary>
		Message
	}

	///<summary></summary>
	public enum InsFilingCodeOldOld{
		///<summary>0</summary>
		Commercial_Insurance,
		///<summary>1</summary>
		SelfPay,
		///<summary>2</summary>
		OtherNonFed,
		///<summary>3</summary>
		PPO,
		///<summary>4</summary>
		POS,
		///<summary>5</summary>
		EPO,
		///<summary>6</summary>
		Indemnity,
		///<summary>7</summary>
		HMO_MedicareRisk,
		///<summary>8</summary>
		DMO,
		///<summary>9</summary>
		BCBS,
		///<summary>10</summary>
		Champus,
		///<summary>11</summary>
		Disability,
		///<summary>12</summary>
		FEP,
		///<summary>13</summary>
		HMO,
		///<summary>14</summary>
		LiabilityMedical,
		///<summary>15</summary>
		MedicarePartB,
		///<summary>16</summary>
		Medicaid,
		///<summary>17</summary>
		ManagedCare_NonHMO,
		///<summary>18</summary>
		OtherFederalProgram,
		///<summary>19</summary>
		SelfAdministered,
		///<summary>20</summary>
		Veterans,
		///<summary>21</summary>
		WorkersComp,
		///<summary>22</summary>
		MutuallyDefined
	}

	///<summary></summary>
	public enum ContactMethod{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		DoNotCall,
		///<summary>2</summary>
		HmPhone,
		///<summary>3</summary>
		WkPhone,
		///<summary>4</summary>
		WirelessPh,
		///<summary>5</summary>
		Email,
		///<summary>6</summary>
		SeeNotes,
		///<summary>7</summary>
		Mail,
		///<summary>8</summary>
		TextMessage
	}

	///<summary>0=None,1=Declined,2=Scheduled,3=Consulted,4=InTreatment,5=Complete</summary>
	public enum ReferralToStatus{
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		Declined,
		///<summary>2</summary>
		Scheduled,
		///<summary>3</summary>
		Consulted,
		///<summary>4</summary>
		InTreatment,
		///<summary>5</summary>
		Complete
	}

	///<summary></summary>
	public enum StatementMode{
		///<summary>0</summary>
		Mail,
		///<summary>1</summary>
		InPerson,
		///<summary>2</summary>
		Email,
		///<summary>3</summary>
		Electronic
	}

	///<summary></summary>
	public enum DeletedObjectType{
		///<summary>0</summary>
		Appointment,
		///<summary>1 - A schedule object.  Only provider schedules are tracked for deletion.</summary>
		ScheduleProv,
		///<summary>2 - When a recall row is deleted, this records the PatNum for which it was deleted.</summary>
		RecallPatNum,
		///<summary>Deprecated</summary>
		RxPat,
		///<summary>Deprecated</summary>
		LabPanel,
		///<summary>Deprecated</summary>
		LabResult,
		///<summary>Deprecated</summary>
		DrugUnit,
		///<summary>Deprecated</summary>
		Medication,
		///<summary>Deprecated</summary>
		MedicationPat,
		///<summary>Deprecated</summary>
		Allergy,
		///<summary>Deprecated</summary>
		AllergyDef,
		///<summary>Deprecated</summary>
		Disease,
		///<summary>Deprecated</summary>
		DiseaseDef,
		///<summary>Deprecated</summary>
		ICD9,
		///<summary>Deprecated</summary>
		Provider,
		///<summary>Deprecated</summary>
		Pharmacy,
		///<summary>Deprecated</summary>
		Statement,
		///<summary>Deprecated</summary>
		Document,
		///<summary>Deprecated</summary>
		Recall
	}

	/////<summary>0=UnknownIfEver,1=SmokerUnkownCurrent,2=NeverSmoked,3=FormerSmoker,4=CurrentSomeDay,5=CurrentEveryDay</summary>
	//public enum SmokingStatus {
	//  ///<summary>0</summary>
	//  UnknownIfEver_Recode9,
	//  ///<summary>1</summary>
	//  SmokerUnknownCurrent_Recode5,
	//  ///<summary>2</summary>
	//  NeverSmoked_Recode4,
	//  ///<summary>3</summary>
	//  FormerSmoker_Recode3,
	//  ///<summary>4</summary>
	//  CurrentSomeDay_Recode2,
	//  ///<summary>5</summary>
	//  CurrentEveryDay_Recode1//,//implement when we get answer from proctor
	//  /////<summary>6</summary>
	//  //LightSmoker,
	//  /////<summary>7</summary>
	//  //HeavySmoker
	//}

	///<summary></summary>
	public enum SmokingSnoMed {
		///<summary>0 - UnknownIfEver</summary>
		_266927001,
		///<summary>1 - SmokerUnknownCurrent</summary>
		_77176002,
		///<summary>2 - NeverSmoked</summary>
		_266919005,
		///<summary>3 - FormerSmoker</summary>
		_8517006,
		///<summary>4 - CurrentSomeDay</summary>
		_428041000124106,
		///<summary>5 - CurrentEveryDay</summary>
		_449868002,
		///<summary>6 - LightSmoker</summary>
		_428061000124105,
		///<summary>7 - HeavySmoker</summary>
		_428071000124103
	}

	///<summary>0=Active, 1=Resolved, 2=Inactive</summary>
	public enum ProblemStatus{
		/// <summary>0</summary>
		Active,
		/// <summary>1</summary>
		Resolved,
		/// <summary>2</summary>
		Inactive
	}

	///<summary>Only applies to 15.4 and following.  This defines what and how the eConnector is running for a customer. Do not re-order and do not rename any values.</summary>
	public enum ListenerServiceType {
		///<summary>0.  Default for people who had been using the listener prior to the 15.3 proxy listener.</summary>
		ListenerService,
		///<summary>1.  Opt-in required to use the proxy service.</summary>
		ListenerServiceProxy,
		///<summary>2.  Customer is off by HQ's choice. This can only be undone by HQ.</summary>
		DisabledByHQ,
		///<summary>3.  Customer listener is off and awaiting customer consent.</summary>
		NoListener
	}

	///<summary>Defines a user-friendly way of describing sorting strategies.  Intended for user selection for sorting grids.  Can easily be added to.</summary>
	public enum SortStrategy {
		///<summary>0.</summary>
		[Description("Name Asc")]
		NameAsc,
		///<summary>1.</summary>
		[Description("Name Desc")]
		NameDesc,
		///<summary>2.</summary>
		[Description("PatNum Asc")]
		PatNumAsc,
		///<summary>3.</summary>
		[Description("PatNum Desc")]
		PatNumDesc
	}
	
	///<summary>The Enumeration value for which Claim Snapshot Trigger that will be stored.</summary>
	public enum ClaimSnapshotTrigger {
		///<summary>0</summary>
		[Description("Claim Created")]
		ClaimCreate,
		///<summary>1</summary>
		[Description("Service - Specific Time")]
		Service,
		///<summary>2</summary>
		[Description("Insurance Payment Received")]
		InsPayment
	}

	///<summary>A permission that an API or FHIR APIKey possesses. The CRUD suffix (Create, Read, Update, Delete) matters here.  Rows may be filtered by WebServiceHQ.CheckFHIRAPIKey() according to their suffix.  DO NOT add or delete permissions from the middle of this list because this is a db field in table FHIRAPIKeyPermission.  New permissions must always go at the end.</summary>
	public enum APIPermission {
		AppointmentCreate,
		AppointmentRead,
		AppointmentUpdate,
		///<summary>Even though this enum value isn't used, we can't delete it because it's being used in Java and Java can't assign int values to an enum.</summary>
		AppointmentDelete,
		PatientCreate,
		PatientRead,
		PatientUpdate,
		//No PatientDelete
		Subscriptions,
		LocationRead,
		OrganizationRead,
		PractitionerRead,
		ScheduleRead,//Also used for Slot resources
		CapabilityStatement,
		AllergyIntoleranceRead,
		MedicationRead,//Used for both Medication and MedicationStatement resources
		ConditionRead,
		ServiceRequestRead,
		ServiceRequestCreate,
		ProcedureRead,
		ProcedureCreate,
		ProcedureUpdate,
		PaymentRead,
		PaymentCreate,
		CommunicationRead,
		CommunicationCreate,
		//MediaCreate,
		//MediaRead,
		///<summary>For non-FHIR, this includes all read permissions.</summary>
		ApiReadAll,
		///<summary>For non-FHIR, this covers all permissions that are not separately listed.  Can be extensive.  Allergies, medications, Rx, Procedures, Provider, schedules, etc.</summary>
		ApiAllOthers,
		///<summary>For non-FHIR, includes commlog entries, setting confirm status on appts, popups</summary>
		ApiComm,
		///<summary>For non-FHIR, allows uploading images and PDFs. This is separate because these uploads might consume a lot of bandwidth.</summary>
		ApiDocuments,
		///<summary>For non-FHIR, run any read-only query.  Returns max 1000 rows at a time, so the query itself must include pagination using sql limit.</summary>
		ApiQueries,
		///<summary>For non-FHIR, includes edit or create appointments</summary>
		ApiAppointments,
		///<summary>For non-FHIR, POST and PUT for claimprocs (insAdj), inssubs, and patplans (this is very complex and not intended for most users)</summary>
		ApiInsurance,
		///<summary>For non-FHIR, includes edit or create patients.</summary>
		ApiPatients
	}

	///<summary>Will be deprecated soon. FHIRKeyStatus has mostly replaced this.</summary>
	public enum APIKeyStatus {
		///<summary>Able to perform read operations. By default, all keys have this status.</summary>
		ReadEnabled,
		///<summary>For an API key to have write permissions, it must first pay ODHQ a fee. This key can still perform read operations.</summary>
		WritePending,
		///<summary>Able to perform read and write operations.</summary>
		WriteEnabled,
		///<summary>ODHQ has purposely turned off write permissions for this key. This key can still perform read operations.</summary>
		WriteDisabled,
		///<summary>Read and write operations disabled.</summary>
		Disabled
	}

	public enum FHIRKeyStatus {
		///<summary>Nobody knows!</summary>
		Unknown,
		///<summary>This key can be used for queries.</summary>
		Enabled,
		///<summary>The OD customer has disabled this key.</summary>
		[Description("Disabled by Customer")]
		DisabledByCustomer,
		///<summary>The 3rd party developer has disabled this key.</summary>
		[Description("Disabled by Developer")]
		DisabledByDeveloper,
		///<summary>Open Dental HQ has disabled this key.</summary>
		[Description("Disabled by Open Dental")]
		DisabledByHQ,
		///<summary>The key is authorized for read only transactions. For backwards compatability.</summary>
		[Description("Read Enabled")]
		EnabledReadOnly,
	}

	///<summary></summary>
	public enum OAuthApplicationNames {
		///<summary>0</summary>
		Dropbox,
		///<summary>1</summary>
		Google,
		///<summary>2</summary>
		QuickBooksOnline,
	}

	///<summary></summary>
	public enum DataStorageType {
		///<summary>0</summary>
		InDatabase,
		///<summary>1</summary>
		LocalAtoZ,
		///<summary>2</summary>
		DropboxAtoZ,
		///<summary>3</summary>
		SftpAtoZ
	}

	///<summary>Only used by the Signup Portal</summary>
	public enum SignupPortalPermission {
		///<summary>This user is denied access to the signup portal.</summary>
		Denied,
		///<summary>The user is only able to sign up any clinic for any eService</summary>
		FullPermission,
		///<summary>The user is only able to see what clinics are signed up for eServices</summary>
		ReadOnly,
		///<summary>The user is only able to sign up their restricted clinic for any eService</summary>
		ClinicOnly,
		///<summary>The user is only able to see what eServices their restricted clinic is signed up for.</summary>
		ClinicOnlyReadOnly,
		///<summary>The user is viewing the Signup Portal from HQ.</summary>
		FromHQ,
		///<summary>Request is coming from the reseller portal.</summary>
		ResellerPortal,
		///<summary>The user is only able to make changes to the SMS Monthly Warning Amount.</summary>
		LimitedSmsOnly,
	}

	/// <summary></summary>
	public enum ClaimProcCreditsGreaterThanProcFee {
		///<summary>0</summary>
		Allow,
		///<summary>1</summary>
		Warn,
		///<summary>2</summary>
		Block,
	}

	/// <summary>Determines behavior when an appointment is scheduled in an operatory assigned to a clinic for a patient with a specialty that does not
	/// exist in the list of that clinic's specialties.</summary>
	public enum ApptSchedEnforceSpecialty {
		///<summary>0</summary>
		[Description("Don't Enforce")]
		DontEnforce,
		///<summary>1</summary>
		[Description("Warn")]
		Warn,
		///<summary>2</summary>
		[Description("Block")]
		Block,
	}

	///<summary>String values that are stored for blockout defintions.</summary>
	public enum BlockoutType {
		///<summary>Do not schedule an appointment over this blockout.</summary>
		[Description("NS")]
		NoSchedule,
		///<summary>Do not allow this blockout to be cut or copied and do not allow another blockout to be pasted on this blockout.</summary>
		[Description("DC")]
		DontCopy,
	}

	///<summary>Scheduling priority used by Appointments.</summary>
	public enum ApptPriority {
		///<summary>0 - Default priority</summary>
		Normal,
		///<summary>1 - Used to identify items for the ASAP list</summary>
		ASAP
	}
	
	///<summary>Scheduling priority used by Recalls.</summary>
	public enum RecallPriority {
		///<summary>0 - Default priority</summary>
		Normal,
		///<summary>1 - Used to identify items for the ASAP list</summary>
		ASAP
	}

	public enum ProcessingMethod {
		///<summary>PayConnect will use the web service to process payments.</summary>
		WebService,
		///<summary>PayConnect will use the terminal to process payments.</summary>
		Terminal,
	}

	public enum WebSchedVerifyType {
		None,
		Text,
		Email,
		TextAndEmail
	}

	///<summary>Used by the OpenDentalService to determine how often a thread should perform a task.  Example:  For Transworld, account activity is sent
	///to the SFTP server at a user specified frequency. Default once per day.  The user can specify a repeat in Days, Hours, or Minutes.</summary>
	public enum FrequencyUnit {
		///<summary>0 - Default frequency is repeating once per day (1 Days).</summary>
		Days,
		///<summary>1</summary>
		Hours,
		///<summary>2</summary>
		Minutes
	}

	///<summary>Used to determine how to handle creating claims with $0 procedures.</summary>
	public enum ClaimZeroDollarProcBehavior {
		///<summary>0.  Default value for the ClaimZeroDollarProcBehavior preference.  Allows all procedures to be attached to a claim</summary>
		Allow,
		///<summary>1.  Prompts the user to confirm attaching the $0 procedures to claims.</summary>
		Warn,
		///<summary>2.  Always block users from creating a claim for $0 procedures.</summary>
		Block,
	}

	///<summary>Differentiate between transaction types.</summary>
	public enum AccountEntryType {
		///<summary>0 - adjustment.AdjNum.  Can be a positive (Debit) or negative (Credit) adjustment to the amount owed.</summary>
		Adjustment = 0,
		///<summary>1 - claimproc.ClaimProcNum. For ins payments and/or writeoffs entered.</summary>
		ClaimPayment,
		///<summary>2 - paysplit.SplitNum.  Patient payment on an account.</summary>
		Payment,
		///<summary>3 - procedurelog.ProcNum.  Positive (debit, increases the amount owed).</summary>
		Procedure,
		///<summary> 4 - claim</summary>
		Claim
	}

	///<summary></summary>
	public enum SupplementalBackupStatuses {
		///<summary>0</summary>
		Disabled,
		///<summary>1</summary>
		Enabled,
		///<summary>2</summary>
		DisabledByHQ,
	}

	///<summary>Bit wise</summary>
	[Flags]
	public enum HostedEmailStatus {
		///<summary>0.</summary>
		[Description("NotActivated")]
		NotActivated=0,
		///<summary>1. The absence of this flag prevents Enabled flag from having any effect.</summary>
		[Description("Signed Up")]
		SignedUp=1,
		///<summary>2. The absense of this flag is equivalent to Disabled.</summary>
		[Description("Enabled")]
		Enabled=2,
	}

	///<summary>Contains exit codes used for FormOpenDental. Not all codes used are listed here. If using a new exit code, you should add it to this
	///enum.</summary>
	public enum FormOpenDentalExitCodes {
		///<summary>The database version is higher than the version of the current program.</summary>
		DbVersionHigherThanCurrent=309,
	}

	///<summary>Contains the type used in FormEServicesWebSchedNotify.</summary>
	public enum WebSchedNotifyType {
		///<summary>0</summary>
		[Description("ASAP")]
		ASAP,
		///<summary>1</summary>
		[Description("NewPat")]
		NewPat,
		///<summary>2</summary>
		[Description("Recall")]
		Recall,
		///<summary>3</summary>
		[Description("ExistingPat")]
		ExistingPat,
	}

	[Flags]
	public enum BugFlag {
		///<summary>0 - None=0</summary>
		None=0,
		///<summary>1 - Open Dental=1</summary>
		[Description("Open Dental")]
		OD=1,
		///<summary>2 - Mobile=2</summary>
		[Description("M")]
		Mobile=2,
	}

	public enum FromASAPShowAppointment {
		///<summary>0</summary>
		[Description("All Appointments")]
		All,
		///<summary>1</summary>
		[Description("Non-Hygiene Appointments")]
		NonHygiene,
		///<summary>2</summary>
		[Description("Hygiene Appointments")]
		Hygiene
	}
}





