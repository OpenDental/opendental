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
			if(_timeCardRule!=null) {
				return;
			}
			//No timeCardRule provided, we must be inserting new entries.
			_timeCardRule=new TimeCardRule();//Allows Load(...) to set empty values.
			_isInsertMode=true;
			listEmp.SelectionMode=OpenDental.UI.SelectionMode.MultiExtended;//When adding new entries allow the user to insert many new rows.
		}

		private void FormTimeCardRuleEdit_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				_listEmployees=Employees.GetEmpsForClinic(Clinics.ClinicNum);
			}
			else {
				_listEmployees=Employees.GetForTimeCard();
				
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
			checkUseRate3.Checked=_timeCardRule.HasWeekendRate3;
			checkIsOvertimeExempt.Checked=_timeCardRule.IsOvertimeExempt;
			textClockInMin.Text=_timeCardRule.MinClockInTime.ToStringHmm();
			if(!_isInsertMode) {
				label1.Text=Lan.g(this,"Employee"); //Change label to reflect new selection mode
			}
		}

		private void but5pm_Click(object sender,EventArgs e) {
			DateTime dateTime=new DateTime(2010,1,1,17,0,0);
			textAfterTimeOfDay.Text=dateTime.ToShortTimeString();
		}

		private void but6am_Click(object sender,EventArgs e) {
			DateTime dateTime=new DateTime(2010,1,1,6,0,0);
			textBeforeTimeOfDay.Text=dateTime.ToShortTimeString();
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

		private void butSave_Click(object sender,EventArgs e) {
			//Verify Data.
			if(listEmp.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select an employee.");
				return;
			}
			if(listEmp.SelectedIndices.Count>1 && listEmp.SelectedIndices.Contains(0)) {
				MsgBox.Show(this,"Cannot select both 'All Employees' and individual employees.");
				return;
			}
			TimeSpan timeSpanOverHoursPerDay=TimeSpan.Zero;
			if(textOverHoursPerDay.Text!="") {
				try {
					if(textOverHoursPerDay.Text.Contains(":")){
						timeSpanOverHoursPerDay=TimeSpan.Parse(textOverHoursPerDay.Text);
					}
					else{
						timeSpanOverHoursPerDay=TimeSpan.FromHours(PIn.Double(textOverHoursPerDay.Text));
					}
				}
				catch {
					MsgBox.Show(this,"Over hours per day invalid.");
					return;
				}
				if(timeSpanOverHoursPerDay==TimeSpan.Zero || timeSpanOverHoursPerDay.Days>0) {
					MsgBox.Show(this,"Over hours per day invalid.");
					return;
				}
			}
			TimeSpan timeSpanAfterTimeOfDay=TimeSpan.Zero;
			if(textAfterTimeOfDay.Text!="") {
				try {
					timeSpanAfterTimeOfDay=DateTime.Parse(textAfterTimeOfDay.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"After time of day invalid.");
					return;
				}
				if(timeSpanAfterTimeOfDay==TimeSpan.Zero || timeSpanAfterTimeOfDay.Days>0) {
					MsgBox.Show(this,"After time of day invalid.");
					return;
				}
			}
			TimeSpan timeSpanBeforeTimeOfDay=TimeSpan.Zero;
			if(textBeforeTimeOfDay.Text!="") {
				try {
					timeSpanBeforeTimeOfDay=DateTime.Parse(textBeforeTimeOfDay.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Before time of day invalid.");
					return;
				}
				if(timeSpanBeforeTimeOfDay==TimeSpan.Zero || timeSpanBeforeTimeOfDay.Days>0) {
					MsgBox.Show(this,"Before time of day invalid.");
					return;
				}
			}
			TimeSpan timeSpanMinClockIn=TimeSpan.Zero;
			if(textClockInMin.Text!="") {
				try {
					timeSpanMinClockIn=DateTime.Parse(textClockInMin.Text).TimeOfDay;
				}
				catch {
					MsgBox.Show(this,"Earliest Clock in Time invalid.");
					return;
				}
				if(timeSpanMinClockIn==TimeSpan.Zero || timeSpanMinClockIn.Days>0) {
					MsgBox.Show(this,"Earliest Clock in Time invalid.");
					return;
				}
			}
			if(!checkIsOvertimeExempt.Checked 
				&& !checkUseRate3.Checked
				&& timeSpanOverHoursPerDay==TimeSpan.Zero 
				&& timeSpanAfterTimeOfDay==TimeSpan.Zero 
				&& timeSpanBeforeTimeOfDay==TimeSpan.Zero 
				&& timeSpanMinClockIn==TimeSpan.Zero)
			{
				MsgBox.Show(this,"At least one Time Card Rule option must be specified.");
				return;
			}
			//save-------------------------------------------------
			//You are allowed to insert multiple TimeCardRules at one time but can only edit one existing entry at a time.
			if(_isInsertMode) {//Insert new entry for every list selection.
				List<TimeCardRule> listTimeCardRulesNew=new List<TimeCardRule>();
				for(int i=0;i<listEmp.SelectedIndices.Count;i++) {
					_timeCardRule.EmployeeNum=0;
					if(listEmp.SelectedIndices[i]!=0) {//Not 'All'
						_timeCardRule.EmployeeNum=_listEmployees[listEmp.SelectedIndices[i]-1].EmployeeNum;//-1 for All
					}
					_timeCardRule.OverHoursPerDay=timeSpanOverHoursPerDay;
					_timeCardRule.AfterTimeOfDay=timeSpanAfterTimeOfDay;
					_timeCardRule.BeforeTimeOfDay=timeSpanBeforeTimeOfDay;
					_timeCardRule.HasWeekendRate3=checkUseRate3.Checked;
					_timeCardRule.IsOvertimeExempt=checkIsOvertimeExempt.Checked;
					_timeCardRule.MinClockInTime=timeSpanMinClockIn;
					listTimeCardRulesNew.Add(_timeCardRule.Clone());
				}
				TimeCardRules.InsertMany(listTimeCardRulesNew);
			}
			else {//Update single entry.
				_timeCardRule.EmployeeNum=0;
				if(listEmp.SelectedIndex!=0) {//Not 'All'
					_timeCardRule.EmployeeNum=_listEmployees[listEmp.SelectedIndex-1].EmployeeNum;//-1 for All
				}
				_timeCardRule.OverHoursPerDay=timeSpanOverHoursPerDay;
				_timeCardRule.AfterTimeOfDay=timeSpanAfterTimeOfDay;
				_timeCardRule.BeforeTimeOfDay=timeSpanBeforeTimeOfDay;
				_timeCardRule.HasWeekendRate3=checkUseRate3.Checked;
				_timeCardRule.IsOvertimeExempt=checkIsOvertimeExempt.Checked;
				_timeCardRule.MinClockInTime=timeSpanMinClockIn;
				TimeCardRules.Update(_timeCardRule);
			}
			DataValid.SetInvalid(InvalidType.TimeCardRules);
			DialogResult=DialogResult.OK;
		}

	}
}