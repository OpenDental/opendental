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
		public ElectID ElectIDSelected;
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

		private void FillElectIDs(long electIDNum) {
			ElectIDs.RefreshCache();
			_listElectIDs=ElectIDs.GetDeepCopy();
			gridElectIDs.BeginUpdate();
			gridElectIDs.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptProcs","Carrier"),320);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Payer ID"),80);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Is Medicaid"),70,HorizontalAlignment.Center);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Comments"),390);
			gridElectIDs.Columns.Add(col);
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
				if(_listElectIDs[i].ElectIDNum==electIDNum) {
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
				ElectIDSelected=_listElectIDs[e.Row];
				DialogResult=DialogResult.OK;
			}
			else {
				using FormElectIDEdit formElectIDEdit=new FormElectIDEdit();
				formElectIDEdit.ElectIDCur=_listElectIDs[e.Row];
				if(formElectIDEdit.ShowDialog()==DialogResult.OK) {
					FillElectIDs(formElectIDEdit.ElectIDCur.ElectIDNum);
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormElectIDEdit formElectIDEdit=new FormElectIDEdit();
			formElectIDEdit.ElectIDCur=new ElectID();
			formElectIDEdit.ElectIDCur.IsNew=true;
			if(formElectIDEdit.ShowDialog()==DialogResult.OK) {
				FillElectIDs(formElectIDEdit.ElectIDCur.ElectIDNum);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(IsSelectMode) {
				if(gridElectIDs.SelectedIndices.Length<1) {
					MessageBox.Show(Lan.g(this,"Please select an item first."));
					return;
				}
				ElectIDSelected=_listElectIDs[gridElectIDs.SelectedIndices[0]];
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}





















