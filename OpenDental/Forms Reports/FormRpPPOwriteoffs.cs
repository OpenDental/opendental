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
	public partial class FormRpPPOwriteoffs : FormODBase {

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




















