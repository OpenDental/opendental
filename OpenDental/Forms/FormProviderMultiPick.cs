using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	///<summary>Only used in one place, when searching for an appt slot.  Could be rolled into FormProviderPick if done carefully.</summary>
	public partial class FormProvidersMultiPick:FormODBase {
		public List<Provider> SelectedProviders;
		private List<Provider> _listProviders;

		public FormProvidersMultiPick(List<Provider> listProviders=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listProviders=listProviders;
		}

		private void FormProvidersMultiPick_Load(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProviders","Abbrev"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","Last Name"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviders","First Name"),90);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			if(_listProviders==null) {
				_listProviders=Providers.GetDeepCopy(true);
			}
			for(int i=0;i<_listProviders.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listProviders[i].Abbr);
				row.Cells.Add(_listProviders[i].LName);
				row.Cells.Add(_listProviders[i].FName);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			List<long> listSelectedProvNums=SelectedProviders.Select(x => x.ProvNum).ToList();
			for(int i=0;i<_listProviders.Count;i++) {
				if(ListTools.In(_listProviders[i].ProvNum,listSelectedProvNums)) {
					gridMain.SetSelected(i,true);
				}
			}
		}


		private void butProvDentist_Click(object sender,EventArgs e) {
			SelectedProviders=new List<Provider>();
			for(int i=0;i<_listProviders.Count;i++) {
				if(!_listProviders[i].IsSecondary) {
					SelectedProviders.Add(_listProviders[i]);
					gridMain.SetSelected(i,true);
				}
				else {
					gridMain.SetSelected(i,false);
				}
			}
		}

		private void butProvHygenist_Click(object sender,EventArgs e) {
			SelectedProviders=new List<Provider>();
			for(int i=0;i<_listProviders.Count;i++) {
				if(_listProviders[i].IsSecondary) {
					SelectedProviders.Add(_listProviders[i]);
					gridMain.SetSelected(i,true);
				}
				else {
					gridMain.SetSelected(i,false);
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SelectedProviders=new List<Provider>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				SelectedProviders.Add(_listProviders[gridMain.SelectedIndices[i]]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}