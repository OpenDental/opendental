using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpProdInc : FormODBase {
		private DateTime dateFrom;
		private DateTime dateTo;
		///<summary>Can be set externally when automating.</summary>
		public string DailyMonthlyAnnual;
		///<summary>If set externally, then this sets the date on startup.</summary>
		public DateTime DateStart;
		///<summary>If set externally, then this sets the date on startup.</summary>
		public DateTime DateEnd;
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
		public FormRpProdInc(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormProduction_Load(object sender, System.EventArgs e) {
			_listProviders=Providers.GetListReports();
			_listProviders.Insert(0,Providers.GetUnearnedProv());
			_listFilteredProviders=new List<Provider>();
			textToday.Text=DateTime.Today.ToShortDateString();
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				Provider prov=Providers.GetFirstOrDefault(x => x.ProvNum==Security.CurUser.ProvNum);
				if(prov!=null) {
					_listProviders=_listProviders.FindAll(x => x.FName == prov.FName && x.LName == prov.LName);
				}
				checkAllProv.Checked=false;
				checkAllProv.Enabled=false;
			}
			//Fill the short list of providers, ignoring those marked "hidden on reports"
			for(int i=0;i<_listProviders.Count;i++){
				if(_listProviders[i].IsHiddenReport) {
					continue;
				}
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				_listFilteredProviders.Add(_listProviders[i].Copy());
			}
			//If the user is not allowed to run the report for all providers, default the selection to the first in the list box.
			if(checkAllProv.Enabled==false && listProv.Items.Count > 0) {
				listProv.SetSelected(0);
			}
			//If the user cannot run this report for any other provider, every single provider available in the list will be the provider logged in.
			if(!Security.IsAuthorized(Permissions.ReportProdIncAllProviders,true)) {
				listProv.SetAll(true);
			}
			if(!PrefC.HasClinicsEnabled){
				listClin.Visible=false;
				labelClin.Visible=false;
				checkAllClin.Visible=false;
				checkClinicInfo.Visible=false;
				checkClinicBreakdown.Visible=false;
			}
			else {
				checkClinicInfo.Checked=PrefC.GetBool(PrefName.ReportPandIhasClinicInfo);
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
			switch(DailyMonthlyAnnual){
				case "Daily":
					radioDaily.Checked=true;
					break;
				case "Monthly":
					radioMonthly.Checked=true;
					break;
				case "Annual":
					radioAnnual.Checked=true;
					break;
			}
			SetDates();
			switch(PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {
				case 0: radioWriteoffPay.Checked=true; break;
				case 1: radioWriteoffProc.Checked=true; break;
				case 2: radioWriteoffBoth.Checked=true; break;
				default: radioWriteoffBoth.Checked=true; break;
			}
			if(DateStart.Year>1880){
				textDateFrom.Text=DateStart.ToShortDateString();
				textDateTo.Text=DateEnd.ToShortDateString();
				switch(DailyMonthlyAnnual) {
					case "Daily":
						RunDaily();
						break;
					case "Monthly":
						RunMonthly();
						break;
					case "Annual":
						RunAnnual();
						break;
				}
				Close();
			}
			Text+=PrefC.ReportingServer.DisplayStr=="" ? "" : " - "+Lan.g(this,"Reporting Server:") +" "+ PrefC.ReportingServer.DisplayStr;
			Plugins.HookAddCode(this,"FormProduction_Load_end");
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

		private void radioDaily_Click(object sender, System.EventArgs e) {
			SetDates();
		}

		private void radioMonthly_Click(object sender, System.EventArgs e) {
			SetDates();
		}

		private void radioAnnual_Click(object sender, System.EventArgs e) {
			SetDates();
		}

		private void radioProvider_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void radioProviderPayroll_Click(object sender,EventArgs e) {
			SetDates();
		}

		private void SetDates(){
			if(radioDaily.Checked) {
				if(PrefC.HasClinicsEnabled) {
					checkClinicInfo.Visible=true;
					if(checkClinicInfo.Checked) {
						checkClinicBreakdown.Visible=true;
					}
					else {
						//Clinic info not checked so hide the clinic breakdown
						checkClinicBreakdown.Checked=false;
						checkClinicBreakdown.Visible=false;
					}
				}
				textDateFrom.Text=DateTime.Today.ToShortDateString();
				textDateTo.Text=DateTime.Today.ToShortDateString();
				butThis.Text=Lan.g(this,"Today");
			}
			else if(radioProvider.Checked) {
				if(PrefC.HasClinicsEnabled) {
					checkClinicInfo.Visible=false;
					checkClinicBreakdown.Visible=true;
				}
				textDateFrom.Text=DateTime.Today.ToShortDateString();
				textDateTo.Text=DateTime.Today.ToShortDateString();
				butThis.Text=Lan.g(this,"Today");
			}
			else if(radioMonthly.Checked) {
				if(PrefC.HasClinicsEnabled) {
					checkClinicInfo.Visible=false;
					checkClinicBreakdown.Visible=true;
				}
				textDateFrom.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).ToShortDateString();
				textDateTo.Text=new DateTime(DateTime.Today.Year,DateTime.Today.Month
					,DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month)).ToShortDateString();
				butThis.Text=Lan.g(this,"This Month");
			}
			else{//annual
				if(PrefC.HasClinicsEnabled) {
					checkClinicInfo.Visible=false;
					checkClinicBreakdown.Visible=true;
				}
				textDateFrom.Text=new DateTime(DateTime.Today.Year,1,1).ToShortDateString();
				textDateTo.Text=new DateTime(DateTime.Today.Year,12,31).ToShortDateString();
				butThis.Text=Lan.g(this,"This Year");
			}
		}

		private void butThis_Click(object sender, System.EventArgs e) {
			SetDates();
		}

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			dateFrom=PIn.Date(textDateFrom.Text);
			if(dateFrom.Year < 1880) {
				MsgBox.Show(this,"Please fix the From date first.");
				return;
			}
			dateTo=PIn.Date(textDateTo.Text);
			if(radioDaily.Checked || radioProvider.Checked) {
				textDateFrom.Text=dateFrom.AddDays(-1).ToShortDateString();
				textDateTo.Text=dateTo.AddDays(-1).ToShortDateString();
			}
			else if(radioMonthly.Checked){
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
			else{//annual
				textDateFrom.Text=dateFrom.AddYears(-1).ToShortDateString();
				textDateTo.Text=dateTo.AddYears(-1).ToShortDateString();
			}
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			dateFrom=PIn.Date(textDateFrom.Text);
			dateTo=PIn.Date(textDateTo.Text);
			if(radioDaily.Checked || radioProvider.Checked) {
				textDateFrom.Text=dateFrom.AddDays(1).ToShortDateString();
				textDateTo.Text=dateTo.AddDays(1).ToShortDateString();
			}
			else if(radioMonthly.Checked){
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
			else{//annual
				textDateFrom.Text=dateFrom.AddYears(1).ToShortDateString();
				textDateTo.Text=dateTo.AddYears(1).ToShortDateString();
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

		///<summary>Gets the list of clinics to use in the report. Filters out clinics depending on selection and security permissions.</summary>
		private List<Clinic> GetClinicsForReport() {
			List<Clinic> listClinics=new List<Clinic>();
			if(PrefC.HasClinicsEnabled) {
				for(int i=0;i<listClin.SelectedIndices.Count;i++) {
					if(Security.CurUser.ClinicIsRestricted) {
						listClinics.Add(_listClinics[listClin.SelectedIndices[i]]);//we know that the list is a 1:1 to _listClinics
					}
					else {
						if(listClin.SelectedIndices[i]==0) {
							listClinics.Add(new Clinic {
								ClinicNum=0,
								Abbr=Lan.g(this,"Unassigned")
							});//Will have ClinicNum of 0 for our "Unassigned" needs.
						}
						else {
							listClinics.Add(_listClinics[listClin.SelectedIndices[i]-1]);//Minus 1 from the selected index
						}
					}
				}
				if(checkAllClin.Checked) {//All Clinics selected; add all visible or hidden unrestricted clinics to the list
					listClinics.AddRange(Clinics.GetAllForUserod(Security.CurUser).Where(x => !listClinics.Select(y => y.ClinicNum).Contains(x.ClinicNum)));
				}
			}
			return listClinics;
		}

		private void RunDaily() {
			if(Plugins.HookMethod(this,"FormRpProdInc.RunDaily_Start",PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text))) {
				return;
			}
			//The old daily prod and inc report (prior to report complex) had portait mode for non-clinic users and landscape for clinic users.
			bool isLandscape=false;
			if((PrefC.HasClinicsEnabled && checkClinicInfo.Checked) || radioWriteoffBoth.Checked) {
				isLandscape=true;
			}
			ReportComplex report=new ReportComplex(true,isLandscape);
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			else if(listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"All providers are hidden on reports.");
				return;
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			dateFrom=PIn.Date(textDateFrom.Text);
			dateTo=PIn.Date(textDateTo.Text);
			List<Provider> listProvs=new List<Provider>();
			if(checkAllProv.Checked) {
				listProvs=_listProviders;
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvs.Add(_listFilteredProviders[listProv.SelectedIndices[i]]);
				}
			}
			List<Clinic> listClinics=GetClinicsForReport();
			//true if the all clinics checkbox is checked and the selected clinics contains every ClinicNum, including the 'Unassigned' ClinicNum of 0,
			//all hidden clinics, and the user cannot be restricted.  'All' clinics means all in the list, which may not be all clinics.
			List<long> listSelectedClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			bool hasAllClinics=checkAllClin.Checked && listSelectedClinicNums.Contains(0)
				&& Clinics.GetDeepCopy().Select(x => x.ClinicNum).All(x => ListTools.In(x,listSelectedClinicNums));
			DataSet dataSetDailyProd=RpProdInc.GetDailyData(dateFrom,dateTo,listProvs,listClinics,checkAllProv.Checked,hasAllClinics
				,checkClinicBreakdown.Checked,checkClinicInfo.Checked,checkUnearned.Checked,GetWriteoffType());
			DataTable tableDailyProd=dataSetDailyProd.Tables["DailyProd"];//Includes multiple clinics that will get separated out later.
			DataSet dataSetDailyProdSplitByClinic=new DataSet();
			if(PrefC.HasClinicsEnabled && checkClinicInfo.Checked) {
				//Split up each clinic into its own table and add that to the data set split up by clinics.
				string lastClinic="";
				DataTable dtClinic=tableDailyProd.Clone();//Clones the structure, not the data.
				for(int i=0;i<tableDailyProd.Rows.Count;i++) {
					string currentClinic=tableDailyProd.Rows[i]["Clinic"].ToString();
					if(currentClinic=="") {
						currentClinic="Unassigned"; //not actually displayed to the user, so no translation.
					}
					if(lastClinic=="") {
						lastClinic=currentClinic;
					}
					//Check if we have successfully added all rows for the current clinic and add the datatable to the dataset if there is information present.
					if(lastClinic!=currentClinic && dtClinic.Rows.Count>0) {
						DataTable dtClinicTemp=dtClinic.Copy();
						dtClinicTemp.TableName="Clinic"+i;//The name of the table does not matter but has to be unique in a DataSet.
						dataSetDailyProdSplitByClinic.Tables.Add(dtClinicTemp);
						dtClinic.Rows.Clear();//Clear out the data to start collecting the information for the next clinic.
						lastClinic=currentClinic;
					}
					dtClinic.Rows.Add(tableDailyProd.Rows[i].ItemArray);
					//If this is the last row, add dtClinic to the dataset.
					if(i==tableDailyProd.Rows.Count-1) {
						DataTable dtClinicTemp=dtClinic.Copy();
						//Added 1 to guarantee unique tablename.
						dtClinicTemp.TableName="Clinic"+(i+1);//The name of the table does not matter but has to be unique in a DataSet. 
						dataSetDailyProdSplitByClinic.Tables.Add(dtClinicTemp);
					}
				}
			}
			report.ReportName="DailyP&I";
			report.AddTitle("Title",Lan.g(this,"Daily Production and Income"));
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle));
			string dateRangeStr=dateFrom.ToShortDateString()+" - "+dateTo.ToShortDateString();
			if(dateFrom.Date==dateTo.Date) {
				dateRangeStr=dateFrom.ToShortDateString();//Do not show a date range for the same day...
			}
			report.AddSubTitle("Date",dateRangeStr);
			if(checkAllProv.Checked) {
				report.AddSubTitle("Providers",Lan.g(this,"All Providers"));
			}
			else {
				string str="";
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					if(i>0) {
						str+=", ";
					}
					str+=_listFilteredProviders[listProv.SelectedIndices[i]].Abbr;
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
			QueryObject query=null;
			//Default Column Widths for Landscape (either clinic info showing or insurance writeoff est info showing) -----------------
			int dateWidth=75;
			int patientNameWidth=130;
			int descriptionWidth=220;
			int provWidth=65;
			int clinicWidth=130;
			int prodWidth=75;
			int adjustWidth=75;
			int writeoffWidth=75;
			int writeoffEstWidth=75;
			int writeoffAdjWidth=75;
			int ptIncWidth=75;
			int insIncWidth=75;
			//This has every column showing and will not fit with the normal landscape column widths.
			if(radioWriteoffBoth.Checked && checkClinicInfo.Checked) {
				clinicWidth=100;
				prodWidth=65;
				adjustWidth=65;
				writeoffWidth=65;
				writeoffEstWidth=65;
				writeoffAdjWidth=65;
				ptIncWidth=65;
				insIncWidth=65;
			}
			if(!report.IsLandscape) {
        //Trim some fat off for non-clinic users because this report shows in portait mode (default widths are for landscape).
        dateWidth=68;
        patientNameWidth=120;
        descriptionWidth=180;
        provWidth=55;
				prodWidth=75;
        adjustWidth=70;
        writeoffWidth=70;
				ptIncWidth=75;
				insIncWidth=75;
      }
      Font font=new Font("Tahoma",8);
			query=report.AddQuery(tableDailyProd,Lan.g(this,"Date")+": "+DateTime.Today.ToShortDateString(),"ClinicSplit",SplitByKind.Value,1,true);
			query.AddColumn("Date",dateWidth,FieldValueType.String,font);
			query.AddColumn("Patient Name",patientNameWidth,FieldValueType.String,font);
			query.AddColumn("Description",descriptionWidth,FieldValueType.String,font);
			query.AddColumn("Prov",provWidth,FieldValueType.String,font);
			if(PrefC.HasClinicsEnabled && checkClinicInfo.Checked) {//Not no clinics
				query.AddColumn("Clinic",clinicWidth,FieldValueType.String,font);
			}
			query.AddColumn("Production",prodWidth,FieldValueType.Number,font);
			query.AddColumn("Adjust",adjustWidth,FieldValueType.Number,font);
			if(radioWriteoffBoth.Checked) {
				query.AddColumn("Writeoff Est",writeoffEstWidth,FieldValueType.Number,font);
				query.AddColumn("Writeoff Adj",writeoffAdjWidth,FieldValueType.Number,font);
			}
			else {
				query.AddColumn("Writeoff",writeoffWidth,FieldValueType.Number,font);
			}
			query.AddColumn("Pt Income",ptIncWidth,FieldValueType.Number,font);
			query.AddColumn("Ins Income",insIncWidth,FieldValueType.Number,font);
			//If more than one clinic selected, we want to add a table to the end of the report that totals all the clinics together.
			//When only one clinic is showing , the "Summary" at the end of every daily report will suffice. (total prod and total income lines).
			if(PrefC.HasClinicsEnabled && listClinics.Count > 1 && checkClinicInfo.Checked) {
				DataTable tableClinicTotals=GetClinicTotals(dataSetDailyProdSplitByClinic);
				query=report.AddQuery(tableClinicTotals,"Clinic Totals","",SplitByKind.None,2,true);
				query.AddColumn("Clinic",410,FieldValueType.String,font);
				query.AddColumn("Production",75,FieldValueType.Number,font);
				query.AddColumn("Adjust",75,FieldValueType.Number,font);
				if(radioWriteoffBoth.Checked) {
					query.AddColumn("Writeoff Est",writeoffEstWidth,FieldValueType.Number,font);
					query.AddColumn("Writeoff Adj",writeoffAdjWidth,FieldValueType.Number,font);
				}
				else {
					query.AddColumn("Writeoff",75,FieldValueType.Number,font);
				}
				query.AddColumn("Pt Income",75,FieldValueType.Number,font);
				query.AddColumn("Ins Income",75,FieldValueType.Number,font);
			}
			//Calculate the total production and total income and add them to the bottom of the report:
			double totalProduction=0;
			double totalIncome=0;
			for(int i=0;i<tableDailyProd.Rows.Count;i++) {
				//Total production is (Production + Adjustments - Writeoffs)
				totalProduction+=PIn.Double(tableDailyProd.Rows[i]["Production"].ToString());
				totalProduction+=PIn.Double(tableDailyProd.Rows[i]["Adjust"].ToString());
				if(radioWriteoffBoth.Checked) {
					totalProduction+=PIn.Double(tableDailyProd.Rows[i]["Writeoff Est"].ToString());
					totalProduction+=PIn.Double(tableDailyProd.Rows[i]["Writeoff Adj"].ToString());
				}
				else {
					totalProduction+=PIn.Double(tableDailyProd.Rows[i]["Writeoff"].ToString());
				}
				//Total income is (Pt Income + Ins Income)
				totalIncome+=PIn.Double(tableDailyProd.Rows[i]["Pt Income"].ToString());
				totalIncome+=PIn.Double(tableDailyProd.Rows[i]["Ins Income"].ToString());
			}
			//Add the Total Production and Total Income to the bottom of the report if there were any rows present.
			if(tableDailyProd.Rows.Count > 0) {
				//Use a custom table and add it like it is a "query" to the report because using a group summary would be more complicated due
				//to the need to add and subtract from multiple columns at the same time.
				DataTable tableTotals=new DataTable("TotalProdAndInc");
				tableTotals.Columns.Add("Summary");
				string prodLabel;
				if(radioWriteoffBoth.Checked) {
					prodLabel="Total Production (Production + Adjustments - Writeoff Ests - WriteOff Adjs)";
				}
				else {
					prodLabel="Total Production (Production + Adjustments - Writeoffs)";
				}
				tableTotals.Rows.Add(Lan.g(this,prodLabel)+" "+totalProduction.ToString("c"));
				tableTotals.Rows.Add(Lan.g(this,"Total Income (Pt Income + Ins Income):")+" "+totalIncome.ToString("c"));
				//Add tableTotals to the report.
				//No column name and no header because we want to display this table to NOT look like a table.
				query=report.AddQuery(tableTotals,"","",SplitByKind.None,2,false);
				query.AddColumn("",785,FieldValueType.String,new Font("Tahoma",8,FontStyle.Bold));
			}
			report.AddPageNum();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			//DialogResult=DialogResult.OK;//Allow running multiple reports.
		}

		private PPOWriteoffDateCalc GetWriteoffType() {
			if(radioWriteoffPay.Checked) {
				return PPOWriteoffDateCalc.InsPayDate;
			}
			else if(radioWriteoffProc.Checked){
				return PPOWriteoffDateCalc.ProcDate;
			}
			else {//radioWriteoffClaim.Checked is checked
				return PPOWriteoffDateCalc.ClaimPayDate;
			}
		} 

		private DataTable GetClinicTotals(DataSet dataSetDailyProdSplitByClinic) {
			DataTable tableClinicTotals=new DataTable("ClinicTotals");
			tableClinicTotals.Columns.Add(new DataColumn("Clinic"));
			tableClinicTotals.Columns.Add(new DataColumn("Production"));
			tableClinicTotals.Columns.Add(new DataColumn("Adjust"));
			if(radioWriteoffBoth.Checked) {
				tableClinicTotals.Columns.Add(new DataColumn("Writeoff Est"));
				tableClinicTotals.Columns.Add(new DataColumn("Writeoff Adj"));
			}
			else {
				tableClinicTotals.Columns.Add(new DataColumn("Writeoff"));
			}
			tableClinicTotals.Columns.Add(new DataColumn("Pt Income"));
			tableClinicTotals.Columns.Add(new DataColumn("Ins Income"));
			for(int i=0;i<dataSetDailyProdSplitByClinic.Tables.Count;i++) {
				string clinicDesc="";
				if(dataSetDailyProdSplitByClinic.Tables[i].Rows.Count > 0) {
					clinicDesc=dataSetDailyProdSplitByClinic.Tables[i].Rows[0]["Clinic"].ToString();//Take description of first row.
				}
				clinicDesc=clinicDesc=="" ? Lan.g(this,"Unassigned") : clinicDesc;
				//Calculate the total production and total income for this clinic.
				double production=0;
				double adjust=0;
				double writeoffest=0;
				double writeoff=0;
				double writeoffadj=0;
				double ptIncome=0;
				double insIncome=0;
				for(int j=0;j<dataSetDailyProdSplitByClinic.Tables[i].Rows.Count;j++) {
					production+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Production"].ToString());
					adjust+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Adjust"].ToString());
					if(radioWriteoffBoth.Checked) {
						writeoffest+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Writeoff Est"].ToString());
						writeoffadj+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Writeoff Adj"].ToString());
					}
					else {
						writeoff+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Writeoff"].ToString());
					}
					ptIncome+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Pt Income"].ToString());
					insIncome+=PIn.Double(dataSetDailyProdSplitByClinic.Tables[i].Rows[j]["Ins Income"].ToString());
				}
				if(radioWriteoffBoth.Checked) {
					tableClinicTotals.Rows.Add(clinicDesc,production,adjust,writeoffest,writeoffadj,ptIncome,insIncome);
				}
				else {
					tableClinicTotals.Rows.Add(clinicDesc,production,adjust,writeoff,ptIncome,insIncome);
				}
			}
			return tableClinicTotals;
		}

		private void RunMonthly(){
			if(Plugins.HookMethod(this,"FormRpProdInc.RunMonthly_Start",PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text))) {
				return;
			}
			//If adding the unearned column, need more space. Set report to landscape.
			ReportComplex report=new ReportComplex(true,radioWriteoffBoth.Checked || checkUnearned.Checked);
			report.PrintMargins=new Margins(45,0,50,50);
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			else if(listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"All providers are hidden on reports.");
				return;
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			dateFrom=PIn.Date(textDateFrom.Text);
			dateTo=PIn.Date(textDateTo.Text);
			List<Provider> listProvs=new List<Provider>();
			if(checkAllProv.Checked) {
				listProvs=_listProviders;
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvs.Add(_listFilteredProviders[listProv.SelectedIndices[i]]);
				}
			}
			List<Clinic> listClinics=GetClinicsForReport();
			//true if the all clinics checkbox is checked and the selected clinics contains every ClinicNum, including the 'Unassigned' ClinicNum of 0,
			//all hidden clinics, and the user cannot be restricted.  'All' clinics means all in the list, which may not be all clinics.
			List<long> listSelectedClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			bool hasAllClinics=checkAllClin.Checked && listSelectedClinicNums.Contains(0)
				&& Clinics.GetDeepCopy().Select(x => x.ClinicNum).All(x => ListTools.In(x,listSelectedClinicNums));
			DataSet ds=RpProdInc.GetMonthlyData(dateFrom,dateTo,listProvs,listClinics,radioWriteoffPay.Checked,checkAllProv.Checked,hasAllClinics
				,radioWriteoffBoth.Checked,checkUnearned.Checked);
			DataTable dt=ds.Tables["Total"];
			DataTable dtClinic=new DataTable();
			if(PrefC.HasClinicsEnabled) {
				dtClinic=ds.Tables["Clinic"];
			}
			report.ReportName="MonthlyP&I";
			report.AddTitle("Title",Lan.g(this,"Monthly Production and Income"));
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
					str+=_listFilteredProviders[listProv.SelectedIndices[i]].Abbr;
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
			if(PrefC.HasClinicsEnabled && checkClinicBreakdown.Checked) {
				query=report.AddQuery(dtClinic,"","Clinic",SplitByKind.Value,1,true);
			}
			else {
				query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
			}
			// add columns to report
			Font font=new Font("Tahoma",8,FontStyle.Regular);
			int datewidth=70;
			int day=35;
			int productionwidth=90;
			int schedwidth=85;
			int adjwidth=85;
			int writeoffestwidth=95;
			int writeoffwidth=80;
			int writeoffadjwidth=70;
			int totprodwidth=90;
			int ptincomewidth=85;
			int unearnedPtIncomeWidth=80;
			int insincomewidth=85;
			int totincomewidth=90;
			int summaryOffSetY=30;
			int summaryIncomeOffSetY=4;
			query.AddColumn("Date",datewidth,FieldValueType.String,font);
			query.AddColumn("Day",day,FieldValueType.String,font);
			query.AddColumn("Production",productionwidth,FieldValueType.Number,font);
			query.AddColumn("Sched",schedwidth,FieldValueType.Number,font);
			query.AddColumn("Adj",adjwidth,FieldValueType.Number,font);
			if(radioWriteoffBoth.Checked) {
				query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number,font);
				query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number,font);
			}
			else {
				query.AddColumn("Writeoff",writeoffwidth,FieldValueType.Number,font);
			}
			query.AddColumn("Tot Prod",totprodwidth,FieldValueType.Number,font);
			query.AddColumn("Pt Income",ptincomewidth,FieldValueType.Number,font);
			if(checkUnearned.Checked) {
				query.AddColumn("Unearned Pt Income",unearnedPtIncomeWidth,FieldValueType.Number,font);
			}
			query.AddColumn("Ins Income",insincomewidth,FieldValueType.Number,font);
			query.AddColumn("Tot Income",totincomewidth,FieldValueType.Number,font);
			if(PrefC.HasClinicsEnabled && listClin.SelectedIndices.Count>1 && checkClinicBreakdown.Checked) {
				//If more than one clinic selected, we want to add a table to the end of the report that totals all the clinics together.
				query=report.AddQuery(dt,"Totals","",SplitByKind.None,2,true);
				query.AddColumn("Date",datewidth,FieldValueType.String,font);
				query.AddColumn("Day",day,FieldValueType.String,font);
				query.AddColumn("Production",productionwidth,FieldValueType.Number,font);
				query.AddColumn("Sched",schedwidth,FieldValueType.Number,font);
				query.AddColumn("Adj",adjwidth,FieldValueType.Number,font);
				if(radioWriteoffBoth.Checked) {
					query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number,font);
					query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number,font);
				}
				else {
					query.AddColumn("Writeoff",writeoffwidth,FieldValueType.Number,font);
				}
				query.AddColumn("Tot Prod",totprodwidth,FieldValueType.Number,font);
				query.AddColumn("Pt Income",ptincomewidth,FieldValueType.Number,font);
				if(checkUnearned.Checked) {
					query.AddColumn("Unearned Pt Income",unearnedPtIncomeWidth,FieldValueType.Number,font);
				}
				query.AddColumn("Ins Income",insincomewidth,FieldValueType.Number,font);
				query.AddColumn("Tot Income",totincomewidth,FieldValueType.Number,font);
				//Column used to align the summary fields.
				string columnNameAlign =radioWriteoffBoth.Checked ? "Tot Prod": "Writeoff";
				if(radioWriteoffBoth.Checked) {
					query.AddGroupSummaryField("Total Production (Production + Scheduled + Adjustments - Writeoff Ests - Writeoff Adjs): ",
						columnNameAlign,"Tot Prod",SummaryOperation.Sum,new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				else {
					query.AddGroupSummaryField("Total Production (Production + Scheduled + Adjustments - Writeoffs): ",columnNameAlign,
						"Tot Prod",SummaryOperation.Sum,new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				if(checkUnearned.Checked) {//if unearned check, add summaries.
					query.AddGroupSummaryField("Total Pt Income (Pt Income + Unearned Pt Income): ",columnNameAlign,"Total Pt Income",
						SummaryOperation.Sum,new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
				}
				query.AddGroupSummaryField("Total Income (Total Pt Income + Ins Income): ",columnNameAlign,"Total Income",SummaryOperation.Sum,
					new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
			}
			else {
				string columnNameAlign=radioWriteoffBoth.Checked ? "Tot Prod" : "Writeoff";//column used to align the summary fields
				if(radioWriteoffBoth.Checked) {
					query.AddGroupSummaryField("Total Production (Production + Scheduled + Adjustments - Writeoff Ests - Writeoff Adjs): ",
						columnNameAlign,"Tot Prod",SummaryOperation.Sum,new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				else {
					query.AddGroupSummaryField("Total Production (Production + Scheduled + Adjustments - Writeoffs): ",columnNameAlign,
						"Tot Prod",SummaryOperation.Sum,new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				if(checkUnearned.Checked) {//if unearned check, add summaries.
					query.AddGroupSummaryField("Total Pt Income (Pt Income + Unearned Pt Income): ",columnNameAlign,"Total Pt Income",
						SummaryOperation.Sum,new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
				}
				query.AddGroupSummaryField("Total Income (Total Pt Income + Ins Income): ",columnNameAlign,"Total Income",SummaryOperation.Sum,
					new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
			}
			report.AddPageNum();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			//DialogResult=DialogResult.OK;//Allow running multiple reports.
		}

		private void RunAnnual(){
			if(Plugins.HookMethod(this,"FormRpProdInc.RunAnnual_Start",PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text))) {
				return;
			}
			//If adding the unearned column, need more space. Set report to landscape.
			ReportComplex report=new ReportComplex(true,radioWriteoffBoth.Checked || checkUnearned.Checked);
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			else if(listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"All providers are hidden on reports.");
				return;
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			dateFrom=PIn.Date(textDateFrom.Text);
			dateTo=PIn.Date(textDateTo.Text);
			List<Provider> listProvs=new List<Provider>();
			if(checkAllProv.Checked) {
				listProvs=_listProviders;
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvs.Add(_listFilteredProviders[listProv.SelectedIndices[i]]);
				}
			}
			List<Clinic> listClinics=GetClinicsForReport();
			//true if the all clinics checkbox is checked and the selected clinics contains every ClinicNum, including the 'Unassigned' ClinicNum of 0,
			//all hidden clinics, and the user cannot be restricted.  'All' clinics means all in the list, which may not be all clinics.
			List<long> listSelectedClinicNums=listClinics.Select(x => x.ClinicNum).ToList();
			bool hasAllClinics=checkAllClin.Checked && listSelectedClinicNums.Contains(0)
				&& Clinics.GetDeepCopy().Select(x => x.ClinicNum).All(x => ListTools.In(x,listSelectedClinicNums));
			DataSet ds=RpProdInc.GetAnnualData(dateFrom,dateTo,listProvs,listClinics,radioWriteoffPay.Checked,checkAllProv.Checked,hasAllClinics
				,radioWriteoffBoth.Checked,checkUnearned.Checked);
			DataTable dt=ds.Tables["Total"];
			DataTable dtClinic=new DataTable();
			if(PrefC.HasClinicsEnabled) {
				dtClinic=ds.Tables["Clinic"];
			}
			report.ReportName="AnnualP&I";
			report.AddTitle("Title",Lan.g(this,"Annual Production and Income"));
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
					str+=_listFilteredProviders[listProv.SelectedIndices[i]].Abbr;
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
			if(PrefC.HasClinicsEnabled && checkClinicBreakdown.Checked) {
				query=report.AddQuery(dtClinic,"","Clinic",SplitByKind.Value,1,true);
			}
			else {
				query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
			}
			// add columns to report
			int datewidth=65;//65px width allows room for a 3 letter month abbreviation and 4 digit year.
			int productionwidth=110;//110px width allows the total row to fit values up to 999,999,999.99 and down to -99,999,999.99
			int adjwidth=110;//110px width allows the total row to fit values up to 999,999,999.99 and down to -99,999,999.99
			int writeoffestwidth=115;//110px width allows the total row to fit values up to 999,999,999.99 and down to -99,999,999.99
			int writeoffwidth=110;//110px width allows the total row to fit values up to 999,999,999.99 and down to -99,999,999.99
			int writeoffadjwidth=80;//80px width allows the total row to fit values up to 999,999.99 and down to -99,999.99
			int totprodwidth=110;//110px width allows the total row to fit values up to 999,999,999.99 and down to -99,999,999.99
			int ptincomewidth=100;//100px width allows the total row to fit values up to 99,999,999.99 and down to -9,999,999.99
			int unearnedPtIncomeWidth=80;//80px width allows the total row to fit values up to 999,999.99 and down to -99,999.99
			int insincomewidth=100;//100px width allows the total row to fit values up to 99,999,999.99 and down to -9,999,999.99
			int totincomewidth=110;//110px width allows the total row to fit values up to 999,999,999.99 and down to -99,999,999.99
			int summaryOffSetY=30;
			int summaryIncomeOffSetY=4;
			query.AddColumn("Month",datewidth,FieldValueType.String);
			query.AddColumn("Production",productionwidth,FieldValueType.Number);
			query.AddColumn("Adjustments",adjwidth,FieldValueType.Number);
			if(radioWriteoffBoth.Checked) {
				query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number);
				query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number);
			}
			else {
				query.AddColumn("Writeoff",writeoffwidth,FieldValueType.Number);
			}
			query.AddColumn("Tot Prod",totprodwidth,FieldValueType.Number);
			query.AddColumn("Pt Income",ptincomewidth,FieldValueType.Number);
			if(checkUnearned.Checked) {
				query.AddColumn("Unearned Pt Income",unearnedPtIncomeWidth,FieldValueType.Number);
			}
			query.AddColumn("Ins Income",insincomewidth,FieldValueType.Number);
			query.AddColumn("Total Income",totincomewidth,FieldValueType.Number);
			if(PrefC.HasClinicsEnabled && listClin.SelectedIndices.Count>1 && checkClinicBreakdown.Checked) {
				//If more than one clinic selected, we want to add a table to the end of the report that totals all the clinics together.
				query=report.AddQuery(dt,"Totals","",SplitByKind.None,2,true);
				query.AddColumn("Month",datewidth,FieldValueType.String);
				query.AddColumn("Production",productionwidth,FieldValueType.Number);
				query.AddColumn("Adjustments",adjwidth,FieldValueType.Number);
				if(radioWriteoffBoth.Checked) {
					query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number);
					query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number);
				}				
				else {
					query.AddColumn("Writeoff",writeoffwidth,FieldValueType.Number);
				}
				query.AddColumn("Tot Prod",totprodwidth,FieldValueType.Number);
				query.AddColumn("Pt Income",ptincomewidth,FieldValueType.Number);
				if(checkUnearned.Checked) {
					query.AddColumn("Unearned Pt Income",unearnedPtIncomeWidth,FieldValueType.Number);
				}
				query.AddColumn("Ins Income",insincomewidth,FieldValueType.Number);
				query.AddColumn("Total Income",totincomewidth,FieldValueType.Number);
				if(radioWriteoffBoth.Checked) {
					query.AddGroupSummaryField("Total Production (Production + Adjustments - Writeoff Ests - Writeoff Adjs): ",
						"Tot Prod","Tot Prod",SummaryOperation.Sum,new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				else {
					query.AddGroupSummaryField("Total Production (Production + Adjustments - Writeoffs): ","Tot Prod",
						"Tot Prod",SummaryOperation.Sum,new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				if(checkUnearned.Checked) {//if unearned check, add summaries.
					query.AddGroupSummaryField("Total Pt Income (Pt Income + Unearned Pt Income): ","Tot Prod","Total Pt Income",
						SummaryOperation.Sum,new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
				}
				query.AddGroupSummaryField("Total Income (Total Pt Income + Ins Income): ","Tot Prod","Total Income",SummaryOperation.Sum,
					new List<int>() { 2 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
			}
			else {
				if(radioWriteoffBoth.Checked) {
					query.AddGroupSummaryField("Total Production (Production + Adjustments - Writeoff Ests - Writeoff Adjs): ",
						"Tot Prod","Tot Prod",SummaryOperation.Sum,new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				else {
					query.AddGroupSummaryField("Total Production (Production + Adjustments - Writeoffs): ","Tot Prod",
						"Tot Prod",SummaryOperation.Sum,new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryOffSetY);
				}
				if(checkUnearned.Checked) {//if unearned check, add summaries.
					query.AddGroupSummaryField("Total Pt Income (Pt Income + Unearned Pt Income): ","Tot Prod","Total Pt Income",
						SummaryOperation.Sum,new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
				}
				query.AddGroupSummaryField("Total Income (Total Pt Income + Ins Income): ","Tot Prod","Total Income",SummaryOperation.Sum,
					new List<int>() { 1 },Color.Black,new Font("Tahoma",9,FontStyle.Bold),75,summaryIncomeOffSetY);
			}
			report.AddPageNum();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			//DialogResult=DialogResult.OK;//Allow running multiple reports.
		}

		private void RunProvider() {
			//If adding the unearned column, need more space. Set report to landscape.
			ReportComplex report=new ReportComplex(true,radioWriteoffBoth.Checked || checkUnearned.Checked);
			dateFrom=PIn.Date(textDateFrom.Text);
			dateTo=PIn.Date(textDateTo.Text);
			if(checkAllProv.Checked) {
				listProv.SetAll(true);
			}
			else if(listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"All providers are hidden on reports.");
				return;
			}
			if(checkAllClin.Checked) {
				listClin.SetAll(true);
			}
			List<Provider> listProvs=new List<Provider>();
			if(checkAllProv.Checked) {
				listProvs=_listProviders;
			}
			else {
				for(int i=0;i<listProv.SelectedIndices.Count;i++) {
					listProvs.Add(_listFilteredProviders[listProv.SelectedIndices[i]]);
				}
			}
			List<Clinic> listClinics=GetClinicsForReport();
			bool hasAllClinics=(!Security.CurUser.ClinicIsRestricted && checkAllClin.Checked);
			DataSet ds=RpProdInc.GetProviderDataForClinics(dateFrom,dateTo,listProvs,listClinics,checkAllProv.Checked,hasAllClinics
				,checkUnearned.Checked,GetWriteoffType());
			report.ReportName="Provider P&I";
			report.AddTitle("Title",Lan.g(this,"Provider Production and Income"));
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
					str+=_listFilteredProviders[listProv.SelectedIndices[i]].Abbr;
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
			DataTable dtClinic=new DataTable();
			if(PrefC.HasClinicsEnabled) {
				dtClinic=ds.Tables["Clinic"].Copy();
			}
			DataTable dt=ds.Tables["Total"].Copy();
			if(PrefC.HasClinicsEnabled && checkClinicBreakdown.Checked) {
				query=report.AddQuery(dtClinic,"","Clinic",SplitByKind.Value,1,true);
			}
			else {
				query=report.AddQuery(dt,"","",SplitByKind.None,1,true);
			}
			// add columns to report
			int provwidth=110;
			int productionwidth=90;
			int adjwidth=90;
			int writeoffestwidth=90;
			int writeoffwidth=90;
			int writeoffadjwidth=90;
			int totprodwidth=90;
			int ptincomewidth=90;
			int unearnedPtIncomeWidth=90;
			int insincomewidth=90;
			int totincomewidth=90;
			query.AddColumn("Provider",provwidth,FieldValueType.String);
			query.AddColumn("Production",productionwidth,FieldValueType.Number);
			query.AddColumn("Adjustments",adjwidth,FieldValueType.Number);
			if(radioWriteoffBoth.Checked) {
				query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number);
				query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number);
			}
			else {
				query.AddColumn("Writeoff",writeoffwidth,FieldValueType.Number);
			}
			query.AddColumn("Tot Prod",totprodwidth,FieldValueType.Number);
			query.AddColumn("Pt Income",ptincomewidth,FieldValueType.Number);
			if(checkUnearned.Checked) {
				query.AddColumn("Unearned Pt Income",unearnedPtIncomeWidth,FieldValueType.Number);
			}
			query.AddColumn("Ins Income",insincomewidth,FieldValueType.Number);
			query.AddColumn("Total Income",totincomewidth,FieldValueType.Number);
			if(PrefC.HasClinicsEnabled && listClin.SelectedIndices.Count>1 && checkClinicBreakdown.Checked) {
				//If more than one clinic selected, we want to add a table to the end of the report that totals all the clinics together.
				query=report.AddQuery(dt,"Totals","",SplitByKind.None,2,true);
				query.AddColumn("Provider",provwidth,FieldValueType.String);
				query.AddColumn("Production",productionwidth,FieldValueType.Number);
				query.AddColumn("Adjustments",adjwidth,FieldValueType.Number);
				if(radioWriteoffBoth.Checked) {
					query.AddColumn("Writeoff Est",writeoffestwidth,FieldValueType.Number);
					query.AddColumn("Writeoff Adj",writeoffadjwidth,FieldValueType.Number);
				}
				else {
					query.AddColumn("Writeoff",writeoffwidth,FieldValueType.Number);
				}
				query.AddColumn("Tot Prod",totprodwidth,FieldValueType.Number);
				query.AddColumn("Pt Income",ptincomewidth,FieldValueType.Number);
				if(checkUnearned.Checked) {
					query.AddColumn("Unearned Pt Income",unearnedPtIncomeWidth,FieldValueType.Number);
				}
				query.AddColumn("Ins Income",insincomewidth,FieldValueType.Number);
				query.AddColumn("Total Income",totincomewidth,FieldValueType.Number);
			}
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
			dateFrom=PIn.Date(textDateFrom.Text);
			dateTo=PIn.Date(textDateTo.Text);
			if(dateTo<dateFrom) {
				MsgBox.Show(this,"To date cannot be before From date.");
				return;
			}
			if(radioDaily.Checked){
				RunDaily();
			}
			else if(radioMonthly.Checked){
				RunMonthly();
			}
			else if(radioAnnual.Checked) {
				RunAnnual();
			}
			else {//Provider
				RunProvider();
			}
			//DialogResult=DialogResult.OK;//Stay here so that a series of similar reports can be run
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}








