using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bridges;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCareCredit:FormODBase {
		#region Private Variables
		private Patient _patient;
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
			if(!CareCredit.IsMerchantNumberByProv) {
				comboProviders.Visible=false;
				labelProviders.Visible=false;
			}
			comboProviders.Items.Clear();
			comboProviders.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinics.SelectedClinicNum));
			comboProviders.SetSelected(0);
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
			CareCreditL.LaunchQuickScreenIndividualPage(_patient,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,
				estimatedFeeAmt:inputAmount);
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