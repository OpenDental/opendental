using OpenDentBusiness;

namespace UnitTestsCore {
	public class SnomedT {
		///<summary>Deletes every entry in the 'snomed' table.</summary>
		public static void ClearSnomedTable() {
			string command="DELETE from snomed";
			DataCore.NonQ(command);
		}

		///<summary>Inserts the new Snomed code and returns it.</summary>
		public static Snomed CreateSnomed(string snomedCode = "",string description = "") {
			Snomed snomed=new Snomed();
			snomed.SnomedCode=snomedCode;
			snomed.Description=description;
			snomed.SnomedNum=Snomeds.Insert(snomed);
			return snomed;
		}

	}
}
