using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormLetterMergeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private LetterMerge _letterMerge;
		private string mergePath;
		private List<Def> _listDefsLetterMergeCat;

		//private ArrayList ALpatSelect;

		///<summary></summary>
		public FormLetterMergeEdit(LetterMerge letterMerge)
		{
			InitializeComponent();
			InitializeLayoutManager();
			_letterMerge=letterMerge;
			Lan.F(this);
		}

		private void FormLetterMergeEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=_letterMerge.Description;
			mergePath=PrefC.GetString(PrefName.LetterMergePath);
			textPath.Text=mergePath;
			textTemplateName.Text=_letterMerge.TemplateName;
			textDataFileName.Text=_letterMerge.DataFileName;
			_listDefsLetterMergeCat=Defs.GetDefsForCategory(DefCat.LetterMergeCats,true);
			for(int i=0;i<_listDefsLetterMergeCat.Count;i++){
				comboCategory.Items.Add(_listDefsLetterMergeCat[i].ItemName);
				if(_letterMerge.Category==_listDefsLetterMergeCat[i].DefNum){
					comboCategory.SelectedIndex=i;
				}
			}
			comboImageFolder.Items.Clear();
			comboImageFolder.Items.AddDefNone();
			comboImageFolder.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ImageCats,true));
			comboImageFolder.SetSelectedDefNum(_letterMerge.ImageFolder); 
			FillPatSelect();
			FillListReferral();
			FillListOther();
		}

		private void FillPatSelect(){
			listPatSelect.Items.Clear();
			listPatSelect.Items.Add("PatNum"); 
			listPatSelect.Items.Add("LName");
			listPatSelect.Items.Add("FName");
			listPatSelect.Items.Add("MiddleI"); 
			listPatSelect.Items.Add("Preferred");
			listPatSelect.Items.Add("Title");
			listPatSelect.Items.Add("Salutation");
			listPatSelect.Items.Add("Address"); 
			listPatSelect.Items.Add("Address2");
			listPatSelect.Items.Add("City"); 
			listPatSelect.Items.Add("State");   
			listPatSelect.Items.Add("Zip");
			listPatSelect.Items.Add("HmPhone");
			listPatSelect.Items.Add("WkPhone"); 
			listPatSelect.Items.Add("WirelessPhone"); 
			listPatSelect.Items.Add("Birthdate");
			listPatSelect.Items.Add("Email");
			listPatSelect.Items.Add("SSN");
			listPatSelect.Items.Add("Gender");
			listPatSelect.Items.Add("PatStatus");
			listPatSelect.Items.Add("Position");  
			listPatSelect.Items.Add("CreditType");
			listPatSelect.Items.Add("BillingType"); 
			listPatSelect.Items.Add("ChartNumber");   
			listPatSelect.Items.Add("PriProv"); 
			listPatSelect.Items.Add("SecProv");
			listPatSelect.Items.Add("FeeSched"); 
			listPatSelect.Items.Add("ApptModNote");
			listPatSelect.Items.Add("AddrNote"); 
			listPatSelect.Items.Add("EstBalance"); 
			listPatSelect.Items.Add("FamFinUrgNote"); 
			listPatSelect.Items.Add("Guarantor");   
			listPatSelect.Items.Add("ImageFolder");
			listPatSelect.Items.Add("MedUrgNote"); 
			listPatSelect.Items.Add("NextAptNum"); 
			listPatSelect.Items.Add("SchoolName"); 
			listPatSelect.Items.Add("StudentStatus");
			listPatSelect.Items.Add("MedicaidID");
			listPatSelect.Items.Add("Bal_0_30");
			listPatSelect.Items.Add("Bal_31_60");
			listPatSelect.Items.Add("Bal_61_90");
			listPatSelect.Items.Add("BalOver90");
			listPatSelect.Items.Add("InsEst");
			listPatSelect.Items.Add("BalTotal");
			listPatSelect.Items.Add("EmployerNum");
			listPatSelect.Items.Add("Race"); //Race is depricated in the patient table, we get it from the PatientRace table entries converted into a PatientRaceOld enum value.
			listPatSelect.Items.Add("County");
			listPatSelect.Items.Add("GradeSchool");
			listPatSelect.Items.Add("GradeLevel");
			listPatSelect.Items.Add("Urgency");
			listPatSelect.Items.Add("DateFirstVisit");
			for(int i=0;i<_letterMerge.Fields.Count;i++){
				for(int j=0;j<listPatSelect.Items.Count;j++){
					if(listPatSelect.Items.GetTextShowingAt(j)==(string)_letterMerge.Fields[i]){
						listPatSelect.SetSelected(j);
					}
				}
			}
		}

		private void FillListReferral(){
			listReferral.Items.Add("LName");
			listReferral.Items.Add("FName");
			listReferral.Items.Add("MName");       
			listReferral.Items.Add("Title"); 
			listReferral.Items.Add("Address"); 
			listReferral.Items.Add("Address2");
			listReferral.Items.Add("City");
			listReferral.Items.Add("ST");
			listReferral.Items.Add("Zip");
			listReferral.Items.Add("Telephone");
			listReferral.Items.Add("Phone2"); 
			listReferral.Items.Add("Email");
			listReferral.Items.Add("IsHidden"); 
			listReferral.Items.Add("NotPerson");  
			listReferral.Items.Add("PatNum"); 
			listReferral.Items.Add("ReferralNum");
			listReferral.Items.Add("Specialty"); 
			listReferral.Items.Add("SSN");
			listReferral.Items.Add("UsingTIN"); 
			listReferral.Items.Add("Note");
			for(int i=0;i<_letterMerge.Fields.Count;i++){
				for(int j=0;j<listReferral.Items.Count;j++){
					if("referral."+listReferral.Items.GetTextShowingAt(j)==(string)_letterMerge.Fields[i]){
						listReferral.SetSelected(j);
					}
				}
			}
		}

		private void FillListOther(){
			listOther.Items.Add("TPResponsPartyNameFL");
			listOther.Items.Add("TPResponsPartyAddress");
			listOther.Items.Add("TPResponsPartyCityStZip");
			listOther.Items.Add("SiteDescription");
			listOther.Items.Add("DateOfLastSavedTP");
			listOther.Items.Add("DateRecallDue");
			listOther.Items.Add("CarrierName");
			listOther.Items.Add("CarrierAddress");
			listOther.Items.Add("CarrierAddress2");
			listOther.Items.Add("CarrierCityStZip");
			listOther.Items.Add("SubscriberNameFL");
			listOther.Items.Add("SubscriberID");
			listOther.Items.Add("NextSchedAppt");
			listOther.Items.Add("Age");
			for(int i=0;i<_letterMerge.Fields.Count;i++){
				for(int j=0;j<listOther.Items.Count;j++){
					if(listOther.Items.GetTextShowingAt(j)==(string)_letterMerge.Fields[i]){
						listOther.SetSelected(j);
					}
				}
			}
		}

		private void butEditPaths_Click(object sender, System.EventArgs e) {
			using FormPath formPath=new FormPath();
			formPath.ShowDialog();
			mergePath=PrefC.GetString(PrefName.LetterMergePath);
			textPath.Text=mergePath;
		}

		private void butBrowse_Click(object sender, System.EventArgs e) {
			if(!Directory.Exists(PrefC.GetString(PrefName.LetterMergePath))){
				MsgBox.Show(this,"Letter merge path invalid");
				return;
			}
			openFileDlg.InitialDirectory=PrefC.GetString(PrefName.LetterMergePath);
			if(openFileDlg.ShowDialog() !=DialogResult.OK){
				openFileDlg.Dispose();
				return;
			}
			textTemplateName.Text=Path.GetFileName(openFileDlg.FileName);
			openFileDlg.Dispose();
		}

		private void butNew_Click(object sender, System.EventArgs e) {
			#if DISABLE_MICROSOFT_OFFICE
				MessageBox.Show(this, "This version of Open Dental does not support Microsoft Word.");
				return;
			#endif
			if(!Directory.Exists(PrefC.GetString(PrefName.LetterMergePath))){
				MsgBox.Show(this,"Letter merge path invalid");
				return;
			}
			if(textTemplateName.Text==""){
				MsgBox.Show(this,"Please enter a template file name first.");
				return;
			}
			string templateFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),textTemplateName.Text);
			if(File.Exists(templateFile)){
				MsgBox.Show(this,"A file with that name already exists.  Choose a different name, or close this window to edit the template.");
				return;
			}
			Object objectMissing=System.Reflection.Missing.Value;
			Object objectFalse=false;
			//Create an instance of Word.
			Word.Application word_Application;
			try {
				word_Application=LetterMerges.GetWordApp();
			}
			catch {
				MsgBox.Show(this,"Error. Is MS Word installed?");
				return;
			}
			//Create a new document.
			Object objectName=templateFile;
			Word._Document word_Document;
			word_Document=word_Application.Documents.Add(ref objectMissing,ref objectMissing,ref objectMissing,
				ref objectMissing);
			word_Document.SaveAs(ref objectName,ref objectMissing,ref objectMissing,ref objectMissing,ref objectMissing,
				ref objectMissing,ref objectMissing,ref objectMissing,ref objectMissing,ref objectMissing,ref objectMissing,
				ref objectMissing,ref objectMissing,ref objectMissing,ref objectMissing,ref objectMissing);
			word_Document.Saved=true;
			word_Document.Close(ref objectFalse,ref objectMissing,ref objectMissing);
			word_Application.WindowState=Word.WdWindowState.wdWindowStateMinimize;
			word_Document=null;
			MsgBox.Show(this,"Done. You can edit the new template after closing this window.");
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			LetterMerges.Delete(_letterMerge);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description");
				return;
			}
			if(this.textDataFileName.Text==""
				|| this.textTemplateName.Text=="")
			{
				MsgBox.Show(this,"Filenames cannot be left blank.");
				return;
			}
			if(comboCategory.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a category");
				return;
			}
			if(listPatSelect.SelectedIndices.Count==0
				&& listReferral.SelectedIndices.Count==0)
			{
				MsgBox.Show(this,"Please select at least one field.");
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			_letterMerge.Description=textDescription.Text;
			_letterMerge.TemplateName=textTemplateName.Text;
			_letterMerge.DataFileName=textDataFileName.Text;
			_letterMerge.Category=_listDefsLetterMergeCat[comboCategory.SelectedIndex].DefNum;
			_letterMerge.ImageFolder=comboImageFolder.GetSelectedDefNum();
			if(IsNew){
				LetterMerges.Insert(_letterMerge);
			}
			else{
				LetterMerges.Update(_letterMerge);
			}
			LetterMergeFields.DeleteForLetter(_letterMerge.LetterMergeNum);
			LetterMergeField letterMergeField;
			for(int i=0;i<listPatSelect.SelectedIndices.Count;i++){
				letterMergeField=new LetterMergeField();
				letterMergeField.LetterMergeNum=_letterMerge.LetterMergeNum;
				letterMergeField.FieldName=listPatSelect.Items.GetTextShowingAt(listPatSelect.SelectedIndices[i]);
				LetterMergeFields.Insert(letterMergeField);
			}
			for(int i=0;i<listReferral.SelectedIndices.Count;i++){
				letterMergeField=new LetterMergeField();
				letterMergeField.LetterMergeNum=_letterMerge.LetterMergeNum;
				letterMergeField.FieldName="referral."+listReferral.Items.GetTextShowingAt(listReferral.SelectedIndices[i]);
				LetterMergeFields.Insert(letterMergeField);
			}
			for(int i=0;i<listOther.SelectedIndices.Count;i++){
				letterMergeField=new LetterMergeField();
				letterMergeField.LetterMergeNum=_letterMerge.LetterMergeNum;
				letterMergeField.FieldName=listOther.Items.GetTextShowingAt(listOther.SelectedIndices[i]);
				LetterMergeFields.Insert(letterMergeField);
			}
			Cursor.Current=Cursors.Default;
			DialogResult=DialogResult.OK;
		}

	}
}