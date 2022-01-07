using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.TsiTransLogs_Tests {
	[TestClass]
	public class TsiTransLogsTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			ProgramPropertyT.ClearProgamPropertyTable();
			OpenDentBusiness.Program prog=Programs.GetCur(ProgramName.Transworld);
			if(prog==null) {
				prog=new OpenDentBusiness.Program() {
					ProgName=ProgramName.Transworld.ToString(),
					ProgDesc=ProgramName.Transworld.ToString(),
					Enabled=true,
				};
				Programs.Insert(prog);
				Cache.Refresh(InvalidType.AllLocal);
			}
			ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,"SftpServerAddress",0,"server_address");
			ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,"SftpServerPort",0,"22");
			ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,"SftpUsername",0,"user_name");
			ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,"SftpPassword",0,"password");
			//0=TsiDemandType.Accelerator,1=TsiDemandType.ProfitRecovery,2=TsiDemandType.Collection
			ProgramPropertyT.CreateProgramProperty(prog.ProgramNum,"SelectedServices",0,"0,1,2");
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


		///<summary>Enables/Disables the Transworld program. Refreshes cache.</summary>
		private void EnableTransworldProgram(bool isEnabled=true) {
			OpenDentBusiness.Program prog=Programs.GetCur(ProgramName.Transworld);
			prog.Enabled=isEnabled;
			Programs.Update(prog);
			Cache.Refresh(InvalidType.AllLocal);
		}
		private void InsertTsiLog(long patNum,TsiTransType type) {
			TsiTransLog logCur=new TsiTransLog() {
				PatNum=patNum,
				UserNum=Security.CurUser.UserNum,
				TransType=type,
				//TransDateTime=DateTime.Now,//set on insert, not editable by user
				ServiceType=TsiServiceType.Accelerator,
				ServiceCode=TsiServiceCode.Diplomatic,
				ClientId="",
				TransAmt=0.00,
				AccountBalance=25,
				FKeyType=TsiFKeyType.None,//not used for placement messages
				FKey=0,//not used for placement messages
				RawMsgText="",
			};
			TsiTransLogs.Insert(logCur);
		}

		[TestMethod]
		public void TsiTransLogs_HasGuarBeenSentToTSI_PatientHasNotBeenSentToTSI() {
			//First, setup the test scenario.
			Patient pat=PatientT.CreatePatient();
			EnableTransworldProgram();
			//Next, perform the thing you're trying to test.
			bool hasBeenSent=TsiTransLogs.HasGuarBeenSentToTSI(pat);
			//Finally, use one or more asserts to verify the results.
			Assert.IsFalse(hasBeenSent);
		}

		[TestMethod]
		public void TsiTransLogs_HasGuarBeenSentToTSI_PatientHasBeenSentToTSI() {
			//First, setup the test scenario.
			Patient pat=PatientT.CreatePatient();
			EnableTransworldProgram();
			InsertTsiLog(pat.PatNum,TsiTransType.PL);
			//Next, perform the thing you're trying to test.
			bool hasBeenSent=TsiTransLogs.HasGuarBeenSentToTSI(pat);
			//Finally, use one or more asserts to verify the results.
			Assert.IsTrue(hasBeenSent);
		}

		[TestMethod]
		public void TsiTransLogs_HasGuarBeenSentToTSI_PatientHasBeenSentToTSIAndCanceled() {
			//First, setup the test scenario.
			Patient pat=PatientT.CreatePatient();
			EnableTransworldProgram();
			InsertTsiLog(pat.PatNum,TsiTransType.PL);
			InsertTsiLog(pat.PatNum,TsiTransType.CN);
			//Next, perform the thing you're trying to test.
			bool hasBeenSent=TsiTransLogs.HasGuarBeenSentToTSI(pat);
			//Finally, use one or more asserts to verify the results.
			Assert.IsFalse(hasBeenSent);
		}

		[TestMethod]
		public void TsiTransLogs_HasGuarBeenSentToTSI_PatientHasBeenSentToTSIAndPaidInFull() {
			//First, setup the test scenario.
			Patient pat=PatientT.CreatePatient();
			EnableTransworldProgram();
			InsertTsiLog(pat.PatNum,TsiTransType.PL);
			InsertTsiLog(pat.PatNum,TsiTransType.PT);
			//Next, perform the thing you're trying to test.
			bool hasBeenSent=TsiTransLogs.HasGuarBeenSentToTSI(pat);
			//Finally, use one or more asserts to verify the results.
			Assert.IsFalse(hasBeenSent);
		}

	}
}
