using System;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable]
	public class OIDInternal:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OIDInternalNum;
		///<summary>Enum:IdentifierType Internal data type to be associated with OIDRoot</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public IdentifierType IDType;
		///<summary>This is the root OID for this data type, when combined with extension, uniquely identifies a single object.</summary>
		public string IDRoot;


		///<summary></summary>
		public OIDInternal Copy() {
			return (OIDInternal)MemberwiseClone();
		}

	}

	///<summary>Stored as string. Sorted and displayed in the order they are present in this enum.  Root should always be first.</summary>
	public enum IdentifierType {
		///<summary>Will most likely be the root of all other OIDs.  Represents the organization.</summary>
		Root,
		///<summary>FK to ehrlab.EhrLabNum.  root+".1"</summary>
		LabOrder,
		///<summary>FK to patient.PatNum.  root+".2"</summary>
		Patient,
		///<summary>FK to provider.ProvNum.  root+".3"</summary>
		Provider,
		///<summary>This will be the root for all CQM reported items, like encounters, procedures, problems, etc.  root+".4"  The extension will be abbreviated name concatenated with the primary key of the object.  Examples: pat5231 or medpat197432 or proc231782 or notperf38291.  This is only used for generating QRDA documents and requires that the encounter, procedure, etc. is uniquely identified in the reports.  The root+".4" makes it unique to this office, the abbreviated name plus primary key makes it unique within the office.</summary>
		CqmItem,
		///<summary>FK to disease.DiseaseNum.  root+".5"</summary>
		Problem,
		///<summary>FK to appointment.AptNum.  root+".6"</summary>
		Appointment,
		///<summary>FK to insplan.PlanNum.  root+".7"</summary>
		InsPlan,
		///<summary>FK to procedurelog.ProcNum.  root+".8"</summary>
		Procedure
	}
}
