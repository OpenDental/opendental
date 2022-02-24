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
		private List<Provider> _listStudents;
		private DataTable ReqTable;
		private List<ReqStudent> reqsAttached;
		public long AptNum;
		private bool hasChanged;
		public long PatNum;
		///<summary>The ReqStudent items that the user has removed from this appointment.</summary>
		private List<ReqStudent> _listReqsRemoved=new List<ReqStudent>();
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
			reqsAttached=ReqStudents.GetForAppt(AptNum);
			if(reqsAttached.Count>0 && listProviderInstructors.Any(x => x.ProvNum==reqsAttached[0].InstructorNum)) {
				comboInstructor.SetSelectedProvNum(reqsAttached[0].InstructorNum);
			}
			FillAttached();
		}

		private void FillStudents() {
			if(comboClass.SelectedIndex==-1) {
				return;
			}
			long schoolClass=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			//int schoolCourse=SchoolCourses.List[comboCourse.SelectedIndex].SchoolCourseNum;
			_listStudents=ReqStudents.GetStudents(schoolClass);
			gridStudents.BeginUpdate();
			gridStudents.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",100);
			gridStudents.ListGridColumns.Add(col);
			gridStudents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listStudents.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listStudents[i].LName+", "+_listStudents[i].FName);
				row.Tag=_listStudents[i];
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
			gridReqs.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",100);
			gridReqs.ListGridColumns.Add(col);
			gridReqs.ListGridRows.Clear();
			if(gridStudents.GetSelectedIndex()==-1) {
				gridReqs.EndUpdate();
				return;
			}
			ReqTable=ReqStudents.GetForCourseClass(schoolCourse,schoolClass);
			GridRow row;
			for(int i=0;i<ReqTable.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ReqTable.Rows[i]["Descript"].ToString());
				gridReqs.ListGridRows.Add(row);
			}
			gridReqs.EndUpdate();
		}

		///<summary>All alterations to TableAttached should have been made</summary>
		private void FillAttached(){
			gridAttached.BeginUpdate();
			gridAttached.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Student",130);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn("Descript",150);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn("Completed",40);
			gridAttached.ListGridColumns.Add(col);
			gridAttached.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<reqsAttached.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Providers.GetAbbr(reqsAttached[i].ProvNum));
				row.Cells.Add(reqsAttached[i].Descript);
				if(reqsAttached[i].DateCompleted.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add("X");
				}
				row.Tag=reqsAttached[i];
				gridAttached.ListGridRows.Add(row);
			}
			gridAttached.EndUpdate();
		}

		private void gridAttached_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(hasChanged){
				MsgBox.Show(this,"Not allowed to edit individual requirements immediately after adding or removing.");
				return;
			}
			using FormReqStudentEdit FormRSE=new FormReqStudentEdit();
			FormRSE.ReqCur=reqsAttached[e.Row].Copy();
			FormRSE.ShowDialog();
			if(FormRSE.DialogResult!=DialogResult.OK) {
				return;
			}
			reqsAttached=ReqStudents.GetForAppt(AptNum);
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
			for(int i=0;i<reqsAttached.Count;i++){
				if(reqsAttached[i].DateCompleted.Year>1880){
					continue;//don't alter instructor of completed reqs.
				}
				if(comboInstructor.SelectedIndex==0){
					reqsAttached[i].InstructorNum=0;
				}
				else{
					reqsAttached[i].InstructorNum=comboInstructor.GetSelectedProvNum();
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(gridReqs.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select at least one requirement from the list at the left first.");
				return;
			}
			ReqStudent req;
			for(int i=0;i<gridReqs.SelectedIndices.Length;i++){
				req=new ReqStudent();
				req.AptNum=AptNum;
				//req.DateCompleted
				req.Descript=ReqTable.Rows[gridReqs.SelectedIndices[i]]["Descript"].ToString();
				if(comboInstructor.SelectedIndex>0){
					req.InstructorNum=comboInstructor.GetSelectedProvNum();
				}
				req.PatNum=PatNum;
				req.ProvNum=_listStudents[gridStudents.GetSelectedIndex()].ProvNum;
				req.ReqNeededNum=PIn.Long(ReqTable.Rows[gridReqs.SelectedIndices[i]]["ReqNeededNum"].ToString());
				//req.ReqStudentNum=0 until synch on OK.
				req.SchoolCourseNum=_listSchoolCourses[comboCourse.SelectedIndex].SchoolCourseNum;
				reqsAttached.Add(req);
				hasChanged=true;
			}
			FillAttached();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(gridAttached.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select at least one requirement from the list below first.");
				return;
			}
			for(int i=gridAttached.SelectedIndices.Length-1;i>=0;i--){//go backwards to remove from end of list
				reqsAttached.RemoveAt(gridAttached.SelectedIndices[i]);
				_listReqsRemoved.Add((ReqStudent)gridAttached.ListGridRows[gridAttached.SelectedIndices[i]].Tag);
				hasChanged=true;
			}
			FillAttached();
		}

		private void butOK_Click(object sender,EventArgs e) {
			ReqStudents.SynchApt(reqsAttached,_listReqsRemoved,AptNum);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		


	}
}





















