using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;


namespace OpenDental {
	///<summary></summary>
	public partial class FormRpDPPOvercharged: FormODBase {
		#region Private Variables
		///<summary>The selected patNum.  Can be 0 to include all.</summary>
		private long _patNum;
		private ReportComplex _reportCur;
		private DateTime _dateFrom;
		private DateTime _dateTo;
		#endregion

		///<summary></summary>
		public FormRpDPPOvercharged() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpDPPOvercharged_Load(object sender,System.EventArgs e) {
			gridMain.ContextMenu=contextMenuGrid;
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
			_patNum=FormOpenDental.PatNumCur;
			if(_patNum>0) {
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
			if(!PrefC.HasClinicsEnabled) {
				comboBoxMultiClinics.Visible=false;
			}
			else {
				comboBoxMultiClinics.IsAllSelected=true;
			}
			FillProvs();
		}

		///<summary>FillGrid() must be left in shown so the form doesn't pop behind when the progress bar is shown.</summary>
		private void FormRpDPPOvercharged_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			#region Column Sizes
			int colWidthStartDate=65;
			int colWidthNames=125;
			int colWidthPovAndClin=90;
			int colAmtsAndDescript=70;
			int colWidthWideAmts=80;
			int colWidthOverriden=64;
			#endregion
		
			RefreshReport();
			gridMain.BeginUpdate();
			if(gridMain.Columns.Count==0) {
				#region Set Column Header Values
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Start Date"),
					colWidthStartDate,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Patient"),colWidthNames,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Guarantor"),colWidthNames,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Provider"),
					colWidthPovAndClin,HorizontalAlignment.Center,GridSortingStrategy.StringCompare));
				if(PrefC.HasClinicsEnabled) {//Only show when clinics are on
					gridMain.Columns.Add(new GridColumn(Lan.g(this,"Clinic"),
						colWidthPovAndClin,HorizontalAlignment.Center,GridSortingStrategy.StringCompare));
				}
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Description"),
					colAmtsAndDescript,HorizontalAlignment.Center,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Overridden"),
					colWidthOverriden,HorizontalAlignment.Center,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Pat Portion"),
					colAmtsAndDescript,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Pat Paid Outside Plan"),
					colWidthWideAmts,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Pat Portion On Plan"),
					colAmtsAndDescript,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Plan Debits"),
					colAmtsAndDescript,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Overcharged"),
					colWidthWideAmts,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Pat Paid\r\nOn Plan"),
					colAmtsAndDescript,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Pat Overpaid"),
					colWidthWideAmts,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				#endregion
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			//Null coalescing in case _reportCur has not been loaded, meaning no dynamic payment plans to load.
			//If there are no dynamic payment plans to load, just show an empty grid.
			for(int i=0;i<_reportCur?.ReportObjects.Count;i++) {
				if(_reportCur.ReportObjects[i].ObjectType!=ReportObjectType.QueryObject) {
					continue;
				}
				QueryObject queryObj=(QueryObject)_reportCur.ReportObjects[i];
				for(int j=0;j<queryObj.ReportTable.Rows.Count;j++) {
					DataRow rowCur=queryObj.ReportTable.Rows[j];
					row=new GridRow();
					#region Set Row Values from Query Object
					row.Cells.Add(PIn.Date(rowCur["DatePayPlanStart"].ToString()).ToShortDateString());
					row.Cells.Add(PIn.String(rowCur["patientName"].ToString()));
					row.Cells.Add(PIn.String(rowCur["guarName"].ToString()));
					row.Cells.Add(PIn.String(rowCur["provAbbr"].ToString()));
					if(PrefC.HasClinicsEnabled) {//Only show if clinics are on
						row.Cells.Add(PIn.String(rowCur["clinicAbbr"].ToString()));
					}
					row.Cells.Add(PIn.String(rowCur["description"].ToString()));
					row.Cells.Add(PIn.String(rowCur["overridden"].ToString()));
					row.Cells.Add(PIn.Double(rowCur["patPortion"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["patPaidOutsidePlan"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["patPortionOnPlan"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["planDebits"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["amtOvercharged"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["patPaidOnPlan"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["amtOverpaid"].ToString()).ToString("c"));
					#endregion
					row.Tag=rowCur;
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void FillProvs() {
			comboBoxMultiProv.IncludeAll=true;
			foreach(Provider provCur in Providers.GetListReports()) {
				comboBoxMultiProv.Items.Add(provCur.GetLongDesc(),provCur);
			}
			comboBoxMultiProv.IsAllSelected=true;
		}

		///<summary>Used to fill the grid and populate the printed report.</summary>
		private void RefreshReport() {
			bool hasValidationPassed=ValidateDateFields();
			DataTable tableOverchargedDPP=new DataTable();
			if(hasValidationPassed) {
				ProgressWin progressOD=new ProgressWin();
				progressOD.ActionMain=() => { 
					tableOverchargedDPP=RpDPPOvercharged.GetDPPOvercharged(_dateFrom,_dateTo,comboBoxMultiClinics.ListClinicNumsSelected,
						comboBoxMultiProv.GetSelectedProvNums(),_patNum);
				};
				progressOD.ShowDialog();
				if(progressOD.IsCancelled){
					return;
				}
			}
			string subTitleProviders=Lan.g(this,"All Providers");
			if(!comboBoxMultiProv.IsAllSelected) {
				subTitleProviders=Lan.g(this,"For Providers:")+" "+string.Join(",",comboBoxMultiProv.GetSelectedProvNums().Select(x => Providers.GetFormalName(x)));
			}
			string subtitleClinics="";
			if(PrefC.HasClinicsEnabled){//Do not show if clinics are not on
				subtitleClinics=comboBoxMultiClinics.GetStringSelectedClinics();
			}
			#region Report Logic
			//This report will never show progress for printing.  This is because the report is being rebuilt whenever the grid is refreshed.
			_reportCur=new ReportComplex(true,true,false);
			_reportCur.ReportName=Lan.g(this,"Overcharged Payment Plans");
			_reportCur.AddTitle("Title",Lan.g(this,"Overcharged Payment Plans"));
			_reportCur.AddSubTitle("Practice Name",PrefC.GetString(PrefName.PracticeTitle));
			if(_dateFrom==_dateTo) {
				_reportCur.AddSubTitle("Report Dates",_dateFrom.ToShortDateString());
			}
			else {
				_reportCur.AddSubTitle("Report Dates",_dateFrom.ToShortDateString()+" - "+_dateTo.ToShortDateString());
			}
			if(_patNum>0) {
				_reportCur.AddSubTitle("Patient",Patients.GetLim(_patNum).GetNameFL());
			}
			_reportCur.AddSubTitle("Providers",subTitleProviders);
			if(PrefC.HasClinicsEnabled) {//Do not show if clinics are not on
				_reportCur.AddSubTitle("Clinics",subtitleClinics);
			}
			#region Col Sizes
			int colWidthStartDate=71;
			int colWidthNames=115;
			int colWidthProvClinDescript=70;
			int colWidthOverriden=41;
			int colWidthAmt=70;
			int colWidthWideAmt=78;
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			#endregion
			QueryObject query=_reportCur.AddQuery(tableOverchargedDPP,DateTime.Today.ToShortDateString());
			query.AddColumn("Start Date",colWidthStartDate,FieldValueType.Date);
			query.AddColumn("Patient",colWidthNames,FieldValueType.String);
			query.AddColumn("Guarantor",colWidthNames,FieldValueType.String);
			query.AddColumn("Provider",colWidthProvClinDescript,FieldValueType.String);
			if(PrefC.HasClinicsEnabled) {
				query.AddColumn("Clinic",colWidthProvClinDescript,FieldValueType.String);
			}
			query.AddColumn("Description",colWidthProvClinDescript,FieldValueType.String);
			query.AddColumn("Over-ridden",colWidthOverriden,FieldValueType.String);
			query.GetColumnDetail("Over-ridden").ContentAlignment=ContentAlignment.MiddleCenter;
			query.AddColumn("Pat\r\nPortion",colWidthAmt,FieldValueType.Number);
			query.AddColumn("Pat Paid Outside Plan",colWidthWideAmt,FieldValueType.Number);
			query.AddColumn("Pat Portion On Plan",colWidthAmt,FieldValueType.Number,fontBold);
			query.AddColumn("Plan\r\nDebits",colWidthAmt,FieldValueType.Number);
			query.AddColumn("Overcharged",colWidthWideAmt,FieldValueType.Number,fontBold);
			query.AddColumn("Pat Paid On Plan",colWidthAmt,FieldValueType.Number);
			query.AddColumn("Pat Overpaid",colWidthAmt,FieldValueType.Number,fontBold);
			_reportCur.AddPageNum();
			_reportCur.SubmitQueries();
			#endregion
		}

		///<summary>Ensures that _dateForm and _dateTo are not equal to DateTime.MinVal and that _dateFrom is greater than _dateTo.</summary>
		private bool ValidateDateFields() {
			_dateFrom=dateRangePicker.GetDateTimeFrom();
			_dateTo=dateRangePicker.GetDateTimeTo();
			if(_dateFrom==DateTime.MinValue || _dateTo==DateTime.MinValue || _dateFrom>_dateTo) {
				return false;
			}
			return true;
		}

		private void comboBoxMultiProv_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBoxMultiProv.SelectedIndices.Count==0) {
				comboBoxMultiProv.IsAllSelected=true;
			}
		}

		private void butCurrent_Click(object sender,EventArgs e) {
			_patNum=FormOpenDental.PatNumCur;
			if(_patNum==0) {
				textPatient.Text="";
			}
			else {
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
		}

		private void butFind_Click(object sender,EventArgs e) {
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel) {
				return;
			}
			_patNum=frmPatientSelect.PatNumSelected;
			textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
		}

		private void butFix_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"Selected Overcharged Productions will have offsetting debits created. Continue?")) {
				return;
			}
			List<GridRow> listSelectedGridRows=gridMain.SelectedGridRows;
			//If no rows are selected, select them all
			if(listSelectedGridRows.Count==0) {
				listSelectedGridRows=gridMain.ListGridRows;
			}
			List<PayPlanCharge> listChargesToInsert=new List<PayPlanCharge>();
			List<long> listPatNums=listSelectedGridRows.Select(x => (DataRow)x.Tag)
				.Select(x => PIn.Long(x["PatNum"].ToString()))
				.ToList();
			long[] longArrPayPlanNums=listSelectedGridRows.Select(x => (DataRow)x.Tag)
				.Select(y => PIn.Long(y["PayPlanNum"].ToString()))
				.ToArray();
			List<Family> listFamilies=Patients.GetFamilies(listPatNums);
			List<PayPlan> listPayPlans=PayPlans.GetMany(longArrPayPlanNums);
			for(int i = 0;i<listSelectedGridRows.Count;i++) {
				DataRow dataRow=(DataRow)listSelectedGridRows[i].Tag;
				PayPlan payPlan=listPayPlans.ToList().Find(x => x.PayPlanNum==PIn.Long(dataRow["PayPlanNum"].ToString()));
				Family family=listFamilies.Find(x => x.ListPats.Select(x => x.PatNum).Contains(PIn.Long(dataRow["PatNum"].ToString())));
				if(family==null || payPlan==null) {
					continue;
				}
				//Create Negative PayPlan Charge that counters the debit
				PayPlanCharge payPlanChargeOffset=PayPlanEdit.CreateDebitChargeDynamic(payPlan,
					family,
					PIn.Long(dataRow["ProvNum"].ToString()),
					PIn.Long(dataRow["ClinicNum"].ToString()),
					principalAmt:-PIn.Double(dataRow["amtOverCharged"].ToString()),
					interestAmt:0,
					dateCharge:DateTime.Now,
					note:"Offsetting overcharge.",
					PIn.Long(dataRow["FKey"].ToString()),
					PIn.Enum<PayPlanLinkType>(dataRow["LinkType"].ToString())
				);
				listChargesToInsert.Add(payPlanChargeOffset);
			}
			PayPlanCharges.InsertMany(listChargesToInsert);
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			_patNum=0;
			textPatient.Text="";
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			using FormReportComplex FormR=new FormReportComplex(_reportCur);
			FormR.ShowDialog();
		}

		private void menuItemGridGoToAccount_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one patient.");
				return;
			}
			DataRow row=(DataRow)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			long patNum=PIn.Long(row["patNum"].ToString());
			GlobalFormOpenDental.GoToModule(EnumModuleType.Account,patNum:patNum);
			SendToBack();
		}

	}
}