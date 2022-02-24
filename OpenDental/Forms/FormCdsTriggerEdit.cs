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
			string[] names=Enum.GetNames(typeof(MatchCardinality));
			for(int i=0;i<names.Length;i++) {
				comboCardinality.Items.Add(names[i]);
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
			GridColumn col=new GridColumn("Category",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code",100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("CodeSystem",120);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn("Op+Value",80);//Example: >=150
			//gridMain.Columns.Add(col);
			col=new GridColumn("Description",250);//Also includes values for labloinc and demographics and vitals. Example: ">150, BP Systolic"
			gridMain.ListGridColumns.Add(col);
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
				Loinc _loincTemp=Loincs.GetByCode(arrayString[i]);//.Split(new string[] { ";" },StringSplitOptions.None)[0]);
				if(_loincTemp==null) {
					continue;
				}
				row.Cells.Add("Laboratory");
				row.Cells.Add(_loincTemp.LoincCode);
				row.Cells.Add("LOINC");
				row.Cells.Add(_loincTemp.NameShort);
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
			using FormDiseaseDefs FormDD=new FormDiseaseDefs();
			FormDD.IsSelectionMode=true;
			FormDD.ShowDialog();
			if(FormDD.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			DiseaseDef diseaseDef=FormDD.ListSelectedDiseaseDefs[0];
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
			using FormIcd9s FormI9=new FormIcd9s();
			FormI9.IsSelectionMode=true;
			FormI9.ShowDialog();
			if(FormI9.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.ProblemIcd9List+=" "+FormI9.SelectedIcd9.ICD9Code+" ";
			FillGrid();
		}

		private void butAddIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s FormI10=new FormIcd10s();
			FormI10.IsSelectionMode=true;
			FormI10.ShowDialog();
			if(FormI10.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.ProblemIcd10List+=" "+FormI10.SelectedIcd10.Icd10Code+" ";
			FillGrid();
		}

		private void butAddSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			FormS.IsMultiSelectMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<FormS.ListSelectedSnomeds.Count;i++) {
				EhrTriggerCur.ProblemSnomedList+=" "+FormS.ListSelectedSnomeds[i].SnomedCode+" ";
			}
			FillGrid();
		}

		private void butAddMed_Click(object sender,EventArgs e) {
			using FormMedications FormM=new FormMedications();
			FormM.IsSelectionMode=true;
			FormM.ShowDialog();
			if(FormM.DialogResult!=DialogResult.OK) {
				return;
			}
			Medication m=Medications.GetMedication(FormM.SelectedMedicationNum);
			EhrTriggerCur.MedicationNumList+=" "+m.MedicationNum+" ";
			if(m.RxCui!=0) {
				EhrTriggerCur.RxCuiList+=" "+m.RxCui+" ";
			}
			FillGrid();
		}

		private void butAddRxNorm_Click(object sender,EventArgs e) {
			using FormRxNorms FormRXN=new FormRxNorms();
			FormRXN.IsMultiSelectMode=true;
			FormRXN.ShowDialog();
			if(FormRXN.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<FormRXN.ListSelectedRxNorms.Count;i++) {
				EhrTriggerCur.RxCuiList+=" "+FormRXN.ListSelectedRxNorms[i].RxCui+" ";
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
			using FormAllergySetup FormAS=new FormAllergySetup();
			FormAS.IsSelectionMode=true;
			FormAS.ShowDialog();
			if(FormAS.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.AllergyDefNumList+=" "+FormAS.SelectedAllergyDefNum+" ";
			FillGrid();
		}

		private void butAddAge_Click(object sender,EventArgs e) {
			//30525-0 = Age (Actual). There are 3 other age LOINCS that should also be checked.  Stored as " Age(Operand)(Value) "
			//21611-7 = Estimated
			//21612-7 = Reported
			//29553-5 = Calculated
			using InputBox IB=new InputBox(Lan.g(this,"Input age criterion as (operand)(value). Examples: <18, >55, =22, <=35."));
			if(IB.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(IB.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(IB.textResult.Text,@"^(<|<=|>|>=|=)\d+$")) {//Starts with <,>,=,<=, or >= followed by numbers, and nothing else.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.DemographicsList+= " age,"+IB.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddGender_Click(object sender,EventArgs e) {
			//46098-0 = Gender. There are 3 other age LOINCS that should also be checked.  Stored as " Age(Operand)(Value) "
			//Other Gender LoincCodes include 21840-4,46607-8,54131-8, and 72143-1
			using InputBox IB=new InputBox(Lan.g(this,"Input genders.  Example: male,female,unknown"));
			//Fill inputBox with current gender codes.---------------------------------------------------------
			if(EhrTriggerCur.DemographicsList.Contains("gender")) {
				string[] arrayString=EhrTriggerCur.DemographicsList.Split(new string[] { " " },StringSplitOptions.RemoveEmptyEntries);
				for(int i=0;i<arrayString.Length;i++) {
					if(arrayString[i].StartsWith("gender")) {
						IB.textResult.Text=arrayString[i].Replace("gender,","");
						break;
					}
				}
			}//end if gender
			IB.ShowDialog();
			if(IB.textResult.Text!="" && !Regex.IsMatch(IB.textResult.Text,@"^(male|female|unknown)(,(male|female|unknown)){0,2}$")) {//m,f,u optionally followed by a comma delimited list with optional white space after comma.
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
			if(IB.textResult.Text=="") {
				FillGrid();
				return;
			}
			//Add new gender codes.
			EhrTriggerCur.DemographicsList+= " gender,"+IB.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddLab_Click(object sender,EventArgs e) {
			using FormCDSILabResult FormCLR=new FormCDSILabResult();
			FormCLR.ShowDialog();
			if(FormCLR.DialogResult!=DialogResult.OK) {
				return;
			}
			EhrTriggerCur.LabLoincList+=" "+FormCLR.ResultCDSITriggerText+" ";
			FillGrid();
		}

		private void butAddHeight_Click(object sender,EventArgs e) {
			//8302-2 = height
			using InputBox IB=new InputBox(Lan.g(this,"Input height criterion as (operand)(value in inches). Examples: >80, <=48.5"));
			if(IB.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(IB.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(IB.textResult.Text,@"^^(<|<=|>|>=|=)(\d)+(.(\d)+)*$")) {//Starts with <,>,=,<=, or >= followed by a float, and nothing else.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.VitalLoincList+= " height,"+IB.textResult.Text.Trim()+" ";
			FillGrid();
		}

		private void butAddWeight_Click(object sender,EventArgs e) {
			//29463-7 = weight
			using InputBox IB=new InputBox(Lan.g(this,"Input weight criterion as (operand)(value). Examples: <=99.5, >=300"));
			if(IB.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(IB.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(IB.textResult.Text,@"^(<|<=|>|>=|=)(\d)+(.(\d)+)*$")) {//Starts with <,>,=,<=, or >= followed by a float, and nothing else.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.VitalLoincList+= " weight,"+IB.textResult.Text.Trim()+" ";
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
			using InputBox IB=new InputBox(Lan.g(this,"Input BMI criterion. Examples: <5, >=27.5%"));
			if(IB.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(IB.textResult.Text)) {
				return;
			}
			if(!Regex.IsMatch(IB.textResult.Text,@"^(<|<=|>|>=|=)(\d)+(.(\d)+)*(%){0,1}$")) {//operand followed by valid float followed by an optional percent sign.
				MsgBox.Show(this,"Invalid format.");
				return;
			}
			EhrTriggerCur.VitalLoincList+= " BMI,"+IB.textResult.Text.Trim()+" ";
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