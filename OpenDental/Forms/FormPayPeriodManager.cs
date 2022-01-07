using System;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental {

	public partial class FormPayPeriodManager:FormODBase {
		private List<PayPeriod> _listPayPeriods;

		///<summary></summary>
		public FormPayPeriodManager() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayPeriodManager_Load(object sender, System.EventArgs e) {
			PayPeriod payPeriod=PayPeriods.GetMostRecent();
			if(payPeriod==null) {
				dateTimeStart.Value=DateTime.Today;
			}
			else {
				dateTimeStart.Value=payPeriod.DateStop.AddDays(1);
			}
			PayPeriodInterval payPeriodInterval=(PayPeriodInterval)PrefC.GetInt(PrefName.PayPeriodIntervalSetting);
			if(payPeriodInterval==PayPeriodInterval.Weekly) {
				radioWeekly.Checked=true;
				textPayPeriods.Text="52";
			}
			else if(payPeriodInterval==PayPeriodInterval.BiWeekly) {
				radioBiWeekly.Checked=true;
				textPayPeriods.Text="26";
			}
			else if(payPeriodInterval==PayPeriodInterval.Monthly) {
				radioMonthly.Checked=true;
				textPayPeriods.Text="12";
			}
			else if(payPeriodInterval==PayPeriodInterval.SemiMonthly) {
				radioSemiMonthly.Checked=true;
				textPayPeriods.Text="24";
				groupBoxSemiMonthly.Enabled=true;
			}
			int dayOfWeek=PrefC.GetInt(PrefName.PayPeriodPayDay);
			if(dayOfWeek!=0) {//They have a day of the week selected
				comboDay.SelectedIndex=dayOfWeek;
				textDaysAfterPayPeriod.Enabled=false;
				checkExcludeWeekends.Enabled=false;
				radioPayBefore.Enabled=false;
				radioPayAfter.Enabled=false;
			}
			else {
				comboDay.SelectedIndex=0;
				textDaysAfterPayPeriod.Text=PrefC.GetString(PrefName.PayPeriodPayAfterNumberOfDays);
				checkExcludeWeekends.Checked=PrefC.GetBool(PrefName.PayPeriodPayDateExcludesWeekends);
				if(checkExcludeWeekends.Checked) {
					if(PrefC.GetBool(PrefName.PayPeriodPayDateBeforeWeekend)) {
						radioPayBefore.Checked=true;
					}
					else {
						radioPayAfter.Checked=true;
					}
				}
				if(!checkExcludeWeekends.Checked) {
					radioPayBefore.Checked=false;
					radioPayBefore.Enabled=false;
					radioPayAfter.Checked=false;
					radioPayAfter.Enabled=false;
				}
				else {
					radioPayBefore.Enabled=true;
					radioPayAfter.Enabled=true;
				}
			}
			_listPayPeriods=new List<PayPeriod>();
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Start Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("End Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Paycheck Date",100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listPayPeriods.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listPayPeriods[i].DateStart.ToShortDateString());
				row.Cells.Add(_listPayPeriods[i].DateStop.ToShortDateString());
				if(_listPayPeriods[i].DatePaycheck.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listPayPeriods[i].DatePaycheck.ToShortDateString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void GenerateSemiMonthlyPayPeriods() {
			int numPeriods=PIn.Int(textPayPeriods.Text);
			int numPeriodsGenerated=0;
			int daysAfterPayPeriod=textDaysAfterPayPeriod.Value;
			bool hasChosenEndDates=radioEndDate.Checked;
			bool doExcludeWeekends=checkExcludeWeekends.Checked;
			bool doPayBeforeWeekends=radioPayBefore.Checked;
			DateTime dateTimeStartPayPeriod=dateTimeStart.Value;
			DateTime dateTimeEndPayPeriod;
			_listPayPeriods.Clear();
			#region Valdation
			if(!textDay1.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(!textDay2.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			int period1Day=PIn.Int(textDay1.Text);
			int period2Day=PIn.Int(textDay2.Text);
			if(period2Day<=period1Day && !checkLast.Checked) {
				MsgBox.Show(this,"Period 2 day must be later than period 1 day.");
				return;
			}
			if((period2Day>=29 && !checkLast.Checked) || period1Day>=29) {
				MsgBox.Show(this,"Day must be less than 29.");
				return;
			}
			if((period1Day>=28 && checkLast.Checked)) {
				MsgBox.Show(this,"Day 1 must be less than 28.");
				return;
			}
			//If payperiods are too short, they can sometimes overlap when paying before weekends. Prevent This.
			if(checkLast.Checked) {
				if(period1Day<=4 || period1Day>=26) {
					MsgBox.Show(this,"Pay Periods must be at least 5 days long.");
					return;
				}
			}
			else {
				int periodLength=Math.Abs(period2Day-period1Day);
				if(periodLength<=4||periodLength>=27) {
					MsgBox.Show(this,"Pay Periods must be at least 5 days long.");
					return;
				}
			}
			#endregion
			//Adjust the input such that the first payperiod generated will include the selected day
			#region Adjust Input	
			if(checkLast.Checked) {
				period2Day=System.DateTime.DaysInMonth(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month);
			}
			if(radioPayDate.Checked) {	
				//Compensate for offset when selecting pay days, this ensures that the first period includes the start day when using pay days.
				dateTimeStartPayPeriod=dateTimeStartPayPeriod.AddDays(daysAfterPayPeriod);
			}
			if(dateTimeStartPayPeriod.Day<=textDay1.Value) {
				//Chosen day is before first day, so move start date  back a month to the day before period 2 day.
				//If last is checked. use last from previous month, not period 2 day.
				DateTime dateTimePrevMonth=dateTimeStartPayPeriod.AddMonths(-1);
				if(checkLast.Checked) {
					dateTimeStartPayPeriod=new DateTime(dateTimePrevMonth.Year, dateTimePrevMonth.Month, DateTime.DaysInMonth(dateTimePrevMonth.Year,dateTimePrevMonth.Month)).AddDays(-1);
				}
				else {
					dateTimeStartPayPeriod=new DateTime(dateTimePrevMonth.Year, dateTimePrevMonth.Month, period2Day).AddDays(-1);
				}
			}
			else if(dateTimeStartPayPeriod.Day>textDay1.Value && dateTimeStartPayPeriod.Day<=period2Day){	
				//Chosen day is between the two days, so move it back to the day before period 1
				dateTimeStartPayPeriod=new DateTime(dateTimeStartPayPeriod.Year, dateTimeStartPayPeriod.Month, period1Day).AddDays(-1);
			}
			else if(dateTimeStartPayPeriod.Day>period2Day) {	
				//Chosen day is afer both days, so move it back too the  day before period 2 day
				dateTimeStartPayPeriod=new DateTime(dateTimeStartPayPeriod.Year, dateTimeStartPayPeriod.Month, period2Day).AddDays(-1);
			}
			#endregion
			#region Generate Pay Periods
			while(numPeriodsGenerated<numPeriods) {//will add either one or zero items to the list on each pass. zero only on first pass
				if(checkLast.Checked) {
					period2Day=System.DateTime.DaysInMonth(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month);
				}
				PayPeriod payPeriod=new PayPeriod();
				#region Current Day After Both Days
				if(dateTimeStartPayPeriod.Day>period1Day && dateTimeStartPayPeriod.Day>period2Day) {			//the current day is after both periods of the month 
					dateTimeStartPayPeriod=dateTimeStartPayPeriod.AddDays(-dateTimeStartPayPeriod.Day+1);		//only runs when you choose a start day after both periods
					dateTimeStartPayPeriod=dateTimeStartPayPeriod.AddMonths(1);															//go to the beginning of next month						
				}
				#endregion
				#region Current Day Before Both Days
				else if(dateTimeStartPayPeriod.Day<=period1Day) {
					dateTimeStartPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period1Day).AddDays(1);
					dateTimeEndPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period2Day);			
					if(hasChosenEndDates) {//end days selected
						if(_listPayPeriods.Count>0) {
							payPeriod.DateStart=_listPayPeriods.Last().DateStop.AddDays(1);
						}
						else {
							payPeriod.DateStart=dateTimeStartPayPeriod;
						}
						payPeriod.DateStop=dateTimeEndPayPeriod;
						payPeriod.DatePaycheck=dateTimeEndPayPeriod.AddDays(daysAfterPayPeriod);
					}
					else {//pay days selected
						if(_listPayPeriods.Count>0) {
							payPeriod.DateStart=_listPayPeriods.Last().DateStop.AddDays(1);
						}
						else {
							payPeriod.DateStart=dateTimeStartPayPeriod.AddDays(-daysAfterPayPeriod);
						}
						payPeriod.DateStop=dateTimeEndPayPeriod.AddDays(-daysAfterPayPeriod);
						payPeriod.DatePaycheck=dateTimeEndPayPeriod;						
					}
					_listPayPeriods.Add(payPeriod);
					dateTimeStartPayPeriod=dateTimeStartPayPeriod.AddDays(period2Day-period1Day-1);	//go to second payperiod
					numPeriodsGenerated++;
				}
				#endregion
				#region Current Day Between Both Days
				else if(dateTimeStartPayPeriod.Day>period1Day && dateTimeStartPayPeriod.Day<=period2Day) {
					if(dateTimeStartPayPeriod.Month==12) {	//case where pay period is split between years
						dateTimeStartPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period2Day).AddDays(1);
						if(checkLast.Checked) {	//case where it spans years and period 2 day is the 31st. dateTimeStartPayPeriod is jan 1st
							dateTimeEndPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period1Day);
						}
						else {
							dateTimeEndPayPeriod= new DateTime(dateTimeStartPayPeriod.Year+1,1,period1Day);
						}
					}
					else {
						dateTimeStartPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period2Day).AddDays(1);
						if(checkLast.Checked) {
							dateTimeEndPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period1Day);
						}
						else {
							dateTimeEndPayPeriod= new DateTime(dateTimeStartPayPeriod.Year,dateTimeStartPayPeriod.Month,period1Day).AddMonths(1);
						}
					}
					if(hasChosenEndDates) {//end days selected		
						if(_listPayPeriods.Count>0) {
							payPeriod.DateStart=_listPayPeriods.Last().DateStop.AddDays(1);
						}
						else {
							payPeriod.DateStart=dateTimeStartPayPeriod;
						}
						payPeriod.DateStop=dateTimeEndPayPeriod;
						payPeriod.DatePaycheck=dateTimeEndPayPeriod.AddDays(daysAfterPayPeriod);
					}
					else {//pay days selected
						if(_listPayPeriods.Count>0) {
							payPeriod.DateStart=_listPayPeriods.Last().DateStop.AddDays(1);
						}
						else {
							payPeriod.DateStart=dateTimeStartPayPeriod.AddDays(-daysAfterPayPeriod);
						}
						payPeriod.DateStop=dateTimeEndPayPeriod.AddDays(-daysAfterPayPeriod);
						payPeriod.DatePaycheck=dateTimeEndPayPeriod;						
					}
					_listPayPeriods.Add(payPeriod);
					dateTimeStartPayPeriod=dateTimeStartPayPeriod.AddDays(-dateTimeStartPayPeriod.Day+1);
					if(!checkLast.Checked) {
						dateTimeStartPayPeriod=dateTimeStartPayPeriod.AddMonths(1);	//go to the beginning of the next month
					}
					numPeriodsGenerated++;
				}
				#endregion
				//check if last added pay day was a weekend, adjust it if needed
				#region Adjust Weekends
				if(_listPayPeriods.Count>0) {
					if(_listPayPeriods.Last().DatePaycheck.DayOfWeek==DayOfWeek.Saturday && doExcludeWeekends) {
						if(doPayBeforeWeekends) {	//Paying before the weekend affects period end date. It is moved back until it is the specified num of days before payday.		
							_listPayPeriods.Last().DatePaycheck=_listPayPeriods.Last().DatePaycheck.AddDays(-1);//friday
							while(_listPayPeriods.Last().DatePaycheck<_listPayPeriods.Last().DateStop.AddDays(textDaysAfterPayPeriod.Value)) {
								_listPayPeriods.Last().DateStop=_listPayPeriods.Last().DateStop.AddDays(-1);
							}
						}
						else {													
							_listPayPeriods.Last().DatePaycheck=_listPayPeriods.Last().DatePaycheck.AddDays(2);//monday
						}
					}
					else if(_listPayPeriods.Last().DatePaycheck.DayOfWeek==DayOfWeek.Sunday && doExcludeWeekends) {
						if(doPayBeforeWeekends) {						
							_listPayPeriods.Last().DatePaycheck=_listPayPeriods.Last().DatePaycheck.AddDays(-2);//friday
							while(_listPayPeriods.Last().DatePaycheck<_listPayPeriods.Last().DateStop.AddDays(textDaysAfterPayPeriod.Value)) {
								_listPayPeriods.Last().DateStop=_listPayPeriods.Last().DateStop.AddDays(-1);
							}
						}
						else {													
							_listPayPeriods.Last().DatePaycheck=_listPayPeriods.Last().DatePaycheck.AddDays(1);//monday
						}
					}
				}
				#endregion
			}
			#endregion
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e) {
			//Allowing modification of pay periods here will cause insertions/updates.  It would require changing in FormPayPeriodEdit to not insert/update.
			using FormPayPeriodEdit FormPayPeriodEdit=new FormPayPeriodEdit(_listPayPeriods[e.Row],_listPayPeriods);
			FormPayPeriodEdit.IsSaveToDb=false;
			FormPayPeriodEdit.IsNew=true;
			if(FormPayPeriodEdit.ShowDialog()==DialogResult.Abort) {
				_listPayPeriods.RemoveAt(e.Row);
			}
			FillGrid();
		}

		private void butGenerate_Click(object sender, EventArgs e) {
			//Generate payperiods based on settings
			if(!textPayPeriods.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDaysAfterPayPeriod.Enabled && !textDaysAfterPayPeriod.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDaysAfterPayPeriod.Enabled && textDaysAfterPayPeriod.Text=="0") {
				MsgBox.Show(this,"# Days After Pay Period cannot be zero.");
				return;
			}
			if(radioSemiMonthly.Checked) {
				GenerateSemiMonthlyPayPeriods();
				return;
			}
			_listPayPeriods.Clear();
			int numPeriods=PIn.Int(textPayPeriods.Text);
			PayPeriodInterval payPeriodInterval=PayPeriodInterval.Weekly;
			if(radioBiWeekly.Checked) {
				payPeriodInterval=PayPeriodInterval.BiWeekly;
			}
			else if(radioMonthly.Checked) {
				payPeriodInterval=PayPeriodInterval.Monthly;
			}
			DateTime startDate=dateTimeStart.Value;//Original value
			for(int i=0;i<numPeriods;i++) {
				PayPeriod payPeriod=new PayPeriod();
				payPeriod.DateStart=startDate;
				payPeriod.IsNew=true;
				//Make PayDate information
				switch(payPeriodInterval) {//Add time to "startDate" to get the new start date for the next iteration as well as figuring out the end date for current payperiod.
					case PayPeriodInterval.Weekly:
						payPeriod.DateStop=startDate.AddDays(6);
						startDate=startDate.AddDays(7);
						break;
					case PayPeriodInterval.BiWeekly:
						payPeriod.DateStop=startDate.AddDays(13);
						startDate=startDate.AddDays(14);
						break;
					case PayPeriodInterval.Monthly:
						payPeriod.DateStop=startDate.AddMonths(1).Subtract(TimeSpan.FromDays(1));
						startDate=startDate.AddMonths(1);
						break;
				}
				if(comboDay.Enabled) {
					//Find the closest day specified after the end of the pay period.
					payPeriod.DatePaycheck=GetDateOfDay(payPeriod.DateStop,(DayOfWeek)(comboDay.SelectedIndex-1));
				}
				else {//# days specified, use "Exclude Weekends" checkbox as well as "Pay Before" and "Pay After" buttons.
					payPeriod.DatePaycheck=payPeriod.DateStop.AddDays(PIn.Int(textDaysAfterPayPeriod.Text));
					if(payPeriod.DatePaycheck.DayOfWeek==DayOfWeek.Saturday && checkExcludeWeekends.Checked) {
						if(radioPayBefore.Checked) {
							if(payPeriod.DatePaycheck.Subtract(TimeSpan.FromDays(1))<=payPeriod.DateStop) {//Can't move the paycheck date to the same day (or before) than the date end.
								payPeriod.DatePaycheck=payPeriod.DatePaycheck.Add(TimeSpan.FromDays(2));//Move it forward to monday
							}
							else {
								payPeriod.DatePaycheck=payPeriod.DatePaycheck.Subtract(TimeSpan.FromDays(1));//Move it back to friday
							}
						}
						else {//radioPayAfter
							payPeriod.DatePaycheck=payPeriod.DatePaycheck.Add(TimeSpan.FromDays(2));//Move it forward to monday
						}
					}
					else if(payPeriod.DatePaycheck.DayOfWeek==DayOfWeek.Sunday && checkExcludeWeekends.Checked) {
						if(radioPayBefore.Checked) {
							if(payPeriod.DatePaycheck.Subtract(TimeSpan.FromDays(2))<=payPeriod.DateStop) {//Can't move the paycheck date to the same day (or before) than the date end.
								payPeriod.DatePaycheck=payPeriod.DatePaycheck.Add(TimeSpan.FromDays(1));//Move it forward to monday
							}
							else {
								payPeriod.DatePaycheck=payPeriod.DatePaycheck.Subtract(TimeSpan.FromDays(2));//Move it back to friday
							}
						}
						else {//radioPayAfter
							payPeriod.DatePaycheck=payPeriod.DatePaycheck.Add(TimeSpan.FromDays(1));//Move it forward to monday
						}
					}
				}
				_listPayPeriods.Add(payPeriod);
			}
			FillGrid();
		}

		///<summary>Returns the DateTime of the first instance of DayOfWeek, given a specific start time.  It will not include the startDate as a result.</summary>
		private DateTime GetDateOfDay(DateTime startDate,DayOfWeek day) {
			DateTime dateTimeResult=startDate.AddDays(1);//PayDate cannot be the same as the last day of the pay period.
			while(dateTimeResult.DayOfWeek!=day) {
        dateTimeResult=dateTimeResult.AddDays(1);
			}
			return dateTimeResult;
		}

		private void radioMonthly_Click(object sender,EventArgs e) {
			textPayPeriods.Text="12";
			groupBoxSemiMonthly.Enabled=false;
			if(textDaysAfterPayPeriod.Text=="0") {
				comboDay.Enabled=true;
			}
		}

		private void radioBiWeekly_Click(object sender,EventArgs e) {
			textPayPeriods.Text="26";
			groupBoxSemiMonthly.Enabled=false;
			if(textDaysAfterPayPeriod.Text=="0") {
				comboDay.Enabled=true;
			}
		}

		private void radioWeekly_Click(object sender,EventArgs e) {
			textPayPeriods.Text="52";
			groupBoxSemiMonthly.Enabled=false;
			if(textDaysAfterPayPeriod.Text=="0") {
				comboDay.Enabled=true;
			}
		}

		private void butSemiMonthly_Click(object sender,EventArgs e) {
			textPayPeriods.Text="24";
			groupBoxSemiMonthly.Enabled=true;
			comboDay.SelectedIndex=0;
			comboDay.Enabled=false;
			textDaysAfterPayPeriod.Enabled=true;
			checkExcludeWeekends.Enabled=true;
		}

		private void checkLast_Click(object sender,EventArgs e) {
			if(checkLast.Checked) {
				textDay2.Enabled=false;
				textDay2.Text="";
			}
			else {
				textDay2.Enabled=true;
				textDay2.Text="16";
			}
		}

		private void comboDay_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboDay.SelectedIndex!=0) {
				textDaysAfterPayPeriod.Text="0";
				textDaysAfterPayPeriod.Enabled=false;
				checkExcludeWeekends.Enabled=false;
				checkExcludeWeekends.Checked=false;
				radioPayBefore.Enabled=false;
				radioPayBefore.Checked=false;
				radioPayAfter.Enabled=false;
				radioPayAfter.Checked=false;
			}
			else {//none selected
				textDaysAfterPayPeriod.Text="0";
				textDaysAfterPayPeriod.Enabled=true;
				checkExcludeWeekends.Enabled=true;
			}
		}

		private void numDaysAfterPayPeriod_TextChanged(object sender,EventArgs e) {
			if((textDaysAfterPayPeriod.Text!="0" && textDaysAfterPayPeriod.Text!="") || radioSemiMonthly.Checked) {
				comboDay.SelectedIndex=0;
				comboDay.Enabled=false;
				checkExcludeWeekends.Enabled=true;
			}
			else {
				comboDay.Enabled=true;
			}
		}

		private void checkExcludeWeekends_CheckedChanged(object sender,EventArgs e) {
			if(!checkExcludeWeekends.Checked) {
				radioPayBefore.Checked=false;
				radioPayBefore.Enabled=false;
				radioPayAfter.Checked=false;
				radioPayAfter.Enabled=false;
			}
			else {
				radioPayBefore.Enabled=true;
				radioPayAfter.Enabled=true;
				if(PrefC.GetBool(PrefName.PayPeriodPayDateBeforeWeekend)) {
					radioPayBefore.Checked=true;
				}
				else {
					radioPayAfter.Checked=true;
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"Pay periods must be generated first.");
				return;
			}
			if(!textDaysAfterPayPeriod.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			PayPeriods.RefreshCache(); //Refresh the cache to include any other changes that might have been made in FormTimeCardSetup.
			//overlapping logic
			if(PayPeriods.AreAnyOverlapping(PayPeriods.GetDeepCopy(),_listPayPeriods)) {
				MsgBox.Show(this,"You have created pay periods that would overlap with existing pay periods. Please fix those pay periods first.");
				return;
			}
			//Save payperiods
			foreach(PayPeriod payPeriod in _listPayPeriods) {//PayPeriods are always new in this form.
				PayPeriods.Insert(payPeriod);
			}
			//Save Preferences
			if(radioWeekly.Checked) {
				Prefs.UpdateInt(PrefName.PayPeriodIntervalSetting,(int)PayPeriodInterval.Weekly);
			}
			else if(radioBiWeekly.Checked) {
				Prefs.UpdateInt(PrefName.PayPeriodIntervalSetting,(int)PayPeriodInterval.BiWeekly);
			}
			else if(radioMonthly.Checked) {
				Prefs.UpdateInt(PrefName.PayPeriodIntervalSetting,(int)PayPeriodInterval.Monthly);
			}
			else {
				Prefs.UpdateInt(PrefName.PayPeriodIntervalSetting,(int)PayPeriodInterval.SemiMonthly);
			}
			Prefs.UpdateInt(PrefName.PayPeriodPayDay,comboDay.SelectedIndex);
			Prefs.UpdateInt(PrefName.PayPeriodPayAfterNumberOfDays,PIn.Int(textDaysAfterPayPeriod.Text));
			Prefs.UpdateBool(PrefName.PayPeriodPayDateExcludesWeekends,checkExcludeWeekends.Checked);
			if(radioPayBefore.Checked) {
				Prefs.UpdateBool(PrefName.PayPeriodPayDateBeforeWeekend,true);
			}
			else if(radioPayAfter.Checked) {
				Prefs.UpdateBool(PrefName.PayPeriodPayDateBeforeWeekend,false);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}





















