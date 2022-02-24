using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Userods_Tests {
	[TestClass]
	public class UserodsTests:TestBase {
		///<summary>This method will get invoked after every single test.</summary>
		[TestCleanup]
		public void Cleanup() {
			CredentialsFailedAfterLoginEvent.Fired-=CredentialsFailedAfterLoginEvent_Fired1;
			RemotingClient.HasLoginFailed=false;
			RevertMiddleTierSettingsIfNeeded();
			Security.CurUser=Userods.GetFirstOrDefault(x => x.UserName==UnitTestUserName);
			Security.PasswordTyped=UnitTestPassword;
		}

		[TestMethod]
		public void Userods_CheckUserAndPassword_IncreaseFailedAttemptsAfterUserHasLoggedInButPasswordIsNotCorrect() {
			//First, setup the test scenario.
			//This test is intended to be tested on middle tier. 
			long group1=UserGroupT.CreateUserGroup("usergroup1");
			Userod myUser=UserodT.CreateUser(MethodBase.GetCurrentMethod().Name+DateTime.Now.Ticks,"reallystrongpassword",userGroupNumbers:new List<long>() {group1 });
			RunTestsAgainstMiddleTier(new OpenDentBusiness.WebServices.OpenDentalServerMockIIS(user:myUser.UserName,password:myUser.Password));
			Security.CurUser=myUser;
			Security.PasswordTyped="passwordguess#1";
			CredentialsFailedAfterLoginEvent.Fired+=CredentialsFailedAfterLoginEvent_Fired1;
			//make a single bad password attempt.
			ODException.SwallowAnyException(() => {
				Userods.CheckUserAndPassword(myUser.UserName,"passwordguess#1",false);
			});
			//Get our user from the DB
			RunTestsAgainstDirectConnection();
			myUser=Userods.GetUserByNameNoCache(myUser.UserName);
			//Asssert that the failed attempt got incremented correctly. 
			Assert.AreEqual(1,myUser.FailedAttempts);
		}

		private void CredentialsFailedAfterLoginEvent_Fired1(ODEventArgs e) {
			//If we don't subscribe to this event then the failed event will keep firing over and over.
			RemotingClient.HasLoginFailed=true;
			throw new Exception("Incorrect username and password");
		}

		//Test #2 Attempt 6 times in total and validate that count increases to 5 and then get message that account has been locked. 
		[TestMethod]
		public void Userods_CheckUserAndPassword_LockoutAfterUserHasLoggedInButPasswordIsNotCorrectAfter5Attempts() {
			//First, setup the test scenario.
			long group1=UserGroupT.CreateUserGroup("usergroup1");
			bool isAccountLocked=false;
			Userod myUser=UserodT.CreateUser(MethodBase.GetCurrentMethod().Name+DateTime.Now.Ticks,"reallystrongpassword",userGroupNumbers:new List<long>() {group1 });
			//Make 5 bad password attempts
			for(int i = 1 ;i < 6;i++) {
				ODException.SwallowAnyException(() => {
					Userods.CheckUserAndPassword(myUser.UserName,"passwordguess#"+i,false);
				});
			}
			try {
				//the 6th bad attempt should kick us with a message saying that our account has been locked.
				Userods.CheckUserAndPassword(myUser.UserName,"passwordguess#6",false);
			}
			catch (Exception e) {
				if(e.Message.Contains("Account has been locked due to failed log in attempts")) {
					isAccountLocked=true;
				}
			}
			//Get our updated user from the DB. 
			myUser=Userods.GetUserByNameNoCache(myUser.UserName);
			//Assert that we got to 5 failed attempts and that the account has been locked. 
			Assert.AreEqual(5,myUser.FailedAttempts);
			Assert.AreEqual(true,isAccountLocked);
		}

		//Test #3 Middle tier specific. Try with wrong password, try different method that calls CheckUSersAndPassword but so that we don't do it directly.
		//Make sure that we don't get to the lockout winodw, or query the DB to make sure failed attempts doesn't get past 1. 
		[TestMethod]
		public void Userods_CheckUserAndPassoword_UpdateFailedAttemptsFromOtherMethods() {
			//First, setup the test scenario.
			long group1=UserGroupT.CreateUserGroup("usergroup1");
			Userod myUser=UserodT.CreateUser(MethodBase.GetCurrentMethod().Name+DateTime.Now.Ticks,"reallystrongpassword",userGroupNumbers:new List<long>() {group1 });
			Security.CurUser=myUser;
			Security.PasswordTyped="passwordguess#1";
			CredentialsFailedAfterLoginEvent.Fired+=CredentialsFailedAfterLoginEvent_Fired1;
			RunTestsAgainstMiddleTier(new OpenDentBusiness.WebServices.OpenDentalServerMockIIS(user:myUser.UserName,password:myUser.Password));
			//try once with the wrong password. Failed attempt should get incremented to 1. 
			ODException.SwallowAnyException(() => {
				Userods.CheckUserAndPassword(myUser.UserName,"passwordguess#1",false);
			});
			//Get our updated user from the DB.
			RunTestsAgainstDirectConnection(); 
			myUser=Userods.GetUserByNameNoCache(myUser.UserName);
			//Assert that we only have 1 failed attempt. 
			Assert.AreEqual(1,myUser.FailedAttempts);
			//now wait for another method to get called
			RunTestsAgainstMiddleTier(new OpenDentBusiness.WebServices.OpenDentalServerMockIIS(user:myUser.UserName,password:myUser.Password));
			ODException.SwallowAnyException(() => {
				Computers.UpdateHeartBeat(Environment.MachineName,false);
			});
			RunTestsAgainstDirectConnection();
			//Get our updated user from the DB. 
			myUser=Userods.GetUserByNameNoCache(myUser.UserName);
			//Assert that we only have 1 failed attempt. 
			Assert.AreEqual(1,myUser.FailedAttempts);
		}

		//Test #4 Similar to #3, call method checkusersandpassword specifically over middle tier assert that failed attempts do get increased to 5. 
		[TestMethod]
		public void Userods_CheckUserAndPassoword_UpdateFailedAttemptsTo5() {
			//First, setup the test scenario.
			long group1=UserGroupT.CreateUserGroup("usergroup1");
			Userod myUser=UserodT.CreateUser(MethodBase.GetCurrentMethod().Name+DateTime.Now.Ticks,"reallystrongpassword",userGroupNumbers:new List<long>() {group1 });
			CredentialsFailedAfterLoginEvent.Fired+=CredentialsFailedAfterLoginEvent_Fired1;
			Security.CurUser=myUser;
			Security.PasswordTyped="passwordguess#1";
			RunTestsAgainstMiddleTier();
			//try with 5 incorrect passwords. Failed attempt should get incremented to 5. 
			for(int i = 1 ;i < 6;i++) {
				ODException.SwallowAnyException(() => {
					try {
						Userods.CheckUserAndPassword(myUser.UserName,"passwordguess#"+i,false);
					}
					catch(Exception e) {

					}
					
				});
			}
			//Get our updated user from the DB. 
			RunTestsAgainstDirectConnection();
			myUser=Userods.GetUserByNameNoCache(myUser.UserName);
			//Assert that there are 5 failed attempts. 
			Assert.AreEqual(5,myUser.FailedAttempts);
		}

		/// <summary>
		/// This test is to make sure that all values are being copied according to specs of F15854
		/// A demo user and clinic is being created and then a copy of said user is made
		/// </summary>
		[TestMethod]
		public void Userods_CopyUser_HappyPath() {
			Userod user=UserodT.CreateUser();
			Clinic clinic=ClinicT.CreateClinic();
			UserClinics.Insert(new UserClinic(clinic.ClinicNum,user.UserNum));
			AlertSubs.Insert(new AlertSub(user.UserNum,clinic.ClinicNum,1));
			UserOdPrefs.Insert(new UserOdPref() { UserNum=user.UserNum,ClinicNum=clinic.ClinicNum,Fkey=clinic.ClinicNum,FkeyType=UserOdFkeyType.ClinicLast});
			//Setup user
			//Fields given by method caller
			PasswordContainer loginDetailsNotExpected=user.LoginDetails;
			string password="Asdf1234!@#$";
			PasswordContainer loginDetails=Authentication.GenerateLoginDetailsSHA512(password);
			bool isPasswordStrong=string.IsNullOrEmpty(Userods.IsPasswordStrong(password));
			string copiedUserName=user.UserName+"(Copy)";
			//Fields directly copied
			bool clinicIsRestrictedExpected=user.ClinicIsRestricted;
			long clinicNumExpected=user.ClinicNum;
			List<UserGroupAttach> listAttachesExpected=UserGroupAttaches.GetForUser(user.UserNum);//Mimics Userods.Insert(...)
			List<UserClinic> listUserClinicsExpected=UserClinics.GetForUser(user.UserNum);//Mimics 
			List<AlertSub> listAlertSubsExpected=AlertSubs.GetAllForUser(user.UserNum);
			//Copy User
			long copiedUserNum=Userods.CopyUser(user,loginDetails,isPasswordStrong,copiedUserName,isForCemt:false).UserNum;
			Cache.Refresh(InvalidType.AllLocal);
			Userod copy=Userods.GetUser(copiedUserNum);
			//Assert
			//Fields given by method caller
			Assert.AreEqual(copiedUserName,copy.UserName);
			Assert.AreEqual(isPasswordStrong,copy.PasswordIsStrong);
			Assert.AreEqual(loginDetails.ToString(),copy.Password);
			Assert.AreNotEqual(loginDetailsNotExpected.ToString(),copy.Password);//Source user's password should not have been copied.
			//Fields directly copied
			Assert.AreEqual(clinicIsRestrictedExpected,copy.ClinicIsRestricted);
			Assert.AreEqual(clinicNumExpected,copy.ClinicNum);
			List<UserGroupAttach> listAttaches=UserGroupAttaches.GetForUser(copy.UserNum);
			Assert.AreEqual(listAttachesExpected.Count,listAttaches.Count);
			foreach(UserGroupAttach expected in listAttachesExpected) {
				Assert.IsTrue(listAttaches.Exists(x => x.UserGroupNum==expected.UserGroupNum));
			}
			List<UserClinic> listUserClinics=UserClinics.GetForUser(copy.UserNum);
			Assert.AreEqual(listUserClinicsExpected.Count,listUserClinics.Count);
			foreach(UserClinic expected in listUserClinicsExpected) {
				Assert.IsTrue(listUserClinics.Exists(x => x.ClinicNum==expected.ClinicNum));
			}
			List<AlertSub> listAlertSubs=AlertSubs.GetAllForUser(copy.UserNum);
			Assert.AreEqual(listAlertSubsExpected.Count,listAlertSubs.Count);
			foreach(AlertSub expected in listAlertSubsExpected) {
				Assert.IsTrue(listAlertSubs.Exists(x => x.ClinicNum==expected.ClinicNum && x.Type==expected.Type 
					&& x.AlertCategoryNum==expected.AlertCategoryNum));
			}
			//Fields not copied (set to default)
			Assert.AreEqual(0,copy.EmployeeNum);
			Assert.AreEqual(0,copy.ProvNum);
			Assert.AreEqual(false,copy.IsHidden);
			Assert.AreEqual(0,copy.TaskListInBox);
			Assert.AreEqual(0,copy.AnesthProvType);
			Assert.AreEqual(false,copy.DefaultHidePopups);
			Assert.AreEqual(0,copy.UserNumCEMT);
			Assert.AreEqual(DateTime.MinValue,copy.DateTFail);
			Assert.AreEqual(0,copy.FailedAttempts);
			Assert.AreEqual("",copy.DomainUser);
			Assert.AreEqual("",copy.MobileWebPin);
			Assert.AreEqual(0,copy.MobileWebPinFailedAttempts);
			Assert.AreEqual(DateTime.MinValue,copy.DateTLastLogin);
			List<UserOdPref> listUserOdPrefs = UserOdPrefT.GetByUser(copy.UserNum);
			Assert.AreEqual(0,listUserOdPrefs.Count);
		}

		/// <summary>
		/// Tests if GetUniqueUserName called in CopyUser is performing as expected.
		/// </summary>
		[TestMethod]
		public void Userods_CopyUser_NameInUse() {
			Userod user=UserodT.CreateUser();
			//Setup user
			//Fields given by method caller
			Userods.CopyUser(user,user.LoginDetails,false,isForCemt:false);//First copy
			long copiedUserNum2=Userods.CopyUser(user,user.LoginDetails,false,isForCemt:false).UserNum;//Second copy
			Cache.Refresh(InvalidType.Security);
			Userod copy=Userods.GetUser(copiedUserNum2);
			Assert.AreEqual(user.UserName+"(Copy)(2)",copy.UserName);
		}
	}
}
