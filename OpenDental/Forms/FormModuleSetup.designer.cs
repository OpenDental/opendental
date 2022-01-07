using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;

namespace OpenDental{
	partial class FormModuleSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModuleSetup));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkIsAlertRadiologyProcsEnabled = new System.Windows.Forms.CheckBox();
			this.checkImagesModuleTreeIsCollapsed = new System.Windows.Forms.CheckBox();
			this.checkApptsCheckFrequency = new System.Windows.Forms.CheckBox();
			this._colorDialog = new System.Windows.Forms.ColorDialog();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabAppts = new System.Windows.Forms.TabPage();
			this.checkBrokenApptRequiredOnMove = new System.Windows.Forms.CheckBox();
			this.groupBox8 = new OpenDental.UI.GroupBoxOD();
			this.checkAppointmentBubblesDisabled = new System.Windows.Forms.CheckBox();
			this.textApptProvbarWidth = new OpenDental.ValidNum();
			this.checkSolidBlockouts = new System.Windows.Forms.CheckBox();
			this.label58 = new System.Windows.Forms.Label();
			this.checkApptExclamation = new System.Windows.Forms.CheckBox();
			this.textApptFontSize = new System.Windows.Forms.TextBox();
			this.butApptLineColor = new System.Windows.Forms.Button();
			this.checkApptBubbleDelay = new System.Windows.Forms.CheckBox();
			this.butColor = new System.Windows.Forms.Button();
			this.label23 = new System.Windows.Forms.Label();
			this.comboDelay = new OpenDental.UI.ComboBoxOD();
			this.label25 = new System.Windows.Forms.Label();
			this.checkApptModuleDefaultToWeek = new System.Windows.Forms.CheckBox();
			this.checkApptRefreshEveryMinute = new System.Windows.Forms.CheckBox();
			this.apptClickDelay = new System.Windows.Forms.Label();
			this.label54 = new System.Windows.Forms.Label();
			this.checkApptsAllowOverlap = new System.Windows.Forms.CheckBox();
			this.checkPreventChangesToComplAppts = new System.Windows.Forms.CheckBox();
			this.textApptAutoRefreshRange = new OpenDental.ValidNum();
			this.labelApptAutoRefreshRange = new System.Windows.Forms.Label();
			this.checkUnscheduledListNoRecalls = new System.Windows.Forms.CheckBox();
			this.checkReplaceBlockouts = new System.Windows.Forms.CheckBox();
			this.labelApptSchedEnforceSpecialty = new System.Windows.Forms.Label();
			this.comboApptSchedEnforceSpecialty = new System.Windows.Forms.ComboBox();
			this.textApptWithoutProcsDefaultLength = new OpenDental.ValidNum();
			this.labelApptWithoutProcsDefaultLength = new System.Windows.Forms.Label();
			this.checkApptAllowEmptyComplete = new System.Windows.Forms.CheckBox();
			this.checkApptAllowFutureComplete = new System.Windows.Forms.CheckBox();
			this.comboTimeArrived = new OpenDental.UI.ComboBoxOD();
			this.checkApptsRequireProcs = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkApptModuleProductionUsesOps = new System.Windows.Forms.CheckBox();
			this.comboTimeSeated = new OpenDental.UI.ComboBoxOD();
			this.checkUseOpHygProv = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkApptModuleAdjInProd = new System.Windows.Forms.CheckBox();
			this.comboTimeDismissed = new OpenDental.UI.ComboBoxOD();
			this.checkApptTimeReset = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox2 = new OpenDental.UI.GroupBoxOD();
			this.label37 = new System.Windows.Forms.Label();
			this.comboBrokenApptProc = new System.Windows.Forms.ComboBox();
			this.checkBrokenApptCommLog = new System.Windows.Forms.CheckBox();
			this.checkBrokenApptAdjustment = new System.Windows.Forms.CheckBox();
			this.comboBrokenApptAdjType = new OpenDental.UI.ComboBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.textWaitRoomWarn = new System.Windows.Forms.TextBox();
			this.checkAppointmentTimeIsLocked = new System.Windows.Forms.CheckBox();
			this.label22 = new System.Windows.Forms.Label();
			this.comboSearchBehavior = new System.Windows.Forms.ComboBox();
			this.textApptBubNoteLength = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.checkWaitingRoomFilterByView = new System.Windows.Forms.CheckBox();
			this.tabFamily = new System.Windows.Forms.TabPage();
			this.butSyncPhNums = new OpenDental.UI.Button();
			this.checkUsePhoneNumTable = new System.Windows.Forms.CheckBox();
			this.checkSameForFamily = new System.Windows.Forms.CheckBox();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.checkInsurancePlansShared = new System.Windows.Forms.CheckBox();
			this.checkCoPayFeeScheduleBlankLikeZero = new System.Windows.Forms.CheckBox();
			this.checkInsDefaultShowUCRonClaims = new System.Windows.Forms.CheckBox();
			this.checkInsPPOsecWriteoffs = new System.Windows.Forms.CheckBox();
			this.checkInsDefaultAssignmentOfBenefits = new System.Windows.Forms.CheckBox();
			this.checkClaimUseOverrideProcDescript = new System.Windows.Forms.CheckBox();
			this.checkInsPlanExclusionsUseUCR = new System.Windows.Forms.CheckBox();
			this.checkPPOpercentage = new System.Windows.Forms.CheckBox();
			this.checkInsPlanExclusionsMarkDoNotBill = new System.Windows.Forms.CheckBox();
			this.checkClaimTrackingRequireError = new System.Windows.Forms.CheckBox();
			this.checkFixedBenefitBlankLikeZero = new System.Windows.Forms.CheckBox();
			this.groupBoxCOB = new OpenDental.UI.GroupBoxOD();
			this.comboCobSendPaidByInsAt = new OpenDental.UI.ComboBoxOD();
			this.labelCobSendPaidByOtherInsAt = new System.Windows.Forms.Label();
			this.comboCobRule = new System.Windows.Forms.ComboBox();
			this.labelCobRule = new System.Windows.Forms.Label();
			this.checkPatientDOBMasked = new System.Windows.Forms.CheckBox();
			this.checkPatientSSNMasked = new System.Windows.Forms.CheckBox();
			this.groupBoxClaimSnapshot = new OpenDental.UI.GroupBoxOD();
			this.comboClaimSnapshotTrigger = new System.Windows.Forms.ComboBox();
			this.textClaimSnapshotRunTime = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.groupBoxSuperFamily = new OpenDental.UI.GroupBoxOD();
			this.comboSuperFamSort = new System.Windows.Forms.ComboBox();
			this.labelSuperFamSort = new System.Windows.Forms.Label();
			this.checkSuperFamSync = new System.Windows.Forms.CheckBox();
			this.checkSuperFamAddIns = new System.Windows.Forms.CheckBox();
			this.checkSuperFamCloneCreate = new System.Windows.Forms.CheckBox();
			this.checkAllowPatsAtHQ = new System.Windows.Forms.CheckBox();
			this.checkAutoFillPatEmail = new System.Windows.Forms.CheckBox();
			this.checkPreferredReferrals = new System.Windows.Forms.CheckBox();
			this.checkTextMsgOkStatusTreatAsNo = new System.Windows.Forms.CheckBox();
			this.checkPatInitBillingTypeFromPriInsPlan = new System.Windows.Forms.CheckBox();
			this.checkFamPhiAccess = new System.Windows.Forms.CheckBox();
			this.checkSelectProv = new System.Windows.Forms.CheckBox();
			this.checkGoogleAddress = new System.Windows.Forms.CheckBox();
			this.tabAccount = new System.Windows.Forms.TabPage();
			this.label62 = new System.Windows.Forms.Label();
			this.groupBox10 = new OpenDental.UI.GroupBoxOD();
			this.comboRefundAdjustmentType = new OpenDental.UI.ComboBoxOD();
			this.labelRefundAdjustmentType = new System.Windows.Forms.Label();
			this.comboLateChargeAdjType = new OpenDental.UI.ComboBoxOD();
			this.labelLateChargeAdjType = new System.Windows.Forms.Label();
			this.groupBoxOD3 = new OpenDental.UI.GroupBoxOD();
			this.checkAutomateSalesTax = new System.Windows.Forms.CheckBox();
			this.comboSalesTaxAdjType = new OpenDental.UI.ComboBoxOD();
			this.label60 = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.textTaxPercent = new System.Windows.Forms.TextBox();
			this.label33 = new System.Windows.Forms.Label();
			this.comboSalesTaxDefaultProvider = new OpenDental.UI.ComboBoxOD();
			this.comboFinanceChargeAdjType = new OpenDental.UI.ComboBoxOD();
			this.comboBillingChargeAdjType = new OpenDental.UI.ComboBoxOD();
			this.comboPayPlanAdj = new OpenDental.UI.ComboBoxOD();
			this.label42 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox4 = new OpenDental.UI.GroupBoxOD();
			this.listboxBadDebtAdjs = new OpenDental.UI.ListBoxOD();
			this.label29 = new System.Windows.Forms.Label();
			this.butBadDebt = new OpenDental.UI.Button();
			this.groupBox9 = new OpenDental.UI.GroupBoxOD();
			this.checkIncTxfrTreatNegProdAsIncome = new System.Windows.Forms.CheckBox();
			this.checkStoreCCTokens = new System.Windows.Forms.CheckBox();
			this.comboPaymentClinicSetting = new System.Windows.Forms.ComboBox();
			this.label38 = new System.Windows.Forms.Label();
			this.checkPaymentsPromptForPayType = new System.Windows.Forms.CheckBox();
			this.checkAllowPrepayProvider = new System.Windows.Forms.CheckBox();
			this.comboUnallocatedSplits = new OpenDental.UI.ComboBoxOD();
			this.label28 = new System.Windows.Forms.Label();
			this.checkAllowFutureDebits = new System.Windows.Forms.CheckBox();
			this.checkAllowEmailCCReceipt = new System.Windows.Forms.CheckBox();
			this.checkAgingProcLifo = new System.Windows.Forms.CheckBox();
			this.groupBox7 = new OpenDental.UI.GroupBoxOD();
			this.checkShowClaimPayTracking = new System.Windows.Forms.CheckBox();
			this.checkShowClaimPatResp = new System.Windows.Forms.CheckBox();
			this.checkPriClaimAllowSetToHoldUntilPriReceived = new System.Windows.Forms.CheckBox();
			this.checkCanadianPpoLabEst = new System.Windows.Forms.CheckBox();
			this.checkInsEstRecalcReceived = new System.Windows.Forms.CheckBox();
			this.checkPromptForSecondaryClaim = new System.Windows.Forms.CheckBox();
			this.checkInsPayNoWriteoffMoreThanProc = new System.Windows.Forms.CheckBox();
			this.checkClaimTrackingExcludeNone = new System.Windows.Forms.CheckBox();
			this.label55 = new System.Windows.Forms.Label();
			this.comboZeroDollarProcClaimBehavior = new System.Windows.Forms.ComboBox();
			this.labelClaimCredit = new System.Windows.Forms.Label();
			this.comboClaimCredit = new System.Windows.Forms.ComboBox();
			this.checkAllowFuturePayments = new System.Windows.Forms.CheckBox();
			this.groupBoxClaimIdPrefix = new OpenDental.UI.GroupBoxOD();
			this.butReplacements = new OpenDental.UI.Button();
			this.textClaimIdentifier = new System.Windows.Forms.TextBox();
			this.checkAllowProcAdjFromClaim = new System.Windows.Forms.CheckBox();
			this.checkProviderIncomeShows = new System.Windows.Forms.CheckBox();
			this.checkClaimFormTreatDentSaysSigOnFile = new System.Windows.Forms.CheckBox();
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical = new System.Windows.Forms.CheckBox();
			this.checkEclaimsMedicalProvTreatmentAsOrdering = new System.Windows.Forms.CheckBox();
			this.checkEclaimsSeparateTreatProv = new System.Windows.Forms.CheckBox();
			this.label20 = new System.Windows.Forms.Label();
			this.textClaimAttachPath = new System.Windows.Forms.TextBox();
			this.checkClaimsValidateACN = new System.Windows.Forms.CheckBox();
			this.textInsWriteoffDescript = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupRepeatingCharges = new OpenDental.UI.GroupBoxOD();
			this.labelRepeatingChargesAutomatedTime = new System.Windows.Forms.Label();
			this.textRepeatingChargesAutomatedTime = new OpenDental.ValidTime();
			this.checkRepeatingChargesRunAging = new System.Windows.Forms.CheckBox();
			this.checkRepeatingChargesAutomated = new System.Windows.Forms.CheckBox();
			this.groupRecurringCharges = new OpenDental.UI.GroupBoxOD();
			this.checkRecurringChargesShowInactive = new System.Windows.Forms.CheckBox();
			this.checkRecurringChargesInactivateDeclinedCards = new System.Windows.Forms.CheckBox();
			this.checkRecurPatBal0 = new System.Windows.Forms.CheckBox();
			this.label56 = new System.Windows.Forms.Label();
			this.comboRecurringChargePayType = new OpenDental.UI.ComboBoxOD();
			this.labelRecurringChargesAutomatedTime = new System.Windows.Forms.Label();
			this.textRecurringChargesTime = new OpenDental.ValidTime();
			this.checkRecurringChargesAutomated = new System.Windows.Forms.CheckBox();
			this.checkRecurringChargesUseTransDate = new System.Windows.Forms.CheckBox();
			this.checkRecurChargPriProv = new System.Windows.Forms.CheckBox();
			this.checkBalancesDontSubtractIns = new System.Windows.Forms.CheckBox();
			this.checkAllowFutureTrans = new System.Windows.Forms.CheckBox();
			this.checkPpoUseUcr = new System.Windows.Forms.CheckBox();
			this.groupCommLogs = new OpenDental.UI.GroupBoxOD();
			this.checkCommLogAutoSave = new System.Windows.Forms.CheckBox();
			this.checkShowFamilyCommByDefault = new System.Windows.Forms.CheckBox();
			this.checkAccountShowPaymentNums = new System.Windows.Forms.CheckBox();
			this.checkShowAllocateUnearnedPaymentPrompt = new System.Windows.Forms.CheckBox();
			this.checkAgingMonthly = new System.Windows.Forms.CheckBox();
			this.checkStatementInvoiceGridShowWriteoffs = new System.Windows.Forms.CheckBox();
			this.groupPayPlans = new OpenDental.UI.GroupBoxOD();
			this.label39 = new System.Windows.Forms.Label();
			this.comboDppUnearnedType = new OpenDental.UI.ComboBoxOD();
			this.label59 = new System.Windows.Forms.Label();
			this.textDynamicPayPlan = new OpenDental.ValidTime();
			this.label27 = new System.Windows.Forms.Label();
			this.comboPayPlansVersion = new System.Windows.Forms.ComboBox();
			this.checkHideDueNow = new System.Windows.Forms.CheckBox();
			this.checkPayPlansUseSheets = new System.Windows.Forms.CheckBox();
			this.checkPayPlansExcludePastActivity = new System.Windows.Forms.CheckBox();
			this.tabTreatPlan = new System.Windows.Forms.TabPage();
			this.groupBoxOD4 = new OpenDental.UI.GroupBoxOD();
			this.label63 = new System.Windows.Forms.Label();
			this.textDiscountPACodes = new System.Windows.Forms.TextBox();
			this.labelDiscountPAFreq = new System.Windows.Forms.Label();
			this.textDiscountXrayCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountXrayFreq = new System.Windows.Forms.Label();
			this.textDiscountPerioCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountPerioFreq = new System.Windows.Forms.Label();
			this.textDiscountLimitedCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountLimitedFreq = new System.Windows.Forms.Label();
			this.textDiscountFluorideCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountProphyFreq = new System.Windows.Forms.Label();
			this.textDiscountExamCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountFluorideFreq = new System.Windows.Forms.Label();
			this.textDiscountProphyCodes = new System.Windows.Forms.TextBox();
			this.labelDiscountExamFreq = new System.Windows.Forms.Label();
			this.checkPromptSaveTP = new System.Windows.Forms.CheckBox();
			this.labelDiscountPercentage = new System.Windows.Forms.Label();
			this.groupBox6 = new OpenDental.UI.GroupBoxOD();
			this.textInsImplant = new System.Windows.Forms.TextBox();
			this.labelInsImplant = new System.Windows.Forms.Label();
			this.label52 = new System.Windows.Forms.Label();
			this.textInsDentures = new System.Windows.Forms.TextBox();
			this.labelInsDentures = new System.Windows.Forms.Label();
			this.textInsPerioMaint = new System.Windows.Forms.TextBox();
			this.labelInsPerioMaint = new System.Windows.Forms.Label();
			this.textInsDebridement = new System.Windows.Forms.TextBox();
			this.labelInsDebridement = new System.Windows.Forms.Label();
			this.textInsSealant = new System.Windows.Forms.TextBox();
			this.labelInsSealant = new System.Windows.Forms.Label();
			this.textInsFlouride = new System.Windows.Forms.TextBox();
			this.labelInsFlouride = new System.Windows.Forms.Label();
			this.textInsCrown = new System.Windows.Forms.TextBox();
			this.labelInsCrown = new System.Windows.Forms.Label();
			this.textInsSRP = new System.Windows.Forms.TextBox();
			this.labelInsSRP = new System.Windows.Forms.Label();
			this.textInsCancerScreen = new System.Windows.Forms.TextBox();
			this.labelInsCancerScreen = new System.Windows.Forms.Label();
			this.textInsProphy = new System.Windows.Forms.TextBox();
			this.labelInsProphy = new System.Windows.Forms.Label();
			this.textInsExam = new System.Windows.Forms.TextBox();
			this.labelInsPano = new System.Windows.Forms.Label();
			this.textInsBW = new System.Windows.Forms.TextBox();
			this.labelInsExam = new System.Windows.Forms.Label();
			this.textInsPano = new System.Windows.Forms.TextBox();
			this.labelInsBW = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.groupInsHist = new OpenDental.UI.GroupBoxOD();
			this.textInsHistProphy = new System.Windows.Forms.TextBox();
			this.labelInsHistProphy = new System.Windows.Forms.Label();
			this.textInsHistPerioLR = new System.Windows.Forms.TextBox();
			this.labelInsHistPerioLR = new System.Windows.Forms.Label();
			this.textInsHistPerioLL = new System.Windows.Forms.TextBox();
			this.labelInsHistPerioLL = new System.Windows.Forms.Label();
			this.textInsHistPerioUL = new System.Windows.Forms.TextBox();
			this.labelInsHistPerioUL = new System.Windows.Forms.Label();
			this.textInsHistPerioUR = new System.Windows.Forms.TextBox();
			this.labelInsHistPerioUR = new System.Windows.Forms.Label();
			this.textInsHistFMX = new System.Windows.Forms.TextBox();
			this.labelInsHistFMX = new System.Windows.Forms.Label();
			this.textInsHistPerioMaint = new System.Windows.Forms.TextBox();
			this.labelInsHistPerioMaint = new System.Windows.Forms.Label();
			this.textInsHistExam = new System.Windows.Forms.TextBox();
			this.labelInsHistDebridement = new System.Windows.Forms.Label();
			this.textInsHistBW = new System.Windows.Forms.TextBox();
			this.labelInsHistExam = new System.Windows.Forms.Label();
			this.textInsHistDebridement = new System.Windows.Forms.TextBox();
			this.labelInsHistBW = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.checkFrequency = new System.Windows.Forms.CheckBox();
			this.groupTreatPlanSort = new OpenDental.UI.GroupBoxOD();
			this.radioTreatPlanSortTooth = new System.Windows.Forms.RadioButton();
			this.radioTreatPlanSortOrder = new System.Windows.Forms.RadioButton();
			this.textTreatNote = new OpenDental.ODtextBox();
			this.checkTPSaveSigned = new System.Windows.Forms.CheckBox();
			this.comboProcDiscountType = new OpenDental.UI.ComboBoxOD();
			this.checkTreatPlanShowCompleted = new System.Windows.Forms.CheckBox();
			this.textDiscountPercentage = new System.Windows.Forms.TextBox();
			this.checkTreatPlanItemized = new System.Windows.Forms.CheckBox();
			this.tabChart = new System.Windows.Forms.TabPage();
			this.checkNotesProviderSigOnly = new System.Windows.Forms.CheckBox();
			this.checkShowPlannedApptPrompt = new System.Windows.Forms.CheckBox();
			this.checkAllowSettingProcsComplete = new System.Windows.Forms.CheckBox();
			this.comboToothNomenclature = new System.Windows.Forms.ComboBox();
			this.textProblemsIndicateNone = new System.Windows.Forms.TextBox();
			this.label32 = new System.Windows.Forms.Label();
			this.checkBoxRxClinicUseSelected = new System.Windows.Forms.CheckBox();
			this.checkProcNoteConcurrencyMerge = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboProcCodeListSort = new System.Windows.Forms.ComboBox();
			this.checkProcProvChangesCp = new System.Windows.Forms.CheckBox();
			this.labelToothNomenclature = new System.Windows.Forms.Label();
			this.comboProcFeeUpdatePrompt = new System.Windows.Forms.ComboBox();
			this.checkPerioTreatImplantsAsNotMissing = new System.Windows.Forms.CheckBox();
			this.labelProcFeeUpdatePrompt = new System.Windows.Forms.Label();
			this.checkAutoClearEntryStatus = new System.Windows.Forms.CheckBox();
			this.butProblemsIndicateNone = new OpenDental.UI.Button();
			this.checkPerioSkipMissingTeeth = new System.Windows.Forms.CheckBox();
			this.label9 = new System.Windows.Forms.Label();
			this.checkProcGroupNoteDoesAggregate = new System.Windows.Forms.CheckBox();
			this.textMedicationsIndicateNone = new System.Windows.Forms.TextBox();
			this.checkProvColorChart = new System.Windows.Forms.CheckBox();
			this.checkSignatureAllowDigital = new System.Windows.Forms.CheckBox();
			this.textAllergiesIndicateNone = new System.Windows.Forms.TextBox();
			this.butMedicationsIndicateNone = new OpenDental.UI.Button();
			this.textMedDefaultStopDays = new System.Windows.Forms.TextBox();
			this.checkClaimProcsAllowEstimatesOnCompl = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.checkProcEditRequireAutoCode = new System.Windows.Forms.CheckBox();
			this.checkChartNonPatientWarn = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.checkProcLockingIsAllowed = new System.Windows.Forms.CheckBox();
			this.checkProcsPromptForAutoNote = new System.Windows.Forms.CheckBox();
			this.textICD9DefaultForNewProcs = new System.Windows.Forms.TextBox();
			this.labelIcdCodeDefault = new System.Windows.Forms.Label();
			this.checkScreeningsUseSheets = new System.Windows.Forms.CheckBox();
			this.butDiagnosisCode = new OpenDental.UI.Button();
			this.butAllergiesIndicateNone = new OpenDental.UI.Button();
			this.checkDxIcdVersion = new System.Windows.Forms.CheckBox();
			this.checkMedicalFeeUsedForNewProcs = new System.Windows.Forms.CheckBox();
			this.tabImages = new System.Windows.Forms.TabPage();
			this.checkPDFLaunchWindow = new System.Windows.Forms.CheckBox();
			this.label61 = new System.Windows.Forms.Label();
			this.textDefaultImageImportFolder = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textAutoImportFolder = new System.Windows.Forms.TextBox();
			this.tabManage = new System.Windows.Forms.TabPage();
			this.comboEraAutomation = new OpenDental.UI.ComboBoxOD();
			this.labelEraAutomation = new System.Windows.Forms.Label();
			this.comboDepositSoftware = new OpenDental.UI.ComboBoxOD();
			this.labelDepositSoftware = new System.Windows.Forms.Label();
			this.checkEraAllowTotalPayment = new System.Windows.Forms.CheckBox();
			this.checkIncludeEraWOPercCoPay = new System.Windows.Forms.CheckBox();
			this.checkClockEventAllowBreak = new System.Windows.Forms.CheckBox();
			this.textClaimsReceivedDays = new OpenDental.ValidNum();
			this.checkShowAutoDeposit = new System.Windows.Forms.CheckBox();
			this.checkEraOneClaimPerPage = new System.Windows.Forms.CheckBox();
			this.checkClaimPaymentBatchOnly = new System.Windows.Forms.CheckBox();
			this.labelClaimsReceivedDays = new System.Windows.Forms.Label();
			this.checkScheduleProvEmpSelectAll = new System.Windows.Forms.CheckBox();
			this.checkClaimsSendWindowValidateOnLoad = new System.Windows.Forms.CheckBox();
			this.checkTimeCardADP = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new OpenDental.UI.GroupBoxOD();
			this.checkStatementsAlphabetically = new System.Windows.Forms.CheckBox();
			this.checkBillingShowProgress = new System.Windows.Forms.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.textBillingElectBatchMax = new OpenDental.ValidNum();
			this.checkStatementShowAdjNotes = new System.Windows.Forms.CheckBox();
			this.checkIntermingleDefault = new System.Windows.Forms.CheckBox();
			this.checkStatementShowReturnAddress = new System.Windows.Forms.CheckBox();
			this.checkStatementShowProcBreakdown = new System.Windows.Forms.CheckBox();
			this.checkStatementShowNotes = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboUseChartNum = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.textStatementsCalcDueDate = new OpenDental.ValidNum();
			this.textPayPlansBillInAdvanceDays = new OpenDental.ValidNum();
			this.comboTimeCardOvertimeFirstDayOfWeek = new System.Windows.Forms.ComboBox();
			this.label16 = new System.Windows.Forms.Label();
			this.checkRxSendNewToQueue = new System.Windows.Forms.CheckBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.tabControlMain.SuspendLayout();
			this.tabAppts.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabFamily.SuspendLayout();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxCOB.SuspendLayout();
			this.groupBoxClaimSnapshot.SuspendLayout();
			this.groupBoxSuperFamily.SuspendLayout();
			this.tabAccount.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.groupBoxOD3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBoxClaimIdPrefix.SuspendLayout();
			this.groupRepeatingCharges.SuspendLayout();
			this.groupRecurringCharges.SuspendLayout();
			this.groupCommLogs.SuspendLayout();
			this.groupPayPlans.SuspendLayout();
			this.tabTreatPlan.SuspendLayout();
			this.groupBoxOD4.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupInsHist.SuspendLayout();
			this.groupTreatPlanSort.SuspendLayout();
			this.tabChart.SuspendLayout();
			this.tabImages.SuspendLayout();
			this.tabManage.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 0;
			this.toolTip1.AutoPopDelay = 600000;
			this.toolTip1.InitialDelay = 0;
			this.toolTip1.IsBalloon = true;
			this.toolTip1.ReshowDelay = 0;
			this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.toolTip1.ToolTipTitle = "Help";
			// 
			// checkIsAlertRadiologyProcsEnabled
			// 
			this.checkIsAlertRadiologyProcsEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsAlertRadiologyProcsEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsAlertRadiologyProcsEnabled.Location = new System.Drawing.Point(141, 432);
			this.checkIsAlertRadiologyProcsEnabled.Name = "checkIsAlertRadiologyProcsEnabled";
			this.checkIsAlertRadiologyProcsEnabled.Size = new System.Drawing.Size(364, 17);
			this.checkIsAlertRadiologyProcsEnabled.TabIndex = 229;
			this.checkIsAlertRadiologyProcsEnabled.Text = "OpenDentalService alerts for scheduled non-CPOE radiology procedures";
			this.checkIsAlertRadiologyProcsEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsAlertRadiologyProcsEnabled.UseVisualStyleBackColor = true;
			// 
			// checkImagesModuleTreeIsCollapsed
			// 
			this.checkImagesModuleTreeIsCollapsed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkImagesModuleTreeIsCollapsed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkImagesModuleTreeIsCollapsed.Location = new System.Drawing.Point(81, 7);
			this.checkImagesModuleTreeIsCollapsed.Name = "checkImagesModuleTreeIsCollapsed";
			this.checkImagesModuleTreeIsCollapsed.Size = new System.Drawing.Size(359, 17);
			this.checkImagesModuleTreeIsCollapsed.TabIndex = 47;
			this.checkImagesModuleTreeIsCollapsed.Text = "Document tree collapses when patient changes";
			this.checkImagesModuleTreeIsCollapsed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkApptsCheckFrequency
			// 
			this.checkApptsCheckFrequency.Location = new System.Drawing.Point(0, 0);
			this.checkApptsCheckFrequency.Name = "checkApptsCheckFrequency";
			this.checkApptsCheckFrequency.Size = new System.Drawing.Size(104, 24);
			this.checkApptsCheckFrequency.TabIndex = 0;
			// 
			// tabControlMain
			// 
			this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlMain.Controls.Add(this.tabAppts);
			this.tabControlMain.Controls.Add(this.tabFamily);
			this.tabControlMain.Controls.Add(this.tabAccount);
			this.tabControlMain.Controls.Add(this.tabTreatPlan);
			this.tabControlMain.Controls.Add(this.tabChart);
			this.tabControlMain.Controls.Add(this.tabImages);
			this.tabControlMain.Controls.Add(this.tabManage);
			this.tabControlMain.Location = new System.Drawing.Point(-3, 1);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1235, 666);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabAppts
			// 
			this.tabAppts.BackColor = System.Drawing.SystemColors.Control;
			this.tabAppts.Controls.Add(this.checkBrokenApptRequiredOnMove);
			this.tabAppts.Controls.Add(this.groupBox8);
			this.tabAppts.Controls.Add(this.checkApptsAllowOverlap);
			this.tabAppts.Controls.Add(this.checkPreventChangesToComplAppts);
			this.tabAppts.Controls.Add(this.textApptAutoRefreshRange);
			this.tabAppts.Controls.Add(this.labelApptAutoRefreshRange);
			this.tabAppts.Controls.Add(this.checkUnscheduledListNoRecalls);
			this.tabAppts.Controls.Add(this.checkReplaceBlockouts);
			this.tabAppts.Controls.Add(this.labelApptSchedEnforceSpecialty);
			this.tabAppts.Controls.Add(this.comboApptSchedEnforceSpecialty);
			this.tabAppts.Controls.Add(this.textApptWithoutProcsDefaultLength);
			this.tabAppts.Controls.Add(this.labelApptWithoutProcsDefaultLength);
			this.tabAppts.Controls.Add(this.checkApptAllowEmptyComplete);
			this.tabAppts.Controls.Add(this.checkApptAllowFutureComplete);
			this.tabAppts.Controls.Add(this.comboTimeArrived);
			this.tabAppts.Controls.Add(this.checkApptsRequireProcs);
			this.tabAppts.Controls.Add(this.label3);
			this.tabAppts.Controls.Add(this.checkApptModuleProductionUsesOps);
			this.tabAppts.Controls.Add(this.comboTimeSeated);
			this.tabAppts.Controls.Add(this.checkUseOpHygProv);
			this.tabAppts.Controls.Add(this.label5);
			this.tabAppts.Controls.Add(this.checkApptModuleAdjInProd);
			this.tabAppts.Controls.Add(this.comboTimeDismissed);
			this.tabAppts.Controls.Add(this.checkApptTimeReset);
			this.tabAppts.Controls.Add(this.label6);
			this.tabAppts.Controls.Add(this.groupBox2);
			this.tabAppts.Controls.Add(this.textWaitRoomWarn);
			this.tabAppts.Controls.Add(this.checkAppointmentTimeIsLocked);
			this.tabAppts.Controls.Add(this.label22);
			this.tabAppts.Controls.Add(this.comboSearchBehavior);
			this.tabAppts.Controls.Add(this.textApptBubNoteLength);
			this.tabAppts.Controls.Add(this.label13);
			this.tabAppts.Controls.Add(this.label21);
			this.tabAppts.Controls.Add(this.checkWaitingRoomFilterByView);
			this.tabAppts.Location = new System.Drawing.Point(4, 22);
			this.tabAppts.Name = "tabAppts";
			this.tabAppts.Padding = new System.Windows.Forms.Padding(3);
			this.tabAppts.Size = new System.Drawing.Size(1227, 640);
			this.tabAppts.TabIndex = 0;
			this.tabAppts.Text = "Appts";
			// 
			// checkBrokenApptRequiredOnMove
			// 
			this.checkBrokenApptRequiredOnMove.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptRequiredOnMove.Location = new System.Drawing.Point(63, 572);
			this.checkBrokenApptRequiredOnMove.Name = "checkBrokenApptRequiredOnMove";
			this.checkBrokenApptRequiredOnMove.Size = new System.Drawing.Size(397, 17);
			this.checkBrokenApptRequiredOnMove.TabIndex = 290;
			this.checkBrokenApptRequiredOnMove.Text = "Force users to break scheduled appointments before rescheduling";
			this.checkBrokenApptRequiredOnMove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox8
			// 
			this.groupBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox8.Controls.Add(this.checkAppointmentBubblesDisabled);
			this.groupBox8.Controls.Add(this.textApptProvbarWidth);
			this.groupBox8.Controls.Add(this.checkSolidBlockouts);
			this.groupBox8.Controls.Add(this.label58);
			this.groupBox8.Controls.Add(this.checkApptExclamation);
			this.groupBox8.Controls.Add(this.textApptFontSize);
			this.groupBox8.Controls.Add(this.butApptLineColor);
			this.groupBox8.Controls.Add(this.checkApptBubbleDelay);
			this.groupBox8.Controls.Add(this.butColor);
			this.groupBox8.Controls.Add(this.label23);
			this.groupBox8.Controls.Add(this.comboDelay);
			this.groupBox8.Controls.Add(this.label25);
			this.groupBox8.Controls.Add(this.checkApptModuleDefaultToWeek);
			this.groupBox8.Controls.Add(this.checkApptRefreshEveryMinute);
			this.groupBox8.Controls.Add(this.apptClickDelay);
			this.groupBox8.Controls.Add(this.label54);
			this.groupBox8.Location = new System.Drawing.Point(584, 17);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(449, 285);
			this.groupBox8.TabIndex = 289;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Appearance";
			// 
			// checkAppointmentBubblesDisabled
			// 
			this.checkAppointmentBubblesDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAppointmentBubblesDisabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAppointmentBubblesDisabled.Location = new System.Drawing.Point(14, 19);
			this.checkAppointmentBubblesDisabled.Name = "checkAppointmentBubblesDisabled";
			this.checkAppointmentBubblesDisabled.Size = new System.Drawing.Size(425, 17);
			this.checkAppointmentBubblesDisabled.TabIndex = 234;
			this.checkAppointmentBubblesDisabled.Text = "Default appointment bubble to \'disabled\' for new appointment views";
			this.checkAppointmentBubblesDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAppointmentBubblesDisabled.UseVisualStyleBackColor = true;
			// 
			// textApptProvbarWidth
			// 
			this.textApptProvbarWidth.Location = new System.Drawing.Point(389, 258);
			this.textApptProvbarWidth.MaxVal = 20;
			this.textApptProvbarWidth.Name = "textApptProvbarWidth";
			this.textApptProvbarWidth.Size = new System.Drawing.Size(50, 20);
			this.textApptProvbarWidth.TabIndex = 288;
			// 
			// checkSolidBlockouts
			// 
			this.checkSolidBlockouts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSolidBlockouts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSolidBlockouts.Location = new System.Drawing.Point(31, 65);
			this.checkSolidBlockouts.Name = "checkSolidBlockouts";
			this.checkSolidBlockouts.Size = new System.Drawing.Size(408, 17);
			this.checkSolidBlockouts.TabIndex = 220;
			this.checkSolidBlockouts.Text = "Use solid blockouts instead of outlines on the Appointments Module";
			this.checkSolidBlockouts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSolidBlockouts.UseVisualStyleBackColor = true;
			// 
			// label58
			// 
			this.label58.Location = new System.Drawing.Point(72, 262);
			this.label58.Name = "label58";
			this.label58.Size = new System.Drawing.Size(315, 16);
			this.label58.TabIndex = 286;
			this.label58.Text = "Width of provider time bar on left of each appointment";
			this.label58.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptExclamation
			// 
			this.checkApptExclamation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptExclamation.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkApptExclamation.Location = new System.Drawing.Point(14, 88);
			this.checkApptExclamation.Name = "checkApptExclamation";
			this.checkApptExclamation.Size = new System.Drawing.Size(425, 17);
			this.checkApptExclamation.TabIndex = 222;
			this.checkApptExclamation.Text = "Show ! on appts for ins not sent, if added to Appt View (might cause slowdown)";
			this.checkApptExclamation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptExclamation.UseVisualStyleBackColor = true;
			// 
			// textApptFontSize
			// 
			this.textApptFontSize.Location = new System.Drawing.Point(389, 235);
			this.textApptFontSize.Name = "textApptFontSize";
			this.textApptFontSize.Size = new System.Drawing.Size(50, 20);
			this.textApptFontSize.TabIndex = 285;
			// 
			// butApptLineColor
			// 
			this.butApptLineColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butApptLineColor.Location = new System.Drawing.Point(415, 136);
			this.butApptLineColor.Name = "butApptLineColor";
			this.butApptLineColor.Size = new System.Drawing.Size(24, 21);
			this.butApptLineColor.TabIndex = 226;
			this.butApptLineColor.Click += new System.EventHandler(this.butApptLineColor_Click);
			// 
			// checkApptBubbleDelay
			// 
			this.checkApptBubbleDelay.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptBubbleDelay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkApptBubbleDelay.Location = new System.Drawing.Point(14, 42);
			this.checkApptBubbleDelay.Name = "checkApptBubbleDelay";
			this.checkApptBubbleDelay.Size = new System.Drawing.Size(425, 17);
			this.checkApptBubbleDelay.TabIndex = 221;
			this.checkApptBubbleDelay.Text = "Appointment bubble popup delay";
			this.checkApptBubbleDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptBubbleDelay.UseVisualStyleBackColor = true;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(415, 111);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(24, 21);
			this.butColor.TabIndex = 225;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(165, 115);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(246, 16);
			this.label23.TabIndex = 223;
			this.label23.Text = "Waiting room alert color";
			this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboDelay
			// 
			this.comboDelay.AllowDrop = true;
			this.comboDelay.Location = new System.Drawing.Point(325, 186);
			this.comboDelay.Name = "comboDelay";
			this.comboDelay.Size = new System.Drawing.Size(114, 21);
			this.comboDelay.TabIndex = 232;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(165, 141);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(246, 16);
			this.label25.TabIndex = 224;
			this.label25.Text = "Appointment time line color";
			this.label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptModuleDefaultToWeek
			// 
			this.checkApptModuleDefaultToWeek.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleDefaultToWeek.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkApptModuleDefaultToWeek.Location = new System.Drawing.Point(33, 163);
			this.checkApptModuleDefaultToWeek.Name = "checkApptModuleDefaultToWeek";
			this.checkApptModuleDefaultToWeek.Size = new System.Drawing.Size(406, 17);
			this.checkApptModuleDefaultToWeek.TabIndex = 220;
			this.checkApptModuleDefaultToWeek.Text = "Appointments Module defaults to week view";
			this.checkApptModuleDefaultToWeek.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkApptRefreshEveryMinute
			// 
			this.checkApptRefreshEveryMinute.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptRefreshEveryMinute.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkApptRefreshEveryMinute.Location = new System.Drawing.Point(33, 213);
			this.checkApptRefreshEveryMinute.Name = "checkApptRefreshEveryMinute";
			this.checkApptRefreshEveryMinute.Size = new System.Drawing.Size(406, 17);
			this.checkApptRefreshEveryMinute.TabIndex = 235;
			this.checkApptRefreshEveryMinute.Text = "Refresh every 60 seconds, keeps waiting room times refreshed";
			this.checkApptRefreshEveryMinute.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// apptClickDelay
			// 
			this.apptClickDelay.Location = new System.Drawing.Point(166, 188);
			this.apptClickDelay.Name = "apptClickDelay";
			this.apptClickDelay.Size = new System.Drawing.Size(157, 18);
			this.apptClickDelay.TabIndex = 233;
			this.apptClickDelay.Text = "Appointment click delay";
			this.apptClickDelay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label54
			// 
			this.label54.Location = new System.Drawing.Point(14, 239);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(373, 16);
			this.label54.TabIndex = 251;
			this.label54.Text = "Appointment font size. Default is 8. Decimals allowed. In addition to Zoom";
			this.label54.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptsAllowOverlap
			// 
			this.checkApptsAllowOverlap.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsAllowOverlap.Location = new System.Drawing.Point(181, 555);
			this.checkApptsAllowOverlap.Name = "checkApptsAllowOverlap";
			this.checkApptsAllowOverlap.Size = new System.Drawing.Size(279, 17);
			this.checkApptsAllowOverlap.TabIndex = 284;
			this.checkApptsAllowOverlap.Text = "Appointments allow overlap";
			this.checkApptsAllowOverlap.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsAllowOverlap.ThreeState = true;
			// 
			// checkPreventChangesToComplAppts
			// 
			this.checkPreventChangesToComplAppts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventChangesToComplAppts.Location = new System.Drawing.Point(34, 538);
			this.checkPreventChangesToComplAppts.Name = "checkPreventChangesToComplAppts";
			this.checkPreventChangesToComplAppts.Size = new System.Drawing.Size(426, 17);
			this.checkPreventChangesToComplAppts.TabIndex = 283;
			this.checkPreventChangesToComplAppts.Text = "Prevent changes to completed appointments with completed procedures";
			this.checkPreventChangesToComplAppts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventChangesToComplAppts.UseVisualStyleBackColor = true;
			// 
			// textApptAutoRefreshRange
			// 
			this.textApptAutoRefreshRange.Location = new System.Drawing.Point(391, 516);
			this.textApptAutoRefreshRange.MaxVal = 600;
			this.textApptAutoRefreshRange.MinVal = -1;
			this.textApptAutoRefreshRange.Name = "textApptAutoRefreshRange";
			this.textApptAutoRefreshRange.ShowZero = false;
			this.textApptAutoRefreshRange.Size = new System.Drawing.Size(70, 20);
			this.textApptAutoRefreshRange.TabIndex = 282;
			// 
			// labelApptAutoRefreshRange
			// 
			this.labelApptAutoRefreshRange.Location = new System.Drawing.Point(26, 519);
			this.labelApptAutoRefreshRange.Name = "labelApptAutoRefreshRange";
			this.labelApptAutoRefreshRange.Size = new System.Drawing.Size(363, 16);
			this.labelApptAutoRefreshRange.TabIndex = 281;
			this.labelApptAutoRefreshRange.Text = "Number of days out to automatically refresh Appointments Module (-1 for all)";
			this.labelApptAutoRefreshRange.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkUnscheduledListNoRecalls
			// 
			this.checkUnscheduledListNoRecalls.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUnscheduledListNoRecalls.Location = new System.Drawing.Point(55, 497);
			this.checkUnscheduledListNoRecalls.Name = "checkUnscheduledListNoRecalls";
			this.checkUnscheduledListNoRecalls.Size = new System.Drawing.Size(406, 17);
			this.checkUnscheduledListNoRecalls.TabIndex = 280;
			this.checkUnscheduledListNoRecalls.Text = "Do not allow recall appointments on the Unscheduled List";
			this.checkUnscheduledListNoRecalls.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReplaceBlockouts
			// 
			this.checkReplaceBlockouts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReplaceBlockouts.Location = new System.Drawing.Point(55, 479);
			this.checkReplaceBlockouts.Name = "checkReplaceBlockouts";
			this.checkReplaceBlockouts.Size = new System.Drawing.Size(406, 17);
			this.checkReplaceBlockouts.TabIndex = 279;
			this.checkReplaceBlockouts.Text = "Allow \'Block appointment scheduling\' blockouts to replace conflicting blockouts";
			this.checkReplaceBlockouts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelApptSchedEnforceSpecialty
			// 
			this.labelApptSchedEnforceSpecialty.Location = new System.Drawing.Point(48, 453);
			this.labelApptSchedEnforceSpecialty.Name = "labelApptSchedEnforceSpecialty";
			this.labelApptSchedEnforceSpecialty.Size = new System.Drawing.Size(247, 17);
			this.labelApptSchedEnforceSpecialty.TabIndex = 278;
			this.labelApptSchedEnforceSpecialty.Text = "Enforce clinic specialties";
			this.labelApptSchedEnforceSpecialty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboApptSchedEnforceSpecialty
			// 
			this.comboApptSchedEnforceSpecialty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboApptSchedEnforceSpecialty.FormattingEnabled = true;
			this.comboApptSchedEnforceSpecialty.Location = new System.Drawing.Point(297, 452);
			this.comboApptSchedEnforceSpecialty.Name = "comboApptSchedEnforceSpecialty";
			this.comboApptSchedEnforceSpecialty.Size = new System.Drawing.Size(163, 21);
			this.comboApptSchedEnforceSpecialty.TabIndex = 277;
			// 
			// textApptWithoutProcsDefaultLength
			// 
			this.textApptWithoutProcsDefaultLength.Location = new System.Drawing.Point(361, 390);
			this.textApptWithoutProcsDefaultLength.MaxVal = 600;
			this.textApptWithoutProcsDefaultLength.Name = "textApptWithoutProcsDefaultLength";
			this.textApptWithoutProcsDefaultLength.ShowZero = false;
			this.textApptWithoutProcsDefaultLength.Size = new System.Drawing.Size(100, 20);
			this.textApptWithoutProcsDefaultLength.TabIndex = 276;
			// 
			// labelApptWithoutProcsDefaultLength
			// 
			this.labelApptWithoutProcsDefaultLength.Location = new System.Drawing.Point(40, 393);
			this.labelApptWithoutProcsDefaultLength.Name = "labelApptWithoutProcsDefaultLength";
			this.labelApptWithoutProcsDefaultLength.Size = new System.Drawing.Size(319, 16);
			this.labelApptWithoutProcsDefaultLength.TabIndex = 275;
			this.labelApptWithoutProcsDefaultLength.Text = "Appointment without procedures default length";
			this.labelApptWithoutProcsDefaultLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptAllowEmptyComplete
			// 
			this.checkApptAllowEmptyComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptAllowEmptyComplete.Location = new System.Drawing.Point(54, 432);
			this.checkApptAllowEmptyComplete.Name = "checkApptAllowEmptyComplete";
			this.checkApptAllowEmptyComplete.Size = new System.Drawing.Size(406, 17);
			this.checkApptAllowEmptyComplete.TabIndex = 274;
			this.checkApptAllowEmptyComplete.Text = "Allow setting appointments without procedures complete";
			this.checkApptAllowEmptyComplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkApptAllowFutureComplete
			// 
			this.checkApptAllowFutureComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptAllowFutureComplete.Location = new System.Drawing.Point(54, 414);
			this.checkApptAllowFutureComplete.Name = "checkApptAllowFutureComplete";
			this.checkApptAllowFutureComplete.Size = new System.Drawing.Size(406, 17);
			this.checkApptAllowFutureComplete.TabIndex = 273;
			this.checkApptAllowFutureComplete.Text = "Allow setting future appointments complete";
			this.checkApptAllowFutureComplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTimeArrived
			// 
			this.comboTimeArrived.Location = new System.Drawing.Point(297, 117);
			this.comboTimeArrived.Name = "comboTimeArrived";
			this.comboTimeArrived.Size = new System.Drawing.Size(163, 21);
			this.comboTimeArrived.TabIndex = 253;
			// 
			// checkApptsRequireProcs
			// 
			this.checkApptsRequireProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsRequireProcs.Location = new System.Drawing.Point(54, 370);
			this.checkApptsRequireProcs.Name = "checkApptsRequireProcs";
			this.checkApptsRequireProcs.Size = new System.Drawing.Size(406, 17);
			this.checkApptsRequireProcs.TabIndex = 272;
			this.checkApptsRequireProcs.Text = "Appointments require procedures";
			this.checkApptsRequireProcs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsRequireProcs.CheckedChanged += new System.EventHandler(this.checkApptsRequireProcs_CheckedChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 121);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(247, 15);
			this.label3.TabIndex = 254;
			this.label3.Text = "Time Arrived trigger";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptModuleProductionUsesOps
			// 
			this.checkApptModuleProductionUsesOps.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleProductionUsesOps.Location = new System.Drawing.Point(54, 352);
			this.checkApptModuleProductionUsesOps.Name = "checkApptModuleProductionUsesOps";
			this.checkApptModuleProductionUsesOps.Size = new System.Drawing.Size(406, 17);
			this.checkApptModuleProductionUsesOps.TabIndex = 271;
			this.checkApptModuleProductionUsesOps.Text = "Appointments Module production uses operatories";
			this.checkApptModuleProductionUsesOps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTimeSeated
			// 
			this.comboTimeSeated.Location = new System.Drawing.Point(297, 139);
			this.comboTimeSeated.Name = "comboTimeSeated";
			this.comboTimeSeated.Size = new System.Drawing.Size(163, 21);
			this.comboTimeSeated.TabIndex = 255;
			// 
			// checkUseOpHygProv
			// 
			this.checkUseOpHygProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseOpHygProv.Location = new System.Drawing.Point(54, 334);
			this.checkUseOpHygProv.Name = "checkUseOpHygProv";
			this.checkUseOpHygProv.Size = new System.Drawing.Size(406, 17);
			this.checkUseOpHygProv.TabIndex = 270;
			this.checkUseOpHygProv.Text = "Force op\'s hygiene provider as secondary provider";
			this.checkUseOpHygProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(48, 143);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(247, 15);
			this.label5.TabIndex = 256;
			this.label5.Text = "Time Seated (in op) trigger";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptModuleAdjInProd
			// 
			this.checkApptModuleAdjInProd.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleAdjInProd.Location = new System.Drawing.Point(54, 316);
			this.checkApptModuleAdjInProd.Name = "checkApptModuleAdjInProd";
			this.checkApptModuleAdjInProd.Size = new System.Drawing.Size(406, 17);
			this.checkApptModuleAdjInProd.TabIndex = 269;
			this.checkApptModuleAdjInProd.Text = "Add daily adjustments to net production";
			this.checkApptModuleAdjInProd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTimeDismissed
			// 
			this.comboTimeDismissed.Location = new System.Drawing.Point(297, 161);
			this.comboTimeDismissed.Name = "comboTimeDismissed";
			this.comboTimeDismissed.Size = new System.Drawing.Size(163, 21);
			this.comboTimeDismissed.TabIndex = 257;
			// 
			// checkApptTimeReset
			// 
			this.checkApptTimeReset.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptTimeReset.Location = new System.Drawing.Point(54, 298);
			this.checkApptTimeReset.Name = "checkApptTimeReset";
			this.checkApptTimeReset.Size = new System.Drawing.Size(406, 17);
			this.checkApptTimeReset.TabIndex = 268;
			this.checkApptTimeReset.Text = "Reset calendar to today on Clinic select";
			this.checkApptTimeReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(48, 165);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(247, 15);
			this.label6.TabIndex = 258;
			this.label6.Text = "Time Dismissed trigger";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox2.Controls.Add(this.label37);
			this.groupBox2.Controls.Add(this.comboBrokenApptProc);
			this.groupBox2.Controls.Add(this.checkBrokenApptCommLog);
			this.groupBox2.Controls.Add(this.checkBrokenApptAdjustment);
			this.groupBox2.Controls.Add(this.comboBrokenApptAdjType);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new System.Drawing.Point(54, 17);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(418, 95);
			this.groupBox2.TabIndex = 267;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Broken Appointment Automation";
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(2, 15);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(240, 15);
			this.label37.TabIndex = 235;
			this.label37.Text = "Broken appointment procedure type";
			this.label37.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBrokenApptProc
			// 
			this.comboBrokenApptProc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBrokenApptProc.FormattingEnabled = true;
			this.comboBrokenApptProc.Location = new System.Drawing.Point(244, 11);
			this.comboBrokenApptProc.MaxDropDownItems = 30;
			this.comboBrokenApptProc.Name = "comboBrokenApptProc";
			this.comboBrokenApptProc.Size = new System.Drawing.Size(162, 21);
			this.comboBrokenApptProc.TabIndex = 234;
			// 
			// checkBrokenApptCommLog
			// 
			this.checkBrokenApptCommLog.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptCommLog.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBrokenApptCommLog.Location = new System.Drawing.Point(21, 33);
			this.checkBrokenApptCommLog.Name = "checkBrokenApptCommLog";
			this.checkBrokenApptCommLog.Size = new System.Drawing.Size(385, 17);
			this.checkBrokenApptCommLog.TabIndex = 61;
			this.checkBrokenApptCommLog.Text = "Make broken appointment commlog";
			this.checkBrokenApptCommLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptCommLog.UseVisualStyleBackColor = true;
			// 
			// checkBrokenApptAdjustment
			// 
			this.checkBrokenApptAdjustment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptAdjustment.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBrokenApptAdjustment.Location = new System.Drawing.Point(21, 49);
			this.checkBrokenApptAdjustment.Name = "checkBrokenApptAdjustment";
			this.checkBrokenApptAdjustment.Size = new System.Drawing.Size(385, 17);
			this.checkBrokenApptAdjustment.TabIndex = 217;
			this.checkBrokenApptAdjustment.Text = "Make broken appointment adjustment";
			this.checkBrokenApptAdjustment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptAdjustment.UseVisualStyleBackColor = true;
			// 
			// comboBrokenApptAdjType
			// 
			this.comboBrokenApptAdjType.Location = new System.Drawing.Point(204, 67);
			this.comboBrokenApptAdjType.Name = "comboBrokenApptAdjType";
			this.comboBrokenApptAdjType.Size = new System.Drawing.Size(203, 21);
			this.comboBrokenApptAdjType.TabIndex = 70;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 70);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(197, 15);
			this.label7.TabIndex = 71;
			this.label7.Text = "Broken appt default adj type";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textWaitRoomWarn
			// 
			this.textWaitRoomWarn.Location = new System.Drawing.Point(377, 272);
			this.textWaitRoomWarn.Name = "textWaitRoomWarn";
			this.textWaitRoomWarn.Size = new System.Drawing.Size(83, 20);
			this.textWaitRoomWarn.TabIndex = 266;
			// 
			// checkAppointmentTimeIsLocked
			// 
			this.checkAppointmentTimeIsLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAppointmentTimeIsLocked.Location = new System.Drawing.Point(246, 208);
			this.checkAppointmentTimeIsLocked.Name = "checkAppointmentTimeIsLocked";
			this.checkAppointmentTimeIsLocked.Size = new System.Drawing.Size(213, 17);
			this.checkAppointmentTimeIsLocked.TabIndex = 259;
			this.checkAppointmentTimeIsLocked.Text = "Appointment time locked by default";
			this.checkAppointmentTimeIsLocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAppointmentTimeIsLocked.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkAppointmentTimeIsLocked_MouseUp);
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(128, 275);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(246, 16);
			this.label22.TabIndex = 265;
			this.label22.Text = "Waiting room alert time in minutes (0 to disable)";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboSearchBehavior
			// 
			this.comboSearchBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSearchBehavior.FormattingEnabled = true;
			this.comboSearchBehavior.Location = new System.Drawing.Point(257, 183);
			this.comboSearchBehavior.MaxDropDownItems = 30;
			this.comboSearchBehavior.Name = "comboSearchBehavior";
			this.comboSearchBehavior.Size = new System.Drawing.Size(203, 21);
			this.comboSearchBehavior.TabIndex = 260;
			// 
			// textApptBubNoteLength
			// 
			this.textApptBubNoteLength.Location = new System.Drawing.Point(376, 227);
			this.textApptBubNoteLength.Name = "textApptBubNoteLength";
			this.textApptBubNoteLength.Size = new System.Drawing.Size(83, 20);
			this.textApptBubNoteLength.TabIndex = 264;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(39, 188);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(217, 15);
			this.label13.TabIndex = 261;
			this.label13.Text = "Search Behavior";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(128, 230);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(246, 16);
			this.label21.TabIndex = 263;
			this.label21.Text = "Appointment bubble max note length (0 for no limit)";
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkWaitingRoomFilterByView
			// 
			this.checkWaitingRoomFilterByView.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkWaitingRoomFilterByView.Location = new System.Drawing.Point(53, 250);
			this.checkWaitingRoomFilterByView.Name = "checkWaitingRoomFilterByView";
			this.checkWaitingRoomFilterByView.Size = new System.Drawing.Size(406, 17);
			this.checkWaitingRoomFilterByView.TabIndex = 262;
			this.checkWaitingRoomFilterByView.Text = "Filter the waiting room based on the selected appointment view";
			this.checkWaitingRoomFilterByView.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabFamily
			// 
			this.tabFamily.BackColor = System.Drawing.SystemColors.Control;
			this.tabFamily.Controls.Add(this.butSyncPhNums);
			this.tabFamily.Controls.Add(this.checkUsePhoneNumTable);
			this.tabFamily.Controls.Add(this.checkSameForFamily);
			this.tabFamily.Controls.Add(this.groupBoxOD1);
			this.tabFamily.Controls.Add(this.groupBoxCOB);
			this.tabFamily.Controls.Add(this.checkPatientDOBMasked);
			this.tabFamily.Controls.Add(this.checkPatientSSNMasked);
			this.tabFamily.Controls.Add(this.groupBoxClaimSnapshot);
			this.tabFamily.Controls.Add(this.groupBoxSuperFamily);
			this.tabFamily.Controls.Add(this.checkAllowPatsAtHQ);
			this.tabFamily.Controls.Add(this.checkAutoFillPatEmail);
			this.tabFamily.Controls.Add(this.checkPreferredReferrals);
			this.tabFamily.Controls.Add(this.checkTextMsgOkStatusTreatAsNo);
			this.tabFamily.Controls.Add(this.checkPatInitBillingTypeFromPriInsPlan);
			this.tabFamily.Controls.Add(this.checkFamPhiAccess);
			this.tabFamily.Controls.Add(this.checkSelectProv);
			this.tabFamily.Controls.Add(this.checkGoogleAddress);
			this.tabFamily.Location = new System.Drawing.Point(4, 22);
			this.tabFamily.Name = "tabFamily";
			this.tabFamily.Padding = new System.Windows.Forms.Padding(3);
			this.tabFamily.Size = new System.Drawing.Size(1227, 640);
			this.tabFamily.TabIndex = 1;
			this.tabFamily.Text = "Family";
			// 
			// butSyncPhNums
			// 
			this.butSyncPhNums.Location = new System.Drawing.Point(482, 585);
			this.butSyncPhNums.Name = "butSyncPhNums";
			this.butSyncPhNums.Size = new System.Drawing.Size(49, 21);
			this.butSyncPhNums.TabIndex = 294;
			this.butSyncPhNums.Text = "Sync";
			this.butSyncPhNums.Click += new System.EventHandler(this.butSyncPhNums_Click);
			// 
			// checkUsePhoneNumTable
			// 
			this.checkUsePhoneNumTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePhoneNumTable.Checked = true;
			this.checkUsePhoneNumTable.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkUsePhoneNumTable.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUsePhoneNumTable.Location = new System.Drawing.Point(70, 587);
			this.checkUsePhoneNumTable.Name = "checkUsePhoneNumTable";
			this.checkUsePhoneNumTable.Size = new System.Drawing.Size(406, 17);
			this.checkUsePhoneNumTable.TabIndex = 293;
			this.checkUsePhoneNumTable.Text = "Store patient phone numbers in a separate table for patient search";
			this.checkUsePhoneNumTable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSameForFamily
			// 
			this.checkSameForFamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSameForFamily.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSameForFamily.Location = new System.Drawing.Point(54, 564);
			this.checkSameForFamily.Name = "checkSameForFamily";
			this.checkSameForFamily.Size = new System.Drawing.Size(422, 17);
			this.checkSameForFamily.TabIndex = 284;
			this.checkSameForFamily.Text = "In Patient Edit window, checkboxes for \"Same for Entire Family\" default to unchec" +
    "ked";
			this.checkSameForFamily.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD1.Controls.Add(this.checkInsurancePlansShared);
			this.groupBoxOD1.Controls.Add(this.checkCoPayFeeScheduleBlankLikeZero);
			this.groupBoxOD1.Controls.Add(this.checkInsDefaultShowUCRonClaims);
			this.groupBoxOD1.Controls.Add(this.checkInsPPOsecWriteoffs);
			this.groupBoxOD1.Controls.Add(this.checkInsDefaultAssignmentOfBenefits);
			this.groupBoxOD1.Controls.Add(this.checkClaimUseOverrideProcDescript);
			this.groupBoxOD1.Controls.Add(this.checkInsPlanExclusionsUseUCR);
			this.groupBoxOD1.Controls.Add(this.checkPPOpercentage);
			this.groupBoxOD1.Controls.Add(this.checkInsPlanExclusionsMarkDoNotBill);
			this.groupBoxOD1.Controls.Add(this.checkClaimTrackingRequireError);
			this.groupBoxOD1.Controls.Add(this.checkFixedBenefitBlankLikeZero);
			this.groupBoxOD1.Location = new System.Drawing.Point(35, 24);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(456, 287);
			this.groupBoxOD1.TabIndex = 283;
			this.groupBoxOD1.TabStop = false;
			this.groupBoxOD1.Text = "Insurance";
			// 
			// checkInsurancePlansShared
			// 
			this.checkInsurancePlansShared.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsurancePlansShared.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsurancePlansShared.Location = new System.Drawing.Point(16, 18);
			this.checkInsurancePlansShared.Name = "checkInsurancePlansShared";
			this.checkInsurancePlansShared.Size = new System.Drawing.Size(425, 17);
			this.checkInsurancePlansShared.TabIndex = 257;
			this.checkInsurancePlansShared.Text = "InsPlan option at bottom, \'Change Plan for all subscribers\', is default";
			this.checkInsurancePlansShared.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkCoPayFeeScheduleBlankLikeZero
			// 
			this.checkCoPayFeeScheduleBlankLikeZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCoPayFeeScheduleBlankLikeZero.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCoPayFeeScheduleBlankLikeZero.Location = new System.Drawing.Point(16, 66);
			this.checkCoPayFeeScheduleBlankLikeZero.Name = "checkCoPayFeeScheduleBlankLikeZero";
			this.checkCoPayFeeScheduleBlankLikeZero.Size = new System.Drawing.Size(425, 17);
			this.checkCoPayFeeScheduleBlankLikeZero.TabIndex = 260;
			this.checkCoPayFeeScheduleBlankLikeZero.Text = "Copay fee schedules treat blank entries as zero";
			this.checkCoPayFeeScheduleBlankLikeZero.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsDefaultShowUCRonClaims
			// 
			this.checkInsDefaultShowUCRonClaims.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultShowUCRonClaims.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsDefaultShowUCRonClaims.Location = new System.Drawing.Point(16, 114);
			this.checkInsDefaultShowUCRonClaims.Name = "checkInsDefaultShowUCRonClaims";
			this.checkInsDefaultShowUCRonClaims.Size = new System.Drawing.Size(425, 17);
			this.checkInsDefaultShowUCRonClaims.TabIndex = 261;
			this.checkInsDefaultShowUCRonClaims.Text = "Insurance plans default to show UCR fee on claims";
			this.checkInsDefaultShowUCRonClaims.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultShowUCRonClaims.Click += new System.EventHandler(this.checkInsDefaultShowUCRonClaims_Click);
			// 
			// checkInsPPOsecWriteoffs
			// 
			this.checkInsPPOsecWriteoffs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPPOsecWriteoffs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsPPOsecWriteoffs.Location = new System.Drawing.Point(16, 162);
			this.checkInsPPOsecWriteoffs.Name = "checkInsPPOsecWriteoffs";
			this.checkInsPPOsecWriteoffs.Size = new System.Drawing.Size(425, 17);
			this.checkInsPPOsecWriteoffs.TabIndex = 270;
			this.checkInsPPOsecWriteoffs.Text = "Calculate secondary insurance PPO write-offs (not recommended, see manual)";
			this.checkInsPPOsecWriteoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPPOsecWriteoffs.UseVisualStyleBackColor = true;
			// 
			// checkInsDefaultAssignmentOfBenefits
			// 
			this.checkInsDefaultAssignmentOfBenefits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultAssignmentOfBenefits.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsDefaultAssignmentOfBenefits.Location = new System.Drawing.Point(16, 138);
			this.checkInsDefaultAssignmentOfBenefits.Name = "checkInsDefaultAssignmentOfBenefits";
			this.checkInsDefaultAssignmentOfBenefits.Size = new System.Drawing.Size(425, 17);
			this.checkInsDefaultAssignmentOfBenefits.TabIndex = 267;
			this.checkInsDefaultAssignmentOfBenefits.Text = "Insurance plans default to assignment of benefits";
			this.checkInsDefaultAssignmentOfBenefits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsDefaultAssignmentOfBenefits.Click += new System.EventHandler(this.checkInsDefaultAssignmentOfBenefits_Click);
			// 
			// checkClaimUseOverrideProcDescript
			// 
			this.checkClaimUseOverrideProcDescript.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimUseOverrideProcDescript.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimUseOverrideProcDescript.Location = new System.Drawing.Point(16, 186);
			this.checkClaimUseOverrideProcDescript.Name = "checkClaimUseOverrideProcDescript";
			this.checkClaimUseOverrideProcDescript.Size = new System.Drawing.Size(425, 17);
			this.checkClaimUseOverrideProcDescript.TabIndex = 263;
			this.checkClaimUseOverrideProcDescript.Text = "Use the description for the charted procedure code on printed claims";
			this.checkClaimUseOverrideProcDescript.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsPlanExclusionsUseUCR
			// 
			this.checkInsPlanExclusionsUseUCR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPlanExclusionsUseUCR.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsPlanExclusionsUseUCR.Location = new System.Drawing.Point(19, 234);
			this.checkInsPlanExclusionsUseUCR.Name = "checkInsPlanExclusionsUseUCR";
			this.checkInsPlanExclusionsUseUCR.Size = new System.Drawing.Size(422, 17);
			this.checkInsPlanExclusionsUseUCR.TabIndex = 277;
			this.checkInsPlanExclusionsUseUCR.Text = "Ins plans with exclusions use UCR fee (can be overridden by plan)";
			this.checkInsPlanExclusionsUseUCR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPPOpercentage
			// 
			this.checkPPOpercentage.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPPOpercentage.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPPOpercentage.Location = new System.Drawing.Point(16, 42);
			this.checkPPOpercentage.Name = "checkPPOpercentage";
			this.checkPPOpercentage.Size = new System.Drawing.Size(425, 17);
			this.checkPPOpercentage.TabIndex = 258;
			this.checkPPOpercentage.Text = "Default new insurance plans to PPO Percentage plan type";
			this.checkPPOpercentage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsPlanExclusionsMarkDoNotBill
			// 
			this.checkInsPlanExclusionsMarkDoNotBill.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPlanExclusionsMarkDoNotBill.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsPlanExclusionsMarkDoNotBill.Location = new System.Drawing.Point(19, 258);
			this.checkInsPlanExclusionsMarkDoNotBill.Name = "checkInsPlanExclusionsMarkDoNotBill";
			this.checkInsPlanExclusionsMarkDoNotBill.Size = new System.Drawing.Size(422, 17);
			this.checkInsPlanExclusionsMarkDoNotBill.TabIndex = 276;
			this.checkInsPlanExclusionsMarkDoNotBill.Text = "Ins plans with exclusions mark as Do Not Bill Ins";
			this.checkInsPlanExclusionsMarkDoNotBill.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimTrackingRequireError
			// 
			this.checkClaimTrackingRequireError.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimTrackingRequireError.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimTrackingRequireError.Location = new System.Drawing.Point(16, 210);
			this.checkClaimTrackingRequireError.Name = "checkClaimTrackingRequireError";
			this.checkClaimTrackingRequireError.Size = new System.Drawing.Size(425, 17);
			this.checkClaimTrackingRequireError.TabIndex = 266;
			this.checkClaimTrackingRequireError.Text = "Require error code when adding claim custom tracking status";
			this.checkClaimTrackingRequireError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkFixedBenefitBlankLikeZero
			// 
			this.checkFixedBenefitBlankLikeZero.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFixedBenefitBlankLikeZero.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkFixedBenefitBlankLikeZero.Location = new System.Drawing.Point(16, 90);
			this.checkFixedBenefitBlankLikeZero.Name = "checkFixedBenefitBlankLikeZero";
			this.checkFixedBenefitBlankLikeZero.Size = new System.Drawing.Size(425, 17);
			this.checkFixedBenefitBlankLikeZero.TabIndex = 275;
			this.checkFixedBenefitBlankLikeZero.Text = "Fixed benefit fee schedules treat blank entries as zero";
			this.checkFixedBenefitBlankLikeZero.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxCOB
			// 
			this.groupBoxCOB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxCOB.Controls.Add(this.comboCobSendPaidByInsAt);
			this.groupBoxCOB.Controls.Add(this.labelCobSendPaidByOtherInsAt);
			this.groupBoxCOB.Controls.Add(this.comboCobRule);
			this.groupBoxCOB.Controls.Add(this.labelCobRule);
			this.groupBoxCOB.Location = new System.Drawing.Point(693, 24);
			this.groupBoxCOB.Name = "groupBoxCOB";
			this.groupBoxCOB.Size = new System.Drawing.Size(383, 70);
			this.groupBoxCOB.TabIndex = 282;
			this.groupBoxCOB.TabStop = false;
			this.groupBoxCOB.Text = "Coordination of Benefits (COB)";
			// 
			// comboCobSendPaidByInsAt
			// 
			this.comboCobSendPaidByInsAt.Location = new System.Drawing.Point(249, 40);
			this.comboCobSendPaidByInsAt.Name = "comboCobSendPaidByInsAt";
			this.comboCobSendPaidByInsAt.Size = new System.Drawing.Size(128, 21);
			this.comboCobSendPaidByInsAt.TabIndex = 287;
			// 
			// labelCobSendPaidByOtherInsAt
			// 
			this.labelCobSendPaidByOtherInsAt.Location = new System.Drawing.Point(6, 41);
			this.labelCobSendPaidByOtherInsAt.Name = "labelCobSendPaidByOtherInsAt";
			this.labelCobSendPaidByOtherInsAt.Size = new System.Drawing.Size(242, 18);
			this.labelCobSendPaidByOtherInsAt.TabIndex = 286;
			this.labelCobSendPaidByOtherInsAt.Text = "Send Paid By Other Insurance At";
			this.labelCobSendPaidByOtherInsAt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCobRule
			// 
			this.comboCobRule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCobRule.FormattingEnabled = true;
			this.comboCobRule.Location = new System.Drawing.Point(249, 14);
			this.comboCobRule.MaxDropDownItems = 30;
			this.comboCobRule.Name = "comboCobRule";
			this.comboCobRule.Size = new System.Drawing.Size(128, 21);
			this.comboCobRule.TabIndex = 262;
			this.comboCobRule.SelectionChangeCommitted += new System.EventHandler(this.comboCobRule_SelectionChangeCommitted);
			// 
			// labelCobRule
			// 
			this.labelCobRule.Location = new System.Drawing.Point(6, 16);
			this.labelCobRule.Name = "labelCobRule";
			this.labelCobRule.Size = new System.Drawing.Size(242, 18);
			this.labelCobRule.TabIndex = 264;
			this.labelCobRule.Text = "Coordination of Benefits (COB) rule";
			this.labelCobRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatientDOBMasked
			// 
			this.checkPatientDOBMasked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientDOBMasked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientDOBMasked.Location = new System.Drawing.Point(54, 541);
			this.checkPatientDOBMasked.Name = "checkPatientDOBMasked";
			this.checkPatientDOBMasked.Size = new System.Drawing.Size(422, 17);
			this.checkPatientDOBMasked.TabIndex = 281;
			this.checkPatientDOBMasked.Text = "Mask patient date of birth";
			this.checkPatientDOBMasked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatientSSNMasked
			// 
			this.checkPatientSSNMasked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSSNMasked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientSSNMasked.Location = new System.Drawing.Point(54, 518);
			this.checkPatientSSNMasked.Name = "checkPatientSSNMasked";
			this.checkPatientSSNMasked.Size = new System.Drawing.Size(422, 17);
			this.checkPatientSSNMasked.TabIndex = 280;
			this.checkPatientSSNMasked.Text = "Mask patient Social Security Numbers";
			this.checkPatientSSNMasked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimSnapshot
			// 
			this.groupBoxClaimSnapshot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxClaimSnapshot.Controls.Add(this.comboClaimSnapshotTrigger);
			this.groupBoxClaimSnapshot.Controls.Add(this.textClaimSnapshotRunTime);
			this.groupBoxClaimSnapshot.Controls.Add(this.label30);
			this.groupBoxClaimSnapshot.Controls.Add(this.label31);
			this.groupBoxClaimSnapshot.Location = new System.Drawing.Point(693, 235);
			this.groupBoxClaimSnapshot.Name = "groupBoxClaimSnapshot";
			this.groupBoxClaimSnapshot.Size = new System.Drawing.Size(383, 71);
			this.groupBoxClaimSnapshot.TabIndex = 279;
			this.groupBoxClaimSnapshot.TabStop = false;
			this.groupBoxClaimSnapshot.Text = "Claim Snapshot";
			// 
			// comboClaimSnapshotTrigger
			// 
			this.comboClaimSnapshotTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClaimSnapshotTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClaimSnapshotTrigger.FormattingEnabled = true;
			this.comboClaimSnapshotTrigger.Location = new System.Drawing.Point(229, 16);
			this.comboClaimSnapshotTrigger.MaxDropDownItems = 30;
			this.comboClaimSnapshotTrigger.Name = "comboClaimSnapshotTrigger";
			this.comboClaimSnapshotTrigger.Size = new System.Drawing.Size(148, 21);
			this.comboClaimSnapshotTrigger.TabIndex = 221;
			// 
			// textClaimSnapshotRunTime
			// 
			this.textClaimSnapshotRunTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimSnapshotRunTime.Location = new System.Drawing.Point(267, 42);
			this.textClaimSnapshotRunTime.Name = "textClaimSnapshotRunTime";
			this.textClaimSnapshotRunTime.Size = new System.Drawing.Size(110, 20);
			this.textClaimSnapshotRunTime.TabIndex = 222;
			// 
			// label30
			// 
			this.label30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label30.Location = new System.Drawing.Point(101, 43);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(165, 17);
			this.label30.TabIndex = 223;
			this.label30.Text = "Service Run Time";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label31
			// 
			this.label31.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label31.Location = new System.Drawing.Point(101, 18);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(127, 17);
			this.label31.TabIndex = 224;
			this.label31.Text = "Claim Snapshot Trigger";
			this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxSuperFamily
			// 
			this.groupBoxSuperFamily.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxSuperFamily.Controls.Add(this.comboSuperFamSort);
			this.groupBoxSuperFamily.Controls.Add(this.labelSuperFamSort);
			this.groupBoxSuperFamily.Controls.Add(this.checkSuperFamSync);
			this.groupBoxSuperFamily.Controls.Add(this.checkSuperFamAddIns);
			this.groupBoxSuperFamily.Controls.Add(this.checkSuperFamCloneCreate);
			this.groupBoxSuperFamily.Location = new System.Drawing.Point(693, 109);
			this.groupBoxSuperFamily.Name = "groupBoxSuperFamily";
			this.groupBoxSuperFamily.Size = new System.Drawing.Size(383, 111);
			this.groupBoxSuperFamily.TabIndex = 278;
			this.groupBoxSuperFamily.TabStop = false;
			this.groupBoxSuperFamily.Text = "Super Family";
			// 
			// comboSuperFamSort
			// 
			this.comboSuperFamSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSuperFamSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSuperFamSort.FormattingEnabled = true;
			this.comboSuperFamSort.Location = new System.Drawing.Point(249, 15);
			this.comboSuperFamSort.MaxDropDownItems = 30;
			this.comboSuperFamSort.Name = "comboSuperFamSort";
			this.comboSuperFamSort.Size = new System.Drawing.Size(128, 21);
			this.comboSuperFamSort.TabIndex = 217;
			// 
			// labelSuperFamSort
			// 
			this.labelSuperFamSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSuperFamSort.Location = new System.Drawing.Point(94, 17);
			this.labelSuperFamSort.Name = "labelSuperFamSort";
			this.labelSuperFamSort.Size = new System.Drawing.Size(154, 17);
			this.labelSuperFamSort.TabIndex = 218;
			this.labelSuperFamSort.Text = "Super family sorting strategy";
			this.labelSuperFamSort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFamSync
			// 
			this.checkSuperFamSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamSync.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamSync.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperFamSync.Location = new System.Drawing.Point(62, 42);
			this.checkSuperFamSync.Name = "checkSuperFamSync";
			this.checkSuperFamSync.Size = new System.Drawing.Size(315, 17);
			this.checkSuperFamSync.TabIndex = 219;
			this.checkSuperFamSync.Text = "Allow syncing patient information to all super family members";
			this.checkSuperFamSync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFamAddIns
			// 
			this.checkSuperFamAddIns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamAddIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamAddIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperFamAddIns.Location = new System.Drawing.Point(6, 64);
			this.checkSuperFamAddIns.Name = "checkSuperFamAddIns";
			this.checkSuperFamAddIns.Size = new System.Drawing.Size(371, 17);
			this.checkSuperFamAddIns.TabIndex = 221;
			this.checkSuperFamAddIns.Text = "Copy super guarantor\'s primary insurance to all new super family members";
			this.checkSuperFamAddIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFamCloneCreate
			// 
			this.checkSuperFamCloneCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamCloneCreate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamCloneCreate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperFamCloneCreate.Location = new System.Drawing.Point(62, 86);
			this.checkSuperFamCloneCreate.Name = "checkSuperFamCloneCreate";
			this.checkSuperFamCloneCreate.Size = new System.Drawing.Size(315, 17);
			this.checkSuperFamCloneCreate.TabIndex = 227;
			this.checkSuperFamCloneCreate.Text = "New patient clones use super family instead of regular family";
			this.checkSuperFamCloneCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowPatsAtHQ
			// 
			this.checkAllowPatsAtHQ.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPatsAtHQ.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowPatsAtHQ.Location = new System.Drawing.Point(54, 495);
			this.checkAllowPatsAtHQ.Name = "checkAllowPatsAtHQ";
			this.checkAllowPatsAtHQ.Size = new System.Drawing.Size(422, 17);
			this.checkAllowPatsAtHQ.TabIndex = 274;
			this.checkAllowPatsAtHQ.Text = "Allow new patients to be added with an unassigned clinic";
			this.checkAllowPatsAtHQ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAutoFillPatEmail
			// 
			this.checkAutoFillPatEmail.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoFillPatEmail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAutoFillPatEmail.Location = new System.Drawing.Point(51, 472);
			this.checkAutoFillPatEmail.Name = "checkAutoFillPatEmail";
			this.checkAutoFillPatEmail.Size = new System.Drawing.Size(425, 17);
			this.checkAutoFillPatEmail.TabIndex = 273;
			this.checkAutoFillPatEmail.Text = "Autofill patient\'s email with the guarantor\'s when adding many new patients";
			this.checkAutoFillPatEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPreferredReferrals
			// 
			this.checkPreferredReferrals.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreferredReferrals.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPreferredReferrals.Location = new System.Drawing.Point(51, 449);
			this.checkPreferredReferrals.Name = "checkPreferredReferrals";
			this.checkPreferredReferrals.Size = new System.Drawing.Size(425, 17);
			this.checkPreferredReferrals.TabIndex = 272;
			this.checkPreferredReferrals.Text = "Show preferred referrals only in the Select Referral window by default";
			this.checkPreferredReferrals.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTextMsgOkStatusTreatAsNo
			// 
			this.checkTextMsgOkStatusTreatAsNo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTextMsgOkStatusTreatAsNo.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTextMsgOkStatusTreatAsNo.Location = new System.Drawing.Point(51, 334);
			this.checkTextMsgOkStatusTreatAsNo.Name = "checkTextMsgOkStatusTreatAsNo";
			this.checkTextMsgOkStatusTreatAsNo.Size = new System.Drawing.Size(425, 17);
			this.checkTextMsgOkStatusTreatAsNo.TabIndex = 265;
			this.checkTextMsgOkStatusTreatAsNo.Text = "Text Msg OK status, treat ?? as No instead of Yes";
			this.checkTextMsgOkStatusTreatAsNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatInitBillingTypeFromPriInsPlan
			// 
			this.checkPatInitBillingTypeFromPriInsPlan.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatInitBillingTypeFromPriInsPlan.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatInitBillingTypeFromPriInsPlan.Location = new System.Drawing.Point(51, 426);
			this.checkPatInitBillingTypeFromPriInsPlan.Name = "checkPatInitBillingTypeFromPriInsPlan";
			this.checkPatInitBillingTypeFromPriInsPlan.Size = new System.Drawing.Size(425, 17);
			this.checkPatInitBillingTypeFromPriInsPlan.TabIndex = 268;
			this.checkPatInitBillingTypeFromPriInsPlan.Text = "New patient primary insurance plan sets patient billing type";
			this.checkPatInitBillingTypeFromPriInsPlan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkFamPhiAccess
			// 
			this.checkFamPhiAccess.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFamPhiAccess.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkFamPhiAccess.Location = new System.Drawing.Point(51, 357);
			this.checkFamPhiAccess.Name = "checkFamPhiAccess";
			this.checkFamPhiAccess.Size = new System.Drawing.Size(425, 17);
			this.checkFamPhiAccess.TabIndex = 269;
			this.checkFamPhiAccess.Text = "Allow guarantor access to family health information in the Patient Portal";
			this.checkFamPhiAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSelectProv
			// 
			this.checkSelectProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSelectProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSelectProv.Location = new System.Drawing.Point(51, 403);
			this.checkSelectProv.Name = "checkSelectProv";
			this.checkSelectProv.Size = new System.Drawing.Size(425, 17);
			this.checkSelectProv.TabIndex = 256;
			this.checkSelectProv.Text = "Primary Provider defaults to \'Select Provider\' in Patient Edit and Add Family win" +
    "dows";
			this.checkSelectProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGoogleAddress
			// 
			this.checkGoogleAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGoogleAddress.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGoogleAddress.Location = new System.Drawing.Point(51, 380);
			this.checkGoogleAddress.Name = "checkGoogleAddress";
			this.checkGoogleAddress.Size = new System.Drawing.Size(425, 17);
			this.checkGoogleAddress.TabIndex = 271;
			this.checkGoogleAddress.Text = "Show Google Maps in Patient Edit window";
			this.checkGoogleAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabAccount
			// 
			this.tabAccount.BackColor = System.Drawing.SystemColors.Control;
			this.tabAccount.Controls.Add(this.label62);
			this.tabAccount.Controls.Add(this.groupBox10);
			this.tabAccount.Controls.Add(this.groupBox9);
			this.tabAccount.Controls.Add(this.checkAgingProcLifo);
			this.tabAccount.Controls.Add(this.groupBox7);
			this.tabAccount.Controls.Add(this.groupRepeatingCharges);
			this.tabAccount.Controls.Add(this.groupRecurringCharges);
			this.tabAccount.Controls.Add(this.checkBalancesDontSubtractIns);
			this.tabAccount.Controls.Add(this.checkAllowFutureTrans);
			this.tabAccount.Controls.Add(this.checkPpoUseUcr);
			this.tabAccount.Controls.Add(this.groupCommLogs);
			this.tabAccount.Controls.Add(this.checkAccountShowPaymentNums);
			this.tabAccount.Controls.Add(this.checkShowAllocateUnearnedPaymentPrompt);
			this.tabAccount.Controls.Add(this.checkAgingMonthly);
			this.tabAccount.Controls.Add(this.checkStatementInvoiceGridShowWriteoffs);
			this.tabAccount.Controls.Add(this.groupPayPlans);
			this.tabAccount.Location = new System.Drawing.Point(4, 22);
			this.tabAccount.Name = "tabAccount";
			this.tabAccount.Padding = new System.Windows.Forms.Padding(3);
			this.tabAccount.Size = new System.Drawing.Size(1227, 640);
			this.tabAccount.TabIndex = 2;
			this.tabAccount.Text = "Account";
			// 
			// label62
			// 
			this.label62.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label62.Location = new System.Drawing.Point(12, 478);
			this.label62.Name = "label62";
			this.label62.Size = new System.Drawing.Size(288, 48);
			this.label62.TabIndex = 308;
			this.label62.Text = "Allocation options for Line Item Accounting have been moved to a different window" +
    ".\r\nSee Main Menu, Setup, Account, Allocations.";
			this.label62.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox10
			// 
			this.groupBox10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox10.Controls.Add(this.comboRefundAdjustmentType);
			this.groupBox10.Controls.Add(this.labelRefundAdjustmentType);
			this.groupBox10.Controls.Add(this.comboLateChargeAdjType);
			this.groupBox10.Controls.Add(this.labelLateChargeAdjType);
			this.groupBox10.Controls.Add(this.groupBoxOD3);
			this.groupBox10.Controls.Add(this.comboFinanceChargeAdjType);
			this.groupBox10.Controls.Add(this.comboBillingChargeAdjType);
			this.groupBox10.Controls.Add(this.comboPayPlanAdj);
			this.groupBox10.Controls.Add(this.label42);
			this.groupBox10.Controls.Add(this.label4);
			this.groupBox10.Controls.Add(this.label12);
			this.groupBox10.Controls.Add(this.groupBox4);
			this.groupBox10.Location = new System.Drawing.Point(7, 180);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(399, 284);
			this.groupBox10.TabIndex = 302;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Adjustments";
			// 
			// comboRefundAdjustmentType
			// 
			this.comboRefundAdjustmentType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboRefundAdjustmentType.Location = new System.Drawing.Point(268, 93);
			this.comboRefundAdjustmentType.Name = "comboRefundAdjustmentType";
			this.comboRefundAdjustmentType.Size = new System.Drawing.Size(125, 21);
			this.comboRefundAdjustmentType.TabIndex = 306;
			// 
			// labelRefundAdjustmentType
			// 
			this.labelRefundAdjustmentType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRefundAdjustmentType.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelRefundAdjustmentType.Location = new System.Drawing.Point(158, 96);
			this.labelRefundAdjustmentType.Name = "labelRefundAdjustmentType";
			this.labelRefundAdjustmentType.Size = new System.Drawing.Size(108, 15);
			this.labelRefundAdjustmentType.TabIndex = 305;
			this.labelRefundAdjustmentType.Text = "Refund adj type";
			this.labelRefundAdjustmentType.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboLateChargeAdjType
			// 
			this.comboLateChargeAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboLateChargeAdjType.Location = new System.Drawing.Point(268, 49);
			this.comboLateChargeAdjType.Name = "comboLateChargeAdjType";
			this.comboLateChargeAdjType.Size = new System.Drawing.Size(125, 21);
			this.comboLateChargeAdjType.TabIndex = 304;
			// 
			// labelLateChargeAdjType
			// 
			this.labelLateChargeAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelLateChargeAdjType.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelLateChargeAdjType.Location = new System.Drawing.Point(156, 53);
			this.labelLateChargeAdjType.Name = "labelLateChargeAdjType";
			this.labelLateChargeAdjType.Size = new System.Drawing.Size(108, 15);
			this.labelLateChargeAdjType.TabIndex = 303;
			this.labelLateChargeAdjType.Text = "Late charge adj type";
			this.labelLateChargeAdjType.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBoxOD3
			// 
			this.groupBoxOD3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOD3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD3.Controls.Add(this.checkAutomateSalesTax);
			this.groupBoxOD3.Controls.Add(this.comboSalesTaxAdjType);
			this.groupBoxOD3.Controls.Add(this.label60);
			this.groupBoxOD3.Controls.Add(this.label26);
			this.groupBoxOD3.Controls.Add(this.textTaxPercent);
			this.groupBoxOD3.Controls.Add(this.label33);
			this.groupBoxOD3.Controls.Add(this.comboSalesTaxDefaultProvider);
			this.groupBoxOD3.Location = new System.Drawing.Point(66, 118);
			this.groupBoxOD3.Name = "groupBoxOD3";
			this.groupBoxOD3.Size = new System.Drawing.Size(328, 95);
			this.groupBoxOD3.TabIndex = 301;
			this.groupBoxOD3.TabStop = false;
			this.groupBoxOD3.Text = "Sales Tax";
			// 
			// checkAutomateSalesTax
			// 
			this.checkAutomateSalesTax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAutomateSalesTax.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutomateSalesTax.Location = new System.Drawing.Point(128, 71);
			this.checkAutomateSalesTax.Name = "checkAutomateSalesTax";
			this.checkAutomateSalesTax.Size = new System.Drawing.Size(195, 17);
			this.checkAutomateSalesTax.TabIndex = 305;
			this.checkAutomateSalesTax.Text = "Automate Sales Tax";
			this.checkAutomateSalesTax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutomateSalesTax.UseVisualStyleBackColor = true;
			// 
			// comboSalesTaxAdjType
			// 
			this.comboSalesTaxAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSalesTaxAdjType.Location = new System.Drawing.Point(197, 25);
			this.comboSalesTaxAdjType.Name = "comboSalesTaxAdjType";
			this.comboSalesTaxAdjType.Size = new System.Drawing.Size(125, 21);
			this.comboSalesTaxAdjType.TabIndex = 288;
			// 
			// label60
			// 
			this.label60.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label60.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label60.Location = new System.Drawing.Point(32, 50);
			this.label60.Name = "label60";
			this.label60.Size = new System.Drawing.Size(161, 15);
			this.label60.TabIndex = 306;
			this.label60.Text = "Sales Tax default provider";
			this.label60.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label26
			// 
			this.label26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label26.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label26.Location = new System.Drawing.Point(109, 6);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(155, 16);
			this.label26.TabIndex = 286;
			this.label26.Text = "Sales Tax percentage";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTaxPercent
			// 
			this.textTaxPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textTaxPercent.Location = new System.Drawing.Point(269, 4);
			this.textTaxPercent.Name = "textTaxPercent";
			this.textTaxPercent.Size = new System.Drawing.Size(53, 20);
			this.textTaxPercent.TabIndex = 285;
			// 
			// label33
			// 
			this.label33.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label33.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label33.Location = new System.Drawing.Point(85, 28);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(108, 15);
			this.label33.TabIndex = 287;
			this.label33.Text = "Sales Tax adj type";
			this.label33.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboSalesTaxDefaultProvider
			// 
			this.comboSalesTaxDefaultProvider.Location = new System.Drawing.Point(197, 47);
			this.comboSalesTaxDefaultProvider.Name = "comboSalesTaxDefaultProvider";
			this.comboSalesTaxDefaultProvider.Size = new System.Drawing.Size(125, 21);
			this.comboSalesTaxDefaultProvider.TabIndex = 303;
			this.comboSalesTaxDefaultProvider.Text = "comboBoxOD1";
			// 
			// comboFinanceChargeAdjType
			// 
			this.comboFinanceChargeAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboFinanceChargeAdjType.Location = new System.Drawing.Point(268, 5);
			this.comboFinanceChargeAdjType.Name = "comboFinanceChargeAdjType";
			this.comboFinanceChargeAdjType.Size = new System.Drawing.Size(125, 21);
			this.comboFinanceChargeAdjType.TabIndex = 276;
			// 
			// comboBillingChargeAdjType
			// 
			this.comboBillingChargeAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBillingChargeAdjType.Location = new System.Drawing.Point(268, 27);
			this.comboBillingChargeAdjType.Name = "comboBillingChargeAdjType";
			this.comboBillingChargeAdjType.Size = new System.Drawing.Size(125, 21);
			this.comboBillingChargeAdjType.TabIndex = 279;
			// 
			// comboPayPlanAdj
			// 
			this.comboPayPlanAdj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPayPlanAdj.Location = new System.Drawing.Point(268, 71);
			this.comboPayPlanAdj.Name = "comboPayPlanAdj";
			this.comboPayPlanAdj.Size = new System.Drawing.Size(125, 21);
			this.comboPayPlanAdj.TabIndex = 302;
			// 
			// label42
			// 
			this.label42.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label42.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label42.Location = new System.Drawing.Point(158, 74);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(108, 15);
			this.label42.TabIndex = 301;
			this.label42.Text = "Payment Plan adj type";
			this.label42.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label4.Location = new System.Drawing.Point(106, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(158, 15);
			this.label4.TabIndex = 278;
			this.label4.Text = "Finance charge adj type";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label12.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label12.Location = new System.Drawing.Point(156, 31);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(108, 15);
			this.label12.TabIndex = 277;
			this.label12.Text = "Billing charge adj type";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox4.Controls.Add(this.listboxBadDebtAdjs);
			this.groupBox4.Controls.Add(this.label29);
			this.groupBox4.Controls.Add(this.butBadDebt);
			this.groupBox4.Location = new System.Drawing.Point(67, 219);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(326, 59);
			this.groupBox4.TabIndex = 283;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Bad Debt Adjustments";
			// 
			// listboxBadDebtAdjs
			// 
			this.listboxBadDebtAdjs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listboxBadDebtAdjs.IntegralHeight = false;
			this.listboxBadDebtAdjs.Location = new System.Drawing.Point(201, 5);
			this.listboxBadDebtAdjs.Name = "listboxBadDebtAdjs";
			this.listboxBadDebtAdjs.Size = new System.Drawing.Size(120, 49);
			this.listboxBadDebtAdjs.TabIndex = 197;
			// 
			// label29
			// 
			this.label29.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label29.Location = new System.Drawing.Point(53, 12);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(147, 20);
			this.label29.TabIndex = 223;
			this.label29.Text = "Current Bad Debt adj types:";
			this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBadDebt
			// 
			this.butBadDebt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBadDebt.Location = new System.Drawing.Point(129, 33);
			this.butBadDebt.Name = "butBadDebt";
			this.butBadDebt.Size = new System.Drawing.Size(68, 21);
			this.butBadDebt.TabIndex = 197;
			this.butBadDebt.Text = "Edit";
			this.butBadDebt.Click += new System.EventHandler(this.butBadDebt_Click);
			// 
			// groupBox9
			// 
			this.groupBox9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox9.Controls.Add(this.checkIncTxfrTreatNegProdAsIncome);
			this.groupBox9.Controls.Add(this.checkStoreCCTokens);
			this.groupBox9.Controls.Add(this.comboPaymentClinicSetting);
			this.groupBox9.Controls.Add(this.label38);
			this.groupBox9.Controls.Add(this.checkPaymentsPromptForPayType);
			this.groupBox9.Controls.Add(this.checkAllowPrepayProvider);
			this.groupBox9.Controls.Add(this.comboUnallocatedSplits);
			this.groupBox9.Controls.Add(this.label28);
			this.groupBox9.Controls.Add(this.checkAllowFutureDebits);
			this.groupBox9.Controls.Add(this.checkAllowEmailCCReceipt);
			this.groupBox9.Location = new System.Drawing.Point(7, 8);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(399, 168);
			this.groupBox9.TabIndex = 301;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Payments";
			// 
			// checkIncTxfrTreatNegProdAsIncome
			// 
			this.checkIncTxfrTreatNegProdAsIncome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncTxfrTreatNegProdAsIncome.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncTxfrTreatNegProdAsIncome.Location = new System.Drawing.Point(50, 147);
			this.checkIncTxfrTreatNegProdAsIncome.Name = "checkIncTxfrTreatNegProdAsIncome";
			this.checkIncTxfrTreatNegProdAsIncome.Size = new System.Drawing.Size(344, 17);
			this.checkIncTxfrTreatNegProdAsIncome.TabIndex = 305;
			this.checkIncTxfrTreatNegProdAsIncome.Text = "Allow negative production to be transferred as income";
			this.checkIncTxfrTreatNegProdAsIncome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncTxfrTreatNegProdAsIncome.UseVisualStyleBackColor = true;
			// 
			// checkStoreCCTokens
			// 
			this.checkStoreCCTokens.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkStoreCCTokens.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStoreCCTokens.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStoreCCTokens.Location = new System.Drawing.Point(180, 3);
			this.checkStoreCCTokens.Name = "checkStoreCCTokens";
			this.checkStoreCCTokens.Size = new System.Drawing.Size(213, 17);
			this.checkStoreCCTokens.TabIndex = 280;
			this.checkStoreCCTokens.Text = "Automatically store credit card tokens";
			this.checkStoreCCTokens.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStoreCCTokens.UseVisualStyleBackColor = true;
			// 
			// comboPaymentClinicSetting
			// 
			this.comboPaymentClinicSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPaymentClinicSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPaymentClinicSetting.FormattingEnabled = true;
			this.comboPaymentClinicSetting.Location = new System.Drawing.Point(230, 23);
			this.comboPaymentClinicSetting.MaxDropDownItems = 30;
			this.comboPaymentClinicSetting.Name = "comboPaymentClinicSetting";
			this.comboPaymentClinicSetting.Size = new System.Drawing.Size(163, 21);
			this.comboPaymentClinicSetting.TabIndex = 290;
			// 
			// label38
			// 
			this.label38.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label38.Location = new System.Drawing.Point(103, 23);
			this.label38.Margin = new System.Windows.Forms.Padding(0);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(125, 21);
			this.label38.TabIndex = 291;
			this.label38.Text = "Patient Payments use";
			this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPaymentsPromptForPayType
			// 
			this.checkPaymentsPromptForPayType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPaymentsPromptForPayType.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPaymentsPromptForPayType.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPaymentsPromptForPayType.Location = new System.Drawing.Point(123, 48);
			this.checkPaymentsPromptForPayType.Name = "checkPaymentsPromptForPayType";
			this.checkPaymentsPromptForPayType.Size = new System.Drawing.Size(270, 17);
			this.checkPaymentsPromptForPayType.TabIndex = 284;
			this.checkPaymentsPromptForPayType.Text = "Payments prompt for Payment Type";
			this.checkPaymentsPromptForPayType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowPrepayProvider
			// 
			this.checkAllowPrepayProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowPrepayProvider.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPrepayProvider.Location = new System.Drawing.Point(184, 129);
			this.checkAllowPrepayProvider.Name = "checkAllowPrepayProvider";
			this.checkAllowPrepayProvider.Size = new System.Drawing.Size(210, 17);
			this.checkAllowPrepayProvider.TabIndex = 303;
			this.checkAllowPrepayProvider.Text = "Allow prepayments to providers";
			this.checkAllowPrepayProvider.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPrepayProvider.UseVisualStyleBackColor = true;
			// 
			// comboUnallocatedSplits
			// 
			this.comboUnallocatedSplits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboUnallocatedSplits.Location = new System.Drawing.Point(268, 69);
			this.comboUnallocatedSplits.Name = "comboUnallocatedSplits";
			this.comboUnallocatedSplits.Size = new System.Drawing.Size(125, 21);
			this.comboUnallocatedSplits.TabIndex = 281;
			// 
			// label28
			// 
			this.label28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label28.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label28.Location = new System.Drawing.Point(3, 73);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(261, 18);
			this.label28.TabIndex = 282;
			this.label28.Text = "Default unearned type for unallocated paysplits";
			this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowFutureDebits
			// 
			this.checkAllowFutureDebits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowFutureDebits.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFutureDebits.Location = new System.Drawing.Point(67, 95);
			this.checkAllowFutureDebits.Name = "checkAllowFutureDebits";
			this.checkAllowFutureDebits.Size = new System.Drawing.Size(327, 17);
			this.checkAllowFutureDebits.TabIndex = 289;
			this.checkAllowFutureDebits.Text = "Allow future dated payments (not recommended, see manual)";
			this.checkAllowFutureDebits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFutureDebits.UseVisualStyleBackColor = true;
			// 
			// checkAllowEmailCCReceipt
			// 
			this.checkAllowEmailCCReceipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowEmailCCReceipt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowEmailCCReceipt.Location = new System.Drawing.Point(184, 112);
			this.checkAllowEmailCCReceipt.Name = "checkAllowEmailCCReceipt";
			this.checkAllowEmailCCReceipt.Size = new System.Drawing.Size(210, 17);
			this.checkAllowEmailCCReceipt.TabIndex = 292;
			this.checkAllowEmailCCReceipt.Text = "Allow emailing credit card receipts";
			this.checkAllowEmailCCReceipt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowEmailCCReceipt.UseVisualStyleBackColor = true;
			// 
			// checkAgingProcLifo
			// 
			this.checkAgingProcLifo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingProcLifo.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgingProcLifo.Location = new System.Drawing.Point(860, 149);
			this.checkAgingProcLifo.Name = "checkAgingProcLifo";
			this.checkAgingProcLifo.Size = new System.Drawing.Size(351, 17);
			this.checkAgingProcLifo.TabIndex = 307;
			this.checkAgingProcLifo.Text = "Transactions attached to a procedure offset each other before aging";
			this.checkAgingProcLifo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingProcLifo.ThreeState = true;
			// 
			// groupBox7
			// 
			this.groupBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox7.Controls.Add(this.checkShowClaimPayTracking);
			this.groupBox7.Controls.Add(this.checkShowClaimPatResp);
			this.groupBox7.Controls.Add(this.checkPriClaimAllowSetToHoldUntilPriReceived);
			this.groupBox7.Controls.Add(this.checkCanadianPpoLabEst);
			this.groupBox7.Controls.Add(this.checkInsEstRecalcReceived);
			this.groupBox7.Controls.Add(this.checkPromptForSecondaryClaim);
			this.groupBox7.Controls.Add(this.checkInsPayNoWriteoffMoreThanProc);
			this.groupBox7.Controls.Add(this.checkClaimTrackingExcludeNone);
			this.groupBox7.Controls.Add(this.label55);
			this.groupBox7.Controls.Add(this.comboZeroDollarProcClaimBehavior);
			this.groupBox7.Controls.Add(this.labelClaimCredit);
			this.groupBox7.Controls.Add(this.comboClaimCredit);
			this.groupBox7.Controls.Add(this.checkAllowFuturePayments);
			this.groupBox7.Controls.Add(this.groupBoxClaimIdPrefix);
			this.groupBox7.Controls.Add(this.checkAllowProcAdjFromClaim);
			this.groupBox7.Controls.Add(this.checkProviderIncomeShows);
			this.groupBox7.Controls.Add(this.checkClaimFormTreatDentSaysSigOnFile);
			this.groupBox7.Controls.Add(this.checkClaimMedTypeIsInstWhenInsPlanIsMedical);
			this.groupBox7.Controls.Add(this.checkEclaimsMedicalProvTreatmentAsOrdering);
			this.groupBox7.Controls.Add(this.checkEclaimsSeparateTreatProv);
			this.groupBox7.Controls.Add(this.label20);
			this.groupBox7.Controls.Add(this.textClaimAttachPath);
			this.groupBox7.Controls.Add(this.checkClaimsValidateACN);
			this.groupBox7.Controls.Add(this.textInsWriteoffDescript);
			this.groupBox7.Controls.Add(this.label17);
			this.groupBox7.Location = new System.Drawing.Point(412, 8);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(399, 482);
			this.groupBox7.TabIndex = 304;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Insurance";
			// 
			// checkShowClaimPayTracking
			// 
			this.checkShowClaimPayTracking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowClaimPayTracking.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowClaimPayTracking.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowClaimPayTracking.Location = new System.Drawing.Point(26, 427);
			this.checkShowClaimPayTracking.Name = "checkShowClaimPayTracking";
			this.checkShowClaimPayTracking.Size = new System.Drawing.Size(367, 17);
			this.checkShowClaimPayTracking.TabIndex = 298;
			this.checkShowClaimPayTracking.Text = "Show Payment Tracking column in the Edit Claim/Payment windows";
			this.checkShowClaimPayTracking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowClaimPatResp
			// 
			this.checkShowClaimPatResp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowClaimPatResp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowClaimPatResp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowClaimPatResp.Location = new System.Drawing.Point(26, 444);
			this.checkShowClaimPatResp.Name = "checkShowClaimPatResp";
			this.checkShowClaimPatResp.Size = new System.Drawing.Size(367, 17);
			this.checkShowClaimPatResp.TabIndex = 299;
			this.checkShowClaimPatResp.Text = "Show Patient Responsibility column in the Edit Claim/Payment windows";
			this.checkShowClaimPatResp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPriClaimAllowSetToHoldUntilPriReceived
			// 
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPriClaimAllowSetToHoldUntilPriReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Location = new System.Drawing.Point(26, 410);
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Name = "checkPriClaimAllowSetToHoldUntilPriReceived";
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Size = new System.Drawing.Size(367, 17);
			this.checkPriClaimAllowSetToHoldUntilPriReceived.TabIndex = 297;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Text = "Allow primary claim status to be \'Hold Until Pri Received\'";
			this.checkPriClaimAllowSetToHoldUntilPriReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.UseVisualStyleBackColor = true;
			// 
			// checkCanadianPpoLabEst
			// 
			this.checkCanadianPpoLabEst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCanadianPpoLabEst.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCanadianPpoLabEst.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCanadianPpoLabEst.Location = new System.Drawing.Point(26, 461);
			this.checkCanadianPpoLabEst.Name = "checkCanadianPpoLabEst";
			this.checkCanadianPpoLabEst.Size = new System.Drawing.Size(367, 17);
			this.checkCanadianPpoLabEst.TabIndex = 300;
			this.checkCanadianPpoLabEst.Text = "Canadian PPO insurance plans create lab estimates";
			this.checkCanadianPpoLabEst.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCanadianPpoLabEst.UseVisualStyleBackColor = true;
			// 
			// checkInsEstRecalcReceived
			// 
			this.checkInsEstRecalcReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsEstRecalcReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsEstRecalcReceived.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsEstRecalcReceived.Location = new System.Drawing.Point(26, 393);
			this.checkInsEstRecalcReceived.Name = "checkInsEstRecalcReceived";
			this.checkInsEstRecalcReceived.Size = new System.Drawing.Size(367, 17);
			this.checkInsEstRecalcReceived.TabIndex = 296;
			this.checkInsEstRecalcReceived.Text = "Recalculate estimates for received claim procedures";
			this.checkInsEstRecalcReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPromptForSecondaryClaim
			// 
			this.checkPromptForSecondaryClaim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPromptForSecondaryClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptForSecondaryClaim.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPromptForSecondaryClaim.Location = new System.Drawing.Point(26, 376);
			this.checkPromptForSecondaryClaim.Name = "checkPromptForSecondaryClaim";
			this.checkPromptForSecondaryClaim.Size = new System.Drawing.Size(367, 17);
			this.checkPromptForSecondaryClaim.TabIndex = 295;
			this.checkPromptForSecondaryClaim.Text = "Prompt for secondary claims";
			this.checkPromptForSecondaryClaim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkInsPayNoWriteoffMoreThanProc
			// 
			this.checkInsPayNoWriteoffMoreThanProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkInsPayNoWriteoffMoreThanProc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsPayNoWriteoffMoreThanProc.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsPayNoWriteoffMoreThanProc.Location = new System.Drawing.Point(26, 359);
			this.checkInsPayNoWriteoffMoreThanProc.Name = "checkInsPayNoWriteoffMoreThanProc";
			this.checkInsPayNoWriteoffMoreThanProc.Size = new System.Drawing.Size(367, 17);
			this.checkInsPayNoWriteoffMoreThanProc.TabIndex = 294;
			this.checkInsPayNoWriteoffMoreThanProc.Text = "Disallow write-offs greater than the adjusted procedure fee";
			this.checkInsPayNoWriteoffMoreThanProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimTrackingExcludeNone
			// 
			this.checkClaimTrackingExcludeNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimTrackingExcludeNone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimTrackingExcludeNone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimTrackingExcludeNone.Location = new System.Drawing.Point(26, 342);
			this.checkClaimTrackingExcludeNone.Name = "checkClaimTrackingExcludeNone";
			this.checkClaimTrackingExcludeNone.Size = new System.Drawing.Size(367, 17);
			this.checkClaimTrackingExcludeNone.TabIndex = 293;
			this.checkClaimTrackingExcludeNone.Text = "Exclude \'None\' as an option on Custom Tracking Status";
			this.checkClaimTrackingExcludeNone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label55
			// 
			this.label55.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label55.Location = new System.Drawing.Point(42, 317);
			this.label55.Name = "label55";
			this.label55.Size = new System.Drawing.Size(180, 17);
			this.label55.TabIndex = 292;
			this.label55.Text = "Creating claims with $0 procedures";
			this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboZeroDollarProcClaimBehavior
			// 
			this.comboZeroDollarProcClaimBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboZeroDollarProcClaimBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboZeroDollarProcClaimBehavior.FormattingEnabled = true;
			this.comboZeroDollarProcClaimBehavior.Location = new System.Drawing.Point(225, 315);
			this.comboZeroDollarProcClaimBehavior.Name = "comboZeroDollarProcClaimBehavior";
			this.comboZeroDollarProcClaimBehavior.Size = new System.Drawing.Size(168, 21);
			this.comboZeroDollarProcClaimBehavior.TabIndex = 291;
			// 
			// labelClaimCredit
			// 
			this.labelClaimCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimCredit.Location = new System.Drawing.Point(24, 194);
			this.labelClaimCredit.Name = "labelClaimCredit";
			this.labelClaimCredit.Size = new System.Drawing.Size(196, 17);
			this.labelClaimCredit.TabIndex = 290;
			this.labelClaimCredit.Text = "Payment exceeds procedure balance";
			this.labelClaimCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClaimCredit
			// 
			this.comboClaimCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClaimCredit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClaimCredit.FormattingEnabled = true;
			this.comboClaimCredit.Location = new System.Drawing.Point(225, 190);
			this.comboClaimCredit.Name = "comboClaimCredit";
			this.comboClaimCredit.Size = new System.Drawing.Size(168, 21);
			this.comboClaimCredit.TabIndex = 289;
			// 
			// checkAllowFuturePayments
			// 
			this.checkAllowFuturePayments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowFuturePayments.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFuturePayments.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowFuturePayments.Location = new System.Drawing.Point(26, 216);
			this.checkAllowFuturePayments.Name = "checkAllowFuturePayments";
			this.checkAllowFuturePayments.Size = new System.Drawing.Size(367, 17);
			this.checkAllowFuturePayments.TabIndex = 288;
			this.checkAllowFuturePayments.Text = "Allow future payments";
			this.checkAllowFuturePayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimIdPrefix
			// 
			this.groupBoxClaimIdPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxClaimIdPrefix.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxClaimIdPrefix.Controls.Add(this.butReplacements);
			this.groupBoxClaimIdPrefix.Controls.Add(this.textClaimIdentifier);
			this.groupBoxClaimIdPrefix.Location = new System.Drawing.Point(214, 238);
			this.groupBoxClaimIdPrefix.Name = "groupBoxClaimIdPrefix";
			this.groupBoxClaimIdPrefix.Size = new System.Drawing.Size(179, 71);
			this.groupBoxClaimIdPrefix.TabIndex = 287;
			this.groupBoxClaimIdPrefix.TabStop = false;
			this.groupBoxClaimIdPrefix.Text = "Claim Identification Prefix";
			// 
			// butReplacements
			// 
			this.butReplacements.Location = new System.Drawing.Point(68, 42);
			this.butReplacements.Name = "butReplacements";
			this.butReplacements.Size = new System.Drawing.Size(107, 23);
			this.butReplacements.TabIndex = 240;
			this.butReplacements.Text = "Replacements";
			this.butReplacements.UseVisualStyleBackColor = true;
			this.butReplacements.Click += new System.EventHandler(this.butReplacements_Click);
			// 
			// textClaimIdentifier
			// 
			this.textClaimIdentifier.Location = new System.Drawing.Point(4, 19);
			this.textClaimIdentifier.Name = "textClaimIdentifier";
			this.textClaimIdentifier.Size = new System.Drawing.Size(171, 20);
			this.textClaimIdentifier.TabIndex = 238;
			// 
			// checkAllowProcAdjFromClaim
			// 
			this.checkAllowProcAdjFromClaim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowProcAdjFromClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowProcAdjFromClaim.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowProcAdjFromClaim.Location = new System.Drawing.Point(26, 168);
			this.checkAllowProcAdjFromClaim.Name = "checkAllowProcAdjFromClaim";
			this.checkAllowProcAdjFromClaim.Size = new System.Drawing.Size(367, 17);
			this.checkAllowProcAdjFromClaim.TabIndex = 286;
			this.checkAllowProcAdjFromClaim.Text = "Allow procedure adjustments from Edit Claim window";
			this.checkAllowProcAdjFromClaim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkProviderIncomeShows
			// 
			this.checkProviderIncomeShows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProviderIncomeShows.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProviderIncomeShows.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProviderIncomeShows.Location = new System.Drawing.Point(26, 19);
			this.checkProviderIncomeShows.Name = "checkProviderIncomeShows";
			this.checkProviderIncomeShows.Size = new System.Drawing.Size(367, 17);
			this.checkProviderIncomeShows.TabIndex = 277;
			this.checkProviderIncomeShows.Text = "Show provider income transfer window after entering insurance payment";
			this.checkProviderIncomeShows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimFormTreatDentSaysSigOnFile
			// 
			this.checkClaimFormTreatDentSaysSigOnFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimFormTreatDentSaysSigOnFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimFormTreatDentSaysSigOnFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimFormTreatDentSaysSigOnFile.Location = new System.Drawing.Point(26, 53);
			this.checkClaimFormTreatDentSaysSigOnFile.Name = "checkClaimFormTreatDentSaysSigOnFile";
			this.checkClaimFormTreatDentSaysSigOnFile.Size = new System.Drawing.Size(367, 17);
			this.checkClaimFormTreatDentSaysSigOnFile.TabIndex = 282;
			this.checkClaimFormTreatDentSaysSigOnFile.Text = "Claim form treating provider shows \'Signature On File\' rather than name";
			this.checkClaimFormTreatDentSaysSigOnFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimMedTypeIsInstWhenInsPlanIsMedical
			// 
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Location = new System.Drawing.Point(26, 36);
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Name = "checkClaimMedTypeIsInstWhenInsPlanIsMedical";
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Size = new System.Drawing.Size(367, 17);
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.TabIndex = 281;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Text = "Set medical claims to institutional when using medical insurance";
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEclaimsMedicalProvTreatmentAsOrdering
			// 
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEclaimsMedicalProvTreatmentAsOrdering.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Location = new System.Drawing.Point(23, 117);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Name = "checkEclaimsMedicalProvTreatmentAsOrdering";
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Size = new System.Drawing.Size(370, 17);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.TabIndex = 285;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Text = "On medical e-claims, send treating provider as ordering provider by default";
			this.checkEclaimsMedicalProvTreatmentAsOrdering.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEclaimsSeparateTreatProv
			// 
			this.checkEclaimsSeparateTreatProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkEclaimsSeparateTreatProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsSeparateTreatProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEclaimsSeparateTreatProv.Location = new System.Drawing.Point(26, 134);
			this.checkEclaimsSeparateTreatProv.Name = "checkEclaimsSeparateTreatProv";
			this.checkEclaimsSeparateTreatProv.Size = new System.Drawing.Size(367, 17);
			this.checkEclaimsSeparateTreatProv.TabIndex = 276;
			this.checkEclaimsSeparateTreatProv.Text = "On e-claims, send treating provider info for each separate procedure";
			this.checkEclaimsSeparateTreatProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label20.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label20.Location = new System.Drawing.Point(43, 99);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(147, 17);
			this.label20.TabIndex = 279;
			this.label20.Text = "Claim attachment export path";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClaimAttachPath
			// 
			this.textClaimAttachPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimAttachPath.Location = new System.Drawing.Point(196, 95);
			this.textClaimAttachPath.Name = "textClaimAttachPath";
			this.textClaimAttachPath.Size = new System.Drawing.Size(197, 20);
			this.textClaimAttachPath.TabIndex = 278;
			// 
			// checkClaimsValidateACN
			// 
			this.checkClaimsValidateACN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimsValidateACN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimsValidateACN.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimsValidateACN.Location = new System.Drawing.Point(26, 151);
			this.checkClaimsValidateACN.Name = "checkClaimsValidateACN";
			this.checkClaimsValidateACN.Size = new System.Drawing.Size(367, 17);
			this.checkClaimsValidateACN.TabIndex = 280;
			this.checkClaimsValidateACN.Text = "Require ACN# in remarks on claims with ADDP group name";
			this.checkClaimsValidateACN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsWriteoffDescript
			// 
			this.textInsWriteoffDescript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsWriteoffDescript.Location = new System.Drawing.Point(263, 73);
			this.textInsWriteoffDescript.Name = "textInsWriteoffDescript";
			this.textInsWriteoffDescript.Size = new System.Drawing.Size(130, 20);
			this.textInsWriteoffDescript.TabIndex = 283;
			// 
			// label17
			// 
			this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label17.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label17.Location = new System.Drawing.Point(6, 76);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(254, 18);
			this.label17.TabIndex = 284;
			this.label17.Text = "PPO write-off description (blank for \"Write-off\")";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupRepeatingCharges
			// 
			this.groupRepeatingCharges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupRepeatingCharges.Controls.Add(this.labelRepeatingChargesAutomatedTime);
			this.groupRepeatingCharges.Controls.Add(this.textRepeatingChargesAutomatedTime);
			this.groupRepeatingCharges.Controls.Add(this.checkRepeatingChargesRunAging);
			this.groupRepeatingCharges.Controls.Add(this.checkRepeatingChargesAutomated);
			this.groupRepeatingCharges.Location = new System.Drawing.Point(818, 558);
			this.groupRepeatingCharges.Name = "groupRepeatingCharges";
			this.groupRepeatingCharges.Size = new System.Drawing.Size(399, 78);
			this.groupRepeatingCharges.TabIndex = 245;
			this.groupRepeatingCharges.TabStop = false;
			this.groupRepeatingCharges.Text = "Repeating Charges";
			// 
			// labelRepeatingChargesAutomatedTime
			// 
			this.labelRepeatingChargesAutomatedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRepeatingChargesAutomatedTime.Enabled = false;
			this.labelRepeatingChargesAutomatedTime.Location = new System.Drawing.Point(169, 53);
			this.labelRepeatingChargesAutomatedTime.Name = "labelRepeatingChargesAutomatedTime";
			this.labelRepeatingChargesAutomatedTime.Size = new System.Drawing.Size(154, 17);
			this.labelRepeatingChargesAutomatedTime.TabIndex = 243;
			this.labelRepeatingChargesAutomatedTime.Text = "Repeating charges run time";
			this.labelRepeatingChargesAutomatedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRepeatingChargesAutomatedTime
			// 
			this.textRepeatingChargesAutomatedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRepeatingChargesAutomatedTime.Enabled = false;
			this.textRepeatingChargesAutomatedTime.Location = new System.Drawing.Point(325, 52);
			this.textRepeatingChargesAutomatedTime.Name = "textRepeatingChargesAutomatedTime";
			this.textRepeatingChargesAutomatedTime.Size = new System.Drawing.Size(68, 20);
			this.textRepeatingChargesAutomatedTime.TabIndex = 243;
			this.textRepeatingChargesAutomatedTime.Leave += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// checkRepeatingChargesRunAging
			// 
			this.checkRepeatingChargesRunAging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRepeatingChargesRunAging.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatingChargesRunAging.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRepeatingChargesRunAging.Location = new System.Drawing.Point(114, 14);
			this.checkRepeatingChargesRunAging.Name = "checkRepeatingChargesRunAging";
			this.checkRepeatingChargesRunAging.Size = new System.Drawing.Size(279, 17);
			this.checkRepeatingChargesRunAging.TabIndex = 239;
			this.checkRepeatingChargesRunAging.Text = "Repeating charges runs aging after posting charges";
			this.checkRepeatingChargesRunAging.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRepeatingChargesAutomated
			// 
			this.checkRepeatingChargesAutomated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRepeatingChargesAutomated.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatingChargesAutomated.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRepeatingChargesAutomated.Location = new System.Drawing.Point(167, 32);
			this.checkRepeatingChargesAutomated.Name = "checkRepeatingChargesAutomated";
			this.checkRepeatingChargesAutomated.Size = new System.Drawing.Size(226, 17);
			this.checkRepeatingChargesAutomated.TabIndex = 238;
			this.checkRepeatingChargesAutomated.Text = "Repeating charges run automatically";
			this.checkRepeatingChargesAutomated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatingChargesAutomated.CheckedChanged += new System.EventHandler(this.checkRepeatingChargesAutomated_CheckedChanged);
			this.checkRepeatingChargesAutomated.Click += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// groupRecurringCharges
			// 
			this.groupRecurringCharges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesShowInactive);
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesInactivateDeclinedCards);
			this.groupRecurringCharges.Controls.Add(this.checkRecurPatBal0);
			this.groupRecurringCharges.Controls.Add(this.label56);
			this.groupRecurringCharges.Controls.Add(this.comboRecurringChargePayType);
			this.groupRecurringCharges.Controls.Add(this.labelRecurringChargesAutomatedTime);
			this.groupRecurringCharges.Controls.Add(this.textRecurringChargesTime);
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesAutomated);
			this.groupRecurringCharges.Controls.Add(this.checkRecurringChargesUseTransDate);
			this.groupRecurringCharges.Controls.Add(this.checkRecurChargPriProv);
			this.groupRecurringCharges.Location = new System.Drawing.Point(818, 381);
			this.groupRecurringCharges.Name = "groupRecurringCharges";
			this.groupRecurringCharges.Size = new System.Drawing.Size(399, 173);
			this.groupRecurringCharges.TabIndex = 244;
			this.groupRecurringCharges.TabStop = false;
			this.groupRecurringCharges.Text = "Recurring Charges";
			// 
			// checkRecurringChargesShowInactive
			// 
			this.checkRecurringChargesShowInactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesShowInactive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesShowInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurringChargesShowInactive.Location = new System.Drawing.Point(128, 48);
			this.checkRecurringChargesShowInactive.Name = "checkRecurringChargesShowInactive";
			this.checkRecurringChargesShowInactive.Size = new System.Drawing.Size(265, 17);
			this.checkRecurringChargesShowInactive.TabIndex = 241;
			this.checkRecurringChargesShowInactive.Text = "Recurring charges show inactive charges by default";
			this.checkRecurringChargesShowInactive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRecurringChargesInactivateDeclinedCards
			// 
			this.checkRecurringChargesInactivateDeclinedCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesInactivateDeclinedCards.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesInactivateDeclinedCards.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurringChargesInactivateDeclinedCards.Location = new System.Drawing.Point(167, 154);
			this.checkRecurringChargesInactivateDeclinedCards.Name = "checkRecurringChargesInactivateDeclinedCards";
			this.checkRecurringChargesInactivateDeclinedCards.Size = new System.Drawing.Size(226, 17);
			this.checkRecurringChargesInactivateDeclinedCards.TabIndex = 254;
			this.checkRecurringChargesInactivateDeclinedCards.Text = "Automatically inactivate declined cards";
			this.checkRecurringChargesInactivateDeclinedCards.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRecurPatBal0
			// 
			this.checkRecurPatBal0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurPatBal0.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurPatBal0.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurPatBal0.Location = new System.Drawing.Point(44, 136);
			this.checkRecurPatBal0.Name = "checkRecurPatBal0";
			this.checkRecurPatBal0.Size = new System.Drawing.Size(349, 17);
			this.checkRecurPatBal0.TabIndex = 246;
			this.checkRecurPatBal0.Text = "Allow recurring charges to run in the absence of a patient balance";
			this.checkRecurPatBal0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurPatBal0.UseVisualStyleBackColor = true;
			// 
			// label56
			// 
			this.label56.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label56.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label56.Location = new System.Drawing.Point(135, 114);
			this.label56.Name = "label56";
			this.label56.Size = new System.Drawing.Size(90, 15);
			this.label56.TabIndex = 253;
			this.label56.Text = "Pay type for CC";
			this.label56.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboRecurringChargePayType
			// 
			this.comboRecurringChargePayType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboRecurringChargePayType.Location = new System.Drawing.Point(230, 111);
			this.comboRecurringChargePayType.Name = "comboRecurringChargePayType";
			this.comboRecurringChargePayType.Size = new System.Drawing.Size(163, 21);
			this.comboRecurringChargePayType.TabIndex = 252;
			// 
			// labelRecurringChargesAutomatedTime
			// 
			this.labelRecurringChargesAutomatedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRecurringChargesAutomatedTime.Enabled = false;
			this.labelRecurringChargesAutomatedTime.Location = new System.Drawing.Point(164, 85);
			this.labelRecurringChargesAutomatedTime.Name = "labelRecurringChargesAutomatedTime";
			this.labelRecurringChargesAutomatedTime.Size = new System.Drawing.Size(159, 17);
			this.labelRecurringChargesAutomatedTime.TabIndex = 243;
			this.labelRecurringChargesAutomatedTime.Text = "Recurring charges run time";
			this.labelRecurringChargesAutomatedTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRecurringChargesTime
			// 
			this.textRecurringChargesTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textRecurringChargesTime.Enabled = false;
			this.textRecurringChargesTime.Location = new System.Drawing.Point(325, 86);
			this.textRecurringChargesTime.Name = "textRecurringChargesTime";
			this.textRecurringChargesTime.Size = new System.Drawing.Size(68, 20);
			this.textRecurringChargesTime.TabIndex = 242;
			this.textRecurringChargesTime.Leave += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// checkRecurringChargesAutomated
			// 
			this.checkRecurringChargesAutomated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesAutomated.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesAutomated.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurringChargesAutomated.Location = new System.Drawing.Point(167, 66);
			this.checkRecurringChargesAutomated.Name = "checkRecurringChargesAutomated";
			this.checkRecurringChargesAutomated.Size = new System.Drawing.Size(226, 17);
			this.checkRecurringChargesAutomated.TabIndex = 240;
			this.checkRecurringChargesAutomated.Text = "Recurring charges run automatically";
			this.checkRecurringChargesAutomated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesAutomated.CheckedChanged += new System.EventHandler(this.checkRecurringChargesAutomated_CheckedChanged);
			this.checkRecurringChargesAutomated.Click += new System.EventHandler(this.PromptRecurringRepeatingChargesTimes);
			// 
			// checkRecurringChargesUseTransDate
			// 
			this.checkRecurringChargesUseTransDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurringChargesUseTransDate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurringChargesUseTransDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurringChargesUseTransDate.Location = new System.Drawing.Point(167, 30);
			this.checkRecurringChargesUseTransDate.Name = "checkRecurringChargesUseTransDate";
			this.checkRecurringChargesUseTransDate.Size = new System.Drawing.Size(226, 17);
			this.checkRecurringChargesUseTransDate.TabIndex = 239;
			this.checkRecurringChargesUseTransDate.Text = "Recurring charges use transaction date";
			this.checkRecurringChargesUseTransDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRecurChargPriProv
			// 
			this.checkRecurChargPriProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkRecurChargPriProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRecurChargPriProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurChargPriProv.Location = new System.Drawing.Point(167, 12);
			this.checkRecurChargPriProv.Name = "checkRecurChargPriProv";
			this.checkRecurChargPriProv.Size = new System.Drawing.Size(226, 17);
			this.checkRecurChargPriProv.TabIndex = 238;
			this.checkRecurChargPriProv.Text = "Recurring charges use primary provider";
			this.checkRecurChargPriProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBalancesDontSubtractIns
			// 
			this.checkBalancesDontSubtractIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBalancesDontSubtractIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBalancesDontSubtractIns.Location = new System.Drawing.Point(860, 8);
			this.checkBalancesDontSubtractIns.Name = "checkBalancesDontSubtractIns";
			this.checkBalancesDontSubtractIns.Size = new System.Drawing.Size(351, 17);
			this.checkBalancesDontSubtractIns.TabIndex = 55;
			this.checkBalancesDontSubtractIns.Text = "Balances don\'t subtract insurance estimate";
			this.checkBalancesDontSubtractIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowFutureTrans
			// 
			this.checkAllowFutureTrans.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowFutureTrans.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowFutureTrans.Location = new System.Drawing.Point(860, 131);
			this.checkAllowFutureTrans.Name = "checkAllowFutureTrans";
			this.checkAllowFutureTrans.Size = new System.Drawing.Size(351, 17);
			this.checkAllowFutureTrans.TabIndex = 244;
			this.checkAllowFutureTrans.Text = "Allow future dated transactions";
			this.checkAllowFutureTrans.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPpoUseUcr
			// 
			this.checkPpoUseUcr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPpoUseUcr.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPpoUseUcr.Location = new System.Drawing.Point(860, 77);
			this.checkPpoUseUcr.Name = "checkPpoUseUcr";
			this.checkPpoUseUcr.Size = new System.Drawing.Size(351, 17);
			this.checkPpoUseUcr.TabIndex = 228;
			this.checkPpoUseUcr.Text = "Use UCR fee for billed fee even if PPO fee is higher";
			this.checkPpoUseUcr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupCommLogs
			// 
			this.groupCommLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupCommLogs.Controls.Add(this.checkCommLogAutoSave);
			this.groupCommLogs.Controls.Add(this.checkShowFamilyCommByDefault);
			this.groupCommLogs.Location = new System.Drawing.Point(818, 323);
			this.groupCommLogs.Name = "groupCommLogs";
			this.groupCommLogs.Size = new System.Drawing.Size(399, 54);
			this.groupCommLogs.TabIndex = 243;
			this.groupCommLogs.TabStop = false;
			this.groupCommLogs.Text = "Commlogs";
			// 
			// checkCommLogAutoSave
			// 
			this.checkCommLogAutoSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCommLogAutoSave.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCommLogAutoSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCommLogAutoSave.Location = new System.Drawing.Point(188, 12);
			this.checkCommLogAutoSave.Name = "checkCommLogAutoSave";
			this.checkCommLogAutoSave.Size = new System.Drawing.Size(205, 17);
			this.checkCommLogAutoSave.TabIndex = 225;
			this.checkCommLogAutoSave.Text = "Commlogs auto save";
			this.checkCommLogAutoSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCommLogAutoSave.UseVisualStyleBackColor = true;
			// 
			// checkShowFamilyCommByDefault
			// 
			this.checkShowFamilyCommByDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowFamilyCommByDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFamilyCommByDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowFamilyCommByDefault.Location = new System.Drawing.Point(184, 30);
			this.checkShowFamilyCommByDefault.Name = "checkShowFamilyCommByDefault";
			this.checkShowFamilyCommByDefault.Size = new System.Drawing.Size(209, 17);
			this.checkShowFamilyCommByDefault.TabIndex = 75;
			this.checkShowFamilyCommByDefault.Text = "Show family commlog entries by default";
			this.checkShowFamilyCommByDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFamilyCommByDefault.Click += new System.EventHandler(this.checkShowFamilyCommByDefault_Click);
			// 
			// checkAccountShowPaymentNums
			// 
			this.checkAccountShowPaymentNums.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAccountShowPaymentNums.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAccountShowPaymentNums.Location = new System.Drawing.Point(860, 59);
			this.checkAccountShowPaymentNums.Name = "checkAccountShowPaymentNums";
			this.checkAccountShowPaymentNums.Size = new System.Drawing.Size(351, 17);
			this.checkAccountShowPaymentNums.TabIndex = 194;
			this.checkAccountShowPaymentNums.Text = "Show payment numbers in Account Module";
			this.checkAccountShowPaymentNums.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowAllocateUnearnedPaymentPrompt
			// 
			this.checkShowAllocateUnearnedPaymentPrompt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAllocateUnearnedPaymentPrompt.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowAllocateUnearnedPaymentPrompt.Location = new System.Drawing.Point(860, 113);
			this.checkShowAllocateUnearnedPaymentPrompt.Name = "checkShowAllocateUnearnedPaymentPrompt";
			this.checkShowAllocateUnearnedPaymentPrompt.Size = new System.Drawing.Size(351, 17);
			this.checkShowAllocateUnearnedPaymentPrompt.TabIndex = 242;
			this.checkShowAllocateUnearnedPaymentPrompt.Text = "Prompt user to allocate unearned income after creating a claim";
			this.checkShowAllocateUnearnedPaymentPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAgingMonthly
			// 
			this.checkAgingMonthly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingMonthly.Enabled = false;
			this.checkAgingMonthly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgingMonthly.Location = new System.Drawing.Point(860, 24);
			this.checkAgingMonthly.Name = "checkAgingMonthly";
			this.checkAgingMonthly.Size = new System.Drawing.Size(351, 35);
			this.checkAgingMonthly.TabIndex = 57;
			this.checkAgingMonthly.Text = "Aging calculated monthly instead of daily (not available\r\nwith enterprise aging)";
			this.checkAgingMonthly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkStatementInvoiceGridShowWriteoffs
			// 
			this.checkStatementInvoiceGridShowWriteoffs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementInvoiceGridShowWriteoffs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatementInvoiceGridShowWriteoffs.Location = new System.Drawing.Point(860, 95);
			this.checkStatementInvoiceGridShowWriteoffs.Name = "checkStatementInvoiceGridShowWriteoffs";
			this.checkStatementInvoiceGridShowWriteoffs.Size = new System.Drawing.Size(351, 17);
			this.checkStatementInvoiceGridShowWriteoffs.TabIndex = 238;
			this.checkStatementInvoiceGridShowWriteoffs.Text = "Invoices\' payments grid shows write-offs\r\n";
			this.checkStatementInvoiceGridShowWriteoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupPayPlans
			// 
			this.groupPayPlans.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupPayPlans.Controls.Add(this.label39);
			this.groupPayPlans.Controls.Add(this.comboDppUnearnedType);
			this.groupPayPlans.Controls.Add(this.label59);
			this.groupPayPlans.Controls.Add(this.textDynamicPayPlan);
			this.groupPayPlans.Controls.Add(this.label27);
			this.groupPayPlans.Controls.Add(this.comboPayPlansVersion);
			this.groupPayPlans.Controls.Add(this.checkHideDueNow);
			this.groupPayPlans.Controls.Add(this.checkPayPlansUseSheets);
			this.groupPayPlans.Controls.Add(this.checkPayPlansExcludePastActivity);
			this.groupPayPlans.Location = new System.Drawing.Point(818, 172);
			this.groupPayPlans.Name = "groupPayPlans";
			this.groupPayPlans.Size = new System.Drawing.Size(399, 147);
			this.groupPayPlans.TabIndex = 240;
			this.groupPayPlans.TabStop = false;
			this.groupPayPlans.Text = "Pay Plans";
			// 
			// label39
			// 
			this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label39.Location = new System.Drawing.Point(3, 121);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(195, 17);
			this.label39.TabIndex = 249;
			this.label39.Text = "Dynamic payment plan prepayment type";
			this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDppUnearnedType
			// 
			this.comboDppUnearnedType.Location = new System.Drawing.Point(199, 120);
			this.comboDppUnearnedType.Name = "comboDppUnearnedType";
			this.comboDppUnearnedType.Size = new System.Drawing.Size(194, 21);
			this.comboDppUnearnedType.TabIndex = 248;
			this.comboDppUnearnedType.Text = "comboBoxOD1";
			// 
			// label59
			// 
			this.label59.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label59.Location = new System.Drawing.Point(169, 98);
			this.label59.Name = "label59";
			this.label59.Size = new System.Drawing.Size(154, 17);
			this.label59.TabIndex = 245;
			this.label59.Text = "Dynamic Pay Plan run time";
			this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDynamicPayPlan
			// 
			this.textDynamicPayPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDynamicPayPlan.Location = new System.Drawing.Point(325, 97);
			this.textDynamicPayPlan.Name = "textDynamicPayPlan";
			this.textDynamicPayPlan.Size = new System.Drawing.Size(68, 20);
			this.textDynamicPayPlan.TabIndex = 244;
			// 
			// label27
			// 
			this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label27.Location = new System.Drawing.Point(75, 54);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(123, 17);
			this.label27.TabIndex = 242;
			this.label27.Text = "Pay Plan charge logic:";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPayPlansVersion
			// 
			this.comboPayPlansVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboPayPlansVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayPlansVersion.FormattingEnabled = true;
			this.comboPayPlansVersion.Location = new System.Drawing.Point(199, 51);
			this.comboPayPlansVersion.Name = "comboPayPlansVersion";
			this.comboPayPlansVersion.Size = new System.Drawing.Size(194, 21);
			this.comboPayPlansVersion.TabIndex = 241;
			this.comboPayPlansVersion.SelectionChangeCommitted += new System.EventHandler(this.comboPayPlansVersion_SelectionChangeCommitted);
			// 
			// checkHideDueNow
			// 
			this.checkHideDueNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkHideDueNow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHideDueNow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHideDueNow.Location = new System.Drawing.Point(75, 76);
			this.checkHideDueNow.Name = "checkHideDueNow";
			this.checkHideDueNow.Size = new System.Drawing.Size(318, 17);
			this.checkHideDueNow.TabIndex = 239;
			this.checkHideDueNow.Text = "Hide \"Due Now\" in Payment Plans Grid";
			this.checkHideDueNow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPayPlansUseSheets
			// 
			this.checkPayPlansUseSheets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPayPlansUseSheets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPayPlansUseSheets.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPayPlansUseSheets.Location = new System.Drawing.Point(75, 30);
			this.checkPayPlansUseSheets.Name = "checkPayPlansUseSheets";
			this.checkPayPlansUseSheets.Size = new System.Drawing.Size(318, 17);
			this.checkPayPlansUseSheets.TabIndex = 227;
			this.checkPayPlansUseSheets.Text = "Pay Plans use Sheets";
			this.checkPayPlansUseSheets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPayPlansExcludePastActivity
			// 
			this.checkPayPlansExcludePastActivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPayPlansExcludePastActivity.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPayPlansExcludePastActivity.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPayPlansExcludePastActivity.Location = new System.Drawing.Point(75, 12);
			this.checkPayPlansExcludePastActivity.Name = "checkPayPlansExcludePastActivity";
			this.checkPayPlansExcludePastActivity.Size = new System.Drawing.Size(318, 17);
			this.checkPayPlansExcludePastActivity.TabIndex = 236;
			this.checkPayPlansExcludePastActivity.Text = "Payment Plans exclude past activity by default";
			this.checkPayPlansExcludePastActivity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabTreatPlan
			// 
			this.tabTreatPlan.BackColor = System.Drawing.SystemColors.Control;
			this.tabTreatPlan.Controls.Add(this.groupBoxOD4);
			this.tabTreatPlan.Controls.Add(this.checkPromptSaveTP);
			this.tabTreatPlan.Controls.Add(this.labelDiscountPercentage);
			this.tabTreatPlan.Controls.Add(this.groupBox6);
			this.tabTreatPlan.Controls.Add(this.label19);
			this.tabTreatPlan.Controls.Add(this.groupInsHist);
			this.tabTreatPlan.Controls.Add(this.label1);
			this.tabTreatPlan.Controls.Add(this.checkFrequency);
			this.tabTreatPlan.Controls.Add(this.groupTreatPlanSort);
			this.tabTreatPlan.Controls.Add(this.textTreatNote);
			this.tabTreatPlan.Controls.Add(this.checkTPSaveSigned);
			this.tabTreatPlan.Controls.Add(this.comboProcDiscountType);
			this.tabTreatPlan.Controls.Add(this.checkTreatPlanShowCompleted);
			this.tabTreatPlan.Controls.Add(this.textDiscountPercentage);
			this.tabTreatPlan.Controls.Add(this.checkTreatPlanItemized);
			this.tabTreatPlan.Location = new System.Drawing.Point(4, 22);
			this.tabTreatPlan.Name = "tabTreatPlan";
			this.tabTreatPlan.Padding = new System.Windows.Forms.Padding(3);
			this.tabTreatPlan.Size = new System.Drawing.Size(1227, 640);
			this.tabTreatPlan.TabIndex = 3;
			this.tabTreatPlan.Text = "Treat\' Plan";
			// 
			// groupBoxOD4
			// 
			this.groupBoxOD4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD4.Controls.Add(this.label63);
			this.groupBoxOD4.Controls.Add(this.textDiscountPACodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountPAFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountXrayCodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountXrayFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountPerioCodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountPerioFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountLimitedCodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountLimitedFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountFluorideCodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountProphyFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountExamCodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountFluorideFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountProphyCodes);
			this.groupBoxOD4.Controls.Add(this.labelDiscountExamFreq);
			this.groupBoxOD4.Location = new System.Drawing.Point(497, 410);
			this.groupBoxOD4.Name = "groupBoxOD4";
			this.groupBoxOD4.Size = new System.Drawing.Size(316, 204);
			this.groupBoxOD4.TabIndex = 243;
			this.groupBoxOD4.TabStop = false;
			this.groupBoxOD4.Text = "Discount Plan Frequency Limitations";
			// 
			// label63
			// 
			this.label63.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label63.Location = new System.Drawing.Point(64, 16);
			this.label63.Name = "label63";
			this.label63.Size = new System.Drawing.Size(246, 17);
			this.label63.TabIndex = 248;
			this.label63.Text = "(all codes should be comma separated)";
			this.label63.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountPACodes
			// 
			this.textDiscountPACodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountPACodes.Location = new System.Drawing.Point(137, 175);
			this.textDiscountPACodes.Name = "textDiscountPACodes";
			this.textDiscountPACodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountPACodes.TabIndex = 15;
			// 
			// labelDiscountPAFreq
			// 
			this.labelDiscountPAFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountPAFreq.Location = new System.Drawing.Point(3, 176);
			this.labelDiscountPAFreq.Name = "labelDiscountPAFreq";
			this.labelDiscountPAFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountPAFreq.TabIndex = 241;
			this.labelDiscountPAFreq.Text = "Periapical X-Ray Codes";
			this.labelDiscountPAFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountXrayCodes
			// 
			this.textDiscountXrayCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountXrayCodes.Location = new System.Drawing.Point(137, 152);
			this.textDiscountXrayCodes.Name = "textDiscountXrayCodes";
			this.textDiscountXrayCodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountXrayCodes.TabIndex = 13;
			// 
			// labelDiscountXrayFreq
			// 
			this.labelDiscountXrayFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountXrayFreq.Location = new System.Drawing.Point(3, 153);
			this.labelDiscountXrayFreq.Name = "labelDiscountXrayFreq";
			this.labelDiscountXrayFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountXrayFreq.TabIndex = 239;
			this.labelDiscountXrayFreq.Text = "X-Ray Codes";
			this.labelDiscountXrayFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountPerioCodes
			// 
			this.textDiscountPerioCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountPerioCodes.Location = new System.Drawing.Point(137, 106);
			this.textDiscountPerioCodes.Name = "textDiscountPerioCodes";
			this.textDiscountPerioCodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountPerioCodes.TabIndex = 9;
			// 
			// labelDiscountPerioFreq
			// 
			this.labelDiscountPerioFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountPerioFreq.Location = new System.Drawing.Point(3, 107);
			this.labelDiscountPerioFreq.Name = "labelDiscountPerioFreq";
			this.labelDiscountPerioFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountPerioFreq.TabIndex = 233;
			this.labelDiscountPerioFreq.Text = "Perio Maintenance Codes";
			this.labelDiscountPerioFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountLimitedCodes
			// 
			this.textDiscountLimitedCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountLimitedCodes.Location = new System.Drawing.Point(137, 129);
			this.textDiscountLimitedCodes.Name = "textDiscountLimitedCodes";
			this.textDiscountLimitedCodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountLimitedCodes.TabIndex = 11;
			// 
			// labelDiscountLimitedFreq
			// 
			this.labelDiscountLimitedFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountLimitedFreq.Location = new System.Drawing.Point(3, 130);
			this.labelDiscountLimitedFreq.Name = "labelDiscountLimitedFreq";
			this.labelDiscountLimitedFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountLimitedFreq.TabIndex = 231;
			this.labelDiscountLimitedFreq.Text = "Limited Exam Codes";
			this.labelDiscountLimitedFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountFluorideCodes
			// 
			this.textDiscountFluorideCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountFluorideCodes.Location = new System.Drawing.Point(137, 83);
			this.textDiscountFluorideCodes.Name = "textDiscountFluorideCodes";
			this.textDiscountFluorideCodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountFluorideCodes.TabIndex = 7;
			// 
			// labelDiscountProphyFreq
			// 
			this.labelDiscountProphyFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountProphyFreq.Location = new System.Drawing.Point(2, 61);
			this.labelDiscountProphyFreq.Name = "labelDiscountProphyFreq";
			this.labelDiscountProphyFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountProphyFreq.TabIndex = 229;
			this.labelDiscountProphyFreq.Text = "Prophylaxis Codes";
			this.labelDiscountProphyFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountExamCodes
			// 
			this.textDiscountExamCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountExamCodes.Location = new System.Drawing.Point(137, 37);
			this.textDiscountExamCodes.Name = "textDiscountExamCodes";
			this.textDiscountExamCodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountExamCodes.TabIndex = 3;
			// 
			// labelDiscountFluorideFreq
			// 
			this.labelDiscountFluorideFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountFluorideFreq.Location = new System.Drawing.Point(3, 84);
			this.labelDiscountFluorideFreq.Name = "labelDiscountFluorideFreq";
			this.labelDiscountFluorideFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountFluorideFreq.TabIndex = 228;
			this.labelDiscountFluorideFreq.Text = "Fluoride Codes";
			this.labelDiscountFluorideFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountProphyCodes
			// 
			this.textDiscountProphyCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDiscountProphyCodes.Location = new System.Drawing.Point(137, 60);
			this.textDiscountProphyCodes.Name = "textDiscountProphyCodes";
			this.textDiscountProphyCodes.Size = new System.Drawing.Size(173, 20);
			this.textDiscountProphyCodes.TabIndex = 5;
			// 
			// labelDiscountExamFreq
			// 
			this.labelDiscountExamFreq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDiscountExamFreq.Location = new System.Drawing.Point(3, 38);
			this.labelDiscountExamFreq.Name = "labelDiscountExamFreq";
			this.labelDiscountExamFreq.Size = new System.Drawing.Size(135, 17);
			this.labelDiscountExamFreq.TabIndex = 227;
			this.labelDiscountExamFreq.Text = "Exam Codes";
			this.labelDiscountExamFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPromptSaveTP
			// 
			this.checkPromptSaveTP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptSaveTP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPromptSaveTP.Location = new System.Drawing.Point(122, 278);
			this.checkPromptSaveTP.Name = "checkPromptSaveTP";
			this.checkPromptSaveTP.Size = new System.Drawing.Size(302, 17);
			this.checkPromptSaveTP.TabIndex = 241;
			this.checkPromptSaveTP.Text = "Prompt to save Treatment Plans";
			this.checkPromptSaveTP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPromptSaveTP.UseVisualStyleBackColor = false;
			// 
			// labelDiscountPercentage
			// 
			this.labelDiscountPercentage.Location = new System.Drawing.Point(119, 158);
			this.labelDiscountPercentage.Margin = new System.Windows.Forms.Padding(0);
			this.labelDiscountPercentage.Name = "labelDiscountPercentage";
			this.labelDiscountPercentage.Size = new System.Drawing.Size(246, 16);
			this.labelDiscountPercentage.TabIndex = 240;
			this.labelDiscountPercentage.Text = "Procedure discount percentage";
			this.labelDiscountPercentage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox6
			// 
			this.groupBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox6.Controls.Add(this.textInsImplant);
			this.groupBox6.Controls.Add(this.labelInsImplant);
			this.groupBox6.Controls.Add(this.label52);
			this.groupBox6.Controls.Add(this.textInsDentures);
			this.groupBox6.Controls.Add(this.labelInsDentures);
			this.groupBox6.Controls.Add(this.textInsPerioMaint);
			this.groupBox6.Controls.Add(this.labelInsPerioMaint);
			this.groupBox6.Controls.Add(this.textInsDebridement);
			this.groupBox6.Controls.Add(this.labelInsDebridement);
			this.groupBox6.Controls.Add(this.textInsSealant);
			this.groupBox6.Controls.Add(this.labelInsSealant);
			this.groupBox6.Controls.Add(this.textInsFlouride);
			this.groupBox6.Controls.Add(this.labelInsFlouride);
			this.groupBox6.Controls.Add(this.textInsCrown);
			this.groupBox6.Controls.Add(this.labelInsCrown);
			this.groupBox6.Controls.Add(this.textInsSRP);
			this.groupBox6.Controls.Add(this.labelInsSRP);
			this.groupBox6.Controls.Add(this.textInsCancerScreen);
			this.groupBox6.Controls.Add(this.labelInsCancerScreen);
			this.groupBox6.Controls.Add(this.textInsProphy);
			this.groupBox6.Controls.Add(this.labelInsProphy);
			this.groupBox6.Controls.Add(this.textInsExam);
			this.groupBox6.Controls.Add(this.labelInsPano);
			this.groupBox6.Controls.Add(this.textInsBW);
			this.groupBox6.Controls.Add(this.labelInsExam);
			this.groupBox6.Controls.Add(this.textInsPano);
			this.groupBox6.Controls.Add(this.labelInsBW);
			this.groupBox6.Location = new System.Drawing.Point(497, 58);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(316, 346);
			this.groupBox6.TabIndex = 232;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Frequency Limitations";
			// 
			// textInsImplant
			// 
			this.textInsImplant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsImplant.Location = new System.Drawing.Point(137, 313);
			this.textInsImplant.Name = "textInsImplant";
			this.textInsImplant.Size = new System.Drawing.Size(173, 20);
			this.textInsImplant.TabIndex = 27;
			// 
			// labelInsImplant
			// 
			this.labelInsImplant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsImplant.Location = new System.Drawing.Point(3, 314);
			this.labelInsImplant.Name = "labelInsImplant";
			this.labelInsImplant.Size = new System.Drawing.Size(135, 17);
			this.labelInsImplant.TabIndex = 250;
			this.labelInsImplant.Text = "Implant Codes";
			this.labelInsImplant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label52
			// 
			this.label52.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label52.Location = new System.Drawing.Point(68, 16);
			this.label52.Name = "label52";
			this.label52.Size = new System.Drawing.Size(246, 17);
			this.label52.TabIndex = 248;
			this.label52.Text = "(all codes should be comma separated)";
			this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsDentures
			// 
			this.textInsDentures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsDentures.Location = new System.Drawing.Point(137, 290);
			this.textInsDentures.Name = "textInsDentures";
			this.textInsDentures.Size = new System.Drawing.Size(173, 20);
			this.textInsDentures.TabIndex = 25;
			// 
			// labelInsDentures
			// 
			this.labelInsDentures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsDentures.Location = new System.Drawing.Point(3, 291);
			this.labelInsDentures.Name = "labelInsDentures";
			this.labelInsDentures.Size = new System.Drawing.Size(135, 17);
			this.labelInsDentures.TabIndex = 247;
			this.labelInsDentures.Text = "Dentures Codes";
			this.labelInsDentures.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsPerioMaint
			// 
			this.textInsPerioMaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsPerioMaint.Location = new System.Drawing.Point(137, 267);
			this.textInsPerioMaint.Name = "textInsPerioMaint";
			this.textInsPerioMaint.Size = new System.Drawing.Size(173, 20);
			this.textInsPerioMaint.TabIndex = 23;
			// 
			// labelInsPerioMaint
			// 
			this.labelInsPerioMaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsPerioMaint.Location = new System.Drawing.Point(3, 268);
			this.labelInsPerioMaint.Name = "labelInsPerioMaint";
			this.labelInsPerioMaint.Size = new System.Drawing.Size(135, 17);
			this.labelInsPerioMaint.TabIndex = 245;
			this.labelInsPerioMaint.Text = "Perio Maintenance Codes";
			this.labelInsPerioMaint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsDebridement
			// 
			this.textInsDebridement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsDebridement.Location = new System.Drawing.Point(137, 244);
			this.textInsDebridement.Name = "textInsDebridement";
			this.textInsDebridement.Size = new System.Drawing.Size(173, 20);
			this.textInsDebridement.TabIndex = 21;
			// 
			// labelInsDebridement
			// 
			this.labelInsDebridement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsDebridement.Location = new System.Drawing.Point(3, 245);
			this.labelInsDebridement.Name = "labelInsDebridement";
			this.labelInsDebridement.Size = new System.Drawing.Size(135, 17);
			this.labelInsDebridement.TabIndex = 243;
			this.labelInsDebridement.Text = "Full Debridement Codes";
			this.labelInsDebridement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsSealant
			// 
			this.textInsSealant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsSealant.Location = new System.Drawing.Point(137, 175);
			this.textInsSealant.Name = "textInsSealant";
			this.textInsSealant.Size = new System.Drawing.Size(173, 20);
			this.textInsSealant.TabIndex = 15;
			// 
			// labelInsSealant
			// 
			this.labelInsSealant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsSealant.Location = new System.Drawing.Point(3, 176);
			this.labelInsSealant.Name = "labelInsSealant";
			this.labelInsSealant.Size = new System.Drawing.Size(135, 17);
			this.labelInsSealant.TabIndex = 241;
			this.labelInsSealant.Text = "Sealant Codes";
			this.labelInsSealant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsFlouride
			// 
			this.textInsFlouride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsFlouride.Location = new System.Drawing.Point(137, 152);
			this.textInsFlouride.Name = "textInsFlouride";
			this.textInsFlouride.Size = new System.Drawing.Size(173, 20);
			this.textInsFlouride.TabIndex = 13;
			// 
			// labelInsFlouride
			// 
			this.labelInsFlouride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsFlouride.Location = new System.Drawing.Point(3, 153);
			this.labelInsFlouride.Name = "labelInsFlouride";
			this.labelInsFlouride.Size = new System.Drawing.Size(135, 17);
			this.labelInsFlouride.TabIndex = 239;
			this.labelInsFlouride.Text = "Fluoride Codes";
			this.labelInsFlouride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsCrown
			// 
			this.textInsCrown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsCrown.Location = new System.Drawing.Point(137, 198);
			this.textInsCrown.Name = "textInsCrown";
			this.textInsCrown.Size = new System.Drawing.Size(173, 20);
			this.textInsCrown.TabIndex = 17;
			// 
			// labelInsCrown
			// 
			this.labelInsCrown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsCrown.Location = new System.Drawing.Point(3, 199);
			this.labelInsCrown.Name = "labelInsCrown";
			this.labelInsCrown.Size = new System.Drawing.Size(135, 17);
			this.labelInsCrown.TabIndex = 237;
			this.labelInsCrown.Text = "Crown Codes";
			this.labelInsCrown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsSRP
			// 
			this.textInsSRP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsSRP.Location = new System.Drawing.Point(137, 221);
			this.textInsSRP.Name = "textInsSRP";
			this.textInsSRP.Size = new System.Drawing.Size(173, 20);
			this.textInsSRP.TabIndex = 19;
			// 
			// labelInsSRP
			// 
			this.labelInsSRP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsSRP.Location = new System.Drawing.Point(3, 222);
			this.labelInsSRP.Name = "labelInsSRP";
			this.labelInsSRP.Size = new System.Drawing.Size(135, 17);
			this.labelInsSRP.TabIndex = 235;
			this.labelInsSRP.Text = "SRP Codes";
			this.labelInsSRP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsCancerScreen
			// 
			this.textInsCancerScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsCancerScreen.Location = new System.Drawing.Point(137, 106);
			this.textInsCancerScreen.Name = "textInsCancerScreen";
			this.textInsCancerScreen.Size = new System.Drawing.Size(173, 20);
			this.textInsCancerScreen.TabIndex = 9;
			// 
			// labelInsCancerScreen
			// 
			this.labelInsCancerScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsCancerScreen.Location = new System.Drawing.Point(3, 107);
			this.labelInsCancerScreen.Name = "labelInsCancerScreen";
			this.labelInsCancerScreen.Size = new System.Drawing.Size(135, 17);
			this.labelInsCancerScreen.TabIndex = 233;
			this.labelInsCancerScreen.Text = "Cancer Screening Codes";
			this.labelInsCancerScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsProphy
			// 
			this.textInsProphy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsProphy.Location = new System.Drawing.Point(137, 129);
			this.textInsProphy.Name = "textInsProphy";
			this.textInsProphy.Size = new System.Drawing.Size(173, 20);
			this.textInsProphy.TabIndex = 11;
			// 
			// labelInsProphy
			// 
			this.labelInsProphy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsProphy.Location = new System.Drawing.Point(3, 130);
			this.labelInsProphy.Name = "labelInsProphy";
			this.labelInsProphy.Size = new System.Drawing.Size(135, 17);
			this.labelInsProphy.TabIndex = 231;
			this.labelInsProphy.Text = "Prophylaxis Codes";
			this.labelInsProphy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsExam
			// 
			this.textInsExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsExam.Location = new System.Drawing.Point(137, 83);
			this.textInsExam.Name = "textInsExam";
			this.textInsExam.Size = new System.Drawing.Size(173, 20);
			this.textInsExam.TabIndex = 7;
			// 
			// labelInsPano
			// 
			this.labelInsPano.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsPano.Location = new System.Drawing.Point(2, 61);
			this.labelInsPano.Name = "labelInsPano";
			this.labelInsPano.Size = new System.Drawing.Size(135, 17);
			this.labelInsPano.TabIndex = 229;
			this.labelInsPano.Text = "Pano/FMX Codes";
			this.labelInsPano.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsBW
			// 
			this.textInsBW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsBW.Location = new System.Drawing.Point(137, 37);
			this.textInsBW.Name = "textInsBW";
			this.textInsBW.Size = new System.Drawing.Size(173, 20);
			this.textInsBW.TabIndex = 3;
			// 
			// labelInsExam
			// 
			this.labelInsExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsExam.Location = new System.Drawing.Point(3, 84);
			this.labelInsExam.Name = "labelInsExam";
			this.labelInsExam.Size = new System.Drawing.Size(135, 17);
			this.labelInsExam.TabIndex = 228;
			this.labelInsExam.Text = "Exam Codes";
			this.labelInsExam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsPano
			// 
			this.textInsPano.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsPano.Location = new System.Drawing.Point(137, 60);
			this.textInsPano.Name = "textInsPano";
			this.textInsPano.Size = new System.Drawing.Size(173, 20);
			this.textInsPano.TabIndex = 5;
			// 
			// labelInsBW
			// 
			this.labelInsBW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsBW.Location = new System.Drawing.Point(3, 38);
			this.labelInsBW.Name = "labelInsBW";
			this.labelInsBW.Size = new System.Drawing.Size(135, 17);
			this.labelInsBW.TabIndex = 227;
			this.labelInsBW.Text = "Bitewing Codes";
			this.labelInsBW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(38, 130);
			this.label19.Margin = new System.Windows.Forms.Padding(0);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(221, 15);
			this.label19.TabIndex = 239;
			this.label19.Text = "Procedure discount adj type";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupInsHist
			// 
			this.groupInsHist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupInsHist.Controls.Add(this.textInsHistProphy);
			this.groupInsHist.Controls.Add(this.labelInsHistProphy);
			this.groupInsHist.Controls.Add(this.textInsHistPerioLR);
			this.groupInsHist.Controls.Add(this.labelInsHistPerioLR);
			this.groupInsHist.Controls.Add(this.textInsHistPerioLL);
			this.groupInsHist.Controls.Add(this.labelInsHistPerioLL);
			this.groupInsHist.Controls.Add(this.textInsHistPerioUL);
			this.groupInsHist.Controls.Add(this.labelInsHistPerioUL);
			this.groupInsHist.Controls.Add(this.textInsHistPerioUR);
			this.groupInsHist.Controls.Add(this.labelInsHistPerioUR);
			this.groupInsHist.Controls.Add(this.textInsHistFMX);
			this.groupInsHist.Controls.Add(this.labelInsHistFMX);
			this.groupInsHist.Controls.Add(this.textInsHistPerioMaint);
			this.groupInsHist.Controls.Add(this.labelInsHistPerioMaint);
			this.groupInsHist.Controls.Add(this.textInsHistExam);
			this.groupInsHist.Controls.Add(this.labelInsHistDebridement);
			this.groupInsHist.Controls.Add(this.textInsHistBW);
			this.groupInsHist.Controls.Add(this.labelInsHistExam);
			this.groupInsHist.Controls.Add(this.textInsHistDebridement);
			this.groupInsHist.Controls.Add(this.labelInsHistBW);
			this.groupInsHist.Location = new System.Drawing.Point(876, 35);
			this.groupInsHist.Name = "groupInsHist";
			this.groupInsHist.Size = new System.Drawing.Size(256, 252);
			this.groupInsHist.TabIndex = 234;
			this.groupInsHist.TabStop = false;
			this.groupInsHist.Text = "Insurance History";
			// 
			// textInsHistProphy
			// 
			this.textInsHistProphy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistProphy.Location = new System.Drawing.Point(151, 88);
			this.textInsHistProphy.Name = "textInsHistProphy";
			this.textInsHistProphy.Size = new System.Drawing.Size(99, 20);
			this.textInsHistProphy.TabIndex = 9;
			// 
			// labelInsHistProphy
			// 
			this.labelInsHistProphy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistProphy.Location = new System.Drawing.Point(7, 89);
			this.labelInsHistProphy.Name = "labelInsHistProphy";
			this.labelInsHistProphy.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistProphy.TabIndex = 243;
			this.labelInsHistProphy.Text = "Prophylaxis Code";
			this.labelInsHistProphy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistPerioLR
			// 
			this.textInsHistPerioLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioLR.Location = new System.Drawing.Point(151, 180);
			this.textInsHistPerioLR.Name = "textInsHistPerioLR";
			this.textInsHistPerioLR.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioLR.TabIndex = 17;
			// 
			// labelInsHistPerioLR
			// 
			this.labelInsHistPerioLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioLR.Location = new System.Drawing.Point(7, 181);
			this.labelInsHistPerioLR.Name = "labelInsHistPerioLR";
			this.labelInsHistPerioLR.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistPerioLR.TabIndex = 241;
			this.labelInsHistPerioLR.Text = "Perio Scaling LR Code";
			this.labelInsHistPerioLR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistPerioLL
			// 
			this.textInsHistPerioLL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioLL.Location = new System.Drawing.Point(151, 157);
			this.textInsHistPerioLL.Name = "textInsHistPerioLL";
			this.textInsHistPerioLL.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioLL.TabIndex = 15;
			// 
			// labelInsHistPerioLL
			// 
			this.labelInsHistPerioLL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioLL.Location = new System.Drawing.Point(7, 158);
			this.labelInsHistPerioLL.Name = "labelInsHistPerioLL";
			this.labelInsHistPerioLL.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistPerioLL.TabIndex = 239;
			this.labelInsHistPerioLL.Text = "Perio Scaling LL Code";
			this.labelInsHistPerioLL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistPerioUL
			// 
			this.textInsHistPerioUL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioUL.Location = new System.Drawing.Point(151, 203);
			this.textInsHistPerioUL.Name = "textInsHistPerioUL";
			this.textInsHistPerioUL.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioUL.TabIndex = 19;
			// 
			// labelInsHistPerioUL
			// 
			this.labelInsHistPerioUL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioUL.Location = new System.Drawing.Point(7, 204);
			this.labelInsHistPerioUL.Name = "labelInsHistPerioUL";
			this.labelInsHistPerioUL.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistPerioUL.TabIndex = 237;
			this.labelInsHistPerioUL.Text = "Perio Scaling UL Code";
			this.labelInsHistPerioUL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistPerioUR
			// 
			this.textInsHistPerioUR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioUR.Location = new System.Drawing.Point(151, 226);
			this.textInsHistPerioUR.Name = "textInsHistPerioUR";
			this.textInsHistPerioUR.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioUR.TabIndex = 21;
			// 
			// labelInsHistPerioUR
			// 
			this.labelInsHistPerioUR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioUR.Location = new System.Drawing.Point(7, 227);
			this.labelInsHistPerioUR.Name = "labelInsHistPerioUR";
			this.labelInsHistPerioUR.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistPerioUR.TabIndex = 235;
			this.labelInsHistPerioUR.Text = "Perio Scaling UR Code";
			this.labelInsHistPerioUR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistFMX
			// 
			this.textInsHistFMX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistFMX.Location = new System.Drawing.Point(151, 42);
			this.textInsHistFMX.Name = "textInsHistFMX";
			this.textInsHistFMX.Size = new System.Drawing.Size(99, 20);
			this.textInsHistFMX.TabIndex = 5;
			// 
			// labelInsHistFMX
			// 
			this.labelInsHistFMX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistFMX.Location = new System.Drawing.Point(7, 43);
			this.labelInsHistFMX.Name = "labelInsHistFMX";
			this.labelInsHistFMX.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistFMX.TabIndex = 233;
			this.labelInsHistFMX.Text = "Pano/FMX Code";
			this.labelInsHistFMX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistPerioMaint
			// 
			this.textInsHistPerioMaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistPerioMaint.Location = new System.Drawing.Point(151, 134);
			this.textInsHistPerioMaint.Name = "textInsHistPerioMaint";
			this.textInsHistPerioMaint.Size = new System.Drawing.Size(99, 20);
			this.textInsHistPerioMaint.TabIndex = 13;
			// 
			// labelInsHistPerioMaint
			// 
			this.labelInsHistPerioMaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistPerioMaint.Location = new System.Drawing.Point(7, 135);
			this.labelInsHistPerioMaint.Name = "labelInsHistPerioMaint";
			this.labelInsHistPerioMaint.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistPerioMaint.TabIndex = 231;
			this.labelInsHistPerioMaint.Text = "Perio Maintenance Code";
			this.labelInsHistPerioMaint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistExam
			// 
			this.textInsHistExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistExam.Location = new System.Drawing.Point(151, 65);
			this.textInsHistExam.Name = "textInsHistExam";
			this.textInsHistExam.Size = new System.Drawing.Size(99, 20);
			this.textInsHistExam.TabIndex = 7;
			// 
			// labelInsHistDebridement
			// 
			this.labelInsHistDebridement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistDebridement.Location = new System.Drawing.Point(7, 112);
			this.labelInsHistDebridement.Name = "labelInsHistDebridement";
			this.labelInsHistDebridement.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistDebridement.TabIndex = 229;
			this.labelInsHistDebridement.Text = "Full Debridement Code";
			this.labelInsHistDebridement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistBW
			// 
			this.textInsHistBW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistBW.Location = new System.Drawing.Point(151, 19);
			this.textInsHistBW.Name = "textInsHistBW";
			this.textInsHistBW.Size = new System.Drawing.Size(99, 20);
			this.textInsHistBW.TabIndex = 3;
			// 
			// labelInsHistExam
			// 
			this.labelInsHistExam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistExam.Location = new System.Drawing.Point(7, 66);
			this.labelInsHistExam.Name = "labelInsHistExam";
			this.labelInsHistExam.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistExam.TabIndex = 228;
			this.labelInsHistExam.Text = "Exam Code";
			this.labelInsHistExam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsHistDebridement
			// 
			this.textInsHistDebridement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textInsHistDebridement.Location = new System.Drawing.Point(151, 111);
			this.textInsHistDebridement.Name = "textInsHistDebridement";
			this.textInsHistDebridement.Size = new System.Drawing.Size(99, 20);
			this.textInsHistDebridement.TabIndex = 11;
			// 
			// labelInsHistBW
			// 
			this.labelInsHistBW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsHistBW.Location = new System.Drawing.Point(7, 20);
			this.labelInsHistBW.Name = "labelInsHistBW";
			this.labelInsHistBW.Size = new System.Drawing.Size(143, 18);
			this.labelInsHistBW.TabIndex = 227;
			this.labelInsHistBW.Text = "Bitewing Code";
			this.labelInsHistBW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 52);
			this.label1.TabIndex = 35;
			this.label1.Text = "Default Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkFrequency
			// 
			this.checkFrequency.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFrequency.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkFrequency.Location = new System.Drawing.Point(593, 35);
			this.checkFrequency.Name = "checkFrequency";
			this.checkFrequency.Size = new System.Drawing.Size(220, 17);
			this.checkFrequency.TabIndex = 1;
			this.checkFrequency.Text = "Enable Insurance Frequency Checking";
			this.checkFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFrequency.UseVisualStyleBackColor = false;
			this.checkFrequency.Click += new System.EventHandler(this.checkFrequency_Click);
			// 
			// groupTreatPlanSort
			// 
			this.groupTreatPlanSort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupTreatPlanSort.Controls.Add(this.radioTreatPlanSortTooth);
			this.groupTreatPlanSort.Controls.Add(this.radioTreatPlanSortOrder);
			this.groupTreatPlanSort.Location = new System.Drawing.Point(253, 217);
			this.groupTreatPlanSort.Name = "groupTreatPlanSort";
			this.groupTreatPlanSort.Size = new System.Drawing.Size(171, 55);
			this.groupTreatPlanSort.TabIndex = 218;
			this.groupTreatPlanSort.TabStop = false;
			this.groupTreatPlanSort.Text = "Sort procedures by";
			// 
			// radioTreatPlanSortTooth
			// 
			this.radioTreatPlanSortTooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortTooth.Location = new System.Drawing.Point(8, 16);
			this.radioTreatPlanSortTooth.Name = "radioTreatPlanSortTooth";
			this.radioTreatPlanSortTooth.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioTreatPlanSortTooth.Size = new System.Drawing.Size(157, 15);
			this.radioTreatPlanSortTooth.TabIndex = 54;
			this.radioTreatPlanSortTooth.Text = "Tooth";
			this.radioTreatPlanSortTooth.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortTooth.Click += new System.EventHandler(this.radioTreatPlanSortTooth_Click);
			// 
			// radioTreatPlanSortOrder
			// 
			this.radioTreatPlanSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortOrder.Checked = true;
			this.radioTreatPlanSortOrder.Location = new System.Drawing.Point(8, 33);
			this.radioTreatPlanSortOrder.Name = "radioTreatPlanSortOrder";
			this.radioTreatPlanSortOrder.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.radioTreatPlanSortOrder.Size = new System.Drawing.Size(157, 15);
			this.radioTreatPlanSortOrder.TabIndex = 53;
			this.radioTreatPlanSortOrder.TabStop = true;
			this.radioTreatPlanSortOrder.Text = "Order Entered";
			this.radioTreatPlanSortOrder.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortOrder.Click += new System.EventHandler(this.radioTreatPlanSortOrder_Click);
			// 
			// textTreatNote
			// 
			this.textTreatNote.AcceptsTab = true;
			this.textTreatNote.BackColor = System.Drawing.SystemColors.Window;
			this.textTreatNote.DetectLinksEnabled = false;
			this.textTreatNote.DetectUrls = false;
			this.textTreatNote.Location = new System.Drawing.Point(61, 33);
			this.textTreatNote.MaxLength = 32767;
			this.textTreatNote.Name = "textTreatNote";
			this.textTreatNote.QuickPasteType = OpenDentBusiness.QuickPasteType.TreatPlan;
			this.textTreatNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTreatNote.Size = new System.Drawing.Size(363, 66);
			this.textTreatNote.TabIndex = 215;
			this.textTreatNote.Text = "";
			// 
			// checkTPSaveSigned
			// 
			this.checkTPSaveSigned.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTPSaveSigned.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTPSaveSigned.Location = new System.Drawing.Point(122, 195);
			this.checkTPSaveSigned.Name = "checkTPSaveSigned";
			this.checkTPSaveSigned.Size = new System.Drawing.Size(302, 17);
			this.checkTPSaveSigned.TabIndex = 213;
			this.checkTPSaveSigned.Text = "Save signed Treatment Plans to PDF";
			this.checkTPSaveSigned.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTPSaveSigned.UseVisualStyleBackColor = false;
			// 
			// comboProcDiscountType
			// 
			this.comboProcDiscountType.Location = new System.Drawing.Point(261, 128);
			this.comboProcDiscountType.Name = "comboProcDiscountType";
			this.comboProcDiscountType.Size = new System.Drawing.Size(163, 21);
			this.comboProcDiscountType.TabIndex = 201;
			// 
			// checkTreatPlanShowCompleted
			// 
			this.checkTreatPlanShowCompleted.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTreatPlanShowCompleted.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTreatPlanShowCompleted.Location = new System.Drawing.Point(65, 105);
			this.checkTreatPlanShowCompleted.Name = "checkTreatPlanShowCompleted";
			this.checkTreatPlanShowCompleted.Size = new System.Drawing.Size(359, 17);
			this.checkTreatPlanShowCompleted.TabIndex = 47;
			this.checkTreatPlanShowCompleted.Text = "Show completed work on graphical tooth chart";
			this.checkTreatPlanShowCompleted.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDiscountPercentage
			// 
			this.textDiscountPercentage.Location = new System.Drawing.Point(371, 155);
			this.textDiscountPercentage.Name = "textDiscountPercentage";
			this.textDiscountPercentage.Size = new System.Drawing.Size(53, 20);
			this.textDiscountPercentage.TabIndex = 211;
			// 
			// checkTreatPlanItemized
			// 
			this.checkTreatPlanItemized.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTreatPlanItemized.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTreatPlanItemized.Location = new System.Drawing.Point(284, 178);
			this.checkTreatPlanItemized.Name = "checkTreatPlanItemized";
			this.checkTreatPlanItemized.Size = new System.Drawing.Size(140, 17);
			this.checkTreatPlanItemized.TabIndex = 212;
			this.checkTreatPlanItemized.Text = "Itemize Treatment Plan";
			this.checkTreatPlanItemized.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTreatPlanItemized.UseVisualStyleBackColor = false;
			// 
			// tabChart
			// 
			this.tabChart.BackColor = System.Drawing.SystemColors.Control;
			this.tabChart.Controls.Add(this.checkNotesProviderSigOnly);
			this.tabChart.Controls.Add(this.checkShowPlannedApptPrompt);
			this.tabChart.Controls.Add(this.checkAllowSettingProcsComplete);
			this.tabChart.Controls.Add(this.checkIsAlertRadiologyProcsEnabled);
			this.tabChart.Controls.Add(this.comboToothNomenclature);
			this.tabChart.Controls.Add(this.textProblemsIndicateNone);
			this.tabChart.Controls.Add(this.label32);
			this.tabChart.Controls.Add(this.checkBoxRxClinicUseSelected);
			this.tabChart.Controls.Add(this.checkProcNoteConcurrencyMerge);
			this.tabChart.Controls.Add(this.label8);
			this.tabChart.Controls.Add(this.comboProcCodeListSort);
			this.tabChart.Controls.Add(this.checkProcProvChangesCp);
			this.tabChart.Controls.Add(this.labelToothNomenclature);
			this.tabChart.Controls.Add(this.comboProcFeeUpdatePrompt);
			this.tabChart.Controls.Add(this.checkPerioTreatImplantsAsNotMissing);
			this.tabChart.Controls.Add(this.labelProcFeeUpdatePrompt);
			this.tabChart.Controls.Add(this.checkAutoClearEntryStatus);
			this.tabChart.Controls.Add(this.butProblemsIndicateNone);
			this.tabChart.Controls.Add(this.checkPerioSkipMissingTeeth);
			this.tabChart.Controls.Add(this.label9);
			this.tabChart.Controls.Add(this.checkProcGroupNoteDoesAggregate);
			this.tabChart.Controls.Add(this.textMedicationsIndicateNone);
			this.tabChart.Controls.Add(this.checkProvColorChart);
			this.tabChart.Controls.Add(this.checkSignatureAllowDigital);
			this.tabChart.Controls.Add(this.textAllergiesIndicateNone);
			this.tabChart.Controls.Add(this.butMedicationsIndicateNone);
			this.tabChart.Controls.Add(this.textMedDefaultStopDays);
			this.tabChart.Controls.Add(this.checkClaimProcsAllowEstimatesOnCompl);
			this.tabChart.Controls.Add(this.label11);
			this.tabChart.Controls.Add(this.checkProcEditRequireAutoCode);
			this.tabChart.Controls.Add(this.checkChartNonPatientWarn);
			this.tabChart.Controls.Add(this.label14);
			this.tabChart.Controls.Add(this.checkProcLockingIsAllowed);
			this.tabChart.Controls.Add(this.checkProcsPromptForAutoNote);
			this.tabChart.Controls.Add(this.textICD9DefaultForNewProcs);
			this.tabChart.Controls.Add(this.labelIcdCodeDefault);
			this.tabChart.Controls.Add(this.checkScreeningsUseSheets);
			this.tabChart.Controls.Add(this.butDiagnosisCode);
			this.tabChart.Controls.Add(this.butAllergiesIndicateNone);
			this.tabChart.Controls.Add(this.checkDxIcdVersion);
			this.tabChart.Controls.Add(this.checkMedicalFeeUsedForNewProcs);
			this.tabChart.Location = new System.Drawing.Point(4, 22);
			this.tabChart.Name = "tabChart";
			this.tabChart.Padding = new System.Windows.Forms.Padding(3);
			this.tabChart.Size = new System.Drawing.Size(1227, 640);
			this.tabChart.TabIndex = 4;
			this.tabChart.Text = "Chart";
			// 
			// checkNotesProviderSigOnly
			// 
			this.checkNotesProviderSigOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNotesProviderSigOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNotesProviderSigOnly.Location = new System.Drawing.Point(673, 213);
			this.checkNotesProviderSigOnly.Name = "checkNotesProviderSigOnly";
			this.checkNotesProviderSigOnly.Size = new System.Drawing.Size(342, 15);
			this.checkNotesProviderSigOnly.TabIndex = 231;
			this.checkNotesProviderSigOnly.Text = "Notes can only be signed by providers";
			this.checkNotesProviderSigOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNotesProviderSigOnly.UseVisualStyleBackColor = true;
			// 
			// checkShowPlannedApptPrompt
			// 
			this.checkShowPlannedApptPrompt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPlannedApptPrompt.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowPlannedApptPrompt.Location = new System.Drawing.Point(169, 450);
			this.checkShowPlannedApptPrompt.Name = "checkShowPlannedApptPrompt";
			this.checkShowPlannedApptPrompt.Size = new System.Drawing.Size(336, 17);
			this.checkShowPlannedApptPrompt.TabIndex = 230;
			this.checkShowPlannedApptPrompt.Text = "Prompt for Planned Appointment";
			this.checkShowPlannedApptPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPlannedApptPrompt.UseVisualStyleBackColor = true;
			// 
			// checkAllowSettingProcsComplete
			// 
			this.checkAllowSettingProcsComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowSettingProcsComplete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowSettingProcsComplete.Location = new System.Drawing.Point(104, 44);
			this.checkAllowSettingProcsComplete.Name = "checkAllowSettingProcsComplete";
			this.checkAllowSettingProcsComplete.Size = new System.Drawing.Size(401, 17);
			this.checkAllowSettingProcsComplete.TabIndex = 74;
			this.checkAllowSettingProcsComplete.Text = "Allow setting procedures complete.  (It\'s better to only set appointments complet" +
    "e)";
			this.checkAllowSettingProcsComplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowSettingProcsComplete.UseVisualStyleBackColor = true;
			// 
			// comboToothNomenclature
			// 
			this.comboToothNomenclature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboToothNomenclature.FormattingEnabled = true;
			this.comboToothNomenclature.Location = new System.Drawing.Point(762, 67);
			this.comboToothNomenclature.Name = "comboToothNomenclature";
			this.comboToothNomenclature.Size = new System.Drawing.Size(255, 21);
			this.comboToothNomenclature.TabIndex = 195;
			// 
			// textProblemsIndicateNone
			// 
			this.textProblemsIndicateNone.Location = new System.Drawing.Point(334, 65);
			this.textProblemsIndicateNone.Name = "textProblemsIndicateNone";
			this.textProblemsIndicateNone.ReadOnly = true;
			this.textProblemsIndicateNone.Size = new System.Drawing.Size(146, 20);
			this.textProblemsIndicateNone.TabIndex = 198;
			// 
			// label32
			// 
			this.label32.Location = new System.Drawing.Point(626, 171);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(134, 15);
			this.label32.TabIndex = 220;
			this.label32.Text = "Procedure Code List sort";
			this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBoxRxClinicUseSelected
			// 
			this.checkBoxRxClinicUseSelected.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxRxClinicUseSelected.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxRxClinicUseSelected.Location = new System.Drawing.Point(93, 414);
			this.checkBoxRxClinicUseSelected.Name = "checkBoxRxClinicUseSelected";
			this.checkBoxRxClinicUseSelected.Size = new System.Drawing.Size(412, 17);
			this.checkBoxRxClinicUseSelected.TabIndex = 228;
			this.checkBoxRxClinicUseSelected.Text = "Rx use selected clinic from Clinics menu instead of selected patient\'s default cl" +
    "inic";
			this.checkBoxRxClinicUseSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxRxClinicUseSelected.UseVisualStyleBackColor = true;
			// 
			// checkProcNoteConcurrencyMerge
			// 
			this.checkProcNoteConcurrencyMerge.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteConcurrencyMerge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcNoteConcurrencyMerge.Location = new System.Drawing.Point(673, 195);
			this.checkProcNoteConcurrencyMerge.Name = "checkProcNoteConcurrencyMerge";
			this.checkProcNoteConcurrencyMerge.Size = new System.Drawing.Size(342, 15);
			this.checkProcNoteConcurrencyMerge.TabIndex = 229;
			this.checkProcNoteConcurrencyMerge.Text = "Procedure notes merge together when concurrency issues occur";
			this.checkProcNoteConcurrencyMerge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteConcurrencyMerge.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(123, 68);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(209, 15);
			this.label8.TabIndex = 197;
			this.label8.Text = "Indicator patient has no problems";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProcCodeListSort
			// 
			this.comboProcCodeListSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProcCodeListSort.FormattingEnabled = true;
			this.comboProcCodeListSort.Location = new System.Drawing.Point(762, 168);
			this.comboProcCodeListSort.MaxDropDownItems = 30;
			this.comboProcCodeListSort.Name = "comboProcCodeListSort";
			this.comboProcCodeListSort.Size = new System.Drawing.Size(255, 21);
			this.comboProcCodeListSort.TabIndex = 219;
			// 
			// checkProcProvChangesCp
			// 
			this.checkProcProvChangesCp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcProvChangesCp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcProvChangesCp.Location = new System.Drawing.Point(70, 396);
			this.checkProcProvChangesCp.Name = "checkProcProvChangesCp";
			this.checkProcProvChangesCp.Size = new System.Drawing.Size(435, 17);
			this.checkProcProvChangesCp.TabIndex = 227;
			this.checkProcProvChangesCp.Text = "Do not allow different procedure and claim procedure providers when attached to c" +
    "laim";
			this.checkProcProvChangesCp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcProvChangesCp.UseVisualStyleBackColor = true;
			// 
			// labelToothNomenclature
			// 
			this.labelToothNomenclature.Location = new System.Drawing.Point(648, 70);
			this.labelToothNomenclature.Name = "labelToothNomenclature";
			this.labelToothNomenclature.Size = new System.Drawing.Size(112, 15);
			this.labelToothNomenclature.TabIndex = 196;
			this.labelToothNomenclature.Text = "Tooth Nomenclature";
			this.labelToothNomenclature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProcFeeUpdatePrompt
			// 
			this.comboProcFeeUpdatePrompt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProcFeeUpdatePrompt.FormattingEnabled = true;
			this.comboProcFeeUpdatePrompt.Location = new System.Drawing.Point(286, 370);
			this.comboProcFeeUpdatePrompt.Name = "comboProcFeeUpdatePrompt";
			this.comboProcFeeUpdatePrompt.Size = new System.Drawing.Size(219, 21);
			this.comboProcFeeUpdatePrompt.TabIndex = 225;
			// 
			// checkPerioTreatImplantsAsNotMissing
			// 
			this.checkPerioTreatImplantsAsNotMissing.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPerioTreatImplantsAsNotMissing.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPerioTreatImplantsAsNotMissing.Location = new System.Drawing.Point(720, 148);
			this.checkPerioTreatImplantsAsNotMissing.Name = "checkPerioTreatImplantsAsNotMissing";
			this.checkPerioTreatImplantsAsNotMissing.Size = new System.Drawing.Size(295, 15);
			this.checkPerioTreatImplantsAsNotMissing.TabIndex = 216;
			this.checkPerioTreatImplantsAsNotMissing.Text = "Perio exams treat implants as not missing";
			this.checkPerioTreatImplantsAsNotMissing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPerioTreatImplantsAsNotMissing.UseVisualStyleBackColor = true;
			// 
			// labelProcFeeUpdatePrompt
			// 
			this.labelProcFeeUpdatePrompt.Location = new System.Drawing.Point(123, 373);
			this.labelProcFeeUpdatePrompt.Name = "labelProcFeeUpdatePrompt";
			this.labelProcFeeUpdatePrompt.Size = new System.Drawing.Size(161, 15);
			this.labelProcFeeUpdatePrompt.TabIndex = 226;
			this.labelProcFeeUpdatePrompt.Text = "Procedure fee update behavior";
			this.labelProcFeeUpdatePrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAutoClearEntryStatus
			// 
			this.checkAutoClearEntryStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoClearEntryStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAutoClearEntryStatus.Location = new System.Drawing.Point(720, 47);
			this.checkAutoClearEntryStatus.Name = "checkAutoClearEntryStatus";
			this.checkAutoClearEntryStatus.Size = new System.Drawing.Size(295, 15);
			this.checkAutoClearEntryStatus.TabIndex = 73;
			this.checkAutoClearEntryStatus.Text = "Reset entry status to \'TreatPlan\' when switching patients";
			this.checkAutoClearEntryStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoClearEntryStatus.UseVisualStyleBackColor = true;
			// 
			// butProblemsIndicateNone
			// 
			this.butProblemsIndicateNone.Location = new System.Drawing.Point(484, 65);
			this.butProblemsIndicateNone.Name = "butProblemsIndicateNone";
			this.butProblemsIndicateNone.Size = new System.Drawing.Size(21, 21);
			this.butProblemsIndicateNone.TabIndex = 199;
			this.butProblemsIndicateNone.Text = "...";
			this.butProblemsIndicateNone.Click += new System.EventHandler(this.butProblemsIndicateNone_Click);
			// 
			// checkPerioSkipMissingTeeth
			// 
			this.checkPerioSkipMissingTeeth.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPerioSkipMissingTeeth.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPerioSkipMissingTeeth.Location = new System.Drawing.Point(720, 130);
			this.checkPerioSkipMissingTeeth.Name = "checkPerioSkipMissingTeeth";
			this.checkPerioSkipMissingTeeth.Size = new System.Drawing.Size(295, 15);
			this.checkPerioSkipMissingTeeth.TabIndex = 215;
			this.checkPerioSkipMissingTeeth.Text = "Perio exams always skip missing teeth";
			this.checkPerioSkipMissingTeeth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPerioSkipMissingTeeth.UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(123, 93);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(209, 15);
			this.label9.TabIndex = 200;
			this.label9.Text = "Indicator patient has no medications";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkProcGroupNoteDoesAggregate
			// 
			this.checkProcGroupNoteDoesAggregate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcGroupNoteDoesAggregate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcGroupNoteDoesAggregate.Location = new System.Drawing.Point(720, 94);
			this.checkProcGroupNoteDoesAggregate.Name = "checkProcGroupNoteDoesAggregate";
			this.checkProcGroupNoteDoesAggregate.Size = new System.Drawing.Size(295, 15);
			this.checkProcGroupNoteDoesAggregate.TabIndex = 206;
			this.checkProcGroupNoteDoesAggregate.Text = "Procedure Group Notes aggregate";
			this.checkProcGroupNoteDoesAggregate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcGroupNoteDoesAggregate.UseVisualStyleBackColor = true;
			// 
			// textMedicationsIndicateNone
			// 
			this.textMedicationsIndicateNone.Location = new System.Drawing.Point(334, 90);
			this.textMedicationsIndicateNone.Name = "textMedicationsIndicateNone";
			this.textMedicationsIndicateNone.ReadOnly = true;
			this.textMedicationsIndicateNone.Size = new System.Drawing.Size(146, 20);
			this.textMedicationsIndicateNone.TabIndex = 201;
			// 
			// checkProvColorChart
			// 
			this.checkProvColorChart.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProvColorChart.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProvColorChart.Location = new System.Drawing.Point(720, 112);
			this.checkProvColorChart.Name = "checkProvColorChart";
			this.checkProvColorChart.Size = new System.Drawing.Size(295, 15);
			this.checkProvColorChart.TabIndex = 214;
			this.checkProvColorChart.Text = "Use provider color in chart";
			this.checkProvColorChart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProvColorChart.UseVisualStyleBackColor = true;
			// 
			// checkSignatureAllowDigital
			// 
			this.checkSignatureAllowDigital.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSignatureAllowDigital.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSignatureAllowDigital.Location = new System.Drawing.Point(169, 348);
			this.checkSignatureAllowDigital.Name = "checkSignatureAllowDigital";
			this.checkSignatureAllowDigital.Size = new System.Drawing.Size(336, 17);
			this.checkSignatureAllowDigital.TabIndex = 223;
			this.checkSignatureAllowDigital.Text = "Allow digital signatures";
			this.checkSignatureAllowDigital.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSignatureAllowDigital.UseVisualStyleBackColor = true;
			// 
			// textAllergiesIndicateNone
			// 
			this.textAllergiesIndicateNone.Location = new System.Drawing.Point(334, 115);
			this.textAllergiesIndicateNone.Name = "textAllergiesIndicateNone";
			this.textAllergiesIndicateNone.ReadOnly = true;
			this.textAllergiesIndicateNone.Size = new System.Drawing.Size(146, 20);
			this.textAllergiesIndicateNone.TabIndex = 204;
			// 
			// butMedicationsIndicateNone
			// 
			this.butMedicationsIndicateNone.Location = new System.Drawing.Point(484, 90);
			this.butMedicationsIndicateNone.Name = "butMedicationsIndicateNone";
			this.butMedicationsIndicateNone.Size = new System.Drawing.Size(21, 21);
			this.butMedicationsIndicateNone.TabIndex = 202;
			this.butMedicationsIndicateNone.Text = "...";
			this.butMedicationsIndicateNone.Click += new System.EventHandler(this.butMedicationsIndicateNone_Click);
			// 
			// textMedDefaultStopDays
			// 
			this.textMedDefaultStopDays.Location = new System.Drawing.Point(466, 243);
			this.textMedDefaultStopDays.Name = "textMedDefaultStopDays";
			this.textMedDefaultStopDays.Size = new System.Drawing.Size(39, 20);
			this.textMedDefaultStopDays.TabIndex = 212;
			// 
			// checkClaimProcsAllowEstimatesOnCompl
			// 
			this.checkClaimProcsAllowEstimatesOnCompl.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimProcsAllowEstimatesOnCompl.Checked = true;
			this.checkClaimProcsAllowEstimatesOnCompl.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkClaimProcsAllowEstimatesOnCompl.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimProcsAllowEstimatesOnCompl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.checkClaimProcsAllowEstimatesOnCompl.Location = new System.Drawing.Point(169, 320);
			this.checkClaimProcsAllowEstimatesOnCompl.Name = "checkClaimProcsAllowEstimatesOnCompl";
			this.checkClaimProcsAllowEstimatesOnCompl.Size = new System.Drawing.Size(336, 25);
			this.checkClaimProcsAllowEstimatesOnCompl.TabIndex = 222;
			this.checkClaimProcsAllowEstimatesOnCompl.Text = "Allow estimates to be created for backdated completed procedures\r\n(not recommende" +
    "d, see manual)";
			this.checkClaimProcsAllowEstimatesOnCompl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimProcsAllowEstimatesOnCompl.UseVisualStyleBackColor = true;
			this.checkClaimProcsAllowEstimatesOnCompl.CheckedChanged += new System.EventHandler(this.checkClaimProcsAllowEstimatesOnCompl_CheckedChanged);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(104, 246);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(361, 15);
			this.label11.TabIndex = 213;
			this.label11.Text = "Medication order default days until stop date (0 for no automatic stop date)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkProcEditRequireAutoCode
			// 
			this.checkProcEditRequireAutoCode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcEditRequireAutoCode.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcEditRequireAutoCode.Location = new System.Drawing.Point(169, 302);
			this.checkProcEditRequireAutoCode.Name = "checkProcEditRequireAutoCode";
			this.checkProcEditRequireAutoCode.Size = new System.Drawing.Size(336, 17);
			this.checkProcEditRequireAutoCode.TabIndex = 221;
			this.checkProcEditRequireAutoCode.Text = "Require use of suggested auto codes";
			this.checkProcEditRequireAutoCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcEditRequireAutoCode.UseVisualStyleBackColor = true;
			// 
			// checkChartNonPatientWarn
			// 
			this.checkChartNonPatientWarn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkChartNonPatientWarn.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkChartNonPatientWarn.Location = new System.Drawing.Point(169, 223);
			this.checkChartNonPatientWarn.Name = "checkChartNonPatientWarn";
			this.checkChartNonPatientWarn.Size = new System.Drawing.Size(336, 17);
			this.checkChartNonPatientWarn.TabIndex = 211;
			this.checkChartNonPatientWarn.Text = "Non-Patient warning";
			this.checkChartNonPatientWarn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkChartNonPatientWarn.UseVisualStyleBackColor = true;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(123, 118);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(209, 15);
			this.label14.TabIndex = 203;
			this.label14.Text = "Indicator patient has no allergies";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkProcLockingIsAllowed
			// 
			this.checkProcLockingIsAllowed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcLockingIsAllowed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcLockingIsAllowed.Location = new System.Drawing.Point(169, 204);
			this.checkProcLockingIsAllowed.Name = "checkProcLockingIsAllowed";
			this.checkProcLockingIsAllowed.Size = new System.Drawing.Size(336, 17);
			this.checkProcLockingIsAllowed.TabIndex = 210;
			this.checkProcLockingIsAllowed.Text = "Procedure locking is allowed";
			this.checkProcLockingIsAllowed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcLockingIsAllowed.UseVisualStyleBackColor = true;
			this.checkProcLockingIsAllowed.Click += new System.EventHandler(this.checkProcLockingIsAllowed_Click);
			// 
			// checkProcsPromptForAutoNote
			// 
			this.checkProcsPromptForAutoNote.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcsPromptForAutoNote.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcsPromptForAutoNote.Location = new System.Drawing.Point(169, 284);
			this.checkProcsPromptForAutoNote.Name = "checkProcsPromptForAutoNote";
			this.checkProcsPromptForAutoNote.Size = new System.Drawing.Size(336, 17);
			this.checkProcsPromptForAutoNote.TabIndex = 218;
			this.checkProcsPromptForAutoNote.Text = "Procedures Prompt For Auto Note";
			this.checkProcsPromptForAutoNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcsPromptForAutoNote.UseVisualStyleBackColor = true;
			// 
			// textICD9DefaultForNewProcs
			// 
			this.textICD9DefaultForNewProcs.Location = new System.Drawing.Point(395, 178);
			this.textICD9DefaultForNewProcs.Name = "textICD9DefaultForNewProcs";
			this.textICD9DefaultForNewProcs.Size = new System.Drawing.Size(85, 20);
			this.textICD9DefaultForNewProcs.TabIndex = 209;
			// 
			// labelIcdCodeDefault
			// 
			this.labelIcdCodeDefault.Location = new System.Drawing.Point(6, 181);
			this.labelIcdCodeDefault.Name = "labelIcdCodeDefault";
			this.labelIcdCodeDefault.Size = new System.Drawing.Size(388, 15);
			this.labelIcdCodeDefault.TabIndex = 203;
			this.labelIcdCodeDefault.Text = "Default ICD-10 code for new procedures and when set complete";
			this.labelIcdCodeDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkScreeningsUseSheets
			// 
			this.checkScreeningsUseSheets.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkScreeningsUseSheets.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkScreeningsUseSheets.Location = new System.Drawing.Point(169, 266);
			this.checkScreeningsUseSheets.Name = "checkScreeningsUseSheets";
			this.checkScreeningsUseSheets.Size = new System.Drawing.Size(336, 17);
			this.checkScreeningsUseSheets.TabIndex = 217;
			this.checkScreeningsUseSheets.Text = "Screenings use Sheets";
			this.checkScreeningsUseSheets.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkScreeningsUseSheets.UseVisualStyleBackColor = true;
			// 
			// butDiagnosisCode
			// 
			this.butDiagnosisCode.Location = new System.Drawing.Point(484, 178);
			this.butDiagnosisCode.Name = "butDiagnosisCode";
			this.butDiagnosisCode.Size = new System.Drawing.Size(21, 21);
			this.butDiagnosisCode.TabIndex = 213;
			this.butDiagnosisCode.Text = "...";
			this.butDiagnosisCode.Click += new System.EventHandler(this.butDiagnosisCode_Click);
			// 
			// butAllergiesIndicateNone
			// 
			this.butAllergiesIndicateNone.Location = new System.Drawing.Point(484, 115);
			this.butAllergiesIndicateNone.Name = "butAllergiesIndicateNone";
			this.butAllergiesIndicateNone.Size = new System.Drawing.Size(21, 21);
			this.butAllergiesIndicateNone.TabIndex = 205;
			this.butAllergiesIndicateNone.Text = "...";
			this.butAllergiesIndicateNone.Click += new System.EventHandler(this.butAllergiesIndicateNone_Click);
			// 
			// checkDxIcdVersion
			// 
			this.checkDxIcdVersion.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDxIcdVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDxIcdVersion.Location = new System.Drawing.Point(169, 159);
			this.checkDxIcdVersion.Name = "checkDxIcdVersion";
			this.checkDxIcdVersion.Size = new System.Drawing.Size(336, 17);
			this.checkDxIcdVersion.TabIndex = 212;
			this.checkDxIcdVersion.Text = "Use ICD-10 Diagnosis Codes (uncheck for ICD-9)";
			this.checkDxIcdVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDxIcdVersion.UseVisualStyleBackColor = true;
			this.checkDxIcdVersion.Click += new System.EventHandler(this.checkDxIcdVersion_Click);
			// 
			// checkMedicalFeeUsedForNewProcs
			// 
			this.checkMedicalFeeUsedForNewProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicalFeeUsedForNewProcs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMedicalFeeUsedForNewProcs.Location = new System.Drawing.Point(169, 141);
			this.checkMedicalFeeUsedForNewProcs.Name = "checkMedicalFeeUsedForNewProcs";
			this.checkMedicalFeeUsedForNewProcs.Size = new System.Drawing.Size(336, 17);
			this.checkMedicalFeeUsedForNewProcs.TabIndex = 208;
			this.checkMedicalFeeUsedForNewProcs.Text = "Use medical fee for new procedures";
			this.checkMedicalFeeUsedForNewProcs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicalFeeUsedForNewProcs.UseVisualStyleBackColor = true;
			// 
			// tabImages
			// 
			this.tabImages.BackColor = System.Drawing.SystemColors.Control;
			this.tabImages.Controls.Add(this.checkPDFLaunchWindow);
			this.tabImages.Controls.Add(this.label61);
			this.tabImages.Controls.Add(this.textDefaultImageImportFolder);
			this.tabImages.Controls.Add(this.label15);
			this.tabImages.Controls.Add(this.textAutoImportFolder);
			this.tabImages.Location = new System.Drawing.Point(4, 22);
			this.tabImages.Name = "tabImages";
			this.tabImages.Padding = new System.Windows.Forms.Padding(3);
			this.tabImages.Size = new System.Drawing.Size(1227, 640);
			this.tabImages.TabIndex = 5;
			this.tabImages.Text = "Imaging";
			// 
			// checkPDFLaunchWindow
			// 
			this.checkPDFLaunchWindow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPDFLaunchWindow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPDFLaunchWindow.Location = new System.Drawing.Point(15, 91);
			this.checkPDFLaunchWindow.Name = "checkPDFLaunchWindow";
			this.checkPDFLaunchWindow.Size = new System.Drawing.Size(440, 15);
			this.checkPDFLaunchWindow.TabIndex = 288;
			this.checkPDFLaunchWindow.Text = "PDF files always launch in a separate window (can help with Remote Desktop)";
			this.checkPDFLaunchWindow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPDFLaunchWindow.UseVisualStyleBackColor = true;
			// 
			// label61
			// 
			this.label61.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label61.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label61.Location = new System.Drawing.Point(102, 42);
			this.label61.Name = "label61";
			this.label61.Size = new System.Drawing.Size(164, 17);
			this.label61.TabIndex = 283;
			this.label61.Text = "Default import folder";
			this.label61.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDefaultImageImportFolder
			// 
			this.textDefaultImageImportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDefaultImageImportFolder.Location = new System.Drawing.Point(270, 39);
			this.textDefaultImageImportFolder.Name = "textDefaultImageImportFolder";
			this.textDefaultImageImportFolder.Size = new System.Drawing.Size(244, 20);
			this.textDefaultImageImportFolder.TabIndex = 282;
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label15.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label15.Location = new System.Drawing.Point(80, 68);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(186, 17);
			this.label15.TabIndex = 281;
			this.label15.Text = "Default folder for automatic import";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAutoImportFolder
			// 
			this.textAutoImportFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textAutoImportFolder.Location = new System.Drawing.Point(270, 65);
			this.textAutoImportFolder.Name = "textAutoImportFolder";
			this.textAutoImportFolder.Size = new System.Drawing.Size(244, 20);
			this.textAutoImportFolder.TabIndex = 280;
			// 
			// tabManage
			// 
			this.tabManage.BackColor = System.Drawing.SystemColors.Control;
			this.tabManage.Controls.Add(this.comboEraAutomation);
			this.tabManage.Controls.Add(this.labelEraAutomation);
			this.tabManage.Controls.Add(this.comboDepositSoftware);
			this.tabManage.Controls.Add(this.labelDepositSoftware);
			this.tabManage.Controls.Add(this.checkEraAllowTotalPayment);
			this.tabManage.Controls.Add(this.checkIncludeEraWOPercCoPay);
			this.tabManage.Controls.Add(this.checkClockEventAllowBreak);
			this.tabManage.Controls.Add(this.textClaimsReceivedDays);
			this.tabManage.Controls.Add(this.checkShowAutoDeposit);
			this.tabManage.Controls.Add(this.checkEraOneClaimPerPage);
			this.tabManage.Controls.Add(this.checkClaimPaymentBatchOnly);
			this.tabManage.Controls.Add(this.labelClaimsReceivedDays);
			this.tabManage.Controls.Add(this.checkScheduleProvEmpSelectAll);
			this.tabManage.Controls.Add(this.checkClaimsSendWindowValidateOnLoad);
			this.tabManage.Controls.Add(this.checkTimeCardADP);
			this.tabManage.Controls.Add(this.groupBox1);
			this.tabManage.Controls.Add(this.comboTimeCardOvertimeFirstDayOfWeek);
			this.tabManage.Controls.Add(this.label16);
			this.tabManage.Controls.Add(this.checkRxSendNewToQueue);
			this.tabManage.Location = new System.Drawing.Point(4, 22);
			this.tabManage.Name = "tabManage";
			this.tabManage.Padding = new System.Windows.Forms.Padding(3);
			this.tabManage.Size = new System.Drawing.Size(1227, 640);
			this.tabManage.TabIndex = 6;
			this.tabManage.Text = "Manage";
			// 
			// comboEraAutomation
			// 
			this.comboEraAutomation.Location = new System.Drawing.Point(381, 543);
			this.comboEraAutomation.Name = "comboEraAutomation";
			this.comboEraAutomation.Size = new System.Drawing.Size(130, 21);
			this.comboEraAutomation.TabIndex = 13;
			// 
			// labelEraAutomation
			// 
			this.labelEraAutomation.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelEraAutomation.Location = new System.Drawing.Point(190, 546);
			this.labelEraAutomation.Name = "labelEraAutomation";
			this.labelEraAutomation.Size = new System.Drawing.Size(189, 15);
			this.labelEraAutomation.TabIndex = 288;
			this.labelEraAutomation.Text = "ERA Automation";
			this.labelEraAutomation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDepositSoftware
			// 
			this.comboDepositSoftware.Location = new System.Drawing.Point(381, 568);
			this.comboDepositSoftware.Name = "comboDepositSoftware";
			this.comboDepositSoftware.Size = new System.Drawing.Size(130, 21);
			this.comboDepositSoftware.TabIndex = 14;
			this.comboDepositSoftware.SelectionChangeCommitted += new System.EventHandler(this.comboDepositSoftware_SelectionChangeCommitted);
			// 
			// labelDepositSoftware
			// 
			this.labelDepositSoftware.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelDepositSoftware.Location = new System.Drawing.Point(187, 571);
			this.labelDepositSoftware.Name = "labelDepositSoftware";
			this.labelDepositSoftware.Size = new System.Drawing.Size(192, 15);
			this.labelDepositSoftware.TabIndex = 253;
			this.labelDepositSoftware.Text = "Deposit Software";
			this.labelDepositSoftware.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEraAllowTotalPayment
			// 
			this.checkEraAllowTotalPayment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraAllowTotalPayment.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEraAllowTotalPayment.Location = new System.Drawing.Point(89, 524);
			this.checkEraAllowTotalPayment.Name = "checkEraAllowTotalPayment";
			this.checkEraAllowTotalPayment.Size = new System.Drawing.Size(421, 17);
			this.checkEraAllowTotalPayment.TabIndex = 12;
			this.checkEraAllowTotalPayment.Text = "ERA allow total payments";
			this.checkEraAllowTotalPayment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIncludeEraWOPercCoPay
			// 
			this.checkIncludeEraWOPercCoPay.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeEraWOPercCoPay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeEraWOPercCoPay.Location = new System.Drawing.Point(73, 469);
			this.checkIncludeEraWOPercCoPay.Name = "checkIncludeEraWOPercCoPay";
			this.checkIncludeEraWOPercCoPay.Size = new System.Drawing.Size(437, 17);
			this.checkIncludeEraWOPercCoPay.TabIndex = 9;
			this.checkIncludeEraWOPercCoPay.Text = "ERA posts write-offs for Category Percentage and Medicaid/Flat Copay";
			this.checkIncludeEraWOPercCoPay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClockEventAllowBreak
			// 
			this.checkClockEventAllowBreak.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClockEventAllowBreak.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClockEventAllowBreak.Location = new System.Drawing.Point(89, 505);
			this.checkClockEventAllowBreak.Name = "checkClockEventAllowBreak";
			this.checkClockEventAllowBreak.Size = new System.Drawing.Size(421, 17);
			this.checkClockEventAllowBreak.TabIndex = 11;
			this.checkClockEventAllowBreak.Text = "Allow paid 30 minute breaks";
			this.checkClockEventAllowBreak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClaimsReceivedDays
			// 
			this.textClaimsReceivedDays.Location = new System.Drawing.Point(450, 411);
			this.textClaimsReceivedDays.MaxVal = 999999;
			this.textClaimsReceivedDays.MinVal = 1;
			this.textClaimsReceivedDays.Name = "textClaimsReceivedDays";
			this.textClaimsReceivedDays.ShowZero = false;
			this.textClaimsReceivedDays.Size = new System.Drawing.Size(60, 20);
			this.textClaimsReceivedDays.TabIndex = 6;
			this.textClaimsReceivedDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkShowAutoDeposit
			// 
			this.checkShowAutoDeposit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAutoDeposit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowAutoDeposit.Location = new System.Drawing.Point(88, 487);
			this.checkShowAutoDeposit.Name = "checkShowAutoDeposit";
			this.checkShowAutoDeposit.Size = new System.Drawing.Size(422, 17);
			this.checkShowAutoDeposit.TabIndex = 10;
			this.checkShowAutoDeposit.Text = "Insurance payments show auto deposit";
			this.checkShowAutoDeposit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEraOneClaimPerPage
			// 
			this.checkEraOneClaimPerPage.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraOneClaimPerPage.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEraOneClaimPerPage.Location = new System.Drawing.Point(89, 451);
			this.checkEraOneClaimPerPage.Name = "checkEraOneClaimPerPage";
			this.checkEraOneClaimPerPage.Size = new System.Drawing.Size(421, 17);
			this.checkEraOneClaimPerPage.TabIndex = 8;
			this.checkEraOneClaimPerPage.Text = "ERA prints one page per claim";
			this.checkEraOneClaimPerPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimPaymentBatchOnly
			// 
			this.checkClaimPaymentBatchOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimPaymentBatchOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimPaymentBatchOnly.Location = new System.Drawing.Point(89, 434);
			this.checkClaimPaymentBatchOnly.Name = "checkClaimPaymentBatchOnly";
			this.checkClaimPaymentBatchOnly.Size = new System.Drawing.Size(421, 17);
			this.checkClaimPaymentBatchOnly.TabIndex = 7;
			this.checkClaimPaymentBatchOnly.Text = "Finalize claim payments in Batch Insurance window only";
			this.checkClaimPaymentBatchOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClaimsReceivedDays
			// 
			this.labelClaimsReceivedDays.Location = new System.Drawing.Point(88, 411);
			this.labelClaimsReceivedDays.MaximumSize = new System.Drawing.Size(1000, 300);
			this.labelClaimsReceivedDays.Name = "labelClaimsReceivedDays";
			this.labelClaimsReceivedDays.Size = new System.Drawing.Size(361, 20);
			this.labelClaimsReceivedDays.TabIndex = 203;
			this.labelClaimsReceivedDays.Text = "Show claims received after days (blank to disable)";
			this.labelClaimsReceivedDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkScheduleProvEmpSelectAll
			// 
			this.checkScheduleProvEmpSelectAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkScheduleProvEmpSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkScheduleProvEmpSelectAll.Location = new System.Drawing.Point(90, 125);
			this.checkScheduleProvEmpSelectAll.Name = "checkScheduleProvEmpSelectAll";
			this.checkScheduleProvEmpSelectAll.Size = new System.Drawing.Size(421, 17);
			this.checkScheduleProvEmpSelectAll.TabIndex = 4;
			this.checkScheduleProvEmpSelectAll.Text = "Select all provider/employees when loading schedules";
			this.checkScheduleProvEmpSelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClaimsSendWindowValidateOnLoad
			// 
			this.checkClaimsSendWindowValidateOnLoad.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimsSendWindowValidateOnLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClaimsSendWindowValidateOnLoad.Location = new System.Drawing.Point(90, 108);
			this.checkClaimsSendWindowValidateOnLoad.Name = "checkClaimsSendWindowValidateOnLoad";
			this.checkClaimsSendWindowValidateOnLoad.Size = new System.Drawing.Size(421, 17);
			this.checkClaimsSendWindowValidateOnLoad.TabIndex = 3;
			this.checkClaimsSendWindowValidateOnLoad.Text = "Claims Send window validate on load (can cause slowness)";
			this.checkClaimsSendWindowValidateOnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTimeCardADP
			// 
			this.checkTimeCardADP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeCardADP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTimeCardADP.Location = new System.Drawing.Point(152, 91);
			this.checkTimeCardADP.Name = "checkTimeCardADP";
			this.checkTimeCardADP.Size = new System.Drawing.Size(359, 17);
			this.checkTimeCardADP.TabIndex = 2;
			this.checkTimeCardADP.Text = "ADP export includes employee name";
			this.checkTimeCardADP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBox1.Controls.Add(this.checkStatementsAlphabetically);
			this.groupBox1.Controls.Add(this.checkBillingShowProgress);
			this.groupBox1.Controls.Add(this.label24);
			this.groupBox1.Controls.Add(this.textBillingElectBatchMax);
			this.groupBox1.Controls.Add(this.checkStatementShowAdjNotes);
			this.groupBox1.Controls.Add(this.checkIntermingleDefault);
			this.groupBox1.Controls.Add(this.checkStatementShowReturnAddress);
			this.groupBox1.Controls.Add(this.checkStatementShowProcBreakdown);
			this.groupBox1.Controls.Add(this.checkStatementShowNotes);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.comboUseChartNum);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.textStatementsCalcDueDate);
			this.groupBox1.Controls.Add(this.textPayPlansBillInAdvanceDays);
			this.groupBox1.Location = new System.Drawing.Point(108, 145);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(413, 261);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Billing and Statements";
			// 
			// checkStatementsAlphabetically
			// 
			this.checkStatementsAlphabetically.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementsAlphabetically.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatementsAlphabetically.Location = new System.Drawing.Point(25, 239);
			this.checkStatementsAlphabetically.Name = "checkStatementsAlphabetically";
			this.checkStatementsAlphabetically.Size = new System.Drawing.Size(377, 16);
			this.checkStatementsAlphabetically.TabIndex = 10;
			this.checkStatementsAlphabetically.Text = "Print statements alphabetically";
			this.checkStatementsAlphabetically.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBillingShowProgress
			// 
			this.checkBillingShowProgress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBillingShowProgress.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBillingShowProgress.Location = new System.Drawing.Point(25, 220);
			this.checkBillingShowProgress.Name = "checkBillingShowProgress";
			this.checkBillingShowProgress.Size = new System.Drawing.Size(377, 16);
			this.checkBillingShowProgress.TabIndex = 9;
			this.checkBillingShowProgress.Text = "Show progress when sending statements";
			this.checkBillingShowProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(25, 193);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(316, 20);
			this.label24.TabIndex = 217;
			this.label24.Text = "Max number of statements per batch (0 for no limit)";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBillingElectBatchMax
			// 
			this.textBillingElectBatchMax.Location = new System.Drawing.Point(342, 194);
			this.textBillingElectBatchMax.Name = "textBillingElectBatchMax";
			this.textBillingElectBatchMax.Size = new System.Drawing.Size(60, 20);
			this.textBillingElectBatchMax.TabIndex = 8;
			this.textBillingElectBatchMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkStatementShowAdjNotes
			// 
			this.checkStatementShowAdjNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowAdjNotes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatementShowAdjNotes.Location = new System.Drawing.Point(34, 45);
			this.checkStatementShowAdjNotes.Name = "checkStatementShowAdjNotes";
			this.checkStatementShowAdjNotes.Size = new System.Drawing.Size(368, 17);
			this.checkStatementShowAdjNotes.TabIndex = 2;
			this.checkStatementShowAdjNotes.Text = "Show notes for adjustments";
			this.checkStatementShowAdjNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIntermingleDefault
			// 
			this.checkIntermingleDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIntermingleDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIntermingleDefault.Location = new System.Drawing.Point(25, 172);
			this.checkIntermingleDefault.Name = "checkIntermingleDefault";
			this.checkIntermingleDefault.Size = new System.Drawing.Size(377, 16);
			this.checkIntermingleDefault.TabIndex = 7;
			this.checkIntermingleDefault.Text = "Account Module statements default to intermingled mode";
			this.checkIntermingleDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkStatementShowReturnAddress
			// 
			this.checkStatementShowReturnAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowReturnAddress.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatementShowReturnAddress.Location = new System.Drawing.Point(125, 11);
			this.checkStatementShowReturnAddress.Name = "checkStatementShowReturnAddress";
			this.checkStatementShowReturnAddress.Size = new System.Drawing.Size(277, 17);
			this.checkStatementShowReturnAddress.TabIndex = 0;
			this.checkStatementShowReturnAddress.Text = "Show return address";
			this.checkStatementShowReturnAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkStatementShowProcBreakdown
			// 
			this.checkStatementShowProcBreakdown.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowProcBreakdown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatementShowProcBreakdown.Location = new System.Drawing.Point(34, 62);
			this.checkStatementShowProcBreakdown.Name = "checkStatementShowProcBreakdown";
			this.checkStatementShowProcBreakdown.Size = new System.Drawing.Size(368, 17);
			this.checkStatementShowProcBreakdown.TabIndex = 3;
			this.checkStatementShowProcBreakdown.Text = "Show procedure breakdown";
			this.checkStatementShowProcBreakdown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkStatementShowNotes
			// 
			this.checkStatementShowNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkStatementShowNotes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkStatementShowNotes.Location = new System.Drawing.Point(34, 28);
			this.checkStatementShowNotes.Name = "checkStatementShowNotes";
			this.checkStatementShowNotes.Size = new System.Drawing.Size(368, 17);
			this.checkStatementShowNotes.TabIndex = 1;
			this.checkStatementShowNotes.Text = "Show notes for payments";
			this.checkStatementShowNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(22, 109);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(318, 27);
			this.label2.TabIndex = 204;
			this.label2.Text = "Days to calculate due date (Usually 10 or 15.  Leave blank to show \"Due on Receip" +
    "t\")";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboUseChartNum
			// 
			this.comboUseChartNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUseChartNum.FormattingEnabled = true;
			this.comboUseChartNum.Location = new System.Drawing.Point(273, 82);
			this.comboUseChartNum.Name = "comboUseChartNum";
			this.comboUseChartNum.Size = new System.Drawing.Size(130, 21);
			this.comboUseChartNum.TabIndex = 4;
			// 
			// label10
			// 
			this.label10.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label10.Location = new System.Drawing.Point(76, 85);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(195, 15);
			this.label10.TabIndex = 208;
			this.label10.Text = "Account Numbers use";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label18
			// 
			this.label18.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label18.Location = new System.Drawing.Point(23, 141);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(318, 27);
			this.label18.TabIndex = 209;
			this.label18.Text = "Days in advance to bill payment plan amounts due\r\n(Usually 10 or 15)";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textStatementsCalcDueDate
			// 
			this.textStatementsCalcDueDate.Location = new System.Drawing.Point(343, 113);
			this.textStatementsCalcDueDate.Name = "textStatementsCalcDueDate";
			this.textStatementsCalcDueDate.ShowZero = false;
			this.textStatementsCalcDueDate.Size = new System.Drawing.Size(60, 20);
			this.textStatementsCalcDueDate.TabIndex = 5;
			this.textStatementsCalcDueDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPayPlansBillInAdvanceDays
			// 
			this.textPayPlansBillInAdvanceDays.Location = new System.Drawing.Point(343, 145);
			this.textPayPlansBillInAdvanceDays.Name = "textPayPlansBillInAdvanceDays";
			this.textPayPlansBillInAdvanceDays.Size = new System.Drawing.Size(60, 20);
			this.textPayPlansBillInAdvanceDays.TabIndex = 6;
			this.textPayPlansBillInAdvanceDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// comboTimeCardOvertimeFirstDayOfWeek
			// 
			this.comboTimeCardOvertimeFirstDayOfWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboTimeCardOvertimeFirstDayOfWeek.FormattingEnabled = true;
			this.comboTimeCardOvertimeFirstDayOfWeek.Location = new System.Drawing.Point(340, 64);
			this.comboTimeCardOvertimeFirstDayOfWeek.Name = "comboTimeCardOvertimeFirstDayOfWeek";
			this.comboTimeCardOvertimeFirstDayOfWeek.Size = new System.Drawing.Size(170, 21);
			this.comboTimeCardOvertimeFirstDayOfWeek.TabIndex = 1;
			// 
			// label16
			// 
			this.label16.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label16.Location = new System.Drawing.Point(90, 67);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(248, 15);
			this.label16.TabIndex = 196;
			this.label16.Text = "Time Card first day of week for overtime";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkRxSendNewToQueue
			// 
			this.checkRxSendNewToQueue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRxSendNewToQueue.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRxSendNewToQueue.Location = new System.Drawing.Point(151, 41);
			this.checkRxSendNewToQueue.Name = "checkRxSendNewToQueue";
			this.checkRxSendNewToQueue.Size = new System.Drawing.Size(359, 17);
			this.checkRxSendNewToQueue.TabIndex = 0;
			this.checkRxSendNewToQueue.Text = "Send all new prescriptions to electronic queue";
			this.checkRxSendNewToQueue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1157, 669);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(63, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1088, 669);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(63, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormModuleSetup
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormModuleSetup";
			this.ShowInTaskbar = false;
			this.Text = "Module Preferences";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormModuleSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormModuleSetup_Load);
			this.tabControlMain.ResumeLayout(false);
			this.tabAppts.ResumeLayout(false);
			this.tabAppts.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.tabFamily.ResumeLayout(false);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxCOB.ResumeLayout(false);
			this.groupBoxClaimSnapshot.ResumeLayout(false);
			this.groupBoxClaimSnapshot.PerformLayout();
			this.groupBoxSuperFamily.ResumeLayout(false);
			this.tabAccount.ResumeLayout(false);
			this.groupBox10.ResumeLayout(false);
			this.groupBoxOD3.ResumeLayout(false);
			this.groupBoxOD3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupBoxClaimIdPrefix.ResumeLayout(false);
			this.groupBoxClaimIdPrefix.PerformLayout();
			this.groupRepeatingCharges.ResumeLayout(false);
			this.groupRepeatingCharges.PerformLayout();
			this.groupRecurringCharges.ResumeLayout(false);
			this.groupRecurringCharges.PerformLayout();
			this.groupCommLogs.ResumeLayout(false);
			this.groupPayPlans.ResumeLayout(false);
			this.groupPayPlans.PerformLayout();
			this.tabTreatPlan.ResumeLayout(false);
			this.tabTreatPlan.PerformLayout();
			this.groupBoxOD4.ResumeLayout(false);
			this.groupBoxOD4.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupInsHist.ResumeLayout(false);
			this.groupInsHist.PerformLayout();
			this.groupTreatPlanSort.ResumeLayout(false);
			this.tabChart.ResumeLayout(false);
			this.tabChart.PerformLayout();
			this.tabImages.ResumeLayout(false);
			this.tabImages.PerformLayout();
			this.tabManage.ResumeLayout(false);
			this.tabManage.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion
		private System.Windows.Forms.CheckBox checkTreatPlanShowCompleted;
		private System.Windows.Forms.CheckBox checkBalancesDontSubtractIns;
		private CheckBox checkAgingMonthly;
		private CheckBox checkAutoClearEntryStatus;
		private CheckBox checkShowFamilyCommByDefault;
		private CheckBox checkAllowSettingProcsComplete;
		private CheckBox checkImagesModuleTreeIsCollapsed;
		private CheckBox checkRxSendNewToQueue;
		private CheckBox checkProcGroupNoteDoesAggregate;
		private CheckBox checkMedicalFeeUsedForNewProcs;
		private CheckBox checkAccountShowPaymentNums;
		private CheckBox checkIntermingleDefault;
		private CheckBox checkStatementShowReturnAddress;
		private CheckBox checkStatementShowProcBreakdown;
		private CheckBox checkStatementShowNotes;
		private CheckBox checkStatementShowAdjNotes;
		private CheckBox checkProcLockingIsAllowed;
		private CheckBox checkTimeCardADP;
		private CheckBox checkChartNonPatientWarn;
		private CheckBox checkTreatPlanItemized;
		private CheckBox checkClaimsSendWindowValidateOnLoad;
		private CheckBox checkProvColorChart;
		private CheckBox checkApptModuleDefaultToWeek;
		private CheckBox checkPerioSkipMissingTeeth;
		private CheckBox checkPerioTreatImplantsAsNotMissing;
		private CheckBox checkScreeningsUseSheets;
		private CheckBox checkTPSaveSigned;
		private CheckBox checkDxIcdVersion;
		private CheckBox checkSuperFamSync;
		private CheckBox checkPayPlansUseSheets;
		private CheckBox checkPpoUseUcr;
		private CheckBox checkProcsPromptForAutoNote;
		private CheckBox checkSuperFamAddIns;
		private CheckBox checkBillingShowProgress;
		private CheckBox checkProcEditRequireAutoCode;
		private CheckBox checkClaimProcsAllowEstimatesOnCompl;
		private CheckBox checkApptsCheckFrequency;
		private CheckBox checkPayPlansExcludePastActivity;
		private CheckBox checkScheduleProvEmpSelectAll;
		private CheckBox checkSignatureAllowDigital;
		private CheckBox checkClaimPaymentBatchOnly;
		private CheckBox checkStatementInvoiceGridShowWriteoffs;
		private CheckBox checkProcProvChangesCp;
		private CheckBox checkSuperFamCloneCreate;
		private CheckBox checkHideDueNow;
		private CheckBox checkBoxRxClinicUseSelected;
		private RadioButton radioTreatPlanSortTooth;
		private RadioButton radioTreatPlanSortOrder;
		private TabPage tabAppts;
		private TabPage tabFamily;
		private TabPage tabAccount;
		private TabPage tabTreatPlan;
		private TabPage tabChart;
		private TabPage tabImages;
		private TabPage tabManage;
		private System.Windows.Forms.Label label1;
		private Label label8;
		private Label label9;
		private Label label14;
		private Label label16;
		private Label label2;
		private Label label10;
		private Label label11;
		private Label label18;
		private Label label24;
		private Label label31;
		private Label label30;
		private Label label32;
		private Label label27;
		private Label labelIcdCodeDefault;
		private Label labelSuperFamSort;
		private Label labelProcFeeUpdatePrompt;
		private Label apptClickDelay;
		private Label labelClaimsReceivedDays;
		private ComboBox comboTimeCardOvertimeFirstDayOfWeek;
		private ComboBox comboUseChartNum;
		private UI.ComboBoxOD comboProcDiscountType;
		private ComboBox comboSuperFamSort;
		private ComboBox comboClaimSnapshotTrigger;
		private ComboBox comboProcCodeListSort;
		private UI.ComboBoxOD comboDelay;
		private ComboBox comboPayPlansVersion;
		private ComboBox comboProcFeeUpdatePrompt;
		private TextBox textDiscountPercentage;
		private TextBox textMedicationsIndicateNone;
		private TextBox textAllergiesIndicateNone;
		private TextBox textMedDefaultStopDays;
		private TextBox textClaimSnapshotRunTime;
		private TextBox textICD9DefaultForNewProcs;
		private TextBox textProblemsIndicateNone;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butProblemsIndicateNone;
		private UI.Button butDiagnosisCode;
		private UI.Button butMedicationsIndicateNone;
		private UI.Button butAllergiesIndicateNone;
		private UI.GroupBoxOD groupBox1;
		private UI.GroupBoxOD groupPayPlans;
		private UI.GroupBoxOD groupTreatPlanSort;
		private TabControl tabControlMain;
		private ODtextBox textTreatNote;
		private ToolTip toolTip1;
		private ValidNum textStatementsCalcDueDate;
		private ValidNum textPayPlansBillInAdvanceDays;
		private ValidNum textBillingElectBatchMax;
		private Label labelDiscountPercentage;
		private Label label19;
		private UI.GroupBoxOD groupBox6;
		private TextBox textInsImplant;
		private Label labelInsImplant;
		private Label label52;
		private TextBox textInsDentures;
		private Label labelInsDentures;
		private TextBox textInsPerioMaint;
		private Label labelInsPerioMaint;
		private TextBox textInsDebridement;
		private Label labelInsDebridement;
		private TextBox textInsSealant;
		private Label labelInsSealant;
		private TextBox textInsFlouride;
		private Label labelInsFlouride;
		private TextBox textInsCrown;
		private Label labelInsCrown;
		private TextBox textInsSRP;
		private Label labelInsSRP;
		private TextBox textInsCancerScreen;
		private Label labelInsCancerScreen;
		private TextBox textInsProphy;
		private Label labelInsProphy;
		private TextBox textInsExam;
		private Label labelInsPano;
		private TextBox textInsBW;
		private Label labelInsExam;
		private TextBox textInsPano;
		private Label labelInsBW;
		private CheckBox checkFrequency;
		private CheckBox checkProcNoteConcurrencyMerge;
		private CheckBox checkSolidBlockouts;
		private CheckBox checkApptExclamation;
		private CheckBox checkApptBubbleDelay;
		private Label label23;
		private Label label25;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.Button butApptLineColor;
		private CheckBox checkAppointmentBubblesDisabled;
		private CheckBox checkApptRefreshEveryMinute;
		private CheckBox checkEraOneClaimPerPage;
		private CheckBox checkIsAlertRadiologyProcsEnabled;
		private CheckBox checkShowAllocateUnearnedPaymentPrompt;
		private ComboBox comboToothNomenclature;
		private Label labelToothNomenclature;
		private CheckBox checkShowPlannedApptPrompt;
		private UI.GroupBoxOD groupCommLogs;
		private CheckBox checkCommLogAutoSave;
		private CheckBox checkAllowFutureTrans;
		private CheckBox checkShowAutoDeposit;
		private ValidNum textClaimsReceivedDays;
		private CheckBox checkClockEventAllowBreak;
		private UI.GroupBoxOD groupRecurringCharges;
		private Label labelRecurringChargesAutomatedTime;
		private ValidTime textRecurringChargesTime;
		private CheckBox checkRecurringChargesAutomated;
		private CheckBox checkRecurringChargesUseTransDate;
		private CheckBox checkRecurChargPriProv;
		private CheckBox checkIncludeEraWOPercCoPay;
		private CheckBox checkStatementsAlphabetically;
		private UI.GroupBoxOD groupInsHist;
		private TextBox textInsHistProphy;
		private Label labelInsHistProphy;
		private TextBox textInsHistPerioLR;
		private Label labelInsHistPerioLR;
		private TextBox textInsHistPerioLL;
		private Label labelInsHistPerioLL;
		private TextBox textInsHistPerioUL;
		private Label labelInsHistPerioUL;
		private TextBox textInsHistPerioUR;
		private Label labelInsHistPerioUR;
		private TextBox textInsHistFMX;
		private Label labelInsHistFMX;
		private TextBox textInsHistPerioMaint;
		private Label labelInsHistPerioMaint;
		private TextBox textInsHistExam;
		private Label labelInsHistDebridement;
		private TextBox textInsHistBW;
		private Label labelInsHistExam;
		private TextBox textInsHistDebridement;
		private Label labelInsHistBW;
		private UI.GroupBoxOD groupRepeatingCharges;
		private Label labelRepeatingChargesAutomatedTime;
		private ValidTime textRepeatingChargesAutomatedTime;
		private CheckBox checkRepeatingChargesRunAging;
		private CheckBox checkRepeatingChargesAutomated;
		private Label label56;
		private UI.ComboBoxOD comboRecurringChargePayType;
		private CheckBox checkEraAllowTotalPayment;
		private CheckBox checkRecurPatBal0;
		private Label label54;
		private CheckBox checkApptsAllowOverlap;
		private CheckBox checkPreventChangesToComplAppts;
		private ValidNum textApptAutoRefreshRange;
		private Label labelApptAutoRefreshRange;
		private CheckBox checkUnscheduledListNoRecalls;
		private CheckBox checkReplaceBlockouts;
		private Label labelApptSchedEnforceSpecialty;
		private ComboBox comboApptSchedEnforceSpecialty;
		private ValidNum textApptWithoutProcsDefaultLength;
		private Label labelApptWithoutProcsDefaultLength;
		private CheckBox checkApptAllowEmptyComplete;
		private CheckBox checkApptAllowFutureComplete;
		private UI.ComboBoxOD comboTimeArrived;
		private CheckBox checkApptsRequireProcs;
		private Label label3;
		private CheckBox checkApptModuleProductionUsesOps;
		private UI.ComboBoxOD comboTimeSeated;
		private CheckBox checkUseOpHygProv;
		private Label label5;
		private CheckBox checkApptModuleAdjInProd;
		private UI.ComboBoxOD comboTimeDismissed;
		private CheckBox checkApptTimeReset;
		private Label label6;
		private UI.GroupBoxOD groupBox2;
		private Label label37;
		private ComboBox comboBrokenApptProc;
		private CheckBox checkBrokenApptCommLog;
		private CheckBox checkBrokenApptAdjustment;
		private UI.ComboBoxOD comboBrokenApptAdjType;
		private Label label7;
		private TextBox textWaitRoomWarn;
		private CheckBox checkAppointmentTimeIsLocked;
		private Label label22;
		private ComboBox comboSearchBehavior;
		private TextBox textApptBubNoteLength;
		private Label label13;
		private Label label21;
		private CheckBox checkWaitingRoomFilterByView;
		private Label labelCobRule;
		private CheckBox checkInsPlanExclusionsUseUCR;
		private CheckBox checkInsPlanExclusionsMarkDoNotBill;
		private CheckBox checkFixedBenefitBlankLikeZero;
		private CheckBox checkAllowPatsAtHQ;
		private CheckBox checkAutoFillPatEmail;
		private CheckBox checkPreferredReferrals;
		private CheckBox checkTextMsgOkStatusTreatAsNo;
		private CheckBox checkPatInitBillingTypeFromPriInsPlan;
		private CheckBox checkFamPhiAccess;
		private CheckBox checkClaimTrackingRequireError;
		private CheckBox checkPPOpercentage;
		private CheckBox checkInsurancePlansShared;
		private CheckBox checkClaimUseOverrideProcDescript;
		private CheckBox checkInsDefaultAssignmentOfBenefits;
		private CheckBox checkSelectProv;
		private ComboBox comboCobRule;
		private CheckBox checkGoogleAddress;
		private CheckBox checkInsPPOsecWriteoffs;
		private CheckBox checkInsDefaultShowUCRonClaims;
		private CheckBox checkCoPayFeeScheduleBlankLikeZero;
		private CheckBox checkAllowPrepayProvider;
		private Label label42;
		private CheckBox checkAllowEmailCCReceipt;
		private Label label38;
		private CheckBox checkAllowFutureDebits;
		private CheckBox checkStoreCCTokens;
		private Label label26;
		private Label label33;
		private Label label12;
		private Label label4;
		private CheckBox checkPaymentsPromptForPayType;
		private Label label28;
		private UI.GroupBoxOD groupBox7;
		private CheckBox checkCanadianPpoLabEst;
		private CheckBox checkInsEstRecalcReceived;
		private CheckBox checkPromptForSecondaryClaim;
		private CheckBox checkInsPayNoWriteoffMoreThanProc;
		private CheckBox checkClaimTrackingExcludeNone;
		private Label label55;
		private ComboBox comboZeroDollarProcClaimBehavior;
		private Label labelClaimCredit;
		private ComboBox comboClaimCredit;
		private CheckBox checkAllowFuturePayments;
		private UI.GroupBoxOD groupBoxClaimIdPrefix;
		private UI.Button butReplacements;
		private TextBox textClaimIdentifier;
		private CheckBox checkAllowProcAdjFromClaim;
		private CheckBox checkProviderIncomeShows;
		private CheckBox checkClaimFormTreatDentSaysSigOnFile;
		private CheckBox checkClaimMedTypeIsInstWhenInsPlanIsMedical;
		private CheckBox checkEclaimsMedicalProvTreatmentAsOrdering;
		private CheckBox checkEclaimsSeparateTreatProv;
		private Label label20;
		private TextBox textClaimAttachPath;
		private CheckBox checkClaimsValidateACN;
		private TextBox textInsWriteoffDescript;
		private Label label17;
		private UI.ComboBoxOD comboPayPlanAdj;
		private ComboBox comboPaymentClinicSetting;
		private TextBox textTaxPercent;
		private UI.ComboBoxOD comboSalesTaxAdjType;
		private UI.ComboBoxOD comboBillingChargeAdjType;
		private UI.ComboBoxOD comboFinanceChargeAdjType;
		private UI.GroupBoxOD groupBox4;
		private UI.ListBoxOD listboxBadDebtAdjs;
		private Label label29;
		private UI.Button butBadDebt;
		private UI.ComboBoxOD comboUnallocatedSplits;
		private UI.GroupBoxOD groupBoxSuperFamily;
		private UI.GroupBoxOD groupBoxClaimSnapshot;
		private TextBox textApptFontSize;
		private Label label58;
		private ValidNum textApptProvbarWidth;
		private UI.GroupBoxOD groupBox8;
		private CheckBox checkAgingProcLifo;
		private Label label59;
		private ValidTime textDynamicPayPlan;
		private UI.GroupBoxOD groupBox10;
		private UI.GroupBoxOD groupBox9;
		private CheckBox checkPatientSSNMasked;
		private CheckBox checkPatientDOBMasked;
		private CheckBox checkBrokenApptRequiredOnMove;
		private CheckBox checkPromptSaveTP;
		private CheckBox checkNotesProviderSigOnly;
		private CheckBox checkPriClaimAllowSetToHoldUntilPriReceived;
		private CheckBox checkShowClaimPayTracking;
		private CheckBox checkShowClaimPatResp;
		private UI.GroupBoxOD groupBoxCOB;
		private UI.ComboBoxOD comboCobSendPaidByInsAt;
		private Label labelCobSendPaidByOtherInsAt;
		private Label labelDepositSoftware;
		private UI.ComboBoxOD comboDepositSoftware;
		private CheckBox checkSameForFamily;
		private UI.GroupBoxOD groupBoxOD1;
		private CheckBox checkIncTxfrTreatNegProdAsIncome;
		private Label label15;
		private TextBox textAutoImportFolder;
		private Label label60;
		private CheckBox checkAutomateSalesTax;
		private ComboBoxOD comboSalesTaxDefaultProvider;
		private GroupBoxOD groupBoxOD3;
		private Label label61;
		private TextBox textDefaultImageImportFolder;
		private CheckBox checkRecurringChargesInactivateDeclinedCards;
		private CheckBox checkPDFLaunchWindow;
		private ComboBoxOD comboLateChargeAdjType;
		private Label labelLateChargeAdjType;
		private GroupBoxOD groupBoxOD4;
		private Label label63;
		private TextBox textDiscountPACodes;
		private Label labelDiscountPAFreq;
		private TextBox textDiscountXrayCodes;
		private Label labelDiscountXrayFreq;
		private TextBox textDiscountPerioCodes;
		private Label labelDiscountPerioFreq;
		private TextBox textDiscountLimitedCodes;
		private Label labelDiscountLimitedFreq;
		private TextBox textDiscountFluorideCodes;
		private Label labelDiscountProphyFreq;
		private TextBox textDiscountExamCodes;
		private Label labelDiscountFluorideFreq;
		private TextBox textDiscountProphyCodes;
		private Label labelDiscountExamFreq;
		private ComboBoxOD comboEraAutomation;
		private Label labelEraAutomation;
		private CheckBox checkRecurringChargesShowInactive;
		private UI.Button butSyncPhNums;
		private CheckBox checkUsePhoneNumTable;
		private ComboBoxOD comboRefundAdjustmentType;
		private Label labelRefundAdjustmentType;
		private Label label62;
		private Label label39;
		private ComboBoxOD comboDppUnearnedType;
	}
}