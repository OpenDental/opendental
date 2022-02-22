using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class MedicationT {

		///<summary>Inserts the new medication and returns it.</summary>
		public static Medication CreateMedication(string medName="",string rxCui="") {
			Medication medication=new Medication();
			medication.MedName=medName;
			if(medication.MedName=="") {
				medication.MedName="Med_"+MiscUtils.CreateRandomAlphaNumericString(8);
			}
			medication.RxCui=PIn.Long(rxCui,false);
			if(medication.RxCui!=0 && RxNorms.GetByRxCUI(rxCui)==null) {
				RxNorm rxNorm=new RxNorm();
				rxNorm.RxCui=rxCui;
				rxNorm.Description=medication.MedName;
				RxNorms.Insert(rxNorm);
			}
			Medications.Insert(medication);
			return medication;
		}

		///<summary>Used to test medication merging. If isGeneric=false, you should pass in the MedicationNum of the generic medication you want</summary>
		public static Medication CreateMedicationForMerge(bool isGeneric,long medNum=0,string medName="") {
			Medication medication=new Medication();
			medication.MedName=string.IsNullOrWhiteSpace(medName) ? "Med_"+MiscUtils.CreateRandomAlphaNumericString(8) : medName;
			Medications.Insert(medication);
			medication.GenericNum=isGeneric ? medication.MedicationNum : medNum;
			Medications.Update(medication);
			return medication;
		}

		public static void ClearMedicationTable() {
			string command="DELETE FROM medication";
			DataCore.NonQ(command);
		}
	}
}
