using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CentralManager {
	public partial class FormCentralConnections:Form {
		///<summary>When OK is clicked, this list will contain all selected connections.</summary>
		public List<CentralConnection> ListConnsSelected;
		///<summary>List of all connections, ordered.  Will be filtered</summary>
		private List<CentralConnection> _listConnsAll;
		///<summary>A filtered list of connections.  Gets refilled every time FillGrid is called.</summary>
		private List<CentralConnection> _listConnsShowing;
		private List<ConnectionGroup> _listConnectionGroups;
		///<summary>Used when selecting a connection for patient transfer and when pushing security settings to a db.</summary>
		public bool IsSelectionMode;

		public FormCentralConnections(){
			InitializeComponent();
		}

		private void FormCentralConnections_Load(object sender,EventArgs e) {
			checkIsAutoLogon.Checked=PrefC.GetBool(PrefName.CentralManagerIsAutoLogon);
			checkUseDynamicMode.Checked=PrefC.GetBool(PrefName.CentralManagerUseDynamicMode);
			_listConnectionGroups=ConnectionGroups.GetAll();
			comboConnectionGroups.Items.Add("All");
			comboConnectionGroups.Items.AddRange(_listConnectionGroups.Select(x => x.Description).ToArray());
			comboConnectionGroups.SelectedIndex=0;//Default to all.
			if(IsSelectionMode) {
				groupPrefs.Visible=false;
				butAdd.Visible=false;
				groupOrdering.Visible=false;
			}
			else{
				butOK.Visible=false;
				butCancel.Text="Close";
			}
			_listConnsAll=CentralConnections.GetConnections();
			//fix any bad item orders
			bool foundBad=false;
			for(int i=0;i<_listConnsAll.Count;i++){
				if(_listConnsAll[i].ItemOrder!=i){
					foundBad=true;
					_listConnsAll[i].ItemOrder=i;
					CentralConnections.Update(_listConnsAll[i]);
				}
			}
			if(foundBad){
				_listConnsAll=CentralConnections.GetConnections();
			}
			FillGrid();
		}

		private void FillGrid() {
			//this gets called a lot if typing search.  So just include the following line in other places.
			//_listConnsAll=CentralConnections.GetConnections();
			_listConnsShowing=null;
			if(comboConnectionGroups.SelectedIndex>0) {
				_listConnsShowing=CentralConnections.FilterConnections(_listConnsAll,textSearch.Text,_listConnectionGroups[comboConnectionGroups.SelectedIndex-1]);
			}
			else {
				_listConnsShowing=CentralConnections.FilterConnections(_listConnsAll,textSearch.Text,null);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("#",40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Database",300);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",260);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			int[] selectedIndices=gridMain.SelectedIndices;
			for(int i=0;i<_listConnsShowing.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listConnsShowing[i].ItemOrder.ToString());
				if(_listConnsShowing[i].DatabaseName=="") {//uri
					row.Cells.Add(_listConnsShowing[i].ServiceURI);
				}
				else {
					row.Cells.Add(_listConnsShowing[i].ServerName+", "+_listConnsShowing[i].DatabaseName);
				}
				row.Cells.Add(_listConnsShowing[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedIndices,true);
		}

		private void comboConnectionGroups_SelectionChangeCommitted(object sender,EventArgs e) {
			SetToolsEnabled();
			FillGrid();
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			SetToolsEnabled();
			FillGrid();
		}

		private void SetToolsEnabled(){
			if(comboConnectionGroups.SelectedIndex==0 //'All'
				&& textSearch.Text=="")
			{
				butUp.Enabled=true;
				butDown.Enabled=true;
				butAlphabetize.Enabled=true;
			}
			else {
				//We only let them re-order if 'All' is selected.
				butUp.Enabled=false;
				butDown.Enabled=false;
				butAlphabetize.Enabled=false;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode) {
				ListConnsSelected=new List<CentralConnection>();
				ListConnsSelected.Add(_listConnsShowing[e.Row]);
				DialogResult=DialogResult.OK;
				return;
			}
			using FormCentralConnectionEdit FormCCE=new FormCentralConnectionEdit();
			FormCCE.CentralConnectionCur=_listConnsShowing[e.Row];
			FormCCE.ShowDialog();
			_listConnsAll=CentralConnections.GetConnections();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			CentralConnection conn=new CentralConnection();
			conn.IsNew=true;
			using FormCentralConnectionEdit FormCCE=new FormCentralConnectionEdit();
			FormCCE.CentralConnectionCur=conn;
			FormCCE.LastItemOrder=0;
			if(_listConnsAll.Count>0) {
				FormCCE.LastItemOrder=_listConnsAll[_listConnsAll.Count-1].ItemOrder;
			}
			FormCCE.ShowDialog();
			_listConnsAll=CentralConnections.GetConnections();
			FillGrid();
		}
		
		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MessageBox.Show(Lans.g(this,"Please select a connection, first."));
				return;
			}
			if(gridMain.SelectedIndices.Length>1){
				MessageBox.Show(Lans.g(this,"Please only select one connection."));
				return;
			}
			int idx=gridMain.SelectedIndices[0];
			if(_listConnsShowing.Count<2 || idx==0){
				return;//nothing to do
			}
			_listConnsShowing[idx].ItemOrder--;
			CentralConnections.Update(_listConnsShowing[idx]);
			_listConnsShowing[idx-1].ItemOrder++;
			CentralConnections.Update(_listConnsShowing[idx-1]);
			_listConnsAll=CentralConnections.GetConnections();
			FillGrid();
			gridMain.SetSelected(idx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MessageBox.Show(Lans.g(this,"Please select a connection, first."));
				return;
			}
			if(gridMain.SelectedIndices.Length>1){
				MessageBox.Show(Lans.g(this,"Please only select one connection."));
				return;
			}
			int idx=gridMain.SelectedIndices[0];
			if(_listConnsShowing.Count<2 || idx==_listConnsShowing.Count-1){
				return;//nothing to do
			}
			_listConnsShowing[idx].ItemOrder++;
			CentralConnections.Update(_listConnsShowing[idx]);
			_listConnsShowing[idx+1].ItemOrder--;
			CentralConnections.Update(_listConnsShowing[idx+1]);
			_listConnsAll=CentralConnections.GetConnections();
			FillGrid();
			gridMain.SetSelected(idx+1,true);
		}

		private void butAlphabetize_Click(object sender,EventArgs e) {
			_listConnsShowing.Sort(delegate(CentralConnection x,CentralConnection y){
				return x.Note.CompareTo(y.Note);
			});
			for(int i=0;i<_listConnsShowing.Count;i++){
				_listConnsShowing[i].ItemOrder=i;
				CentralConnections.Update(_listConnsShowing[i]);
			}
			_listConnsAll=CentralConnections.GetConnections();
			FillGrid();
		}

		private void CheckIsAutoLogon_Click(object sender, EventArgs e){
			//there's no ok button when this is showing. Act immediately.
			Prefs.UpdateBool(PrefName.CentralManagerIsAutoLogon,checkIsAutoLogon.Checked);
			//DataValid.SetInvalid(InvalidType.Prefs);//This doesn't exist in CEMT
		}

		private void CheckUseDynamicMode_Click(object sender, EventArgs e){
			Prefs.UpdateBool(PrefName.CentralManagerUseDynamicMode,checkUseDynamicMode.Checked);
		}

		private void butOK_Click(object sender,EventArgs e) {
			//only visible in selection mode
			if(gridMain.SelectedIndices.Length==0){
				MessageBox.Show(Lans.g(this,"Please select connection(s) first."));
				return;
			}
			ListConnsSelected=new List<CentralConnection>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				ListConnsSelected.Add(_listConnsShowing[gridMain.SelectedIndices[i]]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}