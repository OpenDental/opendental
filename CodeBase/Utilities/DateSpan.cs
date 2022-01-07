using System;

namespace CodeBase {
	public class DateSpan {
		private int _yearsDiff;
		private int _monthsDiff;
		private int _daysDiff;

		public int YearsDiff {
			get {
				return _yearsDiff;
			}
		}

		public int MonthsDiff {
			get {
				return _monthsDiff;
			}
		}

		public int DaysDiff {
			get {
				return _daysDiff;
			}
		}
		
		///<summary>Pass in the two dates that you want to compare. Results will be stored in YearsDiff, MonthsDiff, and DaysDiff.
		///Always subtracts the smaller date from the larger date to return a positive (or 0) value.</summary>
		public DateSpan(DateTime date1,DateTime date2) {
			DateTime beforeDate;
			DateTime afterDate;
			if(date1<=date2) {
				beforeDate=date1;
				afterDate=date2;
			}
			else {
				beforeDate=date2;
				afterDate=date1;
			}
			//Get the Number of Years Difference between two dates
			GetYears(beforeDate,afterDate);
			//Getting the Number of Months Difference but using the Years difference earlier
			GetMonths(beforeDate.AddYears(YearsDiff),afterDate);
			//Getting the Number of Days Difference but using Years and Months difference earlier
			GetDays(beforeDate.AddYears(YearsDiff).AddMonths(MonthsDiff),afterDate);
		}

		///<summary>Gets the number of years between the two passed-in dates.</summary>
		private void GetYears(DateTime startDate,DateTime endDate) {
			int years=0;
			while(endDate>=startDate.AddYears(years)) {//Calculate the number of years between the two dates.
				years++;
			}
			_yearsDiff=years-1;//Subtract 1 to always round down to the nearest year, since partial years are covered by months and days.
		}

		private void GetMonths(DateTime startDate,DateTime endDate) {
			int months=0;
			while(endDate>=startDate.AddMonths(months)) {//Calculate the number of months between the two dates.
				months++;
			}
			_monthsDiff=months-1;//Subtract 1 to always round down to the nearest month, since partial months are covered by days.
		}

		private void GetDays(DateTime startDate,DateTime endDate) {
			int days=0;
			while(endDate > startDate.AddDays(days)) {//Calculate the number of days between the two dates.
				days++;
			}
			_daysDiff=days;//Do not subtract 1 (always round up) since days are the smallest increment of time used.
		}

	}
}
