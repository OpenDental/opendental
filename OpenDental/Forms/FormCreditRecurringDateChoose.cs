using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCreditRecurringDateChoose:FormODBase {
		private CreditCard _creditCard;
		public DateTime DatePay;
		private Patient _patient;

		public FormCreditRecurringDateChoose(CreditCard creditCard,Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			_creditCard=creditCard;
			_patient=patient;
			Lan.F(this);
		}

		private void FormCreditRecurringDateChoose_Load(object sender,EventArgs e) {
			if(FillComboBoxMonthSelect()) {
				return;//We are using the comboBox instead of two buttons.
			}
			//We are using the This Month/Last Month buttons instead of the comboBox.
			DateTime dateThisMonth=GetValidPayDate(DateTime.Today);
			DateTime dateLastMonth=GetValidPayDate(DateTime.Today.AddMonths(-1));
			if(PrefC.GetBool(PrefName.RecurringChargesUseTransDate)) {
				//Labels set here, buttons set in respective button methods butLastMonth_Click/butThisMonth_Click
				labelLastMonth.Text=Lan.g(this,"Recurring charge date will be:")+" "+dateLastMonth.ToShortDateString();
				labelThisMonth.Text=Lan.g(this,"Recurring charge date will be:")+" "+dateThisMonth.ToShortDateString();
			}
			else {
				labelLastMonth.Text+=" "+dateLastMonth.ToShortDateString();
				labelThisMonth.Text+=" "+dateThisMonth.ToShortDateString();
			}
			//If the recurring pay date is in the future do not let them choose that option.
			if(dateThisMonth>DateTime.Now) {
				butThisMonth.Enabled=false;
				labelThisMonth.Text=Lan.g(this,"Cannot make payment for future date:")+" "+dateThisMonth.ToShortDateString();
			}
			if(dateLastMonth<_creditCard.DateStart) {
				labelLastMonth.Text=Lan.g(this,"Cannot make payment before start date:")+" "+ _creditCard.DateStart;
				butLastMonth.Enabled=false;
			}
			if(dateThisMonth<_creditCard.DateStart) {
				labelThisMonth.Text=Lan.g(this,"Cannot make payment before start date:")+" "+ _creditCard.DateStart;
				butThisMonth.Enabled=false;
			}
		}

		///<summary>Fills comboBoxMonthSelect when appropriate, otherwise butThisMonth and butLastMonth are used.  Returns true if comboBoxMonthSelect
		///will be used instead of butThisMonth/butLastMonth.</summary>
		private bool FillComboBoxMonthSelect() {
			if(CreditCards.GetFrequencyType(_creditCard.ChargeFrequency)==ChargeFrequencyType.FixedDayOfMonth) {
				//EX: 2nd and 15th of each Month, instance of a singular day being charged is in GetValidPayDate(...)
				List<int> listDaysOfMonth=CreditCards.GetDaysOfMonthForChargeFrequency(_creditCard.ChargeFrequency).Split(',').Select(x => PIn.Int(x))
					.OrderByDescending(x => x).ToList();
				if(listDaysOfMonth.Count>1) {
					comboBoxMonthSelect.Items.Clear();
					List<DateTime> listDateTimes=new List<DateTime>();
					//For each recurring charge day, get the closest payment and previous payment dates on or after the start date of the recurring charge.
					for(int i=0;i<listDaysOfMonth.Count;i++) {
						int monthOffset=0;
						if(listDaysOfMonth[i]>DateTime.Today.Day) {
							monthOffset=-1;
						}
						DateTime thisMonth=GetDateForDayOfMonth(DateTime.Today.AddMonths(monthOffset),listDaysOfMonth[i]); //This Month/Closest Month
						DateTime lastMonth=GetDateForDayOfMonth(DateTime.Today.AddMonths(monthOffset-1),listDaysOfMonth[i]); //Last Month/Previous Month
						if(thisMonth>=_creditCard.DateStart){ 
							listDateTimes.Add(thisMonth); 
						}
						if(lastMonth>=_creditCard.DateStart) {
							listDateTimes.Add(lastMonth);
						}
					}
					listDateTimes=listDateTimes.OrderByDescending(x=>x).ToList();
					for(int i=0;i<listDateTimes.Count;i++) {
						comboBoxMonthSelect.Items.Add(listDateTimes[i].ToShortDateString(),listDateTimes[i]);
					}
					if(comboBoxMonthSelect.Items.Count>0) {
						comboBoxMonthSelect.SelectedIndex=0;//Begin with the most recent date.
					}
					EnableComboBoxMonthUI();
					return true;
				}
			}
			if(CreditCards.GetFrequencyType(_creditCard.ChargeFrequency)==ChargeFrequencyType.FixedWeekDay) {
				DayOfWeekFrequency dayOfWeekFrequency=CreditCards.GetDayOfWeekFrequency(_creditCard.ChargeFrequency);
				DayOfWeek dayOfWeek=CreditCards.GetDayOfWeek(_creditCard.ChargeFrequency);
				if(dayOfWeekFrequency.In(DayOfWeekFrequency.Every,DayOfWeekFrequency.EveryOther)) {
					FillComboBoxForWeekDays(dayOfWeek,isEveryOther: dayOfWeekFrequency==DayOfWeekFrequency.EveryOther);
					EnableComboBoxMonthUI();
					return true;
				}
			}
			return false;//Either the frequency is FixedDayOfMonth with 1 day per month, or it is FixedWeekDay with nth day frequency (1st, 2nd, 3rd, etc.)
		}

		///<summary>Returns a valid date based on the Month and Year taken from the date passed in and the Day that is set for the recurring charges.</summary>
		private DateTime GetValidPayDate(DateTime date) {
			int dayOfMonth;
			DateTime datePay=date; 
			if(PrefC.IsODHQ && PrefC.GetBool(PrefName.BillingUseBillingCycleDay)) {
				dayOfMonth=_patient.BillingCycleDay;
				return GetDateForDayOfMonth(date,dayOfMonth);
			}
			if(CreditCards.GetFrequencyType(_creditCard.ChargeFrequency)==ChargeFrequencyType.FixedDayOfMonth) {
				//EX: 1st of Each Month (This check only accounts for a singular day of the month being run)
				List<int> listDaysOfMonth=CreditCards.GetDaysOfMonthForChargeFrequency(_creditCard.ChargeFrequency).Split(',').Select(x => PIn.Int(x))
					.OrderByDescending(x => x).ToList();
				if(listDaysOfMonth.Count==1) {//There is only 1 day being charged in a month
					dayOfMonth=listDaysOfMonth.First();
					//This may result in a future date for the "This Month" button. The button will get disabled later.
					datePay=GetDateForDayOfMonth(date,dayOfMonth);
				}
				else {
					MsgBox.Show(this,"Invalid ChargeFrequency.");
					Close();
					return date;
				}
			}
			if(CreditCards.GetFrequencyType(_creditCard.ChargeFrequency)==ChargeFrequencyType.FixedWeekDay) {
				DayOfWeekFrequency dayOfWeekFrequency=CreditCards.GetDayOfWeekFrequency(_creditCard.ChargeFrequency);
				if(!dayOfWeekFrequency.In(DayOfWeekFrequency.Every,DayOfWeekFrequency.EveryOther)) {//EX: 1st Sunday of Each Month
					datePay=CreditCards.GetNthWeekdayofMonth(date,(int)dayOfWeekFrequency-1,CreditCards.GetDayOfWeek(_creditCard.ChargeFrequency));
					//This may result in a future date for the "This Month" button. The button will get disabled later.
				}
				else {
					MsgBox.Show(this,"Invalid ChargeFrequency.");
					Close();
					return date;
				}
			}
			return datePay; 
		}

		///<summary>Populates comboBoxMonthSelect with options based upon the _creditCardCur.ChargeFrequency's DayOfWeek and the date passed in. 
		///It will create "possible" charge dates for the last 2 months. Pass in isEveryOther only when you are calculating for a 
		///ChargeFrequency of "Every Other" so that it correctly calculates based on 2 week intervals instead of 1.</summary>
		private void FillComboBoxForWeekDays(DayOfWeek dayOfWeek,bool isEveryOther=false) {
			comboBoxMonthSelect.Items.Clear();
			DayOfWeek dayOfWeekNow=DateTime.Now.DayOfWeek;
			int daysIncremented=7;
			if(isEveryOther) {
				daysIncremented=14;
			}
			DateTime dateCharge=DateTime.Today;
			dateCharge=dateCharge.AddDays((int)dayOfWeek-(int)dayOfWeekNow);//Get the most recent day of week 
			if(dayOfWeek > dayOfWeekNow) {//EX: chargeDayOfWeek is Saturday, today is Thursday
				dateCharge=dateCharge.AddDays(-daysIncremented);//since dateCharge is in the future we need to go back a week (or two)
			}
			DateTime dateLimit=dateCharge.AddMonths(-2);
			while(dateLimit<=dateCharge && _creditCard.DateStart<=dateCharge) {//add charge dates for the last two months within CreditCard.StartDate limits
				comboBoxMonthSelect.Items.Add(dateCharge.ToShortDateString(),dateCharge);
				dateCharge=dateCharge.AddDays(-daysIncremented);
			}
			if(comboBoxMonthSelect.Items.Count>0) {
				comboBoxMonthSelect.SelectedIndex=0;//Begin with the most recent date.
			}
		}

		///<summary>Makes visible the UI elements related to comboBoxMonthSelect while hiding UI for This and Last Month. 
		///UI for comboBoxMonthSelect is always hidden by default.</summary>
		private void EnableComboBoxMonthUI() {
			//turn off buttons & labels for This/Last Month
			butLastMonth.Visible=false;
			butThisMonth.Visible=false;
			labelLastMonth.Visible=false;
			labelThisMonth.Visible=false;
			if(comboBoxMonthSelect.Items.Count>0) {
				//turn on combobox & label
				comboBoxMonthSelect.Visible=true;
				labelMonthSelect.Visible=true;
				butOK.Visible=true;
			}
			else {
				labelNoDates.Visible=true;
			}
		}

		///<summary>Gets the DateTime for a specified day in a month, using the year and month of the given date.</summary>
		private DateTime GetDateForDayOfMonth(DateTime date,int dayOfMonth) {
			DateTime dateNew;
			try {
				dateNew=new DateTime(date.Year,date.Month,dayOfMonth);
			}
			catch {//Not a valid date, so use the max day in that month.
				dateNew=new DateTime(date.Year,date.Month,DateTime.DaysInMonth(date.Year,date.Month));
			}
			return dateNew;
		}

		private void butLastMonth_Click(object sender,EventArgs e) {
			DatePay=GetValidPayDate(DateTime.Today.AddMonths(-1));
			DialogResult=DialogResult.OK;
		}

		private void butThisMonth_Click(object sender,EventArgs e) {
			DatePay=GetValidPayDate(DateTime.Today);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DatePay=comboBoxMonthSelect.GetSelected<DateTime>();
			DialogResult=DialogResult.OK;
		}
		
		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}