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
		public List<RequiredFieldCondition> ListRequiredFieldConditions=new List<RequiredFieldCondition>();
		private List<string> _listLanguages;
		private List<Clinic> _listClinics;
		private List<Provider> _listProviders;
		public RequiredFieldName RequiredFieldName_;
		///<summary>Keeps track of which enum value is at which index.</summary>
		private List<RequiredFieldName> _listRequiredFieldNames;
		public RequiredField RequiredField_;
		private List<Def> _listDefsBillingTypes;

		public FormReqFieldCondEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReqFieldCondEdit_Load(object sender,EventArgs e) {
			_listLanguages=new List<string>();
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)!="") {
				_listLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			}
			_listDefsBillingTypes=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			_listClinics=Clinics.GetDeepCopy();
			_listProviders=Providers.GetDeepCopy(true);
			comboOperator1.Items.Add(">");
			comboOperator1.Items.Add("<");
			comboOperator1.Items.Add("\u2265");//Greater than or equal to
			comboOperator1.Items.Add("\u2264");//Less than or equal to
			comboOperator2.Items.Add(">");
			comboOperator2.Items.Add("<");
			comboOperator2.Items.Add("\u2265");//Greater than or equal to
			comboOperator2.Items.Add("\u2264");//Less than or equal to
			_listRequiredFieldNames=new List<RequiredFieldName>();
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
			if(!PrefC.GetBool(PrefName.EasyHideHospitals)) {
				AddListConditionType(RequiredFieldName.DischargeDate);
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
		private void AddListConditionType(RequiredFieldName requiredFieldName) {
			if(IsNew && RequiredField_.ListRequiredFieldConditions.Exists(x => x.ConditionType==requiredFieldName)) {
				return;//Do not add to list if the required field already has a condition of that type
			}
			if(requiredFieldName==RequiredFieldName.Birthdate) {
				listConditionType.Items.Add(Lan.g("enumRequiredFieldName","Age"));
			}
			else {
				listConditionType.Items.Add(Lan.g("enumRequiredFieldName",requiredFieldName.ToString()));
			}
			_listRequiredFieldNames.Add(requiredFieldName);
			if(requiredFieldName==RequiredFieldName_) {
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
			RequiredFieldName requiredFieldNameSelected=_listRequiredFieldNames[listConditionType.SelectedIndex];
			switch(requiredFieldNameSelected) {
				case RequiredFieldName.DateTimeDeceased:
				case RequiredFieldName.AdmitDate:
				case RequiredFieldName.DischargeDate:
					SetFieldVisibleHelper(false);
					labelConditionValue2.Visible=true;
					if(requiredFieldNameSelected!=RequiredFieldName_) {
						break;
					}
					if(ListRequiredFieldConditions.Count>0) {
						textConditionValue1.Text=ListRequiredFieldConditions[0].ConditionValue;
						SetOperatorCombo(1,ListRequiredFieldConditions[0].Operator);
					}
					if(ListRequiredFieldConditions.Count>1) {
						textConditionValue2.Text=ListRequiredFieldConditions[1].ConditionValue;
						SetOperatorCombo(2,ListRequiredFieldConditions[1].Operator);
						if(ListRequiredFieldConditions[1].ConditionRelationship==LogicalOperator.And) {
							listRelationships.SelectedIndex=0;
						}
						else { //'Or'
							listRelationships.SelectedIndex=1;
						}
					}
					break;
				case RequiredFieldName.Birthdate:
					SetFieldVisibleHelper(false);
					if(requiredFieldNameSelected!=RequiredFieldName_) {
						break;
					}
					if(ListRequiredFieldConditions.Count>0) {
						textConditionValue1.Text=ListRequiredFieldConditions[0].ConditionValue;
						SetOperatorCombo(1,ListRequiredFieldConditions[0].Operator);
					}
					if(ListRequiredFieldConditions.Count>1) {
						textConditionValue2.Text=ListRequiredFieldConditions[1].ConditionValue;
						SetOperatorCombo(2,ListRequiredFieldConditions[1].Operator);
						if(ListRequiredFieldConditions[1].ConditionRelationship==LogicalOperator.And) {
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
						CultureInfo cultureInfo=CodeBase.MiscUtils.GetCultureFromThreeLetter(_listLanguages[i]);
						if(cultureInfo==null) {//custom language
							listConditionValues.Items.Add(_listLanguages[i]);
						}
						else {
							listConditionValues.Items.Add(cultureInfo.DisplayName);
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
					for(int i=0;i<_listDefsBillingTypes.Count;i++) {
						listConditionValues.Items.Add(_listDefsBillingTypes[i].ItemName);
					}
					ListValuesSetIndices();
					break;
				case RequiredFieldName.PrimaryProvider:
					SetFieldVisibleHelper(true);
					butPickProv.Visible=true;
					for(int i=0;i<_listProviders.Count;i++) {
						listConditionValues.Items.Add(_listProviders[i].GetLongDesc());//Only visible provs added to combobox.
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
			RequiredFieldName requiredFieldNameSelected=_listRequiredFieldNames[listConditionType.SelectedIndex];
			if(requiredFieldNameSelected!=RequiredFieldName_) {
				return;
			}
			if(requiredFieldNameSelected==RequiredFieldName.Clinic 
				&& ListRequiredFieldConditions.Exists(x => x.ConditionValue=="0"))
			{
				listConditionValues.SelectedIndices.Add(0);//Select 'Unassigned'
			}
			for(int i=0;i<listConditionValues.Items.Count;i++) {
				if(requiredFieldNameSelected==RequiredFieldName.BillingType
					&& ListRequiredFieldConditions.Exists(x => x.ConditionValue==_listDefsBillingTypes[i].DefNum.ToString()))
				{
					listConditionValues.SelectedIndices.Add(i);
					continue;
				}
				if(requiredFieldNameSelected==RequiredFieldName.Clinic
					&& i>0
					&& ListRequiredFieldConditions.Exists(x => x.ConditionValue==_listClinics[i-1].ClinicNum.ToString()))//subtract 1 for 'Unassigned'
				{
					listConditionValues.SelectedIndices.Add(i);
					continue;
				}
				if(requiredFieldNameSelected==RequiredFieldName.PrimaryProvider
					&& ListRequiredFieldConditions.Exists(x => x.ConditionValue==_listProviders[i].ProvNum.ToString()))
				{
					listConditionValues.SelectedIndices.Add(i);
					continue;
				}
				if(ListRequiredFieldConditions.Exists(x => x.ConditionValue==listConditionValues.Items.GetTextShowingAt(i))) {
					listConditionValues.SelectedIndices.Add(i);
				}
			}
			if(ListRequiredFieldConditions.Count>0) {
				if(ListRequiredFieldConditions[0].Operator==ConditionOperator.Equals) {
					listRelationships.SelectedIndex=0;//'Is'
				}
				else {
					listRelationships.SelectedIndex=1;//'Is not'
				}
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(_listProviders);
			if(listConditionValues.SelectedIndices.Count>0) {//Initial formProviderPick selection
				formProviderPick.ProvNumSelected=_listProviders[listConditionValues.SelectedIndices[0]].ProvNum;
			}
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			listConditionValues.SelectedIndices.Add(_listProviders.FindIndex(x => x.ProvNum==formProviderPick.ProvNumSelected));
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			RequiredFieldConditions.DeleteAll(ListRequiredFieldConditions.Select(x => x.RequiredFieldConditionNum).ToList());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listConditionType.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a condition type.");
				return;
			}
			RequiredFieldName requiredFieldNameSelected=_listRequiredFieldNames[listConditionType.SelectedIndex];
			RequiredFieldCondition requiredFieldCondition;
			switch(requiredFieldNameSelected) {
				//Types that are continuous values
				case RequiredFieldName.Birthdate:
				case RequiredFieldName.AdmitDate:
				case RequiredFieldName.DischargeDate:
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
					RequiredFieldCondition requiredFieldCondition1=new RequiredFieldCondition();
					requiredFieldCondition1.RequiredFieldNum=RequiredField_.RequiredFieldNum;
					requiredFieldCondition1.ConditionType=requiredFieldNameSelected;
					int agePlaceholder;
					DateTime datePlaceholder;
					if(requiredFieldNameSelected==RequiredFieldName.Birthdate) {
						try {
							agePlaceholder=Int32.Parse(textConditionValue1.Text);
						} 
						catch {
							MsgBox.Show(this,"Please enter a valid integer.");
							return;
						}
					}
					if(requiredFieldNameSelected==RequiredFieldName.AdmitDate || requiredFieldNameSelected==RequiredFieldName.DateTimeDeceased 
						|| requiredFieldNameSelected==RequiredFieldName.DischargeDate) {
						try {
							datePlaceholder=DateTime.Parse(textConditionValue1.Text);
						} 
						catch {
							MsgBox.Show(this,"Please enter a valid date.");
							return;
						}
					}
					requiredFieldCondition1.ConditionValue=textConditionValue1.Text;
					requiredFieldCondition1.Operator=(ConditionOperator)(comboOperator1.SelectedIndex+2);//Plus 2 because Equals and NotEquals are not in the combo box
					RequiredFieldConditions.DeleteAll(ListRequiredFieldConditions.Select(x => x.RequiredFieldConditionNum).ToList());					
					if(textConditionValue2.Text=="") {
						RequiredFieldConditions.Insert(requiredFieldCondition1);
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
					RequiredFieldCondition requiredFieldCondition2=new RequiredFieldCondition();
					requiredFieldCondition2.RequiredFieldNum=RequiredField_.RequiredFieldNum;
					requiredFieldCondition2.ConditionType=requiredFieldNameSelected;
					if(requiredFieldNameSelected==RequiredFieldName.Birthdate) {
						try {
							agePlaceholder=Int32.Parse(textConditionValue2.Text);
						}
						catch {
							MsgBox.Show(this,"Please enter a valid integer.");
							return;
						}
					}
					if(requiredFieldNameSelected==RequiredFieldName.AdmitDate || requiredFieldNameSelected==RequiredFieldName.DateTimeDeceased 
						|| requiredFieldNameSelected==RequiredFieldName.DischargeDate) {
						try {
							DateTime.Parse(textConditionValue2.Text);
						}
						catch {
							MsgBox.Show(this,"Please enter a valid date.");
							return;
						}
					}
					requiredFieldCondition2.ConditionValue=textConditionValue2.Text;
					requiredFieldCondition2.Operator=(ConditionOperator)(comboOperator2.SelectedIndex+2);//Plus 2 because Equals and NotEquals are not in the combo box
					if(listRelationships.SelectedIndex==0) { //'And' is selected
						requiredFieldCondition1.ConditionRelationship=LogicalOperator.And;
						requiredFieldCondition2.ConditionRelationship=LogicalOperator.And;
					}
					else {//'Or' is selected
						requiredFieldCondition1.ConditionRelationship=LogicalOperator.Or;
						requiredFieldCondition2.ConditionRelationship=LogicalOperator.Or;
					}
					RequiredFieldConditions.Insert(requiredFieldCondition1);
					RequiredFieldConditions.Insert(requiredFieldCondition2);
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
					if(requiredFieldNameSelected==RequiredFieldName.PrimaryProvider) {
						for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
							listFkNums.Add(_listProviders[listConditionValues.SelectedIndices[i]].ProvNum);
						}
					}
					else if(requiredFieldNameSelected==RequiredFieldName.BillingType) {
						for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
							listFkNums.Add(_listDefsBillingTypes[listConditionValues.SelectedIndices[i]].DefNum);
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
					RequiredFieldConditions.DeleteAll(ListRequiredFieldConditions.Select(x => x.RequiredFieldConditionNum).ToList());
					//Insert the new conditions
					requiredFieldCondition=new RequiredFieldCondition();
					requiredFieldCondition.RequiredFieldNum=RequiredField_.RequiredFieldNum;
					requiredFieldCondition.ConditionType=requiredFieldNameSelected;
					if(listRelationships.SelectedIndex==0) {//'Is' is selected
						requiredFieldCondition.Operator=ConditionOperator.Equals;
					}
					else {//'Is not' is selected
						requiredFieldCondition.Operator=ConditionOperator.NotEquals;
					}
					for(int i=0;i<listFkNums.Count;i++) {
						requiredFieldCondition.ConditionValue=listFkNums[i].ToString();
						RequiredFieldConditions.Insert(requiredFieldCondition);
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
					RequiredFieldConditions.DeleteAll(ListRequiredFieldConditions.Select(x => x.RequiredFieldConditionNum).ToList());
					//Insert the new conditions
					requiredFieldCondition=new RequiredFieldCondition();
					requiredFieldCondition.RequiredFieldNum=RequiredField_.RequiredFieldNum;
					requiredFieldCondition.ConditionType=requiredFieldNameSelected;
					if(listRelationships.SelectedIndex==0) {//'Is' is selected
						requiredFieldCondition.Operator=ConditionOperator.Equals;
					}
					else {//'Is not' is selected
						requiredFieldCondition.Operator=ConditionOperator.NotEquals;
					}
					for(int i=0;i<listConditionValues.SelectedIndices.Count;i++) {
						requiredFieldCondition.ConditionValue=listConditionValues.Items.GetTextShowingAt(listConditionValues.SelectedIndices[i]);
						RequiredFieldConditions.Insert(requiredFieldCondition);
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