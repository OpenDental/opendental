using System;
using System.Collections;
using System.Collections.Specialized;

namespace OpenDentBusiness{
	///<summary>Currently used in recall interval. Uses all four values together to establish an interval between two dates, letting the user have total control.  Will later be used for such things as lab cases, appointment scheduling, etc.  Includes a way to combine all four values into one number to be stored in the database (as an int32).  Each value has a max of 255, except years has a max of 127.</summary>
	public struct Interval {
		///<summary></summary>
		public int Years;
		///<summary></summary>
		public int Months;
		///<summary></summary>
		public int Weeks;
		///<summary></summary>
		public int Days;

		///<summary></summary>
		public Interval(int combinedValue) {
			BitVector32 bitVector=new BitVector32(combinedValue);
			BitVector32.Section sectionDays=BitVector32.CreateSection(255);
			BitVector32.Section sectionWeeks=BitVector32.CreateSection(255,sectionDays);
			BitVector32.Section sectionMonths=BitVector32.CreateSection(255,sectionWeeks);
			BitVector32.Section sectionYears=BitVector32.CreateSection(255,sectionMonths);
			Days=bitVector[sectionDays];
			Weeks=bitVector[sectionWeeks];
			Months=bitVector[sectionMonths];
			Years=bitVector[sectionYears];
		}

		///<summary></summary>
		public Interval(int days,int weeks,int months,int years) {
			Days=days;
			Weeks=weeks;
			Months=months;
			Years=years;
		}

		///<summary>Define the == operator.</summary>
		public static bool operator==(Interval a,Interval b) {
			if(a.Years==b.Years
				&& a.Months==b.Months
				&& a.Weeks==b.Weeks
				&& a.Days==b.Days) {
				return true;
			}
			return false;
		}

		///<summary>Define the != operator.</summary>
		public static bool operator!=(Interval a,Interval b) {
			if(a.Years==b.Years
				&& a.Months==b.Months
				&& a.Weeks==b.Weeks
				&& a.Days==b.Days) {
				return false;
			}
			return true;
		}

		///<summary>Required to override Equals since we defined == and !=</summary>
		public override bool Equals(Object o) {
			try {
				return (bool)(this==(Interval)o);
			}
			catch {
				return false;
			}
		}

		///<summary>Required to override since we defined == and !=</summary>
		public override int GetHashCode() {
			return ToInt();
		}

		/// <summary>Specify a date and an interval to return a new date based on adding the interval to the original date.</summary>
		public static DateTime operator+(DateTime date,Interval interval) {
			DateTime retVal=date
				.AddYears(interval.Years)
				.AddMonths(interval.Months)
				.AddDays(interval.Weeks*7)
				.AddDays(interval.Days);
			return retVal;
		}

		///<summary></summary>
		public int ToInt() {
			BitVector32 bitVector=new BitVector32(0);
			BitVector32.Section sectionDays=BitVector32.CreateSection(255);
			BitVector32.Section sectionWeeks=BitVector32.CreateSection(255,sectionDays);
			BitVector32.Section sectionMonths=BitVector32.CreateSection(255,sectionWeeks);
			BitVector32.Section sectionYears=BitVector32.CreateSection(255,sectionMonths);
			bitVector[sectionDays]=Days;
			bitVector[sectionWeeks]=Weeks;
			bitVector[sectionMonths]=Months;
			bitVector[sectionYears]=Years;
			return bitVector.Data;
		}

		///<summary>Example: 1y3m1w1d</summary>
		public override string ToString() {
			string retVal="";
			if(Years>0) {
				retVal+=Years.ToString()+"y";
			}
			if(Months>0) {
				retVal+=Months.ToString()+"m";
			}
			if(Weeks>0) {
				retVal+=Weeks.ToString()+"w";
			}
			if(Days>0) {
				retVal+=Days.ToString()+"d";
			}
			return retVal;
		}

	}

	


}









