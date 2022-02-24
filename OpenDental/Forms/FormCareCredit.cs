using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Bridges;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCareCredit:FormODBase {
		#region Private Variables
		private Patient _patCur; 
		#endregion

		private bool IsProviderValid {
			get {
				if(CareCredit.IsMerchantNumberByProv && comboProviders.GetSelectedProvNum()==0) {
					return false;// no provider selected
				}
				return true;
			}
		}

		public FormCareCredit(long patNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=Patients.GetPat(patNum);
		}

		private void FormCareCredit_Load(object sender,EventArgs e) {
			Text=$"CareCredit - {_patCur.GetNameFL()}";
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
			if(!IsProviderValid) {
				MsgBox.Show(this,"No provider selected");
				return false;
			}
			return true;
		}

		private InputBox GetAmountInput(string labelText="Please enter an amount: ") {
			InputBox inputBox=new InputBox(new List<InputBoxParam>() { new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,labelText))}
				,new Func<string, bool>((text) => {
					if(text=="" || !double.TryParse(text,out double res) || res<1) {
						MsgBox.Show(this,"Please enter a value greater than 1.");
						return false;//Should stop user from continuing to payment window.
					}
					return true;//Allow user to the payment window.
				})
			);
			return inputBox;
		}

		private void butApply_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchCreditApplicationPage(_patCur,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,0,1);
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butLookup_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			CareCreditL.LaunchLookupPage(_patCur,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum);
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
			using InputBox inputBox=GetAmountInput("Please enter an estimated fee amount: ");
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			CareCreditL.LaunchQuickScreenIndividualPage(_patCur,comboProviders.GetSelectedProvNum(),comboClinics.SelectedClinicNum,
				estimatedFeeAmt:PIn.Double(inputBox.textResult.Text));
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
			using FormCareCreditTransactions FormCareCreditTrans=new FormCareCreditTransactions(_patCur);
			FormCareCreditTrans.ShowDialog();
			DialogResult=DialogResult.None;//Don't close the form yet.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}