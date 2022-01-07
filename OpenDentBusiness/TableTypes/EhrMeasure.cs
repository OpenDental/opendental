using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>For EHR module, automate measure calculation.</summary>
	[Serializable]
	public class EhrMeasure:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrMeasureNum;
		///<summary>Enum:EhrMeasureType</summary>
		public EhrMeasureType MeasureType;
		///<summary>0-100, -1 indicates not entered yet.</summary>
		public int Numerator;
		///<summary>0-100, -1 indicates not entered yet.</summary>
		public int Denominator;
		///<summary>Not a database column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string Objective;
		///<summary>Not a database column.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string Measure;
		///<summary>Not a database column.  More than this percent for meaningful use.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public int PercentThreshold;
		///<summary>Not a database column.  An explanation of which patients qualify for enumerator.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string NumeratorExplain;
		///<summary>Not a database column.  An explanation of which patients qualify for denominator.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string DenominatorExplain;
		///<summary>Not a database column.  Used for timing calculation of each measure.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public TimeSpan ElapsedTime;
		///<summary>Not a database column.  An explanation of the conditions which would allow a Provider to be excluded from this requirement.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ExclusionExplain;
		///<summary>Not a database column.  Some exclusions have an associated count that the Provider must report if they are to attest to exclusion from this requirement.  Can be 0.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public int ExclusionCount;
		///<summary>Not a database column.  A description of what the count is.  Example: If a Provider writes fewer than 100 Rx's during the reporting period, they can be excluded from reporting the ProvOrderEntry - CPOE measure.  The count would be the number of Rx's entered by the Provider during the reporting period and the label would identify the number as such.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ExclusionCountDescript;

		///<summary></summary>
		public EhrMeasure Copy() {
			return (EhrMeasure)MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum EhrMeasureType {
		///<summary>0</summary>
		ProblemList,
		///<summary>1</summary>
		MedicationList,
		///<summary>2</summary>
		AllergyList,
		///<summary>3</summary>
		Demographics,
		///<summary>4</summary>
		Education,
		///<summary>5</summary>
		TimelyAccess,
		///<summary>6</summary>
		ProvOrderEntry,
		///<summary>7</summary>
		CPOE_MedOrdersOnly,
		///<summary>8</summary>
		CPOE_PreviouslyOrdered,
		///<summary>9</summary>
		Rx,
		///<summary>10</summary>
		VitalSigns,
		///<summary>11</summary>
		VitalSignsBMIOnly,
		///<summary>12</summary>
		VitalSignsBPOnly,
		///<summary>13</summary>
		Smoking,
		///<summary>14</summary>
		Lab,
		///<summary>15</summary>
		ElectronicCopy,
		///<summary>16</summary>
		ClinicalSummaries,
		///<summary>17</summary>
		Reminders,
		///<summary>18</summary>
		MedReconcile,
		///<summary>19- Summary of care record for transition or referral.</summary>
		SummaryOfCare,
		///<summary>20</summary>
		CPOE_LabOrdersOnly,
		///<summary>21</summary>
		CPOE_RadiologyOrdersOnly,
		///<summary>22</summary>
		ElectronicCopyAccess,
		///<summary>23</summary>
		SummaryOfCareElectronic,
		///<summary>24</summary>
		SecureMessaging,
		///<summary>25</summary>
		FamilyHistory,
		///<summary>26</summary>
		ElectronicNote,
		///<summary>27</summary>
		LabImages,
		///<summary>28</summary>
		VitalSigns2014,
		///<summary>29</summary>
		DrugDrugInteractChecking,
		///<summary>30</summary>
		DrugFormularyChecking,
		///<summary>31</summary>
		ProtectElectHealthInfo,
		///<summary>32</summary>
		ImmunizationRegistries,
		///<summary>33</summary>
		SyndromicSurveillance,
		///<summary>34</summary>
		PatientList,
		///<summary>35</summary>
		ClinicalInterventionRules
	}

	///<summary>Helps track whether the current patient has met the measurement objectives.</summary>
	public class EhrMu {
		///<summary></summary>
		public EhrMeasureType MeasureType;
		///<summary></summary>
		public MuMet Met;
		///<summary></summary>
		public string Details;
		///<summary></summary>
		public string Action;
		///<summary></summary>
		public string Action2;
	}

	public enum MuMet {
		False,
		True,
		NA
	}

}
