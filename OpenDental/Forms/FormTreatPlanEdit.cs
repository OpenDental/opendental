using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormTreatPlanEdit : FormODBase {
		private TreatPlan _treatPlan;
		private Userod _userodPresenter;
		private Userod _userodPresenterOld;

		///<summary></summary>
		public FormTreatPlanEdit(TreatPlan treatPlan)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_treatPlan=treatPlan.Copy();
		}

		private void FormTreatPlanEdit_Load(object sender, System.EventArgs e) {
			//this window never comes up for new TP.  Always saved ahead of time.
			if(!Security.IsAuthorized(EnumPermType.TreatPlanEdit,_treatPlan.DateTP)) {
				butSave.Enabled=false;
				butDelete.Enabled=false;
				butPickResponsParty.Enabled=false;
				butClearResponsParty.Enabled=false;
				butSigClear.Enabled=false;
				butDocumentDetach.Enabled=false;
				textHeading.ReadOnly=true;
				textDateTP.ReadOnly=true;
				textNote.ReadOnly=true;
				if(Security.IsAuthorized(EnumPermType.TreatPlanSign,_treatPlan.DateTP)) {//User has permission to edit the heading field.
					textHeading.ReadOnly=false;
					butSave.Enabled=true;
				}
			}
			if(!Security.IsAuthorized(EnumPermType.TreatPlanPresenterEdit,true)) {
				butPickPresenter.Visible=false;
			}
			if(_treatPlan.UserNumPresenter>0) {
				_userodPresenter=Userods.GetUser(_treatPlan.UserNumPresenter);
				_userodPresenterOld=_userodPresenter.Copy();
				textPresenter.Text=_userodPresenter.UserName;
			}
			textUserEntry.Text=Userods.GetName(_treatPlan.SecUserNumEntry);
			textDateTP.Text=_treatPlan.DateTP.ToShortDateString();
			textHeading.Text=_treatPlan.Heading;
			textNote.Text=_treatPlan.Note;
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				labelResponsParty.Visible=false;
				textResponsParty.Visible=false;
				butPickResponsParty.Visible=false;
				butClearResponsParty.Visible=false;
			}
			if(_treatPlan.ResponsParty!=0){
				textResponsParty.Text=Patients.GetLim(_treatPlan.ResponsParty).GetNameLF();
			}
			if(_treatPlan.Signature!="") { //Per Nathan 01 OCT 2015: In addition to invalidating signature (old behavior) we will also block editing signed TPs.
				butSave.Enabled=false;
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
			if(_treatPlan.DocNum>0) {//Was set at some point in the past.
				Document document=Documents.GetByNum(_treatPlan.DocNum);
				if(document.DocNum==0) {
					textDocument.Text="("+Lan.g(this,"Missing Document")+")";//Invalid Fkey to document.DocNum
					butDocumentView.Enabled=false;
				}
				else {
					textDocument.Text=document.Description;
					if(!Documents.DocExists(document.DocNum)) {
						textDocument.Text+=" ("+Lan.g(this,"Unreachable File")+")";//Document points to unreachable file
						butDocumentView.Enabled=false;
					}
				}
				return;
			}
			//hide document controls because there is no attached document
			labelDocument.Visible=false;
			textDocument.Visible=false;
			butDocumentView.Visible=false;
			butDocumentDetach.Visible=false;
		}

		private void butPickResponsParty_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK){
				return;
			}
			_treatPlan.ResponsParty=formPatientSelect.PatNumSelected;
			textResponsParty.Text=Patients.GetLim(_treatPlan.ResponsParty).GetNameLF();
		}

		private void butClearResponsParty_Click(object sender,EventArgs e) {
			_treatPlan.ResponsParty=0;
			textResponsParty.Text="";
		}

		private void butDocumentView_Click(object sender,EventArgs e) {
			Document document=Documents.GetByNum(_treatPlan.DocNum);
			if(document.DocNum==0) {
				MsgBox.Show(this,"Error locating document.");
				return;
			}
			if(!Documents.DocExists(document.DocNum)) {
				MsgBox.Show(this,"Unable to open document.");
				return;
			}
			string errMsg=Documents.OpenDoc(document.DocNum);
			if(errMsg!="") {
				MsgBox.Show(errMsg);
			}
		}

		private void butDocumentDetach_Click(object sender,EventArgs e) {
			_treatPlan.DocNum=0;
			butDocumentView.Enabled=false;
			butDocumentDetach.Enabled=false;
			textDocument.Text="";
		}

		private void butSigClear_Click(object sender,EventArgs e) {
			//Cannot click this button if you are not authorized to edit, so it is safe to re-enable edit controls below.
			//Disable signature buttons
			if(!Security.IsAuthorized(EnumPermType.TreatPlanEdit,_treatPlan.DateTP)) {
				butSave.Enabled=false;
			}
			else {
				butSave.Enabled=true;
			}
			_treatPlan.Signature="";
			_treatPlan.SignatureText="";
			_treatPlan.DateTSigned=DateTime.MinValue;
			_treatPlan.SigIsTopaz=false;
			_treatPlan.SignaturePractice="";
			_treatPlan.SignaturePracticeText="";
			_treatPlan.DateTPracticeSigned=DateTime.MinValue;
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
			FrmUserPick frmUserPick=new FrmUserPick();
			List<Userod> listUserods=Userods.GetWhere(x => x.ClinicIsRestricted == false || x.ClinicNum == Clinics.ClinicNum,true);
			frmUserPick.ListUserodsFiltered=listUserods;
			if(_userodPresenter!=null) {
				frmUserPick.UserNumSuggested=_userodPresenter.UserNum;
			}
			frmUserPick.IsPickNoneAllowed=true;
			frmUserPick.ShowDialog();
			if(!frmUserPick.IsDialogOK) {
				return;
			}
			_userodPresenter=Userods.GetUser(frmUserPick.UserNumSelected);//can be null
			if(_userodPresenter!=null) {
				textPresenter.Text=_userodPresenter.UserName;
				return;
			}
			textPresenter.Text="";
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//if(IsNew){
			//	DialogResult=DialogResult.Cancel;
			//	return;
			//}
			ProcTPs.DeleteForTP(_treatPlan.TreatPlanNum);
			try{
				TreatPlans.Delete(_treatPlan);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.TreatPlanEdit,_treatPlan.PatNum,"Delete TP: "+_treatPlan.DateTP.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			#region Validation
			if(string.IsNullOrWhiteSpace(textHeading.Text)) {
				MsgBox.Show(this,"Header name cannot be empty.");
				return;
			}
			if(!textDateTP.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDateTP.Text==""){
				MsgBox.Show(this,"Please enter a date first.");
				return;
			}
			#endregion Validation
			_treatPlan.DateTP=PIn.Date(textDateTP.Text);
			_treatPlan.Heading=textHeading.Text;
			_treatPlan.Note=textNote.Text;
			if(_userodPresenter!=null) {
				_treatPlan.UserNumPresenter=_userodPresenter.UserNum;
			}
			else {
				_treatPlan.UserNumPresenter=0;
			}
			//PlanCur.SecUserNumEntry is updated automatically by MySQL.
			TreatPlans.Update(_treatPlan);
			SecurityLogs.MakeLogEntry(EnumPermType.TreatPlanEdit,_treatPlan.PatNum,"Edit TP: "+_treatPlan.DateTP.ToShortDateString());
			if(_userodPresenter!=null && (_userodPresenterOld==null || _userodPresenter.UserNum != _userodPresenterOld.UserNum)) {
				SecurityLogs.MakeLogEntry(EnumPermType.TreatPlanPresenterEdit,_treatPlan.PatNum,
					"TP Presenter Changed from "+(_userodPresenterOld==null?"\"null\"":_userodPresenterOld.UserName)+" to "+_userodPresenter.UserName+".");
			}
			DialogResult=DialogResult.OK;
		}

	}
}