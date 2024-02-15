using OpenDentBusiness;

namespace UnitTestsCore {
	public class DocumentT {

		///<summary>Deletes everything from the document table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearDocumentTable() {
			string command="DELETE FROM document";
			DataCore.NonQ(command);
		}

	}
}
