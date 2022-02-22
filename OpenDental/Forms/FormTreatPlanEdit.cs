using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTreatPlanEdit : FormODBase {
		private TreatPlan PlanCur;
		private Userod _presenterCur;
		private Userod _presenterOld;

		///<summary></summary>
		public FormTreatPlanEdit(TreatPlan planCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PlanCur=planCur.Copy();
		}

		private void FormTreatPlanEdit_Load(object sender, System.EventArgs e) {
			//this window never comes up for new TP.  Always saved ahead of time.
			if(!Security.IsAuthorized(Permissions.TreatPlanEdit,PlanCur.DateTP)) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
				butPickResponsParty.Enabled=false;
				butClearResponsParty.Enabled=false;
				butSigClear.Enabled=false;
				butDocumentDetach.Enabled=false;
				textHeading.ReadOnly=true;
				textDateTP.ReadOnly=true;
				textNote.ReadOnly=true;
				if(Security.IsAuthorized(Permissions.TreatPlanSign,PlanCur.DateTP)) {//User has permission to edit the heading field.
					textHeading.ReadOnly=false;
					butOK.Enabled=true;
				}
			}
			if(!Security.IsAuthorized(Permissions.TreatPlanPresenterEdit,true)) {
				butPickPresenter.Visible=false;
			}
			if(PlanCur.UserNumPresenter>0) {
				_presenterCur=Userods.GetUser(PlanCur.UserNumPresenter);
				_presenterOld=_presenterCur.Copy();
				textPresenter.Text=_presenterCur.UserName;
			}
			textUserEntry.Text=Userods.GetName(PlanCur.SecUserNumEntry);
			textDateTP.Text=PlanCur.DateTP.ToShortDateString();
			textHeading.Text=PlanCur.Heading;
			textNote.Text=PlanCur.Note;
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				labelResponsParty.Visible=false;
				textResponsParty.Visible=false;
				butPickResponsParty.Visible=false;
				butClearResponsParty.Visible=false;
			}
			if(PlanCur.ResponsParty!=0){
				textResponsParty.Text=Patients.GetLim(PlanCur.ResponsParty).GetNameLF();
			}
			if(PlanCur.Signature!="") { //Per Nathan 01 OCT 2015: In addition to invalidating signature (old behavior) we will also block editing signed TPs.
				butOK.Enabled=false;
				textHeading.ReadOnly=true;
				textDateTP.ReadOnly=true;
				textNote.ReadOnly=true;
				butClearResponsParty.Enabled=false;
				butPickResponsParty.Enabled=false;
				butSigClear.Visible=true;
				butDocumentDetach.Enabled=false;
			}
			else {
				butSigClear.Visible=false;
				butSigClear.Enabled=false;
			}
			if(PlanCur.DocNum>0) {//Was set at some point in the past.
				Document doc=Documents.GetByNum(PlanCur.DocNum);
				if(doc.DocNum==0) {
					textDocument.Text="("+Lan.g(this,"Missing Document")+")";//Invalid Fkey to document.DocNum
					butDocumentView.Enabled=false;
				}
				else {
					textDocument.Text=doc.Description;
					if(!Documents.DocExists(doc.DocNum)) {
						textDocument.Text+=" ("+Lan.g(this,"Unreachable File")+")";//Document points to unreachable file
						butDocumentView.Enabled=false;
					}
				}
			}
			else {//hide document controls because there is no attached document
				labelDocument.Visible=false;
				textDocument.Visible=false;
				butDocumentView.Visible=false;
				butDocumentDetach.Visible=false;
			}
		}

		private void butPickResponsParty_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.SelectionModeOnly=true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK){
				return;
			}
			PlanCur.ResponsParty=FormPS.SelectedPatNum;
			textResponsParty.Text=Patients.GetLim(PlanCur.ResponsParty).GetNameLF();
		}

		private void butClearResponsParty_Click(object sender,EventArgs e) {
			PlanCur.ResponsParty=0;
			textResponsParty.Text="";
		}

		private void butDocumentView_Click(object sender,EventArgs e) {
			Document doc=Documents.GetByNum(PlanCur.DocNum);
			if(doc.DocNum==0) {
				MsgBox.Show(this,"Error locating document.");
				return;
			}
			if(!Documents.DocExists(doc.DocNum)) {
				MsgBox.Show(this,"Unable to open document.");
				return;
			}
			Documents.OpenDoc(doc.DocNum);
		}

		private void butDocumentDetach_Click(object sender,EventArgs e) {
			PlanCur.DocNum=0;
			butDocumentView.Enabled=false;
			butDocumentDetach.Enabled=false;
			textDocument.Text="";
		}

		private void butSigClear_Click(object sender,EventArgs e) {
			//Cannot click this button if you are not authorized to edit, so it is safe to re-enable edit controls below.
			//Disable signature buttons
			if(!Security.IsAuthorized(Permissions.TreatPlanEdit,PlanCur.DateTP)) {
				butOK.Enabled=false;
			}
			else {
				butOK.Enabled=true;
			}
			PlanCur.Signature="";
			PlanCur.SignatureText="";
			PlanCur.DateTSigned=DateTime.MinValue;
			PlanCur.SigIsTopaz=false;
			PlanCur.SignaturePractice="";
			PlanCur.SignaturePracticeText="";
			PlanCur.DateTPracticeSigned=DateTime.MinValue;
			butSigClear.Enabled=false;
			//Re-enable controls to edit. 
			textHeading.ReadOnly=false;
			textDateTP.ReadOnly=false;
			textNote.ReadOnly=false;
			butClearResponsParty.Enabled=true;
			butPickResponsParty.Enabled=true;
			butDocumentDetach.Enabled=true;
		}

		private void butPickPresenter_Click(object sender,EventArgs e) {
			using FormUserPick FormUP=new FormUserPick();
			List<Userod> listUsers=Userods.GetWhere(x => x.ClinicIsRestricted == false || x.ClinicNum == Clinics.ClinicNum,true);
			FormUP.ListUserodsFiltered=listUsers;
			if(_presenterCur!=null) {
				FormUP.SuggestedUserNum=_presenterCur.UserNum;
			}
			FormUP.IsPickNoneAllowed=true;
			FormUP.ShowDialog();
			if(FormUP.DialogResult!=DialogResult.OK) {
				return;
			}
			_presenterCur=Userods.GetUser(FormUP.SelectedUserNum);//can be null
			if(_presenterCur!=null) {
				textPresenter.Text=_presenterCur.UserName;
			}
			else {
				textPresenter.Text="";
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//if(IsNew){
			//	DialogResult=DialogResult.Cancel;
			//	return;
			//}
			ProcTPs.DeleteForTP(PlanCur.TreatPlanNum);
			try{
				TreatPlans.Delete(PlanCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanEdit,PlanCur.PatNum,"Delete TP: "+PlanCur.DateTP.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateTP.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDateTP.Text==""){
				MsgBox.Show(this,"Please enter a date first.");
				return;
			}
			PlanCur.DateTP=PIn.Date(textDateTP.Text);
			PlanCur.Heading=textHeading.Text;
			PlanCur.Note=textNote.Text;
			if(_presenterCur!=null) {
				PlanCur.UserNumPresenter=_presenterCur.UserNum;
			}
			else {
				PlanCur.UserNumPresenter=0;
			}
			//PlanCur.SecUserNumEntry is updated automatically by MySQL.
			TreatPlans.Update(PlanCur);
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanEdit,PlanCur.PatNum,"Edit TP: "+PlanCur.DateTP.ToShortDateString());
			if(_presenterCur!=null && (_presenterOld==null || _presenterCur.UserNum != _presenterOld.UserNum)) {
				SecurityLogs.MakeLogEntry(Permissions.TreatPlanPresenterEdit,PlanCur.PatNum,
					"TP Presenter Changed from "+(_presenterOld==null?"\"null\"":_presenterOld.UserName)+" to "+_presenterCur.UserName+".");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















