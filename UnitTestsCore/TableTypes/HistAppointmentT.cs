using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class HistAppointmentT {

		///<summary>Deletes everything from the histappointment table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearHistAppointmentTable() {
			string command="DELETE FROM histappointment WHERE HistApptNum > 0";
			DataCore.NonQ(command);
		}


	}
}
