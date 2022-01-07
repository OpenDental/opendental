using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;


namespace UnitTests.DateSpanTests {
	[TestClass]
	public class DateSpanTests:TestBase {
		
		[TestMethod]
		public void DateSpan_YearsDiff_CorrectSpanForExactly14Years() {
			DateTime dateFrom=new DateTime(2006,6,28);
			DateTime dateTo=new DateTime(2020,6,28);
			DateSpan span=new DateSpan(dateFrom,dateTo);
			Assert.AreEqual(14,span.YearsDiff);
		}

		[TestMethod]
		public void DateSpan_YearsDiff_CorrectSpanForADayShortOf14Years() {
			DateTime dateFrom=new DateTime(2006,6,28);
			DateTime dateTo=new DateTime(2020,6,27);
			DateSpan span=new DateSpan(dateFrom,dateTo);
			Assert.AreEqual(13,span.YearsDiff);
		}

		[TestMethod]
		public void DateSpan_YearsDiff_CorrectSpanForADayPast14Years() {
			DateTime dateFrom=new DateTime(2006,6,28);
			DateTime dateTo=new DateTime(2020,6,29);
			DateSpan span=new DateSpan(dateFrom,dateTo);
			Assert.AreEqual(14,span.YearsDiff);
		}

		[TestMethod]
		public void DateSpan_YearsDiff_CorrectSpanForDecemberJanuary() {
			DateTime dateFrom=new DateTime(2006,1,1);
			DateTime dateTo=new DateTime(2020,12,28);
			DateSpan span=new DateSpan(dateFrom,dateTo);
			Assert.AreEqual(14,span.YearsDiff);
		}
	
	}
}
