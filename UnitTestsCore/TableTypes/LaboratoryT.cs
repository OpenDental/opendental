using OpenDentBusiness;

namespace UnitTestsCore {
	public class LaboratoryT {

		///<summary>Deletes everything from the laboratory table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearLaboratoryTable() {
			string command="DELETE FROM laboratory";
			DataCore.NonQ(command);
		}

	}
}
