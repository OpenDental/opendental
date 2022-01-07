using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ApptFieldDefT {

		public static void ClearApptFieldDefTable() {
			string command="DELETE FROM apptfielddef WHERE ApptFieldDefNum > 0";
			DataCore.NonQ(command);
		}
	}
}
