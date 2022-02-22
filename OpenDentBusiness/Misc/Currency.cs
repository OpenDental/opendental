using System;
using System.Globalization;

namespace OpenDentBusiness {
	public class Currency {
		
		///<summary>Gets StringFormat for currency. "F2" for customers, "F4" for HQ.</summary>
		public static string GetCurrencyFormat() {
			if(PrefC.IsODHQ) {
				return "0.00##";
			}
			else {
				return "F2";
			}
		}

		/// <summary> Rounds amt to 2 places for customers, and 4 places for hq. </summary>
		public static double Round(double amt) {
			return PIn.Double(amt.ToString(GetCurrencyFormat()));
		}

	}
}
