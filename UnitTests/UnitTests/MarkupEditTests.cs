using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.MarkupEdit_Tests {
	[TestClass]
	public class MarkupEditTests:TestBase {
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
		public void MarkupEdit_ConvertToPlainText_HTML() {
			string htmltags="<h2>Ideas</h2> <h3>Common Passwords file</h3>A file would be &>included that would contain</br> the top 1000 or so";
			string htmlResult="Ideas Common Passwords fileA file would be &>included that would contain the top 1000 or so";
			Assert.AreEqual(htmlResult,MarkupEdit.ConvertToPlainText(htmltags));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_HtmlWebChatEndSessionMessage() {
			string htmltags="<b>The session has ended.  You can close this browser window.</b><br><br>You can save this conversation for future reference."
				+"  <a href=\"chathistory.aspx\">Save the chat history</a>.";
			string htmlResult="The session has ended.  You can close this browser window.You can save this conversation for future reference.  Save the chat history.";
			Assert.AreEqual(htmlResult,MarkupEdit.ConvertToPlainText(htmltags));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_PageLinks() {
			string PageLinks="This is a title: [[234]] that shouldn't be [There]";
			string pageLinkResult="This is a title:  that shouldn't be [There]";
			Assert.AreEqual(pageLinkResult,MarkupEdit.ConvertToPlainText(PageLinks));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODMarkupImage() {
			string markupString="[[[img:This]] : test]].";
			string markupStringResult="[This : test]].";
			Assert.AreEqual(markupStringResult,MarkupEdit.ConvertMarkupToPlainText(markupString));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODMarkupFont() {
			string markupString="[ [[font:timesnewroman|\r\nemail]] |: test]].";
			string markupStringResult="[ \r\nemail |: test]].";
			Assert.AreEqual(markupStringResult,MarkupEdit.ConvertMarkupToPlainText(markupString));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODMarkupColor() {
			string markupString="[| [[color:red|red\r\nred]] |: ]].";
			string markupStringResult="[| red\r\nred |: ]].";
			Assert.AreEqual(markupStringResult,MarkupEdit.ConvertMarkupToPlainText(markupString));
		}

		[TestMethod]
		public void MarkupEdit_ConvertToPlainText_ODTable() {
			string markupTable="This email contains HTML.\n{|\n!Width=\"100\"|Heading1!!Width=\"100\"|Heading2!!Width=\"100\"|Heading3\n|-\n|one||two||three\n|}";
			string markupTableResult="This email contains HTML.\n\n Heading1 Heading2 Heading3 \n one two three\n";
			Assert.AreEqual(markupTableResult,MarkupEdit.ConvertMarkupToPlainText(markupTable));
		}
	}
}
