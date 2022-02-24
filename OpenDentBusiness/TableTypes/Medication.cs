using System;

namespace OpenDentBusiness{

	///<summary>A list of medications, not attached to any particular patient.  Not allowed to delete if in use by a patient.  Not allowed to edit name once created due to possibility of damage to patient record.</summary>
	[Serializable]
	public class Medication:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MedicationNum;
		///<summary>Name of the medication.  User can change this.  If an RxCui is present, the RxNorm string can be pulled from the in-memory table for UI display in addition to the MedName.</summary>
		public string MedName;
		///<summary>FK to medication.MedicationNum.  Cannot be zero.
		///If this is a generic drug, then the GenericNum will be the same as the MedicationNum.
		///Otherwise, if this is a brand drug, then the GenericNum will be a non-zero value corresponding to another medicaiton.</summary>
		public long GenericNum;
		///<summary>Notes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Notes;
		///<summary>The last date and time this row was altered.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>RxNorm Code identifier.  We should have used a string type.  Used by EHR in CQM.  But the queries should use medicationpat.RxCui, NOT this RxCui, because all medicationpats (meds and orders) coming back from NewCrop will not have a FK to this medication table.  When this RxCui is modified by the user, then medicationpat.RxCui is automatically updated where medicationpat.MedicationNum matches this medication.</summary>
		public long RxCui;

		///<summary>Returns a copy of this Medication.</summary>
		public Medication Copy() {
			return (Medication)this.MemberwiseClone();
		}

	}
	
	

	




}










