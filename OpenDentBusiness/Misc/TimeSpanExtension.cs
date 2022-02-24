using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	public static class TimeSpanExtension {

		///<summary>-H:mm.  If zero, then returns empty string.  Hours can be greater than 24.</summary>
		public static string ToStringHmm(this TimeSpan tspan) {
			//No need to check RemotingRole; no call to db.
			if(tspan==TimeSpan.Zero) {
				return "";
			}
			string retVal="";
			if(tspan < TimeSpan.Zero) {
				retVal+="-";
				tspan=tspan.Duration();
			}
			//It has to be done this way to support hours greater than 24.
			int hours=(tspan.Days*24)+tspan.Hours;
			retVal+=hours.ToString()+":"+tspan.Minutes.ToString().PadLeft(2,'0');
			return retVal;
		}

		///<summary>-H:mm:ss.  If zero, then returns empty string.</summary>
		public static string ToStringHmmss(this TimeSpan tspan) {
			//No need to check RemotingRole; no call to db.
			if(tspan==TimeSpan.Zero) {
				return "";
			}
			string retVal="";
			if(tspan < TimeSpan.Zero) {
				retVal+="-";
				tspan=tspan.Duration();
			}
			int hours=(tspan.Days*24)+tspan.Hours;
			retVal+=hours.ToString()+":"+tspan.Minutes.ToString().PadLeft(2,'0')+":"+tspan.Seconds.ToString().PadLeft(2,'0');
			return retVal;
		}

		///<summary>-mm:ss.  If zero, then returns empty string.</summary>
		public static string ToStringmmss(this TimeSpan tspan) {
			//No need to check RemotingRole; no call to db.
			if(tspan==TimeSpan.Zero) {
				return "";
			}
			string retVal="";
			if(tspan < TimeSpan.Zero) {
				retVal+="-";
				tspan=tspan.Duration();
			}
			retVal+=((int)tspan.TotalMinutes).ToString().PadLeft(2,'0')+":"+tspan.Seconds.ToString().PadLeft(2,'0');
			return retVal;
		}

		///<summary>Does not work well with negative values.</summary>
		public static string ToString(this TimeSpan tspan,string format) {
			//No need to check RemotingRole; no call to db.
			DateTime dt=DateTime.Today;
			dt=dt+tspan;
			return dt.ToString(format);
		}

		///<summary>Does not work well with negative values.</summary>
		public static string ToShortTimeString(this TimeSpan tspan) {
			//No need to check RemotingRole; no call to db.
			DateTime dt=DateTime.Today;
			dt=dt+tspan;
			return dt.ToShortTimeString();
		}


	}
}
