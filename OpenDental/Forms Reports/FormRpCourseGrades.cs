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
	public class FormRpCourseGrades : FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.ComboBox comboSchoolCourse;
		private System.Windows.Forms.ComboBox comboSchoolClass;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpCourseGrades));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboSchoolCourse = new System.Windows.Forms.ComboBox();
			this.comboSchoolClass = new System.Windows.Forms.ComboBox();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(364,153);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(364,112);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboSchoolCourse
			// 
			this.comboSchoolCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolCourse.Location = new System.Drawing.Point(127,64);
			this.comboSchoolCourse.MaxDropDownItems = 30;
			this.comboSchoolCourse.Name = "comboSchoolCourse";
			this.comboSchoolCourse.Size = new System.Drawing.Size(130,21);
			this.comboSchoolCourse.TabIndex = 93;
			// 
			// comboSchoolClass
			// 
			this.comboSchoolClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSchoolClass.Location = new System.Drawing.Point(127,41);
			this.comboSchoolClass.MaxDropDownItems = 30;
			this.comboSchoolClass.Name = "comboSchoolClass";
			this.comboSchoolClass.Size = new System.Drawing.Size(130,21);
			this.comboSchoolClass.TabIndex = 92;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(43,68);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(82,16);
			this.label21.TabIndex = 91;
			this.label21.Text = "Course";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(43,45);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(82,16);
			this.label22.TabIndex = 90;
			this.label22.Text = "Class";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormRpCourseGrades
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(458,204);
			this.Controls.Add(this.comboSchoolCourse);
			this.Controls.Add(this.comboSchoolClass);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpCourseGrades";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Course Grades";
			this.Load += new System.EventHandler(this.FormRpCourseGrades_Load);
			this.ResumeLayout(false);

		}
		#endregion

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




















