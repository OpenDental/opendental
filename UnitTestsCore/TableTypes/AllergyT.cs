using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AllergyT {
		public static void ClearAllergyTable() {
			string command=$"DELETE FROM allergy WHERE AllergyNum > 0";
			DataCore.NonQ(command);
		}
	}
}
