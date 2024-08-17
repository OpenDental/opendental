using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProviderMerge:FormODBase {
		private List<Provider> _listProvidersActive=new List<Provider>();

		public FormProviderMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listProvidersActive=Providers.GetWhere(x => x.ProvStatus != ProviderStatus.Deleted,true);
		}

		private void butChangeProvInto_Click(object sender,EventArgs e) {
			using FormProviderPick FormProviderPick=new FormProviderPick(_listProvidersActive);
			FormProviderPick.ShowDialog();
			if(FormProviderPick.DialogResult!=DialogResult.OK){
				return;
			}
			Provider providerSelected=Providers.GetProv(FormProviderPick.ProvNumSelected);
			textAbbrInto.Text=providerSelected.Abbr;
			textProvNumInto.Text=POut.Long(providerSelected.ProvNum);
			textNpiInto.Text=providerSelected.NationalProvID;
			textFullNameInto.Text=providerSelected.FName+" "+providerSelected.LName;
			CheckUIState();
		}

		private void butChangeProvFrom_Click(object sender,EventArgs e) {
			using FormProviderPick FormProviderPick=new FormProviderPick(checkDeletedProvs.Checked ? Providers.GetDeepCopy() : _listProvidersActive);
			FormProviderPick.ShowDialog();
			if(FormProviderPick.DialogResult!=DialogResult.OK){
				return;
			}
			Provider providerSelected=Providers.GetProv(FormProviderPick.ProvNumSelected);
			textAbbrFrom.Text=providerSelected.Abbr;
			textProvNumFrom.Text=POut.Long(providerSelected.ProvNum);
			textNpiFrom.Text=providerSelected.NationalProvID;
			textFullNameFrom.Text=providerSelected.FName+" "+providerSelected.LName;
			CheckUIState();
		}

		private void CheckUIState() {
			butMerge.Enabled=(textProvNumInto.Text!="" && textProvNumFrom.Text!="");
		}

		private void butMerge_Click(object sender,EventArgs e) {
			string differentFields="";
			if(textProvNumFrom.Text==textProvNumInto.Text) { 
				//do not attempt a merge if the same provider was selected twice, or if one of the fields is blank.
				MsgBox.Show(this,"You must select two different providers to merge.");
				return;
			}
			if(textNpiFrom.Text!=textNpiInto.Text) {
				differentFields+="\r\nNPI";
			}
			if(textFullNameFrom.Text!=textFullNameInto.Text) {
				differentFields+="\r\nFull Name";
			}
			long numPats=Providers.CountPats(PIn.Long(textProvNumFrom.Text));
			long numClaims=Providers.CountClaims(PIn.Long(textProvNumFrom.Text));
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure?  The results are permanent and cannot be undone.")) {
				return;
			}
			string msgText="";
			if(differentFields!="") {
				msgText=Lan.g(this,"The following provider fields do not match")+": "+differentFields+"\r\n";
			}
			msgText+=Lan.g(this,"This change is irreversible")+".  "+Lan.g(this,"This provider is the primary or secondary provider for")+" "+numPats+" "+Lan.g(this,"active patients")
				+", "+Lan.g(this,"and the billing or treating provider for")+" "+numClaims+" "+Lan.g(this,"claims")+".  "
				+Lan.g(this,"Continue anyways?");
			if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)!=DialogResult.OK)	{
				return;
			}
			long rowsChanged=Providers.Merge(PIn.Long(textProvNumFrom.Text),PIn.Long(textProvNumInto.Text));
			string logText=Lan.g(this,"Providers merged")+": "+textAbbrFrom.Text+" "+Lan.g(this,"merged into")+" "+textAbbrInto.Text+".\r\n"
			+Lan.g(this,"Rows changed")+": "+POut.Long(rowsChanged);
			SecurityLogs.MakeLogEntry(Permissions.ProviderMerge,0,logText);
			textAbbrFrom.Clear();
			textProvNumFrom.Clear();
			textNpiFrom.Clear();
			textFullNameFrom.Clear();
			CheckUIState();
			MsgBox.Show(this,"Done.");
			DataValid.SetInvalid(InvalidType.Providers);
			_listProvidersActive=Providers.GetWhere(x => x.ProvStatus != ProviderStatus.Deleted,true);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}