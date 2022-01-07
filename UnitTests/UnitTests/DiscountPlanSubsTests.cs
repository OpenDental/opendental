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

namespace UnitTests.DiscountPlanSubsTests {
	[TestClass]
	public class DiscountPlanSubsTests:TestBase {
		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRange1() {
			//*****************************************************************
			//Expected Annual Range: 01/01/Now.Year - 12/31/Now.Year
			//DP Effective: MinVal
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DateTime refDate=DateTime.Now;
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(true,buckets>index);
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(startDate,new DateTime(DateTime.Now.Year,1,1));
			Assert.AreEqual(stopDate,new DateTime(DateTime.Now.Year,12,31));
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRange2() {
			//*****************************************************************
			//Annual Range: 01/01/Now.Year - 12/31/Now.Year
			//DP Effective: MinVal
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DateTime refDate=new DateTime(DateTime.Now.Year,12,31);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(true,buckets>index);
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(startDate,new DateTime(DateTime.Now.Year,1,1));
			Assert.AreEqual(stopDate,new DateTime(DateTime.Now.Year,12,31));
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRange3() {
			//*****************************************************************
			//Annual Range: 01/01/Now.Year - 12/31/Now.Year
			//DP Effective: MinVal
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DateTime refDate=new DateTime(DateTime.Now.Year,1,1);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(true,buckets>index);
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(startDate,new DateTime(DateTime.Now.Year,1,1));
			Assert.AreEqual(stopDate,new DateTime(DateTime.Now.Year,12,31));
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRange4() {
			//*****************************************************************
			//Annual Range: 01/01/Now.Year+1 - 12/31/Now.Year+1
			//DP Effective: MinVal
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DateTime refDate=new DateTime(DateTime.Now.Year+1,1,1);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(true,buckets>index);
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(new DateTime(DateTime.Now.Year+1,1,1),startDate);
			Assert.AreEqual(new DateTime(DateTime.Now.Year+1,12,31),stopDate);
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsNotInRange1() {
			//*****************************************************************
			//Annual Range: 01/01/Now.Year - 12/31/Now.Year
			//DP Effective: MinVal
			//DP Term: 12/31/Now.Year
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateTerm:new DateTime(DateTime.Now.Year,12,31));
			DateTime refDate=new DateTime(DateTime.Now.Year+1,1,1);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(-1,index);
			Assert.AreEqual(1,buckets);
			Assert.AreEqual(isInRange,false);
			Assert.AreEqual(new DateTime(DateTime.Now.Year,1,1),startDate);
			Assert.AreEqual(new DateTime(DateTime.Now.Year,12,31),stopDate);
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsNotInRange2() {
			//*****************************************************************
			//Annual Range: 01/01/Now.Year - 12/30/Now.Year
			//DP Effective: MinVal
			//DP Term: 12/30/Now.Year
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateTerm:new DateTime(DateTime.Now.Year,12,30));
			DateTime refDate=new DateTime(DateTime.Now.Year+1,1,1);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(-1,index);
			Assert.AreEqual(1,buckets);
			Assert.AreEqual(isInRange,false);
			Assert.AreEqual(new DateTime(DateTime.Now.Year,1,1),startDate);
			Assert.AreEqual(new DateTime(DateTime.Now.Year,12,30),stopDate);
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRangeLeapYear1() {
			//*****************************************************************
			//Annual Range: 01/01/2020 - 12/31/2020
			//DP Effective: 02/29/2020
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:new DateTime(2020,1,1));
			DateTime refDate=new DateTime(2020,1,1);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(1,buckets);
			Assert.AreEqual(new DateTime(2020,1,1),startDate);
			Assert.AreEqual(new DateTime(2020,12,31),stopDate);
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRangeLeapYear2() {
			//*****************************************************************
			//Annual Range: 02/29/2020 - 02/28/2021
			//DP Effective: 02/29/2020
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:new DateTime(2020,2,29));
			DateTime refDate=new DateTime(2021,2,28);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(true,buckets>index);
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(2,buckets);
			Assert.AreEqual(new DateTime(2021,2,28),startDate);
			Assert.AreEqual(new DateTime(2022,2,27),stopDate);
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_GetAnnualDateRangeSegmentForGivenDate_IsInRangeLeapYear3() {
			//*****************************************************************
			//Annual Range: 02/29/2020 - 02/28/2021
			//DP Effective: 02/29/2020
			//DP Term: MinVal
			//*****************************************************************
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:4);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:new DateTime(2020,2,29));
			DateTime refDate=new DateTime(2021,2,27);
			bool isInRange=DiscountPlanSubs.GetAnnualDateRangeSegmentForGivenDate(discountPlanSub,refDate,out DateTime startDate,out DateTime stopDate);
			int buckets=Adjustments.GetAnnualTotalsForPatByDiscountPlanSub(discountPlanSub,discountPlan,refDate).Count;
			int index=Adjustments.GetAnnualMaxSegmentIndex(discountPlanSub,refDate);
			Assert.AreEqual(true,buckets>index);
			Assert.AreEqual(isInRange,true);
			Assert.AreEqual(1,buckets);
			Assert.AreEqual(new DateTime(2021,2,28),startDate);
			Assert.AreEqual(new DateTime(2022,2,27),stopDate);
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_UpdateAssociatedDiscountPlanAmts_AllProcsInRange() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimit=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> {  };
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub });
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count-procHistExceedsLimit.Count-procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>0 && x.DiscountPlanAmt<5));
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub },true);
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_UpdateAssociatedDiscountPlanAmts_SomeProcsInDateRange() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(-1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(-1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimit=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc3,proc4 };
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub });
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count-procHistExceedsLimit.Count-procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>0 && x.DiscountPlanAmt<5));
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub },true);
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_UpdateAssociatedDiscountPlanAmts_SomeProcsInAnnualMaxRange1() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:15);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimit=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc4,proc5 };
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub });
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count-procHistExceedsLimit.Count-procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>0 && x.DiscountPlanAmt<5));
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub },true);
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_UpdateAssociatedDiscountPlanAmts_SomeProcsInAnnualMaxRange2() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,annualMax:13);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimit=new List<Procedure> { proc3 };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc4,proc5 };
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub });
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count-procHistExceedsLimit.Count-procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>0 && x.DiscountPlanAmt<5));
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub },true);
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_UpdateAssociatedDiscountPlanAmts_SomeProcsInFreqLimitRange1() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,xrayFreqLimit:3);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimit=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc4,proc5 };
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub });
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count-procHistExceedsLimit.Count-procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>0 && x.DiscountPlanAmt<5));
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub },true);
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}

		[TestMethod]
		///<Summary></Summary>
		public void DiscountPlanSubs_UpdateAssociatedDiscountPlanAmts_SomeProcsInFreqLimitRange2() {
			DiscountPlanT.ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,xrayFreqLimit:4);
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now);
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode+","+procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimit=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc5 };
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum);
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub });
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count-procHistExceedsLimit.Count-procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==5));
			Assert.AreEqual(procHistExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			Assert.AreEqual(procHistPartiallyExceedsLimit.Count, procHist.Count(x=>x.DiscountPlanAmt>0 && x.DiscountPlanAmt<5));
			DiscountPlanSubs.UpdateAssociatedDiscountPlanAmts(new List<DiscountPlanSub>{ discountPlanSub },true);
			procHist=Procedures.GetManyProc(procHist.Select(x=>x.ProcNum).ToList(),false);
			Assert.AreEqual(procHist.Count, procHist.Count(x=>x.DiscountPlanAmt==0));
			DiscountPlanT.ClearDiscountPlanPrefs();
		}
	}
}
