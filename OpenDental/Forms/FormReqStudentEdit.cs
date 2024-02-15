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
		public ReqStudent ReqStudentCur;

		///<summary></summary>
		public FormReqStudentEdit(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqStudentEdit_Load(object sender, System.EventArgs e) {
			//There should only be two types of users who are allowed to get this far:
			//Students editing their own req, and providers who are instructors for the current requirement.  But we will double check.
			Provider provider=Providers.GetProv(Security.CurUser.ProvNum);
			if(provider!=null && !provider.IsInstructor) {//A student is logged in
				//the student only has ability to view/attach/detach their own requirements
				if(provider.ProvNum!=ReqStudentCur.ProvNum) {
					//but this should never happen
					MsgBox.Show(this,"Students may only edit their own requirements.");
					butDelete.Enabled=false;
					butSave.Enabled=false;
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
				if(provider!=null && provider.IsInstructor && ReqStudentCur.InstructorNum!=0 && ReqStudentCur.InstructorNum!=provider.ProvNum) {
					butDelete.Enabled=false;
					butSave.Enabled=false;
				}
			}
			textStudent.Text=Providers.GetLongDesc(ReqStudentCur.ProvNum);
			textCourse.Text=SchoolCourses.GetDescript(ReqStudentCur.SchoolCourseNum);
			textDescription.Text=ReqStudentCur.Descript;
			if(ReqStudentCur.DateCompleted.Year>1880){
				textDateCompleted.Text=ReqStudentCur.DateCompleted.ToShortDateString();
			}
			//if an apt is attached, then the same pat must be attached.
			Patient patient=Patients.GetPat(ReqStudentCur.PatNum);
			if(patient!=null) {
				textPatient.Text=patient.GetNameFL();
			}
			Appointment appointment=Appointments.GetOneApt(ReqStudentCur.AptNum);
			if(appointment!=null) {
				if(appointment.AptStatus==ApptStatus.UnschedList) {
					textAppointment.Text=Lan.g(this,"Unscheduled");
				}
				else {
					textAppointment.Text=appointment.AptDateTime.ToShortDateString()+" "+appointment.AptDateTime.ToShortTimeString();
				}
				textAppointment.Text+=", "+appointment.ProcDescript;
			}
			comboInstructor.Items.Clear();
			comboInstructor.Items.AddProvNone();
			comboInstructor.SelectedIndex=0;
			List<Provider> listProviderInstructors=Providers.GetDeepCopy(true).FindAll(x => x.IsInstructor);
			comboInstructor.Items.AddProvsFull(listProviderInstructors);
			if(ReqStudentCur.InstructorNum>0) {
				comboInstructor.SetSelectedProvNum(ReqStudentCur.InstructorNum);
			}
		}

		private void butDetachApt_Click(object sender,EventArgs e) {
			ReqStudentCur.AptNum=0;
			textAppointment.Text="";
		}

		private void butDetachPat_Click(object sender,EventArgs e) {
			ReqStudentCur.PatNum=0;
			textPatient.Text="";
		}

		private void butNow_Click(object sender,EventArgs e) {
			textDateCompleted.Text=MiscData.GetNowDateTime().ToShortDateString();
		}

		private void butSelectPat_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK){
				return;
			}
			ReqStudentCur.PatNum=formPatientSelect.PatNumSelected;
			textPatient.Text=Patients.GetPat(ReqStudentCur.PatNum).GetNameFL();
			//if the patient changed, then the appointment must be detached.
			ReqStudentCur.AptNum=0;
			textAppointment.Text="";
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//if(!MsgBox.Show(this,true,"Delete this student requirement?  This is typically handled by using the sync feature from requirements needed.")){
			//	return;
			//}
			try{
				ReqStudents.Delete(ReqStudentCur.ReqStudentNum);//disallows deleting req that exists in reqNeeded.
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
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
				ReqStudentCur.InstructorNum=0;
			}
			else{
				ReqStudentCur.InstructorNum=comboInstructor.GetSelectedProvNum();
			}
			//DateCompleted
			ReqStudentCur.DateCompleted=PIn.Date(textDateCompleted.Text);
			try{
				//if(IsNew){//inserting is always done as part of reqneededs.synch
				//	LabCases.Insert(CaseCur);
				//}
				//else{
				ReqStudents.Update(ReqStudentCur);
				//}
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

	}
}