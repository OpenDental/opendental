using OpenDentBusiness;
using System;
using CodeBase;

namespace UnitTestsCore {
	public class EhrPatientT {

		///<summary>Deletes everything from the ehrpatient table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEhrPatientTable() {
			string command="DELETE FROM ehrpatient";
			DataCore.NonQ(command);
		}

		public static EhrPatient CreateEhrPatient(Patient patient,string motherMaidenFname="",string motherMaidenLname="",YN vacShareOk=YN.Unknown,string medicaidState="",
			SexOrientation sexualOrientation=SexOrientation.DontKnow,GenderId genderIdentity=GenderId.ChooseNotToDisclose,string sexualOrientationNote="",string genderIdentityNote="",DateTime dischargeDate=default) 
		{
			EhrPatient ehrPatient=new EhrPatient {
				PatNum=patient.PatNum,
				MotherMaidenFname=motherMaidenFname,
				MotherMaidenLname=motherMaidenLname,
				VacShareOk=vacShareOk,
				MedicaidState=medicaidState,
				SexualOrientation=EnumTools.GetAttributeOrDefault<EhrAttribute>(sexualOrientation).Snomed,
				GenderIdentity=EnumTools.GetAttributeOrDefault<EhrAttribute>(genderIdentity).Snomed,
				SexualOrientationNote=sexualOrientationNote,
				GenderIdentityNote=genderIdentityNote,
				DischargeDate=dischargeDate
			};
			OpenDentBusiness.Crud.EhrPatientCrud.Insert(ehrPatient,useExistingPK:true); //PatNum PKs must match the EhrPatient.
			return ehrPatient;
		}

	}
}