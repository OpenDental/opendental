using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpAccountingGenLedg:FormODBase {
		private decimal _runningSum;
		private long _accountNumRunningSum;

		///<summary></summary>
		public FormRpAccountingGenLedg() {
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormRpAccountingGenLedg_Load(object sender, System.EventArgs e) {
			if(DateTime.Today.Month>6){//default to this year
				monthCalendarStart.SetDateSelected(new DateTime(DateTime.Today.Year,1,1));
				monthCalendarEnd.SetDateSelected(new DateTime(DateTime.Today.Year,12,31));
			}
			else{//default to last year
				monthCalendarStart.SetDateSelected(new DateTime(DateTime.Today.Year-1,1,1));
				monthCalendarEnd.SetDateSelected(new DateTime(DateTime.Today.Year-1,12,31));
			}
		}

		///<summary></summary>
		private decimal CalcRunningSum(long accountNum,string debitAmt,string creditAmt,AccountType accountType) {
			if(debitAmt==null || creditAmt==null) {
				return 0;//.ToString("N");
			}
			decimal debit;
			decimal credit;
			//Cannot read debitAmt and creditAmt as decimals because it makes the general ledger detail report fail.  Simply cast as decimals when doing mathematical operations.
			try {
				debit=PIn.Decimal(debitAmt);
				credit=PIn.Decimal(creditAmt);
			}
			catch {
				return 0;
			}
			if(accountNum!=_accountNumRunningSum) {//if new or changed group
				_runningSum=0;
			}
			_accountNumRunningSum=accountNum;
			if(accountType.In(AccountType.Asset,AccountType.Expense)) {
				_runningSum+=debit-credit;
			}
			else{
				_runningSum+=credit-debit;
			}
			return _runningSum;
		}

		///<summary></summary>
		private void butOK_Click(object sender, System.EventArgs e) {
			//calculate RE historical bal
			decimal balanceRE=Accounts.GetRE_PreviousYears(monthCalendarEnd.GetDateSelected());
			Account accountRE=Accounts.GetFirstOrDefault(x=>x.IsRetainedEarnings);
			//guaranteed to always be exactly 1, so we don't check for null
			long accountNumRE=accountRE.AccountNum;
			//create the report
			ReportComplex report=new ReportComplex(true,false);
			DataTable data=Accounts.GetGeneralLedger(monthCalendarStart.GetDateSelected(),monthCalendarEnd.GetDateSelected());
			for(int i=0;i<data.Rows.Count;i++) {
				long accountNum=PIn.Long(data.Rows[i]["AccountNum"].ToString());
				AccountType accountType=PIn.Enum<AccountType>(data.Rows[i]["AcctType"].ToString());
				decimal balance=CalcRunningSum(accountNum,
					data.Rows[i]["DebitAmt"].ToString(),
					data.Rows[i]["CreditAmt"].ToString(),
					accountType);
				if(accountNum==accountNumRE) {
					balance+=balanceRE;
				}
				data.Rows[i]["Balance"]=balance.ToString("N");
			}
			Font font=new Font("Tahoma",7);
			Font fontTitle=new Font("Tahoma",9);
			Font fontSubTitle=new Font("Tahoma",8);
			report.ReportName="General Ledger";
			report.AddTitle("Title","Detail of General Ledger",fontTitle);
			report.AddSubTitle("PracName",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",monthCalendarStart.GetDateSelected().ToShortDateString()+" - "
				+monthCalendarEnd.GetDateSelected().ToShortDateString(),fontSubTitle);
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