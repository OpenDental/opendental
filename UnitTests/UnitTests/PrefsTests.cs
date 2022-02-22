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

namespace UnitTests.Prefs_Tests {
	[TestClass]
	public class PrefsTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		/************************************************************************************************************************************************

		///<summary>Duplicate native preferences are not allowed and should throw an exception when filling the cache.</summary>
		[TestMethod]
		public void Prefs_FillCacheFromTable_DuplicateDetected() {
			//Create a duplicate preference that is native to Open Dental.
			List<Pref> listPrefs=new List<Pref>() {
				new Pref(){ PrefName=PrefName.AccountAllowFutureDebits.ToString() },
				new Pref(){ PrefName=PrefName.AccountAllowFutureDebits.ToString() },
			};
			bool hadException=false;
			try {
				Prefs.FillCacheFromTable(OpenDentBusiness.Crud.PrefCrud.ListToTable(listPrefs));
			}
			catch(Exception e) {
				e.DoNothing();
				hadException=true;
			}
			Prefs.RefreshCache();//Put the preference cache back the way it was.
			Assert.IsTrue(hadException);//An exception should have been thrown because these duplicate preferences are native.
		}

		///<summary>Duplicate non-native preferences are allowed should not throw an exception when filling the cache.</summary>
		[TestMethod]
		public void Prefs_FillCacheFromTable_DuplicateAllowed() {
			//Create a duplicate preference that is not native to Open Dental.
			List<Pref> listPrefs=new List<Pref>() {
				new Pref(){ PrefName=MethodBase.GetCurrentMethod().Name },
				new Pref(){ PrefName=MethodBase.GetCurrentMethod().Name },
			};
			bool hadException=false;
			try {
				Prefs.FillCacheFromTable(OpenDentBusiness.Crud.PrefCrud.ListToTable(listPrefs));
			}
			catch(Exception e) {
				e.DoNothing();
				hadException=true;
			}
			Prefs.RefreshCache();//Put the preference cache back the way it was.
			Assert.IsFalse(hadException);//No exception should have been thrown because these duplicate preferences are not native.
		}

		************************************************************************************************************************************************/

	}
}
