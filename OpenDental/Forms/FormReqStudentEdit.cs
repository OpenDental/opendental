using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReqStudentEdit : FormODBase {
		public ReqStudent ReqCur;

		///<summary></summary>
		public FormReqStudentEdit(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqStudentEdit_Load(object sender, System.EventArgs e) {
			//There should only be two types of users who are allowed to get this far:
			//Students editing their own req, and providers who are instructors for the current requirement.  But we will double check.
			Provider provUser=Providers.GetProv(Security.CurUser.ProvNum);
			if(provUser!=null && !provUser.IsInstructor) {//A student is logged in
				//the student only has ability to view/attach/detach their own requirements
				if(provUser.ProvNum!=ReqCur.ProvNum) {
					//but this should never happen
					MsgBox.Show(this,"Students may only edit their own requirements.");
					butDelete.Enabled=false;
					butOK.Enabled=false;
				}
				else{//the student matches
					butDelete.Enabled=false;
					textDateCompleted.Enabled=false;
					butNow.Enabled=false;
					comboInstructor.Enabled=false;
					//a student is only allowed to change the patient and appointment.
				}
			}
			else {//A student is not logged in
				if(provUser!=null && provUser.IsInstructor && ReqCur.InstructorNum!=0 && ReqCur.InstructorNum!=provUser.ProvNum) {
					butDelete.Enabled=false;
					butOK.Enabled=false;
				}
			}
			textStudent.Text=Providers.GetLongDesc(ReqCur.ProvNum);
			textCourse.Text=SchoolCourses.GetDescript(ReqCur.SchoolCourseNum);
			textDescription.Text=ReqCur.Descript;
			if(ReqCur.DateCompleted.Year>1880){
				textDateCompleted.Text=ReqCur.DateCompleted.ToShortDateString();
			}
			//if an apt is attached, then the same pat must be attached.
			Patient pat=Patients.GetPat(ReqCur.PatNum);
			if(pat!=null) {
				textPatient.Text=pat.GetNameFL();
			}
			Appointment apt=Appointments.GetOneApt(ReqCur.AptNum);
			if(apt!=null) {
				if(apt.AptStatus==ApptStatus.UnschedList) {
					textAppointment.Text=Lan.g(this,"Unscheduled");
				}
				else {
					textAppointment.Text=apt.AptDateTime.ToShortDateString()+" "+apt.AptDateTime.ToShortTimeString();
				}
				textAppointment.Text+=", "+apt.ProcDescript;
			}
			comboInstructor.Items.Clear();
			comboInstructor.Items.AddProvNone();
			comboInstructor.SelectedIndex=0;
			List<Provider> listInstructorProviders=Providers.GetDeepCopy(true).FindAll(x => x.IsInstructor);
			comboInstructor.Items.AddProvsFull(listInstructorProviders);
			if(ReqCur.InstructorNum>0) {
				comboInstructor.SetSelectedProvNum(ReqCur.InstructorNum);
			}
		}

		private void butDetachApt_Click(object sender,EventArgs e) {
			ReqCur.AptNum=0;
			textAppointment.Text="";
		}

		private void butDetachPat_Click(object sender,EventArgs e) {
			ReqCur.PatNum=0;
			textPatient.Text="";
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateCompleted.Text=MiscData.GetNowDateTime().ToShortDateString();
		}

		private void butSelectPat_Click(object sender,EventArgs e) {
			using FormPatientSelect FormP=new FormPatientSelect();
			FormP.SelectionModeOnly=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			ReqCur.PatNum=FormP.SelectedPatNum;
			textPatient.Text=Patients.GetPat(ReqCur.PatNum).GetNameFL();
			//if the patient changed, then the appointment must be detached.
			ReqCur.AptNum=0;
			textAppointment.Text="";
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//if(!MsgBox.Show(this,true,"Delete this student requirement?  This is typically handled by using the synch feature from requirements needed.")){
			//	return;
			//}
			try{
				ReqStudents.Delete(ReqCur.ReqStudentNum);//disallows deleting req that exists in reqNeeded.
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDateCompleted.Text!=""){
				try{
					DateTime.Parse(textDateCompleted.Text);
				}
				catch{
					MsgBox.Show(this,"Date is invalid.");
					return;
				}
			}
			//ReqNeededNum-not editable
			//Descript-not editable
			//SchoolCourseNum-not editable
			//ProvNum-Student not editable
			//AptNum-handled earlier
			//PatNum-handled earlier
			//InstructorNum
			if(comboInstructor.SelectedIndex==0){
				ReqCur.InstructorNum=0;
			}
			else{
				ReqCur.InstructorNum=comboInstructor.GetSelectedProvNum();
			}
			//DateCompleted
			ReqCur.DateCompleted=PIn.Date(textDateCompleted.Text);
			try{
				//if(IsNew){//inserting is always done as part of reqneededs.synch
				//	LabCases.Insert(CaseCur);
				//}
				//else{
				ReqStudents.Update(ReqCur);
				//}
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		

		

		

		

		

		

		


	}
}





















