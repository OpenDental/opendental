using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.SubstitutionLinks_Tests {
	[TestClass]
	public class SubstitutionLinksTests:TestBase {

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			SubstitutionLinkT.ClearSubstitutionLinkTable();
			ProcedureCodeT.ClearProcedureCodeTable();
		}

		[TestMethod]
		public void SubstitutionLinks_GetSubstitutionCode_MultiOptions() {
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("D1234");
			SubstitutionLink subLink1=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"1234",SubstitutionCondition.Always,0);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"5678",SubstitutionCondition.Molar,0);
			SubstitutionLink subLink3=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"0009",SubstitutionCondition.Posterior,0);
			SubstitutionLink subLink4=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"1111",SubstitutionCondition.SecondMolar,0);
			List<SubstitutionLink> listSubLinks=new List<SubstitutionLink>() {
				subLink1,subLink2,subLink3,subLink4
			};
			SubstitutionLink subLinkMolar=SubstitutionLinks.GetSubLinkByHierarchy(procCode1,"3",listSubLinks);
			Assert.AreEqual(subLink2,subLinkMolar);
			var subLinkPosterior=SubstitutionLinks.GetSubLinkByHierarchy(procCode1,"4",listSubLinks);
			Assert.AreEqual(subLink3,subLinkPosterior);
			var subLink2ndMolar=SubstitutionLinks.GetSubLinkByHierarchy(procCode1,"31",listSubLinks);
			Assert.AreEqual(subLink4,subLink2ndMolar);
		}

		[TestMethod]
		public void SubstitutionLinks_GetSubstitutionCode_SubConditionNever() {
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("D1234");
			SubstitutionLink subLink1=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"1234",SubstitutionCondition.Never,0);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"5678",SubstitutionCondition.Molar,0);
			SubstitutionLink subLink3=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"0009",SubstitutionCondition.Posterior,0);
			List<SubstitutionLink> listSubLinks=new List<SubstitutionLink>() { subLink1,subLink2,subLink3 };
			SubstitutionLink subLinkNone=SubstitutionLinks.GetSubLinkByHierarchy(procCode1,"1",listSubLinks);
			Assert.AreEqual(subLink1,subLinkNone);
		}

		[TestMethod]
		public void SubstitutionLinks_FilterSubLinksByCodeNum_HappyPath() {
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("D1234");
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("D1111");
			SubstitutionLink subLink1=SubstitutionLinkT.CreateSubstitutionLink(procCode1.CodeNum,"1234",SubstitutionCondition.Always,0);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(procCode2.CodeNum,"5678",SubstitutionCondition.Molar,0);
			List<SubstitutionLink> listSubLinks=new List<SubstitutionLink>() { subLink1,subLink2 };
			List<SubstitutionLink> listFilteredSubLinks=SubstitutionLinks.FilterSubLinksByCodeNum(procCode1.CodeNum,listSubLinks);
			Assert.IsFalse(listFilteredSubLinks.Contains(subLink2));
		}

		[TestMethod]
		public void SubstitutionLinks_GetSubstitutionCode_ProcFeeUpdates() {
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("D1234");
			ProcedureCode procCodeSub=ProcedureCodeT.CreateProcCode("D5678");
			ProcedureCode procCodeSub2=ProcedureCodeT.CreateProcCode("D1111");
			Carrier carrier=CarrierT.CreateCarrier("Carrier");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"TestSched",false);
			FeeScheds.RefreshCache();
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum,feeSched:feeSchedNum,copayFeeSched:feeSchedNum);
			PatientT.CreatePatient("Pat");
			SubstitutionLink subLink=SubstitutionLinkT.CreateSubstitutionLink(procCode.CodeNum,procCodeSub.ProcCode,SubstitutionCondition.Molar,insPlan.PlanNum);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(procCode.CodeNum,procCodeSub2.ProcCode,SubstitutionCondition.Always,insPlan.PlanNum);
			FeeT.GetNewFee(feeSchedNum,procCode.CodeNum,amount:100);
			Fee feeNew=FeeT.GetNewFee(feeSchedNum,ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode,"14",insPlan.PlanNum),amount:50);
			double allowed=InsPlans.GetAllowed(procCode.ProcCode,insPlan.FeeSched,0,false,"","14",0,0,insPlan.PlanNum,new List<SubstitutionLink>(){subLink,subLink2});
			Assert.AreEqual(feeNew.Amount,allowed);
		}

		[TestMethod]
		public void SubstitutionLinks_GetSubstitutionCode_GetCorrectFeeBySubLink() {
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("D1234");
			ProcedureCode procCodeSub=ProcedureCodeT.CreateProcCode("D5678");
			ProcedureCode procCodeSub2=ProcedureCodeT.CreateProcCode("D1111");
			Carrier carrier=CarrierT.CreateCarrier("Carrier");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"TestSched",false);
			string postToothNum="5";
			string toothNum="7";
			FeeScheds.RefreshCache();
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum,feeSched:feeSchedNum,copayFeeSched:feeSchedNum);
			PatientT.CreatePatient("Pat");
			SubstitutionLink subLink=SubstitutionLinkT.CreateSubstitutionLink(procCode.CodeNum,procCodeSub.ProcCode,SubstitutionCondition.Posterior,insPlan.PlanNum);
			SubstitutionLink subLink2=SubstitutionLinkT.CreateSubstitutionLink(procCode.CodeNum,procCodeSub2.ProcCode,SubstitutionCondition.Always,insPlan.PlanNum);
			List<SubstitutionLink> listSubLinks=new List<SubstitutionLink>() {subLink,subLink2};
			Fee feePost=FeeT.GetNewFee(feeSchedNum,procCodeSub.CodeNum,amount:50);
			Fee feeAlways=FeeT.GetNewFee(feeSchedNum,procCodeSub2.CodeNum,amount:25);
			double feeAmtPost=InsPlans.GetAllowed(procCode.ProcCode,insPlan.FeeSched,0,false,"",postToothNum,0,0,insPlan.PlanNum,listSubLinks);
			double feeAmtAlways=InsPlans.GetAllowed(procCode.ProcCode,insPlan.FeeSched,0,false,"",toothNum,0,0,insPlan.PlanNum,listSubLinks);
			Assert.AreEqual(feePost.Amount,feeAmtPost);
			Assert.AreEqual(feeAlways.Amount,feeAmtAlways);
		}

		[TestMethod]
		public void SubstitutionLinks_GetSubstitutioncode_DontUseGlobalIfInsuranceExists() {
			//set up main proc and global sub
			ProcedureCode procMain=ProcedureCodeT.CreateProcCode("D2391");
			ProcedureCode procGlobalSub=ProcedureCodeT.CreateProcCode("D6010");
			procMain.SubstitutionCode=procGlobalSub.ProcCode;
			procMain.SubstOnlyIf=SubstitutionCondition.Always;//the global default may be always but with an insurance level override for the same code, this shouldn't come into play
			ProcedureCodeT.Update(procMain);
			ProcedureCode procCodeSub=ProcedureCodeT.CreateProcCode("D2740");
			Carrier carrier=CarrierT.CreateCarrier("Carrier");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"TestSched",false);
			string toothNum="7";//This is not a posterior tooth
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum,feeSched:feeSchedNum,copayFeeSched:feeSchedNum);
			SubstitutionLink subLinkForMainProc=SubstitutionLinkT.CreateSubstitutionLink(procMain.CodeNum,procCodeSub.ProcCode,SubstitutionCondition.Posterior,insPlan.PlanNum);
			Assert.IsTrue(SubstitutionLinks.HasSubstCodeForPlan(insPlan,procMain.CodeNum,new List<SubstitutionLink>() {subLinkForMainProc}));
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(procMain.ProcCode,toothNum,insPlan.PlanNum);
			Assert.AreNotEqual(procGlobalSub.CodeNum,subCodeNum);//Global substitution not used because at least one plan level sub existed (even though it wasn't satisfied)
			Assert.AreEqual(procMain.CodeNum,subCodeNum);//No substitution
		}

		[TestMethod]
		public void SubstitutionLinks_GetSubstiutioncode_UseGlobalForPosterior() {
			//set up main proc and global sub
			ProcedureCode procMain=ProcedureCodeT.CreateProcCode("D2391");
			ProcedureCode procGlobalSub=ProcedureCodeT.CreateProcCode("D6010");
			procMain.SubstitutionCode=procGlobalSub.ProcCode;
			procMain.SubstOnlyIf=SubstitutionCondition.Posterior;
			ProcedureCodeT.Update(procMain);
			ProcedureCode procCodeSub=ProcedureCodeT.CreateProcCode("D2740");
			Carrier carrier=CarrierT.CreateCarrier("Carrier");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"TestSched",false);
			string toothNum="7";//Not a posterior tooh
			string toothNumPost="5";//Posterior tooth
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum,feeSched:feeSchedNum,copayFeeSched:feeSchedNum);
			SubstitutionLink subLink=SubstitutionLinkT.CreateSubstitutionLink(procCodeSub.CodeNum,"D2394",SubstitutionCondition.Always,insPlan.PlanNum);
			Assert.IsTrue(SubstitutionLinks.HasSubstCodeForPlan(insPlan,procMain.CodeNum,new List<SubstitutionLink>(){subLink}));
			long subCodeNum=ProcedureCodes.GetSubstituteCodeNum(procMain.ProcCode,toothNum,insPlan.PlanNum);
			Assert.AreEqual(procMain.CodeNum,subCodeNum);//not a subst, ins plan sub isn't same code as main, not a posterior tooth passed in
			long subCodeNumPost=ProcedureCodes.GetSubstituteCodeNum(procMain.ProcCode,toothNumPost,insPlan.PlanNum);
			Assert.AreEqual(procGlobalSub.CodeNum,subCodeNumPost);
		}

		[TestMethod]
		public void SubstituionLinks_GetSubstituionCode_GetForPlan() {
			//set up main proc and global sub
			ProcedureCode procMain=ProcedureCodeT.CreateProcCode("D2391");
			ProcedureCode procAlwaysSub=ProcedureCodeT.CreateProcCode("D6010");
			procMain.SubstitutionCode=procAlwaysSub.ProcCode;
			procMain.SubstOnlyIf=SubstitutionCondition.Always;
			ProcedureCodeT.Update(procMain);
			Carrier carrier=CarrierT.CreateCarrier("Carrier");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay,"TestSched",false);
			InsPlan insPlan1=InsPlanT.CreateInsPlan(carrier.CarrierNum,feeSched:feeSchedNum,copayFeeSched:feeSchedNum);//InsPlan with 'Never' Sub code
			InsPlan insPlan2=InsPlanT.CreateInsPlan(carrier.CarrierNum,feeSched:feeSchedNum,copayFeeSched:feeSchedNum);//InsPlan with no sub code--global applies here
			SubstitutionLink subLinkForPlan1=SubstitutionLinkT.CreateSubstitutionLink(procMain.CodeNum,procAlwaysSub.ProcCode,SubstitutionCondition.Never,insPlan1.PlanNum);
			List<SubstitutionLink> listSubLinksForProcMain=new List<SubstitutionLink>() {subLinkForPlan1};
			long subCodeNumForPlan1=ProcedureCodes.GetSubstituteCodeNum(procMain.ProcCode,"5",insPlan1.PlanNum,listSubLinksForProcMain);
			Assert.AreEqual(procMain.CodeNum,subCodeNumForPlan1);//SubCondition==Never, so we return the main proc num
			long subCodeNumForPlan2=ProcedureCodes.GetSubstituteCodeNum(procMain.ProcCode,"5",insPlan2.PlanNum,listSubLinksForProcMain);
			Assert.AreEqual(procAlwaysSub.CodeNum,subCodeNumForPlan2);//different plan with no sub codes so we return the global if any
		}
	}
}
