using System.Collections.Generic;
using System.Linq;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;

namespace UnitTests.Programs_Tests {
	[TestClass]
	public class ProgramsTests:TestBase {
		private static IWebServiceMainHQ _webInstanceOld;
		private static List<OpenDentBusiness.Program> _listTestProgs=new List<OpenDentBusiness.Program>();
		private static List<HqProgram> _listHqProgsForBiz=new List<HqProgram>();

		private static void ResetTestProgValues() {
			foreach(OpenDentBusiness.Program prog in _listTestProgs) {
				prog.IsDisabledByHq=false;
			}
		}

		[ClassInitialize]
		public static void SetUpTest(TestContext testContext) {
			_listTestProgs=Programs.GetWhere(x=> x.ProgName.In(ProgramName.PDMP.ToString(),ProgramName.DentalIntel.ToString(),ProgramName.Xcharge.ToString()));
			ResetTestProgValues();
			_webInstanceOld=WebServiceMainHQProxy.GetWebServiceMainHQInstance();
			WebServiceMainHQProxy.MockWebServiceMainHQ=new WebServiceMainHQMockDemo() {
				EnableAdditionalFeaturesDelegate=(officeData) => {
					_listHqProgsForBiz.Clear(); 
					foreach(OpenDentBusiness.Program prog in _listTestProgs) {
						HqProgram newHq=new HqProgram() {
							ProgramNameAsString=prog.ProgName,
							IsEnabled=false,
						};
						_listHqProgsForBiz.Add(newHq);
					}
					return PayloadHelper.CreateSuccessResponse(new List<PayloadItem> {
						new PayloadItem(_listHqProgsForBiz,"ListHqPrograms"),
						new PayloadItem((long)System.TimeSpan.FromHours(12).TotalHours,"IntervalHours"),
					});
				}
			};
		}

		[ClassCleanup]
		public static void ClassTearDown() {
			_webInstanceOld=null;
		}

		[TestCleanup]
		public void TearDownTest() {
			ResetTestProgValues();
		}

		[TestMethod]
		public void Programs_IsDisabledAtHQUpdateProgramsInDatabase_HappyPath() {
			List<OpenDentBusiness.Program> listProgsBeforeDownload=Programs.GetWhere(x=> _listTestProgs.Contains(x));
			foreach(OpenDentBusiness.Program program in listProgsBeforeDownload) {
				Assert.IsFalse(program.IsDisabledByHq);//make sure all programs are currently allowed. Whether they are enabled by the office is irrelevant
			}
			HqProgram.Download();//Pretend this was called by the service which it will be in proper
			List<OpenDentBusiness.Program> listProgsAfterDownload=Programs.GetWhere(x=> _listTestProgs.Contains(x));
			foreach(OpenDentBusiness.Program program in listProgsAfterDownload) {
				Assert.IsTrue(program.IsDisabledByHq);
			}
		}

		[TestMethod]
		public void Programs_IsDisabledAtHQ_HqDoesNotCare() {
			//Making a list of three programs managed by HQ and one that is not
			List<OpenDentBusiness.Program> listProgsForTest=_listTestProgs;
			listProgsForTest.Add(Programs.GetCur(ProgramName.eClinicalWorks));
			List<OpenDentBusiness.Program> listProgsBeforeDownload=Programs.GetWhere(x=> listProgsForTest.Contains(x));
			//Verifying each one is not disabled by HQ
			foreach(OpenDentBusiness.Program program in listProgsBeforeDownload) {
				Assert.IsFalse(program.IsDisabledByHq);
			}
			HqProgram.Download();
			List<OpenDentBusiness.Program> listProgsAfterDownload=Programs.GetWhere(x=> listProgsForTest.Contains(x));
			foreach(OpenDentBusiness.Program program in listProgsAfterDownload) {
				if(_listTestProgs.Contains(program)) {
					Assert.IsTrue(program.IsDisabledByHq);//make sure that only programs managed at Hq have had their values changed
				}
				else {
					Assert.IsFalse(program.IsDisabledByHq);
				}
			}
		}
	}
}
