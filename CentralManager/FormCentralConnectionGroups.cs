using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CentralManager {
	public partial class FormCentralConnectionGroups:Form {
		private List<ConnectionGroup> _listCentralConnGroups;

		public FormCentralConnectionGroups() {
			InitializeComponent();
		}

		private void FormCentralConnectionGroups_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			_listCentralConnGroups=ConnectionGroups.GetAll();
			//Get all conn group attaches because we will be using all of them in order to show counts.
			List<ConnGroupAttach> listConnGroupAttaches=ConnGroupAttaches.GetAll();
			//always fills combo, too, since it's the same list
			long defaultConnGroupNum=PrefC.GetLong(PrefName.ConnGroupCEMT);
			comboConnectionGroup.Items.Clear();
			comboConnectionGroup.Items.Add(Lan.g(this,"All"));
			comboConnectionGroup.SelectedIndex=0;//Select all by default.
			//Fill in the list of conn groups and update the selected index of the combo box if needed.
			for(int i=0;i<_listCentralConnGroups.Count;i++) {
				comboConnectionGroup.Items.Add(_listCentralConnGroups[i].Description);
				if(_listCentralConnGroups[i].ConnectionGroupNum==defaultConnGroupNum) {
					comboConnectionGroup.SelectedIndex=i+1;
				}
			}
			//grid-----------------------------------------------------------------------------------------------------
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lans.g(this,"Group Name"),280);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Conns"),280){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listCentralConnGroups.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listCentralConnGroups[i].Description);
				row.Cells.Add(listConnGroupAttaches.FindAll(x => x.ConnectionGroupNum==_listCentralConnGroups[i].ConnectionGroupNum).Count.ToString());
				//row.Tag=connGroup;//Not really used currently, but we may want to add filtering later.
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			ConnectionGroup connGroup=new ConnectionGroup();
			connGroup.Description="Group";//in case they close the window without clicking ok
			connGroup.ConnectionGroupNum=ConnectionGroups.Insert(connGroup);
			connGroup.IsNew=true;
			using FormCentralConnectionGroupEdit FormCCGE=new FormCentralConnectionGroupEdit();
			FormCCGE.ConnectionGroupCur=connGroup;
			FormCCGE.ShowDialog();
			//if cancel, deleted inside
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormCentralConnectionGroupEdit FormCCGE=new FormCentralConnectionGroupEdit();
			FormCCGE.ConnectionGroupCur=_listCentralConnGroups[e.Row];
			FormCCGE.ShowDialog();
			//disregard dialog result
			FillGrid();
		}

		private void comboConnectionGroup_SelectionChangeCommitted(object sender, EventArgs e){
			if(comboConnectionGroup.SelectedIndex==0) {
				Prefs.UpdateLong(PrefName.ConnGroupCEMT,0);
			}
			else {
				Prefs.UpdateLong(PrefName.ConnGroupCEMT,_listCentralConnGroups[comboConnectionGroup.SelectedIndex-1].ConnectionGroupNum);
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void FormCentralConnectionGroups_FormClosing(object sender,FormClosingEventArgs e) {
			
		}

	


	}
}
