using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Benefits_Test {
	[TestClass]
	public class BenefitsTest:TestBase {

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			CodeGroupT.ResetToDefaults();
		}

		///<summary>Creates a general deductible of $50, a deductible of $50 on D0220, sets a $30 D0220 complete and creates a claim, 
		///creates a $100 D2750, that is TP'ed, and then creates a $30 D0220 that is TP'ed.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_GetDeductibleByCode_DeductLessThanGeneral)]
		[Documentation.VersionAdded("17.2.34")]
		[Documentation.Description("A customer has an insurance plan with 100% coverage for Diagnostic work and 50% coverage for crowns. " +
			"They also have a $50 general deductible and a $50 deductible on D0220. They undergo a D0220 with a $30 fee, and a D2750 with a $100 fee. " +
			"Their deductible should cover the D0220, and the remaining deductible estimate for the D2750 should be $20.")]
		public void Benefits_GetDeductibleByCode_DeductLessThanGeneral() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceT.AddInsurance(pat,suffix);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<InsPlan> listPlans=InsPlans.RefreshForSubList(listSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			InsPlan plan=InsPlanT.GetPlanForPriSecMed(PriSecMed.Primary,listPatPlans,listPlans,listSubs);
			BenefitT.CreateDeductibleGeneral(plan.PlanNum,BenefitCoverageLevel.Individual,50);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,100);
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,50);
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.Diagnostic,0);
			BenefitT.CreateDeductible(plan.PlanNum,"D0220",50);
			List<Benefit> listBens=Benefits.Refresh(listPatPlans,listSubs);
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.C,"",30);//proc1 - Intraoral - periapical first film
			ClaimT.CreateClaim("P",listPatPlans,listPlans,new List<ClaimProc>(),new List<Procedure> { proc1 },pat,new List<Procedure> { proc1 },listBens,
				listSubs);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2750",ProcStat.TP,"",100,priority:0);//proc2 - Crown
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0220",ProcStat.TP,"",30,priority:1);//proc3 - Intraoral - periapical first film
			List<ClaimProc> claimProcs=ProcedureT.ComputeEstimates(pat,listPatPlans,listPlans,listSubs,listBens);
			ClaimProc claimProc2=claimProcs.FirstOrDefault(x => x.ProcNum==proc2.ProcNum);
			ClaimProc claimProc3=claimProcs.FirstOrDefault(x => x.ProcNum==proc3.ProcNum);
			Assert.AreEqual(20,claimProc2.DedEst);
		}

		///<summary>Creates an ortho procedure with an age limitation and a lifetime max and calculates the insurance estimate.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_UnderAgeWithLifetimeMax)]
		[Documentation.VersionAdded("17.3.67")]
		[Documentation.Description("A patient is 13 years old. Their insurance covers 50% of Ortho fees within the age limit of 18. " +
			"They also have an ortho lifetime max of $1000. They undergo a D8090 with a $3000 fee. Their insurance estimate should be $1000.")]
		public void Benefits_UnderAgeWithLifetimeMax() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate: DateTime.Now.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Orthodontics,50);
			BenefitT.CreateAgeLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.Orthodontics,18,coverageLevel:BenefitCoverageLevel.Individual);
			BenefitT.CreateOrthoMax(ins.PriInsPlan.PlanNum,1000);
			ins.RefreshBenefits();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.TP,"",3000);//comprehensive orthodontic treatment
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,ins);
			Assert.AreEqual(1000,listClaimProcs.First().InsEstTotal);
		}

		///<summary>Creates an ortho procedure with an age limitation and a lifetime max and calculates the insurance estimate.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_OverAgeWithLifetimeMax)]
		[Documentation.VersionAdded("17.3.67")]
		[Documentation.Description("A patient is 20 years old. Their insurance covers 50% of Ortho fees within the age limit of 18. " +
			"They also have an ortho lifetime max of $1000. They undergo a D8090 with a $3000 fee. Their insurance estimate should be $0.")]
		public void Benefits_OverAgeWithLifetimeMax() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate: DateTime.Now.AddYears(-20));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Orthodontics,50);
			BenefitT.CreateAgeLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.Orthodontics,18);
			BenefitT.CreateOrthoMax(ins.PriInsPlan.PlanNum,1000);
			ins.RefreshBenefits();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D8090",ProcStat.TP,"",3000);//comprehensive orthodontic treatment
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,ins);
			Assert.AreEqual(0,listClaimProcs.First().InsEstTotal);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_BitewingFrequency)]
		[Documentation.VersionAdded("18.2")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees, and have a frequency limitation of 1 D0274 procedure per calendar year. " +
			"Within the same year, they undergo two D0274 procedures, each with a $100 fee. Their insurance estimate for the first procedure should be $100. " +
			"The insurance estimate for the second procedure should be $0. " +
			"\r\nWithin that same year, they undergo a D0272 procedure with a $100 fee. No limit has been established for this code, so the procedure's " +
			"insurance estimate should be $100.")]
		public void Benefits_BitewingFrequency() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			ProcedureCodeT.AddIfNotPresent("D0272");
			ProcedureCodeT.AddIfNotPresent("D0274");
			CodeGroupT.Upsert("BitewingLimit","D0272,D0274",EnumCodeGroupFixed.BW);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D0274",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One D0274 per year
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.C,"",100);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.C,"",100);
			List<Procedure> listProcs=new List<Procedure> { proc1,proc2 };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			//Create another BW procedure that has nothing to do with D0274 other than being in the same code group.
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc3 };
			Claim claim2=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);//No frequency limitation is present for this code.
			ClaimT.ReceiveClaim(claim2,listClaimProcs);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_CancerScreeningFrequency)]
		[Documentation.VersionAdded("18.2")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees, and has a limits the frequency of D0431 procedures to 1 per calendar year. " +
			"They undergo a D0431 procedure with a $100 fee. The insurance estimate for this procedure should be $100. " +
			"\r\nWithin the same year, they undergo a D0431 again with the same $100 fee. This time, the insurance estimate for the procedure should be $0.")]
		public void Benefits_CancerScreeningFrequency() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D0431",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One Cancer Screening per year
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0431",ProcStat.C,"",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0431",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc2 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_CrownsFrequency)]
		[Documentation.VersionAdded("18.2")]
		[Documentation.Description("A patient's insurance covers 100% of Crown procedure fees, with a frequency limitation of 1 D2740 per calendar year. " +
			"They undergo two D2740 procedures, each with a $100 fee. The insurance estimate for the first procedure should be $100. The insurance estimate " +
			"for the second procedure should be $0.")]
		public void Benefits_CrownsFrequency() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Crowns,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D2740",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One crown per year
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.C,"11",100);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.C,"11",100);
			List<Procedure> listProcs=new List<Procedure> { proc1,proc2 };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
			ClaimT.ReceiveClaim(claim,listClaimProcs);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D2794",ProcStat.C,"11",100);//Different crown proc
			listProcs=new List<Procedure> { proc3 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);//Reached frequency limitation
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_SRPFrequency)]
		[Documentation.VersionAdded("18.2")]
		[Documentation.Description("A patient's insurance covers 100% of Periodontic fees, with a limit of one D4341 procedure per year. " +
			"They undergo a D4341 procedure with a $100 fee. The insurance estimate for this procedure should be $100." +
			"\r\nWithin the same year, the patient undergoes another D4341 procedure. This time, the procedure's insurance estimate should be $0.")]
		public void Benefits_SRPFrequency() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Periodontics,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D4341",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One SRP per year
			Procedure proc=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.C,"11",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.C,"11",100);
			listProcs=new List<Procedure> { proc2 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_SealantAgeLimit)]
		[Documentation.VersionAdded("18.2")]
		[Documentation.Description("A clinic's Sealant codegroup contains codes D1351 and D1206. Their patient is 13 years old. The patient's insurance covers " +
			"100% of their routine preventative procedure costs with an Age Limit of 12 years for procedures under the Sealant codegroup. " +
			"The patient undergoes a D1206 procedure, with a $100 fee. Their insurance estimate for the procedure should be $0.")]
		public void Benefits_SealantAgeLimit() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			ProcedureCodeT.AddIfNotPresent("D1351");
			ProcedureCodeT.AddIfNotPresent("D1206");
			CodeGroup codeGroup=CodeGroupT.Upsert("SealantAgeLimit","D1351,D1206",EnumCodeGroupFixed.Sealant,showInAgeLimit:true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			ins.ListBenefits.Add(BenefitT.CreateAgeLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.None,12,
				codeGroupNum:codeGroup.CodeGroupNum));//Sealant Age Limit 12 years old
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1206",ProcStat.C,"11",100);//Different procedure in the sealant code group
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First().InsPayEst);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_SealantAgeLimit_PatInAgeRangeHasCoverage)]
		[Documentation.VersionAdded("24.1")]
		[Documentation.Description("A clinic's Sealant codegroup contains codes D1351 and D1206. Their patient is 10 years old. The patient's insurance covers " +
			"100% of their routine preventative procedure costs with an Age Limit of 12 years for procedures under the Sealant codegroup. " +
			"The patient undergoes a D1351 procedure, with a $100 fee. Their insurance estimate for the procedure should be $100.")]
		public void Benefits_SealantAgeLimit_PatInAgeRangeHasCoverage() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-10));
			ProcedureCodeT.AddIfNotPresent("D1351"); // Sealant Code Group
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			CodeGroup codeGroup=CodeGroupT.Upsert("SealantAgeLimit","D1351,D1206",EnumCodeGroupFixed.Sealant,showInAgeLimit:true);
			Benefit benefitSealAgeLim = Benefits.CreateAgeLimitationBenefit(codeGroup.CodeGroupNum,12,ins.PriInsPlan.PlanNum); //Sealant Age Limit 12 years old
			ins.ListBenefits.Add(benefitSealAgeLim);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1351",ProcStat.C,"11",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_BitewingFrequencyCanada)]
		[Documentation.VersionAdded("18.2")]
		[Documentation.Description("A Canadian clinic has a patient whose insurance covers 100% of their diagnostic fees, with frequency limit of one 02144 " +
			"procedure per calendar year. The patient undergoes two 02144 procedures within the same year, each with a fee of $100. The insurance estimate for the " +
			"first procedure should be $100. The insurance estimate for the second procedure should be $0. " +
			"\r\nWithin the same year, the patient undergoes a 02143 procedure with a $100 fee. Because no limit was established for this procedure, " +
			"their insurance estimate should be $100.")]
		public void Benefits_BitewingFrequencyCanada() {
			CultureInfo curCulture=CultureInfo.CurrentCulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			ProcedureCodeT.AddIfNotPresent("02142");
			ProcedureCodeT.AddIfNotPresent("02143");
			ProcedureCodeT.AddIfNotPresent("02144");
			CodeGroupT.Upsert("BitewingFrequencyCanada","02142,02143,02144",EnumCodeGroupFixed.BW);
			CovCats.SetSpansToDefaultCanada();
			CovSpans.RefreshCache();
			try {
				string suffix=MethodBase.GetCurrentMethod().Name;
				Patient pat=PatientT.CreatePatient(suffix);
				InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
				ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
				ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"02144",BenefitQuantity.NumberOfServices,1,
					BenefitTimePeriod.CalendarYear));//One 02144 per year

				Procedure proc1=ProcedureT.CreateProcedure(pat,"02144",ProcStat.C,"",100);
				Procedure proc2=ProcedureT.CreateProcedure(pat,"02144",ProcStat.C,"",100);
				List<Procedure> listProcs=new List<Procedure> { proc1,proc2 };
				List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
				Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
				listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
				Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc1.ProcNum).InsPayEst);
				Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
				ClaimT.ReceiveClaim(claim,listClaimProcs);
				//Create another BW procedure that has nothing to do with 02144 other than being in the same code group.
				Procedure proc3=ProcedureT.CreateProcedure(pat,"02143",ProcStat.C,"",100);
				listProcs=new List<Procedure> { proc3 };
				ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
				System.Diagnostics.Debug.WriteLine(CultureInfo.CurrentCulture.Name);
				listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
				Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);//Reached frequency limitation
			}
			finally {
				Thread.CurrentThread.CurrentCulture=curCulture;
				CovCats.SetSpansToDefaultUsa();
				CovSpans.RefreshCache();
			}
		}

		///<summary>A Bitewing procedure is charted that is not the Bitewing procedure code used to designate frequency limitations. The next calendar 
		///year another Bitewing procedure is charted and verifies the frequency limitation applies to it.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_BitewingFrequencyPastYear)]
		[Documentation.VersionAdded("18.3.52")]
		[Documentation.Description("A patient's insurance covers 100% of their diagnostic fees with a frequency limit of one D0272 procedure every 12 months. " +
			"On 07/15/2018, the patient undergoes a D0272 procedure with a $100 fee. Their insurance estimate for this procedure should be $100. " +
			"\r\nSeven months later, on 02/15/2019, the patient undergoes another D0272 procedure. The insurance estimate for this procedure should be $0.")]
		public void Benefits_BitewingFrequencyPastYear() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			ProcedureCodeT.AddIfNotPresent("D0272");
			ProcedureCodeT.AddIfNotPresent("D0274");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D0272",BenefitQuantity.Months,12,
				BenefitTimePeriod.None));//One BW every 12 months
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.C,"",100,new DateTime(2018,07,15));
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.TP,"",100,new DateTime(2019,02,15));//Another BW procedure
			listClaimProcs=ProcedureT.ComputeEstimates(pat,ins,new DateTime(2019,02,15));
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		///<summary>Limitation frequency for BW. Frequency set to 2 per Calendar year. In this scenario, one procedure is completed on
		///October 1st. The insurance pays 50% of the procedure in the first check. They send a second check later for the rest of the procedure. This
		///is entered as a supplemental procedure. However, due to user error, this was entered in as a second received claimproc. When the second 
		///procedure is charted on October 31st, the insurance estimate should be for the full procedure cost. 
		///While the insurance did pay twice for the first procedure, it only counts as one towards the frequency limit regardless of status.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_TwoReceivedClaimProcsForSameClaim_FrequencyNotMet)]
		[Documentation.VersionAdded("18.3.52")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees, with a limit of two D0274 procedures per calendar year. " +
			"On October 1st, 2018 the patient undergoes a D0274 procedure with a $100 fee. The insurance makes payments in two checks. The first check is received " +
			"and entered properly. For the second check, a user error occurs and the payment is entered as a second received claimproc. " +
			"\r\nOn October 31st, 2018 the patient returns for another D0274 procedure with a fee of $100. The insurance estimate should still be $100. " +
			"Entering a payment as a second claimproc should not count against frequency limitations.")]
		public void Benefits_TwoReceivedClaimProcsForSameClaim_FrequencyNotMet() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.CalendarYear));
			//Add first procedure for the patient
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,1));
			//Create first claimproc. The insurance paid half
			ClaimProcT.AddInsPaid(pat.PatNum,ins.ListInsPlans.FirstOrDefault().PlanNum,proc1.ProcNum,50,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,0,0,new DateTime(2018,10,1));
			//Create second claimproc. The insurance paid second half. Also a received claimproc.
			ClaimProcT.AddInsPaid(pat.PatNum,ins.ListInsPlans.FirstOrDefault().PlanNum,proc1.ProcNum,50,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,0,0,new DateTime(2018,10,2));
			//Create second procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,31));
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc2 };
			//Create claim for the third procedure.
			Claim claim3=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Should be 100 as the first was paid for by insurance, regardless of there being two completed claimprocs.
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
		}

		///<summary>Limitation frequency for BW. Frequency set to 2 per Service year. In this scenario, the service year begins in april. 
		///Two procedures are completed in June and July which the insurance pays for. When another procedure is created in January of the following year
		///the insurance estimate should be 0 as they already met their two for the service year.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_ServiceYear_FrequencyMet)]
		[Documentation.VersionAdded("18.3.43")]
		[Documentation.Description("A patient's insurance covers 100% of their diagnostic fees. They have a limit of two D0274 procedures every service year, " +
			"with the service year starting in April. On June 1st, the patient undergoes a D0274 procedure with a $100 fee. " +
			"The insurance estimate for this procedure should be $100. On July 1st, the patient undergoes another D0274 procedure with a $100 fee. " +
			"The insurance estimate for this procedure should also be $100. " +
			"\r\nIn January of the next year, the patient undergoes a third D0274 procedure with a $100 fee. " +
			"The insurance estimate for this final procedure should be $0.")]
		public void Benefits_ServiceYear_FrequencyMet() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,monthRenew:4);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.ServiceYear));
			int yearPlanStart=(DateTime.Today.Month < 4 ? DateTime.Today.Year-1 : DateTime.Today.Year);
			//Add first procedure for the patient
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(yearPlanStart,6,1));
			//Create first claimproc. The insurance paid all
			ClaimProcT.AddInsPaid(pat.PatNum,ins.ListInsPlans.FirstOrDefault().PlanNum,proc1.ProcNum,100,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,0,0,new DateTime(yearPlanStart,6,1));
			//Create second procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(yearPlanStart,7,1));
			//Create second claimproc. The insurance paid all
			ClaimProcT.AddInsPaid(pat.PatNum,ins.ListInsPlans.FirstOrDefault().PlanNum,proc2.ProcNum,100,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,0,0,new DateTime(yearPlanStart,7,1));
			//Create third procedure
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(yearPlanStart+1,1,1));
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc3 };
			//Create claim for the third procedure.
			Claim claim3=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Should be 0 as the frequency has been met.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);
		}

		///<summary>Limitation frequency for BW. Frequency set to 2 per Service year. In this scenario, the service year begins in april. 
		///One procedure is completed in June which the insurance pays for. When two more procedures are created in January of the following year
		///the insurance estimate should only cover one of the two procedures on the claim.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_ServiceYear_FrequencyMet_SameClaim)]
		[Documentation.VersionAdded("21.3.39")]
		[Documentation.Description("A patient's insurance covers 100% of their diagnostic fees. They have a limit of two D0274 procedures per service year, with the service year starting in April. On June 1st, the patient undergoes a D0274 procedure with a fee of $100. The insurance estimate for this procedure should be $100." +
			"\r\nThe following January, the patient undergoes two more D0274 procedures, each having a $100 fee. These procedures end up on the same claim. " +
			"The first procedure on the claim should have an insurance estimate of $100. The second procedure on the claim should have an insurance estimate of $0.")]
		public void Benefits_ServiceYear_FrequencyMet_SameClaim() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,monthRenew:4);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.ServiceYear));
			int yearPlanStart=(DateTime.Today.Month < 4 ? DateTime.Today.Year-1 : DateTime.Today.Year);
			//Add first procedure for the patient in June.
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(yearPlanStart,6,1));
			//Create first claimproc. The insurance paid all.
			ClaimProcT.AddInsPaid(pat.PatNum,ins.ListInsPlans.FirstOrDefault().PlanNum,proc1.ProcNum,100,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,0,0,new DateTime(yearPlanStart,6,1));
			//Create second and third procedures.
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(yearPlanStart+1,1,1));
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(yearPlanStart+1,1,1));
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc2,proc3 };
			//Create claim for the third procedure.
			Claim claim3=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Insurance should cover 2-1=1 of these 2 procedures on this claim.
			Assert.AreEqual(100,listClaimProcs.Sum(x => x.InsPayEst));
		}

		///<summary>Limitation frequency for BW. Frequency set to 1 per Service year. In this scenario, the service year begins in June. 
		///One procedure is completed in within the service year which the insurance pays for and meets the limitation for BW. This test will look for coverage the next
		///following service year. This will test when someone changes the date to the next service year and clicks Refresh to re-compute estimates similar
		///to how users have that option in the TreatPlan module.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_ServiceYear_FrequencyMet_RefreshAsOfDate)]
		[Documentation.VersionAdded("18.4.48")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees. They are limited to one D0274 procedure every service year, " +
			"with the service year starting in June. The patient undergoes a D0274 procedure on July 1st, with a fee of $100. The insurance estimate for this " +
			"procedure should be $100. The patient then undergoes another D0274 on July 25th, with a fee of $100. The insurance estimate for this procedure " +
			"should be $0. A user changes the second procedure's date to be within the next service year. The procedure's insurance estimate should update to $100.")]
		public void Benefits_ServiceYear_FrequencyMet_RefreshAsOfDate() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix,monthRenew:6);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",1,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.ServiceYear));
			int yearPlanStart=(DateTime.Today.Month < 6 ? DateTime.Today.Year-1 : DateTime.Today.Year);
			int monthBetweenServiceYear=7;
			//Add first procedure for the patient
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.C,"",100,new DateTime(yearPlanStart,monthBetweenServiceYear,1));
			//Create first claimproc. The insurance paid all
			Claim claimReceived=ClaimT.CreateClaim(new List<Procedure> { proc1 },ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimT.ReceiveClaim(claimReceived,ins.ListAllClaimProcs.Where(x => x.ClaimNum==claimReceived.ClaimNum).ToList(),doSetInsPayAmt:true);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Create second procedure within the service year.
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.TP,"",100,new DateTime(yearPlanStart,monthBetweenServiceYear,25));
			List<ClaimProc> listClaimProcs=ProcedureT.ComputeEstimates(pat,ins);
			//Frequency met since it is within the service year. Coverage should be $0.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);
			//Change the procedure date to the next service year.
			int yearPlanNew=yearPlanStart+1;
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.TP,"",100,new DateTime(yearPlanNew,7,1));
			listClaimProcs=ProcedureT.ComputeEstimates(pat,ins);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);
		}

		///<summary>Validates that insurance plan deductible adjustments work.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductible)]
		[Documentation.VersionAdded("14.3.28")]
		[Documentation.Description(@"  Patient has one insurance plan, subscriber self, category percentage.  Benefits: Diagnostic 80%, General Deductible $50.  One TP procedure: D0120 perExam $200.  InsPlan adjustment: $50 deductible.  In the TP module, the proc should show that the patient owes $0 on the deductible.")]
		public void Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductible() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			//Create an annual max that won't get hit.
			BenefitT.CreateAnnualMax(plan.PlanNum,1000);
			//Create a Diagnostic benefit of 80%
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Diagnostic,80);
			//Create a $50 general deductible
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.General,50);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - PerExam
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.TP,"",200);
			//Add an insurance adjustment of $50 deductible.
			ClaimProc claimProcInsAdj=ClaimProcT.AddInsUsedAdjustment(pat.PatNum,plan.PlanNum,0,sub.InsSubNum,50);
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>() {
				new ClaimProcHist() {
					Amount=claimProcInsAdj.InsPayAmt,
					ClaimNum=claimProcInsAdj.ClaimNum,
					Deduct=claimProcInsAdj.DedApplied,
					InsSubNum=claimProcInsAdj.InsSubNum,
					PatNum=claimProcInsAdj.PatNum,
					PlanNum=claimProcInsAdj.PlanNum,
					ProcDate=claimProcInsAdj.ProcDate,
					ProcNum=claimProcInsAdj.ProcNum,
					Status=claimProcInsAdj.Status,
					StrProcCode="",
					Surf="",
					ToothNum="",
					ToothRange="",
				}
			};
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			double actualGeneralDeductible=Benefits.GetDeductibleByCode(benefitList,plan.PlanNum,patPlan.PatPlanNum,proc1.ProcDate,"D0120"
				,histList,loopList,plan,pat.PatNum);
			Assert.AreEqual(0,actualGeneralDeductible);
		}

		///<summary>Validates that insurance plan deductible work for ExclusionRule.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_GetDeductibleByCode_ExcludedDeductible)]
		[Documentation.VersionAdded("19.1.24")]
		[Documentation.Description("A patient's insurance covers 60% of crown fees and has a $50 general deductible. The patient undergoes " +
			"a D2740 procedure with a $1000 fee. The deductible for this procedure should be $50. " +
			"A user adds an exclusion for crowns. Now the deductible for this procedure should be $0.")]
		public void Benefits_GetDeductibleByCode_ExcludedDeductible() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			//Create a Crown benefit of 60%
			BenefitT.CreateCategoryPercent(plan.PlanNum,EbenefitCategory.Crowns,60);
			//Create a $50 general deductible
			BenefitT.CreateDeductible(plan.PlanNum,EbenefitCategory.General,50);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//proc1 - PerExam
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D2740",ProcStat.TP,"",1000);
			Family fam=Patients.GetFamily(pat.PatNum);
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<PatPlan> patPlans=PatPlans.Refresh(pat.PatNum);
			List<Benefit> benefitList=Benefits.Refresh(patPlans,subList);
			List<ClaimProcHist> histList=new List<ClaimProcHist>();
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			double actualGeneralDeductible=Benefits.GetDeductibleByCode(benefitList,plan.PlanNum,patPlan.PatPlanNum,proc1.ProcDate,"D2740"
				,histList,loopList,plan,pat.PatNum);
			Assert.AreEqual(50,actualGeneralDeductible);
			//Add an exclusion to the crown category.
			BenefitT.CreateExclusionForCategory(plan.PlanNum,EbenefitCategory.Crowns);
			//refresh the benefit list.
			benefitList=Benefits.Refresh(patPlans,subList);
			//Add eclusion rule to the insplan.
			plan.ExclusionFeeRule=ExclusionRule.UseUcrFee;
			//The new deductible should be $0 
			actualGeneralDeductible=Benefits.GetDeductibleByCode(benefitList,plan.PlanNum,patPlan.PatPlanNum,proc1.ProcDate,"D2740"
				,histList,loopList,plan,pat.PatNum);
			Assert.AreEqual(0,actualGeneralDeductible);
		}

		///<summary>Validates that insurance plan deductible adjustments apply to all types of deductibles (even code specific).</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductibleApplyToCodeSpecificDeductibles)]
		[Documentation.VersionAdded("19.1.21")]
		[Documentation.Description("A patient's insurance includes a $50 general deductible, a $0 diagnostic deductible, and a $50 " +
			"diagnostic x ray deductible. The patient undergoes a D1110 procedure with a fee of $100, and a D0270 procedure with a fee of $1100. " +
			"An adjustment is added to cover the $50 general deductible. The general deductible and the x ray deductible should both be $0.")]
		public void Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductibleApplyToCodeSpecificDeductibles() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			string procStr1="D1110";
			string procStr2="D0270";
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			//Set up insurance plan and the specific benefit setup required for this bug to occur.
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,carrier.CarrierName);
			//Create an individual general deductible of $50
			BenefitT.CreateDeductible(insInfo.PriInsPlan.PlanNum,EbenefitCategory.General,50);
			//Create a individual general deductible of $0
			BenefitT.CreateDeductible(insInfo.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,0);
			//Create an individual x-ray deductible of $50
			BenefitT.CreateDeductible(insInfo.PriInsPlan.PlanNum,EbenefitCategory.DiagnosticXRay,50);
			//Create TP procedures. For this particular bug we need 1 general proc and 1 x-ray procedure. The general proc must be a higher priority than the x-ray proc.
			Procedure generalProc=ProcedureT.CreateProcedure(pat,procStr1,ProcStat.TP,"",100,procDate:DateTime.Now,priority:1);
			Procedure xrayProc=ProcedureT.CreateProcedure(pat,procStr2,ProcStat.TP,"2",1100,procDate:DateTime.Now,priority:2);
			//Add an adjustment to the insurance benefits to cover the $50 general deductible
			ClaimProc adjClaimProc=ClaimProcT.AddInsUsedAdjustment(pat.PatNum,insInfo.PriInsPlan.PlanNum,0,insInfo.PriInsSub.InsSubNum,50);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			List<InsSub> listSubs=InsSubT.GetInsSubs(pat);
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listSubs);
			//Create the ClaimProcHist object for our adjustment claimproc
			ClaimProcHist insDeductAdj=new ClaimProcHist() {
				Amount=adjClaimProc.InsPayAmt,
				ClaimNum=adjClaimProc.ClaimNum,
				Deduct=adjClaimProc.DedApplied,
				InsSubNum=adjClaimProc.InsSubNum,
				PatNum=adjClaimProc.PatNum,
				PlanNum=adjClaimProc.PlanNum,
				ProcDate=adjClaimProc.ProcDate,
				ProcNum=adjClaimProc.ProcNum,
				Status=adjClaimProc.Status,
				StrProcCode="",
				Surf="",
				ToothNum="",
				ToothRange="",
			};
			//Calculate the deductible remaining for our general procedure
			double generalDeductible=Benefits.GetDeductibleByCode(listBenefits,insInfo.PriInsPlan.PlanNum,insInfo.PriPatPlan.PatPlanNum,generalProc.ProcDate,procStr1
				,new List<ClaimProcHist>() { insDeductAdj },new List<ClaimProcHist>(),InsPlans.GetPlan(insInfo.PriInsPlan.PlanNum,null),pat.PatNum);
			//Create a ClaimProcHist object from our general procedure to use as the looplist when calculating the code specific deductible
			ClaimProcHist generalProcHist=new ClaimProcHist() {
				Amount=109,
				ClaimNum=0,
				Deduct=0,
				InsSubNum=insInfo.PriInsSub.InsSubNum,
				PatNum=pat.PatNum,
				PlanNum=insInfo.PriInsPlan.PlanNum,
				ProcDate=generalProc.ProcDate,
				ProcNum=0,
				Status=ClaimProcStatus.Estimate,
				StrProcCode="D1110",
				Surf="",
				ToothNum="",
				ToothRange=""
			};
			//Calculate the deductible remaining for our xray procedure
			double xrayDeductible=Benefits.GetDeductibleByCode(listBenefits,insInfo.PriInsPlan.PlanNum,insInfo.PriPatPlan.PatPlanNum,generalProc.ProcDate,procStr2
				,new List<ClaimProcHist>() { insDeductAdj },new List<ClaimProcHist>(){ generalProcHist },InsPlans.GetPlan(insInfo.PriInsPlan.PlanNum,null),pat.PatNum);
			Assert.AreEqual(generalDeductible,0);
			Assert.AreEqual(xrayDeductible,0);
        }

		#region InLast12Months

		///<summary>Limitation frequency for BW. Frequency set to 2 in the Last 12 months. In this scenario, two procedures are completed in
		///October and December. When the third is charted in May of the following year, the insurance estimate will be 0.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_InLast12Months_FrequencyMetBasic)]
		[Documentation.VersionAdded("19.1")]
		[Documentation.Description("A patient's insurance covers 100% of their diagnostic fees. The patient is limited to two D0274 " +
			"procedures every 12 months. The patient undergoes a D0274 procedure on October 28th and December 28th of 2018. These procedures " +
			"each had a fee of $100. Then, on May 1st of 2019, the patient comes back for another D0274 procedure. The insurance estimate " +
			"for this third procedure should be $0.")]
		public void Benefits_InLast12Months_FrequencyMetBasic() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.NumberInLast12Months));
			//Add two procedures for the patient
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,28));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,12,28));
			//Create two Claimprocs with status InsHist
			ClaimProc claimProc1=ClaimProcT.CreateClaimProc(pat.PatNum,proc1.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,new DateTime(2018,10,28),-1,-1,-1,ClaimProcStatus.InsHist);
			ClaimProc claimProc2=ClaimProcT.CreateClaimProc(pat.PatNum,proc2.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,new DateTime(2018,12,28),-1,-1,-1,ClaimProcStatus.InsHist);
			//Complete a Procedure for the patient
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2019,5,1));
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc3 };
			//Generate Claim
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);//Reached frequency limitation
		}

		///<summary>Limitation frequency for BW. Frequency set to 2 in the Last 12 months. In this scenario, two procedures are completed on
		///October 1st and 2nd. When the third is charted in October 1st of the following year, the insurance estimate will be non-0.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_InLast12Months_FrequencyNotMetBasic)]
		[Documentation.VersionAdded("19.1")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees. The patient is limited to two D0274 procedures " +
			"every 12 months. The patient undergoes a D0274 procedure on October 1st and October 2nd of 2018. A year later, on October 1st 2019, " +
			"the patient undergoes another D0274 procedure with a $100 fee. The insurance estimate for this procedure should be $100. ")]
		public void Benefits_InLast12Months_FrequencyNotMetBasic() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.NumberInLast12Months));
			//Add two procedures for the patient
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,1));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,2));
			//Create two Claimprocs with status InsHist
			ClaimProc claimProc1=ClaimProcT.CreateClaimProc(pat.PatNum,proc1.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,new DateTime(2018,10,1),-1,-1,-1,ClaimProcStatus.InsHist);
			ClaimProc claimProc2=ClaimProcT.CreateClaimProc(pat.PatNum,proc2.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,new DateTime(2018,10,2),-1,-1,-1,ClaimProcStatus.InsHist);
			//Complete a Procedure for the patient
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2019,10,1));
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc3 };
			//Generate Claim
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Should pay 100. The 10/1/2018 procedure is 12 months in the past exactly.
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);
		}

		///<summary>This method specifically shows that $0 procedures do not count against the frequency limit.
		///Frequency set to 2 in the Last 12 months. In this scenario, two procedures are completed on
		///October 1st and 2nd. When the third is charted on October 3rd of the same year, the insurance estimate will be 0 as the frequency
		///has been met. When the fourth procedure is charted on October 1st of the following year, the insurance estimate will be non-0 because
		///the insurance was not billed for proc3. Meaning, even though the patient has received 3 of these procedures in the past 12 months, the 
		///10/01/19 procedure will be paid for because the 10/03/2018 visit did not count towards the limit.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_InLast12Months_FrequencyMetWithTwoProcsInLast12Months)]
		[Documentation.VersionAdded("19.1")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees. The patient is limited to two D0274 procedures every 12 months. " +
			"The patient goes in for a D0274 procedure on October 1st and 2nd of 2018, each procedure having a fee of $100. They go in for a third D0274 " +
			"procedure on October 3rd 2018, with a $100 fee. The insurance estimate for the third procedure should be $0. On October 1st 2019, the patient " +
			"goes back in for another D0274 procedure with a $100 fee. The insurance estimate for this procedure should be $100. A procedure that was not " +
			"covered by insurance should not count against a limit.")]
		public void Benefits_InLast12Months_FrequencyMetWithTwoProcsInLast12Months() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,ins.PriInsPlan.PlanNum,
				BenefitTimePeriod.NumberInLast12Months));
			//Add three procedures for the patient
			Procedure proc1=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,1));
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,2));
			Procedure proc3=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2018,10,3));
			//Create two Claimprocs with status InsHist for the first two
			ClaimProc claimProc1=ClaimProcT.CreateClaimProc(pat.PatNum,proc1.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,new DateTime(2018,10,1),-1,-1,-1,ClaimProcStatus.InsHist);
			ClaimProc claimProc2=ClaimProcT.CreateClaimProc(pat.PatNum,proc2.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,new DateTime(2018,10,2),-1,-1,-1,ClaimProcStatus.InsHist);
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc3 };
			//Create claim for the third procedure.
			Claim claim3=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Should be 0 as the first two were paid for by the insurance
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);
			ClaimT.ReceiveClaim(claim3,listClaimProcs.Where(x => x.ProcNum==proc3.ProcNum).ToList(),doSetInsPayAmt:true);
			Procedure proc4=ProcedureT.CreateProcedure(pat,"D0274",ProcStat.EC,"",100,new DateTime(2019,10,1));
			listClaimProcs=new List<ClaimProc> { };
			listProcs=new List<Procedure> { proc4 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//proc4 is covered because proc1 is too old to matter and proc3 was not paid, so only proc2 counts against the limit.
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc4.ProcNum).InsPayEst);
		}

		#endregion

		#region InsHist
		///<summary>Limitation frequency for the BW category has been met with an EO(InsHist claimproc). Test that a new completed procedure does not 
		///calculate insurance estimates. </summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_InsHistBitewingFrequency)]
		[Documentation.VersionAdded("18.4")]
		[Documentation.Description("A patient's insurance covers 100% of diagnostic fees. The patient is limited to one D0272 procedure per calendar year. " +
			"The patient underwent a D0272 procedure at an outside clinic. Within the same year, the patient undergoes a D0272 at the user's clinic," +
			" with a fee of $100. The insurance estimate for this procedure should be $0. Outside visits should count towards limits.")]
		public void Benefits_InsHistBitewingFrequency() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			ProcedureCodeT.AddIfNotPresent("D0272");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D0272",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One BW per year
			//Add an EO procedure for a BW
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.EO,"",0);
			//Create a Claimproc with status InsHist
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.InsHist);
			//Complete a new BW procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0272",ProcStat.C,"",100);
			priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc2.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1);
			List<Procedure> listProcs=new List<Procedure> { proc2 };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			//Create the claim for the complete BW procedure
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//The InsPayEst should be zero becauase a InsHist claimproc for BW procedures was created.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		///<summary>Limitation frequency for the exam category has been met with an EO(InsHist claimproc). Test that a new completed procedure does 
		///not calculate insurance estimates. </summary>
		[TestMethod]
		public void Benefits_InsHistExamFrequency() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D0120",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One Exam per year
			//Add an EO procedure for a Exam
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.EO,"",00);
			//Create a Claimproc with status InsHist
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.InsHist);
			//Complete a new Exam procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",100);
			priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc2.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1);
			List<Procedure> listProcs=new List<Procedure> { proc2 };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			//Create the claim for the complete Exam procedure
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//The InsPayEst should be zero becauase a InsHist claimproc for Exam procedures was created.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		///<summary>Limitation frequency for the prophy category has been met with an EO(InsHist claimproc). Test that a new completed procedure does 
		///not calculate insurance estimates. </summary>
		[TestMethod]
		public void Benefits_InsHistProphyFrequency() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D1110",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One Exam per year
			//Add an EO procedure for a D1110
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.EO,"",00);
			//Create a Claimproc with status InsHist
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.InsHist);
			//Complete a new D1110 procedure
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",100);
			priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc2.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1);
			List<Procedure> listProcs=new List<Procedure> { proc2 };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			//Create the claim for the complete D1110 procedure
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//The InsPayEst should be zero becauase a InsHist claimproc for D1110 procedures was created.
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		///<summary>Limitation frequency for the perio scaling UL category has been met with an EO(InsHist claimproc). Test that a new completed 
		///procedure does not calculate insurance estimates. </summary>
		[TestMethod]
		public void Benefits_InsHistPerioScalingULFrequency() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("D4341");
			if(procCode.TreatArea!=TreatmentArea.Quad) {
				procCode.TreatArea=TreatmentArea.Quad;
				ProcedureCodes.Update(procCode);
				ProcedureCodes.RefreshCache();
			}
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Periodontics,100));
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"D4341",BenefitQuantity.NumberOfServices,1,
				BenefitTimePeriod.CalendarYear));//One SRP per year
			//Add an EO procedure for SRP with "UL" surface
			Procedure proc=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.EO,"",0,surf:"UL");
			//Create a Claimproc with status InsHist
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,ins.ListInsPlans.FirstOrDefault().PlanNum,
				ins.ListInsSubs.FirstOrDefault().InsSubNum,DateTime.Today,-1,-1,-1,ClaimProcStatus.InsHist);
			//Complete a new SRP procedure with surface UL
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D4341",ProcStat.C,"",100,surf:"UL");
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			List<Procedure> listProcs=new List<Procedure> { proc2 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}
		
		#endregion

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_GetAnnualMaxDisplay_CalendarYearBenefit)]
		[Documentation.Description("This Unit Test mimics an old bug scenario")]
		public void Benefits_GetAnnualMaxDisplay_CalendarYearBenefit() {
			//Joe - This Unit Test mimics an old bug scenario described in task# 809503.
			string suffix="74";
			Patient pat=PatientT.CreatePatient(suffix);
			Carrier carrier=CarrierT.CreateCarrier(suffix);
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			long subNum=sub.InsSubNum;
			//Patient Annual Max.
			BenefitT.CreateAnnualMax(plan.PlanNum,15000);
			//Procedure sub set benefit.
			Benefit ben=new Benefit();
			ben.PlanNum=plan.PlanNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=1;
			ben.CoverageLevel=BenefitCoverageLevel.Individual;
			ben.MonetaryAmt=1399;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			List<Benefit> listPatBenefits=Benefits.GetForPlanOrPatPlan(plan.PlanNum,patPlan.PatPlanNum);
			Assert.AreEqual(15000,Benefits.GetAnnualMaxDisplay(listPatBenefits,plan.PlanNum,patPlan.PatPlanNum,false));
		}

		/// <summary>validates that procedure specific waiting periods are not overwritten by category specific wait periods</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_ProcedureCodeWaitingPerdiodOverride)]
		[Documentation.VersionAdded("21.2.25")]
		[Documentation.Description("A patient has waiting periods on their insurance. There is a category-specific waiting period of two months for " +
			"Routine Preventative procedures, and a code-specific waiting period of 0 months for D1351 procedures. The patient undergoes a D1351 " +
			"procedure, which is within the category-specific waiting period but outside the code-specific waiting period. The category-specific " +
			"waiting period should not take precedent over the code-specific waiting period. The patient should not be considered in a waiting period. ")]
		public void Benefits_ProcedureCodeWaitingPerdiodOverride() {
			Patient pat=PatientT.CreatePatient();
			Carrier carrier=CarrierT.CreateCarrier("Delta");
			InsPlan plan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub sub=InsSubT.CreateInsSub(pat.PatNum,plan.PlanNum);
			sub.DateEffective = DateTime.Parse("2021-07-01"); // need stable effective date to calculate wait period consistently
			long subNum=sub.InsSubNum;
			//create span specific waiting period
			Benefit spanWaitPeriod = new Benefit();
			spanWaitPeriod.PlanNum = plan.PlanNum;
			spanWaitPeriod.BenefitType=InsBenefitType.WaitingPeriod;
			spanWaitPeriod.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
			spanWaitPeriod.CoverageLevel=0;
			spanWaitPeriod.TimePeriod=BenefitTimePeriod.CalendarYear;
			spanWaitPeriod.Quantity=2;
			spanWaitPeriod.QuantityQualifier=BenefitQuantity.Months;
			Benefits.Insert(spanWaitPeriod);
			//create code specific waiting period
			Benefit codeWaitPeriod = new Benefit();
			codeWaitPeriod.PlanNum = plan.PlanNum;
			codeWaitPeriod.BenefitType=InsBenefitType.WaitingPeriod;
			codeWaitPeriod.CovCatNum=0;
			codeWaitPeriod.CoverageLevel=BenefitCoverageLevel.Individual;
			codeWaitPeriod.TimePeriod=BenefitTimePeriod.CalendarYear;
			codeWaitPeriod.Quantity=0;
			codeWaitPeriod.QuantityQualifier=BenefitQuantity.Months;
			codeWaitPeriod.CodeNum=ProcedureCodes.GetCodeNum("D1351");
			Benefits.Insert(codeWaitPeriod);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,pat.PatNum,subNum);
			//create proc that falls in category and matches benefit code
			Procedure proc = new Procedure();
			proc.CodeNum=ProcedureCodes.GetCodeNum("D1351");
			proc.ProcFee=100.00;
			proc.PatNum=pat.PatNum;
			proc.ProcStatus=ProcStat.C;
			proc.ProcDate=DateTime.Parse("2021-07-15");// within 2 months from effective date, falls within category wait period, but after code specific period
			//create claimproc for this proc
			ClaimProc cp = new ClaimProc();
			cp.PatNum=pat.PatNum;
			cp.ProcNum=proc.ProcNum;
			cp.ProcDate=proc.ProcDate;
			cp.PlanNum=plan.PlanNum;
			cp.Status=ClaimProcStatus.Estimate;
			cp.InsSubNum=sub.InsSubNum;
			cp.EstimateNote="";
			List<ClaimProc> listClaimProcs = new List<ClaimProc>(){ cp };
			List<Benefit> listPatBenefits=Benefits.GetForPlanOrPatPlan(plan.PlanNum,patPlan.PatPlanNum);
			Procedures.ComputeEstimates(proc, pat.PatNum,listClaimProcs, true, new List<InsPlan> { plan }, new List<PatPlan> { patPlan },listPatBenefits,pat.Age,new List<InsSub> { sub });
			bool hasNoWaitNote=false;
			foreach(ClaimProc claimproc in listClaimProcs) {
				hasNoWaitNote=!claimproc.EstimateNote.Contains("Waiting Period");
				Assert.IsTrue(hasNoWaitNote);
			}
		}

		///<summary>Adds patient level override limitation before the plan level limitation. Sorting will prioritize patient level overrides.</summary>
		[TestMethod]
		public void Benefits_CompareTo_PrioritizePatientLevelOverrides_OverrideFirst() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add override frequency limit of 2 for procedure D0274
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.CalendarYear,insInfo.PriPatPlan.PatPlanNum)
			);
			//add a frequency limit of 1 for procedure D0274
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D0274",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.CalendarYear)
			);
			//No sorting has occurred, first item will have a PlanNum and no PatPlanNum since it is a plan level limitation.
			Assert.AreNotEqual(0,insInfo.ListBenefits.First().PatPlanNum);
			Assert.AreEqual(0,insInfo.ListBenefits.First().PlanNum);
			//Sorts list, prioritizes patient level overrides over plan level limitations.
			insInfo.ListBenefits=Benefits.GetForPlanOrPatPlan(insInfo.PriInsPlan.PlanNum,insInfo.PriPatPlan.PatPlanNum);
			//Sorting has occurred, first item will have a PatPlanNum and no PlanNum since it is a patient level override.
			Assert.AreNotEqual(0,insInfo.ListBenefits.First().PatPlanNum);
			Assert.AreEqual(0,insInfo.ListBenefits.First().PlanNum);
		}
		
		///<summary>Adds patient level override limitation after the plan level limitation. Sorting will prioritize patient level overrides.</summary>
		[TestMethod]
		public void Benefits_CompareTo_PrioritizePatientLevelOverrides_PlanFirst() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.ClaimProcsAllowedToBackdate,true);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo insInfo=InsuranceT.AddInsurance(pat,suffix);
			//add a frequency limit of 1 for procedure D0274
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D0274",1,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.CalendarYear)
			);
			//add override frequency limit of 2 for procedure D0274
			insInfo.ListBenefits.Add(
				BenefitT.CreateFrequencyLimitation("D0274",2,BenefitQuantity.NumberOfServices,insInfo.PriInsPlan.PlanNum,BenefitTimePeriod.CalendarYear,insInfo.PriPatPlan.PatPlanNum)
			);
			//No sorting has occurred, first item will have a PlanNum and no PatPlanNum since it is a plan level limitation.
			Assert.AreEqual(0,insInfo.ListBenefits.First().PatPlanNum);
			Assert.AreNotEqual(0,insInfo.ListBenefits.First().PlanNum);
			//Sorts list, prioritizes patient level overrides over plan level limitations.
			insInfo.ListBenefits=Benefits.GetForPlanOrPatPlan(insInfo.PriInsPlan.PlanNum,insInfo.PriPatPlan.PatPlanNum);
			//Sorting has occurred, first item will have a PatPlanNum and no PlanNum since it is a patient level override.
			Assert.AreNotEqual(0,insInfo.ListBenefits.First().PatPlanNum);
			Assert.AreEqual(0,insInfo.ListBenefits.First().PlanNum);
		}

		[TestMethod]
		public void Benefits_Frequency_ProcCodeNoCodeGroup() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			//Clear out all code groups.
			CodeGroupT.ClearCodeGroupTable();
			ProcedureCodeT.AddIfNotPresent("PCNCG");
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Cover 100% of the procedure code.
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("PCNCG"),100));
			//Allow 1 procedure a year.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"PCNCG",BenefitQuantity.NumberOfServices,1,BenefitTimePeriod.CalendarYear));
			Procedure proc=ProcedureT.CreateProcedure(pat,"PCNCG",ProcStat.C,"",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"PCNCG",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc2 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		[TestMethod]
		public void Benefits_Frequency_MultipleCodeGroupsWithSameProcCode() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			//Make sure there are at least two separate code groups have the same procedure code.
			ProcedureCodeT.AddIfNotPresent("MCGWSPC");
			CodeGroup codeGroup1=CodeGroupT.Upsert("GroupMCGWSPC-1","MCGWSPC");
			CodeGroup codeGroup2=CodeGroupT.Upsert("GroupMCGWSPC-2","MCGWSPC");
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Cover 100% of the procedure code.
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("MCGWSPC"),100));
			//Allow 1 procedure a year for codeGroup1.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyCodeGroup(ins.PriInsPlan.PlanNum,codeGroup1.CodeGroupNum,BenefitQuantity.NumberOfServices,1,BenefitTimePeriod.CalendarYear));
			//Allow 2 procedure a year for codeGroup2.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyCodeGroup(ins.PriInsPlan.PlanNum,codeGroup2.CodeGroupNum,BenefitQuantity.NumberOfServices,2,BenefitTimePeriod.CalendarYear));
			Procedure proc=ProcedureT.CreateProcedure(pat,"MCGWSPC",ProcStat.C,"",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"MCGWSPC",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc2 };
			Claim claim2=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
			ClaimT.ReceiveClaim(claim2,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"MCGWSPC",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc3 };
			Claim claim3=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);//Still at frequency limitation
		}

		[TestMethod]
		public void Benefits_Frequency_ProcCodeAndCodeGroup() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			//Clear out all code groups.
			CodeGroupT.ClearCodeGroupTable();
			ProcedureCodeT.AddIfNotPresent("PCACG1");
			ProcedureCodeT.AddIfNotPresent("PCACG2");
			ProcedureCodeT.AddIfNotPresent("PCACG3");
			CodeGroup codeGroup=CodeGroupT.Upsert("GroupPCACG-1","PCACG1,PCACG2,PCACG3");
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Cover 100% of each procedure code.
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("PCACG1"),100));
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("PCACG2"),100));
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("PCACG3"),100));
			//Allow 1 procedure a year.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"PCACG1",BenefitQuantity.NumberOfServices,1,BenefitTimePeriod.CalendarYear));
			//Allow 3 code groups a year.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyCodeGroup(ins.PriInsPlan.PlanNum,codeGroup.CodeGroupNum,BenefitQuantity.NumberOfServices,3,BenefitTimePeriod.CalendarYear));
			Procedure proc=ProcedureT.CreateProcedure(pat,"PCACG1",ProcStat.C,"",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"PCACG1",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc2 };
			Claim claim2=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
			ClaimT.ReceiveClaim(claim2,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"PCACG2",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc3 };
			Claim claim3=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);//However, we haven't reached frequency limitation for the code group yet.
			ClaimT.ReceiveClaim(claim3,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc4=ProcedureT.CreateProcedure(pat,"PCACG3",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc4 };
			Claim claim4=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc4.ProcNum).InsPayEst);//However, we haven't reached frequency limitation for the code group yet.
			Procedure proc5=ProcedureT.CreateProcedure(pat,"PCACG3",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc5 };
			Claim claim5=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First(x => x.ProcNum==proc5.ProcNum).InsPayEst);//Reached frequency limitation for the code group now.
		}

		[TestMethod]
		public void Benefits_Frequency_ProcCodeStartsWithCodeGroup() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			//Clear out all code groups.
			CodeGroupT.ClearCodeGroupTable();
			ProcedureCodeT.AddIfNotPresent("PCSWCG");
			ProcedureCodeT.AddIfNotPresent("PCSWCG.1");
			//Create a CodeGroup that contains only one code; 'PCSWCG'
			CodeGroup codeGroup=CodeGroupT.Upsert("GroupPCSWCG","PCSWCG");
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Cover 100% of the procedure code.
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("PCSWCG.1"),100));
			//Allow 1 code group a year.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyCodeGroup(ins.PriInsPlan.PlanNum,codeGroup.CodeGroupNum,BenefitQuantity.NumberOfServices,1,BenefitTimePeriod.CalendarYear));
			//Frequency limitation benefits should match procedure codes based on how they start, not via exact match.
			//Schedule two PCSWCG.1 codes and make sure the frequency limitation is reached.
			Procedure proc=ProcedureT.CreateProcedure(pat,"PCSWCG.1",ProcStat.C,"",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First().InsPayEst);
			ClaimT.ReceiveClaim(claim,listClaimProcs,doSetInsPayAmt:true);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"PCSWCG.1",ProcStat.C,"",100);
			listProcs=new List<Procedure> { proc2 };
			ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,listClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
		}

		[TestMethod]
		public void Benefits_Frequency_HiddenCodeGroup() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			//Clear out all code groups.
			CodeGroupT.ClearCodeGroupTable();
			ProcedureCodeT.AddIfNotPresent("HCG01");
			CodeGroup codeGroup=CodeGroupT.Upsert("GroupHCG01","HCG01",isHidden:false);
			Patient pat=PatientT.CreatePatient(suffix);
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Cover 100% of the procedure code.
			ins.ListBenefits.Add(BenefitT.CreatePercentForProc(ins.PriInsPlan.PlanNum,ProcedureCodes.GetCodeNum("HCG01"),100));
			//Allow 1 code group a year.
			ins.ListBenefits.Add(BenefitT.CreateFrequencyCodeGroup(ins.PriInsPlan.PlanNum,codeGroup.CodeGroupNum,BenefitQuantity.NumberOfServices,1,BenefitTimePeriod.CalendarYear));
			Procedure proc=ProcedureT.CreateProcedure(pat,"HCG01",ProcStat.C,"",100);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"HCG01",ProcStat.C,"",100);
			List<Procedure> listProcedures=new List<Procedure>() { proc,proc2 };
			ins.ComputeEstimatesForProcs(listProcedures);
			ins.CreateClaim();
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(100,ins.ListAllClaimProcs.First(x => x.ProcNum==proc.ProcNum).InsPayEst);
			Assert.AreEqual(0,ins.ListAllClaimProcs.First(x => x.ProcNum==proc2.ProcNum).InsPayEst);//Reached frequency limitation
			//Hide the code group and assert that frequency limitation is still reached for an additional procedure.
			codeGroup=CodeGroupT.Upsert(codeGroup.GroupName,codeGroup.ProcCodes,isHidden:true);
			Procedure proc3=ProcedureT.CreateProcedure(pat,"HCG01",ProcStat.C,"",100);
			ClaimT.CreateClaim(new List<Procedure> { proc3 },ins);
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,ins.ListAllClaimProcs.First(x => x.ProcNum==proc3.ProcNum).InsPayEst);
		}

		///<summary>A procedure exists with an ADA code of D1201, and is part of the Sealant codegroup.
		///Another procedure exists with an ADA code of D1201.1, and is not part of the Sealant codegroup.
		///A patient with insurance has an age limit of 14 years for the Sealant codegroup.
		///The procedure that is not part of the Sealants codegroup should not have its insurance estimates
		///affected by the patient's Sealant age limit.</summary>
		[Documentation.Numbering(Documentation.EnumTestNum.Benefits_AgeLimits_CodeGroupOverlap)]
		[Documentation.VersionAdded("24.1")]
		[Documentation.Description(
		@"A procedure exists with an ADA code of D1201, and is part of the Sealant codegroup.
		Another procedure exists with an ADA code of D1201.1, and is not part of the Sealant codegroup.
		A patient with insurance has an age limit of 14 years for the Sealant codegroup.
		The procedure that is not part of the Sealants codegroup should not have its insurance estimates
		affected by the patient's Sealant age limit.")]
		[TestMethod]
		public void Benefits_AgeLimits_CodeGroupOverlap() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			CodeGroupT.ClearCodeGroupTable();
			//Create a procedure code that has the same first five characters as an existing code in a code group.
			ProcedureCodeT.AddIfNotPresent("D1201");
			ProcedureCodeT.AddIfNotPresent("D1201.1");
			long codeNum=ProcedureCodes.GetCodeNum("D1201.1");
			CodeGroup codeGroup=CodeGroupT.Upsert("AgeLimGroup","D1201",EnumCodeGroupFixed.Sealant);
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Now.AddYears(-30));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Add an age limitation benefit for the procedure in the new code group.
			ins.ListBenefits.Add(BenefitT.CreateAgeLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.None,13,codeGroupNum:codeGroup.CodeGroupNum));
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1201",ProcStat.C,"",100);
			Procedure proc2=ProcedureT.CreateProcedure(pat,"D1201.1",ProcStat.C,"",100);
			List<Procedure> listProcedures=new List<Procedure>{ proc, proc2 };
			ins.ComputeEstimatesForProcs(listProcedures);
			ins.CreateClaim();
			ins.ListAllClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//The patient's age limit should not impact the coverage of a benefit outside that code group.
			Assert.AreEqual(0,ins.ListAllClaimProcs.Find(x=>x.ProcNum==proc.ProcNum).InsPayEst);
			Assert.AreEqual(100,ins.ListAllClaimProcs.Find(x=>x.ProcNum==proc2.ProcNum).InsPayEst);
		}

		[TestMethod]
		public void Benefits_AgeLimit_CodeGroupNotShownInAgeLimit() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13)); // 13 year old patient
			ProcedureCodeT.AddIfNotPresent("D1351");
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			CodeGroup codeGroup=CodeGroupT.Upsert("SealantAgeLimit","D1351,D1206",EnumCodeGroupFixed.Sealant,showInAgeLimit:false); // CodeGroup not showing in Age Limit grid
			Benefit benefitSealAgeLim = Benefits.CreateAgeLimitationBenefit(codeGroup.CodeGroupNum,12,ins.PriInsPlan.PlanNum); //Sealant Age Limit 12 years old
			ins.ListBenefits.Add(benefitSealAgeLim);
			ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.RoutinePreventive,100));
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1351",ProcStat.C,"11",100);
			List<Procedure> listProcs=new List<Procedure> { proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			Assert.AreEqual(0,listClaimProcs.First().InsPayEst); // Should be aged out.
		}

		/// <summary>Asserts that GetLimitationByCode returns accurate estimates when there is a family annual max and no individual annual max.</summary>
		[TestMethod]
		public void Benefits_GetLimitationByCode_FamilyMaxSet_IndividualMaxNotSet_InsEstOverrideNotSet() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string name="Test";
			Patient patient=PatientT.CreatePatient(name);
			Carrier carrier=CarrierT.CreateCarrier(name);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,patient.PriProv,insSub.InsSubNum);
			string procCodePartCovered="D1111";
			string procCodeNotCovered="D2222";
			double insEstTotal=60;
			double insEstTotal2=25;
			int patientAge=19;
			//Family Annual Max.
			Benefit benefitFamilyMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,50);
			//Individual Annual Max is not set.
			Benefit benefitIndividualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,-1);
			//Age Limitation Fluoride
			Benefit benefitFluoride=BenefitT.CreateAgeLimitation(insPlan.PlanNum,EbenefitCategory.RoutinePreventive,19,BenefitCoverageLevel.Individual);
			List<Benefit> listBenefits=new List<Benefit>{ benefitIndividualMax,benefitFluoride,benefitFamilyMax };
			//Create 1 procedure that will only be partially covered
			Procedure procedurePartCovered=ProcedureT.CreateProcedure(patient,procCodePartCovered,ProcStat.TP,"",100,DateTime.Now);
			//Create 1 procedure that will be over family max
			Procedure procedureNotCovered=ProcedureT.CreateProcedure(patient,procCodeNotCovered,ProcStat.TP,"",50,DateTime.Now);
			string note="";
			LimitationTypeMet limitationTypeMet;
			double coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodePartCovered,new List<ClaimProcHist>(),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal,patientAge,insSub.InsSubNum,-1,out limitationTypeMet);
			//Only $50 of the $100 procedure will be covered since $50 is our family max
			Assert.AreEqual(50,coveredAmt);
			//Create a dummy claimproc for the procCodePartCovered procedure we already processed
			ClaimProcHist claimProcHist=new ClaimProcHist();
			claimProcHist.ProcDate=DateTime.Now;
			claimProcHist.Status=ClaimProcStatus.Estimate;
			claimProcHist.PatNum=patient.PatNum;
			claimProcHist.ProcNum=procedurePartCovered.ProcNum;
			claimProcHist.Amount=insEstTotal;
			claimProcHist.InsSubNum=insSub.InsSubNum;
			claimProcHist.PlanNum=insPlan.PlanNum;
			claimProcHist.StrProcCode=procCodePartCovered;
			coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodeNotCovered,ListTools.FromSingle(claimProcHist),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal2,patientAge,insSub.InsSubNum,-1,out limitationTypeMet);
			//$0 will be covered since we have already exceeded our annual max.
			Assert.AreEqual(0,coveredAmt);
		}
		
		/// <summary>Asserts that GetLimitationByCode returns accurate estimates when there is a family annual max and no individual annual max, even when there is an InsEstOverride that is greater than the InsEstTotal.</summary>
		[TestMethod]
		public void Benefits_GetLimitationByCode_FamilyMaxSet_IndividualMaxNotSet_InsEstOverrideGreaterThanInsEstTotal() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string name="Test";
			Patient patient=PatientT.CreatePatient(name);
			Carrier carrier=CarrierT.CreateCarrier(name);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,patient.PriProv,insSub.InsSubNum);
			string procCodePartCovered="D1111";
			string procCodeNotCovered="D2222";
			double insEstTotal=60;
			double insEstTotal2=25;
			double insEstOverride=100;
			double insEstOverride2=50;
			int patientAge=19;
			//Family Annual Max.
			Benefit benefitFamilyMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,50);
			//Individual Annual Max is not set.
			Benefit benefitIndividualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,-1);
			//Age Limitation Fluoride
			Benefit benefitFluoride=BenefitT.CreateAgeLimitation(insPlan.PlanNum,EbenefitCategory.RoutinePreventive,19,BenefitCoverageLevel.Individual);
			List<Benefit> listBenefits=new List<Benefit>{ benefitIndividualMax,benefitFluoride,benefitFamilyMax };
			//Create 1 procedure that will only be partially covered
			Procedure procedurePartCovered=ProcedureT.CreateProcedure(patient,procCodePartCovered,ProcStat.TP,"",100,DateTime.Now);
			//Create 1 procedure that will be over family max
			Procedure procedureNotCovered=ProcedureT.CreateProcedure(patient,procCodeNotCovered,ProcStat.TP,"",50,DateTime.Now);
			string note="";
			LimitationTypeMet limitationTypeMet;
			double coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodePartCovered,new List<ClaimProcHist>(),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal,patientAge,insSub.InsSubNum,insEstOverride,out limitationTypeMet);
			//Only $50 of the $100 procedure will be covered since $50 is our family max
			Assert.AreEqual(50,coveredAmt);
			//Create a dummy claimproc for the procCodePartCovered procedure we already processed
			ClaimProcHist claimProcHist=new ClaimProcHist();
			claimProcHist.ProcDate=DateTime.Now;
			claimProcHist.Status=ClaimProcStatus.Estimate;
			claimProcHist.PatNum=patient.PatNum;
			claimProcHist.ProcNum=procedurePartCovered.ProcNum;
			claimProcHist.Amount=insEstTotal;
			claimProcHist.InsSubNum=insSub.InsSubNum;
			claimProcHist.PlanNum=insPlan.PlanNum;
			claimProcHist.StrProcCode=procCodePartCovered;
			coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodeNotCovered,ListTools.FromSingle(claimProcHist),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal2,patientAge,insSub.InsSubNum,insEstOverride2,out limitationTypeMet);
			//$0 will be covered since we have already exceeded our annual max.
			Assert.AreEqual(0,coveredAmt);
		}
		
		/// <summary>Asserts that GetLimitationByCode returns accurate estimates when there is a family annual max and no individual annual max, even when there is an InsEstOverride that is less than the InsEstTotal.</summary>
		[TestMethod]
		public void Benefits_GetLimitationByCode_FamilyMaxSet_IndividualMaxNotSet_InsEstOverrideLessThanInsEstTotal() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string name="Test";
			Patient patient=PatientT.CreatePatient(name);
			Carrier carrier=CarrierT.CreateCarrier(name);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,patient.PriProv,insSub.InsSubNum);
			string procCodePartCovered="D1111";
			string procCodeNotCovered="D2222";
			double insEstTotal=60;
			double insEstTotal2=25;
			double insEstOverride=30;
			double insEstOverride2=15;
			int patientAge=19;
			//Family Annual Max.
			Benefit benefitFamilyMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,50);
			//Individual Annual Max is not set.
			Benefit benefitIndividualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,-1);
			//Age Limitation Fluoride
			Benefit benefitFluoride=BenefitT.CreateAgeLimitation(insPlan.PlanNum,EbenefitCategory.RoutinePreventive,19,BenefitCoverageLevel.Individual);
			List<Benefit> listBenefits=new List<Benefit>{ benefitIndividualMax,benefitFluoride,benefitFamilyMax };
			//Create 1 procedure that will only be partially covered
			Procedure procedurePartCovered=ProcedureT.CreateProcedure(patient,procCodePartCovered,ProcStat.TP,"",100,DateTime.Now);
			//Create 1 procedure that will be over family max
			Procedure procedureNotCovered=ProcedureT.CreateProcedure(patient,procCodeNotCovered,ProcStat.TP,"",50,DateTime.Now);
			string note="";
			LimitationTypeMet limitationTypeMet;
			double coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodePartCovered,new List<ClaimProcHist>(),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal,patientAge,insSub.InsSubNum,insEstOverride,out limitationTypeMet);
			//Only $50 of the $100 procedure will be covered since $50 is our family max
			Assert.AreEqual(50,coveredAmt);
			//Create a dummy claimproc for the procCodePartCovered procedure we already processed
			ClaimProcHist claimProcHist=new ClaimProcHist();
			claimProcHist.ProcDate=DateTime.Now;
			claimProcHist.Status=ClaimProcStatus.Estimate;
			claimProcHist.PatNum=patient.PatNum;
			claimProcHist.ProcNum=procedurePartCovered.ProcNum;
			claimProcHist.Amount=insEstTotal;
			claimProcHist.InsSubNum=insSub.InsSubNum;
			claimProcHist.PlanNum=insPlan.PlanNum;
			claimProcHist.StrProcCode=procCodePartCovered;
			coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodeNotCovered,ListTools.FromSingle(claimProcHist),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal2,patientAge,insSub.InsSubNum,insEstOverride2,out limitationTypeMet);
			//$0 will be covered since we have already exceeded our annual max.
			Assert.AreEqual(0,coveredAmt);
		}
		
		/// <summary>Asserts that the InsEstTotal is returned by GetLimitationsByCode when there are no benefits present to reduce it by (no Annual Maxes).</summary>
		[TestMethod]
		public void Benefits_GetLimitationByCode_FamilyMaxNotSet_IndividualMaxNotSet_InsEstOverrideNotSet() {
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			string name="Test";
			Patient patient=PatientT.CreatePatient(name);
			Carrier carrier=CarrierT.CreateCarrier(name);
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlan patPlan=PatPlanT.CreatePatPlan(1,patient.PriProv,insSub.InsSubNum);
			string procCodePartCovered="D1111";
			string procCodeNotCovered="D2222";
			double insEstTotal=60;
			double insEstTotal2=25;
			int patientAge=19;
			//Family Annual Max.
			Benefit benefitFamilyMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Family,-1);
			//Individual Annual Max is not set.
			Benefit benefitIndividualMax=BenefitT.CreateAnnualMax(insPlan.PlanNum,BenefitCoverageLevel.Individual,-1);
			//Age Limitation Fluoride
			Benefit benefitFluoride=BenefitT.CreateAgeLimitation(insPlan.PlanNum,EbenefitCategory.RoutinePreventive,19,BenefitCoverageLevel.Individual);
			List<Benefit> listBenefits=new List<Benefit>{ benefitIndividualMax,benefitFluoride,benefitFamilyMax };
			//Create 1 procedure that will only be partially covered
			Procedure procedurePartCovered=ProcedureT.CreateProcedure(patient,procCodePartCovered,ProcStat.TP,"",100,DateTime.Now);
			//Create 1 procedure that will be over family max
			Procedure procedureNotCovered=ProcedureT.CreateProcedure(patient,procCodeNotCovered,ProcStat.TP,"",50,DateTime.Now);
			string note="";
			LimitationTypeMet limitationTypeMet;
			double coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodePartCovered,new List<ClaimProcHist>(),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal,patientAge,insSub.InsSubNum,-1,out limitationTypeMet);
			//Only $60 of the $100 procedure will be covered since $60 is the InsEstTotal and it should not have been reduced at all
			Assert.AreEqual(60,coveredAmt);
			//Create a dummy claimproc for the procCodePartCovered procedure we already processed
			ClaimProcHist claimProcHist=new ClaimProcHist();
			claimProcHist.ProcDate=DateTime.Now;
			claimProcHist.Status=ClaimProcStatus.Estimate;
			claimProcHist.PatNum=patient.PatNum;
			claimProcHist.ProcNum=procedurePartCovered.ProcNum;
			claimProcHist.Amount=insEstTotal;
			claimProcHist.InsSubNum=insSub.InsSubNum;
			claimProcHist.PlanNum=insPlan.PlanNum;
			claimProcHist.StrProcCode=procCodePartCovered;
			coveredAmt=Benefits.GetLimitationByCode(listBenefits,insPlan.PlanNum,patPlan.PatPlanNum,DateTime.Now,procCodeNotCovered,ListTools.FromSingle(claimProcHist),new List<ClaimProcHist>(),insPlan,patient.PatNum,out note,insEstTotal2,patientAge,insSub.InsSubNum,-1,out limitationTypeMet);
			//$25 will be covered since $25 is the InsEstTotal and it should not have been reduced at all.
			Assert.AreEqual(25,coveredAmt);
		}

		///<summary>Asserts that a CoInsurance benefit with a CodeGroupNum covers only procedures under that codegroup.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_CoInsurance() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Codegroup benefit to cover half of procedure fee.
			Benefit benefit=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:50);
			Benefit benefitOld=benefit.Copy();
			benefit.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefit,benefitOld);
			ins.ListBenefits.Add(benefit);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",100);//Covered by benefit
			Procedure procNotCovered=ProcedureT.CreateProcedure(pat,"D1111",ProcStat.C,"11",100);//Not covered by benefit
			List<Procedure> listProcs=new List<Procedure> { proc,procNotCovered };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//proc should have half coverage.
			double paymentExpected=proc.ProcFee/(double)2;
			Assert.AreEqual(paymentExpected,listClaimProcs.Find(x => x.ProcNum==proc.ProcNum).InsPayEst);
			Assert.AreEqual(0,listClaimProcs.Find(x => x.ProcNum==procNotCovered.ProcNum).InsPayEst);
		}

		///<summary>Asserts that a CoInsurance benefit with a CodeGroupNum is overridden by a benefit with a specific code.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_CoInsurance_PriorityVsCodeSpecific() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Codegroup based benefit, half-coverage.
			Benefit benefit=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:50);
			Benefit benefitOld=benefit.Copy();
			benefit.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefit,benefitOld);
			//code-specific benefit, full-coverage.
			long codeNum=ProcedureCodeT.GetCodeNum("D1234");
			Benefit benefitSpecific=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100,codeNum:codeNum);
			ins.ListBenefits.Add(benefit);
			ins.ListBenefits.Add(benefitSpecific);
			Procedure procSpecific=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",100);//Covered by Code-Specific benefit
			Procedure procGeneral=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",100);//Covered by CodeGroup benefit
			Procedure procNotCovered=ProcedureT.CreateProcedure(pat,"D1111",ProcStat.C,"11",100);//Not covered by benefits
			List<Procedure> listProcs=new List<Procedure> { procSpecific,procGeneral,procNotCovered };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			double paymentExpected=procGeneral.ProcFee/(double)2;
			Assert.AreEqual(procSpecific.ProcFee,listClaimProcs.Find(x => x.ProcNum==procSpecific.ProcNum).InsPayEst);
			Assert.AreEqual(paymentExpected,listClaimProcs.Find(x => x.ProcNum==procGeneral.ProcNum).InsPayEst);
			Assert.AreEqual(0,listClaimProcs.Find(x => x.ProcNum==procNotCovered.ProcNum).InsPayEst);
		}

		///<summary>Asserts that a CoInsurance benefit with a CodeGroupNum overrides a benefit with a coverage category.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_CoInsurance_PriorityVsCovCat() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Codegroup-based benefit with half coverage.
			Benefit benefitCodeGroup=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:50);
			Benefit benefitOld=benefitCodeGroup.Copy();
			benefitCodeGroup.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitCodeGroup,benefitOld);
			//Give endo every code just for the sake of the test. Should have full coverage.
			Benefit benefitCategory=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Endodontics,100);
			long covCatNum=CovCats.GetDeepCopy().Find(x=>x.EbenefitCat==EbenefitCategory.Endodontics).CovCatNum;
			CovSpanT.CreateCovSpan(covCatNum,fromCode:"D0000",toCode:"D9999");
			ins.ListBenefits.Add(benefitCodeGroup);
			ins.ListBenefits.Add(benefitCategory);
			Procedure procCategory=ProcedureT.CreateProcedure(pat,"D1111",ProcStat.C,"11",100);//Covered by Category-Specific benefit
			Procedure procCodeGroup=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",100);//Covered by CodeGroup benefit
			List<Procedure> listProcs=new List<Procedure> { procCategory,procCodeGroup };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//procCategory should have full coverage, procCodeGroup should have half coverage.
			double paymentExpected=procCodeGroup.ProcFee/(double)2;
			Assert.AreEqual(procCategory.ProcFee,listClaimProcs.Find(x => x.ProcNum==procCategory.ProcNum).InsPayEst);
			Assert.AreEqual(paymentExpected,listClaimProcs.Find(x => x.ProcNum==procCodeGroup.ProcNum).InsPayEst);
		}

		///<summary>Asserts that a CoInsurance benefit with a CodeGroupNum overrides a benefit with a no category, codegroup, or procnum.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_CoInsurance_PriorityVsGeneral() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup2","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Codegroup-based benefit, 50% coverage.
			Benefit benefitCodeGroup=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:50);
			Benefit benefitOld=benefitCodeGroup.Copy();
			benefitCodeGroup.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitCodeGroup,benefitOld);
			ins.ListBenefits.Add(benefitCodeGroup);
			//General coverage benefit.
			Benefit benefitCategory=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitCategory);
			Procedure procCategory=ProcedureT.CreateProcedure(pat,"D1111",ProcStat.C,"11",100);//Covered by Category-Specific benefit
			Procedure procCodeGroup=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",100);//Covered by CodeGroup benefit
			List<Procedure> listProcs=new List<Procedure> { procCategory,procCodeGroup };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Procedure outside the codegroup should have full coverage, procedure within the codegroup should have half.
			double paymentExpected=procCodeGroup.ProcFee/(double)2;
			Assert.AreEqual(procCategory.ProcFee,listClaimProcs.Find(x => x.ProcNum==procCategory.ProcNum).InsPayEst);
			Assert.AreEqual(paymentExpected,listClaimProcs.Find(x => x.ProcNum==procCodeGroup.ProcNum).InsPayEst);
		}

		///<summary>CoInsurance benefits A and B are attached to codegroups with shared procedure codes. Proves that A overrides B if A's codegroup has a higher itemorder than B's codegroup.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_CoInsurance_PriorityVsOtherCodeGroup() {
			ProcedureCodeT.AddIfNotPresent("D5678");
			ProcedureCodeT.AddIfNotPresent("D9021");
			//First codegroup, covers just one procedure. Higher item order.
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D5678");
			codeGroup.ItemOrder=2;
			CodeGroups.Update(codeGroup);
			//Second codegroup, covers both procedures. Lower item order.
			CodeGroup codeGroupBroad=CodeGroupT.Upsert("TestCodeGroupBroad","D5678,D9021");
			codeGroupBroad.ItemOrder=1;
			CodeGroups.Update(codeGroupBroad);
			CodeGroups.RefreshCache();
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Benefit has higher item order, lower coverage.
			Benefit benefit=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:50);
			Benefit benefitOld=benefit.Copy();
			benefit.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefit,benefitOld);
			//Benefit has lower item order, higher coverage.
			Benefit benefitBroad=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100);
			Benefit benefitBroadOld=benefit.Copy();
			benefitBroad.CodeGroupNum=codeGroupBroad.CodeGroupNum;
			Benefits.Update(benefitBroad,benefitBroadOld);
			ins.ListBenefits.Add(benefitBroad);
			ins.ListBenefits.Add(benefit);
			Procedure procBroad=ProcedureT.CreateProcedure(pat,"D9021",ProcStat.C,"11",100);//Covered by low-order codegroup benefit
			Procedure proc=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",100);//Covered by high-order codegroup benefit
			List<Procedure> listProcs=new List<Procedure> { procBroad,proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//procBroad should have full coverage, while proc should have half coverage.
			double paymentExpected=proc.ProcFee/(double)2;
			Assert.AreEqual(procBroad.ProcFee,listClaimProcs.Find(x => x.ProcNum==procBroad.ProcNum).InsPayEst);
			Assert.AreEqual(paymentExpected,listClaimProcs.Find(x => x.ProcNum==proc.ProcNum).InsPayEst);
		}

		///<summary>Asserts that a Deductible benefit with a CodeGroupNum covers only procedures under that codegroup.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Deductible() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroupDeduct=CodeGroupT.Upsert("TestCodeGroup","D1234");
			//Procedure must be covered by percentage if deductible is going to apply.
			CodeGroup codeGroupPercent=CodeGroupT.Upsert("TestCodeGroupPercent","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Create the Percent benefit, assign the broad codegroup.
			Benefit benefitPercent=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100);
			Benefit benefitOld=benefitPercent.Copy();
			benefitPercent.CodeGroupNum=codeGroupPercent.CodeGroupNum;
			Benefits.Update(benefitPercent,benefitOld);
			ins.ListBenefits.Add(benefitPercent);
			//Create the Deductible benefit, assign the specific codegroup.
			Benefit benefitDeductible=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:100,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual);
			benefitOld=benefitDeductible.Copy();
			benefitDeductible.CodeGroupNum=codeGroupDeduct.CodeGroupNum;
			Benefits.Update(benefitDeductible,benefitOld);
			ins.ListBenefits.Add(benefitDeductible);
			Procedure procNotDeducted=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",50);//Not covered by Deductible
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",50);//Covered by Deductible
			List<Procedure> listProcs=new List<Procedure> { procNotDeducted,proc };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Deductible should count for first procedure, but not second.
			Assert.AreEqual(proc.ProcFee,listClaimProcs.Find(x => x.ProcNum==proc.ProcNum).DedEst);
			Assert.AreEqual(0,listClaimProcs.Find(x => x.ProcNum==procNotDeducted.ProcNum).DedEst);
			
		}

		///<summary>A codegroup deductible and a code-specific deductible exist. The code for the code-specific deductible is contained within the codegroup deductible's codegroup. 
		///Asserts that the codegroup deductible is counted towards when the code-specific deductible is used.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Deductible_PriorityVsCodeSpecific() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroupDeduct=CodeGroupT.Upsert("TestCodeGroup","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			Benefit benefitPercent=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100);
			Benefit benefitOld=benefitPercent.Copy();
			benefitPercent.CodeGroupNum=codeGroupDeduct.CodeGroupNum;
			Benefits.Update(benefitPercent,benefitOld);
			ins.ListBenefits.Add(benefitPercent);
			//Code-Specific Deductible
			ins.ListBenefits.Add(BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:100,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual,codeNum:ProcedureCodes.GetCodeNum("D5678")));
			//CodeGroup-Wide Deductible
			Benefit benefitDeductible=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:100,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual);
			benefitOld=benefitDeductible.Copy();
			benefitDeductible.CodeGroupNum=codeGroupDeduct.CodeGroupNum;
			Benefits.Update(benefitDeductible,benefitOld);
			ins.ListBenefits.Add(benefitDeductible);
			Procedure procBroadDeductible=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",50);//Covered by Broad deductible
			Procedure procSpecificDeductible=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",70);//Covered by specific deductible.
			List<Procedure> listProcs=new List<Procedure> { procSpecificDeductible,procBroadDeductible };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//procSpecificDeductible should be completely deductible, and both deductibles should be reduced by its procfee.
			double deductibleRemaining=benefitDeductible.MonetaryAmt-procSpecificDeductible.ProcFee;
			Assert.AreEqual(procSpecificDeductible.ProcFee,listClaimProcs.Find(x => x.ProcNum==procSpecificDeductible.ProcNum).DedEst);
			Assert.AreEqual(deductibleRemaining,listClaimProcs.Find(x => x.ProcNum==procBroadDeductible.ProcNum).DedEst);
		}

		///<summary>Asserts that a Deductible benefit with a CodeGroupNum counts towards a deductible with a matching CovCat.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Deductible_PriorityVsCovCat() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroupDeduct=CodeGroupT.Upsert("TestCodeGroup","D1234");
			CodeGroup codeGroupPercent=CodeGroupT.Upsert("TestCodeGroup","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			Benefit benefitPercent=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100);
			Benefit benefitOld=benefitPercent.Copy();
			benefitPercent.CodeGroupNum=codeGroupPercent.CodeGroupNum;
			Benefits.Update(benefitPercent,benefitOld);
			ins.ListBenefits.Add(benefitPercent);
			//Give endodontics a span of everything just for demonstration purposes.
			long covCatNum=CovCats.GetDeepCopy().Find(x=>x.EbenefitCat==EbenefitCategory.Endodontics).CovCatNum;
			CovSpanT.CreateCovSpan(covCatNum,fromCode:"D0000",toCode:"D9999");
			//CovCat-Wide Deductible
			Benefit benefitDeductibleBroad=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:100,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual,covCatNum:covCatNum);
			ins.ListBenefits.Add(benefitDeductibleBroad);
			//CodeGroup-Specific Deductible
			Benefit benefitDeductible=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:100,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual);
			benefitOld=benefitDeductible.Copy();
			benefitDeductible.CodeGroupNum=codeGroupDeduct.CodeGroupNum;
			Benefits.Update(benefitDeductible,benefitOld);
			ins.ListBenefits.Add(benefitDeductible);
			Procedure procBroadDeductible=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",50);//Covered by Broad deductible
			Procedure procSpecificDeductible=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",70);//Covered by specific deductible.
			List<Procedure> listProcs=new List<Procedure> { procSpecificDeductible,procBroadDeductible };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//procSpecific should have its full price as a deductible, but this should reduce the max for the broad deductible as well.
			double deductibleExpected=benefitDeductibleBroad.MonetaryAmt-procSpecificDeductible.ProcFee;
			Assert.AreEqual(procSpecificDeductible.ProcFee,listClaimProcs.Find(x => x.ProcNum==procSpecificDeductible.ProcNum).DedEst);
			Assert.AreEqual(deductibleExpected,listClaimProcs.Find(x => x.ProcNum==procBroadDeductible.ProcNum).DedEst);
		}

		///<summary>Asserts that a Deductible benefit with a CodeGroupNum still contributes to the general deductible.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Deductible_PriorityVsGeneral() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroupDeduct=CodeGroupT.Upsert("TestCodeGroup","D5678");
			CodeGroup codeGroupPercent=CodeGroupT.Upsert("TestCodeGroup2","D1234,D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			Benefit benefitPercent=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100);
			Benefit benefitOld=benefitPercent.Copy();
			benefitPercent.CodeGroupNum=codeGroupPercent.CodeGroupNum;
			Benefits.Update(benefitPercent,benefitOld);
			ins.ListBenefits.Add(benefitPercent);
			//General Deductible
			ins.ListBenefits.Add(BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:50,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual,covCatNum:0));
			//CodeGroup-Specific Deductible
			Benefit benefitDeductible=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:10,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual);
			benefitOld=benefitDeductible.Copy();
			benefitDeductible.CodeGroupNum=codeGroupDeduct.CodeGroupNum;
			Benefits.Update(benefitDeductible,benefitOld);
			ins.ListBenefits.Add(benefitDeductible);
			Procedure procBroadDeductible=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",50);//Covered by General deductible
			Procedure procSpecificDeductible=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",70);//Covered by specific deductible.
			List<Procedure> listProcs=new List<Procedure> { procSpecificDeductible,procBroadDeductible };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//The procedure covered by the codegroup deductible should use up all of that deductible. There should still be some general deductible left over.
			double generalDeductRemain=procBroadDeductible.ProcFee-benefitDeductible.MonetaryAmt;
			Assert.AreEqual(benefitDeductible.MonetaryAmt,listClaimProcs.Find(x => x.ProcNum==procSpecificDeductible.ProcNum).DedEst);
			Assert.AreEqual(generalDeductRemain,listClaimProcs.Find(x => x.ProcNum==procBroadDeductible.ProcNum).DedEst);
		}

		///<summary>Asserts that a procedure covered by two deductibles uses the highest deductible.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Deductible_PriorityVsOtherCodeGroup() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroupDeduct=CodeGroupT.Upsert("TestCodeGroup","D5678");
			codeGroupDeduct.ItemOrder=2;
			CodeGroup codeGroupPercent=CodeGroupT.Upsert("TestCodeGroup2","D1234,D5678");
			codeGroupPercent.ItemOrder=1;
			CodeGroups.Update(codeGroupDeduct);
			CodeGroups.Update(codeGroupPercent);
			CodeGroups.RefreshCache();
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			Benefit benefitPercent=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.CoInsurance,percent:100);
			Benefit benefitOld=benefitPercent.Copy();
			benefitPercent.CodeGroupNum=codeGroupPercent.CodeGroupNum;
			Benefits.Update(benefitPercent,benefitOld);
			ins.ListBenefits.Add(benefitPercent);
			//Less specific benefit, higher deductible
			Benefit benefitDeductibleBroad=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:50,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual);
			benefitOld=benefitPercent.Copy();
			benefitDeductibleBroad.CodeGroupNum=codeGroupPercent.CodeGroupNum;
			ins.ListBenefits.Add(benefitDeductibleBroad);
			//More specific benefit, lower deductible
			Benefit benefitDeductible=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.Deductible,monetaryAmt:20,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitCoverageLevel:BenefitCoverageLevel.Individual);
			benefitOld=benefitDeductible.Copy();
			benefitDeductible.CodeGroupNum=codeGroupDeduct.CodeGroupNum;
			Benefits.Update(benefitDeductible,benefitOld);
			ins.ListBenefits.Add(benefitDeductible);
			Procedure procBroadDeductible=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",100);//Covered by broad deductible
			Procedure procSpecificDeductible=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",50);//Covered by specific deductible.
			List<Procedure> listProcs=new List<Procedure> { procSpecificDeductible,procBroadDeductible };
			List<ClaimProc> listClaimProcs=new List<ClaimProc> { };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,listClaimProcs,listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//ProcSpecific should use up all of the highest deductible.
			Assert.AreEqual(benefitDeductibleBroad.MonetaryAmt,listClaimProcs.Find(x => x.ProcNum==procSpecificDeductible.ProcNum).DedEst);
			Assert.AreEqual(0,listClaimProcs.Find(x => x.ProcNum==procBroadDeductible.ProcNum).DedEst);
		}

		///<summary>Asserts that a codegroup max benefit will only affect the procedures in the codegroup.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Limitation() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			Benefit benefitPercent=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitPercent);
			Benefit benefitLimit=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.General,50);
			Benefit benefitOld=benefitLimit.Copy();
			benefitLimit.CovCatNum=0;
			benefitLimit.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitLimit,benefitOld);
			ins.ListBenefits.Add(benefitLimit);
			//Use up the limited max.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",75); //Limited by benefit
			Procedure procUnlimited=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"12",100); //Not limited by benefit
			List<Procedure> listProcs=new List<Procedure>{ proc,procUnlimited };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Limited proc should reach max, unlimited proc should have full coverage.
			Assert.AreEqual(benefitLimit.MonetaryAmt,listClaimProcs.Find(x=>x.ProcNum==proc.ProcNum).InsPayEst);
			Assert.AreEqual(procUnlimited.ProcFee,listClaimProcs.Find(x=>x.ProcNum==procUnlimited.ProcNum).InsPayEst);
		}

		/// <summary>Asserts that a CodeGroup max overrides a CovCat max.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Limitation_PriorityVsCovCat() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Full price coverage.
			Benefit benefitPercent=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitPercent);
			//CovCat-Based Limit
			Benefit benefitLimitGeneral=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitLimitGeneral);
			//CodeGroup-based benefit
			Benefit benefitLimitSpecific=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.General,50);
			Benefit benefitOld=benefitLimitSpecific.Copy();
			benefitLimitSpecific.CovCatNum=0;
			benefitLimitSpecific.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitLimitSpecific,benefitOld);
			ins.ListBenefits.Add(benefitLimitSpecific);
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",75); //Limited by CodeGroup benefit
			Procedure procUnlimited=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"12",150); //Limited by CovCat benefit
			List<Procedure> listProcs=new List<Procedure>{ procUnlimited,proc };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//The CodeGroup max should have been reached without touching the covcat max.
			Assert.AreEqual(benefitLimitSpecific.MonetaryAmt,listClaimProcs.Find(x=>x.ProcNum==proc.ProcNum).InsPayEst);
			Assert.AreEqual(benefitLimitGeneral.MonetaryAmt,listClaimProcs.Find(x=>x.ProcNum==procUnlimited.ProcNum).InsPayEst);
		}

		/// <summary>Asserts that a CodeGroup max still counts against total max.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Limitation_PriorityVsGeneral() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Full price coverage
			Benefit benefitPercent=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitPercent);
			//General annual max.
			Benefit benefitLimitGeneral=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.None,100);
			Benefit benefitOld=benefitLimitGeneral.Copy();
			benefitLimitGeneral.CovCatNum=0;
			Benefits.Update(benefitLimitGeneral,benefitOld);
			ins.ListBenefits.Add(benefitLimitGeneral);
			//Codegroup-based limit
			Benefit benefitLimitSpecific=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.General,50);
			benefitOld=benefitLimitSpecific.Copy();
			benefitLimitSpecific.CovCatNum=0;
			benefitLimitSpecific.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitLimitSpecific,benefitOld);
			ins.ListBenefits.Add(benefitLimitSpecific);
			Procedure procSpecific=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",75); //Limited by CodeGroup benefit
			Procedure procGeneral=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"12",150); //Limited by General benefit
			List<Procedure> listProcs=new List<Procedure>{ procSpecific,procGeneral };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Assert that procedure hit local max.
			Assert.AreEqual(benefitLimitSpecific.MonetaryAmt,listClaimProcs.Find(x=>x.ProcNum==procSpecific.ProcNum).InsPayEst);
			//Assert that general limit had a chunk taken out of it
			double insAlreadyUsed=listClaimProcs.Find(x=>x.ProcNum==procSpecific.ProcNum).InsPayEst;
			Assert.AreEqual(benefitLimitGeneral.MonetaryAmt-insAlreadyUsed,listClaimProcs.Find(x=>x.ProcNum==procGeneral.ProcNum).InsPayEst);
		}

		/// <summary>Asserts that a CodeNum max overrides a CodeGroup max.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Limitation_PriorityVsCodeSpecific() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234,D5678");
			long codeNum=ProcedureCodes.GetCodeNum("D5678");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Full price coverage
			Benefit benefitPercent=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitPercent);
			//Codegroup limitation
			Benefit benefitLimitGeneral=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.General,50);
			Benefit benefitOld=benefitLimitGeneral.Copy();
			benefitLimitGeneral.CovCatNum=0;
			benefitLimitGeneral.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitLimitGeneral,benefitOld);
			ins.ListBenefits.Add(benefitLimitGeneral);
			//ProcNum limitation
			Benefit benefitLimitSpecific=BenefitT.CreateLimitationProc(ins.PriInsPlan.PlanNum,"D5678",100);
			ins.ListBenefits.Add(benefitLimitSpecific);
			Procedure procCodeGroup=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",75); //Limited by CodeGroup benefit
			Procedure procCodeNum=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"12",150); //Limited by Code Specific benefit
			List<Procedure> listProcs=new List<Procedure>{ procCodeNum,procCodeGroup };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			//Codegroup associated procedure should meet its max.
			Assert.AreEqual(benefitLimitGeneral.MonetaryAmt,listClaimProcs.Find(x=>x.ProcNum==procCodeGroup.ProcNum).InsPayEst);
			//The procedure covered by the specific benefit should not have its max affected. It should meet the full max.
			Assert.AreEqual(benefitLimitSpecific.MonetaryAmt,listClaimProcs.Find(x=>x.ProcNum==procCodeNum.ProcNum).InsPayEst);
		}

		///<summary>Two codegroups exist with overlapping ProcCodes. Different monetary limitation benefits exist for each codegroup.
		///When a procedure with a shared proccode is completed, both the benefit with the lowest max is used. The other is not counted towards.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Limitation_PriorityVsOtherCodeGroup() {
			//Setup codegroups
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678"); //This code will be in both codegroups
			ProcedureCodeT.AddIfNotPresent("D0910");
			CodeGroup codeGroupHigh=CodeGroupT.Upsert("HighCodeGroup","D1234,D5678");
			CodeGroups.Update(codeGroupHigh);
			CodeGroup codeGroupLow=CodeGroupT.Upsert("LowCodeGroup","D5678,D0910");
			CodeGroups.Update(codeGroupLow);
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			ins.AddBenefit(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100));
			//A benefit with a higher max limitation.
			Benefit benefitLimitHighMax=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.None,75);
			Benefit benefitOld=benefitLimitHighMax.Copy();
			benefitLimitHighMax.CovCatNum=0;
			benefitLimitHighMax.CodeGroupNum=codeGroupHigh.CodeGroupNum;
			Benefits.Update(benefitLimitHighMax,benefitOld);
			ins.ListBenefits.Add(benefitLimitHighMax);
			//Benefit with a lower max limitation.
			Benefit benefitLimitLowMax=BenefitT.CreateLimitation(ins.PriInsPlan.PlanNum,EbenefitCategory.None,50);
			benefitOld=benefitLimitLowMax.Copy();
			benefitLimitLowMax.CovCatNum=0;
			benefitLimitLowMax.CodeGroupNum=codeGroupLow.CodeGroupNum;
			Benefits.Update(benefitLimitLowMax,benefitOld);
			ins.ListBenefits.Add(benefitLimitLowMax);
			//This proc is covered by both benefits. However, it will default to using the lower max.
			Procedure procUsesLimit=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"11",100);
			//This proc is covered by the higher max. The previous proc will not count towards its max.
			Procedure procSomeMaxLeft=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"11",50);
			//This procedure is covered by the lower maximum benefit. The max will have been reached for this procedure.
			Procedure procNoMaxLeft=ProcedureT.CreateProcedure(pat,"D0910",ProcStat.C,"11",50);
			//The order of the procedures in this list is important.
			List<Procedure> listProcs=new List<Procedure>{ procUsesLimit,procSomeMaxLeft,procNoMaxLeft };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProcUsedLimit=listClaimProcs.Find(x=>x.ProcNum==procUsesLimit.ProcNum);
			ClaimProc claimProcSomeMaxLeft=listClaimProcs.Find(x=>x.ProcNum==procSomeMaxLeft.ProcNum);
			ClaimProc claimProcNoMaxLeft=listClaimProcs.Find(x=>x.ProcNum==procNoMaxLeft.ProcNum);
			//The first procedure should have used up the insurance maximum.
			Assert.AreEqual(benefitLimitLowMax.MonetaryAmt,claimProcUsedLimit.InsPayEst);
			//The second procedure should not have been touched by the first procedure.
			Assert.AreEqual(procSomeMaxLeft.ProcFee,claimProcSomeMaxLeft.InsPayEst);
			//The third procedure should have had its maximum used up by the first.
			Assert.AreEqual(0,claimProcNoMaxLeft.InsPayEst);
		}

		///<summary>Asserts that a waiting period benefit will function properly when assigned a codegroupnum.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_WaitingPeriod() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			ProcedureCodeT.AddIfNotPresent("D5678");
			//Leave out the other code so we can prove it's not covered by this benefit.
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//DateEffective must be set to process properly.
			ins.PriInsSub.DateEffective=DateTime.Today.AddMonths(-3);
			InsSubs.Update(ins.PriInsSub);
			//Set up full price coverage
			Benefit benefitPercent=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitPercent);
			//Waiting period benefit
			Benefit benefitWait=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatPlanNum,insBenefitType:InsBenefitType.WaitingPeriod,benefitTimePeriod:BenefitTimePeriod.CalendarYear,benefitQuantityQualifier:BenefitQuantity.Months,quantity:6);
			Benefit benefitOld=benefitWait.Copy();
			benefitWait.CovCatNum=0;
			benefitWait.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitWait,benefitOld);
			ins.ListBenefits.Add(benefitWait);
			//Create a procedure that will be covered by the wait limit benefit, and one that will not.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"1",100);
			Procedure procUnlimited=ProcedureT.CreateProcedure(pat,"D5678",ProcStat.C,"3",100);
			List<Procedure> listProcs=new List<Procedure>{ proc,procUnlimited };
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=listClaimProcs.Find(x=>x.ProcNum==proc.ProcNum);
			ClaimProc claimProcUnlimited=listClaimProcs.Find(x=>x.ProcNum==procUnlimited.ProcNum);
			//Assert that the covered procedure is not paid for, and that the uncovered procedure IS paid for.
			Assert.AreEqual(0,claimProc.InsPayEst);
			Assert.AreEqual(procUnlimited.ProcFee,claimProcUnlimited.InsPayEst);
		}

		/// <summary>Asserts that an exclusion benefit with a codegroup applies an exclusion to procedures in that codegroup.</summary>
		[TestMethod]
		public void BenefitsTest_CodeGroupBenefits_Exclusion() {
			ProcedureCodeT.AddIfNotPresent("D1234");
			CodeGroup codeGroup=CodeGroupT.Upsert("TestCodeGroup","D1234");
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix,birthDate:DateTime.Today.AddYears(-13));
			InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
			//Percent coverage benefit
			Benefit benefitPercent=BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.General,100);
			ins.ListBenefits.Add(benefitPercent);
			//Exclusion benefit
			Benefit benefitExclude=BenefitT.CreateBenefit(ins.PriInsPlan.PlanNum,ins.PriPatPlan.PatNum,0,InsBenefitType.Exclusions,0,0,BenefitTimePeriod.None,BenefitQuantity.None,0,0,BenefitCoverageLevel.None);
			Benefit benefitOld=benefitExclude.Copy();
			benefitExclude.CodeGroupNum=codeGroup.CodeGroupNum;
			Benefits.Update(benefitExclude,benefitOld);
			ins.ListBenefits.Add(benefitExclude);
			//Procedure that should be under the exclusion.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1234",ProcStat.C,"1",100);
			List<Procedure> listProcs=ListTools.FromSingle(proc);
			Claim claim=ClaimT.CreateClaim("P",ins.ListPatPlans,ins.ListInsPlans,new List<ClaimProc>(),listProcs,pat,listProcs,ins.ListBenefits,ins.ListInsSubs);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(pat.PatNum);
			ClaimProc claimProc=listClaimProcs.Find(x=>x.ProcNum==proc.ProcNum);
			//Assert that we block this exclusion.
			Assert.AreEqual(0,claimProc.InsPayEst);
		}
	}
}
