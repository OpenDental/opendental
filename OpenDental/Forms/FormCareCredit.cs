using Bridges;
using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCareCredit:FormODBase {
		#region Private Variables
		private Patient _patient;
		private List<ProgramProperty> _listProgramProperties;
		#endregion

		public FormCareCredit(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=Patients.GetPat(patNum);
		}

		private void FormCareCredit_Load(object sender,EventArgs e) {
			Text=$"CareCredit - {_patient.GetNameFL()}";
			comboClinics.Visible=PrefC.HasClinicsEnabled;
			comboClinics.Enabled=CareCredit.IsMerchantNumberByProv;
			labelMerchantClosedDescription.Visible=false;
			if(!CareCredit.IsMerchantNumberByProv) {
				comboProviders.Visible=false;
				labelProviders.Visible=false;
			}
			comboProviders.Items.Clear();
			comboProviders.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinics.SelectedClinicNum));
			comboProviders.SetSelected(0);
			_listProgramProperties=ProgramProperties.GetForProgram(Programs.GetProgramNum(ProgramName.CareCredit));
			string careCreditMerchantNum=PIn.String(ProgramProperties.GetPropValFromList(_listProgramProperties,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,comboClinics.SelectedClinicNum));
			if(CareCredit.IsMerchantNumberByProv) {
				careCreditMerchantNum=ProviderClinics.GetOneOrDefault(comboProviders.GetSelected<Provider>().ProvNum,comboClinics.SelectedClinicNum).CareCreditMerchantId??"";
			}
			DisableControlsForMerchantClosed(careCreditMerchantNum);
		}

		private bool IsValid() {
			if(comboClinics.SelectedClinicNum<0) {
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
			InputBoxParam inputBoxParam=new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,"Please enter an estimated fee amount: "));
			listInputBoxParams.Add(inputBoxParam);
			Func<string,bool> funcOkClick = new Func<string,bool>((text) => {
				if(text=="" || !double.TryParse(text,out double res) || res<1) {
					MsgBox.Show(this,"Please enter a value greater than 1.");
					return false;//Should stop user from continuing to payment window.
				}
				return true;//Allow user to the payment window.
			});
			using InputBox inputBox=new InputBox(listInputBoxParams,funcOkClick);
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return -1; // use as cancel/close click signifier
			}
			return PIn.Double(inputBox.textResult.Text);
		}

		///<summary>Update pullback status and PatFieldDef one more time. Remove once CareCredit fixes non-null QuickScreen details object.</summary>
		private void UpdateCareCreditQuickScreen(CareCreditWebResponse careCreditWebResponse) {
			if(careCreditWebResponse==null) {
				return;
			}
			if(ListTools.In(careCreditWebResponse.ProcessingStatus,CareCreditWebStatus.AccountFound,CareCreditWebStatus.PreApproved)) {//Don't need to check again.
				return;
			}
			CareCredit.UpdateWebResponse(careCreditWebResponse);
			if(careCreditWebResponse.ProcessingStatus!=CareCreditWebStatus.Pending) {
				CareCredit.UpdateCareCreditPatField(careCreditWebResponse.ProcessingStatus.GetDescription(),careCreditWebResponse.PatNum,isBatch:false);
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
			butQuickScreen.Enabled=false;
			labelMerchantClosedDescription.Visible=true;
		}

		private void comboClinics_SelectionChangeCommitted(object sender,EventArgs e) {
			string careCreditMerchantNum=PIn.String(ProgramProperties.GetPropValFromList(_listProgramProperties,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,comboClinics.SelectedClinicNum));
			if(CareCredit.IsMerchantNumberByProv) {
				careCreditMerchantNum=ProviderClinics.GetOneOrDefault(comboProviders.GetSelected<Provider>().ProvNum,comboClinics.SelectedClinicNum).CareCreditMerchantId??"";
			}
			DisableControlsForMerchantClosed(careCreditMerchantNum);
		}

		private void comboProviders_SelectionChangedCommitted(object sender,EventArgs e) {
			string careCreditMerchantNum=ProviderClinics.GetOneOrDefault(comboProviders.GetSelected<Provider>().ProvNum,comboClinics.SelectedClinicNum).CareCreditMerchantId??"";
			DisableControlsForMerchantClosed(careCreditMerchantNum);
		}

		private void butApply_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchCreditApplicationPage(_patient,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,0,1);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butLookup_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchLookupPage(_patient,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butPromotions_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchAdminPage(comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,CareCredit.IsMerchantNumberByProv);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butQuickScreen_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			double inputAmount=GetAmountInput();
			if(inputAmount < 0) {// input box was cancelled/closed
				return;
			}
			//NOTE: uncomment and delete code after launching page when CareCredit fixes non-null QuickScreen details object.
			//CareCreditL.LaunchQuickScreenIndividualPage(_patient,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,
			//	estimatedFeeAmt:inputAmount);

			string urlQuickScreenPage=CareCreditL.GetQSPageUrl(_patient,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,estimatedFeeAmt:inputAmount);
			if(string.IsNullOrEmpty(urlQuickScreenPage)) {
				//Error occurred when trying to get url. Message already displayed to the user. Return
				return;
			}
			using FormCareCreditWeb formCareCreditWeb=new FormCareCreditWeb(urlQuickScreenPage,_patient);
			formCareCreditWeb.ShowDialog();
			if(string.IsNullOrEmpty(formCareCreditWeb.SessionId)) {
				MsgBox.Show("Error retrieving CareCredit web page.");
				return;
			}
			CareCreditWebResponse careCreditWebResponse=CareCreditWebResponses.GetBySessionId(formCareCreditWeb.SessionId);
			if(careCreditWebResponse==null) {
				//This shouldn't happen
				MsgBox.Show("CareCredit web response no longer exist.");
				return;
			}
			UpdateCareCreditQuickScreen(careCreditWebResponse);
			//NOTE: do not delete this, this is part of old code.
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butReport_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchReportsPage(comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butTransactions_Click(object sender,EventArgs e) {
			using FormCareCreditTransactions formCareCreditTransactions=new FormCareCreditTransactions(_patient);
			formCareCreditTransactions.ShowDialog();
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}