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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLetterMergeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private LetterMerge LetterMergeCur;
		private string mergePath;
		private List<Def> _listLetterMergeCatDefs;

		//private ArrayList ALpatSelect;

		///<summary></summary>
		public FormLetterMergeEdit(LetterMerge letterMergeCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			LetterMergeCur=letterMergeCur;
			Lan.F(this);
		}

		private void FormLetterMergeEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=LetterMergeCur.Description;
			mergePath=PrefC.GetString(PrefName.LetterMergePath);
			textPath.Text=mergePath;
			textTemplateName.Text=LetterMergeCur.TemplateName;
			textDataFileName.Text=LetterMergeCur.DataFileName;
			_listLetterMergeCatDefs=Defs.GetDefsForCategory(DefCat.LetterMergeCats,true);
			for(int i=0;i<_listLetterMergeCatDefs.Count;i++){
				comboCategory.Items.Add(_listLetterMergeCatDefs[i].ItemName);
				if(LetterMergeCur.Category==_listLetterMergeCatDefs[i].DefNum){
					comboCategory.SelectedIndex=i;
				}
			}
			comboImageFolder.Items.Clear();
			comboImageFolder.Items.AddDefNone();
			comboImageFolder.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ImageCats,true));
			comboImageFolder.SetSelectedDefNum(LetterMergeCur.ImageFolder); 
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
			for(int i=0;i<LetterMergeCur.Fields.Count;i++){
				for(int j=0;j<listPatSelect.Items.Count;j++){
					if(listPatSelect.Items.GetTextShowingAt(j)==(string)LetterMergeCur.Fields[i]){
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
			for(int i=0;i<LetterMergeCur.Fields.Count;i++){
				for(int j=0;j<listReferral.Items.Count;j++){
					if("referral."+listReferral.Items.GetTextShowingAt(j)==(string)LetterMergeCur.Fields[i]){
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
			for(int i=0;i<LetterMergeCur.Fields.Count;i++){
				for(int j=0;j<listOther.Items.Count;j++){
					if(listOther.Items.GetTextShowingAt(j)==(string)LetterMergeCur.Fields[i]){
						listOther.SetSelected(j);
					}
				}
			}
    }

		private void butEditPaths_Click(object sender, System.EventArgs e) {
			using FormPath FormP=new FormPath();
			FormP.ShowDialog();
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
			Object oMissing=System.Reflection.Missing.Value;
			Object oFalse=false;
			//Create an instance of Word.
			Word.Application WrdApp;
			try{
				WrdApp=LetterMerges.WordApp;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error. Is MS Word installed?"),ex);
				return;
			}
			//Create a new document.
			Object oName=templateFile;
			Word._Document wrdDoc;
			wrdDoc=WrdApp.Documents.Add(ref oMissing,ref oMissing,ref oMissing,
				ref oMissing);
			wrdDoc.SaveAs(ref oName,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
			wrdDoc.Saved=true;
			wrdDoc.Close(ref oFalse,ref oMissing,ref oMissing);
			WrdApp.WindowState=Word.WdWindowState.wdWindowStateMinimize;
			wrdDoc=null;
			MsgBox.Show(this,"Done. You can edit the new template after closing this window.");
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			LetterMerges.Delete(LetterMergeCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
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
			LetterMergeCur.Description=textDescription.Text;
			LetterMergeCur.TemplateName=textTemplateName.Text;
			LetterMergeCur.DataFileName=textDataFileName.Text;
			LetterMergeCur.Category=_listLetterMergeCatDefs[comboCategory.SelectedIndex].DefNum;
			LetterMergeCur.ImageFolder=comboImageFolder.GetSelectedDefNum();
			if(IsNew){
				LetterMerges.Insert(LetterMergeCur);
			}
			else{
				LetterMerges.Update(LetterMergeCur);
			}
			LetterMergeFields.DeleteForLetter(LetterMergeCur.LetterMergeNum);
			LetterMergeField field;
			for(int i=0;i<listPatSelect.SelectedIndices.Count;i++){
				field=new LetterMergeField();
				field.LetterMergeNum=LetterMergeCur.LetterMergeNum;
				field.FieldName=listPatSelect.Items.GetTextShowingAt(listPatSelect.SelectedIndices[i]);
				LetterMergeFields.Insert(field);
			}
			for(int i=0;i<listReferral.SelectedIndices.Count;i++){
				field=new LetterMergeField();
				field.LetterMergeNum=LetterMergeCur.LetterMergeNum;
				field.FieldName="referral."+listReferral.Items.GetTextShowingAt(listReferral.SelectedIndices[i]);
				LetterMergeFields.Insert(field);
			}
			for(int i=0;i<listOther.SelectedIndices.Count;i++){
				field=new LetterMergeField();
				field.LetterMergeNum=LetterMergeCur.LetterMergeNum;
				field.FieldName=listOther.Items.GetTextShowingAt(listOther.SelectedIndices[i]);
				LetterMergeFields.Insert(field);
			}
			Cursor.Current=Cursors.Default;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		


	}
}





















