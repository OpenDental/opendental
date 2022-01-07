using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	partial class FormOpenDental {
		#region Fields
		///<summary>Changes based on if OD HQ.</summary>
		private MenuItemOD _menuItemAccount;
		private MenuItemOD _menuItemAlerts;
		private MenuItemOD _menuItemCareCreditTransactions;
		private MenuItemOD _menuItemClinicsMain;
		private MenuItemOD _menuItemClinics;
		private MenuItemOD _menuItemCounties;
		///<summary>Not available if in Unix.</summary>
		private MenuItemOD _menuItemCreateAtoZ;
		private MenuItemOD _menuItemCustManagement;
		private MenuItemOD _menuItemCustomReports;
		///<summary>Only available in OD HQ.</summary>
		private MenuItemOD _menuItemDefaultCCProcs;
		private MenuItemOD _menuItemDentalSchoolClass;
		private MenuItemOD _menuItemDentalSchoolCourses;
		private MenuItemOD _menuItemDentalSchools;
		///<summary> Only available in OD HQ when Introspection.IsTestingMode is true. </summary>
		private MenuItemOD _menuItemEditTestModeOverrides;
		///<summary>Only show enterprise setup if it is enabled.</summary>
		private MenuItemOD _menuItemEnterprise;
		private MenuItemOD _menuItemEvaluations;
		private MenuItemOD _menuItemFeeSchedGroups;
		///<summary>Only available if Billing/Finance Charges are enabled on the Show Features window.</summary>
		private MenuItemOD _menuItemFinanceCharges;
		///<summary>Only available in OD HQ.</summary>
		private MenuItemOD _menuItemJobManager;
		///<summary>Only available if Late Charges are enabled on the Show Features window.</summary>
		private MenuItemOD _menuItemLateCharges;
		///<summary>Not available if in Unix.</summary>
		private MenuItemOD _menuItemLocalHelpWindows;
		private MenuItemOD _menuItemNewCropBilling;
		private MenuItemOD _menuItemNoAlerts;
		private MenuItemOD _menuItemPatDashboards;
		private MenuItemOD _menuItemPatPortalTransactions;
		private MenuItemOD _menuItemPayloadMonitor;
		private MenuItemOD _menuItemPendingOnlinePayments;
		///<summary>Procedure Lock Tool Pref.</summary>
		private MenuItemOD _menuItemProcLockTool;
		private MenuItemOD _menuItemPublicHealthScreening;
		private MenuItemOD _menuItemQueryFavorites;
		private MenuItemOD _menuItemQueryMonitor;
		private MenuItemOD _menuItemReports;
		///<summary>Only show enterprise setup if it is enabled.</summary>
		private MenuItemOD _menuItemReactivation;
		private MenuItemOD _menuItemRepeatingCharges;
		///<summary>Not available if isWeb.</summary>
		private MenuItemOD _menuItemReplication;
		private MenuItemOD _menuItemRequirementsNeeded;
		///<summary>Only available in OD HQ.</summary>
		private MenuItemOD _menuItemResellers;
		///<summary>Not available if isWeb.</summary>
		private MenuItemOD _menuItemServiceManager;
		private MenuItemOD _menuItemSites;
		private MenuItemOD _menuItemStandard;
		private MenuItemOD _menuItemStandardFiltered;
		private MenuItemOD _menuItemStudentRequirements;
		private MenuItemOD _menuItemActivityLog;
		///<summary>Not available if isWeb.</summary>
		private MenuItemOD _menuItemPrinter;
		///<summary>Available if OD is not set to English Translation.</summary>
		private MenuItemOD _menuItemTranslation;
		private MenuItemOD _menuItemUnfinalizedPay;
		private MenuItemOD _menuItemUserQuery;
		///<summary>Only available in OD HQ.</summary>
		private MenuItemOD _menuItemWebChatTools;
		private MenuItemOD _menuItemXChargeReconcile;
		#endregion Fields

		#region MainMenu
		private void LayoutMenu() {
			menuMain.BeginUpdate();
			//Log Off--------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Log &Off",menuItemLogOff_Click));
			//File-----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemFile=new MenuItemOD("&File");
			menuMain.Add(menuItemFile);
			LayoutMenuFile(menuItemFile);
			//Setup----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemSetup=new MenuItemOD("&Setup");
			menuMain.Add(menuItemSetup);
			LayoutMenuSetup(menuItemSetup);
			//Lists----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemLists=new MenuItemOD("&Lists");
			menuMain.Add(menuItemLists);
			LayoutMenuLists(menuItemLists);
			//Reports--------------------------------------------------------------------------------------------------------
			_menuItemReports=new MenuItemOD("&Reports");
			menuMain.Add(_menuItemReports);
			LayoutMenuReports(_menuItemReports);
			//Custom Reports-------------------------------------------------------------------------------------------------
			_menuItemCustomReports=new MenuItemOD("Custom Reports");
			menuMain.Add(_menuItemCustomReports);
			//Tools----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemTools=new MenuItemOD("&Tools");
			menuMain.Add(menuItemTools);
			LayoutMenuTools(menuItemTools);
			//Clinics--------------------------------------------------------------------------------------------------------
			_menuItemClinicsMain=new MenuItemOD("&Clinics");
			menuMain.Add(_menuItemClinicsMain);
			//eServices------------------------------------------------------------------------------------------------------
			menuMain.Add("eServices",menuItemEServices_Click);
			//Alerts---------------------------------------------------------------------------------------------------------
			_menuItemAlerts=new MenuItemOD("Alerts (0)",menuItemAlerts_Click);
			menuMain.Add(_menuItemAlerts);
			//_menuItemNoAlerts=new MenuItemOD("No alerts");
			//_menuItemAlerts.Add(_menuItemNoAlerts);
			//Help-----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemHelp=new MenuItemOD("&Help");
			menuMain.Add(menuItemHelp);
			LayoutMenuHelp(menuItemHelp);
			menuMain.EndUpdate();
		}
		#endregion MainMenu

		#region File
		///<summary>File Main</summary>
		private void LayoutMenuFile(MenuItemOD menuItemFile) {
			menuItemFile.Add("User Password",menuItemPassword_Click);
			menuItemFile.Add("User Email Address",menuItemUserEmailAddress_Click);
			menuItemFile.Add("User Settings",menuItemUserSettings_Click);
			menuItemFile.AddSeparator();
			_menuItemPrinter=new MenuItemOD("&Printer",menuItemPrinter_Click);
			menuItemFile.Add(_menuItemPrinter);
			menuItemFile.Add("Graphics",menuItemGraphics_Click);
			menuItemFile.AddSeparator();
			menuItemFile.Add("&Choose Database",menuItemConfig_Click);
			menuItemFile.AddSeparator();
			menuItemFile.Add("E&xit",menuItemExit_Click);
		}
		#endregion File

		#region Setup
		///<summary>Setup Main</summary>
		private void LayoutMenuSetup(MenuItemOD menuItemSetup) {
			//Appointments----------------------------------------------------------------------------------------------------
			MenuItemOD menuItemAppts=new MenuItemOD("Appointments");
			menuItemSetup.Add(menuItemAppts);
			LayoutSubMenuAppts(menuItemAppts);
			//Family/Insurance------------------------------------------------------------------------------------------------
			MenuItemOD menuItemFamIns=new MenuItemOD("Family / Insurance");
			menuItemSetup.Add(menuItemFamIns);
			LayoutSubMenuFamIns(menuItemFamIns);
			//Account---------------------------------------------------------------------------------------------------------
			_menuItemAccount=new MenuItemOD("Account");
			menuItemSetup.Add(_menuItemAccount);
			LayoutSubMenuAccount(_menuItemAccount);
			//Treatment Plan--------------------------------------------------------------------------------------------------
			MenuItemOD menuItemTreatPlan=new MenuItemOD("Treat' Plan",menuItemPreferencesTreatPlan_Click);
			menuItemSetup.Add(menuItemTreatPlan);
			//Chart-----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemChart=new MenuItemOD("Chart");
			menuItemSetup.Add(menuItemChart);
			LayoutSubMenuChart(menuItemChart);
			//Imaging---------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemImaging=new MenuItemOD("Imaging");
			menuItemSetup.Add(menuItemImaging);
			LayoutSubMenuImaging(menuItemImaging);
			//Manage----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemManage=new MenuItemOD("Manage");
			menuItemSetup.Add(menuItemManage);
			LayoutSubMenuManage(menuItemManage);
			menuItemSetup.AddSeparator();
			//Advanced Setup--------------------------------------------------------------------------------------------------
			MenuItemOD menuItemAdvSetup=new MenuItemOD("Advanced Setup");
			menuItemSetup.Add(menuItemAdvSetup);
			LayoutSubMenuAdvSetup(menuItemAdvSetup);
			//Menus below have no submenus (name as shown)--------------------------------------------------------------------
			menuItemSetup.Add("Alert Categories",menuItemAlertCategories_Click);
			menuItemSetup.Add("Auto Codes",menuItemAutoCodes_Click);
			menuItemSetup.Add("Automation",menuItemAutomation_Click);
			menuItemSetup.Add("Auto Notes",menuItemAutoNotes_Click);
			if(ODBuild.IsWeb()) {
				menuItemSetup.Add("Cloud Management",menuItemCloudManagement_Click);
			}
			menuItemSetup.Add("Data Paths",menuItemDataPath_Click);
			menuItemSetup.Add("Definitions",menuItemDefinitions_Click);
			_menuItemDentalSchools=new MenuItemOD("Dental Schools",menuItemDentalSchools_Click);
			menuItemSetup.Add(_menuItemDentalSchools);
			menuItemSetup.Add("Display Fields",menuItemDisplayFields_Click);
			_menuItemEnterprise=new MenuItemOD("Enterprise",menuItemEnterprise_Click);
			menuItemSetup.Add(_menuItemEnterprise);
			menuItemSetup.Add("Fee Schedules",menuItemFeeScheds_Click);
			_menuItemFeeSchedGroups=new MenuItemOD("Fee Schedule Groups",menuFeeSchedGroups_Click);
			menuItemSetup.Add(_menuItemFeeSchedGroups);
			menuItemSetup.Add("Laboratories",menuItemLaboratories_Click);
			menuItemSetup.Add("Miscellaneous",menuItemMisc_Click);
			menuItemSetup.Add("Module Preferences",menuItemModules_Click);
			menuItemSetup.Add("Ortho",menuItemOrtho_Click);
			menuItemSetup.Add("Practice",menuItemPractice_Click);
			menuItemSetup.Add("Program Links",menuItemLinks_Click);
			menuItemSetup.Add("Quick Paste Notes",menuItemQuickPasteNotes_Click);
			menuItemSetup.Add("Reports",menuItemReports_Click);
			menuItemSetup.Add("Required Fields",menuItemRequiredFields_Click);
			_menuItemRequirementsNeeded=new MenuItemOD("Requirements Needed",menuItemRequirementsNeeded_Click);
			menuItemSetup.Add(_menuItemRequirementsNeeded);
			menuItemSetup.Add("Schedules",menuItemSched_Click);
			menuItemSetup.Add("Security",menuItemSecurity_Click);
			menuItemSetup.Add("Security Add User",menuItemSecurityAddUser_Click);
			menuItemSetup.Add("Sheets",menuItemSheets_Click);
			menuItemSetup.Add("Spell Check",menuItemSpellCheck_Click);
			menuItemSetup.Add("Tasks",menuItemTask_Click);
			menuItemSetup.Add("Web Forms",menuItemWebForm_Click);
			menuItemSetup.AddSeparator();
			//Obsolete--------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemObsolete=new MenuItemOD("Obsolete");
			menuItemSetup.Add(menuItemObsolete);
			LayoutSubMenuObsolete(menuItemObsolete);
		}

		///<summary>Setup: Appointments</summary>
		private void LayoutSubMenuAppts(MenuItemOD menuItemAppts) {
			menuItemAppts.Add("Appts Preferences",menuItemPreferencesAppts_Click);
			menuItemAppts.AddSeparator();
			menuItemAppts.Add("Appointment Field Defs",menuItemApptFieldDefs_Click);
			menuItemAppts.Add("Appointment Rules",menuItemApptRules_Click);
			menuItemAppts.Add("Appointment Types",menuItemApptTypes_Click);
			menuItemAppts.Add("Appointment Views",menuItemApptViews_Click);
			menuItemAppts.Add("ASAP List",menuItemAsapList_Click);
			menuItemAppts.Add("Confirmations",menuItemConfirmations_Click);
			menuItemAppts.Add("Insurance Verification",menuItemInsVerify_Click);
			menuItemAppts.Add("Operatories",menuItemOperatories_Click);
			menuItemAppts.Add("Recall",menuItemRecall_Click);
			menuItemAppts.Add("Recall Types",menuItemRecallTypes_Click);
			_menuItemReactivation=new MenuItemOD("Reactivation",menuItemReactivation_Click);
			menuItemAppts.Add(_menuItemReactivation);
		}

		///<summary>Setup: Family/Insurance</summary>
		private void LayoutSubMenuFamIns(MenuItemOD menuItemFamIns) {
			menuItemFamIns.Add("Family Preferences",menuItemPreferencesFamily_Click);
			menuItemFamIns.AddSeparator();
			menuItemFamIns.Add("Claim Forms",menuItemClaimForms_Click);
			menuItemFamIns.Add("Clearinghouses",menuItemClearinghouses_Click);
			menuItemFamIns.Add("Insurance Blue Book",menuItemInsBlueBook_Click);
			menuItemFamIns.Add("Insurance Categories",menuItemInsCats_Click);
			menuItemFamIns.Add("Insurance Filing Codes",menuItemInsFilingCodes_Click);
			menuItemFamIns.Add("Patient Field Defs",menuItemPatFieldDefs_Click);
			menuItemFamIns.Add("Payer IDs",menuItemPayerIDs_Click);
		}

		///<summary>Setup: Account</summary>
		private void LayoutSubMenuAccount(MenuItemOD menuItemAccount) {
			menuItemAccount.Add("Account Preferences",menuItemPreferencesAccount_Click);
			menuItemAccount.AddSeparator();
			menuItemAccount.Add("Allocations",menuItemAllocations_Click);
			_menuItemDefaultCCProcs=new MenuItemOD("Default CC Procedures",menuItemDefaultCCProcs_Click);
			menuItemAccount.Add(_menuItemDefaultCCProcs);
		}

		///<summary>Setup: Chart</summary>
		private void LayoutSubMenuChart(MenuItemOD menuItemChart) {
			menuItemChart.Add("Chart Preferences",menuItemPreferencesChart_Click);
			menuItemChart.AddSeparator();
			menuItemChart.Add("EHR",menuItemEHR_Click);
			menuItemChart.Add("Procedure Buttons",menuItemProcedureButtons_Click);
		}

		///<summary>Setup: Imaging</summary>
		private void LayoutSubMenuImaging(MenuItemOD menuItemImaging) {
			menuItemImaging.Add("Imaging Preferences",menuItemPreferencesImaging_Click);
			menuItemImaging.AddSeparator();
			MenuItemOD menuItemImagingDevices=new MenuItemOD("Devices",menuItemImagingDevices_Click);
			menuItemImaging.Add(menuItemImagingDevices);
			menuItemImaging.Add("Mounts",menuItemMounts_Click);
			menuItemImaging.Add("Scanning",menuItemScanning_Click);
		}

		///<summary>Setup: Manage</summary>
		private void LayoutSubMenuManage(MenuItemOD menuItemManage) {
			menuItemManage.Add("Manage Preferences",menuItemPreferencesManage_Click);
			menuItemManage.AddSeparator();
			menuItemManage.Add("E-mail",menuItemEmail_Click);
			menuItemManage.Add("Messaging",menuItemMessaging_Click);
			menuItemManage.Add("Messaging Buttons",menuItemMessagingButs_Click);
			menuItemManage.Add("Time Cards",menuItemTimeCards_Click);
		}

		///<summary>Setup: Advanced Setup</summary>
		private void LayoutSubMenuAdvSetup(MenuItemOD menuItemAdvSetup) {
			menuItemAdvSetup.Add("Computers",menuItemComputers_Click);
			menuItemAdvSetup.Add("FHIR",menuItemFHIR_Click);
			menuItemAdvSetup.Add("HIE",menuItemHIE_Click);
			menuItemAdvSetup.Add("HL7",menuItemHL7_Click);
			_menuItemReplication=new MenuItemOD("Replication",menuItemReplication_Click);
			menuItemAdvSetup.Add(_menuItemReplication);
			menuItemAdvSetup.Add("Show Features",menuItemEasy_Click);
			menuItemAdvSetup.Add("Scheduled Processes",MenuItemScheduledProcesses_Click);
		}

		///<summary>Setup: Obsolete</summary>
		private void LayoutSubMenuObsolete(MenuItemOD menuItemObsolete) {
			menuItemObsolete.Add("Letters",menuItemLetters_Click);
			menuItemObsolete.Add("Questionnaire",menuItemQuestions_Click);
		}
		#endregion Setup

		#region Lists
		///<summary>Lists Main</summary>
		private void LayoutMenuLists(MenuItemOD menuItemLists) {
			MenuItemOD menuItemProcedureCodes=new MenuItemOD("&Procedure Codes",menuItemProcCodes_Click);
			menuItemProcedureCodes.ShortcutKeys=Keys.Control|Keys.Shift|Keys.F;
			menuItemLists.Add(menuItemProcedureCodes);
			menuItemLists.AddSeparator();
			menuItemLists.Add("Allergies",menuItemAllergies_Click);
			_menuItemClinics=new MenuItemOD("Clinics",menuItemClinics_Click);
			menuItemLists.Add(_menuItemClinics);
			MenuItemOD menuItemContacts=new MenuItemOD("&Contacts",menuItemContacts_Click);
			menuItemContacts.ShortcutKeys=Keys.Control|Keys.Shift|Keys.C;
			menuItemLists.Add(menuItemContacts);
			_menuItemCounties=new MenuItemOD("Counties",menuItemCounties_Click);
			menuItemLists.Add(_menuItemCounties);
			_menuItemDentalSchoolClass=new MenuItemOD("Dental School Classes",menuItemSchoolClass_Click);
			menuItemLists.Add(_menuItemDentalSchoolClass);
			_menuItemDentalSchoolCourses=new MenuItemOD("Dental School Courses",menuItemSchoolCourses_Click);
			menuItemLists.Add(_menuItemDentalSchoolCourses);
			menuItemLists.Add("Discount Plans",menuItemDiscountPlans_Click);
			menuItemLists.Add("&Employees",menuItemEmployees_Click);
			menuItemLists.Add("Employers",menuItemEmployers_Click);
			menuItemLists.Add("Insurance Carriers",menuItemCarriers_Click);
			menuItemLists.Add("&Insurance Plans",menuItemInsPlans_Click);
			menuItemLists.Add("Lab Cases",menuItemLabCases_Click);
			menuItemLists.Add("&Medications",menuItemMedications_Click);
			menuItemLists.Add("Pharmacies",menuItemPharmacies_Click);
			menuItemLists.Add("Pre&scriptions",menuItemPrescriptions_Click);
			menuItemLists.Add("Problems",menuItemProblems_Click);
			menuItemLists.Add("Providers",menuItemProviders_Click);
			menuItemLists.Add("&Referrals",menuItemReferrals_Click);
			_menuItemSites=new MenuItemOD("Sites",menuItemSites_Click);
			menuItemLists.Add(_menuItemSites);
			menuItemLists.Add("State Abbreviations",menuItemStateAbbrs_Click);
			menuItemLists.Add("&Zip Codes",menuItemZipCodes_Click);
		}
		#endregion Lists

		#region Reports
		///<summary>Reports Main</summary>
		private void LayoutMenuReports(MenuItemOD menuItemReports) {
			_menuItemStandard=new MenuItemOD("&Standard",menuItemReportsStandard_Click);
			menuItemReports.Add(_menuItemStandard);
			_menuItemStandardFiltered=new MenuItemOD("Standard Favorites",menuItemReportsFilteredClick_Click);
			menuItemReports.Add(_menuItemStandardFiltered);
			menuItemReports.Add("&Graphic",menuItemReportsGraphic_Click);
			_menuItemUserQuery=new MenuItemOD("&User Query",menuItemReportsUserQuery_Click);
			menuItemReports.Add(_menuItemUserQuery);
			_menuItemQueryFavorites=new MenuItemOD("User Query Favorites",menuItemReportsQueryFavorites_Click);
			_menuItemReports.Add(_menuItemQueryFavorites);
			_menuItemActivityLog=new MenuItemOD("eService Activity Log",menuItemReportsActivityLog_Click);
			menuItemReports.Add(_menuItemActivityLog);
			menuItemReports.AddSeparator();
			_menuItemUnfinalizedPay=new MenuItemOD("Unfinalized Payments",menuItemReportsUnfinalizedPay_Click);
			menuItemReports.Add(_menuItemUnfinalizedPay);
		}
		#endregion Reports

		#region Tools
		///<summary>Tools Main</summary>
		private void LayoutMenuTools(MenuItemOD menuItemTools) {
			//Job Mananger----------------------------------------------------------------------------------------------------
			_menuItemJobManager=new MenuItemOD("Job Manager",menuItemJobManager_Click);
			menuItemTools.Add(_menuItemJobManager);
			//Web Chat Tools--------------------------------------------------------------------------------------------------
			_menuItemWebChatTools=new MenuItemOD("Web Chat Tools");
			menuItemTools.Add(_menuItemWebChatTools);
			LayoutSubMenuWebChatTools(_menuItemWebChatTools);
			//Snipping Tool-----------------------------------------------------------------------------------------------
			menuItemTools.Add("&Screen Snipping Tool",menuItemScreenSnip_Click);
			//Print Screen Tool-----------------------------------------------------------------------------------------------
			menuItemTools.Add("&Print Screen Tool",menuItemPrintScreen_Click);
			//Misc Tools------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemMiscTools=new MenuItemOD("Misc Tools");
			menuItemTools.Add(menuItemMiscTools);
			LayoutSubMenuMiscTools(menuItemMiscTools);
			menuItemTools.AddSeparator();
			//Menus below have no submenus (name as shown)--------------------------------------------------------------------
			menuItemTools.Add("&Aging",menuItemAging_Click);
			menuItemTools.Add("Audit Trail",menuItemAuditTrail_Click);
			_menuItemFinanceCharges=new MenuItemOD("Billing/&Finance Charges",menuItemFinanceCharge_Click);
			menuItemTools.Add(_menuItemFinanceCharges);
			_menuItemCareCreditTransactions=new MenuItemOD("CareCredit Transactions",menuItemCareCreditTrans_Click);
			menuItemTools.Add(_menuItemCareCreditTransactions);
			menuItemTools.Add("CC Recurring Charges",menuItemCCRecurring_Click);
			menuItemTools.Add("Certifications",menuItemCertifications_Click);
			_menuItemCustManagement=new MenuItemOD("Customer Management",menuItemCustomerManage_Click);
			menuItemTools.Add(_menuItemCustManagement);
			menuItemTools.Add("Database Maintenance",menuItemDatabaseMaintenance_Click);
			//menuItemTools.Add("Dispensary",menuItemDispensary_Click);//FormDispensary is not fully functional and should not be an available option at this time
			_menuItemEvaluations=new MenuItemOD("Evaluations",menuItemEvaluations_Click);
			menuItemTools.Add(_menuItemEvaluations);
			menuItemTools.Add("Kiosk",menuItemTerminal_Click);
			menuItemTools.Add("Kiosk Manager",menuItemTerminalManager_Click);
			_menuItemTranslation=new MenuItemOD("Language Translation",menuItemTranslation_Click);
			menuItemTools.Add(_menuItemTranslation);
			menuItemTools.Add("Hosted Email",menuItemMassEmails_Click);
			_menuItemLateCharges=new MenuItemOD("Late Charges",menuItemLateCharges_Click);
			menuItemTools.Add(_menuItemLateCharges);
			menuItemTools.Add("Mobile Synch",menuItemMobileSetup_Click);
			_menuItemNewCropBilling=new MenuItemOD("NewCrop Billing",menuItemNewCropBilling_Click);
			menuItemTools.Add(_menuItemNewCropBilling);
			menuItemTools.Add("Ortho Auto Claims",menuItemOrthoAuto_Click);
			_menuItemPatDashboards=new MenuItemOD("Patient Dashboards");
			menuItemTools.Add(_menuItemPatDashboards);
			_menuItemPatPortalTransactions=new MenuItemOD("Patient Portal Transactions",menuItemXWebTrans_Click);
			menuItemTools.Add(_menuItemPatPortalTransactions);
			_menuItemPendingOnlinePayments=new MenuItemOD("Pending &Online Payments",menuItemPendingPayments_Click);
			menuItemTools.Add(_menuItemPendingOnlinePayments);
			_menuItemPublicHealthScreening=new MenuItemOD("Public Health Screening",menuItemScreening_Click);
			menuItemTools.Add(_menuItemPublicHealthScreening);
			_menuItemRepeatingCharges=new MenuItemOD("Repeating Charges",menuItemRepeatingCharges_Click);
			menuItemTools.Add(_menuItemRepeatingCharges);
			_menuItemResellers=new MenuItemOD("Resellers",menuItemResellers_Click);
			menuItemTools.Add(_menuItemResellers);
			menuItemTools.Add("Setup Wizard",menuItemSetupWizard_Click);
			_menuItemStudentRequirements=new MenuItemOD("Student Requirements",menuItemReqStudents_Click);
			menuItemTools.Add(_menuItemStudentRequirements);
			menuItemTools.Add("Web Forms",menuItemWebForms_Click);
			MenuItemOD menuItemWiki=new MenuItemOD("Wiki",menuItemWiki_Click);
			menuItemWiki.ShortcutKeys=Keys.Control|Keys.Shift|Keys.W;
			menuItemTools.Add(menuItemWiki);
			menuItemTools.Add("Zoom",menuItemZoom_Click);
		}

		///<summary>Tools: WebChatTools</summary>
		private void LayoutSubMenuWebChatTools(MenuItemOD menuItemWebChatTools) {
			menuItemWebChatTools.Add("Sessions",menuItemWebChatSessions_Click);
			menuItemWebChatTools.Add("Surveys",menuItemWebChatSurveys_Click);
		}

		///<summary>Tools: Misc Tools</summary>
		private void LayoutSubMenuMiscTools(MenuItemOD menuItemMiscTools) {
			menuItemMiscTools.Add("Auto-Close Payment Plans",menuItemAutoClosePayPlans_Click);
			menuItemMiscTools.Add("Clear Duplicate Blockouts",menuItemDuplicateBlockouts_Click);
			_menuItemCreateAtoZ=new MenuItemOD("Create A to Z Folders",menuItemCreateAtoZFolders_Click);
			menuItemMiscTools.Add(_menuItemCreateAtoZ);
			menuItemMiscTools.Add("Database Maintenance Pat",menuItemDatabaseMaintenancePat_Click);
			menuItemMiscTools.Add("Merge Discount Plans",menuItemMergeDPs_Click);
			menuItemMiscTools.Add("Merge Image Categories",menuItemMergeImageCat_Click);
			menuItemMiscTools.Add("Merge Medications",menuItemMergeMedications_Click);
			menuItemMiscTools.Add("Merge Patients",menuItemMergePatients_Click);
			menuItemMiscTools.Add("Merge Providers",menuItemMergeProviders_Click);
			menuItemMiscTools.Add("Merge Referrals",menuItemMergeReferrals_Click);
			menuItemMiscTools.Add("Move Subscribers",menuItemMoveSubscribers_Click);
			menuItemMiscTools.Add("Patient Status Setter",menuPatientStatusSetter_Click);
			_menuItemProcLockTool=new MenuItemOD("Procedure Lock Tool",menuItemProcLockTool_Click);
			menuItemMiscTools.Add(_menuItemProcLockTool);
			_menuItemServiceManager=new MenuItemOD("Service Manager",menuItemServiceManager_Click);
			menuItemMiscTools.Add(_menuItemServiceManager);
			menuItemMiscTools.Add("Shutdown All Workstations",menuItemShutdown_Click);
			menuItemMiscTools.Add("Telephone Numbers",menuTelephone_Click);
			menuItemMiscTools.Add("Test Latency",menuItemTestLatency_Click);
			_menuItemEditTestModeOverrides=new MenuItemOD("Testing Mode Overrides",menuItemEditTestModeOverrides_Click);
			menuItemMiscTools.Add(_menuItemEditTestModeOverrides);
			_menuItemXChargeReconcile=new MenuItemOD("X-Charge Reconcile",menuItemXChargeReconcile_Click);
			menuItemMiscTools.Add(_menuItemXChargeReconcile);
		}
		#endregion Tools

		#region Help
		///<summary>Help Main</summary>
		private void LayoutMenuHelp(MenuItemOD menuItemHelp) {
			menuItemHelp.Add("Online Support",menuItemRemote_Click);
			_menuItemLocalHelpWindows=new MenuItemOD("Local Help-Windows",menuItemHelpWindows_Click);
			menuItemHelp.Add(_menuItemLocalHelpWindows);
			menuItemHelp.Add("Online Help - Contents",menuItemHelpContents_Click);
			MenuItemOD menuItemOnlineHelpIndex=new MenuItemOD("Online Help - Index",menuItemHelpIndex_Click);
			menuItemOnlineHelpIndex.ShortcutKeys=Keys.Shift|Keys.F1;
			menuItemHelp.Add(menuItemOnlineHelpIndex);
			_menuItemPayloadMonitor=new MenuItemOD("Payload Monitor",MenuItemQueryMonitor_Click);
			menuItemHelp.Add(_menuItemPayloadMonitor);
			menuItemHelp.Add("Training Videos",menuItemWebinar_Click);
			_menuItemQueryMonitor=new MenuItemOD("Query Monitor",MenuItemQueryMonitor_Click);
			menuItemHelp.Add(_menuItemQueryMonitor);
			menuItemHelp.Add("Request Features",menuItemRequestFeatures_Click);
			menuItemHelp.Add("Support Status",MenuItemSupportStatus_Click);
			menuItemHelp.Add("&Update",menuItemUpdate_Click);
			menuItemHelp.AddSeparator();
			menuItemHelp.Add("About",menuItemAbout_Click);
		}
		#endregion Help
	}
}
