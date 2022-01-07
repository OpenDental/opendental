using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeBase {
	public class DateTimeOD {
		
		///<summary>Returns the most recent valid date possible based on the year and month passed in.
		///E.g. y:2017,m:4,d:31 is passed in (an invalid date) which will return a date of "04/30/2017" which is the most recent 'valid' date.
		///Throws an exception if the year is not between 1 and 9999, and if the month is not between 1 and 12.</summary>
		public static DateTime GetMostRecentValidDate(int year,int month,int day) {
			int maxDay=DateTime.DaysInMonth(year,month);
			return new DateTime(year,month,Math.Min(day,maxDay));
		}

		///<summary>Sets the short date format to a standardized format so all dates are interpreted with a four digit year format.  All threads in the application will use this format regardless of the computer's region settings.  Throws an exception if the format could not be normalized.</summary>
		public static void NormalizeApplicationShortDateFormat() {
			CultureInfo cInfo=(CultureInfo)CultureInfo.CurrentCulture.Clone();
			if(cInfo.Name=="en-US") {
				//for the business layer, this functionality is duplicated in the Lan class.  Need for SilverLight.
				cInfo.DateTimeFormat.ShortDatePattern="MM/dd/yyyy";
			}
			else {//not en-US
				string dateFormatCur=cInfo.DateTimeFormat.ShortDatePattern;
				//The carrot indicates the beginning of a word.  {2} means that there has to be exactly 2 y's.  [^y] means any character except y.
				//$ means end of word.
				if(Regex.IsMatch(dateFormatCur,"^y{2}[^y]")//Starts with 2 'y' chars followed by a non 'y' char; looking for "yy/"
					|| Regex.IsMatch(dateFormatCur,"[^y]y{2}[^y]")//Starts with a non 'y' char followed by two 'y' chars followed by a non 'y' char; looking for "/yy/"
					|| Regex.IsMatch(dateFormatCur,"[^y]y{2}$")) //Starts with a non 'y' char followed by two 'y' chars at the end; looking for ending with "/yy"
				{
					//We know there are only two y's in the format.  Force there to be four.
					cInfo.DateTimeFormat.ShortDatePattern=cInfo.DateTimeFormat.ShortDatePattern.Replace("yy","yyyy");
				}
			}
			//Verify that the date time format has been normalized (in case a user has a custom format that we can't decipher).
			if(!cInfo.DateTimeFormat.ShortDatePattern.Contains("yyyy")) {
				//Go ahead and throw an exception instead of forcing a "default" format on the user.
				//This is mainly due to the fact that some formats are expecting days to show before months (or visa versa) and a forced pattern could cause headaches.
				throw new ApplicationException("Error normalizing date format.");//Defend against a failed regex on the date format.
			}
			Application.CurrentCulture=cInfo;
			//"In the .NET Framework 4 and previous versions, by default, the culture of all threads is set to the Windows system culture.  In the .NET
			//Framework 4.5, the DefaultThreadCurrentCulture property enables an application to define the default culture of all threads in an
			//application domain."  https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.defaultthreadcurrentculture(v=vs.110).aspx
			//If we don't set this, dates are formated as M/d/yyyy, e.g. instead of 03/01/2017 you get 3/1/2017.  This causes issues since we sometimes
			//use date strings as part of key data when setting and validating signatures.  The key data inside of a thread will not match the key data
			//outside of a thread.  By defining the DefaultThreadCurrentCulture equal to Application.CurrentCulture, we dictate that, unless explicitly
			//overwritten when a thread is instantiated, all threads spawned by OD will default to the application's current culture not the system culture.
			CultureInfo.DefaultThreadCurrentCulture=Application.CurrentCulture;
			CultureInfo.DefaultThreadCurrentUICulture=Application.CurrentCulture;
		}

		///<summary>Returns true if the inputDate is between dateStart and dateEnd.</summary>
		public static bool Between(DateTime inputDate,DateTime dateStart,DateTime dateEnd) {
			return (inputDate > dateStart && inputDate < dateEnd);
		}

		///<summary>Returns true if the inputDate is between dateStart and dateEnd. Includes the dateStart and dateEnd</summary>
		public static bool BetweenInclusive(DateTime inputDate,DateTime dateStart,DateTime dateEnd) {
			return (inputDate >= dateStart && inputDate <= dateEnd);
		}

		///<summary>Finds the greatest date which is considered to be the given number of months in the past.  Does not consider the time, just the date.
		///Ex: March 31st minus 1 month is Feb. 28th or 29th, but the result we want is March 1st.</summary>
		public static DateTime CalculateForEndOfMonthOffset(DateTime date,int numMonthsInPast) {
			DateTime dateCalc=date.AddMonths(0-numMonthsInPast);
			while(dateCalc.AddMonths(numMonthsInPast) < date) {
				dateCalc=dateCalc.AddDays(1);
			}
			return dateCalc;
		}
	}
}
