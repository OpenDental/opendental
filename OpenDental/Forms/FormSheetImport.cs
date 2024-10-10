using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace OpenDental {
	///<summary>Also handles eForm import.</summary>
	public partial class FormSheetImport:FormODBase {
		public Sheet SheetCur;
		public EForm EFormCur;

		private List<ImportRow> _listImportRows;
		private Patient _patient;
		///<summary>A copy of the patient when the form loads.  Used to know the what will change upon import.</summary>
		private Patient _patientOld;
		private PatientNote _patientNote;
		private PatientNote _patientNoteOld;
		private Family _family;
		///<summary>We must have a readily available bool, whether or not this checkbox field is present on the sheet.  It gets set at the very beginning, then gets changes based on user input on the sheet and in this window.</summary>
		private bool _isAddressSameForFam;
		private InsPlan _insPlan1;
		private InsPlan _insPlan2;
		private List<PatPlan> _listPatPlans;
		private List<InsPlan> _listInsPlans;
		private PatPlan _patPlan1;
		private PatPlan _patPlan2;
		private Relat? _relatIns1;
		private Relat? _relatIns2;
		private Carrier _carrier1;
		private Carrier _carrier2;
		private List<InsSub> _listInsSubs;
		private InsSub _insSub1;
		private InsSub _insSub2;
		private OcrInsScanResponse _ocrResponsePrimaryFront;
		private OcrInsScanResponse _ocrResponsePrimaryBack;
		private OcrInsScanResponse _ocrResponseSecondaryFront;
		private OcrInsScanResponse _ocrResponseSecondaryBack;
		///<summary>In order to import insurance plans the sheet must contain Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone.  This variable gets set when the sheet loads and will indicate if all fields are present for primary OR for secondary insurance.  Insurance should not attempt to import if this is false.</summary>
		private bool _hasRequiredInsFields;
		private bool _hasSectionPersonal;
		private bool _hasSectionAddrHmPhone;
		private bool _hasSectionIns1;
		private bool _hasSectionIns2;
		private bool _hasSectionAllergies;
		private bool _hasSectionMeds;
		private bool _hasSectionProblems;
		private bool _isPatTransferSheet;

		private const string INVALID_DATE="Invalid Date";

		public FormSheetImport() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//Hide borders so that images that are not set are totally invisible. Do so programatically so we can see them in the designer.
			pictureBoxPrimaryInsuranceFront.HasBorder=false;
			pictureBoxPrimaryInsuranceBack.HasBorder=false;
			pictureBoxSecondaryInsuranceFront.HasBorder=false;
			pictureBoxSecondaryInsuranceBack.HasBorder=false;
		}

		private void FormSheetImport_Load(object sender,EventArgs e) {
			if(SheetCur!=null) {
				_patient=Patients.GetPat(SheetCur.PatNum);
				if(SheetCur.SheetFields!=null) {
					_isPatTransferSheet=SheetCur.SheetFields.Any(x => x.FieldName=="isTransfer" && PIn.Bool(x.FieldValue));
				}
			}
			else if(EFormCur!=null){
				_patient=Patients.GetPat(EFormCur.PatNum);
				Text="eForm Import";
				gridMain.Title="eForm Import";
			}
			else {
				throw new NotImplementedException();//js PDF import broke with the move to dot net 4.0.
			}
			_patientOld=_patient.Copy();
			_patientNote=PatientNotes.Refresh(_patient.PatNum,_patient.Guarantor);
			_patientNoteOld=_patientNote.Copy();
			//pre-initialize ocrData to blank. Do this even if we dont have ocr docs. Its easier than adding null checks.
			_ocrResponsePrimaryFront=CreateBlankOcrInsScanResponse();
			_ocrResponsePrimaryBack=CreateBlankOcrInsScanResponse();
			_ocrResponseSecondaryFront=CreateBlankOcrInsScanResponse();
			_ocrResponseSecondaryBack=CreateBlankOcrInsScanResponse();
			//Get list documents for patient Documents.GetPatientData Order by date descending
			List<Document> listDocumentsForInsScans=Documents.GetOcrDocumentsForPat(_patient.PatNum);
			if(!listDocumentsForInsScans.IsNullOrEmpty()) {
				Document documentPrimaryInsFront=listDocumentsForInsScans.Find(x=>x.ImageCaptureType==EnumOcrCaptureType.PrimaryInsFront);
				Document documentPrimaryInsBack=listDocumentsForInsScans.Find(x=>x.ImageCaptureType==EnumOcrCaptureType.PrimaryInsBack);
				Document documentSecondaryInsFront=listDocumentsForInsScans.Find(x=>x.ImageCaptureType==EnumOcrCaptureType.SecondaryInsFront);
				Document documentSecondaryInsBack=listDocumentsForInsScans.Find(x=>x.ImageCaptureType==EnumOcrCaptureType.SecondaryInsBack);
				//Get the images for the OcrData. its fine if we fail to get them.
				if(documentPrimaryInsFront!=null){
					_ocrResponsePrimaryFront=LoadOcrDataFromDocHelper(documentPrimaryInsFront);
				}
				if(documentPrimaryInsBack!=null){
					_ocrResponsePrimaryBack=LoadOcrDataFromDocHelper(documentPrimaryInsBack);
				}
				if(documentSecondaryInsFront!=null){
					_ocrResponseSecondaryFront=LoadOcrDataFromDocHelper(documentSecondaryInsFront);
				}
				if(documentSecondaryInsBack!=null){
					_ocrResponseSecondaryBack=LoadOcrDataFromDocHelper(documentSecondaryInsBack);
				}
			}
			_family=Patients.GetFamily(_patient.PatNum);
			_isAddressSameForFam=true;
			for(int i=0;i<_family.ListPats.Length;i++) {
				if(_patient.HmPhone!=_family.ListPats[i].HmPhone
					|| _patient.Address!=_family.ListPats[i].Address
					|| _patient.Address2!=_family.ListPats[i].Address2
					|| _patient.City!=_family.ListPats[i].City
					|| _patient.State!=_family.ListPats[i].State
					|| _patient.Zip!=_family.ListPats[i].Zip) 
				{
					_isAddressSameForFam=false;
					break;
				}
			}
			_listPatPlans=PatPlans.Refresh(_patient.PatNum);
			_listInsSubs=InsSubs.RefreshForFam(_family);
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			if(_listPatPlans.Count==0) {
				_patPlan1=null;
				_insPlan1=null;
				_insSub1=null;
				_relatIns1=null;
				_carrier1=null;
			}
			else {
				_patPlan1=_listPatPlans[0];
				_insSub1=InsSubs.GetSub(_patPlan1.InsSubNum,_listInsSubs);
				_insPlan1=InsPlans.GetPlan(_insSub1.PlanNum,_listInsPlans);
				_relatIns1=_patPlan1.Relationship;
				_carrier1=Carriers.GetCarrier(_insPlan1.CarrierNum);
			}
			if(_listPatPlans.Count<2) {
				_patPlan2=null;
				_insPlan2=null;
				_insSub2=null;
				_relatIns2=null;
				_carrier2=null;
			}
			else {
				_patPlan2=_listPatPlans[1];
				_insSub2=InsSubs.GetSub(_patPlan2.InsSubNum,_listInsSubs);
				_insPlan2=InsPlans.GetPlan(_insSub2.PlanNum,_listInsPlans);
				_relatIns2=_patPlan2.Relationship;
				_carrier2=Carriers.GetCarrier(_insPlan2.CarrierNum);
			}
			FillRows();
			FillGrid();
			//All the fields have been loaded on the sheet at this point.  Set the required insurance boolean if this is a patient form.
			if(SheetCur!=null && SheetCur.SheetType==SheetTypeEnum.PatientForm) {
				SetHasRequiredInsFields();
			}
			if(EFormCur!=null){
				SetHasRequiredInsFields();
			}
		}

		///<summary>This can only be run once when the form first opens.  After that, the rows are just edited.</summary>
		private void FillRows() {
			_listImportRows=new List<ImportRow>();
			ImportRow importRow;
			string fieldVal;
			#region Sections
			_hasSectionPersonal=false;
			List<string> listFieldNamesPersonal=new List<string>(){
				"LName","FName","MiddleI","Preferred","Gender","Position","Birthdate","SSN","WkPhone",
				"WirelessPhone","wirelessCarrier","Email","PreferContactMethod","PreferConfirmMethod",
				"PreferRecallMethod","referredFrom","ICEName","ICEPhone"
			};
			if(EFormCur!=null && EFormCur.ListEFormFields.Exists(x=>listFieldNamesPersonal.Contains(x.DbLink))){
				_hasSectionPersonal=true;
			}
			if(SheetCur!=null && SheetCur.SheetFields.Exists(x=>listFieldNamesPersonal.Contains(x.FieldName))){
				_hasSectionPersonal=true;
			}
			_hasSectionAddrHmPhone=false;
			List<string> listFieldNamesAddr=new List<string>(){
				"addressAndHmPhoneIsSameEntireFamily","Address","Address2","City","State","StateNoValidation","Zip",
				"HmPhone"
			};
			if(EFormCur!=null && EFormCur.ListEFormFields.Exists(x=>listFieldNamesAddr.Contains(x.DbLink))){
				_hasSectionAddrHmPhone=true;
			}
			if(SheetCur!=null && SheetCur.SheetFields.Exists(x=>listFieldNamesAddr.Contains(x.FieldName))){
				_hasSectionAddrHmPhone=true;
			}
			_hasSectionIns1=false;
			List<string> listFieldNamesIns1=new List<string>(){
				"ins1Relat","ins1SubscriberNameF","ins1SubscriberID","ins1CarrierName","ins1CarrierPhone",
				"ins1EmployerName","ins1GroupName","ins1GroupNum"
			};
			if(EFormCur!=null && EFormCur.ListEFormFields.Exists(x=>listFieldNamesIns1.Contains(x.DbLink))){
				_hasSectionIns1=true;
			}
			if(SheetCur!=null && SheetCur.SheetFields.Exists(x=>listFieldNamesIns1.Contains(x.FieldName))){
				_hasSectionIns1=true;
			}
			_hasSectionIns2=false;
			List<string> listFieldNamesIns2=new List<string>(){
				"ins2Relat","ins2SubscriberNameF","ins2SubscriberID","ins2CarrierName","ins2CarrierPhone",
				"ins2EmployerName","ins2GroupName","ins2GroupNum"
			};
			if(EFormCur!=null && EFormCur.ListEFormFields.Exists(x=>listFieldNamesIns2.Contains(x.DbLink))){
				_hasSectionIns2=true;
			}
			if(SheetCur!=null && SheetCur.SheetFields.Exists(x=>listFieldNamesIns2.Contains(x.FieldName))){
				_hasSectionIns2=true;
			}
			_hasSectionAllergies=false;
			if(EFormCur!=null){
				if(EFormCur.ListEFormFields.Exists(x=>
					x.DbLink.In("allergiesNone","allergiesOther")
					|| x.DbLink.StartsWith("allergy:")
				)){
					_hasSectionAllergies=true;
				}
			}
			if(SheetCur!=null){
				if(SheetCur.SheetFields.Exists(x=>
					x.FieldName.StartsWith("allergy:")
				)){
					_hasSectionAllergies=true;
				}
			}
			_hasSectionMeds=false;
			if(EFormCur!=null){
				if(EFormCur.ListEFormFields.Exists(x=>
					x.FieldType==EnumEFormFieldType.MedicationList
				)){
					_hasSectionMeds=true;
				}
			}
			if(SheetCur!=null){
				if(SheetCur.SheetFields.Exists(x=>
					x.FieldName.StartsWith("inputMed")
					|| x.FieldName.StartsWith("checkMed")
				)){
					_hasSectionMeds=true;
				}
			}
			_hasSectionProblems=false;
			if(EFormCur!=null){
				if(EFormCur.ListEFormFields.Exists(x=>
					x.DbLink.In("problemsNone","problemsOther")
					|| x.DbLink.StartsWith("problem:")
				)){
					_hasSectionProblems=true;
				}
			}
			if(SheetCur!=null){
				if(SheetCur.SheetFields.Exists(x=>
					x.FieldName.StartsWith("problem:")
				)){
					_hasSectionProblems=true;
				}
			}
			#endregion Sections
			#region Patient Form
			if(EFormCur!=null
				|| (SheetCur!=null && SheetCur.SheetType==SheetTypeEnum.PatientForm)) 
			{
				if(_hasSectionPersonal){
					_listImportRows.Add(CreateSeparator("Personal"));
				}
				#region personal
				//LName---------------------------------------------
				fieldVal=GetInputValue("LName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="LName";
					importRow.OldValDisplay=_patient.LName;
					importRow.OldValObj=_patient.LName;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//FName---------------------------------------------
				fieldVal=GetInputValue("FName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="FName";
					importRow.OldValDisplay=_patient.FName;
					importRow.OldValObj=_patient.FName;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//MiddleI---------------------------------------------
				fieldVal=GetInputValue("MiddleI");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="MiddleI";
					importRow.OldValDisplay=_patient.MiddleI;
					importRow.OldValObj=_patient.MiddleI;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Preferred---------------------------------------------
				fieldVal=GetInputValue("Preferred");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="Preferred";
					importRow.OldValDisplay=_patient.Preferred;
					importRow.OldValObj=_patient.Preferred;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Gender---------------------------------------------
				fieldVal=GetRadioValue("Gender");
				if(fieldVal!=null) {//field exists on form
					importRow=new ImportRow();
					importRow.FieldName="Gender";
					importRow.OldValDisplay=Lan.g("enumPatientGender",_patient.Gender.ToString());
					importRow.OldValObj=_patient.Gender;
					if(fieldVal=="") {//no box was checked
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							PatientGender patientGender=(PatientGender)Enum.Parse(typeof(PatientGender),fieldVal);
							importRow.NewValDisplay=Lan.g("enumPatientGender",patientGender.ToString());
							importRow.NewValObj=patientGender;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid gender."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(PatientGender);
					if(importRow.NewValObj!=null && (PatientGender)importRow.NewValObj!=_patient.Gender) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Position---------------------------------------------
				fieldVal=GetRadioValue("Position");
				if(fieldVal!=null) {//field exists on form
					importRow=new ImportRow();
					importRow.FieldName="Position";
					importRow.OldValDisplay=Lan.g("enumPatientPositionr",_patient.Position.ToString());
					importRow.OldValObj=_patient.Position;
					if(fieldVal=="") {//no box was checked
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							PatientPosition patientPosition=(PatientPosition)Enum.Parse(typeof(PatientPosition),fieldVal);
							importRow.NewValDisplay=Lan.g("enumPatientPosition",patientPosition.ToString());
							importRow.NewValObj=patientPosition;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid PatientPosition."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(PatientPosition);
					if(importRow.NewValObj!=null && (PatientPosition)importRow.NewValObj!=_patient.Position) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Birthdate---------------------------------------------
				fieldVal=GetInputValue("Birthdate");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="Birthdate";
					if(_patient.Birthdate.Year<1880) {
						importRow.OldValDisplay="";
					}
					else {
						importRow.OldValDisplay=_patient.Birthdate.ToShortDateString();
					}
					importRow.OldValObj=_patient.Birthdate;
					if(EFormCur!=null){
						importRow.NewValObj=PIn.Date(fieldVal);
					}
					if(SheetCur!=null){
						importRow.NewValObj=SheetFields.GetBirthDate(fieldVal,SheetCur.IsWebForm,SheetCur.IsCemtTransfer);
					}
					if(string.IsNullOrWhiteSpace(fieldVal)) {//Patient entered blank date, consider this to be valid blank date.
						importRow.NewValDisplay="";
						importRow.ImpValDisplay="";
					}
					else if(((DateTime)importRow.NewValObj).Year<1880) {//Patient entered date in incorrect format, consider this invalid (imports as MinValue)
						importRow.NewValDisplay=fieldVal;
						importRow.ImpValDisplay=INVALID_DATE;
						importRow.IsImportRed=true;
					}
					else {
						importRow.NewValDisplay=((DateTime)importRow.NewValObj).ToShortDateString();//Correct formatting, valid date.
						importRow.ImpValDisplay=importRow.NewValDisplay;
					}
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(DateTime);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//SSN---------------------------------------------
				fieldVal=GetInputValue("SSN");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="SSN";
					importRow.OldValDisplay=_patient.SSN;
					importRow.OldValObj=_patient.SSN;
					importRow.NewValDisplay=fieldVal.Replace("-","");//quickly strip dashes
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//WkPhone---------------------------------------------
				fieldVal=GetInputValue("WkPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="WkPhone";
					importRow.OldValDisplay=_patient.WkPhone;
					importRow.OldValObj=_patient.WkPhone;
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//WirelessPhone---------------------------------------------
				fieldVal=GetInputValue("WirelessPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="WirelessPhone";
					importRow.OldValDisplay=_patient.WirelessPhone;
					importRow.OldValObj=_patient.WirelessPhone;
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//wirelessCarrier---------------------------------------------
				fieldVal=GetInputValue("wirelessCarrier");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="wirelessCarrier";
					importRow.OldValDisplay="";
					importRow.OldValObj="";
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					importRow.IsNewValRed=true;//if user entered nothing, the red text won't show anyway.
					_listImportRows.Add(importRow);
				}
				//Email---------------------------------------------
				fieldVal=GetInputValue("Email");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="Email";
					importRow.OldValDisplay=_patient.Email;
					importRow.OldValObj=_patient.Email;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//PreferContactMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferContactMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferContactMethod");
				}
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="PreferContactMethod";
					importRow.OldValDisplay=Lan.g("enumContactMethod",_patient.PreferContactMethod.ToString());
					importRow.OldValObj=_patient.PreferContactMethod;
					if(fieldVal=="") {
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							ContactMethod contactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							importRow.NewValDisplay=Lan.g("enumContactMethod",contactMethod.ToString());
							importRow.NewValObj=contactMethod;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(ContactMethod);
					if(importRow.NewValObj!=null && (ContactMethod)importRow.NewValObj!=_patient.PreferContactMethod) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//PreferConfirmMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferConfirmMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferConfirmMethod");
				}
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="PreferConfirmMethod";
					importRow.OldValDisplay=Lan.g("enumContactMethod",_patient.PreferConfirmMethod.ToString());
					importRow.OldValObj=_patient.PreferConfirmMethod;
					if(fieldVal=="") {
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							ContactMethod contactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							importRow.NewValDisplay=Lan.g("enumContactMethod",contactMethod.ToString());
							importRow.NewValObj=contactMethod;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(ContactMethod);
					if(importRow.NewValObj!=null && (ContactMethod)importRow.NewValObj!=_patient.PreferConfirmMethod) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//PreferRecallMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferRecallMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferRecallMethod");
				}
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="PreferRecallMethod";
					importRow.OldValDisplay=Lan.g("enumContactMethod",_patient.PreferRecallMethod.ToString());
					importRow.OldValObj=_patient.PreferRecallMethod;
					if(fieldVal=="") {
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							ContactMethod contactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							importRow.NewValDisplay=Lan.g("enumContactMethod",contactMethod.ToString());
							importRow.NewValObj=contactMethod;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(ContactMethod);
					if(importRow.NewValObj!=null && (ContactMethod)importRow.NewValObj!=_patient.PreferRecallMethod) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//referredFrom---------------------------------------------
				fieldVal=GetInputValue("referredFrom");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="referredFrom";
					Referral referral=Referrals.GetReferralForPat(_patient.PatNum);
					if(referral==null) {//there was no existing referral
						importRow.OldValDisplay="";
						importRow.OldValObj=null;
						importRow.NewValDisplay=fieldVal;
						importRow.NewValObj=null;
						if(importRow.NewValDisplay!="") {//user did enter a referral
							importRow.ImpValDisplay=Lan.g(this,"[double click to pick]");
							importRow.ImpValObj=null;
							importRow.IsImportRed=true;
							importRow.DoImport=false;//this will change to true after they pick a referral
						}
						else {//user still did not enter a referral
							importRow.ImpValDisplay="";
							importRow.ImpValObj=null;
							importRow.DoImport=false;
						}
					}
					else {//there was an existing referral. We don't allow changing from here since mostly for new patients.
						importRow.OldValDisplay=referral.GetNameFL();
						importRow.OldValObj=referral;
						importRow.NewValDisplay=fieldVal;
						importRow.NewValObj=null;
						importRow.ImpValDisplay="";
						importRow.ImpValObj=null;
						importRow.DoImport=false;
						if(importRow.OldValDisplay!=importRow.NewValDisplay) {//if patient changed an existing referral, at least let user know.
							importRow.IsNewValRed=true;//although they won't be able to do anything about it here
						}
					}
					importRow.TypeObj=typeof(Referral);
					_listImportRows.Add(importRow);
				}
				//StudentStatus---------------------------------------------
				fieldVal=GetRadioValue("StudentStatus");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="StudentStatus";
					if(_patient.StudentStatus=="N") {
						importRow.OldValDisplay="Nonstudent";
					}
					else if(_patient.StudentStatus=="P") {
						importRow.OldValDisplay="Part-time";
					}
					else if(_patient.StudentStatus=="F") {
						importRow.OldValDisplay="Full-time";
					}
					importRow.OldValObj=_patient.StudentStatus;
					if(fieldVal=="") {
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					//fieldVals for StudentStatus are the full word for regular sheets and the db field for eForms, so check for both formats of fieldVal.
					//Regular sheet example: "Fulltime".  eForm example: "F"
					else if(fieldVal=="Nonstudent" || fieldVal=="N") {
						importRow.NewValDisplay="Nonstudent";
						importRow.NewValObj="N";
					}
					else if(fieldVal=="Parttime" || fieldVal=="P") {
						importRow.NewValDisplay="Part-time";
						importRow.NewValObj="P";
					}
					else if (fieldVal=="Fulltime" || fieldVal=="F") {
						importRow.NewValDisplay="Full-time";
						importRow.NewValObj="F";
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.NewValObj!=null && (string)importRow.NewValObj!=_patient.StudentStatus) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ICE Name---------------------------------------------
				fieldVal=GetInputValue("ICEName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ICEName";
					importRow.OldValDisplay=_patientNote.ICEName;
					importRow.OldValObj=_patientNote.ICEName;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					importRow.IsNewValRed=true;//if user entered nothing, the red text won't show anyway.
					_listImportRows.Add(importRow);
				}
				//ICE Phone---------------------------------------------
				fieldVal=GetInputValue("ICEPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ICEPhone";
					importRow.OldValDisplay=_patientNote.ICEPhone;
					importRow.OldValObj=_patientNote.ICEPhone;
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					importRow.IsNewValRed=true;//if user entered nothing, the red text won't show anyway.
					_listImportRows.Add(importRow);
				}	
				#endregion personal
				//Separator-------------------------------------------
				if(_hasSectionAddrHmPhone){
				_listImportRows.Add(CreateSeparator("Address and Home Phone"));
				}
				#region address
				//SameForEntireFamily-------------------------------------------
				if(ContainsOneOfFields("addressAndHmPhoneIsSameEntireFamily")) {
					importRow=new ImportRow();
					importRow.FieldName="addressAndHmPhoneIsSameEntireFamily";
					importRow.FieldDisplay="Same for entire family";
					if(_isAddressSameForFam) {//remember we calculated this in the form constructor.
						importRow.OldValDisplay="X";
						importRow.OldValObj="X";
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					//And now, we will revise AddressSameForFam based on user input
					_isAddressSameForFam=IsChecked("addressAndHmPhoneIsSameEntireFamily");
					if(_isAddressSameForFam) {
						importRow.NewValDisplay="X";
						importRow.NewValObj="X";
						importRow.ImpValDisplay="X";
						importRow.ImpValObj="X";
					}
					else {
						importRow.NewValDisplay="";
						importRow.NewValObj="";
						importRow.ImpValDisplay="";
						importRow.ImpValObj="";
					}
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Address---------------------------------------------
				fieldVal=GetInputValue("Address");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="Address";
					importRow.OldValDisplay=_patient.Address;
					importRow.OldValObj=_patient.Address;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Address2---------------------------------------------
				fieldVal=GetInputValue("Address2");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="Address2";
					importRow.OldValDisplay=_patient.Address2;
					importRow.OldValObj=_patient.Address2;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//City---------------------------------------------
				fieldVal=GetInputValue("City");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="City";
					importRow.OldValDisplay=_patient.City;
					importRow.OldValObj=_patient.City;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//State---------------------------------------------
				fieldVal=GetInputValue("State");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="State";
					importRow.OldValDisplay=_patient.State;
					importRow.OldValObj=_patient.State;
					importRow.NewValDisplay=fieldVal;
					string pattern="^"//start of string
						+"[a-zA-Z][a-zA-Z]"//exactly two letters
						+"$";//end of string
					if(Regex.IsMatch(fieldVal,pattern)) {
						if(CultureInfo.CurrentCulture.Name.EndsWith("US") || CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
							importRow.NewValDisplay=fieldVal.ToUpper();
						}
					}
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//StateNoValidation---------------------------------
				fieldVal=GetInputValue("StateNoValidation");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="StateNoValidation";
					importRow.OldValDisplay=_patient.State;
					importRow.OldValObj=_patient.State;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//Zip---------------------------------------------
				fieldVal=GetInputValue("Zip");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="Zip";
					importRow.OldValDisplay=_patient.Zip;
					importRow.OldValObj=_patient.Zip;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//HmPhone---------------------------------------------
				fieldVal=GetInputValue("HmPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="HmPhone";
					importRow.OldValDisplay=_patient.HmPhone;
					importRow.OldValObj=_patient.HmPhone;
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				#endregion address
				//Separator-------------------------------------------
				if(_hasSectionIns1){
					_listImportRows.Add(CreateSeparator("Insurance Policy 1"));
				}
				#region ins1
				//It turns out that importing insurance is crazy complicated if we want it to be perfect.
				//So it's better to table that plan for now.
				//The new strategy is simply to show them what the user entered and notify them if it seems different.
				//ins1Relat------------------------------------------------------------
				fieldVal=GetRadioValue("ins1Relat");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("ins1Relat");
				}
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1Relat";
					importRow.FieldDisplay="Relationship";
					importRow.OldValDisplay=Lan.g("enumRelat",_relatIns1.ToString());
					importRow.OldValObj=_relatIns1;
					if(fieldVal=="") {
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							Relat relat=(Relat)Enum.Parse(typeof(Relat),fieldVal);
							importRow.NewValDisplay=Lan.g("enumRelat",relat.ToString());
							importRow.NewValObj=relat;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid Relationship."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(Relat);
					importRow.DoImport=false;
					if(importRow.NewValObj!=null && importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins1Subscriber---------------------------------------------
				fieldVal=GetInputValue("ins1SubscriberNameF");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1Subscriber";
					importRow.FieldDisplay="Subscriber";
					if(_insPlan1!=null) {
						importRow.OldValDisplay=_family.GetNameInFamFirst(_insSub1.Subscriber);
						importRow.OldValObj=_insSub1.Subscriber;
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj=null;
					}
					importRow.NewValDisplay=fieldVal;//whether it's empty or has a value
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay="[double click to pick]";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Patient);
					importRow.DoImport=false;
					importRow.IsImportRed=true;
					_listImportRows.Add(importRow);
				}
				//ins1SubscriberID---------------------------------------------
				fieldVal=GetInputValue("ins1SubscriberID");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1SubscriberID";
					importRow.FieldDisplay="Subscriber ID";
					if(_insPlan1!=null) {
						importRow.OldValDisplay=_insSub1.SubscriberID;
						importRow.OldValObj="";
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					importRow.NewValDisplay=fieldVal;//whether it's empty or has a value
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins1CarrierName---------------------------------------------
				fieldVal=GetInputValue("ins1CarrierName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1CarrierName";
					importRow.FieldDisplay="Carrier";
					if(_carrier1!=null) {
						importRow.OldValDisplay=_carrier1.CarrierName;
						importRow.OldValObj=_carrier1;
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					importRow.NewValDisplay=fieldVal;//whether it's empty or has a value
					importRow.NewValObj="";
					importRow.ImpValDisplay="[double click to pick]";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Carrier);
					importRow.DoImport=false;
					importRow.IsImportRed=true;
					_listImportRows.Add(importRow);
				}
				//ins1CarrierPhone---------------------------------------------
				fieldVal=GetInputValue("ins1CarrierPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1CarrierPhone";
					importRow.FieldDisplay="Phone";
					if(_carrier1!=null) {
						importRow.OldValDisplay=_carrier1.Phone;
						importRow.OldValObj="";
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);//whether it's empty or has a value
					importRow.NewValObj="";
					importRow.ImpValDisplay="[double click to pick]";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Carrier);
					importRow.DoImport=false;
					importRow.IsImportRed=true;
					_listImportRows.Add(importRow);
				}
				//ins1EmployerName---------------------------------------------
				fieldVal=GetInputValue("ins1EmployerName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1EmployerName";
					importRow.FieldDisplay="Employer";
					if(_insPlan1==null) {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					else {
						importRow.OldValDisplay=Employers.GetName(_insPlan1.EmployerNum);
						importRow.OldValObj=Employers.GetEmployer(_insPlan1.EmployerNum);
					}
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins1GroupName---------------------------------------------
				fieldVal=GetInputValue("ins1GroupName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1GroupName";
					importRow.FieldDisplay="Group Name";
					if(_insPlan1!=null) {
						importRow.OldValDisplay=_insPlan1.GroupName;
					}
					else {
						importRow.OldValDisplay="";
					}
					importRow.OldValObj="";
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins1GroupNum---------------------------------------------
				fieldVal=GetInputValue("ins1GroupNum");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins1GroupNum";
					importRow.FieldDisplay="Group Num";
					if(_insPlan1!=null) {
						importRow.OldValDisplay=_insPlan1.GroupNum;
					}
					else {
						importRow.OldValDisplay="";
					}
					importRow.OldValObj="";
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				#endregion ins1
				//Separator-------------------------------------------
				if(_hasSectionIns2){
					_listImportRows.Add(CreateSeparator("Insurance Policy 2"));
				}
				#region ins2
				//It turns out that importing insurance is crazy complicated if want it to be perfect.
				//So it's better to table that plan for now.
				//The new strategy is simply to show them what the user entered and notify them if it seems different.
				//ins2Relat------------------------------------------------------------
				fieldVal=GetRadioValue("ins2Relat");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("ins2Relat");
				}
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2Relat";
					importRow.FieldDisplay="Relationship";
					importRow.OldValDisplay=Lan.g("enumRelat",_relatIns2.ToString());
					importRow.OldValObj=_relatIns2;
					if(fieldVal=="") {
						importRow.NewValDisplay="";
						importRow.NewValObj=null;
					}
					else {
						try {
							Relat relat=(Relat)Enum.Parse(typeof(Relat),fieldVal);
							importRow.NewValDisplay=Lan.g("enumRelat",relat.ToString());
							importRow.NewValObj=relat;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid Relationship."));
						}
					}
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(Relat);
					importRow.DoImport=false;
					if(importRow.NewValObj!=null && importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins2Subscriber---------------------------------------------
				fieldVal=GetInputValue("ins2SubscriberNameF");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2Subscriber";
					importRow.FieldDisplay="Subscriber";
					if(_insPlan2!=null) {
						importRow.OldValDisplay=_family.GetNameInFamFirst(_insSub2.Subscriber);
						importRow.OldValObj=_insSub2.Subscriber;
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj=null;
					}
					importRow.NewValDisplay=fieldVal;//whether it's empty or has a value
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay="[double click to pick]";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Patient);
					importRow.DoImport=false;
					importRow.IsImportRed=true;
					_listImportRows.Add(importRow);
				}
				//ins2SubscriberID---------------------------------------------
				fieldVal=GetInputValue("ins2SubscriberID");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2SubscriberID";
					importRow.FieldDisplay="Subscriber ID";
					if(_insPlan2!=null) {
						importRow.OldValDisplay=_insSub2.SubscriberID;
						importRow.OldValObj="";
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					importRow.NewValDisplay=fieldVal;//whether it's empty or has a value
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins2CarrierName---------------------------------------------
				fieldVal=GetInputValue("ins2CarrierName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2CarrierName";
					importRow.FieldDisplay="Carrier";
					if(_carrier2!=null) {
						importRow.OldValDisplay=_carrier2.CarrierName;
						importRow.OldValObj="";
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					importRow.NewValDisplay=fieldVal;//whether it's empty or has a value
					importRow.NewValObj="";
					importRow.ImpValDisplay="[double click to pick]";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Carrier);
					importRow.DoImport=false;
					importRow.IsImportRed=true;
					_listImportRows.Add(importRow);
				}
				//ins2CarrierPhone---------------------------------------------
				fieldVal=GetInputValue("ins2CarrierPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2CarrierPhone";
					importRow.FieldDisplay="Phone";
					if(_carrier2!=null) {
						importRow.OldValDisplay=_carrier2.Phone;
						importRow.OldValObj="";
					}
					else {
						importRow.OldValDisplay="";
						importRow.OldValObj="";
					}
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);//whether it's empty or has a value
					importRow.NewValObj="";
					importRow.ImpValDisplay="[double click to pick]";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Carrier);
					importRow.DoImport=false;
					importRow.IsImportRed=true;
					_listImportRows.Add(importRow);
				}
				//ins2EmployerName---------------------------------------------
				fieldVal=GetInputValue("ins2EmployerName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2EmployerName";
					importRow.FieldDisplay="Employer";
					if(_insPlan2==null) {
						importRow.OldValDisplay="";
					}
					else {
						importRow.OldValDisplay=Employers.GetName(_insPlan2.EmployerNum);
					}
					importRow.OldValObj="";
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins2GroupName---------------------------------------------
				fieldVal=GetInputValue("ins2GroupName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2GroupName";
					importRow.FieldDisplay="Group Name";
					if(_insPlan2!=null) {
						importRow.OldValDisplay=_insPlan2.GroupName;
					}
					else {
						importRow.OldValDisplay="";
					}
					importRow.OldValObj="";
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				//ins2GroupNum---------------------------------------------
				fieldVal=GetInputValue("ins2GroupNum");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ins2GroupNum";
					importRow.FieldDisplay="Group Num";
					if(_insPlan2!=null) {
						importRow.OldValDisplay=_insPlan2.GroupNum;
					}
					else {
						importRow.OldValDisplay="";
					}
					importRow.OldValObj="";
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=fieldVal;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					importRow.DoImport=false;
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				#endregion ins2
			}
			#endregion Patient Form
			#region Medical History (Sheets)
			if(SheetCur!=null && SheetCur.SheetType==SheetTypeEnum.MedicalHistory){
				List<Disease> listDiseases=null;
				//_listImportRows.Add(CreateSeparator("Personal"));//already done
				#region ICE
				//ICE Name---------------------------------------------
				fieldVal=GetInputValue("ICEName");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ICEName";
					importRow.OldValDisplay=_patientNote.ICEName;
					importRow.OldValObj=_patientNote.ICEName;
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					importRow.IsNewValRed=true;//if user entered nothing, the red text won't show anyway.
					_listImportRows.Add(importRow);
				}
				//ICE Phone---------------------------------------------
				fieldVal=GetInputValue("ICEPhone");
				if(fieldVal!=null) {
					importRow=new ImportRow();
					importRow.FieldName="ICEPhone";
					importRow.OldValDisplay=_patientNote.ICEPhone;
					importRow.OldValObj=_patientNote.ICEPhone;
					importRow.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					importRow.NewValObj=importRow.NewValDisplay;
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=importRow.NewValObj;
					importRow.TypeObj=typeof(string);
					if(importRow.OldValDisplay!=importRow.NewValDisplay) {
						importRow.DoImport=true;
					}
					importRow.IsNewValRed=true;//if user entered nothing, the red text won't show anyway.
					_listImportRows.Add(importRow);
				}
				#endregion
				//Separator-------------------------------------------
				if(_hasSectionAllergies){
					_listImportRows.Add(CreateSeparator("Allergies"));
				}
				#region Allergies (Sheets)
				if(SheetCur!=null){
					List<Allergy> listAllergiesPat=Allergies.GetAll(_patient.PatNum,showInactive:true);
					//Get list of all the allergy check boxes
					List<SheetField> listSheetFieldsAllergy=new List<SheetField>();
					listSheetFieldsAllergy=GetSheetFieldsStartWith("allergy:");
					for(int i=0;i<listSheetFieldsAllergy.Count;i++) {
						fieldVal="";
						importRow=new ImportRow();
						importRow.FieldName=listSheetFieldsAllergy[i].FieldName.Remove(0,8);
						importRow.OldValDisplay="";
						importRow.OldValObj=null;
						//Check if allergy exists.
						for(int j=0;j<listAllergiesPat.Count;j++) {
							if(AllergyDefs.GetDescription(listAllergiesPat[j].AllergyDefNum)==listSheetFieldsAllergy[i].FieldName.Remove(0,8)) {
								if(listAllergiesPat[j].StatusIsActive) {
									importRow.OldValDisplay="Y";
								}
								else {
									importRow.OldValDisplay="N";
								}
								importRow.OldValObj=listAllergiesPat[j];
								break;
							}
						}
						SheetField sheetFieldOppositeBox=GetOppositeSheetFieldCheckBox(listSheetFieldsAllergy,listSheetFieldsAllergy[i]);
						if(listSheetFieldsAllergy[i].FieldValue=="") {//Current box not checked.
							if(sheetFieldOppositeBox==null || sheetFieldOppositeBox.FieldValue=="") {//No opposite box or both boxes are not checked.
								//Create a blank row just in case they want to import.
								importRow.NewValDisplay="";
								importRow.NewValObj=listSheetFieldsAllergy[i];
								importRow.ImpValDisplay="";
								importRow.ImpValObj="";
								importRow.TypeObj=typeof(Allergy);
								_listImportRows.Add(importRow);
								if(sheetFieldOppositeBox!=null) {
									listSheetFieldsAllergy.Remove(sheetFieldOppositeBox);//Removes possible duplicate entry.
								}
								continue;
							}
							//Opposite box is checked, figure out if it's a Y or N box.
							if(sheetFieldOppositeBox.RadioButtonValue=="Y") {
								fieldVal="Y";
							}
							else {
								fieldVal="N";
							}
						}
						else {//Current box is checked.
							if(listSheetFieldsAllergy[i].RadioButtonValue=="Y") {
								fieldVal="Y";
							}
							else {
								fieldVal="N";
							}
						}
						//Get rid of the opposite check box so field doesn't show up twice.
						if(sheetFieldOppositeBox!=null) {
							listSheetFieldsAllergy.Remove(sheetFieldOppositeBox);
						}
						importRow.NewValDisplay=fieldVal;
						importRow.NewValObj=listSheetFieldsAllergy[i];
						importRow.ImpValDisplay=importRow.NewValDisplay;
						importRow.ImpValObj=typeof(string);
						importRow.TypeObj=typeof(Allergy);
						if(importRow.OldValDisplay!=importRow.NewValDisplay && !(importRow.OldValDisplay=="" && importRow.NewValDisplay=="N")) {
							importRow.DoImport=true;
						}
						_listImportRows.Add(importRow);
					}
				}
				#endregion Allergies (Sheets)
				//Separator-------------------------------------------
				if(_hasSectionMeds){
					_listImportRows.Add(CreateSeparator("Medications"));
				}
				#region Medications (Sheets)
				List<SheetField> listSheetFieldsCurrentMed=new List<SheetField>();
				List<SheetField> listSheetFieldsNewMed=new List<SheetField>();
				List<SheetField> listSheetFieldsInputMed=GetSheetFieldsStartWith("inputMed");
				List<SheetField> listSheetFieldsCheckMed=GetSheetFieldsStartWith("checkMed");
				for(int i=0;i<listSheetFieldsInputMed.Count;i++) {
					if(listSheetFieldsInputMed[i].FieldType==SheetFieldType.OutputText) {
						listSheetFieldsCurrentMed.Add(listSheetFieldsInputMed[i]);
					}
					else {//User might have tried to type in a new medication they are taking.
						listSheetFieldsNewMed.Add(listSheetFieldsInputMed[i]);
					}
				}
				List<MedicationPat> listMedicationPatsFull=MedicationPats.Refresh(_patient.PatNum,false);
				for(int i=0;i<listSheetFieldsCurrentMed.Count;i++) {
					#region existing medications
					fieldVal="";
					importRow=new ImportRow();
					importRow.FieldName=listSheetFieldsCurrentMed[i].FieldValue;//Will be the name of the drug.
					importRow.OldValDisplay="N";
					importRow.OldValObj=null;
					for(int j=0;j<listMedicationPatsFull.Count;j++) {
						string strMedName=listMedicationPatsFull[j].MedDescript;//for meds that came back from NewCrop
						if(listMedicationPatsFull[j].MedicationNum!=0) {//For meds entered in OD and linked to Medication list.
							strMedName=Medications.GetDescription(listMedicationPatsFull[j].MedicationNum);
						}
						if(listSheetFieldsCurrentMed[i].FieldValue==strMedName) {
							importRow.OldValDisplay="Y";
							importRow.OldValObj=listMedicationPatsFull[j];
						}
					}
					List<SheetField> listSheetFieldsRelatedChkBoxes=GetRelatedMedicalCheckBoxes(listSheetFieldsCheckMed,listSheetFieldsCurrentMed[i]);
					for(int j=0;j<listSheetFieldsRelatedChkBoxes.Count;j++) {//Figure out which corresponding checkbox is checked.
						if(listSheetFieldsRelatedChkBoxes[j].FieldValue!="") {//Patient checked this box.
							if(listSheetFieldsCheckMed[j].RadioButtonValue=="Y") {
								fieldVal="Y";
							}
							else {
								fieldVal="N";
							}
							break;
						}
						//If sheet is only using N boxes and the patient already had this med marked as inactive and then they unchecked the N, so now we need to import it.
						if(listSheetFieldsRelatedChkBoxes.Count==1 && listSheetFieldsRelatedChkBoxes[j].RadioButtonValue=="N" //Only using N boxes for this current medication.
							&& importRow.OldValObj!=null && importRow.OldValDisplay=="N" //Patient has this medication but is currently marked as inactive.
							&& listSheetFieldsRelatedChkBoxes[j].FieldValue=="") //Patient unchecked the medication so we activate it again.
						{
							fieldVal="Y";
						}
					}
					if(listSheetFieldsRelatedChkBoxes.Count==1 
						&& listSheetFieldsRelatedChkBoxes[0].RadioButtonValue=="N" 
						&& listSheetFieldsRelatedChkBoxes[0].FieldValue=="" 
						&& importRow.OldValDisplay=="N" 
						&& importRow.OldValObj!=null)
					{
						importRow.DoImport=true;
					}
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=listSheetFieldsCurrentMed[i];
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=typeof(string);
					importRow.TypeObj=typeof(MedicationPat);
					if(importRow.OldValDisplay!=importRow.NewValDisplay && importRow.NewValDisplay!="") {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
					#endregion
				}
				for(int i=0;i<listSheetFieldsNewMed.Count;i++) {
					#region medications the patient entered
					if(listSheetFieldsNewMed[i].FieldValue=="") {//No medication entered by patient.
						continue;
					}
					importRow=new ImportRow();
					importRow.FieldName=listSheetFieldsNewMed[i].FieldValue;//Whatever the patient typed in...
					importRow.OldValDisplay="";
					importRow.OldValObj=null;
					importRow.NewValDisplay="Y";
					importRow.NewValObj=listSheetFieldsNewMed[i];
					importRow.ImpValDisplay=Lan.g(this,"[double click to pick]");
					importRow.ImpValObj=new long();
					importRow.IsImportRed=true;
					importRow.DoImport=false;//this will change to true after they pick a medication
					importRow.TypeObj=typeof(MedicationPat);
					_listImportRows.Add(importRow);
					#endregion
				}
				#endregion Medications (Sheets)
				//Separator-------------------------------------------
				if(_hasSectionProblems){
					_listImportRows.Add(CreateSeparator("Problems"));
				}
				#region Problems (Sheets only)
				List<SheetField> listSheetFieldsProblems=GetSheetFieldsStartWith("problem:");
				for(int i=0;i<listSheetFieldsProblems.Count;i++) {
					fieldVal="";
					if(i<1) {
						listDiseases=Diseases.Refresh(_patient.PatNum,false);
					}
					importRow=new ImportRow();
					importRow.FieldName=listSheetFieldsProblems[i].FieldName.Remove(0,8);
					//Figure out the current status of this allergy
					importRow.OldValDisplay="";
					importRow.OldValObj=null;
					for(int j=0;j<listDiseases.Count;j++) {
						if(DiseaseDefs.GetName(listDiseases[j].DiseaseDefNum)==listSheetFieldsProblems[i].FieldName.Remove(0,8)) {
							if(listDiseases[j].ProbStatus==ProblemStatus.Active) {
								importRow.OldValDisplay="Y";
							}
							else {
								importRow.OldValDisplay="N";
							}
							importRow.OldValObj=listDiseases[j];
							break;
						}
					}
					SheetField sheetFieldOppositeBox=GetOppositeSheetFieldCheckBox(listSheetFieldsProblems,listSheetFieldsProblems[i]);
					if(listSheetFieldsProblems[i].FieldValue=="") {//Current box not checked.
						if(sheetFieldOppositeBox==null || sheetFieldOppositeBox.FieldValue=="") {//No opposite box or both boxes are not checked.
							//Create a blank row just in case they still want to import.
							importRow.NewValDisplay="";
							importRow.NewValObj=listSheetFieldsProblems[i];
							importRow.ImpValDisplay="";
							importRow.ImpValObj="";
							importRow.TypeObj=typeof(Disease);
							_listImportRows.Add(importRow);
							if(sheetFieldOppositeBox!=null) {
								listSheetFieldsProblems.Remove(sheetFieldOppositeBox);//Removes possible duplicate entry.
							}
							continue;
						}
						//Opposite box is checked, figure out if it's a Y or N box.
						if(sheetFieldOppositeBox.RadioButtonValue=="Y") {
							fieldVal="Y";
						}
						else {
							fieldVal="N";
						}
					}
					else {//Current box is checked.  
						if(listSheetFieldsProblems[i].RadioButtonValue=="Y") {
							fieldVal="Y";
						}
						else {
							fieldVal="N";
						}
					}
					//Get rid of the opposite check box so field doesn't show up twice.
					if(sheetFieldOppositeBox!=null) {
						listSheetFieldsProblems.Remove(sheetFieldOppositeBox);
					}
					importRow.NewValDisplay=fieldVal;
					importRow.NewValObj=listSheetFieldsProblems[i];
					importRow.ImpValDisplay=importRow.NewValDisplay;
					importRow.ImpValObj=typeof(string);
					importRow.TypeObj=typeof(Disease);
					if(importRow.OldValDisplay!=importRow.NewValDisplay && !(importRow.OldValDisplay=="" && importRow.NewValDisplay=="N")) {
						importRow.DoImport=true;
					}
					_listImportRows.Add(importRow);
				}
				#endregion Problems (Sheets only)
			}
			#endregion Medical History (Sheets)
			#region Allergies (eForms)
			if(EFormCur!=null && _hasSectionAllergies){
				_listImportRows.Add(CreateSeparator("Allergies"));
				List<AllergyDef> listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);//from db. AllergyDefs are oddly not cached.
				List<Allergy> listAllergiesPat=Allergies.GetAll(_patient.PatNum,showInactive:false);//just active allergies
				List<string> listStringsPat=new List<string>();//this is for later
				//First, we add a list of all existing allergies
				for(int i=0;i<listAllergiesPat.Count;i++){
					AllergyDef allergyDef=listAllergyDefs.Find(x=>x.AllergyDefNum==listAllergiesPat[i].AllergyDefNum);
					string descript=allergyDef.Description;
					if(allergyDef.Description=="Other"){
						descript+=" - "+listAllergiesPat[i].Reaction;
					}
					else{
						listStringsPat.Add(allergyDef.Description);
					}
					//no need to check allergyDef null for existing allergies
					importRow=new ImportRow();
					importRow.FieldName="";//There is no fieldname that makes sense. It's a combination of multiple.
					importRow.OldValDisplay=descript;
					importRow.OldValObj=listAllergiesPat[i];
					importRow.NewValDisplay="";
					importRow.NewValObj=null;
					importRow.ImpValDisplay="";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Allergy);
					_listImportRows.Add(importRow);
				}
				bool? isCheckedAllergiesNone=IsCheckedEForm("allergiesNone");
				if(isCheckedAllergiesNone!=null) {
					//note that this is mutually exclusive with allergiesOther and allergy:...
					//If this gets hit, none of those will,
					//and if any of those get hit, this will not.
					if(isCheckedAllergiesNone==true) {
						for(int a=0;a<listAllergiesPat.Count;a++) {
							ImportRow importRowAllerg=_listImportRows.Find(x=>x.OldValObj==listAllergiesPat[a]);
							if(importRowAllerg==null){
								continue;//should never happen
							}
							importRowAllerg.DoRemove=true;//this is our trigger to remove the allergy
							importRowAllerg.NewValDisplay="None";
							importRowAllerg.ImpValDisplay="(mark inactive)";
							importRowAllerg.DoImport=true;
						}
					}
				}
				fieldVal=GetInputValue("allergiesOther");
				if(fieldVal!=null) {
					//if allergiesNone was checked, this box will be empty, so loop list will also be empty.
					List<string> listStrsInput=fieldVal.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToList();
					//get list of allergies already covered by checkboxes or radiobuttons
					List<string> listStrAllergiesChecks=EFormCur.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("allergy:")).Select(x=>x.DbLink.Substring(8)).ToList();
					//First loop is to consider which ones to add based on user input
					for(int a=0;a<listStrsInput.Count;a++){
						if(listStringsPat.Contains(listStrsInput[a])){
							continue;
							//The patient already has this as an active allergy, so there's no action for us to take.
							//This is common because of prefill.
						}
						if(listStrAllergiesChecks.Contains(listStrsInput[a])) {
							//If there is a checkbox or radiobutton for an allergy, but user also types it in here,
							//then we have a slight problem. We have to pick one or the other, and they might not match.
							//We have decided in that case to depend on the checkbox/radiobutton and ignore whatever happens in this box.
							continue;
						}
						AllergyDef allergyDef=listAllergyDefs.Find(x=>x.Description.ToLower()==listStrsInput[a].ToLower());
						if(allergyDef==null){
							//this allergy that they typed in does not have any matching allergyDef, so we will use "Other".
							//So we have to check if it exists as an "Other" allergy
							AllergyDef allergyDefOther=listAllergyDefs.Find(x=>x.Description=="Other");
							if(allergyDefOther!=null){
								if(listAllergiesPat.Exists(x=>x.AllergyDefNum==allergyDefOther.AllergyDefNum && x.Reaction==listStrsInput[a])){
									//this is common because of prefill
									continue;
								}
							}
						}
						//need to add one
						importRow=new ImportRow();
						importRow.FieldName="";//There is no fieldname that makes sense. It's a combination of multiple.
						importRow.OldValDisplay="";
						importRow.OldValObj=null;
						importRow.NewValDisplay=listStrsInput[a];//this is how we will know what to add
						importRow.NewValObj=null;//no object until we later create one
						importRow.ImpValDisplay="(add)";
						importRow.ImpValObj=null;//no object until we later create one
						importRow.TypeObj=typeof(Allergy);
						importRow.DoImport=true;
						importRow.DoAdd=true;//although this will be ignored if not importing.
						_listImportRows.Add(importRow);
					}//for
					//Second loop here could consider which ones to remove, but this doesn't seem very useful.
					//It would make more sense for a problem or certainly for a med.
					//But nobody stops having an allergy, so ignore this for now.
				}//if allergiesOther
				List<EFormField> listEFormFieldsAllergyCheck=EFormCur.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("allergy:"));
				//Allergy checkboxes and radiobuttons:
				for(int i=0;i<listEFormFieldsAllergyCheck.Count;i++){
					string strAllergyCheck=listEFormFieldsAllergyCheck[i].DbLink.Substring(8);
					AllergyDef allergyDef=listAllergyDefs.Find(x=>x.Description.ToLower()==strAllergyCheck.ToLower());
					if(allergyDef==null){
						//This shouldn't normally happen, but it certainly is possible because user could change an allergyDef name after adding the checkbox or radiobutton,
						//or this could be an imported form with no allergyDef in the db representing the value in this checkbox/radiobut.
						//We will just add this for them. That way, all imported forms will correctly work.
						//We are doing it here instead of when clicking Import because it's extremely rare
						//and it's more of a setup issue. Also, it would be more complex to pass the info to that section.
						allergyDef=new AllergyDef();
						allergyDef.Description=strAllergyCheck;
						AllergyDefs.Insert(allergyDef);
						listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);
						//DataValid.SetInvalid would be used instead if this was cached.
					}
					//allergyDef is now guaranteed to have a value
					Allergy allergyPat=listAllergiesPat.Find(x=>x.AllergyDefNum==allergyDef.AllergyDefNum);
					//We don't also need to check list of pending imports because allergiesOther defers to this section.
					if(allergyPat==null){
						//This patient does not have an active allergy matching the allergy for this checkbox.
						//Notice that we do not look at inactive allergies and consider flipping them back to active.
						//Once someone has an allergy, it doesn't go away. We don't need to consider that fancy edge case.
						//The worst consequence is that they might end up with one active and one inactive. No big deal.
						if(listEFormFieldsAllergyCheck[i].ValueString=="X"//checkbox
							|| listEFormFieldsAllergyCheck[i].ValueString=="Y")//radiobutton
						{
							//add
							importRow=new ImportRow();
							importRow.FieldName="";//There is no fieldname that makes sense. It's a combination of multiple.
							importRow.OldValDisplay="";
							importRow.OldValObj=null;
							importRow.NewValDisplay=strAllergyCheck;//this is how we will know what to add
							importRow.NewValObj=null;//no object until we later create one
							importRow.ImpValDisplay="(add)";
							importRow.ImpValObj=null;//no object until we later create one
							importRow.TypeObj=typeof(Allergy);
							importRow.DoImport=true;
							importRow.DoAdd=true;//although this will be ignored if not importing.
							_listImportRows.Add(importRow);
						}
						else{
							//db is already accurate
						}
					}
					else{
						//This patient does have an active allergy matching the allergy for this checkbox.
						if((listEFormFieldsAllergyCheck[i].FieldType==EnumEFormFieldType.CheckBox
							&& listEFormFieldsAllergyCheck[i].ValueString=="")
							|| listEFormFieldsAllergyCheck[i].ValueString=="N")//radiobutton (empty rb indicates no change)
						{
							//remove
							ImportRow importRowAllerg=_listImportRows.Find(x=>
								x.TypeObj==typeof(Allergy)
								&& x.OldValObj!=null
								&& ((Allergy)x.OldValObj).AllergyDefNum==allergyDef.AllergyDefNum);
							if(importRowAllerg==null){
								continue;//should never happen
							}
							importRowAllerg.DoRemove=true;//this is our trigger to remove the allergy
							importRowAllerg.NewValDisplay="(unchecked)";
							importRowAllerg.ImpValDisplay="(mark inactive)";
							importRowAllerg.DoImport=true;
						}
						else{
							//db is already accurate
						}
					}
				}//for i allergy checkboxes and radiobuttons
			}
			#endregion Allergies (eForms)
			#region Medications (eForms)
			if(EFormCur!=null && _hasSectionMeds){
				_listImportRows.Add(CreateSeparator("Medications"));
				EFormField eFormFieldMedList=EFormCur.ListEFormFields.Find(x=>x.FieldType==EnumEFormFieldType.MedicationList);
				//No need to check for null because we already did that when setting _hasSectionMeds
				EFormMedListLayout eFormMedListLayout=JsonConvert.DeserializeObject<EFormMedListLayout>(eFormFieldMedList.ValueLabel);
				List<MedicationPat> listMedicationPats=MedicationPats.Refresh(_patient.PatNum,includeDiscontinued:false);
				//First, we add a list of all existing meds
				for(int i=0;i<listMedicationPats.Count;i++){
					Medication medication=Medications.GetMedication(listMedicationPats[i].MedicationNum);
					string strMed=listMedicationPats[i].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
					if(medication!=null){
						strMed=medication.MedName;
					}
					if(listMedicationPats[i].PatNote!=""){
						strMed+=" ("+listMedicationPats[i].PatNote+")";
					}
					importRow=new ImportRow();
					importRow.FieldName="";//not going to show anything here
					importRow.OldValDisplay=strMed;
					importRow.OldValObj=listMedicationPats[i];
					importRow.NewValDisplay="";
					importRow.NewValObj=null;
					importRow.ImpValDisplay="";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(MedicationPat);
					_listImportRows.Add(importRow);
				}
				List<EFormMed> listEFormMeds=new List<EFormMed>();
				if(!String.IsNullOrEmpty(eFormFieldMedList.ValueString)){
					listEFormMeds=JsonConvert.DeserializeObject<List<EFormMed>>(eFormFieldMedList.ValueString);
				}
				//add any new ones
				//If the med is already present, import the freq & quant.
				for(int m=0;m<listEFormMeds.Count;m++){
					string strMedNewVal=listEFormMeds[m].MedName;
					if(listEFormMeds[m].StrengthFreq!=""){
						strMedNewVal+=" ("+listEFormMeds[m].StrengthFreq+")";
					}
					MedicationPat medicationPat=null;
					for(int a=0;a<listMedicationPats.Count;a++){
						Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
						string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
						if(medication!=null){
							strMed=medication.MedName;
						}
						if(strMed==listEFormMeds[m].MedName){
							medicationPat=listMedicationPats[a];
							break;
						}
					}
					if(medicationPat!=null){
						//Now we know that this med is already present in the list.
						ImportRow importRowMed=_listImportRows.Find(x=>x.OldValObj==medicationPat);
						if(importRowMed==null){
							continue;//should never happen
						}
						//Show user that the same med was entered in the eForm or was at least echoed back.
						//Show it in same format as OldValDisplay.
						importRowMed.NewValDisplay=strMedNewVal;
						if(listEFormMeds[m].StrengthFreq==""){
							continue;
						}
						//Might need to import the strength and freq
						if(eFormMedListLayout.ImportCol2AppendDate){
							importRowMed.MedStrengthFreq=medicationPat.PatNote;
							if(medicationPat.PatNote!=""){
								importRowMed.MedStrengthFreq+="\r\n";
							}
							importRowMed.MedStrengthFreq+=DateTime.Today.ToShortDateString()+"-"+listEFormMeds[m].StrengthFreq;
							importRowMed.MedDoImportCol2=true;
							importRowMed.ImpValDisplay="(append)";
						}
						if(eFormMedListLayout.ImportCol2OverwriteDate){
							importRowMed.MedStrengthFreq=DateTime.Today.ToShortDateString()+"-"+listEFormMeds[m].StrengthFreq;
							importRowMed.MedDoImportCol2=true;
							importRowMed.ImpValDisplay="(overwrite)";
						}
						if(eFormMedListLayout.ImportCol2Append){
							importRowMed.MedStrengthFreq=medicationPat.PatNote;
							if(medicationPat.PatNote!=""){
								importRowMed.MedStrengthFreq+="\r\n";
							}
							importRowMed.MedStrengthFreq+=listEFormMeds[m].StrengthFreq;
							importRowMed.MedDoImportCol2=true;
							importRowMed.ImpValDisplay="(append)";
						}
						if(eFormMedListLayout.ImportCol2Overwrite){
							importRowMed.MedStrengthFreq=listEFormMeds[m].StrengthFreq;
							importRowMed.MedDoImportCol2=true;
							importRowMed.ImpValDisplay="(overwrite)";
						}
						if(importRowMed.MedDoImportCol2){
							importRowMed.DoImport=true;
							//	importRowMed.MedStrengthFreq=strengthFreq;
							//	MedicationPats.Update(medicationPat);
							//	listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
						}
						if(!eFormMedListLayout.ImportCol1){
							importRowMed.DoImport=false;
							importRowMed.MedDoImportCol2=false;
							importRowMed.ImpValDisplay="";
						}
						continue;
					}
					//missing in pat, so add it
					importRow=new ImportRow();
					importRow.FieldName="";//Don't show anything here
					importRow.OldValDisplay="";
					importRow.OldValObj=null;
					importRow.MedName=listEFormMeds[m].MedName;//this is how we will know what to add
					importRow.NewValDisplay=strMedNewVal;
					importRow.NewValObj=null;//no object until we later create one
					importRow.ImpValDisplay="(add)";
					importRow.ImpValObj=null;//no object until we later create one
					importRow.TypeObj=typeof(MedicationPat);
					if(eFormMedListLayout.ImportCol1){
						importRow.DoImport=true;
						importRow.DoAdd=true;//although this will be ignored if not importing.
						if(eFormMedListLayout.ImportCol2AppendDate
							|| eFormMedListLayout.ImportCol2Append
							|| eFormMedListLayout.ImportCol2Overwrite
							|| eFormMedListLayout.ImportCol2OverwriteDate)
						{
							importRow.MedStrengthFreq=listEFormMeds[m].StrengthFreq;
							importRow.MedDoImportCol2=true;
						}
					}
					_listImportRows.Add(importRow);
				}
				//remove 
				for(int a=0;a<listMedicationPats.Count;a++){
					Medication medication=Medications.GetMedication(listMedicationPats[a].MedicationNum);
					string strMed=listMedicationPats[a].MedDescript;//used if this medicationPat came from an eRx or eForm import and medication is null
					if(medication!=null){
						strMed=medication.MedName;
					}
					EFormMed eFormMed=listEFormMeds.Find(x=>x.MedName==strMed);
					if(eFormMed!=null){//in the import list
						continue;
					}
					//this patient med is not in the import list, so it needs to be removed.
					ImportRow importRowMed=_listImportRows.Find(x=>
						x.TypeObj==typeof(MedicationPat)
						&& x.OldValObj!=null
						&& ((MedicationPat)x.OldValObj).MedicationNum==listMedicationPats[a].MedicationNum);
					if(importRowMed==null){
						continue;//should never happen
					}
					importRowMed.DoRemove=true;//this is our trigger to remove the med
					importRowMed.NewValDisplay="(removed)";
					importRowMed.ImpValDisplay="(mark inactive)";
					importRowMed.DoImport=true;
				}
			}
			#endregion Medications (eForms)
			#region Problems (eForms)
			if(EFormCur!=null && _hasSectionProblems){
				_listImportRows.Add(CreateSeparator("Problems"));
				List<Disease> listDiseasesPat=Diseases.Refresh(_patient.PatNum,showActiveOnly:true);
				List<string> listStringsPat=new List<string>();//this is for later
				//First, we add a list of all existing diseases
				for(int i=0;i<listDiseasesPat.Count;i++){
					DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseDefNum==listDiseasesPat[i].DiseaseDefNum);
					string descript=diseaseDef.DiseaseName;
					if(diseaseDef.DiseaseName=="Other"){
						descript+=" - "+listDiseasesPat[i].PatNote;
					}
					else{
						listStringsPat.Add(diseaseDef.DiseaseName);
					}
					//no need to check diseaseDef null for existing diseases
					importRow=new ImportRow();
					importRow.FieldName="";//There is no fieldname that makes sense. It's a combination of multiple.
					importRow.OldValDisplay=descript;
					importRow.OldValObj=listDiseasesPat[i];
					importRow.NewValDisplay="";
					importRow.NewValObj=null;
					importRow.ImpValDisplay="";
					importRow.ImpValObj=null;
					importRow.TypeObj=typeof(Disease);
					_listImportRows.Add(importRow);
				}
				bool? isCheckedProblemsNone=IsCheckedEForm("problemsNone");
				if(isCheckedProblemsNone!=null) {
					//note that this is mutually exclusive with diseasesOther and disease:...
					//If this gets hit, none of those will,
					//and if any of those get hit, this will not.
					if(isCheckedProblemsNone==true) {
						for(int a=0;a<listDiseasesPat.Count;a++) {
							ImportRow importRowDisease=_listImportRows.Find(x=>x.OldValObj==listDiseasesPat[a]);
							if(importRowDisease==null){
								continue;//should never happen
							}
							importRowDisease.DoRemove=true;//this is our trigger to remove the disease
							importRowDisease.NewValDisplay="None";
							importRowDisease.ImpValDisplay="(mark inactive)";
							importRowDisease.DoImport=true;
						}
					}
				}
				fieldVal=GetInputValue("problemsOther");
				if(fieldVal!=null) {
					//if problemsNone was checked, this box will be empty, so loop list will also be empty.
					List<string> listStrsInput=fieldVal.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToList();
					//get list of diseases already covered by checkboxes or radiobuttons
					List<string> listStrProblemChecks=EFormCur.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("problem:")).Select(x=>x.DbLink.Substring(8)).ToList();
					//First loop is to consider which ones to add based on user input
					for(int a=0;a<listStrsInput.Count;a++){
						if(listStringsPat.Contains(listStrsInput[a])){
							continue;
							//The patient already has this as an active problem, so there's no action for us to take.
							//This is common because of prefill.
						}
						if(listStrProblemChecks.Contains(listStrsInput[a])) {
							//If there is a checkbox or radiobutton for a problem, but user also types it in here,
							//then we have a slight conundrum. We have to pick one or the other, and they might not match.
							//We have decided in that case to depend on the checkbox/radiobutton and ignore whatever happens in this box.
							continue;
						}
						DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName.ToLower()==listStrsInput[a].ToLower());
						if(diseaseDef==null){
							//this problem that they typed in does not have any matching diseaseDef, so we will use "Other".
							//So we have to check if it exists as an "Other" problem
							DiseaseDef diseaseDefOther=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName=="Other");
							if(diseaseDefOther!=null){
								if(listDiseasesPat.Exists(x=>x.DiseaseDefNum==diseaseDefOther.DiseaseDefNum && x.PatNote==listStrsInput[a])){
									//this is common because of prefill
									continue;
								}
							}
						}
						//need to add one
						importRow=new ImportRow();
						importRow.FieldName="";//There is no fieldname that makes sense. It's a combination of multiple.
						importRow.OldValDisplay="";
						importRow.OldValObj=null;
						importRow.NewValDisplay=listStrsInput[a];//this is how we will know what to add
						importRow.NewValObj=null;//no object until we later create one
						importRow.ImpValDisplay="(add)";
						importRow.ImpValObj=null;//no object until we later create one
						importRow.TypeObj=typeof(Disease);
						importRow.DoImport=true;
						importRow.DoAdd=true;//although this will be ignored if not importing.
						_listImportRows.Add(importRow);
					}//for
					//Second loop to consider which ones to remove (this was not done for allergies)
					for(int a=0;a<listDiseasesPat.Count;a++){
						DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseDefNum==listDiseasesPat[a].DiseaseDefNum);
						if(listStrProblemChecks.Contains(diseaseDef.DiseaseName)) {
							//If there is a checkbox for a problem then we only pay attention to the checkbox.
							//An Other disease wouldn't be represented by a checkbox, so it would automatically pass through this check.
							continue;
						}
						if(diseaseDef.DiseaseName=="Other"){
							if(listStrsInput.Contains(listDiseasesPat[a].PatNote)){
								//This Other disease is still present in the typed box.
								//This is common because of prefill.
								continue;
							}
						}
						else if(listStrsInput.Contains(diseaseDef.DiseaseName)){
							//This disease is still present in the typed box.
							//This is common because of prefill.
							continue;
						}
						//This pt disease is not in typed in the box, so we remove it.
						ImportRow importRowProb=_listImportRows.Find(x=>
							x.TypeObj==typeof(Disease)
							&& x.OldValObj!=null
							&& ((Disease)x.OldValObj).DiseaseNum==listDiseasesPat[a].DiseaseNum);
						if(importRowProb==null){
							continue;//should never happen
						}
						importRowProb.DoRemove=true;//this is our trigger to remove the problem
						importRowProb.NewValDisplay="(removed)";
						importRowProb.ImpValDisplay="(mark inactive)";
						importRowProb.DoImport=true;
					}
				}//if problemsOther
				List<EFormField> listEFormFieldsProblemCheck=EFormCur.ListEFormFields.FindAll(x=>x.DbLink.StartsWith("problem:"));
				//Problem checkboxes and radiobuttons:
				for(int i=0;i<listEFormFieldsProblemCheck.Count;i++){
					string strProblemCheck=listEFormFieldsProblemCheck[i].DbLink.Substring(8);
					DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName.ToLower()==strProblemCheck.ToLower());
					if(diseaseDef==null){
						//This shouldn't normally happen, but it certainly is possible because user could change a diseaseDef name after adding the checkbox or radiobutton,
						//or this could be an imported form with no diseaseDef in the db representing the value in this checkbox/radiobut.
						//We will just add this for them. That way, all imported forms will correctly work.
						//We are doing it here instead of when clicking Import because it's extremely rare
						//and it's more of a setup issue. Also, it would be more complex to pass the info to that section.
						diseaseDef=new DiseaseDef();
						diseaseDef.DiseaseName=strProblemCheck;
						DiseaseDefs.Insert(diseaseDef);
						DataValid.SetInvalid(InvalidType.Diseases);
					}
					//diseaseDef is now guaranteed to have a value
					Disease diseasePat=listDiseasesPat.Find(x=>x.DiseaseDefNum==diseaseDef.DiseaseDefNum);
					//We don't also need to check list of pending imports because diseasesOther defers to this section.
					if(diseasePat==null){
						//This patient does not have an active disease matching the problem for this checkbox or radiobutton.
						//Notice that we do not look at inactive diseases and consider flipping them back to active.
						//Once someone has a problem, it tends to not go away. We don't need to consider that fancy edge case.
						//The worst consequence is that they might end up with one active and one inactive. No big deal.
						if(listEFormFieldsProblemCheck[i].ValueString=="X"//checkbox
							|| listEFormFieldsProblemCheck[i].ValueString=="Y")//radiobutton
						{
							//add
							importRow=new ImportRow();
							importRow.FieldName="";//There is no fieldname that makes sense. It's a combination of multiple.
							importRow.OldValDisplay="";
							importRow.OldValObj=null;
							importRow.NewValDisplay=strProblemCheck;//this is how we will know what to add
							importRow.NewValObj=null;//no object until we later create one
							importRow.ImpValDisplay="(add)";
							importRow.ImpValObj=null;//no object until we later create one
							importRow.TypeObj=typeof(Disease);
							importRow.DoImport=true;
							importRow.DoAdd=true;//although this will be ignored if not importing.
							_listImportRows.Add(importRow);
						}
						else{
							//db is already accurate
						}
					}
					else{
						//This patient does have an active disease matching the problem for this checkbox or radiobutton.
						if((listEFormFieldsProblemCheck[i].FieldType==EnumEFormFieldType.CheckBox
							&& listEFormFieldsProblemCheck[i].ValueString=="")
							|| listEFormFieldsProblemCheck[i].ValueString=="N")//radiobutton (empty rb indicates no change)
						{
							//remove
							ImportRow importRowProblem=_listImportRows.Find(x=>
								x.TypeObj==typeof(Disease)
								&& x.OldValObj!=null
								&& ((Disease)x.OldValObj).DiseaseDefNum==diseaseDef.DiseaseDefNum);
							if(importRowProblem==null){
								continue;//should never happen
							}
							importRowProblem.DoRemove=true;//this is our trigger to remove the allergy
							importRowProblem.NewValDisplay="(unchecked)";
							importRowProblem.ImpValDisplay="(mark inactive)";
							importRowProblem.DoImport=true;
						}
						else{
							//db is already accurate
						}
					}
				}//problem checkboxes and radiobuttons
			}
			#endregion Problems (eForms)
		}

		private void FillGrid() {
			int scrollVal=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn; 
			gridColumn=new GridColumn(Lan.g(this,"FieldName"),140);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lan.g(this,"Current Value"),175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lan.g(this,"Entered Value"),175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lan.g(this,"Import Value"),175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lan.g(this,"Do Import"),60,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			GridCell gridCell;
			for(int i=0;i<_listImportRows.Count;i++) {
				gridRow=new GridRow();
				if(_listImportRows[i].IsSectionHeader) {
					gridRow.Cells.Add(_listImportRows[i].FieldName);
					gridRow.Cells.Add("");
					gridRow.Cells.Add("");
					gridRow.Cells.Add("");
					gridRow.Cells.Add("");
					gridRow.ColorBackG=Color.DarkSlateGray;
					gridRow.ColorText=Color.White;
				}
				else {
					if(_listImportRows[i].FieldDisplay!=null) {
						gridRow.Cells.Add(_listImportRows[i].FieldDisplay);
					}
					else {
						gridRow.Cells.Add(_listImportRows[i].FieldName);
					}
					gridRow.Cells.Add(_listImportRows[i].OldValDisplay);
					gridCell=new GridCell(_listImportRows[i].NewValDisplay);
					if(_listImportRows[i].IsNewValRed) {
						gridCell.ColorText=Color.Firebrick;
						gridCell.Bold=YN.Yes;
					}
					gridRow.Cells.Add(gridCell);
					gridCell=new GridCell(_listImportRows[i].ImpValDisplay);
					if(_listImportRows[i].IsImportRed) {
						gridCell.ColorText=Color.Firebrick;
						gridCell.Bold=YN.Yes;
					}
					gridRow.Cells.Add(gridCell);
					if(_listImportRows[i].DoImport) {
						gridRow.Cells.Add("X");
						gridRow.ColorBackG=Color.FromArgb(225,225,225);
					}
					else {
						gridRow.Cells.Add("");
					}
				}
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=scrollVal;
		}

		/// <summary>For a Sheet, this tries to get the FieldValue from the InputField. For an eForm, this tries to get the ValueString for the DbLink. For ins field, it can also pull from OCR data. Null means that field will not be added to the list for import.</summary>
		private string GetInputValue(string fieldName) {
			string result=null;
			if(SheetCur!=null){
				SheetField sheetField=SheetCur.SheetFields.Find(
					x => x.FieldType==SheetFieldType.InputField && x.FieldName==fieldName);
				if(sheetField is null){
					return null;
				}
				result=sheetField.FieldValue;
			}
			if(EFormCur!=null){
				EFormField eFormField=EFormCur.ListEFormFields.Find(
					x => x.FieldType==EnumEFormFieldType.TextField && x.DbLink==fieldName);
				if(eFormField is null){
					return null;
				}
				result=eFormField.ValueString;
			}
			if(!fieldName.StartsWith("ins")) {
				return result;
			}
			//OCR processing of insurance card from here down
			if(result==null || result!="") {
				//If null, preserve the null.
				//If a value was found on the sheet, prefer the sheet value to ocrData.
				return result;
			}
			switch(fieldName) {
				case "ins1SubscriberNameF":
					result=_ocrResponsePrimaryFront.Member.Name;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.Member.Name;
					}
					break;
				case "ins1SubscriberID":
					result=_ocrResponsePrimaryFront.IdNumber.Prefix+_ocrResponsePrimaryFront.IdNumber.Number;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.IdNumber.Prefix+_ocrResponsePrimaryBack.IdNumber.Number;
					}
					break;
				case "ins1CarrierName":
					result=_ocrResponsePrimaryFront.Insurer;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.Insurer;
					}
					break;
				case "ins1CarrierPhone":
					result=_ocrResponsePrimaryFront.Payer.PhoneNumber;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.Payer.PhoneNumber;
					}
					break;
				case "ins1EmployerName":
					result=_ocrResponsePrimaryFront.Member.Employer;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.Member.Employer;
					}
					break;
				case "ins1GroupName":
					result=_ocrResponsePrimaryFront.Member.Employer;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.Member.Employer;
					}
					break;
				case "ins1GroupNum":
					result=_ocrResponsePrimaryFront.GroupNumber;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponsePrimaryBack.GroupNumber;
					}
					break;
				case "ins2SubscriberNameF":
					result=_ocrResponseSecondaryFront.Member.Name;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.Member.Name;
					}
					break;
				case "ins2SubscriberID":
					result=_ocrResponseSecondaryFront.IdNumber.Prefix+_ocrResponseSecondaryFront.IdNumber.Number;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.IdNumber.Prefix+_ocrResponseSecondaryBack.IdNumber.Number;
					}
					break;
				case "ins2CarrierName":
					result=_ocrResponseSecondaryFront.Insurer;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.Insurer;
					}
					break;
				case "ins2CarrierPhone":
					result=_ocrResponseSecondaryFront.Payer.PhoneNumber;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.Payer.PhoneNumber;
					}
					break;
				case "ins2EmployerName":
					result=_ocrResponseSecondaryFront.Member.Employer;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.Member.Employer;
					}
					break;
				case "ins2GroupName":
					result=_ocrResponseSecondaryFront.Member.Employer;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.Member.Employer;
					}
					break;
				case "ins2GroupNum":
					result=_ocrResponseSecondaryFront.GroupNumber;
					if(result.IsNullOrEmpty()) {
						result=_ocrResponseSecondaryBack.GroupNumber;
					}
					break;
				default: break;
			}
			//Turn null into empty if we got in here. The field exists on the sheet, but didnt have a value, neither does OcrData.
			//If we return null, the field wont be added to the grid. We know it should be there because sheetField.FieldValue was empty string.
			if(result==null) {
				result="";
			}
			return result;
		}

		///<summary>If no radiobox with that name exists, returns null.  If no box is checked, it returns empty string.  For regular sheets this will return either the corresponding enum value or the full word displayed on the form for the selected choice (if no corresponding enum), and EForms will return the DB field.  e.g. for StudentStatus a regular sheet might return "Fulltime" and an EForm would return "F".</summary>
		private string GetRadioValue(string fieldName) {
			if(SheetCur!=null){
				List<SheetField> listSheetFields=SheetCur?.SheetFields?.FindAll(x => x.FieldType==SheetFieldType.CheckBox && x.FieldName==fieldName);
				if(listSheetFields==null || listSheetFields.Count==0) {
					return null;
				}
				SheetField sheetField=listSheetFields.FirstOrDefault(x => x.FieldValue=="X");
				return sheetField==null ? "" : sheetField.RadioButtonValue;
			}
			if(EFormCur!=null){
				EFormField eFormField=EFormCur.ListEFormFields.Find(x=>x.DbLink==fieldName);
				if(eFormField is null){
					return null;
				}
				return eFormField.ValueString;
			}
			return null;
		}

		///<summary>Only the true condition is tested.  If the specified fieldName does not exist, returns false.</summary>
		private bool IsChecked(string fieldName) {
			if(SheetCur!=null){
				return SheetCur?.SheetFields?.FirstOrDefault(x => x.FieldType==SheetFieldType.CheckBox && x.FieldName==fieldName)?.FieldValue=="X";
			}
			return false;
		}

		///<summary>If the specified fieldName does not exist, returns null.</summary>
		private bool? IsCheckedEForm(string fieldName) {
			if(EFormCur!=null){
				EFormField eFormField=EFormCur.ListEFormFields.Find(x=>x.DbLink==fieldName);
				if(eFormField is null){
					return null;
				}
				return eFormField.ValueString=="X";
			}
			return null;
		}

		///<summary>Returns all the sheet fields with FieldNames that start with the passed-in string.</summary>
		private List<SheetField> GetSheetFieldsStartWith(string fieldName) {
			List<SheetField> listSheetFieldsReturnVal=new List<SheetField>();
			if(SheetCur==null) {
				return listSheetFieldsReturnVal;
			}
			for(int i=0;i<SheetCur.SheetFields.Count;i++) {
				//if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.CheckBox
				//	&& SheetCur.SheetFields[i].FieldType!=SheetFieldType.InputField
				//	&& SheetCur.SheetFields[i].FieldType!=SheetFieldType.OutputText) 
				//{
				//	continue;
				//}
				if(!SheetCur.SheetFields[i].FieldName.StartsWith(fieldName)) {
					continue;
				}
				listSheetFieldsReturnVal.Add(SheetCur.SheetFields[i]);
			}
			return listSheetFieldsReturnVal;
		}
		
		///<summary>Returns one sheet field with the same FieldName. Returns null if not found.</summary>
		private ImportRow GetImportRowByFieldName(string fieldName) {
			if(_listImportRows==null) {
				return null;
			}
			for(int i=0;i<_listImportRows.Count;i++) {
				if(_listImportRows[i].FieldName==fieldName){
					return _listImportRows[i];
				}
			}
			return null;
		}

		///<summary>Loops through the list passed in returns the opposite check box.  Returns null if one is not found.</summary>
		private SheetField GetOppositeSheetFieldCheckBox(List<SheetField> sheetFieldList,SheetField sheetFieldCur) {
			for(int i=0;i<sheetFieldList.Count;i++) {
				if(sheetFieldList[i].SheetFieldNum==sheetFieldCur.SheetFieldNum) {
					continue;
				}
				//FieldName will be the same.  Ex: allergy:Sudafed 
				if(sheetFieldList[i].FieldName!=sheetFieldCur.FieldName) {
					continue;
				}
				//This has to be the opposite check box.
				return sheetFieldList[i];
			}
			return null;
		}

		///<summary>Returns all checkboxes related to the inputMed passed in.</summary>
		private List<SheetField> GetRelatedMedicalCheckBoxes(List<SheetField> checkMedList,SheetField inputMed) {
			List<SheetField> listSheetFieldsCheckBoxes=new List<SheetField>();
			for(int i=0;i<checkMedList.Count;i++) {
				if(checkMedList[i].FieldName.Remove(0,8)==inputMed.FieldName.Remove(0,8)) {
					listSheetFieldsCheckBoxes.Add(checkMedList[i]);
				}
			}
			return listSheetFieldsCheckBoxes;
		}

		private bool ContainsOneOfFields(params string[] fieldNameArray) {
			if(SheetCur!=null) {
				return SheetCur.SheetFields.Any(x => fieldNameArray.Contains(x.FieldName));
			}
			if(EFormCur!=null){
				return EFormCur.ListEFormFields.Any(x => fieldNameArray.Contains(x.DbLink));
			}
			return false;//shouldn't happen
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col!=4) {
				return;
			}
			if(_listImportRows[e.Row].IsSectionHeader) {
				return;
			}
			if(!IsImportable(_listImportRows[e.Row])) {
				return;
			}
			_listImportRows[e.Row].DoImport=!_listImportRows[e.Row].DoImport;
			FillGrid();
		}

		///<summary>Mostly the same as IsImportable.  But subtle differences.</summary>
		private bool IsEditable(ImportRow row) {
			if(row.FieldName=="wirelessCarrier"){
				MessageBox.Show(row.FieldName+" "+Lan.g(this,"cannot be imported."));
				return false;
			}
			if(row.FieldName=="referredFrom") {
				if(row.OldValObj!=null) {
					MsgBox.Show(this,"This patient already has a referral source selected and it cannot be changed from here.");
					return false;
				}
			}
			return true;
		}

		private bool IsImportable(ImportRow row) {
			if(row.ImpValObj==null) {
				MsgBox.Show(this,"Please enter a value for this row first.");
				return false;
			}
			return IsEditable(row);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Col!=3) {
				return;
			}
			if(_listImportRows[e.Row].IsSectionHeader) {
				return;
			}
			if(!IsEditable(_listImportRows[e.Row])){
				return;
			}
			if(_listImportRows[e.Row].FieldName=="referredFrom") {
				FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
				frmReferralSelect.IsSelectionMode=true;
				frmReferralSelect.ShowDialog();
				if(frmReferralSelect.IsDialogCancel) {
					return;
				}
				Referral referralSelected=frmReferralSelect.ReferralSelected;
				_listImportRows[e.Row].DoImport=true;
				_listImportRows[e.Row].IsImportRed=false;
				_listImportRows[e.Row].ImpValDisplay=referralSelected.GetNameFL();
				_listImportRows[e.Row].ImpValObj=referralSelected;
			}
			#region string
			else if(_listImportRows[e.Row].TypeObj==typeof(string)) {
				InputBox inputBox;
				if(_listImportRows[e.Row].FieldName.In("WkPhone","WirelessPhone","HmPhone","ins1CarrierPhone","ins2CarrierPhone","ICEPhone")) {
					InputBoxParam inputBoxParam=new InputBoxParam();
					inputBoxParam.InputBoxType_=InputBoxType.ValidPhone;
					inputBoxParam.LabelText=_listImportRows[e.Row].FieldName;
					inputBoxParam.Text=_listImportRows[e.Row].ImpValDisplay;
					inputBox=new InputBox(inputBoxParam);
				}
				else {
					InputBoxParam inputBoxParam=new InputBoxParam();
					inputBoxParam.InputBoxType_=InputBoxType.TextBox;
					inputBoxParam.LabelText=_listImportRows[e.Row].FieldName;
					inputBoxParam.Text=_listImportRows[e.Row].ImpValDisplay;
					inputBox=new InputBox(inputBoxParam);
				}
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return;
				}
				if(_listImportRows[e.Row].FieldName=="addressAndHmPhoneIsSameEntireFamily") {
					if(inputBox.StringResult=="") {
						_isAddressSameForFam=false;
					}
					else if(inputBox.StringResult!="X") {
						_isAddressSameForFam=true;
					}
					else {
						MsgBox.Show(this,"The only allowed values are X or blank.");
						return;
					}
				}
				if(_listImportRows[e.Row].OldValDisplay==inputBox.StringResult) {//value is now same as original
					_listImportRows[e.Row].DoImport=false;
				}
				else {
					_listImportRows[e.Row].DoImport=true;
				}
				_listImportRows[e.Row].ImpValDisplay=inputBox.StringResult;
				_listImportRows[e.Row].ImpValObj=inputBox.StringResult;
			}
			#endregion
			#region Enum
			else if(_listImportRows[e.Row].TypeObj.IsEnum) {
				//Note.  This only works for zero-indexed enums.
				using FormSheetImportEnumPicker formSheetImportEnumPicker=new FormSheetImportEnumPicker(_listImportRows[e.Row].FieldName);
				for(int i=0;i<Enum.GetNames(_listImportRows[e.Row].TypeObj).Length;i++) {
					formSheetImportEnumPicker.listResult.Items.Add(Enum.GetNames(_listImportRows[e.Row].TypeObj)[i]);
					if(_listImportRows[e.Row].ImpValObj!=null && i==(int)_listImportRows[e.Row].ImpValObj) {
						formSheetImportEnumPicker.listResult.SelectedIndex=i;
					}
				}
				formSheetImportEnumPicker.ShowDialog();
				if(formSheetImportEnumPicker.DialogResult==DialogResult.OK) {
					int selectedI=formSheetImportEnumPicker.listResult.SelectedIndex;
					if(_listImportRows[e.Row].ImpValObj==null) {//was initially null
						if(selectedI!=-1) {//an item was selected
							_listImportRows[e.Row].ImpValObj=Enum.ToObject(_listImportRows[e.Row].TypeObj,selectedI);
							_listImportRows[e.Row].ImpValDisplay=_listImportRows[e.Row].ImpValObj.ToString();
						}
					}
					else {//was not initially null
						if((int)_listImportRows[e.Row].ImpValObj!=selectedI) {//value was changed.
							//There's no way for the user to set it to null, so we do not need to test that
							_listImportRows[e.Row].ImpValObj=Enum.ToObject(_listImportRows[e.Row].TypeObj,selectedI);
							_listImportRows[e.Row].ImpValDisplay=_listImportRows[e.Row].ImpValObj.ToString();
						}
					}
					if(selectedI==-1) {
						_listImportRows[e.Row].DoImport=false;//impossible to import a null
					}
					else if(_listImportRows[e.Row].OldValObj!=null && (int)_listImportRows[e.Row].ImpValObj==(int)_listImportRows[e.Row].OldValObj) {//it's the old setting for the patient, whether or not they actually changed it.
						_listImportRows[e.Row].DoImport=false;//so no need to import
					}
					else {
						_listImportRows[e.Row].DoImport=true;
					}
				}
			}
			#endregion
			#region DateTime
			else if(_listImportRows[e.Row].TypeObj==typeof(DateTime)) {//this is only for one field so far: Birthdate
				InputBoxParam inputBoxParam=new InputBoxParam();
				inputBoxParam.InputBoxType_=InputBoxType.ValidDate;
				inputBoxParam.LabelText=_listImportRows[e.Row].FieldName;
				//Display the importing date if valid, otherwise display the invalid date from the form.
				if(_listImportRows[e.Row].ImpValDisplay==INVALID_DATE){
					inputBoxParam.Text=_listImportRows[e.Row].NewValDisplay;
				}
				else {
					inputBoxParam.Text=_listImportRows[e.Row].ImpValDisplay;
				}
				InputBox inputBox=new InputBox(inputBoxParam);//Display the date format that the current computer will use when parsing the date.
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return;
				}
				DateTime dateTimeEntered=inputBox.DateResult;
				if(dateTimeEntered==DateTime.MinValue) {
					_listImportRows[e.Row].ImpValDisplay="";
				}
				else if(dateTimeEntered.Year<1880 || dateTimeEntered.Year>2050) {
					MsgBox.Show(this,INVALID_DATE);
					return;
				}
				else {
					_listImportRows[e.Row].ImpValDisplay=dateTimeEntered.ToShortDateString();
				}
				_listImportRows[e.Row].ImpValObj=dateTimeEntered;
				if(_listImportRows[e.Row].ImpValDisplay==_listImportRows[e.Row].OldValDisplay) {//value is now same as original
					_listImportRows[e.Row].DoImport=false;
				}
				else {
					_listImportRows[e.Row].DoImport=true;
				}
			}
			#endregion
			#region Medication, Allergy or Disease
			else if(_listImportRows[e.Row].TypeObj==typeof(MedicationPat)
				|| _listImportRows[e.Row].TypeObj==typeof(Allergy)
				|| _listImportRows[e.Row].TypeObj==typeof(Disease)) 
			{
				//User entered medications will have a MedicationNum as the ImpValObj.
				if(_listImportRows[e.Row].ImpValObj!=null
					&& _listImportRows[e.Row].ImpValObj.GetType()==typeof(long)) 
				{
					using FormMedications formMedications=new FormMedications();
					formMedications.IsSelectionMode=true;
					formMedications.textSearch.Text=_listImportRows[e.Row].FieldName.Trim();
					formMedications.ShowDialog();
					if(formMedications.DialogResult!=DialogResult.OK) {
						return;
					}
					_listImportRows[e.Row].ImpValDisplay="Y";
					_listImportRows[e.Row].ImpValObj=formMedications.SelectedMedicationNum;
					string descript=Medications.GetDescription(formMedications.SelectedMedicationNum);
					_listImportRows[e.Row].FieldDisplay=descript;
					((SheetField)_listImportRows[e.Row].NewValObj).FieldValue=descript;
					_listImportRows[e.Row].NewValDisplay="Y";
					_listImportRows[e.Row].DoImport=true;
					_listImportRows[e.Row].IsImportRed=false;
				}
				else {
					using FormSheetImportEnumPicker formSheetImportEnumPicker=new FormSheetImportEnumPicker(_listImportRows[e.Row].FieldName);
					formSheetImportEnumPicker.listResult.Items.AddEnums<YN>();
					formSheetImportEnumPicker.listResult.SelectedIndex=0;//Unknown
					if(_listImportRows[e.Row].ImpValDisplay=="Y") {
						formSheetImportEnumPicker.listResult.SelectedIndex=1;
					}
					if(_listImportRows[e.Row].ImpValDisplay=="N") {
						formSheetImportEnumPicker.listResult.SelectedIndex=2;
					}
					formSheetImportEnumPicker.ShowDialog();
					if(formSheetImportEnumPicker.DialogResult!=DialogResult.OK) {
						return;
					}
					int selectedI=formSheetImportEnumPicker.listResult.SelectedIndex;
					switch(selectedI) {
						case 0:
							_listImportRows[e.Row].ImpValDisplay="";
							break;
						case 1:
							_listImportRows[e.Row].ImpValDisplay="Y";
							break;
						case 2:
							_listImportRows[e.Row].ImpValDisplay="N";
							break;
					}
					if(_listImportRows[e.Row].OldValDisplay==_listImportRows[e.Row].ImpValDisplay) {//value is now same as original
						_listImportRows[e.Row].DoImport=false;
					}
					else {
						_listImportRows[e.Row].DoImport=true;
					}
					if(selectedI==-1 || selectedI==0) {
						_listImportRows[e.Row].DoImport=false;
					}
				}
			}
			#endregion
			#region Subscriber
			else if(_listImportRows[e.Row].TypeObj==typeof(Patient)) {
				Patient patientSubscriber=new Patient();
				using FormSubscriberSelect formSubscriberSelect=new FormSubscriberSelect(_family);
				formSubscriberSelect.ShowDialog();
				if(formSubscriberSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				patientSubscriber=Patients.GetPat(formSubscriberSelect.PatNumSelected);
				if(patientSubscriber==null) {
					return;//Should never happen but is a possibility.
				}
				//Use GetNameFirst() because this is how OldValDisplay is displayed.
				string patName=Patients.GetNameFirst(patientSubscriber.FName,patientSubscriber.Preferred);
				if(_listImportRows[e.Row].OldValDisplay==patName) {
					_listImportRows[e.Row].DoImport=false;
				}
				else {
					_listImportRows[e.Row].DoImport=true;
				}
				_listImportRows[e.Row].ImpValDisplay=patName;
				_listImportRows[e.Row].ImpValObj=patientSubscriber;
			}
			#endregion
			#region Carrier
			else if(_listImportRows[e.Row].TypeObj==typeof(Carrier)) {
				//Change both carrier rows at the same time.
				string insStr="ins1";
				if(_listImportRows[e.Row].FieldName.StartsWith("ins2")) {
					insStr="ins2";
				}
				ImportRow sheetImportRowCarrierName=GetImportRowByFieldName(insStr+"CarrierName");
				ImportRow sheetImportRowCarrierPhone=GetImportRowByFieldName(insStr+"CarrierPhone");
				Carrier carrier=new Carrier();
				using FormCarriers formCarriers=new FormCarriers();
				formCarriers.IsSelectMode=true;
				if(sheetImportRowCarrierName!=null) {
					formCarriers.textCarrier.Text=sheetImportRowCarrierName.NewValDisplay;
				}
				if(sheetImportRowCarrierPhone!=null) {
					formCarriers.textPhone.Text=sheetImportRowCarrierPhone.NewValDisplay;
				}
				formCarriers.ShowDialog();
				if(formCarriers.DialogResult!=DialogResult.OK) {
					return;
				}
				carrier=formCarriers.CarrierSelected;
				//Check for nulls because the name AND phone rows might not both be on the sheet.
				if(sheetImportRowCarrierName!=null) {
					if(sheetImportRowCarrierName.OldValDisplay==carrier.CarrierName) {
						sheetImportRowCarrierName.DoImport=false;
					}
					else {
						sheetImportRowCarrierName.DoImport=true;
					}
					sheetImportRowCarrierName.ImpValDisplay=carrier.CarrierName;
					sheetImportRowCarrierName.ImpValObj=carrier;
				}
				if(sheetImportRowCarrierPhone!=null) {
					if(sheetImportRowCarrierPhone.OldValDisplay==carrier.Phone) {
						sheetImportRowCarrierPhone.DoImport=false;
					}
					else {
						sheetImportRowCarrierPhone.DoImport=true;
					}
					sheetImportRowCarrierPhone.ImpValDisplay=carrier.Phone;
					sheetImportRowCarrierPhone.ImpValObj=carrier;
				}
			}
			#endregion
			FillGrid();
		}

		///<summary>Correctly sets the class wide boolean HasRequiredInsFields.  Loops through all the fields on the sheet and makes sure all the required insurance fields needed to import are present for primary OR for secondary insurance.  If some required fields are missing for an insurance, all related ins fields will have DoImport set to false.  Called after the list "Rows" has been filled.</summary>
		private void SetHasRequiredInsFields() {
			//Start off assuming that neither primary nor secondary have the required insurance fields necessary to import insurance.
			_hasRequiredInsFields=false;
			if(CheckSheetForInsFields(true)) {//Check primary fields.
				_hasRequiredInsFields=true;
			}
			else {//Sheet does not have the required fields to import primary insurance.
				SetDoImportToFalseForIns(true);//Unmark all primary ins fields for import.
			}
			if(CheckSheetForInsFields(false)) {//Check secondary fields.
				_hasRequiredInsFields=true;
			}
			else {//Sheet does not have the required fields to import secondary insurance.
				SetDoImportToFalseForIns(false);//Unmark all secondary ins fields for import.
			}
		}

		///<summary>Returns true if all the required insurance fields needed to import are present on the current sheet.  Only call after the list "Rows" has been filled.</summary>
		private bool CheckSheetForInsFields(bool isPrimary) {
			string insStr="ins1";
			if(!isPrimary) {
				insStr="ins2";
			}
			//Load up all five required insurance rows.
			ImportRow sheetImportRowRelation=GetImportRowByFieldName(insStr+"Relat");
			ImportRow sheetImportRowSubscriber=GetImportRowByFieldName(insStr+"Subscriber");
			ImportRow sheetImportRowSubscriberId=GetImportRowByFieldName(insStr+"SubscriberID");
			ImportRow sheetImportRowCarrierName=GetImportRowByFieldName(insStr+"CarrierName");
			ImportRow sheetImportRowCarrierPhone=GetImportRowByFieldName(insStr+"CarrierPhone");
			//Check if all of the required insurance fields exist on this sheet.
			if(sheetImportRowRelation==null 
				|| sheetImportRowSubscriber==null
				|| sheetImportRowSubscriberId==null
				|| sheetImportRowCarrierName==null
				|| sheetImportRowCarrierPhone==null) 
			{
				return false;
			}
			return true;
		}

		///<summary>Loops through the related ins fields and forces DoImport to false.</summary>
		private void SetDoImportToFalseForIns(bool isPrimary) {
			bool isChanged=false;
			string insStr="ins1";
			if(!isPrimary) {
				insStr="ins2";
			}
			//Only five ins fields have the possibility of DoImport being automatically set to true.  The others require a double click.
			ImportRow sheetImportRowRelation=GetImportRowByFieldName(insStr+"Relat");
			ImportRow sheetImportRowSubscriberId=GetImportRowByFieldName(insStr+"SubscriberID");
			ImportRow sheetImportRowEmployerName=GetImportRowByFieldName(insStr+"EmployerName");
			ImportRow sheetImportRowGroupName=GetImportRowByFieldName(insStr+"GroupName");
			ImportRow sheetImportRowGroupNum=GetImportRowByFieldName(insStr+"GroupNum");
			if(sheetImportRowRelation!=null) {
				sheetImportRowRelation.DoImport=false;
				isChanged=true;
			}
			if(sheetImportRowSubscriberId!=null) {
				sheetImportRowSubscriberId.DoImport=false;
				isChanged=true;
			}
			if(sheetImportRowEmployerName!=null) {
				sheetImportRowEmployerName.DoImport=false;
				isChanged=true;
			}
			if(sheetImportRowGroupName!=null) {
				sheetImportRowGroupName.DoImport=false;
				isChanged=true;
			}
			if(sheetImportRowGroupNum!=null) {
				sheetImportRowGroupNum.DoImport=false;
				isChanged=true;
			}
			if(isChanged) {
				FillGrid();//A change was made, refresh the grid.
			}
		}

		///<summary>Returns false if validation fails.  Returns true if all required insurance fields exist, import fields have valid values, and the insurance plan has been imported successfully.  The user will have the option to pick an existing ins plan.  If any fields on the selected plan do not exactly match the imported fields, they will be prompted to choose between the selected plan's values or to create a new ins plan with the import values.  After validating, the actual import of the new ins plan takes place.  That might consist of dropping the current plan and replacing it or simply inserting the new plan.</summary>
		private bool ValidateAndImportInsurance(bool isPrimary) {
			string insStr="";
			string insWarnStr="";
			byte ordinal;
			if(isPrimary) {
				insStr="ins1";
				insWarnStr="primary insurance";
				ordinal=1;
			}
			else {
				insStr="ins2";
				insWarnStr="secondary insurance";
				ordinal=2;
			}
			//Load up every insurance row related to the particular ins.
			ImportRow sheetImportRowRelation=GetImportRowByFieldName(insStr+"Relat");
			ImportRow sheetImportRowSubscriber=GetImportRowByFieldName(insStr+"Subscriber");
			ImportRow sheetImportRowSubscriberId=GetImportRowByFieldName(insStr+"SubscriberID");
			ImportRow sheetImportRowCarrierName=GetImportRowByFieldName(insStr+"CarrierName");
			ImportRow sheetImportRowCarrierPhone=GetImportRowByFieldName(insStr+"CarrierPhone");
			ImportRow sheetImportRowEmployerName=GetImportRowByFieldName(insStr+"EmployerName");
			ImportRow sheetImportRowGroupName=GetImportRowByFieldName(insStr+"GroupName");
			ImportRow sheetImportRowGroupNum=GetImportRowByFieldName(insStr+"GroupNum");
			//Check if the required insurance fields exist on this sheet.
			//NOTE: Employer, group name and group num are optional fields.
			//Checking for nulls in the required fields still needs to be here in this method in case the user has the required fields for one insurance plan but not enough for the other.  They will hit this code ONLY if they have flagged one of the fields on the "other" insurance plan for import that does not have all of the required fields.
			if(sheetImportRowRelation==null 
				|| sheetImportRowSubscriber==null
				|| sheetImportRowSubscriberId==null
				|| sheetImportRowCarrierName==null
				|| sheetImportRowCarrierPhone==null) 
			{
				MessageBox.Show(Lan.g(this,"Required ")+insWarnStr+Lan.g(this," fields are missing on this sheet.  You cannot import ")+insWarnStr
					+Lan.g(this," with this sheet until it contains all of required fields.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone."));
				return false;
			}
			if(sheetImportRowRelation.ImpValObj==null 
				|| sheetImportRowSubscriber.ImpValObj==null
				|| (string)sheetImportRowSubscriberId.ImpValObj==""
				|| sheetImportRowCarrierName.ImpValObj==null
				|| sheetImportRowCarrierPhone.ImpValObj==null) {
				MessageBox.Show(Lan.g(this,"Cannot import ")+insWarnStr+Lan.g(this," until all required fields have been set.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone."));
				return false;
			}
			InsPlan insPlan=null;
			InsSub insSub=null;
			long insSubNum=0;
			long employerNum=0;
			//Get the employer from the db.  If no matching employer found, a new one will automatically get created.
			if(sheetImportRowEmployerName!=null && sheetImportRowEmployerName.ImpValDisplay.Trim()!="") {
				employerNum=Employers.GetEmployerNum(sheetImportRowEmployerName.ImpValDisplay);
			}
			Patient patientSubscriber=(Patient)sheetImportRowSubscriber.ImpValObj;
			//Have user pick a plan------------------------------------------------------------------------------------------------------------
			bool isPlanNew=false;
			List<InsSub> listInsSubs=InsSubs.GetListForSubscriber(patientSubscriber.PatNum);
			FrmInsPlanSelect frmInsPlanSelect=new FrmInsPlanSelect();
			frmInsPlanSelect.carrierText=sheetImportRowCarrierName.ImpValDisplay;
			if(sheetImportRowEmployerName!=null) {
				frmInsPlanSelect.empText=sheetImportRowEmployerName.ImpValDisplay;
			}
			if(sheetImportRowGroupName!=null) {
				frmInsPlanSelect.groupNameText=sheetImportRowGroupName.ImpValDisplay;
			}
			if(sheetImportRowGroupNum!=null) {
				frmInsPlanSelect.groupNumText=sheetImportRowGroupNum.ImpValDisplay;
			}
			frmInsPlanSelect.ShowDialog();
			if(!frmInsPlanSelect.IsDialogOK) {
				return false;
			}
			insPlan=frmInsPlanSelect.InsPlanSelected;
			if(insPlan.PlanNum==0) {
				//User clicked blank plan, so a new plan will be created using the import values.
				isPlanNew=true;
			}
			else {//An existing plan was selected so see if the plan is already subscribed to by the subscriber or create a new inssub.  Patplan will be taken care of later.
				for(int i=0;i<listInsSubs.Count;i++) {
					if(listInsSubs[i].PlanNum==insPlan.PlanNum) {
						insSub=listInsSubs[i];
						insSubNum=insSub.InsSubNum;
					}
				}
				if(insSub==null) {//Create a new inssub if subscriber is not subscribed to this plan yet.
					insSub=new InsSub();
					insSub.PlanNum=insPlan.PlanNum;
					insSub.Subscriber=patientSubscriber.PatNum;
					insSub.SubscriberID=sheetImportRowSubscriberId.ImpValDisplay;
					insSub.ReleaseInfo=true;
					insSub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
					insSubNum=InsSubs.Insert(insSub);
				}
			}
			//User picked a plan but the information they want to import might be different than the chosen plan.  Give them options to use current values or created a new plan.
			//It's still okay to let the user return at this point in order to change importing information.
			DialogResult result;
			//Carrier check-----------------------------------------------------------------------------------------
			if(!isPlanNew && insPlan.CarrierNum!=((Carrier)sheetImportRowCarrierName.ImpValObj).CarrierNum) {
				result=InsuranceImportQuestion("carrier",isPrimary);
				//Yes means the user wants to keep the information on the plan they picked, nothing to do.
				if(result==DialogResult.No) {
					isPlanNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Employer check----------------------------------------------------------------------------------------
			if(!isPlanNew && employerNum>0 && insPlan.EmployerNum!=employerNum) {
				result=InsuranceImportQuestion("employer",isPrimary);
				if(result==DialogResult.No) {
					isPlanNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Subscriber check--------------------------------------------------------------------------------------
			if(!isPlanNew && insSub.Subscriber!=((Patient)sheetImportRowSubscriber.ImpValObj).PatNum) {
				result=InsuranceImportQuestion("subscriber",isPrimary);
				if(result==DialogResult.No) {
					isPlanNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			if(!isPlanNew && insSub.SubscriberID!=sheetImportRowSubscriberId.ImpValDisplay) {
				result=InsuranceImportQuestion("subscriber id",isPrimary);
				if(result==DialogResult.No) {
					isPlanNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Group name check--------------------------------------------------------------------------------------
			if(sheetImportRowGroupName!=null && !isPlanNew && insPlan.GroupName!=sheetImportRowGroupName.ImpValDisplay) {
				result=InsuranceImportQuestion("group name",isPrimary);
				if(result==DialogResult.No) {
					isPlanNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Group num check---------------------------------------------------------------------------------------
			if(sheetImportRowGroupNum!=null && !isPlanNew && insPlan.GroupNum!=sheetImportRowGroupNum.ImpValDisplay) {
				result=InsuranceImportQuestion("group num",isPrimary);
				if(result==DialogResult.No) {
					isPlanNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Create a new plan-------------------------------------------------------------------------------------
			if(isPlanNew) {
				insPlan=new InsPlan();
				if(employerNum>0) {
					insPlan.EmployerNum=employerNum;
				}
				insPlan.PlanType=Prefs.GetBoolNoCache(PrefName.InsDefaultPPOpercent) ? "p" : "";
				insPlan.CarrierNum=((Carrier)sheetImportRowCarrierName.ImpValObj).CarrierNum;
				if(sheetImportRowGroupName!=null) {
					insPlan.GroupName=sheetImportRowGroupName.ImpValDisplay;
				}
				if(sheetImportRowGroupNum!=null) {
					insPlan.GroupNum=sheetImportRowGroupNum.ImpValDisplay;
				}
				InsPlans.Insert(insPlan);
				insSub=new InsSub();
				insSub.PlanNum=insPlan.PlanNum;
				insSub.Subscriber=patientSubscriber.PatNum;
				insSub.SubscriberID=sheetImportRowSubscriberId.ImpValDisplay;
				insSub.ReleaseInfo=true;
				insSub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSubNum=InsSubs.Insert(insSub);
				Benefit benefit;
				List<CovCat> listCovCats=CovCats.GetDeepCopy(true);
				for(int i=0;i<listCovCats.Count;i++) {
					if(listCovCats[i].DefaultPercent==-1) {
						continue;
					}
					benefit=new Benefit();
					benefit.BenefitType=InsBenefitType.CoInsurance;
					benefit.CovCatNum=listCovCats[i].CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.Percent=listCovCats[i].DefaultPercent;
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					benefit.CodeNum=0;
					Benefits.Insert(benefit);
				}
				//Zero deductible diagnostic
				if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
					benefit=new Benefit();
					benefit.CodeNum=0;
					benefit.BenefitType=InsBenefitType.Deductible;
					benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					benefit.MonetaryAmt=0;
					benefit.Percent=-1;
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;
					Benefits.Insert(benefit);
				}
				//Zero deductible preventive
				if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
					benefit=new Benefit();
					benefit.CodeNum=0;
					benefit.BenefitType=InsBenefitType.Deductible;
					benefit.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
					benefit.PlanNum=insPlan.PlanNum;
					benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					benefit.MonetaryAmt=0;
					benefit.Percent=-1;
					benefit.CoverageLevel=BenefitCoverageLevel.Individual;
					Benefits.Insert(benefit);
				}
			}
			//Delete the old patplan-------------------------------------------------------------------------------------------------------------
			if(isPrimary && _patPlan1!=null) {//Importing primary and currently has primary ins.
				PatPlans.DeleteNonContiguous(_patPlan1.PatPlanNum);
			}
			if(!isPrimary && _patPlan2!=null) {//Importing secondary and currently has secondary ins.
				PatPlans.DeleteNonContiguous(_patPlan2.PatPlanNum);
			}
			//Then attach new patplan to the plan------------------------------------------------------------------------------------------------
			PatPlan patplan=new PatPlan();
			patplan.Ordinal=ordinal;//Not allowed to be 0.
			patplan.PatNum=_patient.PatNum;
			patplan.InsSubNum=insSubNum;
			patplan.Relationship=((Relat)sheetImportRowRelation.ImpValObj);
			PatPlans.Insert(patplan);
			//After new plan has been imported, recompute all estimates for this patient because their coverage is now different.  Also set patient.HasIns to the correct value.
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			List<Procedure> listProcedures=Procedures.Refresh(_patient.PatNum);
			List<PatPlan> listPatPlans=PatPlans.Refresh(_patient.PatNum);
			listInsSubs=InsSubs.RefreshForFam(_family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);
			Procedures.ComputeEstimatesForAll(_patient.PatNum,listClaimProcs,listProcedures,listInsPlans,listPatPlans,listBenefits,_patient.Age,listInsSubs);
			Patients.SetHasIns(_patient.PatNum);
			return true;
		}

		///<summary>Displays a yes no cancel message to the user indicating that the import value does not match the selected plan.  They will choose to use the current plan's value or create a new plan.  Only called from ValidateAndImportInsurance.</summary>
		private DialogResult InsuranceImportQuestion(string importValue,bool isPrimary) {
			string insStr="primary ";
			if(!isPrimary) {
				insStr="secondary ";
			}
			MessageBoxButtons msgBoxButton=MessageBoxButtons.OKCancel;
			string createNewPlanMsg="";
			if(Security.IsAuthorized(EnumPermType.InsPlanEdit,true)) {
				msgBoxButton=MessageBoxButtons.YesNoCancel;
				createNewPlanMsg=$"\r\n\r\n{Lan.g(this,"No will create a new plan using all of the import values.")}";
			}
			return MessageBox.Show(Lan.g(this,"The ")+insStr+importValue+Lan.g(this," does not match the selected plan's ")+importValue+".\r\n"
				+Lan.g(this,"Use the selected plan's ")+importValue+"?"
				+createNewPlanMsg,Lan.g(this,"Import ")+insStr+importValue,msgBoxButton);
		}

		private void butImport_Click(object sender,EventArgs e) {
			if(!_listImportRows.Any(x => x.DoImport)) {
				MsgBox.Show(this,"No rows are set for import.");
				return;
			}
			if(_listImportRows.Any(x => x.DoImport && x.ImpValObj is DateTime && x.ImpValDisplay==INVALID_DATE)) {
				MsgBox.Show(this,$"Please fix data entry errors first (ImportValues with '{INVALID_DATE}').");
				return;
			}
			#region Patient Form
			if(EFormCur!=null
				|| (SheetCur!=null && SheetCur.SheetType==SheetTypeEnum.PatientForm)) 
			{
				bool isImportingPriIns=false;
				bool isImportingSecIns=false;
				List<RefAttach> listRefAttaches=new List<RefAttach>();
				for(int i=0;i<_listImportRows.Count;i++) {
					if(!_listImportRows[i].DoImport) {
						continue;
					}
					//Importing insurance happens later.
					if(_listImportRows[i].FieldName.StartsWith("ins1")) {
						isImportingPriIns=true;
						continue;
					}
					if(_listImportRows[i].FieldName.StartsWith("ins2")) {
						isImportingSecIns=true;
						continue;
					}
					switch(_listImportRows[i].FieldName) {
						#region Personal
						case "LName":
							_patient.LName=_listImportRows[i].ImpValDisplay;
							break;
						case "FName":
							_patient.FName=_listImportRows[i].ImpValDisplay;
							break;
						case "MiddleI":
							_patient.MiddleI=_listImportRows[i].ImpValDisplay;
							break;
						case "Preferred":
							_patient.Preferred=_listImportRows[i].ImpValDisplay;
							break;
						case "Gender":
							_patient.Gender=(PatientGender)_listImportRows[i].ImpValObj;
							break;
						case "Position":
							_patient.Position=(PatientPosition)_listImportRows[i].ImpValObj;
							break;
						case "Birthdate":
							_patient.Birthdate=(DateTime)_listImportRows[i].ImpValObj;
							break;
						case "SSN":
							_patient.SSN=_listImportRows[i].ImpValDisplay;
							break;
						case "WkPhone":
							_patient.WkPhone=_listImportRows[i].ImpValDisplay;
							break;
						case "WirelessPhone":
							_patient.WirelessPhone=_listImportRows[i].ImpValDisplay;
							break;
						case "Email":
							_patient.Email=_listImportRows[i].ImpValDisplay;
							break;
						case "PreferContactMethod":
							_patient.PreferContactMethod=(ContactMethod)_listImportRows[i].ImpValObj;
							break;
						case "PreferConfirmMethod":
							_patient.PreferConfirmMethod=(ContactMethod)_listImportRows[i].ImpValObj;
							break;
						case "PreferRecallMethod":
							_patient.PreferRecallMethod=(ContactMethod)_listImportRows[i].ImpValObj;
							break;
						case "referredFrom":
							RefAttach refAttach=new RefAttach();
							refAttach.RefType=ReferralType.RefFrom;
							refAttach.ItemOrder=1;
							refAttach.PatNum=_patient.PatNum;
							refAttach.RefDate=DateTime.Today;
							refAttach.ReferralNum=((Referral)_listImportRows[i].ImpValObj).ReferralNum;
							listRefAttaches.Add(refAttach);
							break;
						case "StudentStatus":
							_patient.StudentStatus=(string)_listImportRows[i].ImpValObj;
							break;
						#endregion
						#region Address and Home Phone
						//AddressSameForFam already set, but not really importable by itself
						case "Address":
							_patient.Address=_listImportRows[i].ImpValDisplay;
							break;
						case "Address2":
							_patient.Address2=_listImportRows[i].ImpValDisplay;
							break;
						case "City":
							_patient.City=_listImportRows[i].ImpValDisplay;
							break;
						case "State":
							_patient.State=_listImportRows[i].ImpValDisplay;
							break;
						case "StateNoValidation":
							_patient.State=_listImportRows[i].ImpValDisplay;
							break;
						case "Zip":
							_patient.Zip=_listImportRows[i].ImpValDisplay;
							break;
						case "HmPhone":
							_patient.HmPhone=_listImportRows[i].ImpValDisplay;
							break;
						#endregion
					}
				}
				//Insurance importing happens before updating the patient information because there is a possibility of returning for more information.
				if(_hasRequiredInsFields) {//Do not attempt to import any insurance unless they have the required fields for importing.
					#region Insurance importing
					bool isPrimaryImported=false;
					if(isImportingPriIns) {//A primary insurance field was flagged for importing.
						if(!ValidateAndImportInsurance(true)) {
							//Field missing or user chose to back out to correct information.
							return;//Nothing has been updated so it's okay to just return here.
						}
						isPrimaryImported=true;
					}
					if(isImportingSecIns) {//A secondary insurance field was flagged for importing.
						if(!ValidateAndImportInsurance(false)) {
							//Field missing or user chose to back out to correct information.
							if(isPrimaryImported) {
								//Primary has been imported, we cannot return at this point.  Simply notify the user that secondary could not be imported correctly.
								MsgBox.Show(this,"Primary insurance was imported successfully but secondary was unable to import.");
							}
							else {//Secondary had problems importing or the user chose to back out and correct information.
								return;//Nothing has been updated so it's okay to just return here.
							}
						}
					}
					#endregion
				}
				else {//Sheet does not contain the required ins fields.
					if(isImportingPriIns) {//The user has manually flagged a primary ins row for importing.
						MsgBox.Show(this,"Required primary insurance fields are missing on this sheet.  You cannot import primary insurance with this sheet until it contains all of required fields.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone.");
					}
					if(isImportingSecIns) {//The user has manually flagged a secondary ins row for importing.
						MsgBox.Show(this,"Required secondary insurance fields are missing on this sheet.  You cannot import secondary insurance with this sheet until it contains all of required fields.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone.");
					}
				}
				if(PatientL.CanVerifyPatientAddressInteraction()) {
					//If the feature is enabled, and the customer is on support, verify addresses with USPS.
					//Here is a good place to do address verification and correction. Other validation has taken place and we havent saved addresses yet.
					OpenDentBusiness.Address address = new OpenDentBusiness.Address();
					address.Address1=_patient.Address;
					address.Address2=_patient.Address2;
					address.City=_patient.City;
					address.State=_patient.State.ToUpper();
					address.Zip= _patient.Zip;
					Address addressVerified=PatientL.VerifyPatientAddressInteraction(address,this);
					if(addressVerified==null) {
						return;
					}
					_patient.Address=addressVerified.Address1;
					_patient.Address2=addressVerified.Address2;
					_patient.City=addressVerified.City;
					_patient.State=addressVerified.State;
					_patient.Zip=addressVerified.Zip;
				}
				//Patient information updating---------------------------------------------------------------------------------------------------------
				Patients.Update(_patient,_patientOld);
				if(_isAddressSameForFam) {
					bool isAuthArchivedEdit=Security.IsAuthorized(EnumPermType.ArchivedPatientEdit,true);
					if(!isAuthArchivedEdit && _family.HasArchivedMember()) {
						MessageBox.Show(Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(EnumPermType.ArchivedPatientEdit)+"\r\n"
							+Lans.g(this,"Archived patients in the family will not be updated.  All other family members will be updated as usual."));
					}
					Patients.UpdateAddressForFam(_patient,false,isAuthArchivedEdit);
				}
				//Import the refattaches
				for(int i=0;i<listRefAttaches.Count;i++) {
					RefAttaches.Insert(listRefAttaches[i]);//no security to block this action.
					SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,_patient.PatNum,"Referred From "+Referrals.GetNameFL(listRefAttaches[i].ReferralNum));
				}
			}
			#endregion
			#region Medical History (Sheets)
			if(SheetCur!=null && SheetCur.SheetType==SheetTypeEnum.MedicalHistory){
				for(int i=0;i<_listImportRows.Count;i++) {
					if(!_listImportRows[i].DoImport) {
						continue;
					}
					if(_listImportRows[i].TypeObj==null) {//Should never happen.
						continue;
					}
					//The reason it was written like this was to catch all allergies, meds, and diseases.
					//All of those would have a Y or N in the import column. 
					YN hasValue=YN.Unknown;
					if(_listImportRows[i].ImpValDisplay=="Y") {
						hasValue=YN.Yes;
					}
					if(_listImportRows[i].ImpValDisplay=="N") {
						hasValue=YN.No;
					}
					if(hasValue==YN.Unknown) {//Unknown, nothing to do.
						continue;
					}
					#region Allergies (sheets)
					if(_listImportRows[i].TypeObj==typeof(Allergy)) {
						//Patient has this allergy in the db so just update the value.
						if(_listImportRows[i].OldValObj!=null) {
							Allergy allergyOld=(Allergy)_listImportRows[i].OldValObj;
							if(hasValue==YN.Yes) {
								allergyOld.StatusIsActive=true;
							}
							else {
								allergyOld.StatusIsActive=false;
							}
							Allergies.Update(allergyOld);
							continue;
						}
						if(hasValue==YN.No) {//We never import allergies with inactive status.
							continue;
						}
						//Allergy does not exist for this patient yet so create one.
						List<AllergyDef> listAllergyDefs=AllergyDefs.GetAll(false);
						SheetField sheetFieldAllergy=(SheetField)_listImportRows[i].NewValObj;
						//Find what allergy user wants to import.
						for(int j=0;j<listAllergyDefs.Count;j++) {
							if(listAllergyDefs[j].Description==sheetFieldAllergy.FieldName.Remove(0,8)) {
								Allergy allergyNew=new Allergy();
								allergyNew.AllergyDefNum=listAllergyDefs[j].AllergyDefNum;
								allergyNew.PatNum=_patient.PatNum;
								allergyNew.StatusIsActive=true;
								Allergies.Insert(allergyNew);
								break;
							}
						}
					}
					#endregion Allergies (sheets)
					#region Medications (sheets)
					else if(_listImportRows[i].TypeObj==typeof(MedicationPat)) {
						//Patient has this medication in the db so leave it alone or set the stop date.
						if(_listImportRows[i].OldValObj!=null) {
						//Set the stop date for the current medication(s).
							MedicationPat medicationPatOld=(MedicationPat)_listImportRows[i].OldValObj;
							if(hasValue==YN.Yes) {
								if(!MedicationPats.IsMedActive(medicationPatOld)) {
									medicationPatOld.DateStop=new DateTime(0001,1,1);//This will activate the med.
								}
							}
							else {
								medicationPatOld.DateStop=DateTime.Today;//Set the med as inactive.
							}
							MedicationPats.Update(medicationPatOld);
							continue;
						}
						if(hasValue==YN.No) {//Don't import medications with inactive status.
							continue;
						}
						//Medication does not exist for this patient yet so create it.
						List<Medication> listMedications=Medications.GetList("");
						SheetField sheetFieldMed=(SheetField)_listImportRows[i].NewValObj;
						//Find what medication user wants to import.
						for(int j=0;j<listMedications.Count;j++) {
							if(Medications.GetDescription(listMedications[j].MedicationNum)==sheetFieldMed.FieldValue) {
								MedicationPat medicationPat=new MedicationPat();
								medicationPat.PatNum=_patient.PatNum;
								medicationPat.MedicationNum=listMedications[j].MedicationNum;
								MedicationPats.Insert(medicationPat);
								break;
							}
						}
					}
					#endregion Medications (sheets)
					#region Diseases (sheets)
					else if(_listImportRows[i].TypeObj==typeof(Disease)) {
						//Patient has this problem in the db so just update the value.
						if(_listImportRows[i].OldValObj!=null) {
							Disease diseaseOld=(Disease)_listImportRows[i].OldValObj;
							if(hasValue==YN.Yes) {
								diseaseOld.ProbStatus=ProblemStatus.Active;
							}
							else {
								diseaseOld.ProbStatus=ProblemStatus.Inactive;
							}
							Diseases.Update(diseaseOld);
							continue;
						}
						if(hasValue==YN.No) {//Don't create new problem with inactive status.
							continue;
						}
						//Problem does not exist for this patient yet so create one.
						SheetField sheetFieldDisease=(SheetField)_listImportRows[i].NewValObj;
						List<DiseaseDef> listDiseaseDefs=DiseaseDefs.GetDeepCopy(true);
						//Find what allergy user wants to import.
						for(int j=0;j<listDiseaseDefs.Count;j++) {
							if(listDiseaseDefs[j].DiseaseName==sheetFieldDisease.FieldName.Remove(0,8)) {
								Disease diseaseNew=new Disease();
								diseaseNew.PatNum=_patient.PatNum;
								diseaseNew.DiseaseDefNum=listDiseaseDefs[j].DiseaseDefNum;
								diseaseNew.ProbStatus=ProblemStatus.Active;
								Diseases.Insert(diseaseNew);
								break;
							}
						}
					}
					#endregion Diseases (sheets)
				}
			}
			#endregion Medical History (Sheets)
			#region Allergies (eForms)
			if(EFormCur!=null && _hasSectionAllergies){
				List<AllergyDef> listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);//from db. AllergyDefs are oddly not cached.
				List<Allergy> listAllergiesPat=Allergies.GetAll(_patient.PatNum,showInactive:false);//just active allergies
				for(int i=0;i<_listImportRows.Count;i++) {
					if(_listImportRows[i].TypeObj!=typeof(Allergy)){
						continue;
					}
					if(!_listImportRows[i].DoImport) {
						continue;
					}
					if(_listImportRows[i].DoRemove){
						Allergy allergy=_listImportRows[i].OldValObj as Allergy;
						allergy.StatusIsActive=false;
						Allergies.Update(allergy);
						continue;
					}
					if(_listImportRows[i].DoAdd){
						//The allergy could be here from checking a box or typing it in.
						AllergyDef allergyDef=listAllergyDefs.Find(x=>x.Description.ToLower()==_listImportRows[i].NewValDisplay.ToLower());
						if(allergyDef==null){
							//This allergy that they typed in does not have any matching allergyDef, so we will use "Other".
							//This won't happen for checkboxes, because we already added a def.
							AllergyDef allergyDefOther=listAllergyDefs.Find(x=>x.Description=="Other");
							if(allergyDefOther==null){//this will only happen once, and then all patients will reuse it for years.
								allergyDefOther=new AllergyDef();
								allergyDefOther.Description="Other";
								AllergyDefs.Insert(allergyDefOther);
								listAllergyDefs=AllergyDefs.GetAll(includeHidden:false);
								//DataValid.SetInvalid would be used instead if this was cached.
							}
							//allergyDefOther is now guaranteed to be valid
							Allergy allergyOther=listAllergiesPat.Find(x=>x.AllergyDefNum==allergyDefOther.AllergyDefNum && x.Reaction==_listImportRows[i].NewValDisplay);
							if(allergyOther==null){
								//This patient does not have an active "Other" allergy representing the allergy that was typed in
								allergyOther=new Allergy();
								allergyOther.PatNum=_patient.PatNum;
								allergyOther.AllergyDefNum=allergyDefOther.AllergyDefNum;
								allergyOther.Reaction=_listImportRows[i].NewValDisplay;
								allergyOther.StatusIsActive=true;
								Allergies.Insert(allergyOther);
								continue;
							}
							//This patient already has an active "Other" allergy representing the allergy that was typed in
							//so db will not need to change.
							continue;
						}//allergyDef==null
						//adding an allergy with known defNum
						Allergy allergy=new Allergy();
						allergy.PatNum=_patient.PatNum;
						allergy.AllergyDefNum=allergyDef.AllergyDefNum;
						allergy.StatusIsActive=true;
						Allergies.Insert(allergy);
						continue;
					}
				}
			}
			#endregion Allergies (eForms)
			#region Meds (eForms)
			if(EFormCur!=null && _hasSectionMeds){
				List<MedicationPat> listMedicationPats=MedicationPats.Refresh(_patient.PatNum,includeDiscontinued:false);
				for(int i=0;i<_listImportRows.Count;i++) {
					if(_listImportRows[i].TypeObj!=typeof(MedicationPat)){
						continue;
					}
					if(!_listImportRows[i].DoImport) {
						continue;
					}
					if(_listImportRows[i].MedDoImportCol2 && !_listImportRows[i].DoAdd){
						MedicationPat medicationPat=_listImportRows[i].OldValObj as MedicationPat;
						medicationPat.PatNote=_listImportRows[i].MedStrengthFreq;
						MedicationPats.Update(medicationPat);
						continue;
					}
					if(_listImportRows[i].DoRemove){
						MedicationPat medicationPat=_listImportRows[i].OldValObj as MedicationPat;
						medicationPat.DateStop=DateTime.Today.AddDays(-1);
						MedicationPats.Update(medicationPat);
						continue;
					}
					if(_listImportRows[i].DoAdd){
						MedicationPat medicationPat=new MedicationPat();
						medicationPat.PatNum=_patient.PatNum;
						Medication medication=Medications.GetFirstOrDefault(x=>x.MedName.ToLower()==_listImportRows[i].MedName.ToLower());
						if(medication is null){
							//There is no existing med (def) in the db with that name, so use the weaker unlinked strategy
							medicationPat.MedDescript=_listImportRows[i].MedName;
						}
						else{
							medicationPat.MedicationNum=medication.MedicationNum;
						}
						if(_listImportRows[i].MedDoImportCol2){
							medicationPat.PatNote=_listImportRows[i].MedStrengthFreq;
						}
						MedicationPats.Insert(medicationPat);
					}
				}
			}
			#endregion Meds (eForms)
			#region Problems (eForms)
			if(EFormCur!=null && _hasSectionProblems){
				List<Disease> listDiseasesPat=Diseases.Refresh(_patient.PatNum,showActiveOnly:true);
				for(int i=0;i<_listImportRows.Count;i++) {
					if(_listImportRows[i].TypeObj!=typeof(Disease)){
						continue;
					}
					if(!_listImportRows[i].DoImport) {
						continue;
					}
					if(_listImportRows[i].DoRemove){
						Disease disease=_listImportRows[i].OldValObj as Disease;
						disease.ProbStatus=ProblemStatus.Inactive;
						disease.DateStop=DateTime.Today.AddDays(-1);//yesterday
						Diseases.Update(disease);
						continue;
					}
					if(_listImportRows[i].DoAdd){
						//The disease could be here from checking a box or typing it in.
						DiseaseDef diseaseDef=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName.ToLower()==_listImportRows[i].NewValDisplay.ToLower());
						if(diseaseDef==null){
							//This disease that they typed in does not have any matching diseaseDef, so we will use "Other".
							//This won't happen for checkboxes, because we already added a def.
							DiseaseDef diseaseDefOther=DiseaseDefs.GetFirstOrDefault(x=>x.DiseaseName=="Other");
							if(diseaseDefOther==null){//this will only happen once, and then all patients will reuse it for years.
								diseaseDefOther=new DiseaseDef();
								diseaseDefOther.DiseaseName="Other";
								DiseaseDefs.Insert(diseaseDefOther);
								DataValid.SetInvalid(InvalidType.Diseases);
							}
							//diseaseDefOther is now guaranteed to be valid
							Disease diseaseOther=listDiseasesPat.Find(x=>x.DiseaseDefNum==diseaseDefOther.DiseaseDefNum && x.PatNote==_listImportRows[i].NewValDisplay);
							if(diseaseOther==null){
								//This patient does not have an active "Other" disease representing the disease that was typed in
								diseaseOther=new Disease();
								diseaseOther.PatNum=_patient.PatNum;
								diseaseOther.DiseaseDefNum=diseaseDefOther.DiseaseDefNum;
								diseaseOther.PatNote=_listImportRows[i].NewValDisplay;
								diseaseOther.ProbStatus=ProblemStatus.Active;
								Diseases.Insert(diseaseOther);
								continue;
							}
							//This patient already has an active "Other" disease representing the disease that was typed in
							//so db will not need to change.
							continue;
						}//diseaseDef==null
						//adding an disease with known defNum
						Disease disease=new Disease();
						disease.PatNum=_patient.PatNum;
						disease.DiseaseDefNum=diseaseDef.DiseaseDefNum;
						disease.ProbStatus=ProblemStatus.Active;
						Diseases.Insert(disease);
						continue;
					}
				}
			}
			#endregion Problems (eForms)
			#region ICE
			List<ImportRow> listSheetImportRowsFieldsIce = _listImportRows.FindAll(x => x.FieldName.StartsWith("ICE") && x.DoImport);
			if(listSheetImportRowsFieldsIce.Count>0) {
				for(int i=0;i<listSheetImportRowsFieldsIce.Count;i++){
					switch(listSheetImportRowsFieldsIce[i].FieldName) {
						case "ICEName":
							_patientNote.ICEName=listSheetImportRowsFieldsIce[i].ImpValDisplay;
							break;
						case "ICEPhone":
							_patientNote.ICEPhone=listSheetImportRowsFieldsIce[i].ImpValDisplay;
							break;
						default:
							break;
					}
				}
				PatientNotes.Update(_patientNote,_patient.Guarantor);
			}
			#endregion
			string logText=SecurityLogHelper();
			if(logText!="") {
				SecurityLogs.MakeLogEntry(EnumPermType.SheetEdit,_patient.PatNum,logText,0,LogSources.None,DateTime.Now);
			}
			MsgBox.Show(this,"Done.");
			DialogResult=DialogResult.OK;
		}

		/// <summary> Loads the Image and sets it in the appropriate PictureBox in the ui if able, and parses stored OcrInsScanResponse. If image fails to load, returned value will be an empty OcrInsScanResponse.</summary>
		private OcrInsScanResponse LoadOcrDataFromDocHelper(Document doc) {
			Bitmap bitmap=null;
			try {
				bitmap=ImageHelper.GetBitmapOfDocumentFromDb(doc.DocNum);
			}
			catch(Exception e) {
				Logger.WriteException(new Exception("FormSheetImport - Could not find Image for document with docNum: "+doc.DocNum, innerException:e), "EClipboard");
			}
			OcrInsScanResponse response=null;
			ODPictureBox pictureBox;
			switch(doc.ImageCaptureType) {
				case EnumOcrCaptureType.PrimaryInsFront:
					pictureBox=pictureBoxPrimaryInsuranceFront;
					break;
				case EnumOcrCaptureType.PrimaryInsBack:
					pictureBox=pictureBoxPrimaryInsuranceBack;
					break;
				case EnumOcrCaptureType.SecondaryInsFront:
					pictureBox=pictureBoxSecondaryInsuranceFront;
					break;
				case EnumOcrCaptureType.SecondaryInsBack:
					pictureBox=pictureBoxSecondaryInsuranceBack;
					break;
				default:
					return CreateBlankOcrInsScanResponse();
			}
			if(!doc.OcrResponseData.IsNullOrEmpty()){
				try {
					response=JsonConvert.DeserializeObject<OcrInsScanResponse>(doc.OcrResponseData);
				}
				catch(Exception e) { 
					Logger.WriteException(new Exception("FormSheetImport - Could not de-serialize OcrInsScanResponse from docNum: "+doc.DocNum, innerException:e), "EClipboard");	
				}
			}
			if(bitmap==null||response==null) {
				//If the image was not loaded, or the OcrInsScanResponse was not loaded, create and return a blank OcrInsScanResposne.
				response=CreateBlankOcrInsScanResponse();
			}
			else {
				pictureBox.Image= bitmap;
			}
			return response;
		}

		///<summary>Returns a string that will be empty if nothing is imported, or if there are no edited fields. Compares 'Current Value' and 'Import Value'
		///columns to determine if there is a change. If changes are being imported will return a security log string including the changes.</summary>
		private string SecurityLogHelper() {
			StringBuilder stringBuilder=new StringBuilder("");
			stringBuilder.Append($"{Security.CurUser.UserName} imported {Patients.GetNameFL(_patient.PatNum)}. The following fields were changed: \r\n");
			int indexDoImport=gridMain.Columns.GetIndex("Do Import");//Denotes an imported field with 'X'.
			int indexFieldName=gridMain.Columns.GetIndex("FieldName");
			int indexCurrentValue=gridMain.Columns.GetIndex("Current Value");
			int indexImportValue=gridMain.Columns.GetIndex("Import Value");
			//Grabbing all of the GridRows marked for import, denoted with an 'X'.
			List<GridRow> listGridRowsEdited=gridMain.ListGridRows.FindAll(x=>x.Cells[indexDoImport].Text=="X");
			//Returns all of the imported GridRows where the Import Value != Current Value.
			listGridRowsEdited=listGridRowsEdited.FindAll(x=>x.Cells[indexImportValue].Text!=x.Cells[indexCurrentValue].Text);
			if(listGridRowsEdited.IsNullOrEmpty()) {
				return "";
			}
			for(int i = 0;i<listGridRowsEdited.Count;i++) {
				stringBuilder.Append($"{listGridRowsEdited[i].Cells[indexFieldName].Text} changed from '{listGridRowsEdited[i].Cells[indexCurrentValue].Text}'");
				stringBuilder.Append($" to '{listGridRowsEdited[i].Cells[indexImportValue].Text}'\r\n");
			}
			return stringBuilder.ToString();
		}

		private OcrInsScanResponse CreateBlankOcrInsScanResponse() {
			OcrInsScanResponse result= new OcrInsScanResponse();
			result.Member=new Member();
			result.Dependents=new Dependent[0];
			result.IdNumber=new IdNumber();
			result.PrescriptionInfo=new PrescriptionInfo();
			result.Copays=new Copay[0];
			result.Payer=new Payer();
			result.Plan=new Plan();
			return result;
		}

		private bool DoImport(string fieldName) {
			for(int i=0;i<_listImportRows.Count;i++) {
				if(_listImportRows[i].FieldName!=fieldName) {
					continue;
				}
				return _listImportRows[i].DoImport;
			}
			return false;
		}

		///<summary>Will return null if field not found or if field marked to not import.</summary>
		private object GetImpObj(string fieldName) {
			for(int i=0;i<_listImportRows.Count;i++) {
				if(_listImportRows[i].FieldName!=fieldName) {
					continue;
				}
				if(!_listImportRows[i].DoImport) {
					return null;
				}
				return _listImportRows[i].ImpValObj;
			}
			return null;
		}

		///<summary>Will return empty string field not found or if field marked to not import.</summary>
		private string GetImpDisplay(string fieldName) {
			for(int i=0;i<_listImportRows.Count;i++) {
				if(_listImportRows[i].FieldName!=fieldName) {
					continue;
				}
				if(!_listImportRows[i].DoImport) {
					return "";
				}
				return _listImportRows[i].ImpValDisplay;
			}
			return "";
		}

		///<summary>Returns a separator and sets the FieldName to the passed in string.</summary>
		private ImportRow CreateSeparator(string displayText) {
			ImportRow importRow=new ImportRow();
			importRow.FieldName=displayText;
			importRow.IsSectionHeader=true;
			return importRow;
		}

		private class ImportRow {
			///<summary>This is the SheetField.FieldName which usually shows in the first column unless overridden by FieldDisplay</summary>
			public string FieldName;
			///<summary>Overrides FieldName in first column. If null, use FieldName;</summary>
			public string FieldDisplay;
			///<summary>Displays in the Old Val column. Can be any altered string representation.</summary>
			public string OldValDisplay;
			///<summary>Frequently the same as OldValDisplay, but since it's an object, it can be an actual enum or some other object entirely, like an allergy.</summary>
			public object OldValObj;
			///<summary>Displays in the New Val column. Can be any altered string representation.</summary>
			public string NewValDisplay;
			///<summary>This is what the user entered. Frequently the same as NewValDisplay, but since it's an object, it can be an actual enum or some other object entirely, like an allergy.</summary>
			public object NewValObj;
			///<summary>Displays in the Imp Val column. Can be any altered string representation.</summary>
			public string ImpValDisplay;
			///<summary>This is what will be imported. Frequently the same as ImpValDisplay, but since it's an object, it can be an actual enum or some other object entirely, like an allergy.</summary>
			public object ImpValObj;
			///<summary>True if we will import. Shows as X in last column.</summary>
			public bool DoImport;
			///<summary>A section header is darker and does not represent any import fields, but is instead the title for a group of them.</summary>
			public bool IsSectionHeader;
			///<summary>This can apply to Old, New, or Imp. Frequently some of those will be null and an object will only be present in other(s).</summary>
			public Type TypeObj;
			///<summary>Some fields are not importable, but they still need to be made obvious to user by coloring the user-entered value red.</summary>
			public bool IsNewValRed;
			///<summary>The import cell is shown with colored text to prompt user to notice.</summary>
			public bool IsImportRed;
			//allergies, meds, probs==============================================
			///<summary>This will trigger removal of an allergy, med, or disease from a patient by setting it inactive.</summary>
			public bool DoRemove;
			///<summary>This will trigger addition of an allergy, med, or disease to a patient.</summary>
			public bool DoAdd;
			///<summary>We need this for meds when we import. This is not what the patient entered or what we show to the user. Instead, it's the new value that should go in the db. So it's the original plus the appended.</summary>
			public string MedStrengthFreq;
			///<summary>We need this for meds when we import.</summary>
			public string MedName;
			///<summary>For now, true if they have one of the 4 import options set to true in setup. We might try to give them more control at import time.</summary>
			public bool MedDoImportCol2;
		}

	}
}