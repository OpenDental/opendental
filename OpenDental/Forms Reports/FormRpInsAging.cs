using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormRpInsAging : FormODBase {
		private List<Provider> _listProviders;
		private List<Def> _listBillingTypeDefs;

		///<summary></summary>
		public FormRpInsAging(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpInsAging_Load(object sender, System.EventArgs e) {
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
			listBillType.Items.AddList(_listBillingTypeDefs,x => x.ItemName);
			listBillType.SelectedIndex=(listBillType.Items.Count>0?0:-1);
			checkBillTypesAll.Checked=true; //all billing types by default, event handler will set visibility
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			listProv.SelectedIndex=(listProv.Items.Count>0?0:-1);
			checkProvAll.Checked=true; //all provs by default, event handler will set visibility
			if(!PrefC.HasClinicsEnabled) {
				checkAllClin.Visible=false;//event handler may set listClin to visible, so hide explicitly after setting unchecked just in case
				listClin.Visible=false;
				labelClin.Visible=false;
			}
			else {
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,true,"Unassigned").ToList();
				if(!listClinics.Exists(x => x.ClinicNum==Clinics.ClinicNum)) {//Could have a hidden clinic selected
					listClinics.Add(Clinics.GetClinic(Clinics.ClinicNum));
				}
				listClin.Items.AddList(listClinics,x => x.Abbr+(x.IsHidden?(" "+Lan.g(this,"(hidden)")):""));
				listClin.SelectedIndex=listClinics.FindIndex(x => x.ClinicNum==Clinics.ClinicNum);//FindIndex could return -1, which is fine
				checkAllClin.Checked=(Clinics.ClinicNum==0);//event handler will set visibility
			}
			if(PrefC.GetBool(PrefName.FutureTransDatesAllowed) || PrefC.GetBool(PrefName.AccountAllowFutureDebits) 
				|| PrefC.GetBool(PrefName.AllowFutureInsPayments)) 
			{
				labelFutureTrans.Visible=true;//Set to false in designer
			}
			if(!checkOnlyShowPatsOutstandingClaims.Checked) {
				groupFilter.Visible=false;
			}
		}

		private void checkBillTypesAll_CheckedChanged(object sender,EventArgs e) {
			listBillType.Visible=!checkBillTypesAll.Checked;
		}

		private void checkProvAll_CheckedChanged(object sender,EventArgs e) {
			listProv.Visible=!checkProvAll.Checked;
		}

		private void checkAllClin_CheckedChanged(object sender,EventArgs e) {
			listClin.Visible=!checkAllClin.Checked;
		}

		private void listClin_Click(object sender,EventArgs e) {
			checkAllClin.Checked=false;//will not clear all selected indices, event handler will hide listClin
		}

		///<summary>Sets parameters/fills lists based on form controls.</summary>
		private RpAgingParamObject GetParamsFromForm() {
			RpAgingParamObject rpo=new RpAgingParamObject();
			rpo.AsOfDate=PIn.Date(textDate.Text);
			if(rpo.AsOfDate.Year<1880) {
				rpo.AsOfDate=DateTime.Today;
			}
			rpo.IsHistoric=(rpo.AsOfDate.Date!=DateTime.Today);
			rpo.IsGroupByFam=radioGroupByFam.Checked;
			rpo.IsInsPayWoCombined=false;
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
			if(checkOnlyShowPatsOutstandingClaims.Checked) {
				rpo.IsDetailedBreakdown=true;
			}
			rpo.GroupByCarrier=rpo.IsDetailedBreakdown;
			rpo.GroupByGroupName=rpo.IsDetailedBreakdown;
			rpo.CarrierNameFilter=textCarrier.Text;
			rpo.GroupNameFilter=textGroupName.Text;
			rpo.IsWoAged=true;
			rpo.IsIncludeNeg=true;
			rpo.IsForInsAging=true;
			rpo.IsIncludeInsNoBal=true;
			return rpo;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!checkBillTypesAll.Checked && listBillType.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one billing type must be selected.");
				return;
			}
			if(!checkProvAll.Checked && listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled && !checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one clinic must be selected.");
				return;
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			RpAgingParamObject rpo=GetParamsFromForm();
			ReportComplex report=new ReportComplex(true,true); 
			DataTable tableAging=new DataTable();
			tableAging=RpInsAging.GetInsAgingTable(rpo);
			report.ReportName=Lan.g(this,"Insurance Aging Report");
			report.AddTitle("InsAging",Lan.g(this, "Insurance Aging Report"));
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("AsOf",Lan.g(this,"As of")+" "+rpo.AsOfDate.ToShortDateString());
			if(radioAny.Checked){
				report.AddSubTitle("Balance",Lan.g(this,"Any Balance"));
			}
			else if(radio30.Checked){
				report.AddSubTitle("Over30",Lan.g(this,"Over 30 Days"));
			}
			else if(radio60.Checked){
				report.AddSubTitle("Over60",Lan.g(this,"Over 60 Days"));
			}
			else if(radio90.Checked){
				report.AddSubTitle("Over90",Lan.g(this,"Over 90 Days"));
			}
			if(checkBillTypesAll.Checked){
				report.AddSubTitle("AllBillingTypes",Lan.g(this,"All Billing Types"));
			}
			else{
				report.AddSubTitle("",string.Join(", ",listBillType.SelectedIndices.OfType<int>().Select(x => _listBillingTypeDefs[x].ItemName)));//there must be at least one selected
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
			QueryObject query=report.AddQuery(tableAging,"Date "+DateTime.Today.ToShortDateString());
			query.AddColumn((radioGroupByFam.Checked ? "Guarantor" : "Patient"),150,FieldValueType.String);
			if(rpo.IsDetailedBreakdown) {
				query.AddColumn("Carrier",220,FieldValueType.String);
				query.AddColumn("Group Name",160,FieldValueType.String);
			}
			query.AddColumn("Ins Pay\r\nEst 0-30",75,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst 31-60",75,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst 61-90",70,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst >90",75,FieldValueType.Number);
			query.AddColumn("Ins Pay\r\nEst Total", 80,FieldValueType.Number);
			if(!rpo.IsDetailedBreakdown) {
				query.AddColumn("Pat Est\r\nBal 0-30",75,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal 31-60",75,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal 61-90",70,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal >90",75,FieldValueType.Number);
				query.AddColumn("Pat Est\r\nBal Total",80,FieldValueType.Number);
				query.AddColumn("-W/O\r\nChange",70,FieldValueType.Number);
				query.AddColumn("=Pat Est\r\nAmt Due",80,FieldValueType.Number);
			}
			report.AddPageNum();
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}

		private void checkShowBreakdownOptions_CheckedChanged(object sender,EventArgs e) {
			groupFilter.Visible=checkOnlyShowPatsOutstandingClaims.Checked;
		}
	}

}
