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
		public long SelectedPatNum;
		public string LnameEntered;
		public string FnameEntered;
		public DateTime BdateEntered;
		///<summary>If true, more than one patient matches FName, LName, and Birthdate. If false, no matches have been found.</summary>
		public bool HasMoreThanOneMatch;
		///<summary>Indicates all downloaded websheets that match this websheet's FName, LName, Email, and Birthdate should be discarded.</summary>
		private bool _isDiscardAll=false;
		///<summary>The number of matching webforms that match this sheet.</summary>
		private int _countMatchingSheets;
		private WebForms_Sheet _webFormsSheet;
		private Sheet _sheetCemt;
		private bool _isWebForm;

		///<summary>Indicates all downloaded websheets that match this websheet's FName, LName, Email, and Birthdate should be discarded.</summary>
		public bool IsDiscardAll {
			get {
				return _isDiscardAll;
			}
		}

		public FormPatientPickWebForm(WebForms_Sheet sheetAndSheetField,int countMatchingSheets,Sheet sheetCemt=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_webFormsSheet=sheetAndSheetField;
			_sheetCemt=sheetCemt;
			_isWebForm=_webFormsSheet!=null;
			_countMatchingSheets=countMatchingSheets;
		}

		private void FormPatientPickWebForm_Load(object sender,EventArgs e) {
			textLName.Text=LnameEntered;
			textFName.Text=FnameEntered;
			textBirthdate.Text=BdateEntered.ToShortDateString();
			if(HasMoreThanOneMatch) {
				labelExplanation.Text=Lan.g(this,"More than one matching patient was found for this submitted web form.");
			}
			if(_sheetCemt!=null && _sheetCemt.SheetFields.Any(x => x.FieldName=="isTransfer" && PIn.Bool(x.FieldValue))) {
				string strCemtSendClinic=_sheetCemt.SheetFields.FirstOrDefault(x => x.FieldName=="sendClinicCEMT")?.FieldValue??"";
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
				if(_webFormsSheet.ClinicNum==0) {
					strClinic="Headquarters";
				}
				else {
					strClinic=Clinics.GetAbbr(_webFormsSheet.ClinicNum);
				}
			}
			else if(_sheetCemt!=null) {
				if(_sheetCemt.ClinicNum==0) {
					strClinic="Headquarters";
				}
				else {
					strClinic=Clinics.GetAbbr(_sheetCemt.ClinicNum);
				}
			}
			textClinic.Text=strClinic;
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			if(HasMoreThanOneMatch) {
				gridMain.Title=Lan.g(this,"Matches");
			}
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Last Name"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"First Name"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Birthdate"),110);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic Name"),110);
				gridMain.ListGridColumns.Add(col);
			}
			listPats=Patients.GetSimilarList(LnameEntered,FnameEntered,BdateEntered);
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
				sheet=SheetUtil.CreateSheetFromWebSheet(0,_webFormsSheet);
			}
			else {
				sheet=_sheetCemt;
			}
			FormSheetFillEdit.ShowForm(sheet,isReadOnly:true);
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			SelectedPatNum=listPats[e.Row].PatNum;
			//Security log for patient select.
			Patient pat=Patients.GetPat(SelectedPatNum);
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SelectedPatNum,"In the 'Pick Patient for Web Form', this user double clicked a name in the suggested list.  "
				+"This caused the web form for this patient: "+LnameEntered+", "+FnameEntered+" "+BdateEntered.ToShortDateString()+"  "
				+"to be manually attached to this other patient: "+pat.LName+", "+pat.FName+" "+pat.Birthdate.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butSelect_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPs=new FormPatientSelect();
			FormPs.SelectionModeOnly=true;
			FormPs.ShowDialog();
			if(FormPs.DialogResult!=DialogResult.OK) {
				return;
			}
			SelectedPatNum=FormPs.SelectedPatNum;
			//Security log for patient select.
			Patient pat=Patients.GetPat(SelectedPatNum);
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SelectedPatNum,"In the 'Pick Patient for Web Form', this user clicked the 'Select' button.  "
				+"By clicking the 'Select' button, the web form for this patient: "+LnameEntered+", "+FnameEntered+" "+BdateEntered.ToShortDateString()+"  "
				+"was manually attached to this other patient: "+pat.LName+", "+pat.FName+" "+pat.Birthdate.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butNew_Click(object sender,EventArgs e) {
			SelectedPatNum=0;
			DialogResult=DialogResult.OK;
		}

		private void butSkip_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Ignore;
		}		

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDiscard_Click(object sender,EventArgs e) {
			string msg=Lan.g(this,"Are you sure you want to discard this webform");
			if(_countMatchingSheets>1) {
				msg+=Lan.g(this," and all ")+(_countMatchingSheets-1)
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