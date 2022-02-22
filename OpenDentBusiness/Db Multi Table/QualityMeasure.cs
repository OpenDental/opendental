using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Used by ehr.</summary>
	[Serializable]
	public class QualityMeasure {
		public QualityType Type;
		public QualityType2014 Type2014;
		public string Id;
		public string Descript;
		public int Denominator;
		public int Numerator;
		public int Exclusions;
		public int Exceptions;
		///<summary>Denominator-Exceptions-Exclusions-Numerator.  Those that do not fall into a sub-population.</summary>
		public int NotMet;
		///<summary>This represents the percentage of patients in the denominator who fall into one of the other sub-populations.  The Reporting Rate is calculated as: Rate=(Numerator+Exclusions+Exceptions)/Denominator. See \\SERVERFILES\storage\EHR\Quality Measures\QRDA\CDAR2_QRDA_CATIII_DSTU_R1_2012NOV\CDAR2_QRDAIII_DSTU_R1_2012NOV.pdf page 86.</summary>
		public decimal ReportingRate;
		///<summary>The performance rate is a ratio of patients that meet the numerator criteria divided by patients in the denominator (after accounting for exclusions and exceptions).  Rate = Numerator/(Denominator-Exclusions-Exceptions).</summary>
		public decimal PerformanceRate;
		public string DenominatorExplain;
		public string NumeratorExplain;
		public string ExclusionsExplain;
		public string ExceptionsExplain;
		public string eMeasureTitle;
		public string eMeasureVNeutralId;
		public string eMeasureVSpecificId;
		public string eMeasureVersion;
		public string eMeasureNum;
		public string eMeasureSetId;
		public string eMeasureDenomId;
		public string eMeasureNumerId;
		public string eMeasureDenexId;
		public string eMeasureDenexcepId;
		public string eMeasureIppId;
		public List<EhrCqmPatient> ListEhrPats;
		public SerializableDictionary<long,List<EhrCqmEncounter>> DictPatNumListEncounters;
		public SerializableDictionary<long,List<EhrCqmProc>> DictPatNumListProcs;
		public SerializableDictionary<long,List<EhrCqmIntervention>> DictPatNumListInterventions;
		public SerializableDictionary<long,List<EhrCqmMeasEvent>> DictPatNumListMeasureEvents;
		public SerializableDictionary<long,List<EhrCqmMedicationPat>> DictPatNumListMedPats;
		public SerializableDictionary<long,List<EhrCqmNotPerf>> DictPatNumListNotPerfs;
		public SerializableDictionary<long,List<EhrCqmProblem>> DictPatNumListProblems;
		public SerializableDictionary<long,List<EhrCqmVitalsign>> DictPatNumListVitalsigns;
		
		///<summary></summary>
		public QualityMeasure Copy() {
			QualityMeasure qualityMeasure=(QualityMeasure)this.MemberwiseClone();
			qualityMeasure.ListEhrPats=new List<EhrCqmPatient>();
			for(int i=0;i<ListEhrPats.Count;i++) {
				qualityMeasure.ListEhrPats.Add(ListEhrPats[i].Copy());
			}
			return qualityMeasure;
		}

		public QualityMeasure() {
			ListEhrPats=new List<EhrCqmPatient>();
			DictPatNumListEncounters=new SerializableDictionary<long,List<EhrCqmEncounter>>();
			DictPatNumListProcs=new SerializableDictionary<long,List<EhrCqmProc>>();
			DictPatNumListInterventions=new SerializableDictionary<long,List<EhrCqmIntervention>>();
			DictPatNumListMeasureEvents=new SerializableDictionary<long,List<EhrCqmMeasEvent>>();
			DictPatNumListMedPats=new SerializableDictionary<long,List<EhrCqmMedicationPat>>();
			DictPatNumListNotPerfs=new SerializableDictionary<long,List<EhrCqmNotPerf>>();
			DictPatNumListProblems=new SerializableDictionary<long,List<EhrCqmProblem>>();
			DictPatNumListVitalsigns=new SerializableDictionary<long,List<EhrCqmVitalsign>>();
		}
	}

	public enum QualityType {
		WeightOver65,
		WeightAdult,
		Hypertension,
		TobaccoUse,
		TobaccoCessation,
		InfluenzaAdult,
		WeightChild_1_1,
		WeightChild_1_2,
		WeightChild_1_3,
		WeightChild_2_1,
		WeightChild_2_2,
		WeightChild_2_3,
		WeightChild_3_1,
		WeightChild_3_2,
		WeightChild_3_3,
		ImmunizeChild_1,
		ImmunizeChild_2,
		ImmunizeChild_3,
		ImmunizeChild_4,
		ImmunizeChild_5,
		ImmunizeChild_6,
		ImmunizeChild_7,
		ImmunizeChild_8,
		ImmunizeChild_9,
		ImmunizeChild_10,
		ImmunizeChild_11,
		ImmunizeChild_12,
		Pneumonia,
		DiabetesBloodPressure,
		BloodPressureManage
	}

	public enum QualityType2014 {
		MedicationsEntered,
		///<summary>Considered Population 1</summary>
		WeightOver65,
		///<summary>Considered Population 2</summary>
		WeightAdult,
		CariesPrevent,
		///<summary>age 0-5.  Considered Stratification 1</summary>
		CariesPrevent_1,
		///<summary>age 6-12.  Considered Stratification 2</summary>
		CariesPrevent_2,
		///<summary>age 13-20.  Considered Stratification 3</summary>
		CariesPrevent_3,
		ChildCaries,
		Pneumonia,
		TobaccoCessation,
		Influenza,
		///<summary>patients 3-16 with height, weight, and BMI recorded.  Considered Numerator 1 All</summary>
		WeightChild_1_1,
		///<summary>patients 3-16 counseled for nutrition.  Considered Numerator 2 All</summary>
		WeightChild_1_2,
		///<summary>patients 3-16 counseled for physical activity.  Considered Numerator 3 All</summary>
		WeightChild_1_3,
		///<summary>patients 3-11 with height, weight, and BMI recorded.  Considered Numerator 1 Stratification 1</summary>
		WeightChild_2_1,
		///<summary>patients 3-11 counseled for nutrition.  Considered Numerator 2 Stratification 1</summary>
		WeightChild_2_2,
		///<summary>patients 3-11 counseled for physical activity.  Considered Numerator 3 Stratification 1</summary>
		WeightChild_2_3,
		///<summary>patients 12-16 with height, weight, and BMI recorded.  Considered Numerator 1 Stratification 2</summary>
		WeightChild_3_1,
		///<summary>patients 12-16 counseled for nutrition.  Considered Numerator 2 Stratification 2</summary>
		WeightChild_3_2,
		///<summary>patients 12-16 counseled for physical activity.  Considered Numerator 3 Stratification 2</summary>
		WeightChild_3_3,
		BloodPressureManage
	}

	public enum CqmItemAbbreviation {
		///<summary>Encounter Performed</summary>
		Enc,
		///<summary>Procedure Performed</summary>
		Proc,
		///<summary>Intervention Order/Performed</summary>
		Ivn,
		///<summary>EhrMeasureEvents, used for Current Medications Documented 'procedures' and Tobacco Assessment Events</summary>
		MeasEvn,
		///<summary>Medication Active/Order/Administered</summary>
		MedPat,
		///<summary>EhrNotPerformed</summary>
		NotPerf,
		///<summary>Problem (Diagnosis)</summary>
		Prob,
		///<summary>Vitalsign</summary>
		Vital,
		///<summary>Payer</summary>
		Payer
		
	}

	///<summary>This is all of the patient data required for QRDA category 1 reporting (in the patient recordTarget section).  If the patient is in ListEhrPats for this EhrCqmData object, the patient is part of the initial patient population.  The patient will also be placed into the 'Numerator', 'Exclusion', or 'Exception' category, for counting up results for the QRDA category 3 report for this measure.  A short explanation will be provided if the patient is not in the 'Numerator' to help the user improve their percentage.</summary>
	public class EhrCqmPatient {
		public Patient EhrCqmPat;
		public List<PatientRace> ListPatientRaces;
		public PatientRace Ethnicity;
		public string PayorSopCode;
		public string PayorDescription;
		public string PayorValueSetOID;
		public bool IsNumerator;
		public bool IsExclusion;
		public bool IsException;
		//for all but one measure denominator=initial patient population.  The influenza vaccine measure says denominator is IPP with some additional restrictions that are not exclusions or exceptions.  In this case we will set this bool to false, so patient will be in IPP but not in denominator.  This will almost always be true since the list of EhrCqmPatients for each measure is the IPP and the denominator.
		public bool IsDenominator;
		public string Explanation;

		public EhrCqmPatient Copy() {
			EhrCqmPatient ehrCqmPatient=(EhrCqmPatient)this.MemberwiseClone();
			ehrCqmPatient.ListPatientRaces=new List<PatientRace>();
			for(int i=0;i<ListPatientRaces.Count;i++) {
				ehrCqmPatient.ListPatientRaces.Add(ListPatientRaces[i].Clone());
			}
			return ehrCqmPatient;
		}

		public EhrCqmPatient() {
			ListPatientRaces=new List<PatientRace>();
		}
	}

	public class EhrCqmEncounter {
		public long EhrCqmEncounterNum;
		public long PatNum;
		public long ProvNum;
		public string CodeValue;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public DateTime DateEncounter;
		//these fields are used by measure 68, meds documented, which is encounter based, not patient based
		public bool IsNumerator;
		public bool IsException;
		public string Explanation;
	}

	public class EhrCqmIntervention {
		public long EhrCqmInterventionNum;
		public long PatNum;
		public long ProvNum;
		public string CodeValue;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public DateTime DateEntry;
	}

	public class EhrCqmProblem {
		public long EhrCqmProblemNum;
		public long PatNum;
		public string CodeValue;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public DateTime DateStart;
		public DateTime DateStop;
	}

	public class EhrCqmMedicationPat {
		public long EhrCqmMedicationPatNum;//will either be a medicationpat or a vaccinepat, the other num will be set to 0
		public long EhrCqmVaccinePatNum;
		public long PatNum;
		public long RxCui;
		public string CVXCode;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public string PatNote;//will be blank if vaccinepat object
		public VaccineCompletionStatus CompletionStatus;
		public DateTime DateStart;
		public DateTime DateStop;
	}

	public class EhrCqmNotPerf {
		public long EhrCqmNotPerfNum;
		public long PatNum;
		public string CodeValue;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public string CodeValueReason;
		public string CodeSystemNameReason;
		public string CodeSystemOIDReason;
		public string DescriptionReason;
		public string ValueSetNameReason;
		public string ValueSetOIDReason;
		public DateTime DateEntry;
	}

	public class EhrCqmMeasEvent {
		public long EhrCqmMeasEventNum;
		public EhrMeasureEventType EventType;
		public long PatNum;
		public string CodeValue;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public DateTime DateTEvent;
	}

	public class EhrCqmProc {
		public long EhrCqmProcNum;
		public long PatNum;
		public long ProvNum;
		public string ProcCode;
		public string CodeSystemName;
		public string CodeSystemOID;
		public string Description;
		public string ValueSetName;
		public string ValueSetOID;
		public DateTime ProcDate;
	}

	public class EhrCqmVitalsign {
		public long EhrCqmVitalsignNum;
		public long PatNum;
		public float Height;
		public float Weight;
		///<summary>BMI=-1 if it is a Physical Exam, Finding: Diastolic/Systolic BP</summary>
		public decimal BMI;//in kg/m2, if valid BMI value: CodeValue=39156-5, CodeSystemName=LOINC, CodeSystemOID=2.16.840.1.113883.6.1, Description=Body mass index (BMI) [Ratio], ValueSetName=BMI LOINC Value, ValueSetOID=2.16.840.1.113883.3.600.1.681
		///<summary>BpSystolic=-1 if it is a Physical Exam, Finding: BMI</summary>
		public int BpSystolic;//for BP Systolic exam: CodeValue=8480-6, CodeSystemName=LOINC, CodeSystemOID=2.16.840.1.113883.6.1, Description=Systolic blood pressure, ValueSetName=Systolic Blood Pressure, ValueSetOID=2.16.840.1.113883.3.526.3.1032
		///<summary>BpDiastolic=-1 if it is a Physical Exam, Finding: BMI</summary>
		public int BpDiastolic;//for BP Diastolic exam: CodeValue=8462-4, CodeSystemName=LOINC, CodeSystemOID=2.16.840.1.113883.6.1, Description=Diastolic blood pressure, ValueSetName=Diastolic Blood Pressure, ValueSetOID=2.16.840.1.113883.3.526.3.1033
		public string HeightExamCode;//LOINC codes only
		public string HeightExamDescript;//from LOINC table
		public string WeightExamCode;//LOINC codes only
		public string WeightExamDescript;//from LOINC table
		public string BMIExamCode;//Percentile code, LOINC codes only
		public string BMIPercentileDescript;//from LOINC table
		public int BMIPercentile;//0-99, if -1 then not a valid BMI Percentile
		public DateTime DateTaken;
	}
}
