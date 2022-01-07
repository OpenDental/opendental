using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpProdGoal : FormODBase {
		private DateTime _dateFrom;
		private DateTime _dateTo;
		private List<Clinic> _listClinics;
		///<summary>Includes hidden and hidden on reports providers.
		///This is used instead of Providers.GetListReports() because we need the full list of providers when running All Providers
		///This is also so we can show provider specific information in the report.
		///Includes providers that share the same name as the provider currently logged if user has the ReportProdIncAllProviders permission.</summary>
		private List<Provider> _listProviders;
		///<summary>Includes hidden providers, excludes hidden on reports providers.</summary>
		///This list directly resembles all providers that are showing within the providers list box that is showing to the user.</summary>
		private List<Provider> _listFilteredProviders;

		///<summary></summary>
		public FormRpProdGoal(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpProdGoal_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			_listFilteredProviders=new List<Provider>();
			textToday.Text=DateTime.Today.ToShortDateString();
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				//They either have permission or have a provider at this point.  If they don't have permission they must have a provider.
				_listProviders=_listProviders.FindAll(x => x.ProvNum==Security.CurUser.ProvNum);
				Provider prov=_listProviders.FirstOrDefault();
				if(prov!=null) {
					_listProviders.AddRange(Providers.GetWhere(x => x.FName == prov.FName && x.LName == prov.LName && x.ProvNum != prov.ProvNum));
				}
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			//Fill the short list of providers, ignoring those marked "hidden on reports"
			for(int i=0;i<_listProviders.Count;i++) {
				if(_listProviders[i].IsHiddenReport) {
					continue;
				}
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				_listFilteredProviders.Add(_listProviders[i].Copy());
			}
			//If the user is not allowed to run the report for all providers, default the selection to the first in the list box.
			if(checkAllProv.Enabled==false && listProv.Items.Count>0) {
				listProv.SetSelected(0);
			}
			//If the user cannot run this report for any other provider, every single provider available in the list will be the provider logged in.
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				listProv.SetAll(true);
			}
			if(!PrefC.HasClinicsEnabled) {
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
				checkClinicBreakdown.Visible=false;
			}
			else {
				checkClinicBreakdown.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicBreakdown);
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
			switch(PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {
				case 0:	radioWriteoffPay.Checked=true; break;
				case 1:	radioWriteoffProc.Checked=true; break;
				case 2:	radioWriteoffClaim.Checked=true; break;
				default:
					radioWriteoffClaim.Checked=true; break;
			}
			Text+=PrefC.ReportingServer.DisplayStr=="" ? "" : " - "+Lan.g(this,"Reporting Server:") +" "+ PrefC.ReportingServer.DisplayStr;
		}

		private PPOWriteoffDateCalc GetWriteoffType() {
			if(radioWriteoffPay.Checked) {
				return PPOWriteoffDateCalc.InsPayDate;
			}
			else if(radioWriteoffClaim.Checked) {
				return PPOWriteoffDateCalc.ClaimPayDate;
			}
			else {
				return PPOWriteoffDateCalc.ProcDate;
			}
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

		private void butThis_Click(object sender, System.EventArgs e) {
			textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
			textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
				,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
		}

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			textDateFrom.Text=_dateFrom.AddMonths(-1).ToShortDateString();
			_dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month)==_dateTo.Day){
				toLastDay=true;
			}
			_dateTo=_dateTo.AddMonths(-1);
			if(toLastDay){
				_dateTo=new DateTime(_dateTo.Year,_dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month));
			}
			textDateTo.Text=_dateTo.ToShortDateString();
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			textDateFrom.Text=_dateFrom.AddMonths(1).ToShortDateString();
			_dateTo=PIn.Date(textDateTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month)==_dateTo.Day){
				toLastDay=true;
			}
			_dateTo=_dateTo.AddMonths(1);
			if(toLastDay) {
				_dateTo=new DateTime(_dateTo.Year,_dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(_dateTo.Year,_dateTo.Month));
			}
			textDateTo.Text=_dateTo.ToShortDateString();
		}

		private void RunProdGoal() {
			//If adding the unearned column, need more space. Set report to landscape.
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			_dateFrom=PIn.Date(textDateFrom.Text);
			_dateTo=PIn.Date(textDateTo.Text);
			List<Provider> listProvs=new List<Provider>();
			if(checkAllProv.Checked){
				listProvs=_listProviders;
			}
			else if(listProv.SelectedIndices.Count>0){
				listProvs=listProv.SelectedIndices.Select(x => _listFilteredProviders[x]).ToList();
			}
			List<Clinic> listClinics=new List<Clinic>();
			List<long> listSelectedClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(listClin.SelectedIndices.Count>0) {
					int offset=Security.CurUser.ClinicIsRestricted?0:1;
					listClinics.AddRange(listClin.SelectedIndices.Select(x => offset==1 && x==0?new Clinic { ClinicNum=0,Abbr=Lan.g(this,"Unassigned") }:_listClinics[x-offset]));
				}
				if(checkAllClin.Checked) {
					//Add all remaining non-restricted clinics to the list
					listClinics.AddRange(Clinics.GetAllForUserod(Security.CurUser).Where(x => !listClinics.Select(y => y.ClinicNum).Contains(x.ClinicNum)));
				}
				//Check here for multi clinic schedule overlap and give notification.
				listSelectedClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
				var listConflicts=listProvs
					.Select(x => new { x.Abbr,listScheds=Schedules.GetClinicOverlapsForProv(_dateFrom,_dateTo,x.ProvNum,listSelectedClinicNums) })
					.Where(x => x.listScheds.Count>0).ToList();
				if(listConflicts.Count>0) {
					string errorMsg="This report is designed to show production goals by clinic and provider.  You have one or more providers during the "
						+"specified period that are scheduled in more than one clinic at the same time.  Due to this, production goals cannot be reported "
						+"accurately.\r\nTo run this report, please fix your scheduling so each provider is only scheduled at one clinic at a time, or select "
						+"different providers or clinics.\r\nIn the mean time, you can run regular production and income reports instead.\r\n\r\n"
						+"Conflicts:\r\n"
						+string.Join("\r\n",listConflicts
							.SelectMany(x => x.listScheds
								.Select(y => x.Abbr+" "+y.SchedDate.ToShortDateString()+" "+y.StartTime.ToShortTimeString()+" - "+y.StopTime.ToShortTimeString())));
					using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(errorMsg);
					msgBox.ShowDialog();
					return;
				}
			}
			ReportComplex report=new ReportComplex(true,false);
			bool hasAllClinics=checkAllClin.Checked && listSelectedClinicNums.Contains(0)
				&& Clinics.GetDeepCopy().Select(x => x.ClinicNum).All(x => ListTools.In(x,listSelectedClinicNums));
			using(DataSet ds=RpProdGoal.GetData(_dateFrom,_dateTo,listProvs,listClinics,checkAllProv.Checked,hasAllClinics,GetWriteoffType()))
			using(DataTable dt=ds.Tables["Total"])
			using(DataTable dtClinic=PrefC.HasClinicsEnabled?ds.Tables["Clinic"]:new DataTable())
			using(Font font=new Font("Tahoma",8,FontStyle.Regular)) {
				report.ReportName="MonthlyP&IGoals";
				report.AddTitle("Title",Lan.g(this,"Monthly Production Goal"));
				report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle));
				report.AddSubTitle("Date",_dateFrom.ToShortDateString()+" - "+_dateTo.ToShortDateString());
				report.AddSubTitle("Providers",checkAllProv.Checked?Lan.g(this,"All Providers"):listProvs.Count==0?"":string.Join(", ",listProvs.Select(x => x.Abbr)));
				if(PrefC.HasClinicsEnabled) {
					report.AddSubTitle("Clinics",hasAllClinics?Lan.g(this,"All Clinics"):listClinics.Count==0?"":string.Join(", ",listClinics.Select(x => x.Abbr)));
				}
				//setup query
				QueryObject query;
				if(PrefC.HasClinicsEnabled && checkClinicBreakdown.Checked) {
					query=report.AddQuery(dtClinic,"","Clinic",SplitByKind.Value,1,true);
				}
				else {
					query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
				}
				// add columns to report
				int dateWidth=70;
				int weekdayWidth=65;
				int prodWidth=90;
				int prodGoalWidth=90;
				int schedWidth=85;
				int adjWidth=85;
				int writeoffWidth=95;
				int writeoffestwidth=95;
				int writeoffadjwidth=70;
				int totProdWidth=90;
				int summaryOffSetY=30;
				int groups=1;
				if(PrefC.HasClinicsEnabled && listClin.SelectedIndices.Count>1 && checkClinicBreakdown.Checked) {
					groups=2;
				}
				for(int i=0;i<groups;i++) {//groups will be 1 or 2 if there are clinic breakdowns
					if(i>0) {//If more than one clinic selected, we want to add a table to the end of the report that totals all the clinics together
						query=report.AddQuery(dt,"Totals","",SplitByKind.None,2,true);
					}
					query.AddColumn("Date",dateWidth,FieldValueType.String,font);
					query.AddColumn("Weekday",weekdayWidth,FieldValueType.String,font);
					query.AddColumn("Production",prodWidth,FieldValueType.Number,font);
					query.AddColumn("Prod Goal",prodGoalWidth,FieldValueType.Number,font);
					query.AddColumn("Scheduled",schedWidth,FieldValueType.Number,font);
					query.AddColumn("Adjusts",adjWidth,FieldValueType.Number,font);
					if(GetWriteoffType()==PPOWriteoffDateCalc.ClaimPayDate) {
						query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number,font);
						query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number,font);
					}
					else {
						query.AddColumn("Writeoff",writeoffWidth,FieldValueType.Number,font);
					}
					query.AddColumn("Tot Prod",totProdWidth,FieldValueType.Number,font);
				}
				string colNameAlign="Writeoff"+(GetWriteoffType()==PPOWriteoffDateCalc.ClaimPayDate?" Est":"");//Column used to align the summary fields.
				string summaryText="Total Production (Production + Scheduled + Adjustments - Writeoff"
					+(GetWriteoffType()==PPOWriteoffDateCalc.ClaimPayDate?" Ests - Writeoff Adjs":"s")+"): ";
				query.AddGroupSummaryField(summaryText,colNameAlign,"Tot Prod",SummaryOperation.Sum,new List<int> { groups },Color.Black,
					new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				report.AddPageNum();
				// execute query
				if(!report.SubmitQueries()) {
					return;
				}
				// display report
				using(FormReportComplex FormR=new FormReportComplex(report)) {
					FormR.ShowDialog();
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
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
			_dateFrom=PIn.Date(textDateFrom.Text);
			_dateTo=PIn.Date(textDateTo.Text);
			if(_dateTo<_dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			RunProdGoal();
			//DialogResult=DialogResult.OK;//Stay here so that a series of similar reports can be run
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}








