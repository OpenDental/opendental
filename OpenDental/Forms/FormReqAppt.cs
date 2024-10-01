using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReqAppt : FormODBase {
		//private DataTable table;
		private List<Provider> _listProvidersStudents;
		private DataTable _tableReq;
		private List<ReqStudent> _listsReqStudentsAttached;
		public long AptNum;
		private bool _hasChanged;
		public long PatNum;
		///<summary>The ReqStudent items that the user has removed from this appointment.</summary>
		private List<ReqStudent> _listReqStudentsRemoved=new List<ReqStudent>();
		private List<SchoolClass> _listSchoolClasses;
		private List<SchoolCourse> _listSchoolCourses;

		///<summary></summary>
		public FormReqAppt()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqAppt_Load(object sender,EventArgs e) {
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
			comboInstructor.Items.Clear();
			comboInstructor.Items.AddProvNone();
			comboInstructor.SelectedIndex=0;
			List<Provider> listProviderInstructors=Providers.GetDeepCopy(true).FindAll(x => x.IsInstructor);
			comboInstructor.Items.AddProvsFull(listProviderInstructors);
			FillStudents();
			FillReqs();
			_listsReqStudentsAttached=ReqStudents.GetForAppt(AptNum);
			if(_listsReqStudentsAttached.Count>0 && listProviderInstructors.Any(x => x.ProvNum==_listsReqStudentsAttached[0].InstructorNum)) {
				comboInstructor.SetSelectedProvNum(_listsReqStudentsAttached[0].InstructorNum);
			}
			FillAttached();
		}

		private void FillStudents() {
			if(comboClass.SelectedIndex==-1) {
				return;
			}
			long schoolClass=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			//int schoolCourse=SchoolCourses.List[comboCourse.SelectedIndex].SchoolCourseNum;
			_listProvidersStudents=ReqStudents.GetStudents(schoolClass);
			gridStudents.BeginUpdate();
			gridStudents.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			gridStudents.Columns.Add(col);
			gridStudents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listProvidersStudents.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listProvidersStudents[i].LName+", "+_listProvidersStudents[i].FName);
				row.Tag=_listProvidersStudents[i];
				gridStudents.ListGridRows.Add(row);
			}
			gridStudents.EndUpdate();
		}

		private void FillReqs(){
			long schoolCourse=0;
			if(comboCourse.SelectedIndex!=-1){
				schoolCourse=_listSchoolCourses[comboCourse.SelectedIndex].SchoolCourseNum;
			}
			long schoolClass=0;
			if(comboClass.SelectedIndex!=-1) {
				schoolClass=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			}
			gridReqs.BeginUpdate();
			gridReqs.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			gridReqs.Columns.Add(col);
			gridReqs.ListGridRows.Clear();
			if(gridStudents.GetSelectedIndex()==-1) {
				gridReqs.EndUpdate();
				return;
			}
			_tableReq=ReqStudents.GetForCourseClass(schoolCourse,schoolClass);
			GridRow row;
			for(int i=0;i<_tableReq.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_tableReq.Rows[i]["Descript"].ToString());
				gridReqs.ListGridRows.Add(row);
			}
			gridReqs.EndUpdate();
		}

		///<summary>All alterations to TableAttached should have been made</summary>
		private void FillAttached(){
			gridAttached.BeginUpdate();
			gridAttached.Columns.Clear();
			GridColumn col=new GridColumn("Student",130);
			gridAttached.Columns.Add(col);
			col=new GridColumn("Descript",150);
			gridAttached.Columns.Add(col);
			col=new GridColumn("Completed",40);
			gridAttached.Columns.Add(col);
			gridAttached.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listsReqStudentsAttached.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Providers.GetAbbr(_listsReqStudentsAttached[i].ProvNum));
				row.Cells.Add(_listsReqStudentsAttached[i].Descript);
				if(_listsReqStudentsAttached[i].DateCompleted.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add("X");
				}
				row.Tag=_listsReqStudentsAttached[i];
				gridAttached.ListGridRows.Add(row);
			}
			gridAttached.EndUpdate();
		}

		private void gridAttached_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_hasChanged){
				MsgBox.Show(this,"Not allowed to edit individual requirements immediately after adding or removing.");
				return;
			}
			using FormReqStudentEdit formReqStudentEdit=new FormReqStudentEdit();
			formReqStudentEdit.ReqStudentCur=_listsReqStudentsAttached[e.Row].Copy();
			formReqStudentEdit.ShowDialog();
			if(formReqStudentEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_listsReqStudentsAttached=ReqStudents.GetForAppt(AptNum);
			FillAttached();
		}

		private void gridStudents_CellClick(object sender,ODGridClickEventArgs e) {
			FillReqs();
		}

		private void comboClass_SelectionChangeCommitted(object sender,EventArgs e) {
			FillStudents();
			FillReqs();
		}

		private void comboCourse_SelectionChangeCommitted(object sender,EventArgs e) {
			FillReqs();
		}


		private void comboInstructor_SelectionChangeCommitted(object sender,EventArgs e) {
			for(int i=0;i<_listsReqStudentsAttached.Count;i++){
				if(_listsReqStudentsAttached[i].DateCompleted.Year>1880){
					continue;//don't alter instructor of completed reqs.
				}
				if(comboInstructor.SelectedIndex==0){
					_listsReqStudentsAttached[i].InstructorNum=0;
				}
				else{
					_listsReqStudentsAttached[i].InstructorNum=comboInstructor.GetSelectedProvNum();
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(gridReqs.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select at least one requirement from the list at the left first.");
				return;
			}
			ReqStudent reqStudent;
			for(int i=0;i<gridReqs.SelectedIndices.Length;i++){
				reqStudent=new ReqStudent();
				reqStudent.AptNum=AptNum;
				//req.DateCompleted
				reqStudent.Descript=_tableReq.Rows[gridReqs.SelectedIndices[i]]["Descript"].ToString();
				if(comboInstructor.SelectedIndex>0){
					reqStudent.InstructorNum=comboInstructor.GetSelectedProvNum();
				}
				reqStudent.PatNum=PatNum;
				reqStudent.ProvNum=_listProvidersStudents[gridStudents.GetSelectedIndex()].ProvNum;
				reqStudent.ReqNeededNum=PIn.Long(_tableReq.Rows[gridReqs.SelectedIndices[i]]["ReqNeededNum"].ToString());
				//req.ReqStudentNum=0 until synch on OK.
				reqStudent.SchoolCourseNum=_listSchoolCourses[comboCourse.SelectedIndex].SchoolCourseNum;
				_listsReqStudentsAttached.Add(reqStudent);
				_hasChanged=true;
			}
			FillAttached();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(gridAttached.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select at least one requirement from the list below first.");
				return;
			}
			for(int i=gridAttached.SelectedIndices.Length-1;i>=0;i--){//go backwards to remove from end of list
				_listsReqStudentsAttached.RemoveAt(gridAttached.SelectedIndices[i]);
				_listReqStudentsRemoved.Add((ReqStudent)gridAttached.ListGridRows[gridAttached.SelectedIndices[i]].Tag);
				_hasChanged=true;
			}
			FillAttached();
		}

		private void butSave_Click(object sender,EventArgs e) {
			ReqStudents.SynchApt(_listsReqStudentsAttached,_listReqStudentsRemoved,AptNum);
			DialogResult=DialogResult.OK;
		}

	}
}
