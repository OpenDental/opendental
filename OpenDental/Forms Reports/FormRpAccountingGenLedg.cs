using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpAccountingGenLedg:FormODBase {
		//private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpAccountingGenLedg() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpAccountingGenLedg_Load(object sender, System.EventArgs e) {
			if(DateTime.Today.Month>6){//default to this year
				date1.SelectionStart=new DateTime(DateTime.Today.Year,1,1);
				date2.SelectionStart=new DateTime(DateTime.Today.Year,12,31);
			}
			else{//default to last year
				date1.SelectionStart=new DateTime(DateTime.Today.Year-1,1,1);
				date2.SelectionStart=new DateTime(DateTime.Today.Year-1,12,31);
			}
		}

		///<summary>This report has never worked for Oracle.</summary>
		private void butOK_Click(object sender, System.EventArgs e) {
			//create the report
			ReportComplex report=new ReportComplex(true,false);
			DataTable data=Accounts.GetGeneralLedger(date1.SelectionStart,date2.SelectionStart);
			for(int i=0;i<data.Rows.Count;i++) {
				data.Rows[i]["Balance"]=ODR.Aggregate.RunningSumForAccounts(data.Rows[i]["AccountNum"],data.Rows[i]["DebitAmt"],data.Rows[i]["CreditAmt"],data.Rows[i]["AcctType"]);
			}
			Font font=new Font("Tahoma",7);
			Font fontTitle=new Font("Tahoma",9);
			Font fontSubTitle=new Font("Tahoma",8);
			report.ReportName="General Ledger";
			report.AddTitle("Title","Detail of General Ledger",fontTitle);
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",date1.SelectionStart.ToShortDateString()+" - "+date2.SelectionStart.ToShortDateString(),fontSubTitle);
			report.Sections[AreaSectionType.ReportHeader].Height-=20;
			//setup query
			QueryObject query;
			query=report.AddQuery(data,"Accounts","Description",SplitByKind.Value,1,true);
			query.GetGroupTitle().Font=new Font("Tahoma",8);
			// add columns to report
			query.AddColumn("Date",75,FieldValueType.Date,font);
			//query.GetColumnDetail("Date").SuppressIfDuplicate = true;
			query.GetColumnDetail("Date").StringFormat="d";
			query.AddColumn("Memo",175,FieldValueType.String,font);
			query.AddColumn("Splits",175,FieldValueType.String,font);
			query.AddColumn("Check",45,FieldValueType.String,font);
			query.AddColumn("Debit",70,FieldValueType.String,font);
			query.AddColumn("Credit",70,FieldValueType.String,font);
			query.AddColumn("Balance",70,FieldValueType.String,font);
			report.AddPageNum(font);
			report.AddGridLines();
			// execute query
			if(!report.SubmitQueries()) {
				return;
			}
			// display report
			using FormReportComplex FormR=new FormReportComplex(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}

		

		

		

	}
}
