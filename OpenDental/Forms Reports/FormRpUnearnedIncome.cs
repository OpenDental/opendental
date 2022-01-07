using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpUnearnedIncome:FormODBase {


		public FormRpUnearnedIncome() {
			InitializeComponent();
			InitializeLayoutManager();
		}
		
		private void FormRpUnearnedIncome_Load(object sender,System.EventArgs e) {
			checkUnearnedAllocationShowProv.Checked=PrefC.GetBool(PrefName.AllowPrepayProvider);
			checkLineItemShowProv.Checked=PrefC.GetBool(PrefName.AllowPrepayProvider);
			FillClinics();
			FillProviders();
			FillUnearnedTypes();
			FillDates();
		}

		#region Fill Methods
		private void FillClinics() {
			if(PrefC.HasClinicsEnabled) {//fill clinic list
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,true,Lan.g(this,"Unassigned"));
				foreach(Clinic clinCur in listClinics) {
					listUnearnedAllocationClins.Items.Add(clinCur.Abbr,clinCur);
					listNetUnearnedClins.Items.Add(clinCur.Abbr,clinCur);
					listLineItemClins.Items.Add(clinCur.Abbr,clinCur);
					listUnearnedAcctClins.Items.Add(clinCur.Abbr,clinCur);
					if(clinCur.ClinicNum == Clinics.ClinicNum) {
						listUnearnedAllocationClins.SetSelectedKey<Clinic>(clinCur.ClinicNum,x => x.ClinicNum);
						listNetUnearnedClins.SetSelectedKey<Clinic>(clinCur.ClinicNum,x => x.ClinicNum);
						listLineItemClins.SetSelectedKey<Clinic>(clinCur.ClinicNum,x => x.ClinicNum);
						listUnearnedAcctClins.SetSelectedKey<Clinic>(clinCur.ClinicNum,x => x.ClinicNum);;
					}
				}
				if(Clinics.ClinicNum==0) {
					checkUnearnedAllocationAllClins.Checked=true;
					checkNetUnearnedAllClins.Checked=true;
					checkLineItemAllClins.Checked=true;
					checkUnearnedAcctAllClins.Checked=true;
				}
			}
			else {//hide label,list,and check box if clinics are not enabled
				listUnearnedAllocationClins.Visible=false;
				labelUnearnedAllocationClins.Visible=false;
				checkUnearnedAllocationAllClins.Visible=false;
				listNetUnearnedClins.Visible=false;
				labelNetUnearnedClins.Visible=false;
				checkNetUnearnedAllClins.Visible=false;
				listLineItemClins.Visible=false;
				labelLineItemClins.Visible=false;
				checkLineItemAllClins.Visible=false;
				listUnearnedAcctClins.Visible=false;
				labelUnearnedAcctClins.Visible=false;
				checkUnearnedAcctAllClins.Visible=false;
			}
		}

		private void FillProviders() {
			List<Provider> listProvs = Providers.GetListReports();
			foreach(Provider provCur in listProvs) {
				listUnearnedAllocationProvs.Items.Add(provCur.Abbr,provCur);
				listNetUnearnedProvs.Items.Add(provCur.Abbr,provCur);
			}
			checkUnearnedAllocationAllProvs.Checked=true;
			checkNetUnearnedAllProvs.Checked=true;
		}

		private void FillUnearnedTypes() {
			List<Def> listDefs = Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true);
			foreach(Def defCur in listDefs) {
				listUnearnedAllocationTypes.Items.Add(defCur.ItemName,defCur);
				listNetUnearnedTypes.Items.Add(defCur.ItemName,defCur);
			}
			checkUnearnedAllocationAllTypes.Checked=true;
			checkNetUnearnedAllTypes.Checked=true;
		}

		private void FillDates() {
			DateTime dateThisFirst = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
			dateLineItemFrom.SelectionStart=dateThisFirst.AddMonths(-1);
			dateLineItemTo.SelectionStart=dateThisFirst.AddDays(-1);
		}
		#endregion

		#region Unearned Allocation

		private void checkUnearnedAllocationAllProvs_CheckedChanged(object sender,EventArgs e) {
			if(checkUnearnedAllocationAllProvs.Checked) {
				listUnearnedAllocationProvs.SelectedIndices.Clear();
			}
		}

		private void checkUnearnedAllocationAllTypes_CheckedChanged(object sender,EventArgs e) {
			if(checkUnearnedAllocationAllTypes.Checked) {
				listUnearnedAllocationTypes.SelectedIndices.Clear();
			}
		}

		private void checkUnearnedAllocationAllClins_CheckedChanged(object sender,EventArgs e) {
			if(checkUnearnedAllocationAllClins.Checked) {
				listUnearnedAllocationClins.SelectedIndices.Clear();
			}
		}

		private void listUnearnedAllocationTypes_Click(object sender,EventArgs e) {
			if(listUnearnedAllocationTypes.SelectedIndices.Count>0) {
				checkUnearnedAllocationAllTypes.Checked=false;
			}
		}

		private void listUnearnedAllocationProvs_Click(object sender,EventArgs e) {
			if(listUnearnedAllocationProvs.SelectedIndices.Count>0) {
				checkUnearnedAllocationAllProvs.Checked=false;
			}
		}

		private void listUnearnedAllocationClins_Click(object sender,EventArgs e) {
			if(listUnearnedAllocationClins.SelectedIndices.Count>0) {
				checkUnearnedAllocationAllClins.Checked=false;
			}
		}

		private void butUnearnedAllocationOK_Click(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(!checkUnearnedAllocationAllClins.Checked && listUnearnedAllocationClins.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			if(!checkUnearnedAllocationAllProvs.Checked && listUnearnedAllocationProvs.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(!checkUnearnedAllocationAllTypes.Checked && listUnearnedAllocationTypes.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one unearned type must be selected.");
				return;
			}
			List<long> listClinicNums = listUnearnedAllocationClins.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			if(checkUnearnedAllocationAllClins.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
				AddHiddenClinicNums(ref listClinicNums);
			}
			List<long> listProvNums = listUnearnedAllocationProvs.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			List<long> listUnearnedTypeNums = listUnearnedAllocationTypes.GetListSelected<Def>().Select(x => x.DefNum).ToList();
			ReportComplex report = new ReportComplex(true,true);
			DataTable table = RpUnearnedIncome.GetUnearnedAllocationData(listClinicNums,listProvNums,listUnearnedTypeNums,
				checkUnearnedAllocationExcludeZero.Checked,checkUnearnedAllocationShowProv.Checked);
			report.ReportName="Unearned Allocation Report";
			QueryObject query=report.AddQuery(table,"Unearned Allocation Report");
			query.AddColumn("Guarantor",150,FieldValueType.String);
			query.GetColumnDetail("Guarantor").Font=new Font(query.GetColumnDetail("Guarantor").Font,FontStyle.Bold);
			query.AddColumn("FamBal",100,FieldValueType.String);
			query.GetColumnDetail("FamBal").Font=new Font(query.GetColumnDetail("FamBal").Font,FontStyle.Bold);
			query.AddColumn("FamUnearned",100,FieldValueType.String);
			query.GetColumnDetail("FamUnearned").Font=new Font(query.GetColumnDetail("FamUnearned").Font,FontStyle.Bold);
			query.AddColumn("FamRemAmt",100,FieldValueType.String);
			query.GetColumnDetail("FamRemAmt").Font=new Font(query.GetColumnDetail("FamRemAmt").Font,FontStyle.Bold);
			if(checkUnearnedAllocationShowProv.Checked) {
				query.AddColumn("Prov",50,FieldValueType.String);
				query.GetColumnDetail("Prov").Font=new Font(query.GetColumnDetail("Prov").Font,FontStyle.Bold);
			}
			query.AddColumn("Patient",150,FieldValueType.String);
			query.AddColumn("Code",50,FieldValueType.String);
			query.AddColumn("Date",80,FieldValueType.String);
			query.AddColumn("Fee",100,FieldValueType.String);
			query.AddColumn("RemAmt",100,FieldValueType.String);
			report.AddTitle("Title","Unearned Allocation Report");
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle));
			if(checkUnearnedAllocationAllTypes.Checked) {
				report.AddSubTitle("UnearnedTypes","All Unearned Types");
			}
			else {
				string unearnedTypes = string.Join(", ",listUnearnedAllocationTypes.GetListSelected<Def>().Select(x => x.ItemName));
				report.AddSubTitle("UnearnedTypes",unearnedTypes);
			}
			if(checkUnearnedAllocationAllProvs.Checked) {
				report.AddSubTitle("Provs",Lan.g(this,"All Providers"));
			}
			else {
				string provNames = string.Join(", ",listUnearnedAllocationProvs.GetListSelected<Provider>().Select(x => x.Abbr));
				report.AddSubTitle("ProvNames",provNames);
			}
			if(PrefC.HasClinicsEnabled) {//show sub titles if clinics are enabled. 
				if(checkUnearnedAllocationAllClins.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames = string.Join(", ",listUnearnedAllocationClins.GetListSelected<Clinic>().Select(x => x.Abbr));
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			report.AddPageNum();
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			//Display report
			using FormReportComplex FormRC = new FormReportComplex(report);
			FormRC.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		#endregion

		#region Net Unearned Income
		private void checkNetUnearnedAllTypes_CheckedChanged(object sender,EventArgs e) {
			if(checkNetUnearnedAllTypes.Checked) {
				listNetUnearnedTypes.SelectedIndices.Clear();
			}
		}

		private void checkNetUnearnedAllProvs_CheckedChanged(object sender,EventArgs e) {
			if(checkNetUnearnedAllProvs.Checked) {
				listNetUnearnedProvs.SelectedIndices.Clear();
			}
		}

		private void checkNetUnearnedAllClins_CheckedChanged(object sender,EventArgs e) {
			if(checkNetUnearnedAllClins.Checked) {
				listNetUnearnedClins.SelectedIndices.Clear();
			}
		}

		private void listNetUnearnedTypes_Click(object sender,EventArgs e) {
			if(listNetUnearnedTypes.SelectedIndices.Count>0) {
				checkNetUnearnedAllTypes.Checked=false;
			}
		}

		private void listNetUnearnedProvs_Click(object sender,EventArgs e) {
			if(listNetUnearnedProvs.SelectedIndices.Count>0) {
				checkNetUnearnedAllProvs.Checked=false;
			}
		}

		private void listNetUnearnedClins_Click(object sender,EventArgs e) {
			if(listNetUnearnedClins.SelectedIndices.Count>0) {
				checkNetUnearnedAllClins.Checked=false;
			}
		}

		private void butNetUnearnedOK_Click(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(!checkNetUnearnedAllClins.Checked && listNetUnearnedClins.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			if(!checkNetUnearnedAllProvs.Checked && listNetUnearnedProvs.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(!checkNetUnearnedAllTypes.Checked && listNetUnearnedTypes.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one unearned type must be selected.");
				return;
			}
			List<long> listClinicNums = listNetUnearnedClins.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			if(checkNetUnearnedAllClins.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
				AddHiddenClinicNums(ref listClinicNums);
			}
			List<long> listProvNums = listNetUnearnedProvs.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			List<long> listUnearnedTypeNums = listNetUnearnedTypes.GetListSelected<Def>().Select(x => x.DefNum).ToList();
			ReportComplex report = new ReportComplex(true,false);
			DataTable table = RpUnearnedIncome.GetNetUnearnedData(listClinicNums,listProvNums,listUnearnedTypeNums,checkNetUnearnedExcludeZero.Checked);
			report.ReportName="Net Unearned Income Report";
			QueryObject query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			query.AddColumn("Patient",170,FieldValueType.String);
			query.AddColumn("Guarantor",170,FieldValueType.String);
			query.AddColumn("Unearned Amt",100,FieldValueType.Number);
			query.AddColumn("Fam Bal",100,FieldValueType.String);
			report.AddTitle("Title","Net Unearned Income");
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle));
			if(checkNetUnearnedAllTypes.Checked) {
				report.AddSubTitle("UnearnedTypes","All Unearned Types");
			}
			else {
				string unearnedTypes = string.Join(", ",listNetUnearnedTypes.GetListSelected<Def>().Select(x => x.ItemName).ToList());
				report.AddSubTitle("UnearnedTypes",unearnedTypes);
			}
			if(checkNetUnearnedAllProvs.Checked) {
				report.AddSubTitle("Provs",Lan.g(this,"All Providers"));
			}
			else {
				string provNames = string.Join(", ",listNetUnearnedProvs.GetListSelected<Provider>().Select(x => x.Abbr).ToList());
				report.AddSubTitle("ProvNames",provNames);
			}
			if(PrefC.HasClinicsEnabled) {//show sub titles if clinics are enabled. 
				if(checkNetUnearnedAllClins.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames = string.Join(", ",listNetUnearnedClins.GetListSelected<Clinic>().Select(x => x.Abbr));
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			report.AddPageNum();
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			//Display report
			using FormReportComplex FormRC = new FormReportComplex(report);
			FormRC.ShowDialog();
			DialogResult=DialogResult.OK;
		}
		#endregion

		#region Line Item Unearned Income
		private void checkLineItemAllClins_CheckedChanged(object sender,EventArgs e) {
			if(checkLineItemAllClins.Checked) {
				listLineItemClins.SelectedIndices.Clear();
			}
		}

		private void listLineItemClins_Click(object sender,EventArgs e) {
			if(listLineItemClins.SelectedIndices.Count>0) {
				checkLineItemAllClins.Checked=false;
			}
		}

		private void butLineItemOK_Click(object sender,EventArgs e) {
			if(dateLineItemTo.SelectionStart<dateLineItemFrom.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {
				if(!checkLineItemAllClins.Checked && listLineItemClins.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			List<long> listClinicNums = new List<long>(); //stores clinicNums of the selected indices
			listClinicNums.AddRange(listLineItemClins.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList());//empty if "All" is checked.
			if(checkLineItemAllClins.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
				AddHiddenClinicNums(ref listClinicNums);
			}
			ReportComplex report = new ReportComplex(true,false);
			DataTable table = RpUnearnedIncome.GetLineItemUnearnedData(listClinicNums,dateLineItemFrom.SelectionStart,dateLineItemTo.SelectionStart,
				checkLineItemShowProv.Checked);
			report.ReportName="Line Item Unearned Income Report";
			report.AddTitle("Title","Line Item Unearned Income Activity");
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle));
			string dateRange = dateLineItemFrom.SelectionStart.ToShortDateString()+" - "+dateLineItemTo.SelectionStart.ToShortDateString();
			report.AddSubTitle("Date",dateRange);			
			if(PrefC.HasClinicsEnabled) {//show sub titles if clinics are enabled. 
				if(checkLineItemAllClins.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames = string.Join(",",listLineItemClins.GetListSelected<Clinic>().Select(x => x.Abbr));
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			QueryObject query;
			if(PrefC.HasClinicsEnabled) {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
				query.AddColumn("Date",100,FieldValueType.Date);
				query.AddColumn("Patient",180,FieldValueType.String);
				query.AddColumn("Type",120,FieldValueType.String);
				query.AddColumn("Clinic",80,FieldValueType.String);
				if(checkLineItemShowProv.Checked) {
					query.AddColumn("Prov",50,FieldValueType.String);
				}
				query.AddColumn("Amount",100,FieldValueType.Number);
			}
			else {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
				query.AddColumn("Date",100,FieldValueType.String);
				query.AddColumn("Patient",260,FieldValueType.String);
				query.AddColumn("Type",120,FieldValueType.String);
				if(checkLineItemShowProv.Checked) {
					query.AddColumn("Prov",50,FieldValueType.String);
				}
				query.AddColumn("Amount",100,FieldValueType.Number);
			}
			report.AddPageNum();
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			//Display report
			using FormReportComplex FormRC = new FormReportComplex(report);
			FormRC.ShowDialog();
			DialogResult=DialogResult.OK;
		}
		#endregion

		#region Unearned Accounts
		private void checkUnearnedAcctAllClins_CheckedChanged(object sender,EventArgs e) {
			if(checkUnearnedAcctAllClins.Checked) {
				listUnearnedAcctClins.SelectedIndices.Clear();
			}
		}

		private void listUnearnedAcctClins_Click(object sender,EventArgs e) {
			if(listUnearnedAcctClins.SelectedIndices.Count>0) {
				checkUnearnedAcctAllClins.Checked=false;
			}
		}

		private void butUnearnedAcctOk_Click(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(!checkUnearnedAcctAllClins.Checked && listUnearnedAcctClins.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			List<long> listClinicNums = new List<long>(); //stores clinicNums of the selected indices
			listClinicNums.AddRange(listUnearnedAcctClins.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList());
			if(checkUnearnedAcctAllClins.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
				AddHiddenClinicNums(ref listClinicNums);
			}
			ReportComplex report = new ReportComplex(true,false);
			DataTable table = RpUnearnedIncome.GetUnearnedAccountData(listClinicNums);
			report.ReportName="Unearned Accounts Report";
			report.AddTitle("Title","Unearned Accounts");
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle));
			if(PrefC.HasClinicsEnabled) {//show sub titles if clinics are enabled. 
				if(checkUnearnedAcctAllClins.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames = string.Join(",",listUnearnedAcctClins.GetListSelected<Clinic>().Select(x => x.Abbr));
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			QueryObject query;
			if(PrefC.HasClinicsEnabled) {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
				query.AddColumn("Guarantor",280,FieldValueType.String);
				query.AddColumn("Type",120,FieldValueType.String);
				query.AddColumn("Clinic",80,FieldValueType.String);
				query.AddColumn("Amount",100,FieldValueType.Number);
			}
			else {
				query=report.AddQuery(table,"","",SplitByKind.None,1,true);
				query.AddColumn("Guarantor",360,FieldValueType.String);
				query.AddColumn("Type",120,FieldValueType.String);
				query.AddColumn("Amount",100,FieldValueType.Number);
			}
			report.AddPageNum();
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			//Display report
			using FormReportComplex FormRC = new FormReportComplex(report);
			FormRC.ShowDialog();
			DialogResult=DialogResult.OK;
		}
		#endregion Unearned Accounts

		#region General Form Methods

		///<summary>If the "All" option is selected for a Clinics list, add hidden (but not restricted) clinics to the report</summary>
		private void AddHiddenClinicNums(ref List<long> listClinicNums) {
			listClinicNums=listClinicNums.Union(Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum)).ToList();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion
	}
}