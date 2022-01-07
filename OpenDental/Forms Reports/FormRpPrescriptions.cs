using System;
using System.Drawing;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.ReportingComplex;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpPrescriptions : FormODBase {

		///<summary></summary>
		public FormRpPrescriptions(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ReportComplex report=new ReportComplex(true,false);
			DataTable table=RpPrescriptions.GetPrescriptionTable(radioPatient.Checked,textBoxInput.Text);
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.ReportName=Lan.g(this,"Prescriptions");
			report.AddTitle("Title",Lan.g(this,"Prescriptions"),fontTitle);
			report.AddSubTitle("PracticeTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			if(radioPatient.Checked){
				report.AddSubTitle("By Patient","By Patient");
			}
			else{
				report.AddSubTitle("By Drug","By Drug");
			}
			QueryObject query=report.AddQuery(table,Lan.g(this,"Date")+": "+DateTime.Today.ToString("d"));			
			query.AddColumn("Patient Name",120,FieldValueType.String);
			query.AddColumn("Date",95,FieldValueType.Date);
			query.AddColumn("Drug Name",100,FieldValueType.String);
			query.AddColumn("Directions",300);
			query.AddColumn("Dispense",100);
			query.AddColumn("Prov Name",100,FieldValueType.String);
			report.AddPageNum(font);
			if(!report.SubmitQueries()) {
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}
	}
}
