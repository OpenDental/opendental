using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoChartSetup:FormODBase {

		private List<OrthoChartTab> _listOrthoChartTabsDb=null;
		private List<OrthoChartTab> _listOrthoChartTabsNew=null;

		public FormOrthoChartSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoChartSetup_Load(object sender,EventArgs e) {
			OrthoChartTabs.RefreshCache();
			_listOrthoChartTabsDb=new List<OrthoChartTab>();
			_listOrthoChartTabsNew=new List<OrthoChartTab>();
			List<OrthoChartTab> listOrthoChartTabs=OrthoChartTabs.GetDeepCopy();
			for(int i=0;i<listOrthoChartTabs.Count;i++) {
				_listOrthoChartTabsDb.Add(listOrthoChartTabs[i].Copy());
				_listOrthoChartTabsNew.Add(listOrthoChartTabs[i].Copy());
			}
			FillGridTabNames();
		}

		private void FillGridTabNames() {
			gridTabNames.BeginUpdate();
			gridTabNames.ListGridRows.Clear();
			gridTabNames.Columns.Clear();
			int isHiddenWidth=100;
			int tabNameWidth=gridTabNames.Width-10-isHiddenWidth;//10 for scrollbar.
			gridTabNames.Columns.Add(new UI.GridColumn("Tab Name",tabNameWidth,HorizontalAlignment.Left));
			gridTabNames.Columns.Add(new UI.GridColumn("Is Hidden",isHiddenWidth,HorizontalAlignment.Center));
			for(int i=0;i<_listOrthoChartTabsNew.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Tag=_listOrthoChartTabsNew[i];
				row.Cells.Add(_listOrthoChartTabsNew[i].TabName);
				row.Cells.Add(_listOrthoChartTabsNew[i].IsHidden?"X":"");
				gridTabNames.ListGridRows.Add(row);
			}
			gridTabNames.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			OrthoChartTab orthoChartTab=new OrthoChartTab();
			FrmOrthoChartTabEdit frmOrthoChartTabEdit=new FrmOrthoChartTabEdit(orthoChartTab);
			frmOrthoChartTabEdit.ShowDialog();
			if(frmOrthoChartTabEdit.IsDialogOK) {
				_listOrthoChartTabsNew.Add(orthoChartTab);
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
			OrthoChartTab orthoChartTabTemp=_listOrthoChartTabsNew[index];
			_listOrthoChartTabsNew[index]=_listOrthoChartTabsNew[index+1];
			_listOrthoChartTabsNew[index+1]=orthoChartTabTemp;
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
			OrthoChartTab orthoChartTabTemp=_listOrthoChartTabsNew[index];
			_listOrthoChartTabsNew[index]=_listOrthoChartTabsNew[index-1];
			_listOrthoChartTabsNew[index-1]=orthoChartTabTemp;
			FillGridTabNames();
			gridTabNames.SetSelected(index-1,true);
		}

		private void gridTabNames_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			OrthoChartTab orthoChartTab=(OrthoChartTab)gridTabNames.ListGridRows[e.Row].Tag;
			FrmOrthoChartTabEdit frmOrthoChartTabEdit=new FrmOrthoChartTabEdit(orthoChartTab);
			frmOrthoChartTabEdit.ShowDialog();
			if(frmOrthoChartTabEdit.IsDialogOK) {
				FillGridTabNames();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool isVisible=false;
			for(int i=0;i<_listOrthoChartTabsNew.Count;i++) {
				_listOrthoChartTabsNew[i].ItemOrder=i;
				if(!_listOrthoChartTabsNew[i].IsHidden) {
					isVisible=true;
				}
			}
			if(!isVisible) {
				MsgBox.Show(this,"At least one tab must not be hidden.");
				return;
			}
			OrthoChartTabs.Sync(_listOrthoChartTabsNew,_listOrthoChartTabsDb);
			DataValid.SetInvalid(InvalidType.OrthoChartTabs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}