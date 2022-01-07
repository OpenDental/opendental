using System;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable]
	public class OIDExternal:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OIDExternalNum;
		///<summary>Enum:IdentifierType Internal data type to be associated with.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public IdentifierType IDType;
		///<summary>This should be a Primary Key to a Table Type defined by the IDType field. Example: If IDType==Patient, then this field should be a PatNum that is a FK to Patient.Patnum</summary>
		public long IDInternal;
		///<summary>The OID extension, when combined with rootExternal it uniquely identifies an object.</summary>
		public string IDExternal;
		///<summary>The OID root, when combined with IDExternal it uniquely identifies an object.</summary>
		public string rootExternal;


		///<summary></summary>
		public OIDExternal Copy() {
			return (OIDExternal)MemberwiseClone();
		}

	}
}
