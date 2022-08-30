using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataConnectionBase;

namespace UnitTests.ODInstaller_Tests {
	[TestClass]
	public class ODInstallerTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		[TestMethod]
		public void ODInstallerTests_DbAdminMysql_CreateAndDestroyAdminUsers() {
			DataConnection conAdmin=new DataConnection();//A brand new admin connection to the unittest### database.
			Assert.AreEqual(null,DbAdminMysql.ModifyUser(conAdmin,"fakeuser1","od123","fakeuser1"));//Grants, verifies new user.
			Assert.AreEqual(null,DbAdminMysql.ModifyUser(conAdmin,"fakeuser2","abcde","fakeuser1"));//Drops fakeuser1 and verifies.
			try {
				DbAdminMysql.DropUser(conAdmin,"fakeuser2");//Drops fakeuser2 and verifies.
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
		}

	}
}
