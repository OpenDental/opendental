using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness.UI;
using UnitTestsCore;
using WpfControls.UI;

namespace UnitTests.PopupHelper_Tests {
	[TestClass]
	public class PopupHelperTests:TestBase {
		#region PopupHelper
		///<summary>Tests the GetURLs method that finds URLs within a string. It supports URLs within parenthesis. This test runs through many cases and ensures the regular expression correctly finds the URL.</summary>
		[TestMethod]
		public void PopupHelper_TestUrlsWithParentheses() {
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
			Assert.IsTrue(listUrls.Contains(@"http://google.com/validUrlsAreDumb"));
			Assert.IsTrue(listUrls.Contains(@"google.com/What/is/regex"));
			Assert.IsTrue(listUrls.Contains(@"https://opendental.com/manual/sheetscheckbox.html"));
		}

		///<summary>Tests to ensure no email addresses get recognized as urls</summary>
		[TestMethod]
		public void PopupHelper_TestEmailAddresses() {
			string testString=@"Joe@yahoo.com,
			Smurf@hotmail.com,
			ExampleNameL@opendental.com,
			youremailaddress@educationplace.net";
			List<string> listUrls=PopupHelper.GetURLsFromText(testString);
			Assert.AreEqual(0,listUrls.Count);
		}

		///<summary>Tests various general websites or website examples</summary>
		[TestMethod]
		public void PopupHelper_TestWebsiteUrls() {
			string testString=@"https://www.opendental.com/manual/graphicaltoothchart.html,
			https://www.opendental.com/index.html,
			https://opendental.com/manual/searchmanual.html,
			https://www.opendental.com/OpenDentalDocumentation24-1.xml,
			https://opendentalsoft.com:1943/ODBugTracker/PreviousVersions.aspx,
			google.com,
			asdf.net,
			abc123.gov/directory
			";
			List<string> listUrls=PopupHelper.GetURLsFromText(testString);
			Assert.AreEqual(8,listUrls.Count);
		}

		///<summary>Tests to make sure that files are properly sorted out</summary>
		[TestMethod]
		public void PopupHelper_TestFiles() {
			string testString=@"C:\Development\NoDpi.txt,
			C:\Development\NoCustomBorders.txt,
			C:\Users\traskm\Desktop\Screenshot 2024-04-17 122431.png,
			V:\Yeah\Bro\lol.txt,
			regex.zip,
			dbcopy.bat,
			brainpain.png,
			one.png,
			test_example_pdf.pdf,
			lol.DCM,
			FrmProperties.xlsx";
			List<string> listUrls=PopupHelper.GetURLsFromText(testString);
			Assert.AreEqual(0,listUrls.Count);
		}

		///<summary>Tests various websites that point to files at the end of the link.</summary>
		[TestMethod]
		public void PopupHelper_TestWeblinksThatLookLikeFiles() {
			string testString=@"https://www.opendental.com/resources/Setup%20Checklist.pdf,
			https://vynedental.com/wp-content/uploads/2021/07/Tesia-Dental-Payer-Listing-Website-v07.01.21.pdf,
			www.MyRealWebsite.com/yeah/totallyrealfile.png,
			https://thisismywebsite.com/mypdf.pdf";
			List<string> listUrls=PopupHelper.GetURLsFromText(testString);
			Assert.AreEqual(4, listUrls.Count);
		}
		#endregion

		#region PopupHelper2
				///<summary>Tests the GetURLs method that finds URLs within a string. It supports URLs within parenthesis. This test runs through many cases and ensures the regular expression correctly finds the URL.</summary>
		[TestMethod]
		public void PopupHelper2_TestUrlsWithParentheses() {
			string testString=@"
				(http://google.com/?NotWorthTheEffort)
				[http://google.com/SendHelp]
				(http://google.com/rergg)regsjgiesrjgrio). 
				(http://google.com/thisisavjwpoegk20-5352nerror)rgerg is the one you want to click on).
				http://google.com/validUrlsAreDumb)
				(google.com/What/is/regex/)
				(https://opendental.com/manual/sheetscheckbox.html).
			";
			List<string> listUrls=PopupHelper2.GetURLsFromText(testString);
			Assert.AreEqual(7,listUrls.Count);
			//All of the URLs below are what we would expect the regular expression to grab.
			Assert.IsTrue(listUrls.Contains(@"http://google.com/?NotWorthTheEffort"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/SendHelp"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/rergg)regsjgiesrjgrio"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/thisisavjwpoegk20-5352nerror)rgerg"));
			Assert.IsTrue(listUrls.Contains(@"http://google.com/validUrlsAreDumb"));
			Assert.IsTrue(listUrls.Contains(@"google.com/What/is/regex"));
			Assert.IsTrue(listUrls.Contains(@"https://opendental.com/manual/sheetscheckbox.html"));
		}

		///<summary>Tests to ensure no email addresses get recognized as urls</summary>
		[TestMethod]
		public void PopupHelper2_TestEmailAddresses() {
			string testString=@"Joe@yahoo.com,
			Smurf@hotmail.com,
			ExampleNameL@opendental.com,
			youremailaddress@educationplace.net";
			List<string> listUrls=PopupHelper2.GetURLsFromText(testString);
			Assert.AreEqual(0,listUrls.Count);
		}

		///<summary>Tests various general websites or website examples</summary>
		[TestMethod]
		public void PopupHelper2_TestWebsiteUrls() {
			string testString=@"https://www.opendental.com/manual/graphicaltoothchart.html,
			https://www.opendental.com/index.html,
			https://opendental.com/manual/searchmanual.html,
			https://www.opendental.com/OpenDentalDocumentation24-1.xml,
			https://opendentalsoft.com:1943/ODBugTracker/PreviousVersions.aspx,
			google.com,
			asdf.net,
			abc123.gov/directory
			";
			List<string> listUrls=PopupHelper2.GetURLsFromText(testString);
			Assert.AreEqual(8,listUrls.Count);
		}

		///<summary>Tests to make sure that files are properly sorted out</summary>
		[TestMethod]
		public void PopupHelper2_TestFiles() {
			string testString=@"C:\Development\NoDpi.txt,
			C:\Development\NoCustomBorders.txt,
			C:\Users\traskm\Desktop\Screenshot 2024-04-17 122431.png,
			V:\Yeah\Bro\lol.txt,
			regex.zip,
			dbcopy.bat,
			brainpain.png,
			one.png,
			test_example_pdf.pdf,
			lol.DCM,
			FrmProperties.xlsx";
			List<string> listUrls=PopupHelper2.GetURLsFromText(testString);
			Assert.AreEqual(0,listUrls.Count);
		}

		///<summary>Tests various websites that point to files at the end of the link.</summary>
		[TestMethod]
		public void PopupHelper2_TestWeblinksThatLookLikeFiles() {
			string testString=@"https://www.opendental.com/resources/Setup%20Checklist.pdf,
			https://vynedental.com/wp-content/uploads/2021/07/Tesia-Dental-Payer-Listing-Website-v07.01.21.pdf,
			www.MyRealWebsite.com/yeah/totallyrealfile.png,
			https://thisismywebsite.com/mypdf.pdf";
			List<string> listUrls=PopupHelper2.GetURLsFromText(testString);
			Assert.AreEqual(4, listUrls.Count);
		}
		#endregion

		/*
		///<summary></summary>
		[TestMethod]
		public void PopupHelper_() {
			string testString=@"";
			List<string> listUrls=PopupHelper.GetURLsFromText(testString);
			Assert.AreEqual();
		}
		*/

	}
}