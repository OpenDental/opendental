using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReplicationEdit:FormODBase {
		public ReplicationServer RepServ;

		public FormReplicationEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReplicationEdit_Load(object sender,EventArgs e) {
			textDescript.Text=RepServ.Descript;
			textServerId.Text=RepServ.ServerId.ToString();
			if(PrefC.GetBool(PrefName.RandomPrimaryKeys)) {
				if(RepServ.RangeStart!=0) {
					textRangeStart.Text=RepServ.RangeStart.ToString();
				}
				if(RepServ.RangeEnd!=0) {
					textRangeEnd.Text=RepServ.RangeEnd.ToString();
				}
			}
			else {
				textRangeStart.Visible=false;
				label4.Visible=false;
				textRangeEnd.Visible=false;
				label5.Visible=false;
				label8.Visible=false;
			}
			textAtoZpath.Text=RepServ.AtoZpath;
			checkUpdateBlocked.Checked=RepServ.UpdateBlocked;
			if(RepServ.ReplicationServerNum==PrefC.GetLong(PrefName.ReplicationUserQueryServer)) {
				checkReportServer.Checked=true;
			}
		}

		private void butThisComputerDesc_Click(object sender,EventArgs e) {
			textDescript.Text=CodeBase.ODEnvironment.GetLocalIPAddress();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(RepServ.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			if(RepServ.ReplicationServerNum==PrefC.GetLong(PrefName.ReplicationUserQueryServer)) {//Current report server.
				if(Prefs.UpdateLong(PrefName.ReplicationUserQueryServer,0)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			ReplicationServers.DeleteObject(RepServ.ReplicationServerNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescript.Text=="") {
				//I guess we don't need to force descript to have a value
			}
			if(!textServerId.IsValid()) {
				MsgBox.Show(this,"Please fix server_id.");
				return;
			}
			int serverid=PIn.Int(textServerId.Text);
			if(serverid==0) {
				MsgBox.Show(this,"Please enter a server_id number greater than zero.");
				return;
			}
			long rangeStart=0;
			if(textRangeStart.Text != "") {
				try {
					rangeStart=long.Parse(textRangeStart.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix range start.");
					return;
				}
			}
			long rangeEnd=0;
			if(textRangeEnd.Text != "") {
				try {
					rangeEnd=long.Parse(textRangeEnd.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix range end.");
					return;
				}
			}
			if((textRangeStart.Text !="" || textRangeEnd.Text !="") && rangeEnd-rangeStart<999999) {
				MsgBox.Show(this,"The end of the range must be at least 999,999 greater than the start of the range.");
				return;
			}
			//Disallow a range that ends after 9 quintillion.  This is because the max value of a long is 9,223,372,036,854,775,807
			//and we want to leave room for the convert script, which doesn't implement our random primary key logic.
			//It will instead use the auto-increment number for the table.  
			//If the table had a primary key of 9,223,372,036,854,775,807, the auto-increment would be out of bounds and give an error.
			//So now the largest random primary key value for any table will be 9 quintillion, which leaves the convert script 200 quadrillion entries
			//before going out of bounds.
			if(rangeEnd>9000000000000000000) {
				MsgBox.Show(this,"The end of the range must be less than or equal to nine quintillion.");
				return;
			}
			RepServ.Descript=textDescript.Text;
			RepServ.ServerId=serverid;//will be valid and greater than 0.
			RepServ.RangeStart=rangeStart;
			RepServ.RangeEnd=rangeEnd;
			RepServ.AtoZpath=textAtoZpath.Text;
			RepServ.UpdateBlocked=checkUpdateBlocked.Checked;
			if(RepServ.IsNew) {
				ReplicationServers.Insert(RepServ);
			}
			else {
				ReplicationServers.Update(RepServ);
			}
			//Update the ReplicationUserQueryServer preference as needed.
			if(checkReportServer.Checked) {
				if(Prefs.UpdateLong(PrefName.ReplicationUserQueryServer,RepServ.ReplicationServerNum)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else if(RepServ.ReplicationServerNum==PrefC.GetLong(PrefName.ReplicationUserQueryServer)) {//If this replication server was the original report server, set the current server to 0.
				if(Prefs.UpdateLong(PrefName.ReplicationUserQueryServer,0)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}

		
}