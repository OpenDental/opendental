using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDatabaseMaintTemp:FormODBase {
		//private bool backupMade;
		private string _duplicateClaimProcInfo;
		private string _duplicateSuppInfo;
		private string _missingSuppInfo;

		public FormDatabaseMaintTemp() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDatabaseMaintTemp_Load(object sender,EventArgs e) {
			FillDatabaseNames();
		}

		private void FillDatabaseNames(){
			comboDbs.Items.Clear();
			List<string> listDbNames=DatabaseMaintenances.GetDatabaseNames();
			for(int i=0;i<listDbNames.Count;i++){
				comboDbs.Items.Add(listDbNames[i]);
			}
			//automatic selection will come later.
		}

		private void butRun_Click(object sender,EventArgs e) {
			if(comboDbs.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a backup database first.");
				return;
			}
			//make sure it's not this database
			if(comboDbs.SelectedItem.ToString()==MiscData.GetCurrentDatabase()){
				MsgBox.Show(this,"Please choose a database other than the current database.");
				return;
			}
			//make sure it's from before March 17th.
			//if(!DatabaseMaintenance.DatabaseIsOlderThanMarchSeventeenth(comboDbs.SelectedItem.ToString())){
			//	MsgBox.Show(this,"The backup database must be older than March 17, 2010.");
			//	return;
			//}
			Cursor=Cursors.WaitCursor;
			textResults.Text="";
			_duplicateClaimProcInfo=DatabaseMaintenances.GetDuplicateClaimProcs();
			if(_duplicateClaimProcInfo==""){
				textResults.Text+="Duplicate claim payments: None found.  Database OK.\r\n\r\n";
			}
			else{
				textResults.Text+=_duplicateClaimProcInfo;
			}
			_duplicateSuppInfo=DatabaseMaintenances.GetDuplicateSupplementalPayments();
			if(_duplicateSuppInfo==""){
				textResults.Text+="Duplicate supplemental payments: None found.  Database OK.\r\n\r\n";
			}
			else{
				textResults.Text+=_duplicateSuppInfo;
			}
			_missingSuppInfo=DatabaseMaintenances.GetMissingClaimProcs(comboDbs.SelectedItem.ToString());
			if(_missingSuppInfo==""){
				textResults.Text+="Missing claim payments: None found.  Database OK.";
			}
			else{
				textResults.Text+=_missingSuppInfo;
			}
			Cursor=Cursors.Default;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			string fileName=CodeBase.ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),"results.txt");
			ODFileUtils.WriteAllTextThenStart(fileName,textResults.Text,"");
			MsgBox.Show(this,"Please print from the text editor.");
		}

		private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("http://www.opendental.com/manual/bugcp.html");
		}

		private void butBackup_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(!ODBuild.IsDebug()) {
				Shared.MakeABackup(BackupLocation.DatabaseMaintenanceTool);
			}
			//backupMade=true;
			if(_duplicateClaimProcInfo!=""){
				butFix1.Enabled=true;
			}
			if(_duplicateSuppInfo!=""){
				butFix2.Enabled=true;
			}
			if(_missingSuppInfo!=""){
				butFix3.Enabled=true;
			}
			Cursor=Cursors.Default;
		}

		private void butFix1_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			textResults.Text=DatabaseMaintenances.FixClaimProcDeleteDuplicates();
			Cursor=Cursors.Default;
		}

		private void butFix2_Click(object sender,EventArgs e) {
			MessageBox.Show("There is not yet a fix for duplicate supplemental payments due to concern about false positives. If you have duplicates, we will need to get a copy of your database to analyze it here.");
		}

		private void butFix3_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			textResults.Text=DatabaseMaintenances.FixMissingClaimProcs(comboDbs.SelectedItem.ToString());
			Cursor=Cursors.Default;
		}

		
	}
}