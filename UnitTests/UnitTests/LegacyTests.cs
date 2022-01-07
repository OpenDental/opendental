using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;
using UnitTestsCore;

//Temp code to be moved elsewhere if we keep it:
namespace UnitTests.Documentation{
	///<summary>Only unit tests that are getting publicly documented receive a number.  This number never gets changed, and the behavior of a numbered and documented unit test never gets changed.  If behavior needs to change, deprecate the old one and create a new one.  The numbering does not need to be strictly sequential, nor does it need to be based on when the unit test was written.  Think of it as a primary key which has no meaning other than for reference purposes.  Legacy tests are less than 100, and the new tests start at 100.</summary>
	public class Numbering:System.Attribute{
		public EnumTestNum TestNum;

		public Numbering(EnumTestNum testNum){
			TestNum=testNum;
		}
	}

	///<summary>Example "21.1"</summary>
	public class VersionAdded:System.Attribute{
		public string Version;

		public VersionAdded(string version){
			Version=version;
		}
	}

	///<summary></summary>
	public class Description:System.Attribute{
		public string Desc;

		public Description(string desc){
			Desc=desc;
		}
	}

	///<summary>See Wiki Unit Test Documentation. Probably call Jordan. Never change the number assigned to an existing enum item.</summary>
	public enum EnumTestNum{
		///<summary>Currently documented separately as tests 1 and 2.  Need to fix documentation.</summary>
		Procedures_ComputeEstimates_Allowed1Allowed2=1,
		Procedures_ComputeEstimates_ZeroCoverageOverAnnualMax=3,
		Procedures_ComputeEstimates_FamilyUnderAnnualMax=4,
		Procedures_ComputeEstimates_IndividualAndFamilyMax=5,
		InsPlan_GetInsUsedDisplay_LimitationsOverride=6,
		Procedures_ComputeEstimates_PreventitiveDiagnosticDeductibleOnlyOnce=7,
		Claims_CalculateAndUpdate_Allowed1Allowed2CompletedProcedures=8,
		Procedures_ComputeEstimates_LimitationsOverrideGeneralLimitations=9,
		thisisisatest,
		Procedures_ComputeEstimates_AnnualMaxReachedZeroCoverage=10,
		Procedures_ComputeEstimates_FamilyMaxNoIndividualMax=11,
		Procedures_ComputeEstimates_WriteoffPPOsPriSecSamePlan=12,
		InsPlan_GetInsUsedDisplay_OrthoProcsNotAffectInsUsed=13,
		Procedures_ComputeEstimates_PriEstNotAffectedBySecClaim=14,
		Procedures_ComputeEstimates_DeductibleOverrides=15,
		Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductible=16,
		Procedures_ComputeEstimates_COBStandardTwoPPOs=17,
		Procedures_ComputeEstimates_COBCarveOutCategoryPercentagePlan=18,
		Procedures_ComputeEstimates_MultipleDeductibles=19,
		InsPlan_GetDedRemainDisplay_IndividualAndFamilyDeductiblesInsRemaining=20,
		Procedures_ComputeEstimates_DeductiblesForProcsNotCovered=21,
		//22 is deprecated
		//23 is deprecated
		TimeCardRules_CalculateDailyOvertime_WithSplitBreak=24,
		TimeCardRules_CalculateWeeklyOvertime_DuringNormalWorkWeek=25,
		TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriods=26,
		TimeCardRules_CalculateWeeklyOvertime_OneWeekWorkWeekStartsOnWednesday=27,
		ClaimProcs_FormClaimProc_TextBoxValuesFromChartModule=28,
		ClaimProcs_FormClaimProc_TextBoxValuesFromClaimEditWindow=29,
		ClaimProcs_FormClaimProc_TextBoxValuesFromTreatPlanModule=30,
		InsPlan_GetPendingDisplay_LimitationsOverrideGeneralLimitations=31,
		TimeCardRules_CalculateDailyOvertime_ForHoursWorkedAfterACertainTime=32,
		TimeCardRules_CalculateDailyOvertime_ForHoursWorkedBeforeACertainTime=33,
		Procedures_ComputeEstimates_GeneralDeductiblesConsideredWithProcedureSpecificDeductibles=34,
		Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductible=35,
		Procedures_ComputeEstimates_COBStandardDualPPOWriteoffZero=36,
		Procedures_ComputeEstimates_PPOProceduresMultipleUnits=37,
		Procedures_ComputeEstimates_CategoryPercentageProceduresMultipleUnits=38,
		Procedures_ComputeEstimates_PPOProceduresMultipleUnitsWriteoff=39,
		Procedures_ComputeEstimates_InsPPOsecWriteoffsPreference=40,
		PaymentEdit_Init_CorrectlyOrderedAutoSplits=41,
		PaymentEdit_Init_CorrectlyOrderedAutoSplitsWithExistingPayment=42,
		PaymentEdit_Init_AutoSplitOverAllocation=43,
		PaymentEdit_Init_AutoSplitForPaymentNegativePaymentAmount=44,
		PaymentEdit_Init_AutoSplitWithAdjustmentAndExistingPayment=45,
		PaymentEdit_Init_AutoSplitForPaymentNegativePaymentAmountNegProcedure=46,
		PaymentEdit_Init_AutoSplitProcedureGuarantor=47,
		PaymentEdit_Init_AutoSplitWithClaimPayments=48,
		Procedures_ComputeEstimates_OverrideUnderFamilyMaxEstimateNote=49,
		Procedures_ComputeEstimates_OverrideOverFamilyMaxEstimateNote=50,
		Procedures_ComputeEstimates_MedicalDeductible=51,
		SignatureBoxWrapper_FillSignature_GetNumberOfTabletPoints=52,
		Legacy_TestFiftyThree=53,
		Legacy_TestFiftyFour=54,
		Legacy_TestFiftyFive=55,
		Legacy_TestFiftySix=56,
		Fees_GetFee_ProviderSpecificFees=57,
		Fees_GetFee_ClinicSpecificFees=58,
		Fees_GetFee_ClinicAndProviderSpecificFees=59,
		Claims_CalculateAndUpdate_ProcedureCodeDowngradeBlankFee=60,
		Claims_CalculateAndUpdate_ProcedureCodeDowngradeHigherFee=61,
		TimeCardRules_CalculateWeeklyOvertime_ForDifferentClinics=62,
		TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriodsForDifferentClinics=63,
		TimeCardRules_CalculateWeeklyOvertime_OneWeekOverTwoPayPeriodsForDifferentClinicPreferences=64,
		Procedures_ComputeEstimates_MedicalFlatCopaySecondaryFeeSchedule=65,
		TimeCardRules_CalculateWeeklyOvertime_ForDifferentClinicsRealData=66,
		TimeCardRules_CalculateWeeklyOvertime_CalculationWithManualOvertime=67,
		Procedures_ComputeEstimates_CategoryPercentageCanadianLabFees=73,
		Benefits_GetAnnualMaxDisplay_CalendarYearBenefit=74,
		Procedures_ComputeEstimates_OrthoMaxSeparateFamilyAndIndividualMaxes=75,
		Procedures_ComputeEstimates_OrthoFamilyMaxOverLargerIndividual=76,
		Appointments_GetApptTimePatternFromProcPatterns_PatternLogic=79,
		Procedures_ComputeEstimates_DeductiblesDualInsuranceNotNegative=80,
		Procedures_ComputeEstimates_FrequencyLimitationMet=81,
		Procedures_ComputeEstimates_MultipleProceduresOneClaimExceedAnnualMax=83,
		Procedures_ComputeEstimates_COBCarveOutSecondaryInsurance=84,
		RecurringCharges_FormCreditRecurringCharges_PaymentPlansV1V2=85,
		Procedures_ComputeEstimates_SalesTaxCalculation_EstimateWriteOff=86,
		Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductibleClaim=100,
		Procedures_ComputeEstimates_CategoryDeductiblesDontExceedLimits=101,
		AccountEntry_MultipleAdjustments_ConsiderPatPayments=102,
		ClaimProcs_ApplyAsTotalPayment_InsuranceEstimateGreaterThanFeeBilled=103,
		ClaimProcs_ApplyAsTotalPayment_TotalAmtPaidGreaterThanTotalProcFees=104,
		PaymentEdit_BalanceAndIncomeTransfer_PayPlansOverpaidInterest=105,
		PaymentEdit_ConstructAndLinkChargeCredits_OffsettingUnattachedAdjustments=114,
		//Add new items to enum here at end of list
	}
}


