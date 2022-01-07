using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RegistrationKeyT {
		public static RegistrationKey CreateRegKey(long patNum,bool isOnlyForTesting=false) {
			RegistrationKey regKey=new RegistrationKey {
				PatNum=patNum,
				DateStarted=DateTime_.Today,
				IsOnlyForTesting=isOnlyForTesting,
				RegKey=MiscUtils.CreateRandomAlphaNumericString(16).ToUpper(),
			};
			regKey.RegistrationKeyNum=OpenDentBusiness.Crud.RegistrationKeyCrud.Insert(regKey);
			return regKey;
		}

		///<summary>Deletes everything from the registrationkey table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearRegKeyTable() {
			string command="DELETE FROM registrationkey WHERE RegistrationKeyNum > 0";
			DataCore.NonQ(command);
		}
	}
}
