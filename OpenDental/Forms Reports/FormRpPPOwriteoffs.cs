using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System.Data;
using CodeBase;
using System.Collections.Generic;

//using System.IO;
//using System.Text;
//using System.Xml.Serialization;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormRpPPOwriteoffs : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private MonthCalendar date2;
		private MonthCalendar date1;
		private Label labelTO;
		private RadioButton radioIndividual;
		private RadioButton radioGroup;
		private TextBox textCarrier;
		private Label label1;
		private GroupBox groupBox3;
		private Label label5;
		private RadioButton radioWriteoffProcDate;
		private RadioButton radioWriteoffInsPayDate;
		private RadioButton radioWriteoffClaimDate;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		public FormRpPPOwriteoffs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.C("All", new System.Windows.Forms.Control[] {
				butOK,
				butCancel,
			});
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPPOwriteoffs));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.radioIndividual = new System.Windows.Forms.RadioButton();
			this.radioGroup = new System.Windows.Forms.RadioButton();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioWriteoffClaimDate = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioWriteoffProcDate = new System.Windows.Forms.RadioButton();
			this.radioWriteoffInsPayDate = new System.Windows.Forms.RadioButton();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(597, 330);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(597, 298);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(289, 36);
			this.date2.MaxSelectionCount = 1;
			this.date2.Name = "date2";
			this.date2.TabIndex = 30;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(33, 36);
			this.date1.MaxSelectionCount = 1;
			this.date1.Name = "date1";
			this.date1.TabIndex = 29;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(249, 36);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(51, 23);
			this.labelTO.TabIndex = 31;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// radioIndividual
			// 
			this.radioIndividual.Checked = true;
			this.radioIndividual.Location = new System.Drawing.Point(33, 227);
			this.radioIndividual.Name = "radioIndividual";
			this.radioIndividual.Size = new System.Drawing.Size(200, 19);
			this.radioIndividual.TabIndex = 32;
			this.radioIndividual.TabStop = true;
			this.radioIndividual.Text = "Individual Claims";
			this.radioIndividual.UseVisualStyleBackColor = true;
			// 
			// radioGroup
			// 
			this.radioGroup.Location = new System.Drawing.Point(33, 250);
			this.radioGroup.Name = "radioGroup";
			this.radioGroup.Size = new System.Drawing.Size(200, 19);
			this.radioGroup.TabIndex = 33;
			this.radioGroup.Text = "Group by Carrier";
			this.radioGroup.UseVisualStyleBackColor = true;
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(33, 309);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(178, 20);
			this.textCarrier.TabIndex = 34;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(29, 283);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(257, 22);
			this.label1.TabIndex = 35;
			this.label1.Text = "Enter a few letters of the carrier to limit results";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioWriteoffClaimDate);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.radioWriteoffProcDate);
			this.groupBox3.Controls.Add(this.radioWriteoffInsPayDate);
			this.groupBox3.Location = new System.Drawing.Point(289, 227);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(281, 129);
			this.groupBox3.TabIndex = 48;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Show Insurance Write-offs";
			// 
			// radioWriteoffClaimDate
			// 
			this.radioWriteoffClaimDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaimDate.Location = new System.Drawing.Point(9, 62);
			this.radioWriteoffClaimDate.Name = "radioWriteoffClaimDate";
			this.radioWriteoffClaimDate.Size = new System.Drawing.Size(244, 40);
			this.radioWriteoffClaimDate.TabIndex = 3;
			this.radioWriteoffClaimDate.Text = "Using initial claim date for write-off estimates, insurance pay date for write-of" +
    "f adjustments";
			this.radioWriteoffClaimDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaimDate.UseVisualStyleBackColor = true;
			this.radioWriteoffClaimDate.CheckedChanged += new System.EventHandler(this.radioWriteoffClaimDate_CheckedChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 103);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(269, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "(This is discussed in the PPO section of the manual)";
			// 
			// radioWriteoffProcDate
			// 
			this.radioWriteoffProcDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProcDate.Location = new System.Drawing.Point(9, 41);
			this.radioWriteoffProcDate.Name = "radioWriteoffProcDate";
			this.radioWriteoffProcDate.Size = new System.Drawing.Size(244, 23);
			this.radioWriteoffProcDate.TabIndex = 1;
			this.radioWriteoffProcDate.Text = "Using procedure date.";
			this.radioWriteoffProcDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProcDate.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffInsPayDate
			// 
			this.radioWriteoffInsPayDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffInsPayDate.Checked = true;
			this.radioWriteoffInsPayDate.Location = new System.Drawing.Point(9, 20);
			this.radioWriteoffInsPayDate.Name = "radioWriteoffInsPayDate";
			this.radioWriteoffInsPayDate.Size = new System.Drawing.Size(244, 23);
			this.radioWriteoffInsPayDate.TabIndex = 0;
			this.radioWriteoffInsPayDate.TabStop = true;
			this.radioWriteoffInsPayDate.Text = "Using insurance payment date.";
			this.radioWriteoffInsPayDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffInsPayDate.UseVisualStyleBackColor = true;
			// 
			// FormRpPPOwriteoffs
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(697, 379);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.radioGroup);
			this.Controls.Add(this.radioIndividual);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpPPOwriteoffs";
			this.ShowInTaskbar = false;
			this.Text = "PPO Writeoffs";
			this.Load += new System.EventHandler(this.FormRpPPOwriteoffs_Load);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormRpPPOwriteoffs_Load(object sender, System.EventArgs e) {
			date1.SelectionStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(-1);
			date2.SelectionStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddDays(-1);
			switch(PrefC.GetInt(PrefName.ReportsPPOwriteoffDefaultToProcDate)) {
				case 0:	radioWriteoffInsPayDate.Checked=true; break;
				case 1:	radioWriteoffProcDate.Checked=true; break;
				case 2:	radioWriteoffClaimDate.Checked=true; break;
				default: radioWriteoffClaimDate.Checked=true; break;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(date2.SelectionStart<date1.SelectionStart) {
				MsgBox.Show(this,"End date cannot be before start date.");
				return;
			}
			if(radioIndividual.Checked){
				ExecuteIndividual();
			}
			else{
				ExecuteGroup();
			}
		}

		private void ExecuteIndividual(){
			Font font=new Font("Tahoma",9);
			Font fontBold=new Font("Tahoma",9,FontStyle.Bold);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex report=new ReportComplex(true,false);
			report.AddTitle("Title",Lan.g(this,"PPO Writeoffs"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",date1.SelectionStart.ToShortDateString()+" - "+date2.SelectionStart.ToShortDateString(),fontSubTitle);
			report.AddSubTitle("Claims",Lan.g(this,"Individual Claims"),fontSubTitle);
			if(textCarrier.Text!="") {
				report.AddSubTitle("Carrier",Lan.g(this,"Carrier like: ")+textCarrier.Text,fontSubTitle);
			}
			DataTable table=new DataTable();
			PPOWriteoffDateCalc writeoffType=GetWriteoffType();
			table=RpPPOwriteoff.GetWriteoffTable(date1.SelectionStart,date2.SelectionStart,radioIndividual.Checked,textCarrier.Text,writeoffType);
			QueryObject query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			query.AddColumn("Date",80,FieldValueType.Date,font);
			query.AddColumn("Patient",120,FieldValueType.String,font);
			query.AddColumn("Carrier",150,FieldValueType.String,font);
			query.AddColumn("Provider",60,FieldValueType.String,font);
			query.AddColumn("Stand Fee",80,FieldValueType.Number,font);
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				query.AddColumn("Writeoff Estimate",80,FieldValueType.Number,font);
				query.AddColumn("Writeoff Adjustment",80,FieldValueType.Number,font);
			}
			else {
				query.AddColumn("Writeoff",80,FieldValueType.Number,font);
			}
			query.AddColumn("PPO Fee",80,FieldValueType.Number,font);
			query.AddSummaryLabel("Stand Fee","Totals:",SummaryOrientation.West,false,fontBold);
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				query.AddGroupSummaryField("PPO Fee (Stand Fee + Writeoff Estimate + Writeoff Adjustment)","Stand Fee","PPO Fee",SummaryOperation.Sum,new List<int>(){1},Color.Black,new Font("Tahoma",9,FontStyle.Bold),0,50);
			}
			else {
				query.AddGroupSummaryField("PPO Fee (Stand Fee + Writeoff)","Stand Fee","PPO Fee",SummaryOperation.Sum,new List<int>(){1},Color.Black,new Font("Tahoma",9,FontStyle.Bold),0,50);
			}
			if(!report.SubmitQueries()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void ExecuteGroup() {
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			ReportComplex report=new ReportComplex(true,false);
			report.AddTitle("Title",Lan.g(this,"PPO Writeoffs"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date SubTitle",date1.SelectionStart.ToShortDateString()+" - "+date2.SelectionStart.ToShortDateString(),fontSubTitle);
			report.AddSubTitle("Claims",Lan.g(this,"Individual Claims"),fontSubTitle);
			if(textCarrier.Text!="") {
				report.AddSubTitle("Carrier",Lan.g(this,"Carrier like: ")+textCarrier.Text,fontSubTitle);
			}
			if(textCarrier.Text!="") {
				report.AddSubTitle("Carrier",Lan.g(this,"Carrier like: ")+textCarrier.Text,fontSubTitle);
			}
			PPOWriteoffDateCalc writeoffType=GetWriteoffType();
			DataTable table=RpPPOwriteoff.GetWriteoffTable(date1.SelectionStart,date2.SelectionStart,radioIndividual.Checked,textCarrier.Text,writeoffType);
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"),"",SplitByKind.None,1,true);
			query.AddColumn("Carrier",180,FieldValueType.String,font);
			query.AddColumn("Stand Fee",80,FieldValueType.Number,font);
			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				query.AddColumn("Writeoff Estimate",80,FieldValueType.Number,font);
				query.AddColumn("Writeoff Adjustment",80,FieldValueType.Number,font);
			}
			else {
				query.AddColumn("Writeoff",80,FieldValueType.Number,font);
			}
			query.AddColumn("PPO Fee",80,FieldValueType.Number,font);

			if(writeoffType==PPOWriteoffDateCalc.ClaimPayDate) {
				query.AddGroupSummaryField("PPO Fee (Stand Fee + Writeoff Estimate + Writeoff Adjustment)","PPO Fee","PPO Fee",SummaryOperation.Sum,new List<int>(){1},Color.Black,new Font("Tahoma",9,FontStyle.Bold),0,50);
			}
			else {
				query.AddGroupSummaryField("PPO Fee (Stand Fee + Writeoff)","PPO Fee","PPO Fee",SummaryOperation.Sum,new List<int>(){1},Color.Black,new Font("Tahoma",9,FontStyle.Bold),0,50);
			}
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private PPOWriteoffDateCalc GetWriteoffType() {
			if(radioWriteoffInsPayDate.Checked) {
				return PPOWriteoffDateCalc.InsPayDate;
			}
			else if(radioWriteoffProcDate.Checked){
				return PPOWriteoffDateCalc.ProcDate;
			}
			else {//radioWriteoffClaim.Checked is checked
				return PPOWriteoffDateCalc.ClaimPayDate;
			}
		} 

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void radioWriteoffClaimDate_CheckedChanged(object sender,EventArgs e) {

		}
	}
}




















