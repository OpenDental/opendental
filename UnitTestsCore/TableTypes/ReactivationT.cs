using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ReactivationT {
		
		///<summary>Deletes everything from the recalltype table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearReactivationTable() {
			string command="DELETE FROM reactivation WHERE ReactivationNum > 0";
			DataCore.NonQ(command);
		}
	}
}
