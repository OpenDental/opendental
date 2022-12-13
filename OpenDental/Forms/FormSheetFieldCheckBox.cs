using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSheetFieldCheckBox:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		public bool IsEditMobile;
		public bool IsReadOnly;
		private List<string> _listRadioButtonVals;
		private List<AllergyDef> _listAllergyDefs;
		private List<DiseaseDef> _listDiseaseDefs;

		public FormSheetFieldCheckBox() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldCheckBox_Load(object sender,EventArgs e) {
			if(IsEditMobile) {
				textTabOrder.Enabled=false;
			}
			if(IsReadOnly) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			_listDiseaseDefs=DiseaseDefs.GetDeepCopy(true);
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
			labelReportableName.Visible=false;
			textReportableName.Visible=false;
			if(SheetFieldDefCur.FieldName.StartsWith("misc")) {
				labelReportableName.Visible=true;
				textReportableName.Visible=true;
				textReportableName.Text=SheetFieldDefCur.ReportableName;
			}
			textUiLabelMobileMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobileMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			textUiLabelMobileRadioButtonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobileRadioButtonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			textUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			textUiLabelMobileCheckBoxNonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobileCheckBoxNonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelAlsoActs.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			comboRadioGroupNameMisc.Text=SheetFieldDefCur.RadioButtonGroup;
			checkRequired.Checked=SheetFieldDefCur.IsRequired;
			textTabOrder.Text=SheetFieldDefCur.TabOrder.ToString();
			textUiLabelMobileMisc.Text=SheetFieldDefCur.UiLabelMobile;
			textUiLabelMobileRadioButtonMisc.Text=SheetFieldDefCur.UiLabelMobileRadioButton;
			textUiLabelMobile.Text=SheetFieldDefCur.UiLabelMobile;
			textUiLabelMobileCheckBoxNonMisc.Text=SheetFieldDefCur.UiLabelMobileRadioButton;
			textMobileMedicalNameOverride.Text=SheetFieldDefCur.UiLabelMobile;
			textItemOverride.Text=SheetFieldDefCur.UiLabelMobileRadioButton;
			//Not allowed to change sheettype or fieldtype once created.  So get all avail fields for this sheettype.
			//These names will not include the ':' for allergies and problems.
			List<SheetFieldDef> listSheetFieldDefs=SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.Check);
			//If an existing SheetFieldDefCur is not found in the list, add it so we maintain current selection.
			if(SheetFieldDefCur.FieldName.In("FluorideProc","AssessmentProc") || SheetFieldDefCur.FieldName.StartsWith("Proc:")){
				//Couldn't find the current sheetfielddef.  Add it to the list.
				//Checkboxes associated to procedure codes will never be present in SheetFieldsAvailable.GetList(...).
				//Previously this list would contain AssessmentProc and FluorideProc.
				//All other checkboxes associated to proc codes will not exists in list either.
				listSheetFieldDefs.Add(SheetFieldDefCur);
			}
			listBoxFields.Items.AddList(listSheetFieldDefs, x => x.FieldName);
			for(int i=0; i<listBoxFields.Items.Count;i++) {
				if(SheetFieldDefCur.FieldName.StartsWith(((SheetFieldDef)listBoxFields.Items.GetObjectAt(i)).FieldName)) {
					listBoxFields.SetSelected(i);
				}
			}
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			textHeight.Text=SheetFieldDefCur.Height.ToString();
			List<string> listGroupNames = new List<string>();
			listGroupNames = SheetDefCur.SheetFieldDefs.FindAll(x => !string.IsNullOrEmpty(x.RadioButtonGroup))
				.GroupBy(x => x.RadioButtonGroup)
				.Select(x => x.Key).ToList();
			for(int i=0;i<listGroupNames.Count;i++) {
				comboRadioGroupNameMisc.Items.Add(listGroupNames[i]);
			}
			if(SheetDefCur.SheetType==SheetTypeEnum.MedicalHistory) {
				radioYes.Checked=true;
				string fieldNameSelected="";
				if (listBoxFields.SelectedIndex>=0){
					fieldNameSelected=listBoxFields.GetSelected<SheetFieldDef>().FieldName;
				}
				if(fieldNameSelected=="allergy") {
					//Will be of format allergy:Aspirin
					FillListMedical(MedicalListType.allergy);
				}
				else if(fieldNameSelected=="problem") {
					//Will be of format problem:Bleeding
					FillListMedical(MedicalListType.problem);
				}
				if(SheetFieldDefCur.RadioButtonValue=="N") {
					radioNo.Checked=true;
					radioYes.Checked=false;
				}
			}
			if(SheetDefCur.SheetType==SheetTypeEnum.Screening) {
				butAddProc.Visible=true;
			}
			if(radioYes.Checked) {
				//If checkbox's text is not the default of "Yes", display the customized text in override box
				if(SheetFieldDefCur.UiLabelMobileRadioButton!=radioYes.Text) {
					textMobileCheckOverride.Text=SheetFieldDefCur.UiLabelMobileRadioButton;
				}
			}
			else if(radioNo.Checked) {
				//If checkbox's text is not the default of "No", display the customized text in override box
				if(SheetFieldDefCur.UiLabelMobileRadioButton!=radioNo.Text) {
					textMobileCheckOverride.Text=SheetFieldDefCur.UiLabelMobileRadioButton;
				}
			}
		}

		private void FormSheetFieldCheckBox_Shown(object sender,EventArgs e) {
			//show allergy/problem message box here so the user can see the list of allergies/problems before deciding to add one
			string fieldNameSelected="";
			if (listBoxFields.SelectedIndex>=0){
				fieldNameSelected=listBoxFields.GetSelected<SheetFieldDef>().FieldName;
			}
			if(fieldNameSelected=="allergy") {
				//if nothing is selected we didn't find it, prompt to add
				if(listMedical.SelectedIndex<=-1) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Allergy does not exist in list. Would you like to add the allergy?")){
						AddAllergy(SheetFieldDefCur);
					}
				}
			}
			else if(fieldNameSelected=="problem") {
				//if nothing is selected we didn't find it, prompt to add
				if(listMedical.SelectedIndex<=-1) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Problem does not exist in problems list. Would you like to add the problem?")){
						AddProblem(SheetFieldDefCur);
					}
				}
			}
		}

		///<summary>Fills listMedical with the corresponding list type.  This saves on load time by only filling necessary lists.
		///Attempts to seelct the cooresponding allergy/problem. Will select nothing if it does not exist. </summary>
		private void FillListMedical(MedicalListType medListType) {
			string medSelection=SheetFieldDefCur.FieldName.Remove(0,Math.Min(SheetFieldDefCur.FieldName.Length,8));
			listMedical.Items.Clear();
			switch(medListType) {
				case MedicalListType.allergy:
					if(_listAllergyDefs==null) {
						_listAllergyDefs=AllergyDefs.GetAll(false);
					}
					listMedical.Items.AddList(_listAllergyDefs, x => x.Description);
					for(int i=0; i<listMedical.Items.Count; i++){
						if(((AllergyDef)listMedical.Items.GetObjectAt(i)).Description==medSelection) {
							listMedical.SetSelected(i);
							break;
						}
					}
					break;
				case MedicalListType.problem:
					listMedical.Items.AddList(_listDiseaseDefs, x => x.DiseaseName);
					for(int i=0; i<listMedical.Items.Count; i++){
						if(((DiseaseDef)listMedical.Items.GetObjectAt(i)).DiseaseName==medSelection) {
							listMedical.SetSelected(i);
							break;
						}
					}
					break;
			}
		}
		
		private void comboRadioGroupNameMisc_SelectedIndexChanged(object sender,EventArgs e) {
			var selected=SheetDefCur.SheetFieldDefs.FirstOrDefault(x => !string.IsNullOrEmpty(x.RadioButtonGroup) && x.RadioButtonGroup==comboRadioGroupNameMisc.Text);
			if(selected==null) {
				return;
			}
			textUiLabelMobileMisc.Text=selected.UiLabelMobile;
		}

		private void listFields_SelectedIndexChanged(object sender,EventArgs e) {
			labelMiscInstructions.Visible=false;
			labelReportableName.Visible=false;
			textReportableName.Visible=false;
			groupRadio.Visible=false;
			groupRadioMisc.Visible=false;
			labelUiLabelMobileCheckBoxNonMisc.Visible=false;
			textUiLabelMobileCheckBoxNonMisc.Visible=false;
			labelRequired.Visible=false;
			checkRequired.Visible=false;
			labelMedical.Visible=false;
			listMedical.Visible=false;
			radioYes.Visible=false;
			radioNo.Visible=false;
			labelYesNo.Visible=false;
			butAddAllergy.Visible=false;
			butAddProblem.Visible=false;
			labelMobileCheckOverride.Visible=false;
			textMobileCheckOverride.Visible=false;
			labelMobileMedicalNameOverride.Visible=false;
			textMobileMedicalNameOverride.Visible=false;
			if(listBoxFields.SelectedIndex==-1) {
				return;
			}
			string fieldNameSelected="";
			if (listBoxFields.SelectedIndex>=0){
				fieldNameSelected=listBoxFields.GetSelected<SheetFieldDef>().FieldName;
			}
			if(SheetDefCur.SheetType==SheetTypeEnum.MedicalHistory) {
				labelRequired.Visible=true;
				checkRequired.Visible=true;
				switch(fieldNameSelected) {
					case "allergy":
						labelMedical.Visible=true;
						listMedical.Visible=true;
						radioYes.Visible=true;
						radioNo.Visible=true;
						labelYesNo.Visible=true;
						labelMedical.Text="Allergies";
						FillListMedical(MedicalListType.allergy);
						butAddAllergy.Visible=true;
						//Only show mobile override option if field name is an allergy and the form it's on is mobile allowed sheet
						labelMobileCheckOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						textMobileCheckOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						labelMobileMedicalNameOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						textMobileMedicalNameOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						break;
					case "problem":
						labelMedical.Visible=true;
						listMedical.Visible=true;
						radioYes.Visible=true;
						radioNo.Visible=true;
						labelYesNo.Visible=true;
						labelMedical.Text="Problems";
						FillListMedical(MedicalListType.problem);
						butAddProblem.Location=butAddAllergy.Location;
						butAddProblem.Visible=true;
						//Only show mobile override option if field name is problem and the form it's on is mobile allowed sheet
						labelMobileCheckOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						textMobileCheckOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						labelMobileMedicalNameOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						textMobileMedicalNameOverride.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
						break;
				}
			}
			if(fieldNameSelected=="misc") {
				labelMiscInstructions.Visible=true;
				labelReportableName.Visible=true;
				textReportableName.Visible=true;
				textReportableName.Text=SheetFieldDefCur.ReportableName;//will either be "" or saved ReportableName.
				groupRadioMisc.Visible=true;
				labelRequired.Visible=true;
				checkRequired.Visible=true;
			}
			else {
				textReportableName.Text="";
				_listRadioButtonVals=SheetFieldsAvailable.GetRadio(fieldNameSelected);
				if(_listRadioButtonVals.Count==0) { //Rare, currently only addressAndHmPhoneIsSameEntireFamily.
					labelUiLabelMobileCheckBoxNonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
					textUiLabelMobileCheckBoxNonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
					return;
				}
				groupRadio.Visible=true;
				labelRequired.Visible=true;
				checkRequired.Visible=true;
				listRadio.Items.Clear();
				for(int i=0;i<_listRadioButtonVals.Count;i++) {
					listRadio.Items.Add(_listRadioButtonVals[i]);
					if(SheetFieldDefCur.RadioButtonValue==_listRadioButtonVals[i]) {
						listRadio.SelectedIndex=i;
					}
				}
				//Set the mobile group caption.
				var sheetFieldGroup=SheetDefCur.SheetFieldDefs.FirstOrDefault(x =>
					x.FieldType==SheetFieldType.CheckBox &&
					!string.IsNullOrEmpty(x.FieldName) &&
					x.FieldName==fieldNameSelected &&
					x.Language==SheetFieldDefCur.Language);
				textUiLabelMobile.Text=sheetFieldGroup==null ? "" : sheetFieldGroup.UiLabelMobile;
			}
		}

		private void listRadio_Click(object sender,EventArgs e) {
			if(listRadio.SelectedIndex==-1){
				return;
			}
			SheetFieldDefCur.RadioButtonValue=_listRadioButtonVals[listRadio.SelectedIndex];
		}

		private void listFields_DoubleClick(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void listMedical_DoubleClick(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			AddAllergy(SheetFieldDefCur);
		}

		private void AddAllergy(SheetFieldDef SheetFieldDefCur) {
			using FormAllergyDefEdit formAllergyDefEdit=new FormAllergyDefEdit();
			formAllergyDefEdit.AllergyDefCur=new AllergyDef();
			formAllergyDefEdit.AllergyDefCur.IsNew=true;
			formAllergyDefEdit.AllergyDefCur.Description=SheetFieldDefCur?.FieldName.Replace("allergy:","")??"";
			formAllergyDefEdit.ShowDialog();
			if(formAllergyDefEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_listAllergyDefs.Add(formAllergyDefEdit.AllergyDefCur);
			FillListMedical(MedicalListType.allergy);
		}

		private void butAddProblem_Click(object sender,EventArgs e) {
			AddProblem(SheetFieldDefCur);
		}

		private void AddProblem(SheetFieldDef SheetFieldDefCur) {
			if(!Security.IsAuthorized(Permissions.ProblemDefEdit)) {
				return;
			}
			DiseaseDef diseaseDef=new DiseaseDef();
			diseaseDef.ICD9Code="";
			diseaseDef.ICD9Code="";
			diseaseDef.Icd10Code="";
			diseaseDef.SnomedCode="";
			diseaseDef.ItemOrder=DiseaseDefs.GetCount();
			diseaseDef.DiseaseName=SheetFieldDefCur?.FieldName.Replace("problem:","")??"";
			using FormDiseaseDefEdit formDiseaseDefEdit=new FormDiseaseDefEdit(diseaseDef,false);
			formDiseaseDefEdit.IsNew=true;
			formDiseaseDefEdit.ShowDialog();
			if(formDiseaseDefEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			DiseaseDefs.Insert(formDiseaseDefEdit.DiseaseDefCur);
			DataValid.SetInvalid(InvalidType.Diseases);
			_listDiseaseDefs=DiseaseDefs.GetDeepCopy(true);
			SecurityLogs.MakeLogEntry(Permissions.ProblemDefEdit,0,formDiseaseDefEdit.SecurityLogMsgText);
			FillListMedical(MedicalListType.problem);
		}

		private void radioYes_Click(object sender,EventArgs e) {
			if(radioYes.Checked) {
				radioNo.Checked=false;
			}
			else {
				radioNo.Checked=true;
			}
		}

		private void radioNo_Click(object sender,EventArgs e) {
			if(radioNo.Checked) {
				radioYes.Checked=false;
			}
			else {
				radioYes.Checked=true;
			}
		}

		private void butAddProc_Click(object sender,EventArgs e) {
			List<GridColumn> listGridCols=new List<GridColumn>();
			GridColumn col = new GridColumn();
			col=new GridColumn(Lan.g(this,"Code"),70);
			listGridCols.Add(col);
			col=new GridColumn(Lan.g(this,"Abbreviation"),90,HorizontalAlignment.Center);
			listGridCols.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),0,HorizontalAlignment.Right);
			listGridCols.Add(col);
			List<ProcedureCode> listProcedureCodesMouth=ProcedureCodes.GetProcCodesByTreatmentArea(false,TreatmentArea.Mouth,TreatmentArea.None)
				.OrderBy(x => x.ProcCode).ToList();
			List<GridRow> listGridRows=new List<GridRow>();
			for(int i=0;i<listProcedureCodesMouth.Count;i++){
				GridRow row=new GridRow (listProcedureCodesMouth[i].ProcCode,listProcedureCodesMouth[i].AbbrDesc,listProcedureCodesMouth[i].Descript);
				row.Tag=listProcedureCodesMouth[i];
				listGridRows.Add(row);
			}
			using FormGridSelection formGridSelection=new FormGridSelection(listGridCols,listGridRows,"Add Procedure","Procedures");
			if(formGridSelection.ShowDialog()!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formGridSelection.ListSelectedTags.Count;i++){
				string fieldName="Proc:"+((ProcedureCode)formGridSelection.ListSelectedTags[i]).ProcCode;
				listBoxFields.Items.Add(fieldName,SheetFieldDef.NewCheckBox(fieldName,0,0,0,0));
				listBoxFields.SetSelected(listBoxFields.Items.Count-1,true);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			SheetFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void SaveAndClose(){
			if(!textXPos.IsValid()
				|| !textYPos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid()
				|| !textTabOrder.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listBoxFields.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a field name first.");
				return;
			}
			if(SheetDefCur.SheetType==SheetTypeEnum.ExamSheet) {
				if(textReportableName.Text.Contains(";") || textReportableName.Text.Contains(":")) {
					MsgBox.Show(this,"Reportable name for Exam Sheet fields may not contain a ':' or a ';'.");
					return;
				}
				if(comboRadioGroupNameMisc.Text.Contains(";") ||comboRadioGroupNameMisc.Text.Contains(":")) {
					MsgBox.Show(this,"Radio button group name for Exam Sheet fields may not contain a ':' or a ';'.");
					return;
				}
			}
			string fieldNameSelected="";
			if (listBoxFields.SelectedIndex>=0){
				fieldNameSelected=listBoxFields.GetSelected<SheetFieldDef>().FieldName;
			}
			string radioButtonValue="";
			string radioItemValue="";
			#region Medical History Sheet
			if(SheetDefCur.SheetType==SheetTypeEnum.MedicalHistory && fieldNameSelected!="misc") {
				if(listMedical.Visible) {
					if(listMedical.SelectedIndex==-1) {
						switch(fieldNameSelected) {
							case "allergy":
								MsgBox.Show(this,"Please select an allergy first.");
								return;
							case "problem":
								MsgBox.Show(this,"Please select a problem first.");
								return;
						}
					}
					string medicalItemNameSelected="";
					if(listMedical.GetSelected<AllergyDef>()!=null && listMedical.GetSelected<AllergyDef>() is AllergyDef){
						medicalItemNameSelected=listMedical.GetSelected<AllergyDef>().Description;
					}
					if(listMedical.GetSelected<DiseaseDef>()!=null && listMedical.GetSelected<DiseaseDef>() is DiseaseDef){
						medicalItemNameSelected=listMedical.GetSelected<DiseaseDef>().DiseaseName;
					}
					fieldNameSelected+=":"+medicalItemNameSelected;
				}
				if(radioNo.Checked || fieldNameSelected.StartsWith("checkMed")) {
					radioButtonValue="N";
					radioItemValue="No";//Default
					if(!textMobileCheckOverride.Text.IsNullOrEmpty()) {
						radioItemValue=textMobileCheckOverride.Text;
					}
				}
				else {
					radioButtonValue="Y";
					radioItemValue="Yes";//Default
					if(!textMobileCheckOverride.Text.IsNullOrEmpty()) {
						radioItemValue=textMobileCheckOverride.Text;
					}
				}
			}
			#endregion
			if(groupRadio.Visible && listRadio.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a Radio Button Value first.");
				return;
			}
			SheetFieldDefCur.FieldName=fieldNameSelected;
			SheetFieldDefCur.ReportableName=textReportableName.Text;//always safe even if not a misc field or if textReportableName is blank.
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			//We will set these below where applicable.
			SheetFieldDefCur.RadioButtonGroup="";
			SheetFieldDefCur.UiLabelMobile="";
			SheetFieldDefCur.UiLabelMobileRadioButton="";
			SheetFieldDefCur.RadioButtonValue=radioButtonValue;
			Action<string> updateGroupCaptionForFieldName=new Action<string>((caption) => {
				SheetFieldDefCur.UiLabelMobile=caption;
				List<SheetFieldDef> listSheetFieldDefs=SheetDefCur.SheetFieldDefs.FindAll(x =>
					x.FieldType==SheetFieldType.CheckBox &&
					!string.IsNullOrEmpty(x.FieldName) &&
					x.FieldName==SheetFieldDefCur.FieldName &&
					x.Language==SheetFieldDefCur.Language);
				for(int i=0;i<listSheetFieldDefs.Count;i++){
					listSheetFieldDefs[i].UiLabelMobile=caption;
				}
			});
			if(groupRadio.Visible) {
				//All items with this group name get this UiLabelMobile.
				updateGroupCaptionForFieldName(textUiLabelMobile.Text);
				if(!textItemOverride.Text.IsNullOrEmpty()) {
					SheetFieldDefCur.UiLabelMobileRadioButton=textItemOverride.Text;
				}
				if(listRadio.SelectedIndex>=0) {
					SheetFieldDefCur.RadioButtonValue=_listRadioButtonVals[listRadio.SelectedIndex];
				}
			}
			else if(groupRadioMisc.Visible){
				SheetFieldDefCur.RadioButtonGroup=comboRadioGroupNameMisc.Text;
				SheetFieldDefCur.UiLabelMobile=textUiLabelMobileMisc.Text;
				SheetFieldDefCur.UiLabelMobileRadioButton=textUiLabelMobileRadioButtonMisc.Text;
				//All items with this group name get this UiLabelMobile.
				List<SheetFieldDef> listSheetFieldDefs2=SheetDefCur.SheetFieldDefs.FindAll(x =>
					x.FieldType==SheetFieldType.CheckBox &&
					!string.IsNullOrEmpty(x.FieldName) &&
					x.FieldName==SheetFieldDefCur.FieldName &&
					x.Language==SheetFieldDefCur.Language);
				for(int i=0;i<listSheetFieldDefs2.Count;i++){
					listSheetFieldDefs2[i].UiLabelMobile=SheetFieldDefCur.UiLabelMobile;
				}
			}
			else if(SheetDefCur.SheetType==SheetTypeEnum.MedicalHistory) {
				//All items with this group name get this UiLabelMobile.
				string medicalItemNameSelected="";
				if(listMedical.GetSelected<AllergyDef>()!=null && listMedical.GetSelected<AllergyDef>() is AllergyDef){
					medicalItemNameSelected=listMedical.GetSelected<AllergyDef>().Description;
				}
				if(listMedical.GetSelected<DiseaseDef>()!=null && listMedical.GetSelected<DiseaseDef>() is DiseaseDef){
					medicalItemNameSelected=listMedical.GetSelected<DiseaseDef>().DiseaseName;
				}
				if(textMobileMedicalNameOverride.Visible && !textMobileMedicalNameOverride.Text.IsNullOrEmpty()) {
					medicalItemNameSelected=textMobileMedicalNameOverride.Text;
				}
				updateGroupCaptionForFieldName(medicalItemNameSelected);
				SheetFieldDefCur.UiLabelMobileRadioButton=radioItemValue;
			}
			else if(labelUiLabelMobileCheckBoxNonMisc.Visible) { 
				//There is no radio button UI displaying to the user so do not set a 'group caption' by invoking updateGroupCaptionForFieldName.
				SheetFieldDefCur.UiLabelMobileRadioButton=textUiLabelMobileCheckBoxNonMisc.Text;
			}
			SheetFieldDefCur.IsRequired=checkRequired.Checked;
			SheetFieldDefCur.TabOrder=PIn.Int(textTabOrder.Text);
			//don't save to database here.
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private enum MedicalListType {
			allergy,
			checkMed,
			problem
		}








	}
}