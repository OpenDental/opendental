
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ApptViewItem_Tests {
	[TestClass]
	public class ApptViewItemTests:TestBase {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			ApptViewItemT.ClearApptViewItem();
			ApptViewItems.RefreshCache();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void ApptViewItem_GetViewsByOp_HappyPath() {
			ApptViewItemT.CreateApptViewItem(1,0);
			ApptViewItemT.CreateApptViewItem(1,0);
			ApptViewItemT.CreateApptViewItem(1,0);
			ApptViewItemT.CreateApptViewItem(2,0);
			ApptViewItems.RefreshCache();
			Assert.AreEqual(3,ApptViewItems.GetViewsByOp(1).Count);
			Assert.AreEqual(1,ApptViewItems.GetViewsByOp(2).Count);
		}
	}
}
