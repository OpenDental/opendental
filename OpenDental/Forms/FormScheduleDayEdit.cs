using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	///<summary></summary>
	public partial class FormScheduleDayEdit:FormODBase {
		private DateTime _dateSched;
		///<summary>Working copy of schedule entries.</summary>
		private List<Schedule> _listSchedules;
		///<summary>Stale copy of schedule entries.</summary>
		private List<Schedule> _listSchedulesOld;
		private List<Provider> _listProviders;
		private List<Employee> _listEmployees;
		///<summary>Only used in schedule sorting. Greatly increases speed of large databases.</summary>
		private List<Employee> _listEmployeesSort;
		///<summary>Only used in schedule sorting. Greatly increases speed of large databases.</summary>
		private List<Provider> _listProvidersSort;
		private List<Clinic> _listClinics;
		public bool ShowOkSchedule = false;
		///<summary>Set by butOkSchedule only.</summary>
		public bool GotoScheduleOnClose = false;
		private List<long> _listProvNumsSelected;
		///<summary>True if the called from FromSchedule, else False. </summary>
		private bool _isFromSchedule;
		///<summary>The provider that was selected in FormSchedule. Will be blank if no provider or multiple providers were selected. </summary>
		private string _provAbbrFilter;
		///<summary>The employee that was selected in FormSchedule. Will be blank if no employee or multiple employees were selected. </summary>
		private string _employeeNameFilter;
		///<summary>Only used once, on startup.</summary>
		private long _clinicNumInitial;

		///<summary></summary>
		public FormScheduleDayEdit(DateTime dateSched) : this(dateSched,0) {
		}

		///<summary>When clinics are enabled, this will filter the employee list box by the clinic passed in.  Pass clinicNum 0 to only show employees not assigned to a clinic.</summary>
		public FormScheduleDayEdit(DateTime dateSched,long clinicNum,string provAbbr="",string empFName="",bool isFromSchedule=false) {
			InitializeComponent();
			InitializeLayoutManager();
			_dateSched=dateSched.Date;
			_clinicNumInitial=clinicNum;
			_isFromSchedule=isFromSchedule;
			_provAbbrFilter=provAbbr;
			_employeeNameFilter=empFName;
			Lan.F(this);
		}

		private void FormScheduleDay_Load(object sender,System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),textSearch);
			butViewGraph.Visible=PrefC.IsODHQ;
			butOkSchedule.Visible=ShowOkSchedule;
			_listClinics=new List<Clinic>();
			_listClinics.Add(new Clinic() { Abbr="" });//so HQ always comes before other clinics in the sort order; only for notes and holidays
			List<Clinic> listClinicsUser=Clinics.GetForUserod(Security.CurUser);
			for(int i=0;i<listClinicsUser.Count;i++){
				_listClinics.Add(listClinicsUser[i]);
			}
			comboClinic.SelectedClinicNum=_clinicNumInitial;
			comboClinicChanged();//fills provs and emps and also fills grid
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(PrefC.GetLong(PrefName.ScheduleProvUnassigned));
			labelDate.Text=_dateSched.ToString("dddd")+"\r\n"+_dateSched.ToShortDateString();
			if(_isFromSchedule && (_provAbbrFilter!="" || _employeeNameFilter!="")) {//single person was passed in from the schedule window.
				if(_provAbbrFilter!="") {//it was a provider schedule
					textSearch.Text=_provAbbrFilter;
				}
				else {//it was an employee schedule
					textSearch.Text=_employeeNameFilter;
				}
			}
			FillGrid(); //Fill grid with text from search bar automatically
			textSearch.Select();//focus and select the filter so it can be easily changed
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			comboClinicChanged();
		}

		private void comboClinicChanged(){
			long clinicNumOrig=comboClinic.SelectedClinicNum;
			this.Text=Lan.g(this,"Edit Day")+" - "+comboClinic.Text;
			if(comboClinic.SelectedClinicNum==0) {
				groupPractice.Text=Lan.g(this,"Practice");
			}
			else {
				groupPractice.Text=Lan.g(this,"Clinic");
			}
			if(_listSchedules!=null) {
				//skips on startup
				//Sync changes to Db because we are going to change our in memory lists.
				try {
					Schedules.SetForDay(_listSchedules,_listSchedulesOld);
				}
				catch(Exception ex) {
					MsgBox.Show(this,ex.Message);
					return;
				}
			}
			//Fill lists with new information from new clinic
			FillProvsAndEmps();
			_listSchedules=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProviders.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmployees.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedulesOld=_listSchedules.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void FillProvsAndEmps() {
			tabPageProv.Text=Lan.g(this,"Providers")+" (0)";
			tabPageEmp.Text=Lan.g(this,"Employees")+" (0)";
			//Seed emp list and prov list with a dummy emp/prov with 'none' for the field that fills the list, FName and Abbr respectively.
			//That way we don't have to add/subtract one in order when selecting from the list based on selected indexes.
			Employee employee = new Employee();
			employee.EmployeeNum=0;
			employee.FName="none";
			_listEmployees=new List<Employee>() {employee};
			Provider provider = new Provider();
			provider.ProvNum=0;
			provider.Abbr="none";
			_listProviders=new List<Provider>() {provider};
			if(PrefC.HasClinicsEnabled) {
				_listProviders.AddRange(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
				_listEmployees.AddRange(Employees.GetEmpsForClinic(comboClinic.SelectedClinicNum));
			}
			else {
				_listProviders.AddRange(Providers.GetDeepCopy(true));
				_listEmployees.AddRange(Employees.GetDeepCopy(true));
			}
			//Prov Listbox
			List<long> listProvNumsPreviouslySelected=listProv.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			listProv.Items.Clear();
			listProv.Items.AddList(_listProviders,x => x.Abbr);
			for(int i=0; i<_listProviders.Count;i++) {
				if(listProvNumsPreviouslySelected.Contains(_listProviders[i].ProvNum)) {
					listProv.SetSelected(i,true);
				}
			}
			if(listProv.SelectedIndices.Count==0) {
				listProv.SelectedIndex=0;//select the 'none' entry
			}
			//Emp Listbox
			List<long> listEmployeeNumsPreviouslySelected=listEmp.GetListSelected<Employee>().Select(x => x.EmployeeNum).ToList();
			listEmp.Items.Clear();
			listEmp.Items.AddList(_listEmployees,x => x.FName);
			for(int i=0; i<_listEmployees.Count;i++) {
				if(listEmployeeNumsPreviouslySelected.Contains(_listEmployees[i].EmployeeNum)) {
					listEmp.SetSelected(i,true);
				}
			}
			if(listEmp.SelectedIndices.Count==0) {
				listEmp.SelectedIndex=0;//select the 'none' entry
			}
		}

		private void listProv_SelectedIndexChanged(object sender,EventArgs e) {
			tabPageProv.Text=Lan.g(this,"Providers")+" ("+listProv.SelectedIndices.OfType<int>().Count(x => x>0)+")";
		}

		private void listEmp_SelectedIndexChanged(object sender,EventArgs e) {
			tabPageEmp.Text=Lan.g(this,"Employees")+" ("+listEmp.SelectedIndices.OfType<int>().Count(x => x>0)+")";
		}

		private void FillGrid() {
			_listEmployeesSort=_listSchedules.Select(x=>x.EmployeeNum).Distinct().Select(x=>Employees.GetEmp(x))//returns null if EmployeeNum==0 or invalid
				.Where(x=>x!=null).ToList();//speed up sort.
			_listProvidersSort=_listSchedules.Select(x=>x.ProvNum).Distinct().Select(x=>Providers.GetProv(x))//returns null if ProvNum==0 or invalid
				.Where(x=>x!=null).ToList();//speed up sort.
			if(PrefC.IsODHQ) {
				//HQ wants their own sort, so instead of complicating the comparer we will just do the comparer on four seperate lists.
				List<Schedule> listSchedulesPracticeNotes=_listSchedules.Where(x => x.EmployeeNum==0 && x.ProvNum==0).ToList();
				listSchedulesPracticeNotes.Sort(CompareSchedule);
				List<Schedule> listSchedulesEmpNotes=_listSchedules.Where(x => x.EmployeeNum!=0 && x.ProvNum==0 && x.StartTime==TimeSpan.Zero).ToList();
				listSchedulesEmpNotes.Sort(CompareSchedule);
				List<Schedule> listSchedulesProv=_listSchedules.Where(x => x.EmployeeNum==0 && x.ProvNum!=0).ToList();
				listSchedulesProv.Sort(CompareSchedule);
				List<Schedule> listSchedulesEmp=_listSchedules.Where(x => x.EmployeeNum!=0 && x.ProvNum==0 && x.StartTime!=TimeSpan.Zero).ToList();
				listSchedulesEmp.Sort(CompareSchedule);
				_listSchedules=new List<Schedule>();
				_listSchedules.AddRange(listSchedulesPracticeNotes);
				_listSchedules.AddRange(listSchedulesEmpNotes);
				_listSchedules.AddRange(listSchedulesProv);
				_listSchedules.AddRange(listSchedulesEmp);
				_listSchedules.Distinct();
			}
			else {
				_listSchedules.Sort(CompareSchedule);
			}
			graphScheduleDay.SetSchedules(_listSchedules);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableSchedDay","Provider"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Employee"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Start Time"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Stop Time"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Ops"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Note"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string note;
			string opDesc;
			for (int i=0;i<_listSchedules.Count;i++) {
				row=new GridRow();
				//Prov
				if(_listSchedules[i].ProvNum==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Providers.GetAbbr(_listSchedules[i].ProvNum));
				}
				//Employee
				if(_listSchedules[i].EmployeeNum==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Employees.GetEmp(_listSchedules[i].EmployeeNum).FName);
				}
				//times
				if(_listSchedules[i].StartTime==TimeSpan.Zero && _listSchedules[i].StopTime==TimeSpan.Zero) {
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listSchedules[i].StartTime.ToShortTimeString());
					row.Cells.Add(_listSchedules[i].StopTime.ToShortTimeString());
				}
				//ops
				opDesc="";
				for (int j=0;j<_listSchedules[i].Ops.Count;j++) {
					Operatory operatory=Operatories.GetOperatory(_listSchedules[i].Ops[j]);
					if(operatory==null || operatory.IsHidden) {//Skip hidden operatories because it just confuses users.
						continue;
					}
					if(opDesc!="") {
						opDesc+=",";
					}
					opDesc+=operatory.Abbrev;
				}
				row.Cells.Add(opDesc);
				//note
				note="";
				if(_listSchedules[i].SchedType==ScheduleType.Practice) {//note or holiday
					string clinicAbbr="";
					if(PrefC.HasClinicsEnabled) {
						clinicAbbr=Clinics.GetAbbr(_listSchedules[i].ClinicNum);
						if(string.IsNullOrEmpty(clinicAbbr)) {
							clinicAbbr="Headquarters";
						}
						clinicAbbr=" ("+clinicAbbr+")";
						if(_listSchedules[i].Status!=SchedStatus.Holiday) {//must be a Note, only add 'Note' if clinics are enabled
							note=Lan.g(this,"Note")+clinicAbbr+": ";
						}
					}
					if(_listSchedules[i].Status==SchedStatus.Holiday) {
						note=Lan.g(this,"Holiday")+clinicAbbr+": ";
					}
				}
				note+=_listSchedules[i].Note;
				row.Cells.Add(note);
				row.Tag=_listSchedules[i];
				//Do not add the row if the user typed something into the search box and no cell contains the text that was typed in.
				string searchTextLower=textSearch.Text.ToLower().Trim();
				if(!string.IsNullOrEmpty(searchTextLower)) {
					//Go through every cell in the row and make sure that there is at least one cell that contains the search text.
					bool hasMatch=false;
					for (int j=0;j<row.Cells.Count;j++) {
						if(row.Cells[j].Text.ToLower().Trim().Contains(searchTextLower)) {
							hasMatch=true;
							break;
						}
					}
					if(!hasMatch) {
						continue;//Do not add this row to gridMain.
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private int CompareSchedule(Schedule scheduleX,Schedule scheduleY){
			if(scheduleX==scheduleY){//also handles null==null
				return 0;
			}
			if(scheduleY==null){
				return 1;
			}
			if(scheduleX==null){
				return -1;
			}
			if(scheduleX.SchedType!=scheduleY.SchedType){
				return scheduleX.SchedType.CompareTo(scheduleY.SchedType);
			}
			if(scheduleX.ProvNum!=scheduleY.ProvNum){
				return _listProvidersSort.Find(x=>x.ProvNum == scheduleX.ProvNum).ItemOrder.CompareTo(_listProvidersSort.Find(x=>x.ProvNum == scheduleY.ProvNum).ItemOrder);
			}
			if(scheduleX.EmployeeNum!=scheduleY.EmployeeNum) {
				Employee employeeX = _listEmployeesSort.Find(x=>x.EmployeeNum==scheduleX.EmployeeNum);
				Employee employeeY = _listEmployeesSort.Find(x=>x.EmployeeNum==scheduleY.EmployeeNum);
				if(employeeX.FName!=employeeY.FName) {
					return employeeX.FName.CompareTo(employeeY.FName);
				}
				if(employeeX.LName!=employeeY.LName) {
					return employeeX.LName.CompareTo(employeeY.LName);
				}
				return scheduleX.EmployeeNum.CompareTo(scheduleY.EmployeeNum);
			}
			if(scheduleX.StartTime!=scheduleY.StartTime) {
				return scheduleX.StartTime.CompareTo(scheduleY.StartTime);
			}
			if(scheduleX.ClinicNum!=scheduleY.ClinicNum
				&& _listClinics.Exists(x=>x.ClinicNum == scheduleX.ClinicNum)
				&& _listClinics.Exists(x=>x.ClinicNum == scheduleY.ClinicNum))
			{
				return _listClinics.Find(x=>x.ClinicNum == scheduleX.ClinicNum).Abbr.CompareTo(_listClinics.Find(x=>x.ClinicNum == scheduleY.ClinicNum).Abbr);
			}
			if(!scheduleX.Status.Equals(scheduleY.Status)) {
				return -scheduleX.Status.CompareTo(scheduleY.Status);//holiday to the top
			}
			if(scheduleX.Note!=scheduleY.Note) {
				return scheduleX.Note.CompareTo(scheduleY.Note);
			}
			if(scheduleX.ScheduleNum!=scheduleY.ScheduleNum) {
				return scheduleX.ScheduleNum.CompareTo(scheduleY.ScheduleNum);
			}
			return scheduleX.GetHashCode().CompareTo(scheduleY.GetHashCode()); //to ensure deterministric sorting, even when PK==0
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Schedule selectedSchedule=(Schedule)gridMain.ListGridRows[e.Row].Tag;
			if(selectedSchedule==null) {
				return;//nothing to do
			}
			using FormScheduleEdit formScheduleEdit=new FormScheduleEdit();
			//Remove clicked on date so that it does not cause itself to be blocked if holiday.
			formScheduleEdit.ListSchedules=_listSchedules.FindAll(x => x!=selectedSchedule)
				.Select(x => x.Copy()).ToList();//Deep copy for safety
			formScheduleEdit.ScheduleCur=selectedSchedule;
			formScheduleEdit.ScheduleCur.IsNew=false;
			formScheduleEdit.ClinicNum=comboClinic.SelectedClinicNum;
			formScheduleEdit.ListProvNums=new List<long>();
			if(selectedSchedule.ProvNum>0) {//Don't look for conflicts against a schedule with no provider.
				formScheduleEdit.ListProvNums.Add(selectedSchedule.ProvNum);
			}
			formScheduleEdit.ShowDialog();
			if(formScheduleEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			//Sync changes because the user may have changed the clinic of a schedule in form
			try {
				Schedules.SetForDay(_listSchedules,_listSchedulesOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			_listSchedules=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProviders.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmployees.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedulesOld=_listSchedules.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		///<summary>Returns true if date text boxes have no errors and the emp and prov lists don't have 'none' selected with other emps/provs.
		///Set isQuiet to true to suppress the message box with the warning.</summary>
		private bool ValidateLists() {
			List<string> listErrorMsgs=new List<string>();
			if(listProv.SelectedIndices.Count>1 && listProv.SelectedIndices.Contains(0)) {//'none' selected with additional provs
				listErrorMsgs.Add(Lan.g(this,"Invalid selection of providers."));
			}
			if(listEmp.SelectedIndices.Count>1 && listEmp.SelectedIndices.Contains(0)) {//'none' selected with additional emps
				listErrorMsgs.Add(Lan.g(this,"Invalid selection of employees."));
			}
			if(listProv.SelectedIndices.OfType<int>().Count(x => x>0)==0 && listEmp.SelectedIndices.OfType<int>().Count(x => x>0)==0) {
				listErrorMsgs.Add(Lan.g(this,"Please select at least one provider or one employee first."));
			}
			if(listErrorMsgs.Count>0) {
				MessageBox.Show(string.Join("\r\n",listErrorMsgs));
			}
			if(listErrorMsgs.Count==0 //only perform this check if everything else is okay.
				&& listProv.SelectedIndices.Count>0 && !listProv.SelectedIndices.Contains(0) //at least one valid provider selected
				&& listEmp.SelectedIndices.Count>0 && !listEmp.SelectedIndices.Contains(0) //at least one valid employee selected
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Both providers and employees are selected, would you like to continue?"))
			{
				return false;
			}
			return listErrorMsgs.Count==0;
		}

		private void butAddTime_Click(object sender, System.EventArgs e) {
			if(!ValidateLists()) {
				return;
			}
			_listProvNumsSelected=new List<long>();//list of provNums selected
			if(!listProv.SelectedIndices.Contains(0)) {// Add all selected providers to the provider number list
				List<int> listSelectedIndices=listProv.SelectedIndices.OfType<int>().ToList();
				for(int i=0;i<listSelectedIndices.Count();i++) {
					_listProvNumsSelected.Add(_listProviders[listSelectedIndices[i]].ProvNum);
				}
			}
			Schedule schedule=new Schedule();
			schedule.SchedDate=_dateSched;
			schedule.Status=SchedStatus.Open;
			schedule.StartTime=new TimeSpan(8,0,0);//8am
			schedule.StopTime=new TimeSpan(17,0,0);//5pm
			//schedtype, provNum, and empnum will be set down below
			using FormScheduleEdit formScheduleEdit=new FormScheduleEdit();
			formScheduleEdit.ScheduleCur=schedule;
			formScheduleEdit.ListSchedules=_listSchedules;
			formScheduleEdit.ListProvNums=_listProvNumsSelected;
			formScheduleEdit.ScheduleCur.IsNew=true;
			formScheduleEdit.ClinicNum=comboClinic.SelectedClinicNum;
			formScheduleEdit.ShowDialog();
			if(formScheduleEdit.DialogResult!=DialogResult.OK){
				return;
			}
			Schedule scheduleTemp;
			for(int i=0;i<listProv.SelectedIndices.Count;i++){
				if(listProv.SelectedIndices[i]==0) {
					continue;
				}
				scheduleTemp=new Schedule();
				scheduleTemp=schedule.Copy();
				scheduleTemp.SchedType=ScheduleType.Provider;
				scheduleTemp.ProvNum=_listProviders[listProv.SelectedIndices[i]].ProvNum;
				_listSchedules.Add(scheduleTemp);
			}
			for(int i=0;i<listEmp.SelectedIndices.Count;i++) {
				if(listEmp.SelectedIndices[i]==0) {
					continue;
				}
				scheduleTemp=new Schedule();
				scheduleTemp=schedule.Copy();
				scheduleTemp.SchedType=ScheduleType.Employee;
				scheduleTemp.EmployeeNum=_listEmployees[listEmp.SelectedIndices[i]].EmployeeNum;
				_listSchedules.Add(scheduleTemp);
			}
			FillGrid();
		}

		private void butProvNote_Click(object sender,EventArgs e) {
			if(!ValidateLists()) {
				return;
			}
			Schedule schedule=new Schedule();
			schedule.SchedDate=_dateSched;
			schedule.Status=SchedStatus.Open;
			//this is so we can differentiate between a practice note and a prov/emp note in FormScheduleEdit.  Updated if necessary below when inserting.
			schedule.SchedType=ScheduleType.Provider;
			//schedtype, provNum, and empnum will be set down below
			using FormScheduleEdit formScheduleEdit=new FormScheduleEdit();
			formScheduleEdit.ScheduleCur=schedule;
			formScheduleEdit.ClinicNum=comboClinic.SelectedClinicNum;
			formScheduleEdit.ShowDialog();
			if(formScheduleEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			Schedule scheduleTemp;
			List<int> listProvIndices=new List<int>();
			listProvIndices = listProv.SelectedIndices.OfType<int>().Where(x=>x>0).ToList();
			for (int i=0;i<listProvIndices.Count;i++) {
				scheduleTemp=new Schedule();
				scheduleTemp=schedule.Copy();
				scheduleTemp.SchedType=ScheduleType.Provider;
				scheduleTemp.ProvNum=_listProviders[listProvIndices[i]].ProvNum;
				_listSchedules.Add(scheduleTemp);
			}
			List<int> listEmpIndices=new List<int>();
			listEmpIndices = listEmp.SelectedIndices.OfType<int>().Where(x=>x>0).ToList();
			for (int i=0;i<listEmpIndices.Count;i++) {
				scheduleTemp=new Schedule();
				scheduleTemp=schedule.Copy();
				scheduleTemp.SchedType=ScheduleType.Employee;
				scheduleTemp.EmployeeNum=_listEmployees[listEmpIndices[i]].EmployeeNum;
				_listSchedules.Add(scheduleTemp);
			}
			FillGrid();
		}

		private void butNote_Click(object sender,EventArgs e) {
			Schedule schedule=new Schedule();
			schedule.SchedDate=_dateSched;
			schedule.Status=SchedStatus.Open;
			schedule.SchedType=ScheduleType.Practice;
			schedule.ClinicNum=comboClinic.SelectedClinicNum;
			using FormScheduleEdit formScheduleEdit=new FormScheduleEdit();
			formScheduleEdit.ScheduleCur=schedule;
			formScheduleEdit.ClinicNum=comboClinic.SelectedClinicNum;
			formScheduleEdit.ShowDialog();
			if(formScheduleEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_listSchedules.Add(schedule);
			FillGrid();
		}

		private void butHoliday_Click(object sender,System.EventArgs e) {
			Schedule schedule=new Schedule();
			schedule.SchedDate=_dateSched;
			schedule.Status=SchedStatus.Holiday;
			schedule.SchedType=ScheduleType.Practice;
			schedule.ClinicNum=comboClinic.SelectedClinicNum;
			using FormScheduleEdit formScheduleEdit=new FormScheduleEdit();
			formScheduleEdit.ListSchedules=_listSchedules;
			formScheduleEdit.ScheduleCur=schedule;
			formScheduleEdit.ClinicNum=comboClinic.SelectedClinicNum;
			formScheduleEdit.ShowDialog();
			if(formScheduleEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_listSchedules.Add(schedule);
			FillGrid();
		}

		private void butBack_Click(object sender,EventArgs e) {
			//Sync changes because we are going to change our in memory lists.
			try {
				Schedules.SetForDay(_listSchedules,_listSchedulesOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			_dateSched=_dateSched.AddDays(-1);
			labelDate.Text=_dateSched.ToString("dddd")+"\r\n"+_dateSched.ToShortDateString();
			//Fill lists with new information from new clinic
			FillProvsAndEmps();
			_listSchedules=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProviders.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmployees.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedulesOld=_listSchedules.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void butForward_Click(object sender,EventArgs e) {
			//Sync changes because we are going to change our in memory lists.
			try {
				Schedules.SetForDay(_listSchedules,_listSchedulesOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			_dateSched=_dateSched.AddDays(1);
			labelDate.Text=_dateSched.ToString("dddd")+"\r\n"+_dateSched.ToShortDateString();
			//Fill lists with new information from new clinic
			FillProvsAndEmps();
			_listSchedules=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProviders.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmployees.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedulesOld=_listSchedules.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			int countProviders;
			//Select all rows if none are currently selected.
			if(gridMain.SelectedIndices.Length==0) {
				gridMain.SetAll(true);
			}
			List<Schedule> listSchedulesToRemove=gridMain.SelectedTags<Schedule>();
			if(listSchedulesToRemove.IsNullOrEmpty()) {
				return;//Nothing to do.
			}
			countProviders=listSchedulesToRemove.Where(x => x.ProvNum!=0).Select(y => y.ProvNum).Distinct().Count();
			if(countProviders>0) {
				string message=Lan.g(this,"Delete schedules on this day for")+" "+countProviders+" "+Lan.g(this,"provider(s)?");
				if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
			}
			_listSchedules.RemoveAll(x => listSchedulesToRemove.Contains(x));
			FillGrid();
		}

		private void butOkSchedule_Click(object sender,EventArgs e) {
			GotoScheduleOnClose=true;
			butOK_Click(sender,e);
		}

		private void butViewGraph_Click(object sender,EventArgs e) {
			using FormGraphEmployeeTime formGraphEmployeeTime=new FormGraphEmployeeTime(_dateSched);
			formGraphEmployeeTime.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			try {
				Schedules.SetForDay(_listSchedules,_listSchedulesOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			if(comboProv.SelectedIndex!=-1
				&& Prefs.UpdateLong(PrefName.ScheduleProvUnassigned,comboProv.GetSelectedProvNum()))
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e){
			LayoutManager.LayoutControlBoundsAndFonts(tabControl1);
		}
	}
}

