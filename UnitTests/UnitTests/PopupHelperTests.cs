using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness.UI;
using UnitTestsCore;

namespace UnitTests.PopupHelper_Tests {
	[TestClass]
	public class PopupHelperTests:TestBase {

		///<summary>Tests the GetURLs method that finds URLs within a string. It supports URLs within parenthesis. This test runs through many cases
		///and ensures the regular expression correctly finds the URL.</summary>
		[TestMethod]
		public void PopupHelper_GetURLsFromText() {
			string testString=@"
				(http://google.com/?NotWorthTheEffort)
				[http://google.com/SendHelp]
				(http://google.com/rergg)regsjgiesrjgrio). 
				(http://google.com/thisisavjwpoegk20-5352nerror)rgerg is the one you want to click on).
				http://google.com/validUrlsAreDumb)
				(google.com/What/is/regex/)
				(https://opendental.com/manual/sheetscheckbox.html).
			";
			List<string> listUrls=PopupHelper.GetURLsFromText(testString);
			Assert.AreEqual(7,listUrls.Count);
			//All of the URLs below are what we would expect the regular expression to grab.
			Assert.IsTrue(listUrls.Contains(@"http://google.com/?NotWorthTheEffort"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/SendHelp"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/rergg)regsjgiesrjgrio"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/thisisavjwpoegk20-5352nerror)rgerg"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/validUrlsAreDumb)"));
			Assert.IsTrue(listUrls.Contains(@"google.com/What/is/regex/"));
			Assert.IsTrue(listUrls.Contains(@"https://opendental.com/manual/sheetscheckbox.html"));
		}

	}
}
