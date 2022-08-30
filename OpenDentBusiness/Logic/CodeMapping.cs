using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDentBusiness {
	public class CodeMapping {

		///<summary>Takes a ProcedureCode and returns the given arch for that code based on ADA standards.
		///Returns "U" by default, an empty string for Canada, "U" for Maxillary codes and "L" for Mandibular codes. </summary>
		public static string GetArchSurfaceFromProcCode(ProcedureCode code) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "";
			}
			else if(CDT.Class1.GetMandibularCodes().Any(x => x.ProcCode==code.ProcCode)) {
				return "L";
			}
			//We assume that if this isn't a Mandibular code then it is Maxillary or "upper arch"
			return "U";
		}

	}
}
