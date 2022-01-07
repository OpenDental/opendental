using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoChartSetup:FormODBase {

		private List<OrthoChartTab> _listDbOrthoChartTabs=null;
		private List<OrthoChartTab> _listNewOrthoChartTabs=null;

		public FormOrthoChartSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoChartSetup_Load(object sender,EventArgs e) {
			OrthoChartTabs.RefreshCache();
			_listDbOrthoChartTabs=new List<OrthoChartTab>();
			_listNewOrthoChartTabs=new List<OrthoChartTab>();
			foreach(OrthoChartTab orthoChartTab in OrthoChartTabs.GetDeepCopy()) {
				_listDbOrthoChartTabs.Add(orthoChartTab.Copy());
				_listNewOrthoChartTabs.Add(orthoChartTab.Copy());
			}
			FillGridTabNames();
		}

		private void FillGridTabNames() {
			gridTabNames.BeginUpdate();
			gridTabNames.ListGridRows.Clear();
			gridTabNames.ListGridColumns.Clear();
			int isHiddenWidth=100;
			int tabNameWidth=gridTabNames.Width-10-isHiddenWidth;//10 for scrollbar.
			gridTabNames.ListGridColumns.Add(new UI.GridColumn("Tab Name",tabNameWidth,HorizontalAlignment.Left));
			gridTabNames.ListGridColumns.Add(new UI.GridColumn("Is Hidden",isHiddenWidth,HorizontalAlignment.Center));
			foreach(OrthoChartTab orthoChartTab in _listNewOrthoChartTabs) {
				UI.GridRow row=new UI.GridRow();
				row.Tag=orthoChartTab;
				row.Cells.Add(orthoChartTab.TabName);
				row.Cells.Add(orthoChartTab.IsHidden?"X":"");
				gridTabNames.ListGridRows.Add(row);
			}
			gridTabNames.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			OrthoChartTab orthoChartTab=new OrthoChartTab();
			using FormOrthoChartTabEdit form=new FormOrthoChartTabEdit(orthoChartTab);
			if(form.ShowDialog()==DialogResult.OK) {
				_listNewOrthoChartTabs.Add(orthoChartTab);
				FillGridTabNames();
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridTabNames.SelectedIndices.Length==0) {
				return;//no selection
			}
			int index=gridTabNames.SelectedIndices[0];
			if(index==gridTabNames.ListGridRows.Count-1) {
				return;//end of list
			}
			OrthoChartTab orthoChartTabTemp=_listNewOrthoChartTabs[index];
			_listNewOrthoChartTabs[index]=_listNewOrthoChartTabs[index+1];
			_listNewOrthoChartTabs[index+1]=orthoChartTabTemp;
			FillGridTabNames();
			gridTabNames.SetSelected(index+1,true);
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridTabNames.SelectedIndices.Length==0) {
				return;//no selection
			}
			int index=gridTabNames.SelectedIndices[0];
			if(index==0) {
				return;//beginning of list
			}
			OrthoChartTab orthoChartTabTemp=_listNewOrthoChartTabs[index];
			_listNewOrthoChartTabs[index]=_listNewOrthoChartTabs[index-1];
			_listNewOrthoChartTabs[index-1]=orthoChartTabTemp;
			FillGridTabNames();
			gridTabNames.SetSelected(index-1,true);
		}

		private void gridTabNames_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			OrthoChartTab orthoChartTab=(OrthoChartTab)gridTabNames.ListGridRows[e.Row].Tag;
			using FormOrthoChartTabEdit form=new FormOrthoChartTabEdit(orthoChartTab);
			if(form.ShowDialog()==DialogResult.OK) {
				FillGridTabNames();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool isVisible=false;
			for(int i=0;i<_listNewOrthoChartTabs.Count;i++) {
				_listNewOrthoChartTabs[i].ItemOrder=i;
				if(!_listNewOrthoChartTabs[i].IsHidden) {
					isVisible=true;
				}
			}
			if(!isVisible) {
				MsgBox.Show(this,"At least one tab must not be hidden.");
				return;
			}
			OrthoChartTabs.Sync(_listNewOrthoChartTabs,_listDbOrthoChartTabs);
			DataValid.SetInvalid(InvalidType.OrthoChartTabs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}