using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System.Reflection;
using UnitTestsCore;

namespace UnitTests.OrthoCases_Tests {
	[TestClass]
	public class OrthoCasesTests:TestBase {
		///<summary>Make sure that setting an OrthoCase active, deactivates other orthocases for a patient.</summary>
		[TestMethod]
		public void OrthoCases_Activate_ActivateAnOrthoCaseAndDeactivateOthersForPat() {
			Userod user=UserodT.CreateUser();
			Security.CurUser=user;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure bandingProc1=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.TP,"",0);
			Procedure bandingProc2=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.TP,"",0);
			long orthoCaseNum1=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc1);
			long orthoCaseNum2=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc2);
			OrthoCase orthoCase2=OrthoCases.GetOne(orthoCaseNum2);
			OrthoPlanLink schedulePlanLink2=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum2,OrthoPlanLinkType.OrthoSchedule);
			OrthoSchedule orthoSchedule2=OrthoSchedules.GetOne(schedulePlanLink2.FKey);
			//Set one OrthoCase inactive. Now orthoCase1 is active and orthoCase2 is inactive.
			OrthoCases.SetActiveState(orthoCase2,schedulePlanLink2,orthoSchedule2,false);
			OrthoCase orthoCase1=OrthoCases.GetOne(orthoCaseNum1);
			OrthoPlanLink schedulePlanLink1=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum1,OrthoPlanLinkType.OrthoSchedule);
			OrthoSchedule orthoSchedule1=OrthoSchedules.GetOne(schedulePlanLink1.FKey);
			orthoCase2=OrthoCases.GetOne(orthoCaseNum2);
			schedulePlanLink2=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum2,OrthoPlanLinkType.OrthoSchedule);
			orthoSchedule2=OrthoSchedules.GetOne(schedulePlanLink2.FKey);
			Assert.AreEqual(orthoCase1.IsActive,true);
			Assert.AreEqual(schedulePlanLink1.IsActive,true);
			Assert.AreEqual(orthoSchedule1.IsActive,true);
			Assert.AreEqual(orthoCase2.IsActive,false);
			Assert.AreEqual(schedulePlanLink2.IsActive,false);
			Assert.AreEqual(orthoSchedule2.IsActive,false);
			//Active orthoCase2 which should inactivate orthoCase1
			OrthoCases.Activate(orthoCase2,pat.PatNum);
			orthoCase1=OrthoCases.GetOne(orthoCaseNum1);
			schedulePlanLink1=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum1,OrthoPlanLinkType.OrthoSchedule);
			orthoSchedule1=OrthoSchedules.GetOne(schedulePlanLink1.FKey);
			orthoCase2=OrthoCases.GetOne(orthoCaseNum2);
			schedulePlanLink2=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum2,OrthoPlanLinkType.OrthoSchedule);
			orthoSchedule2=OrthoSchedules.GetOne(schedulePlanLink2.FKey);
			Assert.AreEqual(orthoCase1.IsActive,false);
			Assert.AreEqual(schedulePlanLink1.IsActive,false);
			Assert.AreEqual(orthoSchedule1.IsActive,false);
			Assert.AreEqual(orthoCase2.IsActive,true);
			Assert.AreEqual(schedulePlanLink2.IsActive,true);
			Assert.AreEqual(orthoSchedule2.IsActive,true);
		}

		///<summary>Make sure that deleting an OrthoCase, deletes all associated objects.</summary>
		[TestMethod]
		public void OrthoCases_Delete_DeleteOrthoCaseAndAssociatedObjects() {
			Prefs.UpdateString(PrefName.OrthoBandingCodes,"D8080");
			Prefs.UpdateString(PrefName.OrthoDebondCodes,"D8070");
			Prefs.UpdateString(PrefName.OrthoVisitCodes,"D8060");
			Userod user=UserodT.CreateUser();
			Security.CurUser=user;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			Procedure visitProc=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",0);
			Procedure debondProc=ProcedureT.CreateProcedure(pat,"D8070",ProcStat.C,"",0);
			long orthoCaseNum=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc);
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,debondProc);
			OrthoCase orthoCase=OrthoCases.GetOne(orthoCaseNum);
			OrthoPlanLink schedulePlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			long orthoscheduleNum=schedulePlanLink.FKey;
			OrthoSchedule orthoSchedule=OrthoSchedules.GetOne(schedulePlanLink.FKey);
			List<OrthoProcLink> listAllProcLinks=OrthoProcLinks.GetManyByOrthoCase(orthoCaseNum);
			OrthoCases.Delete(orthoCase.OrthoCaseNum,orthoSchedule,schedulePlanLink,listAllProcLinks);
			orthoCase=OrthoCases.GetOne(orthoCaseNum);
			schedulePlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			orthoSchedule=OrthoSchedules.GetOne(orthoscheduleNum);
			listAllProcLinks=OrthoProcLinks.GetManyByOrthoCase(orthoCaseNum);
			Assert.AreEqual(orthoCase,null);
			Assert.AreEqual(schedulePlanLink,null);
			Assert.AreEqual(orthoSchedule,null);
			Assert.AreEqual(listAllProcLinks.Count,0);
		}

		///<summary>Make sure that the proper date updates for an OrthoCase when syncing dates with procedures.</summary>
		[TestMethod]
		public void OrthoCases_UpdateDatesByLinkedProc_UpdateBandingAndDebondDates() {
			Prefs.UpdateString(PrefName.OrthoDebondCodes,"D8070");
			Userod user=UserodT.CreateUser();
			Security.CurUser=user;
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			Procedure debondProc=ProcedureT.CreateProcedure(pat,"D8070",ProcStat.C,"",0,procDate:DateTime.Today.AddDays(2));
			long orthoCaseNum=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc);
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,debondProc);
			OrthoProcLink bandingProcLink=OrthoProcLinks.GetByType(orthoCaseNum,OrthoProcType.Banding);
			OrthoProcLink debondProcLink=OrthoProcLinks.GetByType(orthoCaseNum,OrthoProcType.Debond);
			OrthoCase orthoCase=OrthoCases.GetOne(orthoCaseNum);
			Assert.AreEqual(orthoCase.BandingDate,DateTime.Today);
			bandingProc.ProcDate=DateTime.Today.AddDays(1);
			OrthoCases.UpdateDatesByLinkedProc(bandingProcLink,bandingProc);
			orthoCase=OrthoCases.GetOne(orthoCaseNum);
			Assert.AreEqual(orthoCase.BandingDate,DateTime.Today.AddDays(1));
			Assert.AreEqual(orthoCase.DebondDate,DateTime.Today.AddDays(2));
			debondProc.ProcDate=DateTime.Today.AddDays(3);
			OrthoCases.UpdateDatesByLinkedProc(debondProcLink,debondProc);
			orthoCase=OrthoCases.GetOne(orthoCaseNum);
			Assert.AreEqual(orthoCase.BandingDate,DateTime.Today.AddDays(1));
			Assert.AreEqual(orthoCase.DebondDate,DateTime.Today.AddDays(3));
		}

		///<summary>Make sure that procedures get linked to OrthoCase and that OrthoCase gets inactivated if a debond proc is linked.</summary>
		[TestMethod]
		public void OrthoProcLinks_LinkProcForActiveOrthoCase_LinkCompletedProcsToOrthoCase() {
			Prefs.UpdateString(PrefName.OrthoBandingCodes,"D8080");
			Prefs.UpdateString(PrefName.OrthoDebondCodes,"D8070");
			Prefs.UpdateString(PrefName.OrthoVisitCodes,"D8060");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			Procedure visitProc=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",0);
			Procedure debondProc=ProcedureT.CreateProcedure(pat,"D8070",ProcStat.C,"",0);
			long orthoCaseNum=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc);
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,debondProc);
			OrthoCase orthoCase=OrthoCases.GetOne(orthoCaseNum);
			List<OrthoProcLink> listAllProcLinks=OrthoProcLinks.GetManyByOrthoCase(orthoCaseNum);
			List<OrthoProcLink> listVisitProcLinks=OrthoProcLinks.GetVisitLinksForOrthoCase(orthoCaseNum);
			OrthoProcLink debondProcLink=OrthoProcLinks.GetByType(orthoCaseNum,OrthoProcType.Debond);
			Assert.AreEqual(orthoCase.IsActive,false);
			Assert.AreEqual(listAllProcLinks.Count,3);
			Assert.AreEqual(debondProcLink.ProcNum,debondProc.ProcNum);
			Assert.AreEqual(debondProcLink.SecUserNumEntry,Security.CurUser.UserNum);
			Assert.AreEqual(listVisitProcLinks.Count,1);
			Assert.AreEqual(listVisitProcLinks[0].ProcNum,visitProc.ProcNum);
			Assert.AreEqual(listVisitProcLinks[0].SecUserNumEntry,Security.CurUser.UserNum);
		}

		///<summary>Make sure that fees for procedures linked to orthocases are set correctly.</summary>
		[TestMethod]
		public void OrthoProcLinks_SetProcFeeForLinkedProc_SetFeeForEachProcType() {
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			Procedure visitProc=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",0);
			Procedure debondProc=ProcedureT.CreateProcedure(pat,"D8070",ProcStat.C,"",0);
			long orthoCaseNum=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,new Procedure());
			OrthoCase orthoCase=OrthoCases.GetOne(orthoCaseNum);
			OrthoPlanLink schedulePlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			OrthoSchedule orthoSchedule=OrthoSchedules.GetOne(schedulePlanLink.FKey);
			List<OrthoProcLink> listVisitProcLinks=OrthoProcLinks.GetVisitLinksForOrthoCase(orthoCaseNum);
			OrthoProcLinks.SetProcFeeForLinkedProc(orthoCase,bandingProc,OrthoProcType.Banding,listVisitProcLinks,schedulePlanLink,orthoSchedule);
			OrthoProcLinks.SetProcFeeForLinkedProc(orthoCase,visitProc,OrthoProcType.Visit,listVisitProcLinks,schedulePlanLink,orthoSchedule);
			OrthoProcLinks.SetProcFeeForLinkedProc(orthoCase,debondProc,OrthoProcType.Debond,listVisitProcLinks,schedulePlanLink,orthoSchedule);
			Assert.AreEqual(bandingProc.ProcFee,1000);
			Assert.AreEqual(debondProc.ProcFee,400);
			Assert.AreEqual(visitProc.ProcFee,60);
		}

		///<summary>Make Sure that ClaimProc overrides for procedures linked to orthocases are set correctly</summary>
		[TestMethod]
		public void ClaimProcs_ComputeEstimatesByOrthoCase_SetClaimProcForEachProcType() {
			Prefs.UpdateString(PrefName.OrthoBandingCodes,"D8080");
			Prefs.UpdateString(PrefName.OrthoDebondCodes,"D8070");
			Prefs.UpdateString(PrefName.OrthoVisitCodes,"D8060");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,0);
			ClaimProc bandingClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,bandingProc.ProcNum,0,0,bandingProc.ProcDate,1,1,1);
			Procedure visitProc=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",0);
			ClaimProc visitClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,visitProc.ProcNum,0,0,visitProc.ProcDate,1,1,1);
			Procedure debondProc=ProcedureT.CreateProcedure(pat,"D8070",ProcStat.C,"",0);
			ClaimProc debondClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,debondProc.ProcNum,0,0,debondProc.ProcDate,1,1,1);
			long orthoCaseNum=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc);
			OrthoCase orthoCase=OrthoCases.GetOne(orthoCaseNum);
			OrthoPlanLink schedulePlanLink=OrthoPlanLinks.GetOneForOrthoCaseByType(orthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			OrthoSchedule orthoSchedule=OrthoSchedules.GetOne(schedulePlanLink.FKey);
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,bandingProc);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,debondProc);
			List<OrthoProcLink> allProcLinks=OrthoProcLinks.GetManyByOrthoCase(orthoCaseNum);
			OrthoProcLink bandingLink=allProcLinks.FirstOrDefault(x => x.ProcLinkType==OrthoProcType.Banding);
			OrthoProcLink debondLink=allProcLinks.FirstOrDefault(x => x.ProcLinkType==OrthoProcType.Debond);
			OrthoProcLink visitLink=allProcLinks.FirstOrDefault(x => x.ProcLinkType==OrthoProcType.Visit);
			List<ClaimProc> listAllClaimProcs=new List<ClaimProc>() {bandingClaimProc,visitClaimProc,debondClaimProc};
			List<OrthoProcLink> listAllOrthoProcLinks=new List<OrthoProcLink>(){bandingLink,visitLink,debondLink};
			List<PatPlan> listPatPlans=new List<PatPlan>(){patPlan};
			ClaimProcs.ComputeEstimatesByOrthoCase(bandingProc,bandingLink,orthoCase,orthoSchedule,true,listAllClaimProcs,
				new List<ClaimProc>() { bandingClaimProc },listPatPlans,listAllOrthoProcLinks);
			ClaimProcs.ComputeEstimatesByOrthoCase(debondProc,debondLink,orthoCase,orthoSchedule,true,listAllClaimProcs,
				new List<ClaimProc>() { debondClaimProc },listPatPlans,listAllOrthoProcLinks);
			ClaimProcs.ComputeEstimatesByOrthoCase(visitProc,visitLink,orthoCase,orthoSchedule,true,listAllClaimProcs,
				new List<ClaimProc>() { visitClaimProc },listPatPlans,listAllOrthoProcLinks);
			Assert.AreEqual(bandingClaimProc.AllowedOverride,0);
			Assert.AreEqual(bandingClaimProc.CopayOverride,0);
			Assert.AreEqual(bandingClaimProc.PercentOverride,0);
			Assert.AreEqual(bandingClaimProc.PaidOtherInsOverride,0);
			Assert.AreEqual(bandingClaimProc.WriteOffEstOverride,0);
			Assert.AreEqual(bandingClaimProc.InsEstTotalOverride,600);
			Assert.AreEqual(debondClaimProc.AllowedOverride,0);
			Assert.AreEqual(debondClaimProc.CopayOverride,0);
			Assert.AreEqual(debondClaimProc.PercentOverride,0);
			Assert.AreEqual(debondClaimProc.PaidOtherInsOverride,0);
			Assert.AreEqual(debondClaimProc.WriteOffEstOverride,0);
			Assert.AreEqual(debondClaimProc.InsEstTotalOverride,240);
			Assert.AreEqual(visitClaimProc.AllowedOverride,0);
			Assert.AreEqual(visitClaimProc.CopayOverride,0);
			Assert.AreEqual(visitClaimProc.PercentOverride,0);
			Assert.AreEqual(visitClaimProc.PaidOtherInsOverride,0);
			Assert.AreEqual(visitClaimProc.WriteOffEstOverride,0);
			Assert.AreEqual(visitClaimProc.InsEstTotalOverride,36);
		}

		///<summary>Ensure that completed ortho case procedure links to pay plan that is linked to ortho case when appropriate.</summary>
		[TestMethod]
		public void OrthoProcLinks_LinkProcForActiveOrthoCase_ProcLinksToPayPlanWhenTheyShould() {
			//Set Prefs
			Prefs.UpdateString(PrefName.OrthoVisitCodes,"D8060");
			//Create required objects
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name);
			Procedure visitProc1=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",50);
			Procedure visitProc2=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",50);
			Procedure visitProc3=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",50);
			Procedure visitProc4=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",50);
			Procedure visitProc5=ProcedureT.CreateProcedure(pat,"D8060",ProcStat.C,"",50);
			long orthoCaseNum=OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1000,0,100,DateTime.Today,true,DateTime.Today.AddMonths(12),0,1000,250);
			PayPlan payPlanDynamic1=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,DateTime.Today,0,0,25,new List<Procedure>(),new List<Adjustment>()
				,frequency:PayPlanFrequency.Monthly);
			PayPlan payPlanDynamic2=PayPlanT.CreateDynamicPaymentPlan(pat.PatNum,0,DateTime.Today,0,0,25,new List<Procedure>(),new List<Adjustment>()
				,frequency:PayPlanFrequency.Monthly);
			PayPlanLinkT.CreatePayPlanLink(payPlanDynamic2,visitProc2.ProcNum,PayPlanLinkType.Procedure);
			PayPlan patPayPlan=PayPlanT.CreatePayPlanNoCharges(pat.PatNum,50,DateTime.Today,0);
			PayPlanChargeT.CreateOne(patPayPlan.PayPlanNum,0,pat.PatNum,DateTime.Today,50,procNum:visitProc3.ProcNum,
				chargeType:PayPlanChargeType.Credit);
			//Link Pay Plan to Ortho Case
			OrthoPlanLink orthoPlanLinkPatPayPlan=new OrthoPlanLink();
			orthoPlanLinkPatPayPlan.LinkType=OrthoPlanLinkType.PatPayPlan;
			orthoPlanLinkPatPayPlan.OrthoCaseNum=orthoCaseNum;
			orthoPlanLinkPatPayPlan.FKey=payPlanDynamic1.PayPlanNum;
			orthoPlanLinkPatPayPlan.IsActive=true;
			orthoPlanLinkPatPayPlan.SecUserNumEntry=Security.CurUser.UserNum;
			orthoPlanLinkPatPayPlan.OrthoPlanLinkNum=OrthoPlanLinks.Insert(orthoPlanLinkPatPayPlan);
			//Link visitProc1 to ortho case and confirm that it linked to payPlanDynamic1.
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc1);
			List<PayPlanLink> listProcLinksForPayPlan=PayPlanLinks.GetForPayPlansAndLinkType(new List<long>{payPlanDynamic1.PayPlanNum},PayPlanLinkType.Procedure);
			Assert.AreEqual(listProcLinksForPayPlan.Count,1);
			Assert.AreEqual(listProcLinksForPayPlan.Select(x => x.FKey).Contains(visitProc1.ProcNum),true);
			//Link visitProc2 to ortho case and confirm that it did not link to payPlanDynamic1 because it is linked to payPlanDynamic2 already.
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc2);
			listProcLinksForPayPlan=PayPlanLinks.GetForPayPlansAndLinkType(new List<long> { payPlanDynamic1.PayPlanNum },PayPlanLinkType.Procedure);
			Assert.AreEqual(listProcLinksForPayPlan.Count,1);
			Assert.AreEqual(listProcLinksForPayPlan.Select(x => x.FKey).Contains(visitProc2.ProcNum),false);
			//Link visitProc3 to ortho case and confirm that it did not link to payPlanDynamic1 because it is credited to patPayPlan already.
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc3);
			listProcLinksForPayPlan=PayPlanLinks.GetForPayPlansAndLinkType(new List<long> { payPlanDynamic1.PayPlanNum },PayPlanLinkType.Procedure);
			Assert.AreEqual(listProcLinksForPayPlan.Count,1);
			Assert.AreEqual(listProcLinksForPayPlan.Select(x => x.FKey).Contains(visitProc3.ProcNum),false);
			//Close payPlanDynamic1 and confirm that proc won't link when pay plan closed.
			payPlanDynamic1.IsClosed=true;
			PayPlans.Update(payPlanDynamic1);
			orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc4);
			listProcLinksForPayPlan=PayPlanLinks.GetForPayPlansAndLinkType(new List<long> { payPlanDynamic1.PayPlanNum },PayPlanLinkType.Procedure);
			Assert.AreEqual(listProcLinksForPayPlan.Count,1);
			Assert.AreEqual(listProcLinksForPayPlan.Select(x => x.FKey).Contains(visitProc4.ProcNum),false);
			//Open plan again and lock it, then confirm that proc won't link when pay plan is locked
			payPlanDynamic1.IsClosed=false;
			payPlanDynamic1.IsLocked=true;
			PayPlans.Update(payPlanDynamic1);
			orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(pat.PatNum);
			OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,visitProc5);
			listProcLinksForPayPlan=PayPlanLinks.GetForPayPlansAndLinkType(new List<long> { payPlanDynamic1.PayPlanNum },PayPlanLinkType.Procedure);
			Assert.AreEqual(listProcLinksForPayPlan.Count,1);
			Assert.AreEqual(listProcLinksForPayPlan.Select(x => x.FKey).Contains(visitProc5.ProcNum),false);
		}
	}
}


