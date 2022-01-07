using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnitTestsCore;

namespace UnitTests.ProcMultiVisit_Tests {
	[TestClass]
	public class ProcMultiVisitTests:TestBase {

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		[TestMethod]
		public void ProcMultiVisitTests_Crown_PseudoStatusStages() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			List<Procedure> listProcs=new List<Procedure>();
			Procedure procBillable=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"1",100,new DateTime(2018,5,1));//PFM
			listProcs.Add(procBillable);
			Procedure procDelivery=ProcedureT.CreateProcedure(pat,"N4118",ProcStat.TP,"1",0,new DateTime(2018,8,20));//Seat - usually completed several months later.
			listProcs.Add(procDelivery);
			ProcMultiVisits.CreateGroup(listProcs);
			Assert.IsFalse(ProcMultiVisits.IsProcInProcess(procBillable.ProcNum));//This proc is not in process because there must be at least one complete procedure.
			Assert.IsFalse(ProcMultiVisits.IsProcInProcess(procDelivery.ProcNum));//This proc is not in process because there must be at least one complete procedure.
			Procedure procBillableOld=procBillable.Copy();
			procBillable.ProcStatus=ProcStat.C;
			Procedures.Update(procBillable,procBillableOld);
			Assert.IsTrue(ProcMultiVisits.IsProcInProcess(procBillable.ProcNum));//This proc is In Process because it is set complete, while at the same time no all procs in the group are complete.
			Assert.IsFalse(ProcMultiVisits.IsProcInProcess(procDelivery.ProcNum));//This proc is not In Process because not set to complete.
			Procedure procDeliveryOld=procDelivery.Copy();
			procDelivery.ProcStatus=ProcStat.C;
			Procedures.Update(procDelivery,procDeliveryOld);
			Assert.IsFalse(ProcMultiVisits.IsProcInProcess(procBillable.ProcNum));//Both procedures complete means the group is now complete (not In Process).
			Assert.IsFalse(ProcMultiVisits.IsProcInProcess(procDelivery.ProcNum));//Both procedures complete means the group is now complete (not In Process).
		}

		[TestMethod]
		public void ProcMultiVisitTests_CrownGroupComplete_ClaimDates() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			List<Procedure> listProcs=new List<Procedure>();
			Procedure procBillable=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"1",100,new DateTime(2018,5,1));//PFM
			listProcs.Add(procBillable);
			Procedure procDelivery=ProcedureT.CreateProcedure(pat,"N4118",ProcStat.TP,"1",0,new DateTime(2018,8,20));//Seat - usually completed several months later.
			listProcs.Add(procDelivery);
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			ProcMultiVisits.CreateGroup(listProcs);
			Procedure procBillableOld=procBillable.Copy();
			procBillable.ProcStatus=ProcStat.C;
			Procedures.Update(procBillable,procBillableOld);
			Procedure procDeliveryOld=procDelivery.Copy();
			procDelivery.ProcStatus=ProcStat.C;
			Procedures.Update(procDelivery,procDeliveryOld);
			Assert.AreEqual(ProcMultiVisits.IsProcInProcess(procBillable.ProcNum),false);//Both procedures complete means the group is now complete (not In Process).
			Assert.AreEqual(ProcMultiVisits.IsProcInProcess(procDelivery.ProcNum),false);//Both procedures complete means the group is now complete (not In Process).
			Claim claim=new Claim();
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			claim.ClaimStatus="W";
			//Dates of service are calculated inside AccountModules.CreateClaim().
			//The procDelivery cannot be attached to the claim in the UI, because $0 procs are blocked by UI.  Therefore, we only attach the procBilled to the claim here.
			ODTuple<bool,Claim,string> clmResult=AccountModules.CreateClaim(claim,"P",insInfo.ListPatPlans,insInfo.ListInsPlans,listClaimProcs,listProcs,
				insInfo.ListInsSubs,pat,null,new List<Procedure> { procBillable },"",insInfo.PriInsPlan,insInfo.PriInsSub,Relat.Self);
			Assert.AreEqual(clmResult.Item3,"");//Ensure no validation errors creating the claim.  This is to verify the integrity of the unit test design.
			Assert.AreEqual(clmResult.Item2.DateService,procDelivery.ProcDate);//Claim date of service must always be the greatest date in the multi visit group.
			listClaimProcs=ClaimProcs.RefreshForClaim(clmResult.Item2.ClaimNum);
			Assert.AreEqual(listClaimProcs[0].ProcDate,procDelivery.ProcDate);//Proc date of service must always be the greatest date in the multi visit group, even if performed on a different day.
		}

	}
}
