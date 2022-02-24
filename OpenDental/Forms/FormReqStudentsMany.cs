using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReqStudentsMany : FormODBase {
		private DataTable table;
		private List<SchoolClass> _listSchoolClasses;
		private List<SchoolCourse> _listSchoolCourses;

		///<summary></summary>
		public FormReqStudentsMany()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqStudentsMany_Load(object sender,EventArgs e) {
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			for(int i=0;i<_listSchoolClasses.Count;i++) {
				comboClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
			}
			if(comboClass.Items.Count>0) {
				comboClass.SelectedIndex=0;
			}
			for(int i=0;i<_listSchoolCourses.Count;i++) {
				comboCourse.Items.Add(SchoolCourses.GetDescript(_listSchoolCourses[i]));
			}
			if(comboCourse.Items.Count>0) {
				comboCourse.SelectedIndex=0;
			}
			FillGrid();
		}

		private void FillGrid() {
			if(comboClass.SelectedIndex==-1 || comboCourse.SelectedIndex==-1) {
				return;
			}
			long schoolClass=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			long schoolCourse=_listSchoolCourses[comboCourse.SelectedIndex].SchoolCourseNum;
			table=ReqStudents.RefreshManyStudents(schoolClass,schoolCourse);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableReqStudentMany","Last"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableReqStudentMany","First"),100);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn(Lan.g("TableReqStudentMany","Total"),50);
			//gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableReqStudentMany","Done"),50);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["LName"].ToString());
				row.Cells.Add(table.Rows[i]["FName"].ToString());
				//row.Cells.Add(table.Rows[i]["totalreq"].ToString());
				row.Cells.Add(table.Rows[i]["donereq"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReqStudentOne FormR=new FormReqStudentOne();
			FormR.ProvNum=PIn.Long(table.Rows[e.Row]["studentNum"].ToString());
			FormR.ShowDialog();
			FillGrid();
		}

		private void comboClass_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboCourse_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		

		

		


	}
}





















