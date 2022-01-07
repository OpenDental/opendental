using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEvaluationReport:FormODBase {
		//This window is currently unfinished. Once grading has received a final review this will be easier to complete.
		//As of 06/24/2014 the implementation of this wil most likely be evaluation name based.
		//Reports will be able to be run for all evaluations with identical names and they will roll up into a grade.
		//The only problem I foresee with this is if customers delete an EvaluationDef and then create a completely different EvaluationDef with the same name.
		//This problem is currently handled by warning the user, but it still could be an issue.
		private bool _isCourseSelected=false;
		private bool _isInstructorSelected=false;
		private List<SchoolCourse> _schoolCourses;
		private List<Provider> _provInstructors;
		private FormQuery FormQuery2;

		public FormEvaluationReport() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEvaluationReport_Load(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.AddMonths(-4).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			FillCourses();
		}

		private void FillCourses() {
			_schoolCourses=SchoolCourses.GetDeepCopy();
			gridCourses.BeginUpdate();
			gridCourses.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationReport - Courses","CourseID"),60);
			gridCourses.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationReport - Courses","Description"),90);
			gridCourses.ListGridColumns.Add(col);
			gridCourses.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_schoolCourses.Count;i++) {
				row=new GridRow();
				row.Tag=_schoolCourses[i].SchoolCourseNum.ToString();
				row.Cells.Add(_schoolCourses[i].CourseID);
				row.Cells.Add(_schoolCourses[i].Descript);
				gridCourses.ListGridRows.Add(row);
			}
			gridCourses.EndUpdate();
		}

		private void FillInstructors() {
			_provInstructors=Providers.GetInstructors();
			gridInstructors.BeginUpdate();
			gridInstructors.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationReport - Instructors","ProvNum"),50);
			gridInstructors.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationReport - Instructors","Last Name"),80);
			gridInstructors.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationReport - Instructors","First Name"),80);
			gridInstructors.ListGridColumns.Add(col);
			gridInstructors.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_provInstructors.Count;i++) {
				row=new GridRow();
				row.Tag=(_provInstructors[i].ProvNum.ToString());
				row.Cells.Add(_provInstructors[i].ProvNum.ToString());
				row.Cells.Add(_provInstructors[i].LName);
				row.Cells.Add(_provInstructors[i].FName);
				gridInstructors.ListGridRows.Add(row);
			}
			gridInstructors.EndUpdate();
		}

		private void FillStudents() {
			List<long> schoolCourseNums=new List<long>();
			List<long> instructorProvNums=new List<long>();
			for(int i=0;i<gridCourses.SelectedIndices.Length;i++) {
				int index=gridCourses.SelectedIndices[i];
				schoolCourseNums.Add(PIn.Long(gridCourses.ListGridRows[index].Tag.ToString()));
			}
			for(int i=0;i<gridInstructors.SelectedIndices.Length;i++) {
				int index=gridInstructors.SelectedIndices[i];
				instructorProvNums.Add(PIn.Long(gridInstructors.ListGridRows[index].Tag.ToString()));
			}
			DataTable table=Evaluations.GetFilteredList(schoolCourseNums,instructorProvNums);
			gridStudents.BeginUpdate();
			gridStudents.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEvaluationReport - Students","ProvNum"),60);
			gridStudents.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationReport - Students","Last Name"),90);
			gridStudents.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEvaluationReport - Students","First Name"),90);
			gridStudents.ListGridColumns.Add(col);
			gridStudents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["StudentNum"].ToString());
				row.Cells.Add(table.Rows[i]["LName"].ToString());
				row.Cells.Add(table.Rows[i]["FName"].ToString());
				row.Tag=table.Rows[i]["StudentNum"].ToString();
				gridStudents.ListGridRows.Add(row);
			}
			gridStudents.EndUpdate();
		}

		private void checkAllInstructors_CheckedChanged(object sender,EventArgs e) {
			if(checkAllInstructors.Checked) {
				gridInstructors.SetAll(true);
				gridInstructors.Visible=false;
				_isInstructorSelected=true;
				return;
			}
			gridInstructors.SetAll(false);
			gridInstructors.Visible=true;
		}

		private void checkAllCourses_CheckedChanged(object sender,EventArgs e) {
			if(checkAllCourses.Checked) {
				if(!_isCourseSelected) {
					FillInstructors();
					FillStudents();
					_isCourseSelected=true;
				}
				gridCourses.SetAll(true);
				gridCourses.Visible=false;
				return;
			}
			gridCourses.SetAll(false);
			gridCourses.Visible=true;
		}

		private void butAllStudents_Click(object sender,EventArgs e) {
			gridStudents.SetAll(true);
		}

		private void gridCourses_CellClick(object sender,UI.ODGridClickEventArgs e) {
			if(!_isCourseSelected) {
				FillInstructors();
				_isCourseSelected=true;
			}
			if(_isInstructorSelected) {
				FillStudents();
			}
		}

		private void gridInstructors_CellClick(object sender,UI.ODGridClickEventArgs e) {
			FillStudents();
			_isInstructorSelected=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDateStart.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDateStart.Text=="") {
				MsgBox.Show(this,"Please enter a start date.");
				return;
			}
			if(!textDateEnd.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDateEnd.Text=="") {
				MsgBox.Show(this,"Please enter an end date.");
				return;
			}
			if(gridCourses.SelectedIndices.Length<1) {
				MsgBox.Show(this,"At least one course must be selected to run a report.  Please select a row from the course grid.");
				return;
			}
			if(gridInstructors.SelectedIndices.Length<1) {
				MsgBox.Show(this,"At least one instructor must be selected to run a report.  Please select a row from the instructor grid.");
				return;
			}
			if(gridStudents.SelectedIndices.Length<1) {
				MsgBox.Show(this,"At least one student must be selected to run a report.  Please select a row from the student grid.");
				return;
			}
			DateTime dateStart=PIn.Date(textDateStart.Text);
			DateTime dateEnd=PIn.Date(textDateEnd.Text);
			string whereCourses="";
			if(!checkAllCourses.Checked) {
				for(int i=0;i<gridCourses.SelectedIndices.Length;i++) {
					if(i==0) {
						whereCourses+=" AND evaluation.SchoolCourseNum IN(";
					}
					whereCourses+=gridCourses.ListGridRows[gridCourses.SelectedIndices[i]].Tag;
					if(i!=gridCourses.SelectedIndices.Length-1) {
						whereCourses+=",";
					}
				}
				whereCourses+=")";
			}
			string whereInstructors="";
			if(!checkAllInstructors.Checked) {
				for(int i=0;i<gridInstructors.SelectedIndices.Length;i++) {
					if(i==0) {
						whereInstructors+=" AND evaluation.InstructNum IN(";
					}
					whereInstructors+=gridInstructors.ListGridRows[gridInstructors.SelectedIndices[i]].Tag;
					if(i!=gridInstructors.SelectedIndices.Length-1) {
						whereInstructors+=",";
					}
				}
				whereInstructors+=")";
			}
			//No checkbox for students
			string whereStudents=" AND evaluation.StudentNum IN(";
			for(int i=0;i<gridStudents.SelectedIndices.Length;i++) {
				whereStudents+=gridStudents.ListGridRows[gridStudents.SelectedIndices[i]].Tag;
				if(i!=gridStudents.SelectedIndices.Length-1) {
					whereStudents+=",";
				}
			}
			whereStudents+=")";
			ReportSimpleGrid report=new ReportSimpleGrid();
			//Evaluations------------------------------------------------------------------------------
			report.Query="SELECT "+DbHelper.Concat("students.LName","', '","students.FName")+" StudentName,evaluation.DateEval,"
				+"courses.CourseID,"+DbHelper.Concat("instructors.LName","', '","instructors.FName")+" InstructorName,"
			  +"evaluation.EvalTitle,gradeScales.ScaleType,evaluation.OverallGradeShowing,evaluation.OverallGradeNumber";
			report.Query+=" FROM evaluation"
			  +" INNER JOIN provider students ON evaluation.StudentNum=students.ProvNum"
				+" INNER JOIN provider instructors ON evaluation.InstructNum=instructors.ProvNum"
				+" INNER JOIN gradingscale gradeScales ON evaluation.GradingScaleNum=gradeScales.GradingScaleNum"
				+" INNER JOIN schoolcourse courses ON evaluation.SchoolCourseNum=courses.SchoolCourseNum"
				+" WHERE evaluation.DateEval BETWEEN "+POut.Date(dateStart)+" AND "+POut.Date(dateEnd)
				+whereCourses
				+whereInstructors
				+whereStudents
				+" ORDER BY StudentName,evaluation.DateEval";
			FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=true;
			DataTable table=report.GetTempTable();
			report.TableQ=new DataTable();
			int colI=10;
			for(int i=0;i<colI;i++) { //add columns
				report.TableQ.Columns.Add(new System.Data.DataColumn());//blank columns
			}
			report.InitializeColumns();
			DataRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row = report.TableQ.NewRow();//create new row called 'row' based on structure of TableQ
				row[0]=table.Rows[i]["StudentName"].ToString();
				row[1]=PIn.Date(table.Rows[i]["DateEval"].ToString()).ToShortDateString();
				row[2]=table.Rows[i]["CourseID"].ToString();
				row[3]=table.Rows[i]["InstructorName"].ToString();
				row[4]=table.Rows[i]["EvalTitle"].ToString();
				switch((EnumScaleType)PIn.Int(table.Rows[i]["ScaleType"].ToString())) {
					case EnumScaleType.PickList:
						row[5]=Enum.GetName(typeof(EnumScaleType),(int)EnumScaleType.PickList);
						break;
					case EnumScaleType.Percentage:
						row[5]=Enum.GetName(typeof(EnumScaleType),(int)EnumScaleType.Percentage);
						break;
					case EnumScaleType.Weighted:
						row[5]=Enum.GetName(typeof(EnumScaleType),(int)EnumScaleType.Weighted);
						break;
				}
				row[6]=table.Rows[i]["OverallGradeShowing"].ToString();
				row[7]=table.Rows[i]["OverallGradeNumber"].ToString();
				report.TableQ.Rows.Add(row);
			}
			FormQuery2.ResetGrid();
			report.Title=Lan.g(this,"Course Average");
			report.SubTitle.Add(dateStart.ToShortDateString()+" - "+dateEnd.ToShortDateString());
			if(checkAllInstructors.Checked) {
				report.SubTitle.Add(Lan.g(this,"All Instructors"));
			}
			if(checkAllCourses.Checked) {
				report.SubTitle.Add(Lan.g(this,"All Courses"));
			}
			report.SetColumn(this,0,"Student",120);
			report.SetColumn(this,1,"Date",80);
			report.SetColumn(this,2,"Course",100);
			report.SetColumn(this,3,"Instructor",120);
			report.SetColumn(this,4,"Evaluation",90);
			report.SetColumn(this,5,"Scale Type",90);
			report.SetColumn(this,6,"Grade Showing",100);
			report.SetColumn(this,7,"Grade Number",100);
			FormQuery2.ShowDialog();
			FormQuery2.Dispose();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}