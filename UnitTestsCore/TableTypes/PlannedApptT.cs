using OpenDentBusiness;

namespace UnitTestsCore {
	public class PlannedApptT {

		///<summary>Deletes everything from the plannedappt table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearPlannedApptTable() {
			string command="DELETE FROM plannedappt";
			DataCore.NonQ(command);
		}

	}
}
