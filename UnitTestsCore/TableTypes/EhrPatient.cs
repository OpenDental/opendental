using OpenDentBusiness;

namespace UnitTestsCore {
	public class EhrPatientT {

		///<summary>Deletes everything from the ehrpatient table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEhrPatientTable() {
			string command="DELETE FROM ehrpatient";
			DataCore.NonQ(command);
		}

	}
}