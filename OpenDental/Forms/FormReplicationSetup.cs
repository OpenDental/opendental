using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using DataConnectionBase;

namespace OpenDental {
	public partial class FormReplicationSetup:FormODBase {
		private bool changed;
		private List<ReplicationServer> _listReplicationServers;

		public FormReplicationSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			changed=false;
		}

		private void FormReplicationSetup_Load(object sender,EventArgs e) {
			checkRandomPrimaryKeys.Checked=PrefC.GetBool(PrefName.RandomPrimaryKeys);
			if(checkRandomPrimaryKeys.Checked) {
				//not allowed to uncheck it
				checkRandomPrimaryKeys.Enabled=false;
			}
			else {
				checkRandomPrimaryKeys.Visible=false;//Don't allow them to turn random primary keys on if they don't already have it.
				this.Size=new Size(901,640);//resize if random primary keys are not turned on
				butSetRanges.Visible=false;
				label1.Visible=false;
				butTest.Visible=false;
				label2.Visible=false;
			}
			if(PrefC.GetInt(PrefName.ReplicationFailureAtServer_id)>0) {
				groupBoxReplicationFailure.Visible=true;
				textReplicaitonFailureAtServer_id.Text=PrefC.GetInt(PrefName.ReplicationFailureAtServer_id).ToString();
			}
			FillGrid();
		}

		private void FillGrid(){
			ReplicationServers.RefreshCache();
			_listReplicationServers=ReplicationServers.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormReplicationSetup","Description"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormReplicationSetup","server_id"),65);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.GetBool(PrefName.RandomPrimaryKeys)) {	
				col=new GridColumn(Lan.g("FormReplicationSetup","Key Range Start"),160);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("FormReplicationSetup","Key Range End"),160);
				gridMain.ListGridColumns.Add(col);
			}			
			col=new GridColumn(Lan.g("FormReplicationSetup","AtoZ Path"),160);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormReplicationSetup","UpdateBlocked"),100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormReplicationSetup","IsReport"),100,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listReplicationServers.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listReplicationServers[i].Descript);
				row.Cells.Add(_listReplicationServers[i].ServerId.ToString());
				if(PrefC.GetBool(PrefName.RandomPrimaryKeys)) {
					row.Cells.Add(_listReplicationServers[i].RangeStart.ToString("n0"));
					row.Cells.Add(_listReplicationServers[i].RangeEnd.ToString("n0"));
				}
				row.Cells.Add(_listReplicationServers[i].AtoZpath);
				row.Cells.Add(_listReplicationServers[i].UpdateBlocked ? "X" : "");
				row.Cells.Add(_listReplicationServers[i].ReplicationServerNum==PrefC.GetLong(PrefName.ReplicationUserQueryServer) ? "X" : "");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}
		
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReplicationEdit FormR=new FormReplicationEdit();
			FormR.RepServ=_listReplicationServers[e.Row];
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormReplicationEdit FormR=new FormReplicationEdit();
			FormR.RepServ=new ReplicationServer();
			FormR.RepServ.IsNew=true;
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillGrid();
		}

		private void butSetRanges_Click(object sender,EventArgs e) {
			if(_listReplicationServers.Count==0){
				MessageBox.Show(Lan.g(this,"Please add at least one replication server to the list first"));
				return;
			}
			//long serverCount=ReplicationServers.Listt.Count;
			long offset=10000;
			long maxValue=9000000000000000000;//use 9 quintillion instead of max long to ensure a buffer for the ranges.
			long span=(maxValue-offset) / (long)_listReplicationServers.Count;//rounds down
			long counter=offset;
			for(int i=0;i<_listReplicationServers.Count;i++) {
				_listReplicationServers[i].RangeStart=counter;
				counter+=span-1;
				if(i==_listReplicationServers.Count-1) {
					_listReplicationServers[i].RangeEnd=maxValue;
				}
				else {
					_listReplicationServers[i].RangeEnd=counter;
					counter+=1;
				}
				ReplicationServers.Update(_listReplicationServers[i]);
			}
			changed=true;
			FillGrid();
		}

		private void butTest_Click(object sender,EventArgs e) {
			long server_id=ReplicationServers.Server_id;
			string msg="";
			if(server_id==0) {
				msg="server_id not set for this server.\r\n\r\n";
			}
			else {
				msg="server_id = "+server_id.ToString()+"\r\n\r\n";
			} 
			msg+="Sample generated keys:";
			long key;
			List<long> longlist=new List<long>();
			for(int i=0;i<15;i++){
				do{
					key=ReplicationServers.GetKey("patient","PatNum");
					//unfortunately this "random" key is based on time, so we need to ensure that the result set is unique.
					//I think it takes one millisecond to get each key this way.
				}
				while(longlist.Contains(key));
				longlist.Add(key);
				msg+="\r\n"+key.ToString("n0");
			}
			MessageBox.Show(msg);
		}

		private void butSynch_Click(object sender,EventArgs e) {
			if(textUsername.Text=="") {
				MsgBox.Show(this,"Please enter a username first.");
				return;
			}
			if(_listReplicationServers.Count==0) {
				MsgBox.Show(this,"Please add at servers to the list first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string databaseNameOriginal=MiscData.GetCurrentDatabase();
			string compNameOriginal=DataConnection.GetServerName();
			DataConnection dc;
			bool isReplicationSucessfull=true;
			for(int i=0;i<_listReplicationServers.Count;i++) {
				string compName=_listReplicationServers[i].Descript;
				dc=new DataConnection();
				try {
					dc.SetDb(compName,databaseNameOriginal,textUsername.Text,textPassword.Text,"","",DataConnection.DBtype);
					//Connection is considered to be successfull at this point. Now restart the slave process to force replication.
					string command="STOP SLAVE; START SLAVE;";
					dc.NonQ(command);
					command="SHOW SLAVE STATUS";
					DataTable slaveStatus=dc.GetTable(command);
					//Wait for the slave process to become active again.
					for(int j=0;j<40 && slaveStatus.Rows[0]["Slave_IO_Running"].ToString().ToLower()!="yes";j++) {
						Thread.Sleep(1000);
						slaveStatus=dc.GetTable(command);
					}
					if(slaveStatus.Rows[0]["Slave_IO_Running"].ToString().ToLower()!="yes") {
						throw new ApplicationException(Lan.g(this,"Slave IO is not running on server")+" "+compName);
					}
					if(slaveStatus.Rows[0]["Slave_SQL_Running"].ToString().ToLower()!="yes") {
						throw new ApplicationException(Lan.g(this,"Slave SQL is not running on server")+" "+compName);
					}
					//Wait for replication to complete.
					while(slaveStatus.Rows[0]["Slave_IO_State"].ToString().ToLower()!="waiting for master to send event" || 
						slaveStatus.Rows[0]["Seconds_Behind_Master"].ToString()!="0") {
						slaveStatus=dc.GetTable(command);
					}
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(Lan.g(this,"Error forcing synch on server")+" "+compName+": "+ex.Message);
					isReplicationSucessfull=false;
					break;//Cancel operation.
				}
			}
			if(isReplicationSucessfull) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Database synch completed successfully."));
			}
			//Do not leave this function without connecting back to original database, or closing program.
			//At this point we are still connected to the last replication server in the list, try to connect back up to the original database.
			bool isReconnectSuccessful=false;
			while(!isReconnectSuccessful) {
				try {
					//Reconnect back to original database.
					dc=new DataConnection();
					dc.SetDb(compNameOriginal,databaseNameOriginal,textUsername.Text,textPassword.Text,"","",DataConnection.DBtype);
					isReconnectSuccessful=true;//No exception thrown, leave while loop.
				}
				catch(Exception ex) {
					if(MessageBox.Show(Lan.g(this,"Error reconnecting to server")+" "+compNameOriginal+": "+ex.Message+"\r\n"+
						Lan.g(this,"Would you like to retry?  Cancel will close the program."),"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
					{
						FormOpenDental.S_ProcessKillCommand();
					}
				}
			}
		}

		private void butResetReplicationFailureAtServer_id_Click(object sender,EventArgs e) {
			Prefs.UpdateInt(PrefName.ReplicationFailureAtServer_id,0);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormReplicationSetup_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed) {
				DataValid.SetInvalid(InvalidType.ReplicationServers);
			}
		}

	

		

		

		

	

		
		
	}
}