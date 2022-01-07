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
	public partial class FormCanadaSummaryReconciliation:FormODBase {

		private List<Carrier> _listCarriers=new List<Carrier>();
		private List<Provider> _listProviders;
		private List<CanadianNetwork> _listCanadianNetworks;

		public FormCanadaSummaryReconciliation() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCanadaPaymentReconciliation_Load(object sender,EventArgs e) {
			_listCanadianNetworks=CanadianNetworks.GetDeepCopy();
			for(int i=0;i<_listCanadianNetworks.Count;i++) {
				listNetworks.Items.Add(_listCanadianNetworks[i].Abbrev+" - "+_listCanadianNetworks[i].Descript);
			}
			_listCarriers=Carriers.GetWhere(x => x.CDAnetVersion!="02" &&//This transaction does not exist in version 02.
				(x.CanadianSupportedTypes & CanSupTransTypes.RequestForSummaryReconciliation_05)==CanSupTransTypes.RequestForSummaryReconciliation_05);
			for(int i = 0;i<_listCarriers.Count;++i) {
				listCarriers.Items.Add(_listCarriers[i].CarrierName);
			}
			long defaultProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				listTreatingProvider.Items.Add(_listProviders[i].Abbr);
				if(_listProviders[i].ProvNum==defaultProvNum) {
					listTreatingProvider.SelectedIndex=i;
				}
			}
			textDateReconciliation.Text=DateTime.Today.ToShortDateString();
		}

		private void checkGetForAllCarriers_Click(object sender,EventArgs e) {
			groupCarrierOrNetwork.Enabled=!checkGetForAllCarriers.Checked;
		}

		private void listCarriers_Click(object sender,EventArgs e) {
			listNetworks.SelectedIndex=-1;
		}

		private void listNetwork_Click(object sender,EventArgs e) {
			listCarriers.SelectedIndex=-1;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!checkGetForAllCarriers.Checked) {
				if(listCarriers.SelectedIndex<0 && listNetworks.SelectedIndex<0) {
					MsgBox.Show(this,"You must first choose one carrier or one network.");
					return;
				}
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
				if(checkGetForAllCarriers.Checked) {
					Carrier carrier=new Carrier();
					carrier.CDAnetVersion="04";
					carrier.ElectID="999999";//The whole ITRANS network.
					carrier.CanadianEncryptionMethod=1;//No encryption.
					Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
					Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
					CanadianOutput.GetSummaryReconciliation(clearinghouseClin,carrier,null,
						_listProviders[listTreatingProvider.SelectedIndex],reconciliationDate,false,FormCCDPrint.PrintCCD);
				}
				else {
					if(listCarriers.SelectedIndex>=0) {
						Carrier carrier=_listCarriers[listCarriers.SelectedIndex];
						Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
						Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
						CanadianOutput.GetSummaryReconciliation(clearinghouseClin,carrier,null,
							_listProviders[listTreatingProvider.SelectedIndex],reconciliationDate,false,FormCCDPrint.PrintCCD);
					}
					else {
						Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(null);
						Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
						CanadianOutput.GetSummaryReconciliation(clearinghouseClin,null,_listCanadianNetworks[listNetworks.SelectedIndex],
							_listProviders[listTreatingProvider.SelectedIndex],reconciliationDate,false,FormCCDPrint.PrintCCD);
					}
				}
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