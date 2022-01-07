using System;

namespace OpenDentBusiness.Mobile {

	///<summary>Links medications to patients. Patient portal version</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class MedicationPatm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long MedicationPatNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to medication.MedicationNum.</summary>
		public long MedicationNum;
		///<summary>Medication notes specific to this patient.</summary>
		public string PatNote;
		///<summary>Date that the medication was started.  Can be minval if unknown.</summary>
		public DateTime DateStart;
		///<summary>Date that the medication was stopped.  Can be minval if unknown.  If not minval, then this medication is "discontinued".</summary>
		public DateTime DateStop;

		///<summary></summary>
		public MedicationPatm Copy() {
			return (MedicationPatm)this.MemberwiseClone();
		}

	}








}










