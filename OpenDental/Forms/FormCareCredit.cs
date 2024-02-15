using Bridges;
using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCareCredit:FormODBase {
		public Patient PatientCur;
		private List<ProgramProperty> _listProgramProperties;

		public FormCareCredit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCareCredit_Load(object sender,EventArgs e) {
			Text=$"CareCredit - {PatientCur.GetNameFL()}";
			comboClinics.Visible=PrefC.HasClinicsEnabled;
			comboClinics.Enabled=CareCredit.IsMerchantNumberByProv;
			labelMerchantClosedDescription.Visible=false;
			if(!CareCredit.IsMerchantNumberByProv) {
				comboProviders.Visible=false;
				labelProviders.Visible=false;
			}
			comboProviders.Items.Clear();
			comboProviders.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinics.ClinicNumSelected));
			comboProviders.SetSelected(0);
			_listProgramProperties=ProgramProperties.GetForProgram(Programs.GetProgramNum(ProgramName.CareCredit));
			string careCreditMerchantNum=PIn.String(ProgramProperties.GetPropValFromList(_listProgramProperties,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,comboClinics.ClinicNumSelected));
			if(CareCredit.IsMerchantNumberByProv) {
				careCreditMerchantNum=ProviderClinics.GetOneOrDefault(comboProviders.GetSelected<Provider>().ProvNum,comboClinics.ClinicNumSelected).CareCreditMerchantId??"";
			}
			DisableControlsForMerchantClosed(careCreditMerchantNum);
		}

		private bool IsValid() {
			if(comboClinics.ClinicNumSelected<0) {
				MsgBox.Show(this,"No clinic selected");
				return false;
			}
			if(CareCredit.IsMerchantNumberByProv && comboProviders.GetSelectedProvNum()==0) {
				MsgBox.Show(this,"No provider selected");
				return false;// no provider selected
			}
			return true;
		}

		private double GetAmountInput() {
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			InputBoxParam inputBoxParam=new InputBoxParam();
			inputBoxParam.InputBoxType_=InputBoxType.ValidDouble;
			inputBoxParam.LabelText=Lan.g(this,"Please enter an estimated fee amount: ");
			listInputBoxParams.Add(inputBoxParam);
			Func<string,bool> funcOkClick = new Func<string,bool>((text) => {
				if(text=="" || !double.TryParse(text,out double res) || res<1) {
					MsgBox.Show(this,"Please enter a value greater than 1.");
					return false;//Should stop user from continuing to payment window.
				}
				return true;//Allow user to the payment window.
			});
			InputBox inputBox=new InputBox(listInputBoxParams);
			inputBox.FuncOkClick=funcOkClick;
			inputBox.ShowDialog();
			if(inputBox.IsDialogCancel) {
				return -1; // use as cancel/close click signifier
			}
			return PIn.Double(inputBox.StringResult);
		}

		///<summary>Update pullback status and PatFieldDef one more time. Remove once CareCredit fixes non-null QuickScreen details object.</summary>
		private void UpdateCareCreditQuickScreen(CareCreditWebResponse careCreditWebResponse) {
			if(careCreditWebResponse==null) {
				return;
			}
			if(careCreditWebResponse.ProcessingStatus.In(CareCreditWebStatus.AccountFound,CareCreditWebStatus.PreApproved)) {//Don't need to check again.
				return;
			}
			CareCredit.UpdateWebResponse(careCreditWebResponse);
			if(careCreditWebResponse.ProcessingStatus!=CareCreditWebStatus.Pending) {
				CareCreditWebResponses.UpdateCareCreditPatField(careCreditWebResponse.ProcessingStatus.GetDescription(),careCreditWebResponse.PatNum,isBatch:false);
			}
		}

		private void DisableControlsForMerchantClosed(string careCreditMerchantNum) {
			if(!CareCredit.IsMerchantNumClosed(careCreditMerchantNum)) {
				labelMerchantClosedDescription.Visible=false;
				return;
			}
			butApply.Enabled=false;
			butReport.Enabled=false;
			butLookup.Enabled=false;
			butPromotions.Enabled=false;
			labelMerchantClosedDescription.Visible=true;
		}

		private void comboClinics_SelectionChangeCommitted(object sender,EventArgs e) {
			string careCreditMerchantNum=PIn.String(ProgramProperties.GetPropValFromList(_listProgramProperties,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,comboClinics.ClinicNumSelected));
			if(CareCredit.IsMerchantNumberByProv) {
				careCreditMerchantNum=ProviderClinics.GetOneOrDefault(comboProviders.GetSelected<Provider>().ProvNum,comboClinics.ClinicNumSelected).CareCreditMerchantId??"";
			}
			DisableControlsForMerchantClosed(careCreditMerchantNum);
		}

		private void comboProviders_SelectionChangedCommitted(object sender,EventArgs e) {
			string careCreditMerchantNum=ProviderClinics.GetOneOrDefault(comboProviders.GetSelected<Provider>().ProvNum,comboClinics.ClinicNumSelected).CareCreditMerchantId??"";
			DisableControlsForMerchantClosed(careCreditMerchantNum);
		}

		private void butApply_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			PatFieldDef patFieldDef=PatFieldDefs.GetPatFieldCareCredit();
			string careCreditFieldName="";
			if(patFieldDef!=null) {
				careCreditFieldName=patFieldDef.FieldName;
			}
			string careCreditStatus="";
			List<PatField> listPatFields=PatFields.GetPatientData(PatientCur.PatNum);
			for(int i=0;i<listPatFields.Count;i++) {
				if(listPatFields[i].FieldName!=careCreditFieldName) {
					continue;
				}
				if(listPatFields[i].FieldName==careCreditFieldName) {
					careCreditStatus=listPatFields[i].FieldValue;
					break;
				}
			}
			if(careCreditStatus.ToLower().In(CareCreditWebStatus.PreApproved.GetDescription().ToLower())) {
				CareCreditL.LaunchQuickScreenIndividualPage(PatientCur,comboProviders.GetSelectedProvNum(),comboClinics.ClinicNumSelected,1);
			}
			else {
				CareCreditL.LaunchCreditApplicationPage(PatientCur,comboProviders.GetSelectedProvNum(),comboClinics.ClinicNumSelected,0,1);
			}
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butLookup_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchLookupPage(PatientCur,comboProviders.GetSelectedProvNum(),comboClinics.ClinicNumSelected);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butPromotions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchAdminPage(comboProviders.GetSelectedProvNum(),comboClinics.ClinicNumSelected,CareCredit.IsMerchantNumberByProv);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butReport_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchReportsPage(comboProviders.GetSelectedProvNum(),comboClinics.ClinicNumSelected);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butTransactions_Click(object sender,EventArgs e) {
			using FormCareCreditTransactions formCareCreditTransactions=new FormCareCreditTransactions(PatientCur);
			formCareCreditTransactions.ShowDialog();
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

	}
}