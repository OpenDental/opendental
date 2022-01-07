using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormRedundantIndexes:FormODBase {

		public FormRedundantIndexes() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRedundantIndexes_Load(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				checkLogAddStatements.CheckedChanged-=checkLogAddStatements_CheckedChanged;
				checkLogAddStatements.Checked=false;
				checkLogAddStatements.Enabled=false;
			}
			FillGrid();
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"There are no redundant indexes in the current database.");
				DialogResult=DialogResult.OK;
			}
		}

		private void FillGrid() {
			DataTable table=DatabaseMaintenances.GetRedundantIndexesTable();
			if(table.Rows.Count==0) {
				return;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Table"),165));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Index"),165));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Index Columns"),200));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Redundant Of"),615));
			gridMain.ListGridRows.Clear();
			gridMain.ListGridRows.AddRange(table.Select().Select(x =>
				new GridRow(new[] { "TABLE_NAME","INDEX_NAME","INDEX_COLS","REDUNDANT_OF" }.Select(y => PIn.String(x[y].ToString())).ToArray()) { Tag=x }));
			gridMain.EndUpdate();
		}

		private void checkLogAddStatements_CheckedChanged(object sender,EventArgs e) {
			if(!checkLogAddStatements.Checked) {
				string msgText="It is recommended that you log the statements to add the indexes back in case you wish to undo the actions performed by this "
					+"tool.  Without the log the indexes dropped by this tool cannot be recovered.\r\n"
					+"Do you want to re-enable logging of the statements?";
				checkLogAddStatements.Checked=MsgBox.Show(this,MsgBoxButtons.YesNo,msgText);
			}
		}

		private void butAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
		}

		private void butNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
		}

		private void butDrop_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select the index(es) to drop first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This tool could take a long time to finish.  Do you wish to continue?")) {
				return;
			}
			if(checkLogAddStatements.Checked) {
				string path="";
				try {
					path=GetLogFilePath();
					using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(Lan.g(this,"SQL statements to add the selected index(es) back will be in the following location")+":\r\n"+path);
					msgBox.ShowDialog();
				}
				catch(Exception ex) {
					MsgBox.Show(ex.Message+"\r\n"+(ex.InnerException?.Message??""));
				}
			}
			string logText="";
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => logText=DatabaseMaintenances.DropRedundantIndexes(gridMain.SelectedTags<DataRow>());
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(Lan.g(this,"There was an error dropping redundant indexes")+":\r\n"+ex.Message);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			if(checkLogAddStatements.Checked) {
				try {
					SaveLogToFile(logText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(Lan.g(this,"Could not create or modify the log file. Copy and paste the following queries into a text file and "
						+"save it in case you ever want to undo the actions performed by this tool.")+"\r\n\r\n"+logText);
					msgBox.ShowDialog();
				}
			}
			MsgBox.Show(this,"Done.");
			DialogResult=DialogResult.OK;
		}
		
		///<summary>Adds the logText to a centralized log file for the current day if the current data storage type is LocalAtoZ.
		///Throws exceptions to be displayed to the user.</summary>
		private void SaveLogToFile(string logText) {
			//No need to check RemotingRole; no call to db.
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				//If docs are stored in DB, we don't want to create a file because the user has no way to access it.
				//We also skip any cloud storage at this time, could enhance later to include.
				return; //Don't make a log.
			}
			string machineName="~INVALID~";
			ODException.SwallowAnyException(() => { machineName=Environment.MachineName; });
			try {
				//will append to existing file or create new one for the date if it doesn't exist
				File.AppendAllText(GetLogFilePath(),DateTime.Now.ToString()+" - Computer Name: "+machineName+new string('-',45)+Environment.NewLine+logText);
			}
			catch(SecurityException se) {
				throw new ODException(Lan.g(this,"Log not saved to Drop Index Logs folder because user does not have permission to access that file."),se);
			}
			catch(UnauthorizedAccessException uae) {
				throw new ODException(Lan.g(this,"Log not saved to Drop Index Logs folder because user does not have permission to access that file."),uae);
			}
			//Throw all other types of exceptions like usual.
		}

		private string GetLogFilePath() {
			string path=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"DropIndexLogs");
			try {
				if(!Directory.Exists(path)) {
					Directory.CreateDirectory(path);//Create DropIndexLogs folder if it doesn't exist
				}
			}
			catch(Exception ex) {
				throw new ODException(Lan.g(this,"Could not create or access the directory for saving the Drop Index Log file."),ex);
			}
			return ODFileUtils.CombinePaths(path,DateTime.Now.ToString("M_d_yyyy")+".txt");//One file per date
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}