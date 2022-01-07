using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class MedicationPatT {

		///<summary>Inserts the new medicationPat and returns it.</summary>
		public static MedicationPat CreateMedicationPat(long patNum,long medicationNum=0,DateTime dateStart=default(DateTime),
			DateTime dateStop=default(DateTime)) 
		{
			MedicationPat medPat=new MedicationPat();
			medPat.PatNum=patNum;
			medPat.MedicationNum=medicationNum;
			if(medicationNum==0) {
				medPat.MedicationNum=MedicationT.CreateMedication().MedicationNum;
			}
			medPat.DateStart=dateStart;
			medPat.DateStop=dateStop;
			MedicationPats.Insert(medPat);
			return medPat;
		}

		public static void ClearMedicationPatTable() {
			string command="DELETE FROM medicationpat";
			DataCore.NonQ(command);
		}
	}
}
