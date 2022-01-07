using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReqFieldCondEdit:FormODBase {
		public bool IsNew;
		///<summary>Holds all the conditions that are the same type as the condition being loaded.</summary>
		private List<RequiredFieldCondition> _listReqFieldConds;
		private List<string> _listLanguages;
		private List<Clinic> _listClinics;
		private List<Provider> _listProvs;
		private RequiredFieldName _originalFieldName;
		///<summary>Keeps track of which enum value is at which index.</summary>
		private List<RequiredFieldName> _listIndexFieldNames;
		private RequiredField _reqField;
		private List<Def> _listBillingTypeDefs;

		public FormReqFieldCondEdit(RequiredField reqField) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_reqField=reqField;
			_listReqFieldConds=new List<RequiredFieldCondition>();
		}

		public FormReqFieldCondEdit(RequiredField reqField,RequiredFieldName condType) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_reqField=reqField;
			_originalFieldName=condType;
			_listReqFieldConds=reqField.ListRequiredFieldConditions.FindAll(x => x.ConditionType==_originalFieldName);
		}

		private void FormReqFieldCondEdit_Load(object sender,EventArgs e) {
			_listLanguages=new List<string>();
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)!="") {
				_listLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			}
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			_listClinics=Clinics.GetDeepCopy();
			_listProvs=Providers.GetDeepCopy(true);
			comboOperator1.Items.Add(">");
			comboOperator1.Items.Add("<");
			comboOperator1.Items.Add("\u2265");//Greater than or equal to
			comboOperator1.Items.Add("\u2264");//Less than or equal to
			comboOperator2.Items.Add(">");
			comboOperator2.Items.Add("<");
			comboOperator2.Items.Add("\u2265");//Greater than or equal to
			comboOperator2.Items.Add("\u2264");//Less than or equal to
			_listIndexFieldNames=new List<RequiredFieldName>();
			if(!PrefC.GetBool(PrefName.EasyHideHospitals)) {
				AddListConditionType(RequiredFieldName.AdmitDate);
			}
			AddListConditionType(RequiredFieldName.Birthdate);
			AddListConditionType(RequiredFieldName.BillingType);
			if(PrefC.HasClinicsEnabled) {
				AddListConditionType(RequiredFieldName.Clinic);
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				AddListConditionType(RequiredFieldName.DateTimeDeceased);
			}
			AddListConditionType(RequiredFieldName.Gender);
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)!="") {
				AddListConditionType(RequiredFieldName.Language);
			}
			if(!PrefC.GetBool(PrefName.EasyHideMedicaid)) {
				AddListConditionType(RequiredFieldName.MedicaidID);
				AddListConditionType(RequiredFieldName.MedicaidState);
			}
			AddListConditionType(RequiredFieldName.Position);
			AddListConditionType(RequiredFieldName.PatientStatus);
			AddListConditionType(RequiredFieldName.PrimaryProvider);
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {// Not Canadian. en-CA or fr-CA
				AddListConditionType(RequiredFieldName.StudentStatus);
			}
			LayoutManager.MoveLocation(textConditionValue1,new Point(textConditionValue1.Location.X,comboOperator1.Location.Y));
			LayoutManager.MoveLocation(textConditionValue2,new Point(textConditionValue2.Location.X,comboOperator2.Location.Y));
			LayoutManager.MoveLocation(labelConditionValue2,new Point(labelConditionValue2.Location.X,textConditionValue1.Location.Y+21));
			this.Height-=70;//To take care of the space left by the Conditional Value textboxes
		}

		///<summary>Adds the RequiredFieldName to listConditionType, selects the appropriate SelectedIndex, and adds the RequiredFieldName to the 
		///list keeping track of indices.</summary>
		private void AddListConditionType(RequiredFieldName fieldName) {
			if(IsNew && _reqField.ListRequiredFieldConditions.Exists(x => x.ConditionType==fieldName)) {
				return;//Do not add to list if the required field already has a condition of that type
			}
			if(fieldName==RequiredFieldName.Birthdate) {
				listConditionType.Items.Add(Lan.g("enumRequiredFieldName","Age"));
			}
			else {
				listConditionType.Items.Add(Lan.g("enumRequiredFieldName",fieldName.ToString()));
			}
			_listIndexFieldNames.Add(fieldName);
			if(fieldName==_originalFieldName) {
				listConditionType.SelectedIndex=listConditionType.Items.Count-1;
			}
		}

		///<summary>Sets the combo operator selected index. The comboOperator parameter is which combo box is being set. 1 sets comboOperator1 and 
		///2 sets comboOperator2.</summary>
		private void SetOperatorCombo(int comboOperator,ConditionOperator condOp) {
			if(comboOperator==1) {
				comboOperator1.SelectedIndex=(int)condOp-2;//-2 since Equals and NotEquals are never in the combo box
			}
			else {//comboOperator==2
				comboOperator2.SelectedIndex=(int)condOp-2;//-2 since Equals and NotEquals are never in the combo box
			}
		}

		private void listConditionType_SelectedIndexChanged(object sender,EventArgs e) {
			//Intentionally using SelectedIndexChanged so that this code will run when the form loads
			if(listConditionType.SelectedIndex==-1) {
				return;
			}
			RequiredFieldName selectedField=_listIndexFieldNames[listConditionType.SelectedIndex];
			switch(selectedField) {
				case RequiredFieldName.DateTimeDeceased:
				case RequiredFieldName.AdmitDate:
					SetFieldVisibleHelper(false);
					labelConditionValue2.Visible=true;
					if(selectedField!=_originalFieldName) {
						break;
					}
					if(_listReqFieldConds.Count>0) {
						textConditionValue1.Text=_listReqFieldConds[0].ConditionValue;
						SetOperatorCombo(1,_listReqFieldConds[0].Operator);
					}
					if(_listReqFieldConds.Count>1) {
						textConditionValue2.Text=_listReqFieldConds[1].ConditionValue;
						SetOperatorCombo(2,_listReqFieldConds[1].Operator);
						if(_listReqFieldConds[1].ConditionRelationship==LogicalOperator.And) {
							listRelationships.SelectedIndex=0;
						}
						else { //'Or'
							listRelationships.SelectedIndex=1;
						}
					}
					break;
				case RequiredFieldName.Birthdate:
					SetFieldVisibleHelper(false);
					if(selectedField!=_originalFieldName) {
						break;
					}
					if(_listReqFieldConds.Count>0) {
						textConditionValue1.Text=_listReqFieldConds[0].ConditionValue;
						SetOperatorCombo(1,_listReqFieldConds[0].Operator);
					}
					if(_listReqFieldConds.Count>1) {
						textConditionValue2.Text=_listReqFieldConds[1].ConditionValue;
						SetOperatorCombo(2,_listReqFieldConds[1].Operator);
						if(_listReqFieldConds[1].ConditionRelationship==LogicalOperator.And) {
							listRelationships.SelectedIndex=0;
						}
						else { //'Or'
							listRelationships.SelectedIndex=1;
						}
					}
					break;
				case RequiredFieldName.Gender:
					SetFieldVisibleHelper(true);
					PatientGender[] arrayPatientGenders=(PatientGender[])Enum.GetValues(typeof(PatientGender));
					for(int i=0;i<arrayPatientGenders.Length;i++) {
						listConditionValues.Items.Add(Lan.g("enumPatientGender",arrayPatientGenders[i].ToString()));
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.Position:
					SetFieldVisibleHelper(true);
					PatientPosition[] arrayPatientPositions=(PatientPosition[])Enum.GetValues(typeof(PatientPosition));
					for(int i=0;i<arrayPatientPositions.Length;i++) {
						listConditionValues.Items.Add(Lan.g("enumPatientPosition",arrayPatientPositions[i].ToString()));
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.PatientStatus:
					SetFieldVisibleHelper(true);
					PatientStatus[] arrayPatientStatuses=(PatientStatus[])Enum.GetValues(typeof(PatientStatus));
					for(int i=0;i<arrayPatientStatuses.Length;i++) {
						listConditionValues.Items.Add(Lan.g("enumPatientStatus",arrayPatientStatuses[i].ToString()));
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.StudentStatus:
					SetFieldVisibleHelper(true);
					listConditionValues.Items.Add(Lan.g(this,"Nonstudent"));
					listConditionValues.Items.Add(Lan.g(this,"Parttime"));
					listConditionValues.Items.Add(Lan.g(this,"Fulltime"));
					ListValuesSetIndices();
					break;

				case RequiredFieldName.Language:
					SetFieldVisibleHelper(true);
					listConditionValues.Items.Add(Lan.g(this,"none"));//regardless of how many languages are listed, the first item is "none"
					for(int i=0;i<_listLanguages.Count;i++) {
						if(_listLanguages[i]=="") {
							continue;
						}
						CultureInfo culture=CodeBase.MiscUtils.GetCultureFromThreeLetter(_listLanguages[i]);
						if(culture==null) {//custom language
							listConditionValues.Items.Add(_listLanguages[i]);
						}
						else {
							listConditionValues.Items.Add(culture.DisplayName);
						}
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.Clinic:
					SetFieldVisibleHelper(true);
					listConditionValues.Items.Add(Lan.g(this,"Unassigned"));
					for(int i=0;i<_listClinics.Count;i++) {
						listConditionValues.Items.Add(_listClinics[i].Abbr);
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.BillingType:
					SetFieldVisibleHelper(true);
					for(int i=0;i<_listBillingTypeDefs.Count;i++) {
						listConditionValues.Items.Add(_listBillingTypeDefs[i].ItemName);
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.PrimaryProvider:
					SetFieldVisibleHelper(true);
					butPickProv.Visible=true;
					for(int i=0;i<_listProvs.Count;i++) {
						listConditionValues.Items.Add(_listProvs[i].GetLongDesc());//Only visible provs added to combobox.
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.MedicaidID:
					SetFieldVisibleHelper(true);
					listConditionValues.Items.Add(Lan.g(this,"Blank"));
					ListValuesSetIndices();
					break;
				case RequiredFieldName.MedicaidState:
					SetFieldVisibleHelper(true);
					listConditionValues.Items.Add(Lan.g(this,"Blank"));
					ListValuesSetIndices();
					break;
			}
		}

		private void SetFieldVisibleHelper(bool isListBox) {
			butPickProv.Visible=false;
			labelConditionValue2.Visible=false;
			listRelationships.Items.Clear();
			if(isListBox) {
				comboOperator1.Visible=false;
				comboOperator2.Visible=false;
				listConditionValues.Visible=true;
				listConditionValues.Items.Clear();
				textConditionValue1.Visible=false;
				textConditionValue2.Visible=false;
				listRelationships.Items.Add("Is");
				listRelationships.Items.Add("Is not");
			}
			else {
				comboOperator1.Visible=true;
				comboOperator2.Visible=true;
				listConditionValues.Visible=false;
				textConditionValue1.Visible=true;
				textConditionValue1.Text="";
				textConditionValue2.Visible=true;
				textConditionValue2.Text="";
				listRelationships.Items.Add("And");
				listRelationships.Items.Add("Or");
			}
		}

		private void ListValuesSetIndices() {
			RequiredFieldName selectedType=_listIndexFieldNames[listConditionType.SelectedIndex];
			if(selectedType!=_originalFieldName) {
				return;
			}
			if(selectedType==RequiredFieldName.Clinic 
				&& _listReqFieldConds.Exists(x => x.ConditionValue=="0"))
			{
				listConditionValues.SelectedIndices.Add(0);//Select 'Unassigned'
			}
			for(int i=0;i<listConditionValues.Items.Count;i++) {
				if(selectedType==RequiredFieldName.BillingType
					&& _listReqFieldConds.Exists(x => x.ConditionValue==_listBillingTypeDefs[i].DefNum.ToString()))
				{
					listConditionValues.SelectedIndices.Add(i);
					continue;
				}
				if(selectedType==RequiredFieldName.Clinic
					&& i>0
					&& _listReqFieldConds.Exists(x => x.ConditionValue==_listClinics[i-1].ClinicNum.ToString()))//subtract 1 for 'Unassigned'
				{
					listConditionValues.SelectedIndices.Add(i);
					continue;
				}
				if(selectedType==RequiredFieldName.PrimaryProvider
					&& _listReqFieldConds.Exists(x => x.ConditionValue==_listProvs[i].ProvNum.ToString()))
				{
					listConditionValues.SelectedIndices.Add(i);
					continue;
				}
				if(_listReqFieldConds.Exists(x => x.ConditionValue==listConditionValues.Items.GetTextShowingAt(i))) {
					listConditionValues.SelectedIndices.Add(i);
				}
			}
			if(_listReqFieldConds.Count>0) {
				if(_listReqFieldConds[0].Operator==ConditionOperator.Equals) {
					listRelationships.SelectedIndex=0;//'Is'
				}
				else {
					listRelationships.SelectedIndex=1;//'Is not'
				}
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick FormP=new FormProviderPick(_listProvs);
			if(listConditionValues.SelectedIndices.Count>0) {//Initial FormP selection
				FormP.SelectedProvNum=_listProvs[listConditionValues.SelectedIndices[0]].ProvNum;
			}
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			listConditionValues.SelectedIndices.Add(_listProvs.FindIndex(x => x.ProvNum==FormP.SelectedProvNum));
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			RequiredFieldConditions.DeleteAll(_listReqFieldConds.Select(x => x.RequiredFieldConditionNum).ToList());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listConditionType.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a condition type.");
				return;
			}
			RequiredFieldName selectedField=_listIndexFieldNames[listConditionType.SelectedIndex];
			RequiredFieldCondition condition;
			switch(selectedField) {
				//Types that are continuous values
				case RequiredFieldName.Birthdate:
				case RequiredFieldName.AdmitDate:
				case RequiredFieldName.DateTimeDeceased:
					if(textConditionValue1.Text=="") {
						MsgBox.Show(this,"Please enter a condition value.");
						return;
					}
					if(comboOperator1.SelectedIndex==-1) {
						MsgBox.Show(this,"Please select an operator from the drop down list.");
						return;
					}
					//Construct the first condition
					RequiredFieldCondition condition1=new RequiredFieldCondition();
					condition1.RequiredFieldNum=_reqField.RequiredFieldNum;
					condition1.ConditionType=selectedField;
					int agePlaceholder;
					DateTime datePlaceholder;
					if(selectedField==RequiredFieldName.Birthdate && !Int32.TryParse(textConditionValue1.Text,out agePlaceholder)) {
						MsgBox.Show(this,"Please enter a valid integer.");
						return;
					}
					else if((selectedField==RequiredFieldName.AdmitDate || selectedField==RequiredFieldName.DateTimeDeceased)
						&& !DateTime.TryParse(textConditionValue1.Text,out datePlaceholder))
					{
						MsgBox.Show(this,"Please enter a valid date.");
						return;
					}
					condition1.ConditionValue=textConditionValue1.Text;
					condition1.Operator=(ConditionOperator)(comboOperator1.SelectedIndex+2);//Plus 2 because Equals and NotEquals are not in the combo box
					RequiredFieldConditions.DeleteAll(_listReqFieldConds.Select(x => x.RequiredFieldConditionNum).ToList());					
					if(textConditionValue2.Text=="") {
						RequiredFieldConditions.Insert(condition1);
						break;
					}
					//Construct the second condition if it is not blank
					if(comboOperator2.SelectedIndex==-1) {
						MsgBox.Show(this,"Please select an operator from the drop down list.");
						return;
					}
					if(listRelationships.SelectedIndex==-1) {
						MsgBox.Show(this,"Please select 'And' or 'Or'.");
						return;
					}
					RequiredFieldCondition condition2=new RequiredFieldCondition();
					condition2.RequiredFieldNum=_reqField.RequiredFieldNum;
					condition2.ConditionType=selectedField;
					if(selectedField==RequiredFieldName.Birthdate && !Int32.TryParse(textConditionValue2.Text,out agePlaceholder)) {
						MsgBox.Show(this,"Please enter a valid integer.");
						return;
					}
					else if((selectedField==RequiredFieldName.AdmitDate || selectedField==RequiredFieldName.DateTimeDeceased)
						&& !DateTime.TryParse(textConditionValue2.Text,out datePlaceholder))
					{
						MsgBox.Show(this,"Please enter a valid date.");
						return;
					}
					condition2.ConditionValue=textConditionValue2.Text;
					condition2.Operator=(ConditionOperator)(comboOperator2.SelectedIndex+2);//Plus 2 because Equals and NotEquals are not in the combo box
					if(listRelationships.SelectedIndex==0) { //'And' is selected
						condition1.ConditionRelationship=LogicalOperator.And;
						condition2.ConditionRelationship=LogicalOperator.And;
					}
					else {//'Or' is selected
						condition1.ConditionRelationship=LogicalOperator.Or;
						condition2.ConditionRelationship=LogicalOperator.Or;
					}
					RequiredFieldConditions.Insert(condition1);
					RequiredFieldConditions.Insert(condition2);
					break;
				//Types that store the foreign key of the value
				case RequiredFieldName.BillingType:
				case RequiredFieldName.Clinic:
				case RequiredFieldName.PrimaryProvider:
					if(listRelationships.SelectedIndex==-1) {
						MsgBox.Show(this,"Please select 'Is' or 'Is not'.");
						return;
					}
					if(listConditionValues.SelectedIndices.Count==0) {
						MsgBox.Show(this,"Please select a condition value.");
						return;
					}
					List<long> listFkNums=new List<long>();
					if(selectedField==RequiredFieldName.PrimaryProvider) {
						for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
							listFkNums.Add(_listProvs[listConditionValues.SelectedIndices[i]].ProvNum);
						}
					}
					else if(selectedField==RequiredFieldName.BillingType) {
						for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
							listFkNums.Add(_listBillingTypeDefs[listConditionValues.SelectedIndices[i]].DefNum);
						}
					}
					else {	//selectedField==RequiredFieldName.Clinic
						for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
							if(listConditionValues.SelectedIndices[i]==0) { //'Unassigned'
								listFkNums.Add(0);
							}
							else {
								listFkNums.Add(_listClinics[listConditionValues.SelectedIndices[i]-1].ClinicNum);//Subtract 1 to account for 'Unassigned'
							}
						}
					}
					//Delete the original conditions
					RequiredFieldConditions.DeleteAll(_listReqFieldConds.Select(x => x.RequiredFieldConditionNum).ToList());
					//Insert the new conditions
					condition=new RequiredFieldCondition();
					condition.RequiredFieldNum=_reqField.RequiredFieldNum;
					condition.ConditionType=selectedField;
					if(listRelationships.SelectedIndex==0) {//'Is' is selected
						condition.Operator=ConditionOperator.Equals;
					}
					else {//'Is not' is selected
						condition.Operator=ConditionOperator.NotEquals;
					}
					for(int i=0;i<listFkNums.Count;i++) {
						condition.ConditionValue=listFkNums[i].ToString();
						RequiredFieldConditions.Insert(condition);
					}
					break;
				//Types that store the value of listConditionValues
				case RequiredFieldName.StudentStatus:
				case RequiredFieldName.PatientStatus:
				case RequiredFieldName.Position:
				case RequiredFieldName.Gender:
				case RequiredFieldName.Language:
				case RequiredFieldName.MedicaidID:
				case RequiredFieldName.MedicaidState:
					if(listRelationships.SelectedIndex==-1) {
						MsgBox.Show(this,"Please select 'Is' or 'Is not'.");
						return;
					}
					if(listConditionValues.SelectedIndices.Count==0) {
						MsgBox.Show(this,"Please select a value.");
						return;
					}
					//Delete the original conditions
					RequiredFieldConditions.DeleteAll(_listReqFieldConds.Select(x => x.RequiredFieldConditionNum).ToList());
					//Insert the new conditions
					condition=new RequiredFieldCondition();
					condition.RequiredFieldNum=_reqField.RequiredFieldNum;
					condition.ConditionType=selectedField;
					if(listRelationships.SelectedIndex==0) {//'Is' is selected
						condition.Operator=ConditionOperator.Equals;
					}
					else {//'Is not' is selected
						condition.Operator=ConditionOperator.NotEquals;
					}
					for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
						condition.ConditionValue=listConditionValues.Items.GetTextShowingAt(listConditionValues.SelectedIndices[i]);
						RequiredFieldConditions.Insert(condition);
					}
					break;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}