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
	public partial class FormCanadaOutstandingTransactions:FormODBase {

		private List<Carrier> _listCarriers=new List<Carrier>();
		private List<Provider> _listProviders;

		public FormCanadaOutstandingTransactions() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCanadaOutstandingTransactions_Load(object sender,EventArgs e) {
			_listCarriers=Carriers.GetWhere(x => 
				(x.CanadianSupportedTypes & CanSupTransTypes.RequestForOutstandingTrans_04)==CanSupTransTypes.RequestForOutstandingTrans_04);
			for(int i = 0;i<_listCarriers.Count;++i) {
				listCarriers.Items.Add(_listCarriers[i].CarrierName);
			}
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				if(!_listProviders[i].IsCDAnet || _listProviders[i].NationalProvID=="" || _listProviders[i].CanadianOfficeNum=="") {
					continue;
				}
				if(!listOfficeNumbers.Items.Contains(_listProviders[i].CanadianOfficeNum)) {
					listOfficeNumbers.Items.Add(_listProviders[i].CanadianOfficeNum);
				}
			}
			if(listOfficeNumbers.Items.Count<1) {
				MsgBox.Show(this,"At least one unhidden provider must have a CDA Number and an Office Number set before running a Request for Outstanding Transactions.");
				Close();
			}
		}

		private void radioVersion2_Click(object sender,EventArgs e) {
			radioVersion2.Checked=true;
			radioVersion4Itrans.Checked=false;
			radioVersion4ToCarrier.Checked=false;
			groupCarrier.Enabled=false;
		}

		private void radioVersion4Itrans_Click(object sender,EventArgs e) {
			radioVersion2.Checked=false;
			radioVersion4Itrans.Checked=true;
			radioVersion4ToCarrier.Checked=false;
			groupCarrier.Enabled=false;
		}

		private void radioVersion4ToCarrier_Click(object sender,EventArgs e) {
			radioVersion2.Checked=false;
			radioVersion4Itrans.Checked=false;
			radioVersion4ToCarrier.Checked=true;
			groupCarrier.Enabled=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(radioVersion4ToCarrier.Checked) {
				if(listCarriers.SelectedIndex<0) {
					MsgBox.Show(this,"You must first select a carrier to use.");
					return;
				}
			}
			if(listOfficeNumbers.SelectedIndex<0) {
				MsgBox.Show(this,"You must first select an Office Number to use.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			Provider provider=null;
			for(int i=0;i<_listProviders.Count;i++) {
				if(_listProviders[i].CanadianOfficeNum==listOfficeNumbers.SelectedItem.ToString() 
					&& _listProviders[i].NationalProvID!="" && _listProviders[i].IsCDAnet) {
					provider=_listProviders[i];
					break;
				}
			}
			string formatVersion="04";
			Carrier carrier=null;
			if(radioVersion2.Checked) {
				formatVersion="02";
			}
			else if(radioVersion4ToCarrier.Checked) {
				carrier=_listCarriers[listCarriers.SelectedIndex];					
			}
			try {
				CanadianOutput.GetOutstandingForDefault(provider,formatVersion,carrier,FormClaimPrint.PrintCdaClaimForm,FormCCDPrint.PrintCCD);
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Done.");
			}
			catch(ApplicationException applicationException) {
				Cursor=Cursors.Default;
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g(this,"Request failed:")+"\r\n"+applicationException.Message);
				msgBoxCopyPaste.ShowDialog();
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g(this,"Request failed:")+"\r\n"+ex.ToString());
				msgBoxCopyPaste.ShowDialog();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}