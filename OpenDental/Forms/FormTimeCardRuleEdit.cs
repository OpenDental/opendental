using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTimeCardRuleEdit:FormODBase {
		
		///<summary>True when not editing an existing TimeCardRule. 
		///When true a new timeCardRule will be inserted for every selected employee.</summary>
		private bool _isInsertMode;
		private TimeCardRule _timeCardRule;
		private List<Employee> _listEmployees;

		///<summary>Provide timeCardeRule to edit. Otherwise will insert new entry for every selected Employee.
		///This form is used to edit an existing timeCardRule or insert many new timeCardRules.</summary>
		public FormTimeCardRuleEdit(TimeCardRule timeCardRule=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_timeCardRule=timeCardRule;
			if(_timeCardRule==null) {//No timeCardRule provided, we must be inserting new entries.
				_timeCardRule=new TimeCardRule();//Allows Load(...) to set empty values.
				_isInsertMode=true;
				listEmp.SelectionMode=OpenDental.UI.SelectionMode.MultiExtended;//When adding new entries allow the user to insert many new rows.
			}
		}

		private void FormTimeCardRuleEdit_Load(object sender,EventArgs e) {
			if(!PrefC.HasClinicsEnabled) {
				_listEmployees=Employees.GetForTimeCard();
			}
			else {
				_listEmployees=Employees.GetEmpsForClinic(Clinics.ClinicNum);
			}
			listEmp.Items.Add(Lan.g(this,"All Employees"));
			listEmp.SetSelected(0);
			for(int i=0;i<_listEmployees.Count;i++) {
				listEmp.Items.Add(_listEmployees[i].FName+" "+_listEmployees[i].LName);
				if(_listEmployees[i].EmployeeNum==_timeCardRule.EmployeeNum) {
					listEmp.SelectedIndex=listEmp.Items.Count-1;
				}
			}
			textOverHoursPerDay.Text=_timeCardRule.OverHoursPerDay.ToStringHmm();
			textAfterTimeOfDay.Text=_timeCardRule.AfterTimeOfDay.ToStringHmm();
			textBeforeTimeOfDay.Text=_timeCardRule.BeforeTimeOfDay.ToStringHmm();
			checkIsOvertimeExempt.Checked=_timeCardRule.IsOvertimeExempt;
			textClockInMin.Text=_timeCardRule.MinClockInTime.ToStringHmm();
		}

		private void but5pm_Click(object sender,EventArgs e) {
			DateTime dt=new DateTime(2010,1,1,17,0,0);
			textAfterTimeOfDay.Text=dt.ToShortTimeString();
		}

		private void but6am_Click(object sender,EventArgs e) {
			DateTime dt=new DateTime(2010,1,1,6,0,0);
			textBeforeTimeOfDay.Text=dt.ToShortTimeString();
		}

		///<summary>If entering text in overtime, clear differential text boxes.</summary>
		private void textOverHoursPerDay_TextChanged(object sender,EventArgs e) {
			if(textOverHoursPerDay.Text!="") {
				textAfterTimeOfDay.Text="";
				textBeforeTimeOfDay.Text="";
			}
		}

		///<summary>If entering text in differential boxes, clear overtime text box.</summary>
		private void textBeforeTimeOfDay_TextChanged(object sender,EventArgs e) {
			if(textBeforeTimeOfDay.Text!="") {
				textOverHoursPerDay.Text="";
			}
		}

		///<summary>If entering text in differential boxes, clear overtime text box.</summary>
		private void textAfterTimeOfDay_TextChanged(object sender,EventArgs e) {
			if(textAfterTimeOfDay.Text!="") {
				textOverHoursPerDay.Text="";
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_isInsertMode) {//Creating new TimeCardRules, nothing to delete.
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to delete this time card rule?")){
				return;
			}
			TimeCardRules.Delete(_timeCardRule.TimeCardRuleNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Verify Data.
			if(listEmp.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select an employee.");
				return;
			}
			if(listEmp.SelectedIndices.Count>1 && listEmp.SelectedIndices.Contains(0)) {
				MsgBox.Show(this,"Cannot select both 'All Employees' and individual employees.");
				return;
			}
			TimeSpan overHoursPerDay=TimeSpan.Zero;
			if(textOverHoursPerDay.Text!="") {
				try {
					if(textOverHoursPerDay.Text.Contains(":")){
						overHoursPerDay=TimeSpan.Parse(textOverHoursPerDay.Text);
					}
					else{
						overHoursPerDay=TimeSpan.FromHours(PIn.Double(textOverHoursPerDay.Text));
					}
				}
				catch {
					MsgBox.Show(this,"Over hours per day invalid.");
					return;
				}
				if(overHoursPerDay==TimeSpan.Zero || overHoursPerDay.Days>0) {
					MsgBox.Show(this,"Over hours per day invalid.");
					return;
				}
			}
			TimeSpan afterTimeOfDay=TimeSpan.Zero;
			if(textAfterTimeOfDay.Text!="") {
				try {
					afterTimeOfDay=DateTime.Parse(textAfterTimeOfDay.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"After time of day invalid.");
					return;
				}
				if(afterTimeOfDay==TimeSpan.Zero || afterTimeOfDay.Days>0) {
					MsgBox.Show(this,"After time of day invalid.");
					return;
				}
			}
			TimeSpan beforeTimeOfDay=TimeSpan.Zero;
			if(textBeforeTimeOfDay.Text!="") {
				try {
					beforeTimeOfDay=DateTime.Parse(textBeforeTimeOfDay.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Before time of day invalid.");
					return;
				}
				if(beforeTimeOfDay==TimeSpan.Zero || beforeTimeOfDay.Days>0) {
					MsgBox.Show(this,"Before time of day invalid.");
					return;
				}
			}
			TimeSpan minClockIn=TimeSpan.Zero;
			if(textClockInMin.Text!="") {
				try {
					minClockIn=DateTime.Parse(textClockInMin.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Earliest Clock in Time invalid.");
					return;
				}
				if(minClockIn==TimeSpan.Zero || minClockIn.Days>0) {
					MsgBox.Show(this,"Earliest Clock in Time invalid.");
					return;
				}
			}
			if(!checkIsOvertimeExempt.Checked 
				&& overHoursPerDay==TimeSpan.Zero 
				&& afterTimeOfDay==TimeSpan.Zero 
				&& beforeTimeOfDay==TimeSpan.Zero 
				&& minClockIn==TimeSpan.Zero)
			{
				MsgBox.Show(this,"Either is overtime exempt, over hours, after or before time of day or Clock In Min must be entered.");
				return;
			}
			//save-------------------------------------------------
			//You are allowed to insert multiple TimeCardRules at one time but can only edit one existing entry at a time.
			if(_isInsertMode) {//Insert new entry for every list selection.
				List<TimeCardRule> listNewRules=new List<TimeCardRule>();
				for(int i=0;i<listEmp.SelectedIndices.Count;i++) {
					_timeCardRule.EmployeeNum=0;
					if(listEmp.SelectedIndices[i]!=0) {//Not 'All'
						_timeCardRule.EmployeeNum=_listEmployees[listEmp.SelectedIndices[i]-1].EmployeeNum;//-1 for All
					}
					_timeCardRule.OverHoursPerDay=overHoursPerDay;
					_timeCardRule.AfterTimeOfDay=afterTimeOfDay;
					_timeCardRule.BeforeTimeOfDay=beforeTimeOfDay;
					_timeCardRule.IsOvertimeExempt=checkIsOvertimeExempt.Checked;
					_timeCardRule.MinClockInTime=minClockIn;
					listNewRules.Add(_timeCardRule.Clone());
				}
				TimeCardRules.InsertMany(listNewRules);
			}
			else {//Update single entry.
				_timeCardRule.EmployeeNum=0;
				if(listEmp.SelectedIndex!=0) {//Not 'All'
					_timeCardRule.EmployeeNum=_listEmployees[listEmp.SelectedIndex-1].EmployeeNum;//-1 for All
				}
				_timeCardRule.OverHoursPerDay=overHoursPerDay;
				_timeCardRule.AfterTimeOfDay=afterTimeOfDay;
				_timeCardRule.BeforeTimeOfDay=beforeTimeOfDay;
				_timeCardRule.IsOvertimeExempt=checkIsOvertimeExempt.Checked;
				_timeCardRule.MinClockInTime=minClockIn;
				TimeCardRules.Update(_timeCardRule);
			}
			DataValid.SetInvalid(InvalidType.TimeCardRules);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}