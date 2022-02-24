/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General public License as published by
    the Free Software Foundation; either version 2.1 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General public License for more details.

    You should have received a copy of the GNU Lesser General public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;


using fyiReporting.RDL;


namespace fyiReporting.RDL
{
	/// <summary>
	/// The VBFunctions class holds a number of static functions for support VB functions.
	/// </summary>
	sealed public class VBFunctions
	{
		/// <summary>
		/// Obtains the year
		/// </summary>
		/// <param name="dt"></param>
		/// <returns>int year</returns>
		static public int Year(DateTime dt)
		{
			return dt.Year;
		}
		/// <summary>
		/// Returns the integer day of week: 1=Sunday, 2=Monday, ..., 7=Saturday
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public int Weekday(DateTime dt)
		{
			int dow;
			switch (dt.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					dow=1;
					break;
				case DayOfWeek.Monday:
					dow=2;
					break;
				case DayOfWeek.Tuesday:
					dow=3;
					break;
				case DayOfWeek.Wednesday:
					dow=4;
					break;
				case DayOfWeek.Thursday:
					dow=5;
					break;
				case DayOfWeek.Friday:
					dow=6;
					break;
				case DayOfWeek.Saturday:
					dow=7;
					break;
				default:			// should never happen
					dow=1;
					break;
			}
			return dow;
		}

		/// <summary>
		/// Returns the name of the day of week
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		static public string WeekdayName(int d)
		{
			return WeekdayName(d, false);
		}

		/// <summary>
		/// Returns the name of the day of week
		/// </summary>
		/// <param name="d"></param>
		/// <param name="bAbbreviation">true for abbreviated name</param>
		/// <returns></returns>
		static public string WeekdayName(int d, bool bAbbreviation)
		{
			DateTime dt = new DateTime(2005, 5, d);		// May 1, 2005 is a Sunday
			string wdn = bAbbreviation? string.Format("{0:ddd}", dt):string.Format("{0:dddd}", dt);
			return wdn;
		}
		/// <summary>
		/// Get the day of the month.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public int Day(DateTime dt)
		{
			return dt.Day;
		}
		
		/// <summary>
		/// Gets the integer month
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public int Month(DateTime dt)
		{
            return dt.Month;
		}
		
		/// <summary>
		/// Get the month name
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		static public string MonthName(int m)
		{
			return MonthName(m, false);
		}

		/// <summary>
		/// Gets the month name; optionally abbreviated
		/// </summary>
		/// <param name="m"></param>
		/// <param name="bAbbreviation"></param>
		/// <returns></returns>
		static public string MonthName(int m, bool bAbbreviation)
		{
			DateTime dt = new DateTime(2005, m, 1);

			string mdn = bAbbreviation? string.Format("{0:MMM}", dt):string.Format("{0:MMMM}", dt);
			return mdn;
		}

		/// <summary>
		/// Gets the hour
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public int Hour(DateTime dt)
		{
			return dt.Hour;
		}
		/// <summary>
		/// Get the minute
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public int Minute(DateTime dt)
		{
			return dt.Minute;
		}

		/// <summary>
		/// Get the second
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		static public int Second(DateTime dt)
		{
			return dt.Second;
		}

		/// <summary>
		/// Gets the current local date/time on this computer
		/// </summary>
		/// <returns></returns>
		static public DateTime Today()
		{
			return DateTime.Now;
		}
		/// <summary>
		/// Converts the first letter in a string to ANSI code 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public int Asc(string o)
		{
			if (o == null || o.Length == 0)
				return 0;

			return Convert.ToInt32(o[0]);
		}
		/// <summary>
		/// Converts an expression to Boolean  
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public bool CBool(object o)
		{
			return Convert.ToBoolean(o);
		}
		/// <summary>
		/// Converts an expression to type Byte
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public Byte CByte(string o)
		{
			return Convert.ToByte(o);
		}
		/// <summary>
		/// Converts an expression to type Currency - really Decimal
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public decimal CCur(string o)
		{
			return Convert.ToDecimal(o);
		}
		/// <summary>
		/// Converts an expression to type Date
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public DateTime CDate(string o)
		{
			return Convert.ToDateTime(o);
		}
		/// <summary>
		/// Converts the specified ANSI code to a character
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public char Chr(int o)
		{
			return Convert.ToChar(o);
		}
		/// <summary>
		/// Converts the expression to integer
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public int CInt(object o)
		{
			return Convert.ToInt32(o);
		}
		/// <summary>
		/// Converts the expression to long
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public long CLng(object o)
		{
			return Convert.ToInt64(o);
		}
		/// <summary>
		/// Converts the expression to Single
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public Single CSng(object o)
		{
			return Convert.ToSingle(o);
		}
		/// <summary>
		/// Converts the expression to String
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public string CStr(object o)
		{
			return Convert.ToString(o);
		}
		/// <summary>
		/// Returns the hexadecimal value of a specified number
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public string Hex(long o)
		{
			return Convert.ToString(o, 16);
		}
		/// <summary>
		/// Returns the octal value of a specified number
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public string Oct(long o)
		{
			return Convert.ToString(o, 8);
		}

		/// <summary>
		/// Converts the passed parameter to double
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		static public double CDbl(Object o)
		{
			return Convert.ToDouble(o);
		}

		/// <summary>
		/// 1 based offset of string2 in string1
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <returns></returns>
		static public int InStr(string string1, string string2)
		{
			return InStr(1, string1, string2, 0);
		}
		/// <summary>
		/// 1 based offset of string2 in string1
		/// </summary>
		/// <param name="start"></param>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <returns></returns>
		static public int InStr(int start, string string1, string string2)
		{
			return InStr(start, string1, string2, 0);
		}
		/// <summary>
		/// 1 based offset of string2 in string1; optionally case insensitive
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <param name="compare">1 if you want case insensitive compare</param>
		/// <returns></returns>
		static public int InStr(string string1, string string2, int compare)
		{
			return InStr(1, string1, string2, compare);
		}
		/// <summary>
		/// 1 based offset of string2 in string1; optionally case insensitive
		/// </summary>
		/// <param name="start"></param>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <param name="compare"></param>
		/// <returns></returns>
		static public int InStr(int start, string string1, string string2, int compare)
		{
			if (string1 == null || string2 == null || 
				string1.Length == 0 || start > string1.Length ||
				start < 1)
				return 0;
			if (string2.Length == 0)
				return start;

			// Make start zero based
			start--;
			if (start < 0)
				start=0;

			if (compare == 1)	// Make case insensitive comparison?
			{	// yes; just make both strings lower case
				string1 = string1.ToLower();
				string2 = string2.ToLower();
			}

			int i = string1.IndexOf(string2, start);
			return i+1;			// result is 1 based
		}
		/// <summary>
		/// 1 based offset of string2 in string1 starting from end of string
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <returns></returns>
		static public int InStrRev(string string1, string string2)
		{
			return InStrRev(string1, string2, -1, 0);
		}
		/// <summary>
		/// 1 based offset of string2 in string1 starting from end of string - start
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		static public int InStrRev(string string1, string string2, int start)
		{
			return InStrRev(string1, string2, start, 0);
		}
		/// <summary>
		/// 1 based offset of string2 in string1 starting from end of string - start optionally case insensitive
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <param name="start"></param>
		/// <param name="compare">1 for case insensitive comparison</param>
		/// <returns></returns>
		static public int InStrRev(string string1, string string2, int start, int compare)
		{
			if (string1 == null || string2 == null || 
				string1.Length == 0 || string2.Length > string1.Length)
				return 0;

			// TODO this is the brute force method of searching; should use better algorithm
			bool bCaseSensitive = compare == 1;
			int inc= start == -1? string1.Length: start;
			if (inc > string1.Length)
				inc = string1.Length;
			while (inc >= string2.Length)	// go thru the string backwards; but string still needs to long enough to hold find string
			{
				int i=string2.Length-1;
				for ( ; i >= 0; i--)	// match the find string backwards as well
				{
					if (bCaseSensitive)
					{		
						if (Char.ToLower(string1[inc-string2.Length+i]) != string2[i])
							break;
					}
					else
					{
						if (string1[inc-string2.Length+i] != string2[i])
							break;
					}
				}
				if (i < 0)		// We got a match
					return inc+1-string2.Length;
				inc--;					// No match try next character
			}
			return 0;
		}
/// <summary>
/// Returns the lower case of the passed string
/// </summary>
/// <param name="str"></param>
/// <returns></returns>
		static public string LCase(string str)
		{
			return str == null? null: str.ToLower();
		}

		/// <summary>
		/// Returns the left n characters from the string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		static public string Left(string str, int count)
		{
			if (str == null || count >= str.Length)
				return str;
			else
				return str.Substring(0, count);
		}

		/// <summary>
		/// Returns the length of the string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static public int Len(string str)
		{
			return str == null? 0: str.Length;
		}

		/// <summary>
		/// Removes leading blanks from string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static public string LTrim(string str)
		{
			if (str == null || str.Length == 0)
				return str;

			return str.TrimStart(' ');
		}

		/// <summary>
		/// Returns the portion of the string denoted by the start and length.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="start">1 based starting position</param>
		/// <param name="length">length to extract</param>
		/// <returns></returns>
		static public string Mid(string str, int start, int length)
		{
			if (str == null)
				return null;

			if (start > str.Length)
				return "";

			return str.Substring(start-1, length);
		}
		//Replace(string,find,replacewith[,start[,count[,compare]]])
		/// <summary>
		/// Returns string replacing all instances of the searched for text with the replace text.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="find"></param>
		/// <param name="replacewith"></param>
		/// <returns></returns>
		static public string Replace(string str, string find, string replacewith)
		{
			return Replace(str, find, replacewith, 1, -1, 0);
		}
		/// <summary>
		/// Returns string replacing all instances of the searched for text starting at position 
		/// start with the replace text.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="find"></param>
		/// <param name="replacewith"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		static public string Replace(string str, string find, string replacewith, int start)
		{
			return Replace(str, find, replacewith, start, -1, 0);
		}
		/// <summary>
		/// Returns string replacing 'count' instances of the searched for text starting at position 
		/// start with the replace text.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="find"></param>
		/// <param name="replacewith"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		static public string Replace(string str, string find, string replacewith, int start, int count)
		{
			return Replace(str, find, replacewith, start, count, 0);
		}
		/// <summary>
		/// Returns string replacing 'count' instances of the searched for text (optionally
		/// case insensitive) starting at position start with the replace text.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="find"></param>
		/// <param name="replacewith"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <param name="compare">1 for case insensitive search</param>
		/// <returns></returns>
		static public string Replace(string str, string find, string replacewith, int start, int count, int compare)
		{
			if (str == null || find == null || find.Length == 0 || count == 0)
				return str;

			if (count == -1)				// user want all changed?
				count = int.MaxValue;

			StringBuilder sb = new StringBuilder(str);

			bool bCaseSensitive = compare != 0;		// determine if case sensitive; compare = 0 for case sensitive
			if (bCaseSensitive)
				find = find.ToLower();
			int inc=0;
			bool bReplace = (replacewith != null && replacewith.Length > 0);
			// TODO this is the brute force method of searching; should use better algorithm
			while (inc <= sb.Length - find.Length)
			{
				int i=0;
				for ( ; i < find.Length; i++)
				{
					if (bCaseSensitive)
					{		
						if (Char.ToLower(sb[inc+i]) != find[i])
							break;
					}
					else
					{
						if (sb[inc+i] != find[i])
							break;
					}
				}
				if (i == find.Length)		// We got a match
				{
					// replace the found string with the replacement string
					sb.Remove(inc, find.Length);
					if (bReplace)
					{
						sb.Insert(inc, replacewith);
						inc += (replacewith.Length + 1);
					}
					count--;
					if (count == 0)			// have we done as many replaces as requested?
						return sb.ToString();	// yes, return
				}
				else
					inc++;					// No match try next character
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns the rightmost length of string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		static public string Right(string str, int length)
		{
			if (str == null || str.Length <= length)
				return str;

			if (length <= 0)
				return "";

			return str.Substring(str.Length - length);
		}
		/// <summary>
		/// Removes trailing blanks from string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static public string RTrim(string str)
		{
			if (str == null || str.Length == 0)
				return str;

			return str.TrimEnd(' ');
		}
		/// <summary>
		/// Returns blank string of the specified length
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		static public string Space(int length)
		{
			return String(length, ' ');
		}

		//StrComp(string1,string2[,compare])
		/// <summary>
		/// Compares the strings. When string1 &lt; string2: -1, string1 = string2: 0, string1 > string2: 1 
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <returns></returns>
		static public int StrComp(string string1, string string2)
		{
			return StrComp(string1, string2, 0);
		}
		/// <summary>
		/// Compares the strings; optionally with case insensitivity. When string1 &lt; string2: -1, string1 = string2: 0, string1 > string2: 1 
		/// </summary>
		/// <param name="string1"></param>
		/// <param name="string2"></param>
		/// <param name="compare">1 for case insensitive comparison</param>
		/// <returns></returns>
		static public int StrComp(string string1, string string2, int compare)
		{
			if (string1 == null || string2 == null)
				return 0;			// not technically correct; should return null

			return compare == 0? 
				string1.CompareTo(string2):
				string1.ToLower().CompareTo(string2.ToLower());
		}

		/// <summary>
		/// Return string with the character repeated for the length
		/// </summary>
		/// <param name="length"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		static public string String(int length, char c)
		{
			if (length <= 0)
				return "";

			StringBuilder sb = new StringBuilder(length, length);
			for (int i = 0; i < length; i++)
				sb.Append(c);
			return sb.ToString();
		}

		/// <summary>
		/// Returns a string with the characters reversed.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static public string StrReverse(string str)
		{
			if (str == null || str.Length < 2)
				return str;

			StringBuilder sb = new StringBuilder(str, str.Length);
			int i = str.Length-1;
			foreach (char c in str)
			{
				sb[i--] = c;
			}
			return sb.ToString();
		}

		/// <summary>
		/// Removes whitespace from beginning and end of string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static public string Trim(string str)
		{
			if (str == null || str.Length == 0)
				return str;

			return str.Trim(' ');
		}
		/// <summary>
		/// Returns the uppercase version of the string 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		static public string UCase(string str)
		{
			return str == null? null: str.ToUpper();
		}
	}
}
