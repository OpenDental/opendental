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
	public class FormRpAccountingProfitLoss:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar monthCalendarStart;
		private System.Windows.Forms.MonthCalendar monthCalendarEnd;
		private System.Windows.Forms.Label labelTO;
		private System.ComponentModel.Container components = null;
		//private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpAccountingProfitLoss() {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpAccountingProfitLoss));
			this.monthCalendarStart = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.monthCalendarEnd = new System.Windows.Forms.MonthCalendar();
			this.SuspendLayout();
			// 
			// monthCalendarStart
			// 
			this.monthCalendarStart.Location = new System.Drawing.Point(31, 36);
			this.monthCalendarStart.MaxSelectionCount = 1;
			this.monthCalendarStart.Name = "monthCalendarStart";
			this.monthCalendarStart.TabIndex = 1;
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
			this.butCancel.Location = new System.Drawing.Point(422, 219);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(341, 219);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// monthCalendarEnd
			// 
			this.monthCalendarEnd.Location = new System.Drawing.Point(271, 36);
			this.monthCalendarEnd.Name = "monthCalendarEnd";
			this.monthCalendarEnd.TabIndex = 23;
			// 
			// FormRpAccountingProfitLoss
			// 
			this.ClientSize = new System.Drawing.Size(537, 258);
			this.Controls.Add(this.monthCalendarEnd);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.monthCalendarStart);
			this.Controls.Add(this.labelTO);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpAccountingProfitLoss";
			this.ShowInTaskbar = false;
			this.Text = "Profit Loss Report";
			this.Load += new System.EventHandler(this.FormRpAccountingProfitLoss_Load);
			this.ResumeLayout(false);

		}
		#endregion

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
