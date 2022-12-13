using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class POutTests:TestBase {


		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		[TestMethod]
		///<summary>Tests that POut.Double(double, int) behaves in the expected manner, rounding 5's up, throwing for negative numbers, and showing whole numbers for 0 decimal places.</summary>
		public void POutTests_DoubleWithIntegerInput() { 
			double inputValue=0.12345678912345678;
			//negative input shold throw exception
			try{
				POut.Double(inputValue,-1);
				Assert.Fail();
			}
			catch(Exception ex) { 		}
			//zero input should return a whole number
			Assert.AreEqual("0",POut.Double(inputValue,0));
			//greater than zero should return that number of decimal places. 
			Assert.AreEqual("0.123",POut.Double(inputValue,3));
			//big input- max places for double (16 reliably)
			Assert.AreEqual("0.123456789123457",POut.Double(inputValue,16));
			//rounding - to  nearest
			Assert.AreEqual("0.123457",POut.Double(inputValue,6));
			//5 rounds up
			Assert.AreEqual("0.1235",POut.Double(inputValue,4));
			Assert.AreEqual("1",POut.Double(.9,0));
		}

	}
}
