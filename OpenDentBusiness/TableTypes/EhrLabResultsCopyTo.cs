using EhrLaboratories;
using System;

namespace OpenDentBusiness {
	///<summary>For EHR module, copy results to... that contains all required fields for HL7 Lab Reporting Interface (LRI).</summary>
	[Serializable]
	public class EhrLabResultsCopyTo:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabResultsCopyToNum;
		///<summary>FK to ehrlab.EhrLabNum.</summary>
		public long EhrLabNum;
		#region CopyTo OBR.28
		///<summary>May be provnum or NPI num or any other num, when combined with CopyToIdAssigningAuthority should uniquely identify the provider.  OBR.28.1</summary>
		public string CopyToID;
		///<summary>OBR.28.2</summary>
		public string CopyToLName;
		///<summary>OBR.28.3</summary>
		public string CopyToFName;
		///<summary>Middle names or initials therof.  OBR.28.4</summary>
		public string CopyToMiddleNames;
		///<summary>Example: JR or III.  OBR.28.5</summary>
		public string CopyToSuffix;
		///<summary>Example: DR, Not MD, MD would be stored in an optional field that was not implemented called CopyToDegree.  OBR.28.6</summary>
		public string CopyToPrefix;
		#region Ordering Provider Id Assigning Authority OBR.28.9
		///<summary>Usually empty, "The value of [this field] reflects a local code that represents the combination of [the next two fields]."  OBR.28.9.1</summary>
		public string CopyToAssigningAuthorityNamespaceID;
		///<summary>ISO compliant OID that represents the organization that assigned the unique provider ID.  OBR.28.9.2</summary>
		public string CopyToAssigningAuthorityUniversalID;
		///<summary>Always "ISO", unless importing from outside source.  OBR.28.9.3</summary>
		public string CopyToAssigningAuthorityIDType;
		#endregion
		///<summary>Describes the type of name used.  OBR.28.10</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70200 CopyToNameTypeCode;
		///<summary>Must be value from HL70203 code set, see note at bottom of EhrLab.cs for usage.  OBR.28.13</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public HL70203 CopyToIdentifierTypeCode;
		#endregion

		///<summary></summary>
		public EhrLabResultsCopyTo Copy() {
			return (EhrLabResultsCopyTo)MemberwiseClone();
		}

	}

}