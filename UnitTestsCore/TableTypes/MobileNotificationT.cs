using OpenDentBusiness;
using OpenDentBusiness.Crud;
using System.Collections.Generic;

namespace UnitTestsCore {
	public class MobileNotificationT {
		public static void ClearTable() {
			string command="DELETE FROM mobilenotification";
			DataCore.NonQ(command);
		}

		public static List<MobileNotification> GetAll() {
			string command="SELECT * FROM mobilenotification";
			return MobileNotificationCrud.SelectMany(command);
		}
	}
}
