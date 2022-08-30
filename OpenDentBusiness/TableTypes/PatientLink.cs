using System;

namespace OpenDentBusiness {
	///<summary>Keeps track of patients who have been merged or cloned.</summary>
	[Serializable]
	public class PatientLink:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatientLinkNum;
		///<summary>FK to patient.PatNum. The patient that is linked from.
		///For a Merge type, this is that patient that was merged from.
		///For a Clone type, this is the original or master patient.</summary>
		public long PatNumFrom;
		///<summary>FK to patient.PatNum, unless LinkType=PaySimple. The patient that is linked to.
		///For a Merge type, this is that patient that was merged into.
		///For a Clone type, this represents the clone that was made from the PatNumFrom patient.</summary>
		public long PatNumTo;
		///<summary>Enum:PatientLinkType The type of link.</summary>
		public PatientLinkType LinkType;
		///<summary>The time the link was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeLink;

		///<summary></summary>
		public PatientLink Copy() {
			return (PatientLink)this.MemberwiseClone();
		}
	}

	///<summary>The manner in which two patients are linked together.</summary>
	public enum PatientLinkType {
		///<summary>0</summary>
		Undefined,
		///<summary>1 - The two patients have been merged into each other.</summary>
		Merge,
		///<summary>2 - A clone has been made of the From patient.  PatNumFrom is the original or master and PatNumTo is the clone.</summary>
		Clone,
		///<summary>3 - The PatFromTo column will hold the ID for PaySimple.  This should not be used in OpenDental to get a patient.</summary>
		PaySimple,
	}
}
