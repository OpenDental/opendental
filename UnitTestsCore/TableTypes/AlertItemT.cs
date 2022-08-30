using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class AlertItemT {

		///<summary>Deletes all rows from the AlertItem table.  Does not truncate the table, so that PKs are not reused on accident.</summary>
		public static void ClearAlertItemTable() {
			string command="DELETE FROM alertitem WHERE AlertItemNum > 0";
			DataCore.NonQ(command);
		}

	}
}