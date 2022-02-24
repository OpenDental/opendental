using EhrLaboratories;
using System;
using System.Collections.Generic;

namespace OpenDentBusiness {
	///<summary>For EHR module, the specimen upon which the lab orders were/are to be performed on.  NTE.*</summary>
	[Serializable]
	public class EhrLabSpecimen:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabSpecimenNum;
		///<summary>FK to ehrlab.EhrLabNum.  May be 0.</summary>
		public long EhrLabNum;
		///<summary>Enumerates the SPM segments within a single message starting with 1.  SPM.1</summary>
		public long SetIdSPM;
		///<summary>SPM.2</summary>
		public string SpecimenTypeID;
		///<summary>Description of SpecimenTypeId.  SPM.3</summary>
		public string SpecimenTypeText;
		///<summary>CodeSystem that SpecimenTypeId came from.  SPM.4</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string SpecimenTypeCodeSystemName;
		///<summary>SPM.5</summary>
		public string SpecimenTypeIDAlt;
		///<summary>Description of SpecimenTypeIdAlt.  SPM.6</summary>
		public string SpecimenTypeTextAlt;
		///<summary>CodeSystem that SpecimenTypeId came from.  SPM.7</summary>
		//[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public string SpecimenTypeCodeSystemNameAlt;
		///<summary>Optional text that describes the original text used to encode the values above.  SPM.8</summary>
		public string SpecimenTypeTextOriginal;
		///<summary>Stored as string in the format YYYYMMDD[HH[MM[SS]]] where bracketed values are optional.  When time is not known will be valued "0000".  SPM.17.1.1</summary>
		public string CollectionDateTimeStart;
		///<summary>May be empty.  Stored as string in the format YYYYMMDD[HH[MM[SS]]] where bracketed values are optional.  SPM.17.2.1</summary>
		public string CollectionDateTimeEnd;
		///<summary>[0..*]This is not a data column but is stored in a seperate table named EhrLabSpecimenRejectReason.  SPM.21</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabSpecimenRejectReason> _listEhrLabSpecimenRejectReason;
		///<summary>[0..*]This is not a data column but is stored in a seperate table named EhrLabSpecimenCondition.  SPM.24</summary>
		[CrudColumn(IsNotDbColumn=true)]
		private List<EhrLabSpecimenCondition> _listEhrLabSpecimenCondition;


		///<summary></summary>
		public EhrLabSpecimen Copy() {
			return (EhrLabSpecimen)MemberwiseClone();
		}

		public List<EhrLabSpecimenRejectReason> ListEhrLabSpecimenRejectReason {
			get {
				if(_listEhrLabSpecimenRejectReason==null) {
					if(EhrLabNum==0) {
						_listEhrLabSpecimenRejectReason=new List<EhrLabSpecimenRejectReason>();
					}
					else {
						_listEhrLabSpecimenRejectReason=EhrLabSpecimenRejectReasons.GetForEhrLabSpecimen(EhrLabSpecimenNum);
					}
				}
				return _listEhrLabSpecimenRejectReason;
			}
			set {
				_listEhrLabSpecimenRejectReason=value;
			}
		}


		///<summary>Only filled with EhrLabNotes when value is used.  To refresh ListEhrLabResults, set it equal to null or explicitly reassign it using EhrLabResults.GetForLab(EhrLabNum).</summary>
		public List<EhrLabSpecimenCondition> ListEhrLabSpecimenCondition {
			get {
				if(_listEhrLabSpecimenCondition==null) {
					if(EhrLabNum==0) {
						_listEhrLabSpecimenCondition=new List<EhrLabSpecimenCondition>();
					}
					else {
						_listEhrLabSpecimenCondition=EhrLabSpecimenConditions.GetForEhrLabSpecimen(EhrLabSpecimenNum);
					}
				}
				return _listEhrLabSpecimenCondition;
			}
			set {
				_listEhrLabSpecimenCondition=value;
			}
		}

	}

}