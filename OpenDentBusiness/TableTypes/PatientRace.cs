using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Each patient may have multiple races.  Used to represent a race or an ethnicity for a patient.</summary>
	[Serializable]
	public class PatientRace:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatientRaceNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Enum:PatRace Deprecated. CdcrecCode should be used exclusively.</summary>
		public PatRace Race=PatRace.NotSet;
		///<summary>FK to cdcrec.CdcrecCode. The value 'Declined to Specify' is stored as ASKU-ETHNICITY for ethnicity and ASKU-RACE as race.</summary>
		public string CdcrecCode;
		///<summary>This value is the value in the cdcrec.Description for the corresponding CdcrecCode.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string Description;
		///<summary>This value is true if the cdcrec.HierarchicalCode for the corresponding CdcrecCode starts with an 'E', false otherwise.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsEthnicity;
		///<summary>This is the value of the cdcrec.HierarchicalCode for the corresponding CdcrecCode.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string HeirarchicalCode="";

		///<summary></summary>
		public PatientRace() { }

		///<summary></summary>
		public PatientRace(long patNum,string cdcrecCode) {
			PatNum=patNum;
			CdcrecCode=cdcrecCode;
		}

		///<summary>The value that is stored for Declined to Specify Race.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public const string DECLINE_SPECIFY_RACE_CODE="ASKU-RACE";
		///<summary>The value that is stored for Declined to Specify Ethnicity.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public const string DECLINE_SPECIFY_ETHNICITY_CODE="ASKU-ETHNICITY";
		///<summary>The value that is stored for MultiRacial. This is a hidden option that will only be visible if the user had selected it in
		///the past.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public const string MULTI_RACE_CODE="MULTI-RACE";

		///<summary></summary>
		public PatientRace Clone() {
			return (PatientRace)this.MemberwiseClone();
		}

	}

	///<summary>Deprecated. Use CDCREC codes instead.</summary>
	public enum PatRace {
		///<summary>-1 - The value for all PatientRace entries after the Race column was deprecated.</summary>
		NotSet=-1,
		///<summary>0 - Hidden for EHR.</summary>
		Aboriginal=0,
		///<summary>1 - CDCREC:2054-5 Race</summary>
		AfricanAmerican,
		///<summary>2 - CDCREC:1002-5 Race</summary>
		AmericanIndian,
		///<summary>3 - CDCREC:2028-9 Race</summary>
		Asian,
		///<summary>4 - Our hard-coded option for EHR reporting.</summary>
		DeclinedToSpecifyRace,
		///<summary>5 - CDCREC:2076-8 Race</summary>
		HawaiiOrPacIsland,
		///<summary>6 - CDCREC:2135-2 Ethnicicty.  If EHR is turned on, our UI will force this to be supplemental to a base 'race'.</summary>
		Hispanic,//should be renamed to HispanicOrLatino
		///<summary>7 - We had to keep this for backward compatibility.  Hidden for EHR because it's explicitly not allowed.</summary>
		Multiracial,
		///<summary>8 - CDCREC:2131-1 Race.</summary>
		Other,
		///<summary>9 - CDCREC:2106-3 Race</summary>
		White,
		///<summary>10 - CDCREC:2186-5 Ethnicity.  We originally used the lack of Hispanic to indicate NonHispanic.  Now we are going to explicitly store NonHispanic to make queries for ClinicalQualityMeasures easier.</summary>
		NotHispanic,
		///<summary>11 - Our hard-coded option for EHR reporting.</summary>
		DeclinedToSpecifyEthnicity
	}
}