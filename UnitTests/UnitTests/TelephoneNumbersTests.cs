using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.TelephoneNumbers_Tests {
	[TestClass]
	public class TelephoneNumbersTests:TestBase {
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
		public void TelephoneNumbers_IsNumberValidTenDigit_TenNumbers() {
			string phoneNumber="8003334444";
			Assert.IsTrue(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_OneAndTenNumbers() {
			string phoneNumber="18003334444";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_TenNumbersFormatted() {
			string phoneNumber="(800)333-4444";
			Assert.IsTrue(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_OneAndTenNumbersFormatted() {
			string phoneNumber="1(800)333-4444";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_TwelveNumbers() {
			string phoneNumber="800333444455";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_OneAndElevenNumbers() {
			string phoneNumber="180033344445";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_TwelveNumbersFormatted() {
			string phoneNumber="(800)333-444455";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_TwelveNumbersFormattedWithSpace() {
			string phoneNumber="(800)333-4444 55";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_OneAndTwelveNumbersFormatted() {
			string phoneNumber="1(800)333-444455";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

		[TestMethod]
		public void TelephoneNumbers_IsNumberValidTenDigit_OneAndTwelveNumbersFormattedWithSpace() {
			string phoneNumber="1(800)333-4444 55";
			Assert.IsFalse(TelephoneNumbers.IsNumberValidTenDigit(ref phoneNumber));
		}

	}
}
