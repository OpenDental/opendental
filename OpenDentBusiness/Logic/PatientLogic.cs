using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OpenDentBusiness{
	public class PatientLogic {
		///<summary>Returns a formatted name, Last, First.</summary>
		public static string GetNameLF(string lName,string fName, string preferred,string middleI) {
			if(lName==""){
				return "";
			}
			if(preferred=="") {
				return lName+", "+fName+" "+middleI;
			}
			else {
				return lName+", '"+preferred+"' "+fName+" "+middleI;
			}
		}

		///<summary>Converts a date to an age.  Blank if over 115.  Only used where it's important to show the month, too.  Month will only show if less than 18yo.</summary>
		public static string DateToAgeString(DateTime dateBirth) {
			return DateToAgeString(dateBirth,DateTime.Now);
		}

		///<summary>Converts a date to an age.  Blank if over 115.  Only used where it's important to show the month, too.  Month will only show if less than 18yo.
		///Use dateTo=DateTime.Now for current age.</summary>
		public static string DateToAgeString(DateTime dateBirth,DateTime date) {
			if(dateBirth.Year<1880) {
				return "";
			}
			if(date.Year<1880) {
				date=DateTime.Now;
			}
			if(date<dateBirth) {
				return "";
			}
			int years=0;
			int months=0;
			if(dateBirth.Month < date.Month) {//birthday was recently in a previous month
				years=date.Year-dateBirth.Year;
				if(dateBirth.Day < date.Day) {//birthday earlier in the month
					months=(date.Month-dateBirth.Month);
				}
				else if(dateBirth.Day==date.Day) {//birthday day of month same as today
					months=(date.Month-dateBirth.Month);
				}
				else {//day of month later than today
					months=(date.Month-dateBirth.Month)-1;
				}
			}
			else if(dateBirth.Month == date.Month) {//birthday this month
				if(dateBirth.Day < date.Day) {//birthday earlier in this month
					years=date.Year-dateBirth.Year;
					months=0;
				}
				else if(dateBirth.Month == date.Month && dateBirth.Day == date.Day) {//today
					years=date.Year-dateBirth.Year;
					months=0;
				}
				else {//later this month
					years=date.Year-dateBirth.Year-1;
					months=11;
				}
			}
			else {//Hasn't had birthday yet this year.  It will be in a future month.
				years=date.Year-dateBirth.Year-1;
				if(dateBirth.Day < date.Day) {//birthday earlier in the month
					months=12-(dateBirth.Month-date.Month);
				}
				else if(dateBirth.Day==date.Day) {//birthday day of month same as today
					months=12-(dateBirth.Month-date.Month);
				}
				else {//day of month later than today
					months=12-(dateBirth.Month-date.Month)-1;
				}
			}
			if(years<18) {
				return years.ToString()+"y "+months.ToString()+"m";
			}
			else {
				return years.ToString();
			}
			//return AgeToString(DateToAge(date));
		}
	}
}
