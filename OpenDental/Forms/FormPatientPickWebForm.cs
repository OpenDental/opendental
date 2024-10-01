using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using OpenDentBusiness.WebTypes.WebForms;
using System.Linq;

namespace OpenDental {
	public partial class FormPatientPickWebForm:FormODBase {
		private List<Patient> listPats;
		///<summary>If OK.  Can be zero to indicate create new patient.  A result of Cancel indicates quit importing altogether.</summary>
		public long PatNumSelected;
		public string LnameEntered;
		public string FnameEntered;
		public DateTime DateBirthEntered;
		///<summary>If true, more than one patient matches FName, LName, and Birthdate. If false, no matches have been found.</summary>
		public bool HasMoreThanOneMatch;
		///<summary>Indicates all downloaded websheets that match this websheet's FName, LName, Email, and Birthdate should be discarded.</summary>
		private bool _isDiscardAll=false;
		///<summary>The number of matching webforms that match this sheet.</summary>
		public int CountMatchingSheets;
		public WebForms_Sheet WebFormsSheetCur;
		public Sheet SheetCemt;
		private bool _isWebForm;

		///<summary>Indicates all downloaded websheets that match this websheet's FName, LName, Email, and Birthdate should be discarded.</summary>
		public bool IsDiscardAll() {
			return _isDiscardAll;
		}

		public FormPatientPickWebForm() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientPickWebForm_Load(object sender,EventArgs e) {
			_isWebForm=WebFormsSheetCur!=null;
			textLName.Text=LnameEntered;
			textFName.Text=FnameEntered;
			textBirthdate.Text=DateBirthEntered.ToShortDateString();
			if(HasMoreThanOneMatch) {
				labelExplanation.Text=Lan.g(this,"More than one matching patient was found for this submitted web form.");
			}
			if(SheetCemt!=null && SheetCemt.SheetFields.Any(x => x.FieldName=="isTransfer" && PIn.Bool(x.FieldValue))) {
				string strCemtSendClinic=SheetCemt.SheetFields.FirstOrDefault(x => x.FieldName=="sendClinicCEMT")?.FieldValue??"";
				if(!string.IsNullOrEmpty(strCemtSendClinic)) {
					labelExplanation.Text+="\r\n"+Lan.g(this,"Patient was transferred from clinic:")+"  "+strCemtSendClinic;
				}
			}
			FillGrid();
			if(PrefC.HasClinicsEnabled) {
				textClinic.Visible=true;
				textClinic.Enabled=false;
				labelClinic.Visible=true;
				FillClinicText();
			}
		}

		private void FillClinicText() {
			string strClinic="";
			if(_isWebForm) {
				if(WebFormsSheetCur.ClinicNum==0) {
					strClinic="Headquarters";
				}
				else {
					strClinic=Clinics.GetAbbr(WebFormsSheetCur.ClinicNum);
				}
			}
			else if(SheetCemt!=null) {
				if(SheetCemt.ClinicNum==0) {
					strClinic="Headquarters";
				}
				else {
					strClinic=Clinics.GetAbbr(SheetCemt.ClinicNum);
				}
			}
			textClinic.Text=strClinic;
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			if(HasMoreThanOneMatch) {
				gridMain.Title=Lan.g(this,"Matches");
			}
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Last Name"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"First Name"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Birthdate"),110);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic Name"),110);
				gridMain.Columns.Add(col);
			}
			listPats=Patients.GetSimilarList(LnameEntered,FnameEntered,DateBirthEntered);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listPats.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listPats[i].LName);
				row.Cells.Add(listPats[i].FName);
				row.Cells.Add(listPats[i].Birthdate.ToShortDateString());
				if(PrefC.HasClinicsEnabled) {
					string clinicName=Clinics.GetDesc(listPats[i].ClinicNum);
					row.Cells.Add(!string.IsNullOrWhiteSpace(clinicName) ? clinicName : "N/A");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butView_Click(object sender,EventArgs e) {
			Sheet sheet;
			if(_isWebForm) {
				sheet=SheetUtil.CreateSheetFromWebSheet(0,WebFormsSheetCur);
			}
			else {
				sheet=SheetCemt;
			}
			FormSheetFillEdit.ShowForm(sheet,isReadOnly:true);
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			PatNumSelected=listPats[e.Row].PatNum;
			//Security log for patient select.
			Patient patient=Patients.GetPat(PatNumSelected);
			SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,PatNumSelected,"In the 'Pick Patient for Web Form', this user double clicked a name in the suggested list.  "
				+"This caused the web form for this patient: "+LnameEntered+", "+FnameEntered+" "+DateBirthEntered.ToShortDateString()+"  "
				+"to be manually attached to this other patient: "+patient.LName+", "+patient.FName+" "+patient.Birthdate.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butSelect_Click(object sender,EventArgs e) {
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel) {
				return;
			}
			PatNumSelected=frmPatientSelect.PatNumSelected;
			//Security log for patient select.
			Patient patient=Patients.GetPat(PatNumSelected);
			SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,PatNumSelected,"In the 'Pick Patient for Web Form', this user clicked the 'Select' button.  "
				+"By clicking the 'Select' button, the web form for this patient: "+LnameEntered+", "+FnameEntered+" "+DateBirthEntered.ToShortDateString()+"  "
				+"was manually attached to this other patient: "+patient.LName+", "+patient.FName+" "+patient.Birthdate.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butNew_Click(object sender,EventArgs e) {
			PatNumSelected=0;
			DialogResult=DialogResult.OK;
		}

		private void butSkip_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Ignore;
		}

		private void butDiscard_Click(object sender,EventArgs e) {
			string msg=Lan.g(this,"Are you sure you want to discard this webform");
			if(CountMatchingSheets>1) {
				msg+=Lan.g(this," and all ")+(CountMatchingSheets-1)
					+Lan.g(this," remaining webforms that match this name, birthdate, email, and phone numbers?");
			}
			else {
				msg+="? "+Lan.g(this,"There are no other matching forms for this patient")+".";
			}
			msg+=Lan.g(this," Discarded webforms will not be able to be accessed later.");
			if(MessageBox.Show(this,msg,Lan.g(this,"Discard WebForms?"),MessageBoxButtons.YesNo)==DialogResult.Yes) {
				_isDiscardAll=true;
				DialogResult=DialogResult.Ignore;
			}
		}

	}
}