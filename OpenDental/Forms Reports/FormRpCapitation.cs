using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.ReportingComplex;
using OpenDentBusiness;
using System.Data;

namespace OpenDental{
	///<summary></summary>
	public partial class FormRpCapitation : FormODBase {

		///<summary></summary>
		public FormRpCapitation()
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

		private void FormRpCapitation_Load(object sender, System.EventArgs e) {
			DateTime today = DateTime.Today;
			DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
			textDateStart.Text=new DateTime(today.Year,today.Month,1).ToShortDateString();
			textDateEnd.Text=endOfMonth.ToShortDateString();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ExecuteReport();
		}

		private void ExecuteReport(){
			DateTime dateStart;
			DateTime dateEnd;
			bool isMedOrClinic;
			if(!DateTime.TryParse(textDateStart.Text,out dateStart)) {
				MsgBox.Show(this,"Please input a valid date.");
				return;
			}
			if(!DateTime.TryParse(textDateEnd.Text,out dateEnd)) {
				MsgBox.Show(this,"Please input a valid date.");
				return;
			}
			if(String.IsNullOrWhiteSpace(textCarrier.Text)) {
				MsgBox.Show(this,"Carrier can not be blank. Please input a value for carrier.");
				return;
			}
			ReportComplex report=new ReportComplex(true,true);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				isMedOrClinic=true;
			}
			else {
				isMedOrClinic=false;
			}
			Font font=new Font("Tahoma",9);
			Font fontTitle=new Font("Tahoma",17,FontStyle.Bold);
			Font fontSubTitle=new Font("Tahoma",10,FontStyle.Bold);
			report.AddTitle("Title",Lan.g(this,"Capitation Utilization"),fontTitle);
			report.AddSubTitle("PracTitle",PrefC.GetString(PrefName.PracticeTitle),fontSubTitle);
			report.AddSubTitle("Date",textDateStart.Text+" - "+textDateEnd.Text,fontSubTitle);
			DataTable table=RpCapitation.GetCapitationTable(dateStart,dateEnd,textCarrier.Text,isMedOrClinic);
			QueryObject query=report.AddQuery(table,"","",SplitByKind.None,1,true);
			query.AddColumn("Carrier",150,FieldValueType.String,font);
			query.GetColumnDetail("Carrier").SuppressIfDuplicate=true;
			query.AddColumn("Subscriber",120,FieldValueType.String,font);
			query.GetColumnDetail("Subscriber").SuppressIfDuplicate=true;
			query.AddColumn("Subsc SSN",70,FieldValueType.String,font);
			query.GetColumnDetail("Subsc SSN").SuppressIfDuplicate=true;
			query.AddColumn("Patient",120,FieldValueType.String,font);
			query.AddColumn("Pat DOB",80,FieldValueType.Date,font);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				query.AddColumn("Code",140,FieldValueType.String,font);
				query.AddColumn("Proc Description",120,FieldValueType.String,font);
				query.AddColumn("Date",80,FieldValueType.Date,font);
				query.AddColumn("UCR Fee",60,FieldValueType.Number,font);
				query.AddColumn("Co-Pay",60,FieldValueType.Number,font);
			}
			else {
				query.AddColumn("Code",50,FieldValueType.String,font);
				query.AddColumn("Proc Description",120,FieldValueType.String,font);
				query.AddColumn("Tth",30,FieldValueType.String,font);
				query.AddColumn("Surf",40,FieldValueType.String,font);
				query.AddColumn("Date",80,FieldValueType.Date,font);
				query.AddColumn("UCR Fee",70,FieldValueType.Number,font);
				query.AddColumn("Co-Pay",70,FieldValueType.Number,font);
			}
			if(!report.SubmitQueries()) {
				//DialogResult=DialogResult.Cancel;
				return;
			}
			using FormReportComplex FormR=new FormReportComplex(report);
			FormR.ShowDialog();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}




















