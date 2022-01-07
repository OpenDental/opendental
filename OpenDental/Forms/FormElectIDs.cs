using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormElectIDs : FormODBase {
		///<summary></summary>
		public bool IsSelectMode;
		///<summary></summary>
		public ElectID selectedID;
		private List<ElectID> _listElectIDs;

		///<summary></summary>
		public FormElectIDs() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormElectIDs_Load(object sender, System.EventArgs e) {
			FillElectIDs(0);
			butAdd.Visible=(!IsSelectMode);
			Plugins.HookAddCode(this,"FormElectIDs.Load_end");
		}

		private void FillElectIDs(long electIDSelect) {
			ElectIDs.RefreshCache();
			_listElectIDs=ElectIDs.GetDeepCopy();
			gridElectIDs.BeginUpdate();
			gridElectIDs.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptProcs","Carrier"),320);
			gridElectIDs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Payer ID"),80);
			gridElectIDs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Is Medicaid"),70,HorizontalAlignment.Center);
			gridElectIDs.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Comments"),390);
			gridElectIDs.ListGridColumns.Add(col);
			gridElectIDs.ListGridRows.Clear();
			GridRow row;
			int selectedIndex=-1;
			for(int i=0;i<_listElectIDs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listElectIDs[i].CarrierName);
				row.Cells.Add(_listElectIDs[i].PayorID);
				row.Cells.Add(_listElectIDs[i].IsMedicaid?"X":"");
				row.Cells.Add(_listElectIDs[i].Comments);
				gridElectIDs.ListGridRows.Add(row);
				if(_listElectIDs[i].ElectIDNum==electIDSelect) {
					selectedIndex=i;
				}
			}
			gridElectIDs.EndUpdate();
			gridElectIDs.SetSelected(selectedIndex,true);
		}

		private void gridElectIDs_CellClick(object sender,ODGridClickEventArgs e) {
			gridElectIDs.SetSelected(e.Row,true);
		}

		private void gridElectIDs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectMode) {
				selectedID=_listElectIDs[e.Row];
				DialogResult=DialogResult.OK;
			}
			else {
				using FormElectIDEdit FormEdit=new FormElectIDEdit();
				FormEdit.electIDCur=_listElectIDs[e.Row];
				if(FormEdit.ShowDialog()==DialogResult.OK) {
					FillElectIDs(FormEdit.electIDCur.ElectIDNum);
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormElectIDEdit FormEdit=new FormElectIDEdit();
			FormEdit.electIDCur=new ElectID();
			FormEdit.electIDCur.IsNew=true;
			if(FormEdit.ShowDialog()==DialogResult.OK) {
				FillElectIDs(FormEdit.electIDCur.ElectIDNum);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(IsSelectMode) {
				if(gridElectIDs.SelectedIndices.Length<1) {
					MessageBox.Show(Lan.g(this,"Please select an item first."));
					return;
				}
				selectedID=_listElectIDs[gridElectIDs.SelectedIndices[0]];
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}





















