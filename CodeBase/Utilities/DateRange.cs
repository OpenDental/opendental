using System;

namespace CodeBase {
	///<summary>An object that represents a range of dates.</summary>
	public class DateRange {
		///<summary>The beginning of the date range.</summary>
		public DateTime Start;
		///<summary>The end of the date range.</summary>
		public DateTime End;

		public DateRange() {
		}

		public DateRange(DateTime dateRangeStart,DateTime dateRangeEnd) {
			Start=dateRangeStart;
			End=dateRangeEnd;
		}

		public bool IsInRange(DateTime dateTime) {
			return dateTime.Between(Start,End);
		}
	}
}
