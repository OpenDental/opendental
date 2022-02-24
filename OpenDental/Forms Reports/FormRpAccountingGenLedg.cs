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
	public class FormRpAccountingGenLedg:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private System.ComponentModel.Container components = null;
		//private FormQuery FormQuery2;

		///<summary></summary>
		public FormRpAccountingGenLedg() {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpAccountingGenLedg));
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(285, 36);
			this.date2.MaxSelectionCount = 1;
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
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
			this.labelTO.Location = new System.Drawing.Point(234, 44);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 23);
			this.labelTO.TabIndex = 22;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(534, 291);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(534, 256);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRpAccountingGenLedg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(649, 330);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpAccountingGenLedg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "General Ledger Report";
			this.Load += new System.EventHandler(this.FormRpAccountingGenLedg_Load);
			this.ResumeLayout(false);

		}
		#endregion

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
