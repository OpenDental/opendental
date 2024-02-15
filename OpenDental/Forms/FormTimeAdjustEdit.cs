using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormTimeAdjustEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private TimeAdjust _timeAdjust;
		private TimeAdjust _timeAdjustOld;

		///<summary>Used to determine what type of security log text to generate.</summary>
		private enum UnpaidProtectedLeaveLogType {
			Created,
			Deleted,
			Edited,
		}

		///<summary></summary>
		public FormTimeAdjustEdit(TimeAdjust timeAdjust){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_timeAdjust=timeAdjust.Copy();
			_timeAdjustOld=timeAdjust.Copy();
		}

		private void FormTimeAdjustEdit_Load(object sender, System.EventArgs e) {
			if(_timeAdjust.IsAuto) {
				radioAuto.Checked=true;
			}
			else {
				radioManual.Checked=true;
			}
			textTimeEntry.Text=_timeAdjust.TimeEntry.ToString();
			if(_timeAdjust.OTimeHours.TotalHours==0){
				if(_timeAdjust.PtoDefNum==0) {
					if(_timeAdjust.TimeAdjustNum!=0 && _timeAdjust.RegHours==TimeSpan.Zero) {
						//Existing adjustments of 0 hours should display as 0.
						textHours.Text=_timeAdjust.RegHours.TotalHours.ToString("n0");
					}
					else {
						textHours.Text=ClockEvents.Format(_timeAdjust.RegHours);
					}
				}
				else {//Is PTO
					textHours.Text=ClockEvents.Format(_timeAdjust.PtoHours);
				}
			}
			else{
				checkOvertime.Checked=true;
				textHours.Text=ClockEvents.Format(_timeAdjust.OTimeHours);
			}
			checkUnpaidProtectedLeave.Checked=_timeAdjust.IsUnpaidProtectedLeave;
			if(IsNew) {
				textUser.Text=Security.CurUser.UserName;
			}
			else if(_timeAdjust.SecuUserNumEntry==0) {
				textUser.Text="";
			}
			else {
				textUser.Text=Userods.GetName(_timeAdjust.SecuUserNumEntry);
			}
			textNote.Text=_timeAdjust.Note;
			comboPTO.Items.Clear();
			comboPTO.Items.Add(Lan.g(this,"None"));
			comboPTO.SelectedIndex=0;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TimeCardAdjTypes).OrderBy(x => x.ItemName).ToList();
			for(int i=0;i<listDefs.Count;i++) {
				if(listDefs[i].IsHidden && listDefs[i].DefNum==_timeAdjust.PtoDefNum) {
					comboPTO.Items.Add(listDefs[i].ItemName+" "+Lan.g(this,"(hidden)"),listDefs[i]);
				}
				else if(!listDefs[i].IsHidden) {
					comboPTO.Items.Add(listDefs[i].ItemName,listDefs[i]);
				}
				if(listDefs[i].DefNum==_timeAdjust.PtoDefNum) {
					comboPTO.SelectedIndex=comboPTO.Items.Count-1;
				}
			}
		}

		private void checkUnpaidProtectedLeave_Click(object sender,EventArgs e) {
			//User that is not the employee on the adjustment must have the  ProtectedLeaveAdjustmentEdit permission.
			if(Security.CurUser.EmployeeNum!=_timeAdjust.EmployeeNum && !Security.IsAuthorized(EnumPermType.ProtectedLeaveAdjustmentEdit)) {
				checkUnpaidProtectedLeave.Checked=!checkUnpaidProtectedLeave.Checked;
			}
		}

		private string CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType unpaidProtectedLeaveLogType,TimeAdjust timeAdjust,TimeAdjust timeAdjustEdited=null) {
			string stringTimeAdjustNum=$"TimeAdjustNum: {timeAdjust.TimeAdjustNum}";
			string stringEmployeeName=$"{Lan.g(this,"Employee")}: {Employees.GetNameFL(timeAdjust.EmployeeNum)}";
			string stringTimeEntry=$"{Lan.g(this,"Date/Time Entry")}: {timeAdjust.TimeEntry}";
			string stringHours=$"{Lan.g(this,"Hours")}: {HoursStringHelper(timeAdjust)}";
			string stringNote=$"{Lan.g(this,"Note")}: \"{timeAdjust.Note}\"";
			string stringChangedTo=Lan.g(this,"changed to");
			string logText;
			if(unpaidProtectedLeaveLogType.In(UnpaidProtectedLeaveLogType.Created,UnpaidProtectedLeaveLogType.Deleted)) {
				if(unpaidProtectedLeaveLogType==UnpaidProtectedLeaveLogType.Created) {
					logText=Lan.g(this,"Protected Leave Time Card Adjustment Created. ");
				}
				else {//logType==UnpaidProtectedLeaveLogType.Deleted
					logText=Lan.g(this,"Protected Leave Time Card Adjustment Deleted. ");
				}
				return logText+=$"{stringTimeAdjustNum}, {stringEmployeeName}, {stringTimeEntry}, {stringHours}, {stringNote}";
			}
			//logType==UnpaidProtectecLeaveLogType.Edited
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

		private string HoursStringHelper(TimeAdjust timeAdjust) {
			if(timeAdjust.OTimeHours.TotalHours==0) {
				if(timeAdjust.PtoDefNum==0) {
					return ClockEvents.Format(timeAdjust.RegHours);
				}
				//Is PTO
				return ClockEvents.Format(timeAdjust.PtoHours);
			}
			//Is OT
			return ClockEvents.Format(timeAdjust.OTimeHours);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			TimeAdjusts.Delete(_timeAdjust);
			if(_timeAdjust.IsUnpaidProtectedLeave) {
				string logText=CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType.Deleted,_timeAdjustOld);
				SecurityLogs.MakeLogEntry(EnumPermType.ProtectedLeaveAdjustmentEdit,0,logText);
			}
			else if(_timeAdjust.TimeAdjustNum!=0){//If we are deleting a time card adjustment that is in the DB, log this action.
				SecurityLogs.MakeLogEntry(EnumPermType.TimeAdjustEdit,0,
					$"Time Card Adjustment deleted for Employee: {Employees.GetNameFL(_timeAdjust.EmployeeNum)}.");
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,System.EventArgs e) {
			DateTime dateTimeEntry;
			try {
				dateTimeEntry=DateTime.Parse(textTimeEntry.Text);
			}
			catch {
				MsgBox.Show(this,"Please enter a valid Date/Time.");
				return;
			}
			TimeSpan timeSpanHoursEntered;
			try {
				if(textHours.Text.Contains(":")) {
					timeSpanHoursEntered=ClockEvents.ParseHours(textHours.Text);
				}
				else {
					timeSpanHoursEntered=TimeSpan.FromHours(Double.Parse(textHours.Text));
				}
			}
			catch {
				MsgBox.Show(this,"Please enter valid Hours and Minutes.");
				return;
			}
			if(PayPeriods.CannotEditPayPeriodOfDate(dateTimeEntry,_timeAdjust.EmployeeNum)) {
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
				&& !Security.IsAuthorized(EnumPermType.TimecardsEditAll,suppressMessage:true)) 
			{
				MsgBox.Show(this,"HQ users without the Edit All Time Cards permission must select a PTO Type or check the Protected Leave box.");
				return;
			}
			//end of validation
			_timeAdjust.IsAuto=radioAuto.Checked;
			_timeAdjust.TimeEntry=DateTime.Parse(textTimeEntry.Text);
			if(_timeAdjust.TimeEntry.TimeOfDay.Ticks==0) {
				_timeAdjust.TimeEntry=_timeAdjust.TimeEntry.AddMinutes(1);
			}
			if(checkOvertime.Checked){
				_timeAdjust.RegHours=-timeSpanHoursEntered;
				_timeAdjust.OTimeHours=timeSpanHoursEntered;
				_timeAdjust.PtoHours=TimeSpan.FromHours(0);
				_timeAdjust.PtoDefNum=0;
			}
			else if(comboPTO.SelectedIndex==0) {
				_timeAdjust.RegHours=timeSpanHoursEntered;
				_timeAdjust.OTimeHours=TimeSpan.FromHours(0);
				_timeAdjust.PtoHours=TimeSpan.FromHours(0);
				_timeAdjust.PtoDefNum=0;
			}
			else {//Is PTO
				Def def=comboPTO.GetSelected<Def>();
				_timeAdjust.RegHours=TimeSpan.FromHours(0);
				_timeAdjust.OTimeHours=TimeSpan.FromHours(0);
				_timeAdjust.PtoHours=timeSpanHoursEntered;
				_timeAdjust.PtoDefNum=def.DefNum;
			}
			_timeAdjust.Note=textNote.Text;
			_timeAdjust.IsUnpaidProtectedLeave=checkUnpaidProtectedLeave.Checked;
			if(IsNew){
				_timeAdjust.SecuUserNumEntry=Security.CurUser.UserNum;
				TimeAdjusts.Insert(_timeAdjust);
				if(_timeAdjust.IsUnpaidProtectedLeave) {
					string logText=CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType.Created,_timeAdjust);
					SecurityLogs.MakeLogEntry(EnumPermType.ProtectedLeaveAdjustmentEdit,0,logText);
				}
				else {
					SecurityLogs.MakeLogEntry(EnumPermType.TimeAdjustEdit,0,
					$"Time Card Adjustment created for Employee: {Employees.GetNameFL(_timeAdjust.EmployeeNum)}.");
				}
			}
			else{
				TimeAdjusts.Update(_timeAdjust,_timeAdjustOld);
				if(_timeAdjust.IsUnpaidProtectedLeave || _timeAdjustOld.IsUnpaidProtectedLeave) {
					string logText=CreateUnpaidProtectedLeaveSecurityLogText(UnpaidProtectedLeaveLogType.Edited,_timeAdjustOld,_timeAdjust);
					SecurityLogs.MakeLogEntry(EnumPermType.ProtectedLeaveAdjustmentEdit,0,logText);
				}
				else {
					SecurityLogs.MakeLogEntry(EnumPermType.TimeAdjustEdit,0,
					$"Time Card Adjustment edited for Employee: {Employees.GetNameFL(_timeAdjust.EmployeeNum)}.");
				}
			}
			DialogResult=DialogResult.OK;
		}

	}
}





















