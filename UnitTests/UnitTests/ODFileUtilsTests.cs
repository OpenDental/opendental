using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using OpenDental;
using CodeBase;

namespace UnitTests.ODFileUtils_Tests {
	[TestClass]
	public class ODFileUtilsTests : TestBase {
		[TestMethod]
		public void ODFileUtils_FilePath_SingleFolder () {
			string testString=@"This is a test to find the file path C:/ ";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
			Assert.AreEqual("C:/",returnedString[0]);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_SingleFile () {
			string testString=@"This is a test to find the file path up to the parent folder for D:/Test.txt ";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
			//Two slashes because FileInfo.Directory returns 2 and rearranges slashes to go correct way
			Assert.AreEqual("D:\\",returnedString[0]);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_UNCPath () {
			string testString=@"This is a test to find the UNC path \\opendental.od\serverfiles\ ";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
			Assert.AreEqual(@"\\opendental.od\serverfiles",returnedString[0]);
		}

		[TestMethod]
		//Dont find folders with no slash at the end. No way to know if end of file path or not.
		public void ODFileUtils_FilePath_UNCPath_Without_Ending_Slash() {
			string testString=@"This is a test to find the UNC path \\opendental.od\serverfiles";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(0,returnedString.Count);
		}

		[TestMethod]
		//Dont find folders with no white space at the end. No way to know if end of file path or not.
		public void ODFileUtils_FilePath_UNCPath_Without_Ending_Space() {
			string testString=@"This is a test to find the UNC path \\opendental.od\serverfiles\";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_UNCPath_Multiple_One_Line() {
			string testString=@"This is a test to find the UNC path \\opendental.od\serverfiles\ and c:/Test.txt ";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(2,returnedString.Count);
			//Two slashes because FileInfo.Directory returns 2
			Assert.AreEqual("\\\\opendental.od\\serverfiles",returnedString[0]);
			//Rearranges slashes to go correct way
			Assert.AreEqual("c:\\",returnedString[1]);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_FilePath_Backslash() {
			string testString=@"This is a test to find the file path with back slashes C:\Test.txt ";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
			//Two slashes because FileInfo.Directory returns this way
			Assert.AreEqual("C:\\",returnedString[0]);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_InternetURL () {
			string testString=@"This is a test to not find the url https://google.com ";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(0,returnedString.Count);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_NewLines() {
			string testString=@"This is a big block od text 
			with new lines to show new lines can also be white space
			\\opendental.od\serverfiles\
			";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
			//Two slashes because FileInfo.Directory returns 2
			Assert.AreEqual("\\\\opendental.od\\serverfiles",returnedString[0]);
		}

		[TestMethod]
		public void ODFileUtils_FilePath_EndsWithPeriod() {
			//File that ends in a period. Don't pick up the extra period at the end.
			string testString=@"\\Testing period\test.txt.";
			List<string> returnedString=ODFileUtils.GetFilePathsFromText(testString);
			Assert.AreEqual(1,returnedString.Count);
			//Two slashes because FileInfo.Directory returns 2. Regex.
			Assert.AreEqual("\\\\Testing period\\test.txt",returnedString[0]);
		}
	}
}
