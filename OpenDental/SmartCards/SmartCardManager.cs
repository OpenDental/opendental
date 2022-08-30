using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.SmartCards  {
	public static class SmartCardManager {
		/// <summary>
		/// Gets the <see cref="ISmartCardManager"/> for the current operation system.
		/// </summary>
		/// <returns>
		/// A <see cref="ISmartCardManager"/> if the current operation system is supported, else, <see langword="null"/>.
		/// </returns>
		public static ISmartCardManager Load() {
			if(Environment.OSVersion.Platform == PlatformID.Win32NT) {
				return new WindowsSmartCardManager();
			}
			else {
				return null;
			}
		}
	}
}
