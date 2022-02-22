using System;

namespace OpenDentBusiness {
	///<summary>For EHR module, these are all the codes from various code sets that will affect reporting clinical quality measures.  Users cannot edit.  This is not an actual table in the database.  The codes are loaded from the EHR.dll, so it is a static object, no inserts/updates.  Selecting from this 'table' will always use the cache pattern.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class EhrCode:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrCodeNum;
		///<summary>Clinical quality measure test ID's that utilize this code.  Comma delimited list.  Example: 69v2,147v2.</summary>
		public string MeasureIds;
		///<summary>The National Library of Medicine Value Set Authority Center assigned value set name.  Example: Influenza Vaccination.</summary>
		public string ValueSetName;
		///<summary>The value set object identifier for reporting CQM.  Example: 2.16.840.1.113883.3.526.3.402.</summary>
		public string ValueSetOID;
		///<summary>The Quality Data Model category for this code.  2 examples: Condition/Diagnosis/Problem or Encounter.</summary>
		public string QDMCategory;
		///<summary>The code from the specified code system.  Example: 653.83.  This code can belong to multiple value sets, in which case this table will contain multiple rows for this code.</summary>
		public string CodeValue;
		///<summary>The description for this code.  This will frequently be duplicate data, but keeping it here ensures accurate CQM reporting.</summary>
		public string Description;
		///<summary>The code system name for this code.  Possible values are: CDCREC, CDT, CPT, CVX, HCPCS, ICD9CM, ICD10CM, LOINC, RXNORM, SNOMEDCT, SOP, and AdministrativeSex.</summary>
		public string CodeSystem;
		///<summary>The code system object identifier for reporting CQM.  Example: 2.16.840.1.113883.6.103.</summary>
		public string CodeSystemOID;
		///<summary>This is true if the code is in the corresponding table identified by CodeSystem.</summary>
		public bool IsInDb;


		///<summary></summary>
		public EhrCode Copy() {
			return (EhrCode)MemberwiseClone();
		}

	}

}
