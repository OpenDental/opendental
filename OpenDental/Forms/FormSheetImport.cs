using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using Acrobat;
using AFORMAUTLib;//Acrobat forms
using System.Linq;
using CodeBase;
using System.Globalization;

namespace OpenDental {
	public partial class FormSheetImport:FormODBase {
		public Sheet SheetCur;
		public Document DocCur;
		private List<SheetImportRow> Rows;
		private Patient PatCur;
		///<summary>A copy of the patient when the form loads.  Used to know the what will change upon import.</summary>
		private Patient PatOld;
		private PatientNote PatNoteCur;
		private PatientNote PatNoteOld;
		private Family Fam;
		///<summary>We must have a readily available bool, whether or not this checkbox field is present on the sheet.  It gets set at the very beginning, then gets changes based on user input on the sheet and in this window.</summary>
		private bool AddressSameForFam;
		private InsPlan Plan1;
		private InsPlan Plan2;
		private List<PatPlan> PatPlanList;
		private List<InsPlan> PlanList;
		private PatPlan PatPlan1;
		private PatPlan PatPlan2;
		private Relat? Ins1Relat;
		private Relat? Ins2Relat;
		private Carrier Carrier1;
		private Carrier Carrier2;
		private List<InsSub> SubList;
		private InsSub Sub1;
		private InsSub Sub2;
		///<summary>In order to import insurance plans the sheet must contain Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone.  This variable gets set when the sheet loads and will indicate if all fields are present for primary OR for secondary insurance.  Insurance should not attempt to import if this is false.</summary>
		private bool HasRequiredInsFields;
		private bool _isPatTransferSheet;
		private const string INVALID_DATE="Invalid Date";

		public FormSheetImport() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetImport_Load(object sender,EventArgs e) {
			if(SheetCur!=null) {
				PatCur=Patients.GetPat(SheetCur.PatNum);
				PatOld=PatCur.Copy();
				PatNoteCur=PatientNotes.Refresh(PatCur.PatNum,PatCur.Guarantor);
				PatNoteOld=PatNoteCur.Copy();
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
			Fam=Patients.GetFamily(PatCur.PatNum);
			AddressSameForFam=true;
			for(int i=0;i<Fam.ListPats.Length;i++) {
				if(PatCur.HmPhone!=Fam.ListPats[i].HmPhone
					|| PatCur.Address!=Fam.ListPats[i].Address
					|| PatCur.Address2!=Fam.ListPats[i].Address2
					|| PatCur.City!=Fam.ListPats[i].City
					|| PatCur.State!=Fam.ListPats[i].State
					|| PatCur.Zip!=Fam.ListPats[i].Zip) 
				{
					AddressSameForFam=false;
					break;
				}
			}
			PatPlanList=PatPlans.Refresh(PatCur.PatNum);
			SubList=InsSubs.RefreshForFam(Fam);
			PlanList=InsPlans.RefreshForSubList(SubList);
			if(PatPlanList.Count==0) {
				PatPlan1=null;
				Plan1=null;
				Sub1=null;
				Ins1Relat=null;
				Carrier1=null;
			}
			else {
				PatPlan1=PatPlanList[0];
				Sub1=InsSubs.GetSub(PatPlan1.InsSubNum,SubList);
				Plan1=InsPlans.GetPlan(Sub1.PlanNum,PlanList);
				Ins1Relat=PatPlan1.Relationship;
				Carrier1=Carriers.GetCarrier(Plan1.CarrierNum);
			}
			if(PatPlanList.Count<2) {
				PatPlan2=null;
				Plan2=null;
				Sub2=null;
				Ins2Relat=null;
				Carrier2=null;
			}
			else {
				PatPlan2=PatPlanList[1];
				Sub2=InsSubs.GetSub(PatPlan2.InsSubNum,SubList);
				Plan2=InsPlans.GetPlan(Sub2.PlanNum,PlanList);
				Ins2Relat=PatPlan2.Relationship;
				Carrier2=Carriers.GetCarrier(Plan2.CarrierNum);
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
				Rows=new List<SheetImportRow>();
				SheetImportRow row;
				string fieldVal;
				Rows.Add(CreateSeparator("Personal"));
				#region personal
				//LName---------------------------------------------
				fieldVal=GetInputValue("LName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="LName";
					row.OldValDisplay=PatCur.LName;
					row.OldValObj=PatCur.LName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//FName---------------------------------------------
				fieldVal=GetInputValue("FName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="FName";
					row.OldValDisplay=PatCur.FName;
					row.OldValObj=PatCur.FName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//MiddleI---------------------------------------------
				fieldVal=GetInputValue("MiddleI");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="MiddleI";
					row.OldValDisplay=PatCur.MiddleI;
					row.OldValObj=PatCur.MiddleI;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//Preferred---------------------------------------------
				fieldVal=GetInputValue("Preferred");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Preferred";
					row.OldValDisplay=PatCur.Preferred;
					row.OldValObj=PatCur.Preferred;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//Gender---------------------------------------------
				fieldVal=GetRadioValue("Gender");
				if(fieldVal!=null) {//field exists on form
					row=new SheetImportRow();
					row.FieldName="Gender";
					row.OldValDisplay=Lan.g("enumPatientGender",PatCur.Gender.ToString());
					row.OldValObj=PatCur.Gender;
					if(fieldVal=="") {//no box was checked
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							PatientGender gender=(PatientGender)Enum.Parse(typeof(PatientGender),fieldVal);
							row.NewValDisplay=Lan.g("enumPatientGender",gender.ToString());
							row.NewValObj=gender;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid gender."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(PatientGender);
					if(row.NewValObj!=null && (PatientGender)row.NewValObj!=PatCur.Gender) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//Position---------------------------------------------
				fieldVal=GetRadioValue("Position");
				if(fieldVal!=null) {//field exists on form
					row=new SheetImportRow();
					row.FieldName="Position";
					row.OldValDisplay=Lan.g("enumPatientPositionr",PatCur.Position.ToString());
					row.OldValObj=PatCur.Position;
					if(fieldVal=="") {//no box was checked
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							PatientPosition position=(PatientPosition)Enum.Parse(typeof(PatientPosition),fieldVal);
							row.NewValDisplay=Lan.g("enumPatientPosition",position.ToString());
							row.NewValObj=position;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid PatientPosition."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(PatientPosition);
					if(row.NewValObj!=null && (PatientPosition)row.NewValObj!=PatCur.Position) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//Birthdate---------------------------------------------
				fieldVal=GetInputValue("Birthdate");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Birthdate";
					if(PatCur.Birthdate.Year<1880) {
						row.OldValDisplay="";
					}
					else {
						row.OldValDisplay=PatCur.Birthdate.ToShortDateString();
					}
					row.OldValObj=PatCur.Birthdate;
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
					Rows.Add(row);
				}
				//SSN---------------------------------------------
				fieldVal=GetInputValue("SSN");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="SSN";
					row.OldValDisplay=PatCur.SSN;
					row.OldValObj=PatCur.SSN;
					row.NewValDisplay=fieldVal.Replace("-","");//quickly strip dashes
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//WkPhone---------------------------------------------
				fieldVal=GetInputValue("WkPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="WkPhone";
					row.OldValDisplay=PatCur.WkPhone;
					row.OldValObj=PatCur.WkPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//WirelessPhone---------------------------------------------
				fieldVal=GetInputValue("WirelessPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="WirelessPhone";
					row.OldValDisplay=PatCur.WirelessPhone;
					row.OldValObj=PatCur.WirelessPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
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
					Rows.Add(row);
				}
				//Email---------------------------------------------
				fieldVal=GetInputValue("Email");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Email";
					row.OldValDisplay=PatCur.Email;
					row.OldValObj=PatCur.Email;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//PreferContactMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferContactMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferContactMethod");
				}
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="PreferContactMethod";
					row.OldValDisplay=Lan.g("enumContactMethod",PatCur.PreferContactMethod.ToString());
					row.OldValObj=PatCur.PreferContactMethod;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							ContactMethod cmeth=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							row.NewValDisplay=Lan.g("enumContactMethod",cmeth.ToString());
							row.NewValObj=cmeth;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(ContactMethod);
					if(row.NewValObj!=null && (ContactMethod)row.NewValObj!=PatCur.PreferContactMethod) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//PreferConfirmMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferConfirmMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferConfirmMethod");
				}
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="PreferConfirmMethod";
					row.OldValDisplay=Lan.g("enumContactMethod",PatCur.PreferConfirmMethod.ToString());
					row.OldValObj=PatCur.PreferConfirmMethod;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							ContactMethod cmeth=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							row.NewValDisplay=Lan.g("enumContactMethod",cmeth.ToString());
							row.NewValObj=cmeth;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(ContactMethod);
					if(row.NewValObj!=null && (ContactMethod)row.NewValObj!=PatCur.PreferConfirmMethod) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//PreferRecallMethod---------------------------------------------
				fieldVal=GetRadioValue("PreferRecallMethod");
				if(_isPatTransferSheet) {
					fieldVal=GetInputValue("PreferRecallMethod");
				}
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="PreferRecallMethod";
					row.OldValDisplay=Lan.g("enumContactMethod",PatCur.PreferRecallMethod.ToString());
					row.OldValObj=PatCur.PreferRecallMethod;
					if(fieldVal=="") {
						row.NewValDisplay="";
						row.NewValObj=null;
					}
					else {
						try {
							ContactMethod cmeth=(ContactMethod)Enum.Parse(typeof(ContactMethod),fieldVal);
							row.NewValDisplay=Lan.g("enumContactMethod",cmeth.ToString());
							row.NewValObj=cmeth;
						}
						catch {
							MessageBox.Show(fieldVal+Lan.g(this," is not a valid ContactMethod."));
						}
					}
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(ContactMethod);
					if(row.NewValObj!=null && (ContactMethod)row.NewValObj!=PatCur.PreferRecallMethod) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//referredFrom---------------------------------------------
				fieldVal=GetInputValue("referredFrom");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="referredFrom";
					Referral refer=Referrals.GetReferralForPat(PatCur.PatNum);
					if(refer==null) {//there was no existing referral
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
						row.OldValDisplay=refer.GetNameFL();
						row.OldValObj=refer;
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
					Rows.Add(row);
				}
				//ICE Name---------------------------------------------
				fieldVal=GetInputValue("ICEName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEName";
					row.OldValDisplay=PatNoteCur.ICEName;
					row.OldValObj=PatNoteCur.ICEName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					Rows.Add(row);
				}
				//ICE Phone---------------------------------------------
				fieldVal=GetInputValue("ICEPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEPhone";
					row.OldValDisplay=PatNoteCur.ICEPhone;
					row.OldValObj=PatNoteCur.ICEPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					Rows.Add(row);
				}	
				#endregion personal
				//Separator-------------------------------------------
				Rows.Add(CreateSeparator("Address and Home Phone"));
				#region address
				//SameForEntireFamily-------------------------------------------
				if(ContainsOneOfFields("addressAndHmPhoneIsSameEntireFamily")) {
					row=new SheetImportRow();
					row.FieldName="addressAndHmPhoneIsSameEntireFamily";
					row.FieldDisplay="Same for entire family";
					if(AddressSameForFam) {//remember we calculated this in the form constructor.
						row.OldValDisplay="X";
						row.OldValObj="X";
					}
					else {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					//And now, we will revise AddressSameForFam based on user input
					AddressSameForFam=IsChecked("addressAndHmPhoneIsSameEntireFamily");
					if(AddressSameForFam) {
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
					Rows.Add(row);
				}
				//Address---------------------------------------------
				fieldVal=GetInputValue("Address");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Address";
					row.OldValDisplay=PatCur.Address;
					row.OldValObj=PatCur.Address;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//Address2---------------------------------------------
				fieldVal=GetInputValue("Address2");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Address2";
					row.OldValDisplay=PatCur.Address2;
					row.OldValObj=PatCur.Address2;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//City---------------------------------------------
				fieldVal=GetInputValue("City");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="City";
					row.OldValDisplay=PatCur.City;
					row.OldValObj=PatCur.City;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//State---------------------------------------------
				fieldVal=GetInputValue("State");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="State";
					row.OldValDisplay=PatCur.State;
					row.OldValObj=PatCur.State;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//Zip---------------------------------------------
				fieldVal=GetInputValue("Zip");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="Zip";
					row.OldValDisplay=PatCur.Zip;
					row.OldValObj=PatCur.Zip;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				//HmPhone---------------------------------------------
				fieldVal=GetInputValue("HmPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="HmPhone";
					row.OldValDisplay=PatCur.HmPhone;
					row.OldValObj=PatCur.HmPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				#endregion address
				//Separator-------------------------------------------
				Rows.Add(CreateSeparator("Insurance Policy 1"));
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
					row.OldValDisplay=Lan.g("enumRelat",Ins1Relat.ToString());
					row.OldValObj=Ins1Relat;
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
					Rows.Add(row);
				}
				//ins1Subscriber---------------------------------------------
				fieldVal=GetInputValue("ins1SubscriberNameF");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1Subscriber";
					row.FieldDisplay="Subscriber";
					if(Plan1!=null) {
						row.OldValDisplay=Fam.GetNameInFamFirst(Sub1.Subscriber);
						row.OldValObj=Sub1.Subscriber;
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
					Rows.Add(row);
				}
				//ins1SubscriberID---------------------------------------------
				fieldVal=GetInputValue("ins1SubscriberID");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1SubscriberID";
					row.FieldDisplay="Subscriber ID";
					if(Plan1!=null) {
						row.OldValDisplay=Sub1.SubscriberID;
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
					Rows.Add(row);
				}
				//ins1CarrierName---------------------------------------------
				fieldVal=GetInputValue("ins1CarrierName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1CarrierName";
					row.FieldDisplay="Carrier";
					if(Carrier1!=null) {
						row.OldValDisplay=Carrier1.CarrierName;
						row.OldValObj=Carrier1;
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
					Rows.Add(row);
				}
				//ins1CarrierPhone---------------------------------------------
				fieldVal=GetInputValue("ins1CarrierPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1CarrierPhone";
					row.FieldDisplay="Phone";
					if(Carrier1!=null) {
						row.OldValDisplay=Carrier1.Phone;
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
					Rows.Add(row);
				}
				//ins1EmployerName---------------------------------------------
				fieldVal=GetInputValue("ins1EmployerName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1EmployerName";
					row.FieldDisplay="Employer";
					if(Plan1==null) {
						row.OldValDisplay="";
						row.OldValObj="";
					}
					else {
						row.OldValDisplay=Employers.GetName(Plan1.EmployerNum);
						row.OldValObj=Employers.GetEmployer(Plan1.EmployerNum);
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
					Rows.Add(row);
				}
				//ins1GroupName---------------------------------------------
				fieldVal=GetInputValue("ins1GroupName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1GroupName";
					row.FieldDisplay="Group Name";
					if(Plan1!=null) {
						row.OldValDisplay=Plan1.GroupName;
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
					Rows.Add(row);
				}
				//ins1GroupNum---------------------------------------------
				fieldVal=GetInputValue("ins1GroupNum");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins1GroupNum";
					row.FieldDisplay="Group Num";
					if(Plan1!=null) {
						row.OldValDisplay=Plan1.GroupNum;
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
					Rows.Add(row);
				}
				#endregion ins1
				//Separator-------------------------------------------
				Rows.Add(CreateSeparator("Insurance Policy 2"));
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
					row.OldValDisplay=Lan.g("enumRelat",Ins2Relat.ToString());
					row.OldValObj=Ins2Relat;
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
					Rows.Add(row);
				}
				//ins2Subscriber---------------------------------------------
				fieldVal=GetInputValue("ins2SubscriberNameF");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2Subscriber";
					row.FieldDisplay="Subscriber";
					if(Plan2!=null) {
						row.OldValDisplay=Fam.GetNameInFamFirst(Sub2.Subscriber);
						row.OldValObj=Sub2.Subscriber;
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
					Rows.Add(row);
				}
				//ins2SubscriberID---------------------------------------------
				fieldVal=GetInputValue("ins2SubscriberID");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2SubscriberID";
					row.FieldDisplay="Subscriber ID";
					if(Plan2!=null) {
						row.OldValDisplay=Sub2.SubscriberID;
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
					Rows.Add(row);
				}
				//ins2CarrierName---------------------------------------------
				fieldVal=GetInputValue("ins2CarrierName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2CarrierName";
					row.FieldDisplay="Carrier";
					if(Carrier2!=null) {
						row.OldValDisplay=Carrier2.CarrierName;
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
					Rows.Add(row);
				}
				//ins2CarrierPhone---------------------------------------------
				fieldVal=GetInputValue("ins2CarrierPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2CarrierPhone";
					row.FieldDisplay="Phone";
					if(Carrier2!=null) {
						row.OldValDisplay=Carrier2.Phone;
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
					Rows.Add(row);
				}
				//ins2EmployerName---------------------------------------------
				fieldVal=GetInputValue("ins2EmployerName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2EmployerName";
					row.FieldDisplay="Employer";
					if(Plan2==null) {
						row.OldValDisplay="";
					}
					else {
						row.OldValDisplay=Employers.GetName(Plan2.EmployerNum);
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
					Rows.Add(row);
				}
				//ins2GroupName---------------------------------------------
				fieldVal=GetInputValue("ins2GroupName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2GroupName";
					row.FieldDisplay="Group Name";
					if(Plan2!=null) {
						row.OldValDisplay=Plan2.GroupName;
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
					Rows.Add(row);
				}
				//ins2GroupNum---------------------------------------------
				fieldVal=GetInputValue("ins2GroupNum");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ins2GroupNum";
					row.FieldDisplay="Group Num";
					if(Plan2!=null) {
						row.OldValDisplay=Plan2.GroupNum;
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
					Rows.Add(row);
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
				Rows=new List<SheetImportRow>();
				string fieldVal="";
				List<Allergy> allergies=null;
				List<Disease> diseases=null;
				SheetImportRow row;
				Rows.Add(CreateSeparator("Personal"));
				#region ICE
				//ICE Name---------------------------------------------
				fieldVal=GetInputValue("ICEName");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEName";
					row.OldValDisplay=PatNoteCur.ICEName;
					row.OldValObj=PatNoteCur.ICEName;
					row.NewValDisplay=fieldVal;
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					Rows.Add(row);
				}
				//ICE Phone---------------------------------------------
				fieldVal=GetInputValue("ICEPhone");
				if(fieldVal!=null) {
					row=new SheetImportRow();
					row.FieldName="ICEPhone";
					row.OldValDisplay=PatNoteCur.ICEPhone;
					row.OldValObj=PatNoteCur.ICEPhone;
					row.NewValDisplay=TelephoneNumbers.AutoFormat(fieldVal);
					row.NewValObj=row.NewValDisplay;
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=row.NewValObj;
					row.ObjType=typeof(string);
					if(row.OldValDisplay!=row.NewValDisplay) {
						row.DoImport=true;
					}
					row.IsFlagged=true;//if user entered nothing, the red text won't show anyway.
					Rows.Add(row);
				}
				#endregion
				//Separator-------------------------------------------
				Rows.Add(CreateSeparator("Allergies"));
				#region Allergies
				//Get list of all the allergy check boxes
				List<SheetField> allergyList=GetSheetFieldsByFieldName("allergy:");
				for(int i=0;i<allergyList.Count;i++) {
					fieldVal="";
					if(i<1) {
						allergies=Allergies.GetAll(PatCur.PatNum,true);
					}
					row=new SheetImportRow();
					row.FieldName=allergyList[i].FieldName.Remove(0,8);
					row.OldValDisplay="";
					row.OldValObj=null;
					//Check if allergy exists.
					for(int j=0;j<allergies.Count;j++) {
						if(AllergyDefs.GetDescription(allergies[j].AllergyDefNum)==allergyList[i].FieldName.Remove(0,8)) {
							if(allergies[j].StatusIsActive) {
								row.OldValDisplay="Y";
							}
							else {
								row.OldValDisplay="N";
							}
							row.OldValObj=allergies[j];
							break;
						}
					}
					SheetField oppositeBox=GetOppositeSheetFieldCheckBox(allergyList,allergyList[i]);
					if(allergyList[i].FieldValue=="") {//Current box not checked.
						if(oppositeBox==null || oppositeBox.FieldValue=="") {//No opposite box or both boxes are not checked.
							//Create a blank row just in case they want to import.
							row.NewValDisplay="";
							row.NewValObj=allergyList[i];
							row.ImpValDisplay="";
							row.ImpValObj="";
							row.ObjType=typeof(Allergy);
							Rows.Add(row);
							if(oppositeBox!=null) {
								allergyList.Remove(oppositeBox);//Removes possible duplicate entry.
							}
							continue;
						}
						//Opposite box is checked, figure out if it's a Y or N box.
						if(oppositeBox.RadioButtonValue=="Y") {
							fieldVal="Y";
						}
						else {
							fieldVal="N";
						}
					}
					else {//Current box is checked.  
						if(allergyList[i].RadioButtonValue=="Y") {
							fieldVal="Y";
						}
						else {
							fieldVal="N";
						}
					}
					//Get rid of the opposite check box so field doesn't show up twice.
					if(oppositeBox!=null) {
						allergyList.Remove(oppositeBox);
					}
					row.NewValDisplay=fieldVal;
					row.NewValObj=allergyList[i];
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=typeof(string);
					row.ObjType=typeof(Allergy);
					if(row.OldValDisplay!=row.NewValDisplay && !(row.OldValDisplay=="" && row.NewValDisplay=="N")) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				#endregion
				//Separator-------------------------------------------
				Rows.Add(CreateSeparator("Medications"));
				#region Medications
				List<SheetField> inputMedList=GetSheetFieldsByFieldName("inputMed");
				List<SheetField> checkMedList=GetSheetFieldsByFieldName("checkMed");
				List<SheetField> currentMedList=new List<SheetField>();
				List<SheetField> newMedList=new List<SheetField>();
				for(int i=0;i<inputMedList.Count;i++) {
					if(inputMedList[i].FieldType==SheetFieldType.OutputText) {
						currentMedList.Add(inputMedList[i]);
					}
					else {//User might have tried to type in a new medication they are taking.
						newMedList.Add(inputMedList[i]);
					}
				}
				List<MedicationPat> listMedPatFull=MedicationPats.Refresh(PatCur.PatNum,false);
				for(int i=0;i<currentMedList.Count;i++) {
					#region existing medications
					fieldVal="";
					row=new SheetImportRow();
					row.FieldName=currentMedList[i].FieldValue;//Will be the name of the drug.
					row.OldValDisplay="N";
					row.OldValObj=null;
					for(int j=0;j<listMedPatFull.Count;j++) {
						string strMedName=listMedPatFull[j].MedDescript;//for meds that came back from NewCrop
						if(listMedPatFull[j].MedicationNum!=0) {//For meds entered in OD and linked to Medication list.
							strMedName=Medications.GetDescription(listMedPatFull[j].MedicationNum);
						}
						if(currentMedList[i].FieldValue==strMedName) {
							row.OldValDisplay="Y";
							row.OldValObj=listMedPatFull[j];
						}
					}
					List<SheetField> relatedChkBoxes=GetRelatedMedicalCheckBoxes(checkMedList,currentMedList[i]);
					for(int j=0;j<relatedChkBoxes.Count;j++) {//Figure out which corresponding checkbox is checked.
						if(relatedChkBoxes[j].FieldValue!="") {//Patient checked this box.
							if(checkMedList[j].RadioButtonValue=="Y") {
								fieldVal="Y";
							}
							else {
								fieldVal="N";
							}
							break;
						}
						//If sheet is only using N boxes and the patient already had this med marked as inactive and then they unchecked the N, so now we need to import it.
						if(relatedChkBoxes.Count==1 && relatedChkBoxes[j].RadioButtonValue=="N" //Only using N boxes for this current medication.
							&& row.OldValObj!=null && row.OldValDisplay=="N"											//Patient has this medication but is currently marked as inactive.
							&& relatedChkBoxes[j].FieldValue=="")																	//Patient unchecked the medication so we activate it again.
						{
							fieldVal="Y";
						}
					}
					if(relatedChkBoxes.Count==1 && relatedChkBoxes[0].RadioButtonValue=="N" && relatedChkBoxes[0].FieldValue=="" && row.OldValDisplay=="N" && row.OldValObj!=null) {
						row.DoImport=true;
					}
					row.NewValDisplay=fieldVal;
					row.NewValObj=currentMedList[i];
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=typeof(string);
					row.ObjType=typeof(MedicationPat);
					if(row.OldValDisplay!=row.NewValDisplay && row.NewValDisplay!="") {
						row.DoImport=true;
					}
					Rows.Add(row);
					#endregion
				}
				for(int i=0;i<newMedList.Count;i++) {
					#region medications the patient entered
					if(newMedList[i].FieldValue=="") {//No medication entered by patient.
						continue;
					}
					row=new SheetImportRow();
					row.FieldName=newMedList[i].FieldValue;//Whatever the patient typed in...
					row.OldValDisplay="";
					row.OldValObj=null;
					row.NewValDisplay="Y";
					row.NewValObj=newMedList[i];
					row.ImpValDisplay=Lan.g(this,"[double click to pick]");
					row.ImpValObj=new long();
					row.IsFlaggedImp=true;
					row.DoImport=false;//this will change to true after they pick a medication
					row.ObjType=typeof(MedicationPat);
					Rows.Add(row);
					#endregion
				}
				#endregion
				//Separator-------------------------------------------
				Rows.Add(CreateSeparator("Problems"));
				#region Problems
				List<SheetField> problemList=GetSheetFieldsByFieldName("problem:");
				for(int i=0;i<problemList.Count;i++) {
					fieldVal="";
					if(i<1) {
						diseases=Diseases.Refresh(PatCur.PatNum,false);
					}
					row=new SheetImportRow();
					row.FieldName=problemList[i].FieldName.Remove(0,8);
					//Figure out the current status of this allergy
					row.OldValDisplay="";
					row.OldValObj=null;
					for(int j=0;j<diseases.Count;j++) {
						if(DiseaseDefs.GetName(diseases[j].DiseaseDefNum)==problemList[i].FieldName.Remove(0,8)) {
							if(diseases[j].ProbStatus==ProblemStatus.Active) {
								row.OldValDisplay="Y";
							}
							else {
								row.OldValDisplay="N";
							}
							row.OldValObj=diseases[j];
							break;
						}
					}
					SheetField oppositeBox=GetOppositeSheetFieldCheckBox(problemList,problemList[i]);
					if(problemList[i].FieldValue=="") {//Current box not checked.
						if(oppositeBox==null || oppositeBox.FieldValue=="") {//No opposite box or both boxes are not checked.
							//Create a blank row just in case they still want to import.
							row.NewValDisplay="";
							row.NewValObj=problemList[i];
							row.ImpValDisplay="";
							row.ImpValObj="";
							row.ObjType=typeof(Disease);
							Rows.Add(row);
							if(oppositeBox!=null) {
								problemList.Remove(oppositeBox);//Removes possible duplicate entry.
							}
							continue;
						}
						//Opposite box is checked, figure out if it's a Y or N box.
						if(oppositeBox.RadioButtonValue=="Y") {
							fieldVal="Y";
						}
						else {
							fieldVal="N";
						}
					}
					else {//Current box is checked.  
						if(problemList[i].RadioButtonValue=="Y") {
							fieldVal="Y";
						}
						else {
							fieldVal="N";
						}
					}
					//Get rid of the opposite check box so field doesn't show up twice.
					if(oppositeBox!=null) {
						problemList.Remove(oppositeBox);
					}
					row.NewValDisplay=fieldVal;
					row.NewValObj=problemList[i];
					row.ImpValDisplay=row.NewValDisplay;
					row.ImpValObj=typeof(string);
					row.ObjType=typeof(Disease);
					if(row.OldValDisplay!=row.NewValDisplay && !(row.OldValDisplay=="" && row.NewValDisplay=="N")) {
						row.DoImport=true;
					}
					Rows.Add(row);
				}
				#endregion
			}
			#endregion 
		}

		private void FillGrid() {
			int scrollVal=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col; 
			col=new GridColumn(Lan.g(this,"FieldName"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Current Value"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Entered Value"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Import Value"),175);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Do Import"),60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			for(int i=0;i<Rows.Count;i++) {
				row=new GridRow();
				if(Rows[i].IsSeparator) {
					row.Cells.Add(Rows[i].FieldName);
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.ColorBackG=Color.DarkSlateGray;
					row.ColorText=Color.White;
				}
				else {
					if(Rows[i].FieldDisplay!=null) {
						row.Cells.Add(Rows[i].FieldDisplay);
					}
					else {
						row.Cells.Add(Rows[i].FieldName);
					}
					row.Cells.Add(Rows[i].OldValDisplay);
					cell=new GridCell(Rows[i].NewValDisplay);
					if(Rows[i].IsFlagged) {
						cell.ColorText=Color.Firebrick;
						cell.Bold=YN.Yes;
					}
					row.Cells.Add(cell);
					cell=new GridCell(Rows[i].ImpValDisplay);
					if(Rows[i].IsFlaggedImp) {
						cell.ColorText=Color.Firebrick;
						cell.Bold=YN.Yes;
					}
					row.Cells.Add(cell);
					if(Rows[i].DoImport) {
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

		///<summary>If the specified fieldName does not exist, returns null</summary>
		private string GetInputValue(string fieldName) {
			return SheetCur?.SheetFields?.FirstOrDefault(x => x.FieldType==SheetFieldType.InputField && x.FieldName==fieldName)?.FieldValue;
		}

		///<summary>If no radiobox with that name exists, returns null.  If no box is checked, it returns empty string.</summary>
		private string GetRadioValue(string fieldName) {
			List<SheetField> listFields=SheetCur?.SheetFields?.FindAll(x => x.FieldType==SheetFieldType.CheckBox && x.FieldName==fieldName);
			if(listFields==null || listFields.Count==0) {
				return null;
			}
			SheetField fieldCur=listFields.FirstOrDefault(x => x.FieldValue=="X");
			return fieldCur==null ? "" : fieldCur.RadioButtonValue;
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
			List<SheetField> retVal=new List<SheetField>();
			if(SheetCur!=null) {
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
					retVal.Add(SheetCur.SheetFields[i]);
				}
			}
			return retVal;
		}
		
		///<summary>Returns one sheet field with the same FieldName. Returns null if not found.</summary>
		private SheetImportRow GetImportRowByFieldName(string fieldName) {
			if(Rows==null) {
				return null;
			}
			for(int i=0;i<Rows.Count;i++) {
				if(Rows[i].FieldName==fieldName){
					return Rows[i];
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
			List<SheetField> checkBoxes=new List<SheetField>();
			for(int i=0;i<checkMedList.Count;i++) {
				if(checkMedList[i].FieldName.Remove(0,8)==inputMed.FieldName.Remove(0,8)) {
					checkBoxes.Add(checkMedList[i]);
				}
			}
			return checkBoxes;
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
			if(Rows[e.Row].IsSeparator) {
				return;
			}
			if(!IsImportable(Rows[e.Row])) {
				return;
			}
			Rows[e.Row].DoImport=!Rows[e.Row].DoImport;
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
			if(Rows[e.Row].IsSeparator) {
				return;
			}
			if(!IsEditable(Rows[e.Row])){
				return;
			}
			if(Rows[e.Row].FieldName=="referredFrom") {
				using FormReferralSelect formRS=new FormReferralSelect();
				formRS.IsSelectionMode=true;
				formRS.ShowDialog();
				if(formRS.DialogResult!=DialogResult.OK) {
					return;
				}
				Referral referralSelected=formRS.SelectedReferral;
				Rows[e.Row].DoImport=true;
				Rows[e.Row].IsFlaggedImp=false;
				Rows[e.Row].ImpValDisplay=referralSelected.GetNameFL();
				Rows[e.Row].ImpValObj=referralSelected;
			}
			#region string
			else if(Rows[e.Row].ObjType==typeof(string)) {
				InputBox inputbox;
				if(ListTools.In(Rows[e.Row].FieldName,"WkPhone","WirelessPhone","HmPhone","ins1CarrierPhone","ins2CarrierPhone","ICEPhone")) {
					InputBoxParam iBoxParam = new InputBoxParam(InputBoxType.ValidPhone,Rows[e.Row].FieldName,"",new Size(100,20));
					inputbox=new InputBox(null,iBoxParam);
				}
				else {
					inputbox=new InputBox(Rows[e.Row].FieldName);
				}
				inputbox.textResult.Text=Rows[e.Row].ImpValDisplay;
				inputbox.ShowDialog();
				if(inputbox.DialogResult!=DialogResult.OK) {
					inputbox.Dispose();
					return;
				}
				if(Rows[e.Row].FieldName=="addressAndHmPhoneIsSameEntireFamily") {
					if(inputbox.textResult.Text=="") {
						AddressSameForFam=false;
					}
					else if(inputbox.textResult.Text!="X") {
						AddressSameForFam=true;
					}
					else {
						MsgBox.Show(this,"The only allowed values are X or blank.");
						inputbox.Dispose();
						return;
					}
				}
				if(Rows[e.Row].OldValDisplay==inputbox.textResult.Text) {//value is now same as original
					Rows[e.Row].DoImport=false;
				}
				else {
					Rows[e.Row].DoImport=true;
				}
				Rows[e.Row].ImpValDisplay=inputbox.textResult.Text;
				Rows[e.Row].ImpValObj=inputbox.textResult.Text;
				inputbox.Dispose();
			}
			#endregion
			#region Enum
			else if(Rows[e.Row].ObjType.IsEnum) {
				//Note.  This only works for zero-indexed enums.
				using FormSheetImportEnumPicker formEnum=new FormSheetImportEnumPicker(Rows[e.Row].FieldName);
				for(int i=0;i<Enum.GetNames(Rows[e.Row].ObjType).Length;i++) {
					formEnum.listResult.Items.Add(Enum.GetNames(Rows[e.Row].ObjType)[i]);
					if(Rows[e.Row].ImpValObj!=null && i==(int)Rows[e.Row].ImpValObj) {
						formEnum.listResult.SelectedIndex=i;
					}
				}
				formEnum.ShowDialog();
				if(formEnum.DialogResult==DialogResult.OK) {
					int selectedI=formEnum.listResult.SelectedIndex;
					if(Rows[e.Row].ImpValObj==null) {//was initially null
						if(selectedI!=-1) {//an item was selected
							Rows[e.Row].ImpValObj=Enum.ToObject(Rows[e.Row].ObjType,selectedI);
							Rows[e.Row].ImpValDisplay=Rows[e.Row].ImpValObj.ToString();
						}
					}
					else {//was not initially null
						if((int)Rows[e.Row].ImpValObj!=selectedI) {//value was changed.
							//There's no way for the user to set it to null, so we do not need to test that
							Rows[e.Row].ImpValObj=Enum.ToObject(Rows[e.Row].ObjType,selectedI);
							Rows[e.Row].ImpValDisplay=Rows[e.Row].ImpValObj.ToString();
						}
					}
					if(selectedI==-1) {
						Rows[e.Row].DoImport=false;//impossible to import a null
					}
					else if(Rows[e.Row].OldValObj!=null && (int)Rows[e.Row].ImpValObj==(int)Rows[e.Row].OldValObj) {//it's the old setting for the patient, whether or not they actually changed it.
						Rows[e.Row].DoImport=false;//so no need to import
					}
					else {
						Rows[e.Row].DoImport=true;
					}
				}
			}
			#endregion
			#region DateTime
			else if(Rows[e.Row].ObjType==typeof(DateTime)) {//this is only for one field so far: Birthdate				
				using InputBox inputbox=new InputBox(new List<InputBoxParam>() {
					//Display the date format that the current computer will use when parsing the date.
					new InputBoxParam(InputBoxType.ValidDate,Rows[e.Row].FieldName)
				});
				//Display the importing date if valid, otherwise display the invalid date from the form.
				inputbox.textResult.Text=(Rows[e.Row].ImpValDisplay==INVALID_DATE) ? Rows[e.Row].NewValDisplay : Rows[e.Row].ImpValDisplay;
				inputbox.ShowDialog();
				if(inputbox.DialogResult!=DialogResult.OK) {
					return;
				}
				DateTime enteredDate=inputbox.DateEntered;
				if(enteredDate==DateTime.MinValue) {
					Rows[e.Row].ImpValDisplay="";
				}
				else if(enteredDate.Year<1880 || enteredDate.Year>2050) {
					MsgBox.Show(this,INVALID_DATE);
					return;
				}
				else {
					Rows[e.Row].ImpValDisplay=enteredDate.ToShortDateString();
				}
				Rows[e.Row].ImpValObj=enteredDate;
				if(Rows[e.Row].ImpValDisplay==Rows[e.Row].OldValDisplay) {//value is now same as original
					Rows[e.Row].DoImport=false;
				}
				else {
					Rows[e.Row].DoImport=true;
				}
			}
			#endregion
			#region Medication, Allergy or Disease
			else if(Rows[e.Row].ObjType==typeof(MedicationPat)
				|| Rows[e.Row].ObjType==typeof(Allergy)
				|| Rows[e.Row].ObjType==typeof(Disease)) 
			{
				//User entered medications will have a MedicationNum as the ImpValObj.
				if(Rows[e.Row].ImpValObj.GetType()==typeof(long)) {
					using FormMedications FormM=new FormMedications();
					FormM.IsSelectionMode=true;
					FormM.textSearch.Text=Rows[e.Row].FieldName.Trim();
					FormM.ShowDialog();
					if(FormM.DialogResult!=DialogResult.OK) {
						return;
					}
					Rows[e.Row].ImpValDisplay="Y";
					Rows[e.Row].ImpValObj=FormM.SelectedMedicationNum;
					string descript=Medications.GetDescription(FormM.SelectedMedicationNum);
					Rows[e.Row].FieldDisplay=descript;
					((SheetField)Rows[e.Row].NewValObj).FieldValue=descript;
					Rows[e.Row].NewValDisplay="Y";
					Rows[e.Row].DoImport=true;
					Rows[e.Row].IsFlaggedImp=false;
				}
				else {
					using FormSheetImportEnumPicker FormIEP=new FormSheetImportEnumPicker(Rows[e.Row].FieldName);
					FormIEP.listResult.Items.AddEnums<YN>();
					FormIEP.listResult.SelectedIndex=0;//Unknown
					if(Rows[e.Row].ImpValDisplay=="Y") {
						FormIEP.listResult.SelectedIndex=1;
					}
					if(Rows[e.Row].ImpValDisplay=="N") {
						FormIEP.listResult.SelectedIndex=2;
					}
					FormIEP.ShowDialog();
					if(FormIEP.DialogResult!=DialogResult.OK) {
						return;
					}
					int selectedI=FormIEP.listResult.SelectedIndex;
					switch(selectedI) {
						case 0:
							Rows[e.Row].ImpValDisplay="";
							break;
						case 1:
							Rows[e.Row].ImpValDisplay="Y";
							break;
						case 2:
							Rows[e.Row].ImpValDisplay="N";
							break;
					}
					if(Rows[e.Row].OldValDisplay==Rows[e.Row].ImpValDisplay) {//value is now same as original
						Rows[e.Row].DoImport=false;
					}
					else {
						Rows[e.Row].DoImport=true;
					}
					if(selectedI==-1 || selectedI==0) {
						Rows[e.Row].DoImport=false;
					}
				}
			}
			#endregion
			#region Subscriber
			else if(Rows[e.Row].ObjType==typeof(Patient)) {
				Patient subscriber=new Patient();
				using FormSubscriberSelect FormSS=new FormSubscriberSelect(Fam);
				FormSS.ShowDialog();
				if(FormSS.DialogResult!=DialogResult.OK) {
					return;
				}
				subscriber=Patients.GetPat(FormSS.SelectedPatNum);
				if(subscriber==null) {
					return;//Should never happen but is a possibility.
				}
				//Use GetNameFirst() because this is how OldValDisplay is displayed.
				string patName=Patients.GetNameFirst(subscriber.FName,subscriber.Preferred);
				if(Rows[e.Row].OldValDisplay==patName) {
					Rows[e.Row].DoImport=false;
				}
				else {
					Rows[e.Row].DoImport=true;
				}
				Rows[e.Row].ImpValDisplay=patName;
				Rows[e.Row].ImpValObj=subscriber;
			}
			#endregion
			#region Carrier
			else if(Rows[e.Row].ObjType==typeof(Carrier)) {
				//Change both carrier rows at the same time.
				string insStr="ins1";
				if(Rows[e.Row].FieldName.StartsWith("ins2")) {
					insStr="ins2";
				}
				SheetImportRow carrierNameRow=GetImportRowByFieldName(insStr+"CarrierName");
				SheetImportRow carrierPhoneRow=GetImportRowByFieldName(insStr+"CarrierPhone");
				Carrier carrier=new Carrier();
				using FormCarriers FormC=new FormCarriers();
				FormC.IsSelectMode=true;
				if(carrierNameRow!=null) {
					FormC.textCarrier.Text=carrierNameRow.NewValDisplay;
				}
				if(carrierPhoneRow!=null) {
					FormC.textPhone.Text=carrierPhoneRow.NewValDisplay;
				}
				FormC.ShowDialog();
				if(FormC.DialogResult!=DialogResult.OK) {
					return;
				}
				carrier=FormC.CarrierSelected;
				//Check for nulls because the name AND phone rows might not both be on the sheet.
				if(carrierNameRow!=null) {
					if(carrierNameRow.OldValDisplay==carrier.CarrierName) {
						carrierNameRow.DoImport=false;
					}
					else {
						carrierNameRow.DoImport=true;
					}
					carrierNameRow.ImpValDisplay=carrier.CarrierName;
					carrierNameRow.ImpValObj=carrier;
				}
				if(carrierPhoneRow!=null) {
					if(carrierPhoneRow.OldValDisplay==carrier.Phone) {
						carrierPhoneRow.DoImport=false;
					}
					else {
						carrierPhoneRow.DoImport=true;
					}
					carrierPhoneRow.ImpValDisplay=carrier.Phone;
					carrierPhoneRow.ImpValObj=carrier;
				}
			}
			#endregion
			FillGrid();
		}

		///<summary>Correctly sets the class wide boolean HasRequiredInsFields.  Loops through all the fields on the sheet and makes sure all the required insurance fields needed to import are present for primary OR for secondary insurance.  If some required fields are missing for an insurance, all related ins fields will have DoImport set to false.  Called after the list "Rows" has been filled.</summary>
		private void SetHasRequiredInsFields() {
			//Start off assuming that neither primary nor secondary have the required insurance fields necessary to import insurance.
			HasRequiredInsFields=false;
			if(CheckSheetForInsFields(true)) {//Check primary fields.
				HasRequiredInsFields=true;
			}
			else {//Sheet does not have the required fields to import primary insurance.
				SetDoImportToFalseForIns(true);//Unmark all primary ins fields for import.
			}
			if(CheckSheetForInsFields(false)) {//Check secondary fields.
				HasRequiredInsFields=true;
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
			SheetImportRow relationRow=GetImportRowByFieldName(insStr+"Relat");
			SheetImportRow subscriberRow=GetImportRowByFieldName(insStr+"Subscriber");
			SheetImportRow subscriberIdRow=GetImportRowByFieldName(insStr+"SubscriberID");
			SheetImportRow carrierNameRow=GetImportRowByFieldName(insStr+"CarrierName");
			SheetImportRow carrierPhoneRow=GetImportRowByFieldName(insStr+"CarrierPhone");
			//Check if all of the required insurance fields exist on this sheet.
			if(relationRow==null 
		    || subscriberRow==null
		    || subscriberIdRow==null
		    || carrierNameRow==null
		    || carrierPhoneRow==null) 
			{
				return false;
			}
			return true;
		}

		///<summary>Loops through the related ins fields and forces DoImport to false.</summary>
		private void SetDoImportToFalseForIns(bool isPrimary) {
			bool changed=false;
			string insStr="ins1";
			if(!isPrimary) {
				insStr="ins2";
			}
			//Only five ins fields have the possibility of DoImport being automatically set to true.  The others require a double click.
			SheetImportRow relationRow=GetImportRowByFieldName(insStr+"Relat");
			SheetImportRow subscriberIdRow=GetImportRowByFieldName(insStr+"SubscriberID");
			SheetImportRow employerNameRow=GetImportRowByFieldName(insStr+"EmployerName");
			SheetImportRow groupNameRow=GetImportRowByFieldName(insStr+"GroupName");
			SheetImportRow groupNumRow=GetImportRowByFieldName(insStr+"GroupNum");
			if(relationRow!=null) {
				relationRow.DoImport=false;
				changed=true;
			}
			if(subscriberIdRow!=null) {
				subscriberIdRow.DoImport=false;
				changed=true;
			}
			if(employerNameRow!=null) {
				employerNameRow.DoImport=false;
				changed=true;
			}
			if(groupNameRow!=null) {
				groupNameRow.DoImport=false;
				changed=true;
			}
			if(groupNumRow!=null) {
				groupNumRow.DoImport=false;
				changed=true;
			}
			if(changed) {
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
			SheetImportRow relationRow=GetImportRowByFieldName(insStr+"Relat");
			SheetImportRow subscriberRow=GetImportRowByFieldName(insStr+"Subscriber");
			SheetImportRow subscriberIdRow=GetImportRowByFieldName(insStr+"SubscriberID");
			SheetImportRow carrierNameRow=GetImportRowByFieldName(insStr+"CarrierName");
			SheetImportRow carrierPhoneRow=GetImportRowByFieldName(insStr+"CarrierPhone");
			SheetImportRow employerNameRow=GetImportRowByFieldName(insStr+"EmployerName");
			SheetImportRow groupNameRow=GetImportRowByFieldName(insStr+"GroupName");
			SheetImportRow groupNumRow=GetImportRowByFieldName(insStr+"GroupNum");
			//Check if the required insurance fields exist on this sheet.
			//NOTE: Employer, group name and group num are optional fields.
			//Checking for nulls in the required fields still needs to be here in this method in case the user has the required fields for one insurance plan but not enough for the other.  They will hit this code ONLY if they have flagged one of the fields on the "other" insurance plan for import that does not have all of the required fields.
			if(relationRow==null 
				|| subscriberRow==null
				|| subscriberIdRow==null
				|| carrierNameRow==null
				|| carrierPhoneRow==null) 
			{
				MessageBox.Show(Lan.g(this,"Required ")+insWarnStr+Lan.g(this," fields are missing on this sheet.  You cannot import ")+insWarnStr
					+Lan.g(this," with this sheet until it contains all of required fields.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone."));
				return false;
			}
			if(relationRow.ImpValObj==null 
				|| subscriberRow.ImpValObj==null
				|| (string)subscriberIdRow.ImpValObj==""
				|| carrierNameRow.ImpValObj==null
				|| carrierPhoneRow.ImpValObj==null) {
				MessageBox.Show(Lan.g(this,"Cannot import ")+insWarnStr+Lan.g(this," until all required fields have been set.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone."));
				return false;
			}
			InsPlan plan=null;
			InsSub sub=null;
			long insSubNum=0;
			long employerNum=0;
			//Get the employer from the db.  If no matching employer found, a new one will automatically get created.
			if(employerNameRow!=null && employerNameRow.ImpValDisplay.Trim()!="") {
				employerNum=Employers.GetEmployerNum(employerNameRow.ImpValDisplay);
			}
			Patient subscriber=(Patient)subscriberRow.ImpValObj;
			//Have user pick a plan------------------------------------------------------------------------------------------------------------
			bool planIsNew=false;
			List<InsSub> subList=InsSubs.GetListForSubscriber(subscriber.PatNum);
			using FormInsPlans FormIP=new FormInsPlans();
			FormIP.carrierText=carrierNameRow.ImpValDisplay;
			if(employerNameRow!=null) {
				FormIP.empText=employerNameRow.ImpValDisplay;
			}
			if(groupNameRow!=null) {
				FormIP.groupNameText=groupNameRow.ImpValDisplay;
			}
			if(groupNumRow!=null) {
				FormIP.groupNumText=groupNumRow.ImpValDisplay;
			}
			FormIP.IsSelectMode=true;
			FormIP.ShowDialog();
			if(FormIP.DialogResult!=DialogResult.OK) {
				return false;
			}
			plan=FormIP.SelectedPlan;
			if(plan.PlanNum==0) {
				//User clicked blank plan, so a new plan will be created using the import values.
				planIsNew=true;
			}
			else {//An existing plan was selected so see if the plan is already subscribed to by the subscriber or create a new inssub.  Patplan will be taken care of later.
				for(int i=0;i<subList.Count;i++) {
					if(subList[i].PlanNum==plan.PlanNum) {
						sub=subList[i];
						insSubNum=sub.InsSubNum;
					}
				}
				if(sub==null) {//Create a new inssub if subscriber is not subscribed to this plan yet.
					sub=new InsSub();
					sub.PlanNum=plan.PlanNum;
					sub.Subscriber=subscriber.PatNum;
					sub.SubscriberID=subscriberIdRow.ImpValDisplay;
					sub.ReleaseInfo=true;
					sub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
					insSubNum=InsSubs.Insert(sub);
				}
			}
			//User picked a plan but the information they want to import might be different than the chosen plan.  Give them options to use current values or created a new plan.
			//It's still okay to let the user return at this point in order to change importing information.
			DialogResult result;
			//Carrier check-----------------------------------------------------------------------------------------
			if(!planIsNew && plan.CarrierNum!=((Carrier)carrierNameRow.ImpValObj).CarrierNum) {
				result=InsuranceImportQuestion("carrier",isPrimary);
				//Yes means the user wants to keep the information on the plan they picked, nothing to do.
				if(result==DialogResult.No) {
					planIsNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Employer check----------------------------------------------------------------------------------------
			if(!planIsNew && employerNum>0 && plan.EmployerNum!=employerNum) {
				result=InsuranceImportQuestion("employer",isPrimary);
				if(result==DialogResult.No) {
					planIsNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Subscriber check--------------------------------------------------------------------------------------
			if(!planIsNew && sub.Subscriber!=((Patient)subscriberRow.ImpValObj).PatNum) {
				result=InsuranceImportQuestion("subscriber",isPrimary);
				if(result==DialogResult.No) {
					planIsNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			if(!planIsNew && sub.SubscriberID!=subscriberIdRow.ImpValDisplay) {
				result=InsuranceImportQuestion("subscriber id",isPrimary);
				if(result==DialogResult.No) {
					planIsNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Group name check--------------------------------------------------------------------------------------
			if(groupNameRow!=null && !planIsNew && plan.GroupName!=groupNameRow.ImpValDisplay) {
				result=InsuranceImportQuestion("group name",isPrimary);
				if(result==DialogResult.No) {
					planIsNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Group num check---------------------------------------------------------------------------------------
			if(groupNumRow!=null && !planIsNew && plan.GroupNum!=groupNumRow.ImpValDisplay) {
				result=InsuranceImportQuestion("group num",isPrimary);
				if(result==DialogResult.No) {
					planIsNew=true;
				}
				if(result==DialogResult.Cancel) {
					return false;
				}
			}
			//Create a new plan-------------------------------------------------------------------------------------
			if(planIsNew) {
				plan=new InsPlan();
				if(employerNum>0) {
					plan.EmployerNum=employerNum;
				}
				plan.PlanType=Prefs.GetBoolNoCache(PrefName.InsDefaultPPOpercent) ? "p" : "";
				plan.CarrierNum=((Carrier)carrierNameRow.ImpValObj).CarrierNum;
				if(groupNameRow!=null) {
					plan.GroupName=groupNameRow.ImpValDisplay;
				}
				if(groupNumRow!=null) {
					plan.GroupNum=groupNumRow.ImpValDisplay;
				}
				InsPlans.Insert(plan);
				sub=new InsSub();
				sub.PlanNum=plan.PlanNum;
				sub.Subscriber=subscriber.PatNum;
				sub.SubscriberID=subscriberIdRow.ImpValDisplay;
				sub.ReleaseInfo=true;
				sub.AssignBen=PrefC.GetBool(PrefName.InsDefaultAssignBen);
				insSubNum=InsSubs.Insert(sub);
				Benefit ben;
				List<CovCat> listCovCats=CovCats.GetDeepCopy(true);
				for(int i=0;i<listCovCats.Count;i++) {
					if(listCovCats[i].DefaultPercent==-1) {
						continue;
					}
					ben=new Benefit();
					ben.BenefitType=InsBenefitType.CoInsurance;
					ben.CovCatNum=listCovCats[i].CovCatNum;
					ben.PlanNum=plan.PlanNum;
					ben.Percent=listCovCats[i].DefaultPercent;
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
					ben.CodeNum=0;
					Benefits.Insert(ben);
				}
				//Zero deductible diagnostic
				if(CovCats.GetForEbenCat(EbenefitCategory.Diagnostic)!=null) {
					ben=new Benefit();
					ben.CodeNum=0;
					ben.BenefitType=InsBenefitType.Deductible;
					ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
					ben.PlanNum=plan.PlanNum;
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
					ben.MonetaryAmt=0;
					ben.Percent=-1;
					ben.CoverageLevel=BenefitCoverageLevel.Individual;
					Benefits.Insert(ben);
				}
				//Zero deductible preventive
				if(CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive)!=null) {
					ben=new Benefit();
					ben.CodeNum=0;
					ben.BenefitType=InsBenefitType.Deductible;
					ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
					ben.PlanNum=plan.PlanNum;
					ben.TimePeriod=BenefitTimePeriod.CalendarYear;
					ben.MonetaryAmt=0;
					ben.Percent=-1;
					ben.CoverageLevel=BenefitCoverageLevel.Individual;
					Benefits.Insert(ben);
				}
			}
			//Delete the old patplan-------------------------------------------------------------------------------------------------------------
			if(isPrimary && PatPlan1!=null) {//Importing primary and currently has primary ins.
				PatPlans.DeleteNonContiguous(PatPlan1.PatPlanNum);
			}
			if(!isPrimary && PatPlan2!=null) {//Importing secondary and currently has secondary ins.
				PatPlans.DeleteNonContiguous(PatPlan2.PatPlanNum);
			}
			//Then attach new patplan to the plan------------------------------------------------------------------------------------------------
			PatPlan patplan=new PatPlan();
			patplan.Ordinal=ordinal;//Not allowed to be 0.
			patplan.PatNum=PatCur.PatNum;
			patplan.InsSubNum=insSubNum;
			patplan.Relationship=((Relat)relationRow.ImpValObj);
			PatPlans.Insert(patplan);
			//After new plan has been imported, recompute all estimates for this patient because their coverage is now different.  Also set patient.HasIns to the correct value.
			List<ClaimProc> claimProcs=ClaimProcs.Refresh(PatCur.PatNum);
			List<Procedure> procs=Procedures.Refresh(PatCur.PatNum);
			List<PatPlan> patPlans=PatPlans.Refresh(PatCur.PatNum);
			subList=InsSubs.RefreshForFam(Fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			List<Benefit> benList=Benefits.Refresh(patPlans,subList);
			Procedures.ComputeEstimatesForAll(PatCur.PatNum,claimProcs,procs,planList,patPlans,benList,PatCur.Age,subList);
			Patients.SetHasIns(PatCur.PatNum);
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
			if(Security.IsAuthorized(Permissions.InsPlanEdit,true)) {
				msgBoxButton=MessageBoxButtons.YesNoCancel;
				createNewPlanMsg=$"\r\n\r\n{Lan.g(this,"No will create a new plan using all of the import values.")}";
			}
			return MessageBox.Show(Lan.g(this,"The ")+insStr+importValue+Lan.g(this," does not match the selected plan's ")+importValue+".\r\n"
				+Lan.g(this,"Use the selected plan's ")+importValue+"?"
				+createNewPlanMsg,Lan.g(this,"Import ")+insStr+importValue,msgBoxButton);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!Rows.Any(x => x.DoImport)) {
				MsgBox.Show(this,"No rows are set for import.");
				return;
			}
			if(Rows.Any(x => x.DoImport && x.ImpValObj is DateTime && x.ImpValDisplay==INVALID_DATE)) {
				MsgBox.Show(this,$"Please fix data entry errors first (ImportValues with '{INVALID_DATE}').");
				return;
			}
			#region Patient Form
			if(SheetCur.SheetType==SheetTypeEnum.PatientForm) {
				bool importPriIns=false;
				bool importSecIns=false;
				for(int i=0;i<Rows.Count;i++) {
					if(!Rows[i].DoImport) {
						continue;
					}
					//Importing insurance happens later.
					if(Rows[i].FieldName.StartsWith("ins1")) {
						importPriIns=true;
						continue;
					}
					if(Rows[i].FieldName.StartsWith("ins2")) {
						importSecIns=true;
						continue;
					}
					switch(Rows[i].FieldName) {
						#region Personal
						case "LName":
							PatCur.LName=Rows[i].ImpValDisplay;
							break;
						case "FName":
							PatCur.FName=Rows[i].ImpValDisplay;
							break;
						case "MiddleI":
							PatCur.MiddleI=Rows[i].ImpValDisplay;
							break;
						case "Preferred":
							PatCur.Preferred=Rows[i].ImpValDisplay;
							break;
						case "Gender":
							PatCur.Gender=(PatientGender)Rows[i].ImpValObj;
							break;
						case "Position":
							PatCur.Position=(PatientPosition)Rows[i].ImpValObj;
							break;
						case "Birthdate":
							PatCur.Birthdate=(DateTime)Rows[i].ImpValObj;
							break;
						case "SSN":
							PatCur.SSN=Rows[i].ImpValDisplay;
							break;
						case "WkPhone":
							PatCur.WkPhone=Rows[i].ImpValDisplay;
							break;
						case "WirelessPhone":
							PatCur.WirelessPhone=Rows[i].ImpValDisplay;
							break;
						case "Email":
							PatCur.Email=Rows[i].ImpValDisplay;
							break;
						case "PreferContactMethod":
							PatCur.PreferContactMethod=(ContactMethod)Rows[i].ImpValObj;
							break;
						case "PreferConfirmMethod":
							PatCur.PreferConfirmMethod=(ContactMethod)Rows[i].ImpValObj;
							break;
						case "PreferRecallMethod":
							PatCur.PreferRecallMethod=(ContactMethod)Rows[i].ImpValObj;
							break;
						case "referredFrom":
							RefAttach ra=new RefAttach();
							ra.RefType=ReferralType.RefFrom;
							ra.ItemOrder=1;
							ra.PatNum=PatCur.PatNum;
							ra.RefDate=DateTime.Today;
							ra.ReferralNum=((Referral)Rows[i].ImpValObj).ReferralNum;
							RefAttaches.Insert(ra);//no security to block this action.
							SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatCur.PatNum,"Referred From "+Referrals.GetNameFL(ra.ReferralNum));
							break;
						#endregion
						#region Address and Home Phone
						//AddressSameForFam already set, but not really importable by itself
						case "Address":
							PatCur.Address=Rows[i].ImpValDisplay;
							break;
						case "Address2":
							PatCur.Address2=Rows[i].ImpValDisplay;
							break;
						case "City":
							PatCur.City=Rows[i].ImpValDisplay;
							break;
						case "State":
							PatCur.State=Rows[i].ImpValDisplay;
							break;
						case "Zip":
							PatCur.Zip=Rows[i].ImpValDisplay;
							break;
						case "HmPhone":
							PatCur.HmPhone=Rows[i].ImpValDisplay;
							break;
						#endregion
					}
				}
				//Insurance importing happens before updating the patient information because there is a possibility of returning for more information.
				if(HasRequiredInsFields) {//Do not attempt to import any insurance unless they have the required fields for importing.
					#region Insurance importing
					bool primaryImported=false;
					if(importPriIns) {//A primary insurance field was flagged for importing.
						if(!ValidateAndImportInsurance(true)) {
							//Field missing or user chose to back out to correct information.
							return;//Nothing has been updated so it's okay to just return here.
						}
						primaryImported=true;
					}
					if(importSecIns) {//A secondary insurance field was flagged for importing.
						if(!ValidateAndImportInsurance(false)) {
							//Field missing or user chose to back out to correct information.
							if(primaryImported) {
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
					if(importPriIns) {//The user has manually flagged a primary ins row for importing.
						MsgBox.Show(this,"Required primary insurance fields are missing on this sheet.  You cannot import primary insurance with this sheet until it contains all of required fields.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone.");
					}
					if(importSecIns) {//The user has manually flagged a secondary ins row for importing.
						MsgBox.Show(this,"Required secondary insurance fields are missing on this sheet.  You cannot import secondary insurance with this sheet until it contains all of required fields.  Required fields: Relationship, Subscriber, SubscriberID, CarrierName, and CarrierPhone.");
					}
				}
				//Patient information updating---------------------------------------------------------------------------------------------------------
				Patients.Update(PatCur,PatOld);
				if(AddressSameForFam) {
					bool isAuthArchivedEdit=Security.IsAuthorized(Permissions.ArchivedPatientEdit,true);
					if(!isAuthArchivedEdit && Fam.HasArchivedMember()) {
						MessageBox.Show(Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(Permissions.ArchivedPatientEdit)+"\r\n"
							+Lans.g(this,"Archived patients in the family will not be updated.  All other family members will be updated as usual."));
					}
					Patients.UpdateAddressForFam(PatCur,false,isAuthArchivedEdit);
				}
			}
			#endregion
			#region Medical History
			else if(SheetCur.SheetType==SheetTypeEnum.MedicalHistory) {
				for(int i=0;i<Rows.Count;i++) {
					if(!Rows[i].DoImport) {
						continue;
					}
					if(Rows[i].ObjType==null) {//Should never happen.
						continue;
					}
					YN hasValue=YN.Unknown;
					if(Rows[i].ImpValDisplay=="Y") {
						hasValue=YN.Yes;
					}
					if(Rows[i].ImpValDisplay=="N") {
						hasValue=YN.No;
					}
					if(hasValue==YN.Unknown) {//Unknown, nothing to do.
						continue;
					}
					#region Allergies
					if(Rows[i].ObjType==typeof(Allergy)) {
						//Patient has this allergy in the db so just update the value.
						if(Rows[i].OldValObj!=null) {
							Allergy oldAllergy=(Allergy)Rows[i].OldValObj;
							if(hasValue==YN.Yes) {
								oldAllergy.StatusIsActive=true;
							}
							else {
								oldAllergy.StatusIsActive=false;
							}
							Allergies.Update(oldAllergy);
							continue;
						}
						if(hasValue==YN.No) {//We never import allergies with inactive status.
							continue;
						}
						//Allergy does not exist for this patient yet so create one.
						List<AllergyDef> allergyList=AllergyDefs.GetAll(false);
						SheetField allergySheet=(SheetField)Rows[i].NewValObj;
						//Find what allergy user wants to import.
						for(int j=0;j<allergyList.Count;j++) {
							if(allergyList[j].Description==allergySheet.FieldName.Remove(0,8)) {
								Allergy newAllergy=new Allergy();
								newAllergy.AllergyDefNum=allergyList[j].AllergyDefNum;
								newAllergy.PatNum=PatCur.PatNum;
								newAllergy.StatusIsActive=true;
								Allergies.Insert(newAllergy);
								break;
							}
						}
					}
					#endregion
					#region Medications
					else if(Rows[i].ObjType==typeof(MedicationPat)) {
					  //Patient has this medication in the db so leave it alone or set the stop date.
					  if(Rows[i].OldValObj!=null) {
					    //Set the stop date for the current medication(s).
					    MedicationPat oldMedPat=(MedicationPat)Rows[i].OldValObj;
					    if(hasValue==YN.Yes) {
								if(!MedicationPats.IsMedActive(oldMedPat)) {
									oldMedPat.DateStop=new DateTime(0001,1,1);//This will activate the med.
					      }
					    }
							else {
								oldMedPat.DateStop=DateTime.Today;//Set the med as inactive.
							}
							MedicationPats.Update(oldMedPat);
					    continue;
					  }
						if(hasValue==YN.No) {//Don't import medications with inactive status.
							continue;
						}
					  //Medication does not exist for this patient yet so create it.
					  List<Medication> medList=Medications.GetList("");
					  SheetField medSheet=(SheetField)Rows[i].NewValObj;
					  //Find what medication user wants to import.
					  for(int j=0;j<medList.Count;j++) {
					    if(Medications.GetDescription(medList[j].MedicationNum)==medSheet.FieldValue) {
					      MedicationPat medPat=new MedicationPat();
					      medPat.PatNum=PatCur.PatNum;
					      medPat.MedicationNum=medList[j].MedicationNum;
					      MedicationPats.Insert(medPat);
					      break;
					    }
					  }
					}
					#endregion
					#region Diseases
					else if(Rows[i].ObjType==typeof(Disease)) {
						//Patient has this problem in the db so just update the value.
						if(Rows[i].OldValObj!=null) {
							Disease oldDisease=(Disease)Rows[i].OldValObj;
							if(hasValue==YN.Yes) {
								oldDisease.ProbStatus=ProblemStatus.Active;
							}
							else {
								oldDisease.ProbStatus=ProblemStatus.Inactive;
							}
							Diseases.Update(oldDisease);
							continue;
						}
						if(hasValue==YN.No) {//Don't create new problem with inactive status.
							continue;
						}
						//Problem does not exist for this patient yet so create one.
						SheetField diseaseSheet=(SheetField)Rows[i].NewValObj;
						List<DiseaseDef> listDiseaseDefs=DiseaseDefs.GetDeepCopy(true);
						//Find what allergy user wants to import.
						for(int j=0;j<listDiseaseDefs.Count;j++) {
							if(listDiseaseDefs[j].DiseaseName==diseaseSheet.FieldName.Remove(0,8)) {
								Disease newDisease=new Disease();
								newDisease.PatNum=PatCur.PatNum;
								newDisease.DiseaseDefNum=listDiseaseDefs[j].DiseaseDefNum;
								newDisease.ProbStatus=ProblemStatus.Active;
								Diseases.Insert(newDisease);
								break;
							}
						}
					}
					#endregion
				}
			}
			#endregion
			#region ICE
			List<SheetImportRow> listFieldsIce = Rows.FindAll(x => x.FieldName.StartsWith("ICE") && x.DoImport);
			if(listFieldsIce.Count>0) {
				foreach(SheetImportRow row in listFieldsIce) {
					switch(row.FieldName) {
						case "ICEName":
							PatNoteCur.ICEName=row.ImpValDisplay;
							break;
						case "ICEPhone":
							PatNoteCur.ICEPhone=row.ImpValDisplay;
							break;
						default:
							break;
					}
				}
				PatientNotes.Update(PatNoteCur,PatCur.Guarantor);
			}
			#endregion
			string logText=SecurityLogHelper();
			if(logText!="") {
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,PatCur.PatNum,logText,0,LogSources.None,DateTime.Now);
			}
			MsgBox.Show(this,"Done.");
			DialogResult=DialogResult.OK;
		}

		///<summary>Returns a string that will be empty if nothing is imported, or if there are no edited fields. Compares 'Current Value' and 'Import Value'
		///columns to determine if there is a change. If changes are being imported will return a security log string including the changes.</summary>
		private string SecurityLogHelper() {
			StringBuilder logBuilder=new StringBuilder("");
			logBuilder.Append($"{Security.CurUser.UserName} imported {Patients.GetNameFL(PatCur.PatNum)}. The following fields were changed: \r\n");
			int indexDoImport=gridMain.ListGridColumns.GetIndex("Do Import");//Denotes an imported field with 'X'.
			int indexFieldName=gridMain.ListGridColumns.GetIndex("FieldName");
			int indexCurrentValue=gridMain.ListGridColumns.GetIndex("Current Value");
			int indexImportValue=gridMain.ListGridColumns.GetIndex("Import Value");
			//Grabbing all of the GridRows marked for import, denoted with an 'X'.
			List<GridRow> listEditedGridRows=gridMain.ListGridRows.FindAll(x=>x.Cells[indexDoImport].Text=="X");
			//Returns all of the imported GridRows where the Import Value != Current Value.
			listEditedGridRows=listEditedGridRows.FindAll(x=>x.Cells[indexImportValue].Text!=x.Cells[indexCurrentValue].Text);
			if(listEditedGridRows.IsNullOrEmpty()) {
				return "";
			}
			for(int i = 0;i<listEditedGridRows.Count;i++) {
				logBuilder.Append($"{listEditedGridRows[i].Cells[indexFieldName].Text} changed from '{listEditedGridRows[i].Cells[indexCurrentValue].Text}'");
				logBuilder.Append($" to '{listEditedGridRows[i].Cells[indexImportValue].Text}'\r\n");
			}
			return logBuilder.ToString();
		}

		private bool DoImport(string fieldName) {
			for(int i=0;i<Rows.Count;i++) {
				if(Rows[i].FieldName!=fieldName) {
					continue;
				}
				return Rows[i].DoImport;
			}
			return false;
		}

		///<summary>Will return null if field not found or if field marked to not import.</summary>
		private object GetImpObj(string fieldName) {
			for(int i=0;i<Rows.Count;i++) {
				if(Rows[i].FieldName!=fieldName) {
					continue;
				}
				if(!Rows[i].DoImport) {
					return null;
				}
				return Rows[i].ImpValObj;
			}
			return null;
		}

		///<summary>Will return empty string field not found or if field marked to not import.</summary>
		private string GetImpDisplay(string fieldName) {
			for(int i=0;i<Rows.Count;i++) {
				if(Rows[i].FieldName!=fieldName) {
					continue;
				}
				if(!Rows[i].DoImport) {
					return "";
				}
				return Rows[i].ImpValDisplay;
			}
			return "";
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		///<summary>Returns a separator and sets the FieldName to the passed in string.</summary>
		private SheetImportRow CreateSeparator(string displayText) {
			SheetImportRow separator=new SheetImportRow();
			separator.FieldName=displayText;
			separator.IsSeparator=true;
			return separator;
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