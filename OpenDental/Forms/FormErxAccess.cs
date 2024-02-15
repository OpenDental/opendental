using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This form is only visible at HQ, or possibly at a distributor HQ.  This form is used to control provider access to eRx.</summary>
	public partial class FormErxAccess:FormODBase {

		private Patient _patient;
		private List<ProviderErx> _listProviderErxs;

		public FormErxAccess(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}

		private void FormErxAccess_Load(object sender,EventArgs e) {
			FillProviders();
		}

		private void FillProviders() {
			gridProviders.BeginUpdate();
			gridProviders.ListGridRows.Clear();
			gridProviders.Columns.Clear();
			gridProviders.Columns.Add(new UI.GridColumn("Type",74,HorizontalAlignment.Left));//Column width determined in LayoutProviders().
			gridProviders.Columns.Add(new UI.GridColumn("IsEnabled",74,HorizontalAlignment.Center));//Column width determined in LayoutProviders().
			gridProviders.Columns.Add(new UI.GridColumn("IsIDPd",74,HorizontalAlignment.Center));//Column width determined in LayoutProviders().
			gridProviders.Columns.Add(new UI.GridColumn("IsEPCS",74,HorizontalAlignment.Center));
			UI.GridColumn col=new UI.GridColumn("NPI",74,HorizontalAlignment.Left);
			col.IsWidthDynamic=true; //Column width determined in LayoutProviders().
			gridProviders.Columns.Add(col);
			//Gets from db.  Better to call db than to use cache at HQ, since cache might be large.
			//Only get Legacy eRx items.  Other types will be in the BroadcasterMonitor.
			_listProviderErxs=ProviderErxs.Refresh(_patient.PatNum).FindAll(x => x.ErxType==ErxOption.NewCrop);
			for(int i=0;i<_listProviderErxs.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Tag=_listProviderErxs[i];
				row.Cells.Add(new UI.GridCell(_listProviderErxs[i].ErxType.ToString()));
				string status="";
				if(_listProviderErxs[i].IsEnabled==ErxStatus.Enabled) {
					status="X";
				}
				else if(_listProviderErxs[i].IsEnabled!=ErxStatus.Disabled) {
					status="P";
				}
				row.Cells.Add(new UI.GridCell(status));
				row.Cells.Add(new UI.GridCell(_listProviderErxs[i].IsIdentifyProofed?"X":""));
				row.Cells.Add(new UI.GridCell(_listProviderErxs[i].IsEpcs?"X":""));
				row.Cells.Add(new UI.GridCell(_listProviderErxs[i].NationalProviderID));
				gridProviders.ListGridRows.Add(row);
			}
			gridProviders.EndUpdate();
		}

		private void checkShowDoseSpot_Click(object sender,EventArgs e) {
			FillProviders();
		}

		private void checkShowLegacy_Click(object sender,EventArgs e) {
			FillProviders();
		}

		private void butEnable_Click(object sender,EventArgs e) {
			if(gridProviders.SelectedIndices.Length==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			gridProviders.BeginUpdate();
			for(int i=0;i<gridProviders.SelectedIndices.Length;i++) {
				int index=gridProviders.SelectedIndices[i];
				UI.GridRow row=gridProviders.ListGridRows[index];
				row.Cells[1].Text="X";
				ProviderErx providerErx=(ProviderErx)row.Tag;
				providerErx.IsEnabled=ErxStatus.Enabled;
			}
			gridProviders.EndUpdate();
		}

		private void butDisable_Click(object sender,EventArgs e) {
			if(gridProviders.SelectedIndices.Length==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Only use this button if the selected providers were Enabled accidentally or if the provider has canceled eRx.  "
				+"Continue?"))
			{
				return;
			}
			gridProviders.BeginUpdate();
			for(int i=0;i<gridProviders.SelectedIndices.Length;i++) {
				int index=gridProviders.SelectedIndices[i];
				UI.GridRow row=gridProviders.ListGridRows[index];
				row.Cells[1].Text="";
				ProviderErx providerErx=(ProviderErx)row.Tag;
				providerErx.IsEnabled=ErxStatus.Disabled;
			}
			gridProviders.EndUpdate();
		}

		private void butIdpd_Click(object sender,EventArgs e) {
			if(gridProviders.SelectedIndices.Length==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			gridProviders.BeginUpdate();
			for(int i=0;i<gridProviders.SelectedIndices.Length;i++) {
				int index=gridProviders.SelectedIndices[i];
				UI.GridRow row=gridProviders.ListGridRows[index];
				row.Cells[2].Text="X";
				ProviderErx providerErx=(ProviderErx)row.Tag;
				providerErx.IsIdentifyProofed=true;
			}
			gridProviders.EndUpdate();
		}

		private void butNotIdpd_Click(object sender,EventArgs e) {
			if(gridProviders.SelectedIndices.Length==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Only use this button if the selected providers were set to IDP'd accidentally.  Continue?")) {
				return;
			}
			gridProviders.BeginUpdate();
			for(int i=0;i<gridProviders.SelectedIndices.Length;i++) {
				int index=gridProviders.SelectedIndices[i];
				UI.GridRow row=gridProviders.ListGridRows[index];
				row.Cells[2].Text="";
				ProviderErx providerErx=(ProviderErx)row.Tag;
				providerErx.IsIdentifyProofed=false;
			}
			gridProviders.EndUpdate();
		}

		private void butEPCS_Click(object sender,EventArgs e) {
			if(gridProviders.SelectedIndices.Length==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			gridProviders.BeginUpdate();
			for(int i=0;i<gridProviders.SelectedIndices.Length;i++) {
				int index=gridProviders.SelectedIndices[i];
				UI.GridRow row=gridProviders.ListGridRows[index];
				row.Cells[3].Text="X";
				ProviderErx providerErx=(ProviderErx)row.Tag;
				providerErx.IsEpcs=true;
			}
			gridProviders.EndUpdate();
		}

		private void butNotEPCS_Click(object sender,EventArgs e) {
			if(gridProviders.SelectedIndices.Length==0) {
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			gridProviders.BeginUpdate();
			for(int i=0;i<gridProviders.SelectedIndices.Length;i++) {
				int index=gridProviders.SelectedIndices[i];
				UI.GridRow row=gridProviders.ListGridRows[index];
				row.Cells[3].Text="";
				ProviderErx providerErx=(ProviderErx)row.Tag;
				providerErx.IsEpcs=false;
			}
			gridProviders.EndUpdate();
		}

		private void butSave_Click(object sender,EventArgs e) {
			List<ProviderErx> listProviderErxes=ProviderErxs.Refresh(_patient.PatNum).FindAll(x => x.ErxType==ErxOption.NewCrop);
			ProviderErxs.Sync(_listProviderErxs,listProviderErxes);//No cache refresh because this is an HQ only form.
			DialogResult=DialogResult.OK;
		}

	}
}