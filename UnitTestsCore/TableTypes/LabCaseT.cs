using OpenDentBusiness;

namespace UnitTestsCore {
	public class LabCaseT {

		///<summary>Deletes everything from the labcase table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearLabCaseTable() {
			string command="DELETE FROM labcase";
			DataCore.NonQ(command);
		}

	}
}
