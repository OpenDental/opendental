using System;

namespace CodeBase {
	public class ODMethodsT {
		///<summary>Returns a new instance of T if input is null.</summary>
		public static T Coalesce<T>(T input) where T : new() {
			if(input!=null) {
				return input;
			}
			return new T();
		}

		///<summary>Returns the specified defaultValue of T if input is null.</summary>
		public static T Coalesce<T>(T input,T defaultVal) {
			if(input!=null) {
				return input;
			}
			return defaultVal;
		}
		
		///<summary>Returns the specified defaultValue of T if input is null or if input.ComparTo(checkAgainst)==0.</summary>
		public static T ValueIf<T>(T input,T checkAgainst,T defaultVal) where T : IComparable {
			if(input==null||input.CompareTo(checkAgainst)==0) {
				return defaultVal;
			}
			return input;
		}
	}
}
