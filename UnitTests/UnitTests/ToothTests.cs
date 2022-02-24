using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Tooth_Tests {
	[TestClass]
	public class ToothTests:TestBase {
		///<summary></summary>
		[TestMethod]
		public void FormatRangesForDisplay() {
			string inputrange="1,2,3,4,5,7,8,9,11,12,15,16,17,18,21,22,23";
			string result=Tooth.FormatRangeForDisplay(inputrange);
			string desired="1-5,7-9,11,12,15,16,17,18,21-23";
			Assert.AreEqual(result,desired);
			//
			inputrange="2,4,5,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,25";
			result=Tooth.FormatRangeForDisplay(inputrange);
			desired="2,4,5,7-9,11-16,17-23,25";
			Assert.AreEqual(result,desired);
			//spaces
			inputrange="4,5,2, 7,8,9,11 ,13,12,14,15,16,17,18 ,20, 21,22,23,19,25";
			result=Tooth.FormatRangeForDisplay(inputrange);
			desired="2,4,5,7-9,11-16,17-23,25";
			Assert.AreEqual(result,desired);
			//primary
			inputrange="2,4,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,25,A,B,C,S";
			result=Tooth.FormatRangeForDisplay(inputrange);
			desired="2,4,7-9,11-16,17-23,25,A-C,S";
			Assert.AreEqual(result,desired);
		}

		///<summary></summary>
		[TestMethod]
		public void FormatRangesForDb() {
			string inputrange="1-5,7-9,11,12,15,16,17,18,21-23";
			string result=Tooth.FormatRangeForDb(inputrange);
			string desired="1,2,3,4,5,7,8,9,11,12,15,16,17,18,21,22,23";
			Assert.AreEqual(result,desired);
			//
			inputrange="2,4,5,7-9,11-16,17-23,25";
			result=Tooth.FormatRangeForDb(inputrange);
			desired="2,4,5,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,25";
			Assert.AreEqual(result,desired);
			//spaces
			inputrange="4,2,5,7-9 ,11-16,25 ,  17-23";
			result=Tooth.FormatRangeForDb(inputrange);
			desired="2,4,5,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,25";
			Assert.AreEqual(result,desired);
			//primary teeth
			inputrange="4,S,2,A,B,5,7-9,11-16,25,17-23";
			result=Tooth.FormatRangeForDb(inputrange);
			desired="2,4,5,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,25,A,B,S";
			Assert.AreEqual(result,desired);
			//primary teeth, lower case
			inputrange="4,s,2,a,b,5,7-9,11-16,25,17-23";
			result=Tooth.FormatRangeForDb(inputrange);
			desired="2,4,5,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,25,A,B,S";
			Assert.AreEqual(result,desired);
			//junk
			inputrange="1,junk";
			try{
				result=Tooth.FormatRangeForDb(inputrange);
				Assert.Fail();
			}
			catch{
				Assert.IsTrue(true);
			}
		}


	}
}
