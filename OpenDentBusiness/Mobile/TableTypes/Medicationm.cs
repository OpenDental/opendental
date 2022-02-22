using System;

namespace OpenDentBusiness.Mobile {

	///<summary>A list of medications, not attached to any particular patient. Patient portal version </summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class Medicationm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long MedicationNum;
		///<summary>Name of the medication.</summary>
		public string MedName;
		///<summary>FK to medication.MedicationNum.  If this is a generic drug, then the GenericNum will be the same as the MedicationNum.</summary>
		public long GenericNum;
		///<summary>RxNorm Code identifier.  FK to an in-memory dictionary of RxCui/RxNorm mappings.</summary>
		public long RxCui;

		///<summary></summary>
		public Medicationm Copy() {
			return (Medicationm)this.MemberwiseClone();
		}

	}








}










