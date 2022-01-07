using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase {
	public class ODBuild {

		///<summary>Returns true if current build is for Windows OS.</summary>
		public static bool IsWindows() {
			return true;//Later this will be enhanced when we have non-Windows builds.
		}

		///<summary>Returns true if the current build is debug. Useful when you want the release code to show up when searching for references.</summary>
		public static bool IsDebug() {
			//There are some sections of code where we can't use this.  Specifically, places where a variable is a different type in debug vs not.  If anyone has a suggestion for including that pattern, let Jordan know.
#if DEBUG
			return true;
#else
			return false;
#endif
		}

		///<summary>Returns true if the current build is alpha. Useful when you want the release code to show up when searching for references.</summary>
		public static bool IsAlpha() {
#if ALPHA
			return true;
#else
			return false;
#endif
		}

		///<summary>Returns true if current build is using Virtual UI.</summary>
		public static bool IsWeb() {
#if WEB
			return true;
#else
			return false;
#endif
		}

		///<summary>Returns true if current build is for the trial version only.</summary>
		public static bool IsTrial() {
#if TRIALONLY
			return true;
#else
			return false;
#endif
		}

	}
}
