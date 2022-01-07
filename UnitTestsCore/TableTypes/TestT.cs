using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;

namespace UnitTestsCore {
	/// <summary>
	/// Contains general helper methods for unit tests
	/// </summary>
	public class TestT {
		/// <summary>Defaults to 8 AM of the current day,month,year. Specify the numbers of years/months/days to add and the time to change. </summary>
		public static TimeSpan SetDateTime(int addYears=0,int addMonths=0,double addDays=0,int hour=8,int minute=0,int second=0) {
			if(addYears==0 && DateTime.Now.Month+addMonths>12) {
				addYears++;//Thinking that if it is Nov and want to search 3 months in the future, we should be looking at Feb of next year. Days too?
			}
			return new DateTime(DateTime.Now.AddYears(addYears).Year,DateTime.Now.AddMonths(addMonths).Month,DateTime.Now.AddDays(addDays).Day
				,hour,minute,second).TimeOfDay;
		}

		public static TimeSpan SetDateTime(DateTime date,int hour=8,int minute=0,int second=0) {
			return new DateTime(date.Year,date.Month,date.Day,hour,minute,second).TimeOfDay;
		}

		public static DateTime SetDateT(DateTime date,int hour=8,int minute=0,int second=0) {
			return new DateTime(date.Year,date.Month,date.Day,hour,minute,second);
		}
	}
}
