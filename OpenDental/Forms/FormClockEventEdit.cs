using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClockEventEdit : FormODBase {
		private ClockEvent _clockEvent;
		/// <summary>Always 1:1 with values in listStatus.Items</summary>
		private List<TimeClockStatus> _listTimeClockStatusesShown=new List<TimeClockStatus>();

		///<summary></summary>
		public FormClockEventEdit(ClockEvent clockEvent)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clockEvent=clockEvent.Copy();
		}

		private void FormClockEventEdit_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.TimecardDeleteEntry,_clockEvent.TimeEntered1,true)) {
				butDelete.Enabled=false;
				butClear.Enabled=false;
				butNow1.Enabled=false;
				butNow2.Enabled=false;
			}
			if(_clockEvent.ClockStatus==TimeClockStatus.Break){
				groupBox1.Text=Lan.g(this,"Clock Out Date and Time");
				groupBox2.Text=Lan.g(this,"Clock In Date and Time");
				groupTimeSpans.Visible=false;
				groupRate2orRate3.Visible=false;
				checkIsWorkingHome.Visible=false;
			}
			if(PrefC.HasClinicsEnabled) {
				comboClinic.ClinicNumSelected=_clockEvent.ClinicNum;
			}
      if(PrefC.IsODHQ) {
				checkIsWorkingHome.Visible=true;
      }
			//Set Text Fields----------------
			FillInitialControlsHelper();
		}

		///<summary>Fills all controls based on the values of ClockEventCur, which is a copy of the object from the DB.</summary>
		private void FillInitialControlsHelper() {
			//Clock In/Out fields---------------------------------------------------------------------
			textTimeEntered1.Text=_clockEvent.TimeEntered1.ToString();
			textTimeDisplayed1.Text=_clockEvent.TimeDisplayed1.ToString();
			if(_clockEvent.TimeEntered2.Year>1880){
				textTimeEntered2.Text=_clockEvent.TimeEntered2.ToString();
			}
			if(_clockEvent.TimeDisplayed2.Year>1880){
				textTimeDisplayed2.Text=_clockEvent.TimeDisplayed2.ToString();
			}
			//Clock status (i.e. Home, Lunch, Break)--------------------------------------------------
			listStatus.Items.Clear();
			_listTimeClockStatusesShown.Clear();
			for(int i=0;i<Enum.GetValues(typeof(TimeClockStatus)).Length;i++) {
				string statusDescript=((TimeClockStatus)i).GetDescription();
				if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {
					if((TimeClockStatus)i==TimeClockStatus.Break) {
						continue;//Skip Break option.
					}
					else if((TimeClockStatus)i==TimeClockStatus.Lunch) {
						statusDescript=TimeClockStatus.Break.GetDescription();//Change "Lunch" to "Break", still functions as Lunch.
					}
				}
				_listTimeClockStatusesShown.Add((TimeClockStatus)i);
				listStatus.Items.Add(Lan.g("enumTimeClockStatus",statusDescript));
			}
			//When ClockEventAllowBreak is disabled, ClockStatus still maps to the correct index because 'Break' is the last option. 5/24/18
			listStatus.SelectedIndex=_listTimeClockStatusesShown.IndexOf(_clockEvent.ClockStatus);//all clockevents have a status
			//Users were complaining that their employees were altering breaks / "lunch" clock events which was causing problems.
			//We will disable listStatus for any user that does not have the ability to edit all time cards (even if it is their own time card).
			//This is so that the user is forced to use the buttons within the Manage module which is more predictable.
			if(!Security.IsAuthorized(EnumPermType.TimecardsEditAll,true)) {
				listStatus.Enabled=false;
			}
			//Time Spans -----------------------------------------------------------------------------
			//Clocked time------------------------------
			TimeSpan timeSpanClocked=TimeSpan.Zero;
			if(_clockEvent.TimeDisplayed2.Year>1880) {
				timeSpanClocked=_clockEvent.TimeDisplayed2-_clockEvent.TimeDisplayed1;
				textClockedTime.Text=ClockEvents.Format(timeSpanClocked);
			}
			//Adj ------------------------------------
			textAdjustAuto.Text=ClockEvents.Format(_clockEvent.AdjustAuto);
			if(_clockEvent.AdjustIsOverridden) {
				if(_clockEvent.Adjust==TimeSpan.Zero) {
					textAdjust.Text="0";
				}
				else {
					textAdjust.Text=ClockEvents.Format(_clockEvent.Adjust);

				}
			}
			else {
				textAdjust.Text="";
			}
			//Overtime --------------------------------
			textOTimeAuto.Text=ClockEvents.Format(_clockEvent.OTimeAuto);
			if(_clockEvent.OTimeHours==TimeSpan.FromHours(-1)) {//no override
				textOTimeHours.Text="";
			}
			else if(_clockEvent.OTimeHours==TimeSpan.Zero) {
				textOTimeHours.Text="0";
			}
			else {
				textOTimeHours.Text=ClockEvents.Format(_clockEvent.OTimeHours);
			}
			//Regular Time -----------------------------
			if(timeSpanClocked>TimeSpan.Zero) {
				TimeSpan timeSpanRegular=timeSpanClocked
					+(_clockEvent.AdjustIsOverridden                ?_clockEvent.Adjust   :_clockEvent.AdjustAuto)
					-(_clockEvent.OTimeHours==TimeSpan.FromHours(-1)?_clockEvent.OTimeAuto:_clockEvent.OTimeHours);
				textRegTime.Text=ClockEvents.Format(timeSpanRegular);
			}
			//Rate 2 spans -----------------------------------------------------------------------------
			if(timeSpanClocked>TimeSpan.Zero) {
				TimeSpan timeSpanTotal=timeSpanClocked+(_clockEvent.AdjustIsOverridden?_clockEvent.Adjust:_clockEvent.AdjustAuto);//clockedTime+(Adj or AdjAuto)
				TimeSpan timeSpanRate1Hours=timeSpanTotal-(_clockEvent.Rate2Hours==TimeSpan.FromHours(-1)?_clockEvent.Rate2Auto:_clockEvent.Rate2Hours);//totalTime-(Rate2 or Rate2Auto)
				textTotalHours.Text=ClockEvents.Format(timeSpanTotal);
				textRate1Auto.Text=ClockEvents.Format(timeSpanRate1Hours);
			}
			//Rate 2 Time -----------------------------
			textRate2Auto.Text=ClockEvents.Format(_clockEvent.Rate2Auto);
			if(_clockEvent.Rate2Hours==TimeSpan.FromHours(-1)) {
				textRate2Hours.Text="";
			}
			else if(_clockEvent.Rate2Hours==TimeSpan.Zero) {
				textRate2Hours.Text="0";
			}
			else {
				textRate2Hours.Text=ClockEvents.Format(_clockEvent.Rate2Hours);
			}
			//Rate 3 Time -----------------------------
			textRate3Auto.Text=ClockEvents.Format(_clockEvent.Rate3Auto);
			if(_clockEvent.Rate3Hours==TimeSpan.FromHours(-1)) {
				textRate3Hours.Text="";
			}
			else if(_clockEvent.Rate3Hours==TimeSpan.Zero) {
				textRate3Hours.Text="0";
			}
			else {
				textRate3Hours.Text=ClockEvents.Format(_clockEvent.Rate3Hours);
			}
			//notes ------------------------------------------------------------------------------------
			textNote.Text=_clockEvent.Note;
			//checkIsWorkingHome-----------------------------------------------------------------------
			checkIsWorkingHome.Checked=_clockEvent.IsWorkingHome;
		}

		///<summary>Fills all controls based on the values of ClockEventCur, which is a copy of the object from the DB.</summary>
		private void FillAutoControlsHelper() {
			//Clock In/Out fields---------------------------------------------------------------------
			textTimeEntered1.Text=_clockEvent.TimeEntered1.ToString();
			if(_clockEvent.TimeEntered2.Year>1880) {
				textTimeEntered2.Text=_clockEvent.TimeEntered2.ToString();
			}
			//Clocked time------------------------------
			TimeSpan timeSpanClocked=TimeSpan.Zero;
			if(_clockEvent.TimeDisplayed2.Year>1880) {
				timeSpanClocked=_clockEvent.TimeDisplayed2-_clockEvent.TimeDisplayed1;
				textClockedTime.Text=ClockEvents.Format(timeSpanClocked);
			}
			//Adj ------------------------------------
			textAdjustAuto.Text=ClockEvents.Format(_clockEvent.AdjustAuto);
			//Overtime --------------------------------
			textOTimeAuto.Text=ClockEvents.Format(_clockEvent.OTimeAuto);
			//Regular Time -----------------------------
			if(timeSpanClocked>TimeSpan.Zero) {
				TimeSpan timeSpanRegular=timeSpanClocked
					+(_clockEvent.AdjustIsOverridden                ?_clockEvent.Adjust   :_clockEvent.AdjustAuto)
					-(_clockEvent.OTimeHours==TimeSpan.FromHours(-1)?_clockEvent.OTimeAuto:_clockEvent.OTimeHours);
				textRegTime.Text=ClockEvents.Format(timeSpanRegular);
			}
			//Rate 2 and Rate 3 spans -----------------------------------------------------------------------------
			if(timeSpanClocked>TimeSpan.Zero) {
				TimeSpan timeSpanTotal=timeSpanClocked+(_clockEvent.AdjustIsOverridden?_clockEvent.Adjust:_clockEvent.AdjustAuto);//clockedTime+(Adj or AdjAuto)
				TimeSpan timeSpanRate2=_clockEvent.Rate2Hours==TimeSpan.FromHours(-1)?_clockEvent.Rate2Auto:_clockEvent.Rate2Hours;//Rate2 or Rate2Auto
				TimeSpan timeSpanRate3=_clockEvent.Rate3Hours==TimeSpan.FromHours(-1)?_clockEvent.Rate3Auto:_clockEvent.Rate3Hours;//Rate3 or Rate3Auto
				TimeSpan timeSpanRate1Hours=timeSpanTotal-timeSpanRate2-timeSpanRate3;//totalTime-(Rate2 or Rate2Auto)-(Rate3 or Rate3Auto)
				textTotalHours.Text=ClockEvents.Format(timeSpanTotal);
				textRate1Auto.Text=ClockEvents.Format(timeSpanRate1Hours);
			}
			//Rate 2 Time -----------------------------
			textRate2Auto.Text=ClockEvents.Format(_clockEvent.Rate2Auto);
			//Rate 3 Time -----------------------------
			textRate3Auto.Text=ClockEvents.Format(_clockEvent.Rate3Auto);
		}

		private void textTimeDisplayed2_TextChanged(object sender,EventArgs e) {
			try {
				_clockEvent.TimeDisplayed2=DateTime.Parse(textTimeDisplayed2.Text);
			}
			catch{
				clearAutoFieldsHelper();
				return;
			}
			FillAutoControlsHelper();
		}

		private void textTimeDisplayed1_TextChanged(object sender,EventArgs e) {
			try {
				_clockEvent.TimeDisplayed1=DateTime.Parse(textTimeDisplayed1.Text);
			}
			catch {
				clearAutoFieldsHelper();
				return;
			}
			FillAutoControlsHelper();
		}

		private void textTimeEntered2_TextChanged(object sender,EventArgs e) {
			try {
				if(textTimeEntered2.Text=="") {
					_clockEvent.TimeEntered2=DateTime.MinValue;
				}
				else {
					_clockEvent.TimeEntered2=PIn.Date(textTimeEntered2.Text);
				}
			}
			catch {
				return;
			}
			FillAutoControlsHelper();
		}

		private void textAdjust_TextChanged(object sender,EventArgs e) {
			if(textAdjust.Text=="") {
				_clockEvent.AdjustIsOverridden=false;
				_clockEvent.Adjust=TimeSpan.Zero;
			}
			else {
				_clockEvent.AdjustIsOverridden=true;
				try {
					_clockEvent.Adjust=TimeSpan.FromHours(Double.Parse(textAdjust.Text));
				}
				catch {
					return;
				}
			}
			FillAutoControlsHelper();
		}

		private void textOvertime_TextChanged(object sender,EventArgs e) {
			if(textOTimeHours.Text=="") {
				_clockEvent.OTimeHours=TimeSpan.FromHours(-1);
			}
			else {
				//ClockEventCur.OTimeHours=PIn.Time(textOTimeHours.Text);
				try {
					_clockEvent.OTimeHours=TimeSpan.FromHours(Double.Parse(textOTimeHours.Text));
				}
				catch {
					return;
				}
			}
			FillAutoControlsHelper();
		}

		private void textRate2Hours_TextChanged(object sender,EventArgs e) {
			if(textRate2Hours.Text=="") {
				_clockEvent.Rate2Hours=TimeSpan.FromHours(-1);
			}
			else {
				try {
					if(textRate2Hours.Text.Contains(":")) {
						_clockEvent.Rate2Hours=ClockEvents.ParseHours(textRate2Hours.Text);
					}
					else {
						_clockEvent.Rate2Hours=TimeSpan.FromHours(Double.Parse(textRate2Hours.Text));
					}
				}
				catch {
					return;
				}
				//No ClockEvent can have both Rate2 and Rate3 hours
				if(_clockEvent.Rate2Hours!=TimeSpan.Zero) {
					_clockEvent.Rate3Hours=TimeSpan.Zero;
					textRate3Hours.Text="0";
				}
			}
			FillAutoControlsHelper();
		}

		private void textRate3Hours_TextChanged(object sender,EventArgs e) {
			if(textRate3Hours.Text=="") {
				_clockEvent.Rate3Hours=TimeSpan.FromHours(-1);
			}
			else {
				try {
					if(textRate3Hours.Text.Contains(":")) {
						_clockEvent.Rate3Hours=ClockEvents.ParseHours(textRate3Hours.Text);
					}
					else {
						_clockEvent.Rate3Hours=TimeSpan.FromHours(Double.Parse(textRate3Hours.Text));
					}
				}
				catch {
					return;
				}
				//No ClockEvent can have both Rate2 and Rate3 hours
				if(_clockEvent.Rate3Hours!=TimeSpan.Zero) {
					_clockEvent.Rate2Hours=TimeSpan.Zero;
					textRate2Hours.Text="0";
				}
			}
			FillAutoControlsHelper();
		}

		private void butNow1_Click(object sender,EventArgs e) {
			textTimeDisplayed1.Text=DateTime.Now.ToString();
		}

		private void butNow2_Click(object sender,EventArgs e) {
			textTimeDisplayed2.Text=DateTime.Now.ToString();
			if(textTimeEntered2.Text=="") {//only set the time entered if it's blank
				textTimeEntered2.Text=MiscData.GetNowDateTime().ToString();
				_clockEvent.TimeEntered2=MiscData.GetNowDateTime();
			}
			//FillTimeSpans();//not really needed because of the TextChanged event, but might prevent a bug.
			FillAutoControlsHelper();
		}

		private void butClear_Click(object sender,EventArgs e) {
			textTimeDisplayed2.Text="";
			textTimeEntered2.Text="";
			_clockEvent.TimeEntered2=DateTime.MinValue;
			clearAutoFieldsHelper();
			//FillTimeSpans();//not really needed because of the TextChanged event, but might prevent a bug.
			FillAutoControlsHelper();
		}

		private void clearAutoFieldsHelper() {
			textClockedTime.Text="";
			textTotalHours.Text="";
			textRegTime.Text="";
			textRate1Auto.Text="";
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this clock event?")){
				return;
			}
			ClockEvents.Delete(_clockEvent.ClockEventNum);
			Employees.UpdateClockStatus(_clockEvent.EmployeeNum);
			SecurityLogs.MakeLogEntry(EnumPermType.TimecardDeleteEntry,0,
				"Original entry: "+_clockEvent.TimeEntered1.ToString());
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			//if(textAmountBonus.errorProvider1.GetError(textAmountBonus)!="") {
			//  MsgBox.Show(this,"Please enter in a valid dollar amount for Bonus.");
			//  return;
			//}
			DateTime timeDisplayed1=DateTime.MinValue;
			try{
				timeDisplayed1=DateTime.Parse(textTimeDisplayed1.Text);//because this must always be valid
			}
			catch{
				if(_clockEvent.ClockStatus==TimeClockStatus.Break){
					MsgBox.Show(this,"Please enter a valid clock-out date and time.");
				}
				else{
					MsgBox.Show(this,"Please enter a valid clock-in date and time.");
				}
				return;
			}
			if(timeDisplayed1.Date > DateTime.Today) {
				if(_clockEvent.ClockStatus==TimeClockStatus.Break){
					MsgBox.Show(this,"Clock-out date cannot be a future date.");
				}
				else{
					MsgBox.Show(this,"Clock-in date cannot be a future date.");
				}
				return;
			}
			DateTime timeDisplayed2=DateTime.MinValue;
			if(textTimeDisplayed2.Text!=""){//it can be empty
				try{
					timeDisplayed2=DateTime.Parse(textTimeDisplayed2.Text);
				}
				catch{
					if(_clockEvent.ClockStatus==TimeClockStatus.Break){
						MsgBox.Show(this,"Please enter a valid clock-in date and time.");
					}
					else{
						MsgBox.Show(this,"Please enter a valid clock-out date and time.");
					}
					return;
				}
			}
			if(timeDisplayed2.Date > DateTime.Today) {
				if(_clockEvent.ClockStatus==TimeClockStatus.Break){
					MsgBox.Show(this,"Clock-in date cannot be a future date.");
				}
				else{
					MsgBox.Show(this,"Clock-out date cannot be a future date.");
				}
				return;
			}
			if(textTimeDisplayed2.Text!="" && timeDisplayed1 > timeDisplayed2){
				if(_clockEvent.ClockStatus==TimeClockStatus.Break) {
					MsgBox.Show(this,"Break end time cannot be earlier than break start time.");
					return;
				}
				else {
					MsgBox.Show(this,"Clock out time cannot be earlier than clock in time.");
					return;
				}
			}
			if(textTimeDisplayed2.Text=="" && textTimeEntered2.Text!="") {//user is trying to clear the time manually
				MsgBox.Show(this,"A date and time must be entered in the second box, or use the Clear button.");
				return;
			}
			if(PayPeriods.CannotEditPayPeriodOfDate(timeDisplayed1,_clockEvent.EmployeeNum)) {
				string message=Lan.g(this,"You only have permission to edit your time card for the current pay period. The ");
				string messageTwo=Lan.g(this," you have entered does not fall within the current pay period.");
				MessageBox.Show(message+groupBox1.Text+messageTwo);
				return;
			}
			TimeSpan timeSpanOvertime=TimeSpan.Zero;
			TimeSpan timeSpanAdjust=TimeSpan.Zero;
			if(_clockEvent.ClockStatus!=TimeClockStatus.Break) {
				if(textOTimeHours.Text!="") {
					try {
						if(textOTimeHours.Text.Contains(":")) {
							timeSpanOvertime=TimeSpan.Parse(textOTimeHours.Text);
						}
						else {
							timeSpanOvertime=TimeSpan.FromHours(Double.Parse(textOTimeHours.Text));
						}
					}
					catch {
						MsgBox.Show(this,"Please enter a valid overtime amount.");
						return;
					}
					if(timeSpanOvertime < TimeSpan.Zero) {
						MsgBox.Show(this,"Overtime must be positive.");
						return;
					}
				}
				if(textAdjust.Text!="") {
					try {
						if(textAdjust.Text.Contains(":")) {
							timeSpanAdjust=TimeSpan.Parse(textAdjust.Text);
						}
						else {
							timeSpanAdjust=TimeSpan.FromHours(Double.Parse(textAdjust.Text));
						}
					}
					catch {
						MsgBox.Show(this,"Please enter a valid adjustment amount.");
						return;
					}
				}
				if(textRegTime.Text=="") {//Must be invalid calc.
					if(textTimeEntered2.Text=="") {//They haven't clocked out yet.	Invalid calc is expected.
						if(textAdjust.Text.Trim()!=""||textOTimeHours.Text.Trim()!="") {//They're entering in overtime or adjustments.
							MsgBox.Show(this,"Cannot enter overtime or adjustments while clocked in.");//To this timespan is implied.
							return;
						}
					}
					else {//They have clocked out.
						MsgBox.Show(this,"Overtime and adjustments cannot exceed the total time.");
						return;
					}
				}
			}
			//timeEntered2 is largely taken care of, except for this one situation
			if(textTimeDisplayed2.Text!="" && textTimeEntered2.Text=="") {
				_clockEvent.TimeEntered2=MiscData.GetNowDateTime();
			}
			_clockEvent.TimeDisplayed1=timeDisplayed1;
			_clockEvent.TimeDisplayed2=timeDisplayed2;
			_clockEvent.ClockStatus=_listTimeClockStatusesShown[listStatus.SelectedIndex];
			if(textAdjust.Text=="") {//no override
				_clockEvent.AdjustIsOverridden=false;
				_clockEvent.Adjust=TimeSpan.Zero;
			}
			else {
				_clockEvent.AdjustIsOverridden=true;
				_clockEvent.Adjust=timeSpanAdjust;
			}
			if(textOTimeHours.Text=="") {//no override
				_clockEvent.OTimeHours=TimeSpan.FromHours(-1d);
			}
			else {
				_clockEvent.OTimeHours=timeSpanOvertime;
			}
			if(textRate2Hours.Text=="") {
				_clockEvent.Rate2Hours=TimeSpan.FromHours(-1);
			}
			if(textRate3Hours.Text=="") {
				_clockEvent.Rate3Hours=TimeSpan.FromHours(-1);
			}
			//The two auto fields are only set externally.
			_clockEvent.Note=textNote.Text;
			_clockEvent.IsWorkingHome=checkIsWorkingHome.Checked;
			if(PrefC.HasClinicsEnabled) {
				_clockEvent.ClinicNum=comboClinic.ClinicNumSelected;
			}
			ClockEvents.Update(_clockEvent);
			Employees.UpdateClockStatus(_clockEvent.EmployeeNum);
			DialogResult=DialogResult.OK;
		}

	}
}