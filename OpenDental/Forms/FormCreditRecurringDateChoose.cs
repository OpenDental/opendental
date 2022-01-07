using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCreditRecurringDateChoose:FormODBase {
		private CreditCard _creditCardCur;
		public DateTime PayDate;
		private Patient _pat;

		public FormCreditRecurringDateChoose(CreditCard creditCard,Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			_creditCardCur=creditCard;
			_pat=pat;
			Lan.F(this);
		}

		private void FormCreditRecurringDateChoose_Load(object sender,EventArgs e) {
			try {
				if(FillComboBoxMonthSelect()) {
					return;//We are using the comboBox instead of two buttons.
				}
				//We are using the This Month/Last Month buttons instead of the comboBox.
				DateTime thisMonth=GetValidPayDate(DateTime.Today);
				DateTime lastMonth=GetValidPayDate(DateTime.Today.AddMonths(-1));
				if(PrefC.GetBool(PrefName.RecurringChargesUseTransDate)) {
					//Labels set here, buttons set in respective button methods butLastMonth_Click/butThisMonth_Click
					labelLastMonth.Text=Lan.g(this,"Recurring charge date will be:")+" "+lastMonth.ToShortDateString();
					labelThisMonth.Text=Lan.g(this,"Recurring charge date will be:")+" "+thisMonth.ToShortDateString();
				}
				else {
					labelLastMonth.Text+=" "+lastMonth.ToShortDateString();
					labelThisMonth.Text+=" "+thisMonth.ToShortDateString();
				}
				//If the recurring pay date is in the future do not let them choose that option.
				if(thisMonth>DateTime.Now) {
					butThisMonth.Enabled=false;
					labelThisMonth.Text=Lan.g(this,"Cannot make payment for future date:")+" "+thisMonth.ToShortDateString();
				}
				if(lastMonth<_creditCardCur.DateStart) {
					labelLastMonth.Text=Lan.g(this,"Cannot make payment before start date:")+" "+ _creditCardCur.DateStart;
					butLastMonth.Enabled=false;
				}
				if(thisMonth<_creditCardCur.DateStart) {
					labelThisMonth.Text=Lan.g(this,"Cannot make payment before start date:")+" "+ _creditCardCur.DateStart;
					butThisMonth.Enabled=false;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				Close();
			}
		}

		///<summary>Fills comboBoxMonthSelect when appropriate, otherwise butThisMonth and butLastMonth are used.  Returns true if comboBoxMonthSelect
		///will be used instead of butThisMonth/butLastMonth.</summary>
		private bool FillComboBoxMonthSelect() {
			switch(CreditCards.GetFrequencyType(_creditCardCur.ChargeFrequency)) {
				case ChargeFrequencyType.FixedDayOfMonth://EX: 2nd and 15th of each Month, instance of a singular day being charged is in GetValidPayDate(...)
					List<int> listDaysOfMonth=CreditCards.GetDaysOfMonthForChargeFrequency(_creditCardCur.ChargeFrequency).Split(',').Select(x => PIn.Int(x))
						.OrderByDescending(x => x).ToList();
					if(listDaysOfMonth.Count>1) {
						comboBoxMonthSelect.Items.Clear();
						List<DateTime> listDates=new List<DateTime>();
						listDaysOfMonth.ForEach(x => {
							int monthOffset=(x>DateTime.Today.Day) ? -1 : 0;
							DateTime thisMonth=GetDateForDayOfMonth(DateTime.Today.AddMonths(monthOffset),x); //This Month/Closest Month
							DateTime lastMonth=GetDateForDayOfMonth(DateTime.Today.AddMonths(monthOffset-1),x); //Last Month/Previous Month
							if(thisMonth>=_creditCardCur.DateStart){ 
								listDates.Add(thisMonth); 
							}
							if(lastMonth>=_creditCardCur.DateStart) {
								listDates.Add(lastMonth);
							}
						});
						listDates.OrderByDescending(x => x).ForEach(x => comboBoxMonthSelect.Items.Add(x.ToShortDateString(),x));
						if(comboBoxMonthSelect.Items.Count>0) {
							comboBoxMonthSelect.SelectedIndex=0;//Begin with the most recent date.
						}
						EnableComboBoxMonthUI();
						return true;
					}
					break;
				case ChargeFrequencyType.FixedWeekDay:
					DayOfWeekFrequency frequency=CreditCards.GetDayOfWeekFrequency(_creditCardCur.ChargeFrequency);
					DayOfWeek ccDayOfWeek=CreditCards.GetDayOfWeek(_creditCardCur.ChargeFrequency);
					if(ListTools.In(frequency,DayOfWeekFrequency.Every,DayOfWeekFrequency.EveryOther)) {
						FillComboBoxForWeekDays(ccDayOfWeek,isEveryOther: frequency==DayOfWeekFrequency.EveryOther);
						EnableComboBoxMonthUI();
						return true;
					}
					break;
				default:
					throw new ODException(Lan.g(this,"Invalid ChargeFrequency."));
			}
			return false;//Either the frequency is FixedDayOfMonth with 1 day per month, or it is FixedWeekDay with nth day frequency (1st, 2nd, 3rd, etc.)
		}

		///<summary>Returns a valid date based on the Month and Year taken from the date passed in and the Day that is set for the recurring charges.</summary>
		private DateTime GetValidPayDate(DateTime date) {
			int dayOfMonth;
			DateTime retVal;
			if(PrefC.IsODHQ && PrefC.GetBool(PrefName.BillingUseBillingCycleDay)) {
				dayOfMonth=_pat.BillingCycleDay;
				return GetDateForDayOfMonth(date,dayOfMonth);
			}
			switch(CreditCards.GetFrequencyType(_creditCardCur.ChargeFrequency)) {
				case ChargeFrequencyType.FixedDayOfMonth://EX: 1st of Each Month (This check only accounts for a singular day of the month being run)
					List<int> listDaysOfMonth=CreditCards.GetDaysOfMonthForChargeFrequency(_creditCardCur.ChargeFrequency).Split(',').Select(x => PIn.Int(x))
						.OrderByDescending(x => x).ToList();
					if(listDaysOfMonth.Count==1) {//There is only 1 day being charged in a month
						dayOfMonth=listDaysOfMonth.First();
						//This may result in a future date for the "This Month" button. The button will get disabled later.
						retVal=GetDateForDayOfMonth(date,dayOfMonth);
					}
					else {
						throw new ODException(Lan.g(this,"Invalid ChargeFrequency."));
					}
					break;
				case ChargeFrequencyType.FixedWeekDay:
					DayOfWeekFrequency frequency=CreditCards.GetDayOfWeekFrequency(_creditCardCur.ChargeFrequency);
					if(!ListTools.In(frequency,DayOfWeekFrequency.Every,DayOfWeekFrequency.EveryOther)) {//EX: 1st Sunday of Each Month
						retVal=CreditCards.GetNthWeekdayofMonth(date,(int)frequency-1,CreditCards.GetDayOfWeek(_creditCardCur.ChargeFrequency));
						//This may result in a future date for the "This Month" button. The button will get disabled later.
					}
					else {
						throw new ODException(Lan.g(this,"Invalid ChargeFrequency."));
					}
					break;
				default:
					throw new ODException(Lan.g(this,"Invalid ChargeFrequency."));
			}
			return retVal;
		}

		///<summary>Populates comboBoxMonthSelect with options based upon the _creditCardCur.ChargeFrequency's DayOfWeek and the date passed in. 
		///It will create "possible" charge dates for the last 2 months. Pass in isEveryOther only when you are calculating for a 
		///ChargeFrequency of "Every Other" so that it correctly calculates based on 2 week intervals instead of 1.</summary>
		private void FillComboBoxForWeekDays(DayOfWeek chargeDayOfWeek,bool isEveryOther=false) {
			comboBoxMonthSelect.Items.Clear();
			DayOfWeek curDayOfWeek=DateTime.Now.DayOfWeek;
			int daysIncremented=(isEveryOther) ? 14 : 7;
			DateTime dateCharge=DateTime.Today;
			dateCharge=dateCharge.AddDays((int)chargeDayOfWeek-(int)curDayOfWeek);//Get the most recent day of week 
			if(chargeDayOfWeek > curDayOfWeek) {//EX: chargeDayOfWeek is Saturday, today is Thursday
				dateCharge=dateCharge.AddDays(-daysIncremented);//since dateCharge is in the future we need to go back a week (or two)
			}
			DateTime dateLimit=dateCharge.AddMonths(-2);
			while(dateLimit<=dateCharge && _creditCardCur.DateStart<=dateCharge) {//add charge dates for the last two months within CreditCard.StartDate limits
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
			DateTime newDate;
			try {
				newDate=new DateTime(date.Year,date.Month,dayOfMonth);
			}
			catch {//Not a valid date, so use the max day in that month.
				newDate=new DateTime(date.Year,date.Month,DateTime.DaysInMonth(date.Year,date.Month));
			}
			return newDate;
		}

		private void butLastMonth_Click(object sender,EventArgs e) {
			PayDate=GetValidPayDate(DateTime.Today.AddMonths(-1));
			DialogResult=DialogResult.OK;
		}

		private void butThisMonth_Click(object sender,EventArgs e) {
			PayDate=GetValidPayDate(DateTime.Today);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			PayDate=comboBoxMonthSelect.GetSelected<DateTime>();
			DialogResult=DialogResult.OK;
		}
		
		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}