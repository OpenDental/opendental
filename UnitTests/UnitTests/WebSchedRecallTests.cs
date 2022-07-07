using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System.Collections.Generic;
using UnitTestsCore;

namespace UnitTests.UnitTests {
	[TestClass]
	public class WebSchedRecallTests:TestBase {
		static RecallType _recallType;

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			RecallTypeT.ClearRecallTypeTable();
			_recallType=RecallTypeT.CreateRecallType();
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			WebSchedRecallT.ClearWebSchedRecallTable();
			RecallT.ClearRecallTable();
			PatientT.ClearPatientTable();
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
			_recallType=null;
		}

		[TestMethod]
		public void WebSchedRecalls_InsertFromRecallList_AllOneOfEachCommType() {
			Patient pat=PatientT.CreatePatient();
			Recall recall=RecallT.CreateRecall(pat.PatNum,_recallType.RecallTypeNum,DateTime_.Now.AddDays(-1),new Interval(7,0,0,0));
			List<long> recallNums=new List<long>() { recall.RecallNum };
			WebSchedRecalls.InsertForRecallNums(recallNums,false,RecallListSort.DueDate,WebSchedRecallSource.FormRecallList,CommType.Email,DateTime_.Now);
			WebSchedRecalls.InsertForRecallNums(recallNums,false,RecallListSort.DueDate,WebSchedRecallSource.FormRecallList,CommType.Text,DateTime_.Now);
			List<WebSchedRecall> listWebSchedRecalls=WebSchedRecalls.GetAllUnsent();
			Assert.AreEqual(2,listWebSchedRecalls.Count);
		}
		
		[TestMethod]
		public void WebSchedRecalls_InsertFromRecallList_DontAllowSameCommType() {
			Patient pat=PatientT.CreatePatient();
			Recall recall=RecallT.CreateRecall(pat.PatNum,_recallType.RecallTypeNum,DateTime_.Now.AddDays(-1),new Interval(7,0,0,0));
			List<long> recallNums=new List<long>() { recall.RecallNum };
			for(int i=0;i<2;i++){
				WebSchedRecalls.InsertForRecallNums(recallNums,false,RecallListSort.DueDate,WebSchedRecallSource.FormRecallList,CommType.Email,DateTime_.Now);
			}
			List<WebSchedRecall> listWebSchedRecalls=WebSchedRecalls.GetAllUnsent();
			Assert.AreEqual(1,listWebSchedRecalls.Count);
		}
	}
}
