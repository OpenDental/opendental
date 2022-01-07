using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormRpAging : FormODBase {
		private List<Provider> _listProviders;
		private List<Def> _listBillingTypeDefs;

		///<summary></summary>
		public FormRpAging(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAging_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			DateTime lastAgingDate=PrefC.GetDate(PrefName.DateLastAging);
			if(lastAgingDate.Year<1880) {
				textDate.Text="";
			}
			else if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)){
				textDate.Text=lastAgingDate.ToShortDateString();
			}
			else{
				textDate.Text=DateTime.Today.ToShortDateString();
			}
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i=0;i<_listBillingTypeDefs.Count;i++){
				listBillType.Items.Add(_listBillingTypeDefs[i].ItemName);
			}
			if(listBillType.Items.Count>0){
				listBillType.SelectedIndex=0;
			}
			listBillType.Visible=false;
			checkBillTypesAll.Checked=true;
			for(int i=0;i<_listProviders.Count;i++){
				listProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(listProv.Items.Count>0) {
				listProv.SelectedIndex=0;
			}
			checkProvAll.Checked=true;
			listProv.Visible=false;
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
			}
			else {
				List<Clinic> listClinics = Clinics.GetForUserod(Security.CurUser,true,"Unassigned").ToList();
				if(!listClinics.Exists(x => x.ClinicNum==Clinics.ClinicNum)) {//Could have a hidden clinic selected
					listClinics.Add(Clinics.GetClinic(Clinics.ClinicNum));
				}
				listClin.Items.AddList<Clinic>(listClinics,x => x.Abbr+(x.IsHidden?(" "+Lan.g(this,"(hidden)")):""));
				listClin.SelectedIndex=listClinics.FindIndex(x => x.ClinicNum==Clinics.ClinicNum);//FindIndex could return -1, which is fine
				if(Clinics.ClinicNum==0) {
					checkAllClin.Checked=true;
					listClin.Visible=false;
				}
			}
			if(PrefC.GetBool(PrefName.AgingReportShowAgePatPayplanPayments)) {
				//Visibility set to false in designer, only set to visible here.  No UI for pref, only set true via query for specific customer.
				checkAgePatPayPlanPayments.Visible=true;
			}
			if(PrefC.GetBool(PrefName.FutureTransDatesAllowed) || PrefC.GetBool(PrefName.AccountAllowFutureDebits) 
				|| PrefC.GetBool(PrefName.AllowFutureInsPayments)) 
			{
				labelFutureTrans.Visible=true;//Set to false in designer
			}
		}

		private void checkBillTypesAll_Click(object sender,EventArgs e) {
			if(checkBillTypesAll.Checked){
				listBillType.Visible=false;
			}
			else{
				listBillType.Visible=true;
			}
		}

		private void checkProvAll_Click(object sender,EventArgs e) {
			if(checkProvAll.Checked) {
				listProv.Visible=false;
			}
			else {
				listProv.Visible=true;
			}
		}

		private void checkIncludeNeg_Click(object sender, System.EventArgs e) {
			if(!checkIncludeNeg.Checked){
				checkOnlyNeg.Checked=false;
			}
		}

		private void checkOnlyNeg_Click(object sender, System.EventArgs e) {
			if(checkOnlyNeg.Checked){
				checkIncludeNeg.Checked=true;
			}
		}

		private void checkIncludeInsNoBal_Click(object sender,EventArgs e) {
			if(!checkIncludeInsNoBal.Checked) {
				checkOnlyInsNoBal.Checked=false;
			}
		}

		private void checkOnlyInsNoBal_Click(object sender,EventArgs e) {
			if(checkOnlyInsNoBal.Checked) {
				checkIncludeInsNoBal.Checked=true;
			}
		}

		private void checkAllClin_Click(object sender,EventArgs e) {
			if(checkAllClin.Checked) {
				listClin.Visible=false;
			}
			else {
				listClin.Visible=true;
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			checkAllClin.Checked=false;//will not clear all selected indices
		}

		private bool Validation() {
			if(!checkBillTypesAll.Checked && listBillType.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one billing type must be selected.");
				return false;
			}
			if(!checkProvAll.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return false;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return false;
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Invalid date.");
				return false;
			}
			return true;
		}

		///<summary>Sets parameters/fills lists based on form controls.</summary>
		private RpAgingParamObject GetParamsFromForm() {
			RpAgingParamObject rpo=new RpAgingParamObject();
			rpo.AsOfDate=PIn.Date(textDate.Text);
			if(rpo.AsOfDate.Year<1880) {
				rpo.AsOfDate=DateTime.Today;
			}
			rpo.IsHistoric=(rpo.AsOfDate.Date!=DateTime.Today);
			rpo.IsWoAged=checkAgeWriteoffs.Checked;
			rpo.HasDateLastPay=checkHasDateLastPay.Checked;
			rpo.IsGroupByFam=radioGroupByFam.Checked;
			rpo.IsOnlyNeg=checkOnlyNeg.Checked;
			rpo.IsIncludeNeg=checkIncludeNeg.Checked;
			rpo.IsExcludeInactive=checkExcludeInactive.Checked;
			rpo.IsExcludeBadAddress=checkBadAddress.Checked;
			rpo.IsExcludeArchive=checkExcludeArchive.Checked;
			rpo.IsIncludeInsNoBal=checkIncludeInsNoBal.Checked;
			rpo.IsOnlyInsNoBal=checkOnlyInsNoBal.Checked;
			rpo.DoAgePatPayPlanPayments=checkAgePatPayPlanPayments.Checked;
			rpo.IsInsPayWoCombined=false;
			rpo.doExcludeIncomeTransfers=checkBoxExcludeIncomeTransfers.Checked;
			if(!checkBillTypesAll.Checked) {
				rpo.ListBillTypes=listBillType.SelectedIndices.OfType<int>().Select(x => _listBillingTypeDefs[x].DefNum).ToList();
			}
			if(!checkProvAll.Checked) {
				rpo.ListProvNums=listProv.SelectedIndices.OfType<int>().Select(x => _listProviders[x].ProvNum).ToList();
			}
			if(PrefC.HasClinicsEnabled) {
				//if "All" is selected and the user is not restricted, show ALL clinics, including the 0 clinic.
				if(checkAllClin.Checked && !Security.CurUser.ClinicIsRestricted){
					rpo.ListClinicNums.Clear();
					rpo.ListClinicNums.Add(0);
					rpo.ListClinicNums.AddRange(Clinics.GetDeepCopy().Select(x => x.ClinicNum));
				}
				else {
					rpo.ListClinicNums=listClin.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
				}
			}
			rpo.AccountAge=AgeOfAccount.Any;
			if(radio30.Checked) {
				rpo.AccountAge=AgeOfAccount.Over30;
			}
			else if(radio60.Checked) {
				rpo.AccountAge=AgeOfAccount.Over60;
			}
			else if(radio90.Checked) {
				rpo.AccountAge=AgeOfAccount.Over90;
			}
			return rpo;
		}

		private void butGenerateQuery_Click(object sender,EventArgs e) {
			if(!Validation()) {
				return;
			}
			string queryStr=RpAging.GetQueryString(GetParamsFromForm());
			MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(queryStr);
			msgBox.Show(this);
		}

		private void checkBoxExcludeIncomeTransfers_CheckedChanged(object sender,EventArgs e) {
			if(checkBoxExcludeIncomeTransfers.Checked) {
				try {
					Version versionMySQL = new Version(MiscData.GetMySqlVersion());
					if(versionMySQL <= new Version("5.5.62")) { // highest version possible of MySQL 5.5
						MsgBox.Show(this,"This option has been disabled for MySQL 5.5. Call support for assistance in enabling this feature.");
						checkBoxExcludeIncomeTransfers.Enabled = false;
						checkBoxExcludeIncomeTransfers.Checked = false;
					}
				}
				catch(Exception ex) {
					//failed to get version, just proceed as if the version is OK
					ex.DoNothing();
				}
			}
			labelAgingExcludeTransfers.Visible=checkBoxExcludeIncomeTransfers.Checked;
		}

		private void radioGroupByFam_CheckedChanged(object sender,EventArgs e) {
			checkBoxExcludeIncomeTransfers.Visible=radioGroupByFam.Checked;
			if(!radioGroupByFam.Checked) {
				checkBoxExcludeIncomeTransfers.Checked=false;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!Validation()) {
				return;
			}
			ReportComplex report=new ReportComplex(true,false); 
			DataTable tableAging=RpAging.GetAgingTable(GetParamsFromForm());
			report.IsLandscape=checkHasDateLastPay.Checked;
			report.ReportName=Lan.g(this,"AGING OF ACCOUNTS RECEIVABLE REPORT");
			report.AddTitle("Aging Report",Lan.g(this,"AGING OF ACCOUNTS RECEIVABLE"));
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("AsOf",Lan.g(this,"As of ")+textDate.Text);
			if(radioAny.Checked){
				report.AddSubTitle("Balance",Lan.g(this,"Any Balance"));
			}
			if(radio30.Checked){
				report.AddSubTitle("Over30",Lan.g(this,"Over 30 Days"));
			}
			if(radio60.Checked){
				report.AddSubTitle("Over60",Lan.g(this,"Over 60 Days"));
			}
			if(radio90.Checked){
				report.AddSubTitle("Over90",Lan.g(this,"Over 90 Days"));
			}
			if(checkBillTypesAll.Checked){
				report.AddSubTitle("AllBillingTypes",Lan.g(this,"All Billing Types"));
			}
			else{
				report.AddSubTitle("",string.Join(", ",listBillType.SelectedIndices.OfType<int>().Select(x => _listBillingTypeDefs[x].ItemName)));
			}
			if(checkProvAll.Checked) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers"));
			}
			else {
				report.AddSubTitle("Providers",string.Join(", ",listProv.SelectedIndices.OfType<int>().Select(x => _listProviders[x].Abbr)));
			}
			if(checkAllClin.Checked) {
				report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
			}
			else {
				report.AddSubTitle("Clinics",string.Join(", ",listClin.GetListSelected<Clinic>().Select(x => x.Abbr)));
			}
			//Patient Account Aging Query-----------------------------------------------
			bool isWoEstIncluded=true;
			if(checkAgeWriteoffs.Checked && tableAging.Select().All(x => Math.Abs(PIn.Double(x["InsWoEst"].ToString()))<=0.005)) {
				tableAging.Columns.Remove("InsWoEst");
				isWoEstIncluded=false;
			}
			QueryObject query=report.AddQuery(tableAging,"Date "+DateTime.Today.ToShortDateString());
			query.AddColumn((radioGroupByFam.Checked?"Guarantor":"Patient"),(checkAgeWriteoffs.Checked?135:140),FieldValueType.String);
			query.AddColumn("0-30 Days",80,FieldValueType.Number);
			query.AddColumn("31-60 Days",80,FieldValueType.Number);
			query.AddColumn("61-90 Days",80,FieldValueType.Number);
			query.AddColumn("> 90 Days",80,FieldValueType.Number);
			query.AddColumn("Total",80,FieldValueType.Number);
			if(isWoEstIncluded) {
				query.AddColumn("-W/O "+(checkAgeWriteoffs.Checked?"Change":"Est"),(checkAgeWriteoffs.Checked?85:80),FieldValueType.Number);
			}
			query.AddColumn("-Ins Est",80,FieldValueType.Number);
			query.AddColumn("=Patient",80,FieldValueType.Number);
			if(checkHasDateLastPay.Checked) {
				query.AddColumn("",10);//add some space between the right alligned amounts and the left alligned date
				query.AddColumn("Last Pay Date",90,FieldValueType.Date);
			}
			report.AddPageNum();
			report.AddGridLines();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

	}



}
