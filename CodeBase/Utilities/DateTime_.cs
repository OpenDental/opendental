﻿using System;

namespace CodeBase {
	///<summary>This class is useful when testing in order to mock DateTime.Now. In the real code, substitute DateTime.Now with DateTime_.Now, and
	///in the tests, override DateTime_GetNow to change the current time.</summary>
	public static class DateTime_ {
		///<summary>Set this func to return a custom value for DateTime_.Now. If not overridden, will return the real DateTime.Now.</summary>
		private static Func<DateTime> _getNow=() => DateTime.Now;
		///<summary>Unit tests may need to offset time by milliseconds. Usually used to avoid identical timestamps from being entered in db.</summary>
		private static int _offsetMs=0;
		///<summary>True if DateTime_.Now will return a custom value.</summary>
		public static bool IsNowModified {
			get; private set;
		}
		///<summary>The Now time based on DateTime_.GetNow.</summary>
		public static DateTime Now => _getNow();
		///<summary>The Today date based on DateTime_.GetNow.</summary>
		public static DateTime Today => _getNow().Date;

		///<summary>Call this method to return a custom value for DateTime_.Now. If not called, will return the real DateTime.Now.</summary>
		public static void SetNow(Func<DateTime> getNow) {
			if(!ODBuild.IsDebug()) {
				throw new Exception("Not allowed to change DateTime_.Now in release.");
			}
			IsNowModified=true;
			_getNow=getNow;
		}

		///<summary>Call this method to make DateTime_.Now return the normal DateTime.Now.</summary>
		public static void ResetNow() {
			IsNowModified=false;
			_getNow=() => DateTime.Now;
		}

		///<summary>Call this method to make Offset() add milliseconds.</summary>
		public static void SetOffset(int offsetMs) {
			_offsetMs=offsetMs;
		}

		///<summary>Returns current offset milliseconds.</summary>
		public static int GetOffset() {
			return _offsetMs;
		}

		///<summary>Call this method to make Offset() do nothing.</summary>
		public static void ResetOffset() {
			_offsetMs=0;
		}
		
		///<summary>Call this to use the offset that had been previously set by unit test.</summary>
		public static void Offset(ref DateTime dt) {
			dt=dt.AddMilliseconds(_offsetMs);
		}
	}
}
