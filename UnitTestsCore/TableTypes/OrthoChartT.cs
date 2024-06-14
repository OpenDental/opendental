using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class OrthoChartT {
		public static void ClearOrthoChartTable() {
			string command="DELETE FROM orthochart WHERE OrthoChartNum > 0";
			DataCore.NonQ(command);
		}
	}
}
