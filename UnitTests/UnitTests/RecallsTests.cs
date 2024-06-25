using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class RecallsTests:TestBase {
		
		private static RecallType _recallType;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			_recallType=RecallTypeT.CreateRecallType();
		}

		[TestInitialize]
		public void SetupTest() {
			RecallT.ClearRecallTable();			
		}

		[ClassCleanup]
		public static void TearDownClass() {

		}

		///<summary>Ensure the temprecallmaxdate table created by Recalls.GetAddressTableRaw() doesn't suffer from concurrency MySql errors.</summary>
		[TestMethod]
		public void Recalls_GetAddressTableRaw_Concurrency() {
			List<long> listRecallNums=Enumerable.Range(0,100)//Create 100
				.Select(x => PatientT.CreatePatient())//patients
				.Select(x => RecallT.CreateRecall(x.PatNum,_recallType.RecallTypeNum,DateTime.Today.AddDays(-7),_recallType.DefaultInterval).RecallNum)//recalls
				.ToList();
			List<Action> listActions=Enumerable.Range(0,100).Select(x => new Action(() => Recalls.GetAddrTableRaw(listRecallNums))).ToList();
			ODThread.RunParallel(listActions,onException: new ODThread.ExceptionDelegate(ex => Assert.Fail(MiscUtils.GetExceptionText(ex))));
			string command="SHOW TABLES LIKE 'temprecallmaxdate%'";
			DataTable table=DataCore.GetTable(command);
			Assert.AreEqual(0,table.Rows.Count);
		}

		///<summary>MaxAutoReminders greater than reminder count.</summary>
		[TestMethod]
		public void Recalls_HasTooManyReminders_MaxAutoRemindersMoreThanCount() {
			PrefT.UpdateInt(PrefName.RecallShowIfDaysFirstReminder,1);
			PrefT.UpdateInt(PrefName.RecallShowIfDaysSecondReminder,2);
			bool result=Recalls.HasTooManyReminders(numberOfReminders:6,DateTime.Today.AddDays(-10),maxReminders:8);
			Assert.AreEqual(false,result);
		}

		///<summary>MaxAutoReminders less than reminder count.</summary>
		[TestMethod]
		public void Recalls_HasTooManyReminders_MaxAutoRemindersLessThanCount() {
			PrefT.UpdateInt(PrefName.RecallShowIfDaysFirstReminder,1);
			PrefT.UpdateInt(PrefName.RecallShowIfDaysSecondReminder,2);
			bool result=Recalls.HasTooManyReminders(numberOfReminders:6,DateTime.Today.AddDays(-10),maxReminders:4);
			Assert.AreEqual(true,result);
		}

		///<summary>When a patient has two insurance plans, they should only have one entry in the Recall List for each recall appointment type they have.</summary>
		[TestMethod]
		public void Recalls_MultipleInsSubsAndRecallTypes() {
			//Set up a patient with two insurance plans.
			Patient patient=PatientT.CreatePatient();
			string nameInsPri="ABC Health";
			string nameInsSec="DEF Medical";
			InsuranceInfo insInfoAbc=InsuranceT.AddInsurance(patient,nameInsPri);
			InsuranceInfo insInfoDef=InsuranceT.AddInsurance(patient,nameInsSec);
			//Create two unique trigger procedures for recall.
			ProcedureCodeT.CreateProcCode("T1234");
			ProcedureCodeT.CreateProcCode("T5678");
			RecallType recallTypeObs=RecallTypeT.CreateRecallType("Observing","T1234");
			RecallType recallTypeExa=RecallTypeT.CreateRecallType("Examining","T5678",defaultInterval:new Interval(0,1,0,0));
			//Create recalls as though the patient had completed the recall-triggering procedures.
			RecallT.CreateRecall(patient.PatNum,recallTypeObs.RecallTypeNum,DateTime_.Now.AddDays(7),new Interval(0,1,0,0));
			RecallT.CreateRecall(patient.PatNum,recallTypeExa.RecallTypeNum,DateTime_.Now.AddDays(7),new Interval(0,1,0,0));
			DataTable table=Recalls.GetRecallList(DateTime_.Now.AddDays(-14),DateTime_.Now.AddDays(14),false,0,0,0,
				RecallListSort.Alphabetical,RecallListShowNumberReminders.All,0,listRecallTypes:RecallTypes.GetDeepCopy());
			//Find the recall rows unique to the patient.
			List<DataRow> listRows=table.Rows.OfType<DataRow>().ToList().FindAll(x=>PIn.Long((string)x["PatNum"])==patient.PatNum);
			//Confirm there's only two.
			Assert.AreEqual(2,listRows.Count);
			//Confirm they're different appointments.
			List<long> listRecallNums=listRows.Select(x=>PIn.Long((string)x["RecallNum"])).Distinct().ToList();
			Assert.AreEqual(2,listRecallNums.Count);
			//Confirm that both insurance carriers' names will show in the grid.
			for(int i=0;i<listRows.Count;i++) {
				string carrierNamesConcatenated=(string)listRows[i]["CarrierName"];
				Assert.IsTrue(carrierNamesConcatenated.Contains(nameInsPri));
				Assert.IsTrue(carrierNamesConcatenated.Contains(nameInsSec));
			}
		}
	}
}
