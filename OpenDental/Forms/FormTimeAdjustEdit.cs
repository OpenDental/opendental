using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTimeAdjustEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private TimeAdjust _timeAdjustCur;
		private TimeAdjust _timeAdjustOld;

		///<summary>Used to determine what type of security log text to generate.</summary>
		private enum UnpaidProtectedLeaveLogType {
			Created,
			Deleted,
			Edited,
		}

		///<summary></summary>
		public FormTimeAdjustEdit(TimeAdjust timeAdjustCur){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_timeAdjustCur=timeAdjustCur.Copy();
			_timeAdjustOld=timeAdjustCur.Copy();
		}

		private void FormTimeAdjustEdit_Load(object sender, System.EventArgs e) {
			if(_timeAdjustCur.IsAuto) {
				radioAuto.Checked=true;
			}
			else {
				radioManual.Checked=true;
			}
			textTimeEntry.Text=_timeAdjustCur.TimeEntry.ToString();
			if(_timeAdjustCur.OTimeHours.TotalHours==0){
				if(_timeAdjustCur.PtoDefNum==0) {
					if(_timeAdjustCur.TimeAdjustNum!=0 && _timeAdjustCur.RegHours==TimeSpan.Zero) {
						//Existing adjustments of 0 hours should display as 0.
						textHours.Text=_timeAdjustCur.RegHours.TotalHours.ToString("n0");
					}
					else {
						textHours.Text=ClockEvents.Format(_timeAdjustCur.RegHours);
					}
				}
				else {//Is PTO
					textHours.Text=ClockEvents.Format(_timeAdjustCur.PtoHours);
				}
			}
			else{
				checkOvertime.Checked=true;
				textHours.Text=ClockEvents.Format(_timeAdjustCur.OTimeHours);
			}
			checkUnpaidProtectedLeave.Checked=_timeAdjustCur.IsUnpaidProtectedLeave;
			if(IsNew) {
				textUser.Text=Security.CurUser.UserName;
			}
			else if(_timeAdjustCur.SecuUserNumEntry==0) {
				textUser.Text="";
			}
			else {
				textUser.Text=Userods.GetName(_timeAdjustCur.SecuUserNumEntry);
			}
			textNote.Text=_timeAdjustCur.Note;
			comboPTO.Items.Clear();
			comboPTO.Items.Add(Lan.g(this,"None"));
			comboPTO.SelectedIndex=0;
			List<Def> listPtoTypes=Defs.GetDefsForCategory(DefCat.TimeCardAdjTypes).OrderBy(x => x.ItemName).ToList();
			foreach(Def def in listPtoTypes) {
				if(def.IsHidden && def.DefNum==_timeAdjustCur.PtoDefNum) {
					comboPTO.Items.Add(def.ItemName+" "+Lan.g(this,"(hidden)"),def);
				}
				else if(!def.IsHidden) {
					comboPTO.Items.Add(def.ItemName,def);
				}
				if(def.DefNum==_timeAdjustCur.PtoDefNum) {
					comboPTO.SelectedIndex=comboPTO.Items.Count-1;
				}
			}
		}

		private void checkUnpaidProtectedLeave_Click(object sender,EventArgs e) {
			//User that is not the employee on the adjustment must have the  ProtectedLeaveAdjustmentEdit permission.
			if(Security.CurUser.EmployeeNum!=_timeAdjustCur.EmployeeNum && !Security.IsAuthorized(Permissions.ProtectedLeaveAdjustmentEdit)) {
				checkUnpaidProtectedLeave.Checked=!checkUnpaidProtectedLeave.Checked;
			}
		}

		private string CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType logType,TimeAdjust timeAdjust,TimeAdjust timeAdjustEdited=null) {
			string stringTimeAdjustNum=$"TimeAdjustNum: {timeAdjust.TimeAdjustNum}";
			string stringEmployeeName=$"{Lan.g(this,"Employee")}: {Employees.GetNameFL(timeAdjust.EmployeeNum)}";
			string stringTimeEntry=$"{Lan.g(this,"Date/Time Entry")}: {timeAdjust.TimeEntry}";
			string stringHours=$"{Lan.g(this,"Hours")}: {HoursStringHelper(timeAdjust)}";
			string stringNote=$"{Lan.g(this,"Note")}: \"{timeAdjust.Note}\"";
			string stringChangedTo=Lan.g(this,"changed to");
			string logText;
			if(ListTools.In(logType,UnpaidProtectedLeaveLogType.Created,UnpaidProtectedLeaveLogType.Deleted)) {
				if(logType==UnpaidProtectedLeaveLogType.Created) {
					logText=Lan.g(this,"Protected Leave Time Card Adjustment Created. ");
				}
				else {//logType==UnpaidProtectedLeaveLogType.Deleted
					logText=Lan.g(this,"Protected Leave Time Card Adjustment Deleted. ");
				}
				return logText+=$"{stringTimeAdjustNum}, {stringEmployeeName}, {stringTimeEntry}, {stringHours}, {stringNote}";
			}
			else {//logType==UnpaidProtectecLeaveLogType.Edited
				if(timeAdjust.IsUnpaidProtectedLeave && timeAdjustEdited.IsUnpaidProtectedLeave) {
					logText=Lan.g(this,"Protected Leave Time Card Adjustment Edited. ");
				}
				else if(timeAdjust.IsUnpaidProtectedLeave && !timeAdjustEdited.IsUnpaidProtectedLeave) {
					logText=Lan.g(this,"Protected Leave status removed from Time Card Adjustment. ");
				}
				else {//!timeAdjust.IsUnpaidProtectedLeave && timeAdjustNew.IsUnpaidProtectedLeave
					logText=Lan.g(this,"Protected Leave status added to Time Card Adjustment. ");
				}
				logText+=$"{stringTimeAdjustNum}, {stringEmployeeName}, {stringTimeEntry}";
				if(timeAdjust.TimeEntry!=timeAdjustEdited.TimeEntry) {
					logText+=$" {stringChangedTo} {timeAdjustEdited.TimeEntry}";
				}
				logText+=$", {stringHours}";
				if(HoursStringHelper(timeAdjust)!=HoursStringHelper(timeAdjustEdited)) {
					logText+=$" {stringChangedTo} {HoursStringHelper(timeAdjustEdited)}";
				}
				logText+=$", {stringNote}";
				if(timeAdjust.Note!=timeAdjustEdited.Note) {
					logText+=$" {stringChangedTo} \"{timeAdjustEdited.Note}\"";
				}
				return logText;
			}
		}

		private string HoursStringHelper(TimeAdjust timeAdjust) {
			if(timeAdjust.OTimeHours.TotalHours==0) {
				if(timeAdjust.PtoDefNum==0) {
					return ClockEvents.Format(timeAdjust.RegHours);
				}
				else {//Is PTO
					return ClockEvents.Format(timeAdjust.PtoHours);
				}
			}
			else {//Is OT
				return ClockEvents.Format(timeAdjust.OTimeHours);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			TimeAdjusts.Delete(_timeAdjustCur);
			if(_timeAdjustCur.IsUnpaidProtectedLeave) {
				string logText=CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType.Deleted,_timeAdjustOld);
				SecurityLogs.MakeLogEntry(Permissions.ProtectedLeaveAdjustmentEdit,0,logText);
			}
			else if(_timeAdjustCur.TimeAdjustNum!=0){//If we are deleting a time card adjustment that is in the DB, log this action.
				SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
					$"Time Card Adjustment deleted for Employee: {Employees.GetNameFL(_timeAdjustCur.EmployeeNum)}.");
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			DateTime dateTimeEntry;
			try {
				dateTimeEntry=DateTime.Parse(textTimeEntry.Text);
			}
			catch {
				MsgBox.Show(this,"Please enter a valid Date/Time.");
				return;
			}
			TimeSpan hoursEntered;
			try {
				if(textHours.Text.Contains(":")) {
					hoursEntered=ClockEvents.ParseHours(textHours.Text);
				}
				else {
					hoursEntered=TimeSpan.FromHours(Double.Parse(textHours.Text));
				}
			}
			catch {
				MsgBox.Show(this,"Please enter valid Hours and Minutes.");
				return;
			}
			if(PayPeriods.CannotEditPayPeriodOfDate(dateTimeEntry,_timeAdjustCur.EmployeeNum)) {
				MsgBox.Show(this,"You only have permission to edit your time card for the current pay period. "
					+"The date you have entered does not fall within the current pay period.");
				return;
			}
			if((checkOvertime.Checked || checkUnpaidProtectedLeave.Checked) && comboPTO.SelectedIndex!=0) {
				MsgBox.Show(this,"Overtime and Protected Leave Adjustments must have PTO Type set to 'None'.\r\n"
					+"Please select 'None' for PTO Type or uncheck Overtime Adjustment or Protected Leave.");
				return;
			}
			if(checkUnpaidProtectedLeave.Checked && radioAuto.Checked) {
				MsgBox.Show(this,"Protected Leave Adjustments cannot be marked as automatically entered.");
				return;
			}
			if(checkUnpaidProtectedLeave.Checked && checkOvertime.Checked) {
				MsgBox.Show(this,"Protected Leave Adjustments cannot be marked as overtime.");
				return;
			}
			//HQ users without the TimecardEditAll permission can only make Time Card Adjustments for PTO and UPL.
			if(PrefC.IsODHQ
				&& !checkUnpaidProtectedLeave.Checked
				&& comboPTO.SelectedIndex==0
				&& !Security.IsAuthorized(Permissions.TimecardsEditAll,suppressMessage:true)) 
			{
				MsgBox.Show(this,"HQ users without the Edit All Time Cards permission must select a PTO Type or check the Protected Leave box.");
				return;
			}
			//end of validation
			_timeAdjustCur.IsAuto=radioAuto.Checked;
			_timeAdjustCur.TimeEntry=DateTime.Parse(textTimeEntry.Text);
			if(_timeAdjustCur.TimeEntry.TimeOfDay.Ticks==0) {
				_timeAdjustCur.TimeEntry=_timeAdjustCur.TimeEntry.AddMinutes(1);
			}
			if(checkOvertime.Checked){
				_timeAdjustCur.RegHours=-hoursEntered;
				_timeAdjustCur.OTimeHours=hoursEntered;
				_timeAdjustCur.PtoHours=TimeSpan.FromHours(0);
				_timeAdjustCur.PtoDefNum=0;
			}
			else if(comboPTO.SelectedIndex==0) {
				_timeAdjustCur.RegHours=hoursEntered;
				_timeAdjustCur.OTimeHours=TimeSpan.FromHours(0);
				_timeAdjustCur.PtoHours=TimeSpan.FromHours(0);
				_timeAdjustCur.PtoDefNum=0;
			}
			else {//Is PTO
				Def def=comboPTO.GetSelected<Def>();
				_timeAdjustCur.RegHours=TimeSpan.FromHours(0);
				_timeAdjustCur.OTimeHours=TimeSpan.FromHours(0);
				_timeAdjustCur.PtoHours=hoursEntered;
				_timeAdjustCur.PtoDefNum=def.DefNum;
			}
			_timeAdjustCur.Note=textNote.Text;
			_timeAdjustCur.IsUnpaidProtectedLeave=checkUnpaidProtectedLeave.Checked;
			if(IsNew){
				_timeAdjustCur.SecuUserNumEntry=Security.CurUser.UserNum;
				TimeAdjusts.Insert(_timeAdjustCur);
				if(_timeAdjustCur.IsUnpaidProtectedLeave) {
					string logText=CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType.Created,_timeAdjustCur);
					SecurityLogs.MakeLogEntry(Permissions.ProtectedLeaveAdjustmentEdit,0,logText);
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
					$"Time Card Adjustment created for Employee: {Employees.GetNameFL(_timeAdjustCur.EmployeeNum)}.");
				}
			}
			else{
				TimeAdjusts.Update(_timeAdjustCur,_timeAdjustOld);
				if(_timeAdjustCur.IsUnpaidProtectedLeave || _timeAdjustOld.IsUnpaidProtectedLeave) {
					string logText=CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType.Edited,_timeAdjustOld,_timeAdjustCur);
					SecurityLogs.MakeLogEntry(Permissions.ProtectedLeaveAdjustmentEdit,0,logText);
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.TimeAdjustEdit,0,
					$"Time Card Adjustment edited for Employee: {Employees.GetNameFL(_timeAdjustCur.EmployeeNum)}.");
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

	

		

		

		


	}
}





















