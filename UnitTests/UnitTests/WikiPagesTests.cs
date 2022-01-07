using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.WikiPages_Tests {
	[TestClass]
	public class WikiPagesTests:TestBase {
		[TestMethod]
		public void WikiPages_IsWikiPageTitleValid_HashMark() {
			string titleHash,container;
			titleHash="Title with an # mark";
			container="";
			Assert.IsFalse(WikiPages.IsWikiPageTitleValid(titleHash,out container));
		}

		[TestMethod]
		public void WikiPages_IsWikiPageTitleValid_Underline() {
			string titleUnderline,container;
			titleUnderline="_Title that contains an underline";
			Assert.IsFalse(WikiPages.IsWikiPageTitleValid(titleUnderline,out container));
		}

		[TestMethod]
		public void WikiPages_IsWikiPageTitleValid_Quotes() {
			string titleQuote,container;
			titleQuote="This is really only 1/2 a \"title\".";
			Assert.IsFalse(WikiPages.IsWikiPageTitleValid(titleQuote,out container));
		}

		[TestMethod]
		public void WikiPages_IsWikiPageTitleValid_MultipleLines() {
			string titleNewLine,container;
			titleNewLine="Two line \r\n title";
			container="";
			Assert.IsFalse(WikiPages.IsWikiPageTitleValid(titleNewLine,out container));
		}

		[TestMethod]
		public void WikiPages_IsWikiPageTitleValid() {
			string titleValid,container;
			titleValid="This is a valid title";
			container="";
			Assert.IsTrue(WikiPages.IsWikiPageTitleValid(titleValid,out container));
		}
	}
}
