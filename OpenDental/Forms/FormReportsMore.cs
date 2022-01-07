using System;
using System.Diagnostics;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.Bridges;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using DataConnectionBase;
using CodeBase;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReportsMore:FormODBase {
		///<summary>After this form closes, this value is checked to see if any non-modal dialog boxes are needed.</summary>
		public ReportNonModalSelection RpNonModalSelection;
		///<summary>The Date currently selected on the Appointment Module.</summary>
		public DateTime DateSelected;
		private List<DisplayReport> _listProdInc;
		private List<DisplayReport> _listMonthly;
		private List<DisplayReport> _listDaily;
		private List<DisplayReport> _listList;
		private List<DisplayReport> _listPublicHealth;
		private List<DisplayReport> _listArizonaPrimary;
		private List<GroupPermission> _listReportPermissions;

		///<summary></summary>
		public FormReportsMore() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReportsMore_Load(object sender,EventArgs e) {
			Plugins.HookAddCode(this,"FormReportsMore.FormReportsMore_Load_beginning");
			butPW.Visible=Programs.IsEnabled(ProgramName.PracticeWebReports);
			//hiding feature for 13.3
			//butPatList.Visible=PrefC.GetBool(PrefName.ShowFeatureEhr);
			butPatExport.Visible=PrefC.GetBool(PrefName.ShowFeatureEhr);
			LayoutMenu();
			FillLists();
			if(ProgramProperties.IsAdvertisingDisabled(ProgramName.Podium)) {
				groupPatientReviews.Visible=false;
			}
			if(ProgramProperties.IsAdvertisingDisabled(ProgramName.PracticeByNumbers)) {
				groupBusiness.Visible=false;
			}
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		///<summary>Takes all non-hidden display reports and displays them in their various listboxes.  
		///Hides listboxes that have no display reports.</summary>
		private void FillLists() {
			_listProdInc=DisplayReports.GetForCategory(DisplayReportCategory.ProdInc,false);
			_listMonthly=DisplayReports.GetForCategory(DisplayReportCategory.Monthly,false);
			_listDaily=DisplayReports.GetForCategory(DisplayReportCategory.Daily,false);
			_listList=DisplayReports.GetForCategory(DisplayReportCategory.Lists,false);
			_listPublicHealth=DisplayReports.GetForCategory(DisplayReportCategory.PublicHealth,false);
			_listArizonaPrimary=DisplayReports.GetForCategory(DisplayReportCategory.ArizonaPrimaryCare,false);
			_listReportPermissions=GroupPermissions.GetPermsForReports().Where(x => Security.CurUser.IsInUserGroup(x.UserGroupNum)).ToList();
			//add the items to the list boxes and set the list box heights. (positions too?)
			listProdInc.Items.Clear();
			listDaily.Items.Clear();
			listMonthly.Items.Clear();
			listLists.Items.Clear();
			listPublicHealth.Items.Clear();
			listArizonaPrimaryCare.Items.Clear();
			//listUDSReports.Items.Clear();
			foreach(DisplayReport report in _listProdInc) {
				if(!_listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum)) {
					listProdInc.Items.Add(report.Description+" [Locked]");
				}
				else {
					listProdInc.Items.Add(report.Description);
				}
			}
			if(_listProdInc.Count==0) {
				listProdInc.Visible=false;
				labelProdInc.Visible=false;
			}
			else {
				listProdInc.Visible=true;
				labelProdInc.Visible=true;
				LayoutManager.MoveHeight(listProdInc,Math.Min((_listProdInc.Count+1) * LayoutManager.Scale(15),listProdInc.Height));
			}
			foreach(DisplayReport report in _listDaily) {
				if(!_listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum)) {
					listDaily.Items.Add(report.Description+" [Locked]");
				}
				else {
					listDaily.Items.Add(report.Description);
				}
			}
			if(_listDaily.Count==0) {
				listDaily.Visible=false;
				labelDaily.Visible=false;
			}
			else {
				listDaily.Visible=true;
				labelDaily.Visible=true;
				listDaily.Height=Math.Min((_listDaily.Count+1) * LayoutManager.Scale(15),listDaily.Height);
			}
			foreach(DisplayReport report in _listMonthly) {
				if(!_listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum)) {
					listMonthly.Items.Add(report.Description+" [Locked]");
				}
				else {
					listMonthly.Items.Add(report.Description);
				}
			}
			if(_listMonthly.Count==0) {
				listMonthly.Visible=false;
				labelMonthly.Visible=false;
			}
			else {
				listMonthly.Visible=true;
				labelMonthly.Visible=true;
				listMonthly.Height=Math.Min((_listMonthly.Count+1) * LayoutManager.Scale(15),listMonthly.Height);
			}
			foreach(DisplayReport report in _listList) {
				if(!_listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum)) {
					listLists.Items.Add(report.Description+" [Locked]");
				}
				else {
					listLists.Items.Add(report.Description);
				}
			}
			if(_listList.Count==0) {
				listLists.Visible=false;
				labelLists.Visible=false;
			}
			else {
				listLists.Visible=true;
				labelLists.Visible=true;
				listLists.Height=Math.Min((_listList.Count+1) * LayoutManager.Scale(15),listLists.Height);
			}
			foreach(DisplayReport report in _listPublicHealth) {
				if(!_listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum)) {
					listPublicHealth.Items.Add(report.Description+" [Locked]");
				}
				else {
					listPublicHealth.Items.Add(report.Description);
				}
			}
			if(_listPublicHealth.Count==0) {
				listPublicHealth.Visible=false;
				labelPublicHealth.Visible=false;
			}
			else {
				listPublicHealth.Visible=true;
				labelPublicHealth.Visible=true;
				LayoutManager.MoveHeight(listPublicHealth,Math.Min((_listPublicHealth.Count+1) * LayoutManager.Scale(15),listPublicHealth.Height));
			}
			//Arizona primary care list and label must only be visible when the Arizona primary
			//care option is checked in the miscellaneous options.
			foreach(DisplayReport report in _listArizonaPrimary) {
				if(!_listReportPermissions.Exists(x => x.FKey==report.DisplayReportNum)) {
					listArizonaPrimaryCare.Items.Add(report.Description+" [Locked]");
				}
				else {
					listArizonaPrimaryCare.Items.Add(report.Description);
				}
			}
			if(_listArizonaPrimary.Count==0 || !UsingArizonaPrimaryCare()) {
				listArizonaPrimaryCare.Visible=false;
				labelArizonaPrimaryCare.Visible=false;
			}
			else {
				listArizonaPrimaryCare.Visible=true;
				labelArizonaPrimaryCare.Visible=true;
				listArizonaPrimaryCare.Height=Math.Min((_listArizonaPrimary.Count+1) * LayoutManager.Scale(15),49);
			}
		}

		///<summary>Returns true if all of the required patient fields exist which are necessary to run the Arizona Primary Care reports.
		///Otherwise, false is returned.</summary>
		public static bool UsingArizonaPrimaryCare() {
			PatFieldDefs.RefreshCache();
			string[] patientFieldNames=new string[] {
				"SPID#",
				"Eligibility Status",
				"Household Gross Income",
				"Household % of Poverty",
			};
			int[] fieldCounts=new int[patientFieldNames.Length];
			foreach(PatFieldDef pfd in PatFieldDefs.GetDeepCopy(true)) {
				for(int i=0;i<patientFieldNames.Length;i++) {
					if(pfd.FieldName.ToLower()==patientFieldNames[i].ToLower()) {
						fieldCounts[i]++;
						break;
					}
				}
			}
			for(int i=0;i<fieldCounts.Length;i++) {
				//Each field must be defined exactly once. This verifies that each requied field
				//both exists and is not ambiguous with another field of the same name.
				if(fieldCounts[i]!=1) {
					return false;
				}
			}
			return true;
		}

		private void butUserQuery_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				MsgBox.Show(this,"Not allowed while using Oracle.");
				return;
			}
			if(Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
				using FormQuery FormQ=new FormQuery(null);
				FormQ.ShowDialog();
				SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,"");
			}
			else {
				using FormQueryFavorites FormQF = new FormQueryFavorites();
				FormQF.ShowDialog();
				if(FormQF.DialogResult == DialogResult.OK) {
					using FormQuery FormQ=new FormQuery(null,true);
					FormQ.textQuery.Text=FormQF.UserQueryCur.QueryText;
					FormQ.textTitle.Text=FormQF.UserQueryCur.FileName;
					SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,Lan.g(this,"User query form accessed."));
					FormQ.ShowDialog();
				}
			}
		}

		private void butPW_Click(object sender,EventArgs e) {
			try {
				if(!Programs.IsEnabledByHq(ProgramName.PracticeWebReports,out string err)) {
					MsgBox.Show(err);
					return;
				}
				ODFileUtils.ProcessStart("PWReports.exe");
			}
			catch {
				System.Windows.Forms.MessageBox.Show("PracticeWeb Reports module unavailable.");
			}
			SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Practice Web");
		}

		private void listProdInc_MouseDown(object sender,MouseEventArgs e) {
			int selected=listProdInc.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			if(!_listReportPermissions.Exists(x => x.FKey==_listProdInc[selected].DisplayReportNum)) {
				MsgBox.Show(this,"You do not have permission to run this report.");
				return;
			}
			if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				MsgBox.Show(this,"The current user needs to be a provider or have the 'All Providers' permission for this report");
				return;
			}
			OpenReportLocalHelper(_listProdInc[selected],_listReportPermissions,false);
		}

		private void listDaily_MouseDown(object sender,MouseEventArgs e) {
			int selected=listDaily.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listDaily[selected],_listReportPermissions);
		}

		private void listMonthly_MouseDown(object sender,MouseEventArgs e) {
			int selected=listMonthly.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listMonthly[selected],_listReportPermissions);
		}

		private void listLists_MouseDown(object sender,MouseEventArgs e) {
			int selected=listLists.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listList[selected],_listReportPermissions);
		}

		private void listPublicHealth_MouseDown(object sender,MouseEventArgs e) {
			int selected=listPublicHealth.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listPublicHealth[selected],_listReportPermissions);
		}

		private void listArizonaPrimaryCare_MouseDown(object sender,MouseEventArgs e) {
			int selected=this.listArizonaPrimaryCare.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listArizonaPrimary[selected],_listReportPermissions,false);
		}
		
		///<summary>Called from this form to do OpenReportHelper(...) logic and then close when needed.</summary>
		private void OpenReportLocalHelper(DisplayReport displayReport,List<GroupPermission> listReportPermissions,bool doValidatePerm=true)
		{
			RpNonModalSelection=OpenReportHelper(displayReport,DateSelected,listReportPermissions,doValidatePerm);
			switch(displayReport.InternalName) {
				//Non-modal report windows are handled after closing.
				case DisplayReports.ReportNames.UnfinalizedInsPay:
				case DisplayReports.ReportNames.OutstandingInsClaims:
				case DisplayReports.ReportNames.ClaimsNotSent:
				case DisplayReports.ReportNames.CustomAging:
				case DisplayReports.ReportNames.TreatmentFinder:
				case DisplayReports.ReportNames.WebSchedAppointments:
				case DisplayReports.ReportNames.ReferredProcTracking:
				case DisplayReports.ReportNames.ProcNotBilledIns:
				case DisplayReports.ReportNames.IncompleteProcNotes:
				case DisplayReports.ReportNames.ODProcOverpaid:
				case DisplayReports.ReportNames.DPPOvercharged:
					Close();
				break;
			}
		}

		///<summary>Handles form and logic for most given displayReports.
		///Returns ReportNonModalSelection.None if modal report is provided.
		///If non-modal report is provided returns valid(not none) RpNonModalSelection to be handled later, See FormOpenDental.SpecialReportSelectionHelper(...)</summary>
		public static ReportNonModalSelection OpenReportHelper(DisplayReport displayReport,DateTime dateSelected
			,List<GroupPermission> listReportPermissions=null,bool doValidatePerm=true)
		{
			if(doValidatePerm) {
				if(listReportPermissions==null) {
					listReportPermissions=GroupPermissions.GetPermsForReports().Where(x => Security.CurUser.IsInUserGroup(x.UserGroupNum)).ToList();
				}
				if(!listReportPermissions.Exists(x => x.FKey==displayReport.DisplayReportNum)) {
					MsgBox.Show("FormReportsMore","You do not have permission to run this report.");
					return ReportNonModalSelection.None;
				}
			}
			switch(displayReport.InternalName) {
				case "ODToday"://Today
					using(FormRpProdInc FormPI=new FormRpProdInc()) {
						FormPI.DailyMonthlyAnnual="Daily";
						FormPI.DateStart=DateTime.Today;
						FormPI.DateEnd=DateTime.Today;
						FormPI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for today.");
					break;
				case "ODYesterday"://Yesterday
					using(FormRpProdInc FormPI = new FormRpProdInc()) {
						FormPI.DailyMonthlyAnnual="Daily";
						if(DateTime.Today.DayOfWeek==DayOfWeek.Monday) {
							FormPI.DateStart=DateTime.Today.AddDays(-3);
							FormPI.DateEnd=DateTime.Today.AddDays(-3);
						}
						else {
							FormPI.DateStart=DateTime.Today.AddDays(-1);
							FormPI.DateEnd=DateTime.Today.AddDays(-1);
						}
						FormPI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for yesterday.");
					break;
				case "ODThisMonth"://This Month
					using(FormRpProdInc FormPI = new FormRpProdInc()) {
						FormPI.DailyMonthlyAnnual="Monthly";
						FormPI.DateStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
						FormPI.DateEnd=new DateTime(DateTime.Today.AddMonths(1).Year,DateTime.Today.AddMonths(1).Month,1).AddDays(-1);
						FormPI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for this month.");
					break;
				case "ODLastMonth"://Last Month
					using(FormRpProdInc FormPI = new FormRpProdInc()) {
						FormPI.DailyMonthlyAnnual="Monthly";
						FormPI.DateStart=new DateTime(DateTime.Today.AddMonths(-1).Year,DateTime.Today.AddMonths(-1).Month,1);
						FormPI.DateEnd=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddDays(-1);
						FormPI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for last month.");
					break;
				case "ODThisYear"://This Year
					using(FormRpProdInc FormPI = new FormRpProdInc()) {
						FormPI.DailyMonthlyAnnual="Annual";
						FormPI.DateStart=new DateTime(DateTime.Today.Year,1,1);
						FormPI.DateEnd=new DateTime(DateTime.Today.Year,12,31);
						FormPI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for this year.");
					break;
				case "ODMoreOptions"://More Options
					using(FormRpProdInc FormPI = new FormRpProdInc()) {
						FormPI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report 'more options' accessed.");
					break;
				case "ODProviderPayrollSummary":
					using(FormRpProviderPayroll FormPP=new FormRpProviderPayroll()) {
						FormPP.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Provider Payroll Summary report run.");
					break;
				case "ODProviderPayrollDetailed":
					using(FormRpProviderPayroll FormPPD=new FormRpProviderPayroll(true)) {
						FormPPD.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Provider Payroll Detailed report run.");
					break;
				case "ODAdjustments"://Adjustments
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpAdjSheet FormAdjSheet=new FormRpAdjSheet()) {
						FormAdjSheet.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Adjustments report run.");
					break;
				case "ODPayments"://Payments
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpPaySheet FormPaySheet=new FormRpPaySheet()) {
						FormPaySheet.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Payments report run.");
					break;
				case "ODProcedures"://Procedures
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpProcSheet FormProcSheet=new FormRpProcSheet()) {
						FormProcSheet.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Procedures report run.");
					break;
				case DisplayReports.ReportNames.ODProcOverpaid:
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Procedures overpaid report run.");
					return ReportNonModalSelection.ODProcsOverpaid;
				case "ODWriteoffs"://Writeoffs
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpWriteoffSheet FormW=new FormRpWriteoffSheet()) {
						FormW.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Writeoffs report run.");
					break;
				case DisplayReports.ReportNames.IncompleteProcNotes://Incomplete Procedure Notes
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Procedure Notes report run.");
					return ReportNonModalSelection.IncompleteProcNotes;
				case "ODRoutingSlips"://Routing Slips
					using(FormRpRouting FormR=new FormRpRouting()) {
						FormR.DateSelected=dateSelected;
						FormR.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Routing Slips report run.");
					break;
				case "ODNetProdDetailDaily":
					using(FormRpNetProdDetail FormNetProdDetail=new FormRpNetProdDetail(true)) {
						FormNetProdDetail.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Net Prod report run.");
					break;
				case DisplayReports.ReportNames.UnfinalizedInsPay:
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Unfinalized Insurance Payment report run.");
					return ReportNonModalSelection.UnfinalizedInsPay;
				case DisplayReports.ReportNames.PatPortionUncollected:
					using(FormRpPatPortionUncollected FormPPU=new FormRpPatPortionUncollected()) {
						FormPPU.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Patient Portion Uncollected report run.");
					break;
				case "ODAgingAR"://Aging of Accounts Receivable Report
					using(FormRpAging FormA=new FormRpAging()) {
						FormA.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Aging of A/R report run.");
					break;
				case DisplayReports.ReportNames.ClaimsNotSent://Claims Not Sent
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Claims Not Sent report run.");
					return ReportNonModalSelection.UnsentClaim;
				case "ODCapitation"://Capitation Utilization
					using(FormRpCapitation FormC=new FormRpCapitation()) {
						FormC.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Capitation report run.");
					break;
				case "ODFinanceCharge"://Finance Charge Report
					using(FormRpFinanceCharge FormRpFinance=new FormRpFinanceCharge()) {
						FormRpFinance.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Finance Charges report run.");
					break;
				case DisplayReports.ReportNames.OutstandingInsClaims://Outstanding Insurance Claims
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Outstanding Insurance Claims report run.");
					return ReportNonModalSelection.OutstandingIns;
				case DisplayReports.ReportNames.ProcNotBilledIns://Procedures Not Billed to Insurance
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Procedures not billed to insurance report run.");
					return ReportNonModalSelection.ProcNotBilledIns;
				case "ODPPOWriteoffs"://PPO Writeoffs
					using(FormRpPPOwriteoffs FormPPO=new FormRpPPOwriteoffs()) {
						FormPPO.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"PPO Writeoffs report run.");
					break;
				case "ODPaymentPlans"://Payment Plans
					using(FormRpPayPlans FormPayPlans=new FormRpPayPlans()) {
						FormPayPlans.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Payment Plans report run.");
					break;
				case "ODReceivablesBreakdown"://Receivable Breakdown
					using(FormRpReceivablesBreakdown FormRcv = new FormRpReceivablesBreakdown()) {
						FormRcv.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Receivable Breakdown report run.");
					break;
				case "ODUnearnedIncome"://Unearned Income
					using(FormRpUnearnedIncome FormU=new FormRpUnearnedIncome()) {
						FormU.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Unearned Income report run.");
					break;
				case "ODInsuranceOverpaid"://Insurance Overpaid
					using(FormRpInsOverpaid FormI=new FormRpInsOverpaid()) {
						FormI.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Overpaid report run.");
					break;
				case "ODPresentedTreatmentProd"://Treatment Planned Presenter
					using(FormRpPresentedTreatmentProduction FormPTP=new FormRpPresentedTreatmentProduction()) {
						FormPTP.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Treatment Plan Presenter report run.");
					break;
				case "ODTreatmentPresentationStats"://Treatment Planned Presenter
					using(FormRpTreatPlanPresentationStatistics FormTPS = new FormRpTreatPlanPresentationStatistics()) {
						FormTPS.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Treatment Plan Presented Procedures report run.");
					break;
				case "ODInsurancePayPlansPastDue"://Insurance Payment Plans
					using(FormRpInsPayPlansPastDue FormIPP = new FormRpInsPayPlansPastDue()) {
						FormIPP.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Payment Plan report run.");
					break;
				case DisplayReports.ReportNames.InsAging://Insurance Aging Report
					using(FormRpInsAging FormIA=new FormRpInsAging()) {
						FormIA.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Aging report run");
					break;
				case DisplayReports.ReportNames.CustomAging://Insurance Aging Report
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Custom Aging report run");
					return ReportNonModalSelection.CustomAging;
				case "ODActivePatients"://Active Patients
					using(FormRpActivePatients FormAP=new FormRpActivePatients()) {
						FormAP.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Active Patients report run.");
					break;
				case "ODAppointments"://Appointments
					using(FormRpAppointments FormAppointments=new FormRpAppointments()) {
						FormAppointments.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Appointments report run.");
					break;
				case "ODBirthdays"://Birthdays
					using(FormRpBirthday FormB=new FormRpBirthday()) {
						FormB.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Birthdays report run.");
					break;
				case "ODBrokenAppointments"://Broken Appointments
					using(FormRpBrokenAppointments FormBroken=new FormRpBrokenAppointments()) {
						FormBroken.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Broken Appointments report run.");
					break;
				case "ODInsurancePlans"://Insurance Plans
					using(FormRpInsCo FormInsCo=new FormRpInsCo()) {
						FormInsCo.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Plans report run.");
					break;
				case "ODNewPatients"://New Patients
					using(FormRpNewPatients FormNewPats=new FormRpNewPatients()) {
						FormNewPats.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"New Patients report run.");
					break;
				case "ODPatientsRaw"://Patients - Raw
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpPatients FormPatients=new FormRpPatients()) {
						FormPatients.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Patients - Raw report run.");
					break;
				case "ODPatientNotes"://Patient Notes
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormSearchPatNotes FormSearchPatNotes=new FormSearchPatNotes()) {
						FormSearchPatNotes.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Patient Notes report run.");
					break;
				case "ODPrescriptions"://Prescriptions
					using(FormRpPrescriptions FormPrescript=new FormRpPrescriptions()) {
						FormPrescript.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Rx report run.");
					break;
				case "ODProcedureCodes"://Procedure Codes - Fee Schedules
					using(FormRpProcCodes FormProcCodes=new FormRpProcCodes()) {
						FormProcCodes.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Procedure Codes - Fee Schedules report run.");
					break;
				case "ODReferralsRaw"://Referrals - Raw
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpReferrals FormReferral=new FormRpReferrals()) {
						FormReferral.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Referrals - Raw report run.");
					break;
				case "ODReferralAnalysis"://Referral Analysis
					using(FormRpReferralAnalysis FormRA=new FormRpReferralAnalysis()) {
						FormRA.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Referral Analysis report run.");
					break;
				case DisplayReports.ReportNames.ReferredProcTracking://Referred Proc Tracking
					using(FormReferralProcTrack FormRP=new FormReferralProcTrack()) {
						FormRP.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"ReferredProcTracking report run.");
					break;
				case DisplayReports.ReportNames.TreatmentFinder://Treatment Finder
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Treatment Finder report run.");
					return ReportNonModalSelection.TreatmentFinder;
				case DisplayReports.ReportNames.WebSchedAppointments://Web Sched Appts
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Web Sched Appointments report run.");
					return ReportNonModalSelection.WebSchedAppointments;
				case "ODRawScreeningData"://Raw Screening Data
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpPHRawScreen FormPH=new FormRpPHRawScreen()) {
						FormPH.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"PH Raw Screening");
					break;
				case "ODRawPopulationData"://Raw Population Data
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpPHRawPop FormPHR=new FormRpPHRawPop()) {
						FormPHR.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"PH Raw population");
					break;
				case "ODDentalSealantMeasure"://FQHC Dental Sealant Measure
					using(FormRpDentalSealantMeasure FormDSM=new FormRpDentalSealantMeasure()) {
						FormDSM.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"FQHC Dental Sealant Measure report run.");
					break;
				case "ODEligibilityFile"://Eligibility File
					using(FormRpArizonaPrimaryCareEligibility frapce=new FormRpArizonaPrimaryCareEligibility()) {
						frapce.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Arizona Primary Care Eligibility");
					break;
				case "ODEncounterFile"://Encounter File
					using(FormRpArizonaPrimaryCareEncounter frapcn=new FormRpArizonaPrimaryCareEncounter()) {
						frapcn.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Arizona Primary Care Encounter");
					break;
				case "ODDiscountPlan"://Discount Plans
					using(FormRpDiscountPlan FormDiscountPlan=new FormRpDiscountPlan()) {
						FormDiscountPlan.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Discount Plans report run.");
					break;
				case "ODMonthlyProductionGoal"://Monthly Production Goal
					using(FormRpProdGoal FormProdGoal=new FormRpProdGoal()) {
						FormProdGoal.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Monthly Production Goal report run.");
					break;
				case "ODHiddenPaySplits":
					using(FormRpHiddenPaySplits formRpHiddenPaySplits=new FormRpHiddenPaySplits()) {
						formRpHiddenPaySplits.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Hidden PaySplits report run.");
					break;
				case "ODDynamicPayPlanOvercharged":
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Dynamic Payment Plans Overcharged report run.");
					return ReportNonModalSelection.DPPOvercharged;
				default:
					MsgBox.Show("FormReportsMore","Error finding the report");
					break;
			}
			return ReportNonModalSelection.None;
		}

		private void butLaserLabels_Click(object sender,EventArgs e) {
			using FormRpLaserLabels LaserLabels = new FormRpLaserLabels();
			LaserLabels.ShowDialog();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormReportSetup formRS = new FormReportSetup(0,false); //no need to pass in a usergroupnum.
			if(formRS.ShowDialog()==DialogResult.OK) {
				FillLists();
			}
		}

		private void butPatList_Click(object sender,EventArgs e) {
			using FormPatListEHR2014 FormPL=new FormPatListEHR2014();
			FormPL.ShowDialog();
		}

		private void butPatExport_Click(object sender,EventArgs e) {
			using FormEhrPatientExport FormEhrPE=new FormEhrPatientExport();
			FormEhrPE.ShowDialog();
		}
		
		private void picturePracticeByNumbers_Click(object sender,EventArgs e) {
			PracticeByNumbers.ShowPage();
		}

		private void picturePodium_Click(object sender,EventArgs e) {
			try {
				Podium.ShowPage();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

	}

	///<summary>Used in FormReportsMore to indicate that a non-modal window should be shown.</summary>
	public enum ReportNonModalSelection {
		///<summary></summary>
		None,
		///<summary></summary>
		TreatmentFinder,
		///<summary></summary>
		OutstandingIns,
		///<summary></summary>
		UnfinalizedInsPay,
		///<summary></summary>
		UnsentClaim,
		WebSchedAppointments,
		CustomAging,
		IncompleteProcNotes,
		ProcNotBilledIns,
		ODProcsOverpaid,
		DPPOvercharged,
	}
}