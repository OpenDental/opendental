using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class UserWebT {

		public static UserWeb CreateUserWeb(long fKey,UserWebFKeyType fKeyType,string userName="",string password="",bool requireUserNameChange=false,
			bool requirePasswordChange=false,string passwordResetCode="",DateTime dateTimeLastLogin=default(DateTime))
		{
			UserWeb userWeb=new UserWeb() {
				IsNew=true,
				FKey=fKey,
				FKeyType=fKeyType,
				UserName=userName,
				Password=password,
				RequireUserNameChange=requireUserNameChange,
				RequirePasswordChange=requirePasswordChange,
				PasswordResetCode=passwordResetCode,
				DateTimeLastLogin=dateTimeLastLogin,
			};
			UserWebs.Insert(userWeb);
			return userWeb;
		}

		///<summary>Clears the table. Does not truncate so that primary keys are not re-used.</summary>
		public static void ClearUserWebTable() {
			string command="DELETE FROM userweb WHERE UserWebNum > 0";
			DataCore.NonQ(command);
		}

	}
}
