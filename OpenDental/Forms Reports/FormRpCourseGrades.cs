using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using OpenDental.ReportingOld2;

//using System.IO;
//using System.Text;
//using System.Xml.Serialization;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormRpCourseGrades : FormODBase {

		///<summary></summary>
		public FormRpCourseGrades()
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

		private void FormRpCourseGrades_Load(object sender, System.EventArgs e) {
			/*for(int i=0;i<SchoolClasses.List.Length;i++){
				comboSchoolClass.Items.Add(SchoolClasses.List[i].GradYear.ToString()+"-"+SchoolClasses.List[i].Descript);
			}
			for(int i=0;i<SchoolCourses.List.Length;i++){
				comboSchoolCourse.Items.Add(SchoolCourses.List[i].CourseID+"  "+SchoolCourses.List[i].Descript);
			}*/
			//user will never see the interface
			ExecuteReport();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//if(comboSchoolClass.SelectedIndex==-1 || comboSchoolCourse.SelectedIndex==-1){
			//	MsgBox.Show(this,"Please select a class and course.");
			//	return;
			//}
			
		}

		private void ExecuteReport(){
			MessageBox.Show("Incomplete");
			/*
			Report report=new Report();
			//report.IsLandscape=true;
			report.AddTitle("Course Grades");
			//report.AddSubTitle("");//should name the class and course
			report.AddParameter("class",FieldValueType.ForeignKey,new ArrayList()
				,"Class"
				,"appointment.SchoolClassNum = ?"
				,ReportFKType.SchoolClass);
			report.AddParameter("date1",FieldValueType.Date,DateTimeFirst
				,"From Date"
				,"procedurelog.ProcDate >= '?'");
			report.AddParameter("date2",FieldValueType.Date
				,DateTimeFirst.AddMonths(1).AddDays(-1)
				,"To Date"
				,"procedurelog.ProcDate <= '?'");
			report.Query=@"SELECT patient.LName,patient.FName,appointment.GradePoint
				FROM appointment,patient
				WHERE ?class";
			report.AddColumn("Carrier",150,FieldValueType.String);
			report.GetLastRO(ReportObjectKind.FieldObject).SuppressIfDuplicate=true;
			report.AddColumn("Subscriber",120,FieldValueType.String);
			report.GetLastRO(ReportObjectKind.FieldObject).SuppressIfDuplicate=true;
			report.AddColumn("Subsc SSN",70,FieldValueType.String);
			report.GetLastRO(ReportObjectKind.FieldObject).SuppressIfDuplicate=true;
			report.AddColumn("Patient",120,FieldValueType.String);
			report.AddColumn("Pat DOB",80,FieldValueType.Date);
			report.AddColumn("Code",50,FieldValueType.String);
			report.AddColumn("Proc Description",120,FieldValueType.String);
			report.AddColumn("Tth",30,FieldValueType.String);
			report.AddColumn("Surf",40,FieldValueType.String);
			report.AddColumn("Date",80,FieldValueType.Date);
			report.AddColumn("UCR Fee",70,FieldValueType.Number);
			report.AddColumn("Co-Pay",70,FieldValueType.Number);
			report.AddPageNum();
      if(!report.SubmitQuery()){
				DialogResult=DialogResult.Cancel;
				return;
			}
//incomplete: Add functionality for using parameter values in textObjects, probably using inline XML:
			report.AddSubTitle(((DateTime)report.ParameterFields["date1"].CurrentValues[0]).ToShortDateString()+" - "+((DateTime)report.ParameterFields["date2"].CurrentValues[0]).ToShortDateString());
//incomplete: Implement formulas for situations like this:
			for(int i=0;i<report.ReportTable.Rows.Count;i++){
				if(PIn.PDouble(report.ReportTable.Rows[i][11].ToString())==-1){
					report.ReportTable.Rows[i][11]="0";
				}
			}
			FormReport FormR=new FormReport(report);
			//FormR.MyReport=report;
			FormR.ShowDialog();*/
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}




















