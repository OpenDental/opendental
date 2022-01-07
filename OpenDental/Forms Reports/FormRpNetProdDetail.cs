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
	public partial class FormRpNetProdDetail : FormODBase {
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
		public FormRpNetProdDetail(bool isTransactionalDaily=false){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
			if(isTransactionalDaily) {
				radioTransactionalToday.Checked=true;
			}
		}

		private void FormProduction_Load(object sender, System.EventArgs e) {
			checkAllProv.Checked=false;
			_listProviders=Providers.GetListReports();
			textToday.Text=DateTime.Today.ToShortDateString();
			listProv.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(PrefC.HasClinicsEnabled){
				_listClinics=Clinics.GetForUserod(Security.CurUser);
				if(!Security.CurUser.ClinicIsRestricted) {
					listClin.Items.Add(Lan.g(this,"Unassigned"));
					listClin.SetSelected(0);
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
			}
			_listPayPeriods=PayPeriods.GetDeepCopy();
			SetDateRange();
			butThis.Text=Lan.g(this,"This Period");
			SetDates();
		}

		private void checkAllProv_Click(object sender,EventArgs e) {
			if(checkAllProv.Checked) {
				listProv.ClearSelected();
			}
		}

		private void listProv_Click(object sender,EventArgs e) {
			if(listProv.SelectedIndices.Count>0) {
				checkAllProv.Checked=false;
			}
		}

		private void checkAllClin_Click(object sender,EventArgs e) {
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			else {
				listClin.ClearSelected();
			}
		}

		private void listClin_Click(object sender,EventArgs e) {
			if(listClin.SelectedIndices.Count>0) {
				checkAllClin.Checked=false;
			}
		}

		private void SetDates() {
			groupDateRange.Enabled=true;
			if(radioTransactionalHistorical.Checked) {
			}
			else {//Transactional Today
				groupDateRange.Enabled=false;
			}
		}

		private void dtPickerFrom_ValueChanged(object sender,EventArgs e) {
		}

		private void dtPickerTo_ValueChanged(object sender,EventArgs e) {
		}

		private void radioSimpleReport_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioDetailedReport_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioTransactionalToday_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioTransactionalHistorical_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void butThis_Click(object sender, System.EventArgs e) {
			SetDateRange();
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

		private void RunNetProductionDetail() {
			ReportComplex report=new ReportComplex(true,true);
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			dateFrom=dtPickerFrom.Value;
			dateTo=dtPickerTo.Value;
			if(radioTransactionalToday.Checked) {
				dateFrom=DateTime.Today;
				dateTo=DateTime.Today;
			}
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
			string reportName="Provider Payroll Transactional Detailed";
			if(radioTransactionalToday.Checked) {
				reportName+=" Today";
			}
			report.ReportName=reportName;
			report.AddTitle("Title",Lan.g(this,"Provider Payroll Transactional Report"));
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle));
			if(radioTransactionalToday.Checked) {
				report.AddSubTitle("Date",DateTime.Today.ToShortDateString());
			}
			else {
				report.AddSubTitle("Date",dateFrom.ToShortDateString()+" - "+dateTo.ToShortDateString());
			}
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
			DataTable dt=RpProdInc.GetNetProductionDetailDataSet(dateFrom,dateTo,listProvs,listClinics
				,checkAllProv.Checked,checkAllClin.Checked,PrefC.GetBool(PrefName.NetProdDetailUseSnapshotToday));
			query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
			// add columns to report
			Font font=new Font("Tahoma",8,FontStyle.Regular);
			query.AddColumn("Type",80,FieldValueType.String,font);
			query.AddColumn("Date",70,FieldValueType.Date,font);
			query.AddColumn("Clinic",70,FieldValueType.String,font);
			query.AddColumn("PatNum",70,FieldValueType.String,font);
			query.AddColumn("Patient",70,FieldValueType.String,font);
			query.AddColumn("ProcCode",90,FieldValueType.String,font);
			query.AddColumn("Provider",80,FieldValueType.String,font);
			query.AddColumn("UCR",80,FieldValueType.Number,font);
			query.AddColumn("OrigEstWO",100,FieldValueType.Number,font);
			query.AddColumn("EstVsActualWO",80,FieldValueType.Number,font);
			query.AddColumn("Adjustment",80,FieldValueType.Number,font);
			query.AddColumn("NPR",80,FieldValueType.Number,font);
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
			if(dateFrom!=DateTime.Today && dateTo==DateTime.Today && !PrefC.GetBool(PrefName.NetProdDetailUseSnapshotToday)) {
				MsgBox.Show(this,"Cannot run this report for a date range with today's date.  "
					+"This is due to a preference in report setup for calculating writeoffs by snapshot.");
				return;
			}
			RunNetProductionDetail();
			//DialogResult=DialogResult.OK;//Stay here so that a series of similar reports can be run
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}








