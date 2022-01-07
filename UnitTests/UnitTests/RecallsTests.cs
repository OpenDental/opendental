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

	}
}
