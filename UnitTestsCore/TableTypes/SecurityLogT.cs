using OpenDentBusiness;

namespace UnitTestsCore {
	public class SecurityLogT {

		///<summary>Deletes every securitylog of a specified PermType from the securitylog table.</summary>
		public static void ClearSecurityLogsByPermType(EnumPermType permType) {
			string command="DELETE FROM securitylog WHERE PermType="+POut.Int((int)permType);
			DataCore.NonQ(command);
		}

	}
}
