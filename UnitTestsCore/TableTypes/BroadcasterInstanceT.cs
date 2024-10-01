using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class BroadcasterInstanceT {
		public static void ClearBroadcasterInstanceTable() {
			string command="TRUNCATE TABLE broadcasterinstance";
			DataAction.RunEServices(()=>DataCore.NonQ(command));
		}
	}
}
