using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	public partial class FormCanadaPaymentReconciliation:FormODBase {

		private List<Carrier> _listCarriers=new List<Carrier>();
		private List<Provider> _listProviders;

		public FormCanadaPaymentReconciliation() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCanadaPaymentReconciliation_Load(object sender,EventArgs e) {
			_listCarriers=Carriers.GetWhere(x => x.CDAnetVersion!="02" &&//This transaction does not exist in version 02.
				(x.CanadianSupportedTypes & CanSupTransTypes.RequestForPaymentReconciliation_06)==CanSupTransTypes.RequestForPaymentReconciliation_06);
			for(int i = 0;i<_listCarriers.Count;++i) {
				listCarriers.Items.Add(_listCarriers[i].CarrierName);
			}
			long defaultProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				if(_listProviders[i].IsCDAnet) {
					listBillingProvider.Items.Add(_listProviders[i].Abbr);
					listTreatingProvider.Items.Add(_listProviders[i].Abbr);
					if(_listProviders[i].ProvNum==defaultProvNum) {
						listBillingProvider.SelectedIndex=i;
						textBillingOfficeNumber.Text=_listProviders[i].CanadianOfficeNum;
						listTreatingProvider.SelectedIndex=i;
						textTreatingOfficeNumber.Text=_listProviders[i].CanadianOfficeNum;
					}
				}
			}
			textDateReconciliation.Text=DateTime.Today.ToShortDateString();
		}

		private void listBillingProvider_Click(object sender,EventArgs e) {
			textBillingOfficeNumber.Text=_listProviders[listBillingProvider.SelectedIndex].CanadianOfficeNum;
		}

		private void listTreatingProvider_Click(object sender,EventArgs e) {
			textTreatingOfficeNumber.Text=_listProviders[listTreatingProvider.SelectedIndex].CanadianOfficeNum;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listCarriers.SelectedIndex<0) {
				MsgBox.Show(this,"You must first choose a carrier.");
				return;
			}
			if(listBillingProvider.SelectedIndex<0) {
				MsgBox.Show(this,"You must first choose a billing provider.");
				return;
			}
			if(listTreatingProvider.SelectedIndex<0) {
				MsgBox.Show(this,"You must first choose a treating provider.");
				return;
			}
			DateTime reconciliationDate;
			try {
				reconciliationDate=DateTime.Parse(textDateReconciliation.Text).Date;
			}
			catch {
				MsgBox.Show(this,"Reconciliation date invalid.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				Carrier carrier=_listCarriers[listCarriers.SelectedIndex];
				Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum); 
				CanadianOutput.GetPaymentReconciliations(clearinghouseClin,carrier,_listProviders[listTreatingProvider.SelectedIndex],
					_listProviders[listBillingProvider.SelectedIndex],reconciliationDate,Clinics.ClinicNum,false,FormCCDPrint.PrintCCD);
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Done.");
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Request failed: ")+ex.Message);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}