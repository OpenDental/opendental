using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CentralManager {
	public partial class FormCentralConnectionGroupEdit:Form {
		///<summary>List of connections that are already attached to this group and showing in the grid.</summary>
		private List<CentralConnection> _listConnections;
		///<summary>List of connections in the right grid that are available to attach.</summary>
		private List<CentralConnection> _listConnectionsAvail;
		///<summary></summary>
		public ConnectionGroup ConnectionGroupCur;

		public FormCentralConnectionGroupEdit() {
			InitializeComponent();
		}

		private void FormCentralConnectionGroupEdit_Load(object sender,EventArgs e) {
			textDescription.Text=ConnectionGroupCur.Description;
			FillGrid();
			FillGridAvail();
		}

		private void FillGrid() {
			_listConnections=CentralConnections.GetForGroup(ConnectionGroupCur.ConnectionGroupNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Database",320);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",300);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listConnections.Count;i++) {
				row=new GridRow();
				if(_listConnections[i].DatabaseName=="") {//uri
					row.Cells.Add(_listConnections[i].ServiceURI);
				}
				else {
					row.Cells.Add(_listConnections[i].ServerName+", "+_listConnections[i].DatabaseName);
				}
				row.Cells.Add(_listConnections[i].Note);
				//row.Tag=_listConnections[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridAvail() {
			_listConnectionsAvail=CentralConnections.GetNotForGroup(ConnectionGroupCur.ConnectionGroupNum);
			gridAvail.BeginUpdate();
			gridAvail.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Database",320);
			gridAvail.ListGridColumns.Add(col);
			col=new GridColumn("Note",300);
			gridAvail.ListGridColumns.Add(col);
			gridAvail.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listConnectionsAvail.Count;i++) {
				row=new GridRow();
				if(_listConnectionsAvail[i].DatabaseName=="") {//uri
					row.Cells.Add(_listConnectionsAvail[i].ServiceURI);
				}
				else {
					row.Cells.Add(_listConnectionsAvail[i].ServerName+", "+_listConnectionsAvail[i].DatabaseName);
				}
				row.Cells.Add(_listConnectionsAvail[i].Note);
				//row.Tag=_listConnections[i];
				gridAvail.ListGridRows.Add(row);
			}
			gridAvail.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(gridAvail.SelectedIndices.Length==0){
				MessageBox.Show(Lans.g(this,"Please select connections first."));
				return;
			}
			for(int i=0;i<gridAvail.SelectedIndices.Length;i++){
				ConnGroupAttaches.Attach(_listConnectionsAvail[gridAvail.SelectedIndices[i]].CentralConnectionNum,ConnectionGroupCur.ConnectionGroupNum);
			}
			FillGrid();
			FillGridAvail();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MessageBox.Show(Lans.g(this,"Please select a connection first."));
				return;
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				ConnGroupAttaches.Detach(_listConnections[gridMain.SelectedIndices[i]].CentralConnectionNum,ConnectionGroupCur.ConnectionGroupNum);
			}
			FillGrid();
			FillGridAvail();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MessageBox.Show(this,Lans.g(this,"Delete this entire connection group?"),"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return;
			}
			ConnGroupAttaches.DeleteForGroup(ConnectionGroupCur.ConnectionGroupNum);
			ConnectionGroups.Delete(ConnectionGroupCur.ConnectionGroupNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text=="") {
				MessageBox.Show(Lans.g(this,"Please enter a description."));
				return;
			}
			ConnectionGroupCur.Description=textDescription.Text;
			ConnectionGroups.Update(ConnectionGroupCur);//was already inserted if new
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, EventArgs e){
			DialogResult=DialogResult.Cancel;
		}

		private void FormCentralConnectionGroupEdit_FormClosing(object sender, FormClosingEventArgs e){
			if(DialogResult==DialogResult.OK){
				return;
			}
			if(ConnectionGroupCur.IsNew){
				ConnGroupAttaches.DeleteForGroup(ConnectionGroupCur.ConnectionGroupNum);
				ConnectionGroups.Delete(ConnectionGroupCur.ConnectionGroupNum);
			}
		}



	}
}
