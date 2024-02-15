using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using EhrLaboratories;

namespace OpenDental {
	public partial class FormPatListElementEditEHR2014:FormODBase {
		public EhrPatListElement2014 EhrPatListElement2014Cur;
		public bool IsNew;
		public bool DoDelete;
		private List<Ucum> _listUcums;

		public FormPatListElementEditEHR2014() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPatListElementEdit_Load(object sender,EventArgs e) {
			listRestriction.Items.Clear();
			listRestriction.Items.AddEnums<EhrRestrictionType>();
			listRestriction.SetSelectedEnum(EhrPatListElement2014Cur.Restriction);
			listOperand.Items.Clear();
			listOperand.Items.AddEnums<EhrOperand>();
			listOperand.SetSelectedEnum(EhrPatListElement2014Cur.Operand);
			textCompareString.Text=EhrPatListElement2014Cur.CompareString;
			if(EhrPatListElement2014Cur.Restriction==EhrRestrictionType.Problem && !IsNew) {
				textCompareString.Text="";//clear text box for simplicity
				if(ICD9s.CodeExists(EhrPatListElement2014Cur.CompareString)) {
					textCompareString.Text=EhrPatListElement2014Cur.CompareString;
				}
				else if(Snomeds.CodeExists(EhrPatListElement2014Cur.CompareString)) {
					textSNOMED.Text=EhrPatListElement2014Cur.CompareString;
				}
				else {
					MsgBox.Show(this,"Problem code provided is not an existing ICD9 or SNOMED code.");
					//no harm in continuing since this form is error checked on OK click.
				}
			}
			fillCombos();
			if(!IsNew) {
				comboUnits.Text=EhrPatListElement2014Cur.LabValueUnits;
				comboLabValueType.SelectedIndex=(int)EhrPatListElement2014Cur.LabValueType;
			}
			textLabValue.Text=EhrPatListElement2014Cur.LabValue;
			if(EhrPatListElement2014Cur.StartDate.Year>1880) {
				textDateStart.Text=EhrPatListElement2014Cur.StartDate.ToShortDateString();
			}
			if(EhrPatListElement2014Cur.EndDate.Year>1880) {
				textDateStop.Text=EhrPatListElement2014Cur.EndDate.ToShortDateString();
			}
			ChangeLayout();
		}

		private void fillCombos() {
			//Units of measure----------------------------------------
			_listUcums=Ucums.GetAll();
			if(_listUcums.Count==0) {
				MsgBox.Show(this,"Units of measure have not been imported. Go to the code system importer window to import UCUM codes to continue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			int tempSelectedIndex=0;
			for(int i=0;i<_listUcums.Count;i++) {
				comboUnits.Items.Add(_listUcums[i].UcumCode);
				if(_listUcums[i].UcumCode=="mg/dL") {//arbitrarily chosen common unit of measure.
					tempSelectedIndex=i;
				}
			}
			comboUnits.SelectedIndex=tempSelectedIndex;
			//Lab Value Type.  Based off of the HL70125 data type (Value Type)----------------------------------------
			comboLabValueType.Items.Add("Coded Entry");
			comboLabValueType.Items.Add("Coded with Exceptions");
			comboLabValueType.Items.Add("Date");
			comboLabValueType.Items.Add("Formatted Text");
			comboLabValueType.Items.Add("Numeric");//default
			comboLabValueType.Items.Add("Structured Numeric");
			comboLabValueType.Items.Add("String Data");
			comboLabValueType.Items.Add("Time");
			comboLabValueType.Items.Add("Time Stamp");
			comboLabValueType.Items.Add("Text Data");
			comboLabValueType.SelectedIndex=4;//Numeric.  This is what we used to assume all lab results were like.
		}

		private void listRestriction_SelectedIndexChanged(object sender,EventArgs e) {
			ChangeLayout();
		}

		private void ChangeLayout() {
			//All controls' visibility set to false, then set to true below if needed.
			//Labels
			labelOperand.Visible=false;
			labelCompareString.Visible=false;
			labelSNOMED.Visible=false;
			labelLabValue.Visible=false;
			labelAfterDate.Visible=false;
			labelBeforeDate.Visible=false;
			labelExample.Visible=false;
			labelProblemSuggest.Visible=false;
			labelLabValueType.Visible=false;
			//TextBoxes
			textCompareString.Visible=false;
			textSNOMED.Visible=false;
			textLabValue.Visible=false;
			textDateStart.Visible=false;
			textDateStop.Visible=false;
			//Buttons
			butPicker.Visible=false;
			butSNOMED.Visible=false;
			butProblem.Visible=false;
			//ComboBoxes
			comboUnits.Visible=false;
			comboLabValueType.Visible=false;
			//ListBoxes
			listOperand.Visible=false;
			switch(listRestriction.GetSelected<EhrRestrictionType>()) {
				case EhrRestrictionType.Birthdate: 
					//Labels
					labelOperand.Visible=true;
					labelCompareString.Visible=true;
					labelCompareString.Text="Enter age";
					labelExample.Visible=true;
					labelExample.Text="Ex: 22";
					//TextBoxes
					textCompareString.Visible=true;
					//ListBoxes
					listOperand.Visible=true;
					break;
				case EhrRestrictionType.Problem: //Disease
					//Labels
					labelCompareString.Visible=true;
					labelCompareString.Text="ICD9 Code";
					labelSNOMED.Visible=true;
					labelSNOMED.Text="SNOMED Code";
					labelAfterDate.Visible=true;
					labelBeforeDate.Visible=true;
					labelProblemSuggest.Visible=true;
					//TextBoxes
					textCompareString.Visible=true;
					textSNOMED.Visible=true;
					textDateStart.Visible=true;
					textDateStop.Visible=true;
					//Buttons
					butPicker.Visible=true;
					butSNOMED.Visible=true;
					butProblem.Visible=true;
					break;
				case EhrRestrictionType.Medication: 
					//Labels
					labelCompareString.Visible=true;
					labelCompareString.Text="Medication Name";
					labelAfterDate.Visible=true;
					labelBeforeDate.Visible=true;
					labelExample.Visible=true;
					labelExample.Text="Ex: Albuterol";
					//TextBoxes
					textCompareString.Visible=true;
					textDateStart.Visible=true;
					textDateStop.Visible=true;
					//Buttons
					butPicker.Visible=true;
					break;
				case EhrRestrictionType.LabResult: 
					//Labels
					labelOperand.Visible=true;
					labelCompareString.Visible=true;
					labelCompareString.Text="Loinc Code";
					labelLabValue.Visible=true;
					labelAfterDate.Visible=true;
					labelBeforeDate.Visible=true;
					labelExample.Visible=true;
					labelExample.Text="Ex: 1004-1";
					labelLabValueType.Visible=true;
					//TextBoxes
					textCompareString.Visible=true;
					textLabValue.Visible=true;
					textDateStart.Visible=true;
					textDateStop.Visible=true;
					//Buttons
					butPicker.Visible=true;
					//ComboBoxes
					comboUnits.Visible=true;
					comboLabValueType.Visible=true;
					//ListBoxes
					listOperand.Visible=true;
					break;
				case EhrRestrictionType.Gender: 
					//Labels
					labelCompareString.Visible=true;
					labelCompareString.Text="For display and sorting";
					break;
				case EhrRestrictionType.CommPref: 
					//Labels
					labelCompareString.Visible=true;
					labelCompareString.Text="Communication Preference";
					labelExample.Visible=true;
					labelExample.Text="Ex: WirelessPh";
					//TextBoxes
					textCompareString.Visible=true;
					//Buttons
					butPicker.Visible=true;
					break;
				case EhrRestrictionType.Allergy: 
					//Labels
					labelCompareString.Visible=true;
					labelCompareString.Text="Allergy Description (exact)";
					labelAfterDate.Visible=true;
					labelBeforeDate.Visible=true;
					//TextBoxes
					textCompareString.Visible=true;
					textDateStart.Visible=true;
					textDateStop.Visible=true;
					//Buttons
					butPicker.Visible=true;
					break;
				default://Default--------------------------------------------------------------------------------------------------------------
					//Nothing will show
					break;
			}
			#region old code
			//labelCompareString.Visible=true;
			//textCompareString.Visible=true;
			//labelExample.Visible=true;
			//labelLabValue.Visible=false;
			//textLabValue.Visible=false;
			//labelOperand.Visible=false;
			//listOperand.Visible=false;
			//if(listRestriction.SelectedIndex==0) {//Birthdate
			//  labelCompareString.Text="Enter age";
			//  labelExample.Text="Ex: 22";
			//  labelSNOMED.Visible=false;
			//  labelOperand.Visible=true;
			//  listOperand.Visible=true;
			//  butPicker.Visible=false;
			//  labelAfterDate.Visible=false;
			//  labelBeforeDate.Visible=false;
			//  textDateStart.Visible=false;
			//  textDateStop.Visible=false;
			//}
			//if(listRestriction.SelectedIndex==1) {//Disease/Problem
			//  labelCompareString.Text="ICD9 code";
			//  labelCompareString.Text="ICD9 code";
			//  labelExample.Text="Ex: 414.0";
			//  labelSNOMED.Visible=false;
			//  butPicker.Visible=true;
			//  labelAfterDate.Visible=true;
			//  labelBeforeDate.Visible=true;
			//  textDateStart.Visible=true;
			//  textDateStop.Visible=true;
			//}
			//if(listRestriction.SelectedIndex==2) {//Medication
			//  labelCompareString.Text="Medication name";
			//  labelExample.Text="Ex: Coumadin";
			//  labelSNOMED.Visible=false;
			//  butPicker.Visible=true;
			//  labelAfterDate.Visible=true;
			//  labelBeforeDate.Visible=true;
			//  textDateStart.Visible=true;
			//  textDateStop.Visible=true;
			//}
			//if(listRestriction.SelectedIndex==3) {//LabResult
			//  labelCompareString.Text="Test name (exact)";
			//  labelExample.Text="Ex: HDL-cholesterol";
			//  labelSNOMED.Visible=false;
			//  labelLabValue.Visible=true;
			//  textLabValue.Visible=true;
			//  labelOperand.Visible=true;
			//  listOperand.Visible=true;
			//  butPicker.Visible=false;
			//  labelAfterDate.Visible=true;
			//  labelBeforeDate.Visible=true;
			//  textDateStart.Visible=true;
			//  textDateStop.Visible=true;
			//}
			//if(listRestriction.SelectedIndex==4) {//Gender
			//  labelCompareString.Text="For display and sorting";
			//  labelExample.Visible=false;
			//  labelSNOMED.Visible=false;
			//  textCompareString.Visible=false;
			//  butPicker.Visible=false;
			//  labelAfterDate.Visible=false;
			//  labelBeforeDate.Visible=false;
			//  textDateStart.Visible=false;
			//  textDateStop.Visible=false;
			//}
			//if(listRestriction.SelectedIndex==5) {//CommPref
			//  labelCompareString.Text="Communication Method (exact)";
			//  labelExample.Text="Ex: WirelessPh";
			//  labelSNOMED.Visible=false;
			//  textCompareString.Visible=true;
			//  butPicker.Visible=true;
			//  labelAfterDate.Visible=false;
			//  labelBeforeDate.Visible=false;
			//  textDateStart.Visible=false;
			//  textDateStop.Visible=false;
			//}
			//if(listRestriction.SelectedIndex==6) {//Allergy
			//  labelCompareString.Text="Allergy Description (exact)";
			//  labelExample.Visible=false;
			//  textCompareString.Visible=true;
			//  butPicker.Visible=true;
			//  labelAfterDate.Visible=true;
			//  labelBeforeDate.Visible=true;
			//  textDateStart.Visible=true;
			//  textDateStop.Visible=true;
			//}
			#endregion
		}

		private bool IsValid() {
			if(listRestriction.GetSelected<EhrRestrictionType>()!=EhrRestrictionType.LabResult) {
				textLabValue.Text="";
			}
			switch(listRestriction.GetSelected<EhrRestrictionType>()) {
				case EhrRestrictionType.Birthdate:
					try {
						Convert.ToInt32(textCompareString.Text);//used intead of PIn so that an empty string is not evaluated as 0
					}
					catch {
						MsgBox.Show(this,"Please enter a valid age.");
						return false;
					}
					break;
				case EhrRestrictionType.Problem: //Disease
					if(textCompareString.Text=="" && textSNOMED.Text=="") {
						MsgBox.Show(this,"Please enter a valid SNOMED CT or ICD9 code.");
						return false;
					}
					if(textCompareString.Text!="") {
						if(!ICD9s.CodeExists(textCompareString.Text)) {
							MsgBox.Show(this,"ICD9 code does not exist in database, pick from list.");
							return false;
						}
					}
					if(textSNOMED.Text!=""){
						if(!Snomeds.CodeExists(textSNOMED.Text)) {
							MsgBox.Show(this,"SNOMED CT code does not exist in database, pick from list.");
							return false;
						}
					}
					if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
						MsgBox.Show(this,"Please fix date entry errors.");
						return false;
					}
					break;
				case EhrRestrictionType.Medication:
					if(textCompareString.Text=="") {
						MsgBox.Show(this,"Please enter a valid medication.");
						return false;
					}
					if(Medications.GetMedicationFromDbByName(textCompareString.Text)==null) {
						MsgBox.Show(this,"Medication does not exist in database, pick from list.");
						return false;
					}
					if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
						MsgBox.Show(this,"Please fix date entry errors.");
						return false;
					}
					break;
				case EhrRestrictionType.LabResult:
					if(textCompareString.Text=="") {
						MsgBox.Show(this,"Please select a valid Loinc Code.");
						return false;
					}
					//if(Loincs.GetByCode(textCompareString.Text)==null) {
					//	MsgBox.Show(this,"Loinc code does not exist in database, pick from list.");
					//	return false;
					//}
					if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
						MsgBox.Show(this,"Please fix date entry errors.");
						return false;
					}
					break;
				case EhrRestrictionType.Gender:
					textCompareString.Text="";
					break;
				case EhrRestrictionType.CommPref:
					if(textCompareString.Text=="") {
						MsgBox.Show(this,"Please enter a communication preference.");
						return false;
					}
					if(!isContactMethod(textCompareString.Text)){
						MsgBox.Show(this,"Communication preference not defined, pick from list.");
						return false;
					}
					break;
				case EhrRestrictionType.Allergy:
					if(textCompareString.Text=="") {
						MsgBox.Show(this,"Please enter a valid allergy.");
						return false;
					}
					if(AllergyDefs.GetByDescription(textCompareString.Text)==null) {
						MsgBox.Show(this,"Allergy does not exist in database, pick from list.");
						return false;
					}
					if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
						MsgBox.Show(this,"Please fix date entry errors.");
						return false;
					}
					break;
			}
			return true;
		}

		///<summary>Returns true if string matches ContactMethod enum names.</summary>
		private bool isContactMethod(string contactMethodName)
		{
			string[] stringArrayContactNames=Enum.GetNames(typeof(ContactMethod));
			for(int i=0;i<stringArrayContactNames.Length;i++) {
				if(stringArrayContactNames[i]==contactMethodName) {
					return true;
				}
			}
			return false;
		}

		private void butPicker_Click(object sender,EventArgs e) {
			switch(listRestriction.GetSelected<EhrRestrictionType>()) {
				case EhrRestrictionType.Birthdate:
					//Not visible
					break;
				case EhrRestrictionType.Problem:
					if(sender.Equals(butPicker)) {
						using FormIcd9s formIcd9s=new FormIcd9s();
						formIcd9s.IsSelectionMode=true;
						formIcd9s.ShowDialog();
						if(formIcd9s.DialogResult!=DialogResult.OK) {
							return;
						}
						textCompareString.Text=formIcd9s.ICD9Selected.ICD9Code;
						textSNOMED.Text="";
					}
					else if(sender.Equals(butSNOMED)) {
						using FormSnomeds formSnomeds=new FormSnomeds();
						formSnomeds.IsSelectionMode=true;
						formSnomeds.ShowDialog();
						if(formSnomeds.DialogResult!=DialogResult.OK) {
							return;
						}
						textSNOMED.Text=formSnomeds.SnomedSelected.SnomedCode;
						textCompareString.Text="";
					}
					break;
				case EhrRestrictionType.Medication:
					using(FormMedications formMedications=new FormMedications()) {
						formMedications.IsSelectionMode=true;
						formMedications.ShowDialog();
						if(formMedications.DialogResult!=DialogResult.OK) {
							return;
						}
						textCompareString.Text=Medications.GetNameOnly(formMedications.SelectedMedicationNum);
					}
					break;
				case EhrRestrictionType.LabResult:
					using(FormLoincs formLoincs=new FormLoincs()) {
						formLoincs.IsSelectionMode=true;
						formLoincs.ShowDialog();
						if(formLoincs.DialogResult!=DialogResult.OK) {
							return;
						}
						textCompareString.Text=formLoincs.LoincSelected.LoincCode;
						comboUnits.Text=formLoincs.LoincSelected.UnitsUCUM;//may be valued, may be blank.
					}
					break;
				case EhrRestrictionType.Gender:
					//Not visible
					break;
				case EhrRestrictionType.CommPref:
					using(FormCommPrefPicker formCommPrefPicker = new FormCommPrefPicker()) {
						formCommPrefPicker.ShowDialog();
						if(formCommPrefPicker.DialogResult!=DialogResult.OK) {
							return;
						}
						textCompareString.Text=Enum.GetName(typeof(ContactMethod),formCommPrefPicker.ContactMethodCur);
					}
					break;
				case EhrRestrictionType.Allergy:
					using(FormAllergySetup formAllergySetup=new FormAllergySetup()){
						formAllergySetup.IsSelectionMode=true;
						formAllergySetup.ShowDialog();
						if(formAllergySetup.DialogResult!=DialogResult.OK) {
							return;
						}
						textCompareString.Text=AllergyDefs.GetDescription(formAllergySetup.AllergyDefNumSelected);
					}
					break;
				default://should never happen
					break;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!IsNew) {
				DoDelete=true;
			}
			DialogResult=DialogResult.Cancel;
		}

		private void textCompareString_TextChanged(object sender,EventArgs e) {
			if(textCompareString.Text!="") {
				textSNOMED.Text="";
			}
		}

		private void textSNOMED_TextChanged(object sender,EventArgs e) {
			if(textSNOMED.Text!="") {
				textCompareString.Text="";
			}
		}

		private void butProblem_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs();
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			DiseaseDef diseaseDef=formDiseaseDefs.ListDiseaseDefsSelected[0];
			if(diseaseDef.SnomedCode!="") {
				textSNOMED.Text=diseaseDef.SnomedCode;
			}
			else if(diseaseDef.ICD9Code!="") {
				textCompareString.Text=diseaseDef.ICD9Code;
				MsgBox.Show(this,"Selected problem does not have a valid SNOMED CT code.");
			}
			else {
				MsgBox.Show(this,"Selected problem does not have a valid SNOMED CT code.");
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			EhrPatListElement2014Cur.Restriction=listRestriction.GetSelected<EhrRestrictionType>();
			EhrPatListElement2014Cur.Operand=listOperand.GetSelected<EhrOperand>();
				EhrPatListElement2014Cur.CompareString=textCompareString.Text;
			if(EhrPatListElement2014Cur.Restriction==EhrRestrictionType.Problem && textCompareString.Text=="") {
				EhrPatListElement2014Cur.CompareString=textSNOMED.Text;
			}
			EhrPatListElement2014Cur.LabValue=textLabValue.Text;
			EhrPatListElement2014Cur.LabValueType=(HL70125)comboLabValueType.SelectedIndex;
			EhrPatListElement2014Cur.LabValueUnits=comboUnits.Text;//UCUM units or blank.
			try {
				EhrPatListElement2014Cur.StartDate=DateTime.Parse(textDateStart.Text);
			}
			catch {
				EhrPatListElement2014Cur.StartDate=DateTime.MinValue;
			}
			try {
				EhrPatListElement2014Cur.EndDate=DateTime.Parse(textDateStop.Text);
			}
			catch {
				EhrPatListElement2014Cur.EndDate=DateTime.MinValue;
			}
			DialogResult=DialogResult.OK;
		}

	}
}