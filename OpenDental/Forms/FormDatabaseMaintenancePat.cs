using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Text.RegularExpressions;
using OpenDental.UI;
using CodeBase;
using System.Runtime.ExceptionServices;
using System.Linq;

namespace OpenDental {
	public partial class FormDatabaseMaintenancePat:FormODBase {
		///<summary>This list holds MethodInfo for every patient DBM, used in conjunction with _listDatabaseMaintenances.</summary>
		private List<MethodInfo> _listMethodInfosDbm;
		///<summary>List of all DatabaseMaintenance objects used in this form.  Needed to determine IsHidden and IsOld.</summary>
		private List<DatabaseMaintenance> _listDatabaseMaintenances;
		///This is used to populate gridMain.</summary>
		private List<MethodInfo> _listMethodInfosNotHiddenOrOld;
		///This is used to populate gridHidden.</summary>
		private List<MethodInfo> _listMethodInfosHidden;
		///This is used to populate gridOld.</summary>
		private List<MethodInfo> _listMethodInfosOld;
		private long _patNum;
		///<summary>True if the user has been prompted to enter a password, and successfully entered the password for replication DBMs. False otherwise.</summary>
		private bool _isReplicationPasswordEntered=false;
		///<summary>Set on load, true if we detect the office is using replication. Otherwise false.</summary>
		private bool _isUsingReplication=false;
		///<summary>True if the MySQL user has the privileges necessary to determine if the database is using replication (REPLICATION CLIENT and SUPER). Otherwise, false.</summary>
		private bool _hasReplicationPermission=true;

		///<summary>Ensures the column name matches the name for looking up the column index when grids have different column lengths.</summary>
		private readonly string BREAKDOWN_COLUMN_NAME="Break\r\nDown";
		///<summary>Ensures the column name matches the name for looking up the column index when grids have different column lengths.</summary>
		private readonly string RESULTS_COLUMN_NAME="Results";

		public FormDatabaseMaintenancePat(long patNum=0) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
		}

		private void FormDatabaseMaintenancePat_Load(object sender,EventArgs e) {
			//Get all patient specific DBM methods via reflection.
			_listMethodInfosDbm=DatabaseMaintenances.GetMethodsForDisplay(Clinics.ClinicNum,true);
			List<string> listPatDbmMethodNames=_listMethodInfosDbm.Select(x => x.Name).ToList();
			//Add any missing patient specific DBM methods to the database that are not currently present.
			DatabaseMaintenances.InsertMissingDBMs(listPatDbmMethodNames);
			//Get the DatabaseMaintenance objects from the db for all patient specific methods.  Need this for hidden and old grids.
			//Filtering on the method names list removes any non-patient specific DBM methods
			_listDatabaseMaintenances=DatabaseMaintenances.GetAll().FindAll(x => listPatDbmMethodNames.Contains(x.MethodName));
			UpdateTextPatient();
			FillGrid();
			FillGridOld();
			FillGridHidden();
			try {
				_isUsingReplication=PrefC.IsCloudMode || ReplicationServers.IsUsingReplication();
			}
			catch {
				_hasReplicationPermission=false;
			}
		}

		private void FillGrid() {
			_listMethodInfosNotHiddenOrOld=DbmMethodsForGridHelper(isHidden:false,isOld:false);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Name"),300));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,BREAKDOWN_COLUMN_NAME),40,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,RESULTS_COLUMN_NAME),300){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			GridRow row;
			//_listMethodInfosNotHiddenOrOld has already been filled on load with the correct methods to display in the grid.
			for(int i=0;i<_listMethodInfosNotHiddenOrOld.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listMethodInfosNotHiddenOrOld[i].Name);
				row.Cells.Add(DatabaseMaintenances.MethodHasBreakDown(_listMethodInfosNotHiddenOrOld[i]) ? "X" : "");
				row.Cells.Add("");
				row.Tag=_listMethodInfosNotHiddenOrOld[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridOld() {
			_listMethodInfosOld=DbmMethodsForGridHelper(isHidden:false,isOld:true);
			//Add the hidden methods if needed
			if(checkShowHidden.Checked) {
				List<MethodInfo> listMethodInfosHidden=DbmMethodsForGridHelper(isHidden:true,isOld:true);
				_listMethodInfosOld.AddRange(listMethodInfosHidden);
				//Adding hidden methods will cause the list to no longer be ordered by name
				_listMethodInfosOld.Sort(new MethodInfoComparer());
			}
			gridOld.BeginUpdate();
			gridOld.Columns.Clear();
			gridOld.Columns.Add(new GridColumn(Lan.g(this,"Name"),300));
			gridOld.Columns.Add(new GridColumn(Lan.g(this,"Hidden"),45,HorizontalAlignment.Center));
			gridOld.Columns.Add(new GridColumn(Lan.g(this,BREAKDOWN_COLUMN_NAME),40,HorizontalAlignment.Center));
			gridOld.Columns.Add(new GridColumn(Lan.g(this,RESULTS_COLUMN_NAME),300){ IsWidthDynamic=true });
			gridOld.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMethodInfosOld.Count;i++) {
				bool isMethodHidden=_listDatabaseMaintenances.Any(x => x.MethodName==_listMethodInfosOld[i].Name && x.IsHidden);
				row=new GridRow();
				row.Cells.Add(_listMethodInfosOld[i].Name);
				row.Cells.Add(isMethodHidden ? "X" : "");
				row.Cells.Add(DatabaseMaintenances.MethodHasBreakDown(_listMethodInfosOld[i]) ? "X" : "");
				row.Cells.Add("");
				row.Tag=_listMethodInfosOld[i];
				gridOld.ListGridRows.Add(row);
			}
			gridOld.EndUpdate();
		}

		private void FillGridHidden() {
			_listMethodInfosHidden=DbmMethodsForGridHelper(isHidden:true,isOld:false);
			gridHidden.BeginUpdate();
			gridHidden.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Name"),340);
			gridHidden.Columns.Add(col);
			gridHidden.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMethodInfosHidden.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listMethodInfosHidden[i].Name);
				row.Tag=_listMethodInfosHidden[i];
				gridHidden.ListGridRows.Add(row);
			}
			gridHidden.EndUpdate();
		}

		private void Run(GridOD gridCur,DbmMode dbmModeCur) {
			if(_patNum<1) {
				MsgBox.Show(this,"Select a patient first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			//Clear out the result column for all rows before every "run"
			for(int i=0;i<gridCur.ListGridRows.Count;i++) {
				//gridMain and gridOld have a different number of columns, but their matching columns will be named the same.
				int colIndex=gridCur.Columns.GetIndex(RESULTS_COLUMN_NAME);
				gridCur.ListGridRows[i].Cells[colIndex].Text="";//Don't use UpdateResultTextForRow here because users will see the rows clearing out one by one.
			}
			bool isVerbose=checkShow.Checked;
			StringBuilder stringBuilderLogText=new StringBuilder();
			//No longer uses a pre-check for tables.
			if(gridCur.SelectedIndices.Length<1) {
				//No rows are selected so the user wants to run all checks.
				gridCur.SetAll(true);
			}
			string strResult;
			int[] selectedIndices=gridCur.SelectedIndices;
			if(dbmModeCur==DbmMode.Fix && HasReplicationUnsafeMethods(gridCur)){
				VerifyPermissionToRunReplicationUnsafeMethods();
			}
			for(int i=0;i<gridCur.SelectedIndices.Length;i++) {
				MethodInfo methodInfo=(MethodInfo)gridCur.ListGridRows[gridCur.SelectedIndices[i]].Tag;
				DbmMethodAttr dbmMethodAttr=(DbmMethodAttr)Attribute.GetCustomAttribute(methodInfo,typeof(DbmMethodAttr));
				//We always send verbose and modeCur into all DBM methods.
				List<object> listObjectsParameters=new List<object>() { isVerbose,dbmModeCur };
				//There are optional paramaters available to some methods and adding them in the following order is very important.
				if(dbmMethodAttr.HasPatNum) {
					listObjectsParameters.Add(_patNum);
				}
				gridCur.ScrollToIndexBottom(gridCur.SelectedIndices[i]);
				UpdateResultTextForRow(gridCur.SelectedIndices[i],Lan.g("FormDatabaseMaintenance","Running")+"...",gridCur);
				gridCur.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
				strResult=RunMethod(methodInfo,dbmModeCur,listObjectsParameters);
				if(dbmModeCur==DbmMode.Fix) {
					DatabaseMaintenances.UpdateDateLastRun(methodInfo.Name);
				}
				string strStatus="";
				if(strResult=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
					strStatus=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
				}
				UpdateResultTextForRow(gridCur.SelectedIndices[i],strResult+strStatus,gridCur);
				gridCur.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
				stringBuilderLogText.Append(strResult);
			}
			//gridCur.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
			SaveLogToFile(stringBuilderLogText.ToString());
			if(dbmModeCur==DbmMode.Fix) {
				//_isCacheInvalid=true;//Flag cache to be invalidated on closing.  Some DBM fixes alter cached tables.
			}
		}

		private string RunMethod(MethodInfo methodInfo,DbmMode dbmModeCur,List<object> listObjectsParameters) {
			if(dbmModeCur==DbmMode.Fix) {
				DbmMethodAttr dbmMethodAttr=(DbmMethodAttr)Attribute.GetCustomAttribute(methodInfo,typeof(DbmMethodAttr));
				if(dbmMethodAttr!=null && dbmMethodAttr.IsReplicationUnsafe) {
					if(!_hasReplicationPermission){
						return "Replication unsafe method, requires MySQL privilege REPLICATION CLIENT or SUPER";
					}
					if(_isUsingReplication && !_isReplicationPasswordEntered) {
						return "Replication unsafe method, unable to run without authorization.";
					}
				}
			}
			try {
				return (string)(methodInfo.Invoke(null,listObjectsParameters.ToArray()));
			}
			catch(Exception ex) {
				if(ex.InnerException!=null) {
					ExceptionDispatchInfo.Capture(ex.InnerException).Throw();//This preserves the stack trace of the InnerException.
				}
				throw;
			}
		}

		///<summary>Looks at _listDatabaseMaintenances to determine Old and Hidden status and then returns the corresponding MethodInfos from _listDbmMethodInfosDbm.</summary>
		private List<MethodInfo> DbmMethodsForGridHelper(bool isHidden=false,bool isOld=false) {
			List<string> listStrMethods=_listDatabaseMaintenances.FindAll(x => x.IsHidden==isHidden && x.IsOld==isOld)
				.Select(y => y.MethodName)
				.ToList();
			return _listMethodInfosDbm.FindAll(x => listStrMethods.Contains(x.Name));
		}

		private void VerifyPermissionToRunReplicationUnsafeMethods() {
			if(_isReplicationPasswordEntered) {
				return;
			}
			if(!_hasReplicationPermission) {
				MsgBox.Show(this,"Unable to determine if replication is enabled without the MySQL privileges REPLICATION CLIENT or SUPER." +
					" At least one dbm method is not safe to run when replication is enabled. Unsafe methods will be skipped.");
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"At least one dbm method is not safe to run when replication is enabled. Would you like to continue?")) {
				return;
			}
			if(PrefC.IsCloudMode) {
				string strMsg="Replication might be enabled. Running unsafe replication methods may cause database damage. Additional steps must be taken before running unsafe database methods.";
				string strCheckBoxMsg="I have taken the special published process provided by the software support team.";
				InputBoxParam inputBoxParam=new InputBoxParam(InputBoxType.CheckBox,Lan.g(this,strMsg),text:strCheckBoxMsg);
				inputBoxParam.SizeParam=new Size(400,40);
				using InputBox inputBox=new InputBox(null,inputBoxParam);
				inputBox.setTitle("Replication Warning");
				if(inputBox.ShowDialog()!=DialogResult.OK || !inputBox.checkBoxResult.Checked) {
					return;
				}
			}
			else {
				if(!MsgBox.Show(MsgBoxButtons.YesNo,"At least one dbm method is not safe to run when replication is enabled. Would you like to continue?")) {
					return;
				}
				using InputBox inputBox=new InputBox("Please enter password");
				inputBox.setTitle("Run Replication Unsafe DBMs");
				inputBox.textResult.PasswordChar='*';
				inputBox.ShowDialog();
				if(inputBox.DialogResult!=DialogResult.OK) {
					return;
				}
				if(inputBox.textResult.Text!="abracadabra") {
					MsgBox.Show(this,"Wrong password");
					return;
				}
			}
			_isReplicationPasswordEntered=true;
		}

		///<summary>Returns true if any of the selected gridrows have a method tag that is marked as IsReplicationUnsafe and the office is running replication. Otherwise false.</summary>
		private bool HasReplicationUnsafeMethods(GridOD grid) {
			List<MethodInfo> listGridRowTags=grid.SelectedTags<MethodInfo>();
			for(int i=0;i<listGridRowTags.Count;i++) {
				DbmMethodAttr dbmMethodAttr=(DbmMethodAttr)Attribute.GetCustomAttribute(listGridRowTags[i],typeof(DbmMethodAttr));
				if(dbmMethodAttr!=null && dbmMethodAttr.IsReplicationUnsafe && (_isUsingReplication || !_hasReplicationPermission)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Updates the result column for the specified row in the grid with the text passed in.</summary>
		private void UpdateResultTextForRow(int index,string text,GridOD gridCur) {
			int breakdownIndex=gridCur.Columns.GetIndex(BREAKDOWN_COLUMN_NAME);
			int resultsIndex=gridCur.Columns.GetIndex(RESULTS_COLUMN_NAME);
			gridCur.BeginUpdate();
			//Checks to see if it has a breakdown, and if it needs any maintenance to decide whether or not to apply the "X"
			if(!DatabaseMaintenances.MethodHasBreakDown((MethodInfo)gridCur.ListGridRows[index].Tag) || text=="Done.  No maintenance needed.") {
				gridCur.ListGridRows[index].Cells[breakdownIndex].Text="";
			}
			else {
				gridCur.ListGridRows[index].Cells[breakdownIndex].Text="X";
			}
			gridCur.ListGridRows[index].Cells[resultsIndex].Text=text;
			gridCur.EndUpdate();
			Application.DoEvents();
		}

		private void UpdateTextPatient() {
			//For whatever reason GetLim() never returns null.
			textPatient.Text=Patients.GetLim(_patNum).GetNameFL();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			MethodInfo methodInfo=_listMethodInfosNotHiddenOrOld[e.Row];
			if(!DatabaseMaintenances.MethodHasBreakDown(methodInfo)) {
				return;
			}
			//We know that this method supports giving the user a break down and shall call the method's fix section where the break down results should be.
			//TODO: Make sure that DBM methods with break downs ALWAYS have the break down in the fix section.
			if(_patNum<1) {
				MsgBox.Show(this,"Select a patient first.");
				return;
			}
			DbmMethodAttr dbmMethodAttr=(DbmMethodAttr)Attribute.GetCustomAttribute(methodInfo,typeof(DbmMethodAttr));
			//We always send verbose and modeCur into all DBM methods.
			List<object> listObjParameters=new List<object>() { checkShow.Checked,DbmMode.Breakdown };
			//There are optional paramaters available to some methods and adding them in the following order is very important.
			if(dbmMethodAttr.HasPatNum) {
				listObjParameters.Add(_patNum);
			}
			Cursor=Cursors.WaitCursor;
			string strResult=(string)methodInfo.Invoke(null,listObjParameters.ToArray());
			if(strResult=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
				strResult=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
			}
			SaveLogToFile(methodInfo.Name+":\r\n"+strResult);
			//Show the result of the dbm method in a simple copy paste msg box.
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(strResult);
			Cursor=Cursors.Default;
			msgBoxCopyPaste.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
		}

		///<summary>Tries to log the text passed in to a centralized DBM log.  Displays an error message to the user if anything goes wrong.
		///Always sets the current Cursor state back to Cursors.Default once finished.</summary>
		private void SaveLogToFile(string logText) {
			try {
				DatabaseMaintenances.SaveLogToFile(logText);
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
			}
			Cursor=Cursors.Default;
		}

		private void butPatientSelect_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_patNum=formPatientSelect.PatNumSelected;
			UpdateTextPatient();
		}

		private void butNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
		}

		private void butCheck_Click(object sender,EventArgs e) {
			Run(gridMain,DbmMode.Check);
		}

		private void butFix_Click(object sender,EventArgs e) {
			Run(gridMain,DbmMode.Fix);
		}

		private void butCheckOld_Click(object sender,EventArgs e) {
			Run(gridOld,DbmMode.Check);
		}

		private void butFixOld_Click(object sender,EventArgs e) {
			Run(gridOld,DbmMode.Fix);
		}

		private void butNoneOld_Click(object sender,EventArgs e) {
			gridOld.SetAll(false);
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGridOld();
		}

		private void gridOld_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!DatabaseMaintenances.MethodHasBreakDown(_listMethodInfosOld[e.Row])) {
				return;
			}
			//We know that this method supports giving the user a break down and shall call the method's fix section where the break down results should be.
			if(_patNum<1) {
				MsgBox.Show(this,"Select a patient first.");
				return;
			}
			DbmMethodAttr dbmMethodAttr=(DbmMethodAttr)Attribute.GetCustomAttribute(_listMethodInfosOld[e.Row],typeof(DbmMethodAttr));
			//We always send verbose and modeCur into all DBM methods.
			List<object> listObjectsParameters=new List<object>() { checkShow.Checked,DbmMode.Breakdown };
			//There are optional paramaters available to some methods and adding them in the following order is very important.
			if(dbmMethodAttr.HasPatNum) {
				listObjectsParameters.Add(_patNum);
			}
			Cursor=Cursors.WaitCursor;
			string strResult=(string)_listMethodInfosOld[e.Row].Invoke(null,listObjectsParameters.ToArray());
			if(strResult=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
				strResult=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
			}
			//Show the result of the dbm method in a simple copy paste msg box.
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(strResult);
			Cursor=Cursors.Default;
			msgBoxCopyPaste.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormDatabaseMaintenancePat_FormClosing(object sender,FormClosingEventArgs e) {
			//if(_isCacheInvalid) {
			//	Action actionCloseDBM=ODProgress.ShowProgressStatus("DatabaseMaintEvent",this,Lan.g(this,"Refreshing all caches, this can take a while..."));
			//	//Invalidate all cached tables.  DBM could have touched anything so blast them all.  
			//	//Failure to invalidate cache can cause UEs in the main program.
			//	DataValid.SetInvalid(Cache.GetAllCachedInvalidTypes().ToArray());
			//	actionCloseDBM?.Invoke();
			//}
		}
	}
}