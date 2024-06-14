using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormSchedule:FormODBase {
		private DateTime _dateCopyStart;
		private DateTime _dateCopyEnd;
		///<summary>This tracks whether the provList or empList has been click on since the last refresh.  
		///Forces user to refresh before deleting or pasting so that the list exactly matches the grid.</summary>
		private bool _hasProvsChanged;
		private bool _isHeadPrinted;
		private int _pagesPrinted;
		private int _heightHeadingPrint;
		private bool _changed;
		private List<Provider> _listProviders;
		private List<Employee> _listEmployees;
		private DataTable _tableScheds;
		private bool _isResizing;
		private Point _pointClickedCell;
		///<summary>By default is FormScheduleMode.SetupSchedule.
		///If user does not have Schedule permission then will be in FormScheduleMode.ViewSchedule.</summary>
		private FormScheduleMode _formScheduleMode;
		private List<long> _listEmpNumsPreSelected;
		private List<long> _listProvNumsPreSelected;
		///<summary>Keeps track of the last time the "From Date" was set via the fill grid.
		///Used to determine if the user has changed the date since last fill grid.</summary>
		private DateTime _dateFromDate;
		///<summary>Keeps track of the last time the "To Date" was set via the fill grid.
		///Used to determine if the user has changed the date since last fill grid.</summary>
		private DateTime _dateToDate;
		private ScheduleWeekendFilter _scheduleWeekendFilter=ScheduleWeekendFilter.WorkWeek;

		///<summary></summary>
		public FormSchedule(List<long> listPreSelectedEmpNums=null,List<long> listPreSelectedProvNums=null)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listEmpNumsPreSelected=listPreSelectedEmpNums;
			_listProvNumsPreSelected=listPreSelectedProvNums;
		}

		private void FormSchedule_Load(object sender,EventArgs e) {
			radioButtonWorkWeek.Checked=true;
			_formScheduleMode=FormScheduleMode.ViewSchedule;
			if(Security.IsAuthorized(EnumPermType.Schedules,DateTime.MinValue,true)){
				_formScheduleMode=FormScheduleMode.SetupSchedule;
			};
			switch(_formScheduleMode) {
				case FormScheduleMode.SetupSchedule:
					DateTime dateFrom=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
					textDateFrom.Text=dateFrom.ToShortDateString();
					textDateTo.Text=dateFrom.AddMonths(12).AddDays(-1).ToShortDateString();
					break;
				case FormScheduleMode.ViewSchedule:
					butClearWeek.Visible=false;
					groupCopy.Visible=false;
					groupPaste.Visible=false;
					if(PrefC.GetBool(PrefName.DistributorKey)) {//if this is OD HQ
						checkPracticeNotes.Checked=false;
						checkPracticeNotes.Enabled=false;
					}
					dateFrom=DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);//Sunday of current week.
					textDateFrom.Text=dateFrom.ToShortDateString();
					textDateTo.Text=dateFrom.AddMonths(1).AddDays(-1).ToShortDateString();
					break;
			}
			RefreshClinicData();
			FillEmployeesAndProviders();
			FillGrid();
		}

		private void RefreshClinicData() {
			if(!PrefC.HasClinicsEnabled) {
				checkShowClinicSchedules.Visible=false;
				checkClinicNotes.Visible=false;
				checkClinicNotes.Checked=false;
				return;
			}
		}

		///<summary>Fills the employee box based on what clinic is selected.  Set selectAll to true to have all employees in the list box selected by default.</summary>
		private void FillEmployeesAndProviders() {
			tabPageEmp.Text=Lan.g(this,"Employees")+" (0)";
			tabPageProv.Text=Lan.g(this,"Providers")+" (0)";
			//Seed emp list and prov list with a dummy emp/prov with 'none' for the field that fills the list, FName and Abbr respectively.
			//That way we don't have to add/subtract one in order when selecting from the list based on selected indexes.
			_listEmployees=new List<Employee>() { new Employee() { EmployeeNum=0,FName="none" } };
			_listProviders=new List<Provider>() { new Provider() { ProvNum=0,Abbr="none" } };
			if(PrefC.HasClinicsEnabled) {
				//clinicNum will be 0 for unrestricted users with HQ selected in which case this will get only emps/provs not assigned to a clinic
				_listEmployees.AddRange(Employees.GetEmpsForClinic(comboClinic.ClinicNumSelected));
				_listProviders.AddRange(Providers.GetProvsForClinic(comboClinic.ClinicNumSelected));
			}
			else {//Not using clinics
				_listEmployees.AddRange(Employees.GetDeepCopy(true));
				_listProviders.AddRange(Providers.GetDeepCopy(true));
			}
			List<long> listEmpNumsPreviouslySelected=listBoxEmps.GetListSelected<Employee>().Select(x => x.EmployeeNum).ToList();
			listBoxEmps.Items.Clear();
			_listEmployees.ForEach(x => listBoxEmps.Items.Add(x.FName,x));
			List<long> listProvNumsPreviouslySelected=listBoxProvs.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			listBoxProvs.Items.Clear();
			_listProviders.ForEach(x => listBoxProvs.Items.Add(x.Abbr,x));
			if(_listEmpNumsPreSelected!=null || _listProvNumsPreSelected!=null) {
				if(_listEmpNumsPreSelected!=null && _listEmpNumsPreSelected.Count>0) {
					//Employee Listbox
					for(int i=1;i<listBoxEmps.Items.Count;i++) {
						if(!_listEmpNumsPreSelected.Contains(_listEmployees[i].EmployeeNum)) {
							continue;
						}
						listBoxEmps.SetSelected(i,true);
					}
				}
				else {
					listBoxEmps.SelectedIndex=0;//select the 'none' entry;
				}
				if(_listProvNumsPreSelected!=null && _listProvNumsPreSelected.Count>0) {
					//Provider Listbox
					for(int i=1;i<listBoxProvs.Items.Count;i++) {
						if(!_listProvNumsPreSelected.Contains(_listProviders[i].ProvNum)) {
							continue;
						}
						listBoxProvs.SetSelected(i,true);
					}
				}
				else {
					listBoxProvs.SelectedIndex=0;//select the 'none' entry; 
				}
			}
			else if(PrefC.GetBool(PrefName.ScheduleProvEmpSelectAll)) {
				//Employee Listbox
				for(int i=1;i<listBoxEmps.Items.Count;i++) {
					listBoxEmps.SetSelected(i,true);
				}
				//Provider Listbox
				for(int i=1;i<listBoxProvs.Items.Count;i++) {
					listBoxProvs.SetSelected(i,true);
				}
			}
			else {
				if(listEmpNumsPreviouslySelected.Count > 0) {
					for(int i=0; i<listBoxEmps.Items.Count; i++) {
						if(listEmpNumsPreviouslySelected.Contains(((Employee)listBoxEmps.Items.GetObjectAt(i)).EmployeeNum)) {
							listBoxEmps.SetSelected(i,true);
						}
					}
				}
				else {
					listBoxEmps.SelectedIndex=0;//select the 'none' entry;
				}
				if(listProvNumsPreviouslySelected.Count > 0) {
					for(int i=0; i<listBoxProvs.Items.Count; i++) {
						if(listProvNumsPreviouslySelected.Contains(((Provider)listBoxProvs.Items.GetObjectAt(i)).ProvNum)) {
							listBoxProvs.SetSelected(i,true);
						}
					}
				}
				else {
					listBoxProvs.SelectedIndex=0;//select the 'none' entry; 
				}
			}
		}

		///<summary>Returns true if date text boxes have no errors and the emp and prov lists don't have 'none' selected with other emps/provs.
		///Set isQuiet to true to suppress the message box with the warning.</summary>
		private bool ValidateInputs(bool isQuiet=false) {
			List<string> listErrorMsgs=new List<string>();
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				listErrorMsgs.Add(Lan.g(this,"Please fix date errors first."));
			}
			if(listBoxProvs.SelectedIndices.Count>1 && listBoxProvs.SelectedIndices.Contains(0)) {//'none' selected with additional provs
				listErrorMsgs.Add(Lan.g(this,"Invalid selection of providers."));
			}
			if(listBoxEmps.SelectedIndices.Count>1 && listBoxEmps.SelectedIndices.Contains(0)) {//'none' selected with additional emps
				listErrorMsgs.Add(Lan.g(this,"Invalid selection of employees."));
			}
			if(listErrorMsgs.Count > 0 && !isQuiet) {
				MessageBox.Show(string.Join("\r\n",listErrorMsgs));
			}
			return (listErrorMsgs.Count==0);
		}

		private void FillGrid(bool doRefreshData=true){
			_dateCopyStart=DateTime.MinValue;
			_dateCopyEnd=DateTime.MinValue;
			textClipboard.Text="";
			if(!ValidateInputs(true)) {
				return;
			}
			//Clear out the columns and rows for dynamic resizing of the grid to calculate column widths
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.ListGridRows.Clear();
			gridMain.EndUpdate();
			_hasProvsChanged=false;
			List<long> listProvNums=new List<long>();
			for(int i=0;i<listBoxProvs.SelectedIndices.Count;i++){
				listProvNums.Add(_listProviders[listBoxProvs.SelectedIndices[i]].ProvNum);
			}
			List<long> listEmpNums=new List<long>();
			for(int i=0;i<listBoxEmps.SelectedIndices.Count;i++){
				listEmpNums.Add(_listEmployees[listBoxEmps.SelectedIndices[i]].EmployeeNum);
			}
			listProvNums.RemoveAll(x => x==0);
			listEmpNums.RemoveAll(x => x==0);
			if(doRefreshData || this._tableScheds==null) {
				bool canViewNotes=true;
				if(PrefC.IsODHQ) {
					canViewNotes=Security.IsAuthorized(EnumPermType.Schedules,true);
				}
				_dateFromDate=PIn.Date(textDateFrom.Text);
				_dateToDate=PIn.Date(textDateTo.Text);
				Logger.LogToPath("Schedules.GetPeriod",LogPath.Signals,LogPhase.Start);
				_tableScheds=Schedules.GetPeriod(_dateFromDate,_dateToDate,listProvNums,listEmpNums,checkPracticeNotes.Checked,
					checkClinicNotes.Checked,comboClinic.ClinicNumSelected,checkShowClinicSchedules.Checked,canViewNotes);
				Logger.LogToPath("Schedules.GetPeriod",LogPath.Signals,LogPhase.End);
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend || _scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Sunday"),50){ IsWidthDynamic=true });
			}
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek || _scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Monday"),50){ IsWidthDynamic=true });
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Tuesday"),50){ IsWidthDynamic=true });
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Wednesday"),50){ IsWidthDynamic=true });
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Thursday"),50){ IsWidthDynamic=true });
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Friday"),50){ IsWidthDynamic=true });
			}
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend || _scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek){
				gridMain.Columns.Add(new GridColumn(Lan.g("TableSchedule","Saturday"),50){ IsWidthDynamic=true });
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableScheds.Rows.Count;i++){
				row=new GridRow();
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend || _scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek){
					row.Cells.Add(_tableScheds.Rows[i][0].ToString());
				}
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek || _scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek) {
					row.Cells.Add(_tableScheds.Rows[i][1].ToString());
					row.Cells.Add(_tableScheds.Rows[i][2].ToString());
					row.Cells.Add(_tableScheds.Rows[i][3].ToString());
					row.Cells.Add(_tableScheds.Rows[i][4].ToString());
					row.Cells.Add(_tableScheds.Rows[i][5].ToString());
				}
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend || _scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek) {
					row.Cells.Add(_tableScheds.Rows[i][6].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//Set today red
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek 
				&& (DateTime.Today.DayOfWeek==DayOfWeek.Sunday || DateTime.Today.DayOfWeek==DayOfWeek.Saturday))
			{
				return;
			}
			if(DateTime.Today>_dateToDate || DateTime.Today<_dateFromDate){
				return;
			}
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend) {
				return;//don't highlight if we are on the weekend.
			}
			int colI=(int)DateTime.Today.DayOfWeek;
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek){
				colI--;
			}
			gridMain.ListGridRows[Schedules.GetRowCal(_dateFromDate,DateTime.Today)].Cells[colI].ColorText=Color.Red;
			if(_pointClickedCell!=null //when first opening form
				&& _pointClickedCell.Y>-1 
				&& _pointClickedCell.Y< gridMain.ListGridRows.Count
				&& _pointClickedCell.X>-1
				&& _pointClickedCell.X<gridMain.Columns.Count) 
			{
				gridMain.SetSelected(_pointClickedCell);
			}
			//scroll to cell to keep it in view when editing schedules.
			if(gridMain.SelectedCell.X>-1 && gridMain.SelectedCell.Y>-1) {
				gridMain.ScrollToIndex(gridMain.SelectedCell.Y);
			}
		}

		private void checkShowClinicSchedules_CheckedChanged(object sender,EventArgs e) {
			if(checkShowClinicSchedules.Checked) {
				SelectAllProvsAndEmps();
				butClearWeek.Enabled=false;
				butCopyDay.Enabled=false;
				butCopyWeek.Enabled=false;
				butPaste.Enabled=false;
				butRepeat.Enabled=false;
			}
			else {
				butClearWeek.Enabled=true;
				butCopyDay.Enabled=true;
				butCopyWeek.Enabled=true;
				butPaste.Enabled=true;
				butRepeat.Enabled=true;
			}
			if(!ValidateInputs()) {
				return;
			}
			_pointClickedCell=gridMain.SelectedCell;
			FillGrid();
		}

		private void SelectAllProvsAndEmps() {
			listBoxProvs.ClearSelected();
			for(int i=1;i<listBoxProvs.Items.Count;i++) {//i=1 to skip the none
				listBoxProvs.SetSelected(i,true);
			}
			listBoxEmps.ClearSelected();
			for(int i=1;i<listBoxEmps.Items.Count;i++) {//i=1 to skip the none
				listBoxEmps.SetSelected(i,true);
			}
		}

		///<summary>Helper method because this exact code happens several times in this form.  Returns selected providers, employees, and clinic to the variables supplied.</summary>
		private void GetSelectedProvidersEmployeesAndClinic(out List<long> listProvNums,out List<long> listEmployeeNums) {
			listProvNums=new List<long>();
			//Don't populate listProvNums if 'none' is selected; not allowed to select 'none' and another prov validated above.
			if(!listBoxProvs.SelectedIndices.Contains(0)) {
				listProvNums=listBoxProvs.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			}
			listEmployeeNums=new List<long>();
			//Don't populate listEmployeeNums if 'none' is selected; not allowed to select 'none' and another emp validated above.
			if(!listBoxEmps.SelectedIndices.Contains(0)) {
				listEmployeeNums=listBoxEmps.GetListSelected<Employee>().Select(x => x.EmployeeNum).ToList();
			}
		}

		private void listProv_SelectedIndexChanged(object sender,EventArgs e) {
			tabPageProv.Text=Lan.g(this,"Providers")+" ("+listBoxProvs.SelectedIndices.OfType<int>().Count(x => x>0)+")";
		}

		private void listEmp_SelectedIndexChanged(object sender,EventArgs e) {
			tabPageEmp.Text=Lan.g(this,"Employees")+" ("+listBoxEmps.SelectedIndices.OfType<int>().Count(x => x>0)+")";
		}

		///<summary>Double click on listBoxProvs or listBoxEmps triggers a refresh</summary>
		private void listBox_DoubleClick(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			//this code was doing nothing. Not sure what the intent was:
			//comboClinic.Text=Lan.g(this,"Show Practice Notes");
			//if(comboClinic.ClinicNumSelected>0) {
			//	comboClinic.Text=Lan.g(this,"Show Practice and Clinic Notes");
			//}
			FillEmployeesAndProviders();
			if(checkShowClinicSchedules.Checked) {
				SelectAllProvsAndEmps();
			}
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(!ValidateInputs()) {
				return;
			}
			_pointClickedCell=gridMain.SelectedCell;
			FillGrid();
		}

		private void checkPracticeNotes_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkClinicNotes_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			_pointClickedCell=gridMain.SelectedCell;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Schedules,DateTime.MinValue)) {
				return;
			}
			if(!ValidateInputs()) {
				return;
			}
			if(checkShowClinicSchedules.Checked) {
				MsgBox.Show(this,"Schedules cannot be edited in clinic view mode");
				return;
			}
			int clickedCol=e.Col;
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek) {
				clickedCol++;
			}
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend && clickedCol==1) {
				clickedCol=6;//used to calculate correct day to edit.
			}
			//the "clickedCell" is in terms of the entire 7 col layout.
			DateTime dateSelected=Schedules.GetDateCal(_dateFromDate,e.Row,clickedCol);
			if(dateSelected<_dateFromDate || dateSelected>_dateToDate){
				return;
			}
			//MessageBox.Show(selectedDate.ToShortDateString());
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.ClinicNumSelected==-1) {
					MsgBox.Show(this,"Please select a clinic.");
					return;
				}
			}
			string provAbbr="";
			string empFName="";
			//Get all of the selected providers and employees (removing the "none" options).
			List<Provider> listProvidersSelected=listBoxProvs.GetListSelected<Provider>().FindAll(x => x.ProvNum > 0);
			List<Employee> listEmployeesSelected=listBoxEmps.GetListSelected<Employee>().FindAll(x => x.EmployeeNum > 0);
			if(listProvidersSelected.Count==1 && listEmployeesSelected.Count==0) {//only 1 provider selected, pass into schedule day filter
				provAbbr=listProvidersSelected[0].Abbr;
			}
			else if(listEmployeesSelected.Count==1 && listProvidersSelected.Count==0) {//only 1 employee selected, pass into schedule day filter
				empFName=listEmployeesSelected[0].FName;
			}
			else if(listProvidersSelected.Count==1 && listEmployeesSelected.Count==1) {//1 provider and 1 employee selected
				//see if the names match, if we're dealing with the same person it's okay to pass both in, if not then don't pass in either. 
				if(listProvidersSelected[0].FName==listEmployeesSelected[0].FName 
					&& listProvidersSelected[0].LName==listEmployeesSelected[0].LName) 
				{
					provAbbr=listProvidersSelected[0].Abbr;
					empFName=listEmployeesSelected[0].FName;
				}
			}
			using FormScheduleDayEdit formScheduleDayEdit=new FormScheduleDayEdit(dateSelected,comboClinic.ClinicNumSelected,provAbbr,empFName,true);
			formScheduleDayEdit.ShowDialog();
			if(formScheduleDayEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
			_changed=true;
		}

		private void listProv_Click(object sender,EventArgs e) {
			_hasProvsChanged=true;
		}

		private void listEmp_Click(object sender,EventArgs e) {
			_hasProvsChanged=true;
		}

		private void radioButtonWorkWeek_CheckedChanged(object sender,EventArgs e) {
			if(radioButtonWorkWeek.Checked) { 
				SetSelected(ScheduleWeekendFilter.WorkWeek);
			}
		}

		private void radioButtonFullWeek_CheckedChanged(object sender,EventArgs e) {
			if(radioButtonFullWeek.Checked) { 
				SetSelected(ScheduleWeekendFilter.FullWeek);
			}
		}

		private void radioButtonWeekEnd_CheckedChanged(object sender,EventArgs e) {
			if(radioButtonWeekEnd.Checked) { 
				SetSelected(ScheduleWeekendFilter.Weekend);
			}
		}

		private void SetSelected(ScheduleWeekendFilter filter) {
			switch(filter) {
				case ScheduleWeekendFilter.FullWeek: 
					_scheduleWeekendFilter=ScheduleWeekendFilter.FullWeek;
					butCopyWeek.Text=Lan.g(this,"Copy Week");
					butClearWeek.Text=Lan.g(this,"Clear Week");
					break;
				case ScheduleWeekendFilter.WorkWeek: 
					_scheduleWeekendFilter=ScheduleWeekendFilter.WorkWeek;
					butCopyWeek.Text=Lan.g(this,"Copy Week");
					butClearWeek.Text=Lan.g(this,"Clear Week");
					break;
				case ScheduleWeekendFilter.Weekend: 
					_scheduleWeekendFilter=ScheduleWeekendFilter.Weekend;
					butCopyWeek.Text=Lan.g(this,"Copy Weekend");
					butClearWeek.Text=Lan.g(this,"Clear Weekend");
					break;
			}
			FillGrid();
		}

		private void butClearWeek_Click(object sender,EventArgs e) {
			if(!ValidateInputs()) {
				return;
			}
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a date first.");
				return;
			}
			if(_hasProvsChanged) {
				MsgBox.Show(this,"Provider or Employee selection has been changed.  Please refresh first.");
				return;
			}
			if(_dateFromDate!=PIn.Date(textDateFrom.Text) || _dateToDate!=PIn.Date(textDateTo.Text)) {
				MsgBox.Show(this,"Dates have changed, refresh before continuing.");
				return;
			}
			int startI=1;
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek || _scheduleWeekendFilter==ScheduleWeekendFilter.Weekend) {
				startI=0;
			}
			DateTime dateSelectedStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,startI);
			DateTime dateSelectedEnd;
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek || _scheduleWeekendFilter==ScheduleWeekendFilter.Weekend) {
				dateSelectedEnd=dateSelectedStart.AddDays(6);
			}
			else {
				dateSelectedEnd=dateSelectedStart.AddDays(4);
			}
			List<long> listProvNums;
			List<long> listEmployeeNums;
			GetSelectedProvidersEmployeesAndClinic(out listProvNums,out listEmployeeNums);
			if(listProvNums.Count>0) {
				if(MessageBox.Show(Lan.g(this,"Delete schedules for")+" "+listProvNums.Distinct().Count()+" "
					+Lan.g(this,"provider(s) for the selected week?"),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
				{
					return;
				}
			}
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend) {
				//Clear sunday and saturday individually
				Schedules.Clear(dateSelectedStart,dateSelectedStart,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,excludeHolidays:true,comboClinic.ClinicNumSelected);
				Schedules.Clear(dateSelectedEnd,dateSelectedEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,excludeHolidays:true,comboClinic.ClinicNumSelected);
				FillGrid();
				_changed=true;
				return;
			}
			//not weekend
			Schedules.Clear(dateSelectedStart,dateSelectedEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,excludeHolidays:true,comboClinic.ClinicNumSelected);
			FillGrid();
			_changed=true;
		}

		private void butCopyDay_Click(object sender,EventArgs e) {
			if(!ValidateInputs()) {
				return;
			}
			if(gridMain.SelectedCell.X==-1){
				MsgBox.Show(this,"Please select a date first.");
				return;
			}
			if((listBoxEmps.SelectedIndices.Count==1 && listBoxEmps.SelectedIndices.Contains(0)//'none' selected only
				|| listBoxEmps.SelectedIndices.Count==0)//Nothing selected
				&& (listBoxProvs.SelectedIndices.Count==1 && listBoxProvs.SelectedIndices.Contains(0)//'none' selected only
				|| listBoxProvs.SelectedIndices.Count==0))//Nothing selected 
			{
				MsgBox.Show(this,"No providers or employees have been selected.");
				return;
			}
			if(_dateFromDate!=PIn.Date(textDateFrom.Text) || _dateToDate!=PIn.Date(textDateTo.Text)) {
				MsgBox.Show(this,"Dates have changed, refresh before continuing.");
				return;
			}
			int selectedCol=gridMain.SelectedCell.X;
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek) {
				selectedCol++;
			}
			else if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend && selectedCol==1) {
				selectedCol=6;
			}
			_dateCopyStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,selectedCol);
			_dateCopyEnd=_dateCopyStart;
			textClipboard.Text=_dateCopyStart.ToShortDateString();
			SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,textClipboard.Text+" was copied.");
		}

		private void butCopyWeek_Click(object sender,EventArgs e) {
			if(!ValidateInputs()) {
				return;
			}
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a date first.");
				return;
			}
			if((listBoxEmps.SelectedIndices.Count==1 && listBoxEmps.SelectedIndices.Contains(0)//'none' selected only
				|| listBoxEmps.SelectedIndices.Count==0)//Nothing selected
				&& (listBoxProvs.SelectedIndices.Count==1 && listBoxProvs.SelectedIndices.Contains(0)//'none' selected only
				|| listBoxProvs.SelectedIndices.Count==0))//Nothing selected 
			{
				MsgBox.Show(this,"No providers or employees have been selected.");
				return;
			}
			if(_dateFromDate!=PIn.Date(textDateFrom.Text) || _dateToDate!=PIn.Date(textDateTo.Text)) {
				MsgBox.Show(this,"Dates have changed, refresh before continuing.");
				return;
			}
			int startI=1;
			int dateSpan=6;
			string seperator=" - ";
			switch(_scheduleWeekendFilter) {
				case ScheduleWeekendFilter.FullWeek: 
					startI=0;
					dateSpan=6;
					break;
				case ScheduleWeekendFilter.WorkWeek: 
					startI=1;
					dateSpan=4;
					break;
				case ScheduleWeekendFilter.Weekend: 
					startI=0;
					dateSpan=6;
					seperator=" & ";
					break;
			}
			_dateCopyStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,startI);
			_dateCopyEnd=_dateCopyStart.AddDays(dateSpan);
			textClipboard.Text=_dateCopyStart.ToShortDateString()+seperator+_dateCopyEnd.ToShortDateString();
			SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,textClipboard.Text+" was copied.");
		}

		private void butPaste_Click(object sender,EventArgs e) {
			if(!ValidateInputs()) {
				return;
			}
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a date first.");
				return;
			}
			if(_dateCopyStart.Year<1880) {
				MsgBox.Show(this,"Please copy a selection to the clipboard first.");
				return;
			}
			if(_hasProvsChanged){
				MsgBox.Show(this,"Provider or Employee selection has been changed.  Please refresh first.");
				return;
			}
			PasteSelected();
		}

		private void PasteSelected() {
			bool isWeekCopied = (_dateCopyStart!=_dateCopyEnd);
			DateTime dateSelectedStart;
			DateTime dateSelectedEnd;
			//calculate which day or week is currently selected.
			if(isWeekCopied){
				int startI=1;
				int dateSpan=6;
				switch(_scheduleWeekendFilter) {
					case ScheduleWeekendFilter.FullWeek: 
						startI=0;
						dateSpan=6;
						break;
					case ScheduleWeekendFilter.WorkWeek: 
						startI=1;
						dateSpan=4;
						break;
					case ScheduleWeekendFilter.Weekend: 
						startI=0;
						dateSpan=6;
						break;
				}
				dateSelectedStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,startI);
				dateSelectedEnd=dateSelectedStart.AddDays(dateSpan);
			}
			else{
				int selectedCol=gridMain.SelectedCell.X;
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek) {
					selectedCol++;
				}
				else if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend && selectedCol==1) {
					selectedCol=6;
				}
				dateSelectedStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,selectedCol);
				dateSelectedEnd=dateSelectedStart;
			}
			//it's not allowed to paste back over the same day or week.
			if(dateSelectedStart==_dateCopyStart) {
				MsgBox.Show(this,"Not allowed to paste back onto the same date as is on the clipboard.");
				return;
			}
			Action actionCloseScheduleProgress=ODProgress.Show();
			List<long> listProvNums;
			List<long> listEmployeeNums;
			GetSelectedProvidersEmployeesAndClinic(out listProvNums,out listEmployeeNums);
			//Get the official list of schedules that are going to be copied over.
			List<Schedule> listSchedulesToCopy=Schedules.RefreshPeriod(_dateCopyStart,_dateCopyEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,
				checkClinicNotes.Checked,comboClinic.ClinicNumSelected);
			listSchedulesToCopy=FilterScheduleList(listSchedulesToCopy,true);
			if(checkReplace.Checked) {
				if(listProvNums.Count > 0) {
					int countDistinctProvNums=listSchedulesToCopy.Where(x => x.ProvNum!=0).Select(y => y.ProvNum).Distinct().Count();
					actionCloseScheduleProgress?.Invoke();
					if(MessageBox.Show(Lan.g(this,"Replace schedules for")+" "+countDistinctProvNums+" "
						+Lan.g(this,"provider(s)?"),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
					{
						return;
					}
					if(listBoxProvs.SelectedIndices.Count > countDistinctProvNums && !MsgBox.Show(this,MsgBoxButtons.YesNo,
						"One or more selected providers do not have schedules for the date range copied to the Clipboard Contents. "
						+"These providers will have their schedules removed wherever pasted.  Continue?")) 
					{
						return;
					}
					//user chose to continue.
					actionCloseScheduleProgress=ODProgress.Show();
				}
			}
			//Flag every schedule that we are copying as new (because conflict detection requires schedules marked as new)
			listSchedulesToCopy.ForEach(x => x.IsNew=true);
			//Always check for overlapping schedules regardless of checkReplace.Checked.
			if(checkWarnProvOverlap.Checked) {
				//Only check overlapping provider schedules.
				List<Schedule> listSchedulesProv=listSchedulesToCopy.FindAll(x => x.SchedType==ScheduleType.Provider);
				List<long> listProvNumsOverlapping=Schedules.GetOverlappingSchedProvNumsForRange(listSchedulesProv,dateSelectedStart,dateSelectedEnd
					,listIgnoreProvNums:(checkReplace.Checked ? listProvNums : null));
				if(listProvNumsOverlapping.Count>0) {
					actionCloseScheduleProgress?.Invoke();
					if(MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")
						+"\r\n"+Lan.g(this,"Providers affected")
						+":\r\n  "+string.Join("\r\n  ",listProvNumsOverlapping.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
					{
						return;
					}
					actionCloseScheduleProgress=ODProgress.Show();
				}
			}
			List<Schedule> listSchedulesHolidays=GetHolidaySchedules(dateSelectedStart,dateSelectedEnd);
			if(checkReplace.Checked) {
				List<Schedule> listSchedulesToDelete=Schedules.GetSchedulesToDelete(dateSelectedStart,dateSelectedEnd,listProvNums,listEmployeeNums,
					checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.ClinicNumSelected);
				listSchedulesToDelete=FilterScheduleList(listSchedulesToDelete,true);
				Schedules.DeleteMany(listSchedulesToDelete.Select(x => x.ScheduleNum).ToList());
			}
			Schedule schedule=new Schedule();
			int weekDelta=0;
			if(isWeekCopied){
				TimeSpan span=dateSelectedStart-_dateCopyStart;
				weekDelta=span.Days/7;//usually a positive # representing a future paste, but can be negative
			}
			List<Schedule> listSchedulesToInsert=new List<Schedule>();
			for(int i=0;i<listSchedulesToCopy.Count;i++){
				schedule=listSchedulesToCopy[i];
				if(isWeekCopied){
					schedule.SchedDate=schedule.SchedDate.AddDays(weekDelta*7);
				}
				else{
					schedule.SchedDate=dateSelectedStart;
				}
				if(listSchedulesHolidays.Exists(x => x.SchedDate==schedule.SchedDate)){
					continue;//Don't add schedules to a day that's a holiday.
				}
				listSchedulesToInsert.Add(schedule);
			}
			if(listSchedulesHolidays.Count>0){
				MessageBox.Show(Lan.g(this,listSchedulesHolidays.Count+" holidays exist in the destination date range. Holidays will not be replaced and must be done manually."));
			}
			Schedules.Insert(false,true,listSchedulesToInsert);
			DateTime rememberDateStart=_dateCopyStart;
			DateTime rememberDateEnd=_dateCopyEnd;
			_pointClickedCell=gridMain.SelectedCell;
			FillGrid();
			_dateCopyStart=rememberDateStart;
			_dateCopyEnd=rememberDateEnd;
			if(isWeekCopied){
				textClipboard.Text=_dateCopyStart.ToShortDateString()+" - "+_dateCopyEnd.ToShortDateString();
			}
			else{
				textClipboard.Text=_dateCopyStart.ToShortDateString();
			}
			_changed=true;
			SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,"Pasted schedule from "+textClipboard.Text+" to "+schedule.SchedDate.ToShortDateString());
			actionCloseScheduleProgress?.Invoke();
		}

		private void butRepeat_Click(object sender,EventArgs e) {
			bool isWeekCopied=(_dateCopyStart!=_dateCopyEnd);
			if(!ValidateInputs()) {
				return;
			}
			int repeatCount;
			try{
				repeatCount=PIn.Int(textRepeat.Text);
			}
			catch{
				MsgBox.Show(this,"Please fix number box first.");
				return;
			}
			if(repeatCount>1250 && !isWeekCopied) {
				MsgBox.Show(this,"Please enter a number of days less than 1250.");
				return;
			}
			if(repeatCount>250 && isWeekCopied) {
				MsgBox.Show(this,"Please enter a number of weeks less than 250.");
				return;
			}
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a date first.");
				return;
			}
			if(_dateCopyStart.Year<1880) {
				MsgBox.Show(this,"Please copy a selection to the clipboard first.");
				return;
			}
			if(_hasProvsChanged) {
				MsgBox.Show(this,"Provider or Employee selection has been changed.  Please refresh first.");
				return;
			}
			if(repeatCount<=0) {
				MsgBox.Show(this,"Please enter a repeat number greater than 0.");
				return;
			}
			Action actionCloseScheduleProgress=ODProgress.Show();
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
			//calculate which day or week is currently selected.
			DateTime dateSelectedStart;
			DateTime dateSelectedEnd;
			if(isWeekCopied) {
				int startI=1;
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek || _scheduleWeekendFilter==ScheduleWeekendFilter.Weekend) {
					startI=0;
				}
				dateSelectedStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,startI);
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek || _scheduleWeekendFilter==ScheduleWeekendFilter.Weekend) {
					dateSelectedEnd=dateSelectedStart.AddDays(6);
				}
				else {
					dateSelectedEnd=dateSelectedStart.AddDays(4);
				}
			}
			else {
				int selectedCol=gridMain.SelectedCell.X;
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek) {
					selectedCol++;//Increment selected day to account for sunday
				}
				else if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend && selectedCol==1) {
					selectedCol=6;//Set selected day to sunday
				}
				dateSelectedStart=Schedules.GetDateCal(_dateFromDate,gridMain.SelectedCell.Y,selectedCol);
				dateSelectedEnd=dateSelectedStart;
			}
			List<long> listProvNums;
			List<long> listEmployeeNums;
			GetSelectedProvidersEmployeesAndClinic(out listProvNums,out listEmployeeNums);
			Logger.LogToPath("RefreshPeriod",LogPath.Signals,LogPhase.Start);
			List<Schedule> listSchedulesToCopy=Schedules.RefreshPeriod(_dateCopyStart,_dateCopyEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,
				checkClinicNotes.Checked,comboClinic.ClinicNumSelected);
			listSchedulesToCopy=FilterScheduleList(listSchedulesToCopy,true);
			if(checkReplace.Checked) {
				if(listProvNums.Count > 0) {
					int countDistinctProvNums=listSchedulesToCopy.Where(x => x.ProvNum!=0).Select(y => y.ProvNum).Distinct().Count();
					actionCloseScheduleProgress?.Invoke();
					if(MessageBox.Show(Lan.g(this,"Replace schedules for")+" "+countDistinctProvNums+" "
						+Lan.g(this,"provider(s)?"),"",MessageBoxButtons.YesNo)==DialogResult.No) 
					{
						return;
					}
					if(listBoxProvs.SelectedIndices.Count > countDistinctProvNums && !MsgBox.Show(this,MsgBoxButtons.YesNo,
						"One or more selected providers do not have schedules for the date range copied to the Clipboard Contents. "
						+"These providers will have their schedules removed wherever pasted.  Continue?")) 
					{
						return;
					}
					//user chose to continue.
					actionCloseScheduleProgress=ODProgress.Show();
				}
			}
			//Flag every schedule that we are copying as new (because conflict detection requires schedules marked as new)
			listSchedulesToCopy.ForEach(x => x.IsNew=true);
			Logger.LogToPath("RefreshPeriod",LogPath.Signals,LogPhase.End);
			Schedule schedule=new Schedule();
			int weekDelta=0;
			TimeSpan timeSpan;
			if(isWeekCopied) {
				timeSpan=dateSelectedStart-_dateCopyStart;
				weekDelta=timeSpan.Days/7;//usually a positive # representing a future paste, but can be negative
			}
			//this is needed when repeat pasting days in order to calculate skipping weekends.
			//Get the official list of schedules that are going to be copied.
			//Always check for overlapping schedules regardless of checkReplace.Checked.
			int dayCount=0;
			//Only check overlapping provider schedules.
			List<Schedule> listSchedulesProv=listSchedulesToCopy.FindAll(x => x.SchedType==ScheduleType.Provider);
			List<long> listProvNumsOverlapping=new List<long>();
			DateTime dateEnd=dateSelectedEnd;
			for(int i=0;i<repeatCount;i++) {
				DateTime dateStart;
				if(isWeekCopied) {
					dateStart=dateSelectedStart.AddDays(i*7);
					dateEnd=dateSelectedEnd.AddDays(i*7);
				}
				else {
					dateStart=dateSelectedStart.AddDays(dayCount);
					dateEnd=dateSelectedEnd.AddDays(dayCount);
					dayCount+=CalculateNextDay(dateSelectedStart.AddDays(dayCount));
				}
				if(checkWarnProvOverlap.Checked) {
					listProvNumsOverlapping=listProvNumsOverlapping
						.Union(Schedules.GetOverlappingSchedProvNumsForRange(listSchedulesProv,dateStart,dateEnd
							,listIgnoreProvNums: (checkReplace.Checked ? listProvNums : null)))
						.ToList();
				}
			}
			if(listProvNumsOverlapping.Count > 0) {
				actionCloseScheduleProgress?.Invoke();
				if(MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")
					+"\r\n"+Lan.g(this,"Providers affected")
					+":\r\n  "+string.Join("\r\n  ",listProvNumsOverlapping.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
				{
					return;
				}
				actionCloseScheduleProgress=ODProgress.Show();
			}
			Logger.LogToPath("ScheduleUpsert",LogPath.Signals,LogPhase.Start,"repeatCount: "+repeatCount.ToString());
			List<Schedule> listSchedulesToInsert=new List<Schedule>();
			List<long> listSchedNumsToDelete=new List<long>();
			List<Schedule> listSchedulesHoliday=GetHolidaySchedules(dateSelectedStart,dateEnd);
			dayCount=0;//this is needed for repeat pasting days in order to calculate skipping weekends/work week.
			for(int r=0;r<repeatCount;r++){//for example, user wants to repeat 3 times.
				if(checkReplace.Checked) {
					List<Schedule> listSchedulesToDelete=new List<Schedule>();
					if(isWeekCopied){
						Logger.LogToPath("isWeek.Schedules.Clear",LogPath.Signals,LogPhase.Start);
						listSchedulesToDelete=Schedules.GetSchedulesToDelete(dateSelectedStart.AddDays(r*7),dateSelectedEnd.AddDays(r*7),listProvNums,
							listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.ClinicNumSelected);
						Logger.LogToPath("isWeek.Schedules.Clear",LogPath.Signals,LogPhase.End);
					}
					else{
						Logger.LogToPath("!isWeek.Schedules.Clear",LogPath.Signals,LogPhase.Start);
					  listSchedulesToDelete=Schedules.GetSchedulesToDelete(dateSelectedStart.AddDays(dayCount),dateSelectedEnd.AddDays(dayCount),
							listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.ClinicNumSelected);
						Logger.LogToPath("!isWeek.Schedules.Clear",LogPath.Signals,LogPhase.End);
					}
					listSchedulesToDelete=FilterScheduleList(listSchedulesToDelete,true);
					listSchedNumsToDelete.AddRange(listSchedulesToDelete.Select(x => x.ScheduleNum).ToList());
				}
				Logger.LogToPath("SchedList.Insert",LogPath.Signals,LogPhase.Start,"SchedList.Count: "+listSchedulesToCopy.Count.ToString());
				for(int i=0;i<listSchedulesToCopy.Count;i++) {//For example, if 3 weeks for one provider, then about 30 loops.
					schedule=listSchedulesToCopy[i].Copy();
					if(isWeekCopied) {
						schedule.SchedDate=schedule.SchedDate.AddDays((weekDelta+r)*7).AddHours(1).Date;
					}
					else {
						schedule.SchedDate=dateSelectedStart.AddDays(dayCount);
					}
					if(listSchedulesHoliday.Exists(x => x.SchedDate==schedule.SchedDate)){
							continue;//Don't add to holidays
					}
					listSchedulesToInsert.Add(schedule);
				}
				Logger.LogToPath("SchedList.Insert",LogPath.Signals,LogPhase.End);		
				dayCount+=CalculateNextDay(dateSelectedStart.AddDays(dayCount));
			}
			if(listSchedulesHoliday.Count>0) {
				MessageBox.Show(Lan.g(this,listSchedulesHoliday.Count+" holidays exist in the destination date range. Holidays will not be replaced and must be done manually."));
			}
			Schedules.DeleteMany(listSchedNumsToDelete);
			Schedules.Insert(false,true,listSchedulesToInsert);
			Logger.LogToPath("ScheduleUpsert",LogPath.Signals,LogPhase.End);
			DateTime rememberDateStart=_dateCopyStart;
			DateTime rememberDateEnd=_dateCopyEnd;
			_pointClickedCell=gridMain.SelectedCell;
			FillGrid();
			_dateCopyStart=rememberDateStart;
			_dateCopyEnd=rememberDateEnd;
			if(isWeekCopied) {
				textClipboard.Text=_dateCopyStart.ToShortDateString()+" - "+_dateCopyEnd.ToShortDateString();
			}
			else {
				textClipboard.Text=_dateCopyStart.ToShortDateString();
			}
			_changed=true;
			SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,"Repeated schedule "+repeatCount+" time(s) from "+textClipboard.Text+
				" to "+schedule.SchedDate.ToShortDateString());
			actionCloseScheduleProgress?.Invoke();
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Staff schedule printed"));
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadPrinted) {
				text=Lan.g(this,"Schedule");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				_isHeadPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangleBounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		///<summary>Fires as resizing is happening.</summary>
		private void FormSchedule_Resize(object sender,EventArgs e) {
			if(_isResizing || WindowState==FormWindowState.Minimized) {
				return;
			}
			FillGrid(false);
		}

		///<summary>Fires when manual resizing begins.</summary>
		private void FormSchedule_ResizeBegin(object sender,EventArgs e) {
			_pointClickedCell=gridMain.SelectedCell;
			_isResizing=true;
		}

		///<summary>Fires when resizing is complete, except when changing window state. I.e. this is not fired when the window is maximized or minimized.</summary>
		private void FormSchedule_ResizeEnd(object sender,EventArgs e) {
			FillGrid(false);
			_isResizing=false;
		}

		private void FormSchedule_FormClosing(object sender,FormClosingEventArgs e) {
			if(_changed){
				SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,"");
			}
		}

		///<summary>Returns the number of days to get to the next day based on the week filter view selection.</summary>
		private int CalculateNextDay(DateTime dateTimeCur) {
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek && dateTimeCur.DayOfWeek==DayOfWeek.Friday) {
				return 3;//Skip to the following monday
			}
			else if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend && dateTimeCur.DayOfWeek==DayOfWeek.Sunday) {
				return 6;//Skip to the following saturday
			}
			return 1;
		}

		///<summary>Retrieves all holiday schedules within the selected paste date range and selected Clinic.</summary>
		private List<Schedule> GetHolidaySchedules(DateTime dateTimeStart,DateTime dateTimeEnd) {
			if(!checkPracticeNotes.Checked && !checkClinicNotes.Checked) {//No holidays should be showing
				return new List<Schedule>();
			}
			List<long> listClinicNums=new List<long>();
			listClinicNums.AddRange(comboClinic.ListClinics.Select(x=>x.ClinicNum).ToList());//Retrieve all clinicNums from the combobox
			if(!checkPracticeNotes.Checked) {
				listClinicNums.Remove(0);
			}
			if(!checkClinicNotes.Checked) {
				listClinicNums.RemoveAll(x => x!=0);
			}
			else if(comboClinic.ClinicNumSelected!=0) {//A clinic is selected
				listClinicNums.RemoveAll(x => x!=0 && x!=comboClinic.ClinicNumSelected);
			}
			List<Schedule> listSchedulesHoliday=Schedules.GetAllHolidaysForDateRange(dateTimeStart,dateTimeEnd,listClinicNums);
			listSchedulesHoliday=FilterScheduleList(listSchedulesHoliday,false);
			return listSchedulesHoliday;
		}

		///<summary>This will filter any days from the passed in schedule list. This includes days that are not shown in the week filter view.
		///For example, weekends shouldn't be included in Work Week view. And all schedules on a holiday if passed in true.</summary>
		private List<Schedule> FilterScheduleList(List<Schedule> listSchedules,bool removeHolidays){
			if(removeHolidays) {
				List<Schedule> listSchedulesHoliday=listSchedules.FindAll(x => x.Status==SchedStatus.Holiday);
				for(int i=0;i<listSchedulesHoliday.Count;i++) {
					listSchedules.RemoveAll(x => x.SchedDate.Date==listSchedulesHoliday[i].SchedDate);//Filter all schedules on the holiday to not be changed
				}
			}
			if(_scheduleWeekendFilter==ScheduleWeekendFilter.FullWeek) {
				return listSchedules;
			}
			for(int i=listSchedules.Count-1;i>=0;i--) {
				if(_scheduleWeekendFilter==ScheduleWeekendFilter.Weekend 
					&& listSchedules[i].SchedDate.DayOfWeek!=DayOfWeek.Saturday
					&& listSchedules[i].SchedDate.DayOfWeek!=DayOfWeek.Sunday)
				{
					listSchedules.Remove(listSchedules[i]);//Remove weekdays on the weekend view
				}
				else if(_scheduleWeekendFilter==ScheduleWeekendFilter.WorkWeek
					&& (listSchedules[i].SchedDate.DayOfWeek==DayOfWeek.Saturday || listSchedules[i].SchedDate.DayOfWeek==DayOfWeek.Sunday)) {
					listSchedules.Remove(listSchedules[i]);//Remove weekend days on the work week view
				}
			}
			return listSchedules;
		}

		private enum ScheduleWeekendFilter {
			FullWeek,
			WorkWeek,
			Weekend,
		}

		private enum FormScheduleMode {
			SetupSchedule,
			ViewSchedule
		}
	}

}





















