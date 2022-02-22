using System;

namespace CodeBase {
	///<summary>This struct can be used when you want an optional parameter that defaults to DateTime.Today if the parameter is not included.</summary>
	public struct DateDefaultToday {
		private bool _isInitialized;
		private DateTime _dateTime;

		public DateDefaultToday(DateTime dateTime) {
			_dateTime=dateTime;
			_isInitialized=true;
		}

		///<summary>The DateTime stored. Will be DateTime.Today if it has not been set.</summary>
		public DateTime Date {
			get {
				if(!_isInitialized) {
					return DateTime_.Today;
				}
				return _dateTime;
			}
			set {
				_dateTime=value;
				_isInitialized=true;
			}
		}

		///<summary>This operator allows you to pass in a DateTime to a method that accepts a DateDefaultToday.</summary>
		public static implicit operator DateDefaultToday(DateTime dateTime) {
			return new DateDefaultToday(dateTime);
		}
	}
}
