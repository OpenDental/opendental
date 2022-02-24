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
		public FormPayPeriodManager()
		{
			//
			// Required for Windows Form Designer support
			//
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
				numPayPeriods.Text="52";
			}
			else if(payPeriodInterval==PayPeriodInterval.BiWeekly) {
				radioBiWeekly.Checked=true;
				numPayPeriods.Text="26";
			}
			else if(payPeriodInterval==PayPeriodInterval.Monthly) {
				radioMonthly.Checked=true;
				numPayPeriods.Text="12";
			}
			int dayOfWeek=PrefC.GetInt(PrefName.PayPeriodPayDay);
			if(dayOfWeek!=0) {//They have a day of the week selected
				comboDay.SelectedIndex=dayOfWeek;
				numDaysAfterPayPeriod.Enabled=false;
				checkExcludeWeekends.Enabled=false;
				radioPayBefore.Enabled=false;
				radioPayAfter.Enabled=false;
			}
			else {
				comboDay.SelectedIndex=0;
				numDaysAfterPayPeriod.Text=PrefC.GetString(PrefName.PayPeriodPayAfterNumberOfDays);
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
				if(_listPayPeriods[i].DatePaycheck.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listPayPeriods[i].DatePaycheck.ToShortDateString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e) {
			//Allowing modification of pay periods here will cause insertions/updates.  It would require changing in FormPayPeriodEdit to not insert/update.
			using FormPayPeriodEdit FormP=new FormPayPeriodEdit(_listPayPeriods[e.Row],_listPayPeriods);
			FormP.IsSaveToDb=false;
			FormP.IsNew=true;
			if(FormP.ShowDialog()==DialogResult.Abort) {
				_listPayPeriods.RemoveAt(e.Row);
			}
			FillGrid();
		}

		private void butGenerate_Click(object sender, EventArgs e) {
			//Generate payperiods based on settings
			if(!numPayPeriods.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(numDaysAfterPayPeriod.Enabled && !numDaysAfterPayPeriod.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(numDaysAfterPayPeriod.Enabled && numDaysAfterPayPeriod.Text=="0") {
				MsgBox.Show(this,"# Days After Pay Period cannot be zero.");
				return;
			}
			_listPayPeriods.Clear();
			int numPeriods=PIn.Int(numPayPeriods.Text);
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
					payPeriod.DatePaycheck=payPeriod.DateStop.AddDays(PIn.Int(numDaysAfterPayPeriod.Text));
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
			DateTime result=startDate.AddDays(1);//PayDate cannot be the same as the last day of the pay period.
			while(result.DayOfWeek!=day) {
        result=result.AddDays(1);
			}
			return result;
		}

		private void radioMonthly_Click(object sender,EventArgs e) {
			numPayPeriods.Text="12";
		}

		private void radioBiWeekly_Click(object sender,EventArgs e) {
			numPayPeriods.Text="26";
		}

		private void radioWeekly_Click(object sender,EventArgs e) {
			numPayPeriods.Text="52";
		}

		private void comboDay_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboDay.SelectedIndex!=0) {
				numDaysAfterPayPeriod.Text="0";
				numDaysAfterPayPeriod.Enabled=false;
				checkExcludeWeekends.Enabled=false;
				checkExcludeWeekends.Checked=false;
				radioPayBefore.Enabled=false;
				radioPayBefore.Checked=false;
				radioPayAfter.Enabled=false;
				radioPayAfter.Checked=false;
			}
			else {//none selected
				numDaysAfterPayPeriod.Text="0";
				numDaysAfterPayPeriod.Enabled=true;
				checkExcludeWeekends.Enabled=true;
				radioPayBefore.Enabled=true;
				radioPayAfter.Enabled=true;
			}
		}

		private void numDaysAfterPayPeriod_TextChanged(object sender,EventArgs e) {
			if(numDaysAfterPayPeriod.Text!="0" && numDaysAfterPayPeriod.Text!="") {
				comboDay.SelectedIndex=0;
				comboDay.Enabled=false;
				checkExcludeWeekends.Enabled=true;
				radioPayBefore.Enabled=true;
				radioPayAfter.Enabled=true;
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
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"Pay periods must be generated first.");
				return;
			}
			if(!numDaysAfterPayPeriod.IsValid()) {
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
			else {
				Prefs.UpdateInt(PrefName.PayPeriodIntervalSetting,(int)PayPeriodInterval.Monthly);
			}
			Prefs.UpdateInt(PrefName.PayPeriodPayDay,comboDay.SelectedIndex);
			Prefs.UpdateInt(PrefName.PayPeriodPayAfterNumberOfDays,PIn.Int(numDaysAfterPayPeriod.Text));
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





















