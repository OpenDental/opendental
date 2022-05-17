using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ProcButtonQuickT {
		public static void CreateProcButtonQuick(string description="", string code="",string surf="",int yPos=0, int itemOrder=0, bool isLabel=false) {
			ProcButtonQuicks.InsertNewProcQuickButton(
				description,
				ProcedureCodeT.CreateProcCode(code).ProcCode,
				surf,
				yPos,
				itemOrder,
				isLabel);
		}

	}
}
