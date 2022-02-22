using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ClaimSnapshotT {

		public static void ClearClaimSnapshotTable() {
			string command="DELETE FROM claimsnapshot WHERE ClaimSnapshotNum > 0";
			DataCore.NonQ(command);
		}

	}
}
