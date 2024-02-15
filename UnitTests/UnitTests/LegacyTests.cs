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
		PaymentEdit_GetIncomeTransferSplitsFIFO_SuggestImplicitTransfers=106,
		PaymentEdit_GetIncomeTransferSplitsFIFO_DoNotTransferImplicitlyLinkedIncome=107,
		PaymentEdit_GetIncomeTransferSplitsFIFO_InsuranceOverpayment=108,
		PaymentEdit_GetIncomeTransferSplitsFIFO_ImplicitlyTransferOverpayments=109,
		PaymentEdit_GetIncomeTransferSplitsFIFO_UnearnedOffsets=110,
		PaymentEdit_GetIncomeTransferSplitsFIFO_InsurancePaymentPlan=111,
		PaymentEdit_ConstructAndLinkChargeCredits_MismatchedOffsettingAdjustments=112,
		PaymentEdit_ConstructAndLinkChargeCredits_DynamicPayPlanInterestCharge=113,
		PaymentEdit_ConstructAndLinkChargeCredits_OffsettingUnattachedAdjustments=114,
		InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Global=115,
		InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Plan=116,
		InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Off=117,
		InsPlans_ComputeEstimates_FrequencySurpassedZerosWriteoff_Global=118,
		InsPlans_ComputeEstimates_FrequencySurpassedZerosWriteoff_Plan=119,
		InsPlans_ComputeEstimates_FrequencySurpassedZerosWriteoff_Off=120,
		InsPlans_ComputeEstimates_AgingSurpassedZerosWriteoff_Global=121,
		InsPlans_ComputeEstimates_AgingSurpassedZerosWriteoff_Plan=122,
		InsPlans_ComputeEstimates_AgingSurpassedZerosWriteoff_Off=123,
		RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcsNotRecievedNoPayment=124,
		RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcsNotRecieved1Payment=125,
		RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcs1NotReceived1ReceivedNoPayment=126,
		RpOutstandingIns_ZeroClaim_CanadaSentClaim1LabFeeNoPayment=127,
		ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTime_OneOp=128,
		ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTime_TwoOps=129,
		ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTime_OneOp=130,
		ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTime_TwoOps=131,
		ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_OneOp=132,
		ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_TwoOps=133,
		ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTimeOp_OneOp=134,
		ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTimeOp_TwoOps=135,
		ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_OneOp_Only=136,
		Procedures_ComputeEstimates_FrequencyLimitation_Ignore_NoBillIns=137,
		ProcMultiVisit_UpdateGroupForProc_UpdatedClaimDates=138,
		ProcMultiVisit_UpdateGroupForProc_UpdateClaimNotPreAuth=139,
		Procedures_HasMetFrequencyLimitation_1DenturePerArchPer5Yrs=140,
		Procedures_HasMetFrequencyLimitation_1CrownPerTthPer5Yrs=141,
		Procedures_HasNotMetFrequencyLimitation_1CrownPerTthPer5Yrs=142,
		Procedures_HasMetFrequencyLimitation_1CompPerTthPer5Yrs=143,
		Procedures_HasMetFrequencyLimitation_3FillingsPerMouthPerYr=144,
		Procedures_HasMetFrequencyLimitation_1CompPerTthPer5Years_3FillingsPerMouthPerYr=145,
		Procedures_HasMetFrequencyLimitation_ExamsGrouped=146,
		Procedures_HasMetFrequencyLimitation_ExamsSeparate=147,
		Benefits_GetDeductibleByCode_DeductLessThanGeneral=148,
		Benefits_UnderAgeWithLifetimeMax=149,
		Benefits_OverAgeWithLifetimeMax=150,
		Benefits_BitewingFrequency=151,
		Benefits_CancerScreeningFrequency=152,
		Benefits_CrownsFrequency=153,
		Benefits_SRPFrequency=154,
		Benefits_SealantAgeLimit=155,
		Benefits_SealantAgeLimit_PatInAgeRangeHasCoverage=156,
		Benefits_BitewingFrequencyCanada=157,
		Benefits_BitewingFrequencyPastYear=158,
		Benefits_TwoReceivedClaimProcsForSameClaim_FrequencyNotMet=159,
		Benefits_ServiceYear_FrequencyMet=160,
		Benefits_ServiceYear_FrequencyMet_SameClaim=161,
		Benefits_ServiceYear_FrequencyMet_RefreshAsOfDate=162,
		Benefits_GetDeductibleByCode_ExcludedDeductible=163,
		Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductibleApplyToCodeSpecificDeductibles=164,
		Benefits_InLast12Months_FrequencyMetBasic=165,
		Benefits_InLast12Months_FrequencyNotMetBasic=166,
		Benefits_InLast12Months_FrequencyMetWithTwoProcsInLast12Months=167,
		Benefits_InsHistBitewingFrequency=168,
		Benefits_ProcedureCodeWaitingPerdiodOverride=169,
		//Add new items to enum here at end of list
	}
}


