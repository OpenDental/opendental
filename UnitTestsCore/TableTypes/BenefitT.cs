using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class BenefitT {
		public static void CreateAnnualMax(long planNum,double amt){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=0;
			ben.CoverageLevel=BenefitCoverageLevel.Individual;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
		}

		public static Benefit CreateDeductibleGeneral(long planNum,BenefitCoverageLevel coverageLevel,double amt){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Deductible;
			ben.CovCatNum=0;
			ben.CoverageLevel=coverageLevel;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreateAnnualMax(long planNum,BenefitCoverageLevel coverageLevel,double amt){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=0;
			ben.CoverageLevel=coverageLevel;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			return ben;
		}

		public static void CreateDeductible(long planNum,EbenefitCategory category,double amt,BenefitCoverageLevel coverage=BenefitCoverageLevel.Individual){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Deductible;
			ben.CovCatNum=CovCats.GetForEbenCat(category).CovCatNum;
			ben.CoverageLevel=coverage;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
		}

		/// <summary>Takes an individual codeNum instead of a category.</summary>
		public static void CreateDeductible(long planNum,string procCodeStr,double amt){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Deductible;
			ben.CovCatNum=0;
			ben.CodeNum=ProcedureCodeT.GetCodeNum(procCodeStr);
			ben.CoverageLevel=BenefitCoverageLevel.Individual;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
		}

		public static void CreateLimitation(long planNum,EbenefitCategory category,double amt){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=CovCats.GetForEbenCat(category).CovCatNum;
			ben.CoverageLevel=BenefitCoverageLevel.Individual;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
		}

		public static Benefit CreateLimitationProc(long planNum,string procCodeStr,double amt) {
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CodeNum=ProcedureCodeT.GetCodeNum(procCodeStr);
			ben.CoverageLevel=BenefitCoverageLevel.Individual;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			return ben;
		}

		public static void CreateAnnualMaxFamily(long planNum,double amt){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=0;
			ben.CoverageLevel=BenefitCoverageLevel.Family;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
		}

		public static Benefit CreateCategoryPercent(long planNum,EbenefitCategory category,int percent){
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.CoInsurance;
			ben.CovCatNum=CovCats.GetForEbenCat(category).CovCatNum;
			ben.CoverageLevel=BenefitCoverageLevel.None;
			ben.Percent=percent;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreatePercentForProc(long planNum,long codeNum,int percent) {
			Benefit ben=new Benefit();
			ben.CodeNum=codeNum;
			ben.BenefitType=InsBenefitType.CoInsurance;
			ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
			ben.Percent=percent;
			ben.PlanNum=planNum;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreateExclusionForCategory(long planNum,EbenefitCategory category) {
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.CovCatNum=CovCats.GetForEbenCat(category).CovCatNum;
			ben.BenefitType=InsBenefitType.Exclusions;
			ben.CoverageLevel=BenefitCoverageLevel.None;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreateExclusion(long planNum,long codeNum) {
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Exclusions;
			ben.CoverageLevel=BenefitCoverageLevel.None;
			ben.TimePeriod=BenefitTimePeriod.CalendarYear;
			ben.CodeNum=codeNum;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreateFrequencyProc(long planNum,string procCodeStr,BenefitQuantity quantityQualifier,Byte quantity,
			BenefitTimePeriod timePeriod=BenefitTimePeriod.None)
		{
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=0;
			ben.CodeNum=ProcedureCodeT.CreateProcCode(procCodeStr).CodeNum;
			ben.CoverageLevel=BenefitCoverageLevel.None;
			ben.TimePeriod=timePeriod;
			ben.Quantity=quantity;
			ben.QuantityQualifier=quantityQualifier;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreateFrequencyCategory(long planNum,EbenefitCategory category,BenefitQuantity quantityQualifier,Byte quantity,
			BenefitCoverageLevel coverageLevel=BenefitCoverageLevel.None,long codeNum=0) 
		{
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CodeNum=codeNum;
			ben.CovCatNum=CovCats.GetForEbenCat(category)?.CovCatNum??0;
			ben.CoverageLevel=coverageLevel;
			ben.TimePeriod=BenefitTimePeriod.None;
			ben.Quantity=quantity;
			ben.QuantityQualifier=quantityQualifier;
			Benefits.Insert(ben);
			return ben;
		}

		public static Benefit CreateAgeLimitation(long planNum,EbenefitCategory category,int ageThrough,
			BenefitCoverageLevel coverageLevel=BenefitCoverageLevel.None,long codeNum=0) 
		{
			return CreateFrequencyCategory(planNum,category,BenefitQuantity.AgeLimit,(byte)ageThrough,coverageLevel,codeNum);
		}

		public static void CreateOrthoMax(long planNum,double amt) {
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
			ben.CoverageLevel=BenefitCoverageLevel.Individual;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.Lifetime;
			Benefits.Insert(ben);
		}

		public static void CreateOrthoFamilyMax(long planNum,double amt) {
			Benefit ben=new Benefit();
			ben.PlanNum=planNum;
			ben.BenefitType=InsBenefitType.Limitations;
			ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
			ben.CoverageLevel=BenefitCoverageLevel.Family;
			ben.MonetaryAmt=amt;
			ben.TimePeriod=BenefitTimePeriod.Lifetime;
			Benefits.Insert(ben);
		}

		
		public static string BenefitComputeRenewDate(){
			DateTime asofDate=new DateTime(2006,3,19);
			//bool isCalendarYear=true;
			//DateTime insStartDate=new DateTime(2003,3,1);
			DateTime result=BenefitLogic.ComputeRenewDate(asofDate,0);
			if(result!=new DateTime(2006,1,1)){
				throw new ApplicationException("BenefitComputeRenewDate 1 failed.\r\n");
			}
			//isCalendarYear=false;//for the remaining tests
			//earlier in same month
			result=BenefitLogic.ComputeRenewDate(asofDate,3);
			if(result!=new DateTime(2006,3,1)) {
				throw new ApplicationException("BenefitComputeRenewDate 2 failed.\r\n");
			}
			//earlier month in year
			asofDate=new DateTime(2006,5,1);
			result=BenefitLogic.ComputeRenewDate(asofDate,3);
			if(result!=new DateTime(2006,3,1)) {
				throw new ApplicationException("BenefitComputeRenewDate 3 failed.\r\n");
			}
			asofDate=new DateTime(2006,12,1);
			result=BenefitLogic.ComputeRenewDate(asofDate,3);
			if(result!=new DateTime(2006,3,1)) {
				throw new ApplicationException("BenefitComputeRenewDate 4 failed.\r\n");
			}
			//later month in year
			asofDate=new DateTime(2006,2,1);
			result=BenefitLogic.ComputeRenewDate(asofDate,3);
			if(result!=new DateTime(2005,3,1)) {
				throw new ApplicationException("BenefitComputeRenewDate 5 failed.\r\n");
			}
			asofDate=new DateTime(2006,2,12);
			result=BenefitLogic.ComputeRenewDate(asofDate,3);
			if(result!=new DateTime(2005,3,1)) {
				throw new ApplicationException("BenefitComputeRenewDate 6 failed.\r\n");
			}
			//Insurance start date not on the 1st.//no longer possible
			//asofDate=new DateTime(2008,5,10);
			//insStartDate=new DateTime(2007,1,12);
			//result=BenefitLogic.ComputeRenewDate(asofDate,isCalendarYear,insStartDate);
			//if(result!=new DateTime(2008,1,1)) {
			//	textResults.Text+="BenefitComputeRenewDate 7 failed.\r\n";
			//}
			return "BenefitComputeRenewDates passed.\r\n";
		}

		public static Benefit CreateFrequencyLimitation(string procCode,byte quantity,BenefitQuantity quantityQualifier,long planNum,
			BenefitTimePeriod timePeriod) 
		{
			Benefit ben=Benefits.CreateFrequencyBenefit(ProcedureCodeT.CreateProcCode(procCode).CodeNum,quantity,quantityQualifier,planNum,timePeriod);
			Benefits.Insert(ben);
			return ben;
		}

		///<summary>Deletes everything from the benefit table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearBenefitTable() {
			string command="DELETE FROM benefit";
			DataCore.NonQ(command);
		}

	}
}
