using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class RpReceivablesBreakdownTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			AdjustmentT.ClearAdjustmentTable();
			ClaimT.ClearClaimTable();
			ClaimProcT.ClearClaimProcTable();
			PatientT.ClearPatientTable();
			PayPlanT.ClearPayPlanTable();
			PayPlanChargeT.ClearPayPlanChargeTable();
			ProcedureT.ClearProcedureTable();
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
		public void RpReceivablesBreakdown_GetRecvBreakdownTable_AgingProcLifoOff() {
			//This unit test asserts that adjustments do NOT inherit the ProcDate of the procedure they are associated with when AgingProcLifo is off.
			PrefT.UpdateYN(PrefName.AgingProcLifo,YN.No);
			string suffix=MethodInfo.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient patient=PatientT.CreatePatient(suffix,provNum);
			//Create a completed procedure on the 1st a few months ago.
			DateTime dateTimePast=DateTime.Now.AddMonths(-3);
			DateTime dateTimeProc=new DateTime(dateTimePast.Year,dateTimePast.Month,1);
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("APLOFF");
			Procedure procedure=ProcedureT.CreateProcedure(patient,procedureCode.ProcCode,ProcStat.C,"0",100,procDate:dateTimeProc,provNum:provNum);
			//Create an adjustment associated with the completed procedure that is dated a few days afterwards.
			DateTime dateTimeAdj=dateTimeProc.AddDays(10);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(patient.PatNum,-50,adjDate:dateTimeAdj,procDate:dateTimeProc,procNum:procedure.ProcNum,provNum:provNum);
			//Run the Receivables Breakdown report for a date that falls in-between the two objects.
			DateTime dateTimeStart=dateTimeProc.AddDays(5);
			DataTable table=GetRecvBreakdownTableAdj(dateTimeStart,provNum);
			Assert.AreEqual(0,table.Rows.Count);//Adjustment should not show up.
			//Now run the report for a day that includes both objects.
			table=GetRecvBreakdownTableAdj(dateTimeAdj.AddDays(1),provNum);
			Assert.AreEqual(1,table.Rows.Count);//Adjustment should show up.
			Assert.AreEqual(adjustment.AdjDate,PIn.DateT(table.Rows[0][0].ToString()));
			Assert.AreEqual(adjustment.AdjAmt,PIn.Double(table.Rows[0][1].ToString()));
		}

		[TestMethod]
		public void RpReceivablesBreakdown_GetRecvBreakdownTable_AgingProcLifoOn() {
			//This unit test asserts that adjustments inherit the ProcDate of the procedure they are associated with when AgingProcLifo is on.
			PrefT.UpdateYN(PrefName.AgingProcLifo,YN.Yes);
			string suffix=MethodInfo.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider(suffix);
			Patient patient=PatientT.CreatePatient(suffix,provNum);
			//Create a completed procedure on the 1st a few months ago.
			DateTime dateTimePast=DateTime.Now.AddMonths(-3);
			DateTime dateTimeProc=new DateTime(dateTimePast.Year,dateTimePast.Month,1);
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("APLON");
			Procedure procedure=ProcedureT.CreateProcedure(patient,procedureCode.ProcCode,ProcStat.C,"0",100,procDate:dateTimeProc,provNum:provNum);
			//Create an adjustment associated with the completed procedure that is dated a few days afterwards.
			DateTime dateTimeAdj=dateTimeProc.AddDays(10);
			Adjustment adjustment=AdjustmentT.MakeAdjustment(patient.PatNum,-50,adjDate:dateTimeAdj,procDate:dateTimeProc,procNum:procedure.ProcNum,provNum:provNum);
			//Run the Receivables Breakdown report for a date that falls in-between the two objects.
			DateTime dateTimeStart=dateTimeProc.AddDays(5);
			DataTable table=GetRecvBreakdownTableAdj(dateTimeStart,provNum);
			Assert.AreEqual(1,table.Rows.Count);//Adjustment should show up but with the ProcDate since it inherits the ProcDate.
			Assert.AreEqual(adjustment.ProcDate,PIn.DateT(table.Rows[0][0].ToString()));
			Assert.AreEqual(adjustment.AdjAmt,PIn.Double(table.Rows[0][1].ToString()));
			//Now run the report for a day that includes both objects.
			table=GetRecvBreakdownTableAdj(dateTimeAdj.AddDays(1),provNum);
			Assert.AreEqual(1,table.Rows.Count);//Adjustment should still show up and should still utilize the ProcDate (not the AdjDate).
			Assert.AreEqual(adjustment.ProcDate,PIn.DateT(table.Rows[0][0].ToString()));
			Assert.AreEqual(adjustment.AdjAmt,PIn.Double(table.Rows[0][1].ToString()));
		}

		private DataTable GetRecvBreakdownTableAdj(DateTime dateTimeStart,long provNum) {
			//========================================================================
			//===Strange variable date logic stolen from FormRpReceivablesBreakdown===
			//========================================================================
			//Get the year / month and instert the 1st of the month for stop point for calculated running balance
			string wDay="01";
			string wYear=dateTimeStart.Year.ToString();
			string wMonth=dateTimeStart.Month.ToString();
			if(wMonth.Length<2) {
				wMonth="0" + wMonth;
			}
			string wDate=wYear +"-"+ wMonth +"-"+ wDay;
			string bDate=wDate;
			string eDate=POut.Date(dateTimeStart.AddDays(1)).Substring(1,10);// Needed because all Queries are < end date to get correct Starting AR
			//========================================================================
			return RpReceivablesBreakdown.GetRecvBreakdownTable(
				dateStart:dateTimeStart,
				listProvNums:new List<long>() { provNum },
				isWriteoffPay:false,
				isPayPlan2:false,
				wDate:wDate,
				eDate:eDate,
				bDate:bDate,
				tableName:"TableAdj");
		}

	}
}
