using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using OpenDental.UI;
using System.Drawing.Printing;
using OpenDental.ReportingComplex;

namespace OpenDental {
	public partial class FormRpCustomAging:FormODBase {
		private AgingOptions _agingOptions;

		public FormRpCustomAging() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpCustomAging_Load(object sender,EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
			switch(PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {
				case 0:	radioWriteoffInsPayDate.Checked=true; break;
				case 1:	radioWriteoffProcDate.Checked=true; break;
				case 2:	radioWriteoffClaimDate.Checked=true; break;
				default: radioWriteoffClaimDate.Checked=true; break;
			}
			if(PrefC.GetInt(PrefName.PayPlansVersion) == (int)PayPlanVersions.AgeCreditsAndDebits) {
				checkAgePayPlanCharges.Checked=true;
				checkAgePayPlanCredits.Checked=true;
			}
			else {//both Traditional and AgedCreditsOnly only age credits.
				checkAgePayPlanCharges.Checked=false;
				checkAgePayPlanCredits.Checked=true;
			}
			FillClinics();
			FillProvs();
			FillBillType();
		}

		private void FillBillType() {
			listBoxBillTypes.Items.Clear();
			List<Def> listBillTypes = Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i = 0;i < listBillTypes.Count;i++) {
				listBoxBillTypes.Items.Add(listBillTypes[i].ItemName,listBillTypes[i]);
				listBoxBillTypes.SetSelected(i,true);
			}
		}

		private void FillProvs() {
			string provAbbr;
			listBoxProvs.Items.Clear();
			List<Provider> listProvs = Providers.GetListReports();
			for(int i=0;i < listProvs.Count;i++) {
				// add text to providers that are marked as hidden or hidden in reports for the provider list
                if(listProvs[i].IsHidden || listProvs[i].IsHiddenReport) {
					provAbbr=listProvs[i].Abbr+" "+Lan.g(this,"(hidden)") ;
                }
                else {
					provAbbr=listProvs[i].Abbr;
                }
				listBoxProvs.Items.Add(provAbbr,listProvs[i]);
				listBoxProvs.SetSelected(i,true);
			}
		}

		private void FillClinics() {
			if(!PrefC.HasClinicsEnabled) {
				listBoxClins.Visible=false;
				labelClinic.Visible=false;
				checkAllClin.Visible=false;
				return;
			}
			listBoxClins.Items.Clear();
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,true);
			for(int i=0;i<listClinics.Count;i++) {
				listBoxClins.Items.Add(listClinics[i].Abbr,listClinics[i]);
				listBoxClins.SetSelected(i,true);
			}
		}

		private void FillGrid(List<AgingPat> listAgingPats) {
			gridMain.BeginUpdate();
			gridTotals.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridTotals.ListGridColumns.Clear();
			GridColumn col = new GridColumn("Patient",200,GridSortingStrategy.StringCompare);
			gridMain.ListGridColumns.Add(col);
			gridTotals.ListGridColumns.Add(col);
			col = new GridColumn("0-30 Days",100,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			gridTotals.ListGridColumns.Add(col);
			col = new GridColumn("31-60 Days",100,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			gridTotals.ListGridColumns.Add(col);
			col = new GridColumn("61-90 Days",100,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			gridTotals.ListGridColumns.Add(col);
			col = new GridColumn("> 90 Days",100,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			gridTotals.ListGridColumns.Add(col);
			col = new GridColumn("Total",100,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			gridTotals.ListGridColumns.Add(col);
			double totalZeroThirty = 0;
			double totalThirtySixty = 0;
			double totalSixtyNinety = 0;
			double totalOverNinety = 0;
			double totalBalTotal = 0;
			gridMain.ListGridRows.Clear();
			foreach(AgingPat agingPatCur in listAgingPats) {
				GridRow row = new GridRow();
				string patName = agingPatCur.Pat.LName + ", " + agingPatCur.Pat.FName;
				row.Cells.Add(patName);
				totalZeroThirty +=agingPatCur.BalZeroThirty;
				row.Cells.Add(agingPatCur.BalZeroThirty.ToString("f"));
				totalThirtySixty+=agingPatCur.BalThirtySixty;
				row.Cells.Add(agingPatCur.BalThirtySixty.ToString("f"));
				totalSixtyNinety+=agingPatCur.BalSixtyNinety;
				row.Cells.Add(agingPatCur.BalSixtyNinety.ToString("f"));
				totalOverNinety+=agingPatCur.BalOverNinety;
				row.Cells.Add(agingPatCur.BalOverNinety.ToString("f"));
				totalBalTotal+=agingPatCur.BalTotal;
				row.Cells.Add(agingPatCur.BalTotal.ToString("f"));
				row.Tag=agingPatCur;
				gridMain.ListGridRows.Add(row);
			}
			gridTotals.ListGridRows.Clear();
			gridTotals.ListGridRows.Add(new GridRow("Totals:",totalZeroThirty.ToString("f"),totalThirtySixty.ToString("f")
				,totalSixtyNinety.ToString("f"),totalOverNinety.ToString("f"),totalBalTotal.ToString("f")));
			gridMain.EndUpdate();
			gridTotals.EndUpdate();
		}

		#region Helper Methods
		private AgingOptions.AgingInclude GetAgingIncludes() {
			AgingOptions.AgingInclude agingInclude = AgingOptions.AgingInclude.None;
			if(checkAgeProcedures.Checked) {
				agingInclude |= AgingOptions.AgingInclude.ProcedureFees;
			}
			if(checkAgeAdjustments.Checked) {
				agingInclude |= AgingOptions.AgingInclude.Adjustments;
			}
			if(checkAgePayPlanCharges.Checked) {
				agingInclude |= AgingOptions.AgingInclude.PayPlanCharges;
			}
			if(checkAgePayPlanCredits.Checked) {
				agingInclude |= AgingOptions.AgingInclude.PayPlanCredits;
			}
			if(checkAgePatPayments.Checked) {
				agingInclude |= AgingOptions.AgingInclude.PatPayments;
			}
			if(checkAgeInsPayments.Checked) {
				agingInclude |= AgingOptions.AgingInclude.InsPayments;
			}
			if(checkAgeInsEsts.Checked) {
				agingInclude |= AgingOptions.AgingInclude.InsEsts;
			}
			if(checkAgeWriteoffs.Checked) {
				agingInclude |= AgingOptions.AgingInclude.Writeoffs;
			}
			if(checkAgeWriteoffEsts.Checked) {
				agingInclude |= AgingOptions.AgingInclude.WriteoffEsts;
			}
			return agingInclude;
		}

		private PPOWriteoffDateCalc GetWriteoffOptions() {
			if(radioWriteoffProcDate.Checked) {
				return PPOWriteoffDateCalc.ProcDate;
			}
			else if(radioWriteoffInsPayDate.Checked) {
				return PPOWriteoffDateCalc.InsPayDate;
			}
			else {
				return PPOWriteoffDateCalc.ClaimPayDate;
			}
		}

		private AgingOptions.FamilyGrouping GetFamilyGrouping() {
			if(radioGroupByFam.Checked) {
				return AgingOptions.FamilyGrouping.Family;
			}
			else {
				return AgingOptions.FamilyGrouping.Individual;
			}
		}

		private AgeOfAccount GetAgeOfAccount() {
			if(radio30.Checked) {
				return AgeOfAccount.Over30;
			}
			else if(radio60.Checked) {
				return AgeOfAccount.Over60;
			}
			else if(radio90.Checked) {
				return AgeOfAccount.Over90;
			}
			else {
				return AgeOfAccount.Any;
			}
		}

		private AgingOptions.NegativeBalAgingOptions GetNegativeBalOptions() {
			if(radioIncludeNeg.Checked) {
				return AgingOptions.NegativeBalAgingOptions.Include;
			}
			else if(radioShowOnlyNeg.Checked) {
				return AgingOptions.NegativeBalAgingOptions.ShowOnly;
			}
			else {
				return AgingOptions.NegativeBalAgingOptions.Exclude;
			}
		}
		#endregion

		private void butRefresh_Click(object sender,EventArgs e) {
			//refresh aging list
			//refill the grid
			_agingOptions = new AgingOptions {
				DateAsOf = PIn.Date(textDate.Text),
				AgingInc = GetAgingIncludes(),
				WriteoffOptions = GetWriteoffOptions(),
				FamGroup = GetFamilyGrouping(),
				AgeAccount = GetAgeOfAccount(),
				NegativeBalOptions = GetNegativeBalOptions(),
				ExcludeInactive = checkExcludeInactive.Checked,
				ExcludeArchive=checkExcludeArchive.Checked,
				ExcludeBadAddress = checkExcludeBadAddresses.Checked,
				//pass in null for lists to not limit by them.
				ListProvs = checkAllProv.Checked ? null : listBoxProvs.GetListSelected<Provider>(),
				ListClins = checkAllClin.Checked ? null : listBoxClins.GetListSelected<Clinic>(),
				ListBillTypes = checkAllBillType.Checked ? null : listBoxBillTypes.GetListSelected<Def>(),
				AgeCredits = checkAgeCredits.Checked,
			};
			if(_agingOptions.AgingInc == (AgingOptions.AgingInclude.None)) {
				MsgBox.Show(this,"You must select at least one transaction type to include.");
				return;
			}
			FillGrid(RpCustomAging.GetAgingList(_agingOptions));
		}

		private void listBoxBillTypes_Click(object sender,EventArgs e) {
			checkAllBillType.Checked=false;
		}

		private void listBoxProvs_Click(object sender,EventArgs e) {
			checkAllProv.Checked=false;
		}

		private void listBoxClins_Click(object sender,EventArgs e) {
			checkAllClin.Checked=false;
		}

		private void goToAccountToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			if(gridMain.SelectedGridRows.Count==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			GotoModule.GotoAccount(((AgingPat)gridMain.SelectedGridRows[0].Tag).Pat.PatNum);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"The report has no results to show. Please click 'Refresh' to populate the grid first.");
				return;
			}
			DataTable tableReportCur=new DataTable();
			tableReportCur.Columns.Add("Patient");
			tableReportCur.Columns.Add("0_30");
			tableReportCur.Columns.Add("31_60");
			tableReportCur.Columns.Add("61_90");
			tableReportCur.Columns.Add("Over90");
			tableReportCur.Columns.Add("BatTotal");
			//Uses the grid's rows so that the user's row sorting selection is retained.
			foreach(AgingPat agingPatCur in gridMain.ListGridRows.Select(x => (AgingPat)x.Tag)) {
				DataRow row = tableReportCur.NewRow();
				row["Patient"] = agingPatCur.Pat.GetNameLF();
				row["0_30"] = agingPatCur.BalZeroThirty.ToString("f");
				row["31_60"] = agingPatCur.BalThirtySixty.ToString("f");
				row["61_90"] = agingPatCur.BalSixtyNinety.ToString("f");
				row["Over90"] = agingPatCur.BalOverNinety.ToString("f");
				row["BatTotal"] = agingPatCur.BalTotal.ToString("f");
				tableReportCur.Rows.Add(row);
			}
			ReportComplex report=new ReportComplex(true,false);
			report.ReportName=Lan.g(this,"Custom Aging of Accounts Receivable");
			report.AddTitle("Custom Aging Report",Lan.g(this,"Custom Aging of Accounts Receivable"));
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("AsOf",Lan.g(this,"As of ")+_agingOptions.DateAsOf.ToShortDateString());
			List<string> listAgingInc=new List<string>();
			//Go through every aging option and for every one that is selected, add the descriptions as a subtitle
			foreach(AgingOptions.AgingInclude agingInc in Enum.GetValues(typeof(AgingOptions.AgingInclude))) {
				if(agingInc==AgingOptions.AgingInclude.None) {
					continue;
				}
				if(_agingOptions.AgingInc.HasFlag(agingInc)) {
					listAgingInc.Add(Lan.g(this,agingInc.GetDescription()));
				}
			}
			//Add a newline to the list if it's too long.
			if(listAgingInc.Count>5) {
				listAgingInc[(listAgingInc.Count+1)/2]="\r\n"+listAgingInc[(listAgingInc.Count+1)/2];
			}
			report.AddSubTitle("AgeInc",Lan.g(this,"For")+": "+string.Join(", ",listAgingInc));
			if(_agingOptions.AgeAccount==AgeOfAccount.Any) {
				report.AddSubTitle("Balance",Lan.g(this,"Any Balance"));
			}
			else if(_agingOptions.AgeAccount==AgeOfAccount.Over30) {
				report.AddSubTitle("Over30",Lan.g(this,"Over 30 Days"));
			}
			else if(_agingOptions.AgeAccount==AgeOfAccount.Over60) {
				report.AddSubTitle("Over60",Lan.g(this,"Over 60 Days"));
			}
			else if(_agingOptions.AgeAccount==AgeOfAccount.Over90) {
				report.AddSubTitle("Over90",Lan.g(this,"Over 90 Days"));
			}
			if(_agingOptions.ListBillTypes==null) {
				report.AddSubTitle("BillingTypes",Lan.g(this,"All Billing Types"));
			}
			else {
				report.AddSubTitle("BillingTypes",string.Join(", ",_agingOptions.ListBillTypes.Select(x => x.ItemName)));
			}
			if(_agingOptions.ListProvs==null) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers"));
			}
			else {
				report.AddSubTitle("Providers",string.Join(", ",_agingOptions.ListProvs.Select(x => x.Abbr)));
			}
			if(_agingOptions.ListClins==null) {
				report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
			}
			else {
				report.AddSubTitle("Clinics",string.Join(", ",_agingOptions.ListClins.Select(x => x.Abbr)));
			}
			QueryObject query=report.AddQuery(tableReportCur,"Date "+DateTime.Today.ToShortDateString());
			query.AddColumn((_agingOptions.FamGroup == AgingOptions.FamilyGrouping.Family ? "GUARANTOR" : "PATIENT"),160,FieldValueType.String);
			query.AddColumn("0-30 DAYS",75,FieldValueType.Number);
			query.AddColumn("31-60 DAYS",75,FieldValueType.Number);
			query.AddColumn("61-90 DAYS",75,FieldValueType.Number);
			query.AddColumn("> 90 DAYS",75,FieldValueType.Number);
			query.AddColumn("TOTAL",80,FieldValueType.Number);
			report.AddPageNum();
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
		}
		
		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}
	}

}