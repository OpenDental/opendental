using OpenDentBusiness;

namespace UnitTestsCore {
	public class RxPatT {

		///<summary>Deletes everything from the rxpat table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRxPatTable() {
			string command="DELETE FROM rxpat";
			DataCore.NonQ(command);
		}

	}
}
