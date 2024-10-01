using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlEnterpriseReports:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlEnterpriseReports() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void checkUseReportServer_CheckedChanged(object sender,EventArgs e) {
			SetReportServerUIEnabled();
		}

		private void RadioReportServerDirect_CheckedChanged(object sender,EventArgs e) {
			SetReportServerUIEnabled();
		}

		private void ComboDatabase_DropDown(object sender,EventArgs e) {
			FillComboDatabases();
		}

		#region Methods - Event Handlers Sync

		private void textReportingServerCompName_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerCompName);
			prefValSync.PrefVal=textReportingServerCompName.Text;
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void textReportingServerURI_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerURI);
			prefValSync.PrefVal=textReportingServerURI.Text;
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Methods - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		private void FillComboDatabases() {
			comboReportingServerDbName.Items.Clear();
			comboReportingServerDbName.Items.AddRange(GetDatabases());
		}

		private string[] GetDatabases() {
			if(textReportingServerCompName.Text=="") {
				return new string[0];
			}
			DataConnection dataConnection;
			DataTable table;
			string command="SHOW DATABASES";
			//use the one table that we know exists
			if(textReportingServerMySqlUser.Text=="") {
				dataConnection=new DataConnection(textReportingServerCompName.Text,"mysql","root",textMysqlPass.Text,DatabaseType.MySql);
			}
			else {
				dataConnection=new DataConnection(textReportingServerCompName.Text,"mysql",textReportingServerMySqlUser.Text,textMysqlPass.Text,DatabaseType.MySql);
			}
			try {	//if this next step fails, table will simply have 0 rows
				table=dataConnection.GetTable(command,false);
			}
			catch(Exception) {
				return new string[0];
			}
			string[] stringArrayNames=new string[table.Rows.Count];
			for(int i=0;i<table.Rows.Count;i++) {
				stringArrayNames[i]=table.Rows[i][0].ToString();
			}
			return stringArrayNames;
		}

			private void SetReportServerUIEnabled() {
			if(checkReportingServerCompNameOrURI.Checked) {
				radioReportServerDirect.Enabled=true;
				radioReportServerMiddleTier.Enabled=true;
				if(radioReportServerDirect.Checked) {
					groupConnectionSettings.Enabled=true;
					groupMiddleTier.Enabled=false;
				}
				else {
					groupConnectionSettings.Enabled=false;
					groupMiddleTier.Enabled=true;
				}
			}
			else {
				radioReportServerDirect.Enabled=false;
				radioReportServerMiddleTier.Enabled=false;
				groupConnectionSettings.Enabled=false;
				groupMiddleTier.Enabled=false;
			}
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillEnterpriseReports() {
			checkReportingServerCompNameOrURI.Checked=(PrefC.GetString(PrefName.ReportingServerCompName)!="" || PrefC.GetString(PrefName.ReportingServerURI)!="");
			//textReportingServerCompName.Text=PrefC.GetString(PrefName.ReportingServerCompName);
			comboReportingServerDbName.Text=PrefC.GetString(PrefName.ReportingServerDbName);
			textReportingServerMySqlUser.Text=PrefC.GetString(PrefName.ReportingServerMySqlUser);
			string decryptedPass;
			CDT.Class1.Decrypt(PrefC.GetString(PrefName.ReportingServerMySqlPassHash),out decryptedPass);
			textMysqlPass.Text=decryptedPass;
			//textReportingServerURI.Text=PrefC.GetString(PrefName.ReportingServerURI);
			FillComboDatabases();
			SetReportServerUIEnabled();
		}

		public bool SaveEnterpriseReports() {
			if(!checkReportingServerCompNameOrURI.Checked) {
				Changed|=Prefs.UpdateString(PrefName.ReportingServerCompName,"");
				Changed|=Prefs.UpdateString(PrefName.ReportingServerDbName,"");
				Changed|=Prefs.UpdateString(PrefName.ReportingServerMySqlUser,"");
				Changed|=Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,"");
				Changed|=Prefs.UpdateString(PrefName.ReportingServerURI,"");
			}
			else {
				if(radioReportServerDirect.Checked) {
					string reportingServerMySqlPassHash;
					CDT.Class1.Encrypt(textMysqlPass.Text,out reportingServerMySqlPassHash);
					Changed|=Prefs.UpdateString(PrefName.ReportingServerCompName,textReportingServerCompName.Text);
					Changed|=Prefs.UpdateString(PrefName.ReportingServerDbName,comboReportingServerDbName.Text);
					Changed|=Prefs.UpdateString(PrefName.ReportingServerMySqlUser,textReportingServerMySqlUser.Text);
					Changed|=Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,reportingServerMySqlPassHash);
					Changed|=Prefs.UpdateString(PrefName.ReportingServerURI,"");
				}
				else {
					Changed|=Prefs.UpdateString(PrefName.ReportingServerCompName,"");
					Changed|=Prefs.UpdateString(PrefName.ReportingServerDbName,"");
					Changed|=Prefs.UpdateString(PrefName.ReportingServerMySqlUser,"");
					Changed|=Prefs.UpdateString(PrefName.ReportingServerMySqlPassHash,"");
					Changed|=Prefs.UpdateString(PrefName.ReportingServerURI,textReportingServerURI.Text);
				}
			}
			if(Changed) {
				DataValid.SetInvalid(InvalidType.ConnectionStoreClear);
			}
			return true;
		}

		public void FillSynced() {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerCompName);
			textReportingServerCompName.Text=prefValSync.PrefVal;
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerURI);
			textReportingServerURI.Text=prefValSync.PrefVal;
		}
		#endregion Methods - Public
	}
}
