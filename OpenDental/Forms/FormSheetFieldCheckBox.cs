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
		private List<string> radioButtonValues;
		private List<AllergyDef> _listAllergies;
		///<summary>True if the sheet type is MedicalHistory.</summary>
		private bool _isMedHistSheet {
			get { return SheetDefCur.SheetType==SheetTypeEnum.MedicalHistory; }
		}
		private List<DiseaseDef> _listDiseaseDefs;
		private string _selectedFieldName {
			get {
				if(!_hasSelectedFieldName) {
					return "";
				}
				return listBoxFields.GetSelected<SheetFieldDef>().FieldName;
			}
		}
		private bool _hasSelectedFieldName {
			get { return listBoxFields.SelectedIndex>=0; }
		}
		private string _selectedMedicalItemName {
			get {
				if(!_hasSelectedMedicalItem) {
					return "";
				}
				if(listMedical.GetSelected<AllergyDef>()!=null && listMedical.GetSelected<AllergyDef>() is AllergyDef){
					return listMedical.GetSelected<AllergyDef>().Description;
				}
				if(listMedical.GetSelected<DiseaseDef>()!=null && listMedical.GetSelected<DiseaseDef>() is DiseaseDef){
					return listMedical.GetSelected<DiseaseDef>().DiseaseName;
				}
				return "";
			}
		}
		private bool _hasSelectedMedicalItem {
			get { return listMedical.SelectedIndex>=0; }
		}

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
			textUiLabelMobileCheckBoxNonMisc.Text=SheetFieldDefCur.UiLabelMobile;
			textMobileMedicalNameOverride.Text=SheetFieldDefCur.UiLabelMobile;
			textMobileMedicalNameOverride.Text=SheetFieldDefCur.UiLabelMobile;
			textItemOverride.Text=SheetFieldDefCur.UiLabelMobileRadioButton;
			//Not allowed to change sheettype or fieldtype once created.  So get all avail fields for this sheettype.
			//These names will not include the ':' for allergies and problems.
			List<SheetFieldDef> listSheetFieldDefs=SheetFieldsAvailable.GetList(SheetDefCur.SheetType,OutInCheck.Check);
			//If an existing SheetFieldDefCur is not found in the list, add it so we maintain current selection.
			if(ListTools.In(SheetFieldDefCur.FieldName,"FluorideProc","AssessmentProc") || SheetFieldDefCur.FieldName.StartsWith("Proc:")){
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
			SheetDefCur.SheetFieldDefs
				.Where(x => !string.IsNullOrEmpty(x.RadioButtonGroup))
				.GroupBy(x => x.RadioButtonGroup)
				.Select(x => x.Key)
				.ForEach(x => { comboRadioGroupNameMisc.Items.Add(x); });
			if(_isMedHistSheet) {
				radioYes.Checked=true;
				if(_selectedFieldName=="allergy") {
					//Will be of format allergy:Aspirin
					FillListMedical(MedicalListType.allergy);
				}
				else if(_selectedFieldName=="problem") {
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
			if(_selectedFieldName=="allergy") {
				//if nothing is selected we didn't find it, prompt to add
				if(listMedical.SelectedIndex<=-1) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Allergy does not exist in list. Would you like to add the allergy?")){
						AddAllergy(SheetFieldDefCur);
					}
				}
			}
			else if(_selectedFieldName=="problem") {
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
					if(_listAllergies==null) {
						_listAllergies=AllergyDefs.GetAll(false);
					}
					listMedical.Items.AddList(_listAllergies, x => x.Description);
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
			if(!_hasSelectedFieldName) {
				return;
			}
			if(_isMedHistSheet) {
				labelRequired.Visible=true;
				checkRequired.Visible=true;
				switch(_selectedFieldName) {
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
			if(_selectedFieldName=="misc") {
				labelMiscInstructions.Visible=true;
				labelReportableName.Visible=true;
				textReportableName.Visible=true;
				textReportableName.Text=SheetFieldDefCur.ReportableName;//will either be "" or saved ReportableName.
				groupRadioMisc.Visible=true;
				labelRequired.Visible=true;
				checkRequired.Visible=true;
			}
			else if(_isMedHistSheet) {
				return;
			}
			else {
				textReportableName.Text="";
				radioButtonValues=SheetFieldsAvailable.GetRadio(_selectedFieldName);
				if(radioButtonValues.Count==0) { //Rare, currently only addressAndHmPhoneIsSameEntireFamily.
					labelUiLabelMobileCheckBoxNonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
					textUiLabelMobileCheckBoxNonMisc.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
					return;
				}
				groupRadio.Visible=true;
				labelRequired.Visible=true;
				checkRequired.Visible=true;
				listRadio.Items.Clear();
				for(int i=0;i<radioButtonValues.Count;i++) {
					listRadio.Items.Add(radioButtonValues[i]);
					if(SheetFieldDefCur.RadioButtonValue==radioButtonValues[i]) {
						listRadio.SelectedIndex=i;
					}
				}				
				//Set the mobile group caption.
				var sheetFieldGroup=SheetDefCur.SheetFieldDefs.FirstOrDefault(x =>
					x.FieldType==SheetFieldType.CheckBox &&
					!string.IsNullOrEmpty(x.FieldName) &&
					x.FieldName==_selectedFieldName &&
					x.Language==SheetFieldDefCur.Language);
				textUiLabelMobile.Text=sheetFieldGroup==null ? "" : sheetFieldGroup.UiLabelMobile;
			}
		}

		private void listRadio_Click(object sender,EventArgs e) {
			if(listRadio.SelectedIndex==-1){
				return;
			}
			SheetFieldDefCur.RadioButtonValue=radioButtonValues[listRadio.SelectedIndex];
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
			using FormAllergyDefEdit formADE=new FormAllergyDefEdit();
			formADE.AllergyDefCur=new AllergyDef();
			formADE.AllergyDefCur.IsNew=true;
			formADE.AllergyDefCur.Description=SheetFieldDefCur?.FieldName.Replace("allergy:","")??"";
			formADE.ShowDialog();
			if(formADE.DialogResult!=DialogResult.OK) {
				return;
			}
			_listAllergies.Add(formADE.AllergyDefCur);
			FillListMedical(MedicalListType.allergy);
		}

		private void butAddProblem_Click(object sender,EventArgs e) {
			AddProblem(SheetFieldDefCur);
		}

		private void AddProblem(SheetFieldDef SheetFieldDefCur) {
			if(!Security.IsAuthorized(Permissions.ProblemEdit)) {
				return;
			}
			DiseaseDef def=new DiseaseDef() {
				ICD9Code="",
				Icd10Code="",
				SnomedCode="",
				ItemOrder=DiseaseDefs.GetCount(),
				DiseaseName=SheetFieldDefCur?.FieldName.Replace("problem:","")??""
			};
			using FormDiseaseDefEdit formDDE=new FormDiseaseDefEdit(def,false);
			formDDE.IsNew=true;
			formDDE.ShowDialog();
			if(formDDE.DialogResult!=DialogResult.OK) {
				return;
			}
			DiseaseDefs.Insert(formDDE.DiseaseDefCur);
			DataValid.SetInvalid(InvalidType.Diseases);
			_listDiseaseDefs=DiseaseDefs.GetDeepCopy(true);
			SecurityLogs.MakeLogEntry(Permissions.ProblemEdit,0,formDDE.SecurityLogMsgText);
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
			List<GridColumn> listGridCols=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Code"),70),
				new GridColumn(Lan.g(this,"Abbreviation"),90,HorizontalAlignment.Center),
				new GridColumn(Lan.g(this,"Description"),0,HorizontalAlignment.Right)
			};
			List<ProcedureCode> listMouthProcCodes=ProcedureCodes.GetProcCodesByTreatmentArea(false,TreatmentArea.Mouth,TreatmentArea.None)
				.OrderBy(x => x.ProcCode).ToList();
			List<GridRow> listGridRows=new List<GridRow>();
			listMouthProcCodes.ForEach(x => {
				GridRow row=new GridRow (x.ProcCode,x.AbbrDesc,x.Descript);
				row.Tag=x;
				listGridRows.Add(row);
			});
			using FormGridSelection formGridSelect=new FormGridSelection(listGridCols,listGridRows,"Add Procedure","Procedures");
			if(formGridSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			foreach(object tag in formGridSelect.ListSelectedTags) {
				string fieldName="Proc:"+((ProcedureCode)tag).ProcCode;
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
			if(!_hasSelectedFieldName) {
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
			string fieldName=_selectedFieldName;
			string radioButtonValue="";
			string radioItemValue="";
			#region Medical History Sheet
			if(_isMedHistSheet && fieldName!="misc") {
				if(listMedical.Visible) {
					if(!_hasSelectedMedicalItem) {
						switch(fieldName) {
							case "allergy":
								MsgBox.Show(this,"Please select an allergy first.");
								return;
							case "problem":
								MsgBox.Show(this,"Please select a problem first.");
								return;
						}
					}
					fieldName+=":"+_selectedMedicalItemName;
				}
				if(radioNo.Checked || fieldName.StartsWith("checkMed")) {
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
			SheetFieldDefCur.FieldName=fieldName;
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
				SheetDefCur.SheetFieldDefs
					.Where(x =>
						x.FieldType==SheetFieldType.CheckBox &&
						!string.IsNullOrEmpty(x.FieldName) &&
						x.FieldName==SheetFieldDefCur.FieldName &&
						x.Language==SheetFieldDefCur.Language)
					.ForEach(x => {
						x.UiLabelMobile=caption;
					});
			});
			if(groupRadio.Visible) {
				//All items with this group name get this UiLabelMobile.
				updateGroupCaptionForFieldName(textUiLabelMobile.Text);
				if(!textItemOverride.Text.IsNullOrEmpty()) {
					SheetFieldDefCur.UiLabelMobileRadioButton=textItemOverride.Text;
				}
				if(listRadio.SelectedIndex>=0) {
					SheetFieldDefCur.RadioButtonValue=radioButtonValues[listRadio.SelectedIndex];
				}
			}
			else if(groupRadioMisc.Visible){
				SheetFieldDefCur.RadioButtonGroup=comboRadioGroupNameMisc.Text;
				SheetFieldDefCur.UiLabelMobile=textUiLabelMobileMisc.Text;
				SheetFieldDefCur.UiLabelMobileRadioButton=textUiLabelMobileRadioButtonMisc.Text;
				//All items with this group name get this UiLabelMobile.
				SheetDefCur.SheetFieldDefs
					.Where(x =>
						x.FieldType==SheetFieldType.CheckBox &&
						!string.IsNullOrEmpty(x.RadioButtonGroup) &&
						x.RadioButtonGroup==SheetFieldDefCur.RadioButtonGroup &&
						x.Language==SheetFieldDefCur.Language)
					.ForEach(x => x.UiLabelMobile=SheetFieldDefCur.UiLabelMobile);

			}
			else if(_isMedHistSheet) {
				//All items with this group name get this UiLabelMobile.
				string medicalName=_selectedMedicalItemName;
				if(textMobileMedicalNameOverride.Visible && !textMobileMedicalNameOverride.Text.IsNullOrEmpty()) {
					medicalName=textMobileMedicalNameOverride.Text;
				}
				updateGroupCaptionForFieldName(medicalName);
				SheetFieldDefCur.UiLabelMobileRadioButton=radioItemValue;
			}
			else if(labelUiLabelMobileCheckBoxNonMisc.Visible) { 
				//All items with this group name get this UiLabelMobile.
				updateGroupCaptionForFieldName(textUiLabelMobileCheckBoxNonMisc.Text);
				SheetFieldDefCur.UiLabelMobileRadioButton=string.IsNullOrEmpty(radioItemValue) ? textUiLabelMobileCheckBoxNonMisc.Text : radioItemValue;
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