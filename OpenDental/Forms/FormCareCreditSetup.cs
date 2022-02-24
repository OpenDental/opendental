using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCareCreditSetup:FormODBase {

		private Program _progCur;
		///<summary>List of CareCredit program properties for all clinics.
		///Includes properties with ClinicNum=0, the headquarters properties or properties not assigned to a clinic.</summary>
		private List<ProgramProperty> _listProgProps;
		///<summary>Used to switch back to clinics if user selected a different clinic and validation did not pass.</summary>
		private long _previouslySelectedClinicNum;
		///<summary>Default days for program property.</summary>
		private const int DEFAULT_DAYS=2;
		private bool _hasCareCreditPatFieldDefNum=false;
		public bool DoShowApptViewWindow=false;

		///<summary>Used for comboboxDaysOut. The number of days a user can set.</summary>
		public List<int> ListDaysOut => new List<int>() { 1,2,3 };

		public FormCareCreditSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCareCreditSetup_Load(object sender,EventArgs e) {
			_progCur=Programs.GetCur(ProgramName.CareCredit);
			if(_progCur==null) {
				throw new Exception("The CareCredit program is missing from the database.");//should never happen
			}
			checkEnabled.Checked=_progCur.Enabled;
			checkHideAdvertising.Visible=!_progCur.Enabled;
			if(PrefC.HasClinicsEnabled) {
				if(Security.CurUser.ClinicIsRestricted) {
					if(checkEnabled.Checked) {
						checkEnabled.Enabled=false;
					}
				}
				_previouslySelectedClinicNum=comboClinics.SelectedClinicNum;
				textMerchantNumberPractice.Visible=false;
				labelMerchantNumberPractice.Visible=false;
			}
			else {
				// ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				comboClinics.Visible=false;
				groupClinicSettings.Text="";//This will just show a box around the settings
				labelMerchantNumberClinic.Visible=false;
				textMerchantNumberClinic.Visible=false;
			}
			_listProgProps=ProgramProperties.GetForProgram(_progCur.ProgramNum);
			//We only care that the _progCur was enabled prior to opening the form. Missing program properties would have been added already.
			groupPromotions.Visible=_progCur.Enabled;
			AddNeededProgramProperties(0);
			if(PrefC.HasClinicsEnabled) {
				foreach(Clinic clinicCur in Clinics.GetForUserod(Security.CurUser)) {
					AddNeededProgramProperties(clinicCur.ClinicNum);
				}
			}
			List<PatFieldDef> listPatFieldDef=PatFieldDefs.GetDeepCopy().FindAll(x => x.FieldType==PatFieldType.CareCreditStatus);
			long patFieldDefNum=PIn.Long(ProgramProperties.GetPropValFromList(_listProgProps,ProgramProperties.PropertyDescs.CareCredit.CareCreditPatField));
			comboPatFieldDef.Items.Clear();
			comboPatFieldDef.Items.AddList(listPatFieldDef,x => x.FieldName);
			if(patFieldDefNum==0) {
				if(comboPatFieldDef.Items.Count!=0) {
					comboPatFieldDef.SetSelected(0);
				}
			}
			else {
				comboPatFieldDef.SetSelectedKey<PatFieldDef>(patFieldDefNum,x => x.PatFieldDefNum,x => Lan.g(this,"None"));
			}
			_hasCareCreditPatFieldDefNum=ApptViewItems.GetWhere(x => ListTools.In(x.PatFieldDefNum,listPatFieldDef.Select(y => y.PatFieldDefNum).ToList())).Count>0;
			FillFields(isLoad:true);
		}

		private void FillFields(bool isLoad=false) {
			if(isLoad) { 
				comboPaymentType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaymentTypes,true));
				comboBoxDaysOut.Items.AddList(ListDaysOut,x => x.ToString());
			}
			checkHideAdvertising.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditDoDisableAdvertising));
			string payTypeDefNum=ProgramProperties.GetPropValFromList(_listProgProps,ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType,
				comboClinics.SelectedClinicNum);
			comboPaymentType.SetSelectedDefNum(PIn.Long(payTypeDefNum));
			checkQSBatch.Checked=PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchEnabled,
				comboClinics.SelectedClinicNum));
			groupQSBatch.Visible=checkQSBatch.Checked;
			int daysOut=PIn.Int(ProgramProperties.GetPropValFromList(_listProgProps,ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchDays,
				comboClinics.SelectedClinicNum));
			if(daysOut<=0) {
				daysOut=DEFAULT_DAYS;
			}
			comboBoxDaysOut.SetSelectedKey<int>(daysOut,x => x);
			checkMerchantNumByProv.Visible=PrefC.HasClinicsEnabled;
			checkMerchantNumByProv.Checked=false;
			if(PrefC.HasClinicsEnabled && PIn.Bool(ProgramProperties.GetPropValFromList(_listProgProps,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditIsMerchantNumberByProv)))
			{
				checkMerchantNumByProv.Checked=true;
			}
			if(PrefC.HasClinicsEnabled) {
				textMerchantNumberClinic.Visible=!checkMerchantNumByProv.Checked;
				labelMerchantNumberClinic.Visible=!checkMerchantNumByProv.Checked;
				textMerchantNumberClinic.Text=PIn.String(ProgramProperties.GetPropValFromList(_listProgProps,
					ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,comboClinics.SelectedClinicNum));
			}
			else {
				textMerchantNumberPractice.Visible=!checkMerchantNumByProv.Checked;
				labelMerchantNumberPractice.Visible=!checkMerchantNumByProv.Checked;
				textMerchantNumberPractice.Text=PIn.String(ProgramProperties.GetPropValFromList(_listProgProps,
					ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber));
			}
		}

		///<summary>Returns the Merchant Number program property from the in memory list of program properties for ClinicNum 0.</summary>
		private string GetHQMerchantNumber() {
			return PIn.String(ProgramProperties.GetPropValFromList(_listProgProps,ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,0));
		}

		///<summary>Adds any missing program property for the passed in clinic.</summary>
		private void AddNeededProgramProperties(long clinicNum) {
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=""//default value to empty string. 
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchEnabled)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchEnabled,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchEnabled && x.ClinicNum==0)?.PropertyValue??"0",
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps.FirstOrDefault(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType && x.ClinicNum==0)?.PropertyValue??"0",
				});
			}
			if(!_listProgProps.Any(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchDays)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=clinicNum,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchDays,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=_listProgProps
						.FirstOrDefault(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchDays && x.ClinicNum==0)?.PropertyValue??DEFAULT_DAYS.ToString(),
				});
			}
			//These program properties are not clinic specific
			if(!_listProgProps.Any(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditOAuthToken)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=0,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditOAuthToken,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue="",
				});
			}
			if(!_listProgProps.Any(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditPatField)) {
				PatFieldDef patFieldDef=PatFieldDefs.GetDeepCopy().FindAll(x => x.FieldType==PatFieldType.CareCreditStatus).FirstOrDefault();
				long progPropValue=patFieldDef==null ? 0 : patFieldDef.PatFieldDefNum;
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=0,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditPatField,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue=POut.String(progPropValue.ToString()),
				});
			}
			if(!_listProgProps.Any(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditIsMerchantNumberByProv)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=0,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditIsMerchantNumberByProv,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue="0",//Default to false
				});
			}
			if(!_listProgProps.Any(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditDoDisableAdvertising)) {
				_listProgProps.Add(new ProgramProperty() {
					ClinicNum=0,
					PropertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditDoDisableAdvertising,
					ProgramNum=_progCur.ProgramNum,
					PropertyValue="0",
				});
			}
		}

		///<summary>Sets the Payment Type combobox to the first "CareCredit" payment type in the combo box. A new "CareCredit" payment type will be created and added to the combo box if there were none to select when this method was invoked. A "CareCredit" payment type is guaranteed to be selected when this method returns.</summary>
		private void SetDefaultCareCreditPaymentType() {
			Def defCareCredit=comboPaymentType.Items.GetAll<Def>().FirstOrDefault(x => x.ItemName.ToLower()=="carecredit");
			if(defCareCredit!=null) {
				comboPaymentType.SetSelectedDefNum(defCareCredit.DefNum);
				return;
			}
			List<Def> listDefPaymentTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes);//Include hidden defs on purpose.
			defCareCredit=listDefPaymentTypes.FirstOrDefault(x => x.ItemName.ToLower()=="carecredit");
			if(defCareCredit==null) {
				//Create a new CareCredit Def
				defCareCredit=new Def() {
					Category=DefCat.PaymentTypes,
					ItemOrder=listDefPaymentTypes.Max(x => x.ItemOrder)+1,
					ItemName="CareCredit",
					IsHidden=false,
				};
				Defs.Insert(defCareCredit);
				DataValid.SetInvalid(InvalidType.Defs);
			}
			else {
				if(defCareCredit.IsHidden) {
					defCareCredit.IsHidden=false;
					Defs.Update(defCareCredit);
					DataValid.SetInvalid(InvalidType.Defs);
				}
			}
			comboPaymentType.Items.AddDefs(new List<Def>() { defCareCredit });
			comboPaymentType.SetSelectedDefNum(defCareCredit.DefNum);
		}

		///<summary>Updates each clinic's property values with the values on the form. Validation should happen before calling this method.</summary>
		private void UpdatePropertiesInMemory(long clinicNum) {
			string payTypeSelected="0";
			if(comboPaymentType.SelectedIndex>-1) {
				payTypeSelected=comboPaymentType.GetSelectedDefNum().ToString();
			}
			int days=DEFAULT_DAYS;
			if(comboBoxDaysOut.SelectedIndex>-1) { 
				int.TryParse(comboBoxDaysOut.GetSelectedKey<int>(x => x).ToString(),out days);
			}
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum
				&& x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchEnabled)
				.ForEach(x => x.PropertyValue=POut.Bool(checkQSBatch.Checked));
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType)
				.ForEach(x => x.PropertyValue=POut.String(payTypeSelected));
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchDays)
				.ForEach(x => x.PropertyValue=POut.String(days.ToString()));
			string merchantNumber=POut.String(textMerchantNumberPractice.Text);
			if(PrefC.HasClinicsEnabled) {
				merchantNumber=POut.String(textMerchantNumberClinic.Text);
			}
			_listProgProps.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber)
				.ForEach(x => x.PropertyValue=merchantNumber);
			//Update program properties that are not clinic specific
			if(comboPatFieldDef.SelectedIndex>-1) {
				_listProgProps.FindAll(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditPatField)
				.ForEach(x => x.PropertyValue=POut.String(comboPatFieldDef.GetSelected<PatFieldDef>().PatFieldDefNum.ToString()));
			}
			_listProgProps.FindAll(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditIsMerchantNumberByProv)
				.ForEach(x => x.PropertyValue=POut.Bool(checkMerchantNumByProv.Checked));
			_listProgProps.FindAll(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditDoDisableAdvertising)
			.ForEach(x => x.PropertyValue=POut.Bool(checkHideAdvertising.Checked));
		}

		private void comboClinics_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinics.SelectedClinicNum==_previouslySelectedClinicNum) {//didn't change the selected clinic
				return;
			}
			//if CareCredit is enabled and the username and key are set for the current clinic,
			//make the user select a payment type before switching clinics
			if(checkEnabled.Checked && !IsValid(_previouslySelectedClinicNum)) {
				comboClinics.SelectedClinicNum=_previouslySelectedClinicNum;
				return;
			}
			UpdatePropertiesInMemory(_previouslySelectedClinicNum);
			//Update previously selected clinic num since we have updated the values for the clinic.
			_previouslySelectedClinicNum=comboClinics.SelectedClinicNum;
			FillFields();
		}

		private void checkQSBatch_CheckedChanged(object sender,EventArgs e) {
			groupQSBatch.Visible=checkQSBatch.Checked;
		}

		private void checkMerchantNumByProv_CheckedChanged(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				textMerchantNumberClinic.Visible=!checkMerchantNumByProv.Checked;
				labelMerchantNumberClinic.Visible=!checkMerchantNumByProv.Checked;
			}
			else {
				textMerchantNumberPractice.Visible=!checkMerchantNumByProv.Checked;
				labelMerchantNumberPractice.Visible=!checkMerchantNumByProv.Checked;
			}
		}

		private bool IsValid(long clinicNum) {
			if(comboPaymentType.SelectedIndex<0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"No default payment type set. Set a default automatically?","CareCredit")) {
					return false;//They want to set the default payment type.
				}
				//Set a default CareCredit payment type. 
				SetDefaultCareCreditPaymentType();
			}
			if(checkQSBatch.Checked && comboBoxDaysOut.SelectedIndex<0) {
				MsgBox.Show(this,"Number of days out Batch Quickscreen Setting required.");
				return false;
			}
			if(comboPatFieldDef.SelectedIndex<0) {
				//Program is enabled and a patfielddef is not selected or merchant numbers is not by providers and the.
				MsgBox.Show(this,"Approval Status Patient Field is required.");
				return false;
			}
			if(!checkMerchantNumByProv.Checked && !IsMerchantNumberValid(clinicNum)) {
				//User has entered a merchant number but the value does not meet the requirements for a valid Merchant Number
				MsgBox.Show(this,"Merchant Number must be a valid 16-digit number.");
				return false;
			}
			return true;
		}

		private bool IsMerchantNumberValid(long clinicNum) {
			string merchantNumber=textMerchantNumberPractice.Text;
			if(PrefC.HasClinicsEnabled) {
				merchantNumber=textMerchantNumberClinic.Text;
			}
			if(clinicNum!=0 && CareCredit.IsMerchantNumValid(GetHQMerchantNumber()) && string.IsNullOrEmpty(merchantNumber)) {
				//Not HQ Merchant Number. OK if the non HQ ClinicNum's Merchant Number is empty. No override for the clinic
				return true;
			}
			return CareCredit.IsMerchantNumValid(merchantNumber);
		}

		private void butPromotions_Click(object sender,EventArgs e) {
			if(!checkEnabled.Enabled) {
				MsgBox.Show(this,"CareCredit must be enabled.");
				return;
			}
			if(checkMerchantNumByProv.Checked) {
				if(comboClinics.GetSelectedClinic()==null) {
					MsgBox.Show(this,"Please select a clinic.");
					return;
				}
				List<Provider> listProvOverrides=Providers.GetAll();
				using FormProviderPick FormProviderPick=new FormProviderPick(listProvOverrides);
				FormProviderPick.SelectedProvNum=Security.CurUser.ProvNum;
				if(FormProviderPick.ShowDialog()!=DialogResult.OK) {
					return;
				}
				if(FormProviderPick.SelectedProvNum==0) {
					MsgBox.Show(this,"No provider selected. Choose a provider and try again.");
					return;
				}
				CareCreditL.LaunchAdminPage(FormProviderPick.SelectedProvNum,comboClinics.GetSelectedClinic().ClinicNum,isProvOverride:true);
			}
			else {
				string merchantNumber=textMerchantNumberPractice.Text;
				if(PrefC.HasClinicsEnabled) {
					merchantNumber=textMerchantNumberClinic.Text;
				}
				CareCreditL.LaunchAdminPage(merchantNumber);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.CareCredit,out string err)) {
				MessageBox.Show(err);
				return;
			}
			if(checkEnabled.Checked && !IsValid(comboClinics.SelectedClinicNum)) {
				return;
			}
			//Set the program property value before saving.
			UpdatePropertiesInMemory(comboClinics.SelectedClinicNum);
			#region Save
			//Only update the program if the IsEnabled flag has changed
			if(_progCur.Enabled!=checkEnabled.Checked) {
				_progCur.Enabled=checkEnabled.Checked;
				Programs.Update(_progCur);
			}
			ProgramProperties.Sync(_listProgProps,_progCur.ProgramNum);
			#endregion
			DataValid.SetInvalid(InvalidType.Programs);
			bool hasCareCreditQSBatchEnabled=_listProgProps.Any(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditQSBatchEnabled
				&& x.PropertyValue=="1");
			if(checkEnabled.Checked && hasCareCreditQSBatchEnabled && !_hasCareCreditPatFieldDefNum) {
				DoShowApptViewWindow=MsgBox.Show(this,MsgBoxButtons.YesNo,$"Add the {PatFieldType.CareCreditStatus.GetDescription()} to appointment views?");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
