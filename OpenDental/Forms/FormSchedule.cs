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
		private bool _provsChanged;
		private bool headingPrinted;
		private int pagesPrinted;
		private int headingPrintH;
		bool changed;
		private List<Provider> _listProviders;
		private List<Employee> _listEmps;
		private DataTable _tableScheds;
		private bool _isResizing;
		private Point _clickedCell;
		///<summary>By default is FormScheduleMode.SetupSchedule.
		///If user does not have Schedule permission then will be in FormScheduleMode.ViewSchedule.</summary>
		private FormScheduleMode _formMode;
		private List<long> _listPreSelectedEmpNums;
		private List<long> _listPreSelectedProvNums;
		///<summary>Keeps track of the last time the "From Date" was set via the fill grid.
		///Used to determine if the user has changed the date since last fill grid.</summary>
		private DateTime _fromDateCur;
		///<summary>Keeps track of the last time the "To Date" was set via the fill grid.
		///Used to determine if the user has changed the date since last fill grid.</summary>
		private DateTime _toDateCur;
		private ScheduleWeekendFilter _weekendFilter=ScheduleWeekendFilter.WorkWeek;

		///<summary>Checks if dates in textbox match current dates stored.</summary>
		private bool HaveDatesChanged {
			get {
				if(_fromDateCur!=PIn.Date(textDateFrom.Text) || _toDateCur!=PIn.Date(textDateTo.Text)) {
					return true;
				}
				return false;
			}
		}

		///<summary></summary>
		public FormSchedule(List<long> listPreSelectedEmpNums=null,List<long> listPreSelectedProvNums=null)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listPreSelectedEmpNums=listPreSelectedEmpNums;
			_listPreSelectedProvNums=listPreSelectedProvNums;
		}

		private void FormSchedule_Load(object sender,EventArgs e) {
			radioButtonWorkWeek.Checked=true;
			_formMode=FormScheduleMode.ViewSchedule;
			if(Security.IsAuthorized(Permissions.Schedules,DateTime.MinValue,true)){
				_formMode=FormScheduleMode.SetupSchedule;
			};
			switch(_formMode) {
				case FormScheduleMode.SetupSchedule:
					DateTime dateFrom=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
					textDateFrom.Text=dateFrom.ToShortDateString();
					textDateTo.Text=dateFrom.AddMonths(12).AddDays(-1).ToShortDateString();
					break;
				case FormScheduleMode.ViewSchedule:
					butDelete.Visible=false;
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
			_listEmps=new List<Employee>() { new Employee() { EmployeeNum=0,FName="none" } };
			_listProviders=new List<Provider>() { new Provider() { ProvNum=0,Abbr="none" } };
			if(PrefC.HasClinicsEnabled) {
				//clinicNum will be 0 for unrestricted users with HQ selected in which case this will get only emps/provs not assigned to a clinic
				_listEmps.AddRange(Employees.GetEmpsForClinic(comboClinic.SelectedClinicNum));
				_listProviders.AddRange(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			}
			else {//Not using clinics
				_listEmps.AddRange(Employees.GetDeepCopy(true));
				_listProviders.AddRange(Providers.GetDeepCopy(true));
			}
			List<long> listPreviouslySelectedEmpNums=listBoxEmps.GetListSelected<Employee>().Select(x => x.EmployeeNum).ToList();
			listBoxEmps.Items.Clear();
			_listEmps.ForEach(x => listBoxEmps.Items.Add(x.FName,x));
			List<long> listPreviouslySelectedProvNums=listBoxProvs.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			listBoxProvs.Items.Clear();
			_listProviders.ForEach(x => listBoxProvs.Items.Add(x.Abbr,x));
			if(_listPreSelectedEmpNums!=null || _listPreSelectedProvNums!=null) {
				if(_listPreSelectedEmpNums!=null && _listPreSelectedEmpNums.Count>0) {
					//Employee Listbox
					for(int i=1;i<listBoxEmps.Items.Count;i++) {
						if(!_listPreSelectedEmpNums.Contains(_listEmps[i].EmployeeNum)) {
							continue;
						}
						listBoxEmps.SetSelected(i,true);
					}
				}
				else {
					listBoxEmps.SelectedIndex=0;//select the 'none' entry;
				}
				if(_listPreSelectedProvNums!=null && _listPreSelectedProvNums.Count>0) {
					//Provider Listbox
					for(int i=1;i<listBoxProvs.Items.Count;i++) {
						if(!_listPreSelectedProvNums.Contains(_listProviders[i].ProvNum)) {
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
				if(listPreviouslySelectedEmpNums.Count > 0) {
					for(int i=0; i<listBoxEmps.Items.Count; i++) {
						if(listPreviouslySelectedEmpNums.Contains(((Employee)listBoxEmps.Items.GetObjectAt(i)).EmployeeNum)) {
							listBoxEmps.SetSelected(i,true);
						}
					}
				}
				else {
					listBoxEmps.SelectedIndex=0;//select the 'none' entry;
				}
				if(listPreviouslySelectedProvNums.Count > 0) {
					for(int i=0; i<listBoxProvs.Items.Count; i++) {
						if(listPreviouslySelectedProvNums.Contains(((Provider)listBoxProvs.Items.GetObjectAt(i)).ProvNum)) {
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
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridRows.Clear();
			gridMain.EndUpdate();
			_provsChanged=false;
			List<long> provNums=new List<long>();
			for(int i=0;i<listBoxProvs.SelectedIndices.Count;i++){
				provNums.Add(_listProviders[listBoxProvs.SelectedIndices[i]].ProvNum);
			}
			List<long> empNums=new List<long>();
			for(int i=0;i<listBoxEmps.SelectedIndices.Count;i++){
				empNums.Add(_listEmps[listBoxEmps.SelectedIndices[i]].EmployeeNum);
			}
			provNums.RemoveAll(x => x==0);
			empNums.RemoveAll(x => x==0);
			if(doRefreshData || this._tableScheds==null) {
				bool canViewNotes=true;
				if(PrefC.IsODHQ) {
					canViewNotes=Security.IsAuthorized(Permissions.Schedules,true);
				}
				_fromDateCur=PIn.Date(textDateFrom.Text);
				_toDateCur=PIn.Date(textDateTo.Text);
				Logger.LogToPath("Schedules.GetPeriod",LogPath.Signals,LogPhase.Start);
				_tableScheds=Schedules.GetPeriod(_fromDateCur,_toDateCur,provNums,empNums,checkPracticeNotes.Checked,
					checkClinicNotes.Checked,comboClinic.SelectedClinicNum,checkShowClinicSchedules.Checked,canViewNotes);
				Logger.LogToPath("Schedules.GetPeriod",LogPath.Signals,LogPhase.End);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			if(_weekendFilter==ScheduleWeekendFilter.Weekend || _weekendFilter==ScheduleWeekendFilter.FullWeek) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Sunday"),50){ IsWidthDynamic=true });
			}
			if(_weekendFilter==ScheduleWeekendFilter.WorkWeek || _weekendFilter==ScheduleWeekendFilter.FullWeek) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Monday"),50){ IsWidthDynamic=true });
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Tuesday"),50){ IsWidthDynamic=true });
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Wednesday"),50){ IsWidthDynamic=true });
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Thursday"),50){ IsWidthDynamic=true });
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Friday"),50){ IsWidthDynamic=true });
			}
			if(_weekendFilter==ScheduleWeekendFilter.Weekend || _weekendFilter==ScheduleWeekendFilter.FullWeek){
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSchedule","Saturday"),50){ IsWidthDynamic=true });
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableScheds.Rows.Count;i++){
				row=new GridRow();
				if(_weekendFilter==ScheduleWeekendFilter.Weekend || _weekendFilter==ScheduleWeekendFilter.FullWeek){
					row.Cells.Add(_tableScheds.Rows[i][0].ToString());
				}
				if(_weekendFilter==ScheduleWeekendFilter.WorkWeek || _weekendFilter==ScheduleWeekendFilter.FullWeek) {
					row.Cells.Add(_tableScheds.Rows[i][1].ToString());
					row.Cells.Add(_tableScheds.Rows[i][2].ToString());
					row.Cells.Add(_tableScheds.Rows[i][3].ToString());
					row.Cells.Add(_tableScheds.Rows[i][4].ToString());
					row.Cells.Add(_tableScheds.Rows[i][5].ToString());
				}
				if(_weekendFilter==ScheduleWeekendFilter.Weekend || _weekendFilter==ScheduleWeekendFilter.FullWeek) {
					row.Cells.Add(_tableScheds.Rows[i][6].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//Set today red
			if(_weekendFilter==ScheduleWeekendFilter.WorkWeek 
				&& (DateTime.Today.DayOfWeek==DayOfWeek.Sunday || DateTime.Today.DayOfWeek==DayOfWeek.Saturday))
			{
				return;
			}
			if(DateTime.Today>_toDateCur || DateTime.Today<_fromDateCur){
				return;
			}
			if(_weekendFilter==ScheduleWeekendFilter.Weekend) {
				return;//don't highlight if we are on the weekend.
			}
			int colI=(int)DateTime.Today.DayOfWeek;
			if(_weekendFilter==ScheduleWeekendFilter.WorkWeek){
				colI--;
			}
			gridMain.ListGridRows[Schedules.GetRowCal(_fromDateCur,DateTime.Today)].Cells[colI].ColorText=Color.Red;
			if(_clickedCell!=null //when first opening form
				&& _clickedCell.Y>-1 
				&& _clickedCell.Y< gridMain.ListGridRows.Count
				&& _clickedCell.X>-1
				&& _clickedCell.X<gridMain.ListGridColumns.Count) 
			{
				gridMain.SetSelected(_clickedCell);
			}
			//scroll to cell to keep it in view when editing schedules.
			if(gridMain.SelectedCell.X>-1 && gridMain.SelectedCell.Y>-1) {
				gridMain.ScrollToIndex(gridMain.SelectedCell.Y);
			}
		}

		private void checkShowClinicSchedules_CheckedChanged(object sender,EventArgs e) {
			if(checkShowClinicSchedules.Checked) {
				SelectAllProvsAndEmps();
				butDelete.Enabled=false;
				butCopyDay.Enabled=false;
				butCopyWeek.Enabled=false;
				butPaste.Enabled=false;
				butRepeat.Enabled=false;
			}
			else {
				butDelete.Enabled=true;
				butCopyDay.Enabled=true;
				butCopyWeek.Enabled=true;
				butPaste.Enabled=true;
				butRepeat.Enabled=true;
			}
			if(!ValidateInputs()) {
				return;
			}
			_clickedCell=gridMain.SelectedCell;
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
			comboClinic.Text=Lan.g(this,"Show Practice Notes");
			if(comboClinic.SelectedClinicNum>0) {
				comboClinic.Text=Lan.g(this,"Show Practice and Clinic Notes");
			}
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
			_clickedCell=gridMain.SelectedCell;
			FillGrid();
		}

		private void checkPracticeNotes_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkClinicNotes_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			_clickedCell=gridMain.SelectedCell;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.Schedules,DateTime.MinValue)) {
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
			if(_weekendFilter==ScheduleWeekendFilter.WorkWeek) {
				clickedCol++;
			}
			if(_weekendFilter==ScheduleWeekendFilter.Weekend && clickedCol==1) {
				clickedCol=6;//used to calculate correct day to edit.
			}
			//the "clickedCell" is in terms of the entire 7 col layout.
			DateTime selectedDate=Schedules.GetDateCal(_fromDateCur,e.Row,clickedCol);
			if(selectedDate<_fromDateCur || selectedDate>_toDateCur){
				return;
			}
			//MessageBox.Show(selectedDate.ToShortDateString());
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.SelectedClinicNum==-1) {
					MsgBox.Show(this,"Please select a clinic.");
					return;
				}
			}
			string provAbbr="";
			string empFName="";
			//Get all of the selected providers and employees (removing the "none" options).
			List<Provider> listSelectedProvs=listBoxProvs.GetListSelected<Provider>().FindAll(x => x.ProvNum > 0);
			List<Employee> listSelectedEmps=listBoxEmps.GetListSelected<Employee>().FindAll(x => x.EmployeeNum > 0);
			if(listSelectedProvs.Count==1 && listSelectedEmps.Count==0) {//only 1 provider selected, pass into schedule day filter
				provAbbr=listSelectedProvs[0].Abbr;
			}
			else if(listSelectedEmps.Count==1 && listSelectedProvs.Count==0) {//only 1 employee selected, pass into schedule day filter
				empFName=listSelectedEmps[0].FName;
			}
			else if(listSelectedProvs.Count==1 && listSelectedEmps.Count==1) {//1 provider and 1 employee selected
				//see if the names match, if we're dealing with the same person it's okay to pass both in, if not then don't pass in either. 
				if(listSelectedProvs[0].FName==listSelectedEmps[0].FName 
					&& listSelectedProvs[0].LName==listSelectedEmps[0].LName) 
				{
					provAbbr=listSelectedProvs[0].Abbr;
					empFName=listSelectedEmps[0].FName;
				}
			}
			using FormScheduleDayEdit FormS=new FormScheduleDayEdit(selectedDate,comboClinic.SelectedClinicNum,provAbbr,empFName,true);
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
			changed=true;
		}

		private void listProv_Click(object sender,EventArgs e) {
			_provsChanged=true;
		}

		private void listEmp_Click(object sender,EventArgs e) {
			_provsChanged=true;
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
					_weekendFilter=ScheduleWeekendFilter.FullWeek;
					butCopyWeek.Text=Lan.g(this,"Copy Week");
					butDelete.Text=Lan.g(this,"Clear Week");
					break;
				case ScheduleWeekendFilter.WorkWeek: 
					_weekendFilter=ScheduleWeekendFilter.WorkWeek;
					butCopyWeek.Text=Lan.g(this,"Copy Week");
					butDelete.Text=Lan.g(this,"Clear Week");
					break;
				case ScheduleWeekendFilter.Weekend: 
					_weekendFilter=ScheduleWeekendFilter.Weekend;
					butCopyWeek.Text=Lan.g(this,"Copy Weekend");
					butDelete.Text=Lan.g(this,"Clear Weekend");
					break;
			}
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!ValidateInputs()) {
				return;
			}
			if(gridMain.SelectedCell.X==-1) {
				MsgBox.Show(this,"Please select a date first.");
				return;
			}
			if(_provsChanged) {
				MsgBox.Show(this,"Provider or Employee selection has been changed.  Please refresh first.");
				return;
			}
			if(HaveDatesChanged) {
				MsgBox.Show(this,"Dates have changed, refresh before continuing.");
				return;
			}
			int startI=1;
			if(_weekendFilter==ScheduleWeekendFilter.FullWeek || _weekendFilter==ScheduleWeekendFilter.Weekend) {
				startI=0;
			}
			DateTime dateSelectedStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,startI);
			DateTime dateSelectedEnd;
			if(_weekendFilter==ScheduleWeekendFilter.FullWeek || _weekendFilter==ScheduleWeekendFilter.Weekend) {
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
			if(_weekendFilter==ScheduleWeekendFilter.Weekend) {
				//Clear sunday and saturday individually
				Schedules.Clear(dateSelectedStart,dateSelectedStart,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
				Schedules.Clear(dateSelectedEnd,dateSelectedEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
			} 
			else {
				Schedules.Clear(dateSelectedStart,dateSelectedEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.SelectedClinicNum);	
			}
			
			FillGrid();
			changed=true;
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
			if(HaveDatesChanged) {
				MsgBox.Show(this,"Dates have changed, refresh before continuing.");
				return;
			}
			int selectedCol=gridMain.SelectedCell.X;
			if(_weekendFilter==ScheduleWeekendFilter.WorkWeek) {
				selectedCol++;
			}
			else if(_weekendFilter==ScheduleWeekendFilter.Weekend && selectedCol==1) {
				selectedCol=6;
			}
			_dateCopyStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,selectedCol);
			_dateCopyEnd=_dateCopyStart;
			textClipboard.Text=_dateCopyStart.ToShortDateString();
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
			if(HaveDatesChanged) {
				MsgBox.Show(this,"Dates have changed, refresh before continuing.");
				return;
			}
			int startI=1;
			int dateSpan=6;
			string seperator=" - ";
			switch(_weekendFilter) {
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
			_dateCopyStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,startI);
			_dateCopyEnd=_dateCopyStart.AddDays(dateSpan);
			textClipboard.Text=_dateCopyStart.ToShortDateString()+seperator+_dateCopyEnd.ToShortDateString();
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
			if(_provsChanged){
				MsgBox.Show(this,"Provider or Employee selection has been changed.  Please refresh first.");
				return;
			}
			PasteSelected();
		}

		private void PasteSelected() {
			bool isWeek = (_dateCopyStart!=_dateCopyEnd);
			DateTime dateSelectedStart;
			DateTime dateSelectedEnd;
			//calculate which day or week is currently selected.
			if(isWeek){
				int startI=1;
				int dateSpan=6;
				switch(_weekendFilter) {
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
				dateSelectedStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,startI);
				dateSelectedEnd=dateSelectedStart.AddDays(dateSpan);
			}
			else{
				int selectedCol=gridMain.SelectedCell.X;
				if(_weekendFilter==ScheduleWeekendFilter.WorkWeek) {
					selectedCol++;
				}
				else if(_weekendFilter==ScheduleWeekendFilter.Weekend && selectedCol==1) {
					selectedCol=6;
				}
				dateSelectedStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,selectedCol);
				dateSelectedEnd=dateSelectedStart;
			}
			//it's not allowed to paste back over the same day or week.
			if(dateSelectedStart==_dateCopyStart) {
				MsgBox.Show(this,"Not allowed to paste back onto the same date as is on the clipboard.");
				return;
			}
			Action actionCloseScheduleProgress=ODProgress.Show(ODEventType.Schedule,typeof(ScheduleEvent));
			List<long> listProvNums;
			List<long> listEmployeeNums;
			GetSelectedProvidersEmployeesAndClinic(out listProvNums,out listEmployeeNums);
			//Get the official list of schedules that are going to be copied over.
			List<Schedule> listSchedulesToCopy=Schedules.RefreshPeriod(_dateCopyStart,_dateCopyEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,
				checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
			listSchedulesToCopy.RemoveAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday);//Remove holiday schedules from the copy
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
					actionCloseScheduleProgress=ODProgress.Show(ODEventType.Schedule,typeof(ScheduleEvent));
				}
			}
			//Flag every schedule that we are copying as new (because conflict detection requires schedules marked as new)
			listSchedulesToCopy.ForEach(x => x.IsNew=true);
			//Always check for overlapping schedules regardless of checkReplace.Checked.
			if(checkWarnProvOverlap.Checked) {
				//Only check overlapping provider schedules.
				List<Schedule> listProvSchedules=listSchedulesToCopy.FindAll(x => x.SchedType==ScheduleType.Provider);
				List<long> listOverlappingProvNums=Schedules.GetOverlappingSchedProvNumsForRange(listProvSchedules,dateSelectedStart,dateSelectedEnd
					,listIgnoreProvNums:(checkReplace.Checked ? listProvNums : null));
				if(listOverlappingProvNums.Count>0) {
					actionCloseScheduleProgress?.Invoke();
					if(MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")
						+"\r\n"+Lan.g(this,"Providers affected")
						+":\r\n  "+string.Join("\r\n  ",listOverlappingProvNums.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
					{
						return;
					}
					actionCloseScheduleProgress=ODProgress.Show(ODEventType.Schedule,typeof(ScheduleEvent));
				}
			}
			List<Schedule> listHolidaySchedules=new List<Schedule>();
			bool isHoliday=false;
			if(checkReplace.Checked) {
				List<Schedule> listSchedsToDelete=Schedules.GetSchedulesToDelete(dateSelectedStart,dateSelectedEnd,listProvNums,listEmployeeNums,
					checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
				for(int i=listSchedsToDelete.Count-1;i>=0;i--) {//When pasting, do not paste over a holiday schedule.
					if(listSchedsToDelete[i].SchedType==ScheduleType.Practice && listSchedsToDelete[i].Status==SchedStatus.Holiday) {
						listHolidaySchedules.Add(listSchedsToDelete[i]);
						listSchedsToDelete.Remove(listSchedsToDelete[i]);//This is a holiday schedule, do not delete it when clearing.
						isHoliday=true;
					}
					if(_weekendFilter==ScheduleWeekendFilter.Weekend 
						&& (listSchedsToDelete[i].SchedDate.DayOfWeek!=DayOfWeek.Saturday && listSchedsToDelete[i].SchedDate.DayOfWeek!=DayOfWeek.Sunday))
					{
						listSchedsToDelete.Remove(listSchedsToDelete[i]);//Remove weekdays on the weekend view
					}
				}
				Schedules.DeleteMany(listSchedsToDelete.Select(x => x.ScheduleNum).ToList());
			}
			Schedule sched;
			int weekDelta=0;
			if(isWeek){
				TimeSpan span=dateSelectedStart-_dateCopyStart;
				weekDelta=span.Days/7;//usually a positive # representing a future paste, but can be negative
			}
			List<Schedule> listSchedulesToInsert=new List<Schedule>();
			for(int i=0;i<listSchedulesToCopy.Count;i++){
				sched=listSchedulesToCopy[i];
				if(isWeek){
					sched.SchedDate=sched.SchedDate.AddDays(weekDelta*7);
				}
				else{
					sched.SchedDate=dateSelectedStart;
				}
				if(listHolidaySchedules.Exists(x => x.SchedDate==sched.SchedDate)) {
					continue;//Don't add schedules to a day that's a holiday.
				}
				if(_weekendFilter==ScheduleWeekendFilter.Weekend 
						&& listSchedulesToCopy[i].SchedDate.DayOfWeek!=DayOfWeek.Saturday
						&& listSchedulesToCopy[i].SchedDate.DayOfWeek!=DayOfWeek.Sunday)
				{
					continue;//Dont add any week days when on weekend mode
				}
				listSchedulesToInsert.Add(sched);
			}
			if(isHoliday) {
				MsgBox.Show(this,"One or more holidays exist in the destination date range.  These will not be replaced.  "
					+"To replace them, remove holiday schedules from the destination date range and repeat this process.");
			}
			Schedules.Insert(false,true,listSchedulesToInsert.ToArray());
			DateTime rememberDateStart=_dateCopyStart;
			DateTime rememberDateEnd=_dateCopyEnd;
			_clickedCell=gridMain.SelectedCell;
			FillGrid();
			_dateCopyStart=rememberDateStart;
			_dateCopyEnd=rememberDateEnd;
			if(isWeek){
				textClipboard.Text=_dateCopyStart.ToShortDateString()+" - "+_dateCopyEnd.ToShortDateString();
			}
			else{
				textClipboard.Text=_dateCopyStart.ToShortDateString();
			}
			changed=true;
			actionCloseScheduleProgress?.Invoke();
		}

		private void butRepeat_Click(object sender,EventArgs e) {
			bool isWeek=(_dateCopyStart!=_dateCopyEnd);
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
			if(repeatCount>1250 && !isWeek) {
				MsgBox.Show(this,"Please enter a number of days less than 1250.");
				return;
			}
			if(repeatCount>250 && isWeek) {
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
			if(_provsChanged) {
				MsgBox.Show(this,"Provider or Employee selection has been changed.  Please refresh first.");
				return;
			}
			Action actionCloseScheduleProgress=ODProgress.Show(ODEventType.Schedule,typeof(ScheduleEvent));
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
			//calculate which day or week is currently selected.
			DateTime dateSelectedStart;
			DateTime dateSelectedEnd;
			if(isWeek) {
				int startI=1;
				if(_weekendFilter==ScheduleWeekendFilter.FullWeek || _weekendFilter==ScheduleWeekendFilter.Weekend) {
					startI=0;
				}
				dateSelectedStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,startI);
				if(_weekendFilter==ScheduleWeekendFilter.FullWeek || _weekendFilter==ScheduleWeekendFilter.Weekend) {
					dateSelectedEnd=dateSelectedStart.AddDays(6);
				}
				else {
					dateSelectedEnd=dateSelectedStart.AddDays(4);
				}
			}
			else {
				int selectedCol=gridMain.SelectedCell.X;
				if(_weekendFilter==ScheduleWeekendFilter.WorkWeek) {
					selectedCol++;//Increment selected day to account for sunday
				}
				else if(_weekendFilter==ScheduleWeekendFilter.Weekend && selectedCol==1) {
					selectedCol=6;//Set selected day to sunday
				}
				dateSelectedStart=Schedules.GetDateCal(_fromDateCur,gridMain.SelectedCell.Y,selectedCol);
				dateSelectedEnd=dateSelectedStart;
			}
			List<long> listProvNums;
			List<long> listEmployeeNums;
			GetSelectedProvidersEmployeesAndClinic(out listProvNums,out listEmployeeNums);
			Logger.LogToPath("RefreshPeriod",LogPath.Signals,LogPhase.Start);
			List<Schedule> listSchedulesToCopy=Schedules.RefreshPeriod(_dateCopyStart,_dateCopyEnd,listProvNums,listEmployeeNums,checkPracticeNotes.Checked,
				checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
			listSchedulesToCopy.RemoveAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday);//Remove holiday schedules from the copy
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
					actionCloseScheduleProgress=ODProgress.Show(ODEventType.Schedule,typeof(ScheduleEvent));
				}
			}
			//Flag every schedule that we are copying as new (because conflict detection requires schedules marked as new)
			listSchedulesToCopy.ForEach(x => x.IsNew=true);
			Logger.LogToPath("RefreshPeriod",LogPath.Signals,LogPhase.End);
			Schedule sched;
			int weekDelta=0;
			TimeSpan span;
			if(isWeek) {
				span=dateSelectedStart-_dateCopyStart;
				weekDelta=span.Days/7;//usually a positive # representing a future paste, but can be negative
			}
			int dayDelta=0;//this is needed when repeat pasting days in order to calculate skipping weekends.
			//Get the official list of schedules that are going to be copied.
			//Always check for overlapping schedules regardless of checkReplace.Checked.
			if(checkWarnProvOverlap.Checked) {
				//Only check overlapping provider schedules.
				List<Schedule> listProvSchedules=listSchedulesToCopy.FindAll(x => x.SchedType==ScheduleType.Provider);
				List<long> listOverlappingProvNums=new List<long>();
				DateTime dateStart;
				DateTime dateEnd;
				for(int i=0;i<repeatCount;i++) {
					if(isWeek) {
						dateStart=dateSelectedStart.AddDays(i*7);
						dateEnd=dateSelectedEnd.AddDays(i*7);
					}
					else {
						dateStart=dateSelectedStart.AddDays(dayDelta);
						dateEnd=dateSelectedEnd.AddDays(dayDelta);
					}
					listOverlappingProvNums=listOverlappingProvNums
						.Union(Schedules.GetOverlappingSchedProvNumsForRange(listProvSchedules,dateStart,dateEnd
							,listIgnoreProvNums: (checkReplace.Checked ? listProvNums : null)))
						.ToList();
					if(_weekendFilter==ScheduleWeekendFilter.WorkWeek && dateSelectedStart.AddDays(dayDelta).DayOfWeek==DayOfWeek.Friday) {
						dayDelta+=3;//Skip to the following monday
					}
					else if(_weekendFilter==ScheduleWeekendFilter.Weekend && dateSelectedStart.AddDays(dayDelta).DayOfWeek==DayOfWeek.Sunday) {
						dayDelta+=6;//Skip to the following saturday
					}
					else {
						dayDelta++;
					}
				}
				if(listOverlappingProvNums.Count > 0) {
					actionCloseScheduleProgress?.Invoke();
					if(MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")
						+"\r\n"+Lan.g(this,"Providers affected")
						+":\r\n  "+string.Join("\r\n  ",listOverlappingProvNums.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
					{
						return;
					}
					actionCloseScheduleProgress=ODProgress.Show(ODEventType.Schedule,typeof(ScheduleEvent));
				}
			}
			Logger.LogToPath("ScheduleUpsert",LogPath.Signals,LogPhase.Start,"repeatCount: "+repeatCount.ToString());
			List<Schedule> listSchedulesToInsert=new List<Schedule>();
			List<long> listSchedulesToDelete=new List<long>();
			dayDelta=0;//this is needed when repeat pasting days in order to calculate skipping weekends.
			//dayDelta will start out zero and increment separately from r.
			bool isHoliday=false;
			for(int r=0;r<repeatCount;r++){//for example, user wants to repeat 3 times.
				List<Schedule> listHolidaySchedules=new List<Schedule>();
				if(checkReplace.Checked) {
					List<Schedule> listSchedsToDelete=new List<Schedule>();
					if(isWeek){
						Logger.LogToPath("isWeek.Schedules.Clear",LogPath.Signals,LogPhase.Start);
						listSchedsToDelete=Schedules.GetSchedulesToDelete(dateSelectedStart.AddDays(r*7),dateSelectedEnd.AddDays(r*7),listProvNums,
							listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
						Logger.LogToPath("isWeek.Schedules.Clear",LogPath.Signals,LogPhase.End);
					}
					else{
						Logger.LogToPath("!isWeek.Schedules.Clear",LogPath.Signals,LogPhase.Start);
					  listSchedsToDelete=Schedules.GetSchedulesToDelete(dateSelectedStart.AddDays(dayDelta),dateSelectedEnd.AddDays(dayDelta),
							listProvNums,listEmployeeNums,checkPracticeNotes.Checked,checkClinicNotes.Checked,comboClinic.SelectedClinicNum);
						Logger.LogToPath("!isWeek.Schedules.Clear",LogPath.Signals,LogPhase.End);
					}
					for(int i=listSchedsToDelete.Count-1;i>=0;i--) {//When pasting, do not paste over a holiday schedule.
						if(listSchedsToDelete[i].SchedType==ScheduleType.Practice && listSchedsToDelete[i].Status==SchedStatus.Holiday) {
							listHolidaySchedules.Add(listSchedsToDelete[i]);
							listSchedsToDelete.Remove(listSchedsToDelete[i]);//This is a holiday schedule, do not delete it when clearing.
							isHoliday=true;
						}
						if(_weekendFilter==ScheduleWeekendFilter.Weekend 
							&& listSchedsToDelete[i].SchedDate.DayOfWeek!=DayOfWeek.Saturday 
							&& listSchedsToDelete[i].SchedDate.DayOfWeek!=DayOfWeek.Sunday)
						{
							listSchedsToDelete.Remove(listSchedsToDelete[i]);//Remove weekdays on the weekend view
						}
					}
					listSchedulesToDelete.AddRange(listSchedsToDelete.Select(x => x.ScheduleNum).ToList());
				}
				Logger.LogToPath("SchedList.Insert",LogPath.Signals,LogPhase.Start,"SchedList.Count: "+listSchedulesToCopy.Count.ToString());
				for(int i=0;i<listSchedulesToCopy.Count;i++) {//For example, if 3 weeks for one provider, then about 30 loops.
					sched=listSchedulesToCopy[i].Copy();
					if(isWeek) {
						sched.SchedDate=sched.SchedDate.AddDays((weekDelta+r)*7).AddHours(1).Date;
					}
					else {
						sched.SchedDate=dateSelectedStart.AddDays(dayDelta);
					}
					if(listHolidaySchedules.Exists(x => x.SchedDate==sched.SchedDate)) {
						continue;//Don't add a new schedule to a day that's a holiday.
					}
					if(_weekendFilter==ScheduleWeekendFilter.Weekend 
							&& listSchedulesToCopy[i].SchedDate.DayOfWeek!=DayOfWeek.Saturday
							&& listSchedulesToCopy[i].SchedDate.DayOfWeek!=DayOfWeek.Sunday)
					{
						continue;//Dont add any week days when on weekend mode
					}
					listSchedulesToInsert.Add(sched);
				}
				Logger.LogToPath("SchedList.Insert",LogPath.Signals,LogPhase.End);		
				if(_weekendFilter==ScheduleWeekendFilter.WorkWeek && dateSelectedStart.AddDays(dayDelta).DayOfWeek==DayOfWeek.Friday) {
					dayDelta+=3;//Skip to the following monday
				}
				else if(_weekendFilter==ScheduleWeekendFilter.Weekend && dateSelectedStart.AddDays(dayDelta).DayOfWeek==DayOfWeek.Sunday) {
					dayDelta+=6;//Skip to the following saturda	}
				}
				else {
					dayDelta++;
				}
			}
			if(isHoliday) {
				MsgBox.Show(this,"One or more holidays exist in the destination date range.  These will not be replaced.  "
					+"To replace them, remove holiday schedules from the destination date range and repeat this process.");
			}
			Schedules.DeleteMany(listSchedulesToDelete);
			Schedules.Insert(false,true,listSchedulesToInsert.ToArray());
			Logger.LogToPath("ScheduleUpsert",LogPath.Signals,LogPhase.End);
			DateTime rememberDateStart=_dateCopyStart;
			DateTime rememberDateEnd=_dateCopyEnd;
			_clickedCell=gridMain.SelectedCell;
			FillGrid();
			_dateCopyStart=rememberDateStart;
			_dateCopyEnd=rememberDateEnd;
			if(isWeek) {
				textClipboard.Text=_dateCopyStart.ToShortDateString()+" - "+_dateCopyEnd.ToShortDateString();
			}
			else {
				textClipboard.Text=_dateCopyStart.ToShortDateString();
			}
			changed=true;
			actionCloseScheduleProgress?.Invoke();
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Staff schedule printed"));
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Schedule");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
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
			_clickedCell=gridMain.SelectedCell;
			_isResizing=true;
		}

		///<summary>Fires when resizing is complete, except when changing window state. I.e. this is not fired when the window is maximized or minimized.</summary>
		private void FormSchedule_ResizeEnd(object sender,EventArgs e) {
			FillGrid(false);
			_isResizing=false;
		}

		private void FormSchedule_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed){
				SecurityLogs.MakeLogEntry(Permissions.Schedules,0,"");
			}
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





















