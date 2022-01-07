using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;


namespace UnitTests.Authentication_Tests {
	[TestClass]
	public class AuthenticationTests:TestBase {

		[TestMethod]
		public void Authentication_CheckUserodPassword() {
			Userod user=Security.CurUser;
			user.LoginDetails=Authentication.GenerateLoginDetails("awesomePassword",HashTypes.SHA3_512);
			bool result=Authentication.CheckPassword(user,"awesomePassword");
			Assert.IsTrue(result);
			Authentication.UpdatePasswordUserod(user,"awesomePassword");
			//If this is middletier, we need the password to match in our current user object to refill the cache
			Security.PasswordTyped="awesomePassword";
			//Refresh our user object;
			Userods.RefreshCache();
			user=Userods.GetUser(user.UserNum);
			Assert.AreEqual(88,user.PasswordHash.Length);
			string passhash=Authentication.HashPasswordSHA512("awesomePassword",user.LoginDetails.Salt);
			Assert.IsTrue(Authentication.ConstantEquals(passhash,user.PasswordHash));
			//Reset Security.CurUser password back to the unit test password
			Authentication.UpdatePasswordUserod(user,UnitTestPassword);
			//Reset typed password
			Security.PasswordTyped=UnitTestPassword;
			Userods.RefreshCache();
			Security.CurUser=Userods.GetUser(user.UserNum);
		}

		[TestMethod]
		public void Authentication_HashChecking() {
			string salt=Authentication.GenerateSalt(HashTypes.SHA3_512);
			//MD5 Correct
			HashTypes typeCur=HashTypes.MD5;
			string testHash=Authentication.GetHash("Neuromancer",salt,typeCur);
			bool result=Authentication.CheckPassword("Neuromancer",salt,testHash,typeCur);
			Assert.IsTrue(result);
			//MD5 Incorrect
			result=Authentication.CheckPassword("WRONG",salt,testHash,typeCur);
			Assert.IsFalse(result);

			//MD5_ECW Correct
			typeCur=HashTypes.MD5_ECW;
			testHash=Authentication.GetHash("Neuromancer",salt,typeCur);
			result=Authentication.CheckPassword("Neuromancer",salt,testHash,typeCur);
			Assert.IsTrue(result);
			//MD5_ECW Incorrect
			result=Authentication.CheckPassword("Totally-A-Hash",salt,testHash,typeCur);
			Assert.IsFalse(result);

			//SHA3-512 Correct
			typeCur=HashTypes.SHA3_512;		
			testHash=Authentication.GetHash("Neuromancer",salt,typeCur);
			result=Authentication.CheckPassword("Neuromancer",salt,testHash,typeCur);
			Assert.IsTrue(result);
			//SHA3-512 Incorrect
			result=Authentication.CheckPassword("NoHashingForYou",salt,testHash,typeCur);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void Authentication_UpdatePasswordSchema() {
			Userod user=Security.CurUser;
			bool result=Authentication.UpdatePasswordUserod(user,"brandSpankinNewPassword",HashTypes.SHA3_512);
			//If this is middletier, we need the password to match in our current user object to refill the cache
			Security.PasswordTyped="brandSpankinNewPassword";
			Userods.RefreshCache();
			user=Userods.GetUser(user.UserNum);
			Assert.IsTrue(result);
			result=Authentication.CheckPassword(user,"brandSpankinNewPassword");
			Assert.IsTrue(result);
			//Reset Security.CurUser password back to the unit test password
			Authentication.UpdatePasswordUserod(user,UnitTestPassword,HashTypes.SHA3_512);
			//Reset typed password
			Security.PasswordTyped=UnitTestPassword;
			Userods.RefreshCache();
			Security.CurUser=Userods.GetUser(user.UserNum);
		}

		[TestMethod]
		public void Authentication_GenerateSaltLengths() {
			string output=Authentication.GenerateSalt(3);
			//Every 3 bytes is 4 characters because of the base-64 encoding
			Assert.AreEqual(output.Length,4);
			output=Authentication.GenerateSalt(2);	//Anything smaller has padding characters(=)
			Assert.AreEqual(output.Length,4);
			output=Authentication.GenerateSalt(30);
			Assert.AreEqual(output.Length,40);
			output=Authentication.GenerateSalt(31);
			Assert.AreEqual(output.Length,44);
			output=Authentication.GenerateSalt(0);
			Assert.AreEqual(output.Length,0);
		}

		[TestMethod]
		public void Authentication_ConstantEqualsTiming() {
			Stopwatch s=new Stopwatch();
			string reallyLooong="CosaNostra Pizza doesn't have any competition. Competition goes against the Mafia ethic.";
			//First character is incorrect, so usually string comparison would be fast, but we want constant.
			string longNoMatch="%osaNostra Pizza doesn't have any competition. Competition goes against the Mafia ethic.";
			long matchTotal=0;
			long matchAvg;
			long noMatchTotal=0;
			long noMatchAvg;
			//Call it once before we start testing, so the JIT compiler doesn't mess with timings.
			Authentication.ConstantEquals("a","a");
			for(int i=0; i<30;i++) {
				s.Start();
				for(int k=0; k<50;k++) {
					Authentication.ConstantEquals(reallyLooong,reallyLooong);
				}
				s.Stop();
				matchTotal+=s.ElapsedTicks;
				s.Reset();
			}
			matchAvg=matchTotal/30;

			for(int i=0; i<30;i++) {
				s.Start();
				for(int k=0; k<50;k++) {
					Authentication.ConstantEquals(longNoMatch,reallyLooong);
				}
				s.Stop();
				noMatchTotal+=s.ElapsedTicks;
				s.Reset();
			}
			noMatchAvg=noMatchTotal/30;
			long deltaV=Math.Abs(noMatchAvg-matchAvg);
			Assert.IsTrue(deltaV<75);
		}

		[TestMethod]
		public void Authentication_VerifyMD5Hashing() {
			//Our regular MD5 encodes text as unicode, which makes it difficult to normal hashes
			//MD5_ECW encodes as ascii, which makes it easier to test.  Same underlying algorithm is used for both.
			string blank=" ";
			string test=Authentication.HashPasswordMD5(blank,true);
			Assert.IsTrue(test=="7215ee9c7d9dc229d2921a40e899ec5f");
		}

		[TestMethod]
		public void Authentication_VerifySHA3_512Hashing() {
			string blank=" ";
			string test=Authentication.HashPasswordSHA512(blank,"");
			Assert.IsTrue(test=="J3k3VWdgC8ffI5EhDVNr8aNfrtYZqm+bwUDFSbTbArOodGBxByKJC8S1gRmJwNKO0omhhpjb6DWp8nBMlfm4cA==");
		}

		[TestMethod]
		public void Authentication_PasswordContainerDecode() {
			string blankpass="";
			string nonePass="None$$";
			string someText="None$TrashPanda$Groot";
			string md5Hash="7215ee9c7d9dc229d2921a==";

			string trashPass="lakjdfhldjfyndslf";
			string invalidType="Vroom$Vroom$Racing";

			PasswordContainer passCont;
			
			passCont=Authentication.DecodePass(nonePass);
			Assert.AreEqual(HashTypes.None,passCont.HashType);
			Assert.AreEqual("",passCont.Hash);
			Assert.AreEqual("",passCont.Salt);
			Assert.AreEqual(nonePass,passCont.ToString());

			passCont=Authentication.DecodePass(blankpass);
			Assert.AreEqual(HashTypes.None,passCont.HashType);
			Assert.AreEqual("",passCont.Hash);
			Assert.AreEqual("",passCont.Salt);
			//nonePass is the correct format for a blank password.
			Assert.AreEqual(nonePass,passCont.ToString());

			passCont=Authentication.DecodePass(someText);
			Assert.AreEqual(HashTypes.None,passCont.HashType);
			Assert.AreEqual("TrashPanda",passCont.Salt);
			Assert.AreEqual("Groot",passCont.Hash);
			Assert.AreEqual(someText,passCont.ToString());

			passCont=Authentication.DecodePass(md5Hash);
			Assert.AreEqual(HashTypes.MD5,passCont.HashType);
			Assert.AreEqual("7215ee9c7d9dc229d2921a==",passCont.Hash);
			Assert.AreEqual("",passCont.Salt);
			Assert.AreEqual("MD5$$7215ee9c7d9dc229d2921a==",passCont.ToString());
			
			passCont=Authentication.DecodePass(trashPass);
			Assert.AreEqual(HashTypes.None,passCont.HashType);
			Assert.AreEqual("lakjdfhldjfyndslf",passCont.Hash);
			Assert.AreEqual("",passCont.Salt);
			Assert.AreEqual("None$$lakjdfhldjfyndslf",passCont.ToString());

			passCont=Authentication.DecodePass(invalidType);
			Assert.AreEqual(HashTypes.None,passCont.HashType);
			Assert.AreEqual("Vroom$Vroom$Racing",passCont.Hash);
			Assert.AreEqual("",passCont.Salt);
			Assert.AreEqual("None$$Vroom$Vroom$Racing",passCont.ToString());
		}

		[TestMethod]
		public void Authentication_PasswordContainerHashType() {
			string[] hashTypes=new string[] {
				"MD5$$7215ee9c7d9dc229d2921a==",
				"7215ee9c7d9dc229d2921a==",
				"SHA3_512$$J3k3VWdgC8ffI5EhDVNr8aNfrtYZqm+bwUDFSbTbArOodGBxByKJC8S1gRmJwNKO0omhhpjb6DWp8nBMlfm4cA=="
			};
			PasswordContainer passCont;

			passCont=Authentication.DecodePass(hashTypes[0]);
			Assert.AreEqual(HashTypes.MD5,passCont.HashType);
			Assert.AreEqual(hashTypes[0],passCont.ToString());

			passCont=Authentication.DecodePass(hashTypes[1]);
			Assert.AreEqual(HashTypes.MD5,passCont.HashType);
			//Container prepends "MD5$$" to the string
			Assert.AreEqual(hashTypes[0],passCont.ToString());
			
			passCont=Authentication.DecodePass(hashTypes[2]);
			Assert.AreEqual(HashTypes.SHA3_512,passCont.HashType);
			Assert.AreEqual(hashTypes[2],passCont.ToString());
		}

		[TestMethod]
		public void Authentication_TestAuthentication_eCW() {
			//Run this test against a direct connection. eCW uses MD5 hash. If middle tier is enabled, the default User will have a wrong password. 
			//Once we enable eCW, all requests will fail.
			RunTestsAgainstDirectConnection();
			//Turn eCW on for this test.
			OpenDentBusiness.Program p=Programs.GetCur(ProgramName.eClinicalWorks);
			p.Enabled=true;
			Programs.Update(p);
			Programs.RefreshCache();
			//List of hashes to try
			string[] hashStrings=new string[] {
				"MD5_ECW$$81dc9bdb52d04dc20036dbd8313ed055",
				"81dc9bdb52d04dc20036dbd8313ed055",
			};
			PasswordContainer passCont;
			//Test password decode.
			for(int i = 0;i<2;i++) {
				passCont=Authentication.DecodePass(hashStrings[i]);
				Assert.AreEqual(HashTypes.MD5_ECW,passCont.HashType);
				Assert.AreEqual(hashStrings[1],passCont.Hash);
				Assert.AreEqual("",passCont.Salt);
				//Always compare to hashString[0] because decoding it should prepend "MD5_ECW$$" to the string.
				Assert.AreEqual(hashStrings[0],passCont.ToString());
			}
			//Turn eCW back off at the end of the test.
			p.Enabled=false;
			Programs.Update(p);
			Programs.RefreshCache();
			RevertMiddleTierSettingsIfNeeded();
		}
	}
}
