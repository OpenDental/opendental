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
	public partial class FormRpAccountingBalanceSheet:FormODBase {
		//private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpAccountingBalanceSheet() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

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
