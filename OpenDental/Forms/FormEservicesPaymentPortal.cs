using System;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebApps;

namespace OpenDental {
	public partial class FormEServicesPaymentPortal:FormODBase {

		public FormEServicesPaymentPortal() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textPatientFacingPaymentUrl.Text=WebAppUtil.GetWebAppUrl(eServiceCode.PaymentPortalUI,Clinics.ClinicNum);
			if(!PrefC.HasClinicsEnabled) {
				comboClinicPicker.Visible=false;
			}
		}

		private void comboClinicPicker_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboClinicPicker.SelectedClinicNum==-1) {
				return;
			}
			textPatientFacingPaymentUrl.Text=WebAppUtil.GetWebAppUrl(eServiceCode.PaymentPortalUI,comboClinicPicker.SelectedClinicNum);
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}