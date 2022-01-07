using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;
using System.Xml.XPath;
using System.IO;
using OpenDental.UI;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormCdsTriggerEdit:FormODBase {
		public bool IsNew;
		public EhrTrigger EhrTriggerCur;


		public FormCdsTriggerEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrTriggerEdit_Load(object sender,EventArgs e) {
			if(PrefC.GetString(PrefName.SoftwareName)!="") {
				this.Text+=" - "+PrefC.GetString(PrefName.SoftwareName);
			}
			if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).EditBibliography) {
				textBibliography.Enabled=false;
				textInstruction.Enabled=false;
			}
			textDescription.Text=EhrTriggerCur.Description;
			textBibliography.Text=EhrTriggerCur.Bibliography;
			textInstruction.Text=EhrTriggerCur.Instructions;
			FillComboCardinality();
			FillGrid();
		}

		private void FillComboCardinality() {
			string[] stringArrayNames=Enum.GetNames(typeof(MatchCardinality));
			for(int i=0;i<stringArrayNames.Length;i++) {
				comboCardinality.Items.Add(stringArrayNames[i]);
			}
			comboCardinality.SelectedIndex=(int)EhrTriggerCur.Cardinality;
		}

		private void comboCardinality_SelectedIndexChanged(object sender,EventArgs e) {
			EhrTriggerCur.Cardinality=(MatchCardinality)comboCardinality.SelectedIndex;
			switch(EhrTriggerCur.Cardinality) {
				case MatchCardinality.One:
					labelCardinality.Text="For this trigger to provide Clinical Decision Support, only one of the conditions below must be met.";
					break;
				case MatchCardinality.OneOfEachCategory:
					labelCardinality.Text="For this trigger to provide Clinical Decision Support, at least one condition from each category must be met. Categories are Problem, Medication, Allergy, Demographics, Lab Results, and Vital Signs.";
					break;
				case MatchCardinality.TwoOrMore:
					labelCardinality.Text="For this trigger to provide Clinical Decision Support, any two of the conditions below must be met.";
					break;
				case MatchCardinality.All:
					labelCardinality.Text="For this trigger to provide Clinical Decision Support, all of the conditions below must be met.";
					break;
			}
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn column=new GridColumn("Category",80);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Code",100);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("CodeSystem",120);
			gridMain.ListGridColumns.Add(column);
			//col=new ODGridColumn("Op+Value",80);//Example: >=150
			//gridMain.Columns.Add(col);
			column=new GridColumn("Description",250);//Also includes values for labloinc and demographics and vitals. Example: ">150, BP Systolic"
			gridMain.ListGridColumns.Add(column);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//EhrTriggerCur.ProblemDefNumList-----------------------------------------------------------------------------------------------------------------------
			string[] arrayString=EhrTriggerCur.ProblemDefNumList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Problem");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("Problem Def");
				row.Cells.Add(DiseaseDefs.GetItem(PIn.Long(arrayString[i])).DiseaseName);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.ProblemIcd9List---------------------------------------------------------------------------------------------------------------------------
			arrayString=EhrTriggerCur.ProblemIcd9List.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Problem");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("ICD9 CM");
				row.Cells.Add(ICD9s.GetByCode(arrayString[i]).Description);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.ProblemIcd10List;
			arrayString=EhrTriggerCur.ProblemIcd10List.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Problem");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("ICD10 CM");
				row.Cells.Add(Icd10s.GetByCode(arrayString[i]).Description);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.ProblemSnomedList;
			arrayString=EhrTriggerCur.ProblemSnomedList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Problem");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("SNOMED CT");
				row.Cells.Add(Snomeds.GetByCode(arrayString[i]).Description);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.MedicationNumList
			arrayString=EhrTriggerCur.MedicationNumList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Medication");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("Medication Def");
				row.Cells.Add(Medications.GetDescription(PIn.Long(arrayString[i])));
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.RxCuiList
			arrayString=EhrTriggerCur.RxCuiList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Medication");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("RxCui");
				row.Cells.Add(RxNorms.GetByRxCUI(arrayString[i]).Description);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.CvxList
			arrayString=EhrTriggerCur.CvxList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Medication");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("Cvx");
				row.Cells.Add(Cvxs.GetByCode(arrayString[i]).Description);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.AllergyDefNumList
			arrayString=EhrTriggerCur.AllergyDefNumList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				row.Cells.Add("Allergy");
				row.Cells.Add(arrayString[i]);
				row.Cells.Add("Allergy Def");
				row.Cells.Add(AllergyDefs.GetOne(PIn.Long(arrayString[i])).Description);
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.DemographicsList
			arrayString=EhrTriggerCur.DemographicsList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				string[] arrayStringElements=arrayString[i].Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
				switch(arrayStringElements[0]) {
					case "age":
						row.Cells.Add("Demographic");
						row.Cells.Add("30525-0");
						row.Cells.Add("LOINC");
						row.Cells.Add("Age"+arrayStringElements[1]);//Example "Age>55"
						gridMain.ListGridRows.Add(row);
						break;
					case "gender":
						row.Cells.Add("Demographic");
						row.Cells.Add("46098-0");
						row.Cells.Add("LOINC");
						row.Cells.Add("Gender:"+arrayString[i].Replace("gender,",""));//Example "Gender:Male, Female, Unknown/Undifferentiated"
						gridMain.ListGridRows.Add(row);
						break;
					default:
						//should never happen
						continue;//next trigger
				}
			}
			//EhrTriggerCur.LabLoincList
			arrayString=EhrTriggerCur.LabLoincList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				Loinc loincTemp=Loincs.GetByCode(arrayString[i]);//.Split(new string[] { ";" },StringSplitOptions.None)[0]);
				if(loincTemp==null) {
					continue;
				}
				row.Cells.Add("Laboratory");
				row.Cells.Add(loincTemp.LoincCode);
				row.Cells.Add("LOINC");
				row.Cells.Add(loincTemp.NameShort);
				//switch(arrayString[i].Split(new string[] { ";" },StringSplitOptions.RemoveEmptyEntries).Length) {
				//	case 1://loinc only comparison
				//		row.Cells.Add(_loincTemp.NameShort);
				//		break;
				//	case 2://microbiology or unitless lab.
				//		Snomed _snomedTemp=Snomeds.GetByCode(arrayString[i].Split(new string[] { ";" },StringSplitOptions.None)[1]);
				//		row.Cells.Add(_loincTemp.NameShort+", "
				//			+(_snomedTemp==null?arrayString[i].Split(new string[] { ";" },StringSplitOptions.None)[1]:_snomedTemp.Description));//Example: Bacteria Identified, Campylobacter jenuni
				//		break;
				//	case 3://"traditional lab results"
				//		row.Cells.Add(_loincTemp.NameShort+" "
				//	+arrayString[i].Split(new string[] { ";" },StringSplitOptions.None)[1]+" "//example: >150 or a snomed code if microbiology
				//	+arrayString[i].Split(new string[] { ";" },StringSplitOptions.None)[2]    //example: mg/dL or blank
				//			);
				//		break;
				//	default://should never happen. Will display blank.
				//		row.Cells.Add("");
				//		break;
				//}
				gridMain.ListGridRows.Add(row);
			}
			//EhrTriggerCur.VitalLoincList
			arrayString=EhrTriggerCur.VitalLoincList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<arrayString.Length;i++) {
				row=new GridRow();
				string[] arrayStringElements=arrayString[i].Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
				switch(arrayStringElements[0]) {
					case "height":
						row.Cells.Add("Vitals");
						row.Cells.Add("8302-2");
						row.Cells.Add("LOINC");
						row.Cells.Add("Height"+arrayString[i].Replace("height,","")+" in.");//Example "Age>55"
						gridMain.ListGridRows.Add(row);
						break;
					case "weight":
						row.Cells.Add("Vitals");
						row.Cells.Add("29463-7");
						row.Cells.Add("LOINC");
						row.Cells.Add("Weight:"+arrayString[i].Replace("weight,",""));//Example "Gender:Male, Female, Unknown/Undifferentiated"
						gridMain.ListGridRows.Add(row);
						break;
					case "bp???":
						row.Cells.Add("Vitals");
						row.Cells.Add("???There are two.");
						row.Cells.Add("LOINC");
						row.Cells.Add("???");//Example "Gender:Male, Female, Unknown/Undifferentiated"
						gridMain.ListGridRows.Add(row);
						break;
					case "BMI":
						row.Cells.Add("Vitals");
						row.Cells.Add("39156-5");
						row.Cells.Add("LOINC");
						row.Cells.Add("BMI"+arrayString[i].Replace("BMI,","").Replace("%","")+"%");//Example "Gender:Male, Female, Unknown/Undifferentiated"
						gridMain.ListGridRows.Add(row);
						break;
					default:
						//should never happen
						continue;//next trigger
				}
			}
			//End trigger fields.
			gridMain.EndUpdate();
		}

		#region Add Buttons
		private void butAddProblem_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs();
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			DiseaseDef diseaseDef=formDiseaseDefs.ListDiseaseDefsSelected[0];
			//DiseaseDefNum
			if(!EhrTriggerCur.ProblemDefNumList.Contains(" "+diseaseDef.DiseaseDefNum+" ")){
				EhrTriggerCur.ProblemDefNumList+=" "+diseaseDef.DiseaseDefNum+" ";
			}
			//Icd9Num
			if(diseaseDef.ICD9Code!="" && !EhrTriggerCur.ProblemIcd9List.Contains(" "+diseaseDef.ICD9Code+" ")) {
				EhrTriggerCur.ProblemIcd9List+=" "+diseaseDef.ICD9Code+" ";
			}
			//Icd10Num
			if(diseaseDef.Icd10Code!="" && !EhrTriggerCur.ProblemIcd9List.Contains(" "+diseaseDef.Icd10Code+" ")) {
				EhrTriggerCur.ProblemIcd10List+=" "+diseaseDef.Icd10Code+" ";
			}
			//Snomed
			if(diseaseDef.SnomedCode!="" && !EhrTriggerCur.ProblemIcd9List.Contains(" "+diseaseDef.SnomedCode+" ")) {
				EhrTriggerCur.ProblemSnomedList+=" "+diseaseDef.SnomedCode+" ";
			}
			FillGrid();
		}

		private void butAddIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s formIcd9s=new FormIcd9s();
			formIcd9s.IsSelectionMode=true;
			formIcd9s.ShowDialog();
			if(formIcd9s.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.ProblemIcd9List+=" "+formIcd9s.SelectedIcd9.ICD9Code+" ";
			FillGrid();
		}

		private void butAddIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s formIcd10s=new FormIcd10s();
			formIcd10s.IsSelectionMode=true;
			formIcd10s.ShowDialog();
			if(formIcd10s.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.ProblemIcd10List+=" "+formIcd10s.SelectedIcd10.Icd10Code+" ";
			FillGrid();
		}

		private void butAddSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsMultiSelectMode=true;
			formSnomeds.ShowDialog();
			if(formSnomeds.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formSnomeds.ListSelectedSnomeds.Count;i++) {
				EhrTriggerCur.ProblemSnomedList+=" "+formSnomeds.ListSelectedSnomeds[i].SnomedCode+" ";
			}
			FillGrid();
		}

		private void butAddMed_Click(object sender,EventArgs e) {
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult!=DialogResult.OK) {
				return;
			}
			Medication medication=Medications.GetMedication(formMedications.SelectedMedicationNum);
			EhrTriggerCur.MedicationNumList+=" "+medication.MedicationNum+" ";
			if(medication.RxCui!=0) {
				EhrTriggerCur.RxCuiList+=" "+medication.RxCui+" ";
			}
			FillGrid();
		}

		private void butAddRxNorm_Click(object sender,EventArgs e) {
			using FormRxNorms formRxNorms=new FormRxNorms();
			formRxNorms.IsMultiSelectMode=true;
			formRxNorms.ShowDialog();
			if(formRxNorms.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formRxNorms.ListSelectedRxNorms.Count;i++) {
				EhrTriggerCur.RxCuiList+=" "+formRxNorms.ListSelectedRxNorms[i].RxCui+" ";
			}
			FillGrid();
		}

		private void butAddCvx_Click(object sender,EventArgs e) {
			//using FormCvxs FormC=new FormCvxs();
			//FormC.IsSelectionMode=true;
			//FormC.ShowDialog();
			//if(FormC.DialogResult!=DialogResult.OK) {
			//	return;
			//}
			//EhrTriggerCur.CvxList+=" "+FormC.SelectedCvx.CvxCode+" ";
			//FillGrid();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			using FormAllergySetup formAllergySetup=new FormAllergySetup();
			formAllergySetup.IsSelectionMode=true;
			formAllergySetup.ShowDialog();
			if(formAllergySetup.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.AllergyDefNumList+=" "+formAllergySetup.SelectedAllergyDefNum+" ";
			FillGrid();
		}

		private void butAddAge_Click(object sender,EventArgs e) {
			//30525-0 = Age (Actual). There are 3 other age LOINCS that should also be checked.  Stored as " Age(Operand)(Value) "
			//21611-7 = Estimated
			//21612-7 = Reported
			//29553-5 = Calculated
			using InputBox inputBox=new InputBox(Lan.g(this,"Input age criterion as (operand)(value). Examples: <18, >55, =22, <=35."));
			if(inputBox.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(inputBox.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(inputBox.textResult.Text,@"^(<|<=|>|>=|=)\d+$")) {//Starts with <,>,=,<=, or >= followed by numbers, and nothing else.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.DemographicsList+= " age,"+inputBox.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddGender_Click(object sender,EventArgs e) {
			//46098-0 = Gender. There are 3 other age LOINCS that should also be checked.  Stored as " Age(Operand)(Value) "
			//Other Gender LoincCodes include 21840-4,46607-8,54131-8, and 72143-1
			using InputBox inputBox=new InputBox(Lan.g(this,"Input genders.  Example: male,female,unknown"));
			//Fill inputBox with current gender codes.---------------------------------------------------------
			if(EhrTriggerCur.DemographicsList.Contains("gender")) {
				string[] arrayString=EhrTriggerCur.DemographicsList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<arrayString.Length;i++) {
					if(arrayString[i].StartsWith("gender")) {
						inputBox.textResult.Text=arrayString[i].Replace("gender,","");
						break;
					}
				}
			}//end if gender
			inputBox.ShowDialog();
			if(inputBox.textResult.Text!="" && !Regex.IsMatch(inputBox.textResult.Text,@"^(male|female|unknown)(,(male|female|unknown)){0,2}$")) {//m,f,u optionally followed by a comma delimited list with optional white space after comma.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			//remove current gender codes-------------------
			if(EhrTriggerCur.DemographicsList.Contains("gender")){
				string[] arrayString=EhrTriggerCur.DemographicsList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<arrayString.Length;i++) {
					if(arrayString[i].StartsWith("gender")){
						EhrTriggerCur.DemographicsList=EhrTriggerCur.DemographicsList.Replace(" "+arrayString[i]+" ","");
						continue;
					}
				}
			}
			if(inputBox.textResult.Text=="") {
				FillGrid();
				return;
			}
			//Add new gender codes.
			EhrTriggerCur.DemographicsList+= " gender,"+inputBox.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddLab_Click(object sender,EventArgs e) {
			using FormCDSILabResult formCDSILabResult=new FormCDSILabResult();
			formCDSILabResult.ShowDialog();
			if(formCDSILabResult.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.LabLoincList+=" "+formCDSILabResult.ResultCDSITriggerText+" ";
			FillGrid();
		}

		private void butAddHeight_Click(object sender,EventArgs e) {
			//8302-2 = height
			using InputBox inputBox=new InputBox(Lan.g(this,"Input height criterion as (operand)(value in inches). Examples: >80, <=48.5"));
			if(inputBox.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(inputBox.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(inputBox.textResult.Text,@"^^(<|<=|>|>=|=)(\d)+(.(\d)+)*$")) {//Starts with <,>,=,<=, or >= followed by a float, and nothing else.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.VitalLoincList+= " height,"+inputBox.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddWeight_Click(object sender,EventArgs e) {
			//29463-7 = weight
			using InputBox inputBox=new InputBox(Lan.g(this,"Input weight criterion as (operand)(value). Examples: <=99.5, >=300"));
			if(inputBox.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(inputBox.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(inputBox.textResult.Text,@"^(<|<=|>|>=|=)(\d)+(.(\d)+)*$")) {//Starts with <,>,=,<=, or >= followed by a float, and nothing else.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.VitalLoincList+= " weight,"+inputBox.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddBP_Click(object sender,EventArgs e) {
			//TODO:
			//using InputBox IB=new InputBox(Lan.g(this,"Input BP criterion."));
			//IB.ShowDialog();
			//if(false && !Regex.IsMatch(IB.textResult.Text,@"^(<|<=|>|>=|=)\d+$")) {//Starts with <,>,=,<=, or >= followed by numbers, and nothing else.
			//	MsgBox.Show(this,"Invalid format.");
			//	return;
			//}
			//EhrTriggerCur.VitalLoincList+= " BP???,"+IB.textResult.Text.Trim()+" ";
			//FillGrid();
		}

		private void butAddBMI_Click(object sender,EventArgs e) {
			//39156-5 = BMI
			using InputBox inputBox=new InputBox(Lan.g(this,"Input BMI criterion. Examples: <5, >=27.5%"));
			if(inputBox.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(inputBox.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(inputBox.textResult.Text,@"^(<|<=|>|>=|=)(\d)+(.(\d)+)*(%){0,1}$")) {//operand followed by valid float followed by an optional percent sign.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.VitalLoincList+= " BMI,"+inputBox.textResult.Text.Trim()+" ";
			FillGrid();
		}

		#endregion

		private void butDelete_Click(object sender,EventArgs e) {
			EhrTriggers.Delete(EhrTriggerCur.EhrTriggerNum);
			DialogResult=DialogResult.OK;
		}

		private bool IsValid() {
			if(string.IsNullOrEmpty(EhrTriggerCur.Description)) {
				MsgBox.Show("Description cannot be blank.");
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			EhrTriggerCur.Description=textDescription.Text;
			EhrTriggerCur.Instructions=textInstruction.Text;
			EhrTriggerCur.Bibliography=textBibliography.Text;
			if(!IsValid()) {
				return;
			}
			if(IsNew) {
				EhrTriggers.Insert(EhrTriggerCur);
			}
			else {
				EhrTriggers.Update(EhrTriggerCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}