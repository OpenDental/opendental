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

namespace UnitTests.DateTimeOD_Tests {
	[TestClass]
	public class DateTimeODTests:TestBase {
		private static CultureInfo cultureOrig;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			cultureOrig=Application.CurrentCulture;
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			Application.CurrentCulture=cultureOrig;
			CultureInfo.DefaultThreadCurrentCulture=Application.CurrentCulture;
			CultureInfo.DefaultThreadCurrentUICulture=Application.CurrentCulture;
		}

		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void DateTimeOD_NormalizeApplicationShortDateFormat_AllCultures() {
			bool isTwoDigitYearTested = false;
			string strYearFormat = "yyyy";
			foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
				Application.CurrentCulture=culture;
				CultureInfo.DefaultThreadCurrentCulture=Application.CurrentCulture;
				CultureInfo.DefaultThreadCurrentUICulture=Application.CurrentCulture;
				if(!isTwoDigitYearTested && (Regex.IsMatch(Application.CurrentCulture.DateTimeFormat.ShortDatePattern,"^y{2}[^y]")
					|| Regex.IsMatch(Application.CurrentCulture.DateTimeFormat.ShortDatePattern,"[^y]y{2}[^y]")
					|| Regex.IsMatch(Application.CurrentCulture.DateTimeFormat.ShortDatePattern,"[^y]y{2}$"))) {
					isTwoDigitYearTested=true;
				}
				DateTimeOD.NormalizeApplicationShortDateFormat();
				Assert.IsTrue(Application.CurrentCulture.DateTimeFormat.ShortDatePattern.Contains(strYearFormat));
				Assert.IsTrue(CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.ShortDatePattern.Contains(strYearFormat));
				Assert.IsTrue(CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat.ShortDatePattern.Contains(strYearFormat));
			}
			Assert.IsTrue(isTwoDigitYearTested);
		}

	}
}
