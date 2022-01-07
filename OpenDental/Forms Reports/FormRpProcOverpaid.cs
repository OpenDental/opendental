using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpProcOverpaid: FormODBase {

		#region Private Variables
		///<summary>The selected patNum.  Can be 0 to include all.</summary>
		private long _patNum;
		private ReportComplex _myReport;
		private DateTime _myReportDateFrom;
		private DateTime _myReportDateTo;
		private const int _colWidthPatName=125;
		private const int _colWidthProcDate=80;
		private const int _colWidthProcCode=50;
		private const int _colWidthProcTth=25;
		private const int _colWidthProv=85;
		private const int _colWidthFee=75;
		private const int _colWidthInsPay=75;
		private const int _colWidthWO=75;
		private const int _colWidthPtPaid=75;
		private const int _colWidthAdj=70;
		private const int _colWidthOverpay=85;
		#endregion

		///<summary></summary>
		public FormRpProcOverpaid() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormProcOverpaid_Load(object sender,System.EventArgs e) {
			gridMain.ContextMenu=contextMenuGrid;
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
			_patNum=FormOpenDental.CurPatNum;
			if(_patNum>0) {
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
			FillProvs();
			FillGrid();
		}

		private void FillGrid() {
			RefreshReport();
			gridMain.BeginUpdate();
			if(gridMain.ListGridColumns.Count==0) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),_colWidthPatName,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date"),_colWidthProcDate,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Code"),_colWidthProcCode,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Tth"),_colWidthProcTth,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Prov"),_colWidthProv,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Fee"),_colWidthFee,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Ins Paid"),_colWidthInsPay,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Write-off"),_colWidthWO,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Pt Paid"),_colWidthPtPaid,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Adjust"),_colWidthAdj,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Overpayment"),_colWidthOverpay,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_myReport.ReportObjects.Count;i++) {
				if(_myReport.ReportObjects[i].ObjectType!=ReportObjectType.QueryObject) {
					continue;
				}
				QueryObject queryObj=(QueryObject)_myReport.ReportObjects[i];
				for(int j = 0;j<queryObj.ReportTable.Rows.Count;j++) {
					DataRow rowCur=queryObj.ReportTable.Rows[j];
					row=new GridRow();
					row.Cells.Add(rowCur["patientName"].ToString());
					row.Cells.Add(PIn.Date(rowCur["ProcDate"].ToString()).ToShortDateString());
					row.Cells.Add(PIn.String(rowCur["ProcCode"].ToString()));
					row.Cells.Add(PIn.String(rowCur["ToothNum"].ToString()));
					row.Cells.Add(PIn.String(rowCur["Abbr"].ToString()));
					row.Cells.Add(PIn.Double(rowCur["fee"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["insPaid"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["wo"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["ptPaid"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["adjAmt"].ToString()).ToString("c"));
					row.Cells.Add(PIn.Double(rowCur["overpay"].ToString()).ToString("c"));
					row.Tag=rowCur;
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void FillProvs() {
			comboBoxMultiProv.Items.Clear();
			comboBoxMultiProv.Items.AddProvsFull(Providers.GetListReports());
			comboBoxMultiProv.IsAllSelected=true;
		}

		private void RefreshReport() {
			bool hasValidationPassed=ValidateFields();
			DataTable tableOverpaidProcs=new DataTable();
			List<long> listSelectedProvNums=comboBoxMultiProv.GetSelectedProvNums();
			if(hasValidationPassed) {
				tableOverpaidProcs=RpProcOverpaid.GetOverPaidProcs(_patNum,listSelectedProvNums,comboBoxMultiClinics.ListSelectedClinicNums,_myReportDateFrom,_myReportDateTo);
			}
			string subTitleProviders=Lan.g(this,"All Providers");
			if(listSelectedProvNums.Count>0) {
				subTitleProviders=Lan.g(this,"For Providers:")+" "+string.Join(",",listSelectedProvNums.Select(x => Providers.GetFormalName(x)));
			}
			string subtitleClinics=comboBoxMultiClinics.GetStringSelectedClinics();
			_myReport=new ReportComplex(true,false);
			_myReport.ReportName=Lan.g(this,"Overpaid Procedures");
			_myReport.AddTitle("Title",Lan.g(this,"Overpaid Procedures"));
			_myReport.AddSubTitle("Practice Name",PrefC.GetString(PrefName.PracticeTitle));
			if(_myReportDateFrom==_myReportDateTo) {
				_myReport.AddSubTitle("Report Dates",_myReportDateFrom.ToShortDateString());
			}
			else {
				_myReport.AddSubTitle("Report Dates",_myReportDateFrom.ToShortDateString()+" - "+_myReportDateTo.ToShortDateString());
			}
			if(_patNum>0) {
				_myReport.AddSubTitle("Patient",Patients.GetLim(_patNum).GetNameFL());
			}
			_myReport.AddSubTitle("Providers",subTitleProviders);
			if(PrefC.HasClinicsEnabled) {
				_myReport.AddSubTitle("Clinics",subtitleClinics);
			}
			QueryObject query=_myReport.AddQuery(tableOverpaidProcs,DateTime.Today.ToShortDateString());
			query.AddColumn("Patient Name",_colWidthPatName,FieldValueType.String);
			query.AddColumn("Date",_colWidthProcDate,FieldValueType.Date);
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Code",_colWidthProcCode,FieldValueType.String);
			query.AddColumn("Tth",_colWidthProcTth,FieldValueType.String);
			query.AddColumn("Prov",_colWidthProv,FieldValueType.String);
			query.AddColumn("Fee",_colWidthFee,FieldValueType.Number);
			query.AddColumn("Ins Paid",_colWidthInsPay,FieldValueType.Number);
			query.AddColumn("Write-off",_colWidthWO,FieldValueType.Number);
			query.AddColumn("Pt Paid",_colWidthPtPaid,FieldValueType.Number);
			query.AddColumn("Adjust",_colWidthAdj,FieldValueType.Number);
			query.AddColumn("Overpayment",_colWidthOverpay,FieldValueType.Number);
			_myReport.AddPageNum();
			_myReport.SubmitQueries();
		}

		private bool ValidateFields() {
			_myReportDateFrom=dateRangePicker.GetDateTimeFrom();
			_myReportDateTo=dateRangePicker.GetDateTimeTo();
			if(_myReportDateFrom==DateTime.MinValue || _myReportDateTo==DateTime.MinValue || _myReportDateFrom>_myReportDateTo) {
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
			_patNum=FormOpenDental.CurPatNum;
			if(_patNum==0) {
				textPatient.Text="";
			}
			else {
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_patNum=formPatientSelect.SelectedPatNum;
			textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
		}

		private void butAll_Click(object sender,EventArgs e) {
			_patNum=0;
			textPatient.Text="";
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			using FormReportComplex FormR=new FormReportComplex(_myReport);
			FormR.ShowDialog();
		}

		private void menuItemGridGoToAccount_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one patient.");
				return;
			}
			DataRow row=(DataRow)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			long patNum=PIn.Long(row["PatNum"].ToString());
			GotoModule.GotoAccount(patNum);
			SendToBack();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}