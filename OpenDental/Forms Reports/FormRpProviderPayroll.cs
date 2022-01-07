using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Data;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpProviderPayroll : FormODBase {
		private DateTime dateFrom;
		private DateTime dateTo;
		///<summary>Can be set externally when automating.</summary>
		public string DailyMonthlyAnnual;
		///<summary>If set externally, then this sets the date on startup.</summary>
		public DateTime DateStart;
		///<summary>If set externally, then this sets the date on startup.</summary>
		public DateTime DateEnd;
		private List<Clinic> _listClinics;
		private int _selectedPayPeriodIdx=-1;
		private List<Provider> _listProviders;
		private List<PayPeriod> _listPayPeriods;

		///<summary></summary>
		public FormRpProviderPayroll(bool isDetailed=false){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
			if(isDetailed) {
				radioDetailedReport.Checked=true;
			}
		}

		private void FormProduction_Load(object sender, System.EventArgs e) {
			checkAllProv.Checked=false;
			_listProviders=Providers.GetListReports();
			_listProviders.Insert(0,Providers.GetUnearnedProv());
			textToday.Text=DateTime.Today.ToShortDateString();
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				Provider prov=Providers.GetFirstOrDefault(x => x.ProvNum==Security.CurUser.ProvNum);
				if(prov!=null) {
					_listProviders=_listProviders.FindAll(x => x.FName == prov.FName && x.LName == prov.LName);
				}
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			for(int i=0;i<_listProviders.Count;i++){
				listProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(PrefC.HasClinicsEnabled){
        checkClinicInfo.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicInfo);
				checkClinicBreakdown.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicBreakdown);
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				if(!Security.CurUser.ClinicIsRestricted) {
					listClin.Items.Add(Lan.g(this,"Unassigned"));
					listClin.SetSelected(0,true);
				}
				for(int i=0;i<_listClinics.Count;i++) {
					listClin.Items.Add(_listClinics[i].Abbr);
					if(Clinics.ClinicNum==0) {
						listClin.SetSelected(listClin.Items.Count-1);
						checkAllClin.Checked=true;
					}
					if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
						listClin.SelectedIndices.Clear();
						listClin.SetSelected(listClin.Items.Count-1);
					}
				}
			}
			else {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
        checkClinicInfo.Visible=false;
				checkClinicBreakdown.Visible=false;
			}
			if(PrefC.HasClinicsEnabled) {
				checkClinicInfo.Visible=false;
				checkClinicBreakdown.Visible=false;
			}
			_listPayPeriods=PayPeriods.GetDeepCopy();
			SetDateRange();
			butThis.Text=Lan.g(this,"This Period");
			SetDates();
		}

		private void checkAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listProv.SelectedIndices.Clear();
			}
		}

		private void listProv_Click(object sender,EventArgs e) {
			if(listProv.SelectedIndices.Count>0) {
				checkAllProv.Checked=false;
			}
		}

		private void checkAllClin_Click(object sender,EventArgs e) {
			if(checkAllClin.Checked) {
				for(int i=0;i<listClin.Items.Count;i++) {
					listClin.SetSelected(i,true);
				}
			}
			else {
				listClin.SelectedIndices.Clear();
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void SetDateRange() {
			_selectedPayPeriodIdx=PayPeriods.GetForDate(DateTime.Today);
			if(_listPayPeriods.IsNullOrEmpty() || _selectedPayPeriodIdx<0 || _selectedPayPeriodIdx>=_listPayPeriods.Count) {
				dtPickerFrom.Value=DateTime.Today.AddDays(-7);
				dtPickerTo.Value=DateTime.Today;
			}
			else {
				dtPickerFrom.Value=_listPayPeriods[_selectedPayPeriodIdx].DateStart;
				dtPickerTo.Value=_listPayPeriods[_selectedPayPeriodIdx].DateStop;
			}
		}

		private void SetDates() {
			groupDateRange.Enabled=true;
			if(radioSimpleReport.Checked) {
			}
			else if(radioDetailedReport.Checked) {
				if(!PrefC.GetBool(PrefName.ProviderPayrollAllowToday)) {
					if(dtPickerTo.Value>=DateTime.Today) {
						dtPickerTo.Value=DateTime.Today.AddDays(-1);
					}
					if(dtPickerFrom.Value>=DateTime.Today) {
						dtPickerFrom.Value=DateTime.Today.AddDays(-1);
					}
				}
			}
		}

		private void dtPickerFrom_ValueChanged(object sender,EventArgs e) {
			if(radioDetailedReport.Checked 
				&& !PrefC.GetBool(PrefName.ProviderPayrollAllowToday) 
				&& dtPickerFrom.Value>=DateTime.Today) 
			{
				dtPickerFrom.Value=DateTime.Today.AddDays(-1);
			}
		}

		private void dtPickerTo_ValueChanged(object sender,EventArgs e) {
			if(radioDetailedReport.Checked 
				&& !PrefC.GetBool(PrefName.ProviderPayrollAllowToday) 
				&& dtPickerTo.Value>=DateTime.Today) 
			{
				dtPickerTo.Value=DateTime.Today.AddDays(-1);
			}
		}

		private void radioSimpleReport_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioDetailedReport_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioTransactionalHistorical_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioTransactionalToday_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void butThis_Click(object sender, System.EventArgs e) {
			SetDateRange();
			SetDates();
		}

		private void butLeft_Click(object sender, System.EventArgs e) {
			dateFrom=dtPickerFrom.Value;
			dateTo=dtPickerTo.Value;
			if(_selectedPayPeriodIdx>0) {
				_selectedPayPeriodIdx--;
				dtPickerFrom.Value=_listPayPeriods[_selectedPayPeriodIdx].DateStart;
				dtPickerTo.Value=_listPayPeriods[_selectedPayPeriodIdx].DateStop;
			}
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			dateFrom=dtPickerFrom.Value;
			dateTo=dtPickerTo.Value;
			if(_selectedPayPeriodIdx<_listPayPeriods.Count-1) {
				_selectedPayPeriodIdx++;
				dtPickerFrom.Value=_listPayPeriods[_selectedPayPeriodIdx].DateStart;
				dtPickerTo.Value=_listPayPeriods[_selectedPayPeriodIdx].DateStop;
			}
		}
    
    private void checkClinicInfo_CheckedChanged(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(checkClinicInfo.Checked) {
					checkClinicBreakdown.Visible=true;
				}
				else {
					checkClinicBreakdown.Checked=false;
					checkClinicBreakdown.Visible=false;
				}
			}
    }

		private void RunProviderPayroll() {
			ReportComplex report=new ReportComplex(true,true);
			if(checkAllProv.Checked) {
				for(int i=0;i<listProv.Items.Count;i++) {
					listProv.SetSelected(i,true);
				}
			}
			if(checkAllClin.Checked) {
				for(int i=0;i<listClin.Items.Count;i++) {
					listClin.SetSelected(i,true);
				}
			}
			dateFrom=dtPickerFrom.Value;
			dateTo=dtPickerTo.Value;
			List<Provider> listProvs=new List<Provider>();
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				listProvs.Add(_listProviders[listProv.SelectedIndices[i]]);
			}
			List<Clinic> listClinics=new List<Clinic>();
			if(PrefC.HasClinicsEnabled) {
				for(int i=0;i<listClin.SelectedIndices.Count;i++) {
					if(Security.CurUser.ClinicIsRestricted) {
						listClinics.Add(_listClinics[listClin.SelectedIndices[i]]);//we know that the list is a 1:1 to _listClinics
					}
					else {
						if(listClin.SelectedIndices[i]==0) {
							Clinic unassigned=new Clinic();
							unassigned.ClinicNum=0;
							unassigned.Abbr=Lan.g(this,"Unassigned");
							listClinics.Add(unassigned);
						}
						else {
							listClinics.Add(_listClinics[listClin.SelectedIndices[i]-1]);//Minus 1 from the selected index
						}
					}
				}
			}
			DataSet ds=RpProdInc.GetProviderPayrollDataForClinics(dateFrom,dateTo,listProvs,listClinics
				,checkAllProv.Checked,checkAllClin.Checked,radioDetailedReport.Checked);
			report.ReportName="Provider Payroll P&I";
			report.AddTitle("Title",Lan.g(this,"Provider Payroll Production and Income"));
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle));
			report.AddSubTitle("Date",dateFrom.ToShortDateString()+" - "+dateTo.ToShortDateString());
			if(checkAllProv.Checked) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers"));
			}
			else {
				string str="";
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					if(i>0) {
						str+=", ";
					}
					str+=_listProviders[listProv.SelectedIndices[i]].Abbr;
				}
				report.AddSubTitle("Providers",str);
			}
			if(PrefC.HasClinicsEnabled) {
				if(checkAllClin.Checked) {
					report.AddSubTitle("Clinics",Lan.g(this,"All Clinics"));
				}
				else {
					string clinNames="";
					for(int i=0;i<listClin.SelectedIndices.Count;i++) {
						if(i>0) {
							clinNames+=", ";
						}
						if(Security.CurUser.ClinicIsRestricted) {
							clinNames+=_listClinics[listClin.SelectedIndices[i]].Abbr;
						}
						else {
							if(listClin.SelectedIndices[i]==0) {
								clinNames+=Lan.g(this,"Unassigned");
							}
							else {
								clinNames+=_listClinics[listClin.SelectedIndices[i]-1].Abbr;//Minus 1 from the selected index
							}
						}
					}
					report.AddSubTitle("Clinics",clinNames);
				}
			}
			//setup query
			QueryObject query;
			DataTable dt=ds.Tables["Total"].Copy();
			query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
			// add columns to report
			Font font=new Font("Tahoma",8,FontStyle.Regular);
			query.AddColumn("Date",70,FieldValueType.String,font);
			if(radioDetailedReport.Checked) {
				query.AddColumn("Patient",160,FieldValueType.String,font);
			}
			else {
				query.AddColumn("Day",70,FieldValueType.String,font);
			}
			query.AddColumn("UCR Production",90,FieldValueType.Number,font);
			query.AddColumn("Est Writeoff",80,FieldValueType.Number,font);
			query.AddColumn("Prod Adj",80,FieldValueType.Number,font);
			query.AddColumn("Change in Writeoff",100,FieldValueType.Number,font);
			query.AddColumn("Net Prod(NPR)",80,FieldValueType.Number,font);
			query.AddColumn("Pat Inc Alloc",80,FieldValueType.Number,font);
			query.AddColumn("Pat Inc Unalloc",80,FieldValueType.Number,font);
			query.AddColumn("Ins Income",80,FieldValueType.Number,font);
			query.AddColumn("Ins Not Final",80,FieldValueType.Number,font);
			query.AddColumn("Net Income",80,FieldValueType.Number,font);
			report.AddPageNum();
			// execute query
			if(!report.SubmitQueries()) {//Does not actually submit queries because we use datatables in the central management tool.
				return;
			}
			// display the report
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			//DialogResult=DialogResult.OK;//Allow running multiple reports.
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!checkAllProv.Checked && listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(PrefC.HasClinicsEnabled) {
				if(!checkAllClin.Checked && listClin.SelectedIndices.Count==0) {
					MsgBox.Show(this,"At least one clinic must be selected.");
					return;
				}
			}
			dateFrom=dtPickerFrom.Value;
			dateTo=dtPickerTo.Value;
			if(dateTo<dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			if(radioSimpleReport.Checked || radioDetailedReport.Checked) {
				RunProviderPayroll();
			}
			//DialogResult=DialogResult.OK;//Stay here so that a series of similar reports can be run
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}








