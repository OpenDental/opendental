using System;
using System.Collections;

namespace OpenDentBusiness {
	///<summary>Used in EHR only.  Stores an entry indicating whether the office has accepted or denied the amendment.  Amendments can be verbal or written requests to add information to the patient's record.  The provider can either scan / import the document or create a detailed description that indicates what was verbally requested or where the document can be found.</summary>
	[Serializable]
	public class EhrAmendment:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrAmendmentNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>Enum:YN Y=accepted, N=denied, U=requested.</summary>
		public YN IsAccepted;
		///<summary>Description or user-defined location of the amendment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Description;
		///<summary>Enum:AmendmentSource Patient, Provider, Organization, Other.  Required.</summary>
		public AmendmentSource Source;
		///<summary>User-defined name of the amendment source.  For example, a patient name or organization name.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string SourceName;
		///<summary>The file is stored in the A-Z folder in 'EhrAmendments' folder.  This field stores the name of the file.  The files are named automatically based on Date/time along with EhrAmendmentNum for uniqueness.  This meets the requirement of "appending" to the patient's record.</summary>
		public string FileName;
		///<summary>The raw file data encoded as base64.  Only used if there is no AtoZ folder.  This meets the requirement of "appending" to the patient's record.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string RawBase64;
		///<summary>Date and time of the amendment request.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTRequest;
		///<summary>Date and time of the amendment acceptance or denial.  If there is a date here, then the IsAccepted will be set.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTAcceptDeny;
		///<summary>Date and time of the file being appended to the amendment or a link provided.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTAppend;

		///<summary></summary>
		public EhrAmendment Clone() {
			return (EhrAmendment)this.MemberwiseClone();
		}

	}

	///<summary>Source Enumeration</summary>
	public enum AmendmentSource {
		///<summary>0</summary>
		Patient,
		///<summary>1</summary>
		Provider,
		///<summary>2</summary>
		Organization,
		///<summary>3</summary>
		Other
	}

}
