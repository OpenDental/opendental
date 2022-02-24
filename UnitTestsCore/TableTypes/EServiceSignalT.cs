using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EServiceSignalT {
		public static void ClearEServiceSignalTable() {
			string command="DELETE FROM eservicesignal";
			DataCore.NonQ(command);
		}
	}
}
