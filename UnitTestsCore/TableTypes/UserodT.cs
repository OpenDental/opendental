using System.Collections.Generic;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class UserodT {

		///<summary>Inserts the new user, refreshes the cache and then returns UserNum</summary>
		public static Userod CreateUser(string userName="",string password="",List<long> userGroupNumbers=null,long clinicNum=0,bool isClinicIsRestricted=false) {
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
			newUser.LoginDetails=Authentication.GenerateLoginDetails(password,HashTypes.SHA3_512);
			newUser.ClinicNum=clinicNum;
			newUser.ClinicIsRestricted=isClinicIsRestricted;
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

		public static void ClearPasswords() {
			var users=Userods.GetAll();
			users.ForEach(x => { x.Password=""; Userods.Update(x); });
		}
	}
}
