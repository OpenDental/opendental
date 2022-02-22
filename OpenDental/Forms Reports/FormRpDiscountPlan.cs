using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using System.Data;
using CodeBase;

namespace OpenDental {
	public partial class FormRpDiscountPlan:FormODBase {

		public FormRpDiscountPlan() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butOK_Click(object sender,EventArgs e) {
			ReportComplex report=new ReportComplex(true,false);
			DataTable table=RpDiscountPlan.GetTable(textDescription.Text);
			Font fontMain=new Font("Tahoma",8);
			Font fontTitle=new Font("Tahoma",15,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Discount Plan List");
			report.AddTitle("Title",Lan.g(this,"Discount Plan List"),fontTitle);
			report.AddSubTitle("Practice Title",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Description",190,font:fontMain);
			query.AddColumn("FeeSched",145,font:fontMain);
			query.AddColumn("AdjType",145,font:fontMain);
			query.AddColumn("DateEffective",75,fieldValueType:FieldValueType.Date,font:fontMain);
			query.AddColumn("DateTerm",75,fieldValueType:FieldValueType.Date,font:fontMain);
			query.AddColumn("Patient",165,font:fontMain);
			report.AddPageNum(fontMain);
			if(!report.SubmitQueries()) {
				return;
			}
			report.AddFooterText("Total","Total: "+report.TotalRows.ToString(),fontMain,10,ContentAlignment.MiddleRight);
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}