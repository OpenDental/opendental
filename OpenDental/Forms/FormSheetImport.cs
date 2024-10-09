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
	public partial class FormSheetImport:FormODBase {
		public Sheet SheetCur;
		public Document DocCur;
		private List<SheetImportRow> _listSheetImportRows;
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
				_patientOld=_patient.Copy();
				_patientNote=PatientNotes.Refresh(_patient.PatNum,_patient.Guarantor);
				_patientNoteOld=_patientNote.Copy();
				if(SheetCur.SheetFields!=null) {
					_isPatTransferSheet=SheetCur.SheetFields.Any(x => x.FieldName=="isTransfer" && PIn.Bool(x.FieldValue));
				}
			}
			else {
				#region Acro
				throw new NotImplementedException();//js this broke with the move to dot net 4.0.
				/*
				pat=Patients.GetPat(DocCur.PatNum);
				CAcroApp acroApp=null;
				try {
					acroApp=new AcroAppClass();//Initialize Acrobat by creating App object
				}
				catch {
					MsgBox.Show(this,"Requires Acrobat 9 Pro to be installed on this computer.");
					DialogResult=DialogResult.Cancel;
					return;
				}
				//acroApp.Show();// Show Acrobat Viewer
				//acroApp.Hide();//This is annoying if Acrobat is already open for some other reason.
				CAcroAVDoc avDoc=new AcroAVDocClass();
				string pathToPdf=CodeBase.ODFileUtils.CombinePaths(ImageStore.GetPatientFolder(pat),DocCur.FileName);
				if(!avDoc.Open(pathToPdf,"")){
					MessageBox.Show(Lan.g(this,"Could not open")+" "+pathToPdf);
					DialogResult=DialogResult.Cancel;
					return;
				}
				IAFormApp formApp=new AFormAppClass();//Create a IAFormApp object so we can access the form fields in the open document
				IFields myFields=(IFields)formApp.Fields;// Get the IFields object associated with the form
				IEnumerator myEnumerator = myFields.GetEnumerator();// Get the IEnumerator object for myFields
				dictAcrobatFields=new Dictionary<string,string>();
				IField myField;
				string nameClean;
				string valClean;
				while(myEnumerator.MoveNext()) {
					myField=(IField)myEnumerator.Current;// Get the IField object
					if(myField.Value==null){
						continue;
					}
					//if the form was designed in LiveCycle, the names will look like this: topmostSubform[0].page1[0].SSN[0]
					//Whereas, if it was designed in Acrobat, the names will look like this: SSN
					//So...
					nameClean=myField.Name;
					if(nameClean.Contains("[") && nameClean.Contains(".")) {
						nameClean=nameClean.Substring(nameClean.LastIndexOf(".")+1);
						nameClean=nameClean.Substring(0,nameClean.IndexOf("["));
					}
					if(nameClean=="misc") {
						int suffix=1;
						nameClean=nameClean+suffix.ToString();
						while(dictAcrobatFields.ContainsKey(nameClean)) {//untested.
							suffix++;
							nameClean=nameClean+suffix.ToString();
						}
					}
					valClean=myField.Value;
					if(valClean=="Off") {
						valClean="";
					}
					//myField.Type//possible values include text,radiobutton,checkbox
					//MessageBox.Show("Raw:"+myField.Name+"  Name:"+nameClean+"  Value:"+myField.Value);
					if(dictAcrobatFields.ContainsKey(nameClean)) {
						continue;
					}
					dictAcrobatFields.Add(nameClean,valClean);
					//name:topmostSubform[0].page1[0].SSN[0]
				}
				//acroApp.Hide();//Doesn't work well enough
				//this.BringToFront();//Doesn't work
				//acroApp.Minimize();
				acroApp.Exit();
				acroApp=null;
				*/
				#endregion
			}
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
			if(SheetCur.SheetType==SheetTypeEnum.PatientForm) {
				SetHasRequiredInsFields();
			}
		}

		///<summary>This can only be run once when the form first opens.  After that, the rows are just edited.</summary>
		private void FillRows() {
			#region Patient Form
			if(SheetCur.SheetType==SheetTypeEnum.PatientForm) {
				_listSheetImportRows=new List<SheetImportRow>();
				SheetImportRow row;
				string fieldVal;
				_listSheetImportRows.Add(CreateSeparator("Personal"));
				#region personal
				//LName---------------------------------------------
				fieldVal=GetInputValue("LName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="LName";
					row.OldValDisplay=_patient.LName;
					row.OldValObj=_patient.LName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//FName---------------------------------------------
				fieldVal=GetInputValue("FName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="FName";
					row.OldValDisplay=_patient.FName;
					row.OldValObj=_patient.FName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//MiddleI---------------------------------------------
				fieldVal=GetInputValue("MiddleI");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="MiddleI";
					row.OldValDisplay=_patient.MiddleI;
					row.OldValObj=_patient.MiddleI;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Preferred---------------------------------------------
				fieldVal=GetInputValue("Preferred");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Preferred";
					row.OldValDisplay=_patient.Preferred;
					row.OldValObj=_patient.Preferred;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Gender---------------------------------------------
				fieldVal=GetRadioValue("Gender");
				if(fieldVal!=null) {//field exists on form
					row=new SheetImportRow();
					row.FieldName="Gender";
					row.OldValDisplay=Lan.g("enumPatientGender",_patient.Gender.ToString());
					row.OldValObj=_patient.Gender;
					if(fieldVal=="") {//no box was checked
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							PatientGender patientGender=(PatientGender)Enum.Parse(typeof(PatientGender),fieldVal);
							row.NewValDisplay=Lan.g("enumPatientGender",patientGender.ToString());
							row.NewValObj=patientGender;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid gender."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(PatientGender);
					if(row.NewValObj!=null && (PatientGender)row.NewValObj!=_patient.Gender) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Position---------------------------------------------
				fieldVal=GetRadioValue("Position");
				if(fieldVal!=null) {//field exists on form
					row=new SheetImportRow();
					row.FieldName="Position";
					row.OldValDisplay=Lan.g("enumPatientPositionr",_patient.Position.ToString());
					row.OldValObj=_patient.Position;
					if(fieldVal=="") {//no box was checked
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							PatientPosition patientPosition=(PatientPosition)Enum.Parse(typeof(PatientPosition),fieldVal);
							row.NewValDisplay=Lan.g("enumPatientPosition",patientPosition.ToString());
							row.NewValObj=patientPosition;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid PatientPosition."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(PatientPosition);
					if(row.NewValObj!=null && (PatientPosition)row.NewValObj!=_patient.Position) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Birthdate---------------------------------------------
				fieldVal=GetInputValue("Birthdate");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Birthdate";
					if(_patient.Birthdate.Year<1880) {
						row.OldValDisplay="";
					}
					else {
						row.OldValDisplay=_patient.Birthdate.ToShortDateString();
					}
					row.OldValObj=_patient.Birthdate;
					row.NewValObj=SheetFields.GetBirthDate(fieldVal,SheetCur.IsWebForm,SheetCur.IsCemtTransfer);
					if(string.IsNullOrWhiteSpace(fieldVal)) {//Patient entered blank date, consider this to be valid blank date.
						row.NewValDisplay="";
						row.ImpValDisplay="";
					}
					else if(((DateTime)row.NewValObj).Year<1880) {//Patient entered date in incorrect format, consider this invalid (imports as MinValue)
						row.NewValDisplay=fieldVal;
						row.ImpValDisplay=INVALID_DATE;
						row.IsFlaggedImp=true;
					}
					else {
						row.NewValDisplay=((DateTime)row.NewValObj).ToShortDateString();//Correct formatting, valid date.
						row.ImpValDisplay=row.NewValDisplay;
					}
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(DateTime);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//SSN---------------------------------------------
				fieldVal=GetInputValue("SSN");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="SSN";
					row.OldValDisplay=_patient.SSN;
					row.OldValObj=_patient.SSN;
					row.NewValDisplay=fieldVal.Replace("-","");//quickly strip dashes
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//WkPhone---------------------------------------------
				fieldVal=GetInputValue("WkPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="WkPhone";
					row.OldValDisplay=_patient.WkPhone;
					row.OldValObj=_patient.WkPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//WirelessPhone---------------------------------------------
				fieldVal=GetInputValue("WirelessPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="WirelessPhone";
					row.OldValDisplay=_patient.WirelessPhone;
					row.OldValObj=_patient.WirelessPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//wirelessCarrier---------------------------------------------
				fieldVal=GetInputValue("wirelessCarrier");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="wirelessCarrier";
					row.OldValDisplay="";
					row.OldValObj="";
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					_listSheetImportRows.Add(row);
				}
				//Email---------------------------------------------
				fieldVal=GetInputValue("Email");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Email";
					row.OldValDisplay=_patient.Email;
					row.OldValObj=_patient.Email;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//PreferContactMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferContactMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferContactMethod");
				}
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="PreferContactMethod";
					row.OldValDisplay=Lan.g("enumContactMethod",_patient.PreferContactMethod.ToString());
					row.OldValObj=_patient.PreferContactMethod;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							ContactMethod contactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							row.NewValDisplay=Lan.g("enumContactMethod",contactMethod.ToString());
							row.NewValObj=contactMethod;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(ContactMethod);
					if(row.NewValObj!=null && (ContactMethod)row.NewValObj!=_patient.PreferContactMethod) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//PreferConfirmMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferConfirmMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferConfirmMethod");
				}
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="PreferConfirmMethod";
					row.OldValDisplay=Lan.g("enumContactMethod",_patient.PreferConfirmMethod.ToString());
					row.OldValObj=_patient.PreferConfirmMethod;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							ContactMethod contactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							row.NewValDisplay=Lan.g("enumContactMethod",contactMethod.ToString());
							row.NewValObj=contactMethod;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(ContactMethod);
					if(row.NewValObj!=null && (ContactMethod)row.NewValObj!=_patient.PreferConfirmMethod) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//PreferRecallMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferRecallMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferRecallMethod");
				}
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="PreferRecallMethod";
					row.OldValDisplay=Lan.g("enumContactMethod",_patient.PreferRecallMethod.ToString());
					row.OldValObj=_patient.PreferRecallMethod;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							ContactMethod contactMethod=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							row.NewValDisplay=Lan.g("enumContactMethod",contactMethod.ToString());
							row.NewValObj=contactMethod;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(ContactMethod);
					if(row.NewValObj!=null && (ContactMethod)row.NewValObj!=_patient.PreferRecallMethod) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//referredFrom---------------------------------------------
				fieldVal=GetInputValue("referredFrom");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="referredFrom";
					Referral referral=Referrals.GetReferralForPat(_patient.PatNum);
					if(referral==null) {//there was no existing referral
						row.OldValDisplay="";
						row.OldValObj=null;
						row.NewValDisplay=fieldVal;
						row.NewValObj=null;
						if(row.NewValDisplay!="") {//user did enter a referral
							row.ImpValDisplay=Lan.g(this,"[double click to pick]");
							row.ImpValObj=null;
							row.IsFlaggedImp=true;
							row.DoImport=false;//this will change to true after they pick a referral
						}
						else {//user still did not enter a referral
							row.ImpValDisplay="";
							row.ImpValObj=null;
							row.DoImport=false;
						}
					}
					else {//there was an existing referral. We don't allow changing from here since mostly for new patients.
						row.OldValDisplay=referral.GetNameFL();
						row.OldValObj=referral;
						row.NewValDisplay=fieldVal;
						row.NewValObj=null;
						row.ImpValDisplay="";
						row.ImpValObj=null;
						row.DoImport=false;
						if(row.OldValDisplay!=row.NewValDisplay) {//if patient changed an existing referral, at least let user know.
							row.IsFlagged=true;//although they won't be able to do anything about it here
						}
					}
					row.ObjType=typeof(Referral);
					_listSheetImportRows.Add(row);
				}
				//StudentStatus---------------------------------------------
				fieldVal=GetRadioValue("StudentStatus");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="StudentStatus";
					if(_patient.StudentStatus=="N") {
						row.OldValDisplay="Nonstudent";
					}
					else if(_patient.StudentStatus=="P") {
						row.OldValDisplay="Part-time";
					}
					else if(_patient.StudentStatus=="F") {
						row.OldValDisplay="Full-time";
					}
					row.OldValObj=_patient.StudentStatus;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					//fieldVals for StudentStatus are the full word for regular sheets.
					//Regular sheet example: "Fulltime".
					else if(fieldVal=="Nonstudent" || fieldVal=="N") {
						row.NewValDisplay="Nonstudent";
						row.NewValObj="N";
					}
					else if(fieldVal=="Parttime" || fieldVal=="P") {
						row.NewValDisplay="Part-time";
						row.NewValObj="P";
					}
					else if (fieldVal=="Fulltime" || fieldVal=="F") {
						row.NewValDisplay="Full-time";
						row.NewValObj="F";
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.NewValObj!=null && (string)row.NewValObj!=_patient.StudentStatus) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ICE Name---------------------------------------------
				fieldVal=GetInputValue("ICEName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEName";
					row.OldValDisplay=_patientNote.ICEName;
					row.OldValObj=_patientNote.ICEName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					_listSheetImportRows.Add(row);
				}
				//ICE Phone---------------------------------------------
				fieldVal=GetInputValue("ICEPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEPhone";
					row.OldValDisplay=_patientNote.ICEPhone;
					row.OldValObj=_patientNote.ICEPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					_listSheetImportRows.Add(row);
				}	
				#endregion personal
				//Separator-------------------------------------------
				_listSheetImportRows.Add(CreateSeparator("Address and Home Phone"));
				#region address
				//SameForEntireFamily-------------------------------------------
				if(ContainsOneOfFields("addressAndHmPhoneIsSameEntireFamily")) {
					row=new SheetImportRow();
					row.FieldName="addressAndHmPhoneIsSameEntireFamily";
					row.FieldDisplay="Same for entire family";
					if(_isAddressSameForFam) {//remember we calculated this in the form constructor.
						row.OldValDisplay="X";
						row.OldValObj="X";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					//And now, we will revise AddressSameForFam based on user input
					_isAddressSameForFam=IsChecked("addressAndHmPhoneIsSameEntireFamily");
					if(_isAddressSameForFam) {
						row.NewValDisplay="X";
						row.NewValObj="X";
						row.ImpValDisplay="X";
						row.ImpValObj="X";
					}
					else {
						row.NewValDisplay="";
						row.NewValObj="";
						row.ImpValDisplay="";
						row.ImpValObj="";
					}
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Address---------------------------------------------
				fieldVal=GetInputValue("Address");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Address";
					row.OldValDisplay=_patient.Address;
					row.OldValObj=_patient.Address;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Address2---------------------------------------------
				fieldVal=GetInputValue("Address2");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Address2";
					row.OldValDisplay=_patient.Address2;
					row.OldValObj=_patient.Address2;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//City---------------------------------------------
				fieldVal=GetInputValue("City");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="City";
					row.OldValDisplay=_patient.City;
					row.OldValObj=_patient.City;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//State---------------------------------------------
				fieldVal=GetInputValue("State");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="State";
					row.OldValDisplay=_patient.State;
					row.OldValObj=_patient.State;
					row.NewValDisplay=fieldVal;
					string pattern="^"//start of string
						+"[a-zA-Z][a-zA-Z]"//exactly two letters
						+"$";//end of string
					if(Regex.IsMatch(fieldVal,pattern)) {
						if(CultureInfo.CurrentCulture.Name.EndsWith("US") || CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
							row.NewValDisplay=fieldVal.ToUpper();
						}
					}
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//StateNoValidation---------------------------------
				fieldVal=GetInputValue("StateNoValidation");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="StateNoValidation";
					row.OldValDisplay=_patient.State;
					row.OldValObj=_patient.State;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//Zip---------------------------------------------
				fieldVal=GetInputValue("Zip");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Zip";
					row.OldValDisplay=_patient.Zip;
					row.OldValObj=_patient.Zip;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//HmPhone---------------------------------------------
				fieldVal=GetInputValue("HmPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="HmPhone";
					row.OldValDisplay=_patient.HmPhone;
					row.OldValObj=_patient.HmPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				#endregion address
				//Separator-------------------------------------------
				_listSheetImportRows.Add(CreateSeparator("Insurance Policy 1"));
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
					row=new SheetImportRow();
					row.FieldName="ins1Relat";
					row.FieldDisplay="Relationship";
					row.OldValDisplay=Lan.g("enumRelat",_relatIns1.ToString());
					row.OldValObj=_relatIns1;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							Relat relat=(Relat)Enum.Parse(typeof(Relat),fieldVal);
							row.NewValDisplay=Lan.g("enumRelat",relat.ToString());
							row.NewValObj=relat;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid Relationship."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(Relat);
					row.DoImport=false;
					if(row.NewValObj!=null && row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins1Subscriber---------------------------------------------
				fieldVal=GetInputValue("ins1SubscriberNameF");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1Subscriber";
					row.FieldDisplay="Subscriber";
					if(_insPlan1!=null) {
						row.OldValDisplay=_family.GetNameInFamFirst(_insSub1.Subscriber);
						row.OldValObj=_insSub1.Subscriber;
					}
					else {
						row.OldValDisplay="";
						row.OldValObj=null;
					}
					row.NewValDisplay=fieldVal;//whether it's empty or has a value
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay="[double click to pick]";
					row.ImpValObj=null;
					row.ObjType=typeof(Patient);
					row.DoImport=false;
					row.IsFlaggedImp=true;
					_listSheetImportRows.Add(row);
				}
				//ins1SubscriberID---------------------------------------------
				fieldVal=GetInputValue("ins1SubscriberID");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1SubscriberID";
					row.FieldDisplay="Subscriber ID";
					if(_insPlan1!=null) {
						row.OldValDisplay=_insSub1.SubscriberID;
						row.OldValObj="";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					row.NewValDisplay=fieldVal;//whether it's empty or has a value
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins1CarrierName---------------------------------------------
				fieldVal=GetInputValue("ins1CarrierName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1CarrierName";
					row.FieldDisplay="Carrier";
					if(_carrier1!=null) {
						row.OldValDisplay=_carrier1.CarrierName;
						row.OldValObj=_carrier1;
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					row.NewValDisplay=fieldVal;//whether it's empty or has a value
					row.NewValObj="";
					row.ImpValDisplay="[double click to pick]";
					row.ImpValObj=null;
					row.ObjType=typeof(Carrier);
					row.DoImport=false;
					row.IsFlaggedImp=true;
					_listSheetImportRows.Add(row);
				}
				//ins1CarrierPhone---------------------------------------------
				fieldVal=GetInputValue("ins1CarrierPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1CarrierPhone";
					row.FieldDisplay="Phone";
					if(_carrier1!=null) {
						row.OldValDisplay=_carrier1.Phone;
						row.OldValObj="";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);//whether it's empty or has a value
					row.NewValObj="";
					row.ImpValDisplay="[double click to pick]";
					row.ImpValObj=null;
					row.ObjType=typeof(Carrier);
					row.DoImport=false;
					row.IsFlaggedImp=true;
					_listSheetImportRows.Add(row);
				}
				//ins1EmployerName---------------------------------------------
				fieldVal=GetInputValue("ins1EmployerName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1EmployerName";
					row.FieldDisplay="Employer";
					if(_insPlan1==null) {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					else {
						row.OldValDisplay=Employers.GetName(_insPlan1.EmployerNum);
						row.OldValObj=Employers.GetEmployer(_insPlan1.EmployerNum);
					}
					row.NewValDisplay=fieldVal;
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins1GroupName---------------------------------------------
				fieldVal=GetInputValue("ins1GroupName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1GroupName";
					row.FieldDisplay="Group Name";
					if(_insPlan1!=null) {
						row.OldValDisplay=_insPlan1.GroupName;
					}
					else {
						row.OldValDisplay="";
					}
					row.OldValObj="";
					row.NewValDisplay=fieldVal;
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins1GroupNum---------------------------------------------
				fieldVal=GetInputValue("ins1GroupNum");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1GroupNum";
					row.FieldDisplay="Group Num";
					if(_insPlan1!=null) {
						row.OldValDisplay=_insPlan1.GroupNum;
					}
					else {
						row.OldValDisplay="";
					}
					row.OldValObj="";
					row.NewValDisplay=fieldVal;
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				#endregion ins1
				//Separator-------------------------------------------
				_listSheetImportRows.Add(CreateSeparator("Insurance Policy 2"));
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
					row=new SheetImportRow();
					row.FieldName="ins2Relat";
					row.FieldDisplay="Relationship";
					row.OldValDisplay=Lan.g("enumRelat",_relatIns2.ToString());
					row.OldValObj=_relatIns2;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							Relat relat=(Relat)Enum.Parse(typeof(Relat),fieldVal);
							row.NewValDisplay=Lan.g("enumRelat",relat.ToString());
							row.NewValObj=relat;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid Relationship."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(Relat);
					row.DoImport=false;
					if(row.NewValObj!=null && row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins2Subscriber---------------------------------------------
				fieldVal=GetInputValue("ins2SubscriberNameF");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2Subscriber";
					row.FieldDisplay="Subscriber";
					if(_insPlan2!=null) {
						row.OldValDisplay=_family.GetNameInFamFirst(_insSub2.Subscriber);
						row.OldValObj=_insSub2.Subscriber;
					}
					else {
						row.OldValDisplay="";
						row.OldValObj=null;
					}
					row.NewValDisplay=fieldVal;//whether it's empty or has a value
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay="[double click to pick]";
					row.ImpValObj=null;
					row.ObjType=typeof(Patient);
					row.DoImport=false;
					row.IsFlaggedImp=true;
					_listSheetImportRows.Add(row);
				}
				//ins2SubscriberID---------------------------------------------
				fieldVal=GetInputValue("ins2SubscriberID");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2SubscriberID";
					row.FieldDisplay="Subscriber ID";
					if(_insPlan2!=null) {
						row.OldValDisplay=_insSub2.SubscriberID;
						row.OldValObj="";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					row.NewValDisplay=fieldVal;//whether it's empty or has a value
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins2CarrierName---------------------------------------------
				fieldVal=GetInputValue("ins2CarrierName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2CarrierName";
					row.FieldDisplay="Carrier";
					if(_carrier2!=null) {
						row.OldValDisplay=_carrier2.CarrierName;
						row.OldValObj="";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					row.NewValDisplay=fieldVal;//whether it's empty or has a value
					row.NewValObj="";
					row.ImpValDisplay="[double click to pick]";
					row.ImpValObj=null;
					row.ObjType=typeof(Carrier);
					row.DoImport=false;
					row.IsFlaggedImp=true;
					_listSheetImportRows.Add(row);
				}
				//ins2CarrierPhone---------------------------------------------
				fieldVal=GetInputValue("ins2CarrierPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2CarrierPhone";
					row.FieldDisplay="Phone";
					if(_carrier2!=null) {
						row.OldValDisplay=_carrier2.Phone;
						row.OldValObj="";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);//whether it's empty or has a value
					row.NewValObj="";
					row.ImpValDisplay="[double click to pick]";
					row.ImpValObj=null;
					row.ObjType=typeof(Carrier);
					row.DoImport=false;
					row.IsFlaggedImp=true;
					_listSheetImportRows.Add(row);
				}
				//ins2EmployerName---------------------------------------------
				fieldVal=GetInputValue("ins2EmployerName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2EmployerName";
					row.FieldDisplay="Employer";
					if(_insPlan2==null) {
						row.OldValDisplay="";
					}
					else {
						row.OldValDisplay=Employers.GetName(_insPlan2.EmployerNum);
					}
					row.OldValObj="";
					row.NewValDisplay=fieldVal;
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins2GroupName---------------------------------------------
				fieldVal=GetInputValue("ins2GroupName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2GroupName";
					row.FieldDisplay="Group Name";
					if(_insPlan2!=null) {
						row.OldValDisplay=_insPlan2.GroupName;
					}
					else {
						row.OldValDisplay="";
					}
					row.OldValObj="";
					row.NewValDisplay=fieldVal;
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				//ins2GroupNum---------------------------------------------
				fieldVal=GetInputValue("ins2GroupNum");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2GroupNum";
					row.FieldDisplay="Group Num";
					if(_insPlan2!=null) {
						row.OldValDisplay=_insPlan2.GroupNum;
					}
					else {
						row.OldValDisplay="";
					}
					row.OldValObj="";
					row.NewValDisplay=fieldVal;
					row.NewValObj=fieldVal;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					row.DoImport=false;
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				#endregion ins2
				//jsalmon - It was deemed a bug by Nathan and Jordan 12/16/2014 to be showing Misc fields in the sheet import tool.
				////Separator-------------------------------------------
				//Rows.Add(CreateSeparator("Misc"));
				////misc----------------------------------------------------
				//List<string> miscVals=GetMiscValues();
				//for(int i=0;i<miscVals.Count;i++) {
				//	fieldVal=miscVals[i];
				//	row=new SheetImportRow();
				//	row.FieldName="misc";
				//	row.FieldDisplay="misc"+(i+1).ToString();
				//	row.OldValDisplay="";
				//	row.OldValObj="";
				//	row.NewValDisplay=fieldVal;
				//	row.NewValObj="";
				//	row.ImpValDisplay="";
				//	row.ImpValObj="";
				//	row.ObjType=typeof(string);
				//	row.DoImport=false;
				//	row.IsFlagged=true;
				//	Rows.Add(row);
				//}
			}
			#endregion
			#region Medical History
			else if(SheetCur.SheetType==SheetTypeEnum.MedicalHistory) {
				_listSheetImportRows=new List<SheetImportRow>();
				string fieldVal="";
				List<Allergy> listAllergies=null;
				List<Disease> listDiseases=null;
				SheetImportRow row;
				_listSheetImportRows.Add(CreateSeparator("Personal"));
				#region ICE
				//ICE Name---------------------------------------------
				fieldVal=GetInputValue("ICEName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEName";
					row.OldValDisplay=_patientNote.ICEName;
					row.OldValObj=_patientNote.ICEName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					_listSheetImportRows.Add(row);
				}
				//ICE Phone---------------------------------------------
				fieldVal=GetInputValue("ICEPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEPhone";
					row.OldValDisplay=_patientNote.ICEPhone;
					row.OldValObj=_patientNote.ICEPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					_listSheetImportRows.Add(row);
				}
				#endregion
				//Separator-------------------------------------------
				_listSheetImportRows.Add(CreateSeparator("Allergies"));
				#region Allergies
				//Get list of all the allergy check boxes
				List<SheetField> listSheetFieldsAllergy=GetSheetFieldsByFieldName("allergy:");
				for(int i=0;i<listSheetFieldsAllergy.Count;i++) {
					fieldVal="";
					if(i<1) {
						listAllergies=Allergies.GetAll(_patient.PatNum,true);
					}
					row=new SheetImportRow();
					row.FieldName=listSheetFieldsAllergy[i].FieldName.Remove(0,8);
					row.OldValDisplay="";
					row.OldValObj=null;
					//Check if allergy exists.
					for(int j=0;j<listAllergies.Count;j++) {
						if(AllergyDefs.GetDescription(listAllergies[j].AllergyDefNum)==listSheetFieldsAllergy[i].FieldName.Remove(0,8)) {
							if(listAllergies[j].StatusIsActive) {
								row.OldValDisplay="Y";
							}
							else {
								row.OldValDisplay="N";
							}
							row.OldValObj=listAllergies[j];
							break;
						}
					}
					SheetField sheetFieldOppositeBox=GetOppositeSheetFieldCheckBox(listSheetFieldsAllergy,listSheetFieldsAllergy[i]);
					if(listSheetFieldsAllergy[i].FieldValue=="") {//Current box not checked.
						if(sheetFieldOppositeBox==null || sheetFieldOppositeBox.FieldValue=="") {//No opposite box or both boxes are not checked.
							//Create a blank row just in case they want to import.
							row.NewValDisplay="";
							row.NewValObj=listSheetFieldsAllergy[i];
							row.ImpValDisplay="";
							row.ImpValObj="";
							row.ObjType=typeof(Allergy);
							_listSheetImportRows.Add(row);
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
					row.NewValDisplay=fieldVal;
					row.NewValObj=listSheetFieldsAllergy[i];
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=typeof(string);
					row.ObjType=typeof(Allergy);
					if(row.OldValDisplay!=row.NewValDisplay && !(row.OldValDisplay=="" && row.NewValDisplay=="N")) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				#endregion
				//Separator-------------------------------------------
				_listSheetImportRows.Add(CreateSeparator("Medications"));
				#region Medications
				List<SheetField> listSheetFieldsInputMed=GetSheetFieldsByFieldName("inputMed");
				List<SheetField> listSheetFieldsCheckMed=GetSheetFieldsByFieldName("checkMed");
				List<SheetField> listSheetFieldsCurrentMed=new List<SheetField>();
				List<SheetField> listSheetFieldsNewMed=new List<SheetField>();
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
					row=new SheetImportRow();
					row.FieldName=listSheetFieldsCurrentMed[i].FieldValue;//Will be the name of the drug.
					row.OldValDisplay="N";
					row.OldValObj=null;
					for(int j=0;j<listMedicationPatsFull.Count;j++) {
						string strMedName=listMedicationPatsFull[j].MedDescript;//for meds that came back from NewCrop
						if(listMedicationPatsFull[j].MedicationNum!=0) {//For meds entered in OD and linked to Medication list.
							strMedName=Medications.GetDescription(listMedicationPatsFull[j].MedicationNum);
						}
						if(listSheetFieldsCurrentMed[i].FieldValue==strMedName) {
							row.OldValDisplay="Y";
							row.OldValObj=listMedicationPatsFull[j];
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
							&& row.OldValObj!=null && row.OldValDisplay=="N" //Patient has this medication but is currently marked as inactive.
							&& listSheetFieldsRelatedChkBoxes[j].FieldValue=="") //Patient unchecked the medication so we activate it again.
						{
							fieldVal="Y";
						}
					}
					if(listSheetFieldsRelatedChkBoxes.Count==1 
						&& listSheetFieldsRelatedChkBoxes[0].RadioButtonValue=="N" 
						&& listSheetFieldsRelatedChkBoxes[0].FieldValue=="" 
						&& row.OldValDisplay=="N" 
						&& row.OldValObj!=null)
					{
						row.DoImport=true;
					}
					row.NewValDisplay=fieldVal;
					row.NewValObj=listSheetFieldsCurrentMed[i];
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=typeof(string);
					row.ObjType=typeof(MedicationPat);
					if(row.OldValDisplay!=row.NewValDisplay && row.NewValDisplay!="") {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
					#endregion
				}
				for(int i=0;i<listSheetFieldsNewMed.Count;i++) {
					#region medications the patient entered
					if(listSheetFieldsNewMed[i].FieldValue=="") {//No medication entered by patient.
						continue;
					}
					row=new SheetImportRow();
					row.FieldName=listSheetFieldsNewMed[i].FieldValue;//Whatever the patient typed in...
					row.OldValDisplay="";
					row.OldValObj=null;
					row.NewValDisplay="Y";
					row.NewValObj=listSheetFieldsNewMed[i];
					row.ImpValDisplay=Lan.g(this,"[double click to pick]");
					row.ImpValObj=new long();
					row.IsFlaggedImp=true;
					row.DoImport=false;//this will change to true after they pick a medication
					row.ObjType=typeof(MedicationPat);
					_listSheetImportRows.Add(row);
					#endregion
				}
				#endregion
				//Separator-------------------------------------------
				_listSheetImportRows.Add(CreateSeparator("Problems"));
				#region Problems
				List<SheetField> listSheetFieldsProblems=GetSheetFieldsByFieldName("problem:");
				for(int i=0;i<listSheetFieldsProblems.Count;i++) {
					fieldVal="";
					if(i<1) {
						listDiseases=Diseases.Refresh(_patient.PatNum,false);
					}
					row=new SheetImportRow();
					row.FieldName=listSheetFieldsProblems[i].FieldName.Remove(0,8);
					//Figure out the current status of this allergy
					row.OldValDisplay="";
					row.OldValObj=null;
					for(int j=0;j<listDiseases.Count;j++) {
						if(DiseaseDefs.GetName(listDiseases[j].DiseaseDefNum)==listSheetFieldsProblems[i].FieldName.Remove(0,8)) {
							if(listDiseases[j].ProbStatus==ProblemStatus.Active) {
								row.OldValDisplay="Y";
							}
							else {
								row.OldValDisplay="N";
							}
							row.OldValObj=listDiseases[j];
							break;
						}
					}
					SheetField sheetFieldOppositeBox=GetOppositeSheetFieldCheckBox(listSheetFieldsProblems,listSheetFieldsProblems[i]);
					if(listSheetFieldsProblems[i].FieldValue=="") {//Current box not checked.
						if(sheetFieldOppositeBox==null || sheetFieldOppositeBox.FieldValue=="") {//No opposite box or both boxes are not checked.
							//Create a blank row just in case they still want to import.
							row.NewValDisplay="";
							row.NewValObj=listSheetFieldsProblems[i];
							row.ImpValDisplay="";
							row.ImpValObj="";
							row.ObjType=typeof(Disease);
							_listSheetImportRows.Add(row);
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
					row.NewValDisplay=fieldVal;
					row.NewValObj=listSheetFieldsProblems[i];
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=typeof(string);
					row.ObjType=typeof(Disease);
					if(row.OldValDisplay!=row.NewValDisplay && !(row.OldValDisplay=="" && row.NewValDisplay=="N")) {
						row.DoImport=true;
					}
					_listSheetImportRows.Add(row);
				}
				#endregion
			}
			#endregion
		}

		private void FillGrid() {
			int scrollVal=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col; 
			col=new GridColumn(Lan.g(this,"FieldName"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Current Value"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Entered Value"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Import Value"),175);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Do Import"),60,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			for(int i=0;i<_listSheetImportRows.Count;i++) {
				row=new GridRow();
				if(_listSheetImportRows[i].IsSeparator) {
					row.Cells.Add(_listSheetImportRows[i].FieldName);
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.ColorBackG=Color.DarkSlateGray;
					row.ColorText=Color.White;
				}
				else {
					if(_listSheetImportRows[i].FieldDisplay!=null) {
						row.Cells.Add(_listSheetImportRows[i].FieldDisplay);
					}
					else {
						row.Cells.Add(_listSheetImportRows[i].FieldName);
					}
					row.Cells.Add(_listSheetImportRows[i].OldValDisplay);
					cell=new GridCell(_listSheetImportRows[i].NewValDisplay);
					if(_listSheetImportRows[i].IsFlagged) {
						cell.ColorText=Color.Firebrick;
						cell.Bold=YN.Yes;
					}
					row.Cells.Add(cell);
					cell=new GridCell(_listSheetImportRows[i].ImpValDisplay);
					if(_listSheetImportRows[i].IsFlaggedImp) {
						cell.ColorText=Color.Firebrick;
						cell.Bold=YN.Yes;
					}
					row.Cells.Add(cell);
					if(_listSheetImportRows[i].DoImport) {
						row.Cells.Add("X");
						row.ColorBackG=Color.FromArgb(225,225,225);
					}
					else {
						row.Cells.Add("");
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=scrollVal;
		}

		/// <summary> This tries to get the value from the sheet for the field name passed in. If it's an insurnace related field and it was not found on the sheet it pulls the value from Ocr data instead. If null is returned, the row for that field will not be added to the form. </summary>
		private string GetInputValue(string fieldName) {
			string result=GetSheetInputValue(fieldName);
			if(!fieldName.StartsWith("ins")) {
				return result;
			}
			if(result==null || result!="") {
				//If null was returned by GetSheetInputValue, preserve the null.
				//If a value was found on the sheet prefer the sheet value to ocrData.
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
			//If we return null, the field wont be added to the grid. We know it should be there because GetSheetInputValue didnt return null.
			if(result==null) {
				result="";
			}
			return result;
		}

		///<summary>If the specified fieldName does not exist, returns null</summary>
		private string GetSheetInputValue(string fieldName) {
			//Get ocr vals here Make a wrapper method to call this then an ocr version.
			return SheetCur?.SheetFields?.FirstOrDefault(x => x.FieldType==SheetFieldType.InputField && x.FieldName==fieldName)?.FieldValue;
		}

		///<summary>If no radiobox with that name exists, returns null.  If no box is checked, it returns empty string.</summary>
		private string GetRadioValue(string fieldName) {
			List<SheetField> listSheetFields=SheetCur?.SheetFields?.FindAll(x => x.FieldType==SheetFieldType.CheckBox && x.FieldName==fieldName);
			if(listSheetFields==null || listSheetFields.Count==0) {
				return null;
			}
			SheetField sheetField=listSheetFields.FirstOrDefault(x => x.FieldValue=="X");
			return sheetField==null ? "" : sheetField.RadioButtonValue;
		}

		///<summary>Only the true condition is tested.  If the specified fieldName does not exist, returns false.</summary>
		private bool IsChecked(string fieldName) {
			return SheetCur?.SheetFields?.FirstOrDefault(x => x.FieldType==SheetFieldType.CheckBox && x.FieldName==fieldName)?.FieldValue=="X";
		}

		///<summary>Returns the values of all the "misc" textbox fields on this form.</summary>
		private List<string> GetMiscValues() {
			if(SheetCur==null) {
				return new List<string>();
			}
			return SheetCur.SheetFields.FindAll(x => x.FieldType==SheetFieldType.InputField && x.FieldName=="misc").Select(x => x.FieldValue).ToList();
		}

		///<summary>Returns all the sheet fields with FieldNames that start with the passed in string.  Only works for check box, input and output fields for now.</summary>
		private List<SheetField> GetSheetFieldsByFieldName(string fieldName) {
			List<SheetField> listSheetFieldsReturnVal=new List<SheetField>();
			if(SheetCur==null) {
				return listSheetFieldsReturnVal;
			}
			for(int i=0;i<SheetCur.SheetFields.Count;i++) {
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.CheckBox
					&& SheetCur.SheetFields[i].FieldType!=SheetFieldType.InputField
					&& SheetCur.SheetFields[i].FieldType!=SheetFieldType.OutputText) 
				{
					continue;
				}
				if(!SheetCur.SheetFields[i].FieldName.StartsWith(fieldName)) {
					continue;
				}
				listSheetFieldsReturnVal.Add(SheetCur.SheetFields[i]);
			}
			return listSheetFieldsReturnVal;
		}
		
		///<summary>Returns one sheet field with the same FieldName. Returns null if not found.</summary>
		private SheetImportRow GetImportRowByFieldName(string fieldName) {
			if(_listSheetImportRows==null) {
				return null;
			}
			for(int i=0;i<_listSheetImportRows.Count;i++) {
				if(_listSheetImportRows[i].FieldName==fieldName){
					return _listSheetImportRows[i];
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

		private bool ContainsOneOfFields(params string[] fieldNames) {
			if(SheetCur==null) {
				return false;
			}
			return SheetCur.SheetFields.FindAll(x => x.FieldType==SheetFieldType.CheckBox || x.FieldType==SheetFieldType.InputField)
				.Any(x => fieldNames.Contains(x.FieldName));
		}

		private bool ContainsFieldThatStartsWith(string fieldName) {
			if(SheetCur==null) {
				return false;
			}
			return SheetCur.SheetFields.FindAll(x => x.FieldType==SheetFieldType.CheckBox || x.FieldType==SheetFieldType.InputField)
				.Any(x => x.FieldName.StartsWith(fieldName));
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col!=4) {
				return;
			}
			if(_listSheetImportRows[e.Row].IsSeparator) {
				return;
			}
			if(!IsImportable(_listSheetImportRows[e.Row])) {
				return;
			}
			_listSheetImportRows[e.Row].DoImport=!_listSheetImportRows[e.Row].DoImport;
			FillGrid();
		}

		///<summary>Mostly the same as IsImportable.  But subtle differences.</summary>
		private bool IsEditable(SheetImportRow row) {
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
			//if(row.FieldName.StartsWith("ins1") || row.FieldName.StartsWith("ins2")) {
			//  //if(patPlanList.Count>0) {
			//  MsgBox.Show(this,"Insurance cannot be imported yet.");
			//  return false;
			//  //}
			//}
			return true;
		}

		private bool IsImportable(SheetImportRow row) {
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
			if(_listSheetImportRows[e.Row].IsSeparator) {
				return;
			}
			if(!IsEditable(_listSheetImportRows[e.Row])){
				return;
			}
			if(_listSheetImportRows[e.Row].FieldName=="referredFrom") {
				FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
				frmReferralSelect.IsSelectionMode=true;
				frmReferralSelect.ShowDialog();
				if(frmReferralSelect.IsDialogCancel) {
					return;
				}
				Referral referralSelected=frmReferralSelect.ReferralSelected;
				_listSheetImportRows[e.Row].DoImport=true;
				_listSheetImportRows[e.Row].IsFlaggedImp=false;
				_listSheetImportRows[e.Row].ImpValDisplay=referralSelected.GetNameFL();
				_listSheetImportRows[e.Row].ImpValObj=referralSelected;
			}
			#region string
			else if(_listSheetImportRows[e.Row].ObjType==typeof(string)) {
				InputBox inputBox;
				if(_listSheetImportRows[e.Row].FieldName.In("WkPhone","WirelessPhone","HmPhone","ins1CarrierPhone","ins2CarrierPhone","ICEPhone")) {
					InputBoxParam inputBoxParam=new InputBoxParam();
					inputBoxParam.InputBoxType_=InputBoxType.ValidPhone;
					inputBoxParam.LabelText=_listSheetImportRows[e.Row].FieldName;
					inputBoxParam.Text=_listSheetImportRows[e.Row].ImpValDisplay;
					inputBox=new InputBox(inputBoxParam);
				}
				else {
					InputBoxParam inputBoxParam=new InputBoxParam();
					inputBoxParam.InputBoxType_=InputBoxType.TextBox;
					inputBoxParam.LabelText=_listSheetImportRows[e.Row].FieldName;
					inputBoxParam.Text=_listSheetImportRows[e.Row].ImpValDisplay;
					inputBox=new InputBox(inputBoxParam);
				}
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return;
				}
				if(_listSheetImportRows[e.Row].FieldName=="addressAndHmPhoneIsSameEntireFamily") {
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
				if(_listSheetImportRows[e.Row].OldValDisplay==inputBox.StringResult) {//value is now same as original
					_listSheetImportRows[e.Row].DoImport=false;
				}
				else {
					_listSheetImportRows[e.Row].DoImport=true;
				}
				_listSheetImportRows[e.Row].ImpValDisplay=inputBox.StringResult;
				_listSheetImportRows[e.Row].ImpValObj=inputBox.StringResult;
			}
			#endregion
			#region Enum
			else if(_listSheetImportRows[e.Row].ObjType.IsEnum) {
				//Note.  This only works for zero-indexed enums.
				using FormSheetImportEnumPicker formSheetImportEnumPicker=new FormSheetImportEnumPicker(_listSheetImportRows[e.Row].FieldName);
				for(int i=0;i<Enum.GetNames(_listSheetImportRows[e.Row].ObjType).Length;i++) {
					formSheetImportEnumPicker.listResult.Items.Add(Enum.GetNames(_listSheetImportRows[e.Row].ObjType)[i]);
					if(_listSheetImportRows[e.Row].ImpValObj!=null && i==(int)_listSheetImportRows[e.Row].ImpValObj) {
						formSheetImportEnumPicker.listResult.SelectedIndex=i;
					}
				}
				formSheetImportEnumPicker.ShowDialog();
				if(formSheetImportEnumPicker.DialogResult==DialogResult.OK) {
					int selectedI=formSheetImportEnumPicker.listResult.SelectedIndex;
					if(_listSheetImportRows[e.Row].ImpValObj==null) {//was initially null
						if(selectedI!=-1) {//an item was selected
							_listSheetImportRows[e.Row].ImpValObj=Enum.ToObject(_listSheetImportRows[e.Row].ObjType,selectedI);
							_listSheetImportRows[e.Row].ImpValDisplay=_listSheetImportRows[e.Row].ImpValObj.ToString();
						}
					}
					else {//was not initially null
						if((int)_listSheetImportRows[e.Row].ImpValObj!=selectedI) {//value was changed.
							//There's no way for the user to set it to null, so we do not need to test that
							_listSheetImportRows[e.Row].ImpValObj=Enum.ToObject(_listSheetImportRows[e.Row].ObjType,selectedI);
							_listSheetImportRows[e.Row].ImpValDisplay=_listSheetImportRows[e.Row].ImpValObj.ToString();
						}
					}
					if(selectedI==-1) {
						_listSheetImportRows[e.Row].DoImport=false;//impossible to import a null
					}
					else if(_listSheetImportRows[e.Row].OldValObj!=null && (int)_listSheetImportRows[e.Row].ImpValObj==(int)_listSheetImportRows[e.Row].OldValObj) {//it's the old setting for the patient, whether or not they actually changed it.
						_listSheetImportRows[e.Row].DoImport=false;//so no need to import
					}
					else {
						_listSheetImportRows[e.Row].DoImport=true;
					}
				}
			}
			#endregion
			#region DateTime
			else if(_listSheetImportRows[e.Row].ObjType==typeof(DateTime)) {//this is only for one field so far: Birthdate
				InputBoxParam inputBoxParam=new InputBoxParam();
				inputBoxParam.InputBoxType_=InputBoxType.ValidDate;
				inputBoxParam.LabelText=_listSheetImportRows[e.Row].FieldName;
				//Display the importing date if valid, otherwise display the invalid date from the form.
				if(_listSheetImportRows[e.Row].ImpValDisplay==INVALID_DATE){
					inputBoxParam.Text=_listSheetImportRows[e.Row].NewValDisplay;
				}
				else {
					inputBoxParam.Text=_listSheetImportRows[e.Row].ImpValDisplay;
				}
				InputBox inputBox=new InputBox(inputBoxParam);//Display the date format that the current computer will use when parsing the date.
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					return;
				}
				DateTime dateTimeEntered=inputBox.DateResult;
				if(dateTimeEntered==DateTime.MinValue) {
					_listSheetImportRows[e.Row].ImpValDisplay="";
				}
				else if(dateTimeEntered.Year<1880 || dateTimeEntered.Year>2050) {
					MsgBox.Show(this,INVALID_DATE);
					return;
				}
				else {
					_listSheetImportRows[e.Row].ImpValDisplay=dateTimeEntered.ToShortDateString();
				}
				_listSheetImportRows[e.Row].ImpValObj=dateTimeEntered;
				if(_listSheetImportRows[e.Row].ImpValDisplay==_listSheetImportRows[e.Row].OldValDisplay) {//value is now same as original
					_listSheetImportRows[e.Row].DoImport=false;
				}
				else {
					_listSheetImportRows[e.Row].DoImport=true;
				}
			}
			#endregion
			#region Medication, Allergy or Disease
			else if(_listSheetImportRows[e.Row].ObjType==typeof(MedicationPat)
				|| _listSheetImportRows[e.Row].ObjType==typeof(Allergy)
				|| _listSheetImportRows[e.Row].ObjType==typeof(Disease)) 
			{
				//User entered medications will have a MedicationNum as the ImpValObj.
				if(_listSheetImportRows[e.Row].ImpValObj.GetType()==typeof(long)) {
					using FormMedications formMedications=new FormMedications();
					formMedications.IsSelectionMode=true;
					formMedications.textSearch.Text=_listSheetImportRows[e.Row].FieldName.Trim();
					formMedications.ShowDialog();
					if(formMedications.DialogResult!=DialogResult.OK) {
						return;
					}
					_listSheetImportRows[e.Row].ImpValDisplay="Y";
					_listSheetImportRows[e.Row].ImpValObj=formMedications.SelectedMedicationNum;
					string descript=Medications.GetDescription(formMedications.SelectedMedicationNum);
					_listSheetImportRows[e.Row].FieldDisplay=descript;
					((SheetField)_listSheetImportRows[e.Row].NewValObj).FieldValue=descript;
					_listSheetImportRows[e.Row].NewValDisplay="Y";
					_listSheetImportRows[e.Row].DoImport=true;
					_listSheetImportRows[e.Row].IsFlaggedImp=false;
				}
				else {
					using FormSheetImportEnumPicker formSheetImportEnumPicker=new FormSheetImportEnumPicker(_listSheetImportRows[e.Row].FieldName);
					formSheetImportEnumPicker.listResult.Items.AddEnums<YN>();
					formSheetImportEnumPicker.listResult.SelectedIndex=0;//Unknown
					if(_listSheetImportRows[e.Row].ImpValDisplay=="Y") {
						formSheetImportEnumPicker.listResult.SelectedIndex=1;
					}
					if(_listSheetImportRows[e.Row].ImpValDisplay=="N") {
						formSheetImportEnumPicker.listResult.SelectedIndex=2;
					}
					formSheetImportEnumPicker.ShowDialog();
					if(formSheetImportEnumPicker.DialogResult!=DialogResult.OK) {
						return;
					}
					int selectedI=formSheetImportEnumPicker.listResult.SelectedIndex;
					switch(selectedI) {
						case 0:
							_listSheetImportRows[e.Row].ImpValDisplay="";
							break;
						case 1:
							_listSheetImportRows[e.Row].ImpValDisplay="Y";
							break;
						case 2:
							_listSheetImportRows[e.Row].ImpValDisplay="N";
							break;
					}
					if(_listSheetImportRows[e.Row].OldValDisplay==_listSheetImportRows[e.Row].ImpValDisplay) {//value is now same as original
						_listSheetImportRows[e.Row].DoImport=false;
					}
					else {
						_listSheetImportRows[e.Row].DoImport=true;
					}
					if(selectedI==-1 || selectedI==0) {
						_listSheetImportRows[e.Row].DoImport=false;
					}
				}
			}
			#endregion
			#region Subscriber
			else if(_listSheetImportRows[e.Row].ObjType==typeof(Patient)) {
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
				if(_listSheetImportRows[e.Row].OldValDisplay==patName) {
					_listSheetImportRows[e.Row].DoImport=false;
				}
				else {
					_listSheetImportRows[e.Row].DoImport=true;
				}
				_listSheetImportRows[e.Row].ImpValDisplay=patName;
				_listSheetImportRows[e.Row].ImpValObj=patientSubscriber;
			}
			#endregion
			#region Carrier
			else if(_listSheetImportRows[e.Row].ObjType==typeof(Carrier)) {
				//Change both carrier rows at the same time.
				string insStr="ins1";
				if(_listSheetImportRows[e.Row].FieldName.StartsWith("ins2")) {
					insStr="ins2";
				}
				SheetImportRow sheetImportRowCarrierName=GetImportRowByFieldName(insStr+"CarrierName");
				SheetImportRow sheetImportRowCarrierPhone=GetImportRowByFieldName(insStr+"CarrierPhone");
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
			SheetImportRow sheetImportRowRelation=GetImportRowByFieldName(insStr+"Relat");
			SheetImportRow sheetImportRowSubscriber=GetImportRowByFieldName(insStr+"Subscriber");
			SheetImportRow sheetImportRowSubscriberId=GetImportRowByFieldName(insStr+"SubscriberID");
			SheetImportRow sheetImportRowCarrierName=GetImportRowByFieldName(insStr+"CarrierName");
			SheetImportRow sheetImportRowCarrierPhone=GetImportRowByFieldName(insStr+"CarrierPhone");
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
			SheetImportRow sheetImportRowRelation=GetImportRowByFieldName(insStr+"Relat");
			SheetImportRow sheetImportRowSubscriberId=GetImportRowByFieldName(insStr+"SubscriberID");
			SheetImportRow sheetImportRowEmployerName=GetImportRowByFieldName(insStr+"EmployerName");
			SheetImportRow sheetImportRowGroupName=GetImportRowByFieldName(insStr+"GroupName");
			SheetImportRow sheetImportRowGroupNum=GetImportRowByFieldName(insStr+"GroupNum");
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
			SheetImportRow sheetImportRowRelation=GetImportRowByFieldName(insStr+"Relat");
			SheetImportRow sheetImportRowSubscriber=GetImportRowByFieldName(insStr+"Subscriber");
			SheetImportRow sheetImportRowSubscriberId=GetImportRowByFieldName(insStr+"SubscriberID");
			SheetImportRow sheetImportRowCarrierName=GetImportRowByFieldName(insStr+"CarrierName");
			SheetImportRow sheetImportRowCarrierPhone=GetImportRowByFieldName(insStr+"CarrierPhone");
			SheetImportRow sheetImportRowEmployerName=GetImportRowByFieldName(insStr+"EmployerName");
			SheetImportRow sheetImportRowGroupName=GetImportRowByFieldName(insStr+"GroupName");
			SheetImportRow sheetImportRowGroupNum=GetImportRowByFieldName(insStr+"GroupNum");
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

		private void butOK_Click(object sender,EventArgs e) {
			if(!_listSheetImportRows.Any(x => x.DoImport)) {
				MsgBox.Show(this,"No rows are set for import.");
				return;
			}
			if(_listSheetImportRows.Any(x => x.DoImport && x.ImpValObj is DateTime && x.ImpValDisplay==INVALID_DATE)) {
				MsgBox.Show(this,$"Please fix data entry errors first (ImportValues with '{INVALID_DATE}').");
				return;
			}
			#region Patient Form
			if(SheetCur.SheetType==SheetTypeEnum.PatientForm) {
				bool isImportingPriIns=false;
				bool isImportingSecIns=false;
				List<RefAttach> listRefAttaches=new List<RefAttach>();
				for(int i=0;i<_listSheetImportRows.Count;i++) {
					if(!_listSheetImportRows[i].DoImport) {
						continue;
					}
					//Importing insurance happens later.
					if(_listSheetImportRows[i].FieldName.StartsWith("ins1")) {
						isImportingPriIns=true;
						continue;
					}
					if(_listSheetImportRows[i].FieldName.StartsWith("ins2")) {
						isImportingSecIns=true;
						continue;
					}
					switch(_listSheetImportRows[i].FieldName) {
						#region Personal
						case "LName":
							_patient.LName=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "FName":
							_patient.FName=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "MiddleI":
							_patient.MiddleI=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "Preferred":
							_patient.Preferred=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "Gender":
							_patient.Gender=(PatientGender)_listSheetImportRows[i].ImpValObj;
							break;
						case "Position":
							_patient.Position=(PatientPosition)_listSheetImportRows[i].ImpValObj;
							break;
						case "Birthdate":
							_patient.Birthdate=(DateTime)_listSheetImportRows[i].ImpValObj;
							break;
						case "SSN":
							_patient.SSN=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "WkPhone":
							_patient.WkPhone=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "WirelessPhone":
							_patient.WirelessPhone=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "Email":
							_patient.Email=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "PreferContactMethod":
							_patient.PreferContactMethod=(ContactMethod)_listSheetImportRows[i].ImpValObj;
							break;
						case "PreferConfirmMethod":
							_patient.PreferConfirmMethod=(ContactMethod)_listSheetImportRows[i].ImpValObj;
							break;
						case "PreferRecallMethod":
							_patient.PreferRecallMethod=(ContactMethod)_listSheetImportRows[i].ImpValObj;
							break;
						case "referredFrom":
							RefAttach refAttach=new RefAttach();
							refAttach.RefType=ReferralType.RefFrom;
							refAttach.ItemOrder=1;
							refAttach.PatNum=_patient.PatNum;
							refAttach.RefDate=DateTime.Today;
							refAttach.ReferralNum=((Referral)_listSheetImportRows[i].ImpValObj).ReferralNum;
							listRefAttaches.Add(refAttach);
							break;
						case "StudentStatus":
							_patient.StudentStatus=(string)_listSheetImportRows[i].ImpValObj;
							break;
						#endregion
						#region Address and Home Phone
						//AddressSameForFam already set, but not really importable by itself
						case "Address":
							_patient.Address=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "Address2":
							_patient.Address2=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "City":
							_patient.City=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "State":
							_patient.State=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "StateNoValidation":
							_patient.State=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "Zip":
							_patient.Zip=_listSheetImportRows[i].ImpValDisplay;
							break;
						case "HmPhone":
							_patient.HmPhone=_listSheetImportRows[i].ImpValDisplay;
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
			#region Medical History
			else if(SheetCur.SheetType==SheetTypeEnum.MedicalHistory) {
				for(int i=0;i<_listSheetImportRows.Count;i++) {
					if(!_listSheetImportRows[i].DoImport) {
						continue;
					}
					if(_listSheetImportRows[i].ObjType==null) {//Should never happen.
						continue;
					}
					YN hasValue=YN.Unknown;
					if(_listSheetImportRows[i].ImpValDisplay=="Y") {
						hasValue=YN.Yes;
					}
					if(_listSheetImportRows[i].ImpValDisplay=="N") {
						hasValue=YN.No;
					}
					if(hasValue==YN.Unknown) {//Unknown, nothing to do.
						continue;
					}
					#region Allergies
					if(_listSheetImportRows[i].ObjType==typeof(Allergy)) {
						//Patient has this allergy in the db so just update the value.
						if(_listSheetImportRows[i].OldValObj!=null) {
							Allergy allergyOld=(Allergy)_listSheetImportRows[i].OldValObj;
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
						SheetField sheetFieldAllergy=(SheetField)_listSheetImportRows[i].NewValObj;
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
					#endregion
					#region Medications
					else if(_listSheetImportRows[i].ObjType==typeof(MedicationPat)) {
						//Patient has this medication in the db so leave it alone or set the stop date.
						if(_listSheetImportRows[i].OldValObj!=null) {
						//Set the stop date for the current medication(s).
							MedicationPat medicationPatOld=(MedicationPat)_listSheetImportRows[i].OldValObj;
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
						SheetField sheetFieldMed=(SheetField)_listSheetImportRows[i].NewValObj;
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
					#endregion
					#region Diseases
					else if(_listSheetImportRows[i].ObjType==typeof(Disease)) {
						//Patient has this problem in the db so just update the value.
						if(_listSheetImportRows[i].OldValObj!=null) {
							Disease diseaseOld=(Disease)_listSheetImportRows[i].OldValObj;
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
						SheetField sheetFieldDisease=(SheetField)_listSheetImportRows[i].NewValObj;
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
					#endregion
				}
			}
			#endregion
			#region ICE
			List<SheetImportRow> listSheetImportRowsFieldsIce = _listSheetImportRows.FindAll(x => x.FieldName.StartsWith("ICE") && x.DoImport);
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
			for(int i=0;i<_listSheetImportRows.Count;i++) {
				if(_listSheetImportRows[i].FieldName!=fieldName) {
					continue;
				}
				return _listSheetImportRows[i].DoImport;
			}
			return false;
		}

		///<summary>Will return null if field not found or if field marked to not import.</summary>
		private object GetImpObj(string fieldName) {
			for(int i=0;i<_listSheetImportRows.Count;i++) {
				if(_listSheetImportRows[i].FieldName!=fieldName) {
					continue;
				}
				if(!_listSheetImportRows[i].DoImport) {
					return null;
				}
				return _listSheetImportRows[i].ImpValObj;
			}
			return null;
		}

		///<summary>Will return empty string field not found or if field marked to not import.</summary>
		private string GetImpDisplay(string fieldName) {
			for(int i=0;i<_listSheetImportRows.Count;i++) {
				if(_listSheetImportRows[i].FieldName!=fieldName) {
					continue;
				}
				if(!_listSheetImportRows[i].DoImport) {
					return "";
				}
				return _listSheetImportRows[i].ImpValDisplay;
			}
			return "";
		}

		///<summary>Returns a separator and sets the FieldName to the passed in string.</summary>
		private SheetImportRow CreateSeparator(string displayText) {
			SheetImportRow sheetImportRow=new SheetImportRow();
			sheetImportRow.FieldName=displayText;
			sheetImportRow.IsSeparator=true;
			return sheetImportRow;
		}

		private class SheetImportRow {
			public string FieldName;
			///<summary>Overrides FieldName.  If null, use FieldName;</summary>
			public string FieldDisplay;
			public string OldValDisplay;
			public object OldValObj;
			public string NewValDisplay;
			public object NewValObj;
			public string ImpValDisplay;
			public object ImpValObj;
			public bool DoImport;
			public bool IsSeparator;
			///<summary>This is needed because the NewValObj might be null.</summary>
			public Type ObjType;
			///<summary>Some fields are not importable, but they still need to be made obvious to user by coloring the user-entered value red.</summary>
			public bool IsFlagged;
			///<summary>The import cell is shown with colored text to prompt user to notice.</summary>
			public bool IsFlaggedImp;
		}

	}
}