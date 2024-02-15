using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RecallTriggerT {

		///<summary>Deletes everything from the recalltrigger table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRecallTriggerTable() {
			string command="DELETE FROM recalltrigger WHERE RecallTriggerNum > 0";
			DataCore.NonQ(command);
		}


	}
}
