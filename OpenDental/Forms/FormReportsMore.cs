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
		public ReportNonModalSelection ReportNonModalSelection_;
		///<summary>The Date currently selected on the Appointment Module.</summary>
		public DateTime DateSelected;
		private List<DisplayReport> _listDisplayReportsProdInc;
		private List<DisplayReport> _listDisplayReportsMonthly;
		private List<DisplayReport> _listDisplayReportsDaily;
		private List<DisplayReport> _listDisplayReportsList;
		private List<DisplayReport> _listDisplayReportsPublicHealth;
		private List<DisplayReport> _listDisplayReportsArizonaPrimary;
		private List<GroupPermission> _listGroupPermissions;

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
			_listDisplayReportsProdInc=DisplayReports.GetForCategory(DisplayReportCategory.ProdInc,false);
			_listDisplayReportsMonthly=DisplayReports.GetForCategory(DisplayReportCategory.Monthly,false);
			_listDisplayReportsDaily=DisplayReports.GetForCategory(DisplayReportCategory.Daily,false);
			_listDisplayReportsList=DisplayReports.GetForCategory(DisplayReportCategory.Lists,false);
			_listDisplayReportsPublicHealth=DisplayReports.GetForCategory(DisplayReportCategory.PublicHealth,false);
			_listDisplayReportsArizonaPrimary=DisplayReports.GetForCategory(DisplayReportCategory.ArizonaPrimaryCare,false);
			_listGroupPermissions=GroupPermissions.GetPermsForReports(Security.CurUser);
			//add the items to the list boxes and set the list box heights. (positions too?)
			listProdInc.Items.Clear();
			listDaily.Items.Clear();
			listMonthly.Items.Clear();
			listLists.Items.Clear();
			listPublicHealth.Items.Clear();
			listArizonaPrimaryCare.Items.Clear();
			//listUDSReports.Items.Clear();
			//Production and Income------------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<_listDisplayReportsProdInc.Count;i++) {
				if(GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsProdInc[i].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
					listProdInc.Items.Add(_listDisplayReportsProdInc[i].Description);
				}
				else {
					listProdInc.Items.Add(_listDisplayReportsProdInc[i].Description+" [Locked]");
				}
			}
			if(_listDisplayReportsProdInc.Count==0) {
				listProdInc.Visible=false;
				labelProdInc.Visible=false;
			}
			else {
				listProdInc.Visible=true;
				labelProdInc.Visible=true;
				LayoutManager.MoveHeight(listProdInc,Math.Min((_listDisplayReportsProdInc.Count+1) * LayoutManager.Scale(15),listProdInc.Height));
			}
			//Daily----------------------------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<_listDisplayReportsDaily.Count;i++) {
				if(GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsDaily[i].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
					listDaily.Items.Add(_listDisplayReportsDaily[i].Description);
				}
				else {
					listDaily.Items.Add(_listDisplayReportsDaily[i].Description+" [Locked]");
				}
			}
			if(_listDisplayReportsDaily.Count==0) {
				listDaily.Visible=false;
				labelDaily.Visible=false;
			}
			else {
				listDaily.Visible=true;
				labelDaily.Visible=true;
				listDaily.Height=Math.Min((_listDisplayReportsDaily.Count+1) * LayoutManager.Scale(15),listDaily.Height);
			}
			//Monthly--------------------------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<_listDisplayReportsMonthly.Count;i++) {
				if(GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsMonthly[i].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
					listMonthly.Items.Add(_listDisplayReportsMonthly[i].Description);
				}
				else {
					listMonthly.Items.Add(_listDisplayReportsMonthly[i].Description+" [Locked]");
				}
			}
			if(_listDisplayReportsMonthly.Count==0) {
				listMonthly.Visible=false;
				labelMonthly.Visible=false;
			}
			else {
				listMonthly.Visible=true;
				labelMonthly.Visible=true;
				listMonthly.Height=Math.Min((_listDisplayReportsMonthly.Count+1) * LayoutManager.Scale(15),listMonthly.Height);
			}
			//Lists----------------------------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<_listDisplayReportsList.Count;i++) {
				if(GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsList[i].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
					listLists.Items.Add(_listDisplayReportsList[i].Description);
				}
				else {
					listLists.Items.Add(_listDisplayReportsList[i].Description+" [Locked]");
				}
			}
			if(_listDisplayReportsList.Count==0) {
				listLists.Visible=false;
				labelLists.Visible=false;
			}
			else {
				listLists.Visible=true;
				labelLists.Visible=true;
				listLists.Height=Math.Min((_listDisplayReportsList.Count+1) * LayoutManager.Scale(15),listLists.Height);
			}
			//Public Health--------------------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<_listDisplayReportsPublicHealth.Count;i++) {
				if(GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsPublicHealth[i].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
					listPublicHealth.Items.Add(_listDisplayReportsPublicHealth[i].Description);
				}
				else {
					listPublicHealth.Items.Add(_listDisplayReportsPublicHealth[i].Description+" [Locked]");
				}
			}
			if(_listDisplayReportsPublicHealth.Count==0) {
				listPublicHealth.Visible=false;
				labelPublicHealth.Visible=false;
			}
			else {
				listPublicHealth.Visible=true;
				labelPublicHealth.Visible=true;
				LayoutManager.MoveHeight(listPublicHealth,Math.Min((_listDisplayReportsPublicHealth.Count+1) * LayoutManager.Scale(15),listPublicHealth.Height));
			}
			//Arizona Primary Care-------------------------------------------------------------------------------------------------------------------------
			//Arizona primary care list and label must only be visible when the Arizona primary
			//care option is checked in the miscellaneous options.
			for(int i=0;i<_listDisplayReportsArizonaPrimary.Count;i++) {
				if(GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsArizonaPrimary[i].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
					listArizonaPrimaryCare.Items.Add(_listDisplayReportsArizonaPrimary[i].Description);
				}
				else {
					listArizonaPrimaryCare.Items.Add(_listDisplayReportsArizonaPrimary[i].Description+" [Locked]");
				}
			}
			if(_listDisplayReportsArizonaPrimary.Count==0 || !UsingArizonaPrimaryCare()) {
				listArizonaPrimaryCare.Visible=false;
				labelArizonaPrimaryCare.Visible=false;
			}
			else {
				listArizonaPrimaryCare.Visible=true;
				labelArizonaPrimaryCare.Visible=true;
				listArizonaPrimaryCare.Height=Math.Min((_listDisplayReportsArizonaPrimary.Count+1) * LayoutManager.Scale(15),49);
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
			List<PatFieldDef> listPatFieldDefs=PatFieldDefs.GetDeepCopy(true);
			for(int i=0;i<listPatFieldDefs.Count;i++) {
				for(int j=0;j<patientFieldNames.Length;j++) {
					if(listPatFieldDefs[i].FieldName.ToLower()==patientFieldNames[j].ToLower()) {
						fieldCounts[j]++;
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
				using FormUserQuery formUserQuery=new FormUserQuery(null);
				formUserQuery.ShowDialog();
				SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,"");
			}
			else {
				using FormQueryFavorites formQueryFavorites = new FormQueryFavorites();
				formQueryFavorites.ShowDialog();
				if(formQueryFavorites.DialogResult == DialogResult.OK) {
					using FormUserQuery formUserQuery=new FormUserQuery(formQueryFavorites.UserQueryCur.QueryText,true);
					formUserQuery.textQuery.Text=formQueryFavorites.UserQueryCur.QueryText;
					formUserQuery.textTitle.Text=formQueryFavorites.UserQueryCur.FileName;
					SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,Lan.g(this,"User query form accessed."));
					formUserQuery.ShowDialog();
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
			if(!GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,_listDisplayReportsProdInc[selected].DisplayReportNum,listGroupPermissions:_listGroupPermissions)) {
				MsgBox.Show(this,"You do not have permission to run this report.");
				return;
			}
			if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				MsgBox.Show(this,"The current user needs to be a provider or have the 'All Providers' permission for this report");
				return;
			}
			OpenReportLocalHelper(_listDisplayReportsProdInc[selected],_listGroupPermissions,false);
		}

		private void listDaily_MouseDown(object sender,MouseEventArgs e) {
			int selected=listDaily.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listDisplayReportsDaily[selected],_listGroupPermissions);
		}

		private void listMonthly_MouseDown(object sender,MouseEventArgs e) {
			int selected=listMonthly.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listDisplayReportsMonthly[selected],_listGroupPermissions);
		}

		private void listLists_MouseDown(object sender,MouseEventArgs e) {
			int selected=listLists.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listDisplayReportsList[selected],_listGroupPermissions);
		}

		private void listPublicHealth_MouseDown(object sender,MouseEventArgs e) {
			int selected=listPublicHealth.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listDisplayReportsPublicHealth[selected],_listGroupPermissions);
		}

		private void listArizonaPrimaryCare_MouseDown(object sender,MouseEventArgs e) {
			int selected=this.listArizonaPrimaryCare.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			OpenReportLocalHelper(_listDisplayReportsArizonaPrimary[selected],_listGroupPermissions,false);
		}
		
		///<summary>Called from this form to do OpenReportHelper(...) logic and then close when needed.</summary>
		private void OpenReportLocalHelper(DisplayReport displayReport,List<GroupPermission> listReportPermissions,bool doValidatePerm=true)
		{
			ReportNonModalSelection_=OpenReportHelper(displayReport,DateSelected,listReportPermissions,doValidatePerm);
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
				case DisplayReports.ReportNames.PatPortionUncollected:
				case DisplayReports.ReportNames.EraAutoProcessed:
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
					listReportPermissions=GroupPermissions.GetPermsForReports(Security.CurUser);
				}
				if(!GroupPermissions.HasPermission(Security.CurUser,Permissions.Reports,displayReport.DisplayReportNum,listGroupPermissions:listReportPermissions)) {
					MsgBox.Show("FormReportsMore","You do not have permission to run this report.");
					return ReportNonModalSelection.None;
				}
			}
			switch(displayReport.InternalName) {
				case "ODToday"://Today
					using(FormRpProdInc formRpProdInc=new FormRpProdInc()) {
						formRpProdInc.DailyMonthlyAnnual="Daily";
						formRpProdInc.DateStart=DateTime.Today;
						formRpProdInc.DateEnd=DateTime.Today;
						formRpProdInc.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for today.");
					break;
				case "ODYesterday"://Yesterday
					using(FormRpProdInc formRpProdInc = new FormRpProdInc()) {
						formRpProdInc.DailyMonthlyAnnual="Daily";
						if(DateTime.Today.DayOfWeek==DayOfWeek.Monday) {
							formRpProdInc.DateStart=DateTime.Today.AddDays(-3);
							formRpProdInc.DateEnd=DateTime.Today.AddDays(-3);
						}
						else {
							formRpProdInc.DateStart=DateTime.Today.AddDays(-1);
							formRpProdInc.DateEnd=DateTime.Today.AddDays(-1);
						}
						formRpProdInc.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for yesterday.");
					break;
				case "ODThisMonth"://This Month
					using(FormRpProdInc formRpProdInc = new FormRpProdInc()) {
						formRpProdInc.DailyMonthlyAnnual="Monthly";
						formRpProdInc.DateStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
						formRpProdInc.DateEnd=new DateTime(DateTime.Today.AddMonths(1).Year,DateTime.Today.AddMonths(1).Month,1).AddDays(-1);
						formRpProdInc.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for this month.");
					break;
				case "ODLastMonth"://Last Month
					using(FormRpProdInc formRpProdInc = new FormRpProdInc()) {
						formRpProdInc.DailyMonthlyAnnual="Monthly";
						formRpProdInc.DateStart=new DateTime(DateTime.Today.AddMonths(-1).Year,DateTime.Today.AddMonths(-1).Month,1);
						formRpProdInc.DateEnd=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddDays(-1);
						formRpProdInc.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for last month.");
					break;
				case "ODThisYear"://This Year
					using(FormRpProdInc formRpProdInc = new FormRpProdInc()) {
						formRpProdInc.DailyMonthlyAnnual="Annual";
						formRpProdInc.DateStart=new DateTime(DateTime.Today.Year,1,1);
						formRpProdInc.DateEnd=new DateTime(DateTime.Today.Year,12,31);
						formRpProdInc.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report run for this year.");
					break;
				case "ODMoreOptions"://More Options
					using(FormRpProdInc formRpProdInc = new FormRpProdInc()) {
						formRpProdInc.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Production and Income report 'more options' accessed.");
					break;
				case "ODProviderPayrollSummary":
					using(FormRpProviderPayroll formRpProviderPayroll=new FormRpProviderPayroll()) {
						formRpProviderPayroll.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Provider Payroll Summary report run.");
					break;
				case "ODProviderPayrollDetailed":
					using(FormRpProviderPayroll formRpProviderPayroll=new FormRpProviderPayroll(true)) {
						formRpProviderPayroll.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Provider Payroll Detailed report run.");
					break;
				case "ODAdjustments"://Adjustments
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpAdjSheet formRpAdjSheet=new FormRpAdjSheet()) {
						formRpAdjSheet.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Adjustments report run.");
					break;
				case "ODPayments"://Payments
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpPaySheet formRpPaySheet=new FormRpPaySheet()) {
						formRpPaySheet.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Payments report run.");
					break;
				case "ODProcedures"://Procedures
					if(Security.CurUser.ProvNum==0 && !Security.IsAuthorized(Permissions.ReportDailyAllProviders,true)) {
						MsgBox.Show("FormReportsMore","The current user needs to be a provider or have the 'All Providers' permission for Daily reports");
						break;
					}
					using(FormRpProcSheet formRpProcSheet=new FormRpProcSheet()) {
						formRpProcSheet.ShowDialog();
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
					using(FormRpWriteoffSheet formRpWriteoffSheet=new FormRpWriteoffSheet()) {
						formRpWriteoffSheet.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Writeoffs report run.");
					break;
				case DisplayReports.ReportNames.IncompleteProcNotes://Incomplete Procedure Notes
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Procedure Notes report run.");
					return ReportNonModalSelection.IncompleteProcNotes;
				case "ODRoutingSlips"://Routing Slips
					using(FormRpRouting formRpRouting=new FormRpRouting()) {
						formRpRouting.DateSelected=dateSelected;
						formRpRouting.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Routing Slips report run.");
					break;
				case "ODNetProdDetailDaily":
					using(FormRpNetProdDetail formRpNetProdDetail=new FormRpNetProdDetail(true)) {
						formRpNetProdDetail.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Daily Net Prod report run.");
					break;
				case DisplayReports.ReportNames.UnfinalizedInsPay:
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Unfinalized Insurance Payment report run.");
					return ReportNonModalSelection.UnfinalizedInsPay;
				case DisplayReports.ReportNames.PatPortionUncollected:
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Patient Portion Uncollected report run.");
					return ReportNonModalSelection.PatPortionUncollected;
				case "ODAgingAR"://Aging of Accounts Receivable Report
					using(FormRpAging formRpAging=new FormRpAging()) {
						formRpAging.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Aging of A/R report run.");
					break;
				case DisplayReports.ReportNames.ClaimsNotSent://Claims Not Sent
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Claims Not Sent report run.");
					return ReportNonModalSelection.UnsentClaim;
				case "ODCapitation"://Capitation Utilization
					using(FormRpCapitation formRpCapitation=new FormRpCapitation()) {
						formRpCapitation.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Capitation report run.");
					break;
				case "ODFinanceCharge"://Finance Charge Report
					using(FormRpFinanceCharge formRpFinanceCharge=new FormRpFinanceCharge()) {
						formRpFinanceCharge.ShowDialog();
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
					using(FormRpPPOwriteoffs formRpPPOwriteoffs=new FormRpPPOwriteoffs()) {
						formRpPPOwriteoffs.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"PPO Writeoffs report run.");
					break;
				case "ODPaymentPlans"://Payment Plans
					using(FormRpPayPlans formRpPayPlans=new FormRpPayPlans()) {
						formRpPayPlans.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Payment Plans report run.");
					break;
				case "ODReceivablesBreakdown"://Receivable Breakdown
					using(FormRpReceivablesBreakdown formRpReceivablesBreakdown = new FormRpReceivablesBreakdown()) {
						formRpReceivablesBreakdown.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Receivable Breakdown report run.");
					break;
				case "ODUnearnedIncome"://Unearned Income
					using(FormRpUnearnedIncome formRpUnearnedIncome=new FormRpUnearnedIncome()) {
						formRpUnearnedIncome.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Unearned Income report run.");
					break;
				case "ODInsuranceOverpaid"://Insurance Overpaid
					using(FormRpInsOverpaid formRpInsOverpaid=new FormRpInsOverpaid()) {
						formRpInsOverpaid.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Overpaid report run.");
					break;
				case "ODPresentedTreatmentProd"://Treatment Planned Presenter
					using(FormRpPresentedTreatmentProduction formRpPresentedTreatmentProduction=new FormRpPresentedTreatmentProduction()) {
						formRpPresentedTreatmentProduction.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Treatment Plan Presenter report run.");
					break;
				case "ODTreatmentPresentationStats"://Treatment Planned Presenter
					using(FormRpTreatPlanPresentationStatistics formRpTreatPlanPresentationStatistics = new FormRpTreatPlanPresentationStatistics()) {
						formRpTreatPlanPresentationStatistics.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Treatment Plan Presented Procedures report run.");
					break;
				case "ODInsurancePayPlansPastDue"://Insurance Payment Plans
					using(FormRpInsPayPlansPastDue formRpInsPayPlansPastDue = new FormRpInsPayPlansPastDue()) {
						formRpInsPayPlansPastDue.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Payment Plan report run.");
					break;
				case DisplayReports.ReportNames.InsAging://Insurance Aging Report
					using(FormRpInsAging formRpInsAging=new FormRpInsAging()) {
						formRpInsAging.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Aging report run");
					break;
				case DisplayReports.ReportNames.CustomAging://Insurance Aging Report
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Custom Aging report run");
					return ReportNonModalSelection.CustomAging;
				case "ODActivePatients"://Active Patients
					using(FormRpActivePatients formRpActivePatients=new FormRpActivePatients()) {
						formRpActivePatients.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Active Patients report run.");
					break;
				case "ODAppointments"://Appointments
					using(FormRpAppointments formRpAppointments=new FormRpAppointments()) {
						formRpAppointments.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Appointments report run.");
					break;
				case "ODBirthdays"://Birthdays
					using(FormRpBirthday formRpBirthday=new FormRpBirthday()) {
						formRpBirthday.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Birthdays report run.");
					break;
				case "ODBrokenAppointments"://Broken Appointments
					using(FormRpBrokenAppointments formRpBrokenAppointments=new FormRpBrokenAppointments()) {
						formRpBrokenAppointments.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Broken Appointments report run.");
					break;
				case "ODInsurancePlans"://Insurance Plans
					using(FormRpInsCo formRpInsCo=new FormRpInsCo()) {
						formRpInsCo.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Insurance Plans report run.");
					break;
				case "ODNewPatients"://New Patients
					using(FormRpNewPatients formRpNewPatients=new FormRpNewPatients()) {
						formRpNewPatients.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"New Patients report run.");
					break;
				case "ODPatientsRaw"://Patients - Raw
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpPatients formRpPatients=new FormRpPatients()) {
						formRpPatients.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Patients - Raw report run.");
					break;
				case "ODPatientNotes"://Patient Notes
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormSearchPatNotes formSearchPatNotes=new FormSearchPatNotes()) {
						formSearchPatNotes.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Patient Notes report run.");
					break;
				case "ODPrescriptions"://Prescriptions
					using(FormRpPrescriptions formRpPrescriptions=new FormRpPrescriptions()) {
						formRpPrescriptions.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Rx report run.");
					break;
				case "ODProcedureCodes"://Procedure Codes - Fee Schedules
					using(FormRpProcCodes formRpProcCodes=new FormRpProcCodes()) {
						formRpProcCodes.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Procedure Codes - Fee Schedules report run.");
					break;
				case "ODReferralsRaw"://Referrals - Raw
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpReferrals formRpReferrals=new FormRpReferrals()) {
						formRpReferrals.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Referrals - Raw report run.");
					break;
				case "ODReferralAnalysis"://Referral Analysis
					using(FormRpReferralAnalysis formRpReferralAnalysis=new FormRpReferralAnalysis()) {
						formRpReferralAnalysis.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Referral Analysis report run.");
					break;
				case DisplayReports.ReportNames.ReferredProcTracking://Referred Proc Tracking
					using(FormReferralProcTrack formReferralProcTrack=new FormReferralProcTrack()) {
						formReferralProcTrack.ShowDialog();
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
					using(FormRpPHRawScreen formRpPHRawScreen=new FormRpPHRawScreen()) {
						formRpPHRawScreen.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"PH Raw Screening");
					break;
				case "ODRawPopulationData"://Raw Population Data
					if(!Security.IsAuthorized(Permissions.UserQuery)) {
						break;
					}
					using(FormRpPHRawPop formRpPHRawPop=new FormRpPHRawPop()) {
						formRpPHRawPop.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"PH Raw population");
					break;
				case "ODDentalSealantMeasure"://FQHC Dental Sealant Measure
					using(FormRpDentalSealantMeasure formRpDentalSealantMeasure=new FormRpDentalSealantMeasure()) {
						formRpDentalSealantMeasure.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"FQHC Dental Sealant Measure report run.");
					break;
				case "ODEligibilityFile"://Eligibility File
					using(FormRpArizonaPrimaryCareEligibility formRpArizonaPrimaryCareEligibility=new FormRpArizonaPrimaryCareEligibility()) {
						formRpArizonaPrimaryCareEligibility.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Arizona Primary Care Eligibility");
					break;
				case "ODEncounterFile"://Encounter File
					using(FormRpArizonaPrimaryCareEncounter formRpArizonaPrimaryCareEncounter=new FormRpArizonaPrimaryCareEncounter()) {
						formRpArizonaPrimaryCareEncounter.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Arizona Primary Care Encounter");
					break;
				case "ODDiscountPlan"://Discount Plans
					using(FormRpDiscountPlan formRpDiscountPlan=new FormRpDiscountPlan()) {
						formRpDiscountPlan.ShowDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"Discount Plans report run.");
					break;
				case "ODMonthlyProductionGoal"://Monthly Production Goal
					using(FormRpProdGoal formRpProdGoal=new FormRpProdGoal()) {
						formRpProdGoal.ShowDialog();
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
				case DisplayReports.ReportNames.EraAutoProcessed:
					SecurityLogs.MakeLogEntry(Permissions.Reports,0,"ERAs Automatically Processed report run.");
					return ReportNonModalSelection.EraAutoProcessed;
				default:
					MsgBox.Show("FormReportsMore","Error finding the report");
					break;
			}
			return ReportNonModalSelection.None;
		}

		private void butLaserLabels_Click(object sender,EventArgs e) {
			using FormRpLaserLabels formRpLaserLabels = new FormRpLaserLabels();
			formRpLaserLabels.ShowDialog();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormReportSetup formReportSetup = new FormReportSetup(0,false); //no need to pass in a usergroupnum.
			if(formReportSetup.ShowDialog()==DialogResult.OK) {
				FillLists();
			}
		}

		private void butPatList_Click(object sender,EventArgs e) {
			using FormPatListEHR2014 formPatListEHR2014=new FormPatListEHR2014();
			formPatListEHR2014.ShowDialog();
		}

		private void butPatExport_Click(object sender,EventArgs e) {
			using FormEhrPatientExport formEhrPatientExport=new FormEhrPatientExport();
			formEhrPatientExport.ShowDialog();
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
		PatPortionUncollected,
		EraAutoProcessed
	}
}