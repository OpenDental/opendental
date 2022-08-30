using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class OrthoChartRowT {
		public static void ClearOrthoChartRowTable() {
			string command="DELETE FROM orthochartrow WHERE OrthoChartRowNum > 0";
			DataCore.NonQ(command);
		}
	}
}
