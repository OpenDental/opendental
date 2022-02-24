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
	///<summary>Only unit tests that are getting publicly documented receive a number.  This number never gets changed, and the behavior of a numbered and documented unit test never gets changed.  If behavior needs to change, deprecate the old one and create a new one.  The numbering does not need to be strictly sequential, nor does it need to be based on when the unit test was written.  Think of it as a primary key which has no meaning other than for reference purposes.</summary>
	public class Numbering:System.Attribute{
		public EnumTestNum TestNum;

		public Numbering(EnumTestNum testNum){
			TestNum=testNum;
		}
	}

	///<summary>Never change the number assigned to an existing enum item.</summary>
	public enum EnumTestNum{
		///<summary>Currently documented separately as tests 1 and 2.  Need to fix documentation.</summary>
		Procedures_ComputeEstimates_Allowed1Allowed2=1,
		Procedures_ComputeEstimates_ZeroCoverageOverAnnualMax=3,
		Procedures_ComputeEstimates_FamilyUnderAnnualMax=4,
		Procedures_ComputeEstimates_IndividualAndFamilyMax=5
	}
}

namespace UnitTests.Legacy_Tests {
	[TestClass]
	public class LegacyTests:TestBase {

		//Legacy_TestOneTwo => Procedures_ComputeEstimates_Allowed1Allowed2

		//Legacy_TestThree => Procedures_ComputeEstimates_ZeroCoverageOverAnnualMax

		//Legacy_TestFour => Procedures_ComputeEstimates_FamilyUnderAnnualMax

		//Legacy_TestFive => Procedures_ComputeEstimates_IndividualAndFamilyMax

		//Legacy_TestSix => InsPlan_GetInsUsedDisplay_LimitationsOverride

		//Legacy_TestSeven => Procedures_ComputeEstimates_PreventitiveDiagnosticDeductibleOnlyOnce

		//Legacy_TestEight => Claims_CalculateAndUpdate_Allowed1Allowed2CompletedProcedures

		//Legacy_TestNine => Procedures_ComputeEstimates_LimitationsOverrideGeneralLimitations

		//Legacy_TestTen => Procedures_ComputeEstimates_AnnualMaxReachedZeroCoverage

		//Legacy_TestEleven => Procedures_ComputeEstimates_FamilyMaxNoIndividualMax

		//Legacy_TestTwelve => Procedures_ComputeEstimates_WriteoffPPOsPriSecSamePlan

		//Legacy_TestThirteen => InsPlan_GetInsUsedDisplay_OrthoProcsNotAffectInsUsed

		//Legacy_TestFourteen => Procedures_ComputeEstimates_PriEstNotAffectedBySecClaim

		//Legacy_TestFifteen => Procedures_ComputeEstimates_DeductibleOverrides

		//Legacy_TestSixteen => Procedures_ComputeEstimates_CategoryDeductiblesShouldNotExceedRegularDeductible

		//Legacy_TestSeventeen => Procedures_ComputeEstimates_COBStandardTwoPPOs

		//Legacy_TestEighteen => Procedures_ComputeEstimates_COBCarveOutCategoryPercentagePlan

		//Legacy_TestNineteen => Procedures_ComputeEstimates_MultipleDeductibles

		//Legacy_TestTwenty => InsPlan_GetDedRemainDisplay_IndividualAndFamilyDeductiblesInsRemaining

		//Legacy_TestTwentyOne => Procedures_ComputeEstimates_DeductiblesForProcsNotCovered

		//Legacy_TestTwentyTwo => DEPRECATED

		//Legacy_TestTwentyThree => DEPRECATED

		//Legacy_TestTwentyFour - Legacy_TestTwentySeven => Moved to TimeCardRulesTests

		//Legacy_TestTwentyEight => ClaimProcs_FormClaimProc_TextBoxValuesFromChartModule

		//Legacy_TestTwentyNine => ClaimProcs_FormClaimProc_TextBoxValuesFromClaimEditWindow

		//Legacy_TestThirty => ClaimProcs_FormClaimProc_TextBoxValuesFromTreatPlanModule

		//Legacy_TestThirtyOne => InsPlan_GetPendingDisplay_LimitationsOverrideGeneralLimitations

		//Legacy_TestThirtyTwo - Legacy_TestThirtyThree => ?Unknown?

		//Legacy_TestThirtyFour => Procedures_ComputeEstimates_GeneralDeductiblesConsideredWithProcedureSpecificDeductibles

		//Legacy_TestThirtyFive => Benefits_GetDeductibleByCode_InsuranceAdjustmentDeductible

		//Legacy_TestThirtySix => Procedures_ComputeEstimates_COBStandardDualPPOWriteoffZero

		//Legacy_TestThirtySeven => Procedures_ComputeEstimates_PPOProceduresMultipleUnits

		//Legacy_TestThirtyEight => Procedures_ComputeEstimates_CategoryPercentageProceduresMultipleUnits

		//Legacy_TestThirtyNine => Procedures_ComputeEstimates_PPOProceduresMultipleUnitsWriteoff

		//Legacy_TestFourty => Procedures_ComputeEstimates_InsPPOsecWriteoffsPreference

		//Tests 41-48 moved to PaymentsTests

		//Legacy_TestFourtyNine => Procedures_ComputeEstimates_OverrideUnderFamilyMaxEstimateNote

		//Legacy_TestFifty => Procedures_ComputeEstimates_OverrideOverFamilyMaxEstimateNote

		//Legacy_TestFiftyOne => Procedures_ComputeEstimates_MedicalDeductible

		//Legacy_TestFiftyTwo => SignatureBoxWrapper_FillSignature_GetNumberOfTabletPoints

		//Legacy_TestFiftySeven => Fees_GetFee_ProviderSpecificFees

		//Legacy_TestFiftyEight => Fees_GetFee_ClinicSpecificFees

		//Legacy_TestFiftyNine => Fees_GetFee_ClinicAndProviderSpecificFees

		//Legacy_TestSixty => Claims_CalculateAndUpdate_ProcedureCodeDowngradeBlankFee

		//Legacy_TestSixtyOne => Claims_CalculateAndUpdate_ProcedureCodeDowngradeHigherFee

		//Legacy_TestSixtyFive => Procedures_ComputeEstimates_MedicalFlatCopaySecondaryFeeSchedule

		//Legacy_TestSeventyThree => Procedures_ComputeEstimates_CategoryPercentageCanadianLabFees

		//Legacy_TestSeventyFour => Benefits_GetAnnualMaxDisplay_CalendarYearBenefit

		//Legacy_TestSeventyFive => Procedures_ComputeEstimates_OrthoMax

		//Legacy_TestSeventySix => Procedures_ComputeEstimates_OrthoFamilyMaxOverLargerIndividual

		//Legacy_TestSeventyNine => Appointments_GetApptTimePatternFromProcPatterns_PatternLogic

		//Legacy_TestEighty => Procedures_ComputeEstimates_DeductiblesDualInsuranceNotNegative

		//Legacy_TestEightyOne => Procedures_ComputeEstimates_FrequencyLimitationMet

		//Legacy_TestEightyThree => Procedures_ComputeEstimates_MultipleProceduresOneClaimExceedAnnualMax

		//Legacy_TestEightyFour => Procedures_ComputeEstimates_COBCarveOutSecondaryInsurance

		//Legacy_TestEightyFive => RecurringCharges_FormCreditRecurringCharges_PaymentPlansV1V2

	}
}
