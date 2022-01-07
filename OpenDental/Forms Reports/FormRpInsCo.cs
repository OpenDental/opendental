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

namespace OpenDental{
///<summary></summary>
	public partial class FormRpInsCo : FormODBase {
		private FormQuery FormQuery2;
		private string carrier;

		///<summary></summary>
		public FormRpInsCo(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			carrier= PIn.String(textBoxCarrier.Text);
			ReportComplex report=new ReportComplex(true,false);
			DataTable table=RpInsCo.GetInsCoTable(carrier);
			Font fontMain=new Font("Tahoma",8);
			Font fontTitle=new Font("Tahoma",15,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Insurance Plan List");
			report.AddTitle("Title",Lan.g(this,"Insurance Plan List"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));
			query.AddColumn("Carrier Name",230,font:fontMain);
			query.AddColumn("Subscriber Name",175,font:fontMain);
			query.AddColumn("Carrier Phone#",175,font:fontMain);
			query.AddColumn("Group Name",165,font:fontMain);
			report.AddPageNum(fontMain);
			if(!report.SubmitQueries()) {
				return;
			}
			report.AddFooterText("Total","Total: "+report.TotalRows.ToString(),fontMain,10,ContentAlignment.MiddleRight);
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;		
		}
	}
}


















