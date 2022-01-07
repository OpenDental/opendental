using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Adjustments_Tests {
	[TestClass]
	public class AdjustmentsTests:TestBase {
		protected static Provider _salesTaxProvider=new Provider();
		protected static long _salesTaxProviderOld;
		protected static Provider _defaultPracticeProvider=new Provider();
		protected static long _defaultPracticeProviderOld;
		protected static Provider _clinicProv=new Provider();

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		protected static void SetupTest() {
			PatientT.ClearPatientTable();
			ProcedureT.ClearProcedureTable();
			AdjustmentT.ClearAdjustmentTable();
			_salesTaxProviderOld=PrefC.GetLong(PrefName.SalesTaxDefaultProvider);
			_salesTaxProvider.ProvNum=Providers.Insert(new Provider());
			_defaultPracticeProviderOld=PrefC.GetLong(PrefName.PracticeDefaultProv);
			_defaultPracticeProvider.ProvNum=Providers.Insert(new Provider());
			_clinicProv.ProvNum=Providers.Insert(new Provider());
			Providers.RefreshCache();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		protected static void TearDownTest() {
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProviderOld);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,_salesTaxProviderOld);
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		#region CreateAdjustmentForSalesTax
		[TestMethod]
		///<summary>Tests that non-taxable procedures are skipped.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_SkipsNonTaxedProcedures() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D1110",ProcStat.C,"",10.00);
			ProcedureCodeT.SetIsTaxed("D1110",false);
			ProcedureCodes.RefreshCache();
			Adjustments.CreateAdjustmentForSalesTax(proc);
			var test=Adjustments.GetSalesTaxForProc(proc.ProcNum);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(0,listAdjustments.Count);
		}

		[TestMethod]
		///<summary>Tests that the SalesTaxDefaultProvider is used on an automatic run if set.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_UsesSalesTaxDefaultProvider() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00);
			ProcedureCodeT.SetIsTaxed("D0120");
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProvider.ProvNum);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,_salesTaxProvider.ProvNum);
			Adjustments.CreateAdjustmentForSalesTax(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Assert.AreEqual(_salesTaxProvider.ProvNum,listAdjustments[0].ProvNum);
		}

		[TestMethod]
		///<summary>Tests that if clinics are enabled and has a default provider, it will use that
		///over the practice default provider.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_UsesClinicProvIfNoSalesTaxDefaultProv() {
			Clinic clinic=ClinicT.CreateClinic();
			clinic.DefaultProv=_clinicProv.ProvNum;
			Clinics.Update(clinic);
			Clinics.RefreshCache();
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00);
			ProcedureCodeT.SetIsTaxed("D0120");
			Procedure updatedProc=proc.Copy();
			updatedProc.ClinicNum=clinic.ClinicNum;
			Procedures.Update(updatedProc,proc);
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProvider.ProvNum);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,0);
			Adjustments.CreateAdjustmentForSalesTax(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Assert.AreEqual(_clinicProv.ProvNum,listAdjustments[0].ProvNum);
		}

		[TestMethod]
		///<summary>Tests that it will use the practice default provider if it's an automatic run
		///and there is no clinics set up / clinic default provider.</summary>
		public void Adjustments_CreateAdjustmentForSalesTax_UsesPracticeProvIfNoSalesTaxOrClinicDefaultProv() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",10.00);
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,_defaultPracticeProvider.ProvNum);
			Prefs.UpdateLong(PrefName.SalesTaxDefaultProvider,0);
			ProcedureCodeT.SetIsTaxed("D0120");
			Adjustments.CreateAdjustmentForSalesTax(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Assert.AreEqual(_defaultPracticeProvider.ProvNum,listAdjustments[0].ProvNum);
		}
		#endregion CreateAdjustmentForSalesTax

		#region Adjustments_AddSalesTaxIfNoneExists
		///<summary>Tests that when using this method, only one instance of sales tax will be created.</summary>
		[TestMethod]
		public void Adjustments_AddSalesTaxIfNoneExists_DoesNotCreateMultipleAdjustments() {
			Patient pat=PatientT.CreatePatient();
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",0);
			ProcedureCodeT.SetIsTaxed("D0120");
			Adjustments.AddSalesTaxIfNoneExists(proc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
			Adjustments.AddSalesTaxIfNoneExists(proc);
			listAdjustments=Adjustments.GetListForProc(proc.ProcNum);
			Assert.AreEqual(1,listAdjustments.Count);
		}

		[TestMethod]
		public void Adjustments_AddSalesTaxIfNoneExists_DoesNotAddSalesTaxIfProcHasOrthoLink() {
			Patient pat=PatientT.CreatePatient();
			Procedure bandingProc=ProcedureT.CreateProcedure(pat,"D8080",ProcStat.C,"",0);
			ProcedureCodeT.SetIsTaxed("D8080");
			OrthoCaseT.CreateOrthoCase(pat.PatNum,2000,1200,0,800,DateTime.Today,false,DateTime.Today.AddMonths(12),1000,400,60,bandingProc);
			Adjustments.AddSalesTaxIfNoneExists(bandingProc);
			List<Adjustment> listAdjustments=Adjustments.GetListForProc(bandingProc.ProcNum);
			Assert.AreEqual(listAdjustments.Count,0);
		}

		[TestMethod]
		public void Adjustments_CreateAdjustmentForDiscountPlan_NoLimitationsMet() {
			ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:3,paFreqLimit:2,annualMax:25);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				double adjustmentCreated=Adjustments.CreateAdjustmentForDiscountPlan(procHist[i]);
				if(procHistExceedsLimit.Contains(procHist[i])) {
					Assert.AreEqual(0,adjustmentCreated);
				} else if(procHistPartiallyExceedsLimits.Contains(procHist[i])) {
					Assert.AreEqual(true,adjustmentCreated>0 && adjustmentCreated<5);
				} else {
					Assert.AreEqual(5,adjustmentCreated);
				}
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
			}
			ClearDiscountPlanPrefs();
		}

		[TestMethod]
		public void Adjustments_CreateAdjustmentForDiscountPlan_SomeLimitationsMet1() {
			ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:3,paFreqLimit:2,annualMax:18);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode4.ProcCode+","+procCode5.ProcCode);
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
				double adjustmentCreated=Adjustments.CreateAdjustmentForDiscountPlan(procHist[i]);
				if(procHistExceedsLimit.Contains(procHist[i])) {
					Assert.AreEqual(0,adjustmentCreated);
				} else if(procHistPartiallyExceedsLimits.Contains(procHist[i])) {
					Assert.AreEqual(true,adjustmentCreated>0 && adjustmentCreated<5);
				} else {
					Assert.AreEqual(5,adjustmentCreated);
				}
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
			}
			ClearDiscountPlanPrefs();
		}

		[TestMethod]
		public void Adjustments_CreateAdjustmentForDiscountPlan_SomeLimitationsMet2() {
			ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:2,paFreqLimit:2,annualMax:-1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc3 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				double adjustmentCreated=Adjustments.CreateAdjustmentForDiscountPlan(procHist[i]);
				if(procHistExceedsLimit.Contains(procHist[i])) {
					Assert.AreEqual(0,adjustmentCreated);
				} else if(procHistPartiallyExceedsLimits.Contains(procHist[i])) {
					Assert.AreEqual(true,adjustmentCreated>0 && adjustmentCreated<5);
				} else {
					Assert.AreEqual(5,adjustmentCreated);
				}
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			ClearDiscountPlanPrefs();
		}

		[TestMethod]
		public void Adjustments_CreateAdjustmentForDiscountPlan_SomeLimitationsMet3() {
			ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:1,paFreqLimit:1,annualMax:-1);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc2,proc3,proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				double adjustmentCreated=Adjustments.CreateAdjustmentForDiscountPlan(procHist[i]);
				if(procHistExceedsLimit.Contains(procHist[i])) {
					Assert.AreEqual(0,adjustmentCreated);
				} else if(procHistPartiallyExceedsLimits.Contains(procHist[i])) {
					Assert.AreEqual(true,adjustmentCreated>0 && adjustmentCreated<5);
				} else {
					Assert.AreEqual(5,adjustmentCreated);
				}
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
				Procedures.Update(procHist[i],procOld);
			}
			ClearDiscountPlanPrefs();
		}

		[TestMethod]
		public void Adjustments_CreateAdjustmentForDiscountPlan_AllLimitationsMet1() {
			ClearDiscountPlanPrefs();
			Patient pat=PatientT.CreatePatient();
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"DPFS");
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("DPT",feeSchedNum:feeSchedNum,examFreqLimit:3,paFreqLimit:2,annualMax:0);
			DiscountPlanSub discountPlanSub=DiscountPlanSubT.CreateDiscountPlanSub(pat.PatNum,discountPlan.DiscountPlanNum,dateEffective:DateTime.MinValue,dateTerm:DateTime.Now.AddYears(5));
			ProcedureCode procCode1=ProcedureCodeT.CreateProcCode("DPTPC1");
			long feeNum1=FeeT.CreateFee(feeSchedNum,procCode1.CodeNum,amount:95);
			Procedure proc1=ProcedureT.CreateProcedure(pat,procCode1.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode("DPTPC2");
			long feeNum2=FeeT.CreateFee(feeSchedNum,procCode2.CodeNum,amount:95);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procCode2.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode3=ProcedureCodeT.CreateProcCode("DPTPC3");
			long feeNum3=FeeT.CreateFee(feeSchedNum,procCode3.CodeNum,amount:95);
			Procedure proc3=ProcedureT.CreateProcedure(pat,procCode3.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode4=ProcedureCodeT.CreateProcCode("DPTPC4");
			long feeNum4=FeeT.CreateFee(feeSchedNum,procCode4.CodeNum,amount:95);
			Procedure proc4=ProcedureT.CreateProcedure(pat,procCode4.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			ProcedureCode procCode5=ProcedureCodeT.CreateProcCode("DPTPC5");
			long feeNum5=FeeT.CreateFee(feeSchedNum,procCode5.CodeNum,amount:95);
			Procedure proc5=ProcedureT.CreateProcedure(pat,procCode5.ProcCode,ProcStat.TP,"",100,procDate:DateTime.Now.AddYears(1));
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,procCode1.ProcCode+","+procCode2.ProcCode+","+procCode3.ProcCode);
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,procCode4.ProcCode+","+procCode5.ProcCode);
			List<Procedure> procHist=new List<Procedure>();
			procHist.Add(proc1);
			procHist.Add(proc2);
			procHist.Add(proc3);
			procHist.Add(proc4);
			procHist.Add(proc5);
			List<Procedure> procHistPartiallyExceedsLimits=new List<Procedure> {  };
			List<Procedure> procHistExceedsLimit=new List<Procedure> { proc1,proc2,proc3,proc4,proc5 };
			List<long> feeSchedNums=new List<long>();
			feeSchedNums.Add(feeNum1);
			feeSchedNums.Add(feeNum2);
			feeSchedNums.Add(feeNum3);
			feeSchedNums.Add(feeNum4);
			feeSchedNums.Add(feeNum5);
			Procedure procOld;
			for(int i=0;i<procHist.Count;i++) {
				double adjustmentCreated=Adjustments.CreateAdjustmentForDiscountPlan(procHist[i]);
				if(procHistExceedsLimit.Contains(procHist[i])) {
					Assert.AreEqual(0,adjustmentCreated);
				} else if(procHistPartiallyExceedsLimits.Contains(procHist[i])) {
					Assert.AreEqual(true,adjustmentCreated>0 && adjustmentCreated<5);
				} else {
					Assert.AreEqual(5,adjustmentCreated);
				}
				procOld=procHist[i].Copy();
				procHist[i].ProcStatus=ProcStat.C;
			}
			ClearDiscountPlanPrefs();
		}

		public void ClearDiscountPlanPrefs() {
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanFluorideCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanPerioCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,"");
		}
		#endregion

	}
}
