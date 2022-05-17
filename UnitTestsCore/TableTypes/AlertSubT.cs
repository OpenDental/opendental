using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class AlertSubT {
		public static void ClearAlertSubTable() {
			string command="DELETE FROM alertsub WHERE AlertSubNum > 0";
			DataCore.NonQ(command);
		}
	}
}
