using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public class FormRpAccountingBalanceSheet:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private System.ComponentModel.Container components = null;
		//private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpAccountingBalanceSheet() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpAccountingBalanceSheet));
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(31, 36);
			this.date1.MaxSelectionCount = 1;
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(28, 7);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 23);
			this.labelTO.TabIndex = 22;
			this.labelTO.Text = "As of";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(297, 219);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(297, 184);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRpAccountingBalanceSheet
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(412, 258);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpAccountingBalanceSheet";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Balance Sheet Report";
			this.Load += new System.EventHandler(this.FormRpAccountingBalanceSheet_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormRpAccountingBalanceSheet_Load(object sender, System.EventArgs e) {
			if(DateTime.Today.Month>6){//default to this year
				date1.SelectionStart=new DateTime(DateTime.Today.Year,12,31);
			}
			else{//default to last year
				date1.SelectionStart=new DateTime(DateTime.Today.Year-1,12,31);
			}
		}

		///<summary>This report has never worked for Oracle.</summary>
		private void butOK_Click(object sender, System.EventArgs e) {
			//create the report
			ReportComplex reportComplex=new ReportComplex(true,false);
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			DataTable tableAssets=Accounts.GetAssetTable(date1.SelectionStart);
			DataTable tableLiabilities=Accounts.GetLiabilityTable(date1.SelectionStart);
			DataTable tableEquity=Accounts.GetEquityTable(date1.SelectionStart);
			//Add two new rows to the equity data table to show Retained Earnings (Auto) and NetIncomeThisYear
			tableEquity.LoadDataRow(new object[] { "Retained Earnings (Auto)",Accounts.RetainedEarningsAuto(date1.SelectionStart) },LoadOption.OverwriteChanges);
			tableEquity.LoadDataRow(new object[] { "NetIncomeThisYear",Accounts.NetIncomeThisYear(date1.SelectionStart) },LoadOption.OverwriteChanges);
			reportComplex.ReportName="Balance Sheet";
			reportComplex.AddTitle("Title",Lan.g(this,"Balance Sheet"),fontTitle);
			reportComplex.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			reportComplex.AddSubTitle("Date",date1.SelectionStart.ToShortDateString(),fontSubTitle);
			//setup query
			QueryObject queryObject;
			queryObject=reportComplex.AddQuery(tableAssets,"Assets","",SplitByKind.None,1,true);
			// add columns to report
			queryObject.AddColumn("Description",300,FieldValueType.String,font);
			queryObject.AddColumn("Amount",150,FieldValueType.Number,font,"n0");
			queryObject.AddSummaryLabel("Amount","Total Assets",SummaryOrientation.West,false,fontBold);
			queryObject=reportComplex.AddQuery(tableLiabilities,"Liabilities","",SplitByKind.None,0,true);
			queryObject.AddLine("LiabilitiesLine",AreaSectionType.GroupHeader,LineOrientation.Horizontal,LinePosition.North,Color.Black,2,90,0,-30);
			// add columns to report
			queryObject.AddColumn("Description",300,FieldValueType.String,font);
			queryObject.AddColumn("Amount",150,FieldValueType.Number,font,"n0");
			queryObject.AddSummaryLabel("Amount","Total Liabilities",SummaryOrientation.West,false,fontBold);
			queryObject=reportComplex.AddQuery(tableEquity,"Equity","",SplitByKind.None,0,true);
			// add columns to report
			queryObject.AddColumn("Description",300,FieldValueType.String,font);
			queryObject.AddColumn("Amount",150,FieldValueType.Number,font,"n0");
			queryObject.AddSummaryLabel("Amount","Total Equity",SummaryOrientation.West,false,fontBold);
			queryObject.AddGroupSummaryField("Total (Equity+Liabilities):","Amount","SumTotal",SummaryOperation.Sum,color:Color.Black,
				font:fontSubTitle,offSetX:0,offSetY:25,formatString:"c0");
			reportComplex.AddPageNum(font);
			// execute query
			if(!reportComplex.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex formReportComplex=new FormReportComplex(reportComplex);
			//FormR.MyReport=report;
			formReportComplex.ShowDialog();
			font.Dispose();
			fontBold.Dispose();
			fontTitle.Dispose();
			fontSubTitle.Dispose();
			DialogResult=DialogResult.OK;
		}

		

		

		

	}
}
