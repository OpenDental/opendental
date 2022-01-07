using OpenDentBusiness;

namespace UnitTestsCore {
	public class ApptReminderSentT {
		///<summary>Deletes everything from the apptremindersent table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearApptReminderSentTable() {
			string command="DELETE FROM apptremindersent WHERE ApptReminderSentNum > 0";
			DataCore.NonQ(command);
		}
	}
}
