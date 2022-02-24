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
		private List<MethodInfo> _listDbmMethodInfos;
		///<summary>List of all DatabaseMaintenance objects used in this form.  Needed to determine IsHidden and IsOld.</summary>
		private List<DatabaseMaintenance> _listDatabaseMaintenances;
		///This is used to populate gridMain.</summary>
		private List<MethodInfo> _listDbmMethodsGrid;
		///This is used to populate gridHidden.</summary>
		private List<MethodInfo> _listDbmMethodsGridHidden;
		///This is used to populate gridOld.</summary>
		private List<MethodInfo> _listDbmMethodsGridOld;
		private long _patNum;

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
			_listDbmMethodInfos=DatabaseMaintenances.GetMethodsForDisplay(Clinics.ClinicNum,true);
			List<string> listPatDbmMethodNames=_listDbmMethodInfos.Select(x => x.Name).ToList();
			//Add any missing patient specific DBM methods to the database that are not currently present.
			DatabaseMaintenances.InsertMissingDBMs(listPatDbmMethodNames);
			//Get the DatabaseMaintenance objects from the db for all patient specific methods.  Need this for hidden and old grids.
			//Filtering on the method names list removes any non-patient specific DBM methods
			_listDatabaseMaintenances=DatabaseMaintenances.GetAll().FindAll(x => ListTools.In(x.MethodName,listPatDbmMethodNames));
			UpdateTextPatient();
			FillGrid();
			FillGridOld();
			FillGridHidden();
		}

		private void FillGrid() {
			_listDbmMethodsGrid=DbmMethodsForGridHelper(isHidden: false,isOld: false);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),300));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,BREAKDOWN_COLUMN_NAME),40,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,RESULTS_COLUMN_NAME),300){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			GridRow row;
			//_listDbmMethodsGrid has already been filled on load with the correct methods to display in the grid.
			foreach(MethodInfo meth in _listDbmMethodsGrid) {
				row=new GridRow();
				row.Cells.Add(meth.Name);
				row.Cells.Add(DatabaseMaintenances.MethodHasBreakDown(meth) ? "X" : "");
				row.Cells.Add("");
				row.Tag=meth;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridOld() {
			_listDbmMethodsGridOld=DbmMethodsForGridHelper(isHidden: false,isOld: true);
			//Add the hidden methods if needed
			if(checkShowHidden.Checked) {
				_listDbmMethodsGridOld.AddRange(DbmMethodsForGridHelper(isHidden: true,isOld: true));
				//Adding hidden methods will cause the list to no longer be ordered by name
				_listDbmMethodsGridOld.Sort(new MethodInfoComparer());
			}
			gridOld.BeginUpdate();
			gridOld.ListGridColumns.Clear();
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),300));
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,"Hidden"),45,HorizontalAlignment.Center));
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,BREAKDOWN_COLUMN_NAME),40,HorizontalAlignment.Center));
			gridOld.ListGridColumns.Add(new GridColumn(Lan.g(this,RESULTS_COLUMN_NAME),300){ IsWidthDynamic=true });
			gridOld.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDbmMethodsGridOld.Count;i++) {
				bool isMethodHidden=_listDatabaseMaintenances.Any(x => x.MethodName==_listDbmMethodsGridOld[i].Name && x.IsHidden);
				row=new GridRow();
				row.Cells.Add(_listDbmMethodsGridOld[i].Name);
				row.Cells.Add(isMethodHidden ? "X" : "");
				row.Cells.Add(DatabaseMaintenances.MethodHasBreakDown(_listDbmMethodsGridOld[i]) ? "X" : "");
				row.Cells.Add("");
				row.Tag=_listDbmMethodsGridOld[i];
				gridOld.ListGridRows.Add(row);
			}
			gridOld.EndUpdate();
		}

		private void FillGridHidden() {
			_listDbmMethodsGridHidden=DbmMethodsForGridHelper(isHidden: true,isOld: false);
			gridHidden.BeginUpdate();
			gridHidden.ListGridColumns.Clear();
			gridHidden.ListGridColumns.Add(new GridColumn(Lan.g(this,"Name"),340));
			gridHidden.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDbmMethodsGridHidden.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listDbmMethodsGridHidden[i].Name);
				row.Tag=_listDbmMethodsGridHidden[i];
				gridHidden.ListGridRows.Add(row);
			}
			gridHidden.EndUpdate();
		}

		private void Run(GridOD gridCur,DbmMode modeCur) {
			if(_patNum<1) {
				MsgBox.Show(this,"Select a patient first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			//Clear out the result column for all rows before every "run"
			for(int i=0;i<gridCur.ListGridRows.Count;i++) {
				//gridMain and gridOld have a different number of columns, but their matching columns will be named the same.
				gridCur.ListGridRows[i].Cells[gridCur.ListGridColumns.GetIndex(RESULTS_COLUMN_NAME)].Text="";//Don't use UpdateResultTextForRow here because users will see the rows clearing out one by one.
			}
			bool verbose=checkShow.Checked;
			StringBuilder logText=new StringBuilder();
			//No longer uses a pre-check for tables.
			if(gridCur.SelectedIndices.Length<1) {
				//No rows are selected so the user wants to run all checks.
				gridCur.SetAll(true);
			}
			string result;
			int[] selectedIndices=gridCur.SelectedIndices;
			foreach(int i in gridCur.SelectedIndices) {
				DbmMethodAttr methodAttributes=(DbmMethodAttr)Attribute.GetCustomAttribute((MethodInfo)gridCur.ListGridRows[i].Tag,typeof(DbmMethodAttr));
				//We always send verbose and modeCur into all DBM methods.
				List<object> parameters=new List<object>() { verbose,modeCur };
				//There are optional paramaters available to some methods and adding them in the following order is very important.
				if(methodAttributes.HasPatNum) {
					parameters.Add(_patNum);
				}
				
				try {
					gridCur.ScrollToIndexBottom(i);
					UpdateResultTextForRow(i,Lan.g("FormDatabaseMaintenance","Running")+"...",gridCur);
					gridCur.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
					result=(string)((MethodInfo)gridCur.ListGridRows[i].Tag).Invoke(null,parameters.ToArray());
					if(modeCur==DbmMode.Fix) {
						DatabaseMaintenances.UpdateDateLastRun(((MethodInfo)gridCur.ListGridRows[i].Tag).Name);
					}
				}
				catch(Exception ex) {
					if(ex.InnerException!=null) {
						ExceptionDispatchInfo.Capture(ex.InnerException).Throw();//This preserves the stack trace of the InnerException.
					}
					throw;
				}
				string status="";
				if(result=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
					status=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
				}
				UpdateResultTextForRow(i,result+status,gridCur);
				logText.Append(result);
			}
			gridCur.SetSelected(selectedIndices,true);//Reselect all rows that were originally selected.
			SaveLogToFile(logText.ToString());
			if(modeCur==DbmMode.Fix) {
				//_isCacheInvalid=true;//Flag cache to be invalidated on closing.  Some DBM fixes alter cached tables.
			}
		}
		///<summary>Looks at _listDatabaseMaintenances to determine Old and Hidden status and then returns the corresponding MethodInfos from _listDbmMethodInfos.</summary>
		private List<MethodInfo> DbmMethodsForGridHelper(bool isHidden=false,bool isOld=false) {
			List<string> listMethods=_listDatabaseMaintenances.FindAll(x => x.IsHidden==isHidden && x.IsOld==isOld)
				.Select(y => y.MethodName).ToList();
			return _listDbmMethodInfos.FindAll(x => ListTools.In(x.Name,listMethods));
		}

		///<summary>Updates the result column for the specified row in the grid with the text passed in.</summary>
		private void UpdateResultTextForRow(int index,string text,GridOD gridCur) {
			int breakdownIndex=gridCur.ListGridColumns.GetIndex(BREAKDOWN_COLUMN_NAME);
			int resultsIndex=gridCur.ListGridColumns.GetIndex(RESULTS_COLUMN_NAME);
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
			MethodInfo method=_listDbmMethodsGrid[e.Row];
			if(!DatabaseMaintenances.MethodHasBreakDown(method)) {
				return;
			}
			//We know that this method supports giving the user a break down and shall call the method's fix section where the break down results should be.
			//TODO: Make sure that DBM methods with break downs ALWAYS have the break down in the fix section.
			if(_patNum<1) {
				MsgBox.Show(this,"Select a patient first.");
				return;
			}
			DbmMethodAttr methodAttributes=(DbmMethodAttr)Attribute.GetCustomAttribute(method,typeof(DbmMethodAttr));
			//We always send verbose and modeCur into all DBM methods.
			List<object> parameters=new List<object>() { checkShow.Checked,DbmMode.Breakdown };
			//There are optional paramaters available to some methods and adding them in the following order is very important.
			if(methodAttributes.HasPatNum) {
				parameters.Add(_patNum);
			}
			Cursor=Cursors.WaitCursor;
			string result=(string)method.Invoke(null,parameters.ToArray());
			if(result=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
				result=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
			}
			SaveLogToFile(method.Name+":\r\n"+result);
			//Show the result of the dbm method in a simple copy paste msg box.
			MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(result);
			Cursor=Cursors.Default;
			msgBoxCP.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
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
			using FormPatientSelect FormPs=new FormPatientSelect();
			FormPs.SelectionModeOnly=true;
			FormPs.ShowDialog();
			if(FormPs.DialogResult!=DialogResult.OK) {
				return;
			}
			_patNum=FormPs.SelectedPatNum;
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
			if(!DatabaseMaintenances.MethodHasBreakDown(_listDbmMethodsGridOld[e.Row])) {
				return;
			}
			//We know that this method supports giving the user a break down and shall call the method's fix section where the break down results should be.
			if(_patNum<1) {
				MsgBox.Show(this,"Select a patient first.");
				return;
			}
			DbmMethodAttr methodAttributes=(DbmMethodAttr)Attribute.GetCustomAttribute(_listDbmMethodsGridOld[e.Row],typeof(DbmMethodAttr));
			//We always send verbose and modeCur into all DBM methods.
			List<object> parameters=new List<object>() { checkShow.Checked,DbmMode.Breakdown };
			//There are optional paramaters available to some methods and adding them in the following order is very important.
			if(methodAttributes.HasPatNum) {
				parameters.Add(_patNum);
			}
			Cursor=Cursors.WaitCursor;
			string result=(string)_listDbmMethodsGridOld[e.Row].Invoke(null,parameters.ToArray());
			if(result=="") {//Only possible if running a check / fix in non-verbose mode and nothing happened or needs to happen.
				result=Lan.g("FormDatabaseMaintenance","Done.  No maintenance needed.");
			}
			//Show the result of the dbm method in a simple copy paste msg box.
			MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(result);
			Cursor=Cursors.Default;
			msgBoxCP.Show();//Let this window be non-modal so that they can keep it open while they fix their problems.
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