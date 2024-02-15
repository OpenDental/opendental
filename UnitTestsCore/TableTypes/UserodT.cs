using System.Collections.Generic;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class UserodT {

		///<summary>Inserts the new user, refreshes the cache and then returns UserNum</summary>
		public static Userod CreateUser(string userName="",string password="",List<long> userGroupNumbers=null,long clinicNum=0,bool isClinicIsRestricted=false,bool isHidden=false,bool doGeneratePassword=true) {
			if(userName=="") {
				userName="Username"+MiscUtils.CreateRandomAlphaNumericString(8);
			}
			if(password=="") {
				password="1234";
			}
			if(userGroupNumbers==null) {
				userGroupNumbers=new List<long> { 1 };
			}
			Userod newUser=new Userod();
			newUser.UserName=userName;
			if(doGeneratePassword) {
				newUser.LoginDetails=Authentication.GenerateLoginDetails(password,HashTypes.SHA3_512);
			}
			newUser.ClinicNum=clinicNum;
			newUser.ClinicIsRestricted=isClinicIsRestricted;
			newUser.IsHidden=isHidden;
			do {
				//In case the username is already taken
				try {
					newUser.UserNum=Userods.Insert(newUser,userGroupNumbers);
				}
				catch {
					newUser.UserName="Username"+MiscUtils.CreateRandomAlphaNumericString(8);
				}
			}while(newUser.UserNum==0);
			Userods.RefreshCache();
			UserGroupAttaches.RefreshCache();
			return newUser;
		}

		///<summary>Always recreate the admin user</summary>
		public static void ClearTableAndCreateDefaultAdminUser() {
			ClearUserodTable();
			DataAction.RunPractice(() => CreateUser("Admin",doGeneratePassword:false));
		}

		public static void ClearPasswords() {
			var users=Userods.GetAll();
			users.ForEach(x => { x.Password=""; Userods.Update(x); });
		}

		///<summary>Clears the Userod and UserGroupAttach tables</summary>
		public static void ClearUserodTable() {
			string command="DELETE FROM userod";
			DataCore.NonQ(command);
			command="DELETE FROM usergroupattach";
			DataCore.NonQ(command);
		}
	}
}
