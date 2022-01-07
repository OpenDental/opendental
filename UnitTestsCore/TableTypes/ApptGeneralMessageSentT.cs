using OpenDentBusiness;

namespace UnitTestsCore {
	public class ApptGeneralMessageSentT {
		///<summary>Deletes everything from the apptremindersent table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearApptGeneralMessageSentTable() {
			string command="DELETE FROM apptgeneralmessagesent WHERE ApptGeneralMessageSentNum > 0";
			DataCore.NonQ(command);
		}
	}
}
