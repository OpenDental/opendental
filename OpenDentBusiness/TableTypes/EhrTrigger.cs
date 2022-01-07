using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>CDS Triggers when referenced in UI. Used for CDS automation.  May later be expanded to replace "automation."
	///</summary>
	[Serializable]
	public class EhrTrigger:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrTriggerNum;
		///<summary>Short description to describe the trigger.</summary>
		public string Description;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProblemSnomedList;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProblemIcd9List;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProblemIcd10List;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ProblemDefNumList;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MedicationNumList;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string RxCuiList;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string CvxList;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string AllergyDefNumList;
		///<summary>Age, Gender.  Can be multiple age entries but only one gender entry as coma delimited values.  Example: " age,>18  age,&lt;=55  gender,male"</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string DemographicsList;
		/////<summary>Tab delimited list, sub-components separated by semicolon. Loinc;Value;Units\t Example: Cholesterol [Mass/volume] in Serum or Plasma>150mg/dL=="2093-3;>150;mg/dL"</summary>
		///<summary>List of loinc codes padded with spaces.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string LabLoincList;
		///<summary>Examples:  Height,>=72  Weight&lt;,100  BMI=  (BP currently not implemented.)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string VitalLoincList;
		///<summary>The reccomended course of action for this intervention.  </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Instructions;
		///<summary>Bibliographic information, not a URL. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Bibliography;
		///<summary>Enum:MatchCardinality Requires One, OneOfEachCategory, TwoOrMore, or All for trigger to match.  </summary>
		public MatchCardinality Cardinality;


		///<summary></summary>
		public EhrTrigger Copy() {
			return (EhrTrigger)this.MemberwiseClone();
		}

		public EhrTrigger() {
			Description="";
			ProblemSnomedList="";
			ProblemIcd9List="";
			ProblemIcd10List="";
			ProblemDefNumList="";
			MedicationNumList="";
			RxCuiList="";
			CvxList="";
			AllergyDefNumList="";
			DemographicsList="";
			LabLoincList="";
			VitalLoincList="";
			Cardinality=MatchCardinality.One;
		}

		///<summary>Used for displaying what elements of the trigger are set. Example: Medication, Demographics</summary>
		public string GetTriggerCategories() {
			string retVal="";
			if(ProblemSnomedList.Trim()!=""
				|| ProblemIcd9List.Trim()!=""
				|| ProblemIcd10List.Trim()!=""
				|| ProblemDefNumList.Trim()!="") 
			{
				retVal+="Problem";
			}
			if(MedicationNumList.Trim()!=""
				|| CvxList.Trim()!=""
				|| RxCuiList.Trim()!="") {
				retVal+=(retVal==""?"":", ")+"Medication";
			}
			if(AllergyDefNumList.Trim()!=""){
				retVal+=(retVal==""?"":", ")+"Allergy";
			}
			if(DemographicsList.Trim()!="") {
				retVal+=(retVal==""?"":", ")+"Demographic";
			}
			if(LabLoincList.Trim()!="") {
				retVal+=(retVal==""?"":", ")+"Lab Result";
			}
			if(VitalLoincList.Trim()!="") {
				retVal+=(retVal==""?"":", ")+"Vitals";
			}
			return retVal;
		}

	}

	///<summary>Not a DB table. Used to pass intervention information to FormCDSI based on matched trigger. </summary>
	public class CDSIntervention {
		///<summary>The EHRtrigger that this CDSIntervention is generated from. </summary>
		public EhrTrigger EhrTrigger;
		///<summary>The message generated for the user based on the specific objects that triggered the intervention. </summary>
		public string InterventionMessage;
		///<summary>The list of objects that will be passed to FormInfobutton. </summary>
		public List<KnowledgeRequest> TriggerObjects;
	}

	///<summary>Not a DB table. Used to pass trigger object information for a CDSIntervention.</summary>
	public class KnowledgeRequest {
		///<summary>The EHRtrigger that this CDSIntervention is generated from.</summary>
		public string Type;
		///<summary>The message generated for the user based on the specific objects that triggered the intervention.</summary>
		public string Code;
		///<summary>The list of objects that will be passed to FormInfobutton.</summary>
		public CodeSyst CodeSystem;
		///<summary>The description of the knowledge request.</summary>
		public string Description;

		///<summary>Converts the CodeSystem enum value into a string for display purposes.</summary>
		public string GetCodeSystemDisplay() {
			switch(CodeSystem) {
				case CodeSyst.None:
					return "None";
				case CodeSyst.Snomed:
					return "SNOMED CT";
				case CodeSyst.Icd9:
					return "ICD9 CM";
				case CodeSyst.Icd10:
					return "ICD10 CM";
				case CodeSyst.Loinc:
					return "LOINC";
				case CodeSyst.RxNorm:
					return "RxNorm";
				case CodeSyst.ProblemDef:
					return "Problem Def";
				case CodeSyst.AllergyDef:
					return "Allergy Def";
				default:
					return CodeSystem.ToString();
			}
		}
	}

	///<summary>The different types of code systems that a knowledge request can be for.</summary>
	public enum CodeSyst {
		None,
		Snomed,
		Icd9,
		Icd10,
		Loinc,
		RxNorm,
		ProblemDef,
		AllergyDef
	}

	/// <summary></summary>
	public enum MatchCardinality {
		///<summary>0 - If any one of the conditions are met from any of the categories.</summary>
		One,
		///<summary>1 - Must have one match from each of the categories with set values. Categories are :Medication, Allergy, Problem, Vitals, Age, Gender, and Lab Results.</summary>
		OneOfEachCategory,
		///<summary>2 - Must match any two conditions, may be from same category.</summary>
		TwoOrMore,
		///<summary>3 - Must match every code defined in the EhrTrigger.</summary>
		All
	}
}