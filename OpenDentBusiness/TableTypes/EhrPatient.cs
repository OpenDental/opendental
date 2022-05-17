using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>Patient information needed for EHR.  1:1 relation to patient table.  They are stored here because we want to try to keep the size of the patient table a bit smaller.</summary>
	[Serializable]
	public class EhrPatient:TableBase {
		///<summary>FK to patient.PatNum.  Also the primary key for this table. Always one to one relationship with patient table.  A new patient might not have an entry here until needed.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatNum;
		///<summary>Mother's maiden first name.  Exported in HL7 PID-6 for immunization messages.</summary>
		public string MotherMaidenFname;
		///<summary>Mother's maiden last name.  Exported in HL7 PID-6 for immunization messages.</summary>
		public string MotherMaidenLname;
		///<summary>Enum:YN  Indicates whether or not the patient wants to share their vaccination information with other EHRs.  Used in immunization export.</summary>
		public YN VacShareOk;
		///<summary>The abbreviation for the state for the patient's MedicaidID.
		///Displayed in patient information window, used to validate the length of the MedicaidID.</summary>
		public string MedicaidState;
		///<summary>The patient's sexual orientation. Stored as a SNOMED code or HL7 null flavor.</summary>
		public string SexualOrientation;
		///<summary>The patient's gender identity. Stored as a SNOMED code or HL7 null flavor.</summary>
		public string GenderIdentity;
		///<summary>Will be blank unless SexualOrientation is OTH, additional orientation.</summary>
		public string SexualOrientationNote;
		///<summary>Will be blank unless GenderIdentity is OTH, additional gender identity.</summary>
		public string GenderIdentityNote;

		///<summary></summary>
		public EhrPatient Clone() {
			return (EhrPatient)this.MemberwiseClone();
		}

	}

	///<summary>These enums are not stored in the database. The SNOMED codes are what is stored in the database.</summary>
	public enum SexOrientation {
		///<summary>0 - Gay or lesbian.</summary>
		[Ehr(Snomed="38628009")]
		[Description("Gay or Lesbian")]
		GayLesbian,
		///<summary>1 - Straight, heterosexual.</summary>
		[Ehr(Snomed="20430005")]
		Straight,
		///<summary>2 - Bisexual</summary>
		[Ehr(Snomed="42035005")]
		Bisexual,
		///<summary>3 - Additional sexual orientation. The description should be stored in ehrpatient.SexualOrientationNote field.</summary>
		[Ehr(Snomed="OTH")]
		[Description("Additional orientation")]
		AdditionalOrientation,
		///<summary>4 - Unknown.</summary>
		[Ehr(Snomed="UNK")]
		[Description("Don't know")]
		DontKnow,
		///<summary>5 - The patient chose not to disclose their sexual orientation.</summary>
		[Ehr(Snomed="ASKU")]
		[Description("Choose not to disclose")]
		ChooseNotToDisclose
	}

	///<summary>These enums are not stored in the database. The SNOMED codes are what is stored in the database.</summary>
	public enum GenderId {
		///<summary>0 - Male.</summary>
		[Ehr(Snomed="446151000124109")]
		Male,
		///<summary>1 - Female.</summary>
		[Ehr(Snomed="446141000124107")]
		Female,
		///<summary>2 - Transgender Male/Female-to-Male.</summary>
		[Ehr(Snomed="407377005")]
		[Description("Transgender Male/Female-to-Male")]
		TransgenderMale,
		///<summary>3 - Transgender Female/Male-to-Female.</summary>
		[Ehr(Snomed="407376001")]
		[Description("Transgender Female/Male-to-Female")]
		TransgenderFemale,
		///<summary>4 - Genderqueer, neither exclusively male nor female.</summary>
		[Ehr(Snomed="446131000124102")]
		Genderqueer,
		///<summary>5 - Additional gender identify. The description should be stored in ehrpatient.GenderIdentityNote field.</summary>
		[Ehr(Snomed="OTH")]
		[Description("Additional gender category")]
		AdditionalGenderCategory,
		///<summary>6 - The patient chose not to disclose their gender identity.</summary>
		[Ehr(Snomed="ASKU")]
		[Description("Choose not to disclose")]
		ChooseNotToDisclose
	}

	///<summary>EHR-related attributes.</summary>
	public class EhrAttribute:Attribute {
		private string _snomed="";

		///<summary>SNOMED code to be stored in the datatabase.</summary>
		public string Snomed {
			get {
				return _snomed;
			}
			set {
				_snomed=value;
			}
		}		
	}
}