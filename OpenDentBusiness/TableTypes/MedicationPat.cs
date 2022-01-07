using System;

namespace OpenDentBusiness{

	///<summary>Links medications to patients.  For ehr, some of these can be considered 'medication orders', but only if they contain a PatNote (instructions), a ProvNum, and a DateStart.</summary>
	[Serializable]
	public class MedicationPat:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MedicationPatNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to medication.MedicationNum.  If 0, implies that the medication order came from eRx.  This was done to allow MU2 measures to be set by either creating a medication from the medical window, or by creating an manual prescription.</summary>
		public long MedicationNum;
		///<summary>Medication notes specific to this patient.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PatNote;
		///<summary>The last date and time this row was altered.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Date that the medication was started.  Can be minval if unknown.</summary>
		public DateTime DateStart;
		///<summary>Date that the medication was stopped.  Can be minval if unknown.  If minval, then the medication is not "discontinued".  If prior to today, then the medication is "discontinued".  If today or a future date, then not discontinued yet.</summary>
		public DateTime DateStop;
		///<summary>FK to provider.ProvNum. Can be 0. Gets set to the patient's primary provider when adding a new med.  If adding the med from EHR, gets set to the ProvNum of the logged-in user.</summary>
		public long ProvNum;
		///<summary>Only use for eRx (when MedicationNum=0).  For medication orders imported from eRx during synch.</summary>
		public string MedDescript;
		///<summary>For NewCrop medical orders, corresponds to the RxCui of the prescription (NewCrop only returns a value sometimes).  Otherwise, this field is synched with the medication.RxCui field based on medication.MedicationNum.  We should have used a string type.  The only purpose of this field is so that when CCDs are created, we have structured data to put in the XML, not just plain text.  Allergies exported in CCD do not look at this table, but only at the medication table.  Medications require MedicationPat.RxCui or Medication.RxCui to be exported on CCD.</summary>
		public long RxCui;
		///<summary>Uniquely identifies the prescription corresponding to the medical order.
		///Allows us to update existing eRx medical orders when refreshing prescriptions in the Chart (similar to how prescriptions are updated).
		///Also used in 2-way medication synching with eRx.</summary>
		public string ErxGuid;
		///<summary>If eRx is used to prescribe a medication, a medication order is imported automatically into Open Dental.  If a provider is logged in, then this is CPOE (Computerized Provider Order Entry), and this will be true.   Or, if a provider is logged in and Rx entered through OD, it's also CPOE.  If a staff person is logged in, and enters an Rx through NewCrop or OD, then this is non-CPOE, so false.</summary>
		public bool IsCpoe;

		///<summary></summary>
		public MedicationPat Clone() {
			return (MedicationPat)this.MemberwiseClone();
		}
	}


	





}










