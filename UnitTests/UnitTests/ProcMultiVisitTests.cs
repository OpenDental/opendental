using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
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

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ProcMultiVisit_UpdateGroupForProc_UpdatedClaimDates)]
		[Documentation.VersionAdded("22.3")]
		[Documentation.Description("Patient has a Tp'd for a multiple visit group with two procedures, D2750 and N4118. They have their own insurance where they are the subscriber. Then create a claim for D2750 which should have the status 'Hold for In Process' until N4118 is completed. Ensure the claim date is set to the first procedure's date. Then complete N4118 which should update the claim's dates of service accordingly, and have the status 'Waiting to Send'.")]
		public void ProcMultiVisit_UpdateGroupForProc_UpdatedClaimDates() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			List<Procedure> listProcs=new List<Procedure>();
			Procedure procBillable=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"1",100,new DateTime(2018,5,1));//PFM
			listProcs.Add(procBillable);
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			Procedure procDelivery=ProcedureT.CreateProcedure(pat,"N4118",ProcStat.TP,"1",0,new DateTime(2018,8,20));//Seat - usually completed several months later.
			listProcs.Add(procDelivery);
			ProcMultiVisits.CreateGroup(listProcs);
			Procedure procBillableOld=procBillable.Copy();
			procBillable.ProcStatus=ProcStat.C;
			Procedures.Update(procBillable,procBillableOld);
			Assert.AreEqual(ProcMultiVisits.IsProcInProcess(procBillable.ProcNum),true);//This proc is In Process because it is set complete, while at the same time not all procs in the group are complete.
			Assert.AreEqual(ProcMultiVisits.IsProcInProcess(procDelivery.ProcNum),false);//This proc is not In Process because it is not set to complete.
			Claim claim=new Claim();
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			claim.ClaimStatus="W";
			//Dates of service are calculated inside AccountModules.CreateClaim().
			//The procDelivery cannot be attached to the claim in the UI, because $0 procs are blocked by UI.  Therefore, we only attach the procBilled to the claim here.
			ODTuple<bool,Claim,string> clmResult=AccountModules.CreateClaim(claim,"P",insInfo.ListPatPlans,insInfo.ListInsPlans,listClaimProcs,listProcs,
				insInfo.ListInsSubs,pat,null,new List<Procedure> { procBillable },"",insInfo.PriInsPlan,insInfo.PriInsSub,Relat.Self);
			Assert.AreEqual(clmResult.Item2.DateService,procBillable.ProcDate);//Claim date of service must always be the greatest date in the multi visit group.
			listClaimProcs=ClaimProcs.RefreshForClaim(clmResult.Item2.ClaimNum);
			Assert.AreEqual(listClaimProcs[0].ProcDate,procBillable.ProcDate);//Proc date of service must always be the greatest completed date in the multi visit group, even if performed on a different day.
			Assert.AreEqual(clmResult.Item2.ClaimStatus,"I");//MultiVisit Group is not complete so claim should be 'Hold for In process'.
			Procedure procDeliveryOld=procDelivery.Copy();
			procDelivery.ProcStatus=ProcStat.C;
			Procedures.Update(procDelivery,procDeliveryOld);
			List<Claim> listClaims=Claims.Refresh(pat.PatNum);
			Assert.AreEqual(listClaims[0].ClaimStatus,"W");//MultiVisit Group completed so claim should be 'Waiting to Send'.
			listClaimProcs=ClaimProcs.RefreshForClaim(clmResult.Item2.ClaimNum);
			Assert.AreEqual(listClaimProcs[0].ProcDate,procDelivery.ProcDate);//Proc date of service must always be the greatest date in the multi visit group, even if performed on a different day.
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ProcMultiVisit_UpdateGroupForProc_UpdateClaimNotPreAuth)]
		[Documentation.VersionAdded("22.3")]
		[Documentation.Description("Patient has a Tp'd for a multiple visit group with two procedures, D2750 and N4118. They have their own insurance where they are the subscriber. D2750 is first attached to a PreAuth claim. Receive that PreAuth and complete D2750. The PreAuth should still be received. Afterwards create a claim for D2750 which should have the status 'Hold for In Process' until N4118 is completed. The PreAuth claim should not have it's status updated since that claim was already marked 'Received'. Next complete N4118. The PreAuth should still be received, and the other claim should be 'Waiting to send' now.")]
		public void ProcMultiVisit_UpdateGroupForProc_UpdateClaimNotPreAuth() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			List<Procedure> listProcs=new List<Procedure>();
			Procedure procBillable=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"1",100,new DateTime(2018,5,1));//PFM
			listProcs.Add(procBillable);
			Procedure procDelivery=ProcedureT.CreateProcedure(pat,"N4118",ProcStat.TP,"1",0,new DateTime(2018,8,20));//Seat - usually completed several months later.
			listProcs.Add(procDelivery);
			ProcMultiVisits.CreateGroup(listProcs);
			//Create a preauth claim and associate the first procedure.
			Claim claimPreAuth=ClaimT.CreateClaim(new List<Procedure> { procBillable },insInfo,claimType:"PreAuth");
			claimPreAuth.ClaimStatus="R";//manually set the status to received.
			Claims.Update(claimPreAuth);
			//Pretend preauth got received. This is what we do when we receive the claim and enter the ins est amount.
			List<ClaimProc> listClaimProcsPreAuth=ClaimProcs.RefreshForClaim(claimPreAuth.ClaimNum);
			listClaimProcsPreAuth[0].Status=ClaimProcStatus.Preauth;
			listClaimProcsPreAuth[0].InsPayEst=procBillable.ProcFee;
			listClaimProcsPreAuth[0].InsEstTotalOverride=procBillable.ProcFee;
			listClaimProcsPreAuth[0].FeeBilled=procBillable.ProcFee;
			ClaimProcs.Update(listClaimProcsPreAuth[0]);
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,insInfo);
			Procedure procBillableOld=procBillable.Copy();
			procBillable.ProcStatus=ProcStat.C;
			Procedures.Update(procBillable,procBillableOld);
			Assert.AreEqual(ProcMultiVisits.IsProcInProcess(procBillable.ProcNum),true);//This proc is In Process because it is set complete, while at the same time not all procs in the group are complete.
			Assert.AreEqual(ProcMultiVisits.IsProcInProcess(procDelivery.ProcNum),false);//This proc is not In Process because it is not set to complete.
			List<Claim> listClaims=Claims.Refresh(pat.PatNum);
			Assert.AreEqual(listClaims[0].ClaimStatus,"R");//Ensure PreAuth is still marked as recieved.
			//Create a claim for the single completed procedure with the preauth.
			Claim claim=new Claim();
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			claim.ClaimStatus="W";
			ODTuple<bool,Claim,string> clmResult=AccountModules.CreateClaim(claim,"P",insInfo.ListPatPlans,insInfo.ListInsPlans,listClaimProcs,listProcs,
				insInfo.ListInsSubs,pat,null,new List<Procedure> { procBillable },"",insInfo.PriInsPlan,insInfo.PriInsSub,Relat.Self);
			Assert.AreEqual(clmResult.Item2.DateService,procBillable.ProcDate);//Claim date of service must always be the greatest date in the multi visit group.
			listClaimProcs=ClaimProcs.RefreshForClaim(clmResult.Item2.ClaimNum);
			Assert.AreEqual(listClaimProcs[0].ProcDate,procBillable.ProcDate);//Proc date of service must always be the greatest completed date in the multi visit group.
			Assert.AreEqual(clmResult.Item2.ClaimStatus,"I");//MultiVisit Group is not complete so claim should be 'Hold for In process'.
			Procedure procDeliveryOld=procDelivery.Copy();
			procDelivery.ProcStatus=ProcStat.C;
			Procedures.Update(procDelivery,procDeliveryOld);
			listClaims=Claims.Refresh(pat.PatNum);
			Assert.AreEqual(listClaims.FirstOrDefault(x => x.ClaimNum==clmResult.Item2.ClaimNum).ClaimStatus,"W");//MultiVisit Group completed so claim should be 'Waiting to Send'.
			listClaimProcs=ClaimProcs.RefreshForClaim(clmResult.Item2.ClaimNum);
			Assert.AreEqual(listClaimProcs[0].ProcDate,procDelivery.ProcDate);//Proc date of service must always be the greatest date in the multi visit group.
			Assert.AreEqual(listClaims.FirstOrDefault(x => x.ClaimNum==claimPreAuth.ClaimNum).ClaimStatus,"R");//Ensure PreAuth is still marked as recieved.
		}
	}
}
