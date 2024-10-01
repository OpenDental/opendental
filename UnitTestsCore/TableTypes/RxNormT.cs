using OpenDentBusiness;

namespace UnitTestsCore {
	public class RxNormT {
		///<summary>Deletes every entry in the 'rxnorm' table.</summary>
		public static void ClearRxNormTable() {
			string command="DELETE from rxnorm";
			DataCore.NonQ(command);
		}

		///<summary>Inserts the new RxNorm code and returns it.</summary>
		public static RxNorm CreateRxNorm(string rxCui = "",string description = "") {
			RxNorm rxNorm=new RxNorm();
			rxNorm.RxCui=rxCui;
			rxNorm.Description=description;
			rxNorm.RxNormNum=RxNorms.Insert(rxNorm);
			return rxNorm;
		}

	}
}
