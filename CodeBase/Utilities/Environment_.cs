﻿using CodeBase;
using System;

namespace OpenDentalWebCore {
	public static class Environment_ {
		private static Func<string> _getMachineName=() => Environment.MachineName;
		public static bool IsMachineNameModified {
			get; private set;
		}
		public static string MachineName => _getMachineName();

		///<summary>Sets machine name, only allowed in debug.</summary>
		public static void SetMachineName(Func<string> getMachineName) {
			if(!ODBuild.IsDebug()) {
				throw new Exception("Not allowed to change Environment_.MachineName in release.");
			}
			IsMachineNameModified=true;
			_getMachineName=getMachineName;
		}

		///<summary>Resets mock machine name back to the current machine's actual machine name.</summary>
		public static void ResetMachineName() {
			IsMachineNameModified=false;
			_getMachineName=() => Environment.MachineName;
		}
	}
}