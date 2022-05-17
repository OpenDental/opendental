using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;

namespace UnitTests.DiscountPlanTests {
	[TestClass]
	public class DiscountPlanTests:TestBase {

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_OneYearSegment_DoesntExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			//Create 4 exam procs, and discount plan with examFreqLimit = 4
			//set all 4 exams to complete.
			//Test if the 5th proc, 2nd prophy will exceed frequency limitations
			/*****************************************************
				1. Create a pat, discount plan, and discount plan sub linking the pat to the DP.
				2. Create ProcCode, Fee, and ProcedureLog for proc 1-4
				3. Update global DiscountPlan Exam codes to include procCode 1-4
				4. Set procs 1-3 to complete
				5. Check if proc 4 will exceed limitations (it shouldn't)
			******************************************************/
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			List<Procedure> procHistExceedsLimit=new List<Procedure> { };
			for(int i = 0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			procHist.Add(proc4);
			Assert.AreEqual($"",DiscountPlans.CheckDiscountFrequency(new List<Procedure>{ proc4 },pat.PatNum,DateTime.Now,discountPlanSub));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_OneYearSegment_DoesExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			//Create 5 procs, two will be categorized as prophy, 3 as exams.
			//set all 3 exams, and 1 prophy to complete.
			//Test if the 5th proc, 2nd prophy will exceed frequency limitations
			/*****************************************************
				1. Create a pat, discount plan, and discount plan sub linking the pat to the DP.
				2. the DP will have: examFreqLimit=4
				3. Create ProcCode, Fee, and ProcedureLog for proc 1-4
				4. Update global DiscountPlan Exam codes to include procCode 1-4
				5. Set procs 1-4 to complete
				6. Check if proc 5 will exceed limitations (it should)
			******************************************************/
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			Assert.AreEqual($"{procCode5.ProcCode}\r\n",DiscountPlans.CheckDiscountFrequency(new List<Procedure>{ proc5 },pat.PatNum,DateTime.Now,discountPlanSub));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Where(x=>x.DiscountPlanAmt<5).ToList().Count);
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_OneYearSegment_DoesExceed2() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			//Create 5 procs, two will be categorized as prophy, 3 as exams.
			//set all 3 exams, and 1 prophy to complete.
			//Test if the 5th proc, 2nd prophy will exceed frequency limitations
			/*****************************************************
				1. Create a pat, discount plan, and discount plan sub linking the pat to the DP.
				2. the DP will have: 
					 - examFreqLimit=		3
					 - prophyFreqLimit=	1
				3. Create ProcCode, Fee, and ProcedureLog for proc 1-5
				4. Update global DiscountPlan ProcCodes:
					 - Exam: Procs	 1-4
					 - Prophy: Procs 4-5
				5. Set procs 1-4 to complete
				6. Check if proc 5 will exceed limitations (it should)
			******************************************************/
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:3,prophyFreqLimit:1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Assert.AreEqual($"{procCode5.ProcCode}\r\n",DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub));
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,procHistExceedsLimit.All(x=>discountfreqMessage.Contains(ProcedureCodes.GetStringProcCode(x.CodeNum))));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Where(x=>x.DiscountPlanAmt==0).ToList().Count);
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_MultipleSegments_AllDontExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:-1,xrayFreqLimit:1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(5));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(2));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,procHistExceedsLimit.All(x=>discountfreqMessage.Contains(ProcedureCodes.GetStringProcCode(x.CodeNum))));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_MultiplsSegment_SomeExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:-1,xrayFreqLimit:1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6).AddMonths(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3).AddMonths(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc2,proc4 };
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,procHistExceedsLimit.All(x=>discountfreqMessage.Contains(ProcedureCodes.GetStringProcCode(x.CodeNum))));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckFrequencyLimitations_MultiplsSegment_AllExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();

			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_OneYearSegment_DoesntExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:25);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist.Max(x=>x.ProcDate));
			Assert.AreEqual(discountPlan.AnnualMax,listAnnualMaxTots[index]);
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_OneYearSegment_DoesExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:20);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist.Max(x=>x.ProcDate));
			Assert.AreEqual(discountPlan.AnnualMax,listAnnualMaxTots[index]);
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_OneYearSegment_DoesExceed2() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:23);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { proc5 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist.Max(x=>x.ProcDate));
			Assert.AreEqual(discountPlan.AnnualMax,listAnnualMaxTots[index]);
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultipleSegments_AllDontExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:5);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(5));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(4));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(2));
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			for(int i = 0;i<procHist.Count;i++) {
				int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist[i].ProcDate);
				Assert.AreEqual(true,discountPlan.AnnualMax>=listAnnualMaxTots[index]);
			}
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultipleSegment_SomeExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:5);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(2));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc2 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			for(int i = 0;i<procHist.Count;i++) {
				int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist[i].ProcDate);
				Assert.AreEqual(true,discountPlan.AnnualMax>=listAnnualMaxTots[index]);
			}
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultipleSegment_AllExceed() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:5);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc2,proc4,proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			for(int i = 0;i<procHist.Count;i++) {
				int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist[i].ProcDate);
				Assert.AreEqual(true,discountPlan.AnnualMax>=listAnnualMaxTots[index]);
			}
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_CheckAnnualMax_MultipleSegment_AllExceed2() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:6);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(6));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { proc2,proc4 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			for(int i=0;i<procHist.Count;i++) {
				Procedure procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			List<double> listAnnualMaxTots=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,procHist.Max(x=>x.ProcDate));
			for(int i = 0;i<procHist.Count;i++) {
				int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,procHist[i].ProcDate);
				Assert.AreEqual(true,discountPlan.AnnualMax>=listAnnualMaxTots[index]);
			}
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
			
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_AllProcsAreValid() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:-1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			procHist.Add(proc5);
			procOld=proc5.Copy();
			proc5.ProcStatus=ProcStat.C;
			Procedures.Update(proc5,procOld);
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,discountfreqMessage=="");
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_SomeProcsAreValid() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:5,annualMax:-1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(-1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(-1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc1,proc2 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			procHist.Add(proc5);
			procOld=proc5.Copy();
			proc5.ProcStatus=ProcStat.C;
			Procedures.Update(proc5,procOld);
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,discountfreqMessage=="");
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_SomeProcsAreValid2() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4,annualMax:-1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(-1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(-1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc1,proc2 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,discountfreqMessage=="");
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_SomeProcsAreValid3() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4,annualMax:-1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			procHist.Add(proc5);
			procOld=proc5.Copy();
			proc5.ProcStatus=ProcStat.C;
			Procedures.Update(proc5,procOld);
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,procHistExceedsLimit.All(x=>discountfreqMessage.Contains(ProcedureCodes.GetStringProcCode(x.CodeNum))));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_SomeProcsAreValid4() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4,annualMax:14);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { proc3 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc4,proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			int annualIndex=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,proc1.ProcDate);
			List<double> listAdjustmentTotals=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,proc5.ProcDate.AddDays(1));
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,discountfreqMessage=="");
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(discountPlan.AnnualMax,listAdjustmentTotals[annualIndex]);
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_SomeProcsAreValid5() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,limitedFreqLimit:2,annualMax:15);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(3));
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc3,proc4,proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			Assert.AreEqual($"{ProcedureCodes.GetStringProcCode(proc3.CodeNum)}\r\n",DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,procHist[0].ProcDate,discountPlanSub));
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(true,discountfreqMessage.Contains(ProcedureCodes.GetStringProcCode(proc3.CodeNum)));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_SomeProcsAreValid6() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,limitedFreqLimit:2,prophyFreqLimit:2,paFreqLimit:2,annualMax:16);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,procCode1.ProcCode+","+procCode2.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,procCode3.ProcCode+","+procCode4.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { proc4 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,
				procHist.Where(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistPartiallyExceedsLimits.Select(y=>y.ProcNum).ToList().Contains(x)));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistExceedsLimit.Count,
				procHist.Where(x=>x.DiscountPlanAmt==0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistExceedsLimit.Select(y=>y.ProcNum).ToList().Contains(x)));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_UnlimitedFreqLimitation1() {
			/// *******************************************************
			/// Frequency Limitation is -1 (unlimited) for all categories, annual max is -1 (unlimited)
			/// Discount Plan sub date range is the start of this calander year (minval), to Now + 1 year and 1 day
			/// Create 25 procs, all for dateTime now, and attempt to give them all a 5 dollar discount
			/// All should be discounted
			/// *******************************************************
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			PrefT.UpdateString(PrefName.DiscountPlanFluorideCodes,procCode1.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,procCode2.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPerioCodes,procCode4.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			for(int i = 0;i<5;i++) {
				Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc1);
				Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc2);
				Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc3);
				Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc4);
				Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc5);
			}
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(discountfreqMessage,"");
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,
				procHist.Where(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistPartiallyExceedsLimits.Select(y=>y.ProcNum).ToList().Contains(x)));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistExceedsLimit.Count,
				procHist.Where(x=>x.DiscountPlanAmt==0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistExceedsLimit.Select(y=>y.ProcNum).ToList().Contains(x)));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_UnlimitedFreqLimitation2() {
			/// *******************************************************
			/// Frequency Limitation is -1 (unlimited) for all categories, annual max is 100
			/// Discount Plan sub date range is the start of this calander year (minval), to Now + 1 year and 1 day
			/// Create 25 procs, 20 for dateTime now,and 5 for dateTime now + 1 year, and attempt to give them all a 5 dollar discount
			/// All should be discounted
			/// *******************************************************
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:100);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1).AddDays(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			PrefT.UpdateString(PrefName.DiscountPlanFluorideCodes,procCode1.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,procCode2.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode4.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			for(int i = 0;i<5;i++) {
				Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc1);
				Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc2);
				Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc3);
				Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
				procHist.Add(proc4);
				Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
				procHist.Add(proc5);
			}
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(discountfreqMessage,"");
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,
				procHist.Where(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistPartiallyExceedsLimits.Select(y=>y.ProcNum).ToList().Contains(x)));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistExceedsLimit.Count,
				procHist.Where(x=>x.DiscountPlanAmt==0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistExceedsLimit.Select(y=>y.ProcNum).ToList().Contains(x)));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_UnlimitedFreqLimitation3() {
			/// *******************************************************
			/// Frequency Limitation is -1 (unlimited) for all categories, annual max is 100
			/// Discount Plan sub date range is the start of this calander year (minval), to Now + 1 year and 1 day
			/// Create 25 procs, all for dateTime now,and attempt to give them all a 5 dollar discount
			/// 5 should fail to be discounted
			/// *******************************************************
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:100);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1).AddDays(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			PrefT.UpdateString(PrefName.DiscountPlanFluorideCodes,procCode1.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,procCode2.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode4.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			for(int i = 0;i<5;i++) {
				Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc1);
				Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc2);
				Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc3);
				Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc4);
				Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc5);
			}
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { procHist[20],procHist[21],procHist[22],procHist[23],procHist[24] };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(discountfreqMessage,"");
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,
				procHist.Where(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistPartiallyExceedsLimits.Select(y=>y.ProcNum).ToList().Contains(x)));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistExceedsLimit.Count,
				procHist.Where(x=>x.DiscountPlanAmt==0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistExceedsLimit.Select(y=>y.ProcNum).ToList().Contains(x)));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlan_GetAnnualTotalsForPatByDiscountPlanSub_UnlimitedFreqLimitation4() {
			/// *******************************************************
			/// Frequency Limitation is -1 (unlimited) for all categories, annual max is 99
			/// Discount Plan sub date range is the start of this calander year (minval), to Now + 1 year and 1 day
			/// Create 25 procs, all for dateTime now,and attempt to give them all a 5 dollar discount
			/// 5 should fail to be discounted, 1 should be partially discounted $1
			/// *******************************************************
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:99);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(1).AddDays(1));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			PrefT.UpdateString(PrefName.DiscountPlanFluorideCodes,procCode1.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,procCode2.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode4.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			for(int i = 0;i<5;i++) {
				Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc1);
				Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc2);
				Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc3);
				Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc4);
				Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(0));
				procHist.Add(proc5);
			}
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> { procHist[19] };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { procHist[20],procHist[21],procHist[22],procHist[23],procHist[24] };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			string discountfreqMessage=DiscountPlans.CheckDiscountFrequency(procHist,pat.PatNum,DateTime.Now,discountPlanSub);
			Assert.AreEqual(discountfreqMessage,"");
			Assert.AreEqual(procHist.Count-procHistPartiallyExceedsLimits.Count-procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,procHist.Count(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0));
			Assert.AreEqual(procHistPartiallyExceedsLimits.Count,
				procHist.Where(x=>x.DiscountPlanAmt<5 && x.DiscountPlanAmt>0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistPartiallyExceedsLimits.Select(y=>y.ProcNum).ToList().Contains(x)));
			Assert.AreEqual(procHistExceedsLimit.Count,procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistExceedsLimit.Count,
				procHist.Where(x=>x.DiscountPlanAmt==0)
				.Select(x=>x.ProcNum).ToList()
				.Count(x=>procHistExceedsLimit.Select(y=>y.ProcNum).ToList().Contains(x)));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}
	}
}
