using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EServiceBillingT {
		public static EServiceBilling GetByRegKeyNum(long regKeyNum) {
			string command="SELECT * FROM eservicebilling WHERE RegistrationKeyNum="+POut.Long(regKeyNum);
			return OpenDentBusiness.Crud.EServiceBillingCrud.SelectOne(command);
		}

		public static void UpdateDateEntry(long regKeyNum,DateTime newDateTimeEntry) {
			string command=$"UPDATE eservicebilling SET DateTimeEntry={POut.DateT(newDateTimeEntry)} WHERE RegistrationKeyNum={POut.Long(regKeyNum)}";
			DataAction.RunCustomers(() => DataCore.NonQ(command));
		}

		public static void ClearTable() {
			string command="TRUNCATE TABLE eservicebilling";
			DataAction.RunCustomers(() => DataCore.NonQ(command));
		}
	}
}
