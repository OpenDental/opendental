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
	public partial class FormRpAccountingProfitLoss:FormODBase {
		//private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpAccountingProfitLoss() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpAccountingProfitLoss_Load(object sender, System.EventArgs e) {
			if(DateTime.Today.Month>6){//default to this year
				monthCalendarStart.SelectionStart=new DateTime(DateTime.Today.Year,1,1);
				monthCalendarEnd.SelectionStart=new DateTime(DateTime.Today.Year,12,31);
			}
			else{//default to last year
				monthCalendarStart.SelectionStart=new DateTime(DateTime.Today.Year-1,1,1);
				monthCalendarEnd.SelectionStart=new DateTime(DateTime.Today.Year-1,12,31);
			}
		}

		///<summary>This report has never worked for Oracle.</summary>
		private void butOK_Click(object sender, System.EventArgs e) {
			//create the report
			ReportComplex report=new ReportComplex(true,false);
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			DataTable tableIncome=Accounts.GetAccountTotalByType(monthCalendarStart.SelectionStart,monthCalendarEnd.SelectionStart, AccountType.Income);
			DataTable tableExpenses=Accounts.GetAccountTotalByType(monthCalendarStart.SelectionStart,monthCalendarEnd.SelectionStart, AccountType.Expense);
			report.ReportName="Profit & Loss Statement";
			report.AddTitle("Title",Lan.g(this,"Profit & Loss Statement"),fontTitle);
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",monthCalendarStart.SelectionStart.ToShortDateString()+" - "+monthCalendarEnd.SelectionStart.ToShortDateString(),fontSubTitle);
			//setup query
			QueryObject query;
			query=report.AddQuery(tableIncome,"Income","",SplitByKind.None,0,true);
			// add columns to report
			query.AddColumn("Description",300,FieldValueType.String,font);
			query.AddColumn("Amount",150,FieldValueType.Number,font);
			query.AddSummaryLabel("Amount","Total Income",SummaryOrientation.West,false,fontBold);
			query=report.AddQuery(tableExpenses,"Expense","",SplitByKind.None,0,true);
			query.IsNegativeSummary=true;
			// add columns to report
			query.AddColumn("Description",300,FieldValueType.String,font);
			query.AddColumn("Amount",150,FieldValueType.Number,font);
			query.AddSummaryLabel("Amount","Total Expense",SummaryOrientation.West,false,fontBold);
			query.AddGroupSummaryField("Net Income:","Amount","SumTotal",SummaryOperation.Sum,color:Color.Black,font:fontSubTitle,offSetX:0,offSetY:25);
			report.AddPageNum(font);
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();
			font.Dispose();
			fontBold.Dispose();
			fontTitle.Dispose();
			fontSubTitle.Dispose();
			DialogResult=DialogResult.OK;
		}
	}
}
