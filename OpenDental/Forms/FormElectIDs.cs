using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;
using System.Linq;

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
		private List<ElectID> _listElectIDsToShow;

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
			comboCommBridge.IncludeAll=true;
			comboCommBridge.Items.AddEnums<EclaimsCommBridge>();
			Clearinghouse clearinghouseDefaultDental=Clearinghouses.GetDefaultDental();
			Clearinghouse clearinghouseDefaultMedical=Clearinghouses.GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultMed));
			if(clearinghouseDefaultDental is null && clearinghouseDefaultMedical is null) {
				comboCommBridge.IsAllSelected=true;
			}
			else {
				EclaimsCommBridge commBridge;
				if(clearinghouseDefaultDental!=null) {
					commBridge=clearinghouseDefaultDental.CommBridge;
				}
				else {
					commBridge=clearinghouseDefaultMedical.CommBridge;
				}
				comboCommBridge.SetSelectedEnum(commBridge);
			}
			FillElectIDs(0);
			butAdd.Visible=(!IsSelectMode);
			Plugins.HookAddCode(this,"FormElectIDs.Load_end");
		}

		private void FillElectIDs(long electIDNum) {
			ElectIDs.RefreshCache();
			_listElectIDs=ElectIDs.GetDeepCopy();
			_listElectIDsToShow=_listElectIDs;
			if(!comboCommBridge.IsAllSelected) {
				_listElectIDsToShow=_listElectIDs.FindAll(x => x.CommBridge==comboCommBridge.GetSelected<EclaimsCommBridge>());
			}
			gridElectIDs.BeginUpdate();
			gridElectIDs.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableApptProcs","Carrier"),320);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Payer ID"),60);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Is Medicaid"),70,HorizontalAlignment.Center);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","CommBridge"),100,HorizontalAlignment.Center);
			gridElectIDs.Columns.Add(col);
			col=new GridColumn(Lan.g("TableApptProcs","Comments"),350);
			gridElectIDs.Columns.Add(col);
			gridElectIDs.ListGridRows.Clear();
			GridRow row;
			int selectedIndex=-1;
			for(int i=0;i<_listElectIDsToShow.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listElectIDsToShow[i].CarrierName);
				row.Cells.Add(_listElectIDsToShow[i].PayorID);
				row.Cells.Add(_listElectIDsToShow[i].IsMedicaid?"X":"");
				row.Cells.Add(_listElectIDsToShow[i].CommBridge.ToString());
				row.Cells.Add(_listElectIDsToShow[i].Comments);
				gridElectIDs.ListGridRows.Add(row);
				if(_listElectIDsToShow[i].ElectIDNum==electIDNum) {
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
				ElectIDSelected=_listElectIDsToShow[e.Row];
				DialogResult=DialogResult.OK;
			}
			else {
				using FormElectIDEdit formElectIDEdit=new FormElectIDEdit();
				formElectIDEdit.ElectIDCur=_listElectIDsToShow[e.Row];
				if(formElectIDEdit.ShowDialog()==DialogResult.OK) {
					FillElectIDs(formElectIDEdit.ElectIDCur.ElectIDNum);
				}
			}
		}
		private void comboCommBridge_SelectionChangeCommitted(object sender,EventArgs e) {
			FillElectIDs(0);
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
				ElectIDSelected=_listElectIDsToShow[gridElectIDs.SelectedIndices[0]];
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















