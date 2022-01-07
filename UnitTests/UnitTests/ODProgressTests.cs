using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeBase;

namespace UnitTests.ODProgress_Tests {
	[TestClass]
	public class ODProgressTests:TestBase {

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
		public void ODProgress_Show_RaceCondition() {
			//Show a progress window and then immediately tell it to close.  This is the quickest possible "long computation".
			Action actionClose=ODProgress.Show();
			actionClose();//Invoking this action right away was causing progress windows to stay "stuck open".
			Assert.IsTrue(true);//This unit test would never finish if a race condition was present.
		}

		[TestMethod]
		public void ODProgress_ShowAction_RaceCondition() {
			//Show a progress window and then immediately tell it to close.  This is the quickest possible "long computation".
			ODProgress.ShowAction(() => { });//This is the quickest possible "long computation".
			Assert.IsTrue(true);//This unit test would never finish if a race condition was present.
		}

	}
}
