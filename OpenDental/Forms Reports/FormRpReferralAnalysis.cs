using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.ReportingComplex;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpReferralAnalysis:FormODBase {
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRpReferralAnalysis() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReferralAnalysis_Load(object sender, System.EventArgs e) {
			checkLandscape.Visible=false;
			_listProviders=Providers.GetListReports();
			textToday.Text=DateTime.Today.ToShortDateString();
			//always defaults to the current month
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
			listProv.Items.Add(Lan.g(this,"All"));
			for(int i=0;i<_listProviders.Count;i++){
				listProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			listProv.SetSelected(0);
		}

		private void butThis_Click(object sender,EventArgs e) {
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
		}

		private void checkAddress_CheckedChanged(object sender,EventArgs e) {
			if(checkAddress.Checked) {
				checkLandscape.Visible=true;
			}
			else {
				checkLandscape.Visible=false;
			}
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day){
				toLastDay=true;
			}
			textDateFrom.Text=dateFrom.AddMonths(-1).ToShortDateString();
			textDateTo.Text=dateTo.AddMonths(-1).ToShortDateString();
			dateTo=PIn.Date(textDateTo.Text);
			if(toLastDay){
				textDateTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToShortDateString();
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day){
				toLastDay=true;
			}
			textDateFrom.Text=dateFrom.AddMonths(1).ToShortDateString();
			textDateTo.Text=dateTo.AddMonths(1).ToShortDateString();
			dateTo=PIn.Date(textDateTo.Text);
			if(toLastDay){
				textDateTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToShortDateString();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(listProv.SelectedIndices.Count==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(listProv.SelectedIndices[0]==0 && listProv.SelectedIndices.Count>1) {
				MsgBox.Show(this,"You cannot select 'All' providers as well as specific providers.");
				return;
			}
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			if(dateTo<dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			List<long> listProvNums=new List<long>();
			List<string> listProvNames=new List<string>();
			if(listProv.SelectedIndices[0]==0) {
				listProv.ClearSelected();
				for(int i=1;i<listProv.Items.Count;i++) {//Start i at 1 due to the 'All' option.
					listProv.SetSelected(i);
				}
			}
			for(int i=0;i<listProv.SelectedIndices.Count;i++) {
				//Minus 1 due to the 'All' option.
				listProvNums.Add(_listProviders[listProv.SelectedIndices[i]-1].ProvNum);
				listProvNames.Add(_listProviders[listProv.SelectedIndices[i]-1].Abbr);
			}
			ReportComplex report;
			if(checkLandscape.Checked) {
				report=new ReportComplex(true,true);
			}
			else {
				report=new ReportComplex(true,false);
			}
			DataTable table=RpReferralAnalysis.GetReferralTable(dateFrom,dateTo,listProvNums,checkAddress.Checked,checkNewPat.Checked);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Referral Analysis");
			report.AddTitle("Title",Lan.g(this,"Referral Analysis"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",dateFrom.ToString("d")+" - "+dateTo.ToString("d"),fontSubTitle);
			if(listProv.SelectedIndices[0]==0){
				report.AddSubTitle("Provider Subtitle",Lan.g(this,"All Providers"));
			}
			else if(listProv.SelectedIndices.Count==1) {
				report.AddSubTitle("Provider SubTitle",Lan.g(this,"Prov:")+" "+_listProviders[listProv.SelectedIndices[0]-1].GetLongDesc());
			}
			else {
				report.AddSubTitle("Provider SubTitle",string.Join(", ",listProvNames));
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Last Name",100,FieldValueType.String);
			query.AddColumn("First Name",100,FieldValueType.String);
			query.AddColumn("Count",40,FieldValueType.Integer);
			query.AddColumn("Production",75,FieldValueType.Number);
			query.GetColumnDetail("Production").ContentAlignment=ContentAlignment.MiddleRight;
			if(checkAddress.Checked){
				query.AddColumn("Title",40);
				if(checkLandscape.Checked) {
					query.AddColumn("Address",160,FieldValueType.String);
					query.AddColumn("Add2",140,FieldValueType.String);
					query.AddColumn("City",90,FieldValueType.String);
				}
				else {
					query.AddColumn("Address",140,FieldValueType.String);
					query.AddColumn("Add2",80,FieldValueType.String);
					query.AddColumn("City",70,FieldValueType.String);
				}
				query.AddColumn("State",40,FieldValueType.String);
				query.AddColumn("Zip",70,FieldValueType.String);
				if(checkLandscape.Checked) {
					query.AddColumn("Specialty",90,FieldValueType.String);
				}
				else {
					query.AddColumn("Specialty",60,FieldValueType.String);
				}
			}
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
