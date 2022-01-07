using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.CovCatsTests {
	[TestClass]
	public class CovCatsTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
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
		public void CovCats_GetAmtUsedForCat() {
			CultureInfo curCulture=CultureInfo.CurrentCulture;
			Thread.CurrentThread.CurrentCulture=new CultureInfo("en-CA");//Canada
			PrefT.UpdateBool(PrefName.InsChecksFrequency,true);
			ProcedureCodeT.AddIfNotPresent("11111");
			ProcedureCode procCode=ProcedureCodes.GetOne("11111");
			procCode.CanadaTimeUnits=2;
			ProcedureCodeT.Update(procCode);
			ProcedureCodes.RefreshCache();
			CovCats.SetSpansToDefaultCanada();
			DeleteCovCats(new List<string> { "SC/RP","Added by Mistake" });//Delete existing covcats for the two we are about to create.
			CovCats.RefreshCache();
			CovSpans.RefreshCache();
			//Create CovCats
			CovCat catScRp=new CovCat();
			catScRp.Description="SC/RP";
			catScRp.DefaultPercent=-1;
			catScRp.CovOrder=(byte)(CovCats.GetCount()+1);
			catScRp.CovCatNum=CovCats.Insert(catScRp);
			CovCat catMistake=new CovCat();
			catMistake.Description="Added by Mistake";
			catMistake.DefaultPercent=-1;
			catMistake.CovOrder=(byte)(CovCats.GetCount()+5);
			catMistake.IsHidden=true;
			catMistake.CovCatNum=CovCats.Insert(catMistake);
			//Create CovSpan
			CovSpan covSpanScRp=new CovSpan();
			covSpanScRp.CovCatNum=catScRp.CovCatNum;
			covSpanScRp.FromCode="11111";
			covSpanScRp.ToCode="11119";
			CovSpan covSpanMistake=new CovSpan();
			covSpanMistake.CovCatNum=catMistake.CovCatNum;
			covSpanMistake.FromCode="11111";
			covSpanMistake.ToCode="11119";
			CovSpans.Insert(covSpanScRp);//this order is important
			CovSpans.Insert(covSpanMistake);//Add the mistake covspan second
			//Add two additional covspans associated to the covcat that was inserted by "mistake" with different code ranges. 
			covSpanMistake.FromCode="11111";
			covSpanMistake.ToCode="11117";
			CovSpans.Insert(covSpanMistake);
			covSpanMistake.FromCode="43421";
			covSpanMistake.ToCode="43429";
			CovSpans.Insert(covSpanMistake);
			CovCats.RefreshCache();
			CovSpans.RefreshCache();
			try {
				string suffix=MethodBase.GetCurrentMethod().Name;
				Patient pat=PatientT.CreatePatient(suffix);
				InsuranceInfo ins=InsuranceT.AddInsurance(pat,suffix);
				ins.ListBenefits.Add(BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum,EbenefitCategory.Diagnostic,100));
				ins.ListBenefits.Add(BenefitT.CreateFrequencyProc(ins.PriInsPlan.PlanNum,"11111",BenefitQuantity.NumberOfServices,3,
					BenefitTimePeriod.CalendarYear));//3 Scaling and root plane per year
				Procedure proc=ProcedureT.CreateProcedure(pat,"11111",ProcStat.C,"",100);
				List<Procedure> listProcs=new List<Procedure> { proc };
				double totalTimeUnits=CovCats.GetAmtUsedForCat(listProcs,catScRp);
				Assert.AreEqual(2,totalTimeUnits);//Time unit for procedure should be 2.
			}
			finally {
				Thread.CurrentThread.CurrentCulture=curCulture;
				CovCats.SetSpansToDefaultUsa();
				CovSpans.RefreshCache();
			}
		}

		private void DeleteCovCats(List<string> listDescription) {
			List<CovCat> listCovCats=CovCats.GetWhere(x => ListTools.In(x.Description,listDescription));
			foreach(CovCat covCat in listCovCats) {
				CovCats.Delete(covCat);
				CovSpans.DeleteForCat(covCat.CovCatNum);
			}
		}
	}
}
