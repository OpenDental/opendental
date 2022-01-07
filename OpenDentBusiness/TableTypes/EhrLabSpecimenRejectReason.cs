using EhrLaboratories;
using System;

namespace OpenDentBusiness {
	///<summary>For EHR module, the specimen upon which the lab orders were/are to be performed on.  (May Repeat) SPM.21</summary>
	[Serializable]
	public class EhrLabSpecimenRejectReason:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabSpecimenRejectReasonNum;
		///<summary>FK to ehrlab.EhrLabNum.  May be 0.</summary>
		public long EhrLabSpecimenNum;
		///<summary> SPM.21.1</summary>
		public string SpecimenRejectReasonID;
		///<summary>Description of SpecimenRejectReasonId.   SPM.21.2</summary>
		public string SpecimenRejectReasonText;
		///<summary>CodeSystem that SpecimenRejectReasonId came from.   SPM.21.3</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string SpecimenRejectReasonCodeSystemName;
		///<summary> SPM.21.4</summary>
		public string SpecimenRejectReasonIDAlt;
		///<summary>Description of SpecimenRejectReasonIdAlt.   SPM.21.5</summary>
		public string SpecimenRejectReasonTextAlt;
		///<summary>CodeSystem that SpecimenRejectReasonId came from.   SPM.21.6</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string SpecimenRejectReasonCodeSystemNameAlt;
		///<summary>Optional text that describes the original text used to encode the values above.   SPM.21.7</summary>
		public string SpecimenRejectReasonTextOriginal;


		///<summary></summary>
		public EhrLabSpecimenRejectReason Copy() {
			return (EhrLabSpecimenRejectReason)MemberwiseClone();
		}

	}

}

namespace EhrLaboratories {
	///<summary>Order Control Code.  We only use RE.  OID:2.16.840.1.113883.12.119  HL70369 code:HL70119.  Source phinvads.cdc.gov</summary>
	public enum HL70490 {
		//TODO:
	}

}