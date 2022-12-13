using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormRequiredFields:FormODBase {
		private List<RequiredField> _listRequiredFields;
		private RequiredField _requiredField;
		///<summary>Keeps track of which enum value is at which index.</summary>
		private List<RequiredFieldName> _listRequiredFieldNames;

		public FormRequiredFields() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRequiredFields_Load(object sender,EventArgs e) {
			labelExplanation.Text="";//This will be filled later, but blank will function as Visible=false
			FillMain();
			RequiredFieldType[] arrayRequiredFieldTypes=(RequiredFieldType[])Enum.GetValues(typeof(RequiredFieldType));
			for(int i=0;i<arrayRequiredFieldTypes.Length;i++) {
				comboFieldTypes.Items.Add(Lan.g("enumRequiredFieldType",arrayRequiredFieldTypes[i].ToString()));
			}
			comboFieldTypes.SelectedIndex=0;
			FillAvailable();
			checkMedicaidLength.Checked=PrefC.GetBool(PrefName.EnforceMedicaidIDLength);
		}

		private void FillAvailable() {
			_listRequiredFields=RequiredFields.GetDeepCopy();
			listAvailableFields.Items.Clear();
			_listRequiredFieldNames=new List<RequiredFieldName>();
			if(comboFieldTypes.SelectedIndex>=0) {//if a selection
				switch(comboFieldTypes.SelectedIndex) {
					case 0:
						_listRequiredFieldNames=RequiredFields.GetFieldNamesForType(RequiredFieldType.PatientInfo);
						break;
					case 1:
						_listRequiredFieldNames=RequiredFields.GetFieldNamesForType(RequiredFieldType.InsPayEdit);
						break;
				}
			}
			RequiredFieldName[] arrayRequiredFieldNames=(RequiredFieldName[])Enum.GetValues(typeof(RequiredFieldName));
			for(int i=0;i<arrayRequiredFieldNames.Length;i++) {
				switch(arrayRequiredFieldNames[i]) {
					case RequiredFieldName.AdmitDate:
						if(PrefC.GetBool(PrefName.EasyHideHospitals)) {
							continue;//Don't include AdmitDate in the list if hospitals is not enabled
						}
						break;
					case RequiredFieldName.TrophyFolder:
						if(!Programs.IsEnabled(Programs.GetProgramNum(ProgramName.TrophyEnhanced))) {
							continue;//Don't include TrophyFolder in the list if TrophyEnhanced is not enabled
						}
						break;
					case RequiredFieldName.Ward:
						if(PrefC.GetBool(PrefName.EasyHideHospitals)) {
							continue;//Don't include Ward in the list if hospitals is not enabled
						}
						break;
					case RequiredFieldName.Clinic:
						if(!PrefC.HasClinicsEnabled) {
							continue;//Don't include Clinic in the list if clinics is not enabled
						}
						break;
					case RequiredFieldName.PatientStatus:
					case RequiredFieldName.Position:
						continue;//There is no way to not select these.
					case RequiredFieldName.MothersMaidenFirstName:
					case RequiredFieldName.MothersMaidenLastName:
					case RequiredFieldName.DateTimeDeceased:
						if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
							continue;//EHR features
						}
						break;
					case RequiredFieldName.StudentStatus:
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							continue;//Canada uses Eligibility Excep. Code instead of Student Status radio buttons
						}
						break;
					case RequiredFieldName.EligibilityExceptCode:
						if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) { // Not Canadian. en-CA or fr-CA
							continue;//Don't show EligibilityExceptCode
						}
						break;
					case RequiredFieldName.MedicaidID:
					case RequiredFieldName.MedicaidState:
						if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
							continue;//Don't show Medicaid fields
						}
						break;
					case RequiredFieldName.Race:
					case RequiredFieldName.County:
					case RequiredFieldName.Site:
					case RequiredFieldName.GradeLevel:
					case RequiredFieldName.TreatmentUrgency:
					case RequiredFieldName.ResponsibleParty:
						if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) { 
							continue;//Don't show Public Health fields
						}
						break;
					case RequiredFieldName.Ethnicity:
						if(PrefC.GetBool(PrefName.EasyHidePublicHealth)
							|| !PrefC.GetBool(PrefName.ShowFeatureEhr))
						{
							continue;//Don't show Ethnicity
						}
						break;
					default:
						break;
				}
			}
			List<RequiredFieldName> listRequiredFieldNames=_listRequiredFields.Select(y => y.FieldName).Distinct().ToList();
			_listRequiredFieldNames=_listRequiredFieldNames.FindAll(x => !listRequiredFieldNames.Any(y => y==x));
			listAvailableFields.Items.AddList(_listRequiredFieldNames,x => x.ToString());
		}

		private void FillMain() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Field Name"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Conditions"),80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listRequiredFields=RequiredFields.GetDeepCopy();
			for(int i=0;i<_listRequiredFields.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listRequiredFields[i].FieldName.ToString());
				int conditionsCount=_listRequiredFields[i].ListRequiredFieldConditions.Count;
				if(conditionsCount>0) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}				
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillConditions() {
			gridConditions.BeginUpdate();
			gridConditions.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Type"),120);
			gridConditions.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Operator"),80,HorizontalAlignment.Center);
			gridConditions.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Value"),150);
			gridConditions.Columns.Add(col);
			gridConditions.ListGridRows.Clear();
			GridRow row;
			RequiredFieldName requiredFieldNameType;
			string allCondValues="";
			for(int i=0;i<_requiredField.ListRequiredFieldConditions.Count;i++) {
				row=new GridRow();
				requiredFieldNameType=_requiredField.ListRequiredFieldConditions[i].ConditionType;
				if(requiredFieldNameType==RequiredFieldName.Birthdate) {
					row.Cells.Add(Lan.g("enumRequiredFieldName","Age"));
				}
				else {
					row.Cells.Add(Lan.g("enumRequiredFieldName",requiredFieldNameType.ToString()));
				}
				if(_requiredField.ListRequiredFieldConditions[i].Operator==ConditionOperator.NotEquals) {
					row.Cells.Add(Lan.g(this,"Is not"));
				}
				else {
					row.Cells.Add(Lan.g(this,"Is"));//If the operator is <,>,<=, or >=, this will be reflected in the 'Value' column
				}
				//Construct the string for the 'Value' column
				if(requiredFieldNameType==RequiredFieldName.Birthdate || requiredFieldNameType==RequiredFieldName.AdmitDate || requiredFieldNameType==RequiredFieldName.DateTimeDeceased) {
					//Continuous values
					//'Value' column will end up looking like:  Greater than 18 and
					//																					less than 26
					switch(_requiredField.ListRequiredFieldConditions[i].Operator) {
						case ConditionOperator.GreaterThan:
							allCondValues+=Lan.g(this,"greater than")+" ";
							break;
						case ConditionOperator.LessThan:
							allCondValues+=Lan.g(this,"less than")+" ";
							break;
						case ConditionOperator.GreaterThanOrEqual:
							allCondValues+=Lan.g(this,"greater than or equal to")+" ";
							break;
						case ConditionOperator.LessThanOrEqual:
							allCondValues+=Lan.g(this,"less than or equal to")+" ";
							break;
					}
					if(allCondValues.Length>0) {
						allCondValues=allCondValues[0].ToString().ToUpper()+allCondValues.Substring(1);//Turn the first letter to uppercase
					}
				}
				string condVal=_requiredField.ListRequiredFieldConditions[i].ConditionValue;
				switch(requiredFieldNameType) {
					//These RequiredFieldName types store a FK to another value
					case RequiredFieldName.Clinic:
						if(condVal=="0") {
							condVal=Lan.g(this,"Unassigned");
						}
						else {
							condVal=Clinics.GetDesc(PIn.Long(condVal));
						}
						break;
					case RequiredFieldName.BillingType:
						condVal=Defs.GetName(DefCat.BillingTypes,PIn.Long(condVal));
						break;
					case RequiredFieldName.PrimaryProvider:
						condVal=Providers.GetLongDesc(PIn.Long(condVal));
						break;
				}
				allCondValues+=condVal;
				if(i<_requiredField.ListRequiredFieldConditions.Count-1
					&& _requiredField.ListRequiredFieldConditions[i+1].ConditionType==requiredFieldNameType) //The next condition is of the same type
				{
					if((requiredFieldNameType==RequiredFieldName.Birthdate || requiredFieldNameType==RequiredFieldName.AdmitDate || requiredFieldNameType==RequiredFieldName.DateTimeDeceased)
						&& _requiredField.ListRequiredFieldConditions[i].ConditionRelationship==LogicalOperator.And)
					{
						allCondValues+=" "+Lan.g(this,"and")+"\r\n";
					}
					else {
						allCondValues+=" "+Lan.g(this,"or")+"\r\n";
					}
					//Don't add the row, continue building the 'Value' string
				}
				else {//The last condition of this type, now add the row
					row.Cells.Add(allCondValues);
					row.Tag=requiredFieldNameType;
					gridConditions.ListGridRows.Add(row);
					allCondValues="";//for rebuilding the next Value string
				}
			}
			gridConditions.EndUpdate();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			checkMedicaidLength.Visible=false;
			if(gridMain.SelectedIndices.Length!=1) {
				labelExplanation.Text="";
				gridConditions.BeginUpdate();
				gridConditions.ListGridRows.Clear();
				gridConditions.EndUpdate();
				return;
			}
			_requiredField=_listRequiredFields[gridMain.SelectedIndices[0]];
			labelExplanation.Text=Lan.g("enumRequiredFieldName",_requiredField.FieldName.ToString())+" ";
			switch(_requiredField.FieldName) {
				case RequiredFieldName.PreferContactMethod:
				case RequiredFieldName.PreferConfirmMethod:
				case RequiredFieldName.PreferRecallMethod:
				case RequiredFieldName.Language:
				case RequiredFieldName.SecondaryProvider:
				case RequiredFieldName.FeeSchedule:
				case RequiredFieldName.Race:
				case RequiredFieldName.Ethnicity:
				case RequiredFieldName.InsuranceSubscriber:
					labelExplanation.Text+=Lan.g(this,"cannot be 'None'.");
					break;
				case RequiredFieldName.Clinic:
					labelExplanation.Text+=Lan.g(this,"cannot be 'Unassigned'.");
					break;
				case RequiredFieldName.StudentStatus:
					labelExplanation.Text+=Lan.g(this,"must be chosen.");
					break;
				case RequiredFieldName.TextOK:
					labelExplanation.Text+=Lan.g(this,"cannot be '??'.");
					break;
				case RequiredFieldName.EligibilityExceptCode:
					labelExplanation.Text+=Lan.g(this,"cannot be '0 - Please Choose'.");
					break;
				case RequiredFieldName.GradeLevel:
				case RequiredFieldName.TreatmentUrgency:
				case RequiredFieldName.Gender:
					labelExplanation.Text+=Lan.g(this,"cannot be 'Unknown'.");
					break;
				case RequiredFieldName.PrimaryProvider:
					labelExplanation.Text+=Lan.g(this,"cannot be 'Select Provider'.");
					break;
				case RequiredFieldName.MedicaidID:
					labelExplanation.Text+=Lan.g(this,"cannot be blank.");
					break;
				case RequiredFieldName.MedicaidState:
					checkMedicaidLength.Visible=true;
					labelExplanation.Text+=Lan.g(this,"cannot be blank and must be a valid state abbreviation.");
					break;
				case RequiredFieldName.State:
					labelExplanation.Text+=Lan.g(this,"cannot be blank and must be a valid state abbreviation.");
					break;
				default:
					labelExplanation.Text+=Lan.g(this,"cannot be blank.");
					break;
			}
			FillConditions();
		}

		private void gridConditions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReqFieldCondEdit formReqFieldCondEdit=new FormReqFieldCondEdit();
			formReqFieldCondEdit.IsNew=false;
			formReqFieldCondEdit.RequiredField_=_requiredField;
			formReqFieldCondEdit.RequiredFieldName_=(RequiredFieldName)gridConditions.ListGridRows[e.Row].Tag;
			formReqFieldCondEdit.ListRequiredFieldConditions=_requiredField.ListRequiredFieldConditions.FindAll(x => x.ConditionType==(RequiredFieldName)gridConditions.ListGridRows[e.Row].Tag);
			formReqFieldCondEdit.ShowDialog();
			if(formReqFieldCondEdit.DialogResult==DialogResult.OK) {
				_requiredField.RefreshConditions();
				int mainSelectedIndex=gridMain.SelectedIndices[0];
				FillMain();
				FillConditions();
				gridMain.SetSelected(mainSelectedIndex,true);
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(listAvailableFields.SelectedIndices.Count==0) {
				return;
			}
			RequiredField requiredField;
			for(int i=0;i<listAvailableFields.SelectedIndices.Count;i++) {
				requiredField=new RequiredField();
				requiredField.FieldType=(RequiredFieldType)comboFieldTypes.SelectedIndex;
				requiredField.FieldName=_listRequiredFieldNames[listAvailableFields.SelectedIndices[i]];
				RequiredFields.Insert(requiredField);
			}			
			RequiredFields.RefreshCache();
			FillAvailable();
			FillMain();
			gridConditions.BeginUpdate();
			gridConditions.ListGridRows.Clear();
			gridConditions.EndUpdate();
			labelExplanation.Text="";
			checkMedicaidLength.Visible=false;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				return;
			}
			//If any of the selected required fields have a field condition, prompt the user to confirm before deleting the conditions
			if(_listRequiredFields.FindAll(x => gridMain.SelectedIndices.Contains(_listRequiredFields.IndexOf(x))).Any(x => x.ListRequiredFieldConditions.Count>0)
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Do you want to delete the selected required fields and any conditions from the list?"))
			{
				return;
			}
			RequiredField requiredField;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				requiredField=_listRequiredFields[gridMain.SelectedIndices[i]];
				RequiredFields.Delete(requiredField.RequiredFieldNum);
			}
			RequiredFields.RefreshCache();
			if(_requiredField.FieldName==RequiredFieldName.MedicaidState) {
				checkMedicaidLength.Checked=false;
			}
			FillAvailable();
			FillMain();
			gridConditions.BeginUpdate();
			gridConditions.ListGridRows.Clear();
			gridConditions.EndUpdate();
			labelExplanation.Text="";
			checkMedicaidLength.Visible=false;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select one required field first.");
				return;
			}
			int mainSelected=gridMain.SelectedIndices[0];
			using FormReqFieldCondEdit formReqFieldCondEdit=new FormReqFieldCondEdit();
			formReqFieldCondEdit.IsNew=true;
			formReqFieldCondEdit.RequiredField_=_requiredField;
			formReqFieldCondEdit.ShowDialog();
			if(formReqFieldCondEdit.DialogResult==DialogResult.OK) {
				int conditionsBeforeCount=_requiredField.ListRequiredFieldConditions.Count;
				_requiredField.RefreshConditions();
				FillConditions();
				if(conditionsBeforeCount==0) {
					//So that an 'X' will appear in the 'Conditions' column
					FillMain();
					gridMain.SetSelected(mainSelected,true);
				}
			}
		}

		private void comboFieldTypes_SelectedIndexChanged(object sender,EventArgs e) {
			FillAvailable();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridConditions.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a condition first.");
				return;
			}
			RequiredFieldName requiredFieldNameSelectedType;
			for(int i=0;i<gridConditions.SelectedIndices.Length;i++) {
				//Delete all the conditions of the same type
				requiredFieldNameSelectedType=(RequiredFieldName)gridConditions.ListGridRows[gridConditions.SelectedIndices[i]].Tag;
				RequiredFieldConditions.DeleteAll(_requiredField.ListRequiredFieldConditions.FindAll(x => x.ConditionType==requiredFieldNameSelectedType)
					.Select(x => x.RequiredFieldConditionNum).ToList());
			}
			_requiredField.RefreshConditions();
			FillConditions();
			if(_requiredField.ListRequiredFieldConditions.Count==0) {
				//To remove the 'X' from the 'Conditions' column
				int mainSelected=gridMain.SelectedIndices[0];
				FillMain();
				gridMain.SetSelected(mainSelected,true);
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void FormRequiredFields_FormClosing(object sender,FormClosingEventArgs e) {
			if(Prefs.UpdateBool(PrefName.EnforceMedicaidIDLength,checkMedicaidLength.Checked)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DataValid.SetInvalid(InvalidType.RequiredFields);
		}
	}
}