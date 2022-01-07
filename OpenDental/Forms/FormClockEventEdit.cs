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
		private ClockEvent ClockEventCur;
		/// <summary>Always 1:1 with values in listStatus.Items</summary>
		private List<TimeClockStatus> _listShownTimeClockStatuses=new List<TimeClockStatus>();

		///<summary></summary>
		public FormClockEventEdit(ClockEvent clockEventCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ClockEventCur=clockEventCur.Copy();
		}

		private void FormClockEventEdit_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.TimecardDeleteEntry,ClockEventCur.TimeEntered1,true)) {
				butDelete.Enabled=false;
				butClear.Enabled=false;
				butNow1.Enabled=false;
				butNow2.Enabled=false;
			}
			if(ClockEventCur.ClockStatus==TimeClockStatus.Break){
				groupBox1.Text=Lan.g(this,"Clock Out Date and Time");
				groupBox2.Text=Lan.g(this,"Clock In Date and Time");
				groupTimeSpans.Visible=false;
				groupRate2.Visible=false;
			}
			if(PrefC.HasClinicsEnabled) {
				comboClinic.SelectedClinicNum=ClockEventCur.ClinicNum;
			}
			//Set Text Fields----------------
			FillInitialControlsHelper();
		}

		///<summary>Fills all controls based on the values of ClockEventCur, which is a copy of the object from the DB.</summary>
		private void FillInitialControlsHelper() {
			//Clock In/Out fields---------------------------------------------------------------------
			textTimeEntered1.Text=ClockEventCur.TimeEntered1.ToString();
			textTimeDisplayed1.Text=ClockEventCur.TimeDisplayed1.ToString();
			if(ClockEventCur.TimeEntered2.Year>1880){
				textTimeEntered2.Text=ClockEventCur.TimeEntered2.ToString();
			}
			if(ClockEventCur.TimeDisplayed2.Year>1880){
				textTimeDisplayed2.Text=ClockEventCur.TimeDisplayed2.ToString();
			}
			//Clock status (i.e. Home, Lunch, Break)--------------------------------------------------
			listStatus.Items.Clear();
			_listShownTimeClockStatuses.Clear();
			foreach(TimeClockStatus timeClockStatus in Enum.GetValues(typeof(TimeClockStatus))){
				string statusDescript=timeClockStatus.GetDescription();
				if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {
					if(timeClockStatus==TimeClockStatus.Break) {
						continue;//Skip Break option.
					}
					else if(timeClockStatus==TimeClockStatus.Lunch) {
						statusDescript=TimeClockStatus.Break.GetDescription();//Change "Lunch" to "Break", still functions as Lunch.
					}
				}
				_listShownTimeClockStatuses.Add(timeClockStatus);
				listStatus.Items.Add(Lan.g("enumTimeClockStatus",statusDescript));
			}
			//When ClockEventAllowBreak is disabled, ClockStatus still maps to the correct index because 'Break' is the last option. 5/24/18
			listStatus.SelectedIndex=_listShownTimeClockStatuses.IndexOf(ClockEventCur.ClockStatus);//all clockevents have a status
			//Users were complaining that their employees were altering breaks / "lunch" clock events which was causing problems.
			//We will disable listStatus for any user that does not have the ability to edit all time cards (even if it is their own time card).
			//This is so that the user is forced to use the buttons within the Manage module which is more predictable.
			if(!Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
				listStatus.Enabled=false;
			}
			//Time Spans -----------------------------------------------------------------------------
			//Clocked time------------------------------
			TimeSpan clockedTime=TimeSpan.Zero;
			if(ClockEventCur.TimeDisplayed2.Year>1880) {
				clockedTime=ClockEventCur.TimeDisplayed2-ClockEventCur.TimeDisplayed1;
				textClockedTime.Text=ClockEvents.Format(clockedTime);
			}
			//Adj ------------------------------------
			textAdjustAuto.Text=ClockEvents.Format(ClockEventCur.AdjustAuto);
			if(ClockEventCur.AdjustIsOverridden) {
				if(ClockEventCur.Adjust==TimeSpan.Zero) {
					textAdjust.Text="0";
				}
				else {
					textAdjust.Text=ClockEvents.Format(ClockEventCur.Adjust);

				}
			}
			else {
				textAdjust.Text="";
			}
			//Overtime --------------------------------
			textOTimeAuto.Text=ClockEvents.Format(ClockEventCur.OTimeAuto);
			if(ClockEventCur.OTimeHours==TimeSpan.FromHours(-1)) {//no override
				textOTimeHours.Text="";
			}
			else if(ClockEventCur.OTimeHours==TimeSpan.Zero) {
				textOTimeHours.Text="0";
			}
			else {
				textOTimeHours.Text=ClockEvents.Format(ClockEventCur.OTimeHours);
			}
			//Regular Time -----------------------------
			if(clockedTime>TimeSpan.Zero) {
				TimeSpan regularTime=clockedTime
					+(ClockEventCur.AdjustIsOverridden                ?ClockEventCur.Adjust   :ClockEventCur.AdjustAuto)
					-(ClockEventCur.OTimeHours==TimeSpan.FromHours(-1)?ClockEventCur.OTimeAuto:ClockEventCur.OTimeHours);
				textRegTime.Text=ClockEvents.Format(regularTime);
			}
			//Rate 2 spans -----------------------------------------------------------------------------
			if(clockedTime>TimeSpan.Zero) {
				TimeSpan totalTime=clockedTime+(ClockEventCur.AdjustIsOverridden?ClockEventCur.Adjust:ClockEventCur.AdjustAuto);//clockedTime+(Adj or AdjAuto)
				TimeSpan rate1Hours=totalTime-(ClockEventCur.Rate2Hours==TimeSpan.FromHours(-1)?ClockEventCur.Rate2Auto:ClockEventCur.Rate2Hours);//totalTime-(Rate2 or Rate2Auto)
				textTotalHours.Text=ClockEvents.Format(totalTime);
				textRate1Auto.Text=ClockEvents.Format(rate1Hours);
			}
			//Rate 2 Time -----------------------------
			textRate2Auto.Text=ClockEvents.Format(ClockEventCur.Rate2Auto);
			if(ClockEventCur.Rate2Hours==TimeSpan.FromHours(-1)) {
				textRate2Hours.Text="";
			}
			else if(ClockEventCur.Rate2Hours==TimeSpan.Zero) {
				textRate2Hours.Text="0";
			}
			else {
				textRate2Hours.Text=ClockEvents.Format(ClockEventCur.Rate2Hours);
			}
			//notes ------------------------------------------------------------------------------------
			textNote.Text=ClockEventCur.Note;
		}

		///<summary>Fills all controls based on the values of ClockEventCur, which is a copy of the object from the DB.</summary>
		private void FillAutoControlsHelper() {
			//Clock In/Out fields---------------------------------------------------------------------
			textTimeEntered1.Text=ClockEventCur.TimeEntered1.ToString();
			if(ClockEventCur.TimeEntered2.Year>1880) {
				textTimeEntered2.Text=ClockEventCur.TimeEntered2.ToString();
			}
			//Clocked time------------------------------
			TimeSpan clockedTime=TimeSpan.Zero;
			if(ClockEventCur.TimeDisplayed2.Year>1880) {
				clockedTime=ClockEventCur.TimeDisplayed2-ClockEventCur.TimeDisplayed1;
				textClockedTime.Text=ClockEvents.Format(clockedTime);
			}
			//Adj ------------------------------------
			textAdjustAuto.Text=ClockEvents.Format(ClockEventCur.AdjustAuto);
			//Overtime --------------------------------
			textOTimeAuto.Text=ClockEvents.Format(ClockEventCur.OTimeAuto);
			//Regular Time -----------------------------
			if(clockedTime>TimeSpan.Zero) {
				TimeSpan regularTime=clockedTime
					+(ClockEventCur.AdjustIsOverridden                ?ClockEventCur.Adjust   :ClockEventCur.AdjustAuto)
					-(ClockEventCur.OTimeHours==TimeSpan.FromHours(-1)?ClockEventCur.OTimeAuto:ClockEventCur.OTimeHours);
				textRegTime.Text=ClockEvents.Format(regularTime);
			}
			//Rate 2 spans -----------------------------------------------------------------------------
			if(clockedTime>TimeSpan.Zero) {
				TimeSpan totalTime=clockedTime+(ClockEventCur.AdjustIsOverridden?ClockEventCur.Adjust:ClockEventCur.AdjustAuto);//clockedTime+(Adj or AdjAuto)
				TimeSpan rate1Hours=totalTime-(ClockEventCur.Rate2Hours==TimeSpan.FromHours(-1)?ClockEventCur.Rate2Auto:ClockEventCur.Rate2Hours);//totalTime-(Rate2 or Rate2Auto)
				textTotalHours.Text=ClockEvents.Format(totalTime);
				textRate1Auto.Text=ClockEvents.Format(rate1Hours);
			}
			//Rate 2 Time -----------------------------
			textRate2Auto.Text=ClockEvents.Format(ClockEventCur.Rate2Auto);
		}

		private void textTimeDisplayed2_TextChanged(object sender,EventArgs e) {
			try {
				ClockEventCur.TimeDisplayed2=DateTime.Parse(textTimeDisplayed2.Text);
			}
			catch{
				clearAutoFieldsHelper();
				return;
			}
			FillAutoControlsHelper();
		}

		private void textTimeDisplayed1_TextChanged(object sender,EventArgs e) {
			try {
				ClockEventCur.TimeDisplayed1=DateTime.Parse(textTimeDisplayed1.Text);
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
					ClockEventCur.TimeEntered2=DateTime.MinValue;
				}
				else {
					ClockEventCur.TimeEntered2=PIn.Date(textTimeEntered2.Text);
				}
			}
			catch {
				return;
			}
			FillAutoControlsHelper();
		}

		private void textAdjust_TextChanged(object sender,EventArgs e) {
			try {
				if(textAdjust.Text=="") {
					ClockEventCur.AdjustIsOverridden=false;
					ClockEventCur.Adjust=TimeSpan.Zero;
				}
				else {
					ClockEventCur.AdjustIsOverridden=true;
					ClockEventCur.Adjust=TimeSpan.FromHours(Double.Parse(textAdjust.Text));
				}
			}
			catch {
				return;
			}
			FillAutoControlsHelper();
		}

		private void textOvertime_TextChanged(object sender,EventArgs e) {
			try {
				if(textOTimeHours.Text=="") {
					ClockEventCur.OTimeHours=TimeSpan.FromHours(-1);
				}
				else {
					//ClockEventCur.OTimeHours=PIn.Time(textOTimeHours.Text);
					ClockEventCur.OTimeHours=TimeSpan.FromHours(Double.Parse(textOTimeHours.Text));
				}
			}
			catch {
				return;
			}
			FillAutoControlsHelper();
		}

		private void textRate2Hours_TextChanged(object sender,EventArgs e) {
			try {
				if(textRate2Hours.Text=="") {
					ClockEventCur.Rate2Hours=TimeSpan.Zero;
				}
				else {
					//ClockEventCur.Rate2Hours=PIn.Time(textRate2Hours.Text);
					ClockEventCur.Rate2Hours=TimeSpan.FromHours(Double.Parse(textRate2Hours.Text));
				}
			}
			catch {
				return;
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
				ClockEventCur.TimeEntered2=MiscData.GetNowDateTime();
			}
			//FillTimeSpans();//not really needed because of the TextChanged event, but might prevent a bug.
			FillAutoControlsHelper();
		}

		private void butClear_Click(object sender,EventArgs e) {
			textTimeDisplayed2.Text="";
			textTimeEntered2.Text="";
			ClockEventCur.TimeEntered2=DateTime.MinValue;
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
			ClockEvents.Delete(ClockEventCur.ClockEventNum);
			Employees.UpdateClockStatus(ClockEventCur.EmployeeNum);
			SecurityLogs.MakeLogEntry(Permissions.TimecardDeleteEntry,0,
				"Original entry: "+ClockEventCur.TimeEntered1.ToString());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//if(textAmountBonus.errorProvider1.GetError(textAmountBonus)!="") {
			//  MsgBox.Show(this,"Please enter in a valid dollar amount for Bonus.");
			//  return;
			//}
			DateTime timeDisplayed1=DateTime.MinValue;
			try{
				timeDisplayed1=DateTime.Parse(textTimeDisplayed1.Text);//because this must always be valid
			}
			catch{
				if(ClockEventCur.ClockStatus==TimeClockStatus.Break){
					MsgBox.Show(this,"Please enter a valid clock-out date and time.");
				}
				else{
					MsgBox.Show(this,"Please enter a valid clock-in date and time.");
				}
				return;
			}
			if(timeDisplayed1.Date > DateTime.Today) {
				if(ClockEventCur.ClockStatus==TimeClockStatus.Break){
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
					if(ClockEventCur.ClockStatus==TimeClockStatus.Break){
						MsgBox.Show(this,"Please enter a valid clock-in date and time.");
					}
					else{
						MsgBox.Show(this,"Please enter a valid clock-out date and time.");
					}
					return;
				}
			}
			if(timeDisplayed2.Date > DateTime.Today) {
				if(ClockEventCur.ClockStatus==TimeClockStatus.Break){
					MsgBox.Show(this,"Clock-in date cannot be a future date.");
				}
				else{
					MsgBox.Show(this,"Clock-out date cannot be a future date.");
				}
				return;
			}
			if(textTimeDisplayed2.Text!="" && timeDisplayed1 > timeDisplayed2){
				if(ClockEventCur.ClockStatus==TimeClockStatus.Break) {
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
			if(PayPeriods.CannotEditPayPeriodOfDate(timeDisplayed1,ClockEventCur.EmployeeNum)) {
				string message=Lan.g(this,"You only have permission to edit your time card for the current pay period. The ");
				string messageTwo=Lan.g(this," you have entered does not fall within the current pay period.");
				MessageBox.Show(message+groupBox1.Text+messageTwo);
				return;
			}
			TimeSpan overtime=TimeSpan.Zero;
			TimeSpan adjust=TimeSpan.Zero;
			if(ClockEventCur.ClockStatus!=TimeClockStatus.Break) {
				if(textOTimeHours.Text!="") {
					try {
						if(textOTimeHours.Text.Contains(":")) {
							overtime=TimeSpan.Parse(textOTimeHours.Text);
						}
						else {
							overtime=TimeSpan.FromHours(Double.Parse(textOTimeHours.Text));
						}
						if(overtime < TimeSpan.Zero) {
							MsgBox.Show(this,"Overtime must be positive.");
							return;
						}
					}
					catch {
						MsgBox.Show(this,"Please enter a valid overtime amount.");
						return;
					}
				}
				if(textAdjust.Text!="") {
					try {
						if(textAdjust.Text.Contains(":")) {
							adjust=TimeSpan.Parse(textAdjust.Text);
						}
						else {
							adjust=TimeSpan.FromHours(Double.Parse(textAdjust.Text));
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
				ClockEventCur.TimeEntered2=MiscData.GetNowDateTime();
			}
			ClockEventCur.TimeDisplayed1=timeDisplayed1;
			ClockEventCur.TimeDisplayed2=timeDisplayed2;
			ClockEventCur.ClockStatus=_listShownTimeClockStatuses[listStatus.SelectedIndex];
			if(textAdjust.Text=="") {//no override
				ClockEventCur.AdjustIsOverridden=false;
				ClockEventCur.Adjust=TimeSpan.Zero;
			}
			else {
				ClockEventCur.AdjustIsOverridden=true;
				ClockEventCur.Adjust=adjust;
			}
			if(textOTimeHours.Text=="") {//no override
				ClockEventCur.OTimeHours=TimeSpan.FromHours(-1d);
			}
			else {
				ClockEventCur.OTimeHours=overtime;
			}
			if(textRate2Hours.Text=="") {
				ClockEventCur.Rate2Hours=TimeSpan.FromHours(-1);
			}
			//The two auto fields are only set externally.
			ClockEventCur.Note=textNote.Text;
			if(PrefC.HasClinicsEnabled) {
				ClockEventCur.ClinicNum=comboClinic.SelectedClinicNum;
			}
			ClockEvents.Update(ClockEventCur);
			Employees.UpdateClockStatus(ClockEventCur.EmployeeNum);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}