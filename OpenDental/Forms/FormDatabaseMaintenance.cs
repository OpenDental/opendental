using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Summary description for FormDatabaseMaintenance.</summary>
	public partial class FormDatabaseMaintenance:FormODBase {
		///<summary>Holds any text from the log that still needs to be printed when the log spans multiple pages.</summary>
		private string LogTextPrint;
		///<summary>A list of every single DBM method in the database.  Filled on load right after "syncing" the DBM methods to the db.</summary>
		private List<DatabaseMaintenance> _listDatabaseMaintenances;
		///<summary>This is a filtered list of all methods from DatabaseMaintenances.cs that have the DbmMethod attribute.</summary>
		private List<MethodInfo> _listDbmMethods;
		///<summary>This is a filtered list of methods from DatabaseMaintenances.cs that have the DbmMethod attribute and are not hidden or old.  
		///This is used to populate gridMain.</summary>
		private List<MethodInfo> _listDbmMethodsGrid;
		///<summary>This is a filtered list of methods from DatabaseMaintenances.cs that have the DbmMethod attribute and are hidden.  
		///This is used to populate gridHidden.</summary>
		private List<MethodInfo> _listDbmMethodsGridHidden;
		///<summary>This is a filtered list of methods from DatabaseMaintenances.cs that have the DbmMethod attribute and are marked as old.  
		///This is used to populate gridOld.</summary>
		private List<MethodInfo> _listDbmMethodsGridOld;
		///<summary>Holds the date and time of the last time a Check or Fix was run.  Only used for printing.</summary>
		private DateTime _dateTimeLastRun;
		///<summary>This bool keeps track of whether we need to invalidate cache for all users.</summary>
		private bool _isCacheInvalid; 
		///<summary>Thread to manage running DBMs, allows us to cancel mid-run.</summary>
		ODThread _threadRunDBM=null;
		/// <summary>Flag to have the RunDBM thread exit early. This should ONLY be set within the main thread and read by the worker thread.</summary>
		private volatile bool _isCancelled;
		///<summary>So the user doesn't have to enter the password to remove redundant indexes multiple times while the form is open.</summary>
		private bool _redundantPasswordEntered=false;

		///<summary></summary>
		public FormDatabaseMaintenance() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.C(this,new System.Windows.Forms.Control[]{
				this.textBox1,
				//this.textBox2
			});
			Lan.F(this);
		}

		private void FormDatabaseMaintenance_Load(object sender,System.EventArgs e) {
			_listDbmMethods=DatabaseMaintenances.GetMethodsForDisplay(Clinics.ClinicNum);
			DatabaseMaintenances.InsertMissingDBMs(_listDbmMethods.Select(x => x.Name).ToList());
			_listDatabaseMaintenances=DatabaseMaintenances.GetAll();
			if(PrefC.GetBool(PrefName.DatabaseMaintenanceSkipCheckTable)) {
				labelSkipCheckTable.Visible=true;
			}
			FillGrid();
			FillGridHidden();
			FillGridOld();
			FillGridTools();
			textBoxUpdateInProg.Text=PrefC.GetString(PrefName.UpdateInProgressOnComputerName);
			if(string.IsNullOrWhiteSpace(textBoxUpdateInProg.Text)) {
				butClearUpdateInProgress.Enabled=false;
			}
		}

		private void FillGrid() {
			_listDbmMethodsGrid=GetDbmMethodsForGrid();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),300));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Break\r\nDown"),40,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Results"),300){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDbmMethodsGrid.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listDbmMethodsGrid[i].Name);
				row.Cells.Add(DatabaseMaintenances.MethodHasBreakDown(_listDbmMethodsGrid[i]) ? "X" : "");
				row.Cells.Add("");
				row.Tag=_listDbmMethodsGrid[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridHidden() {
			_listDbmMethodsGridHidden=GetDbmMethodsForGrid(isHidden: true,isOld: false);
			gridHidden.BeginUpdate();
			gridHidden.ListGridColumns.Clear();
			gridHidden.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),340));
			gridHidden.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_listDbmMethodsGridHidden.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listDbmMethodsGridHidden[i].Name);
				row.Tag=_listDbmMethodsGridHidden[i];
				gridHidden.ListGridRows.Add(row);
			}
			gridHidden.EndUpdate();
		}

		private void FillGridOld() {
			_listDbmMethodsGridOld=GetDbmMethodsForGrid(isHidden: false,isOld: true);
			_listDbmMethodsGridOld.AddRange(GetDbmMethodsForGrid(isHidden: true,isOld: true));
			_listDbmMethodsGridOld.Sort(new MethodInfoComparer());
			gridOld.BeginUpdate();
			gridOld.ListGridColumns.Clear();
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),300));
			if(checkShowHidden.Checked) {
				gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,"Hidden"),45,HorizontalAlignment.Center));
			}
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,"Break\r\nDown"),40,HorizontalAlignment.Center));
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,"Results"),300){ IsWidthDynamic=true });
			gridOld.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_listDbmMethodsGridOld.Count;i++) {
				bool isMethodHidden=_listDatabaseMaintenances.Any(x => x.MethodName==_listDbmMethodsGridOld[i].Name && x.IsHidden);
				if(!checkShowHidden.Checked && isMethodHidden) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_listDbmMethodsGridOld[i].Name);
				if(checkShowHidden.Checked) {
					row.Cells.Add(isMethodHidden ? "X" : "");
				}
				row.Cells.Add(DatabaseMaintenances.MethodHasBreakDown(_listDbmMethodsGridOld[i]) ? "X" : "");
				row.Cells.Add("");
				row.Tag=_listDbmMethodsGridOld[i];
				gridOld.ListGridRows.Add(row);
			}
			gridOld.EndUpdate();
		}

		private void FillGridTools() {
			gridTools.BeginUpdate();
			gridTools.ListGridColumns.Clear();
			gridTools.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),300,GridSortingStrategy.StringCompare));
			gridTools.ListGridColumns.Add(new GridColumn(Lan.g(this,"Description"),300,GridSortingStrategy.StringCompare));
			gridTools.ListGridRows.Clear();
			GridRow row=new GridRow(Lan.g(this,"Ins Pay Fix"),Lan.g(this,"Creates checks for insurance payments that are not attached to a check."));
			row.Tag=new Action(InsPayFix);
			gridTools.ListGridRows.Add(row);
			if(!PrefC.GetBool(PrefName.DatabaseMaintenanceDisableOptimize)) {
				row=new GridRow(Lan.g(this,"Optimize"),Lan.g(this,"Back up, optimize, and repair tables."));
				row.Tag=new Action(OptimizeFix);
				gridTools.ListGridRows.Add(row);
			}
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				row=new GridRow(Lan.g(this,"Appt Procs"),Lan.g(this,"Fixes procs in the Appt module that aren't correctly showing tooth nums."));
				row.Tag=new Action(ApptProcsFix);
				gridTools.ListGridRows.Add(row);
			}
			row=new GridRow(Lan.g(this,"Spec Char"),Lan.g(this,"Removes special characters from appt notes and appt proc descriptions."));
			row.Tag=new Action(SpecCharFix);
			gridTools.ListGridRows.Add(row);
			if(!ODBuild.IsWeb()) {
				//If the office converted their db to MyISAM, their backups would stop working.
				row=new GridRow(Lan.g(this,"InnoDB"),Lan.g(this,"Converts database storage engine to/from InnoDb."));
				row.Tag=new Action(InnoDBFix);
				gridTools.ListGridRows.Add(row);
			}
			row=new GridRow(Lan.g(this,"Tokens"),Lan.g(this,"Validates tokens on file with the X-Charge server."));
			row.Tag=new Action(TokensFix);
			gridTools.ListGridRows.Add(row);
			if(DataConnection.DBtype!=DatabaseType.Oracle) {
				row=new GridRow(Lan.g(this,"Remove Nulls"),Lan.g(this,"Replace all null strings with empty strings."));
				row.Tag=new Action(RemoveNullsFix);
				gridTools.ListGridRows.Add(row);
			}
			if(Security.IsAuthorized(Permissions.SecurityAdmin,true)){
				row=new GridRow(Lan.g(this,"Etrans"),Lan.g(this,"Clear out etrans entries older than a year old."));
				row.Tag=new Action(EtransFix);
				gridTools.ListGridRows.Add(row);
			}
			row=new GridRow(Lan.g(this,"Active TP's"),Lan.g(this,"Creates an active treatment plan for all pats with treatment planned procs."));
			row.Tag=new Action(ActiveTPsFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Raw Emails"),Lan.g(this,"Fixes emails which are encoded in the Chart progress notes.  Also clears unused attachments."));
			row.Tag=new Action(RawEmailsFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Email Attaches"),Lan.g(this,"Moves email attachment files into the correct In or Out folders."));
			row.Tag=new Action(EmailAttachesFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Recalc Estimates"),Lan.g(this,"Recalc estimates that are associated to non active coverage for the patient."));
			row.Tag=new Action(RecalcEstFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Pay Plan Payments"),Lan.g(this,"Detaches patient payments attached to insurance payment plans and insurance payments attached to patient payment plans."));
			row.Tag=new Action(PayPlanPaymentsFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Balance Families"),Lan.g(this,"Runs income transfer logic for multiple familes at once to zero out family balances."));
			row.Tag=new Action(FamilyBalanceFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Redundant Indexes"),Lan.g(this,"Removes redundant/unnecessary indexes from all tables in the database."));
			row.Tag=new Action(RedundantIndexesFix);
			gridTools.ListGridRows.Add(row);
			row=new GridRow(Lan.g(this,"Patient Missing"),Lan.g(this,"Helps to fix database corruption with loss of patients."));
			row.Tag=new Action(PatientMissingFix);
			gridTools.ListGridRows.Add(row);
			gridTools.EndUpdate();
		}

		private List<MethodInfo> GetDbmMethodsForGrid(bool isHidden=false,bool isOld=false) {
			List<string> listMethods=_listDatabaseMaintenances.FindAll(x => x.IsHidden==isHidden && x.IsOld==isOld)
				.Select(y => y.MethodName).ToList();
			return _listDbmMethods.FindAll(x => ListTools.In(x.Name,listMethods));
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			RunMethodBreakDown(_listDbmMethodsGrid[e.Row]);
		}

		private void gridOld_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			RunMethodBreakDown(_listDbmMethodsGridOld[e.Row]);
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			OnMouseUp(e,gridMain);
		}

		private void gridHidden_MouseUp(object sender,MouseEventArgs e) {
			OnMouseUp(e,gridHidden);
		}

		private void gridOld_MouseUp(object sender,MouseEventArgs e) {
			OnMouseUp(e,gridOld);
		}

		private void OnMouseUp(MouseEventArgs e,GridOD grid) {
			if(grid.SelectedIndices.Length==0 || e.Button!=MouseButtons.Right) {
				contextMenuStrip1.Hide();
				return;
			}
			MethodInfo method=(MethodInfo)grid.ListGridRows[grid.SelectedIndices[0]].Tag;
			if(method!=null) {
				bool isMethodHidden=_listDatabaseMaintenances.Any(x => x.MethodName==method.Name && x.IsHidden);
				hideToolStripMenuItem.Visible=!isMethodHidden;
				unhideToolStripMenuItem.Visible=isMethodHidden;
			}
		}

		private void hideToolStripMenuItem_Click(object sender,EventArgs e) {
			//Users can only hide DBM methods from gridMain or gridOld.
			switch(tabControlDBM.SelectedIndex) {
				case 0://tabChecks
					UpdateDbmIsHiddenForGrid(gridMain,true);
					break;
				case 2://tabOld
					UpdateDbmIsHiddenForGrid(gridOld,true);
					break;
				case 1://tabHidden
				case 3://tabTools
				default:
					return;
			}
		}

		private void unhideToolStripMenuItem_Click(object sender,EventArgs e) {
			//Users can only unhide DBM methods from gridHidden or gridOld.
			switch(tabControlDBM.SelectedIndex) {
				case 1://tabHidden
					UpdateDbmIsHiddenForGrid(gridHidden,false);
					break;
				case 2://tabOld
					UpdateDbmIsHiddenForGrid(gridOld,false);
					break;
				case 0://tabChecks
				case 3://tabTools
				default:
					return;
			}
		}

		private void UpdateDbmIsHiddenForGrid(GridOD grid,bool isHidden) {
			for(int i=0;i<grid.SelectedIndices.Length;i++) {
				MethodInfo method=(MethodInfo)grid.ListGridRows[grid.SelectedIndices[i]].Tag;
				DatabaseMaintenance dbm=_listDatabaseMaintenances.FirstOrDefault(x=>x.MethodName==method.Name);
				if(dbm==null) {
					continue;
				}
				dbm.IsHidden=isHidden;
				DatabaseMaintenances.Update(dbm);
			}
			_listDatabaseMaintenances=DatabaseMaintenances.GetAll();
			FillGrid();
			FillGridHidden();
			FillGridOld();
		}

		private void butNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
		}

		private void butNoneOld_Click(object sender,EventArgs e) {
			gridOld.SetAll(false);
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridHidden.SetAll(true);
		}

		#region Database Tools

		private void butClearUpdateInProgress_Click(object sender,EventArgs e) {
			Prefs.UpdateString(PrefName.UpdateInProgressOnComputerName,"");
			DataValid.SetInvalid(InvalidType.Prefs);
			textBoxUpdateInProg.Text="";
		}

		private void InsPayFix() {
			using FormInsPayFix formIns=new FormInsPayFix();
			formIns.ShowDialog();
		}

		private void OptimizeFix() {
			if(MessageBox.Show(Lan.g("FormDatabaseMaintenance","This tool will backup, optimize, and repair all tables.")+"\r\n"+Lan.g("FormDatabaseMaintenance","Continue?")
				,Lan.g("FormDatabaseMaintenance","Backup Optimize Repair")
				,MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			string result="";
			if(Shared.BackupRepairAndOptimize(true,BackupLocation.OptimizeTool)) {
				result=DateTime.Now.ToString()+"\r\n"+Lan.g("FormDatabaseMaintenance","Repair and Optimization Complete");
			}
			else {
				result=DateTime.Now.ToString()+"\r\n";
				result+=Lan.g("FormDatabaseMaintenance","Backup, repair, or optimize has failed.  Your database has not been altered.")+"\r\n";
				result+=Lan.g("FormDatabaseMaintenance","Please call support for help, a manual backup of your data must be made before trying to fix your database.")+"\r\n";
			}
			Cursor=Cursors.Default;
			SaveLogToFile(result);
			MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(result);
			msgBoxCP.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
		}

		private void ApptProcsFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will fix procedure descriptions in the Appt module that aren't correctly showing tooth numbers.\r\nContinue?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			Appointments.UpdateProcDescriptForAppts(Appointments.GetForPeriod(DateTime.Now.Date.AddMonths(-6),DateTime.MaxValue.AddDays(-10)).ToList());
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Done. Please refresh Appt module to see the changes.");
		}

		private void SpecCharFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This is only used if your mobile synch or middle tier is failing.  This cannot be undone.  Do you wish to continue?")) {
				return;
			}
			using InputBox box=new InputBox("In our online manual, on the database maintenance page, look for the password and enter it below.");
			if(box.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(box.textResult.Text!="fix") {
				MessageBox.Show("Wrong password.");
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=DatabaseMaintenances.FixSpecialCharacters;
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			MsgBox.Show(this,"Done.");
			_isCacheInvalid=true;//Definitions are cached and could have been changed from above DBM.
		}

		private void InnoDBFix() {
			using FormInnoDb form=new FormInnoDb();
			form.ShowDialog();
		}

		private void TokensFix() {
			using FormXchargeTokenTool FormXCT=new FormXchargeTokenTool();
			FormXCT.ShowDialog();
		}

		private void RemoveNullsFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will replace ALL null strings in your database with empty strings.  This cannot be undone.  Do you wish to continue?")) {
				return;
			}
			MessageBox.Show(Lan.g(this,"Number of null strings replaced with empty strings")+": "+DatabaseMaintenances.MySqlRemoveNullStrings());
			_isCacheInvalid=true;//The above DBM could have potentially changed cached tables. 
		}

		private void EtransFix() {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				MsgBox.Show(this,"Tool does not currently support Oracle.  Please call support to see if you need this fix.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear out etrans message text entries over a year old.  An automatic backup of the database will be created before deleting any entries.  This process may take a while to run depending on the size of your database.  Continue?")) {
				return;
			}
			if(!ODBuild.IsDebug()) {
				if(!Shared.MakeABackup(BackupLocation.DatabaseMaintenanceTool)) {
					MsgBox.Show(this,"Etrans message text entries were not altered.  Please try again.");
					return;
				}
			}
			DatabaseMaintenances.ClearOldEtransMessageText();
			MsgBox.Show(this,"Etrans message text entries over a year old removed");
		}

		private void ActiveTPsFix() {
			Cursor=Cursors.WaitCursor;
			List<Procedure> listTpTpiProcs=DatabaseMaintenances.GetProcsNoActiveTp();
			Cursor=Cursors.Default;
			if(listTpTpiProcs.Count==0) {
				MsgBox.Show(this,"Done");
				return;
			}
			int numTPs=listTpTpiProcs.Select(x => x.PatNum).Distinct().ToList().Count;
			int numTPAs=listTpTpiProcs.Count;
			TimeSpan estRuntime=TimeSpan.FromSeconds((numTPs+numTPAs)*0.001d);
			//the factor 0.001 was determined by running tests on a large db
			//212631 TPAs and 30000 TPs were inserted in 225 seconds
			//225/(212631+30000)=0.0009273341 seconds per inserted row (rounded up to 0.001 seconds)
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"From your database size we estimate that this could take "+(estRuntime.Minutes+1)+" minutes to create "
				+numTPs+" treatment plans for "+numTPAs+" procedures if running form the server.\r\nDo you wish to continue?"))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			string msg=DatabaseMaintenances.CreateMissingActiveTPs(listTpTpiProcs);
			Cursor=Cursors.Default;
			if(string.IsNullOrEmpty(msg)) {
				msg="Done";
			}
			MsgBox.Show(this,msg);
		}

		private void PatientMissingFix() {
			MsgBox.Show(this,DatabaseMaintenances.PatientMissing());
		}

		private void RawEmailsFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo
				,"This tool is only necessary to run if utilizing the email inbox feature.\r\n"
				+"Run this tool if email messages are encoded in the Chart progress notes, \r\n"
				+"or if the emailmessage table has grown to a large size.\r\n"
				+"This will decode any encoded clear text emails and will remove unused attachment content.\r\n\r\n"
				+"This tool could take a long time to finish, do you wish to continue?"))
			{
				return;
			}
			string results="";
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => results=DatabaseMaintenances.CleanUpRawEmails();
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				results=Lan.g(this,"There was an error cleaning up email bloat:")+"\r\n"+ex.Message;
			}
			if(progressOD.IsCancelled){
				return;
			}
			MessageBox.Show(results);
		}

		private void EmailAttachesFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo
				,"This tool is only necessary to run if utilizing the email feature.\r\n"
				+"Run this tool if there are files that start with 'In_' or 'Out_' within the AtoZ EmailAttachments folder.  "
				+"The issue is evident when trying to view an attachment and a file not found error occurs.\r\n\r\n"
				+"This tool could take a long time to finish, do you wish to continue?"))
			{
				return;
			}
			string results="";
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => results=DatabaseMaintenances.CleanUpAttachmentsRootDirectory();
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				results=Lan.g(this,"There was an error cleaning up email attachments:")+"\r\n"+ex.Message;
			}
			if(progressOD.IsCancelled){
				return;
			}
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(results);
			msgBoxCopyPaste.Show();
		}

		private void RecalcEstFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo
				,"This tool will mimic what happens when you click OK in the procedure edit window.  "
				+"The tool will identify patients with at least one estimate which belongs to a dropped insurance plan.  "
				+"For each such patient, estimates will be recalculated for current plans, and  "
				+"for plans which have been dropped, estimates associated to the dropped plans will be deleted.\r\n"
				+"This tool could take a long time to finish, do you wish to continue?"))
			{
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => DatabaseMaintenances.RecalcEstimates(Procedures.GetProcsWithOldEstimates());
			progressOD.ShowDialogProgress();
		}
		
		private void PayPlanPaymentsFix() {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo
				,"You are running a tool to detach patient payments from insurance payment plans and detach insurance payments from patient payment plans.  "
				+"The payments will still exist, and because they will now be reflected in the account instead of the payment plan, historical and "
				+"current account balances may change.  Proceed?"))
			{
				return;
			}
			
			string results="";
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => results=DatabaseMaintenances.DetachInvalidPaymentPlanPayments();
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(results);
			msgBoxCopyPaste.ShowInTaskbar=true;
			msgBoxCopyPaste.Text=Lan.g(this,"Payments Fixed");
			msgBoxCopyPaste.Show();
		}

		private void FamilyBalanceFix() {
			using InputBox inputbox=new InputBox("Please enter password");
			inputbox.setTitle("Family Balancer");
			inputbox.textResult.PasswordChar='*';
			inputbox.ShowDialog();
			if(inputbox.DialogResult!=DialogResult.OK) {
				return;
			}
			if(inputbox.textResult.Text!="ConversionsDepartment") {
				MsgBox.Show(this,"Wrong password");
				return;
			}
			if(Security.IsAuthorized(Permissions.SecurityAdmin)) {
				using FormFamilyBalancer form=new FormFamilyBalancer();
				form.ShowDialog();
			}
		}

		private void RedundantIndexesFix() {
			if(!_redundantPasswordEntered) {
				using InputBox inputbox=new InputBox("Please enter password");
				inputbox.setTitle("Remove Redundant Indexes");
				inputbox.textResult.PasswordChar='*';
				inputbox.ShowDialog();
				if(inputbox.DialogResult!=DialogResult.OK) {
					return;
				}
				if(inputbox.textResult.Text!="abracadabra") {
					_redundantPasswordEntered=false;
					MsgBox.Show(this,"Wrong password");
					return;
				}
				_redundantPasswordEntered=true;
			}
			using FormRedundantIndexes formRI=new FormRedundantIndexes();
			formRI.ShowDialog();			
		}

		#endregion

		private void butTemp_Click(object sender,EventArgs e) {
			using FormDatabaseMaintTemp form=new FormDatabaseMaintTemp();
			form.ShowDialog();
		}

		private void Run(GridOD grid,DbmMode modeCur) {
			if(grid.ListGridRows.Count > 0 && grid.ListGridColumns.Count < 3) {//Enforce the requirement of having the Results column as the third column.
				MsgBox.Show(this,"Must have at least three columns in the grid.");
				return;
			}
			ToggleUI(true);//Turn off all UI buttons except the Stop DBM button
			_isCancelled=false;
			Cursor=Cursors.WaitCursor;
			int colresults=2;
			if(grid==gridOld && checkShowHidden.Checked) {
				colresults=3;//There is an extra "Hidden" column to account for when setting the "Results" column.
			}
			//Clear out the result column for all rows before every "run"
			for(int i=0;i<grid.ListGridRows.Count;i++) {
				grid.ListGridRows[i].Cells[colresults].Text="";//Don't use UpdateResultTextForRow here because users will see the rows clearing out one by one.
			}
			bool verbose=checkShow.Checked;
			StringBuilder logText=new StringBuilder();
			ODTuple<string,bool> tableCheckResult=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => tableCheckResult=DatabaseMaintenances.MySQLTables(verbose,modeCur);
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				Cursor=Cursors.Default;
				ToggleUI(false);
				return;
			}
			logText.Append(tableCheckResult.Item1);
			//No database maintenance methods should be run unless this passes.
			if(!tableCheckResult.Item2) {
				Cursor=Cursors.Default;
				MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(tableCheckResult.Item1);//tableCheckResult is already translated.
				msgBoxCP.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
				ToggleUI(false);
				return;
			}
			if(grid.SelectedIndices.Length < 1) {
				//No rows are selected so the user wants to run all checks.
				grid.SetAll(true);
			}
			int[] selectedIndices=grid.SelectedIndices;
			int selectedIndex=-1;
			//Create worker thread to run DBMs. This allows the user to stop running DBM without waiting for all methods to finish
			_threadRunDBM=new ODThread(new ODThread.WorkerDelegate((ODThread o) => {
				for(int i=0;i<selectedIndices.Length;i++) {
					selectedIndex=selectedIndices[i];
					MethodInfo method=(MethodInfo)grid.ListGridRows[selectedIndices[i]].Tag;
					ScrollToBottom(grid,selectedIndices[i]);
					UpdateResultTextForRow(grid,selectedIndices[i],Lan.g("FormDatabaseMaintenance","Running")+"...");
					string result=RunMethod(method,modeCur);
					string status="";
					if(result=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
						status=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
					}
					UpdateResultTextForRow(grid,selectedIndices[i],result+status);
					logText.Append(result);
					//Check flag to see if user wants to stop DBM
					if(_isCancelled) {
						break;
					}
				}
				ToggleUI(false);
				grid.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
			}));
			_threadRunDBM.AddExitHandler((ex) => SaveLogToFile(logText.ToString()));
			_threadRunDBM.AddExceptionHandler(ex => this.InvokeIfRequired(() => {
				FriendlyException.Show("Error during database maintenance.",ex);
				if(selectedIndex>=0) {
					UpdateResultTextForRow(grid,selectedIndex,Lan.g(this,"ERROR: ")+ex.Message);
				}
				logText.Append(Lan.g(this,"ERROR: ")+ex.Message);
				ToggleUI(false);
				grid.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
			}));
			_threadRunDBM.Name="RunDBMThread";
			_threadRunDBM.Start();
			Cursor=Cursors.Default;
		}

		///<summary>Runs a single DBM method.  Updates the DateLastRun column in the database for the method passed in if modeCur is set to Fix.</summary>
		private string RunMethod(MethodInfo method,DbmMode modeCur) {
			List<object> parameters=GetParametersForMethod(method,modeCur);
			return RunMethod(method,parameters,modeCur);
		}

		///<summary>Runs a single DBM method.  Updates the DateLastRun column in the database for the method passed in if modeCur is set to Fix.</summary>
		private string RunMethod(MethodInfo method,List<object> parameters,DbmMode modeCur) {
			string result="";
			try {
				result=(string)method.Invoke(null,parameters.ToArray());
				if(modeCur==DbmMode.Fix) {
					DbmMethodAttr dbmAttribute=(DbmMethodAttr)Attribute.GetCustomAttribute(method,typeof(DbmMethodAttr));
					DatabaseMaintenance databaseMaintenance=_listDatabaseMaintenances.FirstOrDefault(x=>x.MethodName==method.Name);
					if(dbmAttribute!=null && dbmAttribute.IsOneOff && databaseMaintenance!=null && !databaseMaintenance.IsOld) {
						DatabaseMaintenances.MoveToOld(method.Name);
					}
					DatabaseMaintenances.UpdateDateLastRun(method.Name);
				}
			}
			catch(Exception ex) {
				if(ex.InnerException!=null) {
					ExceptionDispatchInfo.Capture(ex.InnerException).Throw();//This preserves the stack trace of the InnerException.
				}
				throw;
			}
			return result;
		}

		///<summary>Runs the DBM method passed in and displays the results in a non-modal MsgBoxCopyPaste window.
		///Does nothing if the DBM method passed in is not flagged for HasBreakDown</summary>
		private void RunMethodBreakDown(MethodInfo method) {
			if(!DatabaseMaintenances.MethodHasBreakDown(method)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			string result=RunMethod(method,DbmMode.Breakdown);
			Cursor=Cursors.Default;
			if(result=="") {
				result=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
			}
			SaveLogToFile(method.Name+":\r\n"+result);
			//Show the result of the dbm method in a simple copy paste msg box.
			MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(result);
			msgBoxCP.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
		}

		///<summary>Returns a list of parameters for the corresponding DBM method.  The order of these parameters is critical.</summary>
		private List<object> GetParametersForMethod(MethodInfo method,DbmMode modeCur) {
			long patNum=0;
			DbmMethodAttr methodAttributes=(DbmMethodAttr)Attribute.GetCustomAttribute(method,typeof(DbmMethodAttr));
			//There are optional paramaters available to some methods and adding them in the following order is very important.
			//We always send verbose and modeCur into all DBM methods first.
			List<object> parameters=new List<object>() { checkShow.Checked,modeCur };
			//Followed by an optional PatNum for patient specific DBM methods.
			if(methodAttributes.HasPatNum) {
				parameters.Add(patNum);
			}
			return parameters;
		}

		///<summary>Tries to log the text passed in to a centralized DBM log.  Displays an error message to the user if anything goes wrong.
		///Always sets the current Cursor state back to Cursors.Default once finished.</summary>
		private void SaveLogToFile(string logText) {
			this.InvokeIfNotDisposed(() => {
				try {
					DatabaseMaintenances.SaveLogToFile(logText);
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
				}
				Cursor=Cursors.Default;
			});
		}

		///<summary>Helper function to make thread-safe UI calls to ODgrid.ScrollToIndexBottom</summary>
		private void ScrollToBottom(GridOD grid,int index) {
			if(this.InvokeRequired) {
				this.Invoke((Action)delegate () { ScrollToBottom(grid,index); });
				return;
			}
			grid.ScrollToIndexBottom(index);
		}

		/// <summary>Updates the result column for the specified row in gridMain with the text passed in.</summary>
		private void UpdateResultTextForRow(GridOD grid,int index,string text) {
			if(this.InvokeRequired) {
				this.Invoke((Action)delegate () { UpdateResultTextForRow(grid,index,text); });
				return;
			}
			int colresults=2;
			if(grid==gridOld && checkShowHidden.Checked) {
				colresults=3;//There is an extra "Hidden" column to account for when setting the "Results" column.
			}
			grid.BeginUpdate();
			grid.ListGridRows[index].Cells[colresults].Text=text;
			grid.EndUpdate();
			Application.DoEvents();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(_dateTimeLastRun==DateTime.MinValue) {
				_dateTimeLastRun=DateTime.Now;
			}
			StringBuilder strB=new StringBuilder();
			strB.Append(_dateTimeLastRun.ToString());
			strB.Append('-',65);
			strB.AppendLine();//New line.
			if(gridMain.SelectedIndices.Length < 1) {
				//No rows are selected so the user wants to run all checks.
				gridMain.SetAll(true);
			}
			int[] selectedIndices=gridMain.SelectedIndices;
			for(int i=0;i<selectedIndices.Length;i++) {
				string resultText=gridMain.ListGridRows[selectedIndices[i]].Cells[2].Text;
				if(!String.IsNullOrEmpty(resultText) && resultText!="Done.  No maintenance needed.") {
					strB.Append(gridMain.ListGridRows[selectedIndices[i]].Cells[0].Text+"\r\n");
					strB.Append("---"+gridMain.ListGridRows[selectedIndices[i]].Cells[2].Text+"\r\n");
					strB.AppendLine();
				}
			}
			strB.AppendLine(Lan.g("FormDatabaseMaintenance","Done"));
			LogTextPrint=strB.ToString();
			PrinterL.TryPrintOrDebugClassicPreview(pd2_PrintPage,Lan.g(this,"Database Maintenance log printed"),new Margins(40,50,50,60),0);
		}

		///<summary>Turn certain UI elements on or off depending on if DBM is running.</summary>
		private void ToggleUI(bool isRunningDbm=false) {
			if(this.InvokeRequired) {
				this.Invoke((Action)delegate () { ToggleUI(isRunningDbm); });
				return;
			}
			if(isRunningDbm) {
				butCheck.Enabled=false;
				butCheckOld.Enabled=false;
				butFix.Enabled=false;
				butFixOld.Enabled=false;
				butPrint.Enabled=false;
				butNone.Enabled=false;
				butClose.Enabled=false;
				butStopDBM.Enabled=true;
				butStopDBMOld.Enabled=true;
				tabTools.Enabled=false;
				checkShow.Enabled=false;
				checkShowHidden.Enabled=false;
				ControlBox=false;//We do NOT want the user to click the X button. They must click the stop button if DBM is running.
			}
			else {
				butCheck.Enabled=true;
				butCheckOld.Enabled=true;
				butFix.Enabled=true;
				butFixOld.Enabled=true;
				butPrint.Enabled=true;
				butNone.Enabled=true;
				butClose.Enabled=true;
				butStopDBM.Enabled=false;
				butStopDBMOld.Enabled=false;
				tabTools.Enabled=true;
				checkShow.Enabled=true;
				checkShowHidden.Enabled=true;
				ControlBox=true;
			}
		}

		private void pd2_PrintPage(object sender,PrintPageEventArgs ev) {//raised for each page to be printed.
			int charsOnPage=0;
			int linesPerPage=0;
			Font font=new Font("Courier New",10);
			ev.Graphics.MeasureString(LogTextPrint,font,ev.MarginBounds.Size,StringFormat.GenericTypographic,out charsOnPage,out linesPerPage);
			ev.Graphics.DrawString(LogTextPrint,font,Brushes.Black,ev.MarginBounds,StringFormat.GenericTypographic);
			LogTextPrint=LogTextPrint.Substring(charsOnPage);
			ev.HasMorePages=(LogTextPrint.Length > 0);
		}
		
		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGridOld();
		}

		private void butCheck_Click(object sender,System.EventArgs e) {
			Run(gridMain,DbmMode.Check);
		}

		private void butCheckOld_Click(object sender,EventArgs e) {
			Run(gridOld,DbmMode.Check);
		}

		private void butFix_Click(object sender,EventArgs e) {
			Fix();
		}

		private void butFixOld_Click(object sender,EventArgs e) {
			Fix(isOld:true);
		}

		private void butFixTools_Click(object sender,EventArgs e) {
			if(gridTools.GetSelectedIndex()==-1) {
				return;
			}
			gridTools.SelectedTag<Action>().Invoke();
		}

		private void butStopDBMOld_Click(object sender,EventArgs e) {
			_isCancelled=true;
			Cursor=Cursors.WaitCursor;
		}

		private void butStopDBM_Click(object sender,EventArgs e) {
			_isCancelled=true;
			Cursor=Cursors.WaitCursor;
		}

		private void Fix(bool isOld=false) {
			List<Computer> runningComps=Computers.GetRunningComputers();
			if(runningComps.Count>50) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"WARNING!\r\nMore than 50 workstations are connected to this database. "
					+"Running DBM may cause severe network slowness. "
					+"We recommend running this tool when fewer users are connected (possibly after working hours). \r\n\r\n"
					+"Continue?")) {
					return;
				}
			}
			if(isOld) {
				Run(gridOld,DbmMode.Fix);
			}
			else {
				Run(gridMain,DbmMode.Fix);
			}
			_isCacheInvalid=true;//Flag cache to be invalidated on closing.  Some DBM fixes alter cached tables.
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		private void FormDatabaseMaintenance_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isCacheInvalid) {
				//Invalidate all cached tables.  DBM could have touched anything so blast them all.
				//Failure to invalidate cache can cause UEs in the main program.
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => DataValid.SetInvalid(Cache.GetAllCachedInvalidTypes().ToArray());
				progressOD.ODEventType=ODEventType.Cache;
				progressOD.ShowDialogProgress();
			}
		}
	}


}
