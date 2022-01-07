using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.FriendlyException_Tests {
	[TestClass]
	public class FriendlyExceptionTests:TestBase {
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
		public void FriendlyException_Show_ThrowsInUnitTest() {
			string strMessageExpected="Exception thrown.";
			string strMessageCaught="FormFriendlyException is not throwing.";
			//Call FriendlyException in a thread so we don't hang in this test if it fails.
			ODThread odThread=new ODThread((o) => {
				try {
					FriendlyException.Show("FriendlyException",new Exception(strMessageExpected));
				}
				catch(Exception ex) {
					strMessageCaught=ex.InnerException.Message;
				}
			});
			odThread.Start();
			int millis=0;
			while(strMessageExpected!=strMessageCaught && millis<1000) {
				System.Threading.Thread.Sleep(1);//sleep 1 milliseconds.
				millis++;
			}
			Assert.AreEqual(strMessageExpected,strMessageCaught);
		}

	}
}
