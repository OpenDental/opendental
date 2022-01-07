using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Bridges_Tests {
	[TestClass]
	public class BridgesTests:TestBase {

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

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void Sirona_IntToByteArray_ArithmeticOverflow() {
			//The old Sirona Sidexis bridge worked for many years due to not needing to write lines longer than 128 characters to the .sdx file.
			//An arithmetic OverflowException would throw if there were more than 128 characters in a single line (e.g. patients with long names).
			//This unit test asserts that the new code that replaced the old code works exactly the same up to the point of failure.
			//The most interesting part about this unit test / bug fix is that IntToByteArrayOld works with integers larger than 128 but Open Dental fails.
			//There must be something else happening in the core of Open Dental that causes Math.IEEERemainder() to act differently than this Unit Test.
			//Regardless, the new way of converting an integer to a byte array by using BitConverter.GetBytes() works in both projects.
			for(int i=0;i<126;i++) {//Only need to assert up to 126 because i will always have 2 added to it (thus max value tested will be 128).
				//The Sirona bridge always adds 2 to the length of the text being passed in
				byte[] arrayExpectedBytes=IntToByteArrayOld(i+2);
				byte[] arrayActualBytes=IntToByteArrayNew(i+2);
				Assert.AreEqual(arrayExpectedBytes[0],arrayActualBytes[0]);
				Assert.AreEqual(arrayExpectedBytes[1],arrayActualBytes[1]);
			}
		}

		///<summary>This was the code that was being used to come up with the first two bytes of the Sidexis .sdx file.</summary>
		private static byte[] IntToByteArrayOld(int toConvert) {
			byte[] retVal=new byte[2];
			retVal[0]=(byte)Math.IEEERemainder(toConvert,256);
			retVal[1]=(byte)(toConvert/256);//rounds down automatically
			return retVal;
		}

		///<summary>This is the equivalent code that replaced IntToByteArrayOld()</summary>
		private static byte[] IntToByteArrayNew(int toConvert) {
			byte[] retVal=new byte[2];
			byte[] arrayBytes=BitConverter.GetBytes(toConvert);
			//Only write the first two bytes to preserve old behavior.
			retVal[0]=arrayBytes[0];
			retVal[1]=arrayBytes[1];
			return retVal;
		}

	}
}
