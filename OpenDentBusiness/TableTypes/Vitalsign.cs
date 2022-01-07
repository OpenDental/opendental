using System;

namespace OpenDentBusiness {
	///<summary>For EHR module, one dated vital sign entry.  BMI is calulated on demand based on height and weight and may be one of 4 ALOINC codes. 39156-5 "Body mass index (BMI) [Ratio]" is most applicable.</summary>
	[Serializable]
	public class Vitalsign:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long VitalsignNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Height of patient in inches. Fractions might be needed some day.  Allowed to be 0.  Six possible LOINC codes, most applicable is 8302-2, "Body height".</summary>
		public float Height;
		///<summary>Lbs.  Allowed to be 0. Six possible LOINC codes, most applicable is 29463-7, "Body weight".</summary>
		public float Weight;
		///<summary>Units are mmHg (millimeters of mercury). Allowed to be 0. LOINC code 8480-6.</summary>
		public int BpSystolic;
		///<summary>Units are mmHg (millimeters of mercury). Allowed to be 0. LOINC code 8462-4.</summary>
		public int BpDiastolic;
		///<summary>The date that the vitalsigns were taken.</summary>
		public DateTime DateTaken;
		///<summary>For an abnormal BMI measurement this must be true in order to meet quality measurement.</summary>//intervention? I think these should be deprecated and use an Intervention object instead.
		public bool HasFollowupPlan;
		///<summary>If a BMI was not recorded, this must be true in order to meet quality measurement.  For children, this is used as an IsPregnant flag, the only valid reason for not taking BMI on children.</summary>//intervention? I think these should be deprecated and use an Intervention object instead.
		public bool IsIneligible;
		///<summary>For HasFollowupPlan or IsIneligible, this documents the specifics.</summary>//intervention? I think these should be deprecated and use an Intervention object instead.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Documentation;
		///<summary>.</summary>//intervention? I think these should be deprecated and use an Intervention object instead.
		public bool ChildGotNutrition;
		///<summary>.</summary>//intervention? I think these should be deprecated and use an Intervention object instead.
		public bool ChildGotPhysCouns;
		///<summary>Used for CQMs.  SNOMED CT code either Normal="", Overweight="238131007", or Underweight="248342006".  Set when BMI is found to be "out of range", based on age groups.  Should be calculated when vital sign is saved.  Calculate based on age as of Jan 1 of the year vitals were taken.  Not currently displayed to user.</summary>
		public string WeightCode;
		///<summary>FK to ehrcode.CodeValue.  Also FK to LOINC.LoincCode.  Used for CQMs.  LOINC code used to describe the height exam performed.  Examples: Body Height Measured=3137-7, Body Height Stated=3138-5, Body Height --pre surgery=8307-1.  We will default to Body Height=8302-2, but user can choose another from the list of 6 allowed.  Can be blank if BP only.</summary>
		public string HeightExamCode;
		///<summary>FK to ehrcode.CodeValue.  Also FK to LOINC.LoincCode.  Used for CQMs.  LOINC code used to describe the weight exam performed.  Examples: Body Weight Measured=3141-9, Body Weight Stated=3142-7, Body Weight --with clothes=8350-1.  We will default to Body Weight=29463-7, but user can choose another from the list of 6 allowed.  Can be blank if BP only.</summary>
		public string WeightExamCode;
		///<summary>FK to ehrcode.CodeValue.  Also FK to LOINC.LoincCode.  Used for CQMs.  LOINC code used to describe the BMI percentile calculated.  We will use LOINC 59576-9 - BMI Percentile Per age and gender.  Can be blank if BP only.</summary>
		public string BMIExamCode;
		///<summary>FK to ehrnotperformed.EhrNotPerformedNum.  This will link a vitalsign to the EhrNotPerformed object where the reason not performed will be stored.  The linking will allow us to display the not performed reason directly in the vital sign window and will make CQM queries easier.  Will be 0 if not linked to an EhrNotPerformed object.</summary>
		public long EhrNotPerformedNum;
		///<summary>FK to disease.DiseaseNum.  This will link this vitalsign object to a pregnancy diagnosis for this patient.  It will be 0 for non pregnant patients.  The disease it is linked to will be inserted automatically based on the default value set.  In order to change this code for this specific exam it will have to be changed in the problems list.</summary>
		public long PregDiseaseNum;
		///<summary>BMI percentile of patient, based on gender and age and the calculated BMI.  We will use the CDC numbers to calculate percentile found here: (http://www.cdc.gov/nchs/data/series/sr_11/sr11_246.pdf).</summary>
		public int BMIPercentile;
		///<summary>Recorded pulse of the patient. Stored in beats per minute.</summary>
		public int Pulse;

		///<summary></summary>
		public Vitalsign Copy() {
			return (Vitalsign)MemberwiseClone();
		}

	}
}
