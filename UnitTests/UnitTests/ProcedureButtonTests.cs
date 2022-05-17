using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ProcedureButtons_Tests {
	[TestClass]
	public class ProcedureButtonTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			string command = "DELETE FROM procbuttonquick";
			DataCore.NonQ(command);
		}

		[TestInitialize]
		public void SetupTest() {
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		/// <summary>Run the code to set the ProcButtonQuick to the default for US offices using D codes, then uses the HasCustomQuickButtons method to see if there are just T codes left.</summary>
		[TestMethod]
		public void ProcedureCodes_InsertDefaultQuickButtonsUS() {
			ProcButtonQuicks.SetToDefaultMySQL();
			bool testBool = true;
				string command = "SELECT CodeValue FROM procbuttonquick WHERE IsLabel = 0";
				List<string> listQuickButtonCodes = DataCore.GetList<string>(command,(x) => x.ToString());
				if(listQuickButtonCodes.Count > 0) {
					foreach(string code in listQuickButtonCodes) {
						if(code.StartsWith("T")) {
							testBool=false;
							break;
						}
					}
				}
			Assert.IsTrue(testBool);
		}

	}
}
