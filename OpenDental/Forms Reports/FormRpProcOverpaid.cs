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
	public class FormRpProcOverpaid: FormODBase {
		private IContainer components;
		private ContextMenu contextMenuGrid;
		private MenuItem menuItemGoToAccount;
		private ImageList imageListCalendar;
		private UI.Button butRefresh;
		private UI.Button butClose;
		private UI.Button butPrint;
		private GridOD gridMain;
		private GroupBox groupBox2;
		private ODDateRangePicker dateRangePicker;
		private ComboBoxClinicPicker comboBoxMultiClinics;
		private ComboBoxOD comboBoxMultiProv;
		private Label labelProv;
		private UI.Button butCurrent;
		private UI.Button butAll;
		private UI.Button butFind;
		private TextBox textPatient;
		private Label labelPatient;
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

		///<summary></summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProcOverpaid));
			this.butClose = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.imageListCalendar = new System.Windows.Forms.ImageList(this.components);
			this.gridMain = new OpenDental.UI.GridOD();
			this.contextMenuGrid = new System.Windows.Forms.ContextMenu();
			this.menuItemGoToAccount = new System.Windows.Forms.MenuItem();
			this.butRefresh = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butCurrent = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butFind = new OpenDental.UI.Button();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.labelProv = new System.Windows.Forms.Label();
			this.comboBoxMultiProv = new OpenDental.UI.ComboBoxOD();
			this.comboBoxMultiClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(928, 658);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(12, 658);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 26);
			this.butPrint.TabIndex = 3;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// imageListCalendar
			// 
			this.imageListCalendar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCalendar.ImageStream")));
			this.imageListCalendar.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListCalendar.Images.SetKeyName(0, "arrowDownTriangle.gif");
			this.imageListCalendar.Images.SetKeyName(1, "arrowUpTriangle.gif");
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 71);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(991, 581);
			this.gridMain.TabIndex = 69;
			this.gridMain.Title = "Procedures Overpaid";
			this.gridMain.TranslationName = "TableProcedures";
			// 
			// contextMenuGrid
			// 
			this.contextMenuGrid.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemGoToAccount});
			// 
			// menuItemGoToAccount
			// 
			this.menuItemGoToAccount.Index = 0;
			this.menuItemGoToAccount.Text = "Go To Account";
			this.menuItemGoToAccount.Click += new System.EventHandler(this.menuItemGridGoToAccount_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(928, 39);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 26);
			this.butRefresh.TabIndex = 73;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.butCurrent);
			this.groupBox2.Controls.Add(this.butAll);
			this.groupBox2.Controls.Add(this.butFind);
			this.groupBox2.Controls.Add(this.textPatient);
			this.groupBox2.Controls.Add(this.labelPatient);
			this.groupBox2.Controls.Add(this.labelProv);
			this.groupBox2.Controls.Add(this.comboBoxMultiProv);
			this.groupBox2.Controls.Add(this.comboBoxMultiClinics);
			this.groupBox2.Controls.Add(this.dateRangePicker);
			this.groupBox2.Location = new System.Drawing.Point(12, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(910, 61);
			this.groupBox2.TabIndex = 77;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filters";
			// 
			// butCurrent
			// 
			this.butCurrent.Location = new System.Drawing.Point(698, 31);
			this.butCurrent.Name = "butCurrent";
			this.butCurrent.Size = new System.Drawing.Size(63, 24);
			this.butCurrent.TabIndex = 78;
			this.butCurrent.Text = "Current";
			this.butCurrent.Click += new System.EventHandler(this.butCurrent_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(841, 31);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(63, 24);
			this.butAll.TabIndex = 77;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butFind
			// 
			this.butFind.Location = new System.Drawing.Point(770, 31);
			this.butFind.Name = "butFind";
			this.butFind.Size = new System.Drawing.Size(63, 24);
			this.butFind.TabIndex = 76;
			this.butFind.Text = "Find";
			this.butFind.Click += new System.EventHandler(this.butFind_Click);
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(698, 11);
			this.textPatient.Name = "textPatient";
			this.textPatient.Size = new System.Drawing.Size(206, 20);
			this.textPatient.TabIndex = 75;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(631, 15);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(65, 13);
			this.labelPatient.TabIndex = 74;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelProv
			// 
			this.labelProv.Location = new System.Drawing.Point(412, 15);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(55, 16);
			this.labelProv.TabIndex = 73;
			this.labelProv.Text = "Providers";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxMultiProv
			// 
			this.comboBoxMultiProv.BackColor = System.Drawing.SystemColors.Window;
			this.comboBoxMultiProv.Location = new System.Drawing.Point(469, 12);
			this.comboBoxMultiProv.Name = "comboBoxMultiProv";
			this.comboBoxMultiProv.SelectionModeMulti = true;
			this.comboBoxMultiProv.IncludeAll = true;
			this.comboBoxMultiProv.Size = new System.Drawing.Size(160, 21);
			this.comboBoxMultiProv.TabIndex = 72;
			this.comboBoxMultiProv.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiProv_SelectionChangeCommitted);
			// 
			// comboBoxMultiClinics
			// 
			this.comboBoxMultiClinics.IncludeAll = true;
			this.comboBoxMultiClinics.IncludeUnassigned = true;
			this.comboBoxMultiClinics.Location = new System.Drawing.Point(432, 34);
			this.comboBoxMultiClinics.Name = "comboBoxMultiClinics";
			this.comboBoxMultiClinics.SelectionModeMulti = true;
			this.comboBoxMultiClinics.Size = new System.Drawing.Size(197, 21);
			this.comboBoxMultiClinics.TabIndex = 71;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Location = new System.Drawing.Point(5, 11);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 22);
			this.dateRangePicker.TabIndex = 0;
			// 
			// FormRpProcOverpaid
			// 
			this.AcceptButton = this.butPrint;
			this.ClientSize = new System.Drawing.Size(1015, 696);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpProcOverpaid";
			this.Text = "Procedures Overpaid Report";
			this.Load += new System.EventHandler(this.FormProcOverpaid_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

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